# Iron Exiles - Player Leveling System

## Game Design Document: Experience, Levels, and Rank Advancement

---

## Overview

Players progress from level 1 to level 150, gaining experience through every activity in the galaxy. The system is built around **three class ranks** (one per 50-level block), each representing a major tier of capability and story phase. XP requirements grow exponentially so that early levels come quickly while higher levels demand real dedication.

### Design Goals

- Early game feels fast. Levels 1 to 10 should click by in hours, not days. Players need to see the galaxy quickly.
- Mid game demands commitment. Ranks 2 and 3 are meaningful achievements that require sustained play.
- All three paths matter. Combat, trade, and exploration each offer viable XP at every level. No single path is forced on any class.
- Ranks unlock story gates. Each rank boundary corresponds to a campaign milestone (new zone, new ship tier, faction storyline chapter).

---

## The Exponential Formula

XP required to reach level n+1 from level n:

    XP(n -> n+1) = 50 * 1.068^n

### Sample Values

| Level | XP for next level | Cumulative XP total |
|-------|-------------------|--------------------|
| 1 -> 2   | 50        | 50     |
| 5 -> 6   | 74       | 369    |
| 10 -> 11 | 110      | 1,180  |
| 20 -> 21 | 206      | 5,400  |
| 30 -> 31 | 383      | 16,000 |
| 40 -> 41 | 712      | 43,000 |
| 50 -> 51 | 1,324    | 109,000|
| 60 -> 61 | 2,459    | 235,000|
| 70 -> 71 | 4,574    | 453,000|
| 80 -> 81 | 8,512    | 800,000|
| 90 -> 91 | 15,836   | 1.4M   |
| 100 -> 101| 29,478  | 2.5M   |
| 110 -> 111| 54,841  | 4.5M   |
| 120 -> 121| 102,064 | 7.8M   |
| 130 -> 131| 189,870 | 13.5M  |
| 140 -> 141| 353,220 | 24M    |
| 149 -> 150| 657,153 | 44M    |

### Key Numbers

| Metric | Value |
|--------|-------|
| Total XP to reach level 150 | ~44 million |
| XP for final level (149 -> 150) | ~657 thousand |
| Growth factor between levels | x1.068 (about 68% increase per 10 levels) |
| XP to cross from rank 2 to 3 (level 100) | 29,478 in a single step |

### Why Exponential?

An exponential curve means:
- The first **50 levels** (about 109k XP total) represent the tutorial galaxy phase. You get through them relatively quickly to reach real content.
- Levels 51 to 100 require about 3x more XP than the first rank, matching the transition from frigate-capable pilot to cruiser-class commander.
- Levels 101 to 150 demand the most (about 4x rank 2), reflecting that endgame players are already doing high-Xp activities and the final stretch is meant to be a marathon of sustained play rather than a sudden cliff.

---

## Class Ranks

Each rank corresponds to a story phase, a new tier of capabilities, and a reputation threshold with your faction.
### Rank I (Levels 1-50) - Wing

Unlocks at Level 50:

| Category | Unlock |
|----------|--------|
| Ship tier | Tier-3 weapons and armor become purchasable from station vendors |
| Jump range | +1 sector baseline (ships can reach frontier systems) |
| Missions | Faction campaign Arc 1 opens - your AI companion origin story begins |
| Trade routes | Access to inter-system bulk trade contracts |
| AI abilities | Slot 3 becomes active; Tier-2 abilities unlocked |
| Combat | Can enter contested space (PvP zones with reduced matchmaking weight) |
| Ship size | Upgrade path to frigate-class frame opens |

Rank I milestones:

| Level | Milestone |
|-------|-----------|
| 10  | First AI ability slot unlocks; starter ship fully upgraded |
| 20  | Faction reputation Friendly - vendor discounts, side quests |
| 30  | Second weapon hardpoint unlocked on your ship (if slots available) |
| 40  | Can pilot corvette-frame upgrade; Tier-2 equipment purchasable |
| 50  | Rank I complete. Faction rank Ally. Campaign Arc 1 Finale unlocks. |

