using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TrayTemps
{
    internal sealed class HistoryTrace
    {
        internal string Name;
        internal Color Color;
        internal HistoryPoint[] Points;
    }

    public sealed class HistoryGraphControl : Control
    {
        private HistoryTrace[] _traces = Array.Empty<HistoryTrace>();
        private double _start, _end, _minimum, _maximum;
        private string _unit = "";
        private bool _hasSamples;
        private ToolTip _hoverToolTip;
        private Point? _hoverLocation;
        private string _hoverText = "";
        internal double OldestSampleTime { get; private set; } = double.PositiveInfinity;
        private int ScalePixels(int pixels) => (int)Math.Round(pixels * DeviceDpi / 96f);
        internal int PlotWidth => Math.Max(1, Width - ScalePixels(72));
        private int GetTextHeight(Graphics graphics) => TextRenderer.MeasureText(graphics, "Ag", Font, Size.Empty, TextFormatFlags.SingleLine | TextFormatFlags.NoPadding).Height;
        private Rectangle GetPlotBounds(int textHeight)
        {
            int top = ScalePixels(5) + 2 * (textHeight + ScalePixels(2));
            return new Rectangle(ScalePixels(54), top, PlotWidth, Math.Max(1, Height - top - textHeight - ScalePixels(10)));
        }
        public HistoryGraphControl() { DoubleBuffered = true; ResizeRedraw = true; Height = 125; MinimumSize = new Size(0, 110); }
        internal void SetData(string title, string unit, double start, double end, HistoryTrace[] traces, bool percent = false)
        {
            Text = title; _unit = unit; _start = start; _end = end; _traces = traces;
            _hasSamples = false;
            double min = double.PositiveInfinity, max = double.NegativeInfinity;
            OldestSampleTime = double.PositiveInfinity;
            foreach (var trace in traces)
            {
                if (trace.Points.Length > 0) OldestSampleTime = Math.Min(OldestSampleTime, trace.Points[0].Time);
                foreach (var point in trace.Points)
                    if (!double.IsNaN(point.Value)) { _hasSamples = true; min = Math.Min(min, point.Value); max = Math.Max(max, Math.Max(point.Value, point.Peak)); }
            }
            _minimum = percent ? 0 : double.IsInfinity(min) ? 0 : Math.Min(0, min);
            _maximum = percent ? 100 : double.IsInfinity(max) ? 1 : Math.Max(_minimum + 1, max * 1.08);
            UpdateHoverToolTip();
            Invalidate();
        }
        internal void SetTimeRange(double start, double end)
        {
            if (_start == start && _end == end) return;
            _start = start; _end = end;
            UpdateHoverToolTip();
            Invalidate();
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            _hoverLocation = e.Location;
            UpdateHoverToolTip();
        }
        protected override void OnMouseLeave(EventArgs e)
        {
            HideHoverToolTip();
            base.OnMouseLeave(e);
        }
        protected override void OnVisibleChanged(EventArgs e)
        {
            if (!Visible) HideHoverToolTip();
            base.OnVisibleChanged(e);
        }
        protected override void OnResize(EventArgs e)
        {
            HideHoverToolTip();
            base.OnResize(e);
        }
        protected override void OnHandleDestroyed(EventArgs e)
        {
            HideHoverToolTip();
            base.OnHandleDestroyed(e);
        }
        private void HideHoverToolTip()
        {
            _hoverLocation = null; _hoverText = "";
            if (IsHandleCreated) _hoverToolTip?.Hide(this);
        }
        private void UpdateHoverToolTip()
        {
            if (!_hoverLocation.HasValue || !IsHandleCreated || IsDisposed) return;
            string text = GetHoverText(_hoverLocation.Value);
            if (text == _hoverText) return;
            _hoverText = text;
            if (text.Length == 0) { _hoverToolTip?.Hide(this); return; }
            if (_hoverToolTip == null) _hoverToolTip = new ToolTip { UseAnimation = false, UseFading = false };
            _hoverToolTip.Show(text, this, _hoverLocation.Value.X + ScalePixels(12),
                _hoverLocation.Value.Y + ScalePixels(18), 15000);
        }
        private string GetHoverText(Point location)
        {
            Rectangle plot;
            using (var graphics = CreateGraphics()) plot = GetPlotBounds(GetTextHeight(graphics));
            if (!plot.Contains(location) || _end <= _start) return "";
            double secondsPerPixel = (_end - _start) / Math.Max(1, plot.Width - 1);
            double time = _start + (location.X - plot.Left) * secondsPerPixel;
            double sampleTime = time, nearestDistance = double.PositiveInfinity;
            var values = new StringBuilder();
            foreach (var trace in _traces)
            {
                var point = FindHoverPoint(trace.Points, time, secondsPerPixel / 2);
                if (!point.HasValue) continue;
                double distance = Math.Abs(point.Value.Time - time);
                if (distance < nearestDistance) { sampleTime = point.Value.Time; nearestDistance = distance; }
                if (values.Length > 0) values.Append("  |  ");
                values.Append(trace.Name).Append(": ").Append(FormatHoverValue(point.Value.Value));
                if (!double.IsNaN(point.Value.Peak) && !double.IsInfinity(point.Value.Peak) && point.Value.Peak > point.Value.Value)
                    values.Append(" (peak ").Append(FormatHoverValue(point.Value.Peak)).Append(')');
            }
            return values.Length == 0 ? "" : "Nearest samples · " + TimeSpan.FromSeconds(Math.Max(0, _end - sampleTime)).ToString(@"mm\:ss\.f") +
                " before latest\n" + values;
        }
        private string FormatHoverValue(double value) =>
            value.ToString(_unit == "GB" || _unit == "ms" ? "0.00" : "0.#") +
            (_unit.Length == 0 || _unit.StartsWith("°", StringComparison.Ordinal) || _unit == "%" ? "" : " ") + _unit;
        private HistoryPoint? FindHoverPoint(HistoryPoint[] points, double time, double tolerance)
        {
            int low = 0, high = points.Length;
            while (low < high)
            {
                int mid = low + (high - low) / 2;
                if (points[mid].Time < time) low = mid + 1; else high = mid;
            }
            int index = low == points.Length ? low - 1 : low == 0 ? 0 :
                time - points[low - 1].Time <= points[low].Time - time ? low - 1 : low;
            if (index < 0) return null;
            var point = points[index];
            if (point.Time < _start || point.Time > _end || double.IsNaN(point.Value) || double.IsInfinity(point.Value)) return null;
            if (Math.Abs(point.Time - time) > tolerance)
            {
                // Only read between samples that the renderer actually connects.
                if (low == 0 || low == points.Length || double.IsNaN(points[low - 1].Value) || double.IsNaN(points[low].Value) ||
                    points[low - 1].Source != points[low].Source || points[low].Time - points[low - 1].Time > 15) return null;
            }
            return point;
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing) { _hoverToolTip?.Dispose(); _hoverToolTip = null; _hoverLocation = null; _hoverText = ""; }
            base.Dispose(disposing);
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            int textHeight = GetTextHeight(g);
            var plot = GetPlotBounds(textHeight);
            Color secondary = Color.FromArgb((ForeColor.R + BackColor.R) / 2, (ForeColor.G + BackColor.G) / 2, (ForeColor.B + BackColor.B) / 2);
            TextRenderer.DrawText(g, Text == _unit ? Text : Text + " (" + _unit + ")", Font, new Point(ScalePixels(8), ScalePixels(5)), ForeColor);
            using (var grid = new Pen(Color.FromArgb(65, ForeColor)))
            {
                for (int i = 0; i <= 2; i++)
                {
                    int y = plot.Top + plot.Height * i / 2;
                    g.DrawLine(grid, plot.Left, y, plot.Right, y);
                    TextRenderer.DrawText(g, (_maximum - (_maximum - _minimum) * i / 2).ToString("0.#"), Font, new Rectangle(0, y - textHeight / 2, plot.Left - ScalePixels(5), textHeight), ForeColor, TextFormatFlags.Right | TextFormatFlags.SingleLine | TextFormatFlags.NoPadding);
                }
            }
            int legend = plot.Right - _traces.Sum(trace => TextRenderer.MeasureText(g, trace.Name, Font).Width + ScalePixels(16));
            foreach (var trace in _traces)
            {
                int legendWidth = TextRenderer.MeasureText(g, trace.Name, Font).Width;
                TextRenderer.DrawText(g, trace.Name, Font, new Point(legend, ScalePixels(5) + textHeight + ScalePixels(2)), trace.Color);
                legend += legendWidth + ScalePixels(16);
                using (var pen = new Pen(trace.Color, Math.Max(1.5f, DeviceDpi / 64f)))
                using (var marker = new SolidBrush(trace.Color))
                {
                    var state = g.Save(); g.SetClip(plot, CombineMode.Intersect);
                    int previousX = -1, binX = -1; float previousY = 0, binFirst = 0, binLast = 0, binMin = 0, binMax = 0;
                    long source = 0; double previousTime = 0;
                    Action flush = () =>
                    {
                        if (binX < 0) return;
                        if (previousX >= 0) g.DrawLine(pen, previousX, previousY, binX, binFirst);
                        if (binMin != binMax) g.DrawLine(pen, binX, binMin, binX, binMax);
                        else if (previousX < 0)
                            g.FillEllipse(marker, binX - pen.Width / 2, binLast - pen.Width / 2, pen.Width, pen.Width);
                        previousX = binX; previousY = binLast; binX = -1;
                    };
                    try
                    {
                        foreach (var point in trace.Points)
                        {
                            if (double.IsNaN(point.Value) || (previousTime > 0 && (point.Source != source || point.Time - previousTime > 15)))
                            { flush(); previousX = -1; }
                            source = point.Source; previousTime = point.Time;
                            if (double.IsNaN(point.Value)) continue;
                            // Right/bottom clip edges are exclusive: retain zero
                            // values and the newest sample inside the visible plot.
                            int x = plot.Left + (int)((point.Time - _start) / Math.Max(1, _end - _start) * Math.Max(0, plot.Width - 1));
                            float y = plot.Bottom - 1 - (float)((point.Value - _minimum) / (_maximum - _minimum) * Math.Max(0, plot.Height - 1));
                            float peak = plot.Bottom - 1 - (float)((point.Peak - _minimum) / (_maximum - _minimum) * Math.Max(0, plot.Height - 1));
                            if (x != binX) { flush(); binX = x; binFirst = binLast = binMin = binMax = y; }
                            binLast = y; binMin = Math.Min(binMin, Math.Min(y, peak)); binMax = Math.Max(binMax, y);
                        }
                        flush();
                    }
                    finally { g.Restore(state); }
                }
            }
            TextRenderer.DrawText(g, "−" + ((_end - _start) / 60).ToString("0") + " min", Font, new Point(plot.Left, plot.Bottom + ScalePixels(4)), secondary);
            int latestWidth = TextRenderer.MeasureText(g, "latest", Font).Width;
            TextRenderer.DrawText(g, "latest", Font, new Rectangle(plot.Right - latestWidth, plot.Bottom + ScalePixels(4), latestWidth, textHeight), secondary, TextFormatFlags.Right | TextFormatFlags.SingleLine);
            if (!_hasSamples)
                TextRenderer.DrawText(g, "No recorded samples", Font, plot, secondary, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
        }
    }
}
