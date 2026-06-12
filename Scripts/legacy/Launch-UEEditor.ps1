#Requires -Version 5.1
<#
.SYNOPSIS
  [Legacy UE5] Launches Unreal Editor with IronExiles.uproject.
#>
param(
    [switch]$Wait
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

Import-Module (Join-Path (Split-Path $PSScriptRoot -Parent) 'IronExiles.Dev.psm1') -Force

$ueRoot = Get-UERoot
$editor = Get-UnrealEditor -UERoot $ueRoot
$uproject = Get-IronExilesUProject

Write-Host "Launching Unreal Editor (legacy UE5)..."
Write-Host "  UE_ROOT:   $ueRoot"
Write-Host "  Project:   $uproject"

if ($Wait) {
    & $editor $uproject
    if ($LASTEXITCODE -ne 0) {
        throw "Unreal Editor exited with code $LASTEXITCODE"
    }
}
else {
    Start-Process -FilePath $editor -ArgumentList @($uproject)
    Write-Host "Unreal Editor started."
}
