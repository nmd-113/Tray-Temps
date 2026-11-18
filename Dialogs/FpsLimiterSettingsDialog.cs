using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using FpsLimiter.Core;

namespace TrayTemps
{
    public partial class FpsLimiterSettingsDialog : Form
    {
        private MainForm _owner;
        private FpsLimiterService _service;
        private FpsLimiterConfiguration _draft = new FpsLimiterConfiguration();
        private bool _updating;
        private OsdHotkeyModifiers _hotkeyModifiers = FpsLimiterConfiguration.DefaultHotkeyModifiers;
        private Keys _hotkeyKey = FpsLimiterConfiguration.DefaultHotkeyKey;
        internal FpsLimiterConfiguration SelectedConfiguration { get; private set; }

        [DllImport("user32.dll")] private static extern bool ReleaseCapture();
        [DllImport("user32.dll")] private static extern IntPtr SendMessage(IntPtr window, int message, IntPtr wParam, IntPtr lParam);

        public FpsLimiterSettingsDialog()
        {
            InitializeComponent();
            games.Resize += Games_Resize;
            UpdateGamesColumnWidths();
            UpdateHotkeyDisplay();
        }

        private void Games_Resize(object sender, EventArgs e) { UpdateGamesColumnWidths(); }

        private void UpdateGamesColumnWidths()
        {
            int pathWidth = Math.Max(100, games.ClientSize.Width - executableColumn.Width - capColumn.Width);
            // Batch the native resize so ListView recalculates its scrollbars
            // after the columns fit, rather than retaining a stale horizontal bar.
            games.BeginUpdate();
            try { pathColumn.Width = pathWidth; }
            finally { games.EndUpdate(); }
            gamesHeader.ColumnStyles[0].Width = executableColumn.Width;
            gamesHeader.ColumnStyles[1].SizeType = SizeType.Absolute;
            gamesHeader.ColumnStyles[1].Width = pathWidth;
            gamesHeader.ColumnStyles[2].SizeType = SizeType.Absolute;
            gamesHeader.ColumnStyles[2].Width = capColumn.Width;
        }

        internal FpsLimiterSettingsDialog(MainForm owner, FpsLimiterConfiguration configuration, FpsLimiterService service) : this()
        {
            if (EmbeddedFonts.IsDesignTime(this)) return;
            _owner = owner; _service = service; _draft = configuration.Clone();
            EmbeddedFonts.ApplyTo(this);
            fallbackCap.Value = _draft.FramesPerSecond;
            exclusions.Text = _draft.ExcludedProcesses;
            toggleHotkey.Checked = _draft.ToggleHotkeyEnabled;
            _hotkeyModifiers = (OsdHotkeyModifiers)_draft.ToggleHotkeyModifiers;
            _hotkeyKey = (Keys)_draft.ToggleHotkeyKey & Keys.KeyCode;
            if (!FpsLimiterConfiguration.IsValidHotkey(_hotkeyModifiers, _hotkeyKey))
            {
                _hotkeyModifiers = FpsLimiterConfiguration.DefaultHotkeyModifiers;
                _hotkeyKey = FpsLimiterConfiguration.DefaultHotkeyKey;
            }
            UpdateHotkeyDisplay();
            PopulateGames();
            ApplyTheme();
            owner.ThemeChanged += Owner_ThemeChanged;
            if (service != null) service.StateChanged += Service_StateChanged;
            UpdateStatus();
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            if (!EmbeddedFonts.IsDesignTime(this)) { WindowCornerHelper.ApplyRoundedCorners(Handle); resizeGrip.UpdateDpiSize(); }
        }

