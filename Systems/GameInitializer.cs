using Night.Characters;
using Rpg_Dungeon.Systems;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Rpg_Dungeon
{
    internal static class GameInitializer
    {
        public static void StartNewGame()
        {
            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                      NEW GAME CREATION                           ║");
            Console.WriteLine("╚══════════════════════════════════════════════════════════════════╝");
            Console.WriteLine();

            int? worldSeed = PromptForWorldSeed();

            if (worldSeed.HasValue)
            {
                Console.WriteLine($"\n🌍 Using world seed: {WorldGenerator.SeedToString(worldSeed.Value)}");
                Console.WriteLine("💡 You can use this seed to generate the same world again!");
            }

            int partySize = GetPartySize();
            var party = CreateParty(partySize, false);

            Console.WriteLine($"\nCreated {party.Count} characters:\n{string.Join("\n", GetPartyDescriptions(party))}");
            Thread.Sleep(2000);
            GameLoopManager.Run(party, false, worldSeed);
        }

        private static int? PromptForWorldSeed()
        {
            Console.WriteLine("╔══════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                      WORLD SEED SELECTION                        ║");
            Console.WriteLine("╚══════════════════════════════════════════════════════════════════╝");
            Console.WriteLine();
            Console.WriteLine("  🎲 World seeds determine the layout of your adventure!");
            Console.WriteLine("  🗺️  Same seed = Same world (towns, dungeons, camps)");
            Console.WriteLine("  ✨ Leave blank for a random world");
            Console.WriteLine();
            Console.Write("Enter world seed (or press Enter for random): ");

            string? input = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(input))
            {
                int randomSeed = (int)(DateTime.Now.Ticks & 0x7FFFFFFF);
                Console.WriteLine($"\n🎲 Generated random seed: {WorldGenerator.SeedToString(randomSeed)}");
                return randomSeed;
            }

            try
            {
                int seed = WorldGenerator.StringToSeed(input.Trim());
                Console.WriteLine($"✓ Seed accepted: {WorldGenerator.SeedToString(seed)}");
                return seed;
            }
            catch
            {
                Console.WriteLine("⚠️  Invalid seed format. Using random seed instead.");
                int randomSeed = (int)(DateTime.Now.Ticks & 0x7FFFFFFF);
                Console.WriteLine($"🎲 Generated random seed: {WorldGenerator.SeedToString(randomSeed)}");
                return randomSeed;
            }
        }

        public static void StartMultiplayerGame()
        {
            try { Console.Clear(); } catch { Console.WriteLine("\n\n"); }
            Console.WriteLine("╔══════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                   MULTIPLAYER GAME SETUP                         ║");
            Console.WriteLine("╚══════════════════════════════════════════════════════════════════╝");
            Console.WriteLine();
            Console.WriteLine("  🎮 Multiplayer allows multiple players to control party members!");
            Console.WriteLine("  📝 Each player creates their own character for the adventure.");
            Console.WriteLine();

            int? worldSeed = PromptForWorldSeed();

            if (worldSeed.HasValue)
            {
                Console.WriteLine($"\n🌍 Using world seed: {WorldGenerator.SeedToString(worldSeed.Value)}");
            }

            int partySize = GetPartySize("Enter number of players (1-4): ");
            var party = CreateParty(partySize, true);

            Console.WriteLine("\n╔══════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                    MULTIPLAYER PARTY CREATED                     ║");
            Console.WriteLine("╚══════════════════════════════════════════════════════════════════╝");
            Console.WriteLine($"\nCreated {party.Count} characters:");
            foreach (var desc in GetPartyDescriptions(party))
            {
                Console.WriteLine($"  👤 {desc}");
            }
            Console.WriteLine("\n💡 Tip: Use option 7 in the main menu to configure multiplayer settings!");
            Thread.Sleep(3000);
            GameLoopManager.Run(party, true, worldSeed);
        }

        private static int GetPartySize(string prompt = "Enter party size (1-4): ")
        {
            int partySize = 0;
            while (partySize <= 0 || partySize > 4)
            {
                Console.Write(prompt);
                var input = Console.ReadLine();
                if (!int.TryParse(input, out partySize) || partySize <= 0 || partySize > 4)
                {
                    Console.WriteLine("Invalid size. Please enter an integer between 1 and 4.");
                    partySize = 0;
                }
            }
            return partySize;
        }

        private static List<Character> CreateParty(int partySize, bool isMultiplayer)
        {
            var party = new List<Character>(partySize);

            for (int i = 0; i < partySize; i++)
            {
                if (isMultiplayer)
                {
                    Console.WriteLine($"\n━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
                    Console.WriteLine($"  PLAYER {i + 1} - Create Your Character");
                    Console.WriteLine($"━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
                }
                else
                {
                    Console.WriteLine($"Create character {i + 1} of {partySize}:");
                }

                var character = CreateCharacter(i, isMultiplayer);
                party.Add(character);
            }

            return party;
        }

        private static Character CreateCharacter(int index, bool isMultiplayer)
        {
            Console.Write("Name: ");
            var name = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(name))
                name = isMultiplayer ? $"Player{index + 1}" : $"Character{index + 1}";

            var race = SelectRace();
            var gender = SelectGender();
            var character = SelectClass(name);

            character.ApplyRaceBonuses(race);
            string bonusText = isMultiplayer ? "✓ Racial bonuses applied: " : "\nRacial bonuses applied: ";
            Console.WriteLine($"{bonusText}{race}");

            ProfessionManager.SelectProfessions(character);

            character.Inventory.AddGold(50);

            Console.WriteLine("\n━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
            Console.WriteLine("  📦 STARTING EQUIPMENT");
            Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
            StartingEquipmentGenerator.GiveStartingEquipment(character);
            Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");

            // Initialize combat abilities
            character.InitializeAbilities();
            if (character.Abilities.Count > 0)
            {
                Console.WriteLine($"\n⚔️  Learned {character.Abilities.Count} combat abilities!");
                foreach (var ability in character.Abilities)
                {
                    Console.WriteLine($"  {ability.Icon} {ability.Name}");
                }
            }

            return character;
        }

        private static Race SelectRace()
        {
            while (true)
            {
                Console.WriteLine("Choose race:\n1) Human\n2) Elf\n3) Dwarf\n4) Gnome");
                var r = Console.ReadLine() ?? string.Empty;
                if (r == "1") return new Human();
                if (r == "2") return new Elf();
                if (r == "3") return new Dwarf();
                if (r == "4") return new Gnome();
            }
        }

        private static string SelectGender()
        {
            while (true)
            {
                Console.WriteLine("Choose gender:\n1) Male\n2) Female");
                var g = Console.ReadLine() ?? string.Empty;
                if (g == "1") return "Male";
                if (g == "2") return "Female";
            }
        }

        private static Character SelectClass(string name)
        {
            while (true)
            {
                Console.WriteLine("Choose class:\n1) Warrior\n2) Mage\n3) Rogue\n4) Priest");
                var cls = Console.ReadLine() ?? string.Empty;
                if (cls == "1") return new Warrior(name);
                if (cls == "2") return new Mage(name);
                if (cls == "3") return new Rogue(name);
                if (cls == "4") return new Priest(name);
            }
        }

        private static List<string> GetPartyDescriptions(List<Character> party)
        {
            var descriptions = new List<string>();
            foreach (var c in party)
            {
                descriptions.Add($"{c.Name} ({c.GetType().Name}) - Gold: {c.Inventory.Gold}");
            }
            return descriptions;
        }

        /// <summary>
        /// Create a default party for quick multiplayer start
        /// </summary>
        public static List<Character> CreateDefaultParty()
        {
            var party = new List<Character>
            {
                new Warrior("Warrior"),
                new Mage("Mage"),
                new Rogue("Rogue"),
                new Priest("Priest")
            };

            // Apply default race and starting equipment
            foreach (var character in party)
            {
                character.ApplyRaceBonuses(new Human());
                character.Inventory.AddGold(50);
                StartingEquipmentGenerator.GiveStartingEquipment(character);
                character.InitializeAbilities();
            }

            return party;
        }
    }
}
