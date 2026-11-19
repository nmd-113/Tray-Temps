using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;

namespace TrayTemps
{
    public partial class MainForm
    {
        private static readonly FrameGraphPoint[] EmptyFrameGraph = Enumerable.Repeat(new FrameGraphPoint(double.NaN, double.NaN), 100).ToArray();
        private HistorySettings _historySettings = new HistorySettings();
        private readonly PerformanceHistory _history = new PerformanceHistory();
        private PerformanceHistoryDialog _historyDialog;
        private FrameStatistics _latestFrameStatistics;
        private long _lastFpsOsdRefresh;
        internal const int FrameGraphRefreshIntervalMilliseconds = 33;
        private const int FrameGraphCaptureIntervalMilliseconds = 100;
        private long _lastFrameStatisticsRefresh, _lastFpsForegroundRefresh, _lastFrameHistoryRefresh;
        private readonly Dictionary<string, OsdMetric> _displayedFrameMetrics = new Dictionary<string, OsdMetric>(StringComparer.Ordinal);
        private readonly Dictionary<string, int> _frameValueDigits = new Dictionary<string, int>(StringComparer.Ordinal);
        private OsdConfiguration _frameOsdConfiguration;
        private int _frameOsdGeneration;
        private long _frameOsdSource;
        private long _historyEpoch, _cpuHistorySource, _gpuHistorySource;
        private string _historyCpuId, _historyGpuId;
        internal HistorySettings HistoryPreferences => _historySettings;
        internal long HistoryVersion => _history.Version;
        internal double HistoryGraphEndTime => HistoryRecording ? HistoryNow : _history.LastTime;
        internal int HistoryGraphOptions
        {
            get
            {
                var config = EffectiveOsd;
                return (config.ShowFps ? 1 : 0) | (config.ShowAverageFps ? 2 : 0) | (config.ShowFpsOnePercentLow ? 4 : 0) |
                    (config.ShowRamUsage ? 8 : 0) | (config.ShowVramUsage ? 16 : 0) |
                    (config.ShowFrameTime || config.ShowFrameTimeGraph ? 32 : 0) | (tempsFahrenheit.Checked ? 64 : 0) |
                    (IsLightModeEnabled ? 128 : 0) | (HistoryRecording ? 256 : 0) | (_historySettings.Minutes << 10);
            }
        }
        private bool HistoryRecording => _historySettings.RecordingEnabled && !_isShutdownInitiated && !_resourcesDisposed;
        private OsdConfiguration EffectiveOsd => _osdPreviewConfiguration ?? _osdConfiguration;
        private bool HistoryNeedsFrames => HistoryRecording && OsdMetricDefinitions.NeedsFrames(EffectiveOsd);
        private static double HistoryNow => Stopwatch.GetTimestamp() / (double)Stopwatch.Frequency;

        private void HistoryButton_Click(object sender, EventArgs e)
        {
            if (_historyDialog != null && !_historyDialog.IsDisposed)
            {
                if (_historyDialog.WindowState == System.Windows.Forms.FormWindowState.Minimized)
                    _historyDialog.WindowState = System.Windows.Forms.FormWindowState.Normal;
                _historyDialog.Show(); _historyDialog.Activate(); return;
            }
            _historyDialog = new PerformanceHistoryDialog(this);
            _historyDialog.FormClosed += (s, args) => _historyDialog = null;
            _historyDialog.Show(this);
        }
        internal void SetHistoryRecording(bool enabled)
        {
            if (_historySettings.RecordingEnabled == enabled) return;
            _historySettings.RecordingEnabled = enabled;
            _lastFrameHistoryRefresh = 0;
            _historyEpoch++;
            SynchronizeFrameCapture();
            if (enabled) RecordHardwareHistory(GetCurrentKnownTemperature(_cpuTempSensor), GetCurrentKnownTemperature(_gpuTempSensor));
            if (_settingsLoaded) SaveSettings();
        }
        internal void SetHistoryRange(int minutes)
        {
            if (minutes != 1 && minutes != 5 && minutes != 10 && minutes != 30) return;
            _historySettings.Minutes = minutes;
            if (_settingsLoaded) SaveSettings();
        }
        internal void SaveHistoryWindowSize(Size size)
        {
            _historySettings.WindowWidth = size.Width; _historySettings.WindowHeight = size.Height;
            if (_settingsLoaded && !_isShutdownInitiated) SaveSettings();
        }
        internal void ClearHistory() => _history.Clear();

        private void SynchronizeFrameCapture()
        {
            if (_isShutdownInitiated || _resourcesDisposed) { StopFpsMonitoring(); return; }
            var config = EffectiveOsd;
            if (HistoryRecording) MarkDisabledHistoryMetrics(config);
            bool needed = (osdEnable.Checked || HistoryRecording) && OsdMetricDefinitions.NeedsFrames(config);
            int interval = osdEnable.Checked && config.ShowFrameTimeGraph ? FrameGraphRefreshIntervalMilliseconds :
                HistoryNeedsFrames ? 250 : GetEffectiveFpsRefreshIntervalMilliseconds(config);
            UpdateFpsMonitoring(needed, interval, config.ShowFpsOnePercentLow, OsdMetricDefinitions.NeedsTiming(config));
        }