        protected override void WndProc(ref Message message)
        {
            base.WndProc(ref message);
            if (EmbeddedFonts.IsDesignTime(this) || message.Msg != 0x84) return;
            var point = PointToClient(new Point((short)message.LParam.ToInt64(), (short)(message.LParam.ToInt64() >> 16)));
            int edge = Math.Max(6, 8 * DeviceDpi / 96);
            bool left = point.X <= edge, right = point.X >= Width - edge, top = point.Y <= edge, bottom = point.Y >= Height - edge;
            if (left && top) message.Result = (IntPtr)13;
            else if (right && top) message.Result = (IntPtr)14;
            else if (left && bottom) message.Result = (IntPtr)16;
            else if (right && bottom) message.Result = (IntPtr)17;
            else if (left) message.Result = (IntPtr)10;
            else if (right) message.Result = (IntPtr)11;
            else if (top) message.Result = (IntPtr)12;
            else if (bottom) message.Result = (IntPtr)15;
        }

        private void Title_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            ReleaseCapture(); SendMessage(Handle, 0xA1, (IntPtr)2, IntPtr.Zero);
        }

        private void Close_Click(object sender, EventArgs e) { DialogResult = DialogResult.Cancel; Close(); }

        protected override bool ProcessCmdKey(ref Message message, Keys keyData)
        {
            if (hotkeyValue.Focused && TryCaptureHotkey(keyData)) return true;
            return base.ProcessCmdKey(ref message, keyData);
        }

