using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LIB.Windows
{
    public class Dialog
    {
        public static DialogResult Show(string message, string title, [Optional] MessageBoxButtons buttons, [Optional] MessageBoxIcon icon, [Optional] MessageBoxDefaultButton defaultButton)
        {
            try { DialogResult dialog = MessageBox.Show(message, title, buttons, icon, defaultButton); return dialog; }
            catch { DialogResult dialog = MessageBox.Show(message, title); return dialog; }
        }
    }
}
