using Newtonsoft.Json;
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

namespace UI.Elements.UserControls
{
    public partial class CustomColorPicker : UserControl
    {
        #region Init
        public CustomColorPicker()
        {
            InitializeComponent();
        }

        private void CustomColorPicker_Load(object sender, EventArgs e)
        {
            Disposed += OnDispose;
            CloseButton.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            UndoButton.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            SaveButton.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            SetSaveButton(false);
            SetUndoButton(true);
            UpdatePanels();
            if (ColorWorker.IsBusy != true) { ColorWorker.RunWorkerAsync(); }
        }
        private void OnDispose(object sender, EventArgs e)
        {
            ColorWorker.Dispose();
            Dispose();
        }
        #endregion Init

        #region UI Functions
        private void SetSaveButton(bool Show)
        {
            if (Show)
            {
                SaveButton.Visible = true;
                ButtonPanel.Height = UndoButton.Visible ? 80 : 40;
                CloseButton.Height = ButtonPanel.Height;
                line17.Height = ButtonPanel.Height;
                ButtonPanel.RowStyles[1].Height = 50;
            }
            else
            {
                SaveButton.Visible = false;
                ButtonPanel.Height = 40;
                CloseButton.Height = ButtonPanel.Height;
                line17.Height = ButtonPanel.Height;
                ButtonPanel.RowStyles[1].Height = 0;
            }
        }
        private void SetUndoButton(bool Show)
        {
            if (Show)
            {
                UndoButton.Visible = true;
                ButtonPanel.Height = SaveButton.Visible ? 80 : 40;
                CloseButton.Height = ButtonPanel.Height;
                line17.Height = ButtonPanel.Height;
                ButtonPanel.RowStyles[0].Height = 50;
            }
            else
            {
                UndoButton.Visible = false;
                ButtonPanel.Height = 40;
                CloseButton.Height = ButtonPanel.Height;
                line17.Height = ButtonPanel.Height;
                ButtonPanel.RowStyles[0].Height = 0;
            }
        }

        /// <summary>
        /// Updates ColorPicker Panels
        /// </summary>
        private void UpdatePanels()
        {
            PanelBackground.BackColor = UI.Helpers.Color.Background;
            PanelMain.BackColor = UI.Helpers.Color.MainElement;
            PanelSub.BackColor = UI.Helpers.Color.SubElement;
            PanelDark.BackColor = UI.Helpers.Color.Dark;
            PanelLight.BackColor = UI.Helpers.Color.Light;
            PanelLine.BackColor = UI.Helpers.Color.Line;
            PanelHover.BackColor = UI.Helpers.Color.Hover;
            PanelClick.BackColor = UI.Helpers.Color.Click;
            PanelSelected.BackColor = UI.Helpers.Color.Selected;
            PanelText.BackColor = UI.Helpers.Color.Text;
            PanelIcon.BackColor = UI.Helpers.Color.Style.customModeRef.Icon;
        }
        #endregion UI Functions

        #region Background Workers
        private const int WorkerDelaySlow = 5000;
        private const int WorkerDelayNormal = 1000;
        private const int WorkerDelayFast = 100;
        private UI.Helpers.Color.Theme oldTheme { get; set; }
        private Color oldBackground { get; set; }
        private async void ColorWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            await Task.Delay(WorkerDelayFast);

            BackColor = UI.Helpers.Color.Background;
            ColorPickerPanel.BackColor = UI.Helpers.Color.MainElement;
            UI.Helpers.Color.UpdateControl(UndoButton);
            UI.Helpers.Color.UpdateControl(SaveButton);
            UI.Helpers.Color.UpdateControl(CloseButton);
            #region Lines
            line16.BackColor = UI.Helpers.Color.Line;
            line17.BackColor = UI.Helpers.Color.Line;
            foreach (var i in ColorPickerPanel.Controls)
            {
                if (i is UI.Elements.Line)
                {
                    var x = (UI.Elements.Line)i;
                    x.BackColor = UI.Helpers.Color.Line;
                }
            }
            #endregion Lines
            #region Labels
            NavLabel.ForeColor = UI.Helpers.Color.Text;
            foreach (var i in ColorPickerPanel.Controls)
            {
                if(i is Label)
                {
                    var x = (Label)i;
                    x.ForeColor = UI.Helpers.Color.Text;
                }
            }
            #endregion Labels
            //Force Redraw UI
            if (!oldTheme.Equals(UI.Helpers.Color.theme))
            {
                Update();
                Refresh();
                Invalidate();
                oldTheme = UI.Helpers.Color.theme;
            }


            if (ColorWorker.IsBusy != true && !ColorWorker.CancellationPending) { ColorWorker.RunWorkerAsync(); }
        }
        #endregion Background Workers

        #region Panel Handlers
        /// <summary>
        /// Brings up <see cref="ColorDialog"/>
        /// </summary>
        /// <returns><see cref="Color"/></returns>
        private System.Drawing.Color ShowColorDialog()
        {
            if (colorDialog.ShowDialog() == DialogResult.OK)
                return colorDialog.Color;
            return System.Drawing.Color.Black;
        }



