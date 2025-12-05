using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RustManager
{
    static class Program
    {
        // Unique id
        static Mutex mutex = new Mutex(true, "CB1AG38n;>wPe2;dB1a;83jOR%tKYJ()2vEHwe77WWfzJJ!l55ZrzBEOsn.7Ky31vS2/X2Px9Hpl./eidhU9/0S065pivzwx9Sd6,23BqCx8ls6Ij&3C*9)!ac98NGb9l8sE+nCrTs)VC0O#y,?V!9Ty&I6)9:40B2-saB3;MM/07wHP2wK6GU?rO:4MxYxYD8u&*w#<sdixk8GSaPp^ZxS,34*f#q$4ozw?c7~GZNNDM'gV2mM-J0>gPNloR6zb");
        [STAThread]
        static void Main()
        {
            if (mutex.WaitOne(TimeSpan.Zero, true))
            {
                Application.SetHighDpiMode(HighDpiMode.SystemAware);
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new MainForm(new byte[16]));

                mutex.ReleaseMutex();
            }
            else
            {
                Application.EnableVisualStyles();
                MessageBox.Show("Only one instance of this program is allowed.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
