# Iron Exiles - Real-Time Combat and Point Defense System

## Game Design Document: Saturation Combat, PD, and Continuous Fire

---

## Combat Philosophy: EVE-Inspired Real-Time Warfare

Iron Exiles combat is **real-time, continuous-fire dogfighting** modeled after EVE Online's core tension: you win by saturating your target's defenses faster than they can defend against you. There is no alpha-strike one-shot. Every weapon fires continuously within its reload cycle, every point-defense system tracks inbound ordnance in real time, and every shield needs time to recharge between bursts of incoming fire.

### Core Tension

The entire game reduces to two equations the player constantly balances:

Offense: Total DPS output (after ECM interception) > Target's sustainable damage tolerance
Defense: Enemy's total DPS input > Your PD intercept rate OR your shield regen + effective HP

This means combat is not about finding the perfect opening. It is about **sustained pressure**, managing power through long engagements, and choosing between punching through enemy defenses or outlasting their ability to regenerate them.

### Combat Pacing

| Aspect | Design |
|--------|--------|
| Time to detect target | 0-15 seconds (sensor range dependent) |
| Close-to-close combat | 30-90 seconds depending on relative speed and jump cooldowns |
| Typical engagement length | 2-8 minutes for solo, 5-20+ minutes for fleet |
| Weapons fire rate | Continuous within reload cycles. Railguns cycle every 1-3 seconds, beams are sustained, missiles fly independently |
| PD response | Real-time automatic interception of inbound projectiles and missiles within tracked slot count |

### What This Means for Gameplay

- No cooldown rotations to manage. Weapons fire automatically while locked. The player manages **aiming, positioning, and power routing**.
- There is no big hit per shot. Damage ticks constantly as long as weapons are tracking. A beam weapon at 10 DPS deals 600 damage over 60 seconds of continuous fire, not in one moment.
- Shields regenerate when not under fire at a rate determined by the shield generator power allocation and tier. If your target is firing faster than regen can recover, their effective HP is just base shields divided by incoming DPS ratio.
- PD intercepts are rate-limited. You can only defend so many inbound projectiles per second. Overwhelm the number or penetrate through with weapons that bypass PD (gravidic beams, phased railguns).
---

## Point Defense System

Every ship has a **point defense system** that automatically tracks and destroys incoming threats. It is a modular slot-based asset like all other ship systems, upgradeable tier-by-tier, and one of the most impactful defensive choices in the game.

### How PD Works (In-Game)

PD is **fully automatic** -- the player does not aim or activate it. The limits are purely mechanical:

1. Tracking slots: How many inbound threats can be tracked and engaged simultaneously (e.g., 6 slots = 6 separate targets at once).
2. Intercept rate: Total number of interceptor rounds fired per second across all active slots, split evenly among tracked targets.
3. Interceptor damage: Damage each interceptor round deals to the incoming threat.
4. Ammo capacity: Finite ammo pool; if depleted, PD goes dark until the ship docks or gets field-repaired.

### Threat Categories and PD Effectiveness

| Threat Type | How PD Intercepts | Notes |
|-------------|-------------------|-------|
| Missiles and torpedoes | Full interception. Interceptors destroy incoming missile before impact. | Primary purpose of PD. Tier-4+ PD can shred most standard missiles easily. |
| Kinetic projectiles (railgun slugs) | Partial penetration. PD intercepts some, others pass through based on effective HP vs interceptor damage. | Harder railgun slugs overwhelm lower-tier PD. High-tier railguns saturate even good PD over time. |
| Energy beams | Not intercepted. Beams are light-speed and continuous. | PD does nothing against beams. You must survive with shields or dodge. |
| Plasma area effects | Partial interception. Can intercept plasma warheads before detonation if launched separately. | Direct plasma lances (instant fire) cannot be intercepted. |

### PD Stats Table by Tier

| Tier | Tracking Slots | Intercept Rate (rounds/s) | Interceptor Damage | Ammo Capacity | Weight (tons) | Cost (credits) |
|------|---------------|---------------------------|--------------------|---------------|---------------|----------------|
| 1 | 2 | 4 | 50 | 500 rounds | 3 | 15,000 |
| 2 | 4 | 8 | 120 | 1,200 rounds | 6 | 45,000 |
| 3 | 6 | 15 | 250 | 2,500 rounds | 10 | 100,000 |
| 4 | 8 | 25 | 500 | 5,000 rounds | 16 | 220,000 |
| 5 | 10 | 40 | 1,000 (phased) | 8,000 rounds | 24 | Quest Only |

