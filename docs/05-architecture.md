# Iron Exiles - Application Architecture

## Technical Recommendation Document

---

## Executive Summary

**Recommended Engine: Unreal Engine 5**

For a multiplayer space combat MMO based on Iron Exiles with Earth & Beyond-style gameplay, Unreal Engine 5 is the recommended choice due to:
- Industry-leading visuals for space environments (Nanite, Lumen)
- Proven MMO networking (Lyra/dedicated server framework)
- Blueprint system for rapid prototyping alongside C++ for performance
- Massive marketplace with space-relevant assets
- Replication system built for persistent multiplayer worlds

**Game Type:** Space MMO - Real-time combat, trade, exploration (Earth & Beyond style)
**Target Platforms:** PC (primary), Console (secondary)
**Perspective:** Third-person ship view + cockpit view (toggle)
**Players:** 50-100 per sector instance, thousands per galaxy server

---

## Why Unreal Engine 5

| Requirement | UE5 Capability |
|-------------|---------------|
| Stunning space visuals | Nanite + Lumen for detailed ships and dynamic lighting |
| Large-scale battles | Niagara VFX system for particle-heavy combat |
| MMO Networking | Dedicated server, replication, Iris (spatial networking) |
| AI-driven enemies + Ship AI | Behavior Trees + EQS + GAS (Gameplay Ability System) |
| UI-heavy MMO interface | UMG/CommonUI for HUD, inventory, trade, chat |
| Modding support (future) | Blueprint exposure enables modding community |
| Audio | MetaSounds for dynamic combat audio |
| Persistent world | Seamless travel via level streaming + World Partition |

### Alternatives Considered

| Engine | Verdict |
|--------|---------|
| Unity | Good networking (Netcode/Mirror) but HDRP less mature; licensing risk |
| Godot | Networking insufficient for MMO-scale; 3D still maturing |
| Custom Engine | Maximum control but 3-5 year time investment before gameplay |

---

## High-Level Architecture (MMO)

```
┌─────────────────────────────────────────────────────────────────────────┐
│                           CLIENT (UE5)                                   │
├─────────────────────────────────────────────────────────────────────────┤
│                                                                          │
│  ┌─────────────┐ ┌─────────────┐ ┌─────────────┐ ┌─────────────────┐  │
│  │  COMBAT     │ │  GALAXY     │ │  ECONOMY    │ │  SOCIAL/CHAT    │  │
│  │  (Flight +  │ │  (Map +     │ │  (Trade +   │ │  (Groups +      │  │
│  │   Weapons)  │ │   Travel)   │ │   Craft)    │ │   Guilds)       │  │
│  └──────┬──────┘ └──────┬──────┘ └──────┬──────┘ └───────┬─────────┘  │
│         │                │                │                │             │
│  ┌──────┴────────────────┴────────────────┴────────────────┴──────────┐ │
│  │              CLIENT CORE SYSTEMS                                    │ │
│  │  ┌────────┐ ┌────────┐ ┌────────┐ ┌────────┐ ┌────────────────┐  │ │
│  │  │Ship AI │ │ HUD/UI │ │Physics │ │ Audio  │ │ Net Prediction │  │ │
│  │  │(Local) │ │(UMG)   │ │(Client)│ │(Meta)  │ │ & Interp       │  │ │
│  │  └────────┘ └────────┘ └────────┘ └────────┘ └────────────────┘  │ │
│  └────────────────────────────────────────────────────────────────────┘ │
│                                    │                                     │
│                            [UE5 Replication]                             │
└────────────────────────────────────┼─────────────────────────────────────┘
                                     │ Network
┌────────────────────────────────────┼─────────────────────────────────────┐
│                         SERVER INFRASTRUCTURE                              │
├───────────────────────────────────────────────────────────────────────────┤
│                                                                            │
│  ┌──────────────────┐  ┌──────────────────┐  ┌────────────────────────┐  │
│  │  GAME SERVERS    │  │  WORLD SERVER    │  │  SERVICES (Backend)    │  │
│  │  (UE5 Dedicated) │  │  (Orchestrator)  │  │                        │  │
│  │                  │  │                  │  │  - Auth Service         │  │
│  │  - Sector Host   │  │  - Sector Mgmt   │  │  - Account/Character   │  │
│  │  - Combat Auth   │  │  - Player Route  │  │  - Economy Service     │  │
│  │  - NPC/AI Sim    │  │  - Load Balance  │  │  - Chat Service        │  │
│  │  - Physics Auth  │  │  - Instance Spin │  │  - Guild Service       │  │
│  │  - Loot/Drops    │  │  - Events/World  │  │  - Mission Service     │  │
│  │                  │  │                  │  │  - Matchmaking         │  │
│  └────────┬─────────┘  └────────┬─────────┘  └───────────┬────────────┘  │
│           │                      │                         │               │
│  ┌────────┴──────────────────────┴─────────────────────────┴────────────┐ │
│  │                        DATABASE LAYER                                 │ │
│  │  ┌──────────┐  ┌──────────┐  ┌──────────┐  ┌──────────────────┐    │ │
│  │  │PostgreSQL│  │  Redis   │  │InfluxDB  │  │ Object Storage   │    │ │
│  │  │(Persist) │  │ (Cache)  │  │(Metrics) │  │ (Assets/Logs)    │    │ │
│  │  └──────────┘  └──────────┘  └──────────┘  └──────────────────┘    │ │
│  └──────────────────────────────────────────────────────────────────────┘ │
└───────────────────────────────────────────────────────────────────────────┘
```

