# TrayTemps

TrayTemps is a lightweight Windows app that shows your **CPU and GPU temperatures** directly in the system tray.

It helps you monitor your hardware without keeping a full monitoring app open.

---

## Features

- Shows CPU and GPU temperatures in the system tray
- Supports separate CPU and GPU tray icons
- Supports combined CPU + GPU tray mode
- Optional temperature-based tray colors
- Optional CPU/GPU identity lines on tray icons
- Custom static tray colors
- Configurable CPU and GPU temperature alerts
- CPU and GPU sensor selection
- Works with or without administrator rights
- Optional PawnIO support for improved low-level sensor access
- Uses Windows/WMI fallbacks when possible
- Loads detailed hardware information in the background for faster startup
- Automatically disables sensor-dependent options when a usable sensor is unavailable
- Shows hardware details for CPU, GPU, RAM, motherboard, BIOS, and storage
- Shows live hardware sensors when supported
- Shows storage health and SMART details when supported
- Optional Windows startup support
- Supports starting minimized to the system tray
- Supports minimized startup with normal or elevated rights
- Light and dark theme support
- Built-in update checking through GitHub
- Distributed as a single portable executable

---

## Screenshots

### Main Window

![Main Window](https://www.naetech.ro/wp-content/uploads/2024/traytemps/traytemps-dark.png)

### Settings Window

![Settings Window](https://www.naetech.ro/wp-content/uploads/2024/traytemps/traytemps-light.png)

### CPU & GPU in Tray

![CPU & GPU in Tray](https://www.naetech.ro/wp-content/uploads/2024/traytemps/traytemps-trayicons.png)

---

## Requirements

- Windows 10 or newer
- .NET Framework 4.8 or later

---

## Installation

1. Go to the [Releases page](https://github.com/nmd-113/Tray-Temps/releases).
2. Download the latest `TrayTemps.exe`.
3. Run the app.

No TrayTemps installation is required.

---

## Hardware Sensor Access

TrayTemps uses [LibreHardwareMonitor](https://github.com/LibreHardwareMonitor/LibreHardwareMonitor) for hardware monitoring and supports [PawnIO](https://github.com/namazso/PawnIO) for low-level hardware access.

The official PawnIO installer is embedded inside TrayTemps. If PawnIO is not available, TrayTemps can ask whether you want to install it.

PawnIO is optional. If installation is declined or unavailable, TrayTemps continues using the sensors and Windows/WMI hardware information that remain accessible.

TrayTemps can also optionally restart with administrator rights for fuller hardware access.

---

## Startup Behavior

TrayTemps supports automatic Windows startup and can start minimized directly to the system tray.

When minimized startup is enabled, TrayTemps can remember whether startup should use:

- Normal user rights
- Administrator rights

Administrator elevation is optional and TrayTemps can continue with reduced hardware access if elevation is declined.

---

## Antivirus Notice

TrayTemps uses [LibreHardwareMonitor](https://github.com/LibreHardwareMonitor/LibreHardwareMonitor) and optional PawnIO low-level hardware access.

TrayTemps does not add antivirus exclusions, disable security features, or bypass Windows protections.

PawnIO installation is performed only after user confirmation and uses the embedded official installer.

Always verify that TrayTemps downloads come from the official project release.

---

## How to Use

After launching, TrayTemps runs in the system tray.

- Double-click a tray icon to open the main window
- Right-click a tray icon for quick actions
- Use the settings page to enable or disable CPU/GPU tray icons
- Enable combined tray mode to show CPU and GPU temperatures in one icon
- Configure temperature alerts, thresholds, sensors, and cooldowns
- Click hardware labels in the main window to open detailed hardware and sensor information
- Use the About page to check for updates

---

## Temperature Colors

TrayTemps supports two color modes:

- **Static colors:** Choose fixed CPU and GPU tray colors
- **Temperature colors:** Automatically change tray colors based on configured temperature limits

CPU and GPU identity lines can also be enabled to make the tray icons easier to distinguish.

---

## Temperature Alerts

TrayTemps can show alerts when the selected CPU or GPU temperature exceeds a configured threshold.

Alerts can be configured separately for each device:

- Enable or disable alerts
- Select the temperature sensor
- Configure the temperature threshold
- Configure the alert cooldown

Available sensors may differ depending on the hardware, drivers, PawnIO availability, and access permissions.

---

## Missing Sensors

Some systems may not expose all hardware sensors.

When a usable CPU or GPU temperature sensor is unavailable:

- The temperature may show `N/A`
- Sensor-dependent options may be disabled
- Hardware information may still be available through Windows/WMI
- TrayTemps does not substitute unrelated sensor values

Installing PawnIO when prompted may provide access to additional supported sensors.

---

## Hardware Detection

TrayTemps separates hardware detection from sensor availability.

If LibreHardwareMonitor does not expose a component or detailed sensor information, TrayTemps can use Windows/WMI fallback information where reliable.

This helps keep CPU, GPU, RAM, storage, motherboard, and other detected hardware visible even when low-level sensors are unavailable.

Detailed hardware discovery is performed in the background where possible so CPU/GPU tray temperatures can appear quickly after startup.

---

## Storage Detection

TrayTemps supports SATA and NVMe storage detection and shows Windows/WMI fallback devices when live LibreHardwareMonitor storage access is unavailable.

Storage information may include:

- Model and interface
- Capacity
- Firmware and serial information
- SMART health
- Remaining life
- Temperature
- Power-on hours
- Other supported health sensors

Available information depends on the drive, controller, drivers, permissions, and sensor support.

---

## Reset Settings

Settings can be reset from within the app without restarting it.

Resetting restores the default application settings.

---

## Uninstall / Cleanup

TrayTemps is portable, but it can create user settings and an optional Windows startup task.

The built-in cleanup flow can remove:

- TrayTemps settings and user data
- The Windows startup task
- Both, when performing a full cleanup

Only TrayTemps-owned files and configuration are removed.

---

## Build From Source

### Requirements

- Visual Studio 2022 recommended
- .NET Framework 4.8 Developer Pack
- Windows 10 or newer

### Build

```powershell
dotnet build Tray-Temps.sln -p:Configuration=Debug -p:Platform=x64
