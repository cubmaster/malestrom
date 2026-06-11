
# Iron Exiles - Ship Assets & Equipment

## Game Design Document: Wearable Components & Depreciation System

---

## Overview

Ships are modular platforms. Every ship has a set of **slots** (beam slots, railgun slots, missile racks) into which players install **assets**. Each asset is a component with:

- A **tier** (1 through 5, higher = better performance but more expensive and heavier)
- A **condition rating** from 100 down to 0, representing wear over time
- An **effectiveness modifier** that scales with condition
- A **depreciation rate** per game turn (how fast it degrades)

### Depreciation Rules

All assets degrade by their listed depreciation rate each game turn.

| Condition Range | Effectiveness Modifier |
|-----------------|----------------------|
| 50% - 100% | Full effectiveness (100%) |
| 26% - 49% | Effective at `(condition / 50) * 100` percent of listed value |
| 0% - 25% | Broken; asset disabled until repaired or replaced |

**Example:** A beam weapon at condition 38 is effective at 76% (38/50 = 0.76) of its listed damage and range. Its heat generation remains full, so it will overheat faster than a fresh unit would.

### Repair & Replacement

- **In-field repair** restores condition by the ship's maintenance bay tier (see below).
- **Station repair** fully restores condition at a cost proportional to the asset tier.
- **Emergency field fix** (Kestrel intervention or manual jury-rig) temporarily sets condition to 30-40 but restores functionality during combat without resupply.

### Maintenance Bay Tiers (per turn restoration in port or safe harbor)

| Bay Tier | Turns Restored Per Turn | Cost Multiplier |
|----------|-----------------------|-----------------|
| None (no bay) | N/A | Repair costs x2 |
| Tier 1 (basic) | +5 | 1.0x |
| Tier 2 (standard) | +15 | 1.3x |
| Tier 3 (advanced) | +25 | 1.6x |
| Tier 4 (Elder-modded) | +40 | 2.0x |

---

## Asset Categories

---

### Energy Beam Weapons

Continuous-fire energy turrets optimized against shields. Draw from reactor power; no ammunition cost but generate heat per shot.

| Tier | Name | Damage Per Shot | Heat Per Shot | Range (units) | Depreciation / Turn | Weight (tons) |
|------|------|-----------------|---------------|---------------|---------------------|---------------|
| 1 | IR-A1 Infantry Beam | 120 | 8 | 3 sectors | 4 | 2 |
| 2 | KV-Spectral Mk.II | 280 | 16 | 5 sectors | 3 | 4 |
| 3 | Vorathis Prism Lance | 520 | 30 | 8 sectors | 3 | 7 |
| 4 | Oruneti Resonance Emitter | 750 | 45 | 12 sectors | 2 | 10 |
| 5 | Elder Phase Weaver | 1,200 | 60 | 18 sectors | 1 | 15 |

**Notes:** Vorathis beams fire at higher voltage but run hot. Oruneti emitters use harmonic resonance for extended range. Elder-weavers can briefly "phase" shots through enemy shields for 30% extra hull damage on the first hit after decloak.

---

### Railgun Weapons

Kinetic projectile launchers optimized against hulls. High burst damage; ammo-limited but no heat generation. Slower against shielded targets (penetration requires multiple hits to burn through).

| Tier | Name | Damage Per Shot | Ammo Capacity / Reload Time | Range (units) | Depreciation / Turn | Weight (tons) |
|------|------|-----------------|----------------------------|---------------|---------------------|---------------|
| 1 | Kethari Slug-Caster M1 | 200 | 50 rounds / 3 turns | 4 sectors | 3 | 3 |
| 2 | Velnari Kinetic Driver | 480 | 80 rounds / 4 turns | 7 sectors | 2 | 6 |
| 3 | Oruneti Accelerator Array | 750 | 120 rounds / 5 turns | 11 sectors | 2 | 9 |
| 4 | Vorathis Grav-Rail | 1,100 | 150 rounds / 6 turns | 15 sectors | 1.5 | 13 |
| 5 | Elder Singularity Driver | 2,000 | 200 rounds / 8 turns | 25 sectors | 1 | 18 |

