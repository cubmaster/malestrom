# Legacy Unreal Engine 5 scaffold

This folder is the **planned** home for the deprecated UE5 tree from REQ-032 and REQ-033.

## Status

The Unreal project currently lives at the **repository root** (`IronExiles.uproject`, `Source/`, `Config/`, `Content/`, UE `Scripts/`). It remains there until **REQ-051** (Unity project foundation) is complete.

## Policy (ADR-034)

- **Do not** add new gameplay features to `Source/IronExiles/` or Unreal-specific automation.
- **Do not** extend child REQs against the UE implementation; re-specify for Unity when those increments are implemented.
- After Unity foundation + flight re-platform, move or delete this legacy tree in a cleanup task.

## Reference

- ADR-034: `.adlc/context/architecture.md`
- LESSON-002: `.adlc/knowledge/lessons/LESSON-002-engine-pivot-unity.md`
- Completed specs: `.adlc/specs/REQ-032-project-foundation/`, `.adlc/specs/REQ-033-ship-flight-prototype/`
