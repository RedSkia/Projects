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

namespace RustManager.UserControls.SubControls
{
    public partial class Logs : UserControl
    {
        public Logs()
        {
            InitializeComponent();
            Disposed += OnDispose;

        }

        void ShowLoadingControl(string TextToDisplay)
        {
            customDatagrid1.Visible = false;
            FunctionForms.LoadingControl control = new FunctionForms.LoadingControl(TextToDisplay) { Dock = DockStyle.Fill };
            DisplayPanel.Controls.Clear();
            DisplayPanel.Controls.Add(control);
            control.Show();
        }

        void HideLoadingControl()
        {

            customDatagrid1.Visible = true;
            FunctionForms.LoadingControl control = new FunctionForms.LoadingControl("") { Dock = DockStyle.Fill };
            DisplayPanel.Controls.Clear();
            control.Dispose();

   
        }

        DataTable EmptyTable = new DataTable();
        DataTable NoResultsTable = new DataTable();
        private void Logs_Load(object sender, EventArgs e)
        {

            SetComboBox();
        


            SetGridEmptyTable();


          

            if (LogWorker.IsBusy != true)
            {
                LogWorker.RunWorkerAsync();
            }

       

        }







        #region Set Tables

        void SetGridEmptyTable()
        {

            if (EmptyTable.Columns.Count <= 0)
            {
                EmptyTable.Columns.Add("No Logs");
            }
  

            customDatagrid1.DataSource = EmptyTable;

            foreach (DataGridViewColumn column in customDatagrid1.Columns)
            {

                column.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

                column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;


            }
        }


        void SetGridNoResults()
        {
            if (NoResultsTable.Columns.Count <= 0)
            {
                NoResultsTable.Columns.Add("No Results");
            }


            customDatagrid1.DataSource = NoResultsTable;

            foreach (DataGridViewColumn column in customDatagrid1.Columns)
            {

                column.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

                column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;


            }
        }
        #endregion




        #region Set ComboBox
        DataTable GridTable = new DataTable();
        void SetComboBox()
        {
            comboBox1.Items.Add("Chat Messages");
            comboBox1.Items.Add("Connections");
            comboBox1.Items.Add("Disconnections");
            comboBox1.Items.Add("Respawns");
            comboBox1.Items.Add("Kicks");
            comboBox1.Items.Add("Bans");
            comboBox1.Items.Add("Reports");
            comboBox1.Items.Add("Violations");
            comboBox1.Items.Add("Items Spawned");
            comboBox1.Items.Add("Commands");
            comboBox1.Items.Add("Deaths");
        }
        #endregion





        #region Log Selector
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string CurrentLogFile = null;



       

            SeachTable(null, 0);
            textBox1.Clear();

            if (comboBox1.SelectedIndex == 0) { CurrentLogFile = "_chat.txt"; UnbindGridTable_Columns_Rows(); SetTable("Date/Time | Trigger | SteamID | Username | Message"); }
            if (comboBox1.SelectedIndex == 1) { CurrentLogFile = "_connections.txt"; UnbindGridTable_Columns_Rows(); SetTable("Date/Time | Trigger | SteamID | Username | IP Address"); }
            if (comboBox1.SelectedIndex == 2) { CurrentLogFile = "_disconnections.txt"; UnbindGridTable_Columns_Rows(); SetTable("Date/Time | Trigger | SteamID | Username"); }
            if (comboBox1.SelectedIndex == 3) { CurrentLogFile = "_respawns.txt"; UnbindGridTable_Columns_Rows(); SetTable("Date/Time | Trigger | SteamID | Username"); }
            if (comboBox1.SelectedIndex == 4) { CurrentLogFile = "_kicks.txt"; UnbindGridTable_Columns_Rows(); SetTable("Date/Time | Trigger | SteamID | Username | Reason"); }
            if (comboBox1.SelectedIndex == 5) { CurrentLogFile = "_bans.txt"; UnbindGridTable_Columns_Rows(); SetTable("Date/Time | Trigger | SteamID | Username | IP Address | Reason"); }
            if (comboBox1.SelectedIndex == 6) { CurrentLogFile = "_reports.txt"; UnbindGridTable_Columns_Rows(); SetTable("Date/Time | Trigger | Reporter ID | Reporter Name | Target ID | Target Name | Subject | Details"); }
            if (comboBox1.SelectedIndex == 7) { CurrentLogFile = "_violations.txt"; UnbindGridTable_Columns_Rows(); SetTable("Date/Time | Trigger | SteamID | Username | Violation | Value"); }
            if (comboBox1.SelectedIndex == 8) { CurrentLogFile = "_itemSpawned.txt"; UnbindGridTable_Columns_Rows(); SetTable("Date/Time | Trigger | Details"); }
            if (comboBox1.SelectedIndex == 9) { CurrentLogFile = "_commands.txt"; UnbindGridTable_Columns_Rows(); SetTable("Date/Time | Trigger | SteamID | Username | Command | Args"); }
            if (comboBox1.SelectedIndex == 10) { CurrentLogFile = "_deaths.txt"; UnbindGridTable_Columns_Rows(); SetTable("Date/Time | Trigger | SteamID | Username | Killer ID | Killer Name"); }




