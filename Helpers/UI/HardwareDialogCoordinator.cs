using LibreHardwareMonitor.Hardware;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TrayTemps
{
    internal static class HardwareDialogCoordinator
    {
        internal static async Task ShowHardwareDialogFromClickAsync(
            Form owner,
            string clickHandlerName,
            string componentName,
            string categoryName,
            Func<string> contentFactory,
            object hardwareUpdateLock,
            Action<IHardware> updateHardwareRecursive,
            Action<HardwareDetailsDialog> registerHardwareDialog,
            Action<HardwareDetailsDialog> unregisterHardwareDialog,
            Func<bool> isShutdownInitiated,
            bool isLightModeEnabled,
            IHardware liveHardware = null,
            Func<Task<string>> liveTextFactory = null)
        {
            try
            {
                await ShowHardwareDialogAsync(
                    owner,
                    componentName,
                    categoryName,
                    contentFactory,
                    hardwareUpdateLock,
                    updateHardwareRecursive,
                    registerHardwareDialog,
                    unregisterHardwareDialog,
                    isShutdownInitiated,
                    isLightModeEnabled,
                    liveHardware,
                    liveTextFactory);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(clickHandlerName + " failed: " + ex);
            }
        }

        internal static async Task ShowHardwareDialogAsync(
            Form owner,
            string componentName,
            string categoryName,
            Func<string> contentFactory,
            object hardwareUpdateLock,
            Action<IHardware> updateHardwareRecursive,
            Action<HardwareDetailsDialog> registerHardwareDialog,
            Action<HardwareDetailsDialog> unregisterHardwareDialog,
            Func<bool> isShutdownInitiated,
            bool isLightModeEnabled,
            IHardware liveHardware = null,
            Func<Task<string>> liveTextFactory = null)
        {
            SetLoadingCursor(owner, true);

            try
            {
                string finalComponentName = HardwareDialogTextHelper.GetFinalComponentName(componentName, categoryName, liveHardware);
                string content = await Task.Run(contentFactory);

                if (owner.IsDisposed || !owner.IsHandleCreated)
                    return;

                Func<Task<string>> liveFactory = BuildLiveTextFactory(liveTextFactory, liveHardware, hardwareUpdateLock, updateHardwareRecursive);

                var dlg = new HardwareDetailsDialog(
                    finalComponentName,
                    categoryName,
                    content,
                    liveFactory,
                    isShutdownInitiated,
                    isLightModeEnabled);

                ShowHardwareDialogInstance(owner, dlg, registerHardwareDialog, unregisterHardwareDialog);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    owner,
                    $"Could not load hardware details.\n\n{ex.Message}",
                    "Hardware Details Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                SetLoadingCursor(owner, false);
            }
        }

        internal static Func<Task<string>> BuildLiveTextFactory(
            Func<Task<string>> liveTextFactory,
            IHardware liveHardware,
            object hardwareUpdateLock,
            Action<IHardware> updateHardwareRecursive)
        {
            Func<Task<string>> liveFactory = liveTextFactory;

            if (liveFactory == null && liveHardware != null)
            {
                liveFactory = () => Task.Run(() =>
                {
                    lock (hardwareUpdateLock)
                    {
                        updateHardwareRecursive(liveHardware);
                        return HardwareLiveSensorsTextHelper.BuildLiveSensorsText(liveHardware);
                    }
                });
            }

            return liveFactory;
        }

        internal static void ShowHardwareDialogInstance(
            Form owner,
            HardwareDetailsDialog dlg,
            Action<HardwareDetailsDialog> registerHardwareDialog,
            Action<HardwareDetailsDialog> unregisterHardwareDialog)
        {
            try
            {
                registerHardwareDialog(dlg);
                dlg.Show(owner);
                dlg.Activate();
            }
            catch
            {
                unregisterHardwareDialog(dlg);
                dlg.Dispose();
                throw;
            }
        }

        private static void SetLoadingCursor(Control owner, bool loading)
        {
            owner.UseWaitCursor = loading;
            Cursor.Current = loading ? Cursors.WaitCursor : Cursors.Default;

            SetLoadingCursorRecursive(owner, loading);
        }

        private static void SetLoadingCursorRecursive(Control parent, bool loading)
        {
            foreach (Control control in parent.Controls)
            {
                control.UseWaitCursor = loading;

                if (control.HasChildren)
                    SetLoadingCursorRecursive(control, loading);
            }
        }
    }
}
