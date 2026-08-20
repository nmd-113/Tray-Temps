using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace TrayTemps
{
    public partial class OsdSettingsDialog : Form
    {
        private const int CsDropShadow = 0x00020000;
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

        public OsdSettingsDialog()
        {
            InitializeComponent();
        }

        internal OsdSettingsDialog(MainForm mainForm, OsdConfiguration configuration)
            : this()
        {
            EmbeddedFonts.ApplyTo(this);
            _mainForm = mainForm;
            _configuration = configuration?.Clone() ?? new OsdConfiguration();

            PopulateOptions();
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
                parameters.ClassStyle |= CsDropShadow;
                return parameters;
            }
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            WindowCornerHelper.ApplyRoundedCorners(Handle);
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

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
            PopulateNumericOptions(fontSizeValue, 8m, 48m, 0.5m);
            PopulateNumericOptions(screenMarginValue, 0m, 100m, 1m);
            PopulateNumericOptions(labelValueSpacing, 0m, 100m, 1m);
            PopulateNumericOptions(rowsSpacing, 0m, 100m, 1m);
            PopulateNumericOptions(columnsSpacing, 0m, 100m, 1m);
            PopulateNumericOptions(columnsValue, 1m, 4m, 1m);

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
            SelectNumericOption(columnsValue, ValueHelper.ClampInt(_configuration.Columns, 1, 4));
            showCpuUsage.Checked = _configuration.ShowCpuUsage;
            showGpuUsage.Checked = _configuration.ShowGpuUsage;
            showRamUsage.Checked = _configuration.ShowRamUsage;
            showVramUsage.Checked = _configuration.ShowVramUsage;
            showFps.Checked = _configuration.ShowFps;
            combineTemperatureAndUsage.Checked = _configuration.CombineTemperatureAndUsage;
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
                ValueHelper.ClampDecimal((decimal)_configuration.FontSize, 8m, 48m));
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
            if (!showCpu.Checked && !showGpu.Checked && !showRamUsage.Checked &&
                !showVramUsage.Checked && !showFps.Checked)
            {
                MessageBox.Show(
                    this,
                    "Select at least one CPU, GPU, RAM, VRAM, or FPS item to display in the OSD.",
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
                TransparentBackground = backgroundOpacityValue.Value == 0,
                OpacityPercent = opacityValue.Value,
                ShowCpuUsage = showCpuUsage.Checked,
                ShowGpuUsage = showGpuUsage.Checked,
                ShowRamUsage = showRamUsage.Checked,
                ShowVramUsage = showVramUsage.Checked,
                ShowFps = showFps.Checked,
                CombineTemperatureAndUsage = combineTemperatureAndUsage.Checked,
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
                LabelValueSpacing = (int)GetNumericOptionValue(labelValueSpacing, 15m),
                RowsSpacing = (int)GetNumericOptionValue(rowsSpacing, 5m),
                ColumnsSpacing = (int)GetNumericOptionValue(columnsSpacing, 20m),
                ScreenMargin = (int)GetNumericOptionValue(screenMarginValue, 12m),
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

        private void SelectFontFamily(string familyName)
        {
            string requested = string.IsNullOrWhiteSpace(familyName)
                ? OsdFontHelper.DefaultFamily
                : familyName;

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
            return string.IsNullOrWhiteSpace(text) ? fallback : text;
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
            bool cpuUsageVisible = cpuTemperatureVisible && showCpuUsage.Checked && !combineTemperatureAndUsage.Checked;
            bool gpuUsageVisible = gpuTemperatureVisible && showGpuUsage.Checked && !combineTemperatureAndUsage.Checked;

            customCpuLabel.Enabled = enabled && cpuTemperatureVisible;
            customGpuLabel.Enabled = enabled && gpuTemperatureVisible;
            customCpuUsageLabel.Enabled = enabled && cpuUsageVisible;
            customGpuUsageLabel.Enabled = enabled && gpuUsageVisible;
            customRamLabel.Enabled = enabled && showRamUsage.Checked;
            customVramLabel.Enabled = enabled && showVramUsage.Checked;
            customFpsLabel.Enabled = enabled && showFps.Checked;

        }

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
            bool selectedCombined = selectedItem != null && selectedItem.IsCombinedTemperatureUsage;
            var updatedItems = new List<OsdItemOption>();
            bool cpuAdded = false;
            bool gpuAdded = false;
            bool combineCpu = combineTemperatureAndUsage.Checked && showCpu.Checked && showCpuUsage.Checked;
            bool combineGpu = combineTemperatureAndUsage.Checked && showGpu.Checked && showGpuUsage.Checked;

            foreach (OsdItemKind item in _separateItemOrder)
            {
                if (combineCpu && IsCpuTemperatureOrUsage(item))
                {
                    if (!cpuAdded)
                    {
                        updatedItems.Add(new OsdItemOption(
                            OsdItemKind.CpuTemperature,
                            isCombinedTemperatureUsage: true));
                        cpuAdded = true;
                    }
                    continue;
                }

                if (combineGpu && IsGpuTemperatureOrUsage(item))
                {
                    if (!gpuAdded)
                    {
                        updatedItems.Add(new OsdItemOption(
                            OsdItemKind.GpuTemperature,
                            isCombinedTemperatureUsage: true));
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
                SelectUpdatedOrderItem(selectedKind, selectedCombined);
            }
            finally
            {
                itemOrder.EndUpdate();
            }
        }

        private static bool IsCpuTemperatureOrUsage(OsdItemKind item)
        {
            return item == OsdItemKind.CpuTemperature || item == OsdItemKind.CpuUsage;
        }

        private static bool IsGpuTemperatureOrUsage(OsdItemKind item)
        {
            return item == OsdItemKind.GpuTemperature || item == OsdItemKind.GpuUsage;
        }

        private bool ItemOrderMatches(
            IList<OsdItemOption> updatedItems)
        {
            if (itemOrder.Items.Count != updatedItems.Count)
                return false;

            for (int index = 0; index < updatedItems.Count; index++)
            {
                var current = itemOrder.Items[index] as OsdItemOption;
                OsdItemOption updated = updatedItems[index];
                if (current == null || current.Kind != updated.Kind ||
                    current.IsCombinedTemperatureUsage != updated.IsCombinedTemperatureUsage)
                {
                    return false;
                }
            }

            return true;
        }

        private void SelectUpdatedOrderItem(OsdItemKind? selectedKind, bool selectedCombined)
        {
            if (!selectedKind.HasValue || itemOrder.Items.Count == 0)
                return;

            for (int index = 0; index < itemOrder.Items.Count; index++)
            {
                var option = itemOrder.Items[index] as OsdItemOption;
                if (option == null)
                    continue;

                bool sameMetricGroup = option.Kind == selectedKind.Value ||
                    (IsCpuTemperatureOrUsage(option.Kind) && IsCpuTemperatureOrUsage(selectedKind.Value)) ||
                    (IsGpuTemperatureOrUsage(option.Kind) && IsGpuTemperatureOrUsage(selectedKind.Value));
                if (sameMetricGroup &&
                    (option.IsCombinedTemperatureUsage == selectedCombined ||
                     option.IsCombinedTemperatureUsage == combineTemperatureAndUsage.Checked))
                {
                    itemOrder.SelectedIndex = index;
                    return;
                }
            }

            itemOrder.SelectedIndex = 0;
        }

        private void UpdateHardwareVisibility()
        {
            showCpuUsage.Enabled = showCpu.Checked;
            showGpuUsage.Enabled = showGpu.Checked;
            cpuFontColor.Enabled = showCpu.Checked;
            gpuFontColor.Enabled = showGpu.Checked;
            ramFontColor.Enabled = showRamUsage.Checked;
            vramFontColor.Enabled = showVramUsage.Checked;
            fpsFontColor.Enabled = showFps.Checked;
            combineTemperatureAndUsage.Enabled =
                (showCpu.Checked && showCpuUsage.Checked) ||
                (showGpu.Checked && showGpuUsage.Checked);
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
                if (option.IsCombinedTemperatureUsage)
                {
                    updatedOrder.Add(option.Kind);
                    updatedOrder.Add(option.Kind == OsdItemKind.CpuTemperature
                        ? OsdItemKind.CpuUsage
                        : OsdItemKind.GpuUsage);
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

        private void ApplyTheme()
        {
            bool light = _mainForm != null && _mainForm.IsLightModeEnabled;
            Color windowBack = light ? Color.FromArgb(218, 226, 238) : Color.FromArgb(21, 21, 21);
            Color surfaceBack = light ? Color.White : Color.FromArgb(40, 40, 40);
            Color text = light ? Color.FromArgb(31, 41, 55) : Color.LightGray;
            Color title = light ? Color.FromArgb(15, 23, 42) : Color.WhiteSmoke;
            Color accent = light ? Color.FromArgb(37, 99, 235) : Color.FromArgb(0, 120, 212);
            Color border = light ? Color.FromArgb(210, 218, 230) : Color.FromArgb(70, 70, 70);

            BackColor = windowBack;
            ForeColor = text;
            mainPanel.BackColor = windowBack;
            rootLayout.BackColor = windowBack;
            leftColumn.BackColor = windowBack;
            rightColumn.BackColor = windowBack;
            formTitle.ForeColor = title;

            foreach (Panel card in new[] { metricsCard, labelsCard, appearanceCard, layoutCard, hotkeyCard })
            {
                card.BackColor = surfaceBack;

                foreach (Label label in card.Controls.OfType<Label>())
                    label.ForeColor = text;
                foreach (CheckBox checkBox in card.Controls.OfType<CheckBox>())
                {
                    checkBox.BackColor = surfaceBack;
                    checkBox.ForeColor = text;
                    checkBox.UseVisualStyleBackColor = false;
                }
            }

            customLabelsLayout.BackColor = surfaceBack;
            foreach (Label label in new[]
            {
                customCpuLabelCaption,
                customGpuLabelCaption,
                customCpuUsageLabelCaption,
                customGpuUsageLabelCaption,
                customRamLabelCaption,
                customVramLabelCaption,
                customFpsLabelCaption,
                spacingHeader
            })
            {
                label.BackColor = surfaceBack;
                label.ForeColor = text;
            }

            fontColorsLayout.BackColor = surfaceBack;
            foreach (Label label in fontColorsLayout.Controls.OfType<Label>())
            {
                label.BackColor = surfaceBack;
                label.ForeColor = text;
            }

            foreach (Control input in new Control[]
            {
                positionValue, fontFamilyValue,
                customCpuLabel, customGpuLabel, customCpuUsageLabel, customGpuUsageLabel,
                customRamLabel, customVramLabel, customFpsLabel, labelValueSpacing, rowsSpacing, columnsSpacing,
                columnsValue, fontSizeValue, screenMarginValue, itemOrder, hotkeyValue
            })
            {
                input.BackColor = light ? Color.White : Color.FromArgb(32, 32, 32);
                input.ForeColor = text;
            }

            opacityValue.BackColor = surfaceBack;
            backgroundOpacityValue.BackColor = surfaceBack;

            if (!_hasCustomBackgroundColor)
                backgroundColor.BackColor = OsdColorHelper.GetDefaultBackground(light);

            foreach (Button button in new[] { orderUp, orderDown })
            {
                button.BackColor = surfaceBack;
                button.ForeColor = text;
                button.FlatAppearance.BorderColor = border;
                button.FlatAppearance.MouseOverBackColor = light
                    ? Color.FromArgb(232, 238, 247)
                    : Color.FromArgb(55, 55, 55);
                button.FlatAppearance.MouseDownBackColor = light
                    ? Color.FromArgb(210, 220, 234)
                    : Color.FromArgb(70, 70, 70);
            }

            foreach (Button button in new[] { backgroundColor, cpuFontColor, gpuFontColor, ramFontColor, vramFontColor, fpsFontColor })
            {
                button.ForeColor = GetReadableForeground(button.BackColor);
                button.FlatAppearance.BorderColor = border;
            }

            saveBtn.BackColor = accent;
            saveBtn.ForeColor = Color.White;
            saveBtn.FlatAppearance.BorderColor = border;
            saveBtn.FlatAppearance.MouseOverBackColor = light
                ? Color.FromArgb(29, 78, 216)
                : Color.FromArgb(0, 102, 184);
            saveBtn.FlatAppearance.MouseDownBackColor = light
                ? Color.FromArgb(30, 64, 175)
                : Color.FromArgb(0, 84, 153);
            exitBtn.BackColor = windowBack;
            exitBtn.ForeColor = title;
            exitBtn.FlatAppearance.BorderColor = windowBack;
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

        private sealed class OsdItemOption
        {
            internal OsdItemOption(
                OsdItemKind kind,
                bool isCombinedTemperatureUsage = false)
            {
                Kind = kind;
                IsCombinedTemperatureUsage = isCombinedTemperatureUsage;
            }

            internal OsdItemKind Kind { get; }
            internal bool IsCombinedTemperatureUsage { get; }

            public override string ToString()
            {
                if (IsCombinedTemperatureUsage)
                {
                    return Kind == OsdItemKind.CpuTemperature
                        ? "CPU + Usage"
                        : "GPU + Usage";
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
                return Value.ToString("0.#", CultureInfo.CurrentCulture);
            }
        }
    }
}