---

## Server Architecture Detail

### Dedicated Game Servers (UE5)

Each sector of space runs as a UE5 Dedicated Server instance:

| Component | Responsibility |
|-----------|---------------|
| Sector Host | Runs one sector (50-100 players), handles physics, NPC AI |
| Combat Authority | Server-authoritative hit detection, damage calculation |
| NPC Simulation | Faction NPCs, enemy ships, patrol routes |
| Loot Authority | Drop tables, reward distribution |
| Replication | Sync ship positions, combat state to all clients |

### World Server (Orchestrator)

Central service managing the persistent galaxy state:

| Component | Responsibility |
|-----------|---------------|
| Sector Manager | Spin up/down sector instances based on population |
| Player Router | Direct players to correct sector server on travel |
| Load Balancer | Distribute players across instances |
| Instance Spawner | Create instanced missions/raids on demand |
| World Events | Trigger server-wide events (Vorathis incursions, etc.) |
| Territory Tracker | Faction control state, guild ownership |

### Backend Microservices

Stateless services handling non-realtime logic:

```
Services/
├── AuthService/           # Login, token management, anti-cheat
├── AccountService/        # Character data, inventory, progression
├── EconomyService/        # Auction house, trade, currency
├── ChatService/           # Text chat, voice relay, mail
├── GuildService/          # Guild management, permissions, shared storage
├── MissionService/        # Mission state, quest tracking, rewards
├── MatchmakingService/    # PvP arena, group finder, fleet battles
├── LeaderboardService/    # Rankings, achievements, statistics
└── AnalyticsService/      # Player behavior, balance telemetry
```

---

## Module Breakdown (Client-Side)

### 1. Combat Module

**Responsibility:** Real-time ship flight, weapons, defenses. Server-authoritative.

```
CombatModule/
├── ShipController/
│   ├── MovementComponent        # 6DOF flight, client-predicted
│   ├── TargetingComponent       # Tab-target + free-aim hybrid
│   ├── PowerManager             # Reactor allocation (W/S/E/AI split)
│   └── InputHandler             # Keyboard/gamepad/HOTAS mapping
├── WeaponSystem/
│   ├── WeaponBase               # Abstract weapon (replicated)
│   ├── BeamWeapon               # Instant-hit energy beams
│   ├── ProjectileWeapon         # Kinetic rounds (coilgun/railgun)
│   ├── MissileWeapon            # Lock-on guided munitions
│   ├── PlasmaWeapon             # AoE splash damage
│   └── DroneWeapon              # Deployable autonomous fighters
├── DefenseSystem/
│   ├── ShieldComponent          # Directional shields, facing mgmt
│   ├── ArmorComponent           # Hull HP + damage reduction
│   ├── PointDefenseComponent    # Auto-intercept missiles/drones
│   └── ECMComponent             # Countermeasures, decoys
├── DamageSystem/
│   ├── DamageModel              # Server-auth HP/subsystem calc
│   ├── SubsystemDamage          # Engine/weapon/shield degradation
│   └── DestructionHandler       # Death, wreckage spawn, loot drop
└── JumpDrive/
    ├── JumpController           # Warmup, cooldown, interdiction
    ├── SectorTransition         # Handoff to world server for travel
    └── WormholeTravel           # Instant sector-to-sector travel
```

### 2. Ship AI Module (Client + Server)

