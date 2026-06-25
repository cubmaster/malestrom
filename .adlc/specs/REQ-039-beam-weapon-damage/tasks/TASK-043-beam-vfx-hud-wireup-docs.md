---
id: TASK-043
title: "Beam VFX, HUD telemetry, prefab wireup, and docs"
status: complete
parent: REQ-039
created: 2026-06-25
updated: 2026-06-25
dependencies: [TASK-042]
repo: malestrom
---

## Description

Client-side beam visual stub, HUD hardpoint firing indicator, telemetry for own/locked hull, and developer docs for the runnable Tier C beam demo.

## Files to Create/Modify

- `Client/Assets/_Project/Scripts/Combat/BeamWeaponVfx.cs` — LineRenderer from ship to locked target when `IsFiring`; disable when no lock
- `Client/Assets/_Project/Scripts/Combat/ShipFlightTelemetryAdapter.cs` — own `HullPercent` from `NetworkDamageableHealth`; set BEAM 1 hardpoint `IsFiring`; locked target hull from damageable
- `Client/Assets/_Project/Scripts/Networking/NetworkPlayerShipFactory.cs` — add `BeamWeaponVfx` component
- `Client/Assets/_Project/Tests/EditMode/FlightHudPresenterTests.cs` — assert locked target hull + firing hardpoint propagate when telemetry stub updated
- `docs/beam-weapon-damage.md` — fire input, server authority, local 2-client demo steps
- `docs/local-multiplayer-test.md` — add beam fire verification bullet

## Acceptance Criteria

- [ ] Beam LineRenderer visible on firing client and observers while `IsFiring` and lock valid
- [ ] HUD BEAM 1 hardpoint shows firing state during sustained fire
- [ ] Own hull bar and locked-target hull bar decrease during beam damage in multiplayer demo
- [ ] Second client sees beam VFX and target hull replication
- [ ] **Runnable Demo:** Client A destroys training dummy (or damages Client B) with sustained primary fire
- [ ] Docs describe primary fire binding and expected DPS/hull behavior

## Technical Notes

Respect LESSON-005: VFX and telemetry stay in Combat; UI reads adapter only. Assign beam definition on prefab or load default tier-1 asset from Resources/_Project path if needed. Polish (VFX Graph, muzzle flash) deferred.
