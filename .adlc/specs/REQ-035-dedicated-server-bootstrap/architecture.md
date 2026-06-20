# REQ-035 Architecture — Dedicated Server Bootstrap

## Approach

Introduce **Netcode for GameObjects (NGO)** and **Unity Transport (UTP)** to enable a headless dedicated server that spawns networked player ships. The architecture follows the server-authoritative pattern from ADR-034: the server owns spawn logic, clients connect and receive ownership of their ship instance.

### Session Flow

```
Server (headless)                          Client A / Client B
─────────────────                          ────────────────────
Start NetworkManager (server mode)
Listen on 0.0.0.0:7878 (UTP)
                                           Start NetworkManager (client mode)
                                           Connect to 127.0.0.1:7878
OnClientConnected(clientId)
  → Spawn NetworkedPlayerShip prefab
  → Assign ownership to clientId
  → Place at next available SpawnPoint
                                           OnNetworkSpawn (local player)
                                             → Attach CockpitCameraRig
                                             → Bind FlightHUD
                                             → Enable ShipInputController
                                           OnNetworkSpawn (remote player)
                                             → Visible but no local control
OnClientDisconnected(clientId)
  → Despawn that client's ship
```

### Key Decisions

| Decision | Choice | Rationale |
|----------|--------|-----------|
| NetworkManager placement | Scene singleton on `EmptySector` | Simple for prototype; world orchestrator replaces in REQ-048 |
| Player ship prefab | NetworkObject with `ShipMovementController` + `NetworkTransform` | Reuses existing flight model; `NetworkTransform` syncs pose for REQ-035 (input replication deferred to REQ-036) |
| Spawn point strategy | Array of transforms under a `SpawnPointManager` component | Data-driven; scene-editable; scales without code changes |
| Server build | Unity Server Build target (Dedicated Server platform) | Built-in headless; Linux + Windows targets |
| Connection address | Default `127.0.0.1:7878`; CLI override via `-connectAddress` / `-connectPort` | Self-hosted local dev per resolved decision |
| Input on remote ships | `ShipInputController` disabled when `!IsOwner` | Prevents ghost input; actual replication is REQ-036 |
| HUD binding | Only on `IsLocalPlayer` | Per BR-5; existing `FlightHudBootstrap` logic stays |

### Assembly Layout

```
IronExiles.Networking.asmdef
  References: Unity.Netcode.Runtime, Unity.Networking.Transport,
              IronExiles.Combat, IronExiles.Core
  Platform: Any (client + server)

IronExiles.Combat.asmdef
  References: Unity.InputSystem, Unity.Netcode.Runtime  (add NGO ref)
```

`IronExiles.Networking` handles session bootstrap, spawn logic, and connection management. `IronExiles.Combat` gains a `Unity.Netcode.Runtime` reference so `ShipMovementController` can be a `NetworkBehaviour` with `NetworkTransform`.

### Components

| Component | Layer | Responsibility |
|-----------|-------|----------------|
| `NetworkSessionManager` | Networking | Starts server/client based on build type + CLI args |
| `SpawnPointManager` | Networking | Tracks available spawn slots; allocates on connect |
| `NetworkedPlayerShip` (prefab logic) | Combat | `NetworkBehaviour` on the ship; enables/disables local systems based on ownership |
| `PlayerShipSpawner` | Networking | Server-only; instantiates prefab on connect, despawns on disconnect |

### CI / Build

- GitHub Actions workflow `unity-server-build.yml` on self-hosted runner (`self-hosted, unity, linux`)
- Builds Linux headless dedicated server via `unity -batchmode -buildTarget LinuxHeadless64`
- Integration test: launch server + 2 client instances in batchmode, assert 2 connected + 2 ships spawned
- Windows local dev script `Run-UnityDedicatedServer.ps1` launches server build

### Proposed Addition to `.adlc/context/architecture.md`

> **ADR-035-1:** Networking assembly (`IronExiles.Networking`) owns session lifecycle and spawn. Combat assembly references `Unity.Netcode.Runtime` for `NetworkBehaviour` on ship components. This keeps networking bootstrap separate from gameplay logic while allowing ship components to be network-aware.

## Out of Scope (deferred)

- Movement input replication (REQ-036)
- Relay/NAT punch-through
- Container/Docker for dedicated server (production infra, not prototype)
- World orchestrator routing
