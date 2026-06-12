---
id: REQ-044
title: "Ship Loadout Save & Restore"
status: draft
deployable: true
created: 2026-06-11
updated: 2026-06-11
component: "backend/AccountService"
domain: "data"
stack: ["postgresql", "unreal", "cpp"]
concerns: ["reliability", "testability"]
tags: ["loadout", "equipment", "persistence", "tier-d"]
---

## Description

Persist equipped weapons, shields, hull tier, and power allocation defaults to `ship_loadouts` / `items` tables and restore on character spawn.

**Why:** Players must keep ship build across sessions before economy/crafting exists.

**Depends on:** REQ-043, REQ-039, REQ-040

**Runnable Demo:** Equip beam weapon, set power preset, logout — login — spawn with same equipment and allocation.

Reference: `docs/05-architecture.md` ship_loadouts, items schema; `docs/07-assets.md` modular slots.

## System Model

### Entities

| Entity | Field | Type | Constraints |
|--------|-------|------|-------------|
| ShipLoadout | slot_type | enum | weapon/shield/engine/reactor |
| ShipLoadout | slot_index | int | per character |
| ShipLoadout | item_id | UUID | FK nullable |
| Item | template_id | string | DataTable row reference |
| Item | tech_level | int | 1-9 |
| Item | stats | JSONB | rolled stats optional |

### Events

| Event | Trigger | Payload |
|-------|---------|---------|
| loadout_saved | equip change or logout | `{ character_id, slots[] }` |
| loadout_applied | character spawn | `{ character_id, resolved_items[] }` |

### Permissions

| Action | Roles Allowed |
|--------|---------------|
| modify_loadout | character owner |
| apply_loadout | game server on spawn |

## Business Rules

- [ ] BR-1: Server applies loadout on spawn; client cannot spoof equipped items.
- [ ] BR-2: Prototype supports one weapon slot + default hull/shield/engine from starter kit.
- [ ] BR-3: Missing/corrupt loadout rows fall back to faction starter kit (logged).
- [ ] BR-4: Loadout changes saved on equip action and on graceful disconnect.
- [ ] BR-5: Item condition/depreciation (docs/07) deferred — items always 100% condition.

## Acceptance Criteria

- [ ] Equipping TL1 beam persists to DB and survives reconnect.
- [ ] Spawn applies DataTable stats from saved template_id.
- [ ] Integration test: save loadout, reload character, assert weapon component matches.
- [ ] **Runnable Demo:** REQ-043 demo + verify beam still equipped after relog.

## External Dependencies

- REQ-043 character row
- REQ-039 weapon component

## Assumptions

- Inventory/cargo not persisted beyond equipped slots in this REQ.
- Bank storage deferred.

## Open Questions

- [ ] Save power allocation defaults with loadout?

## Out of Scope

- Inventory grid UI
- Auction/trade changing ownership
- Asset depreciation

## Retrieved Context

No prior context retrieved — no tagged documents matched this area.
