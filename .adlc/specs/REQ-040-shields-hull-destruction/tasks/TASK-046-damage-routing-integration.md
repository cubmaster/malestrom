---
id: TASK-046
title: "Integrate shields into beam weapon damage path"
status: draft
parent: REQ-040
created: 2026-06-28
updated: 2026-06-28
dependencies: [TASK-044, TASK-045]
---

## Description

Modify the beam weapon controller to route damage through the target's shield controller before applying hull damage. Add the shield component to the ship prefab factory and configure it during network spawn.

## Files to Create/Modify

- `Client/Assets/_Project/Scripts/Combat/NetworkShipBeamWeaponController.cs` — in server-side `Update()` damage application: get target's `NetworkShipShieldController`, compute attack direction, call `ApplyDirectionalDamage()`, apply only overflow to `NetworkDamageableHealth.ApplyDamage()`
- `Client/Assets/_Project/Scripts/Combat/NetworkedShipSetup.cs` — in `ConfigureTargetableAffiliation()`: configure shield controller with default max shield via `ConfigureForServer()`
- `Client/Assets/_Project/Scripts/Networking/NetworkPlayerShipFactory.cs` — add `ship.AddComponent<NetworkShipShieldController>()` to prefab creation
- `Client/Assets/_Project/Scripts/Combat/EmptySectorFlightSetup.cs` — add `ship.AddComponent<NetworkShipShieldController>()` for single-player path consistency
- `Client/Assets/_Project/Tests/EditMode/BeamWeaponControllerLogicTests.cs` — add test verifying damage routes through shields before hull

## Acceptance Criteria

- [ ] Beam damage first hits the shield facing aligned with the attack direction
- [ ] Shield absorbs up to its remaining HP; overflow damages hull
- [ ] When shields are fully depleted on a facing, all damage goes to hull on that facing
- [ ] Other facings remain at full shield even when one is depleted
- [ ] Shield regen kicks in 3s after last damage on a given facing
- [ ] Single-player (EmptySectorFlightSetup) and multiplayer (NetworkPlayerShipFactory) both create ships with shields
- [ ] Existing beam weapon tests still pass (no regression)

## Technical Notes

- Attack direction: `(attacker.transform.position - target.transform.position).normalized` → transform to target's local space via `target.transform.InverseTransformDirection()`
- Guard: if target has no `NetworkShipShieldController`, all damage goes to hull (backwards compatible)
- The beam controller already has `_targeting.GetLockedTarget()` → target transform available
- Must handle case where target is destroyed mid-beam (existing `IsDestroyed` check)
- `Projectile.cs` also deals damage — add shield routing there too for rail/missile weapons
