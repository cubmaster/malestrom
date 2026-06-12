---
id: REQ-049
title: "Combat Mission Framework (Accept → Track → Complete)"
status: draft
deployable: true
created: 2026-06-11
updated: 2026-06-12
component: "game/missions/QuestSystem"
domain: "world"
stack: ["unreal", "cpp", "postgresql"]
concerns: ["testability", "reliability"]
tags: ["missions", "quests", "combat", "tier-f"]
---

## Description

Deliver a minimal mission loop: accept combat mission from board stub → kill N NPCs in sector → return or auto-complete → grant XP/credits reward. Mission state persisted per character.

**Why:** Transforms sandbox combat into a repeatable gameplay loop; foundation for story chains in content tier.

**Depends on:** REQ-041, REQ-045 (REQ-048 required only for cross-sector mission variants)

**Runnable Demo:** Accept "Clear 3 Hostiles" mission → kill 3 NPCs → mission complete notification → XP + credits applied.

Reference: `docs/06-multiplayer-gameplay.md` mission types; `docs/05-architecture.md` MissionModule QuestSystem.

## System Model

### Entities

| Entity | Field | Type | Constraints |
|--------|-------|------|-------------|
| MissionTemplate | mission_id | string | DataTable |
| MissionTemplate | objective_type | enum | kill_npc/collect/reach |
| MissionTemplate | target_count | int | > 0 |
| ActiveMission | character_id | UUID | FK |
| ActiveMission | progress | int | 0 to target_count |
| ActiveMission | status | enum | active/completed/failed |

### Events

| Event | Trigger | Payload |
|-------|---------|---------|
| mission_accepted | Player accepts | `{ character_id, mission_id }` |
| objective_progress | NPC kill matches | `{ progress, target }` |
| mission_completed | progress >= target | `{ rewards }` |

### Permissions

| Action | Roles Allowed |
|--------|---------------|
| accept_mission | character owner, max 1 active in prototype |
| update_progress | server on validated kills |

## Business Rules

- [ ] BR-1: Only one active combat mission per character in prototype.
- [ ] BR-2: Kill credit requires NPC tagged valid for mission faction/sector.
- [ ] BR-3: Rewards grant combat XP (REQ-045) and credits to character row.
- [ ] BR-4: Completed missions cannot be re-accepted until daily reset (optional) or immediately for test.
- [ ] BR-5: Mission UI shows objective text and progress counter.

## Acceptance Criteria

- [ ] At least one mission template in DataTable spawnable from mission board actor/UI stub.
- [ ] Accept → kill 3 NPCs → complete fires with rewards.
- [ ] Progress persists through disconnect mid-mission.
- [ ] Integration test: accept mission, simulate kills, assert completed status in DB.
- [ ] **Runnable Demo:** Full mission loop end-to-end in Sector A.

## External Dependencies

- REQ-041 NPC kills
- REQ-045 XP awards
- MissionService API stub or AccountService extension

## Assumptions

- Mission board is interactable in sector or station placeholder.
- Story chains and faction rep deferred.

## Open Questions

- [ ] Fail mission on disconnect or persist indefinitely?

## Out of Scope

- Group missions, fleet missions
- Trade/explore mission types
- Dynamic procedural mission generator

## Retrieved Context

No prior context retrieved — no tagged documents matched this area.
