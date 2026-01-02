using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Baitaplon
{
    public partial class Giaodienchinh : Form
    {
        // --- KHAI BÁO BIẾN DÙNG CHUNG ---

        // 1. Màu sắc
        Color normalColor = Color.FromArgb(72, 89, 132); // Màu xanh tím than (Mặc định)
        Color activeColor = Color.FromArgb(255, 107, 53); // Màu cam (Khi đang chọn)

        // 2. Biến lưu tên người dùng
        private string _tenNguoiDung;

        // --- CONSTRUCTOR (HÀM KHỞI TẠO) ---

        // Constructor 1: Mặc định (Bắt buộc phải có để Designer chạy)
        public Giaodienchinh()
        {
            InitializeComponent();
        }

        // Constructor 2: Nhận tên từ Form Đăng nhập
        // ": this()" nghĩa là nó sẽ chạy Constructor 1 trước để vẽ giao diện, sau đó mới chạy lệnh trong này
        public Giaodienchinh(string tenNguoiDung) : this()
        {
            _tenNguoiDung = tenNguoiDung;
        }

        // 1. MENU QUẢN LÝ CƠ SỞ (Panel + 4 Nút con)
        bool isCoSoCollapsed = true;

        private void MenuTransition_Tick(object sender, EventArgs e)
        {
            // Chiều cao nút cha (45) + 4 nút con (4*40) = 205
            int chieuCaoToiDa = 165;
            int chieuCaoToiThieu = 45;

            if (isCoSoCollapsed) // MỞ RA
            {
                pnlCoSoContainer.Height += 10;
                if (pnlCoSoContainer.Height >= chieuCaoToiDa)
                {
                    pnlCoSoContainer.Height = chieuCaoToiDa;
                    MenuTransition.Stop();
                    isCoSoCollapsed = false;
                }
            }
            else // ĐÓNG LẠI
            {
                pnlCoSoContainer.Height -= 10;
                if (pnlCoSoContainer.Height <= chieuCaoToiThieu)
                {
                    pnlCoSoContainer.Height = chieuCaoToiThieu;
                    MenuTransition.Stop();
                    isCoSoCollapsed = true;

                    // Đóng xong -> Trả về màu cũ
                    btnQuanLyCoSo.FillColor = normalColor;
                }
            }
        }

        private void btnQuanLyCoSo_Click(object sender, EventArgs e)
        {
            // Bắt đầu mở -> Đổi màu CAM
            if (isCoSoCollapsed)
            {
                btnQuanLyCoSo.FillColor = activeColor;
            }
            MenuTransition.Start();
        }

        // 2. MENU QUẢN LÝ KHÁCH VÀ HỢP ĐỒNG
        bool isKhachCollapsed = true;

        private void KhachTransition_Tick(object sender, EventArgs e)
        {
            int chieuCaoToiDa = 180; // Tùy chỉnh theo số nút con thực tế
            int chieuCaoToiThieu = 45;

            if (isKhachCollapsed)
            {
                pnlKhachContainer.Height += 10;
                if (pnlKhachContainer.Height >= chieuCaoToiDa)
                {
                    pnlKhachContainer.Height = chieuCaoToiDa;
                    KhachTransition.Stop();
                    isKhachCollapsed = false;
                }
            }
            else
            {
                pnlKhachContainer.Height -= 10;
                if (pnlKhachContainer.Height <= chieuCaoToiThieu)
                {
                    pnlKhachContainer.Height = chieuCaoToiThieu;
                    KhachTransition.Stop();
                    isKhachCollapsed = true;

                    // Trả về màu cũ
                    btnQuanLyKhach.FillColor = normalColor;
                }
            }
        }

        private void btnQuanLyKhach_Click(object sender, EventArgs e)
        {
            if (isKhachCollapsed)
            {
                btnQuanLyKhach.FillColor = activeColor;
            }
            KhachTransition.Start();
        }
        
        // 3. MENU BÁO CÁO VÀ HỆ THỐNG
        bool isBaoCaoCollapsed = true;

        private void BaoCaoTransition_Tick(object sender, EventArgs e)
        {
            int chieuCaoToiDa = 180;
            int chieuCaoToiThieu = 45;

            if (isBaoCaoCollapsed)
            {
                pnlBaoCaoContainer.Height += 10;
                if (pnlBaoCaoContainer.Height >= chieuCaoToiDa)
                {
                    pnlBaoCaoContainer.Height = chieuCaoToiDa;
                    BaoCaoTransition.Stop();
                    isBaoCaoCollapsed = false;
                }
            }
            else
            {
                pnlBaoCaoContainer.Height -= 10;
                if (pnlBaoCaoContainer.Height <= chieuCaoToiThieu)
                {
                    pnlBaoCaoContainer.Height = chieuCaoToiThieu;
                    BaoCaoTransition.Stop();
                    isBaoCaoCollapsed = true;

                    // Trả về màu cũ
                    btnBaoCaovaHethong.FillColor = normalColor;
                }
            }
        }

        private void btnBaoCao_Click(object sender, EventArgs e)
        {
            if (isBaoCaoCollapsed)
            {
                btnBaoCaovaHethong.FillColor = activeColor;
            }
            BaoCaoTransition.Start();
        }

        // 4. MENU CÀI ĐẶT
        bool isCaiDatCollapsed = true;

        private void CaiDatTransition_Tick(object sender, EventArgs e)
        {
            int chieuCaoToiDa = 95; // 45 (Cha) + 45 (Con) + Padding
            int chieuCaoToiThieu = 45;

            if (isCaiDatCollapsed)
            {
                pnlCaiDatContainer.Height += 10;
                if (pnlCaiDatContainer.Height >= chieuCaoToiDa)
                {
                    pnlCaiDatContainer.Height = chieuCaoToiDa;
                    CaiDatTransition.Stop();
                    isCaiDatCollapsed = false;
                }
            }
            else
            {
                pnlCaiDatContainer.Height -= 10;
                if (pnlCaiDatContainer.Height <= chieuCaoToiThieu)
                {
                    pnlCaiDatContainer.Height = chieuCaoToiThieu;
                    CaiDatTransition.Stop();
                    isCaiDatCollapsed = true;

                    // Trả về màu cũ
                    btnCaiDat.FillColor = normalColor;
                }
            }
        }

        private void btnCaiDat_Click(object sender, EventArgs e)
        {
            if (isCaiDatCollapsed)
            {
                btnCaiDat.FillColor = activeColor;
            }
            CaiDatTransition.Start();
        }
        // SỰ KIỆN LOAD FORM (Khởi tạo ban đầu)
        private void Giaodienchinh_Load(object sender, EventArgs e)
        {
            // 1. Hiển thị tên người dùng (Nếu có)
            // LƯU Ý: Bạn phải tạo Label tên là lblTenNguoiDung bên Design trước
            if (!string.IsNullOrEmpty(_tenNguoiDung))
            {
                // Kiểm tra xem lblTenNguoiDung có null không để tránh lỗi
                if (lblTenNguoiDung != null)
                {
                    lblTenNguoiDung.Text = "Xin chào, " + _tenNguoiDung;
                }
            }

            // 2. Thu gọn tất cả các Menu về trạng thái đóng
            pnlCoSoContainer.Height = 45;
            isCoSoCollapsed = true;

            pnlKhachContainer.Height = 45;
            isKhachCollapsed = true;

            pnlBaoCaoContainer.Height = 45;
            isBaoCaoCollapsed = true;

            pnlHoadonContainer.Height = 45;
            isHoaDonCollapsed = true;

            pnlCaiDatContainer.Height = 45;
            isCaiDatCollapsed = true;
        }
        // 5. MENU QUẢN LÝ HÓA ĐƠN
        bool isHoaDonCollapsed = true;
        private void pnlHeader_Paint(object sender, PaintEventArgs e)
        {

        }

        private void guna2Button6_Click(object sender, EventArgs e)
        {

        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {

        }

        private void HoaDonTransition_Tick(object sender, EventArgs e)
        {
            if (isHoaDonCollapsed)
            {
                pnlHoadonContainer.Height += 10; // Giả sử Panel của bạn tên là pnlHoadonContainer
                if (pnlHoadonContainer.Height >= 165)
                { // Chiều cao khi mở hết
                    HoaDonTransition.Stop();
                    isHoaDonCollapsed = false;
                }
            }
            else
            {
                pnlHoadonContainer.Height -= 10;
                if (pnlHoadonContainer.Height <= 45)
                { // Chiều cao khi đóng
                    HoaDonTransition.Stop();
                    isHoaDonCollapsed = true;
                    btnQlyHoadon.FillColor = normalColor; // Trả về màu ban đầu
                }
            }
        }

        private void btnQlyHoadon_Click(object sender, EventArgs e)
        {
            if (isHoaDonCollapsed)
            {
                btnQlyHoadon.FillColor = activeColor; // Đổi sang màu cam khi nhấn
            }
            HoaDonTransition.Start();
        }
        //Mo form
        Form currentForm = null;
        private void openChildForm(Control childControl)
        {
            // 1. Xóa các điều khiển cũ trong panel nội dung
            pnlContent.Controls.Clear();

            // 2. Nếu đối tượng truyền vào là một Form, cần thiết lập các thuộc tính đặc thù
            if (childControl is Form childForm)
            {
                childForm.TopLevel = false;
                childForm.FormBorderStyle = FormBorderStyle.None;
            }

            // 3. Thiết lập thuộc tính chung cho cả Form và User Control
            childControl.Dock = DockStyle.Fill;

            // 4. Thêm vào panel và hiển thị
            pnlContent.Controls.Add(childControl);
            pnlContent.Tag = childControl;
            childControl.BringToFront();
            childControl.Show();
        }

        private void btnDongiaDichvu_Click(object sender, EventArgs e)
        {
            openChildForm(new DongiaDichvu());
        }

        private void btnHoadon_Click(object sender, EventArgs e)
        {
            openChildForm(new Hoadon());
        }

        private void btnLichsuThanhtoan_Click(object sender, EventArgs e)
        {
            openChildForm(new LichSuThanhToan());
        }

        private void guna2Button14_Click(object sender, EventArgs e)
        {
            openChildForm(new UC_QuanLySuCo());
        }

        private void guna2Button12_Click(object sender, EventArgs e)
        {
            openChildForm(new Formlichsuthuephong());
        }

        private void guna2Button11_Click(object sender, EventArgs e)
        {
            openChildForm(new Formhopdong());
        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            openChildForm(new Formkhachthue());
        }

        private void QlyPhuongtien_Click(object sender, EventArgs e)
        {
            openChildForm(new QLPT());
        }

        private void btnDanhsachphong_Click(object sender, EventArgs e)
        {
            openChildForm(new FormDanhSachPhong());
        }

        private void btnLoaiphong_Click(object sender, EventArgs e)
        {
            openChildForm(new Loaiphong2());
        }

        private void btnQlytaisan_Click(object sender, EventArgs e)
        {
            openChildForm(new QLTS());
        }

        private void btnBaocaoDoanhthu_Click(object sender, EventArgs e)
        {
            openChildForm(new UC_BaoCaoDoanhThu());
        }

        private void btnBaocaothongke_Click(object sender, EventArgs e)
        {
            openChildForm(new UC_BaoCaoThongKe());
        }

        private void btndangxuat_Click(object sender, EventArgs e)
        {
           
            DialogResult result = MessageBox.Show(
                "Bạn có chắc chắn muốn đăng xuất khỏi hệ thống?",
                "Xác nhận đăng xuất",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result == DialogResult.Yes)
            {
                Application.Restart();
                Environment.Exit(0);
            }
        }
    
    }
}