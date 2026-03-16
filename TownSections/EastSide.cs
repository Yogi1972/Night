using Night.Characters;
using System;
using System.Collections.Generic;

namespace Rpg_Dungeon
{
    internal class EastSide
    {
        #region Fields

        private QuestBoard? _questBoard;
        private BountyBoard? _bountyBoard;
        private Weather? _weather;
        private TimeOfDay? _timeTracker;
        private Journal? _journal;

        #endregion

        #region Setup Methods

        public void SetTrackers(QuestBoard? questBoard, BountyBoard? bountyBoard)
        {
            _questBoard = questBoard;
            _bountyBoard = bountyBoard;
        }

        public void SetJournal(Journal? journal)
        {
            _journal = journal;
        }

        public void SetWeather(Weather? weather)
        {
            _weather = weather;
        }

        public void SetTimeTracker(TimeOfDay? timeTracker)
        {
            _timeTracker = timeTracker;
        }

        #endregion

        #region Public Methods

        public void EnterEastSide(List<Character> party)
        {
            Console.WriteLine("\n╔════════════════════════════════════════╗");
            Console.WriteLine("║      East Side - Service Quarter      ║");
            Console.WriteLine("╚════════════════════════════════════════╝");
            Console.WriteLine("The organized district where adventurers handle business.");
            Console.WriteLine("Guilds, inns, and banks line the streets.\n");

            while (true)
            {
                Console.WriteLine("\n--- East Side: Service Quarter ---");
                Console.WriteLine("1) The Cozy Inn (Rest & Recovery)");
                Console.WriteLine("2) GreyWolf Bank & Storage");
                Console.WriteLine("3) Adventurer's Guild (Quest Board)");
                Console.WriteLine("4) Hunter's Lodge (Bounty Board)");
                Console.WriteLine("5) Training Hall (Level Up Faster) ⚔️🏆");
                Console.WriteLine("0) Return to Town Square");
                Console.Write("Choice: ");

                var choice = Console.ReadLine() ?? string.Empty;

                switch (choice.Trim())
                {
                    case "1":
                        VisitInn(party);
                        break;
                    case "2":
                        VisitBank(party);
                        break;
                    case "3":
                        VisitQuestBoard(party);
                        break;
                    case "4":
                        VisitBountyBoard(party);
                        break;
                    case "5":
                        VisitTrainingHall(party);
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

        #region Private Visit Methods

        private void VisitInn(List<Character> party)
        {
            Console.WriteLine("\n╔════════════════════════════════════════╗");
            Console.WriteLine("║      The Cozy Inn (Rest & Recovery)   ║");
            Console.WriteLine("╚════════════════════════════════════════╝");
            Console.WriteLine("A warm fireplace crackles in the corner.");
            Console.WriteLine("The innkeeper greets you with a smile.\n");

            while (true)
            {
                Console.WriteLine("'What can I do for you?'");
                Console.WriteLine("1) Rest for the night (Free - Full recovery)");
                Console.WriteLine("2) Premium Suite (25 gold - Full recovery + bonus)");
                Console.WriteLine("3) Quick nap (10 gold - Partial recovery)");
                Console.WriteLine("0) Leave inn");
                Console.Write("Choice: ");

                var choice = Console.ReadLine() ?? string.Empty;

                switch (choice.Trim())
                {
                    case "1":
                        RestAtInn(party, RestType.Standard);
                        break;
                    case "2":
                        RestAtInn(party, RestType.Premium);
                        break;
                    case "3":
                        RestAtInn(party, RestType.Quick);
                        break;
                    case "0":
                        Console.WriteLine("'Safe travels!'");
                        return;
                    default:
                        Console.WriteLine("Invalid choice.");
                        break;
                }
            }
        }

        private enum RestType
        {
            Standard,
            Premium,
            Quick
        }

        private void RestAtInn(List<Character> party, RestType restType)
        {
            int cost = 0;
            string description = "";

            switch (restType)
            {
                case RestType.Standard:
                    cost = 0;
                    description = "You settle into a modest room for the night.";
                    break;
                case RestType.Premium:
                    cost = 25;
                    description = "You enjoy a luxurious suite with fine linens and a hearty meal.";
                    break;
                case RestType.Quick:
                    cost = 10;
                    description = "You take a quick nap in a quiet corner of the inn.";
                    break;
            }

            if (cost > 0)
            {
                Console.WriteLine($"This will cost {cost} gold. Who will pay?");
                for (int i = 0; i < party.Count; i++)
                {
                    Console.WriteLine($"{i + 1}) {party[i].Name} - Gold: {party[i].Inventory.Gold}");
                }

                var who = Console.ReadLine() ?? string.Empty;
                if (!int.TryParse(who, out var whoIdx) || whoIdx < 1 || whoIdx > party.Count)
                {
                    Console.WriteLine("Invalid choice.");
                    return;
                }

                var payer = party[whoIdx - 1];
                if (!payer.Inventory.SpendGold(cost))
                {
                    Console.WriteLine("Not enough gold!");
                    return;
                }

                Console.WriteLine($"{payer.Name} paid {cost} gold.");
            }

            Console.WriteLine(description);
            Console.WriteLine();

            foreach (var member in party)
            {
                if (member == null) continue;

                if (!member.IsAlive)
                {
                    Console.WriteLine($"{member.Name} is down and cannot benefit from rest.");
                    continue;
                }

                int oldHealth = member.Health;
                int oldMana = member.Mana;

                switch (restType)
                {
                    case RestType.Standard:
                        member.Heal(member.MaxHealth);
                        member.RestoreMana(member.MaxMana);
                        break;
                    case RestType.Premium:
                        member.Heal(member.MaxHealth);
                        member.RestoreMana(member.MaxMana);
                        Console.WriteLine($"{member.Name} feels refreshed and invigorated!");
                        break;
                    case RestType.Quick:
                        int quickHeal = member.MaxHealth / 2;
                        int quickMana = member.MaxMana / 2;
                        member.Heal(quickHeal);
                        member.RestoreMana(quickMana);
                        break;
                }

                int healthRestored = member.Health - oldHealth;
                int manaRestored = member.Mana - oldMana;

                Console.WriteLine($"{member.Name}: HP {oldHealth} → {member.Health} (+{healthRestored}), Mana {oldMana} → {member.Mana} (+{manaRestored})");
            }

            Console.WriteLine("\nYour party is well rested!");

            // Inn rest advances time (8 hours for standard/premium, 2 for quick)
            if (_timeTracker != null)
            {
                int hoursToAdvance = restType == RestType.Quick ? 2 : 8;
                _timeTracker.AdvanceTime(hoursToAdvance);
            }
        }

        private void VisitBank(List<Character> party)
        {
            Console.WriteLine("\n╔════════════════════════════════════════╗");
            Console.WriteLine("║         GreyWolf Bank & Storage       ║");
            Console.WriteLine("╚════════════════════════════════════════╝");
            Console.WriteLine("'Welcome! Your gold is safe with us.'\n");

            Bank bank = new Bank();
            bank.OpenBank(party);
        }

        private void VisitQuestBoard(List<Character> party)
        {
            Console.WriteLine("\n╔════════════════════════════════════════╗");
            Console.WriteLine("║       Adventurer's Guild - Quests     ║");
            Console.WriteLine("╚════════════════════════════════════════╝");
            Console.WriteLine("A weathered board displays various contracts.\n");

            if (_questBoard == null)
            {
                Console.WriteLine("Quest system unavailable!");
                return;
            }
            _questBoard.OpenQuestBoard(party, _journal, _weather, _timeTracker);
        }

        private void VisitBountyBoard(List<Character> party)
        {
            Console.WriteLine("\n╔════════════════════════════════════════╗");
            Console.WriteLine("║       Hunter's Lodge - Bounties       ║");
            Console.WriteLine("╚════════════════════════════════════════╝");
            Console.WriteLine("A board covered with wanted posters.\n");

            if (_bountyBoard == null)
            {
                Console.WriteLine("Bounty system unavailable!");
                return;
            }
            _bountyBoard.OpenBountyBoard(party, _journal);
        }

        private void VisitTrainingHall(List<Character> party)
        {
            Console.WriteLine("\n╔════════════════════════════════════════╗");
            Console.WriteLine("║    Training Hall - Academy of Mastery ║");
            Console.WriteLine("╚════════════════════════════════════════╝");
            Console.WriteLine("⚔️ 'Welcome to the Training Hall!'");
            Console.WriteLine("🏆 'We have trainers for all classes—base and champion alike!'\n");

            TrainingHall trainingHall = new TrainingHall();
            trainingHall.EnterTrainingHall(party);
        }

        #endregion
    }
}
