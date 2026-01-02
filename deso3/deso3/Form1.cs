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
namespace deso3
{
    public partial class Form1 : Form
    {
        SqlConnection con = new SqlConnection("Data Source=NGUYENTRUNGKIEN\\SQLEXPRESS;Initial Catalog=de3;Integrated Security=True");
        public Form1()
        {
            InitializeComponent();
        }
        private void load_docgia()
        {
            if (con.State == ConnectionState.Closed)
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("select * from Docgia", con);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable tb = new DataTable();
                da.Fill(tb);
                cmd.Dispose();
                con.Close();
                dgvdocgia.DataSource = tb;
                dgvdocgia.Refresh();
            }
        }
        private bool checktrungMTG(string mdg)
        {
            if (con.State == ConnectionState.Closed)
            {
                con.Open();
            }

            string sql = "select count(*) from Docgia where Madocgia=@mdg";
            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.Add("@mdg", SqlDbType.NChar, 50).Value = mdg;
            int kq = (int)cmd.ExecuteScalar();
            con.Close();

            if (kq > 0)
                return true;
            else
                return false;
        }
        private void btnluu_Click(object sender, EventArgs e)
        {
            string mdg = txtmadocgia.Text.Trim();
            string ten = txttendocgia.Text.Trim();
            string gt = cbgioitinh.SelectedItem.ToString();
            DateTime ns = dtngaysinh.Value;
            string ml = txtmalop.Text.Trim();
            string dt = txtdienthoai.Text.Trim();
            string dc = txtdiachi.Text.Trim();
            if (mdg == "")
            {
                txtmadocgia.Focus();
                MessageBox.Show("Ma doc gia khong duoc trong!");
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
                txttendocgia.Focus();
                MessageBox.Show("Ten doc gia khong duoc trong!");
                return;
            }
            //kiem tra trung ma tac gia
            if (checktrungMTG(mdg))
            {
                txtmadocgia.Focus();
                MessageBox.Show("Trung ma doc gia");
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
            if (con.State == ConnectionState.Closed)
            {
                con.Open();
                string sql = "Insert Docgia Values(@mdg,@ten,@gt,@ns,@ml,@dt,@dc)";
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.Add("@mdg", SqlDbType.NChar, 50).Value = mdg;
                cmd.Parameters.Add("@ten", SqlDbType.NChar, 50).Value = ten;
                cmd.Parameters.Add("@gt", SqlDbType.NChar).Value = gt;
                cmd.Parameters.Add("@ns", SqlDbType.Date).Value = ns;
                cmd.Parameters.Add("@ml", SqlDbType.NChar, 50).Value = ml;
                cmd.Parameters.Add("@dt", SqlDbType.NChar, 50).Value = dt;
                cmd.Parameters.Add("@dc", SqlDbType.NChar, 50).Value = dc;
                cmd.ExecuteNonQuery();
                con.Close();
                MessageBox.Show("Them moi thanh cong!");
                load_docgia();

            }
        }

        private void dgvdocgia_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int i = e.RowIndex;
            txtmadocgia.Text = dgvdocgia.Rows[i].Cells["Madocgia"].Value.ToString();
            txttendocgia.Text = dgvdocgia.Rows[i].Cells["Tendocgia"].Value.ToString();
            cbgioitinh.SelectedItem = dgvdocgia.Rows[i].Cells["Gioitinh"].Value.ToString().Trim();
            dtngaysinh.Value = DateTime.Parse(dgvdocgia.Rows[i].Cells["Ngaysinh"].Value.ToString());
            txtmalop.Text = dgvdocgia.Rows[i].Cells["Malop"].Value.ToString();
            txtdienthoai.Text = dgvdocgia.Rows[i].Cells["Dienthoai"].Value.ToString();
            txtdiachi.Text = dgvdocgia.Rows[i].Cells["Diachi"].Value.ToString();
            txtmadocgia.Enabled = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            load_docgia();
        }

