#include <Windows.h>
#include <d3d11.h>
#include <dxgi1_3.h>
#include <MinHook.h>
#include <atomic>
#include "include/TrayFpsProtocol.h"
#include "include/HookInstall.h"
extern "C" void TrayFpsUpdateConfig(TrayFpsConfig*);
extern "C" void TrayFpsBeforePresent(const void* stream);
extern "C" void TrayFpsStop();
extern "C" BOOL WINAPI TrayFpsHookD3D9(HWND);
extern "C" BOOL WINAPI TrayFpsHookOpenGL();

typedef HRESULT (STDMETHODCALLTYPE *PresentFn)(IDXGISwapChain*, UINT, UINT);
typedef HRESULT (STDMETHODCALLTYPE *Present1Fn)(IDXGISwapChain1*, UINT, UINT, const DXGI_PRESENT_PARAMETERS*);
static PresentFn g_present = nullptr;
static Present1Fn g_present1 = nullptr;
static TrayFpsConfig* g_config = nullptr;
static HANDLE g_configMap = nullptr;
static std::atomic<DWORD> g_lastConfigRead(0);
static thread_local bool g_insidePresentation = false;
static volatile LONG g_initialized = 0;
static SRWLOCK g_initializeLock = SRWLOCK_INIT;
static SRWLOCK g_configViewLock = SRWLOCK_INIT;
struct InitializeLock {
    InitializeLock() { AcquireSRWLockExclusive(&g_initializeLock); }
    ~InitializeLock() { ReleaseSRWLockExclusive(&g_initializeLock); }
};
static void releaseConfig() {
    AcquireSRWLockExclusive(&g_configViewLock);
    if (g_config) { UnmapViewOfFile(g_config); g_config = nullptr; }
    if (g_configMap) { CloseHandle(g_configMap); g_configMap = nullptr; }
    ReleaseSRWLockExclusive(&g_configViewLock);
}

static void refreshConfigOccasionally() {
    const DWORD tick = GetTickCount();
    DWORD previous = g_lastConfigRead.load(std::memory_order_relaxed);
    if (static_cast<DWORD>(tick - previous) < 100) return;
    if (!g_lastConfigRead.compare_exchange_strong(previous, tick, std::memory_order_relaxed)) return;
    AcquireSRWLockShared(&g_configViewLock);
    if (g_config) TrayFpsUpdateConfig(g_config);
    ReleaseSRWLockShared(&g_configViewLock);
}
extern "C" __declspec(dllexport) BOOL WINAPI TrayFpsBeginPresentation(const void* stream) {
    if (g_insidePresentation) return FALSE;
    g_insidePresentation = true;
    refreshConfigOccasionally();
    TrayFpsBeforePresent(stream);
    return TRUE;
}
extern "C" __declspec(dllexport) void WINAPI TrayFpsEndPresentation() { g_insidePresentation = false; }

static HRESULT STDMETHODCALLTYPE hookedPresent(IDXGISwapChain* chain, UINT interval, UINT flags) {
    const bool outer = !(flags & DXGI_PRESENT_TEST) && !(flags & DXGI_PRESENT_DO_NOT_WAIT) && TrayFpsBeginPresentation(chain);
    const HRESULT result = g_present(chain, interval, flags);
    if (outer) TrayFpsEndPresentation();
    return result;
}

static HRESULT STDMETHODCALLTYPE hookedPresent1(IDXGISwapChain1* chain, UINT interval, UINT flags, const DXGI_PRESENT_PARAMETERS* params) {
    const bool outer = !(flags & DXGI_PRESENT_TEST) && !(flags & DXGI_PRESENT_DO_NOT_WAIT) && TrayFpsBeginPresentation(chain);
    const HRESULT result = g_present1(chain, interval, flags, params);
    if (outer) TrayFpsEndPresentation();
    return result;
}

static LRESULT CALLBACK dummyWindowProc(HWND hwnd, UINT msg, WPARAM wp, LPARAM lp) { return DefWindowProcW(hwnd, msg, wp, lp); }