### PD Slot Limits by Ship Size

| Ship Size Class | Max PD Hardpoints | Effective Slots at Tier 3 |
|-----------------|-------------------|--------------------------|
| Corvette/Interceptor | 1 | 6 |
| Frigate | 1-2 | 6-12 |
| Destroyer | 2 | 12 |
| Cruiser | 2-3 | 12-18 |
| Heavy Cruiser | 2-3 | 12-18 |
| Battleship | 3-4 | 18-24 |
| Dreadnought+ | 4 | 24 |

### PD Loadout Strategy

PD represents a trade-off: every slot it occupies is one fewer slot for weapons. The most effective ships balance offensive saturation against defensive coverage:

- **Tank build:** Heavy PD (Tier 3-4) + heavy shields + reactor focused on shield regen. Survives sustained fire from multiple attackers.
- **Glass cannon build:** Minimal PD (Tier 1 or no PD) + maximum beam/railgun slots. High damage output, must rely on speed and distance to survive.
- **Hybrid build:** Tier 2-3 PD across two hardpoints (4-6 slots total), moderate weapon loadout. The standard for fleet fighters who need both offense and defense.

### Saturation Under PD

The key combat interaction: **PD has a maximum intercept rate per slot.** If you fire more projectiles at a ship than its PD can track and destroy, the excess penetrates through. This is the core saturation mechanic.

Example engagement with Tier-3 PD (6 tracking slots, 15 intercept rounds/s total):

If the attacker fires 8 simultaneous inbound threats, only 6 can be tracked. The remaining 2 missiles strike unintercepted. If the railgun is firing slugs faster than the total intercept rate can destroy them (e.g., 3 slugs/s but PD can only fire 2.5 rounds/s per slot), the excess penetrates shield-to-hull.

The saturation equations:
- Incoming missiles > PD tracking slots = guaranteed penetration of excess
- Incoming slug rate > PD intercept rate = guaranteed penetration over time
---

## Shield Regeneration as a Core Limiting Mechanic

Shields regenerate when not under fire. The **regen rate** is the critical number that determines how much downtime a ship needs between engagements, and whether incoming sustained damage can outpace regeneration.

### Shield Regeneration Formula

Effective Shield HP = Base Shields x (1 + Stat Scaling from Level) x Tier Multiplier x Reactor Allocation %
Regen Rate (HP/s) = Effective Shield HP / (Base Regen Time) x Tier Multiplier x Reactor Allocation %

### Shield Generator Stats by Tier

| Tier | Base Shield HP | Regen Time (seconds) | Weight (tons) | Cost (credits) |
|------|---------------|---------------------|---------------|----------------|
| 1 | 800 | 30s | 5 | 8,000 |
| 2 | 2,500 | 24s | 9 | 35,000 |
| 3 | 6,000 | 18s | 15 | 100,000 |
| 4 | 12,000 | 12s | 24 | 275,000 |
| 5 | 25,000 (Elder) | 6s (effectively instant) | 38 | Quest Only |

### Shield Regeneration Examples

**Scenario: Velnari Citadel with Tier-3 shield generator, reactor at 30% to shields, level 50 scaling**
- Effective HP: 6,000 x 1.4 (Level 50 scaling) = 8,400
- Regen rate: 8,400 / 18s x 0.30 = ~140 HP/s

If an enemy is dealing 200 DPS to shields, they are outpacing regen by 60 HP/s. The effective shield is being consumed at a faster rate than it recovers. After the fight ends, it takes longer to fully recover than during combat damage is sustained.

**What this means for combat:**
- If your outgoing DPS < enemy shield regen x 2, you will never win a prolonged engagement. You must either increase your DPS (upgrade weapons, add hardpoints), reduce their effective shields (focus fire to deplete them below full), or outmaneuver them into a position where they cannot maintain all their weapon angles.
- A ship taking less damage than its shield regen per second will effectively have **infinite shields** until it takes a burst large enough to exceed the regen rate. This makes sustained multi-ship attacks against tanky targets highly effective -- even if no single ship can kill them, the combined DPS overwhelms regeneration.

