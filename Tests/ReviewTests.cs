using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using FpsLimiter.Core;
using Microsoft.Win32;
using TrayTemps;

internal static partial class ReviewTests
{
    private const BindingFlags Private = BindingFlags.Instance | BindingFlags.NonPublic;
    private static string _output;
    private static readonly string Repos = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "source", "repos");
    private static readonly string HostSettings = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "TrayTemps", "settings.json");
    private static readonly string StandaloneSettings = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "TrayFPS", "settings.json");
    private static string Target => Path.Combine(Repos, "FPSlimiterAsync", "tests", "D3D11GraphicsTest", "x64", "Release", "D3D11GraphicsTest.exe");
    private static string Standalone => Path.Combine(Repos, "FPSlimiterAsync", "dist", "TrayFPS.exe");
    private static object Get(object instance, string name) => typeof(MainForm).GetField(name, Private).GetValue(instance);
    private static void Set(object instance, string name, object value) => typeof(MainForm).GetField(name, Private).SetValue(instance, value);
    private static object Call(object instance, string name, params object[] args) => typeof(MainForm).GetMethod(name, Private).Invoke(instance, args);
    private static void Check(bool condition, string name) { Console.WriteLine((condition ? "PASS: " : "FAIL: ") + name); if (!condition) throw new Exception(name); }
    [DllImport("user32.dll")] private static extern bool SetForegroundWindow(IntPtr window);
    [DllImport("user32.dll")] private static extern IntPtr GetForegroundWindow();
    [DllImport("user32.dll")] private static extern uint GetWindowThreadProcessId(IntPtr window, out uint pid);
    [DllImport("user32.dll")] private static extern bool AttachThreadInput(uint first, uint second, bool attach);
    [DllImport("kernel32.dll")] private static extern uint GetCurrentThreadId();
    [DllImport("user32.dll")] private static extern bool PostMessage(IntPtr window, uint message, IntPtr w, IntPtr l);
    private delegate bool EnumWindow(IntPtr window, IntPtr context);
    [DllImport("user32.dll")] private static extern bool EnumWindows(EnumWindow callback, IntPtr context);
    [DllImport("user32.dll")] private static extern IntPtr GetDlgItem(IntPtr window, int id);
    [DllImport("user32.dll", CharSet = CharSet.Unicode)] private static extern int GetClassName(IntPtr window, System.Text.StringBuilder name, int capacity);
    [DllImport("user32.dll", CharSet = CharSet.Unicode)] private static extern int GetWindowText(IntPtr window, System.Text.StringBuilder text, int capacity);
    [DllImport("user32.dll")] private static extern bool IsWindowEnabled(IntPtr window);
    [DllImport("advapi32.dll", SetLastError = true)] private static extern bool OpenProcessToken(IntPtr process, uint access, out IntPtr token);
    [DllImport("advapi32.dll", SetLastError = true)] private static extern bool GetTokenInformation(IntPtr token, int kind, out int value, int size, out int needed);
    [DllImport("kernel32.dll")] private static extern bool CloseHandle(IntPtr handle);
    [DllImport("user32.dll")] private static extern IntPtr SetThreadDpiAwarenessContext(IntPtr context);
    [DllImport("user32.dll")] private static extern IntPtr MonitorFromPoint(Point point, uint flags);
    [DllImport("shcore.dll")] private static extern int GetDpiForMonitor(IntPtr monitor, int type, out uint x, out uint y);
    [DllImport("user32.dll")] private static extern bool SetWindowPos(IntPtr window, IntPtr after, int x, int y, int width, int height, uint flags);
    [DllImport("user32.dll")] private static extern uint GetDpiForWindow(IntPtr window);
    [DllImport("user32.dll")] private static extern bool ShowWindowAsync(IntPtr window, int command);
    private static bool Elevated(Process process)
    {
        if (!OpenProcessToken(process.Handle, 8, out IntPtr token)) throw new System.ComponentModel.Win32Exception();
        try { if (!GetTokenInformation(token, 20, out int value, 4, out int size)) throw new System.ComponentModel.Win32Exception(); return value != 0; }
        finally { CloseHandle(token); }
    }

    internal static async Task Run(string[] args)
    {
        if (args.Length > 0 && !new[] { "unit", "preflight", "x86", "vulkan", "capture", "cleanup-fixture", "ownership", "startup", "compare", "pacing", "steady", "interactive", "cancel", "hardware", "dpi", "dpi-roundtrip", "mixed-vulkan", "host-vulkan", "host-vulkan-60", "real-osd", "cancel-cap", "pending-exit", "finish-dpi" }.Contains(args[0]))
            throw new ArgumentException("Unknown review mode: " + args[0]);
        _output = Path.Combine(Path.GetTempPath(), "TrayTemps-review-evidence-" + Guid.NewGuid().ToString("N")); Directory.CreateDirectory(_output);
        Console.WriteLine("REVIEW_EVIDENCE=" + _output);
        if (args.Length > 0 && args[0] == "finish-dpi") { await FinishDpi(); return; }
        if (args.Length > 0 && args[0] == "cleanup-fixture")
        {
            foreach (var process in Process.GetProcessesByName("TrayTemps"))
                using (process) { Check(string.Equals(process.MainModule.FileName, Path.Combine(Repos, "Tray-Temps", "dist", "TrayTemps.exe"), StringComparison.OrdinalIgnoreCase), "cleanup process is this pass's fixture"); await Close(process); }
            return;
        }
        if (args.Length > 0 && args[0] == "preflight") { await PreflightExit(); await ExitDuringApply(); return; }
        if (args.Length == 0 || args[0] == "unit") { await RepeatedEnable(); await ExitingTarget(); await PayloadRepair(); await HotkeysAndDpi(); await ModalSettings(); await OsdIndependence(); DesignTimeGuards(); await SlowRestart(); return; }
        if (args[0] == "x86") { await X86(); return; }
        if (args[0] == "vulkan") { await VulkanRestart(); return; }
        if (args[0] == "capture") { await OsdShortcutCapture(); return; }
        Check(Process.GetProcessesByName("TrayTemps").Length == 0 && Process.GetProcessesByName("TrayFPS").Length == 0, "no live user instance before isolated process tests");
        using (var hostPreferences = new Preferences(HostSettings))
        using (var standalonePreferences = new Preferences(StandaloneSettings))
        {
            if (args[0] == "ownership") await OwnershipOrders();
            if (args[0] == "startup") await StartupMatrix();
            if (args[0] == "compare") await Compare();
            if (args[0] == "pacing") await Compare(false, true);
            if (args[0] == "steady") await Compare(true);
            if (args[0] == "interactive") await InteractiveRestart();
            if (args[0] == "cancel") await InteractiveRestart(true);
            if (args[0] == "hardware") await Hardware();
            if (args[0] == "dpi") await DpiRestart();
            if (args[0] == "dpi-roundtrip") await DpiRoundTrips();
            if (args[0] == "mixed-vulkan") await MixedVulkan();
            if (args[0] == "host-vulkan") await HostVulkan();
            if (args[0] == "host-vulkan-60") await HostVulkan60();
            if (args[0] == "real-osd") await RealOsd();
            if (args[0] == "cancel-cap") await CancelCap();
            if (args[0] == "pending-exit") await PendingExit();
        }
    }

    private sealed class Preferences : IDisposable
    {
        private sealed class Entry { internal string Path; internal byte[] Bytes; internal DateTime Time; }
        private readonly List<Entry> _entries = new List<Entry>();
        internal Preferences(string path)
        {
            foreach (string name in new[] { path, path + ".bak", path + ".tmp" })
                _entries.Add(new Entry { Path = name, Bytes = File.Exists(name) ? File.ReadAllBytes(name) : null, Time = File.Exists(name) ? File.GetLastWriteTimeUtc(name) : DateTime.MinValue });
            // Keep a durable recovery copy in case the test driver is terminated.
            string folder = Path.Combine(_output, path == HostSettings ? "host-preferences" : "standalone-preferences"); Directory.CreateDirectory(folder);
            foreach (var entry in _entries) if (entry.Bytes != null) File.WriteAllBytes(Path.Combine(folder, Path.GetFileName(entry.Path)), entry.Bytes);
        }
        public void Dispose()
        {
            foreach (var entry in _entries)
            {
                if (entry.Bytes == null) { if (File.Exists(entry.Path)) File.Delete(entry.Path); }
                else { File.WriteAllBytes(entry.Path, entry.Bytes); File.SetLastWriteTimeUtc(entry.Path, entry.Time); }
            }
        }
    }

    private static AppSettings Config(bool limiter, bool osd, int cap = 60)
    {
        var settings = new AppSettings { StartMinimizedWithAdminRights = false, CpuTrayIcon = false, GpuTrayIcon = false };
        settings.Osd.Enabled = osd;
        settings.Osd.ShowFps = true;
        settings.FpsLimiter.Enabled = limiter; settings.FpsLimiter.FramesPerSecond = cap;
        settings.FpsLimiter.GameLimits.Add(new GameLimit { ExecutablePath = Target, FramesPerSecond = cap });
        return settings;
    }
    private static void WriteHost(AppSettings settings)
    {
        foreach (string suffix in new[] { ".bak", ".tmp" }) if (File.Exists(HostSettings + suffix)) File.Delete(HostSettings + suffix);
        SettingsFileHelper.WriteSettings(HostSettings, settings);
    }
    private static void WriteStandalone(int cap)
    {
        var settings = new LimiterSettings { LimiterEnabled = true, FramesPerSecond = cap };
        settings.GameLimits.Add(new GameLimit { ExecutablePath = Target, FramesPerSecond = cap }); settings.SaveAtomic(StandaloneSettings);
    }
    private static Process Launch(string path, string arguments = "") => Process.Start(new ProcessStartInfo(path, arguments) { UseShellExecute = false, WorkingDirectory = Path.GetDirectoryName(path) });
    private static async Task Close(Process process)
    {
        if (process == null || process.HasExited) return;
        string processName = process.ProcessName;
        process.Refresh();
        if (!process.CloseMainWindow())
            EnumWindows((window, ignored) => { GetWindowThreadProcessId(window, out uint pid); var title = new System.Text.StringBuilder(128); GetWindowText(window, title, title.Capacity); if (pid == process.Id && title.ToString() == "TrayTemps") PostMessage(window, 0x10, IntPtr.Zero, IntPtr.Zero); return true; }, IntPtr.Zero);
        var watch = Stopwatch.StartNew();
        while (!process.HasExited && watch.ElapsedMilliseconds < 30000)
        {
            EnumWindows((window, ignored) =>
            {
                GetWindowThreadProcessId(window, out uint pid);
                if (pid == process.Id)
                {
                    var name = new System.Text.StringBuilder(64); GetClassName(window, name, name.Capacity);
                    if (name.ToString() == "#32770")
                    {
                        IntPtr yes = GetDlgItem(window, 6); if (yes != IntPtr.Zero) PostMessage(window, 0x111, (IntPtr)6, yes);
                        else { IntPtr ok = GetDlgItem(window, 1); if (ok != IntPtr.Zero) PostMessage(window, 0x111, (IntPtr)1, ok); }
                    }
                    else if (watch.ElapsedMilliseconds > 500 && IsWindowEnabled(window)) { var title = new System.Text.StringBuilder(128); GetWindowText(window, title, title.Capacity); if (title.ToString() == "TrayTemps" || title.ToString() == "TrayFPS") PostMessage(window, 0x10, IntPtr.Zero, IntPtr.Zero); }
                }
                return true;
            }, IntPtr.Zero);
            await Task.Delay(100);
        }
        if (!process.HasExited) { process.Kill(); process.WaitForExit(); throw new Exception("Forced test cleanup for " + processName); }
    }
    private static void DismissOk(Process process)
    {
        EnumWindows((window, ignored) =>
        {
            GetWindowThreadProcessId(window, out uint pid); var name = new System.Text.StringBuilder(64); GetClassName(window, name, name.Capacity);
            if (pid == process.Id && name.ToString() == "#32770") { IntPtr ok = GetDlgItem(window, 1); if (ok != IntPtr.Zero) PostMessage(window, 0x111, (IntPtr)1, ok); }
            return true;
        }, IntPtr.Zero);
    }
    private static async Task Focus(Process process)
    {
        for (int attempt = 0; attempt < 50; ++attempt)
        {
            process.Refresh(); IntPtr window = process.MainWindowHandle;
            if (process.ProcessName == "D3D11GraphicsTest")
                EnumWindows((candidate, ignoredContext) =>
                {
                    GetWindowThreadProcessId(candidate, out uint owner);
                    var name = new System.Text.StringBuilder(64); GetClassName(candidate, name, name.Capacity);
                    if (owner == process.Id && name.ToString() == "TrayFPS.D3D11Test") window = candidate;
                    return true;
                }, IntPtr.Zero);
            if (window != IntPtr.Zero)
            {
                ShowWindowAsync(window, 9); // Explorer's hidden broker can otherwise hide its first child window.
                uint current = GetCurrentThreadId(), foreground = GetWindowThreadProcessId(GetForegroundWindow(), out uint ignored), target = GetWindowThreadProcessId(window, out ignored);
                bool joined = current != foreground && AttachThreadInput(current, foreground, true);
                bool joinedTarget = target != current && target != foreground && AttachThreadInput(current, target, true);
                try { SetForegroundWindow(window); }
                finally { if (joinedTarget) AttachThreadInput(current, target, false); if (joined) AttachThreadInput(current, foreground, false); }
                await Task.Delay(100); if (GetForegroundWindow() == window) return;
            }
            await Task.Delay(100);
        }
        throw new Exception("Unable to focus controlled target");
    }
    private static double ReadCap(int pid, bool enabled = true)
    {
        using (var mapping = MemoryMappedFile.OpenExisting("Local\\TrayFPS.Config." + pid))
        using (var view = mapping.CreateViewAccessor()) { Check(view.ReadInt32(12) == (enabled ? 1 : 0), "target shared enable state"); return view.ReadDouble(24); }
    }
    private static async Task WaitForCap(Process target, int cap)
    {
        var wait = Stopwatch.StartNew();
        while (wait.ElapsedMilliseconds < 8000)
        {
            await Focus(target);
            try
            {
                using (var mapping = MemoryMappedFile.OpenExisting("Local\\TrayFPS.Config." + target.Id))
                using (var view = mapping.CreateViewAccessor()) if (view.ReadInt32(12) == 1 && view.ReadDouble(24) == cap) return;
            }
            catch (FileNotFoundException) { }
            await Task.Delay(300);
        }
        throw new Exception("No matching cap after foreground discovery: " + cap);
    }
    private static string Layers()
    {
        var entries = new List<string>();
        foreach (var view in new[] { RegistryView.Registry64, RegistryView.Registry32 })
        using (var root = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, view))
        using (var key = root.OpenSubKey(@"SOFTWARE\Khronos\Vulkan\ImplicitLayers"))
            if (key != null) entries.AddRange(key.GetValueNames().OrderBy(name => name).Select(name => view + "|" + name + "|" + key.GetValue(name)));
        return string.Join("\n", entries);
    }
    private static void StopDiscovery(FpsLimiterService service) => ((System.Windows.Forms.Timer)typeof(FpsLimiterService).GetField("_discovery", Private).GetValue(service)).Stop();

    private static async Task RepeatedEnable()
    {
        using (var service = new FpsLimiterService(new FpsLimiterConfiguration()))
        using (var target = Launch(Target))
        {
            try
            {
                await Task.Delay(700); await service.SetEnabledAsync(true); StopDiscovery(service);
                await service.AttachTargetAsync(target.Id, target.ProcessName, Target, 45);
                using (var mapping = MemoryMappedFile.OpenExisting("Local\\TrayFPS.Config.Global"))
                using (var view = mapping.CreateViewAccessor())
                {
                    Check(view.ReadInt32(16) == target.Id && view.ReadDouble(24) == 45, "initial global target and per-game cap");
                    await service.SetEnabledAsync(true); StopDiscovery(service);
                    Check(view.ReadInt32(16) == target.Id && view.ReadDouble(24) == 45, "repeated ON preserves active Vulkan target and profile cap");
                }
            }
            finally { await service.StopAsync(); if (!target.HasExited) { target.Kill(); target.WaitForExit(); } }
        }
    }

    private sealed class IsolatedForm : MainForm
    {
        protected override void OnLoad(EventArgs e) { }
        internal void Recreate() { RecreateHandle(); }
    }
    private static async Task DpiRestart()
    {
        IntPtr context = SetThreadDpiAwarenessContext((IntPtr)(-4));
        Screen monitor = null;
        try
        {
            foreach (var screen in Screen.AllScreens)
            {
                GetDpiForMonitor(MonitorFromPoint(screen.Bounds.Location, 2), 0, out uint x, out uint y);
                Console.WriteLine("MONITOR=" + screen.DeviceName + " DPI=" + x);
                if (x > 96) monitor = screen;
            }
        }
        finally { SetThreadDpiAwarenessContext(context); }
        Check(monitor != null, "real monitor with higher scaling available");
        WriteHost(Config(true, false, 50));
        using (var first = Launch(Path.Combine(Repos, "Tray-Temps", "dist", "TrayTemps.exe"), "-dpi-restart -defer-pawnio-prompt"))
        {
            try
            {
                await Task.Delay(6000); first.Refresh();
                uint initialDpi = GetDpiForWindow(first.MainWindowHandle);
                context = SetThreadDpiAwarenessContext((IntPtr)(-4));
                try
                {
                    monitor = Screen.AllScreens.First(screen => { GetDpiForMonitor(MonitorFromPoint(screen.Bounds.Location, 2), 0, out uint x, out uint y); return x != initialDpi; });
                }
                finally { SetThreadDpiAwarenessContext(context); }
                Console.WriteLine("DPI_FROM=" + initialDpi + " TO_MONITOR=" + monitor.DeviceName);
                var watch = Stopwatch.StartNew();
                context = SetThreadDpiAwarenessContext((IntPtr)(-4));
                try { Check(SetWindowPos(first.MainWindowHandle, IntPtr.Zero, monitor.Bounds.X + 70, monitor.Bounds.Y + 70, 0, 0, 0x15), "moved real window between monitors with different DPI"); }
                finally { SetThreadDpiAwarenessContext(context); }
                while (!first.HasExited && watch.ElapsedMilliseconds < 25000) await Task.Delay(100);
                Check(first.HasExited, "real WM_DPICHANGED restart exits original host");
                Console.WriteLine("DPI_MOVE_TO_OLD_EXIT_MS=" + watch.ElapsedMilliseconds);
                await Task.Delay(5000);
                var replacements = Process.GetProcessesByName("TrayTemps");
                Check(replacements.Length == 1, "one replacement survives real DPI handover");
                using (var replacement = replacements.Single())
                {
                    try
                    {
                        Console.WriteLine("DPI_REPLACEMENT_START_UTC=" + replacement.StartTime.ToUniversalTime().ToString("O"));
                        using (var game = await MediumTarget(Target, "", "dpi-game"))
                        { try { await Task.Delay(600); await WaitForCap(game, 50); Check(ReadCap(game.Id) == 50, "DPI replacement retains settings and reacquires limiter ownership"); await Task.Delay(2500); Console.WriteLine("DPI_ACTUAL_INTERVAL=" + LiveLines(Path.Combine(_output, "dpi-game.log")).Last()); } finally { if (!game.HasExited) { game.Kill(); game.WaitForExit(); } } }
                        var saved = JsonSerializer.Deserialize<AppSettings>(File.ReadAllText(HostSettings));
                        Check(saved.FpsLimiter.Enabled && saved.FpsLimiter.GameLimits.Single().FramesPerSecond == 50, "DPI restart preserves saved profile and enabled state");
                        Console.WriteLine("DPI_OBSERVATION_READY PID=" + replacement.Id);
                        var manual = Stopwatch.StartNew(); while (!File.Exists(Path.Combine(_output, "tray-restored.ready")) && manual.ElapsedMilliseconds < 180000) await Task.Delay(200);
                        Check(File.Exists(Path.Combine(_output, "tray-restored.ready")), "user completed real minimize-to-tray and restore");
                        if (replacement.HasExited) { await Task.Delay(2000); await FinishDpi(); }
                        else { replacement.Refresh(); Check(replacement.MainWindowHandle != IntPtr.Zero, "DPI replacement window is visible after actual tray restore"); }
                    }
                    finally { await Close(replacement); }
                }
            }
            finally { if (!first.HasExited) await Close(first); }
        }
    }

    private static async Task Hardware()
    {
        WriteHost(Config(true, true));
        using (var host = Launch(Path.Combine(Repos, "Tray-Temps", "dist", "TrayTemps.exe"), "-dpi-restart -defer-pawnio-prompt"))
        {
        await Task.Delay(30000);
        using (var target = Launch(Target))
        {
            try
            {
                await WaitForCap(target, 60); await Focus(target); await Task.Delay(6000);
                using (var log = Process.Start(new ProcessStartInfo("logman", "query -ets") { UseShellExecute = false, RedirectStandardOutput = true }))
                { string sessions = log.StandardOutput.ReadToEnd(); File.WriteAllText(Path.Combine(_output, "etw-sessions.txt"), sessions); Console.WriteLine("HOST_ETW_SESSION=" + sessions.Contains("TrayTemps.FPS.")); }
                var bounds = SystemInformation.VirtualScreen;
                using (var bitmap = new Bitmap(bounds.Width, bounds.Height))
                using (var graphics = Graphics.FromImage(bitmap))
                { graphics.CopyFromScreen(bounds.Location, Point.Empty, bounds.Size); bitmap.Save(Path.Combine(_output, "real-host-osd.png")); }
                Check(ReadCap(target.Id) == 60, "real elevated host enforces cap with OSD enabled");
                Console.WriteLine("HARDWARE_OBSERVATION_READY");
                await Task.Delay(15000);
            }
            finally { if (!target.HasExited) { target.Kill(); target.WaitForExit(); } var watch = Stopwatch.StartNew(); await Close(host); Console.WriteLine("HARDWARE_HOST_CLOSE_MS=" + watch.ElapsedMilliseconds); }
        }
        }
    }

    private static async Task InteractiveRestart(bool cancelOnly = false)
    {
        WriteHost(Config(true, true));
        string exe = Path.Combine(Repos, "Tray-Temps", "dist", "TrayTemps.exe");
        string script = Path.Combine(_output, "launch-medium.ps1");
        File.WriteAllText(script, "$shell = New-Object -ComObject Shell.Application\n$window = 0\n$desktop = $shell.Windows().FindWindowSW(0,0,8,[ref]$window,1)\n$desktop.Document.Application.ShellExecute('" + exe + "','-dpi-restart -defer-pawnio-prompt','','open',1)\n");
        using (var launcher = Launch("C:\\Windows\\System32\\WindowsPowerShell\\v1.0\\powershell.exe", "-NoProfile -ExecutionPolicy Bypass -File \"" + script + "\"")) await Task.Run(() => launcher.WaitForExit());
        await Task.Delay(6000);
        var instances = Process.GetProcessesByName("TrayTemps");
        Check(instances.Length == 1, "Explorer launched actual host");
        using (var first = instances.Single())
        {
            Check(!Elevated(first), "original actual host has a non-elevated token");
            Console.WriteLine("INTERACTIVE_HOST_PID=" + first.Id + " ready; perform cancellation then acceptance via Autostart UI");
            try
            {
                if (cancelOnly)
                {
                    await Task.Delay(90000);
                    Check(!first.HasExited, "cancelled UAC leaves original host alive");
                    File.WriteAllText(script, File.ReadAllText(script).Replace(exe, Target).Replace("-dpi-restart -defer-pawnio-prompt", ""));
                    using (var launcher = Launch("C:\\Windows\\System32\\WindowsPowerShell\\v1.0\\powershell.exe", "-NoProfile -ExecutionPolicy Bypass -File \"" + script + "\"")) await Task.Run(() => launcher.WaitForExit());
                    await Task.Delay(500);
                    using (var game = Process.GetProcessesByName("D3D11GraphicsTest").Single())
                    {
                        try
                        {
                            Check(!Elevated(game), "cancel recovery target has matching non-elevated token"); await WaitForCap(game, 60); Check(ReadCap(game.Id) == 60, "cancelled restart restores real limiting");
                            string layers = Layers();
                            using (var elevatedGame = Launch(Target))
                            {
                                try { Check(Elevated(elevatedGame), "mismatched candidate has elevated token"); await Focus(elevatedGame); await Task.Delay(4000); Check(!first.HasExited && ReadCap(game.Id) == 60 && layers == Layers(), "shipping non-elevated host skips elevated candidate and retains healthy cap"); }
                                finally { if (!elevatedGame.HasExited) { elevatedGame.Kill(); elevatedGame.WaitForExit(); } }
                            }
                        }
                        finally { if (!game.HasExited) { game.Kill(); game.WaitForExit(); } }
                    }
                    return;
                }
                var watch = Stopwatch.StartNew();
                while (!first.HasExited && watch.ElapsedMilliseconds < 240000) await Task.Delay(250);
                Check(first.HasExited, "original host exits after accepted elevated restart");
                Console.WriteLine("OLD_EXIT_UTC=" + DateTime.UtcNow.ToString("O"));
                await Task.Delay(5000);
                var replacements = Process.GetProcessesByName("TrayTemps");
                Check(replacements.Length == 1, "single replacement survives actual restart");
                using (var replacement = replacements.Single())
                {
                    Check(Elevated(replacement), "replacement actual host has elevated token");
                    Console.WriteLine("REPLACEMENT_PID=" + replacement.Id + " START_UTC=" + replacement.StartTime.ToUniversalTime().ToString("O"));
                    using (var lease = new Mutex(false, "Local\\TrayFPS.Instance", out bool created)) Check(!created, "replacement owns limiter lease");
                    var game = Launch(Target);
                    try { await WaitForCap(game, 60); Check(ReadCap(game.Id) == 60, "replacement attaches and limits real target"); await Task.Delay(5000); }
                    finally { if (!game.HasExited) { game.Kill(); game.WaitForExit(); } game.Dispose(); }
                    var close = Stopwatch.StartNew(); await Close(replacement); Console.WriteLine("REPLACEMENT_NORMAL_CLOSE_MS=" + close.ElapsedMilliseconds);
                }
            }
            finally { if (!first.HasExited) await Close(first); }
        }
    }

    private static async Task ExitDuringApply()
    {
        using (var service = new FpsLimiterService(new FpsLimiterConfiguration()))
        using (var target = Launch(Target))
        {
            try
            {
                await service.SetEnabledAsync(true); StopDiscovery(service);
                bool observed = false;
                var kill = Task.Run(() =>
                {
                    var watch = Stopwatch.StartNew();
                    while (!target.HasExited && watch.ElapsedMilliseconds < 5000)
                    {
                        try { using (var map = MemoryMappedFile.OpenExisting("Local\\TrayFPS.Config." + target.Id)) { observed = true; target.Kill(); target.WaitForExit(); return; } }
                        catch (FileNotFoundException) { Thread.Yield(); }
                    }
                });
                await service.AttachTargetAsync(target.Id, target.ProcessName, Target, 45); await kill;
                Check(observed, "candidate killed after Apply created its target mapping");
                Check(service.Enabled && service.LastError == null && (int)typeof(FpsLimiterService).GetField("_attachedPid", Private).GetValue(service) == 0,
                    "exit inside Apply restores waiting session");
                using (var next = Launch(Target))
                {
                    try { await Task.Delay(600); await service.AttachTargetAsync(next.Id, next.ProcessName, Target, 50); Console.WriteLine("POST_APPLY_STATUS=" + service.StatusText + " ERROR=" + service.LastError); Check(service.LastError == null && ReadCap(next.Id) == 50, "next game attaches after exit inside Apply"); }
                    finally { if (!next.HasExited) { next.Kill(); next.WaitForExit(); } }
                }
            }
            finally { await service.StopAsync(); if (!target.HasExited) { target.Kill(); target.WaitForExit(); } }
        }
    }

    private static async Task PreflightExit()
    {
        string native = FpsLimiterPayloads.ExtractNative(); int calls = 0;
        using (var entered = new ManualResetEventSlim())
        using (var proceed = new ManualResetEventSlim())
        using (var service = new FpsLimiterService(new FpsLimiterConfiguration(), () =>
        { if (Interlocked.Increment(ref calls) == 3) { entered.Set(); proceed.Wait(); } return native; }))
        using (var first = Launch(Target))
        using (var candidate = Launch(Target))
        {
            try
            {
                await service.SetEnabledAsync(true); StopDiscovery(service);
                await service.AttachTargetAsync(first.Id, first.ProcessName, Target, 45);
                string layers = Layers(), status = service.StatusText;
                Task attach = service.AttachTargetAsync(candidate.Id, candidate.ProcessName, Target, 60);
                Check(await Task.Run(() => entered.Wait(5000)), "candidate paused after architecture query, before Apply");
                candidate.Kill(); candidate.WaitForExit(); proceed.Set(); await attach;
                Check(service.Enabled && service.LastError == null && ReadCap(first.Id) == 45 && Layers() == layers && service.StatusText == status,
                    "preflight exit retains healthy target cap, status and Vulkan registrations");
                Check((int)typeof(FpsLimiterService).GetField("_attachedPid", Private).GetValue(service) == first.Id, "preflight exit preserves target identity");
                await service.AttachTargetAsync(4, "System", "", 60);
                Check(service.Enabled && service.LastError is System.ComponentModel.Win32Exception && ReadCap(first.Id) == 45 && Layers() == layers,
                    "real protected-process preflight access denial preserves healthy session");
                using (var next = Launch(Target))
                {
                    try { await Task.Delay(600); await service.AttachTargetAsync(next.Id, next.ProcessName, Target, 50); Console.WriteLine("NEXT_CANDIDATE_STATUS=" + service.StatusText + " ERROR=" + service.LastError); Check(service.LastError == null && ReadCap(next.Id) == 50, "next candidate attaches after rejection"); }
                    finally { if (!next.HasExited) { next.Kill(); next.WaitForExit(); } }
                }
            }
            finally { proceed.Set(); await service.StopAsync(); foreach (var item in new[] { first, candidate }) if (!item.HasExited) { item.Kill(); item.WaitForExit(); } }
        }
    }

    private static async Task ExitingTarget()
    {
        string native = FpsLimiterPayloads.ExtractNative(); int calls = 0;
        using (var entered = new ManualResetEventSlim())
        using (var proceed = new ManualResetEventSlim())
        using (var service = new FpsLimiterService(new FpsLimiterConfiguration(), () => { if (Interlocked.Increment(ref calls) == 2) { entered.Set(); proceed.Wait(); } return native; }))
        using (var target = Launch(Target))
        {
            try
            {
                await Task.Delay(600); await service.SetEnabledAsync(true); StopDiscovery(service);
                Task attach = service.AttachTargetAsync(target.Id, target.ProcessName, Target, 45);
                Check(await Task.Run(() => entered.Wait(5000)), "attachment blocked in controlled workload");
                target.Kill(); target.WaitForExit(); proceed.Set(); await attach;
                Check(service.Enabled && service.LastError == null && !service.IsBusy, "game exit during attachment recovers to waiting without disabling preference");
                using (var next = Launch(Target))
                {
                    try { await Task.Delay(500); await service.AttachTargetAsync(next.Id, next.ProcessName, Target, 45); Check(service.Enabled && service.LastError == null && ReadCap(next.Id) == 45, "replacement game attaches after prior game exited during attachment"); }
                    finally { if (!next.HasExited) { next.Kill(); next.WaitForExit(); } }
                }
            }
            finally { proceed.Set(); await service.StopAsync(); if (!target.HasExited) { target.Kill(); target.WaitForExit(); } }
        }
    }
    private static async Task HotkeysAndDpi()
    {
        using (var preferences = new Preferences(HostSettings))
        using (var owner = new IsolatedForm())
        {
            owner.Show(); Call(owner, "SetDefaultControlValues"); Set(owner, "_settingsLoaded", true); Set(owner, "_fpsLimiterRuntimeReady", true);
            var config = (FpsLimiterConfiguration)Get(owner, "_fpsLimiterConfiguration"); config.ToggleHotkeyEnabled = true;
            Call(owner, "ApplyFpsLimiterHotkey"); Check((bool)Get(owner, "_fpsLimiterHotkeyRegistered"), "FPS shortcut first registration");
            owner.Recreate(); Check((bool)Get(owner, "_fpsLimiterHotkeyRegistered"), "FPS shortcut survives HWND recreation");
            var osd = (OsdConfiguration)Get(owner, "_osdConfiguration"); osd.HotkeyEnabled = true; osd.HotkeyModifiers = 6; osd.HotkeyKey = (int)Keys.F12;
            Check((bool)Call(owner, "ApplyOsdHotkeyRegistration", false), "OSD takes shortcut priority when saved later");
            Check(!config.ToggleHotkeyEnabled && !(bool)Get(owner, "_fpsLimiterHotkeyRegistered"), "OSD binding clears only conflicting FPS preference");
            owner.Recreate(); Check((bool)Get(owner, "_osdHotkeyRegistered") && !owner.CanRegisterFpsLimiterHotkey(), "OSD shortcut preserved after HWND recreation and reverse conflict");
            foreach (float scale in new[] { 1f, 1.5f, 2f })
            using (var dialog = new FpsLimiterSettingsDialog(owner, config, null))
            {
                dialog.Scale(new SizeF(scale, scale)); dialog.Show(owner); await Task.Delay(50);
                dialog.Size = dialog.MinimumSize;
                var save = dialog.Controls.Find("save", true).Single(); var editor = dialog.Controls.Find("editor", true).Single(); var cap = dialog.Controls.Find("selectedCap", true).Single();
                Check(save.Right <= save.Parent.ClientSize.Width && cap.Right <= editor.ClientSize.Width && cap.Bottom <= editor.ClientSize.Height, "dialog controls fit minimum size at scale " + scale);
                var body = dialog.Controls.Find("body", true).Single(); var gamesPanel = dialog.Controls.Find("gamesPanel", true).Single();
                var gamesHeader = dialog.Controls.Find("gamesHeader", true).Single(); var games = (ListView)dialog.Controls.Find("games", true).Single();
                Check(gamesPanel.Width >= body.ClientSize.Width * 0.9f && gamesHeader.Width <= gamesPanel.ClientSize.Width &&
                    gamesHeader.Height < gamesPanel.ClientSize.Height && gamesHeader.Controls.Count == 3 &&
                    games.Columns.Cast<ColumnHeader>().Sum(column => column.Width) <= games.ClientSize.Width,
                    "profile header remains visible in the resizable list at scale " + scale);
                using (var bitmap = new Bitmap(dialog.Width, dialog.Height)) { dialog.DrawToBitmap(bitmap, dialog.ClientRectangle); bitmap.Save(Path.Combine(_output, "dialog-scale-" + scale + ".png")); }
                dialog.Close();
            }
            Set(owner, "_settingsLoaded", false); owner.Close();
        }
    }

    private static async Task OsdShortcutCapture()
    {
        using (var preferences = new Preferences(HostSettings))
        using (var owner = new IsolatedForm())
        {
            owner.Show(); Call(owner, "SetDefaultControlValues"); Set(owner, "_settingsLoaded", true); Set(owner, "_fpsLimiterRuntimeReady", true);
            var config = (FpsLimiterConfiguration)Get(owner, "_fpsLimiterConfiguration"); config.ToggleHotkeyEnabled = true; Call(owner, "ApplyFpsLimiterHotkey");
            using (var dialog = new OsdSettingsDialog(owner, (OsdConfiguration)Get(owner,"_osdConfiguration")))
            {
                Exception failure = null; bool captured = false;
                Set(owner,"_osdSettingsDialog",dialog);
                dialog.Shown += async (sender, args) =>
                {
                    try
                    {
                        ((CheckBox)dialog.Controls.Find("hotkeyEnabled",true).Single()).Checked = true;
                        var input = (TextBox)dialog.Controls.Find("hotkeyValue",true).Single(); input.Focus(); await Task.Delay(100);
                        Check(input.Focused,"modal OSD shortcut editor has keyboard focus");
                        PostMessage(owner.Handle,0x312,(IntPtr)0x5455,IntPtr.Zero); await Task.Delay(200);
                        captured = !config.Enabled && input.Text.Contains("F12");
                    }
                    catch (Exception error) { failure = error; }
                    finally { dialog.DialogResult = DialogResult.Cancel; dialog.Close(); }
                };
                dialog.ShowDialog(owner);
                var service = (FpsLimiterService)Get(owner,"_fpsLimiterService"); if(service != null) await service.StopAsync();
                if(failure != null) throw failure;
                Check(captured,"capturing FPS shortcut in modal OSD draft does not toggle live limiter");
                Set(owner,"_osdSettingsDialog",null);
            }
            Set(owner,"_settingsLoaded",false); owner.Close();
        }
    }

    private static Task PayloadRepair()
    {
        string path = Path.Combine(_output, "payload-repair", "engine.dll");
        var extract = typeof(FpsLimiterPayloads).GetMethod("Extract", BindingFlags.Static | BindingFlags.NonPublic);
        Action run = () => extract.Invoke(null, new object[] { "TrayFPS.Payload.x64.dll", path });
        run(); byte[] original = File.ReadAllBytes(path); DateTime modified = File.GetLastWriteTimeUtc(path);
        run(); Check(File.GetLastWriteTimeUtc(path) == modified, "intact native payload reused without rewriting");
        File.WriteAllBytes(path, new byte[] { 1, 2, 3 }); run();
        Check(File.ReadAllBytes(path).SequenceEqual(original), "corrupt native payload repairs from embedded resource");
        File.WriteAllBytes(path, new byte[] { 3, 2, 1 }); bool refused = false;
        using (var locked = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            try { run(); } catch (TargetInvocationException error) when (error.InnerException is IOException) { refused = true; }
        Check(refused, "locked corrupt payload fails safely");
        Check(Directory.GetFiles(Path.GetDirectoryName(path), "*.tmp.*").Length == 0, "failed payload repair removes its temporary file");
        return Task.CompletedTask;
    }

    private static async Task ModalSettings()
    {
        using (var preferences = new Preferences(HostSettings))
        using (var owner = new IsolatedForm())
        using (var service = new FpsLimiterService(new FpsLimiterConfiguration()))
        using (var target = Launch(Target))
        {
            try
            {
                owner.Show(); Call(owner, "SetDefaultControlValues");
                var config = Config(true, false, 45).FpsLimiter;
                Set(owner, "_fpsLimiterConfiguration", config); Set(owner, "_fpsLimiterService", service); Set(owner, "_fpsLimiterRuntimeReady", true); Set(owner, "_settingsLoaded", true);
                WriteHost(Config(true, false, 45));
                await Task.Delay(600); await service.SetEnabledAsync(true); StopDiscovery(service); await service.ApplyConfigurationAsync(config); await service.AttachTargetAsync(target.Id, target.ProcessName, Target, 45);
                foreach (string action in new[] { "cancel", "close", "save" })
                {
                    byte[] saved = File.ReadAllBytes(HostSettings);
                    owner.BeginInvoke((Action)(() =>
                    {
                        var dialog = (FpsLimiterSettingsDialog)Get(owner, "_fpsLimiterDialog");
                        var games = (ListView)dialog.Controls.Find("games", true).Single(); games.Items[0].Selected = true;
                        ((NumericUpDown)dialog.Controls.Find("selectedCap", true).Single()).Value = 33;
                        if (action == "close") dialog.Controls.Find("close", true).OfType<Button>().Single().PerformClick();
                        else dialog.Controls.Find(action, true).OfType<Button>().Single().PerformClick();
                    }));
                    Call(owner, "FpsLimiterSettings_Click", owner, EventArgs.Empty);
                    while (service.IsBusy) await Task.Delay(20);
                    await Task.Delay(50);
                    if (action != "save") Check(File.ReadAllBytes(HostSettings).SequenceEqual(saved) && ReadCap(target.Id) == 45, "dialog " + action + " preserves persisted and live profile cap");
                    else Check(ReadCap(target.Id) == 33 && JsonSerializer.Deserialize<AppSettings>(File.ReadAllText(HostSettings)).FpsLimiter.GameLimits[0].FramesPerSecond == 33, "dialog Save commits and applies profile cap");
                }
            }
            finally { Set(owner, "_settingsLoaded", false); await service.StopAsync(); if (!target.HasExited) { target.Kill(); target.WaitForExit(); } owner.Close(); }
        }
    }

    private static async Task OsdIndependence()
    {
        using (var preferences = new Preferences(HostSettings))
        using (var owner = new IsolatedForm())
        {
            owner.Show(); Call(owner, "SetDefaultControlValues"); Set(owner, "_settingsLoaded", true); Set(owner, "_fpsLimiterRuntimeReady", true);
            var osd = (OsdConfiguration)Get(owner, "_osdConfiguration"); osd.ShowFps = true;
            foreach (bool limiter in new[] { false, true })
            foreach (bool display in new[] { false, true })
            {
                await (Task)Call(owner, "SetFpsLimiterEnabledAsync", limiter);
                var service = (FpsLimiterService)Get(owner, "_fpsLimiterService"); if (service != null) StopDiscovery(service);
                ((CheckBox)Get(owner, "osdEnable")).Checked = display;
                Call(owner, "UpdateTemperatures", (float?)50, (float?)60); Call(owner, "UpdateOsd", (float?)50, (float?)60);
                string before = ((Label)Get(owner, "cpuTempCur")).Text;
                Call(owner, "UpdateTemperatures", (float?)51, (float?)61); Call(owner, "UpdateOsd", (float?)51, (float?)61); Call(owner, "FpsTimer_Tick", owner, EventArgs.Empty);
                Check(before != ((Label)Get(owner, "cpuTempCur")).Text && osd.Enabled == display && (service?.Enabled ?? false) == limiter, "temperature and passive OSD update remain independent: OSD=" + display + " limiter=" + limiter);
            }
            Console.WriteLine("ETW_MONITOR_AVAILABLE=" + (Get(owner, "_fpsMonitor") != null));
            Set(owner, "_settingsLoaded", false); owner.Close();
            var last = (FpsLimiterService)Get(owner, "_fpsLimiterService"); if (last != null) await last.StopAsync();
        }
    }

    private static async Task SlowRestart()
    {
        string native = FpsLimiterPayloads.ExtractNative();
        using (var entered = new ManualResetEventSlim())
        using (var proceed = new ManualResetEventSlim())
        using (var owner = new IsolatedForm())
        {
            owner.Show(); Set(owner, "_fpsLimiterRuntimeReady", true);
            var service = new FpsLimiterService(new FpsLimiterConfiguration(), () => { entered.Set(); proceed.Wait(); return native; }); Set(owner, "_fpsLimiterService", service);
            Task enable = service.SetEnabledAsync(true); Check(await Task.Run(() => entered.Wait(5000)), "slow restart worker entered");
            var controller = (LimiterController)typeof(FpsLimiterService).GetField("_controller", Private).GetValue(service); int stops = 0;
            controller.StatusChanged += (s, e) => { if (controller.Status == LimiterStatus.Disabled) Interlocked.Increment(ref stops); };
            int ticks = 0;
            using (var timer = new System.Windows.Forms.Timer { Interval = 50 })
            {
                timer.Tick += (s,e) => ++ticks; timer.Start();
                try
                {
                    Task<bool> prepare = (Task<bool>)Call(owner, "PrepareFpsLimiterForRestartAsync"); await Task.Delay(17000);
                    Check(!prepare.IsCompleted && ticks > 100, "UI responsive while work exceeds replacement's 15-second retry window before launch");
                    proceed.Set(); Check(await prepare, "slow restart preparation cleans worker before replacement can launch"); await enable;
                    Check(stops == 1 && ReferenceEquals(service.StopAsync(), service.StopAsync()), "normal shutdown is idempotent and disposes controller once");
                    using (var lease = new Mutex(false, "Local\\TrayFPS.Instance", out bool created)) Check(created, "replacement engine ownership immediately available after long cleanup");
                    Call(owner, "ResumeFpsLimiterAfterCancelledRestart"); Check(((CheckBox)Get(owner, "fpsLimiterEnable")).Enabled, "cancelled restart restores usable controls");
                }
                finally { proceed.Set(); await service.StopAsync(); owner.Close(); }
            }
        }
    }

    private static void DesignTimeGuards()
    {
        string layers = Layers();
        var context = System.ComponentModel.LicenseManager.CurrentContext;
        try
        {
            System.ComponentModel.LicenseManager.CurrentContext = new System.ComponentModel.Design.DesigntimeLicenseContext();
            using (var form = new MainForm())
            using (var dialog = new FpsLimiterSettingsDialog())
            using (var noticeDialog = new FpsLimiterNoticesDialog())
            using (var osdDialog = new OsdSettingsDialog())
            {
                form.CreateControl(); dialog.CreateControl(); Call(form,"InitializeFpsLimiterRuntime");
                Check(Get(form,"_fpsLimiterService") == null && !(bool)Get(form,"_fpsLimiterRuntimeReady"), "Designer constructors and runtime guard create no limiter service");
                Check(dialog.AutoScaleMode == osdDialog.AutoScaleMode && dialog.Font.FontFamily.Name == osdDialog.Font.FontFamily.Name && dialog.Font.Size == osdDialog.Font.Size && dialog.BackColor == osdDialog.BackColor,
                    "FPS and OSD dialogs share Designer DPI mode, font, size, and dark surface");
                Check(dialog.ClientSize == new Size(680, 550) && dialog.MinimumSize == new Size(650, 500), "FPS dialog uses compact usable Designer dimensions");
                var save = (Button)typeof(FpsLimiterSettingsDialog).GetField("save", Private).GetValue(dialog);
                var cancel = (Button)typeof(FpsLimiterSettingsDialog).GetField("cancel", Private).GetValue(dialog);
                var close = (Button)typeof(FpsLimiterSettingsDialog).GetField("close", Private).GetValue(dialog);
                Check(ReferenceEquals(dialog.AcceptButton, save) && ReferenceEquals(dialog.CancelButton, cancel) && save.TabIndex == 0 && cancel.TabIndex == 1,
                    "FPS dialog keeps standard Save/Cancel and tab order");
                Check(close.Text == "✖" && close.Font.Size == 9.75F && Equals(close.Tag, "TrayTemps.Font.ExtraBold"),
                    "FPS dialog close glyph uses the other dialogs' bold symbol sizing");
                Check(noticeDialog.AutoScaleMode == AutoScaleMode.Dpi && noticeDialog.ClientSize == new Size(624, 458) &&
                    ((TextBox)typeof(FpsLimiterNoticesDialog).GetField("noticeText", Private).GetValue(noticeDialog)).ScrollBars == ScrollBars.Vertical,
                    "third-party notices dialog keeps its compact Designer layout and vertical scrollbar");
            }
        }
        finally { System.ComponentModel.LicenseManager.CurrentContext = context; }
        Check(Layers() == layers,"Designer construction and disposal preserve Vulkan registration");
    }

    private static async Task OwnershipOrders()
    {
        WriteStandalone(45);
        Process standalone = null, target = null;
        try
        {
            standalone = Launch(Standalone); await Task.Delay(1200); target = Launch(Target); await WaitForCap(target, 45);
            Check(ReadCap(target.Id) == 45, "standalone-first active cap"); string layers = Layers();
            using (var denied = new FpsLimiterService(new FpsLimiterConfiguration()))
            {
                await denied.SetEnabledAsync(true); Check(!denied.Enabled && denied.LastError != null, "host enable rejected while standalone owns engine"); await denied.StopAsync();
            }
            Check(ReadCap(target.Id) == 45 && Layers() == layers, "failed enable/disposal preserves standalone mapping and registrations");
        }
        finally { if (target != null && !target.HasExited) { target.Kill(); target.WaitForExit(); } if (standalone != null) await Close(standalone); }

        using (var service = new FpsLimiterService(new FpsLimiterConfiguration()))
        using (var game = Launch(Target))
        {
            try
            {
                await Task.Delay(700); await service.SetEnabledAsync(true); StopDiscovery(service); await service.AttachTargetAsync(game.Id, game.ProcessName, Target, 50);
                string layers = Layers();
                using (var second = Launch(Standalone)) { await Task.Delay(1000); Check(second.HasExited, "standalone second instance exits while host owns engine"); }
                Check(ReadCap(game.Id) == 50 && Layers() == layers, "host-first cap and Vulkan registrations survive standalone launch");
            }
            finally { await service.StopAsync(); if (!game.HasExited) { game.Kill(); game.WaitForExit(); } }
        }

        string host = Path.Combine(Repos, "Tray-Temps", "dist", "TrayTemps.exe");
        WriteStandalone(45); WriteHost(Config(true, false, 60));
        using (var first = Launch(Standalone))
        using (var game = Launch(Target))
        {
            Process second = null;
            try
            {
                await Task.Delay(1200); await WaitForCap(game, 45); string layers = Layers(); Check(ReadCap(game.Id) == 45, "real standalone first owns target");
                second = Launch(host, "-dpi-restart -defer-pawnio-prompt"); await Task.Delay(3500); DismissOk(second);
                Check(ReadCap(game.Id) == 45 && Layers() == layers, "actual host failed startup enable leaves standalone intact");
                Check(!JsonSerializer.Deserialize<AppSettings>(SettingsFileHelper.ReadSettingsJson(HostSettings)).FpsLimiter.Enabled, "actual failed enable persists only host limiter off");
            }
            finally
            {
                try { if (second != null) { DismissOk(second); await Task.Delay(250); await Close(second); } }
                finally { if (!game.HasExited) { game.Kill(); game.WaitForExit(); } await Close(first); }
            }
        }
        WriteHost(Config(true, false, 50));
        using (var first = Launch(host, "-dpi-restart -defer-pawnio-prompt"))
        using (var game = Launch(Target))
        {
            try
            {
                await Task.Delay(1500); await WaitForCap(game, 50); string layers = Layers(); Check(ReadCap(game.Id) == 50, "actual host first owns target");
                foreach (string application in new[] { Standalone, host })
                using (var second = Launch(application))
                { await Task.Delay(1200); Check(second.HasExited, "second " + Path.GetFileName(application) + " exits"); Check(ReadCap(game.Id) == 50 && Layers() == layers, "active actual host mapping and registrations remain intact"); }
            }
            finally { if (!game.HasExited) { game.Kill(); game.WaitForExit(); } await Close(first); }
        }
    }

    private static async Task StartupMatrix()
    {
        string folder = Path.Combine(_output, "shipping"); Directory.CreateDirectory(folder);
        string host = Path.Combine(folder, "TrayTemps.exe"); File.Copy(Path.Combine(Repos, "Tray-Temps", "dist", "TrayTemps.exe"), host);
        Check(Directory.GetFiles(folder).Length == 1, "clean startup folder contains the Release EXE alone");
        foreach (bool limiter in new[] { false, true })
        foreach (bool osd in new[] { false, true })
        {
            WriteHost(Config(limiter, osd)); string layers = Layers();
            using (var process = Launch(host, "-dpi-restart -defer-pawnio-prompt"))
            {
                try
                {
                    await Task.Delay(5000); process.Refresh(); Check(!process.HasExited && process.MainWindowHandle != IntPtr.Zero, "real Program.Main visible startup OSD=" + osd + " limiter=" + limiter);
                    if (!limiter) Check(Layers() == layers, "limiter-off startup leaves Vulkan registration inactive");
                    else using (var mapping = MemoryMappedFile.OpenExisting("Local\\TrayFPS.Config.Global")) using (var view = mapping.CreateViewAccessor()) Check(view.ReadInt32(12) == 1, "limiter startup enables shared controller independently of OSD");
                }
                finally { await Close(process); }
            }
        }
        foreach (bool old in new[] { true, false })
        {
            var config = Config(!old, false); config.LightMode = true; if (!old) config.FpsLimiter.FramesPerSecond = 1;
            WriteHost(config);
            // Use JsonDocument to remove the complete nested section without touching other fields.
            if (old)
            {
                using (var document = JsonDocument.Parse(JsonSerializer.Serialize(config)))
                using (var stream = new MemoryStream())
                {
                    using (var writer = new Utf8JsonWriter(stream)) { writer.WriteStartObject(); foreach (var property in document.RootElement.EnumerateObject()) if (property.Name != "FpsLimiter") property.WriteTo(writer); writer.WriteEndObject(); }
                    File.WriteAllBytes(HostSettings, stream.ToArray());
                }
            }
            string layers = Layers();
            using (var process = Launch(host, "-dpi-restart -defer-pawnio-prompt")) { try { await Task.Delay(3500); Check(!process.HasExited && Layers() == layers, "actual startup safely loads " + (old ? "old" : "invalid limiter") + " settings"); } finally { await Close(process); } }
            var loaded = JsonSerializer.Deserialize<AppSettings>(File.ReadAllText(HostSettings)); Check(loaded.LightMode && !loaded.FpsLimiter.Enabled && loaded.FpsLimiter.FramesPerSecond == 60, "settings normalization preserves unrelated light theme");
        }
        var hidden = Config(false, false); hidden.StartMinimizedToTray = true; WriteHost(hidden);
        using (var process = Launch(host, "-silent -dpi-restart -defer-pawnio-prompt")) { try { await Task.Delay(4000); process.Refresh(); Check(!process.HasExited && process.MainWindowHandle == IntPtr.Zero, "actual start minimized stays alive without a visible taskbar window"); } finally { await Close(process); } }
    }

    private static async Task VulkanRestart()
    {
        string executable = Path.Combine(Repos, "Tray-Temps", "Tests", "VulkanGraphicsTest", "x64", "Release", "VulkanGraphicsTest.exe");
        using (var target = Launch(executable))
        using (var service = new FpsLimiterService(new FpsLimiterConfiguration()))
        {
            try { await Task.Delay(800); await service.SetEnabledAsync(true); StopDiscovery(service); await service.AttachTargetAsync(target.Id, target.ProcessName, executable, 60); Check(service.Enabled && service.StatusText.StartsWith("Restart required:"), "Vulkan started before preparation reports restart required without disabling limiter"); }
            finally { await service.StopAsync(); if (!target.HasExited) { target.Kill(); target.WaitForExit(); } }
        }
        using (var service = new FpsLimiterService(new FpsLimiterConfiguration()))
        using (var target = new Process { StartInfo = new ProcessStartInfo(executable) { UseShellExecute = false, RedirectStandardOutput = true } })
        {
            var lines = new List<string>(); target.OutputDataReceived += (s,e) => { if (e.Data != null) lock (lines) lines.Add(e.Data); };
            try
            {
                await service.SetEnabledAsync(true); StopDiscovery(service); target.Start(); target.BeginOutputReadLine(); await Task.Delay(700);
                await service.AttachTargetAsync(target.Id, target.ProcessName, executable, 60); await Task.Delay(2200);
                Check(service.Enabled && service.StatusText.StartsWith("Attached"), "restarted Vulkan target attaches");
                await service.SetEnabledAsync(true); StopDiscovery(service); await Task.Delay(2200);
                string last; lock (lines) last = lines.Last();
                double mean = double.Parse(Regex.Match(last,@"mean_ms=([\d.]+)").Groups[1].Value,CultureInfo.InvariantCulture);
                Console.WriteLine("REPEATED_ON_VULKAN=" + last); Check(Math.Abs(mean - 1000.0/60) < 2, "repeated ON retains actual Vulkan cap");
                Task first = service.AttachTargetAsync(target.Id, target.ProcessName, executable, 60); Task duplicate = service.AttachTargetAsync(target.Id, target.ProcessName, executable, 33); await Task.WhenAll(first, duplicate);
                Check(ReadCap(target.Id) == 60, "busy duplicate attachment cannot apply a stale cap");
                Task pause = service.SetEnabledAsync(false); Task rapidOn = service.SetEnabledAsync(true); await Task.WhenAll(pause, rapidOn); Check(!service.Enabled, "toggle while busy is ignored according to existing UI policy");
                await service.SetEnabledAsync(true); StopDiscovery(service); await service.AttachTargetAsync(target.Id, target.ProcessName, executable, 60); await Task.Delay(2200);
                lock (lines) last = lines.Last(); mean = double.Parse(Regex.Match(last,@"mean_ms=([\d.]+)").Groups[1].Value,CultureInfo.InvariantCulture);
                Check(Math.Abs(mean - 1000.0/60) < 2, "fresh ON after completed OFF reapplies actual Vulkan cap");
            }
            finally { await service.StopAsync(); if (!target.HasExited) { target.Kill(); target.WaitForExit(); } }
        }
    }

    private static async Task Compare(bool steadyOnly = false, bool pacingOnly = false)
    {
        string baseline = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Temp", "TrayTemps-integration-ebf4f486d3b840308b9f6e7f4a9347b8", "TrayTemps.exe");
        string integrated = Path.Combine(Repos, "Tray-Temps", "dist", "TrayTemps.exe");
        foreach (int run in Enumerable.Range(1, pacingOnly ? 0 : 3))
        foreach (string app in run % 2 == 1 ? new[] { baseline, integrated } : new[] { integrated, baseline })
        {
            WriteHost(Config(false, false));
            using (var process = Launch(app, "-dpi-restart -defer-pawnio-prompt"))
            {
                try
                {
                    await Task.Delay(steadyOnly ? 20000 : 6000); var cpu = process.TotalProcessorTime; var clock = Stopwatch.StartNew(); await Task.Delay(steadyOnly ? 20000 : 6000); process.Refresh();
                    Console.WriteLine((steadyOnly ? "IDLE_STEADY" : "IDLE") + " run=" + run + " app=" + (app == baseline ? "baseline" : "integrated") + " cpu_ms=" + (process.TotalProcessorTime - cpu).TotalMilliseconds.ToString("F3") + " wall_ms=" + clock.ElapsedMilliseconds);
                }
                finally { await StopBenchmark(process); }
            }
        }
        if (steadyOnly) return;
        foreach (int run in Enumerable.Range(1, 3))
        foreach (bool standalone in run % 2 == 1 ? new[] { true, false } : new[] { false, true })
        {
            if (standalone) WriteStandalone(60); else WriteHost(Config(true, false, 60));
            using (var app = Launch(standalone ? Standalone : integrated, standalone ? "" : "-dpi-restart -defer-pawnio-prompt"))
            using (var target = new Process { StartInfo = new ProcessStartInfo(Target) { UseShellExecute = false, RedirectStandardOutput = true } })
            {
                var lines = new List<Tuple<DateTime,string>>();
                target.OutputDataReceived += (s, e) => { if (e.Data != null) lock (lines) lines.Add(Tuple.Create(DateTime.UtcNow, e.Data)); };
                try
                {
                    await Task.Delay(2500); DateTime start = DateTime.UtcNow; target.Start(); target.BeginOutputReadLine(); await Focus(target);
                    await WaitForCap(target, 60); await Task.Delay(500); Check(ReadCap(target.Id) == 60, "benchmark target attached at matching 60 FPS");
                    var cpu = app.TotalProcessorTime; await Task.Delay(7000); app.Refresh(); double cpuMs = (app.TotalProcessorTime - cpu).TotalMilliseconds;
                    if (!target.HasExited) await Task.Run(() => target.WaitForExit(6000));
                    Check(target.HasExited && target.ExitCode == 0, "benchmark controlled target exits normally");
                    string[] steady; lock (lines) steady = lines.Where(item => item.Item1 > start.AddSeconds(5) && item.Item1 < start.AddSeconds(13)).Select(item => item.Item2).ToArray();
                    double Value(string line, string key) => double.Parse(Regex.Match(line, key + @"=([\d.]+)").Groups[1].Value, CultureInfo.InvariantCulture);
                    string label = standalone ? "standalone" : "integrated";
                    Console.WriteLine("PACE run=" + run + " app=" + label + " windows=" + steady.Length + " mean_ms=" + steady.Average(line => Value(line,"mean_ms")).ToString("F3") + " p95_window_avg_ms=" + steady.Average(line => Value(line,"p95_ms")).ToString("F3") + " p99_window_max_ms=" + steady.Max(line => Value(line,"p99_ms")).ToString("F3") + " jitter_p95_window_avg_ms=" + steady.Average(line => Value(line,"jitter_p95_ms")).ToString("F3") + " outliers=" + steady.Sum(line => Value(line,"outliers")) + " app_cpu_ms_7s=" + cpuMs.ToString("F3"));
                    lock (lines) File.WriteAllLines(Path.Combine(_output, "pace-" + label + "-" + run + ".txt"), lines.Select(item => item.Item2));
                }
                finally { if (!target.HasExited) { target.Kill(); target.WaitForExit(); } await StopBenchmark(app); }
            }
        }
    }

    private static async Task X86()
    {
        string executable = Path.Combine(Repos, "Tray-Temps", "Tests", "ReviewTargets", "Release", "D3D11x86.exe");
        using (var service = new FpsLimiterService(new FpsLimiterConfiguration()))
        using (var target = Launch(executable))
        {
            try
            {
                await Task.Delay(600); await service.SetEnabledAsync(true); StopDiscovery(service);
                await service.AttachTargetAsync(target.Id, target.ProcessName, executable, 60);
                Console.WriteLine("X86_ATTACHMENT enabled=" + service.Enabled + " status=" + service.StatusText);
                Check(service.LastError == null && service.Enabled && service.StatusText.StartsWith("32-bit"), "unsupported x86 attachment is rejected without disabling limiter");
                using (var supported = Launch(Target))
                {
                    try
                    {
                        await Task.Delay(500); await service.AttachTargetAsync(supported.Id,supported.ProcessName,Target,60);
                        string layers = Layers();
                        await service.AttachTargetAsync(target.Id,target.ProcessName,executable,30);
                        Check(service.Enabled && ReadCap(supported.Id) == 60 && Layers() == layers, "unsupported x86 foreground target preserves healthy x64 cap and registrations");
                    }
                    finally { if(!supported.HasExited){supported.Kill();supported.WaitForExit();} }
                }
            }
            finally { await service.StopAsync(); if (!target.HasExited) { target.Kill(); target.WaitForExit(); } }
        }
    }

    private static Task StopBenchmark(Process process)
    {
        // Benchmark termination is outside the sampled interval, symmetric for
        // baseline and integrated builds, and does not interact with exit prompts.
        if (!process.HasExited) { process.Kill(); process.WaitForExit(); }
        using (var lease = new Mutex(false,"Local\\TrayFPS.Instance",out bool created))
            if (created) using (var controller = new LimiterController()) { }
        return Task.CompletedTask;
    }
}
