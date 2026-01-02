using Guna.UI2.WinForms;
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

namespace Baitaplon
{
    public partial class QLTS : Form
    {
        public QLTS()
        {
            InitializeComponent();
            this.Load += FormQLTaiSan_Load;

        }
        private readonly string connStr =
    @"Data Source=NGUYENTRUNGKIEN\SQLEXPRESS;Initial Catalog=Baitaplon-C#;Integrated Security=True";

        private string selectedMaTaiSan = "";
        private string selectedMaPhong = "";

        private void FormQLTaiSan_Load(object sender, EventArgs e)
        {
            SetupUI();
            LoadTaiSan();
        }
        string TaoMaTaiSan()
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = "SELECT ISNULL(MAX(MaTaiSan), 0) + 1 FROM TaiSan";

                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();

                int number = Convert.ToInt32(cmd.ExecuteScalar());

                return number.ToString();   // Trả ra số
            }
        }
        // ================= LOAD =================
        void LoadTaiSan()
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                SqlDataAdapter da = new SqlDataAdapter(
                    "SELECT Mats, Tents, Soluong, Tinhtrang, Maphong FROM TaiSan", conn);

                DataTable dt = new DataTable();
                da.Fill(dt);
                dgvTaiSan.DataSource = dt;
            }
        }

        // ================= THÊM TÀI SẢN =================
        private void btnThem_Click(object sender, EventArgs e)
        {
            if (txtTenTaiSan.Text == "" || txtSoLuong.Text == "")
            {
                MessageBox.Show("Vui lòng nhập đủ thông tin");
                return;
            }

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(
                    @"INSERT INTO TaiSan (Tents, Soluong, Tinhtrang, Maphong)
                      VALUES (@Ten, @SL, @TT, @Phong)", conn);

                cmd.Parameters.AddWithValue("@Ten", txtTenTaiSan.Text);
                cmd.Parameters.AddWithValue("@SL", int.Parse(txtSoLuong.Text));
                cmd.Parameters.AddWithValue("@TT", txtTinhTrang.Text);
                cmd.Parameters.AddWithValue("@Phong", txtMaPhong.Text);

                cmd.ExecuteNonQuery();
            }

            MessageBox.Show("Thêm tài sản thành công");
            LoadTaiSan();
        }



        // ================= SỬA =================
        private void btnSua_Click(object sender, EventArgs e)
        {


            // 1. Kiểm tra xem đã chọn tài sản chưa
            if (string.IsNullOrEmpty(selectedMaTaiSan))
            {
                MessageBox.Show("Vui lòng chọn tài sản cần sửa trên danh sách!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 2. Kiểm tra dữ liệu nhập vào
            if (txtTenTaiSan.Text.Trim() == "")
            {
                MessageBox.Show("Tên tài sản không được để trống!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(txtSoLuong.Text, out int soLuong))
            {
                MessageBox.Show("Số lượng phải là một số nguyên hợp lệ!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 3. Thực hiện Update
            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();
                    string query = @"UPDATE TaiSan 
                     SET Tents = @Ten,
                         Soluong = @SL,
                         Tinhtrang = @TT,
                         Maphong = @MP
                     WHERE Mats = @Ma";

                    SqlCommand cmd = new SqlCommand(query, conn);

                    cmd.Parameters.AddWithValue("@Ma", selectedMaTaiSan);
                    cmd.Parameters.AddWithValue("@Ten", txtTenTaiSan.Text.Trim());
                    cmd.Parameters.AddWithValue("@SL", soLuong);
                    cmd.Parameters.AddWithValue("@TT", txtTinhTrang.Text.Trim());

                    // QUAN TRỌNG: Lấy dữ liệu từ TextBox (txtMaPhong), KHÔNG lấy từ biến selectedMaPhong
                    if (string.IsNullOrWhiteSpace(txtMaPhong.Text))
                    {
                        cmd.Parameters.AddWithValue("@MP", DBNull.Value);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@MP", txtMaPhong.Text.Trim());
                    }

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Cập nhật tài sản thành công!", "Thông báo");
                        LoadTaiSan(); // Load lại dữ liệu

                        // Reset lại biến chọn để tránh lỗi thao tác nhầm lần sau
                        selectedMaTaiSan = "";
                        selectedMaPhong = "";

                        // Xóa trắng các ô nhập liệu (Tùy chọn)
                        txtTenTaiSan.Clear();
                        txtSoLuong.Clear();
                        txtTinhTrang.Clear();
                        txtMaPhong.Clear();
                    }
                    else
                    {
                        MessageBox.Show("Không tìm thấy tài sản để cập nhật (Có thể mã đã bị xóa).", "Lỗi");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi sửa dữ liệu: " + ex.Message, "Lỗi Hệ Thống");
            }
        }

        // ================= XÓA =================
        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(selectedMaTaiSan))
            {
                MessageBox.Show("Bạn chưa chọn dòng để xóa!");
                return;
            }

            if (MessageBox.Show("Xóa tài sản này?", "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.No)
                return;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(
                    "DELETE FROM TaiSan WHERE Mats = @Ma", conn);
                cmd.Parameters.AddWithValue("@Ma", selectedMaTaiSan);
                cmd.ExecuteNonQuery();
            }

            LoadTaiSan();
            MessageBox.Show("Đã xóa");
        }

        // ================= TÌM =================
        private void btnTim_Click(object sender, EventArgs e)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                SqlDataAdapter da = new SqlDataAdapter(
                    "SELECT * FROM TaiSan WHERE Tents LIKE @key", conn);
                da.SelectCommand.Parameters.AddWithValue("@key", "%" + txtTimKiem.Text + "%");

                DataTable dt = new DataTable();
                da.Fill(dt);
                dgvTaiSan.DataSource = dt;
            }
        }

        // ================= CLICK GRID =================
        private void dgvTaiSan_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            DataGridViewRow row = dgvTaiSan.Rows[e.RowIndex];

            selectedMaTaiSan = row.Cells[0].Value.ToString(); // Mats
            txtTenTaiSan.Text = row.Cells[1].Value.ToString();
            txtSoLuong.Text = row.Cells[2].Value.ToString();
            txtTinhTrang.Text = row.Cells[3].Value.ToString();
            selectedMaPhong = row.Cells[4].Value?.ToString();
        }


        // ================= UI =================
        void SetupUI()
        {
            // ===== FORM =====
            this.BackColor = Color.FromArgb(245, 247, 251);
            this.Font = new Font("Segoe UI", 10);

            // ===== TEXTBOX =====
            Guna2TextBox[] tbs =
            {

        txtTenTaiSan,
        txtSoLuong,
        txtTinhTrang,
        txtMaPhong,
        txtTimKiem
    };

            foreach (var tb in tbs)
            {
                tb.BorderRadius = 8;
                tb.Font = new Font("Segoe UI", 10);
            }
            txtTinhTrang.PlaceholderText = "Tình trạng";

            txtMaPhong.PlaceholderText = "Mã phòng";
            txtTenTaiSan.PlaceholderText = "Tên tài sản";
            txtSoLuong.PlaceholderText = "Sô lượng";
            txtTimKiem.PlaceholderText = "🔍 Tìm theo tên loại";

            // ===== BUTTON =====
            btnThem.BorderRadius = 10;
            btnThem.FillColor = Color.FromArgb(34, 197, 94);

            btnSua.BorderRadius = 10;
            btnSua.FillColor = Color.FromArgb(59, 130, 246);

            btnXoa.BorderRadius = 10;
            btnXoa.FillColor = Color.FromArgb(239, 68, 68);

            btnTimKiem.BorderRadius = 8;
            btnTimKiem.FillColor = Color.FromArgb(79, 70, 229);

            // ===== GRID =====
            dgvTaiSan.BorderStyle = BorderStyle.None;
            dgvTaiSan.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvTaiSan.RowTemplate.Height = 36;
        }


        private void guna2HtmlLabel15_Click(object sender, EventArgs e)
        {

        }

        private void dgvPhongTro_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void txtMaPhong_TextChanged(object sender, EventArgs e)
        {                    }

        private void guna2HtmlLabel1_Click(object sender, EventArgs e)
        {

        }

        private void guna2Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void guna2Panel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void txtTimKiem_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
