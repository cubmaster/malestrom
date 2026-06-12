---
id: TASK-008
title: "Content bootstrap, flight automation tests, CI"
status: complete
parent: REQ-033
created: 2026-06-12
updated: 2026-06-12
dependencies: [TASK-007]
---

## Description

Extend content Python to create ship DataTable and player start; add `IronExiles.Flight.*` automation tests; update scripts/CI/README.

## Files to Create/Modify

- `Content/Python/create_empty_sector.py` — DataTable + PlayerStart
- `Source/IronExiles/Tests/IronExilesFlightTest.cpp`
- `Scripts/Run-FlightTests.ps1`
- `.github/workflows/compile-editor.yml` — run flight tests
- `README.md` — flight controls section

## Acceptance Criteria

- [ ] `IronExiles.Flight.SpeedClamp` passes
- [ ] `IronExiles.Flight.SectorBounds` passes
- [ ] CI runs flight tests after foundation tests

## Technical Notes

Apply LESSON-001: Initialize-Content before test runs.
