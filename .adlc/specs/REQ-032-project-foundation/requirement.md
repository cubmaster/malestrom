---
id: REQ-032
title: "UE5 Project Foundation & Dev Pipeline"
status: superseded
deployable: true
created: 2026-06-11
updated: 2026-06-12
component: "game/foundation"
domain: "infra"
stack: ["unreal", "cpp", "github-actions"]
concerns: ["developer-experience", "testability", "reliability"]
tags: ["project-scaffold", "ci", "automation-tests", "tier-a", "legacy"]
---

## Description

> **Superseded by ADR-034 (Unity).** Do not extend. Replacement: **REQ-051** Unity project foundation. UE artifacts remain at repo root until moved to `legacy/unreal/`.

Establish the Unreal Engine 5 project skeleton, module layout, and developer workflow so every subsequent increment has a consistent home. This is the first runnable increment: launching the editor and running an empty automation test pass.

**Why:** Without a pinned engine version, module boundaries, and CI smoke checks, later networking and combat work cannot be verified reproducibly.

**Depends on:** REQ-031 (roadmap only — no code dependency)

**Runnable Demo:** Open the UE5 project, press Play in an empty test map, run `Run Automation Tests` filter `IronExiles.Foundation` — all pass.

## System Model

### Entities

| Entity | Field | Type | Constraints |
|--------|-------|------|-------------|
| ProjectConfig | engine_version | string | Pinned semver, documented in README |
| GameModule | name | string | `IronExiles` primary runtime module |
| TestMap | path | string | `/Game/Maps/Test/EmptySector` |

### Events

| Event | Trigger | Payload |
|-------|---------|---------|
| ci_build_pass | GitHub Actions compile succeeds | `{ commit, platform }` |

### Permissions

| Action | Roles Allowed |
|--------|---------------|
| modify_engine_version | tech lead |

## Business Rules

- [ ] BR-1: Engine version is pinned and committed; upgrades require a new REQ.
- [ ] BR-2: Source lives under `Source/IronExiles/` with C++ module `IronExiles` and optional editor module.
- [ ] BR-3: At least one Automation Test exists and runs in CI (Windows build minimum).
- [ ] BR-4: `.adlc/context/conventions.md` file organization is updated when actual directories differ from planned layout.

## Acceptance Criteria

- [ ] UE5 project opens and compiles on a clean clone following documented setup steps.
- [ ] Empty test map loads in PIE without errors.
- [ ] Automation test `IronExiles.Foundation.ProjectLoads` passes (asserts world spawns, game mode valid).
- [ ] GitHub Actions (or documented local script) compiles Development Editor target.
- [ ] README section documents: engine version, clone, generate project files, first run.
- [ ] **Runnable Demo:** Documented 3-step launch (clone → generate → Play) succeeds on a fresh machine.

## External Dependencies

- Unreal Engine 5 (pinned version)
- Visual Studio 2022 / Build tools
- Git LFS (if binary assets added later)

## Assumptions

- Windows is the primary dev platform for Phase 1; Linux dedicated server target added in REQ-035.
- Placeholder content uses engine primitives (no custom ship meshes yet).

## Open Questions

- Resolved: **Greenfield C++ project, UE 5.5** — see `architecture.md` ADR-032-1, ADR-032-2.

## Out of Scope

- Dedicated server build target (REQ-035)
- Custom ship meshes or VFX
- Backend services

## Retrieved Context

No prior context retrieved — no tagged documents matched this area.
