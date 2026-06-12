# REQ-033 — Architecture: 6DOF Ship Flight Prototype

## Approach

Add client-only 6DOF ship flight to the existing `IronExiles` module using a custom `UShipMovementComponent` on a new `AShipPawn`. Movement is Newtonian (momentum retained when thrust releases) with optional brake input, per-tick speed clamping, and soft sector bounds. Stats load from a `UDataTable` row `Human_Starter_Fighter` with sane C++ fallbacks when content is not yet generated.

Input uses legacy axis mappings in `Config/DefaultInput.ini` (keyboard + mouse minimum). Flight assist toggle is **deferred** (open question resolved).

This REQ stays inside the monolithic `IronExiles` runtime module — separate `CombatModule` split is deferred until module count warrants it (per REQ-032 greenfield ADR).

## Component Design

```
AShipPawn
├── UStaticMeshComponent (Engine cube placeholder)
├── UBoxComponent (collision hull)
└── UShipMovementComponent (UPawnMovementComponent)
        ├── Reads FShipStatsRow from DataTable
        ├── Integrates local-space thrust → world velocity
        ├── Applies pitch/yaw/roll rates
        ├── Clamps |velocity| ≤ MaxSpeed
        └── Clamps position to sector AABB

AIronExilesGameModeBase
├── DefaultPawnClass = AShipPawn
└── SectorBoundsExtent (editable, default 500m cube)

Config/DefaultInput.ini
└── Axis mappings: thrust, strafe, vertical, turn, look, roll, brake
```

## Stats Data

| Field | Type | Human_Starter_Fighter default |
|-------|------|-------------------------------|
| MaxSpeed | float | 5000 uu/s |
| ForwardThrust | float | 2500 uu/s² |
| StrafeThrust | float | 1800 uu/s² |
| RotationRate | float | 90 deg/s |
| BrakeDeceleration | float | 1200 uu/s² |

DataTable asset path: `/Game/Data/DT_ShipStats` (created by content Python).

## Testing

| Test | Filter | Validates |
|------|--------|-----------|
| Speed clamp | `IronExiles.Flight.SpeedClamp` | Max thrust → velocity ≤ MaxSpeed |
| Bounds | `IronExiles.Flight.SectorBounds` | Position clamped inside AABB |

CI: extend `Run-FoundationTests.ps1` or add `Run-FlightTests.ps1` filter `IronExiles.Flight`; workflow runs flight tests after foundation tests.

## Key Decisions (ADRs)

### ADR-033-1: Custom UPawnMovementComponent over FloatingPawnMovement

**Decision:** Implement `UShipMovementComponent` subclass with explicit momentum integration.

**Rationale:** `UFloatingPawnMovement` sets velocity directly (no drift). REQ BR-1 requires momentum when thrust is released.

### ADR-033-2: Legacy axis input for prototype

**Decision:** `DefaultInput.ini` + `SetupPlayerInputComponent` bindings.

**Rationale:** Avoids Enhanced Input asset dependency in REQ-033; keyboard+mouse works without extra uasset beyond DataTable. Enhanced Input migration can be a future REQ.

### ADR-033-3: Defer flight assist toggle

**Decision:** No EVE-style flight assist in REQ-033.

**Rationale:** Reduces tuning surface; arcade drift is acceptable per spec assumptions.

### ADR-033-4: Code sector bounds clamp

**Decision:** Clamp pawn location in movement component using configurable `SectorBoundsExtent`.

**Rationale:** Satisfies AC without requiring invisible wall collision meshes; prevents fall-through. Collision hull still uses box component for future interactions.

## Lessons Applied

- **LESSON-001:** Content init script must create DataTable and refresh map spawn; CI runs `Initialize-Content.ps1` before tests.

## Out of Scope (unchanged)

Replication (REQ-036), power allocation (REQ-038), collision damage.
