---
id: TASK-012
title: "Edit Mode foundation tests (ProjectLoads)"
status: complete
parent: REQ-051
created: 2026-06-12
updated: 2026-06-12
dependencies: [TASK-010, TASK-011]
---

## Description

Add Unity Test Framework Edit Mode tests verifying project bootstrap and EmptySector scene asset.

## Files to Create/Modify

- `Client/Assets/_Project/Tests/EditMode/IronExiles.Core.Tests.asmdef`
- `Client/Assets/_Project/Tests/EditMode/ProjectLoadsTests.cs`

## Acceptance Criteria

- [ ] Test `ProjectLoads_EmptySectorSceneExists` passes in Edit Mode
- [ ] Test `ProjectLoads_BootstrapTypeLoads` passes in Edit Mode
- [ ] Test assembly references `IronExiles.Core` and Test Framework

## Technical Notes

Tests run in Editor only (Edit Mode). CI invokes via game-ci/unity-test-runner.
