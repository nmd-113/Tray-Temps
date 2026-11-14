#include <Windows.h>
#define VK_NO_PROTOTYPES
#include <vulkan/vulkan.h>
#include <vulkan/vk_layer.h>
#include "include/TrayFpsProtocol.h"
#include <atomic>
#include <cstring>
#include <string>
#include <cwctype>

typedef VkResult (VKAPI_PTR *CreateInstanceFn)(const VkInstanceCreateInfo*, const VkAllocationCallbacks*, VkInstance*);
typedef VkResult (VKAPI_PTR *CreateDeviceFn)(VkPhysicalDevice, const VkDeviceCreateInfo*, const VkAllocationCallbacks*, VkDevice*);
typedef PFN_vkVoidFunction (VKAPI_PTR *GetInstanceProcAddrFn)(VkInstance,const char*);
typedef PFN_vkVoidFunction (VKAPI_PTR *GetDeviceProcAddrFn)(VkDevice,const char*);
typedef VkResult (VKAPI_PTR *QueuePresentFn)(VkQueue,const VkPresentInfoKHR*);
struct InstanceState { VkInstance instance; void* dispatch; GetInstanceProcAddrFn next; PFN_vkDestroyInstance destroy; };
struct DeviceState { VkDevice device; void* dispatch; GetDeviceProcAddrFn next; QueuePresentFn present; PFN_vkDestroyDevice destroy; };
static InstanceState g_instances[64] = {};
static DeviceState g_devices[128] = {};
static SRWLOCK g_lock = SRWLOCK_INIT;
static std::atomic<TrayFpsConfig*> g_globalConfig(nullptr);
struct GlobalConfigMapping {
    HANDLE handle = nullptr;
    ~GlobalConfigMapping() {
        auto config = g_globalConfig.load(std::memory_order_relaxed);
        if (config) UnmapViewOfFile(config);
        if (handle) CloseHandle(handle);
    }
};
static GlobalConfigMapping g_globalConfigMap;
static SRWLOCK g_configMapLock = SRWLOCK_INIT;
static std::atomic<DWORD> g_lastConfigRead(0);
static std::atomic<bool> g_processEligible(false);
extern "C" void TrayFpsUpdateConfig(TrayFpsConfig*);
extern "C" void TrayFpsBeforePresent(const void*);

