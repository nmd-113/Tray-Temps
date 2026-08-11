using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Text;
using System.Windows.Forms;

namespace TrayTemps
{
    internal static class HardwareGpuInfoQueryHelper
    {
        internal static HardwareDiscoveryResult GetGpuInfo(Func<string, List<ManagementObject>> wmiQuery)
        {
            var sb = new StringBuilder();
            sb.Append(HardwareReportFormatHelper.Section("GPU"));
            var displayNames = new List<string>();

            var gpus = wmiQuery("SELECT * FROM Win32_VideoController");
            try
            {
                if (gpus.Count == 0)
                {
                    sb.AppendLine();
                    sb.AppendLine("  No GPU information found.");
                    return new HardwareDiscoveryResult("Unknown GPU", sb.ToString(), displayNames, 0);
                }

                int index = 1;
                string displayResolutionSummary = GetDisplayResolutionSummary();
                bool hasDisplayResolutionSummary = !string.IsNullOrWhiteSpace(displayResolutionSummary);
                string resolutionLabel = hasDisplayResolutionSummary
                    ? (displayResolutionSummary.Contains(";") ? "Current Resolutions" : "Current Resolution")
                    : "Resolution";

                foreach (var gpu in gpus)
                {
                    string name = HardwareReportFormatHelper.NormalizeUnknownValue(gpu["Name"]);
                    if (name != "Unknown")
                        displayNames.Add(name);

                    sb.Append(HardwareReportFormatHelper.Group($"GPU #{index++}"));

                    string width = HardwareReportFormatHelper.Safe(gpu["CurrentHorizontalResolution"]);
                    string height = HardwareReportFormatHelper.Safe(gpu["CurrentVerticalResolution"]);
                    string refresh = HardwareReportFormatHelper.Safe(gpu["CurrentRefreshRate"]);
                    string resolutionValue = hasDisplayResolutionSummary
                        ? displayResolutionSummary
                        : $"{width} x {height} @ {refresh}Hz";
                    AppendGpuDetailsFields(sb, gpu, resolutionLabel, resolutionValue);
                }

                string summary = displayNames.Count == 1
                    ? displayNames[0]
                    : displayNames.Count > 1 ? "Graphics Adapters" : "Unknown GPU";
                return new HardwareDiscoveryResult(summary, sb.ToString(), displayNames, gpus.Count);
            }
            finally
            {
                WmiQueryHelper.DisposeAll(gpus);
            }
        }

        internal static string FormatGpuDetails(string details, string selectedGpuName, string selectedGpuIdentifier)
        {
            if (selectedGpuName == null && selectedGpuIdentifier == null)
                return details;

            string section = HardwareReportFormatHelper.Section("GPU");
            string selection =
                HardwareReportFormatHelper.Label("Selected GPU", selectedGpuName) + Environment.NewLine +
                HardwareReportFormatHelper.Label("Identifier", selectedGpuIdentifier) + Environment.NewLine;

            return details != null && details.StartsWith(section, StringComparison.Ordinal)
                ? section + selection + details.Substring(section.Length)
                : section + selection + (details ?? string.Empty);
        }

        private static void AppendGpuDetailsFields(StringBuilder sb, ManagementObject gpu, string resolutionLabel, string resolutionValue)
        {
            sb.AppendLine(HardwareReportFormatHelper.Label("Name", gpu["Name"]));
            sb.AppendLine(HardwareReportFormatHelper.Label("Driver", GetGpuDriverVersionText(gpu)));
            sb.AppendLine(HardwareReportFormatHelper.Label("Driver Date", HardwareReportFormatHelper.FormatWmiDate(HardwareReportFormatHelper.Safe(gpu["DriverDate"]))));
            sb.AppendLine(HardwareReportFormatHelper.Label("Video Processor", gpu["VideoProcessor"]));
            sb.AppendLine(HardwareReportFormatHelper.Label("Dedicated VRAM", GetGpuVramText(gpu)));
            sb.AppendLine(HardwareReportFormatHelper.Label(resolutionLabel, resolutionValue));
            sb.AppendLine(HardwareReportFormatHelper.Label("PNP Device ID", gpu["PNPDeviceID"]));
        }

        private static string GetGpuDriverVersionText(ManagementObject gpu)
        {
            string rawVersion = HardwareReportFormatHelper.NormalizeUnknownValue(gpu["DriverVersion"]);
            string gpuName = HardwareReportFormatHelper.Safe(gpu["Name"]);
            string pnpId = HardwareReportFormatHelper.Safe(gpu["PNPDeviceID"]);

            if (IsNvidiaGpu(gpuName, pnpId) && TryConvertNvidiaDriverVersion(rawVersion, out string nvidiaVersion))
                return nvidiaVersion;

            if (IsAmdGpu(gpuName, pnpId) && TryGetAmdSoftwareVersion(out string amdVersion))
                return amdVersion;

            // Intel's familiar DCH version uses the same four-part form exposed by WMI.
            return rawVersion;
        }

