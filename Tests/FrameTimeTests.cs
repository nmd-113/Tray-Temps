using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using TrayTemps;

internal static class FrameTimeTests
{
    private const BindingFlags Fields = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
    private static void Check(bool value, string label)
    {
        if (!value) throw new Exception("FAILED: " + label);
        Console.WriteLine("PASS: " + label);
    }
    private static object Field(object target, string name) => target.GetType().GetField(name, Fields).GetValue(target);
    private static void Set(ForegroundFpsMonitor monitor, string name, object value) => typeof(ForegroundFpsMonitor).GetField(name, Fields).SetValue(monitor, value);
    private static FrameTimeWindow Window(ForegroundFpsMonitor monitor, string name) => (FrameTimeWindow)Field(Field(monitor, "_frameTimes"), name);
    private static long Fill(FrameTimeWindow window, long start, int count, long interval, int generation = 1)
    {
        window.Reset(generation, start);
        for (int i = 0; i < count; ++i) window.Add(start += interval, generation);
        return start;
    }

    internal static async Task Run()
    {
        MathChecks();
        await ConcurrentChecks();
        MonitorChecks();
        Performance();
    }

    private static async Task ConcurrentChecks()
    {
        var window = new FrameTimeWindow(1000000);
        int generation = 1, done = 0;
        long now = 1;
        var writer = Task.Run(() =>
        {
            try
            {
                for (int round = 1; round <= 30; ++round)
                {
                    System.Threading.Volatile.Write(ref generation, round);
                    window.Reset(round, System.Threading.Interlocked.Read(ref now));
                    for (int i = 0; i < 10000; ++i)
                    {
                        long timestamp = System.Threading.Interlocked.Add(ref now, 10000);
                        window.Add(timestamp, round);
                    }
                }
            }
            finally { System.Threading.Volatile.Write(ref done, 1); }
        });
        var reader = Task.Run(() =>
        {
            while (System.Threading.Volatile.Read(ref done) == 0)
            {
                int? value = window.GetValue(System.Threading.Interlocked.Read(ref now), System.Threading.Volatile.Read(ref generation));
                if (value.HasValue && value != 100) throw new Exception("Concurrent history mixed generations or corrupted intervals.");
            }
        });
        var timing = Task.Run(() =>
        {
            for (int i = 0; i < 300; i++) window.SetTimingEnabled((i & 1) == 0);
        });
        await Task.WhenAll(writer, reader, timing);
        window.Reset(31);
        Check(window.GetValue(now, 31) == null, "concurrent capture, snapshot sorting and source resets do not publish stale or mixed samples");
    }

    private static void MathChecks()
    {
        const long frequency = 1000000;
        var window = new FrameTimeWindow(frequency);
        long now = Fill(window, 1, 99, 20000);
        Check(window.GetValue(now, 1) == null, "1% low requires at least 100 valid intervals");
        window.Add(now += 20000, 1);
        Check(window.GetValue(now, 1) == 50, "20 ms slow-frame average produces exactly 50 FPS");
        Check(window.GetValue(now - 1, 1) == 50, "a Present arriving after the UI clock sample does not cause a false warm-up dash");
        window.Reset(1, now = 1);
        for (int i = 0; i < 1000; ++i) window.Add(now += i < 990 ? 1000 : 20000, 1);
        Check(window.GetValue(now, 1) == 50, "slowest ten of 1000 intervals are averaged, not FPS values or a percentile");
        window.Reset(1, now = 1);
        for (int i = 0; i < 101; ++i) window.Add(now += i == 99 ? 10000 : i == 100 ? 20000 : 1000, 1);
        Check(window.GetValue(now, 1) == 67, "slowest 1% count is rounded up (101 intervals select two)");
        now = Fill(window, 1, 100, 20000);
        Check(window.GetValue(now, 1) == 50, "cache starts with the expected slow-frame value");
        window.Add(now += 100000, 1);
        Check(window.GetValue(now, 1) == 50, "calculation is throttled to at most once per second");
        Check(window.GetValue(now + frequency, 1) == 17, "cached result refreshes after one second");
        now = Fill(window, 1, 100, 20000);
        for (int i = 0; i < 1000; ++i) window.Add(now += 10000, 1);
        Check(window.GetValue(now + 1, 1) == 100, "rolling ten-second window discards old slow frames");
        Check(window.GetValue(now + frequency * 4, 1) == null, "stale capture displays no low value");
        window.Add(now += frequency * 4, 1);
        Check(window.GetValue(now, 1) == null, "a stale gap resets history rather than becoming a slow frame");
        now = Fill(window, 1, 100, 20000, 2);
        window.Add(now - 50000, 2); window.Add(now, 2); window.Add(now + 100000, 1);
        Check(window.GetValue(now, 2) == 50, "duplicate, out-of-order and previous-process intervals are ignored");
        window.Reset(3);
        window.Add(now + 10000, 2);
        Check(window.GetValue(now + 10000, 3) == null, "reset prevents an old callback from repopulating the new generation");
        now = Fill(window, 1, FrameTimeWindow.Capacity + 100, 1, 3);
        Check(window.GetValue(now, 3) == null, "ring overflow never reports a truncated rolling window");
    }

