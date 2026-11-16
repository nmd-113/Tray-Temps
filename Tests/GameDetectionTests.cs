using System;
using System.Diagnostics;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using TrayTemps;

internal static class GameDetectionTests
{
    private const BindingFlags Private = BindingFlags.Instance | BindingFlags.NonPublic;
    private static int _checks;
    private static void Check(bool value, string message) { if (!value) throw new Exception(message); Console.WriteLine("PASS " + (++_checks) + ": " + message); }
    [DllImport("user32.dll")] private static extern IntPtr GetForegroundWindow();
    [DllImport("user32.dll")] private static extern uint GetWindowThreadProcessId(IntPtr window, out uint pid);
    [DllImport("user32.dll")] private static extern bool IsWindowVisible(IntPtr window);

    internal static async Task Run()
    {
        _checks = 0;
        const string custom = @"E:\GAMES\Onimusha Way of the Sword\OnimushaWotS.exe";
        Func<string[], GameDetectionDecision> classify = modules => FpsLimiterGameDetection.Evaluate(custom, true, modules);
        Check(classify(new[] { "steam_api64.dll", "d3d11.dll", "dxgi.dll" }).Eligible, "Onimusha-style custom-folder Steam game is eligible without a profile");
        foreach (string runtime in new[] { "d3d9.dll", "d3d10.dll", "d3d11.dll", "d3d12.dll", "opengl32.dll", "vulkan-1.dll" })
            Check(classify(new[] { runtime, "StEaM_ApI64.DlL" }).Eligible, "game SDK recognition supports " + runtime);
        Check(classify(new[] { "EOSSDK-Win64-Shipping.dll", "d3d12.dll" }).Eligible, "custom-folder Epic SDK games are recognized");
        Check(!classify(new[] { "d3d11.dll", "dxgi.dll" }).Eligible, "a graphics-accelerated desktop app is not eligible from graphics modules alone");
        Check(!classify(new[] { "steam_api64.dll", "dxgi.dll" }).Eligible, "Steam SDK and DXGI alone are insufficient without a rendering runtime");
        Check(!classify(new[] { "steamclient64.dll", "d3d11.dll" }).Eligible, "Steam client modules are not mistaken for the game SDK");
        Check(!FpsLimiterGameDetection.Evaluate(custom, false, new[] { "steam_api64.dll", "d3d11.dll" }).Eligible,
            "automatic discovery requires a visible window");
        Check(!FpsLimiterGameDetection.Evaluate("", true, new[] { "steam_api64.dll", "d3d11.dll" }).Eligible, "an unavailable executable path cannot qualify automatically");
        foreach (string engine in new[] { "UnityPlayer.dll", "GameAssembly.dll", "UnrealEngine.dll", "CrySystem.dll" })
            Check(classify(new[] { engine }).Eligible, "existing engine detection is preserved for " + engine);
        foreach (string location in new[] { @"C:\Steam\steamapps\common\Game\game.exe", @"C:\Epic Games\Game\game.exe", @"D:\XboxGames\Game\game.exe", @"D:\GOG Games\Game\game.exe" })
            Check(FpsLimiterGameDetection.Evaluate(location, true, null).Eligible, "existing installation location remains eligible: " + location);
        Check(!classify(new[] { "steam_api64.dll" }).Eligible && classify(new[] { "steam_api64.dll" }).Reason == "Waiting for graphics runtime" &&
            classify(new[] { "steam_api64.dll", "d3d12.dll" }).Eligible, "a game still loading its renderer can qualify on a later inspection");
        using (var service = new FpsLimiterService(new FpsLimiterConfiguration { ExcludedProcesses = "OnimushaWotS.exe" }))
        {
            var excluded = typeof(FpsLimiterService).GetMethod("IsExcluded", Private);
            Check((bool)excluded.Invoke(service, new object[] { "OnimushaWotS" }) && (bool)excluded.Invoke(service, new object[] { "chrome" }),
                "user exclusions and built-in desktop exclusions remain authoritative");
        }
        await Preflight();
        await ShutdownInspection();
        Console.WriteLine("ALL GAME DETECTION CHECKS PASSED: " + _checks);
    }

    private static async Task Preflight()
    {
        int extractions = 0;
        using (var child = Process.Start(new ProcessStartInfo("powershell.exe", "-NoProfile -NonInteractive -Command Start-Sleep -Seconds 60")
            { UseShellExecute = false, CreateNoWindow = true, WindowStyle = ProcessWindowStyle.Hidden }))
        using (var service = new FpsLimiterService(new FpsLimiterConfiguration(), () => { extractions++; throw new Exception("Unexpected extraction"); }))
        {
            try
            {
                typeof(FpsLimiterService).GetField("<Enabled>k__BackingField", Private).SetValue(service, true);
                var attach = typeof(FpsLimiterService).GetMethod("AttachTargetCoreAsync", Private);
                var identityCheck = typeof(FpsLimiterService).GetMethod("HasProcessIdentityChanged", BindingFlags.Static | BindingFlags.NonPublic);
                Check(!(bool)identityCheck.Invoke(null, new object[] { child.Id, child.StartTime.ToUniversalTime() }) &&
                    (bool)identityCheck.Invoke(null, new object[] { child.Id, child.StartTime.ToUniversalTime().AddSeconds(-1) }),
                    "an attached PID reused by a new process is distinguished by its start time");
                await (Task)attach.Invoke(service, new object[] { child.Id, child.ProcessName, child.MainModule.FileName, 60, DateTime.MinValue, true });
                Check(extractions == 0 && service.Enabled && service.LastError == null, "PID identity changes abort before extraction or hook attachment");
                await (Task)attach.Invoke(service, new object[] { child.Id, child.ProcessName, child.MainModule.FileName, 60, child.StartTime.ToUniversalTime(), true });
                Check(extractions == 0 && service.Enabled && service.LastError == null, "a foreground change aborts automatic attachment without disabling the limiter");
            }
            finally { if (!child.HasExited) { child.Kill(); child.WaitForExit(); } }
        }
    }

