---
id: REQ-036
title: "Server-Authoritative Ship Movement Replication"
status: approved
deployable: true
created: 2026-06-11
updated: 2026-06-20
component: "game/networking/Replication"
domain: "networking"
stack: ["unity", "csharp", "netcode"]
concerns: ["performance", "reliability", "testability"]
tags: ["replication", "client-prediction", "movement", "tier-b"]
repo: malestrom
---

## Description

Replicate ship movement from the dedicated server to all clients with client-side prediction and reconciliation for the owning player. Remote players see smooth interpolated motion.

**Why:** Core MMO feel depends on responsive local control and accurate remote views. Must be proven before combat (which assumes correct positions).

**Depends on:** REQ-035 (session/spawn), REQ-033 (6DOF movement model)

**Runnable Demo:** Two clients connected — Client A flies in circles; Client B observes smooth motion; server remains authoritative (no client position teleports accepted).

Reference: `docs/05-architecture.md` Replication Strategy table (ship position/rotation); ADR-034/035-1.

## System Model

### Entities

| Entity | Field | Type | Constraints |
|--------|-------|------|-------------|
| ShipMovementInput | local_thrust | Vector3 | clamped -1..1 per axis |
| ShipMovementInput | local_rotation | Vector3 | clamped -1..1 per axis |
| ShipMovementInput | brake | bool | |
| NetworkShipMovement | server_tick_hz | float | default 30 Hz |
| NetworkShipMovement | reconcile_position_m | float | default 2m snap/blend threshold |

### Events

| Event | Trigger | Payload |
|-------|---------|---------|
| movement_input_submitted | Owner client each frame | `ShipMovementInput` |
| movement_reconciled | Owner divergence > threshold | `{ position_delta, blend }` |

### Permissions

| Action | Roles Allowed |
|--------|---------------|
| submit_movement_input | owning client (ServerRpc) |
| apply_authoritative_move | server only |

## Business Rules

- [ ] BR-1: Server simulates movement for all ships; clients send input requests, not raw position teleports.
- [ ] BR-2: Owning client predicts locally; reconciliation on divergence beyond threshold.
- [ ] BR-3: Non-owner clients interpolate remote ships (NetworkTransform), no local simulation.
- [ ] BR-4: Replication relevancy documented for sector scale (default: full sector / no culling in prototype).

## Acceptance Criteria

- [ ] Two-client test: both players see each other move in real time.
- [ ] Owning client input latency feels responsive on LAN manual test.
- [ ] Forced server correction does not permanently desync client.
- [ ] Edit Mode or automated test: input payload round-trip + reconciliation threshold behavior.
- [ ] **Runnable Demo:** REQ-035 demo with both ships flying simultaneously visible to both clients.

## External Dependencies

- REQ-035 session layer and networked player ship prefab
- REQ-033 `ShipMovementModel` integrator

## Assumptions

- 30 Hz server simulation tick for prototype; travel-mode throttling deferred.
- NGO `NetworkTransform` (server authority) for remote interpolation.
- No lag compensation for weapons until REQ-039.

## Resolved Decisions

| Question | Decision |
|----------|----------|
| Replication approach | Custom `NetworkShipMovementController` using `ShipMovementModel` + owner ServerRpc input (not UE CharacterMovement) |
| Remote viewers | Server-authoritative `NetworkTransform` with interpolation enabled |

## Open Questions

None.

## Out of Scope

- Sector transition handoff
- Vehicle ownership transfer
- Cheat detection beyond basic server-side simulation
- Distance-based relevancy culling (document only)

## Retrieved Context

- LESSON-004 (Unity flight re-platform): reuse `ShipMovementModel` for server + client prediction
- REQ-035: networking bootstrap, spawn, static ships baseline
