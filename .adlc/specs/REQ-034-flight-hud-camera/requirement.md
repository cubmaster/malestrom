---
id: REQ-034
title: "Chase Camera & Flight HUD Shell"
status: draft
deployable: true
created: 2026-06-11
updated: 2026-06-11
component: "game/ui/HUD"
domain: "ui"
stack: ["unreal", "cpp", "umg"]
concerns: ["a11y", "testability"]
tags: ["hud", "camera", "umg", "tier-a"]
---

## Description

Add a third-person chase camera and a minimal flight HUD showing speed, heading, and hull/shield placeholders. Establishes the UMG widget architecture used by all later UI work.

**Why:** Combat readability requires camera + HUD before weapons land; this increment is independently testable in single-player PIE.

**Depends on:** REQ-033

**Runnable Demo:** PIE — HUD displays live speed and hull bar; camera follows ship during hard turns without clipping through geometry.

Reference: `docs/05-architecture.md` Flight HUD wireframe; camera modes table (chase only in this REQ).

## System Model

### Entities

| Entity | Field | Type | Constraints |
|--------|-------|------|-------------|
| FlightHUD | speed_display | float | bound to ship velocity magnitude |
| FlightHUD | hull_percent | float | 0–100, placeholder static 100 until REQ-040 |
| ChaseCamera | arm_length | float | configurable |
| ChaseCamera | lag_speed | float | smooth follow |

### Events

| Event | Trigger | Payload |
|-------|---------|---------|
| hud_updated | Tick | `{ speed, hull_pct }` |

### Permissions

| Action | Roles Allowed |
|--------|---------------|
| view_hud | local player |

## Business Rules

- [ ] BR-1: HUD updates every frame while possessed; no stale values after unpossess.
- [ ] BR-2: Camera collision pulls in when obstructed (spring arm trace).
- [ ] BR-3: Cockpit view toggle is out of scope; chase camera is default and only mode.

## Acceptance Criteria

- [ ] UMG Flight HUD widget shows speed (m/s or game units labeled consistently).
- [ ] Hull bar widget exists (static 100% OK; wired for REQ-040).
- [ ] Chase camera follows ship rotation and position with configurable lag.
- [ ] Automation test spawns ship, applies thrust, asserts HUD speed > 0.
- [ ] **Runnable Demo:** PIE with HUD visible during REQ-033 flight demo.

## External Dependencies

- REQ-033 ship pawn

## Assumptions

- CommonUI integration deferred until station menus (REQ-050).
- Radar/target UI deferred to REQ-037.

## Open Questions

- [ ] Display units as m/s or abstract "units" matching design docs?

## Out of Scope

- Radar, targeting reticle, power allocation UI (later REQs)
- Cockpit / orbit camera modes
- Multiplayer HUD replication

## Retrieved Context

No prior context retrieved — no tagged documents matched this area.
