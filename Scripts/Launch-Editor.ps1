#Requires -Version 5.1
<#
.SYNOPSIS
  Opens the Iron Exiles Unity project in the Editor (Unity replacement for legacy UE Launch-Editor).
#>
param(
    [string]$ScenePath,
    [switch]$UseHub,
    [switch]$Wait
)

& (Join-Path $PSScriptRoot 'Launch-UnityEditor.ps1') @PSBoundParameters