---

### Rank II (Levels 51-100) - Flight

Unlocks at Level 100:

| Category | Unlock |
|----------|--------|
| Ship tier | Tier-4 equipment available (Vorathis/Elder salvage) |
| Jump range | +2 sectors total; deep-space routes open |
| Missions | Faction campaign Arc 2 - Elder artifact hunting begins |
| Capital ship | Can command a small fleet (up to 3 companion ships in missions) |
| Trade | Manufacturing licenses unlock; player can build/sell goods |
| AI abilities | Slot 5 active; Tier-4 abilities (System Hack, Cloak Detection) |
| Combat | Fleet PvP opens; fleet score calculation begins |

Rank II milestones:

| Level | Milestone |
|-------|-----------|
| 60  | Can pilot destroyer-class frame |
| 70  | Faction reputation Trusted - access to black-market stations |
| 80  | Third weapon hardpoint; Tier-3.5 hybrid equipment (alien-human fusion parts) |
| 90  | Manufacturing license granted; can operate a small refinery or forge |
| 100 | Rank II complete. Faction rank Champion. Campaign Arc 2 Finale unlocks. |

---

### Rank III (Levels 101-150) - Fleet

Unlocks at Level 150:

| Category | Unlock |
|----------|--------|
| Ship tier | Tier-5 (Elder) equipment becomes available via quest only |
| Jump range | +3 sectors; wormhole routes accessible |
| Missions | Faction campaign Arc 3 - endgame story arcs; Elder site expeditions |
| Capital ship | Can command capital ships (battleship/cruiser fleet) |
| Guild/Group | Guild leadership unlocks; can establish guild territory |
| AI abilities | Slot 6 active; Tier-5 ability (Nano Repair, Combat Prediction) |
| Endgame | Elder raids, faction wars, wormhole expeditions available |

Rank III milestones:

| Level | Milestone |
|-------|-----------|
| 120 | Can pilot heavy cruiser or battleship frame |
| 130 | Faction reputation Paragon - unique quests per faction |
| 140 | Elder artifact questline begins; Tier-4.5 experimental equipment |
| 150 | Rank III complete. Maximum level reached. Player is a fleet admiral. |
---

## XP Sources

Every activity grants experience on top of the standard rewards (credits, loot). The base XP values scale by enemy/activity difficulty relative to player level.

### Combat XP

Base XP varies by target tier and relative level:

| Target Type | Relative Level | Base XP | Multiplier |
|------------|---------------|---------|-----------|
| Pirate scout | Same | 15 | x1.0 |
| Kethari patrol | +2 levels | 30 | x1.2 |
| Velnari frigate | +5 levels | 80 | x1.5 |
| Vorathis Hunter-Killer | +8 levels | 200 | x1.8 |
| Elder site guardian | +12 levels | 500 | x2.0 |

Multiplier rules (stack multiplicatively):

| Modifier | Effect |
|----------|--------|
| Level advantage (+3 or more above target) | x0.5 (diminishing returns - do not farm trash) |
| Elite/boss enemy | x2.5 |
| Fleet engagement (multiple enemies in one fight) | +10 XP per additional enemy |
| Mission combat objective | x1.5 bonus to all combat XP in that mission |
| PvP engagement | x1.3 base PvP multiplier; honor system affects final value |

### Foraging / Salvage XP

Scanning and salvaging from the environment:

| Activity | Base XP | Notes |
|----------|---------|-------|
| Scan asteroid (common mineral) | 5 | Routine exploration |
| Scan derelict ship | 15-50 | Scales with derelict estimated age/tier |
| Salvage wreck | 10 per component | Extra XP per salvage event; scales with component tier |
| Discover anomaly | 30-200 | Unknown phenomena; rare but high XP |
| Find artifact fragment | 50-500 | Key to Elder quest chains |

