using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using Night.Characters;
using Rpg_Dungeon;

namespace Night.Systems
{
    /// <summary>
    /// BACKUP - Simple version of Program.cs if the fancy version has issues
    /// Copy this code to replace Main() and related methods if needed
    /// </summary>
    internal static class ProgramBackup
    {
        private static void MainSimple()
        {
            Console.WriteLine("RPG DUNGEON CRAWLER");
            Console.WriteLine("===================");
            Console.WriteLine();

            while (true)
            {
                Console.WriteLine("MAIN MENU:");
                Console.WriteLine("1) Start New Game");
                Console.WriteLine("2) Load Saved Game");
                Console.WriteLine("3) Start Multiplayer Game");
                Console.WriteLine("0) Exit");
                Console.Write("Choose: ");

                var choice = Console.ReadLine() ?? string.Empty;

                switch (choice.Trim())
                {
                    case "1":
                        Console.WriteLine("\nStarting new game...");
                        StartNewGameSimple();
                        return;

                    case "2":
                        Console.WriteLine("\nLoading game...");
                        var loaded = Options.ShowOptions(new List<Character>());
                        if (loaded != null)
                        {
                            Console.WriteLine("Game loaded!");
                            Thread.Sleep(500);
                            RunGameLoopSimple(loaded);
                            return;
                        }
                        Console.WriteLine("No save found.");
                        break;

                    case "3":
                        Console.WriteLine("\nStarting multiplayer...");
                        StartNewGameSimple();
                        return;

                    case "0":
                        Console.WriteLine("Thanks for playing!");
                        Environment.Exit(0);
                        break;

                    default:
                        Console.WriteLine("Invalid choice.");
                        break;
                }
                Console.WriteLine();
            }
        }

        private static void StartNewGameSimple()
        {
            Console.WriteLine("\nNEW GAME");
            Console.Write("Enter party size (1-4): ");
            var input = Console.ReadLine();
            if (!int.TryParse(input, out int partySize) || partySize <= 0 || partySize > 4)
            {
                Console.WriteLine("Invalid. Using 1.");
                partySize = 1;
            }

            var party = new List<Character>(partySize);

            for (int i = 0; i < partySize; i++)
            {
                Console.WriteLine($"\nCreate character {i + 1}:");
                Console.Write("Name: ");
                var name = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(name)) name = $"Character{i + 1}";

                Console.WriteLine("Class: 1) Warrior 2) Mage 3) Rogue 4) Priest");
                var cls = Console.ReadLine() ?? "1";

                Character c = cls.Trim() switch
                {
                    "2" => new Mage(name),
                    "3" => new Rogue(name),
                    "4" => new Priest(name),
                    _ => new Warrior(name)
                };

                c.ApplyRaceBonuses(new Human());
                c.Inventory.AddGold(50);
                party.Add(c);
                Console.WriteLine($"Created: {c.Name} the {c.GetType().Name}");
            }

            Console.WriteLine($"\nParty created with {party.Count} member(s)!");
            Thread.Sleep(1000);
            RunGameLoopSimple(party);
        }

        private static void RunGameLoopSimple(List<Character> party)
        {
            Console.WriteLine("\nGame loop would start here...");
            Console.WriteLine("(This is the simple backup version)");
            Console.WriteLine("Press Enter to continue to actual game...");
            Console.ReadLine();

            // Call the real RunGameLoop from Program class
            // You would need to make it public or call through reflection
        }
    }
}
