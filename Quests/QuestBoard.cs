using Night.Characters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rpg_Dungeon
{
    internal class QuestBoard
    {
        #region Fields

        private readonly List<Quest> _availableQuests;
        private readonly List<Quest> _activeQuests;
        private readonly List<Quest> _completedQuests;
        private readonly Random _random;

        #endregion

        #region Constructor

        public QuestBoard()
        {
            _availableQuests = new List<Quest>();
            _activeQuests = new List<Quest>();
            _completedQuests = new List<Quest>();
            _random = new Random();
            GenerateInitialQuests();
        }

        #endregion

        #region Initialization

        private void GenerateInitialQuests()
        {
            _availableQuests.Add(new Quest(
                "Goblin Slayer",
                "The town is being harassed by goblins. Eliminate 5 of them.",
                QuestType.Kill,
                QuestDifficulty.Easy,
                "Goblin",
                5,
                50,
                100,
                null,
                "Whispering Woods"
            ));

            _availableQuests.Add(new Quest(
                "Orc Hunter",
                "Orcs have been spotted near the town. Kill 3 orcs to protect the citizens.",
                QuestType.Kill,
                QuestDifficulty.Medium,
                "Orc",
                3,
                100,
                200,
                new Equipment("Orcish Blade", EquipmentType.Weapon, 55, 120),
                "Fields of Valor"
            ));

            _availableQuests.Add(new Quest(
                "Dragon Slayer",
                "A fearsome dragon terrorizes the land. Only the bravest can defeat it!",
                QuestType.Kill,
                QuestDifficulty.Elite,
                "Dragon",
                1,
                500,
                1000,
                new Equipment("Dragon Scale Armor", EquipmentType.Armor, 100, 500),
                "Frostpeak Mountains"
            ));

            _availableQuests.Add(new Quest(
                "Herb Gathering",
                "The apothecary needs healing herbs. Collect 10 Healing Herbs.",
                QuestType.Collect,
                QuestDifficulty.Easy,
                "Healing Herb",
                10,
                75,
                80,
                null,
                "Whispering Woods"
            ));

            _availableQuests.Add(new Quest(
                "Rare Materials",
                "A mage requires rare materials. Find 3 Mana Crystals.",
                QuestType.Collect,
                QuestDifficulty.Medium,
                "Mana Crystal",
                3,
                120,
                150,
                null,
                "Luminous Depths"
            ));

            _availableQuests.Add(new Quest(
                "Deep Exploration",
                "Explore the dungeons thoroughly. Complete 5 dungeon levels.",
                QuestType.Explore,
                QuestDifficulty.Medium,
                "Dungeon Level",
                5,
                150,
                250,
                new Equipment("Explorer's Compass", EquipmentType.Accessory, 40, 80),
                "Shadowfen Marsh"
            ));

            _availableQuests.Add(new Quest(
                "Master Explorer",
                "Prove your worth by completing 15 dungeon levels.",
                QuestType.Explore,
                QuestDifficulty.Hard,
                "Dungeon Level",
                15,
                300,
                500,
                null,
                "Starfall Peak"
            ));
        }

        #endregion

        #region Quest Board Interface

        public void OpenQuestBoard(List<Character> party, Journal? journal = null, Weather? weather = null, TimeOfDay? timeTracker = null)
        {
            if (party == null || party.Count == 0) return;

            while (true)
            {
                Console.WriteLine("\n╔════════════════════════════════════════╗");
                Console.WriteLine("║           Quest Board                 ║");
                Console.WriteLine("╚════════════════════════════════════════╝");
                Console.WriteLine("1) View Available Quests");
                Console.WriteLine("2) Accept a Quest");
                Console.WriteLine("3) View Your Active Quests");
                Console.WriteLine("4) Claim Rewards");
                Console.WriteLine("5) Embark on Quest (Travel)");
                Console.WriteLine("0) Leave");
                Console.Write("Choice: ");

                var choice = Console.ReadLine() ?? string.Empty;

                switch (choice.Trim())
                {
                    case "1":
                        ViewAvailableQuestsDisplay();
                        break;
                    case "2":
                        AcceptQuest(party, journal);
                        break;
                    case "3":
                        ViewActiveQuestsDisplay(journal);
                        break;
                    case "4":
                        ClaimRewards(party, journal);
                        break;
                    case "5":
                        EmbarkOnQuest(party, journal, weather, timeTracker);
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Invalid choice.");
                        break;
                }
            }
        }

        #endregion

        #region View Methods

        private void ViewAvailableQuestsDisplay()
        {
            Console.WriteLine("\n=== Available Quests ===");
            if (_availableQuests.Count == 0)
            {
                Console.WriteLine("No quests available. Check back later!");
                return;
            }

            for (int i = 0; i < _availableQuests.Count; i++)
            {
                var q = _availableQuests[i];
                string locationInfo = q.QuestLocation != null ? $" 📍 {q.QuestLocation}" : "";
                Console.WriteLine($"\n{i + 1}) [{q.Difficulty}] {q.Name}{locationInfo}");
                Console.WriteLine($"   {q.Description}");
                Console.WriteLine($"   Type: {q.Type} - {q.ObjectiveName} x{q.ObjectiveCount}");
                Console.WriteLine($"   Rewards: {q.GoldReward}g, {q.ExperienceReward} XP" +
                                (q.EquipmentReward != null ? $", {q.EquipmentReward.Name}" : ""));
            }
        }

        private void AcceptQuest(List<Character> party, Journal? journal)
        {
            Console.WriteLine("\n=== Accept a Quest ===");
            if (_availableQuests.Count == 0)
            {
                Console.WriteLine("No quests available to accept!");
                return;
            }

            for (int i = 0; i < _availableQuests.Count; i++)
            {
                var q = _availableQuests[i];
                string locationInfo = q.QuestLocation != null ? $" 📍 {q.QuestLocation}" : "";
                Console.WriteLine($"\n{i + 1}) [{q.Difficulty}] {q.Name}{locationInfo}");
                Console.WriteLine($"   {q.Description}");
                Console.WriteLine($"   Type: {q.Type} - {q.ObjectiveName} x{q.ObjectiveCount}");
                Console.WriteLine($"   Rewards: {q.GoldReward}g, {q.ExperienceReward} XP" +
                                (q.EquipmentReward != null ? $", {q.EquipmentReward.Name}" : ""));
            }

            Console.Write("\nAccept quest (number) or 0 to cancel: ");
            var input = Console.ReadLine() ?? string.Empty;
            if (!int.TryParse(input, out var idx) || idx < 1 || idx > _availableQuests.Count)
            {
                Console.WriteLine("Cancelled.");
                return;
            }

            var quest = _availableQuests[idx - 1];
            quest.IsActive = true;
            _activeQuests.Add(quest);
            _availableQuests.RemoveAt(idx - 1);

            if (journal != null)
            {
                journal.AddActiveQuest(quest);
            }

            Console.WriteLine($"\n📜 Quest '{quest.Name}' accepted! Good luck!");
            Console.WriteLine("The quest has been added to your journal.");
        }

        private void ViewActiveQuestsDisplay(Journal? journal)
        {
            Console.WriteLine("\n=== Your Active Quests ===");

            var activeQuests = journal?.GetActiveQuests() ?? _activeQuests;

            if (activeQuests.Count == 0)
            {
                Console.WriteLine("No active quests.");
                return;
            }

            foreach (var q in activeQuests)
            {
                string locationInfo = q.QuestLocation != null ? $" 📍 {q.QuestLocation}" : "";
                Console.WriteLine($"\n[{q.Difficulty}] {q.Name}{locationInfo}");
                Console.WriteLine($"  {q.Description}");
                Console.WriteLine($"  Progress: {q.CurrentProgress}/{q.ObjectiveCount} {q.ObjectiveName}");
                Console.WriteLine($"  Status: {(q.IsCompleted ? "✓ COMPLETE - Claim reward!" : "○ In Progress")}");
            }
        }

        private void ViewActiveQuests()
        {
            Console.WriteLine("\n=== Active Quests ===");
            if (_activeQuests.Count == 0)
            {
                Console.WriteLine("No active quests.");
                return;
            }

            foreach (var q in _activeQuests)
            {
                Console.WriteLine($"\n[{q.Difficulty}] {q.Name}");
                Console.WriteLine($"  Progress: {q.CurrentProgress}/{q.ObjectiveCount} {q.ObjectiveName}");
                Console.WriteLine($"  Status: {(q.IsCompleted ? "COMPLETE - Claim reward!" : "In Progress")}");
            }
        }

        private void ViewCompletedQuests()
        {
            Console.WriteLine("\n=== Completed Quests ===");
            if (_completedQuests.Count == 0)
            {
                Console.WriteLine("No completed quests yet.");
                return;
            }

            foreach (var q in _completedQuests)
            {
                Console.WriteLine($"✓ {q.Name} [{q.Difficulty}]");
            }
        }

        #endregion

        #region Quest Management

        private void ClaimRewards(List<Character> party, Journal? journal)
        {
            var activeQuests = journal?.GetActiveQuests() ?? _activeQuests;
            var completedQuests = activeQuests.Where(q => q.IsCompleted).ToList();

            if (completedQuests.Count == 0)
            {
                Console.WriteLine("No completed quests to claim.");
                return;
            }

            Console.WriteLine("\n=== Claim Quest Rewards ===");
            for (int i = 0; i < completedQuests.Count; i++)
            {
                var q = completedQuests[i];
                Console.WriteLine($"{i + 1}) {q.Name} - {q.GoldReward}g, {q.ExperienceReward} XP" +
                                (q.EquipmentReward != null ? $", {q.EquipmentReward.Name}" : ""));
            }

            Console.Write("Claim which quest reward? ");
            var input = Console.ReadLine() ?? string.Empty;
            if (!int.TryParse(input, out var idx) || idx < 1 || idx > completedQuests.Count) return;

            var quest = completedQuests[idx - 1];

            Console.WriteLine("\nWho will receive the rewards?");
            for (int i = 0; i < party.Count; i++)
            {
                Console.WriteLine($"{i + 1}) {party[i].Name} (Lv {party[i].Level})");
            }

            var whoInput = Console.ReadLine() ?? string.Empty;
            if (!int.TryParse(whoInput, out var whoIdx) || whoIdx < 1 || whoIdx > party.Count) return;

            var character = party[whoIdx - 1];
            character.Inventory.AddGold(quest.GoldReward);
            character.GainExperience(quest.ExperienceReward);

            if (quest.EquipmentReward != null)
            {
                if (character.Inventory.AddItem(quest.EquipmentReward))
                {
                    Console.WriteLine($"{character.Name} received {quest.EquipmentReward.Name}!");
                }
                else
                {
                    Console.WriteLine("Inventory full! Equipment reward lost.");
                }
            }

            Console.WriteLine($"\n{character.Name} received {quest.GoldReward} gold and {quest.ExperienceReward} XP!");
            _activeQuests.Remove(quest);
            _completedQuests.Add(quest);

            if (journal != null)
            {
                journal.CompleteQuest(quest);
            }
        }

        private void EmbarkOnQuest(List<Character> party, Journal? journal, Weather? weather, TimeOfDay? timeTracker)
        {
            var activeQuests = journal?.GetActiveQuests() ?? _activeQuests;

            if (activeQuests.Count == 0)
            {
                Console.WriteLine("\nYou have no active quests to embark on!");
                Console.WriteLine("Accept a quest first.");
                return;
            }

            Console.WriteLine("\n=== Embark on Quest ===");
            for (int i = 0; i < activeQuests.Count; i++)
            {
                var q = activeQuests[i];
                string status = q.IsCompleted ? "[COMPLETE]" : $"[{q.CurrentProgress}/{q.ObjectiveCount}]";
                string locationInfo = q.QuestLocation != null ? $" 📍 {q.QuestLocation}" : "";
                Console.WriteLine($"{i + 1}) {q.Name} {status}{locationInfo}");
            }

            Console.Write("\nChoose quest to travel to (or 0 to cancel): ");
            var input = Console.ReadLine() ?? string.Empty;
            if (!int.TryParse(input, out var idx) || idx < 1 || idx > activeQuests.Count) return;

            var selectedQuest = activeQuests[idx - 1];

            if (weather != null && timeTracker != null)
            {
                TravelManager.TravelToQuest(party, weather, timeTracker, selectedQuest.Name);
            }
            else
            {
                Console.WriteLine($"\n🚶 You travel to the quest location for '{selectedQuest.Name}'...");
            }

            string location = selectedQuest.QuestLocation ?? "the quest location";
            Console.WriteLine($"\nYou arrive at {location}!");
            Console.WriteLine($"Objective: {selectedQuest.Description}");
            Console.WriteLine($"Progress: {selectedQuest.CurrentProgress}/{selectedQuest.ObjectiveCount}");
            Console.WriteLine("\nTip: Use the World Map to explore areas and complete quest objectives.");
            Console.WriteLine("Press Enter to return to town...");
            Console.ReadLine();

            if (weather != null && timeTracker != null)
            {
                TravelManager.ReturnToTown(party, weather, timeTracker);
            }
        }

        #endregion

        #region Progress Tracking

        public void UpdateQuestProgress(string objectiveName, int amount = 1)
        {
            foreach (var quest in _activeQuests.Where(q => !q.IsCompleted))
            {
                if (quest.ObjectiveName.Equals(objectiveName, StringComparison.OrdinalIgnoreCase))
                {
                    quest.AddProgress(amount);
                }
            }
        }

        public void UpdateQuestProgressInJournal(Journal? journal, string objectiveName, int amount = 1)
        {
            if (journal == null) return;
            journal.UpdateQuestProgress(objectiveName, amount);
        }

        public List<Quest> GetActiveQuests() => _activeQuests;

        #endregion
    }
}
