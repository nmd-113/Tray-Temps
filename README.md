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
* **High-Quality Icon Rendering:** Uses Anti-Aliasing and specialized font rendering (Consolas) for maximum legibility in the system tray.
* **Fully Customizable Visuals:**
    * Support for custom ARGB colors via a standard color picker.
    * Adjustable temperature thresholds for color transitions.
    * Options for separate or combined tray icons.
* **Configurable Update Interval:** Set how frequently the temperatures refresh.
* **Minimalist Design:** Low resource usage and a clean UI that stays out of your way.
* **Optional Autostart & Silent Mode:** Seamlessly integrate TrayTemps into your Windows startup.
* **Persistent Settings:** Your preferences, colors, and thresholds are saved and loaded automatically.

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
2.  Download the `TrayTemps.zip` file from the latest release.
3.  Extract the contents to a folder of your choice.
4.  Run `TrayTemps.exe`.

**For Autostart:**
Enable "**Autostart with Windows**" in the Settings tab. This creates a scheduled task for silent startup and an optional desktop shortcut.

---

## ⚙️ Usage & Configuration

Once launched, TrayTemps resides in your system tray. 

### Customizing Colors
You can now choose between two coloring modes:
1. **Static Mode:** Click the color box next to CPU/GPU settings to choose a fixed color.
2. **Dynamic Mode:** Enable "Temperature-Based Coloring" to allow icons to transition between three colors (Normal, Warm, Critical) based on the temperature ranges you define in the configuration menu.

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
