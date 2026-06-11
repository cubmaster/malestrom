# Iron Exiles - Planets & Locations

## Game Design Document: Star Systems & Points of Interest

---

## Key Locations

### Earth (Sol System)

| Attribute | Details |
|-----------|---------|
| System | Sol |
| Status | Occupied/contested (early game), liberated (late game) |
| Faction Control | Initially Kethari-occupied |
| Game Role | Player origin, story anchor, liberation objective |

**Gameplay Notes:**
- Tutorial/prologue location
- Returns as a major story objective (liberation campaign)
- Provides context for why humans are fighting
- Home base for resupply in late game

---

### Paradise (Colony World)

| Attribute | Details |
|-----------|---------|
| System | Unnamed alien star system |
| Status | Human colony on alien world |
| Faction Control | Contested between Velnari and Kethari, humans caught in between |
| Game Role | Secondary home base, ground combat missions, diplomacy hub |

**Gameplay Notes:**
- Hub world for side missions and crew recruitment
- Ground-based missions (infantry/vehicle combat variety)
- Diplomatic interactions with Velnari NPCs
- Resource gathering and trade
- Atmospheric flight combat possible

---

### Wormhole Network

| Attribute | Details |
|-----------|---------|
| Type | Permanent spacetime tunnels |
| Creator | Elders |
| Status | Some known, many hidden |
| Strategic Value | Extreme - controls galactic travel |

**Gameplay Notes:**
- Wormholes serve as "highways" between star systems
- Control of wormholes = control of territory
- Hidden wormholes are discoverable (exploration reward)
- Wormhole battles are chokepoint engagements
- Kestrel can detect/manipulate wormholes (unique ability)

---

## Star System Types (Procedural & Handcrafted Mix)

### System Categories

| Type | Description | Gameplay Purpose |
|------|-------------|-----------------|
| Core Worlds | Faction homeworlds, heavily defended | Story missions, endgame content |
| Colony Systems | Settled worlds, moderate defense | Side quests, trade, crew |
| Frontier Systems | Lightly settled or empty | Exploration, scavenging |
| Contested Zones | Active war zones | Fleet battles, dynamic events |
| Dead Systems | Abandoned/destroyed | Loot, lore, mystery |
| Elder Sites | Ancient ruins/artifacts | High-risk, high-reward exploration |
| Wormhole Nexus | Hub of multiple wormhole connections | Strategic chokepoints |

---

## Named Locations by Faction

### Human-Controlled Space

| Location | Type | Notes |
|----------|------|-------|
| Earth / Sol | Homeworld | Starting point, liberation objective |
| Paradise | Colony | Main quest hub, ground missions |
| Camp Alpha | Military Base | Crew upgrades, mission briefings |

### Velnari Space

| Location | Type | Notes |
|----------|------|-------|
| Velnari Homeworld | Core World | Diplomatic missions, ally content |
| Velnari Border Systems | Frontier | Patrol missions, joint operations |
| Trading Posts | Stations | Ship upgrades, intel purchases |

### Kethari Space

| Location | Type | Notes |
|----------|------|-------|
| Kethari Homeworld | Core World | Late-game assault target |
| Warlord Systems | Colony | Clan-specific combat encounters |
| Raiding Bases | Military | Stealth missions, sabotage |

### Vorathis Space

| Location | Type | Notes |
|----------|------|-------|
| Vorathis Core | Core World | Endgame territory, extreme danger |
| Shadow Fleets (mobile) | Fleet | Roaming boss encounters |
| Research Stations | Facility | Stealth heist missions for tech |

### Oruneti Space

| Location | Type | Notes |
|----------|------|-------|
| Oruneti Systems | Colony | Drone-heavy encounters |
| Automated Shipyards | Facility | Sabotage/capture missions |

### Elder Sites (Scattered)

| Location | Type | Notes |
|----------|------|-------|
| Kestrel First Contact | Ruin | Tutorial/lore location |
| Elder Weapons Cache | Ruin | Major upgrade rewards |
| Dormant AI Nodes | Ruin | Story-critical, Kestrel-related |
| Wormhole Control Stations | Ruin | Game-changing strategic assets |

---

## Space Environment Types

### Combat Arenas

| Environment | Tactical Effect |
|-------------|----------------|
| Open Space | No cover, pure maneuvering |
| Asteroid Field | Cover, collision hazard, ambush potential |
| Nebula | Sensor disruption, reduced visibility |
| Debris Field | Salvage opportunity, navigation hazard |
| Near Planet/Moon | Gravity effects, atmospheric entry option |
| Wormhole Proximity | Unstable space, jump interference |
| Star Corona | Radiation damage, sensor blindness |

### Environmental Hazards

| Hazard | Effect |
|--------|--------|
| Radiation Zones | Ongoing hull/shield damage |
| Gravity Wells | Movement penalties, jump prevention |
| Minefields | Explosive obstacles |
| Solar Flares | Periodic massive damage waves |
| Wormhole Instability | Random teleportation, ship damage |

---

## Galaxy Map Design

### Structure
```
Galaxy Map (Strategic Layer)
├── Sectors (Large regions owned by factions)
│   ├── Star Systems (Individual combat/exploration zones)
│   │   ├── Planets (Ground missions, orbital combat)
│   │   ├── Stations (Docking, trade, upgrades)
│   │   ├── Asteroid Belts (Mining, ambushes)
│   │   └── Points of Interest (Events, lore)
│   └── Wormhole Connections (Fast travel between sectors)
└── Deep Space (Between systems - random encounters)
```

### Navigation
- **Jump Drive:** Travel within a sector (short jumps, quick cooldown)
- **Wormholes:** Travel between sectors (instant but fixed routes)
- **Long Jump:** Travel to distant systems (long cooldown, Kestrel-assisted)

---

## Dynamic World Events

| Event Type | Description |
|------------|-------------|
| Fleet Movements | Faction fleets patrol and respond to player actions |
| Territorial Shifts | Factions gain/lose systems based on war progress |
| Raids | Kethari raiding parties attack random systems |
| Discoveries | New Elder sites revealed through story/exploration |
| Wormhole Changes | Some wormholes activate/deactivate over time |
| Refugee Convoys | Protection missions with moral choices |

---

## Design Notes

- The galaxy should feel alive with faction activity independent of the player
- Wormhole control creates strategic depth beyond just combat
- Mix of handcrafted story locations and procedural exploration content
- Environmental variety keeps combat encounters fresh
- Elder sites are the "dungeons" of the game - high risk, high reward
- Paradise provides a contrast to space combat (ground missions, breathing room)
