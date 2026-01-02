using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

using ex_cel = Microsoft.Office.Interop.Excel;

namespace Baitaplon
{
    public partial class UC_QuanLySuCo : Form
    {
        SqlConnection con = new SqlConnection(@"Data Source=NGUYENTRUNGKIEN\SQLEXPRESS;Initial Catalog=Baitaplon-C#;Integrated Security=True");
        private const string ConnectionString = (@"Data Source=NGUYENTRUNGKIEN\SQLEXPRESS;Initial Catalog=Baitaplon-C#;Integrated Security=True");
        //hàm loaddata

        public UC_QuanLySuCo()
        {
            InitializeComponent();
            //vì tạo cột dgv bằng tay nên phải có dòng đóng này vào
            dgvsuco.AutoGenerateColumns = false;
            load_SuCo();  // Load dữ liệu lên DataGridView
            load_Phong(); // Load dữ liệu vào ComboBox Phòng
            
        }
        //
        private void CapNhatThongKe(DataTable dt)
        {
            // 1. Tổng sự cố
            lbltongsuco.Text = dt.Rows.Count.ToString();

            // 2. Tính tổng tiền
            decimal tongTien = dt.AsEnumerable()
                                 .Where(r => r["ChiPhiSuaDuKien"] != DBNull.Value)
                                 .Sum(r => Convert.ToDecimal(r["ChiPhiSuaDuKien"]));
            lbltongchiphi.Text = tongTien.ToString("N0") + " VNĐ";

            // 3. Đã xử lý (Trạng thái = 'Đã hoàn thành')
            int daXuly = dt.AsEnumerable()
                           .Count(r => r["TrangThaiXuLy"]?.ToString() == "Đã xử lý");
            lbldaxuly.Text = daXuly.ToString();

            // 4. Chờ xử lý (Các trạng thái còn lại)
            lblchoxuly.Text = (dt.Rows.Count - daXuly).ToString();
        }
        private void load_SuCo()
        {
            string sql = @"SELECT MaSC, Mats, Maphong, NgayBao,
                                  MoTaSuCo, TrangThaiXuLy, ChiPhiSuaDuKien
                           FROM SuCo";


            SqlDataAdapter da = new SqlDataAdapter(sql, con);
            DataTable dt = new DataTable();
            da.Fill(dt);

            dgvsuco.DataSource = dt;
            CapNhatThongKe(dt);
        }

        private void load_Phong()
        {
            SqlDataAdapter da = new SqlDataAdapter("SELECT Maphong FROM Phongtro", con);
            DataTable dt = new DataTable();
            da.Fill(dt);

            cbphong.DataSource = dt;
            cbphong.DisplayMember = "Maphong";
            cbphong.ValueMember = "Maphong";
            if (dt.Rows.Count > 0) cbphong_SelectionChangeCommitted(null, null);
        }
       
        private void guna2HtmlLabel2_Click(object sender, EventArgs e)
        {

        }

        private void guna2HtmlLabel1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }



        //
        private bool CheckTrungMa(string maSC)
        {
            bool isDuplicate = false;
            string sql = "SELECT COUNT(*) FROM SuCo WHERE MaSC = @ma";
            using (SqlCommand cmd = new SqlCommand(sql, con))
            {
                cmd.Parameters.AddWithValue("@ma", maSC);
                if (con.State == ConnectionState.Closed) con.Open();
                int count = (int)cmd.ExecuteScalar();
                if (count > 0) isDuplicate = true;
                con.Close();
            }
            return isDuplicate;
        }


