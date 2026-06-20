<#
.SYNOPSIS
    Builds the Iron Exiles dedicated server (headless).
.PARAMETER Linux
    Build for Linux instead of Windows.
.PARAMETER OutputDir
    Output directory for the build. Defaults to Builds/Server/<platform>.
#>
param(
    [switch]$Linux,
    [string]$OutputDir
)

$ErrorActionPreference = 'Stop'
Import-Module "$PSScriptRoot/IronExiles.Dev.psm1" -Force

$unityPath = Get-UnityEditorPath
$projectPath = Resolve-Path "$PSScriptRoot/../Client"

if ($Linux) {
    $buildTarget = "StandaloneLinux64"
    $defaultOutput = "$PSScriptRoot/../Builds/Server/Linux"
    $execName = "IronExilesServer"
} else {
    $buildTarget = "StandaloneWindows64"
    $defaultOutput = "$PSScriptRoot/../Builds/Server/Windows"
    $execName = "IronExilesServer.exe"
}

$output = if ($OutputDir) { $OutputDir } else { $defaultOutput }
New-Item -ItemType Directory -Force -Path $output | Out-Null
$outputPath = Join-Path (Resolve-Path $output) $execName

Write-Host "Building dedicated server ($buildTarget) -> $outputPath"

$executeMethod = if ($Linux) {
    "IronExiles.Editor.ServerBuildMenu.BuildLinuxServer"
} else {
    "IronExiles.Editor.ServerBuildMenu.BuildWindowsServer"
}

$args = @(
    "-batchmode",
    "-nographics",
    "-projectPath", $projectPath,
    "-buildTarget", $buildTarget,
    "-standaloneBuildSubtarget", "Server",
    "-executeMethod", $executeMethod,
    "-logFile", "build-server.log",
    "-quit"
)

& $unityPath @args
if ($LASTEXITCODE -ne 0) {
    throw "Unity build failed with exit code $LASTEXITCODE"
}

Write-Host "Build complete: $outputPath"
