---
id: LESSON-006
title: "Pack multi-field replicated state into a single NetworkVariable struct"
domain: game
component: game/combat/DefenseSystem
stack: [unity, csharp, netcode]
concerns: [performance, bandwidth]
tags: [networking, replication, shields, combat]
req: REQ-040
created: 2026-06-28
updated: 2026-06-28
---

## Context

REQ-040 needed to replicate 4 shield facing HP values from server to all clients every frame during combat. The naive approach (4 separate `NetworkVariable<float>`) would trigger 4 independent dirty checks and 4 independent network messages per tick.

## Lesson

Pack related fields that change together into a single `INetworkSerializable` struct and use one `NetworkVariable<TStruct>`. This sends a single update when ANY field changes, reducing per-tick overhead from N dirty checks + N packets to 1 check + 1 packet. The pattern was already established by `ReactorPowerNetworkState` (REQ-038) for power allocation — shields followed the same pattern with `ShieldNetworkState`.

## Takeaway

When adding replicated state where multiple fields change on the same server tick (damage to a facing + regen on another), prefer a packed struct over individual NetworkVariables. Reserve individual NetworkVariables for independently-changing values that update at different rates (e.g., hull HP vs. movement speed).
