using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace FpsLimiter.Core
{
    [DataContract]
    public sealed class GameLimit
    {
        [DataMember] public string ExecutablePath { get; set; } = "";
        [DataMember] public int FramesPerSecond { get; set; } = 60;
        public void Validate()
        {
            if (string.IsNullOrWhiteSpace(ExecutablePath) || ExecutablePath.Length > 32767 || !Path.IsPathRooted(ExecutablePath)) throw new InvalidDataException("Invalid game executable path.");
            if (FramesPerSecond < 10 || FramesPerSecond > 1000) throw new InvalidDataException("Game FPS must be between 10 and 1000.");
        }
    }

    [DataContract]
    public sealed class LimiterSettings
    {
        [DataMember] public int FramesPerSecond { get; set; } = 60;
        [DataMember] public bool LimiterEnabled { get; set; }
        [DataMember] public bool StartMinimized { get; set; }
        [DataMember] public string ExcludedProcesses { get; set; } = "";
        [DataMember] public bool ToggleHotkeyEnabled { get; set; }
        [DataMember] public List<GameLimit> GameLimits { get; set; } = new List<GameLimit>();
        [OnDeserializing]
        private void SetDefaults(StreamingContext context)
        {
            FramesPerSecond = 60;
            LimiterEnabled = false;
            ExcludedProcesses = "";
            GameLimits = new List<GameLimit>();
        }
        public void Validate()
        {
            if (FramesPerSecond < 10 || FramesPerSecond > 1000) throw new InvalidDataException("FPS must be between 10 and 1000.");
            if (ExcludedProcesses == null || ExcludedProcesses.Length > 8192) throw new InvalidDataException("Invalid process exclusions.");
            if (GameLimits == null) GameLimits = new List<GameLimit>();
            if (GameLimits.Count > 512) throw new InvalidDataException("Too many configured games.");
            foreach (var game in GameLimits) { if (game == null) throw new InvalidDataException("Invalid configured game."); game.Validate(); }
        }
        public GameLimit FindGameLimit(string executablePath)
        {
            if (string.IsNullOrWhiteSpace(executablePath) || GameLimits == null) return null;
            return GameLimits.Find(game => string.Equals(game.ExecutablePath, executablePath, StringComparison.OrdinalIgnoreCase));
        }
        public int GetFramesPerSecondForExecutable(string executablePath)
        {
            var game = FindGameLimit(executablePath);
            return game == null ? FramesPerSecond : game.FramesPerSecond;
        }
        public static LimiterSettings Load(string path)
        {
            try { using (var s = File.OpenRead(path)) { var v = (LimiterSettings)new DataContractJsonSerializer(typeof(LimiterSettings)).ReadObject(s); if (v == null) return new LimiterSettings(); v.Validate(); return v; } }
            catch (SerializationException) { return new LimiterSettings(); }
            catch (InvalidDataException) { return new LimiterSettings(); }
            catch (IOException) { return new LimiterSettings(); }
            catch (UnauthorizedAccessException) { return new LimiterSettings(); }
        }
        public void SaveAtomic(string path)
        {
            Validate(); Directory.CreateDirectory(Path.GetDirectoryName(path)); string temp = path + ".tmp";
            using (var s = new FileStream(temp, FileMode.Create, FileAccess.Write, FileShare.None)) { new DataContractJsonSerializer(typeof(LimiterSettings)).WriteObject(s, this); s.Flush(true); }
            if (File.Exists(path)) File.Replace(temp, path, null); else File.Move(temp, path);
        }
    }
}
