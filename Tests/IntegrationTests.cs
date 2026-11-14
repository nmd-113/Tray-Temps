using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using FpsLimiter.Core;
using TrayTemps;

internal static class Program
{
    private static int _result;
    [STAThread] private static int Main(string[] args)
    {
        // Load the shipping EXE first so its existing Costura resolver supplies all managed dependencies.
        var host = Assembly.LoadFrom(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TrayTemps.exe"));
        RuntimeHelpers.RunModuleConstructor(host.ManifestModule.ModuleHandle);
        return Run(args);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static int Run(string[] args)
    {
        Application.EnableVisualStyles();
        using (var runner = new Form { ShowInTaskbar = false, Opacity = 0, Size = new Size(100, 100) })
        {
            runner.Shown += async (sender, e) =>
            {
                try { if (args.Length == 1 && args[0] == "--audit-fixes") AuditFixTests.Run(); else if (args.Length > 0 && args[0] == "--review") await ReviewTests.Run(args.Skip(1).ToArray()); else await Suite.Run(args); Console.WriteLine("ALL INTEGRATION CHECKS PASSED"); }
                catch (Exception error) { Console.Error.WriteLine(error); _result = 1; }
                finally { runner.Close(); }
            };
            Application.Run(runner);
        }
        return _result;
    }
}

internal static class Suite
{
    private const BindingFlags PrivateInstance = BindingFlags.Instance | BindingFlags.NonPublic;
    private static string _evidence;
    private static int _checks;
    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)] private static extern IntPtr LoadLibraryW(string path);
    [DllImport("kernel32.dll")] private static extern bool FreeLibrary(IntPtr handle);
    [DllImport("user32.dll")] private static extern bool SetForegroundWindow(IntPtr window);
    [DllImport("user32.dll")] private static extern IntPtr GetForegroundWindow();
    [DllImport("user32.dll")] private static extern uint GetWindowThreadProcessId(IntPtr window, out uint pid);
    [DllImport("kernel32.dll")] private static extern uint GetCurrentThreadId();
    [DllImport("user32.dll")] private static extern bool AttachThreadInput(uint first, uint second, bool attach);
    [StructLayout(LayoutKind.Sequential)] private struct WindowRect { public int Left, Top, Right, Bottom; }
    [DllImport("user32.dll", SetLastError = true)] private static extern bool GetWindowRect(IntPtr window, out WindowRect bounds);
    private static void Check(bool condition, string description)
    {
        if (!condition) throw new Exception("FAILED: " + description);
        Console.WriteLine("PASS " + (++_checks) + ": " + description);
    }
    private static void CaptureWindow(Form form, string fileName)
    {
        if (!GetWindowRect(form.Handle, out WindowRect bounds)) throw new System.ComponentModel.Win32Exception();
        var rectangle = Rectangle.FromLTRB(bounds.Left, bounds.Top, bounds.Right, bounds.Bottom);
        using (var bitmap = new Bitmap(rectangle.Width, rectangle.Height))
        using (var graphics = System.Drawing.Graphics.FromImage(bitmap))
        {
            graphics.CopyFromScreen(rectangle.Location, Point.Empty, bitmap.Size);
            bitmap.Save(Path.Combine(_evidence, fileName));
        }
    }
    private static object Field(object instance, string name) => typeof(MainForm).GetField(name, PrivateInstance).GetValue(instance);
    private static void SetField(object instance, string name, object value) => typeof(MainForm).GetField(name, PrivateInstance).SetValue(instance, value);
    private static object Call(object instance, string name, params object[] args) => typeof(MainForm).GetMethod(name, PrivateInstance).Invoke(instance, args);
    private static void StopDiscovery(FpsLimiterService service) => ((System.Windows.Forms.Timer)typeof(FpsLimiterService).GetField("_discovery", PrivateInstance).GetValue(service)).Stop();

