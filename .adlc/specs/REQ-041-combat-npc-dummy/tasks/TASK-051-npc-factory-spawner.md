# TASK-051: NPC Ship Factory and Spawner

## Objective
Create the runtime prefab factory (like TargetDummyFactory) and a spawner component that spawns N NPCs on server start and respawns them after death.

## Files to Create
- `Client/Assets/_Project/Scripts/Networking/NPCShipFactory.cs` - Runtime prefab builder
- `Client/Assets/_Project/Scripts/Networking/NPCSpawner.cs` - Spawn/respawn manager

## Design
- NPCShipFactory.CreatePrefab(): builds a GameObject with all required components (NetworkObject, TargetableEntity, NetworkDamageableHealth, NetworkShipShieldController, NetworkShipBeamWeaponController, NetworkShipTargetingController, ShipMovementController, ShipInputController, NPCBrain, NPCShipController)
- NPCSpawner: MonoBehaviour on server, hooks NetworkManager.OnServerStarted, spawns spawn_count NPCs at distributed positions, subscribes to Destroyed event for respawn after delay

## Dependencies
- TASK-048, TASK-049, TASK-050
