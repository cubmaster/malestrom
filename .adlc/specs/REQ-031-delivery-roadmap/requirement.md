---
id: REQ-031
title: "Incremental Delivery Roadmap"
status: draft
deployable: false
created: 2026-06-11
updated: 2026-06-11
component: "adlc/spec"
domain: "game"
stack: ["unreal", "cpp", "postgresql", "redis"]
concerns: ["developer-experience", "testability", "reliability"]
tags: ["incremental-delivery", "roadmap", "phased-rollout", "mvp"]
---

## Description

Iron Exiles is a large MMO; shipping it as one monolith would be untestable and un-runnable for long stretches. This requirement defines the **incremental delivery strategy**: a ordered sequence of REQs where each increment produces a **runnable demo** with **automated verification** before the next increment begins.

Each child REQ (REQ-032 through REQ-050+) must satisfy three gates before it is considered complete:
1. **Runnable** — A developer can launch it locally (editor, dedicated server, or backend service) without manual setup beyond documented steps.
2. **Testable** — Automated tests cover core behavior (UE Automation Tests for gameplay, integration tests for services).
3. **Integrated** — The increment builds on prior REQs without breaking their demos.

### Delivery Tiers

| Tier | REQs | Runnable Demo |
|------|------|---------------|
| **A — Foundation** | REQ-032, REQ-033, REQ-034 | Fly a ship in a test sector (single-player, editor PIE) |
| **B — Multiplayer Shell** | REQ-035, REQ-036, REQ-037 | Two clients connect to dedicated server; see each other move |
| **C — Combat MVP** | REQ-038, REQ-039, REQ-040, REQ-041 | Two players fight a beam weapon + shields + dummy NPC in one sector |
| **D — Persistence** | REQ-042, REQ-043, REQ-044 | Login, create character, reconnect with saved loadout |
| **E — Progression** | REQ-045, REQ-046, REQ-047 | Earn combat XP, use one AI ability, equip TL1–TL3 gear |
| **F — World Loop** | REQ-048, REQ-049, REQ-050 | Travel between two sectors; complete a combat mission; dock at station |
| **G — Economy & Social** | REQ-051+ (future) | Craft, auction, party chat (spec separately) |
| **H — Content Scale** | REQ-060+ (future) | Full galaxy, all factions, endgame (spec separately) |

### Dependency Graph (Critical Path)

```
REQ-032 → REQ-033 → REQ-034
              ↓
         REQ-035 → REQ-036 → REQ-037
              ↓
    REQ-038 → REQ-039 → REQ-040 → REQ-041
              ↓
         REQ-042 → REQ-043 → REQ-044
              ↓
         REQ-045 → REQ-046 → REQ-047
              ↓
         REQ-048 → REQ-049 → REQ-050
```

Parallel work is allowed only when REQs do not share the dependency chain (e.g., REQ-034 HUD can proceed alongside REQ-035 server bootstrap once REQ-033 exists).

## System Model

### Entities

| Entity | Field | Type | Constraints |
|--------|-------|------|-------------|
| DeliveryIncrement | id | string | REQ-xxx slug |
| DeliveryIncrement | tier | string | A through H |
| DeliveryIncrement | depends_on | string[] | Prior REQ ids |
| DeliveryIncrement | runnable_demo | string | One-sentence demo description |
| DeliveryIncrement | test_suite | string | Test project or command name |

### Events

| Event | Trigger | Payload |
|-------|---------|---------|
| increment_complete | All acceptance criteria met for a child REQ | `{ req_id, demo_verified, test_passed }` |
| tier_demo_verified | Last REQ in tier passes integration demo | `{ tier, req_ids[] }` |

### Permissions

| Action | Roles Allowed |
|--------|---------------|
| advance_to_next_req | implementer after `/validate` + `/reflect` |
| skip_increment | tech lead only, with documented rationale |

## Business Rules

- [ ] BR-1: No REQ may start implementation until all listed dependencies are `approved` or `deployed`.
- [ ] BR-2: Each child REQ must include a **Runnable Demo** acceptance criterion describing the exact launch steps.
- [ ] BR-3: Each child REQ must include at least one **automated test** acceptance criterion.
- [ ] BR-4: Tier G and H increments are out of scope for REQ-031; they are tracked as future REQs after Tier F is deployed.
- [ ] BR-5: Design fidelity follows `docs/` GDDs; where GDD conflicts with incremental scope, the child REQ's Out of Scope section wins until a later REQ expands scope.

## Acceptance Criteria

- [ ] Child REQs REQ-032 through REQ-050 exist under `.adlc/specs/` with dependency references.
- [ ] Each tier has a one-sentence runnable demo defined in this roadmap.
- [ ] Critical path dependency graph is documented and acyclic.
- [ ] `/proceed` can be invoked on REQ-032 without requiring Tier G/H specs.

## External Dependencies

- Unreal Engine 5 (version pinned in REQ-032)
- Design documents in `docs/`

## Assumptions

- Implementation begins with a single playable faction (Human) and expands to Velnari/Kethari/Oruneti in later tiers.
- Backend services start as local Docker Compose stacks before cloud deployment.
- Placeholder art is acceptable until asset pipeline REQs are written.

## Open Questions

- [ ] Pin exact UE5 version (5.4 vs 5.5) at REQ-032 kickoff?
- [ ] Use Lyra sample project as base or greenfield UE project?

## Out of Scope

- Full 20+ sector galaxy (Tier H)
- Auction house, guilds, faction wars (Tier G — separate future REQs)
- Production AWS/GameLift deployment (separate infra REQs)
- Console ports

## Retrieved Context

No prior context retrieved — no tagged documents matched this area.
