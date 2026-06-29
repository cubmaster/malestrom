---
id: TASK-047
title: "HUD directional shield display and telemetry wiring"
status: draft
parent: REQ-040
created: 2026-06-28
updated: 2026-06-28
dependencies: [TASK-044]
---

## Description

Wire the directional shield data into the flight telemetry adapter and render per-facing shield bars in the HUD. Replace the hardcoded `ShieldPercent = 100f` with real values from `NetworkShipShieldController`.

## Files to Create/Modify

- `Client/Assets/_Project/Scripts/Combat/ShipFlightTelemetryAdapter.cs` — add reference to `NetworkShipShieldController`, replace `ShieldPercent => 100f` with aggregate calculation, add `GetShieldFacingPercent(int facingIndex)` method exposing per-facing values
- `Client/Assets/_Project/Scripts/Combat/ShipFlightTelemetryAdapter.cs` — extend `IShipFlightTelemetry` interface with `float GetShieldFacingPercent(int facingIndex)` and `int ShieldFacingCount`
- `Client/Assets/_Project/Scripts/UI/FlightHudPresenter.cs` — add `float[] ShieldFacings` (4 values 0-1) to `FlightHudDisplayState`, populate from telemetry
- `Client/Assets/_Project/Scripts/UI/FlightHudView.cs` — add 4 directional shield bar UI elements (small bars arranged as diamond: top=front, bottom=rear, left=port, right=starboard) near the existing shield bar; update `Apply()` to set fill amounts
- `Client/Assets/_Project/Tests/EditMode/FlightHudPresenterTests.cs` — add test cases for directional shield fill calculation, verify 0% when fully depleted, 100% when full

## Acceptance Criteria

- [ ] `ShieldPercent` returns weighted average of all 4 facings (replaces hardcoded 100f)
- [ ] Per-facing shield bars visible in HUD, positioned as directional diamond
- [ ] Bars animate down as shields take damage, animate up during regen
- [ ] When a facing is fully depleted, its bar shows 0% (red/empty)
- [ ] Full shields show 100% (blue/cyan filled)
- [ ] HUD gracefully handles missing `NetworkShipShieldController` (falls back to 100%)
- [ ] Presenter tests pass with stub telemetry returning various shield states

## Technical Notes

- Follow existing HUD pattern: `FlightHudView.Create()` builds UI programmatically
- Shield bars: thin rectangles, cyan fill color, positioned relative to ship status panel
- Layout suggestion: 4 bars in a cross/diamond pattern (front=top, rear=bottom, port=left, starboard=right)
- `ShipFlightTelemetryAdapter.Awake()` already resolves components — add shield controller there
- Fallback: `_shieldController != null ? compute : 100f` to not break ships without shields
- Color coding: cyan (>50%), yellow (20-50%), red (<20%) — optional visual enhancement