**Notes:** Vorathis grav-rails bend projectiles around shields for partial hull damage. Elder drivers create micro-singularities that punch through both armor and shield simultaneously.

---

### Railgun Darts (Ammunition)

Specialized kinetic ammo types loaded into railgun slots. Each dart type is a consumable supply; darts themselves are not separate assets but define what the railgun fires.

| Dart Type | Tier | Damage Multiplier | Special Effect | Depreciation / Turn (stored) |
|-----------|------|-------------------|---------------|------------------------------|
| Standard Slug | 1 | x1.0 | Baseline kinetic damage | N/A (non-degrading) |
|Armor-Piercing Dart | 2 | x1.5 vs hull | Burns through ablative armor fast | +0.5 per turn if ammo depleted in combat |
|EMP Round | 3 | x0.6 vs shields | Stuns systems, disables weapons for 1-2 turns | +1.0 per turn if ammo depleted in combat |
|Nanite Corrosive | 4 | x1.2 vs all | Permanent -10% armor integrity on hit (stacks) | +1.5 per turn if ammo depleted in combat |
|Elder Void Shell | 5 | x3.0 vs all | Tears temporary shield/hull breach for 3 turns | N/A (only 1 charge per Elder site; cannot be replenished) |

---

### Missile Launchers

Missile launch racks hold and fire guided munitions. The launcher is the asset; missiles are consumables loaded into the rack.

| Tier | Name | Max Missiles Loaded | Reload Time | Lock-On Range (units) | Depreciation / Turn | Weight (tons) |
|------|------|--------------------|-------------|-----------------------|---------------------|---------------|
| 1 | Kethari Swarm Pod | 4 missiles | 2 turns | 3 sectors | 3 | 2 |
| 2 | Velnari Volley Tube Mk.III | 8 missiles | 3 turns | 5 sectors | 2.5 | 4 |
| 3 | Oruneti Hex-Launch Array | 16 missiles | 4 turns | 9 sectors | 2 | 7 |
| 4 | Vorathis Hydra Rack | 24 missiles | 5 turns | 14 sectors | 1.5 | 10 |
| 5 | Elder Phase Launcher | 32 missiles | 6 turns | 22 sectors | 1 | 14 |

---

### Missiles (Consumable Munitions)

Loaded into missile racks. Different missile types have different damage profiles, lock-on ranges, and countermeasures susceptibility.

| Missile Type | Tier | Burst Damage | Max Range | Countermeasure Susceptibility | Stacked Degradation (per missile stored / turn) |
|-------------|------|-------------|-----------|------------------------------|-----------------------------------------------|
| Heat-Seeking Rocket | 1 | 350 | 4 sectors | Low (20%) | N/A (non-degrading) |
| Anti-Ship Torpedo | 2 | 800 | 7 sectors | Medium (35%) | +0.3 per turn in rack if not fired |
| EMP Missile | 3 | 600 + system stun | 9 sectors | Medium (30%) | +0.5 per turn in rack if not fired |
| Smart-Strike Cluster | 4 | 1,200 (split across 3 targets) | 13 sectors | High (50%) | +0.8 per turn in rack if not fired |
| Elder Warhead | 5 | 4,000 + permanent sector-wide sensor blind | 20 sectors | N/A (uncounterable) | N/A (only available as quest reward, never restocked) |

**Notes:** Smart-strike missiles split damage across multiple targets. All missiles can be jammed or intercepted by point defense; countermeasure susceptibility is the chance a Vorathis ECM system automatically defeats the missile's guidance.

---

### Jump Drives

FTL propulsion systems. Determines maximum jump distance per charge and speed of travel during transit (higher drive = faster arrival at destination).

