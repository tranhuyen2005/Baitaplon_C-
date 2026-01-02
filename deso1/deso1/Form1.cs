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

namespace deso1
{
    public partial class Form1 : Form
    {
        SqlConnection con = new SqlConnection("Data Source=NGUYENTRUNGKIEN\\SQLEXPRESS;Initial Catalog=deso1;Integrated Security=True");
        public Form1()
        {
            InitializeComponent();
        }

        private void load_sanpham()
        {
            if (con.State == ConnectionState.Closed)
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("select * from Products", con);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable tb = new DataTable();
                da.Fill(tb);
                cmd.Dispose();
                con.Close();
                dgvdanhsach.DataSource = tb;
                dgvdanhsach.Refresh();
            }
        }
        private bool checktrungMTG(string msp)
        {
            if (con.State == ConnectionState.Closed)
            {
                con.Open();
            }

            string sql = "select count(*) from Products where ProductCode=@msp";
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
            string ml = txtmaloai.Text.Trim();
            if (msp == "")
            {
                txtmasp.Focus();
                MessageBox.Show("Ma san pham khong duoc trong!");
                return;
            }
            if (ml == "")
            {
                txtmaloai.Focus();
                MessageBox.Show("Ma loai khong duoc trong!");
                return;
            }
            if (ten == "")
            {
                txttensp.Focus();
                MessageBox.Show("Ten san pham khong duoc trong!");
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
                string sql = "Insert Products Values(@msp,@ten,@gia,@sl,@ml)";
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.Add("@msp", SqlDbType.NChar, 50).Value = msp;
                cmd.Parameters.Add("@ten", SqlDbType.NChar, 50).Value = ten;
                cmd.Parameters.Add("@gia", SqlDbType.Int).Value = gia;
                cmd.Parameters.Add("@sl", SqlDbType.Int).Value = sl;
                cmd.Parameters.Add("@ml", SqlDbType.NChar, 50).Value = ml;
                cmd.ExecuteNonQuery();
                con.Close();
                MessageBox.Show("Them moi thanh cong!");
                load_sanpham();

            }
        }

        private void dgvdanhsach_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dgvdanhsach_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int i = e.RowIndex;
            txtmasp.Text = dgvdanhsach.Rows[i].Cells["ProductCode"].Value.ToString();
            txttensp.Text = dgvdanhsach.Rows[i].Cells["Description"].Value.ToString();
            txtgia.Text = dgvdanhsach.Rows[i].Cells["UnitPrice"].Value.ToString();
            txtsoluong.Text = dgvdanhsach.Rows[i].Cells["OnHandQuantity"].Value.ToString();
            txtmaloai.Text = dgvdanhsach.Rows[i].Cells["CategoryID"].Value.ToString();
            txtmasp.Enabled = false;
        }
    }
}
