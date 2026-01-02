using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

using ex_cel = Microsoft.Office.Interop.Excel;

namespace Baitaplon
{
    public partial class UC_BaoCaoDoanhThu : UserControl
    {

        SqlConnection con = new SqlConnection(@"Data Source=NGUYENTRUNGKIEN\SQLEXPRESS;Initial Catalog=Baitaplon-C#;Integrated Security=True");
        private const string ConnectionString = (@"Data Source=NGUYENTRUNGKIEN\SQLEXPRESS;Initial Catalog=Baitaplon-C#;Integrated Security=True");

        public UC_BaoCaoDoanhThu()
        {
            InitializeComponent();
        }
        //
        
    
        

        private void guna2Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void dgvDoanhThu_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
        //load tháng, năm (form_load)
        private void UC_BaoCaoDoanhThu_Load(object sender, EventArgs e)
        {
            if (con.State == ConnectionState.Closed)
                con.Open();

            // Load Tháng
            DataTable thang = new DataTable();
            thang.Columns.Add("Thang");
            for (int i = 1; i <= 12; i++)
                thang.Rows.Add(i);
            cbthang.DataSource = thang;
            cbthang.DisplayMember = "Thang";
            cbthang.ValueMember = "Thang";

            // Load Năm từ database
            string sql = "SELECT DISTINCT YEAR(Ngaylap) AS Nam FROM HoaDon";
            SqlDataAdapter da = new SqlDataAdapter(sql, con);
            DataTable nam = new DataTable();
            da.Fill(nam);
            cbnam.DataSource = nam;
            cbnam.DisplayMember = "Nam";
            cbnam.ValueMember = "Nam";

            con.Close();
        }
        // tự tạo 1 hàm load doanh thu
        
        // tự tạo hàm tính tổng doanh thu
        private void TinhTongDoanhThu()
        {
            DataTable tb = (DataTable)dgvdoanhthu.DataSource;
            if (tb == null || tb.Rows.Count == 0) return;

            decimal tongChua = 0;
            decimal tongDa = 0;

            foreach (DataRow r in tb.Rows)
            {
                decimal tien = r["Tongtien"] != DBNull.Value ? Convert.ToDecimal(r["Tongtien"]) : 0;
                // Chuẩn hóa so sánh với "Đã trả" theo SQL bạn đã chốt
                string trangThai = r["Trangthai"].ToString().Trim();

                if (trangThai == "Đã trả" || trangThai == "Đã thanh toán")
                    tongDa += tien;
                else
                    tongChua += tien;
            }

            lbtienctt.Text = tongChua.ToString("N0") + " VNĐ";
            lbtiendtt.Text = tongDa.ToString("N0") + " VNĐ";
            lblTongDoanhThu.Text = (tongChua + tongDa).ToString("N0") + " VNĐ";
        }



        private void btnxem_click(object sender, EventArgs e)
        {// Kiểm tra xem người dùng đã chọn đủ thông tin chưa

            if (cbthang.SelectedValue == null || cbnam.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng chọn tháng và năm!");
                return;
            }

            if (con.State == ConnectionState.Closed)
                con.Open();

            string sql = @"SELECT 
                        hd.MaHD,
                        pt.Tenphong,
                        kt.Hoten AS TenKhachHang,
                        hd.Tongtien,
                        hd.Ngaylap,
                        hd.Trangthai
                   FROM HoaDon hd
                   INNER JOIN HopDong hdg ON hd.MaHopDong = hdg.MaHD
                   INNER JOIN PhongTro pt ON hdg.Maphong = pt.Maphong
                   INNER JOIN KhachThue kt ON hdg.Makhach = kt.Makhach
                   WHERE hd.Thang = @thang AND hd.Nam = @nam";

            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@thang", cbthang.SelectedValue);
            cmd.Parameters.AddWithValue("@nam", cbnam.SelectedValue);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            dgvdoanhthu.DataSource = dt;

            con.Close();

            if (dt.Rows.Count == 0)
            {
                MessageBox.Show($"Không có dữ liệu doanh thu cho tháng {cbthang.SelectedValue} năm {cbnam.SelectedValue}", "Thông báo");
                lbtienctt.Text = "0 VNĐ";
                lbtiendtt.Text = "0 VNĐ";
                lblTongDoanhThu.Text = "0 VNĐ";
            }
            else
            {
                TinhTongDoanhThu();
            }
        }

