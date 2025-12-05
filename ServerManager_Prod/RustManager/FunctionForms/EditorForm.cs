using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RustManager.FunctionForms
{
    public partial class EditorForm : Form
    {

        string GlobalFilePath;
        string GlobalFileName;
        public EditorForm(string FilePath, string FileName)
        {
            InitializeComponent();

            try
            {
                GlobalFilePath = FilePath;
                GlobalFileName = FileName;

                CustomLoad();
            } catch { }
        }

        private void EditorForm_Load(object sender, EventArgs e)
        {
            ProgressPanel1.Width = 0;
            label1.Text = "";
            richTextBoxLineNumber.Font = richTextBoxSyntax.Font;
            AddLineNumbers();

        }


        void CustomLoad()
        {
            try
            {
                System.IO.StreamReader sr = new System.IO.StreamReader(GlobalFilePath);
                richTextBoxSyntax.Text = sr.ReadToEnd();
                HighlightSyntax();
                Text = $"{GlobalFileName}";
                sr.Close();
            }
            catch { }
        }

        #region Drag
        #region Import DLL
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool ReleaseCapture();
        #endregion

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            //Correct Drag Function
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void label5_MouseMove(object sender, MouseEventArgs e)
        {
            //Correct Drag Function
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void label4_MouseMove(object sender, MouseEventArgs e)
        {
            //Correct Drag Function
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void label3_MouseMove(object sender, MouseEventArgs e)
        {
            //Correct Drag Function
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void tableLayoutPanel3_MouseMove(object sender, MouseEventArgs e)
        {
            //Correct Drag Function
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }
        #endregion


        #region Overlay
        void Overlay(Form SeletedForm)
        {
            Form Overlay = new Form();
            try
            {

                Overlay.StartPosition = FormStartPosition.Manual;
                Overlay.FormBorderStyle = FormBorderStyle.None;
                Overlay.Opacity = 0.75;
                Overlay.BackColor = Color.Black;
                Overlay.Size = Size;
                Overlay.Location = Location;
                Overlay.ShowIcon = false;
                Overlay.ShowInTaskbar = false;
                Overlay.Show();


                SeletedForm.ShowInTaskbar = false;
                //SeletedForm.Width = Data.AppInfo.Default.EditorSize.Width - 200;
                //SeletedForm.Height = Data.AppInfo.Default.AppSize.Height - 200;
                SeletedForm.StartPosition = FormStartPosition.CenterParent;
                SeletedForm.TopMost = true;
                SeletedForm.Owner = Overlay;
                SeletedForm.ShowDialog();
                Overlay.Dispose();

            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
            finally { Overlay.Dispose(); }
        }
        #endregion


        #region ProcessBar
        Random random = new Random();
        int ProgressBarPercentage = 0;
        async void SetProgressBarValue()
        {
            while (true)
            {
                await Task.Delay(10);
                ProgressPanel1.Dock = DockStyle.None;
                ProgressPanel1.Width = Convert.ToInt32(ProgressPanel2.Width * ProgressBarPercentage / 100);


                if (ProgressBarPercentage >= 100)
                {
                    ProgressPanel1.Dock = DockStyle.Fill;
                    break;
                }
                else if (ProgressBarPercentage < -1)
                {
                    break;
                }
            }
        }
        #endregion


        #region Line Number

        public int getWidth()
        {
            
            
            int w = 0;
            // get total lines of richTextBox1    
            int line = richTextBoxSyntax.Lines.Length;

            if (line <= 9)
            {
                w = 20 + (int)richTextBoxSyntax.Font.Size;
            }
            else if (line <= 99)
            {
                w = 35 + (int)richTextBoxSyntax.Font.Size;
            }
            else if (line <= 999)
            {
                w = 50 + (int)richTextBoxSyntax.Font.Size;
            }
            else if (line <= 9999)
            {
                w = 65 + (int)richTextBoxSyntax.Font.Size;
            }
            else if (line <= 99999)
            {
                w = 80 + (int)richTextBoxSyntax.Font.Size;
            }
            else if (line <= 999999)
            {
                w = 95 + (int)richTextBoxSyntax.Font.Size;
            }
            else if (line <= 9999999)
            {
                w = 110 + (int)richTextBoxSyntax.Font.Size;
            }
            else if (line <= 99999999)
            {
                w = 125 + (int)richTextBoxSyntax.Font.Size;
            }
            else if (line <= 999999999)
            {
                w = 140 + (int)richTextBoxSyntax.Font.Size;
            }


            return w;
            
        }

        public void AddLineNumbers()
        {
            // create & set Point pt to (0,0)    
            Point pt = new Point(0, 0);
            // get First Index & First Line from richTextBoxSyntax    
            int First_Index = richTextBoxSyntax.GetCharIndexFromPosition(pt);
            int First_Line = richTextBoxSyntax.GetLineFromCharIndex(First_Index);
            // set X & Y coordinates of Point pt to ClientRectangle Width & Height respectively    
            pt.X = ClientRectangle.Width;
            pt.Y = ClientRectangle.Height;
            // get Last Index & Last Line from richTextBoxSyntax    
            int Last_Index = richTextBoxSyntax.GetCharIndexFromPosition(pt);
            int Last_Line = richTextBoxSyntax.GetLineFromCharIndex(Last_Index);
            // set Center alignment to LineNumberTextBox    
            richTextBoxLineNumber.SelectionAlignment = HorizontalAlignment.Center;
            // set LineNumberTextBox text to null & width to getWidth() function value    
            richTextBoxLineNumber.Text = "";
            richTextBoxLineNumber.Width = getWidth();
            // now add each line number to LineNumberTextBox upto last line    
            for (int i = First_Line; i <= Last_Line + 0; i++)
            {
                richTextBoxLineNumber.Text += i + 1 + "\n";
            }
        }





        #region Event Calls
        private void richTextBoxSyntax_SelectionChanged(object sender, EventArgs e)
        {
            if (!backgroundWorkerWORKING)
            {
                Point pt = richTextBoxSyntax.GetPositionFromCharIndex(richTextBoxSyntax.SelectionStart);
                if (pt.X == 1)
                {
                    AddLineNumbers();
                }
            }
        }

        private void richTextBoxSyntax_VScroll(object sender, EventArgs e)
        {
            if (!backgroundWorkerWORKING)
            {
                richTextBoxLineNumber.Text = "";
                AddLineNumbers();
                richTextBoxLineNumber.Invalidate();
            }
        }

        private void richTextBoxSyntax_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if(!backgroundWorkerWORKING)
                {
                    AddLineNumbers();
                }
            } catch { }
          
        }

        private void richTextBoxSyntax_FontChanged(object sender, EventArgs e)
        {
            if (!backgroundWorkerWORKING)
            {
                richTextBoxLineNumber.Font = richTextBoxSyntax.Font;
                richTextBoxSyntax.Select();
                AddLineNumbers();
            }
        }

        private void richTextBoxLineNumber_MouseDown(object sender, MouseEventArgs e)
        {
            richTextBoxSyntax.Select();
            richTextBoxLineNumber.DeselectAll();
        }


        #endregion


        #endregion


        #region Main Buttons
        bool SaveingFile = false;
        private void customButton1_Click(object sender, EventArgs e)
        {
            if (!SaveingFile)
            {
                SaveingFile = true;

                richTextBoxSyntax.SaveFile(GlobalFilePath, RichTextBoxStreamType.PlainText);

                LabelFadeOut(label1, "File Saved", 20, 30, 40);

                SaveingFile = false;
            }



        }

        bool LabelFadeing = false;
        async void LabelFadeOut(Control TargetControl, string Text, int TargetRColor, int TargetGColor, int TargetBColor)
        {
            if (!LabelFadeing)
            {
                LabelFadeing = true;


                TargetControl.ForeColor = Color.White;
                TargetControl.Text = Text;
                int RColor = 255;
                int GColor = 255;
                int BColor = 255;

                await Task.Delay(1000);

                int FadeingSpeed = 3;
                while (true)
                {
                    await Task.Delay(1);


                    RColor -= FadeingSpeed;
                    GColor -= FadeingSpeed;
                    BColor -= FadeingSpeed;
                    label1.ForeColor = Color.FromArgb(RColor, GColor, BColor);


                    if (RColor == TargetRColor || GColor == TargetGColor || BColor == TargetBColor)
                    {
                        TargetControl.ForeColor = Color.FromArgb(TargetRColor, TargetGColor, TargetBColor);
                        break;
                    }

                }

                LabelFadeing = false;
            }
           
  
        }


        private void customButton5_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        bool Highlighting = false;
        void HighlightSyntax()
        {
            if(!Highlighting)
            {
                Highlighting = true;
                if (backgroundWorker1.IsBusy != true)
                {
                    label1.ForeColor = Color.White;
                    HidePanel();
                    LoadingTimer.Start();
                    ProgressBarPercentage = 0; SetProgressBarValue(); label1.Text = "Preparing Syntax Highlight";


                    backgroundWorker1.RunWorkerAsync();
                }
                Highlighting = false;
            }
  
        }

        private void customButton2_Click(object sender, EventArgs e)
        {
            HighlightSyntax();
        }

        private void customButton3_Click(object sender, EventArgs e)
        {
            Close();
        }


        int oldWidth;
        int oldHeight;
        Point oldLocation;
        bool ToggleMaximize;
        private void customButton4_Click(object sender, EventArgs e)
        {
            TopCount = 0;
            if (ToggleMaximize)
            {
                ToggleMaximize = false;

                customButton4.BackgroundImage = Data.Icons.MaximiseW;
                WindowState = FormWindowState.Normal;

                Size = new Size(oldWidth, oldHeight);
                Location = new Point(oldLocation.X, oldLocation.Y);

            }
            else
            {
                ToggleMaximize = true;

                oldHeight = Height;
                oldWidth = Width;
                oldLocation = Location;


                customButton4.BackgroundImage = Data.Icons.MaximizedW;
                WindowState = FormWindowState.Normal;

                int x = SystemInformation.WorkingArea.Width;
                int y = SystemInformation.WorkingArea.Height;
                Location = new Point(0, 0);
                Size = new Size(x, y);


            }
        }

        #endregion


        #region BackGroundWorker

        bool backgroundWorkerWORKING = false;
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {

            backgroundWorkerWORKING = true;


            #region Keywords


            string BlueKeywords = @"\b(abstract|as|base|checked|class|const|delegate|enum|extern|false|interface|internal|is|namespace|new|null|override|private|protected|public|readonly|ref|sealed|static|struct|this|true|unchecked|unsafe|virtual|volatile)\b";
            MatchCollection BluekeywordMatches = Regex.Matches(richTextBoxSyntax.Text, BlueKeywords);

            string LightBlueKeywords = @"\b(bool|byte|case|char|decimal|default|double|event|explicit|fixed|float|for|foreach|if|implicit|in|int|lock|long|object|operator|out|params|sbyte|short|sizeof|stackalloc|string|switch|typeof|uint|ulong|ushort|void|while|)\b";
            MatchCollection LightBlueKeywordsMatches = Regex.Matches(richTextBoxSyntax.Text, LightBlueKeywords);

            string PurpleKeywords = @"\b(break|catch|continue|do|else|finally|goto|return|throw|try|using)\b";
            MatchCollection PurpleKeywordsMatches = Regex.Matches(richTextBoxSyntax.Text, PurpleKeywords);


            #endregion

     



            MatchCollection NumbersMatches = Regex.Matches(richTextBoxSyntax.Text, "[0-9]");

            MatchCollection commentMatches = Regex.Matches(richTextBoxSyntax.Text, "//.+?$|#.+?$|::.+?$", RegexOptions.Multiline);

            MatchCollection ColorBracketMatches = Regex.Matches(richTextBoxSyntax.Text, "[(](.+?)[)]");

            MatchCollection ColorSymbolesWhiteMatches = Regex.Matches(richTextBoxSyntax.Text, @"[(){}[\]^!+,.=;-]");

            MatchCollection stringMatches = Regex.Matches(richTextBoxSyntax.Text, "\".+?\"");


            MatchCollection FunctionWordMatches = Regex.Matches(richTextBoxSyntax.Text, "\\b[A-Z]\\w+");


            #region Settings


            int originalIndex = richTextBoxSyntax.SelectionStart;
            int originalLength = richTextBoxSyntax.SelectionLength;
            Color originalColor = Color.FromArgb(150, 200, 250);

 
            Focus();

 
            richTextBoxSyntax.SelectionStart = 0;
            richTextBoxSyntax.SelectionLength = richTextBoxSyntax.Text.Length;
            richTextBoxSyntax.SelectionColor = originalColor;

            #endregion


            #region Process Text




















            // Words [AZ]
            foreach (Match m in FunctionWordMatches)
            {
                richTextBoxSyntax.SelectionStart = m.Index;
                richTextBoxSyntax.SelectionLength = m.Length;
                richTextBoxSyntax.SelectionColor = Color.FromArgb(75, 200, 175);
            }
            // Log
            ProgressBarPercentage = random.Next(0, 14); SetProgressBarValue(); label1.Text = $"Step 1/10 - {ProgressBarPercentage}%";






            // (Text)
            foreach (Match m in ColorBracketMatches)
            {
                richTextBoxSyntax.SelectionStart = m.Index;
                richTextBoxSyntax.SelectionLength = m.Length;
                richTextBoxSyntax.SelectionColor = Color.FromArgb(225, 225, 150);
            }
            // Log
            ProgressBarPercentage = random.Next(14, 22); SetProgressBarValue(); label1.Text = $"Step 2/10 - {ProgressBarPercentage}%";




            #region Keywords
            foreach (Match m in BluekeywordMatches)
            {
                richTextBoxSyntax.SelectionStart = m.Index;
                richTextBoxSyntax.SelectionLength = m.Length;
                richTextBoxSyntax.SelectionColor = Color.FromArgb(60, 120, 180);
            }
            // Log
            ProgressBarPercentage = random.Next(22, 34); SetProgressBarValue(); label1.Text = $"Step 3/10 - {ProgressBarPercentage}%";


            foreach (Match m in LightBlueKeywordsMatches)
            {
                richTextBoxSyntax.SelectionStart = m.Index;
                richTextBoxSyntax.SelectionLength = m.Length;
                richTextBoxSyntax.SelectionColor = Color.FromArgb(60, 120, 180);
            }
            // Log
            ProgressBarPercentage = random.Next(34, 46); SetProgressBarValue(); label1.Text = $"Step 4/10 - {ProgressBarPercentage}%";


            foreach (Match m in PurpleKeywordsMatches)
            {
                richTextBoxSyntax.SelectionStart = m.Index;
                richTextBoxSyntax.SelectionLength = m.Length;
                richTextBoxSyntax.SelectionColor = Color.FromArgb(200, 100, 200);
            }
            // Log
            ProgressBarPercentage = random.Next(46, 59); SetProgressBarValue(); label1.Text = $"Step 5/10 - {ProgressBarPercentage}%";

            #endregion



            // Color White 
            foreach (Match m in ColorSymbolesWhiteMatches)
            {
                richTextBoxSyntax.SelectionStart = m.Index;
                richTextBoxSyntax.SelectionLength = m.Length;
                richTextBoxSyntax.SelectionColor = Color.White;
            }
            // Log
            ProgressBarPercentage = random.Next(59, 67); SetProgressBarValue(); label1.Text = $"Step 6/10 - {ProgressBarPercentage}%";



            // 0-9
            foreach (Match m in NumbersMatches)
            {
                richTextBoxSyntax.SelectionStart = m.Index;
                richTextBoxSyntax.SelectionLength = m.Length;
                richTextBoxSyntax.SelectionColor = Color.FromArgb(125, 200, 125);
            }
            // Log
            ProgressBarPercentage = random.Next(67, 77); SetProgressBarValue(); label1.Text = $"Step 7/10 - {ProgressBarPercentage}%";


            // # OR //
            foreach (Match m in commentMatches)
            {
                richTextBoxSyntax.SelectionStart = m.Index;
                richTextBoxSyntax.SelectionLength = m.Length;
                richTextBoxSyntax.SelectionColor = Color.Green;
            }
            // Log
            ProgressBarPercentage = random.Next(77, 86); SetProgressBarValue(); label1.Text = $"Step 8/10 - {ProgressBarPercentage}%";



            // "Text"
            foreach (Match m in stringMatches)
            {
                richTextBoxSyntax.SelectionStart = m.Index;
                richTextBoxSyntax.SelectionLength = m.Length;
                richTextBoxSyntax.SelectionColor = Color.FromArgb(215, 150, 100);
            }
            // Log
            ProgressBarPercentage = random.Next(86, 95); SetProgressBarValue(); label1.Text = $"Step 9/9 - {ProgressBarPercentage}%";




            #endregion


            #region Setting End



            richTextBoxSyntax.SelectionStart = originalIndex;
            richTextBoxSyntax.SelectionLength = originalLength;
            richTextBoxSyntax.SelectionColor = originalColor;


 
            #endregion

            // Log
            ProgressBarPercentage = 100; SetProgressBarValue(); label1.Text = $"Syntax Highlight Complete - {ProgressBarPercentage}%";



        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            LoadingTimer.Stop();
            ShowPanel();
            backgroundWorkerWORKING = false;
            richTextBoxSyntax.Select();
        }

        #endregion


        #region Interface Functions
        void ShowPanel()
        {
            richTextBoxSyntax.Visible = true;
            customButton1.Enabled = true; customButton1.Image = Data.Icons.Save32W;
            customButton2.Enabled = true; customButton2.Image = Data.Icons.Code32W;
            customButton6.Enabled = true; customButton6.Image = Data.Icons.Restart32W;
            label2.Visible = false;


            MaximumSize = new Size(0, 0);
            MinimumSize = new Size(550, 400);
        }
        void HidePanel()
        {
            richTextBoxSyntax.Visible = false;
            customButton1.Enabled = false; customButton1.Image = Data.Icons.Save32G;
            customButton2.Enabled = false; customButton2.Image = Data.Icons.Code32G;
            customButton6.Enabled = false; customButton6.Image = Data.Icons.Restart32G;
            label2.Visible = true;

            MaximumSize = new Size(Width, Height);
            MinimumSize = new Size(Width, Height);
        }


        private async void LoadingTimer_Tick(object sender, EventArgs e)
        {

            await Task.Delay(250);
            if (label2.Text == "Highlighting") { label2.Text = "Highlighting."; }
            await Task.Delay(250);
            if (label2.Text == "Highlighting.") { label2.Text = "Highlighting.."; }
            await Task.Delay(250);
            if (label2.Text == "Highlighting..") { label2.Text = "Highlighting..."; }
            await Task.Delay(250);
            if (label2.Text == "Highlighting...") { label2.Text = "Highlighting"; }

        }
        #endregion


        #region Resize
        protected override void WndProc(ref Message m)
        {

   
            const int RESIZE_HANDLE_SIZE = 10;
            switch (m.Msg)
            {
                case 0x0084/*NCHITTEST*/ :
                    base.WndProc(ref m);

                    if ((int)m.Result == 0x01/*HTCLIENT*/)
                    {
                        Point screenPoint = new Point(m.LParam.ToInt32());
                        Point clientPoint = this.PointToClient(screenPoint);
                        if (clientPoint.Y <= RESIZE_HANDLE_SIZE)
                        {
                            if (clientPoint.X <= RESIZE_HANDLE_SIZE)
                                m.Result = (IntPtr)13/*HTTOPLEFT*/ ;
                            else if (clientPoint.X < (Size.Width - RESIZE_HANDLE_SIZE))
                                m.Result = (IntPtr)12/*HTTOP*/ ;
                            else
                                m.Result = (IntPtr)14/*HTTOPRIGHT*/ ;
                        }
                        else if (clientPoint.Y <= (Size.Height - RESIZE_HANDLE_SIZE))
                        {
                            if (clientPoint.X <= RESIZE_HANDLE_SIZE)
                                m.Result = (IntPtr)10/*HTLEFT*/ ;
                            else if (clientPoint.X < (Size.Width - RESIZE_HANDLE_SIZE))
                                m.Result = (IntPtr)1/*HTCAPTION*/ ;
                            else
                                m.Result = (IntPtr)11/*HTRIGHT*/ ;
                        }
                        else
                        {
                            if (clientPoint.X <= RESIZE_HANDLE_SIZE)
                                m.Result = (IntPtr)16/*HTBOTTOMLEFT*/ ;
                            else if (clientPoint.X < (Size.Width - RESIZE_HANDLE_SIZE))
                                m.Result = (IntPtr)15/*HTBOTTOM*/ ;
                            else
                                m.Result = (IntPtr)17/*HTBOTTOMRIGHT*/ ;
                        }
                    }
                    return;
            }
            base.WndProc(ref m);
      
        }
        #endregion


        #region Hotkeys

        private void richTextBoxSyntax_KeyDown(object sender, KeyEventArgs e)
        {
            // Overlay Find
            if (e.Control && e.KeyCode == Keys.F)
            {

                Data.OverlayEditorData.Default.Reset();
 
                
                DataCheckerTimer.Start();
                Overlays.EditorOverlayForm OverlayForm = new Overlays.EditorOverlayForm();
                Overlay(OverlayForm);
                

            }

    

            if (e.Control && e.KeyCode == Keys.R)
            {
                CustomLoad();
            }


            if (e.Control && e.KeyCode == Keys.H)
            {
                HighlightSyntax();
            }


            if (e.Control && e.KeyCode == Keys.S)
            {
                if (!SaveingFile)
                {
                    SaveingFile = true;

                    richTextBoxSyntax.SaveFile(GlobalFilePath, RichTextBoxStreamType.PlainText);

                    LabelFadeOut(label1, "File Saved", 20, 30, 40);

                    SaveingFile = false;
                }
            }

            if (e.Control && e.KeyCode == Keys.R)
            {
                richTextBoxSyntax.Redo();
            }
            if (e.Control && e.KeyCode == Keys.Z)
            {
                richTextBoxSyntax.Undo();
            }
        }
        #endregion


        #region Active Logging

        private void EditorForm_Resize(object sender, EventArgs e)
        {
            Data.AppInfo.Default.EditorSize = Size;

            if (!backgroundWorkerWORKING)
            {
                AddLineNumbers();
            }
        }

        private void EditorForm_Deactivate(object sender, EventArgs e)
        {
            Data.AppInfo.Default.EditorActive = false;
        }

        private void EditorForm_Activated(object sender, EventArgs e)
        {
            Data.AppInfo.Default.EditorActive = true;
        }

        #endregion


        #region Interface Undock Top
        int TopCount = 0;
        private async void EditorForm_Move(object sender, EventArgs e)
        {
            Data.AppInfo.Default.EditorLocation = Location;
            //Entire Top ScreenBorder
            if (ToggleMaximize && Location.Y <= 10)
            {
                if (TopCount > 1)
                {

                    await Task.Delay(1);
                    ReleaseCapture();
                    await Task.Delay(1);

                    ToggleMaximize = false;

                    customButton4.BackgroundImage = Data.Icons.MaximiseW;
                    WindowState = FormWindowState.Normal;

                    while (Width != oldWidth || Height != oldHeight || Width == Screen.PrimaryScreen.Bounds.Width || Height == Screen.PrimaryScreen.Bounds.Height)
                    {
                        await Task.Delay(1);
                        ReleaseCapture();
                        await Task.Delay(1);
                        Size = new Size(oldWidth, oldHeight);
                        Location = new Point(oldLocation.X, oldLocation.Y);
                        Cursor.Position = new Point(oldLocation.X + oldWidth / 2, oldLocation.Y);

                        if (Width == oldWidth && Height == oldHeight && Width != Screen.PrimaryScreen.Bounds.Width && Height != Screen.PrimaryScreen.Bounds.Height)
                        {
                            TopCount = 0;
                            await Task.Delay(1);
                            ReleaseCapture();
                            await Task.Delay(100);
                            Size = new Size(oldWidth, oldHeight);
                            Location = new Point(oldLocation.X, oldLocation.Y);
                            Cursor.Position = new Point(oldLocation.X + oldWidth / 2, oldLocation.Y);
                            break;

                        }

                    }



                }
                TopCount++;

            }
        }
        #endregion


        #region Fix Richtextbox Scrolling
        private void richTextBoxSyntax_ContentsResized(object sender, ContentsResizedEventArgs e)
        {
            if (!backgroundWorkerWORKING)
            {
                if (richTextBoxSyntax.ZoomFactor != 1)
                {
              
                    richTextBoxSyntax.SelectAll();
                    richTextBoxSyntax.ZoomFactor = 1.0f;
                    richTextBoxSyntax.DeselectAll();

                }
            }
        }
        #endregion

        private void DataCheckerTimer_Tick(object sender, EventArgs e)
        {

            if (!String.IsNullOrEmpty(Data.OverlayEditorData.Default.Keyword))
            {
                DataCheckerTimer.Stop();

                try
                {
                    richTextBoxSyntax.Find(Data.OverlayEditorData.Default.Keyword);
                    richTextBoxSyntax.DeselectAll();
                } catch { }

            }
            if (!String.IsNullOrEmpty(Data.OverlayEditorData.Default.LineNumber))
            {
                DataCheckerTimer.Stop();

                try
                {
                    int LineNumber = Convert.ToInt32(Data.OverlayEditorData.Default.LineNumber);
                    int index = richTextBoxSyntax.GetFirstCharIndexFromLine(LineNumber - 1);
                    richTextBoxSyntax.Select(index, 0);
                    richTextBoxSyntax.ScrollToCaret();
                } catch {}



            }
     
        }

        private void customButton6_Click(object sender, EventArgs e)
        {
            CustomLoad();
        }
    }
}
