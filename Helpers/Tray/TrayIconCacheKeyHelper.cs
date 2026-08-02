namespace TrayTemps
{
    internal static class TrayIconCacheKeyHelper
    {
        internal static string GetCombinedTrayCacheKey(
            string cpuText,
            string gpuText,
            int cpuColorArgb,
            int gpuColorArgb,
            bool hasDeviceIdentityMarkers,
            int cpuMarkerColorArgb,
            int gpuMarkerColorArgb)
        {
            return $"combined_{cpuText}_{gpuText}_{cpuColorArgb}_{gpuColorArgb}_{hasDeviceIdentityMarkers}_{cpuMarkerColorArgb}_{gpuMarkerColorArgb}";
        }

        internal static string GetSingleTrayCacheKey(
            string text,
            string referenceText,
            int colorArgb,
            bool hasDeviceIdentityMarker,
            int markerColorArgb)
        {
            return $"single_{text}_{referenceText}_{colorArgb}_{hasDeviceIdentityMarker}_{markerColorArgb}";
        }
    }
}
