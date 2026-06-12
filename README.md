# Iron Exiles (Malestrom)

Space MMO prototype based on the Iron Exiles universe. Design docs live in `docs/`; executable code is the Unreal Engine 5 project in this repository.

## Prerequisites

| Tool | Version |
|------|---------|
| Unreal Engine | **5.5.x** (pinned — upgrades require a new REQ) |
| Visual Studio | 2022 with **Desktop development with C++** |
| Git LFS | Required before pulling content assets |

Set the engine path:

```powershell
$env:UE_ROOT = "C:\Program Files\Epic Games\UE_5.5"
```

## Quick start (Runnable Demo)

1. **Clone** and enable LFS:
   ```powershell
   git clone <repo-url> Malestrom
   cd Malestrom
   git lfs install
   git lfs pull
   ```

2. **Generate & open** the project:
   - Right-click `IronExiles.uproject` → *Generate Visual Studio project files*
   - Open `IronExiles.uproject` in Unreal Editor 5.5

3. **Initialize content** (first time only, creates `EmptySector` map):
   ```powershell
   .\Scripts\Initialize-Content.ps1
   ```

4. **Play** — press Play in the editor (`EmptySector` test map, `IronExilesGameModeBase`).

5. **Run foundation tests**:
   ```powershell
   .\Scripts\Run-FoundationTests.ps1
   ```

## Build from command line

```powershell
.\Scripts\Build-Editor.ps1
```

## CI

GitHub Actions workflow `.github/workflows/compile-editor.yml` runs on a **self-hosted Windows runner** with label `ue5` and `UE_ROOT` configured. Use *workflow_dispatch* if no runner is attached yet.

## Project layout

```
IronExiles.uproject
Source/IronExiles/          # C++ game module
Content/Maps/Test/          # EmptySector test sector
Config/                     # DefaultEngine.ini, DefaultGame.ini
Scripts/                    # Build & test automation
docs/                       # Game design documents
.adlc/                      # Spec-driven development artifacts
deploy/                     # Docker/K8s scaffolding (REQ-042+)
```

## ADLC

Incremental delivery is spec-driven. See `.adlc/specs/REQ-031-delivery-roadmap/` for the full REQ sequence. Current milestone: **REQ-032** project foundation.

## License

Proprietary — Iron Exiles.
