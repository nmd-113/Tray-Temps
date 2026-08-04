public class AppSettings
{
    public bool Autostart { get; set; } = false;
    public bool LightMode { get; set; } = false;
    public bool TempsFahrenheit { get; set; } = false;
    public bool StartMinimizedToTray { get; set; } = false;
    public bool StartMinimizedWithAdminRights { get; set; } = true;
    public bool SingleIconTray { get; set; } = false;
    public bool CpuTrayIcon { get; set; } = false;
    public bool GpuTrayIcon { get; set; } = false;
    public bool TempBasedIconColor { get; set; } = false;
    public bool TemperatureAlertsEnabled { get; set; } = false;
    public bool ShowTemperatureColorCorners { get; set; } = true;
    public decimal UpdateInterval { get; set; } = 0.50M;
    public int FontFamily { get; set; } = 0;
    public string TrayFontFamily { get; set; } = "";
    public int CpuColor { get; set; }
    public int GpuColor { get; set; }
    public int IconSize { get; set; } = 90;
    public int CpuIndex { get; set; } = 0;
    public int GpuIndex { get; set; } = 0;
    public int StorageIndex { get; set; } = 0;
    // Hardware order can change between launches. Keep the index for legacy
    // settings, but prefer these stable LibreHardwareMonitor identifiers.
    public string CpuIdentifier { get; set; } = "";
    public string GpuIdentifier { get; set; } = "";
    public string CpuTemperatureSensorIdentifier { get; set; } = "";
    public string GpuTemperatureSensorIdentifier { get; set; } = "";
    public string StorageIdentifier { get; set; } = "";
    public int MinWarmTemp { get; set; } = 60;
    public int MaxWarmTemp { get; set; } = 80;
    public string InstallFolder { get; set; } = "";
    public int WindowWidth { get; set; } = 0;
    public int WindowHeight { get; set; } = 0;
    public int WindowX { get; set; } = -1;
    public int WindowY { get; set; } = -1;
    public int HardwareDialogWidth { get; set; } = 0;
    public int HardwareDialogHeight { get; set; } = 0;
    public int HardwareDialogX { get; set; } = -1;
    public int HardwareDialogY { get; set; } = -1;
    public int NormalTempColor { get; set; }
    public int WarmTempColor { get; set; }
    public int HotTempColor { get; set; }

}
