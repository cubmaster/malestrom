# Iron Exiles — Architecture

## System Diagram

```
┌─────────────────────────────────────────────────────────────────────────┐
│                           CLIENT (UE5)                                   │
├─────────────────────────────────────────────────────────────────────────┤
│  Combat │ Galaxy/Travel │ Economy/Craft │ Social/Chat                   │
│         └───────────────┴───────────────┴─────────────────────────────│
│                    Client Core (Ship AI, HUD, Physics, Net Prediction) │
│                                    │                                     │
│                            [UE5 Replication]                             │
└────────────────────────────────────┼─────────────────────────────────────┘
                                     │ Network
┌────────────────────────────────────┼─────────────────────────────────────┐
│                         SERVER INFRASTRUCTURE                              │
├───────────────────────────────────────────────────────────────────────────┤
│  Game Servers (UE5 Dedicated)  │  World Server (Orchestrator)  │  Backend │
│  - Sector host (50-100 players)│  - Sector mgmt, routing       │  Micro- │
│  - Combat/NPC/loot authority   │  - Load balance, instances    │  services│
│                                │  - World events               │  (K8s)   │
│  ┌─────────────────────────────┴───────────────────────────────┐          │
│  │  PostgreSQL (persist) │ Redis (cache) │ InfluxDB (metrics) │          │
│  └─────────────────────────────────────────────────────────────┘          │
└───────────────────────────────────────────────────────────────────────────┘
```

## Layers

| Layer | Responsibility |
|-------|----------------|
| **Client (UE5)** | Rendering, input, local prediction, UI (UMG), audio, local ship AI companion |
| **Dedicated Game Servers** | Sector simulation, server-authoritative combat, NPC AI, loot, replication |
| **World Orchestrator** | Sector spin-up/down, player routing, load balancing, instanced content, world events |
| **Backend Microservices** | Auth, accounts, economy, chat, guilds, missions, matchmaking, analytics |
| **Data Layer** | PostgreSQL (persistent), Redis (session/cache), InfluxDB (telemetry) |

## Key Patterns

- **Sector instancing** — Open-world sectors run as dedicated UE5 server instances; private missions/raids use pooled instances
- **Server-authoritative combat** — Client-initiated actions validated server-side; damage/HP replicated from server
- **Client prediction** — Ship movement predicted locally, reconciled with server state
- **Microservices backend** — Stateless services for non-realtime logic; game servers handle realtime simulation
- **Container-first backend** — Docker Compose locally, Kubernetes in production; every backend service is containerized (see `.adlc/ETHOS.md` §7)
- **Triple-XP progression** — Combat, trade, and explore XP tracked independently with combined level cap
- **Data-driven content** — UE5 DataTables for hulls, weapons, AI abilities; PostgreSQL for player/inventory state
- **Module-based client architecture** — Combat, Ship AI, Progression, Economy, Galaxy, Social, Mission modules

## ADRs

- **ADR-001 (proposed):** Unreal Engine 5 as primary engine — Nanite/Lumen visuals, proven dedicated server framework, GAS for abilities
- **ADR-002 (proposed):** Sector-based MMO architecture over single-shard — manageable server load, horizontal scaling
- **ADR-003 (proposed):** PostgreSQL + Redis data split — persistent relational data vs. real-time session/cache
- **ADR-032-1 (approved):** Greenfield UE5 project — no Lyra fork; custom MMO modules added incrementally (REQ-032)
- **ADR-032-2 (approved):** Pin UE 5.5.x for editor builds; upgrades via new REQ only (REQ-032)
- **ADR-032-4 (approved):** Self-hosted GitHub Actions runner + `Scripts/Build-Editor.ps1` for CI (REQ-032)
