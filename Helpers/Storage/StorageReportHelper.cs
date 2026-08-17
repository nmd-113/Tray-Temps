using LibreHardwareMonitor.Hardware;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Text;

namespace TrayTemps
{
    internal static class StorageReportHelper
    {
        internal static void AppendStorageStaticDiskFields(StringBuilder sb, ManagementObject disk, List<ManagementObject> physicalDisks)
        {
            sb.AppendLine(HardwareReportFormatHelper.Label("Model", disk["Model"]));
            sb.AppendLine(HardwareReportFormatHelper.Label("Interface", GetStorageInterfaceText(disk, physicalDisks)));
            sb.AppendLine(HardwareReportFormatHelper.Label("Media Type", disk["MediaType"]));
            sb.AppendLine(HardwareReportFormatHelper.Label("Size", HardwareReportFormatHelper.SizeHuman(disk["Size"])));
            sb.AppendLine(HardwareReportFormatHelper.Label("Serial", disk["SerialNumber"]));
            sb.AppendLine(HardwareReportFormatHelper.Label("Firmware", disk["FirmwareRevision"]));
            sb.AppendLine(HardwareReportFormatHelper.Label("Partitions", disk["Partitions"]));
            sb.AppendLine(HardwareReportFormatHelper.Label("PNP ID", disk["PNPDeviceID"]));
        }

        private static string GetStorageInterfaceText(ManagementObject disk, List<ManagementObject> physicalDisks)
        {
            ManagementObject physicalDisk = FindPhysicalDisk(disk, physicalDisks);
            uint busType = physicalDisk == null
                ? 0
                : HardwareReportFormatHelper.ToUInt(physicalDisk["BusType"]);

            // MSFT_PhysicalDisk exposes the actual bus even when Win32_DiskDrive
            // reports the storage-port compatibility value "SCSI".
            if (busType == 17)
                return "NVMe";

            if (busType == 11)
                return "SATA";

            string interfaceType = HardwareReportFormatHelper.Safe(disk["InterfaceType"]);
            string pnpDeviceId = HardwareReportFormatHelper.Safe(disk["PNPDeviceID"]);

            if ((physicalDisk == null || busType <= 1) &&
                interfaceType.Equals("SCSI", StringComparison.OrdinalIgnoreCase) &&
                pnpDeviceId.StartsWith(@"SCSI\DISK&VEN_NVME", StringComparison.OrdinalIgnoreCase))
            {
                return "NVMe";
            }

            return interfaceType;
        }

        private static ManagementObject FindPhysicalDisk(ManagementObject disk, List<ManagementObject> physicalDisks)
        {
            if (disk == null || physicalDisks == null || physicalDisks.Count == 0)
                return null;

            string serial = NormalizeMatchValue(disk["SerialNumber"]);
            string index = NormalizeMatchValue(disk["Index"]);

            ManagementObject match = FindUniqueMatch(physicalDisks, physicalDisk =>
                !string.IsNullOrEmpty(serial) &&
                string.Equals(serial, NormalizeMatchValue(physicalDisk["SerialNumber"]), StringComparison.OrdinalIgnoreCase));

            if (match != null)
                return match;

            return FindUniqueMatch(physicalDisks, physicalDisk =>
                !string.IsNullOrEmpty(index) &&
                string.Equals(index, NormalizeMatchValue(physicalDisk["DeviceId"]), StringComparison.OrdinalIgnoreCase));
        }

        private static ManagementObject FindUniqueMatch(IEnumerable<ManagementObject> objects, Func<ManagementObject, bool> predicate)
        {
            ManagementObject match = null;

            foreach (ManagementObject obj in objects)
            {
                if (!predicate(obj))
                    continue;

                if (match != null)
                    return null;

                match = obj;
            }

            return match;
        }

        private static string NormalizeMatchValue(object value)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
                return string.Empty;

            return HardwareReportFormatHelper.NormalizeStorageText(value.ToString());
        }

