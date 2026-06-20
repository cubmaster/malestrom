---
id: TASK-028
title: "CI workflow: self-hosted Linux server build + 2-client verification"
status: complete
parent: REQ-035
created: 2026-06-19
updated: 2026-06-19
dependencies: [TASK-027]
---

## Description

Create a GitHub Actions workflow on a self-hosted runner that builds the Linux dedicated server and runs an automated 2-client connection verification test.

## Files to Create/Modify

- `.github/workflows/unity-server-build.yml` — workflow: checkout → build Linux headless server → launch server → connect 2 batchmode client instances → assert connected_count==2 and spawned_ships==2 → teardown
- `Client/Assets/_Project/Scripts/Networking/MultiplayerVerificationTest.cs` — batchmode-friendly script that connects as client, waits for spawn, logs success/failure, exits with code
- `Client/Assets/_Project/Editor/ServerBuildMenu.cs` — add `BuildLinuxServer` static method callable from batchmode

## Acceptance Criteria

- [ ] Workflow runs on `self-hosted, unity, linux` runner labels
- [ ] Linux headless server binary is built successfully in CI
- [ ] Automated test launches server + 2 client processes
- [ ] Test asserts: both clients connected AND both ships spawned (exit 0 on success, exit 1 on failure)
- [ ] Workflow fails the PR if verification test fails
- [ ] CI logs show connected client count and spawn count

## Technical Notes

- Self-hosted runner must have Unity 6000.x installed with valid license activation
- Verification approach: server + 2 clients all run as batchmode processes; clients auto-connect, wait N seconds for spawn, then report via stdout/exit code
- Alternative: Unity Play Mode test with `MultiplayerPlayMode` package (if available on the runner)
- Timeout: 30 seconds max per connection attempt; fail fast if server unreachable
- Cleanup: kill server process in `post` step or trap
