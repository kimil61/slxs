# Repository Guidelines

## Project Context
- This repo is for the Unity game `色狼下山 (Saek-Nang-Ha-San)`.
- `DESIGN.md` is the design source of truth for architecture, scope, and roadmap.
- `DEVLOG.md` is the implementation reality log and current task tracker.
- `README.md` is the lightweight onboarding overview.

## Current State
- The project is in late `Phase 1` / early `Phase 2` relative to `DESIGN.md`.
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
- Follow `DESIGN.md` for intended design.
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
- Preserve the architecture direction from `DESIGN.md`: stable core, gameplay tuned through ScriptableObjects where practical.
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
- Update `DESIGN.md` only when intended design changes.
- Update `DEVLOG.md` when implementation status or priorities change.
- Update `README.md` when onboarding information changes.
- Read the latest file(s) in `ChatLogs/` at the start of a new session when recent implementation context matters.
- Add or update the current date's file in `ChatLogs/` at the end of meaningful work sessions so Unity Editor actions, tuning decisions, and next steps are preserved.
- Treat user requests such as `오늘 작업 정리해줘`, `오늘 작업 기준으로 문서 업데이트해줘`, or similar end-of-session phrasing as a cue to update the relevant project docs, typically `ChatLogs/`, `DEVLOG.md`, and when needed `README.md`, `AGENTS.md`, or `DESIGN.md`.
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
- Preferred workflow for new sessions:
  1. Read `AGENTS.md`
  2. Read `DEVLOG.md`
  3. Read `DESIGN.md`
  4. Read the latest relevant file(s) under `ChatLogs/`
  5. Continue work from that documented baseline instead of depending on old long chat threads
