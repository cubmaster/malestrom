---
id: REQ-038
title: "Reactor Power Allocation System"
status: draft
deployable: true
created: 2026-06-11
updated: 2026-06-11
component: "game/combat/PowerManager"
domain: "combat"
stack: ["unreal", "cpp", "umg"]
concerns: ["performance", "testability"]
tags: ["power-allocation", "reactor", "tier-c"]
---

## Description

Implement the W/S/E/AI power split (Weapons, Shields, Engines, AI) that modifies effective output of subsystems. Player adjusts allocation via HUD sliders; server stores authoritative allocation.

**Why:** Power routing is core combat tactics from the GDD and gates weapon/shield/engine effectiveness before beam weapons land.

**Depends on:** REQ-037, REQ-034

**Runnable Demo:** PIE or two-client — slide weapon power to max, observe increased thrust reduction (engine power) and documented effect on weapon readiness metric.

Reference: `docs/05-architecture.md` PowerManager; Flight HUD power section; `docs/09-realtime-combat.md` sustained pressure design.

## System Model

### Entities

| Entity | Field | Type | Constraints |
|--------|-------|------|-------------|
| PowerAllocation | weapons_pct | float | 0–100, sum of all == 100 |
| PowerAllocation | shields_pct | float | 0–100 |
| PowerAllocation | engines_pct | float | 0–100 |
| PowerAllocation | ai_pct | float | 0–100 |
| Reactor | total_output | float | from hull DataTable |

### Events

| Event | Trigger | Payload |
|-------|---------|---------|
| power_reallocated | Client submits new split | `{ w, s, e, ai }` validated server-side |

### Permissions

| Action | Roles Allowed |
|--------|---------------|
| set_allocation | ship owner, server validates |

## Business Rules

- [ ] BR-1: Four allocation values must sum to 100%; invalid submissions rejected.
- [ ] BR-2: Engine allocation modifies max speed and acceleration from REQ-033 baseline.
- [ ] BR-3: Weapon allocation modifies effective DPS cap (applied in REQ-039).
- [ ] BR-4: Shield allocation modifies regen rate (applied in REQ-040).
- [ ] BR-5: AI allocation reserved — no effect until REQ-046 but UI slot exists.

## Acceptance Criteria

- [ ] HUD sliders (or preset buttons) adjust W/S/E/AI split with live total validation.
- [ ] Server rejects invalid totals; client resyncs to server state.
- [ ] Measurable engine effect: 100% engines vs 100% weapons changes max speed by documented ratio.
- [ ] Automation test: set 100% engines, assert speed > 100% weapons case.
- [ ] **Runnable Demo:** Fly ship, max engines, then max weapons — feel speed difference.

## External Dependencies

- REQ-033 movement
- REQ-034 HUD

## Assumptions

- Linear scaling acceptable for prototype; diminishing returns tuned later.

## Open Questions

- [ ] Preset buttons (combat/travel/balanced) in this REQ?

## Out of Scope

- Reactor damage reducing total output
- Power drain from individual weapons beyond allocation cap

## Retrieved Context

No prior context retrieved — no tagged documents matched this area.