            ShowLoadingControl($"Importing\n{CurrentLogFile.Replace("_", "")}\nPlease Wait");


            if (Width >= customDatagrid1.Columns.GetColumnsWidth(DataGridViewElementStates.Visible))
            {
                DataGridLarger = false;
                OldMaxSize = 0;
                Resize_DataGrid(true);
            }

       

        }

        void SetTable(string Columns)
        {
   

            //UnbindDataGrid();
            comboBox2.Items.Clear();

            string[] ColumnsSplit = Columns.Split(" | ");

            foreach (var Column in ColumnsSplit)
            {
                GridTable.Columns.Add(Column);
                comboBox2.Items.Add(Column);
            }
            comboBox2.SelectedIndex = 0;






          

            ImportDataLogs(GridTable, true);



      



        }

        void UnbindGridTable_Columns_Rows()
        {
            GridTable.Columns.Clear();
            GridTable.Rows.Clear();
        }
        #endregion




        #region Import Logs
        long OldByteCount;
        string OldTable;
        int LineCountBuffer = 100;
        void ImportDataLogs(DataTable Table, bool ClearSize)
        {

            try
            {


                string[] lines = null;
                string[] values;
                string CurrentLogFile = null;


                #region Checking Table
                if (!comboBox1.DroppedDown)
                {
                    if (comboBox1.SelectedIndex == 0)
                    {
                        var LogFile = "_chat.txt";
                        lines = File.ReadAllLines($"{Data.AppData.Default.RootFolder}/{Data.AppData.Default.CurrentServer}/LiveServer/Oxide/logs/XLogs/xlogs{LogFile}");
                        CurrentLogFile = LogFile;
                    }
                    if (comboBox1.SelectedIndex == 1)
                    {
                        var LogFile = "_connections.txt";
                        lines = File.ReadAllLines($"{Data.AppData.Default.RootFolder}/{Data.AppData.Default.CurrentServer}/LiveServer/Oxide/logs/XLogs/xlogs{LogFile}");
                        CurrentLogFile = LogFile;
                    }
                    if (comboBox1.SelectedIndex == 2)
                    {
                        var LogFile = "_disconnections.txt";
                        lines = File.ReadAllLines($"{Data.AppData.Default.RootFolder}/{Data.AppData.Default.CurrentServer}/LiveServer/Oxide/logs/XLogs/xlogs{LogFile}");
                        CurrentLogFile = LogFile;
                    }
                    if (comboBox1.SelectedIndex == 3)
                    {
                        var LogFile = "_respawns.txt";
                        lines = File.ReadAllLines($"{Data.AppData.Default.RootFolder}/{Data.AppData.Default.CurrentServer}/LiveServer/Oxide/logs/XLogs/xlogs{LogFile}");
                        CurrentLogFile = LogFile;
                    }
                    if (comboBox1.SelectedIndex == 4)
                    {
                        var LogFile = "_kicks.txt";
                        lines = File.ReadAllLines($"{Data.AppData.Default.RootFolder}/{Data.AppData.Default.CurrentServer}/LiveServer/Oxide/logs/XLogs/xlogs{LogFile}");
                        CurrentLogFile = LogFile;
                    }
                    if (comboBox1.SelectedIndex == 5)
                    {
                        var LogFile = "_bans.txt";
                        lines = File.ReadAllLines($"{Data.AppData.Default.RootFolder}/{Data.AppData.Default.CurrentServer}/LiveServer/Oxide/logs/XLogs/xlogs{LogFile}");
                        CurrentLogFile = LogFile;
                    }
                    if (comboBox1.SelectedIndex == 6)
                    {
                        var LogFile = "_reports.txt";
                        lines = File.ReadAllLines($"{Data.AppData.Default.RootFolder}/{Data.AppData.Default.CurrentServer}/LiveServer/Oxide/logs/XLogs/xlogs{LogFile}");
                        CurrentLogFile = LogFile;
                    }

                    if (comboBox1.SelectedIndex == 7)
                    {
                        var LogFile = "_violations.txt";
                        lines = File.ReadAllLines($"{Data.AppData.Default.RootFolder}/{Data.AppData.Default.CurrentServer}/LiveServer/Oxide/logs/XLogs/xlogs{LogFile}");
                        CurrentLogFile = LogFile;
                    }
                    if (comboBox1.SelectedIndex == 8)
                    {
                        var LogFile = "_itemSpawned.txt";
                        lines = File.ReadAllLines($"{Data.AppData.Default.RootFolder}/{Data.AppData.Default.CurrentServer}/LiveServer/Oxide/logs/XLogs/xlogs{LogFile}");
                        CurrentLogFile = LogFile;
                    }

                    if (comboBox1.SelectedIndex == 9)
                    {
                        var LogFile = "_commands.txt";
                        lines = File.ReadAllLines($"{Data.AppData.Default.RootFolder}/{Data.AppData.Default.CurrentServer}/LiveServer/Oxide/logs/XLogs/xlogs{LogFile}");
                        CurrentLogFile = LogFile;
                    }

                    if (comboBox1.SelectedIndex == 10)
                    {
                        var LogFile = "_deaths.txt";
                        lines = File.ReadAllLines($"{Data.AppData.Default.RootFolder}/{Data.AppData.Default.CurrentServer}/LiveServer/Oxide/logs/XLogs/xlogs{LogFile}");
                        CurrentLogFile = LogFile;
                    }
                }



                #endregion


                if (ClearSize == true)
                {
                    OldByteCount = -1;
                }



                var x = File.ReadAllText($"{Data.AppData.Default.RootFolder}/{Data.AppData.Default.CurrentServer}/LiveServer/Oxide/logs/XLogs/xlogs{CurrentLogFile}");

                var LogFilePath = $"{Data.AppData.Default.RootFolder}/{Data.AppData.Default.CurrentServer}/LiveServer/Oxide/logs/XLogs/xlogs{CurrentLogFile}";


                if (File.ReadAllBytes(LogFilePath).Length <= 0 || string.IsNullOrWhiteSpace(x))
                {

                    if (customDatagrid1.DataSource != EmptyTable)
                    {
                        SetGridEmptyTable();
                    }
                }
                else if (OldByteCount != File.ReadAllBytes(LogFilePath).Length)
                {
                    customDatagrid1.DataSource = Table;


                    Table.Rows.Clear();

                    for (int i = lines.Length; i-- > 0;)
                    {
                        values = lines[i].ToString().Split('|');
                        string[] row = new string[values.Length];



                        for (int j = 0; j < values.Length; j++)
                        {
                            row[j] = values[j].Trim();


                            if (row[j] == string.Empty)
                            {
                                row[j] = $"";

                            }

                        }


                        if (i >= lines.Length - 1 - LineCountBuffer && i <= lines.Length + 1)
                        {

                            Table.Rows.Add(row);
                        }



                        OldByteCount = File.ReadAllBytes(LogFilePath).Length;
                    }






                }
            }
            catch
            {
                if (customDatagrid1.Rows.Count <= 0 && customDatagrid1.DataSource != EmptyTable)
                {
                    SetGridEmptyTable();
                }

            }

            HideLoadingControl();
        }
        #endregion



