---
id: LESSON-001
title: "UE content bootstrap before foundation tests"
created: 2026-06-12
related: [REQ-032]
tags: [unreal, ci, automation-tests]
---

## Context

REQ-032 foundation tests include `IronExiles.Foundation.ProjectLoads`, which loads `/Game/Maps/Test/EmptySector`. The map is generated at dev/CI time via `Scripts/Initialize-Content.ps1` (Unreal Editor Python), not committed as a binary asset.

## Lesson

CI and local test runs must call `Initialize-Content.ps1` **before** `Run-FoundationTests.ps1`. The compile-editor workflow includes both steps in order.

## Follow-up

- Attach a self-hosted GitHub Actions runner with labels `windows`, `ue5` and `UE_ROOT` pointing at UE 5.5 for CI to pass end-to-end.
- On a fresh clone, developers run Initialize-Content once before Play or automation tests.
