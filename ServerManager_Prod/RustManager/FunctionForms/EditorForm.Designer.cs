
namespace RustManager.FunctionForms
{
    partial class EditorForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EditorForm));
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.richTextBoxSyntax = new System.Windows.Forms.RichTextBox();
            this.customButton1 = new IRSM.ClassFunctions.CustomButton();
            this.ProgressPanel2 = new System.Windows.Forms.Panel();
            this.ProgressPanel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.panel8 = new System.Windows.Forms.Panel();
            this.panel9 = new System.Windows.Forms.Panel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panel14 = new System.Windows.Forms.Panel();
            this.panel12 = new System.Windows.Forms.Panel();
            this.panel6 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.customButton6 = new IRSM.ClassFunctions.CustomButton();
            this.panel13 = new System.Windows.Forms.Panel();
            this.panel10 = new System.Windows.Forms.Panel();
            this.panel7 = new System.Windows.Forms.Panel();
            this.panel5 = new System.Windows.Forms.Panel();
            this.panel4 = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.customButton2 = new IRSM.ClassFunctions.CustomButton();
            this.panel11 = new System.Windows.Forms.Panel();
            this.richTextBoxLineNumber = new System.Windows.Forms.RichTextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.LoadingTimer = new System.Windows.Forms.Timer(this.components);
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label5 = new System.Windows.Forms.Label();
            this.customButton3 = new IRSM.ClassFunctions.CustomButton();
            this.customButton4 = new IRSM.ClassFunctions.CustomButton();
            this.customButton5 = new IRSM.ClassFunctions.CustomButton();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.DataCheckerTimer = new System.Windows.Forms.Timer(this.components);
            this.ProgressPanel2.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.tableLayoutPanel4.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.WorkerReportsProgress = true;
            this.backgroundWorker1.WorkerSupportsCancellation = true;
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker1_RunWorkerCompleted);
            // 
            // richTextBoxSyntax
            // 
            this.richTextBoxSyntax.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(40)))), ((int)(((byte)(50)))));
            this.richTextBoxSyntax.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.richTextBoxSyntax.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBoxSyntax.ForeColor = System.Drawing.Color.White;
            this.richTextBoxSyntax.Location = new System.Drawing.Point(0, 0);
            this.richTextBoxSyntax.Margin = new System.Windows.Forms.Padding(0);
            this.richTextBoxSyntax.Name = "richTextBoxSyntax";
            this.richTextBoxSyntax.Size = new System.Drawing.Size(758, 389);
            this.richTextBoxSyntax.TabIndex = 3;
            this.richTextBoxSyntax.Text = "";
            this.richTextBoxSyntax.WordWrap = false;
            this.richTextBoxSyntax.ContentsResized += new System.Windows.Forms.ContentsResizedEventHandler(this.richTextBoxSyntax_ContentsResized);
            this.richTextBoxSyntax.SelectionChanged += new System.EventHandler(this.richTextBoxSyntax_SelectionChanged);
            this.richTextBoxSyntax.VScroll += new System.EventHandler(this.richTextBoxSyntax_VScroll);
            this.richTextBoxSyntax.FontChanged += new System.EventHandler(this.richTextBoxSyntax_FontChanged);
            this.richTextBoxSyntax.TextChanged += new System.EventHandler(this.richTextBoxSyntax_TextChanged);
            this.richTextBoxSyntax.KeyDown += new System.Windows.Forms.KeyEventHandler(this.richTextBoxSyntax_KeyDown);
            // 
            // customButton1
            // 
            this.customButton1.BackColor = System.Drawing.Color.Transparent;
            this.customButton1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.customButton1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.customButton1.FlatAppearance.BorderSize = 0;
            this.customButton1.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(70)))), ((int)(((byte)(80)))));
            this.customButton1.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(60)))), ((int)(((byte)(70)))));
            this.customButton1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.customButton1.ForeColor = System.Drawing.Color.White;
            this.customButton1.Image = global::RustManager.Data.Icons.Save32W;
            this.customButton1.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.customButton1.Location = new System.Drawing.Point(0, 2);
            this.customButton1.Margin = new System.Windows.Forms.Padding(0);
            this.customButton1.Name = "customButton1";
            this.customButton1.Size = new System.Drawing.Size(261, 40);
            this.customButton1.TabIndex = 4;
            this.customButton1.Text = "Save";
            this.customButton1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.customButton1.UseVisualStyleBackColor = false;
            this.customButton1.Click += new System.EventHandler(this.customButton1_Click);
            // 
            // ProgressPanel2
            // 
            this.ProgressPanel2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ProgressPanel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(35)))), ((int)(((byte)(45)))));
            this.ProgressPanel2.Controls.Add(this.ProgressPanel1);
            this.ProgressPanel2.Location = new System.Drawing.Point(5, 530);
            this.ProgressPanel2.Margin = new System.Windows.Forms.Padding(0);
            this.ProgressPanel2.Name = "ProgressPanel2";
            this.ProgressPanel2.Size = new System.Drawing.Size(790, 15);
            this.ProgressPanel2.TabIndex = 7;
            // 
            // ProgressPanel1
            // 
            this.ProgressPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.ProgressPanel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(70)))), ((int)(((byte)(100)))));
            this.ProgressPanel1.Location = new System.Drawing.Point(0, 0);
            this.ProgressPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.ProgressPanel1.Name = "ProgressPanel1";
            this.ProgressPanel1.Size = new System.Drawing.Size(500, 15);
            this.ProgressPanel1.TabIndex = 7;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(30)))), ((int)(((byte)(40)))));
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Margin = new System.Windows.Forms.Padding(0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(790, 60);
            this.label1.TabIndex = 9;
            this.label1.Text = "LOG";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel2.ColumnCount = 1;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Controls.Add(this.panel8, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel2.Location = new System.Drawing.Point(5, 468);
            this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 2;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 2F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(790, 62);
            this.tableLayoutPanel2.TabIndex = 10;
            // 
            // panel8
            // 
            this.panel8.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel8.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(10)))), ((int)(((byte)(20)))), ((int)(((byte)(30)))));
            this.panel8.Location = new System.Drawing.Point(0, 60);
            this.panel8.Margin = new System.Windows.Forms.Padding(0);
            this.panel8.Name = "panel8";
            this.panel8.Size = new System.Drawing.Size(790, 2);
            this.panel8.TabIndex = 12;
            // 
            // panel9
            // 
            this.panel9.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel9.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(10)))), ((int)(((byte)(20)))), ((int)(((byte)(30)))));
            this.panel9.Location = new System.Drawing.Point(0, 0);
            this.panel9.Margin = new System.Windows.Forms.Padding(0);
            this.panel9.Name = "panel9";
            this.panel9.Size = new System.Drawing.Size(261, 2);
            this.panel9.TabIndex = 14;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(30)))), ((int)(((byte)(40)))));
            this.tableLayoutPanel1.ColumnCount = 5;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 2F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33335F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 2F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.Controls.Add(this.panel14, 3, 1);
            this.tableLayoutPanel1.Controls.Add(this.panel12, 3, 2);
            this.tableLayoutPanel1.Controls.Add(this.panel6, 4, 0);
            this.tableLayoutPanel1.Controls.Add(this.panel3, 3, 0);
            this.tableLayoutPanel1.Controls.Add(this.customButton6, 4, 1);
            this.tableLayoutPanel1.Controls.Add(this.panel13, 2, 2);
            this.tableLayoutPanel1.Controls.Add(this.panel10, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.panel7, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.panel5, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.panel4, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.panel1, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.customButton2, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.panel9, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.customButton1, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.panel11, 3, 2);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(5, 424);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 2F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 2F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(790, 44);
            this.tableLayoutPanel1.TabIndex = 15;
            // 
            // panel14
            // 
            this.panel14.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel14.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(10)))), ((int)(((byte)(20)))), ((int)(((byte)(30)))));
            this.panel14.Location = new System.Drawing.Point(525, 2);
            this.panel14.Margin = new System.Windows.Forms.Padding(0);
            this.panel14.Name = "panel14";
            this.panel14.Size = new System.Drawing.Size(2, 40);
            this.panel14.TabIndex = 29;
            // 
            // panel12
            // 
            this.panel12.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel12.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(10)))), ((int)(((byte)(20)))), ((int)(((byte)(30)))));
            this.panel12.Location = new System.Drawing.Point(527, 42);
            this.panel12.Margin = new System.Windows.Forms.Padding(0);
            this.panel12.Name = "panel12";
            this.panel12.Size = new System.Drawing.Size(263, 2);
            this.panel12.TabIndex = 28;
            // 
            // panel6
            // 
            this.panel6.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(10)))), ((int)(((byte)(20)))), ((int)(((byte)(30)))));
            this.panel6.Location = new System.Drawing.Point(527, 0);
            this.panel6.Margin = new System.Windows.Forms.Padding(0);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(263, 2);
            this.panel6.TabIndex = 26;
            // 
            // panel3
            // 
            this.panel3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(10)))), ((int)(((byte)(20)))), ((int)(((byte)(30)))));
            this.panel3.Location = new System.Drawing.Point(525, 0);
            this.panel3.Margin = new System.Windows.Forms.Padding(0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(2, 2);
            this.panel3.TabIndex = 25;
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
            this.customButton6.Image = global::RustManager.Data.Icons.Restart32W;
            this.customButton6.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.customButton6.Location = new System.Drawing.Point(527, 2);
            this.customButton6.Margin = new System.Windows.Forms.Padding(0);
            this.customButton6.Name = "customButton6";
            this.customButton6.Size = new System.Drawing.Size(263, 40);
            this.customButton6.TabIndex = 19;
            this.customButton6.Text = "Reload";
            this.customButton6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.customButton6.UseVisualStyleBackColor = false;
            this.customButton6.Click += new System.EventHandler(this.customButton6_Click);
            // 
            // panel13
            // 
            this.panel13.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel13.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(10)))), ((int)(((byte)(20)))), ((int)(((byte)(30)))));
            this.panel13.Location = new System.Drawing.Point(263, 42);
            this.panel13.Margin = new System.Windows.Forms.Padding(0);
            this.panel13.Name = "panel13";
            this.panel13.Size = new System.Drawing.Size(262, 2);
            this.panel13.TabIndex = 16;
            // 
            // panel10
            // 
            this.panel10.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel10.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(10)))), ((int)(((byte)(20)))), ((int)(((byte)(30)))));
            this.panel10.Location = new System.Drawing.Point(261, 42);
            this.panel10.Margin = new System.Windows.Forms.Padding(0);
            this.panel10.Name = "panel10";
            this.panel10.Size = new System.Drawing.Size(2, 2);
            this.panel10.TabIndex = 24;
            // 
            // panel7
            // 
            this.panel7.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel7.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(10)))), ((int)(((byte)(20)))), ((int)(((byte)(30)))));
            this.panel7.Location = new System.Drawing.Point(0, 42);
            this.panel7.Margin = new System.Windows.Forms.Padding(0);
            this.panel7.Name = "panel7";
            this.panel7.Size = new System.Drawing.Size(261, 2);
            this.panel7.TabIndex = 23;
            // 
            // panel5
            // 
            this.panel5.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(10)))), ((int)(((byte)(20)))), ((int)(((byte)(30)))));
            this.panel5.Location = new System.Drawing.Point(261, 2);
            this.panel5.Margin = new System.Windows.Forms.Padding(0);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(2, 40);
            this.panel5.TabIndex = 21;
            // 
            // panel4
            // 
            this.panel4.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(10)))), ((int)(((byte)(20)))), ((int)(((byte)(30)))));
            this.panel4.Location = new System.Drawing.Point(263, 0);
            this.panel4.Margin = new System.Windows.Forms.Padding(0);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(262, 2);
            this.panel4.TabIndex = 20;
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(10)))), ((int)(((byte)(20)))), ((int)(((byte)(30)))));
            this.panel1.Location = new System.Drawing.Point(261, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(2, 2);
            this.panel1.TabIndex = 17;
            // 
            // customButton2
            // 
            this.customButton2.BackColor = System.Drawing.Color.Transparent;
            this.customButton2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.customButton2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.customButton2.FlatAppearance.BorderSize = 0;
            this.customButton2.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(70)))), ((int)(((byte)(80)))));
            this.customButton2.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(60)))), ((int)(((byte)(70)))));
            this.customButton2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.customButton2.ForeColor = System.Drawing.Color.White;
            this.customButton2.Image = global::RustManager.Data.Icons.Code32W;
            this.customButton2.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.customButton2.Location = new System.Drawing.Point(263, 2);
            this.customButton2.Margin = new System.Windows.Forms.Padding(0);
            this.customButton2.Name = "customButton2";
            this.customButton2.Size = new System.Drawing.Size(262, 40);
            this.customButton2.TabIndex = 16;
            this.customButton2.Text = "Highlight";
            this.customButton2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.customButton2.UseVisualStyleBackColor = false;
            this.customButton2.Click += new System.EventHandler(this.customButton2_Click);
            // 
            // panel11
            // 
            this.panel11.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel11.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(10)))), ((int)(((byte)(20)))), ((int)(((byte)(30)))));
            this.panel11.Location = new System.Drawing.Point(525, 42);
            this.panel11.Margin = new System.Windows.Forms.Padding(0);
            this.panel11.Name = "panel11";
            this.panel11.Size = new System.Drawing.Size(2, 2);
            this.panel11.TabIndex = 27;
            // 
            // richTextBoxLineNumber
            // 
            this.richTextBoxLineNumber.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(40)))), ((int)(((byte)(50)))));
            this.richTextBoxLineNumber.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.richTextBoxLineNumber.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBoxLineNumber.ForeColor = System.Drawing.Color.White;
            this.richTextBoxLineNumber.Location = new System.Drawing.Point(0, 0);
            this.richTextBoxLineNumber.Margin = new System.Windows.Forms.Padding(0);
            this.richTextBoxLineNumber.Name = "richTextBoxLineNumber";
            this.richTextBoxLineNumber.ReadOnly = true;
            this.richTextBoxLineNumber.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.richTextBoxLineNumber.Size = new System.Drawing.Size(30, 389);
            this.richTextBoxLineNumber.TabIndex = 16;
            this.richTextBoxLineNumber.Text = "";
            this.richTextBoxLineNumber.MouseDown += new System.Windows.Forms.MouseEventHandler(this.richTextBoxLineNumber_MouseDown);
            // 
            // label2
            // 
            this.label2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(40)))), ((int)(((byte)(50)))));
            this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label2.Location = new System.Drawing.Point(0, 0);
            this.label2.Margin = new System.Windows.Forms.Padding(0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(758, 389);
            this.label2.TabIndex = 18;
            this.label2.Text = "Highlighting";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // LoadingTimer
            // 
            this.LoadingTimer.Interval = 1000;
            this.LoadingTimer.Tick += new System.EventHandler(this.LoadingTimer_Tick);
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(10)))), ((int)(((byte)(20)))), ((int)(((byte)(30)))));
            this.tableLayoutPanel3.ColumnCount = 8;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel3.Controls.Add(this.label3, 3, 0);
            this.tableLayoutPanel3.Controls.Add(this.label4, 2, 0);
            this.tableLayoutPanel3.Controls.Add(this.pictureBox1, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.label5, 1, 0);
            this.tableLayoutPanel3.Controls.Add(this.customButton3, 7, 0);
            this.tableLayoutPanel3.Controls.Add(this.customButton4, 6, 0);
            this.tableLayoutPanel3.Controls.Add(this.customButton5, 5, 0);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Top;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel3.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 1;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(800, 35);
            this.tableLayoutPanel3.TabIndex = 19;
            this.tableLayoutPanel3.MouseMove += new System.Windows.Forms.MouseEventHandler(this.tableLayoutPanel3_MouseMove);
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Arial", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label3.Location = new System.Drawing.Point(366, 3);
            this.label3.Margin = new System.Windows.Forms.Padding(0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(68, 28);
            this.label3.TabIndex = 3;
            this.label3.Text = "1.0.0";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label3.MouseMove += new System.Windows.Forms.MouseEventHandler(this.label3_MouseMove);
            // 
            // label4
            // 
            this.label4.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Arial", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label4.Location = new System.Drawing.Point(206, 3);
            this.label4.Margin = new System.Windows.Forms.Padding(0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(160, 28);
            this.label4.TabIndex = 2;
            this.label4.Text = "Syntax Editor";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label4.MouseMove += new System.Windows.Forms.MouseEventHandler(this.label4_MouseMove);
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
            // label5
            // 
            this.label5.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Arial", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label5.Location = new System.Drawing.Point(40, 2);
            this.label5.Margin = new System.Windows.Forms.Padding(0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(166, 30);
            this.label5.TabIndex = 1;
            this.label5.Text = "Infinitynet.dk";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label5.MouseMove += new System.Windows.Forms.MouseEventHandler(this.label5_MouseMove);
            // 
            // customButton3
            // 
            this.customButton3.BackColor = System.Drawing.Color.Transparent;
            this.customButton3.BackgroundImage = global::RustManager.Data.Icons.CloseW;
            this.customButton3.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.customButton3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.customButton3.FlatAppearance.BorderSize = 0;
            this.customButton3.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(70)))), ((int)(((byte)(80)))));
            this.customButton3.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(60)))), ((int)(((byte)(70)))));
            this.customButton3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.customButton3.ForeColor = System.Drawing.Color.White;
            this.customButton3.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.customButton3.Location = new System.Drawing.Point(765, 0);
            this.customButton3.Margin = new System.Windows.Forms.Padding(0);
            this.customButton3.Name = "customButton3";
            this.customButton3.Size = new System.Drawing.Size(35, 35);
            this.customButton3.TabIndex = 2;
            this.customButton3.UseVisualStyleBackColor = false;
            this.customButton3.Click += new System.EventHandler(this.customButton3_Click);
            // 
            // customButton4
            // 
            this.customButton4.BackColor = System.Drawing.Color.Transparent;
            this.customButton4.BackgroundImage = global::RustManager.Data.Icons.MaximiseW;
            this.customButton4.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.customButton4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.customButton4.FlatAppearance.BorderSize = 0;
            this.customButton4.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(70)))), ((int)(((byte)(80)))));
            this.customButton4.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(60)))), ((int)(((byte)(70)))));
            this.customButton4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.customButton4.ForeColor = System.Drawing.Color.White;
            this.customButton4.Image = global::RustManager.Data.Icons.MaximiseW;
            this.customButton4.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.customButton4.Location = new System.Drawing.Point(730, 0);
            this.customButton4.Margin = new System.Windows.Forms.Padding(0);
            this.customButton4.Name = "customButton4";
            this.customButton4.Size = new System.Drawing.Size(35, 35);
            this.customButton4.TabIndex = 1;
            this.customButton4.UseVisualStyleBackColor = false;
            this.customButton4.Click += new System.EventHandler(this.customButton4_Click);
            // 
            // customButton5
            // 
            this.customButton5.BackColor = System.Drawing.Color.Transparent;
            this.customButton5.BackgroundImage = global::RustManager.Data.Icons.MinimizeW;
            this.customButton5.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.customButton5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.customButton5.FlatAppearance.BorderSize = 0;
            this.customButton5.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(70)))), ((int)(((byte)(80)))));
            this.customButton5.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(60)))), ((int)(((byte)(70)))));
            this.customButton5.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.customButton5.ForeColor = System.Drawing.Color.White;
            this.customButton5.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.customButton5.Location = new System.Drawing.Point(695, 0);
            this.customButton5.Margin = new System.Windows.Forms.Padding(0);
            this.customButton5.Name = "customButton5";
            this.customButton5.Size = new System.Drawing.Size(35, 35);
            this.customButton5.TabIndex = 0;
            this.customButton5.UseVisualStyleBackColor = false;
            this.customButton5.Click += new System.EventHandler(this.customButton5_Click);
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel4.ColumnCount = 3;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 2F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel4.Controls.Add(this.panel2, 2, 0);
            this.tableLayoutPanel4.Controls.Add(this.richTextBoxLineNumber, 0, 0);
            this.tableLayoutPanel4.Location = new System.Drawing.Point(5, 35);
            this.tableLayoutPanel4.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 1;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel4.Size = new System.Drawing.Size(790, 389);
            this.tableLayoutPanel4.TabIndex = 20;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.richTextBoxSyntax);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(32, 0);
            this.panel2.Margin = new System.Windows.Forms.Padding(0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(758, 389);
            this.panel2.TabIndex = 21;
            // 
            // DataCheckerTimer
            // 
            this.DataCheckerTimer.Tick += new System.EventHandler(this.DataCheckerTimer_Tick);
            // 
            // EditorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(15F, 28F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(10)))), ((int)(((byte)(20)))), ((int)(((byte)(30)))));
            this.ClientSize = new System.Drawing.Size(800, 550);
            this.Controls.Add(this.tableLayoutPanel4);
            this.Controls.Add(this.tableLayoutPanel3);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.tableLayoutPanel2);
            this.Controls.Add(this.ProgressPanel2);
            this.Font = new System.Drawing.Font("Arial", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(550, 400);
            this.Name = "EditorForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "EditorForm";
            this.Activated += new System.EventHandler(this.EditorForm_Activated);
            this.Deactivate += new System.EventHandler(this.EditorForm_Deactivate);
            this.Load += new System.EventHandler(this.EditorForm_Load);
            this.Move += new System.EventHandler(this.EditorForm_Move);
            this.Resize += new System.EventHandler(this.EditorForm_Resize);
            this.ProgressPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.tableLayoutPanel4.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.RichTextBox richTextBoxSyntax;
        private IRSM.ClassFunctions.CustomButton customButton1;
        private System.Windows.Forms.Panel ProgressPanel2;
        private System.Windows.Forms.Panel ProgressPanel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Panel panel8;
        private System.Windows.Forms.Panel panel9;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel panel13;
        private System.Windows.Forms.Panel panel10;
        private System.Windows.Forms.Panel panel7;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Panel panel1;
        private IRSM.ClassFunctions.CustomButton customButton2;
        private System.Windows.Forms.RichTextBox richTextBoxLineNumber;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Timer LoadingTimer;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label5;
        private IRSM.ClassFunctions.CustomButton customButton3;
        private IRSM.ClassFunctions.CustomButton customButton4;
        private IRSM.ClassFunctions.CustomButton customButton5;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Timer DataCheckerTimer;
        private System.Windows.Forms.Panel panel12;
        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.Panel panel3;
        private IRSM.ClassFunctions.CustomButton customButton6;
        private System.Windows.Forms.Panel panel11;
        private System.Windows.Forms.Panel panel14;
    }
}