// Copy entries under the lock; never retain a pointer into a reusable slot.
static GetInstanceProcAddrFn findInstance(VkInstance i) {
    GetInstanceProcAddrFn next = nullptr;
    AcquireSRWLockShared(&g_lock);
    for (const auto& x : g_instances) if (x.instance == i) { next = x.next; break; }
    ReleaseSRWLockShared(&g_lock);
    return next;
}
static InstanceState findInstanceByDispatch(void* key) {
    InstanceState state = {};
    AcquireSRWLockShared(&g_lock);
    for (const auto& x : g_instances) if (x.instance && x.dispatch == key) { state = x; break; }
    ReleaseSRWLockShared(&g_lock);
    return state;
}
static DeviceState findDevice(VkDevice device) {
    DeviceState state = {};
    AcquireSRWLockShared(&g_lock);
    for (const auto& x : g_devices) if (x.device == device) { state = x; break; }
    ReleaseSRWLockShared(&g_lock);
    return state;
}
static QueuePresentFn findPresent(VkQueue q) {
    void* key = *reinterpret_cast<void**>(q);
    QueuePresentFn present = nullptr;
    AcquireSRWLockShared(&g_lock);
    for (const auto& x : g_devices) if (x.device && x.dispatch == key) { present = x.present; break; }
    ReleaseSRWLockShared(&g_lock);
    return present;
}
static void VKAPI_CALL layerDestroyInstance(VkInstance instance, const VkAllocationCallbacks* alloc) {
    PFN_vkDestroyInstance destroy = nullptr;
    AcquireSRWLockExclusive(&g_lock);
    for (auto& entry : g_instances) if (entry.instance == instance) { destroy = entry.destroy; entry = {}; break; }
    ReleaseSRWLockExclusive(&g_lock);
    if (destroy) destroy(instance, alloc);
}
static void VKAPI_CALL layerDestroyDevice(VkDevice device, const VkAllocationCallbacks* alloc) {
    PFN_vkDestroyDevice destroy = nullptr;
    AcquireSRWLockExclusive(&g_lock);
    for (auto& entry : g_devices) if (entry.device == device) { destroy = entry.destroy; entry = {}; break; }
    ReleaseSRWLockExclusive(&g_lock);
    if (destroy) destroy(device, alloc);
}
static bool isExcludedProcess() {
    wchar_t path[MAX_PATH] = {}; if (!GetModuleFileNameW(nullptr, path, MAX_PATH)) return true;
    const wchar_t* base = wcsrchr(path, L'\\'); std::wstring name = base ? base + 1 : path; for (auto& c : name) c = static_cast<wchar_t>(towlower(c));
    static const wchar_t* excluded[] = { L"trayfps.exe", L"tray-temps.exe", L"chrome.exe", L"msedge.exe", L"firefox.exe", L"opera.exe", L"brave.exe", L"explorer.exe", L"dwm.exe", L"searchhost.exe", L"widgets.exe" };
    for (auto item : excluded) if (name == item) return true;
    return false;
}
static VkResult VKAPI_CALL layerQueuePresent(VkQueue q, const VkPresentInfoKHR* info) {
    auto next = findPresent(q);
    if (next && g_processEligible.load(std::memory_order_acquire) && info && info->sType == VK_STRUCTURE_TYPE_PRESENT_INFO_KHR) {
        DWORD now = GetTickCount();
        DWORD previous = g_lastConfigRead.load(std::memory_order_relaxed);
        if (static_cast<DWORD>(now - previous) >= 100 && g_lastConfigRead.compare_exchange_strong(previous, now)) {
            if (!g_globalConfig.load(std::memory_order_acquire)) {
                AcquireSRWLockExclusive(&g_configMapLock);
                if (!g_globalConfig.load(std::memory_order_relaxed)) {
                    HANDLE m = OpenFileMappingW(FILE_MAP_READ | FILE_MAP_WRITE, FALSE, L"Local\\TrayFPS.Config.Global");
                    if (m) {
                        auto config = static_cast<TrayFpsConfig*>(MapViewOfFile(m, FILE_MAP_READ | FILE_MAP_WRITE, 0, 0, sizeof(TrayFpsConfig)));
                        // Preserve the named config across controller exit/restart, as
                        // the layer may remain loaded in an already-running game.
                        if (config) { g_globalConfigMap.handle = m; g_globalConfig.store(config, std::memory_order_release); }
                        else CloseHandle(m);
                    }
                }
                ReleaseSRWLockExclusive(&g_configMapLock);
            }
            TrayFpsConfig* config = g_globalConfig.load(std::memory_order_acquire);
            if (config) TrayFpsUpdateConfig(config);
        }
        TrayFpsConfig* config = g_globalConfig.load(std::memory_order_acquire);
        DWORD targetPid = config ? static_cast<DWORD>(InterlockedCompareExchange(reinterpret_cast<volatile LONG*>(&config->targetPid), 0, 0)) : 0;
        if (targetPid == GetCurrentProcessId()) TrayFpsBeforePresent(q);
        return next(q, info);
    }
    return next ? next(q, info) : VK_ERROR_INITIALIZATION_FAILED;
}

static VkResult VKAPI_CALL layerCreateDevice(VkPhysicalDevice physical, const VkDeviceCreateInfo* ci, const VkAllocationCallbacks* alloc, VkDevice* out) {
    auto* link = const_cast<VkLayerDeviceCreateInfo*>(reinterpret_cast<const VkLayerDeviceCreateInfo*>(ci->pNext));
    while (link && (link->sType != VK_STRUCTURE_TYPE_LOADER_DEVICE_CREATE_INFO || link->function != VK_LAYER_LINK_INFO)) link = const_cast<VkLayerDeviceCreateInfo*>(reinterpret_cast<const VkLayerDeviceCreateInfo*>(link->pNext));
    if (!link || !link->u.pLayerInfo) return VK_ERROR_INITIALIZATION_FAILED;
    VkLayerDeviceLink* deviceLink = link->u.pLayerInfo;
    auto nextGdpa = deviceLink->pfnNextGetDeviceProcAddr;
    auto instanceState = findInstanceByDispatch(*reinterpret_cast<void**>(physical));
    auto instance = instanceState.instance;
    auto gipa = instanceState.next;
    auto create = gipa ? reinterpret_cast<CreateDeviceFn>(gipa(instance, "vkCreateDevice")) : nullptr;
    if (!create) return VK_ERROR_INITIALIZATION_FAILED;
    link->u.pLayerInfo = deviceLink->pNext;
    VkResult result = create(physical, ci, alloc, out);
    if (result == VK_SUCCESS && out && *out) {
        void* key = *reinterpret_cast<void**>(*out);
        auto present = reinterpret_cast<QueuePresentFn>(nextGdpa(*out, "vkQueuePresentKHR"));
        auto destroy = reinterpret_cast<PFN_vkDestroyDevice>(nextGdpa(*out, "vkDestroyDevice"));
        bool stored = false;
        AcquireSRWLockExclusive(&g_lock);
        for (auto& entry : g_devices) if (!entry.device) {
            entry = { *out, key, nextGdpa, present, destroy }; stored = true; break;
        }
        ReleaseSRWLockExclusive(&g_lock);
        if (!stored) { destroy(*out, alloc); *out = VK_NULL_HANDLE; return VK_ERROR_OUT_OF_HOST_MEMORY; }
    }
    return result;
}