**Responsibility:** Player's AI companion system. Distinct from NPC AI.

```
ShipAIModule/
├── AICore/
│   ├── AIPersonalityComponent   # Faction-based personality traits
│   ├── AILevelSystem            # XP tracking, level 1-50
│   ├── AIDialogueManager        # Context-triggered voice lines
│   └── AIMoodSystem             # Reactive personality (combat/idle/trade)
├── AIAbilities/
│   ├── AbilityBase              # Cooldown, power cost, level req
│   ├── PassiveAbilities/
│   │   ├── TargetLockAssist     # +Accuracy
│   │   ├── NavOptimization      # +Speed
│   │   ├── ShieldMonitor        # +Regen
│   │   ├── CloakDetection       # Reveal stealth
│   │   └── CombatPrediction     # Show enemy patterns
│   └── ActiveAbilities/
│       ├── EmergencyRepair      # Hull repair over time
│       ├── SensorBoost          # Extended detection
│       ├── ECMBurst             # Break target locks
│       ├── SystemHack           # Disable enemy system
│       ├── ShieldHarmonic       # Shield buff
│       ├── JumpAssist           # Reduce jump cooldown
│       ├── FleetSync            # Group buff
│       ├── NanoRepair           # Advanced repair
│       ├── MasterHack           # Disable all systems
│       └── Overdrive            # Double stats (risky)
├── AISlotManager/
│   ├── AbilitySlotConfig        # 2-6 slots based on AI level
│   └── AIModuleEquipment        # Equippable AI enhancement chips
└── AIProgression/
    ├── AIExperienceTracker      # XP from missions, discoveries, use
    └── AIUnlockManager          # Level-gated ability unlocks
```

### 3. Progression Module

**Responsibility:** Triple-XP system, leveling, skills, hull upgrades.

```
ProgressionModule/
├── XPSystem/
│   ├── CombatXPTracker          # Kill XP, PvP XP, combat missions
│   ├── TradeXPTracker           # Crafting XP, hauling, sales
│   ├── ExploreXPTracker         # Discovery XP, scanning, artifacts
│   └── OverallLevelCalculator   # Sum of all three (max 150)
├── SkillSystem/
│   ├── SkillTree                # Per-class skill trees
│   ├── SkillPointManager        # Earned every 5 levels
│   └── SkillEffectApplier       # Stat modifiers from skills
├── ShipProgression/
│   ├── HullUpgradeSystem        # Tier 1-6 visual + stat upgrades
│   ├── EquipmentSlotManager     # Slots expand with hull tier
│   └── TechLevelGate            # TL1-TL9 equipment restrictions
└── ClassSpecialization/
    ├── WarriorTree               # Combat-focused abilities
    ├── TraderTree                # Economy-focused abilities
    └── ExplorerTree              # Discovery-focused abilities
```

### 4. Economy Module

**Responsibility:** Crafting, trade, auction house, resource management.

```
EconomyModule/
├── Inventory/
│   ├── InventoryComponent       # Player item storage
│   ├── CargoComponent           # Ship cargo hold (capacity-limited)
│   └── BankComponent            # Station-based extended storage
├── Crafting/
│   ├── RecipeDatabase           # All craftable items + requirements
│   ├── RefinerySystem           # Raw ore → refined materials
│   ├── ManufacturingSystem      # Components → equipment
│   └── AIChipProgramming        # Craft AI enhancement modules
├── Trading/
│   ├── AuctionHouseClient       # Player-to-player market
│   ├── NPCVendorSystem          # Faction shops, tech-level gated
│   ├── TradeRouteTracker        # Buy/sell price differentials
│   └── ContractSystem           # Player-posted jobs
└── ResourceGathering/
    ├── MiningSystem              # Asteroid mining gameplay
    ├── SalvageSystem             # Wreck looting after combat
    └── ScanningRewards           # Anomaly scan → data/materials
```

### 5. Galaxy & Navigation Module

**Responsibility:** World map, travel, sector transitions, exploration.

