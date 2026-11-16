using System;
using System.IO;
using System.ComponentModel;
using System.Diagnostics;
using System.IO.MemoryMappedFiles;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using FpsLimiter.Core;
using TrayTemps;

// These focused checks never enable discovery, modify user settings, or register a layer.
internal static class AuditFixTests
{
    private static void Check(bool condition, string description)
    {
        if (!condition) throw new Exception(description);
        Console.WriteLine("PASS: " + description);
    }

    internal static async Task Run()
    {
        const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
        var service = new FpsLimiterService(new FpsLimiterConfiguration
        {
            ExcludedProcesses = "  My Game.exe ; OTHER.EXE,\r\n\tThird Game\t; ;"
        });
        try
        {
            var method = typeof(FpsLimiterService).GetMethod("IsExcluded", flags);
            Func<string, bool> excluded = name => (bool)method.Invoke(service, new object[] { name });
            Check(excluded("My Game") && excluded("Third Game"), "spaced process names are preserved and surrounding whitespace is trimmed");
            Check(!excluded("My") && !excluded("Game") && !excluded("Third"), "spaced exclusions do not accidentally exclude separate words");
            Check(excluded("other") && excluded("EXPLORER"), "delimiters, case-insensitive matching and built-in exclusions still work");
        }
        finally
        {
            // No controller was created, so dispose only the inactive discovery timer.
            ((Timer)typeof(FpsLimiterService).GetField("_discovery", flags).GetValue(service)).Dispose();
        }

        string sandbox = Path.Combine(Path.GetTempPath(), "TrayTemps-cache-test-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(sandbox);
        try
        {
            string cache = Path.Combine(sandbox, "TrayTemps", "FpsLimiter");
            foreach (string version in new[] { "old-engine", "current-engine" })
            {
                string native = Path.Combine(cache, version, "native", "x64");
                Directory.CreateDirectory(native);
                File.WriteAllText(Path.Combine(native, "FpsLimiter.Native.dll"), "test payload");
                File.WriteAllText(Path.Combine(cache, version, "TrayFPS-Vulkan-x64.json"), "test manifest");
            }
            string neighbor = Path.Combine(sandbox, "TrayTemps", "keep.txt");
            File.WriteAllText(neighbor, "unrelated app data");
            string otherApp = Path.Combine(sandbox, "OtherApp");
            Directory.CreateDirectory(otherApp);
            FpsLimiterPayloads.DeleteCache(sandbox);
            Check(!Directory.Exists(cache), "uninstall cleanup removes all limiter engine versions and manifests");
            Check(File.ReadAllText(neighbor) == "unrelated app data" && Directory.Exists(otherApp), "uninstall cleanup preserves neighboring data");
            FpsLimiterPayloads.DeleteCache(sandbox);
            Check(!Directory.Exists(cache), "cleanup safely accepts an absent cache");
            Directory.CreateDirectory(cache);
            string lockedPayload = Path.Combine(cache, "locked.dll");
            using (var locked = new FileStream(lockedPayload, FileMode.Create, FileAccess.ReadWrite, FileShare.None))
            {
                bool failed = false;
                try { FpsLimiterPayloads.DeleteCache(sandbox); }
                catch (IOException) { failed = true; }
                Check(failed && File.Exists(lockedPayload) && File.Exists(neighbor), "locked payload cleanup fails explicitly and preserves neighboring data");
            }
            FpsLimiterPayloads.DeleteCache(sandbox);
            Check(!Directory.Exists(cache), "cleanup succeeds after the payload is released");
            foreach (string invalid in new[] { "", "relative", @"C:relative", @"\relative" })
            {
                bool rejected = false;
                try { FpsLimiterPayloads.DeleteCache(invalid); }
                catch (InvalidOperationException) { rejected = true; }
                Check(rejected, "cleanup rejects an unresolved application-data root: " + invalid);
            }
        }
        finally
        {
            Directory.Delete(sandbox, true);
        }
        await CandidateFailures();
        ModuleIdentity();
        await PendingLoadCleanup();
    }

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)] private static extern IntPtr LoadLibraryW(string path);
    [DllImport("kernel32.dll")] private static extern bool FreeLibrary(IntPtr module);
    [DllImport("kernel32.dll")] private static extern uint GetCurrentThreadId();
    [DllImport("kernel32.dll", SetLastError = true)] private static extern IntPtr OpenThread(uint access, bool inherit, uint id);
    [DllImport("kernel32.dll")] private static extern bool CloseHandle(IntPtr handle);
    [DllImport("kernel32.dll", SetLastError = true)] private static extern IntPtr VirtualAllocEx(IntPtr process, IntPtr address, UIntPtr size, uint type, uint protection);
    [DllImport("kernel32.dll")] private static extern bool VirtualFreeEx(IntPtr process, IntPtr address, UIntPtr size, uint type);
    [DllImport("kernel32.dll")] private static extern UIntPtr VirtualQueryEx(IntPtr process, IntPtr address, out MemoryInformation info, UIntPtr size);
    [StructLayout(LayoutKind.Sequential)] private struct MemoryInformation
    {
        public IntPtr Base, AllocationBase;
        public uint AllocationProtection;
        public UIntPtr RegionSize;
        public uint State, Protection, Type;
    }

    private static void ModuleIdentity()
    {
        string root = Path.Combine(Path.GetTempPath(), "TrayTemps-modules-" + Guid.NewGuid().ToString("N"));
        var modules = new IntPtr[2];
        try
        {
            var find = typeof(LimiterController).GetMethod("FindRemoteModule", BindingFlags.Static | BindingFlags.NonPublic);
            for (int i = 0; i < modules.Length; ++i)
            {
                string directory = Path.Combine(root, "engine-" + i);
                Directory.CreateDirectory(directory);
                string path = Path.Combine(directory, "FpsLimiter.Native.dll");
                using (var source = typeof(FpsLimiterConfiguration).Assembly.GetManifestResourceStream("TrayFPS.Payload.x64.dll"))
                using (var target = File.Create(path)) source.CopyTo(target);
                modules[i] = LoadLibraryW(path);
                Check(modules[i] != IntPtr.Zero, "isolated payload loads for module identity test");
                Check((IntPtr)find.Invoke(null, new object[] { Process.GetCurrentProcess().Id, path }) == modules[i], "module lookup matches the full engine path");
            }
            Check(modules[0] != modules[1], "two engine paths with the same basename remain distinct");
            Check((IntPtr)find.Invoke(null, new object[] { Process.GetCurrentProcess().Id, Path.Combine(root, "absent", "FpsLimiter.Native.dll") }) == IntPtr.Zero,
                "module lookup never substitutes a different engine with the same basename");
        }
        finally
        {
            foreach (IntPtr module in modules) if (module != IntPtr.Zero) FreeLibrary(module);
            Directory.Delete(root, true);
        }
    }

    private static async Task PendingLoadCleanup()
    {
        using (var process = Process.GetCurrentProcess())
        using (var entered = new System.Threading.ManualResetEventSlim())
        using (var release = new System.Threading.ManualResetEventSlim())
        {
            uint threadId = 0;
            var worker = new System.Threading.Thread(() => { threadId = GetCurrentThreadId(); entered.Set(); release.Wait(); }) { IsBackground = true };
            worker.Start();
            entered.Wait();
            IntPtr thread = OpenThread(0x100000, false, threadId);
            IntPtr buffer = VirtualAllocEx(process.Handle, IntPtr.Zero, (UIntPtr)64, 0x3000, 0x04);
            bool transferred = false;
            try
            {
                Check(thread != IntPtr.Zero && buffer != IntPtr.Zero, "controlled loader thread and path buffer created");
                typeof(LimiterController).GetMethod("TrackPendingLoad", BindingFlags.Static | BindingFlags.NonPublic)
                    .Invoke(null, new object[] { process.Handle, thread, buffer });
                transferred = true;
                GC.Collect(); GC.WaitForPendingFinalizers();
                await Task.Delay(1200);
                MemoryInformation info;
                UIntPtr size = (UIntPtr)Marshal.SizeOf(typeof(MemoryInformation));
                Check(VirtualQueryEx(process.Handle, buffer, out info, size) != UIntPtr.Zero && info.State == 0x1000,
                    "pending load retains its path buffer while the thread can read it");
                release.Set(); worker.Join();
                await Task.Delay(1200);
                Check(VirtualQueryEx(process.Handle, buffer, out info, size) != UIntPtr.Zero && info.State == 0x10000,
                    "timed-out load frees the path buffer after thread exit, including after GC");
            }
            finally
            {
                release.Set(); worker.Join();
                if (!transferred)
                {
                    if (thread != IntPtr.Zero) CloseHandle(thread);
                    if (buffer != IntPtr.Zero) VirtualFreeEx(process.Handle, buffer, UIntPtr.Zero, 0x8000);
                }
            }
        }
    }

    private static async Task CandidateFailures()
    {
        const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
        using (var ownership = new System.Threading.Mutex(false, "Local\\TrayFPS.Instance", out bool created))
        {
            Check(created, "isolated candidate tests own the limiter session");
            string root = Path.Combine(Path.GetTempPath(), "TrayTemps-attach-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(Path.Combine(root, "x64"));
            Directory.CreateDirectory(Path.Combine(root, "x86"));
            File.WriteAllText(Path.Combine(root, "x64", "FpsLimiter.Native.dll"), "mock injector only");
            File.WriteAllText(Path.Combine(root, "x86", "FpsLimiter.Native.dll"), "mock injector only");
            try
            {
                foreach (Exception failure in new Exception[] { new Win32Exception(5), new TimeoutException("simulated timeout"), new Win32Exception("simulated hook rejection"), null })
                using (var candidate = Process.Start(new ProcessStartInfo("powershell.exe", "-NoProfile -NonInteractive -Command Start-Sleep -Seconds 60") { UseShellExecute = false, CreateNoWindow = true }))
                {
                    int firstPid = Process.GetCurrentProcess().Id, attempts = 0;
                    bool reject = true;
                    using (var limiter = new LimiterController((process, dll, required) =>
                    {
                        if (process.Id == firstPid) return;
                        ++attempts;
                        if (!reject) return;
                        if (failure != null) throw failure;
                        process.Kill(); process.WaitForExit();
                    }))
                    using (var service = new FpsLimiterService(new FpsLimiterConfiguration { Enabled = true }, () => root))
                    {
                        try
                        {
                            // Skip registration in these focused tests. The mock injector
                            // exercises the real controller transaction and service policy.
                            typeof(LimiterController).GetField("vulkanLayerPrepared", flags).SetValue(limiter, true);
                            limiter.Apply(firstPid, 90, true, root);
                            typeof(FpsLimiterService).GetField("_controller", flags).SetValue(service, limiter);
                            typeof(FpsLimiterService).GetField("<Enabled>k__BackingField", flags).SetValue(service, true);
                            typeof(FpsLimiterService).GetField("_attachedPid", flags).SetValue(service, firstPid);
                            await service.AttachTargetAsync(candidate.Id, "ControlledCandidate", "test.exe", 60);
                            using (var map = MemoryMappedFile.OpenExisting("Local\\TrayFPS.Config." + firstPid))
                            using (var view = map.CreateViewAccessor())
                                Check(service.Enabled && ReferenceEquals(service.LastError, failure) && view.ReadInt32(12) == 1 && view.ReadDouble(24) == 90,
                                    (failure?.Message ?? "candidate exit") + ": rejection preserves the previous 90 FPS cap and Enabled state");
                            Check((int)typeof(FpsLimiterService).GetField("_attachedPid", flags).GetValue(service) == firstPid, "candidate failure preserves target identity");
                            await service.AttachTargetAsync(candidate.Id, "ControlledCandidate", "test.exe", 60);
                            Check(attempts == 1 && service.Enabled, "same process lifetime is not repeatedly injected after failure");
                            using (var map = MemoryMappedFile.OpenExisting("Local\\TrayFPS.Config." + firstPid))
                            using (var view = map.CreateViewAccessor())
                            {
                                int before = view.ReadInt32(20);
                                await Task.Delay(650);
                                Check(view.ReadInt32(20) != before, "previous target heartbeat continues after candidate rejection");
                            }
                            reject = false;
                            using (var next = Process.Start(new ProcessStartInfo("powershell.exe", "-NoProfile -NonInteractive -Command Start-Sleep -Seconds 60") { UseShellExecute = false, CreateNoWindow = true }))
                            using (var oldMap = MemoryMappedFile.OpenExisting("Local\\TrayFPS.Config." + firstPid))
                            using (var oldView = oldMap.CreateViewAccessor())
                            {
                                try
                                {
                                    await service.AttachTargetAsync(next.Id, "CompatibleCandidate", "next.exe", 75);
                                    using (var nextMap = MemoryMappedFile.OpenExisting("Local\\TrayFPS.Config." + next.Id))
                                    using (var nextView = nextMap.CreateViewAccessor())
                                        Check(service.Enabled && service.LastError == null && oldView.ReadInt32(12) == 0 && nextView.ReadInt32(12) == 1 && nextView.ReadDouble(24) == 75,
                                            "compatible candidate commits after rejection and disables only the previous cap");
                                }
                                finally { if (!next.HasExited) { next.Kill(); next.WaitForExit(); } }
                            }
                        }
                        finally
                        {
                            await service.StopAsync();
                            if (!candidate.HasExited) { candidate.Kill(); candidate.WaitForExit(); }
                        }
                    }
                }
            }
            finally { Directory.Delete(root, true); }
        }
    }
}