    private static async Task ShutdownInspection()
    {
        var resultType = typeof(FpsLimiterService).GetNestedType("DiscoverySnapshot", BindingFlags.NonPublic);
        Type sourceType = typeof(TaskCompletionSource<>).MakeGenericType(resultType);
        object source = Activator.CreateInstance(sourceType);
        var inspection = (Task)sourceType.GetProperty("Task").GetValue(source);
        using (var service = new FpsLimiterService(new FpsLimiterConfiguration()))
        {
            typeof(FpsLimiterService).GetField("_inspection", Private).SetValue(service, inspection);
            Task stop = service.StopAsync();
            Check(!stop.IsCompleted, "shutdown waits for an in-flight discovery inspection");
            sourceType.GetMethod("SetResult").Invoke(source, new object[] { null });
            await stop;
            Check(stop.IsCompleted && !service.IsBusy, "shutdown completes after inspection without leaving a controller operation active");
        }
    }

    // Opt-in live check: no window activation, game file changes, or saved preference writes.
    internal static int RunLive(int pid)
    {
        using (var context = new WindowsFormsSynchronizationContext())
        {
            var previous = SynchronizationContext.Current;
            SynchronizationContext.SetSynchronizationContext(context);
            try
            {
                Task task = Live(pid);
                while (!task.IsCompleted) { Application.DoEvents(); Thread.Sleep(5); }
                task.GetAwaiter().GetResult();
                return 0;
            }
            catch (Exception error) { Console.Error.WriteLine(error); return 1; }
            finally { SynchronizationContext.SetSynchronizationContext(previous); }
        }
    }

    private static async Task Live(int pid)
    {
        _checks = 0;
        using (var process = Process.GetProcessById(pid))
        {
            GetWindowThreadProcessId(GetForegroundWindow(), out uint foreground);
            if (foreground != pid) { Console.WriteLine("LIVE_CHECK_SKIPPED: game must remain in the foreground."); return; }
            string path = process.MainModule.FileName;
            var decision = FpsLimiterGameDetection.Evaluate(path, IsWindowVisible(process.MainWindowHandle),
                process.Modules.Cast<ProcessModule>().Select(module => module.ModuleName));
            Check(decision.Eligible, "running game qualifies automatically: " + decision.Reason);
            var config = new FpsLimiterConfiguration { FramesPerSecond = 60 };
            using (var service = new FpsLimiterService(config))
            {
                try
                {
                    await service.SetEnabledAsync(true);
                    ((System.Windows.Forms.Timer)typeof(FpsLimiterService).GetField("_discovery", Private).GetValue(service)).Stop();
                    await Idle(service);
                    Check(service.Enabled && service.LastError == null, "live limiter started successfully");
                    Check((int)typeof(FpsLimiterService).GetField("_attachedPid", Private).GetValue(service) == pid && config.GameLimits.Count == 0 && ReadCap(pid) == 60,
                        "automatic discovery attaches to the live game at the default cap without an executable profile");
                    config.GameLimits.Add(new FpsLimiter.Core.GameLimit { ExecutablePath = path, FramesPerSecond = 75 });
                    await service.ApplyConfigurationAsync(config);
                    Check(ReadCap(pid) == 75, "an in-memory per-game profile overrides the default cap");
                    config.ExcludedProcesses = process.ProcessName + ".exe";
                    await service.ApplyConfigurationAsync(config);
                    await service.DiscoverForegroundAsync();
                    Check((int)typeof(FpsLimiterService).GetField("_attachedPid", Private).GetValue(service) == 0 && ReadCap(pid) == 0 && service.Enabled,
                        "an exclusion detaches the live game and prevents automatic reattachment");
                    config.ExcludedProcesses = ""; config.GameLimits.Clear();
                    await service.ApplyConfigurationAsync(config);
                    await service.DiscoverForegroundAsync();
                    Check(ReadCap(pid) == 60 && service.Enabled, "removing the temporary exclusion restores automatic detection at the default cap");
                }
                finally { await service.StopAsync(); }
                Check(ReadCap(pid) == 0, "stopping the test disables the live game's cap");
            }
        }
        Console.WriteLine("ALL LIVE GAME DETECTION CHECKS PASSED: " + _checks);
    }

    private static double ReadCap(int pid)
    {
        using (var map = MemoryMappedFile.OpenExisting("Local\\TrayFPS.Config." + pid))
        using (var view = map.CreateViewAccessor()) return view.ReadInt32(12) == 0 ? 0 : view.ReadDouble(24);
    }
    private static async Task Idle(FpsLimiterService service)
    {
        var timeout = Stopwatch.StartNew();
        while ((service.IsBusy || service.IsDiscovering) && timeout.ElapsedMilliseconds < 25000) await Task.Delay(20);
        Check(!service.IsBusy && !service.IsDiscovering, "live discovery and attachment finish");
    }
}
