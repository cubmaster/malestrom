---
id: REQ-033
title: "6DOF Ship Flight Prototype (Single-Player)"
status: complete
deployable: true
created: 2026-06-11
updated: 2026-06-11
component: "game/combat/Movement"
domain: "combat"
stack: ["unity", "csharp"]
concerns: ["performance", "testability"]
tags: ["6dof", "flight", "single-player", "tier-a"]
---

## Description

6DOF momentum-based ship flight in a single-player test sector — the first playable ship loop for Tier A on Unity (re-platform of legacy UE REQ-033).

Implement six-degrees-of-freedom ship movement for a single player in the test sector. Movement uses thrust, strafe, pitch/yaw/roll, and speed clamping appropriate for space combat (not atmospheric flight).

**Why:** Flight is the foundation of every combat, travel, and mission loop. It must feel correct locally before networking adds complexity.

**Depends on:** REQ-051 (Unity project foundation)

**Runnable Demo:** Play Mode in `EmptySector` — WASD + mouse fly a placeholder ship through open space at variable speed.

Reference: `docs/05-architecture.md` Combat Module → `MovementComponent`; `docs/04-space-combat.md` engagement flow.

## System Model

### Entities

| Entity | Field | Type | Constraints |
|--------|-------|------|-------------|
| ShipPawn | max_speed | float | > 0, configurable per hull ScriptableObject |
| ShipPawn | thrust_forward | float | m/s² equivalent |
| ShipPawn | strafe_speed | float | lateral/vertical |
| ShipPawn | rotation_rate | float | deg/s pitch/yaw/roll |
| ShipMovement | velocity | Vector3 | clamped to max_speed |

### Events

| Event | Trigger | Payload |
|-------|---------|---------|
| ship_moved | Tick while input active | `{ position, rotation, velocity }` |

### Permissions

| Action | Roles Allowed |
|--------|---------------|
| pilot_ship | local player |

## Business Rules

- [x] BR-1: Movement is physics-based (momentum); releasing thrust does not instant-stop unless brake input is held.
- [x] BR-2: Max speed is enforced every tick; no unbounded acceleration exploits.
- [x] BR-3: Input scheme supports keyboard + mouse minimum; gamepad mapping is optional in this REQ.
- [x] BR-4: Ship uses a placeholder mesh/collision hull; stats loaded from `Human_Starter_Fighter` defaults (ScriptableObject optional).

## Acceptance Criteria

- [x] Player possesses a ship pawn in the test map and controls 6DOF movement.
- [x] Forward thrust, reverse/brake, strafe (left/right/up/down), and rotation (pitch/yaw/roll) all function.
- [x] Speed clamp verified by automation test applying max thrust for N seconds.
- [x] No collision with sector bounds causes fall-through or stuck state (simple box bounds OK).
- [x] **Runnable Demo:** Play → fly from spawn to far boundary and back under manual control.

## External Dependencies

- REQ-051 Unity project and `EmptySector` test scene

## Assumptions

- Client-side only; no replication in this REQ.
- Arcade-style flight feel is acceptable for prototype; Newtonian drift tuning is iterative.
- Unity uses meters (UE legacy reference used centimeters).

## Open Questions

- Resolved: **Flight assist deferred** — see `architecture.md` ADR-033-3.

## Out of Scope

- Network replication (REQ-036)
- Power allocation affecting engine output (REQ-038)
- Collision damage

## Retrieved Context

No prior context retrieved — no tagged documents matched this area.
