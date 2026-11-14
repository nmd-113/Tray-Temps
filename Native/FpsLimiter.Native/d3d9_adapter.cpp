#include <Windows.h>
#include <d3d9.h>
#include <MinHook.h>
extern "C" BOOL WINAPI TrayFpsBeginPresentation(const void*);
extern "C" void WINAPI TrayFpsEndPresentation();
typedef HRESULT (STDMETHODCALLTYPE *Present9Fn)(IDirect3DDevice9*, const RECT*, const RECT*, HWND, const RGNDATA*);
typedef HRESULT (STDMETHODCALLTYPE *PresentExFn)(IDirect3DDevice9Ex*, const RECT*, const RECT*, HWND, const RGNDATA*, DWORD);
static Present9Fn g_present9 = nullptr; static PresentExFn g_presentEx = nullptr;
static HRESULT STDMETHODCALLTYPE hookPresent9(IDirect3DDevice9* d, const RECT* a, const RECT* b, HWND w, const RGNDATA* r) {
    BOOL outer = TrayFpsBeginPresentation(d); HRESULT result = g_present9(d, a, b, w, r); if (outer) TrayFpsEndPresentation(); return result;
}
static HRESULT STDMETHODCALLTYPE hookPresentEx(IDirect3DDevice9Ex* d, const RECT* a, const RECT* b, HWND w, const RGNDATA* r, DWORD flags) {
    BOOL outer = !(flags & D3DPRESENT_DONOTWAIT) && TrayFpsBeginPresentation(d); HRESULT result = g_presentEx(d, a, b, w, r, flags); if (outer) TrayFpsEndPresentation(); return result;
}
static bool makeDevice(HWND hwnd, IDirect3DDevice9** device) {
    IDirect3D9* api = Direct3DCreate9(D3D_SDK_VERSION); if (!api) return false;
    D3DPRESENT_PARAMETERS p = {}; p.BackBufferWidth = 8; p.BackBufferHeight = 8; p.BackBufferFormat = D3DFMT_X8R8G8B8; p.BackBufferCount = 1; p.SwapEffect = D3DSWAPEFFECT_DISCARD; p.hDeviceWindow = hwnd; p.Windowed = TRUE;
    HRESULT hr = api->CreateDevice(D3DADAPTER_DEFAULT, D3DDEVTYPE_HAL, hwnd, D3DCREATE_SOFTWARE_VERTEXPROCESSING, &p, device);
    if (FAILED(hr)) hr = api->CreateDevice(D3DADAPTER_DEFAULT, D3DDEVTYPE_REF, hwnd, D3DCREATE_SOFTWARE_VERTEXPROCESSING, &p, device);
    api->Release(); return SUCCEEDED(hr);
}
extern "C" BOOL WINAPI TrayFpsHookD3D9(HWND hwnd) {
    IDirect3DDevice9* d = nullptr; if (!makeDevice(hwnd, &d)) return FALSE;
    void** v = *reinterpret_cast<void***>(d); MH_STATUS s = MH_CreateHook(v[17], &hookPresent9, reinterpret_cast<void**>(&g_present9));
    if (s == MH_OK || s == MH_ERROR_ALREADY_CREATED) MH_EnableHook(v[17]); d->Release();
    IDirect3D9Ex* apiEx = nullptr; IDirect3DDevice9Ex* devEx = nullptr;
    if (SUCCEEDED(Direct3DCreate9Ex(D3D_SDK_VERSION, &apiEx))) {
        D3DPRESENT_PARAMETERS p = {}; p.BackBufferWidth = 8; p.BackBufferHeight = 8; p.BackBufferFormat = D3DFMT_X8R8G8B8; p.BackBufferCount = 1; p.SwapEffect = D3DSWAPEFFECT_DISCARD; p.hDeviceWindow = hwnd; p.Windowed = TRUE;
        if (SUCCEEDED(apiEx->CreateDeviceEx(D3DADAPTER_DEFAULT, D3DDEVTYPE_HAL, hwnd, D3DCREATE_SOFTWARE_VERTEXPROCESSING, &p, nullptr, &devEx))) {
            void** exV = *reinterpret_cast<void***>(devEx); MH_STATUS ex = MH_CreateHook(exV[121], &hookPresentEx, reinterpret_cast<void**>(&g_presentEx));
            if (ex == MH_OK || ex == MH_ERROR_ALREADY_CREATED) MH_EnableHook(exV[121]); devEx->Release();
        }
        apiEx->Release();
    }
    return s == MH_OK || s == MH_ERROR_ALREADY_CREATED;
}
