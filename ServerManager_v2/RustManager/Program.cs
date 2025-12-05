using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Media;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RustManager
{
    internal static class Program
    {
        // Unique id
        public const string MutexKey = "%Wly9QBz7j$J-1ULXjGkGo:%).fWJhWZ?X2r+YU16K5;:2hmc4k$>p3Z2LU1K*5/2nKz>D6>3Ut<z>*q'8UV<.0/9B4Id9PmUfI,r.o%xljS<7A47EjRzI3*20F*% x;:ehQt4=>9*,y1%h2Bba'%0>2ls4t6RR??qpN5qh?9Fq$SkbfU%d!$WwIguEba*HQC,y9Y8=k:agIKAIbqY$7O,BS:,H!2f?5>%K#1GV1'8'$P3>,1D0QgPcWU+&)BBy4b";
        static Mutex mutex = new Mutex(true, MutexKey);


        [DllImport("User32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);
        private static Process process;

        [DllImport("user32.dll")]
        private static extern bool IsWindowVisible(IntPtr hWnd);


        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            var processes = Process.GetProcessesByName($"{Application.ProductName}");

            foreach (Process p in processes) { process = p; break; }

   
            if (mutex.WaitOne(0, true)) //Single Instance 
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new RustManager.Elements.MainForm());

                mutex.ReleaseMutex();
            }
            else //Multi Instance 
            {
                if (Process.GetCurrentProcess().Id != process.Id)
                {
                    SetForegroundWindow(process.MainWindowHandle);
                    SystemSounds.Hand.Play();
                }
                else
                {
                    SetForegroundWindow(processes[1].MainWindowHandle);
                    SystemSounds.Hand.Play();
                }
            }
        }
    }
}