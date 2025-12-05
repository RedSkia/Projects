using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LIB.Windows
{
    public class Drag
    {
        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();

        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr hWnd, int wMsg, int wParam, int lParam);

        /// <summary>
        /// Starts dragging on <paramref name="control"/>
        /// </summary>
        /// <param name="control"></param>
        public static void DragHandler(Control control)
        {
            ReleaseCapture();
            SendMessage(control.Handle, 0x112, 0xf012, 0);
        }

        /// <summary>
        /// Releases dragging from all controls
        /// </summary>
        public static void Release() => ReleaseCapture(); 
    }
}