# REQ-032 — Architecture: UE5 Project Foundation

## Approach

Bootstrap a **greenfield** Unreal Engine 5 C++ project named **IronExiles** at the repository root. The increment delivers a compilable editor target, an empty test sector map runnable in PIE, one Automation Test suite (`IronExiles.Foundation`), and a CI/local build script path. No gameplay systems, networking plugins, or backend containers in this REQ — those arrive in REQ-033+ and REQ-042 per the roadmap.

This follows `.adlc/ETHOS.md` §7 in spirit: backend container scaffolding is deferred, but the repo layout reserves `deploy/` for later Docker/K8s work without mixing UE binary assets into container images.

## Repository Layout

```
Malestrom/
├── IronExiles.uproject
├── README.md
├── .gitattributes              # Git LFS rules for uasset/umap (future-proof)
├── Config/
│   ├── DefaultEngine.ini
│   └── DefaultGame.ini
├── Content/
│   └── Maps/Test/EmptySector.umap
├── Source/
│   ├── IronExiles/
│   │   ├── IronExiles.Build.cs
│   │   ├── Public/Private module files
│   │   ├── IronExilesGameModeBase.*
│   │   └── Tests/IronExilesFoundationTest.cpp
│   ├── IronExiles.Target.cs
│   └── IronExilesEditor.Target.cs
├── Scripts/
│   ├── Build-Editor.ps1        # Local + CI entry point
│   └── Run-FoundationTests.ps1
├── .github/workflows/
│   └── compile-editor.yml
└── deploy/
    └── README.md               # Placeholder: Docker/K8s land in REQ-042+
```

## Module Design

| Module | Type | Responsibility |
|--------|------|----------------|
| `IronExiles` | Runtime | Game module; `AGameModeBase` subclass, future pawn/system code |
| (Editor target only) | — | No separate editor module in REQ-032; add `IronExilesEditor` later if needed |

**Build.cs dependencies (initial):** `Core`, `CoreUObject`, `Engine`, `InputCore`, `AutomationTest` (for tests in game module — standard UE pattern for project tests).

## Configuration

- **Default map:** `/Game/Maps/Test/EmptySector`
- **GameMode:** `AIronExilesGameModeBase` — minimal spawn defaults, no custom pawns yet
- **Engine association:** Pinned in `.uproject` / README as **UE 5.5** (adjust only via new REQ per BR-1)

## CI / Build Pipeline

UE5 cannot compile on vanilla GitHub-hosted runners without a preinstalled engine. Architecture uses:

1. **`Scripts/Build-Editor.ps1`** — reads `UE_ROOT` env var, runs `Build.bat IronExilesEditor Win64 Development`, exits non-zero on failure.
2. **`.github/workflows/compile-editor.yml`** — runs on **`workflow_dispatch`** and **`push` to `main`** using a **`self-hosted` Windows runner** label `ue5` (documented setup). If no self-hosted runner exists yet, the workflow still validates script presence; developers run the script locally for the Runnable Demo.

Automation tests run via:

```powershell
& "$UE_ROOT\Engine\Binaries\Win64\UnrealEditor-Cmd.exe" `
  "$Project/IronExiles.uproject" -ExecCmds="Automation RunTests IronExiles.Foundation; Quit" -unattended
```

(wrapped in `Scripts/Run-FoundationTests.ps1`)

## Key Decisions (ADRs)

### ADR-032-1: Greenfield project over Lyra fork

**Decision:** Create a blank C++ UE5 project; do not fork Lyra or Advanced Sessions.

**Rationale:** Lyra bundles opinionated GAS, UI, and sample multiplayer patterns that conflict with the custom sector-based MMO architecture in `docs/05-architecture.md`. Greenfield keeps module boundaries under our control; we add OnlineSubsystem and replication in REQ-035–036 deliberately.

### ADR-032-2: Pin Unreal Engine 5.5

**Decision:** Document and target UE **5.5.x** for editor and Development builds.

**Rationale:** Stable modern UE5 feature set (Nanite/Lumen baseline for later art reqs). Pin exact patch in README when first dev machine confirms install path. Upgrades require a new REQ (BR-1).

### ADR-032-3: Automation tests in game module

**Decision:** Place `IronExiles.Foundation.*` tests under `Source/IronExiles/Tests/` using UE Automation Framework.

**Rationale:** Avoids editor-module overhead for REQ-032; tests validate map + GameMode load in packaged test runner. Matches acceptance criterion `IronExiles.Foundation.ProjectLoads`.

### ADR-032-4: Self-hosted CI runner for compile

**Decision:** GitHub Actions workflow targets self-hosted Windows runner with `UE_ROOT` preconfigured; local `Build-Editor.ps1` is the source of truth.

**Rationale:** Epic does not provide free cloud UE builds. Script-first ensures Runnable Demo works without CI; CI mirrors local script when runner available.

## Data Model / API Changes

None — no backend in this REQ.

## Proposed Updates to `.adlc/context/architecture.md`

After implementation, add ADR-032-1/032-2 references under ADRs and replace `(future) Source/` in conventions with actual layout (handled in TASK-005).

## Lessons Applied

No prior lessons in `.adlc/knowledge/lessons/` — greenfield bootstrap.