```
GalaxyModule/
├── GalaxyMap/
│   ├── SectorGrid               # All sectors, faction ownership
│   ├── NavPointDatabase         # Stations, planets, fields per sector
│   ├── WormholeNetwork          # Fast-travel connections
│   ├── FogOfWar                 # Per-player exploration state
│   └── MapRenderer3D            # Interactive 3D galaxy visualization
├── Travel/
│   ├── InSectorFlight           # Normal flight within sector
│   ├── JumpGateTravel           # Sector-to-sector via gates
│   ├── WormholeTravel           # Hidden fast-travel routes
│   └── SectorLoadManager        # Seamless sector transitions
├── Exploration/
│   ├── ScanningGameplay         # Active scanning mini-game
│   ├── DiscoveryTracker         # What player has found
│   ├── AnomalyGenerator        # Procedural points of interest
│   └── ElderSiteManager         # Rare high-value locations
└── Instancing/
    ├── MissionInstanceManager   # Private mission zones
    ├── RaidInstanceManager      # Group PvE instances
    └── PvPArenaManager          # Matchmade combat zones
```

### 6. Social Module

**Responsibility:** Chat, groups, guilds, friends, PvP rules.

```
SocialModule/
├── Chat/
│   ├── TextChatManager          # Local, faction, global channels
│   ├── VoiceChat                # Proximity + group voice
│   └── MailSystem               # Offline messaging
├── Grouping/
│   ├── PartyManager             # 2-6 player parties
│   ├── FleetManager             # 8-12 player raids
│   ├── GroupFinder              # Matchmaking for group content
│   └── LootDistribution        # Need/Greed/Round-robin
├── Guilds/
│   ├── GuildManager             # Create, join, permissions
│   ├── GuildShipSystem          # Shared capital ship progression
│   ├── TerritoryClaimSystem     # Guild-owned sectors
│   └── GuildMissionBoard        # Cooperative objectives
└── PvP/
    ├── PvPZoneManager           # Safe/contested/open zone rules
    ├── DuelSystem               # Consensual 1v1
    ├── FactionWarManager        # Large-scale faction PvP
    └── BountySystem             # Player bounties
```

### 7. Mission & Narrative Module

**Responsibility:** Quest system, story, daily content, reputation.

```
MissionModule/
├── QuestSystem/
│   ├── StoryMissionChain        # Faction-specific main storyline
│   ├── SideMissionGenerator     # Procedural side content
│   ├── DailyMissionBoard        # Repeatable daily tasks
│   └── GroupMissionQueue        # Party/fleet required missions
├── ReputationSystem/
│   ├── FactionRepTracker        # -100 to +100 per NPC faction
│   ├── RepRewardUnlocker        # Unlock gear/missions at rep tiers
│   └── RepDecayManager          # Slow decay if not maintained
├── DynamicEvents/
│   ├── WorldEventScheduler      # Server-wide timed events
│   ├── VorathisIncursion         # Cooperative defense events
│   └── ElderAwakening           # Rare discovery events
└── DialogueSystem/
    ├── NPCDialogueManager       # Quest givers, vendors, lore NPCs
    ├── AICompanionBanter        # Ship AI contextual commentary
    └── CutsceneManager          # Story moments, hull upgrade scenes
```

---

## NPC AI Module (Server-Side)

**Responsibility:** Enemy ship behavior, faction patrols, world population.

```
NPCAIModule/  (Runs on Dedicated Server)
├── BehaviorTrees/
│   ├── BT_Kethari              # Aggressive rush, swarm, board
│   ├── BT_Velnari                 # Defensive formation, shield focus
│   ├── BT_Vorathis              # Stealth approach, alpha-strike
│   ├── BT_Oruneti             # Deploy drones, stay at range
│   ├── BT_Vexari             # EW disruption, never commit
│   └── BT_Neutral              # Traders, miners, civilians
├── SpawnSystem/
│   ├── PatrolSpawner            # Faction patrols per sector
│   ├── MissionSpawner           # Enemies for active missions
│   ├── DynamicSpawner           # Respond to player actions
│   └── BossSpawner              # Elite/raid enemies
├── FleetCoordination/
│   ├── FormationManager         # Group movement AI
│   ├── ThreatAssessment         # Target prioritization
│   └── RetreatLogic            # Flee when losing
└── WorldPopulation/
    ├── TraderNPCs               # NPC trade ships (ambushable)
    ├── MinerNPCs                # NPC miners in asteroid fields
    └── CivilianTraffic          # Background life in sectors
```

---

## Data Architecture

### Database Schema (PostgreSQL)

