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
    public partial class Hoadon : Form
    {
        SqlConnection con = new SqlConnection("Data Source=NGUYENTRUNGKIEN\\SQLEXPRESS;Initial Catalog=Baitaplon-C#;Integrated Security=True");
        int maHDDangChon = -1; // Biến toàn cục lưu trữ mã hóa đơn đang được chọn từ bảng;
        public Hoadon()
        {
            InitializeComponent();

            LoadHoaDon();
            LoadDataComboBox(); // Gọi hàm đổ dữ liệu vào các ComboBox khi mở form
        }
        // 1. Hàm load dữ liệu ComboBox (Mã hợp đồng, Tháng, Năm)
        private void LoadDataComboBox()
        {
            try
            {
                // Load danh sách Mã hợp đồng từ bảng HopDong
                string sqlHD = "SELECT MaHD FROM HopDong";
                SqlDataAdapter da = new SqlDataAdapter(sqlHD, con);
                DataTable dt = new DataTable();
                da.Fill(dt);
                cbMaHopdong.DataSource = dt;
                cbMaHopdong.DisplayMember = "MaHD";
                cbMaHopdong.ValueMember = "MaHD";
                cbMaHopdong.SelectedIndex = -1; // Không chọn gì lúc mới đầu

                // Thêm thủ công dữ liệu cho Tháng (1-12)
                cbThang.Items.Clear();
                for (int i = 1; i <= 12; i++) cbThang.Items.Add(i.ToString());

                // Thêm thủ công dữ liệu cho Năm
                cbNam.Items.Clear();
                int namHienTai = DateTime.Now.Year;
                for (int i = namHienTai - 2; i <= namHienTai + 2; i++) cbNam.Items.Add(i.ToString());
            }
            catch (Exception ex) { MessageBox.Show("Lỗi load ComboBox: " + ex.Message); }
        }
        private void LoadHoaDon()
        {
            // Thêm điều kiện WHERE để chỉ lấy hóa đơn chưa thanh toán
            string sql = @"
        SELECT  
            hd.MaHD,
            hd.MaHopdong, 
            h.Maphong, 
            hd.Thang,
            hd.Nam,
            hd.Ngaylap,
            hd.Tongtien,
            hd.Trangthai
        FROM HoaDon hd
        INNER JOIN HopDong h ON hd.MaHopdong = h.MaHD
        WHERE hd.Trangthai = N'Chưa trả'"; // Chỉ hiện hóa đơn chưa trả

            SqlDataAdapter da = new SqlDataAdapter(sql, con);
            DataTable dt = new DataTable();
            da.Fill(dt);
            dgvHoadon.DataSource = dt;
        }
        private void dgvHoadon_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvHoadon.Rows[e.RowIndex];

                maHDDangChon = int.Parse(row.Cells["MaHD"].Value.ToString());

                // Sử dụng .Text hoặc .SelectedValue cho ComboBox
                cbMaHopdong.Text = row.Cells["MaHopdong"].Value.ToString();
                cbThang.Text = row.Cells["Thang"].Value?.ToString();
                cbNam.Text = row.Cells["Nam"].Value?.ToString();

                if (DateTime.TryParse(row.Cells["Ngaylap"].Value?.ToString(), out DateTime ngayLap))
                    dtNgayLap.Value = ngayLap;

                txtTongTien.Text = row.Cells["Tongtien"].Value?.ToString();
                cbTrangThai.Text = row.Cells["Trangthai"].Value?.ToString();

                cbMaHopdong.Enabled = false; // Khóa mã hợp đồng khi đang xem/sửa
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if (maHDDangChon == -1)
            {
                MessageBox.Show("Vui lòng chọn hóa đơn cần sửa!");
                return;
            }

            try
            {
                string sql = @"UPDATE HoaDon 
                               SET Thang = @Thang, Nam = @Nam, Ngaylap = @Ngaylap, 
                                   Tongtien = @Tongtien, Trangthai = @Trangthai 
                               WHERE MaHD = @MaHD";

                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@Thang", cbThang.Text);
                cmd.Parameters.AddWithValue("@Nam", cbNam.Text);
                cmd.Parameters.AddWithValue("@Ngaylap", dtNgayLap.Value);
                cmd.Parameters.AddWithValue("@Tongtien", decimal.Parse(txtTongTien.Text));
                cmd.Parameters.AddWithValue("@Trangthai", cbTrangThai.Text);
                cmd.Parameters.AddWithValue("@MaHD", maHDDangChon);

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();

                MessageBox.Show("Cập nhật thành công!");
                LoadHoaDon();
            }
            catch (Exception ex)
            {
                con.Close();
                MessageBox.Show("Lỗi khi sửa: " + ex.Message);
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (maHDDangChon == -1)
            {
                MessageBox.Show("Vui lòng chọn hóa đơn cần xóa!");
                return;
            }

            DialogResult dr = MessageBox.Show("Bạn có muốn xóa hóa đơn này?", "Xác nhận", MessageBoxButtons.YesNo);
            if (dr == DialogResult.Yes)
            {
                try
                {
                    con.Open();
                    // Xóa chi tiết trước để tránh lỗi khóa ngoại
                    string sqlDelCT = "DELETE FROM ChiTietHoaDon WHERE MaHoaDon = @MaHD";
                    SqlCommand cmdCT = new SqlCommand(sqlDelCT, con);
                    cmdCT.Parameters.AddWithValue("@MaHD", maHDDangChon);
                    cmdCT.ExecuteNonQuery();

                    // Xóa hóa đơn
                    string sqlDelHD = "DELETE FROM HoaDon WHERE MaHD = @MaHD";
                    SqlCommand cmdHD = new SqlCommand(sqlDelHD, con);
                    cmdHD.Parameters.AddWithValue("@MaHD", maHDDangChon);
                    cmdHD.ExecuteNonQuery();
                    con.Close();

                    MessageBox.Show("Xóa thành công!");
                    LoadHoaDon();
                    btnLammoi_Click(null, null);
                }
                catch (Exception ex) { con.Close(); MessageBox.Show("Lỗi xóa: " + ex.Message); }
            }

        }

        private void btnTimkiem_Click(object sender, EventArgs e)
        {
            try
            {
                string sql = @"
                    SELECT hd.MaHD, hd.MaHopdong, h.Maphong, hd.Thang, hd.Nam, hd.Ngaylap, hd.Tongtien, hd.Trangthai
                    FROM HoaDon hd
                    JOIN HopDong h ON hd.MaHopdong = h.MaHD
                    WHERE h.Maphong LIKE @Search OR CAST(hd.MaHopdong AS NVARCHAR) LIKE @Search";

                SqlDataAdapter da = new SqlDataAdapter(sql, con);
                da.SelectCommand.Parameters.AddWithValue("@Search", "%" + txtTimKiem.Text.Trim() + "%");
                DataTable dt = new DataTable();
                da.Fill(dt);
                dgvHoadon.DataSource = dt;
            }
            catch (Exception ex) { MessageBox.Show("Lỗi tìm kiếm: " + ex.Message); }
        }

        private void btnLammoi_Click(object sender, EventArgs e)
        {
            maHDDangChon = -1;
            cbMaHopdong.SelectedIndex = -1;
            cbMaHopdong.Enabled = true;
            cbThang.SelectedIndex = -1;
            cbNam.SelectedIndex = -1;
            txtTongTien.Clear();
            cbTrangThai.SelectedIndex = -1;
            dtNgayLap.Value = DateTime.Now;
            LoadHoaDon();
            txtTimKiem.Clear();
        }

        private void btnThemhoadon_Click_1(object sender, EventArgs e)
        {
            if (cbMaHopdong.SelectedIndex == -1 || string.IsNullOrEmpty(txtTongTien.Text))
            {
                MessageBox.Show("Vui lòng chọn Mã hợp đồng và nhập Tổng tiền!");
                return;
            }

            try
            {
                string sql = @"INSERT INTO HoaDon (MaHopdong, Thang, Nam, Ngaylap, Tongtien, Trangthai)
                               VALUES (@MaHopdong, @Thang, @Nam, @Ngaylap, @Tongtien, @Trangthai)";

                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@MaHopdong", cbMaHopdong.Text);
                cmd.Parameters.AddWithValue("@Thang", cbThang.Text);
                cmd.Parameters.AddWithValue("@Nam", cbNam.Text);
                cmd.Parameters.AddWithValue("@Ngaylap", dtNgayLap.Value);
                cmd.Parameters.AddWithValue("@Tongtien", decimal.Parse(txtTongTien.Text));
                cmd.Parameters.AddWithValue("@Trangthai", cbTrangThai.Text);

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();

                MessageBox.Show("Thêm hóa đơn thành công!");
                LoadHoaDon();
                btnLammoi_Click(null, null);
            }
            catch (Exception ex)
            {
                con.Close();
                MessageBox.Show("Lỗi khi thêm: " + ex.Message);
            }
        }

        private void btnChitiethoadon_Click(object sender, EventArgs e)
        {
            if (dgvHoadon.CurrentRow != null)
            {
                // Lấy mã hóa đơn từ dòng đang chọn
                int maHD = Convert.ToInt32(dgvHoadon.CurrentRow.Cells["MaHD"].Value);

                // Khởi tạo form chi tiết và truyền mã hóa đơn sang
                Chitiethoadon frmChiTiet = new Chitiethoadon(maHD);
                frmChiTiet.ShowDialog();

                // Load lại dữ liệu nếu cần sau khi đóng form chi tiết
                LoadHoaDon();
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một hóa đơn!");
            }
        }

        private void btnXuat_Click(object sender, EventArgs e)
        {
            // Lấy DataTable từ DataGridView (Giả sử tên DGV của bạn là dgvHoadon)
            // Nếu bạn dùng DataSource là DataTable trực tiếp:
            DataTable dt = (DataTable)dgvHoadon.DataSource;

            if (dt == null || dt.Rows.Count == 0)
            {
                MessageBox.Show("Không có dữ liệu hóa đơn để xuất!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            ExportHoaDonToExcel(dt);
        }
        public void ExportHoaDonToExcel(DataTable tb)
        {
            try
            {
                // 1. Khởi tạo các đối tượng Excel
                ex_cel.Application oExcel = new ex_cel.Application();
                ex_cel.Workbook oBook = oExcel.Workbooks.Add(Type.Missing);
                ex_cel.Worksheet oSheet = (ex_cel.Worksheet)oBook.Worksheets.get_Item(1);
                oExcel.Visible = true;
                oSheet.Name = "DanhSachHoaDon";

                // 2. Tạo tiêu đề trang hóa đơn
                ex_cel.Range head = oSheet.get_Range("A1", "H1");
                head.MergeCells = true;
                head.Value2 = "DANH SÁCH TỔNG HỢP HÓA ĐƠN TIỀN PHÒNG";
                head.Font.Bold = true;
                head.Font.Name = "Tahoma";
                head.Font.Size = 18;
                head.HorizontalAlignment = ex_cel.XlHAlign.xlHAlignCenter;

                // 3. Tạo tiêu đề cột dựa trên giao diện của bạn
                string[] headers = { "Mã HĐ", "Mã Phòng", "Mã HĐồng", "Tháng", "Năm", "Ngày Lập", "Tổng Tiền", "Trạng Thái" };
                for (int i = 0; i < headers.Length; i++)
                {
                    ex_cel.Range cl = (ex_cel.Range)oSheet.Cells[3, i + 1];
                    cl.Value2 = headers[i];
                    cl.Font.Bold = true;
                    cl.Borders.LineStyle = ex_cel.Constants.xlSolid;
                    cl.Interior.ColorIndex = 15; // Màu xám nhẹ cho header
                    cl.HorizontalAlignment = ex_cel.XlHAlign.xlHAlignCenter;
                }

                // Thiết lập độ rộng cột cho đẹp
                oSheet.get_Range("A3").ColumnWidth = 10;
                oSheet.get_Range("B3").ColumnWidth = 12;
                oSheet.get_Range("F3").ColumnWidth = 15; // Ngày lập
                oSheet.get_Range("G3").ColumnWidth = 18; // Tổng tiền
                oSheet.get_Range("H3").ColumnWidth = 15; // Trạng thái

                // 4. Chuyển dữ liệu từ DataTable vào mảng đối tượng (Object Array) để tối ưu tốc độ
                // Lưu ý: Tên cột phải khớp với DataPropertyName trong ảnh bạn gửi (image_13e2a7.jpg)
                object[,] arr = new object[tb.Rows.Count, 8];
                for (int r = 0; r < tb.Rows.Count; r++)
                {
                    DataRow dr = tb.Rows[r];
                    arr[r, 0] = dr["MaHD"];
                    arr[r, 1] = dr["Maphong"];
                    arr[r, 2] = dr["MaHopdong"];
                    arr[r, 3] = dr["Thang"];
                    arr[r, 4] = dr["Nam"];
                    arr[r, 5] = dr["Ngaylap"];
                    arr[r, 6] = dr["Tongtien"];
                    arr[r, 7] = dr["Trangthai"];
                }

                // 5. Thiết lập vùng đổ dữ liệu
                int rowStart = 4;
                int columnStart = 1;
                int rowEnd = rowStart + tb.Rows.Count - 1;
                int columnEnd = 8;

                ex_cel.Range c1 = (ex_cel.Range)oSheet.Cells[rowStart, columnStart];
                ex_cel.Range c2 = (ex_cel.Range)oSheet.Cells[rowEnd, columnEnd];
                ex_cel.Range range = oSheet.get_Range(c1, c2);

                // Đổ toàn bộ mảng dữ liệu vào Excel một lần duy nhất
                range.Value2 = arr;

                // Kẻ viền và định dạng
                range.Borders.LineStyle = ex_cel.Constants.xlSolid;

                // Căn giữa các cột số và tháng/năm
                oSheet.get_Range("A4", "E" + rowEnd).HorizontalAlignment = ex_cel.XlHAlign.xlHAlignCenter;

                // Định dạng cột Tổng tiền có dấu phân cách nghìn
                oSheet.get_Range("G4", "G" + rowEnd).NumberFormat = "#,##0";

                MessageBox.Show("Xuất danh sách hóa đơn ra Excel thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi hệ thống khi xuất Excel: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}

