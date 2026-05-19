namespace TrayTemps
{
    internal static class TrayIconCacheKeyHelper
    {
        internal static string GetCombinedTrayCacheKey(string cpuText, string gpuText, int cpuColorArgb, int gpuColorArgb)
        {
            return $"combined_{cpuText}_{gpuText}_{cpuColorArgb}_{gpuColorArgb}";
        }

        internal static string GetSingleTrayCacheKey(string text, string referenceText, int colorArgb)
        {
            return $"single_{text}_{referenceText}_{colorArgb}";
        }
    }
}
