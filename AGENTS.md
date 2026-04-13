# Repository Guidelines

## Project Context
- This repo is for the Unity game `色狼下山 (Saek-Nang-Ha-San)`.
- `a.md` is the design source of truth for architecture, scope, and roadmap.
- `DEVLOG.md` is the implementation reality log and current task tracker.
- `README.md` is the lightweight onboarding overview.

## Current State
- The project is in late `Phase 1` / early `Phase 2` relative to `a.md`.
- The active test scene is `Assets/Scenes/SampleScene.unity`.
- The active code baseline is `Assets/_Project/Scripts/`.
- The active gameplay tuning assets are in `Assets/_Project/ScriptableObjs/`.
- Current playable features in-scene:
  - WASD movement
  - third-person camera follow
  - idle / running animation
  - dodge on `Space`
  - light attack on left click
  - heavy attack on `Shift + left click`

## Source Of Truth
- Follow `a.md` for intended design.
- Follow `DEVLOG.md` for what is actually implemented now.
- If design and implementation diverge, do not guess silently. Call out the mismatch and decide which document or system to update.

## Active Layout
- Main gameplay code:
  - `Assets/_Project/Scripts/Core`
  - `Assets/_Project/Scripts/Camera`
  - `Assets/_Project/Scripts/Combat`
  - `Assets/_Project/Scripts/Player`
  - `Assets/_Project/Scripts/Enemy`
  - `Assets/_Project/Scripts/Level`
  - `Assets/_Project/Scripts/UI`
  - `Assets/_Project/Scripts/Utils`
- Main tuning assets:
  - `Assets/_Project/ScriptableObjs/`
- Do not treat `Assets/Scripts` as the primary gameplay code location.

## Current Technical Notes
- The current C# project compiles.
- Player control is now driven by `PlayerStateMachine`, not by the old prototype scripts.
- Gameplay feel is intentionally data-driven through `AttackData`, `DodgeData`, `StaminaData`, and `CombatTuningData`.
- `SampleScene.unity` is still a working testbed, not a final production scene layout.

## Working Rules
- Prefer extending and fixing `Assets/_Project/Scripts/` instead of creating parallel duplicate systems.
- Preserve the architecture direction from `a.md`: stable core, gameplay tuned through ScriptableObjects where practical.
- Before replacing generated code, verify whether the issue is scene wiring, serialized references, tuning data, Animator setup, or an actual code defect.
- Keep code comments short and useful.
- When tuning combat or dodge feel, prefer adjusting ScriptableObject values before rewriting logic.

## Unity Workflow Expectations
- Many tasks in this repo require both code work and Unity Editor work.
- Code-side tasks:
  - state machine logic
  - combat logic
  - input cleanup
  - bug fixing
  - AI / level / UI integration
- Editor-side tasks:
  - Animator states and transitions
  - assigning ScriptableObject references
  - prefab setup
  - collider and hitbox setup
  - NavMesh baking
  - validating animation timing and scene references
- If a task cannot be completed from code alone, explicitly state what must be done in the Unity Editor.

## Documentation Maintenance
- Update `a.md` only when intended design changes.
- Update `DEVLOG.md` when implementation status or priorities change.
- Update `README.md` when onboarding information changes.
- Keep project docs in UTF-8.

## Asset And Git Rules
- Commit Unity `.asset` files together with their `.meta` files.
- Commit ScriptableObject tuning assets when they are required for scene behavior.
- Do not remove or regenerate Unity metadata casually.

## Recommended Immediate Priorities
1. Stabilize `Slash1 -> Slash2` combo behavior.
2. Add one test enemy and verify hitbox / damage / hit feedback.
3. Validate hit stop, knockback, camera shake, and audio layering in actual combat.
4. Move into NavMesh-based enemy integration.
5. Split out a proper `GamePlay` scene when `SampleScene` stops being manageable.

## AGENTS Usage
- No separate `.init` step is required.
- `AGENTS.md` at the repository root is the local instruction file for future agent sessions.