```sql
-- Player & Character Data
CREATE TABLE accounts (
    account_id UUID PRIMARY KEY,
    email TEXT UNIQUE NOT NULL,
    password_hash TEXT NOT NULL,
    created_at TIMESTAMP DEFAULT NOW()
);

CREATE TABLE characters (
    character_id UUID PRIMARY KEY,
    account_id UUID REFERENCES accounts(account_id),
    name TEXT UNIQUE NOT NULL,
    faction TEXT NOT NULL,          -- Human/Velnari/Kethari/Oruneti
    class TEXT NOT NULL,            -- Warrior/Trader/Explorer
    combat_level INT DEFAULT 1,
    trade_level INT DEFAULT 1,
    explore_level INT DEFAULT 1,
    combat_xp BIGINT DEFAULT 0,
    trade_xp BIGINT DEFAULT 0,
    explore_xp BIGINT DEFAULT 0,
    ai_level INT DEFAULT 1,
    ai_xp BIGINT DEFAULT 0,
    credits BIGINT DEFAULT 1000,
    current_sector TEXT,
    hull_tier INT DEFAULT 1,
    created_at TIMESTAMP DEFAULT NOW()
);

-- Ship & Equipment
CREATE TABLE ship_loadouts (
    character_id UUID REFERENCES characters(character_id),
    slot_type TEXT NOT NULL,        -- weapon/shield/device/engine/reactor/ai
    slot_index INT NOT NULL,
    item_id UUID REFERENCES items(item_id),
    PRIMARY KEY (character_id, slot_type, slot_index)
);

CREATE TABLE items (
    item_id UUID PRIMARY KEY,
    owner_id UUID REFERENCES characters(character_id),
    template_id TEXT NOT NULL,      -- References DataTable row
    tech_level INT NOT NULL,
    quality TEXT DEFAULT 'common',  -- common/uncommon/rare/epic/elder
    stats JSONB,                    -- Rolled stats for this instance
    location TEXT DEFAULT 'inventory'  -- inventory/equipped/bank/auction
);

-- AI Companion
CREATE TABLE ai_companions (
    character_id UUID PRIMARY KEY REFERENCES characters(character_id),
    ai_name TEXT NOT NULL,
    personality TEXT NOT NULL,       -- Based on faction
    level INT DEFAULT 1,
    xp BIGINT DEFAULT 0,
    equipped_abilities TEXT[],       -- Array of ability IDs in slots
    unlocked_abilities TEXT[],       -- All abilities unlocked so far
    module_slots JSONB              -- Equipped AI enhancement chips
);

-- Economy
CREATE TABLE auction_listings (
    listing_id UUID PRIMARY KEY,
    seller_id UUID REFERENCES characters(character_id),
    item_id UUID REFERENCES items(item_id),
    price BIGINT NOT NULL,
    listed_at TIMESTAMP DEFAULT NOW(),
    expires_at TIMESTAMP NOT NULL
);

-- Progression
CREATE TABLE faction_reputation (
    character_id UUID REFERENCES characters(character_id),
    faction_name TEXT NOT NULL,
    reputation INT DEFAULT 0,       -- -100 to 100
    PRIMARY KEY (character_id, faction_name)
);

CREATE TABLE discoveries (
    character_id UUID REFERENCES characters(character_id),
    discovery_type TEXT NOT NULL,    -- sector/navpoint/wormhole/elder
    discovery_id TEXT NOT NULL,
    discovered_at TIMESTAMP DEFAULT NOW(),
    PRIMARY KEY (character_id, discovery_id)
);

-- Guilds
CREATE TABLE guilds (
    guild_id UUID PRIMARY KEY,
    name TEXT UNIQUE NOT NULL,
    faction TEXT NOT NULL,
    leader_id UUID REFERENCES characters(character_id),
    capital_ship_tier INT DEFAULT 1,
    territory_sectors TEXT[],
    created_at TIMESTAMP DEFAULT NOW()
);
```

### Redis Cache Layer

| Key Pattern | Data | TTL |
|------------|------|-----|
| `player:{id}:session` | Current sector, position, state | Session |
| `sector:{id}:players` | Player list in sector | Real-time |
| `auction:listings:{category}` | Cached auction queries | 60s |
| `faction:territory` | Current territory map | 30s |
| `events:active` | Live world events | Event duration |

### UE5 Data Tables (Client-Side, Static)

