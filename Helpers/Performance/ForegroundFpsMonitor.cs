using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace TrayTemps
{
    internal sealed class ForegroundFpsMonitor : IDisposable
    {
        private const uint ErrorSuccess = 0;
        private const uint ErrorAlreadyExists = 183;
        private const uint ErrorInvalidParameter = 87;
        private const uint ErrorNotSupported = 50;
        private const uint EventControlCodeDisableProvider = 0;
        private const uint EventControlCodeEnableProvider = 1;
        private const uint EventTraceControlStop = 1;
        private const uint EventTraceControlFlush = 3;
        private const uint EventTraceRealTimeMode = 0x00000100;
        private const uint ProcessTraceModeRealTime = 0x00000100;
        private const uint ProcessTraceModeRawTimestamp = 0x00001000;
        private const uint ProcessTraceModeEventRecord = 0x10000000;
        private const uint WnodeFlagTracedGuid = 0x00020000;
        private const uint EventFilterTypeEventId = 0x80000200;
        private const uint EnableTraceParametersVersion2 = 2;
        private const uint EventEnablePropertyIgnoreKeywordZero = 0x00000010;
        private const ulong PresentKeyword = 0x8000000000000002;
        private const ushort DxgiPresentStartEventId = 0x002a;
        private const ushort D3d9PresentStartEventId = 0x0001;
        private const ushort EventHeaderFlag32Bit = 0x0020;
        private const uint DxgiPresentTest = 0x00000001;
        private const byte DxgiPresentProvider = 1;
        private const byte D3d9PresentProvider = 2;
        private const int EventRecordUserDataLengthOffsetX64 = 86;
        private const int EventRecordUserDataOffsetX64 = 96;
        private const ulong InvalidProcessTraceHandle = ulong.MaxValue;
        private const int EventTracePropertiesSizeX64 = 120;
        private const int EventTraceLogFileSizeX64 = 448;

        private static readonly Guid DxgiProviderGuid =
            new Guid("CA11C036-0102-4A2D-A6AD-F03CFED5D3C9");
        private static readonly Guid D3d9ProviderGuid =
            new Guid("783ACA0A-790E-4D7F-8451-AA850511C6B9");

        private readonly long _qpcFrequency;
        private readonly long _calculationWindowTicks;
        private readonly long _staleValueTicks;
        private readonly long _streamStaleTicks;
        private EventRecordCallback _eventRecordCallback;
        private Thread _consumerThread;
        private IntPtr _propertiesBuffer;
        private string _sessionName;
        private Guid _sessionGuid;
        private ulong _sessionHandle;
        private ulong _consumerHandle = InvalidProcessTraceHandle;
        private int _targetProcessId;
        private int _targetGeneration;
        private int _missingForegroundSamples;
        private int _observedGeneration;
        private byte _selectedPresentProvider;
        private ulong _selectedSwapChain;
        private int _selectedPresentThreadId;
        private long _selectedStreamTimestamp;
        private long _windowStartTimestamp;
        private int _presentIntervals;
        private int _latestFps = -1;
        private long _lastPresentTimestamp;
        private long _lastFlushTimestamp;
        private bool _running;
        private bool _disposed;

        internal ForegroundFpsMonitor()
        {
            QueryPerformanceFrequency(out _qpcFrequency);
            if (_qpcFrequency <= 0)
                _qpcFrequency = 10000000L;

            _calculationWindowTicks = Math.Max(1L, _qpcFrequency / 4L);
            _staleValueTicks = Math.Max(1L, _qpcFrequency * 3L);
            _streamStaleTicks = Math.Max(1L, _qpcFrequency / 2L);
        }

        internal bool Start()
        {
            if (_disposed || _running || IntPtr.Size != 8 ||
                Marshal.SizeOf(typeof(EventTraceProperties)) != EventTracePropertiesSizeX64 ||
                Marshal.SizeOf(typeof(EventTraceLogFile)) != EventTraceLogFileSizeX64)
            {
                return _running;
            }

            try
            {
                uint processId = GetCurrentProcessId();
                uint sessionId;
                _sessionName = "TrayTemps.FPS." +
                    (ProcessIdToSessionId(processId, out sessionId) ? sessionId : processId);
                _sessionGuid = Guid.NewGuid();

                if (!CreatePropertiesBuffer(_sessionGuid))
                    return false;

                if (!StartSession())
                    return FailStart();

                if (!EnablePresentProvider(DxgiProviderGuid, DxgiPresentStartEventId) ||
                    !EnablePresentProvider(D3d9ProviderGuid, D3d9PresentStartEventId))
                {
                    return FailStart();
                }

                _eventRecordCallback = OnEventRecord;
                var logFile = new EventTraceLogFile
                {
                    LoggerName = IntPtr.Add(_propertiesBuffer, EventTracePropertiesSizeX64),
                    ProcessTraceMode = ProcessTraceModeRealTime |
                        ProcessTraceModeRawTimestamp |
                        ProcessTraceModeEventRecord,
                    EventRecordCallback = Marshal.GetFunctionPointerForDelegate(_eventRecordCallback)
                };

                _consumerHandle = OpenTraceW(ref logFile);
                if (_consumerHandle == InvalidProcessTraceHandle)
                    return FailStart();

                var consumerThread = new Thread(ConsumeEvents)
                {
                    IsBackground = true,
                    Name = "TrayTemps FPS ETW"
                };
                consumerThread.Start();
                _consumerThread = consumerThread;
                Volatile.Write(ref _lastFlushTimestamp, 0L);
                _running = true;
                return true;
            }
            catch
            {
                return FailStart();
            }
        }

        private bool StartSession()
        {
            uint status = StartTraceW(out _sessionHandle, _sessionName, _propertiesBuffer);
            if (status == ErrorSuccess)
                return true;

            _sessionHandle = 0;
            if (status != ErrorAlreadyExists)
                return false;

            ControlTraceW(0, _sessionName, _propertiesBuffer, EventTraceControlStop);

            string sessionName = _sessionName;
            ReleasePropertiesBuffer();
            _sessionName = sessionName;
            _sessionGuid = Guid.NewGuid();
            if (!CreatePropertiesBuffer(_sessionGuid))
                return false;

            status = StartTraceW(out _sessionHandle, _sessionName, _propertiesBuffer);
            if (status == ErrorSuccess)
                return true;

            _sessionHandle = 0;
            return false;
        }

        private bool FailStart()
        {
            ulong consumerHandle = _consumerHandle;
            _consumerHandle = InvalidProcessTraceHandle;
            if (consumerHandle != InvalidProcessTraceHandle)
                CloseTrace(consumerHandle);

            StopSession();
            _eventRecordCallback = null;
            _running = false;
            ReleasePropertiesBuffer();
            return false;
        }

        internal void UpdateForegroundProcess()
        {
            if (!_running)
                return;

            int processId = 0;
            IntPtr foregroundWindow = GetForegroundWindow();
            if (foregroundWindow != IntPtr.Zero)
            {
                GetWindowThreadProcessId(foregroundWindow, out uint foregroundProcessId);
                if (foregroundProcessId <= int.MaxValue)
                    processId = (int)foregroundProcessId;
            }

            // Foreground activation can briefly have no window. Ignore one
            // invalid sample, but stop tracking after a consecutive miss.
            if (processId == 0)
            {
                if (_missingForegroundSamples < 2)
                    _missingForegroundSamples++;
                if (_missingForegroundSamples < 2)
                    return;
            }
            else
            {
                _missingForegroundSamples = 0;
            }

            if (Volatile.Read(ref _targetProcessId) == processId)
                return;

            if (Interlocked.Exchange(ref _targetProcessId, processId) != processId)
            {
                Interlocked.Increment(ref _targetGeneration);
                Volatile.Write(ref _latestFps, -1);
                Volatile.Write(ref _lastPresentTimestamp, 0L);
            }
        }

        internal int? GetLatestFps()
        {
            if (!_running)
                return null;

            long lastPresent = Volatile.Read(ref _lastPresentTimestamp);
            QueryPerformanceCounter(out long now);
            if (lastPresent <= 0 || now < lastPresent || now - lastPresent > _staleValueTicks)
                return null;

            int value = Volatile.Read(ref _latestFps);
            return value >= 0 ? (int?)value : null;
        }

        internal void FlushIfNeeded(int intervalMilliseconds)
        {
            if (!_running || _sessionHandle == 0 || intervalMilliseconds <= 0)
                return;

            QueryPerformanceCounter(out long now);
            long intervalTicks = Math.Max(1L, _qpcFrequency * intervalMilliseconds / 1000L);
            long previous = Volatile.Read(ref _lastFlushTimestamp);

            if (previous > 0 && now >= previous && now - previous < intervalTicks)
                return;

            if (Interlocked.CompareExchange(ref _lastFlushTimestamp, now, previous) != previous)
                return;

            ControlTraceW(
                _sessionHandle,
                _sessionName,
                _propertiesBuffer,
                EventTraceControlFlush);
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;
            Stop();
        }

        internal void Stop()
        {
            Volatile.Write(ref _lastFlushTimestamp, 0L);
            _missingForegroundSamples = 0;
            Interlocked.Exchange(ref _targetProcessId, 0);
            Interlocked.Increment(ref _targetGeneration);
            Volatile.Write(ref _latestFps, -1);
            Volatile.Write(ref _lastPresentTimestamp, 0L);

            if (_sessionHandle != 0)
            {
                Guid dxgi = DxgiProviderGuid;
                Guid d3d9 = D3d9ProviderGuid;
                EnableTraceEx2(
                    _sessionHandle, ref dxgi, EventControlCodeDisableProvider,
                    0, 0, 0, 0, IntPtr.Zero);
                EnableTraceEx2(
                    _sessionHandle, ref d3d9, EventControlCodeDisableProvider,
                    0, 0, 0, 0, IntPtr.Zero);
                StopSession();
            }

            ulong consumerHandle = _consumerHandle;
            _consumerHandle = InvalidProcessTraceHandle;
            if (consumerHandle != InvalidProcessTraceHandle)
                CloseTrace(consumerHandle);

            Thread consumerThread = _consumerThread;
            _consumerThread = null;
            if (consumerThread != null && consumerThread != Thread.CurrentThread)
                consumerThread.Join();

            _eventRecordCallback = null;
            _running = false;
            ReleasePropertiesBuffer();
        }

        private unsafe bool CreatePropertiesBuffer(Guid sessionGuid)
        {
            int nameBytes = checked((_sessionName.Length + 1) * sizeof(char));
            int totalBytes = checked(EventTracePropertiesSizeX64 + nameBytes);
            _propertiesBuffer = Marshal.AllocHGlobal(totalBytes);
            if (_propertiesBuffer == IntPtr.Zero)
                return false;

            byte* buffer = (byte*)_propertiesBuffer.ToPointer();
            for (int index = 0; index < totalBytes; index++)
                buffer[index] = 0;

            var properties = new EventTraceProperties
            {
                Wnode = new WnodeHeader
                {
                    BufferSize = (uint)totalBytes,
                    Guid = sessionGuid,
                    ClientContext = 1,
                    Flags = WnodeFlagTracedGuid
                },
                BufferSize = 4,
                MinimumBuffers = 2,
                MaximumBuffers = 4,
                LogFileMode = EventTraceRealTimeMode,
                FlushTimer = 1,
                LoggerNameOffset = EventTracePropertiesSizeX64
            };
            Marshal.StructureToPtr(properties, _propertiesBuffer, false);

            char* destination = (char*)(buffer + EventTracePropertiesSizeX64);
            for (int index = 0; index < _sessionName.Length; index++)
                destination[index] = _sessionName[index];
            destination[_sessionName.Length] = '\0';
            return true;
        }

        private unsafe bool EnablePresentProvider(Guid providerGuid, ushort eventId)
        {
            var filter = new EventFilterEventId
            {
                FilterIn = 1,
                Count = 1,
                EventId = eventId
            };
            var descriptor = new EventFilterDescriptor
            {
                Ptr = (ulong)&filter,
                Size = (uint)sizeof(EventFilterEventId),
                Type = EventFilterTypeEventId
            };
            var parameters = new EnableTraceParameters
            {
                Version = EnableTraceParametersVersion2,
                EnableProperty = EventEnablePropertyIgnoreKeywordZero,
                SourceId = _sessionGuid,
                EnableFilterDesc = (IntPtr)(&descriptor),
                FilterDescCount = 1
            };

            Guid provider = providerGuid;
            uint status = EnableTraceEx2WithParameters(
                _sessionHandle,
                ref provider,
                EventControlCodeEnableProvider,
                0,
                PresentKeyword,
                PresentKeyword,
                0,
                ref parameters);

            if (status == ErrorInvalidParameter || status == ErrorNotSupported)
            {
                status = EnableTraceEx2(
                    _sessionHandle,
                    ref provider,
                    EventControlCodeEnableProvider,
                    0,
                    PresentKeyword,
                    PresentKeyword,
                    0,
                    IntPtr.Zero);
            }

            return status == ErrorSuccess;
        }

        private void ConsumeEvents()
        {
            ulong handle = _consumerHandle;
            if (handle != InvalidProcessTraceHandle)
                ProcessTrace(ref handle, 1, IntPtr.Zero, IntPtr.Zero);
        }

        private unsafe void OnEventRecord(IntPtr eventRecord)
        {
            byte* header = (byte*)eventRecord.ToPointer();
            int processId = *(int*)(header + 12);
            if (processId == 0 || processId != Volatile.Read(ref _targetProcessId))
                return;

            ushort eventId = *(ushort*)(header + 40);
            bool isDxgiPresent = eventId == DxgiPresentStartEventId &&
                *(uint*)(header + 24) == 0xCA11C036u &&
                *(uint*)(header + 28) == 0x4A2D0102u &&
                *(uint*)(header + 32) == 0x3CF0ADA6u &&
                *(uint*)(header + 36) == 0xC9D3D5FEu;
            bool isD3d9Present = eventId == D3d9PresentStartEventId &&
                *(uint*)(header + 24) == 0x783ACA0Au &&
                *(uint*)(header + 28) == 0x4D7F790Eu &&
                *(uint*)(header + 32) == 0x85AA5184u &&
                *(uint*)(header + 36) == 0xB9C61105u;
            if (!isDxgiPresent && !isD3d9Present)
                return;

            int generation = Volatile.Read(ref _targetGeneration);
            long timestamp = *(long*)(header + 16);
            byte provider = isDxgiPresent ? DxgiPresentProvider : D3d9PresentProvider;
            int threadId = *(int*)(header + 8);
            ulong swapChain;
            uint presentFlags;
            ReadPresentData(header, out swapChain, out presentFlags);
            if (isDxgiPresent && (presentFlags & DxgiPresentTest) != 0)
                return;

            if (_observedGeneration != generation)
            {
                _observedGeneration = generation;
                SelectPresentStream(provider, swapChain, threadId, timestamp);
                _windowStartTimestamp = timestamp;
                _presentIntervals = 0;
                return;
            }

            if (!IsSelectedPresentStream(provider, swapChain, threadId, timestamp))
                return;

            long lastPresentTimestamp = Volatile.Read(ref _lastPresentTimestamp);
            if (timestamp <= lastPresentTimestamp)
                return;

            Volatile.Write(ref _lastPresentTimestamp, timestamp);
            if (_windowStartTimestamp <= 0)
            {
                _windowStartTimestamp = timestamp;
                _presentIntervals = 0;
                return;
            }

            if (timestamp <= _windowStartTimestamp)
                return;

            _presentIntervals++;
            long elapsed = timestamp - _windowStartTimestamp;
            if (elapsed < _calculationWindowTicks)
                return;

            double framesPerSecond = _presentIntervals * (double)_qpcFrequency / elapsed;
            int fps = (int)Math.Round(framesPerSecond, MidpointRounding.AwayFromZero);
            Volatile.Write(ref _latestFps, Math.Max(0, Math.Min(9999, fps)));
            _windowStartTimestamp = timestamp;
            _presentIntervals = 0;
        }

        private static unsafe void ReadPresentData(
            byte* eventHeader,
            out ulong swapChain,
            out uint presentFlags)
        {
            swapChain = 0;
            presentFlags = 0;

            ushort userDataLength = *(ushort*)(eventHeader + EventRecordUserDataLengthOffsetX64);
            byte* userData = *(byte**)(eventHeader + EventRecordUserDataOffsetX64);
            if (userData == null)
                return;

            bool is32Bit = (*(ushort*)(eventHeader + 4) & EventHeaderFlag32Bit) != 0;
            if (is32Bit)
            {
                if (userDataLength < sizeof(uint))
                    return;

                swapChain = *(uint*)userData;
                if (userDataLength >= sizeof(uint) + sizeof(uint))
                    presentFlags = *(uint*)(userData + sizeof(uint));
                return;
            }

            if (userDataLength < sizeof(ulong))
                return;

            swapChain = *(ulong*)userData;
            if (userDataLength >= sizeof(ulong) + sizeof(uint))
                presentFlags = *(uint*)(userData + sizeof(ulong));
        }

        private bool IsSelectedPresentStream(
            byte provider,
            ulong swapChain,
            int threadId,
            long timestamp)
        {
            // A process can present multiple swap chains (including overlays).
            // Count one stream instead of incorrectly summing them as one FPS value.
            bool hasSwapChainIdentity = swapChain != 0 || _selectedSwapChain != 0;
            bool matches = provider == _selectedPresentProvider &&
                (hasSwapChainIdentity
                    ? swapChain != 0 && swapChain == _selectedSwapChain
                    : threadId == _selectedPresentThreadId);

            if (matches)
            {
                if (timestamp > _selectedStreamTimestamp)
                    _selectedStreamTimestamp = timestamp;
                return true;
            }

            if (timestamp <= _selectedStreamTimestamp ||
                timestamp - _selectedStreamTimestamp <= _streamStaleTicks)
            {
                return false;
            }

            SelectPresentStream(provider, swapChain, threadId, timestamp);
            _windowStartTimestamp = timestamp;
            _presentIntervals = 0;
            Volatile.Write(ref _latestFps, -1);
            Volatile.Write(ref _lastPresentTimestamp, 0L);
            return false;
        }

        private void SelectPresentStream(
            byte provider,
            ulong swapChain,
            int threadId,
            long timestamp)
        {
            _selectedPresentProvider = provider;
            _selectedSwapChain = swapChain;
            _selectedPresentThreadId = threadId;
            _selectedStreamTimestamp = timestamp;
        }

        private void StopSession()
        {
            if (_sessionHandle == 0)
                return;

            ControlTraceW(
                _sessionHandle,
                _sessionName,
                _propertiesBuffer,
                EventTraceControlStop);
            _sessionHandle = 0;
        }

        private void ReleasePropertiesBuffer()
        {
            if (_propertiesBuffer != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(_propertiesBuffer);
                _propertiesBuffer = IntPtr.Zero;
            }

            _sessionName = null;
        }

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        private delegate void EventRecordCallback(IntPtr eventRecord);

        [StructLayout(LayoutKind.Sequential)]
        private struct WnodeHeader
        {
            internal uint BufferSize;
            internal uint ProviderId;
            internal ulong HistoricalContext;
            internal long TimeStamp;
            internal Guid Guid;
            internal uint ClientContext;
            internal uint Flags;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct EventTraceProperties
        {
            internal WnodeHeader Wnode;
            internal uint BufferSize;
            internal uint MinimumBuffers;
            internal uint MaximumBuffers;
            internal uint MaximumFileSize;
            internal uint LogFileMode;
            internal uint FlushTimer;
            internal uint EnableFlags;
            internal int AgeLimit;
            internal uint NumberOfBuffers;
            internal uint FreeBuffers;
            internal uint EventsLost;
            internal uint BuffersWritten;
            internal uint LogBuffersLost;
            internal uint RealTimeBuffersLost;
            internal IntPtr LoggerThreadId;
            internal uint LogFileNameOffset;
            internal uint LoggerNameOffset;
        }

        [StructLayout(LayoutKind.Explicit, Size = EventTraceLogFileSizeX64)]
        private struct EventTraceLogFile
        {
            [FieldOffset(0)] internal IntPtr LogFileName;
            [FieldOffset(8)] internal IntPtr LoggerName;
            [FieldOffset(28)] internal uint ProcessTraceMode;
            [FieldOffset(400)] internal IntPtr BufferCallback;
            [FieldOffset(424)] internal IntPtr EventRecordCallback;
            [FieldOffset(432)] internal uint IsKernelTrace;
            [FieldOffset(440)] internal IntPtr Context;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct EventFilterEventId
        {
            internal byte FilterIn;
            internal byte Reserved;
            internal ushort Count;
            internal ushort EventId;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct EventFilterDescriptor
        {
            internal ulong Ptr;
            internal uint Size;
            internal uint Type;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct EnableTraceParameters
        {
            internal uint Version;
            internal uint EnableProperty;
            internal uint ControlFlags;
            internal Guid SourceId;
            internal IntPtr EnableFilterDesc;
            internal uint FilterDescCount;
        }

        [DllImport("advapi32.dll", CharSet = CharSet.Unicode)]
        private static extern uint StartTraceW(
            out ulong sessionHandle,
            string sessionName,
            IntPtr properties);

        [DllImport("advapi32.dll", CharSet = CharSet.Unicode)]
        private static extern uint ControlTraceW(
            ulong sessionHandle,
            string sessionName,
            IntPtr properties,
            uint controlCode);

        [DllImport("advapi32.dll", EntryPoint = "EnableTraceEx2")]
        private static extern uint EnableTraceEx2(
            ulong traceHandle,
            ref Guid providerId,
            uint controlCode,
            byte level,
            ulong matchAnyKeyword,
            ulong matchAllKeyword,
            uint timeout,
            IntPtr enableParameters);

        [DllImport("advapi32.dll", EntryPoint = "EnableTraceEx2")]
        private static extern uint EnableTraceEx2WithParameters(
            ulong traceHandle,
            ref Guid providerId,
            uint controlCode,
            byte level,
            ulong matchAnyKeyword,
            ulong matchAllKeyword,
            uint timeout,
            ref EnableTraceParameters enableParameters);

        [DllImport("advapi32.dll", CharSet = CharSet.Unicode)]
        private static extern ulong OpenTraceW(ref EventTraceLogFile logFile);

        [DllImport("advapi32.dll")]
        private static extern uint ProcessTrace(
            ref ulong handleArray,
            uint handleCount,
            IntPtr startTime,
            IntPtr endTime);

        [DllImport("advapi32.dll")]
        private static extern uint CloseTrace(ulong traceHandle);

        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool QueryPerformanceFrequency(out long frequency);

        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool QueryPerformanceCounter(out long counter);

        [DllImport("kernel32.dll")]
        private static extern uint GetCurrentProcessId();

        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool ProcessIdToSessionId(
            uint processId,
            out uint sessionId);

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(
            IntPtr windowHandle,
            out uint processId);
    }
}
