param([string]$VulkanSdkRoot = $env:VULKAN_SDK, [switch]$Stage)
$ErrorActionPreference = 'Stop'
$repo = Split-Path $PSScriptRoot -Parent
if (!$VulkanSdkRoot) { $VulkanSdkRoot = Join-Path $repo 'ThirdParty\Vulkan-Headers' }
$vswhere = Join-Path ${env:ProgramFiles(x86)} 'Microsoft Visual Studio\Installer\vswhere.exe'
if (!(Test-Path -LiteralPath $vswhere -PathType Leaf)) { throw 'Visual Studio 2022 with the v143 C++ tools is required.' }
$msbuild = & $vswhere -latest -products '*' -requires Microsoft.Component.MSBuild -find 'MSBuild\**\Bin\MSBuild.exe' | Select-Object -First 1
if (!$msbuild) { throw 'Visual Studio MSBuild is required.' }
foreach ($header in @('vulkan.h', 'vk_layer.h')) {
    if (!(Test-Path -LiteralPath (Join-Path $VulkanSdkRoot ('include\vulkan\' + $header)))) { throw 'Vulkan headers are missing.' }
}
$nativeRoot = Join-Path $PSScriptRoot 'out\Release'
foreach ($arch in @('x64', 'x86')) {
    $platform = if ($arch -eq 'x86') { 'Win32' } else { 'x64' }
    $output = Join-Path $nativeRoot ($arch + '\')
    & $msbuild (Join-Path $PSScriptRoot 'FpsLimiter.Native\FpsLimiter.Native.vcxproj') /m /t:Rebuild /p:Configuration=Release "/p:Platform=$platform" "/p:VulkanSdkRoot=$VulkanSdkRoot" "/p:OutDir=$output" /p:StageFpsLimiterPayloads=false /v:minimal
    if ($LASTEXITCODE -ne 0) { throw "Native $arch build failed." }
}
if ($Stage) { & (Join-Path $PSScriptRoot 'StageFpsLimiter.ps1') -NativeRoot $nativeRoot }
else { Write-Output 'Native builds verified. The normal solution build stages payloads automatically; use -Stage to stage this standalone verification build.' }