        private void btncapnhat_Click(object sender, EventArgs e)
        {
            string mdg = txtmadocgia.Text.Trim();
            string ten = txttendocgia.Text.Trim();
            string gt = cbgioitinh.SelectedItem.ToString();
            DateTime ns = dtngaysinh.Value;
            string ml = txtmalop.Text.Trim();
            string dt = txtdienthoai.Text.Trim();
            string dc = txtdiachi.Text.Trim();

            if (dt.Length != 10 || !dt.All(char.IsDigit))
            {
                MessageBox.Show("So dien thoai phai la so va co 10 chu so!");
                txtdienthoai.Focus();
                return;
            }
            if (ns >= DateTime.Now)
            {
                dtngaysinh.Focus();
                MessageBox.Show("Ngay sinh phai nho hon ngay hien tai!");
                return;
            }
            if (con.State == ConnectionState.Closed) con.Open();

               
                string sql = "UPDATE Docgia SET Tendocgia=@ten,Gioitinh=@gt, Ngaysinh=@ns,Malop=@ml , Dienthoai=@dt, Diachi=@dc WHERE Madocgia=@mdg";

                SqlCommand cmd = new SqlCommand(sql, con);

                
                cmd.Parameters.Add("@mdg", SqlDbType.NChar, 50).Value = mdg;
                cmd.Parameters.Add("@ten", SqlDbType.NChar, 50).Value = ten;
                cmd.Parameters.Add("@gt", SqlDbType.NChar).Value = gt;
                cmd.Parameters.Add("@ns", SqlDbType.Date).Value = ns;
                cmd.Parameters.Add("@ml", SqlDbType.NChar, 50).Value = ml;
                cmd.Parameters.Add("@dt", SqlDbType.NChar, 50).Value = dt;
                cmd.Parameters.Add("@dc", SqlDbType.NChar, 50).Value = dc;

                cmd.ExecuteNonQuery();
                cmd.Dispose();
                con.Close();

                MessageBox.Show("Sửa thành công");
                load_docgia();
        }

        private void btnxoa_Click(object sender, EventArgs e)
        {
            string mdg = txtmadocgia.Text.Trim();
            DialogResult kq = MessageBox.Show("Ban co chac chan muon xoa khong?", "Delete", MessageBoxButtons.YesNo);
            if (kq == DialogResult.No)
            {
                return;
            }
           
            if (con.State == ConnectionState.Closed)
            {
                con.Open();
                
                string sql = "delete from Docgia where Madocgia='" + mdg + "'";
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.ExecuteNonQuery();
                cmd.Dispose();
                con.Close();
                MessageBox.Show("Xoa thanh cong!");
                load_docgia();
            }
        }