Foraging multiplier: Explorer class receives x1.4 bonus to all foraging XP.

### Trade & Jobs XP

Earning credits and experience through commerce:

| Activity | XP Formula | Notes |
|----------|-----------|-------|
| Complete trade contract | 5 + (cargo value / 200) | Scales with profit margin |
| Manufacture item | 3 + (sell price / 150) | Factory output XP per batch |
| Deliver mission cargo | 8 + (delivery fee / 100) | Includes escort duty bonus |
| Repair NPC ship in station | 10 per repair | Small steady income |
| Run black market route | 20 + (profit / 300) | Higher risk, higher reward |

Trading multiplier: Trader class receives x1.4 bonus to all trade XP.

Warrior class receives a x1.3 bonus to all combat XP (stacks multiplicatively with other modifiers).
---

## Stat Growth Per Level

As players level, their ship stats scale proportionally to keep combat meaningful without requiring gear upgrades alone.

### Core Stat Scaling (per level)

| Stat | Growth per Level | Cumulative at L50 | Cumulative at L100 | Cumulative at L150 |
|------|-----------------|--------------------|--------------------|--------------------|
| Hull HP | +2% of base | +98% | +198% | +298% |
| Shield HP | +2% of base | +98% | +198% | +298% |
| Speed | +0.5% of base | +24.5% | +49.5% | +74.5% |
| Cargo capacity | +1% of base | +49% | +99% | +149% |
| Reactor power | +1.5% of base | +73.5% | +148.5% | +223.5% |

---

## AI Level Progression (Separate from Player Level)

The AI companion has its own independent level curve (1-50), progressing slower than the player:

| Player Level Range | AI Max Level |
|--------------------|-------------|
| 1-10 | 1-10 |
| 11-25 | 11-20 |
| 26-40 | 21-30 |
| 41-55 | 31-40 |
| 56-70 | 41-50 |

AI XP sources:
- Every combat engagement the player participates in (AI gains 30% of player combat XP as AI XP, capped at 50 per session)
- Successful navigation/jump operations (+5 AI XP each)
- Completing mission objectives with the AI (+10-20 AI XP per objective)
- Station downtime repairs (+2 AI XP per station visit)

---

## XP Bonuses and Penalties

### Multipliers

| Source | Effect | Stacking |
|--------|--------|---------|
| Explorer class bonus | x1.4 to foraging/sensor XP | Multiplicative with other bonuses |
| Trader class bonus | x1.4 to trade/crafting XP | Multiplicative |
| Warrior class bonus | x1.3 to combat XP | Multiplicative |
| Faction reputation Ally | +10% all XP | Multiplicative |
| Faction reputation Trusted | +20% all XP | Multiplicative |
| Faction reputation Paragon | +35% all XP | Multiplicative |
| Campaign Arc active bonus | +25% XP to arc-relevant activities | Additive within its category, multiplicative between categories |
| Weekend/Event bonus | +50% all XP (limited-time) | Multiplicative |

### Penalties

| Condition | Effect |
|-----------|--------|
| Level advantage > 3 above target | x0.5 combat XP (prevents trivial farming) |
| Repeating the same low-value activity 10+ times in a session | Diminishing returns: -5% per additional use, minimum 20% |
---

## Level-Up Notifications and Milestones

### On-Level-Up Screen

When a player levels up, they see:
1. XP gained breakdown (source list with XP from each activity)
2. Stat increases (new Hull HP, Shield HP, etc.)
3. New unlocks (ability slots, mission types, equipment tiers)
4. Progress to next milestone (e.g., "20 more levels to Rank II")

### Rank-Up Ceremony

Crossing a rank boundary triggers an in-universe ceremony:
- Faction flag raises; new dialogue from faction commanders
- Ship receives a visual upgrade marker (rank insignia painted on hull)
- AI companion delivers a unique monologue reflecting the player growth
- New zone map unlocks with highlighted objectives

### Milestone Notifications (Non-Rank)

