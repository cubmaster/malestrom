---
id: TASK-039
title: "HUD power sliders, presets, and wiring"
status: complete
parent: REQ-038
created: 2026-06-20
updated: 2026-06-20
dependencies: [TASK-038]
---

## Description

Add interactive power sliders and Combat/Travel/Balanced preset buttons to the flight HUD; wire to ship reactor controller on bind.

## Files to Create/Modify

- `Client/Assets/_Project/Scripts/UI/FlightHudView.cs` — sliders, presets, total label
- `Client/Assets/_Project/Scripts/UI/FlightHudController.cs` — bind power control
- `Client/Assets/_Project/Tests/EditMode/FlightHudPresenterTests.cs` — allocation display test if needed

## Acceptance Criteria

- [ ] Four sliders adjust W/S/E/ECM and maintain 100% total
- [ ] Three preset buttons apply documented splits
- [ ] Power bars reflect server/offline authoritative state
- [ ] Runnable demo: preset travel then combat changes ship speed feel

## Technical Notes

Use linked-slider redistribution via `ReactorPowerAllocationMath.AdjustChannel`. Sliders only active for local player ship.
