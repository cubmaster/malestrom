---
id: TASK-003
title: "IronExiles.Foundation automation tests"
status: complete
parent: REQ-032
created: 2026-06-12
updated: 2026-06-12
dependencies: [TASK-002]
---

## Description

Add UE Automation tests under the `IronExiles.Foundation` category validating project load, map existence, and GameMode configuration.

## Files to Create/Modify

- `Source/IronExiles/Tests/IronExilesFoundationTest.cpp` — automation tests
- `Scripts/Run-FoundationTests.ps1` — runs UnrealEditor-Cmd with Automation filter
- `Source/IronExiles/IronExiles.Build.cs` — confirm AutomationTest dependency if not already present

## Acceptance Criteria

- [ ] Test `IronExiles.Foundation.ProjectLoads` passes: loads `EmptySector`, world valid, GameMode is `AIronExilesGameModeBase`
- [ ] Test `IronExiles.Foundation.DefaultMapConfigured` passes: default map path equals `/Game/Maps/Test/EmptySector`
- [ ] `Scripts/Run-FoundationTests.ps1` exits 0 when all Foundation tests pass
- [ ] Tests discoverable in Session Frontend filter `IronExiles.Foundation`

## Technical Notes

- Use `IMPLEMENT_SIMPLE_AUTOMATION_TEST` or `FAutomationTestFramework` patterns per UE 5.5 docs
- Tests must run headless via `-ExecCmds="Automation RunTests IronExiles.Foundation; Quit"`
- Follow ADR-032-3
