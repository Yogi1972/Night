using Night.Characters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rpg_Dungeon
{
    internal static class Playerleveling
    {
        #region Constants and Fields

        public const int MaxLevel = 100;// Maximum level a character can reach in the game

        private static readonly int[] _xpTable = GenerateXPTable();// Pre-generated XP table for levels 1 to MaxLevel, where each index corresponds to the XP required for that level (index 0 = Level 1, index 1 = Level 2, etc.)

        private static readonly Dictionary<string, LevelGains> _classGains = new Dictionary<string, LevelGains>
        {
            // Base gains for each class (Health, Mana, Stamina, Strength, Agility, Intelligence, ArmorRating) these are the 7 overloaded parameters for the LevelGains constructor, with the last one (ArmorRating) being optional and defaulting to 0 if not provided.
            { "Warrior", new LevelGains(15, 0, 10, 3, 2, 1, 1) },
            { "Mage", new LevelGains(8, 10, 0, 1, 2, 4, 0) },
            { "Rogue", new LevelGains(10, 0, 8, 2, 4, 2, 0) },
            { "Priest", new LevelGains(12, 8, 0, 1, 2, 3, 1) }
        };

        #endregion

        #region XP Table Generation

        private static int[] GenerateXPTable()
        {
            var table = new int[MaxLevel];// Level 1 starts at 0 XP
            table[0] = 0;// Level 1 requires 0 XP this is an array of 100 elements, indexed from 0 to 99, where table[0] corresponds to Level 1, table[1] to Level 2, and so on up to table[99] for Level 100.

            const double baseXP = 100.0;// Base XP for level 2
            const double exponent = 1.5;// Exponential growth factor

            for (int i = 1; i < MaxLevel; i++)
            {
                // Calculate XP for level i+1 based on the formula: XP = baseXP * (level ^ exponent)
                double levelMultiplier = Math.Pow(i, exponent);
                table[i] = (int)(baseXP * levelMultiplier);

                if (i >= 90)// Apply additional multipliers for higher levels to create a steeper curve
                {
                    table[i] = (int)(table[i] * 1.5 * 1.3 * 1.2);
                }
                else if (i >= 75)// Apply a smaller multiplier for mid-high levels
                {
                    table[i] = (int)(table[i] * 1.3 * 1.2);
                }
                else if (i >= 50)// Apply a moderate multiplier for mid levels
                {
                    table[i] = (int)(table[i] * 1.2);
                }
            }
            // Ensure the XP for level 100 is significantly higher to create a challenging endgame
            return table;
        }

        #endregion

        #region Level and XP Methods

        public static int GetXPForLevel(int level)// This method retrieves the XP required for a specific level. It checks if the level is within the valid range (1 to MaxLevel) and returns the corresponding XP from the _xpTable. If the level is out of range, it returns 0.
        {
            if (level < 1 || level > MaxLevel) return 0;
            return _xpTable[level - 1];
        }

        public static int GetXPToNextLevel(int currentLevel)// This method calculates the XP needed to reach the next level from the current level.
                                                            // It checks if the current level is at or above the maximum level and returns 0 if so. Otherwise, it calls GetXPForLevel with the next level (currentLevel + 1) to get the required XP for that level.
        {
            if (currentLevel >= MaxLevel) return 0;
            return GetXPForLevel(currentLevel + 1);
        }

        public static void DisplayLevelProgress(Character character)// This method displays the character's current level, XP, and progress towards the next level.
                                                                    // It retrieves the current XP and level from the character, calculates the XP needed for the next level, and gets the title for the current level.
                                                                    // It then prints this information to the console, including a progress percentage if the character is not at max level.
        {
            int currentXP = character.Experience;// Get the current XP from the character
            int currentLevel = character.Level;// Get the current level from the character
            int currentLevelXP = GetXPForLevel(currentLevel);// Get the XP required for the current level
            int nextLevelXP = GetXPToNextLevel(currentLevel);// Calculate the XP needed for the next level
            string title = GetLevelTitle(currentLevel);// Get the title for the current level

            Console.WriteLine($"\n--- {character.Name} Level Progress ---");// Print a header with the character's name
            VisualEffects.WriteLineColored($"Level: {currentLevel} ({title})", ConsoleColor.Cyan);// Print the current level and title

            if (currentLevel < MaxLevel)// If the character is not at max level, calculate and display the progress percentage towards the next level
            {
                int xpInCurrentLevel = currentXP - currentLevelXP;
                int xpNeededForNextLevel = nextLevelXP - currentLevelXP;

                VisualEffects.DrawProgressBarLine(xpInCurrentLevel, xpNeededForNextLevel, 30, "XP");
            }
            else// If the character is at max level, indicate that the max level has been reached
            {
                VisualEffects.WriteLineColored("⭐ MAX LEVEL REACHED! ⭐", ConsoleColor.Magenta);
            }
        }

        #endregion

        #region Stat Gains

        public static LevelGains GetStatGains(Character character)// This method retrieves the base stat gains for a character based on their class.
                                                                  // It uses the character's type name to look up the corresponding LevelGains in the _classGains dictionary.
                                                                  // If the character's class is not found in the dictionary, it returns a default set of gains.
        {
            string className = character.GetType().Name;// Get the class name of the character (e.g., "Warrior", "Mage", etc.)
            if (_classGains.TryGetValue(className, out var gains))// Try to get the LevelGains for the character's class from the _classGains dictionary
                return gains;// If found, return the corresponding LevelGains

            return new LevelGains(10, 5, 5, 2, 2, 2, 0);// If the class is not found in the dictionary, return a default set of gains (this can be adjusted as needed)
            // The default gains provide a balanced increase in stats for classes that are not explicitly defined in the _classGains dictionary.
            // This ensures that all character types can still level up and gain stats, even if they don't have a specific class defined.
            // Level gains needs health, mana, stamina, strength, agility, intelligence, and armor rating to be returned as a LevelGains object.
        }

        public static LevelGains GetStatGainsForLevel(Character character, int level)// This method calculates the stat gains for a character at a specific level, applying additional bonuses at certain level milestones (every 10 and 25 levels).
                                                                                     // It first retrieves the base stat gains for the character's class using GetStatGains. Then, it checks if the level is a multiple of 25 or 10 and applies additional bonuses accordingly.
                                                                                     // Finally, it returns a new LevelGains object with the total stat gains for that level.
        {
            var baseGains = GetStatGains(character);// Get the base stat gains for the character's class

            if (level % 25 == 0)// If the level is a multiple of 25, apply significant bonuses to the stat gains
            {
                return new LevelGains(
                    baseGains.Health + 15,
                    baseGains.Mana + 10,
                    baseGains.Stamina + 10,
                    baseGains.Strength + 3,
                    baseGains.Agility + 3,
                    baseGains.Intelligence + 3,
                    baseGains.ArmorRating + 2
                );
            }

            if (level % 10 == 0)// If the level is a multiple of 10 (but not 25), apply moderate bonuses to the stat gains
            {
                return new LevelGains(
                    baseGains.Health + 5,
                    baseGains.Mana + 3,
                    baseGains.Stamina + 5,
                    baseGains.Strength + 1,
                    baseGains.Agility + 1,
                    baseGains.Intelligence + 1,
                    baseGains.ArmorRating + 1
                );
            }

            return baseGains;
        }

        #endregion

        #region Level Titles

        public static string GetLevelTitle(int level)// This method returns a title for a given level based on predefined ranges. It uses a switch expression to determine the appropriate title for the level.
        {
            return level switch // The switch expression checks the level against various ranges and returns a corresponding title. If the level does not match any of the defined ranges, it returns "Unknown".
            {
                >= 1 and <= 10 => "Novice",
                >= 11 and <= 20 => "Apprentice",
                >= 21 and <= 30 => "Journeyman",
                >= 31 and <= 40 => "Adept",
                >= 41 and <= 50 => "Expert",
                >= 51 and <= 60 => "Master",
                >= 61 and <= 70 => "GrandMaster",
                >= 71 and <= 80 => "Champion",
                >= 81 and <= 90 => "Legend",
                >= 91 and <= 99 => "Mythic",
                100 => "Ascended",
                _ => "Unknown"
            };
        }

        #endregion

        #region Mob Calculations

        public static int CalculateXPReward(Mob mob, int partySize)//   This method calculates the XP reward for defeating a mob based on its stats and the size of the party.
                                                                   //   It takes into account the mob's health, strength, agility, intelligence, and level, as well as a multiplier based on the party size to ensure that XP rewards are balanced for different group sizes.
        {
            int baseXP = Math.Max(5, mob.Health / 2);
            baseXP += mob.Strength + mob.Agility + mob.Intelligence;

            baseXP = (int)(baseXP * (1.0 + (mob.Level - 1) * 0.1));

            double partySizeMultiplier = Math.Max(0.5, 1.0 - ((partySize - 1) * 0.15));
            int finalXP = (int)(baseXP * partySizeMultiplier);

            return Math.Max(1, finalXP);
        }

        public static int CalculateMobLevel(List<Character> party, int dungeonLevel)// This method calculates the appropriate level for a mob based on the average level of the alive characters in the party and the current dungeon level.
                                                                                    // It takes into account the levels of the alive party members, applies a base adjustment based on the dungeon level, and
                                                                                    // adds additional adjustments based on the party size to ensure that mobs are appropriately challenging for the players.
        {
            if (party == null || party.Count == 0) return 1;

            int totalLevel = 0;
            int aliveCount = 0;
            foreach (var p in party)
            {
                if (p.IsAlive)
                {
                    totalLevel += p.Level;
                    aliveCount++;
                }
            }

            if (aliveCount == 0) return Math.Max(1, dungeonLevel);// If no party members are alive, return a mob level based on the dungeon level (at least 1)

            double avgLevel = (double)totalLevel / aliveCount;// Calculate the average level of the alive party members

            int mobLevel = (int)Math.Round(avgLevel);// Start with the average level as the base mob level

            mobLevel += dungeonLevel - 1;// Adjust mob level based on the dungeon level (higher dungeon levels should have stronger mobs)

            if (party.Count >= 4) mobLevel += 2;// If the party has 4 or more members, increase mob level by 2 to account for the increased player power
            else if (party.Count == 3) mobLevel += 1;// If the party has 3 members, increase mob level by 1

            return Math.Clamp(mobLevel, 1, MaxLevel);// Ensure the mob level is between 1 and MaxLevel
        }

        public static int CalculateMobLevelVariance(int baseMobLevel, Random? rng = null)// This method adds a random variance to the base mob level to create more variety in mob encounters.
                                                                                         // It generates a random number between -2 and +2 and applies it to the base mob level, then clamps the result to ensure it stays within valid bounds.
        {
            rng ??= new Random();// If no Random instance is provided, create a new one

            int variance = rng.Next(-2, 3);// Generate a random variance between -2 and +2 (inclusive)
            int finalLevel = baseMobLevel + variance;// Apply the variance to the base mob level

            return Math.Clamp(finalLevel, 1, MaxLevel);// Ensure the final mob level is between 1 and MaxLevel
        }

        public static string GetMobRankTitle(int mobLevel, int partyAvgLevel)// This method returns a rank title for a mob based on the difference between the mob's level and the average level of the party.
                                                                             // It uses a switch expression to determine the appropriate rank title, categorizing mobs as BOSS, ELITE, STRONG, NORMAL, WEAK, or TRIVIAL based on how their level compares to the party's average level.
        {
            int levelDiff = mobLevel - partyAvgLevel;// Calculate the difference between the mob's level and the party's average level

            return levelDiff switch// Use a switch expression to determine the mob's rank title based on the level difference
            {
                >= 5 => "[BOSS]",
                >= 3 => "[ELITE]",
                >= 1 => "[STRONG]",
                0 => "[NORMAL]",
                >= -2 => "[WEAK]",
                _ => "[TRIVIAL]"
            };
        }

        #endregion
    }

    #region LevelGains Class

    internal class LevelGains// This class represents the stat gains a character receives when leveling up. It includes properties for health, mana, stamina, strength, agility, intelligence, and armor rating.
    {
        #region Properties

        public int Health { get; }
        public int Mana { get; }
        public int Stamina { get; }
        public int Strength { get; }
        public int Agility { get; }
        public int Intelligence { get; }
        public int ArmorRating { get; }

        #endregion

        #region Constructor

        public LevelGains(int health, int mana, int stamina, int strength, int agility, int intelligence, int armorRating = 0)// The constructor initializes the LevelGains object with the specified stat gains for health, mana, stamina, strength, agility, intelligence, and optionally armor rating (defaulting to 0 if not provided).
        {
            Health = health;
            Mana = mana;
            Stamina = stamina;
            Strength = strength;
            Agility = agility;
            Intelligence = intelligence;
            ArmorRating = armorRating;
        }

        #endregion

        #region Methods

        public override string ToString()// This method returns a string representation of the LevelGains object, listing the stat gains in a readable format. It includes health, mana, stamina, strength, agility, intelligence, and armor rating if they are greater than 0.
        {
            var parts = new List<string> { $"+{Health} HP" };// Start with health gain as the first part of the string
            if (Mana > 0) parts.Add($"+{Mana} Mana");// If mana gain is greater than 0, add it to the parts list
            if (Stamina > 0) parts.Add($"+{Stamina} Stamina");// If stamina gain is greater than 0, add it to the parts list
            parts.Add($"+{Strength} Str");// Add strength gain to the parts list
            parts.Add($"+{Agility} Agi");// Add agility gain to the parts list
            parts.Add($"+{Intelligence} Int");// Add intelligence gain to the parts list
            if (ArmorRating > 0) parts.Add($"+{ArmorRating} AR");// If armor rating gain is greater than 0, add it to the parts list
            return string.Join(", ", parts);// Join all the parts together with a comma and space and return the resulting string
        }

        #endregion
    }

    #endregion
}
