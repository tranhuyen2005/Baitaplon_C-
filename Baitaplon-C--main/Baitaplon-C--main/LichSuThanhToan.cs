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
    public partial class LichSuThanhToan : Form
    {
        SqlConnection con = new SqlConnection("Data Source=NGUYENTRUNGKIEN\\SQLEXPRESS;Initial Catalog=Baitaplon-C#;Integrated Security=True");
        public LichSuThanhToan()
        {
            InitializeComponent();
        }

        private void btnxoa_Click(object sender, EventArgs e)
        {
            if (dgvLichSuTT.CurrentRow == null) return;

            string maHD = dgvLichSuTT.CurrentRow.Cells["MaHD"].Value.ToString();
            DialogResult dr = MessageBox.Show($"Bạn có chắc chắn muốn xóa hóa đơn mã {maHD}?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (dr == DialogResult.Yes)
            {
                try
                {
                    if (con.State == ConnectionState.Closed) con.Open();

                    // Xóa chi tiết hóa đơn trước vì có khóa ngoại, sau đó xóa hóa đơn
                    string sql = "DELETE FROM ChiTietHoaDon WHERE MaHoaDon = @id; DELETE FROM HoaDon WHERE MaHD = @id;";
                    SqlCommand cmd = new SqlCommand(sql, con);
                    cmd.Parameters.AddWithValue("@id", maHD);

                    int result = cmd.ExecuteNonQuery();
                    if (result > 0)
                    {
                        MessageBox.Show("Xóa thành công!");
                        LoadData();
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
        // Sự kiện khi Form load: Hiển thị dữ liệu ngay lập tức
        private void LichSuThanhToan_Load(object sender, EventArgs e)
        {
            LoadData();
            ConfigDataGridView();
        }
        // Hàm tải dữ liệu từ SQL Server lên DataGridView
        private void LoadData()
        {
            try
            {
                if (con.State == ConnectionState.Closed) con.Open();

                // Thêm điều kiện WHERE h.Trangthai = N'Đã trả'
                string sql = @"
            SELECT 
                h.MaHD, 
                p.Tenphong, 
                k.Hoten, 
                (CAST(h.Thang AS VARCHAR) + '/' + CAST(h.Nam AS VARCHAR)) AS KyThanhToan,
                h.Ngaylap, 
                h.Tongtien, 
                h.Trangthai
            FROM HoaDon h
            JOIN HopDong hd ON h.MaHopdong = hd.MaHD
            JOIN Phongtro p ON hd.Maphong = p.Maphong
            JOIN KhachThue k ON hd.Makhach = k.Makhach
            WHERE h.Trangthai = N'Đã trả'
            ORDER BY h.Nam DESC, h.Thang DESC";

                SqlDataAdapter da = new SqlDataAdapter(sql, con);
                DataTable dt = new DataTable();
                da.Fill(dt);

                dgvLichSuTT.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải dữ liệu: " + ex.Message);
            }
            finally { con.Close(); }
        }
        private void ConfigDataGridView()
        {
            if (dgvLichSuTT.Columns["Tongtien"] != null)
            {
                dgvLichSuTT.Columns["Tongtien"].DefaultCellStyle.Format = "N0"; // Định dạng 1,000,000
                dgvLichSuTT.Columns["Tongtien"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            }
            dgvLichSuTT.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void btntk_Click(object sender, EventArgs e)
        {
            try
            {
                if (con.State == ConnectionState.Closed) con.Open();
                string search = txttk.Text.Trim();

                string sql = @"
            SELECT 
                h.MaHD, p.Tenphong, k.Hoten, 
                (CAST(h.Thang AS VARCHAR) + '/' + CAST(h.Nam AS VARCHAR)) AS KyThanhToan,
                h.Ngaylap, h.Tongtien, h.Trangthai
            FROM HoaDon h
            JOIN HopDong hd ON h.MaHopdong = hd.MaHD
            JOIN Phongtro p ON hd.Maphong = p.Maphong
            JOIN KhachThue k ON hd.Makhach = k.Makhach
            WHERE h.Trangthai = N'Đã trả' 
            AND (p.Tenphong LIKE @search OR k.Hoten LIKE @search)"; // Thêm điều kiện lọc trạng thái

                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@search", "%" + search + "%");

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dgvLichSuTT.DataSource = dt;
            }
            catch (Exception ex) { MessageBox.Show("Lỗi tìm kiếm: " + ex.Message); }
            finally { con.Close(); }
        }

        private void btnmoi_Click(object sender, EventArgs e)
        {
            txttk.Clear();
            LoadData();
        }

        private void btnxuat_Click(object sender, EventArgs e)
        {
            if (dgvLichSuTT.Rows.Count > 0)
            {
                // Bạn cần cài đặt Reference Microsoft Excel Object Library
                // Code xuất Excel ở đây...
                MessageBox.Show("Chức năng xuất Excel đang được khởi tạo!", "Thông báo");
            }
        }

        private void dgvLichSuTT_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                // Hiển thị trạng thái của dòng đang chọn lên ComboBox cbTrangThai
                cbTrangthai.Text = dgvLichSuTT.Rows[e.RowIndex].Cells["Trangthai"].Value.ToString();
            }   
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            // Kiểm tra xem đã chọn hóa đơn nào chưa
            if (dgvLichSuTT.CurrentRow == null)
            {
                MessageBox.Show("Vui lòng chọn một hóa đơn từ danh sách để sửa!");
                return;
            }

            // Lấy mã hóa đơn (MaHD) từ dòng đang được chọn trong DataGridView
            string maHD = dgvLichSuTT.CurrentRow.Cells["MaHD"].Value.ToString();
            string trangThaiMoi = cbTrangthai.Text; // Lấy trạng thái mới từ ComboBox bạn đã thêm

            try
            {
                if (con.State == ConnectionState.Closed) con.Open();

                // Cập nhật lại trạng thái trong cơ sở dữ liệu
                string sql = "UPDATE HoaDon SET Trangthai = @trangthai WHERE MaHD = @id";
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@trangthai", trangThaiMoi);
                cmd.Parameters.AddWithValue("@id", maHD);

                int result = cmd.ExecuteNonQuery();
                if (result > 0)
                {
                    MessageBox.Show("Cập nhật trạng thái thành công!");

                    // Sau khi cập nhật, gọi lại LoadData() để làm mới bảng.
                    // Nếu bạn đổi từ 'Đã trả' sang 'Chưa trả', dòng đó sẽ biến mất khỏi form này 
                    // do điều kiện WHERE h.Trangthai = N'Đã trả' trong hàm LoadData() của bạn.
                    LoadData();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi cập nhật trạng thái: " + ex.Message);
            }
            finally
            {
                con.Close();
            }
        }
    }
}
