# TrayTemps

**Version 2.5.1.1** · [Changelog](CHANGELOG.md)

TrayTemps is a lightweight Windows utility for monitoring **CPU and GPU temperatures in the system tray**. Its main window shows temperatures, system load and hardware details, with an optional on-screen display (OSD), FPS counter and game FPS limiter.

## Features

- Separate or combined tray icons, custom colors, temperature-based colors and temperature alerts.
- Current, minimum and maximum temperatures; CPU/GPU load; RAM and VRAM usage.
- Customizable, click-through OSD with labels, metric groups, fonts, colors, layout and a global hotkey.
- Built-in FPS counter, 1% Low, average FPS, frametime and a small frametime graph using Windows ETW.
- Optional performance history with CPU/GPU graphs and enabled OSD metrics, retained for up to 30 minutes.
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
3. Enable **OSD** and open its gear button to choose metrics, appearance, layout and presets. OSD is off by default.
4. Enable **FPS limiter** to access its gear button and configure caps and profiles. The limiter is off by default.

## On-screen display

The OSD can show CPU/GPU temperatures and load, RAM/VRAM usage, FPS statistics, frametime and the current time. It is click-through and does not take focus from other applications. Its FPS counter works independently of the limiter and needs no external FPS overlay.

### Metrics and appearance

In **OSD Settings → Metrics**, each row has a visibility checkbox, an editable **Label**, and an editable **Group**. Double-click Label or Group to edit, press Enter or leave the cell to commit, or Esc to cancel. F2 edits the label; Tab switches between the two cells. Empty labels reserve no label width or label/value gap. Right-click while editing for Unicode icons, clipboard actions, or Reset label.

Text metrics with matching group names share one OSD item. Group names ignore surrounding whitespace and case; an empty Group cell displays the metric separately. Disabled metrics retain their membership and appearance preferences. Each member keeps its own font, label, and colors. The frametime graph is a standalone item.

In **Appearance**, all twelve metrics have independent font family, font size, label color, and value color settings. The OSD/background transparency sliders and background color are below the list. Transparency percentages increase as the corresponding surface becomes more transparent; existing saved opacity values retain the same visual appearance.

**Current time** appears last in the Metrics and Appearance lists. Its position in the actual OSD follows **Layout → Display order**.

### Layout and spacing

In **Layout**, the order tree shows individual metrics and named groups. Up/Down moves a selected group or standalone metric; selecting a group member changes its order inside the group. Each group's name from **Metrics → Group** is also its OSD caption. Select a standalone metric or group to edit its **Label/Value Gap**. Fresh settings use 10 px; presets can supply different gaps.

All grouped metrics, including FPS and 1% Low, use one built-in space between value slots. **Grouped values gap** in Layout scales this from 100% to 200% for every pair. Individual labels use the same per-metric **Label/Value Gap** rules in standalone and grouped rows. A group caption uses its first enabled member's font and label color. Old presets and settings still load; legacy FPS gaps migrate to the common per-metric controls, while separate group captions and custom separators are ignored.

FPS, average FPS and 1% Low reserve three digits; frametime reserves `xxx.xx ms`. Missing frame readings show `---` or `---.-- ms` in the same slot, so configuring the OSD outside a game shows its normal spacing. A metric expands when its formatted reading needs more digits and retains that width while readings remain available, avoiding repeated resizing around 1000. Missing readings, a new capture source or stopping capture reset the slot to three digits. Temperatures/load keep three-digit slots, RAM/VRAM reserve at least `xx.xxGB`, and unavailable sensor readings use compact `N/A` text. **Row spacing: 0** removes extra font leading while retaining glyph and outline clearance.

### Presets

In **Presets**, choose **Classic stack**, **Horizontal dashboard**, **Minimal transparent**, or **Spectrum stack**, or load/save a custom JSON preset.

**Spectrum stack** combines colored labels, a clock, CPU/GPU groups, RAM/VRAM, separate FPS/average and 1% Low/frametime groups, and a 160 × 30 frametime graph in a single column over translucent black. Its FPS refresh interval is the minimum **0.25 seconds**.

Fresh settings start with CPU/GPU temperatures selected, white 10 pt labels and values, and OSD off. Presets are loaded explicitly and preserve the current OSD enable state and shortcut. With OSD enabled, changes preview live; click **Save** to apply them or close the dialog to discard them.

