---
id: REQ-045
title: "Combat XP & Level Tracking (Single Track MVP)"
status: draft
deployable: true
created: 2026-06-11
updated: 2026-06-11
component: "game/progression/XPSystem"
domain: "progression"
stack: ["unreal", "cpp", "postgresql"]
concerns: ["reliability", "testability"]
tags: ["combat-xp", "leveling", "triple-xp-mvp", "tier-e"]
---

## Description

Award combat XP for NPC kills and PvP kills; persist `combat_xp` and derived `combat_level` on character. Display XP gain in HUD. Trade and explore tracks stubbed at 0.

**Why:** Validates progression loop before full triple-XP complexity; combat is the first playable loop.

**Depends on:** REQ-041, REQ-043

**Runnable Demo:** Kill 3 NPCs — HUD shows +XP notifications — level increases per formula — relog preserves XP total.

Reference: `docs/08-leveling.md` exponential formula; `docs/06-multiplayer-gameplay.md` triple-XP overview.

## System Model

### Entities

| Entity | Field | Type | Constraints |
|--------|-------|------|-------------|
| CharacterProgress | combat_xp | bigint | >= 0 |
| CharacterProgress | combat_level | int | derived from XP table |
| XPReward | source | enum | npc_kill/pvp_kill/mission |
| XPReward | amount | int | from DataTable |

### Events

| Event | Trigger | Payload |
|-------|---------|---------|
| xp_awarded | Server confirms kill | `{ character_id, track, amount }` |
| level_up | XP crosses threshold | `{ character_id, new_level }` |

### Permissions

| Action | Roles Allowed |
|--------|---------------|
| award_xp | server only |
| read_progress | character owner |

## Business Rules

- [ ] BR-1: XP awards server-authoritative on NPC death (REQ-041 event) and PvP kill.
- [ ] BR-2: Level derived from `docs/08-leveling.md` formula or precomputed table committed in repo.
- [ ] BR-3: `trade_xp` and `explore_xp` remain 0; UI shows three tracks with combat active only.
- [ ] BR-4: XP persists to AccountService on award (batch every 30s or on logout acceptable).
- [ ] BR-5: Anti-farm: same NPC respawn grants reduced XP if killed within 60s (optional stub).

## Acceptance Criteria

- [ ] Killing NPC grants combat XP visible in HUD flytext.
- [ ] Level increments when cumulative XP crosses threshold (test level 1→2 with known XP amount).
- [ ] XP survives disconnect/reconnect.
- [ ] Automation test: apply XP reward function, assert level calculation matches table.
- [ ] **Runnable Demo:** Grind 3 NPC kills, observe level or XP bar change, relog confirms total.

## External Dependencies

- REQ-041 npc_killed event
- REQ-043 character persistence API

## Assumptions

- Overall level (max 150 combined) deferred until trade/explore tracks exist.
- Rank gates (50/100) inactive until full progression REQ.

## Open Questions

- [ ] PvP XP enabled in prototype or NPC-only?

## Out of Scope

- Trade XP, explore XP
- Skill points and skill trees
- AI companion XP (REQ-046)

## Retrieved Context

No prior context retrieved — no tagged documents matched this area.
