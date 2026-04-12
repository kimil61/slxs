# Repository Guidelines

## Project Context
- This repo is for the Unity game `色狼下山 (Saek-Nang-Ha-San)`.
- The main design source is `a.md`.
- Progress history and implementation notes live in `DEVLOG.md`.
- `README.md` is the lightweight project overview, not the full source of truth.

## Current State
- The project is in an early prototype / architecture stage.
- Claude-generated foundation code already exists under `Assets/_Project/Scripts/`.
- Real Unity scene wiring, animation hookup, prefab setup, inspector tuning, and gameplay validation are still largely manual work to be done in the Unity Editor.
- Current playable scene content is still centered around `Assets/Scenes/SampleScene.unity`.
- There is only one real scene in build settings right now: `Assets/Scenes/SampleScene.unity`.

## Source Of Truth
- Follow `a.md` first for architecture and feature direction.
- Use `DEVLOG.md` to understand what was attempted or partially implemented.
- Treat the actual files in `Assets/_Project/Scripts/` as the code baseline, even if docs lag behind.
- If docs and code diverge, do not silently guess. Call out the mismatch and decide explicitly which side to update.

## Code Layout
- Main gameplay code lives in `Assets/_Project/Scripts/`.
- Current subfolders:
  - `Core/`
  - `Camera/`
  - `Combat/`
  - `Player/`
  - `Enemy/`
  - `Level/`
  - `UI/`
  - `Utils/`
- Legacy or prototype-era assumptions may still exist in `SampleScene.unity` and some generated scripts.

## Important Technical Notes
- `Assets/Scripts/` is effectively not the active gameplay code location now.
- `Assets/_Project/Scripts/Player/PlayerInputHandler.cs` expects a generated `PlayerControls` input wrapper class.
- The file currently named `Assets/_Project/Scripts/Player/PlayerControls.cs` is not that generated wrapper; it contains a `PlayerController` MonoBehaviour instead.
- As a result, the current C# project does not cleanly compile yet.
- `SampleScene.unity` still contains prototype-era component references and should be treated as a migration scene, not a finalized gameplay scene.

## Working Rules
- Prefer extending and fixing `Assets/_Project/Scripts/` rather than creating parallel duplicate systems elsewhere.
- Preserve the architecture direction from `a.md`: core systems stable, gameplay tuning exposed through ScriptableObjects where practical.
- Do not rewrite large systems casually just because generated code looks rough; first verify whether the issue is scene wiring, inspector data, or a genuine code bug.
- Before removing or replacing generated code, inspect whether Unity scene objects, prefabs, or serialized references depend on it.
- Keep comments short and high-signal.

## Unity Workflow Expectations
- Many tasks in this repo require both code changes and Unity Editor setup.
- Code-side tasks:
  - fix compile errors
  - improve architecture
  - debug runtime logic
  - add missing systems
- Editor-side tasks:
  - assign Animator Controllers
  - wire references in inspectors
  - create prefabs and ScriptableObject assets
  - configure Input System generation
  - validate scenes, colliders, NavMesh, and animation timing
- When a task cannot be completed from code alone, explicitly state what must be done in the Unity Editor.

## Documentation Maintenance
- If architecture changes, update `a.md` only when the intended design changed.
- If implementation status changed, update `DEVLOG.md`.
- If onboarding or repo overview changed, update `README.md`.
- Keep these files in UTF-8.

## Recommended Next Checks
- Fix the `PlayerControls` input wrapper mismatch so the project compiles.
- Audit `SampleScene.unity` against the new architecture in `Assets/_Project/Scripts/`.
- Decide whether to migrate `SampleScene` forward or create new scenes such as `Boot`, `MainMenu`, `GamePlay`, and `Hub`.
- Verify which generated systems are code-complete versus only skeletons.

## AGENTS Usage
- No separate `.init` step is required for this repo.
- `AGENTS.md` at the repository root is enough as the local project instruction file for future agent sessions.
