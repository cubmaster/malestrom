---
id: TASK-019
title: Chase camera rig with collision pull-in
status: complete
repo: malestrom
depends_on: []
---

## Goal

Replace basic `ShipCameraFollow` with collision-aware `ChaseCameraRig` and testable placement math.

## Files

- `Client/Assets/_Project/Scripts/Combat/ChaseCameraRig.cs`
- `Client/Assets/_Project/Scripts/Combat/ChaseCameraPlacement.cs`
- Remove `ShipCameraFollow.cs`

## Acceptance

- [x] Smooth lag follow
- [x] SphereCast pull-in when obstructed
- [x] Edit Mode tests for placement math
