using Night.Characters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rpg_Dungeon
{
    internal static class BlacksmithingStation
    {
        public static void Open(List<Character> party)
        {
            var smiths = party.Where(p => ProfessionManager.CanCraftWithProfession(p, CraftingProfession.Blacksmithing)).ToList();

            if (smiths.Count == 0)
            {
                Console.WriteLine("\n⚠️ No party members have Blacksmithing profession!");
                Console.WriteLine("Press Enter to continue...");
                Console.ReadLine();
                return;
            }

            while (true)
            {
                Console.WriteLine("\n╔════════════════════════════════════════════════════════════════════╗");
                Console.WriteLine("║                    🔨 BLACKSMITHING FORGE                           ║");
                Console.WriteLine("╚════════════════════════════════════════════════════════════════════╝");
                Console.WriteLine("\n1) Craft Weapons");
                Console.WriteLine("2) Craft Heavy Armor");
                Console.WriteLine("3) Craft Metal Shields");
                Console.WriteLine("4) Craft Tools & Misc");
                Console.WriteLine("5) View Blacksmithing Recipes");
                Console.WriteLine("0) Back");
                Console.Write("Choice: ");

                var choice = Console.ReadLine() ?? string.Empty;

                switch (choice.Trim())
                {
                    case "1":
                        CraftWeapons(smiths);
                        break;
                    case "2":
                        CraftArmor(smiths);
                        break;
                    case "3":
                        CraftShields(smiths);
                        break;
                    case "4":
                        CraftTools(smiths);
                        break;
                    case "5":
                        RecipeDisplay.ShowBlacksmithingRecipes();
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

        private static void CraftWeapons(List<Character> smiths)
        {
            Console.WriteLine("\n=== BLACKSMITH WEAPONS ===");
            Console.WriteLine("1) Iron Sword - 3x Iron Ore + 1x Wood (Str +5, Dur: 50)");
            Console.WriteLine("2) Steel Longsword - 4x Iron Ore + 2x Silver Ore + 1x Leather Scraps (Str +10, Dur: 70)");
            Console.WriteLine("3) Battle Axe - 5x Iron Ore + 2x Wood + 1x Large Bone (Str +12, Dur: 65)");
            Console.WriteLine("4) Warhammer - 6x Iron Ore + 1x Mithril Ore + 1x Wood (Str +14, HP +10, Dur: 80)");
            Console.WriteLine("5) Obsidian Greatsword - 3x Obsidian + 2x Mithril Ore + 1x Fire Crystal (Str +18, Dur: 90)");
            Console.WriteLine("6) Mithril Blade - 4x Mithril Ore + 1x Crystal Shard + 1x Small Gem (Str +16, Agi +5, Dur: 85)");
            Console.WriteLine("0) Cancel");
            Console.Write("Choose weapon to craft: ");

            var choice = Console.ReadLine() ?? string.Empty;
            if (choice == "0") return;

            var crafter = CraftingHelper.SelectCrafterFromList(smiths);
            if (crafter == null) return;

            bool success = false;
            switch (choice)
            {
                case "1":
                    success = CraftingHelper.CraftItem(crafter,
                        new[] { ("Iron Ore", 3), ("Wood", 1) },
                        new Equipment("Iron Sword", EquipmentType.Weapon, 50, 80, str: 5),
                        "Iron Sword", CraftingProfession.Blacksmithing);
                    break;
                case "2":
                    success = CraftingHelper.CraftItem(crafter,
                        new[] { ("Iron Ore", 4), ("Silver Ore", 2), ("Leather Scraps", 1) },
                        new Equipment("Steel Longsword", EquipmentType.Weapon, 70, 140, str: 10),
                        "Steel Longsword", CraftingProfession.Blacksmithing);
                    break;
                case "3":
                    success = CraftingHelper.CraftItem(crafter,
                        new[] { ("Iron Ore", 5), ("Wood", 2), ("Large Bone", 1) },
                        new Equipment("Battle Axe", EquipmentType.Weapon, 65, 150, str: 12),
                        "Battle Axe", CraftingProfession.Blacksmithing);
                    break;
                case "4":
                    success = CraftingHelper.CraftItem(crafter,
                        new[] { ("Iron Ore", 6), ("Mithril Ore", 1), ("Wood", 1) },
                        new Equipment("Warhammer", EquipmentType.Weapon, 80, 180, str: 14, hp: 10),
                        "Warhammer", CraftingProfession.Blacksmithing);
                    break;
                case "5":
                    success = CraftingHelper.CraftItem(crafter,
                        new[] { ("Obsidian", 3), ("Mithril Ore", 2), ("Fire Crystal", 1) },
                        new Equipment("Obsidian Greatsword", EquipmentType.Weapon, 90, 250, str: 18),
                        "Obsidian Greatsword", CraftingProfession.Blacksmithing);
                    break;
                case "6":
                    success = CraftingHelper.CraftItem(crafter,
                        new[] { ("Mithril Ore", 4), ("Crystal Shard", 1), ("Small Gem", 1) },
                        new Equipment("Mithril Blade", EquipmentType.Weapon, 85, 220, str: 16, agi: 5),
                        "Mithril Blade", CraftingProfession.Blacksmithing);
                    break;
            }

            if (success)
                Console.WriteLine($"\n✅ {crafter.Name} successfully forged the weapon!");
        }

        private static void CraftArmor(List<Character> smiths)
        {
            Console.WriteLine("\n=== BLACKSMITH ARMOR ===");
            Console.WriteLine("1) Chainmail - 5x Iron Ore + 2x Copper Ore (Str +3, HP +20, Dur: 70)");
            Console.WriteLine("2) Plate Armor - 8x Iron Ore + 2x Mithril Ore + 1x Leather Scraps (Str +8, HP +35, Dur: 100)");
            Console.WriteLine("3) Steel Cuirass - 6x Iron Ore + 3x Silver Ore + 1x Cotton (Str +6, HP +25, Dur: 85)");
            Console.WriteLine("4) Obsidian Plate - 4x Obsidian + 3x Mithril Ore + 2x Lava Rock (Str +12, HP +50, Dur: 120)");
            Console.WriteLine("5) Mithril Chainmail - 5x Mithril Ore + 2x Silver Ore (Str +10, Agi +4, HP +40, Dur: 110)");
            Console.WriteLine("0) Cancel");
            Console.Write("Choose armor to craft: ");

            var choice = Console.ReadLine() ?? string.Empty;
            if (choice == "0") return;

            var crafter = CraftingHelper.SelectCrafterFromList(smiths);
            if (crafter == null) return;

            bool success = false;
            switch (choice)
            {
                case "1":
                    success = CraftingHelper.CraftItem(crafter,
                        new[] { ("Iron Ore", 5), ("Copper Ore", 2) },
                        new Equipment("Chainmail", EquipmentType.Armor, 70, 130, str: 3, hp: 20),
                        "Chainmail", CraftingProfession.Blacksmithing);
                    break;
                case "2":
                    success = CraftingHelper.CraftItem(crafter,
                        new[] { ("Iron Ore", 8), ("Mithril Ore", 2), ("Leather Scraps", 1) },
                        new Equipment("Plate Armor", EquipmentType.Armor, 100, 200, str: 8, hp: 35),
                        "Plate Armor", CraftingProfession.Blacksmithing);
                    break;
                case "3":
                    success = CraftingHelper.CraftItem(crafter,
                        new[] { ("Iron Ore", 6), ("Silver Ore", 3), ("Cotton", 1) },
                        new Equipment("Steel Cuirass", EquipmentType.Armor, 85, 170, str: 6, hp: 25),
                        "Steel Cuirass", CraftingProfession.Blacksmithing);
                    break;
                case "4":
                    success = CraftingHelper.CraftItem(crafter,
                        new[] { ("Obsidian", 4), ("Mithril Ore", 3), ("Lava Rock", 2) },
                        new Equipment("Obsidian Plate", EquipmentType.Armor, 120, 300, str: 12, hp: 50),
                        "Obsidian Plate", CraftingProfession.Blacksmithing);
                    break;
                case "5":
                    success = CraftingHelper.CraftItem(crafter,
                        new[] { ("Mithril Ore", 5), ("Silver Ore", 2) },
                        new Equipment("Mithril Chainmail", EquipmentType.Armor, 110, 250, str: 10, agi: 4, hp: 40),
                        "Mithril Chainmail", CraftingProfession.Blacksmithing);
                    break;
            }

            if (success)
                Console.WriteLine($"\n✅ {crafter.Name} successfully forged the armor!");
        }

        private static void CraftShields(List<Character> smiths)
        {
            Console.WriteLine("\n=== BLACKSMITH SHIELDS ===");
            Console.WriteLine("1) Iron Shield - 4x Iron Ore + 2x Wood (HP +15, Armor +3, Dur: 60)");
            Console.WriteLine("2) Steel Buckler - 3x Iron Ore + 1x Silver Ore + 1x Leather Scraps (Agi +3, Armor +4, Dur: 55)");
            Console.WriteLine("3) Tower Shield - 7x Iron Ore + 3x Mithril Ore (HP +30, Armor +8, Dur: 90)");
            Console.WriteLine("4) Volcanic Bulwark - 5x Obsidian + 2x Fire Crystal + 3x Lava Rock (HP +40, Str +5, Dur: 100)");
            Console.WriteLine("0) Cancel");
            Console.Write("Choose shield to craft: ");

            var choice = Console.ReadLine() ?? string.Empty;
            if (choice == "0") return;

            var crafter = CraftingHelper.SelectCrafterFromList(smiths);
            if (crafter == null) return;

            bool success = false;
            switch (choice)
            {
                case "1":
                    success = CraftingHelper.CraftItem(crafter,
                        new[] { ("Iron Ore", 4), ("Wood", 2) },
                        new Equipment("Iron Shield", EquipmentType.OffHand, 60, 70, hp: 15, armor: 3),
                        "Iron Shield", CraftingProfession.Blacksmithing);
                    break;
                case "2":
                    success = CraftingHelper.CraftItem(crafter,
                        new[] { ("Iron Ore", 3), ("Silver Ore", 1), ("Leather Scraps", 1) },
                        new Equipment("Steel Buckler", EquipmentType.OffHand, 55, 80, agi: 3, armor: 4),
                        "Steel Buckler", CraftingProfession.Blacksmithing);
                    break;
                case "3":
                    success = CraftingHelper.CraftItem(crafter,
                        new[] { ("Iron Ore", 7), ("Mithril Ore", 3) },
                        new Equipment("Tower Shield", EquipmentType.OffHand, 90, 150, hp: 30, armor: 8),
                        "Tower Shield", CraftingProfession.Blacksmithing);
                    break;
                case "4":
                    success = CraftingHelper.CraftItem(crafter,
                        new[] { ("Obsidian", 5), ("Fire Crystal", 2), ("Lava Rock", 3) },
                        new Equipment("Volcanic Bulwark", EquipmentType.OffHand, 100, 200, hp: 40, str: 5),
                        "Volcanic Bulwark", CraftingProfession.Blacksmithing);
                    break;
            }

            if (success)
                Console.WriteLine($"\n✅ {crafter.Name} successfully forged the shield!");
        }

        private static void CraftTools(List<Character> smiths)
        {
            Console.WriteLine("\n=== BLACKSMITH TOOLS & MISC ===");
            Console.WriteLine("1) Iron Nails - 1x Iron Ore (Crafting Material, x3)");
            Console.WriteLine("2) Steel Chain - 2x Iron Ore + 1x Silver Ore (Crafting Material)");
            Console.WriteLine("3) Metal Clasp - 1x Copper Ore + 1x Iron Ore (Crafting Material, x2)");
            Console.WriteLine("4) Reinforced Frame - 3x Iron Ore + 2x Wood (Crafting Material)");
            Console.WriteLine("0) Cancel");
            Console.Write("Choose item to craft: ");

            var choice = Console.ReadLine() ?? string.Empty;
            if (choice == "0") return;

            var crafter = CraftingHelper.SelectCrafterFromList(smiths);
            if (crafter == null) return;

            bool success = false;
            switch (choice)
            {
                case "1":
                    success = CraftingHelper.CraftGenericItem(crafter,
                        new[] { ("Iron Ore", 1) },
                        new GenericItem("Iron Nails", 5),
                        "Iron Nails", CraftingProfession.Blacksmithing, 3);
                    break;
                case "2":
                    success = CraftingHelper.CraftGenericItem(crafter,
                        new[] { ("Iron Ore", 2), ("Silver Ore", 1) },
                        new GenericItem("Steel Chain", 15),
                        "Steel Chain", CraftingProfession.Blacksmithing);
                    break;
                case "3":
                    success = CraftingHelper.CraftGenericItem(crafter,
                        new[] { ("Copper Ore", 1), ("Iron Ore", 1) },
                        new GenericItem("Metal Clasp", 8),
                        "Metal Clasp", CraftingProfession.Blacksmithing, 2);
                    break;
                case "4":
                    success = CraftingHelper.CraftGenericItem(crafter,
                        new[] { ("Iron Ore", 3), ("Wood", 2) },
                        new GenericItem("Reinforced Frame", 25),
                        "Reinforced Frame", CraftingProfession.Blacksmithing);
                    break;
            }

            if (success)
                Console.WriteLine($"\n✅ {crafter.Name} successfully crafted the item!");
        }
    }
}
