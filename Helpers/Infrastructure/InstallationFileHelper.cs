using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

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

        internal static void ValidateSettingsDirectory(string directory)
        {
            if (!Directory.Exists(directory)) return;
            if ((File.GetAttributes(directory) & FileAttributes.ReparsePoint) != 0)
                throw new IOException("The settings directory is a link and cannot be removed safely.");
            string runtime = Path.Combine(directory, "runtime");
            if (Directory.Exists(runtime) && (File.GetAttributes(runtime) & FileAttributes.ReparsePoint) != 0)
                throw new IOException("The runtime configuration directory is a link and cannot be removed safely.");
        }

        internal static void RemoveSettingsFiles(string directory)
        {
            ValidateSettingsDirectory(directory);
            if (!Directory.Exists(directory)) return;
            string runtime = Path.Combine(directory, "runtime");
            foreach (string name in new[] { "settings.json", "settings.json.tmp", "settings.json.bak" })
                File.Delete(Path.Combine(directory, name));
            if (Directory.Exists(runtime))
            {
                File.Delete(Path.Combine(runtime, "TrayTemps.exe.config"));
                foreach (string temporary in Directory.GetFiles(runtime, "TrayTemps.exe.config.*.tmp"))
                {
                    var match = Regex.Match(Path.GetFileName(temporary), @"^TrayTemps\.exe\.config\.([0-9]+)\.tmp$");
                    if (match.Success && int.TryParse(match.Groups[1].Value, NumberStyles.None, CultureInfo.InvariantCulture, out int processId) && processId > 0)
                        File.Delete(temporary);
                }
                RemoveEmptyDirectory(runtime);
            }
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

        internal static string BuildDeferredUninstallScript(string directory, string settingsDirectory, string shortcut, bool showErrors = false)
        {
            if (!TryGetInstallDirectory(directory, out string verified))
                throw new IOException("The TrayTemps installation could not be verified.");
            // All paths are PowerShell literals; no cmd expansion or recursive folder removal.
            return @"$ErrorActionPreference = 'Stop'
$installFolder = " + Literal(verified) + @"
$settingsFolder = " + Literal(settingsDirectory) + @"
$shortcut = " + Literal(shortcut) + @"
$showErrors = " + (showErrors ? "$true" : "$false") + @"
function Remove-EmptyDirectory([string]$path) {
    if ([IO.Directory]::Exists($path) -and [IO.Directory]::GetFileSystemEntries($path).Length -eq 0) {
        try { [IO.Directory]::Delete($path, $false) } catch [IO.IOException] { }
    }
}
function Assert-SettingsDirectory([string]$path) {
    if ([IO.Directory]::Exists($path)) {
        if (([IO.File]::GetAttributes($path) -band [IO.FileAttributes]::ReparsePoint) -ne 0) { throw 'Settings folder is a link.' }
        $runtime = [IO.Path]::Combine($path, 'runtime')
        if ([IO.Directory]::Exists($runtime) -and ([IO.File]::GetAttributes($runtime) -band [IO.FileAttributes]::ReparsePoint) -ne 0) { throw 'Runtime configuration folder is a link.' }
    }
}
try {
    Assert-SettingsDirectory $settingsFolder
    $executable = [IO.Path]::Combine($installFolder, 'TrayTemps.exe')
    if (([IO.File]::GetAttributes($installFolder) -band [IO.FileAttributes]::ReparsePoint) -ne 0) { throw 'Installation folder is a link.' }
    for ($attempt = 0; $attempt -lt 15 -and [IO.File]::Exists($executable); $attempt++) {
        if (([IO.File]::GetAttributes($executable) -band [IO.FileAttributes]::ReparsePoint) -ne 0) { throw 'Executable is a link.' }
        $identity = [Reflection.AssemblyName]::GetAssemblyName($executable)
        $version = [Diagnostics.FileVersionInfo]::GetVersionInfo($executable)
        if ($identity.Name -ne 'TrayTemps' -or $version.CompanyName -ne 'NaeTech' -or $version.ProductName -ne 'Tray Temps') { throw 'Installation identity changed.' }
        try { [IO.File]::Delete($executable) }
        catch [IO.IOException] { Start-Sleep -Milliseconds 1000 }
        catch [UnauthorizedAccessException] { Start-Sleep -Milliseconds 1000 }
    }
    if ([IO.File]::Exists($executable)) { throw 'TrayTemps is still in use or access was denied. Close other instances and retry uninstall.' }
    Remove-EmptyDirectory $installFolder
    if ([IO.Directory]::Exists($settingsFolder)) {
        Assert-SettingsDirectory $settingsFolder
        $runtime = [IO.Path]::Combine($settingsFolder, 'runtime')
        foreach ($name in @('settings.json', 'settings.json.tmp', 'settings.json.bak')) {
            [IO.File]::Delete([IO.Path]::Combine($settingsFolder, $name))
        }
        if ([IO.Directory]::Exists($runtime)) {
            [IO.File]::Delete([IO.Path]::Combine($runtime, 'TrayTemps.exe.config'))
            foreach ($temporary in [IO.Directory]::GetFiles($runtime, 'TrayTemps.exe.config.*.tmp')) {
                $name = [IO.Path]::GetFileName($temporary)
                if ($name -cmatch '^TrayTemps\.exe\.config\.([0-9]+)\.tmp$') {
                    $runtimeProcessId = 0
                    if ([int]::TryParse($Matches[1], [Globalization.NumberStyles]::None, [Globalization.CultureInfo]::InvariantCulture, [ref]$runtimeProcessId) -and $runtimeProcessId -gt 0) { [IO.File]::Delete($temporary) }
                }
            }
            Remove-EmptyDirectory $runtime
        }
        Remove-EmptyDirectory $settingsFolder
    }
    [IO.File]::Delete($shortcut)
} catch {
    $failureRecord = $_
    $failure = $_.Exception.Message
    $logPath = $PSCommandPath + '.log'
    try { [IO.File]::WriteAllText($logPath, ($failureRecord.ToString() + [Environment]::NewLine + $failureRecord.ScriptStackTrace)) } catch { }
    if ($showErrors) {
        try {
            Add-Type -AssemblyName System.Windows.Forms
            [System.Windows.Forms.MessageBox]::Show(('TrayTemps could not finish uninstalling.' + [Environment]::NewLine + [Environment]::NewLine + $failure + [Environment]::NewLine + [Environment]::NewLine + 'Details: ' + $logPath), 'Uninstall Error', [System.Windows.Forms.MessageBoxButtons]::OK, [System.Windows.Forms.MessageBoxIcon]::Error) | Out-Null
        } catch { }
    }
    throw $failureRecord
} finally { [IO.File]::Delete($PSCommandPath) }
";
        }
    }
}
