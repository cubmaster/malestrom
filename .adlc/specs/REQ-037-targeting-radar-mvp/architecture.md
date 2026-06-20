# REQ-037 Architecture — Targeting, Tab-Lock & Radar MVP

## Approach

Add `TargetableEntity` markers on player ships and NPC dummies. `NetworkShipTargetingController` runs **server-authoritative lock validation** while the **owner client** submits Tab cycle requests via `ServerRpc`. Radar blips are computed on the owner from replicated transforms using shared `TargetSelectionMath` (closest-first, max 20). Lock state replicates via `NetworkVariable<ulong>`.

### Flow

```
Owner Client                         Server
────────────                         ──────
Tab key pressed
ServerRpc(CycleTarget) ───────────► Scan targetables in range
                                    Sort by angle from forward
                                    Update LockedTargetId NV
NetworkVariable ◄────────────────── replicate to all clients
HUD reads lock + local radar scan
```

### Key Decisions

| Decision | Choice | Rationale |
|----------|--------|-----------|
| Lock channel | `ServerRpc` + `NetworkVariable<ulong>` | Small payload; server validates range/LOS |
| Tab sort | Angle from forward, distance tie-break | BR-2 documented in `docs/targeting-radar.md` |
| LOS break | Server raycast; 5s occluded timer | BR-3 configurable |
| Radar display | Owner local scan using same range/max rules | Positions already replicated; BR-4 closest 20 |
| NPC dummy | `TargetDummySpawner` server spawns networked cube | Runnable demo without AI movement |
| Weapons hook | `GetLockedTarget()` on controller | REQ-039 consumption point |

### Components

| Component | Layer | Responsibility |
|-----------|-------|----------------|
| `TargetableEntity` | Combat | Display name, affiliation, hull placeholder |
| `TargetSelectionMath` | Combat | Pure scan/sort/cycle/validation helpers |
| `NetworkShipTargetingController` | Combat | Server lock, LOS timer, replicated lock id |
| `ShipTargetInputController` | Combat | Owner Tab → ServerRpc |
| `TargetDummySpawner` | Networking | Server spawns static NPC dummy |
| `FlightHudView` / presenter | UI | Lock panel + radar blips |

### ADR-037-1 (proposed)

Server-authoritative tab targeting via `NetworkShipTargetingController`; owner radar from replicated entity positions; tab cycle sorts by forward angle.

## Out of Scope

- Weapon firing / damage
- ECM / missile lock
- Faction reputation system
