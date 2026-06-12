# Runs Unity Edit Mode tests for the Iron Exiles Client/ project.
param(
    [string]$ProjectPath = "Client",
    [string]$ResultsPath = "Client/TestResults.xml"
)

$ErrorActionPreference = "Stop"
Import-Module (Join-Path $PSScriptRoot 'IronExiles.Dev.psm1') -Force

$projectRoot = Get-IronExilesProjectRoot
$unityProject = Join-Path $projectRoot $ProjectPath
$resultsFile = Join-Path $projectRoot $ResultsPath

if (-not (Test-Path (Join-Path $unityProject 'ProjectSettings/ProjectVersion.txt'))) {
    throw "Unity project not found at '$unityProject'."
}

$unity = Get-UnityEditorPath
$resultsDir = Split-Path $resultsFile -Parent
if (-not (Test-Path $resultsDir)) {
    New-Item -ItemType Directory -Path $resultsDir | Out-Null
}

Write-Host "Running Edit Mode tests with: $unity"
Write-Host "Project: $unityProject"

& $unity `
    -batchmode `
    -nographics `
    -silent-crashes `
    -projectPath $unityProject `
    -runTests `
    -testPlatform editmode `
    -testResults $resultsFile `
    -logFile -

if ($LASTEXITCODE -ne 0) {
    throw "Unity Edit Mode tests failed with exit code $LASTEXITCODE."
}

Write-Host "Edit Mode tests passed. Results: $resultsFile"
