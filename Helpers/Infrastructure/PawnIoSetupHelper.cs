using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Windows.Forms;

namespace TrayTemps
{
    internal enum PawnIoStartupAction
    {
        Continue,
        ContinueWithElevationConsent,
        RestartApplication,
        ExitApplication
    }

    internal enum PawnIoInstallerResult
    {
        Success,
        Cancelled,
        RebootRequired,
        RebootInitiated,
        Failed
    }

    internal static class PawnIoSetupHelper
    {
        private const string ServiceRegistryPath = @"SYSTEM\CurrentControlSet\Services\PawnIO";
        private const string DeviceRegistryPath = @"SYSTEM\CurrentControlSet\Enum\ROOT\PAWNIO";
        private const string InstallerResourceName = "TrayTemps.Resources.PawnIO_setup.exe";
        private const string InstallerFileName = "PawnIO_setup.exe";
        private const string InstallerVersion = "2.2.0";
        private const string InstallerArguments = "-install -silent";
        private const string TempDirectoryPrefix = "TrayTemps-PawnIO-";
        private const string InstallerSha256 = "1F519A22E47187F70A1379A48CA604981C4FCF694F4E65B734AAA74A9FBA3032";
        private const long InstallerLength = 3410960;
        private const int ErrorCancelled = 1223;
        private const int ErrorInstallUserExit = 1602;
        private const int ErrorSuccessRebootInitiated = 1641;
        private const int ErrorSuccessRebootRequired = 3010;
        // Disabled, removed, reinstall-pending, or failed-install PnP instances are not usable.
        private const int InvalidDeviceConfigFlags = 0x1 | 0x2 | 0x20 | 0x40;
        private static readonly Version MinimumCompatibleVersion = new Version(2, 0);

