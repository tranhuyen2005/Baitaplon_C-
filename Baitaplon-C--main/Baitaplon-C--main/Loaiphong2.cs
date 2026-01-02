using Guna.UI2.WinForms;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;

namespace Baitaplon
{
    public partial class Loaiphong2 : Form
    {
        // 1. Chuỗi kết nối
        string connectionString = @"Data Source=NGUYENTRUNGKIEN\SQLEXPRESS;Initial Catalog=Baitaplon-C#;Integrated Security=True";

        // Biến lưu mã đang chọn để Sửa/Xóa
        int currentMaLoai = -1;

        public Loaiphong2()
        {
            InitializeComponent();
            this.Load += Loaiphong2_Load;

            // Cấu hình bảng hiển thị
            dgvLoaiPhong.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvLoaiPhong.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvLoaiPhong.MultiSelect = false;

            // Gắn sự kiện CellClick (Quan trọng)
            dgvLoaiPhong.CellClick += dgvLoaiPhong_CellClick;
        }

        private void Loaiphong2_Load(object sender, EventArgs e)
        {
            LoadData();
       
        }

        void LoadData()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    // Lấy dữ liệu và đặt tên cột hiển thị tiếng Việt
                    string query = "SELECT Maloaiphong AS [Mã loại], Tenloai AS [Tên loại phòng], Dongia AS [Đơn giá] FROM LoaiPhong";
                    SqlDataAdapter da = new SqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dgvLoaiPhong.DataSource = dt;

