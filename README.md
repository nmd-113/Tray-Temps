# TrayTemps

TrayTemps is a lightweight Windows utility for monitoring **CPU and GPU temperatures in the system tray**. Its main window shows temperatures, system load and hardware details, with an optional on-screen display (OSD), FPS counter and game FPS limiter.

## Features

- Separate or combined tray icons, custom colors, temperature-based colors and temperature alerts.
- Current, minimum and maximum temperatures; CPU/GPU load; RAM and VRAM usage.
- Customizable, click-through OSD with labels, metric groups, fonts, colors, layout and a global hotkey.
- Built-in FPS counter using Windows ETW, with its own display refresh interval.
- Optional FPS limiter with a default cap, per-executable profiles, process exclusions and a toggle hotkey.
- Detailed CPU, GPU, memory, motherboard, BIOS and storage information, including SMART data when available.
- Light/dark themes, a resizable DPI-aware window, start minimized to tray, optional Windows startup and update checking.
- A single portable executable.

## Requirements

- Windows 10 or newer, **x64**.
- .NET Framework 4.8.

## Get started

1. Download `TrayTemps.exe` from the [Releases page](https://github.com/nmd-113/Tray-Temps/releases) and run it. No installation is required.
2. Open **Settings** to choose sensors, tray icons, colors and the update interval.
3. Enable **OSD** and open its gear button to choose the metrics and appearance.
4. Enable **FPS limiter** if you want a game cap, then open its gear button to configure it. The limiter is off by default.

The OSD can show CPU/GPU temperatures and load, RAM/VRAM usage, FPS and the current time. It is click-through and does not take focus from other applications. Its FPS counter works independently of the limiter and needs no external FPS overlay.

## FPS limiter

The limiter detects eligible foreground games and applies the default FPS cap or the profile for that executable. You can add executable profiles, exclude process names, and enable the optional **Ctrl+Shift+F12** toggle. Supported caps range from **10 to 1000 FPS**.

- Targets supported **64-bit Direct3D 9/11/12, OpenGL and Vulkan** applications; compatibility varies by game and driver.
- For Vulkan, enable the limiter **before launching the game** and run the game without administrator rights. TrayTemps itself may run elevated.
- 32-bit attachment and elevated Vulkan targets are unsupported. Protected/anti-cheat games have not been verified.

## Hardware access

TrayTemps uses [LibreHardwareMonitor](https://github.com/LibreHardwareMonitor/LibreHardwareMonitor), Windows/WMI fallbacks, and optional [PawnIO](https://github.com/namazso/PawnIO) support. Administrator rights can provide more complete sensor access. Available readings depend on the hardware, driver and permissions.

The official PawnIO installer is embedded and runs only after user confirmation. If PawnIO is unavailable or installation is declined, monitoring continues with the sensors and Windows/WMI information that remain accessible.

Low-level hardware tools can trigger antivirus heuristics. TrayTemps does not disable antivirus protection, add exclusions, bypass UAC or silently install drivers. Download it from the official release page.

## Screenshots

### Main Window

![TrayTemps Main Window](https://www.naetech.ro/wp-content/uploads/2024/traytemps/traytemps-main-settings.png?v=2.3.0.4)

### OSD Settings

![TrayTemps OSD settings](https://www.naetech.ro/wp-content/uploads/2024/traytemps/traytemps-osd.png?v=2.3.0.4)

### Tray Icons

![TrayTemps tray icons](https://www.naetech.ro/wp-content/uploads/2024/traytemps/traytemps-main-light.png?v=2.3.0.4)

### Hardware Information

![TrayTemps hardware information](https://www.naetech.ro/wp-content/uploads/2024/traytemps/traytemps-hw-dialog.png?v=2.3.0.4)

## Build from source

Use Visual Studio 2022 with the **.NET desktop development** and **Desktop development with C++** tools, the .NET Framework 4.8 Developer Pack and a Windows 10/11 SDK. Open `Tray-Temps.sln`, select **Release / x64**, and press **Build Solution** or **Rebuild Solution**.

The solution builds the native limiter for x64 and x86, stages and verifies its payloads, builds the managed controller, application and test projects, and automatically creates the release ZIP and checksums. The Vulkan headers are bundled, so a separate Vulkan SDK is not required.

The final portable executable is **`bin\x64\Release\TrayTemps.exe`**. Release builds also update `dist/TrayTemps.exe`, the versioned ZIP and `SHA256SUMS.txt`. Debug builds produce the complete application under `bin\x64\Debug` without overwriting the release package.

Commit and sync through Visual Studio as usual. All project sources, bundled headers, native payloads and notices belong in Git; generated `bin`, `obj` and `dist` outputs remain ignored.

For command-line builds, `build-dist.ps1` invokes the same solution build. See the [native build guide](Native/README.md) and [test instructions](Tests/README.md) for development details.

Built with C#/WinForms, .NET Framework 4.8, LibreHardwareMonitor, PawnIO, WMI and ETW.

Development: [test instructions](Tests/README.md), [changelog](CHANGELOG.md), [integration review](Native/INTEGRATION_REVIEW.md), [handover history](Native/HANDOVER_REVIEW.md).

## Links and license

[![TrayTemps | AlternativeTo](https://alternativeto.net/static/badges/badge-compact-color.svg)](https://alternativeto.net/software/traytemps/about/?utm_source=badge&utm_medium=referral)

[![Download Tray-Temps](https://a.fsdn.com/con/app/sf-download-button)](https://sourceforge.net/projects/tray-temps/files/latest/download)

TrayTemps is available under the MIT license; see [LICENSE.txt](LICENSE.txt). Bundled components retain their own licenses, including the [FPS limiter notices](Resources/FpsLimiter/NOTICE.txt) and [JetBrains Mono font license](Resources/OFL-JetBrainsMono.txt).

Created by **NaeTech**.
