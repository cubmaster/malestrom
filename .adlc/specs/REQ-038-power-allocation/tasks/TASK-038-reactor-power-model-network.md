---
id: TASK-038
title: "Reactor power model, network authority, and movement hook"
status: complete
parent: REQ-038
created: 2026-06-20
updated: 2026-06-20
dependencies: []
---

## Description

Pure allocation math, offline + networked controllers, and engine performance scaling in `ShipMovementModel`.

## Files to Create/Modify

- `Client/Assets/_Project/Scripts/Combat/ReactorPowerAllocationMath.cs` — validation, presets, engine multiplier
- `Client/Assets/_Project/Scripts/Combat/IShipReactorPowerControl.cs` — control interface
- `Client/Assets/_Project/Scripts/Combat/ShipReactorPowerController.cs` — offline authority
- `Client/Assets/_Project/Scripts/Combat/NetworkShipReactorPowerController.cs` — NGO replication
- `Client/Assets/_Project/Scripts/Combat/ShipMovementModel.cs` — engine multiplier
- `Client/Assets/_Project/Scripts/Combat/NetworkShipMovementController.cs` — sync multiplier to prediction
- `Client/Assets/_Project/Scripts/Combat/ShipFlightTelemetryAdapter.cs` — read live allocation
- `Client/Assets/_Project/Scripts/Networking/NetworkPlayerShipFactory.cs` — register component
- `Client/Assets/_Project/Scripts/Combat/EmptySectorFlightSetup.cs` — offline component
- `Client/Assets/_Project/Tests/EditMode/ReactorPowerAllocationTests.cs` — validation + speed test

## Acceptance Criteria

- [ ] Invalid allocations (sum != 1) rejected by server
- [ ] Engine multiplier documented as 0.5–1.0 linear scale
- [ ] Edit Mode test: 100% engines max speed > 100% weapons max speed
- [ ] Offline EmptySector ship has working allocation controller

## Technical Notes

Follow `NetworkShipTargetingController` ServerRpc + NetworkVariable pattern. ECM maps to spec AI slot.
