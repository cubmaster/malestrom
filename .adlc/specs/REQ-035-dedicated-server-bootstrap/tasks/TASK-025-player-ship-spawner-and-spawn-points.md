---
id: TASK-025
title: "PlayerShipSpawner and SpawnPointManager"
status: draft
parent: REQ-035
created: 2026-06-19
updated: 2026-06-19
dependencies: [TASK-024]
---

## Description

Implement server-authoritative player ship spawning. When a client connects, the server instantiates a networked player ship prefab at the next available spawn point. When a client disconnects, the server despawns their ship.

## Files to Create/Modify

- `Client/Assets/_Project/Scripts/Networking/PlayerShipSpawner.cs` — server-only component; listens to `NetworkManager.OnClientConnectedCallback` / `OnClientDisconnectedCallback`; spawns/despawns player ship prefab with ownership
- `Client/Assets/_Project/Scripts/Networking/SpawnPointManager.cs` — holds an array of spawn transforms; allocates next free slot per connection
- `Client/Assets/Scenes/Test/EmptySector.unity` — add SpawnPointManager with 2+ spawn point transforms; register player ship prefab in NetworkManager's prefab list

## Acceptance Criteria

- [ ] Server spawns one NetworkObject ship per connected client
- [ ] Each ship is owned by the connecting client (verified via `OwnerClientId`)
- [ ] Spawn positions are distinct (no overlapping)
- [ ] Disconnecting a client despawns their ship without error
- [ ] Server and remaining clients remain stable after a disconnect
- [ ] Edit Mode test validates spawn point allocation logic (round-robin or sequential)

## Technical Notes

- Use `NetworkManager.Singleton.SpawnManager.InstantiateAndSpawn()` or `NetworkObject.SpawnWithOwnership(clientId)`
- Spawn points: create 4 empty GameObjects as children of SpawnPointManager at staggered positions (e.g., every 20m along X axis)
- Prefab must be registered in NetworkManager's `NetworkPrefabs` list
- Server-only code gated by `IsServer` check
