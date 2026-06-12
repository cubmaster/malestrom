---
id: TASK-001
title: "Create UE5 project and IronExiles runtime module"
status: complete
parent: REQ-032
created: 2026-06-12
updated: 2026-06-12
dependencies: []
---

## Description

Create the greenfield Unreal Engine 5 C++ project skeleton: `.uproject`, runtime module, target files, base `Config/`, and Git LFS attributes for future binary assets.

## Files to Create/Modify

- `IronExiles.uproject` — C++ project, engine association 5.5, module list
- `Source/IronExiles/IronExiles.Build.cs` — Core, Engine, InputCore, AutomationTest dependencies
- `Source/IronExiles/IronExiles.h` / `IronExiles.cpp` — primary game module impl
- `Source/IronExiles/IronExilesGameModeBase.h` / `.cpp` — minimal GameMode stub
- `Source/IronExiles.Target.cs` — game target
- `Source/IronExilesEditor.Target.cs` — editor target
- `Config/DefaultEngine.ini` — baseline engine settings, map placeholders
- `Config/DefaultGame.ini` — project id, company name, copyright
- `.gitattributes` — LFS tracking for `*.uasset`, `*.umap`, `*.ubulk`, etc.
- `.gitignore` — append UE build artifacts (`Binaries/`, `Intermediate/`, `Saved/`, `DerivedDataCache/`, `.vs/`)

## Acceptance Criteria

- [ ] Project generates Visual Studio solution via right-click `.uproject` or `GenerateProjectFiles.bat`
- [ ] `IronExilesEditor` Development target compiles without errors on Windows
- [ ] Unreal Editor opens the project and loads an empty default level without crash
- [ ] `AIronExilesGameModeBase` is registered and selectable as project GameMode

## Technical Notes

- Follow ADR-032-1 (greenfield) and ADR-032-2 (UE 5.5) from `architecture.md`
- Do not add Lyra, OnlineSubsystem, or GAS plugins yet
- Module naming must match `.adlc/context/conventions.md` UE conventions (`F`/`U`/`E` prefixes on types)
