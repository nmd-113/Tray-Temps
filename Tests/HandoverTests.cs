using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Automation;
using System.Windows.Forms;
using FpsLimiter.Core;
using TrayTemps;

internal static partial class ReviewTests
{
    [DllImport("kernel32.dll", SetLastError = true)] private static extern bool DebugActiveProcess(uint pid);
    [DllImport("kernel32.dll", SetLastError = true)] private static extern bool DebugActiveProcessStop(uint pid);
    [DllImport("kernel32.dll", SetLastError = true)] private static extern bool DebugSetProcessKillOnExit(bool kill);
    [DllImport("kernel32.dll", SetLastError = true)] private static extern bool WaitForDebugEvent(IntPtr debugEvent, uint timeout);
    [DllImport("kernel32.dll", SetLastError = true)] private static extern bool ContinueDebugEvent(uint pid, uint tid, uint status);
    [DllImport("user32.dll")] private static extern bool IsHungAppWindow(IntPtr window);
    [DllImport("user32.dll")] private static extern bool GetWindowRect(IntPtr window, out NativeWindowBounds bounds);
    [DllImport("user32.dll")] private static extern IntPtr SendMessage(IntPtr window, int message, IntPtr wParam, IntPtr lParam);
    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
    private struct NativeWindowBounds { internal int Left, Top, Right, Bottom; }

    private static async Task DpiRoundTrips()
    {
        Screen high = null, low = null;
        IntPtr previous = SetThreadDpiAwarenessContext((IntPtr)(-4));
        try
        {
            foreach (Screen screen in Screen.AllScreens)
            {
                GetDpiForMonitor(MonitorFromPoint(screen.Bounds.Location, 2), 0, out uint x, out uint y);
                Console.WriteLine("DPI_ROUNDTRIP_MONITOR=" + screen.DeviceName + " bounds=" + screen.Bounds + " dpi=" + x);
                if (x >= 144) high = screen;
                if (x == 96) low = screen;
            }
        }
        finally { SetThreadDpiAwarenessContext(previous); }

        Check(high != null && low != null, "real 150% and 100% monitors are available");
        string hostPath = ShippingHost;
        foreach (Size logicalSize in new[] { new Size(800, 600), new Size(1000, 700) })
        {
            var settings = Config(false, false);
            settings.WindowWidth = logicalSize.Width;
            settings.WindowHeight = logicalSize.Height;
            settings.WindowX = high.Bounds.Left + 100;
            settings.WindowY = high.Bounds.Top + 100;
            settings.ScaleFactor = 1f;
            WriteHost(settings);

            Process host = await StartRoundtripHost(hostPath, logicalSize, 1.5, "initial 150%");
            try
            {
                if (logicalSize.Width == 1000)
                    ResizeRoundtripHost(host, (int)Math.Round(logicalSize.Width * 1.5), (int)Math.Round(logicalSize.Height * 1.5));

                host = await RelaunchAtSameDpi(host, hostPath, logicalSize, 1.5, "close/relaunch at 150%");
                for (int round = 1; round <= 3; round++)
                {
                    host = await MoveAndRestart(host, low, logicalSize, 1.0, "round " + round + " at 100%");
                    host = await MoveAndRestart(host, high, logicalSize, 1.5, "round " + round + " at 150%");
                }

                host = await RelaunchAtSameDpi(host, hostPath, logicalSize, 1.5, "final close/relaunch at 150%");
                host = await MoveAndRestart(host, low, logicalSize, 1.0, "final 100%");
                host = await RelaunchAtSameDpi(host, hostPath, logicalSize, 1.0, "close/relaunch at 100%");
            }
            finally { if (host != null) { await Close(host); host.Dispose(); } }
        }
    }

