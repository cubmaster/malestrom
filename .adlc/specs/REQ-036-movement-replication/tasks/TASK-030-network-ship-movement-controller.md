---
id: TASK-030
title: "NetworkShipMovementController — server sim, owner predict, reconcile"
status: complete
parent: REQ-036
created: 2026-06-20
updated: 2026-06-20
dependencies: [TASK-029]
---

## Description

Implement server-authoritative movement with owner client prediction and reconciliation threshold handling.

## Files to Create/Modify

- `Client/Assets/_Project/Scripts/Combat/NetworkShipMovementController.cs` — ServerRpc input, server FixedUpdate sim, owner Update/LateUpdate predict/reconcile
- `Client/Assets/_Project/Scripts/Combat/NetworkedShipSetup.cs` — wire network movement driver; disable local-only Update path

## Acceptance Criteria

- [ ] Server simulates all player ships from owner input only
- [ ] Owner predicts between server updates; large divergence resets/blends without runaway desync
- [ ] Remote clients do not simulate movement locally

## Technical Notes

- Default 30 Hz server tick via `FixedUpdate` and `Time.fixedDeltaTime`
- Reconcile thresholds: 2m position, 15° rotation (configurable serialized fields)
