---
id: REQ-050
title: "Station Docking & Loadout UI"
status: draft
deployable: true
created: 2026-06-11
updated: 2026-06-11
component: "game/ui/StationMenu"
domain: "ui"
stack: ["unreal", "cpp", "umg"]
concerns: ["testability", "developer-experience"]
tags: ["station", "docking", "loadout-ui", "tier-f"]
---

## Description

Add a station nav point in each sector: approach → dock request → station UI for ship loadout (equip weapon/shield from starter inventory), hull upgrade entry point (REQ-047), and mission board (REQ-049). Undock returns to flight mode.

**Why:** Closes Tier F vertical slice — players can dock, reconfigure, take mission, undock, travel, fight — matching E&B station loop at prototype fidelity.

**Depends on:** REQ-044, REQ-047, REQ-049, REQ-034

**Runnable Demo:** Dock at station → swap beam variant in loadout UI → accept mission → undock → complete mission → redock with credits displayed.

Reference: `docs/05-architecture.md` Station UI menu tree; `docs/06-multiplayer-gameplay.md` station services subset.

## System Model

### Entities

| Entity | Field | Type | Constraints |
|--------|-------|------|-------------|
| Station | station_id | string | per sector |
| DockingZone | radius | float | trigger dock prompt |
| StationUI | mode | enum | loadout/missions/upgrade/undock |
| StarterInventory | items | Item[] | TL1-TL3 templates for testing |

### Events

| Event | Trigger | Payload |
|-------|---------|---------|
| docked | Ship in zone + confirm | `{ character_id, station_id }` |
| undocked | Player selects undock | `{ character_id }` |
| loadout_changed | Equip in UI | `{ slots }` persists REQ-044 |

### Permissions

| Action | Roles Allowed |
|--------|---------------|
| dock | ship owner in range |
| modify_loadout | docked owner only |

## Business Rules

- [ ] BR-1: Cannot dock while in combat (same flag as jump gate REQ-048).
- [ ] BR-2: While docked, ship invulnerable and immobile; UI mode replaces flight HUD.
- [ ] BR-3: Loadout UI lists equipped slots and available starter inventory items only (no auction).
- [ ] BR-4: Mission board shows available templates from REQ-049.
- [ ] BR-5: Undock spawns ship at designated undock point with clear space check.

## Acceptance Criteria

- [ ] Station exists in Sector A (and B if REQ-048 complete) with visible nav marker.
- [ ] Dock/undock flow works in multiplayer without desync.
- [ ] Loadout UI equips items and persists per REQ-044.
- [ ] Mission accept from station UI functional.
- [ ] Hull upgrade accessible from station (REQ-047).
- [ ] **Runnable Demo:** Full Tier F loop: login → dock → equip → mission → undock → travel (optional) → fight → complete → dock.

## External Dependencies

- REQ-044 loadout persistence
- REQ-047 hull upgrade
- REQ-049 mission board
- REQ-034 UI patterns

## Assumptions

- Vendor, auction, crafting tabs shown disabled or hidden.
- 3D hangar ship view optional (2D UI sufficient).

## Open Questions

- [ ] Allow dock in Sector B before economy vendors exist?

## Out of Scope

- Auction house, crafting station, guild UI
- NPC vendor inventory
- Capital ship hangar

## Retrieved Context

No prior context retrieved — no tagged documents matched this area.
