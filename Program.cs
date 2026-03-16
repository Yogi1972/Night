using System;
using System.IO;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Rpg_Dungeon.Systems;

namespace Rpg_Dungeon
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            // Support a special CLI flag for local testing of the updater script generation
            if (args != null && args.Length > 0 && args[0] == "--simulate-updater")
            {
                try
                {
                    Console.WriteLine("--- Updater script simulation ---");
                    var script = UpdateChecker.SimulateAndGetScript();
                    Console.WriteLine(script);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Simulation failed: {ex.Message}");
                }
                return;
            }
            // Self-test: verify world generation and pathfinding determinism with a seed
            if (args != null && args.Length > 0 && args[0] == "--selftest-map")
            {
                try
                {
                    int? seed = null;
                    if (args.Length > 1 && !string.IsNullOrWhiteSpace(args[1])) seed = WorldGenerator.StringToSeed(args[1]);
                    Console.WriteLine("--- Self-test: Map + Pathfinding ---");
                    Console.WriteLine($"Seed: {(seed.HasValue ? WorldGenerator.SeedToString(seed.Value) : "(random)")}");

                    // World generation
                    var wg = new WorldGenerator(seed);
                    var towns = wg.GenerateMajorTowns();
                    var settlements = wg.GenerateSettlements();
                    var camps = wg.GenerateCamps();

                    Console.WriteLine($"Major towns ({towns.Count}): {string.Join(", ", towns.Select(t => t.Name).Take(6))}");
                    Console.WriteLine($"Settlements ({settlements.Count}): {string.Join(", ", settlements.Select(s => s.Name).Take(6))}");
                    Console.WriteLine($"Camps ({camps.Count}): {string.Join(", ", camps.Select(c => c.Name).Take(6))}");

                    // Fog of war basic test
                    var fog = new FogOfWarMap(seed);
                    fog.SetCurrentLocation(towns.FirstOrDefault()?.Name ?? "Havenbrook");
                    Console.WriteLine($"Discovered count after center reveal: {fog.GetDiscoveredCount()} / {fog.GetTotalLocations()}");

                    // Pathfinding deterministic test: build a small obstacle grid seeded
                    int rows = 10, cols = 10;
                    bool[,] walk = new bool[rows, cols];
                    var rng = new Random(seed ?? 12345);
                    for (int y = 0; y < rows; y++)
                    for (int x = 0; x < cols; x++)
                        walk[y, x] = rng.Next(100) < 75; // 75% walkable

                    var start = new Pathfinding.GridPoint(0, 0);
                    var goal = new Pathfinding.GridPoint(cols - 1, rows - 1);
                    var path1 = Pathfinding.GetPath(start, goal, walk);
                    Console.WriteLine($"Path length: {path1.Count}");
                    if (path1.Count > 0)
                    {
                        Console.WriteLine("Path sample: " + string.Join(" -> ", path1.Take(10).Select(p => p.ToString())));
                    }

                    // Repeat to verify determinism
                    var rng2 = new Random(seed ?? 12345);
                    bool[,] walk2 = new bool[rows, cols];
                    for (int y = 0; y < rows; y++)
                    for (int x = 0; x < cols; x++)
                        walk2[y, x] = rng2.Next(100) < 75;
                    var path2 = Pathfinding.GetPath(start, goal, walk2);
                    Console.WriteLine($"Repeat path length: {path2.Count}");

                    Console.WriteLine(path1.Count == path2.Count && path1.SequenceEqual(path2) ? "Pathfinding deterministic: OK" : "Pathfinding deterministic: MISMATCH");

                    Console.WriteLine("Self-test complete. Press Enter to exit...");
                    Console.ReadLine();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Self-test failed: {ex.Message}");
                }
                return;
            }

            // CLI: test hex map rendering with optional seed
            // CLI: export map for Unity import
            if (args != null && args.Length > 0 && args[0] == "--export-unity")
            {
                try
                {
                    int? seed = null;
                    if (args.Length > 1 && !string.IsNullOrWhiteSpace(args[1])) seed = WorldGenerator.StringToSeed(args[1]);
                    var fog = new FogOfWarMap(seed);
                    var outDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "unityrpg");
                    fog.ExportToUnity(outDir);
                    Console.WriteLine($"Exported Unity data to: {outDir}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Unity export failed: {ex.Message}");
                }
                return;
            }
            // --test-hexmap removed (hex map functionality deprecated)
            try
            {
                Console.WriteLine("PROGRAM STARTING...");
                Console.WriteLine($"Version: {VersionControl.FullVersion}");
                Console.WriteLine("Please wait...");
                Thread.Sleep(500);

                try
                {
                    Console.OutputEncoding = System.Text.Encoding.UTF8;
                    Console.WriteLine("UTF-8 encoding set.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"UTF-8 failed: {ex.Message}");
                    ErrorLogger.LogWarning($"UTF-8 encoding failed: {ex.Message}", "Non-critical - continuing with default encoding");
                }

                // Initialize shared services
                GameServices.NPCManager = new NPCManager();

                // Check for updates in the background (fire-and-forget)
                _ = CheckForUpdatesAsync();

                Console.WriteLine("Showing title screen...");
                Thread.Sleep(500);

                TitleScreenManager.Show();
            }
            catch (Exception ex)
            {
                Console.WriteLine("\n╔══════════════════════════════════════════════════════════════════╗");
                Console.WriteLine("║                    CRITICAL ERROR                                ║");
                Console.WriteLine("╚══════════════════════════════════════════════════════════════════╝");
                Console.WriteLine($"\n❌ An unexpected error occurred: {ex.Message}");

                Console.WriteLine($"\n📋 Error Type: {ex.GetType().Name}");

                string logFilePath = ErrorLogger.LogCriticalError(ex, "Unhandled exception in Main()");

                if (!string.IsNullOrEmpty(logFilePath))
                {
                    Console.WriteLine($"\n💾 Error details saved to: {logFilePath}");
                    Console.WriteLine("\n📧 To report this error:");
                    Console.WriteLine($"   1. Navigate to: {ErrorLogger.GetLogDirectory()}");
                    Console.WriteLine("   2. Send the error log file to the developer");
                    Console.WriteLine("\n💡 You can also press 'O' to open the error log folder now.");
                }

                Console.WriteLine("\n📋 Stack Trace:");
                Console.WriteLine(ex.StackTrace);
                Console.WriteLine("\nPress 'O' to open error logs folder, or Enter to exit...");

                var key = Console.ReadKey(true);
                if (key.KeyChar == 'O' || key.KeyChar == 'o')
                {
                    ErrorLogger.OpenLogDirectory();
                    Console.WriteLine("\nOpening error logs folder...");
                    Thread.Sleep(1000);
                }

                Environment.Exit(1);
            }
        }

        /// <summary>
        /// Checks for updates asynchronously in the background
        /// </summary>
        private static async Task CheckForUpdatesAsync()
        {
            try
            {
                Console.WriteLine("🔍 Checking for updates...");
                var updateInfo = await UpdateChecker.CheckForUpdatesAsync();

                if (updateInfo == null)
                {
                    Console.WriteLine("⚠️  Unable to check for updates (no releases published yet or network issue)");
                }
                else if (UpdateChecker.IsNewerVersion(updateInfo))
                {
                    Console.WriteLine("╔══════════════════════════════════════════════════════════════════╗");
                    Console.WriteLine("║               🎉 NEW UPDATE AVAILABLE! 🎉                       ║");
                    Console.WriteLine("╚══════════════════════════════════════════════════════════════════╝");
                    Console.WriteLine($"📦 Latest Version: v{updateInfo.MajorVersion}.{updateInfo.MinorVersion}.{updateInfo.PatchVersion}");
                    Console.WriteLine($"📌 Your Version: v{VersionControl.FullVersion}");
                    Console.WriteLine($"💡 Visit {VersionControl.GitHubReleaseUrl} to download!");
                    Console.WriteLine();
                    Thread.Sleep(2000); // Give user time to see the message
                }
                else
                {
                    Console.WriteLine("✅ No updates available - You're running the latest version!");
                }
            }
            catch (Exception ex)
            {
                // Silently log update check failures - non-critical
                ErrorLogger.LogWarning($"Update check failed: {ex.Message}", "Non-critical - continuing without update check");
                Console.WriteLine("⚠️  Update check failed (continuing anyway)");
            }
        }
    }
}
