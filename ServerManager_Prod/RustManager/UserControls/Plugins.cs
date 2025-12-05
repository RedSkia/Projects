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
using System.Xml.Serialization;

namespace RustManager.UserControls
{
    public partial class Plugins : UserControl
    {
        public Plugins()
        {
            InitializeComponent();
        }

        private void Plugins_Load(object sender, EventArgs e)
        {




            LoadPrivatePlugins();



            Directory.CreateDirectory($"{Data.AppData.Default.RootFolder}/{Data.AppData.Default.CurrentServer}/LiveServer/Oxide/plugins/#Disabled");
  
            ActiveWindow();

            LoadEnabledPlugins();
            LoadDisabledPlugins();
        }



        void LoadPrivatePlugins()
        {
            #region DataGridStyle
            customDatagrid1.Rows.Clear();
            customDatagrid1.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            #endregion


            /*
                To Add A New Plugin
                Add Plugin Interface & CheckInstalled
                Make a new class In PluginFolder
                Set Class to Always Copy
            */
            #region Private Plugin List
                #region XLogs
                    customDatagrid1.Rows.Add("XLogs", "Xray", "a Really Sexy Plguin", "1.0.0", "Install");
                    CheckPluginInstalled("XLogs", 0);
                #endregion
            #endregion


            #region DataGridStyle
            customDatagrid1.RowsDefaultCellStyle.SelectionBackColor = Color.Transparent;
            customDatagrid1.RowsDefaultCellStyle.SelectionForeColor = Color.Transparent;
            customDatagrid1.ClearSelection();
            #endregion

        }



        #region Interface Main Buttons

        void ActiveWindow()
        {
            if (CurrentWindow == "Plugins Viewer")
            {
                LoadEnabledPlugins();
                LoadDisabledPlugins();

                tableLayoutPanel1.Visible = true;
                customDatagrid1.Visible = false;

                panel1.Visible = true;
                panel2.Visible = false;
            }
            if (CurrentWindow == "Plugins Private")
            {
                LoadPrivatePlugins();

                customDatagrid1.Visible = true;
                tableLayoutPanel1.Visible = false;

                panel2.Visible = true;
                panel1.Visible = false;
            }
        }


        string CurrentWindow = "Plugins Viewer";
        private void customButton2_Click(object sender, EventArgs e)
        {
            if (CurrentWindow != "Plugins Viewer")
            {
                CurrentWindow = "Plugins Viewer";
                ActiveWindow();
            }
        }
        private void customButton3_Click(object sender, EventArgs e)
        {
            if (CurrentWindow != "Plugins Private")
            {
                CurrentWindow = "Plugins Private";
                ActiveWindow();
            }
        }




        #endregion

        #region Plugin Viewer


        #region Move Plugins Buttons

        private void customButton1_Click(object sender, EventArgs e)
            {
                try
                {
                    File.Move($"{Data.AppData.Default.RootFolder}/{Data.AppData.Default.CurrentServer}/LiveServer/Oxide/plugins/#Disabled/{listBox2.GetItemText(listBox2.SelectedItem)}", $"{Data.AppData.Default.RootFolder}/{Data.AppData.Default.CurrentServer}/LiveServer/Oxide/plugins/{listBox2.GetItemText(listBox2.SelectedItem)}", true);

                    LoadEnabledPlugins();
                    LoadDisabledPlugins();
                }
                catch { }

            }

            private void customButton6_Click(object sender, EventArgs e)
            {
                try
                {
                    File.Move($"{Data.AppData.Default.RootFolder}/{Data.AppData.Default.CurrentServer}/LiveServer/Oxide/plugins/{listBox1.GetItemText(listBox1.SelectedItem)}", $"{Data.AppData.Default.RootFolder}/{Data.AppData.Default.CurrentServer}/LiveServer/Oxide/plugins/#Disabled/{listBox1.GetItemText(listBox1.SelectedItem)}", true);

                    LoadEnabledPlugins();
                    LoadDisabledPlugins();
                }
                catch { }

            }

