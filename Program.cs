using System;
using System.Diagnostics;
using System.ComponentModel;
using System.IO;
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
        private const int ErrorCancelled = 1223;
        private const int RestartRetryCount = 150;
        private const int RestartRetryDelayMs = 100;

        public static bool RequestElevatedRestart(bool startHidden = false, bool forceVisible = false)
        {
            string arguments = forceVisible
                ? DpiRestartArgument + " " + ElevatedRestartArgument
                : startHidden
                    ? SilentArgument + " " + ElevatedRestartArgument
                    : ElevatedRestartArgument;

            return StartApplication(arguments, "runas", "Elevated restart");
        }

        public static bool RequestDpiRestart(bool startHidden)
        {
            string arguments = startHidden
                ? DpiRestartArgument + " " + SilentArgument
                : DpiRestartArgument;

            return StartApplication(arguments, null, "DPI restart");
        }

        private static bool StartApplication(string arguments, string verb, string operationName)
        {
            try
            {
                var startInfo = new ProcessStartInfo
                {
                    FileName = Application.ExecutablePath,
                    WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory,
                    UseShellExecute = true,
                    Verb = verb ?? string.Empty,
                    Arguments = arguments
                };

                return Process.Start(startInfo) != null;
            }
            catch (Win32Exception ex)
            {
                if (ex.NativeErrorCode != ErrorCancelled)
                    Debug.WriteLine(operationName + " failed: " + ex);

                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(operationName + " failed: " + ex);
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
            bool isInternalRestart = isElevatedRestart || isDpiRestart;
            bool startHidden = HasArgument(args, SilentArgument);
            bool deferPawnIoPrompt = ShouldDeferPawnIoPrompt(startHidden, isDpiRestart && !startHidden);
            bool suppressStartupElevationPrompt = false;
            bool requireStartupElevationConsent = false;
            bool createdNew;
            Mutex mutex = AcquireSingleInstanceMutex(isInternalRestart, out createdNew);

            if (!createdNew || mutex == null)
                return;

            using (mutex)
            {
                if (!isInternalRestart && !deferPawnIoPrompt)
                {
                    PawnIoStartupAction pawnIoAction = PawnIoSetupHelper.EnsureCompatibleInstallation();

                    if (pawnIoAction == PawnIoStartupAction.ExitApplication)
                        return;

                    requireStartupElevationConsent =
                        pawnIoAction == PawnIoStartupAction.ContinueWithElevationConsent;

                    if (pawnIoAction == PawnIoStartupAction.RestartApplication)
                    {
                        if (RequestElevatedRestart(startHidden))
                            return;

                        MessageBox.Show(
                            "PawnIO was installed, but Windows did not allow TrayTemps to restart with administrator rights. " +
                            "TrayTemps will continue normally with the sensors and WMI fallbacks currently available.",
                            "TrayTemps Restart Failed",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);

                        suppressStartupElevationPrompt = true;
                    }
                }

                var mainForm = new MainForm(
                    startHidden,
                    isDpiRestart && !startHidden,
                    suppressStartupElevationPrompt,
                    requireStartupElevationConsent,
                    deferPawnIoPrompt);

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

        private static bool ShouldDeferPawnIoPrompt(bool startHidden, bool forceVisible)
        {
            if (forceVisible)
                return false;

            if (startHidden)
                return true;

            try
            {
                string settingsPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "TrayTemps",
                    "settings.json");

                if (!File.Exists(settingsPath))
                    return false;

                AppSettings settings = System.Text.Json.JsonSerializer.Deserialize<AppSettings>(File.ReadAllText(settingsPath));
                return settings?.StartMinimizedToTray == true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Could not read the startup visibility setting: " + ex);
                return false;
            }
        }
    }
}
