using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ex_cel=Microsoft.Office.Interop.Excel;
namespace Tacgia
{
    public partial class Form1 : Form
    {
        SqlConnection con=new SqlConnection("Data Source=NGUYENTRUNGKIEN\\SQLEXPRESS;Initial Catalog=sach;Integrated Security=True");
        public Form1()
        {
            InitializeComponent();
            load_Tacgia();
        }
        private void load_Tacgia()
        {
            //b1: ket noi voi database
            if(con.State==ConnectionState.Closed)
            {
                con.Open();
            }
            //b2:tao doi tuong command de lay du lieu tu bang Tacgia
            SqlCommand cmd = new SqlCommand("select * from Tacgia", con);
            //b3: tao doi tuong dataAdapter de lay du lieu tu cmd
            SqlDataAdapter da=new SqlDataAdapter(cmd);
            //da.SelectCommand = cmd; neu khong khai ba trong ngoac thi phai khai bao rieng ra ngoai
            //b4: Tao doi tuong datatable de lay du lieu tu da
            DataTable tb=new DataTable();
            da.Fill(tb);
            //giai phong cmd
            cmd.Dispose();
            //ngat ket noi
            con.Close();
            //b5: doi du lieu tu tb vao datagridview
            dgvdanhsach.DataSource = tb;
            dgvdanhsach.Refresh();
        }
        //ham check trung ma tac gia tra ve gia tri TRUE neu khong thi tra ve gia tri FALSE
        private bool checktrungMTG(string mtg)
        {
            if (con.State == ConnectionState.Closed)
            {
                con.Open();
            }

            string sql = "select count(*) from Tacgia where Matacgia=@mtg";
            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.Add("@mtg", SqlDbType.NChar, 50).Value = mtg;
            int kq = (int)cmd.ExecuteScalar();
            con.Close();

            if (kq > 0)
                return true;
            else
                return false;
        }

        private void btnluu_Click(object sender, EventArgs e)
        {
            string mtg = txtmtg.Text.Trim();
            string ht=txthoten.Text.Trim();
            DateTime ns = dtngaysinh.Value;
            string dc=txtdiachi.Text.Trim();
            string gt = cbgioitinh.SelectedItem.ToString();
            string dt=txtdienthoai.Text.Trim();
            string mail=txtemail.Text.Trim();
            //kiem tra du lieu rong
            if(mtg=="")
            {
                txtmtg.Focus();
                MessageBox.Show("Ma tac gia khong uoc trong!");
                return;
            }
            if(ht=="")
            {
                txthoten.Focus();
                MessageBox.Show("Ho ten khong duoc trong!");
                return;
            }
            //kiem tra trung ma tac gia
            if (checktrungMTG(mtg))
            {
                txtmtg.Focus();
                MessageBox.Show("Trung ma tac gia");
                return;
            }
            //kiem tra ngay sinh
            if (ns >= DateTime.Now)
            {
                dtngaysinh.Focus();
                MessageBox.Show("Ngay sinh phai nho hon ngay hien tai!");
                return;
            }
            // 1. Kiểm tra độ dài phải đúng 10
            if (dt.Length != 10 || !dt.All(char.IsDigit))
            {
                MessageBox.Show("So dien thoai phai la so va co 10 chu so!");
                txtdienthoai.Focus();
                return;
            }
            // 6. KIỂM TRA EMAIL (Phải có đuôi @gmail.com)
            if (!mail.EndsWith("@gmail.com") )
            {
                MessageBox.Show("Email phai dung dinh dang @gmail.com!", "Thông báo");
                txtemail.Focus();
                return;
            }
            //b2: ket noi voi databse
            if (con.State==ConnectionState.Closed)
            {
                con.Open();
                //b3: tao doi tuong command de thuc thi cau lenh chen du lieu vao bang Tacgia
                //string ql = "Insert Tacgia Values('" + mtg + "',N'" + ht + "','" + ns + "',N'" + dc + "','" + gt + "',N'" + dt + "','" + email + "')";
                string sql = "Insert Tacgia Values(@mtg,@ht,@ns,@dc,@gt,@dt,@email)";
                SqlCommand cmd=new SqlCommand(sql, con);
                cmd.Parameters.Add("@mtg", SqlDbType.NChar, 50).Value = mtg;
                cmd.Parameters.Add("ht", SqlDbType.NChar, 50).Value = ht;
                cmd.Parameters.Add("@ns", SqlDbType.Date).Value = ns;
                cmd.Parameters.Add("@dc", SqlDbType.NChar, 200).Value = dc;
                cmd.Parameters.Add("@gt", SqlDbType.NChar, 50).Value = gt;
                cmd.Parameters.Add("dt", SqlDbType.NChar, 50).Value = dt;
                cmd.Parameters.Add("@email", SqlDbType.NChar, 50).Value = mail;
                cmd.ExecuteNonQuery();
                MessageBox.Show("Them moi thanh cong!");
                load_Tacgia();
            }
        }

        

