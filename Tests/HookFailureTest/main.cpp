// Run the production adapters with controlled MinHook outcomes. No hooks are
// installed and no registry values or other processes are touched.
#include <Windows.h>
#include <MinHook.h>
#include <vector>
#include <cstdio>
#include <cstdlib>
static std::vector<void*> targets;
static unsigned failCreate = 0, failEnable = 0;
static int removes = 0, disables = 0, uninitializes = 0;
static int paced = 0, forwarded = 0;
MH_STATUS WINAPI MH_Initialize() { targets.clear(); return MH_OK; }
MH_STATUS WINAPI MH_Uninitialize() { ++uninitializes; return MH_OK; }
MH_STATUS WINAPI MH_CreateHook(LPVOID target, LPVOID, LPVOID* original) {
    targets.push_back(target);
    if (failCreate & (1u << (targets.size() - 1))) return MH_ERROR_UNSUPPORTED_FUNCTION;
    *original = target;
    return MH_OK;
}
MH_STATUS WINAPI MH_EnableHook(LPVOID target) {
    for (size_t i = 0; i < targets.size(); ++i)
        if (targets[i] == target && (failEnable & (1u << i))) return MH_ERROR_MEMORY_PROTECT;
    return MH_OK;
}
MH_STATUS WINAPI MH_RemoveHook(LPVOID) { ++removes; return MH_OK; }
MH_STATUS WINAPI MH_DisableHook(LPVOID) { ++disables; return MH_OK; }
extern "C" void TrayFpsUpdateConfig(struct TrayFpsConfig*) { }
extern "C" void TrayFpsBeforePresent(const void*) { ++paced; }
extern "C" void TrayFpsStop() { }
#include "../../Native/FpsLimiter.Native/dxgi_adapter.cpp"
#include "../../Native/FpsLimiter.Native/d3d9_adapter.cpp"
#include "../../Native/FpsLimiter.Native/opengl_adapter.cpp"

static HRESULT STDMETHODCALLTYPE fakePresent9(IDirect3DDevice9*, const RECT*, const RECT*, HWND, const RGNDATA*) { ++forwarded; return S_OK; }
static HRESULT STDMETHODCALLTYPE nestedSwapChainPresent(IDirect3DSwapChain9*, const RECT*, const RECT*, HWND, const RGNDATA*, DWORD) {
    return hookPresent9(nullptr, nullptr, nullptr, nullptr, nullptr);
}
static HRESULT STDMETHODCALLTYPE fakeSwapChainPresent(IDirect3DSwapChain9*, const RECT*, const RECT*, HWND, const RGNDATA*, DWORD) { ++forwarded; return S_OK; }

static void check(bool ok, const char* label) {
    if (!ok) { std::fprintf(stderr, "FAIL: %s\n", label); std::exit(1); }
    std::printf("PASS: %s\n", label);
}
static DWORD initialize(DWORD required, unsigned createFailures, unsigned enableFailures) {
    TrayFpsShutdown();
    failCreate = createFailures; failEnable = enableFailures;
    removes = disables = uninitializes = 0;
    return TrayFpsInitialize(reinterpret_cast<void*>(static_cast<ULONG_PTR>(required)));
}
int main() {
    wchar_t name[80]; wsprintfW(name, L"Local\\TrayFPS.Config.%lu", GetCurrentProcessId());
    HANDLE map = CreateFileMappingW(INVALID_HANDLE_VALUE, nullptr, PAGE_READWRITE, 0, sizeof(TrayFpsConfig), name);
    auto config = static_cast<TrayFpsConfig*>(MapViewOfFile(map, FILE_MAP_ALL_ACCESS, 0, 0, sizeof(TrayFpsConfig)));
    check(config != nullptr, "isolated bootstrap mapping created");
    config->size = sizeof(TrayFpsConfig); config->version = TRAYFPS_PROTOCOL_VERSION;
    check((initialize(1, 0, 0) & 1) != 0, "normal DXGI activation succeeds");
    check((initialize(1, 1, 0) & 1) != 0, "Present1 can succeed when Present creation fails");
    check((initialize(1, 2, 0) & 1) != 0, "Present can succeed when Present1 creation fails");
    check(initialize(1, 3, 0) == 0 && uninitializes == 1 && disables == 1 && g_config == nullptr,
        "other APIs cannot mask missing DXGI hooks; failed initialization rolls back");
    check(initialize(1, 0, 3) == 0 && removes == 2 && uninitializes == 1,
        "DXGI enable failures remove created hooks and report failure");
    check((initialize(2, 3, 0) & 2) != 0, "D3D9 succeeds independently of DXGI");
    check(targets.size() >= 6 && g_swapChainPresent != nullptr, "D3D9 swap-chain Present is included");
    check(initialize(2, 0, 8) == 0 && removes == 1 && uninitializes == 1,
        "D3D9 swap-chain activation failure is rejected and rolled back");
    check((initialize(4, 3, 0) & 4) != 0, "OpenGL succeeds independently of DXGI");
    const size_t hookCount = targets.size();
    check(TrayFpsInitialize(nullptr) != 0 && targets.size() == hookCount, "reattachment reuses initialized adapters");
    check(initialize(4, 0, 32) == 0 && removes == 1 && uninitializes == 1,
        "OpenGL enable failure is rejected and rolled back");
    g_present9 = fakePresent9; g_swapChainPresent = nestedSwapChainPresent;
    paced = forwarded = 0;
    hookSwapChainPresent(nullptr, nullptr, nullptr, nullptr, nullptr, 0);
    check(paced == 1 && forwarded == 1 && !g_insidePresentation, "direct D3D9 swap-chain presentation paces once even with nested device Present");
    g_swapChainPresent = fakeSwapChainPresent;
    paced = forwarded = 0;
    hookSwapChainPresent(nullptr, nullptr, nullptr, nullptr, nullptr, D3DPRESENT_DONOTWAIT);
    check(paced == 0 && forwarded == 1, "D3D9 swap-chain DONOTWAIT forwards without pacing");
    TrayFpsShutdown(); UnmapViewOfFile(config); CloseHandle(map);
    std::puts("ALL HOOK FAILURE CHECKS PASSED");
}
