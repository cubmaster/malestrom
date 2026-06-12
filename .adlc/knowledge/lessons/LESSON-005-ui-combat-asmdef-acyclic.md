---
id: LESSON-005
title: "Keep Combat and UI assemblies acyclic"
domain: game
component: game/ui/HUD
tags: [unity, asmdef, architecture]
created: 2026-06-11
---

## Context

REQ-034 added flight HUD (`IronExiles.UI`) that reads ship telemetry from `IronExiles.Combat`. Initial wiring put `FlightHudController` setup inside `EmptySectorFlightSetup` (Combat), which created a circular asmdef reference (Combat → UI → Combat).

## Lesson

Unity assembly definitions cannot reference each other in a cycle. When UI depends on Combat types (interfaces, telemetry adapters), scene wiring must live in the **UI assembly** or a thin bootstrap layer — not in Combat setup code.

**Pattern used:** `FlightHudBootstrap` (UI) finds the `Player` tag and binds `IShipFlightTelemetry` after `EmptySectorFlightSetup` spawns the ship.

## Takeaway

Before adding cross-assembly `using` statements, sketch the dependency graph. Prefer interfaces in Combat + bootstrap in UI over Combat calling UI types directly.
