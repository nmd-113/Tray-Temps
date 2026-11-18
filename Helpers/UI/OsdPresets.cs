using System;
using System.IO;
using System.Text;
using System.Text.Json;

namespace TrayTemps
{
    internal static class OsdPresets
    {
        private const int MaximumFileSize = 1024 * 1024;
        internal static readonly string[] Names = { "Classic stack", "Horizontal dashboard", "Minimal transparent", "Spectrum stack" };
        private static readonly Lazy<OsdConfiguration>[] Embedded =
        {
            new Lazy<OsdConfiguration>(() => ReadEmbedded(1)),
            new Lazy<OsdConfiguration>(() => ReadEmbedded(2)),
            new Lazy<OsdConfiguration>(() => ReadEmbedded(3)),
            new Lazy<OsdConfiguration>(() => ReadEmbedded(4))
        };

        internal static OsdConfiguration LoadEmbedded(int index) => Embedded[index].Value.Clone();

        private static OsdConfiguration ReadEmbedded(int number)
        {
            using (var stream = typeof(OsdPresets).Assembly.GetManifestResourceStream("TrayTemps.OsdPresets." + number + ".json"))
            using (var reader = new StreamReader(stream, Encoding.UTF8))
            {
                var preset = Parse(reader.ReadToEnd());
                OsdMetricDefinitions.Migrate(preset);
                return preset;
            }
        }

        internal static OsdConfiguration Parse(string json)
        {
            using (var document = JsonDocument.Parse(json))
            {
                var root = document.RootElement;
                if (root.ValueKind == JsonValueKind.Object && root.TryGetProperty("Osd", out var osd)) root = osd;
                if (root.ValueKind != JsonValueKind.Object || !root.TryGetProperty("ShowCpu", out _))
                    throw new JsonException("This file does not contain OSD settings.");
                // Imported settings use the same validation-before-migration order as app settings.
                return JsonSerializer.Deserialize<OsdConfiguration>(root.GetRawText());
            }
        }

        internal static OsdConfiguration LoadFile(string path)
        {
            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                if (stream.Length > MaximumFileSize) throw new IOException("The preset exceeds the 1 MB size limit.");
                using (var reader = new StreamReader(stream, Encoding.UTF8)) return Parse(reader.ReadToEnd());
            }
        }

        internal static void SaveFile(string path, OsdConfiguration configuration)
        {
            string temporary = path + "." + Guid.NewGuid().ToString("N") + ".tmp";
            try
            {
                string json = JsonSerializer.Serialize(configuration, new JsonSerializerOptions { WriteIndented = true });
                var encoding = new UTF8Encoding(false);
                if (encoding.GetByteCount(json) > MaximumFileSize) throw new IOException("The preset exceeds the 1 MB size limit.");
                File.WriteAllText(temporary, json, encoding);
                if (File.Exists(path)) File.Replace(temporary, path, null);
                else File.Move(temporary, path);
            }
            finally
            {
                if (File.Exists(temporary)) File.Delete(temporary);
            }
        }
    }
}
