using System;
using System.Drawing;
using System.Windows.Forms;

namespace TrayTemps
{
    public partial class HardwareSelectionConfig : Form
    {
        private readonly MainForm _mainForm;
        private readonly bool _isCpu;
        private bool _loading;
        private bool _dragging;
        private Point _dragCursorPoint;
        private Point _dragFormPoint;
        private const int CsDropShadow = 0x00020000;

        public HardwareSelectionConfig()
        {
            InitializeComponent();
            if (!EmbeddedFonts.IsDesignTime(this))
                EmbeddedFonts.ApplyTo(this);
        }

        public HardwareSelectionConfig(MainForm mainForm, bool isCpu) : this()
        {
            _mainForm = mainForm;
            _isCpu = isCpu;
            if (_mainForm == null || EmbeddedFonts.IsDesignTime(this))
                return;

            _mainForm.ThemeChanged += MainForm_ThemeChanged;
            ApplyTheme();
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

        private void HardwareSelectionConfig_Load(object sender, EventArgs e)
        {
            if (_mainForm == null || EmbeddedFonts.IsDesignTime(this))
                return;

            formTitle.Text = $"Configure {(_isCpu ? "CPU" : "GPU")}";
            hardwareLabel.Text = _isCpu ? "Detected CPUs" : "Detected GPUs";
            LoadHardwareOptions();
        }

        private void LoadHardwareOptions()
        {
            _loading = true;
            hardwareSelect.BeginUpdate();

            try
            {
                hardwareSelect.Items.Clear();
                foreach (string name in _mainForm.GetHardwareSelectionNames(_isCpu))
                    hardwareSelect.Items.Add(name);

                int selectedIndex = _mainForm.GetSelectedHardwareIndex(_isCpu);
                hardwareSelect.SelectedIndex = selectedIndex >= 0 && selectedIndex < hardwareSelect.Items.Count
                    ? selectedIndex
                    : (hardwareSelect.Items.Count > 0 ? 0 : -1);
                hardwareSelect.Enabled = hardwareSelect.Items.Count > 1;
                saveBtn.Enabled = hardwareSelect.SelectedIndex >= 0;
            }
            finally
            {
                hardwareSelect.EndUpdate();
                _loading = false;
            }

            LoadSensorOptions();
        }

        private void LoadSensorOptions()
        {
            _loading = true;
            sensorSelect.BeginUpdate();

            try
            {
                sensorSelect.Items.Clear();
                int hardwareIndex = hardwareSelect.SelectedIndex;

                foreach (MainForm.TemperatureSensorOption option in _mainForm.GetTemperatureSensorOptions(_isCpu, hardwareIndex))
                    sensorSelect.Items.Add(option);

                string selectedIdentifier = _mainForm.GetSelectedTemperatureSensorIdentifier(_isCpu);
                int selectedIndex = FindSensorIndex(selectedIdentifier);
                sensorSelect.SelectedIndex = selectedIndex >= 0 ? selectedIndex : (sensorSelect.Items.Count > 0 ? 0 : -1);
                sensorSelect.Enabled = sensorSelect.Items.Count > 1;
            }
            finally
            {
                sensorSelect.EndUpdate();
                _loading = false;
            }
        }

        private int FindSensorIndex(string sensorIdentifier)
        {
            if (string.IsNullOrWhiteSpace(sensorIdentifier))
                return 0;

            for (int i = 1; i < sensorSelect.Items.Count; i++)
            {
                if (sensorSelect.Items[i] is MainForm.TemperatureSensorOption option &&
                    string.Equals(option.Identifier, sensorIdentifier, StringComparison.OrdinalIgnoreCase))
                    return i;
            }

            return 0;
        }

        private void HardwareSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_loading)
                LoadSensorOptions();
        }

        private void SaveBtn_Click(object sender, EventArgs e)
        {
            string sensorIdentifier = (sensorSelect.SelectedItem as MainForm.TemperatureSensorOption)?.Identifier ?? string.Empty;
            _mainForm.ApplyHardwareSelection(_isCpu, hardwareSelect.SelectedIndex, sensorIdentifier);
            Close();
        }

        private void ExitBtn_Click(object sender, EventArgs e) => Close();

        private void HardwareSelectionConfig_MouseDown(object sender, MouseEventArgs e)
        {
            _dragging = true;
            _dragCursorPoint = Cursor.Position;
            _dragFormPoint = Location;
        }

        private void HardwareSelectionConfig_MouseMove(object sender, MouseEventArgs e)
        {
            if (!_dragging)
                return;

            Point difference = Point.Subtract(Cursor.Position, new Size(_dragCursorPoint));
            Location = Point.Add(_dragFormPoint, new Size(difference));
        }

        private void HardwareSelectionConfig_MouseUp(object sender, MouseEventArgs e) => _dragging = false;

        private void MainForm_ThemeChanged(object sender, EventArgs e) => ApplyTheme();

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            _mainForm.ThemeChanged -= MainForm_ThemeChanged;
            base.OnFormClosed(e);
        }

        private void ApplyTheme()
        {
            bool light = _mainForm != null && _mainForm.IsLightModeEnabled;
            ThemePalette theme = ThemePalette.Create(light);

            BackColor = theme.WindowBack;
            mainPanel.BackColor = theme.SurfaceBack;
            formTitle.ForeColor = theme.TitleText;
            hardwareLabel.ForeColor = theme.MutedText;
            sensorLabel.ForeColor = theme.MutedText;
            hardwareSelect.BackColor = theme.InputBack;
            hardwareSelect.ForeColor = theme.Text;
            sensorSelect.BackColor = theme.InputBack;
            sensorSelect.ForeColor = theme.Text;
            saveBtn.BackColor = theme.Accent;
            saveBtn.ForeColor = theme.AccentButtonText;
            saveBtn.FlatAppearance.BorderColor = theme.Border;
            exitBtn.BackColor = theme.WindowBack;
            exitBtn.ForeColor = theme.TitleText;
            exitBtn.FlatAppearance.BorderColor = theme.WindowBack;
        }
    }
}
