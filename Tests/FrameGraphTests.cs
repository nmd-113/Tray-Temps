using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using TrayTemps;
using Timer = System.Windows.Forms.Timer;

internal static class FrameGraphTests
{
    private const BindingFlags Flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
    private static int _checks;
    private sealed class Host : MainForm { protected override void OnLoad(EventArgs e) { } }
    private static FieldInfo Field(object value, string name)
    {
        for (Type type = value.GetType(); type != null; type = type.BaseType)
        { var field = type.GetField(name, Flags); if (field != null) return field; }
        throw new MissingFieldException(name);
    }
    private static object Get(object value, string name) => Field(value, name).GetValue(value);
    private static void Set(object value, string name, object field) => Field(value, name).SetValue(value, field);
    private static object Call(object value, string name, params object[] args) =>
        (value is MainForm ? typeof(MainForm) : value.GetType()).GetMethod(name, Flags).Invoke(value, args);
    private static void Check(bool value, string message)
    { if (!value) throw new Exception(message); Console.WriteLine("PASS " + (++_checks) + ": " + message); }
    private static bool Ink(Color color) => color.R > 60 && color.B > 60 && color.G < 40;

    internal static void Run()
    {
        _checks = 0;
        ContinuousFrames(); CaptureReads(); CachedRendering(); Cadence();
        Console.WriteLine("ALL FRAMETIME GRAPH CHECKS PASSED: " + _checks);
    }

    private static void CaptureReads()
    {
        foreach (bool fallback in new[] { false, true })
        using (var monitor = new ForegroundFpsMonitor())
        using (var ready = new ManualResetEventSlim())
        {
            monitor.SetFrameStatisticsEnabled(false, true);
            Set(monitor, "_running", true); Set(monitor, "_targetGeneration", 1);
            Set(monitor, fallback ? "_fallbackLatestFps" : "_latestFps", 60);
            var timestamp = Field(monitor, fallback ? "_fallbackLastPresentTimestamp" : "_lastPresentTimestamp");
            int stop = 0;
            var producer = new Thread(() =>
            {
                while (Volatile.Read(ref stop) == 0)
                {
                    timestamp.SetValue(monitor, Stopwatch.GetTimestamp());
                    ready.Set();
                }
            }) { IsBackground = true, Name = "Controlled graph capture timestamps" };
            producer.Start();
            int unavailable = 0;
            try
            {
                if (!ready.Wait(5000)) throw new Exception("Capture fixture did not start");
                for (int i = 0; i < 50000; i++)
                {
                    if (((monitor.GetFrameStatistics(250, false).Source >> 28) & 15) != (fallback ? 2 : 1)) unavailable++;
                    monitor.GetLatestValues(out int? fps, out _);
                    if (fps != 60) unavailable++;
                }
            }
            finally { Volatile.Write(ref stop, 1); producer.Join(); }
            Check(unavailable == 0, "concurrent completed presents keep statistics and FPS available: " + unavailable +
                "/100000, fallback=" + fallback);
        }
    }

