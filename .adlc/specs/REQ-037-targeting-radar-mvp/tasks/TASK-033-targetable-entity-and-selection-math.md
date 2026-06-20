---
id: TASK-033
title: "Targetable entity model and TargetSelectionMath"
status: complete
parent: REQ-037
created: 2026-06-20
updated: 2026-06-20
dependencies: []
repo: malestrom
---

## Description

Define `TargetableEntity`, affiliation enum, sensor defaults, and pure selection/validation math for tab cycle and radar scan.

## Files to Create/Modify

- `Client/Assets/_Project/Scripts/Combat/TargetableEntity.cs`
- `Client/Assets/_Project/Scripts/Combat/TargetSelectionMath.cs`
- `Client/Assets/_Project/Scripts/Combat/TargetingSensorSettings.cs`

## Acceptance Criteria

- [ ] TargetableEntity exposes display name, affiliation, hull percent, NetworkObject id
- [ ] TargetSelectionMath sorts tab candidates by forward angle then distance
- [ ] Radar scan returns closest N contacts within lock range
- [ ] Edit Mode tests cover sort/cycle/range rejection