extern "C" __declspec(dllexport) DWORD WINAPI TrayFpsInitialize(void* requested) {
    InitializeLock lock;
    DWORD required = static_cast<DWORD>(reinterpret_cast<ULONG_PTR>(requested));
    LONG installed = InterlockedCompareExchange(&g_initialized, 0, 0);
    if (!required && installed) return installed;
    if (!required) required = 1;
    if (installed) return (installed & required) == required ? installed : 0;
    const DWORD pid = GetCurrentProcessId();
    wchar_t mapName[80];
    wsprintfW(mapName, L"Local\\TrayFPS.Config.%lu", pid);
    if (!g_config) {
        HANDLE map = OpenFileMappingW(FILE_MAP_READ | FILE_MAP_WRITE, FALSE, mapName);
        if (!map) return FALSE;
        auto config = static_cast<TrayFpsConfig*>(MapViewOfFile(map, FILE_MAP_READ | FILE_MAP_WRITE, 0, 0, sizeof(TrayFpsConfig)));
        if (!config || config->size != sizeof(TrayFpsConfig) || config->version != TRAYFPS_PROTOCOL_VERSION) {
            if (config) UnmapViewOfFile(config);
            CloseHandle(map);
            return FALSE;
        }
        // A mapped view alone does not preserve the name after all handles close.
        // Keep a handle so controller reconnects update the view used by our hooks.
        AcquireSRWLockExclusive(&g_configViewLock);
        g_configMap = map; g_config = config;
        ReleaseSRWLockExclusive(&g_configViewLock);
    }

    HINSTANCE instance = GetModuleHandleW(nullptr);
    WNDCLASSW wc = {}; wc.lpfnWndProc = dummyWindowProc; wc.hInstance = instance; wc.lpszClassName = L"TrayFPS.HookBootstrap";
    RegisterClassW(&wc);
    HWND hwnd = CreateWindowW(wc.lpszClassName, L"", WS_OVERLAPPEDWINDOW, 0, 0, 8, 8, nullptr, nullptr, instance, nullptr);
    if (!hwnd) { releaseConfig(); return 0; }
    MH_STATUS init = MH_Initialize();
    if (init != MH_OK && init != MH_ERROR_ALREADY_INITIALIZED) { DestroyWindow(hwnd); releaseConfig(); return 0; }
    DXGI_SWAP_CHAIN_DESC desc = {}; desc.BufferCount = 1; desc.BufferDesc.Width = 8; desc.BufferDesc.Height = 8;
    desc.BufferDesc.Format = DXGI_FORMAT_R8G8B8A8_UNORM; desc.BufferUsage = DXGI_USAGE_RENDER_TARGET_OUTPUT;
    desc.OutputWindow = hwnd; desc.SampleDesc.Count = 1; desc.Windowed = TRUE; desc.SwapEffect = DXGI_SWAP_EFFECT_DISCARD;
    IDXGISwapChain* chain = nullptr; ID3D11Device* device = nullptr; ID3D11DeviceContext* context = nullptr; D3D_FEATURE_LEVEL level;
    HRESULT hr = D3D11CreateDeviceAndSwapChain(nullptr, D3D_DRIVER_TYPE_HARDWARE, nullptr, 0, nullptr, 0, D3D11_SDK_VERSION,
        &desc, &chain, &device, &level, &context);
    if (FAILED(hr)) hr = D3D11CreateDeviceAndSwapChain(nullptr, D3D_DRIVER_TYPE_WARP, nullptr, 0, nullptr, 0, D3D11_SDK_VERSION,
        &desc, &chain, &device, &level, &context);
    IDXGISwapChain1* chain1 = nullptr;
    void* present1Target = nullptr;
    if (SUCCEEDED(hr) && SUCCEEDED(chain->QueryInterface(__uuidof(IDXGISwapChain1), reinterpret_cast<void**>(&chain1)))) {
        present1Target = (*reinterpret_cast<void***>(chain1))[22];
    }
    bool a = chain && TrayFpsInstallHook((*reinterpret_cast<void***>(chain))[8], &hookedPresent, reinterpret_cast<void**>(&g_present));
    bool b = present1Target && TrayFpsInstallHook(present1Target, &hookedPresent1, reinterpret_cast<void**>(&g_present1));
    DWORD adapters = (a || b) ? 1u : 0u;
    if (TrayFpsHookD3D9(hwnd)) adapters |= 2u;
    if (TrayFpsHookOpenGL()) adapters |= 4u;
    if (chain1) chain1->Release();
    if (chain) chain->Release();
    if (context) context->Release();
    if (device) device->Release();
    DestroyWindow(hwnd);
    if ((adapters & required) == required) { InterlockedExchange(&g_initialized, adapters); return adapters; }
    MH_DisableHook(MH_ALL_HOOKS);
    MH_Uninitialize();
    releaseConfig();
    TrayFpsStop();
    return 0;
}

extern "C" __declspec(dllexport) void WINAPI TrayFpsShutdown() {
    InitializeLock lock;
    MH_DisableHook(MH_ALL_HOOKS); MH_Uninitialize();
    InterlockedExchange(&g_initialized, 0);
    releaseConfig();
    TrayFpsStop();
}