```cpp
// Ship hull definitions per faction + tier
USTRUCT()
struct FShipHullData : public FTableRowBase
{
    UPROPERTY() FName HullId;
    UPROPERTY() EFaction Faction;
    UPROPERTY() EShipClass Class;
    UPROPERTY() int32 HullTier;        // 1-6
    UPROPERTY() float BaseHullHP;
    UPROPERTY() float BaseShieldHP;
    UPROPERTY() float BaseSpeed;
    UPROPERTY() float Maneuverability;
    UPROPERTY() float ReactorOutput;
    UPROPERTY() int32 WeaponSlots;
    UPROPERTY() int32 DeviceSlots;
    UPROPERTY() int32 CargoCapacity;
    UPROPERTY() int32 AIModuleSlots;
    UPROPERTY() TSoftObjectPtr<UStaticMesh> ShipMesh;
    UPROPERTY() TSoftObjectPtr<UMaterialInstance> ShipMaterial;
};

// Weapon template definitions
USTRUCT()
struct FWeaponData : public FTableRowBase
{
    UPROPERTY() FName WeaponId;
    UPROPERTY() EWeaponType Type;      // Beam/Projectile/Missile/Plasma/Drone
    UPROPERTY() int32 TechLevel;       // 1-9
    UPROPERTY() float BaseDamage;
    UPROPERTY() float FireRate;
    UPROPERTY() float Range;
    UPROPERTY() float EnergyCost;
    UPROPERTY() int32 AmmoCapacity;    // -1 for energy weapons
    UPROPERTY() EFaction FactionRestriction;
};

// AI Ability definitions
USTRUCT()
struct FAIAbilityData : public FTableRowBase
{
    UPROPERTY() FName AbilityId;
    UPROPERTY() FText DisplayName;
    UPROPERTY() EAbilityType Type;     // Active/Passive
    UPROPERTY() int32 RequiredAILevel;
    UPROPERTY() float Cooldown;
    UPROPERTY() float PowerCost;
    UPROPERTY() float Duration;
    UPROPERTY() float EffectValue;
    UPROPERTY() TSoftObjectPtr<UTexture2D> Icon;
};
```

---

## Networking Architecture

### Replication Strategy

| Data | Authority | Replication |
|------|-----------|-------------|
| Ship Position/Rotation | Server | Replicated + client prediction |
| Weapon Fire | Client-initiated, Server-validated | Multicast RPC |
| Damage/HP | Server | Replicated to all in range |
| Shield State | Server | Replicated |
| AI Abilities | Client-request, Server-execute | Multicast VFX |
| Inventory Changes | Server | Owner only |
| Chat Messages | Server relay | Relevant clients |
| Sector Transition | World Server | Handoff protocol |

### Sector Transition Flow

```
Player activates jump gate
    → Client sends JumpRequest RPC to current sector server
    → Sector server validates (cooldown, interdiction check)
    → Sector server notifies World Server: "Player X leaving Sector A"
    → World Server finds/spawns Sector B instance
    → World Server sends connection info to client
    → Client disconnects from Sector A, connects to Sector B
    → Sector B spawns player at gate arrival point
    → Seamless (loading screen with jump VFX)
```

### Instanced Content Flow

```
Player accepts mission / enters raid
    → Server spins up private instance (or reuses pooled)
    → Only party members can connect
    → Instance runs independently
    → On completion: rewards granted, instance recycled
    → Player returned to open-world sector
```

---

## Rendering & Visual Pipeline

### Space Environment

| Feature | UE5 System | Notes |
|---------|-----------|-------|
| Starfields/Nebulae | Volumetric clouds + skybox | Per-sector unique look |
| Ship Detail | Nanite meshes | LOD-free, visual hull tiers |
| Lighting | Lumen GI | Star as primary light source |
| Explosions/VFX | Niagara | Particle budgets for MMO perf |
| Beam Weapons | Niagara beams | Replicated start/end points |
| Shield Effects | Custom shader + Niagara | Hit direction visualization |
| Jump/Wormhole FX | Post-process + Niagara | Transition effects |
| Other Players | Nanite + LOD at distance | Reduce detail past 5km |

### Camera System

| View | Description | When Used |
|------|-------------|-----------|
| Chase Camera | Third-person behind ship (default) | Combat, travel |
| Cockpit View | First-person with instrument panel | Immersion, combat |
| Orbit Camera | Free orbit around own ship | Station docking, inspection |
| Cinematic | Auto-camera for events | Hull upgrades, cutscenes |

---

## UI/UX Architecture (MMO)

### Flight HUD

