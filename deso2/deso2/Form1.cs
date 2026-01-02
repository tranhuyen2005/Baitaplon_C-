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
namespace deso2
{
    public partial class Form1 : Form
    {
        SqlConnection con = new SqlConnection("Data Source=NGUYENTRUNGKIEN\\SQLEXPRESS;Initial Catalog=de2;Integrated Security=True");
        public Form1()
        {
            InitializeComponent();
        }
        private void load_sanpham()
        {
            if (con.State == ConnectionState.Closed)
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("select * from Sanpham", con);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable tb = new DataTable();
                da.Fill(tb);
                cmd.Dispose();
                con.Close();
                dgvsanpham.DataSource = tb;
                dgvsanpham.Refresh();
            }
        }
        private bool checktrungMTG(string msp)
        {
            if (con.State == ConnectionState.Closed)
            {
                con.Open();
            }

            string sql = "select count(*) from Sanpham where masp=@msp";
            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.Add("@msp", SqlDbType.NChar, 50).Value = msp;
            int kq = (int)cmd.ExecuteScalar();
            con.Close();

            if (kq > 0)
                return true;
            else
                return false;
        }
        private void btnluu_Click(object sender, EventArgs e)
        {
            string msp = txtmasp.Text.Trim();
            string ten = txttensp.Text.Trim();
            string gia = txtgia.Text.Trim();
            string sl = txtsoluong.Text.Trim();
            string ncc = txtmancc.Text.Trim();
            if (msp == "")
            {
                txtmasp.Focus();
                MessageBox.Show("Ma doc gia khong duoc trong!");
                return;
            }
            if (ncc == "")
            {
                txtmancc.Focus();
                MessageBox.Show("Ma lop khong duoc trong!");
                return;
            }
            if (ten == "")
            {
                txttensp.Focus();
                MessageBox.Show("Ten doc gia khong duoc trong!");
                return;
            }
            //kiem tra trung ma tac gia
            if (checktrungMTG(msp))
            {
                txtmasp.Focus();
                MessageBox.Show("Trung ma doc gia");
                return;
            }
            if (con.State == ConnectionState.Closed)
            {
                con.Open();
                string sql = "Insert Sanpham Values(@msp,@ten,@gia,@sl,@ncc)";
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.Add("@msp", SqlDbType.NChar, 50).Value = msp;
                cmd.Parameters.Add("@ten", SqlDbType.NChar, 50).Value = ten;
                cmd.Parameters.Add("@gia", SqlDbType.Int).Value = gia;
                cmd.Parameters.Add("@sl", SqlDbType.Int).Value = sl;
                cmd.Parameters.Add("@ncc", SqlDbType.NChar, 50).Value = ncc;
                cmd.ExecuteNonQuery();
                con.Close();
                MessageBox.Show("Them moi thanh cong!");
                load_sanpham();

            }
        }