        private void ReadFrameStatistics(OsdConfiguration config, bool displayGraph = true)
        {
            _latestFrameStatistics = _fpsMonitor?.GetFrameStatistics(GetEffectiveFpsRefreshIntervalMilliseconds(config), displayGraph && osdEnable.Checked && config.ShowFrameTimeGraph) ?? default(FrameStatistics);
            _lastFrameStatisticsRefresh = Stopwatch.GetTimestamp();
        }

        private bool FrameOsdRefreshDue(OsdConfiguration config, long now) =>
            _lastFpsOsdRefresh == 0 || !ReferenceEquals(_frameOsdConfiguration, config) ||
            _frameOsdGeneration != (_fpsMonitor?.TargetGeneration ?? 0) ||
            _frameOsdSource != _latestFrameStatistics.Source ||
            (now - _lastFpsOsdRefresh) * 1000d / Stopwatch.Frequency >= GetEffectiveFpsRefreshIntervalMilliseconds(config);

        private void BeginFrameOsdRefresh(OsdConfiguration config, long now)
        {
            if (_frameOsdGeneration != (_fpsMonitor?.TargetGeneration ?? 0) || _frameOsdSource != _latestFrameStatistics.Source)
                _frameValueDigits.Clear();
            _displayedFrameMetrics.Clear();
            _frameOsdConfiguration = config;
            _frameOsdGeneration = _fpsMonitor?.TargetGeneration ?? 0;
            _frameOsdSource = _latestFrameStatistics.Source;
            _lastFpsOsdRefresh = now;
        }

        private OsdMetric GetDisplayedFrameMetric(OsdConfiguration config, string key)
        {
            if (!_displayedFrameMetrics.TryGetValue(key, out var metric))
                _displayedFrameMetrics[key] = metric = CreateFrameMetric(config, key);
            return metric;
        }

        private OsdMetric CreateFrameMetric(OsdConfiguration config, string key)
        {
            if (key == "Fps" || key == "FpsOnePercentLow") return CreateFpsMetric(config, key == "FpsOnePercentLow");
            if (key == "FrameTimeGraph")
                return new OsdMetric(OsdMetricDefinitions.ResolveLabel(config, key), "")
                {
                    Key = key, Style = config.MetricStyles[key], ValueWidthTemplate = "graph",
                    Graph = _latestFrameStatistics.Graph ?? EmptyFrameGraph,
                    GraphOffset = _latestFrameStatistics.GraphTimestamp > 0
                        ? (float)Math.Max(0, (Stopwatch.GetTimestamp() - _latestFrameStatistics.GraphTimestamp) / (Stopwatch.Frequency / 10d)) : 0
                };
            double? value = key == "AverageFps" ? _latestFrameStatistics.AverageFps : _latestFrameStatistics.FrameTimeMs;
            string text = !value.HasValue ? null : key == "AverageFps" ? value.Value.ToString("0", CultureInfo.InvariantCulture) : value.Value.ToString("0.00", CultureInfo.InvariantCulture) + " ms";
            return CreateFrameValueMetric(config, key, text);
        }

        private OsdMetric CreateFrameValueMetric(OsdConfiguration config, string key, string value)
        {
            string suffix = key == "FrameTime" ? ".## ms" : "";
            string placeholder = key == "FrameTime" ? "---.-- ms" : "---";
            int digits = 3;
            if (value == null) _frameValueDigits.Remove(key);
            else
            {
                // Use the formatted value: rounding 999.995 can itself require four digits.
                int integerDigits = 0;
                while (integerDigits < value.Length && value[integerDigits] >= '0' && value[integerDigits] <= '9') integerDigits++;
                _frameValueDigits.TryGetValue(key, out int previousDigits);
                digits = Math.Max(3, Math.Max(previousDigits, integerDigits));
                _frameValueDigits[key] = digits;
            }
            return new OsdMetric(OsdMetricDefinitions.ResolveLabel(config, key), value ?? placeholder)
            {
                Key = key,
                Style = config.MetricStyles != null ? config.MetricStyles[key] : OsdMetricDefinitions.LegacyStyle(config, key),
                ValueWidthTemplate = new string('#', digits) + suffix,
                AlternateValueWidthTemplate = placeholder
            };
        }

