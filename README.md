# TrayTemps

[![GitHub license](https://img.shields.io/github/license/nmd-113/Tray-Temps?style=flat-square)](LICENSE)
[![GitHub stars](https://img.shields.io/github/stars/nmd-113/Tray-Temps?style=flat-square)](https://github.com/nmd-113/Tray-Temps/stargazers)
[![GitHub forks](https://img.shields.io/github/forks/nmd-113/Tray-Temps?style=flat-square)](https://github.com/nmd-113/Tray-Temps/network/members)
[![GitHub release (latest by date)](https://img.shields.io/github/v/release/nmd-113/Tray-Temps?style=flat-square)](https://github.com/nmd-113/Tray-Temps/releases/latest)

A lightweight and customizable Windows utility that displays your **CPU and GPU temperatures** directly in your system's **notification area (system tray)**. Keep an eye on your hardware's health at a glance without cluttering your desktop!

---

## ✨ Features

* **Real-time Temperature Monitoring:** Accurate readings for CPU and GPU powered by [LibreHardwareMonitor](https://github.com/LibreHardwareMonitor/LibreHardwareMonitor).
* **Hardware Diagnostics:** Detailed component information (CPU architecture, GPU driver version, RAM part numbers, etc.) and live sensor feeds.
* **Dynamic Temperature Coloring:** Automatically change icon colors based on temperature thresholds (Normal, Warm, and Critical).
* **Combined Tray Mode:** Display both CPU and GPU temperatures in a single, space-saving tray icon optimized with the `Consolas` font.
* **High-Quality Icon Rendering:** Uses Anti-Aliasing and high-quality rendering modes to ensure numbers are crisp and readable.
* **Fully Customizable Visuals:**
    * **Static Mode:** Choose any custom ARGB color for each sensor.
    * **Dynamic Mode:** Configure specific temperature ranges and colors for alerts.
* **Configurable Update Interval:** Set how frequently the temperatures refresh.
* **High-DPI Support:** Properly scales icons for 1080p, 1440p, and 480K displays.
* **Minimalist Design:** Low resource usage and a clean UI.
* **Optional Autostart:** Integrated Windows Task Scheduler setup for silent launch on startup.

---

## 📸 Screenshots

**Main Window:**
![TrayTemps Main Window](https://naetech.ro/wp-content/uploads/2024/traytemps/traytemps.jpg?v2.0.1)

**CPU & GPU in Tray:**
![TrayTemps CPU GPU Tray](https://naetech.ro/wp-content/uploads/2024/traytemps/traytemps-tray.jpg)

---

## 🚀 Getting Started

### Prerequisites

* Windows 10 or newer.
* .NET Framework 4.8 or later.

### Installation

1. Go to the [Releases page](https://github.com/nmd-113/Tray-Temps/releases).
2. Download the `TrayTemps.exe` file from the latest release.
3. Run `TrayTemps.exe`.

### ⚠️ Note on Antivirus Flags (WinRing0)
TrayTemps utilizes the **WinRing0** driver (via LibreHardwareMonitor) to access low-level hardware sensors. Because this driver provides direct access to hardware registers, Windows Defender or other security software may flag it as a "potentially unwanted application" or a "driver threat."

**This is a false positive.** The driver is a standard component used by many open-source monitoring tools and is completely safe. If your antivirus prevents the app from reading temperatures, you may need to add an exclusion for `TrayTemps.exe`.

---

## ⚙️ Usage & Configuration

Once launched, TrayTemps resides in your system tray. 

### Advanced Diagnostics
Click on the hardware names (e.g., "CPU", "GPU") in the main window to open a **Detailed Hardware** view. This shows live sensor data, including clock speeds, voltages, and specific memory information.

### Customizing Colors
* **Individual Icons:** Click color boxes to open a full ARGB color dialog.
* **Dynamic Mode:** Enable "Temperature-Based Coloring" for automatic transitions based on your thresholds.
* **Combined Mode:** Enable the combined icon to view both temperatures in one slot.

### Controls
* **Double-click** any TrayTemps icon to open the main dashboard.
* **Right-click** for a quick exit or access to settings.
* **Update Interval:** Adjust the slider to change the polling frequency.

---

## 🤝 Contributing

Contributions are welcome! If you have suggestions, bug reports, or want to contribute code, please feel free to:

1. **Open an issue** for bugs or feature requests.
2. **Fork the repository** and create a pull request.

---

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

## 🙏 Acknowledgements

* [LibreHardwareMonitor](https://github.com/LibreHardwareMonitor/LibreHardwareMonitor) for providing a robust library for hardware monitoring.
