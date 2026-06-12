---
id: TASK-007
title: "AShipPawn, input bindings, and GameMode wiring"
status: complete
parent: REQ-033
created: 2026-06-12
updated: 2026-06-12
dependencies: [TASK-006]
---

## Description

Create `AShipPawn` with placeholder mesh/collision, wire keyboard+mouse input, update GameMode to spawn ship pawn, add `DefaultInput.ini` axis mappings.

## Files to Create/Modify

- `Source/IronExiles/Public/Ship/ShipPawn.h` / `Private/Ship/ShipPawn.cpp`
- `Source/IronExiles/Private/IronExilesGameModeBase.cpp` — DefaultPawnClass, bounds
- `Config/DefaultInput.ini` — WASD + mouse + roll + brake bindings

## Acceptance Criteria

- [ ] PIE possesses ship pawn with visible placeholder hull
- [ ] Forward/reverse, strafe, vertical, pitch/yaw/roll, brake all bound
- [ ] GameMode loads stats row from DataTable path when present

## Technical Notes

Use Engine basic cube mesh. Document bindings in README.
