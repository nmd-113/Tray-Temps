# Changelog

## 2.5.1.0

### Added

- Four built-in OSD presets: Classic stack, Horizontal dashboard, Minimal transparent and Spectrum stack, plus custom JSON preset loading/saving. Spectrum stack uses a 250 × 30 frametime graph and a 0.25-second FPS refresh interval.
- Independent OSD metrics for 1% Low, average FPS, frametime and a frametime graph, sharing the existing capture session and working with the FPS limiter off.
- Layout **Graph size** controls for width (80–800) and height (20–400 logical pixels), defaulting to 160 × 42, with live preview, DPI scaling and preset support.
- A resizable performance history window with up to 30 minutes of graphs. Optional recording continues with the window closed and OSD disabled; samples remain in memory only.
- Editable metric labels, named groups, and individual fonts and label/value colors.
- A customizable FPS limiter toggle shortcut, defaulting to Ctrl+Shift+F11.

### Changed

- Fresh OSD settings use CPU/GPU temperatures, white 10 pt labels and values, and OSD disabled. Built-in presets are loaded explicitly.
- All grouped text metrics, including FPS and 1% Low, use the same separator, group-gap percentage and per-metric label/value spacing rules. Group names supply captions; legacy settings migrate to the common controls.
- `N/A` readings use compact text widths. Availability changes can resize the OSD; numeric readings return to stable reserved slots.
- Current time appears last in the Metrics and Appearance lists while saved Layout display order is preserved.
- Automatic limiter detection recognizes more custom-folder games through game SDK and graphics modules, with inspection kept in the background.

### Fixed

- Frametime graph gaps during long frames and visibly slow scrolling. Graph presentation now runs independently of numeric FPS refresh, reuses unchanged text/background rendering, and retains stale-data and source-reset boundaries. Concurrent primary/fallback capture reads no longer produce false unavailable samples.
- Preset import, validation, preview and Save/Cancel behavior, including preservation of the current OSD enable state and shortcut.
- RAM/VRAM alignment, zero row spacing, hidden-metric group order, oversized OSD layout fitting and empty-settings defaults.
- OSD frame-metric refresh timing while history records, history range synchronization after reset, and unnecessary RAM polling when neither OSD nor history needs it.
- History window sizing, colors, text layout and range-selector alignment after font initialization or theme changes.
- Invisible zero-value history traces and isolated samples at plot edges. History scrolling now adapts to the displayed range, reuses sample snapshots between data updates, and skips unchanged paused/hidden views while retaining genuine missing-data and source-change gaps.
- Unnecessary horizontal scrolling in FPS profiles, hardware-dialog subtitle clipping and missing hardware/save-button glyphs.
- Live-sensor refresh clearing text selections or replacing unchanged text. Hidden/minimized hardware windows now skip live queries.
- Malformed update-response entries causing unhandled exceptions.
- Uninstall stopping when Windows denies deletion of a running executable. Cleanup validates settings paths before removal, retries locked files, removes generated runtime configuration, and reports deferred failures with an error log. Custom presets and unrelated files are preserved.
- Safer atomic settings saves and recovery from incomplete writes.

### Maintenance

- Removed obsolete audit/handover documents, old generated review folders, the unused Visual Studio settings scaffold and a duplicate icon.
- Refreshed build/test documentation and the portable EXE, versioned ZIP and SHA-256 checksums for 2.5.1.0.

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

- Controlled x64 Direct3D 9/11/12, OpenGL and Vulkan checks passed on the Radeon RX 7800 XT. Earlier checks also ran on a GTX 1050.
- Vulkan games must start after the limiter is enabled and run without administrator rights. TrayTemps may run elevated.
- x86 attachment and elevated Vulkan targets remain unsupported. Real-game, anti-cheat, exclusive-fullscreen, VRR and long-duration compatibility are not established by the controlled fixtures.
