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
using static System.Windows.Forms.LinkLabel;
//
using ex_cel = Microsoft.Office.Interop.Excel;
namespace Baitaplon
{
    public partial class UC_BaoCaoThongKe : Form
    {
        SqlConnection con = new SqlConnection(@"Data Source=NGUYENTRUNGKIEN\SQLEXPRESS;Initial Catalog=Baitaplon-C#;Integrated Security=True");
        private const string ConnectionString = (@"Data Source=NGUYENTRUNGKIEN\SQLEXPRESS;Initial Catalog=Baitaplon-C#;Integrated Security=True");
        public UC_BaoCaoThongKe()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void Fillpanel_Paint(object sender, PaintEventArgs e)
        {

        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {

        }

        private void label11_Click(object sender, EventArgs e)
        {

        }

        private void dgvphong_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
        //load tháng,năm(form_load)
        private void UC_BaoCaoThongKe_Load(object sender, EventArgs e)
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



        private void guna2Panel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }


        private void btnxemthongkephong_click(object sender, EventArgs e)
        {

            // 1. Kiểm tra đầu vào
            if (cbthang.SelectedValue == null || cbnam.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng chọn đầy đủ Tháng và Năm!");
                return;
            }

            try
            {
                if (con.State == ConnectionState.Closed) con.Open();

                int thang = int.Parse(cbthang.Text);
                int nam = int.Parse(cbnam.Text);

                // Xác định khoảng thời gian của tháng được chọn để lọc hợp đồng
                DateTime ngayDauThang = new DateTime(nam, thang, 1);
                DateTime ngayCuoiThang = ngayDauThang.AddMonths(1).AddDays(-1);

                // 2. Câu lệnh SQL chuẩn theo Database của nhóm bạn
                // Sử dụng LEFT JOIN để hiện cả phòng trống (không có hợp đồng/khách)
                string sqlGrid = @"
          SELECT 
        p.Maphong, 
        p.Tenphong, 
        lp.Tenloai, 
        p.Dientich, 
        lp.Dongia, 
        ISNULL(k.Hoten, N'---') AS HoTen, 
        ISNULL(k.SDT, N'---') AS SDT,
        CASE 
            WHEN k.Hoten IS NULL THEN N'Trống' 
            ELSE N'Đang thuê' 
        END AS TrangThaiPhong, 
        ISNULL(h.Trangthaihopdong, N'N/A') AS TrangThaiHD,
        h.Ngayketthuc AS NgayKetThuc
    FROM Phongtro p
    INNER JOIN LoaiPhong lp ON p.Maloaiphong = lp.Maloaiphong
    LEFT JOIN HopDong h ON p.Maphong = h.Maphong 
        AND h.Ngaybatdau <= @ngayCuoi 
        AND (h.Ngayketthuc >= @ngayDau OR h.Ngayketthuc IS NULL)
        AND h.Trangthaihopdong = N'Hiệu lực'
    LEFT JOIN KhachThue k ON h.Makhach = k.Makhach";

                SqlCommand cmd = new SqlCommand(sqlGrid, con);
                cmd.Parameters.AddWithValue("@ngayDau", ngayDauThang);
                cmd.Parameters.AddWithValue("@ngayCuoi", ngayCuoiThang);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                // 3. Hiển thị và xử lý tràn dòng

                dgvphong.DataSource = dt;


                CapNhatThongKe();




            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi hiển thị: " + ex.Message);
            }
            finally { if (con.State == ConnectionState.Open) con.Close(); }
        }

        //
        private void CapNhatThongKe()
        {
            if (dgvphong.DataSource == null) return;
            DataTable dt = (DataTable)dgvphong.DataSource;

            // --- KHAI BÁO BIẾN THỜI GIAN Ở ĐÂY ---
            DateTime ngayhientai = DateTime.Now;
            DateTime nguongSapHetHan = ngayhientai.AddDays(30);

            // 1. Tổng số phòng
            lbltongsophong.Text = dt.Rows.Count.ToString();

            // 2. Số phòng đang thuê
            int phongDangThue = dt.AsEnumerable().Count(row => row.Field<string>("HoTen") != "---");
            lblphongdangthue.Text = phongDangThue.ToString();

            // 3. Số phòng còn trống
            lblphongtrong.Text = (dt.Rows.Count - phongDangThue).ToString();



            // 5. Tổng số khách
            int soKhach = dt.AsEnumerable()
                            .Where(row => row.Field<string>("HoTen") != "---")
                            .Select(row => row.Field<string>("HoTen"))
                            .Distinct().Count();
            lbltongsokhach.Text = soKhach.ToString();
        }
        // Hàm bổ trợ để tải lại dữ liệu sau khi sửa