        internal static void AppendUnmatchedSmartLifeInfosSection(StringBuilder sb, List<SmartLifeInfo> unmatchedSmartLifeInfos)
        {
            if (unmatchedSmartLifeInfos == null || unmatchedSmartLifeInfos.Count == 0)
                return;

            sb.Append(HardwareReportFormatHelper.Group("Unmatched SMART Life / Health"));

            foreach (SmartLifeInfo info in unmatchedSmartLifeInfos)
            {
                sb.AppendLine(HardwareReportFormatHelper.Label("Instance", HardwareReportFormatHelper.Safe(info.InstanceName)));
                sb.AppendLine(HardwareReportFormatHelper.Label("Life Remaining", HardwareReportFormatHelper.FormatSmartLifeInfo(info.RemainingPercent, info.UsedPercent)));
                sb.AppendLine(HardwareReportFormatHelper.Label("Life Source", info.Source));
                sb.AppendLine();
            }
        }

        internal static IHardware FindStorageHardwareForDisk(ManagementObject disk, List<IHardware> storageHardwares)
        {
            if (disk == null || storageHardwares == null || storageHardwares.Count == 0)
                return null;

            string diskPnpId = NormalizeMatchValue(disk["PNPDeviceID"]);
            string diskModel = NormalizeMatchValue(disk["Model"]);
            string diskSerial = NormalizeMatchValue(disk["SerialNumber"]);

            foreach (var drive in storageHardwares)
            {
                if (drive == null)
                    continue;

                string driveIdentifier = NormalizeMatchValue(drive.Identifier);

                if (!string.IsNullOrWhiteSpace(diskPnpId) &&
                    !string.IsNullOrWhiteSpace(driveIdentifier) &&
                    (driveIdentifier.Contains(diskPnpId) || diskPnpId.Contains(driveIdentifier)))
                {
                    return drive;
                }
            }

            foreach (var drive in storageHardwares)
            {
                if (drive == null)
                    continue;

                string driveIdentifier = NormalizeMatchValue(drive.Identifier);
                string driveName = NormalizeMatchValue(drive.Name);

                if (!string.IsNullOrWhiteSpace(diskSerial) &&
                    (driveIdentifier.Contains(diskSerial) || driveName.Contains(diskSerial)))
                {
                    return drive;
                }
            }

            IHardware modelMatch = null;

            foreach (var drive in storageHardwares)
            {
                if (drive == null)
                    continue;

                string driveIdentifier = NormalizeMatchValue(drive.Identifier);
                string driveName = NormalizeMatchValue(drive.Name);

                if (string.IsNullOrWhiteSpace(diskModel) ||
                    (!driveName.Contains(diskModel) && !driveIdentifier.Contains(diskModel)))
                    continue;

                if (modelMatch != null)
                    return null;

                modelMatch = drive;
            }

            return modelMatch;
        }

