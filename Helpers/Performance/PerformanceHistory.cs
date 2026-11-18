using System;
using System.Collections.Generic;

namespace TrayTemps
{
    public sealed class HistorySettings
    {
        public bool RecordingEnabled { get; set; }
        public int Minutes { get; set; } = 5;
        public int WindowWidth { get; set; } = 900;
        public int WindowHeight { get; set; } = 700;
        internal HistorySettings Clone() => (HistorySettings)MemberwiseClone();
        internal static HistorySettings Normalize(HistorySettings settings)
        {
            var result = settings?.Clone() ?? new HistorySettings();
            if (result.Minutes != 1 && result.Minutes != 5 && result.Minutes != 10 && result.Minutes != 30) result.Minutes = 5;
            result.WindowWidth = Math.Max(600, Math.Min(3000, result.WindowWidth));
            result.WindowHeight = Math.Max(400, Math.Min(2000, result.WindowHeight));
            return result;
        }
    }

    internal struct HistoryPoint
    {
        internal double Time, Value, Peak;
        internal long Source;
    }

    // UI-thread-owned, bounded to 30 minutes at the fastest supported refresh (250 ms).
    internal sealed class PerformanceHistory
    {
        internal const int Capacity = 7201;
        private sealed class Series { internal readonly HistoryPoint[] Points = new HistoryPoint[Capacity]; internal int Start, Count; }
        private readonly Dictionary<string, Series> _series = new Dictionary<string, Series>(StringComparer.Ordinal);
        internal double LastTime { get; private set; }
        internal void Add(string key, double time, double? value, long source = 0, double? peak = null)
        {
            if (key == "CurrentTime" || key == "FrameTimeGraph") return;
            if (!_series.TryGetValue(key, out var series))
            {
                if (Array.IndexOf(OsdMetricDefinitions.Keys, key) < 0) return;
                _series[key] = series = new Series();
            }
            if (series.Count > 0 && time <= series.Points[(series.Start + series.Count - 1) % Capacity].Time) return;
            while (series.Count > 0 && series.Points[series.Start].Time < time - 1800)
            { series.Start = (series.Start + 1) % Capacity; series.Count--; }
            int index = (series.Start + series.Count) % Capacity;
            double sample = value.HasValue && !double.IsInfinity(value.Value) ? value.Value : double.NaN;
            double maximum = peak.HasValue && !double.IsInfinity(peak.Value) && !double.IsNaN(peak.Value) ? Math.Max(sample, peak.Value) : sample;
            series.Points[index] = new HistoryPoint { Time = time, Value = sample, Peak = maximum, Source = source };
            if (series.Count == Capacity) series.Start = (series.Start + 1) % Capacity;
            else series.Count++;
            LastTime = Math.Max(LastTime, time);
        }
        internal void Pause(string key, double time)
        {
            if (_series.TryGetValue(key, out var series) && series.Count > 0 &&
                !double.IsNaN(series.Points[(series.Start + series.Count - 1) % Capacity].Value)) Add(key, time, null);
        }
        internal HistoryPoint[] Snapshot(string key, double end, int minutes)
        {
            if (!_series.TryGetValue(key, out var series)) return Array.Empty<HistoryPoint>();
            int first = 0;
            while (first < series.Count && series.Points[(series.Start + first) % Capacity].Time < end - minutes * 60) first++;
            var result = new HistoryPoint[series.Count - first];
            for (int i = 0; i < result.Length; i++) result[i] = series.Points[(series.Start + first + i) % Capacity];
            return result;
        }
        internal void Clear() { _series.Clear(); LastTime = 0; }
    }
}