        private void btntimkiem_Click(object sender, EventArgs e)
        {
            string sdt = txtdienthoai.Text.Trim();
            if (!string.IsNullOrEmpty(sdt))
                if (!sdt.All(char.IsDigit))
                {
                    MessageBox.Show("So dien thoai phai la so!");
                    txtdienthoai.Focus();
                    return;
                }
            //kiem tra ngay sinh
            if (dtngaysinh.Value.Date >= DateTime.Now.Date)
            {
                MessageBox.Show("Ngay sinh phai nho hon ngay hien tai!", "Thong bao", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                dtngaysinh.Focus();
                return;
            }
            try
            {
                if (con.State == ConnectionState.Closed) con.Open();

                string sql = "SELECT * FROM Docgia WHERE " +
                             "Madocgia LIKE @mdg AND " +
                             "Tendocgia LIKE @ten AND " +
                             "Malop LIKE @ml AND " +
                             "Dienthoai LIKE @dt AND " +
                             "Diachi LIKE @dc AND " +
                             "Gioitinh = @gt AND " +
                             "CAST(Ngaysinh AS DATE) = @ns"; 

                SqlCommand cmd = new SqlCommand(sql, con);

                cmd.Parameters.AddWithValue("@mdg", "%" + txtmadocgia.Text.Trim() + "%");
                cmd.Parameters.AddWithValue("@ten", "%" + txttendocgia.Text.Trim() + "%");
                cmd.Parameters.AddWithValue("@ml", "%" + txtmalop.Text.Trim() + "%");
                cmd.Parameters.AddWithValue("@dt", "%" + txtdienthoai.Text.Trim() + "%");
                cmd.Parameters.AddWithValue("@dc", "%" + txtdiachi.Text.Trim() + "%");

                string gioiTinh = cbgioitinh.SelectedItem != null ? cbgioitinh.SelectedItem.ToString() : "";
                cmd.Parameters.AddWithValue("@gt", gioiTinh);

                cmd.Parameters.Add("@ns", SqlDbType.Date).Value = dtngaysinh.Value.Date;

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    dgvdocgia.DataSource = dt;
                }
                else
                {
                    MessageBox.Show("Không tìm thấy kết quả nào khớp!");
                    load_docgia();
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
            txtmadocgia.Clear();
            txttendocgia.Clear();
            txtdiachi.Clear();
            txtdienthoai.Clear();
            txtmalop.Clear();

            dtngaysinh.Value = DateTime.Now;

            cbgioitinh.SelectedIndex = -1;

            txtmadocgia.Enabled = true;
            txtmadocgia.Focus();
        }

        private void btnxuat_Click(object sender, EventArgs e)
        {
            
            DataTable tb = (DataTable)dgvdocgia.DataSource;
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
            oSheet.Name = "DANH SACH DOC GIA"; 

            
            ex_cel.Range head = oSheet.get_Range("A1", "H1");
            head.MergeCells = true;
            head.Value2 = "DANH SÁCH ĐỘC GIẢ";
            head.Font.Bold = true;
            head.Font.Name = "Tahoma";
            head.Font.Size = "16";
            head.HorizontalAlignment = ex_cel.XlHAlign.xlHAlignCenter;

            // 4. Tạo tiêu đề cột (Dòng 3)
            // Thiết lập nội dung cho từng cột từ A đến H
            oSheet.get_Range("A3", "A3").Value2 = "STT";
            oSheet.get_Range("B3", "B3").Value2 = "MÃ ĐỘC GIẢ";
            oSheet.get_Range("C3", "C3").Value2 = "HỌ VÀ TÊN";
            oSheet.get_Range("D3", "D3").Value2 = "GIỚI TÍNH";
            oSheet.get_Range("E3", "E3").Value2 = "NGÀY SINH";
            oSheet.get_Range("F3", "F3").Value2 = "MÃ LỚP";
            oSheet.get_Range("G3", "G3").Value2 = "ĐIỆN THOẠI";
            oSheet.get_Range("H3", "H3").Value2 = "ĐỊA CHỈ";

            // Định dạng dòng tiêu đề (Bold, Viền, Màu nền)
            ex_cel.Range rowHead = oSheet.get_Range("A3", "H3");
            rowHead.Font.Bold = true;
            rowHead.Borders.LineStyle = ex_cel.Constants.xlSolid;
            rowHead.Interior.ColorIndex = 15;
            rowHead.HorizontalAlignment = ex_cel.XlHAlign.xlHAlignCenter;

            object[,] arr = new object[tb.Rows.Count, 8];

            for (int r = 0; r < tb.Rows.Count; r++)
            {
                DataRow dr = tb.Rows[r];
                arr[r, 0] = r + 1;                             
                arr[r, 1] = dr["Madocgia"];                    
                arr[r, 2] = dr["Tendocgia"];                  
                arr[r, 3] = dr["Gioitinh"];                     
                arr[r, 4] = dr["Ngaysinh"];                      
                arr[r, 5] = dr["Malop"];                       
                arr[r, 6] = "'" + dr["Dienthoai"];             
                arr[r, 7] = dr["Diachi"];                    
            }

            // 6. Thiết lập vùng điền dữ liệu và kẻ viền
            int rowStart = 4;
            int columnStart = 1;
            int rowEnd = rowStart + tb.Rows.Count - 1;
            int columnEnd = 8;

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
            oSheet.get_Range("A3", "H" + rowEnd).Columns.AutoFit();
        }
    }
}
