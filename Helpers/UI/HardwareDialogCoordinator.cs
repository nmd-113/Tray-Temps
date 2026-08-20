using LibreHardwareMonitor.Hardware;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TrayTemps
{
    internal static class HardwareDialogCoordinator
    {
        internal static async Task<HardwareDetailsDialog> ShowHardwareDialogFromClickAsync(
            Form owner,
            string clickHandlerName,
            string componentName,
            string categoryName,
            string initialContent,
            Func<Task<string>> contentFactory,
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
                return await ShowHardwareDialogAsync(
                    owner,
                    componentName,
                    categoryName,
                    initialContent,
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
                return null;
            }
        }

        internal static Task<HardwareDetailsDialog> ShowHardwareDialogAsync(
            Form owner,
            string componentName,
            string categoryName,
            string initialContent,
            Func<Task<string>> contentFactory,
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
                string finalComponentName = HardwareDialogTextHelper.GetFinalComponentName(componentName, categoryName, liveHardware);
                Func<Task<string>> liveFactory = BuildLiveTextFactory(
                    liveTextFactory,
                    liveHardware,
                    hardwareUpdateLock,
                    updateHardwareRecursive,
                    isShutdownInitiated);

                var dlg = new HardwareDetailsDialog(
                    finalComponentName,
                    categoryName,
                    initialContent,
                    liveFactory,
                    isShutdownInitiated,
                    isLightModeEnabled);

                ShowHardwareDialogInstance(owner, dlg, registerHardwareDialog, unregisterHardwareDialog);
                _ = UpdateDialogDetailsAsync(dlg, contentFactory);
                return Task.FromResult(dlg);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    owner,
                    $"Could not load hardware details.\n\n{ex.Message}",
                    "Hardware Details Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return Task.FromResult<HardwareDetailsDialog>(null);
            }
        }

        private static async Task UpdateDialogDetailsAsync(
            HardwareDetailsDialog dialog,
            Func<Task<string>> contentFactory)
        {
            try
            {
                string content = await contentFactory().ConfigureAwait(true);

                if (!dialog.IsDisposed && dialog.IsHandleCreated)
                    dialog.SetDetailsText(content);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Loading hardware dialog details failed: " + ex);

                if (!dialog.IsDisposed && dialog.IsHandleCreated)
                    dialog.SetDetailsText("Detailed hardware information is unavailable.");
            }
        }

        internal static Func<Task<string>> BuildLiveTextFactory(
            Func<Task<string>> liveTextFactory,
            IHardware liveHardware,
            object hardwareUpdateLock,
            Action<IHardware> updateHardwareRecursive,
            Func<bool> isShutdownInitiated)
        {
            Func<Task<string>> liveFactory = liveTextFactory;

            if (liveFactory == null && liveHardware != null)
            {
                liveFactory = () => Task.Run(() =>
                {
                    lock (hardwareUpdateLock)
                    {
                        if (isShutdownInitiated != null && isShutdownInitiated())
                            return string.Empty;

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

    }
}
