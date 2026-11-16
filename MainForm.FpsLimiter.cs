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
        private OsdHotkeyModifiers _registeredFpsLimiterHotkeyModifiers;
        private Keys _registeredFpsLimiterHotkeyKey;
        private bool _fpsLimiterClosePending, _fpsLimiterCloseReady, _fpsLimiterRestartPending;
        private Exception _fpsLimiterReportedError;

        private async void InitializeFpsLimiterRuntime()
        {
            if (_fpsLimiterRuntimeReady || _isShutdownInitiated || EmbeddedFonts.IsDesignTime(this)) return;
            _fpsLimiterRuntimeReady = true;
            try { FpsLimiterService.CleanupStaleVulkanRegistrations(); }
            catch (Exception error) { Debug.WriteLine("Stale FPS limiter layer cleanup: " + error); }
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
            fpsLimiterSettings.Enabled = !busy && _fpsLimiterConfiguration.Enabled;
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

        private bool IsFpsLimiterShortcut(OsdHotkeyModifiers modifiers, Keys key) =>
            modifiers == (OsdHotkeyModifiers)_fpsLimiterConfiguration.ToggleHotkeyModifiers &&
            (key & Keys.KeyCode) == ((Keys)_fpsLimiterConfiguration.ToggleHotkeyKey & Keys.KeyCode);

        internal bool CanRegisterFpsLimiterHotkey() => CanRegisterFpsLimiterHotkey(
            (OsdHotkeyModifiers)_fpsLimiterConfiguration.ToggleHotkeyModifiers, (Keys)_fpsLimiterConfiguration.ToggleHotkeyKey);

        internal bool CanRegisterFpsLimiterHotkey(OsdHotkeyModifiers modifiers, Keys key)
        {
            key &= Keys.KeyCode;
            if (!FpsLimiterConfiguration.IsValidHotkey(modifiers, key) || !IsHandleCreated || IsDisposed) return false;
            if (_osdConfiguration.HotkeyEnabled && modifiers == (OsdHotkeyModifiers)_osdConfiguration.HotkeyModifiers &&
                key == ((Keys)_osdConfiguration.HotkeyKey & Keys.KeyCode)) return false;
            if (_fpsLimiterHotkeyRegistered && _registeredFpsLimiterHotkeyModifiers == modifiers && _registeredFpsLimiterHotkeyKey == key) return true;
            bool registered = RegisterHotKey(Handle, FpsLimiterHotkeyTestId, (uint)modifiers | ModNoRepeat, (uint)key);
            if (registered) UnregisterHotKey(Handle, FpsLimiterHotkeyTestId);
            return registered;
        }

        private void ApplyFpsLimiterHotkey()
        {
            if (!_fpsLimiterRuntimeReady || _isShutdownInitiated || !IsHandleCreated) return;
            if (!_fpsLimiterConfiguration.ToggleHotkeyEnabled) { UnregisterFpsLimiterHotkey(); return; }
            var modifiers = (OsdHotkeyModifiers)_fpsLimiterConfiguration.ToggleHotkeyModifiers;
            Keys key = (Keys)_fpsLimiterConfiguration.ToggleHotkeyKey & Keys.KeyCode;
            if (_fpsLimiterHotkeyRegistered && _registeredFpsLimiterHotkeyModifiers == modifiers && _registeredFpsLimiterHotkeyKey == key) return;
            UnregisterFpsLimiterHotkey();
            _fpsLimiterHotkeyRegistered = CanRegisterFpsLimiterHotkey(modifiers, key) && RegisterHotKey(Handle, FpsLimiterHotkeyId,
                (uint)modifiers | ModNoRepeat, (uint)key);
            if (_fpsLimiterHotkeyRegistered)
            {
                _registeredFpsLimiterHotkeyModifiers = modifiers;
                _registeredFpsLimiterHotkeyKey = key;
            }
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
            _registeredFpsLimiterHotkeyModifiers = OsdHotkeyModifiers.None;
            _registeredFpsLimiterHotkeyKey = Keys.None;
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
            if (!_fpsLimiterHotkeyRegistered) return;
            if (CaptureHotkeyInSettings(_registeredFpsLimiterHotkeyModifiers, _registeredFpsLimiterHotkeyKey)) return;
            if (!_isShutdownInitiated && _fpsLimiterRuntimeReady && fpsLimiterEnable.Enabled)
                fpsLimiterEnable.Checked = !fpsLimiterEnable.Checked;
        }

        private bool CaptureHotkeyInSettings(OsdHotkeyModifiers modifiers, Keys key) =>
            (_fpsLimiterDialog != null && !_fpsLimiterDialog.IsDisposed && _fpsLimiterDialog.CaptureRegisteredHotkey(modifiers, key)) ||
            (_osdSettingsDialog != null && !_osdSettingsDialog.IsDisposed && _osdSettingsDialog.CaptureRegisteredHotkey(modifiers, key));

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
