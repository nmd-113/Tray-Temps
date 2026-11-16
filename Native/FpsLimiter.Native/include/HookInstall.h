#pragma once
#include <MinHook.h>

// Only report hooks whose trampoline and activation both succeeded. A failed
// activation must not leave a partial installation behind for the next attempt.
inline bool TrayFpsInstallHook(void* target, void* detour, void** original) {
    if (!target || MH_CreateHook(target, detour, original) != MH_OK) return false;
    if (MH_EnableHook(target) == MH_OK) return true;
    MH_RemoveHook(target);
    return false;
}
