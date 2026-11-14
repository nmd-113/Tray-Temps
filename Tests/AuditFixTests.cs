using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using TrayTemps;

// These focused checks never enable discovery, modify user settings, or register a layer.
internal static class AuditFixTests
{
    private static void Check(bool condition, string description)
    {
        if (!condition) throw new Exception(description);
        Console.WriteLine("PASS: " + description);
    }

    internal static void Run()
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
    }
}
