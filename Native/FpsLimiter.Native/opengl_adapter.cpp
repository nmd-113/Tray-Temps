#include <Windows.h>
#include <gl/GL.h>
#include <MinHook.h>
extern "C" BOOL WINAPI TrayFpsBeginPresentation(const void*);
extern "C" void WINAPI TrayFpsEndPresentation();
typedef BOOL (WINAPI *SwapBuffersFn)(HDC);
static SwapBuffersFn g_swapBuffers = nullptr;
static BOOL WINAPI hookSwapBuffers(HDC dc) {
    BOOL outer = wglGetCurrentContext() != nullptr && TrayFpsBeginPresentation(dc);
    BOOL result = g_swapBuffers(dc);
    if (outer) TrayFpsEndPresentation();
    return result;
}
extern "C" BOOL WINAPI TrayFpsHookOpenGL() {
    HMODULE gdi = GetModuleHandleW(L"gdi32.dll"); if (!gdi) gdi = LoadLibraryW(L"gdi32.dll"); if (!gdi) return FALSE;
    FARPROC target = GetProcAddress(gdi, "SwapBuffers"); if (!target) return FALSE;
    MH_STATUS s = MH_CreateHook(target, &hookSwapBuffers, reinterpret_cast<void**>(&g_swapBuffers));
    if (s == MH_OK || s == MH_ERROR_ALREADY_CREATED) MH_EnableHook(target);
    return s == MH_OK || s == MH_ERROR_ALREADY_CREATED;
}