            private void customButton4_Click(object sender, EventArgs e)
            {
                try
                {
                    string[] Files = Directory.GetFiles($"{Data.AppData.Default.RootFolder}/{Data.AppData.Default.CurrentServer}/LiveServer/Oxide/plugins/#Disabled");

                    foreach (var item in Files)
                    {
                        File.Move(item, Path.Combine($"{Data.AppData.Default.RootFolder}/{Data.AppData.Default.CurrentServer}/LiveServer/Oxide/plugins", Path.GetFileName(item)));
                    }
                    LoadEnabledPlugins();
                    LoadDisabledPlugins();
                }
                catch { }
            }

            private void customButton5_Click(object sender, EventArgs e)
            {
                try
                {
                    string[] Files = Directory.GetFiles($"{Data.AppData.Default.RootFolder}/{Data.AppData.Default.CurrentServer}/LiveServer/Oxide/plugins/");

                    foreach (var item in Files)
                    {
                        File.Move(item, Path.Combine($"{Data.AppData.Default.RootFolder}/{Data.AppData.Default.CurrentServer}/LiveServer/Oxide/plugins/#Disabled", Path.GetFileName(item)));
                    }
                    LoadEnabledPlugins();
                    LoadDisabledPlugins();
                }
                catch { }
            }

            #endregion


