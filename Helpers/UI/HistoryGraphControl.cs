using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
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
        private int ScalePixels(int pixels) => (int)Math.Round(pixels * DeviceDpi / 96f);
        private int GetTextHeight(Graphics graphics) => TextRenderer.MeasureText(graphics, "Ag", Font, Size.Empty, TextFormatFlags.SingleLine | TextFormatFlags.NoPadding).Height;
        private Rectangle GetPlotBounds(int textHeight)
        {
            int top = ScalePixels(5) + 2 * (textHeight + ScalePixels(2));
            return new Rectangle(ScalePixels(54), top, Math.Max(1, Width - ScalePixels(72)), Math.Max(1, Height - top - textHeight - ScalePixels(10)));
        }
        public HistoryGraphControl() { DoubleBuffered = true; ResizeRedraw = true; Height = 125; MinimumSize = new Size(0, 110); }
        internal void SetData(string title, string unit, double start, double end, HistoryTrace[] traces, bool percent = false)
        {
            Text = title; _unit = unit; _start = start; _end = end; _traces = traces;
            double min = double.PositiveInfinity, max = double.NegativeInfinity;
            foreach (var trace in traces)
            foreach (var point in trace.Points)
                if (!double.IsNaN(point.Value)) { min = Math.Min(min, point.Value); max = Math.Max(max, Math.Max(point.Value, point.Peak)); }
            _minimum = percent ? 0 : double.IsInfinity(min) ? 0 : Math.Min(0, min);
            _maximum = percent ? 100 : double.IsInfinity(max) ? 1 : Math.Max(_minimum + 1, max * 1.08);
            Invalidate();
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
                {
                    var state = g.Save(); g.SetClip(plot);
                    int previousX = -1, binX = -1; float previousY = 0, binFirst = 0, binLast = 0, binMin = 0, binMax = 0;
                    long source = 0; double previousTime = 0;
                    Action flush = () =>
                    {
                        if (binX < 0) return;
                        if (previousX >= 0) g.DrawLine(pen, previousX, previousY, binX, binFirst);
                        g.DrawLine(pen, binX, binMin, binX, binMax);
                        previousX = binX; previousY = binLast; binX = -1;
                    };
                    foreach (var point in trace.Points)
                    {
                        if (double.IsNaN(point.Value) || (previousTime > 0 && (point.Source != source || point.Time - previousTime > 15)))
                        { flush(); previousX = -1; }
                        source = point.Source; previousTime = point.Time;
                        if (double.IsNaN(point.Value)) continue;
                        int x = plot.Left + (int)((point.Time - _start) / Math.Max(1, _end - _start) * plot.Width);
                        float y = plot.Bottom - (float)((point.Value - _minimum) / (_maximum - _minimum) * plot.Height);
                        float peak = plot.Bottom - (float)((point.Peak - _minimum) / (_maximum - _minimum) * plot.Height);
                        if (x != binX) { flush(); binX = x; binFirst = binLast = binMin = binMax = y; }
                        binLast = y; binMin = Math.Min(binMin, Math.Min(y, peak)); binMax = Math.Max(binMax, y);
                    }
                    flush(); g.Restore(state);
                }
            }
            TextRenderer.DrawText(g, "−" + ((_end - _start) / 60).ToString("0") + " min", Font, new Point(plot.Left, plot.Bottom + ScalePixels(4)), secondary);
            int latestWidth = TextRenderer.MeasureText(g, "latest", Font).Width;
            TextRenderer.DrawText(g, "latest", Font, new Rectangle(plot.Right - latestWidth, plot.Bottom + ScalePixels(4), latestWidth, textHeight), secondary, TextFormatFlags.Right | TextFormatFlags.SingleLine);
            if (!_traces.Any(trace => trace.Points.Any(point => !double.IsNaN(point.Value))))
                TextRenderer.DrawText(g, "No recorded samples", Font, plot, secondary, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
        }
    }
}