        private void dgvsanpham_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int i = e.RowIndex;
            txtmasp.Text = dgvsanpham.Rows[i].Cells["masp"].Value.ToString();
            txttensp.Text = dgvsanpham.Rows[i].Cells["tensp"].Value.ToString();
            txtgia.Text = dgvsanpham.Rows[i].Cells["gia"].Value.ToString();
            txtsoluong.Text = dgvsanpham.Rows[i].Cells["soluong"].Value.ToString();
            txtmancc.Text = dgvsanpham.Rows[i].Cells["mancc"].Value.ToString();
            txtmasp.Enabled = false;
        }

        private void btncapnhat_Click(object sender, EventArgs e)
        {
            string msp = txtmasp.Text.Trim();
            string ten = txttensp.Text.Trim();
            string gia = txtgia.Text.Trim();
            string sl = txtsoluong.Text.Trim();
            string ncc = txtmancc.Text.Trim();
            if (con.State == ConnectionState.Closed) con.Open();

            // Dùng lệnh UPDATE để sửa dữ liệu
            string sql = "UPDATE Sanpham SET tensp=@ten,gia=@gia, soluong=@sl,mancc=@ncc WHERE masp=@msp";

            SqlCommand cmd = new SqlCommand(sql, con);

            // Cách viết tham số theo đúng yêu cầu của bạn:
            cmd.Parameters.Add("@msp", SqlDbType.NChar, 50).Value = msp;
            cmd.Parameters.Add("@ten", SqlDbType.NChar, 50).Value = ten;
            cmd.Parameters.Add("@gia", SqlDbType.Int).Value = gia;
            cmd.Parameters.Add("@sl", SqlDbType.Int).Value = sl;
            cmd.Parameters.Add("@ncc", SqlDbType.NChar, 50).Value = ncc;

            cmd.ExecuteNonQuery();
            cmd.Dispose();
            con.Close();

            MessageBox.Show("Sửa thành công");
            load_sanpham();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            load_sanpham();
        }

        private void btnxoa_Click(object sender, EventArgs e)
        {
            string msp = txtmasp.Text.Trim();
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
                string sql = "delete from Sanpham where masp='" + msp + "'";
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.ExecuteNonQuery();
                cmd.Dispose();
                con.Close();
                MessageBox.Show("Xoa thanh cong!");
                load_sanpham();
            }
        }

        private void btntimkiem_Click(object sender, EventArgs e)
        {
            try
            {
                if (con.State == ConnectionState.Closed) con.Open();

                // Chuỗi SQL kết hợp tất cả các trường
                // Dùng LIKE cho text, và so sánh trực tiếp cho Gioitinh, Ngaysinh
                string sql = "SELECT * FROM Sanpham WHERE " +
                             "masp LIKE @msp AND " +
                             "tensp LIKE @ten AND " +
                             "gia LIKE @gia AND " +
                             "soluong LIKE @sl AND " +
                             "mancc LIKE @ncc ";
                             

                SqlCommand cmd = new SqlCommand(sql, con);

                // 1. Tham số chuỗi (Text)
                cmd.Parameters.AddWithValue("@msp", "%" + txtmasp.Text.Trim() + "%");
                cmd.Parameters.AddWithValue("@ten", "%" + txttensp.Text.Trim() + "%");
                cmd.Parameters.AddWithValue("@gia", "%" + txtgia.Text.Trim() + "%");
                cmd.Parameters.AddWithValue("@sl", "%" + txtsoluong.Text.Trim() + "%");
                cmd.Parameters.AddWithValue("@ncc", "%" + txtmancc.Text.Trim() + "%");

                

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    dgvsanpham.DataSource = dt;
                }
                else
                {
                    MessageBox.Show("Không tìm thấy kết quả nào khớp!");
                    load_sanpham();
                }

                con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tìm kiếm: " + ex.Message);
                if (con.State == ConnectionState.Open) con.Close();
            }
        }

        private void btnreset_Click(object sender, EventArgs e)
        {
            txtmasp.Clear();
            txttensp.Clear();
            txtgia.Clear();
            txtsoluong.Clear();
            txtmancc.Clear();


            txtmasp.Enabled = true;
            txtmasp.Focus();
        }

        private void btnxuat_Click(object sender, EventArgs e)
        {
            // 1. Lấy dữ liệu từ DataGridView (dgvdocgia)
            DataTable tb = (DataTable)dgvsanpham.DataSource;
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
            oSheet.Name = "DANH SACH SAN PHAM"; // Đổi tên sheet cho đúng bài

            // 3. Tạo phần tiêu đề đầu trang
            ex_cel.Range head = oSheet.get_Range("A1", "F1"); // Cột A đến H (8 cột)
            head.MergeCells = true;
            head.Value2 = "DANH SÁCH SAN PHAM";
            head.Font.Bold = true;
            head.Font.Name = "Tahoma";
            head.Font.Size = "16";
            head.HorizontalAlignment = ex_cel.XlHAlign.xlHAlignCenter;

            // 4. Tạo tiêu đề cột (Dòng 3)
            // Thiết lập nội dung cho từng cột từ A đến H
            oSheet.get_Range("A3", "A3").Value2 = "STT";
            oSheet.get_Range("B3", "B3").Value2 = "MÃ SAN PHAM";
            oSheet.get_Range("C3", "C3").Value2 = "TEN SAN PHAM";
            oSheet.get_Range("D3", "D3").Value2 = "GIA";
            oSheet.get_Range("E3", "E3").Value2 = "SO LUONG";
            oSheet.get_Range("F3", "F3").Value2 = "MA NHA CUNG CAP";

            // Định dạng dòng tiêu đề (Bold, Viền, Màu nền)
            ex_cel.Range rowHead = oSheet.get_Range("A3", "F3");
            rowHead.Font.Bold = true;
            rowHead.Borders.LineStyle = ex_cel.Constants.xlSolid;
            rowHead.Interior.ColorIndex = 15;
            rowHead.HorizontalAlignment = ex_cel.XlHAlign.xlHAlignCenter;

            // 5. Đổ dữ liệu vào mảng (Mảng 8 cột khớp với SQL)
            object[,] arr = new object[tb.Rows.Count, 6];

            for (int r = 0; r < tb.Rows.Count; r++)
            {
                DataRow dr = tb.Rows[r];
                arr[r, 0] = r + 1;                               // Cột A: STT
                arr[r, 1] = dr["masp"];                      // Cột B: Mã độc giả
                arr[r, 2] = dr["tensp"];                     // Cột C: Tên
                arr[r, 3] = dr["gia"];                      // Cột D: Giới tính
                arr[r, 4] = dr["soluong"];                      // Cột E: Ngày sinh
                arr[r, 5] = dr["mancc"];                      // Cột H: Địa chỉ
            }

            // 6. Thiết lập vùng điền dữ liệu và kẻ viền
            int rowStart = 4;
            int columnStart = 1;
            int rowEnd = rowStart + tb.Rows.Count - 1;
            int columnEnd = 6;

            ex_cel.Range c1 = (ex_cel.Range)oSheet.Cells[rowStart, columnStart];
            ex_cel.Range c2 = (ex_cel.Range)oSheet.Cells[rowEnd, columnEnd];
            ex_cel.Range range = oSheet.get_Range(c1, c2);

            range.Value2 = arr; // Đổ mảng vào Excel
            range.Borders.LineStyle = ex_cel.Constants.xlSolid;

            ex_cel.Range cl_sl = oSheet.get_Range("E" +  rowStart, "E" + rowEnd);
            cl_sl.NumberFormat = "0";
            cl_sl.HorizontalAlignment = ex_cel.XlHAlign.xlHAlignCenter;

            // Căn giữa cột STT và tự động giãn độ rộng
            oSheet.get_Range("A" + rowStart, "A" + rowEnd).HorizontalAlignment = ex_cel.XlHAlign.xlHAlignCenter;
            oSheet.get_Range("A3", "F" + rowEnd).Columns.AutoFit();
        
    }
    }
}

