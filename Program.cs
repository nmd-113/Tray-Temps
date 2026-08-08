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
        private const string DpiRestartArgument = "-dpi-restart";
        private const string SilentArgument = "-silent";
        private const int RestartRetryCount = 150;
        private const int RestartRetryDelayMs = 100;

        public static bool RequestElevatedRestart(bool startHidden = false, bool forceVisible = false)
        {
            try
            {
                string combinedArguments = forceVisible
                    ? DpiRestartArgument + " " + ElevatedRestartArgument
                    : startHidden
                        ? SilentArgument + " " + ElevatedRestartArgument
                        : ElevatedRestartArgument;

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

        public static bool RequestDpiRestart(bool startHidden)
        {
            try
            {
                var startInfo = new ProcessStartInfo
                {
                    FileName = Application.ExecutablePath,
                    WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory,
                    UseShellExecute = true,
                    Arguments = startHidden
                        ? DpiRestartArgument + " " + SilentArgument
                        : DpiRestartArgument
                };

                return Process.Start(startInfo) != null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("DPI restart failed: " + ex);
                return false;
            }
        }

        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            bool isElevatedRestart = HasArgument(args, ElevatedRestartArgument);
            bool isDpiRestart = HasArgument(args, DpiRestartArgument);
            bool startHidden = HasArgument(args, SilentArgument);
            bool createdNew;
            Mutex mutex = AcquireSingleInstanceMutex(isElevatedRestart || isDpiRestart, out createdNew);

            if (!createdNew || mutex == null)
                return;

            using (mutex)
            {
                var mainForm = new MainForm(startHidden, isDpiRestart && !startHidden);

                Application.Run(mainForm);
            }
        }

        private static Mutex AcquireSingleInstanceMutex(bool waitForPreviousInstance, out bool createdNew)
        {
            int attempts = waitForPreviousInstance ? RestartRetryCount : 1;

            for (int i = 0; i < attempts; i++)
            {
                Mutex mutex = new Mutex(true, SingleInstanceMutexName, out createdNew);

                if (createdNew)
                    return mutex;

                mutex.Dispose();

                if (waitForPreviousInstance && i < attempts - 1)
                    Thread.Sleep(RestartRetryDelayMs);
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
