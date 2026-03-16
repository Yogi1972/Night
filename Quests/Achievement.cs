using System;

namespace Rpg_Dungeon
{
    internal class Achievement
    {
        #region Properties

        public string Name { get; }
        public string Description { get; }
        public AchievementCategory Category { get; }
        public int TargetValue { get; }
        public int CurrentValue { get; set; }
        public bool IsUnlocked { get; set; }
        public int RewardGold { get; }
        public int RewardXP { get; }

        #endregion

        #region Constructor

        public Achievement(string name, string description, AchievementCategory category, int targetValue, int rewardGold, int rewardXP)
        {
            Name = name;
            Description = description;
            Category = category;
            TargetValue = targetValue;
            CurrentValue = 0;
            IsUnlocked = false;
            RewardGold = rewardGold;
            RewardXP = rewardXP;
        }

        #endregion

        #region Methods

        public void AddProgress(int amount = 1)
        {
            if (IsUnlocked) return;

            CurrentValue += amount;
            if (CurrentValue >= TargetValue)
            {
                IsUnlocked = true;
                Console.WriteLine($"\n🏆 ACHIEVEMENT UNLOCKED: {Name}!");
                Console.WriteLine($"   {Description}");
                Console.WriteLine($"   Reward: {RewardGold}g, {RewardXP} XP");
            }
        }

        public float GetProgress() => Math.Min(100f, (float)CurrentValue / TargetValue * 100f);

        #endregion
    }
}
