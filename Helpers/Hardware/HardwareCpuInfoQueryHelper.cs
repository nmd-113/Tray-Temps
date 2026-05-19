using System;
using System.Collections.Generic;
using System.Management;
using System.Text;

namespace TrayTemps
{
    internal static class HardwareCpuInfoQueryHelper
    {
        internal static string GetCpuDetails(bool hasSelectedCpu, string selectedCpuName, string selectedCpuIdentifier, Func<string, List<ManagementObject>> wmiQuery)
        {
            var sb = new StringBuilder();
            sb.Append(HardwareReportFormatHelper.Section("CPU"));

            if (hasSelectedCpu)
            {
                sb.AppendLine(HardwareReportFormatHelper.Label("Selected CPU", selectedCpuName));
                sb.AppendLine(HardwareReportFormatHelper.Label("Identifier", selectedCpuIdentifier));
            }

            var cpus = wmiQuery("SELECT * FROM Win32_Processor");

            if (cpus.Count == 0)
            {
                sb.AppendLine();
                sb.AppendLine("  No CPU information found.");
                return sb.ToString();
            }

            int index = 1;

            foreach (var cpu in cpus)
            {
                sb.Append(HardwareReportFormatHelper.Group($"CPU #{index++}"));
                AppendCpuDetailsFields(sb, cpu);
            }

            return sb.ToString();
        }

        private static void AppendCpuDetailsFields(StringBuilder sb, ManagementObject cpu)
        {
            sb.AppendLine(HardwareReportFormatHelper.Label("Name", cpu["Name"]));
            sb.AppendLine(HardwareReportFormatHelper.Label("Manufacturer", cpu["Manufacturer"]));
            sb.AppendLine(HardwareReportFormatHelper.Label("Cores", cpu["NumberOfCores"]));
            sb.AppendLine(HardwareReportFormatHelper.Label("Threads", cpu["NumberOfLogicalProcessors"]));
            sb.AppendLine(HardwareReportFormatHelper.Label("Max Clock", HardwareReportFormatHelper.Unit(cpu["MaxClockSpeed"], "MHz")));
            sb.AppendLine(HardwareReportFormatHelper.Label("Socket", cpu["SocketDesignation"]));
            sb.AppendLine(HardwareReportFormatHelper.Label("Processor ID", cpu["ProcessorId"]));
            sb.AppendLine(HardwareReportFormatHelper.Label("L2 Cache", HardwareReportFormatHelper.Unit(cpu["L2CacheSize"], "KB")));
            sb.AppendLine(HardwareReportFormatHelper.Label("L3 Cache", HardwareReportFormatHelper.Unit(cpu["L3CacheSize"], "KB")));
            sb.AppendLine(HardwareReportFormatHelper.Label("Architecture", HardwareReportFormatHelper.GetCpuArchitectureString(cpu["Architecture"])));
        }
    }
}
