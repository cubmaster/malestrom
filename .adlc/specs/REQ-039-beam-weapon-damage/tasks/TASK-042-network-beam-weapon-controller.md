---
id: TASK-042
title: "NetworkShipBeamWeaponController and primary fire input"
status: complete
parent: REQ-039
created: 2026-06-25
updated: 2026-06-25
dependencies: [TASK-040, TASK-041]
repo: malestrom
---

## Description

Server-authoritative continuous beam firing: owner holds primary fire, server validates lock/range/power each tick, applies damage to target's `NetworkDamageableHealth`. Replicate firing state for VFX and HUD.

## Files to Create/Modify

- `Client/Assets/_Project/Scripts/Combat/NetworkShipBeamWeaponController.cs` — `SetFiringServerRpc`, server `Update` damage loop, `NetworkVariable<bool> IsFiring`, references targeting + reactor + beam definition
- `Client/Assets/_Project/Scripts/Combat/ShipBeamWeaponInputController.cs` — owner mouse button / fire action → ServerRpc (Input System, mirror `ShipTargetInputController`)
- `Client/Assets/_Project/Scripts/Networking/NetworkPlayerShipFactory.cs` — register beam controller + input on player prefab
- `Client/Assets/_Project/Tests/EditMode/BeamWeaponControllerLogicTests.cs` — server tick damage integration using test doubles or extracted static helper if needed

## Acceptance Criteria

- [ ] Primary fire ServerRpc rejected when sender is not owner
- [ ] No damage applied without valid lock within beam range
- [ ] Damage stops within one server tick when lock breaks or target out of range
- [ ] Effective DPS scales with current weapon power allocation (REQ-038 controller)
- [ ] `IsFiring` replicates to non-owner clients
- [ ] Edit Mode or extracted logic tests pass for tick accumulation over 10 simulated seconds

## Technical Notes

Require `NetworkShipTargetingController`, `NetworkShipReactorPowerController`, and serialized `BeamWeaponDefinition` reference. Server-only damage path — never call `ApplyDamage` from client. Resolve target via `GetLockedTarget()` then `GetComponent<NetworkDamageableHealth>()`. Follow ADR-039-1.
