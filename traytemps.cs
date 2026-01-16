using LibreHardwareMonitor.Hardware;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using SystemInformation = System.Windows.Forms.SystemInformation;

namespace TrayTemps
{
    public partial class TrayTemps : Form
    {
        #region Fields & Constants

        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HTCAPTION = 0x2;
        private const string RegistryPath = @"Software\TrayTemps";
        private const string AppName = "TrayTemps";
        private const int IconSize = 16;

        private Computer _computer;
        private readonly Timer _tempTimer = new Timer();

        private List<IHardware> _cpuHardwares;
        private IHardware _selectedCpuHardware;
        private string _selectedCpuIdentifier;

        private List<IHardware> _gpuHardwares;
        private IHardware _selectedGpuHardware;
        private string _selectedGpuIdentifier;

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
        private bool isShutdownInitiated = false;

        private Color _cpuTrayColor;
        private Color _gpuTrayColor;
        private float _trayFontSize;
        private string _trayFontFamily;

        #endregion

        #region P/Invoke Imports

        [DllImport("user32.dll")]
        private static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool DestroyIcon(IntPtr hIcon);

        #endregion

        #region Constructor & Form Events

        public TrayTemps()
        {
            InitializeComponent();
        }

        private async void TrayTemps_Load(object sender, EventArgs e)
        {
            this.UpdateInterval.ValueChanged -= this.UpdateInterval_ValueChanged;
            this.FontSizeTray.ValueChanged -= this.FontSizeTray_ValueChanged;

            SetDefaultControlValues();
            LoadSettings();

            this.UpdateInterval.ValueChanged += this.UpdateInterval_ValueChanged;
            this.FontSizeTray.ValueChanged += this.FontSizeTray_ValueChanged;

            await InitializeHardwareAsync();
            SetupTimer();
            UpdateTrayIcons();
            TempTimer_Tick(this, EventArgs.Empty);
        }

        private void ExecuteShutdownSequence()
        {
            if (isShutdownInitiated)
            {
                return;
            }
            isShutdownInitiated = true;

            _tempTimer.Stop();
            _tempTimer.Dispose();
            cpuTrayIcon?.Dispose();
            gpuTrayIcon?.Dispose();
            NotifyIcon?.Dispose();

            Task.Run(() => ServiceManager.StopServiceAsync("R0TrayTemps"));
            _computer?.Close();
        }

        private void TrayTemps_FormClosing(object sender, FormClosingEventArgs e)
        {
            ExecuteShutdownSequence();
        }

        #endregion

        #region UI Event Handlers


        private void Exit_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(this, "Are you sure you want to exit TrayTemps?\nClick \"No\" to hide the app to tray.", "Exit Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
            {
                this.Hide();
                UpdateTrayIcons();
            }
            else
            {
                ExecuteShutdownSequence();
                this.Close();
            }
        }

        private void ExitForm_Click(object sender, EventArgs e)
        {
            ExecuteShutdownSequence();
            this.Close();
        }

        private void ShowForm_Click(object sender, EventArgs e) => ShowWindow();

        private void Minimize_Click(object sender, EventArgs e)
        {
            this.Hide();
            UpdateTrayIcons();
        }

