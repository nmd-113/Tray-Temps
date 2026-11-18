using System;
using System.Collections;
using System.Collections.Generic;
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

internal static class PerformanceHistoryTests
{
    private const BindingFlags Flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
    [DllImport("user32.dll")] private static extern IntPtr SetThreadDpiAwarenessContext(IntPtr context);
    private static int _checks;
    private static void Check(bool value, string message) { if (!value) throw new Exception(message); Console.WriteLine("PASS " + (++_checks) + ": " + message); }
    private static FieldInfo Find(object value, string name)
    {
        for (Type type = value.GetType(); type != null; type = type.BaseType) { var field = type.GetField(name, Flags); if (field != null) return field; }
        throw new MissingFieldException(name);
    }
    private static object Get(object value, string name) => Find(value, name).GetValue(value);
    private static void Set(object value, string name, object field) => Find(value, name).SetValue(value, field);
    private static object Call(object value, string name, params object[] args)
    {
        for (Type type = value.GetType(); type != null; type = type.BaseType) { var method = type.GetMethod(name, Flags); if (method != null) return method.Invoke(value, args); }
        throw new MissingMethodException(name);
    }
    private static T Control<T>(Form form, string name) where T : Control => (T)form.Controls.Find(name, true).Single();
    private sealed class Host : MainForm { protected override void OnLoad(EventArgs e) { } }

    internal static async Task Run()
    {
        _checks = 0; Statistics(); OptionalTiming(); Store(); Models(); GraphDpi();
        using (var host = new Host()) { host.Show(); await Ui(host); RefreshCadence(host); host.Close(); }
        Console.WriteLine("ALL PERFORMANCE HISTORY CHECKS PASSED: " + _checks);
    }

    internal static async Task Live(string executable)
    {
        _checks = 0;
        using (var host = new Host())
        using (var game = new Process { StartInfo = new ProcessStartInfo(executable) { UseShellExecute = false, CreateNoWindow = true } })
        {
            bool started = false;
            host.Show();
            var config = host.NormalizeOsdConfiguration(new OsdConfiguration { ShowCpu = false, ShowGpu = false, ShowFps = true,
                ShowFpsOnePercentLow = true, ShowAverageFps = true, ShowFrameTime = true, ShowFrameTimeGraph = true, FpsRefreshIntervalSeconds = .25m });
            Set(host, "_osdConfiguration", config);
            try
            {
                started = game.Start();
                host.SetHistoryRecording(true);
                await Task.Delay(700); await Suite.SelectForeground(game);
                var monitor = (ForegroundFpsMonitor)Get(host, "_fpsMonitor");
                var stats = default(FrameStatistics);
                for (int attempt = 0; attempt < 3; attempt++)
                {
                    monitor?.UpdateForegroundProcess();
                    await Task.Delay(2500);
                    stats = (FrameStatistics)Get(host, "_latestFrameStatistics");
                    if (monitor != null && (int)Get(monitor, "_targetProcessId") == game.Id && stats.AverageFps > 0 && stats.FrameTimeMs > 0) break;
                    await Suite.SelectForeground(game);
                }
                var store = (PerformanceHistory)Get(host, "_history");
                Console.WriteLine("LIVE monitor=" + (monitor != null) + " target=" + (monitor == null ? "none" : Get(monitor, "_targetProcessId")) +
                    " expected=" + game.Id + " fps=" + monitor?.GetLatestFps() + " average=" + stats.AverageFps + " frametime=" + stats.FrameTimeMs);
                Check(monitor != null && monitor.GetLatestFps().HasValue && stats.AverageFps > 0 && stats.FrameTimeMs > 0,
                    "live ETW capture populates average FPS and frametime with OSD and limiter disabled");
                Check(stats.Graph == null && Get(host, "_fpsLimiterService") == null,
                    "background-only recording creates neither graph snapshots nor a limiter");
                Check(store.Snapshot("FpsOnePercentLow", store.LastTime, 1).Any(point => !double.IsNaN(point.Value)),
                    "shared capture records real 1% Low alongside the new metrics");
                Call(host, "HistoryButton_Click", null, EventArgs.Empty);
                await Task.Delay(700);
                Check((int)Get(monitor, "_targetProcessId") == game.Id, "opening the history window preserves the live external target");
                ((PerformanceHistoryDialog)Get(host, "_historyDialog")).Close();
                int before = store.Snapshot("AverageFps", store.LastTime, 1).Length;
                await Task.Delay(1100);
                Check(store.Snapshot("AverageFps", store.LastTime, 1).Length > before,
                    "live average FPS recording continues after the history window closes");
                Control<CheckBox>(host, "osdEnable").Checked = true;
                Call(host, "UpdateOsd", null, null);
                await Task.Delay(700);
                var rows = (System.Collections.Generic.List<OsdMetric>)Get(host, "_lastOsdMetrics");
                Check(rows.Any(metric => metric.Key == "FrameTimeGraph" && metric.Graph?.Length == 100),
                    "live OSD sparkline uses one hundred current buckets from the existing session");
                Check(ReferenceEquals(monitor, Get(host, "_fpsMonitor")), "OSD and background history retain a single capture monitor");
                Control<CheckBox>(host, "osdEnable").Checked = false;
                Check(ReferenceEquals(monitor, Get(host, "_fpsMonitor")), "OSD OFF keeps the live history session running");
                host.SetHistoryRecording(false);
                Check(Get(host, "_fpsMonitor") == null, "stopping the final live consumer releases the capture session");
                Console.WriteLine("ALL LIVE PERFORMANCE HISTORY CHECKS PASSED: " + _checks);
            }
            finally
            {
                host.SetHistoryRecording(false);
                Control<CheckBox>(host, "osdEnable").Checked = false;
                host.Close();
                if (started && !game.HasExited) { game.Kill(); game.WaitForExit(); }
            }
        }
    }

