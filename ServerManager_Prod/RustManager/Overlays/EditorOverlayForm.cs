using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RustManager.Overlays
{
    public partial class EditorOverlayForm : Form
    {
        public EditorOverlayForm()
        {
            InitializeComponent();
        }
        private void EditorOverlayForm_Load(object sender, EventArgs e)
        {
            customButton1.Select();
        }



        private void customButton2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private async void CheckActiveStateTimer_Tick(object sender, EventArgs e)
        {
            await Task.Delay(1);

            if (Data.AppInfo.Default.EditorActive == true)
            {
                TopMost = true;

            }
            else if (Data.AppInfo.Default.EditorActive == false)
            {
                TopMost = false;

            }
        }


        void SetData()
        {
            Data.OverlayEditorData.Default.LineNumber = textBox1.Text;
            Data.OverlayEditorData.Default.Keyword = textBox2.Text;

            Close();
        }


        private void customButton1_Click(object sender, EventArgs e)
        {
            SetData();
        }

        private void textBox1_Enter(object sender, EventArgs e)
        {

            textBox2.Clear();
        }

        private void textBox2_Enter(object sender, EventArgs e)
        {
            textBox1.Clear();
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                SetData();
            }
            if (e.KeyCode == Keys.Escape)
            {
                Close();
            }
        }

        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                SetData();
            }
            if (e.KeyCode == Keys.Escape)
            {
                Close();
            }
        }

        private void customButton2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                Close();
            }
        }

        private void customButton1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                Close();
            }
        }


    }
}
