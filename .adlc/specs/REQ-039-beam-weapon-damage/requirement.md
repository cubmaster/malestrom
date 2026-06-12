---
id: REQ-039
title: "Beam Weapon & Server-Validated Damage"
status: draft
deployable: true
created: 2026-06-11
updated: 2026-06-11
component: "game/combat/WeaponSystem"
domain: "combat"
stack: ["unreal", "cpp"]
concerns: ["security", "performance", "testability"]
tags: ["beam-weapon", "dps", "server-authoritative", "tier-c"]
---

## Description

Implement the first weapon type: a continuous-fire energy beam that applies sustained DPS to a locked target. Client initiates fire; server validates range, lock, and power; damage is authoritative.

**Why:** Beams are the simplest hitscan continuous weapon and establish the damage pipeline for all other weapon types.

**Depends on:** REQ-037, REQ-038

**Runnable Demo:** Two clients or client vs NPC — hold fire on locked target, observe shield/hull bars drop on target (REQ-040 may be parallel; minimum raw HP pool required).

Reference: `docs/09-realtime-combat.md` continuous fire; `docs/04-space-combat.md` energy beams; `docs/07-assets.md` beam tier 1 stats as baseline.

## System Model

### Entities

| Entity | Field | Type | Constraints |
|--------|-------|------|-------------|
| BeamWeapon | base_dps | float | from DataTable, modified by power allocation |
| BeamWeapon | range | float | max lock/fire range |
| BeamWeapon | energy_draw | float | per second while firing |
| DamageApplication | damage_type | enum | energy |
| DamageApplication | dps | float | server computed per tick |

### Events

| Event | Trigger | Payload |
|-------|---------|---------|
| weapon_fired | Server accepts fire input | `{ instigator, target, weapon_id }` |
| damage_applied | Server tick while firing | `{ target, amount, type, source }` |

### Permissions

| Action | Roles Allowed |
|--------|---------------|
| request_fire | ship owner |
| apply_damage | server only |

## Business Rules

- [ ] BR-1: Beam requires valid target lock within range; no damage without lock.
- [ ] BR-2: Damage is applied per server tick as DPS × deltaTime, not single burst.
- [ ] BR-3: Client-side VFX may play optimistically; HP changes only from server replication.
- [ ] BR-4: Firing without sufficient weapon power allocation reduces DPS proportionally (REQ-038).
- [ ] BR-5: Beams are instant hitscan; no projectile replication in this REQ.

## Acceptance Criteria

- [ ] Primary fire activates beam while locked and in range.
- [ ] Targets expose a minimal server-authoritative health pool (hull-only stub acceptable before REQ-040 shields).
- [ ] Target HP decreases at expected rate (automation: 10s fire → HP drop within tolerance).
- [ ] Out-of-range or broken lock stops damage within 1 server tick.
- [ ] Second client sees beam VFX and target HP replication.
- [ ] **Runnable Demo:** Client A fires on Client B or NPC until hull reaches 0 (death handler optional stub).

## External Dependencies

- REQ-037 targeting
- REQ-038 power allocation
- Minimum damageable component on targets (full shields in REQ-040)

## Assumptions

- Tier-1 beam stats (`IR-A1` equivalent) used as sole weapon DataTable row.
- Heat/overheat mechanics deferred.

## Open Questions

- None (minimal hull-only health pool is in scope for this REQ; REQ-040 replaces stub with shields + full damage model).

## Out of Scope

- Kinetic, missile, plasma weapon types
- Point defense
- Subsystem damage

## Retrieved Context

No prior context retrieved — no tagged documents matched this area.
