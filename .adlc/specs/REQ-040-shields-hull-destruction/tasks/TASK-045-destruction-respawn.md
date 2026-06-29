---
id: TASK-045
title: "Destruction event, VFX, and respawn flow"
status: draft
parent: REQ-040
created: 2026-06-28
updated: 2026-06-28
dependencies: []
---

## Description

Add ship destruction when hull reaches 0: server fires a destruction event, clients play explosion VFX, the server despawns the NetworkObject after a brief delay, then respawns a fresh ship at a spawn point after 5 seconds.

## Files to Create/Modify

- `Client/Assets/_Project/Scripts/Combat/NetworkDamageableHealth.cs` — add `event Action<ulong> Destroyed`, fire it when hull transitions to 0 on the server; add `ServerStartRespawnCoroutine()` that waits 5s then calls `PlayerShipSpawner` to respawn
- `Client/Assets/_Project/Scripts/Combat/DestructionVfx.cs` — NEW MonoBehaviour: listens to `NetworkDamageableHealth` hull changes, spawns particle burst when `IsDestroyed` becomes true, auto-destroys after particles finish
- `Client/Assets/_Project/Scripts/Networking/PlayerShipSpawner.cs` — add `RespawnPlayer(ulong clientId)` method that despawns old ship and spawns fresh one at next spawn point
- `Client/Assets/_Project/Scripts/Networking/NetworkPlayerShipFactory.cs` — add `DestructionVfx` component to ship prefab
- `Client/Assets/_Project/Tests/EditMode/DestructionRespawnTests.cs` — test DamageableHealthMath destruction threshold detection, test respawn timing logic

## Acceptance Criteria

- [ ] `NetworkDamageableHealth.Destroyed` event fires exactly once when hull transitions from >0 to <=0
- [ ] Event does NOT fire if damage is applied to already-destroyed ship
- [ ] `DestructionVfx` spawns a visible particle burst on destruction (orange/red expanding sphere)
- [ ] Server despawns destroyed ship after 0.5s (time for clients to see explosion start)
- [ ] Server respawns fresh ship at next spawn point after 5s total from destruction
- [ ] Respawned ship has full hull and full shields
- [ ] Edit Mode tests verify destruction threshold logic

## Technical Notes

- Pattern: `_currentHull.OnValueChanged` callback on clients triggers VFX check
- Server side: coroutine in `NetworkDamageableHealth` or delegate to `PlayerShipSpawner` via event subscription
- `DestructionVfx` uses Unity ParticleSystem (simple sphere burst, no external assets needed for prototype)
- Respawn reuses `SpawnPointManager.GetNextSpawnPosition()` — no new infrastructure
- The 0.5s pre-despawn lets clients see the explosion before the object vanishes
- Prevent double-destruction: guard with `_hasTriggeredDestruction` bool
