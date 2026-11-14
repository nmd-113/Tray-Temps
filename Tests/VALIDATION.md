# Release validation

Checked on 2026-11-14. This is a portable summary; raw diagnostic logs and desktop captures are kept outside version control.

## Build and Git workflow

- Actual Visual Studio Release/x64 Rebuild: **5 succeeded, 0 failed, 0 skipped**.
- Debug/x64 Build: all projects succeeded or were up to date; Release artifacts were preserved.
- Incremental Release Build recreated a missing ZIP automatically.
- Build repaired a missing secondary x86 native DLL without a separate command.
- Repeated native rebuilds produced stable payload hashes.
- A clean export of the staged Git files built all projects and release packages without existing build caches or an external Vulkan SDK.
- Both native payloads, the manifest and the embedded cache version agree. The current engine is `1.0.0-0B84C93A0E2A`.

## Runtime checks

- Isolated exclusion and cache-cleanup regression checks passed.
- Native Vulkan lifecycle checks passed: 192 instance cycles, 384 device cycles, shared dispatch, capacity rollback and recovery.
- The controlled Direct3D 9/11/12 and OpenGL integration suite passed all 130 checks, including profiles, exclusions, OFF/ON, live cap changes and shutdown pass-through.
- Mixed-privilege Vulkan checks passed for an elevated controller and a normal target, including rejection of an elevated candidate while preserving the active cap.
- Shipping-EXE startup checks covered all OSD/limiter combinations, old/invalid settings and start minimized. User preference files were restored.

## AMD host observations

Radeon RX 7800 XT, driver 32.0.32015.16001, Windows 11 IoT Enterprise LTSC build 26100. Controlled windowed presentation intervals:

| API | Caps checked | Typical measured interval |
| --- | --- | --- |
| Direct3D 9/11/12 | 60 and 90 FPS | About 16.67 and 11.11 ms |
| OpenGL | 30 and 20 FPS | About 33.33 and 50.00 ms |
| Vulkan | 45, 60 and 90 FPS | About 22.22, 16.67 and 11.11 ms |

The actual shipping TrayTemps host also capped a normal Vulkan target at 60 FPS. Earlier GTX 1050 tests exercised the preceding implementation; they are not a matched performance comparison with AMD.

These are functional fixture results. Real games, exclusive fullscreen, VRR, anti-cheat compatibility, input latency, true device loss and long-duration pacing remain outside this validation. x86 attachment and elevated Vulkan targets remain unsupported.
