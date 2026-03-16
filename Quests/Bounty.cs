namespace Rpg_Dungeon
{
    internal class Bounty
    {
        #region Properties

        public string TargetName { get; }
        public string Description { get; }
        public BountyDifficulty Difficulty { get; }
        public int GoldReward { get; }
        public int ExperienceReward { get; }
        public bool IsCompleted { get; set; }
        public bool IsClaimed { get; set; }

        #endregion

        #region Constructor

        public Bounty(string targetName, string description, BountyDifficulty difficulty, int goldReward, int expReward)
        {
            TargetName = targetName;
            Description = description;
            Difficulty = difficulty;
            GoldReward = goldReward;
            ExperienceReward = expReward;
            IsCompleted = false;
            IsClaimed = false;
        }

        #endregion
    }
}
