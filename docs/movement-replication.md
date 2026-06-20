# Movement Replication — REQ-036

## Overview

Server-authoritative 6DOF ship movement using `ShipMovementModel` on the dedicated server, with owner client prediction and reconciliation. Remote clients observe interpolated motion via NGO `NetworkTransform`.

## Tick Rate

| Setting | Value | Notes |
|---------|-------|-------|
| Server simulation | **30 Hz** (`FixedUpdate` on server) | Combat prototype default |
| Client prediction | Render frame rate | Owner integrates locally between server updates |
| Travel mode throttling | Deferred | Future REQ |

## Relevancy (Prototype)

Full-sector replication — **no distance culling** in REQ-036. All connected player ships replicate to all clients in `EmptySector`. Documented radius for future work: 5000m sector bounds match `ShipMovementModel` clamp extent.

## Local Two-Client Test

1. Build and start dedicated server (see `docs/local-multiplayer-test.md`)
2. Connect Client A and Client B
3. Fly Client A in circles (WASD + mouse)
4. Observe Client B: remote ship moves smoothly
5. On Client A: controls should feel responsive on LAN; brief corrections acceptable on sharp turns

## Components

- `NetworkShipMovementController` — ServerRpc input, server sim, owner predict/reconcile
- `ShipMovementInput` — network-serializable input payload
- `NetworkTransform` — server authority, remote interpolation

## Troubleshooting

- **Remote ship frozen:** server not receiving input — check owner `NetworkShipMovementController` and server logs
- **Owner jitter every frame:** reconciliation thresholds too aggressive — increase `_positionReconcileThresholdMeters`
- **Rubber-banding:** expected on large divergence; verify server and client use same `ShipMovementModel` stats
