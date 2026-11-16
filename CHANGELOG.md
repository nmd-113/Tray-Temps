# Changelog

## 2.5.0.0

- Added OSD presets: three built-in layouts and custom JSON presets. Classic stack is the default; OSD stays off by default.
- Added an optional 1% Low metric, independent of the FPS limiter.
- Expanded OSD customization with editable labels, metric groups, individual fonts and colors, and a custom group separator.
- Added a customizable FPS limiter toggle shortcut (default Ctrl+Shift+F11).
- Improved OSD alignment, spacing controls, settings responsiveness and UI consistency.
- Fixed preset import, preview and validation issues, and improved limiter reliability.
- Made installation cleanup and settings saves safer, preserving unrelated files and custom presets.
- Fixed hidden-metric group order and unnecessary RAM polling with OSD off.
- Improved automatic FPS limiter detection for custom-folder games, with detection work kept in the background.

## 2.4.0.0

### Added

- Optional FPS limiter in Settings, independent of the OSD FPS counter.
- Default and per-executable caps, process exclusions, and an optional Ctrl+Shift+F12 toggle.
- Limiter configuration and third-party notices dialogs with light/dark themes.
- Complete Visual Studio solution builds, including both native architectures, the managed controller, application and test projects.
- Automatic native payload staging, verification, release ZIP and SHA-256 checksums; no separate packaging command is needed after a Release build.
- Pinned Vulkan C headers bundled with their licenses, removing the separate Vulkan SDK prerequisite.

### Fixed

- Limiter shutdown and restart coordination, including cancellation and pending attachment.
- Unsupported 32-bit and elevated Vulkan candidates preserve an existing supported target.
- Window-size drift when moving between monitors with different DPI.
- Exclusions now preserve executable names containing spaces.
- Vulkan instance/device tracking is reclaimed on destruction, with clean failure and recovery when tracking capacity is exhausted.
- Full uninstall removes all limiter-cache versions and reports locked files so cleanup can be retried.

### Compatibility

- Controlled x64 Direct3D 9/11/12, OpenGL and Vulkan checks passed on the Radeon RX 7800 XT; see the [validation summary](Tests/VALIDATION.md). Earlier GTX 1050 observations are summarized in the [handover history](Native/HANDOVER_REVIEW.md).
- Vulkan games must start after the limiter is enabled and run without administrator rights. TrayTemps may run elevated.
- x86 attachment and elevated Vulkan targets remain unsupported. Real-game, anti-cheat, exclusive-fullscreen, VRR and long-duration compatibility are not established by the controlled fixtures.
