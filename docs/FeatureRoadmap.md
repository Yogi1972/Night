Game Feature Roadmap — Phased Plan & Progress Tracking

Purpose
- Provide a clear, phase-based plan for implementing major systems and keep an explicit progress log so the roadmap can be updated as work completes.

How to use this document
- Work by phase (Phase 1 → Phase 2 → Phase 3). After completing or partially completing an item, update the phase status below with Date, Author and short notes.
- "Effort" is: S (small, days), M (medium, weeks), L (large, months).

-----------------------------
Phase 1 — Core Systems (high impact)
(Implement these first — deliver visible gameplay improvements)

1. Dialogue & Conversation System
- Why: Enables branching narratives and meaningful quests.
- Effort: M
- Dependencies: Quests, NPCs, Journal, SaveGameManager
- Primary targets: `Systems/GameServices.cs` (shared services), `Systems/DialogueManager.cs`, `Data/Dialogues/*.json`, `TownSections/*`
- First steps:
  - Define a compact JSON schema for dialogues (nodes, choices, conditions, actions).
  - Implement `DialogueManager` to load JSON and run conversations with action callbacks.
  - Wire one NPC (example) via `TownSections/CentralSquare.cs` to start the example dialogue.

2. Dialogue-linked Quests & Quest Choices
- Why: Tie dialogue choices to quest state and branching outcomes.
- Effort: S-M
- Dependencies: Dialogue system, `Quests/Quest.cs`, `Quests/Journal.cs`
- First steps: Add dialogue action hooks such as `OfferQuest:` / `AcceptQuest:` and ensure journal persistence.

3. A* Pathfinding & Movement Integration
- Why: Improve enemy navigation and encounters.
- Effort: M
- Dependencies: World grid representation, EnemyAI
- Primary targets: `Systems/Pathfinding.cs`, `Combat/EnemyAI.cs`
- First steps: Implement grid-based A* (API: `GetPath(start, goal, grid)`) and adapt `EnemyAI` to request paths.

4. Save System Versioning & Migration Hooks
- Why: Provide stable upgrades and safe save migrations.
- Effort: S-M
- Dependencies: SaveGameManager, Options, TitleScreen flows
- First steps: Add `SaveFile.Version` and simple migration stubs to handle older formats.

-----------------------------
Phase 2 — Systems & Content (medium-term)

5. Behavior Tree Framework for AI (L)
6. Dialogue / Cutscene Sequencer (M)
7. Data-Driven Content (Loot, Encounters) (M)
8. Localization & Accessibility (S-M)

-----------------------------
Phase 3 — Long-term / Differentiators

9. Networking: Dedicated Server / Matchmaking (L)
10. Player-driven Economy & Marketplace (L)
11. Modding & Scripting Support (L)
12. Analytics & Live Ops Hooks (M)

-----------------------------
Phase Status & Progress Tracking
- Update this section after completing or making progress on an item. Add short entries with Date, Author and Notes.

Phase 1 — Core Systems (status)
- Dialogue & Conversation System — Done (2026-03-15) — Implemented `Systems/DialogueManager.cs`, `Data/Dialogues/example.json`, example wiring in `TownSections/CentralSquare.cs`.
- Dialogue-linked Quests — Done (2026-03-15) — Dialogue actions `AcceptQuest:` and `OfferQuest:` implemented; `NPCManager.FindQuestByName` and `RemoveQuestByName` added; `Journal` persistence extended in `Systems/Options.cs`.
- A* Pathfinding — Done (2026-03-15) — `Systems/Pathfinding.cs` added and `Combat/EnemyAI.cs` updated to request paths.
- Save Versioning & Migration — Partial (2026-03-15) — Save/Load extended to persist minimal quest lists; top-level version field added to DTOs (migration/stubs remain).

Phase 2 & 3 (status)
- (Pending) Behavior Tree Framework — Not started
- (Pending) Cutscene Sequencer — Not started

Notes / Next Steps
- Wire a global shared `NPCManager` at startup (added `Systems/GameServices.cs`; `Program.Main` now assigns `GameServices.NPCManager = new NPCManager()`; update other local constructions to use `GameServices.NPCManager`).
- Remove accidental large commit artifacts (zip) from repo to reduce bloat.
- Keep this file updated: when a task is completed, add a short line to "Phase Status & Progress Tracking" with date and one-line summary.

-----------------------------
Tests & Tooling
- Expand unit tests for core systems (Inventory, SaveGameManager, Dialogue→Quest→Save flow, Pathfinding→EnemyAI).
- Add a simple CI pipeline to run tests automatically.

-----------------------------
Suggested Implementation Order (by phase)
1) Complete remaining Phase 1 items and stabilize save migrations.
2) Begin Phase 2 systems in prioritized order (BT, Cutscenes, Data-driven content).
3) Tackling Phase 3 items once core online systems and tooling are in place.

-----------------------------
If you want me to begin, pick one of the ready Phase 1 tasks that remains (for example: finish save migration stubs, or replace remaining `new NPCManager()` usages with `GameServices.NPCManager`).