        private void dgvdanhsach_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Bước 1: Kiểm tra nếu nhấn vào tiêu đề hoặc vùng trống ngoài danh sách
            if (e.RowIndex < 0 || e.RowIndex >= dgvdanhsach.Rows.Count) return;

            DataGridViewRow row = dgvdanhsach.Rows[e.RowIndex];

            // Bước 2: Kiểm tra nếu đây là dòng mới (dòng trống cuối cùng)
            if (row.IsNewRow) return;

            try
            {
                // Bước 3: Gán dữ liệu an toàn bằng cách kiểm tra DBNull
                txtmtg.Text = row.Cells[0].Value?.ToString() ?? "";
                txthoten.Text = row.Cells[1].Value?.ToString() ?? "";

                // Xử lý riêng cho Ngày sinh (Tránh lỗi DBNull to DateTime)
                if (row.Cells[2].Value != null && row.Cells[2].Value != DBNull.Value)
                {
                    dtngaysinh.Value = Convert.ToDateTime(row.Cells[2].Value);
                }

                txtdiachi.Text = row.Cells[3].Value?.ToString() ?? "";

                // Xử lý ComboBox Giới tính an toàn
                string gtValue = row.Cells[4].Value?.ToString()?.Trim() ?? "";
                cbgioitinh.Text = gtValue;

                txtdienthoai.Text = row.Cells[5].Value?.ToString() ?? "";
                txtemail.Text = row.Cells[6].Value?.ToString() ?? "";

                // Khóa mã tác giả khi đang sửa
                txtmtg.Enabled = false;
            }
            catch (Exception ex)
            {
                // Tránh hiện thông báo lỗi hệ thống làm gián đoạn người dùng
                Console.WriteLine("Lỗi định dạng dòng: " + ex.Message);
            }
        }

        private void btnsua_Click(object sender, EventArgs e)
        {
            string mtg = txtmtg.Text.Trim();
            string ht = txthoten.Text.Trim();
            DateTime ns = dtngaysinh.Value;
            string dc = txtdiachi.Text.Trim();
            string gt = cbgioitinh.SelectedItem?.ToString() ?? "";
            string dt = txtdienthoai.Text.Trim();
            string em = txtemail.Text.Trim();

            if (con.State == ConnectionState.Closed) con.Open();

            // Dùng lệnh UPDATE để sửa dữ liệu
            string sql = "UPDATE Tacgia SET Hovaten=@ht, Ngaysinh=@ns, Diachi=@dc, Gioitinh=@gt, Dienthoai=@dt, Email=@em WHERE Matacgia=@mtg";

            SqlCommand cmd = new SqlCommand(sql, con);

            // Cách viết tham số theo đúng yêu cầu của bạn:
            cmd.Parameters.Add("@ht", SqlDbType.NVarChar, 50).Value = ht;
            cmd.Parameters.Add("@ns", SqlDbType.Date).Value = ns;
            cmd.Parameters.Add("@dc", SqlDbType.NVarChar, 200).Value = dc;
            cmd.Parameters.Add("@gt", SqlDbType.NChar, 50).Value = gt;
            cmd.Parameters.Add("@dt", SqlDbType.NChar, 50).Value = dt;
            cmd.Parameters.Add("@em", SqlDbType.NChar, 50).Value = em;
            cmd.Parameters.Add("@mtg", SqlDbType.NChar, 50).Value = mtg;

            cmd.ExecuteNonQuery();
            cmd.Dispose();
            con.Close();

            MessageBox.Show("Sửa thành công");
            load_Tacgia();
        }

