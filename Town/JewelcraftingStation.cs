using Night.Characters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rpg_Dungeon
{
    internal static class JewelcraftingStation
    {
        public static void Open(List<Character> party)
        {
            var jewelcrafters = party.Where(p => ProfessionManager.CanCraftWithProfession(p, CraftingProfession.Jewelcrafting)).ToList();

            if (jewelcrafters.Count == 0)
            {
                Console.WriteLine("\n⚠️ No party members have Jewelcrafting profession!");
                Console.WriteLine("Press Enter to continue...");
                Console.ReadLine();
                return;
            }

            while (true)
            {
                Console.WriteLine("\n╔════════════════════════════════════════════════════════════════════╗");
                Console.WriteLine("║                    💎 JEWELCRAFTING BENCH                           ║");
                Console.WriteLine("╚════════════════════════════════════════════════════════════════════╝");
                Console.WriteLine("\n1) Craft Rings");
                Console.WriteLine("2) Craft Necklaces");
                Console.WriteLine("3) Craft Amulets");
                Console.WriteLine("4) Cut & Polish Gems");
                Console.WriteLine("5) View Jewelcrafting Recipes");
                Console.WriteLine("0) Back");
                Console.Write("Choice: ");

                var choice = Console.ReadLine() ?? string.Empty;

                switch (choice.Trim())
                {
                    case "1":
                        CraftRings(jewelcrafters);
                        break;
                    case "2":
                        CraftNecklaces(jewelcrafters);
                        break;
                    case "3":
                        CraftAmulets(jewelcrafters);
                        break;
                    case "4":
                        CutGems(jewelcrafters);
                        break;
                    case "5":
                        RecipeDisplay.ShowJewelcraftingRecipes();
                        Console.WriteLine("\nPress Enter to continue...");
                        Console.ReadLine();
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Invalid choice.");
                        break;
                }
            }
        }

        private static void CraftRings(List<Character> jewelcrafters)
        {
            Console.WriteLine("\n=== JEWELCRAFTING - RINGS ===");
            Console.WriteLine("1) Copper Band - 2x Copper Ore + 1x Small Gem (Agi +3, Dur: 30)");
            Console.WriteLine("2) Silver Ring - 2x Silver Ore + 1x Small Gem (Agi +5, Intel +3, Dur: 40)");
            Console.WriteLine("3) Golden Ring - 1x Gold Nugget + 1x Large Gem + 1x Magic Dust (All Stats +4, Dur: 50)");
            Console.WriteLine("4) Pearl Ring - 2x Pearl + 1x Silver Ore + 1x Aqua Crystal (Intel +8, Mana +15, Dur: 55)");
            Console.WriteLine("5) Dragon Ring - 1x Gold Nugget + 2x Large Gem + 1x Fire Crystal + 1x Enchanted Gem (All Stats +7, HP +20, Dur: 70)");
            Console.WriteLine("0) Cancel");
            Console.Write("Choose ring to craft: ");

            var choice = Console.ReadLine() ?? string.Empty;
            if (choice == "0") return;

            var crafter = CraftingHelper.SelectCrafterFromList(jewelcrafters);
            if (crafter == null) return;

            bool success = false;
            switch (choice)
            {
                case "1":
                    success = CraftingHelper.CraftItem(crafter,
                        new[] { ("Copper Ore", 2), ("Small Gem", 1) },
                        new Equipment("Copper Band", EquipmentType.Accessory, 30, 50, agi: 3),
                        "Copper Band", CraftingProfession.Jewelcrafting);
                    break;
                case "2":
                    success = CraftingHelper.CraftItem(crafter,
                        new[] { ("Silver Ore", 2), ("Small Gem", 1) },
                        new Equipment("Silver Ring", EquipmentType.Accessory, 40, 70, agi: 5, intel: 3),
                        "Silver Ring", CraftingProfession.Jewelcrafting);
                    break;
                case "3":
                    success = CraftingHelper.CraftItem(crafter,
                        new[] { ("Gold Nugget", 1), ("Large Gem", 1), ("Magic Dust", 1) },
                        new Equipment("Golden Ring", EquipmentType.Accessory, 50, 100, str: 4, agi: 4, intel: 4),
                        "Golden Ring", CraftingProfession.Jewelcrafting);
                    break;
                case "4":
                    success = CraftingHelper.CraftItem(crafter,
                        new[] { ("Pearl", 2), ("Silver Ore", 1), ("Aqua Crystal", 1) },
                        new Equipment("Pearl Ring", EquipmentType.Accessory, 55, 110, intel: 8, mana: 15),
                        "Pearl Ring", CraftingProfession.Jewelcrafting);
                    break;
                case "5":
                    success = CraftingHelper.CraftItem(crafter,
                        new[] { ("Gold Nugget", 1), ("Large Gem", 2), ("Fire Crystal", 1), ("Enchanted Gem", 1) },
                        new Equipment("Dragon Ring", EquipmentType.Accessory, 70, 200, str: 7, agi: 7, intel: 7, hp: 20),
                        "Dragon Ring", CraftingProfession.Jewelcrafting);
                    break;
            }

            if (success)
                Console.WriteLine($"\n✅ {crafter.Name} successfully crafted the ring!");
        }

        private static void CraftNecklaces(List<Character> jewelcrafters)
        {
            Console.WriteLine("\n=== JEWELCRAFTING - NECKLACES ===");
            Console.WriteLine("1) Fang Necklace - 3x Fang + 1x Spider Silk (Str +5, Agi +3, Dur: 35)");
            Console.WriteLine("2) Coral Pendant - 2x Coral Fragment + 1x Pearl + 1x Silver Ore (Intel +6, Mana +12, Dur: 45)");
            Console.WriteLine("3) Crystal Pendant - 2x Crystal Shard + 1x Small Gem + 1x Silver Ore (Intel +8, Mana +18, Dur: 50)");
            Console.WriteLine("4) Obsidian Amulet - 2x Obsidian + 2x Fire Crystal + 1x Gold Nugget (Str +8, HP +25, Dur: 60)");
            Console.WriteLine("0) Cancel");
            Console.Write("Choose necklace to craft: ");

            var choice = Console.ReadLine() ?? string.Empty;
            if (choice == "0") return;

            var crafter = CraftingHelper.SelectCrafterFromList(jewelcrafters);
            if (crafter == null) return;

            bool success = false;
            switch (choice)
            {
                case "1":
                    success = CraftingHelper.CraftItem(crafter,
                        new[] { ("Fang", 3), ("Spider Silk", 1) },
                        new Equipment("Fang Necklace", EquipmentType.Accessory, 35, 60, str: 5, agi: 3),
                        "Fang Necklace", CraftingProfession.Jewelcrafting);
                    break;
                case "2":
                    success = CraftingHelper.CraftItem(crafter,
                        new[] { ("Coral Fragment", 2), ("Pearl", 1), ("Silver Ore", 1) },
                        new Equipment("Coral Pendant", EquipmentType.Accessory, 45, 80, intel: 6, mana: 12),
                        "Coral Pendant", CraftingProfession.Jewelcrafting);
                    break;
                case "3":
                    success = CraftingHelper.CraftItem(crafter,
                        new[] { ("Crystal Shard", 2), ("Small Gem", 1), ("Silver Ore", 1) },
                        new Equipment("Crystal Pendant", EquipmentType.Accessory, 50, 95, intel: 8, mana: 18),
                        "Crystal Pendant", CraftingProfession.Jewelcrafting);
                    break;
                case "4":
                    success = CraftingHelper.CraftItem(crafter,
                        new[] { ("Obsidian", 2), ("Fire Crystal", 2), ("Gold Nugget", 1) },
                        new Equipment("Obsidian Amulet", EquipmentType.Accessory, 60, 120, str: 8, hp: 25),
                        "Obsidian Amulet", CraftingProfession.Jewelcrafting);
                    break;
            }

            if (success)
                Console.WriteLine($"\n✅ {crafter.Name} successfully crafted the necklace!");
        }

        private static void CraftAmulets(List<Character> jewelcrafters)
        {
            Console.WriteLine("\n=== JEWELCRAFTING - AMULETS ===");
            Console.WriteLine("1) Golden Amulet - 1x Gold Nugget + 2x Large Gem (All Stats +5, Dur: 50)");
            Console.WriteLine("2) Elemental Amulet - 1x Fire Crystal + 1x Aqua Crystal + 1x Sun Crystal + 1x Gold Nugget (All Stats +8, Mana +20, Dur: 65)");
            Console.WriteLine("3) Arcane Amulet - 2x Enchanted Gem + 1x Ancient Tablet + 1x Mystic Essence (Intel +12, Mana +35, All Stats +4, Dur: 80)");
            Console.WriteLine("0) Cancel");
            Console.Write("Choose amulet to craft: ");

            var choice = Console.ReadLine() ?? string.Empty;
            if (choice == "0") return;

            var crafter = CraftingHelper.SelectCrafterFromList(jewelcrafters);
            if (crafter == null) return;

            bool success = false;
            switch (choice)
            {
                case "1":
                    success = CraftingHelper.CraftItem(crafter,
                        new[] { ("Gold Nugget", 1), ("Large Gem", 2) },
                        new Equipment("Golden Amulet", EquipmentType.Accessory, 50, 120, str: 5, agi: 5, intel: 5),
                        "Golden Amulet", CraftingProfession.Jewelcrafting);
                    break;
                case "2":
                    success = CraftingHelper.CraftItem(crafter,
                        new[] { ("Fire Crystal", 1), ("Aqua Crystal", 1), ("Sun Crystal", 1), ("Gold Nugget", 1) },
                        new Equipment("Elemental Amulet", EquipmentType.Accessory, 65, 160, str: 8, agi: 8, intel: 8, mana: 20),
                        "Elemental Amulet", CraftingProfession.Jewelcrafting);
                    break;
                case "3":
                    success = CraftingHelper.CraftItem(crafter,
                        new[] { ("Enchanted Gem", 2), ("Ancient Tablet", 1), ("Mystic Essence", 1) },
                        new Equipment("Arcane Amulet", EquipmentType.Accessory, 80, 250, intel: 12, mana: 35, str: 4, agi: 4),
                        "Arcane Amulet", CraftingProfession.Jewelcrafting);
                    break;
            }

            if (success)
                Console.WriteLine($"\n✅ {crafter.Name} successfully crafted the amulet!");
        }

        private static void CutGems(List<Character> jewelcrafters)
        {
            Console.WriteLine("\n=== GEM CUTTING ===");
            Console.WriteLine("1) Polished Gem - 2x Small Gem (Crafting Material)");
            Console.WriteLine("2) Cut Large Gem - 1x Large Gem (Crafting Material, Improved)");
            Console.WriteLine("3) Volcanic Glass Cut - 2x Volcanic Glass (Crafting Material)");
            Console.WriteLine("4) Refined Pearl - 3x Pearl (Crafting Material)");
            Console.WriteLine("0) Cancel");
            Console.Write("Choose gem to process: ");

            var choice = Console.ReadLine() ?? string.Empty;
            if (choice == "0") return;

            var crafter = CraftingHelper.SelectCrafterFromList(jewelcrafters);
            if (crafter == null) return;

            bool success = false;
            switch (choice)
            {
                case "1":
                    success = CraftingHelper.CraftGenericItem(crafter,
                        new[] { ("Small Gem", 2) },
                        new GenericItem("Polished Gem", 25),
                        "Polished Gem", CraftingProfession.Jewelcrafting);
                    break;
                case "2":
                    success = CraftingHelper.CraftGenericItem(crafter,
                        new[] { ("Large Gem", 1) },
                        new GenericItem("Cut Large Gem", 50),
                        "Cut Large Gem", CraftingProfession.Jewelcrafting);
                    break;
                case "3":
                    success = CraftingHelper.CraftGenericItem(crafter,
                        new[] { ("Volcanic Glass", 2) },
                        new GenericItem("Volcanic Glass Cut", 35),
                        "Volcanic Glass Cut", CraftingProfession.Jewelcrafting);
                    break;
                case "4":
                    success = CraftingHelper.CraftGenericItem(crafter,
                        new[] { ("Pearl", 3) },
                        new GenericItem("Refined Pearl", 60),
                        "Refined Pearl", CraftingProfession.Jewelcrafting);
                    break;
            }

            if (success)
                Console.WriteLine($"\n✅ {crafter.Name} successfully cut the gem!");
        }
    }
}
