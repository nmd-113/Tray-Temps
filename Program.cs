using System;
using System.Linq;
using System.Windows.Forms;

namespace TrayTemps
{
    static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var mainForm = new TrayTemps();

            if (args.Contains("-silent"))
            {
                mainForm.WindowState = FormWindowState.Minimized;
                mainForm.ShowInTaskbar = false;
            }

            Application.Run(mainForm);
        }
    }
}
