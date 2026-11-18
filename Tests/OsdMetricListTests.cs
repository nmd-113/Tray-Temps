using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using TrayTemps;

internal static class OsdMetricListTests
{
    private const BindingFlags Flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
    private static int _checks;
    [DllImport("user32.dll")] private static extern bool PostMessage(IntPtr handle, int msg, IntPtr wp, IntPtr lp);
    [StructLayout(LayoutKind.Sequential)] private struct NativeRect { public int Left, Top, Right, Bottom; }
    [StructLayout(LayoutKind.Sequential)] private struct ComboBoxInfo
    {
        public int Size; public NativeRect Item, Button; public int ButtonState;
        public IntPtr Combo, Edit, List;
    }
    [DllImport("user32.dll")] private static extern bool GetComboBoxInfo(IntPtr handle, ref ComboBoxInfo info);
    [DllImport("user32.dll")] private static extern bool GetWindowRect(IntPtr handle, out NativeRect rect);
    private static void Check(bool value, string message) { if (!value) throw new Exception("FAILED: " + message); Console.WriteLine("PASS " + (++_checks) + ": " + message); }
    private static object Field(object target, string name)
    {
        for (Type type = target.GetType(); type != null; type = type.BaseType) { var f = type.GetField(name, Flags); if (f != null) return f.GetValue(target); }
        throw new MissingFieldException(name);
    }
    private static object Call(object target, string name, params object[] args)
    {
        for (Type type = target.GetType(); type != null; type = type.BaseType) { var method = type.GetMethod(name, Flags); if (method != null) return method.Invoke(target, args); }
        throw new MissingMethodException(name);
    }
    private sealed class Host : MainForm { protected override void OnLoad(EventArgs e) { } }
    private static T Control<T>(Form form, string name) where T : Control => (T)form.Controls.Find(name, true).Single();
    private static ListViewItem Row(OsdSettingsDialog form, string key) => Control<ListView>(form, "metricsList").Items.Cast<ListViewItem>().Single(i => (string)i.Tag == key);
    private static void Begin(OsdSettingsDialog form, string key, int column)
    {
        var list = Control<ListView>(form, "metricsList");
        Call(form, "ShowPage", Enum.Parse(typeof(OsdSettingsDialog).GetNestedType("OsdSettingsPage", BindingFlags.NonPublic), "Metrics"));
        foreach (ListViewItem row in list.Items) row.Selected = false;
        var item = Row(form, key); item.Selected = true; item.Focused = true; list.Focus();
        Call(form, "BeginMetricCellEdit", column);
    }
    private static void Edit(OsdSettingsDialog form, string key, int column, string text, bool commit = true)
    {
        Begin(form, key, column); Control<TextBox>(form, "metricLabelEditor").Text = text; Call(form, "EndMetricLabelEdit", commit);
    }
    private static OsdConfiguration Config() { var config = new OsdConfiguration { MetricGroups = new Dictionary<string, string>(), LabelOverrides = new Dictionary<string, string>() }; OsdMetricDefinitions.Migrate(config); return config; }
    internal static async Task Run()
    {
        _checks = 0; Model();
        using (var host = new Host())
        {
            host.Show(); host.Opacity = 1;
            await Ui(host);
            SettingsConsistency(host);
            Presets(host);
            Render();
            host.Close();
        }
        Console.WriteLine("ALL METRIC LIST CHECKS PASSED: " + _checks);
    }
    private static void Model()
    {
        var legacy = JsonSerializer.Deserialize<OsdConfiguration>("{\"ShowCpuUsage\":true,\"ShowRamUsage\":true,\"MetricGroupingMode\":2,\"ShowFps\":true,\"ShowFpsOnePercentLow\":true,\"FontSize\":24,\"CpuFontColor\":-16711681}");
        OsdMetricDefinitions.Migrate(legacy);
        Check(OsdMetricDefinitions.Group(legacy, "CpuTemperature") == "CPU" && OsdMetricDefinitions.Group(legacy, "RamUsage") == "CPU", "legacy CPU grouping migrates");
        Check(OsdMetricDefinitions.Group(legacy, "Fps") == OsdMetricDefinitions.Group(legacy, "FpsOnePercentLow"), "legacy FPS/low pairing migrates");
        Check(OsdMetricDefinitions.ResolveLabel(legacy, "CpuTemperature") == "CPU" && OsdMetricDefinitions.ResolveLabel(legacy, "CpuUsage") == "", "legacy grouped captions preserve the existing presentation");
        legacy.MetricGroups["CpuUsage"] = "";
        Check(OsdMetricDefinitions.ResolveLabel(legacy, "CpuUsage") == "CPU Load", "ungrouping restores retained standalone label");
        Check(legacy.MetricStyles.Count == OsdMetricDefinitions.Keys.Length && legacy.MetricStyles["CpuUsage"].FontSize == 24 && legacy.MetricStyles["CpuUsage"].LabelColor == legacy.CpuFontColor, "legacy fonts and colors seed all twelve styles");
        string json = JsonSerializer.Serialize(legacy); var copy = JsonSerializer.Deserialize<OsdConfiguration>(json); OsdMetricDefinitions.Migrate(copy);
        Check(copy.MetricGroups["CpuUsage"] == "" && copy.MetricStyles["FpsOnePercentLow"].FontSize == 24, "JSON retains explicit ungrouping and per-metric styles");
        var clone = legacy.Clone(); clone.MetricStyles["Fps"].FontSize = 48; clone.MetricGroups["Fps"] = ""; clone.GroupLabels["CPU"] = "Processor";
        Check(legacy.MetricStyles["Fps"].FontSize == 24 && legacy.MetricGroups["Fps"] != "" && !legacy.GroupLabels.ContainsKey("CPU"), "draft clones isolate nested styles, membership and group captions");
        string before = JsonSerializer.Serialize(copy); OsdMetricDefinitions.Migrate(copy);
        Check(before == JsonSerializer.Serialize(copy), "normalization is idempotent");
        var invalid = Config(); invalid.MetricStyles["Fps"].FontSize = float.NaN; invalid.MetricGroups["Fps"] = "  A\r\n "; invalid.GroupSeparator = " ";
        OsdMetricDefinitions.Migrate(invalid);
        Check(invalid.MetricStyles["Fps"].FontSize == 16 && invalid.MetricGroups["Fps"] == "A" && invalid.GroupSeparator == " ", "invalid sizes and multiline names normalize while explicit spaces survive");
        Check(OsdMetricDefinitions.Separator(null, null) == " ", "ordinary grouped metrics use one built-in space");
        var separatorConfig = Config();
        var fpsPair = new[] { Metric(separatorConfig, "Fps", "144"), Metric(separatorConfig, "FpsOnePercentLow", "92") };
        Check(OsdMetricDefinitions.Separator(fpsPair[0], fpsPair[1]) == "    " &&
            OsdMetricDefinitions.Separator(fpsPair[1], fpsPair[0]) == "    ",
            "empty FPS/1% Low separator keeps two base spaces plus one padding space on both sides in either order");
        var appDefaults = OsdConfiguration.CreateAppDefault();
        var missingOsd = System.Text.Json.JsonSerializer.Deserialize<AppSettings>("{\"LightMode\":true}");
        Check(!appDefaults.Enabled && appDefaults.LabelValueSpacing == 10 && missingOsd.Osd.LabelValueSpacing == 10 &&
            !missingOsd.Osd.Enabled && missingOsd.Osd.ShowCpu && missingOsd.Osd.ShowGpu &&
            OsdMetricDefinitions.Keys.Where(key => key != "CpuTemperature" && key != "GpuTemperature").All(key => !OsdMetricDefinitions.IsVisible(missingOsd.Osd, key)) &&
            missingOsd.Osd.MetricStyles.Values.All(style => style.FontFamily == OsdFontHelper.EmbeddedJetBrainsMonoExtraBoldDisplayName &&
                style.FontSize == 10 && style.LabelColor == Color.White.ToArgb() && style.ValueColor == Color.White.ToArgb()),
            "fresh settings use neutral OSD defaults with only CPU/GPU temperatures enabled");
        var explicitOsd = System.Text.Json.JsonSerializer.Deserialize<AppSettings>("{\"Osd\":{\"Enabled\":true}}");
        Check(explicitOsd.Osd.Enabled, "an explicitly enabled saved OSD setting remains enabled");
        var emptyOsd = JsonSerializer.Deserialize<AppSettings>("{\"LightMode\":true,\"Osd\":{}}");
        Check(emptyOsd.LightMode && JsonSerializer.Serialize(emptyOsd.Osd) == JsonSerializer.Serialize(appDefaults),
            "an empty OSD section uses the same neutral defaults as an omitted section");
        var savedAppearance = JsonSerializer.Deserialize<AppSettings>("{\"Osd\":{\"Enabled\":true,\"FontSize\":23,\"CpuFontColor\":-65536}}");
        OsdMetricDefinitions.Migrate(savedAppearance.Osd);
        var savedAppearanceCopy = JsonSerializer.Deserialize<AppSettings>(JsonSerializer.Serialize(savedAppearance));
        Check(savedAppearanceCopy.Osd.Enabled && savedAppearanceCopy.Osd.MetricStyles["CpuTemperature"].FontSize == 23 &&
            savedAppearanceCopy.Osd.MetricStyles["CpuTemperature"].ValueColor == Color.Red.ToArgb(),
            "nonempty legacy appearance migrates and round-trips without being replaced by fresh defaults");
        Check(JsonSerializer.Deserialize<AppSettings>("{\"Osd\":null}").Osd == null,
            "a null OSD section remains available to the existing app normalization path");
        var duplicateCaptions = Config(); duplicateCaptions.GroupLabels = new Dictionary<string, string> { ["Stats"] = "A", ["stats"] = "B" };
        Check(duplicateCaptions.Clone().GroupLabels.Count == 1, "case-variant saved caption keys clone safely");
        var duplicateGaps = Config();
        duplicateGaps.MetricLabelValueGaps = new Dictionary<string, int> { ["Group:Stats"] = 12, ["group:stats"] = 24 };
        var clonedGaps = duplicateGaps.Clone(); OsdMetricDefinitions.Migrate(clonedGaps);
        Check(clonedGaps.MetricLabelValueGaps.Count == 1 && clonedGaps.MetricLabelValueGaps["GROUP:STATS"] == 24,
            "case-variant saved gap keys clone and migrate without exceptions");
        Check(OsdMetricDefinitions.SingleLine("\ud83d", 40, false) == "" && OsdMetricDefinitions.SingleLine("★  ", 40, false) == "★  ",
            "broken surrogate input is removed while Unicode icons and trailing spaces remain intact");
    }

