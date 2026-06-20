---
id: TASK-023
title: "Add Netcode for GameObjects + Unity Transport packages"
status: complete
parent: REQ-035
created: 2026-06-19
updated: 2026-06-19
dependencies: []
---

## Description

Add NGO and UTP to the Unity project manifest and configure Dedicated Server build target in Project Settings.

## Files to Create/Modify

- `Client/Packages/manifest.json` — add `com.unity.netcode.gameobjects` and `com.unity.transport`
- `Client/ProjectSettings/ProjectSettings.asset` — enable Dedicated Server platform in build targets

## Acceptance Criteria

- [ ] `com.unity.netcode.gameobjects` (latest stable for Unity 6) pinned in manifest.json
- [ ] `com.unity.transport` (compatible version) pinned in manifest.json
- [ ] Project compiles without errors after package addition
- [ ] Dedicated Server build target available in Build Settings

## Technical Notes

- Use version pinning (not `file:` or floating) for reproducible builds
- Unity 6 uses the Dedicated Server platform (not the old "Server Build" checkbox)
- May need to resolve any package dependency conflicts with existing Input System / URP versions
