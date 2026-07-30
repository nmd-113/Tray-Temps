using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace TrayTemps
{
    internal static class HardwareInfoQueryHelper
    {
        internal static Task<string> GetMotherboardNameAsync()
        {
            return Task.Run(() =>
            {
                try
                {
                    using (var searcher = new ManagementObjectSearcher("select Manufacturer, Product from Win32_BaseBoard"))
                    using (var collection = searcher.Get())
                    {
                        foreach (ManagementObject obj in collection.Cast<ManagementObject>())
                        {
                            using (obj)
                            {
                                string manufacturer = obj["Manufacturer"]?.ToString().Trim() ?? "";
                                string product = obj["Product"]?.ToString().Trim() ?? "";
                                string fullName = $"{manufacturer} {product}".Trim();
                                return string.IsNullOrEmpty(fullName) ? "Unknown Motherboard" : fullName;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("GetMotherboardNameAsync failed: " + ex);
                }
                return "Unknown Motherboard";
            });
        }

        internal static Task<string> GetRamInfoAsync()
        {
            return Task.Run(() =>
            {
                try
                {
                    using (var searcher = new ManagementObjectSearcher("select Capacity, SMBIOSMemoryType, ConfiguredClockSpeed from Win32_PhysicalMemory"))
                    using (var collection = searcher.Get())
                    {
                        var individualCapacities = new List<long>();
                        uint memoryType = 0;
                        uint speed = 0;

                        foreach (ManagementObject stick in collection.Cast<ManagementObject>())
                        {
                            using (stick)
                            {
                                individualCapacities.Add(Convert.ToInt64(stick["Capacity"]));
                                if (memoryType == 0) memoryType = Convert.ToUInt32(stick["SMBIOSMemoryType"]);
                                if (speed == 0) speed = Convert.ToUInt32(stick["ConfiguredClockSpeed"]);
                            }
                        }

                        if (individualCapacities.Count == 0) return "Unknown RAM";

                        long totalCapacityGB = individualCapacities.Sum() / (1024 * 1024 * 1024);
                        string configString = HardwareReportFormatHelper.FormatRamConfiguration(individualCapacities);
                        string typeString = HardwareReportFormatHelper.GetMemoryTypeString(memoryType);

                        return $"{totalCapacityGB}GB {configString} {typeString} {speed}MHz";
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("GetRamInfoAsync failed: " + ex);
                    return "Unknown RAM";
                }
            });
        }

        internal static string GetRamDetails(Func<string, List<ManagementObject>> wmiQuery)
        {
            var sb = new StringBuilder();
            var modules = new StringBuilder();

            sb.Append(HardwareReportFormatHelper.Section("RAM"));

            var ram = wmiQuery("SELECT * FROM Win32_PhysicalMemory");
            try
            {
                if (ram.Count == 0)
                {
                    sb.AppendLine();
                    sb.AppendLine("  No RAM information found.");
                    return sb.ToString();
                }

                long totalBytes = 0;
                int index = 1;

                foreach (var mem in ram)
                {
                    object capacityObj = mem["Capacity"];

                    try
                    {
                        if (capacityObj != null)
                            totalBytes += Convert.ToInt64(capacityObj);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("GetMemoryDetails: failed to parse capacity: " + ex);
                    }

                    modules.Append(HardwareReportFormatHelper.Group($"Module #{index++}"));
                    AppendRamModuleFields(modules, mem, capacityObj);
                }

                sb.AppendLine(HardwareReportFormatHelper.Label("Total", HardwareReportFormatHelper.SizeHuman(totalBytes)));
                sb.Append(modules);

                return sb.ToString();
            }
            finally
            {
                WmiQueryHelper.DisposeAll(ram);
            }
        }

        private static void AppendRamModuleFields(StringBuilder sb, ManagementObject mem, object capacityObj)
        {
            sb.AppendLine(HardwareReportFormatHelper.Label("Manufacturer", mem["Manufacturer"]));
            sb.AppendLine(HardwareReportFormatHelper.Label("Capacity", HardwareReportFormatHelper.SizeHuman(capacityObj)));
            sb.AppendLine(HardwareReportFormatHelper.Label("Type", HardwareReportFormatHelper.GetMemoryTypeString(HardwareReportFormatHelper.ToUInt(mem["SMBIOSMemoryType"]))));
            sb.AppendLine(HardwareReportFormatHelper.Label("Speed", HardwareReportFormatHelper.Unit(mem["Speed"], "MHz")));
            sb.AppendLine(HardwareReportFormatHelper.Label("Configured Speed", HardwareReportFormatHelper.Unit(mem["ConfiguredClockSpeed"], "MHz")));
            sb.AppendLine(HardwareReportFormatHelper.Label("Part Number", mem["PartNumber"]));
            sb.AppendLine(HardwareReportFormatHelper.Label("Serial", mem["SerialNumber"]));
            sb.AppendLine(HardwareReportFormatHelper.Label("Bank", mem["BankLabel"]));
            sb.AppendLine(HardwareReportFormatHelper.Label("Slot", mem["DeviceLocator"]));
        }

        internal static string GetMotherboardDetails(Func<string, List<ManagementObject>> wmiQuery)
        {
            var sb = new StringBuilder();

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

            return sb.ToString();
        }

        private static void AppendMotherboardStaticFields(StringBuilder sb, ManagementObject board)
        {
            sb.AppendLine(HardwareReportFormatHelper.Label("Manufacturer", board["Manufacturer"]));
            sb.AppendLine(HardwareReportFormatHelper.Label("Product", board["Product"]));
            sb.AppendLine(HardwareReportFormatHelper.Label("Version", board["Version"]));
            sb.AppendLine(HardwareReportFormatHelper.Label("Serial", board["SerialNumber"]));
        }

        private static void AppendBiosFields(StringBuilder sb, ManagementObject bios)
        {
            sb.AppendLine(HardwareReportFormatHelper.Label("Vendor", bios["Manufacturer"]));
            sb.AppendLine(HardwareReportFormatHelper.Label("Version", bios["SMBIOSBIOSVersion"]));
            sb.AppendLine(HardwareReportFormatHelper.Label("Release Date", HardwareReportFormatHelper.FormatWmiDate(HardwareReportFormatHelper.Safe(bios["ReleaseDate"]))));
            sb.AppendLine(HardwareReportFormatHelper.Label("Serial", bios["SerialNumber"]));
        }
    }
}
