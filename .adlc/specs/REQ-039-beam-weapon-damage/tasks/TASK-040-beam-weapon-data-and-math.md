---
id: TASK-040
title: "Beam weapon definition, DPS math, and weapon power multiplier"
status: complete
parent: REQ-039
created: 2026-06-25
updated: 2026-06-25
dependencies: []
repo: malestrom
---

## Description

Introduce tier-1 beam data (`ScriptableObject`), pure DPS/fire-validation math, and the weapon power multiplier hook promised in REQ-038 BR-3. Cover with Edit Mode tests before networking work lands.

## Files to Create/Modify

- `Client/Assets/_Project/Scripts/Combat/BeamWeaponDefinition.cs` — ScriptableObject with `BaseDps`, `RangeMeters`, `EnergyDrawPerSecond`
- `Client/Assets/_Project/Scripts/Combat/BeamWeaponMath.cs` — effective DPS, tick damage, `CanFireAtTarget` helper (lock id + positions + range)
- `Client/Assets/_Project/Scripts/Combat/ReactorPowerAllocationMath.cs` — add `GetWeaponPerformanceMultiplier` (linear 0–1 on weapons fraction)
- `Client/Assets/_Project/Scripts/Combat/ReactorPowerAllocationSettings.cs` — constants if needed (e.g. default tier-1 DPS baseline)
- `Client/Assets/_Project/Data/BeamWeapons/Tier1InfantryBeam.asset` — default IR-A1 prototype (50 DPS, 2500m range)
- `Client/Assets/_Project/Tests/EditMode/BeamWeaponMathTests.cs` — DPS scaling, tick damage, fire validation, 100% vs 50% weapons ≥2:1 ratio

## Acceptance Criteria

- [ ] `BeamWeaponDefinition` asset loads with 50 base DPS and 2500m range
- [ ] `GetWeaponPerformanceMultiplier(0)` returns 0; `(1)` returns 1
- [ ] Effective DPS at 100% weapons ≥ 2× effective DPS at 50% weapons (same base definition)
- [ ] `BeamWeaponMath.ComputeTickDamage` returns `effectiveDps × deltaTime`
- [ ] `CanFireAtTarget` returns false when out of range or lock id is zero
- [ ] Edit Mode tests pass locally

## Technical Notes

Follow `TargetSelectionMath` static pure-function style. Weapon multiplier is linear (not engine's 0.5 floor). Default hull/max values for damageable work land in TASK-041; this task only defines math inputs.
