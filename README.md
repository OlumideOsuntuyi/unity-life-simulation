# Unity Life Simulation

A from-scratch artificial-life / evolution simulator built in Unity 2022.3 (URP). Animals with genetically-inherited traits hunt, flee, mate, and reproduce inside a custom simulation engine that runs independently of Unity's own GameObject/Transform/Physics stack, alongside a separate flocking ("boids") subsystem for schooling/shoaling creatures.

## What's actually in here

**A custom simulation core** (`Assets/ConsoleScripts`, namespace `Simulation`) — its own `Transform`, `Collider`, rigid body (`DynamicBody`), and math library (`Vector3`), deliberately independent of `UnityEngine`'s equivalents so simulation logic can eventually run off Unity's main thread.

- **Genetics** — `Chromosome` (XX/XY), `Genes`/`GenePotential`/`GeneStrength` implement Mendelian-style dominant/recessive/heterozygous ("Tt") inheritance, with a mutation system driven by a per-trait reward score and a "gestation energy" budget spent on offspring stat changes.
- **Body & behavior** — `Status` (health/stamina/saturation/reproductive urge), `Memory`/`Relationships` (friend/enemy/family/neutral), `Eyes` (raycast-based perception), `Locomotion`, and an `ActionHandler` state machine (Idle → Searching → Attacking → Fleeing → Sleeping) driving predator/prey and mate-seeking behavior.
- **World** — a custom raycaster/collision system and procedural terrain (`FastNoise`-driven continental + erosion fractal layers baked to a heightmap).

**A boids subsystem** (`Simulation.Boids`) — spatial-chunked (16³ grid) flocking simulation with cohesion/alignment/avoidance rules and its own lightweight physics (`VoxelPhysics`).

**Unity-side tooling** (`Assets/UnityScripts`) — a **Species Creator** UI for hand-designing species by spending a gene "points budget", saved/loaded as a JSON species library, plus screen/audio management and two background-thread dispatchers (`ThreadsManager`, `MainThreadDispatcher`) that keep simulation work off Unity's main thread.

## Requirements

- Unity **2022.3.30f1** (URP)
- Packages are pinned in `Packages/manifest.json`, including `com.unity.test-framework` for EditMode/PlayMode tests

## Getting started

1. Open the project folder in Unity Hub (Unity 2022.3.30f1).
2. Open `Assets/Scenes/SampleScene.unity` for the main simulation, or `Assets/Scenes/Bolds.unity` for the boids/flocking demo, or `Assets/Scenes/TerrainTest.unity` for terrain generation.
3. Press Play. In the main scene, use the Species Creator UI to design species (or use the defaults) before starting the simulation.

## Known issues / in-progress work

This project is under active development. A first pass of correctness bugs (broken collision detection, thread-safety issues in the background dispatchers, a few genetics/terrain edge cases) has been fixed — see commit history for details. Remaining rough edges:

- No automated test suite yet, despite the test framework being installed.
- The custom `Simulation` engine and Unity's own physics/transform stack coexist, which duplicates some functionality.
- Several stubbed features are wired in but currently inert (e.g. `Memory.Update()`, `Collider.CheckInRange()`).

## Third-party assets

`Assets/Models/` and `Assets/IgniteCoders/Simple Water Shader/` bundle third-party Unity Asset Store content (low-poly animal/fish packs, water shader, etc.). Check each package's individual license before redistributing this repo's contents beyond personal/educational use.

## License

No license file is currently included — all rights reserved by default. Add a `LICENSE` file if you want to permit reuse.
