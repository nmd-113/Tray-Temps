using System;
using System.IO;
using System.Text.Json;
using System.Windows.Forms;

namespace TrayTemps
{
    public partial class OsdSettingsDialog
    {
        private void PresetsNavButton_Click(object sender, EventArgs e) => ShowPage(OsdSettingsPage.Presets);

        private void LoadEmbeddedPreset_Click(object sender, EventArgs e)
        {
            if (presetList.SelectedIndex >= 0)
                ApplyPreset(OsdPresets.LoadEmbedded(presetList.SelectedIndex));
        }

        private void ApplyPreset(OsdConfiguration preset)
        {
            var normalized = _mainForm.NormalizeOsdConfiguration(preset);
            normalized.Enabled = _configuration.Enabled;
            normalized.HotkeyEnabled = hotkeyEnabled.Checked;
            normalized.HotkeyModifiers = (int)_hotkeyModifiers;
            normalized.HotkeyKey = (int)_hotkeyKey;
            normalized.LabelOverrides = OsdMetricDefinitions.CreateOverrides(normalized);
            _configurationLoaded = false;
            try
            {
                EndMetricLabelEdit(false);
                appearanceGrid.CancelEdit();
                _metricDraft = normalized;
                _separateItemOrder = OsdItemOrderHelper.Parse(normalized.ItemOrder);
                LoadConfiguration(normalized);
                if (_appearancePopulated) PopulateAppearance();
                ApplyTheme();
            }
            finally { _configurationLoaded = true; }
            PreviewVisualSettings();
        }

        private void LoadPresetFile_Click(object sender, EventArgs e)
        {
            using (var dialog = new OpenFileDialog { Title = "Load OSD preset", Filter = "OSD preset (*.json)|*.json", CheckFileExists = true })
            {
                if (dialog.ShowDialog(this) != DialogResult.OK) return;
                try { ApplyPreset(OsdPresets.LoadFile(dialog.FileName)); }
                catch (Exception error) when (IsPresetFileError(error)) { ShowPresetError(error); }
            }
        }

        private void SavePresetFile_Click(object sender, EventArgs e)
        {
            EndMetricLabelEdit(true);
            if (!TryCommitAppearance() || !ValidateMetricSelection()) return;
            using (var dialog = new SaveFileDialog { Title = "Save OSD preset", Filter = "OSD preset (*.json)|*.json", DefaultExt = "json", AddExtension = true, FileName = "My OSD preset.json", OverwritePrompt = true })
            {
                if (dialog.ShowDialog(this) != DialogResult.OK) return;
                try { OsdPresets.SaveFile(dialog.FileName, CreateConfigurationFromControls()); }
                catch (Exception error) when (IsPresetFileError(error)) { ShowPresetError(error); }
            }
        }

        private static bool IsPresetFileError(Exception error) => error is IOException || error is UnauthorizedAccessException ||
            error is JsonException || error is ArgumentException || error is NotSupportedException;

        private void ShowPresetError(Exception error) => MessageBox.Show(this, "Could not read or save the OSD preset.\n\n" + error.Message,
            "OSD Presets", MessageBoxButtons.OK, MessageBoxIcon.Warning);
    }
}