        private void btnthem_click(object sender, EventArgs e)
        {
            if (cbmats.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng chọn tài sản!");
                return;
            }

            if (!Regex.IsMatch(txtchiphidukien.Text.Trim(), @"^\d+$"))
            {
                MessageBox.Show("Chi phí phải là số!");
                return;
            }
            string ma = txtmasc.Text.Trim();
            if (string.IsNullOrEmpty(ma))
            {
                MessageBox.Show("Vui lòng nhập Mã sự cố!"); return;
            }
            // Kiểm tra trùng
            if (CheckTrungMa(txtmasc.Text.Trim()))
            {
                MessageBox.Show("Mã sự cố đã tồn tại! Vui lòng nhập mã khác."); return;
            }
            string sql = @"INSERT INTO SuCo
                           (MaSC,Mats, Maphong, NgayBao, MoTaSuCo, TrangThaiXuLy, ChiPhiSuaDuKien)
                           VALUES (@MaSC,@Mats, @Maphong, @NgayBao, @MoTa, @TrangThai, @ChiPhi)";

            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@MaSC", txtmasc.Text.Trim());

            cmd.Parameters.AddWithValue("@Mats", cbmats.SelectedValue);
            cmd.Parameters.AddWithValue("@Maphong", cbphong.SelectedValue);
            cmd.Parameters.AddWithValue("@NgayBao", dtngaybao.Value);
            cmd.Parameters.AddWithValue("@MoTa", txtmota.Text.Trim());
            cmd.Parameters.AddWithValue("@TrangThai", cbtrangthai.Text.Trim());
            cmd.Parameters.AddWithValue("@ChiPhi", decimal.Parse(txtchiphidukien.Text.Trim()));

            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();

            MessageBox.Show("Thêm sự cố thành công!");
            load_SuCo();
            btnloc_click(null, null);
        }
    

            // B3: Thực thi
            
        

        private void dgvsuco_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var row = dgvsuco.Rows[e.RowIndex];

            txtmasc.Text = row.Cells["MaSC"].Value?.ToString();
            txtmota.Text = row.Cells["MoTaSuCo"].Value?.ToString();
            cbtrangthai.Text = row.Cells["TrangThaiXuLy"].Value?.ToString();
            txtchiphidukien.Text = row.Cells["ChiPhiSuaDuKien"].Value?.ToString();

            if (row.Cells["Maphong"].Value != DBNull.Value)
                cbphong.SelectedValue = row.Cells["Maphong"].Value;

            if (row.Cells["Mats"].Value != DBNull.Value)
                cbmats.SelectedValue = row.Cells["Mats"].Value;

