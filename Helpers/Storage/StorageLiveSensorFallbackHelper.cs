using LibreHardwareMonitor.Hardware;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Management;

namespace TrayTemps
{
    internal static class StorageLiveSensorFallbackHelper
    {
        private static readonly object CacheLock = new object();
        private static readonly TimeSpan MetadataCacheDuration = TimeSpan.FromSeconds(30);
        private static List<StorageLiveFallbackDeviceMetadata> _metadataCache =
            new List<StorageLiveFallbackDeviceMetadata>();
        private static DateTime _metadataCacheTimeUtc = DateTime.MinValue;

        internal static List<StorageLiveFallbackDevice> GetSnapshot()
        {
            List<StorageLiveFallbackDeviceMetadata> metadata = GetMetadata();
            Dictionary<int, StorageLiveFallbackDevice> devices = metadata
                .GroupBy(item => item.Index)
                .ToDictionary(
                    group => group.Key,
                    group => new StorageLiveFallbackDevice(group.Key, group.First().Name));

            foreach (StorageLiveFallbackDeviceMetadata item in metadata)
            {
                if (!string.IsNullOrWhiteSpace(item.LifeRemaining))
                {
                    devices[item.Index].Sensors.Add(new StorageLiveFallbackSensor(
                        SensorType.Level,
                        "Life Remaining",
                        item.LifeRemaining));
                }
            }

            List<ManagementObject> logicalDisks = WmiQueryHelper.WmiQuery(
                "SELECT DeviceID, Size, FreeSpace FROM Win32_LogicalDisk WHERE DriveType = 3");
            List<ManagementObject> performanceRows = WmiQueryHelper.WmiQuery(
                "SELECT Name, DiskReadBytesPersec, DiskWriteBytesPersec, " +
                "PercentDiskReadTime, PercentDiskWriteTime, PercentDiskTime " +
                "FROM Win32_PerfFormattedData_PerfDisk_PhysicalDisk");

            try
            {
                Dictionary<string, LogicalDiskSpace> logicalSpace = BuildLogicalDiskSpace(logicalDisks);

                foreach (ManagementObject row in performanceRows)
                {
                    string instanceName = HardwareReportFormatHelper.Safe(row["Name"]);

                    if (!TryParsePhysicalDiskIndex(instanceName, out int diskIndex))
                        continue;

                    if (!devices.TryGetValue(diskIndex, out StorageLiveFallbackDevice device))
                    {
                        device = new StorageLiveFallbackDevice(diskIndex, "Disk " + diskIndex.ToString(CultureInfo.InvariantCulture));
                        devices.Add(diskIndex, device);
                    }

                    AddPercentSensor(device, row, "PercentDiskReadTime", "Read Activity");
                    AddPercentSensor(device, row, "PercentDiskWriteTime", "Write Activity");
                    AddPercentSensor(device, row, "PercentDiskTime", "Total Activity");
                    AddThroughputSensor(device, row, "DiskReadBytesPersec", "Read Rate");
                    AddThroughputSensor(device, row, "DiskWriteBytesPersec", "Write Rate");
                    AddSpaceSensors(device, instanceName, logicalSpace);
                }

                return devices.Values
                    .OrderBy(device => device.Index)
                    .ToList();
            }
            finally
            {
                WmiQueryHelper.DisposeAll(logicalDisks);
                WmiQueryHelper.DisposeAll(performanceRows);
            }
        }

        private static List<StorageLiveFallbackDeviceMetadata> GetMetadata()
        {
            lock (CacheLock)
            {
                if (DateTime.UtcNow - _metadataCacheTimeUtc < MetadataCacheDuration)
                    return new List<StorageLiveFallbackDeviceMetadata>(_metadataCache);

                List<ManagementObject> disks = WmiQueryHelper.WmiQuery(
                    "SELECT Index, Model, PNPDeviceID FROM Win32_DiskDrive");
                List<SmartLifeInfo> smartLifeInfos = StorageSmartInfoHelper.GetSmartLifeInfos(WmiQueryHelper.WmiQuery);
                var metadata = new List<StorageLiveFallbackDeviceMetadata>();

                try
                {
                    foreach (ManagementObject disk in disks)
                    {
                        if (!TryGetInt32(disk["Index"], out int index))
                            continue;

                        string name = HardwareReportFormatHelper.Safe(disk["Model"]);
                        if (name == "N/A")
                            name = "Disk " + index.ToString(CultureInfo.InvariantCulture);

                        SmartLifeInfo lifeInfo = StorageSmartInfoHelper.FindSmartLifeInfoForDisk(disk, smartLifeInfos);
                        string lifeRemaining = lifeInfo == null
                            ? null
                            : HardwareReportFormatHelper.FormatSmartLifeInfo(
                                lifeInfo.RemainingPercent,
                                lifeInfo.UsedPercent);

                        metadata.Add(new StorageLiveFallbackDeviceMetadata(index, name, lifeRemaining));
                    }
                }
                finally
                {
                    WmiQueryHelper.DisposeAll(disks);
                }

                _metadataCache = metadata;
                _metadataCacheTimeUtc = DateTime.UtcNow;
                return new List<StorageLiveFallbackDeviceMetadata>(_metadataCache);
            }
        }

