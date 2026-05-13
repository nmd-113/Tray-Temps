using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

public sealed class HardwareDetailsDialog : Form
{
    private RichTextBox _detailsBox;
    private RichTextBox _liveBox;
    private Button _detailsMenuBtn;
    private Button _sensorsMenuBtn;
    private Button _copyBtn;
    private Button _closeBtn;
    private Label _subtitleLabel;

    private readonly Func<Task<string>> _liveTextFactory;
    private readonly Func<bool> _shouldStopLiveUpdates;

    private readonly Timer _liveTimer;
    private readonly bool _hasSensors;

    private bool _showingSensors;
    private bool _liveUpdateBusy;

    private readonly Color _bg = Color.FromArgb(25, 25, 25);
    private readonly Color _menuBg = Color.FromArgb(30, 30, 30);
    private readonly Color _boxBg = Color.FromArgb(18, 18, 18);
    private readonly Color _boxBg2 = Color.FromArgb(14, 14, 14);
    private readonly Color _accent = Color.FromArgb(0, 120, 212);
    private readonly Color _selected = Color.FromArgb(45, 45, 45);
    private readonly Color _hover = Color.FromArgb(50, 50, 50);

    private const int WM_NCLBUTTONDOWN = 0xA1;
    private const int HTCAPTION = 0x2;

    [System.Runtime.InteropServices.DllImport("user32.dll")]
    private static extern bool ReleaseCapture();

    [System.Runtime.InteropServices.DllImport("user32.dll")]
    private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

    public HardwareDetailsDialog(
        string componentName,
        string categoryName,
        string detailsText,
        Func<Task<string>> liveTextFactory = null,
        Func<bool> shouldStopLiveUpdates = null)
    {
        _liveTextFactory = liveTextFactory;
        _shouldStopLiveUpdates = shouldStopLiveUpdates;
        _hasSensors = liveTextFactory != null;

        Text = componentName;
        StartPosition = FormStartPosition.CenterParent;
        Size = new Size(700, 500);
        MinimumSize = new Size(620, 420);
        BackColor = _menuBg;
        FormBorderStyle = FormBorderStyle.None;
        ShowIcon = false;
        ShowInTaskbar = false;
        KeyPreview = true;

        var outerBorder = new Panel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(1),
            BackColor = Color.FromArgb(55, 55, 55)
        };

        var leftMenu = BuildLeftMenu(componentName, categoryName);
        var titleBar = BuildTitleBar(componentName);
        var bottomBar = BuildBottomBar();

        _detailsBox = CreateTextBox(_boxBg, Color.Gainsboro);
        _detailsBox.Text = string.IsNullOrWhiteSpace(detailsText)
            ? "No information available."
            : detailsText;

        _liveBox = CreateTextBox(_boxBg2, Color.FromArgb(180, 230, 255));
        _liveBox.Visible = false;
        _liveBox.Text = _hasSensors ? "Loading live sensors..." : "No live sensors available.";

