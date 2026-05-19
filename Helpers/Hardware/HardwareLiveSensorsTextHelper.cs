using LibreHardwareMonitor.Hardware;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace TrayTemps
{
    internal static class HardwareLiveSensorsTextHelper
    {
        internal static string BuildLiveSensorsText(IHardware hardware)
        {
            var sb = new StringBuilder();

            AppendLiveSensorsHeader(sb, "LIVE SENSORS");

            if (hardware == null)
            {
                sb.AppendLine("No hardware selected.");
                return sb.ToString();
            }

            AppendLiveHardwareSensors(sb, hardware, "", false);

            return sb.ToString();
        }

        internal static string BuildAllStorageSensorsText(IEnumerable<IHardware> storageHardwares, Action<IHardware> updateHardwareRecursive)
        {
            var sb = new StringBuilder();

            AppendLiveSensorsHeader(sb, "LIVE STORAGE SENSORS");

            if (storageHardwares == null || !storageHardwares.Any())
            {
                sb.AppendLine("No storage sensors available.");
                return sb.ToString();
            }

            bool appendedAnyDrive = AppendAllStorageLiveSensors(sb, storageHardwares, updateHardwareRecursive);

            if (!appendedAnyDrive)
                sb.AppendLine("No storage sensors available.");

            return sb.ToString();
        }

        internal static void AppendSensorSummary(StringBuilder sb, IHardware hardware)
        {
            if (hardware == null)
                return;

            sb.Append(HardwareReportFormatHelper.Section("SENSORS"));

            try
            {
                hardware.Update();

                var sensors = hardware.Sensors
                    .Where(s =>
                        s.SensorType == SensorType.Temperature ||
                        s.SensorType == SensorType.Load ||
                        s.SensorType == SensorType.Clock ||
                        s.SensorType == SensorType.Power ||
                        s.SensorType == SensorType.Voltage ||
                        s.SensorType == SensorType.Fan)
                    .OrderBy(s => s.SensorType.ToString())
                    .ThenBy(s => s.Name)
                    .ToList();

                if (sensors.Count == 0)
                {
                    sb.AppendLine("  No sensors available.");
                    sb.AppendLine();
                    return;
                }

                foreach (var sensor in sensors)
                    sb.AppendLine(HardwareReportFormatHelper.Label($"{sensor.SensorType} / {sensor.Name}", HardwareReportFormatHelper.FormatSensorValue(sensor)));

                sb.AppendLine();
            }
            catch
            {
                sb.AppendLine("  Could not read sensors.");
                sb.AppendLine();
            }
        }

        private static void AppendLiveSensorsHeader(StringBuilder sb, string title)
        {
            sb.AppendLine(title);
            sb.AppendLine("----------------------------------------------------------------");
            sb.AppendLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            sb.AppendLine();
        }

        private static bool AppendAllStorageLiveSensors(StringBuilder sb, IEnumerable<IHardware> storageHardwares, Action<IHardware> updateHardwareRecursive)
        {
            bool appendedAnyDrive = false;

            foreach (var drive in storageHardwares)
            {
                if (drive == null)
                    continue;

                try
                {
                    updateHardwareRecursive?.Invoke(drive);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("UpdateHardwareRecursive(drive) failed: " + ex);
                }

                AppendLiveHardwareSensors(sb, drive, "", true);
                appendedAnyDrive = true;
            }

            return appendedAnyDrive;
        }

        private static void AppendLiveHardwareSensors(StringBuilder sb, IHardware hardware, string indent, bool validateStorageThroughput)
        {
            if (hardware == null)
                return;

            sb.AppendLine($"{indent}{hardware.Name}");
            sb.AppendLine($"{indent}{new string('-', Math.Min(64, HardwareReportFormatHelper.Safe(hardware.Name).Length + 8))}");

            var sensors = hardware.Sensors
                .Where(s => s.Value.HasValue)
                .OrderBy(s => s.SensorType.ToString())
                .ThenBy(s => s.Name)
                .ToList();

            if (sensors.Count == 0)
            {
                sb.AppendLine($"{indent}No live sensors available.");
            }
            else
            {
                foreach (var sensor in sensors)
                {
                    sb.AppendLine(
                        $"{indent}{sensor.SensorType,-13} {HardwareReportFormatHelper.Safe(sensor.Name),-36} {FormatLiveSensorValue(sensor, validateStorageThroughput)}");
                }
            }

            sb.AppendLine();

            foreach (var subHardware in hardware.SubHardware)
            {
                try
                {
                    subHardware.Update();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("subHardware.Update failed: " + ex);
                }

                AppendLiveHardwareSensors(sb, subHardware, indent + "  ", validateStorageThroughput);
            }
        }

        private static string FormatLiveSensorValue(ISensor sensor, bool validateStorageThroughput)
        {
            if (validateStorageThroughput && IsInvalidStorageThroughputValue(sensor))
                return "N/A";

            return HardwareReportFormatHelper.FormatSensorValue(sensor);
        }

        private static bool IsInvalidStorageThroughputValue(ISensor sensor)
        {
            if (sensor == null || !sensor.Value.HasValue)
                return false;

            if (sensor.SensorType != SensorType.Throughput)
                return false;

            string sensorName = HardwareReportFormatHelper.Safe(sensor.Name);
            bool isReadOrWriteRate =
                sensorName.IndexOf("Read Rate", StringComparison.OrdinalIgnoreCase) >= 0 ||
                sensorName.IndexOf("Write Rate", StringComparison.OrdinalIgnoreCase) >= 0;

            if (!isReadOrWriteRate)
                return false;

            float value = sensor.Value.Value;

            return float.IsNaN(value) ||
                   float.IsInfinity(value) ||
                   value < 0 ||
                   value > 100000f;
        }
    }
}