    private static async Task<Process> StartRoundtripHost(string path, Size logicalSize, double scale, string label)
    {
        var host = Launch(path, "-dpi-restart -defer-pawnio-prompt");
        try
        {
            await Task.Delay(5000);
            VerifyRoundtripState(host, logicalSize, scale, label);
            return host;
        }
        catch
        {
            await Close(host);
            host.Dispose();
            throw;
        }
    }

    private static async Task<Process> RelaunchAtSameDpi(Process host, string path, Size logicalSize, double scale, string label)
    {
        await Close(host);
        int oldPid = host.Id;
        Process replacement = null;
        try
        {
            replacement = await StartRoundtripHost(path, logicalSize, scale, label);
            Check(replacement.Id != oldPid, label + " starts a fresh process");
            host.Dispose();
            return replacement;
        }
        catch
        {
            if (replacement != null) { await Close(replacement); replacement.Dispose(); }
            throw;
        }
    }

    private static async Task<Process> MoveAndRestart(Process host, Screen target, Size logicalSize, double scale, string label)
    {
        host.Refresh();
        IntPtr window = host.MainWindowHandle;
        Check(window != IntPtr.Zero, label + " source window exists");
        Console.WriteLine("DPI_ROUNDTRIP_MOVE=" + label + " pid=" + host.Id + " from=" + GetDpiForWindow(window) + " target=" + target.DeviceName + " bounds=" + target.Bounds);
        IntPtr context = SetThreadDpiAwarenessContext((IntPtr)(-4));
        try
        {
            SendMessage(window, 0x0231, IntPtr.Zero, IntPtr.Zero); // WM_ENTERSIZEMOVE, as for a title-bar drag.
            Check(SetWindowPos(window, IntPtr.Zero, target.Bounds.Left + 100, target.Bounds.Top + 100, 0, 0, 0x15), label + " move across monitors succeeds");
            Console.WriteLine("DPI_ROUNDTRIP_AFTER_MOVE=" + label + " dpi=" + GetDpiForWindow(window) + " rect=" + ReadRoundtripBounds(window));
            SendMessage(window, 0x0232, IntPtr.Zero, IntPtr.Zero); // WM_EXITSIZEMOVE.
        }
        finally { SetThreadDpiAwarenessContext(context); }

        int oldPid = host.Id;
        var wait = Stopwatch.StartNew();
        while (!host.HasExited && wait.ElapsedMilliseconds < 30000) await Task.Delay(100);
        Check(host.HasExited, label + " runs the real DPI restart handover");
        Process replacement = await FindRoundtripReplacement(oldPid, 30000);
        try
        {
            VerifyRoundtripState(replacement, logicalSize, scale, label);
            host.Dispose();
            return replacement;
        }
        catch
        {
            await Close(replacement);
            replacement.Dispose();
            throw;
        }
    }

    private static async Task<Process> FindRoundtripReplacement(int oldPid, int timeout)
    {
        var wait = Stopwatch.StartNew();
        while (wait.ElapsedMilliseconds < timeout)
        {
            foreach (Process candidate in Process.GetProcessesByName("TrayTemps"))
            {
                try
                {
                    if (candidate.Id != oldPid && string.Equals(candidate.MainModule.FileName, ShippingHost, StringComparison.OrdinalIgnoreCase) && candidate.MainWindowHandle != IntPtr.Zero)
                        return candidate;
                }
                catch { candidate.Dispose(); }
                candidate.Dispose();
            }
            await Task.Delay(100);
        }
        throw new TimeoutException("DPI replacement did not create a visible main window.");
    }

    private static void ResizeRoundtripHost(Process host, int width, int height)
    {
        host.Refresh();
        IntPtr context = SetThreadDpiAwarenessContext((IntPtr)(-4));
        try
        {
            Check(SetWindowPos(host.MainWindowHandle, IntPtr.Zero, 0, 0, width, height, 0x16), "manual resize fixture sets a known 150% form size");
        }
        finally { SetThreadDpiAwarenessContext(context); }
    }

