<#
.SYNOPSIS
    Launches the built dedicated server executable.
.PARAMETER Port
    Server listen port. Default: 7878.
.PARAMETER Linux
    Use the Linux build path (for WSL testing).
#>
param(
    [ushort]$Port = 7878,
    [switch]$Linux
)

$ErrorActionPreference = 'Stop'

if ($Linux) {
    $serverPath = "$PSScriptRoot/../Builds/Server/Linux/IronExilesServer"
} else {
    $serverPath = "$PSScriptRoot/../Builds/Server/Windows/IronExilesServer.exe"
}

if (-not (Test-Path $serverPath)) {
    Write-Error "Server build not found at $serverPath. Run Build-DedicatedServer.ps1 first."
    return
}

Write-Host "Starting dedicated server on port $Port..."
& $serverPath -batchmode -nographics -serverPort $Port
