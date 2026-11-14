using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using FpsLimiter.Core;

namespace TrayTemps
{
    // Owned by the UI thread. Only the tracked worker may operate on the controller.
    internal sealed class FpsLimiterService : IDisposable
    {
        private readonly System.Windows.Forms.Timer _discovery = new System.Windows.Forms.Timer { Interval = 2000 };
        private readonly Func<string> _nativeDirectory;
        private FpsLimiterConfiguration _configuration;
        private LimiterController _controller;
        private Mutex _ownership;
        private Task _worker = Task.CompletedTask;
        private Task _operation = Task.CompletedTask;
        private Task _shutdown;
        private bool _stopping;
        private int _attachedPid;
        private string _attachedPath = "", _attachedName = "";
        private static readonly string[] DefaultExclusions = { "TrayFPS", "TrayTemps", "Tray-Temps", "explorer", "dwm", "csrss", "services", "svchost", "chrome", "msedge", "firefox", "Discord", "steam", "EpicGamesLauncher", "GameBar", "Widgets" };
        private static readonly char[] Separators = { ',', ';', '\r', '\n' };
        internal bool Enabled { get; private set; }
        internal bool IsBusy { get; private set; }
        internal string StatusText { get; private set; } = "Disabled";
        internal Exception LastError { get; private set; }
        internal event EventHandler StateChanged;

        [DllImport("user32.dll")] private static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll")] private static extern uint GetWindowThreadProcessId(IntPtr hwnd, out uint pid);
        [DllImport("kernel32.dll", SetLastError = true)] private static extern bool IsWow64Process(IntPtr process, out bool wow64);
        [DllImport("advapi32.dll", SetLastError = true)] private static extern bool OpenProcessToken(IntPtr process, uint access, out IntPtr token);
        [DllImport("advapi32.dll", SetLastError = true)] private static extern bool GetTokenInformation(IntPtr token, int informationClass, out int information, int length, out int required);
        [DllImport("kernel32.dll")] private static extern bool CloseHandle(IntPtr handle);

        internal FpsLimiterService(FpsLimiterConfiguration configuration, Func<string> nativeDirectory = null)
        {
            _configuration = configuration.Clone();
            _nativeDirectory = nativeDirectory ?? FpsLimiterPayloads.ExtractNative;
            _discovery.Tick += Discovery_Tick;
        }

        internal Task SetEnabledAsync(bool enabled)
        {
            if (_stopping || IsBusy) return Task.CompletedTask;
            if (enabled == Enabled) return Task.CompletedTask;
            if (enabled)
            {
                try
                {
                    if (_ownership == null)
                    {
                        var lease = new Mutex(false, "Local\\TrayFPS.Instance", out bool created);
                        if (!created) { lease.Dispose(); throw new InvalidOperationException("Another TrayFPS limiter instance is running. Close it before enabling the limiter here."); }
                        _ownership = lease;
                        _controller = new LimiterController();
                    }
                }
                catch (Exception error) { Fail(error); return Task.CompletedTask; }
                return EnableAsync();
            }
            _discovery.Stop();
            if (_controller == null) { Enabled = false; Changed(); return Task.CompletedTask; }
            return RunAsync(_controller.Pause, () => { Enabled = false; ClearTarget(); StatusText = "Disabled"; });
        }

        private async Task EnableAsync()
        {
            await RunAsync(() => { _controller.Start(); _controller.PrepareVulkan(_configuration.FramesPerSecond, _nativeDirectory()); },
                () => { Enabled = true; StatusText = "Waiting for an eligible foreground game"; _discovery.Start(); });
            if (Enabled && !_stopping) Discovery_Tick(this, EventArgs.Empty);
        }

        internal async Task ApplyConfigurationAsync(FpsLimiterConfiguration configuration)
        {
            // A modal settings dialog still pumps discovery messages. Do not lose Save
            // when discovery is attaching a target in the background.
            while (IsBusy && !_stopping) await _operation;
            if (_stopping) return;
            configuration.Validate();
            _configuration = configuration.Clone();
            if (!Enabled) return;
            if (_attachedPid == 0)
                await RunAsync(() => _controller.PrepareVulkan(_configuration.FramesPerSecond, _nativeDirectory()), () => { });
            else if (IsExcluded(_attachedName))
                await RunAsync(_controller.DetachTarget, () => { ClearTarget(); StatusText = "Waiting for an eligible foreground game"; });
            else
                await AttachTargetAsync(_attachedPid, _attachedName, _attachedPath, _configuration.GetCap(_attachedPath));
        }

