---
id: TASK-026
title: "Networked player ship prefab with ownership-based control"
status: draft
parent: REQ-035
created: 2026-06-19
updated: 2026-06-19
dependencies: [TASK-024]
---

## Description

Convert the player ship into a NetworkObject prefab. Local systems (input, camera, HUD) activate only on the owning client. Remote ships are visible but inert. NetworkTransform syncs position/rotation for static pose visibility (full replication deferred to REQ-036).

## Files to Create/Modify

- `Client/Assets/_Project/Scripts/Combat/ShipMovementController.cs` — make it a `NetworkBehaviour`; disable Update loop when `!IsOwner`
- `Client/Assets/_Project/Scripts/Combat/ShipInputController.cs` — disable when `!IsOwner`
- `Client/Assets/_Project/Scripts/Combat/NetworkedShipSetup.cs` — new component; on `OnNetworkSpawn`, enables/disables local systems (camera, HUD, input) based on `IsOwner` / `IsLocalPlayer`
- `Client/Assets/_Project/Prefabs/PlayerShip.prefab` — new prefab with NetworkObject, NetworkTransform, MeshFilter (cube), MeshRenderer, ShipMovementController, ShipInputController, ShipFlightTelemetryAdapter, NetworkedShipSetup
- `Client/Assets/_Project/Scripts/Combat/IronExiles.Combat.asmdef` — add `Unity.Netcode.Runtime` to references

## Acceptance Criteria

- [ ] Player ship prefab has `NetworkObject` and `NetworkTransform` components
- [ ] Owning client has input, camera, and HUD active
- [ ] Non-owning clients see the ship rendered but cannot control it
- [ ] HUD binds only to the local player's ship (not remote)
- [ ] CockpitCameraRig attaches only to local player's ship
- [ ] Existing Edit Mode tests still pass (ShipMovementModel, ChaseCameraPlacement)

## Technical Notes

- `NetworkBehaviour` requires `NetworkObject` on same GameObject
- `NetworkTransform` provides basic position/rotation sync without custom code
- `IsOwner` = true only on the client that owns this object; `IsLocalPlayer` = similar for player objects
- `EmptySectorFlightSetup` should be disabled/removed in networked mode — spawning is handled by `PlayerShipSpawner` instead
- Keep `EmptySectorFlightSetup` for offline/singleplayer testing (guard with `if (NetworkManager.Singleton == null)`)