        #region Seach Logs 

        void SeachTable(DataTable Table, int Selector)
        {
     

            string searchValue = textBox1.Text.ToLower();
            try
            {


                var re = from row in Table.AsEnumerable()
                         where row[Selector].ToString().ToLower().Contains(searchValue)
                         select row;

                if (re.Count() == 0)
                {







                    SetGridNoResults();

                    #region DataGrid Set Mode
                    foreach (DataGridViewColumn column in customDatagrid1.Columns)
                    {


                        column.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

                    }
                    #endregion

                    foreach (DataGridViewRow row in customDatagrid1.Rows) { customDatagrid1.CurrentCell = null; row.Visible = false; }
                }
                else
                {
            
                    if(!string.IsNullOrEmpty(textBox1.Text))
                    {
                       customDatagrid1.DataSource = re.CopyToDataTable();
                    }
                    else if(string.IsNullOrEmpty(textBox1.Text))
                    {
                        ImportDataLogs(Table, true);
                    }
               

                    foreach (DataGridViewRow row in customDatagrid1.Rows) { row.Visible = true; }



                    #region DataGrid Set Mode
                    if (DataGridLarger)
                    {
                        foreach (DataGridViewColumn column in customDatagrid1.Columns)
                        {


                            column.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

                        }
                    }
                    else
                    {
                        foreach (DataGridViewColumn column in customDatagrid1.Columns)
                        {

                            column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

                        }
                    }
                    #endregion


                }






            }
            catch{}


        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
      
           
            SeachTable(GridTable, comboBox2.SelectedIndex);
            
     
        }
        #endregion


