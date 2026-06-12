# REQ-033 — Architecture: 6DOF Ship Flight Prototype (Unity)

## Approach

Add client-only 6DOF ship flight to `IronExiles.Combat` using a testable `ShipMovementModel` (pure C# integrator) and a thin `ShipMovementController` MonoBehaviour. Movement is Newtonian (momentum retained when thrust releases) with optional brake input, per-tick speed clamping, and soft sector bounds. Stats load from `ShipStatsDefinition` ScriptableObject with C# fallbacks matching legacy UE `Human_Starter_Fighter` (converted to meters).

Input uses the **Input System package** via `Keyboard`/`Mouse` device polling (no Input Action assets required for prototype). Flight assist toggle is **deferred**.

## Component Design

```
EmptySectorFlightSetup (scene)
└── spawns PlayerShip (cube primitive)
    ├── ShipMovementController → ShipMovementModel
    ├── ShipInputController (keyboard + mouse)
    └── BoxCollider hull

Main Camera
└── ShipCameraFollow (chase offset)

ShipStatsDefinition (ScriptableObject, optional)
└── Human_Starter_Fighter defaults in meters
```

## Stats Data (meters / m/s²)

| Field | Human_Starter_Fighter default | UE reference (cm) |
|-------|------------------------------|-------------------|
| MaxSpeed | 50 m/s | 5000 uu/s |
| ForwardThrust | 25 m/s² | 2500 uu/s² |
| StrafeThrust | 18 m/s² | 1800 uu/s² |
| RotationRate | 90 deg/s | 90 deg/s |
| BrakeDeceleration | 12 m/s² | 1200 uu/s² |

Sector bounds default: 5000 m cube (was 500000 uu in UE).

## Controls

| Input | Action |
|-------|--------|
| W / S | Forward / reverse thrust |
| A / D | Strafe left / right |
| Space / Ctrl | Strafe up / down |
| Mouse | Pitch / yaw |
| Q / E | Roll |
| Left Shift | Brake |

## Testing

| Test | Location | Validates |
|------|----------|-----------|
| SpeedClamp | `ShipMovementModelTests` | Max thrust → velocity ≤ MaxSpeed |
| SectorBounds | `ShipMovementModelTests` | Position clamped inside AABB |
| Momentum | `ShipMovementModelTests` | Velocity retained when thrust released |

CI: `Run-UnityTests.ps1` (Edit Mode) includes flight model tests.

## Key Decisions (ADRs)

### ADR-033-1: Pure movement model + thin MonoBehaviour

**Decision:** Integrate physics in `ShipMovementModel`; controller applies to `Transform`.

**Rationale:** Edit Mode tests without Play Mode; mirrors UE `UShipMovementComponent` logic.

### ADR-033-2: Input System device polling for prototype

**Decision:** `ShipInputController` reads `Keyboard.current` / `Mouse.current`.

**Rationale:** Project uses Input System (`activeInputHandler: 1`); avoids `.inputactions` asset dependency in REQ-033.

### ADR-033-3: Defer flight assist toggle

**Decision:** No EVE-style flight assist in REQ-033.

**Rationale:** Reduces tuning surface; arcade drift acceptable per spec.

### ADR-033-4: Code sector bounds clamp

**Decision:** Clamp ship position in movement model using configurable AABB extent.

**Rationale:** Satisfies AC without invisible wall meshes; prevents escape from test sector.

## Lessons Applied

- **LESSON-003:** Hand-author Unity YAML/asmdef when Editor unavailable; verify with Edit Mode tests.

## Out of Scope (unchanged)

Replication (REQ-036), power allocation (REQ-038), collision damage.
