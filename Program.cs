using System;
using System.Diagnostics;
using System.ComponentModel;
using System.Threading;
using System.Windows.Forms;

namespace TrayTemps
{
    static class Program
    {
        private const string SingleInstanceMutexName = "TrayTemps_SingleInstance_Mutex";
        private const string ElevatedRestartArgument = "-elevated-restart";
        private const int ElevatedRestartRetryCount = 50;
        private const int ElevatedRestartRetryDelayMs = 100;

        public static bool RequestElevatedRestart(string arguments = null)
        {
            try
            {
                string combinedArguments = string.IsNullOrWhiteSpace(arguments)
                    ? ElevatedRestartArgument
                    : arguments + " " + ElevatedRestartArgument;

                var startInfo = new ProcessStartInfo
                {
                    FileName = Application.ExecutablePath,
                    WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory,
                    UseShellExecute = true,
                    Verb = "runas",
                    Arguments = combinedArguments
                };

                Process.Start(startInfo);
                return true;
            }
            catch (Win32Exception)
            {
                return false;
            }
        }

        [STAThread]
        static void Main(string[] args)
        {
            bool isElevatedRestart = HasArgument(args, ElevatedRestartArgument);
            bool createdNew;
            Mutex mutex = AcquireSingleInstanceMutex(isElevatedRestart, out createdNew);

            if (!createdNew || mutex == null)
                return;

            using (mutex)
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                var mainForm = new MainForm();

                if (HasArgument(args, "-silent"))
                {
                    mainForm.WindowState = FormWindowState.Minimized;
                    mainForm.ShowInTaskbar = false;
                }

                Application.Run(mainForm);
            }
        }

        private static Mutex AcquireSingleInstanceMutex(bool waitForPreviousInstance, out bool createdNew)
        {
            int attempts = waitForPreviousInstance ? ElevatedRestartRetryCount : 1;

            for (int i = 0; i < attempts; i++)
            {
                Mutex mutex = new Mutex(true, SingleInstanceMutexName, out createdNew);

                if (createdNew)
                    return mutex;

                mutex.Dispose();

                if (waitForPreviousInstance && i < attempts - 1)
                    Thread.Sleep(ElevatedRestartRetryDelayMs);
            }

            createdNew = false;
            return null;
        }

        private static bool HasArgument(string[] args, string expectedArgument)
        {
            return args != null &&
                   Array.Exists(args, arg => string.Equals(arg, expectedArgument, StringComparison.OrdinalIgnoreCase));
        }
    }
}
