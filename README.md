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
- Uses fallback sensors when possible
- Automatically disables tray options when a usable sensor is missing
- Shows hardware details for CPU, GPU, RAM, motherboard, BIOS, and storage
- Shows storage health and SMART details when supported
- Optional Windows startup support
- Supports starting minimized to the system tray
- Light and dark theme support
- Built-in update checking through GitHub

---

## Screenshots

### Main Window

![TrayTemps Main Window](https://naetech.ro/wp-content/uploads/2024/traytemps/traytemps-dark.png?v2.0.3)

### Settings Window

![TrayTemps Settings Window](https://naetech.ro/wp-content/uploads/2024/traytemps/traytemps-light.png?v2.0.3)

### CPU & GPU in Tray

![TrayTemps CPU GPU Tray](https://naetech.ro/wp-content/uploads/2024/traytemps/traytemps-trayicons.jpg)

---

## Requirements

- Windows 10 or newer
- .NET Framework 4.8 or later

---

## Installation

1. Go to the [Releases page](https://github.com/nmd-113/Tray-Temps/releases).
2. Download the latest `TrayTemps.exe`.
3. Run the app.

No installation is required.

---

## Hardware Sensor Access

TrayTemps uses [LibreHardwareMonitor](https://github.com/LibreHardwareMonitor/LibreHardwareMonitor) and PawnIO for low-level hardware sensor access.

If a compatible PawnIO installation is unavailable, TrayTemps can continue with the sensors and Windows/WMI fallback information that remain accessible.

TrayTemps can optionally restart with administrator rights for fuller hardware access. The start-minimized setting can remember whether hidden startup should use administrator rights.

---

## Antivirus Notice

TrayTemps uses [LibreHardwareMonitor](https://github.com/LibreHardwareMonitor/LibreHardwareMonitor) to read hardware sensors.

TrayTemps does not add antivirus exclusions or bypass Windows security features. Verify that downloads come from the official project release.

---

## How to Use

After launching, TrayTemps runs in the system tray.

- Double-click a tray icon to open the main window
- Right-click a tray icon for quick actions
- Use the settings page to enable or disable CPU/GPU tray icons
- Enable combined tray mode to show CPU and GPU temperatures in one icon
- Configure temperature alerts, thresholds, sensors, and cooldowns
- Click hardware labels in the main window to open detailed information
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

Available sensors may differ depending on the hardware and access permissions.

---

## Missing Sensors

Some systems may not expose all temperature sensors.

When a usable CPU or GPU temperature sensor is missing:

- The temperature may show `N/A`
- The related tray option is disabled
- TrayTemps does not fake GPU temperature using CPU temperature

Install PawnIO when prompted to enable supported low-level sensors. TrayTemps continues with available fallbacks if PawnIO is declined or unavailable.

---

## Storage Detection

If live storage sensors are unavailable, TrayTemps can still show disks detected by Windows.

Storage health and SMART details are shown only when supported by the hardware, controller, drivers, and available sensors.

---

## Reset Settings

Settings can be reset from within the app without restarting it.

Resetting restores the default application settings.

---

## Build From Source

### Requirements

- Visual Studio 2022 recommended
- .NET Framework 4.8 Developer Pack
- Windows 10 or newer

### Build

```powershell
dotnet build Tray-Temps.sln -p:Configuration=Debug -p:Platform=x64
````

The compiled application will be placed in the corresponding build output directory.

---

## Issues and Feedback

To report a problem or suggest an improvement, open an issue on the repository's [Issues page](https://github.com/nmd-113/Tray-Temps/issues).

For sensor-related issues, include:

* Windows version
* CPU and GPU models
* Whether PawnIO is installed and its version
* The displayed sensor or hardware report
* The expected and actual behavior

---

## Disclaimer

TrayTemps displays information reported by the operating system, hardware, drivers, and LibreHardwareMonitor.

Sensor readings may differ between systems and should not be treated as a replacement for manufacturer-provided diagnostic tools.
