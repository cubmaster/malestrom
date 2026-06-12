---
id: TASK-006
title: "Ship stats row and UShipMovementComponent"
status: complete
parent: REQ-033
created: 2026-06-12
updated: 2026-06-12
dependencies: []
---

## Description

Define `FShipStatsRow` DataTable struct and implement `UShipMovementComponent` with Newtonian thrust, rotation, speed clamp, brake, and sector bounds.

## Files to Create/Modify

- `Source/IronExiles/Public/Ship/ShipStatsRow.h` — USTRUCT for DataTable
- `Source/IronExiles/Public/Ship/ShipMovementComponent.h` — component API
- `Source/IronExiles/Private/Ship/ShipMovementComponent.cpp` — movement integration
- `Source/IronExiles/IronExiles.Build.cs` — add dependencies if needed

## Acceptance Criteria

- [ ] Movement integrates local thrust into world velocity with momentum
- [ ] Releasing thrust does not instant-stop; brake input decelerates
- [ ] Velocity magnitude clamped to MaxSpeed every tick
- [ ] Position clamped to sector AABB

## Technical Notes

Subclass `UPawnMovementComponent`. Expose `SetMovementInput()` for tests. Default stats match `Human_Starter_Fighter` when DataTable missing.
