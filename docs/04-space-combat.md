# Iron Exiles - Space Combat Mechanics

## Game Design Document: Combat Systems & Gameplay Loop

---

## Core Combat Philosophy

The Iron Exiles series is defined by **asymmetric warfare** - the player is always outgunned on paper but wins through:
1. **Tactical creativity** (positioning, timing, environment use)
2. **Kestrel's abilities** (hacking, ECM, enhanced targeting)
3. **Deception** (decoys, false signatures, misdirection)
4. **Hit-and-run tactics** (jump in, strike, jump out)

Combat should feel tense, cerebral, and rewarding - not a DPS race.

---

## Combat Layers

### Layer 1: Ship-to-Ship Combat (Core)

Real-time tactical combat with pause-and-plan option (a tight human-AI coordination loop).

**Engagement Flow:**
```
Detection → Approach/Position → Engagement → Resolution → Escape/Pursue
```

| Phase | Player Actions |
|-------|---------------|
| Detection | Use sensors, Kestrel to identify threats. Decide to engage or avoid. |
| Approach | Choose vector, use stealth, set up ambush position |
| Engagement | Manage weapons, shields, power allocation, Kestrel abilities |
| Resolution | Destroy/disable enemy, or decide to disengage |
| Escape/Pursue | Jump away or chase fleeing enemies |

---

### Layer 2: Fleet Battles (Late Game)

Command multiple ships with tactical orders.

| Command | Effect |
|---------|--------|
| Focus Fire | All ships target one enemy |
| Spread Formation | Reduce area damage effectiveness |
| Shield Wall | Defensive grouping, shared shield bonuses |
| Flanking Run | Split fleet for crossfire |
| Jump Ambush | Coordinate jump-in timing for surprise |

---

### Layer 3: Boarding Actions

Rare but dramatic - triggered by specific conditions.

| Trigger | Scenario |
|---------|----------|
| Disabled Ship | Board to capture technology |
| Kethari Attack | Defend against boarding parties |
| Stealth Mission | Insert team onto enemy vessel |

---

## Weapon Systems in Combat

### Kinetic Weapons (Railguns/Coilguns)

| Stat | Details |
|------|---------|
| Damage Type | Kinetic (physical impact) |
| Effectiveness | Excellent vs hulls, poor vs shields |
| Ammo | Limited (must resupply) |
| Range | Long |
| Speed | Very fast (relativistic projectiles) |
| Counter | Evasion, point defense |

**Tactics:** Use after shields are down for maximum hull damage.

### Energy Beams (Lasers/Particle Beams)

| Stat | Details |
|------|---------|
| Damage Type | Energy (sustained) |
| Effectiveness | Good vs shields, moderate vs hulls |
| Ammo | Unlimited (drains reactor power) |
| Range | Medium-Long |
| Speed | Instant (light-speed) |
| Counter | Shield tuning, reflective armor |

**Tactics:** Primary shield-stripping weapon. Sustained fire depletes enemy shields.

### Plasma Weapons

| Stat | Details |
|------|---------|
| Damage Type | Thermal/Energy (area) |
| Effectiveness | Good vs grouped targets, moderate vs all |
| Ammo | Unlimited (high power drain) |
| Range | Medium |
| Speed | Moderate |
| Counter | Spread formation, shields |

**Tactics:** Best against clusters. Force enemies to spread out.

### Missiles/Torpedoes

| Stat | Details |
|------|---------|
| Damage Type | Variable (kinetic + explosive) |
| Effectiveness | High burst damage if they connect |
| Ammo | Very limited |
| Range | Very long (self-guided) |
| Speed | Moderate (accelerating) |
| Counter | Point defense, ECM, decoys |

**Tactics:** Alpha-strike openers. Fire first, guide with sensors.

### Gravitic Weapons (Vorathis+ Tier)

| Stat | Details |
|------|---------|
| Damage Type | Gravitic (bypasses shields partially) |
| Effectiveness | Devastating vs all defenses |
| Ammo | Unlimited (extreme power drain) |
| Range | Medium |
| Speed | Instant |
| Counter | Distance, Elder-tech shields only |

**Tactics:** Endgame weapons. Shield-penetrating makes them terrifying.

---

## Defense Systems in Combat

### Shield Management

Shields are the primary defense layer with active management:

| Mechanic | Description |
|----------|-------------|
| Shield Facing | Redirect shield power to threatened facing |
| Recharge Rate | Shields regenerate over time when not taking fire |
| Overload | Temporarily boost shields at cost of other power |
| Frequency Tuning | Tune shields against specific weapon types |
| Shield Collapse | When depleted, significant cooldown before regen |

### Power Allocation

The reactor produces finite power distributed between systems:

```
[===REACTOR OUTPUT===]
├── Weapons     [||||____] 50%
├── Shields     [|||_____] 30%
├── Engines     [|_______] 10%
└── Kestrel/ECM  [|_______] 10%
```

