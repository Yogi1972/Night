# RPG Dungeon — Improvement Checklist (Phase 2.1)

This document lists pragmatic, prioritized items to improve the game. Pick one or more to implement and I can make the changes in the codebase.

1) Build + Automated Checks
   - Enable CI (GitHub Actions) to run `dotnet build` and `dotnet test` on push/PR.
   - Add Roslyn analyzers and EditorConfig to enforce coding style and catch issues.

2) Tests
   - Add unit tests for core systems: `SaveGameManager`, `GameLoopManager`, `NetworkManager`, `ErrorLogger`.
   - Add integration tests for save/load roundtrips.

3) Static Diagnostics & Logging
   - Centralize logging (use Microsoft.Extensions.Logging) and add log levels.
   - Ensure `ErrorLogger` writes structured logs and rotates files.

4) Save System Robustness
   - Validate save file schema and handle corrupted saves gracefully.
   - Add save backup and migration/versioning for future schema changes.

5) Multiplayer Stability
   - Add retry/backoff and timeouts to network connections.
   - Add connection verification, NAT/firewall instructions and port configuration.
   - Add unit tests and simulated network tests.

6) UX & Input Validation
   - Validate all Console.ReadLine inputs robustly; avoid null/empty exceptions.
   - Add clearer menus and keyboard shortcuts.

7) Performance & Profiling
   - Add profiling hooks and benchmarks for hot paths (pathfinding, dungeon gen, combat loop).
   - Use a profiler to identify GC or CPU hotspots.

8) Architecture & Code Quality
   - Split systems into smaller classes with clear responsibilities (single responsibility).
   - Introduce dependency injection for testability (use `Microsoft.Extensions.DependencyInjection`).

9) Documentation
   - Add `docs/Phase-2.1.md` with roadmap and required artifacts.
   - Add README sections: How to build, run, test, and contribute.

10) Packaging & Distribution
   - Add a self-contained publish profile and simple installer instructions.

Suggested first concrete tasks (pick one):
  A) Add a `docs/Phase-2.1.md` with this checklist and create GitHub Actions workflow to run `dotnet build`.
  B) Add basic unit tests project and a sample test for `SaveGameManager` load failure handling.
  C) Add structured logging support and replace `Console.WriteLine` usages in `ErrorLogger` with `ILogger`.

Tell me which item (or letter) to start with and I'll implement it.