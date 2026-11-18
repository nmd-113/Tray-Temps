using System;

namespace TrayTemps
{
    // Optional history only: no per-frame allocation, no sorting on the ETW thread.
    internal sealed class FrameTimeWindow
    {
        // Holds ten seconds beyond the FPS display's 9999 FPS range, with headroom.
        internal const int Capacity = 131072;
        private readonly long[] _timestamps = new long[Capacity];
        private readonly long[] _intervals = new long[Capacity];
        private readonly long[] _snapshot = new long[Capacity];
        private readonly object _gate = new object(), _calculationGate = new object();
        private readonly long _frequency, _windowTicks, _staleTicks;
        private int _start, _count, _generation, _version;
        private long _lastTimestamp, _lastCalculation, _discardedTimestamp;
        private int? _cached;
        private long _totalTicks;
        private Bucket[] _buckets;
        private struct Bucket { internal long Id, Sum, Peak; internal int Count; }

        internal FrameTimeWindow(long frequency, bool timing = true)
        {
            if (frequency <= 0 || frequency > long.MaxValue / 10) throw new ArgumentOutOfRangeException(nameof(frequency));
            _frequency = frequency; _windowTicks = frequency * 10; _staleTicks = frequency * 3;
            if (timing) _buckets = new Bucket[128];
        }

        internal void SetTimingEnabled(bool enabled)
        {
            lock (_gate)
            {
                if (enabled == (_buckets != null)) return;
                _buckets = enabled ? new Bucket[128] : null;
                _totalTicks = 0;
                if (enabled)
                    for (int i = 0; i < _count; i++) _totalTicks += _intervals[(_start + i) & (Capacity - 1)];
            }
        }

        internal void Reset(int generation, long timestamp = 0)
        {
            lock (_gate)
            {
                if (generation < _generation) return;
                ResetCore(generation, timestamp);
            }
        }

        private void ResetCore(int generation, long timestamp)
        {
            _generation = generation; _start = _count = 0;
            _lastTimestamp = timestamp; _lastCalculation = 0; _discardedTimestamp = 0; _cached = null; ++_version;
            _totalTicks = 0;
            if (_buckets != null) Array.Clear(_buckets, 0, _buckets.Length);
        }

        internal void Add(long timestamp, int generation)
        {
            lock (_gate)
            {
                if (generation < _generation || timestamp <= 0) return;
                if (generation != _generation) ResetCore(generation, 0);
                if (timestamp <= _lastTimestamp) return;
                long previous = _lastTimestamp;
                if (previous == 0 || timestamp - previous > _staleTicks)
                {
                    ResetCore(generation, timestamp);
                    return;
                }
                int index = (_start + _count) & (Capacity - 1);
                if (_count == Capacity) { _discardedTimestamp = _timestamps[index]; if (_buckets != null) _totalTicks -= _intervals[index]; }
                _timestamps[index] = timestamp; _intervals[index] = timestamp - previous;
                if (_buckets != null)
                {
                    _totalTicks += _intervals[index];
                    long bucketId = timestamp / Math.Max(1, _frequency / 10);
                    int bucketIndex = (int)(bucketId & 127);
                    if (_buckets[bucketIndex].Id != bucketId) _buckets[bucketIndex] = new Bucket { Id = bucketId };
                    _buckets[bucketIndex].Sum += timestamp - previous;
                    _buckets[bucketIndex].Count++;
                    _buckets[bucketIndex].Peak = Math.Max(_buckets[bucketIndex].Peak, timestamp - previous);
                }
                if (_count == Capacity) _start = (_start + 1) & (Capacity - 1);
                else ++_count;
                _lastTimestamp = timestamp;
            }
        }

