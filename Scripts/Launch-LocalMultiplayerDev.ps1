#Requires -Version 5.1
<#
.SYNOPSIS
  Starts the dedicated server and opens the Unity Editor for local multiplayer dev.
.DESCRIPTION
  Launches IronExilesServer in a separate terminal, waits briefly for the listen port,
  then opens the Unity project (EmptySector by default). Press Play in Unity; this
  launch sets IRON_EXILES_AUTO_CONNECT for the editor process so the client connects
  to the local server and spawns your ship.
.PARAMETER Port
  Server listen port. Default: 7878.
.PARAMETER ScenePath
  Unity scene to open. Default: EmptySector.
.PARAMETER NoBuild
  Fail instead of building when the server executable is missing. By default the script builds automatically.
.PARAMETER SkipServer
  Only open Unity Editor.
.PARAMETER SkipUnity
  Only start the dedicated server.
#>
param(
    [uint16]$Port = 7878,
    [string]$ScenePath = 'Assets/Scenes/Test/EmptySector.unity',
    [switch]$NoBuild,
    [switch]$SkipServer,
    [switch]$SkipUnity,
    [switch]$UseHub
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

Import-Module (Join-Path $PSScriptRoot 'IronExiles.Dev.psm1') -Force

function Test-UdpPortListening {
    param([uint16]$ListenPort)

    if (-not (Get-Command Get-NetUDPEndpoint -ErrorAction SilentlyContinue)) {
        return $false
    }

    $endpoints = @(Get-NetUDPEndpoint -LocalPort $ListenPort -ErrorAction SilentlyContinue)
    return $endpoints.Count -gt 0
}

function Start-DedicatedServerTerminal {
    param(
        [uint16]$ListenPort
    )

    $serverScript = Join-Path $PSScriptRoot 'Run-UnityDedicatedServer.ps1'
    $serverPath = Join-Path $PSScriptRoot '../Builds/Server/Windows/IronExilesServer.exe'

    if (-not (Test-Path $serverPath)) {
        if ($NoBuild) {
            throw @"
Dedicated server build not found at '$serverPath'.

Run:
  .\Scripts\Build-DedicatedServer.ps1
"@
        }

        Write-Host 'Server build not found. Building dedicated server (first run can take several minutes)...'
        & (Join-Path $PSScriptRoot 'Build-DedicatedServer.ps1')
        if (-not (Test-Path $serverPath)) {
            throw "Dedicated server build completed but executable was not found at '$serverPath'."
        }
    }

    if (Test-UdpPortListening -ListenPort $ListenPort) {
        Write-Host "Dedicated server already listening on UDP port $ListenPort. Skipping server launch."
        return
    }

    Write-Host "Starting dedicated server on port $ListenPort in a new terminal..."
    Start-Process -FilePath 'powershell.exe' -ArgumentList @(
        '-NoProfile',
        '-ExecutionPolicy', 'Bypass',
        '-NoExit',
        '-File', $serverScript,
        '-Port', $ListenPort
    ) | Out-Null

    $deadline = (Get-Date).AddSeconds(15)
    while ((Get-Date) -lt $deadline) {
        if (Test-UdpPortListening -ListenPort $ListenPort) {
            Write-Host "Dedicated server is listening on UDP port $ListenPort."
            return
        }

        Start-Sleep -Milliseconds 250
    }

    Write-Warning "Server process started but UDP port $ListenPort is not bound yet. Unity will still open; wait a moment before connecting."
}

if (-not $SkipServer) {
    Start-DedicatedServerTerminal -ListenPort $Port
}

if (-not $SkipUnity) {
    $env:IRON_EXILES_AUTO_CONNECT = '1'

    # Create flag file so an already-running Unity editor can detect multiplayer mode
    $flagPath = Join-Path $PSScriptRoot '../.iron-exiles-auto-connect'
    Set-Content -Path $flagPath -Value '1' -NoNewline

    Write-Host ''
    Write-Host 'Opening Unity Editor...'
    Write-Host '  Press Play — the editor starts as host (server+client) for local multiplayer.'
    Write-Host '  Flag file created: .iron-exiles-auto-connect (delete to return to single-player mode).'
    Write-Host ''

    & (Join-Path $PSScriptRoot 'Launch-UnityEditor.ps1') -ScenePath $ScenePath -UseHub:$UseHub
}
