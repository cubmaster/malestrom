# REQ-039 Architecture — Beam Weapon & Server-Validated Damage

## Approach

Add the first server-authoritative weapon pipeline: tier-1 beam data (`ScriptableObject`), pure DPS math scaled by REQ-038 weapon power allocation, replicated hull health on damageable entities, and a `NetworkShipBeamWeaponController` that applies sustained hitscan damage each server tick while the owner holds primary fire on a valid lock.

Client beam VFX (LineRenderer stub) plays optimistically from replicated `IsFiring`; hull changes flow only through `NetworkDamageableHealth` → `TargetableEntity` for HUD.

### Flow

```
Owner Client                         Server
────────────                         ──────
Mouse held (primary fire)
SetFiringServerRpc(true) ────────► Validate owner + lock + range
                                    Read weapon power allocation
                                    Each tick: effective_dps × dt
                                    NetworkDamageableHealth.ApplyDamage
NetworkVariable ◄────────────────── hull + IsFiring replicate
HUD reads telemetry (locked target hull, hardpoint firing)
Beam VFX (local LineRenderer) from IsFiring + lock endpoints
```

### Key Decisions

| Decision | Choice | Rationale |
|----------|--------|-----------|
| Weapon authority | Owner `ServerRpc` sets firing flag; server simulates DPS | Matches targeting/movement NGO patterns (ADR-037-1) |
| DPS scaling | `base_dps × weapons_fraction` (linear 0–1) | REQ-038 BR-4; 0% weapons → 0 DPS; measurable at 100% vs 50% |
| Damage target | `NetworkDamageableHealth` on any `TargetableEntity` | REQ-040 replaces stub with shields; clean server API now |
| Lock/range gate | Reuse `NetworkShipTargetingController.LockedTargetNetworkObjectId` + `TargetSelectionMath.IsWithinLockRange` | No duplicate validation logic |
| Beam range | `min(beamDefinition.rangeMeters, lockRangeMeters)` | Cannot fire beyond lock sensor range |
| VFX | Optimistic LineRenderer on firing ship; endpoints from instigator → locked target | BR-3; polish deferred |
| HUD hull | `TargetableEntity.HullPercent` delegates to `NetworkDamageableHealth` when present | Preserves REQ-037 HUD lock panel without UI asmdef cycle |
| Offline | Skip beam controller when not spawned; no offline fire in this REQ | Multiplayer-first Tier C increment |
| Tier-1 baseline | 50 sustained DPS, 2500m range, 1000 max hull prototype | Aligns with `docs/09-realtime-combat.md` tier-1 sustained beam table |

### Components

| Component | Layer | Responsibility |
|-----------|-------|----------------|
| `BeamWeaponDefinition` | Combat (Data) | ScriptableObject: base DPS, range, energy draw |
| `BeamWeaponMath` | Combat | Effective DPS, per-tick damage, fire validation helpers |
| `ReactorPowerAllocationMath` | Combat | `GetWeaponPerformanceMultiplier` (0–1 linear) |
| `NetworkDamageableHealth` | Combat | Server-authoritative hull pool; `NetworkVariable` replication |
| `TargetableEntity` | Combat | Display name/affiliation; hull percent from damageable |
| `NetworkShipBeamWeaponController` | Combat | Server tick damage loop; firing state replication |
| `ShipBeamWeaponInputController` | Combat | Owner primary fire → `SetFiringServerRpc` |
| `BeamWeaponVfx` | Combat | Client LineRenderer beam stub |
| `ShipFlightTelemetryAdapter` | Combat | Own hull + locked target hull + hardpoint firing state |
| `NetworkPlayerShipFactory` / `TargetDummyFactory` | Networking | Register new components on prefabs |

### ADR-039-1 (proposed)

Beam weapons use owner `SetFiringServerRpc` + server-side per-tick DPS application via `NetworkDamageableHealth`; effective DPS scales linearly with weapon power allocation (0–1); hitscan validation uses existing lock id and range helpers from REQ-037.

## Lessons Applied

- **LESSON-005:** Keep Combat/UI acyclic — hull and firing state exposed via `IShipFlightTelemetry` / `ShipFlightTelemetryAdapter` only; no Combat → UI references.
- **LESSON-004:** Edit Mode tests for pure math (DPS, multiplier, validation) before Play Mode multiplayer checks.

## Proposed Context Update

Add to `.adlc/context/architecture.md` ADRs section after merge:

- **ADR-039-1 (approved):** Server-authoritative continuous beam DPS via `NetworkShipBeamWeaponController`; hull on `NetworkDamageableHealth`.

## Out of Scope

- Shields, facings, destruction (REQ-040)
- NPC return fire (REQ-041)
- Lag compensation / hit rewind
- Heat, ammo, multiple hardpoints (single BEAM 1 slot stub only)
