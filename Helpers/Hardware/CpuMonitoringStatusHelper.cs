using System.Collections.Generic;
using System.Linq;

namespace TrayTemps
{
    internal enum CpuMonitoringState
    {
        NoCpuDetected,
        WmiCpuWithoutLhmHardware,
        LhmCpuWithoutUsableTemperature,
        MonitoringAvailable
    }

    internal sealed class CpuMonitoringStatus
    {
        internal CpuMonitoringStatus(
            CpuMonitoringState state,
            string wmiDisplayName,
            bool nativeHardwareDetailsAvailable,
            bool sensorConfigurationAvailable,
            bool temperatureAvailable,
            bool elevationMayImproveSensorAccess)
        {
            State = state;
            WmiDisplayName = wmiDisplayName;
            NativeHardwareDetailsAvailable = nativeHardwareDetailsAvailable;
            SensorConfigurationAvailable = sensorConfigurationAvailable;
            TemperatureAvailable = temperatureAvailable;
            ElevationMayImproveSensorAccess = elevationMayImproveSensorAccess;
        }

        internal CpuMonitoringState State { get; }
        internal string WmiDisplayName { get; }
        internal bool NativeHardwareDetailsAvailable { get; }
        internal bool SensorConfigurationAvailable { get; }
        internal bool TemperatureAvailable { get; }
        internal bool ElevationMayImproveSensorAccess { get; }
    }

    internal static class CpuMonitoringStatusHelper
    {
        internal static CpuMonitoringStatus GetStatus(
            IEnumerable<string> wmiDisplayNames,
            int wmiCpuCount,
            int lhmHardwareCount,
            bool hasUsableTemperature)
        {
            List<string> wmiNames = (wmiDisplayNames ?? Enumerable.Empty<string>())
                .Where(name => !string.IsNullOrWhiteSpace(name))
                .ToList();
            bool hasWmiCpu = wmiCpuCount > 0;
            bool hasLhmCpu = lhmHardwareCount > 0;
            CpuMonitoringState state;

            if (!hasWmiCpu && !hasLhmCpu)
                state = CpuMonitoringState.NoCpuDetected;
            else if (!hasLhmCpu)
                state = CpuMonitoringState.WmiCpuWithoutLhmHardware;
            else if (!hasUsableTemperature)
                state = CpuMonitoringState.LhmCpuWithoutUsableTemperature;
            else
                state = CpuMonitoringState.MonitoringAvailable;

            string wmiDisplayName = wmiNames.Count == 1
                ? wmiNames[0]
                : wmiNames.Count > 1 ? "Processors" : hasWmiCpu ? "Unknown CPU" : null;

            return new CpuMonitoringStatus(
                state,
                wmiDisplayName,
                nativeHardwareDetailsAvailable: hasWmiCpu,
                sensorConfigurationAvailable: hasLhmCpu,
                temperatureAvailable: state == CpuMonitoringState.MonitoringAvailable,
                elevationMayImproveSensorAccess: state == CpuMonitoringState.LhmCpuWithoutUsableTemperature);
        }
    }
}