**Player choice:** Allocate power in real-time based on combat needs.

### Countermeasures & ECM

| System | Function |
|--------|----------|
| Decoy Drones | Draw missile locks away |
| Signature Masking | Reduce detection range |
| Comm Jamming | Prevent enemy coordination |
| Sensor Ghosts | Create false targets on enemy sensors |
| Kestrel Hack | Directly disable enemy systems (cooldown) |

---

## Kestrel Combat Abilities

Kestrel provides unique tactical options unavailable to any other faction:

| Ability | Effect | Cooldown | Power Cost |
|---------|--------|----------|------------|
| System Hack | Disable one enemy system for 15s | 60s | High |
| Sensor Flood | Blind all enemies in area for 10s | 45s | Medium |
| Jump Calculation | Instant emergency jump (no normal warmup) | 120s | Extreme |
| Shield Harmonic | Double shield effectiveness for 20s | 90s | High |
| Targeting Override | Perfect accuracy for 10s | 30s | Medium |
| Comm Intercept | Reveal all enemy positions/intentions | Passive | Low |
| Nano-Repair | Repair hull damage during combat | 180s | High |
| Decoy Fleet | Project false fleet signatures | 60s | Medium |

**Balance:** Kestrel abilities are powerful but energy-intensive. Using them drains power from weapons/shields.

---

## Jump Drive Mechanics in Combat

The jump drive is both travel AND tactical tool:

| Use | Mechanic |
|-----|----------|
| Escape | Jump away when losing (cooldown penalty) |
| Ambush | Jump into combat at ideal position |
| Repositioning | Short micro-jumps to reposition mid-fight |
| Pursuit | Track enemy jump signature to follow |

**Constraints:**
- Jump drive requires warmup time (vulnerable period)
- Cooldown after each jump (can't spam)
- Kestrel reduces warmup/cooldown (key advantage)
- Jump interdiction fields prevent escape (enemy tactic)

---

## Combat Scenarios (Book-Inspired)

### 1. "The Kestrel Gambit"
- Player is massively outgunned
- Must use Kestrel abilities + deception to win
- Victory condition: disable/destroy flagship only

### 2. "Wormhole Defense"
- Defend a wormhole chokepoint against waves
- Manage positioning and ammunition
- Reinforcements on timer

### 3. "Hit and Run"
- Jump in, destroy specific target, jump out before fleet responds
- Time pressure + stealth focus

### 4. "Fleet Engagement"
- Command multiple ships against enemy fleet
- Tactical coordination, focus fire, formation management

### 5. "Stealth Approach"
- Navigate through enemy detection grid without being seen
- Use ECM, low power, and timing

### 6. "Boarding Defense"
- Kethari boarding pods incoming
- Switch to internal ship defense (FPS/tactical view)
- Protect critical systems

---

## Damage Model

### Ship Damage States

```
100% ──── Full Combat Capability
 75% ──── Minor damage: cosmetic, slight performance loss
 50% ──── Moderate: systems start failing randomly
 25% ──── Critical: major systems offline, fires, hull breaches
 10% ──── Crippled: minimal function, crew casualties
  0% ──── Destroyed
```

### System Damage (Subsystem Targeting)

Players and enemies can target specific systems:

| Target | Effect When Damaged |
|--------|-------------------|
| Engines | Reduced speed/maneuverability |
| Weapons | Reduced fire rate or disabled weapons |
| Shields | Reduced capacity or failed regen |
| Sensors | Reduced detection, targeting accuracy |
| Jump Drive | Cannot escape, extended cooldown |
| Reactor | Reduced total power output |
| Life Support | Crew attrition over time |

---

## AI Behavior by Faction

| Faction | Combat AI Pattern |
|---------|-------------------|
| Kethari | Aggressive rush, swarm, attempt boarding |
| Velnari | Defensive formation, focus shields, outlast |
| Vorathis | Stealth approach, alpha-strike, disengage if losing |
| Oruneti | Stay at range, deploy drones, flee if mothership threatened |
| Vexari | EW dominance, confuse, never fully commit |

---

## Difficulty Scaling

| Difficulty | Enemy Behavior | Player Resources |
|------------|---------------|-----------------|
| Cadet | Predictable AI, generous ammo/power | Full Kestrel access |
| Captain | Smart AI, standard resources | Standard Kestrel cooldowns |
| Colonel | Adaptive AI, scarce resources | Extended Kestrel cooldowns |
| General | Brutal AI, realistic scarcity | Limited Kestrel, no hints |

---

## Design Notes

- Combat should reward thinking over reflexes (accessible to strategy fans)
- Kestrel is the "difficulty equalizer" - more help on easier modes
- Every fight should feel survivable with the right tactics
- Running away is always a valid strategy (desperate improvisation under pressure)
- Resource scarcity (ammo, power) creates meaningful choices
- Environmental tactics (asteroid cover, nebula blindness) add depth
- The "pause and plan" option respects the cerebral nature of the source material
