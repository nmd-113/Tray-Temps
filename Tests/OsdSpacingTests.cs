using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using TrayTemps;

internal static class OsdSpacingTests
{
    private const BindingFlags Flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
    [DllImport("user32.dll")] private static extern IntPtr SetThreadDpiAwarenessContext(IntPtr context);
    private static int _checks;
    private static void Check(bool value, string message) { if (!value) throw new Exception(message); Console.WriteLine("PASS " + (++_checks) + ": " + message); }
    private static object Call(object instance, string name, params object[] args) => instance.GetType().GetMethod(name, Flags).Invoke(instance, args);
    private static object Field(object instance, string name) => instance.GetType().GetField(name, Flags).GetValue(instance);
    private static IList Layouts(OsdOverlay overlay) => (IList)Field(overlay, "_metricLayouts");
    private static string Template(double capacity) => (string)typeof(MainForm).GetMethod("CreateMemoryWidthTemplate", BindingFlags.Static | BindingFlags.NonPublic).Invoke(null, new object[] { capacity });
    private static readonly Rectangle Area = new Rectangle(0, 0, 4000, 2000);

    internal static void Run()
    {
        _checks = 0;
        foreach (double capacity in new[] { 0, 4, 8, 9.99, 16, 32, 64, 99.99 })
            Check(Template(capacity) == "##.##GB", "memory slots reserve two integer digits for " + capacity + " GB");
        Check(Template(100) == "###.##GB" && Template(128) == "###.##GB" && Template(1024) == "####.##GB", "100 GB and larger capacities retain wider memory slots");
        Compatibility();
        PerRowGaps();
        var legacyGap = System.Text.Json.JsonSerializer.Deserialize<OsdConfiguration>("{}");
        Check(legacyGap.GroupedValuesGapPercent == 100 && OsdConfiguration.CreateAppDefault().GroupedValuesGapPercent == 100,
            "fresh and existing settings retain the one-space group minimum");
        foreach (int invalid in new[] { int.MinValue, 0, 99, 201, int.MaxValue })
        {
            var invalidGap = Config(); invalidGap.GroupedValuesGapPercent = invalid;
            OsdMetricDefinitions.Migrate(invalidGap);
            Check(invalidGap.GroupedValuesGapPercent == (invalid < 100 ? 100 : 200), "invalid grouped gap percentage is bounded: " + invalid);
        }
        var customGap = Config(); customGap.GroupedValuesGapPercent = 137;
        var savedGap = System.Text.Json.JsonSerializer.Deserialize<OsdConfiguration>(System.Text.Json.JsonSerializer.Serialize(customGap));
        Check(savedGap.GroupedValuesGapPercent == 137 && savedGap.Clone().GroupedValuesGapPercent == 137,
            "custom grouped gap survives JSON and configuration cloning");
        Check(Enumerable.Range(0, 3).All(i => OsdPresets.LoadEmbedded(i).GroupedValuesGapPercent == 100),
            "the original embedded presets retain the existing one-space group minimum");
        // Simulate the renderer's DeviceDpi and matching bitmap DPI, without pretending
        // control scaling alone tests real monitor handover. Restore WinForms quirks after use.
        var helper = typeof(Control).Assembly.GetType("System.Windows.Forms.DpiHelper");
        var dpiProperty = helper.GetProperty("EnableDpiChangedMessageHandling", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
        dpiProperty.GetValue(null);
        var flag = helper.GetField("enableDpiChangedMessageHandling", BindingFlags.Static | BindingFlags.NonPublic);
        object prior = flag.GetValue(null);
        IntPtr priorAwareness = SetThreadDpiAwarenessContext(new IntPtr(-4));
        flag.SetValue(null, true);
        try
        {
            foreach (int dpi in new[] { 96, 144, 192 })
            foreach (string family in new[] { "JetBrains Mono ExtraBold", "Segoe UI", "Consolas" })
            {
                MemoryAlignment(family, dpi);
                MissingGroupedValues(family, dpi);
                GroupedValueGaps(family, dpi);
                GroupedGapIncrease(family, dpi);
                OversizedLayout(family, dpi);
                Vertical(family, dpi);
            }
        }
        finally { flag.SetValue(null, prior); SetThreadDpiAwarenessContext(priorAwareness); }
        Console.WriteLine("ALL OSD SPACING CHECKS PASSED: " + _checks);
    }

    private static void PerRowGaps()
    {
        var config = Config();
        config.MetricLabelValueGaps["CpuTemperature"] = 0;
        var style = Style("Segoe UI", 16);
        var metric = Metric("CpuTemperature", "CPU", "60Â°C", style, "###Â°C");
        using (var overlay = new OsdOverlay())
        {
            overlay.UpdateDisplay(config, new[] { metric }, Area);
            var parts = ((IList)Field(Layouts(overlay)[0], "Parts")).Cast<object>().ToArray();
            int zeroGap = (int)Field(parts[1], "X") - ((int)Field(parts[0], "X") + (int)Field(parts[0], "Width"));
            config.MetricLabelValueGaps["CpuTemperature"] = 21;
            overlay.UpdateDisplay(config, new[] { metric }, Area);
            parts = ((IList)Field(Layouts(overlay)[0], "Parts")).Cast<object>().ToArray();
            int customGap = (int)Field(parts[1], "X") - ((int)Field(parts[0], "X") + (int)Field(parts[0], "Width"));
            Check(zeroGap == 0 && customGap == 21, "standalone row label/value gap applies its editable per-metric setting");
        }
        config = Config(); config.MetricGroups["CpuTemperature"] = "CPU"; config.MetricLabelValueGaps["Group:CPU"] = 13;
        metric = Metric("CpuTemperature", "", "60Â°C", style, "###Â°C");
        using (var overlay = new OsdOverlay())
        {
            overlay.UpdateDisplay(config, OsdMetricDefinitions.Compose(config, new[] { metric }), Area);
            var parts = ((IList)Field(Layouts(overlay)[0], "Parts")).Cast<object>().ToArray();
            Check((int)Field(parts[1], "X") - ((int)Field(parts[0], "X") + (int)Field(parts[0], "Width")) == 13,
                "group row gap controls caption-to-values spacing");
        }
        foreach (string key in OsdMetricDefinitions.Keys.Where(value => value != "FrameTimeGraph"))
        using (var overlay = new OsdOverlay())
        {
            config = Config(); config.MetricLabelValueGaps[key] = 17;
            metric = Metric(key, "Metric", "1234", style, "####");
            overlay.UpdateDisplay(config, new[] { metric }, Area);
            var standalone = ((IList)Field(Layouts(overlay)[0], "Parts")).Cast<object>().ToArray();
            int standaloneGap = (int)Field(standalone[1], "X") - (int)Field(standalone[0], "Width");
            config.MetricGroups[key] = "Common";
            overlay.UpdateDisplay(config, OsdMetricDefinitions.Compose(config, new[] { metric }), Area);
            var grouped = ((IList)Field(Layouts(overlay)[0], "Parts")).Cast<object>().ToArray();
            int groupedGap = (int)Field(grouped[2], "X") - (int)Field(grouped[1], "X") - (int)Field(grouped[1], "Width");
            Check(standaloneGap == 17 && groupedGap == standaloneGap, key + " follows the same editable label/value gap rule in standalone and grouped rows");
            config.MetricLabelValueGaps[key] = 0;
            overlay.UpdateDisplay(config, OsdMetricDefinitions.Compose(config, new[] { metric }), Area);
            grouped = ((IList)Field(Layouts(overlay)[0], "Parts")).Cast<object>().ToArray();
            Check((int)Field(grouped[2], "X") == (int)Field(grouped[1], "X") + (int)Field(grouped[1], "Width"),
                key + " honors a zero label/value gap without a metric-specific exception");
        }
    }

    private static void OversizedLayout(string family, int dpi)
    {
        foreach (int columns in new[] { 1, 3 })
        foreach (int spacing in new[] { 0, 100 })
        {
            var config = Config(); config.Columns = columns; config.RowsSpacing = spacing; config.ScreenMargin = 10;
            var style = Style(family, 48);
            var lastStyle = style.Clone(); lastStyle.LabelColor = lastStyle.ValueColor = Color.Lime.ToArgb();
            var metrics = OsdMetricDefinitions.Keys.Select((key, i) => Metric(key, "Metric gj " + i, key == "FrameTimeGraph" ? "" : "1234",
                i == OsdMetricDefinitions.Keys.Length - 1 ? lastStyle : style, "####")).ToArray();
            using (var overlay = new OsdOverlay())
            {
                LayoutAtDpi(overlay, config, metrics, dpi);
                overlay.UpdateDisplay(config, metrics, new Rectangle(0, 0, 1000, 300));
                float scale = (float)Field(overlay, "_contentScale");
                Check(scale > 0 && scale <= 1 && overlay.Width <= 1000 && overlay.Height <= 300 &&
                    Layouts(overlay).Cast<object>().All(row => ((int)Field(row, "Y") + (int)Field(row, "Height")) * scale <= overlay.Height),
                    "oversized OSD keeps every row within the screen: " + family + " / " + dpi + " / columns=" + columns + " / gap=" + spacing);
                using (var bitmap = new Bitmap(overlay.Width, overlay.Height))
                {
                    bitmap.SetResolution(dpi, dpi);
                    using (var graphics = Graphics.FromImage(bitmap)) { graphics.Clear(Color.Transparent); Call(overlay, "DrawOverlay", graphics); }
                    bool lastRowVisible = false;
                    for (int x = 0; x < bitmap.Width && !lastRowVisible; x++)
                    for (int y = 0; y < bitmap.Height; y++)
                    {
                        var pixel = bitmap.GetPixel(x, y);
                        if (pixel.G > 80 && pixel.G - Math.Max(pixel.R, pixel.B) > 30) { lastRowVisible = true; break; }
                    }
                    Check(lastRowVisible, "last metric or graph caption remains visible after height fitting");
                    if (dpi == 96 && family == "JetBrains Mono ExtraBold" && columns == 1 && spacing == 100)
                        bitmap.Save(Path.Combine(Path.GetTempPath(), "TrayTemps-oversized-osd-fit.png"));
                }
                overlay.UpdateDisplay(config, metrics, new Rectangle(0, 0, 4000, 10000));
                Check((float)Field(overlay, "_contentScale") == 1f, "roomy screens retain the configured font sizes and spacing without scaling");
            }
        }
        var wideConfig = Config(); wideConfig.Columns = OsdMetricDefinitions.Keys.Length;
        wideConfig.ScreenMargin = 100; wideConfig.ColumnsSpacing = 100;
        var wideMetrics = OsdMetricDefinitions.Keys.Select(key => Metric(key, "Metric", "1234", Style(family, 48), "####")).ToArray();
        using (var overlay = new OsdOverlay())
        {
            LayoutAtDpi(overlay, wideConfig, wideMetrics, dpi);
            overlay.UpdateDisplay(wideConfig, wideMetrics, new Rectangle(0, 0, 300, 300));
            float scale = (float)Field(overlay, "_contentScale");
            Check(scale > 0 && scale < 1 && overlay.Width <= 300 && overlay.Height <= 300 &&
                Layouts(overlay).Cast<object>().All(row => ((int)Field(row, "X") + (int)Field(row, "Width") * (float)Field(row, "Scale")) * scale <= overlay.Width + 0.01f),
                "large margins and column gaps cannot put metrics outside a narrow screen: " + family + " / " + dpi);
        }
    }

    private static OsdMetricStyle Style(string family, float size) => new OsdMetricStyle { FontFamily = family, FontSize = size, LabelColor = Color.White.ToArgb(), ValueColor = Color.White.ToArgb() };
    private static void MissingGroupedValues(string family, int dpi)
    {
        // Reproduce the saved 1%Low: group, with and without its frametime caption.
        var config = Config(); config.RowsSpacing = 1;
        config.ItemOrder = "FpsOnePercentLow,FrameTime";
        config.MetricGroups["FpsOnePercentLow"] = config.MetricGroups["FrameTime"] = "1%Low:";
        var lowStyle = Style(family, 10); lowStyle.ValueColor = Color.Red.ToArgb();
        var frameStyle = Style(family, 10); frameStyle.ValueColor = Color.Lime.ToArgb();
        foreach (bool showFrameLabel in new[] { false, true })
        using (var overlay = new OsdOverlay())
        {
            object previousLayout = null; Size missingSize = Size.Empty, numericSize = Size.Empty;
            string[] previousValues = null;
            foreach (var values in new[] { new[] { "N/A", "N/A" }, new[] { "N/A", "N/A" },
                new[] { "9", "16.67 ms" }, new[] { "9999", "9999.99 ms" },
                new[] { "N/A", "16.67 ms" }, new[] { "N/A", "9999.99 ms" },
                new[] { "92", "N/A" }, new[] { "9999", "N/A" }, new[] { "N/A", "N/A" } })
            {
                var metrics = new[] { Metric("FpsOnePercentLow", "", values[0], lowStyle, "####"),
                    Metric("FrameTime", showFrameLabel ? "frametime:" : "", values[1], frameStyle, "####.## ms") };
                var rows = OsdMetricDefinitions.Compose(config, metrics);
                if (previousLayout == null) LayoutAtDpi(overlay, config, rows, dpi);
                else overlay.UpdateDisplay(config, rows, Area);
                var layout = Layouts(overlay)[0];
                if (previousValues != null)
                {
                    bool sameAvailability = Enumerable.Range(0, values.Length).All(i => (values[i] == "N/A") == (previousValues[i] == "N/A"));
                    Check(ReferenceEquals(previousLayout, layout) == sameAvailability,
                        "only availability changes rebuild grouped frame slots: " + family + " / " + dpi + ", caption=" + showFrameLabel);
                }
                if (values.All(value => value == "N/A"))
                {
                    if (missingSize == Size.Empty) missingSize = overlay.Size;
                    Check(overlay.Size == missingSize, "repeated and returning N/A values restore the same compact group size");
                }
                else if (values.All(value => value != "N/A"))
                {
                    if (numericSize == Size.Empty) numericSize = overlay.Size;
                    Check(overlay.Size == numericSize && overlay.Width > missingSize.Width,
                        "numeric frame values restore stable slots larger than compact placeholders");
                }
                var parts = ((IList)Field(layout, "Parts")).Cast<object>().Where(p => (int)Field(p, "Member") >= 0).ToArray();
                using (var bitmap = new Bitmap(overlay.Width, overlay.Height))
                {
                    bitmap.SetResolution(dpi, dpi);
                    using (var graphics = Graphics.FromImage(bitmap)) { graphics.Clear(Color.Transparent); Call(overlay, "DrawOverlay", graphics); }
                    if (family == "JetBrains Mono ExtraBold" && dpi == 96 && values.All(v => v == "N/A"))
                        bitmap.Save(Path.Combine(Path.GetTempPath(), "TrayTemps-missing-frame-values-" + showFrameLabel + ".png"));
                    for (int i = values.Length - 1; i >= 0; i--)
                    {
                        int first = bitmap.Width, last = -1;
                        for (int x = 0; x < bitmap.Width; x++)
                        for (int y = 0; y < bitmap.Height; y++)
                        {
                            var color = bitmap.GetPixel(x, y);
                            if (i == 0 ? color.R > 80 && color.R - Math.Max(color.G, color.B) > 30 :
                                color.G > 80 && color.G - Math.Max(color.R, color.B) > 30)
                            { first = Math.Min(first, x); last = Math.Max(last, x); }
                        }
                        int slotLeft = (int)Field(layout, "X") + (int)Field(parts[i], "X");
                        int slotRight = slotLeft + (int)Field(parts[i], "Width");
                        int tolerance = (int)Math.Ceiling(2d * dpi / 96);
                        Check(last >= first && (values[i] == "N/A"
                            ? Math.Abs(first - slotLeft) <= tolerance && Math.Abs(last - slotRight) <= tolerance
                            : Math.Abs(last - slotRight) <= tolerance),
                            values[i] == "N/A" ? "N/A fits its own slot without unused digit or unit space" : "numeric frame values retain fixed leading padding and their unit");
                    }
                }
                previousLayout = layout; previousValues = values;
            }
        }
    }
    private static void GroupedGapIncrease(string family, int dpi)
    {
        var config = Config(); config.RowsSpacing = 5;
        config.MetricGroups["CpuTemperature"] = config.MetricGroups["CpuUsage"] = "CPU";
        config.MetricGroups["Fps"] = config.MetricGroups["FpsOnePercentLow"] = "FPS";
        config.MetricLabelValueGaps["Group:CPU"] = 10;
        var style = Style(family, 10);
        var metrics = new[] { Metric("CpuTemperature", "", "65\u00b0C", style, "###\u00b0C"), Metric("CpuUsage", "", "9%", style, "###%"),
            Metric("Fps", "FPS", "144", style, "####"), Metric("FpsOnePercentLow", "1%Low", "92", style, "####") };
        var rows = OsdMetricDefinitions.Compose(config, metrics);
        using (var overlay = new OsdOverlay())
        {
            LayoutAtDpi(overlay, config, rows, dpi);
            var baseline = Layouts(overlay)[0];
            var parts = ((IList)Field(baseline, "Parts")).Cast<object>().ToArray();
            var separator = parts.Single(p => (string)Field(p, "Text") == " ");
            int width = (int)Field(separator, "Width");
            var baselineFps = Layouts(overlay)[1];
            var fpsParts = ((IList)Field(baselineFps, "Parts")).Cast<object>().ToArray();
            Check((int)Field(fpsParts.Single(p => (string)Field(p, "Text") == " "), "Width") == width,
                "FPS/1% Low uses the same one-space group gap as CPU metrics");
            config.GroupedValuesGapPercent = 150;
            overlay.UpdateDisplay(config, rows, Area);
            var increased = Layouts(overlay)[0];
            var increasedParts = ((IList)Field(increased, "Parts")).Cast<object>().ToArray();
            int gap = (int)Field(increasedParts.Single(p => (string)Field(p, "Text") == " "), "Width");
            Check(!ReferenceEquals(baseline, increased) && gap > width && Math.Abs(gap - width * 1.5) <= 1,
                "percentage change immediately increases the measured group gap: " + family + " / " + dpi);
            Check((int)Field(increased, "Width") - (int)Field(baseline, "Width") == gap - width &&
                Enumerable.Range(0, parts.Length).Where(i => (string)Field(parts[i], "Text") != " ")
                    .All(i => (int)Field(parts[i], "Width") == (int)Field(increasedParts[i], "Width")),
                "group gap increase preserves caption gap and every fixed numeric slot");
            Check((int)Field(Layouts(overlay)[1], "Width") - (int)Field(baselineFps, "Width") == gap - width,
                "the common percentage increases FPS/1% Low spacing by the same amount");
            overlay.UpdateDisplay(config, rows, Area);
            Check(ReferenceEquals(increased, Layouts(overlay)[0]), "unchanged percentage reuses cached OSD layout");
            config.GroupedValuesGapPercent = 100;
            overlay.UpdateDisplay(config, rows, Area);
            Check((int)Field(Layouts(overlay)[0], "Width") == (int)Field(baseline, "Width"),
                "returning to 100% exactly restores original group geometry");
            config.ItemOrder = "CpuTemperature,CpuUsage,FpsOnePercentLow,Fps";
            overlay.UpdateDisplay(config, OsdMetricDefinitions.Compose(config, metrics), Area);
            Check((int)Field(Layouts(overlay)[1], "Width") == (int)Field(baselineFps, "Width"),
                "reversing FPS/1% Low keeps the common spacing rule");
        }
    }
    private static void GroupedValueGaps(string family, int dpi)
    {
        var config = Config(); config.RowsSpacing = 5;
        string[] keys = { "CpuTemperature", "CpuUsage", "RamUsage", "Fps" };
        string[] templates = { "###\u00b0C", "###%", "##.##GB", "####" };
        var colors = new[] { Color.Red, Color.Lime, Color.Blue, Color.Yellow };
        var styles = colors.Select(color => { var style = Style(family, 10); style.ValueColor = color.ToArgb(); return style; }).ToArray();
        foreach (string key in keys) config.MetricGroups[key] = "CPU:";
        using (var overlay = new OsdOverlay())
        {
            object cachedLayout = null; Size outerSize = Size.Empty;
            int[] initialRight = null, initialLeft = null;
            foreach (var values in new[] { new[] { "35\u00b0C", "2%", "7.13GB", "9" }, new[] { "100\u00b0C", "100%", "12.00GB", "9999" }, new[] { "N/A", "N/A", "N/A", "N/A" } })
            {
                var metrics = keys.Select((key, i) => Metric(key, "", values[i], styles[i], templates[i])).ToArray();
                var rows = OsdMetricDefinitions.Compose(config, metrics);
                if (cachedLayout == null)
                {
                    LayoutAtDpi(overlay, config, rows, dpi);
                    cachedLayout = Layouts(overlay)[0]; outerSize = overlay.Size;
                }
                else overlay.UpdateDisplay(config, rows, Area);
                Check(values[0] == "N/A"
                    ? !ReferenceEquals(cachedLayout, Layouts(overlay)[0]) && overlay.Width < outerSize.Width
                    : ReferenceEquals(cachedLayout, Layouts(overlay)[0]) && overlay.Size == outerSize,
                    "group numeric updates stay stable while unavailable values use compact slots: " + family + " / " + dpi);
                using (var image = new Bitmap(overlay.Width, overlay.Height))
                {
                    image.SetResolution(dpi, dpi);
                    overlay.DrawToBitmap(image, overlay.ClientRectangle);
                    int[] left = { image.Width, image.Width, image.Width, image.Width }, right = { -1, -1, -1, -1 };
                    for (int x = 0; x < image.Width; x++)
                    for (int y = 0; y < image.Height; y++)
                    {
                        var pixel = image.GetPixel(x, y);
                        int[] channels = { pixel.R, pixel.G, pixel.B };
                        for (int i = 0; i < colors.Length; i++)
                            if (i == 3 ? pixel.R > 80 && pixel.G > 80 && Math.Min(pixel.R, pixel.G) - pixel.B > 30 :
                                channels[i] > 80 && channels[i] - channels[(i + 1) % 3] > 30 && channels[i] - channels[(i + 2) % 3] > 30)
                            { left[i] = Math.Min(left[i], x); right[i] = Math.Max(right[i], x); }
                    }
                    Check(right.All(value => value >= 0), "all grouped slots render their full values");
                    if (initialRight == null)
                    {
                        initialRight = right; initialLeft = left;
                    }
                    else if (values[0] != "N/A")
                    {
                        Check(Enumerable.Range(0, right.Length).All(i => Math.Abs(right[i] - initialRight[i]) <= Math.Ceiling(2d * dpi / 96)),
                            "temperature, usage, memory and FPS slot ends stay fixed across digit counts: " + family + " / " + dpi);
                        Check(Enumerable.Range(0, left.Length).All(i => left[i] < initialLeft[i]),
                            "shorter numeric values keep leading padding rather than shifting later columns");
                    }
                    if (family == "JetBrains Mono ExtraBold" && dpi == 96 && values[0] != "N/A")
                        image.Save(Path.Combine(Path.GetTempPath(), "TrayTemps-group-spacing-" + values[1].Replace("%", "") + ".png"));
                }
            }
        }
    }
    private static OsdMetric Metric(string key, string label, string value, OsdMetricStyle style, string template = null) =>
        new OsdMetric(label, value) { Key = key, Style = style, ValueWidthTemplate = template };
    private static OsdConfiguration Config() { var config = new OsdConfiguration { ScreenMargin = 0, RowsSpacing = 0, Columns = 1 }; OsdMetricDefinitions.Migrate(config); return config; }
    private static void LayoutAtDpi(OsdOverlay overlay, OsdConfiguration config, IEnumerable<OsdMetric> rows, int dpi, int width = 4000)
    {
        IntPtr handle = overlay.Handle;
        overlay.UpdateDisplay(config, rows, new Rectangle(0, 0, width, 2000));
        typeof(Control).GetField("deviceDpi", Flags).SetValue(overlay, dpi);
        Call(overlay, "InvalidateFontSettings");
        Call(overlay, "RebuildLayout");
        if (overlay.DeviceDpi != dpi || (int)Field(overlay, "_layoutDpi") != dpi) throw new Exception("DPI simulation did not reach the renderer");
    }

    private static void MemoryAlignment(string family, int dpi)
    {
        var config = Config();
        config.MetricGroups = new Dictionary<string, string> { ["CpuTemperature"] = "CPU", ["RamUsage"] = "CPU", ["GpuTemperature"] = "GPU", ["VramUsage"] = "GPU" };
        config.ItemOrder = "CpuTemperature,RamUsage,GpuTemperature,VramUsage";
        var style = Style(family, 16);
        var metrics = new[] { Metric("CpuTemperature", "", "60°C", style, "###°C"), Metric("RamUsage", "", "12.00G", style, Template(16)),
            Metric("GpuTemperature", "", "55°C", style, "###°C"), Metric("VramUsage", "", "8.00G", style, Template(8)) };
        using (var overlay = new OsdOverlay())
        {
            LayoutAtDpi(overlay, config, OsdMetricDefinitions.Compose(config, metrics), dpi);
            var first = ((IList)Field(Layouts(overlay)[0], "Parts")).Cast<object>().Where(part => (int)Field(part, "Member") >= 0).ToArray();
            var second = ((IList)Field(Layouts(overlay)[1], "Parts")).Cast<object>().Where(part => (int)Field(part, "Member") >= 0).ToArray();
            Check(first.Length == 2 && second.Length == 2 &&
                (int)Field(first[0], "X") < (int)Field(first[1], "X") &&
                (int)Field(second[0], "X") < (int)Field(second[1], "X"),
                "grouped memory values flow left to right without forced right alignment: " + family + " / " + dpi + " DPI");
        }
    }

    private static void Vertical(string family, int dpi)
    {
        var config = Config();
        var style = Style(family, 16);
        var rows = new[] { Metric("CpuTemperature", "CPU", "88.88G", style, "##.##G"), Metric("GpuTemperature", "GPU", "88.88G", style, "##.##G") };
        using (var overlay = new OsdOverlay())
        {
            LayoutAtDpi(overlay, config, rows, dpi);
            int compactHeight = overlay.Height;
            int gap = PixelGap(overlay, dpi);
            Check(gap >= 0 && gap <= 4, "zero row spacing leaves only outline/antialias clearance: " + family + " / " + dpi + " DPI (" + gap + " px)");
            Check(WithinRows(overlay, rows, dpi), "compact row glyphs and outlines stay inside their row bounds");
            object original = Layouts(overlay)[0];
            foreach (string value in new[] { "11.11G", "99.99G", "00.00G" })
            {
                rows[0] = Metric("CpuTemperature", "CPU", value, style, "##.##G");
                overlay.UpdateDisplay(config, rows, Area);
                Check(ReferenceEquals(original, Layouts(overlay)[0]) && overlay.Height == compactHeight && WithinRows(overlay, rows, dpi),
                    "numeric refresh retains compact geometry and full glyph bounds: " + value);
            }
            rows[0] = Metric("CpuTemperature", "CPU", "N/A", style, "##.##G");
            overlay.UpdateDisplay(config, rows, Area);
            Check(WithinRows(overlay, rows, dpi) && (int)Field(Layouts(overlay)[0], "Width") < (int)Field(original, "Width"),
                "standalone missing-data glyphs get compact width and height without clipping");
            rows[0] = Metric("CpuTemperature", "CPU", "88.88G", style, "##.##G");
            overlay.UpdateDisplay(config, rows, Area);
            Check(overlay.Height == compactHeight, "returning from N/A restores compact numeric height");
            foreach (int spacing in new[] { 1, 5, 20 })
            {
                config.RowsSpacing = spacing;
                LayoutAtDpi(overlay, config, rows, dpi);
                var layouts = Layouts(overlay);
                int expectedGap = (int)Math.Round(spacing * dpi / 96f);
                Call(overlay, "ActivateFont", style);
                using (var bitmap = new Bitmap(1, 1))
                {
                    bitmap.SetResolution(dpi, dpi);
                    using (var graphics = Graphics.FromImage(bitmap))
                    {
                        int expectedHeight = (int)Math.Ceiling((float)Call(overlay, "Ascent", graphics) + (float)Call(overlay, "Descent", graphics) + (float)Call(overlay, "GetStrokeWidth"));
                        Check((int)Field(layouts[0], "Height") == expectedHeight && (int)Field(layouts[1], "Y") == expectedHeight + expectedGap && overlay.Height > compactHeight,
                            "positive spacing keeps its existing font-cell height and logical gap: " + spacing);
                    }
                }
            }
            config.RowsSpacing = 0;
            var mixed = new[]
            {
                new OsdMetric("Ω CPU 🧠", "") { Key = "Group:CPU", Members = new List<OsdMetric> {
                    Metric("CpuTemperature", "Égj", "99°C", Style("Segoe UI", 32), "###°C"),
                    Metric("RamUsage", "RAM", "8.00G", Style(family, 10), "##.##G") } },
                Metric("Fps", "gyp Å", "144", Style(family, 48), "####"),
                Metric("CurrentTime", "🕒", "11:11 AM", Style("Segoe UI", 12), "##:## AM")
            };
            foreach (int width in new[] { 4000, 300 })
            {
                LayoutAtDpi(overlay, config, mixed, dpi, width);
                Check(WithinRows(overlay, mixed, dpi), "mixed fonts/sizes, Unicode fallback and width fitting do not clip or overlap rows at width " + width);
                int priorBottom = 0;
                foreach (object layout in Layouts(overlay))
                {
                    if ((int)Field(layout, "Y") < priorBottom) throw new Exception("Overlapping row boxes");
                    priorBottom = (int)Field(layout, "Y") + (int)Field(layout, "Height");
                }
            }
            config.Columns = 2;
            LayoutAtDpi(overlay, config, mixed, dpi, 300);
            Check(WithinRows(overlay, mixed, dpi) &&
                (int)Field(Layouts(overlay)[2], "Y") >= Math.Max((int)Field(Layouts(overlay)[0], "Height"), (int)Field(Layouts(overlay)[1], "Height")),
                "multiple compact columns reserve the tallest mixed-font row without overlap");
        }
    }

    private static bool WithinRows(OsdOverlay overlay, IReadOnlyList<OsdMetric> rows, int dpi)
    {
        using (var bitmap = new Bitmap(1, 1))
        {
            bitmap.SetResolution(dpi, dpi);
            using (var graphics = Graphics.FromImage(bitmap))
            for (int i = 0; i < rows.Count; i++)
            {
                var layout = Layouts(overlay)[i];
                float ascent = (float)Field(layout, "Ascent"), scale = (float)Field(layout, "Scale");
                foreach (object part in (IList)Field(layout, "Parts"))
                {
                    int member = (int)Field(part, "Member");
                    string text = member < 0 ? (string)Field(part, "Text") : rows[i].Members == null ? rows[i].Value : rows[i].Members[member].Value;
                    if (string.IsNullOrWhiteSpace(text)) continue;
                    Call(overlay, "ActivateFont", Field(part, "Style"));
                    int left = (int)Math.Round((int)Field(part, "X") * scale);
                    int right = (int)Math.Round(((int)Field(part, "X") + (int)Field(part, "Width")) * scale);
                    float stroke = (float)Call(overlay, "GetStrokeWidth");
                    using (var path = (GraphicsPath)Call(overlay, "CreateFittedTextPath", graphics, text, (float)Math.Max(1, right - left - stroke)))
                    {
                        var bounds = path.GetBounds();
                        float baseline = (float)Call(overlay, "Ascent", graphics);
                        if (ascent - baseline + bounds.Top - stroke / 2 < -0.01f ||
                            ascent - baseline + bounds.Bottom + stroke / 2 > (int)Field(layout, "Height") + 0.01f) return false;
                    }
                }
            }
        }
        return true;
    }

    private static int PixelGap(OsdOverlay overlay, int dpi)
    {
        using (var bitmap = new Bitmap(overlay.Width, overlay.Height))
        {
            bitmap.SetResolution(dpi, dpi);
            using (var graphics = Graphics.FromImage(bitmap)) { graphics.Clear(Color.Transparent); Call(overlay, "DrawOverlay", graphics); }
            int split = (int)Field(Layouts(overlay)[1], "Y"), firstBottom = -1, secondTop = bitmap.Height;
            for (int y = 0; y < bitmap.Height; y++)
            for (int x = 0; x < bitmap.Width; x++)
                if (bitmap.GetPixel(x, y).A > 8) { if (y < split) firstBottom = Math.Max(firstBottom, y); else secondTop = Math.Min(secondTop, y); }
            if (dpi == 96) bitmap.Save(Path.Combine(Path.GetTempPath(), "TrayTemps-compact-rows.png"));
            return secondTop - firstBottom - 1;
        }
    }

    private static void Compatibility()
    {
        var config = Config();
        config.ItemOrder = "CpuTemperature,GpuTemperature,CpuUsage,Fps,FpsOnePercentLow";
        config.MetricGroups = new Dictionary<string, string> { ["CpuTemperature"] = "  My CPU  ", ["CpuUsage"] = "my cpu", ["GpuTemperature"] = "GPU", ["Fps"] = "Game", ["FpsOnePercentLow"] = "game" };
        config.GroupLabels = new Dictionary<string, string> { ["My CPU"] = "Obsolete caption", ["Game"] = "" };
        config.GroupSeparator = " · obsolete · ";
        OsdMetricDefinitions.Migrate(config);
        var style = Style("Segoe UI", 16);
        var metrics = new[] { Metric("CpuUsage", "", "72%", style), Metric("GpuTemperature", "", "60°C", style), Metric("Fps", "FPS", "144", style), Metric("FpsOnePercentLow", "1%Low", "92", style) };
        var rows = OsdMetricDefinitions.Compose(config, metrics);
        Check(rows.Select(row => row.Label).SequenceEqual(new[] { "My CPU", "GPU", "Game" }) && rows[0].Members.Count == 1 && rows[2].Members.Count == 2,
            "trimmed group names supply captions, preserve case-insensitive membership and match hidden-member priority");
        string json = System.Text.Json.JsonSerializer.Serialize(config);
        var saved = System.Text.Json.JsonSerializer.Deserialize<OsdConfiguration>(json); OsdMetricDefinitions.Migrate(saved);
        Check(OsdMetricDefinitions.Compose(saved, metrics).Select(row => row.Label).SequenceEqual(rows.Select(row => row.Label)), "legacy caption/separator fields remain JSON-loadable without changing rendered captions");
        foreach (int preset in Enumerable.Range(0, OsdPresets.Names.Length))
        {
            var embedded = OsdPresets.LoadEmbedded(preset);
            var embeddedRows = OsdMetricDefinitions.Compose(embedded, OsdMetricDefinitions.Keys.Where(key => OsdMetricDefinitions.IsVisible(embedded, key)).Select(key => Metric(key, "", "10", embedded.MetricStyles[key])));
            Check(embeddedRows.Where(row => row.Members != null).All(row => row.Key == "Group:" + row.Label), "embedded preset retains groups using group-name captions: " + (preset + 1));
        }
    }
}
