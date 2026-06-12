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
2. Open **`Client/`** in Unity Hub.
3. Open scene **`Assets/Scenes/Test/EmptySector.unity`**.
4. Press **Play** — placeholder ship spawns; fly with keyboard + mouse (see `Client/README.md`).

```powershell
# Optional: run Edit Mode tests from repo root
.\Scripts\Run-UnityTests.ps1
```

## Project layout

```
Client/                     # Unity project (Iron Exiles) — REQ-051
legacy/unreal/              # Planned home for deprecated UE tree
IronExiles.uproject         # Legacy UE5 (root until moved)
Source/IronExiles/          # Legacy C++ module
Scripts/                    # Repo automation (UE legacy + future Unity CI)
docs/                       # Game design documents
.adlc/                      # Spec-driven development artifacts
deploy/                     # Docker/K8s scaffolding (backend — REQ-042+)
```

## Zed

Project tasks live in `.zed/tasks.json`. After Unity bootstrap, use **Iron Exiles: Open Unity Project** and related test tasks. Legacy Unreal tasks remain prefixed **Legacy UE5:** for the old scaffold.

## CI

- **Unity:** `.github/workflows/unity-ci.yml` (added with REQ-051) — batchmode build + Edit/Play Mode tests.
- **Legacy UE5:** `.github/workflows/compile-editor.yml` — manual `workflow_dispatch` only; requires self-hosted `ue5` runner.

## ADLC

Incremental delivery is spec-driven. See `.adlc/specs/REQ-031-delivery-roadmap/` for the REQ sequence.

**Current milestone:** REQ-034 chase camera + flight HUD shipped. Next: **REQ-035** dedicated server bootstrap.

## Legacy Unreal (optional)

The UE5 scaffold at repo root was completed under REQ-032/033. It is **superseded** and frozen.

Prerequisites: Unreal **5.5.x**, Visual Studio 2022, `Scripts/ue-root.local.ps1` (copy from `Scripts/ue-root.local.ps1.example`).

```powershell
Copy-Item Scripts/ue-root.local.ps1.example Scripts/ue-root.local.ps1
# Edit UE_ROOT path, then:
.\Scripts\Build-Editor.ps1
.\Scripts\Launch-Editor.ps1
.\Scripts\Initialize-Content.ps1
.\Scripts\Run-FoundationTests.ps1
.\Scripts\Run-FlightTests.ps1
```

## License

Proprietary — Iron Exiles.
