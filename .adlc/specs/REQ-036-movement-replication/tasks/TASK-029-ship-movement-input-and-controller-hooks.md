---
id: TASK-029
title: "ShipMovementInput payload and movement controller network hooks"
status: draft
parent: REQ-036
created: 2026-06-20
updated: 2026-06-20
dependencies: []
---

## Description

Add serializable movement input struct and refactor `ShipMovementController` so networked code can drive simulation ticks without the local-only Update loop.

## Files to Create/Modify

- `Client/Assets/_Project/Scripts/Combat/ShipMovementInput.cs` — `INetworkSerializable` thrust/rotation/brake payload
- `Client/Assets/_Project/Scripts/Combat/ShipMovementController.cs` — expose `SimulateInput(...)`, disable Update when `NetworkShipMovementController` present

## Acceptance Criteria

- [ ] Input struct round-trips through NGO serialization helpers
- [ ] Movement controller can simulate from explicit input without reading keyboard
- [ ] Local single-player EmptySector still works when no network driver attached

## Technical Notes

- Keep `ShipMovementModel` pure; controller remains thin wrapper
