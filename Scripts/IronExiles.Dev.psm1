#Requires -Version 5.1
<#
.SYNOPSIS
  Resolves engine paths and project roots for Iron Exiles scripts (Unity + legacy UE5).
#>
Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

$LocalUERootFile = Join-Path $PSScriptRoot 'legacy\ue-root.local.ps1'
if (-not (Test-Path $LocalUERootFile)) {
    $LocalUERootFile = Join-Path $PSScriptRoot 'ue-root.local.ps1'
}
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

function Assert-IronExilesUnityProject {
    param(
        [string]$UnityProject = (Get-IronExilesUnityProject)
    )

    $versionFile = Join-Path $UnityProject 'ProjectSettings/ProjectVersion.txt'
    if (-not (Test-Path $versionFile)) {
        throw "Unity project not found at '$UnityProject' (missing ProjectSettings/ProjectVersion.txt)."
    }

    return (Resolve-Path $UnityProject).Path
}

function Get-UnityPinnedEditorVersion {
    $versionFile = Join-Path (Get-IronExilesUnityProject) 'ProjectSettings/ProjectVersion.txt'
    if (-not (Test-Path $versionFile)) {
        return $null
    }

    foreach ($line in Get-Content $versionFile) {
        if ($line -match '^m_EditorVersion:\s*(.+)$') {
            return $Matches[1].Trim()
        }
    }

    return $null
}

function Get-UnityHubPath {
    $hub = Join-Path ${env:ProgramFiles} 'Unity Hub/Unity Hub.exe'
    if (Test-Path $hub) {
        return (Resolve-Path $hub).Path
    }

    return $null
}

function Get-UnityEditorFromHub {
    param(
        [string]$PreferredVersion = (Get-UnityPinnedEditorVersion)
    )

    $hubRoot = Join-Path ${env:ProgramFiles} 'Unity\Hub\Editor'
    if (-not (Test-Path $hubRoot)) {
        return $null
    }

    if ($PreferredVersion) {
        $exactEditor = Join-Path $hubRoot "$PreferredVersion/Editor/Unity.exe"
        if (Test-Path $exactEditor) {
            return (Resolve-Path $exactEditor).Path
        }
    }

    $candidates = Get-ChildItem $hubRoot -Directory |
        Where-Object { $_.Name -like '6000.*' } |
        Sort-Object {
            if ($PreferredVersion -and $_.Name -eq $PreferredVersion) { 0 }
            elseif ($_.Name -like '6000.0.*') { 1 }
            else { 2 }
        }, Name -Descending

    foreach ($install in $candidates) {
        $editor = Join-Path $install.FullName 'Editor/Unity.exe'
        if (Test-Path $editor) {
            return (Resolve-Path $editor).Path
        }
    }

    $fallback = Get-ChildItem $hubRoot -Directory | Sort-Object Name -Descending | Select-Object -First 1
    if ($fallback) {
        $editor = Join-Path $fallback.FullName 'Editor/Unity.exe'
        if (Test-Path $editor) {
            return (Resolve-Path $editor).Path
        }
    }

    return $null
}

function Get-UnityEditorPath {
    if ($env:UNITY_ROOT) {
        $candidate = Join-Path $env:UNITY_ROOT 'Editor/Unity.exe'
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
        $pinned = Get-UnityPinnedEditorVersion
        if ($pinned -and ($fromHub -notmatch [regex]::Escape($pinned))) {
            Write-Warning "Using Unity editor at '$fromHub' but project pins '$pinned'. Install the pinned version via Unity Hub or set UNITY_ROOT in Scripts/unity-root.local.ps1."
        }
        return $fromHub
    }

    $exampleFile = Join-Path $PSScriptRoot 'unity-root.local.ps1.example'
    $localFile = Join-Path $PSScriptRoot 'unity-root.local.ps1'
    $pinned = Get-UnityPinnedEditorVersion
    $pinnedLine = if ($pinned) { "  Install Unity $pinned via Unity Hub, or" } else { '  Install Unity 6000.0.32f1 (Unity 6 LTS) via Unity Hub, or' }

    throw @"
Unity Editor was not found.

Do one of the following:
  1. $pinnedLine
  2. Copy '$exampleFile' to '$localFile' and set UNITY_ROOT to your Editor folder, or
  3. Set a user environment variable UNITY_ROOT to the Editor install folder
     (for example: C:\Program Files\Unity\Hub\Editor\6000.0.32f1)

Then rerun: powershell -File Scripts/Launch-UnityEditor.ps1
"@
}

function Get-IronExilesUProject {
    return Join-Path (Get-IronExilesProjectRoot) 'IronExiles.uproject'
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

    $exampleFile = Join-Path $PSScriptRoot 'legacy\ue-root.local.ps1.example'
    if (-not (Test-Path $exampleFile)) {
        $exampleFile = Join-Path $PSScriptRoot 'ue-root.local.ps1.example'
    }
    $localFile = Join-Path $PSScriptRoot 'legacy\ue-root.local.ps1'
    if (-not (Test-Path $localFile)) {
        $localFile = Join-Path $PSScriptRoot 'ue-root.local.ps1'
    }
    throw @"
Unreal Engine was not found.

Legacy UE5 scripts live under Scripts/legacy/. To use them:
  1. Install UE 5.5 from the Epic Games Launcher, or
  2. Copy '$exampleFile' to '$localFile' and set your engine path, or
  3. Set a user environment variable UE_ROOT to your engine folder

Then rerun: powershell -File Scripts/legacy/Launch-UEEditor.ps1
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
