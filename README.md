# TrayTemps

[![GitHub license](https://img.shields.io/github/license/nmd-113/Tray-Temps?style=flat-square)](LICENSE)
[![GitHub stars](https://img.shields.io/github/stars/nmd-113/Tray-Temps?style=flat-square)](https://github.com/nmd-113/Tray-Temps/stargazers)
[![GitHub forks](https://img.shields.io/github/forks/nmd-113/Tray-Temps?style=flat-square)](https://github.com/nmd-113/Tray-Temps/network/members)
[![GitHub release (latest by date)](https://img.shields.io/github/v/release/nmd-113/Tray-Temps?style=flat-square)](https://github.com/nmd-113/Tray-Temps/releases/latest)

A lightweight and customizable Windows utility that displays your **CPU and GPU temperatures** directly in your system's **notification area/system tray**.

TrayTemps helps you keep an eye on your hardware at a glance, without keeping a full monitoring window open.

---

## ✨ Features

* **Real-time Temperature Monitoring:** CPU and GPU temperature readings powered by [LibreHardwareMonitor](https://github.com/LibreHardwareMonitor/LibreHardwareMonitor).
* **CPU and GPU Tray Icons:** Display CPU and GPU temperatures directly in the system tray.
* **Combined Tray Mode:** Show both CPU and GPU temperatures in one compact tray icon.
* **Dynamic Temperature Coloring:** Automatically change tray icon colors based on configurable temperature thresholds.
* **Static Custom Colors:** Choose custom CPU/GPU tray icon colors when temperature-based coloring is disabled.
* **Non-Admin Fallback Handling:** TrayTemps now uses available fallback sensor data when possible, even without administrator rights.
* **Smarter Missing-Sensor Behavior:** CPU/GPU tray options are automatically disabled when no usable temperature sensor exists, avoiding fake or misleading readings.
* **Storage Detection Fallback:** If storage sensors are unavailable but Windows can still detect disks through WMI, detected disks are still shown in the main UI.
* **Hardware Diagnostics:** Detailed CPU, GPU, RAM, motherboard, BIOS, and storage information.
* **Live Sensor Details:** Open detailed live sensor views for supported hardware.
* **Storage Health / SMART Details:** Shows available storage health, lifetime, and SMART-related information when supported.
* **Customizable Update Interval:** Adjust how often tray temperatures refresh.
* **High-DPI Friendly Tray Rendering:** Tray icon rendering is optimized for clear text and stable display.
* **Optional Autostart:** Integrated Windows Task Scheduler setup for silent startup.
* **Light/Dark Theme Support:** Simple modern UI with theme-aware controls.

---

## 📸 Screenshots

**Main Window:**

![TrayTemps Main Window](https://naetech.ro/wp-content/uploads/2024/traytemps/traytemps.jpg?v2.0.2)

**CPU & GPU in Tray:**

![TrayTemps CPU GPU Tray](https://naetech.ro/wp-content/uploads/2024/traytemps/traytemps-tray.jpg)

---

## 🚀 Getting Started

### Prerequisites

* Windows 10 or newer.
* .NET Framework 4.8 or later.

### Installation

1. Go to the [Releases page](https://github.com/nmd-113/Tray-Temps/releases).
2. Download `TrayTemps.exe` from the latest release.
3. Run `TrayTemps.exe`.

---

## ⚠️ Administrator Access and Security Notice

TrayTemps can read more complete hardware sensor data when run as administrator.

If you continue without administrator rights, some temperatures, storage health data, or hardware details may be missing, partial, or less reliable. The app will still try to show any available fallback data where possible.

TrayTemps uses LibreHardwareMonitor, which may rely on low-level hardware access. Windows Security / Microsoft Defender or other antivirus software may block this kind of access.

Only allow the app or add an exclusion if you trust this specific build/source.

---

## ⚠️ Note on Antivirus Flags

TrayTemps uses [LibreHardwareMonitor](https://github.com/LibreHardwareMonitor/LibreHardwareMonitor) to access hardware sensors. Some systems or antivirus tools may flag low-level sensor access because it can involve driver-level hardware communication.

If Windows Security or another antivirus blocks the app, some readings may be unavailable.

Only allow the app or add an exclusion if you trust the downloaded file or built it yourself from this repository.

---

## ⚙️ Usage & Configuration

Once launched, TrayTemps runs in the system tray.

### Tray Controls

* **Double-click** a TrayTemps tray icon to open the main window.
* **Right-click** the tray icon for quick actions.
* Use the settings page to enable or disable CPU/GPU tray icons.
* Enable **Combined Tray Mode** to show CPU and GPU temperatures in one icon.

### Temperature Colors

TrayTemps supports two color modes:

* **Static Mode:** Choose fixed custom colors for CPU and GPU tray icons.
* **Temperature-Based Coloring:** Automatically change icon colors based on normal, warm, and critical thresholds.

### Hardware Details

Click the hardware labels in the main window to open detailed information:

* CPU details and live sensors
* GPU details and live sensors
* RAM details
* Motherboard and BIOS details
* Storage details, health, SMART information, and live storage sensors where available

### Storage Fallback

If LibreHardwareMonitor cannot expose live storage sensors, TrayTemps can still show disks detected through Windows WMI. Storage details may still be available even when live storage sensors are not.

### Missing Sensor Behavior

Some integrated GPUs/APUs or limited-access systems may not expose a usable GPU temperature sensor.

When no usable CPU/GPU temperature sensor is available:

* The main temperature may show `N/A`.
* The related tray icon option is disabled.
* TrayTemps does **not** fake GPU temperature using CPU temperature.

---

## 🛠️ Build From Source

### Requirements

* Visual Studio 2022 recommended.
* .NET Framework 4.8 Developer Pack.
* Windows 10 or newer.

### Build

```powershell
dotnet build Tray-Temps.sln -p:Configuration=Debug -p:Platform=x64
