# TASK-049: NPC Brain State Machine

## Objective
Implement the server-side AI state machine that drives NPC behavior: patrol, aggro detection, combat engagement, and death handling.

## Files to Create
- `Client/Assets/_Project/Scripts/Combat/AI/NPCBrain.cs` - MonoBehaviour state machine

## Design
- Runs only on server (guard with IsServer check or disable on clients)
- Patrol: wander around spawn point within patrol_radius
- Aggro: detect nearest player within aggro_radius using Physics.OverlapSphere or TargetableEntity scan
- Combat: lock target, enable firing, orient toward target
- Death: handled by NetworkDamageableHealth.Destroyed event (NPCSpawner manages respawn)
- Disengage: if target moves beyond disengage_radius, return to patrol

## Dependencies
- TASK-048 (NPCSettings, NPCBrainState)
