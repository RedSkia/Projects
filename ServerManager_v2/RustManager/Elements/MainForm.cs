using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace RustManager.Elements
{
    public partial class MainForm : UI.Elements.FormModel
    {
        #region Init
        private MainForm Instance { get; set; }
        public MainForm()
        {
            InitializeComponent();
            Instance = this;
            Disposed += OnDispose;
        }



        private void MainForm_Load(object sender, EventArgs e)
        {
            CheckForIllegalCrossThreadCalls = false;
            if (ColorWorker.IsBusy != true) { ColorWorker.RunWorkerAsync(); }
        }
        #endregion Init

        #region Background Workers
        private const int WorkerDelaySlow = 5000;
        private const int WorkerDelayNormal = 1000;
        private const int WorkerDelayFast = 100;
        private async void ColorWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            await Task.Delay(WorkerDelayFast);
            #region Update
            BackColor = UI.Helpers.Color.Background;
            #endregion Update
            if (ColorWorker.IsBusy != true && !ColorWorker.CancellationPending) { ColorWorker.RunWorkerAsync(); }
        }
        #endregion Background Workers

        private void OnDispose(object sender, EventArgs e)
        {
            ColorWorker.Dispose();
            Dispose();
        }


        protected override void OnFormClosing(FormClosingEventArgs e)
        {

            if (ModifierKeys == Keys.Alt || ModifierKeys == Keys.F4)
            {
                e.Cancel = true;
            }

            if (LIB.Data.Pool.Processing)
            {
                var msg = LIB.Windows.Dialog.Show("Application is currently processing tasks\nDo you wish to cancel & close them?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
                if (msg == DialogResult.No)
                    e.Cancel = true;
            }

            base.OnFormClosing(e);
        }
    }
}