    private static void VerifyRoundtripState(Process host, Size logicalSize, double expectedScale, string label)
    {
        host.Refresh();
        Check(!host.HasExited && host.MainWindowHandle != IntPtr.Zero, label + " host has a visible window");
        uint dpi = GetDpiForWindow(host.MainWindowHandle);
        var bounds = ReadRoundtripBounds(host.MainWindowHandle);
        double actualScale = dpi / 96d;
        var settings = System.Text.Json.JsonSerializer.Deserialize<AppSettings>(File.ReadAllText(HostSettings));
        Console.WriteLine("DPI_ROUNDTRIP_STATE=" + label + " pid=" + host.Id + " dpi=" + dpi + " physical=" + bounds.Width + "x" + bounds.Height + " saved=" + settings.WindowWidth + "x" + settings.WindowHeight + " scale=" + settings.ScaleFactor);
        Check(Math.Abs(actualScale - expectedScale) < 0.02, label + " window is on expected DPI (" + dpi + ")");
        Check(Math.Abs(bounds.Width * 96d / dpi - logicalSize.Width) <= 3 && Math.Abs(bounds.Height * 96d / dpi - logicalSize.Height) <= 3,
            label + " logical size stays " + logicalSize.Width + "x" + logicalSize.Height + "; physical=" + bounds.Width + "x" + bounds.Height);
    }

    private static Rectangle ReadRoundtripBounds(IntPtr window)
    {
        IntPtr context = SetThreadDpiAwarenessContext((IntPtr)(-4));
        try
        {
            if (!GetWindowRect(window, out NativeWindowBounds bounds)) throw new System.ComponentModel.Win32Exception();
            return Rectangle.FromLTRB(bounds.Left, bounds.Top, bounds.Right, bounds.Bottom);
        }
        finally { SetThreadDpiAwarenessContext(context); }
    }
    private static string[] LiveLines(string path)
    {
        if (!File.Exists(path)) return new string[0];
        using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        using (var reader = new StreamReader(stream)) return reader.ReadToEnd().Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
    }
    private static async Task<Process> MediumTarget(string executable, string arguments = "", string label = "medium")
    {
        string pidFile = Path.Combine(_output, label + ".pid"), log = Path.Combine(_output, label + ".log");
        string script = Path.Combine(_output, label + ".ps1");
        string Quote(string value) => "'" + value.Replace("'", "''") + "'";
        File.WriteAllText(script,
            "$info=[Diagnostics.ProcessStartInfo]::new(" + Quote(executable) + "," + Quote(arguments) + ")\n" +
            "$info.UseShellExecute=$false; $info.CreateNoWindow=$true; $info.RedirectStandardOutput=$true; $info.WorkingDirectory=" + Quote(Path.GetDirectoryName(executable)) + "\n" +
            "$p=[Diagnostics.Process]::Start($info); $p.Id | Set-Content " + Quote(pidFile) + "\n" +
            "$writer=[IO.StreamWriter]::new(" + Quote(log) + "); $writer.AutoFlush=$true\n" +
            "try { while(!$p.StandardOutput.EndOfStream){$writer.WriteLine($p.StandardOutput.ReadLine())} } finally {$writer.Dispose(); $p.Dispose()}\n");
        string broker = Path.Combine(_output, label + "-broker.ps1");
        File.WriteAllText(broker, "$shell=New-Object -ComObject Shell.Application\n$window=0\n$desktop=$shell.Windows().FindWindowSW(0,0,8,[ref]$window,1)\n$desktop.Document.Application.ShellExecute('C:\\Windows\\System32\\WindowsPowerShell\\v1.0\\powershell.exe'," + Quote("-NoProfile -ExecutionPolicy Bypass -File \"" + script + "\"") + ",'','open',0)\n");
        using (var launcher = Launch("C:\\Windows\\System32\\WindowsPowerShell\\v1.0\\powershell.exe", "-NoProfile -ExecutionPolicy Bypass -File \"" + broker + "\""))
            await Task.Run(() => launcher.WaitForExit());
        var wait = Stopwatch.StartNew(); while (!File.Exists(pidFile) && wait.ElapsedMilliseconds < 10000) await Task.Delay(50);
        var process = Process.GetProcessById(int.Parse(File.ReadAllText(pidFile).Trim()));
        Check(!Elevated(process), label + " actual token is non-elevated"); return process;
    }

