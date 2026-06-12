---
id: REQ-043
title: "Character Creation & Persistence"
status: draft
deployable: true
created: 2026-06-11
updated: 2026-06-11
component: "backend/AccountService"
domain: "data"
stack: ["postgresql", "unreal", "cpp"]
concerns: ["reliability", "security", "testability"]
tags: ["character", "persistence", "faction", "tier-d"]
---

## Description

Allow authenticated players to create a character (name, faction, class) stored in PostgreSQL and loaded on login. One character per account for prototype.

**Why:** MMO progression requires persistent identity beyond auth tokens; character row anchors loadout and XP in later REQs.

**Depends on:** REQ-042

**Runnable Demo:** Login → create Human Warrior character → disconnect → reconnect → same character loads in hangar/spawn.

Reference: `docs/06-multiplayer-gameplay.md` faction/class selection; `docs/05-architecture.md` characters table.

## System Model

### Entities

| Entity | Field | Type | Constraints |
|--------|-------|------|-------------|
| Character | character_id | UUID | PK |
| Character | account_id | UUID | FK, unique for prototype |
| Character | name | string | unique server-wide |
| Character | faction | enum | Human/Velnari/Kethari/Oruneti |
| Character | class | enum | Warrior/Trader/Explorer |
| Character | current_sector | string | default starter sector |
| Character | credits | int | default 1000 |

### Events

| Event | Trigger | Payload |
|-------|---------|---------|
| character_created | POST /characters | `{ character_id, name, faction, class }` |
| character_loaded | login + fetch | `{ character snapshot }` |

### Permissions

| Action | Roles Allowed |
|--------|---------------|
| create_character | account owner, if none exists |
| read_character | account owner |

## Business Rules

- [ ] BR-1: Character name 3–20 chars, alphanumeric + space, unique case-insensitive.
- [ ] BR-2: Prototype limits one character per account.
- [ ] BR-3: Faction choice determines starter sector and hull template (Human → Sol starter).
- [ ] BR-4: Class choice stored but skill trees inactive until REQ-045+.
- [ ] BR-5: Character creation is idempotent-safe (no duplicate on retry).

## Acceptance Criteria

- [ ] Character creation UI after first login with faction/class pickers.
- [ ] Account service persists character; integration test round-trips DB row.
- [ ] Reconnect loads character name, faction, spawn sector without re-create prompt.
- [ ] Invalid names rejected with user-visible error.
- [ ] **Runnable Demo:** Create character, quit client, relaunch, spawn as same character.

## External Dependencies

- REQ-042 auth token for API calls
- AccountService API (new or extended from auth repo)

## Assumptions

- All four factions selectable at creation; only Human starter zone fully built initially.
- Appearance customization deferred.

## Open Questions

- [ ] Multiple characters per account — defer to post-Tier F REQ?

## Out of Scope

- Character deletion / rename
- Cross-server character transfer
- Ship loadout persistence (REQ-044)

## Retrieved Context

No prior context retrieved — no tagged documents matched this area.
