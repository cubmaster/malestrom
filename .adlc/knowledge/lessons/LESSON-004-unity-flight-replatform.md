# LESSON-004: Unity 6DOF flight re-platform from UE

---
id: LESSON-004
title: "Port flight logic to testable C# model; one Edit Mode asmdef per folder"
created: 2026-06-11
domain: combat
component: game/combat/Movement
related: [REQ-033, ADR-034]
tags: [unity, flight, testing, port]
---

## Context

REQ-033 re-platformed UE `UShipMovementComponent` to Unity under `IronExiles.Combat`. Implementation was hand-authored without Editor; first local test run used Unity 6000.4.x and upgraded packages.

## Lesson

- Extract **`ShipMovementModel`** (pure integrator) so speed clamp, bounds, and momentum tests run in **Edit Mode** without Play Mode.
- Unity forward is **+Z**; UE forward is **+X** — thrust input tests must use local axes, not `Vector3.right` as “forward”.
- Convert UE centimeters to **meters** (÷100) for `MaxSpeed`, thrust, and bounds defaults.
- Unity allows **only one `.asmdef` per folder** — keep flight tests in `IronExiles.Core.Tests` or move combat tests to a subfolder; do not add a second asmdef beside `IronExiles.Core.Tests.asmdef`.
- Pin `Packages/manifest.json` to the LTS versions in `ProjectVersion.txt`; discard Editor auto-upgrades from incidental opens on newer patch editors.
- Input System (`activeInputHandler: 1`) needs **`Unity.InputSystem`** asmdef reference and device polling (`Keyboard.current` / `Mouse.current`) unless `.inputactions` assets are added.

## Follow-up

- REQ-034 flight HUD / chase camera polish
- Move root UE tree to `legacy/unreal/` when no longer needed for reference
