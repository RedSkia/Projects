using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UI.Elements
{
    public partial class FormModel : Form
    {
        #region Settings
        private const string creator = "Infinitynet.dk/Xray";
        private const string product = "Rust Server Manager";
        private const string version = "1.0.0";
        #endregion Settings
        #region Init
        public static FormModel Instance { get; set; }
        public FormModel()
        {
            InitializeComponent();
            Instance = this;
            Icon = Properties.Resources.IcoLogo;
            TitlePicture.Image = Properties.Resources.Logo;
            StartPosition = FormStartPosition.CenterScreen;
            ProcessIcons();
        }
        private async void FormModel_Load(object sender, EventArgs e)
        {
            TitleMenuSetColorCheckbox();

            UI.Helpers.Color.File.Load();

            FormBorderStyle = FormBorderStyle.None;
            if (ColorWorker.IsBusy != true) { ColorWorker.RunWorkerAsync(); }

            CreatorLabel.Text = creator;
            ProductLabel.Text = product;
            VersionLabel.Text = version;

            var titleBar = TitleBar.Height;
            var titleMenu = 0;
            foreach (var i in TitleMenu.Items) { var x = (ToolStripMenuItem)i; titleMenu += x.Width + 10; }
            var titlePicture = TitlePicture.Width;
            var titleLabel = CreatorLabel.Width + ProductLabel.Width + VersionLabel.Width;
            var button = CloseButton.Width + MaximizeButton.Width + MinimizeButton.Width;
            var elements = titlePicture + titleMenu + titleLabel + button * 2;
            var padding = 10;
            MinimumSize = new Size(elements, titleBar + padding + 5);

            FixTitleBarUI(true);
            await Task.Delay(100);
            FixTitleBarUI(true);
        }
        #endregion Init

        #region Form Functions
        /// <summary>
        /// Adds <paramref name="control"/> To <see cref="DisplayPanel"/>
        /// </summary>
        /// <param name="control"></param>
        public void SetDisplayPanel(Control control)
        {
            foreach (Control c in DisplayPanel.Controls) { c.Dispose(); }
            control.Dock = DockStyle.Fill;
            DisplayPanel.Controls.Add(control);
        }

        /// <summary>
        /// Class For Managing Loading Panel
        /// </summary>
        public class Loading
        {
            /// <summary>
            /// Updates Test Of <see cref="LoadingLabel"/>
            /// </summary>
            public static void UpdateLabel(Label label, string text) => label.Text = text;

            /// <summary>
            /// Brings up a <see cref="Panel"/> containing a <see cref="Label"/> with <paramref name="text"/>
            /// </summary>
            /// <returns>The <see cref="Panel"/></returns>
            public static Panel Start(string text)
            {
                Panel panel = new Panel();
                panel.Dock = DockStyle.Fill;
                panel.BackColor = UI.Helpers.Color.Background;
                Instance.Controls.Add(panel);
                panel.BringToFront();
 
                Label label = new Label();
                label.Name = "label";
                label.Text = text;
                label.Dock = DockStyle.Fill;
                label.TextAlign = ContentAlignment.MiddleCenter;
                label.BringToFront();
                label.Font = new Font(new FontFamily("Arial"), 30, FontStyle.Regular, GraphicsUnit.Pixel);
                label.BackColor = UI.Helpers.Color.Background;
                label.ForeColor = UI.Helpers.Color.Text;
                panel.Controls.Add(label);

                return panel;
            }

            /// <summary>
            /// Hides <see cref="LoadingPanel"/> with fadeing effect
            /// </summary>
            public static async void Stop(Panel panel)
            {
                int fadingSpeed = 10;
                var label = panel.Controls.Find("label", true)[0];
                do
                {
                    await Task.Delay(1);
                    label.ForeColor = UI.Helpers.Color.Set(label.ForeColor.R - fadingSpeed, label.ForeColor.G - fadingSpeed, label.ForeColor.B - fadingSpeed);
                    if (label.ForeColor == UI.Helpers.Color.Background) { panel.Dispose(); }
                }
                while (label.ForeColor != UI.Helpers.Color.Background);
            }
        }


        #endregion Form Functions


        /// <summary>
        /// Processing Default Icons (Black White)
        /// </summary>
        private async void ProcessIcons()
        {
            //Load From Disk
            LIB.Helpers.BitmapConverter.LoadImages();

            //Process Images
            LIB.Helpers.BitmapConverter.ProcessRawImage("closeB", Properties.Resources.Close);
            LIB.Helpers.BitmapConverter.ProcessRawImage("maximiseB", Properties.Resources.Maximise);
            LIB.Helpers.BitmapConverter.ProcessRawImage("maximizedB", Properties.Resources.Maximized);
            LIB.Helpers.BitmapConverter.ProcessRawImage("minimizeB", Properties.Resources.Minimize);
            LIB.Helpers.BitmapConverter.ProcessRawImage("editB", Properties.Resources.Edit);
            LIB.Helpers.BitmapConverter.ProcessRawImage("closeB", Properties.Resources.Close);
            LIB.Helpers.BitmapConverter.ProcessRawImage("undoB", Properties.Resources.Undo);
            LIB.Helpers.BitmapConverter.ProcessRawImage("checkmarkB", Properties.Resources.Checkmark);


            //Process White Images
            foreach (var i in LIB.Helpers.BitmapConverter.ImageDB.Get().Where(x => x.Key.EndsWith("B")).ToArray())
            {
                await LIB.Helpers.BitmapConverter.ProcessImage(new LIB.Helpers.BitmapConverter.ImageData
                {
                    bitmap = i.Value,
                    color = Color.White,
                    name = i.Key.Replace("B", "W")
                });
            }

            //Process Custom Icons
            await UI.Helpers.IconHandler.UpdateCustomImages(UI.Helpers.Color.Style.customModeRef.Icon);



            LIB.Helpers.BitmapConverter.SaveImages();
            

        }


        private Size formSize; //Keep form size when it is minimized and restored.Since the form is resized because it takes into account the size of the title bar and borders.
        //Overridden methods (CAREFUL)
        protected override void WndProc(ref Message m)
        {
            const int WM_NCCALCSIZE = 0x0083;//Standar Title Bar - Snap Window
            const int WM_SYSCOMMAND = 0x0112;
            const int SC_MINIMIZE = 0xF020; //Minimize form (Before)
            const int SC_RESTORE = 0xF120; //Restore form (Before)
            const int WM_NCHITTEST = 0x0084;//Win32, Mouse Input Notification: Determine what part of the window corresponds to a point, allows to resize the form.
            const int resizeAreaSize = 10;

            #region Form Resize
            // Resize/WM_NCHITTEST values
            const int HTCLIENT = 1; //Represents the client area of the window
            const int HTLEFT = 10;  //Left border of a window, allows resize horizontally to the left
            const int HTRIGHT = 11; //Right border of a window, allows resize horizontally to the right
            const int HTTOP = 12;   //Upper-horizontal border of a window, allows resize vertically up
            const int HTTOPLEFT = 13;//Upper-left corner of a window border, allows resize diagonally to the left
            const int HTTOPRIGHT = 14;//Upper-right corner of a window border, allows resize diagonally to the right
            const int HTBOTTOM = 15; //Lower-horizontal border of a window, allows resize vertically down
            const int HTBOTTOMLEFT = 16;//Lower-left corner of a window border, allows resize diagonally to the left
            const int HTBOTTOMRIGHT = 17;//Lower-right corner of a window border, allows resize diagonally to the right

            ///<Doc> More Information: https://docs.microsoft.com/en-us/windows/win32/inputdev/wm-nchittest </Doc>

            if (m.Msg == WM_NCHITTEST)
            { //If the windows m is WM_NCHITTEST
                base.WndProc(ref m);
                if (this.WindowState == FormWindowState.Normal)//Resize the form if it is in normal state
                {
                    if ((int)m.Result == HTCLIENT)//If the result of the m (mouse pointer) is in the client area of the window
                    {
                        Point screenPoint = new Point(m.LParam.ToInt32()); //Gets screen point coordinates(X and Y coordinate of the pointer)                           
                        Point clientPoint = this.PointToClient(screenPoint); //Computes the location of the screen point into client coordinates                          

                        if (clientPoint.Y <= resizeAreaSize)//If the pointer is at the top of the form (within the resize area- X coordinate)
                        {
                            if (clientPoint.X <= resizeAreaSize) //If the pointer is at the coordinate X=0 or less than the resizing area(X=10) in 
                                m.Result = (IntPtr)HTTOPLEFT; //Resize diagonally to the left
                            else if (clientPoint.X < (this.Size.Width - resizeAreaSize))//If the pointer is at the coordinate X=11 or less than the width of the form(X=Form.Width-resizeArea)
                                m.Result = (IntPtr)HTTOP; //Resize vertically up
                            else //Resize diagonally to the right
                                m.Result = (IntPtr)HTTOPRIGHT;
                        }
                        else if (clientPoint.Y <= (this.Size.Height - resizeAreaSize)) //If the pointer is inside the form at the Y coordinate(discounting the resize area size)
                        {
                            if (clientPoint.X <= resizeAreaSize)//Resize horizontally to the left
                                m.Result = (IntPtr)HTLEFT;
                            else if (clientPoint.X > (this.Width - resizeAreaSize))//Resize horizontally to the right
                                m.Result = (IntPtr)HTRIGHT;
                        }
                        else
                        {
                            if (clientPoint.X <= resizeAreaSize)//Resize diagonally to the left
                                m.Result = (IntPtr)HTBOTTOMLEFT;
                            else if (clientPoint.X < (this.Size.Width - resizeAreaSize)) //Resize vertically down
                                m.Result = (IntPtr)HTBOTTOM;
                            else //Resize diagonally to the right
                                m.Result = (IntPtr)HTBOTTOMRIGHT;
                        }
                    }
                }
                return;
            }
            #endregion

            //Remove border and keep snap window (Custom)
            if (m.Msg == WM_NCCALCSIZE) { m.Result = IntPtr.Zero; return; }


            //Keep form size when it is minimized and restored. Since the form is resized because it takes into account the size of the title bar and borders.
            if (m.Msg == WM_SYSCOMMAND)
            {
                /// <see cref="https://docs.microsoft.com/en-us/windows/win32/menurc/wm-syscommand"/>
                /// Quote:
                /// In WM_SYSCOMMAND messages, the four low - order bits of the wParam parameter 
                /// are used internally by the system.To obtain the correct result when testing 
                /// the value of wParam, an application must combine the value 0xFFF0 with the 
                /// wParam value by using the bitwise AND operator.
                int wParam = (m.WParam.ToInt32() & 0xFFF0);

                if (wParam == SC_MINIMIZE)  //Before
                    formSize = this.ClientSize;
                if (wParam == SC_RESTORE)// Restored form(Before)
                    this.Size = formSize;
            }
            base.WndProc(ref m);
        }




        //Windoes Event Handlers
        #region Drag
        private async void StartDrag()
        {
            await Task.Delay(1);
            Invalidate(); //Fixes Custom TitleBar Flashing
            await Task.Delay(1);
            LIB.Windows.Drag.Release();
            await Task.Delay(1);
            LIB.Windows.Drag.DragHandler(this);
        }
        private void TitleBar_MouseDown(object sender, MouseEventArgs e) => StartDrag();
        private void TitlePicture_MouseDown(object sender, MouseEventArgs e) => StartDrag();
        private void CreatorLabel_MouseDown(object sender, MouseEventArgs e) => StartDrag();
        private void ProductLabel_MouseDown(object sender, MouseEventArgs e) => StartDrag();
        private void VersionLabel_MouseDown(object sender, MouseEventArgs e) => StartDrag();
        private void TitleMenu_MouseDown(object sender, MouseEventArgs e) => StartDrag();
        #endregion Drag
        #region AreoSnap
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.Style |= 0x40000; //WS_SIZEBOX;
                return cp;
            }
        }
        #endregion AreoSnap


        private bool IsDocked { get; set; }
        private async void FixTitleBarUI(bool Force)
        {
            bool Left = Location.X <= 0 && Width == Screen.PrimaryScreen.WorkingArea.Width / 2;
            bool Right = Location.X >= Screen.PrimaryScreen.WorkingArea.Width - Width && Width == Screen.PrimaryScreen.WorkingArea.Width / 2;

            if (Force)
            {
                Invalidate();
                return;
            }
            if (IsDocked)
            {
                Invalidate();
                IsDocked = false;
            }
            else if (Left || Right)
            {
                await Task.Delay(10);
                Invalidate();
                IsDocked = true;
            }
        }

        //Fixes TitleBar Comes Bace On Deactivate
        private void FormModel_SizeChanged(object sender, EventArgs e) => FixTitleBarUI(true);
        private void FormModel_LocationChanged(object sender, EventArgs e)
        {
            FixTitleBarUI(false);
            if (WindowState == FormWindowState.Normal)
            {
                switch (UI.Helpers.Color.imageColor)
                {
                    case UI.Helpers.Color.ImageColor.black:
                        MaximizeButton.BackgroundImage = Properties.Resources.Maximise;
                        break;
                    case UI.Helpers.Color.ImageColor.white:
                        //MaximizeButton.BackgroundImage = Properties.Resources.MaximiseW;
                        break;
                }
            }
        }

        private void FormModel_Deactivate(object sender, EventArgs e) => FixTitleBarUI(true);
        private void FormModel_Activated(object sender, EventArgs e) => FixTitleBarUI(true);




        #region Background Workers
        private const int WorkerDelaySlow = 5000;
        private const int WorkerDelayNormal = 1000;
        private const int WorkerDelayFast = 100;
        private UI.Helpers.Color.Theme oldTheme { get; set; }
        private async void ColorWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            await Task.Delay(WorkerDelayFast);

            TitleBar.BackColor = UI.Helpers.Color.Background;
            TitleMenu.BackColor = UI.Helpers.Color.Background;
            UI.Helpers.Color.UpdateControl(CreatorLabel); CreatorLabel.ForeColor = UI.Helpers.Color.Text;
            UI.Helpers.Color.UpdateControl(ProductLabel); ProductLabel.ForeColor = UI.Helpers.Color.Text;
            UI.Helpers.Color.UpdateControl(VersionLabel); VersionLabel.ForeColor = UI.Helpers.Color.Text;
            UI.Helpers.Color.UpdateControl(MinimizeButton); MinimizeButton.BackColor = UI.Helpers.Color.Background;
            UI.Helpers.Color.UpdateControl(MaximizeButton); MaximizeButton.BackColor = UI.Helpers.Color.Background;
            UI.Helpers.Color.UpdateControl(CloseButton); CloseButton.BackColor = UI.Helpers.Color.Background;
            TitleMenuSetColorCheckbox();
            try
            {
                switch (UI.Helpers.Color.imageColor)
                {
                    case UI.Helpers.Color.ImageColor.black:
                        CloseButton.BackgroundImage = LIB.Helpers.BitmapConverter.GetImage("closeB");
                        MaximizeButton.BackgroundImage = WindowState == FormWindowState.Normal ? LIB.Helpers.BitmapConverter.GetImage("maximiseB") : LIB.Helpers.BitmapConverter.GetImage("maximizedB");
                        MinimizeButton.BackgroundImage = LIB.Helpers.BitmapConverter.GetImage("minimizeB");
                        break;
                    case UI.Helpers.Color.ImageColor.white:
                        CloseButton.BackgroundImage = LIB.Helpers.BitmapConverter.GetImage("closeW");
                        MaximizeButton.BackgroundImage = WindowState == FormWindowState.Normal ? LIB.Helpers.BitmapConverter.GetImage("maximiseW") : LIB.Helpers.BitmapConverter.GetImage("maximizedW");
                        MinimizeButton.BackgroundImage = LIB.Helpers.BitmapConverter.GetImage("minimizeW");
                        break;
                    case UI.Helpers.Color.ImageColor.custom:
                        CloseButton.BackgroundImage = LIB.Helpers.BitmapConverter.GetImage("closeC");
                        MaximizeButton.BackgroundImage = WindowState == FormWindowState.Normal ? LIB.Helpers.BitmapConverter.GetImage("maximiseC") : LIB.Helpers.BitmapConverter.GetImage("maximizedC");
                        MinimizeButton.BackgroundImage = LIB.Helpers.BitmapConverter.GetImage("minimizeC");
                        break;
                }
            } catch { }


            //Force Redraw UI On Theme Change
            if (!oldTheme.Equals(UI.Helpers.Color.theme))
            {
                Update();
                Refresh();
                Invalidate();
                oldTheme = UI.Helpers.Color.theme;
            }

            if (ColorWorker.IsBusy != true && !ColorWorker.CancellationPending) { ColorWorker.RunWorkerAsync(); }
        }
        #endregion Background Workers

        #region TitleMenu Handler
        /// <summary>
        /// Sets checkbox of <see cref="TitleMenu"/> relative to selected <see cref="theme"/>
        /// </summary>
        private void TitleMenuSetColorCheckbox()
        {
            //UnCheck All
            foreach(ToolStripMenuItem item in TitleMenu.Items){
                foreach (ToolStripMenuItem subItem in item.DropDownItems){
                    subItem.Checked = false;
                }
            }
     
            switch (UI.Helpers.Color.theme)
            {
                case UI.Helpers.Color.Theme.dark:
                    darkToolStripMenuItem.Checked = true;
                    break;
                case UI.Helpers.Color.Theme.light:
                    lightToolStripMenuItem.Checked = true;
                    break;
                case UI.Helpers.Color.Theme.blue:
                    blueToolStripMenuItem.Checked = true;
                    break;
                case UI.Helpers.Color.Theme.custom:
                    customToolStripMenuItem.Checked = true;
                    break;
            }
        }

        private void darkToolStripMenuItem_Click(object sender, EventArgs e) 
        { 
            UI.Helpers.Color.theme = UI.Helpers.Color.Theme.dark; 
            UI.Helpers.Color.lastTheme = UI.Helpers.Color.Theme.dark; 
            TitleMenuSetColorCheckbox(); 
            UI.Helpers.Color.File.Save();
        }
        private void lightToolStripMenuItem_Click(object sender, EventArgs e) 
        { 
            UI.Helpers.Color.theme = UI.Helpers.Color.Theme.light; 
            UI.Helpers.Color.lastTheme = UI.Helpers.Color.Theme.light; 
            TitleMenuSetColorCheckbox();
            UI.Helpers.Color.File.Save();
        }
        private void blueToolStripMenuItem_Click(object sender, EventArgs e) 
        { 
            UI.Helpers.Color.theme = UI.Helpers.Color.Theme.blue; 
            UI.Helpers.Color.lastTheme = UI.Helpers.Color.Theme.blue; 
            TitleMenuSetColorCheckbox();
            UI.Helpers.Color.File.Save();
        }
        private void customToolStripMenuItem_Click(object sender, EventArgs e) 
        {
            UI.Helpers.Color.theme = UI.Helpers.Color.Theme.custom;
            TitleMenuSetColorCheckbox();
            UI.Helpers.Color.File.Save();
        }
        private void editToolStripMenuItem_Click(object sender, EventArgs e) => SetDisplayPanel(new UserControls.CustomColorPicker());
        #endregion TitleMenu Handler
        #region Button Handler
        private void MaximizeButton_Click(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Normal)
            {
                switch (UI.Helpers.Color.imageColor)
                {
                    case UI.Helpers.Color.ImageColor.black:
                        MaximizeButton.BackgroundImage = Properties.Resources.Maximized;
                        break;
                    case UI.Helpers.Color.ImageColor.white:
                        //MaximizeButton.BackgroundImage = Properties.Resources.MaximizedW;
                        break;
                }
                WindowState = FormWindowState.Maximized;
            }
            else if (WindowState == FormWindowState.Maximized)
            {
                switch (UI.Helpers.Color.imageColor)
                {
                    case UI.Helpers.Color.ImageColor.black:
                        MaximizeButton.BackgroundImage = Properties.Resources.Maximise;
                        break;
                    case UI.Helpers.Color.ImageColor.white:
                        //MaximizeButton.BackgroundImage = Properties.Resources.MaximiseW;
                        break;
                }
                WindowState = FormWindowState.Normal;
            }
        }
        private void CloseButton_Click(object sender, EventArgs e) => Application.Exit();
        private void MinimizeButton_Click(object sender, EventArgs e) => WindowState = FormWindowState.Minimized;
        #endregion Button Handler


        #region UI Constant Stuff
        //Outline From Border
        protected override void OnPaint(PaintEventArgs e)
        {
            Pen pen = new Pen(UI.Helpers.Color.Selected, 2);
            Rectangle rect = new Rectangle(0, 0, this.Width, this.Height);
            e.Graphics.DrawRectangle(pen, rect);
        }

        //TrayIcon
        private void TitleTrayIcon_MouseClick(object sender, MouseEventArgs e)
        {
            Activate();
            formSize = Size;
            if (WindowState == FormWindowState.Minimized)
            {
                WindowState = FormWindowState.Normal; 
                Size = formSize;
            }
        }
        #endregion UI Constant Stuff

        private void DisplayPanel_Paint(object sender, PaintEventArgs e)
        {

        }

        private void resetCustomColorsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UI.Helpers.Color.Style.ResetCustomMode();
        }

        int siwticher = 0;
        private void timer1_Tick(object sender, EventArgs e)
        {
            siwticher++;
            switch(siwticher)
            {
                case 1:
                    UI.Helpers.Color.theme = Helpers.Color.Theme.blue;
                    break;
                case 2:
                    UI.Helpers.Color.theme = Helpers.Color.Theme.light;
                    break;
                case 3:
                    UI.Helpers.Color.theme = Helpers.Color.Theme.dark;
                    break;
            }

 
            TitleMenuSetColorCheckbox();
            UI.Helpers.Color.File.Save();

            if (siwticher > 3)
            {
                siwticher = 0;
            }
        }

        private void updatesToolStripMenuItem_Click(object sender, EventArgs e) => SetDisplayPanel(new UserControls.UpdateLog());

    }
}