    private static async Task MixedVulkan()
    {
        string executable = Path.Combine(Repos, "Tray-Temps", "Tests", "VulkanGraphicsTest", "x64", "Release", "VulkanGraphicsTest.exe");
        using (var service = new FpsLimiterService(new FpsLimiterConfiguration()))
        {
            await service.SetEnabledAsync(true); StopDiscovery(service);
            Check(Elevated(Process.GetCurrentProcess()), "test controller runs elevated");
            using (var game = await MediumTarget(executable))
            {
                try
                {
                    await Task.Delay(700); await service.AttachTargetAsync(game.Id, game.ProcessName, executable, 60);
                    Check(service.LastError == null, "elevated controller attaches normal Vulkan target");
                    await Period(60); await service.ApplyConfigurationAsync(new FpsLimiterConfiguration { FramesPerSecond = 45 }); await Period(45);
                    await service.SetEnabledAsync(false); await Period(0);
                    await service.SetEnabledAsync(true); StopDiscovery(service); await service.AttachTargetAsync(game.Id, game.ProcessName, executable, 60); await Period(60);
                    string layers = Layers();
                    using (var candidate = Launch(executable))
                    {
                        try
                        {
                            await Task.Delay(700); Check(Elevated(candidate), "unsupported Vulkan candidate runs elevated");
                            await service.AttachTargetAsync(candidate.Id, candidate.ProcessName, executable, 45);
                            Check(service.Enabled && service.LastError == null && service.StatusText.StartsWith("Elevated Vulkan games are unsupported") && ReadCap(game.Id) == 60 && Layers() == layers,
                                "unsupported Vulkan rejection reports limitation and preserves active cap/registrations");
                            Check((int)typeof(FpsLimiterService).GetField("_attachedPid", Private).GetValue(service) == game.Id, "unsupported Vulkan rejection preserves target identity");
                            await Period(60);
                        }
                        finally { if (!candidate.HasExited) { candidate.Kill(); candidate.WaitForExit(); } }
                    }
                }
                finally { await service.StopAsync(); if (!game.HasExited) { game.Kill(); game.WaitForExit(); } }
            }
        }

        async Task Period(int cap)
        {
            string log = Path.Combine(_output, "medium.log"); int count = LiveLines(log).Length;
            await Task.Delay(1700); var lines = LiveLines(log).Skip(count + 1).Where(line => line.Contains("mean_ms=")).ToArray();
            if (lines.Length == 0) lines = LiveLines(log).Skip(count).Where(line => line.Contains("mean_ms=")).ToArray();
            Check(lines.Length != 0, "Vulkan interval sample available cap=" + cap);
            string sample = lines.Last(); double mean = double.Parse(System.Text.RegularExpressions.Regex.Match(sample, @"mean_ms=([\d.]+)").Groups[1].Value, System.Globalization.CultureInfo.InvariantCulture);
            Console.WriteLine("MIXED_VULKAN cap=" + cap + " " + sample);
            Check(cap == 0 ? mean < 10 : Math.Abs(mean - 1000d / cap) < 2, "actual mixed-privilege Vulkan enforcement cap=" + cap);
        }
    }

