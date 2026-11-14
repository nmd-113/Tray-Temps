# FPS limiter native engine

The limiter consists of `FpsLimiter.Core`, the host service in `Helpers/Performance`, and the native presentation adapters in `FpsLimiter.Native`. The OSD FPS counter is a separate ETW-based feature.

## Normal build

Open `Tray-Temps.sln` in Visual Studio 2022 and build the solution. Install the usual .NET desktop and Desktop development with C++ workloads, including v143 tools, a Windows SDK and the .NET Framework 4.8 Developer Pack.

The solution builds the native x64 engine, builds its x86 counterpart, stages both DLLs under `Resources/FpsLimiter`, and updates the payload hashes and engine cache version automatically. The application waits for this dependency before embedding the payloads. The managed controller and test projects are also solution dependencies.

Release builds produce the portable `bin\x64\Release\TrayTemps.exe` and automatically refresh the EXE, ZIP and checksums in `dist`. Debug builds use the same optimized native engine and do not overwrite the release package. No separate build, staging or packaging command is required.

The pinned C headers in `ThirdParty/Vulkan-Headers` replace the external Vulkan SDK prerequisite. The native build uses the installed Windows SDK. Native intermediates live under `Native/obj`, with DLL build output under `Native/out/Release`; these generated folders are ignored by Git. The staged resource DLLs, manifest and version field are versioned. Unchanged payloads are not rewritten, and reproducible linking avoids timestamp-only DLL changes on rebuilds.

`Native/BuildFpsLimiter.ps1` remains available for standalone native diagnostics; `-Stage` stages that standalone build. `Native/StageFpsLimiter.ps1` is the shared staging/validation implementation used by the normal solution build.

The application is x64. Building the x86 payload does not establish cross-architecture game attachment support.

## Validation and history

- [Test instructions](../Tests/README.md).
- [Build, runtime and AMD validation](../Tests/VALIDATION.md).
- [Changelog](../CHANGELOG.md).

The short integration and handover notes summarize the feature history. Obsolete implementation plans, patch exports and raw diagnostics are not build inputs and are kept out of the source repository.

MinHook and Vulkan-Headers are retained under `ThirdParty` with their upstream licenses. The bundled [FPS limiter notices](../Resources/FpsLimiter/NOTICE.txt) include the third-party attributions.