static VkResult VKAPI_CALL layerCreateInstance(const VkInstanceCreateInfo* ci, const VkAllocationCallbacks* alloc, VkInstance* out) {
    auto* link = const_cast<VkLayerInstanceCreateInfo*>(reinterpret_cast<const VkLayerInstanceCreateInfo*>(ci->pNext));
    while (link && (link->sType != VK_STRUCTURE_TYPE_LOADER_INSTANCE_CREATE_INFO || link->function != VK_LAYER_LINK_INFO)) link = const_cast<VkLayerInstanceCreateInfo*>(reinterpret_cast<const VkLayerInstanceCreateInfo*>(link->pNext));
    if (!link || !link->u.pLayerInfo) return VK_ERROR_INITIALIZATION_FAILED;
    VkLayerInstanceLink* nextLink = link->u.pLayerInfo; auto next = nextLink->pfnNextGetInstanceProcAddr;
    auto create = reinterpret_cast<CreateInstanceFn>(next(VK_NULL_HANDLE, "vkCreateInstance")); if (!create) return VK_ERROR_INITIALIZATION_FAILED;
    link->u.pLayerInfo = nextLink->pNext;
    VkResult result = create(ci, alloc, out);
    if (result == VK_SUCCESS && out && *out) {
        g_processEligible.store(!isExcludedProcess(), std::memory_order_release);
        auto destroy = reinterpret_cast<PFN_vkDestroyInstance>(next(*out, "vkDestroyInstance"));
        bool stored = false;
        AcquireSRWLockExclusive(&g_lock);
        for (auto& entry : g_instances) if (!entry.instance) {
            entry = { *out, *reinterpret_cast<void**>(*out), next, destroy }; stored = true; break;
        }
        ReleaseSRWLockExclusive(&g_lock);
        if (!stored) { destroy(*out, alloc); *out = VK_NULL_HANDLE; return VK_ERROR_OUT_OF_HOST_MEMORY; }
    }
    return result;
}

extern "C" VKAPI_ATTR PFN_vkVoidFunction VKAPI_CALL TrayFpsGetDeviceProcAddr(VkDevice d, const char* name) {
    if (!d || !name) return nullptr;
    auto state = findDevice(d);
    if (!state.device) return nullptr;
    if (strcmp(name, "vkDestroyDevice") == 0) return reinterpret_cast<PFN_vkVoidFunction>(&layerDestroyDevice);
    if (strcmp(name, "vkQueuePresentKHR") == 0 && state.present) return reinterpret_cast<PFN_vkVoidFunction>(&layerQueuePresent);
    return state.next ? state.next(d, name) : nullptr;
}
extern "C" VKAPI_ATTR PFN_vkVoidFunction VKAPI_CALL TrayFpsGetInstanceProcAddr(VkInstance i, const char* name) {
    if (!name) return nullptr;
    if (strcmp(name,"vkGetInstanceProcAddr")==0) return reinterpret_cast<PFN_vkVoidFunction>(&TrayFpsGetInstanceProcAddr);
    if (strcmp(name,"vkGetDeviceProcAddr")==0) return reinterpret_cast<PFN_vkVoidFunction>(&TrayFpsGetDeviceProcAddr);
    if (strcmp(name,"vkCreateInstance")==0) return reinterpret_cast<PFN_vkVoidFunction>(&layerCreateInstance);
    if (strcmp(name,"vkCreateDevice")==0) return reinterpret_cast<PFN_vkVoidFunction>(&layerCreateDevice);
    if (strcmp(name,"vkDestroyInstance")==0) return reinterpret_cast<PFN_vkVoidFunction>(&layerDestroyInstance);
    if (strcmp(name,"vkDestroyDevice")==0) return reinterpret_cast<PFN_vkVoidFunction>(&layerDestroyDevice);
    if (strcmp(name,"vkQueuePresentKHR")==0) return reinterpret_cast<PFN_vkVoidFunction>(&layerQueuePresent);
    auto next = findInstance(i); return next ? next(i, name) : nullptr;
}
extern "C" VKAPI_ATTR VkResult VKAPI_CALL TrayFpsNegotiate(VkNegotiateLayerInterface* v) {
    if (!v || v->sType != LAYER_NEGOTIATE_INTERFACE_STRUCT) return VK_ERROR_INITIALIZATION_FAILED;
    if (v->loaderLayerInterfaceVersion > 2) v->loaderLayerInterfaceVersion = 2;
    v->pfnGetInstanceProcAddr = TrayFpsGetInstanceProcAddr; v->pfnGetDeviceProcAddr = TrayFpsGetDeviceProcAddr; return VK_SUCCESS;
}
