# TASK-052: Bootstrap Wireup and Tests

## Objective
Wire the NPC spawner into the multiplayer bootstrap so NPCs spawn in the test sector, and add edit-mode tests for NPC AI logic.

## Files to Modify
- `Client/Assets/_Project/Scripts/Networking/EmptySectorMultiplayerBootstrap.cs` - Add NPCSpawner setup

## Files to Create
- `Client/Assets/_Project/Tests/EditMode/NPCBrainTest.cs` - Unit tests for state transitions and aggro logic

## Acceptance Criteria
- NPCs spawn on server start in test sector
- Test validates: state transitions (Patrol→Combat on aggro, Combat→Patrol on disengage)
- Test validates: NPC damage application triggers death state

## Dependencies
- TASK-051
