#Requires -Version 5.1
<#
.SYNOPSIS
  Resolves UE_ROOT and project paths for Iron Exiles scripts.
#>
Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

function Get-IronExilesProjectRoot {
    $root = Split-Path -Parent $PSScriptRoot
    return (Resolve-Path $root).Path
}

function Get-IronExilesUProject {
    return Join-Path (Get-IronExilesProjectRoot) 'IronExiles.uproject'
}

function Get-UERoot {
    if ($env:UE_ROOT) {
        return (Resolve-Path $env:UE_ROOT).Path
    }

    $candidates = @(
        'C:\Program Files\Epic Games\UE_5.5',
        'C:\Program Files\Epic Games\UE_5.4'
    )

    foreach ($candidate in $candidates) {
        if (Test-Path $candidate) {
            return $candidate
        }
    }

    throw "UE_ROOT is not set and no default UE install was found. Set UE_ROOT to your Unreal Engine directory."
}

function Get-UnrealEditorCmd {
    param(
        [Parameter(Mandatory = $true)]
        [string]$UERoot
    )

    $cmd = Join-Path $UERoot 'Engine\Binaries\Win64\UnrealEditor-Cmd.exe'
    if (-not (Test-Path $cmd)) {
        throw "UnrealEditor-Cmd.exe not found at $cmd"
    }
    return $cmd
}

function Get-BuildBat {
    param(
        [Parameter(Mandatory = $true)]
        [string]$UERoot
    )

    $build = Join-Path $UERoot 'Engine\Build\BatchFiles\Build.bat'
    if (-not (Test-Path $build)) {
        throw "Build.bat not found at $build"
    }
    return $build
}
