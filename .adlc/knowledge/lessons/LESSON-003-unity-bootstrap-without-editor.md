# LESSON-003: Unity project bootstrap without local Editor

---
id: LESSON-003
title: "Unity scaffold via YAML — validate in Hub on first open"
created: 2026-06-12
related: [REQ-051, ADR-034]
tags: [unity, bootstrap, ci]
---

## Context

REQ-051 bootstrapped `Client/` on a machine without Unity Hub/Editor installed. Project files were authored manually (ProjectSettings, URP assets, scene YAML, asmdef tests).

## Lesson

- Pin `ProjectSettings/ProjectVersion.txt` and document the exact LTS patch in README + architecture.md
- First open in Unity Hub may regenerate `Library/` and tweak ProjectSettings — commit only intentional diffs
- CI (`game-ci/unity-test-runner`) requires `UNITY_LICENSE`; local `Run-UnityTests.ps1` needs Unity on PATH via Hub or `unity-root.local.ps1`
- Keep deterministic `.meta` GUIDs for URP pipeline ↔ scene ↔ build settings references

## Follow-up

- ~~Re-platform REQ-033 flight in Unity after validating foundation in Editor~~ (done — REQ-033)
- REQ-034 flight HUD / camera polish
- Move root UE tree to `legacy/unreal/` when no longer needed for reference
