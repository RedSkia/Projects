using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UI.Elements
{
    internal class RichTextBox : System.Windows.Forms.RichTextBox
    {
        public RichTextBox()
        {
            this.SetStyle(ControlStyles.EnableNotifyMessage, true);
        }

        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        const int WM_MOUSEWHEEL = 0x020A;
        const int EM_SETZOOM = 0x04E1;

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (m.Msg == WM_MOUSEWHEEL)
            {
                if (Control.ModifierKeys == Keys.Control)
                {
                    SendMessage(this.Handle, EM_SETZOOM, IntPtr.Zero, IntPtr.Zero);
                }
            }
        }
    }
}