        private void btnxoa_Click(object sender, EventArgs e)
        {
            //b1: lay du lieu tu dieu khien dua vao bien
            string mtg = txtmtg.Text.Trim();
            DialogResult kq = MessageBox.Show("Ban co chac chan muon xoa khong?", "Delete", MessageBoxButtons.YesNo);
            if (kq == DialogResult.No)
            {
                return;//cac lenh sau do dung luon
            }
            //b2: ket noi voi database
            if (con.State == ConnectionState.Closed)
            {
                con.Open();
                //b3: tao doi tuong command de thuc thi cau lenh truy van
                string sql = "delete from Tacgia where Matacgia='" + mtg + "'";
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.ExecuteNonQuery();
                cmd.Dispose();
                con.Close();
                MessageBox.Show("Xoa thanh cong!");
                load_Tacgia();
            }
        }

        private void btntimkiem_Click(object sender, EventArgs e)
        {
            string mtg = txtmtg_tk.Text.Trim();
            string ht = txthoten_tk.Text.Trim();
            string dt = txtdienthoai_tk.Text.Trim();
            string gt = (cbgioitinh_tk.SelectedItem != null) ? cbgioitinh_tk.SelectedItem.ToString() : "";

            if (con.State == ConnectionState.Closed) con.Open();

            // Sử dụng LIKE và cho phép tìm kiếm rỗng (nếu rỗng thì LIKE '%%' sẽ lấy tất cả)
            string sql = "SELECT * FROM Tacgia WHERE " +
                         "Matacgia LIKE @mtg AND " +
                         "Hovaten LIKE @ht AND " +
                         "Dienthoai LIKE @dt";

            // Riêng Giới tính nếu bạn muốn lọc chính xác khi có chọn
            if (!string.IsNullOrEmpty(gt))
            {
                sql += " AND Gioitinh = @gt";
            }
            if (!string.IsNullOrEmpty(dt))
            {
                // Kiểm tra nếu có bất kỳ ký tự nào không phải là số
                if (!dt.All(char.IsDigit))
                {
                    MessageBox.Show("Số điện thoại tìm kiếm chỉ được phép nhập số!", "Thông báo");
                    txtdienthoai_tk.Focus();
                    return; // Dừng hàm tìm kiếm tại đây
                }
            }
            SqlCommand cmd = new SqlCommand(sql, con);

            // Gán tham số kèm dấu % để tìm kiếm tương đối
            cmd.Parameters.AddWithValue("@mtg", "%" + mtg + "%");
            cmd.Parameters.AddWithValue("@ht", "%" + ht + "%");
            cmd.Parameters.AddWithValue("@dt", "%" + dt + "%");
            if (!string.IsNullOrEmpty(gt))
            {
                cmd.Parameters.AddWithValue("@gt", gt);
            }

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable tb = new DataTable();
            da.Fill(tb);

            dgvdanhsach.DataSource = tb;

            if (tb.Rows.Count == 0)
            {
                MessageBox.Show("Không tìm thấy kết quả nào!");
            }

            con.Close();

        }

