# Targeting & Radar — REQ-037

## Overview

Server-authoritative tab targeting with owner HUD radar blips. Lock state replicates via `NetworkVariable<ulong>`; radar contacts are computed on the owner from replicated entity positions.

## Sensor Defaults

| Setting | Value | Notes |
|---------|-------|-------|
| Lock range | **2500 m** | Server rejects locks beyond range |
| Max radar contacts | **20** | Closest contacts prioritized |
| LOS break timer | **5 s** | Lock clears after sustained occlusion |
| Tab cycle sort | Angle from ship forward, then distance | BR-2 |

## Controls

- **Tab** — cycle hostile/neutral targets within range (owner → ServerRpc)

## Local Two-Client Test

1. Start dedicated server + two clients (`docs/local-multiplayer-test.md`)
2. Confirm radar shows 2+ blips (other player + training dummy at ~40,0,40)
3. Press Tab on Client A — lock panel shows target name and range
4. Client B observes Client A's lock state via replicated target id (future weapon VFX in REQ-039)

## Components

- `TargetableEntity` — name, affiliation, hull placeholder
- `NetworkShipTargetingController` — server lock validation, `GetLockedTarget()`
- `ShipTargetInputController` — Tab input on owner
- `TargetDummySpawner` — server-spawned static NPC dummy
- `TargetSelectionMath` — shared scan/sort helpers

## Troubleshooting

- **Tab does nothing:** verify owner ship has `NetworkShipTargetingController` and server is running
- **No radar blips:** ensure targets have `TargetableEntity` + spawned `NetworkObject`
- **Lock drops unexpectedly:** check LOS raycast hits (default layer mask); open sector has few occluders
