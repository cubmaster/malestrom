---
id: TASK-011
title: "EmptySector test scene and build settings"
status: draft
parent: REQ-051
created: 2026-06-12
updated: 2026-06-12
dependencies: [TASK-009]
---

## Description

Create the Tier A test scene with camera, light, and bootstrap object; register it in Editor build settings.

## Files to Create/Modify

- `Client/Assets/Scenes/Test/EmptySector.unity`
- `Client/ProjectSettings/EditorBuildSettings.asset`

## Acceptance Criteria

- [ ] Scene exists at `Assets/Scenes/Test/EmptySector.unity`
- [ ] Scene contains Main Camera and Directional Light
- [ ] Scene is index 0 in Editor build settings

## Technical Notes

Placeholder art only (no meshes required). Scene should load in Play Mode without custom scripts beyond optional bootstrap marker.