### Shield Saturation: The Regen Drain

When incoming DPS exceeds shield regen, you are in **shield drain mode**. Your shields are effectively being consumed as fast as they regenerate plus your base pool divided by (incoming DPS - regen rate). This is why fleet coordination matters -- three ships each dealing 50 DPS may not trigger drain against a ship with 80 HP/s regen, but together they deal 150 DPS which triggers severe drain.

| Incoming DPS | Ship Shield Regen | Net Drain Rate | Shields at Full (Tier-3) | Time to Deplete |
|-------------|-------------------|----------------|-------------------------|----------------|
| 50 DPS | 140 HP/s | Negative (recovering) | 8,400 | Never recovers if fire is continuous |
| 140 DPS | 140 HP/s | Zero (stalemate) | 8,400 | Stays flat while under fire |
| 200 DPS | 140 HP/s | 60 HP/s drain | 8,400 | ~140 seconds to deplete fully |
| 500 DPS | 140 HP/s | 360 HP/s drain | 8,400 | ~23 seconds to breach shields |
---

## Weapon Fire Rates and Continuous Damage Model

Weapons fire in **continuous cycles**, not turn-based discrete hits. The player locks onto a target and weapons auto-track within their engagement zone.

### Railgun Fire Rate (Per Hardpoint)

| Tier | Cycles per Minute | Time Between Shots | DPS (continuous, average) | Weight (tons) |
|------|-------------------|--------------------|---------------------------|---------------|
| 1 | 120 cpm | Every 0.5s | ~80 | 3 |
| 2 | 90 cpm | Every 0.67s | ~180 | 6 |
| 3 | 60 cpm | Every 1.0s | ~280 | 9 |
| 4 | 45 cpm | Every 1.33s | ~350 | 13 |
| 5 | 30 cpm | Every 2.0s | ~400 | 18 |

### Beam Weapon Fire Rate (Per Hardpoint)

| Tier | Output Type | DPS (continuous, locked) | Heat Generation | Weight (tons) |
|------|------------|--------------------------|-----------------|---------------|
| 1 | Sustained beam | ~50 vs shields/50 vs hull | Low | 2 |
| 2 | Sustained beam | ~140 vs shields/120 vs hull | Medium | 4 |
| 3 | Sustained beam | ~260 vs shields/200 vs hull | High | 7 |
| 4 | Sustained beam | ~375 vs shields/290 vs hull | Very high | 10 |
| 5 | Phased beam (penetrates shields) | ~600 vs all | Extreme | 15 |

### Missile Damage Delivery (Per Rack)

Missiles are launched as discrete events but their damage applies over time as they fly:

| Tier | Launch Speed | Flight Time (medium range) | Impact Damage | PD Interception Difficulty |
|------|-------------|---------------------------|---------------|---------------------------|
| 1 | 2/s | 4s | 300 | Easy (Tier-1+ PD kills instantly) |
| 2 | 1.5/s | 6s | 750 | Moderate (requires Tier-2+ PD) |
| 3 | 1/s | 8s | 1,500 | Hard (requires Tier-3+ PD) |
| 4 | 0.5/s | 10s | 3,500 | Very hard (Tier-4+, phased interceptors recommended) |
| 5 | 0.25/s | 15s (elder) | 8,000 | Nearly impossible without Tier-5 PD or countermeasures |

### Key Insight: DPS is Sustained, Not Spiky

A ship with 6 beam hardpoints at Tier-3 deals roughly **1,560 DPS** continuously while locked onto a target. No single shot is devastating, but over 30 seconds of continuous fire, that is ~47,000 damage to shields alone (or hull damage once shields are gone). This rewards:

- Piloting into position and holding your aim on the same target for as long as possible
- Fleet coordination -- multiple ships focusing the same target accumulate DPS faster than any single ship can defend against
- Shield depletion management -- the first few seconds of engagement do less useful damage because shields absorb most of it. The real damage comes after shields crack and hull is exposed

### Effective PD Range and Penetration