    private static void SettingsConsistency(Host host)
    {
        var original = Config(); original.BackgroundColor = null;
        var themeSwitch = Control<CheckBox>(host, "lightModeSwitch");
        bool previousTheme = themeSwitch.Checked;
        try
        {
            using (var form = new OsdSettingsDialog(host, original))
            {
                form.Show(host);
                OsdConfiguration preview = null;
                form.PreviewConfigurationChanged += config => preview = config;
                foreach (bool light in new[] { false, true, false })
                {
                    themeSwitch.Checked = light;
                    Call(form, "MainForm_ThemeChanged", host, EventArgs.Empty);
                    int expected = OsdColorHelper.GetDefaultBackground(light).ToArgb();
                    Check(Control<Button>(form, "backgroundColor").BackColor.ToArgb() == expected &&
                        preview != null && preview.BackgroundColor == expected,
                        "legacy automatic background color agrees between theme, dialog and preview: light=" + light);
                    var saved = (OsdConfiguration)Call(form, "CreateConfigurationFromControls");
                    Check(host.NormalizeOsdConfiguration(saved).BackgroundColor == expected,
                        "Save preserves the background color that was previewed");
                }
                form.Close();
            }
            Check(original.BackgroundColor == null, "previewing theme defaults does not alter the original settings");
        }
        finally { themeSwitch.Checked = previousTheme; }
        var gaps = Config();
        gaps.MetricLabelValueGaps["CpuTemperature"] = -5;
        gaps.MetricLabelValueGaps["Group:CPU:"] = 120;
        var roundTrip = JsonSerializer.Deserialize<OsdConfiguration>(JsonSerializer.Serialize(gaps));
        roundTrip = host.NormalizeOsdConfiguration(roundTrip);
        Check(roundTrip.MetricLabelValueGaps["CpuTemperature"] == 0 && roundTrip.MetricLabelValueGaps["group:cpu:"] == 100,
            "saved metric and case-insensitive group gaps normalize to the dialog's supported range");
    }

