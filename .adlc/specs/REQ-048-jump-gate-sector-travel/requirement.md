---
id: REQ-048
title: "Second Sector & Jump Gate Travel"
status: draft
deployable: true
created: 2026-06-11
updated: 2026-06-11
component: "game/galaxy/Travel"
domain: "world"
stack: ["unreal", "cpp"]
concerns: ["reliability", "performance", "testability"]
tags: ["sector-travel", "jump-gate", "world-server-stub", "tier-f"]
---

## Description

Add a second sector map and jump gate travel: player activates gate → loading screen/VFX → reconnect or seamless travel to Sector B dedicated server (or same server level streaming for prototype). World orchestrator stub tracks `current_sector` on character.

**Why:** MMO identity requires leaving the starter bubble; validates sector transition flow from architecture doc before full galaxy scale.

**Depends on:** REQ-036, REQ-043

**Runnable Demo:** Fly to jump gate in Sector A → activate → arrive Sector B spawn point → `current_sector` updated in DB on success.

Reference: `docs/05-architecture.md` Sector Transition Flow, JumpDrive; Galaxy Module Travel.

## System Model

### Entities

| Entity | Field | Type | Constraints |
|--------|-------|------|-------------|
| Sector | sector_id | string | e.g., Sol-3, Frontier-1 |
| JumpGate | destination_sector | string | FK logical |
| JumpGate | cooldown_sec | float | default 30 |
| Character | current_sector | string | persisted |

### Events

| Event | Trigger | Payload |
|-------|---------|---------|
| jump_requested | Player activates gate | `{ character_id, from, to }` |
| jump_completed | Arrival in destination | `{ character_id, sector, transform }` |

### Permissions

| Action | Roles Allowed |
|--------|---------------|
| initiate_jump | ship owner, server validates cooldown |
| update_sector | server / world stub |

## Business Rules

- [ ] BR-1: Jump rejected if cooldown active or combat flag set (in combat = took damage in last 10s).
- [ ] BR-2: `current_sector` persisted on successful arrival.
- [ ] BR-3: Sector B has own PlayerStart and optional NPC spawners (copy of test config OK).
- [ ] BR-4: Prototype may use single dedicated server with two sublevels vs two processes — document chosen approach.
- [ ] BR-5: Failed transition rolls back to Sector A spawn without corrupting character row.

## Acceptance Criteria

- [ ] Two sector maps exist and are reachable via jump gate.
- [ ] Jump shows transition VFX or loading screen < 10s on dev hardware.
- [ ] Character `current_sector` in DB matches in-world sector after jump.
- [ ] Automation or scripted test completes A→B→A round trip.
- [ ] **Runnable Demo:** Login in Sector A, jump to B, verify radar/entities differ, jump back.

## External Dependencies

- REQ-043 character sector field
- REQ-036 networking (if cross-server)

## Assumptions

- World orchestrator is a lightweight stub service or in-process manager for prototype.
- Interdiction and wormholes deferred.

## Open Questions

- [ ] Single server level streaming vs two dedicated processes for prototype?

## Out of Scope

- 20+ sector galaxy
- Fleet jump synchronization
- Wormhole hidden routes

## Retrieved Context

No prior context retrieved — no tagged documents matched this area.
