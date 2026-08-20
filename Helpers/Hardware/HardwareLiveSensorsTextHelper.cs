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

            AppendLiveHardwareSensors(sb, hardware, "", includeUnavailableSensors: false);

            return sb.ToString();
        }

        internal static string BuildAllStorageSensorsText(
            IEnumerable<IHardware> storageHardwares,
            Action<IHardware> updateHardwareRecursive,
            IEnumerable<StorageLiveFallbackDevice> fallbackDevices)
        {
            var sb = new StringBuilder();
            List<IHardware> drives = storageHardwares?
                .Where(hardware => hardware != null)
                .ToList() ?? new List<IHardware>();
            List<StorageLiveFallbackDevice> fallbackList = fallbackDevices?
                .Where(device => device != null)
                .ToList() ?? new List<StorageLiveFallbackDevice>();

            AppendLiveSensorsHeader(sb, "LIVE STORAGE SENSORS");

            if (drives.Count == 0 && fallbackList.Count == 0)
            {
                sb.AppendLine("No storage sensors available.");
                return sb.ToString();
            }

            bool appendedAnyDrive = AppendAllStorageLiveSensors(
                sb,
                drives,
                updateHardwareRecursive,
                fallbackList);

            if (!appendedAnyDrive)
                sb.AppendLine("No storage sensors available.");

            return sb.ToString();
        }

        private static void AppendLiveSensorsHeader(StringBuilder sb, string title)
        {
            sb.AppendLine(title);
            sb.AppendLine("----------------------------------------------------------------");
            sb.AppendLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            sb.AppendLine();
        }

        private static bool AppendAllStorageLiveSensors(
            StringBuilder sb,
            IEnumerable<IHardware> storageHardwares,
            Action<IHardware> updateHardwareRecursive,
            List<StorageLiveFallbackDevice> fallbackDevices)
        {
            bool appendedAnyDrive = false;
            var matchedFallbackDevices = new HashSet<StorageLiveFallbackDevice>();

            foreach (var drive in storageHardwares)
            {
                try
                {
                    updateHardwareRecursive?.Invoke(drive);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("UpdateHardwareRecursive(drive) failed: " + ex);
                }

                // LHM 0.9.7-pre722 can expose storage sensors before their first value is
                // available. Keep those sensors visible as N/A instead of leaving
                // an otherwise valid drive section empty.
                StorageLiveFallbackDevice fallbackDevice = FindFallbackDevice(
                    drive,
                    fallbackDevices,
                    matchedFallbackDevices);

                if (fallbackDevice != null)
                    matchedFallbackDevices.Add(fallbackDevice);

                AppendLiveHardwareSensors(
                    sb,
                    drive,
                    "",
                    includeUnavailableSensors: true,
                    fallbackDevice?.Sensors);
                appendedAnyDrive = true;
            }

            foreach (StorageLiveFallbackDevice fallbackDevice in fallbackDevices)
            {
                if (matchedFallbackDevices.Contains(fallbackDevice))
                    continue;

                AppendFallbackDeviceSensors(sb, fallbackDevice);
                appendedAnyDrive = true;
            }

            return appendedAnyDrive;
        }

        private static StorageLiveFallbackDevice FindFallbackDevice(
            IHardware drive,
            IEnumerable<StorageLiveFallbackDevice> fallbackDevices,
            HashSet<StorageLiveFallbackDevice> alreadyMatched)
        {
            if (TryGetStorageIndex(drive, out int storageIndex))
            {
                StorageLiveFallbackDevice indexMatch = fallbackDevices.FirstOrDefault(device =>
                    !alreadyMatched.Contains(device) && device.Index == storageIndex);

                if (indexMatch != null)
                    return indexMatch;
            }

            string driveName = HardwareReportFormatHelper.NormalizeHardwareText(
                HardwareReportFormatHelper.Safe(drive?.Name));

            if (string.IsNullOrEmpty(driveName))
                return null;

            return fallbackDevices.FirstOrDefault(device =>
            {
                if (alreadyMatched.Contains(device))
                    return false;

                string fallbackName = HardwareReportFormatHelper.NormalizeHardwareText(device.Name);
                return !string.IsNullOrEmpty(fallbackName) &&
                    (string.Equals(driveName, fallbackName, StringComparison.OrdinalIgnoreCase) ||
                     driveName.Contains(fallbackName) ||
                     fallbackName.Contains(driveName));
            });
        }

        private static bool TryGetStorageIndex(IHardware drive, out int index)
        {
            index = -1;

            string identifier = drive?.Identifier.ToString();
            if (string.IsNullOrWhiteSpace(identifier))
                return false;

            int separatorIndex = identifier.LastIndexOf('/');
            string indexText = separatorIndex < 0
                ? identifier
                : identifier.Substring(separatorIndex + 1);

            return int.TryParse(indexText, out index);
        }

        private static void AppendLiveHardwareSensors(
            StringBuilder sb,
            IHardware hardware,
            string indent,
            bool includeUnavailableSensors,
            IEnumerable<StorageLiveFallbackSensor> fallbackSensors = null)
        {
            if (hardware == null)
                return;

            string hardwareName = HardwareReportFormatHelper.Safe(hardware.Name);
            sb.AppendLine($"{indent}{hardwareName}");
            sb.AppendLine($"{indent}{new string('-', Math.Min(64, hardwareName.Length + 8))}");

            var sensors = HardwareReportFormatHelper.DistinctSensors(hardware.Sensors)
                .Where(s => includeUnavailableSensors || s.Value.HasValue)
                .OrderBy(s => s.SensorType.ToString())
                .ThenBy(s => s.Name, HardwareReportFormatHelper.NaturalTextComparer)
                .ToList();
            bool unavailableLhmNvmeHealthSensors =
                HardwareReportFormatHelper.HasUnavailableLhmNvmeHealthSensors(sensors);
            if (unavailableLhmNvmeHealthSensors)
            {
                sensors = sensors
                    .Where(s => !HardwareReportFormatHelper.IsUnavailableLhmNvmeHealthSensor(s))
                    .ToList();
            }

            string normalizedNvmeLife = sensors.Any(HardwareReportFormatHelper.IsLhmNvmePercentageUsedSensor)
                ? HardwareReportFormatHelper.GetRemainingLifeText(sensors)
                : null;
            bool hasLhmNvmeLifeSensor = sensors.Any(HardwareReportFormatHelper.IsLhmNvmeLifeSensor);
            List<StorageLiveFallbackSensor> fallbackList = fallbackSensors?
                .Where(sensor => sensor != null)
                .OrderBy(sensor => sensor.SensorType.ToString())
                .ThenBy(sensor => sensor.Name, HardwareReportFormatHelper.NaturalTextComparer)
                .ToList() ?? new List<StorageLiveFallbackSensor>();
            var usedFallbackKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            if (sensors.Count == 0 && fallbackList.Count == 0)
            {
                if (!HasSensorsInSubHardware(hardware, includeUnavailableSensors))
                    sb.AppendLine($"{indent}No live sensors available.");
            }
            else
            {
                // Prefer LHM's actual NVMe Life sensor below.  Use the validated
                // normalized Percentage Used interpretation only if Life is absent.
                if (!hasLhmNvmeLifeSensor && !string.IsNullOrWhiteSpace(normalizedNvmeLife))
                {
                    sb.AppendLine(
                        $"{indent}{SensorType.Level,-13} {"Life Remaining",-36} {normalizedNvmeLife}");
                }

                foreach (var sensor in sensors)
                {
                    string sensorKey = GetStorageSensorMergeKey(sensor.Name);
                    StorageLiveFallbackSensor fallbackSensor = fallbackList.FirstOrDefault(candidate =>
                        string.Equals(
                            GetStorageSensorMergeKey(candidate.Name),
                            sensorKey,
                            StringComparison.OrdinalIgnoreCase));
                    string formattedValue = sensor.Value.HasValue || fallbackSensor == null
                        ? HardwareReportFormatHelper.FormatStorageSensorValue(sensor, sensors)
                        : fallbackSensor.FormattedValue;

                    if (fallbackSensor != null)
                        usedFallbackKeys.Add(sensorKey);

                    string sensorName = HardwareReportFormatHelper.IsLhmNvmeLifeSensor(sensor)
                        ? "Life Remaining"
                        : HardwareReportFormatHelper.Safe(sensor.Name);

                    sb.AppendLine(
                        $"{indent}{sensor.SensorType,-13} {sensorName,-36} {formattedValue}");
                }

                foreach (StorageLiveFallbackSensor fallbackSensor in fallbackList)
                {
                    string fallbackKey = GetStorageSensorMergeKey(fallbackSensor.Name);

                    if (!usedFallbackKeys.Add(fallbackKey))
                        continue;

                    AppendFallbackSensor(sb, fallbackSensor, indent);
                }
            }

            sb.AppendLine();

            foreach (var subHardware in hardware.SubHardware)
                AppendLiveHardwareSensors(sb, subHardware, indent + "  ", includeUnavailableSensors);
        }

        private static void AppendFallbackDeviceSensors(
            StringBuilder sb,
            StorageLiveFallbackDevice device)
        {
            string deviceName = HardwareReportFormatHelper.Safe(device.Name);
            sb.AppendLine(deviceName);
            sb.AppendLine(new string('-', Math.Min(64, deviceName.Length + 8)));

            if (device.Sensors.Count == 0)
            {
                sb.AppendLine("No live sensors available.");
            }
            else
            {
                foreach (StorageLiveFallbackSensor sensor in device.Sensors
                    .OrderBy(item => item.SensorType.ToString())
                    .ThenBy(item => item.Name, HardwareReportFormatHelper.NaturalTextComparer))
                {
                    AppendFallbackSensor(sb, sensor, "");
                }
            }

            sb.AppendLine();
        }

        private static void AppendFallbackSensor(
            StringBuilder sb,
            StorageLiveFallbackSensor sensor,
            string indent)
        {
            sb.AppendLine(
                $"{indent}{sensor.SensorType,-13} {HardwareReportFormatHelper.Safe(sensor.Name),-36} {sensor.FormattedValue}");
        }

        private static string GetStorageSensorMergeKey(string sensorName)
        {
            string normalized = HardwareReportFormatHelper.NormalizeHardwareText(
                HardwareReportFormatHelper.Safe(sensorName));

            if (normalized == "LIFE" ||
                (normalized.Contains("LIFE") &&
                 (normalized.Contains("REMAINING") || normalized.Contains("LEFT"))) ||
                normalized.Contains("PERCENTAGEUSED"))
            {
                return "LIFE";
            }

            return normalized;
        }

        private static bool HasSensorsInSubHardware(IHardware hardware, bool includeUnavailableSensors)
        {
            foreach (IHardware subHardware in hardware.SubHardware)
            {
                if (subHardware.Sensors.Any(sensor =>
                        sensor != null && (includeUnavailableSensors || sensor.Value.HasValue)) ||
                    HasSensorsInSubHardware(subHardware, includeUnavailableSensors))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