    private static void ContinuousFrames()
    {
        foreach (int interval in new[] { 200000, 28571 })
        {
            var window = new FrameTimeWindow(1000000);
            long time = 1000000; window.Reset(1, time);
            for (int i = 0; i < 150; i++) window.Add(time += i % 9 == 0 ? 250000 : interval, 1);
            var stats = window.GetStatistics(time, 1, 250, true);
            int first = Array.FindIndex(stats.Graph, point => !double.IsNaN(point.Mean));
            int last = Array.FindLastIndex(stats.Graph, point => !double.IsNaN(point.Mean));
            Check(stats.Graph.Skip(first).Take(last - first + 1).Any(point => double.IsNaN(point.Mean)),
                "fixture contains real long frames spanning empty 100 ms buckets at interval " + interval);
            using (var overlay = new OsdOverlay())
            foreach (float offset in new[] { 0f, .5f, 1.25f })
            using (var bitmap = new Bitmap(250, 40))
            using (var graphics = Graphics.FromImage(bitmap))
            {
                graphics.Clear(Color.Black);
                Call(overlay, "DrawFrameGraph", graphics, stats.Graph, new Rectangle(0, 0, 250, 40), Color.Magenta, offset);
                int gaps = 0;
                for (int x = Math.Max(2, 2 + (int)Math.Ceiling((first - offset) * 246d / 99));
                    x < 2 + (int)((last - offset) * 246d / 99); x++)
                    if (!Enumerable.Range(1, 38).Any(y => Ink(bitmap.GetPixel(x, y)))) gaps++;
                Check(gaps == 0, "completed frame samples form an uninterrupted line, offset=" + offset + ", interval=" + interval);
                Check(!Enumerable.Range(0, 40).Any(y => Ink(bitmap.GetPixel(0, y)) || Ink(bitmap.GetPixel(249, y))),
                    "scrolling stays clipped inside graph bounds");
                bitmap.Save(Path.Combine(Path.GetTempPath(), "TrayTemps-continuous-graph-" + interval + "-" + offset + ".png"));
            }
            Check(window.GetStatistics(time + 3100000, 1, 250, true).Graph == null,
                "stale capture clears the curve instead of extending old readings");
            window.Add(time += 3200000, 1); window.Add(time += 20000, 1);
            Check(window.GetStatistics(time, 1, 250, true).Graph.Count(point => !double.IsNaN(point.Mean)) == 1,
                "a genuine pause starts a fresh graph without joining the old stream");
            window.Reset(2, time); window.Add(time += 30000, 2);
            Check(window.GetStatistics(time, 2, 250, true).Graph.Count(point => !double.IsNaN(point.Mean)) == 1 &&
                window.GetStatistics(time, 1, 250, true).Graph == null,
                "target/source resets never bridge different capture generations");
        }
    }

    private static List<OsdMetric> Metrics(OsdConfiguration config, FrameGraphPoint[] graph, float offset, string value = "41°C") =>
        new List<OsdMetric>
        {
            new OsdMetric("CPU:", value) { Key = "CpuTemperature", Style = config.MetricStyles["CpuTemperature"], ValueWidthTemplate = "###°C" },
            new OsdMetric("", "") { Key = "FrameTimeGraph", Style = config.MetricStyles["FrameTimeGraph"],
                ValueWidthTemplate = "graph", Graph = graph, GraphOffset = offset }
        };

    private static byte[] Pixels(Bitmap bitmap)
    {
        var data = bitmap.LockBits(new Rectangle(Point.Empty, bitmap.Size), ImageLockMode.ReadOnly, PixelFormat.Format32bppPArgb);
        try { var bytes = new byte[data.Stride * data.Height]; Marshal.Copy(data.Scan0, bytes, 0, bytes.Length); return bytes; }
        finally { bitmap.UnlockBits(data); }
    }

    private static void MatchesFresh(OsdOverlay overlay, OsdConfiguration config, List<OsdMetric> metrics, string reason)
    {
        using (var fresh = new OsdOverlay())
        {
            fresh.Show(); fresh.UpdateDisplay(config, metrics, new Rectangle(0, 0, 1000, 800));
            Set(fresh, "deviceDpi", overlay.DeviceDpi);
            fresh.UpdateDisplay(config, metrics, new Rectangle(0, 0, 1000, 800));
            var actual = (Bitmap)Get(overlay, "_presentationBitmap");
            var expected = (Bitmap)Get(fresh, "_presentationBitmap");
            // Compare with the full, uncached drawing path, including alpha.
            using (var graphics = Graphics.FromImage(expected))
            {
                Call(fresh, "DrawBackground", graphics, expected.Size);
                Call(fresh, "DrawOverlay", graphics);
            }
            Check(actual.Size == expected.Size && Pixels(actual).SequenceEqual(Pixels(expected)), reason);
            fresh.Close();
        }
    }

