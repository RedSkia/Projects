using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RustManager.UserControls.SubControls
{
    public partial class Restart : UserControl
    {
        public Restart()
        {
            InitializeComponent();
        }

        private void Restart_Load(object sender, EventArgs e)
        {
            ProgressPanel1.Width = 0;
            TheLoop();
        }

        #region Interface Functions
        void HideStartButton()
        {
            customButton1.Enabled = false;
            customButton1.Image = Data.Icons.Restart32G;
            textBox1.Enabled = false;
        }
        void ShowStartButton()
        {
            customButton1.Enabled = true;
            customButton1.Image = Data.Icons.Restart32W;
            textBox1.Enabled = true;

        }
        #endregion

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (Convert.ToInt32(textBox1.Text) >= 3600)
                {
                    textBox1.Text = "3600";
                    label1.Text = "60m:0s";
                }
                else
                {
                    int seconds = Convert.ToInt32(textBox1.Text);
                    var s = string.Format("{0:0}m:{1:0}s", (seconds / 60) % 60, seconds % 60);
                    label1.Text = s;
                }
            }
            catch { }
        }



        #region Restart Start
        int ProgressBarPercentage = 0;
        private void customButton1_Click(object sender, EventArgs e)
        {

            try
            {
                if (!String.IsNullOrEmpty(textBox1.Text))
                {
                    if (Convert.ToInt32(textBox1.Text) < 60)
                    {
                        textBox1.Text = "60";
                        label1.Text = "1m:0s";
                        MessageBox.Show($"60 Seconds Is The Minimum Restart Delay", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    else
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
                                    HideStartButton();
                                    // Goes TO MainForm Sub Functions
                                    Data.RestartData.Default.PID = id;
                                    Data.RestartData.Default.RestartSeconds = Convert.ToInt32(textBox1.Text);
                                    Data.RestartData.Default.RestartEnabled = true;
                                }
                            }
                        }
                        TheLoop();
                    }

                } 
                else
                {
                    textBox1.Text = "60";
                    label1.Text = "1m:0s";
                    MessageBox.Show($"60 Seconds Is The Minimum Restart Delay", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                
            } catch { }
        }

        #endregion

        async void TheLoop()
        {
            if (Data.RestartData.Default.RestartEnabled)
            {
    
                while (Data.RestartData.Default.RestartEnabled)
                {
                    await Task.Delay(1);
                    if (!Data.RestartData.Default.RestartEnabled || Data.DataLog.Default.RestartTimeLeft <= 0)
                    {
                        ProgressPanel1.Dock = DockStyle.Fill;
                        ShowStartButton();
                        label1.Text = "0m:0s";
                        label3.Text = "100%";
                        ProgressPanel1.Width = Convert.ToInt32(ProgressPanel2.Width * 100 / 100);

                        break;
                    }

                    HideStartButton();

                    ProgressBarPercentage = (int)Math.Round((double)(100 * Data.DataLog.Default.RestartTimeProcessed) / Data.RestartData.Default.RestartSeconds);




                    #region SetProgressBar
                    ProgressPanel1.Dock = DockStyle.None;
                    ProgressPanel1.Width = Convert.ToInt32(ProgressPanel2.Width * ProgressBarPercentage / 100);
                    #endregion

                    label3.Text = $"{ProgressBarPercentage}%";
                    var s = string.Format("{0:0}m:{1:0}s", (Data.DataLog.Default.RestartTimeLeft / 60) % 60, Data.DataLog.Default.RestartTimeLeft % 60);

                    label1.Text = s;
                }
            }
           
        }

    }
}
