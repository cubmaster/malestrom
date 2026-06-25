# Beam Weapon & Server-Validated Damage (REQ-039)

Tier-1 continuous energy beam with server-authoritative hull damage.

## Controls

| Input | Action |
|-------|--------|
| **Tab** | Cycle lock target (REQ-037) |
| **Left mouse (hold)** | Primary fire — sustained beam on locked target |

## Server Authority

- Owner sends `SetFiringServerRpc(true/false)` while holding/releasing fire.
- Dedicated server validates lock, range, and weapon power allocation each tick.
- Damage applies as `effective_dps × deltaTime` to the target's `NetworkDamageableHealth`.
- Effective DPS = `50 × weapons_fraction` (tier-1 baseline, 100% weapons = 50 DPS).

## Prototype Stats

| Stat | Value |
|------|-------|
| Base DPS | 50 |
| Range | 2500 m (matches targeting lock range) |
| Max hull | 1000 |
| TTK (100% weapons) | ~20 s sustained fire |

## Local Multiplayer Demo

1. Start dedicated server + two clients (see `docs/local-multiplayer-test.md`).
2. Tab-lock the training dummy or the other player's ship.
3. Hold **left mouse** — beam LineRenderer appears; locked target hull bar drops.
4. Observers see beam VFX and hull replication on the target.
5. Max **Weapons** power preset vs **Travel** preset — observe DPS difference (~5× at 100% vs 10% weapons).

## Components

- `NetworkShipBeamWeaponController` — server damage loop
- `NetworkDamageableHealth` — replicated hull pool
- `BeamWeaponMath` / `BeamWeaponControllerLogic` — pure math + tests
