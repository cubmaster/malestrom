#Requires -Version 5.1
<#
.SYNOPSIS
  Opens EmptySector in the Unity Editor for local play-testing (press Play in the Editor).
#>
param(
    [string]$ScenePath = 'Assets/Scenes/Test/EmptySector.unity',
    [switch]$UseHub,
    [switch]$Wait
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

Import-Module (Join-Path $PSScriptRoot 'IronExiles.Dev.psm1') -Force

# Ensure single-player mode by removing the multiplayer flag file
$flagPath = Join-Path $PSScriptRoot '../.iron-exiles-auto-connect'
if (Test-Path $flagPath) {
    Remove-Item $flagPath -Force
}

$unityProject = Assert-IronExilesUnityProject
$sceneFullPath = Join-Path $unityProject ($ScenePath -replace '/', [IO.Path]::DirectorySeparatorChar)
if (-not (Test-Path $sceneFullPath)) {
    throw "Scene not found at '$sceneFullPath'."
}

Write-Host 'Opening EmptySector in Unity Editor (press Play to fly)...'
& (Join-Path $PSScriptRoot 'Launch-UnityEditor.ps1') -ScenePath $ScenePath -UseHub:$UseHub -Wait:$Wait