Different weapons have different effective ranges that affect how a target's PD must respond:

| Weapon Type | Effective Range | PD Engagement Time |
|-------------|---------------|-------------------|
| Railgun slugs | Long (3-8 sectors) | Instant -- PD must track from detection range to impact |
| Beams | Medium-long (2-5 sectors) | Continuous -- no interception possible, shields must absorb continuously |
| Missiles | Very long (self-guided) | Flight time dependent -- PD has 4-15 seconds of warning before impact |
| Plasma lances | Medium (1-3 sectors) | Instant -- like beams, cannot be intercepted |

This means **missile-heavy ships have more time to exploit PD saturation** because missiles give defenders multiple seconds to react. Railgun slugs hit almost instantly but can still overwhelm PD if the slug rate exceeds intercept capacity. Beam weapons completely bypass PD and rely purely on DPS output vs shield HP and regen.
---

## Power Management During Combat

The reactor produces finite power. During combat, you must allocate it dynamically between weapons, shields, engines, and ECM/AI systems. This is the primary tactical control during engagements.

### Power Allocation Layout

```
[REACTOR OUTPUT: 100% total capacity]

Weapons   [|||||_____] ~50% -> determines beam sustained output and railgun cycle speed
Shields   [|||_______] ~30% -> determines shield HP and regen rate
Engines   [|_________] ~12% -> determines speed and maneuverability
ECM/AI    [|_________ ] ~8% -> powers Kestrel, jamming, targeting assists

[Total must not exceed 100%. If it does, the weakest-priority system drops first.]
```

### Power Allocation Combat Tactics by Situation

| Situation | Weapons | Shields | Engines | ECM | Trade-off |
|-----------|---------|---------|---------|-----|-----------|
| Attacking heavy shield target | 65% | 20% | 10% | 5% | You take more damage but deal it faster |
| Under heavy fire, trying to survive | 30% | 40% | 15% | 15% | Less DPS but shields stay up longer |
| Running from superior foe | 10% | 20% | 45% | 25% | Escape faster but cannot fight back |
| Fleet engagement, holding position | 50% | 30% | 10% | 10% | Balanced -- sustain combat while maintaining formation |
| Maximizing PD uptime (sustained defense) | 40% | 35% | 15% | 10% | Shields and regen stay up, weapons still active |

**Power redistribution takes effect within 2 seconds.** During those 2 seconds, your ship is in a transition state: weapons may flicker as they drop power, shields briefly dip before stabilizing at the new level. Smart players time their power shifts to coincide with missile volleys or weapon reload windows for minimal disruption.

### Power and Shield Regeneration Interaction

This is critical: **shield regeneration rate is directly proportional to reactor allocation.** If you divert shield power to weapons mid-engagement, your regen rate drops immediately, making your shields even more vulnerable to incoming DPS:

| Scenario | Shields Allocation | Regen Rate (Tier-3 at L50) | Effective Shield HP |
|----------|-------------------|---------------------------|--------------------|
| Default (30% to shields) | 140 HP/s | 8,400 | Normal combat balance |
| Combat push (20% to shields) | 93 HP/s | 5,600 | Shields drain faster but your beams deal more damage |
| Emergency tank (45% to shields) | 210 HP/s | 12,600 | More regen but less DPS output -- can you kill before they deplete? |

The combat tension here is fundamental: diverting power to weapons increases your offensive pressure while simultaneously reducing your defensive recovery. There is no optimal answer -- it depends on the specific fight and whether you are trying to kill first or survive first.
---

## Core Saturation Tactics

There are three primary ways to defeat an enemy: overwhelm their PD tracking, deplete shields faster than regen, or saturate railgun slugs through the intercept gap. In practice, the best tactics combine all three simultaneously.

### Method 1: Saturation by Overwhelming PD Tracking Slots

Fire more simultaneous incoming threats than the enemy's PD can track at once.

