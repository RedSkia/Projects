using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RustManager.UserControls
{
    public partial class Installer : UserControl
    {
        public Installer()
        {
            InitializeComponent();
            Disposed += OnDispose;
   
            Data.AppData.Default.PropertyChanged += new PropertyChangedEventHandler(Changed);
        }

        void Changed(object sender, PropertyChangedEventArgs e)
        {
            //MessageBox.Show(e.PropertyName);
        }
  



        private void Installer_Load(object sender, EventArgs e)
        {
            ProgressPanel1.Width = 0;
            EnableButtons();


            if (ServerPathChecker.CancellationPending != true)
            {
                ServerPathChecker.RunWorkerAsync();
            }
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
                                else 
                                { 
                                    NoServers();
                                    if (richTextBox1.Text != $"Missing {Data.AppData.Default.CurrentServer}/Start.bat")
                                    {
                                        richTextBox1.Text = "Missing "; ColorRichText($"{Data.AppData.Default.CurrentServer}/Start.bat", Color.FromArgb(250, 50, 50), false);
                                    }
                                }
                            }
                            else 
                            {
                                NoServers();
                                if (richTextBox1.Text != $"Missing {Data.AppData.Default.CurrentServer}/SteamCMD")
                                {
                                    richTextBox1.Text = "Missing "; ColorRichText($"{Data.AppData.Default.CurrentServer}/SteamCMD", Color.FromArgb(250, 50, 50), false);
                                }
                            }
                        }
                        else 
                        { 
                            NoServers();
                            if (richTextBox1.Text != $"Missing {Data.AppData.Default.CurrentServer}/LiveServer")
                            {
                                richTextBox1.Text = "Missing "; ColorRichText($"{Data.AppData.Default.CurrentServer}/LiveServer", Color.FromArgb(250, 50, 50), false);
                            }
                        }
                    }
                    else 
                    { 
                        NoServers();
                        if (richTextBox1.Text != $"Missing {Data.AppData.Default.CurrentServer}")
                        {
                            richTextBox1.Text = "Missing "; ColorRichText($"{Data.AppData.Default.CurrentServer}", Color.FromArgb(250, 50, 50), false);
                        }
                    }
                } 
                else 
                { 
                    NoServers();
                    if (richTextBox1.Text != $"Missing ROOT {Data.AppData.Default.RootFolder}")
                    {
                        richTextBox1.Text = "Missing ROOT "; ColorRichText($"{Data.AppData.Default.RootFolder}", Color.FromArgb(250, 50, 50), false);
                    }
                }
            }
            else 
            {
                NoServers();
                if (richTextBox1.Text != $"No Servers Detected")
                {
                    richTextBox1.Text = "No Servers "; ColorRichText($"Detected", Color.FromArgb(250, 50, 50), false);
                }
            }
        }


        void ServersInstalled()
        {

            if (ServerPathChecker.CancellationPending != true && !richTextBox1.Text.Contains("Oxide Installation Complete") && !richTextBox1.Text.Contains("Server Update Complete") && !richTextBox1.Text.Contains("Server Update Complete") && !richTextBox1.Text.Contains("Server Installation Complete"))
            {
                richTextBox1.Clear();
            }

        }

        void NoServers()
        {

            richTextBox1.Clear();
            customButton2.Enabled = false; customButton2.Image = Data.Icons.Update32G;
            customButton3.Enabled = false; customButton3.Image = Data.Icons.Installer32G;
        }
        #endregion


        #region Installer Functions
        public string CreatePassword(int length)
        {
            const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#£¤$&{/[(])}=?+|´^~*¨<>";
            StringBuilder res = new StringBuilder();
            Random rnd = new Random();
            while (0 < length--)
            {
                res.Append(valid[rnd.Next(valid.Length)]);
            }
            return res.ToString();
        }


        async void StopInstallationFunction()
        {

            await Task.Delay(10);
            try
            {
                Directory.Delete($"{Data.AppInstallerData.Default.TempServerRootPath}/{Data.AppInstallerData.Default.TempCurrentServer}", true);


                if (!Directory.Exists($"{Data.AppInstallerData.Default.TempServerRootPath}/{Data.AppInstallerData.Default.TempCurrentServer}"))
                {
                    Data.AppInstallerData.Default.Reset();

                    richTextBox1.Clear();
                    ColorRichText("Server Installation Aborted", Color.FromArgb(250, 50, 50), true);

                    label2.Text = "Server Installation Aborted";
                    ProgressBarPercentage = -2; SetProgressBarValue();

                }
            }
            catch { }

            try
            {
                Process proc = Process.GetProcessesByName("steamcmd").FirstOrDefault();
                while (proc.Responding)
                {
                    await Task.Delay(10);
                    try
                    {

                        proc.Kill();
                        Directory.Delete($"{Data.AppInstallerData.Default.TempServerRootPath}/{Data.AppInstallerData.Default.TempCurrentServer}", true);

                        Data.AppInstallerData.Default.Reset();

                        richTextBox1.Clear();
                        ColorRichText("Server Installation Aborted", Color.FromArgb(250, 50, 50), true);

                        label2.Text = "Server Installation Aborted";
                        ProgressBarPercentage = -2; SetProgressBarValue();

                        break;
                    }
                    catch { }

                }
            }
            catch { }

            EnableButtons();
            StopInstallation = false;
        }

        #endregion


        #region Interface Functions
        void Overlay(Form SeletedForm)
        {
            Form Overlay = new Form();
            try
            {

                Overlay.StartPosition = FormStartPosition.Manual;
                Overlay.FormBorderStyle = FormBorderStyle.None;
                Overlay.Opacity = 0.75;
                Overlay.BackColor = Color.Black;
                Overlay.Size = Data.AppInfo.Default.AppSize;
                Overlay.Location = Data.AppInfo.Default.AppLocation;
                Overlay.ShowIcon = false;
                Overlay.ShowInTaskbar = false;
                Overlay.Show();


                SeletedForm.ShowInTaskbar = false;
                SeletedForm.Width = Data.AppInfo.Default.AppSize.Width - 200;
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

        void EnableButtons()
        {
            customButton1.Enabled = true; customButton1.Image = Data.Icons.Installer32W;
            customButton2.Enabled = true; customButton2.Image = Data.Icons.Update32W;
            customButton3.Enabled = true; customButton3.Image = Data.Icons.Installer32W;

            customButton4.Enabled = false; customButton4.Image = Data.Icons.Close32G;
        }

        void DisableButtons(bool ShowAbortButton)
        {
            customButton1.Enabled = false; customButton1.Image = Data.Icons.Installer32G;
            customButton2.Enabled = false; customButton2.Image = Data.Icons.Update32G;
            customButton3.Enabled = false; customButton3.Image = Data.Icons.Installer32G;

            if(ShowAbortButton)
            {
                customButton4.Enabled = true; customButton4.Image = Data.Icons.Close32W;
            }
            
        }



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

        async void ColorRichText(string TextToColor, Color Color, bool Newline)
        {
            try
            {
                //await Task.Delay(10);
                if (Newline)
                {
                    richTextBox1.Text += Environment.NewLine + TextToColor;
                }
                else if (!Newline)
                {
                    richTextBox1.Text += TextToColor;
                }






                richTextBox1.Select(richTextBox1.Text.IndexOf(TextToColor), TextToColor.Length);
                richTextBox1.SelectionColor = Color;
                richTextBox1.DeselectAll();

            } catch { }



        }


        #endregion


        #region Interface Buttons
 

        bool StopInstallation = false;
        private void customButton4_Click(object sender, EventArgs e)
        {
            StopInstallation = true;
        }
        #endregion




        //Main Functions

        #region Install Server Code
        private void customButton1_Click(object sender, EventArgs e)
        {
            Data.AppInstallerData.Default.Reset();
            DataCheckerTimer.Start();
            DataCheckerTimer.Start();
            Overlays.InstallerOverlayForm OverlayForm = new Overlays.InstallerOverlayForm();
            Overlay(OverlayForm);
        }
        private void DataCheckerTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                if (!StopInstallation)
                {
                    if (!String.IsNullOrEmpty(Data.AppInstallerData.Default.TempServerRootPath) && !String.IsNullOrEmpty(Data.AppInstallerData.Default.TempCurrentServer) && Data.AppInstallerData.Default.TempServerIndex > 0)
                    {
                        DataCheckerTimer.Stop();


                        Stop_ServerPathChecker();
                        DisableButtons(true);

                        StartServerInstaller();
                    }
                }
                else { StopInstallationFunction(); }
            }
            catch { }
        }


        int LogDelay = 500;
        Random random = new Random();
        async void StartServerInstaller()
        {
            try
            {



                const string quote = "\"";
                const string EndBatLine = " ^";


                //STEP 1
                #region Create Files

                if (!StopInstallation)
                {
                    #region Log
                    richTextBox1.Clear();
                    richTextBox1.Text += "Server Installation Initiated";
                    await Task.Delay(LogDelay);
                    richTextBox1.Text += Environment.NewLine + "Creating Directories";

                    ProgressBarPercentage = random.Next(0, 5); SetProgressBarValue(); label2.Text = $"Step 1/15 - {ProgressBarPercentage}%";
                    #endregion
                }
                else { StopInstallationFunction(); }


                if (!StopInstallation)
                {
                    Directory.CreateDirectory($"{Data.AppInstallerData.Default.TempServerRootPath}/{Data.AppInstallerData.Default.TempCurrentServer}");
                    Directory.CreateDirectory($"{Data.AppInstallerData.Default.TempServerRootPath}/{Data.AppInstallerData.Default.TempCurrentServer}/SteamCMD");
                    Directory.CreateDirectory($"{Data.AppInstallerData.Default.TempServerRootPath}/{Data.AppInstallerData.Default.TempCurrentServer}/LiveServer");
                    Directory.CreateDirectory($"{Data.AppInstallerData.Default.TempServerRootPath}/{Data.AppInstallerData.Default.TempCurrentServer}/LiveServer/Oxide");
                }
                else { StopInstallationFunction(); }


                if (!StopInstallation)
                {
                    #region Log
                    await Task.Delay(LogDelay);
                    richTextBox1.Clear();
                    richTextBox1.Text += "Server Installation Initiated";
                    richTextBox1.Text += Environment.NewLine + "Creating Directories ✔";

                    ProgressBarPercentage = random.Next(5, 11); SetProgressBarValue(); label2.Text = $"Step 2/15 - {ProgressBarPercentage}%";
                    #endregion
                }
                else { StopInstallationFunction(); }


                if (!StopInstallation)
                {
                    #region Log
                    await Task.Delay(LogDelay);
                    richTextBox1.Clear();
                    richTextBox1.Text += "Server Installation Initiated";
                    richTextBox1.Text += Environment.NewLine + "Creating Directories ✔";
                    richTextBox1.Text += Environment.NewLine + "Configuring Server Launch";

                    ProgressBarPercentage = random.Next(11, 16); SetProgressBarValue(); label2.Text = $"Step 3/15 - {ProgressBarPercentage}%";
                    #endregion
                }
                else { StopInstallationFunction(); }

                if (!StopInstallation)
                {
                    #region BatFile

                    //Create Start.bat
                    using (StreamWriter sw = File.CreateText($"{Data.AppInstallerData.Default.TempServerRootPath}/{Data.AppInstallerData.Default.TempCurrentServer}/Start.bat"))
                    {


                        sw.WriteLine("@ECHO OFF");
                        sw.WriteLine("TITLE Server" + Data.AppInstallerData.Default.TempServerIndex);
                        sw.WriteLine("CLS");
                        sw.WriteLine("MODE CON cols=80 lines=20");
                        sw.WriteLine("CD " + quote + @"%1\LiveServer" + quote);
                        sw.WriteLine("for %%* in (..) do ECHO Starting %%~nx*");
                        sw.WriteLine(":: This Batch File is Generated by Infinitynet.dk Rust Server Manager Sharing is Prohibited");
                        sw.WriteLine(":: END OF BATCH");

                        sw.WriteLine($"RustDedicated{Data.AppInstallerData.Default.TempServerIndex}.exe -batchmode -nographics" + EndBatLine);

                        sw.WriteLine("+oxide.directory " + quote + "Oxide" + quote + EndBatLine);
                        sw.WriteLine("+server.hostname " + quote + "In-Game Server Name" + quote + EndBatLine);
                        sw.WriteLine("+server.description " + quote + "In-Game Server Description" + quote + EndBatLine);
                        sw.WriteLine("+server.level " + quote + "Procedural Map" + quote + EndBatLine);
                        sw.WriteLine("+server.seed 1234" + EndBatLine);
                        sw.WriteLine("+server.worldsize 4000" + EndBatLine);
                        sw.WriteLine("+server.maxplayers 10" + EndBatLine);
                        sw.WriteLine("+server.url " + quote + "https://Infinitynet.dk" + quote + EndBatLine);
                        sw.WriteLine("+server.headerimage " + quote + "https://Infinitynet.dk/Image.png" + quote + EndBatLine);
                        sw.WriteLine("+server.ip 0.0.0.0" + EndBatLine);
                        sw.WriteLine("+server.port 28015" + EndBatLine);
                        sw.WriteLine("+server.identity " + quote + "SERVER" + quote + EndBatLine);
                        sw.WriteLine("+server.saveinterval 600" + EndBatLine);
                        sw.WriteLine("+server.secure true" + EndBatLine);
                        sw.WriteLine("+rcon.password " + quote + CreatePassword(64) + quote + EndBatLine);
                        sw.WriteLine("+rcon.port 28016" + EndBatLine);
                        sw.WriteLine("+rcon.web 0");

                        sw.WriteLine("EXIT");
                        sw.Close(); sw.Dispose();
                    }



                    #endregion
                }
                else { StopInstallationFunction(); }

                if (!StopInstallation)
                {
                    #region Log
                    await Task.Delay(LogDelay);
                    richTextBox1.Clear();
                    richTextBox1.Text += "Server Installation Initiated";
                    richTextBox1.Text += Environment.NewLine + "Creating Directories ✔";
                    richTextBox1.Text += Environment.NewLine + "Configuring Server Launch ✔";

                    ProgressBarPercentage = random.Next(16, 23); SetProgressBarValue(); label2.Text = $"Step 4/15 - {ProgressBarPercentage}%";
                    #endregion
                }
                else { StopInstallationFunction(); }


                #endregion

                //STEP 2
                #region SteamCMD Download
                //Get SteamCMD
                using (var client = new WebClient())
                {
                    if (!StopInstallation)
                    {
                        #region Log
                        await Task.Delay(LogDelay);
                        richTextBox1.Clear();
                        richTextBox1.Text += "Server Installation Initiated";
                        richTextBox1.Text += Environment.NewLine + "Creating Directories ✔";
                        richTextBox1.Text += Environment.NewLine + "Configuring Server Launch ✔";
                        richTextBox1.Text += Environment.NewLine + "Downloading SteamCMD";

                        ProgressBarPercentage = random.Next(23, 30); SetProgressBarValue(); label2.Text = $"Step 5/15 - {ProgressBarPercentage}%";
                        #endregion
                    }
                    else { StopInstallationFunction(); }

                    if (!StopInstallation)
                    {
                        //Create and Zip
                        client.DownloadFile("https://steamcdn-a.akamaihd.net/client/installer/steamcmd.zip", $"{Data.AppInstallerData.Default.TempServerRootPath}/{Data.AppInstallerData.Default.TempCurrentServer}/SteamCMD/steamcmd.zip");
                    }
                    else { StopInstallationFunction(); }


                    if (!StopInstallation)
                    {
                        #region Log
                        await Task.Delay(LogDelay);
                        richTextBox1.Clear();
                        richTextBox1.Text += "Server Installation Initiated";
                        richTextBox1.Text += Environment.NewLine + "Creating Directories ✔";
                        richTextBox1.Text += Environment.NewLine + "Configuring Server Launch ✔";
                        richTextBox1.Text += Environment.NewLine + "Downloading SteamCMD ✔";

                        ProgressBarPercentage = random.Next(30, 36); SetProgressBarValue(); label2.Text = $"Step 6/15 - {ProgressBarPercentage}%";
                        #endregion
                    }
                    else { StopInstallationFunction(); }
                }
                #endregion

                //STEP 3
                #region SteamCMD Extract Zip

                if (!StopInstallation)
                {
                    #region Log
                    await Task.Delay(LogDelay);
                    richTextBox1.Clear();
                    richTextBox1.Text += "Server Installation Initiated";
                    richTextBox1.Text += Environment.NewLine + "Creating Directories ✔";
                    richTextBox1.Text += Environment.NewLine + "Configuring Server Launch ✔";
                    richTextBox1.Text += Environment.NewLine + "Downloading SteamCMD ✔";
                    richTextBox1.Text += Environment.NewLine + "Extracting SteamCMD";

                    ProgressBarPercentage = random.Next(36, 43); SetProgressBarValue(); label2.Text = $"Step 7/15 - {ProgressBarPercentage}%";
                    #endregion
                }
                else { StopInstallationFunction(); }

                if (!StopInstallation)
                {
                    //Extract ZIP
                    ZipFile.ExtractToDirectory($"{Data.AppInstallerData.Default.TempServerRootPath}/{Data.AppInstallerData.Default.TempCurrentServer}/SteamCMD/steamcmd.zip", $"{Data.AppInstallerData.Default.TempServerRootPath}/{Data.AppInstallerData.Default.TempCurrentServer}/SteamCMD");
                }
                else { StopInstallationFunction(); }

                if (!StopInstallation)
                {
                    #region Log
                    await Task.Delay(LogDelay);
                    richTextBox1.Clear();
                    richTextBox1.Text += "Server Installation Initiated";
                    richTextBox1.Text += Environment.NewLine + "Creating Directories ✔";
                    richTextBox1.Text += Environment.NewLine + "Configuring Server Launch ✔";
                    richTextBox1.Text += Environment.NewLine + "Downloading SteamCMD ✔";
                    richTextBox1.Text += Environment.NewLine + "Extracting SteamCMD ✔";

                    ProgressBarPercentage = random.Next(43, 50); SetProgressBarValue(); label2.Text = $"Step 8/15 - {ProgressBarPercentage}%";
                    #endregion
                }
                else { StopInstallationFunction(); }


                #endregion

                //STEP 4
                #region Running SteamCMD

                if (!StopInstallation)
                {
                    #region Log
                    await Task.Delay(LogDelay);
                    richTextBox1.Clear();
                    richTextBox1.Text += "Server Installation Initiated";
                    richTextBox1.Text += Environment.NewLine + "Creating Directories ✔";
                    richTextBox1.Text += Environment.NewLine + "Configuring Server Launch ✔";
                    richTextBox1.Text += Environment.NewLine + "Downloading SteamCMD ✔";
                    richTextBox1.Text += Environment.NewLine + "Extracting SteamCMD ✔";
                    richTextBox1.Text += Environment.NewLine + "Launching SteamCMD";

                    ProgressBarPercentage = random.Next(50, 56); SetProgressBarValue(); label2.Text = $"Step 9/15 - {ProgressBarPercentage}%";
                    #endregion
                }
                else { StopInstallationFunction(); }

                if (!StopInstallation)
                {
                    //Install Rust Server With Args
                    string LaunchArgs = "+login anonymous +force_install_dir " + $"{Data.AppInstallerData.Default.TempServerRootPath}/{Data.AppInstallerData.Default.TempCurrentServer}/LiveServer" + " +app_update 258550 +quit";
                    Process.Start($"{Data.AppInstallerData.Default.TempServerRootPath}/{Data.AppInstallerData.Default.TempCurrentServer}/SteamCMD/steamcmd.exe", LaunchArgs);
                }
                else { StopInstallationFunction(); }


                if (!StopInstallation)
                {
                    #region Log
                    await Task.Delay(LogDelay);
                    richTextBox1.Clear();
                    richTextBox1.Text += "Server Installation Initiated";
                    richTextBox1.Text += Environment.NewLine + "Creating Directories ✔";
                    richTextBox1.Text += Environment.NewLine + "Configuring Server Launch ✔";
                    richTextBox1.Text += Environment.NewLine + "Downloading SteamCMD ✔";
                    richTextBox1.Text += Environment.NewLine + "Extracting SteamCMD ✔";
                    richTextBox1.Text += Environment.NewLine + "Launching SteamCMD ✔";

                    ProgressBarPercentage = random.Next(56, 62); SetProgressBarValue(); label2.Text = $"Step 10/15 - {ProgressBarPercentage}%";
                    #endregion
                }
                else { StopInstallationFunction(); }

                if (!StopInstallation)
                {
                    #region Log
                    await Task.Delay(LogDelay);
                    richTextBox1.Clear();
                    richTextBox1.Text += "Server Installation Initiated";
                    richTextBox1.Text += Environment.NewLine + "Creating Directories ✔";
                    richTextBox1.Text += Environment.NewLine + "Configuring Server Launch ✔";
                    richTextBox1.Text += Environment.NewLine + "Downloading SteamCMD ✔";
                    richTextBox1.Text += Environment.NewLine + "Extracting SteamCMD ✔";
                    richTextBox1.Text += Environment.NewLine + "Launching SteamCMD ✔";

                    ProgressBarPercentage = random.Next(62, 68); SetProgressBarValue(); label2.Text = $"Step 11/15 - {ProgressBarPercentage}%";
                    #endregion
                }
                else { StopInstallationFunction(); }

                if (!StopInstallation)
                {
                    #region Log
                    await Task.Delay(LogDelay);
                    richTextBox1.Clear();
                    richTextBox1.Text += "Server Installation Initiated";
                    richTextBox1.Text += Environment.NewLine + "Creating Directories ✔";
                    richTextBox1.Text += Environment.NewLine + "Configuring Server Launch ✔";
                    richTextBox1.Text += Environment.NewLine + "Downloading SteamCMD ✔";
                    richTextBox1.Text += Environment.NewLine + "Extracting SteamCMD ✔";
                    richTextBox1.Text += Environment.NewLine + "Launching SteamCMD ✔";
                    richTextBox1.Text += Environment.NewLine + "Extracting SteamCMD Files";

                    ProgressBarPercentage = random.Next(68, 75); SetProgressBarValue(); label2.Text = $"Step 12/15 - {ProgressBarPercentage}%";
                    #endregion
                }
                else { StopInstallationFunction(); }



                #endregion

                if (!StopInstallation)
                {
                    StatusTimer.Start();
                }
                else { StopInstallationFunction(); }
            }
            catch { }
        }


        bool steamCmdDone = false;
        private async void StatusTimer_Tick(object sender, EventArgs e)
        {
            if (!StopInstallation)
            {

                if (Directory.Exists($"{Data.AppInstallerData.Default.TempServerRootPath}/{Data.AppInstallerData.Default.TempCurrentServer}/SteamCMD/config") && Directory.Exists($"{Data.AppInstallerData.Default.TempServerRootPath}/{Data.AppInstallerData.Default.TempCurrentServer}/SteamCMD/steamapps"))
                {
                    if (!steamCmdDone)
                    {
                        steamCmdDone = true;
                        #region Log
                        await Task.Delay(LogDelay);
                        richTextBox1.Clear();
                        richTextBox1.Text += "Server Installation Initiated";
                        richTextBox1.Text += Environment.NewLine + "Creating Directories ✔";
                        richTextBox1.Text += Environment.NewLine + "Configuring Server Launch ✔";
                        richTextBox1.Text += Environment.NewLine + "Downloading SteamCMD ✔";
                        richTextBox1.Text += Environment.NewLine + "Extracting SteamCMD ✔";
                        richTextBox1.Text += Environment.NewLine + "Launching SteamCMD ✔";
                        richTextBox1.Text += Environment.NewLine + "Extracting SteamCMD Files ✔";

                        ProgressBarPercentage = random.Next(75, 82); SetProgressBarValue(); label2.Text = $"Step 13/15 - {ProgressBarPercentage}%";
                        #endregion

                        #region Log
                        await Task.Delay(LogDelay);
                        richTextBox1.Clear();
                        richTextBox1.Text += "Server Installation Initiated";
                        richTextBox1.Text += Environment.NewLine + "Creating Directories ✔";
                        richTextBox1.Text += Environment.NewLine + "Configuring Server Launch ✔";
                        richTextBox1.Text += Environment.NewLine + "Downloading SteamCMD ✔";
                        richTextBox1.Text += Environment.NewLine + "Extracting SteamCMD ✔";
                        richTextBox1.Text += Environment.NewLine + "Launching SteamCMD ✔";
                        richTextBox1.Text += Environment.NewLine + "Extracting SteamCMD Files ✔";
                        richTextBox1.Text += Environment.NewLine + "Downloading Server Files ";

                        ProgressBarPercentage = random.Next(82, 89); SetProgressBarValue(); label2.Text = $"Step 14/15 - {ProgressBarPercentage}%";
                        #endregion
                    }
                }


                if (Directory.Exists($"{Data.AppInstallerData.Default.TempServerRootPath}/{Data.AppInstallerData.Default.TempCurrentServer}/LiveServer/RustDedicated_Data") && File.Exists($"{Data.AppInstallerData.Default.TempServerRootPath}/{Data.AppInstallerData.Default.TempCurrentServer}/LiveServer/RustDedicated.exe"))
                {
                    StatusTimer.Stop();

                    #region Log
                    await Task.Delay(LogDelay);
                    richTextBox1.Clear();
                    richTextBox1.Text += "Server Installation Initiated";
                    richTextBox1.Text += Environment.NewLine + "Creating Directories ✔";
                    richTextBox1.Text += Environment.NewLine + "Configuring Server Launch ✔";
                    richTextBox1.Text += Environment.NewLine + "Downloading SteamCMD ✔";
                    richTextBox1.Text += Environment.NewLine + "Extracting SteamCMD ✔";
                    richTextBox1.Text += Environment.NewLine + "Launching SteamCMD ✔";
                    richTextBox1.Text += Environment.NewLine + "Extracting SteamCMD Files ✔";
                    richTextBox1.Text += Environment.NewLine + "Downloading Server Files ✔";

                    ProgressBarPercentage = random.Next(89, 93); SetProgressBarValue(); label2.Text = $"Step 15/15 - {ProgressBarPercentage}%";
                    #endregion

                    await Task.Delay(LogDelay);

                    ProgressBarPercentage = random.Next(93, 97); SetProgressBarValue(); label2.Text = $"Step Final - {ProgressBarPercentage}%";

                    InstallationComplete();
                }
            }
            else { StopInstallationFunction(); }

        }

        async void InstallationComplete()
        {
            if (!StopInstallation)
            {
                Data.AppData.Default.RootFolder = Data.AppInstallerData.Default.TempServerRootPath;
                Data.AppData.Default.CurrentServer = Data.AppInstallerData.Default.TempCurrentServer;
                Data.AppData.Default.CurrentServerIndex = Data.AppInstallerData.Default.TempServerIndex;
                Data.AppData.Default.Save();

                //Rename Files
                try
                {
                    System.IO.File.Move($"{Data.AppData.Default.RootFolder}/{Data.AppData.Default.CurrentServer}/LiveServer/RustDedicated.exe", $"{Data.AppData.Default.RootFolder}/{Data.AppData.Default.CurrentServer}/LiveServer/RustDedicated{Data.AppData.Default.CurrentServerIndex}.exe");
                    System.IO.Directory.Move($"{Data.AppData.Default.RootFolder}/{Data.AppData.Default.CurrentServer}/LiveServer/RustDedicated_Data", $"{Data.AppData.Default.RootFolder}/{Data.AppData.Default.CurrentServer}/LiveServer/RustDedicated{Data.AppData.Default.CurrentServerIndex}_Data");
                }
                catch { }
                await Task.Delay(LogDelay);
                ColorRichText("Server Installation Complete", Color.FromArgb(50, 250, 50), true);


                ProgressBarPercentage = 100; SetProgressBarValue(); label2.Text = $"Server Installation Complete - {ProgressBarPercentage}%";

                EnableButtons();
                Start_ServerPathChecker();
            }
            else { StopInstallationFunction(); }

        }
        #endregion

        #region Update Server Code
        private async void customButton2_Click(object sender, EventArgs e)
        {
            Stop_ServerPathChecker();
            DisableButtons(false);

            #region Log
            richTextBox1.Clear();
            richTextBox1.Text += "Server Update Initiated";
            await Task.Delay(LogDelay);
            richTextBox1.Text += Environment.NewLine + "Preparing Directories";

            ProgressBarPercentage = random.Next(0, 16); SetProgressBarValue(); label2.Text = $"Step 1/6 - {ProgressBarPercentage}%";
            #endregion

            //Rename Files
            System.IO.File.Move($"{Data.AppData.Default.RootFolder}/{Data.AppData.Default.CurrentServer}/LiveServer/RustDedicated{Data.AppData.Default.CurrentServerIndex}.exe", $"{Data.AppData.Default.RootFolder}/{Data.AppData.Default.CurrentServer}/LiveServer/RustDedicated.exe");
            System.IO.Directory.Move($"{Data.AppData.Default.RootFolder}/{Data.AppData.Default.CurrentServer}/LiveServer/RustDedicated{Data.AppData.Default.CurrentServerIndex}_Data", $"{Data.AppData.Default.RootFolder}/{Data.AppData.Default.CurrentServer}/LiveServer/RustDedicated_Data");

            #region Log
            await Task.Delay(LogDelay);
            richTextBox1.Clear();
            richTextBox1.Text += "Server Update Initiated";
            richTextBox1.Text += Environment.NewLine + "Preparing Directories ✔";

            ProgressBarPercentage = random.Next(16, 32); SetProgressBarValue(); label2.Text = $"Step 2/6 - {ProgressBarPercentage}%";
            #endregion

            #region Log
            await Task.Delay(LogDelay);
            richTextBox1.Clear();
            richTextBox1.Text += "Server Update Initiated";
            richTextBox1.Text += Environment.NewLine + "Preparing Directories ✔";
            richTextBox1.Text += Environment.NewLine + "Launching SteamCMD";

            ProgressBarPercentage = random.Next(32, 48); SetProgressBarValue(); label2.Text = $"Step 3/6 - {ProgressBarPercentage}%";
            #endregion

            //Install Rust Server With Args
            string LaunchArgs = "+login anonymous +force_install_dir " + $"{Data.AppData.Default.RootFolder}/{Data.AppData.Default.CurrentServer}/LiveServer" + "/" + " +app_update 258550 validate +quit";
            Process.Start($"{Data.AppData.Default.RootFolder}/{Data.AppData.Default.CurrentServer}/SteamCMD/steamcmd.exe", LaunchArgs);

            #region Log
            await Task.Delay(LogDelay);
            richTextBox1.Clear();
            richTextBox1.Text += "Server Update Initiated";
            richTextBox1.Text += Environment.NewLine + "Preparing Directories ✔";
            richTextBox1.Text += Environment.NewLine + "Launching SteamCMD ✔";

            ProgressBarPercentage = random.Next(48, 64); SetProgressBarValue(); label2.Text = $"Step 4/6 - {ProgressBarPercentage}%";
            #endregion

            #region Log
            await Task.Delay(LogDelay);
            richTextBox1.Clear();
            richTextBox1.Text += "Server Update Initiated";
            richTextBox1.Text += Environment.NewLine + "Preparing Directories ✔";
            richTextBox1.Text += Environment.NewLine + "Launching SteamCMD ✔";
            richTextBox1.Text += Environment.NewLine + "Updating Server Files";

            ProgressBarPercentage = random.Next(64, 80); SetProgressBarValue(); label2.Text = $"Step 5/6 - {ProgressBarPercentage}%";
            #endregion

            UpdateTimer.Start();
        }

        int tempvalue = 0;



        private async void UpdateTimer_Tick(object sender, EventArgs e)
        {   
            const string ProcessName = "steamcmd";
            Process CustomProcess = Process.GetProcessesByName(ProcessName).FirstOrDefault();
            if (CustomProcess != null && CustomProcess.Responding) { }
            else // Process Not Running
            {
                UpdateTimer.Stop();
                //Rename Files

                System.IO.File.Move($"{Data.AppData.Default.RootFolder}/{Data.AppData.Default.CurrentServer}/LiveServer/RustDedicated.exe", $"{Data.AppData.Default.RootFolder}/{Data.AppData.Default.CurrentServer}/LiveServer/RustDedicated{Data.AppData.Default.CurrentServerIndex}.exe");
                System.IO.Directory.Move($"{Data.AppData.Default.RootFolder}/{Data.AppData.Default.CurrentServer}/LiveServer/RustDedicated_Data", $"{Data.AppData.Default.RootFolder}/{Data.AppData.Default.CurrentServer}/LiveServer/RustDedicated{Data.AppData.Default.CurrentServerIndex}_Data");

                #region Log
                await Task.Delay(LogDelay);
                richTextBox1.Clear();
                richTextBox1.Text += "Server Update Initiated";
                richTextBox1.Text += Environment.NewLine + "Preparing Directories ✔";
                richTextBox1.Text += Environment.NewLine + "Launching SteamCMD ✔";
                richTextBox1.Text += Environment.NewLine + "Updating Server Files ✔";

                ProgressBarPercentage = random.Next(80, 96); SetProgressBarValue(); label2.Text = $"Step 6/6- {ProgressBarPercentage}%";
                #endregion

                //DONE
                ServerUpdateComplete();

            }
        }

        void ServerUpdateComplete()
        {

            ColorRichText("Server Update Complete", Color.FromArgb(50, 250, 50), true);
            ProgressBarPercentage = 100; SetProgressBarValue(); label2.Text = $"Server Update Complete - {ProgressBarPercentage}%";
            EnableButtons();
            Start_ServerPathChecker();

        }

        #endregion

        #region Oxide Download Code
        private async void customButton3_Click(object sender, EventArgs e)
        {
            
            try
            {


                Stop_ServerPathChecker();
                DisableButtons(false);


                #region Oxide Download Zip
                using (var client = new WebClient())
                {

                    #region Log
                    richTextBox1.Clear();
                    richTextBox1.Text += "Oxide Installation Initiated";
                    await Task.Delay(LogDelay);
                    richTextBox1.Text += Environment.NewLine + "Downloading Oxide";

                    ProgressBarPercentage = random.Next(0, 14); SetProgressBarValue(); label2.Text = $"Step 1/10 - {ProgressBarPercentage}%";
                    #endregion

                    //Download  Zip
                    client.DownloadFile("https://umod.org/games/rust/download", $"{Data.AppData.Default.RootFolder}/{Data.AppData.Default.CurrentServer}/LiveServer/oxide.zip");

                    #region Log
                    await Task.Delay(LogDelay);
                    richTextBox1.Clear();
                    richTextBox1.Text += "Oxide Installation Initiated";
                    richTextBox1.Text += Environment.NewLine + "Downloading Oxide ✔";

                    ProgressBarPercentage = random.Next(14, 24); SetProgressBarValue(); label2.Text = $"Step 2/10 - {ProgressBarPercentage}%";
                    #endregion
                }
                #endregion

                #region Rename Files

                #region Log
                await Task.Delay(LogDelay);
                richTextBox1.Clear();
                richTextBox1.Text += "Oxide Installation Initiated";
                richTextBox1.Text += Environment.NewLine + "Downloading Oxide ✔";
                richTextBox1.Text += Environment.NewLine + "Preparing Directories";

                ProgressBarPercentage = random.Next(24, 35); SetProgressBarValue(); label2.Text = $"Step 3/10 - {ProgressBarPercentage}%";
                #endregion

                if (!Directory.Exists($"{Data.AppData.Default.RootFolder}/{Data.AppData.Default.CurrentServer}/LiveServer/RustDedicated_Data"))
                {
                    System.IO.Directory.Move($"{Data.AppData.Default.RootFolder}/{Data.AppData.Default.CurrentServer}/LiveServer/RustDedicated{Data.AppData.Default.CurrentServerIndex}_Data", $"{Data.AppData.Default.RootFolder}/{Data.AppData.Default.CurrentServer}/LiveServer/RustDedicated_Data");
                }
                else
                {
                    System.IO.Directory.Move($"{Data.AppData.Default.RootFolder}/{Data.AppData.Default.CurrentServer}/LiveServer/RustDedicated{Data.AppData.Default.CurrentServerIndex}_Data", $"{Data.AppData.Default.RootFolder}/{Data.AppData.Default.CurrentServer}/LiveServer/RustDedicated_Data");
                }

                #region Log
                await Task.Delay(LogDelay);
                richTextBox1.Clear();
                richTextBox1.Text += "Oxide Installation Initiated";
                richTextBox1.Text += Environment.NewLine + "Downloading Oxide ✔";
                richTextBox1.Text += Environment.NewLine + "Preparing Directories ✔";

                ProgressBarPercentage = random.Next(35, 46); SetProgressBarValue(); label2.Text = $"Step 4/10 - {ProgressBarPercentage}%";
                #endregion
                #endregion


                #region Extracting Zip

                #region Log
                await Task.Delay(LogDelay);
                richTextBox1.Clear();
                richTextBox1.Text += "Oxide Installation Initiated";
                richTextBox1.Text += Environment.NewLine + "Downloading Oxide ✔";
                richTextBox1.Text += Environment.NewLine + "Preparing Directories ✔";
                richTextBox1.Text += Environment.NewLine + "Extracting Oxide";

                ProgressBarPercentage = random.Next(46, 54); SetProgressBarValue(); label2.Text = $"Step 5/10 - {ProgressBarPercentage}%";
                #endregion

                ZipFile.ExtractToDirectory($"{Data.AppData.Default.RootFolder}/{Data.AppData.Default.CurrentServer}/LiveServer/oxide.zip", $"{Data.AppData.Default.RootFolder}/{Data.AppData.Default.CurrentServer}/LiveServer", true);

                #region Log
                await Task.Delay(LogDelay);
                richTextBox1.Clear();
                richTextBox1.Text += "Oxide Installation Initiated";
                richTextBox1.Text += Environment.NewLine + "Downloading Oxide ✔";
                richTextBox1.Text += Environment.NewLine + "Preparing Directories ✔";
                richTextBox1.Text += Environment.NewLine + "Extracting Oxide ✔";

                ProgressBarPercentage = random.Next(54, 62); SetProgressBarValue(); label2.Text = $"Step 6/10 - {ProgressBarPercentage}%";
                #endregion

                #endregion

                #region Rename Files Reverse

                #region Log
                await Task.Delay(LogDelay);
                richTextBox1.Clear();
                richTextBox1.Text += "Oxide Installation Initiated";
                richTextBox1.Text += Environment.NewLine + "Downloading Oxide ✔";
                richTextBox1.Text += Environment.NewLine + "Preparing Directories ✔";
                richTextBox1.Text += Environment.NewLine + "Extracting Oxide ✔";
                richTextBox1.Text += Environment.NewLine + "Updating Directories";

                ProgressBarPercentage = random.Next(62, 72); SetProgressBarValue(); label2.Text = $"Step 7/10 - {ProgressBarPercentage}%";
                #endregion

                if (!Directory.Exists($"{Data.AppData.Default.RootFolder}/{Data.AppData.Default.CurrentServer}/LiveServer/RustDedicated{Data.AppData.Default.CurrentServerIndex}_Data"))
                {
                    System.IO.Directory.Move($"{Data.AppData.Default.RootFolder}/{Data.AppData.Default.CurrentServer}/LiveServer/RustDedicated_Data", $"{Data.AppData.Default.RootFolder}/{Data.AppData.Default.CurrentServer}/LiveServer/RustDedicated{Data.AppData.Default.CurrentServerIndex}_Data");
                }
                else
                {
                    MessageBox.Show($"RustDedicated_Data is Corrupt {Environment.NewLine} Please Delete The Corrupt One", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                #region Log
                await Task.Delay(LogDelay);
                richTextBox1.Clear();
                richTextBox1.Text += "Oxide Installation Initiated";
                richTextBox1.Text += Environment.NewLine + "Downloading Oxide ✔";
                richTextBox1.Text += Environment.NewLine + "Preparing Directories ✔";
                richTextBox1.Text += Environment.NewLine + "Extracting Oxide ✔";
                richTextBox1.Text += Environment.NewLine + "Updating Directories ✔";

                ProgressBarPercentage = random.Next(72, 83); SetProgressBarValue(); label2.Text = $"Step 8/10 - {ProgressBarPercentage}%";
                #endregion

                #endregion

                #region Log
                await Task.Delay(LogDelay);
                richTextBox1.Clear();
                richTextBox1.Text += "Oxide Installation Initiated";
                richTextBox1.Text += Environment.NewLine + "Downloading Oxide ✔";
                richTextBox1.Text += Environment.NewLine + "Preparing Directories ✔";
                richTextBox1.Text += Environment.NewLine + "Extracting Oxide ✔";
                richTextBox1.Text += Environment.NewLine + "Updating Directories ✔";
                richTextBox1.Text += Environment.NewLine + "Finalizing";


                ProgressBarPercentage = random.Next(83, 92); SetProgressBarValue(); label2.Text = $"Step 9/10 - {ProgressBarPercentage}%";
                #endregion

                File.Delete($"{Data.AppData.Default.RootFolder}/{Data.AppData.Default.CurrentServer}/LiveServer/oxide.zip");

                #region Log
                await Task.Delay(LogDelay);
                richTextBox1.Clear();
                richTextBox1.Text += "Oxide Installation Initiated";
                richTextBox1.Text += Environment.NewLine + "Downloading Oxide ✔";
                richTextBox1.Text += Environment.NewLine + "Preparing Directories ✔";
                richTextBox1.Text += Environment.NewLine + "Extracting Oxide ✔";
                richTextBox1.Text += Environment.NewLine + "Updating Directories ✔";
                richTextBox1.Text += Environment.NewLine + "Finalizing ✔";

                ProgressBarPercentage = random.Next(92, 100); SetProgressBarValue(); label2.Text = $"Step 10/10 - {ProgressBarPercentage}%";
                #endregion


                // Oxide Done
                OxideComplete();

            }
            catch
            {

                ColorRichText("Oxide Installation Failed", Color.FromArgb(250, 50, 50), true);
                ProgressBarPercentage = 0; SetProgressBarValue();
                label2.Text = "Oxide Installation Failed";
                MessageBox.Show("Oxide installation Failed", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                EnableButtons();
                Start_ServerPathChecker();

            }




        }

        async void OxideComplete()
        {

            #region Log
            await Task.Delay(LogDelay);
            richTextBox1.Clear();
            richTextBox1.Text += "Oxide Installation Initiated";
            richTextBox1.Text += Environment.NewLine + "Downloading Oxide ✔";
            richTextBox1.Text += Environment.NewLine + "Preparing Directories ✔";
            richTextBox1.Text += Environment.NewLine + "Extracting Oxide ✔";
            richTextBox1.Text += Environment.NewLine + "Updating Directories ✔";
            richTextBox1.Text += Environment.NewLine + "Finalizing ✔";
            await Task.Delay(LogDelay);

            ColorRichText("Oxide Installation Complete", Color.FromArgb(50, 250, 50), true);

            ProgressBarPercentage = 100; SetProgressBarValue(); label2.Text = $"Oxide Installation Complete - {ProgressBarPercentage}%";
            #endregion

            EnableButtons();
            Start_ServerPathChecker();

        }

        #endregion










        private void OnDispose(object sender, EventArgs e)
        {
            ServerPathChecker.CancelAsync();
            DataCheckerTimer.Stop();
            StatusTimer.Stop();
            UpdateTimer.Stop();
       
            Dispose();
        }


        #region Fix Richtextbox Scrolling
        private void richTextBox1_ContentsResized(object sender, ContentsResizedEventArgs e)
        {
            if (richTextBox1.ZoomFactor != 1)
            {

                richTextBox1.SelectAll();
                richTextBox1.ZoomFactor = 1.0f;
                richTextBox1.DeselectAll();

            }
        }
        #endregion



        #region PathChecker Worker

        void Stop_ServerPathChecker()
        {
            ServerPathChecker.CancelAsync();
            ServerPathChecker.Dispose();
        }
        void Start_ServerPathChecker()
        {
            ServerPathChecker.RunWorkerAsync();
        }

        private async void ServerPathChecker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

            await Task.Delay(1);
            CheckServers();
            if (ServerPathChecker.CancellationPending != true)
            {
                ServerPathChecker.CancelAsync();
                ServerPathChecker.Dispose();
                ServerPathChecker.RunWorkerAsync();
            }

        }
        #endregion
    }
}
