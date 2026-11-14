using System;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;

namespace TrayTemps
{
    internal static class FpsLimiterPayloads
    {
        internal const string EngineVersion = "1.0.0-0B84C93A0E2A";
        internal static string ExtractNative()
        {
            string root = Path.Combine(GetCacheDirectory(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)), EngineVersion, "native");
            Extract("TrayFPS.Payload.x64.dll", Path.Combine(root, "x64", "FpsLimiter.Native.dll"));
            Extract("TrayFPS.Payload.x86.dll", Path.Combine(root, "x86", "FpsLimiter.Native.dll"));
            return root;
        }
        private static string GetCacheDirectory(string localApplicationData)
        {
            if (string.IsNullOrWhiteSpace(localApplicationData) || !Path.IsPathRooted(localApplicationData) ||
                Path.GetPathRoot(localApplicationData).Length < 3)
                throw new InvalidOperationException("The local application data directory could not be resolved.");
            return Path.Combine(Path.GetFullPath(localApplicationData), "TrayTemps", "FpsLimiter");
        }
        internal static void DeleteCache(string localApplicationData)
        {
            string cache = GetCacheDirectory(localApplicationData);
            if (Directory.Exists(cache)) Directory.Delete(cache, true);
        }
        private static void Extract(string resource, string path)
        {
            using (Stream input = Assembly.GetExecutingAssembly().GetManifestResourceStream(resource))
            {
                if (input == null) throw new InvalidDataException("Embedded native payload is missing: " + resource);
                Directory.CreateDirectory(Path.GetDirectoryName(path));
                if (File.Exists(path))
                {
                    using (var existing = File.OpenRead(path))
                        if (Hash(existing) == Hash(input)) return;
                    // Manifest resource streams are seekable; rewind before repairing a mismatched file.
                    input.Position = 0;
                }
                string temp = path + ".tmp." + Guid.NewGuid().ToString("N");
                try
                {
                    using (var output = new FileStream(temp, FileMode.Create, FileAccess.Write, FileShare.Read))
                        input.CopyTo(output);
                    if (File.Exists(path)) File.Replace(temp, path, null); else File.Move(temp, path);
                }
                finally
                {
                    // A sharing violation must not accumulate full DLL copies.
                    try { if (File.Exists(temp)) File.Delete(temp); }
                    catch (IOException error) { System.Diagnostics.Debug.WriteLine(error); }
                    catch (UnauthorizedAccessException error) { System.Diagnostics.Debug.WriteLine(error); }
                }
            }
        }
        private static string Hash(Stream input) { using (var sha = SHA256.Create()) return Convert.ToBase64String(sha.ComputeHash(input)); }
    }
}
