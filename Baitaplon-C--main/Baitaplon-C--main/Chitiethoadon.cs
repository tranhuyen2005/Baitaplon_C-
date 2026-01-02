using Microsoft.Office.Interop.Excel;
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
    public partial class Chitiethoadon : Form
    {
        SqlConnection con = new SqlConnection("Data Source=NGUYENTRUNGKIEN\\SQLEXPRESS;Initial Catalog=Baitaplon-C#;Integrated Security=True");
        int currentMaHD;
        public Chitiethoadon()
        {
            InitializeComponent();
        }
        public Chitiethoadon(int maHD)
        {
            InitializeComponent();
            currentMaHD = maHD;
            SetupGui(); // Hàm khóa các ô không cho sửa
            this.Load += Chitiethoadon_Load;

        }
        private void SetupGui()
        {
            // Khóa các ô không cho phép nhập tay để đảm bảo tính chính xác của dữ liệu
            txtTenkhoanmuc.ReadOnly = true;
            txtThanhtien.ReadOnly = true;
            txtDiencu.ReadOnly = true;
            txtDienmoi.ReadOnly = true;
            txtNuoccu.ReadOnly = true;
            txtNuocmoi.ReadOnly = true;
        }
        private void Chitiethoadon_Load(object sender, EventArgs e)
        {
            LoadDataChiTiet(); // Tải dữ liệu vào GridView
            LoadThongTinDienNuoc(); // Tải dữ liệu vào các ô TextBox điện nước
        }
        private void LoadThongTinDienNuoc()
        {
            try
            {
                if (con.State == ConnectionState.Closed) con.Open();

                // Lấy đơn giá điện (DV01) và nước (DV02) trực tiếp từ bảng Dichvu
                string sql = @"
                    SELECT cs.*
                    FROM ChiSoDienNuoc cs 
                    JOIN HopDong hd ON cs.Maphong = hd.Maphong
                    JOIN HoaDon h ON hd.MaHD = h.MaHopdong 
                    WHERE h.MaHD = @maHD AND cs.Thang = h.Thang AND cs.Nam = h.Nam";

                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@maHD", currentMaHD);
                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    // Đổ dữ liệu điện
                    txtDiencu.Text = dr["Chisodiencu"].ToString();
                    txtDienmoi.Text = dr["Chisodienmoi"].ToString();

                    // Đổ dữ liệu nước
                    txtNuoccu.Text = dr["Chisonuoccu"].ToString();
                    txtNuocmoi.Text = dr["Chisonuocmoi"].ToString();
                }
                dr.Close();
            }
            catch (Exception ex) { MessageBox.Show("Lỗi load chỉ số: " + ex.Message); }
            finally { con.Close(); }
        }
        private void LoadDataChiTiet()
        {
            try
            {
                if (con.State == ConnectionState.Closed) con.Open();

                // Truy vấn lấy đúng các cột hiển thị trên DataGridView của bạn
                string sql = @"
            SELECT 
                hd.Maphong, 
                ct.Tenkhoanmuc, 
                ct.Soluong, 
                ct.Dongia, 
                ct.Thanhtien, 
                h.Thang, 
                h.Nam, 
                cs.Chisodiencu, 
                cs.Chisodienmoi, 
                cs.Chisonuoccu, 
                cs.Chisonuocmoi, 
                h.Tongtien,
                ct.MaCT
            FROM ChiTietHoaDon ct
            JOIN HoaDon h ON ct.MaHoaDon = h.MaHD
            JOIN HopDong hd ON h.MaHopdong = hd.MaHD
            LEFT JOIN ChiSoDienNuoc cs ON hd.Maphong = cs.Maphong 
                 AND h.Thang = cs.Thang AND h.Nam = cs.Nam
            WHERE ct.MaHoaDon = @maHD";

                SqlDataAdapter da = new SqlDataAdapter(sql, con);
                da.SelectCommand.Parameters.AddWithValue("@maHD", currentMaHD);

                System.Data.DataTable dt = new System.Data.DataTable();
                da.Fill(dt);
                dgvChitiethoadon.DataSource = dt;

                // Ẩn cột MaCT để giao diện đẹp hơn (vẫn dùng để sửa dữ liệu)
                if (dgvChitiethoadon.Columns["MaCT"] != null)
                    dgvChitiethoadon.Columns["MaCT"].Visible = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải dữ liệu: " + ex.Message);
            }
            finally { con.Close(); }
        }
        private void dgvChitiethoadon_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvChitiethoadon.Rows[e.RowIndex];

                // Đổ thông tin khoản mục
                txtTenkhoanmuc.Text = row.Cells["Tenkhoanmuc"].Value?.ToString();
                txtSoluong.Text = row.Cells["Soluong"].Value?.ToString();
                txtDongia.Text = row.Cells["Dongia"].Value?.ToString();
                txtThanhtien.Text = row.Cells["Thanhtien"].Value?.ToString();

                // Đổ thông tin chỉ số điện nước
                txtDiencu.Text = row.Cells["Chisodiencu"].Value?.ToString();
                txtDienmoi.Text = row.Cells["Chisodienmoi"].Value?.ToString();
                txtNuoccu.Text = row.Cells["Chisonuoccu"].Value?.ToString();
                txtNuocmoi.Text = row.Cells["Chisonuocmoi"].Value?.ToString();
            }
        }



        private void btnSua_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvChitiethoadon.CurrentRow == null)
                {
                    MessageBox.Show("Vui lòng chọn một dòng để sửa!");
                    return;
                }

                int maCT = Convert.ToInt32(dgvChitiethoadon.CurrentRow.Cells["MaCT"].Value);

                // Kiểm tra tính hợp lệ của Số lượng và Đơn giá
                if (!decimal.TryParse(txtSoluong.Text, out decimal soLuong) ||
                    !decimal.TryParse(txtDongia.Text, out decimal donGia))
                {
                    MessageBox.Show("Số lượng và Đơn giá phải là số!");
                    return;
                }

                if (con.State == ConnectionState.Closed) con.Open();

                // 4. Sửa lại SQL: Cập nhật trực tiếp giá trị từ TextBox vào DB
                // Không dùng CASE WHEN cứng nhắc nữa để bạn có thể tùy chỉnh giá
                string sqlUpdateChiTiet = @"
            UPDATE ChiTietHoaDon 
            SET Soluong = @sl, 
                Dongia = @dg
            WHERE MaCT = @maCT";

                SqlCommand cmd = new SqlCommand(sqlUpdateChiTiet, con);
                cmd.Parameters.Add("@sl", SqlDbType.Decimal).Value = soLuong;
                cmd.Parameters.Add("@dg", SqlDbType.Decimal).Value = donGia;
                cmd.Parameters.Add("@maCT", SqlDbType.Int).Value = maCT;
                cmd.ExecuteNonQuery();

                // 5. Cập nhật lại Tổng tiền cho bảng HoaDon
                string sqlUpdateTongTien = @"
            UPDATE HoaDon 
            SET Tongtien = (SELECT SUM(Thanhtien) FROM ChiTietHoaDon WHERE MaHoaDon = @maHD)
            WHERE MaHD = @maHD";

                SqlCommand cmdTong = new SqlCommand(sqlUpdateTongTien, con);
                cmdTong.Parameters.AddWithValue("@maHD", currentMaHD);
                cmdTong.ExecuteNonQuery();

                MessageBox.Show("Cập nhật thành công!");
                LoadDataChiTiet(); // Làm mới GridView ngay lập tức
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
            finally
            {
                con.Close();
            }
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnXuat_Click(object sender, EventArgs e)
        {
            // Lấy dữ liệu từ DataSource của DataGridView
            System.Data.DataTable dt = (System.Data.DataTable)dgvChitiethoadon.DataSource;
            if (dt != null && dt.Rows.Count > 0)
            {
                ExportExcel(dt, "HoaDonChiTiet");
            }
            else
            {
                MessageBox.Show("Không có dữ liệu để xuất!");
            }
        }

        public void ExportExcel(System.Data.DataTable tb, string sheetname)
        {
            try
            {
                ex_cel.Application oExcel = new ex_cel.Application();
                ex_cel.Workbook oBook = oExcel.Workbooks.Add(Type.Missing);
                ex_cel.Worksheet oSheet = (ex_cel.Worksheet)oBook.Worksheets.get_Item(1);
                oExcel.Visible = true;
                oSheet.Name = sheetname;

                // Lấy thông tin chung từ dòng đầu tiên của DataTable
                string maPhong = tb.Rows[0]["Maphong"].ToString();
                string thang = tb.Rows[0]["Thang"].ToString();
                string nam = tb.Rows[0]["Nam"].ToString();
                string tongTien = tb.Rows[0]["Tongtien"].ToString();

                // Chỉ số điện nước (Lấy 1 lần duy nhất)
                string dienCu = tb.Rows[0]["Chisodiencu"].ToString();
                string dienMoi = tb.Rows[0]["Chisodienmoi"].ToString();
                string nuocCu = tb.Rows[0]["Chisonuoccu"].ToString();
                string nuocMoi = tb.Rows[0]["Chisonuocmoi"].ToString();

                // 1. Tiêu đề chính
                ex_cel.Range head = oSheet.get_Range("A1", "E1");
                head.MergeCells = true;
                head.Value2 = "CHI TIẾT HÓA ĐƠN PHÒNG " + maPhong;
                head.Font.Bold = true; head.Font.Size = 16;
                head.HorizontalAlignment = ex_cel.XlHAlign.xlHAlignCenter;

                // 2. HIỂN THỊ THÔNG TIN CHUNG (Tránh bị lặp ở dưới bảng)
                oSheet.get_Range("A2").Value2 = "Tháng/Năm: " + thang + "/" + nam;

                // Hiển thị chỉ số điện nước thành một dòng riêng biệt phía trên bảng
                ex_cel.Range infoDienNuoc = oSheet.get_Range("A3", "E3");
                infoDienNuoc.MergeCells = true;
                infoDienNuoc.Value2 = $"Điện (Cũ: {dienCu} - Mới: {dienMoi}) | Nước (Cũ: {nuocCu} - Mới: {nuocMoi})";
                infoDienNuoc.Font.Italic = true;

                // 3. Tạo tiêu đề cột cho BẢNG CHI TIẾT (Dòng 5)
                string[] headers = { "STT", "Tên khoản mục", "Số lượng", "Đơn giá", "Thành tiền" };
                for (int i = 0; i < headers.Length; i++)
                {
                    ex_cel.Range cl = (ex_cel.Range)oSheet.Cells[5, i + 1];
                    cl.Value2 = headers[i];
                    cl.Font.Bold = true;
                    cl.Borders.LineStyle = ex_cel.Constants.xlSolid;
                    cl.Interior.ColorIndex = 15;
                    cl.HorizontalAlignment = ex_cel.XlHAlign.xlHAlignCenter;
                }

                // 4. Đổ dữ liệu vào mảng (Chỉ lấy các cột khác nhau)
                object[,] arr = new object[tb.Rows.Count, 5];
                for (int r = 0; r < tb.Rows.Count; r++)
                {
                    arr[r, 0] = r + 1; // STT
                    arr[r, 1] = tb.Rows[r]["Tenkhoanmuc"];
                    arr[r, 2] = tb.Rows[r]["Soluong"];
                    arr[r, 3] = tb.Rows[r]["Dongia"];
                    arr[r, 4] = tb.Rows[r]["Thanhtien"];
                }

                // 5. Thiết lập vùng điền dữ liệu (Bắt đầu từ dòng 6)
                int rowStart = 6;
                int rowEnd = rowStart + tb.Rows.Count - 1;

                ex_cel.Range c1 = (ex_cel.Range)oSheet.Cells[rowStart, 1];
                ex_cel.Range c2 = (ex_cel.Range)oSheet.Cells[rowEnd, 5];
                ex_cel.Range range = oSheet.get_Range(c1, c2);

                range.Value2 = arr;
                range.Borders.LineStyle = ex_cel.Constants.xlSolid;

                // 6. Dòng tổng cộng cuối bảng
                int rowTong = rowEnd + 1;
                ex_cel.Range cellTongText = oSheet.get_Range("A" + rowTong, "D" + rowTong);
                cellTongText.MergeCells = true;
                cellTongText.Value2 = "TỔNG CỘNG THÀNH TIỀN:";
                cellTongText.Font.Bold = true;
                cellTongText.HorizontalAlignment = ex_cel.XlHAlign.xlHAlignRight;

                ex_cel.Range cellTongSo = oSheet.get_Range("E" + rowTong);
                cellTongSo.Value2 = tongTien;
                cellTongSo.Font.Bold = true;
                cellTongSo.Font.Color = ColorTranslator.ToOle(Color.Red);
                cellTongSo.NumberFormat = "#,##0";
                cellTongSo.Borders.LineStyle = ex_cel.Constants.xlSolid;

                // Định dạng cột tiền
                oSheet.get_Range("D6", "E" + rowEnd).NumberFormat = "#,##0";
                oSheet.get_Range("B5").ColumnWidth = 35;
                oSheet.get_Range("C5", "E5").ColumnWidth = 15;

                MessageBox.Show("Xuất hóa đơn thành công!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
        }
    }
}

