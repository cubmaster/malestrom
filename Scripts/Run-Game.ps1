#Requires -Version 5.1
<#
.SYNOPSIS
  Opens EmptySector for local play-testing in Unity (Unity replacement for legacy UE Run-Game).
#>
param(
    [string]$ScenePath = 'Assets/Scenes/Test/EmptySector.unity',
    [switch]$UseHub,
    [switch]$Wait
)

& (Join-Path $PSScriptRoot 'Run-UnityGame.ps1') @PSBoundParameters