    private static AutomationElement Element(Process process, string id)
    {
        var roots = AutomationElement.RootElement.FindAll(TreeScope.Children, new PropertyCondition(AutomationElement.ProcessIdProperty, process.Id));
        foreach (AutomationElement root in roots)
        {
            var found = root.FindFirst(TreeScope.Descendants, new OrCondition(new PropertyCondition(AutomationElement.AutomationIdProperty, id), new PropertyCondition(AutomationElement.NameProperty, id == "selectedCap" ? "Selected game FPS cap" : id)));
            if (found != null && !found.Current.IsOffscreen) return found;
        }
        throw new Exception("Missing visible UI control: " + id);
    }
    private static void Click(Process process, string id) => ((InvokePattern)Element(process, id).GetCurrentPattern(InvokePattern.Pattern)).Invoke();
    private static void Toggle(Process process, string id) => ((TogglePattern)Element(process, id).GetCurrentPattern(TogglePattern.Pattern)).Toggle();
    private static void Snapshot(string label)
    {
        var bounds = SystemInformation.VirtualScreen;
        using (var bitmap = new System.Drawing.Bitmap(bounds.Width, bounds.Height))
        using (var graphics = System.Drawing.Graphics.FromImage(bitmap))
        { graphics.CopyFromScreen(bounds.Location, System.Drawing.Point.Empty, bounds.Size); bitmap.Save(Path.Combine(_output, label + ".png")); }
    }
    private static string Unity(string api) => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads", "fps test tools", "tfm10-win64-" + api, "tfm10-win64-" + api + ".exe");
    private const string Windowed = "-screen-fullscreen 0 -screen-width 800 -screen-height 600 -show-screen-selector 0";
    private static string ShippingHost => Path.Combine(Repos, "Tray-Temps", "dist", "TrayTemps.exe");
    private static async Task StartUnity(Process process)
    {
        EnumWindows((window, ignored) =>
        {
            GetWindowThreadProcessId(window, out uint owner); var text = new System.Text.StringBuilder(128); GetWindowText(window, text, text.Capacity);
            if (owner == process.Id && text.ToString() == "TFM Configuration")
            { PostMessage(window, 0x111, (IntPtr)1, GetDlgItem(window, 1)); Console.WriteLine("UNITY_PLAY: accepted supplied target's configuration dialog"); }
            return true;
        }, IntPtr.Zero);
        await Task.Delay(2500); process.Refresh(); await Focus(process);
    }

