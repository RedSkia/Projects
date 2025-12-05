namespace UI.Elements
{
    partial class FormModel
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormModel));
            this.TitleBar = new System.Windows.Forms.TableLayoutPanel();
            this.TitlePicture = new System.Windows.Forms.PictureBox();
            this.CreatorLabel = new System.Windows.Forms.Label();
            this.ProductLabel = new System.Windows.Forms.Label();
            this.VersionLabel = new System.Windows.Forms.Label();
            this.ColorWorker = new System.ComponentModel.BackgroundWorker();
            this.TitleTrayIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.DisplayPanel = new System.Windows.Forms.Panel();
            this.MinimizeButton = new UI.Elements.Button();
            this.MaximizeButton = new UI.Elements.Button();
            this.CloseButton = new UI.Elements.Button();
            this.TitleMenu = new UI.Elements.MenuStrip();
            this.themeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.darkToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lightToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.blueToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.customToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.resetCustomColorsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.updatesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.TitleBar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.TitlePicture)).BeginInit();
            this.TitleMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // TitleBar
            // 
            this.TitleBar.BackColor = System.Drawing.Color.Gray;
            this.TitleBar.ColumnCount = 9;
            this.TitleBar.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.TitleBar.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.TitleBar.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.TitleBar.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.TitleBar.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.TitleBar.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.TitleBar.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 27F));
            this.TitleBar.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 27F));
            this.TitleBar.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 27F));
            this.TitleBar.Controls.Add(this.MinimizeButton, 6, 0);
            this.TitleBar.Controls.Add(this.MaximizeButton, 7, 0);
            this.TitleBar.Controls.Add(this.CloseButton, 8, 0);
            this.TitleBar.Controls.Add(this.TitlePicture, 0, 0);
            this.TitleBar.Controls.Add(this.CreatorLabel, 2, 0);
            this.TitleBar.Controls.Add(this.ProductLabel, 3, 0);
            this.TitleBar.Controls.Add(this.VersionLabel, 4, 0);
            this.TitleBar.Controls.Add(this.TitleMenu, 1, 0);
            this.TitleBar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.TitleBar.Dock = System.Windows.Forms.DockStyle.Top;
            this.TitleBar.Location = new System.Drawing.Point(10, 10);
            this.TitleBar.Margin = new System.Windows.Forms.Padding(0);
            this.TitleBar.Name = "TitleBar";
            this.TitleBar.RowCount = 1;
            this.TitleBar.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.TitleBar.Size = new System.Drawing.Size(1057, 30);
            this.TitleBar.TabIndex = 0;
            this.TitleBar.MouseDown += new System.Windows.Forms.MouseEventHandler(this.TitleBar_MouseDown);
            // 
            // TitlePicture
            // 
            this.TitlePicture.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TitlePicture.Image = global::UI.Properties.Resources.Logo;
            this.TitlePicture.Location = new System.Drawing.Point(0, 0);
            this.TitlePicture.Margin = new System.Windows.Forms.Padding(0);
            this.TitlePicture.Name = "TitlePicture";
            this.TitlePicture.Size = new System.Drawing.Size(40, 30);
            this.TitlePicture.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.TitlePicture.TabIndex = 0;
            this.TitlePicture.TabStop = false;
            this.TitlePicture.MouseDown += new System.Windows.Forms.MouseEventHandler(this.TitlePicture_MouseDown);
            // 
            // CreatorLabel
            // 
            this.CreatorLabel.AutoSize = true;
            this.CreatorLabel.Location = new System.Drawing.Point(196, 2);
            this.CreatorLabel.Margin = new System.Windows.Forms.Padding(0, 2, 0, 0);
            this.CreatorLabel.Name = "CreatorLabel";
            this.CreatorLabel.Size = new System.Drawing.Size(150, 27);
            this.CreatorLabel.TabIndex = 1;
            this.CreatorLabel.Text = "CreatorLabel";
            this.CreatorLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.CreatorLabel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.CreatorLabel_MouseDown);
            // 
            // ProductLabel
            // 
            this.ProductLabel.AutoSize = true;
            this.ProductLabel.Location = new System.Drawing.Point(346, 2);
            this.ProductLabel.Margin = new System.Windows.Forms.Padding(0, 2, 0, 0);
            this.ProductLabel.Name = "ProductLabel";
            this.ProductLabel.Size = new System.Drawing.Size(155, 27);
            this.ProductLabel.TabIndex = 4;
            this.ProductLabel.Text = "ProductLabel";
            this.ProductLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ProductLabel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ProductLabel_MouseDown);
            // 
            // VersionLabel
            // 
            this.VersionLabel.AutoSize = true;
            this.VersionLabel.Location = new System.Drawing.Point(501, 2);
            this.VersionLabel.Margin = new System.Windows.Forms.Padding(0, 2, 0, 0);
            this.VersionLabel.Name = "VersionLabel";
            this.VersionLabel.Size = new System.Drawing.Size(150, 27);
            this.VersionLabel.TabIndex = 5;
            this.VersionLabel.Text = "VersionLabel";
            this.VersionLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.VersionLabel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.VersionLabel_MouseDown);
            // 
            // ColorWorker
            // 
            this.ColorWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.ColorWorker_RunWorkerCompleted);
            // 
            // TitleTrayIcon
            // 
            this.TitleTrayIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("TitleTrayIcon.Icon")));
            this.TitleTrayIcon.MouseClick += new System.Windows.Forms.MouseEventHandler(this.TitleTrayIcon_MouseClick);
            // 
            // DisplayPanel
            // 
            this.DisplayPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DisplayPanel.Location = new System.Drawing.Point(10, 40);
            this.DisplayPanel.Name = "DisplayPanel";
            this.DisplayPanel.Size = new System.Drawing.Size(1057, 450);
            this.DisplayPanel.TabIndex = 1;
            this.DisplayPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.DisplayPanel_Paint);
            // 
            // MinimizeButton
            // 
            this.MinimizeButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.MinimizeButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.MinimizeButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.MinimizeButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MinimizeButton.FlatAppearance.BorderSize = 0;
            this.MinimizeButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(75)))), ((int)(((byte)(75)))));
            this.MinimizeButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.MinimizeButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.MinimizeButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.MinimizeButton.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.MinimizeButton.Location = new System.Drawing.Point(976, 0);
            this.MinimizeButton.Margin = new System.Windows.Forms.Padding(0);
            this.MinimizeButton.Name = "MinimizeButton";
            this.MinimizeButton.Size = new System.Drawing.Size(27, 30);
            this.MinimizeButton.TabIndex = 1;
            this.MinimizeButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.MinimizeButton.UseVisualStyleBackColor = false;
            this.MinimizeButton.Click += new System.EventHandler(this.MinimizeButton_Click);
            // 
            // MaximizeButton
            // 
            this.MaximizeButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.MaximizeButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.MaximizeButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.MaximizeButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MaximizeButton.FlatAppearance.BorderSize = 0;
            this.MaximizeButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(75)))), ((int)(((byte)(75)))));
            this.MaximizeButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.MaximizeButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.MaximizeButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.MaximizeButton.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.MaximizeButton.Location = new System.Drawing.Point(1003, 0);
            this.MaximizeButton.Margin = new System.Windows.Forms.Padding(0);
            this.MaximizeButton.Name = "MaximizeButton";
            this.MaximizeButton.Size = new System.Drawing.Size(27, 30);
            this.MaximizeButton.TabIndex = 2;
            this.MaximizeButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.MaximizeButton.UseVisualStyleBackColor = false;
            this.MaximizeButton.Click += new System.EventHandler(this.MaximizeButton_Click);
            // 
            // CloseButton
            // 
            this.CloseButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.CloseButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.CloseButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.CloseButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.CloseButton.FlatAppearance.BorderSize = 0;
            this.CloseButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(75)))), ((int)(((byte)(75)))));
            this.CloseButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.CloseButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.CloseButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.CloseButton.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.CloseButton.Location = new System.Drawing.Point(1030, 0);
            this.CloseButton.Margin = new System.Windows.Forms.Padding(0);
            this.CloseButton.Name = "CloseButton";
            this.CloseButton.Size = new System.Drawing.Size(27, 30);
            this.CloseButton.TabIndex = 3;
            this.CloseButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.CloseButton.UseVisualStyleBackColor = false;
            this.CloseButton.Click += new System.EventHandler(this.CloseButton_Click);
            // 
            // TitleMenu
            // 
            this.TitleMenu.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(15)))), ((int)(((byte)(15)))));
            this.TitleMenu.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TitleMenu.Font = new System.Drawing.Font("Arial", 10F);
            this.TitleMenu.GripMargin = new System.Windows.Forms.Padding(0);
            this.TitleMenu.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.TitleMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.themeToolStripMenuItem,
            this.updatesToolStripMenuItem});
            this.TitleMenu.Location = new System.Drawing.Point(40, 0);
            this.TitleMenu.Name = "TitleMenu";
            this.TitleMenu.Padding = new System.Windows.Forms.Padding(0);
            this.TitleMenu.Size = new System.Drawing.Size(156, 30);
            this.TitleMenu.TabIndex = 6;
            this.TitleMenu.Text = "menuStrip1";
            this.TitleMenu.MouseDown += new System.Windows.Forms.MouseEventHandler(this.TitleMenu_MouseDown);
            // 
            // themeToolStripMenuItem
            // 
            this.themeToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.darkToolStripMenuItem,
            this.lightToolStripMenuItem,
            this.blueToolStripMenuItem,
            this.customToolStripMenuItem});
            this.themeToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.themeToolStripMenuItem.Name = "themeToolStripMenuItem";
            this.themeToolStripMenuItem.Size = new System.Drawing.Size(72, 30);
            this.themeToolStripMenuItem.Text = "Theme";
            // 
            // darkToolStripMenuItem
            // 
            this.darkToolStripMenuItem.Checked = true;
            this.darkToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.darkToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.darkToolStripMenuItem.Name = "darkToolStripMenuItem";
            this.darkToolStripMenuItem.Size = new System.Drawing.Size(147, 26);
            this.darkToolStripMenuItem.Text = "Dark";
            this.darkToolStripMenuItem.Click += new System.EventHandler(this.darkToolStripMenuItem_Click);
            // 
            // lightToolStripMenuItem
            // 
            this.lightToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.lightToolStripMenuItem.Name = "lightToolStripMenuItem";
            this.lightToolStripMenuItem.Size = new System.Drawing.Size(147, 26);
            this.lightToolStripMenuItem.Text = "Light";
            this.lightToolStripMenuItem.Click += new System.EventHandler(this.lightToolStripMenuItem_Click);
            // 
            // blueToolStripMenuItem
            // 
            this.blueToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.blueToolStripMenuItem.Name = "blueToolStripMenuItem";
            this.blueToolStripMenuItem.Size = new System.Drawing.Size(147, 26);
            this.blueToolStripMenuItem.Text = "Blue";
            this.blueToolStripMenuItem.Click += new System.EventHandler(this.blueToolStripMenuItem_Click);
            // 
            // customToolStripMenuItem
            // 
            this.customToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.editToolStripMenuItem,
            this.resetCustomColorsToolStripMenuItem});
            this.customToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.customToolStripMenuItem.Name = "customToolStripMenuItem";
            this.customToolStripMenuItem.Size = new System.Drawing.Size(147, 26);
            this.customToolStripMenuItem.Text = "Custom";
            this.customToolStripMenuItem.Click += new System.EventHandler(this.customToolStripMenuItem_Click);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.editToolStripMenuItem.Image = global::UI.Properties.Resources.Edit;
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(245, 26);
            this.editToolStripMenuItem.Text = "Edit";
            this.editToolStripMenuItem.Click += new System.EventHandler(this.editToolStripMenuItem_Click);
            // 
            // resetCustomColorsToolStripMenuItem
            // 
            this.resetCustomColorsToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.resetCustomColorsToolStripMenuItem.Image = global::UI.Properties.Resources.Undo;
            this.resetCustomColorsToolStripMenuItem.Name = "resetCustomColorsToolStripMenuItem";
            this.resetCustomColorsToolStripMenuItem.Size = new System.Drawing.Size(245, 26);
            this.resetCustomColorsToolStripMenuItem.Text = "Reset Custom Colors";
            this.resetCustomColorsToolStripMenuItem.Click += new System.EventHandler(this.resetCustomColorsToolStripMenuItem_Click);
            // 
            // updatesToolStripMenuItem
            // 
            this.updatesToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.updatesToolStripMenuItem.Name = "updatesToolStripMenuItem";
            this.updatesToolStripMenuItem.Size = new System.Drawing.Size(82, 30);
            this.updatesToolStripMenuItem.Text = "Updates";
            this.updatesToolStripMenuItem.Click += new System.EventHandler(this.updatesToolStripMenuItem_Click);
            // 
            // FormModel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(13F, 26F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1077, 500);
            this.Controls.Add(this.DisplayPanel);
            this.Controls.Add(this.TitleBar);
            this.Font = new System.Drawing.Font("Arial", 14F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(5);
            this.Name = "FormModel";
            this.Padding = new System.Windows.Forms.Padding(10);
            this.Text = "FormModel";
            this.Activated += new System.EventHandler(this.FormModel_Activated);
            this.Deactivate += new System.EventHandler(this.FormModel_Deactivate);
            this.Load += new System.EventHandler(this.FormModel_Load);
            this.LocationChanged += new System.EventHandler(this.FormModel_LocationChanged);
            this.SizeChanged += new System.EventHandler(this.FormModel_SizeChanged);
            this.TitleBar.ResumeLayout(false);
            this.TitleBar.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.TitlePicture)).EndInit();
            this.TitleMenu.ResumeLayout(false);
            this.TitleMenu.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel TitleBar;
        private System.ComponentModel.BackgroundWorker ColorWorker;
        private System.Windows.Forms.PictureBox TitlePicture;
        private System.Windows.Forms.Label CreatorLabel;
        private Button MinimizeButton;
        private Button MaximizeButton;
        private Button CloseButton;
        private System.Windows.Forms.Label VersionLabel;
        private System.Windows.Forms.Label ProductLabel;
        private MenuStrip TitleMenu;
        private System.Windows.Forms.ToolStripMenuItem themeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem darkToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem lightToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem blueToolStripMenuItem;
        private System.Windows.Forms.NotifyIcon TitleTrayIcon;
        private System.Windows.Forms.ToolStripMenuItem customToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.Panel DisplayPanel;
        private System.Windows.Forms.ToolStripMenuItem resetCustomColorsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem updatesToolStripMenuItem;
    }
}