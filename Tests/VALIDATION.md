# Release validation

## TrayTemps 2.5.0.0 — 2026-11-16

The current working tree builds in Debug and Release/x64. App version metadata is 2.5.0.0; native engine and protocol identities retain their independent versions. Release packaging produces the portable EXE, ZIP and SHA-256 checksums.

### Completed audit fixes

- Installation identity is checked before removal. Direct and deferred uninstall remove only the known executable and settings files, preserve unknown files/subdirectories and custom presets, and remove containing folders only when empty. Linked installation/settings directories are rejected.
- Installation uses a uniquely named, exclusively created write probe, preserving existing files such as test.tmp.
- Group priority follows its first configured member even when hidden; groups with no enabled members do not render empty rows.
- RAM polling in the temperature refresh requires enabled OSD and RAM metric. RAM-details live updates remain independent.
- Minimal transparent correctly captions its GPU group GPU:.
- Failed atomic settings replacement preserves the previous primary and the flushed recovery file; it no longer falls back to a non-atomic overwrite.
- Automatic limiter detection now accepts a visible custom-folder game with a recognized Steam/Epic game SDK and supported graphics runtime. Existing store/engine rules and exclusions remain in effect. Process/module inspection runs on a tracked worker; foreground, process start time and current profiles/exclusions are rechecked before automatic attachment. Inspection finishes during shutdown without holding up the UI thread.

Earlier OSD/preset fixes remain covered: deep-copy isolation, template 1 defaults with OSD disabled, independent 1% Low selection, editable labels/groups and per-metric styles, preset import/export and validation, Save/close behavior, preserved shortcuts, dynamic columns, spacing availability, stable numeric layout and complete values without ellipsis.

### Verification

| Check | Result |
| --- | --- |
| Debug build | Passed |
| Release/x64 solution build and packaging | Passed |
| Release safety suite | 21 checks passed in Debug and Release |
| General settings/hotkey/lifecycle integration | 105 checks passed in Debug and Release |
| OSD metrics/presets/rendering | 122 checks passed in Debug and Release |
| Frame-time/source/concurrency/performance | Passed, Release |
| Exhaustive numeric text fitting | 40 checks passed, Release |
| Limiter audit hardening | Passed, Release |
| Automatic game detection | 28 checks passed in Debug and Release |
| Live Onimusha automatic attachment/profile/exclusion/stop | 8 checks passed in Debug and Release |

Final release recheck (2026-11-16): the Release solution build and the detection, general integration, release-safety, OSD and limiter audit suites passed again. The ZIP contains only the executable, quick-start text and three license/notice files. Its executable matches dist/TrayTemps.exe byte-for-byte; file/assembly versions are 2.5.0.0 and both distribution hashes match SHA256SUMS.txt. The packaged quick-start now mentions disabled defaults, presets and executable-profile fallback. No loose test logs, stale audit links or packaging temporary files remain in the workspace.

Source release note: required new implementation, preset-resource and regression-test files are still untracked. Include them with the modified project files when committing this release; the successful build covers the complete working tree.

The safety suite executes the generated deferred-uninstall PowerShell script in temporary fixtures, including quoted paths, locked-executable retry, changed identity, unrelated-file preservation, script cleanup and failed settings replacement. It does not uninstall the user's app or alter preferences.

The preceding audit also passed 14 native hook-failure checks and Vulkan instance/device lifecycle and capacity-recovery tests. Capture and native hook code did not change in these fixes.

The live Onimusha process loaded steam_api64.dll and Direct3D 11/12 in its custom E:\GAMES folder. Automatic discovery attached without a saved profile at the requested 60 FPS default, applied an in-memory 75 FPS profile, respected exclusion, restored default attachment and disabled the cap at test shutdown. Preferences and game files were unchanged. These checks verify requested caps and successful hook initialization, not delivered FPS or frame-pacing quality.

### Remaining manual acceptance

Physical mixed-monitor DPI transitions, real-game MPO/VRR and exclusive-fullscreen behavior, anti-cheat compatibility, multi-hour resource stability and fault-injected disk failures require manual or dedicated environment testing. Synthetic timings do not guarantee identical game performance across hardware and drivers.

Regression fixtures and migration readers remain intentional project inputs. No public release was published by these checks.
