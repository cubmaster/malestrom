---
id: TASK-015
title: Ship stats and movement model
status: complete
repo: malestrom
depends_on: []
---

## Goal

Port UE `FShipStatsRow` + `UShipMovementComponent` integration to Unity `ShipStatsDefinition` and testable `ShipMovementModel` (meter units).

## Files

- `Client/Assets/_Project/Scripts/Combat/ShipStatsDefinition.cs`
- `Client/Assets/_Project/Scripts/Combat/ShipMovementModel.cs`
- `Client/Assets/_Project/Scripts/Combat/IronExiles.Combat.asmdef`

## Acceptance

- [x] Human starter fighter defaults match UE ratios (meters)
- [x] Newtonian thrust, brake, rotation, speed clamp, sector bounds
