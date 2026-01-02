using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ex_cel = Microsoft.Office.Interop.Excel;
namespace Baitaplon
{
    public partial class Formhopdong : Form
    {
        SqlConnection con = new SqlConnection("Data Source=NGUYENTRUNGKIEN\\SQLEXPRESS;Initial Catalog=Baitaplon-C#;Integrated Security=True");
        public Formhopdong()
        {
            InitializeComponent();
            LoadDataToCombobox();
            LoadHopDong();
        }
        private void LoadDataToCombobox()
        {
            try
            {
                if (con.State == ConnectionState.Closed) con.Open();

                // Load Mã Phòng
                SqlDataAdapter adpPhong = new SqlDataAdapter("SELECT Maphong, Tenphong FROM Phongtro", con);
                DataTable dtPhong = new DataTable();
                adpPhong.Fill(dtPhong);
                cboMaphong.DataSource = dtPhong;
                cboMaphong.DisplayMember = "Maphong";
                cboMaphong.ValueMember = "Maphong";

                // Load Mã Khách
                SqlDataAdapter adpKhach = new SqlDataAdapter("SELECT Makhach, Hoten FROM KhachThue", con);
                DataTable dtKhach = new DataTable();
                adpKhach.Fill(dtKhach);
                cboMakhach.DataSource = dtKhach;
                cboMakhach.DisplayMember = "Makhach";
                cboMakhach.ValueMember = "Makhach";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi load danh sách phòng/khách: " + ex.Message);
            }
            finally
            {
                con.Close();
            }
}
        private void LoadHopDong()
        {
            try
            {
                // Thêm điều kiện WHERE để lọc bỏ những hợp đồng đã kết thúc
                string query = @"SELECT MaHD, Maphong, Makhach, Ngaylap, Ngaybatdau, Ngayketthuc, 
                         Tiencoc, Giathuethang, Trangthaihopdong 
                         FROM HopDong 
                         WHERE Trangthaihopdong = N'Hiệu lực'"; // Chỉ hiện hợp đồng đang chạy

                SqlDataAdapter adapter = new SqlDataAdapter(query, con);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                dgvhopdong.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải dữ liệu hợp đồng: " + ex.Message);
            }
        }
        private void ClearForm()
        {
            cboMaphong.SelectedIndex = -1;
            cboMakhach.SelectedIndex = -1;
            dtNgaylap.Value = DateTime.Now;
            dtNgaybatdau.Value = DateTime.Now;
            dtNgayketthuc.Value = DateTime.Now.AddMonths(6); // Mặc định hợp đồng 6 tháng
            txtTiencoc.Text = "0";
            txtGiathuethang.Text = "0";
            cboTrangthai.SelectedIndex = -1;
            cboMaphong.Focus();
        }

        private void btnthem_Click(object sender, EventArgs e)
        {
            if (cboMaphong.SelectedValue == null || cboMakhach.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng chọn Mã phòng và Mã khách!");
                return;
            }

            try
            {
                if (con.State == ConnectionState.Closed) con.Open();
                string query = @"INSERT INTO HopDong (Maphong, Makhach, Ngaylap, Ngaybatdau, Ngayketthuc, Tiencoc, Giathuethang, Trangthaihopdong) 
                                 VALUES (@maphong, @makhach, @ngaylap, @ngaybatdau, @ngayketthuc, @tiencoc, @giathue, @trangthai)";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@maphong", cboMaphong.SelectedValue.ToString());
                cmd.Parameters.AddWithValue("@makhach", cboMakhach.SelectedValue.ToString());
                cmd.Parameters.AddWithValue("@ngaylap", dtNgaylap.Value);
                cmd.Parameters.AddWithValue("@ngaybatdau", dtNgaybatdau.Value);
                cmd.Parameters.AddWithValue("@ngayketthuc", dtNgayketthuc.Value);
                cmd.Parameters.AddWithValue("@tiencoc", decimal.Parse(txtTiencoc.Text));
                cmd.Parameters.AddWithValue("@giathue", decimal.Parse(txtGiathuethang.Text));
                cmd.Parameters.AddWithValue("@trangthai", cboTrangthai.Text);

                cmd.ExecuteNonQuery();
                MessageBox.Show("Thêm hợp đồng thành công!");
                LoadHopDong();
                ClearForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi thêm: " + ex.Message);
            }
            finally { con.Close(); }
        }

