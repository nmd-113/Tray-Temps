using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FpsLimiter.Core;

namespace TrayTemps
{
    public sealed class FpsLimiterConfiguration
    {
        public bool Enabled { get; set; }
        public int FramesPerSecond { get; set; } = 60;
        public string ExcludedProcesses { get; set; } = "";
        public bool ToggleHotkeyEnabled { get; set; }
        public List<GameLimit> GameLimits { get; set; } = new List<GameLimit>();

        internal FpsLimiterConfiguration Clone()
        {
            return new FpsLimiterConfiguration
            {
                Enabled = Enabled, FramesPerSecond = FramesPerSecond,
                ExcludedProcesses = ExcludedProcesses, ToggleHotkeyEnabled = ToggleHotkeyEnabled,
                GameLimits = (GameLimits ?? new List<GameLimit>()).Select(game => game == null ? null :
                    new GameLimit { ExecutablePath = game.ExecutablePath, FramesPerSecond = game.FramesPerSecond }).ToList()
            };
        }

        internal void Validate()
        {
            if (GameLimits == null) GameLimits = new List<GameLimit>();
            new LimiterSettings { FramesPerSecond = FramesPerSecond, ExcludedProcesses = ExcludedProcesses, GameLimits = GameLimits }.Validate();
            if (GameLimits.GroupBy(game => game.ExecutablePath, StringComparer.OrdinalIgnoreCase).Any(group => group.Count() > 1))
                throw new InvalidDataException("A game executable is listed more than once.");
        }

        internal static FpsLimiterConfiguration Normalize(FpsLimiterConfiguration configuration)
        {
            if (configuration == null) return new FpsLimiterConfiguration();
            var result = configuration.Clone();
            try { result.Validate(); return result; }
            catch (InvalidDataException) { return new FpsLimiterConfiguration(); }
            catch (ArgumentException) { return new FpsLimiterConfiguration(); }
        }

        internal GameLimit FindGame(string path) => GameLimits.Find(game => string.Equals(game.ExecutablePath, path, StringComparison.OrdinalIgnoreCase));
        internal int GetCap(string path) => FindGame(path)?.FramesPerSecond ?? FramesPerSecond;
    }
}
