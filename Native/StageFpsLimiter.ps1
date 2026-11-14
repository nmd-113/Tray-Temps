param([string]$NativeRoot, [switch]$ValidateOnly)
$ErrorActionPreference = 'Stop'
$repo = Split-Path $PSScriptRoot -Parent
$manifestPath = Join-Path $PSScriptRoot 'PAYLOADS.json'
$helperPath = Join-Path $repo 'Helpers\Performance\FpsLimiterPayloads.cs'

function Write-IfChanged([string]$Path, [string]$Text) {
    if (!(Test-Path -LiteralPath $Path) -or [IO.File]::ReadAllText($Path) -cne $Text) {
        [IO.File]::WriteAllText($Path, $Text, [Text.UTF8Encoding]::new($false))
    }
}

if (!$ValidateOnly -and !$NativeRoot) { throw 'A native output directory is required for staging.' }
$hashes = @{}
foreach ($arch in @('x64', 'x86')) {
    $destination = Join-Path $repo "Resources\FpsLimiter\$arch\FpsLimiter.Native.dll"
    $source = if ($ValidateOnly) { $destination } else { Join-Path $NativeRoot "$arch\FpsLimiter.Native.dll" }
    if (!(Test-Path -LiteralPath $source -PathType Leaf)) { throw "Missing $arch limiter output: $source" }
    $hashes[$arch] = (Get-FileHash -LiteralPath $source -Algorithm SHA256).Hash
    if (!$ValidateOnly) {
        New-Item -ItemType Directory -Path (Split-Path $destination) -Force | Out-Null
        if (!(Test-Path -LiteralPath $destination) -or (Get-FileHash -LiteralPath $destination).Hash -ne $hashes[$arch]) {
            Copy-Item -LiteralPath $source -Destination $destination -Force
        }
    }
}
$sha = [Security.Cryptography.SHA256]::Create()
try { $combined = [BitConverter]::ToString($sha.ComputeHash([Text.Encoding]::ASCII.GetBytes($hashes.x64 + $hashes.x86))).Replace('-', '') }
finally { $sha.Dispose() }
$version = '1.0.0-' + $combined.Substring(0, 12)
$helper = [IO.File]::ReadAllText($helperPath)
if ($ValidateOnly) {
    $manifest = Get-Content -Raw -LiteralPath $manifestPath | ConvertFrom-Json
    if ($manifest.x64 -ne $hashes.x64 -or $manifest.x86 -ne $hashes.x86 -or $manifest.EngineVersion -ne $version -or
        $helper -notmatch ('EngineVersion\s*=\s*"' + [regex]::Escape($version) + '"')) {
        throw 'FPS limiter payloads, manifest and cache version do not match. Rebuild the solution.'
    }
}
else {
    $manifest = [ordered]@{ EngineVersion = $version; x64 = $hashes.x64; x86 = $hashes.x86 } | ConvertTo-Json
    Write-IfChanged $manifestPath ($manifest + [Environment]::NewLine)
    Write-IfChanged $helperPath ([regex]::Replace($helper, 'EngineVersion = "[^"]+"', ('EngineVersion = "' + $version + '"')))
    Write-Output "Staged FPS limiter payloads: $version"
}
