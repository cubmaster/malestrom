#Requires -Version 5.1
Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

Import-Module (Join-Path (Split-Path $PSScriptRoot -Parent) 'IronExiles.Dev.psm1') -Force

$ueRoot = Get-UERoot
$editorCmd = Get-UnrealEditorCmd -UERoot $ueRoot
$uproject = Get-IronExilesUProject

Write-Host "Running IronExiles.Flight automation tests..."
Write-Host "  UE_ROOT:   $ueRoot"
Write-Host "  Project:   $uproject"

& $editorCmd $uproject `
    -ExecCmds="Automation RunTests IronExiles.Flight; Quit" `
    -unattended `
    -nop4 `
    -NullRHI

if ($LASTEXITCODE -ne 0) {
    throw "Flight tests failed with exit code $LASTEXITCODE"
}

Write-Host "Flight tests passed."
