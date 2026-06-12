#Requires -Version 5.1
<#
.SYNOPSIS
  [Legacy UE5] Launches Iron Exiles in standalone game mode.
#>
param(
    [string]$MapPath = '/Game/Maps/Test/EmptySector',
    [switch]$Wait
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

Import-Module (Join-Path (Split-Path $PSScriptRoot -Parent) 'IronExiles.Dev.psm1') -Force

$ueRoot = Get-UERoot
$editor = Get-UnrealEditor -UERoot $ueRoot
$uproject = Get-IronExilesUProject
$arguments = @(
    $uproject,
    $MapPath,
    '-game',
    '-log'
)

Write-Host "Launching Iron Exiles standalone (legacy UE5)..."
Write-Host "  UE_ROOT:   $ueRoot"
Write-Host "  Project:   $uproject"
Write-Host "  Map:       $MapPath"

if ($Wait) {
    & $editor @arguments
    if ($LASTEXITCODE -ne 0) {
        throw "Game exited with code $LASTEXITCODE"
    }
}
else {
    Start-Process -FilePath $editor -ArgumentList $arguments
    Write-Host "Game started."
}
