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
        internal static void AppendStorageStaticDiskFields(StringBuilder sb, ManagementObject disk)
        {
            sb.AppendLine(HardwareReportFormatHelper.Label("Model", disk["Model"]));
            sb.AppendLine(HardwareReportFormatHelper.Label("Interface", disk["InterfaceType"]));
            sb.AppendLine(HardwareReportFormatHelper.Label("Media Type", disk["MediaType"]));
            sb.AppendLine(HardwareReportFormatHelper.Label("Size", HardwareReportFormatHelper.SizeHuman(disk["Size"])));
            sb.AppendLine(HardwareReportFormatHelper.Label("Serial", disk["SerialNumber"]));
            sb.AppendLine(HardwareReportFormatHelper.Label("Firmware", disk["FirmwareRevision"]));
            sb.AppendLine(HardwareReportFormatHelper.Label("Partitions", disk["Partitions"]));
            sb.AppendLine(HardwareReportFormatHelper.Label("PNP ID", disk["PNPDeviceID"]));
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

            string diskPnpId = HardwareReportFormatHelper.NormalizeStorageText(HardwareReportFormatHelper.Safe(disk["PNPDeviceID"]));
            string diskModel = HardwareReportFormatHelper.NormalizeStorageText(HardwareReportFormatHelper.Safe(disk["Model"]));
            string diskSerial = HardwareReportFormatHelper.NormalizeStorageText(HardwareReportFormatHelper.Safe(disk["SerialNumber"]));

            foreach (var drive in storageHardwares)
            {
                if (drive == null)
                    continue;

                string driveIdentifier = HardwareReportFormatHelper.NormalizeStorageText(HardwareReportFormatHelper.Safe(drive.Identifier));
                string driveName = HardwareReportFormatHelper.NormalizeStorageText(HardwareReportFormatHelper.Safe(drive.Name));

                if (!string.IsNullOrWhiteSpace(diskPnpId) &&
                    ((!string.IsNullOrWhiteSpace(driveIdentifier) && driveIdentifier.Contains(diskPnpId)) ||
                     (!string.IsNullOrWhiteSpace(diskPnpId) && diskPnpId.Contains(driveIdentifier))))
                {
                    return drive;
                }

                if (!string.IsNullOrWhiteSpace(diskModel) &&
                    (driveName.Contains(diskModel) || driveIdentifier.Contains(diskModel)))
                {
                    return drive;
                }

                if (!string.IsNullOrWhiteSpace(diskSerial) &&
                    (driveIdentifier.Contains(diskSerial) || driveName.Contains(diskSerial)))
                {
                    return drive;
                }
            }

            return null;
        }

        internal static void AppendStorageHealthSummary(StringBuilder sb, IHardware drive, SmartLifeInfo smartLifeInfo)
        {
            if (drive == null)
                return;

            var sensors = drive.Sensors
                .Where(s => s.Value.HasValue)
                .ToList();

            string remainingLife = HardwareReportFormatHelper.GetRemainingLifeText(sensors);

            if (string.IsNullOrWhiteSpace(remainingLife) && smartLifeInfo != null)
                remainingLife = HardwareReportFormatHelper.FormatSmartLifeInfo(smartLifeInfo.RemainingPercent, smartLifeInfo.UsedPercent);

            if (!string.IsNullOrWhiteSpace(remainingLife))
                sb.AppendLine(HardwareReportFormatHelper.Label("Life Remaining", remainingLife));

            if (smartLifeInfo != null)
                sb.AppendLine(HardwareReportFormatHelper.Label("Life Source", smartLifeInfo.Source));

            var healthSensors = sensors
                .Where(HardwareReportFormatHelper.IsStorageHealthSensor)
                .OrderBy(HardwareReportFormatHelper.GetStorageHealthSortOrder)
                .ThenBy(s => s.Name)
                .ToList();

            bool hidePercentageUsedSensor = smartLifeInfo != null && smartLifeInfo.UsedPercent.HasValue;

            if (hidePercentageUsedSensor)
            {
                healthSensors = healthSensors
                    .Where(s => HardwareReportFormatHelper.Safe(s.Name).IndexOf("percentage used", StringComparison.OrdinalIgnoreCase) < 0)
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

        internal static string GetStorageDetails(
            IEnumerable<IHardware> storageHardwares,
            Action<IHardware> updateHardwareRecursive,
            Func<string, List<ManagementObject>> wmiQuery)
        {
            var sb = new StringBuilder();
            sb.Append(HardwareReportFormatHelper.Section("STORAGE"));

            var disks = wmiQuery("SELECT * FROM Win32_DiskDrive");
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

                    return sb.ToString();
                }

                foreach (var disk in disks)
                {
                    string diskDisplayName = HardwareReportFormatHelper.GetDiskDisplayTitle(disk["Model"], disk["SerialNumber"]);

                    sb.Append(HardwareReportFormatHelper.Group(diskDisplayName));
                    AppendStorageStaticDiskFields(sb, disk);

                    IHardware matchedDrive = FindStorageHardwareForDisk(disk, unmatchedStorageHardwares);
                    SmartLifeInfo smartLifeInfo = StorageSmartInfoHelper.FindSmartLifeInfoForDisk(disk, smartLifeInfos);

                    if (matchedDrive != null || smartLifeInfo != null)
                        AppendStorageHealthSectionForDisk(sb, matchedDrive, smartLifeInfo, unmatchedStorageHardwares, unmatchedSmartLifeInfos, updateHardwareRecursive);
                }

                AppendUnmatchedStorageHardwaresSection(sb, unmatchedStorageHardwares, disks, smartLifeInfos, unmatchedSmartLifeInfos, updateHardwareRecursive);
                AppendUnmatchedSmartLifeInfosSection(sb, unmatchedSmartLifeInfos);

                return sb.ToString();
            }
            finally
            {
                WmiQueryHelper.DisposeAll(disks);
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

                sb.AppendLine(HardwareReportFormatHelper.Label("Drive", string.IsNullOrWhiteSpace(HardwareReportFormatHelper.Safe(drive.Name)) ? driveIdentifier : drive.Name));
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