        // tạo 1 hàm load_data

        private void btntktkphong_click(object sender, EventArgs e)
        {
            // 1. Lấy dữ liệu từ 2 Textbox và 1 ComboBox trạng thái hợp đồng
            string maPhong = txtmaphong_tk.Text.Trim();
            string tenKhach = txtkhachthue.Text.Trim();
            string trangThaiHD = (cbtthopdong.SelectedItem != null) ? cbtthopdong.Text : "Tất cả";

            // 2. Kiểm tra bộ lọc thời gian (Tháng/Năm) - Bắt buộc để JOIN đúng dữ liệu
            if (string.IsNullOrEmpty(cbthang.Text) || string.IsNullOrEmpty(cbnam.Text))
            {
                MessageBox.Show("Vui lòng chọn Tháng và Năm để tìm kiếm!", "Thông báo");
                return;
            }

            int thang = int.Parse(cbthang.Text);
            int nam = int.Parse(cbnam.Text);
            DateTime ngayDau = new DateTime(nam, thang, 1);
            DateTime ngayCuoi = ngayDau.AddMonths(1).AddDays(-1);

            try
            {
                if (con.State == ConnectionState.Closed) con.Open();

                // 3. Câu lệnh SQL (Vẫn giữ LEFT JOIN để lấy được cả phòng trống)
                string sql = @"
            SELECT 
                p.Maphong, p.Tenphong, lp.Tenloai, p.Dientich, lp.Dongia, 
                ISNULL(k.Hoten, N'---') AS HoTen, 
                ISNULL(k.SDT, N'---') AS SDT,
                CASE 
                    WHEN k.Hoten IS NULL OR k.Hoten = N'---' THEN N'Trống' 
                    ELSE N'Đang thuê' 
                END AS TrangThaiPhong, 
                ISNULL(h.Trangthaihopdong, N'N/A') AS TrangThaiHD,
                h.Ngayketthuc AS NgayKetThuc
            FROM Phongtro p
            INNER JOIN LoaiPhong lp ON p.Maloaiphong = lp.Maloaiphong
            LEFT JOIN HopDong h ON p.Maphong = h.Maphong 
                AND h.Ngaybatdau <= @ngayCuoi 
                AND (h.Ngayketthuc >= @ngayDau OR h.Ngayketthuc IS NULL)
                AND h.Trangthaihopdong = N'Hiệu lực'
            LEFT JOIN KhachThue k ON h.Makhach = k.Makhach
            WHERE 1=1";

                // 4. Cộng chuỗi điều kiện động (Bỏ phần lọc trạng thái phòng)
                if (!string.IsNullOrEmpty(maPhong))
                    sql += " AND p.Maphong LIKE @MaP";

                if (!string.IsNullOrEmpty(tenKhach))
                    sql += " AND k.Hoten LIKE @TenK";

                // Chỉ lọc theo Trạng thái hợp đồng (Hiệu lực / N/A)
                if (trangThaiHD == "Hiệu lực")
                    sql += " AND h.Trangthaihopdong = N'Hiệu lực'";
                else if (trangThaiHD == "N/A")
                    sql += " AND h.Trangthaihopdong IS NULL";

                SqlCommand cmd = new SqlCommand(sql, con);

                // 5. Gán Parameters
                cmd.Parameters.AddWithValue("@ngayDau", ngayDau);
                cmd.Parameters.AddWithValue("@ngayCuoi", ngayCuoi);

                if (!string.IsNullOrEmpty(maPhong))
                    cmd.Parameters.AddWithValue("@MaP", "%" + maPhong + "%");

                if (!string.IsNullOrEmpty(tenKhach))
                    cmd.Parameters.AddWithValue("@TenK", "%" + tenKhach + "%");

                // 6. Đổ dữ liệu vào DataTable
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                dgvphong.DataSource = dt;

                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("Không tìm thấy kết quả nào khớp với yêu cầu!", "Thông tin");
                }

                // 7. Cập nhật lại các nhãn thống kê số liệu
                CapNhatThongKe();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
            finally
            {
                if (con.State == ConnectionState.Open) con.Close();
            }



        }

