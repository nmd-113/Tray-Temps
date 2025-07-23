using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Management;
using LibreHardwareMonitor.Hardware;
using System.Drawing;
using System.Drawing.Text;
using Microsoft.Win32;
using System.Linq;
using System.IO;
using System.Threading.Tasks;

namespace TrayTemps
{
    public partial class TrayTemps : Form
    {
        // ----- Fields -----
        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HTCAPTION = 0x2;
        private const string RegistryPath = @"Software\TrayTemps";

        private Computer _computer;
        private readonly Timer TempTimer = new Timer();
        private float cpuMaxTemp = 0;
        private float gpuMaxTemp = 0;
        private string _lastCpuTempText;
        private string _lastGpuTempText;
        private bool isInternalCheckChange = false;
        private bool settingsLoaded = false;
        private bool _isUpdating = false; // Flag to prevent timer re-entrancy

        // ----- P/Invoke Imports -----
        [DllImport("user32.dll")]
        private static extern bool ReleaseCapture();
        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool DestroyIcon(IntPtr hIcon);

        // ----- Constructor & Form Events -----
        public TrayTemps()
        {
            InitializeComponent();
        }

        private async void TrayTemps_Load(object sender, EventArgs e)
        {
            // Set control defaults
            UpdateInterval.SelectedIndex = 2;
            CpuTrayColor.SelectedIndex = 0;
            GpuTrayColor.SelectedIndex = 4;
            FontSizeTray.SelectedIndex = 7;
            FontFamilyTray.SelectedIndex = 0;

            // Load settings and hardware info asynchronously
            LoadSettings();
            await LoadHardwareInfoAsync();
            await InitializeHardwareMonitorAsync();

            // Configure and start the timer after all initialization
            if (double.TryParse(UpdateInterval.Text, out double seconds))
            {
                TempTimer.Interval = (int)(seconds * 1000);
            }
            else
            {
                TempTimer.Interval = 2000; // Default to 2 seconds
            }
            TempTimer.Tick += TempTimer_Tick;
            TempTimer.Start();

            // Trigger an initial update
            UpdateTrayIcons();
            TempTimer_Tick(this, EventArgs.Empty);
        }

        private void TrayTemps_FormClosing(object sender, FormClosingEventArgs e)
        {
            TempTimer.Stop();
            TempTimer.Dispose();
            _computer?.Close();
            cpuTrayIcon?.Dispose();
            gpuTrayIcon?.Dispose();
            NotifyIcon?.Dispose();
        }

        // ----- UI Event Handlers -----
        private void exit_Click(object sender, EventArgs e) => Application.Exit();

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

        private void NotifyIcon_MouseDoubleClick(object sender, MouseEventArgs e) => NormalWindow();
        private void gpuTrayIcon_MouseDoubleClick(object sender, MouseEventArgs e) => NormalWindow();
        private void cpuTrayIcon_MouseDoubleClick(object sender, MouseEventArgs e) => NormalWindow();

        // ----- Core Logic -----
        private void NormalWindow()
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

            // Show main icon only if window is hidden and no other icons are active
            NotifyIcon.Visible = !this.Visible && !cpuChecked && !gpuChecked;
        }

        private async Task LoadHardwareInfoAsync()
        {
            CpuName.Text = await GetCpuNameAsync();
            GpuName.Text = await GetGpuNameAsync();
        }

        private Task<string> GetCpuNameAsync()
        {
            return Task.Run(() =>
            {
                using (var searcher = new ManagementObjectSearcher("select Name from Win32_Processor"))
                {
                    var firstObj = searcher.Get().OfType<ManagementObject>().FirstOrDefault();
                    return firstObj?["Name"]?.ToString() ?? "Unknown CPU";
                }
            });
        }

        private Task<string> GetGpuNameAsync()
        {
            return Task.Run(() =>
            {
                using (var searcher = new ManagementObjectSearcher("select Name from Win32_VideoController"))
                {
                    var firstObj = searcher.Get().OfType<ManagementObject>().FirstOrDefault();
                    return firstObj?["Name"]?.ToString() ?? "Unknown GPU";
                }
            });
        }

        private Task InitializeHardwareMonitorAsync()
        {
            return Task.Run(() =>
            {
                _computer = new Computer
                {
                    IsCpuEnabled = true,
                    IsGpuEnabled = true
                };
                _computer.Open();
            });
        }

