Game Feature Roadmap — Prioritized Additions

Purpose
- Provide a clear, ordered list of systems and features to add to the project to make it competitive with modern RPGs.
- Each item includes a brief description, priority, estimated effort, dependencies, target files/areas, and concrete first steps.

How to use this document
- Follow the list top-to-bottom for highest impact first.
- "Effort" is: S (small, days), M (medium, weeks), L (large, months).
- "Primary targets" are suggested files or areas in the repository to inspect and modify.

-----------------------------
Immediate / High-impact (implement first)
(These deliver visible improvements quickly)

1) Dialogue & Conversation System
- Why: Enables branching narratives, more meaningful quests, NPCs and choices — immediate player-perceived depth.
- Priority: 1
- Effort: M
- Dependencies: Quests, Town/NPC definitions, Journal, SaveGameManager
- Primary targets: create new `Systems\DialogueManager.cs`, add `Data\Dialogues\*.json`, hook into `Quests\Quest.cs`, `Town\Town.cs` NPC interactions
- First steps:
  - Define a JSON schema for dialogues (nodes, choices, conditions, actions, quest hooks).
  - Implement `DialogueManager` to load JSON and run a node-by-node engine with condition checks and action callbacks.
  - Add a simple example dialogue for one NPC in `TownSections` and a unit test.
- Acceptance criteria: Playable branching conversation that can start/complete a quest and persists state in saves.

2) A* Pathfinding & Movement Integration
- Why: Improves enemy navigation, encounter design, and immersion (no more dumb wandering).
- Priority: 2
- Effort: M
- Dependencies: World map/grid representation, EnemyAI, FogOfWarMap/Map
- Primary targets: create `Systems\Pathfinding.cs`, modify `Combat\EnemyAI.cs` to request paths, update `World\Map.cs` or `World\Area.cs` to expose walkable grid
- First steps:
  - Implement grid-based A* (start with small API: GetPath(start, goal)).
  - Replace simple move code in `EnemyAI` with path requests and follow logic.
- Acceptance criteria: Enemies navigate around obstacles and reach targets reliably in tests.

3) Dialogue-linked Quests & Quest Choices
- Why: Adds branching quest outcomes and ties narrative to rewards.
- Priority: 3
- Effort: S-M
- Dependencies: Dialogue system, Quests\Quest.cs, Journal
- Primary targets: `Quests\Quest.cs`, `Quests\Journal.cs`, Dialogue action hooks
- First steps: Add quest state transitions triggered by dialogue actions; persist in save.
- Acceptance criteria: Dialogue decision modifies quest state and journal entries.

4) Save System Versioning & Cloud Hooks
- Why: Stability across releases and better player retention (cross-device saves).
- Priority: 4
- Effort: S-M
- Dependencies: SaveGameManager, Program/Title flow, Tests
- Primary targets: `Systems\SaveGameManager.cs`, `Systems\UpdateChecker.cs` (for migration notices)
- First steps: Add a top-level save format version and migration stub; expose hooks to call cloud upload/download (pluggable, no provider required initially).
- Acceptance criteria: Save files include version; loading older version triggers migration path (stubbed) without crashing.

-----------------------------
Medium-term (deliver in next milestones)

5) Behavior Tree Framework for AI
- Why: Complex, reusable enemy behaviors, group tactics, better combat challenge.
- Priority: 5
- Effort: L
- Dependencies: EnemyAI, Pathfinding, Combat systems
- Primary targets: `Combat\AI\BehaviorTree/*` (new folder), refactor `Combat\EnemyAI.cs`
- First steps: Implement basic BT nodes (sequence, selector, condition, action); convert a simple enemy to use BT.
- Acceptance criteria: At least two enemy types using BT behaviors (patrol, engage, retreat).

6) Dialogue / Cutscene Sequencer (Scripted Events)
- Why: Creates cinematic moments and scripted story beats.
- Priority: 6
- Effort: M
- Dependencies: DialogueManager, VisualEffects, Camera (if any), Town/World systems
- Primary targets: `Systems\CutsceneManager.cs` (new), `Systems\DialogueManager.cs`
- First steps: Implement timeline of actions (move NPC, play dialogue, give item, change camera) serialized as JSON.
- Acceptance criteria: Play a scripted cutscene that moves actors, shows dialogue and modifies world state.