| Tier | Name | Max Range (sectors) | Transit Speed (hours/game turn) | Failure Rate (below 50% condition) | Depreciation / Turn | Weight (tons) |
|------|------|--------------------|-------------------------------|-----------------------------------|---------------------|---------------|
| 1 | Kethari Jump Coil Mk.I | 2 sectors | 3 turns | 8% per jump attempt | 4 | 10 |
| 2 | Velnari Slipstream Drive | 4 sectors | 2 turns | 4% per jump attempt | 2.5 | 18 |
| 3 | Oruneti Phase Skipper | 6 sectors | 1 turn | 1% per jump attempt | 2 | 25 |
| 4 | Vorathis Grav-Warp Drive | 9 sectors | <1 turn | 0.5% per jump attempt | 1.5 | 35 |
| 5 | Elder Rift Walker | Unlimited (instant) | 0 turns | Never | None | 50 |

**Failure Effects:** Below 50% condition, the listed failure rate applies to every jump attempt. Failure causes: ship stranded in interstitial space (must wait 6+ turns for rescue or attempt emergency jump with 25% hull damage), random sector displacement (+/-3 sectors), or catastrophic drive rupture (-40% hull instant damage).

---

### Jump Capacitors

Energy storage banks that power the jump drive. Determines recharge speed and maximum simultaneous jumps before needing a full charge cycle.

| Tier | Name | Full Recharge Time (turns) | Max Bursts Before Full Charge | Energy Capacity | Depreciation / Turn | Weight (tons) |
|------|------|---------------------------|------------------------------|-----------------|---------------------|---------------|
| 1 | Standard Capacitor Bank | 50 turns (port) / 15 turns (combat routing) | 1 burst per 30 turns | 100 kJ | 4 | 5 |
| 2 | Velnari Accumulator Grid | 40 turns / 10 turns | 2 bursts per 25 turns | 200 kJ | 3 | 9 |
| 3 | Oruneti Flux Matrix | 30 turns / 7 turns | 3 bursts per 20 turns | 400 kJ | 2 | 14 |
| 4 | Vorathis Capacitor Core | 20 turns / 4 turns | 5 bursts per 15 turns | 800 kJ | 1.5 | 20 |
| 5 | Elder Singularity Cell | Instant | Unlimited | Infinite | None | 30 |

**Combat Routing Note:** Players can divert reactor power to the capacitor during combat (takes 1 turn to route), reducing recharge time by roughly half but leaving less power for weapons and shields that turn.

---

### Shield Generators

Primary defensive system. Determines base shield capacity, regeneration rate, and resistance type.

| Tier | Name | Max Shield HP | Regen Rate (HP/turn) | Resistance Type | Depreciation / Turn | Weight (tons) |
|------|------|--------------|---------------------|-----------------|---------------------|---------------|
| 1 | Kethari Barrier Field | 800 | 20/turn | Balanced | 3.5 | 4 |
| 2 | Velnari Fortress Shield | 2,500 | 40/turn | Armor-piercing resistant | 2 | 7 |
| 3 | Oruneti Hex-Ward Array | 4,000 | 65/turn | Energy-damage resistant | 1.5 | 11 |
| 4 | Vorathis Adaptive Phase Field | 6,000 | 100/turn (adaptive) | Adapts to last damage type taken | 1 | 15 |
| 5 | Elder Quantum Curtain | 12,000 | 200/turn | All types (adaptive) | None | 22 |

**Adaptive Note:** Vorathis and Elder shields learn from incoming damage. After being hit by a railgun for 3 turns, the shield auto-optimizes to resist kinetic penetration (+15% kinetic resistance per cycle). This also means enemy ECM/hacking that targets shield frequency becomes more effective if the shield is constantly adapting.

---

### Armor Plating

Secondary defense behind shields. Only engaged when shields are depleted or bypassed. Provides passive damage reduction.

| Tier | Name | Base HP | Damage Reduction % | Repair Time (in port) | Depreciation / Turn | Weight (tons) |
|------|------|---------|-------------------|----------------------|---------------------|---------------|
| 1 | Kethari Hull-Plate A1 | 500 | 5% | 2 turns per point lost | 3 | 6 |
| 2 | Velnari Ablative Layer Mk.II | 2,000 | 12% | 4 turns per point lost | 2 | 10 |
| 3 | Oruneti Nano-Repair Mesh | 5,000 | 18% (auto-regenerates 5/turn) | Self-repairing | 1 | 12 |
| 4 | Vorathis Titanium-Carbide Plating | 10,000 | 25% | 10 turns per point lost | 0.5 | 18 |
| 5 | Elder Bio-Armor Shell | 20,000 | 35% (auto-regenerates 50/turn) | Self-repairing + active repair drones | None | 25 |

