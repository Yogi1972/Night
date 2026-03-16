using Night.Characters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rpg_Dungeon
{
    internal static class EnchantingStation
    {
        public static void Open(List<Character> party)
        {
            var enchanters = party.Where(p => ProfessionManager.CanCraftWithProfession(p, CraftingProfession.Enchanting)).ToList();

            if (enchanters.Count == 0)
            {
                Console.WriteLine("\n⚠️ No party members have Enchanting profession!");
                Console.WriteLine("Press Enter to continue...");
                Console.ReadLine();
                return;
            }

            while (true)
            {
                Console.WriteLine("\n╔════════════════════════════════════════════════════════════════════╗");
                Console.WriteLine("║                    ✨ ENCHANTING TABLE                              ║");
                Console.WriteLine("╚════════════════════════════════════════════════════════════════════╝");
                Console.WriteLine("\n1) Enchant Weapons");
                Console.WriteLine("2) Enchant Armor");
                Console.WriteLine("3) Craft Magical Staffs");
                Console.WriteLine("4) Create Enchantment Materials");
                Console.WriteLine("5) View Enchanting Recipes");
                Console.WriteLine("0) Back");
                Console.Write("Choice: ");

                var choice = Console.ReadLine() ?? string.Empty;

                switch (choice.Trim())
                {
                    case "1":
                        EnchantWeapons(enchanters);
                        break;
                    case "2":
                        EnchantArmor(enchanters);
                        break;
                    case "3":
                        CraftMagicalStaffs(enchanters);
                        break;
                    case "4":
                        CreateEnchantmentMaterials(enchanters);
                        break;
                    case "5":
                        RecipeDisplay.ShowEnchantingRecipes();
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

        private static void EnchantWeapons(List<Character> enchanters)
        {
            Console.WriteLine("\n=== ENCHANTED WEAPONS ===");
            Console.WriteLine("1) Flaming Blade - 1x Iron Sword + 2x Fire Crystal + 1x Flame Essence (Str +8, Fire Dmg, Dur: 80)");
            Console.WriteLine("2) Frost Dagger - 1x Assassin's Dagger + 2x Crystal Shard + 1x Aqua Crystal (Agi +10, Ice Dmg, Dur: 75)");
            Console.WriteLine("3) Thunder Axe - 1x Battle Axe + 2x Sun Crystal + 1x Ancient Tablet (Str +15, Lightning, Dur: 95)");
            Console.WriteLine("4) Arcane Staff - 3x Crystal Shard + 2x Arcane Dust + 1x Ancient Tablet + 1x Wood (Intel +12, Mana +30, Dur: 85)");
            Console.WriteLine("0) Cancel");
            Console.Write("Choose weapon to enchant: ");

            var choice = Console.ReadLine() ?? string.Empty;
            if (choice == "0") return;

            var crafter = CraftingHelper.SelectCrafterFromList(enchanters);
            if (crafter == null) return;

            bool success = false;
            switch (choice)
            {
                case "1":
                    success = CraftingHelper.CraftItem(crafter,
                        new[] { ("Iron Sword", 1), ("Fire Crystal", 2), ("Flame Essence", 1) },
                        new Equipment("Flaming Blade", EquipmentType.Weapon, 80, 180, str: 8),
                        "Flaming Blade", CraftingProfession.Enchanting);
                    break;
                case "2":
                    success = CraftingHelper.CraftItem(crafter,
                        new[] { ("Assassin's Dagger", 1), ("Crystal Shard", 2), ("Aqua Crystal", 1) },
                        new Equipment("Frost Dagger", EquipmentType.Weapon, 75, 160, agi: 10),
                        "Frost Dagger", CraftingProfession.Enchanting);
                    break;
                case "3":
                    success = CraftingHelper.CraftItem(crafter,
                        new[] { ("Battle Axe", 1), ("Sun Crystal", 2), ("Ancient Tablet", 1) },
                        new Equipment("Thunder Axe", EquipmentType.Weapon, 95, 220, str: 15),
                        "Thunder Axe", CraftingProfession.Enchanting);
                    break;
                case "4":
                    success = CraftingHelper.CraftItem(crafter,
                        new[] { ("Crystal Shard", 3), ("Arcane Dust", 2), ("Ancient Tablet", 1), ("Wood", 1) },
                        new Equipment("Arcane Staff", EquipmentType.Weapon, 85, 200, intel: 12, mana: 30),
                        "Arcane Staff", CraftingProfession.Enchanting);
                    break;
            }

            if (success)
                Console.WriteLine($"\n✅ {crafter.Name} successfully enchanted the weapon!");
        }

        private static void EnchantArmor(List<Character> enchanters)
        {
            Console.WriteLine("\n=== ENCHANTED ARMOR ===");
            Console.WriteLine("1) Mage Robes - 3x Spider Silk + 2x Magic Dust + 1x Cotton (Intel +8, Mana +25, Dur: 60)");
            Console.WriteLine("2) Blessed Vestments - 2x Cotton + 2x Blessed Water + 1x Pearl (Intel +7, Mana +20, HP +18, Dur: 65)");
            Console.WriteLine("3) Archmage Robes - 4x Spider Silk + 3x Arcane Dust + 2x Crystal Shard + 1x Enchanted Thread (Intel +12, Mana +40, Dur: 80)");
            Console.WriteLine("4) Elemental Cloak - 2x Fire Crystal + 2x Aqua Crystal + 2x Spider Silk (All Stats +4, Dur: 75)");
            Console.WriteLine("0) Cancel");
            Console.Write("Choose armor to enchant: ");

            var choice = Console.ReadLine() ?? string.Empty;
            if (choice == "0") return;

            var crafter = CraftingHelper.SelectCrafterFromList(enchanters);
            if (crafter == null) return;

            bool success = false;
            switch (choice)
            {
                case "1":
                    success = CraftingHelper.CraftItem(crafter,
                        new[] { ("Spider Silk", 3), ("Magic Dust", 2), ("Cotton", 1) },
                        new Equipment("Mage Robes", EquipmentType.Armor, 60, 100, intel: 8, mana: 25),
                        "Mage Robes", CraftingProfession.Enchanting);
                    break;
                case "2":
                    success = CraftingHelper.CraftItem(crafter,
                        new[] { ("Cotton", 2), ("Blessed Water", 2), ("Pearl", 1) },
                        new Equipment("Blessed Vestments", EquipmentType.Armor, 65, 110, intel: 7, mana: 20, hp: 18),
                        "Blessed Vestments", CraftingProfession.Enchanting);
                    break;
                case "3":
                    success = CraftingHelper.CraftItem(crafter,
                        new[] { ("Spider Silk", 4), ("Arcane Dust", 3), ("Crystal Shard", 2), ("Enchanted Thread", 1) },
                        new Equipment("Archmage Robes", EquipmentType.Armor, 80, 180, intel: 12, mana: 40),
                        "Archmage Robes", CraftingProfession.Enchanting);
                    break;
                case "4":
                    success = CraftingHelper.CraftItem(crafter,
                        new[] { ("Fire Crystal", 2), ("Aqua Crystal", 2), ("Spider Silk", 2) },
                        new Equipment("Elemental Cloak", EquipmentType.Armor, 75, 150, str: 4, agi: 4, intel: 4),
                        "Elemental Cloak", CraftingProfession.Enchanting);
                    break;
            }

            if (success)
                Console.WriteLine($"\n✅ {crafter.Name} successfully enchanted the armor!");
        }

        private static void CraftMagicalStaffs(List<Character> enchanters)
        {
            Console.WriteLine("\n=== MAGICAL STAFFS ===");
            Console.WriteLine("1) Apprentice Staff - 1x Wood + 2x Magic Dust + 1x Small Gem (Intel +6, Mana +15, Dur: 50)");
            Console.WriteLine("2) Fire Staff - 2x Wood + 3x Fire Crystal + 1x Flame Essence (Intel +10, Mana +25, Dur: 70)");
            Console.WriteLine("3) Ice Staff - 2x Wood + 3x Aqua Crystal + 2x Crystal Shard (Intel +10, Mana +25, Dur: 70)");
            Console.WriteLine("4) Storm Staff - 2x Wood + 3x Sun Crystal + 1x Ancient Tablet (Intel +11, Mana +28, Dur: 75)");
            Console.WriteLine("5) Staff of the Archmage - 1x Mithril Ore + 5x Arcane Dust + 2x Large Gem + 1x Ancient Tablet (Intel +15, Mana +45, All +3, Dur: 100)");
            Console.WriteLine("0) Cancel");
            Console.Write("Choose staff to craft: ");

            var choice = Console.ReadLine() ?? string.Empty;
            if (choice == "0") return;

            var crafter = CraftingHelper.SelectCrafterFromList(enchanters);
            if (crafter == null) return;

            bool success = false;
            switch (choice)
            {
                case "1":
                    success = CraftingHelper.CraftItem(crafter,
                        new[] { ("Wood", 1), ("Magic Dust", 2), ("Small Gem", 1) },
                        new Equipment("Apprentice Staff", EquipmentType.Weapon, 50, 90, intel: 6, mana: 15),
                        "Apprentice Staff", CraftingProfession.Enchanting);
                    break;
                case "2":
                    success = CraftingHelper.CraftItem(crafter,
                        new[] { ("Wood", 2), ("Fire Crystal", 3), ("Flame Essence", 1) },
                        new Equipment("Fire Staff", EquipmentType.Weapon, 70, 140, intel: 10, mana: 25),
                        "Fire Staff", CraftingProfession.Enchanting);
                    break;
                case "3":
                    success = CraftingHelper.CraftItem(crafter,
                        new[] { ("Wood", 2), ("Aqua Crystal", 3), ("Crystal Shard", 2) },
                        new Equipment("Ice Staff", EquipmentType.Weapon, 70, 140, intel: 10, mana: 25),
                        "Ice Staff", CraftingProfession.Enchanting);
                    break;
                case "4":
                    success = CraftingHelper.CraftItem(crafter,
                        new[] { ("Wood", 2), ("Sun Crystal", 3), ("Ancient Tablet", 1) },
                        new Equipment("Storm Staff", EquipmentType.Weapon, 75, 155, intel: 11, mana: 28),
                        "Storm Staff", CraftingProfession.Enchanting);
                    break;
                case "5":
                    success = CraftingHelper.CraftItem(crafter,
                        new[] { ("Mithril Ore", 1), ("Arcane Dust", 5), ("Large Gem", 2), ("Ancient Tablet", 1) },
                        new Equipment("Staff of the Archmage", EquipmentType.Weapon, 100, 300, intel: 15, mana: 45, str: 3, agi: 3),
                        "Staff of the Archmage", CraftingProfession.Enchanting);
                    break;
            }

            if (success)
                Console.WriteLine($"\n✅ {crafter.Name} successfully crafted the magical staff!");
        }

        private static void CreateEnchantmentMaterials(List<Character> enchanters)
        {
            Console.WriteLine("\n=== ENCHANTMENT MATERIALS ===");
            Console.WriteLine("1) Enchanted Thread - 1x Spider Silk + 1x Magic Dust (Crafting Material)");
            Console.WriteLine("2) Enchanted Gem - 1x Large Gem + 2x Arcane Dust + 1x Crystal Shard (Crafting Material)");
            Console.WriteLine("3) Mystic Essence - 2x Fire Crystal + 2x Aqua Crystal + 1x Sun Crystal (Crafting Material)");
            Console.WriteLine("4) Spell Scroll - 3x Magic Dust + 1x Ancient Tablet (Crafting Material)");
            Console.WriteLine("0) Cancel");
            Console.Write("Choose material to create: ");

            var choice = Console.ReadLine() ?? string.Empty;
            if (choice == "0") return;

            var crafter = CraftingHelper.SelectCrafterFromList(enchanters);
            if (crafter == null) return;

            bool success = false;
            switch (choice)
            {
                case "1":
                    success = CraftingHelper.CraftGenericItem(crafter,
                        new[] { ("Spider Silk", 1), ("Magic Dust", 1) },
                        new GenericItem("Enchanted Thread", 20),
                        "Enchanted Thread", CraftingProfession.Enchanting);
                    break;
                case "2":
                    success = CraftingHelper.CraftGenericItem(crafter,
                        new[] { ("Large Gem", 1), ("Arcane Dust", 2), ("Crystal Shard", 1) },
                        new GenericItem("Enchanted Gem", 80),
                        "Enchanted Gem", CraftingProfession.Enchanting);
                    break;
                case "3":
                    success = CraftingHelper.CraftGenericItem(crafter,
                        new[] { ("Fire Crystal", 2), ("Aqua Crystal", 2), ("Sun Crystal", 1) },
                        new GenericItem("Mystic Essence", 100),
                        "Mystic Essence", CraftingProfession.Enchanting);
                    break;
                case "4":
                    success = CraftingHelper.CraftGenericItem(crafter,
                        new[] { ("Magic Dust", 3), ("Ancient Tablet", 1) },
                        new GenericItem("Spell Scroll", 50),
                        "Spell Scroll", CraftingProfession.Enchanting);
                    break;
            }

            if (success)
                Console.WriteLine($"\n✅ {crafter.Name} successfully created the enchantment material!");
        }
    }
}
