---
id: REQ-041
title: "Combat NPC Dummy & Minimal AI"
status: complete
deployable: true
created: 2026-06-11
updated: 2026-06-29
component: "game/ai/NPCAIModule"
domain: "combat"
stack: ["unreal", "cpp"]
concerns: ["testability", "performance"]
tags: ["npc", "behavior-tree", "pve", "tier-c"]
---

## Description

Spawn a server-side NPC ship dummy that can be targeted, damaged, and optionally fires a weak beam at the player using a minimal Behavior Tree (patrol → aggro → engage → die).

**Why:** Enables solo and small-group PvE testing without second human client; validates AI + combat integration before faction-specific trees.

**Depends on:** REQ-040, REQ-039, REQ-037

**Runnable Demo:** Dedicated server with one human — enter sector, tab-lock NPC, fight until NPC destroyed; NPC returns fire if in aggro range.

Reference: `docs/05-architecture.md` NPCAIModule; `docs/01-races.md` Kethari as first hostile faction placeholder.

## System Model

### Entities

| Entity | Field | Type | Constraints |
|--------|-------|------|-------------|
| NPCShip | faction | enum | Kethari placeholder |
| NPCShip | aggro_radius | float | default 5000 units |
| NPCShip | patrol_path | spline or wander | simple |
| NPCBrain | state | enum | idle/patrol/combat/dead |

### Events

| Event | Trigger | Payload |
|-------|---------|---------|
| npc_aggro | Player enters radius | `{ npc, player }` |
| npc_killed | hull 0 | `{ npc, killer, xp_placeholder }` |

### Permissions

| Action | Roles Allowed |
|--------|---------------|
| damage_npc | any player |
| control_npc | server AI only |

## Business Rules

- [ ] BR-1: NPCs exist only on server; clients receive replicated pawns.
- [ ] BR-2: NPC uses same damage/shield/hull rules as players (REQ-040).
- [ ] BR-3: NPC weapon DPS ≤ player starter beam to avoid unwinnable duels.
- [ ] BR-4: At least one NPC spawner in test sector with configurable count (default 3).
- [ ] BR-5: Dead NPC respawns after configurable delay (default 60s) for repeatable tests.

## Acceptance Criteria

- [ ] NPC spawns in test sector on server start.
- [ ] Player can lock and destroy NPC using beam weapons.
- [ ] NPC enters combat and fires beam when player in aggro range.
- [ ] Automation test: spawn NPC, apply damage, assert death within expected time window.
- [ ] **Runnable Demo:** Single human clears 3 NPCs in test sector.

## External Dependencies

- REQ-039, REQ-040 combat systems
- REQ-036 replication for NPC movement

## Assumptions

- Generic Kethari placeholder mesh/stats; faction-specific BT (BT_Kethari) expanded later.
- No loot on kill until economy tier.

## Open Questions

- [ ] Use UE Behavior Tree or simple state machine for prototype?

## Out of Scope

- Fleet coordination, patrol routes across sectors
- Faction reputation changes
- XP award (REQ-045)

## Retrieved Context

No prior context retrieved — no tagged documents matched this area.