---

### ECM / Countermeasure Systems

Electronic warfare modules. Reduce enemy accuracy, break missile locks, and provide stealth detection capability.

| Tier | Name | Jamming Range (sectors) | Lock Break Chance | Stealth Detection Bonus | Depreciation / Turn | Weight (tons) |
|------|------|-----------------------|-------------------|------------------------|---------------------|---------------|
| 1 | Kethari Noise Generator | 2 sectors | 10% per turn | +1 sector detection | 3 | 2 |
| 2 | Velnari Decoy Emitters Mk.III | 4 sectors | 25% per turn | +3 sectors detection | 2 | 4 |
| 3 | Oruneti Holo-Phase Jammer | 6 sectors | 40% per turn | +5 sectors detection (can project false signatures) | 1.5 | 6 |
| 4 | Vorathis Black-Bank ECM Suite | 9 sectors | 65% per turn | +8 sectors detection (reveals cloaked ships in range) | 1 | 8 |
| 5 | Elder Null-Field Emitter | Unlimited | Instant lock break | Reveals all non-Elder signatures within sector | None | 12 |

---

### Reactor Systems

Power generation core. Determines total reactor capacity available for weapons, shields, jump capacitor routing, and ECM. Higher capacity = more systems can run at full power simultaneously.

| Tier | Name | Total Power (kW) | Power Per Turn Regen | Efficiency Bonus | Depreciation / Turn | Weight (tons) |
|------|------|-----------------|---------------------|---------------|---------------------|---------------|
| 1 | Kethari Fusion Cell Mk.I | 200 kW | 5 kW/turn | Baseline | 4 | 8 |
| 2 | Velnari Reactor Block II | 500 kW | 15 kW/turn | +10% shield regen when excess power available | 2.5 | 14 |
| 3 | Oruneti Harmonic Core | 900 kW | 30 kW/turn | +15% beam weapon efficiency when routing to beams | 2 | 20 |
| 4 | Vorathis Quantum Reactor | 1,800 kW | 60 kW/turn | Dynamic allocation (auto-distributes optimal power split) | 1.5 | 28 |
| 5 | Elder Zero-Point Module | Unlimited | Instant full regen | All systems +25% efficiency when powered | None | 35 |

**Power Management:** When total demand exceeds reactor output, the ship auto-distributes available power proportionally (shields drop first, then beams, then rails, then ECM). Player can manually prioritize which systems get preferential power allocation.

---

## Asset Slot Limits by Ship Size

| Size Class | Beam Slots | Railgun Slots | Missile Racks | Shield Generator | Armor Plating | ECM Suite | Jump Drive | Jump Capacitor | Reactor |
|------------|-----------|---------------|---------------|-----------------|--------------|-----------|-----------|---------------|---------|
| Interceptor/Corvette | 1-2 | 1-2 | 0-1 | 1 | 1 | 0-1 | 1 | 1 | 1 |
| Frigate | 2-4 | 2-3 | 1-2 | 1 | 1 | 0-1 | 1 | 1 | 1 |
| Destroyer | 4-6 | 3-5 | 2-3 | 1-2 | 1 | 1 | 1 | 1 | 1 |
| Cruiser | 6-8 | 4-6 | 3-4 | 1-2 | 1-2 | 1 | 1 | 1-2 | 1-2 |
| Heavy Cruiser | 8-10 | 5-8 | 4-6 | 1-2 | 1-2 | 1 | 1 | 1-2 | 1-2 |
| Battlecruiser | 8-12 | 6-10 | 4-8 | 1-2 | 1-2 | 1 | 1 | 1-2 | 2 |
| Battleship | 10-14 | 8-12 | 4-10 | 1-3 | 2 | 1 | 1 | 1-2 | 2-3 |
| Dreadnought+ | 12-18 | 10-16 | 6-14 | 2-3 | 2-3 | 1-2 | 1-2 | 1-3 | 2-4 |

