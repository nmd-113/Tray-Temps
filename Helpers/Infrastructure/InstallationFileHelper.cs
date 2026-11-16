using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace TrayTemps
{
    internal static class InstallationFileHelper
    {
        internal static bool TryGetInstallDirectory(string path, out string directory)
        {
            directory = null;
            try
            {
                if (string.IsNullOrWhiteSpace(path)) return false;
                string full = Path.GetFullPath(path).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
                string executable = Path.Combine(full, "TrayTemps.exe");
                if (!string.Equals(Path.GetFileName(full), "TrayTemps", StringComparison.OrdinalIgnoreCase) ||
                    !Directory.Exists(full) || !File.Exists(executable) ||
                    (File.GetAttributes(full) & FileAttributes.ReparsePoint) != 0 ||
                    (File.GetAttributes(executable) & FileAttributes.ReparsePoint) != 0 ||
                    AssemblyName.GetAssemblyName(executable).Name != "TrayTemps") return false;
                var version = FileVersionInfo.GetVersionInfo(executable);
                if (version.CompanyName != "NaeTech" || version.ProductName != "Tray Temps") return false;
                directory = full;
                return true;
            }
            catch (Exception error) when (error is IOException || error is UnauthorizedAccessException ||
                error is ArgumentException || error is BadImageFormatException || error is NotSupportedException)
            { return false; }
        }

        internal static void CheckWriteAccess(string directory)
        {
            string probe = Path.Combine(directory, ".TrayTemps-write-" + Guid.NewGuid().ToString("N") + ".tmp");
            bool created = false;
            try
            {
                using (var file = new FileStream(probe, FileMode.CreateNew, FileAccess.Write, FileShare.None))
                {
                    created = true;
                    file.WriteByte(0);
                }
            }
            finally { if (created) File.Delete(probe); }
        }

        internal static void RemoveInstalledFiles(string directory)
        {
            if (!TryGetInstallDirectory(directory, out string verified))
                throw new IOException("The TrayTemps installation could not be verified.");
            File.Delete(Path.Combine(verified, "TrayTemps.exe"));
            RemoveEmptyDirectory(verified);
        }

        internal static void RemoveSettingsFiles(string directory)
        {
            if (!Directory.Exists(directory)) return;
            if ((File.GetAttributes(directory) & FileAttributes.ReparsePoint) != 0)
                throw new IOException("The settings directory is a link and cannot be removed safely.");
            foreach (string name in new[] { "settings.json", "settings.json.tmp", "settings.json.bak" })
                File.Delete(Path.Combine(directory, name));
            RemoveEmptyDirectory(directory);
        }

        private static void RemoveEmptyDirectory(string directory)
        {
            if (Directory.GetFileSystemEntries(directory).Length == 0)
            {
                // Nonrecursive deletion also protects against a file arriving after the empty check.
                try { Directory.Delete(directory, false); }
                catch (IOException) when (Directory.Exists(directory)) { }
            }
        }

        private static string Literal(string value) => "'" + value.Replace("'", "''") + "'";

        internal static string BuildDeferredUninstallScript(string directory, string settingsDirectory, string shortcut)
        {
            if (!TryGetInstallDirectory(directory, out string verified))
                throw new IOException("The TrayTemps installation could not be verified.");
            // All paths are PowerShell literals; no cmd expansion or recursive folder removal.
            return @"$ErrorActionPreference = 'Stop'
$installFolder = " + Literal(verified) + @"
$settingsFolder = " + Literal(settingsDirectory) + @"
$shortcut = " + Literal(shortcut) + @"
function Remove-EmptyDirectory([string]$path) {
    if ([IO.Directory]::Exists($path) -and [IO.Directory]::GetFileSystemEntries($path).Length -eq 0) {
        try { [IO.Directory]::Delete($path, $false) } catch [IO.IOException] { }
    }
}
try {
    $executable = [IO.Path]::Combine($installFolder, 'TrayTemps.exe')
    if (([IO.File]::GetAttributes($installFolder) -band [IO.FileAttributes]::ReparsePoint) -ne 0) { throw 'Installation folder is a link.' }
    for ($attempt = 0; $attempt -lt 15 -and [IO.File]::Exists($executable); $attempt++) {
        if (([IO.File]::GetAttributes($executable) -band [IO.FileAttributes]::ReparsePoint) -ne 0) { throw 'Executable is a link.' }
        $identity = [Reflection.AssemblyName]::GetAssemblyName($executable)
        $version = [Diagnostics.FileVersionInfo]::GetVersionInfo($executable)
        if ($identity.Name -ne 'TrayTemps' -or $version.CompanyName -ne 'NaeTech' -or $version.ProductName -ne 'Tray Temps') { throw 'Installation identity changed.' }
        try { [IO.File]::Delete($executable) } catch [IO.IOException] { Start-Sleep -Milliseconds 1000 }
    }
    if ([IO.File]::Exists($executable)) { throw 'TrayTemps is still in use. Retry uninstall after it exits.' }
    Remove-EmptyDirectory $installFolder
    if ([IO.Directory]::Exists($settingsFolder)) {
        if (([IO.File]::GetAttributes($settingsFolder) -band [IO.FileAttributes]::ReparsePoint) -ne 0) { throw 'Settings folder is a link.' }
        foreach ($name in @('settings.json', 'settings.json.tmp', 'settings.json.bak')) {
            [IO.File]::Delete([IO.Path]::Combine($settingsFolder, $name))
        }
        Remove-EmptyDirectory $settingsFolder
    }
    [IO.File]::Delete($shortcut)
} finally { [IO.File]::Delete($PSCommandPath) }
";
        }
    }
}
