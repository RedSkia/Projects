using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RustManager
{
    public partial class MainForm : Form
    {
        public static byte[] ByteToken;

        public MainForm(byte[] Token)
        {
            InitializeComponent();
            Data.RestartData.Default.PropertyChanged += RestartData_PropertyChanged;
      
            if (Token != ByteToken)
            {
                Application.Exit();
            }
     

        }

        private async void MainForm_Load(object sender, EventArgs e)
        {

            SetCombobox();
            CheckServers();
            
            if(DisplayPanel.Controls.Count <= 0)
            {
                CurrentWindow = "Home"; ActiveWindow();
            }

            if (ServerPathChecker.IsBusy != true)
            {
               ServerPathChecker.RunWorkerAsync();
            }

            if (ServerStalledChecker.IsBusy != true)
            {
                //ServerStalledChecker.RunWorkerAsync();
            }


            if (ServerAliveStatus.IsBusy != true)
            {
                //ServerAliveStatus.RunWorkerAsync();
            }

            Data.AppInfo.Default.AppMinSize = MinimumSize;
        }




        #region Check Servers



        void CheckServers()
        {

            //If Servers Installed
            if (Data.AppData.Default.RootFolder != string.Empty)
            {
                if (Directory.Exists($"{Data.AppData.Default.RootFolder}/"))
                {
                    if (Directory.Exists($"{Data.AppData.Default.RootFolder}/{Data.AppData.Default.CurrentServer}/"))
                    {
                        if (Directory.Exists($"{Data.AppData.Default.RootFolder}/{Data.AppData.Default.CurrentServer}/LiveServer"))
                        {
                            if (Directory.Exists($"{Data.AppData.Default.RootFolder}/{Data.AppData.Default.CurrentServer}/SteamCMD"))
                            {
                                if (File.Exists($"{Data.AppData.Default.RootFolder}/{Data.AppData.Default.CurrentServer}/Start.bat"))
                                {
                                    ServersInstalled();
                                }
                                else { NoServers(); }
                            }
                            else { NoServers(); }
                        }
                        else { NoServers(); }
                    }
                    else { NoServers(); }
                }
                else { NoServers(); }
            }
            else { NoServers(); }
        }


        void SetCombobox()
        {
            try
            {
                //Set ComboBox
                var Servers = new BindingSource();
                foreach (string folder in Directory.GetDirectories(Data.AppData.Default.RootFolder))
                    Servers.Add(Path.GetFileName(folder));
                comboBox1.DataSource = Servers;
            } catch { }

        }

        bool InstalledActive = false;
        void ServersInstalled()
        {
            if(!InstalledActive)
            {

                CurrentWindow = Data.AppInfo.Default.AppLastWindow; ActiveWindow();
                customButton4.Enabled = true; customButton4.Image = Data.Icons.House32W;
                customButton5.Enabled = true; customButton5.Image = Data.Icons.Plugin32W;
                customButton7.Enabled = true; customButton7.Image = Data.Icons.Gear32W;
            }
            InstalledActive = true;
            NOServersActive = false;
        }

        bool NOServersActive = false;
        void NoServers()
        {

      
            if (!NOServersActive)
            {
                if(CurrentWindow != "Installer")
                {
                    Data.AppInfo.Default.AppLastWindow = CurrentWindow;
                    CurrentWindow = "Installer"; ActiveWindow();
                    customButton4.Enabled = false; customButton4.Image = Data.Icons.House32G;
                    customButton5.Enabled = false; customButton5.Image = Data.Icons.Plugin32G;
                    customButton7.Enabled = false; customButton7.Image = Data.Icons.Gear32G;
                }
                else
                {
                    customButton4.Enabled = false; customButton4.Image = Data.Icons.House32G;
                    customButton5.Enabled = false; customButton5.Image = Data.Icons.Plugin32G;
                    customButton7.Enabled = false; customButton7.Image = Data.Icons.Gear32G;
                }
    
            }
            NOServersActive = true;
            InstalledActive = false;
        }
        #endregion


        #region Interface Main Buttons

        string CurrentWindow = "";


        void ActiveWindow()
        {

            foreach (Control i in DisplayPanel.Controls)
            {
                i.Dispose();


            }




            if (CurrentWindow == "Home")
            {
                UserControls.Home Control = new UserControls.Home() { Dock = DockStyle.Fill };
                DisplayPanel.Controls.Clear();

                if(DisplayPanel.Controls.Count <= 0)
                {
                    DisplayPanel.Controls.Add(Control);
                    Control.Show();

                    if(DisplayPanel.Controls.Contains(Control))
                    {

                        panel5.Visible = true;

                        panel6.Visible = false;
                        panel7.Visible = false;
                        panel8.Visible = false;
                    }
                }
            }


            if (CurrentWindow == "Plugins")
            {
                UserControls.Plugins Control = new UserControls.Plugins() { Dock = DockStyle.Fill };
                DisplayPanel.Controls.Clear();

                if (DisplayPanel.Controls.Count <= 0)
                {
                    DisplayPanel.Controls.Add(Control);
                    Control.Show();

                    if (DisplayPanel.Controls.Contains(Control))
                    {

                        panel6.Visible = true;

                        panel5.Visible = false;
                        panel7.Visible = false;
                        panel8.Visible = false;
                    }
                }
            }

            if (CurrentWindow == "Installer")
            {
                UserControls.Installer Control = new UserControls.Installer() { Dock = DockStyle.Fill };
                DisplayPanel.Controls.Clear();

                if (DisplayPanel.Controls.Count <= 0)
                {
                    DisplayPanel.Controls.Add(Control);
                    Control.Show();

                    if (DisplayPanel.Controls.Contains(Control))
                    {

                        panel7.Visible = true;

                        panel5.Visible = false;
                        panel6.Visible = false;
                        panel8.Visible = false;
                    }
                }
            }

            if (CurrentWindow == "Options")
            {
                UserControls.Options Control = new UserControls.Options() { Dock = DockStyle.Fill };
                DisplayPanel.Controls.Clear();

                if (DisplayPanel.Controls.Count <= 0)
                {
                    DisplayPanel.Controls.Add(Control);
                    Control.Show();

                    if (DisplayPanel.Controls.Contains(Control))
                    {
                        panel8.Visible = true;

                        panel5.Visible = false;
                        panel6.Visible = false;
                        panel7.Visible = false;
                    }
                }
            }
        }


        //Home
        private void customButton4_Click(object sender, EventArgs e)
        {
            if (CurrentWindow != "Home")
            {
                CurrentWindow = "Home";
                ActiveWindow();
            }


        }

        //Plugins
        private void customButton5_Click(object sender, EventArgs e)
        {
            if (CurrentWindow != "Plugins")
            {
                CurrentWindow = "Plugins";
                ActiveWindow();
            }
        }

        //Installer
        private void customButton6_Click(object sender, EventArgs e)
        {
            if (CurrentWindow != "Installer")
            {
                CurrentWindow = "Installer";
                ActiveWindow();
            }
        }

        //Options
        private void customButton7_Click(object sender, EventArgs e)
        {
            if (CurrentWindow != "Options")
            {
                CurrentWindow = "Options";
                ActiveWindow();
            }
        }
        #endregion


        #region Select Server ComboBox
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                //Get Number
                string StringInput = comboBox1.SelectedItem.ToString();
                string IntOutput = string.Empty;

                for (int i = 0; i < StringInput.Length; i++)
                {
                    if (Char.IsDigit(StringInput[i]))
                        IntOutput += StringInput[i];
                }

                Data.AppData.Default.CurrentServer = comboBox1.SelectedItem.ToString();
                Data.AppData.Default.CurrentServerIndex = Convert.ToInt32(IntOutput);

            } catch { }
            CheckServers();
         

        }

        private void comboBox1_KeyUp(object sender, KeyEventArgs e)
        {
            e.Handled = true;
        }

        private void comboBox1_KeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;
        }

        #endregion


        #region Active Logging
        private void MainForm_Resize(object sender, EventArgs e)
        {
            Data.AppInfo.Default.AppSize = Size;

        }

        private void MainForm_Deactivate(object sender, EventArgs e)
        {
            Data.AppInfo.Default.AppActive = false;
        }

        private void MainForm_Activated(object sender, EventArgs e)
        {
            Data.AppInfo.Default.AppActive = true;
        }
        #endregion


        #region Drag

        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        private void tableLayoutPanel1_MouseMove(object sender, MouseEventArgs e)
        {
            //Correct Drag Function
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }
        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            //Correct Drag Function
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }
        private void label1_MouseMove(object sender, MouseEventArgs e)
        {
            //Correct Drag Function
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }
        private void label2_MouseMove(object sender, MouseEventArgs e)
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

        #region Close Maximize Minimize Buttons

        private void customButton1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        int oldWidth;
        int oldHeight;
        Point oldLocation;
        bool ToggleMaximize;
        private void customButton2_Click(object sender, EventArgs e)
        {
            TopCount = 0;
            if (ToggleMaximize)
            {
                ToggleMaximize = false;

                customButton2.BackgroundImage = Data.Icons.MaximiseW;
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


                customButton2.BackgroundImage = Data.Icons.MaximizedW;
                WindowState = FormWindowState.Normal;

                int x = SystemInformation.WorkingArea.Width;
                int y = SystemInformation.WorkingArea.Height;
                Location = new Point(0, 0);
                Size = new Size(x, y);


            }
        }

        private void customButton3_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        #endregion

        #region Interface Undock Top
        int TopCount = 0;
        private async void MainForm_Move(object sender, EventArgs e)
        {
            Data.AppInfo.Default.AppLocation = Location;
            //Entire Top ScreenBorder
            if (ToggleMaximize && Location.Y <= 10)
            {
                if (TopCount > 1)
                {

                    await Task.Delay(1);
                    ReleaseCapture();
                    await Task.Delay(1);

                    ToggleMaximize = false;

                    customButton2.BackgroundImage = Data.Icons.MaximiseW;
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










































        // Sub Functions
        #region SendKeys
        public static class WindowHelper
        {
            public static void BringProcessToFront(Process process)
            {
                IntPtr handle = process.MainWindowHandle;
                if (IsIconic(handle))
                {
                    ShowWindow(handle, SW_RESTORE);
                }
                SetForegroundWindow(handle);
            }

            const int SW_RESTORE = 9;

            [System.Runtime.InteropServices.DllImport("User32.dll")]
            private static extern bool SetForegroundWindow(IntPtr handle);
            [System.Runtime.InteropServices.DllImport("User32.dll")]
            private static extern bool ShowWindow(IntPtr handle, int nCmdShow);
            [System.Runtime.InteropServices.DllImport("User32.dll")]
            private static extern bool IsIconic(IntPtr handle);
        }


        void CustomSendKeys(Process proc, string Keys)
        {
            if (proc != null && proc.Responding)
            {
                WindowHelper.BringProcessToFront(proc);
                SendKeys.SendWait(Keys);
            }


        }
        #endregion


        #region Restart Function

        async void RestartData_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {

            if (Data.RestartData.Default.PID > 0 && Data.RestartData.Default.RestartSeconds > 0 && Data.RestartData.Default.RestartEnabled)
            {
                const string EnterKey = "{Enter}";
                int RestartMinsSecs = Data.RestartData.Default.RestartSeconds / 60 % 60 * 60;
                int RestartSecs = Data.RestartData.Default.RestartSeconds % 60;
                Data.DataLog.Default.RestartTimeLeft = RestartMinsSecs + RestartSecs;
                CustomSendKeys(Process.GetProcessById(Data.RestartData.Default.PID), $"Say Server Restart Initiated{EnterKey}");
                await Task.Delay(100);

       

                while (Data.RestartData.Default.RestartEnabled)
                {
                    try
                    {

                        if(!RestartTimerLog.Enabled)
                        {
                            RestartTimerLog.Start();
                        }
          

                        if (RestartSecs > 0 && Data.RestartData.Default.RestartEnabled)
                        {
                            RestartSendingKeysActive = true;
                            CustomSendKeys(Process.GetProcessById(Data.RestartData.Default.PID), $"Say Server Restarting in <color=orange>{RestartMinsSecs / 60}m:{RestartSecs}s</color=orange>{EnterKey}");
                            RestartSendingKeysActive = false;
                            await Task.Delay(RestartSecs * 1000);
                            RestartSecs = 0;
                        }

                        if (RestartMinsSecs > 60 && Data.RestartData.Default.RestartEnabled)
                        {
                            RestartSendingKeysActive = true;
                            CustomSendKeys(Process.GetProcessById(Data.RestartData.Default.PID), $"Say Server Restarting in <color=orange>{RestartMinsSecs / 60}m</color=orange>{EnterKey}");
                            RestartSendingKeysActive = false;
                            await Task.Delay(60000);
                            RestartMinsSecs -= 60;
                        }

                        if (RestartMinsSecs <= 60 && RestartMinsSecs > 10 && Data.RestartData.Default.RestartEnabled)
                        {
                            RestartSendingKeysActive = true;
                            CustomSendKeys(Process.GetProcessById(Data.RestartData.Default.PID), $"Say Server Restarting in <color=orange>{RestartMinsSecs}s</color=orange>{EnterKey}");
                            RestartSendingKeysActive = false;
                            await Task.Delay(10000);
                            RestartMinsSecs -= 10;
                        }


                        if (RestartMinsSecs <= 10 && Data.RestartData.Default.RestartEnabled)
                        {
                            RestartSendingKeysActive = true;
                            CustomSendKeys(Process.GetProcessById(Data.RestartData.Default.PID), $"Say Server Restarting in <color=orange>{RestartMinsSecs}s</color=orange>{EnterKey}");
                            RestartSendingKeysActive = false;
                            await Task.Delay(1000);
                            RestartMinsSecs -= 1;
                        }

                        if (RestartMinsSecs <= 0 && Data.RestartData.Default.RestartEnabled)
                        {
                            CustomSendKeys(Process.GetProcessById(Data.RestartData.Default.PID), $"{EnterKey}Say Restarting in <color=orange>NOW</color>{EnterKey}");
                            await Task.Delay(1);
                            CustomSendKeys(Process.GetProcessById(Data.RestartData.Default.PID), $"{EnterKey}Save{EnterKey}");
                            await Task.Delay(1);
                            CustomSendKeys(Process.GetProcessById(Data.RestartData.Default.PID), $"{EnterKey}Quit{EnterKey}");

                            //Kill Process
                            Process.GetProcessById(Data.RestartData.Default.PID).Kill();
                            await Task.Delay(1);

                            Data.DataLog.Default.RestartTimeLeft = 0;
                            Data.RestartData.Default.RestartEnabled = false;
                            RestartTimerLog.Stop();


                            //Start Process After Kill
                            Process.Start($"{Data.AppData.Default.RootFolder}\\{Data.AppData.Default.CurrentServer}\\Start.bat", $"{Data.AppData.Default.RootFolder}\\{Data.AppData.Default.CurrentServer}");


                            break;
                        }


                    }
                    catch
                    {
                        MessageBox.Show("Process Not Running" + Environment.NewLine + "SendKeys Failed", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                    }

                }
  

                
            }
        }


        //int TimeLeftLog = Data.RestartData.Default.RestartSeconds / 60 % 60 * 60 + Data.RestartData.Default.RestartSeconds % 60;


        bool RestartSendingKeysActive = false;

        private void RestartTimerLog_Tick(object sender, EventArgs e)
        {


            //FIX
            if (Process.GetProcessById(Data.RestartData.Default.PID).HasExited)
            {
                RestartTimerLog.Stop();
                MessageBox.Show("Exited.");

            }





            if (!Data.RestartData.Default.RestartEnabled)
            {
                RestartTimerLog.Stop();
            }
            if (!RestartSendingKeysActive)
            {
                Data.DataLog.Default.RestartTimeLeft -= 1;
                Data.DataLog.Default.RestartTimeProcessed += 1;
            }

        }







        #endregion


        #region Server Alive Status
        private async void ServerAliveStatus_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

            if (ServerAliveStatus.IsBusy != true)
            {
                await Task.Delay(1);
                ServerAliveStatus.RunWorkerAsync();
            }
        }

        private void ServerAliveStatus_DoWork(object sender, DoWorkEventArgs e)
        {


            if (StartingCheck() == "Responding" && TcpConnectCheck() == "Connected")
            {
                Data.ServerDataLog.Default.ServerStatus = "Online";
            }
            else if (StartingCheck() == "Responding")
            {
                Data.ServerDataLog.Default.ServerStatus = "Responding";
            }
            else
            {
                Data.ServerDataLog.Default.ServerStatus = "Offline";
            }

        }
        #endregion

        #region Stalled Server Checker
        List<Tuple<string, int, string>> ServerDatabase = new List<Tuple<string, int, string>>();

        int StalledCounter = 0;
        int ServerIndex = 0;
        int CheckServerIndex = 0;
        int FileIndex = 0;

        const int StalledMaxValue = 20; //Mins

        bool StalledServerDataLoaded = false;
        private void ServerStalledChecker_DoWork(object sender, DoWorkEventArgs e)
        {


            ServerIndex = 0;
            FileIndex = 0;
            CheckServerIndex = 0;

            StalledCounter += 1;


            Data.ServerDataLog.Default.ServerStalledMaxPoints = StalledMaxValue;

            if (!StalledServerDataLoaded)
            {
                //Create Data
                foreach (string Server in Directory.GetDirectories(Data.AppData.Default.RootFolder))
                {
                    var FileDataSum = "";
                    foreach (var SavFile in Directory.GetFiles($"{Data.AppData.Default.RootFolder}\\{Path.GetFileName(Server)}\\LiveServer\\server\\SERVER", "*.sav.*"))
                    {
                        FileDataSum = File.GetLastWriteTime(SavFile).ToString();
                        FileDataSum = FileDataSum.Sum(x => Convert.ToInt32(x)).ToString();

                    }
                    ServerDatabase.Add(Tuple.Create(Path.GetFileName(Server), 0, FileDataSum.ToString()));



                    ServerIndex++;
                }

                StalledServerDataLoaded = true;
            }


            //Check
            foreach (string Server in Directory.GetDirectories(Data.AppData.Default.RootFolder))
            {
                var FileDataValue = "";
                foreach (var SavFile in Directory.GetFiles($"{Data.AppData.Default.RootFolder}\\{Path.GetFileName(Server)}\\LiveServer\\server\\SERVER", "*.sav.*"))
                {
                    FileDataValue += File.GetLastWriteTime(SavFile).ToString();



                    FileIndex++;
                }

                if (ServerDatabase[CheckServerIndex].Item3 != FileDataValue.ToString())
                {

                    try
                    {
                        ServerDatabase[CheckServerIndex] = Tuple.Create(Path.GetFileName(Server), 0, FileDataValue.ToString());
                    }
                    catch { }

                }
                else
                {
                    try
                    {
                        ServerDatabase[CheckServerIndex] = Tuple.Create(Path.GetFileName(Server), ServerDatabase[CheckServerIndex].Item2 + 1, FileDataValue.ToString());
                    }
                    catch { }

                }

                //Server Stalled DO
                if (ServerDatabase[CheckServerIndex].Item2 > StalledMaxValue)
                {
                    ServerDatabase[CheckServerIndex] = Tuple.Create(Path.GetFileName(Server), 0, FileDataValue.ToString());
                    MessageBox.Show($"{Server} Stalled !!");
                }


                #region Log Data
                if (Path.GetFileName(Server) == Data.AppData.Default.CurrentServer)
                {
                    //MessageBox.Show(Path.GetFileName(Server));
                    //MessageBox.Show($"{Server} Points = {ServerDatabase[CheckServerIndex].Item2}");
                    Data.ServerDataLog.Default.ServerStalledPoints = ServerDatabase[CheckServerIndex].Item2;
                }
                #endregion

          
                CheckServerIndex++;
            }

            StalledServerDataLoaded = true;
        }


        int StalledLoopDelaySec = 1;
        private async void ServerStalledChecker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

            await Task.Delay(StalledLoopDelaySec * 1000);
            if (ServerStalledChecker.IsBusy != true)
            {
                ServerStalledChecker.RunWorkerAsync();
            }
        }
        #endregion







   

        static string TcpConnectCheck()
        {
            using (TcpClient client = new TcpClient())
            {
                try
                {
                    client.Connect("localhost", 28016);
                    return ("Connected");
       
                }catch { }
                client.Close();
                client.Dispose();
            }
            return ("Attempting Connection");
        }

        static string StartingCheck()
        {
            var processes = Process.GetProcesses().Where(p => p.MainWindowTitle == $"Server{Data.AppData.Default.CurrentServerIndex}");
            foreach (var process in processes)
            {
                var id = process.Id;
                var Wintitle = process.MainWindowTitle;

                if (process.MainWindowTitle == $"Server{Data.AppData.Default.CurrentServerIndex}")
                {

                    if (Process.GetProcessById(id).Responding)
                    {


                        Process[] pname = Process.GetProcessesByName($"RustDedicated{Data.AppData.Default.CurrentServerIndex}");
                        if (pname.Length != 0)
                        {
                            return ("Responding");


                        }


                    }
                }
            }
            return ("Not Responding");
        }



        private async void ServerPathChecker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            await Task.Delay(1);
            CheckServers();
            if (ServerPathChecker.IsBusy != true)
            {
                ServerPathChecker.RunWorkerAsync();
            }
        }


    }
}
