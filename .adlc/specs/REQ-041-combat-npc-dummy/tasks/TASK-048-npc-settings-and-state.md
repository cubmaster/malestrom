# TASK-048: NPC Settings and State Enum

## Objective
Create the foundational types for the NPC AI system: settings constants and the brain state enum.

## Files to Create
- `Client/Assets/_Project/Scripts/Combat/AI/NPCSettings.cs` - Static configuration constants
- `Client/Assets/_Project/Scripts/Combat/AI/NPCBrainState.cs` - State machine states enum

## Acceptance Criteria
- NPCSettings exposes all configurable NPC parameters as constants
- NPCBrainState enum has: Idle, Patrol, Combat, Dead
- Namespace: IronExiles.Combat.AI

## Dependencies
None (foundation task)