        private void HotkeyValue_KeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true; e.SuppressKeyPress = true;
            TryCaptureHotkey(e.KeyData);
        }

        private bool TryCaptureHotkey(Keys keyData)
        {
            if (!OsdHotkeyHelper.TryParse(keyData, out OsdHotkeyModifiers modifiers, out Keys key) ||
                !FpsLimiterConfiguration.IsValidHotkey(modifiers, key)) return false;
            _hotkeyModifiers = modifiers; _hotkeyKey = key;
            UpdateHotkeyDisplay();
            return true;
        }

        internal bool CaptureRegisteredHotkey(OsdHotkeyModifiers modifiers, Keys key)
        {
            if (!hotkeyValue.Focused) return false;
            return TryCaptureHotkey(OsdHotkeyHelper.ToKeyData(modifiers, key));
        }

        private void UpdateHotkeyDisplay() { hotkeyValue.Text = OsdHotkeyHelper.Format(_hotkeyModifiers, _hotkeyKey); }

        private void PopulateGames()
        {
            games.BeginUpdate(); games.Items.Clear();
            foreach (var game in _draft.GameLimits.OrderBy(item => Path.GetFileName(item.ExecutablePath), StringComparer.OrdinalIgnoreCase))
            {
                var item = new ListViewItem(Path.GetFileName(game.ExecutablePath));
                item.SubItems.Add(game.ExecutablePath); item.SubItems.Add(game.FramesPerSecond.ToString());
                item.Tag = game; item.ToolTipText = game.ExecutablePath; games.Items.Add(item);
            }
            games.EndUpdate(); Games_SelectedIndexChanged(this, EventArgs.Empty);
        }

        private void Games_SelectedIndexChanged(object sender, EventArgs e)
        {
            _updating = true;
            bool selected = games.SelectedItems.Count != 0;
            removeGame.Enabled = selected; selectedCap.Enabled = selected;
            if (selected) selectedCap.Value = ((GameLimit)games.SelectedItems[0].Tag).FramesPerSecond;
            _updating = false;
        }

        private void SelectedCap_ValueChanged(object sender, EventArgs e)
        {
            if (_updating || games.SelectedItems.Count == 0) return;
            ((GameLimit)games.SelectedItems[0].Tag).FramesPerSecond = (int)selectedCap.Value;
            games.SelectedItems[0].SubItems[2].Text = selectedCap.Value.ToString();
        }

        private void AddGame_Click(object sender, EventArgs e)
        {
            if (executablePicker.ShowDialog(this) != DialogResult.OK) return;
            foreach (string file in executablePicker.FileNames)
            {
                if (_draft.GameLimits.Count >= 512) { MessageBox.Show(this, "Up to 512 executable profiles are supported.", Text); break; }
                string path = Path.GetFullPath(file);
                if (File.Exists(path) && _draft.FindGame(path) == null)
                    _draft.GameLimits.Add(new GameLimit { ExecutablePath = path, FramesPerSecond = (int)fallbackCap.Value });
            }
            PopulateGames();
        }

        private void RemoveGame_Click(object sender, EventArgs e)
        {
            if (games.SelectedItems.Count == 0) return;
            _draft.GameLimits.Remove((GameLimit)games.SelectedItems[0].Tag); PopulateGames();
        }

        private void Save_Click(object sender, EventArgs e)
        {
            var result = _draft.Clone();
            result.FramesPerSecond = (int)fallbackCap.Value;
            result.ExcludedProcesses = exclusions.Text.Trim();
            result.ToggleHotkeyEnabled = toggleHotkey.Checked;
            result.ToggleHotkeyModifiers = (int)_hotkeyModifiers;
            result.ToggleHotkeyKey = (int)_hotkeyKey;
            try { result.Validate(); }
            catch (Exception error) when (error is InvalidDataException || error is ArgumentException)
            { MessageBox.Show(this, error.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            if (result.ToggleHotkeyEnabled && _owner != null && !_owner.CanRegisterFpsLimiterHotkey(_hotkeyModifiers, _hotkeyKey))
            {
                MessageBox.Show(this, "This hotkey is invalid or already used by OSD or another application. Choose Ctrl, Shift, or Alt plus one key.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                hotkeyValue.Focus(); return;
            }
            SelectedConfiguration = result;
            DialogResult = DialogResult.OK; Close();
        }

        private void Notices_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            using (var stream = typeof(FpsLimiterSettingsDialog).Assembly.GetManifestResourceStream("TrayTemps.FpsLimiter.Notice"))
            using (var reader = new StreamReader(stream))
            using (var dialog = new FpsLimiterNoticesDialog(reader.ReadToEnd(), _owner))
                dialog.ShowDialog(this);
        }

        private void Owner_ThemeChanged(object sender, EventArgs e) { ApplyTheme(); }
        private void Service_StateChanged(object sender, EventArgs e) { UpdateStatus(); }
        private void UpdateStatus() { if (!IsDisposed) status.Text = _service?.StatusText ?? "Disabled"; }

        private void ApplyTheme()
        {
            var theme = ThemePalette.Create(_owner != null && _owner.IsLightModeEnabled);
            BackColor = theme.Border;
            ApplyControlTheme(layout, theme);
            titleBar.BackColor = theme.BarBack; title.BackColor = theme.BarBack; bottomBar.BackColor = theme.BarBack;
            resizeGrip.BackColor = theme.BarBack;
            gamesHeader.BackColor = theme.BarBack;
            foreach (Control header in gamesHeader.Controls)
            {
                header.BackColor = theme.BarBack;
                header.ForeColor = theme.TitleText;
            }
            notices.BackColor = theme.BarBack;
            save.BackColor = theme.Accent; save.ForeColor = theme.AccentButtonText;
            status.ForeColor = theme.MutedText; notices.LinkColor = theme.Link;
            resizeGrip.LightTheme = _owner != null && _owner.IsLightModeEnabled;
        }

        private static void ApplyControlTheme(Control control, ThemePalette theme)
        {
            control.BackColor = control is TextBox || control is NumericUpDown || control is ListView ? theme.InputBack : theme.WindowBack;
            control.ForeColor = theme.Text;
            if (control is Button button)
            {
                button.BackColor = theme.ButtonBack; button.ForeColor = theme.BodyText;
                button.FlatAppearance.BorderColor = theme.Border;
                button.FlatAppearance.MouseOverBackColor = theme.ButtonHover;
                button.FlatAppearance.MouseDownBackColor = theme.ButtonDown;
            }
            if (control is NumericUpDown) return;
            foreach (Control child in control.Controls) ApplyControlTheme(child, theme);
        }

        private void Unsubscribe()
        {
            if (_owner != null) _owner.ThemeChanged -= Owner_ThemeChanged;
            if (_service != null) _service.StateChanged -= Service_StateChanged;
            _owner = null; _service = null;
        }
    }
}
