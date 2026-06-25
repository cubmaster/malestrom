---
id: REQ-039
title: "Beam Weapon & Server-Validated Damage"
status: complete
deployable: true
created: 2026-06-11
updated: 2026-06-25
component: "game/combat/WeaponSystem"
domain: "combat"
stack: ["unity", "csharp", "netcode"]
concerns: ["security", "performance", "testability"]
tags: ["beam-weapon", "dps", "server-authoritative", "tier-c"]
repo: malestrom
---

## Description

Implement the first weapon type: a continuous-fire energy beam that applies sustained DPS to a locked target. Owner client sends fire input via Unity Netcode for GameObjects; dedicated server validates range, lock, and weapon power allocation; damage is authoritative and replicated.

**Why:** Beams are the simplest hitscan continuous weapon and establish the damage pipeline for all other weapon types.

**Depends on:** REQ-037 (targeting), REQ-038 (power allocation), REQ-036 (networked ships)

**Runnable Demo:** Two clients or client vs NPC dummy — hold primary fire on locked target, observe hull bar drop on target (REQ-040 may run in parallel; this REQ ships a hull-only damage stub).

Reference: `Client/Assets/_Project/Scripts/Combat/` (`NetworkShipTargetingController`, `NetworkShipReactorPowerController`, `TargetableEntity`); `docs/09-realtime-combat.md` continuous fire; `docs/04-space-combat.md` energy beams; `docs/07-assets.md` IR-A1 tier-1 baseline.

## System Model

### Entities

| Entity | Field | Type | Constraints |
|--------|-------|------|-------------|
| BeamWeaponDefinition | base_dps | float | ScriptableObject row; modified by weapon power allocation |
| BeamWeaponDefinition | range_meters | float | max lock/fire range (default aligns with targeting lock range) |
| BeamWeaponDefinition | energy_draw | float | per second while firing (telemetry only in this REQ) |
| DamageableHealth | current_hull | float | server-authoritative; replicated for HUD |
| DamageableHealth | max_hull | float | prototype default on player ships and NPC dummies |
| DamageApplication | damage_type | enum | energy |
| DamageApplication | dps | float | server computed per tick |

### Events

| Event | Trigger | Payload |
|-------|---------|---------|
| weapon_fired | Server accepts fire input | `{ instigator_network_object_id, target_network_object_id, weapon_id }` |
| damage_applied | Server tick while firing | `{ target_network_object_id, amount, type, source_network_object_id }` |

### Permissions

| Action | Roles Allowed |
|--------|---------------|
| request_fire | ship owner (ServerRpc) |
| apply_damage | server only |

## Business Rules

- [ ] BR-1: Beam requires valid target lock within range; no damage without lock.
- [ ] BR-2: Damage is applied per server tick as `effective_dps × deltaTime`, not single burst.
- [ ] BR-3: Client-side beam VFX may play optimistically; HP changes only from server `NetworkVariable` replication.
- [ ] BR-4: Weapon power allocation scales effective DPS proportionally via `ReactorPowerAllocationMath` (0% weapons → 0 DPS, 100% weapons → full base DPS).
- [ ] BR-5: Beams are instant hitscan; no projectile replication in this REQ.
- [ ] BR-6: Fire input uses Unity Input System on owner client; server validates lock/range each tick while fire is held.

## Acceptance Criteria

- [ ] Primary fire (mouse button / bound action) activates beam while locked and in range.
- [ ] Targets expose a minimal server-authoritative hull pool via `NetworkDamageableHealth` (or equivalent); shields deferred to REQ-040.
- [ ] Target hull decreases at expected rate (automation: 10s fire at 100% weapons → HP drop within tolerance).
- [ ] Out-of-range or broken lock stops damage within 1 server tick.
- [ ] Second client sees beam VFX and target hull replication on locked ship / dummy.
- [ ] Edit Mode test: DPS math, lock/range validation helpers, weapon power multiplier.
- [ ] Measurable power effect: 100% weapons vs 50% weapons changes beam DPS by ≥2:1 ratio at same lock.
- [ ] **Runnable Demo:** Client A fires on Client B or NPC dummy until hull reaches 0 (death handler optional stub).

## External Dependencies

- REQ-037 targeting (`NetworkShipTargetingController`, `TargetableEntity`)
- REQ-038 power allocation (`NetworkShipReactorPowerController`, weapon DPS multiplier hook)
- REQ-036 movement replication (networked player ships)
- Minimum damageable component on targets (full shields in REQ-040)

## Assumptions

- Tier-1 beam stats (`IR-A1` equivalent, ~50 sustained DPS prototype baseline) defined as a single `ScriptableObject` asset under `Client/Assets/_Project/Data/`.
- Heat/overheat mechanics deferred.
- Beam VFX is a simple LineRenderer or lightweight VFX Graph stub; polish deferred.
- Primary fire binds to existing combat input map pattern (extend `ShipTargetInputController` or sibling weapon input component).

## Open Questions

- None (minimal hull-only health pool is in scope for this REQ; REQ-040 replaces stub with shields + full damage model).

## Out of Scope

- Kinetic, missile, plasma weapon types
- Point defense
- Subsystem damage
- Lag compensation / rewind for hitscan (deferred; server validates current lock state only)

## Retrieved Context

- ADR-034/036-1/037-1/038-1: Unity + NGO server authority; targeting and power patterns established
- REQ-038 BR-3: weapon allocation DPS cap lands in this REQ
- REQ-037: `TargetableEntity.HullPercent` is display-only today — replace with replicated server health for combat
- Mapped from UE WeaponSystem spec to Unity NGO + `IronExiles.Combat` assembly
