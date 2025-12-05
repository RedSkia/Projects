using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UI.Elements.UserControls
{
    public partial class UpdateLog : UserControl
    {


        private class UpdateData
        {
            public string Added { get; set; }
            public string Removed { get; set; }
            public string Improved { get; set; }
            public string Fixed { get; set; }
            public string Details { get; set; }
        }

        private static Dictionary<string, UpdateData> updates = new Dictionary<string, UpdateData>();
        private static void AddUpdate(string version, UpdateData update)
        {
            if(!updates.ContainsKey(version)) { updates.Add(version, update); }
        }
        #region Updates
        private void LoadUpdates()
        {
            AddUpdate("1.0.0", new UpdateData()
            {
                Details = "Release"
            });
        }
        #endregion Updates


        public UpdateLog()
        {
            InitializeComponent();

            //imageList.Images.Add(Properties.Resources.ArrowDown);
            //imageList.Images.Add(Properties.Resources.ArrowUp);

            LoadUpdates();
            foreach (var item in updates)
            {
                listBox1.Items.Add(item.Key);
            }


        }


        private void UpdateLog_Load(object sender, EventArgs e)
        {

        }


        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            var i = updates.Values.ToArray()[listBox1.SelectedIndex];

            string Added = i.Added != null ? $"\nAdded: {i.Added}" : String.Empty;
            string Removed = i.Removed != null ? $"\nRemoved: {i.Removed}" : String.Empty;
            string Improved = i.Improved != null ? $"\nImproved: {i.Improved}" : String.Empty;
            string Fixed = i.Fixed != null ? $"\nFixed: {i.Fixed}" : String.Empty;
            string Details = i.Details != null ? $"\nDetails: {i.Details}" : String.Empty;



            richTextBox1.Text = $"" +
                $"{Added}" +
                $"{Removed}" +
                $"{Improved}" +
                $"{Fixed}" +
                $"{Details}";
        
                richTextBox1.Lines = richTextBox1.Lines.Skip(1).ToArray();
            
        }
    }
}