        internal static void AppendStorageHealthSummary(StringBuilder sb, IHardware drive, SmartLifeInfo smartLifeInfo)
        {
            if (drive == null)
                return;

            var sensors = HardwareReportFormatHelper.DistinctSensors(drive.Sensors)
                .Where(s => s.Value.HasValue)
                .ToList();

            string remainingLife = HardwareReportFormatHelper.GetRemainingLifeText(sensors);
            bool usedSmartLifeFallback = false;

            if (string.IsNullOrWhiteSpace(remainingLife) && smartLifeInfo != null)
            {
                remainingLife = HardwareReportFormatHelper.FormatSmartLifeInfo(smartLifeInfo.RemainingPercent, smartLifeInfo.UsedPercent);
                usedSmartLifeFallback = true;
            }

            if (!string.IsNullOrWhiteSpace(remainingLife))
                sb.AppendLine(HardwareReportFormatHelper.Label("Life Remaining", remainingLife));

            if (usedSmartLifeFallback)
                sb.AppendLine(HardwareReportFormatHelper.Label("Life Source", smartLifeInfo.Source));

            var healthSensors = sensors
                .Where(HardwareReportFormatHelper.IsStorageHealthSensor)
                .OrderBy(HardwareReportFormatHelper.GetStorageHealthSortOrder)
                .ThenBy(s => s.Name, HardwareReportFormatHelper.NaturalTextComparer)
                .ToList();

            if (!string.IsNullOrWhiteSpace(remainingLife))
            {
                healthSensors = healthSensors
                    .Where(s => !HardwareReportFormatHelper.IsCanonicalStorageLifeSensor(s))
                    .ToList();
            }

            if (healthSensors.Count == 0)
            {
                sb.AppendLine(HardwareReportFormatHelper.Label("Health Sensors", "N/A"));
                return;
            }

            sb.AppendLine(HardwareReportFormatHelper.Label("Health Sensors", $"{healthSensors.Count} available"));

            foreach (var sensor in healthSensors)
                sb.AppendLine(HardwareReportFormatHelper.Label($"  {(sensor.Name == "Available Spare Threshold" ? "Spare Threshold" : sensor.Name)}", HardwareReportFormatHelper.FormatSensorValue(sensor)));
        }

        internal static void AppendSmartLifeInfo(StringBuilder sb, SmartLifeInfo smartLifeInfo)
        {
            if (smartLifeInfo == null)
                return;

            sb.AppendLine(HardwareReportFormatHelper.Label("Life Remaining", HardwareReportFormatHelper.FormatSmartLifeInfo(smartLifeInfo.RemainingPercent, smartLifeInfo.UsedPercent)));
            sb.AppendLine(HardwareReportFormatHelper.Label("Life Source", smartLifeInfo.Source));
        }

        internal static HardwareDiscoveryResult GetStorageInfo(
            IEnumerable<IHardware> storageHardwares,
            Action<IHardware> updateHardwareRecursive,
            Func<string, List<ManagementObject>> wmiQuery)
        {
            var sb = new StringBuilder();
            var displayNames = new List<string>();
            sb.Append(HardwareReportFormatHelper.Section("STORAGE"));

            var disks = wmiQuery("SELECT * FROM Win32_DiskDrive");
            var physicalDisks = WmiQueryHelper.WmiQuery(
                @"root\Microsoft\Windows\Storage",
                "SELECT DeviceId, SerialNumber, BusType FROM MSFT_PhysicalDisk");
            var smartLifeInfos = StorageSmartInfoHelper.GetSmartLifeInfos(WmiQueryHelper.WmiQuery);
            var unmatchedSmartLifeInfos = new List<SmartLifeInfo>(smartLifeInfos);
            var unmatchedStorageHardwares = storageHardwares == null
                ? new List<IHardware>()
                : storageHardwares.Where(d => d != null).ToList();

            try
            {
                if (disks.Count == 0)
                {
                    sb.AppendLine();
                    sb.AppendLine("  No storage information found.");

                    AppendUnmatchedStorageHardwaresSection(sb, unmatchedStorageHardwares, disks, smartLifeInfos, unmatchedSmartLifeInfos, updateHardwareRecursive);
                    AppendUnmatchedSmartLifeInfosSection(sb, unmatchedSmartLifeInfos);

                    return new HardwareDiscoveryResult("Unknown Storage", sb.ToString(), displayNames, 0);
                }

                foreach (var disk in disks)
                {
                    string diskDisplayName = HardwareReportFormatHelper.GetDiskDisplayTitle(disk["Model"], disk["SerialNumber"]);
                    if (!string.IsNullOrWhiteSpace(diskDisplayName))
                        displayNames.Add(diskDisplayName);

                    sb.Append(HardwareReportFormatHelper.Group(diskDisplayName));
                    AppendStorageStaticDiskFields(sb, disk, physicalDisks);

                    IHardware matchedDrive = FindStorageHardwareForDisk(disk, unmatchedStorageHardwares);
                    SmartLifeInfo smartLifeInfo = StorageSmartInfoHelper.FindSmartLifeInfoForDisk(disk, smartLifeInfos);

                    if (matchedDrive != null || smartLifeInfo != null)
                        AppendStorageHealthSectionForDisk(sb, matchedDrive, smartLifeInfo, unmatchedStorageHardwares, unmatchedSmartLifeInfos, updateHardwareRecursive);
                }

                AppendUnmatchedStorageHardwaresSection(sb, unmatchedStorageHardwares, disks, smartLifeInfos, unmatchedSmartLifeInfos, updateHardwareRecursive);
                AppendUnmatchedSmartLifeInfosSection(sb, unmatchedSmartLifeInfos);

                string summary = displayNames.Count == 1
                    ? displayNames[0]
                    : displayNames.Count > 1 ? "Storage" : "Unknown Storage";
                return new HardwareDiscoveryResult(summary, sb.ToString(), displayNames, disks.Count);
            }
            finally
            {
                WmiQueryHelper.DisposeAll(disks);
                WmiQueryHelper.DisposeAll(physicalDisks);
            }
        }

