using System;
using System.Diagnostics;
using System.IO;
using System.Text.Json;

namespace TrayTemps
{
    internal static class SettingsFileHelper
    {
        internal static bool HasSettingsFile(string settingsPath)
        {
            return File.Exists(settingsPath) ||
                   File.Exists(settingsPath + ".tmp") ||
                   File.Exists(settingsPath + ".bak");
        }

        internal static string ReadSettingsJson(string settingsPath)
        {
            string[] candidates = { settingsPath, settingsPath + ".tmp", settingsPath + ".bak" };

            foreach (string path in candidates)
            {
                if (!File.Exists(path))
                    continue;

                try
                {
                    string json = File.ReadAllText(path);
                    AppSettings settings = JsonSerializer.Deserialize<AppSettings>(json);
                    if (settings == null)
                        throw new InvalidDataException("Settings content was empty.");
                    return json;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Ignoring invalid settings candidate '{path}': {ex.Message}");
                }
            }

            throw new InvalidDataException("No valid settings file was found.");
        }

        internal static void WriteSettings(string settingsPath, AppSettings settings)
        {
            string directory = Path.GetDirectoryName(settingsPath);
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(settings, options);
            string tempPath = settingsPath + ".tmp";

            using (var fs = new FileStream(tempPath, FileMode.Create, FileAccess.Write, FileShare.None))
            using (var sw = new StreamWriter(fs))
            {
                sw.Write(json);
                sw.Flush();
                fs.Flush(true);
            }

            ReplaceSettingsFile(settingsPath, tempPath);
        }

        private static void ReplaceSettingsFile(string settingsPath, string tempPath)
        {
            string backupPath = settingsPath + ".bak";

            try
            {
                if (File.Exists(settingsPath))
                    File.Replace(tempPath, settingsPath, backupPath);
                else
                    File.Move(tempPath, settingsPath);
            }
            catch (PlatformNotSupportedException)
            {
                File.Copy(tempPath, settingsPath, true);
                File.Delete(tempPath);
            }
            catch (IOException) when (File.Exists(tempPath))
            {
                File.Copy(tempPath, settingsPath, true);
                File.Delete(tempPath);
            }
        }
    }
}