---

## Depreciation Example Walkthrough

### Scenario: A Vorathis Predator Cruiser in combat, running Tier-3 Oruneti beams and Tier-2 Velnari shields

**Starting condition (fresh fit):**
- Oruneti Resonance Emitter (Beam): condition 100, damage 750 per shot
- Velnari Fortress Shield: condition 100, shield HP 2,500

**After 10 turns of combat:**
- Beam degradation: -1.5 x 10 = -15. Condition now: 85 (still at full effectiveness)
- Shield degradation: -2 x 10 = -20. Condition now: 80 (still at full effectiveness)

**After 40 turns of sustained combat:**
- Beam condition: 85 - (1.5 x 30) = 40. Now below 50% threshold! Effective damage: (40/50) x 750 = **600 per shot** (20% penalty)
- Shield condition: 80 - (2 x 30) = 20. Now critically degraded! Effective shield HP: (20/50) x 2,500 = **1,000** (60% lost), regen rate also halved to ~16/turn

**After 70 turns of sustained combat:**
- Beam condition: 40 - (1.5 x 30) = -5 → **Broken.** Ship can attempt emergency field fix (sets to condition 35 for the duration of combat).
- Shield condition: 20 - (2 x 30) = -40 → **Broken.** Shields offline; ship must rely on armor plating.

**Repair timeline:**
- In port with Tier-2 maintenance bay: shield repair costs ~2 hours/game days, full restoration. Beam requires station visit due to critical failure.

### Combat Routing (Emergency Power Diversion)

Players can divert reactor power to the jump capacitor during combat:
- Takes 1 turn of diverted power (less available for weapons/shields that turn)
- Reduces capacitor recharge time by ~50%
- Useful when trapped or pursuing a fleeing enemy who might escape via jump

---

## Cost Reference (Credits per Asset at Tier)

| Tier | Beam Cost | Railgun Cost | Missile Rack Cost | Shield Cost | Jump Drive Cost | Capacitor Cost | ECM Cost | Reactor Cost | Armor Set Cost |
|------|----------|-------------|------------------|------------|----------------|---------------|----------|-------------|---------------|
| 1 | 5,000 | 4,000 | 3,000 | 8,000 | 12,000 | 6,000 | 2,000 | 7,000 | 3,000 |
| 2 | 25,000 | 18,000 | 15,000 | 35,000 | 50,000 | 28,000 | 12,000 | 30,000 | 15,000 |
| 3 | 85,000 | 65,000 | 50,000 | 100,000 | 150,000 | 80,000 | 40,000 | 80,000 | 45,000 |
| 4 | 250,000 | 180,000 | 140,000 | 275,000 | 400,000 | 200,000 | 120,000 | 200,000 | 110,000 |
| 5 | Quest Only | Quest Only | Quest Only | Quest Only | Quest Only | Quest Only | Quest Only | Quest Only | Quest Only |

**Station Repair Costs (percentage of asset cost):**
- Condition > 50%: 10% of original cost
- Condition 26-50%: 30% of original cost
- Condition < 26% (critical): 75% of original cost + mandatory 48-hour shipyard lockout

---

## Asset Compatibility Notes

- **Elder-tier assets** require a compatible power source (Tier-3+ reactor) or they operate at -1 tier effectiveness. An Elder beam on a Tier-2 reactor fires at Tier-3 levels.
- **Vorathis Phase Field shields** cannot coexist with Vorathis ECM suites on the same ship (both draw from the same frequency band; installing both causes mutual interference, reducing shield HP by 25% and jamming range by 40%). Player must choose one Vorathis specialty per ship.
- **Oruneti Harmonic Reactor** + **Oruneti Hex-Ward Array** create a synergy bonus: both systems share power routing, giving +10% additional shield regen for each system installed. Stackable with another Oruneti tier-3+ asset of the same faction for +25%.
- **Kethari swarm components** (mismatched low-tier assets from the same faction) do not create synergy but also do not have compatibility issues; they are deliberately standardized for quick fleet-wide swaps.