        private void btnxuat_click(object sender, EventArgs e)
        {
            // 1. Kiểm tra dữ liệu trên DataGridView thay vì DataTable gốc
            if (dgvphong.Rows.Count == 0)
            {
                MessageBox.Show("Không có dữ liệu hiển thị trên lưới để xuất!", "Thông báo");
                return;
            }

            try
            {
                ex_cel.Application oExcel = new ex_cel.Application();
                oExcel.Visible = true;
                oExcel.DisplayAlerts = false;
                oExcel.Application.SheetsInNewWorkbook = 1;

                ex_cel.Workbook oBook = (ex_cel.Workbook)(oExcel.Workbooks.Add(Type.Missing));
                ex_cel.Worksheet oSheet = (ex_cel.Worksheet)oBook.Worksheets.get_Item(1);
                oSheet.Name = "THONG KE PHONG";

                // 2. Tiêu đề chính (Dòng 1)
                ex_cel.Range head = oSheet.get_Range("A1", "K1");
                head.MergeCells = true;
                head.Value2 = "DANH SÁCH THỐNG KÊ PHÒNG";
                head.Font.Bold = true;
                head.Font.Name = "Tahoma";
                head.Font.Size = 16;
                head.HorizontalAlignment = ex_cel.XlHAlign.xlHAlignCenter;

                // 3. Tiêu đề cột (Dòng 3) - Có 11 cột từ A đến K
                string[] headers = {
            "STT", "MÃ PHÒNG", "TÊN PHÒNG", "LOẠI PHÒNG", "DIỆN TÍCH",
            "GIÁ THUÊ", "KHÁCH THUÊ", "NGÀY HẾT HẠN", "DIỆN THOẠI",
            "TT PHÒNG", "TT HỢP ĐỒNG"
        };

                for (int i = 0; i < headers.Length; i++)
                {
                    ex_cel.Range cl = (ex_cel.Range)oSheet.Cells[3, i + 1];
                    cl.Value2 = headers[i];
                    cl.Font.Bold = true;
                    cl.Interior.ColorIndex = 15; // Màu xám
                    cl.Borders.LineStyle = ex_cel.Constants.xlSolid;
                    cl.HorizontalAlignment = ex_cel.XlHAlign.xlHAlignCenter;
                }

                // 4. Đổ dữ liệu từ DataGridView vào mảng (arr)
                // Số dòng thực tế trên lưới (trừ dòng trống cuối nếu có)
                int rowCount = dgvphong.Rows.Count;
                if (dgvphong.AllowUserToAddRows) rowCount--;

                // QUAN TRỌNG: Khai báo mảng 11 cột (tương ứng A -> K)
                object[,] arr = new object[rowCount, 11];

                for (int r = 0; r < rowCount; r++)
                {
                    DataGridViewRow gRow = dgvphong.Rows[r];

                    arr[r, 0] = r + 1;                                  // STT
                    arr[r, 1] = gRow.Cells["Maphong"].Value;           // Cột B
                    arr[r, 2] = gRow.Cells["Tenphong"].Value;          // Cột C
                    arr[r, 3] = gRow.Cells["Tenloai"].Value;           // Cột D
                    arr[r, 4] = gRow.Cells["Dientich"].Value;          // Cột E
                    arr[r, 5] = gRow.Cells["Dongia"].Value;            // Cột F
                    arr[r, 6] = gRow.Cells["HoTen"].Value;             // Cột G
                    arr[r, 7] = gRow.Cells["NgayKetThuc"].Value;       // Cột H
                    arr[r, 8] = "'" + gRow.Cells["SDT"].Value;         // Cột I (Dấu ' để giữ số 0 điện thoại)
                    arr[r, 9] = gRow.Cells["TrangThaiPhong"].Value;    // Cột J
                    arr[r, 10] = gRow.Cells["TrangThaiHD"].Value;      // Cột K
                }

                // 5. Dán mảng dữ liệu vào Excel
                int rowStart = 4;
                int rowEnd = rowStart + rowCount - 1;

                ex_cel.Range rangeData = oSheet.get_Range("A" + rowStart, "K" + rowEnd);
                rangeData.Value2 = arr;
                rangeData.Borders.LineStyle = ex_cel.Constants.xlSolid;

                // 6. Định dạng
                // Định dạng cột H (Ngày hết hạn)
                ex_cel.Range clNgay = oSheet.get_Range("H" + rowStart, "H" + rowEnd);
                clNgay.NumberFormat = "dd/mm/yyyy";

                // Định dạng cột F (Giá thuê - Tiền tệ)
                ex_cel.Range clGia = oSheet.get_Range("F" + rowStart, "F" + rowEnd);
                clGia.NumberFormat = "#,##0";

                // Tự động giãn cột và căn giữa STT
                oSheet.get_Range("A" + rowStart, "A" + rowEnd).HorizontalAlignment = ex_cel.XlHAlign.xlHAlignCenter;
                oSheet.get_Range("A1", "K" + rowEnd).Columns.AutoFit();

                MessageBox.Show("Xuất báo cáo thành công!", "Thông báo");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi xuất Excel: " + ex.Message);
            }           // Cột G: Email
        }


    }
}
        
