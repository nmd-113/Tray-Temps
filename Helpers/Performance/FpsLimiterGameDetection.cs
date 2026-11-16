using System;
using System.Collections.Generic;

namespace TrayTemps
{
    internal struct GameDetectionDecision
    {
        internal bool Eligible { get; }
        internal string Reason { get; }
        internal GameDetectionDecision(bool eligible, string reason) { Eligible = eligible; Reason = reason; }
    }

    internal static class FpsLimiterGameDetection
    {
        private static readonly string[] GameLocations = { @"\steamapps\common\", @"\epic games\", @"\xboxgames\", @"\gog games\" };
        internal static GameDetectionDecision Evaluate(string path, bool visibleWindow, IEnumerable<string> modules)
        {
            if (string.IsNullOrEmpty(path)) return new GameDetectionDecision(false, "Executable path unavailable");
            if (!visibleWindow) return new GameDetectionDecision(false, "No visible game window");
            foreach (string location in GameLocations)
                if (path.IndexOf(location, StringComparison.OrdinalIgnoreCase) >= 0)
                    return new GameDetectionDecision(true, "Recognized game location");

            bool gameSdk = false, graphics = false;
            if (modules != null)
                foreach (string name in modules)
                {
                    if (string.IsNullOrEmpty(name)) continue;
                    if (name.Equals("UnityPlayer.dll", StringComparison.OrdinalIgnoreCase) ||
                        name.Equals("GameAssembly.dll", StringComparison.OrdinalIgnoreCase) ||
                        name.StartsWith("Unreal", StringComparison.OrdinalIgnoreCase) ||
                        name.Equals("CrySystem.dll", StringComparison.OrdinalIgnoreCase))
                        return new GameDetectionDecision(true, "Recognized game engine");
                    gameSdk |= name.Equals("steam_api64.dll", StringComparison.OrdinalIgnoreCase) ||
                        name.Equals("steam_api.dll", StringComparison.OrdinalIgnoreCase) ||
                        name.Equals("EOSSDK-Win64-Shipping.dll", StringComparison.OrdinalIgnoreCase) ||
                        name.Equals("EOSSDK-Win32-Shipping.dll", StringComparison.OrdinalIgnoreCase);
                    graphics |= name.Equals("d3d9.dll", StringComparison.OrdinalIgnoreCase) ||
                        name.Equals("d3d10.dll", StringComparison.OrdinalIgnoreCase) ||
                        name.Equals("d3d10_1.dll", StringComparison.OrdinalIgnoreCase) ||
                        name.Equals("d3d11.dll", StringComparison.OrdinalIgnoreCase) ||
                        name.Equals("d3d12.dll", StringComparison.OrdinalIgnoreCase) ||
                        name.Equals("opengl32.dll", StringComparison.OrdinalIgnoreCase) ||
                        name.Equals("vulkan-1.dll", StringComparison.OrdinalIgnoreCase);
                }
            return gameSdk && graphics ? new GameDetectionDecision(true, "Game SDK and supported graphics runtime") :
                new GameDetectionDecision(false, gameSdk ? "Waiting for graphics runtime" : "Game evidence missing");
        }
    }
}
