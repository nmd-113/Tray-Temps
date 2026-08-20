using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

public partial class HardwareDetailsDialog : Form
{
    private Func<Task<string>> _liveTextFactory;
    private readonly Func<bool> _shouldStopLiveUpdates;
    private bool _hasSensors;
    private bool _showingSensors;
    private bool _liveUpdateBusy;

    private Color _menuBg = Color.FromArgb(30, 30, 30);
    private Color _selected = Color.FromArgb(45, 45, 45);
    private Color _menuText = Color.WhiteSmoke;
    private Color _disabledText = Color.FromArgb(100, 100, 100);
    private const int CsDropShadow = 0x00020000;

    // API for Drag & Drop
    [DllImport("user32.dll")]
    private static extern bool ReleaseCapture();
    [DllImport("user32.dll")]
    private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);
    [DllImport("user32.dll")]
    private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, ref Point lParam);
    private const int WM_NCLBUTTONDOWN = 0xA1;
    private const int HTCAPTION = 0x2;

    // API for Anti-Flicker
    private const int WM_SETREDRAW = 0x000B;
    private const int EM_GETSCROLLPOS = 0x04DD;
    private const int EM_SETSCROLLPOS = 0x04DE;

    public HardwareDetailsDialog()
    {
        InitializeComponent();
        TrayTemps.EmbeddedFonts.ApplyTo(this);
        ConfigureReportBoxes();
        _closeBtn.Click += closeTopBtn_Click;
        ApplyTheme(false);
    }

    protected override CreateParams CreateParams
    {
        get
        {
            CreateParams createParams = base.CreateParams;
            createParams.ClassStyle |= CsDropShadow;
            return createParams;
        }
    }

    private void ConfigureReportBoxes()
    {
        // Safely replace Designer-created fonts for the report boxes.
        var oldDetailsFont = _detailsBox.Font;
        var newDetailsFont = new Font("Consolas", 9.5f, FontStyle.Regular, GraphicsUnit.Point);
        _detailsBox.Font = newDetailsFont;
        if (oldDetailsFont != null && !ReferenceEquals(oldDetailsFont, newDetailsFont) && !ReferenceEquals(oldDetailsFont, _detailsBox.Parent?.Font))
            oldDetailsFont.Dispose();

        _detailsBox.ScrollBars = RichTextBoxScrollBars.Both;
        _detailsBox.WordWrap = false;

        var oldLiveFont = _liveBox.Font;
        var newLiveFont = new Font("Consolas", 10f, FontStyle.Regular, GraphicsUnit.Point);
        _liveBox.Font = newLiveFont;
        if (oldLiveFont != null && !ReferenceEquals(oldLiveFont, newLiveFont) && !ReferenceEquals(oldLiveFont, _liveBox.Parent?.Font))
            oldLiveFont.Dispose();

        _liveBox.ScrollBars = RichTextBoxScrollBars.Both;
        _liveBox.WordWrap = false;
    }

    public HardwareDetailsDialog(
        string componentName,
        string categoryName,
        string detailsText,
        Func<Task<string>> liveTextFactory = null,
        Func<bool> shouldStopLiveUpdates = null,
        bool lightTheme = false) : this()
    {
        _liveTextFactory = liveTextFactory;
        _shouldStopLiveUpdates = shouldStopLiveUpdates;
        _hasSensors = liveTextFactory != null;
        ApplyTheme(lightTheme);

        Text = componentName;
        titleLabel.Text = componentName;
        menuTitle.Text = categoryName.ToUpper();
        iconLabel.Text = GetDetailsIcon(categoryName + " " + componentName);

        _detailsBox.Text = string.IsNullOrWhiteSpace(detailsText)
            ? "No information available."
            : detailsText;

        _liveBox.Text = _hasSensors ? "Loading sensors..." : "Live sensors not available.";

        _sensorsMenuBtn.Enabled = _hasSensors;
        _sensorsMenuBtn.ForeColor = _hasSensors ? _menuText : _disabledText;

        AttachDragEvents();
    }

    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);
        TrayTemps.WindowCornerHelper.ApplyRoundedCorners(Handle);
        resizeGrip?.UpdateDpiSize();
    }

    protected override void OnResize(EventArgs e)
    {
        base.OnResize(e);
        resizeGrip?.UpdateDpiSize();
    }

    private void HardwareDetailsDialog_Shown(object sender, EventArgs e)
    {
        ClearSelection(_detailsBox);
        ClearSelection(_liveBox);
        ShowPage(false);

        if (_hasSensors)
        {
            _liveTimer.Start();
            _ = RunInitialLiveSensorUpdateAsync();
        }
    }

    private async Task RunInitialLiveSensorUpdateAsync()
    {
        try
        {
            await UpdateLiveSensorsAsync();
        }
        catch (Exception ex)
        {
            Debug.WriteLine("HardwareDetailsDialog initial live sensor update failed: " + ex);
        }
    }

    private void HardwareDetailsDialog_FormClosed(object sender, FormClosedEventArgs e)
    {
        try
        {
            _liveTimer.Stop();
            _liveTimer.Dispose();
        }
        catch (Exception ex)
        {
            Debug.WriteLine("HardwareDetailsDialog_FormClosed: " + ex);
        }
    }

    private void HardwareDetailsDialog_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Escape)
            Close();
    }

    private async void _liveTimer_Tick(object sender, EventArgs e)
    {
        try
        {
            // Stop timer to prevent race conditions if reading takes > 1s
            _liveTimer.Stop();
            await UpdateLiveSensorsAsync();

            if (!IsDisposed && (_shouldStopLiveUpdates == null || !_shouldStopLiveUpdates()))
            {
                _liveTimer.Start();
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine("HardwareDetailsDialog._liveTimer_Tick: " + ex);
        }
    }

    private void _sensorsMenuBtn_Click(object sender, EventArgs e) => ShowPage(true);
    private void _detailsMenuBtn_Click(object sender, EventArgs e) => ShowPage(false);
    private void closeTopBtn_Click(object sender, EventArgs e) => Close();
    private void minimizeBtn_Click(object sender, EventArgs e) => WindowState = FormWindowState.Minimized;

    private void copyAllBtn_Click(object sender, EventArgs e)
    {
        var sb = new System.Text.StringBuilder();

        if (!string.IsNullOrWhiteSpace(_detailsBox.Text))
            sb.Append(_detailsBox.Text);

        if (_hasSensors && !string.IsNullOrWhiteSpace(_liveBox.Text))
        {
            if (sb.Length > 0)
                sb.AppendLine().AppendLine();
            sb.Append(_liveBox.Text);
        }

        CopyToClipboard(sb.ToString());
    }

    private void _copyBtn_Click(object sender, EventArgs e)
    {
        CopyToClipboard(_showingSensors ? _liveBox.Text : _detailsBox.Text);
    }

    private void ShowPage(bool sensors)
    {
        if (sensors && !_hasSensors) return;

        _showingSensors = sensors;

        cardDetails.Visible = !sensors;
        cardSensors.Visible = sensors;

        _detailsMenuBtn.BackColor = sensors ? _menuBg : _selected;
        _sensorsMenuBtn.BackColor = sensors ? _selected : _menuBg;
        _detailsMenuBtn.ForeColor = _menuText;
        _sensorsMenuBtn.ForeColor = _hasSensors ? _menuText : _disabledText;

        _subtitleLabel.Text = sensors ? "Real-time sensor monitoring" : "Static hardware specifications";
        _copyBtn.Text = sensors ? "Copy Sensors" : "Copy Details";
        contentHeaderLabel.Text = sensors ? "Hardware Sensors" : "System Information";

        ClearSelection(sensors ? _liveBox : _detailsBox);
        _closeBtn.Focus();
    }

    private void ApplyTheme(bool light)
    {
        Color windowBack = light ? Color.FromArgb(218, 226, 238) : Color.FromArgb(30, 30, 30);
        Color pageBack = light ? Color.FromArgb(246, 248, 252) : Color.FromArgb(25, 25, 25);
        Color surfaceBack = light ? Color.White : Color.FromArgb(18, 18, 18);
        Color secondarySurface = light ? Color.FromArgb(248, 250, 252) : Color.FromArgb(15, 15, 15);
        Color barBack = light ? Color.White : Color.FromArgb(30, 30, 30);
        Color border = light ? Color.FromArgb(210, 218, 230) : Color.FromArgb(60, 60, 60);
        Color text = light ? Color.FromArgb(31, 41, 55) : Color.WhiteSmoke;
        Color bodyText = light ? Color.FromArgb(51, 65, 85) : Color.FromArgb(220, 220, 220);
        Color muted = light ? Color.FromArgb(91, 103, 122) : Color.DarkGray;
        Color accent = light ? Color.FromArgb(37, 99, 235) : Color.FromArgb(0, 120, 212);
        Color buttonBack = light ? Color.FromArgb(241, 245, 249) : Color.FromArgb(45, 45, 45);
        Color buttonHover = light ? Color.FromArgb(226, 239, 255) : Color.FromArgb(55, 55, 55);
        Color buttonDown = light ? Color.FromArgb(210, 228, 255) : Color.FromArgb(65, 65, 65);

        _menuBg = light ? Color.White : Color.FromArgb(30, 30, 30);
        _selected = light ? Color.FromArgb(226, 239, 255) : Color.FromArgb(45, 45, 45);
        _menuText = text;
        _disabledText = light ? Color.FromArgb(148, 163, 184) : Color.FromArgb(100, 100, 100);

        if (resizeGrip != null && !resizeGrip.IsDisposed)
        {
            resizeGrip.LightTheme = light;
            resizeGrip.BackColor = barBack;
        }

        BackColor = windowBack;
        outerBorder.BackColor = border;
        mainPanel.BackColor = pageBack;
        contentArea.BackColor = pageBack;
        leftMenu.BackColor = _menuBg;
        titleBar.BackColor = barBack;
        bottomBar.BackColor = barBack;
        headerPanel.BackColor = pageBack;
        accentLine.BackColor = accent;

        cardDetails.BackColor = surfaceBack;
        cardSensors.BackColor = secondarySurface;
        _detailsBox.BackColor = surfaceBack;
        _detailsBox.ForeColor = bodyText;
        _liveBox.BackColor = secondarySurface;
        _liveBox.ForeColor = light ? accent : Color.FromArgb(120, 210, 255);

        titleLabel.ForeColor = text;
        _subtitleLabel.ForeColor = muted;
        contentHeaderLabel.ForeColor = muted;
        menuTitle.ForeColor = muted;
        iconLabel.ForeColor = text;

        ApplyDialogButtonTheme(_closeBtn, buttonBack, text, border, buttonHover, buttonDown);
        ApplyDialogButtonTheme(copyAllBtn, buttonBack, text, border, buttonHover, buttonDown);
        ApplyDialogButtonTheme(_copyBtn, buttonBack, text, border, buttonHover, buttonDown);
        ApplyMenuButtonTheme(_detailsMenuBtn, text, buttonHover, buttonDown);
        ApplyMenuButtonTheme(_sensorsMenuBtn, text, buttonHover, buttonDown);
        ApplyTitleButtonTheme(minimizeBtn, text, buttonHover, buttonDown);
        ApplyTitleButtonTheme(closeTopBtn, text, Color.FromArgb(220, 38, 38), Color.DarkRed);

        ShowPage(_showingSensors);
    }

    private static void ApplyDialogButtonTheme(Button button, Color back, Color text, Color border, Color hover, Color down)
    {
        button.BackColor = back;
        button.ForeColor = text;
        button.FlatAppearance.BorderColor = border;
        button.FlatAppearance.MouseOverBackColor = hover;
        button.FlatAppearance.MouseDownBackColor = down;
    }

    private static void ApplyMenuButtonTheme(Button button, Color text, Color hover, Color down)
    {
        button.ForeColor = text;
        button.FlatAppearance.MouseOverBackColor = hover;
        button.FlatAppearance.MouseDownBackColor = down;
    }

    private static void ApplyTitleButtonTheme(Button button, Color text, Color hover, Color down)
    {
        button.ForeColor = text;
        button.FlatAppearance.MouseOverBackColor = hover;
        button.FlatAppearance.MouseDownBackColor = down;
    }

    public void SetLightTheme(bool light)
    {
        ApplyTheme(light);
    }

    public void SetDetailsText(string detailsText)
    {
        if (IsDisposed)
            return;

        if (InvokeRequired)
        {
            try
            {
                BeginInvoke((Action<string>)SetDetailsText, detailsText);
            }
            catch (InvalidOperationException)
            {
                // The dialog closed after the initial disposal check.
            }
            return;
        }

        _detailsBox.Text = string.IsNullOrWhiteSpace(detailsText)
            ? "No information available."
            : detailsText;
        ClearSelection(_detailsBox);
    }

    public void SetLiveTextFactory(Func<Task<string>> liveTextFactory)
    {
        if (liveTextFactory == null || IsDisposed)
            return;

        if (InvokeRequired)
        {
            try
            {
                BeginInvoke((Action<Func<Task<string>>>)SetLiveTextFactory, liveTextFactory);
            }
            catch (InvalidOperationException)
            {
                // The dialog closed after the initial disposal check.
            }
            return;
        }

        _liveTextFactory = liveTextFactory;
        _hasSensors = true;
        _liveBox.Text = "Loading sensors...";
        _sensorsMenuBtn.Enabled = true;
        _sensorsMenuBtn.ForeColor = _menuText;

        if (Visible && !_liveTimer.Enabled)
        {
            _liveTimer.Start();
            _ = RunInitialLiveSensorUpdateAsync();
        }
    }

    private async Task UpdateLiveSensorsAsync()
    {
        // Guard against re-entrancy, missing sensors, or disposed dialog.
        if (_liveUpdateBusy || !_hasSensors || IsDisposed) return;
        _liveUpdateBusy = true;

        bool redrawDisabled = false;
        try
        {
            // Retrieve the live sensor text.
            string text = await _liveTextFactory();

            // If the dialog has been disposed or the control handle is not created,
            // skip updating to avoid cross-thread or invalid handle issues.
            if (IsDisposed || !_liveBox.IsHandleCreated) return;

            // Anti-Flicker: Disable redraw before updating the text.
            SendMessage(_liveBox.Handle, WM_SETREDRAW, IntPtr.Zero, IntPtr.Zero);
            redrawDisabled = true;

            int oldSelectionStart = _liveBox.SelectionStart;
            Point oldScrollPosition = Point.Empty;
            SendMessage(_liveBox.Handle, EM_GETSCROLLPOS, IntPtr.Zero, ref oldScrollPosition);

            _liveBox.Text = string.IsNullOrWhiteSpace(text) ? "No sensors detected." : text;

            _liveBox.SelectionStart = Math.Min(oldSelectionStart, _liveBox.TextLength);
            _liveBox.SelectionLength = 0;
            SendMessage(_liveBox.Handle, EM_SETSCROLLPOS, IntPtr.Zero, ref oldScrollPosition);
        }
        catch
        {
            // Preserve original error handling behavior.
            if (!IsDisposed && _liveBox.IsHandleCreated)
                _liveBox.Text = "Error reading sensors.";
        }
        finally
        {
            // Ensure redraw is always re-enabled if it was disabled.
            if (redrawDisabled && !IsDisposed && _liveBox.IsHandleCreated)
            {
                SendMessage(_liveBox.Handle, WM_SETREDRAW, new IntPtr(1), IntPtr.Zero);
                _liveBox.Invalidate();
            }

            _liveUpdateBusy = false;
        }
    }

    private void AttachDragEvents()
    {
        Control[] dragControls = { leftMenu, iconLabel, menuTitle, titleBar, titleLabel, _subtitleLabel };
        foreach (var ctrl in dragControls)
        {
            ctrl.MouseDown += (s, e) =>
            {
                if (e.Button != MouseButtons.Left) return;
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, (IntPtr)HTCAPTION, IntPtr.Zero);
            };
        }
    }

    private static void ClearSelection(RichTextBox box)
    {
        if (box == null) return;
        box.SelectionStart = 0;
        box.SelectionLength = 0;
    }

    private static void CopyToClipboard(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return;
        try { Clipboard.SetText(text); } catch (Exception ex) { Debug.WriteLine("CopyToClipboard failed: " + ex); }
    }

    private static string GetDetailsIcon(string title)
    {
        string t = title.ToLowerInvariant();
        if (t.Contains("cpu")) return "🧠";
        if (t.Contains("gpu") || t.Contains("video")) return "🎮";
        if (t.Contains("ram") || t.Contains("memory")) return "💾";
        if (t.Contains("storage") || t.Contains("disk")) return "🗄️";
        if (t.Contains("motherboard") || t.Contains("board")) return "🔌";
        return "ℹ️";
    }

    protected override void WndProc(ref Message m)
    {
        const int WM_NCHITTEST = 0x84;
        const int HTLEFT = 10, HTRIGHT = 11, HTTOP = 12, HTTOPLEFT = 13;
        const int HTTOPRIGHT = 14, HTBOTTOM = 15, HTBOTTOMLEFT = 16, HTBOTTOMRIGHT = 17;
        int resizeAreaSize = Math.Max(6, (int)Math.Round(8d * DeviceDpi / 96d));

        base.WndProc(ref m);

        if (m.Msg != WM_NCHITTEST) return;

        long lParam = m.LParam.ToInt64();
        int x = (short)(lParam & 0xFFFF);
        int y = (short)((lParam >> 16) & 0xFFFF);
        Point cursor = PointToClient(new Point(x, y));

        bool left = cursor.X <= resizeAreaSize;
        bool right = cursor.X >= Width - resizeAreaSize;
        bool top = cursor.Y <= resizeAreaSize;
        bool bottom = cursor.Y >= Height - resizeAreaSize;

        if (left && top) m.Result = (IntPtr)HTTOPLEFT;
        else if (left && bottom) m.Result = (IntPtr)HTBOTTOMLEFT;
        else if (right && top) m.Result = (IntPtr)HTTOPRIGHT;
        else if (right && bottom) m.Result = (IntPtr)HTBOTTOMRIGHT;
        else if (left) m.Result = (IntPtr)HTLEFT;
        else if (right) m.Result = (IntPtr)HTRIGHT;
        else if (top) m.Result = (IntPtr)HTTOP;
        else if (bottom) m.Result = (IntPtr)HTBOTTOM;
    }
}
