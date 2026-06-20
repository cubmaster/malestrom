---
id: TASK-027
title: "Dedicated server build scripts and local multi-client test"
status: complete
parent: REQ-035
created: 2026-06-19
updated: 2026-06-19
dependencies: [TASK-025, TASK-026]
---

## Description

Create build scripts for the dedicated server (Windows + Linux) and document the local two-client + server test workflow.

## Files to Create/Modify

- `Scripts/Build-DedicatedServer.ps1` — PowerShell script that invokes Unity batchmode to build the Dedicated Server target (Windows by default, `-Linux` flag for Linux headless)
- `Scripts/Run-UnityDedicatedServer.ps1` — launches the built Windows server executable with configurable port
- `Client/Assets/_Project/Editor/ServerBuildMenu.cs` — Unity Editor menu item for one-click server build (Iron Exiles > Build Dedicated Server)
- `README.md` or `docs/local-multiplayer-test.md` — step-by-step for local testing: build server → launch → launch 2 clients → verify

## Acceptance Criteria

- [ ] `Build-DedicatedServer.ps1` produces a runnable server executable for Windows
- [ ] `Build-DedicatedServer.ps1 -Linux` produces a Linux headless binary
- [ ] `Run-UnityDedicatedServer.ps1` launches the server on port 7878
- [ ] Documentation covers: build → launch server → launch 2 clients → observe 2 ships
- [ ] Server runs headless (no rendering window on dedicated server build)

## Technical Notes

- Unity 6 Dedicated Server platform uses `-standaloneBuildSubtarget Server` or the Dedicated Server platform in batchmode
- Build command: `unity -batchmode -nographics -projectPath ./Client -executeMethod IronExiles.Editor.ServerBuildMenu.BuildLinuxServer -quit`
- Windows build: `-buildTarget StandaloneWindows64 -standaloneBuildSubtarget Server`
- Linux build: `-buildTarget StandaloneLinux64 -standaloneBuildSubtarget Server`
