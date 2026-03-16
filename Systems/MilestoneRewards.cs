using Night.Characters;
using System;
using System.Collections.Generic;

namespace Rpg_Dungeon
{
    /// <summary>
    /// Manages milestone rewards when players reach significant levels
    /// </summary>
    internal static class MilestoneRewards
    {
        private static readonly HashSet<int> _milestoneLevels = new HashSet<int>
        {
            5, 10, 15, 20, 25, 30, 35, 40, 45, 50,
            60, 70, 75, 80, 90, 100
        };

        /// <summary>
        /// Check if a level is a milestone
        /// </summary>
        public static bool IsMilestone(int level)
        {
            return _milestoneLevels.Contains(level);
        }

        /// <summary>
        /// Award milestone rewards to a character
        /// </summary>
        public static void AwardMilestoneReward(Character character)
        {
            int level = character.Level;

            if (!IsMilestone(level))
                return;

            VisualEffects.ShowMilestoneReward(level);
            Console.WriteLine();

            VisualEffects.WriteLineColored($"🎉 {character.Name} has reached a milestone level!", ConsoleColor.Yellow);
            Console.WriteLine();

            // Calculate rewards based on level
            int goldReward = level * 50;

            character.Inventory.AddGold(goldReward);
            VisualEffects.WriteSuccess($"💰 Received {goldReward} gold!\n");

            // Special rewards at major milestones
            if (level == 10)
            {
                Console.WriteLine("📜 Unlocked: Access to Champion Classes!");
                VisualEffects.WriteInfo("   (You can now upgrade your class to a specialized champion)\n");
            }
            else if (level == 25)
            {
                Console.WriteLine("⚔️  Unlocked: Advanced Combat Techniques!");
                VisualEffects.WriteInfo("   (You've learned new skills and abilities)\n");

                var legendaryItem = LegendaryItemSystem.GetLegendaryForLevel(level);
                LegendaryItemSystem.AnnounceItemFound(legendaryItem);
                character.Inventory.AddItem(legendaryItem);
                VisualEffects.WriteSuccess($"✨ {legendaryItem.Name} has been added to your inventory!\n");
            }
            else if (level == 50)
            {
                Console.WriteLine("👑 Title Earned: 'Champion of the Realm'");
                VisualEffects.WriteInfo("   (Your reputation precedes you)\n");

                var legendaryItem = LegendaryItemSystem.GetLegendaryForLevel(level);
                LegendaryItemSystem.AnnounceItemFound(legendaryItem);
                character.Inventory.AddItem(legendaryItem);
                VisualEffects.WriteSuccess($"✨ {legendaryItem.Name} has been added to your inventory!\n");

                // Offer Mythic Title ascension
                if (MythicTitleManager.CanSelectMythicTitle(character))
                {
                    Console.WriteLine("\n🌟 You have unlocked MYTHIC TITLE ASCENSION!");
                    Console.WriteLine("   Press any key to choose your Mythic Title...");
                    Console.ReadKey(true);
                    MythicTitleManager.ShowMythicTitleSelection(character);
                }
            }
            else if (level == 75)
            {
                Console.WriteLine("⚡ Unlocked: Legendary Skill Tree Branch!");
                VisualEffects.WriteInfo("   (New powerful skills are now available)\n");

                var legendaryItem = LegendaryItemSystem.GetLegendaryForLevel(level);
                LegendaryItemSystem.AnnounceItemFound(legendaryItem);
                character.Inventory.AddItem(legendaryItem);
                VisualEffects.WriteSuccess($"✨ {legendaryItem.Name} has been added to your inventory!\n");
            }
            else if (level == 100)
            {
                Console.WriteLine();
                VisualEffects.WriteLineColored("╔═══════════════════════════════════════════════════════════╗", ConsoleColor.Magenta);
                VisualEffects.WriteLineColored("║                                                           ║", ConsoleColor.Magenta);
                VisualEffects.WriteLineColored("║           ⚡ MAXIMUM LEVEL ACHIEVED! ⚡                  ║", ConsoleColor.Yellow);
                VisualEffects.WriteLineColored("║                                                           ║", ConsoleColor.Magenta);
                VisualEffects.WriteLineColored("║         You have become a LEGEND!                         ║", ConsoleColor.Cyan);
                VisualEffects.WriteLineColored("║                                                           ║", ConsoleColor.Magenta);
                VisualEffects.WriteLineColored("╚═══════════════════════════════════════════════════════════╝", ConsoleColor.Magenta);
                Console.WriteLine();

                // Ultimate reward
                goldReward *= 5;
                character.Inventory.AddGold(goldReward);
                VisualEffects.WriteSuccess($"💰 Bonus Gold: {goldReward}!\n");

                var legendaryItem = LegendaryItemSystem.GetLegendaryForLevel(level);
                LegendaryItemSystem.AnnounceItemFound(legendaryItem);
                character.Inventory.AddItem(legendaryItem);
                VisualEffects.WriteSuccess($"✨ {legendaryItem.Name} has been added to your inventory!\n");
            }
            else if (level % 10 == 0)
            {
                // Every 10 levels (not covered above), give a special reward
                Console.WriteLine($"🎁 Special Milestone Reward!");
                int bonusGold = level * 25;
                character.Inventory.AddGold(bonusGold);
                VisualEffects.WriteSuccess($"💰 Bonus Gold: {bonusGold}!\n");
            }

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey(true);
        }
    }
}
