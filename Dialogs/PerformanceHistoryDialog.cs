using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace TrayTemps
{
    public partial class PerformanceHistoryDialog : Form
    {
        private const int CsDropShadow = 0x00020000;
        private const int WmNcLButtonDown = 0x00A1;
        private const int WmNcHitTest = 0x0084;
        private readonly MainForm _host;
        private bool _loading;
        [DllImport("user32.dll")] private static extern bool ReleaseCapture();
        [DllImport("user32.dll")] private static extern IntPtr SendMessage(IntPtr window, int message, IntPtr wParam, IntPtr lParam);
        protected override CreateParams CreateParams
        {
            get
            {
                var parameters = base.CreateParams;
                if (!EmbeddedFonts.IsDesignTime(this)) parameters.ClassStyle |= CsDropShadow;
                return parameters;
            }
        }
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            if (EmbeddedFonts.IsDesignTime(this)) return;
            WindowCornerHelper.ApplyRoundedCorners(Handle);
            UpdateResizeGrip();
        }
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            if (EmbeddedFonts.IsDesignTime(this) || m.Msg != WmNcHitTest || WindowState == FormWindowState.Maximized) return;
            long coordinates = m.LParam.ToInt64();
            Point cursor = PointToClient(new Point((short)(coordinates & 0xFFFF), (short)((coordinates >> 16) & 0xFFFF)));
            int edge = Math.Max(6, (int)Math.Round(8d * DeviceDpi / 96d));
            bool left = cursor.X <= edge, right = cursor.X >= Width - edge;
            bool top = cursor.Y <= edge, bottom = cursor.Y >= Height - edge;
            if (left && top) m.Result = (IntPtr)13;
            else if (right && top) m.Result = (IntPtr)14;
            else if (left && bottom) m.Result = (IntPtr)16;
            else if (right && bottom) m.Result = (IntPtr)17;
            else if (left) m.Result = (IntPtr)10;
            else if (right) m.Result = (IntPtr)11;
            else if (top) m.Result = (IntPtr)12;
            else if (bottom) m.Result = (IntPtr)15;
        }
        public PerformanceHistoryDialog() { InitializeComponent(); }
        internal PerformanceHistoryDialog(MainForm host) : this()
        {
            _host = host;
            EmbeddedFonts.ApplyTo(this);
            _loading = true;
            recording.Checked = host.HistoryPreferences.RecordingEnabled;
            range.SelectedIndex = Array.IndexOf(new[] { 1, 5, 10, 30 }, host.HistoryPreferences.Minutes);
            Size = new Size(host.HistoryPreferences.WindowWidth, host.HistoryPreferences.WindowHeight);
            Icon = host.Icon;
            _loading = false;
            host.ThemeChanged += ThemeChanged;
            ApplyTheme(); RefreshGraphs();
        }
        private void ThemeChanged(object sender, EventArgs e) => ApplyTheme();
        private void UnsubscribeTheme() { if (_host != null) _host.ThemeChanged -= ThemeChanged; }
        private void ApplyTheme()
        {
            var theme = ThemePalette.Create(_host != null && _host.IsLightModeEnabled);
            BackColor = theme.Border;
            root.BackColor = content.BackColor = plots.BackColor = theme.WindowBack;
            ForeColor = rangeLabel.ForeColor = recording.ForeColor = theme.Text;
            header.BackColor = theme.SurfaceBack;
            titleBar.BackColor = title.BackColor = footer.BackColor = theme.BarBack;
            title.ForeColor = theme.TitleText;
            resizeGrip.LightTheme = _host != null && _host.IsLightModeEnabled;
            resizeGrip.BackColor = theme.BarBack;
            range.BackColor = theme.InputBack; range.ForeColor = theme.Text;
            clear.BackColor = theme.SurfaceBack; clear.ForeColor = theme.Text; clear.FlatAppearance.BorderColor = theme.Border;
            clear.FlatAppearance.MouseOverBackColor = theme.ButtonHover; clear.FlatAppearance.MouseDownBackColor = theme.ButtonDown;
            exitBtn.BackColor = theme.BarBack; exitBtn.ForeColor = theme.TitleText;
            exitBtn.FlatAppearance.BorderColor = theme.BarBack;
            foreach (var graph in new[] { temperatureGraph, usageGraph, fpsGraph, memoryGraph, frameGraph })
            { graph.BackColor = theme.SurfaceBack; graph.ForeColor = theme.Text; }
            RefreshGraphs();
        }
        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            if (_host == null) return;
            FitToWorkingArea(Screen.FromControl(this).WorkingArea);
            UpdateResizeGrip();
            viewTimer.Start(); RefreshGraphs();
        }
        private void FitToWorkingArea(Rectangle area)
        {
            // Keep saved sizes usable on smaller displays, including a higher-DPI screen.
            MinimumSize = new Size(Math.Min(MinimumSize.Width, area.Width), Math.Min(MinimumSize.Height, area.Height));
            Size = new Size(Math.Min(Width, area.Width), Math.Min(Height, area.Height));
            Location = new Point(Math.Max(area.Left, Math.Min(Left, area.Right - Width)), Math.Max(area.Top, Math.Min(Top, area.Bottom - Height)));
        }
        protected override void OnResizeEnd(EventArgs e)
        {
            base.OnResizeEnd(e);
            if (_host != null && WindowState == FormWindowState.Normal) _host.SaveHistoryWindowSize(Size);
        }
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            UpdateResizeGrip();
        }
        private void UpdateResizeGrip()
        {
            if (resizeGrip == null || footer == null || resizeGrip.IsDisposed) return;
            resizeGrip.UpdateDpiSize();
            resizeGrip.Location = new Point(Math.Max(0, footer.ClientSize.Width - resizeGrip.Width),
                Math.Max(0, footer.ClientSize.Height - resizeGrip.Height));
        }
        private void Title_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left || WindowState == FormWindowState.Maximized) return;
            ReleaseCapture();
            SendMessage(Handle, WmNcLButtonDown, (IntPtr)2, IntPtr.Zero);
        }
        private void Exit_Click(object sender, EventArgs e) => Close();
        protected override void OnVisibleChanged(EventArgs e) { base.OnVisibleChanged(e); if (viewTimer != null) { if (Visible) viewTimer.Start(); else viewTimer.Stop(); } }
        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            if (_host != null)
            {
                UnsubscribeTheme();
                if (WindowState == FormWindowState.Normal) _host.SaveHistoryWindowSize(Size);
            }
            base.OnFormClosed(e);
        }
        private void Recording_CheckedChanged(object sender, EventArgs e)
        {
            if (_loading || _host == null) return;
            _host.SetHistoryRecording(recording.Checked); RefreshGraphs();
        }
        private void Range_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_loading || _host == null || range.SelectedIndex < 0) return;
            _host.SetHistoryRange(new[] { 1, 5, 10, 30 }[range.SelectedIndex]); RefreshGraphs();
        }
        private void Clear_Click(object sender, EventArgs e) { _host?.ClearHistory(); RefreshGraphs(); }
        private void ViewTimer_Tick(object sender, EventArgs e) => RefreshGraphs();
        private void RefreshGraphs()
        {
            if (_host == null || IsDisposed || WindowState == FormWindowState.Minimized) return;
            _loading = true;
            try
            {
                recording.Checked = _host.HistoryPreferences.RecordingEnabled;
                range.SelectedIndex = Array.IndexOf(new[] { 1, 5, 10, 30 }, _host.HistoryPreferences.Minutes);
            }
            finally { _loading = false; }
            _host.FillHistoryGraphs(temperatureGraph, usageGraph, fpsGraph, memoryGraph, frameGraph);
        }
    }
}