        private void dgvdoanhthu_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {

        }
        // sửa trạng thái theo mã hóa đơn

        private void btn_sua(object sender, EventArgs e)
        {
            if (txtMaHD.Text == "")
            {
                MessageBox.Show("Chọn hóa đơn cần sửa!");
                return;
            }

            if (con.State == ConnectionState.Closed)
                con.Open();

            string sql = @"UPDATE HoaDon
                   SET Trangthai = @tt
                   WHERE MaHD = @ma";

            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@tt", cbTrangThai.Text);
            cmd.Parameters.AddWithValue("@ma", txtMaHD.Text);

            cmd.ExecuteNonQuery();
            con.Close();

            MessageBox.Show("Cập nhật thành công!");

            btnxem_click(null, null); // load lại danh sách

        }
        
        private void btntimkiem_click(object sender, EventArgs e)
        {
            if (con.State == ConnectionState.Closed)
                con.Open();
            // Kiểm tra nếu ô tìm kiếm trống
            if (string.IsNullOrWhiteSpace(txtTim.Text))
            {
                MessageBox.Show("Vui lòng nhập từ khóa cần tìm (Mã HD, Tên khách hoặc Trạng thái)!");
                return;
            }
            string sql = @"SELECT 
                        hd.MaHD,
                        pt.Tenphong,
                        kt.Hoten AS TenKhachHang,
                        hd.Tongtien,
                        hd.Ngaylap,
                        hd.Trangthai
                   FROM HoaDon hd
                   INNER JOIN HopDong hdg ON hd.MaHopDong = hdg.MaHD
                   INNER JOIN PhongTro pt ON hdg.Maphong = pt.Maphong
                   INNER JOIN KhachThue kt ON hdg.Makhach = kt.Makhach
                   WHERE hd.MaHD LIKE @key
                      OR kt.Hoten LIKE @key
                      OR hd.Trangthai LIKE @key";

            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@key", "%" + txtTim.Text.Trim() + "%");

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            dgvdoanhthu.DataSource = dt;
            con.Close();
            // THÊM THÔNG BÁO NẾU KHÔNG CÓ KẾT QUẢ
            if (dt.Rows.Count == 0)
            {
                MessageBox.Show("Không tìm thấy kết quả nào phù hợp với từ khóa: " + txtTim.Text, "Thông báo");
                // Xóa trắng các nhãn thống kê vì không có dữ liệu
                lbtienctt.Text = "0 VNĐ";
                lbtiendtt.Text = "0 VNĐ";
                lblTongDoanhThu.Text = "0 VNĐ";
            }
            else
            {
                TinhTongDoanhThu();
            }
        }