    internal static async Task AuditUi()
    {
        _checks = 0;
        using (var host = new Host())
        {
            host.Show();
            var config = host.NormalizeOsdConfiguration(new OsdConfiguration());
            foreach (string key in OsdMetricDefinitions.Keys) OsdMetricDefinitions.SetVisible(config, key, true);
            Set(host, "_osdConfiguration", config);
            var store = (PerformanceHistory)Get(host, "_history");
            double now = Stopwatch.GetTimestamp() / (double)Stopwatch.Frequency;
            foreach (string key in OsdMetricDefinitions.Keys)
                for (int i = 0; i < 240; i++) store.Add(key, now - 60 + i / 4d, key == "FrameTime" ? 16 : 60 + i % 20);
            foreach (bool light in new[] { false, true })
            {
                Control<CheckBox>(host, "lightModeSwitch").Checked = light;
                Call(host, "ApplyTheme");
                var theme = ThemePalette.Create(light);
                var heading = Control<Button>(host, "historyButton").Parent;
                using (var image = new Bitmap(heading.Width, heading.Height))
                { heading.DrawToBitmap(image, new Rectangle(Point.Empty, image.Size)); image.Save(Path.Combine(Path.GetTempPath(), "TrayTemps-audit-heading-" + light + ".png")); }
                using (var history = new PerformanceHistoryDialog(host))
                {
                    history.Show(host);
                    var titleBar = Control<Panel>(history, "titleBar");
                    var title = Control<Label>(history, "title");
                    var exit = Control<Button>(history, "exitBtn");
                    var footer = Control<Panel>(history, "footer");
                    var grip = Control<WindowResizeGripPanel>(history, "resizeGrip");
                    Check(history.FormBorderStyle == FormBorderStyle.None && history.IsHandleCreated &&
                        titleBar.Height >= 30 && titleBar.Height == exit.Height && title.Text == "Performance history" &&
                        exit.AccessibleName == "Close performance history",
                        "history standard title bar values: border=" + history.FormBorderStyle + ", handle=" + history.IsHandleCreated +
                        ", height=" + titleBar.Height + ", title=" + title.Text + ", close=" + exit.AccessibleName);
                    Check(grip.Parent == footer && grip.Cursor == Cursors.SizeNWSE &&
                        grip.Right == footer.ClientSize.Width && grip.Bottom == footer.ClientSize.Height,
                        "history resize grip geometry: parent=" + (grip.Parent == footer) + ", grip=" + grip.Bounds +
                        ", footer=" + footer.ClientSize);
                    foreach (Size size in new[] { new Size(600, 400), new Size(900, 700), new Size(1400, 900) })
                    {
                        history.Size = size; await Task.Delay(30);
                        var content = Control<Panel>(history, "content");
                        var range = Control<ComboBox>(history, "range");
                        var clear = Control<Button>(history, "clear");
                        Check(!content.HorizontalScroll.Visible,
                            "history scrolls vertically without horizontal overflow at " + size + ", light=" + light);
                        Check(range.Bounds.IntersectsWith(clear.Bounds) == false && Math.Abs(range.Top + range.Height / 2 - clear.Top - clear.Height / 2) <= 1,
                            "history header controls share a vertical center without overlap at " + size);
                        Check(Control<Label>(history, "rangeLabel").PreferredSize.Width <= Control<Label>(history, "rangeLabel").Width,
                            "history range label fits on one line at " + size);
                        foreach (string name in new[] { "temperatureGraph", "usageGraph", "fpsGraph", "memoryGraph", "frameGraph" })
                            Check(Control<HistoryGraphControl>(history, name).Width == Control<HistoryGraphControl>(history, "temperatureGraph").Width,
                                name + " shares graph width at " + size);
                        using (var image = new Bitmap(history.Width, history.Height))
                        { history.DrawToBitmap(image, new Rectangle(Point.Empty, image.Size)); image.Save(Path.Combine(Path.GetTempPath(), "TrayTemps-audit-history-" + size.Width + "-" + light + ".png")); }
                    }
                    var fpsGraph = Control<HistoryGraphControl>(history, "fpsGraph");
                    var before = ((HistoryTrace[])Get(fpsGraph, "_traces")).ToDictionary(trace => trace.Name, trace => trace.Color);
                    config.ShowFps = false; config.ShowRamUsage = false; Call(history, "RefreshGraphs");
                    Check(((HistoryTrace[])Get(fpsGraph, "_traces")).All(trace => before[trace.Name] == trace.Color), "history metric colors stay stable when another metric is disabled");
                    var vram = ((HistoryTrace[])Get(Control<HistoryGraphControl>(history, "memoryGraph"), "_traces")).Single();
                    Check(vram.Color == before["Average FPS"], "VRAM keeps its color when RAM is disabled");
                    Check(((HistoryTrace[])Get(fpsGraph, "_traces")).All(trace => Contrast(trace.Color, theme.SurfaceBack) >= 4.5),
                        "small graph legends have readable contrast against the current theme");
                    config.ShowFps = true; config.ShowRamUsage = true;
                    Check(Control<Button>(history, "clear").FlatAppearance.MouseOverBackColor == theme.ButtonHover,
                        "history Clear hover uses the application theme");
                    history.Size = new Size(1800, 1400);
                    Call(history, "FitToWorkingArea", new Rectangle(50, 50, 1000, 650));
                    Check(new Rectangle(50, 50, 1000, 650).Contains(history.Bounds), "saved history bounds fit a smaller working area");
                    history.Size = new Size(820, 560); Call(history, "OnResizeEnd", EventArgs.Empty);
                    Check(host.HistoryPreferences.WindowWidth == 820 && host.HistoryPreferences.WindowHeight == 560,
                        "normal history resize saves its size while the window stays open");
                    exit.PerformClick();
                    Check(!history.Visible, "history title-bar exit control closes the window");
                }
                using (var osd = new OsdSettingsDialog(host, config))
                {
                    osd.Show(host);
                    foreach (string page in new[] { "Metrics", "Appearance", "Layout", "Presets" })
                    {
                        Call(osd, "ShowPage", Enum.Parse(typeof(OsdSettingsDialog).GetNestedType("OsdSettingsPage", BindingFlags.NonPublic), page));
                        foreach (Size size in new[] { osd.MinimumSize, new Size(1000, 760) })
                        {
                            osd.Size = size; await Task.Delay(30);
                            using (var image = new Bitmap(osd.Width, osd.Height))
                            { osd.DrawToBitmap(image, new Rectangle(Point.Empty, image.Size)); image.Save(Path.Combine(Path.GetTempPath(), "TrayTemps-audit-osd-" + page + "-" + size.Width + "-" + light + ".png")); }
                            if (page == "Presets")
                            {
                                var presetList = Control<ComboBox>(osd, "presetList");
                                var loadFile = Control<Button>(osd, "loadPresetFile");
                                var loadSelected = Control<Button>(osd, "loadEmbeddedPreset");
                                var saveFile = Control<Button>(osd, "savePresetFile");
                                Check(Control<TableLayoutPanel>(osd, "presetsLayout").BackColor == theme.SurfaceBack &&
                                    presetList.Left == loadFile.Left && presetList.Right == loadFile.Right &&
                                    loadSelected.Left == saveFile.Left && loadSelected.Right == saveFile.Right &&
                                    Control<Label>(osd, "presetsTitle").Left == loadFile.Left,
                                    "Presets uses the shared card surface and aligned label/control edges at " + size);
                            }
                            if (page == "Metrics")
                                Check(Control<TextBox>(osd, "hotkeyValue").Right == Control<ListView>(osd, "metricsList").Right,
                                    "hotkey field and metric list share the same right inset at " + size);
                            if (page == "Layout")
                            {
                                var layoutPage = Control<TabPage>(osd, "layoutPage");
                                layoutPage.AutoScrollPosition = new Point(0, int.MaxValue);
                                await Task.Delay(20);
                                var lastField = Control<ComboBox>(osd, "groupedValuesGapPercent");
                                Check(layoutPage.RectangleToScreen(layoutPage.ClientRectangle).Contains(lastField.RectangleToScreen(lastField.ClientRectangle)),
                                    "last Layout field is reachable by scrolling at " + size);
                                layoutPage.AutoScrollPosition = Point.Empty;
                            }
                        }
                    }
                    var draft = (OsdConfiguration)Get(osd, "_metricDraft");
                    Call(osd, "ShowPage", Enum.Parse(typeof(OsdSettingsDialog).GetNestedType("OsdSettingsPage", BindingFlags.NonPublic), "Layout"));
                    osd.Size = osd.MinimumSize;
                    foreach (var pair in new[] { new[] { "positionLabel", "positionValue" }, new[] { "columnsLabel", "columnsValue" },
                        new[] { "fpsRefreshIntervalLabel", "fpsRefreshIntervalValue" },
                        new[] { "groupedValuesGapLabel", "groupedValuesGapPercent" },
                        new[] { "screenMarginLabel", "screenMarginValue" } })
                    {
                        var label = Control<Label>(osd, pair[0]); var input = Control<ComboBox>(osd, pair[1]);
                        Check(label.PreferredSize.Width <= label.Width, pair[0] + " fits the minimum-width layout");
                        Check(Math.Abs(label.Top + label.Height / 2 - input.Top - input.Height / 2) <= 1,
                            pair[1] + " aligns vertically with its label: " + label.Bounds + " / " + input.Bounds);
                    }
                    var gapLabel = Control<Label>(osd, "fpsLabelValueSpacingLabel");
                    var groupedGap = Control<ComboBox>(osd, "groupedValuesGapPercent");
                    var spacingTable = Control<TableLayoutPanel>(osd, "spacingLayout");
                    Check(spacingTable.GetRow(groupedGap) == 3 && spacingTable.GetColumn(groupedGap) == 1 &&
                        groupedGap.Left == Control<ComboBox>(osd, "externalSpacingValue").Left &&
                        groupedGap.Right == Control<ComboBox>(osd, "externalSpacingValue").Right &&
                        groupedGap.BackColor == theme.InputBack && groupedGap.ForeColor == theme.Text,
                        "group gap percentage occupies the last left spacing row with matching theme and alignment");
                    var gapEditor = Control<ComboBox>(osd, "fpsLabelValueSpacing");
                    var orderDown = Control<Button>(osd, "orderDown");
                    var orderList = Control<TreeView>(osd, "itemOrder");
                    Check(gapLabel.Parent == Control<Control>(osd, "layoutCard") && gapEditor.Parent == gapLabel.Parent &&
                        gapEditor.Left == orderDown.Left && gapEditor.Right == orderDown.Right &&
                        Math.Abs(gapEditor.Bottom - orderList.Bottom) <= 1 && gapLabel.Bottom <= gapEditor.Top &&
                        gapLabel.Top >= Control<Label>(osd, "displayOrderLabel").Bottom,
                        "gap selector aligns with order buttons and the list bottom, below its priority hint");
                    foreach (string key in OsdMetricDefinitions.Keys) OsdMetricDefinitions.SetVisible(draft, key, false);
                    var order = Control<TreeView>(osd, "itemOrder");
                    order.SelectedNode = order.Nodes.Cast<TreeNode>().SelectMany(node => new[] { node }.Concat(node.Nodes.Cast<TreeNode>()))
                        .Single(node => (string)node.Tag == "FrameTime");
                    draft.ShowFrameTime = true; Call(osd, "UpdateHardwareVisibility");
                    Check(Control<ComboBox>(osd, "fpsRefreshIntervalValue").Enabled && Control<ComboBox>(osd, "fpsLabelValueSpacing").Enabled,
                        "frametime refresh and selected-row gap controls are enabled");
                    order.SelectedNode = order.Nodes.Cast<TreeNode>().SelectMany(node => new[] { node }.Concat(node.Nodes.Cast<TreeNode>()))
                        .Single(node => (string)node.Tag == "AverageFps");
                    draft.ShowFrameTime = false; draft.ShowAverageFps = true; Call(osd, "UpdateHardwareVisibility");
                    Check(Control<ComboBox>(osd, "fpsLabelValueSpacing").Enabled, "selecting average FPS enables its editable row gap");
                    var nodeToEdit = order.Nodes.Cast<TreeNode>().SelectMany(node => new[] { node }.Concat(node.Nodes.Cast<TreeNode>()))
                        .Single(node => (string)node.Tag == "CurrentTime");
                    order.SelectedNode = null;
                    Call(osd, "ItemOrder_NodeMouseClick", order, new TreeNodeMouseClickEventArgs(nodeToEdit, MouseButtons.Left, 1,
                        order.ClientSize.Width - 4, nodeToEdit.Bounds.Top + 2));
                    Check(ReferenceEquals(order.SelectedNode, nodeToEdit) && Control<ComboBox>(osd, "fpsLabelValueSpacing").DroppedDown,
                        "clicking another display-order gap cell selects and opens that row's editable value list");
                    Control<ComboBox>(osd, "fpsLabelValueSpacing").DroppedDown = false;
                    osd.Close();
                }
            }
            host.Close();
        }
        Console.WriteLine("ALL UI AUDIT CHECKS PASSED: " + _checks);
    }