        private void TrayTemps_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HTCAPTION, 0);
            }
        }

        private void TrayIcon_MouseDoubleClick(object sender, MouseEventArgs e) => ShowWindow();

        private void NotifyIcon_MouseDoubleClick(object sender, MouseEventArgs e) => TrayIcon_MouseDoubleClick(sender, e);
        private void CpuTrayIcon_MouseDoubleClick(object sender, MouseEventArgs e) => TrayIcon_MouseDoubleClick(sender, e);
        private void GpuTrayIcon_MouseDoubleClick(object sender, MouseEventArgs e) => TrayIcon_MouseDoubleClick(sender, e);
        private void CpuTrayEnable_CheckedChanged(object sender, EventArgs e) => Setting_CheckedChanged(sender, e);
        private void GpuTrayEnable_CheckedChanged(object sender, EventArgs e) => Setting_CheckedChanged(sender, e);
        private void CpuTrayColor_SelectedIndexChanged(object sender, EventArgs e) => Setting_SelectedIndexChanged(sender, e);
        private void GpuTrayColor_SelectedIndexChanged(object sender, EventArgs e) => Setting_SelectedIndexChanged(sender, e);
        private void FontFamilyTray_SelectedIndexChanged(object sender, EventArgs e) => Setting_SelectedIndexChanged(sender, e);

        private void CpuSelector_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CpuSelector.SelectedIndex < 0 || _cpuHardwares == null || CpuSelector.SelectedIndex >= _cpuHardwares.Count) return;

            _selectedCpuHardware = _cpuHardwares[CpuSelector.SelectedIndex];
            CpuName.Text = _selectedCpuHardware.Name;

            _cpuTempSensor = _selectedCpuHardware.Sensors.FirstOrDefault(s => s.SensorType == SensorType.Temperature && s.Name.Contains("Package"))
                             ?? _selectedCpuHardware.Sensors.FirstOrDefault(s => s.SensorType == SensorType.Temperature);

            _selectedCpuIdentifier = _selectedCpuHardware.Identifier.ToString();
            SaveSetting("SelectedCpuIdentifier", _selectedCpuIdentifier);

            _cpuMinTemp = float.MaxValue;
            _cpuMaxTemp = float.MinValue;
            _lastCpuTempText = null;
            TempTimer_Tick(this, EventArgs.Empty);
        }
        private void GpuSelector_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (GpuSelector.SelectedIndex < 0 || _gpuHardwares == null || GpuSelector.SelectedIndex >= _gpuHardwares.Count) return;

            _selectedGpuHardware = _gpuHardwares[GpuSelector.SelectedIndex];
            GpuName.Text = _selectedGpuHardware.Name;

            _gpuTempSensor = _selectedGpuHardware.Sensors.FirstOrDefault(s => s.SensorType == SensorType.Temperature && s.Name.Contains("Core"))
                             ?? _selectedGpuHardware.Sensors.FirstOrDefault(s => s.SensorType == SensorType.Temperature);

            _selectedGpuIdentifier = _selectedGpuHardware.Identifier.ToString();
            SaveSetting("SelectedGpuIdentifier", _selectedGpuIdentifier);

            _gpuMinTemp = float.MaxValue;
            _gpuMaxTemp = float.MinValue;
            _lastGpuTempText = null;
            TempTimer_Tick(this, EventArgs.Empty);
        }

        #endregion

        #region Core Logic

        private void SetDefaultControlValues()
        {
            UpdateInterval.Value = 0.50M;
            FontSizeTray.Value = 12M;
            CpuTrayColor.SelectedIndex = 0;
            GpuTrayColor.SelectedIndex = 4;
            FontFamilyTray.SelectedIndex = 0;
        }

        private async Task InitializeHardwareAsync()
        {
            var mboNameTask = GetMotherboardNameAsync();
            var ramInfoTask = GetRamInfoAsync();

            MboName.Text = await mboNameTask;
            RamAmount.Text = await ramInfoTask;

            await InitializeHardwareMonitorAsync();
        }

        private Task InitializeHardwareMonitorAsync()
        {
            return Task.Run(() =>
            {
                _computer = new Computer { IsCpuEnabled = true, IsGpuEnabled = true };
                _computer.Open();

                _cpuHardwares = _computer.Hardware.Where(h => h.HardwareType == HardwareType.Cpu).ToList();
                _gpuHardwares = _computer.Hardware.Where(h => h.HardwareType == HardwareType.GpuAmd || h.HardwareType == HardwareType.GpuNvidia || h.HardwareType == HardwareType.GpuIntel).ToList();

                this.Invoke((MethodInvoker)delegate
                {
                    PopulateHardwareSelector(CpuSelector, _cpuHardwares, _selectedCpuIdentifier, CpuName, "CPU");
                    PopulateHardwareSelector(GpuSelector, _gpuHardwares, _selectedGpuIdentifier, GpuName, "GPU");
                });
            });
        }

        private void SetupTimer()
        {
            UpdateTimerInterval();
            _tempTimer.Tick += TempTimer_Tick;
            _tempTimer.Start();
        }

        private async void TempTimer_Tick(object sender, EventArgs e)
        {
            _tempTimer.Stop();

            try
            {
                (float? cpuTemp, float? gpuTemp) = await Task.Run(() =>
                {
                    _selectedCpuHardware?.Update();
                    _selectedGpuHardware?.Update();
                    return (_cpuTempSensor?.Value, _gpuTempSensor?.Value);
                });

                UpdateTemperatures(cpuTemp, gpuTemp);
                UpdateAllTrayIcons(cpuTemp, gpuTemp);
            }
            finally
            {
                if (this.IsHandleCreated && !isShutdownInitiated)
                {
                    _tempTimer.Start();
                }
            }
        }

        #endregion

        #region UI & Icon Updates

        private void ShowWindow()
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
            UpdateTrayIcons();
        }

        private void UpdateTrayIcons()
        {
            bool cpuChecked = CpuTrayEnable.Checked;
            bool gpuChecked = GpuTrayEnable.Checked;

            cpuTrayIcon.Visible = cpuChecked;
            gpuTrayIcon.Visible = gpuChecked;
            NotifyIcon.Visible = !this.Visible && !cpuChecked && !gpuChecked;
        }

        private void UpdateTemperatures(float? cpuTemp, float? gpuTemp)
        {
            if (cpuTemp.HasValue)
            {
                float currentCpuTemp = cpuTemp.Value;
                _cpuMinTemp = Math.Min(_cpuMinTemp, currentCpuTemp);
                _cpuMaxTemp = Math.Max(_cpuMaxTemp, currentCpuTemp);

                CpuTemp.Text = $"{currentCpuTemp:F0}°C";
                CpuMin.Text = $"{_cpuMinTemp:F0}°C";
                CpuMax.Text = $"{_cpuMaxTemp:F0}°C";
            }
            else
            {
                CpuTemp.Text = "N/A";
                CpuMin.Text = "N/A";
                CpuMax.Text = "N/A";
            }

            if (gpuTemp.HasValue)
            {
                float currentGpuTemp = gpuTemp.Value;
                _gpuMinTemp = Math.Min(_gpuMinTemp, currentGpuTemp);
                _gpuMaxTemp = Math.Max(_gpuMaxTemp, currentGpuTemp);

                GpuTemp.Text = $"{currentGpuTemp:F0}°C";
                GpuMin.Text = $"{_gpuMinTemp:F0}°C";
                GpuMax.Text = $"{_gpuMaxTemp:F0}°C";
            }
            else
            {
                GpuTemp.Text = "N/A";
                GpuMin.Text = "N/A";
                GpuMax.Text = "N/A";
            }
        }

        private void UpdateAllTrayIcons(float? cpuTemp, float? gpuTemp)
        {
            if (CpuTrayEnable.Checked && cpuTemp.HasValue)
            {
                UpdateSingleTrayIcon(cpuTrayIcon, cpuTemp.Value, ref _lastCpuTempText, _cpuTrayColor);
            }
            if (GpuTrayEnable.Checked && gpuTemp.HasValue)
            {
                UpdateSingleTrayIcon(gpuTrayIcon, gpuTemp.Value, ref _lastGpuTempText, _gpuTrayColor);
            }

            if (!CpuTrayEnable.Checked && !GpuTrayEnable.Checked)
            {
                string cpuHover = cpuTemp.HasValue ? $"{cpuTemp.Value:F0}°C" : "N/A";
                string gpuHover = gpuTemp.HasValue ? $"{gpuTemp.Value:F0}°C" : "N/A";
                NotifyIcon.Text = $"CPU: {cpuHover} | GPU: {gpuHover}";
            }
            else
            {
                NotifyIcon.Text = AppName;
            }
        }

        private void UpdateSingleTrayIcon(NotifyIcon icon, float temp, ref string lastText, Color color)
        {
            string newText = $"{temp:F0}";
            if (newText != lastText)
            {
                Icon oldIcon = icon.Icon;
                icon.Icon = CreateTempIcon(newText, color, _trayFontSize, _trayFontFamily);
                oldIcon?.Dispose();
                lastText = newText;
            }
        }

        private Icon CreateTempIcon(string text, Color color, float baseFontSize, string fontFamily)
        {
            Size iconSize = SystemInformation.SmallIconSize;

            using (var bmp = new Bitmap(iconSize.Width, iconSize.Height))
            using (var g = Graphics.FromImage(bmp))
            {
                float dpiScale = g.DpiX / 96f;
                float scaledFontSize = baseFontSize * dpiScale;

                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
                g.Clear(Color.Transparent);

                using (var font = new Font(fontFamily, scaledFontSize, FontStyle.Bold, GraphicsUnit.Pixel))
                {
                    SizeF textSize = g.MeasureString(text, font);
                    float x = (iconSize.Width - textSize.Width) / 2;
                    float y = (iconSize.Height - textSize.Height) / 2 + (dpiScale * 0.5f);

                    using (var brush = new SolidBrush(color))
                    {
                        g.DrawString(text, font, brush, new PointF(x, y));
                    }

                    IntPtr hIcon = bmp.GetHicon();
                    Icon newIcon = (Icon)Icon.FromHandle(hIcon).Clone();
                    DestroyIcon(hIcon);
                    return newIcon;
                }
            }
        }

        #endregion

        #region Settings Management

        private void LoadSettings()
        {
            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(RegistryPath))
                {
                    if (key == null) return;

                    object GetValue(string name, object def) => key.GetValue(name, def);

                    CpuTrayEnable.Checked = Convert.ToBoolean(GetValue(nameof(CpuTrayEnable), 0));
                    GpuTrayEnable.Checked = Convert.ToBoolean(GetValue(nameof(GpuTrayEnable), 0));
                    autostartApp.Checked = Convert.ToBoolean(GetValue(nameof(autostartApp), 0));

                    CpuTrayColor.SelectedItem = GetValue(nameof(CpuTrayColor), CpuTrayColor.Text);
                    GpuTrayColor.SelectedItem = GetValue(nameof(GpuTrayColor), GpuTrayColor.Text);
                    FontFamilyTray.SelectedItem = GetValue(nameof(FontFamilyTray), FontFamilyTray.Text);

                    if (decimal.TryParse(GetValue(nameof(FontSizeTray), "12").ToString(), out decimal fontSizeValue))
                    {
                        if (fontSizeValue >= FontSizeTray.Minimum && fontSizeValue <= FontSizeTray.Maximum)
                            FontSizeTray.Value = fontSizeValue;
                    }

                    if (decimal.TryParse(GetValue(nameof(UpdateInterval), "0.50").ToString(), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out decimal intervalValue))
                    {
                        if (intervalValue >= UpdateInterval.Minimum && intervalValue <= UpdateInterval.Maximum)
                            UpdateInterval.Value = intervalValue;
                    }

                    _selectedCpuIdentifier = GetValue("SelectedCpuIdentifier", "").ToString();
                    _selectedGpuIdentifier = GetValue("SelectedGpuIdentifier", "").ToString();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading settings: {ex.Message}");
            }
            finally
            {
                CacheDisplaySettings();
                _settingsLoaded = true;
            }
        }

        private void SaveSetting(string name, object value)
        {
            if (!_settingsLoaded) return;
            try
            {
                using (RegistryKey key = Registry.CurrentUser.CreateSubKey(RegistryPath))
                {
                    if (value is bool b)
                        key.SetValue(name, b ? 1 : 0, RegistryValueKind.DWord);
                    else
                        key.SetValue(name, value, RegistryValueKind.String);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving setting '{name}': {ex.Message}");
            }
        }

        private void CacheDisplaySettings()
        {
            _cpuTrayColor = ColorTranslator.FromHtml(CpuTrayColor.Text);
            _gpuTrayColor = ColorTranslator.FromHtml(GpuTrayColor.Text);
            _trayFontFamily = FontFamilyTray.Text.Trim();
            _trayFontSize = (float)this.FontSizeTray.Value;
        }

        private void Setting_CheckedChanged(object sender, EventArgs e)
        {
            if (sender is CheckBox chk)
            {
                SaveSetting(chk.Name, chk.Checked);
                UpdateTrayIcons();
                if (chk.Name == nameof(CpuTrayEnable)) _lastCpuTempText = null;
                if (chk.Name == nameof(GpuTrayEnable)) _lastGpuTempText = null;
            }
        }

        private void Setting_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (sender is ComboBox cmb)
            {
                SaveSetting(cmb.Name, cmb.Text);
                CacheDisplaySettings();
                _lastCpuTempText = null;
                _lastGpuTempText = null;

                TempTimer_Tick(this, EventArgs.Empty);
            }
        }

        private void FontSizeTray_ValueChanged(object sender, EventArgs e)
        {
            decimal fontSize = this.FontSizeTray.Value;
            SaveSetting(nameof(FontSizeTray), fontSize.ToString());
            CacheDisplaySettings();
            _lastCpuTempText = null;
            _lastGpuTempText = null;
            TempTimer_Tick(this, EventArgs.Empty);
        }

        private void UpdateInterval_ValueChanged(object sender, EventArgs e)
        {
            decimal interval = this.UpdateInterval.Value;
            SaveSetting(nameof(UpdateInterval), interval.ToString("F2", System.Globalization.CultureInfo.InvariantCulture));
            UpdateTimerInterval();
        }

        private void UpdateTimerInterval()
        {
            decimal intervalSeconds = this.UpdateInterval.Value;
            _tempTimer.Interval = Math.Max(100, (int)(intervalSeconds * 1000));
        }

        #endregion

        #region Autostart & Cleanup

        private async void AutostartApp_CheckedChanged(object sender, EventArgs e)
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
                RevertCheckbox(autostartApp, false);
                return;
            }

            SaveSetting(nameof(autostartApp), true);

            await InstallAndRestartAsync();

            // If user canceled install (no install folder saved), revert checkbox
            string installFolder = Registry.GetValue(
                @"HKEY_CURRENT_USER\" + RegistryPath,
                "InstallFolder",
                null) as string;

            if (string.IsNullOrEmpty(installFolder))
                RevertCheckbox(autostartApp, false);
        }

        private async Task HandleAutostartDisable()
        {
            var result = MessageBox.Show(this, "Remove installed app, shortcut, and startup entry?\n\nYes = Remove all\nNo = Remove only startup entry\nCancel = Do nothing",
                                        "Confirm Remove", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
            switch (result)
            {
                case DialogResult.Yes:
                    SaveSetting(nameof(autostartApp), false);
                    UninstallAndExit(); // Această metodă va închide aplicația
                    break;
                case DialogResult.No:
                    SaveSetting(nameof(autostartApp), false);
                    await RemoveStartupTaskAsync();
                    MessageBox.Show(this, "Startup entry removed.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    break;
                case DialogResult.Cancel:
                    RevertCheckbox(autostartApp, true);
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
            string destFolder;

            using (var fbd = new FolderBrowserDialog())
            {
                fbd.Description = "Select installation folder for TrayTemps";
                fbd.ShowNewFolderButton = true;

                if (fbd.ShowDialog(this) != DialogResult.OK ||
                    string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    return Task.FromResult(0);
                }

                destFolder = Path.Combine(fbd.SelectedPath, AppName);
            }

            string destExe = Path.Combine(destFolder, "TrayTemps.exe");

            return Task.Run(() =>
            {
                Directory.CreateDirectory(destFolder);

                string sourceFolder = AppDomain.CurrentDomain.BaseDirectory;
                string[] filesToCopy =
                {
            "TrayTemps.exe",
            "HidSharp.dll",
            "LibreHardwareMonitorLib.dll"
        };

                foreach (string file in filesToCopy)
                {
                    string src = Path.Combine(sourceFolder, file);
                    string dst = Path.Combine(destFolder, file);

                    if (File.Exists(src))
                        File.Copy(src, dst, true);
                }

                Registry.SetValue(
                    @"HKEY_CURRENT_USER\" + RegistryPath,
                    "InstallFolder",
                    destFolder,
                    RegistryValueKind.String);

                string arguments =
                    "/Create /F /RL HIGHEST /SC ONLOGON /TN \"" + AppName +
                    "\" /TR \"\\\"" + destExe + "\\\" -silent\"";

                RunProcessAndWait("schtasks", arguments);
                CreateShortcutOnDesktop(destExe);

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
            });
        }

        private void UninstallAndExit()
        {
            string installFolder = Registry.GetValue(
                @"HKEY_CURRENT_USER\" + RegistryPath,
                "InstallFolder",
                null) as string;

            if (string.IsNullOrEmpty(installFolder) || !Directory.Exists(installFolder))
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
                AppName + ".lnk");

            string registryKeyPath = @"HKEY_CURRENT_USER\" + RegistryPath;
            string batPath = Path.Combine(Path.GetTempPath(), "DeleteTrayTemps.bat");

            string script =
        @"@echo off
setlocal
cd /d ""%~dp0""

schtasks /Delete /TN """ + AppName + @""" /F > nul 2>&1
reg delete """ + registryKeyPath + @""" /f > nul 2>&1

set ""install_folder=" + installFolder + @"""
set attempts=0

:loop
if %attempts% GEQ 15 goto cleanup
if not exist ""%install_folder%"" goto cleanup

rmdir /s /q ""%install_folder%""
if not exist ""%install_folder%"" goto cleanup

set /a attempts+=1
ping 127.0.0.1 -n 2 > nul
goto loop

:cleanup
if exist """ + shortcutPath + @""" del /f /q """ + shortcutPath + @"""

(goto) 2>nul & del ""%~f0""";

            File.WriteAllText(batPath, script);

            var psi = new ProcessStartInfo(batPath)
            {
                UseShellExecute = true,
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden
            };

            Process.Start(psi);

            ExecuteShutdownSequence();
            Close();
        }

        private Task RemoveStartupTaskAsync()
        {
            return Task.Run(() =>
                RunProcessAndWait("schtasks", "/Delete /TN \"" + AppName + "\" /F"));
        }

        #endregion

        #region Helper Methods

        private Task<string> GetMotherboardNameAsync()
        {
            return Task.Run(() =>
            {
                try
                {
                    string query = "select Manufacturer, Product from Win32_BaseBoard";
                    using (var searcher = new ManagementObjectSearcher(query))
                    {
                        var firstObj = searcher.Get().OfType<ManagementObject>().FirstOrDefault();
                        if (firstObj != null)
                        {
                            string manufacturer = firstObj["Manufacturer"]?.ToString().Trim() ?? "";
                            string product = firstObj["Product"]?.ToString().Trim() ?? "";
                            string fullName = $"{manufacturer} {product}".Trim();
                            return string.IsNullOrEmpty(fullName) ? "Unknown Motherboard" : fullName;
                        }
                    }
                }
                catch { return "Unknown Motherboard"; }
                return "Unknown Motherboard";
            });
        }

        private Task<string> GetRamInfoAsync()
        {
            return Task.Run(() =>
            {
                try
                {
                    string query = "select Capacity, SMBIOSMemoryType, ConfiguredClockSpeed from Win32_PhysicalMemory";
                    using (var searcher = new ManagementObjectSearcher(query))
                    {
                        var sticks = searcher.Get().OfType<ManagementObject>().ToList();
                        if (sticks.Count == 0) return "Unknown RAM";

                        long totalCapacityBytes = sticks.Sum(mo => Convert.ToInt64(mo["Capacity"]));
                        var individualCapacities = sticks.Select(mo => Convert.ToInt64(mo["Capacity"])).ToList();

                        uint memoryType = Convert.ToUInt32(sticks[0]["SMBIOSMemoryType"]);
                        uint speed = Convert.ToUInt32(sticks[0]["ConfiguredClockSpeed"]);

                        long totalCapacityGB = totalCapacityBytes / (1024 * 1024 * 1024);
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

        private Task<string> GetHardwareNameAsync(string wmiClass, string defaultName)
        {
            return Task.Run(() =>
            {
                try
                {
                    using (var searcher = new ManagementObjectSearcher($"select Name from {wmiClass}"))
                    {
                        var firstObj = searcher.Get().OfType<ManagementObject>().FirstOrDefault();
                        return firstObj?["Name"]?.ToString()?.Trim() ?? defaultName;
                    }
                }
                catch { return defaultName; }
            });
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
            var psi = new System.Diagnostics.ProcessStartInfo(fileName, arguments)
            {
                UseShellExecute = true,
                CreateNoWindow = true,
                WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden
            };
            try
            {
                System.Diagnostics.Process.Start(psi)?.WaitForExit();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to run process '{fileName}': {ex.Message}");
                MessageBox.Show($"An error occurred while running a required command:\n{ex.Message}", "Execution Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PopulateHardwareSelector(ComboBox selector, List<IHardware> hardwareList, string savedIdentifier, Label nameLabel, string hardwareType)
        {
            selector.Items.Clear();
            if (hardwareList != null && hardwareList.Any())
            {
                for (int i = 0; i < hardwareList.Count; i++)
                {
                    selector.Items.Add(i);
                }

                int initialIndex = -1;
                if (!string.IsNullOrEmpty(savedIdentifier))
                {
                    var hardwareToSelect = hardwareList.FirstOrDefault(h => h.Identifier.ToString() == savedIdentifier);
                    if (hardwareToSelect != null)
                    {
                        initialIndex = hardwareList.IndexOf(hardwareToSelect);
                    }
                }
                selector.SelectedIndex = (initialIndex != -1) ? initialIndex : 0;
                selector.Enabled = hardwareList.Count > 1;
            }
            else
            {
                nameLabel.Text = $"No {hardwareType} found";
                selector.Enabled = false;
            }
        }

        #endregion
    }
}