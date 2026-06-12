---
id: TASK-004
title: "Build scripts and GitHub Actions compile workflow"
status: complete
parent: REQ-032
created: 2026-06-12
updated: 2026-06-12
dependencies: [TASK-001, TASK-003]
---

## Description

Provide reproducible compile automation via PowerShell scripts and a GitHub Actions workflow targeting a self-hosted UE5 runner, satisfying BR-3 and CI acceptance criteria.

## Files to Create/Modify

- `Scripts/Build-Editor.ps1` — requires `$env:UE_ROOT`, builds `IronExilesEditor` Win64 Development
- `.github/workflows/compile-editor.yml` — self-hosted `windows` + label `ue5`, calls Build-Editor.ps1 on push/main and workflow_dispatch
- `deploy/README.md` — one-paragraph pointer: backend Docker/K8s scaffolding starts REQ-042 (Ethos §7 placeholder)

## Acceptance Criteria

- [ ] `Scripts/Build-Editor.ps1` fails fast with clear message if `UE_ROOT` unset
- [ ] Script completes successfully on dev machine with UE 5.5 installed
- [ ] GitHub Actions workflow file validates syntactically and documents runner prerequisites in workflow comment
- [ ] CI workflow invokes `Run-FoundationTests.ps1` after successful compile (satisfies BR-3 when self-hosted runner is available)
- [ ] Workflow uses `workflow_dispatch` so missing self-hosted runner does not block contributors on push

## Technical Notes

- Follow ADR-032-4: script is source of truth; CI optional until self-hosted runner provisioned
- Do not store Epic credentials or engine binaries in repo
- Workflow should not block contributors without self-hosted runner — use `continue-on-error` or manual dispatch only if team prefers; document chosen behavior in README
