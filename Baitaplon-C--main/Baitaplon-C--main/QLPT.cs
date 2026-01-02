using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Excel = Microsoft.Office.Interop.Excel;

namespace Baitaplon
{
    public partial class QLPT : Form
    {
        // 1. CẤU HÌNH KẾT NỐI
        // Lưu ý: Thay 'DESKTOP-0IEDA9R' bằng tên máy của bạn nếu cần
        string connectionString = @"Data Source=NGUYENTRUNGKIEN\SQLEXPRESS;Initial Catalog=Baitaplon-C#;Integrated Security=True";

        // Biến lưu mã xe đang chọn (để sửa/xóa)
        int currentMaPT = -1;

        public QLPT()
        {
            InitializeComponent();
            this.Load += QLPT_Load;
            // QUAN TRỌNG: Tắt tự động tạo cột để dùng cột bạn tự thiết kế
            dgvPhuongTien.AutoGenerateColumns = false;

        }

        // --- LOAD FORM ---
        private void QLPT_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        // --- HÀM TẢI DỮ LIỆU (Dùng tên cột GỐC của Database) ---
        void LoadData()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    // Lấy dữ liệu giữ nguyên tên cột gốc, KHÔNG dùng "AS" đổi tên
                    string query = @"
                        SELECT 
                            pt.MaPT, 
                            k.Hoten,       -- Lấy từ bảng KhachThue
                            k.CCCD, 
                            k.SDT, 
                            k.Ngaysinh,
                            pt.Loaixe,     -- Lấy từ bảng PhuongTien
                            pt.Hieuxe, 
                            pt.Bienso, 
                            pt.Mauxe, 
                            pt.Ngaydangky
                        FROM PhuongTien pt
                        JOIN KhachThue k ON pt.Makhach = k.Makhach";

