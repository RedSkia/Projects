using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UI.Helpers
{
    public class Color
    {
        /// <summary>
        /// Checks If <paramref name="value"/>  <see cref="System.Drawing.Color.FromArgb(int)"/> Is Between <paramref name="min"/> And <paramref name="max"/> Value
        /// </summary>
        public static bool IsBetween(int value, int min, int max) => (Enumerable.Range(min < 0 ? 0 : min, max).Contains(value));


        #region Custom Mode File Managment
        public class File
        {
            private static FileData fileData = new FileData();
            public class FileData
            {
                public Theme theme { get; set; }
                public Style.CustomMode mode { get; set; }
            }

            public static void Load()
            {
                var i = JsonConvert.DeserializeObject<FileData>(JsonConvert.SerializeObject(LIB.Settings.CORE.JsonManager.LoadFile(LIB.Settings.CORE.FileRef.Color)));
                if(i != null)
                {
                    theme = i.theme;
                    Style.customModeRef = i.mode;
                }
                else { Save(); }
             
            }
            public static void Save()
            {
                fileData.theme = theme;
                fileData.mode = Style.customModeRef;
                LIB.Settings.CORE.JsonManager.SaveFile(LIB.Settings.CORE.FileRef.Color, fileData);
            }
        }

        #endregion Custom Mode File Managment


        public static Theme theme { get; set; }
        public static Theme lastTheme { get; set; }
        public enum Theme
        {
            dark,
            light,
            blue,
            custom
        }

        #region Helpers
        /// <summary>
        /// Updates a controls color relative to <see cref="Theme"/>
        /// </summary>
        public static void UpdateControl(Control control)
        {
            switch (control)
            {
                case UI.Elements.Checkbox checkbox:
                    checkbox.CheckBoxColor = Color.SubElement;
                    checkbox.TextColor = Color.Text;
                    break;

                case System.Windows.Forms.Label label:
                    label.ForeColor = Color.Text;
                    break;

                case System.Windows.Forms.Button button:
                    button.BackColor = Color.MainElement;
                    button.ForeColor = Color.Text;
                    button.FlatAppearance.MouseOverBackColor = Color.Hover;
                    button.FlatAppearance.MouseDownBackColor = Color.Click;
                    break;

                case UI.Elements.ComboBox comboBox:
                    comboBox.BackColor = Color.Background;
                    comboBox.buttonColor = Color.MainElement;
                    comboBox.borderColor = Color.Line;
                    comboBox.ForeColor = Color.Text;
                    break;

                case UI.Elements.DataGridView dataGridView:
                    dataGridView.BackgroundColor = Color.Hover;
                    dataGridView.ColumnHeadersDefaultCellStyle.BackColor = Color.MainElement;
                    dataGridView.ColumnHeadersDefaultCellStyle.ForeColor = Color.Text;
                    dataGridView.DefaultCellStyle.ForeColor = Color.Text;
                    dataGridView.GridColor = Color.Hover;
                    dataGridView.RowsDefaultCellStyle.BackColor = Color.Dark;
                    dataGridView.AlternatingRowsDefaultCellStyle.BackColor = Color.Light;
                    dataGridView.DefaultCellStyle.SelectionBackColor = Color.SubElement;
                    dataGridView.DefaultCellStyle.SelectionForeColor = Color.Text;
                    break;

                case UI.Elements.TabControl tabControl:
                    tabControl.Refresh();
                    break;

            }
        }

        /// <returns><see cref="System.Drawing.Color"/> From <paramref name="R"/> <paramref name="G"/> <paramref name="B"/> Prevent values > <see cref="byte.MaxValue"/>(255)</returns>
        public static System.Drawing.Color Set(int R, int G, int B) => System.Drawing.Color.FromArgb(
            R > byte.MaxValue ? byte.MaxValue : R < byte.MinValue ? 1 : R,
            G > byte.MaxValue ? byte.MaxValue : G < byte.MinValue ? 1 : G,
            B > byte.MaxValue ? byte.MaxValue : B < byte.MinValue ? 1 : B);
        #endregion Helpers

        public class Style
        {
            /// <summary>
            /// Resets <see cref="CustomMode"/> Back to <see cref="lastTheme"/> Colors
            /// </summary>
            public static void ResetCustomMode()
            {
                Style selectedTheme = null;
                switch(lastTheme)
                {
                    case Theme.dark:
                        selectedTheme = darkModeRef;
                        break;
                    case Theme.light:
                        selectedTheme = lightModeRef;
                        break;
                    case Theme.blue:
                        selectedTheme = blueModeRef;
                        break;
                }

                for (int i = 0; i < customModeRef.GetType().GetProperties().Length-1; i++)
                {
                    var x = selectedTheme.GetType().GetProperties()[i].GetValue(selectedTheme);
                    customModeRef.GetType().GetProperties()[i].SetValue(customModeRef, x);
                }
                File.Save();
            }


            public static DarkMode darkModeRef = new DarkMode();
            public class DarkMode : Style
            {
                public System.Drawing.Color Background => Color.Set(15, 15, 15);
                public System.Drawing.Color MainElement => Color.Set(
                    Background.R * 2,
                    Background.G * 2,
                    Background.B * 2);
                public System.Drawing.Color SubElement => Color.Set(
                   MainElement.R + 15,
                   MainElement.G + 15,
                   MainElement.B + 15);
                public System.Drawing.Color Line => Color.Set(
                     SubElement.R + 30,
                     SubElement.G + 30,
                     SubElement.B + 30);
                public System.Drawing.Color Hover => Color.Set(
                     SubElement.R + 15,
                     SubElement.G + 15,
                     SubElement.B + 15);
                public System.Drawing.Color Click => Color.Set(
                     Hover.R + 15,
                     Hover.G + 15,
                     Hover.B + 15);
                public System.Drawing.Color Dark => Color.Set(
                SubElement.R + 15,
                SubElement.G + 15,
                SubElement.B + 15);
                public System.Drawing.Color Light => Color.Set(
                Dark.R + 30,
                Dark.G + 30,
                Dark.B + 30);
                public System.Drawing.Color Selected => Color.Set(
                   Light.R + 30,
                   Light.G + 30,
                   Light.B + 30);
                public System.Drawing.Color Text => Color.Set(255, 255, 255);
            }

            public static LightMode lightModeRef = new LightMode();
            public class LightMode : Style
            {
                public System.Drawing.Color Background => Color.Set(150, 150, 150);
                public System.Drawing.Color MainElement => Color.Set(
                    Background.R + 20,
                    Background.G + 20,
                    Background.B + 20);
                public System.Drawing.Color SubElement => Color.Set(
                   MainElement.R + 15,
                   MainElement.G + 15,
                   MainElement.B + 15);
                public System.Drawing.Color Dark => Color.Set(
                    SubElement.R + 15,
                    SubElement.G + 15,
                    SubElement.B + 15);
                public System.Drawing.Color Light => Color.Set(
                    Dark.R + 30,
                    Dark.G + 30,
                    Dark.B + 30);
                public System.Drawing.Color Line => Color.Set(
                     SubElement.R + 30,
                     SubElement.G + 30,
                     SubElement.B + 30);
                public System.Drawing.Color Hover => Color.Set(
                     SubElement.R + 30,
                     SubElement.G + 30,
                     SubElement.B + 30);
                public System.Drawing.Color Click => Color.Set(
                    Hover.R + 15,
                    Hover.G + 15,
                    Hover.B + 15);
                public System.Drawing.Color Selected => Color.Set(
                     Light.R + 30,
                     Light.G + 30,
                     Light.B + 30);
                public System.Drawing.Color Text => Color.Set(0, 0, 0);
            }

            public static BlueMode blueModeRef = new BlueMode();
            public class BlueMode : Style
            {
                public System.Drawing.Color Background => Color.Set(15, 25, 35);
                public System.Drawing.Color MainElement => Color.Set(
                    Background.R + 20,
                    Background.G + 20,
                    Background.B + 20);
                public System.Drawing.Color SubElement => Color.Set(
                   MainElement.R + 15,
                   MainElement.G + 15,
                   MainElement.B + 15);
                public System.Drawing.Color Dark => Color.Set(
                    SubElement.R + 15,
                    SubElement.G + 15,
                    SubElement.B + 15);
                public System.Drawing.Color Light => Color.Set(
                    Dark.R + 30,
                    Dark.G + 30,
                    Dark.B + 30);
                public System.Drawing.Color Line => Color.Set(
                     SubElement.R + 30,
                     SubElement.G + 30,
                     SubElement.B + 30);
                public System.Drawing.Color Hover => Color.Set(
                    SubElement.R + 30,
                    SubElement.G + 30,
                    SubElement.B + 30);
                public System.Drawing.Color Click => Color.Set(
                    Hover.R + 15,
                    Hover.G + 15,
                    Hover.B + 15);
                public System.Drawing.Color Selected => Color.Set(
                    Light.R + 30,
                    Light.G + 30,
                    Light.B + 30);
                public System.Drawing.Color Text => Color.Set(255, 255, 255);
            }


            public static CustomMode customModeRef = new CustomMode();
            public class CustomMode : Style
            {
                public System.Drawing.Color Background { get; set; } = 
                    (theme == Theme.dark) ? Style.darkModeRef.Background :
                    (theme == Theme.light) ? Style.lightModeRef.Background :
                    (theme == Theme.blue) ? Style.blueModeRef.Background :
                    Style.darkModeRef.Background;
                public System.Drawing.Color MainElement { get; set; } =
                    (theme == Theme.dark) ? Style.darkModeRef.MainElement :
                    (theme == Theme.light) ? Style.lightModeRef.MainElement :
                    (theme == Theme.blue) ? Style.blueModeRef.MainElement :
                    Style.darkModeRef.MainElement;
                public System.Drawing.Color SubElement { get; set; } =
                    (theme == Theme.dark) ? Style.darkModeRef.SubElement :
                    (theme == Theme.light) ? Style.lightModeRef.SubElement :
                    (theme == Theme.blue) ? Style.blueModeRef.SubElement :
                    Style.darkModeRef.SubElement;
                public System.Drawing.Color Dark { get; set; } =
                    (theme == Theme.dark) ? Style.darkModeRef.Dark :
                    (theme == Theme.light) ? Style.lightModeRef.Dark :
                    (theme == Theme.blue) ? Style.blueModeRef.Dark :
                    Style.darkModeRef.Dark;
                public System.Drawing.Color Light { get; set; } =
                    (theme == Theme.dark) ? Style.darkModeRef.Light :
                    (theme == Theme.light) ? Style.lightModeRef.Light :
                    (theme == Theme.blue) ? Style.blueModeRef.Light :
                    Style.darkModeRef.Light;
                public System.Drawing.Color Line { get; set; } =
                    (theme == Theme.dark) ? Style.darkModeRef.Line :
                    (theme == Theme.light) ? Style.lightModeRef.Line :
                    (theme == Theme.blue) ? Style.blueModeRef.Line :
                    Style.darkModeRef.Line;
                public System.Drawing.Color Hover { get; set; } =
                    (theme == Theme.dark) ? Style.darkModeRef.Hover :
                    (theme == Theme.light) ? Style.lightModeRef.Hover :
                    (theme == Theme.blue) ? Style.blueModeRef.Hover :
                    Style.darkModeRef.Hover;
                public System.Drawing.Color Click { get; set; } =
                    (theme == Theme.dark) ? Style.darkModeRef.Click :
                    (theme == Theme.light) ? Style.lightModeRef.Click :
                    (theme == Theme.blue) ? Style.blueModeRef.Click :
                    Style.darkModeRef.Click;
                public System.Drawing.Color Selected { get; set; } =
                    (theme == Theme.dark) ? Style.darkModeRef.Selected :
                    (theme == Theme.light) ? Style.lightModeRef.Selected :
                    (theme == Theme.blue) ? Style.blueModeRef.Selected :
                    Style.darkModeRef.Selected;
                public System.Drawing.Color Text { get; set; } =
                    (theme == Theme.dark) ? Style.darkModeRef.Text :
                    (theme == Theme.light) ? Style.lightModeRef.Text :
                    (theme == Theme.blue) ? Style.blueModeRef.Text :
                    Style.darkModeRef.Text;
                public System.Drawing.Color Icon { get; set; } = Color.Set(255, 0, 0);
            }
        }


        public enum ImageColor
        {
            black,
            white,
            custom
        }
        #region Color Elements
        public static ImageColor imageColor =>
            (theme == Theme.dark) ? ImageColor.white :
            (theme == Theme.light) ? ImageColor.black :
            (theme == Theme.blue) ? ImageColor.white :
            (theme == Theme.custom) ? ImageColor.custom :
            ImageColor.white;
        public static System.Drawing.Color Background =>
            (theme == Theme.dark) ? Style.darkModeRef.Background :
            (theme == Theme.light) ? Style.lightModeRef.Background :
            (theme == Theme.blue) ? Style.blueModeRef.Background :
            (theme == Theme.custom) ? Style.customModeRef.Background :
            Style.darkModeRef.Background;
        public static System.Drawing.Color MainElement =>
            (theme == Theme.dark) ? Style.darkModeRef.MainElement :
            (theme == Theme.light) ? Style.lightModeRef.MainElement :
            (theme == Theme.blue) ? Style.blueModeRef.MainElement :
            (theme == Theme.custom) ? Style.customModeRef.MainElement :
            Style.darkModeRef.MainElement;
        public static System.Drawing.Color SubElement =>
            (theme == Theme.dark) ? Style.darkModeRef.SubElement :
            (theme == Theme.light) ? Style.lightModeRef.SubElement :
            (theme == Theme.blue) ? Style.blueModeRef.SubElement :
            (theme == Theme.custom) ? Style.customModeRef.SubElement :
            Style.darkModeRef.SubElement;
        public static System.Drawing.Color Dark =>
            (theme == Theme.dark) ? Style.darkModeRef.Dark :
            (theme == Theme.light) ? Style.lightModeRef.Dark :
            (theme == Theme.blue) ? Style.blueModeRef.Dark :
            (theme == Theme.custom) ? Style.customModeRef.Dark :
            Style.darkModeRef.Dark;
        public static System.Drawing.Color Light =>
            (theme == Theme.dark) ? Style.darkModeRef.Light :
            (theme == Theme.light) ? Style.lightModeRef.Light :
            (theme == Theme.blue) ? Style.blueModeRef.Light :
            (theme == Theme.custom) ? Style.customModeRef.Light :
            Style.darkModeRef.Light;
        public static System.Drawing.Color Line =>
            (theme == Theme.dark) ? Style.darkModeRef.Line :
            (theme == Theme.light) ? Style.lightModeRef.Line :
            (theme == Theme.blue) ? Style.blueModeRef.Line :
            (theme == Theme.custom) ? Style.customModeRef.Line :
            Style.darkModeRef.Line;
        public static System.Drawing.Color Hover =>
            (theme == Theme.dark) ? Style.darkModeRef.Hover :
            (theme == Theme.light) ? Style.lightModeRef.Hover :
            (theme == Theme.blue) ? Style.blueModeRef.Hover :
            (theme == Theme.custom) ? Style.customModeRef.Hover :
            Style.darkModeRef.Hover;
        public static System.Drawing.Color Click =>
            (theme == Theme.dark) ? Style.darkModeRef.Click :
            (theme == Theme.light) ? Style.lightModeRef.Click :
            (theme == Theme.blue) ? Style.blueModeRef.Click :
            (theme == Theme.custom) ? Style.customModeRef.Click :
            Style.darkModeRef.Click;
        public static System.Drawing.Color Selected =>
            (theme == Theme.dark) ? Style.darkModeRef.Selected :
            (theme == Theme.light) ? Style.lightModeRef.Selected :
            (theme == Theme.blue) ? Style.blueModeRef.Selected :
            (theme == Theme.custom) ? Style.customModeRef.Selected :
            Style.darkModeRef.Selected;
        public static System.Drawing.Color Text =>
            (theme == Theme.dark) ? Style.darkModeRef.Text :
            (theme == Theme.light) ? Style.lightModeRef.Text :
            (theme == Theme.blue) ? Style.blueModeRef.Text :
            (theme == Theme.custom) ? Style.customModeRef.Text :
            Style.darkModeRef.Text;
        #endregion Color Elements
    }
}
