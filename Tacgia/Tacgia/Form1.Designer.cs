namespace Tacgia
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.txtmtg_tk = new System.Windows.Forms.TextBox();
            this.txthoten_tk = new System.Windows.Forms.TextBox();
            this.txtdienthoai_tk = new System.Windows.Forms.TextBox();
            this.cbgioitinh_tk = new System.Windows.Forms.ComboBox();
            this.btntimkiem = new System.Windows.Forms.Button();
            this.btnxuat = new System.Windows.Forms.Button();
            this.dgvdanhsach = new System.Windows.Forms.DataGridView();
            this.matacgia = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.hoten = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ngaysinh = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.diachi = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.gioitinh = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dienthoai = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.email = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.txtmtg = new System.Windows.Forms.TextBox();
            this.txthoten = new System.Windows.Forms.TextBox();
            this.txtdienthoai = new System.Windows.Forms.TextBox();
            this.txtemail = new System.Windows.Forms.TextBox();
            this.txtdiachi = new System.Windows.Forms.TextBox();
            this.dtngaysinh = new System.Windows.Forms.DateTimePicker();
            this.cbgioitinh = new System.Windows.Forms.ComboBox();
            this.btnluu = new System.Windows.Forms.Button();
            this.btnsua = new System.Windows.Forms.Button();
            this.btnxoa = new System.Windows.Forms.Button();
            this.btnthoat = new System.Windows.Forms.Button();
            this.btnreset = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvdanhsach)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(69, 56);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(72, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "Ma tac gia:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(69, 115);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(67, 16);
            this.label2.TabIndex = 1;
            this.label2.Text = "Ho va ten:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(486, 56);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(57, 16);
            this.label3.TabIndex = 2;
            this.label3.Text = "Gioi tinh:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(486, 115);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(70, 16);
            this.label4.TabIndex = 3;
            this.label4.Text = "Dien thoai:";
            // 
            // txtmtg_tk
            // 
            this.txtmtg_tk.Location = new System.Drawing.Point(164, 49);
            this.txtmtg_tk.Name = "txtmtg_tk";
            this.txtmtg_tk.Size = new System.Drawing.Size(276, 22);
            this.txtmtg_tk.TabIndex = 4;
            // 
            // txthoten_tk
            // 
            this.txthoten_tk.Location = new System.Drawing.Point(164, 109);
            this.txthoten_tk.Name = "txthoten_tk";
            this.txthoten_tk.Size = new System.Drawing.Size(276, 22);
            this.txthoten_tk.TabIndex = 5;
            // 
            // txtdienthoai_tk
            // 
            this.txtdienthoai_tk.Location = new System.Drawing.Point(587, 109);
            this.txtdienthoai_tk.Name = "txtdienthoai_tk";
            this.txtdienthoai_tk.Size = new System.Drawing.Size(276, 22);
            this.txtdienthoai_tk.TabIndex = 6;
            // 
            // cbgioitinh_tk
            // 
            this.cbgioitinh_tk.FormattingEnabled = true;
            this.cbgioitinh_tk.Items.AddRange(new object[] {
            "Nu",
            "Nam",
            "Khac"});
            this.cbgioitinh_tk.Location = new System.Drawing.Point(587, 49);
            this.cbgioitinh_tk.Name = "cbgioitinh_tk";
            this.cbgioitinh_tk.Size = new System.Drawing.Size(276, 24);
            this.cbgioitinh_tk.TabIndex = 7;
            // 
            // btntimkiem
            // 
            this.btntimkiem.Location = new System.Drawing.Point(900, 49);
            this.btntimkiem.Name = "btntimkiem";
            this.btntimkiem.Size = new System.Drawing.Size(91, 23);
            this.btntimkiem.TabIndex = 8;
            this.btntimkiem.Text = "Tim kiem";
            this.btntimkiem.UseVisualStyleBackColor = true;
            this.btntimkiem.Click += new System.EventHandler(this.btntimkiem_Click);
            // 
            // btnxuat
            // 
            this.btnxuat.Location = new System.Drawing.Point(900, 108);
            this.btnxuat.Name = "btnxuat";
            this.btnxuat.Size = new System.Drawing.Size(91, 23);
            this.btnxuat.TabIndex = 9;
            this.btnxuat.Text = "Xuat";
            this.btnxuat.UseVisualStyleBackColor = true;
            this.btnxuat.Click += new System.EventHandler(this.btnxuat_Click);
            // 
            // dgvdanhsach
            // 
            this.dgvdanhsach.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvdanhsach.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.matacgia,
            this.hoten,
            this.ngaysinh,
            this.diachi,
            this.gioitinh,
            this.dienthoai,
            this.email});
            this.dgvdanhsach.Location = new System.Drawing.Point(72, 180);
            this.dgvdanhsach.Name = "dgvdanhsach";
            this.dgvdanhsach.RowHeadersWidth = 51;
            this.dgvdanhsach.RowTemplate.Height = 24;
            this.dgvdanhsach.Size = new System.Drawing.Size(919, 215);
            this.dgvdanhsach.TabIndex = 10;
            this.dgvdanhsach.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvdanhsach_CellClick);
            // 
            // matacgia
            // 
            this.matacgia.DataPropertyName = "Matacgia";
            this.matacgia.HeaderText = "Ma tac gia";
            this.matacgia.MinimumWidth = 6;
            this.matacgia.Name = "matacgia";
            this.matacgia.Width = 125;
            // 
            // hoten
            // 
            this.hoten.DataPropertyName = "Hovaten";
            this.hoten.HeaderText = "Ho va ten";
            this.hoten.MinimumWidth = 6;
            this.hoten.Name = "hoten";
            this.hoten.Width = 125;
            // 
            // ngaysinh
            // 
            this.ngaysinh.DataPropertyName = "Ngaysinh";
            this.ngaysinh.HeaderText = "Ngay sinh";
            this.ngaysinh.MinimumWidth = 6;
            this.ngaysinh.Name = "ngaysinh";
            this.ngaysinh.Width = 125;
            // 
            // diachi
            // 
            this.diachi.DataPropertyName = "Diachi";
            this.diachi.HeaderText = "Dia chi";
            this.diachi.MinimumWidth = 6;
            this.diachi.Name = "diachi";
            this.diachi.Width = 125;
            // 
            // gioitinh
            // 
            this.gioitinh.DataPropertyName = "Gioitinh";
            this.gioitinh.HeaderText = "Gioi tinh";
            this.gioitinh.MinimumWidth = 6;
            this.gioitinh.Name = "gioitinh";
            this.gioitinh.Width = 125;
            // 
            // dienthoai
            // 
            this.dienthoai.DataPropertyName = "Dienthoai";
            this.dienthoai.HeaderText = "Dien thoai";
            this.dienthoai.MinimumWidth = 6;
            this.dienthoai.Name = "dienthoai";
            this.dienthoai.Width = 125;
            // 
            // email
            // 
            this.email.DataPropertyName = "Email";
            this.email.HeaderText = "Email";
            this.email.MinimumWidth = 6;
            this.email.Name = "email";
            this.email.Width = 125;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(69, 429);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(158, 16);
            this.label5.TabIndex = 11;
            this.label5.Text = "Cap nhat thong tin tac gia:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(69, 476);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(72, 16);
            this.label6.TabIndex = 12;
            this.label6.Text = "Ma tac gia:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(69, 524);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(67, 16);
            this.label7.TabIndex = 13;
            this.label7.Text = "Ho va ten:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(69, 573);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(70, 16);
            this.label8.TabIndex = 14;
            this.label8.Text = "Ngay sinh:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(69, 618);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(51, 16);
            this.label9.TabIndex = 15;
            this.label9.Text = "Dia chi:";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(499, 476);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(57, 16);
            this.label10.TabIndex = 16;
            this.label10.Text = "Gioi tinh:";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(499, 524);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(70, 16);
            this.label11.TabIndex = 17;
            this.label11.Text = "Dien thoai:";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(499, 573);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(44, 16);
            this.label12.TabIndex = 18;
            this.label12.Text = "Email:";
            // 
            // txtmtg
            // 
            this.txtmtg.Location = new System.Drawing.Point(164, 476);
            this.txtmtg.Name = "txtmtg";
            this.txtmtg.Size = new System.Drawing.Size(276, 22);
            this.txtmtg.TabIndex = 19;
            // 
            // txthoten
            // 
            this.txthoten.Location = new System.Drawing.Point(164, 524);
            this.txthoten.Name = "txthoten";
            this.txthoten.Size = new System.Drawing.Size(276, 22);
            this.txthoten.TabIndex = 20;
            // 
            // txtdienthoai
            // 
            this.txtdienthoai.Location = new System.Drawing.Point(587, 518);
            this.txtdienthoai.Name = "txtdienthoai";
            this.txtdienthoai.Size = new System.Drawing.Size(276, 22);
            this.txtdienthoai.TabIndex = 21;
            // 
            // txtemail
            // 
            this.txtemail.Location = new System.Drawing.Point(587, 573);
            this.txtemail.Name = "txtemail";
            this.txtemail.Size = new System.Drawing.Size(276, 22);
            this.txtemail.TabIndex = 22;
            // 
            // txtdiachi
            // 
            this.txtdiachi.Location = new System.Drawing.Point(164, 618);
            this.txtdiachi.Name = "txtdiachi";
            this.txtdiachi.Size = new System.Drawing.Size(699, 22);
            this.txtdiachi.TabIndex = 23;
            // 
            // dtngaysinh
            // 
            this.dtngaysinh.Location = new System.Drawing.Point(164, 573);
            this.dtngaysinh.Name = "dtngaysinh";
            this.dtngaysinh.Size = new System.Drawing.Size(276, 22);
            this.dtngaysinh.TabIndex = 24;
            // 
            // cbgioitinh
            // 
            this.cbgioitinh.AccessibleDescription = "";
            this.cbgioitinh.AccessibleName = "";
            this.cbgioitinh.FormattingEnabled = true;
            this.cbgioitinh.Items.AddRange(new object[] {
            "Nu",
            "Nam",
            "Khac"});
            this.cbgioitinh.Location = new System.Drawing.Point(587, 468);
            this.cbgioitinh.Name = "cbgioitinh";
            this.cbgioitinh.Size = new System.Drawing.Size(276, 24);
            this.cbgioitinh.TabIndex = 25;
            this.cbgioitinh.SelectedIndexChanged += new System.EventHandler(this.cbgioitinh_SelectedIndexChanged);
            // 
            // btnluu
            // 
            this.btnluu.Location = new System.Drawing.Point(911, 429);
            this.btnluu.Name = "btnluu";
            this.btnluu.Size = new System.Drawing.Size(91, 23);
            this.btnluu.TabIndex = 26;
            this.btnluu.Text = "Luu";
            this.btnluu.UseVisualStyleBackColor = true;
            this.btnluu.Click += new System.EventHandler(this.btnluu_Click);
            // 
            // btnsua
            // 
            this.btnsua.Location = new System.Drawing.Point(911, 491);
            this.btnsua.Name = "btnsua";
            this.btnsua.Size = new System.Drawing.Size(91, 23);
            this.btnsua.TabIndex = 27;
            this.btnsua.Text = "Sua";
            this.btnsua.UseVisualStyleBackColor = true;
            this.btnsua.Click += new System.EventHandler(this.btnsua_Click);
            // 
            // btnxoa
            // 
            this.btnxoa.Location = new System.Drawing.Point(911, 554);
            this.btnxoa.Name = "btnxoa";
            this.btnxoa.Size = new System.Drawing.Size(91, 23);
            this.btnxoa.TabIndex = 28;
            this.btnxoa.Text = "Xoa";
            this.btnxoa.UseVisualStyleBackColor = true;
            this.btnxoa.Click += new System.EventHandler(this.btnxoa_Click);
            // 
            // btnthoat
            // 
            this.btnthoat.Location = new System.Drawing.Point(911, 611);
            this.btnthoat.Name = "btnthoat";
            this.btnthoat.Size = new System.Drawing.Size(91, 23);
            this.btnthoat.TabIndex = 29;
            this.btnthoat.Text = "Thoat";
            this.btnthoat.UseVisualStyleBackColor = true;
            this.btnthoat.Click += new System.EventHandler(this.btnthoat_Click);
            // 
            // btnreset
            // 
            this.btnreset.Location = new System.Drawing.Point(911, 656);
            this.btnreset.Name = "btnreset";
            this.btnreset.Size = new System.Drawing.Size(75, 23);
            this.btnreset.TabIndex = 30;
            this.btnreset.Text = "reset";
            this.btnreset.UseVisualStyleBackColor = true;
            this.btnreset.Click += new System.EventHandler(this.btnreset_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1043, 691);
            this.Controls.Add(this.btnreset);
            this.Controls.Add(this.btnthoat);
            this.Controls.Add(this.btnxoa);
            this.Controls.Add(this.btnsua);
            this.Controls.Add(this.btnluu);
            this.Controls.Add(this.cbgioitinh);
            this.Controls.Add(this.dtngaysinh);
            this.Controls.Add(this.txtdiachi);
            this.Controls.Add(this.txtemail);
            this.Controls.Add(this.txtdienthoai);
            this.Controls.Add(this.txthoten);
            this.Controls.Add(this.txtmtg);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.dgvdanhsach);
            this.Controls.Add(this.btnxuat);
            this.Controls.Add(this.btntimkiem);
            this.Controls.Add(this.cbgioitinh_tk);
            this.Controls.Add(this.txtdienthoai_tk);
            this.Controls.Add(this.txthoten_tk);
            this.Controls.Add(this.txtmtg_tk);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.dgvdanhsach)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtmtg_tk;
        private System.Windows.Forms.TextBox txthoten_tk;
        private System.Windows.Forms.TextBox txtdienthoai_tk;
        private System.Windows.Forms.ComboBox cbgioitinh_tk;
        private System.Windows.Forms.Button btntimkiem;
        private System.Windows.Forms.Button btnxuat;
        private System.Windows.Forms.DataGridView dgvdanhsach;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox txtmtg;
        private System.Windows.Forms.TextBox txthoten;
        private System.Windows.Forms.TextBox txtdienthoai;
        private System.Windows.Forms.TextBox txtemail;
        private System.Windows.Forms.TextBox txtdiachi;
        private System.Windows.Forms.DateTimePicker dtngaysinh;
        private System.Windows.Forms.ComboBox cbgioitinh;
        private System.Windows.Forms.Button btnluu;
        private System.Windows.Forms.Button btnsua;
        private System.Windows.Forms.Button btnxoa;
        private System.Windows.Forms.Button btnthoat;
        private System.Windows.Forms.DataGridViewTextBoxColumn matacgia;
        private System.Windows.Forms.DataGridViewTextBoxColumn hoten;
        private System.Windows.Forms.DataGridViewTextBoxColumn ngaysinh;
        private System.Windows.Forms.DataGridViewTextBoxColumn diachi;
        private System.Windows.Forms.DataGridViewTextBoxColumn gioitinh;
        private System.Windows.Forms.DataGridViewTextBoxColumn dienthoai;
        private System.Windows.Forms.DataGridViewTextBoxColumn email;
        private System.Windows.Forms.Button btnreset;
    }
}

