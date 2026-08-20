using System;
using System.Diagnostics;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;

namespace TrayTemps
{
    static class Program
    {
        private const string SingleInstanceMutexName = "TrayTemps_SingleInstance_Mutex";
        private const string ElevatedRestartArgument = "-elevated-restart";
        private const string DpiRestartArgument = "-dpi-restart";
        private const string DpiRestartPageArgumentPrefix = "-dpi-page=";
        private const string DeferredPawnIoPromptArgument = "-defer-pawnio-prompt";
        private const string SilentArgument = "-silent";
        private const string ConfiguredRuntimeDataKey = "TrayTemps.ConfiguredRuntime";
        private const string EmbeddedConfigurationResourceName = "TrayTemps.App.config";
        private const int ErrorCancelled = 1223;
        private const int RestartRetryCount = 150;
        private const int RestartRetryDelayMs = 100;
        private static Mutex _singleInstanceMutex;

        public static bool RequestElevatedRestart(bool startHidden = false, bool forceVisible = false)
        {
            string arguments = forceVisible
                ? DpiRestartArgument + " " + ElevatedRestartArgument
                : startHidden
                    ? SilentArgument + " " + ElevatedRestartArgument
                    : ElevatedRestartArgument;

            return StartApplication(arguments, "runas", "Elevated restart");
        }

        public static bool RequestDpiRestart(
            bool startHidden,
            bool deferPawnIoPrompt,
            int selectedPageIndex)
        {
            string arguments = DpiRestartArgument +
                (startHidden ? " " + SilentArgument : string.Empty) +
                (deferPawnIoPrompt ? " " + DeferredPawnIoPromptArgument : string.Empty) +
                " " + DpiRestartPageArgumentPrefix + selectedPageIndex;

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
            bool isConfiguredRuntime =
                AppDomain.CurrentDomain.GetData(ConfiguredRuntimeDataKey) is bool configured && configured;

            if (!isConfiguredRuntime && TryRunConfiguredRuntime(args))
                return;

            RunApplication(args);
        }

        private static void RunApplication(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            bool isElevatedRestart = HasArgument(args, ElevatedRestartArgument);
            bool isDpiRestart = HasArgument(args, DpiRestartArgument);
            bool isInternalRestart = isElevatedRestart || isDpiRestart;
            bool startHidden = HasArgument(args, SilentArgument);
            bool forceVisible = isDpiRestart && !startHidden;
            bool deferPawnIoPrompt = isDpiRestart
                ? HasArgument(args, DeferredPawnIoPromptArgument)
                : ShouldDeferPawnIoPrompt(startHidden, forceVisible: false);
            int selectedPageIndex = isDpiRestart
                ? GetIntArgument(args, DpiRestartPageArgumentPrefix, 0, 2)
                : 0;
            bool suppressStartupElevationPrompt = isDpiRestart;
            bool requireStartupElevationConsent = false;
            bool createdNew;
            Mutex mutex = AcquireSingleInstanceMutex(isInternalRestart, out createdNew);

            if (!createdNew || mutex == null)
                return;

            // Keep the named object alive until the configured runtime AppDomain
            // is unloaded. This prevents an internal restart from taking ownership
            // while shutdown continuations from the old runtime are still unwinding.
            _singleInstanceMutex = mutex;

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
                forceVisible,
                suppressStartupElevationPrompt,
                requireStartupElevationConsent,
                deferPawnIoPrompt,
                selectedPageIndex);

            Application.Run(mainForm);
            GC.KeepAlive(_singleInstanceMutex);
        }

