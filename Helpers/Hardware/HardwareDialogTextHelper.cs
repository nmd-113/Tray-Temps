using System;
using LibreHardwareMonitor.Hardware;

namespace TrayTemps
{
    internal static class HardwareDialogTextHelper
    {
        internal static string GetComponentDisplayName(IHardware hardware, string labelText, string fallback)
        {
            if (hardware != null && !string.IsNullOrWhiteSpace(hardware.Name))
                return HardwareReportFormatHelper.Safe(hardware.Name);

            return GetCleanDialogTitle(labelText, fallback);
        }

        internal static string GetFinalComponentName(string componentName, string categoryName, IHardware hardware)
        {
            string clean = GetCleanDialogTitle(componentName, null);

            if (!string.IsNullOrWhiteSpace(clean))
                return clean;

            if (hardware != null && !string.IsNullOrWhiteSpace(hardware.Name))
                return HardwareReportFormatHelper.Safe(hardware.Name);

            return categoryName;
        }

        internal static string GetCategoryDialogTitle(string specificName, string categoryTitle, int componentCount)
        {
            if (componentCount > 1)
                return categoryTitle;

            return GetCleanDialogTitle(specificName, categoryTitle);
        }

        internal static string GetCleanDialogTitle(string text, string fallback)
        {
            if (string.IsNullOrWhiteSpace(text))
                return fallback;

            text = HardwareReportFormatHelper.SanitizeSingleLineText(text
                .Replace("\r", "\n")
                .Split('\n')[0]);

            if (text.Equals("N/A", StringComparison.OrdinalIgnoreCase))
                return fallback;

            if (text.Equals("Loading...", StringComparison.OrdinalIgnoreCase))
                return fallback;

            if (text.StartsWith("Loading hardware information", StringComparison.OrdinalIgnoreCase))
                return fallback;

            return text;
        }
    }
}
