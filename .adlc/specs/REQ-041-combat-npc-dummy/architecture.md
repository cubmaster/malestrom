# REQ-041 Architecture: Combat NPC Dummy & Minimal AI

## Overview

Server-side NPC ships with a minimal state-machine AI that can patrol, aggro on players, engage with beam weapons, and be destroyed. Reuses existing combat infrastructure (NetworkDamageableHealth, NetworkShipShieldController, NetworkShipBeamWeaponController, TargetableEntity).

## Design Decisions

| Decision | Choice | Rationale |
|----------|--------|-----------|
| AI approach | Simple state machine | Prototype scope; BT adds complexity without benefit for 4 states |
| NPC ownership | Server-owned NetworkObject | No player owns NPCs; server controls all AI logic |
| Combat system | Reuse player components | BR-2 requires same damage/shield/hull rules |
| Movement | ShipMovementModel integration | Reuse existing physics; NPC feeds synthetic input |
| Weapon DPS | Configurable, default 60% of player | BR-3: NPC DPS <= player starter beam |

## Component Architecture

```
NPCShipPrefab (NetworkObject, server-owned)
├── TargetableEntity (Hostile, "Kethari Drone")
├── NetworkDamageableHealth (hull HP)
├── NetworkShipShieldController (directional shields)
├── NetworkShipBeamWeaponController (weak beam, NPC definition)
├── NetworkShipTargetingController (for beam to resolve targets)
├── ShipMovementController + ShipMovementModel (physics)
├── NPCBrain (state machine: Idle → Patrol → Combat → Dead)
└── NPCShipController (feeds movement input from brain decisions)
```

## New Files

| File | Purpose |
|------|---------|
| `Scripts/Combat/AI/NPCBrainState.cs` | State enum (Idle, Patrol, Combat, Dead) |
| `Scripts/Combat/AI/NPCBrain.cs` | Server-side state machine, aggro detection, target selection |
| `Scripts/Combat/AI/NPCShipController.cs` | Translates brain decisions into movement input |
| `Scripts/Combat/AI/NPCSettings.cs` | Static constants (aggro radius, respawn delay, DPS cap) |
| `Scripts/Networking/NPCShipFactory.cs` | Creates NPC prefab at runtime (like TargetDummyFactory) |
| `Scripts/Networking/NPCSpawner.cs` | Spawns N NPCs on server start, handles respawn on death |

## State Machine

```
Idle → (server start) → Patrol
Patrol → (player in aggro_radius) → Combat
Combat → (player leaves 2x aggro_radius OR LOS lost 10s) → Patrol
Combat/Patrol → (hull <= 0) → Dead
Dead → (after respawn_delay) → respawn as new instance
```

## Integration Points

- NPCSpawner hooks into NetworkManager.OnServerStarted (like TargetDummySpawner)
- EmptySectorMultiplayerBootstrap wires up NPCSpawner alongside existing spawners
- NPC uses same BeamWeaponDefinition ScriptableObject system (with lower DPS)
- NPC TargetableEntity is Hostile affiliation (already supported by targeting system)

## Configuration

| Setting | Default | Source |
|---------|---------|--------|
| aggro_radius | 200f (meters) | NPCSettings |
| disengage_radius | 400f (2x aggro) | NPCSettings |
| npc_max_hull | 500f | NPCSettings |
| npc_shield_per_facing | 100f | NPCSettings |
| npc_beam_dps | 150f (60% of player 250) | NPCSettings |
| spawn_count | 3 | NPCSpawner serialized field |
| respawn_delay | 60f seconds | NPCSettings |
| patrol_radius | 100f | NPCSettings |
| patrol_speed_fraction | 0.4f | NPCSettings |
