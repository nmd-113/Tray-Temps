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

            ApplyVisualSettings();
            ResizeForContent();
            ApplyConfiguredPosition();
            PresentLayeredWindow();
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
            int columnGap = ScaleLogical(20);
            int combinedValueGap = ScaleLogical(12);
            int rowHeight = Math.Max(ScaleLogical(22), _displayFont.Height + ScaleLogical(4));
            int[] labelSectionWidths = new int[columns];
            int[] valueBlockWidths = new int[columns];

            using (Graphics graphics = CreateGraphics())
            {
                if (_metrics.Any(metric => metric.TrailingValue != null) &&
                    (_trailingValueSlotWidth <= 0 ||
                     _trailingValueSlotDpi != DeviceDpi))
                {
                    _trailingValueSlotWidth = MeasureStablePercentSlotWidth(graphics);
                    _trailingValueSlotDpi = DeviceDpi;
                }

                for (int index = 0; index < _metrics.Count; index++)
                {
                    int column = index % columns;
                    OsdMetric metric = _metrics[index];
                    int labelWidth = MeasureDisplayTextWidth(graphics, metric.Label);
                    int labelGap = ScaleLogical(Math.Max(0, _configuration.LabelValueSpacing));
                    labelSectionWidths[column] = Math.Max(
                        labelSectionWidths[column],
                        labelWidth + labelGap);
                    valueBlockWidths[column] = Math.Max(
                        valueBlockWidths[column],
                        GetValueBlockWidth(graphics, metric, combinedValueGap));
                }
            }

            int widestColumn = Enumerable.Range(0, columns)
                .Max(column => labelSectionWidths[column] + valueBlockWidths[column]);
            int width = padding * 2 + widestColumn * columns + columnGap * Math.Max(0, columns - 1);
            int height = padding * 2 + rows * rowHeight;
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
            int padding = GetConfiguredPadding();
            int columnGap = ScaleLogical(20);
            int combinedValueGap = ScaleLogical(12);
            int rowHeight = Math.Max(ScaleLogical(22), _displayFont.Height + ScaleLogical(4));
            int availableWidth = ClientSize.Width - padding * 2 - columnGap * Math.Max(0, columns - 1);
            int columnWidth = Math.Max(1, availableWidth / columns);
            int[] valueBlockWidths = new int[columns];

            for (int index = 0; index < _metrics.Count; index++)
            {
                int column = index % columns;
                valueBlockWidths[column] = Math.Max(
                    valueBlockWidths[column],
                    GetValueBlockWidth(graphics, _metrics[index], combinedValueGap));
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
                    int x = padding + column * (columnWidth + columnGap);
                    int y = padding + row * rowHeight;
                    int primaryValueWidth = GetPrimaryValueSlotWidth(graphics, metric);
                    int labelGap = ScaleLogical(Math.Max(0, _configuration.LabelValueSpacing));
                    int valueBlockWidth = valueBlockWidths[column];
                    int valueBlockX = x + Math.Max(0, columnWidth - valueBlockWidth);
                    Rectangle labelBounds = new Rectangle(
                        x,
                        y,
                        Math.Max(1, valueBlockX - x - labelGap),
                        rowHeight);
                    Rectangle primaryValueBounds = new Rectangle(
                        valueBlockX,
                        y,
                        primaryValueWidth,
                        rowHeight);
                    Rectangle trailingValueBounds = metric.TrailingValue != null
                        ? new Rectangle(
                            x + Math.Max(0, columnWidth - _trailingValueSlotWidth),
                            y,
                            _trailingValueSlotWidth,
                            rowHeight)
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
                        StringAlignment.Near);
                    if (metric.TrailingValue != null)
                    {
                        DrawDisplayText(
                            graphics,
                            metric.TrailingValue,
                            trailingValueBounds,
                            textColor,
                            StringAlignment.Near);
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
            float availableWidth = Math.Max(1f, bounds.Width - strokeWidth * 2f);
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
                        ? bounds.Right - strokeWidth - pathBounds.Width - pathBounds.X
                        : bounds.Left + strokeWidth - pathBounds.X;
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
                (int)Math.Ceiling(MeasureTextPathWidth(graphics, text) + GetStrokeWidth() * 2f));
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