        /// <summary>
        /// Handler for <paramref name="sender"/> (Panel)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PanelClickHandler(object sender, MouseEventArgs e)
        {
            if ((sender != null && sender is Panel))
            {
                var panel = sender as Panel;

                if(panel == PanelBackground) { oldBackground = panel.BackColor; }
                panelChanged.Add(new LatestPanelChanged
                {
                    panel = panel,
                    color = panel.BackColor
                });

                panel.BackColor = ShowColorDialog();
                SetSaveButton(true);
                IsSaved = false; /*Custom Colors Saved*/
            }
        }

        #region Event Handlers
        public class LatestPanelChanged
        {
            public Panel panel { get; set; }
            public Color color { get; set; }
        }

        private static List<LatestPanelChanged> panelChanged = new List<LatestPanelChanged>();
        private void PanelBackground_MouseClick(object sender, MouseEventArgs e) => PanelClickHandler(sender, e);
        private void PanelMain_MouseClick(object sender, MouseEventArgs e) => PanelClickHandler(sender, e);
        private void PanelSub_MouseClick(object sender, MouseEventArgs e) => PanelClickHandler(sender, e);
        private void PanelDark_MouseClick(object sender, MouseEventArgs e) => PanelClickHandler(sender, e);
        private void PanelLight_MouseClick(object sender, MouseEventArgs e) => PanelClickHandler(sender, e);
        private void PanelLine_MouseClick(object sender, MouseEventArgs e) => PanelClickHandler(sender, e);
        private void PanelHover_MouseClick(object sender, MouseEventArgs e) => PanelClickHandler(sender, e);
        private void PanelClick_MouseClick(object sender, MouseEventArgs e) => PanelClickHandler(sender, e);
        private void PanelSelected_MouseClick(object sender, MouseEventArgs e) => PanelClickHandler(sender, e);
        private void PanelText_MouseClick(object sender, MouseEventArgs e) => PanelClickHandler(sender, e);
        private void PanelIcon_MouseClick(object sender, MouseEventArgs e) => PanelClickHandler(sender, e);
        #endregion Event Handlers
        #endregion Panel Handlers

        private bool IsSaved { get; set; } = false;
        private async void SaveButton_Click(object sender, EventArgs e)
        {
            UI.Helpers.Color.Style.customModeRef.Background = PanelBackground.BackColor;
            UI.Helpers.Color.Style.customModeRef.Click = PanelClick.BackColor;
            UI.Helpers.Color.Style.customModeRef.Dark = PanelDark.BackColor;
            UI.Helpers.Color.Style.customModeRef.Hover = PanelHover.BackColor;
            UI.Helpers.Color.Style.customModeRef.Light = PanelLight.BackColor;
            UI.Helpers.Color.Style.customModeRef.Line = PanelLine.BackColor;
            UI.Helpers.Color.Style.customModeRef.MainElement = PanelMain.BackColor;
            UI.Helpers.Color.Style.customModeRef.Selected = PanelSelected.BackColor;
            UI.Helpers.Color.Style.customModeRef.SubElement = PanelSub.BackColor;
            UI.Helpers.Color.Style.customModeRef.Text = PanelText.BackColor;
            UI.Helpers.Color.Style.customModeRef.Icon = PanelIcon.BackColor;
            SetSaveButton(false);

            await UI.Helpers.IconHandler.UpdateCustomImages(UI.Helpers.Color.Style.customModeRef.Icon);
            UI.Helpers.Color.File.Save();
            IsSaved = true; /*Custom Colors Saved*/

        }
        private void UndoButton_Click(object sender, EventArgs e)
        {
            SetUndoButton(true);
            IsSaved = false; /*Custom Colors Saved*/

            if (!IsSaved) { SetSaveButton(true); }
            else { SetSaveButton(false); }

            if (panelChanged.Count > 0)
            {
                panelChanged[0].panel.BackColor = panelChanged[0].color;
                panelChanged.RemoveAt(0);
            }
        }
        private void CloseButton_Click(object sender, EventArgs e)
        {
            Dispose();
        }


        private void button4_Click(object sender, EventArgs e)
        {
            LIB.RustServer.Manage.Add.DirtyLocal("DirtyServ1", new LIB.RustServer.Manage.ServerData.DirtyLocal
            {
                type = LIB.RustServer.Manage.SeverType.dirtyLocal,
                Root = "Root/Folder",
                StartBat = "Start.bat",
                SteamCMD = "SteamCMD.exe",
                Oxide = "Oxide/Plugs",
                rcon = new LIB.RustServer.Manage.ServerData.Rcon
                {
                    Host = "locahost",
                    Pass = "LetMeIN",
                    Port = 30011,
                }
           
            });
        }

        private void button1_Click(object sender, EventArgs e)
        {

            LIB.RustServer.Manage.Add.Rcon("RconServ1", new LIB.RustServer.Manage.ServerData.Rcon
            {
                type = LIB.RustServer.Manage.SeverType.rcon,
                Host = "locahost",
                Pass = "LetMeIN",
                Port = 30011,
            });
        }

        private void button3_Click(object sender, EventArgs e)
        {
            LIB.RustServer.Manage.SaveServers();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            LIB.RustServer.Manage.LoadServers();

        }


        private void button2_Click(object sender, EventArgs e)
        {
            LIB.Helpers.BitmapConverter.SaveImages();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            LIB.Helpers.BitmapConverter.LoadImages();

        }

        private void button7_Click(object sender, EventArgs e)
        {
            label2.Text = "";
            foreach (var server in LIB.RustServer.Manage.GetServers())
            {


            }
        }

   
    }
}