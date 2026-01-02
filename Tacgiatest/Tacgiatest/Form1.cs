using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tacgiatest
{
    public partial class Form1 : Form
    {
        SqlConnection con=new SqlConnection("Data Source=NGUYENTRUNGKIEN\\SQLEXPRESS;Initial Catalog=sach;Integrated Security=True");
        public Form1()
        {
            InitializeComponent();
        }
        private void load_tacgia()
        {
            if(con.State==ConnectionState.Closed)
            {
                con.Open();

            }
            SqlCommand cmd = new SqlCommand("select * from Tacgia",con);
            SqlDataAdapter da=new SqlDataAdapter(cmd);
            DataTable tb=new DataTable();
            da.Fill(tb);
            dgvtacgia.DataSource = tb;
            dgvtacgia.Refresh();
            cmd.Dispose();
            con.Close();
        }
        private bool checktrungMTG(string mtg)
        {

            if (con.State == ConnectionState.Closed)
            {
                con.Open();
            }

            string sql = "select count(*) from Tacgia where Matacgia=@mtg";
            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.Add("@mtg", SqlDbType.NChar, 50).Value = mtg;

            int kq = (int)cmd.ExecuteScalar(); // Thực thi lệnh lấy số lượng

            con.Close(); // Xong việc thì đóng kết nối

            // Đoạn if...else của bạn đặt ở đây là chuẩn nhất:
            if(kq>0)
                return true;
            else
                return false;
        }
        private void btnluu_Click(object sender, EventArgs e)
        {
            string mtg=txtmatacgia.Text.Trim();
            string ht=txthoten.Text.Trim();
            DateTime ns = dtngaysinh.Value;
            string dc=txtdiachi.Text.Trim();
            string gt=cbgioitinh.SelectedItem.ToString();
            string dt=txtdienthoai.Text.Trim();
            string email=txtemail.Text.Trim();
            //kiem tra trong mtg
            if(mtg=="") {
                txtmatacgia.Focus();
                MessageBox.Show("Ma tac gia khong duoc trong");
                return;
            }
            //kiem tra trong ho ten
            if (ht == "")
            {
                txthoten.Focus();
                MessageBox.Show("Ho ten khong duoc trong");
                return;
            }
            //kiem tra trung mtg
            if (checktrungMTG(mtg))
            {
                txtmatacgia.Focus();
                MessageBox.Show("Ma tac gia khong duoc trung");
                return;
            }
            //kiem tra so dien thoai
            if(dt.Length!=10 || !dt.All(char.IsDigit))
            {
                txtdienthoai.Focus();
                MessageBox.Show("Dien thoai phai la so va nho hon 10 so");
                return;
            }
            //kiem tra ngay sinh
            if(ns>=DateTime.Now)
            {
                dtngaysinh.Focus();
                MessageBox.Show("Ngay sinh phai nho hon ngay hom nay!");
                return;
            }
            //kiem tra email
            if(!email.EndsWith("@gmail.com"))
            {
                txtemail.Focus();
                MessageBox.Show("Email khong dung dinh dang @gmail.com", "Thong bao");
                return;
            }
            if (con.State == ConnectionState.Closed)
            {
                con.Open();
            }
                string sql = "insert Tacgia values(@mtg,@ht,@ns,@dc,@gt,@dt,@email)";
                SqlCommand cmd=new SqlCommand(sql, con);
                cmd.Parameters.Add("@mtg", SqlDbType.NChar, 50).Value = mtg;
                cmd.Parameters.Add("@ht", SqlDbType.NChar, 50).Value = ht;
                cmd.Parameters.Add("@ns", SqlDbType.Date, 50).Value = ns;
                cmd.Parameters.Add("@dc", SqlDbType.NChar, 200).Value = dc;
                cmd.Parameters.Add("@gt", SqlDbType.NChar, 50).Value = gt;
                cmd.Parameters.Add("@dt", SqlDbType.NChar, 50).Value = dt;
                cmd.Parameters.Add("@email", SqlDbType.NChar, 50).Value = email;
                cmd.ExecuteNonQuery();
                MessageBox.Show("Them moi thanh cong!");
                load_tacgia();
            con.Close();
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            load_tacgia();
        }

        private void dgvtacgia_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int i = e.RowIndex;
            txtmatacgia.Text = dgvtacgia.Rows[i].Cells[0].Value.ToString();
            txthoten.Text = dgvtacgia.Rows[i].Cells[1].Value.ToString();
            dtngaysinh.Value = Convert.ToDateTime(dgvtacgia.Rows[i].Cells[2].Value);
            txtdiachi.Text = dgvtacgia.Rows[i].Cells[3].Value.ToString();
            cbgioitinh.SelectedItem = dgvtacgia.Rows[i].Cells[4].Value.ToString().Trim();
            txtdienthoai.Text = dgvtacgia.Rows[i].Cells[5].Value.ToString();
            txtemail.Text = dgvtacgia.Rows[i].Cells[6].Value.ToString();
            txtmatacgia.Enabled = false;
        }
    }
}
