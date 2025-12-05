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
    public partial class Console : UserControl
    {
        public Console()
        {
            InitializeComponent();
        }

        void Custom_Load()
        {
            richTextBox1.Clear();
            //Reset ConsoleMessages
            if (DateTime.Now.ToString("HH:mm") == "00:00")
            {
                Data.AppCollections.Default.ConsoleMessages.Clear();
            }

            comboBox1.SelectedIndex = 0;
            textBox1.AutoCompleteSource = AutoCompleteSource.CustomSource;

            AutoCompleteStringCollection data = new AutoCompleteStringCollection();
            foreach (var i in Data.AppCollections.Default.ConsoleCommandsCollection)
            {
                data.Add(i);
            }
            textBox1.AutoCompleteCustomSource = data;

            try
            {
                foreach (var i in Data.AppCollections.Default.ConsoleMessages)
                {
                    richTextBox1.ReadOnly = false;
                    richTextBox1.Text += i;
                    richTextBox1.ReadOnly = true;
                }
            }
            catch { }
        }
        private void Console_Load(object sender, EventArgs e)
        {


            Custom_Load();

            if (ServerCheckWorker.IsBusy != true)
            {
                ServerCheckWorker.RunWorkerAsync();
            }



        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(comboBox1.SelectedIndex == 0)
            {
                textBox1.AutoCompleteMode = AutoCompleteMode.None;
            }
            else if(comboBox1.SelectedIndex == 1)
            {
                textBox1.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            }
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

        async void ColorRichText(string TextToColor, Color Color, bool Newline)
        {

            await Task.Delay(10);
            if (Newline)
            {
                richTextBox1.ReadOnly = false;
                richTextBox1.Text += Environment.NewLine + TextToColor;
                richTextBox1.ReadOnly = true;

            }
            else if (!Newline)
            {
                richTextBox1.ReadOnly = false;
                richTextBox1.Text += TextToColor;
                richTextBox1.ReadOnly = true;
            }




            int index = richTextBox1.Text.IndexOf(TextToColor);

            int length = TextToColor.Length;

            richTextBox1.Select(index, length);
            richTextBox1.SelectionColor = Color;
            richTextBox1.DeselectAll();

        }


        const string EnterKey = "{Enter}";
        private void customButton1_Click(object sender, EventArgs e)
        {
   
            if (textBox1.Text != string.Empty)
            {
                if(richTextBox1.Text.Contains("SendKeys Failed"))
                {
                    richTextBox1.Clear();
                    customButton1.PerformClick();
                }
                else
                {
                    try
                    {

                        var processes = Process.GetProcesses().Where(p => p.MainWindowTitle == $"Server{Data.AppData.Default.CurrentServerIndex}");

                        if (processes.Count() > 0)
                        {
                            foreach (var process in processes)
                            {
                                var id = process.Id;
                                var Wintitle = process.MainWindowTitle;

                                if (process.MainWindowTitle == $"Server{Data.AppData.Default.CurrentServerIndex}")
                                {

                                    if (Process.GetProcessById(id).Responding)
                                    {
                                        if (comboBox1.SelectedIndex == 0)
                                        {
                                            if (richTextBox1.Text == string.Empty)
                                            {
                                                var Message = $"[{DateTime.Now.ToString("HH:mm")}] [Say] {textBox1.Text}";

                                                richTextBox1.ReadOnly = false;
                                                richTextBox1.Text += $"{Message}";
                                                richTextBox1.ReadOnly = true;
                                                FunctionClass.SendKeysClass.CustomSendKeys(Process.GetProcessById(id), $"Say {textBox1.Text}{EnterKey}");

                                                Data.AppCollections.Default.ConsoleMessages.Add($"{Message}");
                                            }
                                            else if (richTextBox1.Text != string.Empty)
                                            {
                                                var Message = $"{Environment.NewLine}[{DateTime.Now.ToString("HH:mm")}] [Say] {textBox1.Text}";


                                                richTextBox1.ReadOnly = false;
                                                richTextBox1.Text += $"{Message}";
                                                richTextBox1.ReadOnly = true;
                                                FunctionClass.SendKeysClass.CustomSendKeys(Process.GetProcessById(id), $"Say {textBox1.Text}{EnterKey}");

                                                Data.AppCollections.Default.ConsoleMessages.Add($"{Message}");
                                            }


                                        }
                                        if (comboBox1.SelectedIndex == 1)
                                        {
                                            if (richTextBox1.Text == string.Empty)
                                            {
                                                var Message = $"[{DateTime.Now.ToString("HH:mm")}] [Command] {textBox1.Text}";

                                                richTextBox1.ReadOnly = false;
                                                richTextBox1.Text += $"{Message}";

                                                richTextBox1.ReadOnly = true;
                                                FunctionClass.SendKeysClass.CustomSendKeys(Process.GetProcessById(id), $"{textBox1.Text}{EnterKey}");

                                                Data.AppCollections.Default.ConsoleMessages.Add($"{Message}");

                                            }
                                            else if (richTextBox1.Text != string.Empty)
                                            {
                                                var Message = $"{Environment.NewLine}[{DateTime.Now.ToString("HH:mm")}] [Command] {textBox1.Text}";

                                                richTextBox1.ReadOnly = false;
                                                richTextBox1.Text += $"{Message}";
                                                richTextBox1.ReadOnly = true;

                                                FunctionClass.SendKeysClass.CustomSendKeys(Process.GetProcessById(id), $"{textBox1.Text}{EnterKey}");

                                                Data.AppCollections.Default.ConsoleMessages.Add($"{Message}");
                                            }
                                        }

                                        textBox1.Clear();
                                    }



                                }
                                else
                                {
                                    richTextBox1.Clear();
                                    ColorRichText("SendKeys Failed", Color.FromArgb(250, 50, 50), false);
                                    break;
                                }
                            }

                        }
                        else
                        {
                            richTextBox1.Clear();
                            ColorRichText("SendKeys Failed", Color.FromArgb(250, 50, 50), false);
                        }

                    }
                    catch
                    {
                        richTextBox1.Clear();
                        ColorRichText("SendKeys Failed", Color.FromArgb(250, 50, 50), false);
                    }
                }
             
   
           
                
            }
            else
            {
                MessageBox.Show("Textbox Empty", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        string OldServer = Data.AppData.Default.CurrentServer;
        private async void ServerCheckWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if(OldServer != Data.AppData.Default.CurrentServer)
            {
                OldServer = Data.AppData.Default.CurrentServer;
                Data.AppCollections.Default.ConsoleMessages.Clear();
                Custom_Load();
            }
            await Task.Delay(1);
            if (!ServerCheckWorker.CancellationPending)
            {
                ServerCheckWorker.RunWorkerAsync();
            }
        }
    }

}
