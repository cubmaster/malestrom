# Iron Exiles — Unity Client

Open this folder in **Unity Hub** (Unity **6000.0.32f1** / Unity 6 LTS).

## Quick start

1. Unity Hub → **Open** → select this `Client/` folder.
2. Open scene `Assets/Scenes/Test/EmptySector.unity`.
3. Press **Play** — a placeholder ship spawns with chase camera and flight HUD.

## Flight controls (REQ-033)

| Input | Action |
|-------|--------|
| W / S | Forward / reverse thrust |
| A / D | Strafe left / right |
| Space / Ctrl | Strafe up / down |
| Mouse | Pitch / yaw |
| Q / E | Roll |
| Left Shift | Brake (hold) |

Movement is Newtonian: releasing thrust keeps momentum until you brake or hit sector bounds.

## Flight HUD & camera (REQ-034)

- **HUD** (lower-left): speed in m/s, heading in degrees, hull bar (placeholder 100% until REQ-040).
- **Chase camera**: smooth follow with sphere-cast collision pull-in so the camera does not clip through geometry.

## Tests

From repo root:

```powershell
.\Scripts\Run-UnityTests.ps1
```

Or in Editor: **Window → General → Test Runner → Edit Mode** (`ShipMovementModelTests`, `FlightHudPresenterTests`, `ChaseCameraPlacementTests`).

## Layout

See `.adlc/context/conventions.md` and `.adlc/specs/REQ-051-unity-project-foundation/architecture.md`.