    internal static async Task Run(string[] args)
    {
        _evidence = Path.Combine(Path.GetTempPath(), "TrayTemps-tests-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_evidence);
        Console.WriteLine("Evidence: " + _evidence);
        Settings();
        string native = FpsLimiterPayloads.ExtractNative();
        Check(File.Exists(Path.Combine(native, "x64", "FpsLimiter.Native.dll")) && File.Exists(Path.Combine(native, "x86", "FpsLimiter.Native.dll")), "both embedded native payloads extract");
        string hash = Hash(Path.Combine(native, "x64", "FpsLimiter.Native.dll"));
        Check(native == FpsLimiterPayloads.ExtractNative() && hash == Hash(Path.Combine(native, "x64", "FpsLimiter.Native.dll")), "payload extraction reuses verified cache");
        IntPtr module = LoadLibraryW(Path.Combine(native, "x64", "FpsLimiter.Native.dll"));
        Check(module != IntPtr.Zero, "x64 native payload loads from extracted cache");
        if (module != IntPtr.Zero) FreeLibrary(module);
        await Ui();
        await OwnershipAndShutdown(native);
        await HostLifecycle(native);
        if (args.Length > 1 && args[0] == "--discovery") { await Discovery(args[1]); return; }
        if (args.Length > 1 && args[0] == "--vulkan") { await Graphics(args[1], "Vulkan"); return; }
        if (args.Length != 0)
        {
            foreach (string api in new[] { "D3D11", "D3D12", "D3D9", "OpenGL" })
                await Graphics(Path.Combine(args[0], api + "GraphicsTest", "x64", "Release", api + "GraphicsTest.exe"), api);
            await Discovery(args[0]);
        }
        if (args.Length > 1) await Graphics(args[1], "Vulkan");
    }

    private static string Hash(string path)
    {
        using (var sha = System.Security.Cryptography.SHA256.Create())
        using (var stream = File.OpenRead(path)) return BitConverter.ToString(sha.ComputeHash(stream));
    }

    private static void Settings()
    {
        var old = JsonSerializer.Deserialize<AppSettings>("{\"LightMode\":true,\"UpdateInterval\":0.75,\"CpuIdentifier\":\"cpu-test\"}");
        Check(old.LightMode && old.UpdateInterval == .75m && !old.FpsLimiter.Enabled && old.FpsLimiter.FramesPerSecond == 60, "old settings preserve host values and default limiter off");
        old.FpsLimiter = new FpsLimiterConfiguration { Enabled = true, FramesPerSecond = 72, ToggleHotkeyEnabled = true, ExcludedProcesses = "editor.exe" };
        old.FpsLimiter.GameLimits.Add(new GameLimit { ExecutablePath = @"C:\Games\test.exe", FramesPerSecond = 90 });
        string path = Path.Combine(_evidence, "settings.json");
        SettingsFileHelper.WriteSettings(path, old);
        var loaded = JsonSerializer.Deserialize<AppSettings>(SettingsFileHelper.ReadSettingsJson(path));
        Check(loaded.FpsLimiter.Enabled && loaded.FpsLimiter.ToggleHotkeyEnabled && loaded.FpsLimiter.GameLimits[0].FramesPerSecond == 90 && loaded.LightMode && loaded.CpuIdentifier == "cpu-test", "shared settings writer persists host and all limiter fields");
        SettingsFileHelper.WriteSettings(path, loaded);
        File.WriteAllText(path, "broken");
        Check(JsonSerializer.Deserialize<AppSettings>(SettingsFileHelper.ReadSettingsJson(path)).FpsLimiter.Enabled, "existing settings backup recovery includes limiter");
        var clone = loaded.FpsLimiter.Clone(); clone.GameLimits[0].FramesPerSecond = 120;
        Check(loaded.FpsLimiter.GameLimits[0].FramesPerSecond == 90, "profile drafts are deep copies");
        clone.FramesPerSecond = 0;
        Check(!FpsLimiterConfiguration.Normalize(clone).Enabled && FpsLimiterConfiguration.Normalize(null).FramesPerSecond == 60, "invalid limiter section normalizes independently");
        clone = loaded.FpsLimiter.Clone(); clone.GameLimits = null; clone.Validate();
        Check(clone.GameLimits.Count == 0, "null profile list normalizes safely");
        clone.GameLimits.Add(new GameLimit { ExecutablePath = @"C:\Games\test.exe" });
        clone.GameLimits.Add(new GameLimit { ExecutablePath = @"c:\games\TEST.exe" });
        bool duplicate = false; try { clone.Validate(); } catch (InvalidDataException) { duplicate = true; }
        Check(duplicate, "duplicate executable profiles rejected case-insensitively");
    }

    // Skip real startup: never open hardware, run PawnIO/elevation, or read/write the user's settings.
    private sealed class IsolatedMainForm : MainForm { protected override void OnLoad(EventArgs e) { } }
    private static async Task Ui()
    {
        using (var owner = new IsolatedMainForm())
        {
            owner.Show(); owner.Opacity = 1;
            Call(owner, "SetDefaultControlValues");
            Call(owner, "ApplyTheme");
            var checkbox = (CheckBox)Field(owner, "fpsLimiterEnable");
            var settingsButton = (Button)Field(owner, "fpsLimiterSettings");
            Check(checkbox.Text == "FPS limiter:" && settingsButton.Text == "\u2699", "host limiter controls have Designer text and names");
            checkbox.Checked = true;
            Check(((FpsLimiterConfiguration)Field(owner, "_fpsLimiterConfiguration")).Enabled && settingsButton.Enabled, "checkbox event updates configuration before runtime initialization");
            checkbox.Checked = false;
            Check(!checkbox.Checked && !settingsButton.Enabled && Field(owner, "_fpsLimiterService") == null, "default host UI creates no limiter runtime or polling");
            var panel = (TableLayoutPanel)Field(owner, "generalSettingsPanel");
            Check(panel.GetControlFromPosition(1, 3) == Field(owner, "fpsLimiterPanel") && panel.RowCount == 4, "new controls occupy free settings cell without resizing existing rows");
            var config = new FpsLimiterConfiguration { FramesPerSecond = 72 };
            config.GameLimits.Add(new GameLimit { ExecutablePath = @"C:\Games\draft.exe", FramesPerSecond = 90 });
            SetField(owner, "_fpsLimiterConfiguration", config);
            foreach (bool light in new[] { false, true })
            {
                ((CheckBox)Field(owner, "lightModeSwitch")).Checked = light;
                Call(owner, "ApplyTheme");
                using (var dialog = new FpsLimiterSettingsDialog(owner, config, null))
                {
                    dialog.Show(owner); await Task.Delay(100);
                    dialog.Location = Point.Empty;
                    await Task.Delay(50);
                    var numeric = (NumericUpDown)dialog.Controls.Find("fallbackCap", true).Single();
                    Check(numeric.Value == 72 && dialog.BackColor == ThemePalette.Create(light).Border, "configuration follows " + (light ? "light" : "dark") + " host theme and values");
                    Check(((Control)Field(owner, "fpsLimiterPanel")).BackColor == ThemePalette.Create(light).SurfaceBack, "main limiter row follows host surface theme");
                    Check(dialog.Controls.Find("games", true).Single().Height > 50 && dialog.Controls.Find("save", true).Single().Right <= dialog.ClientSize.Width, "dialog layout keeps profiles and Save visible");
                    if (!GetWindowRect(dialog.Handle, out WindowRect bounds)) throw new System.ComponentModel.Win32Exception();
                    var windowBounds = Rectangle.FromLTRB(bounds.Left, bounds.Top, bounds.Right, bounds.Bottom);
                    using (var bitmap = new Bitmap(windowBounds.Width, windowBounds.Height))
                    {
                        using (var graphics = System.Drawing.Graphics.FromImage(bitmap))
                            graphics.CopyFromScreen(windowBounds.Location, Point.Empty, bitmap.Size);
                        bitmap.Save(Path.Combine(_evidence, light ? "dialog-light.png" : "dialog-dark.png"));
                    }
                    var games = (ListView)dialog.Controls.Find("games", true).Single();
                    var gamesHeader = dialog.Controls.Find("gamesHeader", true).Single();
                    var body = dialog.Controls.Find("body", true).Single(); var gamesPanel = dialog.Controls.Find("gamesPanel", true).Single();
                    var resizeGrip = dialog.Controls.Find("resizeGrip", true).Single();
                    var palette = ThemePalette.Create(light);
                    Check(games.HeaderStyle == ColumnHeaderStyle.None && gamesHeader.Controls.Count == 3 &&
                        gamesPanel.Width >= body.ClientSize.Width * 0.9f && gamesHeader.Controls.Cast<Control>().All(header => header.BackColor == palette.BarBack && header.ForeColor == palette.TitleText),
                        "FPS profile header uses standard themed Designer labels");
                    Check(resizeGrip.BackColor == palette.BarBack, "FPS resize grip background matches the " + (light ? "light" : "dark") + " footer");
                    ((CheckBox)Field(owner, "lightModeSwitch")).Checked = !light;
                    Check(dialog.BackColor == ThemePalette.Create(!light).Border, "open limiter dialog responds to host theme event");
                    ((CheckBox)Field(owner, "lightModeSwitch")).Checked = light;
                    numeric.Value = 100;
                    dialog.Close();
                    Check(config.FramesPerSecond == 72 && dialog.SelectedConfiguration == null, "closing draft leaves host configuration unchanged");
                }

                using (var stream = typeof(FpsLimiterSettingsDialog).Assembly.GetManifestResourceStream("TrayTemps.FpsLimiter.Notice"))
                using (var reader = new StreamReader(stream))
                using (var noticeDialog = new FpsLimiterNoticesDialog(reader.ReadToEnd(), owner))
                {
                    var noticeText = (TextBox)noticeDialog.Controls.Find("noticeText", true).Single();
                    var palette = ThemePalette.Create(light);
                    Check(noticeDialog.ClientSize == new Size(624, 458) && noticeDialog.MinimumSize == new Size(520, 360) &&
                        noticeText.Multiline && noticeText.ReadOnly && noticeText.WordWrap && noticeText.ScrollBars == ScrollBars.Vertical &&
                        noticeText.TextLength > 1000 && noticeText.Text.Contains(Environment.NewLine),
                        "third-party notices use a compact read-only vertical-scroll window");
                    Check(noticeDialog.BackColor == palette.WindowBack && noticeText.BackColor == palette.InputBack && noticeText.ForeColor == palette.Text,
                        "third-party notices follow the " + (light ? "light" : "dark") + " theme");
                    var closeTimer = new System.Windows.Forms.Timer { Interval = 100 };
                    bool opensWithoutSelectingNotice = false;
                    closeTimer.Tick += (sender, e) =>
                    {
                        closeTimer.Stop();
                        opensWithoutSelectingNotice = noticeText.SelectionLength == 0;
                        CaptureWindow(noticeDialog, "notices-" + (light ? "light" : "dark") + ".png");
                        ((Button)noticeDialog.Controls.Find("close", true).Single()).PerformClick();
                    };
                    closeTimer.Start();
                    DialogResult noticeResult = noticeDialog.ShowDialog(owner);
                    closeTimer.Dispose();
                    Check(noticeResult == DialogResult.OK, "third-party notices close normally from the modal dialog");
                    Check(opensWithoutSelectingNotice, "third-party notice text opens unselected at the start");
                }
            }
            using (var dialog = new FpsLimiterSettingsDialog(owner, config, null))
            {
                dialog.Show(owner);
                ((NumericUpDown)dialog.Controls.Find("fallbackCap", true).Single()).Value = 80;
                ((Button)dialog.Controls.Find("save", true).Single()).PerformClick();
                Check(dialog.SelectedConfiguration.FramesPerSecond == 80 && config.FramesPerSecond == 72, "Save returns isolated validated configuration");
            }
            SetField(owner, "_fpsLimiterRuntimeReady", true);
            config.ToggleHotkeyEnabled = true;
            Call(owner, "ApplyFpsLimiterHotkey");
            Check((bool)Field(owner, "_fpsLimiterHotkeyRegistered"), "FPS toggle shortcut registers with host handle");
            Check(owner.CanRegisterOsdHotkey(OsdHotkeyModifiers.Control | OsdHotkeyModifiers.Shift, Keys.F12), "OSD can validate FPS shortcut without changing live registration");
            Call(owner, "GiveOsdHotkeyPriority", OsdHotkeyModifiers.Control | OsdHotkeyModifiers.Shift, Keys.F12);
            Check(!config.ToggleHotkeyEnabled && !(bool)Field(owner, "_fpsLimiterHotkeyRegistered"), "committed OSD conflict disables only FPS shortcut");
            var osd = (OsdConfiguration)Field(owner, "_osdConfiguration");
            osd.HotkeyEnabled = true; osd.HotkeyModifiers = 6; osd.HotkeyKey = (int)Keys.F12;
            Check(!owner.CanRegisterFpsLimiterHotkey(), "FPS shortcut validation gives configured OSD binding priority");
            osd.HotkeyEnabled = false;
            owner.Hide(); Call(owner, "ShowWindow"); await Task.Delay(100);
            Check(owner.Visible && owner.WindowState == FormWindowState.Normal, "host tray restore still shows normal window");
            owner.Close();
        }
    }

    private static async Task OwnershipAndShutdown(string native)
    {
        using (var lease = new Mutex(false, "Local\\TrayFPS.Instance", out bool created))
        {
            Check(created, "no existing TrayFPS instance owns the engine");
            using (var denied = new FpsLimiterService(new FpsLimiterConfiguration(), () => { throw new Exception("must not extract"); }))
            {
                await denied.SetEnabledAsync(true);
                Check(!denied.Enabled && denied.LastError is InvalidOperationException && typeof(FpsLimiterService).GetField("_controller", PrivateInstance).GetValue(denied) == null, "nonowner refuses activation without creating or stopping controller");
                await denied.StopAsync();
            }
        }
        var started = new ManualResetEventSlim(); var release = new ManualResetEventSlim();
        using (var service = new FpsLimiterService(new FpsLimiterConfiguration(), () => { started.Set(); release.Wait(); return native; }))
        {
            Task enable = service.SetEnabledAsync(true);
            Check(await Task.Run(() => started.Wait(5000)), "pending startup worker entered controlled extraction");
            Task stop = service.StopAsync();
            Check(!stop.IsCompleted && service.IsBusy, "shutdown waits for pending attachment/startup work");
            release.Set(); await stop; await enable;
            using (var lease = new Mutex(false, "Local\\TrayFPS.Instance", out bool created)) Check(created, "ownership released only after pending worker cleanup");
        }
        started.Dispose(); release.Dispose();
        using (var entered = new ManualResetEventSlim())
        using (var proceed = new ManualResetEventSlim())
        using (var service = new FpsLimiterService(new FpsLimiterConfiguration(), () => { entered.Set(); proceed.Wait(); return native; }))
        {
            Task enable = service.SetEnabledAsync(true);
            Check(await Task.Run(() => entered.Wait(5000)), "configuration race worker entered");
            Task save = service.ApplyConfigurationAsync(new FpsLimiterConfiguration { FramesPerSecond = 73 });
            Check(!save.IsCompleted, "configuration Save waits for active worker");
            proceed.Set(); await enable; await save; StopDiscovery(service);
            using (var mapping = System.IO.MemoryMappedFiles.MemoryMappedFile.OpenExisting("Local\\TrayFPS.Config.Global"))
            using (var view = mapping.CreateViewAccessor()) Check(view.ReadDouble(24) == 73, "queued Save applies latest fallback after startup");
            await service.StopAsync();
        }
        using (var service = new FpsLimiterService(new FpsLimiterConfiguration()))
        {
            await service.SetEnabledAsync(true); StopDiscovery(service);
            Check(service.Enabled && service.LastError == null, "embedded runtime starts");
            await service.SetEnabledAsync(false);
            using (var lease = new Mutex(false, "Local\\TrayFPS.Instance", out bool created)) Check(!created, "disabled controller retains exclusive lease until shutdown");
            await service.SetEnabledAsync(true); StopDiscovery(service);
            Check(service.Enabled && service.LastError == null, "runtime resumes after disable");
            await service.ApplyConfigurationAsync(new FpsLimiterConfiguration { FramesPerSecond = 75 });
            Check(service.LastError == null && service.Enabled, "fallback cap updates while waiting for target");
            await service.StopAsync();
        }
    }

    private static async Task HostLifecycle(string native)
    {
        foreach (bool restart in new[] { true, false })
        using (var entered = new ManualResetEventSlim())
        using (var proceed = new ManualResetEventSlim())
        using (var owner = new IsolatedMainForm())
        {
            owner.Show();
            var service = new FpsLimiterService(new FpsLimiterConfiguration(), () => { entered.Set(); proceed.Wait(); return native; });
            SetField(owner, "_fpsLimiterService", service);
            Task enable = service.SetEnabledAsync(true);
            Check(await Task.Run(() => entered.Wait(5000)), "host lifecycle pending worker entered");
            try
            {
                if (restart)
                {
                    var prepare = (Task<bool>)Call(owner, "PrepareFpsLimiterForRestartAsync");
                    Check(!prepare.IsCompleted, "host restart waits for limiter cleanup before replacement launch");
                    proceed.Set(); Check(await prepare && Field(owner, "_fpsLimiterService") == null, "host restart releases cleaned service");
                    Call(owner, "ResumeFpsLimiterAfterCancelledRestart");
                    Check(!(bool)Field(owner, "_fpsLimiterRestartPending"), "cancelled restart restores FPS UI availability");
                }
                else
                {
                    Call(owner, "ExecuteShutdownSequence"); owner.Close();
                    Check(!owner.IsDisposed, "close defers while central shutdown has pending limiter work");
                    proceed.Set(); await service.StopAsync(); await Task.Delay(100);
                    Check(owner.IsDisposed, "deferred close completes even after central shutdown began");
                }
            }
            finally { proceed.Set(); await service.StopAsync(); await enable; }
        }
        using (var entered = new ManualResetEventSlim())
        using (var proceed = new ManualResetEventSlim())
        using (var owner = new IsolatedMainForm())
        {
            owner.Show();
            var service = new FpsLimiterService(new FpsLimiterConfiguration(), () => { entered.Set(); proceed.Wait(); return native; });
            SetField(owner, "_fpsLimiterService", service);
            Task enable = service.SetEnabledAsync(true);
            try
            {
                Check(await Task.Run(() => entered.Wait(5000)), "restart/exit race worker entered");
                var prepare = (Task<bool>)Call(owner, "PrepareFpsLimiterForRestartAsync");
                owner.Close(); proceed.Set();
                Check(!await prepare, "user exit cancels a replacement restart awaiting cleanup");
                await Task.Delay(100);
                Check(owner.IsDisposed, "user exit during restart preparation closes normally");
            }
            finally { proceed.Set(); await service.StopAsync(); await enable; }
        }
    }

    private sealed class Sample { internal DateTime Time; internal string Line; }

    private static async Task Discovery(string targets)
    {
        string firstPath = Path.Combine(targets, "D3D11GraphicsTest", "x64", "Release", "D3D11GraphicsTest.exe");
        string secondPath = Path.Combine(targets, "D3D12GraphicsTest", "x64", "Release", "D3D12GraphicsTest.exe");
        var config = new FpsLimiterConfiguration();
        config.GameLimits.Add(new GameLimit { ExecutablePath = firstPath, FramesPerSecond = 45 });
        config.GameLimits.Add(new GameLimit { ExecutablePath = secondPath, FramesPerSecond = 50 });
        using (var service = new FpsLimiterService(config))
        using (var first = Process.Start(new ProcessStartInfo(firstPath) { UseShellExecute = false, RedirectStandardOutput = true, CreateNoWindow = true }))
        using (var second = Process.Start(new ProcessStartInfo(secondPath) { UseShellExecute = false, RedirectStandardOutput = true, CreateNoWindow = true }))
        {
            first.BeginOutputReadLine(); second.BeginOutputReadLine();
            try
            {
                await Task.Delay(800);
                await service.SetEnabledAsync(true); StopDiscovery(service);
                await WaitIdle(service);
                foreach (var process in new[] { first, second })
                {
                    await SelectForeground(process);
                    typeof(FpsLimiterService).GetMethod("Discovery_Tick", PrivateInstance).Invoke(service, new object[] { null, EventArgs.Empty });
                    await WaitIdle(service);
                    int expected = process == first ? 45 : 50;
                    using (var mapping = System.IO.MemoryMappedFiles.MemoryMappedFile.OpenExisting("Local\\TrayFPS.Config." + process.Id))
                    using (var view = mapping.CreateViewAccessor()) Check(view.ReadInt32(12) == 1 && view.ReadDouble(24) == expected, "foreground discovery applies executable profile " + expected);
                }
                using (var mapping = System.IO.MemoryMappedFiles.MemoryMappedFile.OpenExisting("Local\\TrayFPS.Config." + first.Id))
                using (var view = mapping.CreateViewAccessor()) Check(view.ReadInt32(12) == 0, "switching foreground target disables previous target mapping");
                config.ExcludedProcesses = second.ProcessName + ".exe";
                await service.ApplyConfigurationAsync(config);
                typeof(FpsLimiterService).GetMethod("Discovery_Tick", PrivateInstance).Invoke(service, new object[] { null, EventArgs.Empty });
                await WaitIdle(service);
                Check((int)typeof(FpsLimiterService).GetField("_attachedPid", PrivateInstance).GetValue(service) == 0 && service.Enabled, "saved exclusion detaches active target and prevents reattachment");
            }
            finally
            {
                await service.StopAsync();
                foreach (var process in new[] { first, second }) if (!process.HasExited) { process.Kill(); process.WaitForExit(); }
            }
        }
    }

    private static async Task WaitIdle(FpsLimiterService service)
    {
        var timeout = Stopwatch.StartNew();
        while (service.IsBusy && timeout.ElapsedMilliseconds < 25000) await Task.Delay(25);
        Check(!service.IsBusy && service.LastError == null, "discovery worker finishes successfully");
    }

    private static async Task SelectForeground(Process process)
    {
        var deadline = Stopwatch.StartNew();
        while (deadline.ElapsedMilliseconds < 5000)
        {
            process.Refresh();
            IntPtr window = process.MainWindowHandle;
            if (window != IntPtr.Zero)
            {
                uint current = GetCurrentThreadId();
                uint foreground = GetWindowThreadProcessId(GetForegroundWindow(), out uint ignored);
                uint target = GetWindowThreadProcessId(window, out ignored);
                bool joinedForeground = foreground != current && AttachThreadInput(current, foreground, true);
                bool joinedTarget = target != current && target != foreground && AttachThreadInput(current, target, true);
                try { SetForegroundWindow(window); }
                finally
                {
                    if (joinedTarget) AttachThreadInput(current, target, false);
                    if (joinedForeground) AttachThreadInput(current, foreground, false);
                }
                await Task.Delay(100);
                if (GetForegroundWindow() == window) { Check(true, "controlled foreground target selected"); return; }
            }
            await Task.Delay(100);
        }
        throw new Exception("Could not focus " + process.ProcessName + "; hwnd=" + process.MainWindowHandle + "; foreground=" + GetForegroundWindow());
    }

    private static async Task Graphics(string executable, string api)
    {
        Check(File.Exists(executable), api + " controlled target available");
        var samples = new List<Sample>();
        var diagnostics = new List<string>();
        int initialCap = api == "OpenGL" ? 30 : 60;
        int liveCap = api == "OpenGL" ? 20 : 90;
        using (var service = new FpsLimiterService(new FpsLimiterConfiguration { FramesPerSecond = initialCap }))
        using (var process = new Process { StartInfo = new ProcessStartInfo(executable) { UseShellExecute = false, RedirectStandardOutput = true, RedirectStandardError = true, CreateNoWindow = true } })
        {
            process.OutputDataReceived += (s, e) => { if (e.Data != null) lock (samples) samples.Add(new Sample { Time = DateTime.UtcNow, Line = e.Data }); };
            process.ErrorDataReceived += (s, e) => { if (e.Data != null) lock (diagnostics) diagnostics.Add(e.Data); };
            await service.SetEnabledAsync(true); StopDiscovery(service);
            Check(service.Enabled, api + " limiter prepares before target starts");
            process.Start(); process.BeginOutputReadLine(); process.BeginErrorReadLine();
            try
            {
                await Task.Delay(800);
                await service.AttachTargetAsync(process.Id, process.ProcessName, executable, initialCap);
                Check(service.LastError == null, api + " attaches");
                DateTime begin = DateTime.UtcNow; await Task.Delay(2500);
                AssertPeriod(samples, begin, DateTime.UtcNow, initialCap, api + " enabled at " + initialCap);
                await service.SetEnabledAsync(false);
                begin = DateTime.UtcNow; await Task.Delay(2500);
                AssertPeriod(samples, begin, DateTime.UtcNow, -initialCap, api + " disabled pass-through");
                await service.SetEnabledAsync(true); StopDiscovery(service);
                await service.AttachTargetAsync(process.Id, process.ProcessName, executable, initialCap);
                begin = DateTime.UtcNow; await Task.Delay(2500);
                AssertPeriod(samples, begin, DateTime.UtcNow, initialCap, api + " re-enabled at " + initialCap);
                var config = new FpsLimiterConfiguration { FramesPerSecond = liveCap };
                await service.ApplyConfigurationAsync(config);
                begin = DateTime.UtcNow; await Task.Delay(2500);
                AssertPeriod(samples, begin, DateTime.UtcNow, liveCap, api + " live cap " + liveCap);
                await service.StopAsync();
                begin = DateTime.UtcNow; await Task.Delay(2000);
                AssertPeriod(samples, begin, DateTime.UtcNow, -initialCap, api + " controller exit returns pass-through");
                if (!process.HasExited) await Task.Run(() => process.WaitForExit(6000));
                Check(process.HasExited && process.ExitCode == 0, api + " controlled target clean exit");
            }
            finally
            {
                await service.StopAsync();
                if (!process.HasExited) { process.Kill(); process.WaitForExit(); }
                lock (samples) File.WriteAllLines(Path.Combine(_evidence, api + ".log"), samples.Select(sample => sample.Time.ToString("O") + " " + sample.Line));
                lock (diagnostics) File.WriteAllLines(Path.Combine(_evidence, api + ".stderr.log"), diagnostics);
            }
        }
    }

    private static void AssertPeriod(List<Sample> samples, DateTime start, DateTime end, int cap, string name)
    {
        Sample[] period; lock (samples) period = samples.Where(sample => sample.Time >= start.AddSeconds(1.15) && sample.Time <= end).ToArray();
        Check(period.Length > 0, name + " stable samples collected");
        double mean = period.Average(sample => double.Parse(Regex.Match(sample.Line, @"mean_ms=([\d.]+)").Groups[1].Value, System.Globalization.CultureInfo.InvariantCulture));
        Console.WriteLine(name + ": mean_ms=" + mean.ToString("F3") + " | " + period.Last().Line);
        Check(cap < 0 ? mean < (1000.0 / -cap) * .8 : Math.Abs(mean - 1000.0 / cap) < 2.0, name + " measured presentation timing");
    }
}
