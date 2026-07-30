using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Management;
using System.Text;
using LibreHardwareMonitor.Hardware;

namespace TrayTemps
{
    internal static class HardwareReportFormatHelper
    {
        internal static string Section(string title)
        {
            title = Safe(title).ToUpperInvariant();

            return
                $"{title}\r\n" +
                $"{new string('═', 64)}\r\n";
        }

        internal static string Group(string title)
        {
            title = Safe(title);

            return
                $"\r\n[{title}]\r\n" +
                $"{new string('─', Math.Min(64, title.Length + 2))}\r\n";
        }

        internal static string Label(string key, object value)
        {
            string k = string.IsNullOrWhiteSpace(key) ? "Info" : key.Trim();
            string v = Safe(value);

            if (k.Length <= 20)
                return $"  {k,-20} : {v}";

            return
                $"  {k}\r\n" +
                $"  {new string(' ', 20)} : {v}";
        }

        internal static string Safe(object value)
        {
            if (value == null)
                return "N/A";

            string text = value.ToString();

            if (string.IsNullOrWhiteSpace(text))
                return "N/A";

            return text.Trim();
        }

        internal static string Unit(object value, string unit)
        {
            string text = Safe(value);

            if (text == "N/A")
                return text;

            return $"{text} {unit}";
        }

        internal static string SizeHuman(object bytesObj)
        {
            try
            {
                if (bytesObj == null)
                    return "N/A";

                double bytes = Convert.ToDouble(bytesObj);

                if (bytes <= 0)
                    return "0 B";

                string[] suffixes = { "B", "KB", "MB", "GB", "TB" };
                int index = 0;

                while (bytes >= 1024 && index < suffixes.Length - 1)
                {
                    bytes /= 1024;
                    index++;
                }

                return $"{bytes:0.0} {suffixes[index]}";
            }
            catch
            {
                return "N/A";
            }
        }

        internal static string FormatWmiDate(string value)
        {
            if (string.IsNullOrWhiteSpace(value) || value == "N/A")
                return "N/A";

            try
            {
                return ManagementDateTimeConverter.ToDateTime(value).ToString("yyyy-MM-dd");
            }
            catch
            {
                return value;
            }
        }

        internal static uint ToUInt(object value)
        {
            try
            {
                if (value == null)
                    return 0;

                return Convert.ToUInt32(value);
            }
            catch
            {
                return 0;
            }
        }

        internal static string RegistryValueToString(object value)
        {
            if (value == null)
                return string.Empty;

            if (value is string s)
                return s;

            if (value is string[] array)
                return string.Join(", ", array);

            return value.ToString();
        }

        internal static string GetCpuArchitectureString(object value)
        {
            uint code = ToUInt(value);

            switch (code)
            {
                case 0:
                    return "x86";

                case 5:
                    return "ARM";

                case 9:
                    return "x64";

                case 12:
                    return "ARM64";

                default:
                    return code == 0 ? "N/A" : code.ToString();
            }
        }

        internal static string GetMemoryTypeString(uint type)
        {
            switch (type)
            {
                case 20:
                    return "DDR";

                case 21:
                    return "DDR2";

                case 24:
                    return "DDR3";

                case 26:
                    return "DDR4";

                case 34:
                    return "DDR5";

                default:
                    return type == 0 ? "N/A" : $"Unknown ({type})";
            }
        }

        internal static string FormatRamConfiguration(List<long> capacities)
        {
            if (capacities == null || capacities.Count == 0) return "";

            var stickGroups = capacities.GroupBy(c => c / (1024 * 1024 * 1024))
                                        .Select(g => new { CapacityGB = g.Key, Count = g.Count() })
                                        .OrderByDescending(g => g.CapacityGB);

            string config = string.Join(" + ", stickGroups.Select(g => $"{g.Count}x{g.CapacityGB}GB"));
            return $"({config})";
        }

        internal static string FormatSmartLifeInfo(float remainingPercent, float? usedPercent)
        {
            if (usedPercent.HasValue)
                return $"{remainingPercent:0.0} % ({usedPercent.Value:0.0} % used)";

            return $"{remainingPercent:0.0} %";
        }

        internal static string GetDiskDisplayTitle(object modelValue, object serialValue)
        {
            string diskModel = Safe(modelValue);
            string diskSerial = Safe(serialValue);

            return !string.IsNullOrWhiteSpace(diskModel)
                ? diskModel
                : (!string.IsNullOrWhiteSpace(diskSerial) ? diskSerial : "Unknown Disk");
        }

