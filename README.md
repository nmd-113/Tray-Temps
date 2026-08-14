# TrayTemps

TrayTemps is a lightweight Windows hardware monitoring utility that shows **CPU and GPU temperatures directly in the system tray**.

It also includes an optional customizable **on-screen display (OSD)** for hardware stats and FPS.

---

## Features

* CPU and GPU temperatures in the system tray
* Separate or combined tray icons
* Custom tray colors and temperature-based colors
* CPU/GPU temperature alerts
* Selectable temperature sensors
* Customizable OSD
* CPU/GPU temperature and load
* RAM and VRAM usage
* Built-in FPS counter using Windows ETW
* Global OSD hotkey
* Detailed CPU, GPU, RAM, motherboard, BIOS, and storage information
* SMART and storage health information when supported
* Windows/WMI hardware fallbacks
* Optional PawnIO support for additional low-level sensors
* Light and dark themes
* Start minimized to tray
* Optional Windows startup
* Built-in GitHub update checking
* Single portable executable

---

## Screenshots

### Main Window

![Main Window](https://www.naetech.ro/wp-content/uploads/2024/traytemps/traytemps-dark.png)

### Settings

![Settings](https://www.naetech.ro/wp-content/uploads/2024/traytemps/traytemps-light.png)

### CPU & GPU Tray Icons

![CPU & GPU Tray Icons](https://www.naetech.ro/wp-content/uploads/2024/traytemps/traytemps-trayicons.png)

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

The OSD supports custom labels, spacing, font, colors, opacity, padding, layout, screen position, and a configurable global hotkey.

The overlay is click-through and does not take focus from other applications.

---

## FPS Counter

TrayTemps includes a lightweight built-in FPS counter using native Windows **Event Tracing for Windows (ETW)**.

No RTSS, MSI Afterburner, AMD overlay, NVIDIA overlay, or external FPS application is required.

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
