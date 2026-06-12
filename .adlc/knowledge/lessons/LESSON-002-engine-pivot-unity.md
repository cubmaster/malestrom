---
id: LESSON-002
title: "Engine pivot — Unreal to Unity (ADR-034)"
created: 2026-06-12
related: [REQ-032, REQ-033, REQ-051, ADR-034]
tags: [unity, architecture, migration]
---

## Context

REQ-032/033 delivered a UE5 C++ scaffold (project, flight prototype). Development was blocked by Unreal install size, `UE_ROOT` configuration, and team preference for Unity.

## Decision

Architecture updated to **Unity 6 LTS + Netcode for GameObjects** (ADR-034). Backend microservices, sector orchestration, PostgreSQL/Redis, and Docker/K8s deployment model are **unchanged**.

## Lesson

- Do not extend `Source/IronExiles/` or Unreal-specific scripts for new features
- Re-platform foundation (project scaffold, flight, tests) under `Client/` in a dedicated REQ before continuing Tier B multiplayer
- Update ADLC specs that reference `unreal`, `cpp`, Automation Tests, or `.uproject` when those REQs are re-opened

## Follow-up

- ~~Create REQ for Unity project foundation (replacement for REQ-032 scope)~~ → **REQ-051**
- Move or delete `legacy/unreal/` after Unity foundation + flight re-platform land
