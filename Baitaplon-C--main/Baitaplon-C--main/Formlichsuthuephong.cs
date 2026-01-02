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
    public partial class Formlichsuthuephong : Form
    {
        SqlConnection con = new SqlConnection("Data Source=NGUYENTRUNGKIEN\\SQLEXPRESS;Initial Catalog=Baitaplon-C#;Integrated Security=True");
        public Formlichsuthuephong()
        {
            InitializeComponent();
        }
        private void LoadLichSu()
        {
            try
            {
                if (con.State == ConnectionState.Closed) con.Open();

                // Truy vấn từ View đã tạo ở bước 1
                string query = "SELECT * FROM View_LichSuThue WHERE Trangthaihopdong = N'Hết hạn' OR Trangthaihopdong = N'Đã trả phòng'";

                SqlDataAdapter adp = new SqlDataAdapter(query, con);
                DataTable dt = new DataTable();
                adp.Fill(dt);

                // Gán dữ liệu vào DataGridView
                dgvlichsuthuephong.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải lịch sử: " + ex.Message);
            }
            finally { con.Close(); }
        }
        private void Formlichsuthuephong_Load(object sender, EventArgs e)
        {
            LoadLichSu();
        }

        private void btnxoa_Click(object sender, EventArgs e)
        {
            if (dgvlichsuthuephong.SelectedRows.Count > 0 || dgvlichsuthuephong.CurrentRow != null)
            {
                string maHD = dgvlichsuthuephong.CurrentRow.Cells["MaHD"].Value.ToString();

                DialogResult dr = MessageBox.Show("Hợp đồng này có dữ liệu liên quan ở nhiều bảng. Bạn có chắc chắn muốn xóa TOÀN BỘ không?",
                                                 "Xác nhận xóa hệ thống", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (dr == DialogResult.Yes)
                {
                    try
                    {
                        if (con.State == ConnectionState.Closed) con.Open();

                        // Xóa theo thứ tự từ bảng con sâu nhất đến bảng cha
                        // 1. Xóa ChiTietHoaDon dựa trên MaHD thông qua bảng HoaDon
                        // 2. Xóa HoaDon
                        // 3. Xóa HopDong
                        string query = @"DELETE FROM ChiTietHoaDon WHERE MaHoaDon IN (SELECT MaHoaDon FROM HoaDon WHERE MaHopdong = @mahd);
                                 DELETE FROM HoaDon WHERE MaHopdong = @mahd;
                                 DELETE FROM HopDong WHERE MaHD = @mahd;";

                        SqlCommand cmd = new SqlCommand(query, con);
                        cmd.Parameters.AddWithValue("@mahd", maHD);

                        cmd.ExecuteNonQuery();

                        MessageBox.Show("Đã dọn dẹp sạch sẽ dữ liệu hợp đồng " + maHD);
                        LoadLichSu();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Lỗi hệ thống: " + ex.Message);
                    }
                    finally { con.Close(); }
                }
            }
        }

        private void btntk_Click(object sender, EventArgs e)
        {
            try
            {
                if (con.State == ConnectionState.Closed) con.Open();

                // Tìm kiếm trên tất cả các cột có trong bảng
                // Kết hợp dữ liệu từ KhachThue và HopDong
                string query = @"SELECT * FROM View_LichSuThue 
                         WHERE (MaHD LIKE @search 
                            OR Maphong LIKE @search 
                            OR Makhach LIKE @search 
                            OR Hoten LIKE @search 
                            OR CCCD LIKE @search 
                            OR Ngaybatdau LIKE @search 
                            OR Ngayketthuc LIKE @search 
                            OR Tiencoc LIKE @search 
                            OR Giathuethang LIKE @search 
                            OR Trangthaihopdong LIKE @search 
                            OR Ngaylap LIKE @search 
                            OR SDT LIKE @search 
                            OR Gioitinh LIKE @search 
                            OR Ngaysinh LIKE @search 
                            OR Quequan LIKE @search)
                         AND (Trangthaihopdong = N'Đã trả phòng' OR Trangthaihopdong = N'Hết hạn')";

                SqlCommand cmd = new SqlCommand(query, con);
                // Thiết lập tham số tìm kiếm với ký tự đại diện % để tìm gần đúng
                cmd.Parameters.AddWithValue("@search", "%" + txttk.Text.Trim() + "%");

                SqlDataAdapter adp = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adp.Fill(dt);

                // Hiển thị kết quả lên Guna2DataGridView
                dgvlichsuthuephong.DataSource = dt;

                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("Không tìm thấy dữ liệu nào khớp với từ khóa!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi hệ thống khi tìm kiếm: " + ex.Message);
            }
            finally { con.Close(); }
        }

        private void btnmoi_Click(object sender, EventArgs e)
        {
            txttk.Clear(); 
            LoadLichSu();  
        }

        private void btnxuat_Click(object sender, EventArgs e)
        {
            // Lấy dữ liệu hiện tại từ DataGridView chuyển sang DataTable
            DataTable tb = (DataTable)dgvlichsuthuephong.DataSource;

            if (tb == null || tb.Rows.Count == 0)
            {
                MessageBox.Show("Không có dữ liệu để xuất!", "Thông báo");
                return;
            }
            ex_cel.Application oExcel = new ex_cel.Application();
            ex_cel.Workbooks oBooks;
            ex_cel.Sheets oSheets;
            ex_cel.Workbook oBook;
            ex_cel.Worksheet oSheet;

            oExcel.Visible = true;
            oExcel.DisplayAlerts = false;
            oExcel.Application.SheetsInNewWorkbook = 1;
            oBooks = oExcel.Workbooks;
            oBook = (ex_cel.Workbook)(oExcel.Workbooks.Add(Type.Missing));
            oSheets = oBook.Worksheets;
            oSheet = (ex_cel.Worksheet)oSheets.get_Item(1);
            oSheet.Name = "Lich su thue phong";

            // 1. Tạo tiêu đề lớn (Gộp từ cột A đến O - vì có 15 cột)
            ex_cel.Range head = oSheet.get_Range("A1", "O1");
            head.MergeCells = true;
            head.Value2 = "DANH SÁCH LỊCH SỬ THUÊ PHÒNG";
            head.Font.Bold = true;
            head.Font.Name = "Tahoma";
            head.Font.Size = "18";
            head.HorizontalAlignment = ex_cel.XlHAlign.xlHAlignCenter;

            // 2. Tạo tiêu đề các cột (A3 đến O3)
            string[] headers = { "Mã HĐ", "Mã Phòng", "Mã Khách", "Họ Tên", "CCCD", "Ngày Bắt Đầu", "Ngày Kết Thúc", "Tiền Cọc", "Giá Thuê", "Trạng Thái", "Ngày Lập", "SĐT", "Giới Tính", "Ngày Sinh", "Quê Quán" };
            for (int i = 0; i < headers.Length; i++)
            {
                ex_cel.Range cl = (ex_cel.Range)oSheet.Cells[3, i + 1];
                cl.Value2 = headers[i];
                cl.ColumnWidth = 15.0; // Độ rộng mặc định
                cl.Font.Bold = true;
                cl.Borders.LineStyle = ex_cel.Constants.xlSolid;
                cl.Interior.ColorIndex = 15;
                cl.HorizontalAlignment = ex_cel.XlHAlign.xlHAlignCenter;
            }

            // 3. Chuyển dữ liệu từ DataTable vào mảng
            object[,] arr = new object[tb.Rows.Count, tb.Columns.Count];
            for (int r = 0; r < tb.Rows.Count; r++)
            {
                DataRow dr = tb.Rows[r];
                for (int c = 0; c < tb.Columns.Count; c++)
                {
                    // Định dạng SĐT (cột 12 - chỉ số 11) thành dạng text để không mất số 0
                    if (c == 11)
                        arr[r, c] = "'" + dr[c].ToString();
                    else
                        arr[r, c] = dr[c];
                }
            }

            // 4. Thiết lập vùng điền dữ liệu (Bắt đầu từ hàng 4)
            int rowStart = 4;
            int columnStart = 1;
            int rowEnd = rowStart + tb.Rows.Count - 1;
            int columnEnd = tb.Columns.Count;

            if (tb.Rows.Count > 0)
            {
                ex_cel.Range c1 = (ex_cel.Range)oSheet.Cells[rowStart, columnStart];
                ex_cel.Range c2 = (ex_cel.Range)oSheet.Cells[rowEnd, columnEnd];
                ex_cel.Range range = oSheet.get_Range(c1, c2);

                range.Value2 = arr; // Đổ mảng dữ liệu vào Excel
                range.Borders.LineStyle = ex_cel.Constants.xlSolid;

                // 5. Định dạng các cột ngày tháng cho đẹp
                // Cột F (Ngày bắt đầu), G (Ngày kết thúc), K (Ngày lập), N (Ngày sinh)
                string[] dateCols = { "F", "G", "K", "N" };
                foreach (string col in dateCols)
                {
                    ex_cel.Range rgDate = oSheet.get_Range(col + rowStart, col + rowEnd);
                    rgDate.NumberFormat = "dd/mm/yyyy";
                }
            }
        }

        private void btnsua_Click(object sender, EventArgs e)
        {
            // 1. Kiểm tra xem đã chọn hợp đồng nào chưa
            if (dgvlichsuthuephong.CurrentRow == null)
            {
                MessageBox.Show("Vui lòng chọn hợp đồng cần khôi phục hiệu lực!");
                return;
            }

            // 2. Lấy MaHD của dòng đang chọn
            string maHD = dgvlichsuthuephong.CurrentRow.Cells["MaHD"].Value.ToString();

            // 3. Hỏi xác nhận
            DialogResult dr = MessageBox.Show("Bạn có muốn khôi phục hợp đồng " + maHD + " về trạng thái 'Hiệu lực' không?",
                                             "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dr == DialogResult.Yes)
            {
                try
                {
                    if (con.State == ConnectionState.Closed) con.Open();

                    // 4. Cập nhật trạng thái về 'Hiệu lực'
                    string query = "UPDATE HopDong SET Trangthaihopdong = N'Hiệu lực' WHERE MaHD = @mahd";

                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@mahd", maHD);

                    int result = cmd.ExecuteNonQuery();

                    if (result > 0)
                    {
                        MessageBox.Show("Đã khôi phục hợp đồng thành công! Hợp đồng sẽ xuất hiện lại trong Form Hợp Đồng.");
                        // 5. Tải lại lưới lịch sử (nó sẽ biến mất khỏi đây vì lưới này chỉ hiện 'Hết hạn')
                        LoadLichSu();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi cập nhật: " + ex.Message);
                }
            }

        }

        private void dgvlichsuthuephong_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                // Hiển thị trạng thái của dòng đang chọn lên ComboBox cboTrangThai
                cboTrangthai.Text = dgvlichsuthuephong.Rows[e.RowIndex].Cells["Trangthaihopdong"].Value.ToString();
            }
        }
    }
}
