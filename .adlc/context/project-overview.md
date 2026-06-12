# Iron Exiles — Project Overview

## What It Does

Iron Exiles is a multiplayer space combat MMO inspired by *Earth and Beyond* (2002), set in the Iron Exiles universe. Players choose a faction, receive a starter ship with an AI companion, and progress through combat, trade, and exploration across a persistent galaxy. Core loops include real-time ship combat, triple-XP progression (combat/trade/explore), ship AI leveling, crafting and economy, faction reputation, guilds, and sector-based world travel with instanced missions and raids.

## Tech Stack

| Layer | Technology |
|-------|------------|
| Game Client & Dedicated Servers | Unreal Engine 5 (C++ core, Blueprint gameplay) |
| Networking | UE5 replication, dedicated servers, sector instancing |
| Backend Services | Containerized microservices (Auth, Account, Economy, Chat, Guild, Mission, etc.) |
| Local Development | Docker + Docker Compose (all backend deps and services) |
| Production Deployment | Kubernetes (EKS or equivalent) |
| Primary Database | PostgreSQL |
| Cache | Redis |
| Metrics | InfluxDB |
| Object Storage | S3-compatible (assets, logs) |
| Game Server Hosting | AWS GameLift / EC2 (UE5 dedicated servers — containerized where practical) |
| CI/CD | GitHub Actions + Jenkins (planned); build and push container images |

## Project Scope

**In scope (design phase):**
- Game design documents for races, ships, planets, combat, leveling, assets, multiplayer, and architecture
- UE5 client-server MMO with sector-based instancing (50–100 players per sector)
- Server-authoritative combat with client prediction
- Triple-XP progression, ship AI companion system, economy/crafting, social/guild systems
- Persistent galaxy with jump gates, wormholes, and instanced content

**Out of scope (current repo state):**
- Implemented game code (repo currently contains design docs only)
- Live backend infrastructure deployment
- Console ports (secondary target, post-PC)
- Modding support (future consideration)