    private static double Contrast(Color first, Color second)
    {
        Func<Color, double> luminance = color =>
        {
            Func<byte, double> linear = component => { double value = component / 255d; return value <= .04045 ? value / 12.92 : Math.Pow((value + .055) / 1.055, 2.4); };
            return .2126 * linear(color.R) + .7152 * linear(color.G) + .0722 * linear(color.B);
        };
        double a = luminance(first), b = luminance(second);
        return (Math.Max(a, b) + .05) / (Math.Min(a, b) + .05);
    }

    private static void Statistics()
    {
        var window = new FrameTimeWindow(1000000);
        long time = 1; window.Reset(1, time);
        for (int i = 0; i < 120; i++) window.Add(time += 10000, 1);
        var stats = window.GetStatistics(time, 1, 500, true);
        Check(Math.Abs(stats.AverageFps.Value - 100) < .00001 && stats.FrameTimeMs == 10 && stats.PeakFrameTimeMs == 10,
            "100 ten-millisecond frames per second yield 100 average FPS and 10 ms frametime");
        Check(stats.Graph.Length == 100 && stats.Graph.Any(point => point.Mean == 10), "frametime graph uses a bounded 100-bucket ten-second view");
        window.Add(time += 60000, 1);
        stats = window.GetStatistics(time, 1, 500, true);
        Check(stats.Graph.Any(point => point.Peak == 60 && point.Mean < 60) && stats.PeakFrameTimeMs == 60,
            "bucket averages preserve a sixty-millisecond spike as a separate peak");
        double total = 120 * .01 + .06;
        Check(Math.Abs(stats.AverageFps.Value - 121 / total) < .0001, "average FPS uses interval count divided by actual elapsed frame time");
        for (int i = 0; i < 1100; i++) window.Add(time += 10000, 1);
        stats = window.GetStatistics(time, 1, 500, false);
        Check(Math.Abs(stats.AverageFps.Value - 100) < .0001 && stats.Graph == null, "old spikes expire from the rolling average and graphs allocate only when requested");
        Check(window.GetStatistics(time + 4000000, 1, 500, true).FrameTimeMs == null, "stale capture shows missing statistics rather than held values");
        Check(window.GetStatistics(time + 1500000, 1, 250, false).FrameTimeMs == 10,
            "delayed ETW batches keep a valid frametime until the existing stale-data limit");
        window.Reset(2, time);
        window.Add(time += 20000, 2);
        Check(window.GetStatistics(time, 2, 500, false).AverageFps == null && window.GetStatistics(time, 2, 500, false).FrameTimeMs == 20,
            "average FPS warms up for one second while frametime can show its first accepted interval");
        Check(window.GetStatistics(time, 1, 500, false).FrameTimeMs == null, "an old generation cannot read a new process history");
        for (int i = 0; i < 100; i++) window.Add(time += 20000, 2);
        window.GetStatistics(time, 2, 500, false);
        int gen0 = GC.CollectionCount(0); long bytes = GC.GetAllocatedBytesForCurrentThread();
        for (int i = 0; i < 1000; i++) window.GetStatistics(time, 2, 500, false);
        Check(GC.GetAllocatedBytesForCurrentThread() == bytes && GC.CollectionCount(0) == gen0, "numeric statistics reads reuse existing storage without heap allocations");
        using (var monitor = new ForegroundFpsMonitor())
        {
            monitor.SetFrameStatisticsEnabled(false, true);
            object history = Get(monitor, "_frameTimes");
            monitor.SetFrameStatisticsEnabled(true, true);
            Check(ReferenceEquals(history, Get(monitor, "_frameTimes")), "timing metrics and 1% Low share the same capture buffers");
            monitor.SetFrameStatisticsEnabled(false, false);
            Check(Get(monitor, "_frameTimes") == null, "disabling all timing consumers releases optional frame buffers");
            Set(monitor, "_running", true); monitor.UpdateTargetProcess(123456789);
            int generation = monitor.TargetGeneration;
            monitor.UpdateTargetProcess(Process.GetCurrentProcess().Id);
            Check(monitor.TargetGeneration == generation, "opening a TrayTemps window retains the last external target");
            monitor.UpdateTargetProcess(123456788);
            Check(monitor.TargetGeneration > generation, "switching to another external process resets statistics");
        }
    }