    private static void CachedRendering()
    {
        using (var host = new Host())
        foreach (int opacity in new[] { 0, 50, 100 })
        foreach (int dpi in new[] { 96, 144, 192 })
        {
            var config = host.NormalizeOsdConfiguration(new OsdConfiguration { ShowFrameTimeGraph = true, FrameTimeGraphWidth = 250,
                FrameTimeGraphHeight = 30, BackgroundOpacityPercent = opacity, BackgroundCornerRoundness = 15 });
            config.MetricStyles["FrameTimeGraph"].ValueColor = Color.Magenta.ToArgb();
            var points = Enumerable.Range(0, 100).Select(i => new FrameGraphPoint(10 + i % 3, i == 50 ? 90 : 15)).ToArray();
            using (var overlay = new OsdOverlay())
            {
                overlay.Show();
                Set(overlay, "deviceDpi", dpi);
                overlay.UpdateDisplay(config, Metrics(config, points, 0), new Rectangle(0, 0, 1000, 800));
                var surface = Get(overlay, "_staticPresentationBitmap");
                var layout = ((System.Collections.IList)Get(overlay, "_metricLayouts"))[0];
                var initialPixels = Pixels((Bitmap)Get(overlay, "_presentationBitmap"));
                var metrics = Metrics(config, points, .65f);
                overlay.UpdateDisplay(config, metrics, new Rectangle(0, 0, 1000, 800));
                Check(ReferenceEquals(surface, Get(overlay, "_staticPresentationBitmap")) &&
                    ReferenceEquals(layout, ((System.Collections.IList)Get(overlay, "_metricLayouts"))[0]) &&
                    !initialPixels.SequenceEqual(Pixels((Bitmap)Get(overlay, "_presentationBitmap"))),
                    "sub-bucket scrolling reuses text/geometry and still updates pixels, opacity=" + opacity + ", DPI=" + dpi);
                MatchesFresh(overlay, config, metrics, "cached scrolling matches a fresh frame without alpha trails, opacity=" + opacity);
                points[50] = new FrameGraphPoint(20, 250);
                metrics = Metrics(config, (FrameGraphPoint[])points.Clone(), .9f);
                overlay.UpdateDisplay(config, metrics, new Rectangle(0, 0, 1000, 800));
                MatchesFresh(overlay, config, metrics, "new spikes rescale cleanly without leaving the previous graph behind");
                metrics = Metrics(config, points, .2f, "52°C");
                overlay.UpdateDisplay(config, metrics, new Rectangle(0, 0, 1000, 800));
                MatchesFresh(overlay, config, metrics, "numeric text refresh invalidates the cached text");
                config = config.Clone(); config.MetricStyles["CpuTemperature"].ValueColor = Color.Cyan.ToArgb();
                metrics = Metrics(config, points, .3f, "N/A");
                overlay.UpdateDisplay(config, metrics, new Rectangle(0, 0, 1000, 800));
                MatchesFresh(overlay, config, metrics, "N/A sizing and color changes invalidate cached geometry and text");
                var timer = Stopwatch.StartNew();
                for (int i = 0; i < 150; i++) overlay.UpdateDisplay(config, Metrics(config, points, i / 150f, "N/A"), new Rectangle(0, 0, 1000, 800));
                Console.WriteLine("GRAPH_REDRAW opacity=" + opacity + " DPI=" + dpi + " mean_ms=" + (timer.Elapsed.TotalMilliseconds / 150).ToString("0.000"));
                metrics = Metrics(config, points, 0, "52°C"); metrics.RemoveAt(1);
                overlay.UpdateDisplay(config, metrics, new Rectangle(0, 0, 1000, 800));
                Check(Get(overlay, "_staticPresentationBitmap") == null && Get(overlay, "_staticPresentationGraphics") == null,
                    "removing the graph releases its extra cache and uses ordinary text rendering");
                MatchesFresh(overlay, config, metrics, "disabling the graph leaves no cached graph pixels behind");
                overlay.Close();
                Check(Get(overlay, "_staticPresentationBitmap") == null && Get(overlay, "_staticPresentationGraphics") == null &&
                    Get(overlay, "_presentationBitmap") == null, "closing releases both graph and cached text surfaces");
            }
        }
    }

