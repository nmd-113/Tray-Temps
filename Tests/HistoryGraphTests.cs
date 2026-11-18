using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using TrayTemps;

internal static class HistoryGraphTests
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
    private static bool Ink(Color color) => color.R - color.G > 25 && color.B - color.G > 25;
    private static HistoryPoint Point(double time, double value, long source = 1, double? peak = null) =>
        new HistoryPoint { Time = time, Value = value, Peak = peak ?? value, Source = source };
    private static Rectangle Plot(HistoryGraphControl graph, Graphics graphics) =>
        (Rectangle)Call(graph, "GetPlotBounds", (int)Call(graph, "GetTextHeight", graphics));
    private static bool Column(Bitmap bitmap, Rectangle plot, int x) =>
        Enumerable.Range(plot.Top, plot.Height).Any(y => Ink(bitmap.GetPixel(x, y)));
    private static HistoryTrace Trace(HistoryPoint[] points) => new HistoryTrace { Name = "Trace", Color = Color.Magenta, Points = points };

    internal static void Run()
    {
        _checks = 0; Rendering(); RefreshAndPause(); BoundedWork();
        Console.WriteLine("ALL HISTORY GRAPH CHECKS PASSED: " + _checks);
    }

    private static void Rendering()
    {
        foreach (int dpi in new[] { 96, 144, 192 })
        foreach (bool light in new[] { false, true })
        using (var graph = new HistoryGraphControl { Size = new Size(500 * dpi / 96, 140 * dpi / 96),
            BackColor = light ? Color.White : Color.Black, ForeColor = light ? Color.Black : Color.White })
        {
            var handle = graph.Handle; Set(graph, "deviceDpi", dpi);
            using (var bitmap = new Bitmap(graph.Width, graph.Height))
            using (var graphics = Graphics.FromImage(bitmap))
            {
                var plot = Plot(graph, graphics);
                foreach (int count in new[] { 1, 2 })
                foreach (double value in new[] { 0d, 20d, 100d })
                {
                    graph.SetData("Metric", "%", 0, 60, new[] { Trace(count == 1 ? new[] { Point(60, value) } :
                        new[] { Point(50, value), Point(60, value) }) }, true);
                    graph.DrawToBitmap(bitmap, graph.ClientRectangle);
                    Check(Enumerable.Range(plot.Left, plot.Width).Any(x => Column(bitmap, plot, x)),
                        "history shows " + count + " sample(s) at " + value + "%, DPI=" + dpi + ", light=" + light);
                }
                graph.SetData("Metric", "ms", 0, 60, new[] { Trace(new[] { Point(10, 20), Point(20, 25), Point(30, 20) }) });
                graph.DrawToBitmap(bitmap, graph.ClientRectangle);
                int left = plot.Left + (int)Math.Ceiling(10d / 60 * (plot.Width - 1));
                int right = plot.Left + (int)(30d / 60 * (plot.Width - 1));
                Check(Enumerable.Range(left, right - left).All(x => Column(bitmap, plot, x)),
                    "valid irregular samples remain continuously connected without artificial buckets");
                foreach (var points in new[]
                {
                    new[] { Point(10, 20), Point(15, 20), Point(25, double.NaN), Point(35, 20), Point(40, 20) },
                    new[] { Point(10, 20), Point(15, 20), Point(35, 20, 2), Point(40, 20, 2) },
                    new[] { Point(5, 20), Point(10, 20), Point(40, 20), Point(45, 20) }
                })
                {
                    graph.SetData("Metric", "ms", 0, 60, new[] { Trace(points) });
                    graph.DrawToBitmap(bitmap, graph.ClientRectangle);
                    Check(!Column(bitmap, plot, plot.Left + (int)(25d / 60 * (plot.Width - 1))),
                        "missing readings, source changes and genuine long pauses remain gaps");
                }
                graph.SetData("Metric", "%", 0, 60, new[] { Trace(new[] { Point(30, 10, peak: 80), Point(30.001, 10) }) }, true);
                graph.DrawToBitmap(bitmap, graph.ClientRectangle);
                int peakX = plot.Left + (int)(.5 * (plot.Width - 1));
                Check(Enumerable.Range(plot.Top, plot.Height).Count(y => Ink(bitmap.GetPixel(peakX, y))) > plot.Height / 2,
                    "several samples in one pixel retain their peak extent");
            }
        }
    }

    private static void RefreshAndPause()
    {
        using (var host = new Host())
        {
            host.Show(); host.SetHistoryRange(1); host.SetHistoryRecording(true);
            Call(host, "RecordHardwareHistory", (float?)40, (float?)60);
            using (var dialog = new PerformanceHistoryDialog(host))
            {
                dialog.Show(host); Call(dialog, "RefreshGraphs");
                var graph = (HistoryGraphControl)dialog.Controls.Find("temperatureGraph", true).Single();
                var timer = (System.Windows.Forms.Timer)Get(dialog, "viewTimer");
                var points = ((HistoryTrace[])Get(graph, "_traces"))[0].Points;
                double end = (double)Get(graph, "_end");
                Thread.Sleep(10); Call(dialog, "ViewTimer_Tick", null, EventArgs.Empty);
                Check(timer.Interval == 100 && ReferenceEquals(points, ((HistoryTrace[])Get(graph, "_traces"))[0].Points) &&
                    (double)Get(graph, "_end") > end, "history scrolls at the faster presentation cadence without recopying samples");
                Call(host, "RecordHardwareHistory", (float?)50, (float?)70);
                Call(dialog, "ViewTimer_Tick", null, EventArgs.Empty);
                Check(ReferenceEquals(points, ((HistoryTrace[])Get(graph, "_traces"))[0].Points),
                    "new data respects the existing 500 ms snapshot-work budget");
                Set(dialog, "_lastDataRefresh", Stopwatch.GetTimestamp() - Stopwatch.Frequency);
                Call(dialog, "ViewTimer_Tick", null, EventArgs.Empty);
                Check(!ReferenceEquals(points, ((HistoryTrace[])Get(graph, "_traces"))[0].Points) &&
                    ((HistoryTrace[])Get(graph, "_traces"))[0].Points.Last().Value == 50,
                    "the next data refresh shows the newest hardware readings");
                Call(dialog, "Clear_Click", null, EventArgs.Empty);
                Check(((HistoryTrace[])Get(graph, "_traces"))[0].Points.Length == 0,
                    "Clear updates immediately without waiting for the snapshot throttle");
                var history = (PerformanceHistory)Get(host, "_history");
                double oldTime = Stopwatch.GetTimestamp() / (double)Stopwatch.Frequency - 61;
                history.Add("CpuTemperature", oldTime, 50);
                graph.SetData("Temperatures", "°C", oldTime - 59, oldTime + 1, new[] { Trace(new[] { Point(oldTime, 50) }) });
                Set(dialog, "_lastDataVersion", history.Version);
                Set(dialog, "_lastDataRefresh", Stopwatch.GetTimestamp() - Stopwatch.Frequency);
                Call(dialog, "ViewTimer_Tick", null, EventArgs.Empty);
                Check(((HistoryTrace[])Get(graph, "_traces"))[0].Points.Length == 0,
                    "expired cached samples leave the visible range even without a new source sample");
                Call(dialog, "Clear_Click", null, EventArgs.Empty);
                host.SetHistoryRecording(false); Call(dialog, "ViewTimer_Tick", null, EventArgs.Empty);
                var paused = Get(graph, "_traces"); end = (double)Get(graph, "_end");
                int invalidations = 0; graph.Invalidated += (s, e) => invalidations++;
                for (int i = 0; i < 10; i++) Call(dialog, "ViewTimer_Tick", null, EventArgs.Empty);
                Check(ReferenceEquals(paused, Get(graph, "_traces")) && (double)Get(graph, "_end") == end && invalidations == 0,
                    "paused history neither copies snapshots nor repaints an unchanged plot");
                host.SetHistoryRange(30); Call(dialog, "RefreshGraphs");
                Check(timer.Interval == 500, "thirty-minute plots retain the lower-cost presentation interval");
                host.SetHistoryRange(1); Call(dialog, "RefreshGraphs");
                Check(timer.Interval == 100, "short history ranges restore faster scrolling");
                paused = Get(graph, "_traces");
                dialog.Hide(); Call(host, "ClearHistory"); Call(dialog, "ViewTimer_Tick", null, EventArgs.Empty);
                Check(ReferenceEquals(paused, Get(graph, "_traces")) && !timer.Enabled,
                    "hidden history skips timer and snapshot work");
                dialog.Show(host);
                dialog.WindowState = FormWindowState.Minimized;
                Call(host, "ClearHistory"); Call(dialog, "ViewTimer_Tick", null, EventArgs.Empty);
                Check(ReferenceEquals(paused, Get(graph, "_traces")), "minimized history skips presentation work");
                dialog.WindowState = FormWindowState.Normal;
                Set(dialog, "_lastDataRefresh", 0L); Call(dialog, "ViewTimer_Tick", null, EventArgs.Empty);
                Check(!ReferenceEquals(paused, Get(graph, "_traces")), "restored history refreshes its pending data state");
                dialog.Close(); Check(dialog.IsDisposed && !timer.Enabled, "closing history disposes the faster view timer");
            }
            host.Close();
        }
    }

    private static void BoundedWork()
    {
        var history = new PerformanceHistory();
        history.Add("Fps", 1, 60); long version = history.Version;
        history.Add("Fps", 1, 90); history.Add("Unknown", 2, 100);
        Check(history.Version == version, "ignored samples do not invalidate cached history views");
        history.Pause("Fps", 3); version = history.Version; history.Pause("Fps", 4);
        Check(history.Version == version, "an already missing metric does not continually invalidate snapshots");
        history.Clear(); Check(history.Version != version && history.LastTime == 0, "Clear invalidates the view even when recording is paused");
        using (var graph = new HistoryGraphControl { Size = new Size(900, 140), BackColor = Color.Black, ForeColor = Color.White })
        using (var bitmap = new Bitmap(900, 140))
        {
            var points = Enumerable.Range(0, PerformanceHistory.Capacity).Select(i => Point(5000 + i * .25, 60 + i % 3, peak: i % 50 == 0 ? 180 : 65)).ToArray();
            graph.SetData("FPS", "FPS", 5000, 6800, new[] { Trace(points), Trace(points), Trace(points) });
            graph.DrawToBitmap(bitmap, graph.ClientRectangle);
            var watch = Stopwatch.StartNew();
            for (int i = 0; i < 30; i++) { graph.SetTimeRange(5000 + i * .1, 6800 + i * .1); graph.DrawToBitmap(bitmap, graph.ClientRectangle); }
            Console.WriteLine("HISTORY_30_MIN_THREE_TRACES mean_ms=" + (watch.Elapsed.TotalMilliseconds / 30).ToString("0.000"));
            Check(ReferenceEquals(((HistoryTrace[])Get(graph, "_traces"))[0].Points, points),
                "time-axis motion reuses the bounded thirty-minute sample array");
        }
    }
}
