# Testing TrayTemps

Build `Tray-Temps.sln` in Visual Studio. The managed test harness and native lifecycle test are built automatically; interactive tests are not run by the build.

See [VALIDATION.md](VALIDATION.md) for the current build and compatibility summary. Raw logs, screenshots and recovery files are local diagnostics, not repository content.

## Focused checks

From a Developer PowerShell after a Release/x64 build:

```powershell
.\Tests\bin\Release\TrayTemps.FpsLimiterTests.exe --audit-fixes
.\Tests\VulkanLayerLifecycleTest\x64\Release\VulkanLayerLifecycleTest.exe
```

The audit checks exercise spaced exclusions and cache cleanup in isolated temporary folders. They do not enable the limiter or change user preferences or Vulkan registrations.

The native test runs the production layer against a controlled next layer: instance/device recreation, shared dispatch and table-capacity failure/recovery. It requires no GPU, registration or injection.

## Integration and graphics

Run `Tests/bin/Release/TrayTemps.FpsLimiterTests.exe` without arguments for settings, UI, theme, hotkeys, ownership and host lifecycle checks. The harness bypasses normal hardware/PawnIO startup. Runtime checks temporarily register the per-user Vulkan layer and clean up on shutdown.

Optional positional arguments add controlled graphics checks:

1. A TrayFPS `tests` directory containing built x64 Release D3D9, D3D11, D3D12 and OpenGL fixtures.
2. The absolute path to a built Vulkan graphics fixture.

These cover cap enforcement, OFF/ON, live changes, shutdown pass-through, profiles and exclusions. OpenGL uses 30/20 FPS; other fixtures use 60/90 FPS. The executable profile and exclusion tests require an interactive desktop. Close existing TrayTemps/TrayFPS instances and keep keyboard/mouse idle while foreground tests run.

`Tests/VulkanGraphicsTest` and `Tests/ReviewTargets` contain optional fixture projects, separate from the normal solution. The Vulkan graphics fixture requires SDK headers/import libraries; the x86 fixture references the original TrayFPS D3D11 test source through `TrayFpsTestsRoot`. The x86 check verifies safe rejection, not x86 attachment support.

## Review modes

`TrayTemps.FpsLimiterTests.exe --review <mode>` provides additional diagnostics:

- `unit`, `startup`: lifecycle, settings, UI geometry and clean-folder startup combinations.
- `capture`, `ownership`: shortcut capture and exclusive engine ownership.
- `preflight`, `x86`, `vulkan`: candidate rejection, target-exit recovery and Vulkan restart behavior.
- `mixed-vulkan`, `host-vulkan-60`: elevated controller/host with normal Vulkan targets.
- `compare`, `pacing`, `steady`: controlled comparisons requiring the original baseline binaries.
- `interactive`, `cancel`, `cancel-cap`, `pending-exit`, `hardware`, `dpi`, `dpi-roundtrip`, `finish-dpi`, `host-vulkan`, `real-osd`: machine-specific or manual acceptance procedures.

Paths and prerequisites are defined in `ReviewTests.cs` and `HandoverTests.cs`; some modes require the original standalone fixtures or supplied Unity targets. They are developer diagnostics, not portable unattended CI tests. Real-process modes back up preferences, restore them on scope exit and retain recovery copies in their temporary evidence folder. Unknown modes fail explicitly.

Release solution builds refresh `dist` automatically. Packaging can be checked by copying only the shipping EXE and test EXE to a clean folder and running the focused checks there; no sidecar DLLs should be needed.

Controlled presentation intervals establish functional behavior, not input latency or broad real-game/driver compatibility. Consult [VALIDATION.md](VALIDATION.md) for the tested scope and remaining limitations.
