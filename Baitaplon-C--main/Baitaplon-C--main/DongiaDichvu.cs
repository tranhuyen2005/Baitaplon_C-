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
    public partial class DongiaDichvu : Form
    {
        SqlConnection con = new SqlConnection("Data Source=NGUYENTRUNGKIEN\\SQLEXPRESS;Initial Catalog=Baitaplon-C#;Integrated Security= True");
        public DongiaDichvu()
        {
            InitializeComponent();
            LoadDataDichVu();
        }
        // Hàm hiển thị dữ liệu lên DataGridView
        void LoadDataDichVu()
        {
            try
            {
                SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM Dichvu", con);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                dgvDichVu.DataSource = dt; // Đổ dữ liệu vào bảng hiển thị
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể tải dữ liệu: " + ex.Message);
            }
        }

        // Hàm làm sạch các ô nhập sau khi thêm thành công
        void ClearFormDichVu()
        {
            txtMaDichVu.Clear();
            txtTenDichVu.Clear();
            txtDonViTinh.Clear();
            txtDonGia.Text = "0";
            txtMaDichVu.Focus(); // Đưa con trỏ chuột về ô đầu tiên
        }
        

        
        private void dgvDichVu_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Kiểm tra nếu người dùng nhấn vào dòng tiêu đề (header) thì bỏ qua
            if (e.RowIndex >= 0)
            {
                // Lấy dòng hiện tại được chọn
                DataGridViewRow row = dgvDichVu.Rows[e.RowIndex];

                // Gán dữ liệu từ các cột của dòng đó vào các TextBox tương ứng
                // Lưu ý: Thay "ColumnName" bằng tên thực tế của cột hoặc số thứ tự cột (0, 1, 2...)
                txtMaDichVu.Text = row.Cells["MaDV"].Value.ToString();
                txtTenDichVu.Text = row.Cells["TenDV"].Value.ToString();
                txtDonViTinh.Text = row.Cells["Donvitinh"].Value.ToString();
                txtDonGia.Text = row.Cells["Dongia"].Value.ToString();
                txtMaDichVu.Enabled = false;
            }
        }

        

        private void btnLammoi_Click(object sender, EventArgs e)
        {
            txtMaDichVu.Clear();
            txtTenDichVu.Clear();
            txtDonViTinh.Clear();
            txtDonGia.Clear();
            txtMaDichVu.Enabled = true;
            txtMaDichVu.Focus();

            LoadDataDichVu();
            txtTimKiem.Clear();
        }

        
        private void ExportExcel(System.Data.DataTable tb,string sheetname)
        {
            // 1. Khởi tạo các đối tượng Excel
            ex_cel.Application oExcel = new ex_cel.Application();
            ex_cel.Workbooks oBooks;
            ex_cel.Sheets oSheets;
            ex_cel.Workbook oBook;
            ex_cel.Worksheet oSheet;

            // 2. Tạo mới một Excel WorkBook 
            oExcel.Visible = true;
            oExcel.DisplayAlerts = false;
            oExcel.Application.SheetsInNewWorkbook = 1;
            oBooks = oExcel.Workbooks;
            oBook = (ex_cel.Workbook)(oExcel.Workbooks.Add(Type.Missing));
            oSheets = oBook.Worksheets;
            oSheet = (ex_cel.Worksheet)oSheets.get_Item(1);
            oSheet.Name = sheetname;

            // 3. Tạo tiêu đề chính cho file (Dòng 1)
            ex_cel.Range head = oSheet.get_Range("A1", "D1"); // Chỉ dùng 4 cột A, B, C, D
            head.MergeCells = true;
            head.Value2 = "DANH SÁCH ĐƠN GIÁ DỊCH VỤ";
            head.Font.Bold = true;
            head.Font.Name = "Tahoma";
            head.Font.Size = "16";
            head.HorizontalAlignment = ex_cel.XlHAlign.xlHAlignCenter;

            // 4. Tạo tiêu đề cột (Dòng 3)
            // Cột A: Mã dịch vụ
            ex_cel.Range cl1 = oSheet.get_Range("A3", "A3");
            cl1.Value2 = "MÃ DỊCH VỤ";
            cl1.ColumnWidth = 15.0;

            // Cột B: Tên dịch vụ
            ex_cel.Range cl2 = oSheet.get_Range("B3", "B3");
            cl2.Value2 = "TÊN DỊCH VỤ";
            cl2.ColumnWidth = 30.0;

            // Cột C: Đơn vị tính
            ex_cel.Range cl3 = oSheet.get_Range("C3", "C3");
            cl3.Value2 = "ĐƠN VỊ TÍNH";
            cl3.ColumnWidth = 15.0;

            // Cột D: Đơn giá
            ex_cel.Range cl4 = oSheet.get_Range("D3", "D3");
            cl4.Value2 = "ĐƠN GIÁ";
            cl4.ColumnWidth = 20.0;

            // Định dạng hàng tiêu đề (Dòng 3)
            ex_cel.Range rowHead = oSheet.get_Range("A3", "D3");
            rowHead.Font.Bold = true;
            rowHead.Borders.LineStyle = ex_cel.Constants.xlSolid;
            rowHead.Interior.ColorIndex = 15; // Màu xám nhạt
            rowHead.HorizontalAlignment = ex_cel.XlHAlign.xlHAlignCenter;

            // 5. Chuyển dữ liệu từ DataTable vào mảng Object để đổ vào Excel
            object[,] arr = new object[tb.Rows.Count, tb.Columns.Count];

            for (int r = 0; r < tb.Rows.Count; r++)
            {
                DataRow dr = tb.Rows[r];
                for (int c = 0; c < tb.Columns.Count; c++)
                {
                    arr[r, c] = dr[c];
                }
            }

            // 6. Thiết lập vùng điền dữ liệu (Bắt đầu từ dòng 4)
            int rowStart = 4;
            int columnStart = 1;
            int rowEnd = rowStart + tb.Rows.Count - 1;
            int columnEnd = tb.Columns.Count;

            // Xác định vùng Range chứa dữ liệu
            ex_cel.Range c1 = (ex_cel.Range)oSheet.Cells[rowStart, columnStart];
            ex_cel.Range c2 = (ex_cel.Range)oSheet.Cells[rowEnd, columnEnd];
            ex_cel.Range range = oSheet.get_Range(c1, c2);

            // Điền mảng dữ liệu vào Excel một lần để tối ưu tốc độ
            range.Value2 = arr;

            // Kẻ viền cho toàn bộ vùng dữ liệu
            range.Borders.LineStyle = ex_cel.Constants.xlSolid;

            // 7. Định dạng bổ sung
            // Căn giữa cột Mã dịch vụ và Đơn vị tính
            ex_cel.Range colMa = oSheet.get_Range("A4", "A" + rowEnd);
            colMa.HorizontalAlignment = ex_cel.XlHAlign.xlHAlignCenter;

            ex_cel.Range colDVT = oSheet.get_Range("C4", "C" + rowEnd);
            colDVT.HorizontalAlignment = ex_cel.XlHAlign.xlHAlignCenter;

            // Định dạng cột Đơn giá có dấu phân cách hàng nghìn
            ex_cel.Range colGia = oSheet.get_Range("D4", "D" + rowEnd);
            colGia.NumberFormat = "#,##0";
        }

        private void btnXuat_Click(object sender, EventArgs e)
        {
            // Lấy dữ liệu hiện tại từ DataGridView (DataSource thường là DataTable)
            System.Data.DataTable dt = (System.Data.DataTable)dgvDichVu.DataSource;

            if (dt != null && dt.Rows.Count > 0)
            {
                ExportExcel(dt, "DichVu");
            }
            else
            {
                MessageBox.Show("Không có dữ liệu để xuất!", "Thông báo");
            }
        }

        private void btnSua_Click_1(object sender, EventArgs e)
        {
            // 1. Kiểm tra dữ liệu đầu vào không được để trống
            if (string.IsNullOrWhiteSpace(txtMaDichVu.Text))
            {
                MessageBox.Show("Vui lòng chọn dịch vụ từ bảng để sửa!", "Thông báo");
                return;
            }

            // 2. Kiểm tra định dạng số cho Đơn giá
            if (!decimal.TryParse(txtDonGia.Text, out decimal donGia))
            {
                MessageBox.Show("Đơn giá phải là số hợp lệ!", "Lỗi nhập liệu");
                return;
            }

            // 3. Thực hiện cập nhật
            try
            {
                if (con.State == ConnectionState.Closed) con.Open();

                // Câu lệnh SQL Update dựa trên MaDV
                string query = "UPDATE Dichvu SET TenDV = @ten, Donvitinh = @dvt, Dongia = @gia WHERE MaDV = @ma";

                SqlCommand cmd = new SqlCommand(query, con);

                // Truyền giá trị từ các ô nhập liệu (TextBox) vào tham số
                cmd.Parameters.AddWithValue("@ma", txtMaDichVu.Text.Trim());
                cmd.Parameters.AddWithValue("@ten", txtTenDichVu.Text.Trim());
                cmd.Parameters.AddWithValue("@dvt", txtDonViTinh.Text.Trim());
                cmd.Parameters.AddWithValue("@gia", donGia);

                int result = cmd.ExecuteNonQuery();

                if (result > 0)
                {
                    MessageBox.Show("Cập nhật dịch vụ thành công!", "Thành công");
                    LoadDataDichVu();  // Tải lại bảng DataGridView
                    ClearFormDichVu(); // Xóa trắng các ô nhập liệu
                }
                else
                {
                    MessageBox.Show("Không tìm thấy Mã dịch vụ để cập nhật!", "Lỗi");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi sửa: " + ex.Message);
            }
            finally
            {
                con.Close();
            }
        }

        private void btnThem_Click_1(object sender, EventArgs e)
        {
            // 1. Kiểm tra không để trống các trường quan trọng
            if (string.IsNullOrWhiteSpace(txtMaDichVu.Text) || string.IsNullOrWhiteSpace(txtTenDichVu.Text))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ Mã và Tên dịch vụ!", "Thông báo");
                return;
            }

            // 2. Kiểm tra định dạng số cho Đơn giá
            if (!decimal.TryParse(txtDonGia.Text, out decimal donGia))
            {
                MessageBox.Show("Đơn giá phải là số hợp lệ!", "Lỗi nhập liệu");
                return;
            }

            try
            {
                // Kiểm tra nếu kết nối đang đóng thì mở ra
                if (con.State == ConnectionState.Closed) con.Open();

                string query = "INSERT INTO Dichvu (MaDV, TenDV, Donvitinh, Dongia) VALUES (@ma, @ten, @dvt, @gia)";

                // Sử dụng biến 'con' trực tiếp ở đây
                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@ma", txtMaDichVu.Text.Trim());
                cmd.Parameters.AddWithValue("@ten", txtTenDichVu.Text.Trim());
                cmd.Parameters.AddWithValue("@dvt", txtDonViTinh.Text.Trim());
                cmd.Parameters.AddWithValue("@gia", donGia);

                int result = cmd.ExecuteNonQuery();
                if (result > 0)
                {
                    MessageBox.Show("Đã thêm dịch vụ: " + txtTenDichVu.Text, "Thành công");
                    LoadDataDichVu();
                    ClearFormDichVu();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
            finally
            {
                con.Close(); // Luôn đóng kết nối sau khi xong
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            // 1. Kiểm tra xem đã chọn mã dịch vụ chưa
            if (string.IsNullOrWhiteSpace(txtMaDichVu.Text))
            {
                MessageBox.Show("Vui lòng chọn dịch vụ từ bảng để xóa!", "Thông báo");
                return;
            }

            // 2. Hỏi xác nhận trước khi xóa để tránh nhấn nhầm
            DialogResult dialogResult = MessageBox.Show("Bạn có chắc chắn muốn xóa dịch vụ này không?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (dialogResult == DialogResult.Yes)
            {
                try
                {
                    if (con.State == ConnectionState.Closed) con.Open();

                    string query = "DELETE FROM Dichvu WHERE MaDV = @ma";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@ma", txtMaDichVu.Text.Trim());

                    int result = cmd.ExecuteNonQuery();

                    if (result > 0)
                    {
                        MessageBox.Show("Đã xóa dịch vụ thành công!", "Thành công");
                        LoadDataDichVu();  // Tải lại bảng
                        ClearFormDichVu(); // Xóa trắng các ô nhập liệu
                    }
                    else
                    {
                        MessageBox.Show("Không tìm thấy dịch vụ để xóa!", "Lỗi");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi xóa: " + ex.Message);
                }
                finally
                {
                    con.Close();
                }
            }
        }

        private void btnTimkiem_Click(object sender, EventArgs e)
        {
            try
            {
                // 1. Lấy từ khóa từ ô TextBox tìm kiếm (giả sử tên là txtTimKiem)
                // Nếu bạn dùng ô nhập tên phòng ở phía trên, hãy thay đúng tên TextBox đó
                string keyword = txtTimKiem.Text.Trim();

                if (string.IsNullOrEmpty(keyword))
                {
                    MessageBox.Show("Vui lòng nhập tên dịch vụ cần tìm!", "Thông báo");
                    return;
                }

                // 2. Tạo câu lệnh truy vấn tìm kiếm gần đúng với LIKE
                string query = "SELECT * FROM Dichvu WHERE TenDV LIKE @key";

                SqlDataAdapter adapter = new SqlDataAdapter(query, con);
                // Thêm dấu % vào hai đầu để tìm kiếm mọi vị trí có chứa từ khóa
                adapter.SelectCommand.Parameters.AddWithValue("@key", "%" + keyword + "%");

                DataTable dt = new DataTable();
                adapter.Fill(dt);

                // 3. Hiển thị kết quả lên DataGridView
                dgvDichVu.DataSource = dt;

                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("Không tìm thấy dịch vụ nào khớp với từ khóa!", "Kết quả");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tìm kiếm: " + ex.Message);
            }
        }
    }
}
    
    

