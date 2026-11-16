using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
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
        private Task<DiscoverySnapshot> _inspection = Task.FromResult<DiscoverySnapshot>(null);
        private bool _discoveryInProgress;
        private string _lastDetectionReport;
        private bool _stopping;
        private int _attachedPid;
        private DateTime _attachedStartTime;
        private string _attachedPath = "", _attachedName = "";
        private readonly Dictionary<int, DateTime> _rejectedCandidates = new Dictionary<int, DateTime>();
        private static readonly string[] DefaultExclusions = { "TrayFPS", "TrayTemps", "Tray-Temps", "explorer", "dwm", "csrss", "services", "svchost", "chrome", "msedge", "firefox", "Discord", "steam", "EpicGamesLauncher", "GameBar", "Widgets" };
        private static readonly char[] Separators = { ',', ';', '\r', '\n' };
        internal bool Enabled { get; private set; }
        internal bool IsBusy { get; private set; }
        internal string StatusText { get; private set; } = "Disabled";
        internal Exception LastError { get; private set; }
        internal bool IsDiscovering => _discoveryInProgress;
        internal string LastDetectionReason { get; private set; }
        internal event EventHandler StateChanged;

        [DllImport("user32.dll")] private static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll")] private static extern uint GetWindowThreadProcessId(IntPtr hwnd, out uint pid);
        [DllImport("user32.dll")] private static extern bool IsWindowVisible(IntPtr hwnd);
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

        internal static void CleanupStaleVulkanRegistrations()
        {
            using (var lease = new Mutex(false, "Local\\TrayFPS.Instance", out bool created))
                if (created) LimiterController.CleanupOwnLayerRegistrations();
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
                        LimiterController.CleanupOwnLayerRegistrations();
                        _controller = new LimiterController();
                    }
                }
                catch (Exception error) { Fail(error); return Task.CompletedTask; }
                return EnableAsync();
            }
            _discovery.Stop();
            if (_controller == null) { Enabled = false; Changed(); return Task.CompletedTask; }
            return RunAsync(ReleaseController, () => { Enabled = false; ClearTarget(); _rejectedCandidates.Clear(); StatusText = "Disabled"; });
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
            => AttachTargetCoreAsync(pid, name, path, cap, null, false);

        private Task AttachTargetCoreAsync(int pid, string name, string path, int cap, DateTime? expectedStart, bool requireForeground)
        {
            if (!Enabled || _stopping || IsBusy || pid <= 0 || pid == Process.GetCurrentProcess().Id || IsExcluded(name)) return Task.CompletedTask;
            bool unsupported = false, elevatedVulkan = false, rejected = false;
            Exception rejection = null;
            string skipped = null;
            DateTime attachedIdentity = default(DateTime);
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
                        DateTime identity = process.StartTime.ToUniversalTime();
                        attachedIdentity = identity;
                        if (expectedStart.HasValue && (identity != expectedStart.Value ||
                            !string.Equals(GetExecutablePath(process), path, StringComparison.OrdinalIgnoreCase)))
                        { rejected = true; skipped = "Process identity changed"; return; }
                        if (requireForeground && !IsForegroundProcess(pid))
                        { rejected = true; skipped = "Foreground changed before attachment"; return; }
                        if (_rejectedCandidates.TryGetValue(pid, out DateTime previous) && previous == identity) { rejected = true; return; }
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
                        if (requireForeground && !IsForegroundProcess(pid))
                        { rejected = true; skipped = "Foreground changed before attachment"; return; }
                        // Only Apply can change the session. Preflight failures must
                        // leave the previous target and global registration intact.
                        applying = true;
                        try { _controller.Apply(pid, cap, true, native); }
                        catch (Exception error) when (error is ArgumentException || error is InvalidOperationException || error is Win32Exception || error is TimeoutException || error is UnauthorizedAccessException)
                        {
                            rejected = true;
                            if (!HasExited(pid)) rejection = error;
                            if (_rejectedCandidates.Count >= 256) _rejectedCandidates.Remove(_rejectedCandidates.First().Key);
                            _rejectedCandidates[pid] = identity;
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
                    if (skipped != null) ReportDetection(pid, name, skipped);
                    LastError = rejection;
                    if (rejection != null) StatusText = "Could not attach to " + name + ": " + rejection.Message
                        + (_attachedPid != 0 ? " The previous target remains active." : " Waiting for another game.");
                    return;
                }
                if (unsupported)
                {
                    if (_attachedPid == 0) StatusText = "32-bit game attachment is not supported. Waiting for a compatible 64-bit game.";
                    return;
                }
                _attachedPid = pid; _attachedPath = path; _attachedName = name;
                _attachedStartTime = attachedIdentity;
                StatusText = _controller.Status == LimiterStatus.RestartRequired
                    ? "Restart required: relaunch this Vulkan game while the limiter is enabled."
                    : "Attached to " + name + " - " + cap + " FPS requested (presentation hooks ready)";
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
            try { ReleaseController(); } catch (Exception cleanup) { Debug.WriteLine("FPS limiter cleanup: " + cleanup); }
            Changed();
        }

        private void ClearTarget() { _attachedPid = 0; _attachedStartTime = default(DateTime); _attachedPath = ""; _attachedName = ""; }
        private void Changed() { if (!_stopping) StateChanged?.Invoke(this, EventArgs.Empty); }

        private bool IsExcluded(string name) => IsExcludedByConfiguration(name, _configuration);

        private static bool IsExcludedByConfiguration(string name, FpsLimiterConfiguration configuration)
        {
            var user = (configuration.ExcludedProcesses ?? "").Split(Separators, StringSplitOptions.RemoveEmptyEntries)
                .Select(item => item.Trim()).Where(item => item.Length != 0);
            return DefaultExclusions.Concat(user).Any(item => string.Equals(item.EndsWith(".exe", StringComparison.OrdinalIgnoreCase) ? item.Substring(0, item.Length - 4) : item, name, StringComparison.OrdinalIgnoreCase));
        }

        private async void Discovery_Tick(object sender, EventArgs args)
            => await DiscoverForegroundAsync();

        internal async Task DiscoverForegroundAsync()
        {
            if (!Enabled || IsBusy || _stopping || _discoveryInProgress) return;
            _discoveryInProgress = true;
            try
            {
                GetWindowThreadProcessId(GetForegroundWindow(), out uint foreground);
                int pid = foreground <= int.MaxValue ? (int)foreground : 0;
                int previousTarget = _attachedPid;
                DateTime previousIdentity = _attachedStartTime;
                // Configuration objects are privately cloned and replaced, never mutated.
                // Hold this snapshot while the UI remains free to save a newer one.
                var configuration = _configuration;
                DateTime? previousRejection = _rejectedCandidates.TryGetValue(pid, out DateTime rejectedStart) ? rejectedStart : (DateTime?)null;
                _inspection = Task.Run(() => InspectForeground(pid, previousTarget, previousIdentity, configuration, previousRejection));
                var snapshot = await _inspection;
                if (_stopping || !Enabled || IsBusy || previousTarget != _attachedPid || previousIdentity != _attachedStartTime) return;
                if (snapshot.AttachedExited)
                {
                    await RunAsync(_controller.DetachTarget, () => { ClearTarget(); StatusText = "Waiting for an eligible foreground game"; });
                    if (_stopping || !Enabled) return;
                }
                if (!snapshot.HasCandidate || !IsForegroundProcess(snapshot.Pid)) return;
                // Recheck exclusions/profiles against the latest settings after inspection.
                if (IsExcluded(snapshot.Name)) { ReportDetection(snapshot.Pid, snapshot.Name, "Excluded"); return; }
                if (_rejectedCandidates.TryGetValue(snapshot.Pid, out DateTime rejected) && rejected == snapshot.Started)
                {
                    ReportDetection(snapshot.Pid, snapshot.Name, "Previous attachment rejected; restart game or toggle limiter to retry");
                    return;
                }
                var game = _configuration.FindGame(snapshot.Path);
                ReportDetection(snapshot.Pid, snapshot.Name, game != null ? "Executable profile" : snapshot.Decision.Reason);
                if (game == null && !snapshot.Decision.Eligible) return;
                await AttachTargetCoreAsync(snapshot.Pid, snapshot.Name, snapshot.Path,
                    game?.FramesPerSecond ?? _configuration.FramesPerSecond, snapshot.Started, true);
            }
            catch (Exception error) when (error is ArgumentException || error is InvalidOperationException ||
                error is Win32Exception || error is NotSupportedException)
            { Trace.WriteLine("FPS discovery: " + error.Message); }
            catch (Exception error) { if (!_stopping) Fail(error); }
            finally { _discoveryInProgress = false; }
        }

        private sealed class DiscoverySnapshot
        {
            internal int Pid;
            internal string Name = "", Path = "";
            internal DateTime Started;
            internal bool AttachedExited, HasCandidate;
            internal GameDetectionDecision Decision = new GameDetectionDecision(false, "Process information unavailable");
        }

        private static DiscoverySnapshot InspectForeground(int pid, int attachedPid, DateTime attachedIdentity, FpsLimiterConfiguration configuration, DateTime? rejectedStart)
        {
            var result = new DiscoverySnapshot { Pid = pid, AttachedExited = attachedPid != 0 && HasProcessIdentityChanged(attachedPid, attachedIdentity) };
            if (pid <= 0 || pid == Process.GetCurrentProcess().Id || (pid == attachedPid && !result.AttachedExited)) return result;
            result.HasCandidate = true;
            try
            {
                using (var process = Process.GetProcessById(pid))
                {
                    result.Name = process.ProcessName;
                    if (IsExcludedByConfiguration(result.Name, configuration))
                    { result.Decision = new GameDetectionDecision(false, "Excluded"); return result; }
                    result.Started = process.StartTime.ToUniversalTime();
                    result.Path = GetExecutablePath(process);
                    if (rejectedStart == result.Started) return result;
                    if (configuration.FindGame(result.Path) != null)
                    {
                        // The UI rechecks the profile after this worker returns. A removed
                        // profile must wait for a fresh automatic inspection on the next tick.
                        result.Decision = new GameDetectionDecision(false, "Profile changed; waiting for automatic inspection");
                        return result;
                    }
                    IntPtr window = process.MainWindowHandle;
                    bool visible = window != IntPtr.Zero && IsWindowVisible(window) && !string.IsNullOrEmpty(process.MainWindowTitle);
                    result.Decision = FpsLimiterGameDetection.Evaluate(result.Path, visible, null);
                    if (!visible || result.Decision.Eligible || result.Path.Length == 0) return result;
                    result.Decision = FpsLimiterGameDetection.Evaluate(result.Path, visible,
                        process.Modules.Cast<ProcessModule>().Select(module => module.ModuleName));
                }
            }
            catch (Exception error) when (error is ArgumentException || error is InvalidOperationException ||
                error is Win32Exception || error is NotSupportedException)
            { result.Decision = new GameDetectionDecision(false, "Process information unavailable"); }
            return result;
        }

        private static bool HasProcessIdentityChanged(int pid, DateTime expected)
        {
            try
            {
                using (var process = Process.GetProcessById(pid))
                    return process.HasExited || (expected != default(DateTime) && process.StartTime.ToUniversalTime() != expected);
            }
            catch (ArgumentException) { return true; }
            catch (InvalidOperationException) { return true; }
            catch (Win32Exception) { return false; }
        }

        private static bool IsForegroundProcess(int pid) =>
            GetWindowThreadProcessId(GetForegroundWindow(), out uint current) != 0 && current == (uint)pid;

        private void ReportDetection(int pid, string name, string reason)
        {
            string report = pid + ": " + name + " — " + reason;
            if (_lastDetectionReport == report) return;
            _lastDetectionReport = report;
            LastDetectionReason = reason;
            Trace.WriteLine("FPS discovery: " + report);
            if (_attachedPid == 0)
            {
                StatusText = "Waiting for an eligible foreground game — " + reason;
                Changed();
            }
        }

        private static string GetExecutablePath(Process process)
        {
            try { return Path.GetFullPath(process.MainModule.FileName); }
            catch (Win32Exception) { return ""; }
            catch (InvalidOperationException) { return ""; }
            catch (NotSupportedException) { return ""; }
            catch (PathTooLongException) { return ""; }
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
            try { await Task.WhenAll(_worker, _inspection).ConfigureAwait(false); }
            catch (Exception error) { Debug.WriteLine("FPS limiter worker during shutdown: " + error); }
            ReleaseController();
        }

        private void ReleaseController()
        {
            _controller?.Dispose();
            _controller = null;
            // Retain ownership if cleanup fails; another publisher must not start.
            _ownership?.Dispose(); _ownership = null;
        }

        public void Dispose() { _ = StopAsync(); }
    }
}