7) Data-Driven Content (Loot tables, Encounters, Items)
- Why: Enables easier tuning, balancing and future mod support.
- Priority: 7
- Effort: M
- Dependencies: LootTable, MobFactory, Items, WorldGenerator, DungeonGenerator
- Primary targets: `Data\LootTables\*.json`, `Combat\LootTable.cs`, `World\DungeonGenerator.cs`
- First steps: Extract hard-coded tables into JSON and a loader; wire loader into generation systems.
- Acceptance criteria: Content changes via JSON produce deterministic, expected behavior without recompiling.

8) Localization & Accessibility Basics
- Why: Expands audience and improves UX (subtitles, color options).
- Priority: 8
- Effort: S-M
- Dependencies: UI/VisualEffects, Dialogue strings, Titles
- Primary targets: `Systems\Localization.cs`, `Data\Localization\en.json` etc., `VisualEffects.cs`
- First steps: Replace raw strings in dialogue/examples with localization keys and implement a simple loader.
- Acceptance criteria: Switch language at runtime for test keys; dialogue displays localized strings.

-----------------------------
Long-term / Competitive Differentiators

9) Networking: Dedicated Server / Matchmaking / Authoritative Model
- Why: Professional multiplayer experience with reduced cheating and stable sessions.
- Priority: 9
- Effort: L (multiple teams)
- Dependencies: Systems\Multiplayer*, NetworkManager, MultiplayerSessionManager
- Primary targets: `Systems\NetworkMultiplayerManager.cs`, `Systems\MultiplayerSessionManager.cs`
- First steps: Design authoritative server API and message flows; separate client/server logic; implement basic server simulation locally.
- Acceptance criteria: Server authoritatively simulates combat for a test multiplayer session.

10) Player-driven Economy & Marketplace
- Why: Long-term retention and emergent gameplay.
- Priority: 10
- Effort: L
- Dependencies: Bank, Shops, Trading, Inventory, Multiplayer systems
- Primary targets: `Systems\Marketplace.cs` (new), `Systems\Trading.cs`
- First steps: Create marketplace service with order book, taxation and simple UI; simulate transactions in unit tests.
- Acceptance criteria: Players can list/buy/sell items, and trades persist across sessions.

11) Modding & Scripting Support
- Why: Community content extends longevity and attracts players.
- Priority: 11
- Effort: L
- Dependencies: Data-driven content, security model, documentation
- Primary targets: `Systems\ScriptingHost.cs` (Lua/C# sandbox), `Data\Mods\` structure
- First steps: Decide on script engine (Lua or C# scripting sandbox), implement safe loader and example mod.
- Acceptance criteria: One working mod that changes a loot table and spawns an NPC.

12) Analytics & Live Ops Hooks
- Why: Data-driven balancing and live improvements.
- Priority: 12
- Effort: M
- Dependencies: SaveGameManager, progression, multiplayer
- Primary targets: `Systems\Telemetry.cs`, analytics event points in progression and combat
- First steps: Add lightweight telemetry event interface and examples; allow pluggable sinks.
- Acceptance criteria: Events logged locally and format defined for sending to a backend.

-----------------------------
Tests & Tooling (parallel work)
- Expand unit tests for core systems (Inventory, SaveGameManager, Playerleveling, Combat calculations).
- Add integration tests for Dialogue → Quest → Save flow and for Pathfinding → EnemyAI path following.
- Add a simple CI YAML to run tests automatically.

-----------------------------
Suggested Implementation Order (linear, by iteration)
1) Dialogue system + JSON schema + one example NPC (Immediate)
2) Dialogue → Quest integration and save persistence (Immediate)
3) A* Pathfinding + EnemyAI integration (Immediate)
4) Save versioning & cloud-save hooks (Immediate)
5) Extract key data to JSON (loot, encounters) (Medium)
6) Basic Behavior Trees for complex AI (Medium)
7) Cutscene sequencer (Medium)
8) Localization + accessibility (Medium)
9) Networking modernization (Dedicated server design) (Long)
10) Marketplace/economy & modding (Long)
11) Telemetry and live-ops (Long)

-----------------------------
Estimation guidance
- Use small vertical slices that include content + UI + persistence + tests.
- Deliver each feature behind a feature flag where possible.

-----------------------------
Next recommended immediate task for me to implement (choose one)
- Create starter `Systems\DialogueManager.cs` + `Data\Dialogues\example.json` and wire one NPC in `TownSections\CentralSquare.cs` to open the dialogue.
- OR create starter `Systems\Pathfinding.cs` and update `Combat\EnemyAI.cs` to request a path.

If you want me to begin, pick one of the two immediate tasks above and I will create the initial files and a unit test.

