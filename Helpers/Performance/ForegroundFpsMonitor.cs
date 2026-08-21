using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

namespace TrayTemps
{
    internal sealed class ForegroundFpsMonitor : IDisposable
    {
        private const uint ErrorSuccess = 0;
        private const uint ErrorAlreadyExists = 183;
        private const uint ErrorInsufficientBuffer = 122;
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
        private const ushort DxgiPresentMultiplaneOverlayStartEventId = 0x0037;
        private const ushort D3d9PresentStartEventId = 0x0001;
        private const ushort DxgKrnlPresentHistoryStartEventId = 0x00ab;
        private const ushort DxgKrnlPresentHistoryDetailedStartEventId = 0x00d7;
        private const ushort EventHeaderFlag32Bit = 0x0020;
        private const uint DxgiPresentTest = 0x00000001;
        private const uint DxgKrnlRedirectedFlipModel = 2;
        private const uint DxgKrnlRedirectedBltModel = 3;
        private const ulong DxgKrnlBaseKeyword = 0x0000000000000001;
        private const ulong DxgKrnlPresentKeyword = 0x0000000008000000;
        private const ushort TdhInTypeUInt32 = 8;
        private const uint PropertyStruct = 0x00000001;
        private const uint PropertyParamLength = 0x00000002;
        private const uint PropertyParamCount = 0x00000004;
        private const int MaximumTdhMetadataSize = 1024 * 1024;
        private const int MaximumFallbackCandidates = 8;
        private const int FallbackDominanceRatio = 2;
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
        private static readonly Guid DxgKrnlProviderGuid =
            new Guid("802EC45A-1E99-4B83-9920-87C98277BA9D");
        private const string HistoryModelPropertyName = "Model";

        private readonly long _qpcFrequency;
        private readonly long _calculationWindowTicks;
        private readonly long _staleValueTicks;
        private readonly long _streamStaleTicks;
        private readonly bool _isWindows11OrGreater;
        private readonly FallbackCandidate[] _fallbackCandidates =
            new FallbackCandidate[MaximumFallbackCandidates];
        private readonly sbyte[] _history171SchemaStates = new sbyte[byte.MaxValue + 1];
        private readonly sbyte[] _history215SchemaStates = new sbyte[byte.MaxValue + 1];
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
        private int _fallbackObservedGeneration;
        private int _fallbackCandidateCount;
        private bool _fallbackCandidateOverflow;
        private long _fallbackQualificationStartTimestamp;
        private long _fallbackLastObservedTimestamp;
        private ushort _selectedFallbackEventId;
        private uint _selectedFallbackModel;
        private int _selectedFallbackThreadId;
        private long _selectedFallbackStreamTimestamp;
        private long _fallbackWindowStartTimestamp;
        private int _fallbackPresentIntervals;
        private int _fallbackLatestFps = -1;
        private long _fallbackLastPresentTimestamp;
        private long _lastFlushTimestamp;
        private bool _dxgKrnlFallbackEnabled;
        private bool _running;
        private bool _disposed;
#if DEBUG
        private byte _debugPublishedBackend;
#endif

        internal ForegroundFpsMonitor()
        {
            QueryPerformanceFrequency(out _qpcFrequency);
            if (_qpcFrequency <= 0)
                _qpcFrequency = 10000000L;

            _calculationWindowTicks = Math.Max(1L, _qpcFrequency / 4L);
            _staleValueTicks = Math.Max(1L, _qpcFrequency * 3L);
            _streamStaleTicks = Math.Max(1L, _qpcFrequency / 2L);
            _isWindows11OrGreater = IsWindows11OrGreater();
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

                if (!EnablePresentProvider(
                        DxgiProviderGuid,
                        DxgiPresentStartEventId,
                        DxgiPresentMultiplaneOverlayStartEventId) ||
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

                _dxgKrnlFallbackEnabled = EnableDxgKrnlFallbackProvider(out uint fallbackStatus);
                if (!_dxgKrnlFallbackEnabled)
                    FpsDebug("DxgKrnl fallback provider unavailable: 0x" + fallbackStatus.ToString("X"));

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
            _dxgKrnlFallbackEnabled = false;
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
                Volatile.Write(ref _fallbackLatestFps, -1);
                Volatile.Write(ref _fallbackLastPresentTimestamp, 0L);
            }
        }

