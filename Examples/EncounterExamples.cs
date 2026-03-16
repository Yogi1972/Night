using Night.Characters;
using System;
using System.Collections.Generic;

namespace Rpg_Dungeon
{
    /// <summary>
    /// Examples demonstrating how to use the Encounter class in various scenarios
    /// </summary>
    internal static class EncounterExamples
    {
        #region Basic Usage Examples

        /// <summary>
        /// Example 1: Generate a basic balanced encounter
        /// </summary>
        public static void Example_BasicEncounter(List<Character> party)
        {
            Console.WriteLine("=== Basic Encounter Example ===");

            // Create and generate a normal difficulty encounter
            var encounter = new Encounter();
            encounter.GenerateEncounter(party, EncounterDifficulty.Normal);

            // The encounter automatically scales enemy count based on party size
            // For a party of 4, you'll typically get 3-4 enemies
            // For a party of 2, you'll get 2 enemies

            bool victory = encounter.StartEncounter(party);

            if (victory)
            {
                Console.WriteLine("Your party emerged victorious!");
            }
            else
            {
                Console.WriteLine("Your party needs to regroup...");
            }
        }

        /// <summary>
        /// Example 2: Use the quick encounter method
        /// </summary>
        public static void Example_QuickEncounter(List<Character> party)
        {
            Console.WriteLine("=== Quick Encounter Example ===");

            // One-line encounter generation and execution
            bool victory = Encounter.QuickEncounter(party, EncounterDifficulty.Hard);

            Console.WriteLine(victory ? "Victory!" : "Defeat!");
        }

        /// <summary>
        /// Example 3: Generate encounters with different difficulties
        /// </summary>
        public static void Example_DifficultyScaling(List<Character> party)
        {
            Console.WriteLine("=== Difficulty Scaling Example ===");

            // Easy encounter - fewer enemies, lower level
            var easyEncounter = new Encounter();
            easyEncounter.GenerateEncounter(party, EncounterDifficulty.Easy);
            Console.WriteLine($"Easy: {easyEncounter.TotalEnemyCount} enemies");

            // Hard encounter - more enemies or higher level
            var hardEncounter = new Encounter();
            hardEncounter.GenerateEncounter(party, EncounterDifficulty.Hard);
            Console.WriteLine($"Hard: {hardEncounter.TotalEnemyCount} enemies");

            // Elite encounter - fewer but much stronger enemies
            var eliteEncounter = new Encounter();
            eliteEncounter.GenerateEncounter(party, EncounterDifficulty.Elite);
            Console.WriteLine($"Elite: {eliteEncounter.TotalEnemyCount} enemies");
        }

        /// <summary>
        /// Example 4: Generate a boss encounter
        /// </summary>
        public static void Example_BossEncounter(List<Character> party)
        {
            Console.WriteLine("=== Boss Encounter Example ===");

            var encounter = new Encounter();
            encounter.GenerateBossEncounter(party, "Ancient Dragon");

            // Boss encounters have 1 boss + minions based on party size
            // Party of 4 = 1 boss + 2 minions
            // Party of 2 = 1 boss + 1 minion

            bool victory = encounter.StartEncounter(party);

            if (victory)
            {
                Console.WriteLine("🏆 Boss defeated! Epic victory!");
            }
        }

        /// <summary>
        /// Example 5: Random encounter for exploration
        /// </summary>
        public static void Example_RandomEncounter(List<Character> party, int areaLevel)
        {
            Console.WriteLine("=== Random Encounter Example ===");

            // Random chance of easy/normal/hard based on area
            bool victory = Encounter.RandomEncounter(party, areaLevel);

            Console.WriteLine(victory ? "Encounter survived!" : "Party defeated!");
        }

        #endregion

        #region Integration Examples

        /// <summary>
        /// Example 6: Using encounters in dungeon exploration
        /// </summary>
        public static void Example_DungeonRoomEncounter(List<Character> party, int dungeonLevel, RoomType roomType)
        {
            Console.WriteLine("=== Dungeon Room Encounter Example ===");

            var encounter = new Encounter();

            switch (roomType)
            {
                case RoomType.Combat:
                    encounter.GenerateEncounter(party, EncounterDifficulty.Normal);
                    break;

                case RoomType.Elite:
                    encounter.GenerateEncounter(party, EncounterDifficulty.Elite);
                    break;

                case RoomType.Boss:
                    encounter.GenerateBossEncounter(party);
                    break;

                default:
                    Console.WriteLine("No combat in this room.");
                    return;
            }

            bool victory = encounter.StartEncounter(party);

            if (!victory)
            {
                Console.WriteLine("Retreat to safety!");
            }
        }

