---
id: REQ-046
title: "Ship AI Companion Core (3 Abilities)"
status: draft
deployable: true
created: 2026-06-11
updated: 2026-06-11
component: "game/ai/ShipAIModule"
domain: "progression"
stack: ["unreal", "cpp", "umg"]
concerns: ["testability", "performance"]
tags: ["kestrel", "ai-companion", "abilities", "tier-e"]
---

## Description

Implement the ship AI companion with level 1–10 progression stub, 2 ability slots, and three abilities: **Emergency Repair** (active), **Target Lock Assist** (passive), **ECM Burst** (active). AI draws from AI power allocation (REQ-038).

**Why:** Kestrel/ship AI is a differentiator in Iron Exiles design and must be proven alongside combat.

**Depends on:** REQ-038, REQ-040, REQ-045

**Runnable Demo:** Trigger Emergency Repair during combat — hull regens over time; ECM Burst on cooldown breaks enemy lock (manual test with 2 clients).

Reference: `docs/05-architecture.md` ShipAIModule; `docs/06-multiplayer-gameplay.md` AI companion; `docs/08-leveling.md` AI slot unlocks.

## System Model

### Entities

| Entity | Field | Type | Constraints |
|--------|-------|------|-------------|
| AICompanion | level | int | 1-10 in this REQ |
| AICompanion | personality | string | faction-based flavor text |
| AIAbility | ability_id | string | DataTable row |
| AIAbility | cooldown_remaining | float | server tracked |
| AIAbility | power_cost | float | from AI allocation |

### Events

| Event | Trigger | Payload |
|-------|---------|---------|
| ai_ability_activated | Server accepts use | `{ character_id, ability_id }` |
| ai_bark | Context trigger | `{ line_id }` optional stub |

### Permissions

| Action | Roles Allowed |
|--------|---------------|
| activate_ability | ship owner, server validates CD + power |

## Business Rules

- [ ] BR-1: Abilities execute server-side; clients play VFX/audio on multicast.
- [ ] BR-2: Emergency Repair restores hull over 10s; interrupted if hull max reached.
- [ ] BR-3: ECM Burst clears target lock on enemies within range (break lock, not permanent immunity).
- [ ] BR-4: Target Lock Assist passive reduces lock acquisition time by documented % (server-side).
- [ ] BR-5: AI level 1 at creation; XP for AI deferred — manual level set for testing OK.

## Acceptance Criteria

- [ ] HUD shows 2 AI ability buttons with cooldown states.
- [ ] Emergency Repair restores hull in automation test (damaged → repair → HP increases).
- [ ] ECM Burst clears lock on target player in two-client test.
- [ ] AI power allocation at 0% prevents active ability use.
- [ ] **Runnable Demo:** Take damage, repair, use ECM in mock PvP.

## External Dependencies

- REQ-038 AI power slice
- REQ-037 lock system for ECM interaction

## Assumptions

- Voice/dialogue lines stubbed as on-screen text.
- AI levels 11–50 and full ability roster in future REQ.

## Open Questions

- [ ] Name companion "Kestrel" for Human only or generic "Ship AI"?

## Out of Scope

- AI chip equipment crafting
- Full 50-level AI progression
- AI dialogue manager / mood system

## Retrieved Context

No prior context retrieved — no tagged documents matched this area.