        private static bool IsNvidiaGpu(string name, string pnpId)
        {
            return name.IndexOf("NVIDIA", StringComparison.OrdinalIgnoreCase) >= 0 ||
                   pnpId.IndexOf("VEN_10DE", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private static bool IsAmdGpu(string name, string pnpId)
        {
            return name.IndexOf("AMD", StringComparison.OrdinalIgnoreCase) >= 0 ||
                   name.IndexOf("Radeon", StringComparison.OrdinalIgnoreCase) >= 0 ||
                   pnpId.IndexOf("VEN_1002", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private static bool TryConvertNvidiaDriverVersion(string rawVersion, out string vendorVersion)
        {
            vendorVersion = null;
            string[] parts = rawVersion.Split('.');

            if (parts.Length != 4 ||
                !int.TryParse(parts[2], out int thirdPart) ||
                !int.TryParse(parts[3], out int fourthPart) ||
                thirdPart < 10 || thirdPart > 19 ||
                fourthPart < 0 || fourthPart > 9999)
            {
                return false;
            }

            string vendorDigits = (thirdPart % 10).ToString() + fourthPart.ToString("D4");

            if (vendorDigits.Length != 5)
                return false;

            vendorVersion = vendorDigits.Substring(0, 3) + "." + vendorDigits.Substring(3, 2);
            return true;
        }

        private static bool TryGetAmdSoftwareVersion(out string version)
        {
            version = null;

            foreach (RegistryView view in new[] { RegistryView.Registry64, RegistryView.Registry32 })
            {
                try
                {
                    using (RegistryKey baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, view))
                    {
                        string candidate;

                        using (RegistryKey installedDriverKey = baseKey.OpenSubKey(
                            @"SOFTWARE\AMD\AMDInstallManager\CheckForUpdates\GraphicsDriverConsumer"))
                        {
                            candidate = installedDriverKey?.GetValue("VersionCurrentlyInstalled")?.ToString().Trim();
                        }

                        if (IsFamiliarAmdVersion(candidate))
                        {
                            version = candidate;
                            return true;
                        }

                        using (RegistryKey amdKey = baseKey.OpenSubKey(@"SOFTWARE\AMD\CN"))
                        {
                            candidate = amdKey?.GetValue("RadeonSoftwareVersion")?.ToString().Trim();
                        }

                        if (IsFamiliarAmdVersion(candidate))
                        {
                            version = candidate;
                            return true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Reading AMD software version failed: " + ex);
                }
            }

            return false;
        }

        private static bool IsFamiliarAmdVersion(string value)
        {
            if (string.IsNullOrWhiteSpace(value) || value.Length > 24)
                return false;

            string[] parts = value.Split('.');

            if (parts.Length < 2 || parts.Length > 3)
                return false;

            return parts.All(part => part.Length > 0 && part.Length <= 4 && part.All(char.IsDigit));
        }

        private static string GetGpuVramText(ManagementObject gpu)
        {

            if (TryGetGpuVramFromRegistry(gpu, out ulong bytes) && bytes > 0)
                return HardwareReportFormatHelper.SizeHuman(bytes);

            if (TryGetGpuVramFromWmi(gpu, out bytes) && bytes > 0)
                return HardwareReportFormatHelper.SizeHuman(bytes);

            return "N/A";
        }

        private static bool TryGetGpuVramFromWmi(ManagementObject gpu, out ulong bytes)
        {
            bytes = 0;

            try
            {
                object adapterRam = gpu["AdapterRAM"];

                if (adapterRam == null)
                    return false;

                if (adapterRam is uint v)
                {
                    bytes = v;
                    return bytes > 0;
                }

                if (adapterRam is int value)
                {
                    if (value <= 0)
                        return false;

                    bytes = (ulong)value;
                    return bytes > 0;
                }

                bytes = Convert.ToUInt64(adapterRam);
                return bytes > 0;
            }
            catch
            {
                return false;
            }
        }

        private static bool TryGetGpuVramFromRegistry(ManagementObject gpu, out ulong bytes)
        {
            bytes = 0;

            try
            {
                string gpuName = HardwareReportFormatHelper.Safe(gpu["Name"]);
                string pnpId = HardwareReportFormatHelper.Safe(gpu["PNPDeviceID"]);
                return TryGetGpuMemorySizeFromRegistry(gpuName, pnpId, out bytes);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("IsGpuMemoryAvailable failed: " + ex);
            }

            return false;
        }

        private static bool TryGetGpuMemorySizeFromRegistry(string gpuName, string pnpId, out ulong bytes)
        {
            bytes = 0;

            RegistryView[] views =
            {
                RegistryView.Registry64,
                RegistryView.Registry32
            };

            foreach (RegistryView view in views)
            {
                using (RegistryKey baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, view))
                using (RegistryKey videoKey = baseKey.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\Video"))
                {
                    if (videoKey == null)
                        continue;

                    foreach (string guidName in videoKey.GetSubKeyNames())
                    {
                        using (RegistryKey guidKey = videoKey.OpenSubKey(guidName))
                        {
                            if (guidKey == null)
                                continue;

                            foreach (string adapterSubKeyName in guidKey.GetSubKeyNames())
                            {
                                using (RegistryKey adapterKey = guidKey.OpenSubKey(adapterSubKeyName))
                                {
                                    if (adapterKey == null)
                                        continue;

                                    if (!IsGpuRegistryMatch(adapterKey, gpuName, pnpId))
                                        continue;

                                    if (TryReadGpuMemoryValue(adapterKey, "HardwareInformation.qwMemorySize", out bytes))
                                        return true;

                                    if (TryReadGpuMemoryValue(adapterKey, "HardwareInformation.MemorySize", out bytes))
                                        return true;
                                }
                            }
                        }
                    }
                }
            }

            return false;
        }

        private static string GetDisplayResolutionSummary()
        {
            try
            {
                var resolutions = Screen.AllScreens
                    .Select(screen => $"{screen.Bounds.Width}x{screen.Bounds.Height}")
                    .Where(text => !string.IsNullOrWhiteSpace(text))
                    .ToList();

                if (resolutions.Count == 0)
                    return null;

                return string.Join("; ", resolutions);
            }
            catch
            {
                return null;
            }
        }

        private static bool IsGpuRegistryMatch(RegistryKey adapterKey, string gpuName, string pnpId)
        {
            string adapterString = HardwareReportFormatHelper.RegistryValueToString(adapterKey.GetValue("HardwareInformation.AdapterString"));
            string chipType = HardwareReportFormatHelper.RegistryValueToString(adapterKey.GetValue("HardwareInformation.ChipType"));
            string matchingDeviceId = HardwareReportFormatHelper.RegistryValueToString(adapterKey.GetValue("MatchingDeviceId"));
            string deviceDescription = HardwareReportFormatHelper.RegistryValueToString(adapterKey.GetValue("Device Description"));
            string providerName = HardwareReportFormatHelper.RegistryValueToString(adapterKey.GetValue("ProviderName"));

            string registryText = $"{adapterString} {chipType} {matchingDeviceId} {deviceDescription} {providerName}";
            string regNorm = HardwareReportFormatHelper.NormalizeGpuText(registryText);
            string nameNorm = HardwareReportFormatHelper.NormalizeGpuText(gpuName);
            string pnpNorm = HardwareReportFormatHelper.NormalizeGpuText(pnpId);

            if (!string.IsNullOrWhiteSpace(nameNorm) && regNorm.Contains(nameNorm))
                return true;

            if (!string.IsNullOrWhiteSpace(pnpNorm))
            {
                string shortPnp = pnpNorm;

                int revIndex = shortPnp.IndexOf("REV", StringComparison.OrdinalIgnoreCase);
                if (revIndex > 0)
                    shortPnp = shortPnp.Substring(0, revIndex);

                if (!string.IsNullOrWhiteSpace(shortPnp) && regNorm.Contains(shortPnp))
                    return true;
            }

            return false;
        }

        private static bool TryReadGpuMemoryValue(RegistryKey key, string valueName, out ulong bytes)
        {
            bytes = 0;

            try
            {
                object value = key.GetValue(valueName);

                if (value == null)
                    return false;

                if (value is long)
                {
                    long v = (long)value;

                    if (v <= 0)
                        return false;

                    bytes = (ulong)v;
                    return true;
                }

                if (value is int)
                {
                    int v = (int)value;

                    if (v <= 0)
                        return false;

                    bytes = (uint)v;
                    return true;
                }

                if (value is byte[] data)
                {
                    if (data.Length >= 8)
                    {
                        bytes = BitConverter.ToUInt64(data, 0);
                        return bytes > 0;
                    }

                    if (data.Length >= 4)
                    {
                        bytes = BitConverter.ToUInt32(data, 0);
                        return bytes > 0;
                    }
                }

                bytes = Convert.ToUInt64(value);
                return bytes > 0;
            }
            catch
            {
                return false;
            }
        }
    }
}