### FPS statistics and frametime

Set **FPS refresh** independently from the hardware update interval, from **0.25 to 10 seconds**. This controls numeric frame-metric updates. The frametime graph scrolls independently, targeting about 30 updates per second while visible; history recording can also sample more frequently without changing the selected numeric interval.

**1% Low** can be selected independently of FPS, displayed separately, or assigned to any group. It shares the FPS monitoring session even when FPS is hidden and the limiter is off. Its label/value gap follows the same Layout rules as other metrics. Existing settings migrate their former CPU/GPU groups and enabled FPS/low pairing automatically, preserving saved labels, fonts, colors, and ordering.

The value uses the latest 10 seconds of accepted frame intervals from the current FPS stream. After at least 100 intervals, it averages the slowest 1% (count rounded up) and converts that average to FPS. A 20 ms average gives 50 FPS. Calculation runs at most once per second. `---` indicates warming up, stale data or an unavailable complete window. Process/stream changes reset frame statistics; OSD OFF stops capture unless history recording still needs it. Different tools use different definitions of 1% low, so compare results using the same method.

**Average FPS** uses the latest 10 seconds of accepted intervals, after a one-second warm-up. **Frametime** shows the mean accepted interval over the FPS refresh window in milliseconds. The **Frametime graph** shows ten seconds in 100 ms buckets, retaining peaks alongside averages as a thin trace over a transparent background. Subtle top and bottom guide lines keep the graph area visible without game samples. Set its width and height with Layout's **Graph size** controls: default **80 × 20**, width **20–800**, height **8–400**, in logical pixels. Dimensions scale with display DPI and fit the available screen space. Explicit saved dimensions are preserved. All three metrics are off by default and use the same capture session.

Long frames can span several graph buckets; the line connects their completed samples without artificial breaks. Stale capture and process/stream resets clear the old curve. Faster graph updates reuse the unchanged OSD text and background, and stop when the graph is disabled.

## Performance history

Click the graph icon beside **Temperatures and usage** to open the resizable **Performance history** window. **Record history** is off by default; when checked, collection continues with the window closed and with OSD disabled. CPU/GPU temperatures and usage are always recorded; additional numeric metrics follow the OSD metric selections. The 1/5/10/30-minute selector changes only the displayed range. Recording can be paused without losing samples, and Clear empties the graphs. Up to 30 minutes stay in memory until TrayTemps exits; only the recording preference, range and window size are saved. Application/source changes and missing readings break graph lines.

History scrolling adapts to the selected range and window width, with faster updates for short ranges and a lower redraw rate for long ranges. Data snapshots retain their existing refresh budget. Zero readings and isolated samples remain visible; paused, hidden and minimized views avoid unnecessary redraws.

Hover over a history plot to inspect the nearest recorded values for its traces, with units and time relative to the latest edge. Frametime tooltips also show distinct peaks. Empty regions and missing-data/source-change gaps do not display held readings.

## FPS limiter

The limiter detects eligible foreground games and applies the default FPS cap or the profile for that executable. You can add executable profiles, exclude process names, and enable a customizable toggle shortcut (default **Ctrl+Shift+F11**). Click the shortcut field in FPS limiter settings and press Ctrl, Shift, or Alt plus a key, then Save. OSD shortcuts take priority if the same combination is assigned to both. Supported caps range from **10 to 1000 FPS**.

Games in custom folders can also be detected through recognized game SDK and graphics modules. Add an executable profile if a game is still unrecognized; exclusions always take priority.

- Targets supported **64-bit Direct3D 9/11/12, OpenGL and Vulkan** applications; compatibility varies by game and driver.
- For Vulkan, enable the limiter **before launching the game** and run the game without administrator rights. TrayTemps itself may run elevated.
- 32-bit attachment and elevated Vulkan targets are unsupported. Protected/anti-cheat games have not been verified.
- Turning the limiter off removes its Vulkan registrations and releases ownership. Games that already loaded the layer continue in pass-through mode.
- A rejected game leaves the previous target active. Repeated attempts to inject a rejected process are suppressed; restart that game or toggle the limiter off and on to retry.
- When a game already has a different limiter engine loaded, restart the game before attaching the new engine.

## Hardware access

