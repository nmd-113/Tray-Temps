param([switch]$PackageOnly, [string]$BuiltExecutable, [ValidateSet('Debug', 'Release')][string]$Configuration = 'Release')
# Release builds call PackageOnly after compilation; the wrapper remains usable from a terminal.
$ErrorActionPreference = 'Stop'
$repo = $PSScriptRoot
if (!$PackageOnly) {
    $vswhere = Join-Path ${env:ProgramFiles(x86)} 'Microsoft Visual Studio\Installer\vswhere.exe'
    if (!(Test-Path -LiteralPath $vswhere -PathType Leaf)) { throw 'Visual Studio 2022 or Build Tools with MSBuild is required.' }
    $msbuild = & $vswhere -latest -products '*' -requires Microsoft.Component.MSBuild -find 'MSBuild\**\Bin\MSBuild.exe' | Select-Object -First 1
    if (!$msbuild) { throw 'Visual Studio MSBuild is required.' }
    & $msbuild (Join-Path $repo 'Tray-Temps.sln') /restore /t:Rebuild /m "/p:Configuration=$Configuration" /p:Platform=x64 /v:minimal
    if ($LASTEXITCODE -ne 0) { throw 'TrayTemps build failed.' }
    return
}
& (Join-Path $repo 'Native\StageFpsLimiter.ps1') -ValidateOnly
if (!$BuiltExecutable) { $BuiltExecutable = Join-Path $repo 'bin\x64\Release\TrayTemps.exe' }
if (!(Test-Path -LiteralPath $BuiltExecutable -PathType Leaf)) { throw 'The built release executable is missing.' }
$dist = Join-Path $repo 'dist'
New-Item -ItemType Directory -Path $dist -Force | Out-Null
Copy-Item -LiteralPath $BuiltExecutable -Destination (Join-Path $dist 'TrayTemps.exe') -Force
$exe = Join-Path $dist 'TrayTemps.exe'
$version = [Diagnostics.FileVersionInfo]::GetVersionInfo($exe).FileVersion
if ($version -notmatch '^\d+\.\d+\.\d+\.\d+$') { throw 'The executable has an invalid release version.' }
$archiveName = "TrayTemps-$version-win-x64.zip"
$zip = Join-Path $dist $archiveName
$temporary = $zip + '.tmp'
$quickStart = @"
TrayTemps $version

CPU/GPU temperatures in the tray, hardware information, customizable OSD,
an FPS counter and an optional game FPS limiter.

Requirements: Windows 10 or newer (x64), .NET Framework 4.8.
Extract this archive and run TrayTemps.exe. No installation is required.
Open Settings to select sensors, tray icons, OSD metrics and the FPS limiter.
The limiter is off by default; its gear button configures caps and profiles.

The limiter targets supported x64 Direct3D 9/11/12, OpenGL and Vulkan apps.
Enable it before launching Vulkan games; run those games without admin rights.
TrayTemps itself may run elevated. 32-bit attachment and elevated Vulkan
targets are unsupported. Protected/anti-cheat games have not been verified.
Close standalone TrayFPS before enabling the limiter here.

Administrator rights and optional PawnIO can improve hardware sensor access.
The embedded PawnIO installer runs only after user confirmation.
Settings: %APPDATA%\TrayTemps\settings.json

Source, documentation and releases: https://github.com/nmd-113/Tray-Temps
See LICENSE.txt, FPS-LIMITER-NOTICES.txt and OFL-JetBrainsMono.txt for licenses.
"@
Add-Type -AssemblyName System.IO.Compression
Add-Type -AssemblyName System.IO.Compression.FileSystem
try {
    $stream = [IO.File]::Open($temporary, [IO.FileMode]::Create, [IO.FileAccess]::ReadWrite, [IO.FileShare]::None)
    try {
        $archive = [IO.Compression.ZipArchive]::new($stream, [IO.Compression.ZipArchiveMode]::Create, $true)
        try {
            # Explicit contents keep build caches, tests and local preferences out of the release.
            $files = [ordered]@{
                'TrayTemps.exe' = $exe
                'LICENSE.txt' = (Join-Path $repo 'LICENSE.txt')
                'FPS-LIMITER-NOTICES.txt' = (Join-Path $repo 'Resources\FpsLimiter\NOTICE.txt')
                'OFL-JetBrainsMono.txt' = (Join-Path $repo 'Resources\OFL-JetBrainsMono.txt')
            }
            foreach ($name in $files.Keys) {
                [IO.Compression.ZipFileExtensions]::CreateEntryFromFile($archive, $files[$name], $name, [IO.Compression.CompressionLevel]::Optimal) | Out-Null
            }
            $writer = [IO.StreamWriter]::new($archive.CreateEntry('README.txt').Open(), [Text.UTF8Encoding]::new($false))
            try { $writer.WriteLine($quickStart) } finally { $writer.Dispose() }
        }
        finally { $archive.Dispose() }
    }
    finally { $stream.Dispose() }
    for ($attempt = 0; ; $attempt++) {
        try {
            # Preserve a CLR null backup path; PowerShell casts $null to an empty string here.
            if (Test-Path -LiteralPath $zip) { [IO.File]::Replace($temporary, $zip, [NullString]::Value) }
            else { [IO.File]::Move($temporary, $zip) }
            break
        }
        catch [IO.IOException] {
            $code = $_.Exception.GetBaseException().HResult -band 0xFFFF
            if ($code -notin @(32, 33) -or $attempt -ge 9) { throw }
            Start-Sleep -Milliseconds 200
        }
    }
}
finally {
    if (Test-Path -LiteralPath $temporary) { Remove-Item -LiteralPath $temporary -Force }
}
$checksums = foreach ($name in @('TrayTemps.exe', $archiveName)) {
    $hash = (Get-FileHash -LiteralPath (Join-Path $dist $name) -Algorithm SHA256).Hash.ToLowerInvariant()
    "$hash  $name"
}
[IO.File]::WriteAllLines((Join-Path $dist 'SHA256SUMS.txt'), $checksums, [Text.UTF8Encoding]::new($false))
Write-Output "Built $exe"
Write-Output "Packaged $zip"
Write-Output "Checksums: $dist\SHA256SUMS.txt"