                    // Format cột đơn giá có dấu phân cách hàng nghìn
                    if (dgvLoaiPhong.Columns["Đơn giá"] != null)
                        dgvLoaiPhong.Columns["Đơn giá"].DefaultCellStyle.Format = "N0";
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi tải dữ liệu: " + ex.Message);
                }
            }
        }

        // --- HÀM RESET Ô NHẬP ---
        void ResetInput()
        {
            txtTenLoai.Clear();
            txtDonGia.Clear();
            txtTimKiem.Clear();
            currentMaLoai = -1;
            txtTenLoai.Focus();
        }

        // --- SỰ KIỆN CLICK VÀO BẢNG (ĐỂ LẤY DỮ LIỆU SỬA/XÓA) ---
        private void dgvLoaiPhong_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Kiểm tra nếu click vào dòng hợp lệ (không phải header)
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvLoaiPhong.Rows[e.RowIndex];

                // 1. Lấy Mã loại (Dùng tên cột Tiếng Việt khớp với câu SQL bên trên)
                if (row.Cells["Mã loại"].Value != null)
                {
                    currentMaLoai = Convert.ToInt32(row.Cells["Mã loại"].Value);
                }

                // 2. Lấy Tên loại
                if (row.Cells["Tên loại phòng"].Value != null)
                {
                    txtTenLoai.Text = row.Cells["Tên loại phòng"].Value.ToString();
                }

                // 3. Lấy Đơn giá (Xử lý xóa dấu phẩy nếu có)
                if (row.Cells["Đơn giá"].Value != null)
                {
                    // Lấy giá trị chuỗi, bỏ dấu phân cách ngàn (,) hoặc (.) tùy máy
                    string gia = row.Cells["Đơn giá"].Value.ToString();
                    // Cách đơn giản nhất: Lấy số nguyên
                    txtDonGia.Text = gia.Replace(",", "").Replace(".", "").Split(',')[0];
                }
            }
        }

        // --- NÚT THÊM ---
        private void btnthem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTenLoai.Text) || string.IsNullOrWhiteSpace(txtDonGia.Text))
            {
                MessageBox.Show("Vui lòng nhập tên loại và đơn giá!");
                return;
            }

            if (!decimal.TryParse(txtDonGia.Text, out decimal donGia))
            {
                MessageBox.Show("Đơn giá phải là số!"); return;
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string sql = "INSERT INTO LoaiPhong (Tenloai, Dongia) VALUES (@Ten, @Gia)";
                    SqlCommand cmd = new SqlCommand(sql, conn);

                    cmd.Parameters.AddWithValue("@Ten", txtTenLoai.Text);
                    cmd.Parameters.AddWithValue("@Gia", donGia);

                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Thêm thành công!");
                    LoadData();
                    ResetInput();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi thêm: " + ex.Message);
                }
            }
        }

        // --- NÚT SỬA ---
        private void btnSua_Click(object sender, EventArgs e)
        {
            // Kiểm tra xem đã chọn dòng nào chưa
            if (currentMaLoai == -1)
            {
                MessageBox.Show("Vui lòng chọn loại phòng cần sửa từ bảng!");
                return;
            }

            if (!decimal.TryParse(txtDonGia.Text, out decimal donGia))
            {
                MessageBox.Show("Đơn giá phải là số!"); return;
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string sql = "UPDATE LoaiPhong SET Tenloai=@Ten, Dongia=@Gia WHERE Maloaiphong=@Ma";
                    SqlCommand cmd = new SqlCommand(sql, conn);

                    cmd.Parameters.AddWithValue("@Ma", currentMaLoai); // Dùng biến toàn cục đã lấy từ CellClick
                    cmd.Parameters.AddWithValue("@Ten", txtTenLoai.Text);
                    cmd.Parameters.AddWithValue("@Gia", donGia);

                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Sửa thành công!");
                    LoadData();
                    ResetInput();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi sửa: " + ex.Message);
                }
            }
        }

        // --- NÚT XÓA ---
        private void btnxoa_Click(object sender, EventArgs e)
        {
            if (currentMaLoai == -1)
            {
                MessageBox.Show("Vui lòng chọn loại phòng cần xóa!");
                return;
            }

            if (MessageBox.Show("Bạn có chắc chắn muốn xóa?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    try
                    {
                        conn.Open();
                        string sql = "DELETE FROM LoaiPhong WHERE Maloaiphong=@Ma";
                        SqlCommand cmd = new SqlCommand(sql, conn);
                        cmd.Parameters.AddWithValue("@Ma", currentMaLoai);

                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Đã xóa!");
                        LoadData();
                        ResetInput();
                    }
                    catch (SqlException sqlEx)
                    {
                        if (sqlEx.Number == 547) // Lỗi khóa ngoại (đang được sử dụng)
                            MessageBox.Show("Không thể xóa vì loại phòng này đang có phòng trọ sử dụng!");
                        else
                            MessageBox.Show("Lỗi SQL: " + sqlEx.Message);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Lỗi: " + ex.Message);
                    }
                }
            }
        }

        // --- NÚT TÌM KIẾM ---
        private void btnTimKiem_Click(object sender, EventArgs e)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    // Tìm kiếm theo Tên hoặc Mã
                    string sql = @"SELECT Maloaiphong AS [Mã loại], Tenloai AS [Tên loại phòng], Dongia AS [Đơn giá] 
                                   FROM LoaiPhong 
                                   WHERE Tenloai LIKE @TuKhoa OR CAST(Maloaiphong AS NVARCHAR) LIKE @TuKhoa";

                    SqlDataAdapter da = new SqlDataAdapter(sql, conn);
                    da.SelectCommand.Parameters.AddWithValue("@TuKhoa", "%" + txtTimKiem.Text.Trim() + "%");

                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dgvLoaiPhong.DataSource = dt;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi tìm kiếm: " + ex.Message);
                }
            }
        }

        // --- NÚT LÀM MỚI ---
        private void btnmoi_Click(object sender, EventArgs e)
        {
            ResetInput();
            LoadData();
        }

        // --- NÚT XUẤT EXCEL ---
        private void btnxuat_Click(object sender, EventArgs e)
        {
            if (dgvLoaiPhong.Rows.Count == 0) return;

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Excel (*.xlsx)|*.xlsx";
            sfd.FileName = "DanhSachLoaiPhong.xlsx";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                Excel.Application app = new Excel.Application();
                Excel.Workbook wb = app.Workbooks.Add(Type.Missing);
                Excel.Worksheet ws = null;

                try
                {
                    ws = wb.Sheets[1];
                    ws.Name = "LoaiPhong";

                    // Ghi tiêu đề cột
                    for (int i = 1; i < dgvLoaiPhong.Columns.Count + 1; i++)
                    {
                        ws.Cells[1, i] = dgvLoaiPhong.Columns[i - 1].HeaderText;
                    }

                    // Ghi dữ liệu
                    for (int i = 0; i < dgvLoaiPhong.Rows.Count; i++)
                    {
                        for (int j = 0; j < dgvLoaiPhong.Columns.Count; j++)
                        {
                            if (dgvLoaiPhong.Rows[i].Cells[j].Value != null)
                            {
                                ws.Cells[i + 2, j + 1] = dgvLoaiPhong.Rows[i].Cells[j].Value.ToString();
                            }
                        }
                    }

                    wb.SaveAs(sfd.FileName);
                    MessageBox.Show("Xuất Excel thành công!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi xuất Excel: " + ex.Message);
                }
                finally
                {
                    app.Quit();
                    if (ws != null) System.Runtime.InteropServices.Marshal.ReleaseComObject(ws);
                    if (wb != null) System.Runtime.InteropServices.Marshal.ReleaseComObject(wb);
                    if (app != null) System.Runtime.InteropServices.Marshal.ReleaseComObject(app);
                }
            }
        }

        // Sự kiện TextChanged (Có thể để trống nếu không dùng chức năng tìm kiếm real-time)
        private void txtTimKiem_TextChanged(object sender, EventArgs e)
        {
            // Nếu muốn tìm kiếm ngay khi gõ phím thì gọi hàm btnTimKiem_Click(null, null);
        }
    }
}