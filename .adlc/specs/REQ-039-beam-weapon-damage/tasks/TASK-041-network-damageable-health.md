---
id: TASK-041
title: "NetworkDamageableHealth and TargetableEntity hull bridge"
status: complete
parent: REQ-039
created: 2026-06-25
updated: 2026-06-25
dependencies: []
repo: malestrom
---

## Description

Add server-authoritative replicated hull HP on networked entities. Bridge `TargetableEntity.HullPercent` to replicated state so REQ-037 lock panel shows live damage. Wire onto player ship and training dummy prefabs.

## Files to Create/Modify

- `Client/Assets/_Project/Scripts/Combat/NetworkDamageableHealth.cs` — `NetworkVariable<float>` current/max hull; server-only `ApplyDamage`; optional `IsDestroyed` when hull ≤ 0
- `Client/Assets/_Project/Scripts/Combat/TargetableEntity.cs` — read hull from `NetworkDamageableHealth` when present; keep serialized fallback for offline tests
- `Client/Assets/_Project/Scripts/Networking/NetworkPlayerShipFactory.cs` — add `NetworkDamageableHealth` (1000 max hull default)
- `Client/Assets/_Project/Scripts/Networking/TargetDummySpawner.cs` — add `NetworkDamageableHealth` on dummy prefab in `TargetDummyFactory`
- `Client/Assets/_Project/Tests/EditMode/NetworkDamageableHealthTests.cs` — pure damage clamp/destruction helper tests (no NGO spawn required)

## Acceptance Criteria

- [ ] Server `ApplyDamage` reduces current hull and clamps at zero
- [ ] `TargetableEntity.HullPercent` reflects replicated hull on spawned entities
- [ ] Player ship prefab includes `NetworkDamageableHealth`
- [ ] Training dummy prefab includes `NetworkDamageableHealth`
- [ ] Edit Mode tests cover damage clamp and percent conversion

## Technical Notes

Use `NetworkVariable<float>` with server write permission (same pattern as reactor allocation). Death/despawn is optional stub — log or disable collider at 0 HP; full destruction in REQ-040. Default max hull 1000 gives ~20s TTK at 50 DPS (100% weapons).
