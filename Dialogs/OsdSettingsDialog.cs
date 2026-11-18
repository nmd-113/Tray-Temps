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
        private bool _updatingColumnOptions;
        private bool _hasCustomBackgroundColor;
        private bool _appearancePopulated;
        private bool _customLabelMenuInitialized;
        private List<OsdItemKind> _separateItemOrder;
        private readonly Dictionary<string, ToolStripMenuItem> _iconCategoryMenus =
            new Dictionary<string, ToolStripMenuItem>(StringComparer.Ordinal);
        private readonly List<string> _iconCategoryOrder = new List<string>();
        private TextBox _activeCustomLabelTextBox;
        private OsdSettingsPage _currentPage = OsdSettingsPage.Presets;
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

            InitializeAppearanceFonts();
            EmbeddedFonts.ApplyTo(this);
            appearanceGrid.DefaultCellStyle.Font = _appearanceBodyFont;
            appearanceGrid.ColumnHeadersDefaultCellStyle.Font = _appearanceHeaderFont;
            _mainForm = mainForm;
            _configuration = configuration?.Clone() ?? OsdConfiguration.CreateAppDefault();
            _metricDraft = _configuration.Clone();
            _metricDraft.LabelOverrides = OsdMetricDefinitions.CreateOverrides(_configuration);
            OsdMetricDefinitions.Migrate(_metricDraft);

            PopulateOptions();
            presetList.Items.AddRange(OsdPresets.Names);
            presetList.SelectedIndex = 0;
            LoadConfiguration();
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
            if (HandleMetricEditorKey(keyData)) return true;
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
            PopulateNumericOptions(screenMarginValue, 0m, 100m, 1m);

            PopulateNumericOptions(rowsSpacing, 0m, 100m, 1m);
            PopulateNumericOptions(columnsSpacing, 0m, 100m, 1m);
            PopulateNumericOptions(groupedValuesGapPercent, 100m, 200m, 10m);
            PopulateNumericOptions(frameTimeGraphWidthValue, OsdConfiguration.MinFrameTimeGraphWidth,
                OsdConfiguration.MaxFrameTimeGraphWidth, 10m);
            PopulateNumericOptions(frameTimeGraphHeightValue, OsdConfiguration.MinFrameTimeGraphHeight,
                OsdConfiguration.MaxFrameTimeGraphHeight, 2m);
            PopulateNumericOptions(columnsValue, 1m, 1m, 1m);
            PopulateNumericOptions(fpsRefreshIntervalValue, 0.25m, 10m, 0.25m);
            PopulateNumericOptions(fpsLabelValueSpacing, 0m, 100m, 1m);
            PopulateNumericOptions(externalSpacingValue, 0m, 100m, 1m);
            PopulateNumericOptions(cornerRoundnessValue, 0m, 50m, 1m);
            _separateItemOrder = OsdItemOrderHelper.Parse(_configuration.ItemOrder);
        }

        private void LoadConfiguration(OsdConfiguration source = null)
        {
            source = source ?? _configuration;
            positionValue.SelectedIndex = ValueHelper.ClampInt((int)source.Position, 0, 8);
            opacityValue.Value = 100 - ValueHelper.ClampInt(source.OpacityPercent, 20, 100);
            SelectNumericOption(screenMarginValue, ValueHelper.ClampInt(source.ScreenMargin, 0, 100));
            SelectNumericOption(externalSpacingValue, ValueHelper.ClampInt(source.ExternalSpacing, 0, 100));
            SelectNumericOption(groupedValuesGapPercent, ValueHelper.ClampInt(source.GroupedValuesGapPercent, 100, 200));
            SelectNumericOption(frameTimeGraphWidthValue, ValueHelper.ClampInt(source.FrameTimeGraphWidth,
                OsdConfiguration.MinFrameTimeGraphWidth, OsdConfiguration.MaxFrameTimeGraphWidth));
            SelectNumericOption(frameTimeGraphHeightValue, ValueHelper.ClampInt(source.FrameTimeGraphHeight,
                OsdConfiguration.MinFrameTimeGraphHeight, OsdConfiguration.MaxFrameTimeGraphHeight));
            SelectNumericOption(cornerRoundnessValue, ValueHelper.ClampInt(source.BackgroundCornerRoundness, 0, 50));
            SelectNumericOption(columnsValue, ValueHelper.ClampInt(source.Columns, 1, OsdMetricDefinitions.Keys.Length));
            SelectNumericOption(
                fpsRefreshIntervalValue,
                ValueHelper.ClampDecimal(
                    source.FpsRefreshIntervalSeconds ?? 0.5m,
                    0.25m,
                    10m));
            _hasCustomBackgroundColor = source.BackgroundColor.HasValue;
            backgroundColor.BackColor = _hasCustomBackgroundColor
                ? OsdColorHelper.GetOpaqueColor(source.BackgroundColor.Value)
                : OsdColorHelper.GetDefaultBackground(_mainForm != null && _mainForm.IsLightModeEnabled);
            backgroundOpacityValue.Value = source.TransparentBackground ? 100 : 100 - ValueHelper.ClampInt(source.BackgroundOpacityPercent, 0, 100);
            SelectNumericOption(
                rowsSpacing,
                ValueHelper.ClampInt(source.RowsSpacing, 0, 100));
            SelectNumericOption(
                columnsSpacing,
                ValueHelper.ClampInt(source.ColumnsSpacing, 0, 100));
            _hotkeyModifiers = (OsdHotkeyModifiers)source.HotkeyModifiers;
            _hotkeyKey = (Keys)source.HotkeyKey;
            if (!OsdHotkeyHelper.IsValid(_hotkeyModifiers, _hotkeyKey))
            {
                _hotkeyModifiers = OsdHotkeyHelper.DefaultModifiers;
                _hotkeyKey = OsdHotkeyHelper.DefaultKey;
            }
            hotkeyEnabled.Checked = source.HotkeyEnabled;

            PopulateMetricList();
            RebuildItemOrder();
            UpdateHotkeyDisplay();
            UpdateHardwareVisibility();
            RefreshMetricRows();
            UpdateOpacityLabel();
            UpdateBackgroundOpacityLabel();

        }

        private void SaveBtn_Click(object sender, EventArgs e)
        {
            EndMetricLabelEdit(true);
            if (!TryCommitAppearance() || !ValidateMetricSelection()) return;

            if (hotkeyEnabled.Checked &&
                (_mainForm == null || !_mainForm.CanRegisterOsdHotkey(_hotkeyModifiers, _hotkeyKey)))
            {
                ShowPage(OsdSettingsPage.Layout);
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

        private bool ValidateMetricSelection()
        {
            if (!_metricDraft.ShowCpu && !_metricDraft.ShowGpu &&
                !_metricDraft.ShowCpuUsage && !_metricDraft.ShowGpuUsage &&
                !_metricDraft.ShowRamUsage && !_metricDraft.ShowVramUsage && !OsdMetricDefinitions.NeedsFrames(_metricDraft) && !_metricDraft.ShowCurrentTime)
            {
                ShowPage(OsdSettingsPage.Metrics);
                MessageBox.Show(
                    this,
                    "Select at least one metric to display in the OSD.",
                    "OSD Settings",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }

        private OsdConfiguration CreateConfigurationFromControls()
        {
            var result = _metricDraft.Clone();
            ApplyCellEdit(result);
            result.Enabled = _configuration.Enabled;
            result.Position = (OsdPosition)Math.Max(0, positionValue.SelectedIndex);
            // Preview the same resolved color shown by the button, including a
            // legacy configuration that relies on the current theme's default.
            result.BackgroundColor = backgroundColor.BackColor.ToArgb();
            result.BackgroundOpacityPercent = 100 - backgroundOpacityValue.Value;
            result.TransparentBackground = result.BackgroundOpacityPercent == 0;
            result.OpacityPercent = 100 - opacityValue.Value;
            result.BackgroundCornerRoundness = (int)GetNumericOptionValue(cornerRoundnessValue, 0);
            result.FpsRefreshIntervalSeconds = GetNumericOptionValue(fpsRefreshIntervalValue, 0.5m);


            result.RowsSpacing = (int)GetNumericOptionValue(rowsSpacing, 5);
            result.ColumnsSpacing = (int)GetNumericOptionValue(columnsSpacing, 20);
            result.GroupedValuesGapPercent = (int)GetNumericOptionValue(groupedValuesGapPercent, 100);
            result.FrameTimeGraphWidth = (int)GetNumericOptionValue(frameTimeGraphWidthValue, OsdConfiguration.DefaultFrameTimeGraphWidth);
            result.FrameTimeGraphHeight = (int)GetNumericOptionValue(frameTimeGraphHeightValue, OsdConfiguration.DefaultFrameTimeGraphHeight);
            result.ScreenMargin = (int)GetNumericOptionValue(screenMarginValue, 10);
            result.ExternalSpacing = (int)GetNumericOptionValue(externalSpacingValue, 0);
            result.Columns = (int)GetNumericOptionValue(columnsValue, 1);
            result.ItemOrder = OsdItemOrderHelper.Serialize(_separateItemOrder);
            result.HotkeyEnabled = hotkeyEnabled.Checked;
            result.HotkeyModifiers = (int)_hotkeyModifiers; result.HotkeyKey = (int)_hotkeyKey;
            return result;
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

        private void GroupedValuesGapPercent_Format(object sender, ListControlConvertEventArgs e)
        {
            if (e.ListItem is OsdNumericOption option)
                e.Value = option.ToString() + "%";
        }

        private void ColumnsValue_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateSpacingControlAvailability();
            if (!_updatingColumnOptions)
                PreviewVisualSettings();
        }

        private void UpdateColumnOptions()
        {
            int separateRows = 0;
            var groupedRows = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (OsdItemKind kind in _separateItemOrder)
            {
                string key = kind.ToString();
                if (!OsdMetricDefinitions.IsVisible(_metricDraft, key))
                    continue;

                string group = OsdMetricDefinitions.Group(_metricDraft, key);
                if (group.Length == 0)
                    separateRows++;
                else
                    groupedRows.Add(group);
            }

            int maximum = Math.Max(1, separateRows + groupedRows.Count);
            int selected = (int)GetNumericOptionValue(columnsValue, _configuration.Columns);
            selected = Math.Max(1, Math.Min(selected, maximum));

            _updatingColumnOptions = true;
            columnsValue.BeginUpdate();
            try
            {
                columnsValue.Items.Clear();
                for (int value = 1; value <= maximum; value++)
                    columnsValue.Items.Add(new OsdNumericOption(value));
                columnsValue.SelectedIndex = selected - 1;
            }
            finally
            {
                columnsValue.EndUpdate();
                _updatingColumnOptions = false;
            }

            UpdateSpacingControlAvailability();
        }

        private void UpdateSpacingControlAvailability()
        {
            int columns = (int)GetNumericOptionValue(columnsValue, 1);
            bool atMaximum = columnsValue.Items.Count > 0 && columns >= columnsValue.Items.Count;
            rowsGapHeader.Enabled = rowsSpacing.Enabled = !atMaximum;
            columnsGapHeader.Enabled = columnsSpacing.Enabled = columns > 1;
        }

        private void InitializeCustomLabelContextMenu()
        {
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

        }

        private void LabelCommand_Click(object sender, EventArgs e)
        {
            if (_activeCustomLabelTextBox == null) return;
            switch (((ToolStripItem)sender).Name)
            {
                case "labelUndo": if (CanEditActiveCustomLabel() && _activeCustomLabelTextBox.CanUndo) _activeCustomLabelTextBox.Undo(); break;
                case "labelCut": if (CanEditActiveCustomLabel()) _activeCustomLabelTextBox.Cut(); break;
                case "labelCopy": _activeCustomLabelTextBox.Copy(); break;
                case "labelPaste": if (CanEditActiveCustomLabel()) _activeCustomLabelTextBox.Paste(); break;
                case "labelDelete": if (CanEditActiveCustomLabel()) ReplaceCustomLabelSelection(_activeCustomLabelTextBox, ""); break;
                case "labelSelectAll": _activeCustomLabelTextBox.SelectAll(); break;
            }
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
            insertIconMenu.DropDownItems.Add(categoryMenu);
        }

        private void CustomLabelContextMenu_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _activeCustomLabelTextBox = customLabelContextMenu.SourceControl as TextBox;
            if (_activeCustomLabelTextBox == null)
            {
                e.Cancel = true;
                return;
            }

            if (!_customLabelMenuInitialized)
            {
                InitializeCustomLabelContextMenu();
                _customLabelMenuInitialized = true;
                ApplyCustomLabelContextMenuTheme(ThemePalette.Create(_mainForm != null && _mainForm.IsLightModeEnabled));
            }

            TextBox textBox = _activeCustomLabelTextBox;
            bool hasSelection = textBox.SelectionLength > 0;
            bool editable = textBox.Enabled && !textBox.ReadOnly;
            labelUndo.Enabled = editable && textBox.CanUndo;
            labelCut.Enabled = labelDelete.Enabled = editable && hasSelection;
            labelCopy.Enabled = hasSelection;
            labelPaste.Enabled = editable;
            labelSelectAll.Enabled = textBox.TextLength > 0;
            resetLabelMenuItem.Visible = labelResetSeparator.Visible = textBox == metricLabelEditor && _editingColumn == 1;
            ReorderIconCategories(GetPrioritizedIconCategories(_editingMetricKey));
        }

        private void ReorderIconCategories(IEnumerable<string> prioritizedCategories)
        {
            insertIconMenu.DropDownItems.Clear();
            var orderedKeys = new List<string>(prioritizedCategories);
            foreach (string key in _iconCategoryOrder)
            {
                if (!orderedKeys.Contains(key))
                    orderedKeys.Add(key);
            }

            foreach (string key in orderedKeys)
                insertIconMenu.DropDownItems.Add(_iconCategoryMenus[key]);

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

        private static IEnumerable<string> GetPrioritizedIconCategories(string metricKey)
        {
            switch (metricKey)
            {
                case "CpuTemperature": return new[] { "cpu", "temperature" };
                case "GpuTemperature": return new[] { "gpu", "temperature" };
                case "CpuUsage": return new[] { "cpu", "fps" };
                case "GpuUsage": return new[] { "gpu", "fps" };
                case "RamUsage": return new[] { "ram" };
                case "VramUsage": return new[] { "vram" };
                case "FpsOnePercentLow":
                case "Fps": return new[] { "fps" };
                case "CurrentTime": return new[] { "clock" };
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

        private static void PopulateNumericOptions(
            ComboBox input,
            decimal minimum,
            decimal maximum,
            decimal increment)
        {
            input.BeginUpdate();
            try
            {
                for (decimal value = minimum; value <= maximum; value += increment)
                    input.Items.Add(new OsdNumericOption(value));
            }
            finally { input.EndUpdate(); }
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

        private void HotkeyValue_KeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;
            e.SuppressKeyPress = true;

            TryCaptureHotkey(e.KeyData);
        }

        private bool TryCaptureHotkey(Keys keyData)
        {
            if (!OsdHotkeyHelper.TryParse(keyData, out OsdHotkeyModifiers modifiers, out Keys key))
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

            return TryCaptureHotkey(OsdHotkeyHelper.ToKeyData(modifiers, key));
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

        private void BackgroundColor_Click(object sender, EventArgs e)
        {
            colorDialog.Color = backgroundColor.BackColor;
            if (colorDialog.ShowDialog(this) == DialogResult.OK)
            {
                backgroundColor.BackColor = colorDialog.Color;
                backgroundColor.ForeColor = GetReadableForeground(backgroundColor.BackColor);
                _hasCustomBackgroundColor = true;
                PreviewVisualSettings();
            }
        }

        private void MainForm_ThemeChanged(object sender, EventArgs e)
        {
            ApplyTheme();
            PreviewVisualSettings();
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

            if (page == OsdSettingsPage.Appearance && !_appearancePopulated)
            {
                PopulateAppearance();
                _appearancePopulated = true;
            }

            TabPage selectedPage = page == OsdSettingsPage.Metrics
                ? metricsPage
                : page == OsdSettingsPage.Appearance ? appearancePage : page == OsdSettingsPage.Layout ? layoutPage : presetsPage;

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

            presetsNavButton.BackColor = page == OsdSettingsPage.Presets ? _navigationSelected : _navigationBack;
            presetsNavButton.ForeColor = _navigationText;
            metricsNavButton.ForeColor = _navigationText;
            appearanceNavButton.ForeColor = _navigationText;
            layoutNavButton.ForeColor = _navigationText;

            Button selectedButton = page == OsdSettingsPage.Metrics
                ? metricsNavButton
                : page == OsdSettingsPage.Appearance ? appearanceNavButton : page == OsdSettingsPage.Layout ? layoutNavButton : presetsNavButton;
            if (accentLine.Parent != selectedButton)
                selectedButton.Controls.Add(accentLine);
            accentLine.BringToFront();
        }

        private void ApplyCustomLabelContextMenuTheme(ThemePalette theme)
        {
            ToolStripRenderer renderer = new ToolStripProfessionalRenderer(new OsdContextMenuColorTable(theme));
            Font menuFont = Font;
            foreach (var menu in new[] { customLabelContextMenu, metricContextMenu })
            {
                if (menu == null) continue;
                menu.BackColor = theme.SurfaceBack;
                menu.ForeColor = theme.Text;
                menu.Font = menuFont;
                menu.Renderer = renderer;
                foreach (ToolStripItem item in menu.Items)
                    ApplyContextMenuItemTheme(item, theme, renderer, menuFont);
            }
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

            if (menuItem.DropDownItems.Count == 0)
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
            presetsPage.BackColor = theme.WindowBack;
            presetsLayout.BackColor = theme.SurfaceBack;
            presetsHelp.ForeColor = theme.Text;
            presetsTitle.ForeColor = theme.TitleText;
            metricsPage.UseVisualStyleBackColor = false;
            appearancePage.UseVisualStyleBackColor = false;
            layoutPage.UseVisualStyleBackColor = false;
            presetsPage.UseVisualStyleBackColor = false;
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

            foreach (Panel card in new[] { metricsCard, appearanceCard, layoutCard, spacingCard, hotkeyCard })
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
            metricsListHeader.BackColor = theme.BarBack;
            groupFieldsLayout.BackColor = theme.SurfaceBack;
            metricNameHeader.BackColor = theme.BarBack; metricNameHeader.ForeColor = theme.TitleText;
            metricLabelHeader.BackColor = theme.BarBack; metricLabelHeader.ForeColor = theme.TitleText;
            metricLabelsInfoIcon.ForeColor = theme.MutedText;
            metricGroupHeader.BackColor = theme.BarBack; metricGroupHeader.ForeColor = theme.TitleText;
            metricListHint.ForeColor = theme.MutedText;
            _disabledMetricColor = theme.MutedText;
            spacingLayout.BackColor = theme.SurfaceBack;
            frameTimeGraphSizeLayout.BackColor = theme.SurfaceBack;
            foreach (Label label in new[]
            {
                rowsGapHeader,
                columnsGapHeader,
                groupedValuesGapLabel,
                frameTimeGraphSizeLabel,
                frameTimeGraphSizeSeparator,
                fpsRefreshIntervalLabel,
                fpsLabelValueSpacingLabel,
                externalSpacingLabel,
                cornerRoundnessLabel
            })
            {
                label.BackColor = theme.SurfaceBack;
                label.ForeColor = theme.Text;
            }

            foreach (Control input in new Control[]
            {
                positionValue, itemOrder, presetList,
                metricsList, metricLabelEditor, rowsSpacing, columnsSpacing, groupedValuesGapPercent,
                columnsValue, screenMarginValue, fpsRefreshIntervalValue,
                frameTimeGraphWidthValue, frameTimeGraphHeightValue,

                fpsLabelValueSpacing, externalSpacingValue, cornerRoundnessValue, hotkeyValue
            })
            {
                input.BackColor = theme.InputBack;
                input.ForeColor = theme.Text;
            }

            RefreshMetricRows();
            RefreshOrderColors();
            opacityValue.BackColor = theme.SurfaceBack;
            backgroundOpacityValue.BackColor = theme.SurfaceBack;

            if (!_hasCustomBackgroundColor)
                backgroundColor.BackColor = OsdColorHelper.GetDefaultBackground(light);

            foreach (Button button in new[] { orderUp, orderDown, loadEmbeddedPreset, loadPresetFile, savePresetFile })
            {
                button.BackColor = theme.SurfaceBack;
                button.ForeColor = theme.Text;
                button.FlatAppearance.BorderColor = theme.Border;
                button.FlatAppearance.MouseOverBackColor = theme.NavigationHover;
                button.FlatAppearance.MouseDownBackColor = theme.NavigationDown;
            }

            foreach (Button button in new[] { metricsNavButton, appearanceNavButton, layoutNavButton, presetsNavButton })
            {
                button.ForeColor = _navigationText;
                button.FlatAppearance.MouseOverBackColor = theme.NavigationHover;
                button.FlatAppearance.MouseDownBackColor = theme.NavigationDown;
            }

            ApplyAppearanceTheme(theme);
            backgroundColor.ForeColor = GetReadableForeground(backgroundColor.BackColor);
            backgroundColor.FlatAppearance.BorderColor = theme.Border;

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
            Layout,
            Presets
        }
    }
}
