using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading;
using Microsoft.Win32;

namespace FpsLimiter.Core
{
    public enum LimiterStatus { Disabled, WaitingForGame, Attached, RestartRequired, AccessDenied }

    // Controller owns settings/session policy. No per-frame managed callbacks.
    public sealed class LimiterController : IDisposable
    {
        private MemoryMappedFile map, globalMap; private MemoryMappedViewAccessor view, globalView;
        private readonly object configGate = new object(); private Timer heartbeatTimer;
        private int activeProcessId;
        private DateTime vulkanLayerPreparedAtUtc = DateTime.MinValue;
        private bool vulkanLayerPrepared;
        private const uint ProtocolVersion = 1;
        public LimiterStatus Status { get; private set; } = LimiterStatus.Disabled;
        public event EventHandler StatusChanged;
        private void SetStatus(LimiterStatus s) { Status = s; var cb = StatusChanged; if (cb != null) cb(this, EventArgs.Empty); }

        public void Start() { if (Status == LimiterStatus.Disabled) SetStatus(LimiterStatus.WaitingForGame); }
        public void PrepareVulkan(int fps, string nativeDll)
        {
            if (fps < 10 || fps > 1000) throw new ArgumentOutOfRangeException(nameof(fps));
            StartGlobalVulkan(fps, nativeDll);
        }
        public void Apply(int processId, int fps, bool enabled, string nativeDll)
        {
            if (fps < 10 || fps > 1000) throw new ArgumentOutOfRangeException(nameof(fps));
            if (activeProcessId == processId && view != null)
            {
                WriteConfig(processId, fps, enabled);
                SetStatus(GetTargetStatus(processId, enabled));
                return;
            }
            if (view != null) ClearTarget();
            using (var process = Process.GetProcessById(processId))
            {
                bool x64 = Is64Bit(process.Handle);
                string dll = Path.Combine(nativeDll, x64 ? "x64" : "x86", "FpsLimiter.Native.dll");
                if (!File.Exists(dll)) throw new FileNotFoundException("Native module for target architecture is missing.", dll);
                string name = "Local\\TrayFPS.Config." + processId;
                var security = new MemoryMappedFileSecurity();
                security.AddAccessRule(new AccessRule<MemoryMappedFileRights>(WindowsIdentity.GetCurrent().User, MemoryMappedFileRights.ReadWrite, AccessControlType.Allow));
                map = MemoryMappedFile.CreateOrOpen(name, 32, MemoryMappedFileAccess.ReadWrite, MemoryMappedFileOptions.None, security, HandleInheritability.None);
                view = map.CreateViewAccessor(0, 32, MemoryMappedFileAccess.ReadWrite);
                try { if (enabled) StartGlobalVulkan(fps, nativeDll); WriteConfig(processId, fps, enabled); Inject(process, dll); }
                catch { Stop(); throw; }
                activeProcessId = processId;
                SetStatus(GetTargetStatus(processId, enabled));
            }
        }
        private void WriteConfig(int pid, int fps, bool enabled)
        {
            lock (configGate) {
                WriteView(view, pid, fps, enabled);
                if (globalView != null) WriteGlobalConfig(pid, fps, enabled);
            }
            if (enabled) EnsureHeartbeat();
            else if (heartbeatTimer != null) { heartbeatTimer.Dispose(); heartbeatTimer = null; }
        }
        private void EnsureHeartbeat()
        {
            if (heartbeatTimer == null) heartbeatTimer = new Timer(_ => { lock (configGate) { if (view != null) { view.Write(20, Environment.TickCount); view.Flush(); } if (globalView != null) { globalView.Write(20, Environment.TickCount); globalView.Flush(); } } }, null, 500, 500);
        }
        private static void WriteView(MemoryMappedViewAccessor target, int pid, int fps, bool enabled)
        {
            long seq = target.ReadInt32(8); if ((seq & 1) != 0) seq++;
            target.Write(8, (int)(seq + 1)); target.Write(0, 32u); target.Write(4, ProtocolVersion); target.Write(12, enabled ? 1 : 0); target.Write(16, (uint)pid); target.Write(20, Environment.TickCount); target.Write(24, (double)fps); target.Write(8, (int)(seq + 2)); target.Flush();
        }
        private void StartGlobalVulkan(int fps, string nativeRoot)
        {
            lock (configGate)
            {
                if (globalMap == null) { globalMap = MemoryMappedFile.CreateOrOpen("Local\\TrayFPS.Config.Global", 32, MemoryMappedFileAccess.ReadWrite); globalView = globalMap.CreateViewAccessor(0, 32, MemoryMappedFileAccess.ReadWrite); }
                WriteGlobalConfig(0, fps, true);
                EnsureHeartbeat();
            }
            string manifestRoot = Path.GetDirectoryName(nativeRoot);
            RegisterLayer(nativeRoot, Path.Combine(manifestRoot, "TrayFPS-Vulkan-x64.json"), RegistryView.Registry64, "x64");
            if (Environment.Is64BitOperatingSystem) RegisterLayer(nativeRoot, Path.Combine(manifestRoot, "TrayFPS-Vulkan-x86.json"), RegistryView.Registry32, "x86");
            if (!vulkanLayerPrepared) { vulkanLayerPreparedAtUtc = DateTime.UtcNow; vulkanLayerPrepared = true; }
        }
        private void WriteGlobalConfig(int targetPid, int fps, bool enabled) { WriteView(globalView, targetPid, fps, enabled); }
        private LimiterStatus GetTargetStatus(int processId, bool enabled)
        {
            if (!enabled) return LimiterStatus.Disabled;
            if (FindRemoteModule(processId, "vulkan-1.dll") == IntPtr.Zero) return LimiterStatus.Attached;
            try { using (var process = Process.GetProcessById(processId)) return process.StartTime.ToUniversalTime() < vulkanLayerPreparedAtUtc ? LimiterStatus.RestartRequired : LimiterStatus.Attached; }
            catch (ArgumentException) { return LimiterStatus.RestartRequired; }
            catch (InvalidOperationException) { return LimiterStatus.RestartRequired; }
            catch (Win32Exception) { return LimiterStatus.RestartRequired; }
        }
        public void DetachTarget()
        {
            ClearTarget();
            SetStatus(globalView != null ? LimiterStatus.WaitingForGame : LimiterStatus.Disabled);
        }
        public void Pause()
        {
            if (heartbeatTimer != null) { heartbeatTimer.Dispose(); heartbeatTimer = null; }
            ClearTarget();
            lock (configGate)
            {
                if (globalView != null)
                {
                    double fps = globalView.ReadDouble(24);
                    WriteGlobalConfig(0, fps >= 10.0 && fps <= 1000.0 ? (int)fps : 60, false);
                }
            }
            SetStatus(LimiterStatus.Disabled);
        }
        private void ClearTarget()
        {
            lock (configGate)
            {
                if (view != null) { WriteView(view, 0, 60, false); view.Dispose(); view = null; }
                if (map != null) { map.Dispose(); map = null; }
                if (globalView != null)
                {
                    int enabled = globalView.ReadInt32(12); double fps = globalView.ReadDouble(24);
                    WriteGlobalConfig(0, (int)fps, enabled != 0);
                }
                activeProcessId = 0;
            }
        }
        private static void RegisterLayer(string nativeRoot, string manifest, RegistryView view, string arch)
        {
            string dll = Path.Combine(nativeRoot, arch, "FpsLimiter.Native.dll");
            string json = "{\"file_format_version\":\"1.2.0\",\"layer\":{\"name\":\"VK_LAYER_TRAYFPS_limiter\",\"type\":\"GLOBAL\",\"library_path\":\"" + dll.Replace("\\", "\\\\") + "\",\"api_version\":\"1.3.0\",\"implementation_version\":\"1\",\"description\":\"TrayFPS frame limiter\",\"disable_environment\":{\"TRAYFPS_DISABLE_VULKAN_LAYER\":\"\"},\"functions\":{\"vkGetInstanceProcAddr\":\"vkGetInstanceProcAddr\",\"vkGetDeviceProcAddr\":\"vkGetDeviceProcAddr\",\"vkNegotiateLoaderLayerInterfaceVersion\":\"vkNegotiateLoaderLayerInterfaceVersion\"}}}";
            File.WriteAllText(manifest, json);
            using (var root = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, view)) using (var key = root.CreateSubKey("SOFTWARE\\Khronos\\Vulkan\\ImplicitLayers")) key.SetValue(manifest, 0, RegistryValueKind.DWord);
        }
        private static void UnregisterLayer(string manifest, RegistryView view)
        {
            using (var root = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, view)) using (var key = root.OpenSubKey("SOFTWARE\\Khronos\\Vulkan\\ImplicitLayers", true)) if (key != null) key.DeleteValue(manifest, false);
        }
        private static void CleanupOwnLayerRegistrations()
        {
            foreach (var view in Environment.Is64BitOperatingSystem ? new[] { RegistryView.Registry64, RegistryView.Registry32 } : new[] { RegistryView.Registry64 })
            using (var root = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, view))
            using (var key = root.OpenSubKey("SOFTWARE\\Khronos\\Vulkan\\ImplicitLayers", true))
            {
                if (key == null) continue;
                foreach (string value in key.GetValueNames())
                {
                    if (!Path.GetFileName(value).StartsWith("TrayFPS-Vulkan-", StringComparison.OrdinalIgnoreCase)) continue;
                    try { if (File.ReadAllText(value).IndexOf("\"name\":\"VK_LAYER_TRAYFPS_limiter\"", StringComparison.Ordinal) >= 0) key.DeleteValue(value, false); }
                    catch (IOException) { }
                    catch (UnauthorizedAccessException) { }
                }
            }
        }
        private static bool Is64Bit(IntPtr process)
        {
            bool wow; if (Environment.Is64BitOperatingSystem && IsWow64Process(process, out wow)) return !wow;
            return Environment.Is64BitProcess;
        }
        private static void Inject(Process process, string dll)
        {
            IntPtr h = OpenProcess(0x043A, false, process.Id); if (h == IntPtr.Zero) throw new Win32Exception(Marshal.GetLastWin32Error());
            try
            {
                IntPtr localModule = LoadLibraryW(dll); if (localModule == IntPtr.Zero) throw new Win32Exception(Marshal.GetLastWin32Error());
                try
                {
                    IntPtr init = GetProcAddress(localModule, "TrayFpsInitialize");
                    if (init == IntPtr.Zero) throw new Win32Exception("The native module does not export TrayFpsInitialize.");
                    byte[] bytes = System.Text.Encoding.Unicode.GetBytes(dll + "\0"); IntPtr remote = VirtualAllocEx(h, IntPtr.Zero, (UIntPtr)bytes.Length, 0x3000, 0x04);
                    if (remote == IntPtr.Zero) throw new Win32Exception(Marshal.GetLastWin32Error());
                    bool remoteThreadMayReadPath = false;
                    try
                    {
                        UIntPtr written;
                        if (!WriteProcessMemory(h, remote, bytes, (UIntPtr)bytes.Length, out written) || written.ToUInt64() != (ulong)bytes.Length) throw new Win32Exception(Marshal.GetLastWin32Error());
                        IntPtr load = GetProcAddress(GetModuleHandle("kernel32.dll"), "LoadLibraryW");
                        IntPtr thread = CreateRemoteThread(h, IntPtr.Zero, 0, load, remote, 0, IntPtr.Zero);
                        if (thread == IntPtr.Zero) throw new Win32Exception(Marshal.GetLastWin32Error());
                        remoteThreadMayReadPath = true;
                        try
                        {
                            if (WaitForSingleObject(thread, 10000) != 0) throw new TimeoutException("Timed out loading the limiter into the game.");
                            remoteThreadMayReadPath = false;
                            uint module;
                            if (!GetExitCodeThread(thread, out module) || module == 0) throw new Win32Exception("The game rejected module loading.");
                        }
                        finally { CloseHandle(thread); }
                        IntPtr remoteModule = FindRemoteModule(process.Id, Path.GetFileName(dll));
                        if (remoteModule == IntPtr.Zero) throw new Win32Exception("Loaded module could not be found in the target process.");
                        long offset = init.ToInt64() - localModule.ToInt64(); IntPtr remoteInit = new IntPtr(remoteModule.ToInt64() + offset);
                        thread = CreateRemoteThread(h, IntPtr.Zero, 0, remoteInit, IntPtr.Zero, 0, IntPtr.Zero);
                        if (thread == IntPtr.Zero) throw new Win32Exception(Marshal.GetLastWin32Error());
                        try { uint result; if (WaitForSingleObject(thread, 10000) != 0) throw new TimeoutException("Timed out installing DXGI hooks."); if (!GetExitCodeThread(thread, out result) || result == 0) throw new Win32Exception("Native hook initialization failed."); }
                        finally { CloseHandle(thread); }
                    }
                    finally { if (!remoteThreadMayReadPath) VirtualFreeEx(h, remote, UIntPtr.Zero, 0x8000); }
                }
                finally { FreeLibrary(localModule); }
            }
            finally { CloseHandle(h); }
        }
        private static IntPtr FindRemoteModule(int pid, string moduleName)
        {
            IntPtr snap = CreateToolhelp32Snapshot(0x18, (uint)pid); if (snap == new IntPtr(-1)) return IntPtr.Zero;
            try { var entry = new ModuleEntry { Size = (uint)Marshal.SizeOf(typeof(ModuleEntry)) }; if (Module32First(snap, ref entry)) do { if (string.Equals(entry.Module, moduleName, StringComparison.OrdinalIgnoreCase)) return entry.Base; } while (Module32Next(snap, ref entry)); return IntPtr.Zero; }
            finally { CloseHandle(snap); }
        }
        public void Stop() {
            if (heartbeatTimer != null) { heartbeatTimer.Dispose(); heartbeatTimer = null; }
            ClearTarget();
            lock (configGate) {
                if (globalView != null) { WriteView(globalView, 0, 60, false); globalView.Dispose(); globalView = null; }
                if (globalMap != null) { globalMap.Dispose(); globalMap = null; }
            }
            string app = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "TrayFPS", "1.0.0");
            string x64 = Path.Combine(app, "TrayFPS-Vulkan-x64.json"), x86 = Path.Combine(app, "TrayFPS-Vulkan-x86.json");
            UnregisterLayer(x64, RegistryView.Registry64); if (Environment.Is64BitOperatingSystem) UnregisterLayer(x86, RegistryView.Registry32);
            CleanupOwnLayerRegistrations();
            vulkanLayerPrepared = false; vulkanLayerPreparedAtUtc = DateTime.MinValue;
            SetStatus(LimiterStatus.Disabled);
        }
        public void Dispose() { Stop(); }

        [DllImport("kernel32.dll", SetLastError=true)] static extern IntPtr OpenProcess(uint access, bool inherit, int pid);
        [DllImport("kernel32.dll", SetLastError=true)] static extern IntPtr VirtualAllocEx(IntPtr p, IntPtr a, UIntPtr n, uint type, uint protect);
        [DllImport("kernel32.dll", SetLastError=true)] static extern bool WriteProcessMemory(IntPtr p, IntPtr a, byte[] b, UIntPtr n, out UIntPtr written);
        [DllImport("kernel32.dll", SetLastError=true)] static extern IntPtr CreateRemoteThread(IntPtr p, IntPtr attr, uint stack, IntPtr start, IntPtr arg, uint flags, IntPtr id);
        [DllImport("kernel32.dll", SetLastError=true)] static extern uint WaitForSingleObject(IntPtr h, uint ms);
        [DllImport("kernel32.dll", SetLastError=true)] static extern bool GetExitCodeThread(IntPtr h, out uint code);
        [DllImport("kernel32.dll")] static extern bool CloseHandle(IntPtr h);
        [DllImport("kernel32.dll", CharSet=CharSet.Ansi, SetLastError=true)] static extern IntPtr GetProcAddress(IntPtr m, string n);
        [DllImport("kernel32.dll", CharSet=CharSet.Unicode)] static extern IntPtr GetModuleHandle(string n);
        [DllImport("kernel32.dll", CharSet=CharSet.Unicode, SetLastError=true)] static extern IntPtr LoadLibraryW(string n);
        [DllImport("kernel32.dll")] static extern bool FreeLibrary(IntPtr m);
        [StructLayout(LayoutKind.Sequential, CharSet=CharSet.Unicode)] struct ModuleEntry { public uint Size, ModuleId, ProcessId, GlobalUsage, ProcessUsage; public IntPtr Base; public uint BaseSize; public IntPtr ModuleHandle; [MarshalAs(UnmanagedType.ByValTStr, SizeConst=256)] public string Module; [MarshalAs(UnmanagedType.ByValTStr, SizeConst=260)] public string Path; }
        [DllImport("kernel32.dll", SetLastError=true)] static extern IntPtr CreateToolhelp32Snapshot(uint flags, uint pid);
        [DllImport("kernel32.dll", CharSet=CharSet.Unicode, SetLastError=true)] static extern bool Module32First(IntPtr snapshot, ref ModuleEntry entry);
        [DllImport("kernel32.dll", CharSet=CharSet.Unicode, SetLastError=true)] static extern bool Module32Next(IntPtr snapshot, ref ModuleEntry entry);
        [DllImport("kernel32.dll", SetLastError=true)] static extern bool IsWow64Process(IntPtr p, out bool wow);
        [DllImport("kernel32.dll", SetLastError=true)] static extern bool VirtualFreeEx(IntPtr p, IntPtr a, UIntPtr n, uint type);
    }
}
