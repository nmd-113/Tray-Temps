# TrayTemps

TrayTemps is a lightweight Windows hardware monitoring utility that shows **CPU and GPU temperatures directly in the system tray** and provides compact temperature and load cards in its main window.

It also includes an optional customizable **on-screen display (OSD)** for hardware statistics and FPS.

---

## Features

* CPU and GPU temperatures in the system tray
* Separate or combined tray icons
* Custom tray colors and temperature-based colors
* CPU/GPU temperature alerts
* Selectable CPU/GPU devices and temperature sensors
* Current, minimum, and maximum CPU/GPU temperatures
* Compact CPU/GPU load indicators with dynamic usage colors
* Customizable OSD with dedicated Metrics, Appearance, and Layout pages
* RAM and VRAM usage
* Built-in FPS counter using Windows ETW
* Independently configurable FPS display refresh interval
* Global OSD hotkey
* Detailed CPU, GPU, RAM, motherboard, BIOS, and storage information
* SMART and storage health information when supported
* Windows/WMI hardware fallbacks
* Optional PawnIO support for additional low-level sensors
* Resizable, DPI-aware interface with light and dark themes
* Start minimized to tray
* Optional Windows startup
* Built-in GitHub update checking
* Single portable executable

---

## Screenshots

### OSD Settings

![TrayTemps OSD settings](https://www.naetech.ro/wp-content/uploads/2024/traytemps/traytemps-dark.png?v=2.2.0.1)

### Tray Icons

![TrayTemps tray icons](https://www.naetech.ro/wp-content/uploads/2024/traytemps/traytemps-light.png?v=2.2.0.1)

### Hardware Information

![TrayTemps hardware information](https://www.naetech.ro/wp-content/uploads/2024/traytemps/traytemps-hardwareinfo.png?v=2.2.0.1)

---

## Requirements

* Windows 10 or newer
* x64 Windows
* .NET Framework 4.8

---

## Installation

1. Download the latest `TrayTemps.exe` from the [Releases page](https://github.com/nmd-113/Tray-Temps/releases).
2. Run the application.

No installation is required.

---

## On-Screen Display

The optional OSD can display:

* CPU temperature
* GPU temperature
* CPU load
* GPU load
* RAM usage
* VRAM usage
* FPS

The OSD supports custom labels, fonts, colors, opacity, screen position, configurable columns and display order, row/column and label/value spacing, and a global visibility hotkey. CPU/GPU temperature and usage values can optionally be combined into compact entries.

The overlay is click-through and does not take focus from other applications.

---

## FPS Counter

TrayTemps includes a lightweight built-in FPS counter using native Windows **Event Tracing for Windows (ETW)**.

No RTSS, MSI Afterburner, AMD overlay, NVIDIA overlay, or external FPS application is required.

The FPS display refresh interval can be configured independently without increasing CPU/GPU, memory, storage, or temperature sensor polling.

FPS availability may vary depending on the game, rendering method, and anti-cheat software.

---

## Hardware Sensor Access

TrayTemps uses [LibreHardwareMonitor](https://github.com/LibreHardwareMonitor/LibreHardwareMonitor) for sensor monitoring and supports [PawnIO](https://github.com/namazso/PawnIO) for additional low-level hardware access.

The official PawnIO installer is embedded inside TrayTemps and is only executed after user confirmation.

If PawnIO is unavailable or installation is declined, TrayTemps continues using the sensors and Windows/WMI information that remain accessible.

TrayTemps can also optionally run with administrator rights for fuller hardware access.

---

## Antivirus Notice

Low-level hardware monitoring tools can occasionally trigger heuristic antivirus detections.

TrayTemps does **not** disable antivirus protection, add exclusions, bypass UAC, or silently install drivers.

Always download TrayTemps from the official GitHub release page.

---

## Build From Source

### Requirements

* Visual Studio 2022
* .NET Framework 4.8 Developer Pack
* Windows 10 or newer

```powershell
dotnet build Tray-Temps.sln -p:Configuration=Release -p:Platform=x64
```

---

## Built With

* C# / WinForms
* .NET Framework 4.8
* LibreHardwareMonitor
* PawnIO
* Windows Management Instrumentation (WMI)
* Event Tracing for Windows (ETW)

---

## License

See [LICENSE.txt](LICENSE.txt).

---

Created by **NaeTech**
