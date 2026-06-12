#Requires -Version 5.1
<#
.SYNOPSIS
  Opens the Iron Exiles Unity project (Client/) in the Unity Editor.
#>
param(
    [string]$ScenePath,
    [switch]$UseHub,
    [switch]$Wait
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

Import-Module (Join-Path $PSScriptRoot 'IronExiles.Dev.psm1') -Force

$unityProject = Assert-IronExilesUnityProject

if ($UseHub) {
    $hub = Get-UnityHubPath
    if (-not $hub) {
        throw 'Unity Hub was not found. Install Unity Hub or omit -UseHub to launch Unity.exe directly.'
    }

    Write-Host 'Opening Unity Hub with project...'
    Write-Host "  Project: $unityProject"
    Start-Process -FilePath $hub -ArgumentList @('--', '--projectPath', $unityProject)
    return
}

$unity = Get-UnityEditorPath
$arguments = @('-projectPath', $unityProject)
if ($ScenePath) {
    $arguments += $ScenePath
}

Write-Host 'Launching Unity Editor...'
Write-Host "  Unity:   $unity"
Write-Host "  Project: $unityProject"
if ($ScenePath) {
    Write-Host "  Scene:   $ScenePath"
}

if ($Wait) {
    & $unity @arguments
    if ($LASTEXITCODE -ne 0) {
        throw "Unity Editor exited with code $LASTEXITCODE"
    }
}
else {
    Start-Process -FilePath $unity -ArgumentList $arguments
    Write-Host 'Unity Editor started.'
}
