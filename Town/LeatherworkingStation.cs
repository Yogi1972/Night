using Night.Characters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rpg_Dungeon
{
    internal static class LeatherworkingStation
    {
        public static void Open(List<Character> party)
        {
            var leatherworkers = party.Where(p => ProfessionManager.CanCraftWithProfession(p, CraftingProfession.Leatherworking)).ToList();

            if (leatherworkers.Count == 0)
            {
                Console.WriteLine("\n⚠️ No party members have Leatherworking profession!");
                Console.WriteLine("Press Enter to continue...");
                Console.ReadLine();
                return;
            }

            while (true)
            {
                Console.WriteLine("\n╔════════════════════════════════════════════════════════════════════╗");
                Console.WriteLine("║                    🧵 LEATHERWORKING STATION                        ║");
                Console.WriteLine("╚════════════════════════════════════════════════════════════════════╝");
                Console.WriteLine("\n1) Craft Light Armor");
                Console.WriteLine("2) Craft Leather Accessories");
                Console.WriteLine("3) Craft Bags & Storage");
                Console.WriteLine("4) Craft Tanning Materials");
                Console.WriteLine("5) View Leatherworking Recipes");
                Console.WriteLine("0) Back");
                Console.Write("Choice: ");

                var choice = Console.ReadLine() ?? string.Empty;

                switch (choice.Trim())
                {
                    case "1":
                        CraftArmor(leatherworkers);
                        break;
                    case "2":
                        CraftAccessories(leatherworkers);
                        break;
                    case "3":
                        CraftBags(leatherworkers);
                        break;
                    case "4":
                        CraftTanningMaterials(leatherworkers);
                        break;
                    case "5":
                        RecipeDisplay.ShowLeatherworkingRecipes();
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

        private static void CraftArmor(List<Character> leatherworkers)
        {
            Console.WriteLine("\n=== LEATHER ARMOR ===");
            Console.WriteLine("1) Leather Vest - 2x Wolf Pelt + 1x Iron Nails (Agi +4, HP +12, Dur: 45)");
            Console.WriteLine("2) Studded Leather - 3x Deer Hide + 2x Iron Nails + 1x Metal Clasp (Agi +6, HP +18, Dur: 60)");
            Console.WriteLine("3) Ranger's Armor - 4x Wolf Pelt + 2x Spider Silk + 1x Steel Chain (Agi +8, Str +3, HP +25, Dur: 75)");
            Console.WriteLine("4) Shadow Leather - 3x Deer Hide + 2x Desert Chitin + 1x Obsidian (Agi +10, Intel +4, Dur: 70)");
            Console.WriteLine("5) Scaled Armor - 5x Fish Scales + 2x Wolf Pelt + 1x Aqua Crystal (Agi +7, HP +30, Armor +5, Dur: 80)");
            Console.WriteLine("0) Cancel");
            Console.Write("Choose armor to craft: ");

            var choice = Console.ReadLine() ?? string.Empty;
            if (choice == "0") return;

            var crafter = CraftingHelper.SelectCrafterFromList(leatherworkers);
            if (crafter == null) return;

            bool success = false;
            switch (choice)
            {
                case "1":
                    success = CraftingHelper.CraftItem(crafter,
                        new[] { ("Wolf Pelt", 2), ("Iron Nails", 1) },
                        new Equipment("Leather Vest", EquipmentType.Armor, 45, 80, agi: 4, hp: 12),
                        "Leather Vest", CraftingProfession.Leatherworking);
                    break;
                case "2":
                    success = CraftingHelper.CraftItem(crafter,
                        new[] { ("Deer Hide", 3), ("Iron Nails", 2), ("Metal Clasp", 1) },
                        new Equipment("Studded Leather", EquipmentType.Armor, 60, 110, agi: 6, hp: 18),
                        "Studded Leather", CraftingProfession.Leatherworking);
                    break;
                case "3":
                    success = CraftingHelper.CraftItem(crafter,
                        new[] { ("Wolf Pelt", 4), ("Spider Silk", 2), ("Steel Chain", 1) },
                        new Equipment("Ranger's Armor", EquipmentType.Armor, 75, 150, agi: 8, str: 3, hp: 25),
                        "Ranger's Armor", CraftingProfession.Leatherworking);
                    break;
                case "4":
                    success = CraftingHelper.CraftItem(crafter,
                        new[] { ("Deer Hide", 3), ("Desert Chitin", 2), ("Obsidian", 1) },
                        new Equipment("Shadow Leather", EquipmentType.Armor, 70, 140, agi: 10, intel: 4),
                        "Shadow Leather", CraftingProfession.Leatherworking);
                    break;
                case "5":
                    success = CraftingHelper.CraftItem(crafter,
                        new[] { ("Fish Scales", 5), ("Wolf Pelt", 2), ("Aqua Crystal", 1) },
                        new Equipment("Scaled Armor", EquipmentType.Armor, 80, 160, agi: 7, hp: 30, armor: 5),
                        "Scaled Armor", CraftingProfession.Leatherworking);
                    break;
            }

            if (success)
                Console.WriteLine($"\n✅ {crafter.Name} successfully crafted the armor!");
        }

        private static void CraftAccessories(List<Character> leatherworkers)
        {
            Console.WriteLine("\n=== LEATHER ACCESSORIES ===");
            Console.WriteLine("1) Leather Belt - 2x Wolf Pelt + 1x Metal Clasp (Str +2, HP +8, Dur: 40)");
            Console.WriteLine("2) Hide Bracers - 2x Deer Hide + 1x Iron Nails (Agi +4, Armor +2, Dur: 45)");
            Console.WriteLine("3) Scaled Gloves - 3x Fish Scales + 1x Cotton (Agi +5, Dur: 50)");
            Console.WriteLine("4) Chitin Pauldrons - 3x Desert Chitin + 1x Steel Chain (Str +4, Armor +4, Dur: 60)");
            Console.WriteLine("0) Cancel");
            Console.Write("Choose accessory to craft: ");

            var choice = Console.ReadLine() ?? string.Empty;
            if (choice == "0") return;

            var crafter = CraftingHelper.SelectCrafterFromList(leatherworkers);
            if (crafter == null) return;

            bool success = false;
            switch (choice)
            {
                case "1":
                    success = CraftingHelper.CraftItem(crafter,
                        new[] { ("Wolf Pelt", 2), ("Metal Clasp", 1) },
                        new Equipment("Leather Belt", EquipmentType.Accessory, 40, 60, str: 2, hp: 8),
                        "Leather Belt", CraftingProfession.Leatherworking);
                    break;
                case "2":
                    success = CraftingHelper.CraftItem(crafter,
                        new[] { ("Deer Hide", 2), ("Iron Nails", 1) },
                        new Equipment("Hide Bracers", EquipmentType.Accessory, 45, 65, agi: 4, armor: 2),
                        "Hide Bracers", CraftingProfession.Leatherworking);
                    break;
                case "3":
                    success = CraftingHelper.CraftItem(crafter,
                        new[] { ("Fish Scales", 3), ("Cotton", 1) },
                        new Equipment("Scaled Gloves", EquipmentType.Accessory, 50, 75, agi: 5),
                        "Scaled Gloves", CraftingProfession.Leatherworking);
                    break;
                case "4":
                    success = CraftingHelper.CraftItem(crafter,
                        new[] { ("Desert Chitin", 3), ("Steel Chain", 1) },
                        new Equipment("Chitin Pauldrons", EquipmentType.Accessory, 60, 90, str: 4, armor: 4),
                        "Chitin Pauldrons", CraftingProfession.Leatherworking);
                    break;
            }

            if (success)
                Console.WriteLine($"\n✅ {crafter.Name} successfully crafted the accessory!");
        }

        private static void CraftBags(List<Character> leatherworkers)
        {
            Console.WriteLine("\n=== BAGS & STORAGE ===");
            Console.WriteLine("1) Small Pouch - 1x Wolf Pelt + 1x Cotton (Utility Item)");
            Console.WriteLine("2) Travel Pack - 2x Deer Hide + 1x Steel Chain + 2x Cotton (Utility Item)");
            Console.WriteLine("3) Backpack - 3x Wolf Pelt + 2x Reinforced Frame (Utility Item)");
            Console.WriteLine("0) Cancel");
            Console.Write("Choose bag to craft: ");

            var choice = Console.ReadLine() ?? string.Empty;
            if (choice == "0") return;

            var crafter = CraftingHelper.SelectCrafterFromList(leatherworkers);
            if (crafter == null) return;

            bool success = false;
            switch (choice)
            {
                case "1":
                    success = CraftingHelper.CraftBackpackItem(crafter,
                        new[] { ("Wolf Pelt", 1), ("Cotton", 1) },
                        new Backpack("Small Pouch", 5, 25),
                        "Small Pouch", CraftingProfession.Leatherworking);
                    break;
                case "2":
                    success = CraftingHelper.CraftBackpackItem(crafter,
                        new[] { ("Deer Hide", 2), ("Steel Chain", 1), ("Cotton", 2) },
                        new Backpack("Travel Pack", 10, 50),
                        "Travel Pack", CraftingProfession.Leatherworking);
                    break;
                case "3":
                    success = CraftingHelper.CraftBackpackItem(crafter,
                        new[] { ("Wolf Pelt", 3), ("Reinforced Frame", 2) },
                        new Backpack("Backpack", 15, 75),
                        "Backpack", CraftingProfession.Leatherworking);
                    break;
            }

            if (success)
                Console.WriteLine($"\n✅ {crafter.Name} successfully crafted the bag!");
        }

        private static void CraftTanningMaterials(List<Character> leatherworkers)
        {
            Console.WriteLine("\n=== TANNING MATERIALS ===");
            Console.WriteLine("1) Leather Scraps - 1x Wolf Pelt (Crafting Material, x3)");
            Console.WriteLine("2) Tanning Oil - 1x Cactus Flesh + 1x Seaweed (Crafting Material)");
            Console.WriteLine("3) Treated Leather - 2x Deer Hide + 1x Tanning Oil (Crafting Material)");
            Console.WriteLine("4) Cloth - 2x Cotton + 1x Spider Silk (Crafting Material, x2)");
            Console.WriteLine("0) Cancel");
            Console.Write("Choose material to craft: ");

            var choice = Console.ReadLine() ?? string.Empty;
            if (choice == "0") return;

            var crafter = CraftingHelper.SelectCrafterFromList(leatherworkers);
            if (crafter == null) return;

            bool success = false;
            switch (choice)
            {
                case "1":
                    success = CraftingHelper.CraftGenericItem(crafter,
                        new[] { ("Wolf Pelt", 1) },
                        new GenericItem("Leather Scraps", 3),
                        "Leather Scraps", CraftingProfession.Leatherworking, 3);
                    break;
                case "2":
                    success = CraftingHelper.CraftGenericItem(crafter,
                        new[] { ("Cactus Flesh", 1), ("Seaweed", 1) },
                        new GenericItem("Tanning Oil", 8),
                        "Tanning Oil", CraftingProfession.Leatherworking);
                    break;
                case "3":
                    success = CraftingHelper.CraftGenericItem(crafter,
                        new[] { ("Deer Hide", 2), ("Tanning Oil", 1) },
                        new GenericItem("Treated Leather", 20),
                        "Treated Leather", CraftingProfession.Leatherworking);
                    break;
                case "4":
                    success = CraftingHelper.CraftGenericItem(crafter,
                        new[] { ("Cotton", 2), ("Spider Silk", 1) },
                        new GenericItem("Cloth", 10),
                        "Cloth", CraftingProfession.Leatherworking, 2);
                    break;
            }

            if (success)
                Console.WriteLine($"\n✅ {crafter.Name} successfully processed the materials!");
        }
    }
}
