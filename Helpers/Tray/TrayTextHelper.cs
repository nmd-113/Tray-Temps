using System;
using System.Globalization;
using System.Linq;

namespace TrayTemps
{
    internal static class TrayTextHelper
    {
        internal static string GetTrayReferenceText(params string[] values)
        {
            bool needsThreeDigits = values.Any(ValueNeedsThreeDigitTrayReference);
            return needsThreeDigits ? "000" : "00";
        }

        internal static bool ValueNeedsThreeDigitTrayReference(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return false;

            if (!int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int parsed))
                return false;

            return Math.Abs(parsed) >= 100;
        }
    }
}
