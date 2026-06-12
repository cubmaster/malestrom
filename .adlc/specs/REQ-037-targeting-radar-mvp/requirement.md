---
id: REQ-037
title: "Targeting, Tab-Lock & Radar (MVP)"
status: draft
deployable: true
created: 2026-06-11
updated: 2026-06-11
component: "game/combat/Targeting"
domain: "combat"
stack: ["unreal", "cpp", "umg"]
concerns: ["testability", "performance"]
tags: ["targeting", "radar", "tab-lock", "tier-b"]
---

## Description

Implement tab-target selection, a basic radar widget showing nearby entities, and replicated target lock state so weapons (REQ-039) know what to track. Supports players and static NPC dummies.

**Why:** E&B / Iron Exiles combat is target-driven continuous fire, not free-aim only. Targeting must exist before weapons.

**Depends on:** REQ-036, REQ-034

**Runnable Demo:** Two clients + one NPC dummy — tab-cycle targets, radar blips update, locked target HUD panel shows name and range.

Reference: `docs/05-architecture.md` TargetingComponent; `docs/04-space-combat.md` detection phase.

## System Model

### Entities

| Entity | Field | Type | Constraints |
|--------|-------|------|-------------|
| TargetLock | target_actor | Actor* | server-validated |
| TargetLock | lock_range | float | from sensor DataTable |
| RadarContact | entity_id | string | unique in sector |
| RadarContact | distance | float | updated per tick |
| RadarContact | affiliation | enum | friendly/hostile/neutral |

### Events

| Event | Trigger | Payload |
|-------|---------|---------|
| target_locked | Server confirms lock | `{ instigator, target_id }` |
| target_lost | Out of range or destroyed | `{ instigator, reason }` |

### Permissions

| Action | Roles Allowed |
|--------|---------------|
| request_lock | ship owner |
| validate_lock | server |

## Business Rules

- [ ] BR-1: Lock requests beyond sensor range are rejected server-side.
- [ ] BR-2: Tab cycles visible hostile/neutral contacts within range sorted by angle or distance (documented).
- [ ] BR-3: Breaking line-of-sight for > N seconds clears lock (N configurable, default 5s).
- [ ] BR-4: Radar shows max M contacts (default 20); closest prioritized.

## Acceptance Criteria

- [ ] Tab key cycles valid targets within range on server authority.
- [ ] HUD shows locked target name, distance, and hull placeholder bar.
- [ ] Radar widget displays blips for players and NPC dummies in range.
- [ ] Lock state replicates to all relevant clients.
- [ ] Automation test: spawn dummy, lock, assert `GetTarget()` matches dummy.
- [ ] **Runnable Demo:** Lock onto second player's ship across clients; radar shows 2+ blips.

## External Dependencies

- REQ-036 replication
- REQ-034 HUD framework

## Assumptions

- Free-aim reticle optional overlay deferred; tab-lock is primary for prototype weapons.
- Faction hostility rules simplified (all non-party = hostile for PvP test).

## Open Questions

- [ ] Hybrid free-aim + tab-lock in prototype or tab-only until REQ-039?

## Out of Scope

- Missile lock-on mechanics
- ECM breaking locks
- 3D galaxy map (separate REQ)

## Retrieved Context

No prior context retrieved — no tagged documents matched this area.
