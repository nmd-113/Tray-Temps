using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace TrayTemps
{
    public partial class OsdOverlay : Form
    {
        private const int WsExTransparent = 0x00000020;
        private const int WsExToolWindow = 0x00000080;
        private const int WsExLayered = 0x00080000;
        private const int WsExNoActivate = 0x08000000;
        private const int WmNcHitTest = 0x0084;
        private const int SwpNoSize = 0x0001;
        private const int SwpNoMove = 0x0002;
        private const int SwpNoActivate = 0x0010;
        private const int SwpNoOwnerZOrder = 0x0200;
        private const int UlwAlpha = 0x00000002;
        private const byte AcSrcOver = 0x00;
        private const byte AcSrcAlpha = 0x01;
        private const int TextStrokeAlpha = 220;
        private static readonly IntPtr HtTransparent = new IntPtr(-1);
        private static readonly IntPtr HwndTopMost = new IntPtr(-1);
        private readonly List<OsdMetric> _metrics = new List<OsdMetric>();
        private readonly List<RowLayout> _metricLayouts = new List<RowLayout>();
        private readonly Dictionary<string, int> _valueTemplateWidthCache =
            new Dictionary<string, int>(StringComparer.Ordinal);
        private readonly Dictionary<string, FontFamily> _unicodeFallbackFamilies =
            new Dictionary<string, FontFamily>(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, Font> _unicodeFallbackFonts =
            new Dictionary<string, Font>(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, FontFamily> _unicodeFontChoiceCache =
            new Dictionary<string, FontFamily>(StringComparer.Ordinal);
        private readonly Dictionary<string, byte[]> _unicodeFontCmapCache =
            new Dictionary<string, byte[]>(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, UnicodeTextRun[]> _unicodeTextRunCache =
            new Dictionary<string, UnicodeTextRun[]>(StringComparer.Ordinal);
        private Bitmap _presentationBitmap;
        private Graphics _presentationGraphics;
        private Bitmap _staticPresentationBitmap;
        private Graphics _staticPresentationGraphics;
        private bool _staticPresentationDirty = true;
        private GraphicsPath _backgroundPath;
        private Size _presentationSize;
        private Size _backgroundPathSize;
        private int _presentationDpi;
        private int _backgroundPathRadius = -1;
        private Font _displayFont;
        private OsdConfiguration _configuration = new OsdConfiguration();
        private Rectangle _workingArea;
        private Color _textColor = Color.WhiteSmoke;
        private Color _backgroundColor = Color.FromArgb(24, 24, 24);
        private readonly Dictionary<string, Font> _metricFonts = new Dictionary<string, Font>(StringComparer.Ordinal);
        private string _fontContextKey = "";
        private float? _drawBaseline;
        private int _layoutDpi;
        private float _contentScale = 1f;

        public OsdOverlay()
        {
            InitializeComponent();
        }

        protected override bool ShowWithoutActivation => true;

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams parameters = base.CreateParams;
                if (!EmbeddedFonts.IsDesignTime(this))
                    parameters.ExStyle |= WsExTransparent | WsExToolWindow | WsExLayered | WsExNoActivate;
                return parameters;
            }
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            if (EmbeddedFonts.IsDesignTime(this))
                return;

            ApplyPassiveUpdateMode(Handle);
        }

        private static void ApplyPassiveUpdateMode(IntPtr windowHandle)
        {
            try
            {
                // Native BOOL is four bytes, not the size of a managed bool.
                int enabled = 1;
                int result = DwmSetWindowAttribute(windowHandle, 16, ref enabled, sizeof(int));
                System.Diagnostics.Trace.WriteLine(
                    "OSD passive update: HWND=0x" + windowHandle.ToInt64().ToString("X") +
                    " HRESULT=0x" + result.ToString("X8") +
                    (result < 0 ? " failed; continuing with existing renderer." : " accepted; game effect unverified."));
            }
            catch (Exception ex) when (ex is DllNotFoundException ||
                                       ex is EntryPointNotFoundException ||
                                       ex is ExternalException)
            {
                System.Diagnostics.Trace.WriteLine(
                    "OSD passive update unavailable: HWND=0x" + windowHandle.ToInt64().ToString("X") + " " + ex);
            }
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            if (EmbeddedFonts.IsDesignTime(this))
                return;

            // A hidden per-monitor-DPI form can receive its final DPI only when shown.
            // Recalculate if that final DPI differs from the cached measurements.
            if (_layoutDpi != DeviceDpi)
                RebuildLayout();
            ApplyConfiguredPosition();
            PresentLayeredWindow();
        }

        protected override void WndProc(ref Message message)
        {
            if (!EmbeddedFonts.IsDesignTime(this) && message.Msg == WmNcHitTest)
            {
                message.Result = HtTransparent;
                return;
            }

            base.WndProc(ref message);
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            DisposeRenderResources();
            base.OnFormClosed(e);
        }

        private void DisposeRenderResources()
        {
            DisposePresentationSurface();
            DisposeDisplayFont();
            DisposeUnicodeFallbackFonts();
            _unicodeTextRunCache.Clear();
            _unicodeFontChoiceCache.Clear();
            _unicodeFontCmapCache.Clear();
            foreach (FontFamily family in _unicodeFallbackFamilies.Values)
                family?.Dispose();
            _unicodeFallbackFamilies.Clear();
        }

        private void DisposePresentationSurface()
        {
            _presentationGraphics?.Dispose();
            _presentationGraphics = null;
            _presentationBitmap?.Dispose();
            _presentationBitmap = null;
            DisposeStaticPresentationSurface();
            _backgroundPath?.Dispose();
            _backgroundPath = null;
            _presentationSize = Size.Empty;
            _backgroundPathSize = Size.Empty;
            _backgroundPathRadius = -1;
            _presentationDpi = 0;
        }

        private void DisposeStaticPresentationSurface()
        {
            _staticPresentationGraphics?.Dispose();
            _staticPresentationGraphics = null;
            _staticPresentationBitmap?.Dispose();
            _staticPresentationBitmap = null;
            _staticPresentationDirty = true;
        }

        private void DisposeDisplayFont()
        {
            foreach (Font font in _metricFonts.Values) font.Dispose();
            _metricFonts.Clear();
            _displayFont = null;
        }

        internal void UpdateDisplay(
            OsdConfiguration configuration,
            IEnumerable<OsdMetric> metrics,
            Rectangle workingArea)
        {
            OsdConfiguration nextConfiguration = configuration ?? new OsdConfiguration();
            IReadOnlyList<OsdMetric> nextMetrics = metrics as IReadOnlyList<OsdMetric> ??
                (metrics ?? Enumerable.Empty<OsdMetric>()).ToList();
            if (!HasDisplayChanged(
                    nextConfiguration,
                    nextMetrics,
                    workingArea,
                    out bool layoutChanged,
                    out bool fontSettingsChanged,
                    out bool graphOnlyChanged))
            {
                return;
            }

            bool visualSettingsChanged = layoutChanged ||
                _configuration.BackgroundColor != nextConfiguration.BackgroundColor ||
                _configuration.BackgroundCornerRoundness != nextConfiguration.BackgroundCornerRoundness;
            bool positionChanged = layoutChanged ||
                _workingArea != workingArea ||
                _configuration.Position != nextConfiguration.Position ||
                _configuration.ExternalSpacing != nextConfiguration.ExternalSpacing;
            bool configurationSnapshotChanged = layoutChanged || _workingArea != workingArea ||
                _configuration.Position != nextConfiguration.Position ||
                _configuration.ExternalSpacing != nextConfiguration.ExternalSpacing ||
                _configuration.BackgroundColor != nextConfiguration.BackgroundColor ||
                _configuration.BackgroundCornerRoundness != nextConfiguration.BackgroundCornerRoundness ||
                _configuration.OpacityPercent != nextConfiguration.OpacityPercent ||
                _configuration.BackgroundOpacityPercent != nextConfiguration.BackgroundOpacityPercent;
            if (configurationSnapshotChanged)
                _configuration = nextConfiguration.Clone();
            _workingArea = workingArea;
            _metrics.Clear();
            _metrics.AddRange(nextMetrics);

            if (_workingArea.Width > 0 && _workingArea.Height > 0 &&
                (!IsHandleCreated || Screen.FromControl(this).WorkingArea != _workingArea))
            {
                Location = _workingArea.Location;
            }

            if (visualSettingsChanged)
                ApplyVisualSettings();
            if (fontSettingsChanged)
                InvalidateFontSettings();
            if (layoutChanged)
                RebuildLayout();
            if (positionChanged)
                ApplyConfiguredPosition();
            _staticPresentationDirty |= !graphOnlyChanged;
            PresentLayeredWindow();
        }

        private bool HasDisplayChanged(
            OsdConfiguration configuration,
            IReadOnlyList<OsdMetric> metrics,
            Rectangle workingArea,
            out bool layoutChanged,
            out bool fontSettingsChanged,
            out bool graphOnlyChanged)
        {
            fontSettingsChanged = _displayFont == null || _layoutDpi != DeviceDpi ||
                _configuration.FontFamily != configuration.FontFamily || _configuration.FontSize != configuration.FontSize;
            layoutChanged = _displayFont == null || _layoutDpi != DeviceDpi || _metrics.Count != metrics.Count ||
                _workingArea.Size != workingArea.Size ||
                _configuration.Columns != configuration.Columns || _configuration.ScreenMargin != configuration.ScreenMargin ||
                _configuration.RowsSpacing != configuration.RowsSpacing || _configuration.ColumnsSpacing != configuration.ColumnsSpacing ||
                _configuration.GroupedValuesGapPercent != configuration.GroupedValuesGapPercent ||
                _configuration.FrameTimeGraphWidth != configuration.FrameTimeGraphWidth ||
                _configuration.FrameTimeGraphHeight != configuration.FrameTimeGraphHeight ||
                _configuration.LabelValueSpacing != configuration.LabelValueSpacing ||
                !MetricGapsEqual(_configuration.MetricLabelValueGaps, configuration.MetricLabelValueGaps) ||
                _configuration.FontFamily != configuration.FontFamily || _configuration.FontSize != configuration.FontSize;
            bool changed = layoutChanged || _workingArea != workingArea ||
                _configuration.Position != configuration.Position || _configuration.ExternalSpacing != configuration.ExternalSpacing ||
                _configuration.BackgroundColor != configuration.BackgroundColor ||
                _configuration.BackgroundCornerRoundness != configuration.BackgroundCornerRoundness ||
                _configuration.OpacityPercent != configuration.OpacityPercent ||
                _configuration.BackgroundOpacityPercent != configuration.BackgroundOpacityPercent;
            bool textChanged = changed;
            for (int i = 0; i < Math.Min(_metrics.Count, metrics.Count); i++)
            {
                var before = _metrics[i]; var after = metrics[i];
                if (before.Key != after.Key || before.Label != after.Label || (before.Members == null) != (after.Members == null))
                    layoutChanged = true;
                int beforeCount = before.Members?.Count ?? 1, afterCount = after.Members?.Count ?? 1;
                if (beforeCount != afterCount) layoutChanged = true;
                for (int j = 0; j < Math.Min(beforeCount, afterCount); j++)
                {
                    var x = before.Members == null ? before : before.Members[j];
                    var y = after.Members == null ? after : after.Members[j];
                    var xs = x.Style ?? OsdMetricDefinitions.LegacyStyle(_configuration, x.Key ?? "");
                    var ys = y.Style ?? OsdMetricDefinitions.LegacyStyle(configuration, y.Key ?? "");
                    fontSettingsChanged |= xs.FontFamily != ys.FontFamily || xs.FontSize != ys.FontSize;
                    if (x.Key != y.Key || x.Label != y.Label || xs.FontFamily != ys.FontFamily || xs.FontSize != ys.FontSize ||
                        x.ValueWidthTemplate != y.ValueWidthTemplate ||
                        x.AlternateValueWidthTemplate != y.AlternateValueWidthTemplate ||
                        (y.Key != "FrameTimeGraph" && (x.Value == "N/A") != (y.Value == "N/A")) ||
                        (string.IsNullOrEmpty(y.ValueWidthTemplate) && x.Value != y.Value))
                        layoutChanged = true;
                    bool textOrColorChanged = x.Value != y.Value || x.TextColor != y.TextColor || xs.LabelColor != ys.LabelColor || xs.ValueColor != ys.ValueColor;
                    textChanged |= textOrColorChanged;
                    changed |= textOrColorChanged || !ReferenceEquals(x.Graph, y.Graph) || x.GraphOffset != y.GraphOffset;
                }
            }
            graphOnlyChanged = !textChanged && !layoutChanged;
            return changed || layoutChanged;
        }

        private static bool MetricGapsEqual(Dictionary<string, int> first, Dictionary<string, int> second)
        {
            if (ReferenceEquals(first, second)) return true;
            if (first == null || second == null || first.Count != second.Count) return false;
            foreach (var pair in first)
                if (!second.TryGetValue(pair.Key, out int value) || value != pair.Value) return false;
            return true;
        }

        private void ApplyVisualSettings()
        {
            _backgroundColor = _configuration.BackgroundColor.HasValue
                ? OsdColorHelper.GetOpaqueColor(_configuration.BackgroundColor.Value) : OsdColorHelper.GetDefaultBackground(false);
        }

        private void InvalidateFontSettings()
        {
            // Font and DPI changes invalidate glyph measurements. Geometry-only edits reuse these contexts.
            DisposeUnicodeFallbackFonts();
            _unicodeTextRunCache.Clear(); _unicodeFontChoiceCache.Clear(); _valueTemplateWidthCache.Clear();
            DisposeDisplayFont();
        }

        private void ActivateFont(OsdMetricStyle style)
        {
            string family = style?.FontFamily ?? _configuration.FontFamily;
            float size = style?.FontSize ?? _configuration.FontSize;
            if (string.IsNullOrWhiteSpace(family)) family = OsdFontHelper.DefaultFamily;
            size = Math.Max(6, Math.Min(48, size));
            string key = family + "|" + size.ToString("R", CultureInfo.InvariantCulture);
            if (!_metricFonts.TryGetValue(key, out var font)) _metricFonts[key] = font = OsdFontHelper.CreateFont(family, size);
            _displayFont = font; _fontContextKey = key;
        }

        private float Ascent(Graphics graphics) => GetFontBaseline(_displayFont.FontFamily, _displayFont.Style, graphics.DpiY * _displayFont.SizeInPoints / 72f);
        private float Descent(Graphics graphics)
        {
            var family = _displayFont.FontFamily;
            return graphics.DpiY * _displayFont.SizeInPoints / 72f * family.GetCellDescent(_displayFont.Style) / family.GetEmHeight(_displayFont.Style);
        }

        private void RebuildLayout()
        {
            _metricLayouts.Clear();
            int count = _metrics.Count;
            int columns = Math.Max(1, Math.Min(_configuration.Columns, Math.Max(1, count)));
            int rows = Math.Max(1, (count + columns - 1) / columns);
            int padding = GetConfiguredPadding(), rowGap = GetRowsSpacing(), columnGap = GetColumnsSpacing();
            int[] widths = new int[columns], heights = new int[rows];
            bool[] graphLines = new bool[rows];
            int fullWidth;
            using (var bitmap = new Bitmap(1, 1))
            {
                bitmap.SetResolution(DeviceDpi, DeviceDpi);
                using (var graphics = Graphics.FromImage(bitmap))
                {
                    foreach (var row in _metrics)
                    {
                        var layout = new RowLayout();
                        var members = row.Members ?? new List<OsdMetric> { row };
                        layout.IsGroup = row.Members != null;
                        layout.IsGraph = row.Key == "FrameTimeGraph";
                        int x = 0; float ascent = 0, descent = 0;
                        if (row.Members != null && !string.IsNullOrEmpty(row.Label))
                        {
                            ActivateFont(members[0].Style);
                            int width = MeasureDisplayTextWidth(graphics, row.Label);
                            layout.Parts.Add(new TextPlacement(row.Label, members[0].Style, true, x, width) { Owner = 0, IsGroupCaption = true });
                            x += width + ScaleLogical(OsdMetricDefinitions.LabelGap(_configuration, row.Key, row.Label));
                            ascent = Ascent(graphics); descent = Descent(graphics);
                        }
                        for (int member = 0; member < members.Count; member++)
                        {
                            var metric = members[member]; ActivateFont(metric.Style);
                            if (!layout.IsGraph || !string.IsNullOrWhiteSpace(metric.Label))
                            { ascent = Math.Max(ascent, Ascent(graphics)); descent = Math.Max(descent, Descent(graphics)); }
                            string label = metric.Label;
                            int labelWidth = string.IsNullOrEmpty(label) ? 0 : MeasureDisplayTextWidth(graphics, label);
                            if (labelWidth > 0)
                            {
                                layout.Parts.Add(new TextPlacement(label, metric.Style, true, x, labelWidth) { Owner = member });
                                int labelGap = ScaleLogical(OsdMetricDefinitions.LabelGap(_configuration, metric.Key, null));
                                x += labelWidth + labelGap;
                            }
                            int valueWidth = layout.IsGraph
                                ? ScaleLogical(ValueHelper.ClampInt(_configuration.FrameTimeGraphWidth,
                                    OsdConfiguration.MinFrameTimeGraphWidth, OsdConfiguration.MaxFrameTimeGraphWidth))
                                : GetPrimaryValueSlotWidth(graphics, metric, MeasureDisplayTextWidth(graphics, metric.Value));
                            layout.Parts.Add(new TextPlacement(metric.Value, metric.Style, false, x, valueWidth, member));
                            x += valueWidth;
                            if (member + 1 < members.Count)
                            {
                                string separator = OsdMetricDefinitions.GroupValueSeparator;
                                using (var format = StringFormat.GenericTypographic)
                                {
                                    format.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces | StringFormatFlags.NoWrap;
                                    float gapScale = ValueHelper.ClampInt(_configuration.GroupedValuesGapPercent, 100, 200) / 100f;
                                    int separatorWidth = (int)Math.Ceiling(graphics.MeasureString(separator, _displayFont, int.MaxValue, format).Width * gapScale);
                                    layout.Parts.Add(new TextPlacement(separator, metric.Style, false, x, separatorWidth) { Owner = member });
                                    x += separatorWidth;
                                }
                            }
                        }
                        layout.Width = x; layout.Ascent = ascent;
                        layout.Height = (int)Math.Ceiling(ascent + descent + GetStrokeWidth());
                        if (layout.IsGraph) layout.Height = Math.Max(layout.Height, GetFrameTimeGraphHeight());
                        int index = _metricLayouts.Count;
                        graphLines[index / columns] |= layout.IsGraph;
                        widths[index % columns] = Math.Max(widths[index % columns], x);
                        heights[index / columns] = Math.Max(heights[index / columns], layout.Height);
                        _metricLayouts.Add(layout);
                    }
                    fullWidth = padding * 2 + widths.Sum() + columnGap * (columns - 1);
                    int clientWidth = Math.Max(1, Math.Min(_workingArea.Width > 0 ? _workingArea.Width : fullWidth, fullWidth));
                    int available = clientWidth - padding * 2 - columnGap * (columns - 1);
                    // When padding/gaps consume the screen, retain usable columns
                    // and let the whole-layout fit below scale them together.
                    if (available >= columns && widths.Sum() > available)
                        for (int c = 0; c < columns; c++) widths[c] = available / columns + (c < available % columns ? 1 : 0);
                    fullWidth = padding * 2 + widths.Sum() + columnGap * (columns - 1);
                    if (_configuration.RowsSpacing == 0 || graphLines.Any(line => line))
                    {
                        Array.Clear(heights, 0, heights.Length);
                        for (int i = 0; i < _metricLayouts.Count; i++)
                        {
                            var layout = _metricLayouts[i];
                            float scale = layout.Width > widths[i % columns] ? widths[i % columns] / (float)Math.Max(1, layout.Width) : 1f;
                            if (!layout.IsGraph && (_configuration.RowsSpacing == 0 || graphLines[i / columns]))
                                MeasureCompactRow(graphics, layout, _metrics[i], scale);
                            heights[i / columns] = Math.Max(heights[i / columns], layout.Height);
                        }
                    }
                }
            }
            int fullHeight = padding * 2 + heights.Sum() + rowGap * (rows - 1);
            // Fit the whole layout uniformly when it exceeds the screen.
            // Normal-sized layouts retain their exact configured glyphs and spacing.
            int fittedHeight = Math.Max(1, Math.Min(_workingArea.Height > 0 ? _workingArea.Height : fullHeight, fullHeight));
            int fittedWidth = Math.Max(1, Math.Min(_workingArea.Width > 0 ? _workingArea.Width : fullWidth, fullWidth));
            _contentScale = Math.Min(1f, Math.Min(fittedHeight / (float)Math.Max(1, fullHeight), fittedWidth / (float)Math.Max(1, fullWidth)));
            ClientSize = new Size(Math.Max(1, Math.Min(fittedWidth, (int)Math.Ceiling(fullWidth * _contentScale))),
                Math.Max(1, Math.Min(fittedHeight, (int)Math.Ceiling(fullHeight * _contentScale))));
            for (int i = 0; i < _metricLayouts.Count; i++)
            {
                var row = _metricLayouts[i]; int column = i % columns, line = i / columns;
                row.X = padding + widths.Take(column).Sum() + columnGap * column;
                row.Y = padding + heights.Take(line).Sum() + rowGap * line;
                // Center the graph and neighboring glyph bounds within their shared line.
                if (graphLines[line]) row.Y += (heights[line] - row.Height) / 2;
                row.Scale = row.Width > widths[column] ? widths[column] / (float)Math.Max(1, row.Width) : 1f;
            }
            _layoutDpi = DeviceDpi;
        }

        private void MeasureCompactRow(Graphics graphics, RowLayout layout, OsdMetric row, float scale)
        {
            float top = 0, bottom = 0;
            foreach (var part in layout.Parts)
            {
                OsdMetric metric = part.Member < 0 ? null : row.Members == null ? row : row.Members[part.Member];
                ActivateFont(part.Style);
                float baseline = Ascent(graphics);
                float inset = GetStrokeWidth() / 2f + 0.5f; // outline plus antialias coverage
                Action<string> include = text =>
                {
                    if (string.IsNullOrWhiteSpace(text)) return;
                    // Use the same fitting as drawing; narrow layouts can scale glyphs vertically.
                    using (var path = CreateFittedTextPath(graphics, text, Math.Max(1, part.Width * scale - GetStrokeWidth())))
                    {
                        if (path.PointCount == 0) return;
                        var bounds = path.GetBounds();
                        top = Math.Min(top, bounds.Top - baseline - inset);
                        bottom = Math.Max(bottom, bounds.Bottom - baseline + inset);
                    }
                };
                include(part.Text);
                if (metric != null && metric.Value != "N/A" && !string.IsNullOrEmpty(metric.ValueWidthTemplate))
                {
                    // Reserve every digit and both clock periods so
                    // normal value refreshes keep their baseline and never clip descenders.
                    for (char digit = '0'; digit <= '9'; digit++) include(digit.ToString());
                    include(metric.ValueWidthTemplate.Replace('#', '0'));
                    include(metric.AlternateValueWidthTemplate?.Replace('#', '0'));
                }
            }
            layout.Ascent = (float)Math.Ceiling(-top);
            layout.Height = Math.Max(1, (int)Math.Ceiling(layout.Ascent + bottom));
        }

        protected override void OnPaint(PaintEventArgs e) { base.OnPaint(e); DrawOverlay(e.Graphics); }

        private void DrawOverlay(Graphics graphics)
        {
            DrawLayers(graphics, drawGraphs: true, drawText: true);
        }

        private void DrawLayers(Graphics graphics, bool drawGraphs, bool drawText)
        {
            if (_metricLayouts.Count != _metrics.Count) return;
            var state = graphics.Save();
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            try
            {
                graphics.ScaleTransform(_contentScale, _contentScale);
                for (int rowIndex = 0; rowIndex < _metricLayouts.Count; rowIndex++)
                {
                    var row = _metricLayouts[rowIndex];
                    if (!drawText && !row.IsGraph) continue;
                    var metrics = _metrics[rowIndex].Members;
                    foreach (var part in row.Parts)
                    {
                        // Numeric values refresh without rebuilding their measured slots.
                        string text = part.Member >= 0 ? (metrics == null ? _metrics[rowIndex].Value : metrics[part.Member].Value) : part.Text;
                        var style = part.Member >= 0 ? (metrics == null ? _metrics[rowIndex].Style : metrics[part.Member].Style) : part.Style;
                        // Resolve label/color-only edits from current metrics as well.
                        if (metrics != null && part.Owner >= 0) style = metrics[part.Owner].Style;
                        else if (metrics == null) style = _metrics[rowIndex].Style;
                        ActivateFont(style);
                        _drawBaseline = row.IsGraph ? (float?)null : row.Y + row.Ascent;
                        var color = style == null ? _metrics[rowIndex].TextColor ?? _textColor : Color.FromArgb(part.Label ? style.LabelColor : style.ValueColor);
                        int left = (int)Math.Round(part.X * row.Scale);
                        int right = (int)Math.Round((part.X + part.Width) * row.Scale);
                        if (row.IsGraph && part.Member >= 0)
                        {
                            int graphHeight = GetFrameTimeGraphHeight();
                            if (drawGraphs) DrawFrameGraph(graphics, _metrics[rowIndex].Graph,
                                new Rectangle(row.X + left, row.Y + (row.Height - graphHeight) / 2,
                                    Math.Max(1, right - left), graphHeight), color, _metrics[rowIndex].GraphOffset);
                            continue;
                        }
                        if (!drawText) continue;
                        if (row.IsGraph && string.IsNullOrEmpty(_metrics[rowIndex].Label)) continue;
                        // Reserve numeric padding for numbers. N/A starts at the slot's
                        // left edge, without inheriting unused digits or unit spacing.
                        bool numericValue = part.Member >= 0 && !row.IsGraph && text != "N/A" &&
                            (metrics == null ? _metrics[rowIndex].Key : metrics[part.Member].Key) != "CurrentTime";
                        DrawDisplayText(graphics, text,
                            new Rectangle(row.X + left, row.Y, Math.Max(0, right - left), row.Height),
                            color, numericValue);
                    }
                }
            }
            finally { _drawBaseline = null; graphics.Restore(state); }
        }

        private void DrawFrameGraph(Graphics graphics, FrameGraphPoint[] points, Rectangle bounds, Color color, float offset = 0)
        {
            if (bounds.Width <= 0 || bounds.Height <= 0) return;
            points = points ?? Array.Empty<FrameGraphPoint>();
            double max = 33.34;
            foreach (var point in points)
                if (!double.IsNaN(point.Peak) && !double.IsInfinity(point.Peak)) max = Math.Max(max, point.Peak * 1.05);
            using (var line = new Pen(color, 1.25f * DeviceDpi / 96f))
            {
                // Reserve only the stroke's radius so edge samples remain inside the bounds.
                var plot = RectangleF.Inflate(bounds, -line.Width / 2, -line.Width / 2);
                if (plot.Width <= 0 || plot.Height <= 0) return;
                var state = graphics.Save();
                graphics.SetClip(bounds, CombineMode.Intersect);
                graphics.SmoothingMode = SmoothingMode.AntiAlias;
                try
                {
                    Color guideColor = _backgroundColor.GetBrightness() < 0.5f ? Color.White : Color.Black;
                    using (var guide = new Pen(Color.FromArgb(45, guideColor), Math.Max(1f, DeviceDpi / 96f)))
                    {
                        graphics.DrawLine(guide, plot.Left, bounds.Top + guide.Width / 2, plot.Right, bounds.Top + guide.Width / 2);
                        graphics.DrawLine(guide, plot.Left, bounds.Bottom - guide.Width / 2, plot.Right, bounds.Bottom - guide.Width / 2);
                    }
                    PointF? previous = null;
                    for (int i = 0; i < points.Length; i++)
                    {
                        var point = points[i];
                        // Empty buckets can be spanned by a single long frame. These
                        // points belong to one continuous capture; stale/source resets
                        // clear the buffer, so only join actual completed samples.
                        if (double.IsNaN(point.Mean) || double.IsInfinity(point.Mean) || point.Mean < 0) continue;
                        float x = plot.Left + (i - offset) * plot.Width / Math.Max(1, points.Length - 1);
                        float y = plot.Bottom - (float)(point.Mean / max * plot.Height);
                        double peakValue = double.IsNaN(point.Peak) || double.IsInfinity(point.Peak) ? point.Mean : point.Peak;
                        float peak = plot.Bottom - (float)(peakValue / max * plot.Height);
                        graphics.DrawLine(line, x, y, x, peak);
                        var current = new PointF(x, y);
                        if (previous.HasValue) graphics.DrawLine(line, previous.Value, current);
                        previous = current;
                    }
                }
                finally { graphics.Restore(state); }
            }
        }

        private void DrawDisplayText(Graphics graphics, string text, Rectangle bounds, Color textColor, bool padToSlot)
        {
            if (bounds.Width <= 0 || bounds.Height <= 0 || string.IsNullOrWhiteSpace(text)) return;
            Color opaque = Color.FromArgb(textColor.R, textColor.G, textColor.B);
            Color stroke = (0.299 * opaque.R + 0.587 * opaque.G + 0.114 * opaque.B) >= 145
                ? Color.FromArgb(TextStrokeAlpha, 0, 0, 0) : Color.FromArgb(TextStrokeAlpha, 255, 255, 255);
            float strokeWidth = GetStrokeWidth();
            using (var path = CreateFittedTextPath(graphics, text, Math.Max(1, bounds.Width - strokeWidth)))
            {
                if (path == null || path.PointCount == 0) return;
                var pathBounds = path.GetBounds();
                float x = padToSlot ? bounds.Right - strokeWidth / 2 - pathBounds.Right
                    : bounds.Left + strokeWidth / 2 - pathBounds.Left;
                float y = _drawBaseline.HasValue ? _drawBaseline.Value - Ascent(graphics) : bounds.Top + (bounds.Height - pathBounds.Height) / 2 - pathBounds.Top;
                using (var transform = new Matrix()) { transform.Translate(x, y); path.Transform(transform); }
                using (var pen = new Pen(stroke, strokeWidth) { LineJoin = LineJoin.Round }) graphics.DrawPath(pen, path);
                using (var brush = new SolidBrush(opaque)) graphics.FillPath(brush, path);
            }
        }
        private GraphicsPath CreateFittedTextPath(Graphics graphics, string text, float availableWidth)
        {
            GraphicsPath path = CreateTextPath(graphics, text);
            float width = path.GetBounds().Width;
            if (width > availableWidth && width > 0)
            {
                // Fit the complete glyph path; labels, values and icons never become ellipses.
                using (var transform = new Matrix())
                {
                    float scale = Math.Max(0, availableWidth) / width;
                    transform.Scale(scale, scale);
                    path.Transform(transform);
                }
            }
            return path;
        }
        private Size MeasureDisplayTextSize(Graphics graphics, string text)
        {
            using (GraphicsPath path = CreateTextPath(graphics, text))
            {
                RectangleF bounds = path.GetBounds();
                float strokeWidth = GetStrokeWidth();
                return new Size(
                    Math.Max(1, (int)Math.Ceiling(bounds.Width + strokeWidth)),
                    Math.Max(1, (int)Math.Ceiling(bounds.Height + strokeWidth)));
            }
        }

        private int MeasureDisplayTextWidth(Graphics graphics, string text)
        {
            return MeasureDisplayTextSize(graphics, text).Width;
        }

        private int GetPrimaryValueSlotWidth(
            Graphics graphics,
            OsdMetric metric,
            int measuredValueWidth)
        {
            // Missing readings need no unused digits or unit suffix. Restore the
            // stable numeric slot when data becomes available again.
            if (metric.Value == "N/A" || string.IsNullOrEmpty(metric.ValueWidthTemplate))
                return measuredValueWidth;

            return Math.Max(
                measuredValueWidth,
                Math.Max(
                    MeasureStableValueTemplateWidth(graphics, metric.ValueWidthTemplate),
                    string.IsNullOrEmpty(metric.AlternateValueWidthTemplate) ? 0 :
                        MeasureStableValueTemplateWidth(graphics, metric.AlternateValueWidthTemplate)));
        }

        private int MeasureStableValueTemplateWidth(Graphics graphics, string template)
        {
            string cacheKey = _fontContextKey + "|" + DeviceDpi + "|" + template;
            if (_valueTemplateWidthCache.TryGetValue(cacheKey, out int cachedWidth))
                return cachedWidth;

            int width = 0;
            for (char digit = '0'; digit <= '9'; digit++)
                width = Math.Max(width, MeasureDisplayTextWidth(graphics, template.Replace('#', digit)));

            if (template.All(character => character == '#'))
            {
                // Repeated digits alone miss mixed first/last glyph bearings:
                // e.g. 6007 can be wider than any of 0000, 1111, ..., 9999.
                float advance = 0, leftOverhang = 0, rightOverhang = 0;
                using (StringFormat format = StringFormat.GenericTypographic)
                {
                    format.FormatFlags |= StringFormatFlags.NoWrap | StringFormatFlags.MeasureTrailingSpaces;
                    for (char digit = '0'; digit <= '9'; ++digit)
                    {
                        string text = digit.ToString();
                        float digitAdvance = graphics.MeasureString(text, _displayFont, int.MaxValue, format).Width;
                        using (GraphicsPath path = CreateTextPath(graphics, text))
                        {
                            RectangleF bounds = path.GetBounds();
                            advance = Math.Max(advance, digitAdvance);
                            leftOverhang = Math.Max(leftOverhang, -bounds.Left);
                            rightOverhang = Math.Max(rightOverhang, bounds.Right - digitAdvance);
                        }
                    }
                }
                width = Math.Max(width, (int)Math.Ceiling(template.Length * advance + leftOverhang + rightOverhang + GetStrokeWidth()));
                width = Math.Max(width, MeasureDisplayTextWidth(graphics, "N/A"));
            }

            _valueTemplateWidthCache[cacheKey] = width;
            return width;
        }

        private GraphicsPath CreateTextPath(Graphics graphics, string text)
        {
            var path = new GraphicsPath();
            float emSize = graphics.DpiY * _displayFont.SizeInPoints / 72f;
            if (IsAscii(text))
            {
                AddTextPathRun(path, graphics, text ?? string.Empty, _displayFont.FontFamily, _displayFont, emSize, 0f, null);
                return path;
            }

            UnicodeTextRun[] runs = GetUnicodeTextRuns(graphics, text ?? string.Empty, emSize);
            if (runs.Length == 0)
                return path;

            float x = 0f;
            using (StringFormat format = StringFormat.GenericTypographic)
            {
                format.FormatFlags |= StringFormatFlags.NoWrap;
                foreach (UnicodeTextRun run in runs)
                {
                    path.AddString(
                        run.Text,
                        run.Family,
                        (int)run.Font.Style,
                        emSize,
                        new PointF(x, run.BaselineOffset),
                        format);
                    x += run.Advance;
                }
            }

            return path;
        }

        private UnicodeTextRun[] GetUnicodeTextRuns(
            Graphics graphics,
            string text,
            float emSize)
        {
            string cacheKey = _fontContextKey + "|" + DeviceDpi.ToString(CultureInfo.InvariantCulture) + "\0" + text;
            if (_unicodeTextRunCache.TryGetValue(cacheKey, out UnicodeTextRun[] cachedRuns))
                return cachedRuns;

            List<string> textElements = GetUnicodeTextElements(text);
            if (textElements.Count == 0)
                return Array.Empty<UnicodeTextRun>();

            var runs = new List<UnicodeTextRun>();
            float primaryBaseline = GetFontBaseline(_displayFont.FontFamily, _displayFont.Style, emSize);
            FontFamily runFamily = GetFontFamilyForTextElement(graphics, textElements[0]);
            var runText = new StringBuilder();
            using (StringFormat format = StringFormat.GenericTypographic)
            {
                format.FormatFlags |= StringFormatFlags.NoWrap | StringFormatFlags.MeasureTrailingSpaces;
                foreach (string element in textElements)
                {
                    FontFamily elementFamily = GetFontFamilyForTextElement(graphics, element);
                    if (!SameFontFamily(runFamily, elementFamily))
                    {
                        AddUnicodeTextRun(runs, graphics, runText.ToString(), runFamily, primaryBaseline, emSize, format);
                        runText.Clear();
                        runFamily = elementFamily;
                    }

                    runText.Append(element);
                }

                AddUnicodeTextRun(runs, graphics, runText.ToString(), runFamily, primaryBaseline, emSize, format);
            }

            UnicodeTextRun[] result = runs.ToArray();
            if (_unicodeTextRunCache.Count >= 128)
                _unicodeTextRunCache.Clear();
            _unicodeTextRunCache[cacheKey] = result;
            return result;
        }

        private void AddUnicodeTextRun(
            IList<UnicodeTextRun> runs,
            Graphics graphics,
            string text,
            FontFamily family,
            float primaryBaseline,
            float emSize,
            StringFormat format)
        {
            if (string.IsNullOrEmpty(text))
                return;

            Font font = GetFontForFamily(family);
            float advance = graphics.MeasureString(text, font, int.MaxValue, format).Width;
            float baselineOffset = primaryBaseline - GetFontBaseline(family, font.Style, emSize);
            runs.Add(new UnicodeTextRun(text, family, font, advance, baselineOffset));
        }

        private float AddTextPathRun(
            GraphicsPath path,
            Graphics graphics,
            string text,
            FontFamily family,
            Font font,
            float emSize,
            float y,
            StringFormat format,
            float x = 0f)
        {
            if (string.IsNullOrEmpty(text))
                return 0f;

            bool measureAdvance = format != null;
            StringFormat runFormat = format;
            bool disposeFormat = false;
            if (runFormat == null)
            {
                runFormat = StringFormat.GenericTypographic;
                runFormat.FormatFlags |= StringFormatFlags.NoWrap | StringFormatFlags.MeasureTrailingSpaces;
                disposeFormat = true;
            }

            try
            {
                path.AddString(
                    text,
                    family,
                    (int)font.Style,
                    emSize,
                    new PointF(x, y),
                    runFormat);
                return measureAdvance
                    ? graphics.MeasureString(text, font, int.MaxValue, runFormat).Width
                    : 0f;
            }
            finally
            {
                if (disposeFormat)
                    runFormat.Dispose();
            }
        }

        private FontFamily GetFontFamilyForTextElement(Graphics graphics, string textElement)
        {
            if (IsAscii(textElement))
                return _displayFont.FontFamily;

            if (_unicodeFontChoiceCache.TryGetValue(_fontContextKey + "|" + textElement, out FontFamily cachedFamily))
                return cachedFamily;

            FontFamily selectedFamily = _displayFont.FontFamily;
            if (FontSupportsTextElement(graphics, _displayFont, textElement))
            {
                CacheUnicodeFontChoice(textElement, selectedFamily);
                return selectedFamily;
            }

            bool hasSurrogatePair = textElement.Any(char.IsSurrogate);
            string[] fallbackNames = hasSurrogatePair
                ? new[] { "Segoe UI Emoji", "Segoe UI Symbol", "Segoe UI" }
                : new[] { "Segoe UI Symbol", "Segoe UI", "Segoe UI Emoji" };
            foreach (string fallbackName in fallbackNames)
            {
                FontFamily fallbackFamily = GetUnicodeFallbackFamily(fallbackName);
                if (fallbackFamily == null)
                    continue;

                Font fallbackFont = GetFontForFamily(fallbackFamily);
                if (FontSupportsTextElement(graphics, fallbackFont, textElement))
                {
                    CacheUnicodeFontChoice(textElement, fallbackFamily);
                    return fallbackFamily;
                }
            }

            CacheUnicodeFontChoice(textElement, selectedFamily);
            return selectedFamily;
        }

        private void CacheUnicodeFontChoice(string textElement, FontFamily family)
        {
            if (_unicodeFontChoiceCache.Count >= 512)
                _unicodeFontChoiceCache.Clear();
            _unicodeFontChoiceCache[_fontContextKey + "|" + textElement] = family;
        }

        private bool FontSupportsTextElement(Graphics graphics, Font font, string textElement)
        {
            bool foundVisibleCodePoint = false;
            for (int index = 0; index < textElement.Length;)
            {
                int codePoint = GetCodePoint(textElement, index, out int codePointLength);
                if (codePoint != 0x200D && !IsVariationSelector(codePoint))
                {
                    foundVisibleCodePoint = true;
                    if (GetFontGlyphIndex(graphics, font, codePoint) == 0)
                        return false;
                }
                index += codePointLength;
            }

            return foundVisibleCodePoint;
        }

        private int GetFontGlyphIndex(Graphics graphics, Font font, int codePoint)
        {
            byte[] cmap = GetFontCmap(graphics, font);
            if (cmap == null)
                return 0;

            return GetGlyphIndexFromCmap(cmap, codePoint);
        }

        private byte[] GetFontCmap(Graphics graphics, Font font)
        {
            string cacheKey = font.Name + "|" + (int)font.Style;
            if (_unicodeFontCmapCache.TryGetValue(cacheKey, out byte[] cachedCmap))
                return cachedCmap;

            IntPtr fontHandle = IntPtr.Zero;
            IntPtr deviceContext = IntPtr.Zero;
            IntPtr previousObject = IntPtr.Zero;
            byte[] cmap = null;
            try
            {
                fontHandle = font.ToHfont();
                deviceContext = graphics.GetHdc();
                previousObject = SelectObject(deviceContext, fontHandle);
                if (previousObject != IntPtr.Zero && previousObject != new IntPtr(-1))
                {
                    uint cmapTag = 0x70616D63; // GetFontData uses the little-endian FOURCC for 'cmap'.
                    uint dataSize = GetFontData(deviceContext, cmapTag, 0, null, 0);
                    if (dataSize != uint.MaxValue && dataSize >= 4 && dataSize <= int.MaxValue)
                    {
                        cmap = new byte[(int)dataSize];
                        uint bytesRead = GetFontData(deviceContext, cmapTag, 0, cmap, dataSize);
                        if (bytesRead != dataSize)
                            cmap = null;
                    }
                }
            }
            catch (Exception ex) when (ex is ExternalException || ex is ArgumentException || ex is InvalidOperationException)
            {
                cmap = null;
            }
            finally
            {
                if (deviceContext != IntPtr.Zero)
                {
                    if (previousObject != IntPtr.Zero && previousObject != new IntPtr(-1))
                        SelectObject(deviceContext, previousObject);
                    graphics.ReleaseHdc(deviceContext);
                }
                if (fontHandle != IntPtr.Zero)
                    DeleteObject(fontHandle);
            }

            _unicodeFontCmapCache[cacheKey] = cmap;
            return cmap;
        }

        private static int GetGlyphIndexFromCmap(byte[] cmap, int codePoint)
        {
            if (cmap == null || cmap.Length < 4)
                return 0;

            int tableCount = ReadUInt16(cmap, 2);
            int format12Offset = -1;
            int format4Offset = -1;
            for (int index = 0; index < tableCount; index++)
            {
                int recordOffset = 4 + index * 8;
                if (recordOffset + 8 > cmap.Length)
                    break;

                int platformId = ReadUInt16(cmap, recordOffset);
                int encodingId = ReadUInt16(cmap, recordOffset + 2);
                uint subtableOffset = ReadUInt32(cmap, recordOffset + 4);
                if (subtableOffset > int.MaxValue || subtableOffset + 2 > cmap.Length)
                    continue;

                int offset = (int)subtableOffset;
                int format = ReadUInt16(cmap, offset);
                if (format == 12 && (platformId == 0 || (platformId == 3 && encodingId == 10)))
                    format12Offset = offset;
                else if (format == 4 && (platformId == 0 || (platformId == 3 && encodingId == 1)))
                    format4Offset = offset;
            }

            int glyphIndex = GetFormat12GlyphIndex(cmap, format12Offset, codePoint);
            if (glyphIndex != 0)
                return glyphIndex;
            return GetFormat4GlyphIndex(cmap, format4Offset, codePoint);
        }

        private static int GetFormat12GlyphIndex(byte[] cmap, int offset, int codePoint)
        {
            if (offset < 0 || offset + 16 > cmap.Length)
                return 0;

            uint groupCount = ReadUInt32(cmap, offset + 12);
            for (uint index = 0; index < groupCount; index++)
            {
                long groupOffsetLong = (long)offset + 16L + (long)index * 12L;
                if (groupOffsetLong + 12 > cmap.Length)
                    return 0;

                int groupOffset = (int)groupOffsetLong;
                uint startCode = ReadUInt32(cmap, groupOffset);
                uint endCode = ReadUInt32(cmap, groupOffset + 4);
                uint startGlyph = ReadUInt32(cmap, groupOffset + 8);
                if ((uint)codePoint >= startCode && (uint)codePoint <= endCode)
                    return (int)(startGlyph + (uint)codePoint - startCode);
            }

            return 0;
        }

        private static int GetFormat4GlyphIndex(byte[] cmap, int offset, int codePoint)
        {
            if (offset < 0 || codePoint > 0xFFFF || offset + 16 > cmap.Length)
                return 0;

            int segmentCount = ReadUInt16(cmap, offset + 6) / 2;
            int endCodesOffset = offset + 14;
            int startCodesOffset = endCodesOffset + segmentCount * 2 + 2;
            int deltasOffset = startCodesOffset + segmentCount * 2;
            int rangeOffsetsOffset = deltasOffset + segmentCount * 2;
            if (segmentCount <= 0 || rangeOffsetsOffset + segmentCount * 2 > cmap.Length)
                return 0;

            for (int index = 0; index < segmentCount; index++)
            {
                int endCode = ReadUInt16(cmap, endCodesOffset + index * 2);
                int startCode = ReadUInt16(cmap, startCodesOffset + index * 2);
                if (codePoint < startCode || codePoint > endCode)
                    continue;

                int delta = unchecked((short)ReadUInt16(cmap, deltasOffset + index * 2));
                int rangeOffsetAddress = rangeOffsetsOffset + index * 2;
                int rangeOffset = ReadUInt16(cmap, rangeOffsetAddress);
                if (rangeOffset == 0)
                    return (codePoint + delta) & 0xFFFF;

                int glyphAddress = rangeOffsetAddress + rangeOffset + (codePoint - startCode) * 2;
                if (glyphAddress + 2 > cmap.Length)
                    return 0;

                int glyphIndex = ReadUInt16(cmap, glyphAddress);
                return glyphIndex == 0 ? 0 : (glyphIndex + delta) & 0xFFFF;
            }

            return 0;
        }

        private static int ReadUInt16(byte[] data, int offset)
        {
            return (data[offset] << 8) | data[offset + 1];
        }

        private static uint ReadUInt32(byte[] data, int offset)
        {
            return ((uint)data[offset] << 24) |
                   ((uint)data[offset + 1] << 16) |
                   ((uint)data[offset + 2] << 8) |
                   data[offset + 3];
        }

        private FontFamily GetUnicodeFallbackFamily(string familyName)
        {
            if (_unicodeFallbackFamilies.TryGetValue(familyName, out FontFamily cachedFamily))
                return cachedFamily;

            FontFamily family = null;
            try
            {
                family = new FontFamily(familyName);
            }
            catch (ArgumentException)
            {
                // The fallback family is not installed on this Windows version.
            }

            _unicodeFallbackFamilies[familyName] = family;
            return family;
        }

        private Font GetFontForFamily(FontFamily family)
        {
            if (SameFontFamily(family, _displayFont.FontFamily))
                return _displayFont;

            if (_unicodeFallbackFonts.TryGetValue(_fontContextKey + "|" + family.Name, out Font cachedFont))
                return cachedFont;

            FontStyle style = _displayFont.Style;
            if (!family.IsStyleAvailable(style))
                style = FontStyle.Regular;

            var font = new Font(family, _displayFont.Size, style, _displayFont.Unit);
            _unicodeFallbackFonts[_fontContextKey + "|" + family.Name] = font;
            return font;
        }

        private void DisposeUnicodeFallbackFonts()
        {
            foreach (Font font in _unicodeFallbackFonts.Values)
                font.Dispose();
            _unicodeFallbackFonts.Clear();
        }

        private static float GetFontBaseline(FontFamily family, FontStyle style, float emSize)
        {
            int emHeight = family.GetEmHeight(style);
            return emHeight <= 0 ? emSize * 0.8f : emSize * family.GetCellAscent(style) / emHeight;
        }

        private static bool SameFontFamily(FontFamily left, FontFamily right)
        {
            return string.Equals(left?.Name, right?.Name, StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsAscii(string text)
        {
            if (string.IsNullOrEmpty(text))
                return true;

            for (int index = 0; index < text.Length; index++)
            {
                if (text[index] > 0x7F)
                    return false;
            }

            return true;
        }

        private static List<string> GetUnicodeTextElements(string text)
        {
            var elements = new List<string>();
            if (string.IsNullOrEmpty(text))
                return elements;

            var currentElement = new StringBuilder();
            bool previousWasJoiner = false;
            int regionalIndicatorCount = 0;
            for (int index = 0; index < text.Length;)
            {
                int codePoint = GetCodePoint(text, index, out int codePointLength);
                UnicodeCategory category = CharUnicodeInfo.GetUnicodeCategory(text, index);
                bool combining = category == UnicodeCategory.NonSpacingMark ||
                    category == UnicodeCategory.SpacingCombiningMark ||
                    category == UnicodeCategory.EnclosingMark;
                bool regionalIndicator = codePoint >= 0x1F1E6 && codePoint <= 0x1F1FF;
                bool attach = currentElement.Length == 0 || combining || IsVariationSelector(codePoint) ||
                    (codePoint >= 0x1F3FB && codePoint <= 0x1F3FF) || codePoint == 0x200D ||
                    previousWasJoiner || (regionalIndicator && regionalIndicatorCount == 1);

                if (!attach)
                {
                    elements.Add(currentElement.ToString());
                    currentElement.Clear();
                    regionalIndicatorCount = 0;
                }

                currentElement.Append(text, index, codePointLength);
                previousWasJoiner = codePoint == 0x200D;
                if (regionalIndicator)
                    regionalIndicatorCount++;
                else if (!combining && !IsVariationSelector(codePoint) && codePoint != 0x200D)
                    regionalIndicatorCount = 0;
                index += codePointLength;
            }

            if (currentElement.Length > 0)
                elements.Add(currentElement.ToString());
            return elements;
        }

        private static int GetCodePoint(string text, int index, out int length)
        {
            char first = text[index];
            if (char.IsHighSurrogate(first) && index + 1 < text.Length && char.IsLowSurrogate(text[index + 1]))
            {
                length = 2;
                return char.ConvertToUtf32(first, text[index + 1]);
            }

            length = 1;
            return first;
        }

        private static bool IsVariationSelector(int codePoint)
        {
            return (codePoint >= 0xFE00 && codePoint <= 0xFE0F) ||
                   (codePoint >= 0xE0100 && codePoint <= 0xE01EF);
        }

        private float GetStrokeWidth()
        {
            return Math.Max(0.85f, DeviceDpi / 96f);
        }

        private void PresentLayeredWindow()
        {
            if (IsDisposed || !IsHandleCreated || ClientSize.Width <= 0 || ClientSize.Height <= 0)
                return;

            EnsurePresentationSurface();
            Bitmap bitmap = _presentationBitmap;
            Graphics graphics = _presentationGraphics;
            if (_metrics.Any(metric => metric.Key == "FrameTimeGraph"))
            {
                if (_staticPresentationBitmap == null)
                {
                    _staticPresentationBitmap = new Bitmap(bitmap.Width, bitmap.Height, PixelFormat.Format32bppPArgb);
                    _staticPresentationBitmap.SetResolution(DeviceDpi, DeviceDpi);
                    _staticPresentationGraphics = Graphics.FromImage(_staticPresentationBitmap);
                }
                if (_staticPresentationDirty) RebuildStaticPresentation();
                // Copy alpha as well as color so a moving graph cannot leave trails
                // on translucent or fully transparent backgrounds.
                var previousCompositingMode = graphics.CompositingMode;
                try
                {
                    graphics.CompositingMode = CompositingMode.SourceCopy;
                    graphics.DrawImageUnscaled(_staticPresentationBitmap, 0, 0);
                }
                finally { graphics.CompositingMode = previousCompositingMode; }
                DrawLayers(graphics, drawGraphs: true, drawText: false);
            }
            else
            {
                DisposeStaticPresentationSurface();
                DrawBackground(graphics, bitmap.Size);
                DrawOverlay(graphics);
            }

            IntPtr screenDc = GetDC(IntPtr.Zero);
            if (screenDc == IntPtr.Zero)
                return;

            IntPtr memoryDc = CreateCompatibleDC(screenDc);
            if (memoryDc == IntPtr.Zero)
            {
                ReleaseDC(IntPtr.Zero, screenDc);
                return;
            }

            IntPtr bitmapHandle = IntPtr.Zero;
            IntPtr previousObject = IntPtr.Zero;

            try
            {
                bitmapHandle = bitmap.GetHbitmap(Color.FromArgb(0));
                previousObject = SelectObject(memoryDc, bitmapHandle);
                if (previousObject == IntPtr.Zero || previousObject == new IntPtr(-1))
                    return;

                var destination = new NativePoint(Left, Top);
                var source = new NativePoint(0, 0);
                var size = new NativeSize(bitmap.Width, bitmap.Height);
                var blend = new BlendFunction
                {
                    BlendOp = AcSrcOver,
                    SourceConstantAlpha = (byte)Math.Round(
                        255d * Math.Max(20, Math.Min(100, _configuration.OpacityPercent)) / 100d),
                    AlphaFormat = AcSrcAlpha
                };

                UpdateLayeredWindow(
                    Handle,
                    screenDc,
                    ref destination,
                    ref size,
                    memoryDc,
                    ref source,
                    0,
                    ref blend,
                    UlwAlpha);
                EnsureTopMost();
            }
            finally
            {
                if (previousObject != IntPtr.Zero && previousObject != new IntPtr(-1))
                    SelectObject(memoryDc, previousObject);
                if (bitmapHandle != IntPtr.Zero)
                    DeleteObject(bitmapHandle);
                if (memoryDc != IntPtr.Zero)
                    DeleteDC(memoryDc);
                if (screenDc != IntPtr.Zero)
                    ReleaseDC(IntPtr.Zero, screenDc);
            }
        }

        private void RebuildStaticPresentation()
        {
            var graphics = _staticPresentationGraphics;
            DrawBackground(graphics, _staticPresentationBitmap.Size);
            DrawLayers(graphics, drawGraphs: false, drawText: true);
            _staticPresentationDirty = false;
        }

        private void DrawBackground(Graphics graphics, Size size)
        {
            graphics.Clear(Color.Transparent);
            int backgroundOpacity = ValueHelper.ClampInt(_configuration.BackgroundOpacityPercent, 0, 100);
            if (backgroundOpacity > 0)
            {
                using (var brush = new SolidBrush(Color.FromArgb((int)Math.Round(255d * backgroundOpacity / 100d), _backgroundColor)))
                {
                    EnsureBackgroundPath(size);
                    if (_backgroundPath == null)
                        graphics.FillRectangle(brush, 0, 0, size.Width, size.Height);
                    else
                    {
                        var previousSmoothingMode = graphics.SmoothingMode;
                        try { graphics.SmoothingMode = SmoothingMode.AntiAlias; graphics.FillPath(brush, _backgroundPath); }
                        finally { graphics.SmoothingMode = previousSmoothingMode; }
                    }
                }
            }
        }

        private void EnsurePresentationSurface()
        {
            Size size = ClientSize;
            if (_presentationBitmap != null && _presentationSize == size &&
                _presentationDpi == DeviceDpi)
                return;

            DisposePresentationSurface();
            _presentationBitmap = new Bitmap(size.Width, size.Height, PixelFormat.Format32bppPArgb);
            _presentationBitmap.SetResolution(DeviceDpi, DeviceDpi);
            _presentationGraphics = Graphics.FromImage(_presentationBitmap);
            _presentationSize = size;
            _presentationDpi = DeviceDpi;
        }

        private void EnsureBackgroundPath(Size size)
        {
            int requestedRadius = ScaleLogical(ValueHelper.ClampInt(_configuration.BackgroundCornerRoundness, 0, 50));
            int radius = Math.Min(requestedRadius, Math.Min(size.Width, size.Height) / 2);
            if (_backgroundPathSize == size && _backgroundPathRadius == radius)
                return;

            _backgroundPath?.Dispose();
            _backgroundPath = null;
            _backgroundPathSize = size;
            _backgroundPathRadius = radius;
            if (radius <= 0)
                return;

            int diameter = radius * 2;
            var path = new GraphicsPath();
            path.AddArc(0, 0, diameter, diameter, 180, 90);
            path.AddArc(size.Width - diameter, 0, diameter, diameter, 270, 90);
            path.AddArc(size.Width - diameter, size.Height - diameter, diameter, diameter, 0, 90);
            path.AddArc(0, size.Height - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();
            _backgroundPath = path;
        }

        private void EnsureTopMost()
        {
            if (IsDisposed || !IsHandleCreated)
                return;

            SetWindowPos(
                Handle,
                HwndTopMost,
                0,
                0,
                0,
                0,
                SwpNoSize | SwpNoMove | SwpNoActivate | SwpNoOwnerZOrder);
        }

        private void ApplyConfiguredPosition()
        {
            Rectangle area = _workingArea.Width > 0 && _workingArea.Height > 0
                ? _workingArea
                : Screen.PrimaryScreen.WorkingArea;
            int externalSpacing = ScaleLogical(ValueHelper.ClampInt(_configuration.ExternalSpacing, 0, 100));
            int left = area.Left;
            int top = area.Top;

            switch (_configuration.Position)
            {
                case OsdPosition.TopCenter:
                case OsdPosition.Center:
                case OsdPosition.BottomCenter:
                    left = area.Left + (area.Width - Width) / 2;
                    break;
                case OsdPosition.TopRight:
                case OsdPosition.CenterRight:
                case OsdPosition.BottomRight:
                    left = area.Right - Width - externalSpacing;
                    break;
                default:
                    left = area.Left + externalSpacing;
                    break;
            }

            switch (_configuration.Position)
            {
                case OsdPosition.CenterLeft:
                case OsdPosition.Center:
                case OsdPosition.CenterRight:
                    top = area.Top + (area.Height - Height) / 2;
                    break;
                case OsdPosition.BottomLeft:
                case OsdPosition.BottomCenter:
                case OsdPosition.BottomRight:
                    top = area.Bottom - Height - externalSpacing;
                    break;
                default:
                    top = area.Top + externalSpacing;
                    break;
            }

            left = Math.Max(area.Left, Math.Min(left, area.Right - Width));
            top = Math.Max(area.Top, Math.Min(top, area.Bottom - Height));
            Location = new Point(left, top);
        }

        private int GetConfiguredPadding()
        {
            return ScaleLogical(Math.Max(0, _configuration.ScreenMargin));
        }

        private int GetRowsSpacing()
        {
            return ScaleLogical(Math.Max(0, _configuration.RowsSpacing));
        }

        private int GetFrameTimeGraphHeight()
        {
            return ScaleLogical(ValueHelper.ClampInt(_configuration.FrameTimeGraphHeight,
                OsdConfiguration.MinFrameTimeGraphHeight, OsdConfiguration.MaxFrameTimeGraphHeight));
        }

        private int GetColumnsSpacing()
        {
            return ScaleLogical(Math.Max(0, _configuration.ColumnsSpacing));
        }

        private int ScaleLogical(int value)
        {
            return (int)Math.Round(value * DeviceDpi / 96f);
        }

        private struct UnicodeTextRun
        {
            internal UnicodeTextRun(
                string text,
                FontFamily family,
                Font font,
                float advance,
                float baselineOffset)
            {
                Text = text;
                Family = family;
                Font = font;
                Advance = advance;
                BaselineOffset = baselineOffset;
            }

            internal string Text { get; }
            internal FontFamily Family { get; }
            internal Font Font { get; }
            internal float Advance { get; }
            internal float BaselineOffset { get; }
        }

        private sealed class RowLayout
        {
            internal readonly List<TextPlacement> Parts = new List<TextPlacement>();
            internal int Width, Height, X, Y;
            internal float Ascent, Scale;
            internal bool IsGroup, IsGraph;
        }
        private sealed class TextPlacement
        {
            internal TextPlacement(string text, OsdMetricStyle style, bool label, int x, int width, int member = -1)
            { Text = text; Style = style; Label = label; X = x; Width = width; Member = member; }
            internal readonly string Text;
            internal readonly OsdMetricStyle Style;
            internal readonly bool Label;
            internal int X;
            internal readonly int Width, Member;
            internal int Owner = -1;
            internal bool IsGroupCaption;
        }
        [StructLayout(LayoutKind.Sequential)]
        private struct NativePoint
        {
            internal NativePoint(int x, int y)
            {
                X = x;
                Y = y;
            }

            internal int X;
            internal int Y;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct NativeSize
        {
            internal NativeSize(int width, int height)
            {
                Width = width;
                Height = height;
            }

            internal int Width;
            internal int Height;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct BlendFunction
        {
            internal byte BlendOp;
            internal byte BlendFlags;
            internal byte SourceConstantAlpha;
            internal byte AlphaFormat;
        }

        [DllImport("dwmapi.dll", ExactSpelling = true)]
        private static extern int DwmSetWindowAttribute(
            IntPtr windowHandle, int attribute, ref int value, int valueSize);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool UpdateLayeredWindow(
            IntPtr windowHandle,
            IntPtr destinationDc,
            ref NativePoint destination,
            ref NativeSize size,
            IntPtr sourceDc,
            ref NativePoint source,
            int colorKey,
            ref BlendFunction blend,
            int flags);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool SetWindowPos(
            IntPtr windowHandle,
            IntPtr windowHandleInsertAfter,
            int x,
            int y,
            int width,
            int height,
            int flags);

        [DllImport("user32.dll")]
        private static extern IntPtr GetDC(IntPtr windowHandle);

        [DllImport("user32.dll")]
        private static extern int ReleaseDC(IntPtr windowHandle, IntPtr deviceContext);

        [DllImport("gdi32.dll", SetLastError = true)]
        private static extern IntPtr CreateCompatibleDC(IntPtr deviceContext);

        [DllImport("gdi32.dll", SetLastError = true)]
        private static extern bool DeleteDC(IntPtr deviceContext);

        [DllImport("gdi32.dll", SetLastError = true)]
        private static extern IntPtr SelectObject(IntPtr deviceContext, IntPtr graphicsObject);

        [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
        private static extern uint GetFontData(
            IntPtr deviceContext,
            uint table,
            uint offset,
            [Out] byte[] data,
            uint dataSize);

        [DllImport("gdi32.dll", SetLastError = true)]
        private static extern bool DeleteObject(IntPtr graphicsObject);
    }
}