        private static Dictionary<string, LogicalDiskSpace> BuildLogicalDiskSpace(
            IEnumerable<ManagementObject> logicalDisks)
        {
            var result = new Dictionary<string, LogicalDiskSpace>(StringComparer.OrdinalIgnoreCase);

            foreach (ManagementObject disk in logicalDisks)
            {
                string deviceId = HardwareReportFormatHelper.Safe(disk["DeviceID"]);

                if (deviceId == "N/A" || !TryGetUInt64(disk["Size"], out ulong size))
                    continue;

                TryGetUInt64(disk["FreeSpace"], out ulong freeSpace);
                result[deviceId] = new LogicalDiskSpace(size, freeSpace);
            }

            return result;
        }

        private static void AddSpaceSensors(
            StorageLiveFallbackDevice device,
            string performanceInstanceName,
            Dictionary<string, LogicalDiskSpace> logicalSpace)
        {
            ulong totalSize = 0;
            ulong totalFreeSpace = 0;

            foreach (string token in performanceInstanceName.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (!token.EndsWith(":", StringComparison.Ordinal) ||
                    !logicalSpace.TryGetValue(token, out LogicalDiskSpace space))
                {
                    continue;
                }

                totalSize += space.Size;
                totalFreeSpace += space.FreeSpace;
            }

            if (totalSize == 0)
                return;

            double usedPercent = 100d - (100d * totalFreeSpace / totalSize);
            device.Sensors.Add(new StorageLiveFallbackSensor(
                SensorType.Load,
                "Used Space",
                usedPercent.ToString("0.0", CultureInfo.InvariantCulture) + " %"));
            device.Sensors.Add(new StorageLiveFallbackSensor(
                SensorType.Data,
                "Free Space",
                HardwareReportFormatHelper.SizeHuman(totalFreeSpace)));
            device.Sensors.Add(new StorageLiveFallbackSensor(
                SensorType.Data,
                "Total Space",
                HardwareReportFormatHelper.SizeHuman(totalSize)));
        }

        private static void AddPercentSensor(
            StorageLiveFallbackDevice device,
            ManagementObject row,
            string propertyName,
            string sensorName)
        {
            if (!TryGetDouble(row[propertyName], out double value))
                return;

            device.Sensors.Add(new StorageLiveFallbackSensor(
                SensorType.Load,
                sensorName,
                value.ToString("0.0", CultureInfo.InvariantCulture) + " %"));
        }

        private static void AddThroughputSensor(
            StorageLiveFallbackDevice device,
            ManagementObject row,
            string propertyName,
            string sensorName)
        {
            if (!TryGetDouble(row[propertyName], out double value))
                return;

            device.Sensors.Add(new StorageLiveFallbackSensor(
                SensorType.Throughput,
                sensorName,
                HardwareReportFormatHelper.FormatBytesPerSecond(value)));
        }

        private static bool TryParsePhysicalDiskIndex(string instanceName, out int index)
        {
            index = -1;

            if (string.IsNullOrWhiteSpace(instanceName) ||
                instanceName.Equals("_Total", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            int separatorIndex = instanceName.IndexOf(' ');
            string indexText = separatorIndex < 0
                ? instanceName
                : instanceName.Substring(0, separatorIndex);

            return int.TryParse(
                indexText,
                NumberStyles.Integer,
                CultureInfo.InvariantCulture,
                out index);
        }

        private static bool TryGetInt32(object value, out int result)
        {
            try
            {
                result = Convert.ToInt32(value, CultureInfo.InvariantCulture);
                return true;
            }
            catch
            {
                result = 0;
                return false;
            }
        }

        private static bool TryGetUInt64(object value, out ulong result)
        {
            try
            {
                result = Convert.ToUInt64(value, CultureInfo.InvariantCulture);
                return true;
            }
            catch
            {
                result = 0;
                return false;
            }
        }

        private static bool TryGetDouble(object value, out double result)
        {
            try
            {
                result = Convert.ToDouble(value, CultureInfo.InvariantCulture);
                return !double.IsNaN(result) && !double.IsInfinity(result);
            }
            catch
            {
                result = 0;
                return false;
            }
        }

        private sealed class StorageLiveFallbackDeviceMetadata
        {
            internal StorageLiveFallbackDeviceMetadata(int index, string name, string lifeRemaining)
            {
                Index = index;
                Name = name;
                LifeRemaining = lifeRemaining;
            }

            internal int Index { get; }
            internal string Name { get; }
            internal string LifeRemaining { get; }
        }

        private struct LogicalDiskSpace
        {
            internal LogicalDiskSpace(ulong size, ulong freeSpace)
            {
                Size = size;
                FreeSpace = freeSpace;
            }

            internal ulong Size { get; }
            internal ulong FreeSpace { get; }
        }
    }

    internal sealed class StorageLiveFallbackDevice
    {
        internal StorageLiveFallbackDevice(int index, string name)
        {
            Index = index;
            Name = HardwareReportFormatHelper.Safe(name);
            Sensors = new List<StorageLiveFallbackSensor>();
        }

        internal int Index { get; }
        internal string Name { get; }
        internal List<StorageLiveFallbackSensor> Sensors { get; }
    }

    internal sealed class StorageLiveFallbackSensor
    {
        internal StorageLiveFallbackSensor(SensorType sensorType, string name, string formattedValue)
        {
            SensorType = sensorType;
            Name = HardwareReportFormatHelper.Safe(name);
            FormattedValue = HardwareReportFormatHelper.Safe(formattedValue);
        }

        internal SensorType SensorType { get; }
        internal string Name { get; }
        internal string FormattedValue { get; }
    }
}
