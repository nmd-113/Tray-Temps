using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using FpsLimiter.Core;

namespace TrayTemps
{
    public sealed class FpsLimiterConfiguration
    {
        internal const OsdHotkeyModifiers DefaultHotkeyModifiers = OsdHotkeyModifiers.Control | OsdHotkeyModifiers.Shift;
        internal const Keys DefaultHotkeyKey = Keys.F11;
        public bool Enabled { get; set; }
        public int FramesPerSecond { get; set; } = 60;
        public string ExcludedProcesses { get; set; } = "";
        public bool ToggleHotkeyEnabled { get; set; }
        public int ToggleHotkeyModifiers { get; set; } = (int)DefaultHotkeyModifiers;
        public int ToggleHotkeyKey { get; set; } = (int)DefaultHotkeyKey;
        public List<GameLimit> GameLimits { get; set; } = new List<GameLimit>();

        internal FpsLimiterConfiguration Clone()
        {
            return new FpsLimiterConfiguration
            {
                Enabled = Enabled, FramesPerSecond = FramesPerSecond,
                ExcludedProcesses = ExcludedProcesses, ToggleHotkeyEnabled = ToggleHotkeyEnabled,
                ToggleHotkeyModifiers = ToggleHotkeyModifiers, ToggleHotkeyKey = ToggleHotkeyKey,
                GameLimits = (GameLimits ?? new List<GameLimit>()).Select(game => game == null ? null :
                    new GameLimit { ExecutablePath = game.ExecutablePath, FramesPerSecond = game.FramesPerSecond }).ToList()
            };
        }

        internal void Validate()
        {
            if (!IsValidHotkey((OsdHotkeyModifiers)ToggleHotkeyModifiers, (Keys)ToggleHotkeyKey))
                throw new ArgumentException("Choose Ctrl, Shift, or Alt plus one key for the FPS limiter shortcut. F12 is reserved by Windows.");
            if (GameLimits == null) GameLimits = new List<GameLimit>();
            new LimiterSettings { FramesPerSecond = FramesPerSecond, ExcludedProcesses = ExcludedProcesses, GameLimits = GameLimits }.Validate();
            if (GameLimits.GroupBy(game => game.ExecutablePath, StringComparer.OrdinalIgnoreCase).Any(group => group.Count() > 1))
                throw new InvalidDataException("A game executable is listed more than once.");
        }

        internal static FpsLimiterConfiguration Normalize(FpsLimiterConfiguration configuration)
        {
            if (configuration == null) return new FpsLimiterConfiguration();
            var result = configuration.Clone();
            if (!IsValidHotkey((OsdHotkeyModifiers)result.ToggleHotkeyModifiers, (Keys)result.ToggleHotkeyKey))
            {
                result.ToggleHotkeyModifiers = (int)DefaultHotkeyModifiers;
                result.ToggleHotkeyKey = (int)DefaultHotkeyKey;
            }
            try { result.Validate(); return result; }
            catch (InvalidDataException) { return new FpsLimiterConfiguration(); }
            catch (ArgumentException) { return new FpsLimiterConfiguration(); }
        }

        internal GameLimit FindGame(string path) => GameLimits.Find(game => string.Equals(game.ExecutablePath, path, StringComparison.OrdinalIgnoreCase));
        internal static bool IsValidHotkey(OsdHotkeyModifiers modifiers, Keys key) =>
            OsdHotkeyHelper.IsValid(modifiers, key) && (key & Keys.KeyCode) != Keys.F12;
        internal int GetCap(string path) => FindGame(path)?.FramesPerSecond ?? FramesPerSecond;
    }
}
