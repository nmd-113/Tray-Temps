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

        public ColorTempsConfig()
        {
            InitializeComponent();
            if (!EmbeddedFonts.IsDesignTime(this))
                EmbeddedFonts.ApplyTo(this);
        }

        public ColorTempsConfig(MainForm mainForm) : this()
        {
            _mainForm = mainForm;
            if (_mainForm == null || EmbeddedFonts.IsDesignTime(this))
                return;

            ApplyTheme();
            _mainForm.ThemeChanged += MainForm_ThemeChanged;
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            if (!EmbeddedFonts.IsDesignTime(this))
                WindowCornerHelper.ApplyRoundedCorners(Handle);
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams createParams = base.CreateParams;
                if (!EmbeddedFonts.IsDesignTime(this))
                    createParams.ClassStyle |= CsDropShadow;
                return createParams;
            }
        }

        private void ColorTempsConfig_Load(object sender, EventArgs e)
        {
            if (_mainForm == null || EmbeddedFonts.IsDesignTime(this))
                return;

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

        private void TemperatureColor_Click(object sender, EventArgs e)
        {
            if (sender is Button button && colorDialog.ShowDialog() == DialogResult.OK)
                button.BackColor = colorDialog.Color;
        }

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
            _mainForm.ShowTemperatureColorCorners = showCornerMarkers.Checked;
            _mainForm.TemperatureAlertsEnabled = temperatureAlerts.Checked;

            _mainForm.SaveSettings();

            _mainForm.RefreshTemperatureDisplayFromCurrentValues();

            this.Close();
        }


        private void LoadSettings()
        {
            normalTempColor.BackColor = _mainForm.NormalColor;
            warmTempColor.BackColor = _mainForm.WarningColor;
            hotTempColor.BackColor = _mainForm.CriticalColor;
            showCornerMarkers.Checked = _mainForm.ShowTemperatureColorCorners;
            temperatureAlerts.Checked = _mainForm.TemperatureAlertsEnabled;

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
            ThemePalette theme = ThemePalette.Create(light);

            BackColor = theme.WindowBack;
            ForeColor = theme.Text;
            mainPanel.BackColor = theme.SurfaceBack;

            formTitle.ForeColor = theme.TitleText;
            colorsetLabel.ForeColor = theme.MutedText;
            tempsIntervalLabel.ForeColor = theme.MutedText;
            lineLabel.ForeColor = theme.MutedText;
            showCornerMarkers.ForeColor = theme.MutedText;
            temperatureAlerts.ForeColor = theme.MutedText;

            ApplyInputTheme(warmTempMin, theme.InputBack, theme.Text);
            ApplyInputTheme(warmTempMax, theme.InputBack, theme.Text);

            saveBtn.BackColor = theme.Accent;
            saveBtn.ForeColor = theme.AccentButtonText;
            saveBtn.FlatAppearance.BorderColor = theme.Border;

            exitBtn.BackColor = theme.WindowBack;
            exitBtn.ForeColor = theme.TitleText;
            exitBtn.FlatAppearance.BorderColor = theme.WindowBack;
            exitBtn.FlatAppearance.MouseDownBackColor = light ? theme.NavSelected : theme.SurfaceBack;

            ApplyColorButtonTheme(normalTempColor, theme.Border);
            ApplyColorButtonTheme(warmTempColor, theme.Border);
            ApplyColorButtonTheme(hotTempColor, theme.Border);
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
