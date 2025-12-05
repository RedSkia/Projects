using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RustManager.Overlays
{
    public partial class InstallerOverlayForm : Form
    {
        public InstallerOverlayForm()
        {
            InitializeComponent();
        }


        const string quote = "\"";

        private void InstallerOverlayForm_Load(object sender, EventArgs e)
        {

        }

        #region Interface Buttons
        private void customButton1_Click(object sender, EventArgs e)
        {
            Close();
        }
        #endregion

        private async void CheckActiveStateTimer_Tick(object sender, EventArgs e)
        {
            await Task.Delay(1);

            if (Data.AppInfo.Default.AppActive == true)
            {
                TopMost = true;

            }
            else if (Data.AppInfo.Default.AppActive == false)
            {
                TopMost = false;

            }
        }

        //Save Server Button
        private void customButton4_Click(object sender, EventArgs e)
        {
            CheckActiveStateTimer.Stop();
            TopMost = false;

            if (!String.IsNullOrEmpty(textBox1.Text) && Directory.Exists(textBox1.Text))
            {

            }
            else { MessageBox.Show("Installer Path Invalid", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }

            //If Browse Server Installed
            if (!String.IsNullOrEmpty(textBox1.Text))
            {
                if (Directory.Exists($"{textBox1.Text}"))
                {
                    if (Directory.Exists($"{textBox1.Text}/LiveServer"))
                    {
                        if (Directory.Exists($"{textBox1.Text}/SteamCMD"))
                        {
                            if (File.Exists($"{textBox1.Text}/Start.bat"))
                            {
                                string TempRootPath = textBox1.Text;

                                //Splitting Function
                                var SplitPath = TempRootPath.Split(new[] { "\\" }, StringSplitOptions.None);


                                //Get Number
                                string StringInput = $"{SplitPath[0]}/{SplitPath[1]}/{SplitPath[2]}/{SplitPath[3]}";
                                string StringOutput = string.Empty;

                                for (int i = 0; i < StringInput.Length; i++)
                                {
                                    if (Char.IsDigit(StringInput[i]))
                                        StringOutput += StringInput[i];
                                }

                                //Getting Splitting Data
                                Data.AppData.Default.RootFolder = $"{SplitPath[0]}/{SplitPath[1]}/{SplitPath[2]}";
                                Data.AppData.Default.CurrentServer = $"{SplitPath[0]}/{SplitPath[1]}/{SplitPath[2]}/{SplitPath[3]}";
                                Data.AppData.Default.CurrentServerIndex = Convert.ToInt32(StringOutput);

                                Data.AppData.Default.Save();
                                MessageBox.Show($"Server Root Saved", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                //MessageBox.Show($"ROOT \n{$"{SplitPath[0]}/{SplitPath[1]}/{SplitPath[2]}"} \n\nCurrent Serv \n{SplitPath[0]}/{SplitPath[1]}/{SplitPath[2]}/{SplitPath[3]} \n\nCurrent Index \n{StringOutput}");



                            }
                            else { MessageBox.Show($"Missing File {quote}Start.bat{quote}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                        }
                        else { MessageBox.Show($"Missing Folder {quote}SteamCMD{quote}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                    }
                    else { MessageBox.Show($"Missing Folder {quote}LiveServer{quote}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                } 
                else { MessageBox.Show($"Installer Path Invalid", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            } 
            else { MessageBox.Show($"Empty Path", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }



        }







        //Installer Button
        private void customButton3_Click(object sender, EventArgs e)
        {
            CheckActiveStateTimer.Stop();
            TopMost = false;
            
            if (!String.IsNullOrEmpty(textBox1.Text) && Directory.Exists(textBox1.Text))
            {
                if (numericUpDown1.Value > 0)
                {
                    if (!Directory.Exists($"{textBox1.Text}/Server{numericUpDown1.Value}"))
                    {
                        //Goes To Installer  DataCheckerTimer
                        Data.AppInstallerData.Default.TempServerRootPath = textBox1.Text;
                        Data.AppInstallerData.Default.TempCurrentServer = $"Server{numericUpDown1.Value}";
                        Data.AppInstallerData.Default.TempServerIndex = Convert.ToInt32(numericUpDown1.Value);
                        Close();

                    }
                    else { MessageBox.Show("Server Allready Exists", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);}
                }
                else { MessageBox.Show("Index Invalid", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);}
            }
            else { MessageBox.Show("Installer Path Invalid", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);}
            
   

        }

        private void customButton2_Click(object sender, EventArgs e)
        {
            CheckActiveStateTimer.Stop();
            TopMost = false;

            OpenFileDialog folderBrowser = new OpenFileDialog();
            // Set validate names and check file exists to false otherwise windows will
            // not let you select "Folder Selection."
            folderBrowser.ValidateNames = false;
            folderBrowser.CheckFileExists = false;
            folderBrowser.CheckPathExists = true;
            // Always default to Folder Selection.
            folderBrowser.FileName = "";
            if (folderBrowser.ShowDialog() == DialogResult.OK)
            {
                string folderPath = Path.GetDirectoryName(folderBrowser.FileName);
                textBox1.Text = folderPath.ToLower();

                CheckActiveStateTimer.Start();
                TopMost = true;
            }
     
        }

    }
}
