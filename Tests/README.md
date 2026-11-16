# Testing TrayTemps

Build `Tray-Temps.sln` in Visual Studio. The managed test harness and native lifecycle test are built automatically; interactive tests are not run by the build.

See [VALIDATION.md](VALIDATION.md) for the current build and compatibility summary. Raw logs, screenshots and recovery files are local diagnostics, not repository content.

## Focused checks

From a Developer PowerShell after a Release/x64 build:

```powershell
.\Tests\bin\Release\TrayTemps.FpsLimiterTests.exe --audit-fixes
.\Tests\bin\Release\TrayTemps.FpsLimiterTests.exe --release-safety
.\Tests\bin\Release\TrayTemps.FpsLimiterTests.exe --game-detection
.\Tests\VulkanLayerLifecycleTest\x64\Release\VulkanLayerLifecycleTest.exe
```

The audit checks exercise exclusions, cache cleanup, transactional candidate rejection (refusal, timeout, hook failure and exit), retry suppression, next-target recovery, full-path module identity and deferred loader-buffer cleanup. They use a mock injector, temporary payloads and child processes; they do not inject hooks, change user preferences or register Vulkan layers.

The release-safety checks cover verified installation cleanup, preservation of unknown files and custom presets, locked-executable retries, safe write probes and atomic settings recovery, stable group ordering with hidden metrics, OSD-only RAM polling, and embedded preset captions. All filesystem fixtures are temporary; installers, user preferences and real games are not changed.

The game-detection checks cover custom-folder Steam/Epic SDK games, supported rendering runtimes, ordinary graphics applications, existing engine/store recognition, exclusions, delayed renderer loading, process identity and foreground changes, and shutdown while inspection is pending.

The optional `--game-detection-live <pid>` mode requires that game to remain in the foreground. It attaches the limiter, requests a 60 FPS default and a temporary 75 FPS profile, verifies exclusion and reattachment, then disables the cap in `finally`. It saves no preferences, changes no game files and does not activate the game window. Use this mode only for an intended live game check with other limiter instances closed. Its assertions verify hook attachment and requested cap values; they do not measure delivered frame pacing.

Build `Tests/HookFailureTest/HookFailureTest.vcxproj` for Release/x64 and run `Tests/HookFailureTest/x64/Release/HookFailureTest.exe` for simulated MinHook creation/activation failures against the production adapters. This separate test uses the installed graphics runtime for bootstrap, installs no hooks, and checks API-specific results, partial DXGI success, rollback and the D3D9 swap-chain hook.

The native test runs the production layer against a controlled next layer: instance/device recreation, shared dispatch and table-capacity failure/recovery. It requires no GPU, registration or injection.

## Integration and graphics

The --metric-list mode checks three-column metric editing, arbitrary groups, optional captions, group/member ordering, the five-column appearance grid, migration/JSON/deep-copy isolation, independent low selection, native group double-click behavior, Unicode separator insertion, themes, scaling, mixed-font baselines, exact whitespace advances, stable numeric slots and color-only repaint behavior. It saves local UI/OSD screenshots without changing user preferences.

It also checks the Presets page, template 1 defaults with OSD disabled, embedded preset isolation, custom JSON round trips and overwrite, extraction from full settings files, rejection of unrelated JSON, repeated appearance refresh, preserved OSD enable state and shortcut, the existing preview/Save behavior, legacy label editing, migration ordering, active-edit preview suppression, failed overwrite cleanup, oversized input/output rejection, and minimum-size Presets layout.

For the optional FPS 1% low metric, run `TrayTemps.FpsLimiterTests.exe --one-percent-low`. This checks the slowest-1% average, 100-interval warm-up, ten-second expiry, stale gaps, source/generation resets, concurrent capture/sorting, overflow handling, and reports capture/sort timings at 60/144/1000/9999 FPS. Use `--metric-list` separately for the OSD metric selection, grouping, appearance, Save/Cancel, and rendering UI checks. These timings are local microbenchmarks, not a guarantee for all games.

`TrayTemps.FpsLimiterTests.exe --one-percent-text` exhaustively measures values 0–9999 plus the unavailable `N/A` value in JetBrains Mono ExtraBold/Regular, Segoe UI, Arial and Consolas at 16/24/32/48 pt. It also verifies that fitting into constrained widths preserves every glyph of the low label and value, without substituting an ellipsis. This covers the mixed-digit bearing regression (e.g. `6007` at 32 pt exceeded the old repeated-digit slot by one pixel).

`TrayTemps.FpsLimiterTests.exe --one-percent-live <absolute-D3D11-fixture-path>` exercises the existing ETW session against a real foreground graphics fixture, verifies standalone low display with FPS hidden and the limiter disabled, refresh across separate hardware groups, and stable geometry, option OFF and OSD OFF/ON cleanup/warm-up. Keep keyboard/mouse idle while it selects the fixture. It creates local logs and an OSD screenshot without changing user settings. MPO ON/OFF, exclusive fullscreen, VRR and real-game comparisons remain manual acceptance checks; this test does not change MPO settings.

Run `Tests/bin/Release/TrayTemps.FpsLimiterTests.exe` without arguments for settings, UI, theme, hotkeys, ownership and host lifecycle checks. The harness bypasses normal hardware/PawnIO startup. Runtime checks temporarily register the per-user Vulkan layer and clean up on shutdown.

These checks also cover exact OFF cleanup after manifest deletion, stale registrations at OFF startup, ownership release and preservation of unrelated manifest names. Custom toggle shortcut checks cover old-settings defaults, serialization, normalization, registration replacement, occupied bindings, window recreation, Save/Cancel, and capturing existing FPS/OSD shortcuts without toggling either feature. The default toggle shortcut is Ctrl+Shift+F11.

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

`--review mixed-vulkan <absolute-fixture-path>` overrides the historical fixture location. It verifies caps, OFF pass-through, absence of the layer in a new Vulkan process after OFF, OFF/ON reuse, and elevated-candidate rejection. Use this mode when the harness is elevated: the ordinary Vulkan graphics mode inherits elevation, and elevated Vulkan targets are intentionally unsupported.

Release solution builds refresh `dist` automatically. Packaging can be checked by copying only the shipping EXE and test EXE to a clean folder and running the focused checks there; no sidecar DLLs should be needed.

Controlled presentation intervals establish functional behavior, not input latency or broad real-game/driver compatibility. Consult [VALIDATION.md](VALIDATION.md) for the tested scope and remaining limitations.
