using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RustManager.FunctionClass
{
    class SendKeysClass
    {
   
            const int SW_RESTORE = 9;

            [System.Runtime.InteropServices.DllImport("User32.dll")]
            private static extern bool SetForegroundWindow(IntPtr handle);
            [System.Runtime.InteropServices.DllImport("User32.dll")]
            private static extern bool ShowWindow(IntPtr handle, int nCmdShow);
            [System.Runtime.InteropServices.DllImport("User32.dll")]
            private static extern bool IsIconic(IntPtr handle);

            public static void CustomSendKeys(Process proc, string Keys)
            {
                if (proc != null && proc.Responding)
                {
                    SendKeysClass.BringProcessToFront(proc);
                    System.Windows.Forms.SendKeys.SendWait(Keys);
                }
            }

            public static void BringProcessToFront(Process process)
            {
                IntPtr handle = process.MainWindowHandle;
                if (IsIconic(handle))
                {
                    ShowWindow(handle, SW_RESTORE);
                }
                SetForegroundWindow(handle);
            }


    }
}
