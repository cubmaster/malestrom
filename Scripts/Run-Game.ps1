#Requires -Version 5.1
<#
.SYNOPSIS
  Launches Iron Exiles in standalone game mode (no editor UI).
#>
param(
    [string]$MapPath = '/Game/Maps/Test/EmptySector',
    [switch]$Wait
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

Import-Module (Join-Path $PSScriptRoot 'IronExiles.Dev.psm1') -Force

$ueRoot = Get-UERoot
$editor = Get-UnrealEditor -UERoot $ueRoot
$uproject = Get-IronExilesUProject
$arguments = @(
    $uproject,
    $MapPath,
    '-game',
    '-log'
)

Write-Host "Launching Iron Exiles (standalone)..."
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
