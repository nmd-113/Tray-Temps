using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
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
        private const int UlwAlpha = 0x00000002;
        private const byte AcSrcOver = 0x00;
        private const byte AcSrcAlpha = 0x01;
        private const int TextStrokeAlpha = 220;
        private static readonly IntPtr HtTransparent = new IntPtr(-1);
        private readonly List<OsdMetric> _metrics = new List<OsdMetric>();
        private readonly Dictionary<string, int> _valueTemplateWidthCache =
            new Dictionary<string, int>(StringComparer.Ordinal);
        private Font _displayFont;
        private string _displayFontFamily;
        private float _displayFontSize;
        private OsdConfiguration _configuration = new OsdConfiguration();
        private Rectangle _workingArea;
        private Color _textColor = Color.WhiteSmoke;
        private Color _backgroundColor = Color.FromArgb(24, 24, 24);
        private int _trailingValueSlotWidth;
        private int _trailingValueSlotDpi;

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
                parameters.ExStyle |= WsExTransparent | WsExToolWindow | WsExLayered | WsExNoActivate;
                return parameters;
            }
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            // A hidden per-monitor-DPI form can receive its final DPI only when shown.
            // Recalculate once so the layered bitmap matches the text measured at that DPI.
            ResizeForContent();
            ApplyConfiguredPosition();
            PresentLayeredWindow();
        }

        protected override void WndProc(ref Message message)
        {
            if (message.Msg == WmNcHitTest)
            {
                message.Result = HtTransparent;
                return;
            }

            base.WndProc(ref message);
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            DisposeDisplayFont();
            base.OnFormClosed(e);
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
            _configuration = configuration?.Clone() ?? new OsdConfiguration();
            _workingArea = workingArea;
            _metrics.Clear();
            _metrics.AddRange(metrics ?? Enumerable.Empty<OsdMetric>());

            MoveToTargetMonitorForMeasurement();
            ApplyVisualSettings();
            ResizeForContent();
            ApplyConfiguredPosition();
            PresentLayeredWindow();
        }

        private void MoveToTargetMonitorForMeasurement()
        {
            if (_workingArea.Width <= 0 || _workingArea.Height <= 0)
                return;

            if (!IsHandleCreated || Screen.FromControl(this).WorkingArea != _workingArea)
                Location = _workingArea.Location;
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
            float fontSize = Math.Max(8f, Math.Min(48f, _configuration.FontSize));
            if (_displayFont == null ||
                !string.Equals(_displayFontFamily, fontFamily, StringComparison.OrdinalIgnoreCase) ||
                Math.Abs(_displayFontSize - fontSize) > 0.01f)
            {
                Font nextFont = OsdFontHelper.CreateFont(fontFamily, fontSize);
                _displayFont?.Dispose();
                _displayFont = nextFont;
                _displayFontFamily = fontFamily;
                _displayFontSize = fontSize;
                _trailingValueSlotWidth = 0;
                _valueTemplateWidthCache.Clear();
            }
        }

        private void ResizeForContent()
        {
            int itemCount = Math.Max(1, _metrics.Count);
            int columns = Math.Max(1, Math.Min(_configuration.Columns, itemCount));
            int rows = (int)Math.Ceiling(itemCount / (double)columns);
            int padding = GetConfiguredPadding();
            int rowGap = GetRowsSpacing();
            int columnGap = GetColumnsSpacing();
            int rowSpacing = rows > 1 ? rowGap * (rows - 1) : 0;
            int columnSpacing = columns > 1 ? columnGap * (columns - 1) : 0;
            int combinedValueGap = ScaleLogical(12);
            int[] labelSectionWidths = new int[columns];
            int[] valueBlockWidths = new int[columns];
            int[] rowHeights;

            using (var measurementBitmap = new Bitmap(1, 1, PixelFormat.Format32bppPArgb))
            {
                measurementBitmap.SetResolution(DeviceDpi, DeviceDpi);
                using (Graphics graphics = Graphics.FromImage(measurementBitmap))
                {
                    if (_metrics.Any(metric => metric.TrailingValue != null) &&
                        (_trailingValueSlotWidth <= 0 ||
                         _trailingValueSlotDpi != DeviceDpi))
                    {
                        _trailingValueSlotWidth = MeasureStablePercentSlotWidth(graphics);
                        _trailingValueSlotDpi = DeviceDpi;
                    }

                    MeasureColumnWidths(
                        graphics,
                        columns,
                        ScaleLogical(Math.Max(0, _configuration.LabelValueSpacing)),
                        combinedValueGap,
                        labelSectionWidths,
                        valueBlockWidths);
                    rowHeights = MeasureRowHeights(graphics, columns, rows);
                }
            }

            int contentWidth = 0;
            for (int column = 0; column < columns; column++)
                contentWidth += labelSectionWidths[column] + valueBlockWidths[column];

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
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            DrawOverlay(e.Graphics);
        }

        private void DrawOverlay(Graphics graphics)
        {
            if (_displayFont == null || _metrics.Count == 0)
                return;

            int columns = Math.Max(1, Math.Min(_configuration.Columns, _metrics.Count));
            int rows = (int)Math.Ceiling(_metrics.Count / (double)columns);
            int padding = GetConfiguredPadding();
            int rowGap = GetRowsSpacing();
            int columnGap = GetColumnsSpacing();
            int columnSpacing = columns > 1 ? columnGap * (columns - 1) : 0;
            int combinedValueGap = ScaleLogical(12);
            int labelGap = ScaleLogical(Math.Max(0, _configuration.LabelValueSpacing));
            int[] labelSectionWidths = new int[columns];
            int[] valueBlockWidths = new int[columns];
            int[] columnWidths = new int[columns];
            int[] columnLefts = new int[columns];
            int[] rowHeights;
            int[] rowTops = new int[rows];

            MeasureColumnWidths(
                graphics,
                columns,
                labelGap,
                combinedValueGap,
                labelSectionWidths,
                valueBlockWidths);
            rowHeights = MeasureRowHeights(graphics, columns, rows);
            int rowTop = padding;
            for (int row = 0; row < rows; row++)
            {
                rowTops[row] = rowTop;
                rowTop += rowHeights[row];
                if (row < rows - 1)
                    rowTop += rowGap;
            }

            int naturalContentWidth = 0;
            for (int column = 0; column < columns; column++)
                naturalContentWidth += labelSectionWidths[column] + valueBlockWidths[column];

            int availableContentWidth = Math.Max(
                1,
                ClientSize.Width - padding * 2 - columnSpacing);
            bool constrained = availableContentWidth < naturalContentWidth;
            int columnLeft = padding;
            for (int column = 0; column < columns; column++)
            {
                columnLefts[column] = columnLeft;
                columnWidths[column] = constrained
                    ? availableContentWidth / columns + (column < availableContentWidth % columns ? 1 : 0)
                    : labelSectionWidths[column] + valueBlockWidths[column];
                columnLeft += columnWidths[column];
                if (column < columns - 1)
                    columnLeft += columnGap;
            }

            GraphicsState graphicsState = graphics.Save();
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

            try
            {
                for (int index = 0; index < _metrics.Count; index++)
                {
                    int column = index % columns;
                    int row = index / columns;
                    OsdMetric metric = _metrics[index];
                    Color textColor = metric.TextColor ?? _textColor;
                    int x = columnLefts[column];
                    int y = rowTops[row];
                    int valueBlockWidth = Math.Min(valueBlockWidths[column], columnWidths[column]);
                    int valueBlockX = x + Math.Max(0, columnWidths[column] - valueBlockWidth);
                    int trailingValueWidth = metric.TrailingValue != null
                        ? Math.Min(_trailingValueSlotWidth, valueBlockWidth)
                        : 0;
                    int combinedGap = trailingValueWidth > 0
                        ? Math.Min(combinedValueGap, valueBlockWidth - trailingValueWidth)
                        : 0;
                    int primaryValueRight = valueBlockX + valueBlockWidth;
                    if (metric.TrailingValue != null)
                        primaryValueRight -= combinedGap + trailingValueWidth;

                    Rectangle labelBounds = new Rectangle(
                        x,
                        y,
                        Math.Max(0, valueBlockX - x - labelGap),
                        rowHeights[row]);
                    Rectangle primaryValueBounds = new Rectangle(
                        valueBlockX,
                        y,
                        Math.Max(0, primaryValueRight - valueBlockX),
                        rowHeights[row]);
                    Rectangle trailingValueBounds = metric.TrailingValue != null
                        ? new Rectangle(
                            valueBlockX + valueBlockWidth - trailingValueWidth,
                            y,
                            trailingValueWidth,
                            rowHeights[row])
                        : Rectangle.Empty;

                    DrawDisplayText(
                        graphics,
                        metric.Label,
                        labelBounds,
                        textColor,
                        StringAlignment.Near);
                    DrawDisplayText(
                        graphics,
                        metric.Value,
                        primaryValueBounds,
                        textColor,
                        StringAlignment.Far);
                    if (metric.TrailingValue != null)
                    {
                        DrawDisplayText(
                            graphics,
                            metric.TrailingValue,
                            trailingValueBounds,
                            textColor,
                            StringAlignment.Far);
                    }
                }
            }
            finally
            {
                graphics.Restore(graphicsState);
            }
        }

        private int GetValueBlockWidth(Graphics graphics, OsdMetric metric, int combinedValueGap)
        {
            int width = GetPrimaryValueSlotWidth(graphics, metric);
            if (metric.TrailingValue != null)
                width += combinedValueGap + _trailingValueSlotWidth;

            return width;
        }

        private void MeasureColumnWidths(
            Graphics graphics,
            int columns,
            int labelGap,
            int combinedValueGap,
            int[] labelSectionWidths,
            int[] valueBlockWidths)
        {
            for (int index = 0; index < _metrics.Count; index++)
            {
                int column = index % columns;
                OsdMetric metric = _metrics[index];
                labelSectionWidths[column] = Math.Max(
                    labelSectionWidths[column],
                    MeasureDisplayTextWidth(graphics, metric.Label) + labelGap);
                valueBlockWidths[column] = Math.Max(
                    valueBlockWidths[column],
                    GetValueBlockWidth(graphics, metric, combinedValueGap));
            }
        }

        private int[] MeasureRowHeights(Graphics graphics, int columns, int rows)
        {
            var rowHeights = new int[rows];
            int sharedRowHeight = 1;
            for (int index = 0; index < _metrics.Count; index++)
            {
                int row = index / columns;
                OsdMetric metric = _metrics[index];
                rowHeights[row] = Math.Max(rowHeights[row], MeasureOutlinedTextHeight(graphics, metric.Label));
                rowHeights[row] = Math.Max(rowHeights[row], MeasureOutlinedTextHeight(graphics, metric.Value));
                if (metric.TrailingValue != null)
                    rowHeights[row] = Math.Max(
                        rowHeights[row],
                        MeasureOutlinedTextHeight(graphics, metric.TrailingValue));
                sharedRowHeight = Math.Max(sharedRowHeight, rowHeights[row]);
            }

            for (int row = 0; row < rowHeights.Length - 1; row++)
                rowHeights[row] = sharedRowHeight;

            rowHeights[rowHeights.Length - 1] = Math.Max(1, rowHeights[rowHeights.Length - 1]);

            return rowHeights;
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
                if (path.GetBounds().Width > availableWidth)
                {
                    path.Dispose();
                    path = null;

                    string displayText = TrimDisplayTextToWidth(graphics, text, availableWidth);
                    if (string.IsNullOrEmpty(displayText))
                        return;

                    path = CreateTextPath(graphics, displayText);
                }

                using (var textBrush = new SolidBrush(opaqueTextColor))
                {
                    RectangleF pathBounds = path.GetBounds();
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

        private int MeasureOutlinedTextWidth(Graphics graphics, string text)
        {
            return Math.Max(
                1,
                (int)Math.Ceiling(MeasureTextPathWidth(graphics, text) + GetStrokeWidth()));
        }

        private int MeasureOutlinedTextHeight(Graphics graphics, string text)
        {
            using (GraphicsPath path = CreateTextPath(graphics, text))
                return Math.Max(1, (int)Math.Ceiling(path.GetBounds().Height + GetStrokeWidth()));
        }

        private int MeasureDisplayTextWidth(Graphics graphics, string text)
        {
            return MeasureOutlinedTextWidth(graphics, text);
        }

        private int MeasureStablePercentSlotWidth(Graphics graphics)
        {
            int width = MeasureDisplayTextWidth(graphics, "N/A");
            for (int value = 0; value <= 100; value++)
                width = Math.Max(width, MeasureDisplayTextWidth(graphics, value + "%"));

            return width;
        }

        private int GetPrimaryValueSlotWidth(Graphics graphics, OsdMetric metric)
        {
            int width = MeasureDisplayTextWidth(graphics, metric.Value);
            if (string.IsNullOrEmpty(metric.ValueWidthTemplate))
                return width;

            return Math.Max(width, MeasureStableValueTemplateWidth(graphics, metric.ValueWidthTemplate));
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

            using (var format = new StringFormat(StringFormat.GenericTypographic))
            {
                format.FormatFlags |= StringFormatFlags.NoWrap;
                path.AddString(
                    text ?? string.Empty,
                    _displayFont.FontFamily,
                    (int)_displayFont.Style,
                    emSize,
                    PointF.Empty,
                    format);
            }

            return path;
        }

        private float GetStrokeWidth()
        {
            return Math.Max(0.85f, DeviceDpi / 96f);
        }

        private void PresentLayeredWindow()
        {
            if (IsDisposed || !IsHandleCreated || ClientSize.Width <= 0 || ClientSize.Height <= 0)
                return;

            using (var bitmap = new Bitmap(ClientSize.Width, ClientSize.Height, PixelFormat.Format32bppPArgb))
            {
                bitmap.SetResolution(DeviceDpi, DeviceDpi);
                using (Graphics graphics = Graphics.FromImage(bitmap))
                {
                    graphics.Clear(Color.Transparent);

                    int backgroundOpacity = Math.Max(
                        0,
                        Math.Min(100, _configuration.BackgroundOpacityPercent));
                    if (backgroundOpacity > 0)
                    {
                        int alpha = (int)Math.Round(255d * backgroundOpacity / 100d);
                        using (var backgroundBrush = new SolidBrush(Color.FromArgb(alpha, _backgroundColor)))
                            graphics.FillRectangle(backgroundBrush, 0, 0, bitmap.Width, bitmap.Height);
                    }

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
        }

        private void ApplyConfiguredPosition()
        {
            Rectangle area = _workingArea.Width > 0 && _workingArea.Height > 0
                ? _workingArea
                : Screen.PrimaryScreen.WorkingArea;
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
                    left = area.Right - Width;
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
                    top = area.Bottom - Height;
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

        [DllImport("gdi32.dll", SetLastError = true)]
        private static extern bool DeleteObject(IntPtr graphicsObject);
    }
}
