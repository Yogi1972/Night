using Night.Characters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rpg_Dungeon
{
    internal static class CraftingWorkshop
    {
        public static void Open(List<Character> party)
        {
            if (party == null || party.Count == 0)
            {
                Console.WriteLine("No one to craft.");
                return;
            }

            while (true)
            {
                Console.WriteLine("\n╔════════════════════════════════════════════════════════════════════╗");
                Console.WriteLine("║                    🛠️ CRAFTING WORKSHOP                             ║");
                Console.WriteLine("╚════════════════════════════════════════════════════════════════════╝");
                Console.WriteLine("\n📋 Select Crafting Station:");
                Console.WriteLine("1) 🔨 Blacksmithing Forge - Weapons & Heavy Armor");
                Console.WriteLine("2) 🧵 Leatherworking Station - Light Armor & Hide Goods");
                Console.WriteLine("3) ⚗️ Alchemy Lab - Potions & Elixirs");
                Console.WriteLine("4) ✨ Enchanting Table - Magical Enhancements");
                Console.WriteLine("5) 💎 Jewelcrafting Bench - Gems & Jewelry");
                Console.WriteLine("6) 🔥 Simple Crafting - Basic items anyone can make");
                Console.WriteLine("7) 📖 View All Recipes");
                Console.WriteLine("8) 📊 View Party Professions");
                Console.WriteLine("0) Leave Workshop");
                Console.Write("Choice: ");
                var choice = Console.ReadLine() ?? string.Empty;

                switch (choice.Trim())
                {
                    case "1":
                        BlacksmithingStation.Open(party);
                        break;
                    case "2":
                        LeatherworkingStation.Open(party);
                        break;
                    case "3":
                        AlchemyStation.Open(party);
                        break;
                    case "4":
                        EnchantingStation.Open(party);
                        break;
                    case "5":
                        JewelcraftingStation.Open(party);
                        break;
                    case "6":
                        SimpleCraftingStation.Open(party);
                        break;
                    case "7":
                        RecipeDisplay.ShowAllRecipes();
                        break;
                    case "8":
                        ViewPartyProfessions(party);
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Invalid choice.");
                        break;
                }
            }
        }

        private static void ViewPartyProfessions(List<Character> party)
        {
            Console.WriteLine("\n╔════════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                    📊 PARTY PROFESSIONS                             ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════════════╝");

            foreach (var member in party)
            {
                Console.WriteLine($"\n👤 {member.Name} ({member.GetType().Name}):");
                Console.WriteLine($"   Primary: {ProfessionManager.GetProfessionIcon(member.PrimaryProfession)} {member.PrimaryProfession}");
                Console.WriteLine($"      {ProfessionManager.GetProfessionDescription(member.PrimaryProfession)}");
                Console.WriteLine($"   Secondary: {ProfessionManager.GetProfessionIcon(member.SecondaryProfession)} {member.SecondaryProfession}");
                Console.WriteLine($"      {ProfessionManager.GetProfessionDescription(member.SecondaryProfession)}");
            }

            Console.WriteLine("\n\nPress Enter to continue...");
            Console.ReadLine();
        }
    }
}
