---
id: TASK-017
title: EmptySector scene bootstrap and camera
status: complete
repo: malestrom
depends_on: [TASK-016]
---

## Goal

Spawn placeholder player ship in `EmptySector` with chase camera on Play.

## Files

- `Client/Assets/_Project/Scripts/Combat/EmptySectorFlightSetup.cs`
- `Client/Assets/_Project/Scripts/Combat/ShipCameraFollow.cs`
- `Client/Assets/Scenes/Test/EmptySector.unity`
- `Client/ProjectSettings/TagManager.asset` (Player tag)

## Acceptance

- [x] Play EmptySector → cube ship spawns, camera follows
