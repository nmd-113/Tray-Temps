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
        private readonly List<MetricLayout> _metricLayouts = new List<MetricLayout>();
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
        private GraphicsPath _backgroundPath;
        private Size _presentationSize;
        private Size _backgroundPathSize;
        private int _presentationDpi;
        private int _backgroundPathRadius = -1;
        private Font _displayFont;
        private string _displayFontFamily;
        private float _displayFontSize;
        private OsdConfiguration _configuration = new OsdConfiguration();
        private Rectangle _workingArea;
        private Color _textColor = Color.WhiteSmoke;
        private Color _backgroundColor = Color.FromArgb(24, 24, 24);
        private int _trailingValueSlotWidth;
        private int _secondTrailingValueSlotWidth;
        private int _stablePercentSlotWidth;
        private int _stablePercentSlotDpi;
        private int _layoutDpi;

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
            DisposePresentationSurface();
            DisposeDisplayFont();
            DisposeUnicodeFallbackFonts();
            _unicodeTextRunCache.Clear();
            _unicodeFontChoiceCache.Clear();
            _unicodeFontCmapCache.Clear();
            foreach (FontFamily family in _unicodeFallbackFamilies.Values)
                family.Dispose();
            _unicodeFallbackFamilies.Clear();
            base.OnFormClosed(e);
        }

        private void DisposePresentationSurface()
        {
            _presentationGraphics?.Dispose();
            _presentationGraphics = null;
            _presentationBitmap?.Dispose();
            _presentationBitmap = null;
            _backgroundPath?.Dispose();
            _backgroundPath = null;
            _presentationSize = Size.Empty;
            _backgroundPathSize = Size.Empty;
            _backgroundPathRadius = -1;
            _presentationDpi = 0;
        }

        private void DisposeDisplayFont()
        {
            _displayFont?.Dispose();
            _displayFont = null;
            _displayFontFamily = null;
            _displayFontSize = 0f;
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
                    out bool layoutChanged))
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
            if (layoutChanged)
                RebuildLayout();
            if (positionChanged)
                ApplyConfiguredPosition();
            PresentLayeredWindow();
        }

        private bool HasDisplayChanged(
            OsdConfiguration configuration,
            IReadOnlyList<OsdMetric> metrics,
            Rectangle workingArea,
            out bool layoutChanged)
        {
            layoutChanged = _displayFont == null || _layoutDpi != DeviceDpi ||
                _metricLayouts.Count != metrics.Count ||
                _workingArea.Size != workingArea.Size ||
                !string.Equals(
                    _configuration.FontFamily,
                    configuration.FontFamily,
                    StringComparison.OrdinalIgnoreCase) ||
                Math.Abs(_configuration.FontSize - configuration.FontSize) > 0.01f ||
                _configuration.Columns != configuration.Columns ||
                _configuration.ScreenMargin != configuration.ScreenMargin ||
                _configuration.RowsSpacing != configuration.RowsSpacing ||
                _configuration.ColumnsSpacing != configuration.ColumnsSpacing ||
                _configuration.LabelValueSpacing != configuration.LabelValueSpacing;

            bool displayChanged = layoutChanged ||
                _workingArea != workingArea ||
                _configuration.Position != configuration.Position ||
                _configuration.BackgroundColor != configuration.BackgroundColor ||
                _configuration.BackgroundCornerRoundness != configuration.BackgroundCornerRoundness ||
                _configuration.ExternalSpacing != configuration.ExternalSpacing ||
                _configuration.BackgroundOpacityPercent != configuration.BackgroundOpacityPercent ||
                _configuration.OpacityPercent != configuration.OpacityPercent;

            int comparableMetricCount = Math.Min(_metrics.Count, metrics.Count);
            for (int index = 0; index < comparableMetricCount; index++)
            {
                OsdMetric current = _metrics[index];
                OsdMetric next = metrics[index];
                bool valueChanged =
                    !string.Equals(current.Value, next.Value, StringComparison.Ordinal);
                bool metricLayoutChanged =
                    !string.Equals(current.Label, next.Label, StringComparison.Ordinal) ||
                    !string.Equals(current.ValueWidthTemplate, next.ValueWidthTemplate, StringComparison.Ordinal) ||
                    !string.Equals(current.AlternateValueWidthTemplate, next.AlternateValueWidthTemplate, StringComparison.Ordinal) ||
                    !string.Equals(current.TrailingValueWidthTemplate, next.TrailingValueWidthTemplate, StringComparison.Ordinal) ||
                    !string.Equals(current.SecondTrailingValueWidthTemplate, next.SecondTrailingValueWidthTemplate, StringComparison.Ordinal) ||
                    current.TrailingValueColor != next.TrailingValueColor ||
                    current.SecondTrailingValueColor != next.SecondTrailingValueColor ||
                    current.LabelValueSpacingOverride != next.LabelValueSpacingOverride ||
                    current.ValueFollowsLabel != next.ValueFollowsLabel ||
                    (current.TrailingValue == null) != (next.TrailingValue == null) ||
                    (current.SecondTrailingValue == null) != (next.SecondTrailingValue == null) ||
                    (string.IsNullOrEmpty(next.ValueWidthTemplate) && valueChanged);
                if (metricLayoutChanged)
                    layoutChanged = true;
                if (metricLayoutChanged ||
                    valueChanged ||
                    !string.Equals(current.TrailingValue, next.TrailingValue, StringComparison.Ordinal) ||
                    !string.Equals(current.SecondTrailingValue, next.SecondTrailingValue, StringComparison.Ordinal) ||
                    !string.Equals(current.TrailingValueWidthTemplate, next.TrailingValueWidthTemplate, StringComparison.Ordinal) ||
                    current.TextColor != next.TextColor)
                    displayChanged = true;
            }

            return displayChanged;
        }

        private void ApplyVisualSettings()
        {
            _backgroundColor = _configuration.BackgroundColor.HasValue
                ? OsdColorHelper.GetOpaqueColor(_configuration.BackgroundColor.Value)
                : OsdColorHelper.GetDefaultBackground(lightMode: false);
            _textColor = Color.WhiteSmoke;

            string fontFamily = string.IsNullOrWhiteSpace(_configuration.FontFamily)
                ? OsdFontHelper.DefaultFamily
                : _configuration.FontFamily;
            float fontSize = Math.Max(6f, Math.Min(48f, _configuration.FontSize));
            if (_displayFont == null ||
                !string.Equals(_displayFontFamily, fontFamily, StringComparison.OrdinalIgnoreCase) ||
                Math.Abs(_displayFontSize - fontSize) > 0.01f)
            {
                Font nextFont = OsdFontHelper.CreateFont(fontFamily, fontSize);
                _displayFont?.Dispose();
                _displayFont = nextFont;
                _displayFontFamily = fontFamily;
                _displayFontSize = fontSize;
                DisposeUnicodeFallbackFonts();
                _unicodeFontChoiceCache.Clear();
                _unicodeFontCmapCache.Clear();
                _unicodeTextRunCache.Clear();
                _trailingValueSlotWidth = 0;
                _secondTrailingValueSlotWidth = 0;
                _stablePercentSlotWidth = 0;
                _stablePercentSlotDpi = 0;
                _valueTemplateWidthCache.Clear();
            }
        }

        private void RebuildLayout()
        {
            if (_displayFont == null)
                ApplyVisualSettings();

            _metricLayouts.Clear();
            int itemCount = Math.Max(1, _metrics.Count);
            int columns = Math.Max(1, Math.Min(8, Math.Min(_configuration.Columns, itemCount)));
            int rows = (int)Math.Ceiling(itemCount / (double)columns);
            int padding = GetConfiguredPadding();
            int rowGap = GetRowsSpacing();
            int columnGap = GetColumnsSpacing();
            int rowSpacing = rows > 1 ? rowGap * (rows - 1) : 0;
            int columnSpacing = columns > 1 ? columnGap * (columns - 1) : 0;
            int combinedValueGap = GetCombinedValueGap();
            int labelGap = ScaleLogical(Math.Max(0, _configuration.LabelValueSpacing));
            int[] columnContentWidths = new int[columns];
            int[] rowHeights = new int[rows];
            int[] valueBlockWidths = new int[_metrics.Count];
            int[] labelWidths = new int[_metrics.Count];

            using (var measurementBitmap = new Bitmap(1, 1, PixelFormat.Format32bppPArgb))
            {
                measurementBitmap.SetResolution(DeviceDpi, DeviceDpi);
                using (Graphics graphics = Graphics.FromImage(measurementBitmap))
                {
                    if (_metrics.Any(metric => metric.TrailingValue != null))
                    {
                        bool percentValuesOnly = true;
                        foreach (OsdMetric metric in _metrics)
                        {
                            if (metric.TrailingValue == null)
                                continue;

                            percentValuesOnly &= metric.TrailingValueWidthTemplate == "###%";
                        }

                        if (percentValuesOnly)
                        {
                            if (_stablePercentSlotWidth <= 0 || _stablePercentSlotDpi != DeviceDpi)
                            {
                                _stablePercentSlotWidth = MeasureStablePercentSlotWidth(graphics);
                                _stablePercentSlotDpi = DeviceDpi;
                            }
                            _trailingValueSlotWidth = _stablePercentSlotWidth;
                        }
                        else
                        {
                            int trailingValueWidth = 0;
                            foreach (OsdMetric metric in _metrics)
                            {
                                if (metric.TrailingValue == null)
                                    continue;

                                trailingValueWidth = Math.Max(
                                    trailingValueWidth,
                                    string.IsNullOrEmpty(metric.TrailingValueWidthTemplate)
                                        ? MeasureDisplayTextWidth(graphics, metric.TrailingValue)
                                        : MeasureStableValueTemplateWidth(graphics, metric.TrailingValueWidthTemplate));
                            }
                            _trailingValueSlotWidth = trailingValueWidth;
                        }
                    }
                    if (_metrics.Any(metric => metric.SecondTrailingValue != null))
                    {
                        int secondTrailingValueWidth = 0;
                        foreach (OsdMetric metric in _metrics)
                        {
                            if (metric.SecondTrailingValue == null)
                                continue;

                            secondTrailingValueWidth = Math.Max(
                                secondTrailingValueWidth,
                                string.IsNullOrEmpty(metric.SecondTrailingValueWidthTemplate)
                                    ? MeasureDisplayTextWidth(graphics, metric.SecondTrailingValue)
                                    : MeasureStableValueTemplateWidth(graphics, metric.SecondTrailingValueWidthTemplate));
                        }
                        _secondTrailingValueSlotWidth = secondTrailingValueWidth;
                    }

                    MeasureLayoutMetrics(
                        graphics,
                        columns,
                        labelGap,
                        combinedValueGap,
                        columnContentWidths,
                        rowHeights,
                        valueBlockWidths,
                        labelWidths);
                }
            }

            int contentWidth = 0;
            for (int column = 0; column < columns; column++)
                contentWidth += columnContentWidths[column];

            int width = padding * 2 + contentWidth + columnSpacing;
            int contentHeight = rowHeights.Sum();
            int height = padding * 2 + contentHeight + rowSpacing;
            int maxWidth = _workingArea.Width > 0
                ? _workingArea.Width
                : width;
            int maxHeight = _workingArea.Height > 0
                ? _workingArea.Height
                : height;
            ClientSize = new Size(
                Math.Max(1, Math.Min(maxWidth, width)),
                Math.Max(1, Math.Min(maxHeight, height)));

            int availableContentWidth = Math.Max(
                1,
                ClientSize.Width - padding * 2 - columnSpacing);
            bool constrained = availableContentWidth < contentWidth;
            int[] columnWidths = new int[columns];
            int[] columnLefts = new int[columns];
            int columnLeft = padding;
            for (int column = 0; column < columns; column++)
            {
                columnLefts[column] = columnLeft;
                columnWidths[column] = constrained
                    ? availableContentWidth / columns + (column < availableContentWidth % columns ? 1 : 0)
                    : columnContentWidths[column];
                columnLeft += columnWidths[column];
                if (column < columns - 1)
                    columnLeft += columnGap;
            }

            int[] rowTops = new int[rows];
            int rowTop = padding;
            for (int row = 0; row < rows; row++)
            {
                rowTops[row] = rowTop;
                rowTop += rowHeights[row];
                if (row < rows - 1)
                    rowTop += rowGap;
            }

            for (int index = 0; index < _metrics.Count; index++)
            {
                int column = index % columns;
                int row = index / columns;
                OsdMetric metric = _metrics[index];
                int x = columnLefts[column];
                int y = rowTops[row];
                int valueBlockWidth = Math.Min(valueBlockWidths[index], columnWidths[column]);
                int metricLabelGap = GetMetricLabelGap(metric, labelGap);
                int rightAlignedValueX = x + Math.Max(0, columnWidths[column] - valueBlockWidth);
                int labelFollowingValueX = x + labelWidths[index] + metricLabelGap;
                int valueBlockX = metric.ValueFollowsLabel
                    ? Math.Min(labelFollowingValueX, rightAlignedValueX)
                    : rightAlignedValueX;
                int trailingValueWidth = metric.TrailingValue != null
                    ? Math.Min(_trailingValueSlotWidth, valueBlockWidth)
                    : 0;
                int secondTrailingValueWidth = metric.SecondTrailingValue != null
                    ? Math.Min(_secondTrailingValueSlotWidth, Math.Max(0, valueBlockWidth - trailingValueWidth))
                    : 0;
                int combinedGap = trailingValueWidth > 0
                    ? Math.Min(combinedValueGap, valueBlockWidth - trailingValueWidth - secondTrailingValueWidth)
                    : 0;
                int secondCombinedGap = secondTrailingValueWidth > 0
                    ? Math.Min(combinedValueGap, valueBlockWidth - trailingValueWidth - secondTrailingValueWidth - combinedGap)
                    : 0;
                int primaryValueRight = valueBlockX + valueBlockWidth;
                if (metric.SecondTrailingValue != null)
                    primaryValueRight -= secondCombinedGap + secondTrailingValueWidth;
                if (metric.TrailingValue != null)
                    primaryValueRight -= combinedGap + trailingValueWidth;

                _metricLayouts.Add(new MetricLayout(
                    new Rectangle(
                        x,
                        y,
                        Math.Max(0, valueBlockX - x - metricLabelGap),
                        rowHeights[row]),
                    new Rectangle(
                        valueBlockX,
                        y,
                        Math.Max(0, primaryValueRight - valueBlockX),
                        rowHeights[row]),
                    metric.TrailingValue != null
                        ? new Rectangle(
                            valueBlockX + valueBlockWidth - trailingValueWidth - secondTrailingValueWidth - secondCombinedGap,
                            y,
                            trailingValueWidth,
                            rowHeights[row])
                        : Rectangle.Empty,
                    metric.SecondTrailingValue != null
                        ? new Rectangle(valueBlockX + valueBlockWidth - secondTrailingValueWidth, y, secondTrailingValueWidth, rowHeights[row])
                        : Rectangle.Empty));
            }

            _layoutDpi = DeviceDpi;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            DrawOverlay(e.Graphics);
        }

        private void DrawOverlay(Graphics graphics)
        {
            if (_displayFont == null || _metrics.Count == 0 ||
                _metricLayouts.Count != _metrics.Count)
                return;

            GraphicsState graphicsState = graphics.Save();
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

            try
            {
                for (int index = 0; index < _metrics.Count; index++)
                {
                    OsdMetric metric = _metrics[index];
                    MetricLayout layout = _metricLayouts[index];
                    Color textColor = metric.TextColor ?? _textColor;

                    DrawDisplayText(
                        graphics,
                        metric.Label,
                        layout.LabelBounds,
                        textColor,
                        StringAlignment.Near);
                    DrawDisplayText(
                        graphics,
                        metric.Value,
                        layout.PrimaryValueBounds,
                        textColor,
                        StringAlignment.Far);
                    if (metric.TrailingValue != null)
                    {
                        DrawDisplayText(
                            graphics,
                            metric.TrailingValue,
                            layout.TrailingValueBounds,
                            metric.TrailingValueColor ?? textColor,
                            StringAlignment.Far);
                    }
                    if (metric.SecondTrailingValue != null)
                    {
                        DrawDisplayText(
                            graphics,
                            metric.SecondTrailingValue,
                            layout.SecondTrailingValueBounds,
                            metric.SecondTrailingValueColor ?? textColor,
                            StringAlignment.Far);
                    }
                }
            }
            finally
            {
                graphics.Restore(graphicsState);
            }
        }

        private void MeasureLayoutMetrics(
            Graphics graphics,
            int columns,
            int labelGap,
            int combinedValueGap,
            int[] columnContentWidths,
            int[] rowHeights,
            int[] valueBlockWidths,
            int[] labelWidths)
        {
            int sharedRowHeight = MeasureDisplayTextSize(
                graphics,
                "N/A 100% 999\u00B0C 999\u00B0F 999.99G 9999").Height;

            for (int index = 0; index < _metrics.Count; index++)
            {
                int column = index % columns;
                int row = index / columns;
                OsdMetric metric = _metrics[index];
                Size labelSize = MeasureDisplayTextSize(graphics, metric.Label);
                labelWidths[index] = labelSize.Width;
                Size valueSize = MeasureDisplayTextSize(graphics, metric.Value);
                int valueBlockWidth = GetPrimaryValueSlotWidth(
                    graphics,
                    metric,
                    valueSize.Width);
                int metricHeight = Math.Max(labelSize.Height, valueSize.Height);
                if (metric.TrailingValue != null)
                {
                    valueBlockWidth += combinedValueGap + _trailingValueSlotWidth;
                    metricHeight = Math.Max(
                        metricHeight,
                        MeasureDisplayTextSize(graphics, metric.TrailingValue).Height);
                }
                if (metric.SecondTrailingValue != null)
                {
                    valueBlockWidth += combinedValueGap + _secondTrailingValueSlotWidth;
                    metricHeight = Math.Max(metricHeight, MeasureDisplayTextSize(graphics, metric.SecondTrailingValue).Height);
                }

                valueBlockWidths[index] = valueBlockWidth;
                int rowWidth = labelSize.Width + GetMetricLabelGap(metric, labelGap) + valueBlockWidth;
                columnContentWidths[column] = Math.Max(columnContentWidths[column], rowWidth);
                rowHeights[row] = Math.Max(rowHeights[row], metricHeight);
                sharedRowHeight = Math.Max(sharedRowHeight, rowHeights[row]);
            }

            for (int row = 0; row < rowHeights.Length; row++)
                rowHeights[row] = sharedRowHeight;
        }

        private void DrawDisplayText(
            Graphics graphics,
            string text,
            Rectangle bounds,
            Color textColor,
            StringAlignment alignment)
        {
            if (bounds.Width <= 0 || bounds.Height <= 0 || string.IsNullOrEmpty(text))
                return;

            Color opaqueTextColor = Color.FromArgb(textColor.R, textColor.G, textColor.B);
            double luminance =
                (0.299 * opaqueTextColor.R) +
                (0.587 * opaqueTextColor.G) +
                (0.114 * opaqueTextColor.B);
            Color strokeColor = luminance >= 145
                ? Color.FromArgb(TextStrokeAlpha, 0, 0, 0)
                : Color.FromArgb(TextStrokeAlpha, 255, 255, 255);
            float strokeWidth = GetStrokeWidth();
            float strokeInset = strokeWidth / 2f;
            float availableWidth = Math.Max(1f, bounds.Width - strokeWidth);
            GraphicsPath path = CreateTextPath(graphics, text);
            try
            {
                RectangleF pathBounds = path.GetBounds();
                if (pathBounds.Width > availableWidth)
                {
                    path.Dispose();
                    path = null;

                    string displayText = TrimDisplayTextToWidth(graphics, text, availableWidth);
                    if (string.IsNullOrEmpty(displayText))
                        return;

                    path = CreateTextPath(graphics, displayText);
                    pathBounds = path.GetBounds();
                }

                using (var textBrush = new SolidBrush(opaqueTextColor))
                {
                    float x = alignment == StringAlignment.Far
                        ? bounds.Right - strokeInset - pathBounds.Width - pathBounds.X
                        : bounds.Left + strokeInset - pathBounds.X;
                    float y = bounds.Top + ((bounds.Height - pathBounds.Height) / 2f) - pathBounds.Y;

                    using (var transform = new Matrix())
                    {
                        transform.Translate(x, y);
                        path.Transform(transform);
                    }

                    if (strokeWidth > 0f)
                    {
                        using (var strokePen = new Pen(strokeColor, strokeWidth))
                        {
                            strokePen.LineJoin = LineJoin.Round;
                            graphics.DrawPath(strokePen, path);
                        }
                    }
                    graphics.FillPath(textBrush, path);
                }
            }
            finally
            {
                path?.Dispose();
            }
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

        private int MeasureStablePercentSlotWidth(Graphics graphics)
        {
            int width = MeasureDisplayTextWidth(graphics, "N/A");
            for (int value = 0; value <= 100; value++)
                width = Math.Max(width, MeasureDisplayTextWidth(graphics, value + "%"));

            return width;
        }

        private int GetPrimaryValueSlotWidth(
            Graphics graphics,
            OsdMetric metric,
            int measuredValueWidth)
        {
            if (string.IsNullOrEmpty(metric.ValueWidthTemplate))
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
            string cacheKey = DeviceDpi + "|" + template;
            if (_valueTemplateWidthCache.TryGetValue(cacheKey, out int cachedWidth))
                return cachedWidth;

            int width = 0;
            for (char digit = '0'; digit <= '9'; digit++)
                width = Math.Max(width, MeasureDisplayTextWidth(graphics, template.Replace('#', digit)));

            _valueTemplateWidthCache[cacheKey] = width;
            return width;
        }

        private string TrimDisplayTextToWidth(Graphics graphics, string text, float maximumWidth)
        {
            const string ellipsis = "…";
            if (MeasureTextPathWidth(graphics, ellipsis) > maximumWidth)
                return string.Empty;

            if (!IsAscii(text))
            {
                List<string> textElements = GetUnicodeTextElements(text);
                int lowElementCount = 0;
                int highElementCount = textElements.Count;
                while (lowElementCount < highElementCount)
                {
                    int elementCount = lowElementCount + ((highElementCount - lowElementCount + 1) / 2);
                    string candidate = string.Concat(textElements.Take(elementCount)).TrimEnd() + ellipsis;
                    if (MeasureTextPathWidth(graphics, candidate) <= maximumWidth)
                        lowElementCount = elementCount;
                    else
                        highElementCount = elementCount - 1;
                }

                return string.Concat(textElements.Take(lowElementCount)).TrimEnd() + ellipsis;
            }

            int low = 0;
            int high = text.Length;
            while (low < high)
            {
                int length = low + ((high - low + 1) / 2);
                string candidate = text.Substring(0, length).TrimEnd() + ellipsis;
                if (MeasureTextPathWidth(graphics, candidate) <= maximumWidth)
                    low = length;
                else
                    high = length - 1;
            }

            if (low > 0 && low < text.Length && char.IsHighSurrogate(text[low - 1]))
                low--;

            return text.Substring(0, low).TrimEnd() + ellipsis;
        }

        private float MeasureTextPathWidth(Graphics graphics, string text)
        {
            using (GraphicsPath path = CreateTextPath(graphics, text))
                return path.GetBounds().Width;
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
            string cacheKey = DeviceDpi.ToString(CultureInfo.InvariantCulture) + "\0" + text;
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

            if (_unicodeFontChoiceCache.TryGetValue(textElement, out FontFamily cachedFamily))
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
            _unicodeFontChoiceCache[textElement] = family;
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

            if (_unicodeFallbackFonts.TryGetValue(family.Name, out Font cachedFont))
                return cachedFont;

            FontStyle style = _displayFont.Style;
            if (!family.IsStyleAvailable(style))
                style = FontStyle.Regular;

            var font = new Font(family, _displayFont.Size, style, _displayFont.Unit);
            _unicodeFallbackFonts[family.Name] = font;
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

        private int GetCombinedValueGap()
        {
            // Keep a small DPI-scaled gap between the two stable value slots.
            return Math.Max(ScaleLogical(8), (int)Math.Ceiling(GetStrokeWidth()));
        }

        private int GetMetricLabelGap(OsdMetric metric, int defaultGap)
        {
            if (string.IsNullOrEmpty(metric.Label))
                return 0;

            return metric.LabelValueSpacingOverride.HasValue
                ? ScaleLogical(Math.Max(0, metric.LabelValueSpacingOverride.Value))
                : defaultGap;
        }

        private void PresentLayeredWindow()
        {
            if (IsDisposed || !IsHandleCreated || ClientSize.Width <= 0 || ClientSize.Height <= 0)
                return;

            EnsurePresentationSurface();
            Bitmap bitmap = _presentationBitmap;
            Graphics graphics = _presentationGraphics;
            graphics.Clear(Color.Transparent);

            int backgroundOpacity = Math.Max(
                0,
                Math.Min(100, _configuration.BackgroundOpacityPercent));
            if (backgroundOpacity > 0)
            {
                int alpha = (int)Math.Round(255d * backgroundOpacity / 100d);
                using (var backgroundBrush = new SolidBrush(Color.FromArgb(alpha, _backgroundColor)))
                {
                    EnsureBackgroundPath(bitmap.Size);
                    if (_backgroundPath == null)
                    {
                        graphics.FillRectangle(backgroundBrush, 0, 0, bitmap.Width, bitmap.Height);
                    }
                    else
                    {
                        SmoothingMode previousSmoothingMode = graphics.SmoothingMode;
                        graphics.SmoothingMode = SmoothingMode.AntiAlias;
                        try
                        {
                            graphics.FillPath(backgroundBrush, _backgroundPath);
                        }
                        finally
                        {
                            graphics.SmoothingMode = previousSmoothingMode;
                        }
                    }
                }
            }

            DrawOverlay(graphics);

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

        private struct MetricLayout
        {
            internal MetricLayout(
                Rectangle labelBounds,
                Rectangle primaryValueBounds,
                Rectangle trailingValueBounds,
                Rectangle secondTrailingValueBounds)
            {
                LabelBounds = labelBounds;
                PrimaryValueBounds = primaryValueBounds;
                TrailingValueBounds = trailingValueBounds;
                SecondTrailingValueBounds = secondTrailingValueBounds;
            }

            internal Rectangle LabelBounds { get; }
            internal Rectangle PrimaryValueBounds { get; }
            internal Rectangle TrailingValueBounds { get; }
            internal Rectangle SecondTrailingValueBounds { get; }
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
