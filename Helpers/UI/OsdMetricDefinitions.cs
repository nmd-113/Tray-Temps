using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace TrayTemps
{
    internal static class OsdMetricDefinitions
    {
        internal static readonly string[] Keys = { "CpuTemperature", "GpuTemperature", "CpuUsage", "GpuUsage", "RamUsage", "VramUsage", "Fps", "FpsOnePercentLow", "CurrentTime", "AverageFps", "FrameTime", "FrameTimeGraph" };
        internal static readonly string[] Names = { "CPU temperature", "GPU temperature", "CPU load", "GPU load", "RAM used", "VRAM used", "FPS", "1% Low", "Current time", "Average FPS", "Frametime", "Frametime graph" };

        internal static string Group(OsdConfiguration config, string key) =>
            key != "FrameTimeGraph" && config.MetricGroups != null && config.MetricGroups.TryGetValue(key, out string group) ? group : string.Empty;

        internal static bool IsFrameMetric(string key) => key == "Fps" || key == "FpsOnePercentLow" || key == "AverageFps" || key == "FrameTime" || key == "FrameTimeGraph";
        internal static bool NeedsFrames(OsdConfiguration config) => config.ShowFps || config.ShowFpsOnePercentLow || NeedsTiming(config);
        internal static bool NeedsTiming(OsdConfiguration config) => config.ShowAverageFps || config.ShowFrameTime || config.ShowFrameTimeGraph;

        // Built-in gap: one ordinary space; the FPS pair keeps its two base
        // spaces and the existing additional space on either side.
        internal static string Separator(OsdMetric preceding, OsdMetric following) => IsFpsPair(preceding, following) ? "    " : " ";

        private static bool IsFpsPair(OsdMetric preceding, OsdMetric following) =>
            preceding != null && following != null &&
            (preceding.Key == "Fps" && following.Key == "FpsOnePercentLow" ||
             preceding.Key == "FpsOnePercentLow" && following.Key == "Fps");

        internal static string SingleLine(string text, int limit, bool trim)
        {
            string value = (text ?? string.Empty).Replace("\r", "").Replace("\n", "").Replace("\t", " ");
            var clean = new StringBuilder(value.Length);
            for (int i = 0; i < value.Length; i++)
            {
                char character = value[i];
                if (char.IsHighSurrogate(character))
                {
                    if (i + 1 < value.Length && char.IsLowSurrogate(value[i + 1])) { clean.Append(character); clean.Append(value[++i]); }
                }
                else if (!char.IsLowSurrogate(character) && !char.IsControl(character)) clean.Append(character);
            }
            value = clean.ToString();
            if (trim) value = value.Trim().Normalize(NormalizationForm.FormC);
            if (value.Length > limit)
            {
                int length = limit;
                if (char.IsHighSurrogate(value[length - 1])) length--;
                value = value.Substring(0, length);
            }
            return value;
        }

        internal static OsdMetricStyle LegacyStyle(OsdConfiguration config, string key)
        {
            int color = key.StartsWith("Cpu", StringComparison.Ordinal) ? config.CpuFontColor :
                key.StartsWith("Gpu", StringComparison.Ordinal) ? config.GpuFontColor :
                key == "RamUsage" ? config.RamFontColor : key == "VramUsage" ? config.VramFontColor :
                key == "CurrentTime" ? Color.WhiteSmoke.ToArgb() : config.FpsFontColor;
            return new OsdMetricStyle { FontFamily = config.FontFamily, FontSize = config.FontSize, LabelColor = color, ValueColor = color };
        }

        internal static void Migrate(OsdConfiguration config)
        {
            config.GroupedValuesGapPercent = Math.Max(100, Math.Min(200, config.GroupedValuesGapPercent));
            bool legacy = config.MetricGroups == null;
            var groups = new Dictionary<string, string>(StringComparer.Ordinal);
            var hidden = new Dictionary<string, string>(StringComparer.Ordinal);
            if (config.GroupedLabelOverrides != null)
                foreach (var pair in config.GroupedLabelOverrides.Where(p => Keys.Contains(p.Key))) hidden[pair.Key] = NormalizeLabel(pair.Value, "");
            if (legacy)
            {
                var mode = config.MetricGroupingMode ?? (config.CombineTemperatureAndUsage ? OsdMetricGroupingMode.LoadAndTemps : OsdMetricGroupingMode.Disabled);
                foreach (string prefix in new[] { "Cpu", "Gpu" })
                {
                    bool temperature = prefix == "Cpu" ? config.ShowCpu : config.ShowGpu;
                    bool usage = prefix == "Cpu" ? config.ShowCpuUsage : config.ShowGpuUsage;
                    string memory = prefix == "Cpu" ? "RamUsage" : "VramUsage";
                    bool usedMemory = mode == OsdMetricGroupingMode.LoadTempsAndUsedMemory && IsVisible(config, memory);
                    if (mode == OsdMetricGroupingMode.Disabled || !temperature || (!usage && !usedMemory)) continue;
                    groups[prefix + "Temperature"] = prefix.ToUpperInvariant();
                    if (usage) { groups[prefix + "Usage"] = prefix.ToUpperInvariant(); hidden[prefix + "Usage"] = ""; }
                    if (usedMemory) { groups[memory] = prefix.ToUpperInvariant(); hidden[memory] = ""; }
                    if (config.LabelOverrides == null) config.LabelOverrides = CreateOverrides(config);
                    if (!config.LabelOverrides.ContainsKey(prefix + "Temperature")) config.LabelOverrides[prefix + "Temperature"] = prefix.ToUpperInvariant();
                }
                if (config.ShowFps && config.ShowFpsOnePercentLow) { groups["Fps"] = "Performance"; groups["FpsOnePercentLow"] = "Performance"; }
            }
            else foreach (var pair in config.MetricGroups.Where(p => Keys.Contains(p.Key))) groups[pair.Key] = SingleLine(pair.Value, 40, true);
            config.MetricGroups = groups; config.GroupedLabelOverrides = hidden;
            if (config.GroupLabels == null) config.GroupLabels = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            var styles = new Dictionary<string, OsdMetricStyle>(StringComparer.Ordinal);
            foreach (string key in Keys)
            {
                var style = config.MetricStyles != null && config.MetricStyles.TryGetValue(key, out var saved) && saved != null ? saved.Clone() : LegacyStyle(config, key);
                style.FontFamily = SingleLine(style.FontFamily, 128, true);
                if (string.IsNullOrWhiteSpace(style.FontFamily)) style.FontFamily = OsdFontHelper.DefaultFamily;
                style.FontSize = float.IsNaN(style.FontSize) || float.IsInfinity(style.FontSize) ? 16f : Math.Max(6, Math.Min(48, style.FontSize));
                styles[key] = style;
            }
            config.MetricStyles = styles;
            var gaps = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            if (config.MetricLabelValueGaps != null)
                foreach (var pair in config.MetricLabelValueGaps)
                    if (!string.IsNullOrWhiteSpace(pair.Key)) gaps[pair.Key] = Math.Max(0, Math.Min(100, pair.Value));
            if (config.FpsLabelValueSpacing.HasValue)
                foreach (string key in new[] { "Fps", "FpsOnePercentLow", "AverageFps" })
                    if (!gaps.ContainsKey(key)) gaps[key] = Math.Max(0, Math.Min(100, config.FpsLabelValueSpacing.Value));
            config.MetricLabelValueGaps = gaps;
        }

        internal static int LabelGap(OsdConfiguration config, string key, string group)
        {
            if (config.MetricLabelValueGaps != null)
            {
                string groupKey = string.IsNullOrEmpty(group) ? null : "Group:" + group;
                if (groupKey != null && config.MetricLabelValueGaps.TryGetValue(groupKey, out int groupGap)) return Math.Max(0, Math.Min(100, groupGap));
                if (!string.IsNullOrEmpty(key) && config.MetricLabelValueGaps.TryGetValue(key, out int metricGap)) return Math.Max(0, Math.Min(100, metricGap));
            }
            return config.LabelValueSpacing;
        }

        internal static List<OsdMetric> Compose(OsdConfiguration config, IEnumerable<OsdMetric> metrics)
        {
            var available = metrics.ToDictionary(metric => metric.Key, StringComparer.Ordinal);
            var result = new List<OsdMetric>();
            var grouped = new Dictionary<string, OsdMetric>(StringComparer.OrdinalIgnoreCase);
            var order = OsdItemOrderHelper.Parse(config.ItemOrder);
            foreach (var kind in order)
            {
                string key = kind.ToString();
                string group = Group(config, key);
                if (group.Length == 0) continue;
                if (!grouped.TryGetValue(group, out var row))
                {
                    row = new OsdMetric(group, "") { Key = "Group:" + group, Members = new List<OsdMetric>() };
                    grouped[group] = row;
                }
                if (available.TryGetValue(key, out var metric))
                {
                    if (row.Members.Count == 0) row.Style = metric.Style;
                    row.Members.Add(metric);
                }
            }
            // Group priority follows its first configured member, just like the order tree,
            // even when that member is hidden. Only groups with visible members are emitted.
            foreach (var kind in order)
            {
                string key = kind.ToString();
                string group = Group(config, key);
                if (group.Length == 0)
                {
                    if (available.TryGetValue(key, out var metric)) result.Add(metric);
                }
                else if (grouped.TryGetValue(group, out var row))
                {
                    if (row.Members.Count > 0) result.Add(row);
                    grouped.Remove(group);
                }
            }
            return result;
        }

        internal static bool IsVisible(OsdConfiguration config, string key)
        {
            switch (key)
            {
                case "CpuTemperature": return config.ShowCpu;
                case "GpuTemperature": return config.ShowGpu;
                case "CpuUsage": return config.ShowCpuUsage;
                case "GpuUsage": return config.ShowGpuUsage;
                case "RamUsage": return config.ShowRamUsage;
                case "VramUsage": return config.ShowVramUsage;
                case "Fps": return config.ShowFps;
                case "FpsOnePercentLow": return config.ShowFpsOnePercentLow;
                case "CurrentTime": return config.ShowCurrentTime;
                case "AverageFps": return config.ShowAverageFps;
                case "FrameTime": return config.ShowFrameTime;
                case "FrameTimeGraph": return config.ShowFrameTimeGraph;
                default: return false;
            }
        }

        internal static void SetVisible(OsdConfiguration config, string key, bool value)
        {
            switch (key)
            {
                case "CpuTemperature": config.ShowCpu = value; break;
                case "GpuTemperature": config.ShowGpu = value; break;
                case "CpuUsage": config.ShowCpuUsage = value; break;
                case "GpuUsage": config.ShowGpuUsage = value; break;
                case "RamUsage": config.ShowRamUsage = value; break;
                case "VramUsage": config.ShowVramUsage = value; break;
                case "Fps": config.ShowFps = value; break;
                case "FpsOnePercentLow": config.ShowFpsOnePercentLow = value; break;
                case "CurrentTime": config.ShowCurrentTime = value; break;
                case "AverageFps": config.ShowAverageFps = value; break;
                case "FrameTime": config.ShowFrameTime = value; break;
                case "FrameTimeGraph": config.ShowFrameTimeGraph = value; break;
            }
        }

        internal static string DefaultLabel(OsdConfiguration config, string key)
        {
            var grouping = config.MetricGroupingMode ?? (config.CombineTemperatureAndUsage ? OsdMetricGroupingMode.LoadAndTemps : OsdMetricGroupingMode.Disabled);
            switch (key)
            {
                case "CpuTemperature": return config.MetricGroups == null && grouping != OsdMetricGroupingMode.Disabled && config.ShowCpu &&
                    (config.ShowCpuUsage || (grouping == OsdMetricGroupingMode.LoadTempsAndUsedMemory && config.ShowRamUsage)) ? "CPU" : "CPU Temp";
                case "GpuTemperature": return config.MetricGroups == null && grouping != OsdMetricGroupingMode.Disabled && config.ShowGpu &&
                    (config.ShowGpuUsage || (grouping == OsdMetricGroupingMode.LoadTempsAndUsedMemory && config.ShowVramUsage)) ? "GPU" : "GPU Temp";
                case "CpuUsage": return "CPU Load";
                case "GpuUsage": return "GPU Load";
                case "RamUsage": return "RAM Use";
                case "VramUsage": return "VRAM Use";
                case "Fps": return "FPS";
                case "FpsOnePercentLow": return "1%Low";
                case "CurrentTime": return "Time";
                case "AverageFps": return "Avg FPS";
                case "FrameTime": return "Frametime";
                case "FrameTimeGraph": return "Frametime";
                default: return string.Empty;
            }
        }

        internal static string NormalizeLabel(string value, string fallback)
        {
            if (value == null) return fallback;
            string text = SingleLine(HardwareReportFormatHelper.SanitizeSingleLineText(value), int.MaxValue, false);
            return string.IsNullOrWhiteSpace(text) ? string.Empty : text;
        }

        internal static string ResolveLabel(OsdConfiguration config, string key, string fallback = null)
        {
            if (Group(config, key).Length > 0 && config.GroupedLabelOverrides != null && config.GroupedLabelOverrides.TryGetValue(key, out string grouped)) return grouped;
            fallback = fallback ?? DefaultLabel(config, key);
            if (config.LabelOverrides != null)
                return config.LabelOverrides.TryGetValue(key, out string label) ? NormalizeLabel(label, fallback) : fallback;
            if (config.LabelMode != OsdLabelMode.Custom) return fallback;
            switch (key)
            {
                case "CpuTemperature": return NormalizeLabel(config.CustomCpuLabel, fallback);
                case "GpuTemperature": return NormalizeLabel(config.CustomGpuLabel, fallback);
                case "CpuUsage": return NormalizeLabel(config.CustomCpuUsageLabel, fallback);
                case "GpuUsage": return NormalizeLabel(config.CustomGpuUsageLabel, fallback);
                case "RamUsage": return NormalizeLabel(config.CustomRamLabel, fallback);
                case "VramUsage": return NormalizeLabel(config.CustomVramLabel, fallback);
                case "Fps": return NormalizeLabel(config.CustomFpsLabel, fallback);
                case "CurrentTime": return NormalizeLabel(config.CustomTimeLabel, fallback);
                default: return fallback;
            }
        }

        internal static Dictionary<string, string> CreateOverrides(OsdConfiguration config)
        {
            var result = new Dictionary<string, string>(StringComparer.Ordinal);
            foreach (string key in Keys)
            {
                if (config.LabelOverrides != null)
                {
                    if (config.LabelOverrides.TryGetValue(key, out string label) && label != null)
                        result[key] = NormalizeLabel(label, string.Empty);
                }
                else if (config.LabelMode == OsdLabelMode.Custom && key != "FpsOnePercentLow")
                    result[key] = ResolveLabel(config, key);
            }
            return result;
        }
    }
}
