---
id: TASK-035
title: "NPC dummy spawner and prefab wireup"
status: complete
parent: REQ-037
created: 2026-06-20
updated: 2026-06-20
dependencies: [TASK-033]
repo: malestrom
---

## Description

Server-spawned static NPC dummy and wire targeting components on player prefab.

## Files to Create/Modify

- `Client/Assets/_Project/Scripts/Networking/TargetDummySpawner.cs`
- `Client/Assets/_Project/Scripts/Networking/EmptySectorMultiplayerBootstrap.cs`
- `Client/Assets/_Project/Scripts/Combat/NetworkedShipSetup.cs`

## Acceptance Criteria

- [ ] NPC dummy spawns on dedicated server in EmptySector
- [ ] Player ships have TargetableEntity + targeting controller
