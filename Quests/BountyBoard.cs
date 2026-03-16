using Night.Characters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rpg_Dungeon
{
    internal class BountyBoard
    {
        #region Fields

        private readonly List<Bounty> _activeBounties;
        private readonly List<Bounty> _completedBounties;
        private readonly Random _random;

        #endregion

        #region Constructor

        public BountyBoard()
        {
            _activeBounties = new List<Bounty>();
            _completedBounties = new List<Bounty>();
            _random = new Random();
            GenerateInitialBounties();
        }

        #endregion

        #region Initialization

        private void GenerateInitialBounties()
        {
            // Common Bounties
            _activeBounties.Add(new Bounty(
                "Goblin Raider",
                "A goblin has been stealing from travelers. Eliminate it!",
                BountyDifficulty.Common,
                50,
                75
            ));

            _activeBounties.Add(new Bounty(
                "Bandit Leader",
                "The leader of a bandit gang terrorizes the roads.",
                BountyDifficulty.Common,
                75,
                100
            ));

            // Uncommon Bounties
            _activeBounties.Add(new Bounty(
                "Orc Warchief",
                "A powerful orc leads raids against the town.",
                BountyDifficulty.Uncommon,
                150,
                200
            ));

            _activeBounties.Add(new Bounty(
                "Dark Mage",
                "A rogue mage practices forbidden magic in the dungeons.",
                BountyDifficulty.Uncommon,
                180,
                250
            ));

            // Rare Bounties
            _activeBounties.Add(new Bounty(
                "Troll King",
                "An enormous troll rules over a cave system.",
                BountyDifficulty.Rare,
                300,
                450
            ));

            _activeBounties.Add(new Bounty(
                "Shadow Assassin",
                "A deadly assassin strikes from the darkness.",
                BountyDifficulty.Rare,
                350,
                500
            ));

            // Legendary Bounties
            _activeBounties.Add(new Bounty(
                "Ancient Dragon",
                "A legendary dragon that has lived for centuries.",
                BountyDifficulty.Legendary,
                1000,
                1500
            ));

            _activeBounties.Add(new Bounty(
                "Lich Lord",
                "An undead sorcerer of immense power.",
                BountyDifficulty.Legendary,
                1200,
                1800
            ));
        }

        #endregion

        #region Board Interface

        public void OpenBountyBoard(List<Character> party, Journal? journal = null)
        {
            if (party == null || party.Count == 0) return;

            while (true)
            {
                Console.WriteLine("\n╔════════════════════════════════════════╗");
                Console.WriteLine("║           Bounty Board                ║");
                Console.WriteLine("╚════════════════════════════════════════╝");
                Console.WriteLine("Hunt dangerous targets for rewards!");
                Console.WriteLine("\n1) View Available Bounties");
                Console.WriteLine("2) Accept a Bounty");
                Console.WriteLine("3) View Your Active Bounties");
                Console.WriteLine("4) Claim Bounty Rewards");
                Console.WriteLine("5) Generate New Bounty (50g)");
                Console.WriteLine("0) Leave");
                Console.Write("Choice: ");

                var choice = Console.ReadLine() ?? string.Empty;

                switch (choice.Trim())
                {
                    case "1":
                        ViewAvailableBounties();
                        break;
                    case "2":
                        AcceptBounty(party, journal);
                        break;
                    case "3":
                        ViewActiveBounties(journal);
                        break;
                    case "4":
                        ClaimBountyRewards(party, journal);
                        break;
                    case "5":
                        GenerateNewBounty(party);
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

        private void ViewAvailableBounties()
        {
            Console.WriteLine("\n=== Available Bounties ===");
            if (_activeBounties.Count == 0)
            {
                Console.WriteLine("No bounties available at this time.");
                return;
            }

            for (int i = 0; i < _activeBounties.Count; i++)
            {
                var bounty = _activeBounties[i];
                var difficultySymbol = bounty.Difficulty switch
                {
                    BountyDifficulty.Common => "●",
                    BountyDifficulty.Uncommon => "◆",
                    BountyDifficulty.Rare => "★",
                    BountyDifficulty.Legendary => "☆",
                    _ => "○"
                };

                Console.WriteLine($"\n{i + 1}) {difficultySymbol} [{bounty.Difficulty}] {bounty.TargetName}");
                Console.WriteLine($"   {bounty.Description}");
                Console.WriteLine($"   Reward: {bounty.GoldReward} gold, {bounty.ExperienceReward} XP");
            }
        }

        private void ViewActiveBounties(Journal? journal)
        {
            Console.WriteLine("\n=== Your Active Bounties ===");

            var activeBounties = journal?.GetActiveBounties() ?? new List<Bounty>();

            if (activeBounties.Count == 0)
            {
                Console.WriteLine("You have no active bounties. Accept one from the available bounties!");
                return;
            }

            foreach (var bounty in activeBounties.OrderBy(b => b.Difficulty))
            {
                var difficultySymbol = bounty.Difficulty switch
                {
                    BountyDifficulty.Common => "●",
                    BountyDifficulty.Uncommon => "◆",
                    BountyDifficulty.Rare => "★",
                    BountyDifficulty.Legendary => "☆",
                    _ => "○"
                };

                var status = bounty.IsCompleted ? "✓ COMPLETE - Claim reward!" : "○ Hunt in dungeons";
                Console.WriteLine($"\n{difficultySymbol} [{bounty.Difficulty}] {bounty.TargetName}");
                Console.WriteLine($"  {bounty.Description}");
                Console.WriteLine($"  Reward: {bounty.GoldReward} gold, {bounty.ExperienceReward} XP");
                Console.WriteLine($"  Status: {status}");
            }
        }

        #endregion

        #region Reward Management

        private void AcceptBounty(List<Character> party, Journal? journal)
        {
            Console.WriteLine("\n=== Accept a Bounty ===");
            if (_activeBounties.Count == 0)
            {
                Console.WriteLine("No bounties available to accept.");
                return;
            }

            for (int i = 0; i < _activeBounties.Count; i++)
            {
                var bounty = _activeBounties[i];
                var difficultySymbol = bounty.Difficulty switch
                {
                    BountyDifficulty.Common => "●",
                    BountyDifficulty.Uncommon => "◆",
                    BountyDifficulty.Rare => "★",
                    BountyDifficulty.Legendary => "☆",
                    _ => "○"
                };

                Console.WriteLine($"\n{i + 1}) {difficultySymbol} [{bounty.Difficulty}] {bounty.TargetName}");
                Console.WriteLine($"   {bounty.Description}");
                Console.WriteLine($"   Reward: {bounty.GoldReward} gold, {bounty.ExperienceReward} XP");
            }

            Console.Write("\nAccept which bounty? (number or 0 to cancel): ");
            var input = Console.ReadLine() ?? string.Empty;
            if (!int.TryParse(input, out var idx) || idx < 1 || idx > _activeBounties.Count)
            {
                Console.WriteLine("Cancelled.");
                return;
            }

            var selectedBounty = _activeBounties[idx - 1];

            // Add to journal if available
            if (journal != null)
            {
                journal.AddActiveBounty(selectedBounty);
            }

            Console.WriteLine($"\n🎯 Bounty accepted: {selectedBounty.TargetName}");
            Console.WriteLine($"Hunt this target in the dungeons to complete the bounty!");
            Console.WriteLine("The bounty has been added to your journal.");
        }

        private void ClaimBountyRewards(List<Character> party, Journal? journal)
        {
            var activeBounties = journal?.GetActiveBounties() ?? new List<Bounty>();
            var unclaimedBounties = activeBounties.Where(b => b.IsCompleted && !b.IsClaimed).ToList();

            if (unclaimedBounties.Count == 0)
            {
                Console.WriteLine("No bounty rewards to claim!");
                return;
            }

            Console.WriteLine("\n=== Claim Bounty Rewards ===");
            for (int i = 0; i < unclaimedBounties.Count; i++)
            {
                var bounty = unclaimedBounties[i];
                Console.WriteLine($"{i + 1}) {bounty.TargetName} [{bounty.Difficulty}] - {bounty.GoldReward}g, {bounty.ExperienceReward} XP");
            }

            Console.Write("\nClaim which bounty? ");
            var input = Console.ReadLine() ?? string.Empty;
            if (!int.TryParse(input, out var idx) || idx < 1 || idx > unclaimedBounties.Count) return;

            var selectedBounty = unclaimedBounties[idx - 1];

            Console.WriteLine("\nWho will receive the rewards?");
            for (int i = 0; i < party.Count; i++)
            {
                Console.WriteLine($"{i + 1}) {party[i].Name} (Lv {party[i].Level})");
            }

            var whoInput = Console.ReadLine() ?? string.Empty;
            if (!int.TryParse(whoInput, out var whoIdx) || whoIdx < 1 || whoIdx > party.Count) return;

            var character = party[whoIdx - 1];
            character.Inventory.AddGold(selectedBounty.GoldReward);
            character.GainExperience(selectedBounty.ExperienceReward);
            selectedBounty.IsClaimed = true;

            Console.WriteLine($"\n💰 {character.Name} claimed the bounty for {selectedBounty.TargetName}!");
            Console.WriteLine($"Received: {selectedBounty.GoldReward} gold, {selectedBounty.ExperienceReward} XP");

            // Move to completed in journal
            if (journal != null)
            {
                journal.CompleteBounty(selectedBounty);
            }
        }

        #endregion

        #region Bounty Generation

        private void GenerateNewBounty(List<Character> party)
        {
            Console.WriteLine("\nThe bounty master can generate a new random bounty for 50 gold.");
            Console.WriteLine("Who will pay?");
            for (int i = 0; i < party.Count; i++)
            {
                Console.WriteLine($"{i + 1}) {party[i].Name} - Gold: {party[i].Inventory.Gold}");
            }

            var input = Console.ReadLine() ?? string.Empty;
            if (!int.TryParse(input, out var idx) || idx < 1 || idx > party.Count) return;

            var payer = party[idx - 1];
            if (!payer.Inventory.SpendGold(50))
            {
                Console.WriteLine("Not enough gold!");
                return;
            }

            var newBounty = CreateRandomBounty();
            _activeBounties.Add(newBounty);

            Console.WriteLine($"\n📜 New bounty posted: {newBounty.TargetName} [{newBounty.Difficulty}]");
            Console.WriteLine($"{newBounty.Description}");
            Console.WriteLine($"Reward: {newBounty.GoldReward}g, {newBounty.ExperienceReward} XP");
        }

        private Bounty CreateRandomBounty()
        {
            var difficulty = (BountyDifficulty)_random.Next(0, 4);

            var targets = new[]
            {
                ("Rogue Warrior", "A deserter from the army now leads bandits."),
                ("Cave Troll", "A massive troll terrorizes miners."),
                ("Necromancer", "A dark wizard raises the dead."),
                ("Giant Spider", "An enormous spider hunts travelers."),
                ("Vampire Lord", "A bloodthirsty vampire preys on villagers."),
                ("Demon", "A demon has escaped from the underworld."),
                ("Corrupted Knight", "A once-noble knight now serves evil."),
                ("Basilisk", "A deadly serpent with a petrifying gaze.")
            };

            var target = targets[_random.Next(targets.Length)];

            int baseGold = difficulty switch
            {
                BountyDifficulty.Common => _random.Next(40, 80),
                BountyDifficulty.Uncommon => _random.Next(120, 200),
                BountyDifficulty.Rare => _random.Next(250, 400),
                BountyDifficulty.Legendary => _random.Next(800, 1500),
                _ => 50
            };

            int baseXP = (int)(baseGold * 1.5);

            return new Bounty(target.Item1, target.Item2, difficulty, baseGold, baseXP);
        }

        #endregion

        #region Completion Tracking

        public void CheckBountyCompletion(string enemyName, Journal? journal = null)
        {
            var bountiesToCheck = journal?.GetActiveBounties() ?? _activeBounties;

            foreach (var bounty in bountiesToCheck.Where(b => !b.IsCompleted))
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

        public List<Bounty> GetActiveBounties() => _activeBounties;

        #endregion
    }
}