| Attacker Setup | Simultaneous Threats Generated | Defender PD (Tier-3) | Result |
|---------------|-------------------------------|---------------------|--------|
| 3 missile racks firing (2/s each = 6 missiles) + 4 railgun hardpoints | 10 simultaneous inbound threats | 6 tracking slots | 4 missiles penetrate unintercepted, plus some railgun slugs if PD splits capacity |
| 6 missile racks firing (2/s each = 12 missiles) | 12 simultaneous inbound threats | Tier-5 PD (10 slots) | 2 missiles penetrate -- still effective with high-tier warheads |
| Mixed: 4 missiles + railgun barrages at staggered timing | Threats arrive in waves, never all simultaneously but tracking is split across different threat types | Same PD splits slots between missile-tracking and slug-penetration modes | More efficient than pure missile saturation -- forces PD to manage multiple intercept strategies |

### Method 2: Saturation by Out-Pacing Shield Regen

Maintain continuous DPS on a target higher than their shield regeneration rate.

| Defender Shield Setup | Effective HP at L50 | Regen Rate | Attacker Needs This Much Continuous DPS |
|----------------------|--------------------|------------|---------------------------------------|
| Tier-2 + 30% reactor | ~750 HP | ~14 HP/s | >14 sustained DPS to drain over time |
| Tier-3 + 40% reactor | ~2,400 HP | ~46 HP/s | >46 sustained DPS |
| Tier-4 + 50% reactor | ~7,500 HP | ~1,562 HP/s | Fleet-level coordination required (8+ ships) |

**The shield drain threshold:** If attacker DPS < regen rate, the defender's shields never go down during combat. They effectively have infinite HP until an attack comes that exceeds regen. This is why fleet attacks are essential against heavy tanks -- no single ship can exceed their regen, but combined they easily do.

### Method 3: Railgun Slug Rate Overload

Fire railguns faster than the PD intercept rate can destroy the slugs. Each slug must be individually killed by an interceptor round. If 3 slugs arrive per second and PD fires only 2 rounds/s, one penetrates every second forever.

| Railgun Barrage | Slugs/Second Incoming | PD Intercept Rate (Tier-3) | Penetration Result |
|-----------------|----------------------|---------------------------|-------------------|
| 3 railguns @ Tier-2 | ~4.5 slugs/s total | 15 rounds/s | No penetration -- PD kills all slugs before impact |
| 6 railguns @ Tier-4 | ~4.5 per gun = 27 slugs/s | 15 rounds/s | 12+ slugs penetrate every second. Hull vaporized in seconds. |
| Mixed saturation: 3 rails + 6 missiles at staggered intervals | Railgun + missiles compete for same PD ammo pool | PD must split intercept capacity across both types | Most efficient breakdown strategy -- PD cannot optimize for both simultaneously |

### The Multi-Vector Saturation (Best Case for Attackers)

The optimal attack fires from multiple angles simultaneously:

1. **Missile swarm** exhausts PD tracking slots
2. **Railgun barrage** saturates remaining intercept capacity
3. **Beam weapons** apply continuous DPS that bypasses PD entirely while the enemy is distracted by intercepting other threats

At this point, the target is taking damage from three independent sources that their PD cannot address simultaneously. This is fleet combat at its most effective: a coordinated attack where each ship handles a different saturation layer.
---

## Ship Role Archetypes (EVE-Inspired)

### Heavy Cruiser / Battleship - Tank

| Aspect | Details |
|--------|---------|
| Primary role | Sustain the longest engagements, hold position in fleet line |
| Typical loadout | Tier 3-4 PD (6-8 slots), heavy shield generator, 6-8 beam hardpoints, moderate railguns |
| Playstyle | Hold target lock and fire continuously. Power allocation skewed toward shields (35-40%). In a fleet, this ship is the anvil that other ships hammer against. |
| Key stat to maximize | Shield HP + regen rate |
| Vulnerability | Slow maneuverability; vulnerable if flanked by fast interceptors with saturation missile loadouts |

### Destroyer / Frigate - Interceptor

| Aspect | Details |
|--------|---------|
| Primary role | Rapid strike, PD saturation via overwhelming numbers |
| Typical loadout | Minimal PD (Tier 2, 4 slots), maximum missile racks (6+) + railgun hardpoints for slug saturation |
| Playstyle | Sprint in, dump full missile volley to overwhelm enemy PD tracking slots, fire railguns at the rate that exceeds PD intercept capacity, sprint out. High risk, high reward. |
| Key stat to maximize | Speed + weapon hardpoint density |
| Vulnerability | Cannot survive prolonged engagement -- must win quickly or disengage before shields are drained |

