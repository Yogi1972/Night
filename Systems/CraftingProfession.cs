using Night.Characters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rpg_Dungeon
{
    #region Crafting Profession Enum

    public enum CraftingProfession
    {
        None,
        Blacksmithing,
        Leatherworking,
        Alchemy,
        Enchanting,
        Jewelcrafting,
        Herbalism
    }

    #endregion

    #region Profession Manager

    internal static class ProfessionManager
    {
        public static void SelectProfessions(Character character)
        {
            Console.WriteLine($"\n╔════════════════════════════════════════════════════════════════════╗");
            Console.WriteLine($"║               {character.Name} - SELECT CRAFTING PROFESSIONS              ║");
            Console.WriteLine($"╚════════════════════════════════════════════════════════════════════╝");
            Console.WriteLine("\n📚 You may choose 2 crafting professions to master:");
            Console.WriteLine("\n🔨 PRODUCTION PROFESSIONS (Create items):");
            Console.WriteLine("   1) Blacksmithing - Craft weapons, armor, and metal equipment");
            Console.WriteLine("   2) Leatherworking - Craft leather armor and hide equipment");
            Console.WriteLine("   3) Alchemy - Brew potions, elixirs, and consumables");
            Console.WriteLine("   4) Enchanting - Add magical properties to equipment");
            Console.WriteLine("   5) Jewelcrafting - Cut gems and craft jewelry");
            Console.WriteLine("\n🌿 GATHERING PROFESSIONS (Collect resources):");
            Console.WriteLine("   6) Herbalism - Gather herbs and plants for alchemy");

            Console.WriteLine("\n💡 TIP: Choose 1 production + 1 gathering profession for self-sufficiency!");
            Console.WriteLine("        Or choose 2 production professions for versatility!");

            character.PrimaryProfession = SelectSingleProfession("\nSelect your PRIMARY profession (1-6): ");

            CraftingProfession secondProfession;
            do
            {
                secondProfession = SelectSingleProfession("\nSelect your SECONDARY profession (1-6): ");
                if (secondProfession == character.PrimaryProfession)
                {
                    Console.WriteLine("⚠️ You must choose a different profession!");
                }
            } while (secondProfession == character.PrimaryProfession);

            character.SecondaryProfession = secondProfession;

            Console.WriteLine($"\n✅ Professions selected:");
            Console.WriteLine($"   Primary: {GetProfessionIcon(character.PrimaryProfession)} {character.PrimaryProfession}");
            Console.WriteLine($"   Secondary: {GetProfessionIcon(character.SecondaryProfession)} {character.SecondaryProfession}");
            Console.WriteLine("\n🎯 You can now craft items and gather resources based on your professions!");
            Console.WriteLine("   Use camps to search for resources matching your professions.");
        }

        private static CraftingProfession SelectSingleProfession(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                var choice = Console.ReadLine()?.Trim() ?? "";

                switch (choice)
                {
                    case "1": return CraftingProfession.Blacksmithing;
                    case "2": return CraftingProfession.Leatherworking;
                    case "3": return CraftingProfession.Alchemy;
                    case "4": return CraftingProfession.Enchanting;
                    case "5": return CraftingProfession.Jewelcrafting;
                    case "6": return CraftingProfession.Herbalism;
                    default:
                        Console.WriteLine("Invalid choice. Please select 1-6.");
                        break;
                }
            }
        }

        public static string GetProfessionIcon(CraftingProfession profession)
        {
            return profession switch
            {
                CraftingProfession.Blacksmithing => "🔨",
                CraftingProfession.Leatherworking => "🧵",
                CraftingProfession.Alchemy => "⚗️",
                CraftingProfession.Enchanting => "✨",
                CraftingProfession.Jewelcrafting => "💎",
                CraftingProfession.Herbalism => "🌿",
                _ => "❓"
            };
        }

        public static string GetProfessionDescription(CraftingProfession profession)
        {
            return profession switch
            {
                CraftingProfession.Blacksmithing => "Forge weapons and heavy armor from metal ores",
                CraftingProfession.Leatherworking => "Craft light armor and accessories from hides",
                CraftingProfession.Alchemy => "Brew potions and elixirs from herbs and reagents",
                CraftingProfession.Enchanting => "Infuse magic into equipment for bonuses",
                CraftingProfession.Jewelcrafting => "Cut gems and craft fine jewelry",
                CraftingProfession.Herbalism => "Gather plants, herbs, and natural reagents",
                _ => "No profession"
            };
        }

        public static bool CanCraftWithProfession(Character character, CraftingProfession requiredProfession)
        {
            return character.PrimaryProfession == requiredProfession ||
                   character.SecondaryProfession == requiredProfession;
        }

        public static List<CraftingProfession> GetCharacterProfessions(Character character)
        {
            var professions = new List<CraftingProfession>();
            if (character.PrimaryProfession != CraftingProfession.None)
                professions.Add(character.PrimaryProfession);
            if (character.SecondaryProfession != CraftingProfession.None)
                professions.Add(character.SecondaryProfession);
            return professions;
        }
    }

    #endregion
}
