using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RustManager.Functions
{
    class CustomTooltip : System.Windows.Forms.ToolTip
    {
        public CustomTooltip()
        {
            BackColor = Color.FromArgb(10, 20, 30);
            ForeColor = Color.White;
            OwnerDraw = true;
            Draw += CustomTooltip_Draw;
            Popup += CustomTooltip_Popup;
            //ToolTipIcon = ToolTipIcon.Info;

        }

        private void CustomTooltip_Draw(object sender, DrawToolTipEventArgs e)
        {
            e.DrawBackground();
            e.DrawText();
        }








        private void CustomTooltip_Popup(object sender, PopupEventArgs e) // use this event to set the size of the tool tip
        {
            HideShadow();
        }

        public const int GCL_STYLE = -26;
        public const int CS_DROPSHADOW = 0x20000;
        [DllImport("user32.dll", EntryPoint = "GetClassLong")]
        public static extern int GetClassLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll", EntryPoint = "SetClassLong")]
        public static extern int SetClassLong(IntPtr hWnd, int nIndex, int dwNewLong);
        public void HideShadow()
        {
            var hwnd = (IntPtr)typeof(CustomTooltip).GetProperty("Handle",
                 System.Reflection.BindingFlags.NonPublic |
                 System.Reflection.BindingFlags.Instance).GetValue(this);
            var cs = GetClassLong(hwnd, GCL_STYLE);
            if ((cs & CS_DROPSHADOW) == CS_DROPSHADOW)
            {
                cs = cs & ~CS_DROPSHADOW;
                SetClassLong(hwnd, GCL_STYLE, cs);
            }
        }
    }
}
