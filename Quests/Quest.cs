using System;

namespace Rpg_Dungeon
{
    internal class Quest
    {
        #region Properties

        public string Name { get; }
        public string Description { get; }
        public QuestType Type { get; }
        public QuestDifficulty Difficulty { get; }
        public int GoldReward { get; }
        public int ExperienceReward { get; }
        public Equipment? EquipmentReward { get; }
        public bool IsCompleted { get; set; }
        public bool IsActive { get; set; }

        public string ObjectiveName { get; }
        public int ObjectiveCount { get; }
        public int CurrentProgress { get; set; }

        public string? QuestLocation { get; }

        #endregion

        #region Constructor

        public Quest(string name, string description, QuestType type, QuestDifficulty difficulty,
                     string objectiveName, int objectiveCount, int goldReward, int expReward, Equipment? equipReward = null, string? questLocation = null)
        {
            Name = name;
            Description = description;
            Type = type;
            Difficulty = difficulty;
            ObjectiveName = objectiveName;
            ObjectiveCount = objectiveCount;
            CurrentProgress = 0;
            GoldReward = goldReward;
            ExperienceReward = expReward;
            EquipmentReward = equipReward;
            IsCompleted = false;
            IsActive = false;
            QuestLocation = questLocation;
        }

        #endregion

        #region Methods

        public bool CheckCompletion()
        {
            return CurrentProgress >= ObjectiveCount;
        }

        public void AddProgress(int amount = 1)
        {
            CurrentProgress = Math.Min(CurrentProgress + amount, ObjectiveCount);
            if (CheckCompletion() && !IsCompleted)
            {
                Console.WriteLine($"\n*** Quest '{Name}' completed! Return to town to claim your reward! ***");
                IsCompleted = true;
            }
        }

        #endregion
    }
}