    private static void OptionalTiming()
    {
        var lowOnly = new FrameTimeWindow(1000000, false);
        long lowTime = 1; lowOnly.Reset(1, lowTime);
        for (int i = 0; i < 150; i++) lowOnly.Add(lowTime += 10000, 1);
        Check(Get(lowOnly, "_buckets") == null && lowOnly.GetValue(lowTime, 1) == 100 &&
            lowOnly.GetStatistics(lowTime, 1, 500, true).Graph == null && lowOnly.GetStatistics(lowTime, 1, 500, false).AverageFps == null,
            "1% Low alone skips timing buckets and statistics scans while preserving its result");
        lowOnly.SetTimingEnabled(true);
        Check(lowOnly.GetValue(lowTime, 1) == 100 && lowOnly.GetStatistics(lowTime, 1, 500, false).AverageFps == 100,
            "enabling timing retains existing low samples and reconstructs the rolling interval total");
        lowOnly.Add(lowTime += 20000, 1);
        Check(lowOnly.GetStatistics(lowTime, 1, 500, true).Graph.Any(point => point.Peak == 20),
            "new timing buckets start collecting when enabled");
        lowOnly.SetTimingEnabled(false);
        Check(Get(lowOnly, "_buckets") == null && lowOnly.GetValue(lowTime, 1).HasValue,
            "disabling timing releases its buckets without resetting 1% Low history");
    }

