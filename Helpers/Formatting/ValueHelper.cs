using System;
using System.Drawing;

namespace TrayTemps
{
    internal static class ValueHelper
    {
        internal static decimal ClampDecimal(decimal value, decimal min, decimal max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }

        internal static int ClampInt(int value, int min, int max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }

        internal static Color LoadColorOrDefault(int argb, Color fallback)
        {
            return argb == 0 ? fallback : Color.FromArgb(argb);
        }
    }
}