    private sealed class PresentReplay : IDisposable
    {
        internal readonly ForegroundFpsMonitor Monitor = new ForegroundFpsMonitor();
        private readonly IntPtr _record = Marshal.AllocHGlobal(112), _data = Marshal.AllocHGlobal(16);
        internal readonly Action<IntPtr> Callback;
        internal PresentReplay(bool collect)
        {
            Marshal.Copy(new byte[112], 0, _record, 112);
            Marshal.Copy(new byte[16], 0, _data, 16);
            Marshal.WriteInt32(_record, 8, 123); Marshal.WriteInt32(_record, 12, 777);
            Marshal.WriteInt16(_record, 40, 0x2a);
            Marshal.Copy(new Guid("CA11C036-0102-4A2D-A6AD-F03CFED5D3C9").ToByteArray(), 0, IntPtr.Add(_record, 24), 16);
            Marshal.WriteInt16(_record, 86, 12); Marshal.WriteIntPtr(_record, 96, _data);
            Marshal.WriteInt64(_data, 1);
            Set(Monitor, "_targetProcessId", 777); Set(Monitor, "_targetGeneration", 1); Set(Monitor, "_running", true);
            Monitor.SetFrameStatisticsEnabled(collect, false);
            Callback = (Action<IntPtr>)Delegate.CreateDelegate(typeof(Action<IntPtr>), Monitor,
                typeof(ForegroundFpsMonitor).GetMethod("OnEventRecord", Fields));
        }
        internal void Present(long timestamp, long swapChain = 1, int flags = 0)
        {
            Marshal.WriteInt64(_record, 16, timestamp); Marshal.WriteInt64(_data, swapChain); Marshal.WriteInt32(_data, 8, flags);
            Callback(_record);
        }
        internal void Replay(long start, int count, long step)
        {
            for (int i = 0; i < count; ++i) { Marshal.WriteInt64(_record, 16, start + i * step); Callback(_record); }
        }
        public void Dispose() { Monitor.Dispose(); Marshal.FreeHGlobal(_record); Marshal.FreeHGlobal(_data); }
    }

    private static void MonitorChecks()
    {
        using (var off = new PresentReplay(false))
        using (var on = new PresentReplay(true))
        {
            Check(Field(off.Monitor, "_frameTimes") == null, "option OFF allocates no history or scratch arrays");
            long frequency = Stopwatch.Frequency, now = Stopwatch.GetTimestamp(), start = now - frequency * 2;
            off.Replay(start, 101, frequency / 50); on.Replay(start, 101, frequency / 50);
            off.Monitor.GetLatestValues(out int? original, out int? absent);
            on.Monitor.GetLatestValues(out int? fps, out int? low);
            Check(original == fps && fps == 50 && absent == null && low == 50, "accepted ETW Presents supply 1% low without changing regular FPS");
            var primary = Window(on.Monitor, "Primary");
            on.Present(now + frequency / 100, flags: 1);
            Check(primary.GetValue(now + frequency / 100, 1) == 50, "DXGI test presentations do not contaminate history");
            var fallback = Window(on.Monitor, "Fallback");
            Fill(fallback, start, 100, frequency / 50);
            Set(on.Monitor, "_lastPresentTimestamp", now - frequency * 4);
            Set(on.Monitor, "_fallbackLatestFps", 50); Set(on.Monitor, "_fallbackLastPresentTimestamp", now);
            on.Monitor.GetLatestValues(out fps, out low);
            Check(fps == 50 && low == null, "switch to fallback clears its prior low-value history");
            Fill(fallback, start, 100, frequency / 50);
            on.Monitor.GetLatestValues(out fps, out low);
            Check(low == 50, "fallback supplies a value after its own fresh warm-up");
            Set(on.Monitor, "_lastPresentTimestamp", now);
            on.Monitor.GetLatestValues(out fps, out low);
            Check(fps == 50 && low == null, "return to primary never reuses its old low value");
            Fill(primary, start, 100, frequency / 50);
            on.Present(now + frequency, swapChain: 2);
            Check(primary.GetValue(now + frequency, 1) == null, "accepted swap-chain changes reset the low history");
            on.Monitor.UpdateTargetProcess(123456789);
            Check((int)Field(on.Monitor, "_targetGeneration") > 1 &&
                primary.GetValue(now + frequency, (int)Field(on.Monitor, "_targetGeneration")) == null,
                "foreground process changes immediately invalidate the previous history");
            on.Monitor.SetFrameStatisticsEnabled(false, false);
            Check(Field(on.Monitor, "_frameTimes") == null, "disabling the option releases optional history");
            on.Monitor.SetFrameStatisticsEnabled(true, false);
            Check(Window(on.Monitor, "Primary").GetValue(now, 1) == null, "re-enabling starts with fresh samples");
            on.Monitor.Stop();
            Check(Field(on.Monitor, "_frameTimes") == null && on.Monitor.GetLatestFps() == null, "OSD monitoring stop drops history and cached values");
        }
    }

