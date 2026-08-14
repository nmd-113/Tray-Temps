using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.Json.Serialization;
using System.Windows.Forms;

namespace TrayTemps
{
    public enum OsdPosition
    {
        TopLeft,
        TopCenter,
        TopRight,
        CenterLeft,
        Center,
        CenterRight,
        BottomLeft,
        BottomCenter,
        BottomRight
    }

    public enum OsdLabelMode
    {
        Short,
        // Value 2 preserves compatibility with existing settings files.
        Custom = 2
    }

    internal enum OsdItemKind
    {
        CpuTemperature,
        GpuTemperature,
        CpuUsage,
        GpuUsage,
        RamUsage,
        VramUsage,
        Fps
    }

    [Flags]
    internal enum OsdHotkeyModifiers
    {
        None = 0,
        Alt = 1,
        Control = 2,
        Shift = 4
    }

    public sealed class OsdConfiguration
    {
        private int _labelValueSpacing = 14;
        private bool _hasExplicitLabelValueSpacing;

        public bool Enabled { get; set; }
        public OsdPosition Position { get; set; } = OsdPosition.TopRight;
        public bool ShowCpu { get; set; } = true;
        public bool ShowGpu { get; set; } = true;
        public string FontFamily { get; set; } = OsdFontHelper.DefaultFamily;
        public float FontSize { get; set; } = 16f;
        public int CpuFontColor { get; set; } = Color.Aqua.ToArgb();
        public int GpuFontColor { get; set; } = Color.Gold.ToArgb();
        public int RamFontColor { get; set; } = Color.LightGreen.ToArgb();
        public int VramFontColor { get; set; } = Color.Violet.ToArgb();
        public int FpsFontColor { get; set; } = Color.WhiteSmoke.ToArgb();
        public int? BackgroundColor { get; set; } = Color.FromArgb(24, 24, 24).ToArgb();
        public int BackgroundOpacityPercent { get; set; } = 100;
        // Kept for compatibility with settings written before background opacity was adjustable.
        public bool TransparentBackground { get; set; }
        public int OpacityPercent { get; set; } = 90;
        public bool ShowCpuUsage { get; set; }
        public bool ShowGpuUsage { get; set; }
        public bool ShowRamUsage { get; set; }
        public bool ShowVramUsage { get; set; }
        public bool ShowFps { get; set; }
        public bool CombineTemperatureAndUsage { get; set; }
        public OsdLabelMode LabelMode { get; set; } = OsdLabelMode.Short;
        public string CustomCpuLabel { get; set; } = "CPU Temp";
        public string CustomGpuLabel { get; set; } = "GPU Temp";
        public string CustomCpuUsageLabel { get; set; } = "CPU Load";
        public string CustomGpuUsageLabel { get; set; } = "GPU Load";
        public string CustomRamLabel { get; set; } = "RAM Use";
        public string CustomVramLabel { get; set; } = "VRAM Use";
        public string CustomFpsLabel { get; set; } = "FPS";
        public int LabelValueSpacing
        {
            get => _labelValueSpacing;
            set
            {
                _labelValueSpacing = value;
                _hasExplicitLabelValueSpacing = true;
            }
        }
        // Nullable legacy properties allow existing settings to migrate to the universal value.
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? CpuTemperatureSpacing { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? GpuTemperatureSpacing { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? CpuUsageSpacing { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? GpuUsageSpacing { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? RamUsageSpacing { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? VramUsageSpacing { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? FpsSpacing { get; set; }
        public int ScreenMargin { get; set; } = 12;
        public int Columns { get; set; } = 1;
        public string ItemOrder { get; set; } =
            "CpuTemperature,GpuTemperature,CpuUsage,GpuUsage,RamUsage,VramUsage,Fps";
        public bool HotkeyEnabled { get; set; }
        public int HotkeyModifiers { get; set; } =
            (int)(OsdHotkeyModifiers.Control | OsdHotkeyModifiers.Shift);
        public int HotkeyKey { get; set; } = (int)Keys.O;

        internal OsdConfiguration Clone()
        {
            return (OsdConfiguration)MemberwiseClone();
        }

        [JsonIgnore]
        internal bool HasExplicitLabelValueSpacing => _hasExplicitLabelValueSpacing;
    }

    internal static class OsdColorHelper
    {
        internal static Color GetDefaultBackground(bool lightMode)
        {
            return lightMode ? Color.FromArgb(242, 246, 252) : Color.FromArgb(24, 24, 24);
        }

        internal static Color GetOpaqueColor(int argb)
        {
            Color color = Color.FromArgb(argb);
            return Color.FromArgb(color.R, color.G, color.B);
        }
    }

    internal static class OsdFontHelper
    {
        internal const string DefaultFamily = "Segoe UI";
        internal const string EmbeddedBunkenDisplayName = "Bunken Tech Sans Pro Bold";

        internal static string[] GetAvailableFamilyNames()
        {
            FontFamily[] families = FontFamily.Families;
            try
            {
                return families
                    .Select(family => family.Name)
                    .Concat(new[] { EmbeddedBunkenDisplayName, "Consolas" })
                    .Where(name => !string.IsNullOrWhiteSpace(name))
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .OrderBy(name => name, StringComparer.OrdinalIgnoreCase)
                    .ToArray();
            }
            finally
            {
                foreach (FontFamily family in families)
                    family.Dispose();
            }
        }

        internal static Font CreateFont(string familyName, float size)
        {
            float safeSize = Math.Max(8f, Math.Min(48f, size));

            try
            {
                if (IsEmbeddedBunkenFamily(familyName) && EmbeddedFonts.Bold != null)
                {
                    FontStyle style = EmbeddedFonts.Bold.IsStyleAvailable(FontStyle.Bold)
                        ? FontStyle.Bold
                        : FontStyle.Regular;
                    return new Font(EmbeddedFonts.Bold, safeSize, style, GraphicsUnit.Point);
                }

                return new Font(
                    string.IsNullOrWhiteSpace(familyName) ? DefaultFamily : familyName,
                    safeSize,
                    FontStyle.Bold,
                    GraphicsUnit.Point);
            }
            catch
            {
                return new Font(DefaultFamily, safeSize, FontStyle.Bold, GraphicsUnit.Point);
            }
        }

        private static bool IsEmbeddedBunkenFamily(string familyName)
        {
            return string.Equals(familyName, EmbeddedBunkenDisplayName, StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(familyName, EmbeddedFonts.Bold?.Name, StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(familyName, EmbeddedFonts.Book?.Name, StringComparison.OrdinalIgnoreCase);
        }
    }

    internal sealed class OsdMetric
    {
        internal OsdMetric(string label, string value, Color? textColor = null)
        {
            Label = label ?? string.Empty;
            Value = value ?? "N/A";
            TextColor = textColor;
        }

        internal OsdMetric(string label, string value, string trailingValue, Color? textColor)
            : this(label, value, textColor)
        {
            TrailingValue = trailingValue;
        }

        internal string Label { get; }
        internal string Value { get; }
        internal string TrailingValue { get; }
        internal string ValueWidthTemplate { get; set; }
        internal Color? TextColor { get; }
    }

    internal static class OsdHotkeyHelper
    {
        internal const OsdHotkeyModifiers DefaultModifiers =
            OsdHotkeyModifiers.Control | OsdHotkeyModifiers.Shift;
        internal const Keys DefaultKey = Keys.O;
        internal const OsdHotkeyModifiers AllowedModifiers =
            OsdHotkeyModifiers.Alt | OsdHotkeyModifiers.Control | OsdHotkeyModifiers.Shift;

        internal static bool IsValid(OsdHotkeyModifiers modifiers, Keys key)
        {
            Keys keyCode = key & Keys.KeyCode;
            return (modifiers & AllowedModifiers) != OsdHotkeyModifiers.None &&
                   (modifiers & ~AllowedModifiers) == OsdHotkeyModifiers.None &&
                   keyCode != Keys.None &&
                   keyCode != Keys.KeyCode &&
                   keyCode != Keys.ControlKey &&
                   keyCode != Keys.ShiftKey &&
                   keyCode != Keys.Menu &&
                   keyCode != Keys.LWin &&
                   keyCode != Keys.RWin &&
                   keyCode != Keys.LButton &&
                   keyCode != Keys.RButton &&
                   keyCode != Keys.MButton &&
                   keyCode != Keys.XButton1 &&
                   keyCode != Keys.XButton2 &&
                   Enum.IsDefined(typeof(Keys), keyCode);
        }

        internal static string Format(OsdHotkeyModifiers modifiers, Keys key)
        {
            var parts = new List<string>();

            if ((modifiers & OsdHotkeyModifiers.Control) != 0)
                parts.Add("Ctrl");
            if ((modifiers & OsdHotkeyModifiers.Shift) != 0)
                parts.Add("Shift");
            if ((modifiers & OsdHotkeyModifiers.Alt) != 0)
                parts.Add("Alt");

            Keys keyCode = key & Keys.KeyCode;
            if (keyCode != Keys.None)
                parts.Add(GetKeyDisplayName(keyCode));

            return string.Join(" + ", parts);
        }

        private static string GetKeyDisplayName(Keys key)
        {
            if (key >= Keys.D0 && key <= Keys.D9)
                return ((int)(key - Keys.D0)).ToString();

            if (key >= Keys.NumPad0 && key <= Keys.NumPad9)
                return "Num " + ((int)(key - Keys.NumPad0));

            return key.ToString();
        }
    }

    internal static class OsdItemOrderHelper
    {
        private static readonly OsdItemKind[] DefaultOrder =
        {
            OsdItemKind.CpuTemperature,
            OsdItemKind.GpuTemperature,
            OsdItemKind.CpuUsage,
            OsdItemKind.GpuUsage,
            OsdItemKind.RamUsage,
            OsdItemKind.VramUsage,
            OsdItemKind.Fps
        };

        internal static List<OsdItemKind> Parse(string value)
        {
            var result = new List<OsdItemKind>();

            if (!string.IsNullOrWhiteSpace(value))
            {
                foreach (string part in value.Split(','))
                {
                    if (Enum.TryParse(part.Trim(), true, out OsdItemKind item) && !result.Contains(item))
                        result.Add(item);
                }
            }

            foreach (OsdItemKind item in DefaultOrder)
            {
                if (!result.Contains(item))
                    result.Add(item);
            }

            return result;
        }

        internal static string Serialize(IEnumerable<OsdItemKind> items)
        {
            return string.Join(",", items ?? DefaultOrder);
        }

        internal static string GetDisplayName(OsdItemKind item)
        {
            switch (item)
            {
                case OsdItemKind.CpuTemperature: return "CPU temperature";
                case OsdItemKind.GpuTemperature: return "GPU temperature";
                case OsdItemKind.CpuUsage: return "CPU usage";
                case OsdItemKind.GpuUsage: return "GPU usage";
                case OsdItemKind.RamUsage: return "RAM usage";
                case OsdItemKind.VramUsage: return "VRAM usage";
                case OsdItemKind.Fps: return "FPS";
                default: return item.ToString();
            }
        }
    }
}
