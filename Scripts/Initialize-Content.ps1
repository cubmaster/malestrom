#Requires -Version 5.1
Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

Import-Module (Join-Path $PSScriptRoot 'IronExiles.Dev.psm1') -Force

$ueRoot = Get-UERoot
$editorCmd = Get-UnrealEditorCmd -UERoot $ueRoot
$uproject = Get-IronExilesUProject
$pythonScript = Join-Path (Get-IronExilesProjectRoot) 'Content\Python\create_empty_sector.py'

if (-not (Test-Path $pythonScript)) {
    throw "Missing content script: $pythonScript"
}

Write-Host "Generating EmptySector map via Unreal Editor Python..."
Write-Host "  Script: $pythonScript"

& $editorCmd $uproject `
    -ExecutePythonScript=$pythonScript `
    -unattended `
    -nop4

if ($LASTEXITCODE -ne 0) {
    throw "Initialize-Content failed with exit code $LASTEXITCODE"
}

Write-Host "Content initialization complete."