        /// <summary>
        /// Example 7: Scaling encounters for party composition
        /// </summary>
        public static void Example_PartyCompositionScaling(List<Character> party)
        {
            Console.WriteLine("=== Party Composition Example ===");

            // The Encounter class automatically analyzes:
            // - Party size (1-4 members)
            // - Average party level
            // - Total party power (stats + equipment)

            Console.WriteLine($"Party has {party.Count} member(s)");

            if (party.Count == 1)
            {
                // Solo player: Encounter generates 1-2 enemies at appropriate level
                var encounter = new Encounter();
                encounter.GenerateEncounter(party, EncounterDifficulty.Normal);
                Console.WriteLine($"Solo encounter: {encounter.TotalEnemyCount} enemy");
            }
            else if (party.Count == 4)
            {
                // Full party: Encounter generates 3-5 enemies for more dynamic combat
                var encounter = new Encounter();
                encounter.GenerateEncounter(party, EncounterDifficulty.Normal);
                Console.WriteLine($"Full party encounter: {encounter.TotalEnemyCount} enemies");
            }
        }

        /// <summary>
        /// Example 8: Progressive difficulty encounters
        /// </summary>
        public static void Example_ProgressiveDifficulty(List<Character> party)
        {
            Console.WriteLine("=== Progressive Difficulty Example ===");

            // Warm-up encounter
            Console.WriteLine("Wave 1 - Easy:");
            Encounter.QuickEncounter(party, EncounterDifficulty.Easy);

            // Give party a moment
            Console.WriteLine("\nPress Enter for next wave...");
            Console.ReadLine();

            // Standard encounter
            Console.WriteLine("Wave 2 - Normal:");
            Encounter.QuickEncounter(party, EncounterDifficulty.Normal);

            Console.WriteLine("\nPress Enter for final wave...");
            Console.ReadLine();

            // Challenging final wave
            Console.WriteLine("Wave 3 - Hard:");
            Encounter.QuickEncounter(party, EncounterDifficulty.Hard);
        }

        #endregion

        #region Usage Guide

        public static void ShowUsageGuide()
        {
            Console.WriteLine("╔════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║              ENCOUNTER CLASS USAGE GUIDE                       ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════════╝");
            Console.WriteLine();
            Console.WriteLine("The Encounter class intelligently scales combat based on:");
            Console.WriteLine("  • Party Size (1-4 members)");
            Console.WriteLine("  • Party Average Level");
            Console.WriteLine("  • Party Total Power (stats + equipment)");
            Console.WriteLine();
            Console.WriteLine("DIFFICULTY SCALING:");
            Console.WriteLine("  Easy:   Fewer enemies (party-1), lower level (-1 to -2)");
            Console.WriteLine("  Normal: Equal enemies (~party size), same level (±1)");
            Console.WriteLine("  Hard:   More enemies (+1 to +2), higher level (+1 to +2)");
            Console.WriteLine("  Elite:  Fewer enemies (party/2), much higher level (+2 to +4)");
            Console.WriteLine("  Boss:   1 boss + minions, highest level (+3 to +5)");
            Console.WriteLine();
            Console.WriteLine("USAGE PATTERNS:");
            Console.WriteLine();
            Console.WriteLine("1. Quick Encounter (one-liner):");
            Console.WriteLine("   Encounter.QuickEncounter(party, EncounterDifficulty.Normal);");
            Console.WriteLine();
            Console.WriteLine("2. Custom Encounter:");
            Console.WriteLine("   var encounter = new Encounter();");
            Console.WriteLine("   encounter.GenerateEncounter(party, EncounterDifficulty.Hard);");
            Console.WriteLine("   bool won = encounter.StartEncounter(party);");
            Console.WriteLine();
            Console.WriteLine("3. Boss Encounter:");
            Console.WriteLine("   var encounter = new Encounter();");
            Console.WriteLine("   encounter.GenerateBossEncounter(party);");
            Console.WriteLine("   bool won = encounter.StartEncounter(party);");
            Console.WriteLine();
            Console.WriteLine("4. Random Encounter (for exploration):");
            Console.WriteLine("   Encounter.RandomEncounter(party, areaLevel);");
            Console.WriteLine();
        }

        #endregion
    }
}
