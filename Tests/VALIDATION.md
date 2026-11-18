# Release validation

## Final audit fixes — 2026-11-18

Hardware refreshes now reuse displayed frame values and graphs until the selected OSD interval expires, while history keeps recording current samples. Source changes and appearance previews refresh immediately. Oversized OSD layouts fit uniformly within the working area, including large margins and column gaps. Empty OSD settings sections use neutral defaults; nonempty legacy appearance settings retain their migration behavior. An open history window synchronizes its range and recording controls after reset. Timing buckets and statistics scans are skipped for 1% Low-only capture, and timing toggles retain its samples. Removed unused history target-name state and Designer trailing whitespace.

Standard Debug and Release/x64 builds passed without project compiler warnings. Release passed 1,159 checks across general integration, limiter safety, game detection, presets/metrics, spacing, history, UI, numeric text fitting, frame timing, native hook failures and Vulkan lifecycle. Debug passed 1,030 checks across the affected managed suites. The original four audit probes now reproduce the corrected behavior; saved user settings remain byte-for-byte unchanged. Height/width fitting covers three fonts at simulated 96/144/192 DPI. Live-game pacing, physical monitor transitions and long-duration resource stability remain manual checks.

Final recheck: an independent checkout exported from the staged source built with restore and passed all 1,159 Release checks, including both native suites. Both release archives contain only the five intended shipping files; their executable entries and SHA-256 checksums match. Required source/test files are included in the staged changes, with no untracked leftovers or whitespace-check failures. No commit was created.

## UI and history recheck — 2026-11-17

Confirmed and fixed graph colors changing with other metric selections, weak light-theme legend contrast, graph text overlap with larger fonts, unthemed Clear hover colors, an FPS-gap control enabled for frametime-only selection, stale FPS-only captions/tooltips, oversized history windows opening outside smaller working areas, and failure to restore a minimized history window from the main icon. Window size is saved on normal resize. Disabling a metric now inserts one missing sample so later data cannot bridge that inactive period; frametime recording remains active when its graph is selected. Invalid peaks fall back to their valid mean.

Debug and Release builds passed without warnings. Both passed 88 UI checks, 54 history checks, metric-list/presets (117), spacing (141), frame-time, release safety (21) and general integration (105). Release also passed text-fit, audit fixes and game detection (28). Dark/light captures cover all four OSD pages at minimum/larger sizes and history at 600×400, 900×700 and 1400×900. Checks cover field centers, label fit, equal graph widths, scrolling, color stability, legend contrast, hover colors and disabled controls. Graph title/legend/axis layout was checked with 8.25/12 pt fonts at simulated 96/120/144/192 DPI. Both live D3D11 suites passed, including background history with the window closed and shared capture cleanup. Physical monitor handover, native desktop chrome and other games remain environment-specific manual checks.

## Frame metrics and performance history — 2026-11-17

Added opt-in average FPS, frametime and a standalone frametime graph, sharing the existing ETW stream and source resets. Average FPS uses accepted intervals over ten seconds with a one-second warm-up. Frametime uses the latest refresh-window intervals; anchoring that short window to the last accepted timestamp avoids false N/A values from delayed ETW batches. The graph retains 100 ms means and peaks. New metrics and history recording default to off; old enum IDs, settings and embedded presets retain their behavior.

History uses bounded, lazily allocated per-metric rings covering at most 30 minutes at 250 ms collection. CPU/GPU temperatures and usage use existing hardware refreshes; enabled numeric OSD metrics supply additional series. Current Time is represented by the axis. Recording remains active with the history window closed or OSD off, without a second ETW session or FPS limiter. Numeric reads allocate no memory; graph snapshots are skipped with OSD off. Visible history refreshes at 500 ms, preserves peaks during pixel reduction, and breaks lines for missing readings and changed sources. Recording preferences, range and size are saved; sample data remains in memory only. History capture does not increase the configured OSD refresh rate.

Debug and Release/x64 solution builds passed without warnings. Both configurations passed history (45), metric-list/presets (117), OSD spacing (141), frame-time and general integration (105) checks; release safety also passed in both. Release text-fit, audit fixes, game detection (28) and native Vulkan lifecycle checks passed. History graphs were rendered at simulated 96/120/144/192 DPI, and dark/light screenshots were reviewed. The optional live D3D11 history test passed nine checks for actual timing values, closed-window recording, preserving the external target while viewing history, OSD graph data, shared capture and final shutdown. Existing live 1% Low checks passed too. Tests bypass normal hardware/PawnIO startup and do not save user preferences.

