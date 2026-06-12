---
id: REQ-047
title: "Hull Tiers & TL1–TL3 Equipment Gating"
status: draft
deployable: true
created: 2026-06-11
updated: 2026-06-11
component: "game/progression/ShipProgression"
domain: "progression"
stack: ["unreal", "cpp"]
concerns: ["testability", "reliability"]
tags: ["hull-tier", "tech-level", "equipment", "tier-e"]
---

## Description

Implement hull tiers 1–3 with stat scaling, visual placeholder differentiation, and tech-level gating so players cannot equip TL4+ gear. Upgrade hull at station stub (console command OK for prototype).

**Why:** Progression requires tangible ship power growth beyond XP numbers; gates equipment power curve from GDD.

**Depends on:** REQ-044, REQ-045

**Runnable Demo:** Start hull tier 1 → run upgrade command or station stub → tier 2 with higher HP/speed caps → attempt TL4 weapon equip fails with message.

Reference: `docs/05-architecture.md` HullUpgradeSystem, TechLevelGate; `docs/02-ships.md` hull tiers; `docs/08-leveling.md` rank unlocks simplified.

## System Model

### Entities

| Entity | Field | Type | Constraints |
|--------|-------|------|-------------|
| HullTier | tier | int | 1-3 in this REQ |
| HullTier | base_hull_hp | float | scales per DataTable |
| HullTier | weapon_slots | int | increases with tier |
| TechGate | max_equip_tl | int | tier 1→TL3, tier 2→TL3, tier 3→TL3 |

### Events

| Event | Trigger | Payload |
|-------|---------|---------|
| hull_upgraded | Valid upgrade | `{ character_id, old_tier, new_tier }` |
| equip_rejected | TL too high | `{ item_tl, max_tl }` |

### Permissions

| Action | Roles Allowed |
|--------|---------------|
| upgrade_hull | character owner at station stub |
| equip_item | owner if TL <= gate |

## Business Rules

- [ ] BR-1: Hull tier caps max HP, shield capacity, and slot counts per DataTable.
- [ ] BR-2: Upgrade requires combat level >= documented threshold (e.g., tier 2 at level 5, tier 3 at level 15 — configurable).
- [ ] BR-3: Equipment TL cannot exceed hull tier allowed max (TL3 max in this REQ).
- [ ] BR-4: Hull tier persists via REQ-044 loadout/character extension.
- [ ] BR-5: Visual mesh swap uses placeholder variants (material or mesh per tier).

## Acceptance Criteria

- [ ] Three hull tier DataTable rows drive stat differences measurably in automation test.
- [ ] Upgrade path tier 1→2→3 works via documented dev command or station interactable.
- [ ] Equip TL4 item rejected with UI/log message.
- [ ] Tier persists across reconnect.
- [ ] **Runnable Demo:** Upgrade hull, verify HP bar max increased, still equip TL1 beam.

## External Dependencies

- REQ-044 persistence
- REQ-045 level for upgrade gates

## Assumptions

- Credits cost for upgrade optional (deduct 1000 credits if economy stub exists).
- Tier 4–6 visual upgrades in content-tier REQs.

## Open Questions

- [ ] Tie upgrade to combat level only or combined level?

## Out of Scope

- Hull tier 4–6
- TL4+ equipment
- Cutscene on upgrade

## Retrieved Context

No prior context retrieved — no tagged documents matched this area.
