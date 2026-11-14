using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TrayTemps
{
    public partial class MainForm
    {
        private const int FpsLimiterHotkeyId = 0x5455, FpsLimiterHotkeyTestId = 0x5456;
        private FpsLimiterConfiguration _fpsLimiterConfiguration = new FpsLimiterConfiguration();
        private FpsLimiterService _fpsLimiterService;
        private FpsLimiterSettingsDialog _fpsLimiterDialog;
        private bool _fpsLimiterRuntimeReady, _fpsLimiterUiSync, _fpsLimiterHotkeyRegistered;
        private bool _fpsLimiterClosePending, _fpsLimiterCloseReady, _fpsLimiterRestartPending;
        private Exception _fpsLimiterReportedError;

        private async void InitializeFpsLimiterRuntime()
        {
            if (_fpsLimiterRuntimeReady || _isShutdownInitiated || EmbeddedFonts.IsDesignTime(this)) return;
            _fpsLimiterRuntimeReady = true;
            ApplyFpsLimiterHotkey();
            SyncFpsLimiterControls();
            if (_fpsLimiterConfiguration.Enabled) await SetFpsLimiterEnabledAsync(true);
        }

        private async void FpsLimiterEnable_CheckedChanged(object sender, EventArgs e)
        {
            if (_fpsLimiterUiSync || _fpsLimiterRestartPending || _fpsLimiterClosePending) return;
            _fpsLimiterConfiguration.Enabled = fpsLimiterEnable.Checked;
            SyncFpsLimiterControls();
            if (!_settingsLoaded || !_fpsLimiterRuntimeReady) return;
            SaveSettings();
            await SetFpsLimiterEnabledAsync(fpsLimiterEnable.Checked);
        }

        private async Task SetFpsLimiterEnabledAsync(bool enabled)
        {
            if (_isShutdownInitiated || !_fpsLimiterRuntimeReady) return;
            if (_fpsLimiterService == null && enabled)
            {
                _fpsLimiterService = new FpsLimiterService(_fpsLimiterConfiguration);
                _fpsLimiterService.StateChanged += FpsLimiter_StateChanged;
            }
            if (_fpsLimiterService != null) await _fpsLimiterService.SetEnabledAsync(enabled);
            if (!_isShutdownInitiated && !IsDisposed) SyncFpsLimiterControls();
        }

        private void FpsLimiter_StateChanged(object sender, EventArgs e)
        {
            if (_isShutdownInitiated || IsDisposed || _fpsLimiterService == null) return;
            var service = _fpsLimiterService;
            if (!service.IsBusy && _fpsLimiterConfiguration.Enabled != service.Enabled)
            {
                _fpsLimiterConfiguration.Enabled = service.Enabled;
                if (_settingsLoaded) SaveSettings();
            }
            SyncFpsLimiterControls();
            if (!service.IsBusy && service.LastError != null && !ReferenceEquals(_fpsLimiterReportedError, service.LastError))
            {
                _fpsLimiterReportedError = service.LastError;
                MessageBox.Show(this, service.StatusText, "FPS limiter", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void SyncFpsLimiterControls()
        {
            if (fpsLimiterEnable == null || fpsLimiterEnable.IsDisposed) return;
            bool busy = (_fpsLimiterService?.IsBusy ?? false) || _fpsLimiterClosePending || _fpsLimiterRestartPending;
            _fpsLimiterUiSync = true;
            fpsLimiterEnable.Checked = _fpsLimiterConfiguration.Enabled;
            fpsLimiterEnable.Enabled = !busy;
            fpsLimiterSettings.Enabled = _fpsLimiterConfiguration.Enabled && !busy;
            _fpsLimiterUiSync = false;
            mainToolTip.SetToolTip(fpsLimiterEnable, _fpsLimiterService?.StatusText ?? "Automatically limits eligible foreground games when enabled.");
        }

        private async void FpsLimiterSettings_Click(object sender, EventArgs e)
        {
            if (_fpsLimiterService?.IsBusy == true || _fpsLimiterRestartPending || _fpsLimiterClosePending) return;
            using (var dialog = new FpsLimiterSettingsDialog(this, _fpsLimiterConfiguration, _fpsLimiterService))
            {
                _fpsLimiterDialog = dialog;
                try
                {
                    if (dialog.ShowDialog(this) != DialogResult.OK || dialog.SelectedConfiguration == null) return;
                    bool enabled = _fpsLimiterConfiguration.Enabled;
                    _fpsLimiterConfiguration = dialog.SelectedConfiguration.Clone();
                    _fpsLimiterConfiguration.Enabled = enabled;
                    ApplyFpsLimiterHotkey();
                    SaveSettings();
                    if (_fpsLimiterService != null) await _fpsLimiterService.ApplyConfigurationAsync(_fpsLimiterConfiguration);
                }
                finally { _fpsLimiterDialog = null; }
            }
        }

        private static bool IsFpsLimiterShortcut(OsdHotkeyModifiers modifiers, Keys key) =>
            modifiers == (OsdHotkeyModifiers.Control | OsdHotkeyModifiers.Shift) && (key & Keys.KeyCode) == Keys.F12;

        internal bool CanRegisterFpsLimiterHotkey()
        {
            if (_osdConfiguration.HotkeyEnabled && IsFpsLimiterShortcut((OsdHotkeyModifiers)_osdConfiguration.HotkeyModifiers, (Keys)_osdConfiguration.HotkeyKey)) return false;
            if (_fpsLimiterHotkeyRegistered) return true;
            if (!IsHandleCreated || IsDisposed) return false;
            bool registered = RegisterHotKey(Handle, FpsLimiterHotkeyTestId, (uint)(OsdHotkeyModifiers.Control | OsdHotkeyModifiers.Shift) | ModNoRepeat, (uint)Keys.F12);
            if (registered) UnregisterHotKey(Handle, FpsLimiterHotkeyTestId);
            return registered;
        }

        private void ApplyFpsLimiterHotkey()
        {
            if (!_fpsLimiterRuntimeReady || _isShutdownInitiated || !IsHandleCreated) return;
            if (!_fpsLimiterConfiguration.ToggleHotkeyEnabled) { UnregisterFpsLimiterHotkey(); return; }
            if (_fpsLimiterHotkeyRegistered) return;
            _fpsLimiterHotkeyRegistered = CanRegisterFpsLimiterHotkey() && RegisterHotKey(Handle, FpsLimiterHotkeyId,
                (uint)(OsdHotkeyModifiers.Control | OsdHotkeyModifiers.Shift) | ModNoRepeat, (uint)Keys.F12);
            if (!_fpsLimiterHotkeyRegistered)
            {
                _fpsLimiterConfiguration.ToggleHotkeyEnabled = false;
                if (_settingsLoaded) SaveSettings();
            }
        }

        private void UnregisterFpsLimiterHotkey()
        {
            if (_fpsLimiterHotkeyRegistered && IsHandleCreated) UnregisterHotKey(Handle, FpsLimiterHotkeyId);
            _fpsLimiterHotkeyRegistered = false;
        }

        private void GiveOsdHotkeyPriority(OsdHotkeyModifiers modifiers, Keys key)
        {
            if (!IsFpsLimiterShortcut(modifiers, key) || !_fpsLimiterConfiguration.ToggleHotkeyEnabled) return;
            UnregisterFpsLimiterHotkey();
            _fpsLimiterConfiguration.ToggleHotkeyEnabled = false;
            if (_settingsLoaded) SaveSettings();
        }

        private void ToggleFpsLimiterFromHotkey()
        {
            if (_osdSettingsDialog != null && !_osdSettingsDialog.IsDisposed &&
                _osdSettingsDialog.CaptureRegisteredHotkey(OsdHotkeyModifiers.Control | OsdHotkeyModifiers.Shift, Keys.F12))
                return;
            if (!_isShutdownInitiated && _fpsLimiterRuntimeReady && fpsLimiterEnable.Enabled)
                fpsLimiterEnable.Checked = !fpsLimiterEnable.Checked;
        }

        private bool DeferFpsLimiterClose(FormClosingEventArgs e)
        {
            if (_fpsLimiterService == null || _fpsLimiterCloseReady) return false;
            UnregisterFpsLimiterHotkey();
            Task stop = _fpsLimiterService.StopAsync();
            if (stop.IsCompleted)
            {
                try { stop.GetAwaiter().GetResult(); } catch (Exception error) { Debug.WriteLine("FPS limiter shutdown: " + error); }
                return false;
            }
            if (e.CloseReason == CloseReason.WindowsShutDown || e.CloseReason == CloseReason.TaskManagerClosing) return false;
            e.Cancel = true;
            if (!_fpsLimiterClosePending)
            {
                _fpsLimiterClosePending = true; SyncFpsLimiterControls();
                _ = FinishFpsLimiterCloseAsync(stop);
            }
            return true;
        }

        private async Task FinishFpsLimiterCloseAsync(Task stop)
        {
            try { await stop; } catch (Exception error) { Debug.WriteLine("FPS limiter shutdown: " + error); }
            if (!IsDisposed) { _fpsLimiterCloseReady = true; Close(); }
        }

        private void ShutdownFpsLimiter()
        {
            _fpsLimiterRuntimeReady = false;
            UnregisterFpsLimiterHotkey();
            _fpsLimiterDialog?.Close();
            if (_fpsLimiterService == null) return;
            _fpsLimiterService.StateChanged -= FpsLimiter_StateChanged;
            _ = ObserveFpsLimiterShutdownAsync(_fpsLimiterService.StopAsync());
        }

        private static async Task ObserveFpsLimiterShutdownAsync(Task stop)
        {
            try { await stop.ConfigureAwait(false); } catch (Exception error) { Debug.WriteLine("FPS limiter shutdown: " + error); }
        }

        private async Task<bool> PrepareFpsLimiterForRestartAsync()
        {
            if (_fpsLimiterRestartPending || _fpsLimiterClosePending || _isShutdownInitiated || IsDisposed) return false;
            if (_fpsLimiterService == null) return true;
            _fpsLimiterRestartPending = true; SyncFpsLimiterControls(); UnregisterFpsLimiterHotkey();
            var service = _fpsLimiterService;
            service.StateChanged -= FpsLimiter_StateChanged;
            try
            {
                await service.StopAsync();
                _fpsLimiterService = null;
                // A user exit can arrive while an attachment is unwinding.
                // Do not launch a replacement or reset controls after that exit.
                return !_fpsLimiterClosePending && !_isShutdownInitiated && !IsDisposed;
            }
            catch (Exception error)
            {
                Debug.WriteLine("FPS limiter restart cleanup: " + error);
                MessageBox.Show(this, "The FPS limiter could not finish cleanup. Close TrayTemps before retrying the restart.", "FPS limiter", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
        }

        private async void ResumeFpsLimiterAfterCancelledRestart()
        {
            if (_isShutdownInitiated || IsDisposed) return;
            _fpsLimiterRestartPending = false;
            ApplyFpsLimiterHotkey(); SyncFpsLimiterControls();
            if (_fpsLimiterConfiguration.Enabled) await SetFpsLimiterEnabledAsync(true);
        }

        private async Task RestartElevatedWithFpsLimiterAsync(bool preserveStartupVisibility)
        {
            if (!await PrepareFpsLimiterForRestartAsync() || _isShutdownInitiated || IsDisposed) return;
            if (TryRestartElevated(preserveStartupVisibility)) { ExecuteShutdownSequence(); Close(); }
            else
            {
                ResumeFpsLimiterAfterCancelledRestart();
                MessageBox.Show(this, "Windows did not allow the elevated restart.", "TrayTemps", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
