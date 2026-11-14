#define VK_USE_PLATFORM_WIN32_KHR
#define WIN32_LEAN_AND_MEAN
#define NOMINMAX
#include <Windows.h>
#include <vulkan/vulkan.h>
#include <algorithm>
#include <cmath>
#include <cstdio>
#include <numeric>
#include <vector>

static LRESULT CALLBACK WindowProc(HWND window, UINT message, WPARAM w, LPARAM l) { return DefWindowProcW(window, message, w, l); }
static void Check(VkResult result) { if (result != VK_SUCCESS && result != VK_SUBOPTIMAL_KHR) { std::printf("Vulkan error %d\n", result); std::exit(3); } }

int main()
{
    HINSTANCE application = GetModuleHandleW(nullptr);
    WNDCLASSW wc = {}; wc.hInstance = application; wc.lpszClassName = L"TrayTemps.Vulkan.Test"; wc.lpfnWndProc = WindowProc;
    RegisterClassW(&wc);
    HWND window = CreateWindowW(wc.lpszClassName, L"TrayTemps Vulkan controlled target", WS_OVERLAPPEDWINDOW | WS_VISIBLE, 100, 100, 800, 600, nullptr, nullptr, application, nullptr);
    if (!window) return 2;
    const char* extensions[] = { VK_KHR_SURFACE_EXTENSION_NAME, VK_KHR_WIN32_SURFACE_EXTENSION_NAME };
    VkInstanceCreateInfo instanceInfo = {}; instanceInfo.sType = VK_STRUCTURE_TYPE_INSTANCE_CREATE_INFO; instanceInfo.enabledExtensionCount = 2; instanceInfo.ppEnabledExtensionNames = extensions;
    VkInstance instance; Check(vkCreateInstance(&instanceInfo, nullptr, &instance));
    VkWin32SurfaceCreateInfoKHR surfaceInfo = {}; surfaceInfo.sType = VK_STRUCTURE_TYPE_WIN32_SURFACE_CREATE_INFO_KHR; surfaceInfo.hinstance = application; surfaceInfo.hwnd = window;
    VkSurfaceKHR surface; Check(vkCreateWin32SurfaceKHR(instance, &surfaceInfo, nullptr, &surface));
    uint32_t count = 0; Check(vkEnumeratePhysicalDevices(instance, &count, nullptr));
    std::vector<VkPhysicalDevice> devices(count); Check(vkEnumeratePhysicalDevices(instance, &count, devices.data()));
    VkPhysicalDevice physical = VK_NULL_HANDLE; uint32_t family = 0;
    for (auto candidate : devices) {
        uint32_t n = 0; vkGetPhysicalDeviceQueueFamilyProperties(candidate, &n, nullptr);
        std::vector<VkQueueFamilyProperties> properties(n); vkGetPhysicalDeviceQueueFamilyProperties(candidate, &n, properties.data());
        for (uint32_t i = 0; i < n; ++i) { VkBool32 present = false; Check(vkGetPhysicalDeviceSurfaceSupportKHR(candidate, i, surface, &present)); if (present && (properties[i].queueFlags & VK_QUEUE_GRAPHICS_BIT)) { physical = candidate; family = i; break; } }
        if (physical) break;
    }
    if (!physical) return 4;
    float priority = 1;
    VkDeviceQueueCreateInfo queueInfo = {}; queueInfo.sType = VK_STRUCTURE_TYPE_DEVICE_QUEUE_CREATE_INFO; queueInfo.queueFamilyIndex = family; queueInfo.queueCount = 1; queueInfo.pQueuePriorities = &priority;
    const char* deviceExtension = VK_KHR_SWAPCHAIN_EXTENSION_NAME;
    VkDeviceCreateInfo deviceInfo = {}; deviceInfo.sType = VK_STRUCTURE_TYPE_DEVICE_CREATE_INFO; deviceInfo.queueCreateInfoCount = 1; deviceInfo.pQueueCreateInfos = &queueInfo; deviceInfo.enabledExtensionCount = 1; deviceInfo.ppEnabledExtensionNames = &deviceExtension;
    VkDevice device; Check(vkCreateDevice(physical, &deviceInfo, nullptr, &device));
    VkQueue queue; vkGetDeviceQueue(device, family, 0, &queue);
    VkSurfaceCapabilitiesKHR capabilities; Check(vkGetPhysicalDeviceSurfaceCapabilitiesKHR(physical, surface, &capabilities));
    Check(vkGetPhysicalDeviceSurfaceFormatsKHR(physical, surface, &count, nullptr)); std::vector<VkSurfaceFormatKHR> formats(count); Check(vkGetPhysicalDeviceSurfaceFormatsKHR(physical, surface, &count, formats.data()));
    Check(vkGetPhysicalDeviceSurfacePresentModesKHR(physical, surface, &count, nullptr)); std::vector<VkPresentModeKHR> modes(count); Check(vkGetPhysicalDeviceSurfacePresentModesKHR(physical, surface, &count, modes.data()));
    VkSwapchainCreateInfoKHR swapInfo = {}; swapInfo.sType = VK_STRUCTURE_TYPE_SWAPCHAIN_CREATE_INFO_KHR; swapInfo.surface = surface;
    swapInfo.minImageCount = capabilities.minImageCount; swapInfo.imageFormat = formats[0].format == VK_FORMAT_UNDEFINED ? VK_FORMAT_B8G8R8A8_UNORM : formats[0].format; swapInfo.imageColorSpace = formats[0].colorSpace;
    swapInfo.imageExtent = capabilities.currentExtent; if (swapInfo.imageExtent.width == UINT32_MAX) swapInfo.imageExtent = { 800, 600 };
    swapInfo.imageArrayLayers = 1; swapInfo.imageUsage = VK_IMAGE_USAGE_COLOR_ATTACHMENT_BIT; swapInfo.imageSharingMode = VK_SHARING_MODE_EXCLUSIVE; swapInfo.preTransform = capabilities.currentTransform;
    swapInfo.compositeAlpha = VK_COMPOSITE_ALPHA_OPAQUE_BIT_KHR; swapInfo.clipped = VK_TRUE;
    swapInfo.presentMode = std::find(modes.begin(), modes.end(), VK_PRESENT_MODE_IMMEDIATE_KHR) != modes.end() ? VK_PRESENT_MODE_IMMEDIATE_KHR : VK_PRESENT_MODE_FIFO_KHR;
    VkSwapchainKHR swap; Check(vkCreateSwapchainKHR(device, &swapInfo, nullptr, &swap));
    Check(vkGetSwapchainImagesKHR(device, swap, &count, nullptr)); std::vector<VkImage> images(count); Check(vkGetSwapchainImagesKHR(device, swap, &count, images.data())); std::vector<bool> initialized(count, false);
    VkCommandPoolCreateInfo poolInfo = {}; poolInfo.sType = VK_STRUCTURE_TYPE_COMMAND_POOL_CREATE_INFO; poolInfo.queueFamilyIndex = family; poolInfo.flags = VK_COMMAND_POOL_CREATE_RESET_COMMAND_BUFFER_BIT;
    VkCommandPool pool; Check(vkCreateCommandPool(device, &poolInfo, nullptr, &pool));
    VkCommandBufferAllocateInfo allocation = {}; allocation.sType = VK_STRUCTURE_TYPE_COMMAND_BUFFER_ALLOCATE_INFO; allocation.commandPool = pool; allocation.level = VK_COMMAND_BUFFER_LEVEL_PRIMARY; allocation.commandBufferCount = 1;
    VkCommandBuffer command; Check(vkAllocateCommandBuffers(device, &allocation, &command));
    VkSemaphoreCreateInfo semaphoreInfo = {}; semaphoreInfo.sType = VK_STRUCTURE_TYPE_SEMAPHORE_CREATE_INFO;
    VkSemaphore acquired, rendered; Check(vkCreateSemaphore(device, &semaphoreInfo, nullptr, &acquired)); Check(vkCreateSemaphore(device, &semaphoreInfo, nullptr, &rendered));
    LARGE_INTEGER frequency, previous, now; QueryPerformanceFrequency(&frequency); QueryPerformanceCounter(&previous);
    auto end = GetTickCount64() + 16000, report = GetTickCount64() + 1000; std::vector<double> samples;
    while (GetTickCount64() < end) {
        MSG message; while (PeekMessageW(&message, nullptr, 0, 0, PM_REMOVE)) { TranslateMessage(&message); DispatchMessageW(&message); }
        uint32_t index; Check(vkAcquireNextImageKHR(device, swap, UINT64_MAX, acquired, VK_NULL_HANDLE, &index));
        Check(vkResetCommandBuffer(command, 0)); VkCommandBufferBeginInfo begin = {}; begin.sType = VK_STRUCTURE_TYPE_COMMAND_BUFFER_BEGIN_INFO; Check(vkBeginCommandBuffer(command, &begin));
        if (!initialized[index]) {
            VkImageMemoryBarrier barrier = {}; barrier.sType = VK_STRUCTURE_TYPE_IMAGE_MEMORY_BARRIER; barrier.oldLayout = VK_IMAGE_LAYOUT_UNDEFINED; barrier.newLayout = VK_IMAGE_LAYOUT_PRESENT_SRC_KHR; barrier.srcQueueFamilyIndex = barrier.dstQueueFamilyIndex = VK_QUEUE_FAMILY_IGNORED; barrier.image = images[index]; barrier.subresourceRange = { VK_IMAGE_ASPECT_COLOR_BIT, 0, 1, 0, 1 };
            vkCmdPipelineBarrier(command, VK_PIPELINE_STAGE_TOP_OF_PIPE_BIT, VK_PIPELINE_STAGE_BOTTOM_OF_PIPE_BIT, 0, 0, nullptr, 0, nullptr, 1, &barrier); initialized[index] = true;
        }
        Check(vkEndCommandBuffer(command)); VkPipelineStageFlags wait = VK_PIPELINE_STAGE_TOP_OF_PIPE_BIT;
        VkSubmitInfo submit = {}; submit.sType = VK_STRUCTURE_TYPE_SUBMIT_INFO; submit.waitSemaphoreCount = 1; submit.pWaitSemaphores = &acquired; submit.pWaitDstStageMask = &wait; submit.commandBufferCount = 1; submit.pCommandBuffers = &command; submit.signalSemaphoreCount = 1; submit.pSignalSemaphores = &rendered;
        Check(vkQueueSubmit(queue, 1, &submit, VK_NULL_HANDLE));
        VkPresentInfoKHR present = {}; present.sType = VK_STRUCTURE_TYPE_PRESENT_INFO_KHR; present.waitSemaphoreCount = 1; present.pWaitSemaphores = &rendered; present.swapchainCount = 1; present.pSwapchains = &swap; present.pImageIndices = &index;
        Check(vkQueuePresentKHR(queue, &present)); Check(vkQueueWaitIdle(queue));
        QueryPerformanceCounter(&now); samples.push_back(1000.0 * (now.QuadPart - previous.QuadPart) / frequency.QuadPart); previous = now;
        if (GetTickCount64() >= report) {
            auto sorted = samples; std::sort(sorted.begin(), sorted.end());
            std::printf("frames=%zu mean_ms=%.3f p95_ms=%.3f p99_ms=%.3f api=Vulkan\n", samples.size(), std::accumulate(samples.begin(), samples.end(), 0.0) / samples.size(), sorted[(sorted.size()-1)*95/100], sorted[(sorted.size()-1)*99/100]); std::fflush(stdout); samples.clear(); report = GetTickCount64() + 1000;
        }
    }
    Check(vkDeviceWaitIdle(device)); vkDestroySemaphore(device, acquired, nullptr); vkDestroySemaphore(device, rendered, nullptr); vkDestroyCommandPool(device, pool, nullptr); vkDestroySwapchainKHR(device, swap, nullptr); vkDestroyDevice(device, nullptr); vkDestroySurfaceKHR(instance, surface, nullptr); vkDestroyInstance(instance, nullptr); DestroyWindow(window);
    return 0;
}
