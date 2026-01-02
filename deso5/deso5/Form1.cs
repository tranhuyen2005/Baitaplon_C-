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
namespace deso5
{
    public partial class Form1 : Form
    {
        SqlConnection con = new SqlConnection("Data Source=NGUYENTRUNGKIEN\\SQLEXPRESS;Initial Catalog=deso5;Integrated Security=True");
        public Form1()
        {
            InitializeComponent();
        }
        private void load_nhanvien()
        {
            if (con.State == ConnectionState.Closed)
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("select * from Nhanvien", con);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable tb = new DataTable();
                da.Fill(tb);
                cmd.Dispose();
                con.Close();
                dgvnhanvien.DataSource = tb;
                dgvnhanvien.Refresh();
            }
        }
        private bool checktrungMTG(string mnv)
        {
            if (con.State == ConnectionState.Closed)
            {
                con.Open();
            }

            string sql = "select count(*) from Nhanvien where MANV=@mnv";
            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.Add("@mnv", SqlDbType.NChar, 50).Value = mnv;
            int kq = (int)cmd.ExecuteScalar();
            con.Close();

            if (kq > 0)
                return true;
            else
                return false;
        }
        private void btnluu_Click(object sender, EventArgs e)
        {
            string mnv = txtmanhanvien.Text.Trim();
            string ten = txttennhanvien.Text.Trim();
            DateTime ns = dtngaysinh.Value;
            string dc = txtdiachi.Text.Trim();
            string gt = cbgioitinh.SelectedItem.ToString();
            
            string luong = txtluong.Text.Trim();
            string map = txtmaphong.Text.Trim();
            
            if (mnv == "")
            {
                txtmanhanvien.Focus();
                MessageBox.Show("Ma nhan vien khong duoc trong!");
                return;
            }
            if (map == "")
            {
                txtmaphong.Focus();
                MessageBox.Show("Ma phong khong duoc trong!");
                return;
            }
            if (ten == "")
            {
                txttennhanvien.Focus();
                MessageBox.Show("Ten nhan vien khong duoc trong!");
                return;
            }
            //kiem tra trung ma tac gia
            if (checktrungMTG(mnv))
            {
                txtmanhanvien.Focus();
                MessageBox.Show("Trung ma nhan vien");
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
                string sql = "Insert Nhanvien Values(@mnv,@ten,@ns,@dc,@gt,@luong,@map)";
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.Add("@mnv", SqlDbType.NChar, 50).Value = mnv;
                cmd.Parameters.Add("@ten", SqlDbType.NChar, 50).Value = ten;
                cmd.Parameters.Add("@ns", SqlDbType.Date).Value = ns;
                cmd.Parameters.Add("@dc", SqlDbType.NChar).Value = dc;
                cmd.Parameters.Add("@gt", SqlDbType.NChar, 50).Value = gt;
                cmd.Parameters.Add("@luong", SqlDbType.Int, 50).Value = luong;
                cmd.Parameters.Add("@map", SqlDbType.NChar, 50).Value = map;
                cmd.ExecuteNonQuery();
                con.Close();
                MessageBox.Show("Them moi thanh cong!");
                load_nhanvien();

            }
        }

