using Night.Characters;
using Night.Shops;
using System;
using System.Collections.Generic;

namespace Rpg_Dungeon
{
    internal class NorthSide
    {
        #region Fields

        private readonly Shops _shops;

        #endregion

        #region Constructor

        public NorthSide(Shops shops)
        {
            _shops = shops;
        }

        #endregion

        #region Public Methods

        public void EnterNorthSide(List<Character> party)
        {
            Console.WriteLine("\n╔════════════════════════════════════════╗");
            Console.WriteLine("║       North Side - Crafters District  ║");
            Console.WriteLine("╚════════════════════════════════════════╝");
            Console.WriteLine("The sound of hammers and forges fills the air.");
            Console.WriteLine("Master craftsmen work their trades here.\n");

            while (true)
            {
                Console.WriteLine("\n--- North Side: Crafters District ---");
                Console.WriteLine("1) The Rusty Anvil (Blacksmith)");
                Console.WriteLine("2) Tanner's Workshop (Leather Worker)");
                Console.WriteLine("3) Golden Trinkets (Jeweler)");
                Console.WriteLine("0) Return to Town Square");
                Console.Write("Choice: ");

                var choice = Console.ReadLine() ?? string.Empty;

                switch (choice.Trim())
                {
                    case "1":
                        VisitBlacksmith(party);
                        break;
                    case "2":
                        VisitLeatherWorker(party);
                        break;
                    case "3":
                        VisitJeweler(party);
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

        #region Private Shop Methods

        private void VisitBlacksmith(List<Character> party)
        {
            Console.WriteLine("\n╔════════════════════════════════════════╗");
            Console.WriteLine("║        The Rusty Anvil (Blacksmith)   ║");
            Console.WriteLine("╚════════════════════════════════════════╝");
            Console.WriteLine("A burly dwarf looks up from his forge.");
            Console.WriteLine("'Aye, what can I do fer ye today?'\n");
            _shops.OpenBlacksmith(party);
        }

        private void VisitLeatherWorker(List<Character> party)
        {
            Console.WriteLine("\n╔════════════════════════════════════════╗");
            Console.WriteLine("║      Tanner's Workshop (Leather Work) ║");
            Console.WriteLine("╚════════════════════════════════════════╝");
            Console.WriteLine("The smell of tanned leather fills the air.");
            Console.WriteLine("'Looking for quality leather goods?'\n");
            _shops.OpenLeatherWorker(party);
        }

        private void VisitJeweler(List<Character> party)
        {
            Console.WriteLine("\n╔════════════════════════════════════════╗");
            Console.WriteLine("║      Golden Trinkets (Jeweler)        ║");
            Console.WriteLine("╚════════════════════════════════════════╝");
            Console.WriteLine("Gems and precious metals glitter in display cases.");
            Console.WriteLine("'Looking for the perfect accessory? I have rings and necklaces for every adventurer!'\n");
            _shops.OpenJeweler(party);
        }

        #endregion
    }
}
