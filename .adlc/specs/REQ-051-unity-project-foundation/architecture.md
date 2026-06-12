# REQ-051 — Unity Project Foundation

## Overview

Greenfield Unity 6 LTS project at `Client/` replacing the superseded UE5 scaffold. Delivers pinned editor version **6000.0.32f1**, URP defaults, `_Project` assembly layout, `EmptySector` test scene, Edit Mode tests, Zed tasks, and CI batchmode smoke build.

## Layout

```
Client/
├── Assets/
│   ├── _Project/
│   │   ├── Scripts/
│   │   │   └── Core/           # IronExiles.Core.asmdef
│   │   └── Tests/
│   │       └── EditMode/       # IronExiles.Core.Tests.asmdef
│   ├── Scenes/Test/
│   │   └── EmptySector.unity
│   └── Settings/               # URP asset, Input System
├── Packages/manifest.json      # URP, Test Framework, Input System
└── ProjectSettings/
```

## CI

`.github/workflows/unity-ci.yml` triggers on `Client/**` changes. Uses batchmode:

- `-runTests -testPlatform editmode`
- Fail build on compile errors

## Legacy

Do not delete root UE tree in this REQ. Document in `legacy/unreal/README.md`; move in cleanup after REQ-033 Unity flight lands.

## ADRs

- **ADR-034** — Unity as primary engine (approved)
