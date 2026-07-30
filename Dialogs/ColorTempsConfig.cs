using System;
using System.Drawing;
using System.Windows.Forms;

namespace TrayTemps
{
    public partial class ColorTempsConfig : Form
    {
        private readonly MainForm _mainForm;
        private bool dragging = false;
        private Point dragCursorPoint;
        private Point dragFormPoint;

        private bool _updating = false;
        private const int CsDropShadow = 0x00020000;

        public ColorTempsConfig(MainForm mainForm)
        {
            InitializeComponent();
            EmbeddedFonts.ApplyTo(this);

            _mainForm = mainForm;
            ApplyTheme();
            _mainForm.ThemeChanged += MainForm_ThemeChanged;

            warmTempMin.ValueChanged += NumericRange_ValueChanged;
            warmTempMax.ValueChanged += NumericRange_ValueChanged;
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

        private void ColorTempsConfig_Load(object sender, EventArgs e)
        {
            LoadSettings();
        }

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

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            if (_mainForm != null)
                _mainForm.ThemeChanged -= MainForm_ThemeChanged;

            base.OnFormClosed(e);
        }

        private void MainForm_ThemeChanged(object sender, EventArgs e)
        {
            ApplyTheme();
        }

        private void ApplyColor(Button target)
        {
            if (colorDialog.ShowDialog() == DialogResult.OK)
                target.BackColor = colorDialog.Color;
        }

        private void MinTempColor_Click(object sender, EventArgs e)
            => ApplyColor(normalTempColor);

        private void WarmTempColor_Click(object sender, EventArgs e)
            => ApplyColor(warmTempColor);

        private void HotTempColor_Click(object sender, EventArgs e)
            => ApplyColor(hotTempColor);

        private void NumericRange_ValueChanged(object sender, EventArgs e)
        {
            if (_updating) return;

            _updating = true;

            if (warmTempMin.Value > warmTempMax.Value)
                warmTempMax.Value = warmTempMin.Value;

            _updating = false;
        }

        private void SaveBtn_Click(object sender, EventArgs e)
        {
            _mainForm.NormalColor = normalTempColor.BackColor;
            _mainForm.WarningColor = warmTempColor.BackColor;
            _mainForm.CriticalColor = hotTempColor.BackColor;
            _mainForm.WarmTempMin = ToCelsiusThreshold(warmTempMin.Value);
            _mainForm.WarmTempMax = ToCelsiusThreshold(warmTempMax.Value);

            _mainForm.SaveSettings();

            _mainForm.ResetTrayCache();
            _mainForm.RefreshTemperatureDisplayFromCurrentValues();

            this.Close();
        }


        private void LoadSettings()
        {
            normalTempColor.BackColor = _mainForm.NormalColor;
            warmTempColor.BackColor = _mainForm.WarningColor;
            hotTempColor.BackColor = _mainForm.CriticalColor;

            tempsIntervalLabel.Text = _mainForm.UsesFahrenheit
                ? "Temperature interval (Warm, °F)"
                : "Temperature interval (Warm, °C)";

            decimal maximumDisplayThreshold = _mainForm.UsesFahrenheit ? 482M : 230M;
            warmTempMin.Maximum = maximumDisplayThreshold;
            warmTempMax.Maximum = maximumDisplayThreshold;

            warmTempMin.Value = Clamp(ToDisplayThreshold(_mainForm.WarmTempMin), warmTempMin.Minimum, warmTempMin.Maximum);
            warmTempMax.Value = Clamp(ToDisplayThreshold(_mainForm.WarmTempMax), warmTempMax.Minimum, warmTempMax.Maximum);
        }

        private decimal ToDisplayThreshold(int celsius)
        {
            return _mainForm.UsesFahrenheit
                ? Math.Round((decimal)celsius * 1.8M + 32M, MidpointRounding.AwayFromZero)
                : celsius;
        }

        private int ToCelsiusThreshold(decimal displayValue)
        {
            decimal celsius = _mainForm.UsesFahrenheit
                ? (displayValue - 32M) / 1.8M
                : displayValue;

            return Math.Max(0, Math.Min(230, (int)Math.Round(celsius, MidpointRounding.AwayFromZero)));
        }

        private void ApplyTheme()
        {
            bool light = _mainForm != null && _mainForm.IsLightModeEnabled;

            Color windowBack = light ? Color.FromArgb(218, 226, 238) : Color.FromArgb(21, 21, 21);
            Color surfaceBack = light ? Color.White : Color.FromArgb(40, 40, 40);
            Color inputBack = light ? Color.White : Color.FromArgb(40, 40, 40);
            Color text = light ? Color.FromArgb(31, 41, 55) : Color.LightGray;
            Color titleText = light ? Color.FromArgb(15, 23, 42) : Color.WhiteSmoke;
            Color mutedText = light ? Color.FromArgb(91, 103, 122) : Color.LightGray;
            Color accent = light ? Color.FromArgb(37, 99, 235) : Color.FromArgb(0, 120, 212);
            Color border = light ? Color.FromArgb(210, 218, 230) : Color.FromArgb(30, 30, 30);

            BackColor = windowBack;
            ForeColor = text;
            mainPanel.BackColor = surfaceBack;

            formTitle.ForeColor = titleText;
            colorsetLabel.ForeColor = mutedText;
            tempsIntervalLabel.ForeColor = mutedText;
            lineLabel.ForeColor = mutedText;

            ApplyInputTheme(warmTempMin, inputBack, text);
            ApplyInputTheme(warmTempMax, inputBack, text);

            saveBtn.BackColor = accent;
            saveBtn.ForeColor = Color.White;
            saveBtn.FlatAppearance.BorderColor = border;

            exitBtn.BackColor = windowBack;
            exitBtn.ForeColor = titleText;
            exitBtn.FlatAppearance.BorderColor = windowBack;
            exitBtn.FlatAppearance.MouseDownBackColor = light ? Color.FromArgb(226, 239, 255) : Color.FromArgb(40, 40, 40);

            ApplyColorButtonTheme(normalTempColor, border);
            ApplyColorButtonTheme(warmTempColor, border);
            ApplyColorButtonTheme(hotTempColor, border);
        }

        private static void ApplyInputTheme(NumericUpDown input, Color backColor, Color foreColor)
        {
            input.BackColor = backColor;
            input.ForeColor = foreColor;
        }

        private static void ApplyColorButtonTheme(Button button, Color border)
        {
            button.ForeColor = Color.Black;
            button.FlatAppearance.BorderColor = border;
        }

        private decimal Clamp(decimal value, decimal min, decimal max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }


        private void ExitBtn_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
