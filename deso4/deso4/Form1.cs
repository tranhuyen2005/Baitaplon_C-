using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ex_cel = Microsoft.Office.Interop.Excel;
namespace deso4
{
    public partial class Form1 : Form
    {
        SqlConnection con=new SqlConnection("Data Source=NGUYENTRUNGKIEN\\SQLEXPRESS;Initial Catalog=deso4;Integrated Security=True");
        public Form1()
        {
            InitializeComponent();
        }
        private void load_sinhvien()
        {
            if (con.State == ConnectionState.Closed)
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("select * from Sinhvien", con);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable tb = new DataTable();
                da.Fill(tb);
                cmd.Dispose();
                con.Close();
                dgvsinhvien.DataSource = tb;
                dgvsinhvien.Refresh();
            }
        }
        private bool checktrungMTG(string msv)
        {
            if (con.State == ConnectionState.Closed)
            {
                con.Open();
            }

            string sql = "select count(*) from Sinhvien where Masv=@msv";
            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.Add("@msv", SqlDbType.NChar, 50).Value = msv;
            int kq = (int)cmd.ExecuteScalar();
            con.Close();

            if (kq > 0)
                return true;
            else
                return false;
        }
        private void btnluu_Click(object sender, EventArgs e)
        {
            string msv = txtmasinhvien.Text.Trim();
            string ten = txttensinvien.Text.Trim();
            string gt = cbgioitinh.SelectedItem.ToString();
            DateTime ns = dtngaysinh.Value;
            string ml = txtmalop.Text.Trim();
            string dc = txtdiachi.Text.Trim();
            if (msv == "")
            {
                txtmasinhvien.Focus();
                MessageBox.Show("Ma sinh vien khong duoc trong!");
                return;
            }
            if (ml == "")
            {
                txtmalop.Focus();
                MessageBox.Show("Ma lop khong duoc trong!");
                return;
            }
            if (ten == "")
            {
                txttensinvien.Focus();
                MessageBox.Show("Ten sinh vien khong duoc trong!");
                return;
            }
            //kiem tra trung ma tac gia
            if (checktrungMTG(msv))
            {
                txtmasinhvien.Focus();
                MessageBox.Show("Trung ma sinh vien");
                return;
            }
            //kiem tra ngay sinh
            if (ns >= DateTime.Now)
            {
                dtngaysinh.Focus();
                MessageBox.Show("Ngay sinh phai nho hon ngay hien tai!");
                return;
            }
            if (con.State == ConnectionState.Closed)
            {
                con.Open();
                string sql = "Insert Sinhvien Values(@msv,@ten,@gt,@ns,@ml,@dc)";
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.Add("@msv", SqlDbType.NChar, 50).Value = msv;
                cmd.Parameters.Add("@ten", SqlDbType.NChar, 50).Value = ten;
                cmd.Parameters.Add("@gt", SqlDbType.NChar).Value = gt;
                cmd.Parameters.Add("@ns", SqlDbType.Date).Value = ns;
                cmd.Parameters.Add("@ml", SqlDbType.NChar, 50).Value = ml;
                cmd.Parameters.Add("@dc", SqlDbType.NChar, 50).Value = dc;
                cmd.ExecuteNonQuery();
                con.Close();
                MessageBox.Show("Them moi thanh cong!");
                load_sinhvien();

            }
        }

        private void btncapnhat_Click(object sender, EventArgs e)
        {
            string msv = txtmasinhvien.Text.Trim();
            string ten = txttensinvien.Text.Trim();
            string gt = cbgioitinh.SelectedItem.ToString();
            DateTime ns = dtngaysinh.Value;
            string ml = txtmalop.Text.Trim();
            string dc = txtdiachi.Text.Trim();
            if (con.State == ConnectionState.Closed) con.Open();

            // Dùng lệnh UPDATE để sửa dữ liệu
            string sql = "UPDATE Sinhvien SET Hoten=@ten,Gioitinh=@gt, Ngaysinh=@ns,Malop=@ml , Diachi=@dc WHERE Masv=@msv";

            SqlCommand cmd = new SqlCommand(sql, con);

            // Cách viết tham số theo đúng yêu cầu của bạn:
            cmd.Parameters.Add("@msv", SqlDbType.NChar, 50).Value = msv;
            cmd.Parameters.Add("@ten", SqlDbType.NChar, 50).Value = ten;
            cmd.Parameters.Add("@gt", SqlDbType.NChar).Value = gt;
            cmd.Parameters.Add("@ns", SqlDbType.Date).Value = ns;
            cmd.Parameters.Add("@ml", SqlDbType.NChar, 50).Value = ml;
            cmd.Parameters.Add("@dc", SqlDbType.NChar, 50).Value = dc;

            cmd.ExecuteNonQuery();
            cmd.Dispose();
            con.Close();

            MessageBox.Show("Sửa thành công");
            load_sinhvien();
        }

        

