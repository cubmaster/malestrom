---
id: TASK-034
title: "NetworkShipTargetingController and Tab input"
status: complete
parent: REQ-037
created: 2026-06-20
updated: 2026-06-20
dependencies: [TASK-033]
repo: malestrom
---

## Description

Server-authoritative lock with NetworkVariable, ServerRpc tab cycle, LOS break timer, owner Tab input.

## Files to Create/Modify

- `Client/Assets/_Project/Scripts/Combat/NetworkShipTargetingController.cs`
- `Client/Assets/_Project/Scripts/Combat/ShipTargetInputController.cs`
- `Client/Assets/_Project/Scripts/Networking/NetworkPlayerShipFactory.cs`

## Acceptance Criteria

- [ ] Tab ServerRpc cycles hostile/neutral targets in range on server
- [ ] Out-of-range lock requests rejected
- [ ] LOS blocked > 5s clears lock on server
- [ ] GetLockedTarget() returns current TargetableEntity
