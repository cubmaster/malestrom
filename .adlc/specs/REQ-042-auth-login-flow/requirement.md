---
id: REQ-042
title: "Auth Service & Login Flow (Local Stack)"
status: draft
deployable: true
created: 2026-06-11
updated: 2026-06-11
component: "backend/AuthService"
domain: "auth"
stack: ["postgresql", "redis", "docker", "unreal"]
concerns: ["security", "reliability", "testability"]
tags: ["login", "jwt", "docker-compose", "tier-d"]
---

## Description

Deliver a minimal Auth microservice with register/login endpoints, JWT issuance, and UE5 client login screen that gates multiplayer connect. Runs via Docker Compose locally alongside PostgreSQL.

**Why:** Persistent MMO identity starts with auth; isolating it early prevents bolting auth onto an anonymous multiplayer stack later.

**Depends on:** REQ-035 (session connect after token)

**Runnable Demo:** `docker compose up` → register account in client → login → receive token → connect to dedicated server with token validated by server or gateway stub.

Reference: `docs/05-architecture.md` AuthService, accounts schema.

## System Model

### Entities

| Entity | Field | Type | Constraints |
|--------|-------|------|-------------|
| Account | account_id | UUID | PK |
| Account | email | string | unique, required |
| Account | password_hash | string | bcrypt or argon2 |
| SessionToken | jwt | string | exp claim, signed |
| SessionToken | account_id | UUID | FK |

### Events

| Event | Trigger | Payload |
|-------|---------|---------|
| account_registered | POST /register | `{ account_id, email }` |
| login_success | POST /login valid | `{ token, expires_at }` |
| login_failed | invalid creds | `{ reason }` |

### Permissions

| Action | Roles Allowed |
|--------|---------------|
| register | anonymous |
| login | anonymous |
| connect_game_server | authenticated (valid JWT) |

## Business Rules

- [ ] BR-1: Passwords never stored plaintext; minimum length 8 enforced.
- [ ] BR-2: JWT expires within 24h (configurable); refresh flow out of scope.
- [ ] BR-3: Failed login rate limit: 5 attempts / minute / IP (basic).
- [ ] BR-4: Game server rejects connect without valid token when auth mode enabled (flag to bypass for dev).

## Acceptance Criteria

- [ ] Docker Compose starts Auth + PostgreSQL with documented env file.
- [ ] REST register/login integration tests pass in CI.
- [ ] UE5 login UI accepts credentials and stores token for session connect.
- [ ] Dedicated server validates token (direct HTTP or shared secret stub documented).
- [ ] **Runnable Demo:** New user registers, logs in, joins REQ-035 server flow with auth enabled.

## External Dependencies

- Docker, PostgreSQL
- HTTP client in UE (VaRest, custom, or Epic Online integration)

## Assumptions

- HTTPS terminated at gateway in production; local HTTP OK for dev.
- Email verification deferred.

## Open Questions

- [ ] Validate JWT in UE dedicated server directly or via sidecar?

## Out of Scope

- OAuth / Steam login
- Anti-cheat
- Password reset flow

## Retrieved Context

No prior context retrieved — no tagged documents matched this area.