        internal static bool IsStorageHealthSensor(ISensor sensor)
        {
            if (sensor == null)
                return false;

            string name = Safe(sensor.Name);

            if (sensor.SensorType == SensorType.Temperature)
                return true;

            string[] healthTerms =
            {
                "life",
                "health",
                "wear",
                "spare",
                "percentage used",
                "media errors",
                "unsafe shutdown",
                "power on hours",
                "power cycles",
                "data integrity",
                "available reserved"
            };

            return healthTerms.Any(term => name.IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0);
        }

        internal static int GetStorageHealthSortOrder(ISensor sensor)
        {
            string name = Safe(sensor?.Name);

            if (name.IndexOf("remaining", StringComparison.OrdinalIgnoreCase) >= 0)
                return 0;

            if (name.IndexOf("percentage used", StringComparison.OrdinalIgnoreCase) >= 0)
                return 1;

            if (name.IndexOf("temperature", StringComparison.OrdinalIgnoreCase) >= 0)
                return 2;

            if (name.IndexOf("spare", StringComparison.OrdinalIgnoreCase) >= 0)
                return 3;

            if (name.IndexOf("wear", StringComparison.OrdinalIgnoreCase) >= 0)
                return 4;

            return 10;
        }

        internal static string FormatSensorValue(ISensor sensor)
        {
            if (sensor == null || !sensor.Value.HasValue)
                return "N/A";

            float value = sensor.Value.Value;

            switch (sensor.SensorType)
            {
                case SensorType.Temperature:
                    return $"{value:0.0} °C";

                case SensorType.Load:
                case SensorType.Level:
                    return $"{value:0.0} %";

                case SensorType.Clock:
                    return $"{value:0} MHz";

                case SensorType.Power:
                    return $"{value:0.0} W";

                case SensorType.Voltage:
                    return $"{value:0.000} V";

                case SensorType.Fan:
                    return $"{value:0} RPM";

                case SensorType.Data:
                    return $"{value:0.0} GB";

                case SensorType.SmallData:
                    return $"{value:0.0} MB";

                case SensorType.Throughput:
                    return IsStorageReadWriteThroughputSensor(sensor)
                        ? $"{value:0.0} KB/s"
                        : $"{value:0.0} MB/s";

                case SensorType.TimeSpan:
                    return $"{value:0.0} h";

                case SensorType.Energy:
                    return $"{value:0.0} Wh";

                default:
                    return value.ToString("0.##", CultureInfo.InvariantCulture);
            }
        }

        internal static bool IsStorageReadWriteThroughputSensor(ISensor sensor)
        {
            if (sensor == null || sensor.SensorType != SensorType.Throughput)
                return false;

            string sensorName = Safe(sensor.Name);
            return sensorName.IndexOf("Read Rate", StringComparison.OrdinalIgnoreCase) >= 0 ||
                   sensorName.IndexOf("Write Rate", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        internal static string NormalizeGpuText(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            text = text.ToUpperInvariant();

            var sb = new StringBuilder(text.Length);

            foreach (char c in text)
            {
                if (char.IsLetterOrDigit(c))
                    sb.Append(c);
            }

            return sb.ToString();
        }

        internal static string NormalizeStorageText(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            text = text.ToUpperInvariant();
            var sb = new StringBuilder(text.Length);

            foreach (char c in text)
            {
                if (char.IsLetterOrDigit(c))
                    sb.Append(c);
            }

            return sb.ToString();
        }

        internal static string GetRemainingLifeText(IEnumerable<ISensor> sensors)
        {
            foreach (var sensor in sensors)
            {
                string name = Safe(sensor.Name);

                if (name.IndexOf("remaining", StringComparison.OrdinalIgnoreCase) >= 0 &&
                    name.IndexOf("life", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return FormatSensorValue(sensor);
                }

                if (name.IndexOf("life", StringComparison.OrdinalIgnoreCase) >= 0 &&
                    name.IndexOf("left", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return FormatSensorValue(sensor);
                }
            }

            foreach (var sensor in sensors)
            {
                string name = Safe(sensor.Name);

                if (name.IndexOf("percentage used", StringComparison.OrdinalIgnoreCase) >= 0 &&
                    sensor.Value.HasValue)
                {
                    float remaining = Math.Max(0, 100 - sensor.Value.Value);
                    return $"{remaining:0.0} % ({sensor.Value.Value:0.0} % used)";
                }
            }

            return null;
        }
    }
}
