namespace DSTPServer
{
    partial class DSTP
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DSTP));
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.button1 = new System.Windows.Forms.Button();
            this.btn_Sign = new System.Windows.Forms.Button();
            this.listBox_Lan = new System.Windows.Forms.ListBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.button2 = new System.Windows.Forms.Button();
            this.listBox_APP = new System.Windows.Forms.ListBox();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.btn_UpdateListBoxRTUNo = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.listBox_RTU_No = new System.Windows.Forms.ListBox();
            this.label4 = new System.Windows.Forms.Label();
            this.listBox_RTU = new System.Windows.Forms.ListBox();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btn_sel_display = new System.Windows.Forms.Button();
            this.txtbox_sel_Name = new System.Windows.Forms.TextBox();
            this.txtbox_sel_AccountName = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.comboBox_sel_Permission = new System.Windows.Forms.ComboBox();
            this.btn_delete = new System.Windows.Forms.Button();
            this.btn_add = new System.Windows.Forms.Button();
            this.btn_update = new System.Windows.Forms.Button();
            this.btn_Cleartxt = new System.Windows.Forms.Button();
            this.btn_display = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.DGV_Account = new System.Windows.Forms.DataGridView();
            this.comboBox_Permission = new System.Windows.Forms.ComboBox();
            this.comboBox_IsUse = new System.Windows.Forms.ComboBox();
            this.txtbox_Account_Password = new System.Windows.Forms.TextBox();
            this.txtbox_Company = new System.Windows.Forms.TextBox();
            this.txtbox_Tel = new System.Windows.Forms.TextBox();
            this.txtbox_Name = new System.Windows.Forms.TextBox();
            this.txtBox_Account_Name = new System.Windows.Forms.TextBox();
            this.picbox_Account_Picture = new System.Windows.Forms.PictureBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DGV_Account)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picbox_Account_Picture)).BeginInit();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tabControl1.ItemSize = new System.Drawing.Size(164, 40);
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tabControl1.Multiline = true;
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.Padding = new System.Drawing.Point(90, 4);
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(980, 766);
            this.tabControl1.SizeMode = System.Windows.Forms.TabSizeMode.FillToRight;
            this.tabControl1.TabIndex = 1;
            // 
            // tabPage1
            // 
            this.tabPage1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tabPage1.Controls.Add(this.button1);
            this.tabPage1.Controls.Add(this.btn_Sign);
            this.tabPage1.Controls.Add(this.listBox_Lan);
            this.tabPage1.Location = new System.Drawing.Point(4, 44);
            this.tabPage1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tabPage1.Size = new System.Drawing.Size(972, 718);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "内网连接";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(818, 649);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(145, 62);
            this.button1.TabIndex = 3;
            this.button1.Text = "清空信息";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // btn_Sign
            // 
            this.btn_Sign.Location = new System.Drawing.Point(818, 5);
            this.btn_Sign.Name = "btn_Sign";
            this.btn_Sign.Size = new System.Drawing.Size(145, 62);
            this.btn_Sign.TabIndex = 2;
            this.btn_Sign.Text = "关闭标识(测试用)";
            this.btn_Sign.UseVisualStyleBackColor = true;
            this.btn_Sign.Click += new System.EventHandler(this.btn_Sign_Click);
            // 
            // listBox_Lan
            // 
            this.listBox_Lan.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.listBox_Lan.Dock = System.Windows.Forms.DockStyle.Left;
            this.listBox_Lan.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.listBox_Lan.FormattingEnabled = true;
            this.listBox_Lan.ItemHeight = 20;
            this.listBox_Lan.Location = new System.Drawing.Point(4, 5);
            this.listBox_Lan.Name = "listBox_Lan";
            this.listBox_Lan.Size = new System.Drawing.Size(808, 706);
            this.listBox_Lan.TabIndex = 0;
            // 
            // tabPage2
            // 
            this.tabPage2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tabPage2.Controls.Add(this.button2);
            this.tabPage2.Controls.Add(this.listBox_APP);
            this.tabPage2.Location = new System.Drawing.Point(4, 44);
            this.tabPage2.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tabPage2.Size = new System.Drawing.Size(972, 718);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "APP连接";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(818, 646);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(145, 62);
            this.button2.TabIndex = 4;
            this.button2.Text = "清空信息";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // listBox_APP
            // 
            this.listBox_APP.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.listBox_APP.Dock = System.Windows.Forms.DockStyle.Left;
            this.listBox_APP.Font = new System.Drawing.Font("微软雅黑", 10.5F);
            this.listBox_APP.FormattingEnabled = true;
            this.listBox_APP.ItemHeight = 20;
            this.listBox_APP.Location = new System.Drawing.Point(4, 5);
            this.listBox_APP.Name = "listBox_APP";
            this.listBox_APP.Size = new System.Drawing.Size(808, 706);
            this.listBox_APP.TabIndex = 3;
            // 
            // tabPage3
            // 
            this.tabPage3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tabPage3.Controls.Add(this.btn_UpdateListBoxRTUNo);
            this.tabPage3.Controls.Add(this.button3);
            this.tabPage3.Controls.Add(this.listBox_RTU_No);
            this.tabPage3.Controls.Add(this.label4);
            this.tabPage3.Controls.Add(this.listBox_RTU);
            this.tabPage3.Location = new System.Drawing.Point(4, 44);
            this.tabPage3.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tabPage3.Size = new System.Drawing.Size(972, 718);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "RTU连接";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // btn_UpdateListBoxRTUNo
            // 
            this.btn_UpdateListBoxRTUNo.Location = new System.Drawing.Point(853, 29);
            this.btn_UpdateListBoxRTUNo.Name = "btn_UpdateListBoxRTUNo";
            this.btn_UpdateListBoxRTUNo.Size = new System.Drawing.Size(110, 70);
            this.btn_UpdateListBoxRTUNo.TabIndex = 14;
            this.btn_UpdateListBoxRTUNo.Text = "更新列表";
            this.btn_UpdateListBoxRTUNo.UseVisualStyleBackColor = true;
            this.btn_UpdateListBoxRTUNo.Click += new System.EventHandler(this.btn_UpdateListBoxRTUNo_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(495, 649);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(145, 62);
            this.button3.TabIndex = 13;
            this.button3.Text = "清空信息";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // listBox_RTU_No
            // 
            this.listBox_RTU_No.FormattingEnabled = true;
            this.listBox_RTU_No.ItemHeight = 21;
            this.listBox_RTU_No.Location = new System.Drawing.Point(555, 29);
            this.listBox_RTU_No.Name = "listBox_RTU_No";
            this.listBox_RTU_No.Size = new System.Drawing.Size(292, 550);
            this.listBox_RTU_No.TabIndex = 12;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(551, 5);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(89, 21);
            this.label4.TabIndex = 11;
            this.label4.Text = "在线RTU：";
            // 
            // listBox_RTU
            // 
            this.listBox_RTU.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.listBox_RTU.Dock = System.Windows.Forms.DockStyle.Left;
            this.listBox_RTU.Font = new System.Drawing.Font("微软雅黑", 10.5F);
            this.listBox_RTU.FormattingEnabled = true;
            this.listBox_RTU.ItemHeight = 20;
            this.listBox_RTU.Location = new System.Drawing.Point(4, 5);
            this.listBox_RTU.Name = "listBox_RTU";
            this.listBox_RTU.Size = new System.Drawing.Size(485, 706);
            this.listBox_RTU.TabIndex = 7;
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.groupBox2);
            this.tabPage4.Controls.Add(this.btn_delete);
            this.tabPage4.Controls.Add(this.btn_add);
            this.tabPage4.Controls.Add(this.btn_update);
            this.tabPage4.Controls.Add(this.btn_Cleartxt);
            this.tabPage4.Controls.Add(this.btn_display);
            this.tabPage4.Controls.Add(this.groupBox1);
            this.tabPage4.Controls.Add(this.comboBox_Permission);
            this.tabPage4.Controls.Add(this.comboBox_IsUse);
            this.tabPage4.Controls.Add(this.txtbox_Account_Password);
            this.tabPage4.Controls.Add(this.txtbox_Company);
            this.tabPage4.Controls.Add(this.txtbox_Tel);
            this.tabPage4.Controls.Add(this.txtbox_Name);
            this.tabPage4.Controls.Add(this.txtBox_Account_Name);
            this.tabPage4.Controls.Add(this.picbox_Account_Picture);
            this.tabPage4.Controls.Add(this.label9);
            this.tabPage4.Controls.Add(this.label8);
            this.tabPage4.Controls.Add(this.label11);
            this.tabPage4.Controls.Add(this.label10);
            this.tabPage4.Controls.Add(this.label7);
            this.tabPage4.Controls.Add(this.label6);
            this.tabPage4.Controls.Add(this.label3);
            this.tabPage4.Controls.Add(this.label2);
            this.tabPage4.Location = new System.Drawing.Point(4, 44);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(972, 718);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "账号维护";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btn_sel_display);
            this.groupBox2.Controls.Add(this.txtbox_sel_Name);
            this.groupBox2.Controls.Add(this.txtbox_sel_AccountName);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.label12);
            this.groupBox2.Controls.Add(this.comboBox_sel_Permission);
            this.groupBox2.Location = new System.Drawing.Point(12, 434);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(940, 87);
            this.groupBox2.TabIndex = 7;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "查询";
            // 
            // btn_sel_display
            // 
            this.btn_sel_display.Location = new System.Drawing.Point(765, 28);
            this.btn_sel_display.Name = "btn_sel_display";
            this.btn_sel_display.Size = new System.Drawing.Size(121, 41);
            this.btn_sel_display.TabIndex = 5;
            this.btn_sel_display.Text = "查询信息";
            this.btn_sel_display.UseVisualStyleBackColor = true;
            this.btn_sel_display.Click += new System.EventHandler(this.btn_sel_display_Click);
            // 
            // txtbox_sel_Name
            // 
            this.txtbox_sel_Name.Location = new System.Drawing.Point(301, 35);
            this.txtbox_sel_Name.Name = "txtbox_sel_Name";
            this.txtbox_sel_Name.Size = new System.Drawing.Size(110, 29);
            this.txtbox_sel_Name.TabIndex = 2;
            // 
            // txtbox_sel_AccountName
            // 
            this.txtbox_sel_AccountName.Location = new System.Drawing.Point(119, 35);
            this.txtbox_sel_AccountName.Name = "txtbox_sel_AccountName";
            this.txtbox_sel_AccountName.Size = new System.Drawing.Size(110, 29);
            this.txtbox_sel_AccountName.TabIndex = 2;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(253, 38);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(42, 21);
            this.label5.TabIndex = 1;
            this.label5.Text = "姓名";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(55, 38);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(58, 21);
            this.label1.TabIndex = 1;
            this.label1.Text = "用户名";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(441, 35);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(74, 21);
            this.label12.TabIndex = 1;
            this.label12.Text = "账号权限";
            // 
            // comboBox_sel_Permission
            // 
            this.comboBox_sel_Permission.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_sel_Permission.FormattingEnabled = true;
            this.comboBox_sel_Permission.Items.AddRange(new object[] {
            "内网使用者",
            "APP管理员",
            "APP操作员",
            ""});
            this.comboBox_sel_Permission.Location = new System.Drawing.Point(521, 32);
            this.comboBox_sel_Permission.Name = "comboBox_sel_Permission";
            this.comboBox_sel_Permission.Size = new System.Drawing.Size(161, 29);
            this.comboBox_sel_Permission.TabIndex = 4;
            // 
            // btn_delete
            // 
            this.btn_delete.Location = new System.Drawing.Point(777, 655);
            this.btn_delete.Name = "btn_delete";
            this.btn_delete.Size = new System.Drawing.Size(121, 50);
            this.btn_delete.TabIndex = 6;
            this.btn_delete.Text = "删除数据";
            this.btn_delete.UseVisualStyleBackColor = true;
            this.btn_delete.Click += new System.EventHandler(this.btn_delete_Click);
            // 
            // btn_add
            // 
            this.btn_add.Location = new System.Drawing.Point(421, 655);
            this.btn_add.Name = "btn_add";
            this.btn_add.Size = new System.Drawing.Size(121, 50);
            this.btn_add.TabIndex = 6;
            this.btn_add.Text = "新增数据";
            this.btn_add.UseVisualStyleBackColor = true;
            this.btn_add.Click += new System.EventHandler(this.btn_add_Click);
            // 
            // btn_update
            // 
            this.btn_update.Location = new System.Drawing.Point(595, 655);
            this.btn_update.Name = "btn_update";
            this.btn_update.Size = new System.Drawing.Size(121, 50);
            this.btn_update.TabIndex = 6;
            this.btn_update.Text = "修改数据";
            this.btn_update.UseVisualStyleBackColor = true;
            this.btn_update.Click += new System.EventHandler(this.btn_update_Click);
            // 
            // btn_Cleartxt
            // 
            this.btn_Cleartxt.Location = new System.Drawing.Point(248, 655);
            this.btn_Cleartxt.Name = "btn_Cleartxt";
            this.btn_Cleartxt.Size = new System.Drawing.Size(121, 50);
            this.btn_Cleartxt.TabIndex = 6;
            this.btn_Cleartxt.Text = "清空文本";
            this.btn_Cleartxt.UseVisualStyleBackColor = true;
            this.btn_Cleartxt.Click += new System.EventHandler(this.btn_Cleartxt_Click);
            // 
            // btn_display
            // 
            this.btn_display.Location = new System.Drawing.Point(83, 655);
            this.btn_display.Name = "btn_display";
            this.btn_display.Size = new System.Drawing.Size(121, 50);
            this.btn_display.TabIndex = 6;
            this.btn_display.Text = "更新信息";
            this.btn_display.UseVisualStyleBackColor = true;
            this.btn_display.Click += new System.EventHandler(this.btn_display_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.DGV_Account);
            this.groupBox1.Location = new System.Drawing.Point(12, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(940, 422);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "账号信息";
            // 
            // DGV_Account
            // 
            this.DGV_Account.AllowUserToAddRows = false;
            this.DGV_Account.AllowUserToDeleteRows = false;
            this.DGV_Account.AllowUserToOrderColumns = true;
            this.DGV_Account.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.DGV_Account.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DGV_Account.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DGV_Account.Location = new System.Drawing.Point(3, 25);
            this.DGV_Account.Name = "DGV_Account";
            this.DGV_Account.ReadOnly = true;
            this.DGV_Account.RowHeadersVisible = false;
            this.DGV_Account.RowTemplate.Height = 23;
            this.DGV_Account.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.DGV_Account.Size = new System.Drawing.Size(934, 394);
            this.DGV_Account.TabIndex = 0;
            this.DGV_Account.SelectionChanged += new System.EventHandler(this.DGV_Account_SelectionChanged);
            // 
            // comboBox_Permission
            // 
            this.comboBox_Permission.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_Permission.FormattingEnabled = true;
            this.comboBox_Permission.Items.AddRange(new object[] {
            "内网使用者",
            "APP管理员",
            "APP操作员"});
            this.comboBox_Permission.Location = new System.Drawing.Point(791, 606);
            this.comboBox_Permission.Name = "comboBox_Permission";
            this.comboBox_Permission.Size = new System.Drawing.Size(161, 29);
            this.comboBox_Permission.TabIndex = 4;
            // 
            // comboBox_IsUse
            // 
            this.comboBox_IsUse.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_IsUse.FormattingEnabled = true;
            this.comboBox_IsUse.Items.AddRange(new object[] {
            "是",
            "否"});
            this.comboBox_IsUse.Location = new System.Drawing.Point(791, 527);
            this.comboBox_IsUse.Name = "comboBox_IsUse";
            this.comboBox_IsUse.Size = new System.Drawing.Size(161, 29);
            this.comboBox_IsUse.TabIndex = 4;
            // 
            // txtbox_Account_Password
            // 
            this.txtbox_Account_Password.Location = new System.Drawing.Point(269, 606);
            this.txtbox_Account_Password.Name = "txtbox_Account_Password";
            this.txtbox_Account_Password.Size = new System.Drawing.Size(162, 29);
            this.txtbox_Account_Password.TabIndex = 3;
            // 
            // txtbox_Company
            // 
            this.txtbox_Company.Location = new System.Drawing.Point(533, 606);
            this.txtbox_Company.Name = "txtbox_Company";
            this.txtbox_Company.Size = new System.Drawing.Size(162, 29);
            this.txtbox_Company.TabIndex = 3;
            // 
            // txtbox_Tel
            // 
            this.txtbox_Tel.Location = new System.Drawing.Point(533, 568);
            this.txtbox_Tel.Name = "txtbox_Tel";
            this.txtbox_Tel.Size = new System.Drawing.Size(162, 29);
            this.txtbox_Tel.TabIndex = 3;
            // 
            // txtbox_Name
            // 
            this.txtbox_Name.Location = new System.Drawing.Point(533, 527);
            this.txtbox_Name.Name = "txtbox_Name";
            this.txtbox_Name.Size = new System.Drawing.Size(162, 29);
            this.txtbox_Name.TabIndex = 3;
            // 
            // txtBox_Account_Name
            // 
            this.txtBox_Account_Name.Location = new System.Drawing.Point(269, 527);
            this.txtBox_Account_Name.Name = "txtBox_Account_Name";
            this.txtBox_Account_Name.Size = new System.Drawing.Size(162, 29);
            this.txtBox_Account_Name.TabIndex = 3;
            // 
            // picbox_Account_Picture
            // 
            this.picbox_Account_Picture.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picbox_Account_Picture.Location = new System.Drawing.Point(71, 530);
            this.picbox_Account_Picture.Name = "picbox_Account_Picture";
            this.picbox_Account_Picture.Size = new System.Drawing.Size(100, 100);
            this.picbox_Account_Picture.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picbox_Account_Picture.TabIndex = 2;
            this.picbox_Account_Picture.TabStop = false;
            this.picbox_Account_Picture.Click += new System.EventHandler(this.picbox_Account_Picture_Click);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(8, 530);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(57, 21);
            this.label9.TabIndex = 1;
            this.label9.Text = "照   片";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(453, 609);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(74, 21);
            this.label8.TabIndex = 1;
            this.label8.Text = "所在单位";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(711, 609);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(74, 21);
            this.label11.TabIndex = 1;
            this.label11.Text = "账号权限";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(711, 530);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(74, 21);
            this.label10.TabIndex = 1;
            this.label10.Text = "是否使用";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(453, 571);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(74, 21);
            this.label7.TabIndex = 1;
            this.label7.Text = "联系方式";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(470, 530);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(57, 21);
            this.label6.TabIndex = 1;
            this.label6.Text = "姓   名";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(205, 609);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(57, 21);
            this.label3.TabIndex = 1;
            this.label3.Text = "密   码";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(205, 530);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(58, 21);
            this.label2.TabIndex = 1;
            this.label2.Text = "用户名";
            // 
            // DSTP
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(980, 766);
            this.Controls.Add(this.tabControl1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(996, 805);
            this.MinimumSize = new System.Drawing.Size(996, 805);
            this.Name = "DSTP";
            this.Text = "堵水调剖数据中转系统";
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.tabPage4.ResumeLayout(false);
            this.tabPage4.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.DGV_Account)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picbox_Account_Picture)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.ListBox listBox_Lan;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.ListBox listBox_APP;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.ListBox listBox_RTU_No;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ListBox listBox_RTU;
        private System.Windows.Forms.Button btn_Sign;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox comboBox_Permission;
        private System.Windows.Forms.ComboBox comboBox_IsUse;
        private System.Windows.Forms.TextBox txtbox_Account_Password;
        private System.Windows.Forms.TextBox txtbox_Company;
        private System.Windows.Forms.TextBox txtbox_Tel;
        private System.Windows.Forms.TextBox txtbox_Name;
        private System.Windows.Forms.TextBox txtBox_Account_Name;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.DataGridView DGV_Account;
        private System.Windows.Forms.Button btn_delete;
        private System.Windows.Forms.Button btn_add;
        private System.Windows.Forms.Button btn_update;
        private System.Windows.Forms.Button btn_display;
        private System.Windows.Forms.Button btn_Cleartxt;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox txtbox_sel_AccountName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtbox_sel_Name;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.ComboBox comboBox_sel_Permission;
        private System.Windows.Forms.Button btn_sel_display;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.PictureBox picbox_Account_Picture;
        private System.Windows.Forms.Button btn_UpdateListBoxRTUNo;
    }
}

