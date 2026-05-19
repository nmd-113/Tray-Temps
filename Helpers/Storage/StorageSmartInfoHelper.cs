using LibreHardwareMonitor.Hardware;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Management;

namespace TrayTemps
{
    internal static class StorageSmartInfoHelper
    {
        internal static List<SmartLifeInfo> GetSmartLifeInfos(Func<string, string, List<ManagementObject>> scopedWmiQuery)
        {
            var infos = new List<SmartLifeInfo>();
            var rows = scopedWmiQuery(@"root\wmi", "SELECT InstanceName, VendorSpecific FROM MSStorageDriver_FailurePredictData");

            foreach (var row in rows)
            {
                try
                {
                    string instanceName = HardwareReportFormatHelper.Safe(row["InstanceName"]);
                    byte[] data = row["VendorSpecific"] as byte[];

                    if (data == null || data.Length < 362)
                        continue;

                    SmartLifeInfo info = ParseSmartLifeInfo(instanceName, data);

                    if (info != null)
                        infos.Add(info);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("GetSmartLifeInfos: failed to parse row: " + ex);
                }
            }

            return infos;
        }

        private static SmartLifeInfo ParseSmartLifeInfo(string instanceName, byte[] data)
        {
            var attributes = new Dictionary<byte, SmartAttributeInfo>();

            for (int offset = 2; offset + 12 <= data.Length && offset < 362; offset += 12)
            {
                byte id = data[offset];

                if (id == 0)
                    continue;

                byte current = data[offset + 3];
                byte worst = data[offset + 4];
                ulong raw = ReadSmartRawValue(data, offset + 5);

                attributes[id] = new SmartAttributeInfo(id, current, worst, raw);
            }

            if (TryBuildSmartLifeInfo(instanceName, attributes, 0xE7, "SMART E7 SSD Life Left", false, true, out SmartLifeInfo info))
                return info;

            if (TryBuildSmartLifeInfo(instanceName, attributes, 0xE9, "SMART E9 Media Wearout Indicator", false, false, out info))
                return info;

            if (TryBuildSmartLifeInfo(instanceName, attributes, 0xCA, "SMART CA Percentage Lifetime Used", true, true, out info))
                return info;

            return null;
        }

        private static bool TryBuildSmartLifeInfo(
            string instanceName,
            Dictionary<byte, SmartAttributeInfo> attributes,
            byte id,
            string source,
            bool valueIsUsedPercent,
            bool preferRaw,
            out SmartLifeInfo info)
        {
            info = null;

            if (!attributes.TryGetValue(id, out SmartAttributeInfo attribute))
                return false;

            if (!TryGetSmartAttributePercent(attribute, preferRaw, out float percent))
                return false;

            float remaining = valueIsUsedPercent
                ? Math.Max(0, 100 - percent)
                : percent;

            float? used = valueIsUsedPercent ? percent : (float?)null;

            info = new SmartLifeInfo(instanceName, remaining, used, source);
            return true;
        }

        private static bool TryGetSmartAttributePercent(SmartAttributeInfo attribute, bool preferRaw, out float percent)
        {
            percent = 0;

            if (preferRaw && attribute.Raw > 0 && attribute.Raw <= 100)
            {
                percent = attribute.Raw;
                return true;
            }

            if (attribute.Current > 0 && attribute.Current <= 100)
            {
                percent = attribute.Current;
                return true;
            }

            if (!preferRaw && attribute.Raw > 0 && attribute.Raw <= 100)
            {
                percent = attribute.Raw;
                return true;
            }

            return false;
        }

        private static ulong ReadSmartRawValue(byte[] data, int offset)
        {
            ulong value = 0;

            for (int i = 0; i < 6 && offset + i < data.Length; i++)
                value |= ((ulong)data[offset + i]) << (8 * i);

            return value;
        }

        internal static SmartLifeInfo FindSmartLifeInfoForDrive(
            IHardware drive,
            List<ManagementObject> disks,
            List<SmartLifeInfo> smartLifeInfos)
        {
            if (drive == null || disks == null || smartLifeInfos == null)
                return null;

            string driveName = HardwareReportFormatHelper.NormalizeStorageText(drive.Name);

            foreach (var disk in disks)
            {
                string model = HardwareReportFormatHelper.NormalizeStorageText(HardwareReportFormatHelper.Safe(disk["Model"]));

                if (string.IsNullOrWhiteSpace(model) || string.IsNullOrWhiteSpace(driveName))
                    continue;

                if (model.Contains(driveName) || driveName.Contains(model))
                    return FindSmartLifeInfoForDisk(disk, smartLifeInfos);
            }

            return null;
        }

        internal static SmartLifeInfo FindSmartLifeInfoForDisk(ManagementObject disk, List<SmartLifeInfo> smartLifeInfos)
        {
            if (disk == null || smartLifeInfos == null)
                return null;

            string pnpId = HardwareReportFormatHelper.NormalizeStorageText(HardwareReportFormatHelper.Safe(disk["PNPDeviceID"]));
            string model = HardwareReportFormatHelper.NormalizeStorageText(HardwareReportFormatHelper.Safe(disk["Model"]));

            foreach (var info in smartLifeInfos)
            {
                string instance = HardwareReportFormatHelper.NormalizeStorageText(info.InstanceName);

                if (!string.IsNullOrWhiteSpace(pnpId) &&
                    (instance.Contains(pnpId) || pnpId.Contains(instance)))
                    return info;

                if (!string.IsNullOrWhiteSpace(model) && instance.Contains(model))
                    return info;
            }

            return null;
        }
    }

    internal sealed class SmartLifeInfo
    {
        public SmartLifeInfo(string instanceName, float remainingPercent, float? usedPercent, string source)
        {
            InstanceName = instanceName;
            RemainingPercent = remainingPercent;
            UsedPercent = usedPercent;
            Source = source;
        }

        public string InstanceName { get; }
        public float RemainingPercent { get; }
        public float? UsedPercent { get; }
        public string Source { get; }
    }

    internal struct SmartAttributeInfo
    {
        public SmartAttributeInfo(byte id, byte current, byte worst, ulong raw)
        {
            Id = id;
            Current = current;
            Worst = worst;
            Raw = raw;
        }

        public byte Id { get; }
        public byte Current { get; }
        public byte Worst { get; }
        public ulong Raw { get; }
    }
}