        internal Task AttachTargetAsync(int pid, string name, string path, int cap)
        {
            if (!Enabled || _stopping || IsBusy || pid <= 0 || pid == Process.GetCurrentProcess().Id || IsExcluded(name)) return Task.CompletedTask;
            bool exited = false, unsupported = false, elevatedVulkan = false, rejected = false;
            Exception rejection = null;
            return RunAsync(() =>
            {
                // Keep the process handle alive through Apply. Windows cannot reuse
                // its PID while this handle is held, even if the candidate exits.
                Process process = null;
                bool applying = false;
                try
                {
                    // The current controller has no cross-architecture injector.
                    // Reject WOW64 before Apply can clear a healthy x64 target.
                    process = Process.GetProcessById(pid);
                    try
                    {
                        if (Environment.Is64BitProcess)
                        {
                            if (!IsWow64Process(process.Handle, out bool wow64)) throw new Win32Exception(Marshal.GetLastWin32Error());
                            if (wow64) { unsupported = true; return; }
                        }
                        // The per-user implicit layer is ignored by elevated Vulkan
                        // loaders. Reject before Apply can replace a working session.
                        if (IsElevated(process) && process.Modules.Cast<ProcessModule>().Any(module => module.ModuleName.Equals("vulkan-1.dll", StringComparison.OrdinalIgnoreCase)))
                        { elevatedVulkan = true; return; }
                        string native = _nativeDirectory();
                        if (process.HasExited) { rejected = true; return; }
                        // Only Apply can change the session. Preflight failures must
                        // leave the previous target and global registration intact.
                        applying = true;
                        try { _controller.Apply(pid, cap, true, native); }
                        catch (Exception error) when ((error is ArgumentException || error is InvalidOperationException || error is Win32Exception) && process.HasExited)
                        {
                            _controller.DetachTarget();
                            _controller.Start();
                            _controller.PrepareVulkan(_configuration.FramesPerSecond, _nativeDirectory());
                            exited = true;
                        }
                    }
                    catch (Win32Exception error) when (!applying && error.NativeErrorCode == 5)
                    { rejected = true; rejection = error; }
                }
                catch (Exception error) when (!applying && (error is ArgumentException || error is InvalidOperationException || error is Win32Exception) && HasExited(pid))
                {
                    rejected = true;
                }
                finally { process?.Dispose(); }
            }, () =>
            {
                if (elevatedVulkan)
                {
                    StatusText = "Elevated Vulkan games are unsupported by the per-user layer. Run the game without administrator rights."
                        + (_attachedPid != 0 ? " The previous target remains active." : "");
                    return;
                }
                if (rejected)
                {
                    LastError = rejection;
                    if (rejection != null) StatusText = "Candidate access denied. TrayTemps and the game must have matching privileges.";
                    return;
                }
                if (unsupported)
                {
                    if (_attachedPid == 0) StatusText = "32-bit game attachment is not supported. Waiting for a compatible 64-bit game.";
                    return;
                }
                if (exited) { ClearTarget(); StatusText = "Waiting for an eligible foreground game"; return; }
                _attachedPid = pid; _attachedPath = path; _attachedName = name;
                StatusText = _controller.Status == LimiterStatus.RestartRequired
                    ? "Restart required: relaunch this Vulkan game while the limiter is enabled."
                    : "Attached to " + name + " - " + cap + " FPS";
            });
        }

        private static bool HasExited(int pid)
        {
            try { using (var process = Process.GetProcessById(pid)) return process.HasExited; }
            catch (ArgumentException) { return true; }
            catch (Win32Exception) { return false; } // Access denied is not proof of exit.
        }

        private static bool IsElevated(Process process)
        {
            if (!OpenProcessToken(process.Handle, 8, out IntPtr token)) throw new Win32Exception(Marshal.GetLastWin32Error());
            try
            {
                if (!GetTokenInformation(token, 20, out int elevated, sizeof(int), out int required)) throw new Win32Exception(Marshal.GetLastWin32Error());
                return elevated != 0;
            }
            finally { CloseHandle(token); }
        }

        private Task RunAsync(Action work, Action completed)
        {
            IsBusy = true; LastError = null; Changed();
            _worker = Task.Run(work);
            _operation = CompleteAsync(_worker, completed);
            return _operation;
        }

        private async Task CompleteAsync(Task worker, Action completed)
        {
            try { await worker; if (!_stopping) completed(); }
            catch (Exception error) { if (!_stopping) Fail(error); }
            finally { if (!_stopping) { IsBusy = false; Changed(); } }
        }

