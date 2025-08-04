using LibreHardwareMonitor.Hardware;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Threading.Tasks;
using System.Windows.Forms;

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
        private IHardware _cpuHardware;
        private IHardware _gpuHardware;
        private ISensor _cpuTempSensor;
        private ISensor _gpuTempSensor;
        private readonly Timer _tempTimer = new Timer();
        private float _cpuMaxTemp = float.MinValue;
        private float _cpuMinTemp = float.MaxValue;
        private float _gpuMaxTemp = float.MinValue;
        private float _gpuMinTemp = float.MaxValue;

        private string _lastCpuTempText;
        private string _lastGpuTempText;
        private bool _settingsLoaded = false;
        private bool _isUpdating = false;
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
            SetDefaultControlValues();
            LoadSettings();
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


        private void exit_Click(object sender, EventArgs e)
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

        private void minimize_Click(object sender, EventArgs e)
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

        // Compatibility wrappers for Designer
        private void NotifyIcon_MouseDoubleClick(object sender, MouseEventArgs e) => TrayIcon_MouseDoubleClick(sender, e);
        private void cpuTrayIcon_MouseDoubleClick(object sender, MouseEventArgs e) => TrayIcon_MouseDoubleClick(sender, e);
        private void gpuTrayIcon_MouseDoubleClick(object sender, MouseEventArgs e) => TrayIcon_MouseDoubleClick(sender, e);
        private void CpuTrayEnable_CheckedChanged(object sender, EventArgs e) => Setting_CheckedChanged(sender, e);
        private void GpuTrayEnable_CheckedChanged(object sender, EventArgs e) => Setting_CheckedChanged(sender, e);
        private void CpuTrayColor_SelectedIndexChanged(object sender, EventArgs e) => Setting_SelectedIndexChanged(sender, e);
        private void GpuTrayColor_SelectedIndexChanged(object sender, EventArgs e) => Setting_SelectedIndexChanged(sender, e);
        private void FontSizeTray_SelectedIndexChanged(object sender, EventArgs e) => Setting_SelectedIndexChanged(sender, e);
        private void FontFamilyTray_SelectedIndexChanged(object sender, EventArgs e) => Setting_SelectedIndexChanged(sender, e);

        #endregion

        #region Core Logic

        private void SetDefaultControlValues()
        {
            UpdateInterval.SelectedIndex = 2;
            CpuTrayColor.SelectedIndex = 0;
            GpuTrayColor.SelectedIndex = 4;
            FontSizeTray.SelectedIndex = 7;
            FontFamilyTray.SelectedIndex = 0;
        }

        private async Task InitializeHardwareAsync()
        {
            var cpuNameTask = GetHardwareNameAsync("Win32_Processor", "Unknown CPU");
            var gpuNameTask = GetHardwareNameAsync("Win32_VideoController", "Unknown GPU");
            var mboNameTask = GetMotherboardNameAsync();
            var ramInfoTask = GetRamInfoAsync();

            CpuName.Text = await cpuNameTask;
            GpuName.Text = await gpuNameTask;
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

                _cpuHardware = _computer.Hardware.FirstOrDefault(h => h.HardwareType == HardwareType.Cpu);
                _gpuHardware = _computer.Hardware.FirstOrDefault(h => h.HardwareType == HardwareType.GpuAmd || h.HardwareType == HardwareType.GpuNvidia);

                _cpuTempSensor = _cpuHardware?.Sensors.FirstOrDefault(s => s.SensorType == SensorType.Temperature && s.Name.Contains("Package"))
                                 ?? _cpuHardware?.Sensors.FirstOrDefault(s => s.SensorType == SensorType.Temperature);

                _gpuTempSensor = _gpuHardware?.Sensors.FirstOrDefault(s => s.SensorType == SensorType.Temperature && s.Name.Contains("Core"))
                                 ?? _gpuHardware?.Sensors.FirstOrDefault(s => s.SensorType == SensorType.Temperature);
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
            if (_isUpdating) return;
            _isUpdating = true;

            try
            {
                (float? cpuTemp, float? gpuTemp) = await Task.Run(() =>
                {
                    _cpuHardware?.Update();
                    _gpuHardware?.Update();
                    return (_cpuTempSensor?.Value, _gpuTempSensor?.Value);
                });

                UpdateTemperatures(cpuTemp, gpuTemp);
                UpdateAllTrayIcons(cpuTemp, gpuTemp);
            }
            finally
            {
                _isUpdating = false;
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

        private Icon CreateTempIcon(string text, Color color, float fontSize, string fontFamily)
        {
            const int IconSize = 16;

            using (var bmp = new Bitmap(IconSize, IconSize, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
            using (var g = Graphics.FromImage(bmp))
            {
                float dpiScale = g.DpiX / 96f;
                float scaledFontSize = fontSize * dpiScale;

                using (var font = new Font(fontFamily, scaledFontSize, FontStyle.Bold, GraphicsUnit.Pixel))
                {
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                    g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
                    g.Clear(Color.Transparent);

                    var textSize = g.MeasureString(text, font);

                    float x = (IconSize - textSize.Width) / 2f;
                    float y = (IconSize - textSize.Height) / 2f;

                    using (var brush = new SolidBrush(color))
                    {
                        g.DrawString(text, font, brush, x, y);
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
                    FontSizeTray.SelectedItem = GetValue(nameof(FontSizeTray), FontSizeTray.Text);
                    FontFamilyTray.SelectedItem = GetValue(nameof(FontFamilyTray), FontFamilyTray.Text);
                    UpdateInterval.SelectedItem = GetValue(nameof(UpdateInterval), UpdateInterval.Text);
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
            float.TryParse(FontSizeTray.Text, out _trayFontSize);
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

        private void UpdateInterval_SelectedIndexChanged(object sender, EventArgs e)
        {
            SaveSetting(nameof(UpdateInterval), UpdateInterval.Text);
            UpdateTimerInterval();
        }

        private void UpdateTimerInterval()
        {
            if (double.TryParse(UpdateInterval.Text, out double seconds))
            {
                _tempTimer.Interval = Math.Max(500, (int)(seconds * 1000));
            }
        }

        #endregion

        #region Autostart & Cleanup

        private async void autostartApp_CheckedChanged(object sender, EventArgs e)
        {
            if (!_settingsLoaded || _isInternalCheckChange) return;

            var control = (CheckBox)sender;
            control.Enabled = false;

            if (control.Checked)
                await HandleAutostartEnable();
            else
                await HandleAutostartDisable();

            if (control.IsHandleCreated) control.Enabled = true;
        }

        private async Task HandleAutostartEnable()
        {
            var result = MessageBox.Show(this,"Add app to run silently at Windows startup?", "Startup", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                SaveSetting(nameof(autostartApp), true);
                await InstallAndRestartAsync();
            }
            else
            {
                RevertCheckbox(autostartApp, false);
            }
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
            string destFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), AppName);
            string destExe = Path.Combine(destFolder, "TrayTemps.exe");

            return Task.Run(() =>
            {
                Directory.CreateDirectory(destFolder);
                string sourceFolder = AppDomain.CurrentDomain.BaseDirectory;
                string[] filesToCopy = { "TrayTemps.exe", "HidSharp.dll", "LibreHardwareMonitorLib.dll" };
                foreach (var file in filesToCopy.Where(f => File.Exists(Path.Combine(sourceFolder, f))))
                {
                    File.Copy(Path.Combine(sourceFolder, file), Path.Combine(destFolder, file), true);
                }

                string arguments = $"/Create /F /RL HIGHEST /SC ONLOGON /TN {AppName} /TR \"\\\"{destExe}\\\" -silent\"";
                RunProcessAndWait("schtasks", arguments);
                CreateShortcutOnDesktop(destExe);
                MessageBox.Show(this, "TrayTemps has been installed successfully. App will now close. Restart it from the desktop shortcut.", "Installation Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ExecuteShutdownSequence();
                this.Close();
            });
        }

        private void UninstallAndExit()
        {
            string installFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), AppName);
            string shortcutPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), $"{AppName}.lnk");
            string registryKeyPath = $@"HKEY_CURRENT_USER\{RegistryPath}";
            string batPath = Path.Combine(Path.GetTempPath(), "DeleteTrayTemps.bat");

            string script = $@"@echo off
                            setlocal
                            rem Change working directory to a neutral location to unlock the install folder.
                            cd /d ""%~dp0""

                            rem Delete system entries first.
                            schtasks /Delete /TN ""{AppName}"" /F > nul 2>&1
                            reg delete ""{registryKeyPath}"" /f > nul 2>&1

                            rem Begin loop to wait for the main app to close.
                            set ""install_folder={installFolder}""
                            set ""attempts=0""

                            :delete_loop
                            if %attempts% geq 15 goto cleanup
                            if not exist ""%install_folder%"" goto folder_deleted

                            rmdir /s /q ""%install_folder%""
                            if not exist ""%install_folder%"" goto folder_deleted

                            set /a attempts=attempts+1
                            ping 127.0.0.1 -n 2 > nul
                            goto delete_loop

                            :folder_deleted
                            rem Folder was deleted, now clean up the rest.

                            :cleanup
                            if exist ""{shortcutPath}"" (
                                del /f /q ""{shortcutPath}""
                            )

                            rem Self-delete the batch file.
                            (goto) 2>nul & del ""%~f0""
                            ";

            File.WriteAllText(batPath, script);

            var psi = new System.Diagnostics.ProcessStartInfo(batPath)
            {
                UseShellExecute = true,
                CreateNoWindow = true,
                WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden
            };
            System.Diagnostics.Process.Start(psi);
            ExecuteShutdownSequence();
            this.Close();
        }

        private Task RemoveStartupTaskAsync()
        {
            return Task.Run(() => RunProcessAndWait("schtasks", $"/Delete /TN {AppName} /F"));
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

        #endregion
    }
}