        #region Logs_Loop
        private async void LogWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {


            if (!comboBox1.DroppedDown && string.IsNullOrEmpty(textBox1.Text) && !comboBox2.DroppedDown)
            {



                ImportDataLogs(GridTable, false);

            }


            await Task.Delay(1000);
            if (!LogWorker.CancellationPending)
            {
                LogWorker.RunWorkerAsync();
            }


        }
        #endregion



        #region Fixes Combobox Scrolling

        private void comboBox1_KeyUp(object sender, KeyEventArgs e)
        {
            e.Handled = true;
        }

        private void comboBox1_KeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;
        }
        #endregion

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            textBox1.Clear();
        }








        #region Resize Data Grid
        bool DataGridLarger = false;
        int OldMaxSize;

        void Resize_DataGrid(bool ForceSet_AllCells)
        {
            if(ForceSet_AllCells)
            {
                foreach (DataGridViewColumn column in customDatagrid1.Columns)
                {
                    column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                }
            }

            #region Datagrid Fixer
            if (Width >= customDatagrid1.Columns.GetColumnsWidth(DataGridViewElementStates.Visible))
            {
 
                if (!DataGridLarger)
                {


                    OldMaxSize = customDatagrid1.Columns.GetColumnsWidth(DataGridViewElementStates.Visible);


                    foreach (DataGridViewColumn column in customDatagrid1.Columns)
                    {


                        column.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;


                    }

                    DataGridLarger = true;
                }

            }
            if (Width <= OldMaxSize)
            {
                if(customDatagrid1.DataSource == GridTable)
                {
                    foreach (DataGridViewColumn column in customDatagrid1.Columns)
                    {

                        column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;


                    }
                }
   
                DataGridLarger = false;

            }
            #endregion
        }
        #endregion



        private void Logs_Resize(object sender, EventArgs e)
        {


            Resize_DataGrid(false);
         

        }



        //On Dispose/Close
        private void OnDispose(object sender, EventArgs e)
        {

            LogWorker.CancelAsync(); LogWorker.Dispose();

            Dispose();
        }


        private void customDatagrid1_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            MessageBox.Show($"Copied To Clipboard\n\n{customDatagrid1.SelectedCells[0].Value}", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            Clipboard.SetText(customDatagrid1.SelectedCells[0].Value.ToString());
        }



    }
}
