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

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

Import-Module "$PSScriptRoot/IronExiles.Dev.psm1" -Force

$unityPath = Get-UnityEditorPath
$projectRoot = Get-IronExilesProjectRoot
$projectPath = Assert-IronExilesUnityProject

if ($Linux) {
    $buildTarget = 'StandaloneLinux64'
    $defaultOutput = Join-Path $projectRoot 'Builds/Server/Linux'
    $execName = 'IronExilesServer'
} else {
    $buildTarget = 'StandaloneWindows64'
    $defaultOutput = Join-Path $projectRoot 'Builds/Server/Windows'
    $execName = 'IronExilesServer.exe'
}

$output = if ($OutputDir) { $OutputDir } else { $defaultOutput }
New-Item -ItemType Directory -Force -Path $output | Out-Null
$outputPath = Join-Path (Resolve-Path $output) $execName

Write-Host "Building dedicated server ($buildTarget) -> $outputPath"

if (-not $Linux -and -not (Test-Path $outputPath)) {
    Install-UnityWindowsServerModule | Out-Null
}

$executeMethod = if ($Linux) {
    'IronExiles.Editor.ServerBuildMenu.BuildLinuxServer'
} else {
    'IronExiles.Editor.ServerBuildMenu.BuildWindowsServer'
}

$logFile = Join-Path $projectRoot 'build-server.log'

$unityArgs = @(
    '-batchmode',
    '-nographics',
    '-projectPath', $projectPath,
    '-executeMethod', $executeMethod,
    '-logFile', $logFile,
    '-quit'
)

Write-Host "Unity log: $logFile"

$process = Start-Process -FilePath $unityPath -ArgumentList $unityArgs -Wait -PassThru -NoNewWindow
if ($process.ExitCode -ne 0) {
    throw "Unity build failed with exit code $($process.ExitCode). See $logFile"
}

if (-not (Test-Path $outputPath)) {
    throw "Unity reported success but server executable was not found at '$outputPath'. See $logFile"
}

Write-Host "Build complete: $outputPath"