        private void dgvnhanvien_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int i = e.RowIndex;
            txtmanhanvien.Text = dgvnhanvien.Rows[i].Cells["MANV"].Value.ToString();
            txttennhanvien.Text = dgvnhanvien.Rows[i].Cells["TENNV"].Value.ToString();
            txtdiachi.Text = dgvnhanvien.Rows[i].Cells["DCHI"].Value.ToString();
            cbgioitinh.SelectedItem = dgvnhanvien.Rows[i].Cells["GT"].Value.ToString().Trim();
            txtluong.Text = dgvnhanvien.Rows[i].Cells["LUONG"].Value.ToString();
            txtmaphong.Text = dgvnhanvien.Rows[i].Cells["MAP"].Value.ToString();
            
            
            txtmanhanvien.Enabled = false;
        }

        private void btncapnhat_Click(object sender, EventArgs e)
        {
            string mnv = txtmanhanvien.Text.Trim();
            string ten = txttennhanvien.Text.Trim();
            DateTime ns = dtngaysinh.Value;
            string dc = txtdiachi.Text.Trim();
            string gt = cbgioitinh.SelectedItem.ToString();

            string luong = txtluong.Text.Trim();
            string map = txtmaphong.Text.Trim();
            if (con.State == ConnectionState.Closed) con.Open();

            // Dùng lệnh UPDATE để sửa dữ liệu
            string sql = "UPDATE Nhanvien SET TENNV=@ten,NS=@ns, DCHI=@dc,GT=@gt , LUONG=@luong, MAP=@map WHERE MANV=@mnv";

            SqlCommand cmd = new SqlCommand(sql, con);

            // Cách viết tham số theo đúng yêu cầu của bạn:
            cmd.Parameters.Add("@mnv", SqlDbType.NChar, 50).Value = mnv;
            cmd.Parameters.Add("@ten", SqlDbType.NChar, 50).Value = ten;
            cmd.Parameters.Add("@ns", SqlDbType.Date).Value = ns;
            cmd.Parameters.Add("@dc", SqlDbType.NChar).Value = dc;
            cmd.Parameters.Add("@gt", SqlDbType.NChar, 50).Value = gt;
            cmd.Parameters.Add("@luong", SqlDbType.Int, 50).Value = luong;
            cmd.Parameters.Add("@map", SqlDbType.NChar, 50).Value = map;

            cmd.ExecuteNonQuery();
            cmd.Dispose();
            con.Close();

            MessageBox.Show("Sửa thành công");
            load_nhanvien();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            load_nhanvien();
        }

        private void btnxoa_Click(object sender, EventArgs e)
        {
            string mnv = txtmanhanvien.Text.Trim();
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
                string sql = "delete from Nhanvien where MANV='" + mnv + "'";
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.ExecuteNonQuery();
                cmd.Dispose();
                con.Close();
                MessageBox.Show("Xoa thanh cong!");
                load_nhanvien();
            }
        }

        private void btntimkiem_Click(object sender, EventArgs e)
        {
            try
            {
                if (con.State == ConnectionState.Closed) con.Open();

                // Chuỗi SQL kết hợp tất cả các trường
                // Dùng LIKE cho text, và so sánh trực tiếp cho Gioitinh, Ngaysinh
                string sql = "SELECT * FROM Nhanvien WHERE " +
                             "MANV LIKE @mnv AND " +
                             "TENNV LIKE @ten AND " +
                             "MAP LIKE @map AND " +
                             "LUONG LIKE @luong AND " +
                             "DCHI LIKE @dc AND " +
                             "GT = @gt AND " +
                             "CAST(NS AS DATE) = @ns"; // So sánh chỉ phần Ngày/Tháng/Năm

                SqlCommand cmd = new SqlCommand(sql, con);

                // 1. Tham số chuỗi (Text)
                cmd.Parameters.AddWithValue("@mnv", "%" + txtmanhanvien.Text.Trim() + "%");
                cmd.Parameters.AddWithValue("@ten", "%" + txttennhanvien.Text.Trim() + "%");
                cmd.Parameters.AddWithValue("@map", "%" + txtmaphong.Text.Trim() + "%");
                cmd.Parameters.AddWithValue("@luong", "%" + txtluong.Text.Trim() + "%");
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
                    dgvnhanvien.DataSource = dt;
                }
                else
                {
                    MessageBox.Show("Không tìm thấy kết quả nào khớp!");
                    load_nhanvien();
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
            txtmanhanvien.Clear();
            txttennhanvien.Clear();
            txtdiachi.Clear();
            txtluong.Clear();
            txtmaphong.Clear();

            dtngaysinh.Value = DateTime.Now;

            cbgioitinh.SelectedIndex = -1;

            txtmanhanvien.Enabled = true;
            txtmanhanvien.Focus();
        }

        private void btnxuat_Click(object sender, EventArgs e)
        {
            // 1. Lấy dữ liệu từ DataGridView (dgvdocgia)
            DataTable tb = (DataTable)dgvnhanvien.DataSource;
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
            oSheet.Name = "DANH SACH NHAN VIEN"; // Đổi tên sheet cho đúng bài

            // 3. Tạo phần tiêu đề đầu trang
            ex_cel.Range head = oSheet.get_Range("A1", "H1"); // Cột A đến H (8 cột)
            head.MergeCells = true;
            head.Value2 = "DANH SÁCH NHAN VIEN";
            head.Font.Bold = true;
            head.Font.Name = "Tahoma";
            head.Font.Size = "16";
            head.HorizontalAlignment = ex_cel.XlHAlign.xlHAlignCenter;

            // 4. Tạo tiêu đề cột (Dòng 3)
            // Thiết lập nội dung cho từng cột từ A đến H
            oSheet.get_Range("A3", "A3").Value2 = "STT";
            oSheet.get_Range("B3", "B3").Value2 = "MÃ NHAN VIEN";
            oSheet.get_Range("C3", "C3").Value2 = "HỌ VÀ TÊN";
            oSheet.get_Range("D3", "D3").Value2 = "NGAY SINH";
            oSheet.get_Range("E3", "E3").Value2 = "DIA CHI";
            oSheet.get_Range("F3", "F3").Value2 = "GIOI TINH";
            oSheet.get_Range("G3", "G3").Value2 = "LUONG";
            oSheet.get_Range("H3", "H3").Value2 = "MA PHONG";

            // Định dạng dòng tiêu đề (Bold, Viền, Màu nền)
            ex_cel.Range rowHead = oSheet.get_Range("A3", "H3");
            rowHead.Font.Bold = true;
            rowHead.Borders.LineStyle = ex_cel.Constants.xlSolid;
            rowHead.Interior.ColorIndex = 15;
            rowHead.HorizontalAlignment = ex_cel.XlHAlign.xlHAlignCenter;

            // 5. Đổ dữ liệu vào mảng (Mảng 8 cột khớp với SQL)
            object[,] arr = new object[tb.Rows.Count, 8];

            for (int r = 0; r < tb.Rows.Count; r++)
            {
                DataRow dr = tb.Rows[r];
                arr[r, 0] = r + 1;                               // Cột A: STT
                arr[r, 1] = dr["MANV"];                      // Cột B: Mã độc giả
                arr[r, 2] = dr["TENNV"];                     // Cột C: Tên
                arr[r, 3] = dr["NS"];                      // Cột D: Giới tính
                arr[r, 4] = dr["DCHI"];                      // Cột E: Ngày sinh
                arr[r, 5] = dr["GT"];                         // Cột F: Mã lớp
                arr[r, 6] = dr["LUONG"];               // Cột G: SĐT (Thêm dấu ' để giữ số 0 đầu)
                arr[r, 7] = dr["MAP"];                        // Cột H: Địa chỉ
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