            #region Load Plugins
            List<string> items = new List<string>();
            void LoadEnabledPlugins()
            {
                try
                {
                    var EnabledPluginCount = Directory.GetFiles($"{Data.AppData.Default.RootFolder}/{Data.AppData.Default.CurrentServer}/LiveServer/Oxide/plugins");
                    label1.Text = $"Enabled Plugins {EnabledPluginCount.Length}";

                    listBox1.Items.Clear();


                    var fileNames = Directory.GetFiles($"{Data.AppData.Default.RootFolder}/{Data.AppData.Default.CurrentServer}/LiveServer/Oxide/plugins");
                    foreach (var fileName in fileNames)
                    {
                        listBox1.Items.Add(Path.GetFileName(fileName));

                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
            void LoadDisabledPlugins()
            {
                try
                {
                    var DisabledPluginCount = Directory.GetFiles($"{Data.AppData.Default.RootFolder}/{Data.AppData.Default.CurrentServer}/LiveServer/Oxide/plugins/#Disabled");
                    label2.Text = $"Disabled Plugins {DisabledPluginCount.Length}";

                    listBox2.Items.Clear();


                    var fileNames = Directory.GetFiles($"{Data.AppData.Default.RootFolder}/{Data.AppData.Default.CurrentServer}/LiveServer/Oxide/plugins/#Disabled");
                    foreach (var fileName in fileNames)
                    {
                        listBox2.Items.Add(Path.GetFileName(fileName));

                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }

            #endregion


            #region Listbox 1 & 2 Style
            private void listBox1_DrawItem(object sender, DrawItemEventArgs e)
            {
                if (e.Index < 0) return;
                //if the item state is selected them change the back color 
                if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
                    e = new DrawItemEventArgs(e.Graphics,
                    e.Font,
                    e.Bounds,
                    e.Index,
                    e.State ^ DrawItemState.Selected,
                    e.ForeColor,
                    Color.FromArgb(15, 25, 35));//Choose the color

                // Draw the background of the ListBox control for each item.
                e.DrawBackground();
                // Draw the current item text
                e.Graphics.DrawString(listBox1.Items[e.Index].ToString(), e.Font, Brushes.White, e.Bounds, StringFormat.GenericDefault);
                // If the ListBox has focus, draw a focus rectangle around the selected item.
                e.DrawFocusRectangle();
            }

            private void listBox2_DrawItem(object sender, DrawItemEventArgs e)
            {
                if (e.Index < 0) return;
                //if the item state is selected them change the back color 
                if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
                    e = new DrawItemEventArgs(e.Graphics,
                    e.Font,
                    e.Bounds,
                    e.Index,
                    e.State ^ DrawItemState.Selected,
                    e.ForeColor,
                    Color.FromArgb(15, 25, 35));//Choose the color

                // Draw the background of the ListBox control for each item.
                e.DrawBackground();
                // Draw the current item text
                e.Graphics.DrawString(listBox2.Items[e.Index].ToString(), e.Font, Brushes.White, e.Bounds, StringFormat.GenericDefault);
                // If the ListBox has focus, draw a focus rectangle around the selected item.
                e.DrawFocusRectangle();
            }
            #endregion
            #region Listbox 1 & 2 Functions

            bool ListBoxSwitch = false;
            private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
            {
                if (ListBoxSwitch)
                {
                    listBox2.SelectedItem = null;
                }
                ListBoxSwitch = false;
            }

            private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
            {
                if (!ListBoxSwitch)
                {
                    listBox1.SelectedItem = null;
                }
                ListBoxSwitch = true;
            }

            private void listBox1_MouseDoubleClick(object sender, MouseEventArgs e)
            {
                OpenConfigFile();
            }

            private void listBox1_KeyDown(object sender, KeyEventArgs e)
            {
                if (e.KeyCode == Keys.Enter)
                {
                    OpenConfigFile();
                }
            }
            #endregion


            #region Textbox Search 

            private void textBox1_TextChanged(object sender, EventArgs e)
            {

                if (!String.IsNullOrEmpty(textBox1.Text))
                {
                    listBox1.SelectedItems.Clear();
                    for (int num = listBox1.Items.Count - 1; num >= 0; num--)
                    {
                        if (listBox1.Items[num].ToString().ToLower().Contains(textBox1.Text.ToLower()))
                        {
                            listBox1.SetSelected(num, true);
                        }
                    }
                }
                else
                {
                    listBox1.SelectedItems.Clear();
                }

            }

            private void textBox2_TextChanged(object sender, EventArgs e)
            {
                if (!String.IsNullOrEmpty(textBox2.Text))
                {
                    listBox2.SelectedItems.Clear();
                    for (int num = listBox2.Items.Count - 1; num >= 0; num--)
                    {
                        if (listBox2.Items[num].ToString().ToLower().Contains(textBox2.Text.ToLower()))
                        {
                            listBox2.SetSelected(num, true);
                        }
                    }
                }
                else
                {
                    listBox2.SelectedItems.Clear();
                }
            }

            #endregion


            #region Functions

            void OpenConfigFile()
            {
                //Splitting Function
                string TempPath = listBox1.GetItemText(listBox1.SelectedItem);
                var SplitPath = TempPath.Split(new[] { "." }, StringSplitOptions.None);

                //Start With Args & Path
                string Path = $"{Data.AppData.Default.RootFolder}/{Data.AppData.Default.CurrentServer}/LiveServer/Oxide/config/{SplitPath[0]}.json";
                if (File.Exists(Path))
                {
                    FunctionForms.EditorForm NextForm = new FunctionForms.EditorForm($"{Path}", $"{SplitPath[0]}.json");
                    NextForm.Show();
                }
                else
                {
                    MessageBox.Show($"Config not found for {SplitPath[0]}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        #endregion


        #endregion

        #region Private Plugins

            #region Plugin Installer

            private void customDatagrid1_CellClick(object sender, DataGridViewCellEventArgs e)
            {
                if (e.ColumnIndex == customDatagrid1.Columns["Column5"].Index)
                {
                    var PluginName = customDatagrid1.Rows[e.RowIndex].Cells[0].Value.ToString();


                    //Plugin Installed DO Uninstall
                    if (PluginInstalled(PluginName) == true)
                    {
                        File.Delete($"{Data.AppData.Default.RootFolder}/{Data.AppData.Default.CurrentServer}/LiveServer/Oxide/plugins/{PluginName}.cs");
                    }
                    //Plugin NOT Installed DO Install
                    else if (PluginInstalled(PluginName) == false)
                    {
                        if (GetPluginCode(PluginName) != "Error")
                        {
                            using (StreamWriter sw = new StreamWriter(File.Open($"{Data.AppData.Default.RootFolder}/{Data.AppData.Default.CurrentServer}/LiveServer/Oxide/plugins/{PluginName}.cs", System.IO.FileMode.Append)))
                            {
                                sw.Write(GetPluginCode(PluginName));
                            }
                        }
                        else
                        {
                            MessageBox.Show($"{PluginName} - Plugin Code Is Unavailable", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    LoadPrivatePlugins();
                }
            }
            #endregion

            #region Plugin Functions
            void CheckPluginInstalled(string PluginName, int PluginIndex)
                {
                    if (File.Exists($"{Data.AppData.Default.RootFolder}/{Data.AppData.Default.CurrentServer}/LiveServer/Oxide/plugins/{PluginName}.cs"))
                    {
                        int colCount = customDatagrid1.Rows[PluginIndex].Cells.Count;
                        if (customDatagrid1.Rows[PluginIndex].Cells[0].Value == PluginName)
                        {
                            customDatagrid1.Rows[PluginIndex].Cells[colCount - 1].Value = "Uninstall";
                        }
                    }
                }

                bool PluginInstalled(string Plugin)
                {
                    if (File.Exists($"{Data.AppData.Default.RootFolder}/{Data.AppData.Default.CurrentServer}/LiveServer/Oxide/plugins/{Plugin}.cs"))
                    {
                        return (true);
                    }
                    return (false);
                }

                string GetPluginCode(string PluginName)
                {
                    try
                    {
                        StreamReader reader = new StreamReader($"{Environment.CurrentDirectory}/PluginFolder/{PluginName}.cs");

                        return ($"{reader.ReadToEnd()}");
                    }
                    catch
                    {

                    }
                    return ($"Error");
                }
                #endregion

            #region DataGrid Hover Function

            private void customDatagrid1_CellMouseMove(object sender, DataGridViewCellMouseEventArgs e)
            {
                int colCount = customDatagrid1.Rows[0].Cells.Count;

                if (e.RowIndex >= 0 && e.ColumnIndex >= colCount - 1)
                {
                    customDatagrid1[e.ColumnIndex, e.RowIndex].Style.BackColor = Color.FromArgb(10, 20, 30);
                }
            }

            private void customDatagrid1_CellMouseLeave(object sender, DataGridViewCellEventArgs e)
            {

                int colCount = customDatagrid1.Rows[0].Cells.Count;

                if (e.RowIndex % 2 == 0)
                {
                    if (e.RowIndex >= 0 && e.ColumnIndex >= colCount - 1)
                    {
                        customDatagrid1[e.ColumnIndex, e.RowIndex].Style.BackColor = customDatagrid1.RowsDefaultCellStyle.BackColor;
                    }
                }
                else
                {
                    if (e.RowIndex >= 0 && e.ColumnIndex >= colCount - 1)
                    {
                        customDatagrid1[e.ColumnIndex, e.RowIndex].Style.BackColor = customDatagrid1.AlternatingRowsDefaultCellStyle.BackColor;
                    }
                }


            }
            #endregion

        #endregion


 
        bool DataGridLarger = false;
        int OldMaxSize;
        private void Plugins_Resize(object sender, EventArgs e)
        {
            #region Datagrid Fixer
            if (Width >= customDatagrid1.Columns.GetColumnsWidth(DataGridViewElementStates.Visible))
            {
                if (!DataGridLarger)
                {
          
      
                    OldMaxSize = customDatagrid1.Columns.GetColumnsWidth(DataGridViewElementStates.Visible);
               
                    
                
                    foreach (DataGridViewColumn column in customDatagrid1.Columns)
                    {
                        if (column.Index == 2)
                        {
                            column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                        }
                        else
                        {
                           column.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
         
                        }
               
                    }

         
                    DataGridLarger = true;
                }

            }
            if (Width <= OldMaxSize)
            {
                foreach (DataGridViewColumn column in customDatagrid1.Columns)
                {
               
        
                    column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;


                }
                DataGridLarger = false;

            }
            #endregion
        }

        private void DataUpdaterWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

        }
    }
}
