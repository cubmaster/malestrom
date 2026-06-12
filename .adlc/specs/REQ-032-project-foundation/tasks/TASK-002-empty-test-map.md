---
id: TASK-002
title: "Empty test sector map and default map configuration"
status: complete
parent: REQ-032
created: 2026-06-12
updated: 2026-06-12
dependencies: [TASK-001]
---

## Description

Create the empty space test map used by all Tier A demos and wire it as the editor + game default map with `AIronExilesGameModeBase`.

## Files to Create/Modify

- `Content/Maps/Test/EmptySector.umap` — empty level, simple skybox/light, large bounds volume optional
- `Config/DefaultEngine.ini` — set `GameDefaultMap` and `EditorStartupMap` to `/Game/Maps/Test/EmptySector`
- `Config/DefaultGame.ini` — set `GlobalDefaultGameMode` to `/Script/IronExiles.IronExilesGameModeBase`
- `Source/IronExiles/IronExilesGameModeBase.cpp` — ensure default pawn class is `DefaultPawn` or placeholder spectator pawn for REQ-032 (ship pawn is REQ-033)

## Acceptance Criteria

- [ ] Editor opens directly into `EmptySector` map
- [ ] PIE launches without errors on `EmptySector`
- [ ] GameMode is `IronExilesGameModeBase` when pressing Play
- [ ] Map path matches requirement: `/Game/Maps/Test/EmptySector`

## Technical Notes

- Use engine primitive sky sphere / directional light only — no custom art
- Player pawn can be `DefaultPawn` with fly mode or spectator until REQ-033 adds ship; document in GameMode
- Commit `.umap` via Git LFS if enabled
