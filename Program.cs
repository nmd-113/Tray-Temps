using System;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace TrayTemps
{
    static class Program
    {
        private const string SingleInstanceMutexName = "TrayTemps_SingleInstance_Mutex";

        [STAThread]
        static void Main(string[] args)
        {
            using (var mutex = new Mutex(true, SingleInstanceMutexName, out bool createdNew))
            {
                if (!createdNew)
                    return;

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                var mainForm = new MainForm();

                if (args != null && args.Contains("-silent"))
                {
                    mainForm.WindowState = FormWindowState.Minimized;
                    mainForm.ShowInTaskbar = false;
                }

                Application.Run(mainForm);
            }
        }
    }
}