# Iron Exiles (Malestrom)

Space MMO prototype based on the Iron Exiles universe. Design docs live in `docs/`; the **active game client** is a **Unity** project (see ADR-034 in `.adlc/context/architecture.md`).

## Engine status

| Stack | Status |
|-------|--------|
| **Unity 6 LTS** (`Client/`) | **Active** — foundation (REQ-051) + 6DOF flight (REQ-033) + HUD/camera (REQ-034) |
| **Unreal Engine 5** (repo root) | **Legacy** — REQ-032/033 reference; do not extend |

See `.adlc/knowledge/lessons/LESSON-002-engine-pivot-unity.md` for the pivot rationale.

## Prerequisites (Unity)

| Tool | Version |
|------|---------|
| Unity Hub | Latest |
| Unity Editor | **6000.0.32f1** (Unity 6 LTS — pinned in `Client/ProjectSettings/ProjectVersion.txt`) |
| .NET SDK | Per Unity version requirements |

Optional: `Scripts/unity-root.local.ps1` (copy from example) to point batchmode scripts at your Hub editor install.

## Quick start (Unity)

1. **Clone** the repository.
2. Run `.\Scripts\Launch-UnityEditor.ps1` (or open **`Client/`** in Unity Hub).
3. Open scene **`Assets/Scenes/Test/EmptySector.unity`**.
4. Press **Play** — ship spawns with chase camera and flight HUD (see `Client/README.md`).

```powershell
# Open Unity Editor with Client/
.\Scripts\Launch-UnityEditor.ps1

# Open EmptySector for play-testing (press Play in the Editor)
.\Scripts\Run-UnityGame.ps1

# Run Edit Mode tests (batchmode — close Unity Editor first)
.\Scripts\Run-UnityTests.ps1
```

## Project layout

```
Client/                     # Unity project (Iron Exiles) — REQ-051
legacy/unreal/              # Planned home for deprecated UE tree
IronExiles.uproject         # Legacy UE5 (root until moved)
Source/IronExiles/          # Legacy C++ module
Scripts/                    # Repo automation (Unity dev scripts; UE legacy under Scripts/legacy/)
docs/                       # Game design documents
.adlc/                      # Spec-driven development artifacts
deploy/                     # Docker/K8s scaffolding (backend — REQ-042+)
```

## Zed

Project tasks live in `.zed/tasks.json`. In VS Code / Cursor, use **Run and Debug** (`.vscode/launch.json`) or **Tasks: Run Task** (`.vscode/tasks.json`).

| Task | Script |
|------|--------|
| **Local Multiplayer Dev (Server + Unity)** | `Scripts/Launch-LocalMultiplayerDev.ps1` |
| Start Dedicated Server | `Scripts/Run-UnityDedicatedServer.ps1` |
| Open Unity Editor | `Scripts/Launch-UnityEditor.ps1` |
| Play EmptySector | `Scripts/Run-UnityGame.ps1` |
| Edit Mode tests | `Scripts/Run-UnityTests.ps1` |

In Zed: **task: spawn** (or `Alt+Shift+T`) → **Iron Exiles: Local Multiplayer Dev (Server + Unity)**.

That starts the dedicated server in one terminal and opens Unity with EmptySector. Press **Play**, then **Connect To Server** on `NetworkSessionManager` (or enable **Auto Connect In Editor** on `EmptySectorMultiplayerBootstrap` first).

```powershell
# Same flow from a shell (builds server automatically on first run)
.\Scripts\Launch-LocalMultiplayerDev.ps1
```

Legacy Unreal tasks are prefixed **Legacy UE5:** and live under `Scripts/legacy/`.

## CI

- **Unity:** `.github/workflows/unity-ci.yml` (added with REQ-051) — batchmode build + Edit/Play Mode tests.
- **Legacy UE5:** `.github/workflows/compile-editor.yml` — manual `workflow_dispatch` only; requires self-hosted `ue5` runner.

## ADLC

Incremental delivery is spec-driven. See `.adlc/specs/REQ-031-delivery-roadmap/` for the REQ sequence.

**Current milestone:** REQ-034 chase camera + flight HUD shipped. Next: **REQ-035** dedicated server bootstrap.

## Legacy Unreal (optional)

The UE5 scaffold at repo root was completed under REQ-032/033. It is **superseded** and frozen. Scripts are under `Scripts/legacy/`.

Prerequisites: Unreal **5.5.x**, Visual Studio 2022, `Scripts/legacy/ue-root.local.ps1` (copy from `Scripts/legacy/ue-root.local.ps1.example`).

```powershell
Copy-Item Scripts/legacy/ue-root.local.ps1.example Scripts/legacy/ue-root.local.ps1
# Edit UE_ROOT path, then:
.\Scripts\legacy\Build-Editor.ps1
.\Scripts\legacy\Launch-UEEditor.ps1
.\Scripts\legacy\Initialize-Content.ps1
.\Scripts\legacy\Run-FoundationTests.ps1
.\Scripts\legacy\Run-FlightTests.ps1
```

## License

Proprietary — Iron Exiles.
