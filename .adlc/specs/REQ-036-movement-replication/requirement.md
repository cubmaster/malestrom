---
id: REQ-036
title: "Server-Authoritative Ship Movement Replication"
status: draft
deployable: true
created: 2026-06-11
updated: 2026-06-11
component: "game/networking/Replication"
domain: "networking"
stack: ["unreal", "cpp"]
concerns: ["performance", "reliability", "testability"]
tags: ["replication", "client-prediction", "movement", "tier-b"]
---

## Description

Replicate ship movement from server to all clients with client-side prediction and reconciliation for the owning player. Remote players see smooth interpolated motion.

**Why:** Core MMO feel depends on responsive local control and accurate remote views. Must be proven before combat (which assumes correct positions).

**Depends on:** REQ-035, REQ-033

**Runnable Demo:** Two clients connected — Client A flies in circles; Client B observes smooth motion; server remains authoritative (no client teleport exploits in test).

Reference: `docs/05-architecture.md` Replication Strategy table (ship position/rotation).

## System Model

### Entities

| Entity | Field | Type | Constraints |
|--------|-------|------|-------------|
| ReplicatedMovement | server_transform | FTransform | authoritative |
| ReplicatedMovement | client_predicted | bool | owner only |
| NetSettings | tick_rate | float | 30 Hz combat default documented |

### Events

| Event | Trigger | Payload |
|-------|---------|---------|
| movement_corrector | Server/client mismatch | `{ delta, snap_or_blend }` |

### Permissions

| Action | Roles Allowed |
|--------|---------------|
| submit_movement_input | owning client |
| apply_authoritative_move | server |

## Business Rules

- [ ] BR-1: Server simulates movement for all ships; clients send input or acceleration requests, not raw position teleports.
- [ ] BR-2: Owning client predicts locally; reconciliation on divergence beyond threshold.
- [ ] BR-3: Non-owner clients interpolate remote ships (no prediction).
- [ ] BR-4: Replication relevancy uses distance culling appropriate for sector scale (documented radius).

## Acceptance Criteria

- [ ] Two-client test: both players see each other move in real time.
- [ ] Owning client input latency feels responsive (<100ms on LAN in manual test).
- [ ] Forced server correction does not permanently desync client.
- [ ] Automation test (multi-instance or mocked): server moves pawn, client receives updated transform within N ticks.
- [ ] **Runnable Demo:** REQ-035 demo with both ships flying simultaneously visible to both clients.

## External Dependencies

- REQ-035 session layer
- REQ-033 movement component refactored for server authority

## Assumptions

- 30 Hz server tick for prototype; 10 Hz travel mode deferred.
- No lag compensation for weapons until REQ-039.

## Open Questions

- [ ] CharacterMovementComponent-style vs custom movement replication?

## Out of Scope

- Sector transition handoff
- Vehicle ownership transfer
- Cheat detection beyond basic validation

## Retrieved Context

No prior context retrieved — no tagged documents matched this area.