        private void Fail(Exception error)
        {
            Enabled = false; _discovery.Stop(); ClearTarget(); LastError = error;
            StatusText = error is UnauthorizedAccessException || (error is Win32Exception win32 && win32.NativeErrorCode == 5)
                ? "Access denied. TrayTemps and the game must have matching privileges." : error.Message;
            // Never clean up another instance: a controller exists only after ownership succeeds.
            try { _controller?.Stop(); } catch (Exception cleanup) { Debug.WriteLine("FPS limiter cleanup: " + cleanup); }
            Changed();
        }

        private void ClearTarget() { _attachedPid = 0; _attachedPath = ""; _attachedName = ""; }
        private void Changed() { if (!_stopping) StateChanged?.Invoke(this, EventArgs.Empty); }

        private bool IsExcluded(string name)
        {
            var user = (_configuration.ExcludedProcesses ?? "").Split(Separators, StringSplitOptions.RemoveEmptyEntries)
                .Select(item => item.Trim()).Where(item => item.Length != 0);
            return DefaultExclusions.Concat(user).Any(item => string.Equals(item.EndsWith(".exe", StringComparison.OrdinalIgnoreCase) ? item.Substring(0, item.Length - 4) : item, name, StringComparison.OrdinalIgnoreCase));
        }

        private async void Discovery_Tick(object sender, EventArgs args)
        {
            if (!Enabled || IsBusy || _stopping) return;
            try
            {
                if (_attachedPid != 0)
                {
                    bool exited;
                    try { using (var process = Process.GetProcessById(_attachedPid)) exited = process.HasExited; }
                    catch (ArgumentException) { exited = true; }
                    if (exited)
                    {
                        await RunAsync(_controller.DetachTarget, () => { ClearTarget(); StatusText = "Waiting for an eligible foreground game"; });
                        if (_stopping || !Enabled) return;
                    }
                }
                if (GetWindowThreadProcessId(GetForegroundWindow(), out uint pid) == 0 || pid == 0 || pid == (uint)Process.GetCurrentProcess().Id || pid == _attachedPid) return;
                using (var process = Process.GetProcessById((int)pid))
                {
                    string name = process.ProcessName;
                    if (IsExcluded(name)) return;
                    string path = GetExecutablePath(process);
                    var game = _configuration.FindGame(path);
                    if (game == null && !LooksLikeGame(process, path)) return;
                    await AttachTargetAsync(process.Id, name, path, game?.FramesPerSecond ?? _configuration.FramesPerSecond);
                }
            }
            catch (ArgumentException) { }
            catch (InvalidOperationException) { }
            catch (Win32Exception) { }
            catch (NotSupportedException) { }
            catch (Exception error) { if (!_stopping) Fail(error); }
        }

        private static string GetExecutablePath(Process process)
        {
            try { return Path.GetFullPath(process.MainModule.FileName); }
            catch (Win32Exception) { return ""; }
            catch (InvalidOperationException) { return ""; }
            catch (NotSupportedException) { return ""; }
            catch (PathTooLongException) { return ""; }
        }

        private static bool LooksLikeGame(Process process, string path)
        {
            if (string.IsNullOrEmpty(path) || string.IsNullOrEmpty(process.MainWindowTitle) || process.MainWindowHandle == IntPtr.Zero) return false;
            string lower = path.ToLowerInvariant();
            if (lower.Contains("\\steamapps\\common\\") || lower.Contains("\\epic games\\") || lower.Contains("\\xboxgames\\") || lower.Contains("\\gog games\\")) return true;
            foreach (ProcessModule module in process.Modules)
            {
                string name = module.ModuleName;
                if (name.Equals("UnityPlayer.dll", StringComparison.OrdinalIgnoreCase) || name.Equals("GameAssembly.dll", StringComparison.OrdinalIgnoreCase) || name.StartsWith("Unreal", StringComparison.OrdinalIgnoreCase) || name.Equals("CrySystem.dll", StringComparison.OrdinalIgnoreCase)) return true;
            }
            return false;
        }

        internal Task StopAsync()
        {
            if (_shutdown != null) return _shutdown;
            _stopping = true; _discovery.Stop(); _discovery.Dispose();
            _shutdown = StopCoreAsync();
            return _shutdown;
        }

        private async Task StopCoreAsync()
        {
            // Cleanup must also complete after direct form disposal without a running UI context.
            try { await _worker.ConfigureAwait(false); }
            catch (Exception error) { Debug.WriteLine("FPS limiter worker during shutdown: " + error); }
            _controller?.Dispose();
            _controller = null;
            // Retain ownership if cleanup fails; another publisher must not start.
            _ownership?.Dispose(); _ownership = null;
        }

        public void Dispose() { _ = StopAsync(); }
    }
}
