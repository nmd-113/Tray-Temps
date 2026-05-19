using System;
using LibreHardwareMonitor.Hardware;

namespace TrayTemps
{
    internal static class HardwareDialogTextHelper
    {
        internal static string GetComponentDisplayName(IHardware hardware, string labelText, string fallback)
        {
            if (hardware != null && !string.IsNullOrWhiteSpace(hardware.Name))
                return hardware.Name.Trim();

            return GetCleanDialogTitle(labelText, fallback);
        }

        internal static string GetFinalComponentName(string componentName, string categoryName, IHardware hardware)
        {
            if (hardware != null && !string.IsNullOrWhiteSpace(hardware.Name))
                return hardware.Name.Trim();

            string clean = GetCleanDialogTitle(componentName, categoryName);

            if (string.IsNullOrWhiteSpace(clean))
                return categoryName;

            return clean;
        }

        internal static string GetCleanDialogTitle(string text, string fallback)
        {
            if (string.IsNullOrWhiteSpace(text))
                return fallback;

            text = text
                .Replace("\r", "\n")
                .Split('\n')[0]
                .Trim();

            if (text.Equals("N/A", StringComparison.OrdinalIgnoreCase))
                return fallback;

            if (text.Equals("Loading...", StringComparison.OrdinalIgnoreCase))
                return fallback;

            return text;
        }
    }
}
