# TASK-050: NPC Ship Controller

## Objective
Translate NPCBrain decisions into movement commands, feeding synthetic input to ShipMovementModel so the NPC physically moves and rotates.

## Files to Create
- `Client/Assets/_Project/Scripts/Combat/AI/NPCShipController.cs` - Server-side movement driver

## Design
- Reads current brain state and target from NPCBrain
- Patrol: generate waypoints within patrol_radius, fly toward each at patrol_speed_fraction
- Combat: orient toward locked target, maintain engagement distance
- Directly drives ShipMovementController.SimulateInput() with synthetic ShipMovementInput
- No NetworkShipMovementController needed (server owns the object, position replicates via NetworkTransform or equivalent)

## Dependencies
- TASK-048, TASK-049
