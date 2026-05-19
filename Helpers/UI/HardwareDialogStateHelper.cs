using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace TrayTemps
{
    internal static class HardwareDialogStateHelper
    {
        internal static void RegisterHardwareDialog(
            HardwareDetailsDialog dialog,
            List<HardwareDetailsDialog> openHardwareDialogs,
            Rectangle? savedHardwareDialogBounds,
            Size defaultMinimumSize,
            bool isLightModeEnabled,
            EventHandler boundsChangedHandler,
            FormClosedEventHandler formClosedHandler,
            Func<Rectangle, bool> isWindowBoundsVisible)
        {
            ApplySavedHardwareDialogBounds(dialog, savedHardwareDialogBounds, defaultMinimumSize, isWindowBoundsVisible);
            dialog.SetLightTheme(isLightModeEnabled);

            dialog.LocationChanged += boundsChangedHandler;
            dialog.SizeChanged += boundsChangedHandler;
            dialog.FormClosed += formClosedHandler;
            openHardwareDialogs.Add(dialog);
        }

        internal static void UnregisterHardwareDialog(
            HardwareDetailsDialog dialog,
            List<HardwareDetailsDialog> openHardwareDialogs,
            EventHandler boundsChangedHandler,
            FormClosedEventHandler formClosedHandler)
        {
            if (dialog == null)
                return;

            dialog.LocationChanged -= boundsChangedHandler;
            dialog.SizeChanged -= boundsChangedHandler;
            dialog.FormClosed -= formClosedHandler;
            openHardwareDialogs.Remove(dialog);
        }

        internal static void ApplySavedHardwareDialogBounds(
            HardwareDetailsDialog dialog,
            Rectangle? savedHardwareDialogBounds,
            Size defaultMinimumSize,
            Func<Rectangle, bool> isWindowBoundsVisible)
        {
            if (!savedHardwareDialogBounds.HasValue)
                return;

            Rectangle bounds = savedHardwareDialogBounds.Value;
            int minWidth = dialog.MinimumSize.Width > 0 ? dialog.MinimumSize.Width : defaultMinimumSize.Width;
            int minHeight = dialog.MinimumSize.Height > 0 ? dialog.MinimumSize.Height : defaultMinimumSize.Height;
            bounds.Width = Math.Max(minWidth, bounds.Width);
            bounds.Height = Math.Max(minHeight, bounds.Height);

            if (!isWindowBoundsVisible(bounds))
                return;

            dialog.StartPosition = FormStartPosition.Manual;
            dialog.Bounds = bounds;
        }

        internal static void RememberHardwareDialogBounds(Form dialog, Action<Rectangle> rememberBounds)
        {
            if (dialog == null || dialog.IsDisposed || dialog.WindowState != FormWindowState.Normal)
                return;

            Rectangle bounds = dialog.Bounds;
            if (bounds.Width <= 0 || bounds.Height <= 0)
                return;

            rememberBounds(bounds);
        }

        internal static void CaptureOpenHardwareDialogBounds(
            List<HardwareDetailsDialog> openHardwareDialogs,
            Action<Rectangle> rememberBounds)
        {
            foreach (HardwareDetailsDialog dialog in openHardwareDialogs.ToArray())
            {
                if (dialog == null || dialog.IsDisposed)
                    openHardwareDialogs.Remove(dialog);
            }

            HardwareDetailsDialog openDialog = openHardwareDialogs
                .LastOrDefault(dialog => dialog.WindowState == FormWindowState.Normal);

            if (openDialog != null)
                RememberHardwareDialogBounds(openDialog, rememberBounds);
        }

        internal static void ApplyThemeToOpenHardwareDialogs(
            List<HardwareDetailsDialog> openHardwareDialogs,
            bool isLightModeEnabled)
        {
            foreach (HardwareDetailsDialog dialog in openHardwareDialogs.ToArray())
            {
                if (dialog == null || dialog.IsDisposed)
                {
                    openHardwareDialogs.Remove(dialog);
                    continue;
                }

                dialog.SetLightTheme(isLightModeEnabled);
            }
        }

        internal static void CloseOpenHardwareDialogs(
            List<HardwareDetailsDialog> openHardwareDialogs,
            Action<Rectangle> rememberBounds)
        {
            foreach (HardwareDetailsDialog dialog in openHardwareDialogs.ToArray())
            {
                try
                {
                    RememberHardwareDialogBounds(dialog, rememberBounds);

                    if (!dialog.IsDisposed)
                        dialog.Close();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("CloseOpenHardwareDialogs: failed to close dialog: " + ex);
                }
            }

            openHardwareDialogs.Clear();
        }
    }
}
