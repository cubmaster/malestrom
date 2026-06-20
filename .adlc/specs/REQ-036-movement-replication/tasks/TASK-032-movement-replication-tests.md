---
id: TASK-032
title: "Movement replication Edit Mode tests"
status: draft
parent: REQ-036
created: 2026-06-20
updated: 2026-06-20
dependencies: [TASK-029]
---

## Description

Add Edit Mode coverage for input packing and reconciliation threshold helper logic.

## Files to Create/Modify

- `Client/Assets/_Project/Scripts/Combat/ShipMovementReplicationMath.cs` — static reconcile helpers (testable)
- `Client/Assets/_Project/Tests/EditMode/ShipMovementReplicationTests.cs` — threshold + input clamp tests

## Acceptance Criteria

- [ ] Tests pass in Unity Edit Mode test runner
- [ ] Reconciliation helper returns snap vs blend as specified

## Technical Notes

- Keep math in pure static class to avoid Play Mode / NGO dependency in tests
