# REQ-038 Architecture — Reactor Power Allocation

## Approach

Add server-authoritative W/S/E/ECM power split with HUD sliders and preset buttons. Engine allocation scales thrust and max speed in `ShipMovementModel` via a documented linear multiplier (50% at 0% engines, 100% at 100% engines). Weapon and shield slots replicate for HUD display; combat effects land in REQ-039/040. ECM maps to spec "AI" slot (reserved, no gameplay effect).

### Flow

```
Owner HUD sliders/presets
        │
        ▼
RequestAllocation ──► ServerRpc (network) or local apply (offline)
        │
        ▼
Server validates sum == 100% ──► NetworkVariable replicate
        │
        ▼
ShipMovementModel.SetEnginePerformanceMultiplier
        │
        ▼
HUD bars read Current allocation from controller
```

### Key Decisions

| Decision | Choice | Rationale |
|----------|--------|-----------|
| Authority | `NetworkVariable` + owner `ServerRpc` | Matches targeting/movement NGO patterns |
| Normalized values | 0–1 fractions summing to 1 | HUD already uses fillAmount 0–1 |
| Engine effect | `lerp(0.5, 1.0, engines)` on thrust + max speed | Measurable 2:1 ratio for acceptance test |
| ECM slot | UI label ECM; spec AI reserved | BR-5; HUD already shows ECM |
| Offline path | `ShipReactorPowerController` | EmptySector training without Netcode |
| Presets | Combat / Travel / Balanced buttons | Resolves open question in spec |

### Components

| Component | Layer | Responsibility |
|-----------|-------|----------------|
| `ReactorPowerAllocationMath` | Combat | Validation, presets, slider redistribution, engine multiplier |
| `IShipReactorPowerControl` | Combat | Read/request allocation |
| `ShipReactorPowerController` | Combat | Offline/local authority + movement hook |
| `NetworkShipReactorPowerController` | Combat | Server validation, replication, movement hook |
| `ShipMovementModel` | Combat | Engine performance multiplier on thrust/clamp |
| `FlightHudView` / `FlightHudController` | UI | Sliders, presets, live bars |
| `ShipFlightTelemetryAdapter` | Combat | Telemetry reads controller allocation |

### ADR-038-1 (proposed)

Reactor power uses NGO `NetworkVariable` with owner `ServerRpc`; engine allocation applies a 0.5–1.0 performance multiplier in `ShipMovementModel`; ECM slot reserved for REQ-046.

## Out of Scope

- Weapon DPS cap (REQ-039)
- Shield regen (REQ-040)
- Reactor damage reducing total output
