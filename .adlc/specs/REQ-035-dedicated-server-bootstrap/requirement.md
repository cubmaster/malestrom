---
id: REQ-035
title: "Dedicated Server Bootstrap & Client Connection"
status: draft
deployable: true
created: 2026-06-11
updated: 2026-06-11
component: "game/networking/Session"
domain: "networking"
stack: ["unreal", "cpp"]
concerns: ["reliability", "testability", "security"]
tags: ["dedicated-server", "multiplayer", "tier-b"]
---

## Description

Enable headless dedicated server builds and connect two game clients to the same sector instance. Players spawn as ship pawns at designated start points. No movement replication yet — only session join and pawn possession proof.

**Why:** Validates the MMO server model early: dedicated authority process + thin clients. Catches packaging, port, and session issues before combat replication.

**Depends on:** REQ-033, REQ-032

**Runnable Demo:** Launch dedicated server executable → connect Client A and Client B via console `open IP:PORT` or packaged launcher script → both see two ships in world.

Reference: `docs/05-architecture.md` Dedicated Game Servers, Networking Architecture.

## System Model

### Entities

| Entity | Field | Type | Constraints |
|--------|-------|------|-------------|
| GameSession | server_name | string | configurable |
| GameSession | max_players | int | default 10 for prototype |
| PlayerStart | team | int | optional, unused in prototype |
| DedicatedServer | listen_port | int | default 7777 |

### Events

| Event | Trigger | Payload |
|-------|---------|---------|
| player_joined | Client PostLogin | `{ player_id, connection_id }` |
| player_spawned | Pawn possessed | `{ player_id, spawn_transform }` |

### Permissions

| Action | Roles Allowed |
|--------|---------------|
| join_session | any connected client |
| spawn_pawn | server GameMode |

## Business Rules

- [ ] BR-1: Server is authoritative for pawn spawn; clients cannot spawn arbitrary pawns.
- [ ] BR-2: Dedicated server build target compiles for Windows and Linux.
- [ ] BR-3: Minimum two simultaneous clients supported in test sector.
- [ ] BR-4: Disconnecting client removes pawn cleanly without crashing server.

## Acceptance Criteria

- [ ] `IronExilesServer` (or documented target) builds headless.
- [ ] Two clients connect to same server instance without error.
- [ ] Each client possesses its own ship pawn at a unique PlayerStart.
- [ ] Automation or integration script: start server, connect 2 clients, assert player count == 2.
- [ ] Documented launch script for local two-client test.
- [ ] **Runnable Demo:** Server + 2 clients running; each player sees the other's placeholder ship (static OK).

## External Dependencies

- REQ-032, REQ-033
- Advanced Sessions or built-in UE Online Subsystem (Epic default for LAN)

## Assumptions

- LAN/local IP testing only; auth token gate added in REQ-042.
- Steam/EOS integration deferred.

## Open Questions

- [ ] Use Online Subsystem Null for local dev only?

## Out of Scope

- Movement replication (REQ-036)
- Anti-cheat
- World orchestrator / sector handoff

## Retrieved Context

No prior context retrieved — no tagged documents matched this area.
