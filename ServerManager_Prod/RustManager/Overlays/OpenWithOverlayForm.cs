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

namespace RustManager.Overlays
{
    public partial class OpenWithOverlayForm : Form
    {
        public OpenWithOverlayForm()
        {
            InitializeComponent();
        }

        private void OpenWithOverlayForm_Load(object sender, EventArgs e)
        {

        }

        private void customButton1_Click(object sender, EventArgs e)
        {
            Close();
        }

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

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
          

            try
            {
                if (comboBox1.SelectedIndex == 0)
                {
                    var proc1 = new ProcessStartInfo();
                    proc1.FileName = "cmd.exe";
                    proc1.Arguments = "/c " + $"Code {Data.AppData.Default.RootFolder}/{Data.AppData.Default.CurrentServer}/Start.bat";
                    proc1.CreateNoWindow = true;
                    Process.Start(proc1);
                }
                if (comboBox1.SelectedIndex == 1)
                {
                    var proc1 = new ProcessStartInfo();
                    proc1.FileName = "cmd.exe";
                    proc1.Arguments = "/c " + $"Start notepad++ {Data.AppData.Default.RootFolder}/{Data.AppData.Default.CurrentServer}/Start.bat";
                    proc1.CreateNoWindow = true;
                    Process.Start(proc1);
                }
                if (comboBox1.SelectedIndex == 2)
                {
                    var proc1 = new ProcessStartInfo();
                    proc1.FileName = "cmd.exe";
                    proc1.Arguments = "/c " + $"Notepad {Data.AppData.Default.RootFolder}/{Data.AppData.Default.CurrentServer}/Start.bat";
                    proc1.CreateNoWindow = true;
                    Process.Start(proc1);
                }
            }
            catch 
            {
                if (comboBox1.SelectedIndex == 0) { MessageBox.Show("Visual Studio Code - Launch Error", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                if (comboBox1.SelectedIndex == 1) { MessageBox.Show("Notepad++ - Launch Error", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                if (comboBox1.SelectedIndex == 2) { MessageBox.Show("Default Editor - Launch Error", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            }



            Close();

    
        }
    }
}