    private sealed class Host : MainForm { protected override void OnLoad(EventArgs e) { } }
    private static object HostField(MainForm host, string name) => typeof(MainForm).GetField(name, Fields).GetValue(host);
    private static void UpdateOsd(MainForm host) => typeof(MainForm).GetMethod("UpdateOsd", Fields).Invoke(host, new object[] { (float?)50, (float?)60 });

    private static OsdMetric LastMetric(MainForm host, string key) =>
        ((System.Collections.Generic.List<OsdMetric>)HostField(host, "_lastOsdMetrics"))
            .SelectMany(row => row.Members ?? new System.Collections.Generic.List<OsdMetric> { row }).FirstOrDefault(metric => metric.Key == key);
    internal static async Task Live(string executable)
    {
        Check(File.Exists(executable), "live graphics fixture exists");
        string evidence = Path.Combine(Path.GetTempPath(), "TrayTemps-one-percent-live-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(evidence); Console.WriteLine("EVIDENCE=" + evidence);
        using (var host = new Host())
        using (var game = new Process { StartInfo = new ProcessStartInfo(executable) { UseShellExecute = false, RedirectStandardOutput = true, CreateNoWindow = true } })
        {
            var lines = new System.Collections.Generic.List<string>();
            game.OutputDataReceived += (s, e) => { if (e.Data != null) lock (lines) lines.Add(e.Data); };
            host.Show();
            var config = new OsdConfiguration { ShowCpu = false, ShowGpu = false, ShowFps = true, FontSize = 32f, FpsRefreshIntervalSeconds = 0.25m };
            typeof(MainForm).GetField("_osdConfiguration", Fields).SetValue(host, config);
            var enabled = (CheckBox)HostField(host, "osdEnable");
            try
            {
                game.Start(); game.BeginOutputReadLine();
                await Task.Delay(600); await Suite.SelectForeground(game);
                enabled.Checked = true; UpdateOsd(host);
                await Task.Delay(2000);
                var monitor = (ForegroundFpsMonitor)HostField(host, "_fpsMonitor");
                Check(monitor != null && monitor.GetLatestFps().HasValue && (int)Field(monitor, "_targetProcessId") == game.Id,
                    "existing ETW session supplies FPS for the real foreground fixture");
                Check(Field(monitor, "_frameTimes") == null, "live baseline has no optional history");
                config.ShowFpsOnePercentLow = true; UpdateOsd(host);
                await Task.Delay(2200);
                monitor.GetLatestValues(out int? fps, out int? low);
                var metric = (OsdMetric)LastMetric(host, "FpsOnePercentLow");
                Check(fps.HasValue && low.HasValue && metric.Label == "1%Low" && metric.Value != "N/A",
                    "live ETW timings populate 1%Low while the FPS limiter is disabled");
                Console.WriteLine("LIVE_FPS=" + fps + " ONE_PERCENT_LOW=" + low + " TEXT=" + metric.Value + " " + metric.Label + " " + metric.Value);
                Check(HostField(host, "_fpsLimiterService") == null && ReferenceEquals(monitor, HostField(host, "_fpsMonitor")),
                    "enabling the low metric reuses the FPS session and creates no limiter");
                config.ShowFps = false; UpdateOsd(host);
                await Task.Delay(1100);
                Check(LastMetric(host, "Fps") == null && LastMetric(host, "FpsOnePercentLow").Value != "N/A" &&
                    ReferenceEquals(monitor, HostField(host, "_fpsMonitor")), "standalone 1% Low refreshes with FPS hidden using the same ETW session");
                config.ShowFps = true;
                config.MetricGroups["Fps"] = "CPU"; config.MetricGroups["FpsOnePercentLow"] = "GPU";
                config.ShowCpu = true; config.ShowGpu = true; config.MetricGroups["CpuTemperature"] = "CPU"; config.MetricGroups["GpuTemperature"] = "GPU";
                UpdateOsd(host); await Task.Delay(600);
                Check(LastMetric(host, "CpuTemperature").Value == "50°C" && LastMetric(host, "GpuTemperature").Value == "60°C" &&
                    LastMetric(host, "FpsOnePercentLow").Value != "N/A", "FPS refresh preserves hardware values in two separate arbitrary groups");
                config.ShowCpu = false; config.ShowGpu = false; config.MetricGroups.Clear();
                config.LabelOverrides = new System.Collections.Generic.Dictionary<string, string> { ["Fps"] = "", ["FpsOnePercentLow"] = "Slow" };
                UpdateOsd(host);
                metric = (OsdMetric)LastMetric(host, "FpsOnePercentLow");
                Check(LastMetric(host, "Fps").Label == "" && metric.Label == "Slow" && metric.Value != null,
                    "live custom low label and blank FPS label retain both measurements");
                config.LabelOverrides["FpsOnePercentLow"] = ""; UpdateOsd(host);
                metric = (OsdMetric)LastMetric(host, "FpsOnePercentLow");
                Check(LastMetric(host, "Fps").Label == "" && metric.Label == "" && metric.Value != null,
                    "live blank labels retain both FPS values without an additional caption");
                config.LabelOverrides.Clear(); UpdateOsd(host);
                var overlay = (OsdOverlay)HostField(host, "_osdOverlay");
                Size stableSize = overlay.Size;
                using (var bitmap = new Bitmap(overlay.Width, overlay.Height))
                {
                    using (var graphics = Graphics.FromImage(bitmap)) graphics.CopyFromScreen(overlay.Location, Point.Empty, overlay.Size);
                    bitmap.Save(Path.Combine(evidence, "one-percent-osd.png"));
                }
                await Task.Delay(1100);
                Check(overlay.Size == stableSize, "FPS and low refreshes keep overlay geometry stable");
                enabled.Checked = false;
                Check(HostField(host, "_fpsMonitor") == null && HostField(host, "_osdOverlay") == null && Field(monitor, "_frameTimes") == null,
                    "live OSD OFF stops capture and drops low history");
                await Task.Delay(250);
                enabled.Checked = true; UpdateOsd(host);
                var replacement = (ForegroundFpsMonitor)HostField(host, "_fpsMonitor");
                Check(replacement != null && !ReferenceEquals(replacement, monitor), "OSD ON creates a fresh monitoring session");
                await Task.Delay(2200);
                replacement.GetLatestValues(out fps, out low);
                Check(fps.HasValue && low.HasValue, "OSD OFF/ON warms up a fresh low metric without stale values");
                config.ShowFpsOnePercentLow = false; UpdateOsd(host);
                Check(Field(replacement, "_frameTimes") == null && LastMetric(host, "FpsOnePercentLow") == null,
                    "live option OFF removes extra text and releases history while FPS continues");
                Check(replacement.GetLatestFps().HasValue, "ordinary FPS remains available after low collection is disabled");
            }
            finally
            {
                enabled.Checked = false;
                if (game.Id > 0 && !game.HasExited) { game.Kill(); game.WaitForExit(); }
                lock (lines) File.WriteAllLines(Path.Combine(evidence, "graphics.log"), lines);
                host.Close();
            }
        }
    }

    internal static void TextChecks()
    {
        foreach (string family in new[] { OsdFontHelper.DefaultFamily, "JetBrains Mono", "Segoe UI", "Arial", "Consolas" })
        foreach (float size in new[] { 16f, 24f, 32f, 48f })
        using (var overlay = new OsdOverlay())
        using (var bitmap = new Bitmap(1, 1))
        {
            var configuration = new OsdConfiguration { FontFamily = family, FontSize = size };
            var metric = new OsdMetric("FPS", "144", Color.White)
            {
                ValueWidthTemplate = "####",
                LabelValueSpacingOverride = 15
            };
            overlay.UpdateDisplay(configuration, new[] { metric }, new Rectangle(0, 0, 4000, 2000));
            bitmap.SetResolution(overlay.DeviceDpi, overlay.DeviceDpi);
            using (var graphics = Graphics.FromImage(bitmap))
            {
                var measure = (Func<Graphics, string, int>)Delegate.CreateDelegate(typeof(Func<Graphics, string, int>), overlay,
                    typeof(OsdOverlay).GetMethod("MeasureDisplayTextWidth", Fields));
                int available = (int)typeof(OsdOverlay).GetMethod("MeasureStableValueTemplateWidth", Fields).Invoke(overlay, new object[] { graphics, "####" });
                int maximum = 0; string worst = "";
                for (int value = 0; value <= 9999; ++value)
                {
                    string text = value.ToString(System.Globalization.CultureInfo.InvariantCulture);
                    int width = measure(graphics, text);
                    if (width > maximum) { maximum = width; worst = text; }
                }
                maximum = Math.Max(maximum, measure(graphics, "N/A"));
                Check(maximum <= available, family + " " + size + " pt fits all low values: worst=" + worst + " required=" + maximum + " reserved=" + available);
                var originalPath = (Func<Graphics, string, System.Drawing.Drawing2D.GraphicsPath>)Delegate.CreateDelegate(
                    typeof(Func<Graphics, string, System.Drawing.Drawing2D.GraphicsPath>), overlay, typeof(OsdOverlay).GetMethod("CreateTextPath", Fields));
                var fittedPath = (Func<Graphics, string, float, System.Drawing.Drawing2D.GraphicsPath>)Delegate.CreateDelegate(
                    typeof(Func<Graphics, string, float, System.Drawing.Drawing2D.GraphicsPath>), overlay, typeof(OsdOverlay).GetMethod("CreateFittedTextPath", Fields));
                bool complete = true;
                foreach (string text in new[] { "6007", "1001", "9999", "0", "N/A", "\u00b7 1%Low" })
                using (var raw = originalPath(graphics, text))
                using (var fitted = fittedPath(graphics, text, raw.GetBounds().Width / 2f))
                {
                    complete &= raw.PointCount == fitted.PointCount && raw.PathTypes.SequenceEqual(fitted.PathTypes);
                    complete &= fitted.GetBounds().Width <= raw.GetBounds().Width / 2f + 0.01f;
                    PointF[] source = raw.PathPoints, destination = fitted.PathPoints;
                    for (int point = 0; point < Math.Min(source.Length, destination.Length); ++point)
                        complete &= Math.Abs(destination[point].X - source[point].X / 2f) < 0.01f &&
                            Math.Abs(destination[point].Y - source[point].Y / 2f) < 0.01f;
                }
                Check(complete, family + " " + size + " pt constrained low text preserves every glyph without ellipsis");
            }
        }
    }

    private static void Performance()
    {
        const int count = 300000;
        using (var off = new PresentReplay(false))
        using (var on = new PresentReplay(true))
        {
            off.Replay(1, 1000, 100); on.Replay(1, 1000, 100);
            long clock = Stopwatch.Frequency;
            var baseline = new double[5]; var enabled = new double[5];
            int collections = GC.CollectionCount(0);
            for (int i = 0; i < 5; ++i)
            {
                long start = Stopwatch.GetTimestamp(); off.Replay(1000000L + i * count * 100L, count, 100);
                baseline[i] = (Stopwatch.GetTimestamp() - start) * 1e9 / clock / count;
                start = Stopwatch.GetTimestamp(); on.Replay(1000000L + i * count * 100L, count, 100);
                enabled[i] = (Stopwatch.GetTimestamp() - start) * 1e9 / clock / count;
            }
            Array.Sort(baseline); Array.Sort(enabled);
            Console.WriteLine("PERF_PRESENT_OFF_NS=" + baseline[2].ToString("F1") + " ON_NS=" + enabled[2].ToString("F1") +
                " ADDED_NS=" + Math.Max(0, enabled[2] - baseline[2]).ToString("F1") + " GEN0_COLLECTIONS=" + (GC.CollectionCount(0) - collections));
        }
        foreach (int rate in new[] { 60, 144, 1000, 9999 })
        {
            var window = new FrameTimeWindow(1000000);
            long end = 1; uint random = 42;
            window.Reset(1, end);
            long period = 1000000 / rate;
            for (int i = 0; i < rate * 10; ++i)
            {
                random = unchecked(random * 1664525 + 1013904223);
                long interval = period * 3 / 4 + (random % (uint)Math.Max(1, period / 2));
                if (i % 100 == 0) interval *= 2;
                window.Add(end += interval, 1);
            }
            long start = Stopwatch.GetTimestamp();
            int? value = window.GetValue(end, 1);
            double ms = (Stopwatch.GetTimestamp() - start) * 1000.0 / Stopwatch.Frequency;
            Check(value.HasValue, "bounded history calculation succeeds at " + rate + " FPS");
            Console.WriteLine("PERF_SORT_FPS=" + rate + " SAMPLES=" + rate * 10 + " MS=" + ms.ToString("F3"));
        }
    }
}
