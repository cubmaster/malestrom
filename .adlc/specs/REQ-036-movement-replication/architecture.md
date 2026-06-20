# REQ-036 Architecture — Server-Authoritative Movement Replication

## Approach

Extend the REQ-033 `ShipMovementModel` with a `NetworkShipMovementController` (`NetworkBehaviour`) that runs **server-authoritative simulation** while the **owning client predicts** locally and **reconciles** when server state diverges. Remote clients rely on NGO `NetworkTransform` interpolation only.

### Simulation Flow

```
Owner Client                          Server                         Remote Client
────────────                          ──────                         ─────────────
Read keyboard/mouse
Predict ShipMovementModel (Update)
Apply transform locally
ServerRpc(SubmitInput) ───────────► Buffer input
                                    Simulate model @ 30Hz (FixedUpdate)
                                    Apply authoritative transform
NetworkTransform ◄────────────────── replicate ────────────────► interpolate visual
Reconcile if error > threshold
```

### Key Decisions

| Decision | Choice | Rationale |
|----------|--------|-----------|
| Input channel | `ServerRpc` with `ShipMovementInput` struct | Small payload; owner-only; server validates implicitly by simulating |
| Server tick | 30 Hz `FixedUpdate` on dedicated server + host | Matches combat prototype assumption in spec |
| Prediction | Owner runs same `ShipMovementModel` in `Update` | Reuses REQ-033 math; responsive LAN feel |
| Reconciliation | Position > 2m or rotation > 15° → blend model toward server transform | Prevents permanent desync without harsh snaps every frame |
| Remote ships | Disable local movement/input; `NetworkTransform` interpolate | BR-3; no ghost simulation on observers |
| Tick / relevancy docs | `docs/movement-replication.md` | BR-4 documentation for prototype (no culling yet) |

### Components

| Component | Layer | Responsibility |
|-----------|-------|----------------|
| `ShipMovementInput` | Combat | `INetworkSerializable` input payload |
| `NetworkShipMovementController` | Combat | Server sim, owner predict, reconcile, ServerRpc |
| `ShipMovementController` | Combat | Refactored: exposes model tick helpers; disables Update when networked |
| `NetworkedShipSetup` | Combat | Enables movement network driver instead of local-only controller |

### Proposed Addition to `.adlc/context/architecture.md`

> **ADR-036-1:** Ship movement replication uses server-side `ShipMovementModel` simulation with owner client prediction/reconciliation via `NetworkShipMovementController`. Observers never simulate movement locally.

## Out of Scope

- Weapon lag compensation
- Network relevancy culling implementation
- Anti-cheat velocity caps beyond model clamps