    private static OsdMetric Metric(MainForm host, string key) => ((List<OsdMetric>)Get(host, "_lastOsdMetrics")).Single(row => row.Key == key);

    private static void Cadence()
    {
        using (var host = new Host())
        {
            host.Show();
            var config = host.NormalizeOsdConfiguration(new OsdConfiguration { ShowCpu = false, ShowGpu = false, ShowFps = true,
                ShowFrameTimeGraph = true, FpsRefreshIntervalSeconds = 10m });
            var monitor = new ForegroundFpsMonitor();
            Set(monitor, "_running", true); Set(monitor, "_targetGeneration", 1); Set(monitor, "_targetProcessId", 123456789);
            monitor.SetFrameStatisticsEnabled(false, true);
            var window = (FrameTimeWindow)Get(Get(monitor, "_frameTimes"), "Primary");
            long now = Stopwatch.GetTimestamp(), time = now - Stopwatch.Frequency * 2;
            window.Reset(1, time);
            for (int i = 0; i < 120; i++) window.Add(time += Stopwatch.Frequency / 60, 1);
            Set(monitor, "_lastPresentTimestamp", now); Set(monitor, "_latestFps", 111);
            Set(host, "_fpsMonitor", monitor); Set(host, "_osdConfiguration", config);
            var enabled = (CheckBox)host.Controls.Find("osdEnable", true).Single(); enabled.Checked = true;
            Call(host, "UpdateOsd", null, null);
            Check(((Timer)Get(host, "_fpsTimer")).Interval == MainForm.FrameGraphRefreshIntervalMilliseconds,
                "only a visible frametime graph selects the fast presentation timer");
            var fps = Metric(host, "Fps"); var graph = Metric(host, "FrameTimeGraph");
            Set(host, "_lastFpsOsdRefresh", now); Set(host, "_lastFpsForegroundRefresh", now);
            Set(host, "_lastFrameStatisticsRefresh", now + Stopwatch.Frequency);
            Set(monitor, "_latestFps", 222);
            for (int i = 0; i < 5; i++) { Thread.Sleep(5); Call(host, "FpsTimer_Tick", null, EventArgs.Empty); }
            Check(ReferenceEquals(fps, Metric(host, "Fps")) && Metric(host, "Fps").Value == "111",
                "graph animation cannot bypass the selected ten-second numeric refresh");
            Check(ReferenceEquals(graph.Graph, Metric(host, "FrameTimeGraph").Graph) && graph.GraphOffset != Metric(host, "FrameTimeGraph").GraphOffset,
                "graph motion continues between capture snapshots without allocating another sample array");
            host.SetHistoryRecording(true); Call(host, "FpsTimer_Tick", null, EventArgs.Empty);
            var history = (PerformanceHistory)Get(host, "_history");
            int count = history.Snapshot("Fps", history.LastTime, 1).Length;
            Set(monitor, "_latestFps", 333); Call(host, "FpsTimer_Tick", null, EventArgs.Empty);
            Check(history.Snapshot("Fps", history.LastTime, 1).Length == count,
                "fast graph ticks do not accelerate history beyond its bounded recording cadence");
            Set(host, "_lastFrameHistoryRefresh", Stopwatch.GetTimestamp() - Stopwatch.Frequency);
            Call(host, "FpsTimer_Tick", null, EventArgs.Empty);
            Check(history.Snapshot("Fps", history.LastTime, 1).Last().Value == 333 && Metric(host, "Fps").Value == "111",
                "history still collects current values independently of held numeric text");
            enabled.Checked = false;
            Check(ReferenceEquals(monitor, Get(host, "_fpsMonitor")) && ((Timer)Get(host, "_fpsTimer")).Interval == 250,
                "OSD OFF removes fast presentation while preserving requested background capture");
            host.SetHistoryRecording(false);
            Check(Get(host, "_fpsMonitor") == null && !((Timer)Get(host, "_fpsTimer")).Enabled,
                "stopping the final consumer releases capture and its fast timer");
            host.Close();
        }
    }
}
