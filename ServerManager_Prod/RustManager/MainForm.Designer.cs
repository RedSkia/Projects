
namespace RustManager
{
    partial class MainForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.customButton1 = new IRSM.ClassFunctions.CustomButton();
            this.customButton2 = new IRSM.ClassFunctions.CustomButton();
            this.customButton3 = new IRSM.ClassFunctions.CustomButton();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.panel21 = new System.Windows.Forms.Panel();
            this.panel20 = new System.Windows.Forms.Panel();
            this.panel19 = new System.Windows.Forms.Panel();
            this.panel18 = new System.Windows.Forms.Panel();
            this.panel16 = new System.Windows.Forms.Panel();
            this.panel15 = new System.Windows.Forms.Panel();
            this.panel12 = new System.Windows.Forms.Panel();
            this.panel9 = new System.Windows.Forms.Panel();
            this.panel14 = new System.Windows.Forms.Panel();
            this.panel4 = new System.Windows.Forms.Panel();
            this.panel10 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.panel11 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.customButton7 = new IRSM.ClassFunctions.CustomButton();
            this.customButton6 = new IRSM.ClassFunctions.CustomButton();
            this.customButton5 = new IRSM.ClassFunctions.CustomButton();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.customButton4 = new IRSM.ClassFunctions.CustomButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel13 = new System.Windows.Forms.Panel();
            this.panel5 = new System.Windows.Forms.Panel();
            this.panel6 = new System.Windows.Forms.Panel();
            this.panel7 = new System.Windows.Forms.Panel();
            this.panel8 = new System.Windows.Forms.Panel();
            this.panel17 = new System.Windows.Forms.Panel();
            this.DisplayPanel = new System.Windows.Forms.Panel();
            this.RestartTimerLog = new System.Windows.Forms.Timer(this.components);
            this.ServerStalledChecker = new System.ComponentModel.BackgroundWorker();
            this.ServerAliveStatus = new System.ComponentModel.BackgroundWorker();
            this.ServerPathChecker = new System.ComponentModel.BackgroundWorker();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.tableLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(10)))), ((int)(((byte)(20)))), ((int)(((byte)(30)))));
            this.tableLayoutPanel1.ColumnCount = 8;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel1.Controls.Add(this.label3, 3, 0);
            this.tableLayoutPanel1.Controls.Add(this.label2, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.pictureBox1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.label1, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.customButton1, 7, 0);
            this.tableLayoutPanel1.Controls.Add(this.customButton2, 6, 0);
            this.tableLayoutPanel1.Controls.Add(this.customButton3, 5, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(800, 35);
            this.tableLayoutPanel1.TabIndex = 0;
            this.tableLayoutPanel1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.tableLayoutPanel1_MouseMove);
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Arial", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label3.Location = new System.Drawing.Point(455, 3);
            this.label3.Margin = new System.Windows.Forms.Padding(0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(68, 28);
            this.label3.TabIndex = 3;
            this.label3.Text = "1.0.0";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label3.MouseMove += new System.Windows.Forms.MouseEventHandler(this.label3_MouseMove);
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Arial", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label2.Location = new System.Drawing.Point(206, 3);
            this.label2.Margin = new System.Windows.Forms.Padding(0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(249, 28);
            this.label2.TabIndex = 2;
            this.label2.Text = "Rust Server Manager";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label2.MouseMove += new System.Windows.Forms.MouseEventHandler(this.label2_MouseMove);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox1.Image = global::RustManager.Data.Icons.Logo;
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(40, 35);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseMove);
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Arial", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label1.Location = new System.Drawing.Point(40, 2);
            this.label1.Margin = new System.Windows.Forms.Padding(0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(166, 30);
            this.label1.TabIndex = 1;
            this.label1.Text = "Infinitynet.dk";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.label1_MouseMove);
            // 
            // customButton1
            // 
            this.customButton1.BackColor = System.Drawing.Color.Transparent;
            this.customButton1.BackgroundImage = global::RustManager.Data.Icons.CloseW;
            this.customButton1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.customButton1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.customButton1.FlatAppearance.BorderSize = 0;
            this.customButton1.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(70)))), ((int)(((byte)(80)))));
            this.customButton1.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(60)))), ((int)(((byte)(70)))));
            this.customButton1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.customButton1.ForeColor = System.Drawing.Color.White;
            this.customButton1.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.customButton1.Location = new System.Drawing.Point(765, 0);
            this.customButton1.Margin = new System.Windows.Forms.Padding(0);
            this.customButton1.Name = "customButton1";
            this.customButton1.Size = new System.Drawing.Size(35, 35);
            this.customButton1.TabIndex = 7;
            this.customButton1.UseVisualStyleBackColor = false;
            this.customButton1.Click += new System.EventHandler(this.customButton1_Click);
            // 
            // customButton2
            // 
            this.customButton2.BackColor = System.Drawing.Color.Transparent;
            this.customButton2.BackgroundImage = global::RustManager.Data.Icons.MaximiseW;
            this.customButton2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.customButton2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.customButton2.FlatAppearance.BorderSize = 0;
            this.customButton2.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(70)))), ((int)(((byte)(80)))));
            this.customButton2.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(60)))), ((int)(((byte)(70)))));
            this.customButton2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.customButton2.ForeColor = System.Drawing.Color.White;
            this.customButton2.Image = global::RustManager.Data.Icons.MaximiseW;
            this.customButton2.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.customButton2.Location = new System.Drawing.Point(730, 0);
            this.customButton2.Margin = new System.Windows.Forms.Padding(0);
            this.customButton2.Name = "customButton2";
            this.customButton2.Size = new System.Drawing.Size(35, 35);
            this.customButton2.TabIndex = 6;
            this.customButton2.UseVisualStyleBackColor = false;
            this.customButton2.Click += new System.EventHandler(this.customButton2_Click);
            // 
            // customButton3
            // 
            this.customButton3.BackColor = System.Drawing.Color.Transparent;
            this.customButton3.BackgroundImage = global::RustManager.Data.Icons.MinimizeW;
            this.customButton3.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.customButton3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.customButton3.FlatAppearance.BorderSize = 0;
            this.customButton3.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(70)))), ((int)(((byte)(80)))));
            this.customButton3.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(60)))), ((int)(((byte)(70)))));
            this.customButton3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.customButton3.ForeColor = System.Drawing.Color.White;
            this.customButton3.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.customButton3.Location = new System.Drawing.Point(695, 0);
            this.customButton3.Margin = new System.Windows.Forms.Padding(0);
            this.customButton3.Name = "customButton3";
            this.customButton3.Size = new System.Drawing.Size(35, 35);
            this.customButton3.TabIndex = 5;
            this.customButton3.UseVisualStyleBackColor = false;
            this.customButton3.Click += new System.EventHandler(this.customButton3_Click);
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(30)))), ((int)(((byte)(40)))));
            this.tableLayoutPanel2.ColumnCount = 9;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 250F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 2F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 2F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 2F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 2F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel2.Controls.Add(this.panel21, 1, 2);
            this.tableLayoutPanel2.Controls.Add(this.panel20, 3, 2);
            this.tableLayoutPanel2.Controls.Add(this.panel19, 5, 2);
            this.tableLayoutPanel2.Controls.Add(this.panel18, 7, 2);
            this.tableLayoutPanel2.Controls.Add(this.panel16, 7, 1);
            this.tableLayoutPanel2.Controls.Add(this.panel15, 5, 1);
            this.tableLayoutPanel2.Controls.Add(this.panel12, 8, 2);
            this.tableLayoutPanel2.Controls.Add(this.panel9, 2, 2);
            this.tableLayoutPanel2.Controls.Add(this.panel14, 3, 1);
            this.tableLayoutPanel2.Controls.Add(this.panel4, 7, 0);
            this.tableLayoutPanel2.Controls.Add(this.panel10, 4, 2);
            this.tableLayoutPanel2.Controls.Add(this.panel3, 5, 0);
            this.tableLayoutPanel2.Controls.Add(this.panel11, 6, 2);
            this.tableLayoutPanel2.Controls.Add(this.panel2, 3, 0);
            this.tableLayoutPanel2.Controls.Add(this.customButton7, 8, 0);
            this.tableLayoutPanel2.Controls.Add(this.customButton6, 6, 0);
            this.tableLayoutPanel2.Controls.Add(this.customButton5, 4, 0);
            this.tableLayoutPanel2.Controls.Add(this.comboBox1, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.customButton4, 2, 0);
            this.tableLayoutPanel2.Controls.Add(this.panel1, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.panel13, 1, 1);
            this.tableLayoutPanel2.Controls.Add(this.panel5, 2, 1);
            this.tableLayoutPanel2.Controls.Add(this.panel6, 4, 1);
            this.tableLayoutPanel2.Controls.Add(this.panel7, 6, 1);
            this.tableLayoutPanel2.Controls.Add(this.panel8, 8, 1);
            this.tableLayoutPanel2.Controls.Add(this.panel17, 0, 2);
            this.tableLayoutPanel2.Location = new System.Drawing.Point(5, 35);
            this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 3;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 5F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 2F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(790, 47);
            this.tableLayoutPanel2.TabIndex = 1;
            // 
            // panel21
            // 
            this.panel21.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel21.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(10)))), ((int)(((byte)(20)))), ((int)(((byte)(30)))));
            this.panel21.Location = new System.Drawing.Point(250, 45);
            this.panel21.Margin = new System.Windows.Forms.Padding(0);
            this.panel21.Name = "panel21";
            this.panel21.Size = new System.Drawing.Size(2, 2);
            this.panel21.TabIndex = 9;
            // 
            // panel20
            // 
            this.panel20.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel20.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(10)))), ((int)(((byte)(20)))), ((int)(((byte)(30)))));
            this.panel20.Location = new System.Drawing.Point(385, 45);
            this.panel20.Margin = new System.Windows.Forms.Padding(0);
            this.panel20.Name = "panel20";
            this.panel20.Size = new System.Drawing.Size(2, 2);
            this.panel20.TabIndex = 9;
            // 
            // panel19
            // 
            this.panel19.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel19.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(10)))), ((int)(((byte)(20)))), ((int)(((byte)(30)))));
            this.panel19.Location = new System.Drawing.Point(520, 45);
            this.panel19.Margin = new System.Windows.Forms.Padding(0);
            this.panel19.Name = "panel19";
            this.panel19.Size = new System.Drawing.Size(2, 2);
            this.panel19.TabIndex = 9;
            // 
            // panel18
            // 
            this.panel18.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel18.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(10)))), ((int)(((byte)(20)))), ((int)(((byte)(30)))));
            this.panel18.Location = new System.Drawing.Point(655, 45);
            this.panel18.Margin = new System.Windows.Forms.Padding(0);
            this.panel18.Name = "panel18";
            this.panel18.Size = new System.Drawing.Size(2, 2);
            this.panel18.TabIndex = 9;
            // 
            // panel16
            // 
            this.panel16.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel16.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(10)))), ((int)(((byte)(20)))), ((int)(((byte)(30)))));
            this.panel16.Location = new System.Drawing.Point(655, 40);
            this.panel16.Margin = new System.Windows.Forms.Padding(0);
            this.panel16.Name = "panel16";
            this.panel16.Size = new System.Drawing.Size(2, 5);
            this.panel16.TabIndex = 8;
            // 
            // panel15
            // 
            this.panel15.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel15.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(10)))), ((int)(((byte)(20)))), ((int)(((byte)(30)))));
            this.panel15.Location = new System.Drawing.Point(520, 40);
            this.panel15.Margin = new System.Windows.Forms.Padding(0);
            this.panel15.Name = "panel15";
            this.panel15.Size = new System.Drawing.Size(2, 5);
            this.panel15.TabIndex = 8;
            // 
            // panel12
            // 
            this.panel12.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel12.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(10)))), ((int)(((byte)(20)))), ((int)(((byte)(30)))));
            this.panel12.Location = new System.Drawing.Point(657, 45);
            this.panel12.Margin = new System.Windows.Forms.Padding(0);
            this.panel12.Name = "panel12";
            this.panel12.Size = new System.Drawing.Size(133, 2);
            this.panel12.TabIndex = 8;
            // 
            // panel9
            // 
            this.panel9.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel9.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(10)))), ((int)(((byte)(20)))), ((int)(((byte)(30)))));
            this.panel9.Location = new System.Drawing.Point(252, 45);
            this.panel9.Margin = new System.Windows.Forms.Padding(0);
            this.panel9.Name = "panel9";
            this.panel9.Size = new System.Drawing.Size(133, 2);
            this.panel9.TabIndex = 15;
            // 
            // panel14
            // 
            this.panel14.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel14.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(10)))), ((int)(((byte)(20)))), ((int)(((byte)(30)))));
            this.panel14.Location = new System.Drawing.Point(385, 40);
            this.panel14.Margin = new System.Windows.Forms.Padding(0);
            this.panel14.Name = "panel14";
            this.panel14.Size = new System.Drawing.Size(2, 5);
            this.panel14.TabIndex = 8;
            // 
            // panel4
            // 
            this.panel4.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(10)))), ((int)(((byte)(20)))), ((int)(((byte)(30)))));
            this.panel4.Location = new System.Drawing.Point(655, 0);
            this.panel4.Margin = new System.Windows.Forms.Padding(0);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(2, 40);
            this.panel4.TabIndex = 10;
            // 
            // panel10
            // 
            this.panel10.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel10.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(10)))), ((int)(((byte)(20)))), ((int)(((byte)(30)))));
            this.panel10.Location = new System.Drawing.Point(387, 45);
            this.panel10.Margin = new System.Windows.Forms.Padding(0);
            this.panel10.Name = "panel10";
            this.panel10.Size = new System.Drawing.Size(133, 2);
            this.panel10.TabIndex = 16;
            // 
            // panel3
            // 
            this.panel3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(10)))), ((int)(((byte)(20)))), ((int)(((byte)(30)))));
            this.panel3.Location = new System.Drawing.Point(520, 0);
            this.panel3.Margin = new System.Windows.Forms.Padding(0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(2, 40);
            this.panel3.TabIndex = 9;
            // 
            // panel11
            // 
            this.panel11.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel11.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(10)))), ((int)(((byte)(20)))), ((int)(((byte)(30)))));
            this.panel11.Location = new System.Drawing.Point(522, 45);
            this.panel11.Margin = new System.Windows.Forms.Padding(0);
            this.panel11.Name = "panel11";
            this.panel11.Size = new System.Drawing.Size(133, 2);
            this.panel11.TabIndex = 8;
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(10)))), ((int)(((byte)(20)))), ((int)(((byte)(30)))));
            this.panel2.Location = new System.Drawing.Point(385, 0);
            this.panel2.Margin = new System.Windows.Forms.Padding(0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(2, 40);
            this.panel2.TabIndex = 8;
            // 
            // customButton7
            // 
            this.customButton7.BackColor = System.Drawing.Color.Transparent;
            this.customButton7.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.customButton7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.customButton7.FlatAppearance.BorderSize = 0;
            this.customButton7.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(70)))), ((int)(((byte)(80)))));
            this.customButton7.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(60)))), ((int)(((byte)(70)))));
            this.customButton7.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.customButton7.ForeColor = System.Drawing.Color.White;
            this.customButton7.Image = global::RustManager.Data.Icons.Gear32W;
            this.customButton7.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.customButton7.Location = new System.Drawing.Point(657, 0);
            this.customButton7.Margin = new System.Windows.Forms.Padding(0);
            this.customButton7.Name = "customButton7";
            this.customButton7.Size = new System.Drawing.Size(133, 40);
            this.customButton7.TabIndex = 4;
            this.customButton7.Text = "Options";
            this.customButton7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.customButton7.UseVisualStyleBackColor = false;
            this.customButton7.Click += new System.EventHandler(this.customButton7_Click);
            // 
            // customButton6
            // 
            this.customButton6.BackColor = System.Drawing.Color.Transparent;
            this.customButton6.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.customButton6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.customButton6.FlatAppearance.BorderSize = 0;
            this.customButton6.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(70)))), ((int)(((byte)(80)))));
            this.customButton6.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(60)))), ((int)(((byte)(70)))));
            this.customButton6.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.customButton6.ForeColor = System.Drawing.Color.White;
            this.customButton6.Image = global::RustManager.Data.Icons.Installer32W;
            this.customButton6.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.customButton6.Location = new System.Drawing.Point(522, 0);
            this.customButton6.Margin = new System.Windows.Forms.Padding(0);
            this.customButton6.Name = "customButton6";
            this.customButton6.Size = new System.Drawing.Size(133, 40);
            this.customButton6.TabIndex = 3;
            this.customButton6.Text = "Installer";
            this.customButton6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.customButton6.UseVisualStyleBackColor = false;
            this.customButton6.Click += new System.EventHandler(this.customButton6_Click);
            // 
            // customButton5
            // 
            this.customButton5.BackColor = System.Drawing.Color.Transparent;
            this.customButton5.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.customButton5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.customButton5.FlatAppearance.BorderSize = 0;
            this.customButton5.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(70)))), ((int)(((byte)(80)))));
            this.customButton5.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(60)))), ((int)(((byte)(70)))));
            this.customButton5.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.customButton5.ForeColor = System.Drawing.Color.White;
            this.customButton5.Image = global::RustManager.Data.Icons.Plugin32W;
            this.customButton5.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.customButton5.Location = new System.Drawing.Point(387, 0);
            this.customButton5.Margin = new System.Windows.Forms.Padding(0);
            this.customButton5.Name = "customButton5";
            this.customButton5.Size = new System.Drawing.Size(133, 40);
            this.customButton5.TabIndex = 2;
            this.customButton5.Text = "Plugins";
            this.customButton5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.customButton5.UseVisualStyleBackColor = false;
            this.customButton5.Click += new System.EventHandler(this.customButton5_Click);
            // 
            // comboBox1
            // 
            this.comboBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(0, 6);
            this.comboBox1.Margin = new System.Windows.Forms.Padding(0);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(250, 36);
            this.comboBox1.TabIndex = 0;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            this.comboBox1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.comboBox1_KeyDown);
            this.comboBox1.KeyUp += new System.Windows.Forms.KeyEventHandler(this.comboBox1_KeyUp);
            // 
            // customButton4
            // 
            this.customButton4.BackColor = System.Drawing.Color.Transparent;
            this.customButton4.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.customButton4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.customButton4.FlatAppearance.BorderSize = 0;
            this.customButton4.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(70)))), ((int)(((byte)(80)))));
            this.customButton4.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(60)))), ((int)(((byte)(70)))));
            this.customButton4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.customButton4.ForeColor = System.Drawing.Color.White;
            this.customButton4.Image = global::RustManager.Data.Icons.House32W;
            this.customButton4.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.customButton4.Location = new System.Drawing.Point(252, 0);
            this.customButton4.Margin = new System.Windows.Forms.Padding(0);
            this.customButton4.Name = "customButton4";
            this.customButton4.Size = new System.Drawing.Size(133, 40);
            this.customButton4.TabIndex = 1;
            this.customButton4.Text = "Home";
            this.customButton4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.customButton4.UseVisualStyleBackColor = false;
            this.customButton4.Click += new System.EventHandler(this.customButton4_Click);
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(10)))), ((int)(((byte)(20)))), ((int)(((byte)(30)))));
            this.panel1.Location = new System.Drawing.Point(250, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(2, 40);
            this.panel1.TabIndex = 7;
            // 
            // panel13
            // 
            this.panel13.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel13.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(10)))), ((int)(((byte)(20)))), ((int)(((byte)(30)))));
            this.panel13.Location = new System.Drawing.Point(250, 40);
            this.panel13.Margin = new System.Windows.Forms.Padding(0);
            this.panel13.Name = "panel13";
            this.panel13.Size = new System.Drawing.Size(2, 5);
            this.panel13.TabIndex = 17;
            // 
            // panel5
            // 
            this.panel5.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(90)))), ((int)(((byte)(120)))));
            this.panel5.Location = new System.Drawing.Point(252, 40);
            this.panel5.Margin = new System.Windows.Forms.Padding(0);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(133, 5);
            this.panel5.TabIndex = 11;
            // 
            // panel6
            // 
            this.panel6.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(90)))), ((int)(((byte)(120)))));
            this.panel6.Location = new System.Drawing.Point(387, 40);
            this.panel6.Margin = new System.Windows.Forms.Padding(0);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(133, 5);
            this.panel6.TabIndex = 12;
            // 
            // panel7
            // 
            this.panel7.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel7.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(90)))), ((int)(((byte)(120)))));
            this.panel7.Location = new System.Drawing.Point(522, 40);
            this.panel7.Margin = new System.Windows.Forms.Padding(0);
            this.panel7.Name = "panel7";
            this.panel7.Size = new System.Drawing.Size(133, 5);
            this.panel7.TabIndex = 13;
            // 
            // panel8
            // 
            this.panel8.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel8.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(90)))), ((int)(((byte)(120)))));
            this.panel8.Location = new System.Drawing.Point(657, 40);
            this.panel8.Margin = new System.Windows.Forms.Padding(0);
            this.panel8.Name = "panel8";
            this.panel8.Size = new System.Drawing.Size(133, 5);
            this.panel8.TabIndex = 14;
            // 
            // panel17
            // 
            this.panel17.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel17.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(10)))), ((int)(((byte)(20)))), ((int)(((byte)(30)))));
            this.panel17.Location = new System.Drawing.Point(0, 45);
            this.panel17.Margin = new System.Windows.Forms.Padding(0);
            this.panel17.Name = "panel17";
            this.panel17.Size = new System.Drawing.Size(250, 2);
            this.panel17.TabIndex = 8;
            // 
            // DisplayPanel
            // 
            this.DisplayPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.DisplayPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(40)))), ((int)(((byte)(50)))));
            this.DisplayPanel.Location = new System.Drawing.Point(5, 82);
            this.DisplayPanel.Margin = new System.Windows.Forms.Padding(5, 0, 5, 5);
            this.DisplayPanel.Name = "DisplayPanel";
            this.DisplayPanel.Size = new System.Drawing.Size(790, 463);
            this.DisplayPanel.TabIndex = 2;
            // 
            // RestartTimerLog
            // 
            this.RestartTimerLog.Interval = 1000;
            this.RestartTimerLog.Tick += new System.EventHandler(this.RestartTimerLog_Tick);
            // 
            // ServerStalledChecker
            // 
            this.ServerStalledChecker.WorkerSupportsCancellation = true;
            this.ServerStalledChecker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.ServerStalledChecker_DoWork);
            this.ServerStalledChecker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.ServerStalledChecker_RunWorkerCompleted);
            // 
            // ServerAliveStatus
            // 
            this.ServerAliveStatus.DoWork += new System.ComponentModel.DoWorkEventHandler(this.ServerAliveStatus_DoWork);
            this.ServerAliveStatus.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.ServerAliveStatus_RunWorkerCompleted);
            // 
            // ServerPathChecker
            // 
            this.ServerPathChecker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.ServerPathChecker_RunWorkerCompleted);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(15F, 28F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(10)))), ((int)(((byte)(20)))), ((int)(((byte)(30)))));
            this.ClientSize = new System.Drawing.Size(800, 550);
            this.Controls.Add(this.DisplayPanel);
            this.Controls.Add(this.tableLayoutPanel2);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Font = new System.Drawing.Font("Arial", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(800, 550);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "MainForm";
            this.Activated += new System.EventHandler(this.MainForm_Activated);
            this.Deactivate += new System.EventHandler(this.MainForm_Deactivate);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.Move += new System.EventHandler(this.MainForm_Move);
            this.Resize += new System.EventHandler(this.MainForm_Resize);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private IRSM.ClassFunctions.CustomButton customButton1;
        private IRSM.ClassFunctions.CustomButton customButton2;
        private IRSM.ClassFunctions.CustomButton customButton3;
        private IRSM.ClassFunctions.CustomButton customButton7;
        private IRSM.ClassFunctions.CustomButton customButton6;
        private IRSM.ClassFunctions.CustomButton customButton5;
        private IRSM.ClassFunctions.CustomButton customButton4;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.Panel panel7;
        private System.Windows.Forms.Panel panel8;
        private System.Windows.Forms.Panel DisplayPanel;
        private System.Windows.Forms.Panel panel17;
        private System.Windows.Forms.Panel panel16;
        private System.Windows.Forms.Panel panel15;
        private System.Windows.Forms.Panel panel14;
        private System.Windows.Forms.Panel panel12;
        private System.Windows.Forms.Panel panel11;
        private System.Windows.Forms.Panel panel9;
        private System.Windows.Forms.Panel panel10;
        private System.Windows.Forms.Panel panel13;
        private System.Windows.Forms.Panel panel21;
        private System.Windows.Forms.Panel panel20;
        private System.Windows.Forms.Panel panel19;
        private System.Windows.Forms.Panel panel18;
        private System.Windows.Forms.Timer RestartTimerLog;
        private System.ComponentModel.BackgroundWorker ServerStalledChecker;
        private System.ComponentModel.BackgroundWorker ServerAliveStatus;
        private System.ComponentModel.BackgroundWorker ServerPathChecker;
    }
}