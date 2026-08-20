using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace TrayTemps
{
    // Shared visual resize affordance for borderless forms that already handle
    // non-client resize hit testing.
    internal sealed class WindowResizeGripPanel : Panel
    {
        private const int LogicalResizeArea = 8;
        private const int WmNcLButtonDown = 0xA1;
        private const int HtBottomRight = 17;
        private bool _lightTheme;

        [DllImport("user32.dll")]
        private static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        internal WindowResizeGripPanel()
        {
            SetStyle(
                ControlStyles.UserPaint |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.SupportsTransparentBackColor,
                true);

            BackColor = Color.Transparent;
            Cursor = Cursors.SizeNWSE;
            TabStop = false;
        }

        internal bool LightTheme
        {
            get { return _lightTheme; }
            set
            {
                if (_lightTheme == value)
                    return;

                _lightTheme = value;
                Invalidate();
            }
        }

        internal void UpdateDpiSize()
        {
            int resizeArea = Math.Max(6, (int)Math.Round(LogicalResizeArea * DeviceDpi / 96d));
            int size = resizeArea * 2 + 4;
            Size desiredSize = new Size(size, size);

            if (Size != desiredSize)
                Size = desiredSize;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            float dpiScale = Math.Max(0.85f, DeviceDpi / 96f);
            int inset = Math.Max(3, (int)Math.Round(3f * dpiScale));
            int spacing = Math.Max(3, (int)Math.Round(3f * dpiScale));
            int length = Math.Max(5, (int)Math.Round(5f * dpiScale));
            Color color = _lightTheme
                ? Color.FromArgb(125, 75, 75, 75)
                : Color.FromArgb(65, 170, 170, 170);

            using (var pen = new Pen(color, Math.Max(1f, dpiScale)))
            {
                for (int index = 0; index < 3; index++)
                {
                    int offset = index * spacing;
                    e.Graphics.DrawLine(
                        pen,
                        Width - inset - length - offset,
                        Height - inset,
                        Width - inset,
                        Height - inset - length - offset);
                }
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            Form form = FindForm();
            if (e.Button != MouseButtons.Left || form == null || form.WindowState == FormWindowState.Maximized)
                return;

            ReleaseCapture();
            SendMessage(form.Handle, WmNcLButtonDown, (IntPtr)HtBottomRight, IntPtr.Zero);
        }
    }
}