                    SqlDataAdapter da = new SqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    dgvPhuongTien.DataSource = dt;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi tải dữ liệu: " + ex.Message);
                }
            }
        }

        // --- SỰ KIỆN CLICK VÀO BẢNG (Map theo tên cột GỐC) ---
        private void dgvPhuongTien_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < dgvPhuongTien.Rows.Count)
            {
                DataGridViewRow row = dgvPhuongTien.Rows[e.RowIndex];

                // Kiểm tra dòng dữ liệu
                if (row.DataBoundItem == null) return;

                // Ép kiểu về DataRowView để lấy dữ liệu gốc
                DataRowView drv = (DataRowView)row.DataBoundItem;

                // Lấy MaPT
                currentMaPT = Convert.ToInt32(drv["MaPT"]);

                // Đổ dữ liệu lên TextBox (Dùng tên cột y hệt trong Database)
                txtTenChu.Text = drv["Hoten"].ToString();
                txtCCCD.Text = drv["CCCD"].ToString();
                txtSDT.Text = drv["SDT"].ToString();

                if (drv["Ngaysinh"] != DBNull.Value)
                    dtpNgaySinh.Value = Convert.ToDateTime(drv["Ngaysinh"]);

                txtLoaiXe.Text = drv["Loaixe"].ToString();
                txtHieuXe.Text = drv["Hieuxe"].ToString();
                txtBienSo.Text = drv["Bienso"].ToString();
                txtMauXe.Text = drv["Mauxe"].ToString();

                if (drv["Ngaydangky"] != DBNull.Value)
                    dtpNgayDangKy.Value = Convert.ToDateTime(drv["Ngaydangky"]);
            }
        }

        // --- HÀM HỖ TRỢ: LẤY ID KHÁCH TỪ CCCD ---
        private int GetMakhachByCCCD(string cccd)
        {
            int maKhach = -1;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    // Tìm mã khách trong bảng KhachThue dựa vào CCCD nhập vào
                    SqlCommand cmd = new SqlCommand("SELECT Makhach FROM KhachThue WHERE CCCD = @CCCD", conn);
                    cmd.Parameters.AddWithValue("@CCCD", cccd);
                    object result = cmd.ExecuteScalar();
                    if (result != null) maKhach = Convert.ToInt32(result);
                }
                catch { }
            }
            return maKhach;
        }

        // Hàm Reset ô nhập
        void ResetInput()
        {
            txtTenChu.Clear(); txtCCCD.Clear(); txtSDT.Clear();
            txtLoaiXe.Clear(); txtHieuXe.Clear(); txtBienSo.Clear(); txtMauXe.Clear();
            dtpNgayDangKy.Value = DateTime.Now; currentMaPT = -1;
        }

        // --- NÚT THÊM ---

        // --- HÀM MỚI: Tự động thêm khách hàng và trả về ID vừa tạo ---
        private int ThemMoiKhachHang(string hoten, string cccd, string sdt, DateTime ngaysinh)
        {
            int newId = -1;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    // Câu lệnh INSERT kết hợp SELECT SCOPE_IDENTITY() để lấy ngay ID vừa tạo
                    string sql = @"INSERT INTO KhachThue (Hoten, CCCD, SDT, Ngaysinh) 
                           VALUES (@Hoten, @CCCD, @SDT, @Ngaysinh);
                           SELECT CAST(SCOPE_IDENTITY() AS INT);";

                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@Hoten", hoten);
                    cmd.Parameters.AddWithValue("@CCCD", cccd);
                    cmd.Parameters.AddWithValue("@SDT", sdt);
                    cmd.Parameters.AddWithValue("@Ngaysinh", ngaysinh);

                    // Thực thi và lấy ID
                    newId = (int)cmd.ExecuteScalar();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi tạo khách hàng mới: " + ex.Message);
                    return -1;
                }
            }
            return newId;
        }
        private void btnThem_Click(object sender, EventArgs e)
        {
            // 1. Kiểm tra dữ liệu đầu vào (Bắt buộc phải có Tên và CCCD để lỡ tạo mới còn có dữ liệu)
            if (string.IsNullOrWhiteSpace(txtCCCD.Text) ||
                string.IsNullOrWhiteSpace(txtLoaiXe.Text) ||
                string.IsNullOrWhiteSpace(txtTenChu.Text))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ: Tên chủ xe, CCCD và Loại xe!");
                return;
            }

            // 2. Kiểm tra xem CCCD này đã tồn tại chưa
            int idKhach = GetMakhachByCCCD(txtCCCD.Text.Trim());

            // --- LOGIC XỬ LÝ ID ---
            if (idKhach == -1)
            {
                // TRƯỜNG HỢP 1: Khách chưa tồn tại -> Tự động tạo mới
                idKhach = ThemMoiKhachHang(
                    txtTenChu.Text.Trim(),
                    txtCCCD.Text.Trim(),
                    txtSDT.Text.Trim(),
                    dtpNgaySinh.Value
                );

                if (idKhach == -1) return; // Nếu lỗi khi tạo khách thì dừng lại
            }
            else
            {
                // TRƯỜNG HỢP 2: Khách đã tồn tại -> Chỉ cảnh báo nhẹ hoặc dùng luôn ID cũ
                // Ở đây ta dùng luôn ID cũ để cho phép 1 người có nhiều xe
                // (Tùy chọn: Bạn có thể cập nhật lại Tên/SĐT cho khách cũ ở đây nếu muốn)
            }

            // 3. Thêm xe vào Database (Dùng idKhach đã xác định ở trên)
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string sql = @"INSERT INTO PhuongTien (Makhach, Loaixe, Hieuxe, Bienso, Mauxe, Ngaydangky) 
                           VALUES (@Mk, @Lx, @Hx, @Bs, @Mx, @Ndk)";

                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@Mk", idKhach); // ID khách (cũ hoặc mới đều dùng được)
                    cmd.Parameters.AddWithValue("@Lx", txtLoaiXe.Text);
                    cmd.Parameters.AddWithValue("@Hx", txtHieuXe.Text);

                    // Xử lý null cho biển số và mẫu xe
                    cmd.Parameters.AddWithValue("@Bs", string.IsNullOrEmpty(txtBienSo.Text) ? (object)DBNull.Value : txtBienSo.Text);
                    cmd.Parameters.AddWithValue("@Mx", string.IsNullOrEmpty(txtMauXe.Text) ? (object)DBNull.Value : txtMauXe.Text);

                    cmd.Parameters.AddWithValue("@Ndk", dtpNgayDangKy.Value);

                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Thêm xe thành công!");
                    LoadData();
                    ResetInput();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi thêm xe: " + ex.Message);
                }
            }
        }

        // --- NÚT SỬA ---
        // Hàm tìm xem ai đang là chủ của cái xe này (Lấy ID Khách từ ID Xe)
        private int GetCurrentMakhachID(int maPT)
        {
            int makhach = -1;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    // Truy vấn ngược: Từ Mã PT tìm ra Mã Khách
                    string sql = "SELECT Makhach FROM PhuongTien WHERE MaPT = @MaPT";
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@MaPT", maPT);
                    object result = cmd.ExecuteScalar();
                    if (result != null) makhach = Convert.ToInt32(result);
                }
                catch { }
            }
            return makhach;
        }
        private void btnSua_Click(object sender, EventArgs e)
        {
            // 1. Kiểm tra đầu vào
            if (currentMaPT == -1)
            {
                MessageBox.Show("Vui lòng chọn xe cần sửa!");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtCCCD.Text) || string.IsNullOrWhiteSpace(txtTenChu.Text))
            {
                MessageBox.Show("Tên chủ và CCCD không được để trống!");
                return;
            }

            // 2. Lấy ID của người đang sở hữu xe này
            int idKhach = GetCurrentMakhachID(currentMaPT);

            if (idKhach == -1)
            {
                MessageBox.Show("Không tìm thấy dữ liệu chủ xe trong hệ thống!");
                return;
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlTransaction transaction = conn.BeginTransaction(); // Dùng Transaction để đảm bảo cả 2 lệnh cùng thành công

                try
                {
                    // BƯỚC A: Cập nhật thông tin cá nhân (Bảng KhachThue)
                    string sqlKhach = @"UPDATE KhachThue 
                                SET Hoten = @Hoten, 
                                    CCCD = @CCCD, 
                                    SDT = @SDT, 
                                    Ngaysinh = @Ngaysinh 
                                WHERE Makhach = @Makhach";

                    SqlCommand cmdKhach = new SqlCommand(sqlKhach, conn, transaction);
                    cmdKhach.Parameters.AddWithValue("@Hoten", txtTenChu.Text);
                    cmdKhach.Parameters.AddWithValue("@CCCD", txtCCCD.Text);
                    cmdKhach.Parameters.AddWithValue("@SDT", txtSDT.Text);
                    cmdKhach.Parameters.AddWithValue("@Ngaysinh", dtpNgaySinh.Value);
                    cmdKhach.Parameters.AddWithValue("@Makhach", idKhach);

                    cmdKhach.ExecuteNonQuery();

                    // BƯỚC B: Cập nhật thông tin xe (Bảng PhuongTien)
                    string sqlXe = @"UPDATE PhuongTien 
                             SET Loaixe = @Loaixe, 
                                 Hieuxe = @Hieuxe, 
                                 Bienso = @Bienso, 
                                 Mauxe = @Mauxe, 
                                 Ngaydangky = @Ngaydangky 
                             WHERE MaPT = @MaPT";

                    SqlCommand cmdXe = new SqlCommand(sqlXe, conn, transaction);
                    cmdXe.Parameters.AddWithValue("@Loaixe", txtLoaiXe.Text);
                    cmdXe.Parameters.AddWithValue("@Hieuxe", txtHieuXe.Text);
                    cmdXe.Parameters.AddWithValue("@Bienso", string.IsNullOrEmpty(txtBienSo.Text) ? (object)DBNull.Value : txtBienSo.Text);
                    cmdXe.Parameters.AddWithValue("@Mauxe", txtMauXe.Text);
                    cmdXe.Parameters.AddWithValue("@Ngaydangky", dtpNgayDangKy.Value);
                    cmdXe.Parameters.AddWithValue("@MaPT", currentMaPT);

                    cmdXe.ExecuteNonQuery();

                    // Nếu cả 2 lệnh trên ok thì chốt đơn
                    transaction.Commit();

                    MessageBox.Show("Đã cập nhật toàn bộ thông tin (Chủ xe & Phương tiện)!");
                    LoadData(); // Tải lại bảng
                    ResetInput();
                }
                catch (Exception ex)
                {
                    transaction.Rollback(); // Nếu lỗi thì hoàn tác lại như cũ
                    MessageBox.Show("Lỗi cập nhật: " + ex.Message + "\n(Lưu ý: CCCD không được trùng với người khác)");
                }
            }
        }

        // --- NÚT XÓA ---
        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (currentMaPT == -1) { MessageBox.Show("Vui lòng chọn xe cần xóa!"); return; }
            if (MessageBox.Show("Bạn chắc chắn muốn xóa?", "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    try
                    {
                        conn.Open();
                        SqlCommand cmd = new SqlCommand("DELETE FROM PhuongTien WHERE MaPT = @MaPT", conn);
                        cmd.Parameters.AddWithValue("@MaPT", currentMaPT);
                        cmd.ExecuteNonQuery();

                        MessageBox.Show("Đã xóa!");
                        LoadData(); ResetInput();
                    }
                    catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message); }
                }
            }
        }

        // --- NÚT XUẤT EXCEL ---
        private void btnXuat_Click(object sender, EventArgs e)
        {
            if (dgvPhuongTien.Rows.Count == 0) return;
            SaveFileDialog sfd = new SaveFileDialog { Filter = "Excel (*.xlsx)|*.xlsx", FileName = "DS_PhuongTien.xlsx" };
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                Excel.Application app = new Excel.Application();
                Excel.Workbook wb = app.Workbooks.Add(Type.Missing);
                Excel.Worksheet ws = wb.ActiveSheet;
                try
                {
                    for (int i = 1; i <= dgvPhuongTien.Columns.Count; i++) ws.Cells[1, i] = dgvPhuongTien.Columns[i - 1].HeaderText;
                    for (int i = 0; i < dgvPhuongTien.Rows.Count; i++)
                        for (int j = 0; j < dgvPhuongTien.Columns.Count; j++)
                            if (dgvPhuongTien.Rows[i].Cells[j].Value != null)
                                ws.Cells[i + 2, j + 1] = dgvPhuongTien.Rows[i].Cells[j].Value.ToString();
                    wb.SaveAs(sfd.FileName);
                    MessageBox.Show("Xuất Excel thành công!");
                }
                catch (Exception ex) { MessageBox.Show(ex.Message); }
                finally { app.Quit(); Marshal.ReleaseComObject(app); }
            }
        }

        // --- HÀM PHỤ: Insert dữ liệu từ Excel vào SQL ---
        private bool InsertXeFromExcel(int makhach, string loaixe, string hieuxe, string bienso, string mauxe)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = @"INSERT INTO PhuongTien (Makhach, Loaixe, Hieuxe, Bienso, Mauxe, Ngaydangky) 
                                     VALUES (@Mk, @Lx, @Hx, @Bs, @Mx, GETDATE())";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Mk", makhach);
                    cmd.Parameters.AddWithValue("@Lx", loaixe);
                    cmd.Parameters.AddWithValue("@Hx", string.IsNullOrEmpty(hieuxe) ? (object)DBNull.Value : hieuxe);
                    cmd.Parameters.AddWithValue("@Bs", string.IsNullOrEmpty(bienso) ? (object)DBNull.Value : bienso);
                    cmd.Parameters.AddWithValue("@Mx", string.IsNullOrEmpty(mauxe) ? (object)DBNull.Value : mauxe);

                    cmd.ExecuteNonQuery();
                    return true; // Thành công
                }
                catch
                {
                    return false; // Thất bại
                }
            }
        }
        // Nút Nhập (Để trống nếu chưa dùng)
        private void btnNhap_Click(object sender, EventArgs e) {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Excel Files|*.xls;*.xlsx;*.xlsm";
            ofd.Title = "Chọn file Excel chứa danh sách xe";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                // Khởi tạo các đối tượng Excel
                Excel.Application app = new Excel.Application();
                Excel.Workbook wb = null;
                Excel.Worksheet ws = null;

                try
                {
                    wb = app.Workbooks.Open(ofd.FileName);
                    ws = wb.Sheets[1]; // Lấy Sheet đầu tiên
                    Excel.Range range = ws.UsedRange;

                    int rowCount = range.Rows.Count;
                    int successCount = 0;
                    int failCount = 0;

                    // Duyệt từ dòng 2 (bỏ dòng tiêu đề Header)
                    for (int i = 2; i <= rowCount; i++)
                    {
                        // 1. Đọc dữ liệu từ từng ô (Cột 1: CCCD, 2: Loại, 3: Hiệu, 4: Biển, 5: Màu)
                        string cccd = range.Cells[i, 1].Value?.ToString().Trim();
                        string loaiXe = range.Cells[i, 2].Value?.ToString().Trim();
                        string hieuXe = range.Cells[i, 3].Value?.ToString().Trim();
                        string bienSo = range.Cells[i, 4].Value?.ToString().Trim();
                        string mauXe = range.Cells[i, 5].Value?.ToString().Trim();

                        // Kiểm tra dữ liệu rác hoặc dòng trống
                        if (string.IsNullOrEmpty(cccd) || string.IsNullOrEmpty(loaiXe))
                        {
                            continue; // Bỏ qua dòng này
                        }

                        // 2. Tìm ID khách hàng dựa trên CCCD (Quan trọng)
                        int idKhach = GetMakhachByCCCD(cccd);

                        if (idKhach != -1)
                        {
                            // 3. Nếu tìm thấy khách -> Thực hiện thêm xe
                            if (InsertXeFromExcel(idKhach, loaiXe, hieuXe, bienSo, mauXe))
                            {
                                successCount++;
                            }
                            else
                            {
                                failCount++;
                            }
                        }
                        else
                        {
                            // Không tìm thấy khách có CCCD này trong hệ thống
                            failCount++;
                        }
                    }

                    MessageBox.Show($"Kết quả nhập:\n- Thành công: {successCount} xe\n- Thất bại: {failCount} xe\n(Lưu ý: Thất bại do sai CCCD hoặc dữ liệu lỗi)", "Thông báo");
                    LoadData(); // Tải lại bảng để xem kết quả
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi đọc file Excel: " + ex.Message);
                }
                finally
                {
                    // Dọn dẹp bộ nhớ Excel (Bắt buộc để không bị treo máy)
                    if (wb != null) wb.Close(false);
                    if (app != null) app.Quit();

                    if (ws != null) Marshal.ReleaseComObject(ws);
                    if (wb != null) Marshal.ReleaseComObject(wb);
                    if (app != null) Marshal.ReleaseComObject(app);
                }
            }
        }

        private void btnTimKiem_Click(object sender, EventArgs e)
        {
            string tuKhoa = txtTimKiem.Text.Trim();

            // Nếu ô tìm kiếm trống -> Tải lại toàn bộ danh sách
            if (string.IsNullOrEmpty(tuKhoa))
            {
                LoadData();
                return;
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    // Câu lệnh tìm kiếm: Tìm theo Tên HOẶC CCCD HOẶC Biển số
                    // Sử dụng LIKE và dấu % để tìm kiếm gần đúng (chứa từ khóa)
                    string query = @"
                        SELECT 
                            pt.MaPT, 
                            k.Hoten, 
                            k.CCCD, 
                            k.SDT, 
                            k.Ngaysinh,
                            pt.Loaixe, 
                            pt.Hieuxe, 
                            pt.Bienso, 
                            pt.Mauxe, 
                            pt.Ngaydangky
                        FROM PhuongTien pt
                        JOIN KhachThue k ON pt.Makhach = k.Makhach
                        WHERE k.Hoten LIKE @kw 
                           OR k.CCCD LIKE @kw 
                           OR pt.Bienso LIKE @kw";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    // Thêm dấu % vào đầu và cuối để tìm kiếm 'chứa' từ khóa
                    cmd.Parameters.AddWithValue("@kw", "%" + tuKhoa + "%");

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    // Đổ dữ liệu tìm được lên bảng
                    dgvPhuongTien.DataSource = dt;

                    if (dt.Rows.Count == 0)
                    {
                        MessageBox.Show("Không tìm thấy kết quả nào!");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi tìm kiếm: " + ex.Message);
                }
            }
        }

       
    }
}