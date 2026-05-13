using LibreHardwareMonitor.Hardware;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Text;
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
        private bool _isRefreshingTemps = false;
        private bool _resourcesDisposed = false;

        private readonly object _hardwareUpdateLock = new object();

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
            ApplyLabelHover(cpuModel,
                            gpuModel,
                            ramDetails,
                            storageDetails,
                            motherboardDetails);
        }

        private async void MainForm_Load(object sender, EventArgs e)
        {
            appVersion.Text = $"Version: {Application.ProductVersion}";

            using (var g = CreateGraphics())
            {
                _dpiScale = g.DpiX / 96f;
            }

            SetDefaultControlValues();
            LoadSettings();
            CacheDisplaySettings();

            UpdateTrayIcons();
            SelectedTabChanged(this, EventArgs.Empty);

            await InitializeHardwareAsync();

            SetupTimer();
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
            string motherboard = await GetMotherboardNameAsync();
            string ram = await GetRamInfoAsync();

            if (IsDisposed || !IsHandleCreated)
                return;

            motherboardDetails.Text = motherboard;
            ramDetails.Text = ram;

            await InitializeHardwareMonitorAsync();
        }

        private Task InitializeHardwareMonitorAsync()
        {
            return Task.Run(() =>
            {
                Computer newComputer = null;

                try
                {
                    List<IHardware> cpuHardwares;
                    List<IHardware> gpuHardwares;
                    List<IHardware> storageHardwares;

                    lock (_hardwareUpdateLock)
                    {
                        if (_isShutdownInitiated || _resourcesDisposed)
                            return;

                        newComputer = new Computer
                        {
                            IsCpuEnabled = true,
                            IsGpuEnabled = true,
                            IsStorageEnabled = true,
                            IsMotherboardEnabled = true,
                            IsMemoryEnabled = true,
                            IsControllerEnabled = true
                        };

                        newComputer.Open();

                        foreach (var hardware in newComputer.Hardware)
                            UpdateHardwareRecursive(hardware);

                        cpuHardwares = newComputer.Hardware
                            .Where(h => h.HardwareType == HardwareType.Cpu)
                            .ToList();

                        gpuHardwares = newComputer.Hardware
                            .Where(h =>
                                h.HardwareType == HardwareType.GpuAmd ||
                                h.HardwareType == HardwareType.GpuNvidia ||
                                h.HardwareType == HardwareType.GpuIntel)
                            .ToList();

                        storageHardwares = newComputer.Hardware
                            .Where(h => h.HardwareType == HardwareType.Storage)
                            .ToList();

                        _computer = newComputer;
                        _cpuHardwares = cpuHardwares;
                        _gpuHardwares = gpuHardwares;
                        _storageHardwares = storageHardwares;

                        newComputer = null;
                    }

                    if (!IsHandleCreated || IsDisposed || _isShutdownInitiated || _resourcesDisposed)
                        return;

                    Invoke((MethodInvoker)delegate
                    {
                        if (_isShutdownInitiated || _resourcesDisposed || IsDisposed)
                            return;

                        PopulateHardwareSelector(cpuIndexSelect, _cpuHardwares, _savedCpuIndex, cpuModel, "CPU");
                        PopulateHardwareSelector(gpuIndexSelect, _gpuHardwares, _savedGpuIndex, gpuModel, "GPU");
                        PopulateHardwareSelector(storageIndexSelect, _storageHardwares, _savedStorageIndex, storageDetails, "Disk");

                        string cpuText = cpuModel.Text;
                        string gpuText = gpuModel.Text;

                        if (gpuText.IndexOf("nvidia", StringComparison.OrdinalIgnoreCase) >= 0)
                            gpuBrandPic.Image = Properties.Resources.nvidia;
                        else if (gpuText.IndexOf("amd", StringComparison.OrdinalIgnoreCase) >= 0 ||
                                 gpuText.IndexOf("radeon", StringComparison.OrdinalIgnoreCase) >= 0)
                            gpuBrandPic.Image = Properties.Resources.amd;
                        else if (gpuText.IndexOf("intel", StringComparison.OrdinalIgnoreCase) >= 0 ||
                                 gpuText.IndexOf("arc", StringComparison.OrdinalIgnoreCase) >= 0)
                            gpuBrandPic.Image = Properties.Resources.intel;

                        if (cpuText.IndexOf("intel", StringComparison.OrdinalIgnoreCase) >= 0)
                            cpuBrandPic.Image = Properties.Resources.intel;
                        else if (cpuText.IndexOf("amd", StringComparison.OrdinalIgnoreCase) >= 0 ||
                                 cpuText.IndexOf("ryzen", StringComparison.OrdinalIgnoreCase) >= 0)
                            cpuBrandPic.Image = Properties.Resources.amd;
                    });
                }
                catch (Exception ex)
                {
                    try
                    {
                        newComputer?.Close();
                    }
                    catch { }

                    if (!_isShutdownInitiated && !_resourcesDisposed && IsHandleCreated && !IsDisposed)
                    {
                        try
                        {
                            Invoke((MethodInvoker)delegate
                            {
                                MessageBox.Show(
                                    this,
                                    $"Hardware initialization failed.\n\n{ex.Message}",
                                    "Hardware Error",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                            });
                        }
                        catch { }
                    }
                }
            });
        }

        private void UpdateHardwareRecursive(IHardware hardware)
        {
            hardware.Update();

            foreach (IHardware subHardware in hardware.SubHardware)
            {
                UpdateHardwareRecursive(subHardware);
            }
        }

        private void SetupTimer()
        {
            _tempTimer.Stop();
            _tempTimer.Tick -= TempTimer_Tick;
            _tempTimer.Tick += TempTimer_Tick;

            UpdateTimerInterval();
            _tempTimer.Start();
        }

        private void UpdateTimerInterval()
        {
            _tempTimer.Interval = Math.Max(250, (int)(refreshValue.Value * 1000));
        }

        private async void TempTimer_Tick(object sender, EventArgs e)
        {
            if (_isShutdownInitiated || _isRefreshingTemps || _resourcesDisposed)
                return;

            _isRefreshingTemps = true;

            try
            {
                _tempTimer.Stop();
            }
            catch { }

            try
            {
                await Task.Run(() =>
                {
                    lock (_hardwareUpdateLock)
                    {
                        if (_isShutdownInitiated || _resourcesDisposed || _computer == null)
                            return;

                        if (_selectedCpuHardware != null)
                            UpdateHardwareRecursive(_selectedCpuHardware);

                        if (_selectedGpuHardware != null)
                            UpdateHardwareRecursive(_selectedGpuHardware);
                    }
                });

                if (_isShutdownInitiated || _resourcesDisposed || IsDisposed || !IsHandleCreated)
                    return;

                float? cpuTemp = IsValidTemp(_cpuTempSensor?.Value)
                    ? _cpuTempSensor.Value
                    : null;

                float? gpuTemp = IsValidTemp(_gpuTempSensor?.Value)
                    ? _gpuTempSensor.Value
                    : null;

                UpdateTemperatures(cpuTemp, gpuTemp);
                UpdateAllTrayIcons(cpuTemp, gpuTemp);
            }
            catch
            {
                // Hardware read errors can be transient.
            }
            finally
            {
                _isRefreshingTemps = false;

                if (!_isShutdownInitiated && !_resourcesDisposed && IsHandleCreated && !IsDisposed)
                {
                    try
                    {
                        _tempTimer.Start();
                    }
                    catch
                    {
                        // Timer may already be disposed during shutdown.
                    }
                }
            }
        }

        private static bool IsValidTemp(float? value)
        {
            return value.HasValue &&
                   value.Value > 0 &&
                   value.Value < 130;
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
            if (icon == null || _cpuBrush == null || _gpuBrush == null)
                return;

            string cacheKey = $"{cpuTemp:F0}_{gpuTemp:F0}_{_cpuBrush.Color.ToArgb()}_{_gpuBrush.Color.ToArgb()}";

            if (cacheKey == _lastCpuTempText)
                return;

            Icon oldIcon = icon.Icon;
            Icon newIcon = CreateCombinedTempIcon($"{cpuTemp:F0}", $"{gpuTemp:F0}");

            icon.Icon = newIcon;

            oldIcon?.Dispose();

            _lastCpuTempText = cacheKey;
        }

        private void UpdateSingleTrayIcon(NotifyIcon icon, float temp, ref string lastText, SolidBrush brush)
        {
            if (icon == null || brush == null)
                return;

            string cacheKey = $"{temp:F0}_{brush.Color.ToArgb()}";

            if (cacheKey == lastText)
                return;

            Icon oldIcon = icon.Icon;
            Icon newIcon = CreateTempIcon($"{temp:F0}", brush);

            icon.Icon = newIcon;

            oldIcon?.Dispose();

            lastText = cacheKey;
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

            float dpiScale = _dpiScale;

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

            if (fontFamilyValue.Items.Count > 0)
                fontFamilyValue.SelectedIndex = 0;

            WarmTempMin = 60;
            WarmTempMax = 80;

            NormalColor = Color.White;
            WarningColor = Color.Yellow;
            CriticalColor = Color.Red;
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
                    CenterToScreen();
                    SetDefaultControlValues();
                    _settingsLoaded = true;
                    return;
                }

                string json = File.ReadAllText(SettingsFilePath);
                var settings = System.Text.Json.JsonSerializer.Deserialize<AppSettings>(json);

                if (settings == null)
                {
                    SetDefaultControlValues();
                    CenterToScreen();
                    return;
                }

                if (settings.WindowWidth > 0 && settings.WindowHeight > 0)
                {
                    int width = Math.Max(MinimumSize.Width, settings.WindowWidth);
                    int height = Math.Max(MinimumSize.Height, settings.WindowHeight);

                    Rectangle savedBounds = new Rectangle(settings.WindowX, settings.WindowY, width, height);

                    Size = new Size(width, height);

                    if (settings.WindowX != -1 && IsWindowBoundsVisible(savedBounds))
                        Location = new Point(settings.WindowX, settings.WindowY);
                    else
                        CenterToScreen();
                }
                else
                {
                    CenterToScreen();
                }

                autostartInstall.Checked = settings.Autostart;
                tempsFahrenheit.Checked = settings.TempsFahrenheit;
                singleIconTray.Checked = settings.SingleIconTray;
                enableCpuTray.Checked = settings.CpuTrayIcon;
                enableGpuTray.Checked = settings.GpuTrayIcon;
                colortempsEnable.Checked = settings.TempBasedIconColor;

                refreshValue.Value = ClampDecimal(settings.UpdateInterval, refreshValue.Minimum, refreshValue.Maximum);
                iconsizeValue.Value = ClampDecimal(settings.IconSize, iconsizeValue.Minimum, iconsizeValue.Maximum);

                if (fontFamilyValue.Items.Count > 0)
                    fontFamilyValue.SelectedIndex = ClampInt(settings.FontFamily, 0, fontFamilyValue.Items.Count - 1);

                cpuColorValue.BackColor = Color.FromArgb(settings.CpuColor);
                gpuColorValue.BackColor = Color.FromArgb(settings.GpuColor);

                WarmTempMin = ClampInt(settings.MinWarmTemp, 0, 130);
                WarmTempMax = ClampInt(settings.MaxWarmTemp, WarmTempMin, 130);

                NormalColor = Color.FromArgb(settings.NormalTempColor);
                WarningColor = Color.FromArgb(settings.WarmTempColor);
                CriticalColor = Color.FromArgb(settings.HotTempColor);

                _savedCpuIndex = Math.Max(0, settings.CpuIndex);
                _savedGpuIndex = Math.Max(0, settings.GpuIndex);
                _savedStorageIndex = Math.Max(0, settings.StorageIndex);

                InstallPath = settings.InstallFolder;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to load settings: {ex.Message}");
                SetDefaultControlValues();
                CenterToScreen();
            }
            finally
            {
                SingleIconTray_CheckedChanged(this, EventArgs.Empty);
                ColortempsEnable_CheckedChanged(this, EventArgs.Empty);
                _settingsLoaded = true;
            }
        }

        private static decimal ClampDecimal(decimal value, decimal min, decimal max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }

        private static int ClampInt(int value, int min, int max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }

        private static bool IsWindowBoundsVisible(Rectangle bounds)
        {
            foreach (Screen screen in Screen.AllScreens)
            {
                if (screen.WorkingArea.IntersectsWith(bounds))
                    return true;
            }

            return false;
        }

        #endregion

        #region [ Event Handlers: UI Actions ]

        private void SetTab(int index)
        {
            mainTabControl.SelectedIndex = index;
            SelectedTabChanged(null, EventArgs.Empty);
        }

        private void HomeBtn_Click(object sender, EventArgs e) => SetTab(0);
        private void SettingsBtn_Click(object sender, EventArgs e) => SetTab(1);
        private void AboutBtn_Click(object sender, EventArgs e) => SetTab(2);

        private void SelectedTabChanged(object sender, EventArgs e)
        {
            Color sidepanelline = Color.FromArgb(0, 120, 212);
            Color selectedColor = Color.FromArgb(50, 50, 50);
            Color defaultColor = Color.FromArgb(30, 30, 30);

            int index = mainTabControl.SelectedIndex;

            homeBtn.BackColor = (index == 0) ? selectedColor : defaultColor;
            settingsBtn.BackColor = (index == 1) ? selectedColor : defaultColor;
            aboutBtn.BackColor = (index == 2) ? selectedColor : defaultColor;

            // HOME
            homePanel.ColumnStyles[0].SizeType = SizeType.Absolute;
            homePanel.ColumnStyles[0].Width = (index == 0) ? 3 : 0;
            sidepanelHome.Visible = (index == 0);
            if (index == 0) sidepanelHome.BackColor = sidepanelline;

            // SETTINGS
            settingsPanel.ColumnStyles[0].SizeType = SizeType.Absolute;
            settingsPanel.ColumnStyles[0].Width = (index == 1) ? 3 : 0;
            sidepanelSettings.Visible = (index == 1);
            if (index == 1) sidepanelSettings.BackColor = sidepanelline;

            // ABOUT
            aboutPanel.ColumnStyles[0].SizeType = SizeType.Absolute;
            aboutPanel.ColumnStyles[0].Width = (index == 2) ? 3 : 0;
            sidepanelAbout.Visible = (index == 2);
            if (index == 2) sidepanelAbout.BackColor = sidepanelline;
        }

        private void ExitBtn_Click(object sender, EventArgs e)
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

        private void MinimizeBtn_Click(object sender, EventArgs e)
        {
            Hide();
            UpdateTrayIcons();
        }

        private void DonatePic_Click(object sender, EventArgs e)
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

        private void GpuTrayIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ShowWindow();
        }

        private void CpuTrayIcon_MouseDoubleClick(object sender, MouseEventArgs e)
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
            if (sender is ComboBox)
            {
                CacheDisplaySettings();
                _lastCpuTempText = null;
                _lastGpuTempText = null;
                TempTimer_Tick(this, EventArgs.Empty);
            }
        }

        private void ColortempsEnable_CheckedChanged(object sender, EventArgs e)
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

        private void SingleIconTray_CheckedChanged(object sender, EventArgs e)
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

        private void ColortempsConfig_Click(object sender, EventArgs e)
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
            if (cpuIndexSelect.SelectedIndex < 0 ||
                _cpuHardwares == null ||
                cpuIndexSelect.SelectedIndex >= _cpuHardwares.Count)
                return;

            _selectedCpuHardware = _cpuHardwares[cpuIndexSelect.SelectedIndex];

            cpuModel.Text = _selectedCpuHardware.Name;
            cpuName.Text = _selectedCpuHardware.Name;

            UpdateHardwareRecursive(_selectedCpuHardware);

            var tempSensors = _selectedCpuHardware.Sensors
                .Where(s => s.SensorType == SensorType.Temperature)
                .ToList();

            _cpuTempSensor =
                tempSensors.FirstOrDefault(s =>
                    s.Name.IndexOf("Package", StringComparison.OrdinalIgnoreCase) >= 0)

                ?? tempSensors.FirstOrDefault(s =>
                    s.Name.IndexOf("Tctl", StringComparison.OrdinalIgnoreCase) >= 0)

                ?? tempSensors.FirstOrDefault(s =>
                    s.Name.IndexOf("Core Max", StringComparison.OrdinalIgnoreCase) >= 0)

                ?? tempSensors
                    .OrderByDescending(s => s.Value ?? 0)
                    .FirstOrDefault();

            _selectedCpuIdentifier = _selectedCpuHardware.Identifier.ToString();

            _cpuMinTemp = float.MaxValue;
            _cpuMaxTemp = float.MinValue;

            _lastCpuTempText = null;

            TempTimer_Tick(this, EventArgs.Empty);
        }

        private void GpuIndexSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (gpuIndexSelect.SelectedIndex < 0 ||
                _gpuHardwares == null ||
                gpuIndexSelect.SelectedIndex >= _gpuHardwares.Count)
                return;

            _selectedGpuHardware = _gpuHardwares[gpuIndexSelect.SelectedIndex];

            gpuModel.Text = _selectedGpuHardware.Name;
            gpuName.Text = _selectedGpuHardware.Name;

            UpdateHardwareRecursive(_selectedGpuHardware);

            var tempSensors = _selectedGpuHardware.Sensors
                .Where(s => s.SensorType == SensorType.Temperature)
                .ToList();

            _gpuTempSensor =
                tempSensors.FirstOrDefault(s =>
                    s.Name.Equals("GPU Core", StringComparison.OrdinalIgnoreCase))

                ?? tempSensors.FirstOrDefault(s =>
                    s.Name.Equals("GPU", StringComparison.OrdinalIgnoreCase))

                ?? tempSensors.FirstOrDefault(s =>
                    s.Name.IndexOf("Core", StringComparison.OrdinalIgnoreCase) >= 0)

                ?? tempSensors.FirstOrDefault(s =>
                    s.Name.IndexOf("Hot Spot", StringComparison.OrdinalIgnoreCase) >= 0)

                ?? tempSensors
                    .OrderByDescending(s => s.Value ?? 0)
                    .FirstOrDefault();

            _selectedGpuIdentifier = _selectedGpuHardware.Identifier.ToString();

            _gpuMinTemp = float.MaxValue;
            _gpuMaxTemp = float.MinValue;

            _lastGpuTempText = null;

            TempTimer_Tick(this, EventArgs.Empty);
        }

        private void StorageIndexSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_storageHardwares == null ||
                storageIndexSelect.SelectedIndex < 0 ||
                storageIndexSelect.SelectedIndex >= _storageHardwares.Count)
            {
                _selectedStorageIdentifier = null;
                return;
            }

            var selectedDrive = _storageHardwares[storageIndexSelect.SelectedIndex];

            _selectedStorageIdentifier = selectedDrive.Identifier.ToString();
            storageDetails.Text = selectedDrive.Name;
        }

        private void CpuColorValue_Click(object sender, EventArgs e)
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

        private void GpuColorValue_Click(object sender, EventArgs e)
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

        #region [ HardwareDetails ]

        // =========================
        // UI Entry Points
        // =========================

        public async void CpuModel_Click(object sender, EventArgs e)
        {
            await ShowHardwareDialogAsync(
                GetComponentDisplayName(_selectedCpuHardware, cpuModel.Text, "CPU"),
                "CPU",
                GetCpuDetails,
                _selectedCpuHardware);
        }

        public async void GpuModel_Click(object sender, EventArgs e)
        {
            await ShowHardwareDialogAsync(
                GetComponentDisplayName(_selectedGpuHardware, gpuModel.Text, "GPU"),
                "GPU",
                GetGpuDetails,
                _selectedGpuHardware);
        }

        public async void RamDetails_Click(object sender, EventArgs e)
        {
            await ShowHardwareDialogAsync(
                GetCleanDialogTitle(ramDetails.Text, "RAM"),
                "RAM",
                GetRamDetails,
                GetFirstHardware(HardwareType.Memory));
        }

        public async void StorageDetails_Click(object sender, EventArgs e)
        {
            await ShowHardwareDialogAsync(
                GetCleanDialogTitle(storageDetails.Text, "Storage"),
                "Storage",
                GetStorageDetails,
                GetSelectedStorageHardware());
        }

        public async void MotherboardDetails_Click(object sender, EventArgs e)
        {
            await ShowHardwareDialogAsync(
                GetCleanDialogTitle(motherboardDetails.Text, "Motherboard"),
                "Motherboard",
                GetMotherboardDetails,
                GetFirstHardware(HardwareType.Motherboard));
        }

        private static void ApplyLabelHover(params Label[] labels)
        {
            foreach (Label label in labels)
            {
                Color normalColor = label.ForeColor;

                label.MouseEnter += (s, e) =>
                {
                    label.ForeColor = Color.White;
                    label.Cursor = Cursors.Hand;
                };

                label.MouseLeave += (s, e) =>
                {
                    label.ForeColor = normalColor;
                    label.Cursor = Cursors.Default;
                };
            }
        }

        private IHardware GetFirstHardware(HardwareType hardwareType)
        {
            if (_computer == null)
                return null;

            return _computer.Hardware.FirstOrDefault(h => h.HardwareType == hardwareType);
        }

        private IHardware GetSelectedStorageHardware()
        {
            if (_storageHardwares == null)
                return null;

            int index = storageIndexSelect.SelectedIndex;

            if (index < 0 || index >= _storageHardwares.Count)
                return null;

            return _storageHardwares[index];
        }

        private async Task ShowHardwareDialogAsync(
            string componentName,
            string categoryName,
            Func<string> contentFactory,
            IHardware liveHardware = null)
        {
            SetLoadingCursor(true);

            try
            {
                string finalComponentName = GetFinalComponentName(componentName, categoryName, liveHardware);
                string content = await Task.Run(contentFactory);

                if (IsDisposed || !IsHandleCreated)
                    return;

                Func<Task<string>> liveFactory = null;

                if (liveHardware != null)
                {
                    liveFactory = () => Task.Run(() =>
                    {
                        lock (_hardwareUpdateLock)
                        {
                            UpdateHardwareRecursive(liveHardware);
                            return BuildLiveSensorsText(liveHardware);
                        }
                    });
                }

                using (var dlg = new HardwareDetailsDialog(
                    finalComponentName,
                    categoryName,
                    content,
                    liveFactory,
                    () => _isShutdownInitiated))
                {
                    dlg.ShowDialog(this);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    this,
                    $"Could not load hardware details.\n\n{ex.Message}",
                    "Hardware Details Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                SetLoadingCursor(false);
            }
        }

        private void SetLoadingCursor(bool loading)
        {
            UseWaitCursor = loading;
            Cursor.Current = loading ? Cursors.WaitCursor : Cursors.Default;

            SetLoadingCursorRecursive(this, loading);
        }

        private static void SetLoadingCursorRecursive(Control parent, bool loading)
        {
            foreach (Control control in parent.Controls)
            {
                control.UseWaitCursor = loading;

                if (control.HasChildren)
                    SetLoadingCursorRecursive(control, loading);
            }
        }

        // =========================
        // Text / Formatting Helpers
        // =========================

        private const int InfoLabelWidth = 24;

        private static string GetComponentDisplayName(IHardware hardware, string labelText, string fallback)
        {
            if (hardware != null && !string.IsNullOrWhiteSpace(hardware.Name))
                return hardware.Name.Trim();

            return GetCleanDialogTitle(labelText, fallback);
        }

        private static string GetFinalComponentName(string componentName, string categoryName, IHardware hardware)
        {
            if (hardware != null && !string.IsNullOrWhiteSpace(hardware.Name))
                return hardware.Name.Trim();

            string clean = GetCleanDialogTitle(componentName, categoryName);

            if (string.IsNullOrWhiteSpace(clean))
                return categoryName;

            return clean;
        }

        private static string GetCleanDialogTitle(string text, string fallback)
        {
            if (string.IsNullOrWhiteSpace(text))
                return fallback;

            text = text
                .Replace("\r", "\n")
                .Split('\n')[0]
                .Trim();

            if (text.Equals("N/A", StringComparison.OrdinalIgnoreCase))
                return fallback;

            if (text.Equals("Loading...", StringComparison.OrdinalIgnoreCase))
                return fallback;

            return text;
        }

        private static string Section(string title)
        {
            title = Safe(title).ToUpperInvariant();

            return
                $"{title}\r\n" +
                $"{new string('═', 64)}\r\n";
        }

        private static string Group(string title)
        {
            title = Safe(title);

            return
                $"\r\n[{title}]\r\n" +
                $"{new string('─', Math.Min(64, title.Length + 2))}\r\n";
        }

        private static string Label(string key, object value)
        {
            string k = string.IsNullOrWhiteSpace(key) ? "Info" : key.Trim();
            string v = Safe(value);

            if (k.Length <= InfoLabelWidth)
                return $"  {k,-InfoLabelWidth} : {v}";

            return
                $"  {k}\r\n" +
                $"  {new string(' ', InfoLabelWidth)} : {v}";
        }

        private static string Safe(object value)
        {
            if (value == null)
                return "N/A";

            string text = value.ToString();

            if (string.IsNullOrWhiteSpace(text))
                return "N/A";

            return text.Trim();
        }

        private static string Unit(object value, string unit)
        {
            string text = Safe(value);

            if (text == "N/A")
                return text;

            return $"{text} {unit}";
        }

        private static string SizeHuman(object bytesObj)
        {
            try
            {
                if (bytesObj == null)
                    return "N/A";

                double bytes = Convert.ToDouble(bytesObj);

                if (bytes <= 0)
                    return "0 B";

                string[] suffixes = { "B", "KB", "MB", "GB", "TB" };
                int index = 0;

                while (bytes >= 1024 && index < suffixes.Length - 1)
                {
                    bytes /= 1024;
                    index++;
                }

                return $"{bytes:0.0} {suffixes[index]}";
            }
            catch
            {
                return "N/A";
            }
        }

        private static string FormatWmiDate(string value)
        {
            if (string.IsNullOrWhiteSpace(value) || value == "N/A")
                return "N/A";

            try
            {
                return ManagementDateTimeConverter.ToDateTime(value).ToString("yyyy-MM-dd");
            }
            catch
            {
                return value;
            }
        }

        private static uint ToUInt(object value)
        {
            try
            {
                if (value == null)
                    return 0;

                return Convert.ToUInt32(value);
            }
            catch
            {
                return 0;
            }
        }

        // =========================
        // Live Sensors
        // =========================

        private string BuildLiveSensorsText(IHardware hardware)
        {
            var sb = new StringBuilder();

            sb.AppendLine("LIVE SENSORS");
            sb.AppendLine("════════════════════════════════════════════════════════════════");
            sb.AppendLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            sb.AppendLine();

            if (hardware == null)
            {
                sb.AppendLine("No hardware selected.");
                return sb.ToString();
            }

            AppendLiveHardwareSensors(sb, hardware, "");

            return sb.ToString();
        }

        private void AppendLiveHardwareSensors(StringBuilder sb, IHardware hardware, string indent)
        {
            if (hardware == null)
                return;

            sb.AppendLine($"{indent}{hardware.Name}");
            sb.AppendLine($"{indent}{new string('─', Math.Min(64, Safe(hardware.Name).Length + 8))}");

            var sensors = hardware.Sensors
                .Where(s => s.Value.HasValue)
                .OrderBy(s => s.SensorType.ToString())
                .ThenBy(s => s.Name)
                .ToList();

            if (sensors.Count == 0)
            {
                sb.AppendLine($"{indent}No live sensors available.");
            }
            else
            {
                foreach (var sensor in sensors)
                {
                    sb.AppendLine(
                        $"{indent}{sensor.SensorType,-13} {Safe(sensor.Name),-36} {FormatSensorValue(sensor)}");
                }
            }

            sb.AppendLine();

            foreach (var subHardware in hardware.SubHardware)
            {
                try
                {
                    subHardware.Update();
                }
                catch { }

                AppendLiveHardwareSensors(sb, subHardware, indent + "  ");
            }
        }

        private static void AppendSensorSummary(StringBuilder sb, IHardware hardware)
        {
            if (hardware == null)
                return;

            sb.Append(Section("SENSORS"));

            try
            {
                hardware.Update();

                var sensors = hardware.Sensors
                    .Where(s =>
                        s.SensorType == SensorType.Temperature ||
                        s.SensorType == SensorType.Load ||
                        s.SensorType == SensorType.Clock ||
                        s.SensorType == SensorType.Power ||
                        s.SensorType == SensorType.Voltage ||
                        s.SensorType == SensorType.Fan)
                    .OrderBy(s => s.SensorType.ToString())
                    .ThenBy(s => s.Name)
                    .ToList();

                if (sensors.Count == 0)
                {
                    sb.AppendLine("  No sensors available.");
                    sb.AppendLine();
                    return;
                }

                foreach (var sensor in sensors)
                    sb.AppendLine(Label($"{sensor.SensorType} / {sensor.Name}", FormatSensorValue(sensor)));

                sb.AppendLine();
            }
            catch
            {
                sb.AppendLine("  Could not read sensors.");
                sb.AppendLine();
            }
        }

        private static string FormatSensorValue(ISensor sensor)
        {
            if (sensor == null || !sensor.Value.HasValue)
                return "N/A";

            float value = sensor.Value.Value;

            switch (sensor.SensorType)
            {
                case SensorType.Temperature:
                    return $"{value:0.0} °C";

                case SensorType.Load:
                    return $"{value:0.0} %";

                case SensorType.Clock:
                    return $"{value:0} MHz";

                case SensorType.Power:
                    return $"{value:0.0} W";

                case SensorType.Voltage:
                    return $"{value:0.000} V";

                case SensorType.Fan:
                    return $"{value:0} RPM";

                default:
                    return value.ToString("0.##", CultureInfo.InvariantCulture);
            }
        }

        // =========================
        // WMI Helpers
        // =========================

        private static List<ManagementObject> WmiQuery(string query)
        {
            var list = new List<ManagementObject>();

            try
            {
                using (var searcher = new ManagementObjectSearcher(query))
                using (var results = searcher.Get())
                {
                    foreach (ManagementObject obj in results.Cast<ManagementObject>())
                        list.Add(obj);
                }
            }
            catch { }

            return list;
        }

        private static string GetCpuArchitectureString(object value)
        {
            uint code = ToUInt(value);

            switch (code)
            {
                case 0:
                    return "x86";

                case 5:
                    return "ARM";

                case 9:
                    return "x64";

                case 12:
                    return "ARM64";

                default:
                    return code == 0 ? "N/A" : code.ToString();
            }
        }

        private static string GetMemoryTypeString(uint type)
        {
            switch (type)
            {
                case 20:
                    return "DDR";

                case 21:
                    return "DDR2";

                case 24:
                    return "DDR3";

                case 26:
                    return "DDR4";

                case 34:
                    return "DDR5";

                default:
                    return type == 0 ? "N/A" : $"Unknown ({type})";
            }
        }

        // =========================
        // CPU
        // =========================

        public string GetCpuDetails()
        {
            var sb = new StringBuilder();
            sb.Append(Section("CPU"));

            if (_selectedCpuHardware != null)
            {
                sb.AppendLine(Label("Selected CPU", _selectedCpuHardware.Name));
                sb.AppendLine(Label("Identifier", _selectedCpuHardware.Identifier));
            }

            var cpus = WmiQuery("SELECT * FROM Win32_Processor");

            if (cpus.Count == 0)
            {
                sb.AppendLine();
                sb.AppendLine("  No CPU information found.");
                return sb.ToString();
            }

            int index = 1;

            foreach (var cpu in cpus)
            {
                sb.Append(Group($"CPU #{index++}"));

                sb.AppendLine(Label("Name", cpu["Name"]));
                sb.AppendLine(Label("Manufacturer", cpu["Manufacturer"]));
                sb.AppendLine(Label("Cores", cpu["NumberOfCores"]));
                sb.AppendLine(Label("Threads", cpu["NumberOfLogicalProcessors"]));
                sb.AppendLine(Label("Max Clock", Unit(cpu["MaxClockSpeed"], "MHz")));
                sb.AppendLine(Label("Socket", cpu["SocketDesignation"]));
                sb.AppendLine(Label("Processor ID", cpu["ProcessorId"]));
                sb.AppendLine(Label("L2 Cache", Unit(cpu["L2CacheSize"], "KB")));
                sb.AppendLine(Label("L3 Cache", Unit(cpu["L3CacheSize"], "KB")));
                sb.AppendLine(Label("Architecture", GetCpuArchitectureString(cpu["Architecture"])));
            }

            return sb.ToString();
        }

        // =========================
        // GPU
        // =========================

        public string GetGpuDetails()
        {
            var sb = new StringBuilder();
            sb.Append(Section("GPU"));

            if (_selectedGpuHardware != null)
            {
                sb.AppendLine(Label("Selected GPU", _selectedGpuHardware.Name));
                sb.AppendLine(Label("Identifier", _selectedGpuHardware.Identifier));
            }

            var gpus = WmiQuery("SELECT * FROM Win32_VideoController");

            if (gpus.Count == 0)
            {
                sb.AppendLine();
                sb.AppendLine("  No GPU information found.");
                return sb.ToString();
            }

            int index = 1;

            foreach (var gpu in gpus)
            {
                sb.Append(Group($"GPU #{index++}"));

                string width = Safe(gpu["CurrentHorizontalResolution"]);
                string height = Safe(gpu["CurrentVerticalResolution"]);
                string refresh = Safe(gpu["CurrentRefreshRate"]);

                sb.AppendLine(Label("Name", gpu["Name"]));
                sb.AppendLine(Label("Driver", gpu["DriverVersion"]));
                sb.AppendLine(Label("Driver Date", FormatWmiDate(Safe(gpu["DriverDate"]))));
                sb.AppendLine(Label("Video Processor", gpu["VideoProcessor"]));
                sb.AppendLine(Label("Dedicated VRAM", GetGpuVramText(gpu)));
                sb.AppendLine(Label("DAC Type", gpu["AdapterDACType"]));
                sb.AppendLine(Label("Resolution", $"{width} x {height} @ {refresh}Hz"));
                sb.AppendLine(Label("PNP Device ID", gpu["PNPDeviceID"]));
            }

            return sb.ToString();
        }

        private static string GetGpuVramText(ManagementObject gpu)
        {

            if (TryGetGpuVramFromRegistry(gpu, out ulong bytes) && bytes > 0)
                return SizeHuman(bytes);

            if (TryGetGpuVramFromWmi(gpu, out bytes) && bytes > 0)
                return SizeHuman(bytes);

            return "N/A";
        }

        private static bool TryGetGpuVramFromWmi(ManagementObject gpu, out ulong bytes)
        {
            bytes = 0;

            try
            {
                object adapterRam = gpu["AdapterRAM"];

                if (adapterRam == null)
                    return false;

                if (adapterRam is uint v)
                {
                    bytes = v;
                    return bytes > 0;
                }

                if (adapterRam is int value)
                {
                    if (value <= 0)
                        return false;

                    bytes = (ulong)value;
                    return bytes > 0;
                }

                bytes = Convert.ToUInt64(adapterRam);
                return bytes > 0;
            }
            catch
            {
                return false;
            }
        }

        private static bool TryGetGpuVramFromRegistry(ManagementObject gpu, out ulong bytes)
        {
            bytes = 0;

            try
            {
                string gpuName = Safe(gpu["Name"]);
                string pnpId = Safe(gpu["PNPDeviceID"]);

                RegistryView[] views =
                {
            RegistryView.Registry64,
            RegistryView.Registry32
        };

                foreach (RegistryView view in views)
                {
                    using (RegistryKey baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, view))
                    using (RegistryKey videoKey = baseKey.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\Video"))
                    {
                        if (videoKey == null)
                            continue;

                        foreach (string guidName in videoKey.GetSubKeyNames())
                        {
                            using (RegistryKey guidKey = videoKey.OpenSubKey(guidName))
                            {
                                if (guidKey == null)
                                    continue;

                                foreach (string adapterSubKeyName in guidKey.GetSubKeyNames())
                                {
                                    using (RegistryKey adapterKey = guidKey.OpenSubKey(adapterSubKeyName))
                                    {
                                        if (adapterKey == null)
                                            continue;

                                        if (!IsGpuRegistryMatch(adapterKey, gpuName, pnpId))
                                            continue;

                                        if (TryReadGpuMemoryValue(adapterKey, "HardwareInformation.qwMemorySize", out bytes))
                                            return true;

                                        if (TryReadGpuMemoryValue(adapterKey, "HardwareInformation.MemorySize", out bytes))
                                            return true;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch { }

            return false;
        }

        private static bool IsGpuRegistryMatch(RegistryKey adapterKey, string gpuName, string pnpId)
        {
            string adapterString = RegistryValueToString(adapterKey.GetValue("HardwareInformation.AdapterString"));
            string chipType = RegistryValueToString(adapterKey.GetValue("HardwareInformation.ChipType"));
            string matchingDeviceId = RegistryValueToString(adapterKey.GetValue("MatchingDeviceId"));
            string deviceDescription = RegistryValueToString(adapterKey.GetValue("Device Description"));
            string providerName = RegistryValueToString(adapterKey.GetValue("ProviderName"));

            string registryText = $"{adapterString} {chipType} {matchingDeviceId} {deviceDescription} {providerName}";
            string regNorm = NormalizeGpuText(registryText);
            string nameNorm = NormalizeGpuText(gpuName);
            string pnpNorm = NormalizeGpuText(pnpId);

            if (!string.IsNullOrWhiteSpace(nameNorm) && regNorm.Contains(nameNorm))
                return true;

            if (!string.IsNullOrWhiteSpace(pnpNorm))
            {
                string shortPnp = pnpNorm;

                int revIndex = shortPnp.IndexOf("REV", StringComparison.OrdinalIgnoreCase);
                if (revIndex > 0)
                    shortPnp = shortPnp.Substring(0, revIndex);

                if (!string.IsNullOrWhiteSpace(shortPnp) && regNorm.Contains(shortPnp))
                    return true;
            }

            return false;
        }

        private static bool TryReadGpuMemoryValue(RegistryKey key, string valueName, out ulong bytes)
        {
            bytes = 0;

            try
            {
                object value = key.GetValue(valueName);

                if (value == null)
                    return false;

                if (value is long)
                {
                    long v = (long)value;

                    if (v <= 0)
                        return false;

                    bytes = (ulong)v;
                    return true;
                }

                if (value is int)
                {
                    int v = (int)value;

                    if (v <= 0)
                        return false;

                    bytes = (uint)v;
                    return true;
                }

                if (value is byte[] data)
                {
                    if (data.Length >= 8)
                    {
                        bytes = BitConverter.ToUInt64(data, 0);
                        return bytes > 0;
                    }

                    if (data.Length >= 4)
                    {
                        bytes = BitConverter.ToUInt32(data, 0);
                        return bytes > 0;
                    }
                }

                bytes = Convert.ToUInt64(value);
                return bytes > 0;
            }
            catch
            {
                return false;
            }
        }

        private static string RegistryValueToString(object value)
        {
            if (value == null)
                return string.Empty;

            if (value is byte[] data)
            {
                try
                {
                    string unicode = Encoding.Unicode.GetString(data).TrimEnd('\0', ' ');
                    if (!string.IsNullOrWhiteSpace(unicode))
                        return unicode;

                    string ascii = Encoding.ASCII.GetString(data).TrimEnd('\0', ' ');
                    return ascii;
                }
                catch
                {
                    return string.Empty;
                }
            }

            return value.ToString();
        }

        private static string NormalizeGpuText(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            text = text.ToUpperInvariant();

            var sb = new StringBuilder(text.Length);

            foreach (char c in text)
            {
                if (char.IsLetterOrDigit(c))
                    sb.Append(c);
            }

            return sb.ToString();
        }

        // =========================
        // RAM
        // =========================

        public string GetRamDetails()
        {
            var sb = new StringBuilder();
            var modules = new StringBuilder();

            sb.Append(Section("RAM"));

            var ram = WmiQuery("SELECT * FROM Win32_PhysicalMemory");

            if (ram.Count == 0)
            {
                sb.AppendLine();
                sb.AppendLine("  No RAM information found.");
                return sb.ToString();
            }

            long totalBytes = 0;
            int index = 1;

            foreach (var mem in ram)
            {
                object capacityObj = mem["Capacity"];

                try
                {
                    if (capacityObj != null)
                        totalBytes += Convert.ToInt64(capacityObj);
                }
                catch { }

                modules.Append(Group($"Module #{index++}"));

                modules.AppendLine(Label("Manufacturer", mem["Manufacturer"]));
                modules.AppendLine(Label("Capacity", SizeHuman(capacityObj)));
                modules.AppendLine(Label("Type", GetMemoryTypeString(ToUInt(mem["SMBIOSMemoryType"]))));
                modules.AppendLine(Label("Speed", Unit(mem["Speed"], "MHz")));
                modules.AppendLine(Label("Configured Speed", Unit(mem["ConfiguredClockSpeed"], "MHz")));
                modules.AppendLine(Label("Part Number", mem["PartNumber"]));
                modules.AppendLine(Label("Serial", mem["SerialNumber"]));
                modules.AppendLine(Label("Bank", mem["BankLabel"]));
                modules.AppendLine(Label("Slot", mem["DeviceLocator"]));
            }

            sb.AppendLine(Label("Total", SizeHuman(totalBytes)));
            sb.Append(modules);

            return sb.ToString();
        }

        // =========================
        // STORAGE
        // =========================

        public string GetStorageDetails()
        {
            var sb = new StringBuilder();
            sb.Append(Section("STORAGE"));

            if (!string.IsNullOrWhiteSpace(_selectedStorageIdentifier))
                sb.AppendLine(Label("Selected Identifier", _selectedStorageIdentifier));

            if (_storageHardwares != null && _storageHardwares.Count > 0)
            {
                sb.Append(Group("LibreHardwareMonitor Drives"));

                for (int i = 0; i < _storageHardwares.Count; i++)
                {
                    var drive = _storageHardwares[i];
                    string driveIdentifier = Safe(drive.Identifier);
                    string selected = driveIdentifier == _selectedStorageIdentifier ? "Yes" : "No";

                    sb.AppendLine(Label($"Drive #{i}", drive.Name));
                    sb.AppendLine(Label("Identifier", driveIdentifier));
                    sb.AppendLine(Label("Selected", selected));
                    sb.AppendLine();
                }
            }

            var disks = WmiQuery("SELECT * FROM Win32_DiskDrive");

            if (disks.Count == 0)
            {
                sb.AppendLine();
                sb.AppendLine("  No storage information found.");
                return sb.ToString();
            }

            int index = 1;

            foreach (var disk in disks)
            {
                sb.Append(Group($"Disk #{index++}"));

                sb.AppendLine(Label("Model", disk["Model"]));
                sb.AppendLine(Label("Interface", disk["InterfaceType"]));
                sb.AppendLine(Label("Media Type", disk["MediaType"]));
                sb.AppendLine(Label("Size", SizeHuman(disk["Size"])));
                sb.AppendLine(Label("Serial", disk["SerialNumber"]));
                sb.AppendLine(Label("Firmware", disk["FirmwareRevision"]));
                sb.AppendLine(Label("Partitions", disk["Partitions"]));
                sb.AppendLine(Label("PNP Device ID", disk["PNPDeviceID"]));
            }

            return sb.ToString();
        }

        // =========================
        // MOTHERBOARD + BIOS
        // =========================

        public string GetMotherboardDetails()
        {
            var sb = new StringBuilder();

            sb.Append(Section("MOTHERBOARD"));

            var boards = WmiQuery("SELECT * FROM Win32_BaseBoard");

            if (boards.Count == 0)
            {
                sb.AppendLine();
                sb.AppendLine("  No motherboard information found.");
            }
            else
            {
                int index = 1;

                foreach (var board in boards)
                {
                    sb.Append(Group($"Motherboard #{index++}"));

                    sb.AppendLine(Label("Manufacturer", board["Manufacturer"]));
                    sb.AppendLine(Label("Product", board["Product"]));
                    sb.AppendLine(Label("Version", board["Version"]));
                    sb.AppendLine(Label("Serial", board["SerialNumber"]));
                }
            }

            sb.AppendLine();
            sb.Append(Section("BIOS"));

            var biosList = WmiQuery("SELECT * FROM Win32_BIOS");

            if (biosList.Count == 0)
            {
                sb.AppendLine();
                sb.AppendLine("  No BIOS information found.");
            }
            else
            {
                int index = 1;

                foreach (var bios in biosList)
                {
                    sb.Append(Group($"BIOS #{index++}"));

                    sb.AppendLine(Label("Vendor", bios["Manufacturer"]));
                    sb.AppendLine(Label("Version", bios["SMBIOSBIOSVersion"]));
                    sb.AppendLine(Label("Release Date", FormatWmiDate(Safe(bios["ReleaseDate"]))));
                    sb.AppendLine(Label("Serial", bios["SerialNumber"]));
                }
            }

            return sb.ToString();
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
                        foreach (ManagementObject obj in collection.Cast<ManagementObject>())
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

                        foreach (ManagementObject stick in collection.Cast<ManagementObject>())
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

        private void CreateShortcutOnDesktop(string targetPath)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(targetPath) || !File.Exists(targetPath))
                    return;

                string deskPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                string shortcutPath = Path.Combine(deskPath, $"{AppName}.lnk");

                Type shellType = Type.GetTypeFromProgID("WScript.Shell");

                if (shellType == null)
                    return;

                dynamic shell = Activator.CreateInstance(shellType);
                dynamic shortcut = shell.CreateShortcut(shortcutPath);

                shortcut.TargetPath = targetPath;
                shortcut.WorkingDirectory = Path.GetDirectoryName(targetPath);
                shortcut.Description = "Launch TrayTemps";
                shortcut.Save();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Could not create shortcut: " + ex.Message);
            }
        }

        private bool RunProcessAndWait(string fileName, string arguments, out string error)
        {
            error = "";

            try
            {
                var psi = new ProcessStartInfo(fileName, arguments)
                {
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };

                using (var process = new Process())
                {
                    process.StartInfo = psi;
                    process.Start();

                    string output = process.StandardOutput.ReadToEnd();
                    string stdError = process.StandardError.ReadToEnd();

                    process.WaitForExit();

                    if (process.ExitCode != 0)
                    {
                        error = string.IsNullOrWhiteSpace(stdError) ? output : stdError;
                        return false;
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
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
            if (_isShutdownInitiated)
                return;

            _isShutdownInitiated = true;
            _resourcesDisposed = true;

            try
            {
                _tempTimer.Stop();
            }
            catch { }

            try
            {
                lock (_hardwareUpdateLock)
                {
                    try
                    {
                        _computer?.Close();
                    }
                    catch { }

                    _computer = null;

                    _selectedCpuHardware = null;
                    _selectedGpuHardware = null;

                    _cpuTempSensor = null;
                    _gpuTempSensor = null;
                }
            }
            catch { }

            try
            {
                ServiceManager.StopService("R0TrayTemps", 5);
            }
            catch { }

            try
            {
                DisposeTrayIcon(cpuTrayIcon);
                cpuTrayIcon = null;

                DisposeTrayIcon(gpuTrayIcon);
                gpuTrayIcon = null;

                DisposeTrayIcon(NotifyIcon);
                NotifyIcon = null;
            }
            catch { }

            try
            {
                _trayFont?.Dispose();
                _cpuBrush?.Dispose();
                _gpuBrush?.Dispose();

                _trayFont = null;
                _cpuBrush = null;
                _gpuBrush = null;
            }
            catch { }

            try
            {
                _tempTimer.Dispose();
            }
            catch { }
        }

        private static void DisposeTrayIcon(NotifyIcon trayIcon)
        {
            if (trayIcon == null)
                return;

            try
            {
                trayIcon.Visible = false;

                if (trayIcon.Icon != null)
                {
                    trayIcon.Icon.Dispose();
                    trayIcon.Icon = null;
                }

                trayIcon.Dispose();
            }
            catch { }
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
                this,
                "Add app to run silently at Windows startup?",
                "Startup",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result != DialogResult.Yes)
            {
                RevertCheckbox(autostartInstall, false);
                return;
            }

            bool installed = await InstallAndRestartAsync();

            if (!installed)
                RevertCheckbox(autostartInstall, false);
        }

        private async Task HandleAutostartDisable()
        {
            var result = MessageBox.Show(
                this,
                "Remove installed app, shortcut, and startup entry?\n\nYes = Remove all\nNo = Remove only startup entry\nCancel = Do nothing",
                "Confirm Remove",
                MessageBoxButtons.YesNoCancel,
                MessageBoxIcon.Warning);

            switch (result)
            {
                case DialogResult.Yes:
                    SaveSettings();
                    UninstallAndExit();
                    break;

                case DialogResult.No:
                    SaveSettings();
                    await RemoveStartupTaskAsync();

                    MessageBox.Show(
                        this,
                        "Startup entry removed.",
                        "Info",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

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

        private Task<bool> InstallAndRestartAsync()
        {
            using (var fbd = new FolderBrowserDialog())
            {
                fbd.Description = "Select installation folder for TrayTemps\n(Example: Program Files)";
                fbd.ShowNewFolderButton = true;

                string programFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);

                if (Directory.Exists(programFiles))
                    fbd.SelectedPath = programFiles;

                if (fbd.ShowDialog(this) != DialogResult.OK || string.IsNullOrWhiteSpace(fbd.SelectedPath))
                    return Task.FromResult(false);

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
                    MessageBox.Show(
                        this,
                        "Cannot write to selected folder. Choose another folder or run as administrator.",
                        "Access Denied",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    return Task.FromResult(false);
                }
            }

            string currentExePath = Application.ExecutablePath;
            string destExe = Path.Combine(InstallPath, $"{AppName}.exe");

            return Task.Run(() =>
            {
                try
                {
                    Directory.CreateDirectory(InstallPath);

                    File.Copy(currentExePath, destExe, true);

                    SaveSettings();


                    string createTaskArgs =
                        $"/Create /F /RL HIGHEST /SC ONLOGON /TN \"{AppName}\" /TR \"\\\"{destExe}\\\" -silent\"";

                    if (!RunProcessAndWait("schtasks.exe", createTaskArgs, out string error))
                        throw new Exception("Could not create startup task:\n" + error);

                    string powerShellArgs =
                        $"-NoProfile -ExecutionPolicy Bypass -Command \"Set-ScheduledTask -TaskName '{AppName}' -Settings (New-ScheduledTaskSettingsSet -AllowStartIfOnBatteries -DontStopIfGoingOnBatteries)\"";

                    RunProcessAndWait("powershell.exe", powerShellArgs, out error);

                    CreateShortcutOnDesktop(destExe);

                    if (IsHandleCreated && !IsDisposed)
                    {
                        Invoke(new Action(() =>
                        {
                            MessageBox.Show(
                                this,
                                "TrayTemps has been installed successfully.\nRestart it from the desktop shortcut.",
                                "Installation Complete",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);

                            ExecuteShutdownSequence();
                            Close();
                        }));
                    }

                    return true;
                }
                catch (Exception ex)
                {
                    if (IsHandleCreated && !IsDisposed)
                    {
                        Invoke(new Action(() =>
                        {
                            MessageBox.Show(
                                this,
                                ex.Message,
                                "Installation Error",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                        }));
                    }

                    return false;
                }
            });
        }

        private void UninstallAndExit()
        {
            if (string.IsNullOrEmpty(InstallPath) || !Directory.Exists(InstallPath))
            {
                MessageBox.Show(
                    this,
                    "Installation folder not found.",
                    "Uninstall Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                return;
            }

            string shortcutPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory),
                $"{AppName}.lnk");

            string batPath = Path.Combine(Path.GetTempPath(),
                                          $"DeleteTrayTemps_{Guid.NewGuid():N}.bat");

            string script = $@"@echo off
setlocal
cd /d ""%~dp0""

schtasks /Delete /TN ""{AppName}"" /F > nul 2>&1
sc stop ""R0TrayTemps"" > nul 2>&1

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
            return Task.Run(() =>
            {

                if (!RunProcessAndWait("schtasks.exe", $"/Delete /TN \"{AppName}\" /F", out string error))
                {
                    Debug.WriteLine("Could not remove startup task: " + error);
                }
            });
        }

        #endregion
    }
}