    private static async Task HostVulkan()
    {
        string executable = Unity("vulkan"); var settings = Config(true, true);
        settings.FpsLimiter.GameLimits.Clear(); settings.FpsLimiter.GameLimits.Add(new GameLimit { ExecutablePath = executable, FramesPerSecond = 60 }); WriteHost(settings);
        using (var host = Launch(ShippingHost, "-dpi-restart -defer-pawnio-prompt"))
        {
            try
            {
                await Task.Delay(4000); Check(Elevated(host), "shipping host is elevated before normal Vulkan launch");
                using (var game = await MediumTarget(executable, Windowed, "unity-vulkan"))
                {
                    try
                    {
                        await Task.Delay(5000); await StartUnity(game); await WaitForCap(game, 60); await Task.Delay(2000); Snapshot("host-vulkan-60");
                        Click(host, "settingsBtn"); Click(host, "fpsLimiterSettings"); await Task.Delay(500);
                        var list = Element(host, "games");
                        var nodes = list.FindAll(TreeScope.Descendants, System.Windows.Automation.Condition.TrueCondition);
                        File.WriteAllLines(Path.Combine(_output, "games-uia.txt"), nodes.Cast<AutomationElement>().Select(node => node.Current.ControlType.ProgrammaticName + " " + node.Current.Name + " " + node.Current.AutomationId));
                        var item = list.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.NameProperty, Path.GetFileName(executable))) ?? throw new Exception("Profile row missing from accessibility tree");
                        ((SelectionItemPattern)item.GetCurrentPattern(SelectionItemPattern.Pattern)).Select();
                        Snapshot("host-dialog");
                        var roots = AutomationElement.RootElement.FindAll(TreeScope.Children, new PropertyCondition(AutomationElement.ProcessIdProperty, host.Id));
                        File.WriteAllLines(Path.Combine(_output, "dialog-uia.txt"), roots.Cast<AutomationElement>().SelectMany(root => root.FindAll(TreeScope.Descendants, System.Windows.Automation.Condition.TrueCondition).Cast<AutomationElement>()).Select(node => node.Current.ControlType.ProgrammaticName + "|" + node.Current.Name + "|" + node.Current.AutomationId + "|off=" + node.Current.IsOffscreen));
                        var decrement = Element(host, "editor").FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.AutomationIdProperty, "SmallDecrement"));
                        Check(decrement != null, "live profile spinner decrement accessible");
                        for (int step = 0; step < 15; step++) { ((InvokePattern)decrement.GetCurrentPattern(InvokePattern.Pattern)).Invoke(); await Task.Delay(30); }
                        Click(host, "save"); await Task.Delay(1000);
                        Check(ReadCap(game.Id) == 45, "shipping settings Save changes live normal Vulkan profile"); await Focus(game); await Task.Delay(2000); Snapshot("host-vulkan-45");
                        Toggle(host, "fpsLimiterEnable"); await Task.Delay(600); ReadCap(game.Id, false); Console.WriteLine("PASS: shipping OFF clears target enable state"); await Focus(game); await Task.Delay(1500); Snapshot("host-vulkan-off");
                        Toggle(host, "fpsLimiterEnable"); await WaitForCap(game, 45); await Task.Delay(2000); Snapshot("host-vulkan-on");
                        string layers = Layers();
                        using (var candidate = Launch(executable, Windowed))
                        {
                            try
                            {
                                await Task.Delay(5000); await StartUnity(candidate); Check(Elevated(candidate), "shipping Vulkan candidate is elevated"); await Focus(candidate); await Task.Delay(3000);
                                Click(host, "fpsLimiterSettings"); await Task.Delay(500);
                                string status = Element(host, "status").Current.Name; Console.WriteLine("SHIPPING_STATUS=" + status);
                                Check(status.StartsWith("Elevated Vulkan games are unsupported") && ReadCap(game.Id) == 45 && Layers() == layers, "shipping host rejects elevated Vulkan without disturbing active cap");
                                Click(host, "cancel");
                            }
                            finally { if (!candidate.HasExited) { candidate.Kill(); candidate.WaitForExit(); } }
                        }
                    }
                    finally { if (!game.HasExited) { game.Kill(); game.WaitForExit(); } }
                }
            }
            finally { await Close(host); }
        }
    }

    private static async Task HostVulkan60()
    {
        string executable = Path.Combine(Repos, "Tray-Temps", "Tests", "VulkanGraphicsTest", "x64", "Release", "VulkanGraphicsTest.exe");
        var settings = Config(true, false); settings.FpsLimiter.GameLimits.Clear(); settings.FpsLimiter.GameLimits.Add(new GameLimit { ExecutablePath = executable, FramesPerSecond = 60 }); WriteHost(settings);
        using (var host = Launch(ShippingHost, "-dpi-restart -defer-pawnio-prompt"))
        {
            try
            {
                await Task.Delay(4000); Check(Elevated(host), "actual shipping host runs elevated before Vulkan launch");
                using (var game = await MediumTarget(executable, "", "host-vulkan-60"))
                {
                    try
                    {
                        await Task.Delay(600); await WaitForCap(game, 60); await Task.Delay(2500);
                        string sample = LiveLines(Path.Combine(_output, "host-vulkan-60.log")).Last(); Console.WriteLine("SHIPPING_VULKAN_60=" + sample);
                        double mean = double.Parse(System.Text.RegularExpressions.Regex.Match(sample, @"mean_ms=([\d.]+)").Groups[1].Value, System.Globalization.CultureInfo.InvariantCulture);
                        Check(Math.Abs(mean - 1000d / 60) < 2, "actual shipping elevated host enforces 60 FPS in normal Vulkan target");
                    }
                    finally { if (!game.HasExited) { game.Kill(); game.WaitForExit(); } }
                }
            }
            finally { await Close(host); }
        }
    }

    private static async Task RealOsd()
    {
        string executable = Unity("dx11"); var settings = Config(false, true, 30);
        settings.FpsLimiter.GameLimits.Clear(); settings.FpsLimiter.GameLimits.Add(new GameLimit { ExecutablePath = executable, FramesPerSecond = 30 }); WriteHost(settings);
        using (var host = Launch(ShippingHost, "-dpi-restart -defer-pawnio-prompt"))
        {
            try
            {
                await Task.Delay(20000);
                using (var game = Launch(executable, Windowed))
                {
                    try
                    {
                        await Task.Delay(5000); await StartUnity(game);
                        await Task.Delay(5000); Snapshot("real-osd-only");
                        Click(host, "settingsBtn"); Toggle(host, "osdEnable"); Toggle(host, "fpsLimiterEnable"); await WaitForCap(game, 30); await Task.Delay(4000); Snapshot("real-limiter-only");
                        Toggle(host, "osdEnable"); await Focus(game); await Task.Delay(4000); Snapshot("real-both");
                    }
                    finally { if (!game.HasExited) { game.Kill(); game.WaitForExit(); } }
                }
                await Task.Delay(5000); Snapshot("real-after-game-exit");
                using (var game = Launch(executable, Windowed))
                { try { await Task.Delay(4000); await StartUnity(game); await WaitForCap(game, 30); await Task.Delay(4000); Snapshot("real-next-game"); } finally { if (!game.HasExited) { game.Kill(); game.WaitForExit(); } } }
            }
            finally { await Close(host); }
        }
    }

    private static async Task CancelCap()
    {
        WriteHost(Config(true, false, 45));
        using (var host = await MediumTarget(ShippingHost, "-dpi-restart -defer-pawnio-prompt", "cancel-host"))
        {
            try
            {
                await Task.Delay(4000); Console.WriteLine("CANCEL_READY PID=" + host.Id + " SIGNAL=" + Path.Combine(_output, "uac-cancel.ready"));
                var wait = Stopwatch.StartNew(); while (!File.Exists(Path.Combine(_output, "uac-cancel.ready")) && wait.ElapsedMilliseconds < 180000) await Task.Delay(200);
                Check(File.Exists(Path.Combine(_output, "uac-cancel.ready")) && !host.HasExited, "original host survives user-declined actual UAC restart");
                using (var game = await MediumTarget(Target, "", "cancel-game"))
                {
                    try
                    {
                        await Task.Delay(600); await WaitForCap(game, 45); await Task.Delay(2500);
                        Check(ReadCap(game.Id) == 45, "surviving host reapplies saved cap after UAC cancellation");
                        string sample = LiveLines(Path.Combine(_output, "cancel-game.log")).Last(); Console.WriteLine("CANCEL_ACTUAL_INTERVAL=" + sample);
                        double mean = double.Parse(System.Text.RegularExpressions.Regex.Match(sample, @"mean_ms=([\d.]+)").Groups[1].Value, System.Globalization.CultureInfo.InvariantCulture);
                        Check(Math.Abs(mean - 1000d / 45) < 2, "real presentation capped after UAC cancellation");
                    }
                    finally { if (!game.HasExited) { game.Kill(); game.WaitForExit(); } }
                }
            }
            finally { await Close(host); }
        }
    }

    private static async Task PendingExit()
    {
        var settings = Config(false, false, 45); settings.FpsLimiter.ToggleHotkeyEnabled = true;
        WriteHost(settings); string layers = Layers();
        using (var host = Launch(ShippingHost, "-dpi-restart -defer-pawnio-prompt"))
        using (var game = Launch(Target))
        using (var entered = new ManualResetEventSlim())
        using (var release = new ManualResetEventSlim())
        {
            Task debugger = null;
            try
            {
                await Task.Delay(4000); Click(host, "settingsBtn"); await Focus(game);
                debugger = Task.Run(() =>
                {
                    bool attached = false; IntPtr buffer = Marshal.AllocHGlobal(1024);
                    try
                    {
                        if (!DebugActiveProcess((uint)game.Id)) throw new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error());
                        attached = true; DebugSetProcessKillOnExit(false);
                        if (!WaitForDebugEvent(buffer, 5000)) throw new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error());
                        entered.Set(); release.Wait(20000);
                        ContinueDebugEvent((uint)Marshal.ReadInt32(buffer, 4), (uint)Marshal.ReadInt32(buffer, 8), 0x10002);
                        if (Marshal.ReadInt32(buffer) == 3) // CREATE_PROCESS_DEBUG_EVENT owns these three handles.
                            foreach (int offset in new[] { 16, 24, 32 }) { IntPtr handle = Marshal.ReadIntPtr(buffer, offset); if (handle != IntPtr.Zero) CloseHandle(handle); }
                    }
                    finally { if (attached) DebugActiveProcessStop((uint)game.Id); Marshal.FreeHGlobal(buffer); }
                });
                Check(await Task.Run(() => entered.Wait(6000)), "controlled game stopped at a real Windows debug event");
                host.Refresh(); PostMessage(host.MainWindowHandle, 0x312, (IntPtr)0x5455, IntPtr.Zero);
                var wait = Stopwatch.StartNew(); bool mapping = false;
                while (wait.ElapsedMilliseconds < 6000 && !mapping)
                {
                    try { using (var map = System.IO.MemoryMappedFiles.MemoryMappedFile.OpenExisting("Local\\TrayFPS.Config." + game.Id)) mapping = true; }
                    catch (FileNotFoundException) { await Task.Delay(50); }
                }
                Check(mapping, "real shipping attachment entered Apply while controlled game was stopped");
                host.Refresh(); IntPtr window = host.MainWindowHandle; var closeWatch = Stopwatch.StartNew(); Task closing = Close(host);
                await Task.Delay(1200); Check(!host.HasExited && !IsHungAppWindow(window), "real host remains responsive and defers exit during pending attachment");
                release.Set(); await debugger; await closing;
                Console.WriteLine("PENDING_EXIT_TOTAL_MS=" + closeWatch.ElapsedMilliseconds);
                using (var lease = new Mutex(false, "Local\\TrayFPS.Instance", out bool created)) Check(created, "real pending exit releases limiter ownership");
                Check(Layers() == layers, "real pending exit cleans registrations");
            }
            finally
            {
                release.Set(); if (debugger != null) await debugger;
                if (!game.HasExited) { game.Kill(); game.WaitForExit(); }
                if (!host.HasExited) await Close(host);
            }
        }
    }

    private static async Task FinishDpi()
    {
        using (var host = Process.GetProcessesByName("TrayTemps").Single())
        {
            Check(string.Equals(host.MainModule.FileName, ShippingHost, StringComparison.OrdinalIgnoreCase), "surviving DPI/tray process belongs to this fixture");
            try
            {
                host.Refresh(); Check(host.MainWindowHandle != IntPtr.Zero, "latest DPI/tray replacement has a normal visible window");
                Console.WriteLine("FINAL_DPI_PID=" + host.Id + " DPI=" + GetDpiForWindow(host.MainWindowHandle));
                using (var game = await MediumTarget(Target, "", "final-dpi-game"))
                {
                    try { await Task.Delay(600); await WaitForCap(game, 50); Check(ReadCap(game.Id) == 50, "latest tray-restored replacement retains profile and actual limiting"); await Task.Delay(2500); Console.WriteLine("FINAL_DPI_INTERVAL=" + LiveLines(Path.Combine(_output, "final-dpi-game.log")).Last()); }
                    finally { if (!game.HasExited) { game.Kill(); game.WaitForExit(); } }
                }
            }
            finally { await Close(host); }
        }
    }
}
