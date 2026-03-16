using Night.Characters;
using Night.Shops;
using System;
using System.Collections.Generic;

namespace Rpg_Dungeon
{
    internal class WestSide
    {
        #region Fields

        private readonly Shops _shops;

        #endregion

        #region Constructor

        public WestSide(Shops shops)
        {
            _shops = shops;
        }

        #endregion

        #region Public Methods

        public void EnterWestSide(List<Character> party)
        {
            Console.WriteLine("\n╔════════════════════════════════════════╗");
            Console.WriteLine("║    West Side - Entertainment District ║");
            Console.WriteLine("╚════════════════════════════════════════╝");
            Console.WriteLine("Lively music and laughter echo through the streets.");
            Console.WriteLine("Places of leisure and companionship await.\n");

            while (true)
            {
                Console.WriteLine("\n--- West Side: Entertainment District ---");
                Console.WriteLine("1) The Lucky Dragon (Gambling)");
                Console.WriteLine("2) Pet Stable (Companions)");
                Console.WriteLine("3) Fine Threads (Tailor)");
                Console.WriteLine("0) Return to Town Square");
                Console.Write("Choice: ");

                var choice = Console.ReadLine() ?? string.Empty;

                switch (choice.Trim())
                {
                    case "1":
                        VisitGamblingDen(party);
                        break;
                    case "2":
                        VisitPetStable(party);
                        break;
                    case "3":
                        VisitTailor(party);
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

        private void VisitGamblingDen(List<Character> party)
        {
            Console.WriteLine("\n╔════════════════════════════════════════╗");
            Console.WriteLine("║       The Lucky Dragon - Casino       ║");
            Console.WriteLine("╚════════════════════════════════════════╝");
            Console.WriteLine("Dice clatter and cards shuffle in this establishment.\n");

            Gambling gambling = new Gambling();
            gambling.OpenGamblingDen(party);
        }

        private void VisitPetStable(List<Character> party)
        {
            Console.WriteLine("\n╔════════════════════════════════════════╗");
            Console.WriteLine("║        Pet Stable - Companions        ║");
            Console.WriteLine("╚════════════════════════════════════════╝");
            Console.WriteLine("Various creatures look at you with hopeful eyes.\n");

            Console.WriteLine("Which party member wants to visit the pet shop?");
            for (int i = 0; i < party.Count; i++)
            {
                Console.WriteLine($"{i + 1}) {party[i].Name}");
            }
            Console.Write("Choice: ");
            var choice = Console.ReadLine() ?? string.Empty;
            if (int.TryParse(choice, out var idx) && idx >= 1 && idx <= party.Count)
            {
                PetShop petShop = new PetShop();
                petShop.OpenPetShop(party[idx - 1]);
            }
            else
            {
                Console.WriteLine("Invalid choice.");
            }
        }

        private void VisitTailor(List<Character> party)
        {
            Console.WriteLine("\n╔════════════════════════════════════════╗");
            Console.WriteLine("║         Fine Threads (Tailor)         ║");
            Console.WriteLine("╚════════════════════════════════════════╝");
            Console.WriteLine("An elegant elf arranges bolts of fine cloth.");
            Console.WriteLine("'Welcome! Need something tailored?'\n");
            _shops.OpenTailor(party);
        }

        #endregion
    }
}
