---
id: TASK-013
title: "Unity dev scripts and local editor path resolution"
status: draft
parent: REQ-051
created: 2026-06-12
updated: 2026-06-12
dependencies: [TASK-009]
---

## Description

Implement `Run-UnityTests.ps1` and Unity Hub path resolution (mirroring UE `ue-root.local.ps1` pattern).

## Files to Create/Modify

- `Scripts/Run-UnityTests.ps1`
- `Scripts/unity-root.local.ps1.example`
- `Scripts/IronExiles.Dev.psm1` — add `Get-UnityEditorPath`

## Acceptance Criteria

- [ ] Script exits with clear error if `Client/` missing
- [ ] Script resolves Unity editor from `unity-root.local.ps1`, `UNITY_ROOT`, or Hub install folder
- [ ] Script documents batchmode test invocation when editor is found

## Technical Notes

Machine may not have Unity installed; script must fail gracefully with setup instructions.