        private void btnxuat_Click(object sender, EventArgs e)
        {
            //Tạo các đối tượng Excel
            DataTable tb = (DataTable)dgvdanhsach.DataSource;
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
            oSheet.Name = "DANH SACH TAC GIA";
            // Tạo phần đầu nếu muốn
            ex_cel.Range head = oSheet.get_Range("A1", "H1");
            head.MergeCells = true;
            head.Value2 = "DANH SÁCH TÁC GIẢ";
            head.Font.Bold = true;
            head.Font.Name = "Tahoma";
            head.Font.Size = "16";
            head.HorizontalAlignment = ex_cel.XlHAlign.xlHAlignCenter;
            // Tạo tiêu đề cột 
            ex_cel.Range cl1 = oSheet.get_Range("A3", "A3");
            cl1.Value2 = "STT";
            cl1.ColumnWidth = 7.5;

            ex_cel.Range cl2 = oSheet.get_Range("B3", "B3");
            cl2.Value2 = "MÃ TÁC GIẢ";
            cl2.ColumnWidth = 25.0;

            ex_cel.Range cl3 = oSheet.get_Range("C3", "C3");
            cl3.Value2 = "HỌ VÀ TÊN";
            cl3.ColumnWidth = 40.0;

            ex_cel.Range cl4 = oSheet.get_Range("D3", "D3");
            cl4.Value2 = "NGAY SINH";
            cl4.ColumnWidth = 15.0;

            ex_cel.Range cl5 = oSheet.get_Range("E3", "E3");
            cl5.Value2 = "DIA CHI";
            cl5.ColumnWidth = 25.0;

            ex_cel.Range cl6 = oSheet.get_Range("F3", "F3");
            cl6.Value2 = "GIOI TINH";
            cl6.ColumnWidth = 20.0;
            //ex_cel.Range cl6_1 = oSheet.get_Range("F4", "F1000");
            //cl6_1.Columns.NumberFormat = "dd/mm/yyyy";


            ex_cel.Range cl7 = oSheet.get_Range("G3", "G3");
            cl7.Value2 = "DIEN THOAI";
            cl7.ColumnWidth = 40;

            ex_cel.Range cl8 = oSheet.get_Range("H3", "H3");
            cl8.Value2 = "EMAIL";
            cl8.ColumnWidth = 45.0;

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
                arr[r, 1] = dr["Matacgia"];         // Cột B: Mã tác giả
                arr[r, 2] = dr["Hovaten"];          // Cột C: Họ và tên
                arr[r, 3] = dr["Ngaysinh"];         // Cột D: Ngày sinh
                arr[r, 4] = dr["Diachi"];           // Cột E: Địa chỉ
                arr[r, 5] = dr["Gioitinh"];         // Cột F: Giới tính
                arr[r, 6] = "'" + dr["Dienthoai"];  // Cột G: Điện thoại (Dấu ' để giữ số 0)
                arr[r, 7] = dr["Email"];            // Cột H: Email
            }

            // 2. Thiết lập vùng điền dữ liệu (Bắt đầu từ cột 1 là cột A)
            int rowStart = 4;
            int columnStart = 1;
            int rowEnd = rowStart + tb.Rows.Count - 1;
            int columnEnd = 8; // Kết thúc ở cột 8 (tương ứng cột H)

            ex_cel.Range c1 = (ex_cel.Range)oSheet.Cells[rowStart, columnStart];
            ex_cel.Range c2 = (ex_cel.Range)oSheet.Cells[rowEnd, columnEnd];
            ex_cel.Range range = oSheet.get_Range(c1, c2);

            // Đổ mảng vào Excel
            range.Value2 = arr;

            // 3. Kẻ viền cho vùng dữ liệu
            range.Borders.LineStyle = ex_cel.Constants.xlSolid;

            // 4. Sửa lỗi lệch định dạng Ngày sinh (Cột D)
            // Trong code của bạn Ngày sinh ở cột D, nên phải định dạng cột D
            ex_cel.Range cl_ngs = oSheet.get_Range("D" + rowStart, "D" + rowEnd);
            cl_ngs.NumberFormat = "dd/mm/yyyy";

            // 5. Căn giữa cột STT cho đẹp
            oSheet.get_Range("A" + rowStart, "A" + rowEnd).HorizontalAlignment = ex_cel.XlHAlign.xlHAlignCenter;

            // Tự động giãn độ rộng các cột cho vừa nội dung
            oSheet.get_Range("A3", "H" + rowEnd).Columns.AutoFit();
        }

        private void btnreset_Click(object sender, EventArgs e)
        {
            txtmtg.Clear();
            txthoten.Clear();
            txtdiachi.Clear();
            txtdienthoai.Clear();
            txtemail.Clear();

            dtngaysinh.Value = DateTime.Now;

            cbgioitinh.SelectedIndex = -1;
            
            txtmtg.Enabled = true;
            txtmtg.Focus();
        }

        private void btnthoat_Click(object sender, EventArgs e)
        {
            // Hiển thị hộp thoại xác nhận với tiêu đề và biểu tượng dấu hỏi
            DialogResult kq = MessageBox.Show("Ban co chac chan muon thoat khong?", "Thong bao", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            // Nếu người dùng chọn Yes thì mới thực hiện lệnh đóng Form
            if (kq == DialogResult.Yes)
            {
                this.Close();
            }
        }

        private void cbgioitinh_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
