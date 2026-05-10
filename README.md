# TrayTemps

[![GitHub license](https://img.shields.io/github/license/nmd-113/Tray-Temps?style=flat-square)](LICENSE)
[![GitHub stars](https://img.shields.io/github/stars/nmd-113/Tray-Temps?style=flat-square)](https://github.com/nmd-113/Tray-Temps/stargazers)
[![GitHub forks](https://img.shields.io/github/forks/nmd-113/Tray-Temps?style=flat-square)](https://github.com/nmd-113/Tray-Temps/network/members)
[![GitHub release (latest by date)](https://img.shields.io/github/v/release/nmd-113/Tray-Temps?style=flat-square)](https://github.com/nmd-113/Tray-Temps/releases/latest)

A lightweight and customizable Windows utility that displays your **CPU and GPU temperatures** directly in your system's **notification area (system tray)**. Keep an eye on your hardware's health at a glance without cluttering your desktop!

---

## ✨ Features

* **Real-time Temperature Monitoring:** Accurate readings for CPU and GPU powered by [LibreHardwareMonitor](https://github.com/LibreHardwareMonitor/LibreHardwareMonitor).
* **Dynamic Temperature Coloring:** Automatically change icon colors based on temperature thresholds (Normal, Warm, and Critical).
* **Combined Tray Mode:** Display both CPU and GPU temperatures in a single, space-saving tray icon optimized with the `Consolas` font for perfect alignment.
* **High-Quality Icon Rendering:** Uses Anti-Aliasing and high-quality rendering modes to ensure numbers are crisp and readable.
* **Fully Customizable Visuals:**
    * **Static Mode:** Choose any custom ARGB color for each sensor.
    * **Dynamic Mode:** Configure specific temperature ranges and colors for color-coded alerts.
    * **Font Control:** Support for any system font in standard mode.
* **Configurable Update Interval:** Set how frequently the temperatures refresh.
* **High-DPI Support:** Properly scales icons for 1080p, 1440p, and 4K displays.
* **Minimalist Design:** Low resource usage (<30MB RAM) and a clean UI.
* **Optional Autostart:** Integrated Windows Task Scheduler setup for silent launch on startup.

---

## 📸 Screenshots

**Main Window:**
![TrayTemps Main Window](https://naetech.ro/wp-content/uploads/2024/traytemps/traytemps.jpg?v2)

**CPU & GPU in Tray:**
![TrayTemps CPU GPU Tray](https://naetech.ro/wp-content/uploads/2024/traytemps/traytemps-tray.jpg)

---

## 🚀 Getting Started

### Prerequisites

* Windows 10 or newer.
* .NET Framework 4.8 or later.

### Installation

1.  Go to the [Releases page](https://github.com/nmd-113/Tray-Temps/releases).
2.  Download the `TrayTemps.exe` file from the latest release.
3.  Run `TrayTemps.exe`.

**For Autostart:**
Enable "**Autostart with Windows**" in the Settings tab. This creates a scheduled task for silent startup (bypassing UAC prompts) and an optional desktop shortcut.

---

## ⚙️ Usage & Configuration

Once launched, TrayTemps resides in your system tray. 

### Customizing Colors
* **Individual Icons:** In standard mode, click the color boxes to open a full ARGB color dialog.
* **Dynamic Mode:** Check the "Temperature-Based Coloring" box to enable automatic color transitions based on your custom thresholds.
* **Combined Mode:** Enable the combined icon to view both temperatures in one icon slot. This mode uses a fixed `Consolas` layout for better legibility.

### Controls
* **Double-click** any TrayTemps icon to open the main dashboard.
* **Right-click** the main icon for a quick exit or access to settings.
* **Update Interval:** Adjust the slider to change the polling frequency (in seconds).

---

## 🤝 Contributing

Contributions are welcome! If you have suggestions, bug reports, or want to contribute code, please feel free to:

1.  **Open an issue** for bugs or feature requests.
2.  **Fork the repository** and create a pull request.

---

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

## 🙏 Acknowledgements

* [LibreHardwareMonitor](https://github.com/LibreHardwareMonitor/LibreHardwareMonitor) for providing a robust library for hardware monitoring.
