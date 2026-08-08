using LibreHardwareMonitor.Hardware;
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
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TrayTemps
{
    public partial class MainForm : Form
    {
        #region [ Fields / Constants ]

        private const string AppName = "TrayTemps";
        private const string EmbeddedBunkenBoldDisplayName = "Bunken Tech Sans Pro Bold (Embedded)";
        private string InstallPath;
        private string SettingsFilePath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), AppName, "settings.json");
        private const int IconSize = 16;
        private const int SmCxSmallIcon = 49;
        private const int CsDropShadow = 0x00020000;
        private const int MainMenuShadowWidth = 10;
        private const int ResizeGripLogicalPixels = 8;
        private const decimal MinimumRefreshIntervalSeconds = 0.25M;
        private const decimal DefaultRefreshIntervalSeconds = 0.50M;
        private const decimal MaximumRefreshIntervalSeconds = 10M;
        private const int MinimumIconSizePercent = 30;
        private const int MaximumIconSizePercent = 100;
        private const int DefaultIconSizePercent = 90;
        private const int StartupTaskQueryTimeoutMs = 2000;
        private static readonly TimeSpan TemperatureAlertCooldown = TimeSpan.FromSeconds(5);
        private const string GitHubTagsApiUrl = "https://api.github.com/repos/nmd-113/Tray-Temps/tags?per_page=100";
        private const string GitHubReleasePageUrl = "https://github.com/nmd-113/Tray-Temps/releases/tag/";
        private static readonly Size HardwareDialogMinimumSize = new Size(640, 440);
        private static readonly HttpClient UpdateCheckClient = CreateUpdateCheckClient();

        private Computer _computer;
        private readonly Timer _tempTimer = new Timer();

        private List<IHardware> _cpuHardwares;
        private IHardware _selectedCpuHardware;
        private string _selectedCpuIdentifier;
        private string _savedCpuTemperatureSensorIdentifier;

        private List<IHardware> _gpuHardwares;
        private IHardware _selectedGpuHardware;
        private string _selectedGpuIdentifier;
        private string _savedGpuTemperatureSensorIdentifier;

        private List<IHardware> _storageHardwares;
        private List<string> _wmiStorageDisplayNames = new List<string>();

        private int _savedCpuIndex = 0;
        private int _savedGpuIndex = 0;
        private string _savedCpuIdentifier;
        private string _savedGpuIdentifier;

        public int WarmTempMin;
        public int WarmTempMax;

        public Color NormalColor;
        public Color WarningColor;
        public Color CriticalColor;
        public bool TemperatureAlertsEnabled { get; set; }

        public FontFamily BunkenBold;
        public FontFamily BunkenRegular;
        public bool IsLightModeEnabled => lightModeSwitch != null && lightModeSwitch.Checked;
        public bool UsesFahrenheit => tempsFahrenheit != null && tempsFahrenheit.Checked;
        private bool _showTemperatureColorCorners = true;
        internal bool ShowTemperatureColorCorners
        {
            get => _showTemperatureColorCorners;
            set
            {
                _showTemperatureColorCorners = value;
                UpdateTrayColorLabels();
            }
        }

        private bool ShowDeviceIdentityMarkers =>
            colortempsEnable != null && colortempsEnable.Checked && ShowTemperatureColorCorners;
        public event EventHandler ThemeChanged;

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
        private bool _desiredEnableCpuTray = false;
        private bool _desiredEnableGpuTray = false;
        private bool _desiredSingleIconTray = false;
        private bool _desiredColorTempsEnabled = false;
        private bool _startMinimizedWithAdminRights = true;
        private bool _isShutdownInitiated = false;
        private bool _isRefreshingTemps = false;
        private bool _resourcesDisposed = false;
        private bool _sensorElevationPromptShown = false;
        private bool _isInitializingHardwareSelectors = false;
        private bool _isInitializingTemperatureSensorSelectors = false;
        private bool _temperatureTimerConfigured = false;
        private readonly bool _startHiddenFromCommandLine;
        private readonly bool _forceVisibleOnStartup;
        private bool _initialWindowDisplaySuppressed = true;
        private int _cpuInvalidSensorCycles;
        private int _gpuInvalidSensorCycles;
        private bool _cpuHotAlertRaised;
        private bool _gpuHotAlertRaised;
        private DateTime _nextTemperatureAlertUtc = DateTime.MinValue;
        private Rectangle? _savedHardwareDialogBounds;
        private Rectangle? _lastNormalWindowBounds;
        private bool _restoreLastWindowBoundsWhenShown;
        private bool _explicitShowPending;

        private bool IsStartMinimizedWithAdminRights =>
            minimizeOnStart != null && minimizeOnStart.Checked && _startMinimizedWithAdminRights;

        private bool ShouldStartHidden =>
            !_forceVisibleOnStartup &&
            (_startHiddenFromCommandLine || (minimizeOnStart != null && minimizeOnStart.Checked));

        private bool ShouldDpiRestartHidden =>
            !_explicitShowPending &&
            (!Visible || WindowState == FormWindowState.Minimized || _restoreLastWindowBoundsWhenShown);

        private readonly object _hardwareUpdateLock = new object();
        private readonly List<HardwareDetailsDialog> _openHardwareDialogs = new List<HardwareDetailsDialog>();

        private string _trayFontFamily;
        private int _lastTrayIconPixelSize;
        private float _dpiScale = 1f;
        private bool _dpiMonitoringReady;
        private bool _dpiRestartPending;
        private bool _windowMoveResizeActive;
        private DpiRestartRequest? _deferredDpiRestart;
        private Color _darkWindowBackColor = Color.FromArgb(21, 21, 21);
        private Color _darkPanelBackColor = Color.FromArgb(25, 25, 25);

        private Font _trayFont;
        private SolidBrush _cpuBrush;
        private SolidBrush _gpuBrush;
        private readonly Dictionary<string, TrayGlyphMetrics> _trayGlyphMetricsCache = new Dictionary<string, TrayGlyphMetrics>();

        #endregion

        #region [ Native Methods ]

        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int WM_DPICHANGED = 0x02E0;
        private const int HTCAPTION = 0x2;
        private const int HTBOTTOMRIGHT = 17;

        [DllImport("user32.dll")]
        private static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        [DllImport("user32.dll")]
        private static extern int GetSystemMetrics(int nIndex);

        [DllImport("user32.dll")]
        private static extern uint GetDpiForSystem();

        [DllImport("user32.dll")]
        private static extern int GetSystemMetricsForDpi(int nIndex, uint dpi);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool DestroyIcon(IntPtr hIcon);

        [StructLayout(LayoutKind.Sequential)]
        private struct NativeRect
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;

            public Rectangle ToRectangle()
            {
                return Rectangle.FromLTRB(Left, Top, Right, Bottom);
            }
        }

        private struct DpiRestartRequest
        {
            public DpiRestartRequest(int newDpi, Rectangle? normalBounds, Rectangle? suggestedBounds)
            {
                NewDpi = newDpi;
                NormalBounds = normalBounds;
                SuggestedBounds = suggestedBounds;
            }

            public int NewDpi;
            public Rectangle? NormalBounds;
            public Rectangle? SuggestedBounds;
        }

        #endregion

        #region [ Custom Controls ]

        private sealed class ResizeGripPanel : Panel
        {
            public ResizeGripPanel()
            {
                SetStyle(
                    ControlStyles.UserPaint |
                    ControlStyles.AllPaintingInWmPaint |
                    ControlStyles.OptimizedDoubleBuffer |
                    ControlStyles.SupportsTransparentBackColor,
                    true);
            }
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            if (!_dpiMonitoringReady)
                RefreshCurrentDpiScale();

            WindowCornerHelper.ApplyRoundedCorners(Handle);
        }

        private void RefreshCurrentDpiScale()
        {
            using (Graphics graphics = Graphics.FromHwnd(Handle))
            {
                if (graphics.DpiX > 0)
                    _dpiScale = graphics.DpiX / 96f;
            }
        }

        #endregion

        #region [ Constructor / Form Lifecycle ]

        public MainForm(bool startHiddenFromCommandLine = false, bool forceVisibleOnStartup = false)
        {
            _startHiddenFromCommandLine = startHiddenFromCommandLine;
            _forceVisibleOnStartup = forceVisibleOnStartup;
            LoadFonts();
            InitializeComponent();

            _darkPanelBackColor = panelWrapper.BackColor;
            _darkWindowBackColor = Color.FromArgb(21, 21, 21);

            EmbeddedFonts.ApplyTo(this);
            PopulateFontFamilyOptions();
            MainFormShadowHelper.InitializeCardShadows(
                MainFormShadowHelper.GetShadowCards(
                    mainComponentsPanel,
                    gpuPanel,
                    cpuPanel,
                    generalSettingsPanel,
                    traySettingsPanel,
                    appAboutExtra),
                ShadowCardParent_Paint,
                CardShadowParent_Changed);
            MainFormShadowHelper.InitializeMainMenuShadow(
                MainFormShadowHelper.GetMainMenuShadowHosts(homePage, settingsPage, aboutPage),
                mainMenu,
                ShadowMainMenuHost_Paint,
                MainMenuShadow_Changed);
            ApplyLabelHover(cpuModel,
                gpuModel,
                ramDetails,
                storageDetails,
                motherboardDetails);

            // WinForms shows the startup form before its Load handler can hide it.
            // Keep that first show invisible until the loaded settings decide whether
            // the app should open normally or remain in the notification area.
            Opacity = 0;
            ShowInTaskbar = false;
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

        private async void MainForm_Load(object sender, EventArgs e)
        {
            try
            {
                appVersion.Text = $"Version: {Application.ProductVersion}";

                SetDefaultControlValues();
                LoadSettings();
                RefreshCurrentDpiScale();
                _dpiMonitoringReady = true;
                RememberNormalWindowBounds();
                CacheDisplaySettings();

                if (!PromptForElevationAtStartupIfNeeded())
                    return;

                bool startHidden = ShouldStartHidden;

                if (!startHidden)
                    RestoreInitialWindowDisplay();

                UpdateTrayIcons();

                if (startHidden)
                    BeginInvoke((MethodInvoker)HideToTrayAfterStartup);

                SelectedTabChanged(this, EventArgs.Empty);

                await InitializeHardwareAsync();

                SetupTimer();
                TempTimer_Tick(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Unhandled exception in MainForm_Load: " + ex);
                RestoreInitialWindowDisplay();
                try { MessageBox.Show(this, $"An unexpected error occurred:\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); } catch { Debug.WriteLine("Failed to show error MessageBox in MainForm_Load."); }
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_settingsLoaded)
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
            if (WindowState == FormWindowState.Minimized)
            {
                HideToTray();
                return;
            }

            if (resizeGrip != null && !resizeGrip.IsDisposed)
                UpdateResizeGripSize();
            RememberNormalWindowBounds();
        }

        protected override void OnLocationChanged(EventArgs e)
        {
            base.OnLocationChanged(e);
            RememberNormalWindowBounds();
        }

        private void QueueDpiRestart(DpiRestartRequest request)
        {
            if (_dpiRestartPending || IsDisposed || _isShutdownInitiated || _resourcesDisposed)
                return;

            _dpiRestartPending = true;

            BeginInvoke((MethodInvoker)delegate
            {
                if (IsDisposed || _isShutdownInitiated || _resourcesDisposed)
                {
                    return;
                }

                bool startHidden = ShouldDpiRestartHidden;

                if (!startHidden && request.SuggestedBounds.HasValue &&
                    request.SuggestedBounds.Value.Width > 0 && request.SuggestedBounds.Value.Height > 0)
                {
                    _lastNormalWindowBounds = request.SuggestedBounds.Value;
                }
                else if (request.NormalBounds.HasValue)
                {
                    float scaleFactor = request.NewDpi / (_dpiScale * 96f);
                    _lastNormalWindowBounds = ScaleWindowBounds(request.NormalBounds.Value, scaleFactor);
                }

                if (!Program.RequestDpiRestart(startHidden))
                {
                    _dpiRestartPending = false;
                    _dpiScale = request.NewDpi / 96f;

                    if (_explicitShowPending)
                    {
                        _explicitShowPending = false;
                        ShowWindow();
                    }
                    else if (!startHidden)
                    {
                        RestoreInitialWindowDisplay();
                    }

                    return;
                }

                Hide();
                Close();
            });
        }

        private static Rectangle ScaleWindowBounds(Rectangle bounds, float scaleFactor)
        {
            return new Rectangle(
                bounds.Location,
                new Size(
                    Math.Max(1, (int)Math.Round(bounds.Width * scaleFactor)),
                    Math.Max(1, (int)Math.Round(bounds.Height * scaleFactor))));
        }

        protected override void WndProc(ref Message m)
        {
            const int WM_NCHITTEST = 0x84;
            const int WM_ENTERSIZEMOVE = 0x0231;
            const int WM_EXITSIZEMOVE = 0x0232;
            const int HTLEFT = 10, HTRIGHT = 11, HTTOP = 12, HTTOPLEFT = 13;
            const int HTTOPRIGHT = 14, HTBOTTOM = 15, HTBOTTOMLEFT = 16;

            if (m.Msg == WM_ENTERSIZEMOVE)
            {
                _windowMoveResizeActive = true;
                _deferredDpiRestart = null;
            }

            int newDpi = m.Msg == WM_DPICHANGED
                ? unchecked((int)((long)m.WParam & 0xFFFF))
                : 0;
            bool dpiActuallyChanged = _dpiMonitoringReady &&
                                      newDpi > 0 &&
                                      Math.Abs(newDpi - (_dpiScale * 96f)) >= 1f;
            bool restartHidden = ShouldDpiRestartHidden;
            Rectangle? normalBounds = WindowState == FormWindowState.Normal && Visible
                ? Bounds
                : _lastNormalWindowBounds;
            Rectangle? suggestedBounds = dpiActuallyChanged && !restartHidden && m.LParam != IntPtr.Zero
                ? Marshal.PtrToStructure<NativeRect>(m.LParam).ToRectangle()
                : (Rectangle?)null;
            DpiRestartRequest? dpiRestart = dpiActuallyChanged
                ? new DpiRestartRequest(newDpi, normalBounds, suggestedBounds)
                : (DpiRestartRequest?)null;

            base.WndProc(ref m);

            if (m.Msg == WM_DPICHANGED)
            {
                if (_windowMoveResizeActive)
                    _deferredDpiRestart = dpiRestart;
                else if (dpiRestart.HasValue)
                    QueueDpiRestart(dpiRestart.Value);
            }

            if (m.Msg == WM_EXITSIZEMOVE)
            {
                _windowMoveResizeActive = false;

                if (_deferredDpiRestart.HasValue)
                {
                    DpiRestartRequest request = _deferredDpiRestart.Value;

                    if (Visible && WindowState == FormWindowState.Normal)
                    {
                        request = new DpiRestartRequest(
                            request.NewDpi,
                            request.NormalBounds,
                            Bounds);
                    }

                    QueueDpiRestart(request);
                }

                _deferredDpiRestart = null;
            }

            if (m.Msg == WM_NCHITTEST)
            {
                int resizeAreaSize = GetResizeGripSize();
                int x = (short)(m.LParam.ToInt64() & 0xFFFF);
                int y = (short)((m.LParam.ToInt64() >> 16) & 0xFFFF);
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

        private int GetResizeGripSize()
        {
            return Math.Max(6, (int)Math.Round(ResizeGripLogicalPixels * _dpiScale));
        }

        private void UpdateResizeGripSize()
        {
            if (resizeGrip == null || resizeGrip.IsDisposed)
                return;

            int size = GetResizeGripSize() * 2 + 4;
            Size desiredSize = new Size(size, size);

            if (resizeGrip.Size != desiredSize)
                resizeGrip.Size = desiredSize;

            resizeGrip.Location = new Point(
                Math.Max(0, panelWrapper.ClientSize.Width - size),
                Math.Max(0, panelWrapper.ClientSize.Height - size));
        }

        private void ResizeGrip_Paint(object sender, PaintEventArgs e)
        {
            if (!(sender is Control grip))
                return;

            float dpiScale = _dpiScale;
            int inset = Math.Max(3, (int)Math.Round(3f * dpiScale));
            int spacing = Math.Max(3, (int)Math.Round(3f * dpiScale));
            int length = Math.Max(5, (int)Math.Round(5f * dpiScale));
            Color color = IsLightModeEnabled
                ? Color.FromArgb(125, 75, 75, 75)
                : Color.FromArgb(65, 170, 170, 170);

            using (var pen = new Pen(color, Math.Max(1f, dpiScale)))
            {
                for (int i = 0; i < 3; i++)
                {
                    int offset = i * spacing;
                    e.Graphics.DrawLine(
                        pen,
                        grip.Width - inset - length - offset,
                        grip.Height - inset,
                        grip.Width - inset,
                        grip.Height - inset - length - offset);
                }
            }
        }

        private void ResizeGrip_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left || WindowState == FormWindowState.Maximized)
                return;

            ReleaseCapture();
            SendMessage(Handle, WM_NCLBUTTONDOWN, HTBOTTOMRIGHT, 0);
        }

        private void MainForm_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HTCAPTION, 0);
            }
        }

        #endregion

        #region [ Startup / Hardware Initialization ]

        private async Task InitializeHardwareAsync()
        {
            string motherboard = await HardwareInfoQueryHelper.GetMotherboardNameAsync();
            string ram = await HardwareInfoQueryHelper.GetRamInfoAsync();

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
                    List<string> wmiStorageDisplayNames;

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
                        {
                            try
                            {
                                hardware.Update();
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine("Hardware update failed during initialization: " + ex);
                            }
                        }

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

                    if (storageHardwares == null || storageHardwares.Count == 0)
                        wmiStorageDisplayNames = LoadWmiStorageDisplayNames();
                    else
                        wmiStorageDisplayNames = new List<string>();

                    if (!IsHandleCreated || IsDisposed || _isShutdownInitiated || _resourcesDisposed)
                        return;

                    Invoke((MethodInvoker)delegate
                    {
                        if (_isShutdownInitiated || _resourcesDisposed || IsDisposed)
                            return;

                        _isInitializingHardwareSelectors = true;

                        try
                        {
                            PopulateHardwareSelector(cpuIndexSelect, _cpuHardwares, GetSavedHardwareIndex(_cpuHardwares, _savedCpuIdentifier, _savedCpuIndex), cpuModel, "CPU");
                            PopulateHardwareSelector(gpuIndexSelect, _gpuHardwares, GetSavedHardwareIndex(_gpuHardwares, _savedGpuIdentifier, _savedGpuIndex), gpuModel, "GPU");
                            cpuConfigButton.Enabled = _cpuHardwares != null && _cpuHardwares.Count > 0;
                            gpuConfigButton.Enabled = _gpuHardwares != null && _gpuHardwares.Count > 0;
                            _wmiStorageDisplayNames = wmiStorageDisplayNames;
                            UpdateStorageDetailsText();
                        }
                        finally
                        {
                            _isInitializingHardwareSelectors = false;
                        }

                        ApplySelectedCpuHardwareFromCurrentIndex(updateHardware: false);
                        ApplySelectedGpuHardwareFromCurrentIndex(updateHardware: false);
                        UpdateTemperatureTrayCheckboxAvailability();

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

                        PromptForElevationIfCriticalSensorsAreMissing();
                    });
                }
                catch (Exception ex)
                {
                    try
                    {
                        newComputer?.Close();
                    }
                    catch (Exception innerEx)
                    {
                        Debug.WriteLine(innerEx.ToString());
                    }

                    if (!_isShutdownInitiated && !_resourcesDisposed && IsHandleCreated && !IsDisposed)
                    {
                        try
                        {
                            Invoke((MethodInvoker)delegate
                            {
                                if (!IsRunningAsAdministrator() && PromptForElevatedSensorRestart("Hardware initialization failed: " + ex.Message))
                                    return;

                                MessageBox.Show(
                                    this,
                                    $"Hardware initialization failed.\n\n{ex.Message}",
                                    "Hardware Error",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                            });
                        }
                        catch (Exception innerEx)
                        {
                            Debug.WriteLine(innerEx.ToString());
                        }
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
            _temperatureTimerConfigured = true;
            _tempTimer.Start();
        }

        private void UpdateTimerInterval()
        {
            _tempTimer.Interval = Math.Max(250, (int)(GetSelectedRefreshInterval() * 1000));
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
            catch (Exception ex)
            {
                Debug.WriteLine("TempTimer_Tick: failed to stop timer: " + ex);
            }

            try
            {
                await Task.Run(() =>
                {
                    lock (_hardwareUpdateLock)
                    {
                        if (_isShutdownInitiated || _resourcesDisposed || _computer == null)
                            return;

                        _selectedCpuHardware?.Update();
                        RefreshCpuFallbackSensorHardware();

                        if (_selectedGpuHardware != null)
                            UpdateHardwareRecursive(_selectedGpuHardware);

                        RefreshUnavailableTemperatureSensors();
                    }
                });

                if (_isShutdownInitiated || _resourcesDisposed || IsDisposed || !IsHandleCreated)
                    return;

                float? cpuTemp = TemperatureFormatHelper.IsValidTemp(_cpuTempSensor?.Value)
                    ? _cpuTempSensor.Value
                    : null;

                float? gpuTemp = TemperatureFormatHelper.IsValidTemp(_gpuTempSensor?.Value)
                    ? _gpuTempSensor.Value
                    : null;

                UpdateTemperatures(cpuTemp, gpuTemp);
                UpdateTemperatureTrayCheckboxAvailability();
                UpdateAllTrayIcons(cpuTemp, gpuTemp);
                EvaluateTemperatureAlerts(cpuTemp, gpuTemp);
            }
            catch (Exception ex)
            {
                // Hardware read errors can be transient.
                Debug.WriteLine("TempTimer_Tick error: " + ex);
            }
            finally
            {
                _isRefreshingTemps = false;

                if (_temperatureTimerConfigured && !_isShutdownInitiated && !_resourcesDisposed && IsHandleCreated && !IsDisposed)
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

        #endregion

        #region [ Timer / Temperature Refresh ]

        private void ShowWindow()
        {
            _explicitShowPending = true;
            bool restorationStarted = false;

            try
            {
                if (_dpiRestartPending)
                    return;

                restorationStarted = true;
                WindowState = FormWindowState.Normal;
                // Keep bounds tracking suspended until the form is visible again.
                // Windows can report a temporary position while displays resume.
                RestoreLastWindowBoundsWhenShown();
                Show();

                if (_dpiRestartPending)
                    return;

                RestoreInitialWindowDisplay();

                if (_dpiRestartPending)
                    return;

                BringToFront();
                Activate();
                UpdateTrayIcons();
            }
            finally
            {
                if (restorationStarted)
                    _restoreLastWindowBoundsWhenShown = false;

                if (!_dpiRestartPending)
                    _explicitShowPending = false;
            }
        }

        private void RestoreInitialWindowDisplay()
        {
            if (!_initialWindowDisplaySuppressed || IsDisposed)
                return;

            ShowInTaskbar = true;

            if (_dpiRestartPending)
                return;

            Opacity = 1;
            _initialWindowDisplaySuppressed = false;
        }

        private void RememberNormalWindowBounds()
        {
            if (!Visible || _restoreLastWindowBoundsWhenShown || WindowState != FormWindowState.Normal)
                return;

            Rectangle bounds = Bounds;
            if (bounds.Width > 0 && bounds.Height > 0 && IsWindowBoundsVisible(bounds))
                _lastNormalWindowBounds = bounds;
        }

        private void RestoreLastWindowBoundsWhenShown()
        {
            if (!_restoreLastWindowBoundsWhenShown)
                return;

            if (_lastNormalWindowBounds.HasValue && IsWindowBoundsVisible(_lastNormalWindowBounds.Value))
                Bounds = _lastNormalWindowBounds.Value;
            else
                CenterToScreen();
        }

        private void UpdateTrayIcons()
        {
            if (_isShutdownInitiated || _resourcesDisposed)
                return;

            bool cpuChecked = enableCpuTray != null && enableCpuTray.Checked;
            bool gpuChecked = enableGpuTray != null && enableGpuTray.Checked;
            bool combinedTray = singleIconTray != null && singleIconTray.Checked && cpuChecked && gpuChecked;

            if (cpuTrayIcon != null)
                cpuTrayIcon.Visible = cpuChecked;

            if (gpuTrayIcon != null)
                gpuTrayIcon.Visible = gpuChecked && !combinedTray;

            bool isHidden = !Visible || WindowState == FormWindowState.Minimized;

            if (NotifyIcon != null)
                NotifyIcon.Visible = isHidden && !cpuChecked && !gpuChecked;
        }

        private void UpdateTemperatures(float? cpuTemp, float? gpuTemp)
        {
            bool useFahrenheit = tempsFahrenheit.Checked;
            string unit = TemperatureFormatHelper.GetUnit(useFahrenheit);

            if (cpuTemp.HasValue)
            {
                float temp = cpuTemp.Value;
                if (temp < _cpuMinTemp) _cpuMinTemp = temp;
                if (temp > _cpuMaxTemp) _cpuMaxTemp = temp;

                SetTextIfChanged(cpuTempCur, $"{TemperatureFormatHelper.GetDisplayTemp(temp, useFahrenheit):F0}{unit}");
                SetTextIfChanged(cpuTempMin, $"{TemperatureFormatHelper.GetDisplayTemp(_cpuMinTemp, useFahrenheit):F0}{unit}");
                SetTextIfChanged(cpuTempMax, $"{TemperatureFormatHelper.GetDisplayTemp(_cpuMaxTemp, useFahrenheit):F0}{unit}");
            }
            else
            {
                SetTextIfChanged(cpuTempCur, "N/A");
                SetTextIfChanged(cpuTempMin, "N/A");
                SetTextIfChanged(cpuTempMax, "N/A");
            }

            if (gpuTemp.HasValue)
            {
                float temp = gpuTemp.Value;
                if (temp < _gpuMinTemp) _gpuMinTemp = temp;
                if (temp > _gpuMaxTemp) _gpuMaxTemp = temp;

                SetTextIfChanged(gpuTempCur, $"{TemperatureFormatHelper.GetDisplayTemp(temp, useFahrenheit):F0}{unit}");
                SetTextIfChanged(gpuTempMin, $"{TemperatureFormatHelper.GetDisplayTemp(_gpuMinTemp, useFahrenheit):F0}{unit}");
                SetTextIfChanged(gpuTempMax, $"{TemperatureFormatHelper.GetDisplayTemp(_gpuMaxTemp, useFahrenheit):F0}{unit}");
            }
            else
            {
                SetTextIfChanged(gpuTempCur, "N/A");
                SetTextIfChanged(gpuTempMin, "N/A");
                SetTextIfChanged(gpuTempMax, "N/A");
            }
        }

        private void EvaluateTemperatureAlerts(float? cpuTemp, float? gpuTemp)
        {
            if (!TemperatureAlertsEnabled)
            {
                ResetTemperatureAlertState();
                return;
            }

            UpdateHotTemperatureAlert(ref _cpuHotAlertRaised, "CPU", cpuTemp);
            UpdateHotTemperatureAlert(ref _gpuHotAlertRaised, "GPU", gpuTemp);
        }

        private void UpdateHotTemperatureAlert(ref bool alertRaised, string deviceName, float? temperature)
        {
            if (!temperature.HasValue || temperature.Value <= WarmTempMax)
            {
                alertRaised = false;
                return;
            }

            if (alertRaised)
                return;

            if (DateTime.UtcNow < _nextTemperatureAlertUtc)
                return;

            NotifyIcon alertIcon = GetVisibleTrayIconForAlert();
            if (alertIcon == null)
                return;

            bool useFahrenheit = UsesFahrenheit;
            string unit = TemperatureFormatHelper.GetUnit(useFahrenheit);
            float displayTemperature = TemperatureFormatHelper.GetDisplayTemp(temperature.Value, useFahrenheit);
            float displayThreshold = TemperatureFormatHelper.GetDisplayTemp(WarmTempMax, useFahrenheit);

            try
            {
                alertIcon.ShowBalloonTip(
                    5000,
                    $"TrayTemps: {deviceName} temperature is Hot",
                    $"{deviceName} is {displayTemperature:F0}{unit}. Hot threshold: {displayThreshold:F0}{unit}.",
                    ToolTipIcon.Warning);
                alertRaised = true;
                _nextTemperatureAlertUtc = DateTime.UtcNow + TemperatureAlertCooldown;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Temperature alert failed: " + ex);
            }
        }

        private void ResetTemperatureAlertState()
        {
            _cpuHotAlertRaised = false;
            _gpuHotAlertRaised = false;
            _nextTemperatureAlertUtc = DateTime.MinValue;
        }

        private NotifyIcon GetVisibleTrayIconForAlert()
        {
            if (cpuTrayIcon?.Visible == true)
                return cpuTrayIcon;

            if (gpuTrayIcon?.Visible == true)
                return gpuTrayIcon;

            if (NotifyIcon?.Visible == true)
                return NotifyIcon;

            return null;
        }

        private static void SetTextIfChanged(Control control, string text)
        {
            if (control.Text != text)
                control.Text = text;
        }

        private static void SetNotifyIconTextIfChanged(NotifyIcon icon, string text)
        {
            if (icon.Text != text)
                icon.Text = text;
        }

        private void UpdateAllTrayIcons(float? cpuTemp, float? gpuTemp)
        {
            RefreshTrayIconRenderSize();

            bool useFahrenheit = tempsFahrenheit.Checked;
            string unit = TemperatureFormatHelper.GetUnit(useFahrenheit);
            string cpuHover = cpuTemp.HasValue ? $"{TemperatureFormatHelper.GetDisplayTemp(cpuTemp.Value, useFahrenheit):F0}{unit}" : "N/A";
            string gpuHover = gpuTemp.HasValue ? $"{TemperatureFormatHelper.GetDisplayTemp(gpuTemp.Value, useFahrenheit):F0}{unit}" : "N/A";

            if (colortempsEnable.Checked)
            {
                if (cpuTemp.HasValue)
                {
                    float val = cpuTemp.Value;
                    if (val < WarmTempMin) _cpuBrush.Color = NormalColor;
                    else if (val <= WarmTempMax) _cpuBrush.Color = WarningColor;
                    else _cpuBrush.Color = CriticalColor;
                }
                else
                {
                    _cpuBrush.Color = Color.Gray;
                }
                if (gpuTemp.HasValue)
                {
                    float val = gpuTemp.Value;
                    if (val < WarmTempMin) _gpuBrush.Color = NormalColor;
                    else if (val <= WarmTempMax) _gpuBrush.Color = WarningColor;
                    else _gpuBrush.Color = CriticalColor;
                }
                else
                {
                    _gpuBrush.Color = Color.Gray;
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
                string cpuText = TemperatureFormatHelper.FormatTrayTemperature(cpuTemp, useFahrenheit);
                string gpuText = TemperatureFormatHelper.FormatTrayTemperature(gpuTemp, useFahrenheit);

                cpuTrayIcon.Visible = true;
                gpuTrayIcon.Visible = false;
                UpdateCombinedTrayIcon(
                    cpuTrayIcon,
                    cpuText,
                    gpuText);
                SetNotifyIconTextIfChanged(cpuTrayIcon, $"CPU: {cpuHover} | GPU: {gpuHover}");
                return;
            }

            string cpuTrayText = TemperatureFormatHelper.FormatTrayTemperature(cpuTemp, useFahrenheit);
            string gpuTrayText = TemperatureFormatHelper.FormatTrayTemperature(gpuTemp, useFahrenheit);
            string sharedReferenceText = TrayTextHelper.GetTrayReferenceText(
                enableCpuTray.Checked ? cpuTrayText : null,
                enableGpuTray.Checked ? gpuTrayText : null);

            UpdateNonCombinedTrayIcons(cpuTrayText, gpuTrayText, sharedReferenceText);

            if (enableCpuTray.Checked)
                SetNotifyIconTextIfChanged(cpuTrayIcon, $"CPU: {cpuHover}");

            if (enableGpuTray.Checked)
                SetNotifyIconTextIfChanged(gpuTrayIcon, $"GPU: {gpuHover}");

            if (!enableCpuTray.Checked && !enableGpuTray.Checked)
            {
                SetNotifyIconTextIfChanged(NotifyIcon, $"CPU: {cpuHover} | GPU: {gpuHover}");
            }
            else
            {
                SetNotifyIconTextIfChanged(NotifyIcon, AppName);
            }
        }

        private void UpdateCombinedTrayIcon(NotifyIcon icon, string cpuText, string gpuText)
        {
            if (icon == null || _cpuBrush == null || _gpuBrush == null)
                return;

            bool showDeviceIdentityMarkers = ShowDeviceIdentityMarkers;
            string cacheKey = TrayIconCacheKeyHelper.GetCombinedTrayCacheKey(
                cpuText,
                gpuText,
                _cpuBrush.Color.ToArgb(),
                _gpuBrush.Color.ToArgb(),
                showDeviceIdentityMarkers,
                showDeviceIdentityMarkers ? cpuColorValue.BackColor.ToArgb() : 0,
                showDeviceIdentityMarkers ? gpuColorValue.BackColor.ToArgb() : 0);

            if (cacheKey == _lastCpuTempText)
                return;

            Icon newIcon = CreateCombinedTempIcon(
                cpuText,
                gpuText,
                showDeviceIdentityMarkers ? cpuColorValue.BackColor : (Color?)null,
                showDeviceIdentityMarkers ? gpuColorValue.BackColor : (Color?)null);

            ReplaceTrayIconImage(icon, newIcon);

            _lastCpuTempText = cacheKey;
        }

        private void UpdateNonCombinedTrayIcons(string cpuText, string gpuText, string referenceText)
        {
            bool cpuEnabled = enableCpuTray.Checked;
            bool gpuEnabled = enableGpuTray.Checked;
            bool showDeviceIdentityMarkers = ShowDeviceIdentityMarkers;

            if ((!cpuEnabled && !gpuEnabled) || _trayFont == null || _cpuBrush == null || _gpuBrush == null)
                return;

            string cpuCacheKey = cpuEnabled
                ? TrayIconCacheKeyHelper.GetSingleTrayCacheKey(
                    cpuText,
                    referenceText,
                    _cpuBrush.Color.ToArgb(),
                    showDeviceIdentityMarkers,
                    showDeviceIdentityMarkers ? cpuColorValue.BackColor.ToArgb() : 0)
                : null;
            string gpuCacheKey = gpuEnabled
                ? TrayIconCacheKeyHelper.GetSingleTrayCacheKey(
                    gpuText,
                    referenceText,
                    _gpuBrush.Color.ToArgb(),
                    showDeviceIdentityMarkers,
                    showDeviceIdentityMarkers ? gpuColorValue.BackColor.ToArgb() : 0)
                : null;
            bool cpuChanged = cpuEnabled && cpuCacheKey != _lastCpuTempText;
            bool gpuChanged = gpuEnabled && gpuCacheKey != _lastGpuTempText;

            if (!cpuChanged && !gpuChanged)
                return;

            bool refreshTogether = cpuEnabled && gpuEnabled;
            Icon newCpuIcon = null;
            Icon newGpuIcon = null;

            try
            {
                TrayPathTextLayout sharedLayout = CreateSingleTrayTextLayout(cpuEnabled, gpuEnabled, cpuText, gpuText, referenceText);

                if (cpuEnabled && (refreshTogether || cpuChanged))
                    newCpuIcon = CreateTempIcon(
                        cpuText,
                        _cpuBrush,
                        sharedLayout,
                        showDeviceIdentityMarkers ? cpuColorValue.BackColor : (Color?)null);

                if (gpuEnabled && (refreshTogether || gpuChanged))
                    newGpuIcon = CreateTempIcon(
                        gpuText,
                        _gpuBrush,
                        sharedLayout,
                        showDeviceIdentityMarkers ? gpuColorValue.BackColor : (Color?)null);
            }
            catch
            {
                newCpuIcon?.Dispose();
                newGpuIcon?.Dispose();
                throw;
            }

            if (newCpuIcon != null)
            {
                ReplaceTrayIconImage(cpuTrayIcon, newCpuIcon);
                _lastCpuTempText = cpuCacheKey;
            }

            if (newGpuIcon != null)
            {
                ReplaceTrayIconImage(gpuTrayIcon, newGpuIcon);
                _lastGpuTempText = gpuCacheKey;
            }
        }

        private TrayPathTextLayout CreateSingleTrayTextLayout(
            bool cpuEnabled,
            bool gpuEnabled,
            string cpuText,
            string gpuText,
            string referenceText)
        {
            int size = GetTrayIconPixelSize();
            float occupancy = GetTrayTextOccupancy();
            var texts = new List<string>();

            if (cpuEnabled)
                texts.Add(cpuText);

            if (gpuEnabled)
                texts.Add(gpuText);

            float textHeight = size - (ShowDeviceIdentityMarkers ? GetDeviceIdentityLineReservedSpace(size) : 0f);

            return CreateTrayTextLayout(
                texts,
                referenceText,
                _trayFont,
                new RectangleF(0, 0, size, textHeight),
                size * occupancy,
                textHeight * occupancy,
                GetSingleTrayTextPadding(),
                GetSingleTrayOutlineWidth());
        }

        private Icon CreateTempIcon(string text, SolidBrush brush, TrayPathTextLayout layout, Color? deviceMarkerColor)
        {
            int size = GetTrayIconPixelSize();

            using (var bmp = new Bitmap(size, size, PixelFormat.Format32bppArgb))
            using (var g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.Transparent);
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.TextRenderingHint = TextRenderingHint.AntiAlias;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.CompositingQuality = CompositingQuality.HighQuality;

                float textHeight = size - (deviceMarkerColor.HasValue ? GetDeviceIdentityLineReservedSpace(size) : 0f);
                var textTarget = new RectangleF(0, 0, size, textHeight);

                DrawTrayPathText(
                    g,
                    text,
                    _trayFont,
                    layout,
                    textTarget,
                    brush);

                if (deviceMarkerColor.HasValue)
                    DrawDeviceIdentityLine(g, new RectangleF(0, 0, size, size), deviceMarkerColor.Value, DeviceIdentityLineEdge.Bottom);

                return CreateOwnedIconFromBitmap(bmp);
            }
        }

        private int GetTrayIconPixelSize()
        {
            try
            {
                uint systemDpi = GetDpiForSystem();
                int dpiAwareSize = GetSystemMetricsForDpi(SmCxSmallIcon, systemDpi);
                if (dpiAwareSize > 0)
                    return dpiAwareSize;
            }
            catch (EntryPointNotFoundException)
            {
                // Windows versions before these DPI APIs use the fallback below.
            }

            int systemSmallIconSize = GetSystemMetrics(SmCxSmallIcon);
            if (systemSmallIconSize > 0)
                return systemSmallIconSize;

            return Math.Max(IconSize, (int)Math.Round(IconSize * _dpiScale));
        }

        private static float GetDeviceIdentityLineReservedSpace(int iconPixelSize)
        {
            float scale = iconPixelSize / (float)IconSize;
            float lineThickness = Math.Max(1f, scale);
            float lineGap = Math.Max(1f, scale);
            return lineThickness + lineGap;
        }

        private Icon CreateCombinedTempIcon(string cpuText, string gpuText, Color? cpuMarkerColor, Color? gpuMarkerColor)
        {
            int size = GetTrayIconPixelSize();

            using (var bmp = new Bitmap(size, size, PixelFormat.Format32bppArgb))
            using (var g = Graphics.FromImage(bmp))
            using (var combinedFont = CreateCombinedTrayFont(size))
            {
                g.Clear(Color.Transparent);
                float occupancy = GetCombinedTrayTextOccupancy();

                g.TextRenderingHint = TextRenderingHint.AntiAlias;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.CompositingQuality = CompositingQuality.HighQuality;

                float rowGap = Math.Max(1f, size * 0.04f);
                float rowHeight = (size - rowGap) / 2f;
                float lineReservedSpace = (cpuMarkerColor.HasValue || gpuMarkerColor.HasValue)
                    ? GetDeviceIdentityLineReservedSpace(size)
                    : 0f;
                bool useVerticalIdentityLines = lineReservedSpace > 0f && !UsesFahrenheit;
                float textRowHeight = useVerticalIdentityLines
                    ? rowHeight
                    : Math.Max(1f, rowHeight - lineReservedSpace);
                float textWidth = useVerticalIdentityLines
                    ? Math.Max(1f, size - lineReservedSpace)
                    : size;
                string referenceText = TrayTextHelper.GetTrayReferenceText(cpuText, gpuText);
                TrayTextLayout sharedLayout = CreateTrayTextLayout(
                    new[] { cpuText, gpuText },
                    referenceText,
                    combinedFont,
                    new RectangleF(0, 0, textWidth, textRowHeight),
                    textWidth * occupancy,
                    textRowHeight * occupancy,
                    GetCombinedTrayTextPadding(),
                    GetCombinedTrayOutlineWidth(),
                    GetCombinedTraySlotAdvanceFactor());

                DrawStableTrayText(
                    g,
                    cpuText,
                    combinedFont,
                    sharedLayout,
                    new RectangleF(useVerticalIdentityLines ? lineReservedSpace : 0f, useVerticalIdentityLines ? 0f : lineReservedSpace, textWidth, textRowHeight),
                    _cpuBrush);

                DrawStableTrayText(
                    g,
                    gpuText,
                    combinedFont,
                    sharedLayout,
                    new RectangleF(useVerticalIdentityLines ? lineReservedSpace : 0f, rowHeight + rowGap, textWidth, textRowHeight),
                    _gpuBrush);

                if (cpuMarkerColor.HasValue)
                    DrawDeviceIdentityLine(
                        g,
                        new RectangleF(0, 0, size, rowHeight),
                        cpuMarkerColor.Value,
                        useVerticalIdentityLines ? DeviceIdentityLineEdge.Left : DeviceIdentityLineEdge.Top);

                if (gpuMarkerColor.HasValue)
                    DrawDeviceIdentityLine(
                        g,
                        new RectangleF(0, rowHeight + rowGap, size, rowHeight),
                        gpuMarkerColor.Value,
                        useVerticalIdentityLines ? DeviceIdentityLineEdge.Left : DeviceIdentityLineEdge.Bottom);

                return CreateOwnedIconFromBitmap(bmp);
            }
        }

        private static Icon CreateOwnedIconFromBitmap(Bitmap bitmap)
        {
            IntPtr hIcon = IntPtr.Zero;

            try
            {
                hIcon = bitmap.GetHicon();
                return (Icon)Icon.FromHandle(hIcon).Clone();
            }
            finally
            {
                if (hIcon != IntPtr.Zero)
                    DestroyIcon(hIcon);
            }
        }

        private static void ReplaceTrayIconImage(NotifyIcon trayIcon, Icon newIcon)
        {
            Icon oldIcon = null;

            try
            {
                oldIcon = trayIcon.Icon;
                trayIcon.Icon = newIcon;
            }
            catch
            {
                newIcon?.Dispose();
                throw;
            }

            oldIcon?.Dispose();
        }

        private Font CreateCombinedTrayFont(int iconPixelSize)
        {
            float fontSize = Math.Max(IconSize, iconPixelSize) * 0.95f;
            return CreateTrayFont(_trayFontFamily, fontSize);
        }

        private void DrawStableTrayText(
            Graphics g,
            string text,
            Font font,
            TrayTextLayout layout,
            RectangleF target,
            Brush brush)
        {
            if (string.IsNullOrWhiteSpace(text))
                return;

            if (layout.Metrics.SlotWidth <= 0 || layout.Metrics.MaxHeight <= 0)
                return;

            float scale = layout.Scale;
            float textWidth = GetTrayGroupWidth(text.Length, layout.Metrics.SlotWidth, layout.SlotAdvance) * scale;
            float groupLeft = target.Left + (target.Width - textWidth) / 2f;
            float minLeft = target.Left + layout.Padding;
            float maxRight = target.Right - layout.Padding;

            if (groupLeft < minLeft)
                groupLeft = minLeft;

            if (groupLeft + textWidth > maxRight)
                groupLeft = maxRight - textWidth;

            for (int i = 0; i < text.Length; i++)
            {
                using (GraphicsPath glyphPath = CreateTrayTextPath(text[i].ToString(), font))
                {
                    RectangleF glyphBounds = glyphPath.GetBounds();
                    float slotLeft = groupLeft + (i * layout.SlotAdvance * scale);
                    float x = slotLeft + (layout.Metrics.SlotWidth * scale / 2f) - (glyphBounds.Width * scale / 2f) - (glyphBounds.X * scale);
                    float y = target.Top + (target.Height / 2f) - (glyphBounds.Height * scale / 2f) - (glyphBounds.Y * scale);

                    ClampTextTransformInsideTarget(glyphBounds, scale, target, layout.Padding, ref x, ref y);

                    GraphicsState state = g.Save();
                    g.TranslateTransform(x, y);
                    g.ScaleTransform(scale, scale);
                    DrawTrayGlyphOutline(g, glyphPath, scale, layout.OutlineWidth);
                    g.FillPath(brush, glyphPath);
                    g.Restore(state);
                }
            }
        }

        private void DrawTrayPathText(
            Graphics g,
            string text,
            Font font,
            TrayPathTextLayout layout,
            RectangleF target,
            Brush brush)
        {
            if (string.IsNullOrWhiteSpace(text) || layout.Scale <= 0)
                return;

            using (GraphicsPath textPath = CreateTrayTextPath(text, font))
            {
                RectangleF bounds = textPath.GetBounds();
                if (bounds.Width <= 0 || bounds.Height <= 0)
                    return;

                float scale = layout.Scale;
                float x = target.Left + (target.Width / 2f) - (bounds.Width * scale / 2f) - (bounds.X * scale);
                float y = target.Top + (target.Height / 2f) - (bounds.Height * scale / 2f) - (bounds.Y * scale);

                ClampTextTransformInsideTarget(bounds, scale, target, layout.Padding, ref x, ref y);

                GraphicsState state = g.Save();
                g.TranslateTransform(x, y);
                g.ScaleTransform(scale, scale);
                DrawTrayGlyphOutline(g, textPath, scale, layout.OutlineWidth);
                g.FillPath(brush, textPath);
                g.Restore(state);
            }
        }

        private static void DrawTrayGlyphOutline(Graphics g, GraphicsPath glyphPath, float scale, float outlineWidth)
        {
            float scaledOutlineWidth = outlineWidth / Math.Max(0.01f, scale);

            using (var pen = new Pen(Color.FromArgb(220, 0, 0, 0), scaledOutlineWidth))
            {
                pen.LineJoin = LineJoin.Round;
                g.DrawPath(pen, glyphPath);
            }
        }

        private enum DeviceIdentityLineEdge
        {
            Top,
            Bottom,
            Left,
            Right
        }

        private static void DrawDeviceIdentityLine(Graphics g, RectangleF target, Color color, DeviceIdentityLineEdge edge)
        {
            float scale = target.Width / IconSize;
            float lineThickness = Math.Max(1f, scale);
            float edgeInset = Math.Max(1f, scale);

            using (var brush = new SolidBrush(color))
            {
                switch (edge)
                {
                    case DeviceIdentityLineEdge.Top:
                        g.FillRectangle(brush, target.Left + edgeInset, target.Top, Math.Max(1f, target.Width - (edgeInset * 2f)), lineThickness);
                        break;

                    case DeviceIdentityLineEdge.Bottom:
                        g.FillRectangle(brush, target.Left + edgeInset, target.Bottom - lineThickness, Math.Max(1f, target.Width - (edgeInset * 2f)), lineThickness);
                        break;

                    case DeviceIdentityLineEdge.Left:
                        float leftLineHeight = Math.Min(target.Height, Math.Max(5f, 5f * scale));
                        g.FillRectangle(brush, target.Left, target.Top + ((target.Height - leftLineHeight) / 2f), lineThickness, leftLineHeight);
                        break;

                    case DeviceIdentityLineEdge.Right:
                        float rightLineHeight = Math.Min(target.Height, Math.Max(5f, 5f * scale));
                        g.FillRectangle(brush, target.Right - lineThickness, target.Top + ((target.Height - rightLineHeight) / 2f), lineThickness, rightLineHeight);
                        break;
                }
            }
        }

        private TrayPathTextLayout CreateTrayTextLayout(
            IEnumerable<string> texts,
            string referenceText,
            Font font,
            RectangleF target,
            float maxWidth,
            float maxHeight,
            float padding,
            float outlineWidth)
        {
            var textValues = (texts ?? Enumerable.Empty<string>())
                .Where(value => !string.IsNullOrWhiteSpace(value))
                .ToList();

            if (!string.IsNullOrWhiteSpace(referenceText))
                textValues.Add(referenceText);

            if (textValues.Count == 0)
                return new TrayPathTextLayout(1f, padding, outlineWidth);

            float requiredWidth = 0f;
            float requiredHeight = 0f;

            foreach (string value in textValues)
            {
                using (GraphicsPath textPath = CreateTrayTextPath(value, font))
                {
                    RectangleF bounds = textPath.GetBounds();
                    requiredWidth = Math.Max(requiredWidth, bounds.Width);
                    requiredHeight = Math.Max(requiredHeight, bounds.Height);
                }
            }

            if (requiredWidth <= 0 || requiredHeight <= 0)
                return new TrayPathTextLayout(1f, padding, outlineWidth);

            float safePadding = Math.Max(padding, outlineWidth / 2f);
            float safeMaxWidth = Math.Max(1f, Math.Min(maxWidth, target.Width - (safePadding * 2f)));
            float safeMaxHeight = Math.Max(1f, Math.Min(maxHeight, target.Height - (safePadding * 2f)));
            float scale = Math.Min(1f, Math.Min(safeMaxWidth / requiredWidth, safeMaxHeight / requiredHeight));

            return new TrayPathTextLayout(scale, safePadding, outlineWidth);
        }

        private TrayTextLayout CreateTrayTextLayout(
            IEnumerable<string> texts,
            string referenceText,
            Font font,
            RectangleF target,
            float maxWidth,
            float maxHeight,
            float padding,
            float outlineWidth,
            float slotAdvanceFactor)
        {
            string[] textValues = (texts ?? Enumerable.Empty<string>())
                .Where(value => !string.IsNullOrWhiteSpace(value))
                .ToArray();
            TrayGlyphMetrics metrics = GetTrayGlyphMetrics(textValues, font);

            if (metrics.SlotWidth <= 0 || metrics.MaxHeight <= 0)
                return new TrayTextLayout(metrics, 0f, 1f, padding, outlineWidth);

            int referenceSlots = Math.Max(
                referenceText?.Length ?? 0,
                textValues.Length > 0 ? textValues.Max(value => value.Length) : 0);
            float slotAdvance = GetTraySlotAdvance(metrics.SlotWidth, slotAdvanceFactor);
            float safePadding = Math.Max(padding, outlineWidth / 2f);
            float safeMaxWidth = Math.Max(1f, Math.Min(maxWidth, target.Width - (safePadding * 2f)));
            float safeMaxHeight = Math.Max(1f, Math.Min(maxHeight, target.Height - (safePadding * 2f)));
            float referenceWidth = GetTrayGroupWidth(referenceSlots, metrics.SlotWidth, slotAdvance);
            float scale = referenceWidth <= 0f
                ? 1f
                : Math.Min(1f, Math.Min(safeMaxWidth / referenceWidth, safeMaxHeight / metrics.MaxHeight));

            return new TrayTextLayout(metrics, slotAdvance, scale, safePadding, outlineWidth);
        }

        private TrayGlyphMetrics GetTrayGlyphMetrics(IEnumerable<string> texts, Font font)
        {
            string charSet = GetTrayMetricCharacters(texts);
            string cacheKey = GetTrayGlyphMetricsCacheKey(charSet, font);

            if (_trayGlyphMetricsCache.TryGetValue(cacheKey, out TrayGlyphMetrics cached))
                return cached;

            float maxWidth = 0f;
            float maxHeight = 0f;

            foreach (char c in charSet)
            {
                using (GraphicsPath glyphPath = CreateTrayTextPath(c.ToString(), font))
                {
                    RectangleF bounds = glyphPath.GetBounds();
                    maxWidth = Math.Max(maxWidth, bounds.Width);
                    maxHeight = Math.Max(maxHeight, bounds.Height);
                }
            }

            var metrics = new TrayGlyphMetrics(maxWidth, maxHeight);
            _trayGlyphMetricsCache[cacheKey] = metrics;
            return metrics;
        }

        private static string GetTrayMetricCharacters(IEnumerable<string> texts)
        {
            var chars = new HashSet<char>("0123456789");

            foreach (string text in texts ?? Enumerable.Empty<string>())
            {
                foreach (char c in text ?? string.Empty)
                {
                    if (!char.IsDigit(c))
                        chars.Add(c);
                }
            }

            return new string(chars.OrderBy(c => c).ToArray());
        }

        private string GetTrayGlyphMetricsCacheKey(string charSet, Font font)
        {
            return string.Format(
                CultureInfo.InvariantCulture,
                "{0}|{1}|{2:F2}|{3}",
                font.FontFamily.Name,
                (int)font.Style,
                font.Size,
                charSet);
        }

        private static float GetTraySlotAdvance(float slotWidth, float factor)
        {
            return slotWidth * factor;
        }

        private static float GetTrayGroupWidth(int slots, float slotWidth, float slotAdvance)
        {
            if (slots <= 0)
                return 0f;

            return slotWidth + ((slots - 1) * slotAdvance);
        }

        private struct TrayGlyphMetrics
        {
            public TrayGlyphMetrics(float slotWidth, float maxHeight)
            {
                SlotWidth = slotWidth;
                MaxHeight = maxHeight;
            }

            public float SlotWidth { get; }
            public float MaxHeight { get; }
        }

        private struct TrayTextLayout
        {
            public TrayTextLayout(TrayGlyphMetrics metrics, float slotAdvance, float scale, float padding, float outlineWidth)
            {
                Metrics = metrics;
                SlotAdvance = slotAdvance;
                Scale = scale;
                Padding = padding;
                OutlineWidth = outlineWidth;
            }

            public TrayGlyphMetrics Metrics { get; }
            public float SlotAdvance { get; }
            public float Scale { get; }
            public float Padding { get; }
            public float OutlineWidth { get; }
        }

        private struct TrayPathTextLayout
        {
            public TrayPathTextLayout(float scale, float padding, float outlineWidth)
            {
                Scale = scale;
                Padding = padding;
                OutlineWidth = outlineWidth;
            }

            public float Scale { get; }
            public float Padding { get; }
            public float OutlineWidth { get; }
        }

        private GraphicsPath CreateTrayTextPath(string text, Font font)
        {
            var path = new GraphicsPath();
            path.AddString(
                text,
                font.FontFamily,
                (int)font.Style,
                font.Size,
                new Point(0, 0),
                StringFormat.GenericTypographic);

            return path;
        }

        private static void ClampTextTransformInsideTarget(RectangleF textBounds, float scale, RectangleF target, float padding, ref float x, ref float y)
        {
            float left = x + textBounds.Left * scale;
            float right = x + textBounds.Right * scale;
            float top = y + textBounds.Top * scale;
            float bottom = y + textBounds.Bottom * scale;

            float minX = target.Left + padding;
            float maxX = target.Right - padding;
            float minY = target.Top + padding;
            float maxY = target.Bottom - padding;

            if (left < minX)
                x += minX - left;

            if (right > maxX)
                x -= right - maxX;

            if (top < minY)
                y += minY - top;

            if (bottom > maxY)
                y -= bottom - maxY;
        }

        private static float GetSingleTrayTextPadding()
        {
            return 0.25f;
        }

        private static float GetSingleTrayOutlineWidth()
        {
            return 0.55f;
        }

        private static float GetCombinedTrayTextPadding()
        {
            return 0.35f;
        }

        private static float GetCombinedTrayOutlineWidth()
        {
            return 0.45f;
        }

        private static float GetCombinedTraySlotAdvanceFactor()
        {
            return 1.02f;
        }

        private static float GetCombinedTrayTextOccupancy()
        {
            return 1f;
        }

        private float GetTrayTextOccupancy()
        {
            float requested = GetSelectedIconSize() / 100f;
            return Math.Max(0.3f, Math.Min(1f, requested));
        }

        private void CacheDisplaySettings()
        {
            _trayFontFamily = fontFamilyValue.Text.Trim();
            if (string.IsNullOrWhiteSpace(_trayFontFamily))
                _trayFontFamily = EmbeddedFonts.Bold.Name;

            int iconPixelSize = GetTrayIconPixelSize();
            _lastTrayIconPixelSize = iconPixelSize;
            float calculatedFontSize = iconPixelSize;

            _trayFont?.Dispose();
            _trayFont = CreateTrayFont(_trayFontFamily, calculatedFontSize);
            _trayGlyphMetricsCache.Clear();

            _cpuBrush?.Dispose();
            _cpuBrush = new SolidBrush(cpuColorValue.BackColor);

            _gpuBrush?.Dispose();
            _gpuBrush = new SolidBrush(gpuColorValue.BackColor);
        }

        private void RefreshTrayIconRenderSize()
        {
            int iconPixelSize = GetTrayIconPixelSize();
            if (_lastTrayIconPixelSize == iconPixelSize)
                return;

            CacheDisplaySettings();
            ResetTrayCache();
        }

        #endregion

        #region [ Settings Load / Save ]

        private void SetDefaultControlValues()
        {
            lightModeSwitch.Checked = false;
            tempsFahrenheit.Checked = false;
            minimizeOnStart.Checked = false;
            enableCpuTray.Checked = false;
            enableGpuTray.Checked = false;
            singleIconTray.Checked = false;
            colortempsEnable.Checked = false;

            _desiredEnableCpuTray = false;
            _desiredEnableGpuTray = false;
            _desiredSingleIconTray = false;
            _desiredColorTempsEnabled = false;
            _startMinimizedWithAdminRights = true;
            UpdateMinimizeOnStartLabel();
            ShowTemperatureColorCorners = true;
            TemperatureAlertsEnabled = false;

            SelectRefreshInterval(DefaultRefreshIntervalSeconds);
            SelectIconSize(DefaultIconSizePercent);

            cpuColorValue.BackColor = Color.Aqua;
            gpuColorValue.BackColor = Color.Gold;

            if (fontFamilyValue.Items.Count > 0)
                SelectFontFamily(EmbeddedFonts.Bold.Name, 0);

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
                HardwareDialogStateHelper.CaptureOpenHardwareDialogBounds(
                    _openHardwareDialogs,
                    bounds => _savedHardwareDialogBounds = bounds);

                bool isInstalled = IsInstalledAppPresent();
                bool hasStartupTask = isInstalled && IsStartupEntryPresent();

                var settings = new AppSettings
                {
                    // Autostart is retained for backward-compatible readers; it now
                    // correctly represents the scheduled task rather than install state.
                    Autostart = hasStartupTask,
                    LightMode = lightModeSwitch.Checked,
                    TempsFahrenheit = tempsFahrenheit.Checked,
                    SingleIconTray = _desiredSingleIconTray,
                    CpuTrayIcon = _desiredEnableCpuTray,
                    GpuTrayIcon = _desiredEnableGpuTray,
                    TempBasedIconColor = _desiredColorTempsEnabled,
                    TemperatureAlertsEnabled = TemperatureAlertsEnabled,
                    ShowTemperatureColorCorners = ShowTemperatureColorCorners,
                    UpdateInterval = GetSelectedRefreshInterval(),
                    MinWarmTemp = WarmTempMin,
                    MaxWarmTemp = WarmTempMax,
                    NormalTempColor = NormalColor.ToArgb(),
                    WarmTempColor = WarningColor.ToArgb(),
                    HotTempColor = CriticalColor.ToArgb(),
                    FontFamily = fontFamilyValue.SelectedIndex,
                    TrayFontFamily = fontFamilyValue.Text,
                    CpuColor = cpuColorValue.BackColor.ToArgb(),
                    GpuColor = gpuColorValue.BackColor.ToArgb(),
                    IconSize = GetSelectedIconSize(),
                    CpuIndex = cpuIndexSelect.SelectedIndex,
                    GpuIndex = gpuIndexSelect.SelectedIndex,
                    CpuIdentifier = _selectedCpuIdentifier,
                    GpuIdentifier = _selectedGpuIdentifier,
                    CpuTemperatureSensorIdentifier = GetSelectedTemperatureSensorIdentifier(cpuTempSensorSelect),
                    GpuTemperatureSensorIdentifier = GetSelectedTemperatureSensorIdentifier(gpuTempSensorSelect),
                    InstallFolder = InstallPath,
                    StartMinimizedToTray = minimizeOnStart.Checked,
                    StartMinimizedWithAdminRights = _startMinimizedWithAdminRights
                };

                Rectangle? windowBounds = Visible && WindowState == FormWindowState.Normal && IsWindowBoundsVisible(Bounds)
                    ? Bounds
                    : _lastNormalWindowBounds;

                if (windowBounds.HasValue)
                {
                    Rectangle bounds = windowBounds.Value;
                    settings.WindowWidth = bounds.Width;
                    settings.WindowHeight = bounds.Height;
                    settings.WindowX = bounds.X;
                    settings.WindowY = bounds.Y;
                }

                if (_savedHardwareDialogBounds.HasValue)
                {
                    Rectangle bounds = _savedHardwareDialogBounds.Value;
                    settings.HardwareDialogWidth = bounds.Width;
                    settings.HardwareDialogHeight = bounds.Height;
                    settings.HardwareDialogX = bounds.X;
                    settings.HardwareDialogY = bounds.Y;
                }

                string directory = Path.GetDirectoryName(SettingsFilePath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                var options = new System.Text.Json.JsonSerializerOptions { WriteIndented = true };
                string json = System.Text.Json.JsonSerializer.Serialize(settings, options);

                string tempPath = SettingsFilePath + ".tmp";

                using (var fs = new FileStream(tempPath, FileMode.Create, FileAccess.Write, FileShare.None))
                using (var sw = new StreamWriter(fs))
                {
                    sw.Write(json);
                    sw.Flush();
                    fs.Flush(true);
                }

                ReplaceSettingsFile(tempPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Eroare critica la salvarea setarilor:\n{ex.Message}", "Save Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadSettings()
        {
            try
            {
                if (!File.Exists(SettingsFilePath) && !File.Exists(SettingsFilePath + ".tmp") && !File.Exists(SettingsFilePath + ".bak"))
                {
                    CenterToScreen();
                    SetDefaultControlValues();
                    _settingsLoaded = true;
                    return;
                }

                string json = ReadSettingsJson();
                var settings = System.Text.Json.JsonSerializer.Deserialize<AppSettings>(json);

                if (settings == null)
                {
                    SetDefaultControlValues();
                    CenterToScreen();
                    return;
                }

                ApplySavedWindowBounds(settings);

                _savedHardwareDialogBounds = GetSavedHardwareDialogBounds(settings);

                ApplyLoadedBasicControlSettings(settings);
                ApplyLoadedVisualAndThresholdSettings(settings);
                ApplyLoadedSelectionAndInstallSettings(settings);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to load settings: {ex.Message}");
                SetDefaultControlValues();
                CenterToScreen();
            }
            finally
            {
                ApplyPostLoadUiSync();
            }
        }

        private string ReadSettingsJson()
        {
            string[] candidates = { SettingsFilePath, SettingsFilePath + ".tmp", SettingsFilePath + ".bak" };

            foreach (string path in candidates)
            {
                if (!File.Exists(path))
                    continue;

                try
                {
                    string json = File.ReadAllText(path);
                    AppSettings settings = System.Text.Json.JsonSerializer.Deserialize<AppSettings>(json);
                    if (settings == null)
                        throw new InvalidDataException("Settings content was empty.");
                    return json;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Ignoring invalid settings candidate '{path}': {ex.Message}");
                }
            }

            throw new InvalidDataException("No valid settings file was found.");
        }

        private void ReplaceSettingsFile(string tempPath)
        {
            string backupPath = SettingsFilePath + ".bak";

            try
            {
                if (File.Exists(SettingsFilePath))
                    File.Replace(tempPath, SettingsFilePath, backupPath);
                else
                    File.Move(tempPath, SettingsFilePath);

            }
            catch (PlatformNotSupportedException)
            {
                File.Copy(tempPath, SettingsFilePath, true);
                File.Delete(tempPath);
            }
            catch (IOException) when (File.Exists(tempPath))
            {
                File.Copy(tempPath, SettingsFilePath, true);
                File.Delete(tempPath);
            }
        }

        private void ApplySavedWindowBounds(AppSettings settings)
        {
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
        }

        private void ApplyLoadedBasicControlSettings(AppSettings settings)
        {
            lightModeSwitch.Checked = settings.LightMode;
            tempsFahrenheit.Checked = settings.TempsFahrenheit;
            minimizeOnStart.Checked = settings.StartMinimizedToTray;
            _startMinimizedWithAdminRights = settings.StartMinimizedWithAdminRights;
            UpdateMinimizeOnStartLabel();
            singleIconTray.Checked = settings.SingleIconTray;
            enableCpuTray.Checked = settings.CpuTrayIcon;
            enableGpuTray.Checked = settings.GpuTrayIcon;
            colortempsEnable.Checked = settings.TempBasedIconColor;
            TemperatureAlertsEnabled = settings.TemperatureAlertsEnabled;
            ShowTemperatureColorCorners = settings.ShowTemperatureColorCorners;

            _desiredEnableCpuTray = enableCpuTray.Checked;
            _desiredEnableGpuTray = enableGpuTray.Checked;
            _desiredSingleIconTray = singleIconTray.Checked;
            _desiredColorTempsEnabled = colortempsEnable.Checked;

            SelectRefreshInterval(settings.UpdateInterval);
            SelectIconSize(settings.IconSize);
        }

        private void ApplyLoadedVisualAndThresholdSettings(AppSettings settings)
        {
            if (fontFamilyValue.Items.Count > 0)
                SelectFontFamily(settings.TrayFontFamily, settings.FontFamily);

            cpuColorValue.BackColor = ValueHelper.LoadColorOrDefault(settings.CpuColor, Color.Aqua);
            gpuColorValue.BackColor = ValueHelper.LoadColorOrDefault(settings.GpuColor, Color.Gold);

            WarmTempMin = ValueHelper.ClampInt(settings.MinWarmTemp, 0, 130);
            WarmTempMax = ValueHelper.ClampInt(settings.MaxWarmTemp, WarmTempMin, 130);

            NormalColor = ValueHelper.LoadColorOrDefault(settings.NormalTempColor, Color.White);
            WarningColor = ValueHelper.LoadColorOrDefault(settings.WarmTempColor, Color.Yellow);
            CriticalColor = ValueHelper.LoadColorOrDefault(settings.HotTempColor, Color.Red);
        }

        private void ApplyLoadedSelectionAndInstallSettings(AppSettings settings)
        {
            _savedCpuIndex = Math.Max(0, settings.CpuIndex);
            _savedGpuIndex = Math.Max(0, settings.GpuIndex);
            _savedCpuIdentifier = settings.CpuIdentifier;
            _savedGpuIdentifier = settings.GpuIdentifier;
            _savedCpuTemperatureSensorIdentifier = settings.CpuTemperatureSensorIdentifier;
            _savedGpuTemperatureSensorIdentifier = settings.GpuTemperatureSensorIdentifier;

            InstallPath = settings.InstallFolder;
        }

        private void ApplyPostLoadUiSync()
        {
            UpdateCombinedTrayCheckboxAvailability();
            UpdateTemperatureColorsCheckboxAvailability();
            SingleIconTray_CheckedChanged(this, EventArgs.Empty);
            ColortempsEnable_CheckedChanged(this, EventArgs.Empty);
            ApplyTheme();
            UpdateAutostartCheckboxStateAndText();
            _settingsLoaded = true;
        }

        private void PopulateFontFamilyOptions()
        {
            fontFamilyValue.Items.Clear();

            var names = FontFamily.Families
                .Select(f => f.Name)
                .Concat(new[] { EmbeddedBunkenBoldDisplayName, "Consolas" })
                .Where(name => !string.IsNullOrWhiteSpace(name))
                .Where(name => !IsHiddenBunkenFontListEntry(name))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(name => name, StringComparer.OrdinalIgnoreCase)
                .ToArray();

            fontFamilyValue.Items.AddRange(names);
        }

        private void SelectFontFamily(string familyName, int fallbackIndex)
        {
            if (IsEmbeddedBunkenFamily(familyName))
                familyName = EmbeddedBunkenBoldDisplayName;

            if (!string.IsNullOrWhiteSpace(familyName))
            {
                for (int i = 0; i < fontFamilyValue.Items.Count; i++)
                {
                    if (string.Equals(fontFamilyValue.Items[i].ToString(), familyName, StringComparison.OrdinalIgnoreCase))
                    {
                        fontFamilyValue.SelectedIndex = i;
                        return;
                    }
                }
            }

            fontFamilyValue.SelectedIndex = ValueHelper.ClampInt(fallbackIndex, 0, fontFamilyValue.Items.Count - 1);
        }

        private Font CreateTrayFont(string familyName, float size)
        {
            try
            {
                if (IsEmbeddedBunkenFamily(familyName))
                    return new Font(EmbeddedFonts.Bold, size, GetEmbeddedBoldStyle(), GraphicsUnit.Pixel);

                return new Font(familyName, size, FontStyle.Bold, GraphicsUnit.Pixel);
            }
            catch
            {
                return new Font(EmbeddedFonts.Bold, size, GetEmbeddedBoldStyle(), GraphicsUnit.Pixel);
            }
        }

        private static FontStyle GetEmbeddedBoldStyle()
        {
            return EmbeddedFonts.Bold.IsStyleAvailable(FontStyle.Bold)
                ? FontStyle.Bold
                : FontStyle.Regular;
        }

        private static bool IsEmbeddedBunkenFamily(string familyName)
        {
            if (string.Equals(familyName, EmbeddedBunkenBoldDisplayName, StringComparison.OrdinalIgnoreCase))
                return true;

            return string.Equals(familyName, EmbeddedFonts.Bold.Name, StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(familyName, EmbeddedFonts.Book.Name, StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsHiddenBunkenFontListEntry(string familyName)
        {
            return string.Equals(familyName, EmbeddedFonts.Bold.Name, StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(familyName, EmbeddedFonts.Book.Name, StringComparison.OrdinalIgnoreCase);
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

        private static Rectangle? GetSavedHardwareDialogBounds(AppSettings settings)
        {
            if (settings.HardwareDialogWidth <= 0 || settings.HardwareDialogHeight <= 0)
                return null;

            int width = Math.Max(HardwareDialogMinimumSize.Width, settings.HardwareDialogWidth);
            int height = Math.Max(HardwareDialogMinimumSize.Height, settings.HardwareDialogHeight);
            var bounds = new Rectangle(settings.HardwareDialogX, settings.HardwareDialogY, width, height);

            if (settings.HardwareDialogX == -1 || !IsWindowBoundsVisible(bounds))
                return null;

            return bounds;
        }

        private void RegisterHardwareDialog(HardwareDetailsDialog dialog)
        {
            HardwareDialogStateHelper.RegisterHardwareDialog(
                dialog,
                _openHardwareDialogs,
                _savedHardwareDialogBounds,
                HardwareDialogMinimumSize,
                IsLightModeEnabled,
                HardwareDialog_BoundsChanged,
                HardwareDialog_FormClosed,
                IsWindowBoundsVisible);
        }

        private void UnregisterHardwareDialog(HardwareDetailsDialog dialog)
        {
            HardwareDialogStateHelper.UnregisterHardwareDialog(
                dialog,
                _openHardwareDialogs,
                HardwareDialog_BoundsChanged,
                HardwareDialog_FormClosed);
        }

        private void HardwareDialog_BoundsChanged(object sender, EventArgs e)
        {
            if (sender is Form dialog)
                HardwareDialogStateHelper.RememberHardwareDialogBounds(
                    dialog,
                    bounds => _savedHardwareDialogBounds = bounds);
        }

        private void HardwareDialog_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (!(sender is HardwareDetailsDialog dialog))
                return;

            HardwareDialogStateHelper.RememberHardwareDialogBounds(
                dialog,
                bounds => _savedHardwareDialogBounds = bounds);
            UnregisterHardwareDialog(dialog);

            if (!_isShutdownInitiated && !_resourcesDisposed)
                SaveSettings();
        }

        private void ApplyTheme()
        {
            ThemePalette theme = GetThemePalette();

            ApplyThemeToMainContainers(theme);
            ApplyThemeToLabels(theme);
            ApplyThemeToCheckboxes(theme);
            ApplyThemeToInputsAndButtons(theme);
            ApplyThemeToMenus(theme);
            ApplyThemeInvalidation();
            ApplyThemeExitButtonHoverOverrides();
        }

        private void ApplyThemeToMainContainers(ThemePalette theme)
        {
            BackColor = theme.WindowBack;
            ForeColor = theme.Text;

            ApplyBackColor(theme.PageBack, panelWrapper, homePage, settingsPage, aboutPage, tempsWrapper);
            ApplyBackColor(theme.SurfaceBack,
                mainComponentsPanel,
                cpuPanel,
                gpuPanel,
                generalSettingsPanel,
                traySettingsPanel,
                appAboutExtra,
                refreshPanel,
                fontFamilyPanel,
                cpuColorPanel,
                gpuColorPanel,
                iconsizePanel,
                colortempsPanel);
            ApplyBackColor(theme.NavBack, mainMenu, AppDataPnl, homePanel, settingsPanel, aboutPanel);
            ApplyBackColor(theme.Accent, divider1, divider2, divider3, sidepanelHome, sidepanelSettings, sidepanelAbout);
        }

        private void ApplyThemeToLabels(ThemePalette theme)
        {
            foreach (Label label in MainFormShadowHelper.FindControls<Label>(this))
            {
                if (label != appAboutExtra)
                    label.BackColor = label.Parent?.BackColor ?? theme.PageBack;

                label.ForeColor = theme.Text;
            }

            appAboutExtra.BackColor = theme.SurfaceBack;

            ApplyForeColor(theme.TitleText,
                appTitle,
                mainComponentsTitle,
                sysmonTitle,
                tempTitle,
                settingsTitle,
                genSettings,
                traySettingsLabel,
                aboutTitle,
                appTitleAbout);

            ApplyForeColor(theme.MutedText,
                componentType,
                componentModel,
                indexLabel,
                compCpuLabel,
                compGpuLabel,
                compRamLabel,
                CompStorageLabel,
                CompMotherboardLabel,
                cpuTempLabel,
                gpuTempLabel,
                cpuTempCurLabel,
                cpuTempMinLabel,
                cpuTempMaxLabel,
                gpuTempCurLabel,
                gpuTempMinLabel,
                gpuTempMaxLabel,
                appAboutExtra,
                appVersion);

            ApplyForeColor(theme.Text,
                cpuModel,
                gpuModel,
                ramDetails,
                storageDetails,
                motherboardDetails,
                cpuName,
                gpuName,
                refreshLabel,
                fontFamilyLabel,
                cpuColorLabel,
                gpuColorLabel,
                iconsizeLabel);

            ApplyForeColor(theme.CurrentTemp, cpuTempCur, gpuTempCur);
            ApplyForeColor(theme.Success, cpuTempMin, gpuTempMin);
            ApplyForeColor(theme.Danger, cpuTempMax, gpuTempMax);
            githubLink.ForeColor = theme.Link;
        }

        private void ApplyThemeToCheckboxes(ThemePalette theme)
        {
            foreach (CheckBox checkBox in MainFormShadowHelper.FindControls<CheckBox>(this))
            {
                checkBox.BackColor = checkBox.Parent?.BackColor ?? theme.SurfaceBack;
                checkBox.ForeColor = theme.Text;
                checkBox.UseVisualStyleBackColor = false;
            }
        }

        private void ApplyThemeToInputsAndButtons(ThemePalette theme)
        {
            ApplyComboBoxTheme(theme, IsLightModeEnabled, refreshValue, iconsizeValue, fontFamilyValue);
            ApplyNavButtonTheme(theme, homeBtn, settingsBtn, aboutBtn);
            ApplyComponentButtonTheme(theme, cpuConfigButton, gpuConfigButton);
            ApplyWindowButtonTheme(theme, minimizeBtn);
            ApplyWindowButtonTheme(theme, exitBtn);
            ApplyAccentButtonTheme(theme, colortempsConfig);
            ApplyColorButtonTheme(theme, cpuColorValue, gpuColorValue);
        }

        private void ApplyThemeToMenus(ThemePalette theme)
        {
            contextMenuStrip.BackColor = theme.SurfaceBack;
            contextMenuStrip.ForeColor = theme.Text;
            contextMenuStrip.Renderer = new ToolStripProfessionalRenderer(new TrayMenuColorTable(theme));
            ConfigureTrayDisplayDropDown(theme);
            ApplyTrayMenuTheme(theme, ShowForm, trayDisplayMenu, openSettingsTray);
            ApplyTrayMenuTheme(theme,
                trayCpuEnabledMenu,
                trayGpuEnabledMenu,
                trayCombinedMenu,
                trayFahrenheitMenu,
                trayTemperatureColorsMenu,
                trayConfigureColorsMenu);
            SettingsTray.BackColor = theme.SurfaceBack;
            SettingsTray.ForeColor = theme.Danger;
            SettingsTray.Font = contextMenuStrip.Font;
        }

        private void ConfigureTrayDisplayDropDown(ThemePalette theme)
        {
            var dropDown = trayDisplayMenu.DropDown as ToolStripDropDownMenu;
            if (dropDown == null)
                return;

            dropDown.BackColor = theme.SurfaceBack;
            dropDown.ForeColor = theme.Text;
            dropDown.Font = contextMenuStrip.Font;
            dropDown.Renderer = contextMenuStrip.Renderer;
            dropDown.ShowImageMargin = false;
            dropDown.ShowCheckMargin = true;
        }

        private void ApplyTrayMenuTheme(ThemePalette theme, params ToolStripMenuItem[] menuItems)
        {
            foreach (ToolStripMenuItem menuItem in menuItems)
            {
                menuItem.BackColor = theme.SurfaceBack;
                menuItem.ForeColor = theme.Text;
                menuItem.Font = contextMenuStrip.Font;
            }
        }

        private void ApplyThemeInvalidation()
        {
            MainFormShadowHelper.InvalidateCardShadowParents(
                MainFormShadowHelper.GetShadowCards(
                    mainComponentsPanel,
                    gpuPanel,
                    cpuPanel,
                    generalSettingsPanel,
                    traySettingsPanel,
                    appAboutExtra));
            MainFormShadowHelper.InvalidateMainMenuShadowHosts(
                MainFormShadowHelper.GetMainMenuShadowHosts(homePage, settingsPage, aboutPage));
            SelectedTabChanged(null, EventArgs.Empty);
        }

        private void ApplyThemeExitButtonHoverOverrides()
        {
            exitBtn.FlatAppearance.MouseDownBackColor = Color.FromArgb(242, 60, 60);
            exitBtn.FlatAppearance.MouseOverBackColor = Color.FromArgb(210, 50, 50);
        }

        private ThemePalette GetThemePalette()
        {
            if (IsLightModeEnabled)
            {
                return new ThemePalette
                {
                    WindowBack = Color.FromArgb(218, 226, 238),
                    PageBack = Color.FromArgb(246, 248, 252),
                    SurfaceBack = Color.White,
                    NavBack = Color.White,
                    NavSelected = Color.FromArgb(226, 239, 255),
                    Text = Color.FromArgb(31, 41, 55),
                    MutedText = Color.FromArgb(91, 103, 122),
                    TitleText = Color.FromArgb(15, 23, 42),
                    Accent = Color.FromArgb(37, 99, 235),
                    InputBack = Color.White,
                    Border = Color.FromArgb(210, 218, 230),
                    ButtonText = Color.White,
                    CurrentTemp = Color.FromArgb(15, 23, 42),
                    Success = Color.FromArgb(21, 128, 61),
                    Danger = Color.FromArgb(220, 38, 38),
                    Link = Color.FromArgb(4, 120, 87),
                    HardwareHoverText = Color.FromArgb(37, 99, 235)
                };
            }

            return new ThemePalette
            {
                WindowBack = _darkWindowBackColor,
                PageBack = _darkPanelBackColor,
                SurfaceBack = Color.FromArgb(40, 40, 40),
                NavBack = Color.FromArgb(30, 30, 30),
                NavSelected = Color.FromArgb(50, 50, 50),
                Text = Color.LightGray,
                MutedText = Color.DarkGray,
                TitleText = Color.WhiteSmoke,
                Accent = Color.FromArgb(0, 120, 212),
                InputBack = Color.FromArgb(40, 40, 40),
                Border = Color.FromArgb(30, 30, 30),
                ButtonText = Color.LightGray,
                CurrentTemp = Color.LightGray,
                Success = Color.LimeGreen,
                Danger = Color.Red,
                Link = Color.SeaGreen,
                HardwareHoverText = Color.White
            };
        }

        private static void ApplyBackColor(Color color, params Control[] controls)
        {
            foreach (Control control in controls)
            {
                if (control != null)
                    control.BackColor = color;
            }
        }

        private static void ApplyForeColor(Color color, params Control[] controls)
        {
            foreach (Control control in controls)
            {
                if (control != null)
                    control.ForeColor = color;
            }
        }

        private static void ApplyComboBoxTheme(ThemePalette theme, bool lightTheme, params ComboBox[] comboBoxes)
        {
            Color backColor = lightTheme
                ? Color.FromArgb(230, 235, 240)
                : theme.InputBack;

            foreach (ComboBox comboBox in comboBoxes)
            {
                if (comboBox == null)
                    continue;

                comboBox.BackColor = backColor;
                comboBox.ForeColor = theme.Text;
            }
        }

        private static void ApplyNavButtonTheme(ThemePalette theme, params Button[] buttons)
        {
            foreach (Button button in buttons)
            {
                if (button == null)
                    continue;

                button.BackColor = theme.NavBack;
                button.ForeColor = theme.Text;
                button.FlatAppearance.BorderColor = theme.NavBack;
                button.FlatAppearance.MouseDownBackColor = theme.NavSelected;
                button.FlatAppearance.MouseOverBackColor = theme.NavSelected;
            }
        }

        private static void ApplyComponentButtonTheme(ThemePalette theme, params Button[] buttons)
        {
            foreach (Button button in buttons)
            {
                if (button == null)
                    continue;

                button.BackColor = theme.SurfaceBack;
                button.ForeColor = theme.MutedText;
                button.FlatAppearance.BorderColor = theme.SurfaceBack;
                button.FlatAppearance.MouseDownBackColor = theme.NavSelected;
                button.FlatAppearance.MouseOverBackColor = theme.NavSelected;
            }
        }

        private static void ApplyWindowButtonTheme(ThemePalette theme, Button button)
        {
            if (button == null)
                return;

            button.BackColor = theme.WindowBack;
            button.ForeColor = theme.TitleText;
            button.FlatAppearance.MouseDownBackColor = theme.NavSelected;
            button.FlatAppearance.MouseOverBackColor = theme.NavSelected;
        }

        private static void ApplyAccentButtonTheme(ThemePalette theme, Button button)
        {
            if (button == null)
                return;

            button.BackColor = theme.Accent;
            button.ForeColor = theme.ButtonText;
            button.FlatAppearance.BorderColor = theme.Border;
        }

        private static void ApplyColorButtonTheme(ThemePalette theme, params Button[] buttons)
        {
            foreach (Button button in buttons)
            {
                if (button == null)
                    continue;

                button.ForeColor = Color.Black;
                button.FlatAppearance.BorderColor = theme.Border;
            }
        }

        private void CardShadowParent_Changed(object sender, EventArgs e)
        {
            MainFormShadowHelper.InvalidateCardShadowParents(
                MainFormShadowHelper.GetShadowCards(
                    mainComponentsPanel,
                    gpuPanel,
                    cpuPanel,
                    generalSettingsPanel,
                    traySettingsPanel,
                    appAboutExtra));
        }

        private void MainMenuShadow_Changed(object sender, EventArgs e)
        {
            MainFormShadowHelper.InvalidateMainMenuShadowHosts(
                MainFormShadowHelper.GetMainMenuShadowHosts(homePage, settingsPage, aboutPage));
        }

        private void ShadowCardParent_Paint(object sender, PaintEventArgs e)
        {
            MainFormShadowHelper.CardShadowParent_Paint(
                sender,
                e,
                MainFormShadowHelper.GetShadowCards(
                    mainComponentsPanel,
                    gpuPanel,
                    cpuPanel,
                    generalSettingsPanel,
                    traySettingsPanel,
                    appAboutExtra),
                IsLightModeEnabled);
        }

        private void ShadowMainMenuHost_Paint(object sender, PaintEventArgs e)
        {
            MainFormShadowHelper.MainMenuShadowHost_Paint(
                sender,
                e,
                mainMenu,
                IsLightModeEnabled,
                MainMenuShadowWidth);
        }

        private sealed class ThemePalette
        {
            public Color WindowBack;
            public Color PageBack;
            public Color SurfaceBack;
            public Color NavBack;
            public Color NavSelected;
            public Color Text;
            public Color MutedText;
            public Color TitleText;
            public Color Accent;
            public Color InputBack;
            public Color Border;
            public Color ButtonText;
            public Color CurrentTemp;
            public Color Success;
            public Color Danger;
            public Color Link;
            public Color HardwareHoverText;
        }

        private sealed class TrayMenuColorTable : ProfessionalColorTable
        {
            private readonly ThemePalette _theme;

            public TrayMenuColorTable(ThemePalette theme)
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

        #endregion

        #region [ Window / Navigation / UI Events ]

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
            ThemePalette theme = GetThemePalette();
            Color sidepanelline = theme.Accent;
            Color selectedColor = theme.NavSelected;
            Color defaultColor = theme.NavBack;

            int index = mainTabControl.SelectedIndex;

            ApplyTabButtonSelectionColors(index, selectedColor, defaultColor);
            ApplyTabButtonTextColors(theme.Text);
            ApplyTabSideIndicators(index, sidepanelline);
        }

        private void ApplyTabButtonSelectionColors(int index, Color selectedColor, Color defaultColor)
        {
            homeBtn.BackColor = (index == 0) ? selectedColor : defaultColor;
            settingsBtn.BackColor = (index == 1) ? selectedColor : defaultColor;
            aboutBtn.BackColor = (index == 2) ? selectedColor : defaultColor;
        }

        private void ApplyTabButtonTextColors(Color textColor)
        {
            homeBtn.ForeColor = textColor;
            settingsBtn.ForeColor = textColor;
            aboutBtn.ForeColor = textColor;
        }

        private void ApplyTabSideIndicators(int index, Color sidepanelline)
        {
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
            ConfirmExit();
        }

        private void MinimizeBtn_Click(object sender, EventArgs e)
        {
            HideToTray();
        }

        private void DonatePic_Click(object sender, EventArgs e)
        {
            OpenUrl("https://revolut.me/nmd113");
        }

        private void GithubLink_Click(object sender, EventArgs e)
        {
            OpenUrl("https://github.com/nmd-113/Tray-Temps");
        }

        private void ShowForm_Click(object sender, EventArgs e)
        {
            ShowWindow();
        }

        private void OpenSettingsTray_Click(object sender, EventArgs e)
        {
            ShowWindow();
            SetTab(1);
        }

        private async void CheckUpdates_Click(object sender, EventArgs e)
        {
            if (!TryParseUpdateVersion(Application.ProductVersion, out Version currentVersion))
            {
                MessageBox.Show(this, "The installed app version could not be read.", "Check for Updates", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string originalText = checkUpdates.Text;
            checkUpdates.Enabled = false;
            checkUpdates.Text = "Checking...";

            try
            {
                string tagsJson = await UpdateCheckClient.GetStringAsync(GitHubTagsApiUrl);
                if (IsDisposed || Disposing)
                    return;

                if (!TryGetLatestGitHubTag(tagsJson, out Version latestVersion, out string latestTag))
                    throw new InvalidDataException("No version tags were found in the repository.");

                if (latestVersion <= currentVersion)
                {
                    MessageBox.Show(
                        this,
                        $"TrayTemps is up to date.\n\nInstalled version: v{currentVersion}",
                        "Check for Updates",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    return;
                }

                DialogResult result = MessageBox.Show(
                    this,
                    $"A newer version is available.\n\nInstalled: v{currentVersion}\nLatest: {latestTag}\n\nOpen the update page?",
                    "Update Available",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Information);

                if (result == DialogResult.Yes)
                    OpenUrl(GitHubReleasePageUrl + Uri.EscapeDataString(latestTag));
            }
            catch (Exception ex) when (ex is HttpRequestException || ex is TaskCanceledException || ex is InvalidDataException || ex is System.Text.Json.JsonException)
            {
                MessageBox.Show(
                    this,
                    "Unable to check for updates. Check your internet connection and try again.\n\n" + ex.Message,
                    "Check for Updates",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
            finally
            {
                if (!IsDisposed)
                {
                    checkUpdates.Text = originalText;
                    checkUpdates.Enabled = true;
                }
            }
        }

        private static HttpClient CreateUpdateCheckClient()
        {
            var client = new HttpClient { Timeout = TimeSpan.FromSeconds(10) };
            client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", AppName + " update checker");
            return client;
        }

        private static bool TryGetLatestGitHubTag(string tagsJson, out Version latestVersion, out string latestTag)
        {
            latestVersion = null;
            latestTag = null;

            using (var document = System.Text.Json.JsonDocument.Parse(tagsJson))
            {
                if (document.RootElement.ValueKind != System.Text.Json.JsonValueKind.Array)
                    return false;

                foreach (System.Text.Json.JsonElement tagElement in document.RootElement.EnumerateArray())
                {
                    if (!tagElement.TryGetProperty("name", out System.Text.Json.JsonElement nameElement) ||
                        !TryParseUpdateVersion(nameElement.GetString(), out Version tagVersion))
                    {
                        continue;
                    }

                    if (latestVersion == null || tagVersion > latestVersion)
                    {
                        latestVersion = tagVersion;
                        latestTag = nameElement.GetString();
                    }
                }
            }

            return latestVersion != null;
        }

        private static bool TryParseUpdateVersion(string value, out Version version)
        {
            version = null;
            if (string.IsNullOrWhiteSpace(value))
                return false;

            string versionText = value.Trim();
            if (versionText.StartsWith("v", StringComparison.OrdinalIgnoreCase))
                versionText = versionText.Substring(1);

            return Version.TryParse(versionText, out version);
        }

        private void ContextMenuStrip_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SyncTrayMenuState();
        }

        private void SyncTrayMenuState()
        {
            trayCpuEnabledMenu.Checked = enableCpuTray.Checked;
            trayCpuEnabledMenu.Enabled = enableCpuTray.Enabled;
            trayGpuEnabledMenu.Checked = enableGpuTray.Checked;
            trayGpuEnabledMenu.Enabled = enableGpuTray.Enabled;

            bool canCombine = enableCpuTray.Checked && enableGpuTray.Checked && singleIconTray.Enabled;
            trayCombinedMenu.Checked = singleIconTray.Checked;
            trayCombinedMenu.Enabled = canCombine;

            trayFahrenheitMenu.Checked = tempsFahrenheit.Checked;
            trayFahrenheitMenu.Enabled = tempsFahrenheit.Enabled;
            trayTemperatureColorsMenu.Checked = colortempsEnable.Checked;
            trayTemperatureColorsMenu.Enabled = colortempsEnable.Enabled;
            trayConfigureColorsMenu.Enabled = colortempsEnable.Checked && colortempsConfig.Enabled;
        }

        private void TrayCpuEnabledMenu_Click(object sender, EventArgs e)
        {
            if (enableCpuTray.Enabled)
                enableCpuTray.Checked = !enableCpuTray.Checked;
        }

        private void TrayGpuEnabledMenu_Click(object sender, EventArgs e)
        {
            if (enableGpuTray.Enabled)
                enableGpuTray.Checked = !enableGpuTray.Checked;
        }

        private void TrayCombinedMenu_Click(object sender, EventArgs e)
        {
            if (singleIconTray.Enabled)
                singleIconTray.Checked = !singleIconTray.Checked;
        }

        private void TrayFahrenheitMenu_Click(object sender, EventArgs e)
        {
            tempsFahrenheit.Checked = !tempsFahrenheit.Checked;
        }

        private void TrayTemperatureColorsMenu_Click(object sender, EventArgs e)
        {
            if (colortempsEnable.Enabled)
                colortempsEnable.Checked = !colortempsEnable.Checked;
        }

        private void TrayConfigureColorsMenu_Click(object sender, EventArgs e)
        {
            if (!colortempsEnable.Checked || !colortempsConfig.Enabled)
                return;

            ColortempsConfig_Click(colortempsConfig, EventArgs.Empty);
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

        private void OpenUrl(string url)
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            });
        }

        private void HideToTray()
        {
            RememberNormalWindowBounds();
            _restoreLastWindowBoundsWhenShown = _lastNormalWindowBounds.HasValue;
            Hide();
            UpdateTrayIcons();
        }

        private void HideToTrayAfterStartup()
        {
            if (_isShutdownInitiated || _resourcesDisposed || IsDisposed ||
                _explicitShowPending || !_initialWindowDisplaySuppressed)
                return;

            HideToTray();
        }

        private void ConfirmExit()
        {
            DialogResult result = MessageBox.Show(
                this,
                "Are you sure you want to exit TrayTemps?",
                "Exit Confirmation",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
                Close();
        }

        #endregion

        #region [ Theme / UI State ]

        private void ClearSettings_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
                "Reset in-app settings to their default values?",
                "Reset Settings",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result != DialogResult.Yes)
                return;

            try
            {
                bool settingsWereLoaded = _settingsLoaded;
                _settingsLoaded = false;
                _isInternalCheckChange = true;

                try
                {
                    SetDefaultControlValues();
                }
                finally
                {
                    _isInternalCheckChange = false;
                    _settingsLoaded = settingsWereLoaded;
                }

                ApplyPostLoadUiSync();
                SaveSettings();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error resetting settings: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Setting_CheckedChanged(object sender, EventArgs e)
        {
            if (_isInternalCheckChange)
                return;

            if (sender is CheckBox chk)
            {
                if (chk.Name == nameof(enableCpuTray))
                    _desiredEnableCpuTray = enableCpuTray.Checked;
                else if (chk.Name == nameof(enableGpuTray))
                    _desiredEnableGpuTray = enableGpuTray.Checked;

                if (HandleTemperatureUnitCheckboxChanged(chk))
                    return;

                UpdateTrayIcons();
                HandleTrayVisibilityCheckboxChanged(chk);
            }
        }

        private void MinimizeOnStart_CheckedChanged(object sender, EventArgs e)
        {
            if (!_settingsLoaded || _isInternalCheckChange)
                return;

            if (!minimizeOnStart.Checked)
            {
                UpdateMinimizeOnStartLabel();
                SaveSettings();
                return;
            }

            DialogResult result = MessageBox.Show(
                this,
                "Choose how TrayTemps should start when hidden to the tray.\n\n" +
                "Yes: Start with administrator rights for complete low-level hardware access.\n\n" +
                "No: Start without administrator rights. Some hardware information may be unavailable or incomplete.\n\n" +
                "This choice applies the next time TrayTemps starts.",
                "Start Minimized to Tray",
                MessageBoxButtons.YesNoCancel,
                MessageBoxIcon.Question);

            if (result == DialogResult.Cancel)
            {
                _isInternalCheckChange = true;
                minimizeOnStart.Checked = false;
                _isInternalCheckChange = false;
                UpdateMinimizeOnStartLabel();
                return;
            }

            _startMinimizedWithAdminRights = result == DialogResult.Yes;
            UpdateMinimizeOnStartLabel();
            SaveSettings();
        }

        private void UpdateMinimizeOnStartLabel()
        {
            if (!minimizeOnStart.Checked)
            {
                minimizeOnStart.Text = "Start minimized to tray";
                return;
            }

            minimizeOnStart.Text = _startMinimizedWithAdminRights
                ? "Start minimized to tray (Elevated)"
                : "Start minimized to tray (Normal)";
        }

        private bool HandleTemperatureUnitCheckboxChanged(CheckBox chk)
        {
            if (chk.Name != nameof(tempsFahrenheit))
                return false;

            RefreshTemperatureDisplayFromCurrentValues();
            return true;
        }

        private void HandleTrayVisibilityCheckboxChanged(CheckBox chk)
        {
            if (chk.Name == nameof(enableCpuTray) || chk.Name == nameof(enableGpuTray))
            {
                if (_settingsLoaded)
                {
                    UpdateCombinedTrayCheckboxAvailability();
                    UpdateTemperatureColorsCheckboxAvailability();
                }

                ResetTrayCache();

                if (_settingsLoaded)
                    TempTimer_Tick(this, EventArgs.Empty);
            }
        }

        private void UpdateCombinedTrayCheckboxAvailability()
        {
            bool canUseCombinedTray = enableCpuTray.Checked && enableGpuTray.Checked;

            if (!canUseCombinedTray && singleIconTray.Checked)
            {
                _isInternalCheckChange = true;
                singleIconTray.Checked = false;
                _isInternalCheckChange = false;
            }
            else if (canUseCombinedTray && !singleIconTray.Checked && _desiredSingleIconTray)
            {
                _isInternalCheckChange = true;
                singleIconTray.Checked = true;
                _isInternalCheckChange = false;
            }

            singleIconTray.Enabled = canUseCombinedTray;
        }

        private void UpdateTemperatureColorsCheckboxAvailability()
        {
            bool canUseTemperatureColors = enableCpuTray.Checked || enableGpuTray.Checked;

            if (!canUseTemperatureColors && colortempsEnable.Checked)
            {
                _isInternalCheckChange = true;
                colortempsEnable.Checked = false;
                _isInternalCheckChange = false;
            }
            else if (canUseTemperatureColors && !colortempsEnable.Checked && _desiredColorTempsEnabled)
            {
                _isInternalCheckChange = true;
                colortempsEnable.Checked = true;
                _isInternalCheckChange = false;
            }

            colortempsEnable.Enabled = canUseTemperatureColors;
        }

        private void UpdateTemperatureTrayCheckboxAvailability()
        {
            UpdateCpuTrayCheckboxAvailability();
            UpdateGpuTrayCheckboxAvailability();
            UpdateCombinedTrayCheckboxAvailability();
            UpdateTemperatureColorsCheckboxAvailability();
            UpdateTrayIcons();
        }

        private void UpdateCpuTrayCheckboxAvailability()
        {
            bool hasUsableCpuTemperature = IsUsableTemperatureSensor(_cpuTempSensor);

            if (hasUsableCpuTemperature)
            {
                if (!enableCpuTray.Checked && _desiredEnableCpuTray)
                {
                    _isInternalCheckChange = true;
                    enableCpuTray.Checked = true;
                    _isInternalCheckChange = false;
                }

                enableCpuTray.Enabled = true;
                return;
            }

            if (enableCpuTray.Checked)
            {
                _isInternalCheckChange = true;
                enableCpuTray.Checked = false;
                _isInternalCheckChange = false;
            }

            enableCpuTray.Enabled = false;
        }

        private void UpdateGpuTrayCheckboxAvailability()
        {
            bool hasUsableGpuTemperature = IsUsableTemperatureSensor(_gpuTempSensor);

            if (hasUsableGpuTemperature)
            {
                if (!enableGpuTray.Checked && _desiredEnableGpuTray)
                {
                    _isInternalCheckChange = true;
                    enableGpuTray.Checked = true;
                    _isInternalCheckChange = false;
                }

                enableGpuTray.Enabled = true;
                return;
            }

            if (enableGpuTray.Checked)
            {
                _isInternalCheckChange = true;
                enableGpuTray.Checked = false;
                _isInternalCheckChange = false;
            }

            enableGpuTray.Enabled = false;
        }

        private void Setting_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (sender is ComboBox)
            {
                RefreshDisplaySettingPreview(resetCpuTrayCacheText: true, resetGpuTrayCacheText: true);
            }
        }

        private void LightModeSwitch_CheckedChanged(object sender, EventArgs e)
        {
            ApplyTheme();
            HardwareDialogStateHelper.ApplyThemeToOpenHardwareDialogs(_openHardwareDialogs, IsLightModeEnabled);
            ThemeChanged?.Invoke(this, EventArgs.Empty);
        }

        private void ColortempsEnable_CheckedChanged(object sender, EventArgs e)
        {
            if (_isInternalCheckChange)
                return;

            _desiredColorTempsEnabled = colortempsEnable.Checked;

            bool useTempColors = colortempsEnable.Checked;

            UpdateTrayColorLabels();

            colortempsConfig.Enabled = useTempColors;

            if (_settingsLoaded)
            {
                RefreshDisplaySettingPreview(resetCpuTrayCacheText: true, resetGpuTrayCacheText: true);
            }
        }

        private void UpdateTrayColorLabels()
        {
            if (cpuColorLabel == null || gpuColorLabel == null)
                return;

            bool useIdentityLines = ShowDeviceIdentityMarkers;
            cpuColorLabel.Text = useIdentityLines ? "CPU Line:" : "CPU Color:";
            gpuColorLabel.Text = useIdentityLines ? "GPU Line:" : "GPU Color:";
        }

        private void SingleIconTray_CheckedChanged(object sender, EventArgs e)
        {
            if (_isInternalCheckChange)
                return;

            _desiredSingleIconTray = singleIconTray.Checked;

            ApplySingleIconTrayControlState();
            ApplyGpuTrayVisibilityForMultiIconMode();

            ResetTrayCache();
            UpdateTrayIcons();

            if (_settingsLoaded)
                TempTimer_Tick(this, EventArgs.Empty);
        }

        private void ApplySingleIconTrayControlState()
        {
            if (singleIconTray.Checked)
            {
                iconsizeValue.Enabled = false;
            }
            else
            {
                iconsizeValue.Enabled = true;
            }

            fontFamilyValue.Enabled = true;
        }

        private void ApplyGpuTrayVisibilityForMultiIconMode()
        {
            if (!singleIconTray.Checked && enableGpuTray.Checked)
            {
                gpuTrayIcon.Visible = true;
            }
        }

        private void ColortempsConfig_Click(object sender, EventArgs e)
        {
            ColorTempsConfig cfg = new ColorTempsConfig(this);
            cfg.Show();
        }

        private void RefreshValue_ValueChanged(object sender, EventArgs e)
        {
            if (refreshValue.SelectedIndex >= 0)
                UpdateTimerInterval();
        }

        private void IconsizeValue_ValueChanged(object sender, EventArgs e)
        {
            if (iconsizeValue.SelectedIndex >= 0)
                RefreshDisplaySettingPreview(resetCpuTrayCacheText: true, resetGpuTrayCacheText: true);
        }

        private void SelectRefreshInterval(decimal value)
        {
            decimal clamped = ValueHelper.ClampDecimal(value, MinimumRefreshIntervalSeconds, MaximumRefreshIntervalSeconds);
            decimal snapped = Math.Round(clamped * 4M, MidpointRounding.AwayFromZero) / 4M;
            SelectComboBoxText(refreshValue, FormatRefreshInterval(snapped));
        }

        private void SelectIconSize(int value)
        {
            int clamped = Math.Max(MinimumIconSizePercent, Math.Min(MaximumIconSizePercent, value));
            int snapped = Math.Max(MinimumIconSizePercent, Math.Min(MaximumIconSizePercent, ((int)Math.Round(clamped / 5d, MidpointRounding.AwayFromZero)) * 5));
            SelectComboBoxText(iconsizeValue, snapped.ToString(CultureInfo.InvariantCulture));
        }

        private decimal GetSelectedRefreshInterval()
        {
            if (TryGetComboBoxDecimalValue(refreshValue, out decimal value))
                return value;

            return DefaultRefreshIntervalSeconds;
        }

        private int GetSelectedIconSize()
        {
            if (TryGetComboBoxIntValue(iconsizeValue, out int value))
                return value;

            return DefaultIconSizePercent;
        }

        private static bool TryGetComboBoxDecimalValue(ComboBox comboBox, out decimal value)
        {
            string text = comboBox?.Text?.Trim();

            if (decimal.TryParse(text, NumberStyles.Number, CultureInfo.InvariantCulture, out value))
                return true;

            return decimal.TryParse(text, NumberStyles.Number, CultureInfo.CurrentCulture, out value);
        }

        private static bool TryGetComboBoxIntValue(ComboBox comboBox, out int value)
        {
            string text = comboBox?.Text?.Trim();
            return int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out value) ||
                   int.TryParse(text, NumberStyles.Integer, CultureInfo.CurrentCulture, out value);
        }

        private static void SelectComboBoxText(ComboBox comboBox, string text)
        {
            if (comboBox == null)
                return;

            int index = comboBox.FindStringExact(text);
            if (index >= 0)
            {
                comboBox.SelectedIndex = index;
            }
            else if (comboBox.Items.Count > 0)
            {
                comboBox.SelectedIndex = 0;
            }
        }

        private static string FormatRefreshInterval(decimal value)
        {
            return value.ToString("0.##", CultureInfo.InvariantCulture);
        }

        private void RefreshDisplaySettingPreview(bool resetCpuTrayCacheText, bool resetGpuTrayCacheText)
        {
            CacheDisplaySettings();

            if (resetCpuTrayCacheText)
                _lastCpuTempText = null;

            if (resetGpuTrayCacheText)
                _lastGpuTempText = null;

            TempTimer_Tick(this, EventArgs.Empty);
        }

        public void RefreshTemperatureDisplayFromCurrentValues()
        {
            float? cpuTemp = GetCurrentKnownTemperature(_cpuTempSensor);
            float? gpuTemp = GetCurrentKnownTemperature(_gpuTempSensor);

            UpdateTemperatures(cpuTemp, gpuTemp);
            RefreshTrayIconsFromCurrentValues(cpuTemp, gpuTemp);
        }

        private static float? GetCurrentKnownTemperature(ISensor sensor)
        {
            return TemperatureFormatHelper.IsValidTemp(sensor?.Value)
                ? sensor.Value
                : (float?)null;
        }

        private void RefreshTrayIconsFromCurrentValues(float? cpuTemp, float? gpuTemp)
        {
            ResetTrayCache();
            UpdateAllTrayIcons(cpuTemp, gpuTemp);
        }

        private void CpuIndexSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_isInitializingHardwareSelectors)
                return;

            if (!ApplySelectedCpuHardwareFromCurrentIndex(updateHardware: true))
                return;

            ResetCpuTemperatureDisplayState();

            TempTimer_Tick(this, EventArgs.Empty);
        }

        private void CpuTempSensorSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_isInitializingHardwareSelectors || _isInitializingTemperatureSensorSelectors || _selectedCpuHardware == null)
                return;

            _cpuTempSensor = GetConfiguredTemperatureSensor(cpuTempSensorSelect, _selectedCpuHardware, isCpu: true);
            _savedCpuTemperatureSensorIdentifier = GetSelectedTemperatureSensorIdentifier(cpuTempSensorSelect);
            ResetCpuTemperatureDisplayState();
            TempTimer_Tick(this, EventArgs.Empty);
        }

        private void GpuIndexSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_isInitializingHardwareSelectors)
                return;

            if (!ApplySelectedGpuHardwareFromCurrentIndex(updateHardware: true))
                return;

            ResetGpuTemperatureDisplayState();

            TempTimer_Tick(this, EventArgs.Empty);
        }

        private void GpuTempSensorSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_isInitializingHardwareSelectors || _isInitializingTemperatureSensorSelectors || _selectedGpuHardware == null)
                return;

            _gpuTempSensor = GetConfiguredTemperatureSensor(gpuTempSensorSelect, _selectedGpuHardware, isCpu: false);
            _savedGpuTemperatureSensorIdentifier = GetSelectedTemperatureSensorIdentifier(gpuTempSensorSelect);
            ResetGpuTemperatureDisplayState();
            TempTimer_Tick(this, EventArgs.Empty);
        }

        private void CpuConfigButton_Click(object sender, EventArgs e)
        {
            using (var config = new HardwareSelectionConfig(this, isCpu: true))
                config.ShowDialog(this);
        }

        private void GpuConfigButton_Click(object sender, EventArgs e)
        {
            using (var config = new HardwareSelectionConfig(this, isCpu: false))
                config.ShowDialog(this);
        }

        internal List<string> GetHardwareSelectionNames(bool isCpu)
        {
            List<IHardware> hardwares = isCpu ? _cpuHardwares : _gpuHardwares;

            if (hardwares == null)
                return new List<string>();

            return hardwares
                .Select((hardware, index) => string.IsNullOrWhiteSpace(hardware?.Name)
                    ? $"{(isCpu ? "CPU" : "GPU")} {index}"
                    : $"{index}: {hardware.Name}")
                .ToList();
        }

        internal int GetSelectedHardwareIndex(bool isCpu)
        {
            return isCpu ? cpuIndexSelect.SelectedIndex : gpuIndexSelect.SelectedIndex;
        }

        internal List<TemperatureSensorOption> GetTemperatureSensorOptions(bool isCpu, int hardwareIndex)
        {
            List<IHardware> hardwares = isCpu ? _cpuHardwares : _gpuHardwares;
            var options = new List<TemperatureSensorOption> { new TemperatureSensorOption(null, "Auto") };

            if (hardwares == null || hardwareIndex < 0 || hardwareIndex >= hardwares.Count)
                return options;

            options.AddRange(GetTemperatureSensorsForSelection(hardwares[hardwareIndex])
                .Select(sensor => new TemperatureSensorOption(sensor, sensor.Name)));
            return options;
        }

        internal string GetSelectedTemperatureSensorIdentifier(bool isCpu)
        {
            return GetSelectedTemperatureSensorIdentifier(isCpu ? cpuTempSensorSelect : gpuTempSensorSelect);
        }

        internal void ApplyHardwareSelection(bool isCpu, int hardwareIndex, string sensorIdentifier)
        {
            List<IHardware> hardwares = isCpu ? _cpuHardwares : _gpuHardwares;

            if (hardwares == null || hardwareIndex < 0 || hardwareIndex >= hardwares.Count)
                return;

            _isInitializingHardwareSelectors = true;

            try
            {
                if (isCpu)
                {
                    _savedCpuTemperatureSensorIdentifier = sensorIdentifier ?? string.Empty;
                    cpuIndexSelect.SelectedIndex = hardwareIndex;

                    if (!ApplySelectedCpuHardwareFromCurrentIndex(updateHardware: true))
                        return;

                    ResetCpuTemperatureDisplayState();
                }
                else
                {
                    _savedGpuTemperatureSensorIdentifier = sensorIdentifier ?? string.Empty;
                    gpuIndexSelect.SelectedIndex = hardwareIndex;

                    if (!ApplySelectedGpuHardwareFromCurrentIndex(updateHardware: true))
                        return;

                    ResetGpuTemperatureDisplayState();
                }
            }
            finally
            {
                _isInitializingHardwareSelectors = false;
            }

            if (_settingsLoaded)
                SaveSettings();

            TempTimer_Tick(this, EventArgs.Empty);
        }

        private void ResetCpuTemperatureDisplayState()
        {
            _cpuInvalidSensorCycles = 0;
            _cpuMinTemp = float.MaxValue;
            _cpuMaxTemp = float.MinValue;
            _lastCpuTempText = null;
            _cpuHotAlertRaised = false;
            _nextTemperatureAlertUtc = DateTime.MinValue;
            UpdateTemperatureTrayCheckboxAvailability();
        }

        private void ResetGpuTemperatureDisplayState()
        {
            _gpuInvalidSensorCycles = 0;
            _gpuMinTemp = float.MaxValue;
            _gpuMaxTemp = float.MinValue;
            _lastGpuTempText = null;
            _gpuHotAlertRaised = false;
            _nextTemperatureAlertUtc = DateTime.MinValue;
            UpdateTemperatureTrayCheckboxAvailability();
        }

        private bool ApplySelectedCpuHardwareFromCurrentIndex(bool updateHardware)
        {
            if (cpuIndexSelect.SelectedIndex < 0 ||
                _cpuHardwares == null ||
                cpuIndexSelect.SelectedIndex >= _cpuHardwares.Count)
                return false;

            _selectedCpuHardware = _cpuHardwares[cpuIndexSelect.SelectedIndex];
            SetupSelectedCpuHardware(updateHardware);

            _selectedCpuIdentifier = _selectedCpuHardware.Identifier.ToString();

            return true;
        }

        private bool ApplySelectedGpuHardwareFromCurrentIndex(bool updateHardware)
        {
            if (gpuIndexSelect.SelectedIndex < 0 ||
                _gpuHardwares == null ||
                gpuIndexSelect.SelectedIndex >= _gpuHardwares.Count)
                return false;

            _selectedGpuHardware = _gpuHardwares[gpuIndexSelect.SelectedIndex];
            SetupSelectedGpuHardware(updateHardware);

            _selectedGpuIdentifier = _selectedGpuHardware.Identifier.ToString();

            return true;
        }

        private void SetupSelectedCpuHardware(bool updateHardware)
        {
            cpuModel.Text = _selectedCpuHardware.Name;
            cpuName.Text = _selectedCpuHardware.Name;

            if (updateHardware)
                UpdateHardwareRecursive(_selectedCpuHardware);

            PopulateTemperatureSensorSelector(cpuTempSensorSelect, _selectedCpuHardware, _savedCpuTemperatureSensorIdentifier);
            _cpuTempSensor = GetConfiguredTemperatureSensor(cpuTempSensorSelect, _selectedCpuHardware, isCpu: true);
            _cpuInvalidSensorCycles = 0;
        }

        private void SetupSelectedGpuHardware(bool updateHardware)
        {
            gpuModel.Text = _selectedGpuHardware.Name;
            gpuName.Text = _selectedGpuHardware.Name;

            if (updateHardware)
                UpdateHardwareRecursive(_selectedGpuHardware);

            PopulateTemperatureSensorSelector(gpuTempSensorSelect, _selectedGpuHardware, _savedGpuTemperatureSensorIdentifier);
            _gpuTempSensor = GetConfiguredTemperatureSensor(gpuTempSensorSelect, _selectedGpuHardware, isCpu: false);
            _gpuInvalidSensorCycles = 0;
        }

        private void PopulateTemperatureSensorSelector(ComboBox selector, IHardware hardware, string savedSensorIdentifier)
        {
            _isInitializingTemperatureSensorSelectors = true;
            selector.BeginUpdate();

            try
            {
                selector.Items.Clear();
                selector.Items.Add(new TemperatureSensorOption(null, "Auto"));

                foreach (ISensor sensor in GetTemperatureSensorsForSelection(hardware))
                    selector.Items.Add(new TemperatureSensorOption(sensor, sensor.Name));

                int selectedIndex = FindTemperatureSensorIndex(selector, savedSensorIdentifier);
                selector.SelectedIndex = selectedIndex >= 0 ? selectedIndex : 0;
                selector.Enabled = selector.Items.Count > 1;
            }
            finally
            {
                selector.EndUpdate();
                _isInitializingTemperatureSensorSelectors = false;
            }
        }

        private static List<ISensor> GetTemperatureSensorsForSelection(IHardware hardware)
        {
            if (hardware == null)
                return new List<ISensor>();

            return hardware.Sensors
                .Concat(EnumerateTemperatureSensorsRecursive(hardware.SubHardware))
                .Where(sensor => sensor != null && sensor.SensorType == SensorType.Temperature)
                .GroupBy(sensor => sensor.Identifier.ToString(), StringComparer.OrdinalIgnoreCase)
                .Select(group => group.First())
                .OrderBy(GetTemperatureSensorPriority)
                .ThenBy(sensor => sensor.Name, StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        private static int GetTemperatureSensorPriority(ISensor sensor)
        {
            string name = sensor?.Name ?? string.Empty;

            if (name.IndexOf("Package", StringComparison.OrdinalIgnoreCase) >= 0 ||
                name.IndexOf("Tctl", StringComparison.OrdinalIgnoreCase) >= 0 ||
                name.IndexOf("Tdie", StringComparison.OrdinalIgnoreCase) >= 0)
                return 0;

            if (name.IndexOf("Hot Spot", StringComparison.OrdinalIgnoreCase) >= 0 ||
                name.IndexOf("Hotspot", StringComparison.OrdinalIgnoreCase) >= 0)
                return 1;

            if (name.IndexOf("Core", StringComparison.OrdinalIgnoreCase) >= 0)
                return 2;

            return 3;
        }

        private static int FindTemperatureSensorIndex(ComboBox selector, string sensorIdentifier)
        {
            if (string.IsNullOrWhiteSpace(sensorIdentifier))
                return 0;

            for (int i = 1; i < selector.Items.Count; i++)
            {
                if (selector.Items[i] is TemperatureSensorOption option &&
                    string.Equals(option.Identifier, sensorIdentifier, StringComparison.OrdinalIgnoreCase))
                    return i;
            }

            return 0;
        }

        private static string GetSelectedTemperatureSensorIdentifier(ComboBox selector)
        {
            return (selector?.SelectedItem as TemperatureSensorOption)?.Identifier ?? string.Empty;
        }

        private static ISensor GetConfiguredTemperatureSensor(ComboBox selector, IHardware hardware, bool isCpu)
        {
            ISensor sensor = (selector?.SelectedItem as TemperatureSensorOption)?.Sensor;
            if (sensor != null)
                return sensor;

            return isCpu
                ? SelectPreferredCpuTemperatureSensor(hardware)
                : SelectPreferredGpuTemperatureSensor(hardware);
        }

        internal sealed class TemperatureSensorOption
        {
            public TemperatureSensorOption(ISensor sensor, string displayName)
            {
                Sensor = sensor;
                Identifier = sensor?.Identifier.ToString() ?? string.Empty;
                DisplayName = displayName;
            }

            public ISensor Sensor { get; }
            public string Identifier { get; }
            public string DisplayName { get; }

            public override string ToString() => DisplayName;
        }

        private static ISensor SelectPreferredCpuTemperatureSensor(IHardware cpuHardware)
        {
            var tempSensors = cpuHardware.Sensors
                .Where(s => s.SensorType == SensorType.Temperature)
                .ToList();

            ISensor rootSensor =
                tempSensors.FirstOrDefault(s =>
                    s.Name.IndexOf("Package", StringComparison.OrdinalIgnoreCase) >= 0)

                ?? tempSensors.FirstOrDefault(s =>
                    s.Name.IndexOf("Tctl", StringComparison.OrdinalIgnoreCase) >= 0)

                ?? tempSensors.FirstOrDefault(s =>
                    s.Name.IndexOf("Core Max", StringComparison.OrdinalIgnoreCase) >= 0)

                ?? tempSensors
                    .OrderByDescending(s => s.Value ?? 0)
                    .FirstOrDefault();

            if (IsUsableTemperatureSensor(rootSensor))
                return rootSensor;

            var fallbackSensors = EnumerateTemperatureSensorsRecursive(cpuHardware.SubHardware)
                .Where(IsUsableTemperatureSensor)
                .ToList();

            return SelectPreferredCpuTemperatureSensorFromCandidates(fallbackSensors);
        }

        private static ISensor SelectPreferredGpuTemperatureSensor(IHardware gpuHardware)
        {
            var tempSensors = gpuHardware.Sensors
                .Where(s => s.SensorType == SensorType.Temperature)
                .ToList();

            ISensor rootSensor =
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

            if (IsUsableTemperatureSensor(rootSensor))
                return rootSensor;

            var fallbackSensors = EnumerateTemperatureSensorsRecursive(gpuHardware.SubHardware)
                .Where(IsUsableTemperatureSensor)
                .ToList();

            return SelectPreferredGpuTemperatureSensorFromCandidates(fallbackSensors);
        }

        private void RefreshCpuFallbackSensorHardware()
        {
            IHardware sensorHardware = _cpuTempSensor?.Hardware;

            if (sensorHardware == null || ReferenceEquals(sensorHardware, _selectedCpuHardware))
                return;

            sensorHardware.Update();
        }

        private void RefreshUnavailableTemperatureSensors()
        {
            const int RescanAfterInvalidCycles = 3;

            if (IsUsableTemperatureSensor(_cpuTempSensor))
            {
                _cpuInvalidSensorCycles = 0;
            }
            else if (++_cpuInvalidSensorCycles >= RescanAfterInvalidCycles && _selectedCpuHardware != null)
            {
                _cpuTempSensor = GetConfiguredTemperatureSensor(cpuTempSensorSelect, _selectedCpuHardware, isCpu: true);
                _cpuInvalidSensorCycles = 0;
            }

            if (IsUsableTemperatureSensor(_gpuTempSensor))
            {
                _gpuInvalidSensorCycles = 0;
            }
            else if (++_gpuInvalidSensorCycles >= RescanAfterInvalidCycles && _selectedGpuHardware != null)
            {
                _gpuTempSensor = GetConfiguredTemperatureSensor(gpuTempSensorSelect, _selectedGpuHardware, isCpu: false);
                _gpuInvalidSensorCycles = 0;
            }
        }

        private static int GetSavedHardwareIndex(List<IHardware> hardwares, string savedIdentifier, int fallbackIndex)
        {
            if (hardwares == null || hardwares.Count == 0)
                return 0;

            if (!string.IsNullOrWhiteSpace(savedIdentifier))
            {
                int identifierIndex = hardwares.FindIndex(h =>
                    h != null && string.Equals(h.Identifier.ToString(), savedIdentifier, StringComparison.OrdinalIgnoreCase));
                if (identifierIndex >= 0)
                    return identifierIndex;
            }

            return fallbackIndex >= 0 && fallbackIndex < hardwares.Count ? fallbackIndex : 0;
        }

        private static IEnumerable<ISensor> EnumerateTemperatureSensorsRecursive(IEnumerable<IHardware> hardwares)
        {
            foreach (IHardware hardware in hardwares)
            {
                if (hardware == null)
                    continue;

                foreach (ISensor sensor in hardware.Sensors)
                {
                    if (sensor.SensorType == SensorType.Temperature)
                        yield return sensor;
                }

                foreach (ISensor sensor in EnumerateTemperatureSensorsRecursive(hardware.SubHardware))
                    yield return sensor;
            }
        }

        private bool PromptForElevationAtStartupIfNeeded()
        {
            if (_sensorElevationPromptShown || IsRunningAsAdministrator() || _isShutdownInitiated || _resourcesDisposed)
                return true;

            _sensorElevationPromptShown = true;

            if (minimizeOnStart.Checked)
            {
                return !IsStartMinimizedWithAdminRights || !RestartElevatedAndClose();
            }

            const string message =
                "Tray-Temps can read more complete hardware sensor data when run as administrator.\n\n" +
                "If you continue without administrator rights, some temperatures, storage health data, or hardware details may be missing, partial, or less reliable.\n\n" +
                "Windows Security / Microsoft Defender may block low-level hardware access. Only allow the app or add an exclusion if you trust this specific build/source.\n\n" +
                "Restart Tray-Temps as administrator now?";

            DialogResult result = MessageBox.Show(
                this,
                message,
                "Sensor Access",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result != DialogResult.Yes)
                return true;

            if (!RestartElevatedAndClose())
            {
                MessageBox.Show(
                    this,
                    "Windows did not allow the elevated restart.",
                    "Elevation Cancelled",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                return true;
            }

            return false;
        }

        private static bool IsUsableTemperatureSensor(ISensor sensor)
        {
            return TemperatureFormatHelper.IsValidTemp(sensor?.Value);
        }

        private static ISensor SelectPreferredCpuTemperatureSensorFromCandidates(List<ISensor> tempSensors)
        {
            return
                tempSensors.FirstOrDefault(s =>
                    s.Name.IndexOf("Package", StringComparison.OrdinalIgnoreCase) >= 0)

                ?? tempSensors.FirstOrDefault(s =>
                    s.Name.IndexOf("Tctl", StringComparison.OrdinalIgnoreCase) >= 0)

                ?? tempSensors.FirstOrDefault(s =>
                    s.Name.IndexOf("Core Max", StringComparison.OrdinalIgnoreCase) >= 0)

                ?? tempSensors
                    .OrderByDescending(s => s.Value ?? 0)
                    .FirstOrDefault();
        }

        private static ISensor SelectPreferredGpuTemperatureSensorFromCandidates(List<ISensor> tempSensors)
        {
            return
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
        }

        private void UpdateStorageDetailsText()
        {
            List<string> storageNames = _storageHardwares?
                .Where(hardware => !string.IsNullOrWhiteSpace(hardware?.Name))
                .Select(hardware => hardware.Name)
                .ToList();

            if (storageNames == null || storageNames.Count == 0)
                storageNames = _wmiStorageDisplayNames?.Where(name => !string.IsNullOrWhiteSpace(name)).ToList();

            storageDetails.Text = storageNames == null || storageNames.Count == 0
                ? "No Disk found"
                : string.Join(" | ", storageNames.Select((name, index) => $"{index + 1}.{name}"));
        }

        private static List<string> LoadWmiStorageDisplayNames()
        {
            var displayNames = new List<string>();
            List<ManagementObject> disks = WmiQueryHelper.WmiQuery("SELECT * FROM Win32_DiskDrive");
            try
            {
                foreach (ManagementObject disk in disks)
                    displayNames.Add(HardwareReportFormatHelper.GetDiskDisplayTitle(disk["Model"], disk["SerialNumber"]));
            }
            finally
            {
                WmiQueryHelper.DisposeAll(disks);
            }

            return displayNames;
        }

        private void CpuColorValue_Click(object sender, EventArgs e)
        {
            HandleTrayColorSelection(cpuColorValue, resetCpuTrayCacheText: true, resetGpuTrayCacheText: false);
        }

        private void GpuColorValue_Click(object sender, EventArgs e)
        {
            HandleTrayColorSelection(gpuColorValue, resetCpuTrayCacheText: false, resetGpuTrayCacheText: true);
        }

        private void HandleTrayColorSelection(Button targetButton, bool resetCpuTrayCacheText, bool resetGpuTrayCacheText)
        {
            using (ColorDialog cd = new ColorDialog())
            {
                cd.Color = targetButton.BackColor;
                if (cd.ShowDialog() == DialogResult.OK)
                {
                    targetButton.BackColor = cd.Color;

                    RefreshDisplaySettingPreview(resetCpuTrayCacheText, resetGpuTrayCacheText);
                }
            }
        }

        #endregion

        #region [ Hardware Details Dialogs ]

        // =========================
        // UI Entry Points
        // =========================

        public async void CpuModel_Click(object sender, EventArgs e)
        {
            await HardwareDialogCoordinator.ShowHardwareDialogFromClickAsync(
                this,
                "CpuModel_Click",
                HardwareDialogTextHelper.GetComponentDisplayName(_selectedCpuHardware, cpuModel.Text, "CPU"),
                "CPU",
                () => HardwareCpuInfoQueryHelper.GetCpuDetails(
                    _selectedCpuHardware != null,
                    _selectedCpuHardware?.Name,
                    _selectedCpuHardware?.Identifier.ToString(),
                    WmiQueryHelper.WmiQuery),
                _hardwareUpdateLock,
                UpdateHardwareRecursive,
                RegisterHardwareDialog,
                UnregisterHardwareDialog,
                () => _isShutdownInitiated,
                IsLightModeEnabled,
                _selectedCpuHardware);
        }

        public async void GpuModel_Click(object sender, EventArgs e)
        {
            await HardwareDialogCoordinator.ShowHardwareDialogFromClickAsync(
                this,
                "GpuModel_Click",
                HardwareDialogTextHelper.GetComponentDisplayName(_selectedGpuHardware, gpuModel.Text, "GPU"),
                "GPU",
                () => HardwareGpuInfoQueryHelper.GetGpuDetails(
                    _selectedGpuHardware?.Name,
                    _selectedGpuHardware?.Identifier.ToString(),
                    WmiQueryHelper.WmiQuery),
                _hardwareUpdateLock,
                UpdateHardwareRecursive,
                RegisterHardwareDialog,
                UnregisterHardwareDialog,
                () => _isShutdownInitiated,
                IsLightModeEnabled,
                _selectedGpuHardware);
        }

        public async void RamDetails_Click(object sender, EventArgs e)
        {
            await HardwareDialogCoordinator.ShowHardwareDialogFromClickAsync(
                this,
                "RamDetails_Click",
                HardwareDialogTextHelper.GetCleanDialogTitle(ramDetails.Text, "RAM"),
                "RAM",
                () => HardwareInfoQueryHelper.GetRamDetails(WmiQueryHelper.WmiQuery),
                _hardwareUpdateLock,
                UpdateHardwareRecursive,
                RegisterHardwareDialog,
                UnregisterHardwareDialog,
                () => _isShutdownInitiated,
                IsLightModeEnabled,
                GetFirstHardware(HardwareType.Memory));
        }

        public async void StorageDetails_Click(object sender, EventArgs e)
        {
            await HardwareDialogCoordinator.ShowHardwareDialogFromClickAsync(
                this,
                "StorageDetails_Click",
                "Storage",
                "Storage",
                () => StorageReportHelper.GetStorageDetails(_storageHardwares, UpdateHardwareRecursive, WmiQueryHelper.WmiQuery),
                _hardwareUpdateLock,
                UpdateHardwareRecursive,
                RegisterHardwareDialog,
                UnregisterHardwareDialog,
                () => _isShutdownInitiated,
                IsLightModeEnabled,
                liveTextFactory: BuildAllStorageSensorsTextAsync);
        }

        public async void MotherboardDetails_Click(object sender, EventArgs e)
        {
            await HardwareDialogCoordinator.ShowHardwareDialogFromClickAsync(
                this,
                "MotherboardDetails_Click",
                HardwareDialogTextHelper.GetCleanDialogTitle(motherboardDetails.Text, "Motherboard"),
                "Motherboard",
                () => HardwareInfoQueryHelper.GetMotherboardDetails(WmiQueryHelper.WmiQuery),
                _hardwareUpdateLock,
                UpdateHardwareRecursive,
                RegisterHardwareDialog,
                UnregisterHardwareDialog,
                () => _isShutdownInitiated,
                IsLightModeEnabled,
                GetFirstHardware(HardwareType.Motherboard));
        }

        private void ApplyLabelHover(params Label[] labels)
        {
            foreach (Label label in labels)
            {
                label.MouseEnter += (s, e) =>
                {
                    label.ForeColor = GetThemePalette().HardwareHoverText;
                    label.Cursor = Cursors.Hand;
                };

                label.MouseLeave += (s, e) =>
                {
                    label.ForeColor = GetThemePalette().Text;
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


        // =========================
        // Live Sensors

        private Task<string> BuildAllStorageSensorsTextAsync()
        {
            return Task.Run(() =>
            {
                lock (_hardwareUpdateLock)
                {
                    return HardwareLiveSensorsTextHelper.BuildAllStorageSensorsText(
                        _storageHardwares,
                        UpdateHardwareRecursive);
                }
            });
        }

        // =========================
        // WMI Helpers
        // =========================
        // =========================
        // CPU
        // =========================

        // =========================
        // GPU
        // =========================

        // =========================
        // RAM
        // =========================

        // =========================
        // STORAGE
        // =========================

        // =========================
        // MOTHERBOARD + BIOS
        // =========================

        #endregion

        #region [ Utility Helpers ]


        public void ResetTrayCache()
        {
            _lastCpuTempText = null;
            _lastGpuTempText = null;
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

        private bool RunProcessAndWait(string fileName, string arguments, out string error, int timeoutMs = System.Threading.Timeout.Infinite)
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

                    Task<string> outputTask = process.StandardOutput.ReadToEndAsync();
                    Task<string> errorTask = process.StandardError.ReadToEndAsync();

                    bool exited = timeoutMs == System.Threading.Timeout.Infinite
                        ? process.WaitForExit(int.MaxValue)
                        : process.WaitForExit(timeoutMs);

                    if (!exited)
                    {
                        try
                        {
                            process.Kill();
                            process.WaitForExit();
                        }
                        catch
                        {
                        }

                        error = $"Process timed out after {timeoutMs} ms.";
                        return false;
                    }

                    Task.WaitAll(outputTask, errorTask);

                    string output = outputTask.Result;
                    string stdError = errorTask.Result;

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

        private static bool IsRunningAsAdministrator()
        {
            try
            {
                using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
                {
                    WindowsPrincipal principal = new WindowsPrincipal(identity);
                    return principal.IsInRole(WindowsBuiltInRole.Administrator);
                }
            }
            catch
            {
                return false;
            }
        }

        private bool PromptForElevatedSensorRestart(string reason)
        {
            if (minimizeOnStart.Checked)
                return IsStartMinimizedWithAdminRights && RestartElevatedAndClose();

            string message =
                "TrayTemps could not initialize full hardware sensors without administrator rights.\n\n" +
                "Restart as administrator to enable low-level temperature sensors?\n\n" +
                "Reason: " + reason;

            DialogResult result = MessageBox.Show(
                this,
                message,
                "Sensor Access",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result != DialogResult.Yes)
                return false;

            if (!RestartElevatedAndClose())
            {
                MessageBox.Show(
                    this,
                    "Windows did not allow the elevated restart.",
                    "Elevation Cancelled",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                return false;
            }

            return true;
        }

        private void PromptForElevationIfCriticalSensorsAreMissing()
        {
            if (_sensorElevationPromptShown || IsRunningAsAdministrator() || _isShutdownInitiated || _resourcesDisposed)
                return;

            bool hasCpuHardware = _cpuHardwares != null && _cpuHardwares.Count > 0;
            bool missingCpuTemperature = hasCpuHardware &&
                (_cpuTempSensor == null || !TemperatureFormatHelper.IsValidTemp(_cpuTempSensor.Value));
            bool missingStorage = _storageHardwares == null || _storageHardwares.Count == 0;

            if (!missingCpuTemperature && !missingStorage)
                return;

            _sensorElevationPromptShown = true;

            var reasons = new List<string>();

            if (missingCpuTemperature)
                reasons.Add("CPU temperature sensor is unavailable");

            if (missingStorage)
                reasons.Add("storage sensors are unavailable");

            PromptForElevatedSensorRestart(string.Join("; ", reasons));
        }

        private bool TryRestartElevated(bool preserveStartupVisibility = false)
        {
            return Program.RequestElevatedRestart(
                preserveStartupVisibility && _startHiddenFromCommandLine,
                preserveStartupVisibility && _forceVisibleOnStartup);
        }

        private bool RestartElevatedAndClose()
        {
            if (!TryRestartElevated(preserveStartupVisibility: true))
                return false;

            ExecuteShutdownSequence();
            Close();
            return true;
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

        private void LoadFonts()
        {
            EmbeddedFonts.Initialize();
            BunkenBold = EmbeddedFonts.Bold;
            BunkenRegular = EmbeddedFonts.Book;
        }

        #endregion

        #region [ Install / Autostart / Elevation ]

        private void ExecuteShutdownSequence()
        {
            if (_isShutdownInitiated)
                return;

            _isShutdownInitiated = true;
            _resourcesDisposed = true;

            StopTemperatureTimerForShutdown();
            CloseHardwareDialogsForShutdown();
            CloseHardwareMonitorForShutdown();

            try
            {
                ServiceManager.StopService("R0TrayTemps", 5);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("ExecuteShutdownSequence: ServiceManager.StopService failed: " + ex);
            }

            DisposeTrayIconsForShutdown();
            DisposeUiResourcesForShutdown();
            DisposeTimerForShutdown();
        }

        private void StopTemperatureTimerForShutdown()
        {
            try
            {
                _tempTimer.Stop();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("ExecuteShutdownSequence: failed to stop timer: " + ex);
            }
        }

        private void CloseHardwareDialogsForShutdown()
        {
            HardwareDialogStateHelper.CloseOpenHardwareDialogs(
                _openHardwareDialogs,
                bounds => _savedHardwareDialogBounds = bounds);
        }

        private void CloseHardwareMonitorForShutdown()
        {
            try
            {
                lock (_hardwareUpdateLock)
                {
                    try
                    {
                        _computer?.Close();
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("ExecuteShutdownSequence: _computer.Close failed: " + ex);
                    }

                    _computer = null;

                    _selectedCpuHardware = null;
                    _selectedGpuHardware = null;

                    _cpuTempSensor = null;
                    _gpuTempSensor = null;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("ExecuteShutdownSequence: error while clearing hardware refs: " + ex);
            }
        }

        private void DisposeTrayIconsForShutdown()
        {
            try
            {
                DisposeTrayIcon(cpuTrayIcon);
                cpuTrayIcon = null;

                DisposeTrayIcon(gpuTrayIcon);
                gpuTrayIcon = null;

                DisposeTrayIcon(NotifyIcon);
                NotifyIcon = null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("ExecuteShutdownSequence: error disposing tray icons: " + ex);
            }
        }

        private void DisposeUiResourcesForShutdown()
        {
            try
            {
                _trayFont?.Dispose();
                _cpuBrush?.Dispose();
                _gpuBrush?.Dispose();

                _trayFont = null;
                _cpuBrush = null;
                _gpuBrush = null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("ExecuteShutdownSequence: error disposing brushes: " + ex);
            }
        }

        private void DisposeTimerForShutdown()
        {
            try
            {
                _tempTimer.Dispose();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("ExecuteShutdownSequence: failed disposing timer: " + ex);
            }
        }

        private static void DisposeTrayIcon(NotifyIcon trayIcon)
        {
            if (trayIcon == null)
                return;

            try
            {
                trayIcon.Visible = false;

                Icon icon = trayIcon.Icon;
                trayIcon.Icon = null;
                icon?.Dispose();

                trayIcon.Dispose();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("DisposeTrayIcon failed: " + ex);
            }
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

                if (!IsDisposed)
                    UpdateAutostartCheckboxStateAndText();
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

            if (!IsRunningAsAdministrator())
            {
                HandleAutostartAdminRequired(
                    "Windows startup installation needs administrator rights.\n\nTrayTemps will restart elevated; then enable Autostart again.",
                    false);
                return;
            }

            bool installed = await InstallAndRestartAsync();

            if (!installed)
                RevertCheckbox(autostartInstall, false);
        }

        private async Task HandleAutostartDisable()
        {
            if (IsInstalledAppPresent() && !IsStartupEntryPresent())
            {
                var installedOnlyResult = MessageBox.Show(
                    this,
                    "The startup entry is already removed.\n\nYes = Re-add startup entry\nNo = Remove app\nCancel = Keep installed",
                    "Confirm Remove",
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Warning);

                switch (installedOnlyResult)
                {
                    case DialogResult.Yes:
                        if (!IsRunningAsAdministrator())
                        {
                            HandleAutostartAdminRequired(
                                "Adding the startup task needs administrator rights.\n\nTrayTemps will restart elevated; then choose Re-add startup entry again.",
                                true);
                            break;
                        }

                        await AddStartupTaskAsync();
                        SaveSettings();
                        MessageBox.Show(
                            this,
                            "Startup entry added.",
                            "Startup",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                        break;

                    case DialogResult.No:
                        await HandleAutostartDisableYes();
                        break;

                    case DialogResult.Cancel:
                        HandleAutostartDisableCancel();
                        break;
                }

                return;
            }

            var result = MessageBox.Show(
                this,
                "Remove installed app, shortcut, and startup entry?\n\nYes = Remove all\nNo = Remove only startup entry\nCancel = Do nothing",
                "Confirm Remove",
                MessageBoxButtons.YesNoCancel,
                MessageBoxIcon.Warning);

            switch (result)
            {
                case DialogResult.Yes:
                    await HandleAutostartDisableYes();
                    break;

                case DialogResult.No:
                    await HandleAutostartDisableNo();
                    break;

                case DialogResult.Cancel:
                    HandleAutostartDisableCancel();
                    break;
            }
        }

        private Task HandleAutostartDisableYes()
        {
            if (!IsRunningAsAdministrator())
            {
                HandleAutostartAdminRequired(
                    "Full uninstall needs administrator rights.\n\nTrayTemps will restart elevated; then disable Autostart again.",
                    true);
                return Task.CompletedTask;
            }

            SaveSettings();
            UninstallAndExit();
            return Task.CompletedTask;
        }

        private async Task HandleAutostartDisableNo()
        {
            if (!IsRunningAsAdministrator())
            {
                HandleAutostartAdminRequired(
                    "Removing the startup task needs administrator rights.\n\nTrayTemps will restart elevated; then disable Autostart again.",
                    true);
                return;
            }

            string error = await RemoveStartupTaskAsync();
            if (!string.IsNullOrWhiteSpace(error))
                throw new Exception("Could not remove startup task:\n" + error);

            SaveSettings();

            MessageBox.Show(
                this,
                "Startup entry removed.",
                "Info",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        private void HandleAutostartDisableCancel()
        {
            RevertCheckbox(autostartInstall, true);
        }

        private void HandleAutostartAdminRequired(string message, bool revertValue)
        {
            MessageBox.Show(
                this,
                message,
                "Administrator Required",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            RevertCheckbox(autostartInstall, revertValue);

            if (TryRestartElevated())
            {
                ExecuteShutdownSequence();
                Close();
            }
        }

        private void RevertCheckbox(CheckBox chk, bool state)
        {
            _isInternalCheckChange = true;
            chk.Checked = state;
            _isInternalCheckChange = false;
        }

        private string GetInstalledExePath()
        {
            if (string.IsNullOrWhiteSpace(InstallPath))
                return string.Empty;

            return Path.Combine(InstallPath, $"{AppName}.exe");
        }

        private bool IsInstalledAppPresent()
        {
            string installedExePath = GetInstalledExePath();
            return !string.IsNullOrWhiteSpace(installedExePath) && File.Exists(installedExePath);
        }

        private bool IsRunningFromInstalledPath()
        {
            string installedExePath = GetInstalledExePath();

            if (string.IsNullOrWhiteSpace(installedExePath))
                return false;

            try
            {
                string currentExePath = Path.GetFullPath(Application.ExecutablePath.Trim());
                string installedFullPath = Path.GetFullPath(installedExePath.Trim());
                return string.Equals(currentExePath, installedFullPath, StringComparison.OrdinalIgnoreCase);
            }
            catch
            {
                return string.Equals(Application.ExecutablePath, installedExePath, StringComparison.OrdinalIgnoreCase);
            }
        }

        private bool IsStartupEntryPresent()
        {
            bool result = RunProcessAndWait(
                "schtasks.exe",
                $"/Query /TN \"{AppName}\"",
                out string error,
                StartupTaskQueryTimeoutMs);

            if (!result && !string.IsNullOrWhiteSpace(error))
                Debug.WriteLine("IsStartupEntryPresent failed: " + error);

            return result;
        }

        private void UpdateAutostartCheckboxStateAndText()
        {
            bool isInstalled = IsInstalledAppPresent();
            bool hasStartupEntry = isInstalled && IsStartupEntryPresent();

            RevertCheckbox(autostartInstall, isInstalled);

            if (!isInstalled)
            {
                autostartInstall.Text = "Install and start with Windows";
            }
            else if (hasStartupEntry)
            {
                autostartInstall.Text = "Installed — startup task enabled";
            }
            else
            {
                autostartInstall.Text = "Installed — startup task disabled";
            }
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

                    if (!CreateStartupTask(destExe, out string error))
                        throw new Exception("Could not create startup task:\n" + error);

                    CreateShortcutOnDesktop(destExe);
                    SaveSettings();

                    if (IsHandleCreated && !IsDisposed)
                    {
                        Invoke(new Action(() =>
                        {
                            MessageBox.Show(
                                this,
                                "TrayTemps has been installed successfully.\nIt will now restart from the installed location.",
                                "Installation Complete",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);

                            if (RestartFromInstalledPath(destExe, out string restartError))
                            {
                                ExecuteShutdownSequence();
                                Close();
                            }
                            else
                            {
                                MessageBox.Show(
                                    this,
                                    "TrayTemps was installed, but the installed copy could not be started.\n\n" + restartError,
                                    "Installation Error",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                            }
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

        private bool RestartFromInstalledPath(string installedExePath, out string error)
        {
            error = string.Empty;

            try
            {
                if (string.IsNullOrWhiteSpace(installedExePath) || !File.Exists(installedExePath))
                {
                    error = "Installed executable was not found.";
                    return false;
                }

                var startInfo = new ProcessStartInfo(installedExePath)
                {
                    UseShellExecute = true,
                    WorkingDirectory = Path.GetDirectoryName(installedExePath),
                    Arguments = "-elevated-restart"
                };

                Process process = Process.Start(startInfo);
                return process != null;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }
        }

        private void UninstallAndExit()
        {
            MessageBox.Show(
                this,
                "TrayTemps will now uninstall and exit.\n\nIf TrayTemps is running from the installed location, it will be removed after exit.",
                "Uninstall",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

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

            bool shouldCloseCurrentApp = IsRunningFromInstalledPath();

            string shortcutPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory),
                $"{AppName}.lnk");
            string settingsFolder = Path.GetDirectoryName(SettingsFilePath);

            if (!shouldCloseCurrentApp)
            {
                if (TryDirectUninstallInstalledCopy(shortcutPath, out string error))
                {
                    InstallPath = string.Empty;
                    UpdateAutostartCheckboxStateAndText();
                }
                else
                {
                    MessageBox.Show(
                        this,
                        error,
                        "Uninstall Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }

                return;
            }

            string batPath = Path.Combine(Path.GetTempPath(),
                                          $"DeleteTrayTemps_{Guid.NewGuid():N}.bat");

            string script = $@"@echo off
setlocal
cd /d ""%~dp0""

schtasks /Delete /TN ""{AppName}"" /F > nul 2>&1
sc stop ""R0TrayTemps"" > nul 2>&1

set ""install_folder={InstallPath}""
set ""settings_folder={settingsFolder}""
set attempts=0

:loop
if %attempts% GEQ 15 goto settings
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

            Process process = Process.Start(new ProcessStartInfo(batPath)
            {
                UseShellExecute = true,
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden
            });

            if (process == null)
            {
                MessageBox.Show(
                    this,
                    "Uninstall could not start.",
                    "Uninstall Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                return;
            }

            _settingsLoaded = false;
            ExecuteShutdownSequence();
            Close();
        }

        private bool TryDirectUninstallInstalledCopy(string shortcutPath, out string error)
        {
            error = string.Empty;

            try
            {
                RunProcessAndWait("schtasks.exe", $"/Delete /TN \"{AppName}\" /F", out string _);

                if (Directory.Exists(InstallPath))
                    Directory.Delete(InstallPath, true);

                if (File.Exists(shortcutPath))
                    File.Delete(shortcutPath);

                return true;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }
        }

        private Task AddStartupTaskAsync()
        {
            string installedExePath = GetInstalledExePath();

            return Task.Run(() =>
            {
                if (string.IsNullOrWhiteSpace(installedExePath) || !File.Exists(installedExePath))
                    throw new FileNotFoundException("Installed executable was not found.", installedExePath);

                if (!CreateStartupTask(installedExePath, out string error))
                    throw new Exception("Could not create startup task:\n" + error);
            });
        }

        private bool CreateStartupTask(string executablePath, out string error)
        {
            string createTaskArgs =
                $"/Create /F /RL HIGHEST /SC ONLOGON /TN \"{AppName}\" /TR \"\\\"{executablePath}\\\" -silent\"";

            if (!RunProcessAndWait("schtasks.exe", createTaskArgs, out error))
                return false;

            string powerShellArgs =
                $"-NoProfile -Command \"Set-ScheduledTask -TaskName '{AppName}' -Settings (New-ScheduledTaskSettingsSet -AllowStartIfOnBatteries -DontStopIfGoingOnBatteries)\"";

            RunProcessAndWait("powershell.exe", powerShellArgs, out string _);
            return true;
        }

        private Task<string> RemoveStartupTaskAsync()
        {
            return Task.Run(() =>
            {
                if (RunProcessAndWait("schtasks.exe", $"/Delete /TN \"{AppName}\" /F", out string error))
                    return null;

                return string.IsNullOrWhiteSpace(error)
                    ? "schtasks.exe did not complete successfully."
                    : error;
            });
        }

        #endregion
    }
}
