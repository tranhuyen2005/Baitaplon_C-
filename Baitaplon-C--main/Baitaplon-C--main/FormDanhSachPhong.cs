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
    public partial class FormDanhSachPhong : Form
    {
        string connectionString =
            @"Data Source=NGUYENTRUNGKIEN\SQLEXPRESS;Initial Catalog=Baitaplon-C#;Integrated Security=True";
        private string selectedMaPhong;

        public FormDanhSachPhong()
        {
            InitializeComponent();
            this.Load += FormDanhSachPhong_Load;
          dgvPhongTro.CellClick += dgvPhongTro_CellClick;

        }
        private void FormDanhSachPhong_Load(object sender, EventArgs e)
        {

            LoadPhong();
            SetupUI();
        }
        void LoadPhong()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string sql = "SELECT * FROM PhongTro";
                SqlDataAdapter da = new SqlDataAdapter(sql, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dgvPhongTro.DataSource = dt;
            }
        }
        string TaoMaPhong()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT ISNULL(MAX(CAST(SUBSTRING(Maphong, 2, 10) AS INT)), 0) + 1 FROM Phongtro";

                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();

                int number = Convert.ToInt32(cmd.ExecuteScalar());
                conn.Close();

                return "P" + number.ToString("000");
            }
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            string a = TaoMaPhong();  // Tạo mã phòng mới

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"INSERT INTO Phongtro
(Maphong, Tenphong, Dientich, Trangthaiphongtro, Maloaiphong, Makhach, Mats)
VALUES
(@Maphong, @Tenphong, @Dientich, @Trangthaiphongtro, @Maloaiphong, @Makhach, @Mats)";


                SqlCommand cmd = new SqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@Maphong", a);
                cmd.Parameters.AddWithValue("@Tenphong", txtTenPhong.Text);
                cmd.Parameters.AddWithValue("@Dientich", int.Parse(txtDienTich.Text));
                cmd.Parameters.AddWithValue("@Trangthaiphongtro", txtTrangThai.Text);

                // Nếu chưa dùng 3 cột này thì cho NULL
                cmd.Parameters.AddWithValue("@Maloaiphong", DBNull.Value);
                cmd.Parameters.AddWithValue("@Makhach", DBNull.Value);
                cmd.Parameters.AddWithValue("@Mats", DBNull.Value);

                conn.Open();
                cmd.ExecuteNonQuery();
            }

            LoadPhong();
            MessageBox.Show("Thêm phòng thành công!");
        }


        private void btnSua_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(selectedMaPhong))
            {
                MessageBox.Show("Vui lòng chọn phòng cần sửa.");
                return;
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"UPDATE Phongtro SET 
                        Tenphong = @Tenphong,
                        Dientich = @Dientich,
                        Trangthaiphongtro = @Trangthaiphongtro
                        WHERE Maphong = @Maphong";

                SqlCommand cmd = new SqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@Maphong", selectedMaPhong);
                cmd.Parameters.AddWithValue("@Tenphong", txtTenPhong.Text);
                cmd.Parameters.AddWithValue("@Dientich", int.Parse(txtDienTich.Text));
                cmd.Parameters.AddWithValue("@Trangthaiphongtro", txtTrangThai.Text);

                conn.Open();
                cmd.ExecuteNonQuery();
            }

            LoadPhong();
            MessageBox.Show("Sửa thành công!");
        }






        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(selectedMaPhong))
            {
                MessageBox.Show("Vui lòng chọn phòng cần xóa.");
                return;
            }

            if (MessageBox.Show("Xóa phòng này?", "Xác nhận",
                MessageBoxButtons.YesNo) == DialogResult.No) return;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand(
                    "DELETE FROM PhongTro WHERE Maphong = @Maphong", conn);

                cmd.Parameters.AddWithValue("@Maphong", selectedMaPhong);

                int rows = cmd.ExecuteNonQuery();
                MessageBox.Show("Đã xóa: " + rows + " dòng");

                LoadPhong();
                selectedMaPhong = ""; // reset khi xóa xong
            }
        }


        private void btnTimKiem_Click(object sender, EventArgs e)
        {
           
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string sql = @"SELECT * FROM PhongTro
                               WHERE MaPhong LIKE @key
                               OR TenPhong LIKE @key";

                SqlDataAdapter da = new SqlDataAdapter(sql, conn);
                da.SelectCommand.Parameters.AddWithValue(
                    "@key", "%" + txtTimKiem.Text + "%");

                DataTable dt = new DataTable();
                da.Fill(dt);
                dgvPhongTro.DataSource = dt;
            }
        }
        private void dgvPhongTro_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            DataGridViewRow r = dgvPhongTro.Rows[e.RowIndex];

            selectedMaPhong = r.Cells["Maphong"].Value.ToString();  // ⭐ LƯU MÃ PHÒNG

            txtTenPhong.Text = r.Cells["TenPhong"].Value.ToString();
            txtDienTich.Text = r.Cells["Dientich"].Value.ToString();
            txtTrangThai.Text = dgvPhongTro.Rows[e.RowIndex].Cells[3].Value.ToString();

        }

        void SetupUI()
        {
            // ===== FORM =====
            this.BackColor = Color.FromArgb(245, 247, 251);
            this.Font = new Font("Segoe UI", 10);

            // ===== TEXTBOX =====
            Guna2TextBox[] textBoxes =
            {
        txtTenPhong, txtDienTich,
        txtTrangThai, txtTimKiem
    };

            foreach (var txt in textBoxes)
            {
                txt.BorderRadius = 8;
                txt.Font = new Font("Segoe UI", 10);
            }

        
            txtTenPhong.PlaceholderText = "Tên phòng";
            txtDienTich.PlaceholderText = "Diện tích";
            txtTrangThai.PlaceholderText = "Trạng thái";
           
            txtTimKiem.PlaceholderText = "🔍 Tìm theo mã / tên phòng";

            // ===== BUTTON =====
            btnThem.FillColor = Color.FromArgb(34, 197, 94);
            btnThem.BorderRadius = 10;
            btnThem.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnThem.Text = "➕ Thêm";

            btnSua.FillColor = Color.FromArgb(59, 130, 246);
            btnSua.BorderRadius = 10;
            btnSua.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnSua.Text = "✏️ Sửa";

            btnXoa.FillColor = Color.FromArgb(239, 68, 68);
            btnXoa.BorderRadius = 10;
            btnXoa.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnXoa.Text = "❌ Xóa";

            btnTimKiem.FillColor = Color.FromArgb(79, 70, 229);
            btnTimKiem.BorderRadius = 8;
            btnTimKiem.Font = new Font("Segoe UI", 9, FontStyle.Bold);

            // ===== DATAGRIDVIEW =====
            dgvPhongTro.BorderStyle = BorderStyle.None;
            dgvPhongTro.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvPhongTro.RowTemplate.Height = 36;

            dgvPhongTro.ThemeStyle.HeaderStyle.BackColor = Color.FromArgb(79, 70, 229);
            dgvPhongTro.ThemeStyle.HeaderStyle.ForeColor = Color.White;
            dgvPhongTro.ThemeStyle.HeaderStyle.Font =
                new Font("Segoe UI", 10, FontStyle.Bold);

            dgvPhongTro.ThemeStyle.RowsStyle.Font =
                new Font("Segoe UI", 10);

            dgvPhongTro.AlternatingRowsDefaultCellStyle.BackColor =
                Color.FromArgb(245, 247, 251);

            dgvPhongTro.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            
          

        }

        private void dgvPhongTro_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void pnltitle_Click(object sender, EventArgs e)
        {

        }
    }
}
