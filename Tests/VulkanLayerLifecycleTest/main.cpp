// Exercise the real layer against a controlled next layer. No GPU, injection,
// registry registration, or application-data access is required.
#include "../../Native/FpsLimiter.Native/vulkan_layer.cpp"
#include <cstdio>
#include <cstdlib>

extern "C" void TrayFpsUpdateConfig(TrayFpsConfig*) {}
extern "C" void TrayFpsBeforePresent(const void*) {}

struct FakeHandle { void* dispatch; };
static FakeHandle instances[512] = {}, devices[1024] = {};
static size_t instanceCount = 0, deviceCount = 0;
static int destroyedInstances = 0, destroyedDevices = 0, presentations = 0;
static int sharedDeviceDispatch;
static const VkAllocationCallbacks allocator = {};

static void Check(bool condition, const char* message) {
    if (!condition) { std::printf("FAIL: %s\n", message); std::exit(1); }
}
static void VKAPI_CALL destroyInstance(VkInstance, const VkAllocationCallbacks* alloc) {
    Check(alloc == &allocator, "instance allocator forwarded"); ++destroyedInstances;
}
static void VKAPI_CALL destroyDevice(VkDevice, const VkAllocationCallbacks* alloc) {
    Check(alloc == &allocator, "device allocator forwarded"); ++destroyedDevices;
}
static VkResult VKAPI_CALL createInstance(const VkInstanceCreateInfo*, const VkAllocationCallbacks*, VkInstance* out) {
    Check(instanceCount < 512, "instance fixture capacity");
    auto* handle = &instances[instanceCount++]; handle->dispatch = handle;
    *out = reinterpret_cast<VkInstance>(handle); return VK_SUCCESS;
}
static VkResult VKAPI_CALL createDevice(VkPhysicalDevice, const VkDeviceCreateInfo*, const VkAllocationCallbacks*, VkDevice* out) {
    Check(deviceCount < 1024, "device fixture capacity");
    auto* handle = &devices[deviceCount++]; handle->dispatch = &sharedDeviceDispatch;
    *out = reinterpret_cast<VkDevice>(handle); return VK_SUCCESS;
}
static VkResult VKAPI_CALL present(VkQueue, const VkPresentInfoKHR*) { ++presentations; return VK_SUCCESS; }
static void VKAPI_CALL forwardedFunction() {}
static PFN_vkVoidFunction VKAPI_CALL nextInstance(VkInstance, const char* name) {
    if (strcmp(name, "vkCreateInstance") == 0) return reinterpret_cast<PFN_vkVoidFunction>(createInstance);
    if (strcmp(name, "vkCreateDevice") == 0) return reinterpret_cast<PFN_vkVoidFunction>(createDevice);
    if (strcmp(name, "vkDestroyInstance") == 0) return reinterpret_cast<PFN_vkVoidFunction>(destroyInstance);
    return reinterpret_cast<PFN_vkVoidFunction>(forwardedFunction);
}
static PFN_vkVoidFunction VKAPI_CALL nextDevice(VkDevice, const char* name) {
    if (strcmp(name, "vkDestroyDevice") == 0) return reinterpret_cast<PFN_vkVoidFunction>(destroyDevice);
    if (strcmp(name, "vkQueuePresentKHR") == 0) return reinterpret_cast<PFN_vkVoidFunction>(present);
    return reinterpret_cast<PFN_vkVoidFunction>(forwardedFunction);
}
static VkResult NewInstance(VkInstance* out) {
    VkLayerInstanceLink link = {}; link.pfnNextGetInstanceProcAddr = nextInstance;
    VkLayerInstanceCreateInfo layer = {}; layer.sType = VK_STRUCTURE_TYPE_LOADER_INSTANCE_CREATE_INFO;
    layer.function = VK_LAYER_LINK_INFO; layer.u.pLayerInfo = &link;
    VkInstanceCreateInfo info = {}; info.sType = VK_STRUCTURE_TYPE_INSTANCE_CREATE_INFO; info.pNext = &layer;
    auto create = reinterpret_cast<PFN_vkCreateInstance>(TrayFpsGetInstanceProcAddr(VK_NULL_HANDLE, "vkCreateInstance"));
    return create(&info, &allocator, out);
}
static VkResult NewDevice(VkInstance instance, VkDevice* out) {
    VkLayerDeviceLink link = {}; link.pfnNextGetInstanceProcAddr = nextInstance; link.pfnNextGetDeviceProcAddr = nextDevice;
    VkLayerDeviceCreateInfo layer = {}; layer.sType = VK_STRUCTURE_TYPE_LOADER_DEVICE_CREATE_INFO;
    layer.function = VK_LAYER_LINK_INFO; layer.u.pLayerInfo = &link;
    VkDeviceCreateInfo info = {}; info.sType = VK_STRUCTURE_TYPE_DEVICE_CREATE_INFO; info.pNext = &layer;
    auto create = reinterpret_cast<PFN_vkCreateDevice>(TrayFpsGetInstanceProcAddr(instance, "vkCreateDevice"));
    return create(reinterpret_cast<VkPhysicalDevice>(instance), &info, &allocator, out);
}
static void DeleteInstance(VkInstance instance) {
    auto destroy = reinterpret_cast<PFN_vkDestroyInstance>(TrayFpsGetInstanceProcAddr(instance, "vkDestroyInstance"));
    Check(destroy != nullptr, "instance destruction entrypoint exists"); destroy(instance, &allocator);
}
static void DeleteDevice(VkInstance instance, VkDevice device, bool viaInstance = false) {
    auto destroy = reinterpret_cast<PFN_vkDestroyDevice>(viaInstance
        ? TrayFpsGetInstanceProcAddr(instance, "vkDestroyDevice") : TrayFpsGetDeviceProcAddr(device, "vkDestroyDevice"));
    Check(destroy != nullptr, "device destruction entrypoint exists"); destroy(device, &allocator);
}

