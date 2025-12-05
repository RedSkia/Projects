using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RustManager.UserControls
{
    public partial class Home : UserControl
    {
        public Home()
        {
            InitializeComponent();


            Disposed += OnDispose;

        }




        private void Home_Load(object sender, EventArgs e)
        {


            


            //DataList.Add(new Tuple<string, string, string>("dd", "something", "something"));


            //ReloadData();

            dataGridView1.ForeColor = Color.Black;

            label1.Text = Data.AppData.Default.CurrentServer;

            if (InfoWorker.IsBusy != true)
            {
                InfoWorker.RunWorkerAsync();
            }


            if (ServerAliveWorker.IsBusy != true)
            {
                ServerAliveWorker.RunWorkerAsync();
            }

            if (ServerStalledWorker.IsBusy != true)
            {
                ServerStalledWorker.RunWorkerAsync();
            }
        }




        #region Interface Buttons
        private void customButton10_Click(object sender, EventArgs e)
        {
            var proc1 = new ProcessStartInfo();
            proc1.FileName = "cmd.exe";
            proc1.Arguments = "/c " + $"start {Data.AppData.Default.RootFolder}/{Data.AppData.Default.CurrentServer}";
            proc1.CreateNoWindow = true;
            Process.Start(proc1);
        }
        #endregion


        string CurrentWindow = "";
        #region Active Window
        async void ActiveWindow()
        {

            foreach (Control i in DisplayPanel.Controls)
            {
                i.Dispose();
            }

            if (CurrentWindow == "Logs")
            {
                UserControls.SubControls.Logs NextWindow = new UserControls.SubControls.Logs() { Dock = DockStyle.Fill };
                DisplayPanel.Controls.Clear();

                await Task.Delay(1);
                DisplayPanel.Controls.Add(NextWindow);
                NextWindow.Show();

            }


            if (CurrentWindow == "Restart")
            {
                UserControls.SubControls.Restart NextWindow = new UserControls.SubControls.Restart() { Dock = DockStyle.Fill };
                DisplayPanel.Controls.Clear();

                await Task.Delay(1);
                DisplayPanel.Controls.Add(NextWindow);
                NextWindow.Show();

            }

            if (CurrentWindow == "Console")
            {
                UserControls.SubControls.Console NextWindow = new UserControls.SubControls.Console() { Dock = DockStyle.Fill };
                DisplayPanel.Controls.Clear();

                await Task.Delay(1);
                DisplayPanel.Controls.Add(NextWindow);
                NextWindow.Show();

            }

            if (CurrentWindow == "Backup")
            {
                UserControls.SubControls.Backup NextWindow = new UserControls.SubControls.Backup() { Dock = DockStyle.Fill };
                DisplayPanel.Controls.Clear();

                await Task.Delay(1);
                DisplayPanel.Controls.Add(NextWindow);
                NextWindow.Show();

            }
        }
        #endregion

        #region MainButtons 
        //Start Server
        private void customButton1_Click(object sender, EventArgs e)
        {
            Process.Start($"{Data.AppData.Default.RootFolder}\\{Data.AppData.Default.CurrentServer}\\Start.bat", $"{Data.AppData.Default.RootFolder}\\{Data.AppData.Default.CurrentServer}");
        }


        //Restart Current Server
        private void customButton2_Click(object sender, EventArgs e)
        {
            if (CurrentWindow != "Restart")
            {
                CurrentWindow = "Restart";
                ActiveWindow();
            }
        }


        // Edit Start.bat With CUSTOM Edit program
        private void customButton9_Click(object sender, EventArgs e)
        {
            FunctionForms.EditorForm NextForm = new FunctionForms.EditorForm($"{Data.AppData.Default.RootFolder}/{Data.AppData.Default.CurrentServer}/Start.bat", "Start.bat");
            NextForm.Show();

        }



        #endregion















































        void ReloadData()
        {
            int TempIndex = 0;

            richTextBox1.Clear();
            //DataList.Clear();
  
            foreach (string ServerFolder in Directory.GetDirectories(Data.AppData.Default.RootFolder))
            {
                try
                {

                    foreach (var SavFile in Directory.GetFiles($"{Data.AppData.Default.RootFolder}\\{Path.GetFileName(ServerFolder)}\\LiveServer\\server\\SERVER", "*.sav.*"))
                    {

           

                        FileInfo fs = new FileInfo(SavFile);
                        long filesize = fs.Length;

                        //DataList.Add(new Tuple<string, DateTime, long>(Path.GetFileName(ServerFolder), File.GetLastWriteTime(SavFile), GetFileSizeBytes(SavFile)));

                        //richTextBox1.Text += ($"{DataList[TempIndex].Item1}\n{DataList[TempIndex].Item2}\n{File.GetLastWriteTime(SavFile)}\n\n");
                        TempIndex++;
                    }
                } catch { }
            }

        }
        



        // Def Ticks Every 1 min Stall Limit 20 min
        int StalledMaxValue = 20;
        int StalledTickEveryMins = 1; 
        int StalledValue = 0;


        //List<Tuple<string, DateTime, long>> DataList = new List<Tuple<string, DateTime, long>>();

        List<Tuple<string, int>> StalledList = new List<Tuple<string, int>>();

















        /// <summary>
        /// <para>Item1 = Server</para>
        /// <para>Item2 = oldDate</para>
        /// <para>Item3 = newDate</para>
        /// <para>Item4 = StalledPoints</para>
        /// </summary>
        List<Tuple<string>> ServerList = new List<Tuple<string>>();
        List<Tuple<DateTime>> ServerDateList = new List<Tuple<DateTime>>();
        //List<Tuple<string>> ServerStalledPointsList = new List<Tuple<string>>();

        int IndexCounter1 = 0;
        bool Loaded = false;


        List<string> foo = new List<string>();
        private void customButton11_Click(object sender, EventArgs e)
        {
           
            dataGridView1.Rows.Clear();
            dataGridView1.Refresh();
            IndexCounter1 = 0;

            if(!Loaded)
            {
                // Count Servers Set Static Value
                foreach (string Server in Directory.GetDirectories(Data.AppData.Default.RootFolder))
                {
                    ServerList.Add(Tuple.Create(Path.GetFileName(Server)));
                }
                Loaded = true;
            }

     

            foreach (var Server in ServerList)
            {
                //label3.Text = ServerStalledPointsList[IndexCounter1].Item1.ToString();
                foreach (var SavFile in Directory.GetFiles($"{Data.AppData.Default.RootFolder}\\{Server.Item1}\\LiveServer\\server\\SERVER", "*.sav.*"))
                {
                   
           
                        ServerDateList.Insert(0, Tuple.Create(File.GetLastWriteTime(SavFile)));
         

                    if(ServerList[IndexCounter1].Item1 == Server.Item1)
                    {
     
                        dataGridView1.Rows.Add($"{ServerList[IndexCounter1].Item1}", $"{SavFile}", $"{File.GetLastWriteTime(SavFile)}", $"{ServerDateList[IndexCounter1].Item1}");


             

             

                        //Checks

                        if (ServerDateList[IndexCounter1].Item1 != File.GetLastWriteTime(SavFile))
                        {
                            ServerDateList.Insert(0, Tuple.Create(File.GetLastWriteTime(SavFile)));
        

                        }
                        



                    }
                }
                IndexCounter1++;
            }

    

            //MessageBox.Show(foo.Sum(x => Convert.ToInt32(x)).ToString());
            



            #region OLD
            /*
            await Task.Delay(StalledTickEveryMins * 600);

            label3.Text = $"{StalledValue}/{StalledMaxValue}";



            richTextBox1.Clear();



            if (StalledValue >= StalledMaxValue)
            {
                label3.Text = $"STALLED";
                break;
            }
            */
            /*
            foreach(string ServerFolder in Directory.GetDirectories(Data.AppData.Default.RootFolder))
            {
                try
                {

                    foreach (var SavFile in Directory.GetFiles($"{Data.AppData.Default.RootFolder}\\{Path.GetFileName(ServerFolder)}\\LiveServer\\server\\SERVER", "*.sav.*"))
                    {
                        StalledList.Add(new Tuple<string, int>(Path.GetFileName(ServerFolder), StalledValue));
                        DataList.Add(new Tuple<string, DateTime, long>(Path.GetFileName(ServerFolder), File.GetLastWriteTime(SavFile), GetFileSizeBytes(SavFile)));
                        if (DataList[IndexCounter].Item2 != File.GetLastWriteTime(SavFile))
                        {
                            StalledList.Add(new Tuple<string, int>(Path.GetFileName(ServerFolder), 0));
                        }

                        if (DataList[IndexCounter].Item3 != GetFileSizeBytes(SavFile))
                        {
                            StalledList.Add(new Tuple<string, int>(Path.GetFileName(ServerFolder), 0));
                        }

                        richTextBox1.Text += $"{StalledList[IndexCounter].Item2}\n{DataList[IndexCounter].Item1}\n{DataList[IndexCounter].Item2}\n{File.GetLastWriteTime(SavFile)}\n\n";

                        IndexCounter++;


                    }
                } catch { }

            }

                      StalledValue++;
            */


            #endregion



        }

        void ServerStalled()
        {

        }


        long GetFileSizeBytes(string FilePath)
        {
            FileInfo fs = new FileInfo(FilePath);
            long filesize = fs.Length;
            return filesize;
        }





        List<Tuple<string, int, string>> ServerDatabase = new List<Tuple<string, int, string>>();

        int StalledCounter = 0;
        int ServerIndex = 0;
        int CheckServerIndex = 0;
        int FileIndex = 0;
        int TestIndexCounter = 0;
        bool StalledServerDataLoaded = false;
        private void customButton12_Click(object sender, EventArgs e)
        {
     
            dataGridView1.DataSource = null;
            //ServerDatabase.Clear();
   
            ServerIndex = 0;
            TestIndexCounter = 0;
            FileIndex = 0;
            CheckServerIndex = 0;

            StalledCounter += 1;
            if(!StalledServerDataLoaded)
            {
                //Create Data
                foreach (string Server in Directory.GetDirectories(Data.AppData.Default.RootFolder))
                {
                    var FileDataSum = "";
                    foreach (var SavFile in Directory.GetFiles($"{Data.AppData.Default.RootFolder}\\{Path.GetFileName(Server)}\\LiveServer\\server\\SERVER", "*.sav.*"))
                    {
                        FileDataSum =  File.GetLastWriteTime(SavFile).ToString();
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
          
                if(ServerDatabase[CheckServerIndex].Item2 > 10)
                {
                    ServerDatabase[CheckServerIndex] = Tuple.Create(Path.GetFileName(Server), 0, FileDataValue.ToString());
                    MessageBox.Show($"{Server} Stalled !!");
                }

                CheckServerIndex++;
            }
            label3.Text = "Changed = " + TestIndexCounter;


            dataGridView1.Rows.Clear();
            dataGridView1.Refresh();
            dataGridView1.DataSource = ServerDatabase;


            StalledServerDataLoaded = true;
        }

        private void customButton7_Click(object sender, EventArgs e)
        {
            if (CurrentWindow != "Console")
            {
                CurrentWindow = "Console";
                ActiveWindow();
            }
        }

        private void customButton4_Click(object sender, EventArgs e)
        {
            if (CurrentWindow != "Backup")
            {
                CurrentWindow = "Backup";
                ActiveWindow();
            }
        }


        private void customButton5_Click(object sender, EventArgs e)
        {


            string title = "WARNING";
            string message = $"Do you want to wipe {Data.AppData.Default.CurrentServer} along with ALL server backups ?";
            MessageBoxButtons buttons = MessageBoxButtons.YesNo;

            DialogResult result = MessageBox.Show(message, title, buttons, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);

            
            if (result == DialogResult.Yes)
            {
                //Clears All Old Backups
                Directory.Delete($"{Data.AppData.Default.RootFolder}/{Data.AppData.Default.CurrentServer}/LiveServer/BACKUP", true);

                //Make a Temp Backup Before Wipe
                MakeBackupDir($"{Data.AppData.Default.RootFolder}/{Data.AppData.Default.CurrentServer}/LiveServer/server/SERVER", $"{Data.AppData.Default.RootFolder}/{Data.AppData.Default.CurrentServer}/LiveServer/BACKUP/WipeUndoBackup/Backup");

                //Wipe Server
                DirectoryInfo dir = new DirectoryInfo($"{Data.AppData.Default.RootFolder}/{Data.AppData.Default.CurrentServer}/LiveServer/server/SERVER");

                foreach (FileInfo file in dir.GetFiles())
                {
                    file.Delete();
                }

                //Wipe Plugin Data
                Directory.Delete($"{Data.AppData.Default.RootFolder}/{Data.AppData.Default.CurrentServer}/LiveServer/Oxide/Logs", true);
      
                MessageBox.Show("Server wiped", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);


            }

        }

        void MakeBackupDir(string sourceDir, string targetDir)
        {
            Directory.CreateDirectory(targetDir);

            foreach (var file in Directory.GetFiles(sourceDir))
                File.Copy(file, Path.Combine(targetDir, Path.GetFileName(file)), true);

            foreach (var directory in Directory.GetDirectories(sourceDir))
                MakeBackupDir(directory, Path.Combine(targetDir, Path.GetFileName(directory)));
        }

        private void customButton8_Click(object sender, EventArgs e)
        {

        }









        //On Dispose/Close
        private void OnDispose(object sender, EventArgs e)
        {

            InfoWorker.CancelAsync(); InfoWorker.Dispose();
            ServerStalledWorker.CancelAsync(); ServerStalledWorker.Dispose();
            ServerAliveWorker.CancelAsync(); ServerAliveWorker.Dispose();
            Dispose();




        }






















        private async void ServerAliveWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            #region Work
            if (Data.ServerDataLog.Default.ServerStatus == "Online")
            {
                pictureBox1.Image = Data.Icons.Online32;


                customTooltip1.SetToolTip(pictureBox1, "Online");
            }

            if (Data.ServerDataLog.Default.ServerStatus == "Responding")
            {
                pictureBox1.Image = Data.Icons.Starting32;

                customTooltip1.SetToolTip(pictureBox1, "Responding");
            }

            if (Data.ServerDataLog.Default.ServerStatus == "Offline")
            {
                pictureBox1.Image = Data.Icons.Offline32;

                customTooltip1.SetToolTip(pictureBox1, "Offline");
            }

            #endregion
            await Task.Delay(1);
            if (!ServerAliveWorker.CancellationPending)
            {
                ServerAliveWorker.RunWorkerAsync();
            }


        }

        private async void ServerStalledWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            label2.Text = $"{Data.ServerDataLog.Default.ServerStalledPoints}/{Data.ServerDataLog.Default.ServerStalledMaxPoints}";

            await Task.Delay(1);
            if (!ServerStalledWorker.CancellationPending)
            {
                ServerStalledWorker.RunWorkerAsync();
            }
        }
    
        private async void InfoWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

            label1.Text = Data.AppData.Default.CurrentServer;
            await Task.Delay(1);
            if (!InfoWorker.CancellationPending)
            {
                InfoWorker.RunWorkerAsync();
            }

        }

        private void customButton6_Click(object sender, EventArgs e)
        {
            if (CurrentWindow != "Logs")
            {
                CurrentWindow = "Logs";
                ActiveWindow();
            }
        }
    }
}

