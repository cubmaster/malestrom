---
id: REQ-037
title: "Targeting, Tab-Lock & Radar (MVP)"
status: complete
deployable: true
created: 2026-06-11
updated: 2026-06-20
component: "game/combat/Targeting"
domain: "combat"
stack: ["unity", "csharp", "netcode"]
concerns: ["testability", "performance"]
tags: ["targeting", "radar", "tab-lock", "tier-b"]
repo: malestrom
---

## Description

Implement tab-target selection, a basic radar widget showing nearby entities, and replicated target lock state so weapons (REQ-039) know what to track. Supports players and static NPC dummies.

**Why:** E&B / Iron Exiles combat is target-driven continuous fire, not free-aim only. Targeting must exist before weapons.

**Depends on:** REQ-036 (movement replication), REQ-034 (HUD shell)

**Runnable Demo:** Two clients + one NPC dummy — tab-cycle targets, radar blips update, locked target HUD panel shows name and range.

Reference: `docs/05-architecture.md` TargetingComponent; `docs/04-space-combat.md` detection phase.

## System Model

### Entities

| Entity | Field | Type | Constraints |
|--------|-------|------|-------------|
| TargetLock | target_network_id | ulong | server-validated NetworkObjectId |
| TargetLock | lock_range | float | default 2500m |
| RadarContact | entity_id | ulong | NetworkObjectId |
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
| request_lock / cycle_target | ship owner (ServerRpc) |
| validate_lock | server |

## Business Rules

- [ ] BR-1: Lock requests beyond sensor range are rejected server-side.
- [ ] BR-2: Tab cycles visible hostile/neutral contacts within range sorted by angle from ship forward (documented).
- [ ] BR-3: Breaking line-of-sight for > N seconds clears lock (N configurable, default 5s).
- [ ] BR-4: Radar shows max M contacts (default 20); closest prioritized.

## Acceptance Criteria

- [ ] Tab key cycles valid targets within range on server authority.
- [ ] HUD shows locked target name, distance, and hull placeholder bar.
- [ ] Radar widget displays blips for players and NPC dummies in range.
- [ ] Lock state replicates to all relevant clients.
- [ ] Edit Mode test: target selection math + lock validation helpers.
- [ ] **Runnable Demo:** Lock onto second player's ship across clients; radar shows 2+ blips.

## External Dependencies

- REQ-036 replication
- REQ-034 HUD framework

## Assumptions

- Tab-lock is primary for prototype weapons; free-aim reticle overlay deferred.
- Faction hostility simplified (non-self ships = hostile for PvP test).
- Radar contact positions derived from replicated transforms on owner client; lock validated on server.

## Resolved Decisions

| Question | Decision |
|----------|----------|
| Hybrid free-aim + tab-lock | Tab-only until REQ-039 weapons |
| Tab sort order | Angle from ship forward, then distance tie-break |
| NPC dummy | Server-spawned networked static cube in EmptySector |

## Open Questions

None.

## Out of Scope

- Missile lock-on mechanics
- ECM breaking locks
- 3D galaxy map (separate REQ)

## Retrieved Context

- REQ-036: networked player ships with movement
- REQ-034: FlightHudView radar panel shell (contact count only today)
- ADR-034/036-1: Unity + NGO server authority