        var contentArea = new Panel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(10),
            BackColor = _bg
        };

        contentArea.Controls.Add(_detailsBox);
        contentArea.Controls.Add(_liveBox);

        var mainPanel = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = _bg
        };

        mainPanel.Controls.Add(contentArea);
        mainPanel.Controls.Add(bottomBar);
        mainPanel.Controls.Add(titleBar);

        outerBorder.Controls.Add(mainPanel);
        outerBorder.Controls.Add(leftMenu);

        Controls.Add(outerBorder);

        AcceptButton = _closeBtn;
        CancelButton = _closeBtn;

        _liveTimer = new Timer { Interval = 1000 };
        _liveTimer.Tick += async (s, e) => await UpdateLiveSensorsAsync();

        Shown += async (s, e) =>
        {
            ClearSelection(_detailsBox);
            ClearSelection(_liveBox);
            ShowPage(false);

            if (_hasSensors)
            {
                _liveTimer.Start();
                await UpdateLiveSensorsAsync();
            }
        };

        KeyDown += (s, e) =>
        {
            if (e.KeyCode == Keys.Escape)
                Close();
        };

        FormClosed += (s, e) =>
        {
            try
            {
                _liveTimer.Stop();
                _liveTimer.Dispose();
            }
            catch { }
        };
    }

    private Panel BuildLeftMenu(string componentName, string categoryName)
    {
        var leftMenu = new Panel
        {
            Dock = DockStyle.Left,
            Width = 105,
            BackColor = _menuBg
        };

        var accentLine = new Panel
        {
            Dock = DockStyle.Left,
            Width = 3,
            BackColor = _accent
        };

        var iconLabel = new Label
        {
            Text = GetDetailsIcon(categoryName + " " + componentName),
            Dock = DockStyle.Top,
            Height = 64,
            TextAlign = ContentAlignment.BottomCenter,
            Font = new Font("Segoe UI Emoji", 24f, FontStyle.Regular),
            ForeColor = Color.WhiteSmoke
        };

        var menuTitle = new Label
        {
            Text = categoryName,
            Dock = DockStyle.Top,
            Height = 40,
            TextAlign = ContentAlignment.TopCenter,
            Font = new Font("Segoe UI", 9.5f, FontStyle.Regular),
            ForeColor = Color.WhiteSmoke
        };

        _sensorsMenuBtn = CreateMenuButton("Sensors");
        _sensorsMenuBtn.Enabled = _hasSensors;
        _sensorsMenuBtn.ForeColor = _hasSensors ? Color.WhiteSmoke : Color.Gray;
        _sensorsMenuBtn.Click += (s, e) => ShowPage(true);

        _detailsMenuBtn = CreateMenuButton("Details");
        _detailsMenuBtn.Click += (s, e) => ShowPage(false);

        AttachDrag(leftMenu);
        AttachDrag(iconLabel);
        AttachDrag(menuTitle);

        leftMenu.Controls.Add(_sensorsMenuBtn);
        leftMenu.Controls.Add(_detailsMenuBtn);
        leftMenu.Controls.Add(menuTitle);
        leftMenu.Controls.Add(iconLabel);
        leftMenu.Controls.Add(accentLine);

        return leftMenu;
    }

    private Panel BuildTitleBar(string componentName)
    {
        var titleBar = new Panel
        {
            Dock = DockStyle.Top,
            Height = 48,
            BackColor = _menuBg
        };

        var closeTopBtn = CreateTopButton("✖", true);
        closeTopBtn.Click += (s, e) => Close();

        var minimizeBtn = CreateTopButton("─", false);
        minimizeBtn.Click += (s, e) => WindowState = FormWindowState.Minimized;

        var titleLabel = new Label
        {
            Text = componentName,
            Dock = DockStyle.Top,
            Height = 26,
            Padding = new Padding(12, 5, 0, 0),
            Font = new Font("Segoe UI", 11f, FontStyle.Bold),
            ForeColor = Color.WhiteSmoke,
            AutoEllipsis = true
        };

        _subtitleLabel = new Label
        {
            Text = "Static component details",
            Dock = DockStyle.Top,
            Height = 20,
            Padding = new Padding(12, 0, 0, 0),
            Font = new Font("Segoe UI", 8.5f, FontStyle.Regular),
            ForeColor = Color.Silver,
            AutoEllipsis = true
        };

        AttachDrag(titleBar);
        AttachDrag(titleLabel);
        AttachDrag(_subtitleLabel);

        titleBar.Controls.Add(_subtitleLabel);
        titleBar.Controls.Add(titleLabel);
        titleBar.Controls.Add(minimizeBtn);
        titleBar.Controls.Add(closeTopBtn);

        return titleBar;
    }

    private Panel BuildBottomBar()
    {
        var bottomBar = new Panel
        {
            Dock = DockStyle.Bottom,
            Height = 50,
            Padding = new Padding(10, 9, 10, 9),
            BackColor = _menuBg
        };

        _closeBtn = CreateBottomButton("Close", 100);
        _closeBtn.DialogResult = DialogResult.OK;

        var copyAllBtn = CreateBottomButton("Copy All", 100);
        copyAllBtn.Click += (s, e) =>
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
        };

        _copyBtn = CreateBottomButton("Copy Details", 115);
        _copyBtn.Click += (s, e) =>
        {
            CopyToClipboard(_showingSensors ? _liveBox.Text : _detailsBox.Text);
        };

        bottomBar.Controls.Add(_closeBtn);
        bottomBar.Controls.Add(copyAllBtn);
        bottomBar.Controls.Add(_copyBtn);

        return bottomBar;
    }

    private void ShowPage(bool sensors)
    {
        if (sensors && !_hasSensors)
            return;

        _showingSensors = sensors;

        _detailsBox.Visible = !sensors;
        _liveBox.Visible = sensors;

        _detailsMenuBtn.BackColor = sensors ? _menuBg : _selected;
        _sensorsMenuBtn.BackColor = sensors ? _selected : _menuBg;

        _subtitleLabel.Text = sensors
            ? "Live sensor data"
            : "Static component details";

        _copyBtn.Text = sensors ? "Copy Sensors" : "Copy Details";

        ClearSelection(_detailsBox);
        ClearSelection(_liveBox);

        _closeBtn.Focus();
    }

    private async Task UpdateLiveSensorsAsync()
    {
        if (_liveUpdateBusy || !_hasSensors || IsDisposed)
            return;

        if (_shouldStopLiveUpdates != null && _shouldStopLiveUpdates())
            return;

        _liveUpdateBusy = true;

        try
        {
            string text = await _liveTextFactory();

            if (IsDisposed || !_liveBox.IsHandleCreated)
                return;

            int oldSelectionStart = _liveBox.SelectionStart;

            _liveBox.Text = string.IsNullOrWhiteSpace(text)
                ? "No live sensors available."
                : text;

            _liveBox.SelectionStart = Math.Min(oldSelectionStart, _liveBox.TextLength);
            _liveBox.SelectionLength = 0;
        }
        catch
        {
            if (!IsDisposed && _liveBox.IsHandleCreated)
                _liveBox.Text = "Could not read live sensors.";
        }
        finally
        {
            _liveUpdateBusy = false;
        }
    }

    private Button CreateMenuButton(string text)
    {
        var btn = new Button
        {
            Text = text,
            Height = 42,
            Dock = DockStyle.Top,
            BackColor = _menuBg,
            ForeColor = Color.WhiteSmoke,
            TextAlign = ContentAlignment.MiddleLeft,
            Padding = new Padding(14, 0, 0, 0),
            Font = new Font("Segoe UI", 9.5f, FontStyle.Regular),
            FlatStyle = FlatStyle.Flat,
            UseVisualStyleBackColor = false,
            TabStop = false
        };

        btn.FlatAppearance.BorderSize = 0;
        btn.FlatAppearance.MouseOverBackColor = _hover;
        btn.FlatAppearance.MouseDownBackColor = Color.FromArgb(60, 60, 60);

        return btn;
    }

    private Button CreateTopButton(string text, bool isClose)
    {
        var btn = new Button
        {
            Text = text,
            Width = 48,
            Dock = DockStyle.Right,
            BackColor = _menuBg,
            ForeColor = Color.WhiteSmoke,
            Font = new Font("Segoe UI", isClose ? 10f : 14f, FontStyle.Regular),
            FlatStyle = FlatStyle.Flat,
            UseVisualStyleBackColor = false,
            TabStop = false
        };

        btn.FlatAppearance.BorderSize = 0;
        btn.FlatAppearance.MouseDownBackColor = Color.FromArgb(70, 70, 70);
        btn.FlatAppearance.MouseOverBackColor = isClose ? Color.Red : Color.FromArgb(40, 40, 40);

        return btn;
    }

    private Button CreateBottomButton(string text, int width)
    {
        var btn = new Button
        {
            Text = text,
            Width = width,
            Dock = DockStyle.Right,
            BackColor = Color.FromArgb(45, 45, 45),
            ForeColor = Color.WhiteSmoke,
            Font = new Font("Segoe UI", 9f, FontStyle.Regular),
            FlatStyle = FlatStyle.Flat,
            UseVisualStyleBackColor = false
        };

        btn.FlatAppearance.BorderSize = 1;
        btn.FlatAppearance.BorderColor = Color.FromArgb(75, 75, 75);
        btn.FlatAppearance.MouseOverBackColor = _hover;
        btn.FlatAppearance.MouseDownBackColor = Color.FromArgb(65, 65, 65);

        return btn;
    }

    private static RichTextBox CreateTextBox(Color backColor, Color foreColor)
    {
        return new RichTextBox
        {
            Dock = DockStyle.Fill,
            ReadOnly = true,
            BorderStyle = BorderStyle.None,
            BackColor = backColor,
            ForeColor = foreColor,
            Font = new Font("Consolas", 9.5f),
            WordWrap = false,
            ScrollBars = RichTextBoxScrollBars.Both,
            DetectUrls = false,
            TabStop = false,
            HideSelection = true
        };
    }

    private void AttachDrag(Control control)
    {
        control.MouseDown += (s, e) =>
        {
            if (e.Button != MouseButtons.Left)
                return;

            ReleaseCapture();
            SendMessage(Handle, WM_NCLBUTTONDOWN, (IntPtr)HTCAPTION, IntPtr.Zero);
        };
    }

    private static void ClearSelection(RichTextBox box)
    {
        if (box == null)
            return;

        box.SelectionStart = 0;
        box.SelectionLength = 0;
    }

    private static void CopyToClipboard(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return;

        try
        {
            Clipboard.SetText(text);
        }
        catch { }
    }

    private static string GetDetailsIcon(string title)
    {
        string t = title.ToLowerInvariant();

        if (t.Contains("cpu")) return "🧠";
        if (t.Contains("gpu")) return "🎮";
        if (t.Contains("ram")) return "💾";
        if (t.Contains("storage")) return "🗄️";
        if (t.Contains("motherboard")) return "🔌";

        return "ℹ️";
    }

    protected override void WndProc(ref Message m)
    {
        const int WM_NCHITTEST = 0x84;
        const int HTLEFT = 10;
        const int HTRIGHT = 11;
        const int HTTOP = 12;
        const int HTTOPLEFT = 13;
        const int HTTOPRIGHT = 14;
        const int HTBOTTOM = 15;
        const int HTBOTTOMLEFT = 16;
        const int HTBOTTOMRIGHT = 17;
        const int resizeAreaSize = 8;

        base.WndProc(ref m);

        if (m.Msg != WM_NCHITTEST)
            return;

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