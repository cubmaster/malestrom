---
id: REQ-038
title: "Reactor Power Allocation System"
status: complete
deployable: true
created: 2026-06-11
updated: 2026-06-21
component: "game/combat/ReactorPower"
domain: "combat"
stack: ["unity", "csharp", "netcode", "ugui"]
concerns: ["performance", "testability"]
tags: ["power-allocation", "reactor", "tier-c"]
---

## Description

Implement the W/S/E/ECM power split (Weapons, Shields, Engines, ECM — spec "AI" slot) that modifies effective output of subsystems. Player adjusts allocation via HUD sliders and preset buttons; dedicated server stores authoritative allocation via Unity Netcode for GameObjects.

**Why:** Power routing is core combat tactics from the GDD and gates weapon/shield/engine effectiveness before beam weapons land.

**Depends on:** REQ-037, REQ-034, REQ-033

**Runnable Demo:** EmptySector offline or local multiplayer — slide engine power to max, observe higher max speed vs max weapons preset.

Reference: `Client/Assets/_Project/Scripts/Combat/` movement + telemetry; `FlightHudView` power section.

## System Model

### Entities

| Entity | Field | Type | Constraints |
|--------|-------|------|-------------|
| PowerAllocation | weapons | float | 0–1, sum of all == 1 |
| PowerAllocation | shields | float | 0–1 |
| PowerAllocation | engines | float | 0–1 |
| PowerAllocation | ecm | float | 0–1 (spec AI; reserved) |
| Reactor | total_output | float | from hull stats (future) |

### Events

| Event | Trigger | Payload |
|-------|---------|---------|
| power_reallocated | Client submits new split | `{ w, s, e, ecm }` validated server-side |

### Permissions

| Action | Roles Allowed |
|--------|---------------|
| set_allocation | ship owner, server validates |

## Business Rules

- [ ] BR-1: Four allocation values must sum to 100%; invalid submissions rejected.
- [ ] BR-2: Engine allocation modifies max speed and acceleration from REQ-033 baseline (0.5× at 0% engines, 1.0× at 100%).
- [ ] BR-3: Weapon allocation modifies effective DPS cap (applied in REQ-039).
- [ ] BR-4: Shield allocation modifies regen rate (applied in REQ-040).
- [ ] BR-5: ECM allocation reserved — no effect until REQ-046 but UI slot exists.

## Acceptance Criteria

- [ ] HUD sliders and preset buttons adjust W/S/E/ECM split with live total validation.
- [ ] Server rejects invalid totals; client resyncs to server state.
- [ ] Measurable engine effect: 100% engines vs 100% weapons changes max speed by 2:1 ratio.
- [ ] Edit Mode test: set 100% engines, assert speed > 100% weapons case.
- [ ] **Runnable Demo:** Fly ship, max engines preset, then max weapons — feel speed difference.

## External Dependencies

- REQ-033 movement (`ShipMovementModel`)
- REQ-034 HUD (`FlightHudView`)
- REQ-036 movement replication (`NetworkShipMovementController`)

## Assumptions

- Linear scaling acceptable for prototype; diminishing returns tuned later.
- Preset buttons included (Combat / Travel / Balanced).

## Open Questions

- [x] Preset buttons (combat/travel/balanced) in this REQ? **Yes — included.**

## Out of Scope

- Reactor damage reducing total output
- Power drain from individual weapons beyond allocation cap

## Retrieved Context

Mapped from UE PowerManager spec to Unity NGO + UGUI flight HUD.
