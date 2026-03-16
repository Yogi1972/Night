using Night.Characters;
using Night.Shops;
using System;
using System.Collections.Generic;

namespace Rpg_Dungeon
{
    internal class SouthSide
    {
        #region Fields

        private readonly Shops _shops;
        private AchievementTracker? _achievementTracker;

        #endregion

        #region Constructor

        public SouthSide(Shops shops)
        {
            _shops = shops;
        }

        #endregion

        #region Setup Methods

        public void SetAchievementTracker(AchievementTracker? achievementTracker)
        {
            _achievementTracker = achievementTracker;
        }

        #endregion

        #region Public Methods

        public void EnterSouthSide(List<Character> party)
        {
            Console.WriteLine("\n╔════════════════════════════════════════╗");
            Console.WriteLine("║     South Side - Mystic Quarter       ║");
            Console.WriteLine("╚════════════════════════════════════════╝");
            Console.WriteLine("Mystical energy fills the air here.");
            Console.WriteLine("Healers and mages practice their arts.\n");

            while (true)
            {
                Console.WriteLine("\n--- South Side: Mystic Quarter ---");
                Console.WriteLine("1) Arcane Emporium (Mage Shop)");
                Console.WriteLine("2) Healing Hands (Apothecary)");
                Console.WriteLine("3) Hall of Fame (Achievements)");
                Console.WriteLine("4) Champion's Sanctuary (Ascension) 🏆");
                Console.WriteLine("0) Return to Town Square");
                Console.Write("Choice: ");

                var choice = Console.ReadLine() ?? string.Empty;

                switch (choice.Trim())
                {
                    case "1":
                        VisitMageShop(party);
                        break;
                    case "2":
                        VisitApothecary(party);
                        break;
                    case "3":
                        ViewAchievements(party);
                        break;
                    case "4":
                        VisitChampionSanctuary(party);
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

        private void VisitMageShop(List<Character> party)
        {
            Console.WriteLine("\n╔════════════════════════════════════════╗");
            Console.WriteLine("║     Arcane Emporium (Mage Shop)       ║");
            Console.WriteLine("╚════════════════════════════════════════╝");
            Console.WriteLine("Mystical energies swirl around the shop.");
            Console.WriteLine("'Seeking arcane wares? You've come to the right place.'\n");
            _shops.OpenMageShop(party);
        }

        private void VisitApothecary(List<Character> party)
        {
            Console.WriteLine("\n╔════════════════════════════════════════╗");
            Console.WriteLine("║      Healing Hands (Apothecary)       ║");
            Console.WriteLine("╚════════════════════════════════════════╝");
            Console.WriteLine("Shelves of potions and herbs line the walls.");
            Console.WriteLine("'Need healing supplies? I have the finest potions!'\n");
            _shops.OpenApothecary(party);
        }

        private void ViewAchievements(List<Character> party)
        {
            Console.WriteLine("\n╔════════════════════════════════════════╗");
            Console.WriteLine("║         Hall of Fame - Achievements   ║");
            Console.WriteLine("╚════════════════════════════════════════╝");
            Console.WriteLine("Your greatest accomplishments are recorded here.\n");

            if (_achievementTracker == null)
            {
                Console.WriteLine("Achievement system unavailable!");
                return;
            }

            _achievementTracker.DisplayAchievements();

            // Option to claim rewards
            Console.Write("\nClaim rewards? (y/n): ");
            var claim = Console.ReadLine() ?? string.Empty;
            if (claim.Trim().Equals("y", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("Which party member will claim rewards?");
                for (int i = 0; i < party.Count; i++)
                {
                    Console.WriteLine($"{i + 1}) {party[i].Name}");
                }
                var input = Console.ReadLine() ?? string.Empty;
                if (int.TryParse(input, out var idx) && idx >= 1 && idx <= party.Count)
                {
                    _achievementTracker.ClaimRewards(party[idx - 1]);
                }
            }
        }

        private void VisitChampionSanctuary(List<Character> party)
        {
            Console.WriteLine("\n╔════════════════════════════════════════╗");
            Console.WriteLine("║    Champion's Sanctuary - Ascension   ║");
            Console.WriteLine("╚════════════════════════════════════════╝");
            Console.WriteLine("✨ A sacred chamber filled with ancient power.");
            Console.WriteLine("🏆 Only those who have reached level 25 may ascend here.\n");

            var eligibleCharacters = new List<Character>();
            foreach (var member in party)
            {
                if (ChampionClassManager.CanSelectChampionClass(member))
                {
                    eligibleCharacters.Add(member);
                }
            }

            if (eligibleCharacters.Count == 0)
            {
                Console.WriteLine("❌ None of your party members are eligible for Champion Class ascension.");
                Console.WriteLine("   Requirements:");
                Console.WriteLine("   - Level 25 or higher");
                Console.WriteLine("   - No Champion Class already selected");
                Console.WriteLine();
                Console.WriteLine("Current party status:");
                foreach (var member in party)
                {
                    string status = member.HasChampionClass
                        ? $"[{member.ChampionClass}]"
                        : $"[Level {member.Level}/25]";
                    Console.WriteLine($"  - {member.Name} {status}");
                }
                Console.WriteLine("\nPress any key to return...");
                Console.ReadKey(true);
                return;
            }

            Console.WriteLine("✨ Eligible party members for ascension:");
            for (int i = 0; i < eligibleCharacters.Count; i++)
            {
                Console.WriteLine($"{i + 1}) {eligibleCharacters[i].Name} - Level {eligibleCharacters[i].Level} {eligibleCharacters[i].GetType().Name}");
            }
            Console.WriteLine($"{eligibleCharacters.Count + 1}) Return");
            Console.Write("\nSelect character to ascend: ");

            var input = Console.ReadLine() ?? string.Empty;
            if (int.TryParse(input, out int choice) && choice >= 1 && choice <= eligibleCharacters.Count)
            {
                ChampionClassManager.ShowChampionClassSelection(eligibleCharacters[choice - 1]);
            }
        }

        #endregion
    }
}