### Cruiser / Heavy Cruiser - Turret

| Aspect | Details |
|--------|---------|
| Primary role | Sustained damage over long engagements |
| Typical loadout | Tier 3 PD (6 slots), all beam weapons (8-10 hardpoints), moderate reactor to weapons |
| Playstyle | Hold position in fleet formation, maintain lock on one target continuously. DPS adds up over minutes. The most reliable damage source in fleet combat. |
| Key stat to maximize | Sustained DPS per second while maintaining lock |
| Vulnerability | If enemy flanks or ECM-blinds you, your reliance on continuous tracking becomes a liability |

### Corvette / Fighter - Rapid Fire Harasser

| Aspect | Details |
|--------|---------|
| Primary role | Fast harassment, PD slot exhaustion |
| Typical loadout | Tier 1 PD (2 slots -- barely enough), all railguns or plasma weapons for rapid-fire saturation |
| Playstyle | Circle at close range, fire continuously. Job is to make the enemy waste PD ammo on you while fleet mates do the real damage from outside PD range. |
| Key stat to maximize | Speed + agility + reload speed |
| Vulnerability | Extremely fragile -- one sustained beam lock from a turret ship and you are dead |

### Carrier - Swarm Launcher

| Aspect | Details |
|--------|---------|
| Primary role | Deploy missile swarms and drone strikes that overwhelm any conceivable PD response |
| Typical loadout | Minimal PD, all missile racks (12+ hardpoints), drone bays |
| Playstyle | Stay at long range, launch waves of missiles that exhaust enemy PD ammo. When their PD is depleted, the fleet pushes in for the kill. |
| Key stat to maximize | Missile rack count + missile warhead tier |
| Vulnerability | Slow turning and weak self-defense -- if an interceptor closes distance before your missiles land, you are in trouble |

### Drone Carrier - Automation Platform

| Aspect | Details |
|--------|---------|
| Primary role | Persistent harassment that does not consume player attention |
| Typical loadout | Tier 2-3 PD (supporting the drones presence), drone management systems, moderate beam weapons for self-defense |
| Playstyle | Direct AI-companion or manual control of drone swarm. Drones provide continuous low-level damage that adds up over engagement time and drains enemy resources passively. |
| Key stat to maximize | Drone count + autonomous targeting accuracy |
| Vulnerability | If drones are overwhelmed, the carrier must rely on its own moderate defenses -- which were designed for survival, not offense |

---

## Fleet Combat Tactics

### The Anvil-and-Hammer Formation

Tank ships (heavy cruisers/battleships) hold position as an **anvil** -- their PD absorbs incoming fire while their sustained DPS chips away at the enemy line. Interceptor/rapid-fire ships swarm in as **hammers** -- overwhelming the tanked target's PD with saturation fire while the enemy is busy dealing with the anvil.

Result: Enemy PD gets saturated on both fronts simultaneously. The tank absorbs the brunt, the hammer delivers the kill.

### Spread Formation (PD Exhaustion)

Multiple ships attack from different angles and vectors. Each ship forces the enemy's PD to track new threats in multiple directions -- splitting tracking slots unevenly. Ships fire missile volleys with staggered timing so that each volley arrives while the enemy is still recovering from the previous one. Result: PD ammo depletes faster than it can be sustained; eventually a penetration wave hits.

### Swarm Tactics (Kethari-Style)

8-20 smaller ships attack as a single coordinated unit. Each ship fires enough missiles to require 1-2 PD tracking slots. Combined, the swarm requires 20+ simultaneous tracking slots -- only achievable with Tier-5 PD. Result: Enemy is forced to disengage or die. The saturation of tracking plus intercept capacity cannot be overcome by any single defensive setup.

### Long-Range Attrition (Oruneti-Style)

Ships maintain distance outside enemy effective range. Continuous beam fire accumulates over minutes, slowly draining shields and PD ammo. Shield regen cannot keep up because the ships never let the target recover fully -- just enough damage to keep regen busy while fleet advances position by position. Result: Enemy runs out of shield HP before they can close to effective firing range.

### Flanking Maneuver (Vorathis-Style)

