using System;
using System.Runtime.InteropServices;

namespace TrayTemps
{
    internal static class WindowCornerHelper
    {
        private const int DwmWindowCornerPreference = 33;
        private const int DwmWindowCornerRound = 2;

        [DllImport("dwmapi.dll")]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attribute, ref int attributeValue, int attributeSize);

        internal static void ApplyRoundedCorners(IntPtr handle)
        {
            if (handle == IntPtr.Zero)
                return;

            try
            {
                int preference = DwmWindowCornerRound;
                DwmSetWindowAttribute(handle, DwmWindowCornerPreference, ref preference, sizeof(int));
            }
            catch (DllNotFoundException)
            {
                // DWM is unavailable only on unsupported Windows versions.
            }
            catch (EntryPointNotFoundException)
            {
                // Older DWM versions do not implement this attribute.
            }
        }
    }
}