    private static void Store()
    {
        var history = new PerformanceHistory();
        for (int i = 1; i <= 10000; i++) history.Add("Fps", i * .25, i);
        var points = history.Snapshot("Fps", history.LastTime, 30);
        Check(points.Length == PerformanceHistory.Capacity && points.Last().Time - points.First().Time == 1800,
            "history remains bounded to thirty minutes at the fastest collection rate");
        Check(history.Snapshot("Fps", history.LastTime, 1).Length == 241, "display range selects existing samples without changing collection cadence");
        history.Add("Fps", 2501, null, 2);
        history.Add("Fps", 2502, 60, 3);
        points = history.Snapshot("Fps", history.LastTime, 1);
        Check(double.IsNaN(points[points.Length - 2].Value) && points.Last().Source == 3, "missing samples and application/source changes remain explicit gaps");
        history.Add("CurrentTime", 2600, 1);
        Check(history.Snapshot("CurrentTime", 2600, 30).Length == 0, "Current Time is an axis rather than a recorded numeric series");
        history.Pause("Fps", 2503); history.Pause("Fps", 2504);
        Check(history.Snapshot("Fps", 2504, 1).Count(point => point.Time > 2502) == 1 && double.IsNaN(history.Snapshot("Fps", 2504, 1).Last().Value),
            "disabling a recorded metric adds one gap without repeatedly appending missing samples");
        history.Add("FrameTime", 2505, 10, 1, double.PositiveInfinity);
        Check(history.Snapshot("FrameTime", 2505, 1).Single().Peak == 10, "invalid graph peaks fall back to the valid mean");
        history.Clear(); Check(history.LastTime == 0 && history.Snapshot("Fps", 2600, 30).Length == 0, "Clear releases history without changing recording preferences");
    }

    private static void Models()
    {
        var settings = JsonSerializer.Deserialize<AppSettings>("{\"LightMode\":true}");
        Check(!settings.History.RecordingEnabled && settings.History.Minutes == 5 && !settings.Osd.ShowAverageFps && !settings.Osd.ShowFrameTime && !settings.Osd.ShowFrameTimeGraph,
            "old settings default recording and all three new OSD metrics to off");
        Check((int)OsdItemKind.CurrentTime == 7 && (int)OsdItemKind.FpsOnePercentLow == 8 && (int)OsdItemKind.AverageFps == 9,
            "appended metrics retain existing serialized enum identifiers");
        var config = new OsdConfiguration { MetricGroups = new System.Collections.Generic.Dictionary<string, string> { ["FrameTimeGraph"] = "FPS" } };
        OsdMetricDefinitions.Migrate(config);
        Check(config.MetricStyles.Count == 12 && OsdMetricDefinitions.Group(config, "FrameTimeGraph") == "" && OsdItemOrderHelper.Parse(config.ItemOrder).Count == 12,
            "migration seeds twelve styles/order entries and keeps the graph standalone");
        var preferences = HistorySettings.Normalize(new HistorySettings { RecordingEnabled = true, Minutes = -1, WindowWidth = 1 });
        Check(preferences.RecordingEnabled && preferences.Minutes == 5 && preferences.WindowWidth == 600,
            "invalid saved ranges and bounds normalize without dropping recording preference");
        var copy = JsonSerializer.Deserialize<AppSettings>(JsonSerializer.Serialize(new AppSettings { History = new HistorySettings { RecordingEnabled = true, Minutes = 30 } }));
        Check(copy.History.RecordingEnabled && copy.History.Minutes == 30, "recording preference and display range survive settings serialization");
    }