        private void RecordHardwareHistory(float? cpu, float? gpu)
        {
            if (!HistoryRecording) return;
            MarkDisabledHistoryMetrics(EffectiveOsd);
            double time = HistoryNow;
            string cpuId = _selectedCpuHardware?.Identifier.ToString(), gpuId = _selectedGpuHardware?.Identifier.ToString();
            if (cpuId != _historyCpuId) { _historyCpuId = cpuId; _cpuHistorySource++; }
            if (gpuId != _historyGpuId) { _historyGpuId = gpuId; _gpuHistorySource++; }
            long cpuSource = (_historyEpoch << 32) | _cpuHistorySource, gpuSource = (_historyEpoch << 32) | _gpuHistorySource;
            _history.Add("CpuTemperature", time, cpu, cpuSource);
            _history.Add("GpuTemperature", time, gpu, gpuSource);
            _history.Add("CpuUsage", time, GetPreferredLoadValue(_selectedCpuHardware, true), cpuSource);
            _history.Add("GpuUsage", time, GetPreferredLoadValue(_selectedGpuHardware, false), gpuSource);
            if (EffectiveOsd.ShowRamUsage) _history.Add("RamUsage", time, GetMemoryUsageGb(GetCompletedMemoryHardware(), "Memory", out _), _historyEpoch);
            if (EffectiveOsd.ShowVramUsage) _history.Add("VramUsage", time, GetMemoryUsageGb(_selectedGpuHardware, "GPU Memory", out _), gpuSource);
        }

        private void MarkDisabledHistoryMetrics(OsdConfiguration config)
        {
            double time = HistoryNow;
            foreach (string key in OsdMetricDefinitions.Keys)
            {
                if (key == "RamUsage" || key == "VramUsage" || OsdMetricDefinitions.IsFrameMetric(key))
                    if (key != "FrameTimeGraph" && !(key == "FrameTime" && config.ShowFrameTimeGraph) && !OsdMetricDefinitions.IsVisible(config, key))
                        _history.Pause(key, time);
            }
        }
        private void RecordFrameHistory(OsdConfiguration config)
        {
            if (!HistoryRecording || _fpsMonitor == null) return;
            _fpsMonitor.GetLatestValues(out int? fps, out int? low);
            double time = HistoryNow;
            long source = (_latestFrameStatistics.Source != 0 ? _latestFrameStatistics.Source : _fpsMonitor.TargetGeneration) ^ (_historyEpoch << 48);
            if (config.ShowFps) _history.Add("Fps", time, fps, source);
            if (config.ShowFpsOnePercentLow) _history.Add("FpsOnePercentLow", time, low, source);
            if (config.ShowAverageFps) _history.Add("AverageFps", time, _latestFrameStatistics.AverageFps, source);
            if (config.ShowFrameTime || config.ShowFrameTimeGraph)
                _history.Add("FrameTime", time, _latestFrameStatistics.FrameTimeMs, source, _latestFrameStatistics.PeakFrameTimeMs);
        }

        internal void FillHistoryGraphs(HistoryGraphControl temperatures, HistoryGraphControl usage, HistoryGraphControl fps, HistoryGraphControl memory, HistoryGraphControl frames)
        {
            int minutes = _historySettings.Minutes;
            double end = HistoryGraphEndTime;
            var config = EffectiveOsd;
            bool light = IsLightModeEnabled;
            Color cpu = light ? Color.Teal : Color.Cyan, gpu = light ? Color.FromArgb(161, 85, 0) : Color.Gold;
            Func<string, Color, HistoryTrace> trace = (key, color) =>
            {
                var points = _history.Snapshot(key, end, minutes);
                if (tempsFahrenheit.Checked && (key == "CpuTemperature" || key == "GpuTemperature"))
                    for (int i = 0; i < points.Length; i++) { points[i].Value = points[i].Value * 9 / 5 + 32; points[i].Peak = points[i].Peak * 9 / 5 + 32; }
                return new HistoryTrace { Name = OsdMetricDefinitions.Names[Array.IndexOf(OsdMetricDefinitions.Keys, key)], Color = color, Points = points };
            };
            temperatures.SetData("Temperatures", tempsFahrenheit.Checked ? "°F" : "°C", end - minutes * 60, end, new[] { trace("CpuTemperature", cpu), trace("GpuTemperature", gpu) });
            usage.SetData("Usage", "%", end - minutes * 60, end, new[] { trace("CpuUsage", cpu), trace("GpuUsage", gpu) }, true);
            var fpsKeys = new[] { "Fps", "AverageFps", "FpsOnePercentLow" }.Where(key => OsdMetricDefinitions.IsVisible(config, key)).ToArray();
            fps.Visible = fpsKeys.Length > 0;
            fps.SetData("FPS", "FPS", end - minutes * 60, end, fpsKeys.Select(key => trace(key, key == "Fps" ? cpu : key == "AverageFps" ? gpu : light ? Color.Purple : Color.Violet)).ToArray());
            var memoryKeys = new[] { "RamUsage", "VramUsage" }.Where(key => OsdMetricDefinitions.IsVisible(config, key)).ToArray();
            memory.Visible = memoryKeys.Length > 0;
            memory.SetData("Memory used", "GB", end - minutes * 60, end, memoryKeys.Select(key => trace(key, key == "RamUsage" ? cpu : gpu)).ToArray());
            frames.Visible = config.ShowFrameTime || config.ShowFrameTimeGraph;
            frames.SetData("Frametime (average and peaks)", "ms", end - minutes * 60, end, new[] { trace("FrameTime", gpu) });
        }
    }
}
