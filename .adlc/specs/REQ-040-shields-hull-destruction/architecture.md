# REQ-040 Architecture: Directional Shields, Hull HP & Destruction

## Approach

Add a new `NetworkShipShieldController` (NetworkBehaviour) that manages 4 directional shield facings (Front/Rear/Port/Starboard). Modify the beam weapon damage path to route damage through shields first — shields absorb on the facing aligned with the attack vector, overflow spills to hull. When hull reaches 0, the server triggers destruction; clients play VFX and the ship respawns after 5s.

All shield math lives in a static `ShieldMath` utility class (testable without Play Mode). The `NetworkShipShieldController` replicates per-facing HP via a custom `INetworkSerializable` struct (single NetworkVariable, 4 floats — avoids 4 separate NetworkVariables for bandwidth efficiency).

## Key Decisions

### ADR-040-1: Single NetworkVariable for all facings

**Decision:** Pack all 4 shield facing HP values into one `INetworkSerializable` struct replicated as a single `NetworkVariable<ShieldNetworkState>`.

**Rationale:** 4 separate NetworkVariables would each trigger independent dirty-flag checks and bandwidth. A single struct sends one update per tick when any facing changes — matching the existing `ReactorPowerNetworkState` pattern from REQ-038.

### ADR-040-2: Direction determined by dot product on server

**Decision:** The server computes which shield facing absorbs damage using the dot product of the attack vector against the target ship's local axes.

**Rationale:** Simple, deterministic, server-authoritative. No client input needed for facing selection. The beam controller already has attacker and target positions available at damage time.

### ADR-040-3: Destruction as server-side despawn + delayed respawn

**Decision:** On 0 hull, the server despawns the NetworkObject after a short delay (allows clients to see explosion VFX), then respawns a fresh ship at a spawn point after 5s.

**Rationale:** Simplest flow for prototype. No wreckage entity needed. Respawn reuses existing `PlayerShipSpawner` + `SpawnPointManager`.

## Data Model Changes

**New struct:** `ShieldNetworkState` (INetworkSerializable)
- `float Front, Rear, Port, Starboard` — current HP per facing

**New component:** `NetworkShipShieldController` (NetworkBehaviour)
- `NetworkVariable<ShieldNetworkState>` — replicated shield state
- `float _maxShieldPerFacing` — from settings
- `float _regenRatePerSecond` — base rate, scaled by power
- `float[] _lastDamageTime` — per-facing cooldown tracking

**Modified:** `NetworkDamageableHealth`
- Add `event Action<ulong> Destroyed` — fires on server when hull reaches 0
- Add destruction/respawn coroutine

## Damage Flow (Modified)

```
BeamWeaponController.Update() [server]
  → Compute damage tick (existing)
  → Compute attack direction: (attacker.position - target.position).normalized
  → Call target.GetComponent<NetworkShipShieldController>().ApplyDirectionalDamage(direction, damage)
    → ShieldMath.DetermineFacing(localDirection) → facing enum
    → Absorb up to shield HP on that facing
    → Return overflow
  → Call target.GetComponent<NetworkDamageableHealth>().ApplyDamage(overflow)
    → If hull <= 0 → fire Destroyed event → begin despawn/respawn
```

## Shield Regen Flow

```
NetworkShipShieldController.Update() [server only]
  → For each facing where Time.time - lastDamageTime[facing] > RegenCooldownSeconds:
    → regenRate = baseRate * ShieldMath.GetPowerMultiplier(reactorPower.Current.Shields)
    → shield[facing] = min(shield[facing] + regenRate * deltaTime, maxShield)
    → Update NetworkVariable
```

## HUD Integration

- `ShipFlightTelemetryAdapter` gains reference to `NetworkShipShieldController`
- `ShieldPercent` computed as average of all 4 facings (aggregate for existing bar)
- New: expose per-facing values for directional shield diamond display
- `FlightHudPresenter` maps to `FlightHudDisplayState` (add 4 shield fill fields)
- `FlightHudView` renders 4 small bars or a diamond indicator

## Files Affected

| File | Change |
|------|--------|
| `Scripts/Combat/NetworkShipShieldController.cs` | NEW — core shield system |
| `Scripts/Combat/ShieldMath.cs` | NEW — static math (facing detection, regen, absorption) |
| `Scripts/Combat/ShieldSettings.cs` | NEW — constants (max HP, regen rate, cooldown) |
| `Scripts/Combat/DestructionVfx.cs` | NEW — explosion particles on destruction |
| `Scripts/Combat/NetworkDamageableHealth.cs` | MODIFY — destruction event, respawn |
| `Scripts/Combat/NetworkShipBeamWeaponController.cs` | MODIFY — route through shields |
| `Scripts/Combat/ShipFlightTelemetryAdapter.cs` | MODIFY — read shield data |
| `Scripts/Combat/NetworkedShipSetup.cs` | MODIFY — configure shields on spawn |
| `Scripts/Networking/NetworkPlayerShipFactory.cs` | MODIFY — add shield component |
| `Scripts/UI/FlightHudPresenter.cs` | MODIFY — shield fill values |
| `Scripts/UI/FlightHudView.cs` | MODIFY — directional shield bars |
| `Tests/EditMode/ShieldMathTests.cs` | NEW — unit tests |
| `Tests/EditMode/DestructionRespawnTests.cs` | NEW — destruction flow tests |