One force engages the enemy frontally while themain force approaches from behind via stealth or ECM masking. The flanking force attacks with all weapons unimpeded -- the enemy PD must split its resources between front and rear, and its shields have weakest points at the aft arc. Result: devastating hull exposure on the rear of the target.
---

## Combat Flow (Real-Time)

### Step-by-Step Engagement Cycle

```
1. DETECT (0-15s)     -- Sensors pick up target signature, identify class and armament
2. POSITION (15-60s)   -- Approach into weapon range while PD scans for incoming threats
3. INITIAL CONTACT (immediate) -- First railgun slugs fly, beams begin tracking, PD auto-engages inbound threats
4. SUSTAINED ENGAGEMENT (continuous) -- Both ships fire continuously; shields take damage; PD ammo drains
5. SHIELD BREACH (variable time)    -- Once incoming DPS > shield regen, shields fall and hull is exposed
6. DECISION POINT                  -- Does the attacker push through PD saturation to finish the kill? Or does the defender break line-of-sight and retreat for shield recovery?
7. RESOLUTION                       -- Ship destroyed, disabled, or disengaged (retreated to shield regen range)
```

### What Each Phase Feels Like to the Pilot

| Phase | Visual Cues | Player Actions |
|-------|-------------|---------------|
| Detect | Target icon on HUD; sensor sweep ping | Assess threat level. Can I take this alone? Should I call for backup? |
| Position | Closing range indicator; weapon ranges light up green as you come in range | Adjust vector for optimal firing angle; lock weapons on target; begin power allocation |
| Initial contact | First missile impacts light up on shields (or PD intercept flashes), railgun tracers streak past | Watch shield bar -- is it holding? Is PD ammo counting down? |
| Sustained engagement | Shields slowly draining, hull taking heat warnings, enemy doing the same to you | Adjust power allocation: more weapons if they need pressure, more shields if incoming fire exceeds regen |
| Shield breach | Shield effect flickers and drops; you see the target hull for the first time | All guns -- maximum power to weapons. This is the moment to commit or disengage. |
| Decision point | Target at 30% HP but your PD ammo is at 15%. Your shields are at 80% (regen holding steady) | Fight it out (can you burst them before PD dies)? Or break line and retreat? |
| Resolution | Ship explodes/escapes/disables. New target in sensor range or jump cooldown countdown begins | Assess damage state; redistribute power; prepare for next engagement |

---

## Key Combat Takeaways

1. **There is no one-shot kill.** Every ship must be worn down through sustained DPS over time. Positioning to maintain lock and surviving long enough to deal the damage matters more than any single weapon choice.

2. **PD is the primary defense bottleneck.** A ships real survivability is determined by PD intercept rate plus ammo pool, not just shield HP. Out-think your opponent: saturate their tracking first, then overwhelm their intercept capacity, and finally deplete armor through the gaps.

3. **Shield regen makes sitting still dangerous for attackers.** If a tanky ship can sit at range and regenerate shields between burst windows, they effectively have infinite HP against attackers who cannot maintain continuous DPS above regen rate. Fleet coordination is mandatory for taking down heavy targets.

4. **Power allocation is the most important tactical decision during combat.** Moving power from shields to weapons mid-engagement can mean the difference between killing your target before it kills you and getting eaten by sustained beam fire. The 2-second transition window means this must be timed around reload cycles or missile impacts for minimal effectiveness loss.

5. **Fleet combat is where saturation really matters.** In a fleet engagement, every ship DPS adds up. A single ship with Tier-4 PD cannot hold against 3+ attackers from different angles -- the tracking slots simply run out before their intercept capacity. Fleet coordination is the primary determinant of outcome in major engagements.

6. **Every defense can be overcome if approached from the right combination of vectors.** PD handles missiles well but does nothing to beams. Shields absorb beam DPS until they crack. PD tracking saturates on missile count but not on slug volume. The player who understands which layer to attack first wins -- and the fleet that attacks multiple layers simultaneously wins decisively.

7. **Retreating is a valid tactical choice.** When shields are drained and PD ammo is low, breaking line-of-sight and running for shield regen range is not failure -- it is preserving your ship for the next engagement. The game rewards knowing when to fight and when to live to fight another time.

