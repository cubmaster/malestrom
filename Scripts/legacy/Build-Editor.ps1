#Requires -Version 5.1
Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

Import-Module (Join-Path (Split-Path $PSScriptRoot -Parent) 'IronExiles.Dev.psm1') -Force

$ueRoot = Get-UERoot
$buildBat = Get-BuildBat -UERoot $ueRoot
$uproject = Get-IronExilesUProject

Write-Host "Building IronExilesEditor (Development Win64)..."
Write-Host "  UE_ROOT:   $ueRoot"
Write-Host "  Project:   $uproject"

& $buildBat IronExilesEditor Win64 Development "-Project=$uproject" -WaitMutex
if ($LASTEXITCODE -ne 0) {
    throw "Build failed with exit code $LASTEXITCODE"
}

Write-Host "Build succeeded."