        private async void TempTimer_Tick(object sender, EventArgs e)
        {
            if (_isUpdating) return; // Prevent overlapping updates
            _isUpdating = true;

            try
            {
                // Offload hardware polling to a background thread
                var (cpuTemp, gpuTemp) = await Task.Run(() =>
                {
                    foreach (var hardware in _computer.Hardware)
                        hardware.Update();

                    float? cTemp = _computer.Hardware.FirstOrDefault(h => h.HardwareType == HardwareType.Cpu)?
                                            .Sensors.FirstOrDefault(s => s.SensorType == SensorType.Temperature)?.Value;

                    float? gTemp = _computer.Hardware.FirstOrDefault(h => h.HardwareType == HardwareType.GpuAmd || h.HardwareType == HardwareType.GpuNvidia)?
                                            .Sensors.FirstOrDefault(s => s.SensorType == SensorType.Temperature)?.Value;

                    return (cTemp, gTemp);
                });

                // --- UI updates (back on the main thread) ---
                if (cpuTemp.HasValue && cpuTemp.Value > cpuMaxTemp) cpuMaxTemp = cpuTemp.Value;
                if (gpuTemp.HasValue && gpuTemp.Value > gpuMaxTemp) gpuMaxTemp = gpuTemp.Value;

                CpuTemp.Text = cpuTemp.HasValue ? $"{cpuTemp.Value:F0}°C" : "N/A";
                GpuTemp.Text = gpuTemp.HasValue ? $"{gpuTemp.Value:F0}°C" : "N/A";
                CpuMax.Text = $"{cpuMaxTemp:F0}°C";
                GpuMax.Text = $"{gpuMaxTemp:F0}°C";

                string fontFamily = FontFamilyTray.Text.Trim();
                float fontSize = float.Parse(FontSizeTray.Text);

                // Update CPU tray icon if needed
                if (CpuTrayEnable.Checked && cpuTemp.HasValue)
                {
                    string cpuText = $"{cpuTemp.Value:F0}";
                    if (cpuText != _lastCpuTempText)
                    {
                        Color color = ColorTranslator.FromHtml(CpuTrayColor.Text);
                        Icon newIcon = CreateTempIcon(cpuText, color, fontSize, fontFamily);
                        cpuTrayIcon.Icon?.Dispose();
                        cpuTrayIcon.Icon = newIcon;
                        _lastCpuTempText = cpuText;
                    }
                }

                // Update GPU tray icon if needed
                if (GpuTrayEnable.Checked && gpuTemp.HasValue)
                {
                    string gpuText = $"{gpuTemp.Value:F0}";
                    if (gpuText != _lastGpuTempText)
                    {
                        Color color = ColorTranslator.FromHtml(GpuTrayColor.Text);
                        Icon newIcon = CreateTempIcon(gpuText, color, fontSize, fontFamily);
                        gpuTrayIcon.Icon?.Dispose();
                        gpuTrayIcon.Icon = newIcon;
                        _lastGpuTempText = gpuText;
                    }
                }

                // Update hover text for the main icon
                if (!CpuTrayEnable.Checked && !GpuTrayEnable.Checked)
                {
                    string cpuHoverText = cpuTemp.HasValue ? $"{cpuTemp.Value:F0}°C" : "N/A";
                    string gpuHoverText = gpuTemp.HasValue ? $"{gpuTemp.Value:F0}°C" : "N/A";
                    NotifyIcon.Text = $"CPU: {cpuHoverText} | GPU: {gpuHoverText}";
                }
                else
                {
                    NotifyIcon.Text = "TrayTemps";
                }
            }
            finally
            {
                _isUpdating = false; // Release the lock
            }
        }

        private Icon CreateTempIcon(string text, Color color, float fontSize, string fontFamily)
        {
            const int iconSize = 16;
            using (var bmp = new Bitmap(iconSize, iconSize))
            using (var g = Graphics.FromImage(bmp))
            {
                // Fallback font if the selected one isn't available
                using (var installedFonts = new InstalledFontCollection())
                {
                    if (!installedFonts.Families.Any(f => f.Name.Equals(fontFamily, StringComparison.OrdinalIgnoreCase)))
                        fontFamily = "Tahoma";
                }

                using (var font = new Font(fontFamily, fontSize, FontStyle.Bold, GraphicsUnit.Pixel))
                {
                    g.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;
                    g.Clear(Color.Transparent);

                    Size textSize = TextRenderer.MeasureText(g, text, font, new Size(iconSize, iconSize), TextFormatFlags.NoPadding);
                    int x = (iconSize - textSize.Width) / 2;
                    int y = (iconSize - textSize.Height) / 2;

                    TextRenderer.DrawText(g, text, font, new Point(x, y), color, TextFormatFlags.NoPadding | TextFormatFlags.NoClipping);

                    IntPtr hIcon = bmp.GetHicon();
                    Icon icon = (Icon)Icon.FromHandle(hIcon).Clone();
                    DestroyIcon(hIcon); // Important: release the unmanaged handle
                    return icon;
                }
            }
        }

