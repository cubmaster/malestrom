# Iron Exiles - Multiplayer Gameplay Design

## Game Design Document: Core Gameplay Loop (Earth & Beyond Style)

---

## Overview

This is a **space MMO** inspired by the gameplay of *Earth and Beyond* (2002) set in the *Iron Exiles* universe. Players select a faction/race, receive a starter ship with an AI companion, and progress through missions, combat, trade, and exploration across a persistent galaxy.

---

## Race & Class Selection

### Playable Factions

At game start, the player selects a faction. Each faction provides:
- A unique starter ship with distinct stats
- A faction-specific AI personality
- Unique skill trees and abilities
- Different starting zones and storylines
- Visual ship aesthetics

| Faction | Playstyle Focus | Ship Traits | Starting Zone |
|---------|----------------|-------------|---------------|
| **Human** | Adaptable / Balanced | Jack-of-all-trades, AI is strongest | Sol System / Paradise |
| **Velnari** | Tank / Trade | Heavy shields, large cargo | Velnari Border Systems |
| **Kethari** | Combat / Aggression | Fast, high damage, light armor | Kethari Warlord Space |
| **Oruneti** | Tech / Drones | Drone support, hacking, sensors | Oruneti Automated Sector |

### Classes (Per Faction)

Each faction has **3 class archetypes** (like Earth & Beyond's Warrior/Trader/Explorer):

| Class | Focus | XP Source | Key Skills |
|-------|-------|-----------|------------|
| **Warrior** | Combat | Killing enemies, PvP, missions | Weapons, shields, tactics |
| **Trader** | Economy | Crafting, hauling, manufacturing | Cargo, refining, negotiation |
| **Explorer** | Discovery | Scanning, mapping, finding artifacts | Sensors, cloaking, hacking |

**Example Combinations:**
- Human Warrior = "Pirate" (Merry Band of Pirates style)
- Velnari Trader = "Merchant Fleet Captain"
- Kethari Warrior = "Warlord"
- Oruneti Explorer = "Recon Drone Commander"

---

## Starting Experience

### Character Creation Flow

```
1. Select Faction (Human / Velnari / Kethari / Oruneti)
2. Select Class (Warrior / Trader / Explorer)
3. Customize Appearance (avatar for station interactions)
4. Name Ship & AI
5. Tutorial Mission (faction-specific intro)
6. Enter Open Galaxy
```

### Starter Ship Stats (by Faction)

| Stat | Human | Velnari | Kethari | Oruneti |
|------|-------|-------|----------|----------|
| Hull HP | 100 | 80 | 90 | 70 |
| Shield HP | 100 | 150 | 80 | 100 |
| Speed | 100 | 70 | 120 | 90 |
| Cargo | 50 | 80 | 30 | 40 |
| Weapon Slots | 2 | 2 | 3 | 1 |
| Device Slots | 2 | 2 | 1 | 3 |
| Tech Level | TL1 | TL1 | TL1 | TL1 |

---

## The Ship AI System

Every player has a **Ship AI companion** - the game's signature feature (inspired by Kestrel). The AI grows with the player.

### AI Personality

The AI has a personality based on faction, but evolves over time:

| Faction | AI Personality | Voice Style |
|---------|---------------|-------------|
| Human | Sarcastic, witty (Kestrel-like) | Comedic, fourth-wall-breaking |
| Velnari | Cautious, nurturing | Calm, advisory |
| Kethari | Aggressive, boastful | Warrior bravado, taunting |
| Oruneti | Analytical, precise | Robotic, data-focused |

### AI Capabilities (Level-Based)

The AI has its own **experience level** (1-50) that unlocks abilities:

| AI Level | Tier | Unlocked Capabilities |
|----------|------|----------------------|
| 1-10 | Basic | Navigation assist, basic targeting, simple comms |
| 11-20 | Improved | Auto-repair (slow), ECM basics, threat warnings |
| 21-30 | Advanced | System hacking, jump calculation boost, shield tuning |
| 31-40 | Superior | Multi-target tracking, fleet coordination, stealth assist |
| 41-50 | Elite | Advanced hacking, wormhole detection, combat prediction |

### AI Ability Slots

Players choose which AI abilities are active (limited slots):

```
AI Level 1-10:   2 ability slots
AI Level 11-20:  3 ability slots
AI Level 21-30:  4 ability slots
AI Level 31-40:  5 ability slots
AI Level 41-50:  6 ability slots
```

### AI Abilities List

| Ability | Type | Effect | Unlock Level |
|---------|------|--------|-------------|
| Target Lock Assist | Passive | +15% weapon accuracy | 1 |
| Nav Optimization | Passive | +10% travel speed | 1 |
| Shield Monitor | Passive | +5% shield regen | 5 |
| Threat Alert | Passive | Warns of incoming enemies | 5 |
| Emergency Repair | Active | Repairs 10% hull over 30s | 10 |
| Sensor Boost | Active | 2x detection range for 20s | 12 |
| ECM Burst | Active | Break all enemy target locks | 15 |
| Comm Intercept | Active | See enemy chat/orders for 15s | 18 |
| Shield Harmonic | Active | +50% shield resist for 10s | 22 |
| System Hack | Active | Disable one enemy system 8s | 25 |
| Jump Assist | Active | -50% jump cooldown for next jump | 28 |
| Cloak Detection | Passive | Reveal cloaked ships in range | 30 |
| Nano Repair | Active | Repair 25% hull over 20s | 32 |
| Fleet Sync | Active | Buff all group members +10% | 35 |
| Combat Prediction | Passive | Shows enemy attack patterns | 38 |
| Wormhole Sense | Passive | Detect hidden wormholes | 40 |
| Master Hack | Active | Disable all enemy systems 5s | 45 |
| Overdrive | Active | Double all ship stats for 10s (dangerous) | 50 |

### AI Leveling

The AI gains XP from:
- Completing missions (any type)
- Discovering new sectors/anomalies
- Using AI abilities successfully
- Player reaching level milestones
- Finding Elder tech artifacts (bonus XP)

---

## Three-Pillar XP System (Earth & Beyond Style)

### Experience Categories

Like Earth & Beyond, players earn XP in three separate tracks:

| XP Type | Earned From | Rewards |
|---------|-------------|---------|
| **Combat XP** | Killing enemies, PvP, combat missions | Weapon skills, combat abilities, hull upgrades |
| **Trade XP** | Crafting, hauling cargo, selling goods | Manufacturing skills, cargo capacity, trade routes |
| **Explore XP** | Discovering locations, scanning, artifacts | Sensor skills, cloaking, jump range, map reveals |

### Overall Level

```
Overall Level = Combat Level + Trade Level + Explore Level
Max per category: 50
Max overall: 150
```

### Level Benefits

| Level Milestone | Reward |
|----------------|--------|
| Every 5 levels (any track) | 1 Skill Point |
| Every 10 levels (any track) | New equipment slot |
| Every 15 levels (Combat) | Hull upgrade (visual + stats) |
| Every 15 levels (Trade) | Cargo expansion |
| Every 15 levels (Explore) | Sensor/Jump upgrade |
| Overall 30 | Class specialization unlock |
| Overall 75 | Advanced ship unlock |
| Overall 100 | Capital ship access |
| Overall 150 | Elder tech integration |

---

## Ship Progression System

### Hull Upgrades (Visual + Stat Progression)

Ships visually evolve as players progress (like Earth & Beyond):

| Hull Tier | Combat Level Required | Changes |
|-----------|----------------------|---------|
| Tier 1 (Starter) | 1 | Basic ship, minimal slots |
| Tier 2 | 10 | Slightly larger, +1 weapon slot |
| Tier 3 | 20 | Visible upgrades, +1 device slot, new abilities |
| Tier 4 | 30 | Major visual overhaul, larger ship class |
| Tier 5 | 40 | Faction flagship appearance, all slots unlocked |
| Tier 6 (Endgame) | 50 | Unique elite appearance, max stats |

### Equipment Tech Levels

All gear has a Tech Level (TL1-TL9), restricting what you can equip:

| Tech Level | Required Overall Level | Quality |
|------------|----------------------|---------|
| TL1 | 1 | Starter/Common |
| TL2 | 15 | Improved |
| TL3 | 30 | Standard |
| TL4 | 45 | Advanced |
| TL5 | 60 | Superior |
| TL6 | 75 | Elite |
| TL7 | 90 | Faction Prototype |
| TL8 | 120 | Experimental |
| TL9 | 140 | Elder-Enhanced |

### Equipment Slots

| Slot Type | Starting | Max (Tier 6) | Function |
|-----------|----------|--------------|----------|
| Weapons | 2 | 6-8 | Offensive armament |
| Shields | 1 | 2-3 | Defensive barriers |
| Devices | 2 | 5-7 | Utility/abilities |
| Engine | 1 | 1-2 | Speed/maneuver |
| Reactor | 1 | 1-2 | Power generation |
| Cargo | 1 | 3-5 | Storage capacity |
| AI Module | 1 | 3 | AI enhancement chips |

---

## Combat System

### Real-Time Space Combat (E&B Style)

Combat is **real-time 3D flight** with direct ship control:

| Mechanic | Description |
|----------|-------------|
| Flight | 6DOF (six degrees of freedom) ship movement |
| Targeting | Tab-target or free-aim depending on weapon type |
| Weapons | Fire with hotkeys, manage energy/ammo |
| Shields | Auto-absorb damage, can redirect facing |
| Abilities | Class + AI abilities on cooldown bars |
| Dodge | Manual evasion of missiles/torpedoes |

### Combat Flow

```
1. Detection (sensors/AI alerts you to hostile)
2. Approach (maneuver into weapon range)
3. Engage (fire weapons, activate abilities)
4. Manage (power allocation, shield facing, AI commands)
5. Resolve (enemy destroyed/fled, or disengage yourself)
6. Loot (salvage wreckage for materials/gear)
```

### Power Management (Real-Time)

```
Reactor Output: [████████████████████] 100%

Distribute between:
├── Weapons:  [████████░░] 40% → Affects fire rate & damage
├── Shields:  [██████░░░░] 30% → Affects regen speed
├── Engines:  [████░░░░░░] 20% → Affects speed & maneuver
└── AI/ECM:   [██░░░░░░░░] 10% → Affects AI ability power
```

### Weapon Types

| Type | Behavior | Best Class | Ammo |
|------|----------|-----------|------|
| Beam | Instant hit, sustained | Oruneti | Energy (infinite) |
| Projectile | Fast travel, burst | Kethari | Magazines (buy/craft) |
| Missile | Lock-on, guided | Human | Limited stock |
| Plasma | AoE splash | Velnari | Energy (heavy drain) |
| Drone | Autonomous fighters | Oruneti | Deployable units |

### PvP Rules

| Zone Type | PvP Rules |
|-----------|-----------|
| Safe Zones (Stations) | No PvP |
| Faction Space | PvP only vs rival factions |
| Contested Zones | Free-for-all PvP |
| Wormhole Space | Unrestricted, high risk/reward |
| Dueling | Consensual 1v1 anywhere |

---

## Mission System

### Mission Types (E&B Style)

| Mission Type | Description | XP Reward |
|--------------|-------------|-----------|
| **Story Missions** | Main narrative arc per faction | All types |
| **Combat Missions** | Kill targets, defend areas, assault bases | Combat XP |
| **Trade Missions** | Deliver cargo, craft items, supply stations | Trade XP |
| **Explore Missions** | Chart systems, scan anomalies, find artifacts | Explore XP |
| **Faction Missions** | Reputation grinding with NPC factions | Mixed |
| **Group Missions** | Require party (raids/fleet actions) | Bonus all |
| **Daily/Repeatable** | Quick tasks for currency/XP | Varies |

### Mission Difficulty Scaling

| Difficulty | Solo/Group | Reward Multiplier |
|------------|-----------|-------------------|
| Normal | Solo | 1x |
| Hard | Solo or 2-3 | 1.5x |
| Elite | Group (4-6) | 2.5x |
| Raid | Fleet (8-12) | 4x |

### Faction Reputation System

Players build reputation with NPC factions for rewards:

| Rep Level | Status | Benefit |
|-----------|--------|---------|
| -100 to -50 | Hostile | Attacked on sight |
| -50 to 0 | Unfriendly | No services |
| 0 to 25 | Neutral | Basic trade |
| 25 to 50 | Friendly | Missions available |
| 50 to 75 | Allied | Discount pricing, rare gear |
| 75 to 100 | Exalted | Unique ships, AI upgrades, lore |

---

## Trade & Economy

### Resource Gathering

| Activity | Resources Gained |
|----------|-----------------|
| Asteroid Mining | Raw ores, crystals |
| Wreck Salvage | Components, scrap, rare drops |
| Anomaly Scanning | Data cores, Elder fragments |
| Mission Rewards | Credits, materials, gear |

### Crafting System

```
Raw Materials → Refine → Components → Assemble → Equipment

Example:
  Titanium Ore → Refined Titanium → Hull Plating → TL3 Armor Module
```

| Craft Category | Output |
|----------------|--------|
| Weapons Manufacturing | Guns, missiles, beam arrays |
| Shield Engineering | Shield generators, capacitors |
| Engine Assembly | Thrusters, afterburners |
| Device Fabrication | ECM units, repair bots, scanners |
| AI Chip Programming | AI ability enhancers |

### Player Economy

- **Auction House:** Player-to-player trade at stations
- **Trade Routes:** Buy low in one system, sell high in another
- **Contracts:** Players post jobs for others (escort, delivery, crafting)
- **Guild Stations:** Player guilds can own/operate trade stations

---

## Exploration System

### Discovery Mechanics

| Activity | Explore XP | Reward |
|----------|-----------|--------|
| Enter new sector | Medium | Map reveal, lore entry |
| Scan anomaly | Medium | Data/materials |
| Find hidden nav point | High | Rare spawns, shortcuts |
| Discover wormhole | Very High | New travel route for all |
| Locate Elder site | Extreme | AI upgrades, TL9 components |

### Scanning Gameplay

Players actively scan space for hidden content:

```
[Scanning...] ░░░░░░░░░░ 0%
[Scanning...] ████░░░░░░ 40% - Signal detected!
[Scanning...] ████████░░ 80% - Anomaly classified: Elder Fragment
[Complete!]   ██████████ 100% - Location marked. Explore XP +500
```

### The Map

- Galaxy divided into **Sectors** (safe, contested, hostile, unknown)
- Each sector has multiple **Nav Points** (stations, planets, fields)
- **Fog of War** on unexplored sectors
- Explored sectors show faction control, resources, hazards

---

## Group & Social Systems

### Group Play

| Feature | Details |
|---------|---------|
| Party Size | 2-6 players |
| Fleet Size | Up to 12 (raids) |
| Role Trinity | Tank (Velnari), DPS (Kethari), Support (Human/Oruneti) |
| Group XP | Shared, with bonus for diversity |
| Loot | Need/Greed roll or round-robin |

### Guilds (Fleets)

| Feature | Details |
|---------|---------|
| Max Size | 100 players |
| Guild Ships | Shared capital ship (guild progresses it) |
| Territory | Guilds can claim systems in contested space |
| Guild Missions | Cooperative multi-week objectives |
| Guild AI | Shared advanced AI for guild operations |

### Cross-Faction Play

- Factions are **rival** but not permanently enemies
- **Contested zones** are free PvP
- **Neutral stations** allow cross-faction trade/social
- **Temporary alliances** for raid content (shifting political alliances)

---

## Endgame Content

| Content | Description |
|---------|-------------|
| Elder Raids | 12-player fleet vs Elder automated defenses |
| Faction Wars | Guild vs Guild territory control |
| Wormhole Exploration | Procedural dangerous space with best loot |
| Capital Ship Command | Upgrade guild capital ship, fleet battles |
| AI Ascension | Final AI evolution quest chain (level 50 AI) |
| Vorathis Incursions | Server-wide events, all factions cooperate |
| Season Content | Rotating story arcs, limited-time missions |

---

## Multiplayer Architecture

### Server Structure

```
Galaxy Server (Persistent World)
├── Sector Instances (50-100 players per sector)
│   ├── PvE Zones (instanced missions)
│   ├── PvP Zones (open world)
│   └── Station Interiors (social hubs)
├── Matchmaking Service (PvP arena, group finder)
├── Economy Server (auction house, trade)
└── Guild/Social Server (chat, fleet management)
```

### Session Types

| Type | Players | Description |
|------|---------|-------------|
| Open World | 50-100 per sector | Persistent shared space |
| Instanced Mission | 1-6 | Private mission zone |
| Fleet Battle | 12 vs 12 | PvP arena |
| Raid Instance | 8-12 | Cooperative PvE |
| Station | 50+ | Social/trade hub |

---

## Monetization Model (Recommended)

| Model | Details |
|-------|---------|
| Base Game | Buy-to-play ($40-60) |
| Expansions | Major content packs ($20-30) |
| Cosmetics | Ship skins, AI voice packs, effects |
| NOT Pay-to-Win | No stat-boosting purchases |

---

## Summary: Core Gameplay Loop

```
┌─────────────────────────────────────────────────────────┐
│                    CORE LOOP                              │
│                                                          │
│  Select Mission → Travel to Location → Complete Task     │
│       ↓                                                  │
│  Earn XP (Combat/Trade/Explore) + Loot + Credits        │
│       ↓                                                  │
│  Return to Station → Upgrade Ship/AI/Gear               │
│       ↓                                                  │
│  Unlock New Content (Zones/Missions/Abilities)          │
│       ↓                                                  │
│  Repeat at Higher Tier ←────────────────────────────────│
│                                                          │
│  SOCIAL: Group up → Tackle harder content → Guild play  │
│  PVP: Enter contested space → Fight rivals → Claim zone │
└─────────────────────────────────────────────────────────┘
```

---

## Design Pillars

1. **"Punch Above Your Weight"** - Smart play beats gear score
2. **Your AI is Your Identity** - It grows with you, becomes unique
3. **Three Paths, One Galaxy** - Combat, Trade, Explore all viable
4. **Faction Pride** - Your race matters, but cooperation is possible
5. **Always Something to Do** - Solo, group, or PvP content at every level
