#Requires -Version 5.1
Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

Import-Module (Join-Path $PSScriptRoot 'IronExiles.Dev.psm1') -Force

$ueRoot = Get-UERoot
$editorCmd = Get-UnrealEditorCmd -UERoot $ueRoot
$uproject = Get-IronExilesUProject

Write-Host "Running IronExiles.Foundation automation tests..."
Write-Host "  UE_ROOT:   $ueRoot"
Write-Host "  Project:   $uproject"

& $editorCmd $uproject `
    -ExecCmds="Automation RunTests IronExiles.Foundation; Quit" `
    -unattended `
    -nop4 `
    -NullRHI

if ($LASTEXITCODE -ne 0) {
    throw "Foundation tests failed with exit code $LASTEXITCODE"
}

Write-Host "Foundation tests passed."
