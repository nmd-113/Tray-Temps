using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace TrayTemps
{
    public partial class OsdSettingsDialog : Form
    {
        private const int CsDropShadow = 0x00020000;
        private const int EmReplaceSel = 0x00C2;
        private readonly MainForm _mainForm;
        private readonly OsdConfiguration _configuration;
        private bool _dragging;
        private Point _dragCursorPoint;
        private Point _dragFormPoint;
        private OsdHotkeyModifiers _hotkeyModifiers = OsdHotkeyHelper.DefaultModifiers;
        private Keys _hotkeyKey = OsdHotkeyHelper.DefaultKey;
        private bool _themeSubscribed;
        private bool _configurationLoaded;
        private bool _hasCustomBackgroundColor;
        private List<OsdItemKind> _separateItemOrder;
        private readonly Dictionary<string, ToolStripMenuItem> _iconCategoryMenus =
            new Dictionary<string, ToolStripMenuItem>(StringComparer.Ordinal);
        private readonly List<string> _iconCategoryOrder = new List<string>();
        private TextBox _activeCustomLabelTextBox;
        private ToolStripMenuItem _insertIconMenu;
        private OsdSettingsPage _currentPage = OsdSettingsPage.Metrics;
        private Color _navigationBack = Color.FromArgb(30, 30, 30);
        private Color _navigationSelected = Color.FromArgb(50, 50, 50);
        private Color _navigationText = Color.WhiteSmoke;

        public OsdSettingsDialog()
        {
            InitializeComponent();
        }

        internal OsdSettingsDialog(MainForm mainForm, OsdConfiguration configuration)
            : this()
        {
            if (EmbeddedFonts.IsDesignTime(this))
                return;

            EmbeddedFonts.ApplyTo(this);
            _mainForm = mainForm;
            _configuration = configuration?.Clone() ?? new OsdConfiguration();

            PopulateOptions();
            LoadConfiguration();
            InitializeCustomLabelContextMenu();
            _configurationLoaded = true;
            ApplyTheme();
            _mainForm.ThemeChanged += MainForm_ThemeChanged;
            _themeSubscribed = true;
        }

        internal OsdConfiguration SelectedConfiguration { get; private set; }
        internal event Action<OsdConfiguration> PreviewConfigurationChanged;

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams parameters = base.CreateParams;
                if (!EmbeddedFonts.IsDesignTime(this))
                    parameters.ClassStyle |= CsDropShadow;
                return parameters;
            }
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            if (!EmbeddedFonts.IsDesignTime(this))
            {
                WindowCornerHelper.ApplyRoundedCorners(Handle);
                resizeGrip?.UpdateDpiSize();
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (!EmbeddedFonts.IsDesignTime(this))
                resizeGrip?.UpdateDpiSize();
        }

        protected override void WndProc(ref Message message)
        {
            const int WmNcHitTest = 0x84;
            const int HtLeft = 10;
            const int HtRight = 11;
            const int HtTop = 12;
            const int HtTopLeft = 13;
            const int HtTopRight = 14;
            const int HtBottom = 15;
            const int HtBottomLeft = 16;
            const int HtBottomRight = 17;
            int resizeAreaSize = Math.Max(6, (int)Math.Round(8d * DeviceDpi / 96d));

            base.WndProc(ref message);

            if (EmbeddedFonts.IsDesignTime(this) || message.Msg != WmNcHitTest)
                return;

            long coordinates = message.LParam.ToInt64();
            int x = (short)(coordinates & 0xFFFF);
            int y = (short)((coordinates >> 16) & 0xFFFF);
            Point cursor = PointToClient(new Point(x, y));

            bool left = cursor.X <= resizeAreaSize;
            bool right = cursor.X >= Width - resizeAreaSize;
            bool top = cursor.Y <= resizeAreaSize;
            bool bottom = cursor.Y >= Height - resizeAreaSize;

            if (left && top) message.Result = (IntPtr)HtTopLeft;
            else if (left && bottom) message.Result = (IntPtr)HtBottomLeft;
            else if (right && top) message.Result = (IntPtr)HtTopRight;
            else if (right && bottom) message.Result = (IntPtr)HtBottomRight;
            else if (left) message.Result = (IntPtr)HtLeft;
            else if (right) message.Result = (IntPtr)HtRight;
            else if (top) message.Result = (IntPtr)HtTop;
            else if (bottom) message.Result = (IntPtr)HtBottom;
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            if (EmbeddedFonts.IsDesignTime(this))
                return;

            Rectangle workingArea = Screen.FromControl(this).WorkingArea;
            int margin = Math.Max(12, DeviceDpi / 8);
            Size maximumSize = new Size(
                Math.Max(320, workingArea.Width - margin * 2),
                Math.Max(420, workingArea.Height - margin * 2));

            if (Width > maximumSize.Width || Height > maximumSize.Height)
            {
                Size = new Size(Math.Min(Width, maximumSize.Width), Math.Min(Height, maximumSize.Height));
                Location = new Point(
                    workingArea.Left + (workingArea.Width - Width) / 2,
                    workingArea.Top + (workingArea.Height - Height) / 2);
            }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            UnsubscribeFromThemeChanges();
            base.OnFormClosed(e);
        }

        protected override bool ProcessCmdKey(ref Message message, Keys keyData)
        {
            if (hotkeyValue != null && hotkeyValue.Focused && TryCaptureHotkey(keyData))
                return true;

            return base.ProcessCmdKey(ref message, keyData);
        }

        private void UnsubscribeFromThemeChanges()
        {
            if (_themeSubscribed && _mainForm != null)
            {
                _mainForm.ThemeChanged -= MainForm_ThemeChanged;
                _themeSubscribed = false;
            }
        }

        private void PopulateOptions()
        {
            positionValue.Items.AddRange(new object[]
            {
                "Top left", "Top center", "Top right",
                "Center left", "Center", "Center right",
                "Bottom left", "Bottom center", "Bottom right"
            });
            fontFamilyValue.Items.AddRange(OsdFontHelper.GetAvailableFamilyNames());
            PopulateNumericOptions(fontSizeValue, 6m, 48m, 0.5m);
            PopulateNumericOptions(screenMarginValue, 0m, 100m, 1m);
            PopulateNumericOptions(labelValueSpacing, 0m, 100m, 1m);
            PopulateNumericOptions(rowsSpacing, 0m, 100m, 1m);
            PopulateNumericOptions(columnsSpacing, 0m, 100m, 1m);
            PopulateNumericOptions(columnsValue, 1m, 8m, 1m);
            PopulateNumericOptions(fpsRefreshIntervalValue, 0.25m, 10m, 0.25m);
            PopulateNumericOptions(fpsLabelValueSpacing, 0m, 100m, 1m);
            PopulateNumericOptions(externalSpacingValue, 0m, 100m, 1m);
            PopulateNumericOptions(cornerRoundnessValue, 0m, 50m, 1m);
            metricGroupingMode.Items.AddRange(new object[]
            {
                "Disabled", "Load + Temps", "Load + Temps + Used Memory"
            });

            foreach (OsdItemKind item in OsdItemOrderHelper.Parse(_configuration.ItemOrder))
                itemOrder.Items.Add(new OsdItemOption(item));

            _separateItemOrder = itemOrder.Items
                .Cast<OsdItemOption>()
                .Select(option => option.Kind)
                .ToList();
        }

        private void LoadConfiguration()
        {
            positionValue.SelectedIndex = ValueHelper.ClampInt((int)_configuration.Position, 0, 8);
            customLabelsEnabled.Checked = _configuration.LabelMode == OsdLabelMode.Custom;
            opacityValue.Value = ValueHelper.ClampInt(_configuration.OpacityPercent, opacityValue.Minimum, opacityValue.Maximum);
            SelectNumericOption(screenMarginValue, ValueHelper.ClampInt(_configuration.ScreenMargin, 0, 100));
            SelectNumericOption(externalSpacingValue, ValueHelper.ClampInt(_configuration.ExternalSpacing, 0, 100));
            SelectNumericOption(cornerRoundnessValue, ValueHelper.ClampInt(_configuration.BackgroundCornerRoundness, 0, 50));
            SelectNumericOption(columnsValue, ValueHelper.ClampInt(_configuration.Columns, 1, 8));
            showCpuUsage.Checked = _configuration.ShowCpuUsage;
            showGpuUsage.Checked = _configuration.ShowGpuUsage;
            showRamUsage.Checked = _configuration.ShowRamUsage;
            showVramUsage.Checked = _configuration.ShowVramUsage;
            showFps.Checked = _configuration.ShowFps;
            showCurrentTime.Checked = _configuration.ShowCurrentTime;
            SelectNumericOption(
                fpsRefreshIntervalValue,
                ValueHelper.ClampDecimal(
                    _configuration.FpsRefreshIntervalSeconds ?? 0.5m,
                    0.25m,
                    10m));
            SelectNumericOption(
                fpsLabelValueSpacing,
                ValueHelper.ClampInt(
                    _configuration.FpsLabelValueSpacing ?? _configuration.LabelValueSpacing,
                    0,
                    100));
            int groupingMode = _configuration.MetricGroupingMode.HasValue
                ? (int)_configuration.MetricGroupingMode.Value
                : (_configuration.CombineTemperatureAndUsage ? 1 : 0);
            metricGroupingMode.SelectedIndex = ValueHelper.ClampInt(groupingMode, 0, 2);
            showCpu.Checked = _configuration.ShowCpu;
            showGpu.Checked = _configuration.ShowGpu;
            SelectFontFamily(_configuration.FontFamily);
            cpuFontColor.BackColor = Color.FromArgb(_configuration.CpuFontColor);
            gpuFontColor.BackColor = Color.FromArgb(_configuration.GpuFontColor);
            ramFontColor.BackColor = Color.FromArgb(_configuration.RamFontColor);
            vramFontColor.BackColor = Color.FromArgb(_configuration.VramFontColor);
            fpsFontColor.BackColor = Color.FromArgb(_configuration.FpsFontColor);
            _hasCustomBackgroundColor = _configuration.BackgroundColor.HasValue;
            backgroundColor.BackColor = _hasCustomBackgroundColor
                ? OsdColorHelper.GetOpaqueColor(_configuration.BackgroundColor.Value)
                : OsdColorHelper.GetDefaultBackground(_mainForm != null && _mainForm.IsLightModeEnabled);
            backgroundOpacityValue.Value = _configuration.TransparentBackground
                ? 0
                : ValueHelper.ClampInt(
                    _configuration.BackgroundOpacityPercent,
                    backgroundOpacityValue.Minimum,
                    backgroundOpacityValue.Maximum);
            customCpuLabel.Text = NormalizeCustomLabel(_configuration.CustomCpuLabel, "CPU Temp");
            customGpuLabel.Text = NormalizeCustomLabel(_configuration.CustomGpuLabel, "GPU Temp");
            customCpuUsageLabel.Text = NormalizeCustomLabel(_configuration.CustomCpuUsageLabel, "CPU Load");
            customGpuUsageLabel.Text = NormalizeCustomLabel(_configuration.CustomGpuUsageLabel, "GPU Load");
            customRamLabel.Text = NormalizeCustomLabel(_configuration.CustomRamLabel, "RAM Use");
            customVramLabel.Text = NormalizeCustomLabel(_configuration.CustomVramLabel, "VRAM Use");
            customFpsLabel.Text = NormalizeCustomLabel(_configuration.CustomFpsLabel, "FPS");
            customTimeLabel.Text = NormalizeCustomLabel(_configuration.CustomTimeLabel, "Time");
            SelectNumericOption(
                labelValueSpacing,
                ValueHelper.ClampInt(_configuration.LabelValueSpacing, 0, 100));
            SelectNumericOption(
                rowsSpacing,
                ValueHelper.ClampInt(_configuration.RowsSpacing, 0, 100));
            SelectNumericOption(
                columnsSpacing,
                ValueHelper.ClampInt(_configuration.ColumnsSpacing, 0, 100));
            SelectNumericOption(
                fontSizeValue,
                ValueHelper.ClampDecimal((decimal)_configuration.FontSize, 6m, 48m));
            _hotkeyModifiers = (OsdHotkeyModifiers)_configuration.HotkeyModifiers;
            _hotkeyKey = (Keys)_configuration.HotkeyKey;
            if (!OsdHotkeyHelper.IsValid(_hotkeyModifiers, _hotkeyKey))
            {
                _hotkeyModifiers = OsdHotkeyHelper.DefaultModifiers;
                _hotkeyKey = OsdHotkeyHelper.DefaultKey;
            }
            hotkeyEnabled.Checked = _configuration.HotkeyEnabled;

            UpdateHotkeyDisplay();
            UpdateHardwareVisibility();
            UpdateCustomLabelAvailability();
            UpdateOpacityLabel();
            UpdateBackgroundOpacityLabel();
            if (itemOrder.Items.Count > 0)
                itemOrder.SelectedIndex = 0;
        }

        private void SaveBtn_Click(object sender, EventArgs e)
        {
            if (!showCpu.Checked && !showGpu.Checked &&
                !showCpuUsage.Checked && !showGpuUsage.Checked &&
                !showRamUsage.Checked && !showVramUsage.Checked && !showFps.Checked && !showCurrentTime.Checked)
            {
                MessageBox.Show(
                    this,
                    "Select at least one metric to display in the OSD.",
                    "OSD Settings",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            if (hotkeyEnabled.Checked &&
                (_mainForm == null || !_mainForm.CanRegisterOsdHotkey(_hotkeyModifiers, _hotkeyKey)))
            {
                MessageBox.Show(
                    this,
                    "This hotkey is invalid or already in use. Choose a shortcut containing Ctrl, Shift, or Alt and one non-modifier key.",
                    "OSD Hotkey",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                hotkeyValue.Focus();
                return;
            }

            SelectedConfiguration = CreateConfigurationFromControls();

            DialogResult = DialogResult.OK;
            Close();
        }

        private OsdConfiguration CreateConfigurationFromControls()
        {
            return new OsdConfiguration
            {
                Enabled = _configuration.Enabled,
                Position = (OsdPosition)positionValue.SelectedIndex,
                ShowCpu = showCpu.Checked,
                ShowGpu = showGpu.Checked,
                FontFamily = fontFamilyValue.Text,
                FontSize = (float)GetNumericOptionValue(fontSizeValue, 16m),
                CpuFontColor = cpuFontColor.BackColor.ToArgb(),
                GpuFontColor = gpuFontColor.BackColor.ToArgb(),
                RamFontColor = ramFontColor.BackColor.ToArgb(),
                VramFontColor = vramFontColor.BackColor.ToArgb(),
                FpsFontColor = fpsFontColor.BackColor.ToArgb(),
                BackgroundColor = _hasCustomBackgroundColor
                    ? backgroundColor.BackColor.ToArgb()
                    : (int?)null,
                BackgroundOpacityPercent = backgroundOpacityValue.Value,
                BackgroundCornerRoundness = (int)GetNumericOptionValue(cornerRoundnessValue, 0m),
                TransparentBackground = backgroundOpacityValue.Value == 0,
                OpacityPercent = opacityValue.Value,
                ShowCpuUsage = showCpuUsage.Checked,
                ShowGpuUsage = showGpuUsage.Checked,
                ShowRamUsage = showRamUsage.Checked,
                ShowVramUsage = showVramUsage.Checked,
                ShowFps = showFps.Checked,
                ShowCurrentTime = showCurrentTime.Checked,
                FpsRefreshIntervalSeconds = GetNumericOptionValue(fpsRefreshIntervalValue, 0.5m),
                FpsLabelValueSpacing = (int)GetNumericOptionValue(fpsLabelValueSpacing, 15m),
                CombineTemperatureAndUsage = metricGroupingMode.SelectedIndex > 0,
                MetricGroupingMode = (OsdMetricGroupingMode)Math.Max(0, metricGroupingMode.SelectedIndex),
                LabelMode = customLabelsEnabled.Checked
                    ? OsdLabelMode.Custom
                    : OsdLabelMode.Short,
                CustomCpuLabel = NormalizeCustomLabel(customCpuLabel.Text, "CPU Temp"),
                CustomGpuLabel = NormalizeCustomLabel(customGpuLabel.Text, "GPU Temp"),
                CustomCpuUsageLabel = NormalizeCustomLabel(customCpuUsageLabel.Text, "CPU Load"),
                CustomGpuUsageLabel = NormalizeCustomLabel(customGpuUsageLabel.Text, "GPU Load"),
                CustomRamLabel = NormalizeCustomLabel(customRamLabel.Text, "RAM Use"),
                CustomVramLabel = NormalizeCustomLabel(customVramLabel.Text, "VRAM Use"),
                CustomFpsLabel = NormalizeCustomLabel(customFpsLabel.Text, "FPS"),
                CustomTimeLabel = NormalizeCustomLabel(customTimeLabel.Text, "Time"),
                LabelValueSpacing = (int)GetNumericOptionValue(labelValueSpacing, 15m),
                RowsSpacing = (int)GetNumericOptionValue(rowsSpacing, 5m),
                ColumnsSpacing = (int)GetNumericOptionValue(columnsSpacing, 20m),
                ScreenMargin = (int)GetNumericOptionValue(screenMarginValue, 10m),
                ExternalSpacing = (int)GetNumericOptionValue(externalSpacingValue, 0m),
                Columns = (int)GetNumericOptionValue(columnsValue, 1m),
                ItemOrder = OsdItemOrderHelper.Serialize(_separateItemOrder),
                HotkeyEnabled = hotkeyEnabled.Checked,
                HotkeyModifiers = (int)_hotkeyModifiers,
                HotkeyKey = (int)_hotkeyKey
            };
        }

        private void PreviewVisualSettings()
        {
            if (!_configurationLoaded)
                return;

            PreviewConfigurationChanged?.Invoke(CreateConfigurationFromControls());
        }

        private void VisualSettingChanged(object sender, EventArgs e)
        {
            PreviewVisualSettings();
        }

        private void InitializeCustomLabelContextMenu()
        {
            var undoItem = new ToolStripMenuItem("Undo", null, (sender, e) =>
            {
                if (CanEditActiveCustomLabel() && _activeCustomLabelTextBox.CanUndo)
                    _activeCustomLabelTextBox.Undo();
            });
            var cutItem = new ToolStripMenuItem("Cut", null, (sender, e) =>
            {
                if (CanEditActiveCustomLabel() && _activeCustomLabelTextBox.SelectionLength > 0)
                    _activeCustomLabelTextBox.Cut();
            });
            var copyItem = new ToolStripMenuItem("Copy", null, (sender, e) =>
            {
                if (_activeCustomLabelTextBox != null && _activeCustomLabelTextBox.SelectionLength > 0)
                    _activeCustomLabelTextBox.Copy();
            });
            var pasteItem = new ToolStripMenuItem("Paste", null, (sender, e) =>
            {
                if (CanEditActiveCustomLabel())
                    _activeCustomLabelTextBox.Paste();
            });
            var deleteItem = new ToolStripMenuItem("Delete", null, (sender, e) =>
            {
                if (CanEditActiveCustomLabel() && _activeCustomLabelTextBox.SelectionLength > 0)
                    ReplaceCustomLabelSelection(_activeCustomLabelTextBox, string.Empty);
            });
            var selectAllItem = new ToolStripMenuItem("Select All", null, (sender, e) =>
            {
                if (_activeCustomLabelTextBox != null && _activeCustomLabelTextBox.Enabled)
                    _activeCustomLabelTextBox.SelectAll();
            });

            customLabelContextMenu.Items.AddRange(new ToolStripItem[]
            {
                undoItem,
                new ToolStripSeparator(),
                cutItem,
                copyItem,
                pasteItem,
                deleteItem,
                new ToolStripSeparator(),
                selectAllItem
            });

            _insertIconMenu = new ToolStripMenuItem("Insert icon");
            AddIconCategory("cpu", "CPU / Processor",
                "⚙|Gear", "⚡|Lightning", "♨|Hot springs", "🧠|Brain", "💻|Laptop", "🖥|Desktop", "🖧|Network", "🔧|Wrench", "🔥|Fire", "⏣|Hexagon");
            AddIconCategory("gpu", "GPU / Graphics",
                "🎮|Gamepad", "🕹|Joystick", "🖥|Desktop", "🎨|Palette", "🖼|Picture", "◈|Diamond outline", "◆|Diamond", "✦|Sparkle", "⚡|Lightning", "🚀|Rocket", "👁️|Eye");
            AddIconCategory("ram", "RAM / System Memory",
                "▣|Square with fill", "▤|Bars", "▥|Bars", "▦|Grid", "▧|Shaded grid", "▨|Shaded grid", "💾|Floppy disk", "🗃|Card file", "📦|Package", "🧠|Brain", "🛢|Storage");
            AddIconCategory("vram", "VRAM / Video Memory",
                "🎮|Gamepad", "▣|Square with fill", "▦|Grid", "▧|Shaded grid", "◈|Diamond outline", "◉|Bullseye", "💾|Floppy disk", "🖼|Picture", "⚡|Lightning", "◆|Diamond");
            AddIconCategory("fps", "FPS / Performance",
                "◉|Bullseye", "◎|Double circle", "◌|Dotted circle", "◍|Shaded circle", "↗|Rising arrow", "↟|Up arrow", "↑|Up", "⇧|Shift arrow", "⚡|Lightning", "🚀|Rocket", "📈|Rising chart", "📊|Bar chart", "🎯|Target");
            AddIconCategory("temperature", "Temperature / Cooling",
                "🌡|Thermometer", "♨|Hot springs", "☀|Sun", "❄|Snowflake", "❅|Snowflake", "🔥|Fire", "💧|Droplet", "🌊|Wave", "🌀|Cyclone", "⚡|Lightning");
            AddIconCategory("clock", "Clock / Time",
                "◷|Clock", "◴|Clock", "◵|Clock", "◶|Clock", "◰|Clock", "◱|Clock", "⏱|Stopwatch", "⏲|Timer", "⏰|Alarm clock", "⌚|Watch", "🕒|Clock face", "⧖|Hourglass");
            AddIconCategory("general", "General / Status",
                "●|Filled circle", "○|Circle", "◉|Bullseye", "◎|Double circle", "★|Star", "☆|Outline star", "✓|Check", "✔|Heavy check", "✕|Cross", "✖|Heavy cross", "▲|Up triangle", "▼|Down triangle", "◆|Diamond", "◇|Outline diamond", "■|Square", "□|Outline square", "▶|Play", "▷|Outline play");

            customLabelContextMenu.Items.Add(new ToolStripSeparator());
            customLabelContextMenu.Items.Add(_insertIconMenu);
            customLabelContextMenu.ShowItemToolTips = true;
            _insertIconMenu.DropDown.ShowItemToolTips = true;
            customLabelContextMenu.Opening += CustomLabelContextMenu_Opening;
        }

        private bool CanEditActiveCustomLabel()
        {
            return _activeCustomLabelTextBox != null && _activeCustomLabelTextBox.Enabled &&
                   !_activeCustomLabelTextBox.ReadOnly;
        }

        private void AddIconCategory(string key, string title, params string[] entries)
        {
            var categoryMenu = new ToolStripMenuItem(title) { Tag = key };
            var seenIcons = new HashSet<string>(StringComparer.Ordinal);
            foreach (string entry in entries)
            {
                int separator = entry.IndexOf('|');
                if (separator <= 0)
                    continue;

                string icon = entry.Substring(0, separator);
                if (!seenIcons.Add(icon))
                    continue;

                string name = entry.Substring(separator + 1);
                var iconItem = new ToolStripMenuItem(icon + "  " + name)
                {
                    ToolTipText = "Insert " + name + " (" + icon + ")",
                    Tag = icon
                };
                iconItem.Click += InsertIcon_Click;
                categoryMenu.DropDownItems.Add(iconItem);
            }

            _iconCategoryMenus.Add(key, categoryMenu);
            _iconCategoryOrder.Add(key);
            _insertIconMenu.DropDownItems.Add(categoryMenu);
        }

        private void CustomLabelContextMenu_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _activeCustomLabelTextBox = customLabelContextMenu.SourceControl as TextBox;
            if (_activeCustomLabelTextBox == null)
            {
                e.Cancel = true;
                return;
            }

            TextBox textBox = _activeCustomLabelTextBox;
            bool hasSelection = textBox.SelectionLength > 0;
            bool editable = textBox.Enabled && !textBox.ReadOnly;
            if (customLabelContextMenu.Items[0] is ToolStripMenuItem undoItem)
                undoItem.Enabled = editable && textBox.CanUndo;
            if (customLabelContextMenu.Items[2] is ToolStripMenuItem cutItem)
                cutItem.Enabled = editable && hasSelection;
            if (customLabelContextMenu.Items[3] is ToolStripMenuItem copyItem)
                copyItem.Enabled = hasSelection;
            if (customLabelContextMenu.Items[4] is ToolStripMenuItem pasteItem)
                pasteItem.Enabled = editable;
            if (customLabelContextMenu.Items[5] is ToolStripMenuItem deleteItem)
                deleteItem.Enabled = editable && hasSelection;
            if (customLabelContextMenu.Items[7] is ToolStripMenuItem selectAllItem)
                selectAllItem.Enabled = textBox.TextLength > 0;

            ReorderIconCategories(GetPrioritizedIconCategories(textBox.Name));
        }

        private void ReorderIconCategories(IEnumerable<string> prioritizedCategories)
        {
            _insertIconMenu.DropDownItems.Clear();
            var orderedKeys = new List<string>(prioritizedCategories);
            foreach (string key in _iconCategoryOrder)
            {
                if (!orderedKeys.Contains(key))
                    orderedKeys.Add(key);
            }

            foreach (string key in orderedKeys)
                _insertIconMenu.DropDownItems.Add(_iconCategoryMenus[key]);

            int availableLength = _activeCustomLabelTextBox.MaxLength == 0
                ? int.MaxValue
                : _activeCustomLabelTextBox.MaxLength -
                    (_activeCustomLabelTextBox.TextLength - _activeCustomLabelTextBox.SelectionLength);
            foreach (ToolStripMenuItem category in _iconCategoryMenus.Values)
            {
                foreach (ToolStripMenuItem iconItem in category.DropDownItems.OfType<ToolStripMenuItem>())
                    iconItem.Enabled = iconItem.Tag is string icon && icon.Length <= availableLength;
            }
        }

        private static IEnumerable<string> GetPrioritizedIconCategories(string textBoxName)
        {
            switch (textBoxName)
            {
                case "customCpuLabel": return new[] { "cpu", "temperature" };
                case "customGpuLabel": return new[] { "gpu", "temperature" };
                case "customCpuUsageLabel": return new[] { "cpu", "fps" };
                case "customGpuUsageLabel": return new[] { "gpu", "fps" };
                case "customRamLabel": return new[] { "ram" };
                case "customVramLabel": return new[] { "vram" };
                case "customFpsLabel": return new[] { "fps" };
                case "customTimeLabel": return new[] { "clock" };
                default: return Array.Empty<string>();
            }
        }

        private void InsertIcon_Click(object sender, EventArgs e)
        {
            if (_activeCustomLabelTextBox == null || !_activeCustomLabelTextBox.Enabled ||
                _activeCustomLabelTextBox.ReadOnly || !(sender is ToolStripMenuItem item) ||
                !(item.Tag is string icon))
                return;

            int availableLength = _activeCustomLabelTextBox.MaxLength == 0
                ? int.MaxValue
                : _activeCustomLabelTextBox.MaxLength -
                    (_activeCustomLabelTextBox.TextLength - _activeCustomLabelTextBox.SelectionLength);
            if (icon.Length > availableLength)
                return;

            ReplaceCustomLabelSelection(_activeCustomLabelTextBox, icon);
            _activeCustomLabelTextBox.Focus();
        }

        private static void ReplaceCustomLabelSelection(TextBox textBox, string value)
        {
            if (!textBox.IsHandleCreated)
                textBox.CreateControl();

            IntPtr textPointer = Marshal.StringToHGlobalUni(value);
            try
            {
                // TRUE asks the native edit control to retain the previous text for Undo.
                SendMessageW(textBox.Handle, EmReplaceSel, new IntPtr(1), textPointer);
            }
            finally
            {
                Marshal.FreeHGlobal(textPointer);
            }
        }

        private void SelectFontFamily(string familyName)
        {
            string requested = OsdFontHelper.NormalizeFamilyName(
                string.IsNullOrWhiteSpace(familyName)
                    ? OsdFontHelper.DefaultFamily
                    : familyName);

            for (int index = 0; index < fontFamilyValue.Items.Count; index++)
            {
                if (string.Equals(fontFamilyValue.Items[index].ToString(), requested, StringComparison.OrdinalIgnoreCase))
                {
                    fontFamilyValue.SelectedIndex = index;
                    return;
                }
            }

            fontFamilyValue.Items.Add(requested);
            fontFamilyValue.SelectedIndex = fontFamilyValue.Items.Count - 1;
        }

        private static void PopulateNumericOptions(
            ComboBox input,
            decimal minimum,
            decimal maximum,
            decimal increment)
        {
            for (decimal value = minimum; value <= maximum; value += increment)
                input.Items.Add(new OsdNumericOption(value));
        }

        private static void SelectNumericOption(ComboBox input, decimal value)
        {
            int insertIndex = input.Items.Count;

            for (int index = 0; index < input.Items.Count; index++)
            {
                if (!(input.Items[index] is OsdNumericOption option))
                    continue;

                if (option.Value == value)
                {
                    input.SelectedIndex = index;
                    return;
                }

                if (option.Value > value)
                {
                    insertIndex = index;
                    break;
                }
            }

            input.Items.Insert(insertIndex, new OsdNumericOption(value));
            input.SelectedIndex = insertIndex;
        }

        private static decimal GetNumericOptionValue(ComboBox input, decimal fallback)
        {
            return input.SelectedItem is OsdNumericOption option
                ? option.Value
                : fallback;
        }

        private static string NormalizeCustomLabel(string value, string fallback)
        {
            string text = HardwareReportFormatHelper.SanitizeSingleLineText(value);
            return value == null
                ? fallback
                : string.IsNullOrWhiteSpace(text) ? string.Empty : text;
        }

        private void HotkeyValue_KeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;
            e.SuppressKeyPress = true;

            TryCaptureHotkey(e.KeyData);
        }

        private bool TryCaptureHotkey(Keys keyData)
        {
            OsdHotkeyModifiers modifiers = OsdHotkeyModifiers.None;
            if ((keyData & Keys.Control) != Keys.None)
                modifiers |= OsdHotkeyModifiers.Control;
            if ((keyData & Keys.Shift) != Keys.None)
                modifiers |= OsdHotkeyModifiers.Shift;
            if ((keyData & Keys.Alt) != Keys.None)
                modifiers |= OsdHotkeyModifiers.Alt;

            Keys key = keyData & Keys.KeyCode;
            if (!OsdHotkeyHelper.IsValid(modifiers, key))
                return false;

            _hotkeyModifiers = modifiers;
            _hotkeyKey = key;
            UpdateHotkeyDisplay();
            return true;
        }

        internal bool CaptureRegisteredHotkey(OsdHotkeyModifiers modifiers, Keys key)
        {
            if (hotkeyValue == null || !hotkeyValue.Focused)
                return false;

            Keys keyData = key & Keys.KeyCode;
            if ((modifiers & OsdHotkeyModifiers.Control) != 0)
                keyData |= Keys.Control;
            if ((modifiers & OsdHotkeyModifiers.Shift) != 0)
                keyData |= Keys.Shift;
            if ((modifiers & OsdHotkeyModifiers.Alt) != 0)
                keyData |= Keys.Alt;

            return TryCaptureHotkey(keyData);
        }

        private void UpdateHotkeyDisplay()
        {
            hotkeyValue.Text = OsdHotkeyHelper.Format(_hotkeyModifiers, _hotkeyKey);
        }

        private void OpacityValue_ValueChanged(object sender, EventArgs e)
        {
            UpdateOpacityLabel();
            PreviewVisualSettings();
        }

        private void UpdateOpacityLabel()
        {
            opacityValueLabel.Text = opacityValue.Value + "%";
        }

        private void BackgroundOpacityValue_ValueChanged(object sender, EventArgs e)
        {
            UpdateBackgroundOpacityLabel();
            PreviewVisualSettings();
        }

        private void UpdateBackgroundOpacityLabel()
        {
            backgroundOpacityValueLabel.Text = backgroundOpacityValue.Value + "%";
        }

        private void CustomLabelsEnabled_CheckedChanged(object sender, EventArgs e)
        {
            UpdateCustomLabelAvailability();
            PreviewVisualSettings();
        }

        private void UpdateCustomLabelAvailability()
        {
            bool enabled = customLabelsEnabled.Checked;
            bool cpuTemperatureVisible = showCpu.Checked;
            bool gpuTemperatureVisible = showGpu.Checked;
            bool cpuUsageCombined = CombineCpuTemperatureAndUsage;
            bool gpuUsageCombined = CombineGpuTemperatureAndUsage;
            bool cpuMemoryCombined = CombineCpuMemory;
            bool gpuMemoryCombined = CombineGpuMemory;
            bool cpuUsageVisible = showCpuUsage.Checked && !cpuUsageCombined;
            bool gpuUsageVisible = showGpuUsage.Checked && !gpuUsageCombined;

            customCpuLabel.Enabled = enabled && cpuTemperatureVisible;
            customGpuLabel.Enabled = enabled && gpuTemperatureVisible;
            customCpuUsageLabel.Enabled = enabled && cpuUsageVisible;
            customGpuUsageLabel.Enabled = enabled && gpuUsageVisible;
            customRamLabel.Enabled = enabled && showRamUsage.Checked && !cpuMemoryCombined;
            customVramLabel.Enabled = enabled && showVramUsage.Checked && !gpuMemoryCombined;
            customFpsLabel.Enabled = enabled && showFps.Checked;
            customTimeLabel.Enabled = enabled && showCurrentTime.Checked;

        }

        private bool CanCombineCpuTemperatureAndUsage => showCpu.Checked && showCpuUsage.Checked;
        private bool CanCombineGpuTemperatureAndUsage => showGpu.Checked && showGpuUsage.Checked;
        private OsdMetricGroupingMode SelectedGroupingMode => (OsdMetricGroupingMode)Math.Max(0, metricGroupingMode.SelectedIndex);
        private bool CombineCpuTemperatureAndUsage =>
            SelectedGroupingMode != OsdMetricGroupingMode.Disabled && CanCombineCpuTemperatureAndUsage;
        private bool CombineGpuTemperatureAndUsage =>
            SelectedGroupingMode != OsdMetricGroupingMode.Disabled && CanCombineGpuTemperatureAndUsage;
        private bool CombineCpuMemory => showCpu.Checked && showRamUsage.Checked &&
            SelectedGroupingMode == OsdMetricGroupingMode.LoadTempsAndUsedMemory;
        private bool CombineGpuMemory => showGpu.Checked && showVramUsage.Checked &&
            SelectedGroupingMode == OsdMetricGroupingMode.LoadTempsAndUsedMemory;

        private void HardwareVisibility_CheckedChanged(object sender, EventArgs e)
        {
            UpdateHardwareVisibility();
            UpdateCustomLabelAvailability();
            UpdateItemOrderForCombinedMode();
            PreviewVisualSettings();
        }

        private void UpdateItemOrderForCombinedMode()
        {
            OsdItemOption selectedItem = itemOrder.SelectedItem as OsdItemOption;
            OsdItemKind? selectedKind = selectedItem?.Kind;
            OsdMetricGroupingMode selectedGrouping = selectedItem?.GroupingMode ?? OsdMetricGroupingMode.Disabled;
            var updatedItems = new List<OsdItemOption>();
            bool cpuAdded = false;
            bool gpuAdded = false;
            bool combineCpu = CombineCpuTemperatureAndUsage || CombineCpuMemory;
            bool combineGpu = CombineGpuTemperatureAndUsage || CombineGpuMemory;

            foreach (OsdItemKind item in _separateItemOrder)
            {
                if (combineCpu && IsCpuGroupedItem(item) &&
                    (item != OsdItemKind.RamUsage || CombineCpuMemory))
                {
                    if (!cpuAdded)
                    {
                        updatedItems.Add(new OsdItemOption(
                            OsdItemKind.CpuTemperature,
                            SelectedGroupingMode));
                        cpuAdded = true;
                    }
                    continue;
                }

                if (combineGpu && IsGpuGroupedItem(item) &&
                    (item != OsdItemKind.VramUsage || CombineGpuMemory))
                {
                    if (!gpuAdded)
                    {
                        updatedItems.Add(new OsdItemOption(
                            OsdItemKind.GpuTemperature,
                            SelectedGroupingMode));
                        gpuAdded = true;
                    }
                    continue;
                }

                updatedItems.Add(new OsdItemOption(item));
            }

            if (ItemOrderMatches(updatedItems))
                return;

            itemOrder.BeginUpdate();
            try
            {
                itemOrder.Items.Clear();
                itemOrder.Items.AddRange(updatedItems.Cast<object>().ToArray());
                SelectUpdatedOrderItem(selectedKind, selectedGrouping);
            }
            finally
            {
                itemOrder.EndUpdate();
            }
        }

        private static bool IsCpuGroupedItem(OsdItemKind item) =>
            item == OsdItemKind.CpuTemperature || item == OsdItemKind.CpuUsage || item == OsdItemKind.RamUsage;

        private static bool IsGpuGroupedItem(OsdItemKind item) =>
            item == OsdItemKind.GpuTemperature || item == OsdItemKind.GpuUsage || item == OsdItemKind.VramUsage;

        private bool ItemOrderMatches(
            IList<OsdItemOption> updatedItems)
        {
            if (itemOrder.Items.Count != updatedItems.Count)
                return false;

            for (int index = 0; index < updatedItems.Count; index++)
            {
                OsdItemOption updated = updatedItems[index];
                if (!(itemOrder.Items[index] is OsdItemOption current) || current.Kind != updated.Kind ||
                    current.GroupingMode != updated.GroupingMode)
                {
                    return false;
                }
            }

            return true;
        }

        private void SelectUpdatedOrderItem(OsdItemKind? selectedKind, OsdMetricGroupingMode selectedGrouping)
        {
            if (!selectedKind.HasValue || itemOrder.Items.Count == 0)
                return;

            for (int index = 0; index < itemOrder.Items.Count; index++)
            {
                if (!(itemOrder.Items[index] is OsdItemOption option))
                    continue;

                bool sameMetricGroup = option.Kind == selectedKind.Value ||
                    (IsCpuGroupedItem(option.Kind) && IsCpuGroupedItem(selectedKind.Value)) ||
                    (IsGpuGroupedItem(option.Kind) && IsGpuGroupedItem(selectedKind.Value));
                if (sameMetricGroup &&
                    (option.GroupingMode == selectedGrouping || option.GroupingMode == SelectedGroupingMode))
                {
                    itemOrder.SelectedIndex = index;
                    return;
                }
            }

            itemOrder.SelectedIndex = 0;
        }

        private void UpdateHardwareVisibility()
        {
            cpuFontColor.Enabled = showCpu.Checked || showCpuUsage.Checked;
            gpuFontColor.Enabled = showGpu.Checked || showGpuUsage.Checked;
            ramFontColor.Enabled = showRamUsage.Checked;
            vramFontColor.Enabled = showVramUsage.Checked;
            fpsFontColor.Enabled = showFps.Checked;
            fpsRefreshIntervalLabel.Enabled = showFps.Checked;
            fpsRefreshIntervalValue.Enabled = showFps.Checked;
            fpsLabelValueSpacingLabel.Enabled = showFps.Checked;
            fpsLabelValueSpacing.Enabled = showFps.Checked;
            metricGroupingMode.Enabled = showCpu.Checked || showGpu.Checked;
        }

        private void FontColor_Click(object sender, EventArgs e)
        {
            if (!(sender is Button target))
                return;

            colorDialog.Color = target.BackColor;
            if (colorDialog.ShowDialog(this) == DialogResult.OK)
            {
                target.BackColor = colorDialog.Color;
                target.ForeColor = GetReadableForeground(target.BackColor);
                if (target == backgroundColor)
                    _hasCustomBackgroundColor = true;
                PreviewVisualSettings();
            }
        }

        private void OrderUp_Click(object sender, EventArgs e)
        {
            MoveSelectedItem(-1);
        }

        private void OrderDown_Click(object sender, EventArgs e)
        {
            MoveSelectedItem(1);
        }

        private void MoveSelectedItem(int offset)
        {
            int oldIndex = itemOrder.SelectedIndex;
            int newIndex = oldIndex + offset;
            if (oldIndex < 0 || newIndex < 0 || newIndex >= itemOrder.Items.Count)
                return;

            object item = itemOrder.Items[oldIndex];
            itemOrder.Items.RemoveAt(oldIndex);
            itemOrder.Items.Insert(newIndex, item);
            itemOrder.SelectedIndex = newIndex;
            UpdateSeparateItemOrderFromDisplayedItems();
            PreviewVisualSettings();
        }

        private void UpdateSeparateItemOrderFromDisplayedItems()
        {
            var updatedOrder = new List<OsdItemKind>();

            foreach (OsdItemOption option in itemOrder.Items.Cast<OsdItemOption>())
            {
                if (option.GroupingMode != OsdMetricGroupingMode.Disabled)
                {
                    updatedOrder.Add(option.Kind);
                    bool cpu = option.Kind == OsdItemKind.CpuTemperature;
                    updatedOrder.Add(cpu ? OsdItemKind.CpuUsage : OsdItemKind.GpuUsage);
                    if (option.GroupingMode == OsdMetricGroupingMode.LoadTempsAndUsedMemory)
                        updatedOrder.Add(cpu ? OsdItemKind.RamUsage : OsdItemKind.VramUsage);
                }
                else
                {
                    updatedOrder.Add(option.Kind);
                }
            }

            _separateItemOrder = updatedOrder;
        }

        private void MainForm_ThemeChanged(object sender, EventArgs e)
        {
            ApplyTheme();
        }

        private void MetricsNavButton_Click(object sender, EventArgs e)
        {
            ShowPage(OsdSettingsPage.Metrics);
        }

        private void AppearanceNavButton_Click(object sender, EventArgs e)
        {
            ShowPage(OsdSettingsPage.Appearance);
        }

        private void LayoutNavButton_Click(object sender, EventArgs e)
        {
            ShowPage(OsdSettingsPage.Layout);
        }

        private void ShowPage(OsdSettingsPage page)
        {
            _currentPage = page;

            TabPage selectedPage = page == OsdSettingsPage.Metrics
                ? metricsPage
                : page == OsdSettingsPage.Appearance ? appearancePage : layoutPage;

            pageTabs.SuspendLayout();
            try
            {
                pageTabs.SelectedTab = selectedPage;
            }
            finally
            {
                pageTabs.ResumeLayout(true);
            }

            metricsNavButton.BackColor = page == OsdSettingsPage.Metrics
                ? _navigationSelected
                : _navigationBack;
            appearanceNavButton.BackColor = page == OsdSettingsPage.Appearance
                ? _navigationSelected
                : _navigationBack;
            layoutNavButton.BackColor = page == OsdSettingsPage.Layout
                ? _navigationSelected
                : _navigationBack;

            metricsNavButton.ForeColor = _navigationText;
            appearanceNavButton.ForeColor = _navigationText;
            layoutNavButton.ForeColor = _navigationText;

            Button selectedButton = page == OsdSettingsPage.Metrics
                ? metricsNavButton
                : page == OsdSettingsPage.Appearance ? appearanceNavButton : layoutNavButton;
            if (accentLine.Parent != selectedButton)
                selectedButton.Controls.Add(accentLine);
            accentLine.Dock = DockStyle.Left;
            accentLine.Width = 3;
            accentLine.BringToFront();
        }

        private void ApplyCustomLabelContextMenuTheme(ThemePalette theme)
        {
            ToolStripRenderer renderer = new ToolStripProfessionalRenderer(new OsdContextMenuColorTable(theme));
            Font menuFont = Font;
            customLabelContextMenu.BackColor = theme.SurfaceBack;
            customLabelContextMenu.ForeColor = theme.Text;
            customLabelContextMenu.Font = menuFont;
            customLabelContextMenu.Renderer = renderer;

            foreach (ToolStripItem item in customLabelContextMenu.Items)
                ApplyContextMenuItemTheme(item, theme, renderer, menuFont);
        }

        private static void ApplyContextMenuItemTheme(
            ToolStripItem item,
            ThemePalette theme,
            ToolStripRenderer renderer,
            Font menuFont)
        {
            item.BackColor = theme.SurfaceBack;
            item.ForeColor = theme.Text;
            item.Font = menuFont;
            if (!(item is ToolStripMenuItem menuItem))
                return;

            menuItem.DropDown.BackColor = theme.SurfaceBack;
            menuItem.DropDown.ForeColor = theme.Text;
            menuItem.DropDown.Font = menuFont;
            menuItem.DropDown.Renderer = renderer;
            foreach (ToolStripItem child in menuItem.DropDownItems)
                ApplyContextMenuItemTheme(child, theme, renderer, menuFont);
        }

        private void ApplyTheme()
        {
            bool light = _mainForm != null && _mainForm.IsLightModeEnabled;
            ThemePalette theme = ThemePalette.Create(light);

            _navigationBack = theme.NavBack;
            _navigationSelected = theme.NavSelected;
            _navigationText = theme.TitleText;
            ApplyCustomLabelContextMenuTheme(theme);

            BackColor = theme.WindowBack;
            ForeColor = theme.Text;
            outerBorder.BackColor = theme.Border;
            mainPanel.BackColor = theme.WindowBack;
            titleBar.BackColor = theme.BarBack;
            bottomBar.BackColor = theme.BarBack;
            contentPanel.BackColor = theme.WindowBack;
            pageTabs.BackColor = theme.WindowBack;
            pageTabs.ForeColor = theme.Text;
            metricsPage.BackColor = theme.WindowBack;
            appearancePage.BackColor = theme.WindowBack;
            layoutPage.BackColor = theme.WindowBack;
            metricsPage.UseVisualStyleBackColor = false;
            appearancePage.UseVisualStyleBackColor = false;
            layoutPage.UseVisualStyleBackColor = false;
            metricsPageLayout.BackColor = theme.WindowBack;
            layoutPageLayout.BackColor = theme.WindowBack;
            navigationPanel.BackColor = _navigationBack;
            navigationTitle.ForeColor = theme.MutedText;
            subtitleLabel.ForeColor = theme.MutedText;
            accentLine.BackColor = theme.Accent;

            if (resizeGrip != null && !resizeGrip.IsDisposed)
            {
                resizeGrip.LightTheme = light;
                resizeGrip.BackColor = theme.BarBack;
            }

            foreach (Panel card in new[] { metricsCard, labelsCard, appearanceCard, layoutCard, spacingCard, hotkeyCard })
            {
                card.BackColor = theme.SurfaceBack;

                foreach (Label label in card.Controls.OfType<Label>())
                    label.ForeColor = theme.Text;
                foreach (CheckBox checkBox in card.Controls.OfType<CheckBox>())
                {
                    checkBox.BackColor = theme.SurfaceBack;
                    checkBox.ForeColor = theme.Text;
                    checkBox.UseVisualStyleBackColor = false;
                }
            }

            customLabelsLayout.BackColor = theme.SurfaceBack;
            customLabelsInfoIconLabel.BackColor = theme.SurfaceBack;
            customLabelsInfoIconLabel.ForeColor = theme.MutedText;
            spacingLayout.BackColor = theme.SurfaceBack;
            foreach (Label label in new[]
            {
                customCpuLabelCaption,
                customGpuLabelCaption,
                customCpuUsageLabelCaption,
                customGpuUsageLabelCaption,
                customRamLabelCaption,
                customVramLabelCaption,
                customFpsLabelCaption,
                customTimeLabelCaption,
                spacingHeader,
                rowsGapHeader,
                columnsGapHeader,
                fpsRefreshIntervalLabel,
                fpsLabelValueSpacingLabel,
                externalSpacingLabel,
                cornerRoundnessLabel
            })
            {
                label.BackColor = theme.SurfaceBack;
                label.ForeColor = theme.Text;
            }

            fontColorsLayout.BackColor = theme.SurfaceBack;
            foreach (Label label in fontColorsLayout.Controls.OfType<Label>())
            {
                label.BackColor = theme.SurfaceBack;
                label.ForeColor = theme.Text;
            }

            foreach (Control input in new Control[]
            {
                positionValue, fontFamilyValue,
                customCpuLabel, customGpuLabel, customCpuUsageLabel, customGpuUsageLabel,
                customRamLabel, customVramLabel, customFpsLabel, customTimeLabel, labelValueSpacing, rowsSpacing, columnsSpacing,
                columnsValue, fontSizeValue, screenMarginValue, fpsRefreshIntervalValue,
                metricGroupingMode,
                fpsLabelValueSpacing, externalSpacingValue, cornerRoundnessValue, itemOrder, hotkeyValue
            })
            {
                input.BackColor = theme.InputBack;
                input.ForeColor = theme.Text;
            }

            opacityValue.BackColor = theme.SurfaceBack;
            backgroundOpacityValue.BackColor = theme.SurfaceBack;

            if (!_hasCustomBackgroundColor)
                backgroundColor.BackColor = OsdColorHelper.GetDefaultBackground(light);

            foreach (Button button in new[] { orderUp, orderDown })
            {
                button.BackColor = theme.SurfaceBack;
                button.ForeColor = theme.Text;
                button.FlatAppearance.BorderColor = theme.Border;
                button.FlatAppearance.MouseOverBackColor = theme.NavigationHover;
                button.FlatAppearance.MouseDownBackColor = theme.NavigationDown;
            }

            foreach (Button button in new[] { metricsNavButton, appearanceNavButton, layoutNavButton })
            {
                button.ForeColor = _navigationText;
                button.FlatAppearance.MouseOverBackColor = theme.NavigationHover;
                button.FlatAppearance.MouseDownBackColor = theme.NavigationDown;
            }

            foreach (Button button in new[] { backgroundColor, cpuFontColor, gpuFontColor, ramFontColor, vramFontColor, fpsFontColor })
            {
                button.ForeColor = GetReadableForeground(button.BackColor);
                button.FlatAppearance.BorderColor = theme.Border;
            }

            saveBtn.BackColor = theme.Accent;
            saveBtn.ForeColor = theme.AccentButtonText;
            saveBtn.FlatAppearance.BorderColor = theme.Border;
            saveBtn.FlatAppearance.MouseOverBackColor = light
                ? Color.FromArgb(29, 78, 216)
                : Color.FromArgb(0, 102, 184);
            saveBtn.FlatAppearance.MouseDownBackColor = light
                ? Color.FromArgb(30, 64, 175)
                : Color.FromArgb(0, 84, 153);
            exitBtn.BackColor = theme.BarBack;
            exitBtn.ForeColor = theme.TitleText;
            exitBtn.FlatAppearance.BorderColor = theme.BarBack;
            exitBtn.FlatAppearance.MouseOverBackColor = Color.FromArgb(220, 38, 38);
            exitBtn.FlatAppearance.MouseDownBackColor = Color.DarkRed;
            ShowPage(_currentPage);
        }

        private void PageTabs_DrawItem(object sender, DrawItemEventArgs e)
        {
            // The sidebar is the visible page selector.  The tab strip remains a
            // one-pixel, themed surface so the standard TabControl has no chrome
            // that can introduce a light border around the page content.
            using (var brush = new SolidBrush(pageTabs.BackColor))
                e.Graphics.FillRectangle(brush, e.Bounds);
        }

        private static Color GetReadableForeground(Color background)
        {
            double luminance = (0.299 * background.R) + (0.587 * background.G) + (0.114 * background.B);
            return luminance >= 150 ? Color.Black : Color.White;
        }

        private void DragSurface_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
                return;

            _dragging = true;
            _dragCursorPoint = Cursor.Position;
            _dragFormPoint = Location;
        }

        private void DragSurface_MouseMove(object sender, MouseEventArgs e)
        {
            if (!_dragging)
                return;

            Point difference = Point.Subtract(Cursor.Position, new Size(_dragCursorPoint));
            Location = Point.Add(_dragFormPoint, new Size(difference));
        }

        private void DragSurface_MouseUp(object sender, MouseEventArgs e)
        {
            _dragging = false;
        }

        private void ExitBtn_Click(object sender, EventArgs e)
        {
            Close();
        }

        private sealed class OsdContextMenuColorTable : ProfessionalColorTable
        {
            private readonly ThemePalette _theme;

            internal OsdContextMenuColorTable(ThemePalette theme)
            {
                _theme = theme;
            }

            public override Color ToolStripDropDownBackground => _theme.SurfaceBack;
            public override Color MenuItemSelected => _theme.NavSelected;
            public override Color MenuItemSelectedGradientBegin => _theme.NavSelected;
            public override Color MenuItemSelectedGradientEnd => _theme.NavSelected;
            public override Color MenuItemPressedGradientBegin => _theme.NavSelected;
            public override Color MenuItemPressedGradientMiddle => _theme.NavSelected;
            public override Color MenuItemPressedGradientEnd => _theme.NavSelected;
            public override Color MenuItemBorder => _theme.Border;
            public override Color ToolStripBorder => _theme.Border;
            public override Color ImageMarginGradientBegin => _theme.SurfaceBack;
            public override Color ImageMarginGradientMiddle => _theme.SurfaceBack;
            public override Color ImageMarginGradientEnd => _theme.SurfaceBack;
            public override Color SeparatorDark => _theme.Border;
            public override Color SeparatorLight => _theme.SurfaceBack;
            public override Color CheckBackground => _theme.NavSelected;
            public override Color CheckSelectedBackground => _theme.NavSelected;
            public override Color CheckPressedBackground => _theme.NavSelected;
        }

        [DllImport("user32.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
        private static extern IntPtr SendMessageW(IntPtr windowHandle, int message, IntPtr wParam, IntPtr lParam);

        private sealed class OsdItemOption
        {
            internal OsdItemOption(
                OsdItemKind kind,
                OsdMetricGroupingMode groupingMode = OsdMetricGroupingMode.Disabled)
            {
                Kind = kind;
                GroupingMode = groupingMode;
            }

            internal OsdItemKind Kind { get; }
            internal OsdMetricGroupingMode GroupingMode { get; }

            public override string ToString()
            {
                if (GroupingMode != OsdMetricGroupingMode.Disabled)
                {
                    string group = Kind == OsdItemKind.CpuTemperature ? "CPU" : "GPU";
                    return GroupingMode == OsdMetricGroupingMode.LoadTempsAndUsedMemory
                        ? group + " Temperature + Load + Used Memory"
                        : group + " Temperature + Usage";
                }

                return OsdItemOrderHelper.GetDisplayName(Kind);
            }
        }

        private sealed class OsdNumericOption
        {
            internal OsdNumericOption(decimal value)
            {
                Value = value;
            }

            internal decimal Value { get; }

            public override string ToString()
            {
                return Value.ToString("0.##", CultureInfo.CurrentCulture);
            }
        }

        private enum OsdSettingsPage
        {
            Metrics,
            Appearance,
            Layout
        }
    }
}