        internal static void AppendStorageHealthSectionForDisk(
            StringBuilder sb,
            IHardware matchedDrive,
            SmartLifeInfo smartLifeInfo,
            List<IHardware> unmatchedStorageHardwares,
            List<SmartLifeInfo> unmatchedSmartLifeInfos,
            Action<IHardware> updateHardwareRecursive)
        {
            sb.AppendLine();
            sb.AppendLine("  Health / SMART");

            if (matchedDrive != null)
            {
                string driveIdentifier = HardwareReportFormatHelper.Safe(matchedDrive.Identifier);
                sb.AppendLine(HardwareReportFormatHelper.Label("Identifier", driveIdentifier));

                try
                {
                    updateHardwareRecursive(matchedDrive);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("UpdateHardwareRecursive(drive) failed: " + ex);
                }

                AppendStorageHealthSummary(sb, matchedDrive, smartLifeInfo);
                unmatchedStorageHardwares.Remove(matchedDrive);
            }
            else
            {
                AppendSmartLifeInfo(sb, smartLifeInfo);
            }

            if (smartLifeInfo != null)
                unmatchedSmartLifeInfos.Remove(smartLifeInfo);
        }

        internal static void AppendUnmatchedStorageHardwaresSection(
            StringBuilder sb,
            List<IHardware> unmatchedStorageHardwares,
            List<ManagementObject> disks,
            List<SmartLifeInfo> smartLifeInfos,
            List<SmartLifeInfo> unmatchedSmartLifeInfos,
            Action<IHardware> updateHardwareRecursive)
        {
            if (unmatchedStorageHardwares == null || unmatchedStorageHardwares.Count == 0)
                return;

            sb.Append(HardwareReportFormatHelper.Group("Unmatched LibreHardwareMonitor Drives / Health"));

            foreach (var drive in unmatchedStorageHardwares)
            {
                string driveIdentifier = HardwareReportFormatHelper.Safe(drive.Identifier);

                string driveName = HardwareReportFormatHelper.Safe(drive.Name);
                sb.AppendLine(HardwareReportFormatHelper.Label("Drive", driveName == "N/A" ? driveIdentifier : driveName));
                sb.AppendLine(HardwareReportFormatHelper.Label("Identifier", driveIdentifier));

                try
                {
                    updateHardwareRecursive(drive);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("UpdateHardwareRecursive(drive) failed: " + ex);
                }

                SmartLifeInfo driveSmartLifeInfo = StorageSmartInfoHelper.FindSmartLifeInfoForDrive(drive, disks, smartLifeInfos);
                AppendStorageHealthSummary(sb, drive, driveSmartLifeInfo);

                if (driveSmartLifeInfo != null)
                    unmatchedSmartLifeInfos.Remove(driveSmartLifeInfo);

                sb.AppendLine();
            }
        }
    }
}
