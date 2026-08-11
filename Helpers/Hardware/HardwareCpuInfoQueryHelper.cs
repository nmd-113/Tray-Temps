using System;
using System.Collections.Generic;
using System.Management;
using System.Text;

namespace TrayTemps
{
    internal static class HardwareCpuInfoQueryHelper
    {
        internal static HardwareDiscoveryResult GetCpuInfo(Func<string, List<ManagementObject>> wmiQuery)
        {
            var sb = new StringBuilder();
            sb.Append(HardwareReportFormatHelper.Section("CPU"));
            var displayNames = new List<string>();

            var cpus = wmiQuery("SELECT * FROM Win32_Processor");
            try
            {
                if (cpus.Count == 0)
                {
                    sb.AppendLine();
                    sb.AppendLine("  No CPU information found.");
                    return new HardwareDiscoveryResult("Unknown CPU", sb.ToString(), displayNames, 0);
                }

                int index = 1;

                foreach (var cpu in cpus)
                {
                    string name = HardwareReportFormatHelper.NormalizeUnknownValue(cpu["Name"]);
                    if (name != "Unknown")
                        displayNames.Add(name);

                    sb.Append(HardwareReportFormatHelper.Group($"CPU #{index++}"));
                    AppendCpuDetailsFields(sb, cpu);
                }

                string summary = displayNames.Count == 1
                    ? displayNames[0]
                    : displayNames.Count > 1 ? "Processors" : "Unknown CPU";
                return new HardwareDiscoveryResult(summary, sb.ToString(), displayNames, cpus.Count);
            }
            finally
            {
                WmiQueryHelper.DisposeAll(cpus);
            }
        }

        internal static string FormatCpuDetails(string details, bool hasSelectedCpu, string selectedCpuName, string selectedCpuIdentifier)
        {
            if (!hasSelectedCpu)
                return details;

            string section = HardwareReportFormatHelper.Section("CPU");
            string selection =
                HardwareReportFormatHelper.Label("Selected CPU", selectedCpuName) + Environment.NewLine +
                HardwareReportFormatHelper.Label("Identifier", selectedCpuIdentifier) + Environment.NewLine;

            return details != null && details.StartsWith(section, StringComparison.Ordinal)
                ? section + selection + details.Substring(section.Length)
                : section + selection + (details ?? string.Empty);
        }

        private static void AppendCpuDetailsFields(StringBuilder sb, ManagementObject cpu)
        {
            sb.AppendLine(HardwareReportFormatHelper.Label("Name", cpu["Name"]));
            sb.AppendLine(HardwareReportFormatHelper.Label("Manufacturer", cpu["Manufacturer"]));
            sb.AppendLine(HardwareReportFormatHelper.Label("Cores", cpu["NumberOfCores"]));
            sb.AppendLine(HardwareReportFormatHelper.Label("Threads", cpu["NumberOfLogicalProcessors"]));
            sb.AppendLine(HardwareReportFormatHelper.Label("Base Clock", HardwareReportFormatHelper.Unit(cpu["MaxClockSpeed"], "MHz")));
            sb.AppendLine(HardwareReportFormatHelper.Label("Socket", cpu["SocketDesignation"]));
            sb.AppendLine(HardwareReportFormatHelper.Label("Processor ID", cpu["ProcessorId"]));
            sb.AppendLine(HardwareReportFormatHelper.Label("L2 Cache", HardwareReportFormatHelper.Unit(cpu["L2CacheSize"], "KB")));
            sb.AppendLine(HardwareReportFormatHelper.Label("L3 Cache", HardwareReportFormatHelper.Unit(cpu["L3CacheSize"], "KB")));
            sb.AppendLine(HardwareReportFormatHelper.Label("Architecture", HardwareReportFormatHelper.GetCpuArchitectureString(cpu["Architecture"])));
        }
    }
}
