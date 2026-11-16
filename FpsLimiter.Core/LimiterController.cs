using System;
using System.ComponentModel;
using System.Collections.Generic;
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
        private bool vulkanLayerPrepared;
        private string registeredManifest64, registeredManifest32, registeredNativeRoot;
        private const uint ProtocolVersion = 1;
        private static readonly HashSet<Timer> pendingLoads = new HashSet<Timer>();
        private readonly Action<Process, string, uint> inject;
        public LimiterController() : this(Inject) { }
        internal LimiterController(Action<Process, string, uint> inject) { this.inject = inject ?? throw new ArgumentNullException(nameof(inject)); }
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
            using (var process = Process.GetProcessById(processId))
            {
                bool x64 = Is64Bit(process.Handle);
                string dll = Path.Combine(nativeDll, x64 ? "x64" : "x86", "FpsLimiter.Native.dll");
                if (!File.Exists(dll)) throw new FileNotFoundException("Native module for target architecture is missing.", dll);
                string name = "Local\\TrayFPS.Config." + processId;
                var security = new MemoryMappedFileSecurity();
                security.AddAccessRule(new AccessRule<MemoryMappedFileRights>(WindowsIdentity.GetCurrent().User, MemoryMappedFileRights.ReadWrite, AccessControlType.Allow));
                MemoryMappedFile candidateMap = null;
                MemoryMappedViewAccessor candidateView = null;
                try
                {
                    candidateMap = MemoryMappedFile.CreateOrOpen(name, 32, MemoryMappedFileAccess.ReadWrite, MemoryMappedFileOptions.None, security, HandleInheritability.None);
                    candidateView = candidateMap.CreateViewAccessor(0, 32, MemoryMappedFileAccess.ReadWrite);
                    // Bootstrap with pacing disabled. The old target and its heartbeat
                    // remain intact until the candidate has initialized successfully.
                    WriteView(candidateView, processId, fps, false);
                    uint required = GetRequiredAdapters(processId);
                    if (required != 8) inject(process, dll, required);
                    if (process.HasExited) throw new InvalidOperationException("The candidate exited during attachment.");
                    if (enabled && !vulkanLayerPrepared) StartGlobalVulkan(fps, nativeDll);
                    lock (configGate)
                    {
                        ClearTarget();
                        map = candidateMap; view = candidateView;
                        candidateMap = null; candidateView = null;
                        activeProcessId = processId;
                        WriteConfig(processId, fps, enabled);
                    }
                }
                finally
                {
                    if (candidateView != null) { WriteView(candidateView, 0, 60, false); candidateView.Dispose(); }
                    candidateMap?.Dispose();
                }
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
            registeredNativeRoot = Path.GetFullPath(nativeRoot);
            registeredManifest64 = Path.GetFullPath(Path.Combine(manifestRoot, "TrayFPS-Vulkan-x64.json"));
            RegisterLayer(nativeRoot, registeredManifest64, RegistryView.Registry64, "x64");
            if (Environment.Is64BitOperatingSystem)
            {
                registeredManifest32 = Path.GetFullPath(Path.Combine(manifestRoot, "TrayFPS-Vulkan-x86.json"));
                RegisterLayer(nativeRoot, registeredManifest32, RegistryView.Registry32, "x86");
            }
            vulkanLayerPrepared = true;
        }
        private void WriteGlobalConfig(int targetPid, int fps, bool enabled) { WriteView(globalView, targetPid, fps, enabled); }
        private LimiterStatus GetTargetStatus(int processId, bool enabled)
        {
            if (!enabled) return LimiterStatus.Disabled;
            if (FindRemoteModule(processId, "vulkan-1.dll") == IntPtr.Zero) return LimiterStatus.Attached;
            // Existing games keep a loaded layer across OFF/ON and owner handover.
            // Inspect the exact payload instead of inferring layer presence from age.
            return registeredNativeRoot != null && FindRemoteModule(processId,
                Path.Combine(registeredNativeRoot, "x64", "FpsLimiter.Native.dll")) != IntPtr.Zero
                ? LimiterStatus.Attached : LimiterStatus.RestartRequired;
        }
        public void DetachTarget()
        {
            ClearTarget();
            SetStatus(globalView != null ? LimiterStatus.WaitingForGame : LimiterStatus.Disabled);
        }
        public void Pause()
        {
            Stop();
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
        public static void CleanupOwnLayerRegistrations()
        {
            foreach (var view in Environment.Is64BitOperatingSystem ? new[] { RegistryView.Registry64, RegistryView.Registry32 } : new[] { RegistryView.Registry64 })
            using (var root = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, view))
            using (var key = root.OpenSubKey("SOFTWARE\\Khronos\\Vulkan\\ImplicitLayers", true))
            {
                if (key == null) continue;
                foreach (string value in key.GetValueNames())
                {
                    if (IsOwnLayerManifest(value)) key.DeleteValue(value, false);
                }
            }
        }
        private static bool IsOwnLayerManifest(string manifest)
        {
            // Only exact filenames in our own versioned cache (or the legacy
            // standalone cache) qualify, including manifests lost after a crash.
            try
            {
                if (!Path.IsPathRooted(manifest)) return false;
                string full = Path.GetFullPath(manifest);
                string file = Path.GetFileName(full);
                if (!file.Equals("TrayFPS-Vulkan-x64.json", StringComparison.OrdinalIgnoreCase) &&
                    !file.Equals("TrayFPS-Vulkan-x86.json", StringComparison.OrdinalIgnoreCase)) return false;
                string local = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                string parent = Path.GetDirectoryName(Path.GetDirectoryName(full));
                return string.Equals(parent, Path.Combine(local, "TrayTemps", "FpsLimiter"), StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(parent, Path.Combine(local, "TrayFPS"), StringComparison.OrdinalIgnoreCase);
            }
            catch (ArgumentException) { return false; }
            catch (NotSupportedException) { return false; }
            catch (PathTooLongException) { return false; }
        }
        private static uint GetRequiredAdapters(int pid)
        {
            if (FindRemoteModule(pid, "vulkan-1.dll") != IntPtr.Zero) return 8;
            if (FindRemoteModule(pid, "d3d9.dll") != IntPtr.Zero) return 2;
            if (FindRemoteModule(pid, "opengl32.dll") != IntPtr.Zero) return 4;
            return 1; // DXGI (D3D10/11/12); unknown renderers must not succeed via GDI alone.
        }
        private static bool Is64Bit(IntPtr process)
        {
            bool wow; if (Environment.Is64BitOperatingSystem && IsWow64Process(process, out wow)) return !wow;
            return Environment.Is64BitProcess;
        }
        private static void Inject(Process process, string dll, uint required)
        {
            IntPtr h = OpenProcess(0x10043A, false, process.Id); if (h == IntPtr.Zero) throw new Win32Exception(Marshal.GetLastWin32Error());
            try
            {
                IntPtr localModule = LoadLibraryW(dll); if (localModule == IntPtr.Zero) throw new Win32Exception(Marshal.GetLastWin32Error());
                try
                {
                    IntPtr init = GetProcAddress(localModule, "TrayFpsInitialize");
                    if (init == IntPtr.Zero) throw new Win32Exception("The native module does not export TrayFpsInitialize.");
                    IntPtr remoteModule = FindRemoteModule(process.Id, Path.GetFullPath(dll));
                    bool reused = remoteModule != IntPtr.Zero;
                    if (remoteModule == IntPtr.Zero)
                    {
                        if (FindRemoteModule(process.Id, Path.GetFileName(dll)) != IntPtr.Zero)
                            throw new InvalidOperationException("A different limiter payload is already loaded. Restart this game before attaching the new engine.");
                        byte[] bytes = System.Text.Encoding.Unicode.GetBytes(dll + "\0");
                        IntPtr remote = VirtualAllocEx(h, IntPtr.Zero, (UIntPtr)bytes.Length, 0x3000, 0x04);
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
                                if (WaitForSingleObject(thread, 10000) != 0)
                                {
                                    TrackPendingLoad(h, thread, remote);
                                    thread = IntPtr.Zero;
                                    throw new TimeoutException("Timed out loading the limiter into the game.");
                                }
                                remoteThreadMayReadPath = false;
                                uint module;
                                if (!GetExitCodeThread(thread, out module)) throw new Win32Exception("Could not query the module loader thread.");
                            }
                            finally { if (thread != IntPtr.Zero) CloseHandle(thread); }
                        }
                        finally { if (!remoteThreadMayReadPath) VirtualFreeEx(h, remote, UIntPtr.Zero, 0x8000); }
                        // A thread's 32-bit exit code cannot represent an x64 HMODULE.
                        remoteModule = FindRemoteModule(process.Id, Path.GetFullPath(dll));
                    }
                    if (remoteModule == IntPtr.Zero) throw new Win32Exception("The exact limiter payload could not be found in the target process.");
                    long offset = init.ToInt64() - localModule.ToInt64(); IntPtr remoteInit = new IntPtr(remoteModule.ToInt64() + offset);
                    // Bootstrap dependencies pollute later module-based API detection.
                    // A reused engine remembers which adapters it initialized.
                    IntPtr initThread = CreateRemoteThread(h, IntPtr.Zero, 0, remoteInit, reused ? IntPtr.Zero : (IntPtr)required, 0, IntPtr.Zero);
                    if (initThread == IntPtr.Zero) throw new Win32Exception(Marshal.GetLastWin32Error());
                    try
                    {
                        if (WaitForSingleObject(initThread, 10000) != 0) throw new TimeoutException("Timed out installing presentation hooks.");
                        if (!GetExitCodeThread(initThread, out uint result) || result == 0 || (!reused && (result & required) != required))
                            throw new Win32Exception("Native hook initialization failed for the target graphics API.");
                    }
                    finally { CloseHandle(initThread); }
                }
                finally { FreeLibrary(localModule); }
            }
            finally { CloseHandle(h); }
        }
        private static IntPtr FindRemoteModule(int pid, string moduleName)
        {
            bool fullPath = Path.IsPathRooted(moduleName);
            IntPtr snap;
            int attempts = 0;
            do { snap = CreateToolhelp32Snapshot(0x18, (uint)pid); }
            while (snap == new IntPtr(-1) && Marshal.GetLastWin32Error() == 24 && ++attempts < 3);
            if (snap == new IntPtr(-1))
            {
                if (fullPath) throw new Win32Exception(Marshal.GetLastWin32Error());
                return IntPtr.Zero;
            }
            try
            {
                var entry = new ModuleEntry { Size = (uint)Marshal.SizeOf(typeof(ModuleEntry)) };
                if (Module32First(snap, ref entry))
                    do { if (string.Equals(fullPath ? entry.Path : entry.Module, moduleName, StringComparison.OrdinalIgnoreCase)) return entry.Base; }
                    while (Module32Next(snap, ref entry));
                int error = Marshal.GetLastWin32Error();
                if (fullPath && error != 18) throw new Win32Exception(error);
                return IntPtr.Zero;
            }
            finally { CloseHandle(snap); }
        }
        private static void TrackPendingLoad(IntPtr process, IntPtr thread, IntPtr path)
        {
            IntPtr current = GetCurrentProcess();
            if (!DuplicateHandle(current, process, current, out IntPtr retainedProcess, 0, false, 2))
                throw new Win32Exception(Marshal.GetLastWin32Error());
            // The thread owns the path until it exits. Keep both handles alive and
            // reconcile asynchronously, even after the controller is stopped.
            Timer cleanup = null;
            int completed = 0;
            cleanup = new Timer(_ =>
            {
                if (WaitForSingleObject(thread, 0) != 0 && WaitForSingleObject(retainedProcess, 0) != 0) return;
                if (Interlocked.Exchange(ref completed, 1) != 0) return;
                cleanup.Change(Timeout.Infinite, Timeout.Infinite);
                if (WaitForSingleObject(retainedProcess, 0) != 0) VirtualFreeEx(retainedProcess, path, UIntPtr.Zero, 0x8000);
                CloseHandle(thread); CloseHandle(retainedProcess); cleanup.Dispose();
                lock (pendingLoads) pendingLoads.Remove(cleanup);
            }, null, Timeout.Infinite, Timeout.Infinite);
            lock (pendingLoads) pendingLoads.Add(cleanup);
            cleanup.Change(1000, 1000);
        }
        public void Stop() {
            if (heartbeatTimer != null) { heartbeatTimer.Dispose(); heartbeatTimer = null; }
            ClearTarget();
            lock (configGate) {
                if (globalView != null) { WriteView(globalView, 0, 60, false); globalView.Dispose(); globalView = null; }
                if (globalMap != null) { globalMap.Dispose(); globalMap = null; }
            }
            if (registeredManifest64 != null) UnregisterLayer(registeredManifest64, RegistryView.Registry64);
            if (registeredManifest32 != null) UnregisterLayer(registeredManifest32, RegistryView.Registry32);
            registeredManifest64 = registeredManifest32 = registeredNativeRoot = null;
            vulkanLayerPrepared = false;
            SetStatus(LimiterStatus.Disabled);
        }
        public void Dispose() { Stop(); }

        [DllImport("kernel32.dll", SetLastError=true)] static extern IntPtr OpenProcess(uint access, bool inherit, int pid);
        [DllImport("kernel32.dll")] static extern IntPtr GetCurrentProcess();
        [DllImport("kernel32.dll", SetLastError=true)] static extern bool DuplicateHandle(IntPtr sourceProcess, IntPtr source, IntPtr targetProcess, out IntPtr target, uint access, bool inherit, uint options);
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