        private static bool TryRunConfiguredRuntime(string[] args)
        {
            AppDomain runtimeDomain = null;
            bool runtimeStarted = false;

            try
            {
                string executablePath = Assembly.GetExecutingAssembly().Location;
                if (string.IsNullOrWhiteSpace(executablePath))
                    return false;

                string runtimeDirectory = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "TrayTemps",
                    "runtime");
                string configurationPath = Path.Combine(runtimeDirectory, "TrayTemps.exe.config");

                byte[] configurationBytes;
                using (Stream stream = Assembly.GetExecutingAssembly()
                    .GetManifestResourceStream(EmbeddedConfigurationResourceName))
                {
                    if (stream == null)
                        return false;

                    using (var buffer = new MemoryStream())
                    {
                        stream.CopyTo(buffer);
                        configurationBytes = buffer.ToArray();
                    }
                }

                EnsureRuntimeConfiguration(configurationPath, configurationBytes);

                var setup = new AppDomainSetup
                {
                    ApplicationBase = Path.GetDirectoryName(executablePath),
                    ConfigurationFile = configurationPath
                };

                runtimeDomain = AppDomain.CreateDomain("TrayTemps Configured Runtime", null, setup);
                runtimeDomain.SetData(ConfiguredRuntimeDataKey, true);

                runtimeStarted = true;
                Environment.ExitCode = runtimeDomain.ExecuteAssembly(executablePath, args ?? new string[0]);
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Configured runtime startup failed: " + ex);

                if (runtimeStarted)
                    Environment.ExitCode = 1;

                return runtimeStarted;
            }
            finally
            {
                if (runtimeDomain != null)
                {
                    try
                    {
                        AppDomain.Unload(runtimeDomain);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("Configured runtime cleanup failed: " + ex);
                    }
                }
            }
        }

        private static void EnsureRuntimeConfiguration(string configurationPath, byte[] expectedBytes)
        {
            if (IsExpectedConfiguration(configurationPath, expectedBytes))
                return;

            string directory = Path.GetDirectoryName(configurationPath);
            Directory.CreateDirectory(directory);

            string temporaryPath = configurationPath + "." + Process.GetCurrentProcess().Id + ".tmp";
            try
            {
                using (var output = new FileStream(
                    temporaryPath,
                    FileMode.Create,
                    FileAccess.Write,
                    FileShare.None))
                {
                    output.Write(expectedBytes, 0, expectedBytes.Length);
                    output.Flush(true);
                }

                try
                {
                    if (File.Exists(configurationPath))
                        File.Replace(temporaryPath, configurationPath, null);
                    else
                        File.Move(temporaryPath, configurationPath);
                }
                catch (IOException)
                {
                    // Another instance may have completed the same atomic update first.
                    if (!IsExpectedConfiguration(configurationPath, expectedBytes))
                        throw;
                }

                if (!IsExpectedConfiguration(configurationPath, expectedBytes))
                    throw new IOException("The embedded runtime configuration could not be validated.");
            }
            finally
            {
                try
                {
                    if (File.Exists(temporaryPath))
                        File.Delete(temporaryPath);
                }
                catch (IOException)
                {
                    // A stale temporary file is harmless and can be replaced later.
                }
                catch (UnauthorizedAccessException)
                {
                    // A stale temporary file is harmless and can be replaced later.
                }
            }
        }

        private static bool IsExpectedConfiguration(string configurationPath, byte[] expectedBytes)
        {
            try
            {
                return File.Exists(configurationPath) &&
                       BytesEqual(File.ReadAllBytes(configurationPath), expectedBytes);
            }
            catch (IOException)
            {
                return false;
            }
        }

        private static bool BytesEqual(byte[] left, byte[] right)
        {
            if (left == null || right == null || left.Length != right.Length)
                return false;

            for (int i = 0; i < left.Length; i++)
            {
                if (left[i] != right[i])
                    return false;
            }

            return true;
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

        private static int GetIntArgument(string[] args, string prefix, int minimum, int maximum)
        {
            if (args == null)
                return minimum;

            for (int i = 0; i < args.Length; i++)
            {
                string argument = args[i];
                if (argument == null || !argument.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                    continue;

                if (int.TryParse(argument.Substring(prefix.Length), out int value) &&
                    value >= minimum && value <= maximum)
                {
                    return value;
                }
            }

            return minimum;
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