    private static ForegroundFpsMonitor FakeMonitor() { var monitor = new ForegroundFpsMonitor(); Set(monitor, "_running", true); return monitor; }
    private static void GraphDpi()
    {
        var helper = typeof(Control).Assembly.GetType("System.Windows.Forms.DpiHelper");
        helper.GetProperty("EnableDpiChangedMessageHandling", BindingFlags.Static | Flags).GetValue(null);
        var flag = helper.GetField("enableDpiChangedMessageHandling", BindingFlags.Static | Flags);
        object prior = flag.GetValue(null); flag.SetValue(null, true);
        IntPtr awareness = SetThreadDpiAwarenessContext(new IntPtr(-4));
        try
        {
            foreach (int dpi in new[] { 96, 120, 144, 192 })
            foreach (float fontSize in new[] { 8.25f, 12f })
            using (var graph = new HistoryGraphControl())
            using (var font = new Font("Segoe UI", fontSize * dpi / 96))
            using (var bitmap = new Bitmap(600 * dpi / 96, 155 * dpi / 96))
            {
                graph.Size = bitmap.Size; graph.Font = font;
                graph.BackColor = Color.FromArgb(24, 24, 24); graph.ForeColor = Color.White;
                IntPtr handle = graph.Handle;
                Set(graph, "deviceDpi", dpi);
                graph.SetData("Frametime", "ms", 0, 60, new[] { new HistoryTrace { Name = "Frametime", Color = Color.Gold,
                    Points = Enumerable.Range(0, 241).Select(i => new HistoryPoint { Time = i / 4d, Value = 10, Peak = i == 120 ? 60 : 12 }).ToArray() } });
                graph.DrawToBitmap(bitmap, new Rectangle(Point.Empty, bitmap.Size));
                bitmap.Save(Path.Combine(Path.GetTempPath(), "TrayTemps-history-dpi-" + dpi + "-font-" + fontSize + ".png"));
                using (var graphics = Graphics.FromImage(bitmap))
                {
                    int textHeight = (int)Call(graph, "GetTextHeight", graphics);
                    var plot = (Rectangle)Call(graph, "GetPlotBounds", textHeight);
                    Check(graph.DeviceDpi == dpi && plot.Top >= 2 * textHeight + 5 * dpi / 96 && plot.Bottom + textHeight <= graph.Height,
                        "history title, legend, plot and axis do not overlap at " + dpi + " DPI and " + fontSize + " pt");
                }
            }
        }
        finally { flag.SetValue(null, prior); SetThreadDpiAwarenessContext(awareness); }
    }
    private static void RefreshCadence(Host host)
    {
        var config = host.NormalizeOsdConfiguration(new OsdConfiguration { ShowCpu = false, ShowGpu = false,
            ShowFps = true, FpsRefreshIntervalSeconds = 10m });
        Set(host, "_osdConfiguration", config);
        var monitor = FakeMonitor(); Set(host, "_fpsMonitor", monitor);
        monitor.UpdateForegroundProcess();
        Set(monitor, "_lastPresentTimestamp", Stopwatch.GetTimestamp()); Set(monitor, "_latestFps", 111);
        host.SetHistoryRecording(true); Control<CheckBox>(host, "osdEnable").Checked = true;
        Call(host, "UpdateOsd", (float?)60, (float?)70);
        Set(monitor, "_latestFps", 222); Set(monitor, "_lastPresentTimestamp", Stopwatch.GetTimestamp());
        Call(host, "UpdateOsd", (float?)61, (float?)71);
        Check(((List<OsdMetric>)Get(host, "_lastOsdMetrics")).Single().Value == "111",
            "a hardware update cannot bypass a ten-second displayed FPS interval");
        Call(host, "RecordFrameHistory", config);
        var history = (PerformanceHistory)Get(host, "_history");
        Check(history.Snapshot("Fps", history.LastTime, 1).Last().Value == 222,
            "history records current FPS independently of the cached display interval");
        Set(host, "_lastFpsOsdRefresh", 0L); Call(host, "UpdateOsd", (float?)62, (float?)72);
        Check(((List<OsdMetric>)Get(host, "_lastOsdMetrics")).Single().Value == "222",
            "the next requested refresh shows the current FPS");
        Set(monitor, "_targetGeneration", monitor.TargetGeneration + 1);
        Set(monitor, "_latestFps", -1); Set(monitor, "_lastPresentTimestamp", 0L);
        Call(host, "UpdateOsd", (float?)63, (float?)73);
        Check(((List<OsdMetric>)Get(host, "_lastOsdMetrics")).Single().Value == "N/A",
            "a target change clears cached frame values immediately without waiting for the display interval");
        host.SetHistoryRecording(false); Control<CheckBox>(host, "osdEnable").Checked = false;
    }