TrayTemps uses [LibreHardwareMonitor](https://github.com/LibreHardwareMonitor/LibreHardwareMonitor), Windows/WMI fallbacks, and optional [PawnIO](https://github.com/namazso/PawnIO) support. Administrator rights can provide more complete sensor access. Available readings depend on the hardware, driver and permissions.

The official PawnIO installer is embedded and runs only after user confirmation. If PawnIO is unavailable or installation is declined, monitoring continues with the sensors and Windows/WMI information that remain accessible.

Low-level hardware tools can trigger antivirus heuristics. TrayTemps does not disable antivirus protection, add exclusions, bypass UAC or silently install drivers. Download it from the official release page.

## Settings and uninstall

Settings are stored in `%APPDATA%\TrayTemps\settings.json`, including when running the portable executable. Custom JSON presets are saved wherever you choose. History samples stay in memory and are discarded when the app exits.

Full uninstall removes the verified installed executable, startup task, desktop shortcut, generated settings/runtime files and limiter cache. Custom presets and unrelated files are preserved. The installed executable is removed after exit when necessary; locked-file failures are reported so cleanup can be retried.

## Screenshots

### Main Window & Settings

![TrayTemps Main Menu](https://www.naetech.ro/wp-content/uploads/2024/traytemps/Main-Menu.png?v=2.5.1.1)
![TrayTemps Settings Menu](https://www.naetech.ro/wp-content/uploads/2024/traytemps/Setting-Menu.png?v=2.5.1.1)

### OSD Settings Window & FPS limiter Window

![TrayTemps OSD Settings Window & FPS limiter Window](https://www.naetech.ro/wp-content/uploads/2024/traytemps/OSD-FPS.png?v=2.5.1.1)

### Tray Icons & Tray Menu

![TrayTemps Tray Icons & Tray Menu](https://www.naetech.ro/wp-content/uploads/2024/traytemps/Tray-Icons.png?v=2.5.1.1)

### Hardware Information & Performance History

![TrayTemps hardware information](https://www.naetech.ro/wp-content/uploads/2024/traytemps/HW-Dialog-and-Performance-History.png?v=2.5.1.1)

### OSD Profiles

![TrayTemps OSD Profiles](https://www.naetech.ro/wp-content/uploads/2024/traytemps/OSD-Profiles.png?v=2.5.1.1)

## Build from source

Use Visual Studio 2022 with the **.NET desktop development** and **Desktop development with C++** tools, the .NET Framework 4.8 Developer Pack and a Windows 10/11 SDK. Open `Tray-Temps.sln`, select **Release / x64**, and press **Build Solution** or **Rebuild Solution**.

The solution builds the native limiter for x64 and x86, stages and verifies its payloads, builds the managed controller, application and test projects, and automatically creates the release ZIP and checksums. The Vulkan headers are bundled, so a separate Vulkan SDK is not required.

The final portable executable is **`bin\x64\Release\TrayTemps.exe`**. Release builds also update `dist/TrayTemps.exe`, the versioned ZIP and `SHA256SUMS.txt`. Debug builds produce the complete application under `bin\x64\Debug` without overwriting the release package.

Commit and sync through Visual Studio as usual. All project sources, bundled headers, native payloads and notices belong in Git; generated `bin`, `obj` and `dist` outputs remain ignored.

For command-line builds, `build-dist.ps1` invokes the same solution build. See the [native build guide](Native/README.md) and [test instructions](Tests/README.md) for development details.

Built with C#/WinForms, .NET Framework 4.8, LibreHardwareMonitor, PawnIO, WMI and ETW.

## Links and license

[![TrayTemps | AlternativeTo](https://alternativeto.net/static/badges/badge-compact-color.svg)](https://alternativeto.net/software/traytemps/about/?utm_source=badge&utm_medium=referral)

[![Download Tray-Temps](https://a.fsdn.com/con/app/sf-download-button)](https://sourceforge.net/projects/tray-temps/files/latest/download)

TrayTemps is available under the MIT license; see [LICENSE.txt](LICENSE.txt). Bundled components retain their own licenses, including the [FPS limiter notices](Resources/FpsLimiter/NOTICE.txt) and [JetBrains Mono font license](Resources/OFL-JetBrainsMono.txt).

Created by **NaeTech**.
