#Requires -Version 5.1
<#
.SYNOPSIS
  Resolves engine paths and project roots for Iron Exiles scripts (Unity + legacy UE5).
#>
Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

$LocalUERootFile = Join-Path $PSScriptRoot 'ue-root.local.ps1'
if (Test-Path $LocalUERootFile) {
    . $LocalUERootFile
}

$LocalUnityRootFile = Join-Path $PSScriptRoot 'unity-root.local.ps1'
if (Test-Path $LocalUnityRootFile) {
    . $LocalUnityRootFile
}

function Get-IronExilesProjectRoot {
    $root = Split-Path -Parent $PSScriptRoot
    return (Resolve-Path $root).Path
}

function Get-IronExilesUnityProject {
    return Join-Path (Get-IronExilesProjectRoot) 'Client'
}

function Get-IronExilesUProject {
    return Join-Path (Get-IronExilesProjectRoot) 'IronExiles.uproject'
}

function Get-UnityEditorFromHub {
    $hubRoot = Join-Path ${env:ProgramFiles} 'Unity\Hub\Editor'
    if (-not (Test-Path $hubRoot)) {
        return $null
    }

    $preferred = Get-ChildItem $hubRoot -Directory |
        Where-Object { $_.Name -like '6000.*' } |
        Sort-Object Name -Descending |
        Select-Object -First 1

    if (-not $preferred) {
        $preferred = Get-ChildItem $hubRoot -Directory | Sort-Object Name -Descending | Select-Object -First 1
    }

    if (-not $preferred) {
        return $null
    }

    $editor = Join-Path $preferred.FullName 'Editor\Unity.exe'
    if (Test-Path $editor) {
        return (Resolve-Path $editor).Path
    }

    return $null
}

function Get-UnityEditorPath {
    if ($env:UNITY_ROOT) {
        $candidate = Join-Path $env:UNITY_ROOT 'Editor\Unity.exe'
        if (Test-Path $candidate) {
            return (Resolve-Path $candidate).Path
        }
        if (Test-Path $env:UNITY_ROOT) {
            return (Resolve-Path $env:UNITY_ROOT).Path
        }
        throw "UNITY_ROOT is set to '$($env:UNITY_ROOT)' but Unity.exe was not found."
    }

    $fromHub = Get-UnityEditorFromHub
    if ($fromHub) {
        return $fromHub
    }

    $exampleFile = Join-Path $PSScriptRoot 'unity-root.local.ps1.example'
    $localFile = Join-Path $PSScriptRoot 'unity-root.local.ps1'
    throw @"
Unity Editor was not found.

Do one of the following:
  1. Install Unity 6000.0.32f1 (Unity 6 LTS) via Unity Hub, or
  2. Copy '$exampleFile' to '$localFile' and set UNITY_ROOT to your Editor folder, or
  3. Set a user environment variable UNITY_ROOT to the Editor install folder
     (for example: C:\Program Files\Unity\Hub\Editor\6000.0.32f1)

Then rerun: powershell -File Scripts/Run-UnityTests.ps1
"@
}

function Get-UERootFromEpicLauncher {
    $launcherDat = Join-Path $env:ProgramData 'Epic\UnrealEngineLauncher\LauncherInstalled.dat'
    if (-not (Test-Path $launcherDat)) {
        return $null
    }

    try {
        $installed = Get-Content $launcherDat -Raw | ConvertFrom-Json
        $entries = @($installed.InstallationList)
        if ($entries.Count -eq 0) {
            return $null
        }

        $preferred = $entries | Where-Object { $_.AppName -match '^UE_5\.5' } | Select-Object -First 1
        if (-not $preferred) {
            $preferred = $entries | Sort-Object AppName -Descending | Select-Object -First 1
        }

        if ($preferred -and $preferred.InstallLocation -and (Test-Path $preferred.InstallLocation)) {
            return (Resolve-Path $preferred.InstallLocation).Path
        }
    }
    catch {
        return $null
    }

    return $null
}

function Get-UERoot {
    if ($env:UE_ROOT) {
        if (-not (Test-Path $env:UE_ROOT)) {
            throw "UE_ROOT is set to '$($env:UE_ROOT)' but that path does not exist."
        }
        return (Resolve-Path $env:UE_ROOT).Path
    }

    $candidates = @(
        (Get-UERootFromEpicLauncher),
        'C:\Program Files\Epic Games\UE_5.5',
        'D:\Program Files\Epic Games\UE_5.5',
        'E:\Program Files\Epic Games\UE_5.5',
        'C:\Program Files\Epic Games\UE_5.4'
    ) | Where-Object { $_ -and (Test-Path $_) }

    foreach ($candidate in $candidates) {
        $editor = Join-Path $candidate 'Engine\Binaries\Win64\UnrealEditor.exe'
        if (Test-Path $editor) {
            return (Resolve-Path $candidate).Path
        }
    }

    $exampleFile = Join-Path $PSScriptRoot 'ue-root.local.ps1.example'
    $localFile = Join-Path $PSScriptRoot 'ue-root.local.ps1'
    throw @"
Unreal Engine was not found.

Do one of the following:
  1. Install UE 5.5 from the Epic Games Launcher, or
  2. Copy '$exampleFile' to '$localFile' and set your engine path, or
  3. Set a user environment variable UE_ROOT to your engine folder
     (for example: C:\Program Files\Epic Games\UE_5.5)

Then rerun: powershell -File Scripts/Run-Game.ps1
"@
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

function Get-UnrealEditor {
    param(
        [Parameter(Mandatory = $true)]
        [string]$UERoot
    )

    $editor = Join-Path $UERoot 'Engine\Binaries\Win64\UnrealEditor.exe'
    if (-not (Test-Path $editor)) {
        throw "UnrealEditor.exe not found at $editor"
    }
    return $editor
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
