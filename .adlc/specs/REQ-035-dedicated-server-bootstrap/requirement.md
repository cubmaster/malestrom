---
id: REQ-035
title: "Dedicated Server Bootstrap & Client Connection"
status: complete
deployable: true
created: 2026-06-11
updated: 2026-06-20
component: "game/networking/Session"
domain: "networking"
stack: ["unity", "csharp", "netcode", "unity-transport"]
concerns: ["reliability", "testability", "security"]
tags: ["dedicated-server", "multiplayer", "tier-b", "netcode-for-gameobjects"]
repo: malestrom
---

## Description

Enable **headless Unity dedicated server** builds and connect **two game clients** to the same `EmptySector` instance using **Netcode for GameObjects (NGO)**. The server spawns one networked player ship per connection at designated spawn points. Ships may remain static for this increment — **no movement replication** yet.

**Why:** Validates the MMO server model early: dedicated authority process + thin clients. Surfaces packaging, port, transport, and spawn ownership issues before REQ-036 movement replication and combat.

**Depends on:** REQ-033 (Unity 6DOF flight), REQ-034 (HUD/camera shell). **Does not** depend on REQ-032 (superseded UE foundation).

**Runnable Demo:** Start dedicated server → connect Client A and Client B via documented local IP/port (or Multiplayer Play Mode helper) → both see two placeholder ships in the sector.

Reference: `docs/05-architecture.md` Dedicated Game Servers, Networking Architecture; ADR-034 (Unity + NGO).

## System Model

### Entities

| Entity | Field | Type | Constraints |
|--------|-------|------|-------------|
| NetworkSession | listen_address | string | default `0.0.0.0` on server |
| NetworkSession | listen_port | ushort | default `7878` (configurable) |
| NetworkSession | max_players | int | default 10 for prototype |
| PlayerSpawnPoint | spawn_index | int | unique per slot in scene |
| PlayerSpawnPoint | transform | Vector3/Quaternion | world pose |
| NetworkedPlayerShip | owner_client_id | ulong | NGO owner id |
| NetworkedPlayerShip | is_local_player | bool | true for owning client only |

### Events

| Event | Trigger | Payload |
|-------|---------|---------|
| client_connected | NGO `OnClientConnected` | `{ client_id }` |
| client_disconnected | NGO `OnClientDisconnected` | `{ client_id }` |
| player_ship_spawned | Server spawn complete | `{ client_id, spawn_index, position }` |

### Permissions

| Action | Roles Allowed |
|--------|---------------|
| start_host | dev tooling only (not production path) |
| start_dedicated_server | server build / launch script |
| connect_as_client | any client with address + port |
| spawn_player_ship | **server only** (informed by LESSON-002 — server-authoritative pattern) |

## Business Rules

- [ ] BR-1: Server is authoritative for player ship spawn; clients cannot spawn arbitrary networked ships.
- [ ] BR-2: Dedicated server build target produces a runnable **headless** binary for **Windows** and **Linux**; **Linux headless build runs in CI on first delivery** (informed by ADR-034).
- [ ] BR-3: Minimum **two simultaneous clients** supported in `EmptySector` without error.
- [ ] BR-4: Disconnecting a client despawns or hides that player's ship without crashing the server or other clients.
- [ ] BR-5: Local-player HUD/camera (REQ-034) attach only to the ship owned by that client; remote ships are visible but not possessed.

## Acceptance Criteria

- [ ] `Client/` includes Netcode for GameObjects + Unity Transport packages pinned in `manifest.json`.
- [ ] Documented dedicated-server build produces headless server executables for **Windows** and **Linux**.
- [ ] **CI (self-hosted):** GitHub Actions workflow on a **self-hosted runner** (labels `self-hosted`, `unity`, `linux`) builds the **Linux headless dedicated server** and runs automated multiplayer verification (multi-instance Play Mode test or equivalent batchmode script).
- [ ] Two clients connect to the same dedicated server instance without error.
- [ ] Each client receives its own networked ship at a distinct spawn point; each sees the other player's ship (static pose OK).
- [ ] Automated verification on **self-hosted CI**: asserts **connected client count == 2** and **spawned player ships == 2** after connect (Play Mode multi-instance or scripted integration test).
- [ ] Documented repo scripts or README steps for local two-client + server test (e.g. `Run-UnityDedicatedServer.ps1` + two client launches — names finalized in architecture phase).
- [ ] **Runnable Demo:** Server + 2 clients running in `EmptySector`; each player sees two ships and local HUD on own ship only.

## External Dependencies

- REQ-033 (`IronExiles.Combat` movement/input — local control disabled or ignored on remote peers until REQ-036)
- REQ-034 (`IronExiles.UI` HUD binds to local owned ship telemetry)
- Unity packages: **Netcode for GameObjects**, **Unity Transport (UTP)**
- Optional dev aid: Unity Multiplayer Play Mode / ParrelSync-style workflow (document if used)

## Assumptions

- LAN / `127.0.0.1` testing only; JWT/auth gate deferred to REQ-042 (informed by REQ-031 Tier B sequencing).
- Self-hosted GitHub Actions runner with Unity **6000.0.32f1**, `UNITY_LICENSE` (or equivalent activation), and capacity for Linux headless server builds (informed by LESSON-003).
- CI runs **Linux headless dedicated-server build immediately** in REQ-035 — not deferred to a follow-up REQ.
- Relay/NAT punch-through and Unity Gaming Services relay deferred.
- Placeholder cube ship visuals from REQ-033 are acceptable; no replication of flight input yet.
- Single `EmptySector` scene hosts both spawn logic and session bootstrap for the prototype.

## Resolved Decisions

| Question | Decision |
|----------|----------|
| CI runner | **Self-hosted** GitHub Actions runner with Unity installed (`self-hosted`, `unity`, `linux` labels). Multi-instance Play Mode or batchmode integration test for 2-client verification. |
| Linux headless in CI | **Yes, immediately** — Linux dedicated-server build is a first-pass acceptance gate, not documentation-only. |

## Resolved Decisions (continued)

| Question | Decision |
|----------|----------|
| Default connection flow | **Hardcoded localhost:7878** for self-hosted local dev. Clients auto-connect to `127.0.0.1:7878` in prototype. CLI override (`-connectAddress`, `-connectPort`) available for non-local testing. |

## Open Questions

None.

## Out of Scope

- Movement / rotation replication (REQ-036)
- Targeting, weapons, damage (REQ-037+)
- Anti-cheat and server-side movement validation beyond spawn ownership
- World orchestrator, sector handoff, matchmaking
- Steam/EOS/Unity Gaming Services authentication

## Retrieved Context

- LESSON-002 (lesson, score 1): Engine pivot — Unreal to Unity (ADR-034)
- LESSON-003 (lesson, score 1): Unity bootstrap without editor / CI constraints
- LESSON-004 (lesson, score 1): Unity flight re-platform patterns from REQ-033
