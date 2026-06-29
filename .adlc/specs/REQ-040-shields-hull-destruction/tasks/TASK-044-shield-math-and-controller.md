---
id: TASK-044
title: "Shield math, settings, and NetworkShipShieldController"
status: draft
parent: REQ-040
created: 2026-06-28
updated: 2026-06-28
dependencies: []
---

## Description

Create the core directional shield system: a static `ShieldMath` utility, a `ShieldSettings` constants class, and the `NetworkShipShieldController` NetworkBehaviour that manages 4 shield facings with server-authoritative HP replication and power-scaled regeneration.

## Files to Create/Modify

- `Client/Assets/_Project/Scripts/Combat/ShieldMath.cs` — static utility: `DetermineFacing(Vector3 localDirection)`, `ComputeAbsorption(float shieldHp, float damage)`, `ComputeRegenPerTick(float baseRate, float shieldPowerFraction, float deltaTime)`, `GetPowerMultiplier(float shieldPowerFraction)`
- `Client/Assets/_Project/Scripts/Combat/ShieldSettings.cs` — constants: `DefaultMaxShieldPerFacing=250f`, `DefaultRegenRatePerSecond=25f`, `RegenCooldownSeconds=3f`, `DefaultPowerMultiplierMax=2f`
- `Client/Assets/_Project/Scripts/Combat/ShieldNetworkState.cs` — `INetworkSerializable` struct with Front/Rear/Port/Starboard floats
- `Client/Assets/_Project/Scripts/Combat/NetworkShipShieldController.cs` — NetworkBehaviour: `NetworkVariable<ShieldNetworkState>`, `ApplyDirectionalDamage(Vector3 worldAttackDirection, float damage)` returns overflow, server-side `Update()` for regen with cooldown tracking, reads `IShipReactorPowerControl` for scaling
- `Client/Assets/_Project/Tests/EditMode/ShieldMathTests.cs` — unit tests for all ShieldMath functions

## Acceptance Criteria

- [ ] `ShieldMath.DetermineFacing()` correctly maps attack vectors to Front/Rear/Port/Starboard using dot products against local forward/right
- [ ] `ShieldMath.ComputeAbsorption()` returns (absorbed, overflow) correctly when shield > damage, shield < damage, and shield == 0
- [ ] `ShieldMath.ComputeRegenPerTick()` scales by power fraction (0 power = 0.5x base, full power = 2x base)
- [ ] `NetworkShipShieldController.ApplyDirectionalDamage()` reduces correct facing, returns overflow to caller
- [ ] Shield regen only applies to facings not damaged within `RegenCooldownSeconds`
- [ ] `ShieldNetworkState` serializes/deserializes all 4 fields correctly
- [ ] All ShieldMath tests pass in Edit Mode

## Technical Notes

- Follow `BeamWeaponMath` pattern: static methods, no Unity objects in math class
- Follow `ReactorPowerNetworkState` pattern for the serializable struct
- `NetworkShipShieldController` requires `NetworkObject` on same GameObject
- Server-only writes: `NetworkVariableWritePermission.Server`
- Track `_lastDamageTime` per facing as plain float array (server only, no replication needed)
- Regen cooldown: `Time.time - _lastDamageTime[facing] >= ShieldSettings.RegenCooldownSeconds`
- Power multiplier: `Mathf.Lerp(0.5f, ShieldSettings.DefaultPowerMultiplierMax, shieldFraction)`
