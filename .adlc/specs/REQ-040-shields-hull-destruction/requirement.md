---
id: REQ-040
title: "Directional Shields, Hull HP & Destruction"
status: complete
deployable: true
created: 2026-06-11
updated: 2026-06-28
component: "game/combat/DefenseSystem"
domain: "combat"
stack: ["unity", "csharp"]
concerns: ["performance", "testability", "reliability"]
tags: ["shields", "hull", "damage-model", "tier-c"]
---

## Description

Add directional shields (facings), hull HP, shield regeneration when not under fire, and ship destruction when hull reaches zero. Integrates with REQ-039 beam damage (energy vs shields first).

**Why:** Completes the core combat loop: sustained fire vs regenerating defenses. Required before NPC combat and progression XP.

**Depends on:** REQ-039, REQ-038, REQ-034

**Runnable Demo:** Fire beams at target — shields deplete on facing, regen when fire stops, hull damage after shields down, target explodes/despawns at 0 HP.

Reference: `docs/09-realtime-combat.md` shield regen tension; `docs/05-architecture.md` ShieldComponent, DamageModel, DestructionHandler.

## System Model

### Entities

| Entity | Field | Type | Constraints |
|--------|-------|------|-------------|
| ShieldFacing | current_hp | float | per facing: front/back/left/right |
| ShieldFacing | max_hp | float | from equipment |
| ShieldFacing | regen_rate | float | modified by shield power allocation |
| Hull | current_hp | float | 0 to max |
| Hull | max_hp | float | from hull DataTable |
| DamageResult | shield_absorbed | float | energy damage primarily hits shields |

### Events

| Event | Trigger | Payload |
|-------|---------|---------|
| ship_destroyed | hull <= 0 | `{ victim, killer, position }` |
| shield_hit | damage on facing | `{ facing, amount_remaining }` |

### Permissions

| Action | Roles Allowed |
|--------|---------------|
| apply_damage | server |
| view_damage_state | all clients in relevancy |

## Business Rules

- [ ] BR-1: Energy damage depletes shield facing aligned with attack vector (approximate facing OK for prototype).
- [ ] BR-2: Hull receives damage only when facing shields are depleted or bypassed (no bypass in this REQ).
- [ ] BR-3: Shield regen applies only to facings not taking damage in last N seconds (default 3s).
- [ ] BR-4: Shield regen rate scales with shield power allocation (REQ-038).
- [ ] BR-5: Destruction is server-triggered; clients play VFX on replicated death event.
- [ ] BR-6: Destroyed ships respawn at PlayerStart after 5s in prototype (no loot yet).

## Acceptance Criteria

- [ ] HUD hull/shield bars reflect live replicated values (REQ-034 widgets wired).
- [ ] Sustained beam fire depletes shields then hull on correct facing.
- [ ] Regen observable when fire stops (automation: damage, wait, assert shield increased).
- [ ] Ship at 0 hull triggers destruction + respawn flow.
- [ ] **Runnable Demo:** Full fight until destruction in two-client or PvE dummy scenario.

## External Dependencies

- REQ-039 damage pipeline
- REQ-038 shield regen scaling

## Assumptions

- Four facings sufficient; hex facings deferred.
- Wreckage/loot drops deferred to economy tier.

## Open Questions

- [ ] Respawn invulnerability window duration?

## Out of Scope

- Armor damage reduction
- Point defense (REQ-055+ planned)
- Subsystem damage (engines/weapons offline)

## Retrieved Context

No prior context retrieved — no tagged documents matched this area.
