#Requires -Version 5.1
<#
.SYNOPSIS
  Opens the Iron Exiles Unity project in the Editor (no compile step required).
#>
param(
    [string]$ScenePath,
    [switch]$UseHub,
    [switch]$Wait
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

Write-Host 'Unity projects compile on open; launching Editor...'
& (Join-Path $PSScriptRoot 'Launch-UnityEditor.ps1') @PSBoundParameters
