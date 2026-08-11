using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Text;

namespace TrayTemps
{
    internal static class HardwareInfoQueryHelper
    {
        internal static HardwareDiscoveryResult GetRamInfo(Func<string, List<ManagementObject>> wmiQuery)
        {
            var sb = new StringBuilder();
            var modules = new StringBuilder();
            var individualCapacities = new List<long>();
            uint memoryType = 0;
            uint speed = 0;

            sb.Append(HardwareReportFormatHelper.Section("RAM"));

            var ram = wmiQuery("SELECT * FROM Win32_PhysicalMemory");
            try
            {
                if (ram.Count == 0)
                {
                    sb.AppendLine();
                    sb.AppendLine("  No RAM information found.");
                    return new HardwareDiscoveryResult("Unknown RAM", sb.ToString(), new List<string>(), 0);
                }

                long totalBytes = 0;
                int index = 1;

                foreach (var mem in ram)
                {
                    object capacityObj = mem["Capacity"];

                    try
                    {
                        if (capacityObj != null)
                        {
                            long capacity = Convert.ToInt64(capacityObj);
                            totalBytes += capacity;
                            individualCapacities.Add(capacity);
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("GetMemoryDetails: failed to parse capacity: " + ex);
                    }

                    modules.Append(HardwareReportFormatHelper.Group($"Module #{index++}"));
                    AppendRamModuleFields(modules, mem, capacityObj);

                    if (memoryType == 0)
                        memoryType = HardwareReportFormatHelper.ToUInt(mem["SMBIOSMemoryType"]);
                    if (speed == 0)
                        speed = HardwareReportFormatHelper.ToUInt(mem["ConfiguredClockSpeed"]);
                }

                sb.AppendLine(HardwareReportFormatHelper.Label("Total", HardwareReportFormatHelper.SizeHuman(totalBytes)));
                sb.Append(modules);

                long totalCapacityGb = totalBytes / (1024L * 1024L * 1024L);
                string configuration = HardwareReportFormatHelper.FormatRamConfiguration(individualCapacities);
                string type = HardwareReportFormatHelper.GetMemoryTypeString(memoryType);
                string speedText = speed == 0 ? string.Empty : " " + speed + "MHz";
                string summary = $"{totalCapacityGb}GB {configuration} {type}{speedText}".Trim();

                return new HardwareDiscoveryResult(summary, sb.ToString(), new List<string>(), ram.Count);
            }
            finally
            {
                WmiQueryHelper.DisposeAll(ram);
            }
        }

        private static void AppendRamModuleFields(StringBuilder sb, ManagementObject mem, object capacityObj)
        {
            object speed = mem["Speed"];
            object configuredSpeed = mem["ConfiguredClockSpeed"];

            sb.AppendLine(HardwareReportFormatHelper.Label("Manufacturer", HardwareReportFormatHelper.NormalizeUnknownValue(mem["Manufacturer"])));
            sb.AppendLine(HardwareReportFormatHelper.Label("Capacity", capacityObj == null ? "Unknown" : HardwareReportFormatHelper.SizeHuman(capacityObj)));
            sb.AppendLine(HardwareReportFormatHelper.Label("Type", HardwareReportFormatHelper.GetMemoryTypeString(HardwareReportFormatHelper.ToUInt(mem["SMBIOSMemoryType"]))));
            sb.AppendLine(HardwareReportFormatHelper.Label("Speed", FormatRamSpeed(speed)));

            if (!AreEquivalentRamSpeeds(speed, configuredSpeed))
                sb.AppendLine(HardwareReportFormatHelper.Label("Configured Speed", FormatRamSpeed(configuredSpeed)));

            sb.AppendLine(HardwareReportFormatHelper.Label("Part Number", HardwareReportFormatHelper.NormalizeUnknownValue(mem["PartNumber"])));
            sb.AppendLine(HardwareReportFormatHelper.Label("Serial", NormalizeRamSerialNumber(mem["SerialNumber"])));
            sb.AppendLine(HardwareReportFormatHelper.Label("Bank", HardwareReportFormatHelper.NormalizeUnknownValue(mem["BankLabel"])));
            sb.AppendLine(HardwareReportFormatHelper.Label("Slot", HardwareReportFormatHelper.NormalizeUnknownValue(mem["DeviceLocator"])));
        }

        private static string FormatRamSpeed(object value)
        {
            string text = HardwareReportFormatHelper.NormalizeUnknownValue(value);
            return text == "Unknown" ? text : text + " MHz";
        }

        private static bool AreEquivalentRamSpeeds(object speed, object configuredSpeed)
        {
            string speedText = HardwareReportFormatHelper.NormalizeUnknownValue(speed);
            string configuredText = HardwareReportFormatHelper.NormalizeUnknownValue(configuredSpeed);
            return string.Equals(speedText, configuredText, StringComparison.OrdinalIgnoreCase);
        }

        private static string NormalizeRamSerialNumber(object value)
        {
            string serial = HardwareReportFormatHelper.NormalizeUnknownValue(value);
            bool isAllZeroes = serial.Length > 0 && serial.All(character => character == '0');
            bool isAllFs = serial.Length > 0 && serial.All(character => character == 'F' || character == 'f');

            return isAllZeroes || isAllFs
                ? "Unknown"
                : serial;
        }

        internal static HardwareDiscoveryResult GetMotherboardInfo(Func<string, List<ManagementObject>> wmiQuery)
        {
            var sb = new StringBuilder();
            var displayNames = new List<string>();

            sb.Append(HardwareReportFormatHelper.Section("MOTHERBOARD"));

            var boards = wmiQuery("SELECT * FROM Win32_BaseBoard");
            try
            {
                if (boards.Count == 0)
                {
                    sb.AppendLine();
                    sb.AppendLine("  No motherboard information found.");
                }
                else
                {
                    int index = 1;
                    foreach (var board in boards)
                    {
                        string manufacturer = HardwareReportFormatHelper.NormalizeUnknownValue(board["Manufacturer"]);
                        string product = HardwareReportFormatHelper.NormalizeUnknownValue(board["Product"]);
                        string displayName = string.Join(" ", new[] { manufacturer, product }
                            .Where(value => !value.Equals("Unknown", StringComparison.OrdinalIgnoreCase)));

                        if (!string.IsNullOrWhiteSpace(displayName))
                            displayNames.Add(displayName);

                        sb.Append(HardwareReportFormatHelper.Group($"Motherboard #{index++}"));
                        AppendMotherboardStaticFields(sb, board);
                    }
                }
            }
            finally { WmiQueryHelper.DisposeAll(boards); }

            sb.AppendLine();
            sb.Append(HardwareReportFormatHelper.Section("BIOS"));

            var biosList = wmiQuery("SELECT * FROM Win32_BIOS");
            try
            {
                if (biosList.Count == 0)
                {
                    sb.AppendLine();
                    sb.AppendLine("  No BIOS information found.");
                }
                else
                {
                    int index = 1;
                    foreach (var bios in biosList)
                    {
                        sb.Append(HardwareReportFormatHelper.Group($"BIOS #{index++}"));
                        AppendBiosFields(sb, bios);
                    }
                }
            }
            finally { WmiQueryHelper.DisposeAll(biosList); }

            string summary = displayNames.Count > 0 ? displayNames[0] : "Unknown Motherboard";
            return new HardwareDiscoveryResult(summary, sb.ToString(), displayNames, boards.Count);
        }

        private static void AppendMotherboardStaticFields(StringBuilder sb, ManagementObject board)
        {
            sb.AppendLine(HardwareReportFormatHelper.Label("Manufacturer", HardwareReportFormatHelper.NormalizeUnknownValue(board["Manufacturer"])));
            sb.AppendLine(HardwareReportFormatHelper.Label("Product", HardwareReportFormatHelper.NormalizeUnknownValue(board["Product"])));
            sb.AppendLine(HardwareReportFormatHelper.Label("Version", HardwareReportFormatHelper.NormalizeUnknownValue(board["Version"])));
            sb.AppendLine(HardwareReportFormatHelper.Label("Serial", HardwareReportFormatHelper.NormalizeUnknownValue(board["SerialNumber"])));
        }

        private static void AppendBiosFields(StringBuilder sb, ManagementObject bios)
        {
            sb.AppendLine(HardwareReportFormatHelper.Label("Vendor", HardwareReportFormatHelper.NormalizeUnknownValue(bios["Manufacturer"])));
            sb.AppendLine(HardwareReportFormatHelper.Label("Version", HardwareReportFormatHelper.NormalizeUnknownValue(bios["SMBIOSBIOSVersion"])));
            sb.AppendLine(HardwareReportFormatHelper.Label("Release Date", HardwareReportFormatHelper.FormatWmiDate(HardwareReportFormatHelper.NormalizeUnknownValue(bios["ReleaseDate"]))));
            sb.AppendLine(HardwareReportFormatHelper.Label("Serial", HardwareReportFormatHelper.NormalizeUnknownValue(bios["SerialNumber"])));
        }
    }
}
