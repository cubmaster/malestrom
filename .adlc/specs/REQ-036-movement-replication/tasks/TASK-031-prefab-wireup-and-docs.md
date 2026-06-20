---
id: TASK-031
title: "Wire networked ship prefab and movement replication docs"
status: complete
parent: REQ-036
created: 2026-06-20
updated: 2026-06-20
dependencies: [TASK-030]
---

## Description

Attach movement replication to the runtime player ship prefab and document tick/relevancy defaults.

## Files to Create/Modify

- `Client/Assets/_Project/Scripts/Networking/NetworkPlayerShipFactory.cs` — add/configure `NetworkShipMovementController` + `NetworkTransform`
- `docs/movement-replication.md` — 30 Hz tick, relevancy note, local two-client test steps

## Acceptance Criteria

- [ ] Spawned networked ships include movement replication components
- [ ] Documentation describes server tick and prototype relevancy (no culling)

## Technical Notes

- Ensure `NetworkTransform` uses server authority (default for server-spawned objects)
