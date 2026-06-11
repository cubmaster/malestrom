# Iron Exiles - Races & Factions

## Game Design Document: Species & Faction Reference

---

## Galactic Hierarchy (Power Tier System)

The universe operates on a strict power hierarchy. In-game, this translates to technology tiers that determine ship capabilities, weapon effectiveness, and available upgrades.

```
Tier 0: Elders (Extinct - Tech remains as artifacts/MacGuffins)
Tier 1: Vexari (Spider-like apex predators)
Tier 2: Vorathis (large Cat-like overlords)
Tier 3: Oruneti / Velnari (Client races of Tier 1-2)
Tier 4: Kethari (Lizard warriors, client of Vorathis)
Tier 5: Humans (Newcomers, wildcard faction)
```

---

## Faction Details

### 1. Elders (Tier 0 - Environmental/Lore Only)

| Attribute | Details |
|-----------|---------|
| Status | Extinct |
| Role in Game | Their technology forms artifacts, quest objectives, and rare upgrades |
| Technology | God-tier - wormhole manipulation, AI creation, reality-bending |

**Game Relevance:**
- Elder artifacts are ultimate power-ups/quest rewards
- Elder sites are high-risk exploration zones
- Kestrel (Elder AI) is a key gameplay mechanic - an AI companion providing tactical advantages

---

### 2. Vexari (Tier 1)

| Attribute | Details |
|-----------|---------|
| Appearance | Insectoid/Spider-like |
| Personality | Cunning manipulators, never fight directly |
| Combat Style | Psychological warfare, electronic warfare, proxy conflicts |
| Ship Aesthetic | Organic, unsettling, asymmetrical designs |
| Strengths | EW superiority, stealth, hive coordination |
| Weaknesses | Rarely commit to direct engagement |

**Game Mechanics:**
- Vexari ships specialize in debuffs, jamming, and disruption
- Encounters with Vexari are rare and terrifying "boss-level" events
- Their ships use hive-mind coordination (AI-controlled fleet synergy)

---

### 3. Vorathis (Tier 2)

| Attribute | Details |
|-----------|---------|
| Appearance | Avian/Reptilian bipeds with beaked features |
| Personality | Arrogant, honor-bound, see themselves as rightful rulers |
| Combat Style | Overwhelming stealth + firepower |
| Ship Aesthetic | Sleek, angular, dark materials, stealth-focused |
| Strengths | Advanced stealth, devastating weapons, shield technology |
| Weaknesses | Arrogance leads to underestimating opponents |

**Game Mechanics:**
- Primary antagonist faction for mid-to-late game
- Ships decloak and alpha-strike - high burst damage
- Player must develop counter-stealth capabilities to fight them
- Honor system: some diplomatic options available

---

### 4. Velnari (Tier 3) - "Tunnelers"

| Attribute | Details |
|-----------|---------|
| Appearance | Hamster/rodent-like |
| Personality | Pragmatic, defensive-minded, somewhat friendly |
| Combat Style | Tank-and-shield, attrition warfare |
| Ship Aesthetic | Bulky, rounded, heavily armored |
| Strengths | Massive shields, armor, endurance in prolonged fights |
| Weaknesses | Slower ships, less offensive punch |

**Game Mechanics:**
- Potential ally faction
- Velnari ships are "tanks" - high HP, high shields, slow
- Trade and diplomacy quests available
- Their territory is a relatively safe zone early-game

---

### 5. Kethari (Tier 4) - "Lizards"

| Attribute | Details |
|-----------|---------|
| Appearance | Reptilian/Lizard-like |
| Personality | Aggressive, expansionist, warlord culture |
| Combat Style | Aggressive swarm tactics, overwhelming numbers |
| Ship Aesthetic | Angular, predatory, sea-creature naming conventions |
| Strengths | Numbers, aggression, boarding actions |
| Weaknesses | Individually inferior tech, poor coordination between clans |

**Game Mechanics:**
- Early-game antagonist (tutorial enemies)
- Swarm AI behavior - attack in packs
- Boarding mechanics when fighting Kethari
- Internal clan rivalries can be exploited diplomatically
- Source of initial stolen ships for player

---

### 6. Oruneti (Tier 3)

| Attribute | Details |
|-----------|---------|
| Appearance | Amphibious/frog-like |
| Personality | Bureaucratic, risk-averse, technical |
| Combat Style | Calculated, use drones and automated systems |
| Ship Aesthetic | Functional, modular, drone-carrier focused |
| Strengths | Drone warfare, automation, logistics |
| Weaknesses | Avoid direct combat, brittle when plans fail |

**Game Mechanics:**
- Drone-carrier enemies - destroy the mothership to disable drones
- Hacking opportunities against their automated systems
- Intelligence-gathering missions in their space

---

### 7. Humans (Tier 5) - Player Faction

| Attribute | Details |
|-----------|---------|
| Appearance | Human |
| Personality | Resourceful, unpredictable, underestimated |
| Combat Style | Asymmetric warfare, improvisation, Kestrel-enhanced |
| Ship Aesthetic | Kitbashed alien tech, jury-rigged, evolving |
| Strengths | Adaptability, Kestrel AI companion, creativity |
| Weaknesses | Inferior base technology, limited resources |

**Game Mechanics:**
- Player starts with stolen/salvaged ships
- Progression through scavenging and upgrading with alien tech
- Kestrel provides unique abilities (hacking, navigation, ECM)
- "Punch above your weight" gameplay loop
- Deception and trickery as core tactical options

---

## Faction Relationships Matrix

| | Vexari | Vorathis | Velnari | Kethari | Oruneti | Humans |
|---|---|---|---|---|---|---|
| **Vexari** | - | Cold War | Allied | Enemy | Client | Ignored |
| **Vorathis** | Cold War | - | Enemy | Client | Enemy | Hostile |
| **Velnari** | Allied | Enemy | - | War | Neutral | Complex |
| **Kethari** | Enemy | Client | War | - | Rivals | Hostile |
| **Oruneti** | Client | Enemy | Neutral | Rivals | - | Hostile |
| **Humans** | Ignored | Hostile | Complex | Hostile | Hostile | - |

---

## Kestrel the Magnificent (Elder AI)

### Role in Gameplay

Kestrel functions as a core gameplay system - not just a companion character.

| System | Function |
|--------|----------|
| Navigation | Calculates jump routes, finds hidden wormholes |
| Electronic Warfare | Hacks enemy ships, disrupts comms |
| Ship Enhancement | Upgrades weapons/shields beyond normal capability |
| Intelligence | Translates, intercepts communications, identifies threats |
| Tactical Advisor | Suggests combat strategies (optional hints system) |

### Limitations (Game Balance)

- **Energy dependent** - abilities drain ship power
- **Cooldowns** - major abilities have significant cooldown timers
- **Mood system** - Kestrel's willingness to help varies (narrative mechanic)
- **Cannot create matter** - needs materials for repairs/upgrades
- **Single instance** - if the ship is destroyed, Kestrel may be lost (permadeath stakes)

---

## Design Notes

- Each faction should feel distinctly different to fight against
- The tech tier system creates natural difficulty progression
- Faction relationships enable diplomatic gameplay alongside combat
- Kestrel should feel powerful but not game-breaking (resource management)