One Release replay measured capture at 9.4 ns/frame with optional timing off and 22.8 ns/frame with it on, with zero Gen0 collections; 1% Low sorting took approximately 0.017/0.045/0.306/2.923 ms at 60/144/1000/9999 FPS. These are local microbenchmarks, not a guarantee of zero impact in every game. Physical DPI handover, real-game/MPO behavior and long gaming sessions remain manual acceptance checks; 30-minute bounds were checked with synthetic samples. Release EXE/ZIP hashes match SHA256SUMS.txt. Removed the obsolete low-only capture configuration helper; diagnostics remain outside the repository.

## OSD spacing and simplified group controls — 2026-11-17

Reproduced an 8 GB memory template using one integer digit while 16 GB used two, and approximately 12–13 pixels of font-leading gap between sampled rows at zero spacing/96 DPI. Memory slots now reserve at least two integer digits and retain three or more for capacities of 100 GB and above. Zero spacing measures glyph bounds with outline/antialias clearance, including mixed fonts, Unicode fallback and width fitting. Numeric updates retain measured layouts; transitions to/from N/A remeasure only compact rows so missing-data glyphs cannot clip or leave permanent padding. Positive row spacing retains the existing font-cell height and logical gap.

Metrics → Group supplies each group's caption with the same case-insensitive membership, ordering and first-configured spelling as the priority tree. Removed the Layout group-caption/separator controls, events and obsolete separator drawing logic. Old GroupLabels/GroupSeparator JSON fields remain loadable but are ignored for rendering. Numeric values retain leading padding inside their fixed template slots. Ordinary grouped metrics use one built-in space between slots; the FPS/1% Low pair retains four spaces including its existing padding and the dedicated label/value gap.

Debug/Release builds passed. The spacing suite passed 141 checks in both configurations, covering 8/16 GB group alignment, larger capacities, Segoe UI/JetBrains Mono/Consolas at simulated renderer and bitmap DPI of 96/144/192, mixed sizes, accents/descenders/icons, N/A transitions, numeric layout reuse, positive spacing, narrow and multiple-column layouts, group-name captions and legacy settings/presets. The updated OSD suite passed 117 checks, general integration 105, and release safety 21 in both configurations; Release text-fit and frame-time suites passed too. Layout and compact-row screenshots were reviewed. Distribution hashes match SHA256SUMS.txt; version remains 2.5.0.0. No removed control/handler references remain in application code.

Physical cross-monitor DPI handover and the other PC remain manual acceptance checks. Earlier 2026-11-16 results below describe the UI before this simplification.

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

Earlier OSD/preset fixes remain covered: deep-copy isolation, neutral fresh defaults with OSD disabled, independent 1% Low selection, editable labels/groups and per-metric styles, preset import/export and validation, Save/close behavior, preserved shortcuts, dynamic columns, spacing availability, stable numeric layout and complete values without ellipsis.

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

The safety suite executes the generated deferred-uninstall PowerShell script in temporary fixtures, including quoted paths, locked-executable retry, changed identity, unrelated-file preservation, script cleanup and failed settings replacement. It does not uninstall the user's app or alter preferences.

The preceding audit also passed 14 native hook-failure checks and Vulkan instance/device lifecycle and capacity-recovery tests. Capture and native hook code did not change in these fixes.

The live Onimusha process loaded steam_api64.dll and Direct3D 11/12 in its custom E:\GAMES folder. Automatic discovery attached without a saved profile at the requested 60 FPS default, applied an in-memory 75 FPS profile, respected exclusion, restored default attachment and disabled the cap at test shutdown. Preferences and game files were unchanged. These checks verify requested caps and successful hook initialization, not delivered FPS or frame-pacing quality.

### Remaining manual acceptance

Physical mixed-monitor DPI transitions, real-game MPO/VRR and exclusive-fullscreen behavior, anti-cheat compatibility, multi-hour resource stability and fault-injected disk failures require manual or dedicated environment testing. Synthetic timings do not guarantee identical game performance across hardware and drivers.

Regression fixtures and migration readers remain intentional project inputs. No public release was published by these checks.
