using Night.Characters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rpg_Dungeon
{
    /// <summary>
    /// Example showing how to integrate the Encounter class into the existing Dungeon system
    /// This file demonstrates best practices for replacing the old single-mob system
    /// </summary>
    internal static class EncounterIntegration
    {
        #region Dungeon Integration Example

        /// <summary>
        /// Example: Replacing HandleRoomEncounter in Dungeon.cs to use the new Encounter system
        /// </summary>
        public static bool HandleRoomEncounterNew(List<Character> party, RoomType roomType, int floorNumber)
        {
            // Skip non-combat rooms
            if (roomType == RoomType.Empty || roomType == RoomType.Treasure || roomType == RoomType.Stairs)
            {
                return true;
            }

            Console.WriteLine($"\n⚠️  Combat Encounter on Floor {floorNumber}!");

            // Create encounter based on room type
            var encounter = new Encounter();

            switch (roomType)
            {
                case RoomType.Combat:
                    // Normal combat room - balanced encounter
                    encounter.GenerateEncounter(party, EncounterDifficulty.Normal);
                    break;

                case RoomType.Elite:
                    // Elite room - tougher enemies
                    encounter.GenerateEncounter(party, EncounterDifficulty.Elite);
                    Console.WriteLine("⭐ This is an ELITE encounter! Prepare for a tough fight!");
                    break;

                case RoomType.Boss:
                    // Boss room - epic fight
                    encounter.GenerateBossEncounter(party, $"Floor {floorNumber} Guardian");
                    Console.WriteLine("👑 BOSS ROOM! This will be your greatest challenge yet!");
                    break;

                default:
                    return true;
            }

            // Execute the encounter
            bool victory = encounter.StartEncounter(party);

            if (victory)
            {
                Console.WriteLine($"✓ Room cleared!");
            }

            return victory;
        }

        #endregion

        #region Area Exploration Integration

        /// <summary>
        /// Example: Using encounters for random world map encounters
        /// </summary>
        public static void HandleAreaExploration(List<Character> party, Area area)
        {
            var rng = new Random();

            // 30% chance of encounter while exploring
            if (rng.Next(100) < 30)
            {
                Console.WriteLine($"\n🌍 An encounter occurs while exploring {area.Name}!");

                // Use area level to determine encounter difficulty
                int partyAvgLevel = party.Count > 0 ? (int)party.Average(c => c.Level) : 1;

                EncounterDifficulty difficulty;
                if (area.RecommendedLevel > partyAvgLevel + 3)
                {
                    // Area is too high level - use normal or hard
                    difficulty = EncounterDifficulty.Hard;
                }
                else if (area.RecommendedLevel < partyAvgLevel - 3)
                {
                    // Area is too low level - use easy
                    difficulty = EncounterDifficulty.Easy;
                }
                else
                {
                    // Random normal/hard mix
                    difficulty = rng.Next(2) == 0 ? EncounterDifficulty.Normal : EncounterDifficulty.Hard;
                }

                var encounter = new Encounter();
                encounter.GenerateEncounter(party, difficulty);
                bool victory = encounter.StartEncounter(party);

                if (!victory)
                {
                    Console.WriteLine("Your party retreats from the area!");
                }
            }
        }

        #endregion

        #region Quest Integration

        /// <summary>
        /// Example: Generate encounters for quest objectives
        /// </summary>
        public static bool HandleQuestCombatObjective(List<Character> party, Quest quest)
        {
            Console.WriteLine($"\n📜 Quest Combat: {quest.Name}");

            // Scale difficulty based on quest difficulty
            EncounterDifficulty encounterDifficulty = quest.Difficulty switch
            {
                QuestDifficulty.Easy => EncounterDifficulty.Easy,
                QuestDifficulty.Medium => EncounterDifficulty.Normal,
                QuestDifficulty.Hard => EncounterDifficulty.Hard,
                QuestDifficulty.Elite => EncounterDifficulty.Elite,
                _ => EncounterDifficulty.Normal
            };

            var encounter = new Encounter();
            encounter.GenerateEncounter(party, encounterDifficulty);

            return encounter.StartEncounter(party);
        }

        #endregion

        #region Bounty Integration

        /// <summary>
        /// Example: Generate encounters for bounty targets
        /// </summary>
        public static bool HandleBountyEncounter(List<Character> party, Bounty bounty)
        {
            Console.WriteLine($"\n🎯 Bounty Target: {bounty.TargetName}");
            Console.WriteLine($"Reward: {bounty.GoldReward} gold");

            // Bounties are typically boss-style encounters
            var encounter = new Encounter();

            if (bounty.Difficulty == BountyDifficulty.Legendary)
            {
                // Legendary bounties are full boss encounters
                encounter.GenerateBossEncounter(party, bounty.TargetName);
            }
            else
            {
                // Map bounty difficulty to encounter difficulty
                EncounterDifficulty encounterDifficulty = bounty.Difficulty switch
                {
                    BountyDifficulty.Common => EncounterDifficulty.Easy,
                    BountyDifficulty.Uncommon => EncounterDifficulty.Normal,
                    BountyDifficulty.Rare => EncounterDifficulty.Hard,
                    _ => EncounterDifficulty.Normal
                };

                encounter.GenerateEncounter(party, encounterDifficulty);
            }

            bool victory = encounter.StartEncounter(party);

            if (victory)
            {
                Console.WriteLine($"\n✓ Bounty completed! Claim your {bounty.GoldReward} gold reward!");
            }

            return victory;
        }

        #endregion

        #region Progressive Difficulty

        /// <summary>
        /// Example: Multi-wave encounters that get progressively harder
        /// </summary>
        public static bool HandleWaveEncounter(List<Character> party, int waveCount = 3)
        {
            Console.WriteLine($"\n🌊 WAVE ENCOUNTER: {waveCount} waves incoming!");

            for (int wave = 1; wave <= waveCount; wave++)
            {
                Console.WriteLine($"\n╔═══════════════════════════════════╗");
                Console.WriteLine($"║        WAVE {wave} of {waveCount}                ║");
                Console.WriteLine($"╚═══════════════════════════════════╝");

                // Increase difficulty each wave
                EncounterDifficulty difficulty = wave switch
                {
                    1 => EncounterDifficulty.Easy,
                    2 => EncounterDifficulty.Normal,
                    3 => EncounterDifficulty.Hard,
                    _ => EncounterDifficulty.Hard
                };

                var encounter = new Encounter();
                encounter.GenerateEncounter(party, difficulty);
                bool victory = encounter.StartEncounter(party);

                if (!victory)
                {
                    Console.WriteLine($"\n💀 Party defeated on wave {wave}!");
                    return false;
                }

                // Brief rest between waves
                if (wave < waveCount)
                {
                    Console.WriteLine("\n🛡️  Brief respite before the next wave...");
                    Console.WriteLine("Press Enter to continue...");
                    Console.ReadLine();

                    // Restore small amount of HP/Mana between waves
                    foreach (var member in party)
                    {
                        if (member.IsAlive)
                        {
                            member.Heal(member.GetTotalMaxHP() / 10);
                            member.RestoreMana(member.MaxMana / 10);
                        }
                    }
                }
            }

            Console.WriteLine("\n🏆 All waves defeated! VICTORY!");
            return true;
        }

        #endregion

        #region Adaptive Encounters

        /// <summary>
        /// Example: Encounters that adapt based on party state
        /// </summary>
        public static bool HandleAdaptiveEncounter(List<Character> party)
        {
            // Check party health
            int totalHP = 0;
            int totalMaxHP = 0;
            foreach (var member in party)
            {
                if (member.IsAlive)
                {
                    totalHP += member.Health;
                    totalMaxHP += member.GetTotalMaxHP();
                }
            }

            double healthPercent = totalMaxHP > 0 ? (double)totalHP / totalMaxHP : 1.0;

            // Adjust difficulty based on party health
            EncounterDifficulty difficulty;
            if (healthPercent < 0.4)
            {
                // Party is wounded - go easy
                difficulty = EncounterDifficulty.Easy;
                Console.WriteLine("🩹 The party is wounded. A weaker encounter appears...");
            }
            else if (healthPercent > 0.8)
            {
                // Party is healthy - challenge them
                difficulty = EncounterDifficulty.Hard;
                Console.WriteLine("💪 The party is at full strength. A challenging encounter appears!");
            }
            else
            {
                // Normal difficulty
                difficulty = EncounterDifficulty.Normal;
            }

            return Encounter.QuickEncounter(party, difficulty);
        }

        #endregion

        #region Themed Encounters

        /// <summary>
        /// Example: Create themed encounters (all undead, all beasts, etc.)
        /// Future enhancement - requires MobFactory support for type filtering
        /// </summary>
        public static void Example_ThemedEncounter(List<Character> party, string theme)
        {
            Console.WriteLine($"\n🎭 Themed Encounter: {theme}");

            // This is a conceptual example showing how you could extend the system
            // to support themed encounters once MobFactory supports enemy types

            var encounter = new Encounter();

            switch (theme.ToLower())
            {
                case "undead":
                    Console.WriteLine("💀 A horde of undead rises!");
                    // encounter.GenerateThemedEncounter(party, EnemyType.Undead, EncounterDifficulty.Normal);
                    break;

                case "beasts":
                    Console.WriteLine("🐺 Wild beasts attack!");
                    // encounter.GenerateThemedEncounter(party, EnemyType.Beast, EncounterDifficulty.Normal);
                    break;

                case "bandits":
                    Console.WriteLine("⚔️  A bandit ambush!");
                    // encounter.GenerateThemedEncounter(party, EnemyType.Humanoid, EncounterDifficulty.Normal);
                    break;

                default:
                    encounter.GenerateEncounter(party, EncounterDifficulty.Normal);
                    break;
            }

            encounter.StartEncounter(party);
        }

        #endregion
    }
}
