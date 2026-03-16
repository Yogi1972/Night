using Night.Characters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rpg_Dungeon
{
    #region Journal Class

    internal class Journal
    {
        #region Fields

        private readonly List<Quest> _activeQuests;
        private readonly List<Quest> _completedQuests;
        private readonly List<Bounty> _activeBounties;
        private readonly List<Bounty> _completedBounties;

        #endregion

        #region Constructor

        public Journal()
        {
            _activeQuests = new List<Quest>();
            _completedQuests = new List<Quest>();
            _activeBounties = new List<Bounty>();
            _completedBounties = new List<Bounty>();
        }

        #endregion

        #region Quest Management

        public void AddActiveQuest(Quest quest)
        {
            if (!_activeQuests.Contains(quest))
            {
                _activeQuests.Add(quest);
                quest.IsActive = true;
            }
        }

        public void CompleteQuest(Quest quest)
        {
            if (_activeQuests.Remove(quest))
            {
                _completedQuests.Add(quest);
                quest.IsCompleted = true;
            }
        }

        public List<Quest> GetActiveQuests() => _activeQuests;

        public List<Quest> GetCompletedQuests() => _completedQuests;

        #endregion

        #region Bounty Management

        public void AddActiveBounty(Bounty bounty)
        {
            if (!_activeBounties.Contains(bounty))
            {
                _activeBounties.Add(bounty);
            }
        }

        public void CompleteBounty(Bounty bounty)
        {
            if (_activeBounties.Remove(bounty))
            {
                _completedBounties.Add(bounty);
                bounty.IsCompleted = true;
                bounty.IsClaimed = true;
            }
        }

        public List<Bounty> GetActiveBounties() => _activeBounties;

        public List<Bounty> GetCompletedBounties() => _completedBounties;

        #endregion

        #region Journal Display

        public void OpenJournal(List<Character> party)
        {
            if (party == null || party.Count == 0) return;

            while (true)
            {
                Console.WriteLine("\n╔════════════════════════════════════════╗");
                Console.WriteLine("║              Journal                  ║");
                Console.WriteLine("╚════════════════════════════════════════╝");
                Console.WriteLine($"Active Quests: {_activeQuests.Count} | Completed Quests: {_completedQuests.Count}");
                Console.WriteLine($"Active Bounties: {_activeBounties.Count} | Completed Bounties: {_completedBounties.Count}");

                Console.WriteLine("\n1) View Active Quests");
                Console.WriteLine("2) View Completed Quests");
                Console.WriteLine("3) View Active Bounties");
                Console.WriteLine("4) View Completed Bounties");
                Console.WriteLine("5) View All (Summary)");
                Console.WriteLine("0) Close Journal");
                Console.Write("Choice: ");

                var choice = Console.ReadLine() ?? string.Empty;

                switch (choice.Trim())
                {
                    case "1":
                        ViewActiveQuestsDetails();
                        break;
                    case "2":
                        ViewCompletedQuestsDetails();
                        break;
                    case "3":
                        ViewActiveBountiesDetails();
                        break;
                    case "4":
                        ViewCompletedBountiesDetails();
                        break;
                    case "5":
                        ViewAllSummary();
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

        #region Display Methods

        private void ViewActiveQuestsDetails()
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
                Console.WriteLine($"  {q.Description}");
                Console.WriteLine($"  Type: {q.Type} - {q.ObjectiveName}");
                Console.WriteLine($"  Progress: {q.CurrentProgress}/{q.ObjectiveCount}");
                Console.WriteLine($"  Rewards: {q.GoldReward}g, {q.ExperienceReward} XP" +
                                (q.EquipmentReward != null ? $", {q.EquipmentReward.Name}" : ""));
                Console.WriteLine($"  Status: {(q.IsCompleted ? "✓ COMPLETE - Claim at Quest Board" : "○ In Progress")}");
            }
        }

        private void ViewCompletedQuestsDetails()
        {
            Console.WriteLine("\n=== Completed Quests ===");
            if (_completedQuests.Count == 0)
            {
                Console.WriteLine("No completed quests yet.");
                return;
            }

            foreach (var q in _completedQuests)
            {
                Console.WriteLine($"✓ [{q.Difficulty}] {q.Name}");
            }
            Console.WriteLine($"\nTotal Quests Completed: {_completedQuests.Count}");
        }

        private void ViewActiveBountiesDetails()
        {
            Console.WriteLine("\n=== Active Bounties ===");
            if (_activeBounties.Count == 0)
            {
                Console.WriteLine("No active bounties.");
                return;
            }

            foreach (var bounty in _activeBounties.OrderBy(b => b.Difficulty))
            {
                var difficultySymbol = bounty.Difficulty switch
                {
                    BountyDifficulty.Common => "●",
                    BountyDifficulty.Uncommon => "◆",
                    BountyDifficulty.Rare => "★",
                    BountyDifficulty.Legendary => "☆",
                    _ => "○"
                };

                var status = bounty.IsCompleted ? "✓ COMPLETE - Claim at Bounty Board" : "○ Hunt Target";
                Console.WriteLine($"\n{difficultySymbol} [{bounty.Difficulty}] {bounty.TargetName}");
                Console.WriteLine($"  {bounty.Description}");
                Console.WriteLine($"  Reward: {bounty.GoldReward} gold, {bounty.ExperienceReward} XP");
                Console.WriteLine($"  Status: {status}");
            }
        }

        private void ViewCompletedBountiesDetails()
        {
            Console.WriteLine("\n=== Completed Bounties ===");
            if (_completedBounties.Count == 0)
            {
                Console.WriteLine("No bounties completed yet.");
                return;
            }

            foreach (var bounty in _completedBounties)
            {
                Console.WriteLine($"✓ [{bounty.Difficulty}] {bounty.TargetName}");
            }
            Console.WriteLine($"\nTotal Bounties Completed: {_completedBounties.Count}");
        }

        private void ViewAllSummary()
        {
            Console.WriteLine("\n╔════════════════════════════════════════╗");
            Console.WriteLine("║         Journal Summary               ║");
            Console.WriteLine("╚════════════════════════════════════════╝");

            Console.WriteLine("\n--- Quests ---");
            Console.WriteLine($"Active: {_activeQuests.Count}");
            Console.WriteLine($"Completed: {_completedQuests.Count}");

            if (_activeQuests.Count > 0)
            {
                Console.WriteLine("\nActive Quests:");
                foreach (var q in _activeQuests)
                {
                    var status = q.IsCompleted ? "✓" : "○";
                    Console.WriteLine($"  {status} {q.Name} ({q.CurrentProgress}/{q.ObjectiveCount})");
                }
            }

            Console.WriteLine("\n--- Bounties ---");
            Console.WriteLine($"Active: {_activeBounties.Count}");
            Console.WriteLine($"Completed: {_completedBounties.Count}");

            if (_activeBounties.Count > 0)
            {
                Console.WriteLine("\nActive Bounties:");
                foreach (var b in _activeBounties)
                {
                    var status = b.IsCompleted ? "✓" : "○";
                    Console.WriteLine($"  {status} {b.TargetName} [{b.Difficulty}]");
                }
            }

            Console.WriteLine($"\n--- Total Progress ---");
            Console.WriteLine($"Quests completed: {_completedQuests.Count}");
            Console.WriteLine($"Bounties completed: {_completedBounties.Count}");
            Console.WriteLine($"Total objectives achieved: {_completedQuests.Count + _completedBounties.Count}");
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

        public void CheckBountyCompletion(string enemyName)
        {
            foreach (var bounty in _activeBounties.Where(b => !b.IsCompleted))
            {
                if (enemyName.Contains(bounty.TargetName, StringComparison.OrdinalIgnoreCase) ||
                    bounty.TargetName.Contains(enemyName, StringComparison.OrdinalIgnoreCase))
                {
                    bounty.IsCompleted = true;
                    Console.WriteLine($"\n🎯 BOUNTY COMPLETED: {bounty.TargetName}!");
                    Console.WriteLine($"Return to town to claim your reward: {bounty.GoldReward}g, {bounty.ExperienceReward} XP");
                }
            }
        }

        #endregion
    }

    #endregion
}