        internal int? GetValue(long now, int generation)
        {
            // Only readers share the reusable scratch array. Capture never takes this lock.
            lock (_calculationGate)
            {
                int count, version;
                lock (_gate)
                {
                    if (generation != _generation || _lastTimestamp <= 0)
                    {
                        _cached = null;
                        return null;
                    }
                    // A Present can arrive after the UI sampled QPC but before
                    // taking this lock. Include it without flickering to a dash.
                    now = Math.Max(now, _lastTimestamp);
                    if (now - _lastTimestamp > _staleTicks) { _cached = null; return null; }
                    long cutoff = now - _windowTicks;
                    // At extreme rates, never label a truncated history as a full
                    // rolling window. Normal display-range rates fit in Capacity.
                    if (_discardedTimestamp > 0 && _discardedTimestamp >= cutoff) { _cached = null; return null; }
                    while (_count > 0 && _timestamps[_start] < cutoff)
                    {
                        if (_buckets != null) _totalTicks -= _intervals[_start];
                        _start = (_start + 1) & (Capacity - 1); --_count;
                    }
                    if (_count < 100) { _cached = null; return null; }
                    if (_lastCalculation > 0 && now - _lastCalculation < _frequency) return _cached;
                    count = _count; version = _version;
                    int first = Math.Min(count, Capacity - _start);
                    Array.Copy(_intervals, _start, _snapshot, 0, first);
                    if (first < count) Array.Copy(_intervals, 0, _snapshot, first, count - first);
                    _lastCalculation = now;
                }
                Array.Sort(_snapshot, 0, count);
                int slowCount = (count + 99) / 100;
                double total = 0;
                for (int index = count - slowCount; index < count; ++index) total += _snapshot[index];
                int value = (int)Math.Min(9999, Math.Round(_frequency * (double)slowCount / total, MidpointRounding.AwayFromZero));
                lock (_gate)
                {
                    if (_version != version || _generation != generation) return null;
                    _cached = value;
                    return _cached;
                }
            }
        }

        internal FrameStatistics GetStatistics(long now, int generation, int refreshMilliseconds, bool graph)
        {
            lock (_gate)
            {
                var result = new FrameStatistics { Version = _version };
                if (_buckets == null) return result;
                if (generation != _generation || _lastTimestamp <= 0 || now - _lastTimestamp > _staleTicks) return result;
                now = Math.Max(now, _lastTimestamp);
                long cutoff = now - _windowTicks;
                while (_count > 0 && _timestamps[_start] < cutoff)
                { _totalTicks -= _intervals[_start]; _start = (_start + 1) & (Capacity - 1); --_count; }
                if (_count == 0 || (_discardedTimestamp > 0 && _discardedTimestamp >= cutoff)) return result;
                if (_lastTimestamp - (_timestamps[_start] - _intervals[_start]) >= _frequency)
                    result.AverageFps = _count * (double)_frequency / _totalTicks;
                // ETW delivers batches after their Present timestamps. Anchor the
                // short mean to the latest accepted frame, with freshness checked above.
                long recentCutoff = _lastTimestamp - _frequency * Math.Max(100, Math.Min(10000, refreshMilliseconds)) / 1000;
                long sum = 0, peak = 0; int count = 0;
                for (int i = _count - 1; i >= 0; i--)
                {
                    int index = (_start + i) & (Capacity - 1);
                    if (_timestamps[index] < recentCutoff) break;
                    sum += _intervals[index]; peak = Math.Max(peak, _intervals[index]); count++;
                }
                if (count > 0)
                {
                    result.FrameTimeMs = sum * 1000d / _frequency / count;
                    result.PeakFrameTimeMs = peak * 1000d / _frequency;
                }
                if (graph)
                {
                    result.Graph = new FrameGraphPoint[100];
                    long lastBucket = now / Math.Max(1, _frequency / 10);
                    result.GraphTimestamp = lastBucket * Math.Max(1, _frequency / 10);
                    for (int i = 0; i < result.Graph.Length; i++)
                    {
                        long id = lastBucket - 99 + i;
                        var bucket = _buckets[(int)(id & 127)];
                        result.Graph[i] = bucket.Id == id && bucket.Count > 0
                            ? new FrameGraphPoint(bucket.Sum * 1000d / _frequency / bucket.Count, bucket.Peak * 1000d / _frequency)
                            : new FrameGraphPoint(double.NaN, double.NaN);
                    }
                }
                return result;
            }
        }
    }
}