            if (row.Cells["NgayBao"].Value != DBNull.Value)
                dtngaybao.Value = Convert.ToDateTime(row.Cells["NgayBao"].Value);

        }
        

        private void btnsua_click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtmasc.Text))
            {
                MessageBox.Show("Chưa chọn bản ghi!");
                return;
            }
            // Kiểm tra xem mã có tồn tại TRƯỚC khi sửa không
            if (!CheckTrungMa(txtmasc.Text.Trim()))
            {
                MessageBox.Show("Mã sự cố không tồn tại để cập nhật!"); return;
            }
            string sql = @"UPDATE SuCo SET
                           Mats=@Mats,
                           Maphong=@Maphong,
                           NgayBao=@NgayBao,
                           TrangThaiXuLy=@TrangThai,
                           ChiPhiSuaDuKien=@ChiPhi,
                           MoTaSuCo=@MoTa
                           WHERE MaSC=@MaSC";

            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@MaSC", txtmasc.Text);
            cmd.Parameters.AddWithValue("@Mats", cbmats.SelectedValue);
            cmd.Parameters.AddWithValue("@Maphong", cbphong.SelectedValue);
            cmd.Parameters.AddWithValue("@NgayBao", dtngaybao.Value);
            cmd.Parameters.AddWithValue("@TrangThai", cbtrangthai.Text.Trim());
            cmd.Parameters.AddWithValue("@ChiPhi", decimal.Parse(txtchiphidukien.Text.Trim()));
            cmd.Parameters.AddWithValue("@MoTa", txtmota.Text.Trim());

            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();

            MessageBox.Show("Cập nhật thành công!");
            load_SuCo();
        }

        private void btnxoa_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtmasc.Text)) return;

            if (MessageBox.Show("Xóa bản ghi này?", "Xác nhận",
                MessageBoxButtons.YesNo) == DialogResult.No) return;
            if (!CheckTrungMa(txtmasc.Text.Trim()))
            {
                MessageBox.Show("Không tìm thấy mã này để xóa!"); return;
            }
            SqlCommand cmd = new SqlCommand("DELETE FROM SuCo WHERE MaSC=@MaSC", con);
            cmd.Parameters.AddWithValue("@MaSC", txtmasc.Text);

            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();

            MessageBox.Show("Xóa thành công!");
            load_SuCo();
        }

        private void btnloc_click(object sender, EventArgs e)
        {
            string sql = @"SELECT sc.MaSC, sc.Mats, sc.Maphong, sc.NgayBao, 
                                      sc.MoTaSuCo, sc.TrangThaiXuLy, sc.ChiPhiSuaDuKien,
                                      kt.Hoten AS TenKhach
                               FROM SuCo sc
                               INNER JOIN Phongtro p ON sc.Maphong = p.Maphong
                               LEFT JOIN KhachThue kt ON p.Makhach = kt.Makhach
                               WHERE MONTH(sc.NgayBao) = @thang AND YEAR(sc.NgayBao) = @nam";

            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@thang", int.Parse(cbthang.Text));
            cmd.Parameters.AddWithValue("@nam", int.Parse(cbnam.Text));

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            dgvsuco.DataSource = dt;
            CapNhatThongKe(dt);
        }
        // phần này đưa lên trên load_phong
        private void cbphong_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (cbphong.SelectedValue == null) return;

            string maphong = cbphong.SelectedValue.ToString();

            SqlDataAdapter da = new SqlDataAdapter(
                "SELECT Mats FROM TaiSan WHERE Maphong = @maphong", con);
            da.SelectCommand.Parameters.AddWithValue("@maphong", maphong);

            DataTable dt = new DataTable();
            da.Fill(dt);

            cbmats.DataSource = dt;
            cbmats.DisplayMember = "Mats";
            cbmats.ValueMember = "Mats";
        }

        private void UC_QuanLySuCo_Load(object sender, EventArgs e)
        {
            // 1. Xóa hết các dòng gán DataSource cũ cho cbnam trong code (nếu có)
            // 2. Chỉ thiết lập giá trị hiển thị mặc định
            cbthang.Text = DateTime.Now.Month.ToString();

            // Nếu Items nhập tay vẫn lỗi, hãy dùng code này để nạp lại chuẩn:
            cbnam.Items.Clear();
            for (int i = 2020; i <= 2030; i++)
            {
                cbnam.Items.Add(i.ToString());
            }
            cbnam.Text = DateTime.Now.Year.ToString();

            load_Phong();
            load_SuCo();
        }

        private void btntimkiem_click(object sender, EventArgs e)
        {
            // Lấy DataTable đang làm nguồn cho DataGridView
            DataTable dt = dgvsuco.DataSource as DataTable;

            if (dt == null) return;

            string filterText = txttimkiem.Text.Trim().Replace("'", "''"); // Tránh lỗi SQL Injection cho chuỗi lọc

            if (string.IsNullOrEmpty(filterText))
            {
                dt.DefaultView.RowFilter = ""; // Nếu trống thì hiện tất cả
            }
            else
            {
                // Lọc theo nhiều cột cùng lúc (Mã SC, Mã phòng, Tên tài sản, Mô tả)
                dt.DefaultView.RowFilter = string.Format(
                    "MaSC LIKE '%{0}%' OR Maphong LIKE '%{0}%' OR MoTaSuCo LIKE '%{0}%'",
                    filterText);
            }

            // Cập nhật lại các con số thống kê sau khi lọc
            CapNhatThongKe(dt.DefaultView.ToTable());
        }

        private void btnxuat_click(object sender, EventArgs e)
        {
            //Tạo các đối tượng Excel
            DataTable tb = (DataTable)dgvsuco.DataSource;
            if (tb == null || tb.Rows.Count == 0) return;
            ex_cel.Application oExcel = new ex_cel.Application();
            ex_cel.Workbooks oBooks;
            ex_cel.Sheets oSheets;
            ex_cel.Workbook oBook;
            ex_cel.Worksheet oSheet;
            //Tạo mới một Excel WorkBook
            oExcel.Visible = true;
            oExcel.DisplayAlerts = false;
            oExcel.Application.SheetsInNewWorkbook = 1;
            oBooks = oExcel.Workbooks;
            oBook = (ex_cel.Workbook)(oExcel.Workbooks.Add(Type.Missing));
            oSheets = oBook.Worksheets;
            oSheet = (ex_cel.Worksheet)oSheets.get_Item(1);
            oSheet.Name = "DANH SACH SU CO";
            // Tạo phần đầu nếu muốn
            ex_cel.Range head = oSheet.get_Range("A1", "H1");
            head.MergeCells = true;
            head.Value2 = "DANH SÁCH SỰ CỐ";
            head.Font.Bold = true;
            head.Font.Name = "Tahoma";
            head.Font.Size = "16";
            head.HorizontalAlignment = ex_cel.XlHAlign.xlHAlignCenter;
            // Tạo tiêu đề cột
            ex_cel.Range cl1 = oSheet.get_Range("A3", "A3");
            cl1.Value2 = "STT";
            cl1.ColumnWidth = 7.5;

            ex_cel.Range cl2 = oSheet.get_Range("B3", "B3");
            cl2.Value2 = "MÃ SỰ CỐ";
            cl2.ColumnWidth = 25.0;

            ex_cel.Range cl3 = oSheet.get_Range("C3", "C3");
            cl3.Value2 = "MÃ TÀI SẢN";
            cl3.ColumnWidth = 40.0;

            ex_cel.Range cl4 = oSheet.get_Range("D3", "D3");
            cl4.Value2 = "MÃ PHÒNG";
            cl4.ColumnWidth = 15.0;

            ex_cel.Range cl5 = oSheet.get_Range("E3", "E3");
            cl5.Value2 = "NGÀY BÁO";
            cl5.ColumnWidth = 25.0;

            ex_cel.Range cl6 = oSheet.get_Range("F3", "F3");
            cl6.Value2 = "MÔ TẢ SỰ CỐ";
            cl6.ColumnWidth = 20.0;
            //ex_cel.Range cl6_1 = oSheet.get_Range("F4", "F1000");
            //cl6_1.Columns.NumberFormat = "dd/mm/yyyy";


            ex_cel.Range cl7 = oSheet.get_Range("G3", "G3");
            cl7.Value2 = "TRẠNG THÁI XỬ LÝ";
            cl7.ColumnWidth = 40;
            //
            ex_cel.Range cl8 = oSheet.get_Range("H3", "H3");
            cl8.Value2 = "CHI PHÍ DỰ KIẾN";
            cl8.ColumnWidth = 40;


            ex_cel.Range rowHead = oSheet.get_Range("A3", "H3");
            rowHead.Font.Bold = true;
            // Kẻ viền
            rowHead.Borders.LineStyle = ex_cel.Constants.xlSolid;
            // Thiết lập màu nền
            rowHead.Interior.ColorIndex = 15;
            rowHead.HorizontalAlignment = ex_cel.XlHAlign.xlHAlignCenter;
            // ... (Phần khai báo Excel và tiêu đề giữ nguyên như code của bạn)

            // 1. Tạo mảng đối tượng với 8 cột tương ứng từ A đến H
            // Thứ tự mong muốn: STT(0), Mã(1), Tên(2), Ngày(3), Địa chỉ(4), Giới tính(5), SĐT(6), Email(7)
            object[,] arr = new object[tb.Rows.Count, 8];

            for (int r = 0; r < tb.Rows.Count; r++)
            {
                DataRow dr = tb.Rows[r];

                arr[r, 0] = r + 1;                  // Cột A: Số thứ tự (Tự tăng)
                arr[r, 1] = dr["MaSC"];         // Cột B:
                arr[r, 2] = dr["Mats"];          // Cột C:
                arr[r, 3] = dr["Maphong"];         // Cột D:
                       
                arr[r, 4] = dr["NgayBao"]; 
                arr[r, 5] = dr["MoTaSuCo"];         // Cột F: Giới tính
                // Cột F: Giới tính
                arr[r, 6] = dr["TrangThaiXuLy"];  
                arr[r, 7] = dr["ChiPhiSuaDuKien"];         // Cột F: Giới tính
                                                   // Cột F: Giới tính

                // Cột G: Email
            }

            // 2. Thiết lập vùng điền dữ liệu (Bắt đầu từ cột 1 là cột A)
            int rowStart = 4;
            int columnStart = 1;
            int rowEnd = rowStart + tb.Rows.Count - 1;
            int columnEnd = 8; // Kết thúc ở cột 7 (tương ứng cột G)

            ex_cel.Range c1 = (ex_cel.Range)oSheet.Cells[rowStart, columnStart];
            ex_cel.Range c2 = (ex_cel.Range)oSheet.Cells[rowEnd, columnEnd];
            ex_cel.Range range = oSheet.get_Range(c1, c2);

            // Đổ mảng vào Excel
            range.Value2 = arr;

            // 3. Kẻ viền cho vùng dữ liệu
            range.Borders.LineStyle = ex_cel.Constants.xlSolid;

            // 4. Sửa lỗi lệch định dạng Ngày sinh (Cột F)
            // Trong code của bạn Ngày sinh ở cột F, nên phải định dạng cột F
            ex_cel.Range cl_ngs = oSheet.get_Range("E" + rowStart, "E" + rowEnd);
            cl_ngs.NumberFormat = "dd/mm/yyyy";

            // 5. Căn giữa cột STT cho đẹp
            oSheet.get_Range("A" + rowStart, "A" + rowEnd).HorizontalAlignment = ex_cel.XlHAlign.xlHAlignCenter;

            // Tự động giãn độ rộng các cột cho vừa nội dung
            oSheet.get_Range("A3", "G" + rowEnd).Columns.AutoFit();

        }

        private void btnnhapfile_click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Excel Files|*.xlsx;*.xls";
            ofd.Title = "Chọn file Excel để xem dữ liệu";

            if (ofd.ShowDialog() != DialogResult.OK) return;

            string filename = ofd.FileName;

            // Khai báo các đối tượng COM Excel
            ex_cel.Application excelApp = null;
            ex_cel.Workbooks workbooks = null;
            ex_cel.Workbook workbook = null;
            ex_cel.Worksheet wsheet = null;

            try
            {
                excelApp = new ex_cel.Application();
                workbooks = excelApp.Workbooks;
                workbook = workbooks.Open(filename);
                wsheet = workbook.Sheets[1];
                ex_cel.Range range = wsheet.UsedRange;

                int rowCount = range.Rows.Count;
                int colCount = range.Columns.Count;

                // Tạo DataTable để chứa dữ liệu tạm thời
                DataTable dtExcel = new DataTable();

                // Tạo các cột cho DataTable dựa trên cấu trúc của DataGridView hoặc bảng SuCo
                dtExcel.Columns.Add("MaSC");
                dtExcel.Columns.Add("Mats");
                dtExcel.Columns.Add("Maphong");
                dtExcel.Columns.Add("NgayBao", typeof(DateTime));
                dtExcel.Columns.Add("MoTaSuCo");
                dtExcel.Columns.Add("TrangThaiXuLy");
                dtExcel.Columns.Add("ChiPhiSuaDuKien", typeof(decimal));

                // Duyệt dữ liệu từ dòng 2 (bỏ qua tiêu đề)
                for (int i = 2; i <= rowCount; i++)
                {
                    DataRow dr = dtExcel.NewRow();

                    // Đọc dữ liệu từ các Cell vào DataRow
                    dr["MaSC"] = wsheet.Cells[i, 1].Value?.ToString();
                    dr["Mats"] = wsheet.Cells[i, 2].Value?.ToString();
                    dr["Maphong"] = wsheet.Cells[i, 3].Value?.ToString();

                    // Xử lý ngày tháng
                    object cellNgay = wsheet.Cells[i, 4].Value;
                    if (cellNgay is DateTime) dr["NgayBao"] = (DateTime)cellNgay;
                    else if (cellNgay is double) dr["NgayBao"] = DateTime.FromOADate((double)cellNgay);
                    else dr["NgayBao"] = DateTime.Now;

                    dr["MoTaSuCo"] = wsheet.Cells[i, 5].Value?.ToString();
                    dr["TrangThaiXuLy"] = wsheet.Cells[i, 6].Value?.ToString() ?? "Chờ xử lý";

                    // Xử lý chi phí
                    object cellChiPhi = wsheet.Cells[i, 7].Value;
                    decimal chiPhi = 0;
                    if (cellChiPhi != null) decimal.TryParse(cellChiPhi.ToString(), out chiPhi);
                    dr["ChiPhiSuaDuKien"] = chiPhi;

                    dtExcel.Rows.Add(dr);
                }

                // Hiển thị lên DataGridView
                dgvsuco.DataSource = dtExcel;

                // Cập nhật lại các số liệu thống kê trên giao diện (lbltongsuco, lbltongchiphi...)
                CapNhatThongKe(dtExcel);

                MessageBox.Show("Đã tải dữ liệu từ file Excel lên bảng hiển thị!", "Thành công");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi đọc file Excel: " + ex.Message);
            }
            finally
            {
                // Giải phóng tài nguyên Excel
                if (workbook != null)
                {
                    workbook.Close(false);
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(workbook);
                }
                if (workbooks != null) System.Runtime.InteropServices.Marshal.ReleaseComObject(workbooks);
                if (excelApp != null)
                {
                    excelApp.Quit();
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(excelApp);
                }
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }
        
        
    }
}
