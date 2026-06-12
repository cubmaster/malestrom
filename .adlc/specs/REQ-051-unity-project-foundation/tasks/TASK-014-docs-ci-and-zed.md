---
id: TASK-014
title: "README, CI workflow, Zed tasks, architecture pivot docs"
status: complete
parent: REQ-051
created: 2026-06-12
updated: 2026-06-12
dependencies: [TASK-012, TASK-013]
---

## Description

Finalize developer docs, Unity CI workflow, Zed tasks, and commit ADR-034 architecture pivot documentation updates.

## Files to Create/Modify

- `README.md`
- `.github/workflows/unity-ci.yml`
- `.zed/tasks.json`
- `.adlc/specs/REQ-051-unity-project-foundation/architecture.md` — pin Unity version
- `.adlc/context/conventions.md` — confirm Client/ layout note

## Acceptance Criteria

- [ ] README documents Unity 6000.0.32f1 and 3-step quick start
- [ ] `unity-ci.yml` runs editmode tests when `UNITY_LICENSE` configured
- [ ] Zed tasks open `Client/` and run Unity tests script
- [ ] Runnable demo steps documented end-to-end

## Technical Notes

Include architecture pivot files staged from prior session (ADR-034 docs, legacy/unreal README).
