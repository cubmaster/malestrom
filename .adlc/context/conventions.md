# Iron Exiles — Conventions

## File Organization

```
Malestrom/
├── IronExiles.uproject      # UE5 project entry (engine 5.5)
├── Source/IronExiles/       # Primary C++ runtime module
├── Source/IronExiles.Target.cs
├── Source/IronExilesEditor.Target.cs
├── Config/                  # DefaultEngine.ini, DefaultGame.ini
├── Content/                 # Unreal assets (Maps/Test/EmptySector, etc.)
├── Content/Python/          # Editor utility scripts (content bootstrap)
├── Scripts/                 # Build-Editor.ps1, Run-FoundationTests.ps1, IronExiles.Dev.psm1
├── deploy/                  # Docker/K8s scaffolding (backend services, REQ-042+)
├── docs/                    # Game design documents (GDD, architecture)
│   ├── 01-races.md … 09-realtime-combat.md
└── .adlc/                   # ADLC spec-driven development artifacts
    ├── context/
    ├── specs/
    ├── bugs/
    └── knowledge/
```

## Naming

- **Design docs:** Numbered prefix for ordering (`01-races.md`, `05-architecture.md`)
- **ADLC specs:** `REQ-{number}-{short-slug}.md` in `.adlc/specs/`
- **ADLC bugs:** `BUG-{number}-{short-slug}.md` in `.adlc/bugs/`
- **UE5 modules (planned):** PascalCase module names matching architecture doc (`CombatModule`, `ShipAIModule`, `GalaxyModule`)
- **UE5 C++ types:** `F` prefix for structs, `U` prefix for UObject classes, `E` prefix for enums (UE convention)
- **Database tables:** snake_case plural (`characters`, `ship_loadouts`, `auction_listings`)

## Testing

- **Unit tests:** UE5 Automation Tests for C++ gameplay systems
- **Integration tests:** Dedicated server headless runs for replication and combat validation
- **Load tests:** Monthly sector capacity testing (target: 100 players + 100 NPCs per sector)
- **Performance targets:** 60 fps solo/station, 30+ fps fleet battles, <150ms latency playable

## Error Handling

- **Combat:** Server rejects invalid actions (cooldown violations, out-of-range shots); no client-side damage authority
- **Economy:** Rate limiting and anomaly detection on backend services; server validates all inventory changes
- **Sector transitions:** World server orchestrates handoff; failed transitions roll back to previous sector state
- **API services:** Standard HTTP error codes; idempotent operations where possible

## Deployment

- **Local:** Docker Compose at repo root (or `deploy/docker/`) brings up backend services and dependencies. Developers do not install PostgreSQL/Redis natively for routine work.
- **Production:** Kubernetes manifests (or Helm) under `deploy/k8s/` (or equivalent). CI builds, tags, and pushes images; cluster deploys from those images.
- **Per service:** Each backend microservice includes a `Dockerfile`, health/readiness endpoints, and Compose service entry. K8s Deployment + Service definitions added before a service is considered deployable.
- **Game servers:** UE5 dedicated server builds may run on GameLift/EC2; container packaging is preferred for orchestrator/world-server stubs and backend services.

## Git Conventions

- **Branch naming:** `feat/REQ-{number}-{slug}`, `fix/BUG-{number}-{slug}`, `docs/{topic}`
- **Commit messages:** Imperative mood, reference REQ/BUG when applicable (e.g., `feat(combat): add beam weapon replication REQ-003`)
- **PR process:** ADLC pipeline creates PRs via `/proceed`; require review before merge; destructive deploys require human confirmation
