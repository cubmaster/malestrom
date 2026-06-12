---
id: REQ-051
title: "Unity Project Foundation & Dev Pipeline"
status: complete
deployable: true
created: 2026-06-12
updated: 2026-06-12
component: "game/foundation"
domain: "infra"
stack: ["unity", "csharp", "github-actions", "urp"]
concerns: ["developer-experience", "testability", "reliability"]
tags: ["project-scaffold", "ci", "unity-test-framework", "tier-a", "adr-034"]
---

## Description

Bootstrap the **Unity 6 LTS** client under `Client/`, establish assembly layout, test scenes, and CI so subsequent increments (flight, Netcode, combat) have a consistent home. Replaces the superseded UE5 foundation (REQ-032) per ADR-034.

**Why:** Architecture pivoted to Unity for iteration speed and tooling; Tier B+ must not build on the frozen Unreal scaffold.

**Depends on:** REQ-031 (roadmap)

**Runnable Demo:** Open `Client/` in Unity Hub, open `EmptySector` test scene, press Play — Edit Mode test `ProjectLoads` passes; scene runs without errors.

## System Model

### Entities

| Entity | Field | Type | Constraints |
|--------|-------|------|-------------|
| ProjectConfig | unity_version | string | Pinned in `ProjectSettings/ProjectVersion.txt` + README |
| ClientRoot | path | string | `Client/` |
| TestScene | path | string | `Assets/Scenes/Test/EmptySector.unity` |
| Assembly | name | string | `IronExiles.Core` minimum; optional `IronExiles.Core.Tests` |

### Events

| Event | Trigger | Payload |
|-------|---------|---------|
| ci_unity_pass | GitHub Actions Unity batchmode succeeds | `{ commit, platform }` |

### Permissions

| Action | Roles Allowed |
|--------|---------------|
| modify_unity_version | tech lead |

## Business Rules

- [ ] BR-1: Unity LTS version is pinned and documented; upgrades require a new REQ.
- [ ] BR-2: Game code lives under `Client/Assets/_Project/` with assembly definitions per `.adlc/context/conventions.md`.
- [ ] BR-3: At least one Edit Mode test runs in CI (Windows batchmode minimum).
- [ ] BR-4: URP is the default render pipeline unless ADR-034 is amended.
- [ ] BR-5: Do not modify or depend on legacy UE code at repo root for new features.

## Acceptance Criteria

- [ ] `Client/` opens in Unity Hub on a clean clone following README steps.
- [ ] `EmptySector` test scene loads in Play Mode without errors.
- [ ] Edit Mode test asserts test scene exists and core bootstrap types load.
- [ ] GitHub Actions workflow runs Unity batchmode build + tests (or documented local equivalent until runner exists).
- [ ] README documents Unity version, clone, open project, first Play.
- [ ] `.zed/tasks.json` includes Unity open/test tasks (not UE-only).
- [ ] **Runnable Demo:** Documented 3-step launch (clone → open `Client/` → Play) succeeds on a fresh machine with Unity installed.

## External Dependencies

- Unity Hub + Unity 6 LTS Editor
- .NET SDK (Unity-bundled)
- GitHub Actions `game-ci/unity-builder` or self-hosted Unity runner (optional for first merge)

## Assumptions

- Windows is primary dev platform for Phase 1; Linux headless server target added in REQ-035.
- Placeholder content uses primitives (capsule/cube ship proxy) until art REQs.
- Netcode packages are not required in REQ-051 (REQ-035 adds dedicated server bootstrap).

## Open Questions

- Resolved: pin **Unity 6000.0.32f1** (Unity 6 LTS) in `ProjectSettings/ProjectVersion.txt`.
- Resolved: CI uses **game-ci/unity-test-runner** with `UNITY_LICENSE` secret; local runs use `Scripts/Run-UnityTests.ps1`.

## Out of Scope

- 6DOF flight (REQ-033 Unity re-platform)
- Netcode / dedicated server build
- Moving `legacy/unreal/` tree (follow-up cleanup after flight re-platform)
- Backend services

## Retrieved Context

- ADR-034: `.adlc/context/architecture.md`
- LESSON-002: `.adlc/knowledge/lessons/LESSON-002-engine-pivot-unity.md`
- Superseded reference: `.adlc/specs/REQ-032-project-foundation/`
