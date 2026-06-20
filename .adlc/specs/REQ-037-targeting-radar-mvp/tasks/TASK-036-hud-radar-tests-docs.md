---
id: TASK-036
title: "HUD lock panel, radar blips, tests, docs"
status: complete
parent: REQ-037
created: 2026-06-20
updated: 2026-06-20
dependencies: [TASK-034, TASK-035]
repo: malestrom
---

## Description

Extend flight HUD for locked target panel and radar blips; telemetry bridge; docs and Edit Mode tests.

## Files to Create/Modify

- `Client/Assets/_Project/Scripts/UI/FlightHudView.cs`
- `Client/Assets/_Project/Scripts/UI/FlightHudPresenter.cs`
- `Client/Assets/_Project/Scripts/Combat/ShipFlightTelemetryAdapter.cs`
- `Client/Assets/_Project/Tests/EditMode/TargetSelectionTests.cs`
- `docs/targeting-radar.md`
- `.adlc/context/architecture.md`

## Acceptance Criteria

- [ ] HUD shows locked target name, distance, hull bar
- [ ] Radar panel renders blips for in-range contacts
- [ ] Edit Mode tests pass for selection math
- [ ] docs/targeting-radar.md documents tab sort and sensor defaults
