---
id: TASK-009
title: "Create Unity Client/ project scaffold (URP, packages, gitignore)"
status: draft
parent: REQ-051
created: 2026-06-12
updated: 2026-06-12
dependencies: []
---

## Description

Bootstrap `Client/` with Unity 6 LTS project settings, URP packages, render pipeline assets, and repo gitignore entries for Unity artifacts.

## Files to Create/Modify

- `Client/ProjectSettings/ProjectVersion.txt` — pin 6000.0.32f1
- `Client/Packages/manifest.json` — URP, Input System, Test Framework
- `Client/ProjectSettings/*.asset` — baseline editor/player settings
- `Client/Assets/Settings/UniversalRP.asset` — URP pipeline asset
- `Client/Assets/Settings/UniversalRenderer.asset` — forward renderer
- `.gitignore` — Unity Library/Temp/Logs exclusions under Client/

## Acceptance Criteria

- [ ] `Client/ProjectSettings/ProjectVersion.txt` pins Unity 6000.0.32f1
- [ ] `manifest.json` includes URP and com.unity.test-framework
- [ ] Graphics settings reference the URP asset
- [ ] `.gitignore` excludes `Client/Library/` and related generated folders

## Technical Notes

Follow ADR-034 and `.adlc/context/conventions.md`. Use deterministic GUIDs in `.meta` files for pipeline/scene references.
