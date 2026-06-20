---
id: TASK-024
title: "Create IronExiles.Networking assembly and NetworkSessionManager"
status: complete
parent: REQ-035
created: 2026-06-19
updated: 2026-06-19
dependencies: [TASK-023]
---

## Description

Create the `IronExiles.Networking` assembly and implement `NetworkSessionManager` — the entry point that decides server vs. client mode and starts the appropriate NGO role.

## Files to Create/Modify

- `Client/Assets/_Project/Scripts/Networking/IronExiles.Networking.asmdef` — references Unity.Netcode.Runtime, Unity.Networking.Transport, IronExiles.Combat, IronExiles.Core
- `Client/Assets/_Project/Scripts/Networking/NetworkSessionManager.cs` — MonoBehaviour that auto-starts as server (headless) or client (player build); configures UTP with address/port from CLI args or defaults (127.0.0.1:7878)
- `Client/Assets/_Project/Scripts/Networking/NetworkCommandLineArgs.cs` — parses `-connectAddress`, `-connectPort`, `-serverPort` from command line

## Acceptance Criteria

- [ ] `IronExiles.Networking.asmdef` compiles with correct references
- [ ] Headless/server build auto-starts as NetworkManager server on port 7878
- [ ] Client build auto-connects to 127.0.0.1:7878 (overridable via CLI)
- [ ] NetworkManager is added to the EmptySector scene (or spawned by bootstrap)
- [ ] Edit Mode test validates CLI arg parsing logic

## Technical Notes

- Detect headless via `Application.isBatchMode` or `#if UNITY_SERVER` define
- NetworkManager should be a scene singleton; place on a dedicated GameObject in EmptySector
- UTP connection data: `UnityTransport.SetConnectionData(address, port)`
- Default max connections: 10 (per spec)
