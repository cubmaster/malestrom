---
id: TASK-010
title: "IronExiles.Core assembly and bootstrap types"
status: draft
parent: REQ-051
created: 2026-06-12
updated: 2026-06-12
dependencies: [TASK-009]
---

## Description

Add the primary runtime assembly under `Client/Assets/_Project/` with a minimal bootstrap type used by tests and future game systems.

## Files to Create/Modify

- `Client/Assets/_Project/Scripts/Core/IronExiles.Core.asmdef`
- `Client/Assets/_Project/Scripts/Core/GameBootstrap.cs`

## Acceptance Criteria

- [ ] `IronExiles.Core` assembly compiles (namespace `IronExiles.Core`)
- [ ] `GameBootstrap.ProjectName` returns `"Iron Exiles"`

## Technical Notes

Match C# naming from conventions.md. No gameplay logic beyond bootstrap constants.