        internal static PawnIoStartupAction EnsureCompatibleInstallation()
        {
            Version installedVersion = GetInstalledVersion();

            if (IsCompatibleVersion(installedVersion))
                return PawnIoStartupAction.Continue;

            string state = installedVersion == null
                ? "PawnIO is not installed."
                : "The installed PawnIO version (" + installedVersion + ") is not compatible.";

            DialogResult choice = MessageBox.Show(
                state + "\n\n" +
                "PawnIO enables LibreHardwareMonitor to access additional low-level hardware sensors. " +
                "Install the official PawnIO " + InstallerVersion + " driver now?\n\n" +
                "The official installer will run in silent install mode. Windows will request administrator approval for the installer only. " +
                "If you choose No, TrayTemps will continue with the sensors and WMI fallbacks currently available.",
                "PawnIO Sensor Driver",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (choice != DialogResult.Yes)
                return PawnIoStartupAction.ContinueWithElevationConsent;

            string tempDirectory = null;
            string installerPath = null;

            try
            {
                installerPath = ExtractInstaller(out tempDirectory);

                string validationError;
                if (!ValidateInstaller(installerPath, out validationError))
                {
                    ShowFailure("The embedded PawnIO installer failed integrity validation.\n\n" + validationError);
                    return PawnIoStartupAction.ContinueWithElevationConsent;
                }

                int exitCode = RunInstaller(installerPath, tempDirectory);
                PawnIoInstallerResult result = ClassifyInstallerExitCode(exitCode);

                switch (result)
                {
                    case PawnIoInstallerResult.Success:
                        MessageBox.Show(
                            "PawnIO was installed successfully. TrayTemps will now request administrator approval and restart for full hardware access.",
                            "PawnIO Installed",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                        return PawnIoStartupAction.RestartApplication;

                    case PawnIoInstallerResult.Cancelled:
                        MessageBox.Show(
                            "PawnIO installation was cancelled. TrayTemps will continue normally.",
                            "PawnIO Installation Cancelled",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                        return PawnIoStartupAction.ContinueWithElevationConsent;

                    case PawnIoInstallerResult.RebootRequired:
                        MessageBox.Show(
                            "PawnIO was installed, but Windows must be restarted before the driver is fully available.\n\n" +
                            "TrayTemps will request administrator approval and restart now, then continue with available sensor fallbacks until Windows is rebooted.",
                            "PawnIO Installed - Restart Required",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                        return PawnIoStartupAction.RestartApplication;

                    case PawnIoInstallerResult.RebootInitiated:
                        MessageBox.Show(
                            "PawnIO was installed and Windows has started a system restart. TrayTemps will close.",
                            "PawnIO Installed",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                        return PawnIoStartupAction.ExitApplication;

                    default:
                        ShowFailure("The PawnIO installer exited with code " + exitCode + " (0x" + exitCode.ToString("X8") + ").");
                        return PawnIoStartupAction.ContinueWithElevationConsent;
                }
            }
            catch (Win32Exception ex)
            {
                if (ex.NativeErrorCode == ErrorCancelled)
                {
                    MessageBox.Show(
                        "Administrator approval was cancelled. TrayTemps will continue normally.",
                        "PawnIO Installation Cancelled",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
                else
                {
                    ShowFailure("Windows could not start the PawnIO installer.\n\n" + ex.Message);
                }

                return PawnIoStartupAction.ContinueWithElevationConsent;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("PawnIO installation failed: " + ex);
                ShowFailure("PawnIO installation could not be completed.\n\n" + ex.Message);
                return PawnIoStartupAction.ContinueWithElevationConsent;
            }
            finally
            {
                CleanupTemporaryFiles(installerPath, tempDirectory);
            }
        }

        internal static bool IsCompatibleVersion(Version version)
        {
            return version != null && version >= MinimumCompatibleVersion;
        }

        internal static PawnIoInstallerResult ClassifyInstallerExitCode(int exitCode)
        {
            switch (exitCode)
            {
                case 0:
                    return PawnIoInstallerResult.Success;

                case ErrorCancelled:
                case ErrorInstallUserExit:
                    return PawnIoInstallerResult.Cancelled;

                case ErrorSuccessRebootRequired:
                    return PawnIoInstallerResult.RebootRequired;

                case ErrorSuccessRebootInitiated:
                    return PawnIoInstallerResult.RebootInitiated;

                default:
                    return PawnIoInstallerResult.Failed;
            }
        }

        internal static bool ValidateInstaller(string installerPath, out string error)
        {
            error = null;

            if (string.IsNullOrWhiteSpace(installerPath) || !File.Exists(installerPath))
            {
                error = "The extracted installer file was not found.";
                return false;
            }

            var file = new FileInfo(installerPath);
            if (file.Length != InstallerLength)
            {
                error = "The extracted installer size did not match the official PawnIO " + InstallerVersion + " release.";
                return false;
            }

            string actualHash;
            using (SHA256 sha256 = SHA256.Create())
            using (FileStream stream = new FileStream(installerPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                actualHash = BitConverter.ToString(sha256.ComputeHash(stream)).Replace("-", string.Empty);
            }

            if (!string.Equals(actualHash, InstallerSha256, StringComparison.OrdinalIgnoreCase))
            {
                error = "The extracted installer SHA-256 hash did not match the official PawnIO " + InstallerVersion + " release.";
                return false;
            }

            return true;
        }

        private static Version GetInstalledVersion()
        {
            return IsPawnIoDeviceRegistered()
                ? GetInstalledDriverVersion()
                : null;
        }

        private static bool IsPawnIoDeviceRegistered()
        {
            try
            {
                using (RegistryKey baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
                using (RegistryKey devicesKey = baseKey.OpenSubKey(DeviceRegistryPath))
                {
                    if (devicesKey == null)
                        return false;

                    foreach (string instanceName in devicesKey.GetSubKeyNames())
                    {
                        using (RegistryKey instanceKey = devicesKey.OpenSubKey(instanceName))
                        {
                            string service = instanceKey?.GetValue("Service")?.ToString().Trim();
                            int configFlags = Convert.ToInt32(instanceKey?.GetValue("ConfigFlags") ?? 0);

                            if (string.Equals(service, "PawnIO", StringComparison.OrdinalIgnoreCase) &&
                                (configFlags & InvalidDeviceConfigFlags) == 0)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("PawnIO device detection failed: " + ex);
            }

            return false;
        }

        private static Version GetInstalledDriverVersion()
        {
            try
            {
                using (RegistryKey baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
                using (RegistryKey key = baseKey.OpenSubKey(ServiceRegistryPath))
                {
                    int serviceType = Convert.ToInt32(key?.GetValue("Type") ?? 0);
                    int startType = Convert.ToInt32(key?.GetValue("Start") ?? 4);
                    string imagePath = key?.GetValue("ImagePath")?.ToString().Trim();
                    string driverPath = ResolveDriverPath(imagePath);

                    if (serviceType != 1 || startType == 4 ||
                        string.IsNullOrWhiteSpace(driverPath) ||
                        !string.Equals(Path.GetFileName(driverPath), "PawnIO.sys", StringComparison.OrdinalIgnoreCase) ||
                        !File.Exists(driverPath))
                    {
                        return null;
                    }

                    FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(driverPath);
                    if (versionInfo.FileMajorPart < 0 || versionInfo.FileMinorPart < 0 ||
                        versionInfo.FileBuildPart < 0 || versionInfo.FilePrivatePart < 0)
                    {
                        return null;
                    }

                    return new Version(
                        versionInfo.FileMajorPart,
                        versionInfo.FileMinorPart,
                        versionInfo.FileBuildPart,
                        versionInfo.FilePrivatePart);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("PawnIO driver version detection failed: " + ex);
                return null;
            }
        }

        private static string ResolveDriverPath(string imagePath)
        {
            if (string.IsNullOrWhiteSpace(imagePath))
                return null;

            string path = Environment.ExpandEnvironmentVariables(imagePath.Trim().Trim('"'));
            const string systemRootPrefix = @"\SystemRoot\";
            const string nativePathPrefix = @"\??\";

            if (path.StartsWith(systemRootPrefix, StringComparison.OrdinalIgnoreCase))
            {
                string systemRoot = Environment.GetEnvironmentVariable("SystemRoot");
                if (string.IsNullOrWhiteSpace(systemRoot))
                    return null;

                path = Path.Combine(systemRoot, path.Substring(systemRootPrefix.Length));
            }
            else if (path.StartsWith(nativePathPrefix, StringComparison.Ordinal))
            {
                path = path.Substring(nativePathPrefix.Length);
            }

            return Path.GetFullPath(path);
        }

        private static string ExtractInstaller(out string tempDirectory)
        {
            tempDirectory = Path.Combine(Path.GetTempPath(), TempDirectoryPrefix + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(tempDirectory);

            string installerPath = Path.Combine(tempDirectory, InstallerFileName);
            Assembly assembly = Assembly.GetExecutingAssembly();

            using (Stream resource = assembly.GetManifestResourceStream(InstallerResourceName))
            {
                if (resource == null)
                    throw new InvalidOperationException("The embedded PawnIO installer resource is unavailable.");

                using (var output = new FileStream(
                    installerPath,
                    FileMode.CreateNew,
                    FileAccess.Write,
                    FileShare.None,
                    81920,
                    FileOptions.WriteThrough))
                {
                    resource.CopyTo(output);
                    output.Flush(true);
                }
            }

            return installerPath;
        }

        private static int RunInstaller(string installerPath, string workingDirectory)
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = installerPath,
                Arguments = InstallerArguments,
                WorkingDirectory = workingDirectory,
                UseShellExecute = true,
                Verb = "runas"
            };

            using (Process process = Process.Start(startInfo))
            {
                if (process == null)
                    throw new InvalidOperationException("Windows did not start the PawnIO installer.");

                process.WaitForExit();
                return process.ExitCode;
            }
        }

        private static void CleanupTemporaryFiles(string installerPath, string tempDirectory)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(tempDirectory) &&
                    Directory.Exists(tempDirectory) &&
                    IsOwnedTemporaryDirectory(tempDirectory))
                {
                    string expectedInstallerPath = Path.Combine(tempDirectory, InstallerFileName);

                    if (!string.IsNullOrWhiteSpace(installerPath) &&
                        string.Equals(Path.GetFullPath(installerPath), Path.GetFullPath(expectedInstallerPath), StringComparison.OrdinalIgnoreCase) &&
                        File.Exists(installerPath))
                    {
                        File.Delete(installerPath);
                    }

                    Directory.Delete(tempDirectory, false);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("PawnIO temporary file cleanup failed: " + ex);
            }
        }

        private static bool IsOwnedTemporaryDirectory(string directory)
        {
            string fullDirectory = Path.GetFullPath(directory).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            string tempRoot = Path.GetFullPath(Path.GetTempPath()).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            string parent = Path.GetDirectoryName(fullDirectory);
            string name = Path.GetFileName(fullDirectory);

            return string.Equals(parent, tempRoot, StringComparison.OrdinalIgnoreCase) &&
                   name.StartsWith(TempDirectoryPrefix, StringComparison.Ordinal);
        }

        private static void ShowFailure(string message)
        {
            MessageBox.Show(
                message + "\n\nTrayTemps will continue normally with available sensors and WMI fallbacks.",
                "PawnIO Installation Failed",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
        }
    }
}
