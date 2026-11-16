# TrayTemps

TrayTemps is a lightweight Windows utility for monitoring **CPU and GPU temperatures in the system tray**. Its main window shows temperatures, system load and hardware details, with an optional on-screen display (OSD), FPS counter and game FPS limiter.

## Features

- Separate or combined tray icons, custom colors, temperature-based colors and temperature alerts.
- Current, minimum and maximum temperatures; CPU/GPU load; RAM and VRAM usage.
- Customizable, click-through OSD with labels, metric groups, fonts, colors, layout and a global hotkey.
- Built-in FPS counter and optional 1% Low metric using Windows ETW, with a configurable refresh interval.
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
4. Enable **FPS limiter** to access its gear button and configure caps and profiles. The limiter is off by default.

The OSD can show CPU/GPU temperatures and load, RAM/VRAM usage, FPS and the current time. It is click-through and does not take focus from other applications. Its FPS counter works independently of the limiter and needs no external FPS overlay.

In **OSD Settings → Metrics**, each row has a visibility checkbox, an editable **Label**, and an editable **Group**. Double-click Label or Group to edit, press Enter or leave the cell to commit, or Esc to cancel. F2 edits the label; Tab switches between the two cells. Empty labels reserve no label width or label/value gap. Right-click while editing for Unicode icons, clipboard actions, or Reset label.

Metrics with matching group names share one OSD item. Group names ignore surrounding whitespace and case; an empty Group cell displays the metric separately. Disabled metrics retain their membership and appearance preferences. Each member keeps its own font, label, and colors.

In **Appearance**, all nine metrics have independent font family, font size, label color, and value color settings. The OSD/background transparency sliders and background color are below the list. Transparency percentages increase as the corresponding surface becomes more transparent; existing saved opacity values retain the same visual appearance.

In **Layout**, the order tree shows individual metrics and named groups. Up/Down moves a selected group or standalone metric; selecting a group member changes its order inside the group. Select a group to edit its optional **Group label**. A blank group label hides that caption. Group names identify membership and are not automatically displayed as captions.

The **Group separator** field accepts text, Unicode icons, or spaces and uses the same right-click icon menu. Empty uses one space, or two between FPS and 1% Low. The FPS/1% Low pair also adds one space on each side of its separator. Explicit whitespace is preserved, and separators appear only between enabled group members. The separator inherits the preceding member's font and value color. A group label inherits its first enabled member's font and label color.

In **Presets**, load one of three built-in layouts or load/save custom JSON presets. **Classic stack** is the default layout; OSD remains off by default. Loading previews the layout while preserving the current OSD enable state and shortcut; click **Save** to apply it or close to discard.

**1% Low** can be selected independently of FPS, displayed separately, or assigned to any group. It uses the existing FPS monitoring session even when FPS is hidden and the limiter is off. The **FPS spacing** control applies to both FPS and 1% Low label/value pairs. Existing settings migrate their former CPU/GPU groups and enabled FPS/low pairing automatically, preserving saved labels, fonts, colors, and ordering.

The value uses the latest 10 seconds of accepted frame intervals from the current FPS stream. After at least 100 intervals, it averages the slowest 1% (count rounded up) and converts that average to FPS. A 20 ms average gives 50 FPS. Calculation runs at most once per second. `N/A` indicates warming up, stale data or an unavailable complete window, matching the regular FPS metric; process/stream changes and OSD OFF reset history. Different tools use different definitions of 1% low, so compare results using the same method. This option works with the FPS limiter off.

## FPS limiter

The limiter detects eligible foreground games and applies the default FPS cap or the profile for that executable. You can add executable profiles, exclude process names, and enable a customizable toggle shortcut (default **Ctrl+Shift+F11**). Click the shortcut field in FPS limiter settings and press Ctrl, Shift, or Alt plus a key, then Save. OSD shortcuts take priority if the same combination is assigned to both. Supported caps range from **10 to 1000 FPS**.

Games in custom folders can also be detected through recognized game SDK and graphics modules. Add an executable profile if a game is still unrecognized; exclusions always take priority.

- Targets supported **64-bit Direct3D 9/11/12, OpenGL and Vulkan** applications; compatibility varies by game and driver.
- For Vulkan, enable the limiter **before launching the game** and run the game without administrator rights. TrayTemps itself may run elevated.
- 32-bit attachment and elevated Vulkan targets are unsupported. Protected/anti-cheat games have not been verified.
- Close standalone TrayFPS before enabling the limiter in TrayTemps.
- Turning the limiter off removes its Vulkan registrations and releases ownership. Games that already loaded the layer continue in pass-through mode.
- A rejected game leaves the previous target active. Repeated attempts to inject a rejected process are suppressed; restart that game or toggle the limiter off and on to retry.
- When a game already has a different limiter engine loaded, restart the game before attaching the new engine.

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
