# Legacy Unreal Engine 5 scripts (REQ-032/033)

These scripts target the **deprecated** UE5 scaffold at the repo root (`IronExiles.uproject`). Do not use them for new work — use the Unity scripts in `Scripts/` instead.

| Script | Purpose |
|--------|---------|
| `Launch-UEEditor.ps1` | Open `IronExiles.uproject` in Unreal Editor |
| `Run-UEGame.ps1` | Launch standalone game (`EmptySector` map) |
| `Build-Editor.ps1` | Build `IronExilesEditor` Win64 Development |
| `Build-And-Launch-UEEditor.ps1` | Build, then open editor |
| `Run-FoundationTests.ps1` | UE Automation tests (`IronExiles.Foundation`) |
| `Run-FlightTests.ps1` | UE Automation tests (`IronExiles.Flight`) |
| `Initialize-Content.ps1` | Generate EmptySector map via Editor Python |

Setup: copy `ue-root.local.ps1.example` to `ue-root.local.ps1` and set `$env:UE_ROOT`, or install UE 5.5 via Epic Games Launcher.