        private void Form1_Load(object sender, EventArgs e)
        {
            load_sinhvien();
        }

        private void dgvsinhvien_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int i = e.RowIndex;
            txtmasinhvien.Text = dgvsinhvien.Rows[i].Cells["Masv"].Value.ToString();
            txttensinvien.Text = dgvsinhvien.Rows[i].Cells["Hoten"].Value.ToString();
            cbgioitinh.SelectedItem = dgvsinhvien.Rows[i].Cells["Gioitinh"].Value.ToString().Trim();
            dtngaysinh.Value = DateTime.Parse(dgvsinhvien.Rows[i].Cells["Ngaysinh"].Value.ToString());
            txtmalop.Text = dgvsinhvien.Rows[i].Cells["Malop"].Value.ToString();
            txtdiachi.Text = dgvsinhvien.Rows[i].Cells["Diachi"].Value.ToString();
            txtmasinhvien.Enabled = false;
        }

        private void btnxoa_Click(object sender, EventArgs e)
        {
            string msv = txtmasinhvien.Text.Trim();
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
                string sql = "delete from Sinhvien where Masv='" + msv + "'";
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.ExecuteNonQuery();
                cmd.Dispose();
                con.Close();
                MessageBox.Show("Xoa thanh cong!");
                load_sinhvien();
            }

        }

        private void btntimkiem_Click(object sender, EventArgs e)
        {
            try
            {
                if (con.State == ConnectionState.Closed) con.Open();

                // Chuỗi SQL kết hợp tất cả các trường
                // Dùng LIKE cho text, và so sánh trực tiếp cho Gioitinh, Ngaysinh
                string sql = "SELECT * FROM Sinhvien WHERE " +
                             "Masv LIKE @msv AND " +
                             "Hoten LIKE @ten AND " +
                             "Malop LIKE @ml AND " +
                             "Diachi LIKE @dc AND " +
                             "Gioitinh = @gt AND " +
                             "CAST(Ngaysinh AS DATE) = @ns"; // So sánh chỉ phần Ngày/Tháng/Năm

                SqlCommand cmd = new SqlCommand(sql, con);

                // 1. Tham số chuỗi (Text)
                cmd.Parameters.AddWithValue("@msv", "%" + txtmasinhvien.Text.Trim() + "%");
                cmd.Parameters.AddWithValue("@ten", "%" + txttensinvien.Text.Trim() + "%");
                cmd.Parameters.AddWithValue("@ml", "%" + txtmalop.Text.Trim() + "%");
                cmd.Parameters.AddWithValue("@dc", "%" + txtdiachi.Text.Trim() + "%");

                // 2. Tham số Giới tính (Lấy từ ComboBox)
                string gioiTinh = cbgioitinh.SelectedItem != null ? cbgioitinh.SelectedItem.ToString() : "";
                cmd.Parameters.AddWithValue("@gt", gioiTinh);

                // 3. Tham số Ngày sinh
                // Lấy giá trị ngày từ DateTimePicker
                cmd.Parameters.Add("@ns", SqlDbType.Date).Value = dtngaysinh.Value.Date;

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    dgvsinhvien.DataSource = dt;
                }
                else
                {
                    MessageBox.Show("Không tìm thấy kết quả nào khớp!");
                    load_sinhvien();
                }

                con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tìm kiếm: " + ex.Message);
                if (con.State == ConnectionState.Open) con.Close();
            }       
        }

        private void btnxuat_Click(object sender, EventArgs e)
        {
            // 1. Lấy dữ liệu từ DataGridView (dgvdocgia)
            DataTable tb = (DataTable)dgvsinhvien.DataSource;
            if (tb == null || tb.Rows.Count == 0) return;

            // 2. Khởi tạo các đối tượng Excel
            ex_cel.Application oExcel = new ex_cel.Application();
            ex_cel.Workbooks oBooks;
            ex_cel.Sheets oSheets;
            ex_cel.Workbook oBook;
            ex_cel.Worksheet oSheet;

            oExcel.Visible = true;
            oExcel.DisplayAlerts = false;
            oExcel.Application.SheetsInNewWorkbook = 1;
            oBooks = oExcel.Workbooks;
            oBook = (ex_cel.Workbook)(oExcel.Workbooks.Add(Type.Missing));
            oSheets = oBook.Worksheets;
            oSheet = (ex_cel.Worksheet)oSheets.get_Item(1);
            oSheet.Name = "DANH SACH SINH VIEN"; // Đổi tên sheet cho đúng bài

            // 3. Tạo phần tiêu đề đầu trang
            ex_cel.Range head = oSheet.get_Range("A1", "H1"); // Cột A đến H (8 cột)
            head.MergeCells = true;
            head.Value2 = "DANH SÁCH SINH VIEN";
            head.Font.Bold = true;
            head.Font.Name = "Tahoma";
            head.Font.Size = "16";
            head.HorizontalAlignment = ex_cel.XlHAlign.xlHAlignCenter;

            // 4. Tạo tiêu đề cột (Dòng 3)
            // Thiết lập nội dung cho từng cột từ A đến H
            oSheet.get_Range("A3", "A3").Value2 = "STT";
            oSheet.get_Range("B3", "B3").Value2 = "MÃ SINH VIEN";
            oSheet.get_Range("C3", "C3").Value2 = "HỌ VÀ TÊN";
            oSheet.get_Range("D3", "D3").Value2 = "GIỚI TÍNH";
            oSheet.get_Range("E3", "E3").Value2 = "NGÀY SINH";
            oSheet.get_Range("F3", "F3").Value2 = "MÃ LỚP";
            oSheet.get_Range("G3", "G3").Value2 = "ĐIA CHI";

            // Định dạng dòng tiêu đề (Bold, Viền, Màu nền)
            ex_cel.Range rowHead = oSheet.get_Range("A3", "G3");
            rowHead.Font.Bold = true;
            rowHead.Borders.LineStyle = ex_cel.Constants.xlSolid;
            rowHead.Interior.ColorIndex = 15;
            rowHead.HorizontalAlignment = ex_cel.XlHAlign.xlHAlignCenter;

            // 5. Đổ dữ liệu vào mảng (Mảng 8 cột khớp với SQL)
            object[,] arr = new object[tb.Rows.Count, 7];

            for (int r = 0; r < tb.Rows.Count; r++)
            {
                DataRow dr = tb.Rows[r];
                arr[r, 0] = r + 1;                        
                arr[r, 1] = dr["Masv"];                    
                arr[r, 2] = dr["Hoten"];                
                arr[r, 3] = dr["Gioitinh"];              
                arr[r, 4] = dr["Ngaysinh"];                     
                arr[r, 5] = dr["Malop"];              
                arr[r, 6] = dr["Diachi"];                      
            }

            // 6. Thiết lập vùng điền dữ liệu và kẻ viền
            int rowStart = 4;
            int columnStart = 1;
            int rowEnd = rowStart + tb.Rows.Count - 1;
            int columnEnd = 7;

            ex_cel.Range c1 = (ex_cel.Range)oSheet.Cells[rowStart, columnStart];
            ex_cel.Range c2 = (ex_cel.Range)oSheet.Cells[rowEnd, columnEnd];
            ex_cel.Range range = oSheet.get_Range(c1, c2);

            range.Value2 = arr; // Đổ mảng vào Excel
            range.Borders.LineStyle = ex_cel.Constants.xlSolid;

            // 7. Định dạng Ngày sinh (Cột E) và căn chỉnh
            ex_cel.Range cl_ngs = oSheet.get_Range("E" + rowStart, "E" + rowEnd);
            cl_ngs.NumberFormat = "dd/mm/yyyy";

            // Căn giữa cột STT và tự động giãn độ rộng
            oSheet.get_Range("A" + rowStart, "A" + rowEnd).HorizontalAlignment = ex_cel.XlHAlign.xlHAlignCenter;
            oSheet.get_Range("A3", "G" + rowEnd).Columns.AutoFit();
        }
    }
    }