    private static async Task Ui(Host host)
    {
        var button = Control<Button>(host, "historyButton");
        var iconSize = TextRenderer.MeasureText(button.Text, button.Font, Size.Empty, TextFormatFlags.SingleLine);
        Check(iconSize.Width <= button.ClientSize.Width && iconSize.Height <= button.ClientSize.Height &&
            button.Text.Length > 0 && button.Parent == Control<Label>(host, "tempTitle").Parent,
            "history Unicode icon fits the button beside the temperature heading: " + iconSize + " in " + button.ClientSize);
        var config = host.NormalizeOsdConfiguration(new OsdConfiguration { ShowCpu = false, ShowGpu = false, ShowFps = true, ShowAverageFps = true, ShowFrameTime = true, ShowFrameTimeGraph = true });
        Set(host, "_osdConfiguration", config);
        var legacyGaps = config.Clone(); legacyGaps.FpsLabelValueSpacing = 27;
        Check(((OsdMetric)Call(host, "CreateFpsMetric", legacyGaps, false)).LabelValueSpacingOverride == 27 &&
            ((OsdMetric)Call(host, "CreateFrameMetric", legacyGaps, "AverageFps")).LabelValueSpacingOverride == 27,
            "production FPS and average FPS metrics retain saved internal group spacing");
        var monitor = FakeMonitor(); Set(host, "_fpsMonitor", monitor);
        host.SetHistoryRecording(true);
        Check(ReferenceEquals(monitor, Get(host, "_fpsMonitor")) && !Control<CheckBox>(host, "osdEnable").Checked,
            "recording uses shared capture without enabling OSD");
        Call(host, "HistoryButton_Click", null, EventArgs.Empty);
        var dialog = (PerformanceHistoryDialog)Get(host, "_historyDialog"); await Task.Delay(20);
        Check(Control<CheckBox>(dialog, "recording").Checked && Control<ComboBox>(dialog, "range").Items.Count == 4,
            "history dialog reflects recording and offers four display ranges");
        host.SetHistoryRange(30); Call(dialog, "RefreshGraphs");
        Check(Control<ComboBox>(dialog, "range").SelectedIndex == 3, "an open history window reflects externally updated range settings");
        Set(host, "_isInternalCheckChange", true);
        Call(host, "SetDefaultControlValues");
        Set(host, "_isInternalCheckChange", false);
        Call(dialog, "RefreshGraphs");
        Check(Control<ComboBox>(dialog, "range").SelectedIndex == 1 && !Control<CheckBox>(dialog, "recording").Checked &&
            host.HistoryPreferences.Minutes == 5 && !host.HistoryPreferences.RecordingEnabled,
            "reset synchronizes the open history range and recording controls without writing them back");
        Set(host, "_osdConfiguration", config); Set(host, "_fpsMonitor", monitor); host.SetHistoryRecording(true);
        Call(host, "HistoryButton_Click", null, EventArgs.Empty);
        Check(ReferenceEquals(dialog, Get(host, "_historyDialog")), "reopening uses the same history window");
        dialog.WindowState = FormWindowState.Minimized;
        Call(host, "HistoryButton_Click", null, EventArgs.Empty);
        Check(dialog.WindowState == FormWindowState.Normal, "the history icon restores an existing minimized history window");
        foreach (bool light in new[] { false, true })
        {
            Control<CheckBox>(host, "lightModeSwitch").Checked = light;
            var history = (PerformanceHistory)Get(host, "_history");
            history.Clear();
            double now = Stopwatch.GetTimestamp() / (double)Stopwatch.Frequency;
            for (int i = 0; i < 30; i++)
            {
                history.Add("CpuTemperature", now - 30 + i, 45 + i % 5);
                history.Add("GpuTemperature", now - 30 + i, 55 + i % 8);
                history.Add("CpuUsage", now - 30 + i, 30 + i);
                history.Add("GpuUsage", now - 30 + i, 90);
                history.Add("Fps", now - 30 + i, 144);
                history.Add("FrameTime", now - 30 + i, 6.94, 0, i == 20 ? 60 : 8);
            }
            Call(dialog, "RefreshGraphs");
            using (var image = new Bitmap(dialog.Width, dialog.Height))
            { dialog.DrawToBitmap(image, dialog.ClientRectangle); image.Save(Path.Combine(Path.GetTempPath(), "TrayTemps-performance-history-" + light + ".png")); }
            Check(Control<HistoryGraphControl>(dialog, "temperatureGraph").BackColor == ThemePalette.Create(light).SurfaceBack,
                "history graph follows " + (light ? "light" : "dark") + " theme");
        }
        dialog.Size = new Size(600, 400);
        Check(Control<ComboBox>(dialog, "range").Right <= Control<ComboBox>(dialog, "range").Parent.ClientSize.Width,
            "history controls remain within the minimum-size resizable window");
        dialog.Close();
        Check(host.HistoryPreferences.RecordingEnabled && ReferenceEquals(monitor, Get(host, "_fpsMonitor")),
            "closing history leaves recording enabled and keeps the shared FPS monitor");
        Call(host, "RecordHardwareHistory", (float?)60, (float?)70);
        var store = (PerformanceHistory)Get(host, "_history");
        Check(store.Snapshot("CpuTemperature", store.LastTime, 1).Last().Value == 60,
            "hardware collection continues with the history window closed");
        store.Clear();
        double frameTime = Stopwatch.GetTimestamp() / (double)Stopwatch.Frequency;
        store.Add("Fps", frameTime - .5, 120);
        store.Add("FrameTime", frameTime - .5, 10);
        config.ShowFps = false; config.ShowFrameTime = false;
        Call(host, "SynchronizeFrameCapture");
        Check(double.IsNaN(store.Snapshot("Fps", store.LastTime, 1).Last().Value) && store.Snapshot("FrameTime", store.LastTime, 1).Last().Value == 10,
            "disabling FPS breaks its line while an enabled frametime graph retains its numeric series");
        config.ShowFrameTimeGraph = false; Call(host, "SynchronizeFrameCapture");
        Check(double.IsNaN(store.Snapshot("FrameTime", store.LastTime, 1).Last().Value), "disabling both frametime consumers breaks their history line");
        config.ShowFps = true; config.ShowFrameTime = true; config.ShowFrameTimeGraph = true;
        Control<CheckBox>(host, "osdEnable").Checked = true;
        config.FpsRefreshIntervalSeconds = 2m;
        monitor.UpdateForegroundProcess();
        Call(host, "UpdateOsd", (float?)60, (float?)70);
        Set(host, "_lastFpsOsdRefresh", Stopwatch.GetTimestamp());
        long refresh = (long)Get(host, "_lastFpsOsdRefresh");
        Call(host, "FpsTimer_Tick", null, EventArgs.Empty);
        Check((long)Get(host, "_lastFpsOsdRefresh") == refresh && ((Timer)Get(host, "_fpsTimer")).Interval == 250,
            "history collection preserves the selected OSD refresh rate");
        var displayed = ((List<OsdMetric>)Get(host, "_lastOsdMetrics")).Single(metric => metric.Key == "FrameTimeGraph");
        Call(host, "UpdateOsd", (float?)62, (float?)72);
        Check(ReferenceEquals(displayed, ((List<OsdMetric>)Get(host, "_lastOsdMetrics")).Single(metric => metric.Key == "FrameTimeGraph")) &&
            (long)Get(host, "_lastFpsOsdRefresh") == refresh,
            "hardware refresh reuses displayed frame values and graph until their interval elapses");
        Set(host, "_lastFpsOsdRefresh", refresh - Stopwatch.Frequency * 3);
        Call(host, "UpdateOsd", (float?)63, (float?)73);
        Check(!ReferenceEquals(displayed, ((List<OsdMetric>)Get(host, "_lastOsdMetrics")).Single(metric => metric.Key == "FrameTimeGraph")),
            "hardware refresh updates frame metrics when the configured interval is due");
        var preview = config.Clone(); preview.MetricStyles["FrameTimeGraph"].ValueColor = Color.Red.ToArgb();
        Call(host, "PreviewOsdConfiguration", preview);
        Check(((List<OsdMetric>)Get(host, "_lastOsdMetrics")).Single(metric => metric.Key == "FrameTimeGraph").Style.ValueColor == Color.Red.ToArgb(),
            "frame metric appearance previews update immediately despite a pending refresh interval");
        Set(host, "_osdPreviewConfiguration", null); Call(host, "UpdateOsd", (float?)63, (float?)73);
        host.SetHistoryRecording(false);
        Check(ReferenceEquals(monitor, Get(host, "_fpsMonitor")), "stopping history preserves capture required by enabled OSD");
        int count = store.Snapshot("CpuTemperature", store.LastTime, 30).Length;
        Call(host, "RecordHardwareHistory", (float?)61, (float?)71);
        Check(store.Snapshot("CpuTemperature", store.LastTime, 30).Length == count, "unchecked recording adds no new history samples");
        host.SetHistoryRecording(true);
        Control<CheckBox>(host, "osdEnable").Checked = false;
        Check(ReferenceEquals(monitor, Get(host, "_fpsMonitor")), "turning OSD off preserves capture required by background recording");
        host.SetHistoryRecording(false);
        Check(Get(host, "_fpsMonitor") == null, "capture stops when neither history nor OSD needs it");

        using (var osd = new OsdSettingsDialog(host, config))
        {
            osd.Show(host); var list = Control<ListView>(osd, "metricsList");
            Check(list.Items.Count == 12 && list.Items.Cast<ListViewItem>().Any(item => (string)item.Tag == "FrameTimeGraph"), "OSD list includes all three new selectable metrics");
            var graphRow = list.Items.Cast<ListViewItem>().Single(item => (string)item.Tag == "FrameTimeGraph");
            graphRow.Selected = true; Call(osd, "BeginMetricCellEdit", 2);
            Check(!Control<TextBox>(osd, "metricLabelEditor").Visible, "graph Group editing is unavailable without adding extra UI");
            osd.Close();
        }
        using (var overlay = new OsdOverlay())
        {
            var graph = new OsdMetric("Frametime:", "") { Key = "FrameTimeGraph", Style = config.MetricStyles["FrameTimeGraph"], ValueWidthTemplate = "graph",
                Graph = Enumerable.Range(0, 100).Select(i => new FrameGraphPoint(6.94, i == 50 ? 60 : 8)).ToArray() };
            overlay.UpdateDisplay(config, new[] { graph }, new Rectangle(0, 0, 1920, 1080));
            var layouts = (IList)Get(overlay, "_metricLayouts"); object layout = layouts[0];
            graph = new OsdMetric("Frametime:", "") { Key = graph.Key, Style = graph.Style, ValueWidthTemplate = graph.ValueWidthTemplate,
                Graph = Enumerable.Range(0, 100).Select(i => new FrameGraphPoint(10, 12)).ToArray() };
            overlay.UpdateDisplay(config, new[] { graph }, new Rectangle(0, 0, 1920, 1080));
            Check(ReferenceEquals(layout, ((IList)Get(overlay, "_metricLayouts"))[0]) && overlay.Height >= 42,
                "sparkline data refreshes without rebuilding OSD geometry and reserves its graph height");
            using (var image = new Bitmap(overlay.Width, overlay.Height)) { overlay.DrawToBitmap(image, overlay.ClientRectangle); image.Save(Path.Combine(Path.GetTempPath(), "TrayTemps-frametime-osd.png")); }
            var graphParts = (IList)Get(((IList)Get(overlay, "_metricLayouts"))[0], "Parts");
            int graphWidth = (int)Get(graphParts[graphParts.Count - 1], "Width");
            graph.Style = graph.Style.Clone(); graph.Style.FontSize = 32;
            overlay.UpdateDisplay(config, new[] { graph }, new Rectangle(0, 0, 1920, 1080));
            graphParts = (IList)Get(((IList)Get(overlay, "_metricLayouts"))[0], "Parts");
            Check((int)Get(graphParts[graphParts.Count - 1], "Width") == graphWidth,
                "frametime font size preserves the graph width while reserving space for its caption");
            graph.Style.LabelColor = Color.Lime.ToArgb(); graph.Style.ValueColor = Color.Magenta.ToArgb();
            foreach (float fontSize in new[] { 10f, 32f, 48f })
            {
                var captionStyle = graph.Style.Clone(); captionStyle.FontSize = fontSize;
                var captionGraph = new OsdMetric("Frametime gj", "N/A") { Key = "FrameTimeGraph", Style = captionStyle,
                    ValueWidthTemplate = "graph", Graph = null };
                overlay.UpdateDisplay(config, new[] { captionGraph }, new Rectangle(0, 0, 1920, 1080));
                using (var image = new Bitmap(overlay.Width, overlay.Height))
                {
                    overlay.DrawToBitmap(image, overlay.ClientRectangle);
                    Check(Enumerable.Range(0, image.Width).Any(x => Enumerable.Range(0, image.Height).Any(y => image.GetPixel(x, y).ToArgb() == Color.Lime.ToArgb())),
                        "nonempty graph caption stays visible without samples at font size " + fontSize);
                    Check(!Enumerable.Range(0, image.Width).Any(x => Enumerable.Range(0, image.Height).Any(y => image.GetPixel(x, y).ToArgb() == Color.Magenta.ToArgb())),
                        "null graph samples never draw N/A or a value-colored line");
                }
            }
            graph = new OsdMetric("", "N/A") { Key = "FrameTimeGraph", Style = graph.Style, ValueWidthTemplate = "graph",
                Graph = Enumerable.Repeat(new FrameGraphPoint(double.NaN, double.NaN), 100).ToArray() };
            overlay.UpdateDisplay(config, new[] { graph }, new Rectangle(0, 0, 1920, 1080));
            using (var image = new Bitmap(overlay.Width, overlay.Height))
            {
                overlay.DrawToBitmap(image, overlay.ClientRectangle);
                Check(!Enumerable.Range(0, image.Width).Any(x => Enumerable.Range(0, image.Height).Any(y =>
                    image.GetPixel(x, y).ToArgb() == Color.Magenta.ToArgb() || image.GetPixel(x, y).ToArgb() == Color.Lime.ToArgb())),
                    "empty frametime graph draws no caption, N/A placeholder or value-colored line");
            }
            graph = new OsdMetric("", "") { Key = "FrameTimeGraph", Style = graph.Style, ValueWidthTemplate = "graph",
                Graph = Enumerable.Range(0, 100).Select(i => new FrameGraphPoint(6.94, 8)).ToArray() };
            overlay.UpdateDisplay(config, new[] { graph }, new Rectangle(0, 0, 1920, 1080));
            using (var image = new Bitmap(overlay.Width, overlay.Height))
            {
                overlay.DrawToBitmap(image, overlay.ClientRectangle);
                Check(Enumerable.Range(0, image.Width).Any(x => Enumerable.Range(0, image.Height).Any(y => image.GetPixel(x, y).ToArgb() == Color.Magenta.ToArgb())),
                    "frametime graph line uses the Appearance value color while its caption is blank");
            }
        }
    }
}
