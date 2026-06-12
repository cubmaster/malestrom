# Iron Exiles — Conventions

## File Organization

```
Malestrom/
├── Client/                      # Unity project (IronExiles)
│   ├── Assets/
│   │   ├── _Project/            # Game code, scenes, prefabs, ScriptableObjects
│   │   ├── Scenes/Test/         # EmptySector and test scenes
│   │   └── Settings/            # URP/HDRP, Netcode, Input System
│   ├── Packages/
│   └── ProjectSettings/
├── legacy/unreal/               # Deprecated UE5 scaffold (REQ-032/033), pending removal
├── deploy/                      # Docker/K8s for backend services
├── docs/                        # Game design documents
├── Scripts/                     # Repo automation (backend, CI helpers — not Unity Editor)
└── .adlc/                       # Spec-driven development artifacts
```

Until the Unity project is bootstrapped, the UE5 tree may still live at repo root (`IronExiles.uproject`, `Source/`). New work must not extend it.

## Naming

- **Design docs:** Numbered prefix (`01-races.md`, `05-architecture.md`)
- **ADLC specs:** `REQ-{number}-{short-slug}/` under `.adlc/specs/`
- **Unity assemblies:** PascalCase (`IronExiles.Combat`, `IronExiles.Galaxy`, …)
- **C# types:** PascalCase types; interfaces `I` prefix; private fields `_camelCase`
- **ScriptableObjects:** suffix `Data`, `Config`, or `Definition` (e.g. `ShipStatsDefinition`)
- **Database tables:** snake_case plural (`characters`, `ship_loadouts`, `auction_listings`)

## Testing

- **Unit / component tests:** Unity Test Framework (Edit Mode)
- **Gameplay tests:** Play Mode tests + dedicated-server headless runs for replication/combat
- **Backend tests:** xUnit/NUnit in microservice repos (containerized in CI)
- **Performance targets:** 60 fps solo/station, 30+ fps fleet battles, <150ms latency playable

## Error Handling

- **Combat:** Server rejects invalid actions (cooldown violations, out-of-range shots); no client-side damage authority
- **Economy:** Rate limiting and anomaly detection on backend services; server validates all inventory changes
- **Sector transitions:** World server orchestrates handoff; failed transitions roll back to previous sector state
- **API services:** Standard HTTP error codes; idempotent operations where possible

## Deployment

- **Local:** Docker Compose for backend services and dependencies
- **Production:** Kubernetes manifests under `deploy/k8s/`
- **Game servers:** Unity dedicated server builds (Linux headless) on GameLift/EC2 or K8s Jobs/Deployments
- **Client:** Player installs via launcher/patch CDN — not containerized

## Git Conventions

- **Branch naming:** `feat/REQ-{number}-{slug}`, `fix/BUG-{number}-{slug}`, `docs/{topic}`
- **Commit messages:** Imperative mood, reference REQ/BUG when applicable
- **PR process:** ADLC pipeline via `/proceed`; review before merge

## Zed / IDE

- Unity Editor is the primary game run target once `Client/` exists
- Zed tasks should invoke `Scripts/` helpers or documented `unity` batchmode commands — not legacy Unreal Editor scripts
