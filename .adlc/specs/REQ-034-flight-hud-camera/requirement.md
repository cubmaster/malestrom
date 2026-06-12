---
id: REQ-034
title: "Chase Camera & Flight HUD Shell"
status: complete
deployable: true
created: 2026-06-11
updated: 2026-06-11
component: "game/ui/HUD"
domain: "ui"
stack: ["unity", "csharp", "ugui"]
concerns: ["a11y", "testability"]
tags: ["hud", "camera", "tier-a"]
repo: malestrom
---

## Description

Add a third-person chase camera with collision pull-in and a minimal flight HUD showing speed, heading, and a hull placeholder bar. Establishes the Unity HUD presenter/view pattern used by later UI work.

**Why:** Combat readability requires camera + HUD before weapons land; this increment is independently testable in single-player Play Mode.

**Depends on:** REQ-033 (Unity 6DOF flight)

**Runnable Demo:** Play Mode in `EmptySector` — HUD displays live speed and hull bar; camera follows ship during hard turns without clipping through geometry.

Reference: `docs/05-architecture.md` Flight HUD wireframe; camera modes table (chase only in this REQ).

## System Model

### Entities

| Entity | Field | Type | Constraints |
|--------|-------|------|-------------|
| FlightHUD | speed_display | float | bound to ship velocity magnitude (m/s) |
| FlightHUD | heading_display | float | 0–360° yaw |
| FlightHUD | hull_percent | float | 0–100, placeholder static 100 until REQ-040 |
| ChaseCamera | arm_length | float | configurable |
| ChaseCamera | lag_speed | float | smooth follow |
| ChaseCamera | collision_radius | float | sphere cast radius |

### Events

| Event | Trigger | Payload |
|-------|---------|---------|
| hud_updated | Tick | `{ speed, heading, hull_pct }` |

### Permissions

| Action | Roles Allowed |
|--------|---------------|
| view_hud | local player |

## Business Rules

- [x] BR-1: HUD updates every frame while ship active; clears/hides when ship disabled.
- [x] BR-2: Camera collision pulls in when obstructed (sphere cast along arm).
- [x] BR-3: Cockpit view toggle is out of scope; chase camera is default and only mode.

## Acceptance Criteria

- [x] Flight HUD shows speed in **m/s** and heading in degrees.
- [x] Hull bar widget exists (static 100% OK; wired for REQ-040).
- [x] Chase camera follows ship rotation and position with configurable lag and collision pull-in.
- [x] Edit Mode test asserts HUD presenter reports speed > 0 when telemetry reports movement.
- [x] **Runnable Demo:** Play Mode with HUD visible during REQ-033 flight controls.

## External Dependencies

- REQ-033 ship movement (`IronExiles.Combat`)

## Assumptions

- uGUI (`UnityEngine.UI`) for prototype HUD; UI Toolkit migration deferred.
- CommonUI / station menus deferred until REQ-050.
- Radar/target UI deferred to REQ-037.

## Open Questions

- Resolved: display speed as **m/s** (matches REQ-033 meter-based movement).

## Out of Scope

- Radar, targeting reticle, power allocation UI (later REQs)
- Cockpit / orbit camera modes
- Multiplayer HUD replication

## Retrieved Context

No prior context retrieved — no tagged documents matched this area.