        // Sự kiện khi click vào 1 dòng trên Grid sẽ hiện mã lên TextBox để sửa
        private void dgvdoanhthu_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                txtMaHD.Text = dgvdoanhthu.Rows[e.RowIndex].Cells["MaHD"].Value.ToString();
                cbTrangThai.Text = dgvdoanhthu.Rows[e.RowIndex].Cells["Trangthai"].Value.ToString();
            }
        }

        private void guna2Panel1_Paint_1(object sender, PaintEventArgs e)
        {

        }

        private void btn_xuat_click(object sender, EventArgs e)
        {
            // 1. Kiểm tra nếu Grid không có dữ liệu thì không xuất
            if (dgvdoanhthu.Rows.Count == 0)
            {
                MessageBox.Show("Không có dữ liệu doanh thu để xuất!", "Thông báo");
                return;
            }

            ex_cel.Application oExcel = null;
            try
            {
                // 2. Khởi tạo đối tượng Excel
                oExcel = new ex_cel.Application();
                oExcel.Visible = false; // Ẩn Excel đi để tránh người dùng click làm gián đoạn (chống lỗi 0x800AC472)
                oExcel.DisplayAlerts = false;
                oExcel.ScreenUpdating = false; // Tắt cập nhật màn hình để tăng tốc độ xuất

                ex_cel.Workbook oBook = oExcel.Workbooks.Add(Type.Missing);
                ex_cel.Worksheet oSheet = (ex_cel.Worksheet)oBook.Worksheets.get_Item(1);
                oSheet.Name = "DOANH THU";

                // 3. Tạo tiêu đề báo cáo
                ex_cel.Range head = oSheet.get_Range("A1", "F1");
                head.MergeCells = true;
                head.Value2 = "BÁO CÁO CHI TIẾT DOANH THU THÁNG " + cbthang.Text + " NĂM " + cbnam.Text;
                head.Font.Bold = true;
                head.Font.Size = 16;
                head.HorizontalAlignment = ex_cel.XlHAlign.xlHAlignCenter;

                // 4. Tạo tiêu đề cột (Dòng 3)
                string[] headers = { "STT", "Mã Hóa Đơn", "Tên Phòng", "Khách Hàng", "Tổng Tiền (VNĐ)", "Trạng Thái" };
                for (int i = 0; i < headers.Length; i++)
                {
                    ex_cel.Range cl = (ex_cel.Range)oSheet.Cells[3, i + 1];
                    cl.Value2 = headers[i];
                    cl.Font.Bold = true;
                    cl.Interior.ColorIndex = 15; // Màu xám nhạt
                    cl.Borders.LineStyle = ex_cel.Constants.xlSolid;
                    cl.HorizontalAlignment = ex_cel.XlHAlign.xlHAlignCenter;
                }

                // 5. Chuẩn bị dữ liệu bằng Mảng (Array) để đổ 1 lần duy nhất (Tránh lỗi Index)
                int rowCount = dgvdoanhthu.Rows.Count;
                if (dgvdoanhthu.AllowUserToAddRows) rowCount--; // Trừ dòng trống cuối grid nếu có

                object[,] arr = new object[rowCount, headers.Length];

                for (int r = 0; r < rowCount; r++)
                {
                    DataGridViewRow gRow = dgvdoanhthu.Rows[r];
                    arr[r, 0] = r + 1; // STT
                    arr[r, 1] = gRow.Cells["MaHD"].Value;
                    arr[r, 2] = gRow.Cells["Tenphong"].Value;
                    arr[r, 3] = gRow.Cells["TenKhachHang"].Value;
                    arr[r, 4] = gRow.Cells["Tongtien"].Value;
                    arr[r, 5] = gRow.Cells["Trangthai"].Value;
                }

                // 6. Đổ mảng dữ liệu vào Excel bắt đầu từ dòng 4
                int rowStart = 4;
                int rowEnd = rowStart + rowCount - 1;
                ex_cel.Range rangeData = oSheet.get_Range("A" + rowStart, "F" + rowEnd);
                rangeData.Value2 = arr;
                rangeData.Borders.LineStyle = ex_cel.Constants.xlSolid;

                // 7. Thêm phần Tổng kết ở cuối file Excel
                int rowSum = rowEnd + 2;
                oSheet.Cells[rowSum, 4] = "TỔNG DOANH THU:";
                oSheet.Cells[rowSum, 5] = lblTongDoanhThu.Text;
                oSheet.get_Range("D" + rowSum, "E" + rowSum).Font.Bold = true;
                oSheet.get_Range("D" + rowSum, "E" + rowSum).Font.Color = ColorTranslator.ToOle(Color.Red);

                // 8. Định dạng cột tiền tệ cho đẹp
                ex_cel.Range clTien = oSheet.get_Range("E" + rowStart, "E" + rowSum);
                clTien.NumberFormat = "#,##0";

                // 9. Hoàn tất và hiển thị Excel
                oSheet.Columns.AutoFit();
                oExcel.ScreenUpdating = true;
                oExcel.Visible = true;

                MessageBox.Show("Xuất báo cáo doanh thu thành công!", "Thông báo");
            }
            catch (System.Runtime.InteropServices.COMException comEx)
            {
                // Xử lý lỗi nếu Excel bị bận
                if ((uint)comEx.ErrorCode != 0x800AC472)
                    MessageBox.Show("Lỗi Excel: " + comEx.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi hệ thống: " + ex.Message);
            }
            finally
            {
                // Giải phóng tài nguyên nếu cần
                System.Runtime.InteropServices.Marshal.ReleaseComObject(oExcel);
            }
        }

        private void guna2HtmlLabel3_Click(object sender, EventArgs e)
        {

        }

        // 
    }
    }
        