```
┌─────────────────────────────────────────────────────────────────────┐
│ [Faction]  [Sector: Sol-3]  [Players: 47]         [Chat] [Menu]    │
│                                                                      │
│ ┌────┐                                              ┌─────────────┐ │
│ │SHLD│ F:██████                                     │   RADAR     │ │
│ │    │ B:████░░            ┌─Target─────┐           │  · ○  ★  ·  │ │
│ │    │ L:█████░            │ Kethari   │           │    ·  ·     │ │
│ │    │ R:██████            │ Sea Snake  │           │  ○    ·  ○  │ │
│ │HULL│ [████████░░] 82%   │ HP: ███░░  │           └─────────────┘ │
│ └────┘                     │ Dist: 2.4km│                           │
│                            └────────────┘                           │
│                                                                      │
│ ┌─Power──────────────────┐  ┌─AI Abilities──────────────────────┐  │
│ │ WPN [█████░░░] 50%     │  │ [HACK]    [ECM]     [REPAIR]     │  │
│ │ SHD [████░░░░] 35%     │  │ CD:45s    READY     CD:120s      │  │
│ │ ENG [██░░░░░░] 10%     │  │                                   │  │
│ │ AI  [█░░░░░░░]  5%     │  │ AI: "Enemies at 6 o'clock!"     │  │
│ └────────────────────────┘  └───────────────────────────────────┘  │
│                                                                      │
│ [Weapon 1: Railgun ████] [Weapon 2: Beam ██] [Missile: 12]         │
│ [Speed: 340 m/s]  [XP: Combat +125]  [Credits: 45,230]            │
└─────────────────────────────────────────────────────────────────────┘
```

### Station UI

```
Station Menu (Docked)
├── Ship Loadout (Equip weapons, shields, devices, AI modules)
├── Ship Upgrade (Hull tier upgrade when eligible)
├── AI Management (Ability slots, review AI level/XP)
├── Auction House (Buy/sell player items)
├── Vendor (NPC shop, faction-gated)
├── Crafting Station (Refine, manufacture, program AI chips)
├── Mission Board (Available missions by type)
├── Hangar (View ship in 3D, customize cosmetics)
└── Social (Guild, friends, mail, group finder)
```

### Galaxy Map UI

```
Galaxy Map (Overlay)
├── Sector View (zoom in to nav points, players, resources)
├── Faction Territory (color-coded ownership)
├── Wormhole Routes (known connections)
├── Player Location (current position highlighted)
├── Mission Markers (active quest destinations)
└── Guild Territory (claimed systems highlighted)
```

---

## Development Phases (MMO-Adjusted)

### Phase 1: Tech Prototype (4-5 months)
- Basic 6DOF ship flight (client-predicted, server-auth)
- Single sector with 10 players connected
- One weapon type firing + damage
- Shield system with replication
- Basic login + character persistence
- Placeholder ship models (one per faction)

### Phase 2: Core Gameplay (6-8 months)
- All weapon types + power allocation
- Ship AI system (levels 1-10, 3 abilities)
- Triple-XP system tracking
- Hull Tier 1-3 progression
- TL1-TL5 equipment
- 3 sectors with jump gate travel
- Basic mission system (combat missions)
- NPC enemies with faction behavior trees
- Station docking + equipment UI

### Phase 3: Economy & Social (4-6 months)
- Crafting system (mining → refining → manufacturing)
- Auction house
- Trade missions, trade XP
- Group system (parties of 2-6)
- Chat system
- Guild creation + basic features
- Exploration scanning + Explore XP

### Phase 4: Content & World (6-8 months)
- Full galaxy (20+ sectors)
- All hull tiers (1-6) with visual upgrades
- AI levels 1-50 with all abilities
- Story mission chains per faction
- Faction reputation system
- PvP zones + dueling
- World events (Vorathis incursions)
- Elder sites (endgame exploration)

### Phase 5: Endgame & Polish (4-6 months)
- Raid instances (8-12 player)
- Guild capital ships + territory control
- Faction war system
- TL6-TL9 Elder-enhanced gear
- AI Ascension quest chain
- Season system framework
- Performance optimization + load testing
- Sound design + music + VFX polish

### Phase 6: Launch & Live Service
- Open beta stress testing
- Balance tuning from telemetry
- Launch
- Monthly content updates
- Seasonal events
- Expansion planning

---

## Technical Considerations

### Performance Targets

| Scenario | Target FPS | Max Entities |
|----------|-----------|-------------|
| Solo flight | 60 fps | Player + 50 NPCs |
| Sector (50 players) | 60 fps | 50 players + 100 NPCs |
| Fleet battle (PvP) | 30+ fps | 24 players + effects |
| Station (social) | 60 fps | 50+ player ships docked |

### Network Requirements