    private static void Presets(Host host)
    {
        var first = OsdPresets.LoadEmbedded(0);
        var defaults = OsdConfiguration.CreateAppDefault();
        foreach (int presetIndex in new[] { 0, 1, 2 })
        {
            var preset = OsdPresets.LoadEmbedded(presetIndex);
            Check(preset.LabelValueSpacing == 10 && preset.FpsLabelValueSpacing == 10 &&
                preset.MetricLabelValueGaps.Values.All(gap => gap == 10),
                "embedded preset uses ten-pixel defaults including migrated per-metric gaps: " + presetIndex);
            var disabledKeys = OsdMetricDefinitions.Keys.Where(key => !OsdMetricDefinitions.IsVisible(preset, key));
            Check(disabledKeys.All(key => preset.LabelOverrides[key] == defaults.LabelOverrides[key] &&
                OsdMetricDefinitions.Group(preset, key).Length == 0 &&
                preset.MetricStyles[key].FontFamily == defaults.MetricStyles[key].FontFamily &&
                preset.MetricStyles[key].FontSize == defaults.MetricStyles[key].FontSize &&
                preset.MetricStyles[key].LabelColor == defaults.MetricStyles[key].LabelColor &&
                preset.MetricStyles[key].ValueColor == defaults.MetricStyles[key].ValueColor),
                "embedded preset disabled metrics retain neutral app defaults: " + presetIndex);
            Check(!preset.Enabled && !preset.HotkeyEnabled && preset.GroupLabels.Count == 0 &&
                preset.GroupSeparator == null && preset.CustomCpuLabel == defaults.CustomCpuLabel &&
                preset.CustomGpuLabel == defaults.CustomGpuLabel && preset.CustomFpsLabel == defaults.CustomFpsLabel &&
                (preset.Columns != 1 || preset.ColumnsSpacing == defaults.ColumnsSpacing) &&
                (preset.BackgroundOpacityPercent != 0 || preset.BackgroundCornerRoundness == defaults.BackgroundCornerRoundness),
                "embedded preset unused legacy, shortcut, and layout settings use app defaults: " + presetIndex);
        }
        first.Enabled = false;
        Check(defaults.ShowCpu && defaults.ShowGpu && !defaults.ShowFps && first.ItemOrder != defaults.ItemOrder,
            "fresh defaults are neutral and distinct from the explicitly loaded preset");
        first.MetricStyles["Fps"].FontSize = 48;
        Check(OsdPresets.LoadEmbedded(0).MetricStyles["Fps"].FontSize != 48, "embedded preset loads return independent deep copies");
        string path = Path.Combine(Path.GetTempPath(), "TrayTemps-preset-test-" + Guid.NewGuid().ToString("N") + ".json");
        try
        {
            OsdPresets.SaveFile(path, defaults);
            Check(JsonSerializer.Serialize(OsdPresets.LoadFile(path)) == JsonSerializer.Serialize(defaults), "custom presets round-trip all OSD settings");
            OsdPresets.SaveFile(path, OsdPresets.LoadEmbedded(2));
            Check(!OsdPresets.LoadFile(path).ShowCurrentTime, "saving over a custom preset atomically replaces its contents");
            string full = "{\"Autostart\":true,\"Osd\":" + JsonSerializer.Serialize(defaults) + "}";
            Check(JsonSerializer.Serialize(OsdPresets.Parse(full)) == JsonSerializer.Serialize(defaults), "importing a complete app file extracts only its OSD section");
            bool rejected = false;
            try { OsdPresets.Parse("{\"Autostart\":true}"); } catch (JsonException) { rejected = true; }
            Check(rejected, "unrelated JSON cannot replace the dialog draft");
            string saved = File.ReadAllText(path);
            using (var locked = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                rejected = false;
                try { OsdPresets.SaveFile(path, defaults); } catch (IOException) { rejected = true; }
                Check(rejected, "an unavailable preset destination reports a save failure");
            }
            Check(File.ReadAllText(path) == saved && !Directory.GetFiles(Path.GetDirectoryName(path), Path.GetFileName(path) + ".*.tmp").Any(),
                "a failed preset overwrite preserves existing contents and removes its temporary file");
            var oversized = defaults.Clone(); oversized.CustomCpuLabel = new string('x', 1024 * 1024);
            rejected = false;
            try { OsdPresets.SaveFile(path, oversized); } catch (IOException) { rejected = true; }
            Check(rejected && File.ReadAllText(path) == saved, "oversized exports cannot overwrite a readable preset with an unloadable file");
            File.WriteAllText(path, new string(' ', 1024 * 1024 + 1));
            rejected = false;
            try { OsdPresets.LoadFile(path); } catch (IOException) { rejected = true; }
            Check(rejected, "oversized input is rejected before JSON parsing");
        }
        finally { if (File.Exists(path)) File.Delete(path); }
        var original = Config(); original.HotkeyEnabled = false;
        string before = JsonSerializer.Serialize(original);
        using (var form = new OsdSettingsDialog(host, original))
        {
            form.Show(host);
            var navigationOrder = new[] { "presetsNavButton", "metricsNavButton", "appearanceNavButton", "layoutNavButton" };
            Check(Control<TabControl>(form, "pageTabs").SelectedTab.Name == "presetsPage" &&
                navigationOrder.Select(name => Control<Button>(form, name)).OrderBy(button => button.Top)
                    .Select(button => button.Name).SequenceEqual(navigationOrder),
                "OSD opens on Presets with the sidebar ordered Presets, Metrics, Appearance, Layout");
            Call(form, "PresetsNavButton_Click", null, EventArgs.Empty);
            Check(Control<TabControl>(form, "pageTabs").SelectedTab.Name == "presetsPage" && Control<ComboBox>(form, "presetList").Items.Count == 3,
                "Presets navigation opens the Designer page with three embedded templates");
            using (var bitmap = new Bitmap(form.Width, form.Height))
            {
                form.DrawToBitmap(bitmap, form.ClientRectangle);
                bitmap.Save(Path.Combine(Path.GetTempPath(), "TrayTemps-osd-presets.png"));
            }
            int previews = 0; form.PreviewConfigurationChanged += config => previews++;
            var groupedGap = Control<ComboBox>(form, "groupedValuesGapPercent");
            Check(groupedGap.DropDownStyle == ComboBoxStyle.DropDownList && groupedGap.Items.Count == 11 &&
                groupedGap.GetItemText(groupedGap.SelectedItem) == "100%", "group gap selector starts at the existing minimum and displays percentages");
            groupedGap.SelectedIndex = 5;
            Check(previews == 1 && ((OsdConfiguration)Call(form, "CreateConfigurationFromControls")).GroupedValuesGapPercent == 150,
                "group gap selector immediately previews its saved percentage");
            previews = 0;
            Call(form, "ApplyPreset", OsdPresets.LoadEmbedded(1));
            var draft = (OsdConfiguration)Call(form, "CreateConfigurationFromControls");
            Check(draft.GroupedValuesGapPercent == 100 && groupedGap.GetItemText(groupedGap.SelectedItem) == "100%",
                "preset loading restores its own default grouped spacing");
            Check(previews == 1 && !draft.Enabled && !draft.HotkeyEnabled && draft.Columns == 5 && draft.ItemOrder == host.NormalizeOsdConfiguration(OsdPresets.LoadEmbedded(1)).ItemOrder,
                "preset load emits one complete preview and preserves OSD enable state and the current shortcut");
            Call(form, "ShowPage", Enum.Parse(typeof(OsdSettingsDialog).GetNestedType("OsdSettingsPage", BindingFlags.NonPublic), "Appearance"));
            var grid = Control<DataGridView>(form, "appearanceGrid");
            int fonts = ((DataGridViewComboBoxColumn)grid.Columns[1]).Items.Count;
            for (int i = 0; i < 3; i++) Call(form, "ApplyPreset", OsdPresets.LoadEmbedded(2));
            draft = (OsdConfiguration)Call(form, "CreateConfigurationFromControls");
            Check(grid.Rows.Count == OsdMetricDefinitions.Keys.Length && ((DataGridViewComboBoxColumn)grid.Columns[1]).Items.Count == fonts &&
                draft.TransparentBackground && !draft.ShowFpsOnePercentLow && !draft.ShowCurrentTime && draft.MetricStyles["Fps"].FontSize == 12,
                "repeated preset loads refresh appearance, visibility and layout without duplicating rows or font choices");
            Check(JsonSerializer.Serialize(original) == before && form.SelectedConfiguration == null,
                "preset previews do not modify saved settings before dialog Save");
            Begin(form, "Fps", 1);
            previews = 0;
            Call(form, "ApplyPreset", OsdPresets.Parse("{\"ShowCpu\":true,\"ShowGpu\":false}"));
            Check(previews == 1, "loading a preset during label editing emits only the final complete preview");
            Edit(form, "CpuTemperature", 1, "Processor");
            Check(((OsdConfiguration)Call(form, "CreateConfigurationFromControls")).LabelOverrides["CpuTemperature"] == "Processor",
                "legacy preset without label overrides remains editable after import");
            const string malformedLegacy = "{\"ShowCpu\":true,\"ShowCpuUsage\":true,\"MetricGroupingMode\":999}";
            Call(form, "ApplyPreset", OsdPresets.Parse(malformedLegacy));
            Check(OsdMetricDefinitions.Group((OsdConfiguration)Call(form, "CreateConfigurationFromControls"), "CpuTemperature").Length == 0,
                "legacy preset migration runs after invalid grouping modes are normalized");
            Call(form, "ApplyPreset", OsdPresets.LoadEmbedded(2));
            grid.Rows[0].Cells[2].Value = "invalid";
            Call(form, "PresetsNavButton_Click", null, EventArgs.Empty);
            Check(!(bool)Call(form, "TryCommitAppearance") && Control<TabControl>(form, "pageTabs").SelectedTab.Name == "appearancePage",
                "invalid appearance input redirects validation from Presets to its editable cell");
            Call(form, "ApplyPreset", OsdPresets.LoadEmbedded(2));
            Call(form, "PresetsNavButton_Click", null, EventArgs.Empty);
            form.Size = new Size(720, 480);
            foreach (float scale in new[] { 1f, 1.5f, 4f / 3f })
            {
                if (scale != 1) form.Scale(new SizeF(scale, scale));
                var help = Control<Label>(form, "presetsHelp");
                Size text = TextRenderer.MeasureText(help.Text, help.Font, new Size(help.Width, int.MaxValue), TextFormatFlags.WordBreak);
                Check(text.Height <= help.Height && new[] { "presetList", "loadEmbeddedPreset", "loadPresetFile", "savePresetFile" }
                    .Select(name => Control<Control>(form, name)).All(control => control.Left >= 0 && control.Right <= control.Parent.ClientSize.Width && control.Bottom <= control.Parent.ClientSize.Height),
                    "Presets help and controls fit at minimum dialog size and control scale " + scale);
            }
            Call(form, "SaveBtn_Click", null, EventArgs.Empty);
            Check(form.SelectedConfiguration != null && form.SelectedConfiguration.TransparentBackground && !form.SelectedConfiguration.HotkeyEnabled,
                "dialog Save commits the loaded preset through the existing save path");
        }
        original.Enabled = true;
        before = JsonSerializer.Serialize(original);
        using (var form = new OsdSettingsDialog(host, original))
        {
            form.Show(host);
            Control<CheckBox>(form, "hotkeyEnabled").Checked = true;
            Call(form, "TryCaptureHotkey", Keys.Control | Keys.Alt | Keys.F8);
            Call(form, "ApplyPreset", OsdPresets.LoadEmbedded(1));
            var draft = (OsdConfiguration)Call(form, "CreateConfigurationFromControls");
            Check(draft.Enabled && draft.HotkeyEnabled && draft.HotkeyKey == (int)Keys.F8 && draft.HotkeyModifiers == 3,
                "preset loading also preserves enabled OSD and a shortcut edited in the current dialog");
            var nine = Config();
            foreach (string key in OsdMetricDefinitions.Keys) OsdMetricDefinitions.SetVisible(nine, key, true);
            nine.Columns = OsdMetricDefinitions.Keys.Length;
            Call(form, "ApplyPreset", nine);
            Check(((OsdConfiguration)Call(form, "CreateConfigurationFromControls")).Columns == OsdMetricDefinitions.Keys.Length && Control<ComboBox>(form, "columnsValue").Items.Count == OsdMetricDefinitions.Keys.Length,
                "preset normalization and dialog loading retain a column for every standalone metric");
            form.Close();
            Check(form.SelectedConfiguration == null && JsonSerializer.Serialize(original) == before,
                "closing after multiple preset previews discards the draft and leaves the original configuration intact");
        }
    }
    private static async Task Ui(Host host)
    {
        var tabs = (TabControl)Field(host, "mainTabControl"); tabs.SelectedIndex = 1;
        string[] buttonNames = { "osdSettings", "fpsLimiterSettings", "clearSettings", "colortempsConfig", "cpuColorValue", "gpuColorValue" };
        Check(buttonNames.Select(name => ((Button)Field(host, name)).Height).Distinct().Count() == 1, "settings action button heights remain consistent");
        var original = Config(); original.ShowCpu = original.ShowGpu = false; original.ShowFpsOnePercentLow = true;
        var normalized = (OsdConfiguration)Call(host, "NormalizeOsdConfiguration", original);
        Check(!normalized.ShowCpu && normalized.ShowFpsOnePercentLow, "low-only configuration does not force CPU visibility");
        using (var form = new OsdSettingsDialog(host, original))
        {
            form.Show(host); await Task.Delay(30);
            var list = Control<ListView>(form, "metricsList"); var grid = Control<DataGridView>(form, "appearanceGrid");
            var columnCount = Control<ComboBox>(form, "columnsValue");
            var rowSpacing = Control<ComboBox>(form, "rowsSpacing"); var columnSpacing = Control<ComboBox>(form, "columnsSpacing");
            var rowSpacingLabel = Control<Label>(form, "rowsGapHeader"); var columnSpacingLabel = Control<Label>(form, "columnsGapHeader");
            Check(columnCount.Items.Count == 1 && columnCount.SelectedIndex == 0 && !rowSpacing.Enabled && !columnSpacing.Enabled &&
                !rowSpacingLabel.Enabled && !columnSpacingLabel.Enabled,
                "column choices and spacing availability follow the one currently enabled standalone metric");
            Check(grid.Rows.Count == 0, "Appearance rows stay deferred until the user opens that page");
            Call(form, "ShowPage", Enum.Parse(typeof(OsdSettingsDialog).GetNestedType("OsdSettingsPage", BindingFlags.NonPublic), "Appearance"));
            await Task.Delay(10);
            string[] columnFields = { "appearanceMetricColumn", "appearanceFamilyColumn", "appearanceSizeColumn", "appearanceLabelColorColumn", "appearanceValueColorColumn" };
            Check(columnFields.Select((name, index) => ReferenceEquals(Field(form, name), grid.Columns[index]) && grid.Columns[index].Name == name).All(value => value),
                "Designer column names, fields and live grid columns remain identical after serialization");
            using (var designer = new OsdSettingsDialog())
            {
                var designerGrid = Control<DataGridView>(designer, "appearanceGrid");
                Check(Math.Abs(grid.DefaultCellStyle.Font.SizeInPoints - designerGrid.DefaultCellStyle.Font.SizeInPoints) < 0.01f &&
                    Math.Abs(grid.ColumnHeadersDefaultCellStyle.Font.SizeInPoints - designerGrid.ColumnHeadersDefaultCellStyle.Font.SizeInPoints) < 0.01f,
                    "runtime grid fonts preserve the Designer row/header point sizes");
            }
            Check(Control<TextBox>(form, "metricLabelEditor").Font.SizeInPoints == list.Font.SizeInPoints &&
                Control<Label>(form, "metricGroupHeader").Font.SizeInPoints == Control<Label>(form, "metricNameHeader").Font.SizeInPoints,
                "metric editor and all column headers use matching point sizes");
            Check(list.Columns.Count == 3 && list.Columns[2].Text == "Group" && list.Items.Count == OsdMetricDefinitions.Keys.Length, "Metrics provides all twelve options and editable Group column");
            Check(grid.Columns.Count == 5 && grid.Rows.Count == OsdMetricDefinitions.Keys.Length && grid.Columns[1] is DataGridViewComboBoxColumn && grid.Columns[3] is DataGridViewButtonColumn, "Appearance provides the five requested columns for all metrics");
            Check(Math.Abs(grid.DefaultCellStyle.Font.SizeInPoints - list.Font.SizeInPoints) < 0.01f &&
                Control<TreeView>(form, "itemOrder").Font.SizeInPoints < list.Font.SizeInPoints,
                "Metrics and Appearance share their text size while Display Order remains intentionally smaller");
            Check(grid.Columns[1].Width > grid.Columns[3].Width + grid.Columns[4].Width,
                "Font Family gets more width than both compact color columns together");
            var metricHeader = Control<Label>(form, "metricNameHeader");
            Check(grid.ColumnHeadersDefaultCellStyle.Font.FontFamily.Name == metricHeader.Font.FontFamily.Name &&
                Math.Abs(grid.ColumnHeadersDefaultCellStyle.Font.SizeInPoints - metricHeader.Font.SizeInPoints) < 0.01f,
                "Appearance and Metrics use the same header typeface and size (" + grid.ColumnHeadersDefaultCellStyle.Font + "; " + metricHeader.Font + ")");
            Check(grid.CellBorderStyle == DataGridViewCellBorderStyle.None && grid.SelectionMode == DataGridViewSelectionMode.FullRowSelect,
                "Appearance uses plain rows and whole-row selection like Metrics");
            Check(!string.IsNullOrEmpty((string)grid.Rows[0].Cells[3].Value) && !string.IsNullOrEmpty((string)grid.Rows[0].Cells[4].Value) &&
                Control<Button>(form, "backgroundColor").Text.Length > 0, "all color picker buttons use a Unicode palette icon");
            Check(grid.Rows[0].Cells[3].InheritedStyle.Font.SizeInPoints == 9 &&
                grid.Rows[0].Cells[4].InheritedStyle.Font.SizeInPoints == 9,
                "list palette icons use the smaller 9-point size");
            Check(list.Items[0].Bounds.Height >= 26 && grid.Rows[0].Height == 26 &&
                Control<TreeView>(form, "itemOrder").ItemHeight == 26 &&
                list.Items[list.Items.Count - 1].Bounds.Bottom > list.ClientSize.Height,
                "metric rows retain their height and scroll when the twelve options exceed the viewport (native height " + list.Items[0].Bounds.Height + ")");
            Check(grid.Rows[0].Cells[3].ToolTipText.StartsWith("Label color (RGB ", StringComparison.Ordinal) &&
                grid.Rows[0].Cells[4].ToolTipText.StartsWith("Value color (RGB ", StringComparison.Ordinal),
                "color cells retain compact purpose and RGB tooltips");
            Check(Control<Panel>(form, "metricsListHeader").Height == 30 && grid.ColumnHeadersHeight >= 44,
                "Metrics and Appearance headers have extra vertical room");
            Check(new[] { "groupCaption", "groupCaptionLabel", "groupSeparator", "groupSeparatorLabel" }
                .All(name => form.Controls.Find(name, true).Length == 0), "obsolete group caption and separator controls are removed from the Designer");
            Row(form, "FpsOnePercentLow").Checked = false; Row(form, "FpsOnePercentLow").Checked = true;
            Check(Row(form, "FpsOnePercentLow").Checked && !Row(form, "Fps").Checked, "1% Low can be selected independently of FPS");
            Begin(form, "CpuUsage", 1); Control<TextBox>(form, "metricLabelEditor").Text = "Usage after check";
            Row(form, "Fps").Checked = true;
            Check(columnCount.Items.Count == 2 && rowSpacing.Enabled && !columnSpacing.Enabled,
                "adding a standalone metric expands the column range and keeps column spacing disabled at one column");
            columnCount.SelectedIndex = 1;
            Check(!rowSpacing.Enabled && columnSpacing.Enabled,
                "row spacing is disabled at the maximum column count while column spacing becomes available");
            columnCount.SelectedIndex = 0;
            await Task.Delay(40);
            Check(Row(form, "CpuUsage").SubItems[1].Text == "Usage after check", "checkbox changes commit and refresh the active edited cell after native notification");
            Row(form, "Fps").Checked = false;
            Check(columnCount.Items.Count == 1 && columnCount.SelectedIndex == 0 && !rowSpacing.Enabled && !columnSpacing.Enabled,
                "hiding a metric shrinks the column range and clamps the selected count safely");
            Edit(form, "Fps", 1, "Frames"); Edit(form, "Fps", 2, "  Performance  "); Edit(form, "FpsOnePercentLow", 2, "performance");
            Check(Row(form, "Fps").SubItems[2].Text == "Performance", "Group editor trims names");
            Check(columnCount.Items.Count == 1 && !rowSpacing.Enabled && !columnSpacing.Enabled,
                "grouping the enabled FPS metrics reduces the maximum to their single rendered group row");
            var tree = Control<TreeView>(form, "itemOrder");
            var parent = tree.Nodes.Cast<TreeNode>().Single(node => ((string)node.Tag).StartsWith("Group:"));
            Check(parent.Nodes.Count == 2, "case-insensitive group membership produces one group with both members");
            tree.SelectedNode = parent.Nodes[1]; Call(form, "MoveSelectedItem", -1);
            Check((string)parent.Nodes[0].Tag == "FpsOnePercentLow", "member priority moves within its group");
            Edit(form, "FpsOnePercentLow", 1, ""); Edit(form, "FpsOnePercentLow", 2, "", false);
            Check(Row(form, "FpsOnePercentLow").SubItems[1].Text == "" && Row(form, "FpsOnePercentLow").SubItems[2].Text.Length > 0, "Esc/cancel preserves membership and explicit blank labels");
            // Native double-click must not also change the visibility checkbox.
            Call(form, "ShowPage", Enum.Parse(typeof(OsdSettingsDialog).GetNestedType("OsdSettingsPage", BindingFlags.NonPublic), "Metrics"));
            Row(form, "Fps").EnsureVisible(); Rectangle bounds = Row(form, "Fps").SubItems[2].Bounds;
            int packed = (bounds.Left + 10) | ((bounds.Top + bounds.Height / 2) << 16); bool wasChecked = Row(form, "Fps").Checked;
            PostMessage(list.Handle, 0x201, (IntPtr)1, (IntPtr)packed); PostMessage(list.Handle, 0x202, IntPtr.Zero, (IntPtr)packed);
            PostMessage(list.Handle, 0x203, (IntPtr)1, (IntPtr)packed); PostMessage(list.Handle, 0x202, IntPtr.Zero, (IntPtr)packed);
            await Task.Delay(60);
            Check(Control<TextBox>(form, "metricLabelEditor").Visible && Row(form, "Fps").Checked == wasChecked, "native Group double-click edits without toggling metric visibility");
            Call(form, "EndMetricLabelEdit", true);
            grid.Rows[7].Cells[2].Value = "32"; grid.EndEdit();
            var draft = (OsdConfiguration)Call(form, "CreateConfigurationFromControls");
            Check(draft.MetricStyles["FpsOnePercentLow"].FontSize == 32 && original.MetricStyles["FpsOnePercentLow"].FontSize == 16, "per-metric font edits preview without leaking into original settings");
            grid.Rows[7].Cells[2].Value = "bad";
            Call(form, "SaveBtn_Click", null, EventArgs.Empty);
            Check(form.SelectedConfiguration == null && ((OsdConfiguration)Call(form, "CreateConfigurationFromControls")).MetricStyles["FpsOnePercentLow"].FontSize == 32,
                "invalid appearance cells cannot close Save or overwrite the valid style draft");
            grid.Rows[7].Cells[2].Value = "32";
            Begin(form, "Fps", 2);
            var groupEditor = Control<TextBox>(form, "metricLabelEditor");
            var menu = (ContextMenuStrip)Field(form, "customLabelContextMenu");
            groupEditor.Focus(); groupEditor.SelectAll(); menu.Show(groupEditor, new Point(0, groupEditor.Height));
            await Task.Delay(20);
            var categories = (Dictionary<string, ToolStripMenuItem>)Field(form, "_iconCategoryMenus");
            var icon = categories["general"].DropDownItems.OfType<ToolStripMenuItem>().First();
            string symbol = (string)icon.Tag; icon.PerformClick(); menu.Close();
            Check(groupEditor.Text == symbol && Row(form, "Fps").SubItems[1].Text == "Frames", "Unicode menu edits the metric Group cell without changing its label");
            Call(form, "EndMetricLabelEdit", false);
            Call(form, "ShowPage", Enum.Parse(typeof(OsdSettingsDialog).GetNestedType("OsdSettingsPage", BindingFlags.NonPublic), "Metrics"));
            var info = Control<Label>(form, "metricLabelsInfoIcon"); var tooltip = (ToolTip)Field(form, "settingsToolTip");
            Check(info.Visible && tooltip.GetToolTip(info).Length > 0, "label help icon and tooltip remain available");
            Check(info.Font.FontFamily.Name == "Segoe UI Symbol", "information icon retains its Designer symbol font at runtime");
            string metricsCapture = Path.Combine(Path.GetTempPath(), "TrayTemps-new-metrics.png");
            using (var bitmap = new Bitmap(form.Width, form.Height)) { form.DrawToBitmap(bitmap, form.ClientRectangle); bitmap.Save(metricsCapture); }
            foreach (bool light in new[] { false, true })
            {
                ((CheckBox)Field(host, "lightModeSwitch")).Checked = light; Call(form, "ApplyTheme");
                Check(TreeNodes(tree.Nodes).Where(node => node.Nodes.Count == 0).All(node => node.ForeColor ==
                    (OsdMetricDefinitions.IsVisible((OsdConfiguration)Field(form, "_metricDraft"), (string)node.Tag) ? tree.ForeColor : ThemePalette.Create(light).MutedText)),
                    "order tree updates enabled and disabled text colors for " + (light ? "light" : "dark") + " theme");
                Call(form, "ShowPage", Enum.Parse(typeof(OsdSettingsDialog).GetNestedType("OsdSettingsPage", BindingFlags.NonPublic), "Appearance"));
                var capture = Path.Combine(Path.GetTempPath(), "TrayTemps-new-appearance-" + light + ".png");
                using (var bitmap = new Bitmap(form.Width, form.Height)) { form.DrawToBitmap(bitmap, form.ClientRectangle); bitmap.Save(capture); }
                Check(grid.BackgroundColor == ThemePalette.Create(light).InputBack, "appearance grid follows " + (light ? "light" : "dark") + " theme");
                Check(grid.ColumnHeadersDefaultCellStyle.BackColor == metricHeader.BackColor && grid.ColumnHeadersDefaultCellStyle.ForeColor == metricHeader.ForeColor &&
                    grid.DefaultCellStyle.BackColor == list.BackColor && grid.DefaultCellStyle.ForeColor == list.ForeColor,
                    "Appearance and Metrics share header and row colors in " + (light ? "light" : "dark") + " theme");
                Check(Control<TableLayoutPanel>(form, "backgroundColorLayout").BackColor == ThemePalette.Create(light).SurfaceBack &&
                    Control<Label>(form, "backgroundColorLabel").ForeColor == ThemePalette.Create(light).Text,
                    "nested background-color controls follow " + (light ? "light" : "dark") + " theme");
                using (var swatches = new Bitmap(grid.Width, grid.Height))
                {
                    grid.DrawToBitmap(swatches, grid.ClientRectangle);
                    bool confined = true;
                    for (int row = 0; row < 2; row++) for (int column = 3; column <= 4; column++)
                    {
                        var cell = grid.Rows[row].Cells[column]; var cellBounds = grid.GetCellDisplayRectangle(column, row, false);
                        Color rowBack = cell.Selected ? grid.DefaultCellStyle.SelectionBackColor : grid.DefaultCellStyle.BackColor;
                        confined &= swatches.GetPixel(cellBounds.Left + 2, cellBounds.Top + cellBounds.Height / 2).ToArgb() == rowBack.ToArgb() &&
                            swatches.GetPixel(cellBounds.Left + cellBounds.Width / 2, cellBounds.Bottom - 1).ToArgb() == rowBack.ToArgb() &&
                            swatches.GetPixel(cellBounds.Left + cell.InheritedStyle.Padding.Left + 3, cellBounds.Top + cellBounds.Height / 2).ToArgb() == cell.InheritedStyle.BackColor.ToArgb();
                    }
                    Check(confined, "swatches color only their inset buttons, including selected rows, in " + (light ? "light" : "dark") + " theme");
                }
                Check(grid.Rows.Cast<DataGridViewRow>().All(row => row.Cells.Cast<DataGridViewCell>().All(cell => cell.ErrorText.Length == 0)), "all font/size cells contain valid combobox values after repaint");
                Console.WriteLine("CAPTURE=" + capture);
            }
            CheckAppearanceDropdown(grid, 1); CheckAppearanceDropdown(grid, 2);
            Call(form, "ShowPage", Enum.Parse(typeof(OsdSettingsDialog).GetNestedType("OsdSettingsPage", BindingFlags.NonPublic), "Layout"));
            string layoutCapture = Path.Combine(Path.GetTempPath(), "TrayTemps-new-layout.png");
            using (var bitmap = new Bitmap(form.Width, form.Height)) { form.DrawToBitmap(bitmap, form.ClientRectangle); bitmap.Save(layoutCapture); }
            foreach (float scale in new[] { 1f, 1.5f, 4f / 3f })
            {
                if (scale != 1) form.Scale(new SizeF(scale, scale));
                Check(Control<ComboBox>(form, "positionValue").Right <= Control<ComboBox>(form, "positionValue").Parent.ClientSize.Width &&
                    Control<ComboBox>(form, "columnsValue").Right <= Control<ComboBox>(form, "columnsValue").Parent.ClientSize.Width,
                    "remaining layout controls fit at " + scale);
                Call(form, "ShowPage", Enum.Parse(typeof(OsdSettingsDialog).GetNestedType("OsdSettingsPage", BindingFlags.NonPublic), "Appearance"));
                var footer = Control<TableLayoutPanel>(form, "appearanceFooter");
                var osdSlider = Control<TrackBar>(form, "opacityValue"); var bgSlider = Control<TrackBar>(form, "backgroundOpacityValue");
                var colorPanel = Control<TableLayoutPanel>(form, "backgroundColorLayout");
                var colorButton = Control<Button>(form, "backgroundColor"); var colorLabel = Control<Label>(form, "backgroundColorLabel");
                Check(osdSlider.Left == bgSlider.Left && osdSlider.Width == bgSlider.Width &&
                    footer.GetControlFromPosition(3, 0) == colorPanel && footer.GetControlFromPosition(3, 1) == null &&
                    footer.GetRow(colorPanel) == footer.GetRow(bgSlider) && footer.GetRow(osdSlider) == 1 &&
                    colorPanel.Left > bgSlider.Right && colorPanel.Right <= footer.ClientSize.Width &&
                    colorButton.Right <= colorPanel.ClientSize.Width && colorLabel.Right <= colorButton.Left,
                    "sliders align, background controls occupy the top row and lower-right cell stays empty at " + scale);
                Call(form, "ShowPage", Enum.Parse(typeof(OsdSettingsDialog).GetNestedType("OsdSettingsPage", BindingFlags.NonPublic), "Layout"));
            }
            form.Close();
            Check(original.MetricGroups.Count == 0 && original.LabelOverrides.Count == 0, "Cancel preserves original group and label settings");
        }
        using (var form = new OsdSettingsDialog(host, original))
        {
            form.Show(host); await Task.Delay(20);
            Check(Control<DataGridView>(form, "appearanceGrid").Rows.Count == 0,
                "a fresh dialog does not construct Appearance rows unless opened");
            Call(form, "SaveBtn_Click", null, EventArgs.Empty);
            Check(form.SelectedConfiguration != null,
                "Save works when Appearance was never visited");
        }
        using (var form = new OsdSettingsDialog(host, original))
        {
            form.Show(host); await Task.Delay(20); Edit(form, "FpsOnePercentLow", 1, "Slow");
            Begin(form, "FpsOnePercentLow", 2); Control<TextBox>(form, "metricLabelEditor").Text = "Stats";
            Call(form, "SaveBtn_Click", null, EventArgs.Empty);
            Check(form.SelectedConfiguration.MetricGroups["FpsOnePercentLow"] == "Stats" && form.SelectedConfiguration.LabelOverrides["FpsOnePercentLow"] == "Slow", "Save commits the active Group cell and independent low label");
        }
    }
    private static void CheckAppearanceDropdown(DataGridView grid, int column)
    {
        object original = grid.Rows[0].Cells[column].Value;
        grid.CurrentCell = grid.Rows[0].Cells[column]; grid.Focus(); grid.BeginEdit(true);
        var editor = grid.EditingControl as ComboBox;
        if (editor != null) editor.DroppedDown = true;
        var info = new ComboBoxInfo { Size = Marshal.SizeOf(typeof(ComboBoxInfo)) };
        NativeRect bounds;
        Check(editor != null && editor.DroppedDown && editor.Items.Count > 8 &&
            GetComboBoxInfo(editor.Handle, ref info) && GetWindowRect(info.List, out bounds) &&
            bounds.Bottom - bounds.Top <= editor.ItemHeight * 8 + 8,
            grid.Columns[column].HeaderText + " native dropdown is limited to eight visible options");
        editor.SelectedIndex = editor.Items.Count - 1;
        object last = editor.SelectedItem; editor.DroppedDown = false; grid.EndEdit();
        Check(Equals(grid.Rows[0].Cells[column].Value, last), grid.Columns[column].HeaderText + " choices beyond the first eight remain selectable");
        grid.Rows[0].Cells[column].Value = original;
    }
    private static OsdMetric Metric(OsdConfiguration config, string key, string value) => new OsdMetric(OsdMetricDefinitions.ResolveLabel(config, key), value) { Key = key, Style = config.MetricStyles[key], ValueWidthTemplate = "####", LabelValueSpacingOverride = 15 };
    private static IEnumerable<TreeNode> TreeNodes(TreeNodeCollection nodes)
    {
        foreach (TreeNode node in nodes)
        {
            yield return node;
            foreach (TreeNode child in TreeNodes(node.Nodes)) yield return child;
        }
    }
    private static IList Layouts(OsdOverlay overlay) => (IList)Field(overlay, "_metricLayouts");
    private static IList Parts(object row) => (IList)Field(row, "Parts");
    private static void Render()
    {
        var config = Config();
        config.MetricGroups["Fps"] = "Stats"; config.MetricGroups["FpsOnePercentLow"] = "stats";
        config.MetricStyles["FpsOnePercentLow"].FontFamily = "Segoe UI";
        config.MetricStyles["FpsOnePercentLow"].FontSize = 32;
        config.GroupLabels["Stats"] = "Game";
        var metrics = new[] { Metric(config, "Fps", "144"), Metric(config, "FpsOnePercentLow", "92"), Metric(config, "CpuTemperature", "60") };
        using (var overlay = new OsdOverlay())
        {
            var area = new Rectangle(0, 0, 4000, 2000);
            var rows = OsdMetricDefinitions.Compose(config, metrics);
            Check(rows.Count == 2 && rows.Any(row => row.Members?.Count == 2 && row.Label == "Stats"), "group name supplies the caption regardless of an obsolete saved label");
            overlay.UpdateDisplay(config, rows, area);
            var groupLayout = Layouts(overlay)[1]; // CPU is first in default order
            var parts = Parts(groupLayout);
            Check(parts.Count == 6, "group caption, two label/value pairs and one separator are measured");
            int previousRight = 0; bool ordered = true;
            foreach (object part in parts) { ordered &= (int)Field(part, "X") >= previousRight; previousRight = (int)Field(part, "X") + (int)Field(part, "Width"); }
            Check(ordered, "mixed-font group segments do not overlap");
            object originalLayout = groupLayout;
            object configurationSnapshot = Field(overlay, "_configuration");
            metrics[1] = Metric(config, "FpsOnePercentLow", "6007");
            overlay.UpdateDisplay(config, OsdMetricDefinitions.Compose(config, metrics), area);
            Check(ReferenceEquals(originalLayout, Layouts(overlay)[1]) && ReferenceEquals(configurationSnapshot, Field(overlay, "_configuration")),
                "numeric refresh reuses stable layout and configuration snapshot with mixed fonts");
            config.MetricStyles["FpsOnePercentLow"] = config.MetricStyles["FpsOnePercentLow"].Clone();
            config.MetricStyles["FpsOnePercentLow"].ValueColor = Color.Red.ToArgb();
            metrics[1] = Metric(config, "FpsOnePercentLow", "6007");
            overlay.UpdateDisplay(config, OsdMetricDefinitions.Compose(config, metrics), area);
            Check(ReferenceEquals(originalLayout, Layouts(overlay)[1]), "color-only change repaints without remeasuring");
            Check((string)Field(Parts(Layouts(overlay)[1])[2], "Text") == "144", "FPS slot retains its stable layout text while values refresh independently");
            int builtInGap = (int)Field(Parts(Layouts(overlay)[1])[3], "Width");
            object fixedLayout = Layouts(overlay)[1];
            foreach (string obsolete in new[] { " ", "custom", "" })
            {
                config.GroupSeparator = obsolete; overlay.UpdateDisplay(config, OsdMetricDefinitions.Compose(config, metrics), area);
                Check(ReferenceEquals(fixedLayout, Layouts(overlay)[1]) && (int)Field(Parts(Layouts(overlay)[1])[3], "Width") == builtInGap,
                    "obsolete custom separator does not affect built-in spacing or rebuild geometry: " + obsolete);
            }
            config.LabelOverrides["Fps"] = ""; config.LabelOverrides["FpsOnePercentLow"] = ""; config.GroupLabels["Stats"] = "";
            metrics[0] = Metric(config, "Fps", "144"); metrics[1] = Metric(config, "FpsOnePercentLow", "92");
            overlay.UpdateDisplay(config, OsdMetricDefinitions.Compose(config, metrics), area);
            Check(Parts(Layouts(overlay)[1]).Count == 4, "blank metric labels still retain the caption from the group name");
            overlay.UpdateDisplay(config, OsdMetricDefinitions.Compose(config, metrics), new Rectangle(0, 0, 160, 120));
            Check((float)Field(Layouts(overlay)[1], "Scale") > 0 && overlay.Width <= 160, "narrow screen fits the entire group without dropping member values");
            config.Columns = 2; overlay.UpdateDisplay(config, OsdMetricDefinitions.Compose(config, metrics), area);
            int retainedFontCount = ((IDictionary)Field(overlay, "_metricFonts")).Count;
            config.ColumnsSpacing += 1; overlay.UpdateDisplay(config, OsdMetricDefinitions.Compose(config, metrics), area);
            Check(((IDictionary)Field(overlay, "_metricFonts")).Count == retainedFontCount,
                "geometry-only spacing edits reuse the current metric font contexts");
            var screenshot = Path.Combine(Path.GetTempPath(), "TrayTemps-new-groups-osd.png");
            using (var bitmap = new Bitmap(overlay.Width, overlay.Height)) { overlay.DrawToBitmap(bitmap, overlay.ClientRectangle); bitmap.Save(screenshot); }
            Console.WriteLine("CAPTURE=" + screenshot);
            var before = GC.CollectionCount(0); var timer = Stopwatch.StartNew();
            for (int i = 0; i < 100; i++) overlay.UpdateDisplay(config, OsdMetricDefinitions.Compose(config, metrics), area);
            Console.WriteLine("OSD_UNCHANGED_100_MS=" + timer.Elapsed.TotalMilliseconds.ToString("F2") + " GEN0=" + (GC.CollectionCount(0) - before));
            overlay.Dispose();
            Check(((IDictionary)Field(overlay, "_metricFonts")).Count == 0 && ((IDictionary)Field(overlay, "_unicodeFallbackFonts")).Count == 0,
                "direct overlay disposal releases primary and fallback font caches");
        }

        var alignmentConfig = Config();
        alignmentConfig.Columns = 1;
        alignmentConfig.MetricGroups["CpuTemperature"] = "CPU";
        alignmentConfig.MetricGroups["CpuUsage"] = "CPU";
        alignmentConfig.MetricGroups["GpuTemperature"] = "GPU";
        alignmentConfig.MetricGroups["VramUsage"] = "GPU";
        alignmentConfig.MetricGroups["Fps"] = "Performance";
        alignmentConfig.MetricGroups["FpsOnePercentLow"] = "Performance";
        alignmentConfig.GroupLabels["CPU"] = "CPU:";
        alignmentConfig.GroupLabels["GPU"] = "GPU:";
        alignmentConfig.GroupLabels["Performance"] = "";
        var hardware = new[]
        {
            Metric(alignmentConfig, "CpuTemperature", "60°C"), Metric(alignmentConfig, "CpuUsage", "72%"),
            Metric(alignmentConfig, "GpuTemperature", "55°C"), Metric(alignmentConfig, "VramUsage", "8.00G"),
            Metric(alignmentConfig, "Fps", "144"), Metric(alignmentConfig, "FpsOnePercentLow", "92")
        };
        using (var overlay = new OsdOverlay())
        {
            var rows = OsdMetricDefinitions.Compose(alignmentConfig, hardware);
            overlay.UpdateDisplay(alignmentConfig, rows, new Rectangle(0, 0, 4000, 2000));
            var layouts = Layouts(overlay);
            int[] rightEdges = layouts.Cast<object>().Select(row => Parts(row).Cast<object>()
                .Where(part => !(bool)Field(part, "IsGroupCaption"))
                .Max(part => (int)Field(part, "X") + (int)Field(part, "Width"))).ToArray();
            int cpuIndex = rows.FindIndex(row => row.Key == "Group:CPU");
            int gpuIndex = rows.FindIndex(row => row.Key == "Group:GPU");
            int performanceIndex = rows.FindIndex(row => row.Key == "Group:Performance");
            Check(rows.Count == 3 && rightEdges.Length == 3 &&
                (int)Field(Parts(layouts[cpuIndex])[0], "X") == 0 && (int)Field(Parts(layouts[gpuIndex])[0], "X") == 0,
                "grouped metrics and captions begin at the left and values retain natural order");
            var fpsParts = Parts(layouts[performanceIndex]);
            Check((string)Field(fpsParts[0], "Text") == "Performance" &&
                (int)Field(fpsParts[2], "X") - ((int)Field(fpsParts[1], "X") + (int)Field(fpsParts[1], "Width")) == 15 &&
                (int)Field(fpsParts[5], "X") - ((int)Field(fpsParts[4], "X") + (int)Field(fpsParts[4], "Width")) == 15,
                "a grouped FPS pair uses its group name and keeps both dedicated label/value gaps");

            var clock = Metric(alignmentConfig, "CurrentTime", "01:11");
            overlay.UpdateDisplay(alignmentConfig, new[] { clock }, new Rectangle(0, 0, 4000, 2000));
            var clockParts = Parts(Layouts(overlay)[0]);
            Check((int)Field(clockParts[0], "X") == 0 &&
                (int)Field(clockParts[1], "X") >= (int)Field(clockParts[0], "X") + (int)Field(clockParts[0], "Width"),
                "Current Time follows the same left-to-right flow as every other individual metric");
        }
    }
}
