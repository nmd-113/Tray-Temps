using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TrayTemps;

internal static class ReleaseSafetyTests
{
    private const BindingFlags Flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
    private static int _checks;
    private static void Check(bool value, string message) { if (!value) throw new Exception(message); Console.WriteLine("PASS " + (++_checks) + ": " + message); }
    private static FieldInfo Field(object value, string name) => typeof(MainForm).GetField(name, Flags);
    private sealed class Host : MainForm { protected override void OnLoad(EventArgs e) { } }
    private sealed class Hardware : RealProxy
    {
        internal int Updates;
        internal Action Updated;
        internal Hardware(Type type) : base(type) { }
        public override IMessage Invoke(IMessage message)
        {
            var call = (IMethodCallMessage)message;
            if (call.MethodName == "Update") { Updates++; Updated?.Invoke(); }
            Type type = ((MethodInfo)call.MethodBase).ReturnType;
            object result = type == typeof(void) ? null : type == typeof(string) ? "Test hardware" :
                type.IsArray ? Array.CreateInstance(type.GetElementType(), 0) : type.IsValueType ? Activator.CreateInstance(type) : null;
            return new ReturnMessage(result, null, 0, call.LogicalCallContext, call);
        }
    }

    internal static async Task Run()
    {
        _checks = 0;
        string root = Path.Combine(Path.GetTempPath(), "TrayTemps-release-safety-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(root);
        try
        {
            await Files(root);
            Groups();
            await Polling();
            Settings(root);
            Check(OsdMetricDefinitions.Group(OsdPresets.LoadEmbedded(2), "GpuTemperature") == "GPU:",
                "Minimal transparent identifies its GPU group correctly through the active group name");
        }
        finally
        {
            string fullRoot = Path.GetFullPath(root);
            string temporaryPrefix = Path.GetFullPath(Path.GetTempPath()).TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar + "TrayTemps-release-safety-";
            if (!fullRoot.StartsWith(temporaryPrefix, StringComparison.OrdinalIgnoreCase)) throw new IOException("Refusing cleanup outside the temporary test directory.");
            Directory.Delete(fullRoot, true);
        }
        Console.WriteLine("ALL RELEASE SAFETY CHECKS PASSED: " + _checks);
    }

    private static string Installation(string root, string name)
    {
        string path = Path.Combine(root, name, "TrayTemps"); Directory.CreateDirectory(path);
        File.Copy(typeof(MainForm).Assembly.Location, Path.Combine(path, "TrayTemps.exe"));
        return path;
    }
    private static async Task Files(string root)
    {
        string invalid = Path.Combine(root, "unowned", "TrayTemps"); Directory.CreateDirectory(invalid);
        File.WriteAllText(Path.Combine(invalid, "TrayTemps.exe"), "not an executable");
        Check(!InstallationFileHelper.TryGetInstallDirectory(invalid, out _), "an unrelated file with the app filename is not accepted as an installation");
        string direct = Installation(root, "direct");
        Check(InstallationFileHelper.TryGetInstallDirectory(direct, out _), "a genuine legacy-compatible executable is accepted");
        File.WriteAllText(Path.Combine(direct, "test.tmp"), "user data");
        InstallationFileHelper.CheckWriteAccess(direct);
        Check(File.ReadAllText(Path.Combine(direct, "test.tmp")) == "user data" && Directory.GetFiles(direct, ".TrayTemps-write-*.tmp").Length == 0,
            "the write probe preserves an existing test.tmp and removes only its own file");
        Directory.CreateDirectory(Path.Combine(direct, "personal"));
        File.WriteAllText(Path.Combine(direct, "personal", "document.txt"), "keep");
        InstallationFileHelper.RemoveInstalledFiles(direct);
        Check(!File.Exists(Path.Combine(direct, "TrayTemps.exe")) && File.Exists(Path.Combine(direct, "test.tmp")) &&
            File.Exists(Path.Combine(direct, "personal", "document.txt")), "direct uninstall preserves unknown files and subdirectories");
        string empty = Installation(root, "empty"); InstallationFileHelper.RemoveInstalledFiles(empty);
        Check(!Directory.Exists(empty), "direct uninstall removes the containing directory when it becomes empty");

        string settings = Path.Combine(root, "settings"); Directory.CreateDirectory(settings);
        foreach (string name in new[] { "settings.json", "settings.json.tmp", "settings.json.bak", "My preset.json" })
            File.WriteAllText(Path.Combine(settings, name), "test");
        string runtime = Path.Combine(settings, "runtime"); Directory.CreateDirectory(runtime);
        foreach (string name in new[] { "TrayTemps.exe.config", "TrayTemps.exe.config.123.tmp", "TrayTemps.exe.config.custom.tmp", "TrayTemps.exe.config.0.tmp", "TrayTemps.exe.config.2147483648.tmp", "notes.txt" })
            File.WriteAllText(Path.Combine(runtime, name), "fixture");
        InstallationFileHelper.RemoveSettingsFiles(settings);
        Check(Directory.GetFiles(settings).Select(Path.GetFileName).SequenceEqual(new[] { "My preset.json" }),
            "settings cleanup preserves custom presets and removes only known settings files");
        Check(!File.Exists(Path.Combine(runtime, "TrayTemps.exe.config")) && !File.Exists(Path.Combine(runtime, "TrayTemps.exe.config.123.tmp")) &&
            File.Exists(Path.Combine(runtime, "TrayTemps.exe.config.custom.tmp")) && File.Exists(Path.Combine(runtime, "TrayTemps.exe.config.0.tmp")) &&
            File.Exists(Path.Combine(runtime, "TrayTemps.exe.config.2147483648.tmp")) && File.Exists(Path.Combine(runtime, "notes.txt")),
            "direct settings cleanup removes generated runtime files and preserves unknown runtime content");
        string ownedSettings = Path.Combine(root, "owned-settings"); Directory.CreateDirectory(Path.Combine(ownedSettings, "runtime"));
        File.WriteAllText(Path.Combine(ownedSettings, "settings.json"), "fixture");
        File.WriteAllText(Path.Combine(ownedSettings, "runtime", "TrayTemps.exe.config"), "fixture");
        File.WriteAllText(Path.Combine(ownedSettings, "runtime", "TrayTemps.exe.config.123.tmp"), "fixture");
        InstallationFileHelper.RemoveSettingsFiles(ownedSettings);
        Check(!Directory.Exists(ownedSettings), "direct uninstall removes the whole settings folder when only generated files remain");

        // Exercise the actual deferred script, including literal quote/percent/ampersand paths.
        string deferred = Installation(root, "deferred ' & % !");
        File.WriteAllText(Path.Combine(deferred, "document.txt"), "keep");
        string shortcut = Path.Combine(root, "test.lnk"); File.WriteAllText(shortcut, "fixture");
        File.WriteAllText(Path.Combine(settings, "settings.json"), "fixture");
        File.WriteAllText(Path.Combine(runtime, "TrayTemps.exe.config"), "fixture");
        File.WriteAllText(Path.Combine(runtime, "TrayTemps.exe.config.456.tmp"), "fixture");
        string script = Path.Combine(root, "cleanup.ps1");
        File.WriteAllText(script, InstallationFileHelper.BuildDeferredUninstallScript(deferred, settings, shortcut), new UTF8Encoding(true));
        using (var process = StartScript(script))
        {
            bool exited = await Task.Run(() => process.WaitForExit(20000));
            if (!exited) process.Kill();
            Check(exited && process.ExitCode == 0, "deferred uninstall executes successfully: " + process.StandardError.ReadToEnd());
        }
        Check(!File.Exists(Path.Combine(deferred, "TrayTemps.exe")) && File.Exists(Path.Combine(deferred, "document.txt")) &&
            File.Exists(Path.Combine(settings, "My preset.json")) && !File.Exists(Path.Combine(settings, "settings.json")) &&
            !File.Exists(shortcut) && !File.Exists(script), "deferred uninstall preserves unknown data and removes its own script");
        Check(!File.Exists(Path.Combine(runtime, "TrayTemps.exe.config")) && !File.Exists(Path.Combine(runtime, "TrayTemps.exe.config.456.tmp")) &&
            File.Exists(Path.Combine(runtime, "TrayTemps.exe.config.custom.tmp")) && File.Exists(Path.Combine(runtime, "TrayTemps.exe.config.0.tmp")) &&
            File.Exists(Path.Combine(runtime, "TrayTemps.exe.config.2147483648.tmp")) && File.Exists(Path.Combine(runtime, "notes.txt")),
            "deferred settings cleanup removes generated runtime files while preserving unknown files");

        string lockedDirectory = Installation(root, "locked");
        string marker = Path.Combine(root, "retry-observed.txt");
        script = Path.Combine(root, "locked-cleanup.ps1");
        string instrumented = InstallationFileHelper.BuildDeferredUninstallScript(lockedDirectory, settings, shortcut)
            .Replace("Start-Sleep -Milliseconds 1000", "[IO.File]::WriteAllText('" + marker.Replace("'", "''") + "', 'waiting'); Start-Sleep -Milliseconds 1000");
        File.WriteAllText(script, instrumented, new UTF8Encoding(true));
        using (var locked = new FileStream(Path.Combine(lockedDirectory, "TrayTemps.exe"), FileMode.Open, FileAccess.Read, FileShare.Read))
        using (var process = StartScript(script))
        {
            var timeout = Stopwatch.StartNew();
            while (!File.Exists(marker) && !process.HasExited && timeout.ElapsedMilliseconds < 10000) await Task.Delay(20);
            Check(File.Exists(marker) && File.Exists(Path.Combine(lockedDirectory, "TrayTemps.exe")), "deferred uninstall retries a locked executable instead of deleting its directory");
            locked.Dispose();
            bool exited = await Task.Run(() => process.WaitForExit(20000));
            if (!exited) process.Kill();
            Check(exited && process.ExitCode == 0 && !Directory.Exists(lockedDirectory), "deferred uninstall completes after the executable lock is released");
        }

        string runningDirectory = Path.Combine(root, "running", "TrayTemps"); Directory.CreateDirectory(runningDirectory);
        string runningExe = Path.Combine(runningDirectory, "TrayTemps.exe");
        using (var compiler = System.CodeDom.Compiler.CodeDomProvider.CreateProvider("CSharp"))
        {
            var parameters = new System.CodeDom.Compiler.CompilerParameters { GenerateExecutable = true, OutputAssembly = runningExe };
            var result = compiler.CompileAssemblyFromSource(parameters,
                "using System; using System.Reflection; [assembly: AssemblyCompany(\"NaeTech\")] [assembly: AssemblyProduct(\"Tray Temps\")] " +
                "class Fixture { static void Main() { Console.WriteLine(\"READY\"); Console.ReadLine(); } }");
            if (result.Errors.HasErrors) throw new Exception(string.Join(Environment.NewLine, result.Errors.Cast<System.CodeDom.Compiler.CompilerError>()));
        }
        string runningSettings = Path.Combine(root, "running-settings"); Directory.CreateDirectory(Path.Combine(runningSettings, "runtime"));
        File.WriteAllText(Path.Combine(runningSettings, "settings.json"), "fixture");
        File.WriteAllText(Path.Combine(runningSettings, "runtime", "TrayTemps.exe.config"), "fixture");
        string runningShortcut = Path.Combine(root, "running.lnk"); File.WriteAllText(runningShortcut, "fixture");
        marker = Path.Combine(root, "running-retry.txt"); script = Path.Combine(root, "running-cleanup.ps1");
        File.WriteAllText(script, InstallationFileHelper.BuildDeferredUninstallScript(runningDirectory, runningSettings, runningShortcut)
            .Replace("Start-Sleep -Milliseconds 1000", "[IO.File]::WriteAllText('" + marker.Replace("'", "''") + "', 'waiting'); Start-Sleep -Milliseconds 1000"), new UTF8Encoding(true));
        using (var running = Process.Start(new ProcessStartInfo(runningExe) { UseShellExecute = false, CreateNoWindow = true,
            WindowStyle = ProcessWindowStyle.Hidden, RedirectStandardInput = true, RedirectStandardOutput = true }))
        {
            try
            {
                var ready = running.StandardOutput.ReadLineAsync();
                Check(await Task.WhenAny(ready, Task.Delay(5000)) == ready && await ready == "READY", "controlled installed executable starts without hardware or saved preferences");
                bool denied = false;
                try { File.Delete(runningExe); } catch (UnauthorizedAccessException) { denied = true; }
                Check(denied, "a genuinely running executable reproduces access-denied deletion instead of a file-sharing IOException");
                using (var cleanup = StartScript(script))
                {
                    try
                    {
                        var timeout = Stopwatch.StartNew();
                        while (!File.Exists(marker) && !cleanup.HasExited && timeout.ElapsedMilliseconds < 10000) await Task.Delay(20);
                        Check(File.Exists(marker) && !cleanup.HasExited && File.Exists(Path.Combine(runningSettings, "settings.json")),
                            "deferred uninstall retries the running executable and waits before removing settings");
                        running.StandardInput.WriteLine();
                        Check(await Task.Run(() => running.WaitForExit(5000)), "controlled executable exits normally");
                        bool exited = await Task.Run(() => cleanup.WaitForExit(20000));
                        Check(exited && cleanup.ExitCode == 0 && !Directory.Exists(runningDirectory) && !Directory.Exists(runningSettings) &&
                            !File.Exists(runningShortcut) && !File.Exists(script), "deferred uninstall removes the executable, settings, runtime configuration and shortcut after process exit");
                    }
                    finally { if (!cleanup.HasExited) { cleanup.Kill(); cleanup.WaitForExit(); } }
                }
            }
            finally { if (!running.HasExited) { running.Kill(); running.WaitForExit(); } }
        }

        string changed = Installation(root, "changed");
        script = Path.Combine(root, "changed-cleanup.ps1");
        File.WriteAllText(script, InstallationFileHelper.BuildDeferredUninstallScript(changed, settings, shortcut), new UTF8Encoding(true));
        File.WriteAllText(Path.Combine(changed, "TrayTemps.exe"), "replacement is not TrayTemps");
        using (var process = StartScript(script))
        {
            bool exited = await Task.Run(() => process.WaitForExit(20000));
            if (!exited) process.Kill();
            Check(exited && process.ExitCode != 0 && File.Exists(Path.Combine(changed, "TrayTemps.exe")) &&
                File.Exists(Path.Combine(settings, "My preset.json")), "deferred uninstall refuses an executable whose identity changed before cleanup");
            Check(File.Exists(script + ".log") && !string.IsNullOrWhiteSpace(File.ReadAllText(script + ".log")) && !File.Exists(script),
                "failed deferred cleanup retains an error log after removing its temporary script");
        }

        string linkedSettings = Path.Combine(root, "linked-settings"); Directory.CreateDirectory(linkedSettings);
        string runtimeTarget = Path.Combine(root, "runtime-target"); Directory.CreateDirectory(runtimeTarget);
        string runtimeLink = Path.Combine(linkedSettings, "runtime");
        File.WriteAllText(Path.Combine(linkedSettings, "settings.json"), "fixture");
        File.WriteAllText(Path.Combine(runtimeTarget, "TrayTemps.exe.config"), "keep linked data");
        string linkScript = Path.Combine(root, "create-runtime-link.ps1");
        File.WriteAllText(linkScript, "$ErrorActionPreference = 'Stop'; New-Item -ItemType Junction -Path '" + runtimeLink.Replace("'", "''") +
            "' -Target '" + runtimeTarget.Replace("'", "''") + "' | Out-Null", new UTF8Encoding(true));
        try
        {
            using (var process = StartScript(linkScript))
            {
                bool exited = await Task.Run(() => process.WaitForExit(5000));
                if (!exited) { process.Kill(); process.WaitForExit(); }
                Check(exited && process.ExitCode == 0, "runtime-link safety fixture is created inside the temporary test directory");
            }
            bool refused = false;
            try { InstallationFileHelper.RemoveSettingsFiles(linkedSettings); } catch (IOException) { refused = true; }
            Check(refused && File.Exists(Path.Combine(linkedSettings, "settings.json")) && File.ReadAllText(Path.Combine(runtimeTarget, "TrayTemps.exe.config")) == "keep linked data",
                "direct settings cleanup refuses linked runtime directories before touching settings or target files");
            string directLinkInstall = Installation(root, "direct-linked-runtime");
            using (var host = new Host())
            {
                object[] arguments = { directLinkInstall, shortcut, linkedSettings, null };
                bool removed = (bool)typeof(MainForm).GetMethod("TryDirectUninstallInstalledCopy", Flags).Invoke(host, arguments);
                Check(!removed && File.Exists(Path.Combine(directLinkInstall, "TrayTemps.exe")) && !string.IsNullOrWhiteSpace((string)arguments[3]),
                    "direct uninstall validates settings before deleting the installed executable");
            }
            string linkInstall = Installation(root, "linked-runtime"); script = Path.Combine(root, "linked-runtime-cleanup.ps1");
            File.WriteAllText(script, InstallationFileHelper.BuildDeferredUninstallScript(linkInstall, linkedSettings, shortcut), new UTF8Encoding(true));
            using (var process = StartScript(script))
            {
                bool exited = await Task.Run(() => process.WaitForExit(20000));
                if (!exited) { process.Kill(); process.WaitForExit(); }
                Check(exited && process.ExitCode != 0 && File.Exists(Path.Combine(linkInstall, "TrayTemps.exe")) && File.Exists(Path.Combine(linkedSettings, "settings.json")) &&
                    File.ReadAllText(Path.Combine(runtimeTarget, "TrayTemps.exe.config")) == "keep linked data",
                    "deferred cleanup preflight preserves the executable, settings and linked target data");
            }
        }
        finally { if (Directory.Exists(runtimeLink)) Directory.Delete(runtimeLink, false); }
    }

    private static Process StartScript(string script) => Process.Start(new ProcessStartInfo
    {
        FileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), @"WindowsPowerShell\v1.0\powershell.exe"),
        Arguments = "-NoProfile -NonInteractive -ExecutionPolicy Bypass -File \"" + script + "\"",
        UseShellExecute = false, CreateNoWindow = true, WindowStyle = ProcessWindowStyle.Hidden, RedirectStandardError = true
    });

    private static void Groups()
    {
        var config = new OsdConfiguration
        {
            ShowCpu = false, ShowGpu = true, ShowCpuUsage = true,
            MetricGroups = new Dictionary<string, string> { ["CpuTemperature"] = "A", ["CpuUsage"] = "A", ["GpuTemperature"] = "B" },
            ItemOrder = "CpuTemperature,GpuTemperature,CpuUsage"
        };
        OsdMetricDefinitions.Migrate(config);
        Func<List<OsdMetric>> rows = () => OsdMetricDefinitions.Compose(config,
            OsdItemOrderHelper.Parse(config.ItemOrder).Select(kind => kind.ToString()).Where(key => OsdMetricDefinitions.IsVisible(config, key))
            .Select(key => new OsdMetric(key, "10") { Key = key, Style = config.MetricStyles[key] }));
        Check(rows().Select(row => row.Key).SequenceEqual(new[] { "Group:A", "Group:B" }), "a hidden first group member does not change group priority");
        config.ShowCpu = true;
        Check(rows()[0].Members.Select(metric => metric.Key).SequenceEqual(new[] { "CpuTemperature", "CpuUsage" }), "enabled group members retain their configured order");
        config.ShowCpu = config.ShowCpuUsage = false;
        Check(rows().Select(row => row.Key).SequenceEqual(new[] { "Group:B" }), "groups with no enabled members do not produce empty rows");
        config.ShowCpuUsage = true; config.ItemOrder = "GpuTemperature,CpuUsage,CpuTemperature";
        Check(rows().Select(row => row.Key).SequenceEqual(new[] { "Group:B", "Group:A" }), "changing configured group priority changes rendered priority");
    }

    private static async Task Polling()
    {
        foreach (var state in new[] { new[] { false, true }, new[] { true, false }, new[] { true, true } })
        using (var host = new Host())
        {
            host.Show();
            ((CheckBox)Field(host, "osdEnable").GetValue(host)).Checked = state[0];
            Field(host, "_osdConfiguration").SetValue(host, new OsdConfiguration { ShowRamUsage = state[1] });
            var computer = Field(host, "_computer"); computer.SetValue(host, Activator.CreateInstance(computer.FieldType));
            var memoryTask = Field(host, "_memoryHardwareTask"); Type type = memoryTask.FieldType.GetGenericArguments()[0];
            var memory = new Hardware(type); var cpu = new Hardware(type) { Updated = () => Field(host, "_isShutdownInitiated").SetValue(host, true) };
            Field(host, "_selectedCpuHardware").SetValue(host, cpu.GetTransparentProxy());
            object task = typeof(Task).GetMethod("FromResult").MakeGenericMethod(type).Invoke(null, new[] { memory.GetTransparentProxy() });
            memoryTask.SetValue(host, task); Field(host, "_temperatureTimerConfigured").SetValue(host, true);
            typeof(MainForm).GetMethod("TempTimer_Tick", Flags).Invoke(host, new object[] { null, EventArgs.Empty });
            var timeout = Stopwatch.StartNew();
            while ((bool)Field(host, "_isRefreshingTemps").GetValue(host) && timeout.ElapsedMilliseconds < 3000) await Task.Delay(5);
            Check(cpu.Updates == 1 && memory.Updates == (state[0] && state[1] ? 1 : 0),
                "RAM polling requires enabled OSD and RAM metric (OSD=" + state[0] + ", RAM=" + state[1] + ")");
        }
    }

    private static void Settings(string root)
    {
        string file = Path.Combine(root, "atomic-settings.json");
        SettingsFileHelper.WriteSettings(file, new AppSettings { IconSize = 90 });
        string before = File.ReadAllText(file);
        bool rejected = false;
        using (var locked = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read))
        {
            try { SettingsFileHelper.WriteSettings(file, new AppSettings { IconSize = 110 }); }
            catch (IOException) { rejected = true; }
        }
        Check(rejected && File.ReadAllText(file) == before && File.Exists(file + ".tmp"),
            "failed atomic settings replacement preserves both the old primary and the flushed recovery file");
        Check(SettingsFileHelper.ReadSettingsJson(file).Contains("110"), "settings recovery can read the new valid temporary candidate after replacement fails");
    }
}