| Level | Notification |
|-------|-------------|
| 25  | Halfway to Rank II - you are earning your stripes. |
| 75  | Rank II midpoint. The frontier is yours to claim. |
| 100 | You have led flights of ships through fire. Your fleet awaits. |
| 125 | Elder tech calls to those who have earned it. |

---

## Integration with Existing Systems

### Ship Progression Alignment

| Player Level Range | Ship Frame Equivalent | Gear Tier Available |
|--------------------|----------------------|--------------------|
| 1-10  | Corvette (starter)   | Tier-1             |
| 11-25 | Corvette to Frigate   | Tier-1 to 2        |
| 26-40 | Frigate                | Tier-2 to 3        |
| 41-50 | Frigate (upgraded)   | Tier-3             |
| 51-70 | Destroyer              | Tier-3 to 4        |
| 71-100| Cruiser                | Tier-4             |
| 101-120| Heavy Cruiser/Battleship | Tier-4 to Elder|
| 121-150| Capital ship (fleet) | Tier-5 (quest-only)|

### AI Ability Unlock Timeline

| Player Level | AI Max Level | New Abilities Unlocked |
|-------------|-------------|----------------------|
| 1   | 1    | Target Lock Assist, Nav Optimization |
| 5   | 3    | Shield Monitor, Threat Alert |
| 10  | 5    | Emergency Repair (new slot) |
| 26  | 11   | Sensor Boost (new slot) |
| 40  | 15   | ECM Burst (new slot) |
| 55  | 20   | Comm Intercept (new slot) |
| 78  | 30   | Shield Harmonic, System Hack (new slots) |
| 95  | 35   | Jump Assist (new slot) |
| 108 | 40   | Cloak Detection (new slot) |
| 125 | 45   | Nano Repair (new slot) |
| 145 | 50   | Flare Burst, Combat Prediction (final slots) |
---

## Session Duration Estimates

For a player engaging primarily in their class preferred activity:

| Activity | Time per Level (approx) | Levels per Hour |
|----------|------------------------|-----------------|
| Solo combat (mid-game zone) | 8-15 min | 4-7.5 |
| Fleet combat | 5-10 min | 6-12 |
| Trading runs | 12-20 min | 3-5 |
| Exploration/scanning | 10-18 min | 3.3-6 |
| Mixed (balanced play) | 7-12 min | 5-8.5 |

Rank transition times (approximate, solo play):

| Transition | Approx. Time |
|-----------|-------------|
| Rank I (L1-L50) | 8-12 hours |
| Rank II (L51-L100) | 25-35 hours |
| Rank III (L101-L150) | 60-80 hours |

---

## Design Notes

### On Difficulty

The exponential curve means the last third of the game (levels 101-150) will take roughly as long as the first two-thirds combined. This is intentional - it creates a natural endgame filter where only committed players reach the final content, keeping the player population manageable for sector-based server design.

### On Class Balance

All three classes should reach level 50 in roughly the same total time when playing their preferred activity. The class bonuses (x1.4 on their specialty) ensure that specialists are never forced to switch paths, but also do not make switching feel like a bad trade: a Warrior can still earn XP trading at nearly the same rate as a Trader, just without the multiplier.

### On XP from All Three Paths

The system is designed so that any single activity path (combat-only, trade-only, or exploration-only) can get a player to level 150. However:
- Mixed play earns XP about 20-30% faster due to variety bonuses and reduced diminishing returns
- Certain late-game missions require at least two different XP backgrounds (e.g., a combat-foraged artifact requires both combat knowledge and exploration scanning)
- This gently encourages players to sample the full game without forcing any particular path

### On the Rank Boundaries

The 50-level rank structure maps directly to campaign arcs:
- **Rank I** = Tutorial through early frontier (levels are earned quickly to keep momentum)
- **Rank II** = Deep space exploration and fleet command (the grind here is intentional - it represents the player building their reputation across the galaxy)
- **Rank III** = Endgame, where players become legends of the sector
