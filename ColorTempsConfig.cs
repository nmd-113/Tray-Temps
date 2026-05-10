using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Text.Json;

namespace TrayTemps
{
    public partial class ColorTempsConfig : Form
    {
        private MainForm _mainForm;
        private bool dragging = false;
        private Point dragCursorPoint;
        private Point dragFormPoint;

        private const string AppName = "TrayTemps";

        private string SettingsFilePath =>
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                         AppName,
                         "settings.json");

        private bool _updating = false;

        public ColorTempsConfig(MainForm mainForm)
        {
            InitializeComponent();

            _mainForm = mainForm;

            warmTempMin.ValueChanged += NumericRange_ValueChanged;
            warmTempMax.ValueChanged += NumericRange_ValueChanged;
        }

        private void ColorTempsConfig_Load(object sender, EventArgs e)
        {
            LoadSettings();
        }

        // ---------------- DRAG ----------------

        private void ColorTempsConfig_MouseDown(object sender, MouseEventArgs e)
        {
            dragging = true;
            dragCursorPoint = Cursor.Position;
            dragFormPoint = Location;
        }

        private void ColorTempsConfig_MouseMove(object sender, MouseEventArgs e)
        {
            if (!dragging) return;

            Point diff = Point.Subtract(Cursor.Position, new Size(dragCursorPoint));
            Location = Point.Add(dragFormPoint, new Size(diff));
        }

        private void ColorTempsConfig_MouseUp(object sender, MouseEventArgs e)
        {
            dragging = false;
        }

        // ---------------- COLORS ----------------

        private void ApplyColor(Button target)
        {
            if (colorDialog.ShowDialog() == DialogResult.OK)
                target.BackColor = colorDialog.Color;
        }

        private void minTempColor_Click(object sender, EventArgs e)
            => ApplyColor(normalTempColor);

        private void warmTempColor_Click(object sender, EventArgs e)
            => ApplyColor(warmTempColor);

        private void hotTempColor_Click(object sender, EventArgs e)
            => ApplyColor(hotTempColor);

        // ---------------- NUMERIC RANGE ----------------

        private void NumericRange_ValueChanged(object sender, EventArgs e)
        {
            if (_updating) return;

            _updating = true;

            if (warmTempMin.Value > warmTempMax.Value)
                warmTempMax.Value = warmTempMin.Value;

            _updating = false;
        }

        // ---------------- SAVE ----------------

        private void saveBtn_Click(object sender, EventArgs e)
        {
            _mainForm.NormalColor = normalTempColor.BackColor;
            _mainForm.WarningColor = warmTempColor.BackColor;
            _mainForm.CriticalColor = hotTempColor.BackColor;
            _mainForm.WarmTempMin = (int)warmTempMin.Value;
            _mainForm.WarmTempMax = (int)warmTempMax.Value;

            _mainForm.SaveSettings();

            _mainForm.ResetTrayCache();

            this.Close();
        }

        // ---------------- LOAD ----------------

        private void LoadSettings()
        {
            normalTempColor.BackColor = _mainForm.NormalColor;
            warmTempColor.BackColor = _mainForm.WarningColor;
            hotTempColor.BackColor = _mainForm.CriticalColor;

            warmTempMin.Value = Clamp(_mainForm.WarmTempMin, warmTempMin.Minimum, warmTempMin.Maximum);
            warmTempMax.Value = Clamp(_mainForm.WarmTempMax, warmTempMax.Minimum, warmTempMax.Maximum);
        }

        private decimal Clamp(decimal value, decimal min, decimal max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }

        // ---------------- EXIT ----------------

        private void exitBtn_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}