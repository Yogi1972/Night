using Night.Characters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rpg_Dungeon
{
    internal class AchievementTracker
    {
        #region Fields

        private readonly List<Achievement> _achievements;
        private readonly HashSet<string> _claimedAchievements;

        #endregion

        #region Constructor

        public AchievementTracker()
        {
            _achievements = new List<Achievement>();
            _claimedAchievements = new HashSet<string>();
            InitializeAchievements();
        }

        #endregion

        #region Initialization

        private void InitializeAchievements()
        {
            // Combat Achievements
            _achievements.Add(new Achievement(
                "First Blood",
                "Defeat your first enemy",
                AchievementCategory.Combat,
                1, 25, 50
            ));

            _achievements.Add(new Achievement(
                "Goblin Slayer",
                "Defeat 50 goblins",
                AchievementCategory.Combat,
                50, 100, 200
            ));

            _achievements.Add(new Achievement(
                "Dragon Hunter",
                "Defeat 5 dragons",
                AchievementCategory.Combat,
                5, 500, 1000
            ));

            _achievements.Add(new Achievement(
                "Centurion",
                "Defeat 100 enemies total",
                AchievementCategory.Combat,
                100, 200, 300
            ));

            // Exploration Achievements
            _achievements.Add(new Achievement(
                "Dungeon Delver",
                "Complete 10 dungeon levels",
                AchievementCategory.Exploration,
                10, 150, 250
            ));

            _achievements.Add(new Achievement(
                "Master Explorer",
                "Complete 50 dungeon levels",
                AchievementCategory.Exploration,
                50, 400, 600
            ));

            _achievements.Add(new Achievement(
                "Treasure Hunter",
                "Open 25 treasure chests",
                AchievementCategory.Exploration,
                25, 100, 150
            ));

            // Wealth Achievements
            _achievements.Add(new Achievement(
                "Penny Pincher",
                "Accumulate 1000 gold",
                AchievementCategory.Wealth,
                1000, 50, 100
            ));

            _achievements.Add(new Achievement(
                "Wealthy Adventurer",
                "Accumulate 5000 gold",
                AchievementCategory.Wealth,
                5000, 250, 500
            ));

            _achievements.Add(new Achievement(
                "Merchant Prince",
                "Sell 100 items to shops",
                AchievementCategory.Wealth,
                100, 300, 400
            ));

            // Collection Achievements
            _achievements.Add(new Achievement(
                "Geared Up",
                "Equip items in all 6 equipment slots",
                AchievementCategory.Collection,
                6, 100, 150
            ));

            _achievements.Add(new Achievement(
                "Fully Stocked",
                "Fill all inventory slots at once",
                AchievementCategory.Collection,
                1, 75, 100
            ));

            _achievements.Add(new Achievement(
                "Master Crafter",
                "Craft 25 items",
                AchievementCategory.Collection,
                25, 200, 300
            ));

            // Social Achievements
            _achievements.Add(new Achievement(
                "Quest Starter",
                "Complete your first quest",
                AchievementCategory.Social,
                1, 50, 100
            ));

            _achievements.Add(new Achievement(
                "Hero of the Town",
                "Complete 10 quests",
                AchievementCategory.Social,
                10, 300, 500
            ));

            _achievements.Add(new Achievement(
                "Pet Lover",
                "Raise a pet's loyalty to 100",
                AchievementCategory.Social,
                1, 100, 150
            ));
        }

        #endregion

        #region Display Methods

        public void DisplayAchievements()
        {
            Console.WriteLine("\n╔════════════════════════════════════════╗");
            Console.WriteLine("║          Achievements                 ║");
            Console.WriteLine("╚════════════════════════════════════════╝");

            var categories = Enum.GetValues(typeof(AchievementCategory)).Cast<AchievementCategory>();

            foreach (var category in categories)
            {
                var categoryAchievements = _achievements.Where(a => a.Category == category).ToList();
                if (categoryAchievements.Count == 0) continue;

                Console.WriteLine($"\n--- {category} ---");
                foreach (var achievement in categoryAchievements)
                {
                    var status = achievement.IsUnlocked ? "✓" : " ";
                    var claimed = _claimedAchievements.Contains(achievement.Name) ? "[CLAIMED]" : "";
                    Console.WriteLine($"[{status}] {achievement.Name} {claimed}");
                    Console.WriteLine($"    {achievement.Description}");
                    if (!achievement.IsUnlocked)
                    {
                        Console.WriteLine($"    Progress: {achievement.CurrentValue}/{achievement.TargetValue} ({achievement.GetProgress():F1}%)");
                    }
                    else if (!_claimedAchievements.Contains(achievement.Name))
                    {
                        Console.WriteLine($"    Reward Ready: {achievement.RewardGold}g, {achievement.RewardXP} XP");
                    }
                }
            }

            var totalUnlocked = _achievements.Count(a => a.IsUnlocked);
            Console.WriteLine($"\nTotal: {totalUnlocked}/{_achievements.Count} achievements unlocked");
        }

        #endregion

        #region Reward Management

        public void ClaimRewards(Character character)
        {
            var unclaimedAchievements = _achievements.Where(a => a.IsUnlocked && !_claimedAchievements.Contains(a.Name)).ToList();

            if (unclaimedAchievements.Count == 0)
            {
                Console.WriteLine("No unclaimed achievement rewards!");
                return;
            }

            Console.WriteLine("\n=== Unclaimed Achievement Rewards ===");
            for (int i = 0; i < unclaimedAchievements.Count; i++)
            {
                var ach = unclaimedAchievements[i];
                Console.WriteLine($"{i + 1}) {ach.Name} - {ach.RewardGold}g, {ach.RewardXP} XP");
            }
            Console.WriteLine($"{unclaimedAchievements.Count + 1}) Claim All");

            Console.Write("Claim which reward? ");
            var input = Console.ReadLine() ?? string.Empty;
            if (!int.TryParse(input, out var choice)) return;

            if (choice == unclaimedAchievements.Count + 1)
            {
                // Claim all
                int totalGold = 0;
                int totalXP = 0;
                foreach (var ach in unclaimedAchievements)
                {
                    totalGold += ach.RewardGold;
                    totalXP += ach.RewardXP;
                    _claimedAchievements.Add(ach.Name);
                }
                character.Inventory.AddGold(totalGold);
                character.GainExperience(totalXP);
                Console.WriteLine($"\n{character.Name} claimed all rewards: {totalGold}g, {totalXP} XP!");
            }
            else if (choice >= 1 && choice <= unclaimedAchievements.Count)
            {
                var ach = unclaimedAchievements[choice - 1];
                character.Inventory.AddGold(ach.RewardGold);
                character.GainExperience(ach.RewardXP);
                _claimedAchievements.Add(ach.Name);
                Console.WriteLine($"\n{character.Name} claimed {ach.Name}: {ach.RewardGold}g, {ach.RewardXP} XP!");
            }
        }

        #endregion

        #region Progress Tracking

        public void TrackProgress(string achievementName, int amount = 1)
        {
            var achievement = _achievements.FirstOrDefault(a => a.Name == achievementName);
            achievement?.AddProgress(amount);
        }

        public void TrackCombatKill(string enemyName)
        {
            TrackProgress("First Blood");
            TrackProgress("Centurion");

            if (enemyName.IndexOf("Goblin", StringComparison.OrdinalIgnoreCase) >= 0)
                TrackProgress("Goblin Slayer");
            if (enemyName.IndexOf("Dragon", StringComparison.OrdinalIgnoreCase) >= 0)
                TrackProgress("Dragon Hunter");
        }

        public void TrackDungeonComplete()
        {
            TrackProgress("Dungeon Delver");
            TrackProgress("Master Explorer");
        }

        public void TrackChestOpened()
        {
            TrackProgress("Treasure Hunter");
        }

        public void TrackGoldAccumulation(int totalGold)
        {
            var pennyPincher = _achievements.First(a => a.Name == "Penny Pincher");
            var wealthy = _achievements.First(a => a.Name == "Wealthy Adventurer");

            if (!pennyPincher.IsUnlocked && totalGold >= 1000)
                pennyPincher.AddProgress(1000);
            if (!wealthy.IsUnlocked && totalGold >= 5000)
                wealthy.AddProgress(5000);
        }

        public void TrackItemSold()
        {
            TrackProgress("Merchant Prince");
        }

        public void TrackEquippedSlots(int equippedCount)
        {
            var geared = _achievements.FirstOrDefault(a => a.Name == "Geared Up");
            if (geared != null && !geared.IsUnlocked && equippedCount >= 6)
                geared.AddProgress(6);
        }

        public void TrackInventoryFull()
        {
            TrackProgress("Fully Stocked");
        }

        public void TrackCraftedItem()
        {
            TrackProgress("Master Crafter");
        }

        public void TrackQuestComplete()
        {
            TrackProgress("Quest Starter");
            TrackProgress("Hero of the Town");
        }

        public void TrackPetLoyalty(int loyalty)
        {
            var petLover = _achievements.FirstOrDefault(a => a.Name == "Pet Lover");
            if (petLover != null && !petLover.IsUnlocked && loyalty >= 100)
                petLover.AddProgress(1);
        }

        #endregion
    }
}
