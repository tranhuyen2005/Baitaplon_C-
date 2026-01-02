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
    public partial class Formkhachthue : Form
    {
        SqlConnection con = new SqlConnection("Data Source=NGUYENTRUNGKIEN\\SQLEXPRESS;Initial Catalog=Baitaplon-C#;Integrated Security=True");
        string maKhachDangChon = "";
        public Formkhachthue()
        {
            InitializeComponent();
            LoadKhachThue();
        }
        private void LoadKhachThue()
        {
            try
            {
                // Truy vấn bảng KhachThue
                SqlDataAdapter adapter = new SqlDataAdapter("SELECT Makhach, Hoten, CCCD, SDT, Gioitinh, Ngaysinh, Quequan FROM KhachThue", con);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                dgvkhachthue.DataSource = dt; // Giả sử tên DataGridView của bạn là dgvKhachThue
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể tải dữ liệu khách thuê: " + ex.Message);
            }
        }
        private void ClearFormKhachThue()
        {
            maKhachDangChon = ""; // Ô này thường để trống vì DB tự tăng
            txtHoTen.Clear();
            txtCCCD.Clear();
            txtSDT.Clear();
            txtQueQuan.Clear();
            cboGioiTinh.SelectedIndex = -1;
            dtpNgaySinh.Value = DateTime.Now;
            txtHoTen.Focus();
        }
        private bool KiemTraTrungCCCD(string cccd)
        {
            bool exists = false;
            try
            {
                if (con.State == ConnectionState.Closed) con.Open();
                // Đếm xem có bao nhiêu dòng có số CCCD này
                string query = "SELECT COUNT(*) FROM KhachThue WHERE CCCD = @cccd";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@cccd", cccd);

                int count = (int)cmd.ExecuteScalar();
                if (count > 0) exists = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi kiểm tra CCCD: " + ex.Message);
            }
            finally
            {
                con.Close();
            }
            return exists;
        }
        private void btnthem_Click(object sender, EventArgs e)
        {
            // Kiểm tra không để trống các trường quan trọng (Họ tên và CCCD)
            if (string.IsNullOrWhiteSpace(txtHoTen.Text) || string.IsNullOrWhiteSpace(txtCCCD.Text))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ Họ tên và CCCD!", "Thông báo");
                return;
            }
            // 2. KIỂM TRA TRÙNG CCCD TẠI ĐÂY
            if (KiemTraTrungCCCD(txtCCCD.Text.Trim()))
            {
                MessageBox.Show("Số CCCD này đã tồn tại trong hệ thống. Vui lòng kiểm tra lại!", "Lỗi nhập liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCCCD.Focus(); // Đưa con trỏ về ô CCCD để người dùng sửa
                return;
            }
            try
            {
                if (con.State == ConnectionState.Closed) con.Open();

                // Lưu ý: Không chèn cột Makhach vì trong SQL bạn để IDENTITY(1,1)
                string query = @"INSERT INTO KhachThue (Hoten, CCCD, SDT, Gioitinh, Ngaysinh, Quequan) 
                                 VALUES (@hoten, @cccd, @sdt, @gioitinh, @ngaysinh, @quequan)";

                SqlCommand cmd = new SqlCommand(query, con);

                // Gán tham số
                
                cmd.Parameters.AddWithValue("@hoten", txtHoTen.Text.Trim());
                cmd.Parameters.AddWithValue("@cccd", txtCCCD.Text.Trim());
                cmd.Parameters.AddWithValue("@sdt", txtSDT.Text.Trim());
                cmd.Parameters.AddWithValue("@gioitinh", cboGioiTinh.Text); // Lấy từ ComboBox
                cmd.Parameters.AddWithValue("@ngaysinh", dtpNgaySinh.Value); // Lấy từ DateTimePicker
                cmd.Parameters.AddWithValue("@quequan", txtQueQuan.Text.Trim());

                int result = cmd.ExecuteNonQuery();
                if (result > 0)
                {
                    MessageBox.Show("Đã thêm khách thuê: " + txtHoTen.Text, "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadKhachThue(); // Tải lại bảng
                    ClearFormKhachThue(); // Xóa trắng form
                }
            }
            catch (Exception ex)
            {
                // Kiểm tra lỗi trùng CCCD (vì trong SQL có ràng buộc UNIQUE)
                if (ex.Message.Contains("CCCD") || ex.Message.Contains("unique"))
                    MessageBox.Show("Số CCCD này đã tồn tại!", "Lỗi");
                else
                    MessageBox.Show("Lỗi: " + ex.Message);
            }
            finally
            {
                con.Close();
            }
        }

        private void dgvkhachthue_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvkhachthue.Rows[e.RowIndex];
                maKhachDangChon = row.Cells["Makhach"].Value.ToString();
                txtHoTen.Text = row.Cells["Hoten"].Value.ToString();
                txtCCCD.Text = row.Cells["CCCD"].Value.ToString();
                txtSDT.Text = row.Cells["SDT"].Value.ToString();
                cboGioiTinh.Text = row.Cells["Gioitinh"].Value.ToString();
                dtpNgaySinh.Value = Convert.ToDateTime(row.Cells["Ngaysinh"].Value);
                txtQueQuan.Text = row.Cells["Quequan"].Value.ToString();
            }
        }

        private void btnsua_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(maKhachDangChon))
            {
                MessageBox.Show("Hãy chọn một khách thuê trong danh sách để sửa!");
                return;
            }

            try
            {
                if (con.State == ConnectionState.Closed) con.Open();
                string query = @"UPDATE KhachThue 
                                 SET Hoten=@hoten, CCCD=@cccd, SDT=@sdt, Gioitinh=@gioitinh, Ngaysinh=@ngaysinh, Quequan=@quequan 
                                 WHERE Makhach=@makhach";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@makhach", maKhachDangChon); // Dùng biến tạm làm điều kiện
                cmd.Parameters.AddWithValue("@hoten", txtHoTen.Text.Trim());
                cmd.Parameters.AddWithValue("@cccd", txtCCCD.Text.Trim());
                cmd.Parameters.AddWithValue("@sdt", txtSDT.Text.Trim());
                cmd.Parameters.AddWithValue("@gioitinh", cboGioiTinh.Text);
                cmd.Parameters.AddWithValue("@ngaysinh", dtpNgaySinh.Value);
                cmd.Parameters.AddWithValue("@quequan", txtQueQuan.Text.Trim());

                cmd.ExecuteNonQuery();
                MessageBox.Show("Cập nhật thông tin thành công!");
                LoadKhachThue();
                ClearFormKhachThue();
            }
            catch (Exception ex) { MessageBox.Show("Lỗi khi sửa: " + ex.Message); }
            finally { con.Close(); }
        }

        private void btnxoa_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(maKhachDangChon))
            {
                MessageBox.Show("Vui lòng chọn khách thuê cần xóa!");
                return;
            }

            DialogResult dr = MessageBox.Show("Bạn có chắc chắn muốn xóa khách thuê này không?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (dr == DialogResult.Yes)
            {
                try
                {
                    if (con.State == ConnectionState.Closed) con.Open();
                    string query = "DELETE FROM KhachThue WHERE Makhach = @makhach";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@makhach", maKhachDangChon);

                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Đã xóa khách thuê thành công!");
                    LoadKhachThue();
                    ClearFormKhachThue();
                }
                catch (Exception)
                {
                    // Lỗi này thường xảy ra nếu khách này đang có Hợp đồng (khóa ngoại)
                    MessageBox.Show("Không thể xóa khách này vì dữ liệu đang được sử dụng ở bảng khác!");
                }
                finally { con.Close(); }
            }
        }

        private void btnmoi_Click(object sender, EventArgs e)
        {
            // 1. Đổ dữ liệu vào bảng trước
            LoadKhachThue();

            txttk.Clear();

            // 2. Hủy chọn dòng trong bảng (Để nó không tự động nạp dữ liệu lên TextBox)
            dgvkhachthue.ClearSelection();
            dgvkhachthue.CurrentCell = null;

            // 3. Xóa trắng form SAU CÙNG
            ClearFormKhachThue();

            // 4. Thông báo
            MessageBox.Show("Dữ liệu đã được làm mới!", "Thông báo");
        }

        private void btntk_Click(object sender, EventArgs e)
        {
            try
            {
                if (con.State == ConnectionState.Closed) con.Open();

                // Tìm kiếm trên TẤT CẢ các cột thông tin
                string query = @"SELECT Makhach, Hoten, CCCD, SDT, Gioitinh, Ngaysinh, Quequan 
                         FROM KhachThue 
                         WHERE Hoten LIKE @search 
                            OR CCCD LIKE @search 
                            OR SDT LIKE @search
                            OR Gioitinh LIKE @search
                            OR Quequan LIKE @search
                            OR CONVERT(VARCHAR, Ngaysinh, 103) LIKE @search";
                // 103 là định dạng dd/mm/yyyy trong SQL

                SqlDataAdapter adapter = new SqlDataAdapter(query, con);
                adapter.SelectCommand.Parameters.AddWithValue("@search", "%" + txttk.Text.Trim() + "%");

                DataTable dt = new DataTable();
                adapter.Fill(dt);
                dgvkhachthue.DataSource = dt;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi tìm kiếm: " + ex.Message);
            }
            finally
            {
                con.Close();
            }
        }

        private void btnxuat_Click(object sender, EventArgs e)
        {
            // 1. Lấy dữ liệu hiện tại từ DataGridView ra DataTable
            DataTable tb = (DataTable)dgvkhachthue.DataSource;

            if (tb == null || tb.Rows.Count == 0)
            {
                MessageBox.Show("Không có dữ liệu để xuất!", "Thông báo");
                return;
            }

            // 2. Khởi tạo các đối tượng Excel
            ex_cel.Application oExcel = new ex_cel.Application();
            ex_cel.Workbooks oBooks;
            ex_cel.Sheets oSheets;
            ex_cel.Workbook oBook;
            ex_cel.Worksheet oSheet;

            // 3. Thiết lập môi trường Excel
            oExcel.Visible = true;
            oExcel.DisplayAlerts = false;
            oExcel.Application.SheetsInNewWorkbook = 1;
            oBooks = oExcel.Workbooks;
            oBook = (ex_cel.Workbook)(oExcel.Workbooks.Add(Type.Missing));
            oSheets = oBook.Worksheets;
            oSheet = (ex_cel.Worksheet)oSheets.get_Item(1);
            oSheet.Name = "Danh sach khach thue";

            // 4. Tạo Tiêu đề trang (Header lớn)
            ex_cel.Range head = oSheet.get_Range("A1", "G1");
            head.MergeCells = true;
            head.Value2 = "DANH SÁCH KHÁCH THUÊ PHÒNG";
            head.Font.Bold = true;
            head.Font.Name = "Tahoma";
            head.Font.Size = "18";
            head.HorizontalAlignment = ex_cel.XlHAlign.xlHAlignCenter;

            // 5. Tạo tiêu đề các cột (Header bảng) - Ở dòng 3
            string[] headers = { "Mã khách", "Họ và tên", "CCCD", "Điện thoại", "Giới tính", "Ngày sinh", "Quê quán" };
            double[] widths = { 10.0, 30.0, 20.0, 20.0, 15.0, 20.0, 30.0 };

            for (int i = 0; i < headers.Length; i++)
            {
                ex_cel.Range cl = (ex_cel.Range)oSheet.Cells[3, i + 1];
                cl.Value2 = headers[i];
                cl.ColumnWidth = widths[i];
                cl.Font.Bold = true;
                cl.Interior.ColorIndex = 15; // Màu nền xám nhạt giống mẫu
                cl.Borders.LineStyle = ex_cel.Constants.xlSolid;
                cl.HorizontalAlignment = ex_cel.XlHAlign.xlHAlignCenter;
            }

            // 6. Chuyển dữ liệu từ DataTable vào mảng Object (để đổ dữ liệu nhanh)
            object[,] arr = new object[tb.Rows.Count, tb.Columns.Count];
            for (int r = 0; r < tb.Rows.Count; r++)
            {
                DataRow dr = tb.Rows[r];
                for (int c = 0; c < tb.Columns.Count; c++)
                {
                    // Cột CCCD (index 2) và SDT (index 3) thêm dấu nháy đơn để không mất số 0
                    if (c == 2 || c == 3)
                        arr[r, c] = "'" + dr[c].ToString();
                    else
                        arr[r, c] = dr[c];
                }
            }

            // 7. Thiết lập vùng điền dữ liệu (Bắt đầu từ dòng 4)
            int rowStart = 4;
            int columnStart = 1;
            int rowEnd = rowStart + tb.Rows.Count - 1;
            int columnEnd = tb.Columns.Count;

            ex_cel.Range c1 = (ex_cel.Range)oSheet.Cells[rowStart, columnStart];
            ex_cel.Range c2 = (ex_cel.Range)oSheet.Cells[rowEnd, columnEnd];
            ex_cel.Range range = oSheet.get_Range(c1, c2);

            // 8. Đổ dữ liệu vào và định dạng
            range.Value2 = arr; // Đổ toàn bộ mảng dữ liệu vào Excel 1 lần (rất nhanh)
            range.Borders.LineStyle = ex_cel.Constants.xlSolid; // Kẻ viền cho phần dữ liệu

            // Căn giữa cột Mã khách (Cột A) và Giới tính (Cột E)
            oSheet.get_Range("A" + rowStart, "A" + rowEnd).HorizontalAlignment = ex_cel.XlHAlign.xlHAlignCenter;
            oSheet.get_Range("E" + rowStart, "E" + rowEnd).HorizontalAlignment = ex_cel.XlHAlign.xlHAlignCenter;

            // 9. Định dạng ngày sinh (Cột F)
            ex_cel.Range cl_ngs = oSheet.get_Range("F" + rowStart, "F" + rowEnd);
            cl_ngs.NumberFormat = "dd/mm/yyyy";

            MessageBox.Show("Xuất dữ liệu thành công!", "Thông báo");
        }
    }
}
