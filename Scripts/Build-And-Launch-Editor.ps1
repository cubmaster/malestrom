#Requires -Version 5.1
<#
.SYNOPSIS
  Builds the editor target, then launches Unreal Editor.
#>
param(
    [switch]$SkipBuild
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

if (-not $SkipBuild) {
    & (Join-Path $PSScriptRoot 'Build-Editor.ps1')
}

& (Join-Path $PSScriptRoot 'Launch-Editor.ps1')