        internal int? GetLatestFps()
        {
            if (!_running)
                return null;

            long lastPresent = Volatile.Read(ref _lastPresentTimestamp);
            QueryPerformanceCounter(out long now);
            int value = Volatile.Read(ref _latestFps);
            if (lastPresent > 0 && now >= lastPresent &&
                now - lastPresent <= _staleValueTicks && value >= 0)
            {
                DebugPublishedResult(1, value);
                return value;
            }

            long fallbackLastPresent = Volatile.Read(ref _fallbackLastPresentTimestamp);
            int fallbackValue = Volatile.Read(ref _fallbackLatestFps);
            if (fallbackLastPresent > 0 && now >= fallbackLastPresent &&
                now - fallbackLastPresent <= _staleValueTicks && fallbackValue >= 0)
            {
                DebugPublishedResult(2, fallbackValue);
                return fallbackValue;
            }

            DebugPublishedResult(0, -1);
            return null;
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
            Volatile.Write(ref _fallbackLatestFps, -1);
            Volatile.Write(ref _fallbackLastPresentTimestamp, 0L);

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
                if (_dxgKrnlFallbackEnabled)
                {
                    Guid dxgKrnl = DxgKrnlProviderGuid;
                    EnableTraceEx2(
                        _sessionHandle, ref dxgKrnl, EventControlCodeDisableProvider,
                        0, 0, 0, 0, IntPtr.Zero);
                }
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
            _dxgKrnlFallbackEnabled = false;
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

        private unsafe bool EnablePresentProvider(
            Guid providerGuid,
            ushort eventId,
            ushort additionalEventId = 0)
        {
            var filter = new EventFilterEventIds
            {
                FilterIn = 1,
                Count = (ushort)(additionalEventId == 0 ? 1 : 2),
                EventId = eventId,
                AdditionalEventId = additionalEventId
            };
            var descriptor = new EventFilterDescriptor
            {
                Ptr = (ulong)&filter,
                Size = (uint)(sizeof(EventFilterEventIds) -
                    (additionalEventId == 0 ? sizeof(ushort) : 0)),
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

        private unsafe bool EnableDxgKrnlFallbackProvider(out uint status)
        {
            var filter = new EventFilterEventIds
            {
                FilterIn = 1,
                Count = 2,
                EventId = DxgKrnlPresentHistoryStartEventId,
                AdditionalEventId = DxgKrnlPresentHistoryDetailedStartEventId
            };
            var descriptor = new EventFilterDescriptor
            {
                Ptr = (ulong)&filter,
                Size = (uint)sizeof(EventFilterEventIds),
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

            ulong matchAnyKeyword = DxgKrnlBaseKeyword;
            if (_isWindows11OrGreater)
                matchAnyKeyword |= DxgKrnlPresentKeyword;

            Guid provider = DxgKrnlProviderGuid;
            status = EnableTraceEx2WithParameters(
                _sessionHandle,
                ref provider,
                EventControlCodeEnableProvider,
                0,
                matchAnyKeyword,
                DxgKrnlBaseKeyword,
                0,
                ref parameters);

            // This provider is optional. Do not retry unfiltered and never
            // fail the already-enabled DXGI/D3D9 monitor.
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
            bool isDxgiPresent =
                (eventId == DxgiPresentStartEventId ||
                 eventId == DxgiPresentMultiplaneOverlayStartEventId) &&
                *(uint*)(header + 24) == 0xCA11C036u &&
                *(uint*)(header + 28) == 0x4A2D0102u &&
                *(uint*)(header + 32) == 0x3CF0ADA6u &&
                *(uint*)(header + 36) == 0xC9D3D5FEu;
            bool isD3d9Present = eventId == D3d9PresentStartEventId &&
                *(uint*)(header + 24) == 0x783ACA0Au &&
                *(uint*)(header + 28) == 0x4D7F790Eu &&
                *(uint*)(header + 32) == 0x85AA5184u &&
                *(uint*)(header + 36) == 0xB9C61105u;
            bool isDxgKrnlFallback = _dxgKrnlFallbackEnabled &&
                (eventId == DxgKrnlPresentHistoryStartEventId ||
                 eventId == DxgKrnlPresentHistoryDetailedStartEventId) &&
                *(uint*)(header + 24) == 0x802EC45Au &&
                *(uint*)(header + 28) == 0x4B831E99u &&
                *(uint*)(header + 32) == 0xC9872099u &&
                *(uint*)(header + 36) == 0x9DBA7782u;
            if (!isDxgiPresent && !isD3d9Present && !isDxgKrnlFallback)
                return;

            int generation = Volatile.Read(ref _targetGeneration);
            long timestamp = *(long*)(header + 16);
            if (isDxgKrnlFallback)
            {
                try
                {
                    HandleFallbackEvent(eventRecord, header, eventId, generation, timestamp);
                }
                catch
                {
                    _dxgKrnlFallbackEnabled = false;
                    ResetFallbackState(generation);
                    FpsDebug("DxgKrnl fallback disabled after callback failure");
                }
                return;
            }

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
            ResetFallbackForPrimary(generation);
            _windowStartTimestamp = timestamp;
            _presentIntervals = 0;
        }

        private unsafe void HandleFallbackEvent(
            IntPtr eventRecord,
            byte* header,
            ushort eventId,
            int generation,
            long timestamp)
        {
            if (timestamp <= 0)
                return;

            if (_fallbackObservedGeneration != generation)
                ResetFallbackState(generation);

            if (HasValidPrimaryAt(timestamp))
            {
                ResetFallbackForPrimary(generation);
                return;
            }

            byte version = *(header + 42);
            byte opcode = *(header + 45);
            int threadId = *(int*)(header + 8);
            if (threadId == 0)
                return;

            if (opcode != 1 ||
                !TryReadHistoryModel(eventRecord, header, eventId, version, out uint model))
            {
                return;
            }

            if (model != DxgKrnlRedirectedFlipModel &&
                model != DxgKrnlRedirectedBltModel)
            {
                return;
            }

            ProcessFallbackCandidate(eventId, model, threadId, timestamp);
        }

        private bool HasValidPrimaryAt(long timestamp)
        {
            int value = Volatile.Read(ref _latestFps);
            long lastPresent = Volatile.Read(ref _lastPresentTimestamp);
            if (value < 0 || lastPresent <= 0)
                return false;

            return timestamp <= lastPresent ||
                timestamp - lastPresent <= _staleValueTicks;
        }

        private unsafe bool TryReadHistoryModel(
            IntPtr eventRecord,
            byte* header,
            ushort eventId,
            byte version,
            out uint model)
        {
            model = 0;
            if (version != 0 && version != 2)
            {
                RejectHistorySchema(eventId, version, "unknown version");
                return false;
            }

            sbyte[] schemaStates = eventId == DxgKrnlPresentHistoryStartEventId
                ? _history171SchemaStates
                : _history215SchemaStates;
            sbyte schemaState = schemaStates[version];
            if (schemaState == 0)
            {
                bool valid = ValidateHistorySchema(eventRecord, eventId, version);
                schemaStates[version] = (sbyte)(valid ? 1 : -1);
                if (!valid)
                {
                    FpsDebug("DxgKrnl event " + eventId +
                        " rejected TDH schema version " + version);
                    return false;
                }
            }
            else if (schemaState < 0)
            {
                return false;
            }

            ushort userDataLength = *(ushort*)(header + EventRecordUserDataLengthOffsetX64);
            byte* userData = *(byte**)(header + EventRecordUserDataOffsetX64);
            if (userData == null || userDataLength < sizeof(uint))
            {
                FpsDebug("DxgKrnl event " + eventId + " rejected short payload");
                return false;
            }

            fixed (char* propertyName = HistoryModelPropertyName)
            {
                var descriptor = new PropertyDataDescriptor
                {
                    PropertyName = (ulong)propertyName,
                    ArrayIndex = uint.MaxValue
                };
                uint status = TdhGetProperty(
                    eventRecord,
                    0,
                    IntPtr.Zero,
                    1,
                    ref descriptor,
                    sizeof(uint),
                    out model);
                if (status != ErrorSuccess)
                {
                    FpsDebug("DxgKrnl event " + eventId +
                        " rejected TDH property status 0x" + status.ToString("X"));
                    model = 0;
                    return false;
                }
            }

            return true;
        }

        private void RejectHistorySchema(ushort eventId, byte version, string reason)
        {
            sbyte[] schemaStates = eventId == DxgKrnlPresentHistoryStartEventId
                ? _history171SchemaStates
                : _history215SchemaStates;
            if (schemaStates[version] == 0)
            {
                schemaStates[version] = -1;
                FpsDebug("DxgKrnl event " + eventId + " rejected " + reason +
                    " " + version);
            }
        }

        private unsafe bool ValidateHistorySchema(
            IntPtr eventRecord,
            ushort eventId,
            byte version)
        {
            uint bufferSize = 0;
            uint status = TdhGetEventInformation(
                eventRecord,
                0,
                IntPtr.Zero,
                IntPtr.Zero,
                ref bufferSize);
            if (status != ErrorInsufficientBuffer ||
                bufferSize < sizeof(TraceEventInfoHeader) ||
                bufferSize > MaximumTdhMetadataSize)
            {
                return false;
            }

            IntPtr metadataBuffer = Marshal.AllocHGlobal((int)bufferSize);
            if (metadataBuffer == IntPtr.Zero)
                return false;

            try
            {
                uint actualSize = bufferSize;
                status = TdhGetEventInformation(
                    eventRecord,
                    0,
                    IntPtr.Zero,
                    metadataBuffer,
                    ref actualSize);
                if (status != ErrorSuccess || actualSize > bufferSize ||
                    actualSize < sizeof(TraceEventInfoHeader))
                {
                    return false;
                }

                byte* metadata = (byte*)metadataBuffer.ToPointer();
                TraceEventInfoHeader* info = (TraceEventInfoHeader*)metadata;
                if (info->ProviderGuid != DxgKrnlProviderGuid ||
                    info->EventDescriptor.Id != eventId ||
                    info->EventDescriptor.Version != version ||
                    info->TopLevelPropertyCount > info->PropertyCount ||
                    info->PropertyCount > 1024)
                {
                    return false;
                }

                long requiredSize = sizeof(TraceEventInfoHeader) +
                    (long)info->PropertyCount * sizeof(EventPropertyInfo);
                if (requiredSize > actualSize)
                    return false;

                EventPropertyInfo* properties =
                    (EventPropertyInfo*)(metadata + sizeof(TraceEventInfoHeader));
                for (uint index = 0; index < info->TopLevelPropertyCount; index++)
                {
                    EventPropertyInfo* property = properties + index;
                    if (!MetadataNameEquals(
                            metadata,
                            actualSize,
                            property->NameOffset,
                            HistoryModelPropertyName))
                    {
                        continue;
                    }

                    if ((property->Flags &
                            (PropertyStruct | PropertyParamLength | PropertyParamCount)) != 0 ||
                        property->InType != TdhInTypeUInt32 ||
                        property->Count != 1 ||
                        (property->Length != 0 && property->Length != sizeof(uint)))
                    {
                        return false;
                    }

                    return true;
                }

                return false;
            }
            finally
            {
                Marshal.FreeHGlobal(metadataBuffer);
            }
        }

        private static unsafe bool MetadataNameEquals(
            byte* metadata,
            uint metadataSize,
            uint nameOffset,
            string expected)
        {
            if ((nameOffset & 1) != 0 || nameOffset >= metadataSize)
                return false;

            uint availableCharacters = (metadataSize - nameOffset) / sizeof(char);
            if (availableCharacters <= expected.Length)
                return false;

            char* name = (char*)(metadata + nameOffset);
            for (int index = 0; index < expected.Length; index++)
            {
                if (name[index] != expected[index])
                    return false;
            }

            return name[expected.Length] == '\0';
        }

        private void ProcessFallbackCandidate(
            ushort eventId,
            uint model,
            int threadId,
            long timestamp)
        {
            if (_fallbackLastObservedTimestamp > 0 &&
                timestamp < _fallbackLastObservedTimestamp)
            {
                int generation = _fallbackObservedGeneration;
                ResetFallbackState(generation);
            }
            if (timestamp > _fallbackLastObservedTimestamp)
                _fallbackLastObservedTimestamp = timestamp;

            if (_selectedFallbackEventId != 0)
            {
                bool matches = eventId == _selectedFallbackEventId &&
                    model == _selectedFallbackModel &&
                    threadId == _selectedFallbackThreadId;
                if (matches)
                {
                    if (timestamp < _selectedFallbackStreamTimestamp)
                    {
                        int generation = _fallbackObservedGeneration;
                        ResetFallbackState(generation);
                        AddFallbackCandidate(eventId, model, threadId, timestamp);
                        return;
                    }
                    if (timestamp == _selectedFallbackStreamTimestamp)
                        return;

                    if (timestamp - _selectedFallbackStreamTimestamp > _streamStaleTicks)
                    {
                        int generation = _fallbackObservedGeneration;
                        ResetFallbackState(generation);
                        AddFallbackCandidate(eventId, model, threadId, timestamp);
                        return;
                    }

                    _selectedFallbackStreamTimestamp = timestamp;
                    SampleFallbackTimestamp(timestamp);
                    return;
                }

                if (timestamp <= _selectedFallbackStreamTimestamp ||
                    timestamp - _selectedFallbackStreamTimestamp <= _streamStaleTicks)
                {
                    return;
                }

                int observedGeneration = _fallbackObservedGeneration;
                ResetFallbackState(observedGeneration);
                AddFallbackCandidate(eventId, model, threadId, timestamp);
                return;
            }

            AddFallbackCandidate(eventId, model, threadId, timestamp);
        }

        private void AddFallbackCandidate(
            ushort eventId,
            uint model,
            int threadId,
            long timestamp)
        {
            if (_fallbackQualificationStartTimestamp <= 0)
                _fallbackQualificationStartTimestamp = timestamp;

            int candidateIndex = FindFallbackCandidate(eventId, model, threadId);
            if (candidateIndex < 0)
            {
                if (_fallbackCandidateCount >= _fallbackCandidates.Length)
                {
                    _fallbackCandidateOverflow = true;
                }
                else
                {
                    candidateIndex = _fallbackCandidateCount++;
                    _fallbackCandidates[candidateIndex] = new FallbackCandidate
                    {
                        EventId = eventId,
                        Model = model,
                        ThreadId = threadId,
                        FirstTimestamp = timestamp,
                        LastTimestamp = timestamp,
                        Count = 1
                    };
                }
            }
            else
            {
                FallbackCandidate candidate = _fallbackCandidates[candidateIndex];
                if (timestamp < candidate.LastTimestamp)
                {
                    int generation = _fallbackObservedGeneration;
                    ResetFallbackState(generation);
                    AddFallbackCandidate(eventId, model, threadId, timestamp);
                    return;
                }
                if (timestamp == candidate.LastTimestamp)
                    return;

                if (timestamp - candidate.LastTimestamp > _streamStaleTicks)
                {
                    candidate.FirstTimestamp = timestamp;
                    candidate.LastTimestamp = timestamp;
                    candidate.Count = 1;
                }
                else
                {
                    candidate.LastTimestamp = timestamp;
                    if (candidate.Count < int.MaxValue)
                        candidate.Count++;
                }
                _fallbackCandidates[candidateIndex] = candidate;
            }

            long qualificationElapsed = timestamp - _fallbackQualificationStartTimestamp;
            if (qualificationElapsed >= _streamStaleTicks)
            {
                if (TrySelectFallbackSource(timestamp))
                    return;

                ClearFallbackCandidates();
                _fallbackQualificationStartTimestamp = timestamp;
                _fallbackCandidates[0] = new FallbackCandidate
                {
                    EventId = eventId,
                    Model = model,
                    ThreadId = threadId,
                    FirstTimestamp = timestamp,
                    LastTimestamp = timestamp,
                    Count = 1
                };
                _fallbackCandidateCount = 1;
            }
        }

        private int FindFallbackCandidate(
            ushort eventId,
            uint model,
            int threadId)
        {
            for (int index = 0; index < _fallbackCandidateCount; index++)
            {
                FallbackCandidate candidate = _fallbackCandidates[index];
                if (candidate.EventId == eventId &&
                    candidate.Model == model &&
                    candidate.ThreadId == threadId)
                {
                    return index;
                }
            }

            return -1;
        }

        private bool TrySelectFallbackSource(long timestamp)
        {
            if (_fallbackCandidateOverflow)
                return false;

            int bestPriority = int.MaxValue;
            int bestIndex = -1;
            int secondBestCount = 0;
            for (int index = 0; index < _fallbackCandidateCount; index++)
            {
                FallbackCandidate candidate = _fallbackCandidates[index];
                if (candidate.Count < 2 ||
                    candidate.LastTimestamp <= candidate.FirstTimestamp ||
                    candidate.LastTimestamp - candidate.FirstTimestamp < _calculationWindowTicks ||
                    timestamp < candidate.LastTimestamp ||
                    timestamp - candidate.LastTimestamp > _streamStaleTicks)
                {
                    continue;
                }

                int priority = GetFallbackPriority(candidate.EventId);
                if (priority < bestPriority)
                {
                    bestPriority = priority;
                    bestIndex = index;
                    secondBestCount = 0;
                }
                else if (priority == bestPriority)
                {
                    if (bestIndex < 0 ||
                        candidate.Count > _fallbackCandidates[bestIndex].Count)
                    {
                        secondBestCount = bestIndex < 0
                            ? 0
                            : _fallbackCandidates[bestIndex].Count;
                        bestIndex = index;
                    }
                    else if (candidate.Count > secondBestCount)
                    {
                        secondBestCount = candidate.Count;
                    }
                }
            }

            if (bestIndex < 0)
                return false;

            FallbackCandidate best = _fallbackCandidates[bestIndex];
            if (secondBestCount > 0 &&
                best.Count < secondBestCount * FallbackDominanceRatio)
            {
                return false;
            }

            SelectFallbackSource(best);
            return true;
        }

        private static int GetFallbackPriority(ushort eventId)
        {
            return eventId == DxgKrnlPresentHistoryStartEventId ? 1 : 2;
        }

        private void SelectFallbackSource(FallbackCandidate candidate)
        {
            _selectedFallbackEventId = candidate.EventId;
            _selectedFallbackModel = candidate.Model;
            _selectedFallbackThreadId = candidate.ThreadId;
            _selectedFallbackStreamTimestamp = candidate.LastTimestamp;
            _fallbackWindowStartTimestamp = candidate.LastTimestamp;
            _fallbackPresentIntervals = 0;
            Volatile.Write(ref _fallbackLatestFps, -1);
            Volatile.Write(ref _fallbackLastPresentTimestamp, candidate.LastTimestamp);
            ClearFallbackCandidates();

            FpsDebug("DxgKrnl fallback selected event " + candidate.EventId +
                " model " + candidate.Model +
                " thread " + candidate.ThreadId);
        }

        private void SampleFallbackTimestamp(long timestamp)
        {
            Volatile.Write(ref _fallbackLastPresentTimestamp, timestamp);
            if (_fallbackWindowStartTimestamp <= 0)
            {
                _fallbackWindowStartTimestamp = timestamp;
                _fallbackPresentIntervals = 0;
                return;
            }

            if (timestamp <= _fallbackWindowStartTimestamp)
                return;

            _fallbackPresentIntervals++;
            long elapsed = timestamp - _fallbackWindowStartTimestamp;
            if (elapsed < _calculationWindowTicks)
                return;

            double framesPerSecond =
                _fallbackPresentIntervals * (double)_qpcFrequency / elapsed;
            int fps = (int)Math.Round(framesPerSecond, MidpointRounding.AwayFromZero);
            Volatile.Write(
                ref _fallbackLatestFps,
                Math.Max(0, Math.Min(9999, fps)));
            _fallbackWindowStartTimestamp = timestamp;
            _fallbackPresentIntervals = 0;
        }

        private void ResetFallbackForPrimary(int generation)
        {
            if (_fallbackObservedGeneration == generation &&
                _selectedFallbackEventId == 0 &&
                _fallbackCandidateCount == 0 &&
                Volatile.Read(ref _fallbackLatestFps) < 0 &&
                Volatile.Read(ref _fallbackLastPresentTimestamp) == 0)
            {
                return;
            }

            ResetFallbackState(generation);
        }

        private void ResetFallbackState(int generation)
        {
            _fallbackObservedGeneration = generation;
            _fallbackLastObservedTimestamp = 0;
            _selectedFallbackEventId = 0;
            _selectedFallbackModel = 0;
            _selectedFallbackThreadId = 0;
            _selectedFallbackStreamTimestamp = 0;
            _fallbackWindowStartTimestamp = 0;
            _fallbackPresentIntervals = 0;
            ClearFallbackCandidates();
            Volatile.Write(ref _fallbackLatestFps, -1);
            Volatile.Write(ref _fallbackLastPresentTimestamp, 0L);
        }

        private void ClearFallbackCandidates()
        {
            for (int index = 0; index < _fallbackCandidateCount; index++)
                _fallbackCandidates[index] = default(FallbackCandidate);
            _fallbackCandidateCount = 0;
            _fallbackCandidateOverflow = false;
            _fallbackQualificationStartTimestamp = 0;
        }

        [Conditional("DEBUG")]
        private static void FpsDebug(string message)
        {
            Debug.WriteLine("ForegroundFpsMonitor: " + message);
        }

        [Conditional("DEBUG")]
        private void DebugPublishedResult(byte backend, int fps)
        {
#if DEBUG
            if (_debugPublishedBackend == backend)
                return;

            _debugPublishedBackend = backend;
            string source = backend == 1
                ? "primary"
                : backend == 2 ? "fallback" : "none";
            Debug.WriteLine("ForegroundFpsMonitor: publishing " + source +
                (fps >= 0 ? " FPS " + fps : string.Empty));
#endif
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

        private static unsafe bool IsWindows11OrGreater()
        {
            var versionInfo = new RtlOsVersionInfo
            {
                Size = (uint)sizeof(RtlOsVersionInfo)
            };
            return RtlGetVersion(ref versionInfo) == 0 &&
                versionInfo.MajorVersion >= 10 &&
                versionInfo.BuildNumber >= 22000;
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
        private struct EventFilterEventIds
        {
            internal byte FilterIn;
            internal byte Reserved;
            internal ushort Count;
            internal ushort EventId;
            internal ushort AdditionalEventId;
        }

        private struct FallbackCandidate
        {
            internal ushort EventId;
            internal uint Model;
            internal int ThreadId;
            internal long FirstTimestamp;
            internal long LastTimestamp;
            internal int Count;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct EventDescriptor
        {
            internal ushort Id;
            internal byte Version;
            internal byte Channel;
            internal byte Level;
            internal byte Opcode;
            internal ushort Task;
            internal ulong Keyword;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct TraceEventInfoHeader
        {
            internal Guid ProviderGuid;
            internal Guid EventGuid;
            internal EventDescriptor EventDescriptor;
            internal uint DecodingSource;
            internal uint ProviderNameOffset;
            internal uint LevelNameOffset;
            internal uint ChannelNameOffset;
            internal uint KeywordsNameOffset;
            internal uint TaskNameOffset;
            internal uint OpcodeNameOffset;
            internal uint EventMessageOffset;
            internal uint ProviderMessageOffset;
            internal uint BinaryXmlOffset;
            internal uint BinaryXmlSize;
            internal uint EventNameOffset;
            internal uint EventAttributesOffset;
            internal uint PropertyCount;
            internal uint TopLevelPropertyCount;
            internal uint Flags;
        }

        [StructLayout(LayoutKind.Explicit, Size = 24)]
        private struct EventPropertyInfo
        {
            [FieldOffset(0)] internal uint Flags;
            [FieldOffset(4)] internal uint NameOffset;
            [FieldOffset(8)] internal ushort InType;
            [FieldOffset(10)] internal ushort OutType;
            [FieldOffset(12)] internal uint MapNameOffset;
            [FieldOffset(16)] internal ushort Count;
            [FieldOffset(18)] internal ushort Length;
            [FieldOffset(20)] internal uint Reserved;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct PropertyDataDescriptor
        {
            internal ulong PropertyName;
            internal uint ArrayIndex;
            internal uint Reserved;
        }

        private unsafe struct RtlOsVersionInfo
        {
            internal uint Size;
            internal uint MajorVersion;
            internal uint MinorVersion;
            internal uint BuildNumber;
            internal uint PlatformId;
            internal fixed char ServicePack[128];
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

        [DllImport("tdh.dll")]
        private static extern uint TdhGetEventInformation(
            IntPtr eventRecord,
            uint tdhContextCount,
            IntPtr tdhContext,
            IntPtr buffer,
            ref uint bufferSize);

        [DllImport("tdh.dll")]
        private static extern uint TdhGetProperty(
            IntPtr eventRecord,
            uint tdhContextCount,
            IntPtr tdhContext,
            uint propertyDataCount,
            ref PropertyDataDescriptor propertyData,
            uint bufferSize,
            out uint buffer);

        [DllImport("ntdll.dll")]
        private static extern int RtlGetVersion(ref RtlOsVersionInfo versionInfo);

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
