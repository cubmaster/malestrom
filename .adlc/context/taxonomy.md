# Taxonomy — Retrieval Tag Vocabulary

Iron Exiles (Malestrom) legal values for retrieval tag dimensions.

**This file is project-local.** Extend as new areas emerge. Values are advisory — consistent vocabulary improves retrieval quality.

**Note on `tags`:** the `tags` dimension is intentionally free-form and is NOT enumerated here.

## component (narrow area)

Single string. Hierarchical where helpful.

- `game/foundation`
- `game/combat/Movement`
- `game/combat/WeaponSystem`
- `game/combat/DefenseSystem`
- `game/combat/Targeting`
- `game/combat/PowerManager`
- `game/networking/Session`
- `game/networking/Replication`
- `game/ui/HUD`
- `game/ui/StationMenu`
- `game/ai/NPCAIModule`
- `game/ai/ShipAIModule`
- `game/progression/XPSystem`
- `game/progression/ShipProgression`
- `game/galaxy/Travel`
- `game/missions/QuestSystem`
- `backend/AuthService`
- `backend/AccountService`
- `adlc/spec`

## domain (broad area)

- `combat`
- `networking`
- `progression`
- `world`
- `ui`
- `auth`
- `data`
- `infra`
- `game`
- `adlc`

## stack (tech layers)

- `unreal`
- `cpp`
- `umg`
- `postgresql`
- `redis`
- `docker`
- `docker-compose`
- `github-actions`
- `kubernetes`

## concerns (cross-cutting dimensions)

- `security`
- `performance`
- `reliability`
- `testability`
- `developer-experience`
- `observability`

## tags (free-form)

Examples: `6dof`, `dedicated-server`, `beam-weapon`, `triple-xp`, `jump-gate`, `incremental-delivery`, `tier-a`, `mvp`, `pve`, `jwt`.

The `tags` dimension is the lowest-weight signal in retrieval (+1 per match).
