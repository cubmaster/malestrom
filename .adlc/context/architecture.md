# Iron Exiles — Architecture

## System Diagram

```
┌─────────────────────────────────────────────────────────────────────────┐
│                         CLIENT (Unity)                                   │
├─────────────────────────────────────────────────────────────────────────┤
│  Combat │ Galaxy/Travel │ Economy/Craft │ Social/Chat                   │
│         └───────────────┴───────────────┴─────────────────────────────│
│         Client Core (Ship AI, HUD/UI, Physics, Net Prediction)         │
│                                    │                                     │
│              [Netcode for GameObjects / custom transport]                │
└────────────────────────────────────┼─────────────────────────────────────┘
                                     │ Network
┌────────────────────────────────────┼─────────────────────────────────────┐
│                         SERVER INFRASTRUCTURE                              │
├───────────────────────────────────────────────────────────────────────────┤
│  Game Servers (Unity Dedicated) │ World Server (Orchestrator) │ Backend   │
│  - Sector host (50-100 players) │ - Sector mgmt, routing      │ Micro-    │
│  - Combat/NPC/loot authority    │ - Load balance, instances   │ services  │
│                                 │ - World events              │ (K8s)     │
│  ┌─────────────────────────────┴───────────────────────────────┐          │
│  │  PostgreSQL (persist) │ Redis (cache) │ InfluxDB (metrics) │          │
│  └─────────────────────────────────────────────────────────────┘          │
└───────────────────────────────────────────────────────────────────────────┘
```

## Layers

| Layer | Responsibility |
|-------|----------------|
| **Client (Unity)** | Rendering (URP/HDRP), input, local prediction, UI (uGUI/UI Toolkit), audio, local ship AI companion |
| **Dedicated Game Servers** | Headless Unity builds; sector simulation, server-authoritative combat, NPC AI, loot, replication |
| **World Orchestrator** | Sector spin-up/down, player routing, load balancing, instanced content, world events |
| **Backend Microservices** | Auth, accounts, economy, chat, guilds, missions, matchmaking, analytics |
| **Data Layer** | PostgreSQL (persistent), Redis (session/cache), InfluxDB (telemetry) |

## Key Patterns

- **Sector instancing** — Open-world sectors run as dedicated Unity server instances; private missions/raids use pooled instances
- **Server-authoritative combat** — Client-initiated actions validated server-side; damage/HP replicated from server
- **Client prediction** — Ship movement predicted locally, reconciled with server state
- **Microservices backend** — Stateless services for non-realtime logic; game servers handle realtime simulation
- **Container-first backend** — Docker Compose locally, Kubernetes in production; every backend service is containerized (see `.adlc/ETHOS.md` §7)
- **Triple-XP progression** — Combat, trade, and explore XP tracked independently with combined level cap
- **Data-driven content** — ScriptableObjects / JSON + Addressables for hulls, weapons, AI abilities; PostgreSQL for player/inventory state
- **Assembly-based client architecture** — Combat, Ship AI, Progression, Economy, Galaxy, Social, Mission assemblies under `Client/`

## ADRs

- **ADR-001 (superseded):** ~~Unreal Engine 5~~ — replaced by ADR-034
- **ADR-002 (proposed):** Sector-based MMO architecture over single-shard — manageable server load, horizontal scaling
- **ADR-003 (proposed):** PostgreSQL + Redis data split — persistent relational data vs. real-time session/cache
- **ADR-032-1 … ADR-032-4 (superseded):** UE5 foundation decisions — historical; see `legacy/unreal/` and LESSON-002
- **ADR-035-1 (approved):** `IronExiles.Networking` owns session lifecycle and spawn; `IronExiles.Combat` references `Unity.Netcode.Runtime` for network-aware ship components. Runtime `EmptySectorMultiplayerBootstrap` wires NetworkManager/spawner without hand-editing scene YAML.
- **ADR-036-1 (approved):** Server-side `ShipMovementModel` simulation with owner client prediction/reconciliation via `NetworkShipMovementController`. Observers never simulate movement locally; `NetworkTransform` interpolates remote ships.
- **ADR-037-1 (approved):** Server-authoritative tab targeting via `NetworkShipTargetingController`; owner radar from replicated transforms; tab cycle sorts by forward angle then distance.
- **ADR-034 (approved):** **Unity** as primary game engine — see below

### ADR-034: Unity as primary game engine

**Decision:** Use **Unity 6 LTS** (or current Unity LTS at project bootstrap) for client and dedicated game servers. Core gameplay in **C#**; hot paths may use Burst/Jobs where profiling demands it.

**Rationale:**
- Team/tooling preference and faster local iteration without Epic Launcher / UE install footprint
- **Netcode for GameObjects** (or Unity Multiplayer Services) for dedicated-server replication
- Large asset store and hiring pool for MMO UI/economy tooling
- Headless Linux server builds for sector hosting on GameLift/EC2/K8s

**Stack choices:**
| Area | Choice |
|------|--------|
| Render pipeline | URP default; HDRP optional for flagship visuals |
| Networking | Netcode for GameObjects + Unity Transport; server-authoritative |
| UI | UI Toolkit for MMO HUD/menus; uGUI where pragmatic |
| Content | Addressables + ScriptableObject data definitions |
| Tests | Unity Test Framework (Edit Mode + Play Mode) |
| CI | `unity-builder` GitHub Action or self-hosted Unity runner |

**Consequences:**
- REQ-032/033 **Unreal implementation is legacy** — do not extend; re-implement foundation on Unity in a follow-up REQ
- Backend/orchestrator architecture **unchanged** (still Docker/K8s microservices)
- Design docs module names remain conceptual; implementation uses C# namespaces/assemblies

**Rejected (for this project):** Stay on UE5 (install/team constraints), Godot (MMO net maturity), custom engine (time-to-market).

## Legacy Unreal artifacts

The repository may still contain an early UE5 scaffold (`IronExiles.uproject`, `Source/`, `Scripts/*Editor*.ps1`) from REQ-032/033. Treat these as **deprecated** until removed in a cleanup REQ. All new game code lives under `Client/` (Unity project root).
