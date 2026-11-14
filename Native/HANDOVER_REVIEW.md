# FPS limiter handover summary

The earlier implementation was exercised on a GTX 1050 / Windows 10 host. Later controlled graphics and shipping-host checks were performed on a Radeon RX 7800 XT / Windows 11 host. These are compatibility observations, not a matched GPU performance comparison.

The current [validation summary](../Tests/VALIDATION.md) covers solution builds, payload staging, lifecycle regressions and graphics fixtures. The [native build guide](README.md) describes the automatic Visual Studio workflow, and the [changelog](../CHANGELOG.md) lists release changes.

Known limits remain: x86 attachment and elevated Vulkan targets are unsupported. Controlled tests do not establish real-game, anti-cheat, exclusive-fullscreen, VRR or input-latency compatibility.

Historical raw logs, desktop captures, intermediate build hashes and patch exports have been archived outside the source tree. This page retains the original documentation link.