| Metric | Target |
|--------|--------|
| Tick Rate | 30 Hz (combat), 10 Hz (travel) |
| Player Bandwidth | ~50 KB/s per player |
| Latency Tolerance | <150ms playable, <80ms ideal |
| Sector Capacity | 100 players max per instance |

### Scalability Strategy

| Challenge | Solution |
|-----------|----------|
| Player population growth | Horizontal sector server scaling |
| Fleet battle performance | Reduce tick rate, LOD ships, cull distant VFX |
| Database load | Read replicas, Redis cache, sharding |
| World events (many players) | Overflow instances, phasing |
| Asset streaming | World Partition, Nanite auto-LOD |

### Key Technical Risks

| Risk | Mitigation |
|------|-----------|
| MMO-scale networking | Prototype early, load test monthly |
| Server costs | Dynamic scaling, idle sector shutdown |
| Cheating/exploits | Server-authoritative combat, anti-cheat |
| Sector transitions (seamless) | Preload assets, jump VFX covers load |
| AI companion sync | AI runs locally, server validates outcomes |
| Economy exploits | Rate limiting, anomaly detection, GM tools |

---

## Infrastructure & DevOps

### Hosting Recommendation

| Component | Platform | Notes |
|-----------|----------|-------|
| Game Servers | AWS GameLift or custom EC2 | Auto-scaling UE5 dedicated servers |
| Backend Services | Kubernetes (EKS) | Microservices, auto-scale |
| Database | AWS RDS (PostgreSQL) | Managed, multi-AZ |
| Cache | ElastiCache (Redis) | Session data, hot queries |
| CDN | CloudFront | Asset delivery, patches |
| Monitoring | Grafana + Prometheus | Server health, player metrics |
| CI/CD | GitHub Actions + Jenkins | Build, test, deploy pipeline |

### Deployment Architecture

```
                    [CloudFront CDN]
                          │
                    [Load Balancer]
                     /    |    \
            [Auth]  [API Gateway] [Chat]
                          |
                  [Backend Services]
                     /    |    \
          [PostgreSQL] [Redis] [Object Store]

  [GameLift / EC2 Fleet]
    ├── Sector Server Pool (auto-scale)
    ├── Instance Server Pool (on-demand)
    └── World Orchestrator (singleton)
```

---

## Recommended Team Composition (MMO)

| Role | Count | Responsibility |
|------|-------|---------------|
| Technical Director | 1 | Architecture, server infrastructure |
| Lead Gameplay Programmer (C++) | 1 | Combat, flight, ship systems |
| Network Programmer | 2 | Replication, server auth, transitions |
| Backend Engineer | 2 | Microservices, database, economy |
| AI Programmer | 1 | Ship AI, NPC behavior trees |
| UI Programmer | 1 | HUD, station menus, galaxy map |
| DevOps/Infrastructure | 1 | Servers, CI/CD, monitoring |
| Technical Artist | 1 | Shaders, VFX, optimization |
| 3D Artist (Ships) | 2 | Ship models (4 factions × 6 tiers) |
| Environment Artist | 1 | Sectors, stations, skyboxes |
| VFX Artist | 1 | Combat effects, jump FX |
| Game Designer (Systems) | 1 | Balance, progression, economy |
| Game Designer (Content) | 1 | Missions, world design, events |
| Narrative Designer | 1 | Story, AI dialogue, faction lore |
| Sound Designer | 1 | Audio, music, AI voice |
| QA Lead | 1 | Testing, exploits, balance |
| QA Tester | 2 | Regression, multiplayer testing |
| Community Manager | 1 | Beta feedback, player relations |

**Total: ~22 people (full production)**
**Minimum viable for alpha: 8-10 (combined roles)**

---

## Summary

| Decision | Choice | Rationale |
|----------|--------|-----------|
| Engine | Unreal Engine 5 | Best visuals + proven dedicated server framework |
| Language | C++ (core) + Blueprint (gameplay) | Performance + rapid iteration |
| Architecture | Client-Server MMO with sector instancing | Scalable, server-authoritative |
| Backend | Microservices (Kubernetes) | Independent scaling per service |
| Database | PostgreSQL + Redis | Persistent data + real-time cache |
| Combat | Server-authoritative, client-predicted | Fair PvP, cheat-resistant |
| Progression | Triple-XP (E&B style) + Ship AI leveling | Depth, multiple play paths |
| World Structure | Sector-based with instanced content | Manageable server load |
| Hosting | AWS GameLift + EKS | Auto-scaling, global deployment |