        // ----- Settings and Autostart -----
        private void LoadSettings()
        {
            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(RegistryPath))
                {
                    if (key == null) return;

                    // Simple helper to load values
                    object GetValue(string name, object defaultValue) => key.GetValue(name, defaultValue);

                    CpuTrayEnable.Checked = (int)GetValue(nameof(CpuTrayEnable), 0) == 1;
                    GpuTrayEnable.Checked = (int)GetValue(nameof(GpuTrayEnable), 0) == 1;
                    autostartApp.Checked = (int)GetValue(nameof(autostartApp), 0) == 1;

                    CpuTrayColor.SelectedItem = GetValue(nameof(CpuTrayColor), CpuTrayColor.Text) as string;
                    GpuTrayColor.SelectedItem = GetValue(nameof(GpuTrayColor), GpuTrayColor.Text) as string;
                    FontSizeTray.SelectedItem = GetValue(nameof(FontSizeTray), FontSizeTray.Text) as string;
                    FontFamilyTray.SelectedItem = GetValue(nameof(FontFamilyTray), FontFamilyTray.Text) as string;
                    UpdateInterval.SelectedItem = GetValue(nameof(UpdateInterval), UpdateInterval.Text) as string;
                }
            }
            catch (Exception ex) { Console.WriteLine($"Error loading settings: {ex.Message}"); }
            finally { settingsLoaded = true; }
        }

        private void SaveSetting(string name, object value)
        {
            try
            {
                using (RegistryKey key = Registry.CurrentUser.CreateSubKey(RegistryPath))
                {
                    if (value is bool b)
                        key.SetValue(name, b ? 1 : 0, RegistryValueKind.DWord);
                    else
                        key.SetValue(name, value.ToString(), RegistryValueKind.String);
                }
            }
            catch (Exception ex) { Console.WriteLine($"Error saving setting: {ex.Message}"); }
        }

        // --- Event handlers for settings changes ---
        private void CpuTrayEnable_CheckedChanged(object sender, EventArgs e)
        {
            if (!settingsLoaded) return;
            SaveSetting(nameof(CpuTrayEnable), CpuTrayEnable.Checked);
            UpdateTrayIcons();
            _lastCpuTempText = null; // Force redraw
        }

        private void GpuTrayEnable_CheckedChanged(object sender, EventArgs e)
        {
            if (!settingsLoaded) return;
            SaveSetting(nameof(GpuTrayEnable), GpuTrayEnable.Checked);
            UpdateTrayIcons();
            _lastGpuTempText = null; // Force redraw
        }

        private void CpuTrayColor_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!settingsLoaded) return;
            SaveSetting(nameof(CpuTrayColor), CpuTrayColor.Text);
            _lastCpuTempText = null;
        }

        private void GpuTrayColor_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!settingsLoaded) return;
            SaveSetting(nameof(GpuTrayColor), GpuTrayColor.Text);
            _lastGpuTempText = null;
        }

        private void FontSizeTray_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!settingsLoaded) return;
            SaveSetting(nameof(FontSizeTray), FontSizeTray.Text);
            _lastCpuTempText = null;
            _lastGpuTempText = null;
        }

        private void FontFamilyTray_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!settingsLoaded) return;
            SaveSetting(nameof(FontFamilyTray), FontFamilyTray.Text);
            _lastCpuTempText = null;
            _lastGpuTempText = null;
        }

        private void UpdateInterval_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!settingsLoaded) return;
            if (double.TryParse(UpdateInterval.Text, out double seconds))
            {
                TempTimer.Interval = (int)(seconds * 1000);
                SaveSetting(nameof(UpdateInterval), UpdateInterval.Text);
            }
        }

        private async void autostartApp_CheckedChanged(object sender, EventArgs e)
        {
            if (!settingsLoaded || isInternalCheckChange) return;

            var control = (CheckBox)sender;
            control.Enabled = false; // Disable control during operation

            if (autostartApp.Checked)
            {
                var result = MessageBox.Show("Add app to run silently at Windows startup?", "Startup", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    SaveSetting(nameof(autostartApp), true);
                    await AutostartAppAsync(); // Will handle restart and exit
                }
                else
                {
                    // User cancelled, revert checkbox state
                    isInternalCheckChange = true;
                    autostartApp.Checked = false;
                    isInternalCheckChange = false;
                }
            }
            else
            {
                var result = MessageBox.Show("Remove installed app, shortcut, and startup entry?\n\nYes = Remove all\nNo = Remove only startup entry\nCancel = Do nothing",
                                             "Confirm Remove", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    SaveSetting(nameof(autostartApp), false);
                    await SelfDeleteAndCleanupAsync(); // Will handle cleanup and exit
                }
                else if (result == DialogResult.No)
                {
                    SaveSetting(nameof(autostartApp), false);
                    await RemoveStartupEntryOnlyAsync();
                }
                else
                {
                    // User cancelled, revert checkbox state
                    isInternalCheckChange = true;
                    autostartApp.Checked = true;
                    isInternalCheckChange = false;
                }
            }

            if (control.IsHandleCreated) control.Enabled = true; // Re-enable control
        }

        private async Task AutostartAppAsync()
        {
            MessageBox.Show("TrayTemps will now be installed and restarted from the new location.", "Installation", MessageBoxButtons.OK, MessageBoxIcon.Information);

            await Task.Run(() =>
            {
                string sourceFolder = AppDomain.CurrentDomain.BaseDirectory;
                string destFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "TrayTemps");
                Directory.CreateDirectory(destFolder);

                string[] filesToCopy = { "TrayTemps.exe", "HidSharp.dll", "LibreHardwareMonitorLib.dll" /* Add others if necessary */ };
                foreach (var file in filesToCopy)
                {
                    string src = Path.Combine(sourceFolder, file);
                    string dst = Path.Combine(destFolder, file);
                    if (File.Exists(src)) File.Copy(src, dst, true);
                }

                string destExe = Path.Combine(destFolder, "TrayTemps.exe");
                var psi = new System.Diagnostics.ProcessStartInfo("schtasks", $"/Create /F /RL HIGHEST /SC ONLOGON /TN TrayTemps /TR \"\\\"{destExe}\\\" -silent\"")
                {
                    Verb = "runas",
                    UseShellExecute = true,
                    CreateNoWindow = true
                };
                System.Diagnostics.Process.Start(psi)?.WaitForExit();

                CreateShortcutOnDesktop(destExe);
            });

            // Start new process and exit
            string newExePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "TrayTemps", "TrayTemps.exe");
            System.Diagnostics.Process.Start(newExePath);
            Application.Exit();
        }

        private void CreateShortcutOnDesktop(string targetPath)
        {
            string deskPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            string shortcutPath = Path.Combine(deskPath, "TrayTemps.lnk");

            Type shellType = Type.GetTypeFromProgID("WScript.Shell");
            dynamic shell = Activator.CreateInstance(shellType);
            dynamic shortcut = shell.CreateShortcut(shortcutPath);
            shortcut.TargetPath = targetPath;
            shortcut.WorkingDirectory = Path.GetDirectoryName(targetPath);
            shortcut.Description = "TrayTemps";
            shortcut.Save();
        }

        private Task RemoveStartupEntryOnlyAsync()
        {
            return Task.Run(() =>
            {
                var psiDeleteTask = new System.Diagnostics.ProcessStartInfo("schtasks", "/Delete /TN TrayTemps /F")
                {
                    Verb = "runas",
                    UseShellExecute = true,
                    CreateNoWindow = true
                };
                System.Diagnostics.Process.Start(psiDeleteTask)?.WaitForExit();
                MessageBox.Show("Startup entry removed.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            });
        }

        private async Task SelfDeleteAndCleanupAsync()
        {
            await Task.Run(() =>
            {
                // Remove Startup Task
                var psiDeleteTask = new System.Diagnostics.ProcessStartInfo("schtasks", "/Delete /TN TrayTemps /F") { Verb = "runas", CreateNoWindow = true };
                System.Diagnostics.Process.Start(psiDeleteTask)?.WaitForExit();

                // Remove Registry Settings
                Registry.CurrentUser.DeleteSubKeyTree(RegistryPath, false);

                // Create a batch file for self-deletion
                string installFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "TrayTemps");
                string shortcutPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "TrayTemps.lnk");
                string batPath = Path.Combine(Path.GetTempPath(), "DeleteTrayTemps.bat");

                using (var sw = new StreamWriter(batPath, false))
                {
                    sw.WriteLine("@echo off");
                    sw.WriteLine("timeout /t 2 /nobreak >nul"); // Wait for main app to close
                    if (File.Exists(shortcutPath)) sw.WriteLine($"del /f /q \"{shortcutPath}\"");
                    if (Directory.Exists(installFolder)) sw.WriteLine($"rmdir /s /q \"{installFolder}\"");
                    sw.WriteLine($"(goto) 2>nul & del \"%~f0\""); // Delete self
                }

                // Run the batch file and exit
                var psiRunBat = new System.Diagnostics.ProcessStartInfo("cmd.exe", $"/c \"{batPath}\"")
                {
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden
                };
                System.Diagnostics.Process.Start(psiRunBat);
            });
            Application.Exit();
        }
    }
}