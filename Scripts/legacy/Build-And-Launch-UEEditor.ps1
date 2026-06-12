#Requires -Version 5.1
<#
.SYNOPSIS
  [Legacy UE5] Builds the editor target, then launches Unreal Editor.
#>
param(
    [switch]$SkipBuild
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

if (-not $SkipBuild) {
    & (Join-Path $PSScriptRoot 'Build-Editor.ps1')
}

& (Join-Path $PSScriptRoot 'Launch-UEEditor.ps1')
