# FPS limiter integration notes

The optional limiter is integrated into the existing settings, theme, tray and window lifecycle. It defaults to off and remains independent of the ETW-based OSD FPS counter.

- The host service serializes discovery, attachment, configuration and shutdown.
- Executable profiles, exclusions and the optional Ctrl+Shift+F12 shortcut use the normal settings file.
- OSD shortcut conflicts take priority when saved.
- Native engine ownership prevents simultaneous standalone TrayFPS and TrayTemps control.
- Pending attachments are cleaned up before shutdown, restart or uninstall.
- Vulkan tracking is reclaimed on object destruction; exclusions preserve spaces in executable names.

Visual Studio builds all application dependencies and test projects, stages both native architectures, and packages Release builds automatically. See the [native build guide](README.md), [validation summary](../Tests/VALIDATION.md), and [changelog](../CHANGELOG.md).

Older implementation plans, one-off patch exports and raw machine logs have been archived outside the source tree. This page retains the original documentation link.
