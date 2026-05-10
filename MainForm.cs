using LibreHardwareMonitor.Hardware;
using LibreHardwareMonitor.Hardware.Cpu;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TrayTemps
{
    public partial class MainForm : Form
    {
        #region [ Fields and Variables ]

        private bool dragging = false;
        private Point dragCursorPoint;
        private Point dragFormPoint;
        private const string AppName = "TrayTemps";
        private string InstallPath;
        private string SettingsFilePath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), AppName, "settings.json");
        private const int IconSize = 16;

        private Computer _computer;
        private readonly Timer _tempTimer = new Timer();

        private List<IHardware> _cpuHardwares;
        private IHardware _selectedCpuHardware;
        private string _selectedCpuIdentifier;

        private List<IHardware> _gpuHardwares;
        private IHardware _selectedGpuHardware;
        private string _selectedGpuIdentifier;

        private List<IHardware> _storageHardwares;
        private string _selectedStorageIdentifier;

        private int _savedCpuIndex = 0;
        private int _savedGpuIndex = 0;
        private int _savedStorageIndex = 0;

        public int WarmTempMin;
        public int WarmTempMax;

        public Color NormalColor;
        public Color WarningColor;
        public Color CriticalColor;

        private ISensor _cpuTempSensor;
        private ISensor _gpuTempSensor;
        private float _cpuMaxTemp = float.MinValue;
        private float _cpuMinTemp = float.MaxValue;
        private float _gpuMaxTemp = float.MinValue;
        private float _gpuMinTemp = float.MaxValue;

        private string _lastCpuTempText;
        private string _lastGpuTempText;
        private bool _settingsLoaded = false;
        private bool _isInternalCheckChange = false;
        private bool _isShutdownInitiated = false;

        private string _trayFontFamily;
        private float _dpiScale = 1f;

        private Font _trayFont;
        private SolidBrush _cpuBrush;
        private SolidBrush _gpuBrush;

        #endregion

        #region [ P/Invoke Imports ]

        [DllImport("user32.dll")]
        private static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool DestroyIcon(IntPtr hIcon);

        #endregion

        #region [ Form Lifecycle & Window Handling ]

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            appVersion.Text = $"Version: {Application.ProductVersion}";

            using (var g = this.CreateGraphics())
            {
                _dpiScale = g.DpiX / 96f;
            }

            SetDefaultControlValues();
            _ = InitializeHardwareAsync();
            SetupTimer();
            LoadSettings();
            UpdateTrayIcons();
            TempTimer_Tick(this, EventArgs.Empty);
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveSettings();
            ExecuteShutdownSequence();
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);
            UpdateTrayIcons();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (WindowState == FormWindowState.Minimized) Hide();
        }

        protected override void WndProc(ref Message m)
        {
            const int WM_NCHITTEST = 0x84;
            const int HTLEFT = 10, HTRIGHT = 11, HTTOP = 12, HTTOPLEFT = 13;
            const int HTTOPRIGHT = 14, HTBOTTOM = 15, HTBOTTOMLEFT = 16, HTBOTTOMRIGHT = 17;
            const int resizeAreaSize = 10;

            base.WndProc(ref m);

            if (m.Msg == WM_NCHITTEST)
            {
                int x = (int)(m.LParam.ToInt64() & 0xFFFF);
                int y = (int)((m.LParam.ToInt64() >> 16) & 0xFFFF);
                Point cursor = PointToClient(new Point(x, y));

                bool left = cursor.X <= resizeAreaSize;
                bool right = cursor.X >= Width - resizeAreaSize;
                bool top = cursor.Y <= resizeAreaSize;
                bool bottom = cursor.Y >= Height - resizeAreaSize;

                if (left && top) m.Result = (IntPtr)HTTOPLEFT;
                else if (left && bottom) m.Result = (IntPtr)HTBOTTOMLEFT;
                else if (right && top) m.Result = (IntPtr)HTTOPRIGHT;
                else if (right && bottom) m.Result = (IntPtr)HTBOTTOMRIGHT;
                else if (left) m.Result = (IntPtr)HTLEFT;
                else if (right) m.Result = (IntPtr)HTRIGHT;
                else if (top) m.Result = (IntPtr)HTTOP;
                else if (bottom) m.Result = (IntPtr)HTBOTTOM;
            }
        }

        private void MainForm_MouseDown(object sender, MouseEventArgs e)
        {
            dragging = true;
            dragCursorPoint = Cursor.Position;
            dragFormPoint = this.Location;
        }

        private void MainForm_MouseMove(object sender, MouseEventArgs e)
        {
            if (dragging)
            {
                Point diff = Point.Subtract(Cursor.Position, new Size(dragCursorPoint));
                this.Location = Point.Add(dragFormPoint, new Size(diff));
            }
        }

        private void MainForm_MouseUp(object sender, MouseEventArgs e)
        {
            dragging = false;
        }

        #endregion

        #region [ Core Hardware Logic ]

        private async Task InitializeHardwareAsync()
        {
            motherboardDetails.Text = await GetMotherboardNameAsync();
            ramDetails.Text = await GetRamInfoAsync();
            await InitializeHardwareMonitorAsync();
        }

        private Task InitializeHardwareMonitorAsync()
        {
            return Task.Run(() =>
            {
                try
                {
                    _computer = new Computer
                    {
                        IsCpuEnabled = true,
                        IsGpuEnabled = true,
                        IsStorageEnabled = true
                    };
                    _computer.Open();

                    _cpuHardwares = _computer.Hardware.Where(h => h.HardwareType == HardwareType.Cpu).ToList();
                    _gpuHardwares = _computer.Hardware.Where(h => h.HardwareType == HardwareType.GpuAmd || h.HardwareType == HardwareType.GpuNvidia || h.HardwareType == HardwareType.GpuIntel).ToList();
                    _storageHardwares = _computer.Hardware.Where(h => h.HardwareType == HardwareType.Storage).ToList();

                    if (this.IsHandleCreated && !this.IsDisposed)
                    {
                        Invoke((MethodInvoker)delegate
                        {
                            PopulateHardwareSelector(cpuIndexSelect, _cpuHardwares, _savedCpuIndex, cpuModel, "CPU");
                            PopulateHardwareSelector(gpuIndexSelect, _gpuHardwares, _savedGpuIndex, gpuModel, "GPU");
                            PopulateHardwareSelector(storageIndexSelect, _storageHardwares, _savedStorageIndex, storageDetails, "Disk");
                        });
                    }
                }
                catch (Exception ex)
                {
                    if (this.IsHandleCreated && !this.IsDisposed)
                    {
                        Invoke((MethodInvoker)delegate {
                            MessageBox.Show($"Eroare la inițializarea senzorilor hardware. Asigură-te că rulezi ca Administrator!\n\n{ex.Message}", "Eroare Hardware", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        });
                    }
                }
            });
        }

        private void SetupTimer()
        {
            UpdateTimerInterval();
            _tempTimer.Tick += TempTimer_Tick;
            _tempTimer.Start();
        }

        private void UpdateTimerInterval()
        {
            _tempTimer.Interval = Math.Max(100, (int)(refreshValue.Value * 1000));
        }

        private async void TempTimer_Tick(object sender, EventArgs e)
        {
            _tempTimer.Stop();

            try
            {
                await Task.Run(() =>
                {
                    _selectedCpuHardware?.Update();
                    _selectedGpuHardware?.Update();
                });

                float? cpuTemp = _cpuTempSensor?.Value;
                float? gpuTemp = _gpuTempSensor?.Value;

                UpdateTemperatures(cpuTemp, gpuTemp);
                UpdateAllTrayIcons(cpuTemp, gpuTemp);
            }
            finally
            {
                if (IsHandleCreated && !_isShutdownInitiated)
                {
                    _tempTimer.Start();
                }
            }
        }

        #endregion

        #region [ UI & Tray Icon Updates ]

        private void ShowWindow()
        {
            Show();
            WindowState = FormWindowState.Normal;
            UpdateTrayIcons();
        }

        private void UpdateTrayIcons()
        {
            bool cpuChecked = enableCpuTray.Checked;
            bool gpuChecked = enableGpuTray.Checked;

            cpuTrayIcon.Visible = cpuChecked;
            gpuTrayIcon.Visible = gpuChecked;

            bool isHidden = !Visible || WindowState == FormWindowState.Minimized;
            NotifyIcon.Visible = isHidden && !cpuChecked && !gpuChecked;
        }

        private void UpdateTemperatures(float? cpuTemp, float? gpuTemp)
        {
            string unit = GetUnit();

            if (cpuTemp.HasValue)
            {
                float temp = cpuTemp.Value;
                if (temp < _cpuMinTemp) _cpuMinTemp = temp;
                if (temp > _cpuMaxTemp) _cpuMaxTemp = temp;

                cpuTempCur.Text = $"{GetDisplayTemp(temp):F0}{unit}";
                cpuTempMin.Text = $"{GetDisplayTemp(_cpuMinTemp):F0}{unit}";
                cpuTempMax.Text = $"{GetDisplayTemp(_cpuMaxTemp):F0}{unit}";
            }
            else { cpuTempCur.Text = cpuTempMin.Text = cpuTempMax.Text = "N/A"; }

            if (gpuTemp.HasValue)
            {
                float temp = gpuTemp.Value;
                if (temp < _gpuMinTemp) _gpuMinTemp = temp;
                if (temp > _gpuMaxTemp) _gpuMaxTemp = temp;

                gpuTempCur.Text = $"{GetDisplayTemp(temp):F0}{unit}";
                gpuTempMin.Text = $"{GetDisplayTemp(_gpuMinTemp):F0}{unit}";
                gpuTempMax.Text = $"{GetDisplayTemp(_gpuMaxTemp):F0}{unit}";
            }
            else { gpuTempCur.Text = gpuTempMin.Text = gpuTempMax.Text = "N/A"; }
        }

        private void UpdateAllTrayIcons(float? cpuTemp, float? gpuTemp)
        {
            string unit = GetUnit();

            if (colortempsEnable.Checked)
            {
                if (cpuTemp.HasValue)
                {
                    float val = GetDisplayTemp(cpuTemp.Value);
                    if (val < WarmTempMin) _cpuBrush.Color = NormalColor;
                    else if (val <= WarmTempMax) _cpuBrush.Color = WarningColor;
                    else _cpuBrush.Color = CriticalColor;
                }
                if (gpuTemp.HasValue)
                {
                    float val = GetDisplayTemp(gpuTemp.Value);
                    if (val < WarmTempMin) _gpuBrush.Color = NormalColor;
                    else if (val <= WarmTempMax) _gpuBrush.Color = WarningColor;
                    else _gpuBrush.Color = CriticalColor;
                }
            }
            else
            {
                _cpuBrush.Color = cpuColorValue.BackColor;
                _gpuBrush.Color = gpuColorValue.BackColor;
            }

            // ---------------------------------

            if (singleIconTray.Checked && enableCpuTray.Checked && enableGpuTray.Checked)
            {
                cpuTrayIcon.Visible = true;
                gpuTrayIcon.Visible = false;
                UpdateCombinedTrayIcon(cpuTrayIcon, GetDisplayTemp(cpuTemp ?? 0), GetDisplayTemp(gpuTemp ?? 0));
                return;
            }

            if (enableCpuTray.Checked && cpuTemp.HasValue)
            {
                UpdateSingleTrayIcon(cpuTrayIcon, GetDisplayTemp(cpuTemp.Value), ref _lastCpuTempText, _cpuBrush);
            }

            if (enableGpuTray.Checked && gpuTemp.HasValue)
            {
                UpdateSingleTrayIcon(gpuTrayIcon, GetDisplayTemp(gpuTemp.Value), ref _lastGpuTempText, _gpuBrush);
            }

            if (!enableCpuTray.Checked && !enableGpuTray.Checked)
            {
                string cpuHover = cpuTemp.HasValue ? $"{GetDisplayTemp(cpuTemp.Value):F0}{unit}" : "N/A";
                string gpuHover = gpuTemp.HasValue ? $"{GetDisplayTemp(gpuTemp.Value):F0}{unit}" : "N/A";
                NotifyIcon.Text = $"CPU: {cpuHover} | GPU: {gpuHover}";
            }
            else
            {
                NotifyIcon.Text = AppName;
            }
        }

        private void UpdateCombinedTrayIcon(NotifyIcon icon, float cpuTemp, float gpuTemp)
        {
            string cacheKey = $"{cpuTemp:F0}_{gpuTemp:F0}_{_cpuBrush.Color.ToArgb()}_{_gpuBrush.Color.ToArgb()}";

            if (cacheKey != _lastCpuTempText)
            {
                Icon oldIcon = icon.Icon;
                icon.Icon = CreateCombinedTempIcon($"{cpuTemp:F0}", $"{gpuTemp:F0}");
                if (oldIcon != null)
                {
                    DestroyIcon(oldIcon.Handle);
                    oldIcon.Dispose();
                }

                _lastCpuTempText = cacheKey;
            }
        }

        private void UpdateSingleTrayIcon(NotifyIcon icon, float temp, ref string lastText, SolidBrush brush)
        {
            string cacheKey = $"{temp:F0}_{brush.Color.ToArgb()}";

            if (cacheKey != lastText)
            {
                Icon oldIcon = icon.Icon;
                icon.Icon = CreateTempIcon($"{temp:F0}", brush);
                if (oldIcon != null)
                {
                    DestroyIcon(oldIcon.Handle);
                    oldIcon.Dispose();
                }

                lastText = cacheKey;
            }
        }

        private Icon CreateTempIcon(string text, SolidBrush brush)
        {
            int size = Math.Max(IconSize, (int)(IconSize * _dpiScale));
            float maxAllowedSize = size;

            using (var bmp = new Bitmap(size, size))
            using (var g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.Transparent);
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.TextRenderingHint = TextRenderingHint.AntiAlias;

                using (var path = new GraphicsPath())
                {
                    path.AddString(
                        text,
                        _trayFont.FontFamily,
                        (int)_trayFont.Style,
                        g.DpiY * _trayFont.SizeInPoints / 72,
                        new Point(0, 0),
                        StringFormat.GenericTypographic);

                    RectangleF bounds = path.GetBounds();

                    float scale = 1.0f;
                    if (bounds.Width > maxAllowedSize || bounds.Height > maxAllowedSize)
                    {
                        scale = Math.Min(maxAllowedSize / bounds.Width, maxAllowedSize / bounds.Height);
                    }

                    float x = (size / 2f) - (bounds.Width * scale / 2f) - (bounds.X * scale);
                    float y = (size / 2f) - (bounds.Height * scale / 2f) - (bounds.Y * scale);

                    g.TranslateTransform(x, y);
                    g.ScaleTransform(scale, scale);

                    g.FillPath(brush, path);
                }

                IntPtr hIcon = bmp.GetHicon();
                Icon icon = (Icon)Icon.FromHandle(hIcon).Clone();
                DestroyIcon(hIcon);
                return icon;
            }
        }

        private Icon CreateCombinedTempIcon(string cpuText, string gpuText)
        {
            const int baseSize = 16;

            float dpiScale = Graphics.FromHwnd(IntPtr.Zero).DpiX / 96f;

            int size = baseSize;

            using (var bmp = new Bitmap(size, size, PixelFormat.Format32bppArgb))
            using (var g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.Transparent);

                g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.CompositingQuality = CompositingQuality.HighQuality;

                float fontSize = (size * 0.55f) * dpiScale;

                using (var font = new Font("Consolas", fontSize, FontStyle.Regular, GraphicsUnit.Pixel))
                {
                    var flags = TextFormatFlags.HorizontalCenter |
                                TextFormatFlags.VerticalCenter |
                                TextFormatFlags.NoPadding;

                    var cpuRect = new Rectangle(0, 0, size, size / 2);
                    var gpuRect = new Rectangle(0, size / 2, size, size / 2);

                    TextRenderer.DrawText(g, cpuText, font, cpuRect,
                        _cpuBrush.Color, Color.Transparent, flags);

                    TextRenderer.DrawText(g, gpuText, font, gpuRect,
                        _gpuBrush.Color, Color.Transparent, flags);
                }

                IntPtr hIcon = bmp.GetHicon();
                Icon icon = (Icon)Icon.FromHandle(hIcon).Clone();
                DestroyIcon(hIcon);
                return icon;
            }
        }

        private void CacheDisplaySettings()
        {
            _trayFontFamily = fontFamilyValue.Text.Trim();

            int iconPixelSize = Math.Max(IconSize, (int)(IconSize * _dpiScale));

            float percentage = (float)iconsizeValue.Value / 100f;
            float calculatedFontSize = iconPixelSize * percentage;

            _trayFont?.Dispose();
            _trayFont = new Font(_trayFontFamily, calculatedFontSize, FontStyle.Bold, GraphicsUnit.Pixel);

            _cpuBrush?.Dispose();
            _cpuBrush = new SolidBrush(cpuColorValue.BackColor);

            _gpuBrush?.Dispose();
            _gpuBrush = new SolidBrush(gpuColorValue.BackColor);
        }

        #endregion

        #region [ Settings Management ]

        private void SetDefaultControlValues()
        {
            refreshValue.Value = 0.50M;
            iconsizeValue.Value = 75;
            cpuColorValue.BackColor = Color.Aqua;
            gpuColorValue.BackColor = Color.Gold;
            fontFamilyValue.SelectedIndex = 0;

            WarmTempMin = 60;
            WarmTempMax = 80;
            NormalColor = Color.FromArgb(-1);
            WarningColor = Color.FromArgb(-256);
            CriticalColor = Color.FromArgb(-65536);
        }

        public void SaveSettings()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(SaveSettings));
                return;
            }

            try
            {
                var settings = new AppSettings
                {
                    Autostart = autostartInstall.Checked,
                    TempsFahrenheit = tempsFahrenheit.Checked,
                    SingleIconTray = singleIconTray.Checked,
                    CpuTrayIcon = enableCpuTray.Checked,
                    GpuTrayIcon = enableGpuTray.Checked,
                    TempBasedIconColor = colortempsEnable.Checked,
                    UpdateInterval = refreshValue.Value,
                    MinWarmTemp = WarmTempMin,
                    MaxWarmTemp = WarmTempMax,
                    NormalTempColor = NormalColor.ToArgb(),
                    WarmTempColor = WarningColor.ToArgb(),
                    HotTempColor = CriticalColor.ToArgb(),
                    FontFamily = fontFamilyValue.SelectedIndex,
                    CpuColor = cpuColorValue.BackColor.ToArgb(),
                    GpuColor = gpuColorValue.BackColor.ToArgb(),
                    IconSize = (int)iconsizeValue.Value,
                    CpuIndex = cpuIndexSelect.SelectedIndex,
                    GpuIndex = gpuIndexSelect.SelectedIndex,
                    StorageIndex = storageIndexSelect.SelectedIndex,
                    InstallFolder = InstallPath
                };

                if (this.WindowState == FormWindowState.Normal)
                {
                    settings.WindowWidth = this.Width;
                    settings.WindowHeight = this.Height;
                    settings.WindowX = this.Location.X;
                    settings.WindowY = this.Location.Y;
                }

                string directory = Path.GetDirectoryName(SettingsFilePath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                var options = new System.Text.Json.JsonSerializerOptions { WriteIndented = true };
                string json = System.Text.Json.JsonSerializer.Serialize(settings, options);

                using (var fs = new FileStream(SettingsFilePath, FileMode.Create, FileAccess.Write, FileShare.None))
                using (var sw = new StreamWriter(fs))
                {
                    sw.Write(json);
                    sw.Flush();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Eroare critică la salvarea setărilor:\n{ex.Message}", "Save Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadSettings()
        {
            try
            {
                if (!File.Exists(SettingsFilePath))
                {
                    this.CenterToScreen();
                    SetDefaultControlValues();
                    _settingsLoaded = true;
                    return;
                }

                string json = File.ReadAllText(SettingsFilePath);
                var settings = System.Text.Json.JsonSerializer.Deserialize<AppSettings>(json);

                if (settings != null)
                {
                    if (settings.WindowWidth > 0)
                    {
                        this.Size = new Size(settings.WindowWidth, settings.WindowHeight);
                        if (settings.WindowX != -1) this.Location = new Point(settings.WindowX, settings.WindowY);
                    }

                    autostartInstall.Checked = settings.Autostart;
                    tempsFahrenheit.Checked = settings.TempsFahrenheit;
                    singleIconTray.Checked = settings.SingleIconTray;
                    enableCpuTray.Checked = settings.CpuTrayIcon;
                    enableGpuTray.Checked = settings.GpuTrayIcon;
                    colortempsEnable.Checked = settings.TempBasedIconColor;
                    refreshValue.Value = settings.UpdateInterval;
                    fontFamilyValue.SelectedIndex = (settings.FontFamily < fontFamilyValue.Items.Count) ? settings.FontFamily : 0;
                    cpuColorValue.BackColor = Color.FromArgb(settings.CpuColor);
                    gpuColorValue.BackColor = Color.FromArgb(settings.GpuColor);
                    iconsizeValue.Value = settings.IconSize;
                    WarmTempMin = settings.MinWarmTemp;
                    WarmTempMax = settings.MaxWarmTemp;
                    NormalColor = Color.FromArgb(settings.NormalTempColor);
                    WarningColor = Color.FromArgb(settings.WarmTempColor);
                    CriticalColor = Color.FromArgb(settings.HotTempColor);

                    _savedCpuIndex = settings.CpuIndex;
                    _savedGpuIndex = settings.GpuIndex;
                    _savedStorageIndex = settings.StorageIndex;

                    InstallPath = settings.InstallFolder;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to load settings: {ex.Message}");
                SetDefaultControlValues();
            }
            finally
            {
                singleIconTray_CheckedChanged(this, EventArgs.Empty);
                colortempsEnable_CheckedChanged(this, EventArgs.Empty);
                _settingsLoaded = true;
            }
        }

        #endregion

        #region [ Event Handlers: UI Actions ]

        private void homeBtn_Click(object sender, EventArgs e)
        {
            mainTabControl.SelectedIndex = 0;
        }

        private void settingsBtn_Click(object sender, EventArgs e)
        {
            mainTabControl.SelectedIndex = 1;
        }

        private void aboutBtn_Click(object sender, EventArgs e)
        {
            mainTabControl.SelectedIndex = 2;
        }

        private void exitBtn_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(this, "Are you sure you want to exit TrayTemps?\nClick \"No\" to hide the app to tray.", "Exit Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
            {
                Hide();
                UpdateTrayIcons();
            }
            else
            {
                this.Close();
            }
        }

        private void minimizeBtn_Click(object sender, EventArgs e)
        {
            Hide();
            UpdateTrayIcons();
        }

        private void donatePic_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = "https://revolut.me/nmd113",
                UseShellExecute = true
            });
        }

        private void GithubLink_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = "https://github.com/nmd-113/Tray-Temps",
                UseShellExecute = true
            });
        }

        private void ShowForm_Click(object sender, EventArgs e)
        {
            ShowWindow();
        }

        private void ExitForm_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void NotifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ShowWindow();
        }

        private void gpuTrayIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ShowWindow();
        }

        private void cpuTrayIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ShowWindow();
        }

        #endregion

        #region [ Event Handlers: Settings & Controls ]

        private void ComboResize(object sender, EventArgs e)
        {
            this.SuspendLayout();
            int h = ramDetails.Height;
            cpuIndexSelect.ItemHeight = h;
            gpuIndexSelect.ItemHeight = h;
            storageIndexSelect.ItemHeight = h;
            this.ResumeLayout(true);
        }

        private void Setting_CheckedChanged(object sender, EventArgs e)
        {
            if (sender is CheckBox chk)
            {
                UpdateTrayIcons();
                if (chk.Name == nameof(enableCpuTray)) _lastCpuTempText = null;
                if (chk.Name == nameof(enableGpuTray)) _lastGpuTempText = null;
            }
        }

        private void Setting_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (sender is ComboBox cmb)
            {
                CacheDisplaySettings();
                _lastCpuTempText = null;
                _lastGpuTempText = null;
                TempTimer_Tick(this, EventArgs.Empty);
            }
        }

        private void colortempsEnable_CheckedChanged(object sender, EventArgs e)
        {
            bool useTempColors = colortempsEnable.Checked;

            cpuColorValue.Enabled = !useTempColors;
            gpuColorValue.Enabled = !useTempColors;

            colortempsConfig.Enabled = useTempColors;

            if (_settingsLoaded)
            {
                CacheDisplaySettings();
                ResetTrayCache();
                TempTimer_Tick(this, EventArgs.Empty);
            }
        }

        private void singleIconTray_CheckedChanged(object sender, EventArgs e)
        {
            if (singleIconTray.Checked)
            {
                iconsizeValue.Enabled = false;
                fontFamilyValue.Enabled = false;
            }
            else
            {
                iconsizeValue.Enabled = true;
                fontFamilyValue.Enabled = true;
                if (enableGpuTray.Checked)
                {
                    gpuTrayIcon.Visible = true;
                }
            }
        }

        private void colortempsConfig_Click(object sender, EventArgs e)
        {
            ColorTempsConfig cfg = new ColorTempsConfig(this);
            cfg.Show();
        }

        private void RefreshValue_ValueChanged(object sender, EventArgs e)
        {
            UpdateTimerInterval();
        }

        private void IconsizeValue_ValueChanged(object sender, EventArgs e)
        {
            CacheDisplaySettings();
            _lastCpuTempText = null;
            _lastGpuTempText = null;
            TempTimer_Tick(this, EventArgs.Empty);
        }

        private void CpuIndexSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cpuIndexSelect.SelectedIndex < 0 || _cpuHardwares == null || cpuIndexSelect.SelectedIndex >= _cpuHardwares.Count) return;

            _selectedCpuHardware = _cpuHardwares[cpuIndexSelect.SelectedIndex];
            cpuModel.Text = _selectedCpuHardware.Name;
            cpuName.Text = _selectedCpuHardware.Name;

            _cpuTempSensor = _selectedCpuHardware.Sensors.FirstOrDefault(s => s.SensorType == SensorType.Temperature && s.Name.Contains("Package"))
                             ?? _selectedCpuHardware.Sensors.FirstOrDefault(s => s.SensorType == SensorType.Temperature);

            _selectedCpuIdentifier = _selectedCpuHardware.Identifier.ToString();

            _cpuMinTemp = float.MaxValue;
            _cpuMaxTemp = float.MinValue;
            _lastCpuTempText = null;
            TempTimer_Tick(this, EventArgs.Empty);
        }

        private void GpuIndexSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (gpuIndexSelect.SelectedIndex < 0 || _gpuHardwares == null || gpuIndexSelect.SelectedIndex >= _gpuHardwares.Count) return;

            _selectedGpuHardware = _gpuHardwares[gpuIndexSelect.SelectedIndex];
            gpuModel.Text = _selectedGpuHardware.Name;
            gpuName.Text = _selectedGpuHardware.Name;

            _gpuTempSensor = _selectedGpuHardware.Sensors
                .Where(s => s.SensorType == SensorType.Temperature)
                .FirstOrDefault(s => s.Name.Contains("Core") || s.Name.Contains("Package") || s.Name.Equals("GPU", StringComparison.OrdinalIgnoreCase))
                ?? _selectedGpuHardware.Sensors.FirstOrDefault(s => s.SensorType == SensorType.Temperature);

            _selectedGpuIdentifier = _selectedGpuHardware.Identifier.ToString();

            _gpuMinTemp = float.MaxValue;
            _gpuMaxTemp = float.MinValue;
            _lastGpuTempText = null;
            TempTimer_Tick(this, EventArgs.Empty);
        }

        private void storageIndexSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (storageIndexSelect.SelectedIndex >= 0 && storageIndexSelect.SelectedIndex < _storageHardwares.Count)
            {
                var selectedDrive = _storageHardwares[storageIndexSelect.SelectedIndex];
                _selectedStorageIdentifier = selectedDrive.Identifier.ToString();

                storageDetails.Text = selectedDrive.Name;
            }
        }

        private void cpuColorValue_Click(object sender, EventArgs e)
        {
            using (ColorDialog cd = new ColorDialog())
            {
                cd.Color = cpuColorValue.BackColor;
                if (cd.ShowDialog() == DialogResult.OK)
                {
                    cpuColorValue.BackColor = cd.Color;

                    CacheDisplaySettings();
                    _lastCpuTempText = null;
                    TempTimer_Tick(this, EventArgs.Empty);
                }
            }
        }

        private void gpuColorValue_Click(object sender, EventArgs e)
        {
            using (ColorDialog cd = new ColorDialog())
            {
                cd.Color = gpuColorValue.BackColor;
                if (cd.ShowDialog() == DialogResult.OK)
                {
                    gpuColorValue.BackColor = cd.Color;

                    CacheDisplaySettings();
                    _lastGpuTempText = null;
                    TempTimer_Tick(this, EventArgs.Empty);
                }
            }
        }

        #endregion

        #region [ Helper Methods ]

        private float GetDisplayTemp(float celsius) => tempsFahrenheit.Checked ? (celsius * 1.8f) + 32 : celsius;
        private string GetUnit() => tempsFahrenheit.Checked ? "°F" : "°C";

        public void ResetTrayCache()
        {
            _lastCpuTempText = null;
            _lastGpuTempText = null;
        }

        private Task<string> GetMotherboardNameAsync()
        {
            return Task.Run(() =>
            {
                try
                {
                    using (var searcher = new ManagementObjectSearcher("select Manufacturer, Product from Win32_BaseBoard"))
                    using (var collection = searcher.Get())
                    {
                        foreach (ManagementObject obj in collection)
                        {
                            using (obj)
                            {
                                string manufacturer = obj["Manufacturer"]?.ToString().Trim() ?? "";
                                string product = obj["Product"]?.ToString().Trim() ?? "";
                                string fullName = $"{manufacturer} {product}".Trim();
                                return string.IsNullOrEmpty(fullName) ? "Unknown Motherboard" : fullName;
                            }
                        }
                    }
                }
                catch { }
                return "Unknown Motherboard";
            });
        }

        private Task<string> GetRamInfoAsync()
        {
            return Task.Run(() =>
            {
                try
                {
                    using (var searcher = new ManagementObjectSearcher("select Capacity, SMBIOSMemoryType, ConfiguredClockSpeed from Win32_PhysicalMemory"))
                    using (var collection = searcher.Get())
                    {
                        var individualCapacities = new List<long>();
                        uint memoryType = 0;
                        uint speed = 0;

                        foreach (ManagementObject stick in collection)
                        {
                            using (stick)
                            {
                                individualCapacities.Add(Convert.ToInt64(stick["Capacity"]));
                                if (memoryType == 0) memoryType = Convert.ToUInt32(stick["SMBIOSMemoryType"]);
                                if (speed == 0) speed = Convert.ToUInt32(stick["ConfiguredClockSpeed"]);
                            }
                        }

                        if (individualCapacities.Count == 0) return "Unknown RAM";

                        long totalCapacityGB = individualCapacities.Sum() / (1024 * 1024 * 1024);
                        string configString = FormatRamConfiguration(individualCapacities);
                        string typeString = GetMemoryTypeString(memoryType);

                        return $"{totalCapacityGB}GB {configString} {typeString} {speed}MHz";
                    }
                }
                catch { return "Unknown RAM"; }
            });
        }

        private string FormatRamConfiguration(List<long> capacities)
        {
            if (capacities == null || capacities.Count == 0) return "";

            var stickGroups = capacities.GroupBy(c => c / (1024 * 1024 * 1024))
                                        .Select(g => new { CapacityGB = g.Key, Count = g.Count() })
                                        .OrderByDescending(g => g.CapacityGB);

            string config = string.Join(" + ", stickGroups.Select(g => $"{g.Count}x{g.CapacityGB}GB"));
            return $"({config})";
        }

        private string GetMemoryTypeString(uint memoryTypeCode)
        {
            switch (memoryTypeCode)
            {
                case 20: return "DDR";
                case 21: return "DDR2";
                case 24: return "DDR3";
                case 26: return "DDR4";
                case 34: return "DDR5";
                default: return "RAM";
            }
        }

        private void CreateShortcutOnDesktop(string targetPath)
        {
            try
            {
                string deskPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                string shortcutPath = Path.Combine(deskPath, $"{AppName}.lnk");

                Type shellType = Type.GetTypeFromProgID("WScript.Shell");
                dynamic shell = Activator.CreateInstance(shellType);
                dynamic shortcut = shell.CreateShortcut(shortcutPath);
                shortcut.TargetPath = targetPath;
                shortcut.WorkingDirectory = Path.GetDirectoryName(targetPath);
                shortcut.Description = "Launch TrayTemps";
                shortcut.Save();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Could not create shortcut: {ex.Message}");
            }
        }

        private void RunProcessAndWait(string fileName, string arguments)
        {
            var psi = new ProcessStartInfo(fileName, arguments)
            {
                UseShellExecute = true,
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden
            };
            try
            {
                Process.Start(psi)?.WaitForExit();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to run process '{fileName}': {ex.Message}");
                MessageBox.Show($"An error occurred while running a required command:\n{ex.Message}", "Execution Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PopulateHardwareSelector(ComboBox selector, List<IHardware> hardwareList, int savedIndex, Label nameLabel, string hardwareType)
        {
            selector.Items.Clear();
            if (hardwareList != null && hardwareList.Any())
            {
                for (int i = 0; i < hardwareList.Count; i++)
                    selector.Items.Add(i);

                if (savedIndex >= 0 && savedIndex < hardwareList.Count)
                {
                    selector.SelectedIndex = savedIndex;
                }
                else
                {
                    selector.SelectedIndex = 0;
                }

                selector.Enabled = hardwareList.Count > 1;
            }
            else
            {
                nameLabel.Text = $"No {hardwareType} found";
                selector.Enabled = false;
            }
        }

        #endregion

        #region [ Installation & Cleanup ]

        private void ExecuteShutdownSequence()
        {
            if (_isShutdownInitiated) return;
            _isShutdownInitiated = true;

            _tempTimer.Stop();

            if (cpuTrayIcon != null) { cpuTrayIcon.Icon?.Dispose(); cpuTrayIcon.Dispose(); }
            if (gpuTrayIcon != null) { gpuTrayIcon.Icon?.Dispose(); gpuTrayIcon.Dispose(); }
            if (NotifyIcon != null) { NotifyIcon.Icon?.Dispose(); NotifyIcon.Dispose(); }

            _trayFont?.Dispose();
            _cpuBrush?.Dispose();
            _gpuBrush?.Dispose();

            Task.Run(() => ServiceManager.StopServiceAsync("R0TrayTemps"));
            _computer?.Close();
            _tempTimer.Dispose();
        }

        private async void AutostartInstall_CheckedChanged(object sender, EventArgs e)
        {
            if (!_settingsLoaded || _isInternalCheckChange) return;

            var control = (CheckBox)sender;
            control.Enabled = false;

            try
            {
                if (control.Checked)
                    await HandleAutostartEnable();
                else
                    await HandleAutostartDisable();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                RevertCheckbox(control, !control.Checked);
            }
            finally
            {
                if (control.IsHandleCreated)
                    control.Enabled = true;
            }
        }

        private async Task HandleAutostartEnable()
        {
            var result = MessageBox.Show(
                this, "Add app to run silently at Windows startup?", "Startup",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result != DialogResult.Yes)
            {
                RevertCheckbox(autostartInstall, false);
                return;
            }

            SaveSettings();
            await InstallAndRestartAsync();

            if (string.IsNullOrEmpty(InstallPath))
                RevertCheckbox(autostartInstall, false);
        }

        private async Task HandleAutostartDisable()
        {
            var result = MessageBox.Show(this, "Remove installed app, shortcut, and startup entry?\n\nYes = Remove all\nNo = Remove only startup entry\nCancel = Do nothing",
                                        "Confirm Remove", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
            switch (result)
            {
                case DialogResult.Yes:
                    SaveSettings();
                    UninstallAndExit();
                    break;
                case DialogResult.No:
                    SaveSettings();
                    await RemoveStartupTaskAsync();
                    MessageBox.Show(this, "Startup entry removed.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    break;
                case DialogResult.Cancel:
                    RevertCheckbox(autostartInstall, true);
                    break;
            }
        }

        private void RevertCheckbox(CheckBox chk, bool state)
        {
            _isInternalCheckChange = true;
            chk.Checked = state;
            _isInternalCheckChange = false;
        }

        private Task InstallAndRestartAsync()
        {
            using (var fbd = new FolderBrowserDialog())
            {
                fbd.Description = "Select installation folder for TrayTemps\n(Example: Program Files)";
                fbd.ShowNewFolderButton = true;

                string programFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
                if (Directory.Exists(programFiles))
                    fbd.SelectedPath = programFiles;

                if (fbd.ShowDialog(this) != DialogResult.OK || string.IsNullOrWhiteSpace(fbd.SelectedPath))
                    return Task.CompletedTask;

                InstallPath = Path.Combine(fbd.SelectedPath, AppName);

                try
                {
                    Directory.CreateDirectory(InstallPath);
                    string testFile = Path.Combine(InstallPath, "test.tmp");
                    File.WriteAllText(testFile, "test");
                    File.Delete(testFile);
                }
                catch
                {
                    MessageBox.Show("Cannot write to selected folder. Choose another or run as administrator.", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return Task.CompletedTask;
                }
            }

            string destExe = Path.Combine(InstallPath, "TrayTemps.exe");
            string currentExePath = Application.ExecutablePath;

            return Task.Run(() =>
            {
                Directory.CreateDirectory(InstallPath);

                string sourceFolder = AppDomain.CurrentDomain.BaseDirectory;

                File.Copy(currentExePath, destExe, true);

                SaveSettings();

                string arguments = $"/Create /F /RL HIGHEST /SC ONLOGON /TN \"{AppName}\" /TR \"\\\"{destExe}\\\" -silent\"";
                RunProcessAndWait("schtasks", arguments);

                System.Threading.Thread.Sleep(500);

                string arguments2 = $"-NoProfile -ExecutionPolicy Bypass -Command \"Set-ScheduledTask -TaskName '{AppName}' -Settings (New-ScheduledTaskSettingsSet -AllowStartIfOnBatteries -DontStopIfGoingOnBatteries)\"";

                try
                {
                    RunProcessAndWait("powershell", arguments2);
                }
                catch
                {
                    // Ignore specific powershell errors
                }

                CreateShortcutOnDesktop(destExe);

                Invoke(new Action(() =>
                {
                    MessageBox.Show(this, "TrayTemps has been installed successfully.\nRestart it from the desktop shortcut.", "Installation Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ExecuteShutdownSequence();
                    Close();
                }));
            });
        }

        private void UninstallAndExit()
        {
            if (string.IsNullOrEmpty(InstallPath) || !Directory.Exists(InstallPath))
            {
                MessageBox.Show(this, "Installation folder not found.", "Uninstall Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string shortcutPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), $"{AppName}.lnk");
            string batPath = Path.Combine(Path.GetTempPath(), "DeleteTrayTemps.bat");

            string script = $@"@echo off
setlocal
cd /d ""%~dp0""

schtasks /Delete /TN ""{AppName}"" /F > nul 2>&1

set ""install_folder={InstallPath}""
set ""settings_folder=%AppData%\{AppName}""
set attempts=0

:loop
if %attempts% GEQ 15 goto cleanup
if not exist ""%install_folder%"" goto settings

rmdir /s /q ""%install_folder%""
if not exist ""%install_folder%"" goto settings

set /a attempts+=1
ping 127.0.0.1 -n 2 > nul
goto loop

:settings
if exist ""%settings_folder%"" rmdir /s /q ""%settings_folder%""

:cleanup
if exist ""{shortcutPath}"" del /f /q ""{shortcutPath}""

(goto) 2>nul & del ""%~f0""";

            File.WriteAllText(batPath, script);

            Process.Start(new ProcessStartInfo(batPath)
            {
                UseShellExecute = true,
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden
            });

            ExecuteShutdownSequence();
            Close();
        }

        private Task RemoveStartupTaskAsync()
        {
            return Task.Run(() => RunProcessAndWait("schtasks", $"/Delete /TN \"{AppName}\" /F"));
        }

        #endregion
    }
}