# REQ-034 — Architecture: Chase Camera & Flight HUD Shell (Unity)

## Approach

Extend REQ-033 flight with **`ChaseCameraRig`** (collision-aware chase camera replacing basic `ShipCameraFollow`) and a new **`IronExiles.UI`** assembly containing presenter/view HUD code. HUD uses runtime-built **uGUI** canvas. Hull remains static 100% until REQ-040.

## Component Design

```
EmptySectorFlightSetup
├── spawns PlayerShip (REQ-033)
├── ChaseCameraRig on Main Camera
└── FlightHudController (creates Canvas + binds telemetry)

PlayerShip
├── ShipMovementController
└── ShipFlightTelemetryAdapter

IronExiles.UI
├── IShipFlightTelemetry
├── FlightHudPresenter
├── FlightHudView
└── FlightHudController
```

## Key Decisions (ADRs)

### ADR-034-1: Presenter/view split with uGUI prototype

### ADR-034-2: Runtime HUD canvas

### ADR-034-3: SphereCast camera collision via ChaseCameraPlacement helper

### ADR-034-4: m/s speed display

See full ADR text in repo architecture.md at merge.
