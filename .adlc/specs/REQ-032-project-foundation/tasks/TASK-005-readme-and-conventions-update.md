---
id: TASK-005
title: "Developer README and conventions update"
status: complete
parent: REQ-032
created: 2026-06-12
updated: 2026-06-12
dependencies: [TASK-003, TASK-004]
---

## Description

Document the 3-step Runnable Demo and update project conventions to reflect the actual UE5 directory layout (BR-4).

## Files to Create/Modify

- `README.md` — engine version pin, prerequisites (VS2022, UE 5.5), clone, generate project files, first PIE, run Foundation tests, run build script
- `.adlc/context/conventions.md` — replace `(future) Source/` tree with actual layout from architecture.md
- `.adlc/specs/REQ-032-project-foundation/requirement.md` — resolve Open Questions section (greenfield + UE 5.5 decided)

## Acceptance Criteria

- [ ] README documents exact 3-step launch: clone → generate/open project → Play in EmptySector
- [ ] README documents `UE_ROOT`, `Build-Editor.ps1`, and `Run-FoundationTests.ps1`
- [ ] `.adlc/context/conventions.md` file organization matches repo after TASK-001–004
- [ ] **Runnable Demo** steps in README verified by following them on a clean clone (manual sign-off)
- [ ] REQ-032 open question marked resolved in requirement or architecture ADRs referenced

## Technical Notes

- Resolve open question: **Greenfield, UE 5.5** — do not fork Lyra
- Note Windows-primary; Linux server target deferred to REQ-035
- Link to `docs/05-architecture.md` for long-term MMO architecture context