        private void dgvhopdong_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvhopdong.Rows[e.RowIndex];
                // Lấy MaHD ẩn hoặc từ cột đầu tiên để dùng cho việc Sửa/Xóa
                string currentMaHD = row.Cells["MaHD"].Value.ToString();

                // Đổ dữ liệu vào ComboBox
                cboMaphong.SelectedValue = row.Cells["Maphong"].Value.ToString();
                cboMakhach.SelectedValue = row.Cells["Makhach"].Value.ToString();
                dtNgaylap.Value = Convert.ToDateTime(row.Cells["Ngaylap"].Value);
                dtNgaybatdau.Value = Convert.ToDateTime(row.Cells["Ngaybatdau"].Value);
                dtNgayketthuc.Value = row.Cells["Ngayketthuc"].Value != DBNull.Value ? Convert.ToDateTime(row.Cells["Ngayketthuc"].Value) : DateTime.Now;
                txtTiencoc.Text = row.Cells["Tiencoc"].Value.ToString();
                txtGiathuethang.Text = row.Cells["Giathuethang"].Value.ToString();
                cboTrangthai.Text = row.Cells["Trangthaihopdong"].Value.ToString();

                // Lưu MaHD vào Tag của Form hoặc biến tạm để Sửa/Xóa vì không còn ô txtMaHD
                this.Tag = currentMaHD;
            }
        }

        private void btnsua_Click(object sender, EventArgs e)
        {
            if (this.Tag == null)
            {
                MessageBox.Show("Chọn hợp đồng trong danh sách trước khi sửa!");
                return;
            }

            try
            {
                if (con.State == ConnectionState.Closed) con.Open();
                string query = @"UPDATE HopDong SET Maphong=@maphong, Makhach=@makhach, Ngaylap=@ngaylap, 
                                 Ngaybatdau=@ngaybatdau, Ngayketthuc=@ngayketthuc, Tiencoc=@tiencoc, 
                                 Giathuethang=@giathue, Trangthaihopdong=@trangthai WHERE MaHD=@mahd";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@mahd", this.Tag.ToString());
                cmd.Parameters.AddWithValue("@maphong", cboMaphong.SelectedValue.ToString());
                cmd.Parameters.AddWithValue("@makhach", cboMakhach.SelectedValue.ToString());
                cmd.Parameters.AddWithValue("@ngaylap", dtNgaylap.Value);
                cmd.Parameters.AddWithValue("@ngaybatdau", dtNgaybatdau.Value);
                cmd.Parameters.AddWithValue("@ngayketthuc", dtNgayketthuc.Value);
                cmd.Parameters.AddWithValue("@tiencoc", decimal.Parse(txtTiencoc.Text));
                cmd.Parameters.AddWithValue("@giathue", decimal.Parse(txtGiathuethang.Text));
                cmd.Parameters.AddWithValue("@trangthai", cboTrangthai.Text);

                cmd.ExecuteNonQuery();
                MessageBox.Show("Cập nhật thành công!");
                LoadHopDong();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
            finally { con.Close(); }
        }

        private void btnxoa_Click(object sender, EventArgs e)
        {
            if (this.Tag == null)
            {
                MessageBox.Show("Vui lòng chọn hợp đồng cần xóa!");
                return;
            }

            if (MessageBox.Show("Bạn có chắc muốn xóa vĩnh viễn hợp đồng này và các dữ liệu liên quan?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                try
                {
                    if (con.State == ConnectionState.Closed) con.Open();

                    // Cách triệt để: Xóa hóa đơn liên quan trước để tránh lỗi ràng buộc
                    string deleteHoaDon = "DELETE FROM HoaDon WHERE MaHD = @mahd";
                    SqlCommand cmd1 = new SqlCommand(deleteHoaDon, con);
                    cmd1.Parameters.AddWithValue("@mahd", this.Tag.ToString());
                    cmd1.ExecuteNonQuery();

                    // Sau đó mới xóa hợp đồng
                    string deleteHopDong = "DELETE FROM HopDong WHERE MaHD = @mahd";
                    SqlCommand cmd2 = new SqlCommand(deleteHopDong, con);
                    cmd2.Parameters.AddWithValue("@mahd", this.Tag.ToString());
                    cmd2.ExecuteNonQuery();

                    MessageBox.Show("Xóa thành công!");
                    LoadHopDong();
                    ClearForm();
                    this.Tag = null;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi xóa: " + ex.Message);
                }
                finally { con.Close(); }
            }
        }

        private void btnmoi_Click(object sender, EventArgs e)
        {
            LoadHopDong();
            ClearForm();
            txttk.Clear();
        }

        private void btntk_Click(object sender, EventArgs e)
        {
            try
            {
                // 1. Kiểm tra nếu ô tìm kiếm trống thì tải lại toàn bộ danh sách
                if (string.IsNullOrWhiteSpace(txttk.Text))
                {
                    LoadHopDong();
                    return;
                }

                // 2. Viết câu lệnh SQL tìm kiếm (LIKE giúp tìm kiếm gần đúng)
                // Tìm theo Mã phòng, Mã khách hoặc Trạng thái hợp đồng
                string query = @"SELECT MaHD, Maphong, Makhach, Ngaylap, Ngaybatdau, Ngayketthuc, Tiencoc, Giathuethang, Trangthaihopdong 
                         FROM HopDong 
                         WHERE Maphong LIKE @tk 
                         OR CAST(Makhach AS NVARCHAR) LIKE @tk 
                         OR Trangthaihopdong LIKE @tk";

                if (con.State == ConnectionState.Closed) con.Open();

                SqlCommand cmd = new SqlCommand(query, con);
                // Thêm dấu % để tìm kiếm mọi vị trí (ví dụ: gõ "10" sẽ ra phòng "P101", "P102")
                cmd.Parameters.AddWithValue("@tk", "%" + txttk.Text.Trim() + "%");

                SqlDataAdapter adp = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adp.Fill(dt);

                // 3. Hiển thị kết quả lên DataGridView
                dgvhopdong.DataSource = dt;

                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("Không tìm thấy kết quả nào khớp với từ khóa!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tìm kiếm: " + ex.Message);
            }
            finally
            {
                con.Close();
            }
        }

        private void btnxuat_Click(object sender, EventArgs e)
        {
            DataTable tb = (DataTable)dgvhopdong.DataSource;
            if (tb == null || tb.Rows.Count == 0) return;

            try
            {
                ex_cel.Application oExcel = new ex_cel.Application();
                ex_cel.Workbook oBook = oExcel.Workbooks.Add(Type.Missing);
                ex_cel.Worksheet oSheet = (ex_cel.Worksheet)oBook.Worksheets[1];
                oExcel.Visible = true;

                // Header lớn
                ex_cel.Range head = oSheet.get_Range("A1", "I1");
                head.MergeCells = true;
                head.Value2 = "DANH SÁCH HỢP ĐỒNG THUÊ PHÒNG";
                head.Font.Bold = true; head.Font.Size = 16;
                head.HorizontalAlignment = ex_cel.XlHAlign.xlHAlignCenter;

                // Tiêu đề cột
                string[] headers = { "Mã HD", "Mã Phòng", "Mã Khách", "Ngày Lập", "Bắt Đầu", "Kết Thúc", "Tiền Cọc", "Giá Thuê", "Trạng Thái" };
                for (int i = 0; i < headers.Length; i++)
                {
                    ex_cel.Range cl = (ex_cel.Range)oSheet.Cells[3, i + 1];
                    cl.Value2 = headers[i];
                    cl.Font.Bold = true;
                    cl.Interior.ColorIndex = 15;
                    cl.Borders.LineStyle = ex_cel.Constants.xlSolid;
                }

                // Dữ liệu
                object[,] arr = new object[tb.Rows.Count, tb.Columns.Count];
                for (int r = 0; r < tb.Rows.Count; r++)
                    for (int c = 0; c < tb.Columns.Count; c++)
                        arr[r, c] = tb.Rows[r][c];

                ex_cel.Range range = oSheet.get_Range("A4", "I" + (tb.Rows.Count + 3));
                range.Value2 = arr;
                range.Borders.LineStyle = ex_cel.Constants.xlSolid;
                oSheet.Columns.AutoFit();

                MessageBox.Show("Xuất Excel thành công!");
            }
            catch (Exception ex) 
            { 
                MessageBox.Show("Lỗi Excel: " + ex.Message); 
            }
        }
    }
}
