public class AppSettings
{
    public bool Autostart { get; set; } = false;
    public bool TempsFahrenheit { get; set; } = false;
    public bool SingleIconTray { get; set; } = false;
    public bool CpuTrayIcon { get; set; } = true;
    public bool GpuTrayIcon { get; set; } = false;
    public decimal UpdateInterval { get; set; } = 0.50M;
    public int FontFamily { get; set; } = 0;
    public int CpuColor { get; set; } = 0;
    public int GpuColor { get; set; } = 4;
    public int IconSize { get; set; } = 75;
    public int CpuIndex { get; set; } = 0;
    public int GpuIndex { get; set; } = 0;
    public int StorageIndex { get; set; } = 0;
    public string InstallFolder { get; set; } = "";
    public int WindowWidth { get; set; } = 0;
    public int WindowHeight { get; set; } = 0;
    public int WindowX { get; set; } = -1;
    public int WindowY { get; set; } = -1;
}