int main() {
    for (int n = 0; n < 192; ++n) {
        VkInstance instance = VK_NULL_HANDLE;
        Check(NewInstance(&instance) == VK_SUCCESS, "instance recreation beyond original capacity");
        Check(TrayFpsGetInstanceProcAddr(instance, "vkExample") != nullptr, "live instance forwarding");
        DeleteInstance(instance);
        Check(destroyedInstances == n + 1, "instance destruction forwards exactly once");
        Check(TrayFpsGetInstanceProcAddr(instance, "vkExample") == nullptr, "destroyed instance is untracked");
    }
    std::puts("PASS: 192 distinct instance create/destroy cycles");

    VkInstance instance = VK_NULL_HANDLE;
    Check(NewInstance(&instance) == VK_SUCCESS, "device test instance");
    for (int n = 0; n < 384; ++n) {
        VkDevice device = VK_NULL_HANDLE;
        Check(NewDevice(instance, &device) == VK_SUCCESS, "device recreation beyond original capacity");
        Check(TrayFpsGetDeviceProcAddr(device, "vkExample") != nullptr, "live device forwarding");
        DeleteDevice(instance, device, (n & 1) != 0);
        Check(destroyedDevices == n + 1, "device destruction forwards exactly once");
        Check(TrayFpsGetDeviceProcAddr(device, "vkExample") == nullptr, "destroyed device is untracked");
    }
    std::puts("PASS: 384 distinct device cycles through both destruction entrypoints");

    VkDevice first = VK_NULL_HANDLE, second = VK_NULL_HANDLE;
    Check(NewDevice(instance, &first) == VK_SUCCESS && NewDevice(instance, &second) == VK_SUCCESS, "shared-dispatch devices created");
    DeleteDevice(instance, first);
    auto queuePresent = reinterpret_cast<PFN_vkQueuePresentKHR>(TrayFpsGetDeviceProcAddr(second, "vkQueuePresentKHR"));
    g_processEligible.store(false); // Only check forwarding; never open global configuration.
    Check(queuePresent && queuePresent(reinterpret_cast<VkQueue>(second), nullptr) == VK_SUCCESS && presentations == 1,
        "destroying one device preserves another device sharing its dispatch table");
    DeleteDevice(instance, second);
    std::puts("PASS: shared dispatch remains valid after sibling device destruction");

    VkDevice liveDevices[128] = {};
    for (auto& device : liveDevices) Check(NewDevice(instance, &device) == VK_SUCCESS, "fill device table");
    VkDevice overflowDevice = VK_NULL_HANDLE;
    int before = destroyedDevices;
    Check(NewDevice(instance, &overflowDevice) == VK_ERROR_OUT_OF_HOST_MEMORY && !overflowDevice && destroyedDevices == before + 1,
        "device overflow destroys the downstream object and returns an error");
    DeleteDevice(instance, liveDevices[0]);
    Check(NewDevice(instance, &liveDevices[0]) == VK_SUCCESS, "device slot reusable after table exhaustion");
    for (auto device : liveDevices) DeleteDevice(instance, device);
    DeleteInstance(instance);
    std::puts("PASS: device capacity failure and recovery");

    VkInstance liveInstances[64] = {};
    for (auto& item : liveInstances) Check(NewInstance(&item) == VK_SUCCESS, "fill instance table");
    VkInstance overflowInstance = VK_NULL_HANDLE;
    before = destroyedInstances;
    Check(NewInstance(&overflowInstance) == VK_ERROR_OUT_OF_HOST_MEMORY && !overflowInstance && destroyedInstances == before + 1,
        "instance overflow destroys the downstream object and returns an error");
    DeleteInstance(liveInstances[0]);
    Check(NewInstance(&liveInstances[0]) == VK_SUCCESS, "instance slot reusable after table exhaustion");
    for (auto item : liveInstances) DeleteInstance(item);
    std::puts("PASS: instance capacity failure and recovery");
    Check(static_cast<size_t>(destroyedInstances) == instanceCount && static_cast<size_t>(destroyedDevices) == deviceCount,
        "every downstream object destroyed exactly once");
    std::puts("ALL VULKAN LIFECYCLE CHECKS PASSED");
}
