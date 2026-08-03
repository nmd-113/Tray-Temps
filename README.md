# TrayTemps

TrayTemps is a lightweight Windows app that shows your **CPU and GPU temperatures** directly in the system tray.

It helps you keep an eye on your hardware without keeping a full monitoring app open.

---

## Features

- Shows CPU and GPU temperatures in the system tray
- Supports separate CPU/GPU tray icons
- Supports combined CPU + GPU tray mode
- Optional temperature-based tray colors with CPU/GPU identity lines to tray icons.
- Custom static tray colors
- Works with or without administrator rights
- Uses fallback sensors when possible
- Automatically disables tray options when a usable sensor is missing
- Shows hardware details for CPU, GPU, RAM, motherboard, BIOS, and storage
- Shows storage health / SMART details when supported
- Optional Windows startup support
- Light and dark theme support

---

## Screenshots

**Main Window**

![TrayTemps Main Window](https://naetech.ro/wp-content/uploads/2024/traytemps/traytemps.jpg?v2.0.2)

**CPU & GPU in Tray**

![TrayTemps CPU GPU Tray](https://naetech.ro/wp-content/uploads/2024/traytemps/traytemps-tray.jpg)

---

## Requirements

- Windows 10 or newer
- .NET Framework 4.8 or later

---

## Installation

1. Go to the [Releases page](https://github.com/nmd-113/Tray-Temps/releases).
2. Download the latest `TrayTemps.exe`.
3. Run the app.

---

## Administrator Access

TrayTemps can read more complete hardware sensor data when running as administrator.

You can still use the app without administrator rights, but some temperatures, storage health data, or hardware details may be missing or less reliable.

The app will still try to show any available fallback data.

---

## Antivirus Notice

TrayTemps uses [LibreHardwareMonitor](https://github.com/LibreHardwareMonitor/LibreHardwareMonitor) to read hardware sensors.

Some antivirus tools may block this type of low-level hardware access.

Only allow the app or add an exclusion if you trust the downloaded file or built it yourself from this repository.

---

## How to Use

After launching, TrayTemps runs in the system tray.

- Double-click a tray icon to open the main window
- Right-click a tray icon for quick actions
- Use the settings page to enable or disable CPU/GPU tray icons
- Enable combined tray mode to show CPU and GPU temperatures in one icon
- Click hardware labels in the main window to open detailed information

---

## Temperature Colors

TrayTemps supports two color modes:

- **Static colors**: choose fixed CPU and GPU tray colors
- **Temperature colors**: automatically change tray colors based on temperature limits

---

## Missing Sensors

Some systems may not expose all temperature sensors.

When a usable CPU or GPU temperature sensor is missing:

- The temperature may show `N/A`
- The related tray option is disabled
- TrayTemps does not fake GPU temperature using CPU temperature

---

## Storage Detection

If live storage sensors are unavailable, TrayTemps can still show disks detected by Windows.

Storage health and SMART details are shown only when supported by the hardware and available sensors.

---

## Build From Source

### Requirements

- Visual Studio 2022 recommended
- .NET Framework 4.8 Developer Pack
- Windows 10 or newer

### Build

```powershell
dotnet build Tray-Temps.sln -p:Configuration=Debug -p:Platform=x64
