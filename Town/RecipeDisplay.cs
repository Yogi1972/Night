using System;

namespace Rpg_Dungeon
{
    internal static class RecipeDisplay
    {
        public static void ShowAllRecipes()
        {
            Console.WriteLine("\n╔════════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                    📖 CRAFTING RECIPE BOOK                          ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════════════╝");
            Console.WriteLine("\n1) 🔨 Blacksmithing Recipes");
            Console.WriteLine("2) 🧵 Leatherworking Recipes");
            Console.WriteLine("3) ⚗️ Alchemy Recipes");
            Console.WriteLine("4) ✨ Enchanting Recipes");
            Console.WriteLine("5) 💎 Jewelcrafting Recipes");
            Console.WriteLine("6) 📚 Resource Gathering Guide");
            Console.WriteLine("0) Back");
            Console.Write("Choice: ");

            var choice = Console.ReadLine() ?? string.Empty;

            switch (choice.Trim())
            {
                case "1":
                    ShowBlacksmithingRecipes();
                    break;
                case "2":
                    ShowLeatherworkingRecipes();
                    break;
                case "3":
                    ShowAlchemyRecipes();
                    break;
                case "4":
                    ShowEnchantingRecipes();
                    break;
                case "5":
                    ShowJewelcraftingRecipes();
                    break;
                case "6":
                    ResourceGathering.DisplayResourceGuide();
                    return;
                case "0":
                    return;
            }

            Console.WriteLine("\n\nPress Enter to continue...");
            Console.ReadLine();
        }

        public static void ShowBlacksmithingRecipes()
        {
            Console.WriteLine("\n🔨 === BLACKSMITHING RECIPES ===");
            Console.WriteLine("\n--- WEAPONS ---");
            Console.WriteLine("Iron Sword: 3x Iron Ore + 1x Wood");
            Console.WriteLine("Steel Longsword: 4x Iron Ore + 2x Silver Ore + 1x Leather Scraps");
            Console.WriteLine("Battle Axe: 5x Iron Ore + 2x Wood + 1x Large Bone");
            Console.WriteLine("Warhammer: 6x Iron Ore + 1x Mithril Ore + 1x Wood");
            Console.WriteLine("Obsidian Greatsword: 3x Obsidian + 2x Mithril Ore + 1x Fire Crystal");
            Console.WriteLine("Mithril Blade: 4x Mithril Ore + 1x Crystal Shard + 1x Small Gem");

            Console.WriteLine("\n--- ARMOR ---");
            Console.WriteLine("Chainmail: 5x Iron Ore + 2x Copper Ore");
            Console.WriteLine("Plate Armor: 8x Iron Ore + 2x Mithril Ore + 1x Leather Scraps");
            Console.WriteLine("Steel Cuirass: 6x Iron Ore + 3x Silver Ore + 1x Cotton");
            Console.WriteLine("Obsidian Plate: 4x Obsidian + 3x Mithril Ore + 2x Lava Rock");
            Console.WriteLine("Mithril Chainmail: 5x Mithril Ore + 2x Silver Ore");

            Console.WriteLine("\n--- SHIELDS ---");
            Console.WriteLine("Iron Shield: 4x Iron Ore + 2x Wood");
            Console.WriteLine("Steel Buckler: 3x Iron Ore + 1x Silver Ore + 1x Leather Scraps");
            Console.WriteLine("Tower Shield: 7x Iron Ore + 3x Mithril Ore");
            Console.WriteLine("Volcanic Bulwark: 5x Obsidian + 2x Fire Crystal + 3x Lava Rock");

            Console.WriteLine("\n--- COMPONENTS ---");
            Console.WriteLine("Iron Nails: 1x Iron Ore (makes 3x)");
            Console.WriteLine("Steel Chain: 2x Iron Ore + 1x Silver Ore");
            Console.WriteLine("Metal Clasp: 1x Copper Ore + 1x Iron Ore (makes 2x)");
            Console.WriteLine("Reinforced Frame: 3x Iron Ore + 2x Wood");
        }

        public static void ShowLeatherworkingRecipes()
        {
            Console.WriteLine("\n🧵 === LEATHERWORKING RECIPES ===");
            Console.WriteLine("\n--- ARMOR ---");
            Console.WriteLine("Leather Vest: 2x Wolf Pelt + 1x Iron Nails");
            Console.WriteLine("Studded Leather: 3x Deer Hide + 2x Iron Nails + 1x Metal Clasp");
            Console.WriteLine("Ranger's Armor: 4x Wolf Pelt + 2x Spider Silk + 1x Steel Chain");
            Console.WriteLine("Shadow Leather: 3x Deer Hide + 2x Desert Chitin + 1x Obsidian");
            Console.WriteLine("Scaled Armor: 5x Fish Scales + 2x Wolf Pelt + 1x Aqua Crystal");

            Console.WriteLine("\n--- ACCESSORIES ---");
            Console.WriteLine("Leather Belt: 2x Wolf Pelt + 1x Metal Clasp");
            Console.WriteLine("Hide Bracers: 2x Deer Hide + 1x Iron Nails");
            Console.WriteLine("Scaled Gloves: 3x Fish Scales + 1x Cotton");
            Console.WriteLine("Chitin Pauldrons: 3x Desert Chitin + 1x Steel Chain");

            Console.WriteLine("\n--- BAGS ---");
            Console.WriteLine("Small Pouch: 1x Wolf Pelt + 1x Cotton");
            Console.WriteLine("Travel Pack: 2x Deer Hide + 1x Steel Chain + 2x Cotton");
            Console.WriteLine("Backpack: 3x Wolf Pelt + 2x Reinforced Frame");

            Console.WriteLine("\n--- MATERIALS ---");
            Console.WriteLine("Leather Scraps: 1x Wolf Pelt (makes 3x)");
            Console.WriteLine("Tanning Oil: 1x Cactus Flesh + 1x Seaweed");
            Console.WriteLine("Treated Leather: 2x Deer Hide + 1x Tanning Oil");
            Console.WriteLine("Cloth: 2x Cotton + 1x Spider Silk (makes 2x)");
        }

        public static void ShowAlchemyRecipes()
        {
            Console.WriteLine("\n⚗️ === ALCHEMY RECIPES ===");
            Console.WriteLine("\n--- HEALING ---");
            Console.WriteLine("Healing Potion: 2x Healing Herb + 5g");
            Console.WriteLine("Greater Healing Potion: 1x Healing Potion + 1x Blessed Water + 1x Mana Flower + 15g");
            Console.WriteLine("Superior Healing Potion: 2x Greater Healing Potion + 1x Large Gem + 1x Essence Extract + 30g");
            Console.WriteLine("Regeneration Potion: 3x Healing Herb + 1x Rootvine + 1x Magic Dust + 20g");

            Console.WriteLine("\n--- MANA ---");
            Console.WriteLine("Potion of Mana: 2x Mana Flower + 1x Crystal Shard + 10g");
            Console.WriteLine("Greater Mana Potion: 2x Potion of Mana + 1x Aqua Crystal + 1x Arcane Dust + 20g");
            Console.WriteLine("Elixir of Arcane Power: 1x Greater Mana Potion + 1x Enchanted Gem + 1x Mystic Essence + 40g");
            Console.WriteLine("Mana Surge Potion: 3x Mana Flower + 2x Magic Dust + 1x Sun Crystal + 25g");

            Console.WriteLine("\n--- STAMINA ---");
            Console.WriteLine("Potion of Stamina: 2x Dried Herbs + 1x Cactus Flesh + 10g");
            Console.WriteLine("Greater Stamina Potion: 2x Potion of Stamina + 1x Rootvine + 1x Sulfur + 20g");
            Console.WriteLine("Elixir of Endurance: 1x Greater Stamina Potion + 1x Flame Essence + 1x Large Gem + 40g");
            Console.WriteLine("Berserker's Brew: 2x Mushroom Cap + 2x Scorpion Venom + 1x Ash Powder + 25g");

            Console.WriteLine("\n--- COMBAT ---");
            Console.WriteLine("Potion of Strength: 2x Mushroom Cap + 1x Sulfur + 1x Raw Meat + 15g");
            Console.WriteLine("Potion of Agility: 2x Dried Herbs + 1x Scorpion Venom + 1x Kelp Strand + 15g");
            Console.WriteLine("Potion of Intelligence: 2x Mana Flower + 1x Crystal Shard + 20g");
            Console.WriteLine("Potion of Fortitude: 2x Cactus Flesh + 1x Lava Rock + 1x Rootvine + 20g");
            Console.WriteLine("Elixir of the Titan: 1x Obsidian + 1x Fire Crystal + 2x Flame Essence + 50g");

            Console.WriteLine("\n--- SPECIAL ---");
            Console.WriteLine("Elixir of Fire Resistance: 2x Fire Crystal + 1x Lava Rock + 1x Healing Herb + 15g");
            Console.WriteLine("Elixir of Water Breathing: 2x Kelp Strand + 1x Fish Scales + 1x Seaweed + 15g");
            Console.WriteLine("Elixir of Vitality: 1x Healing + 1x Mana + 1x Stamina Potion + 35g");
            Console.WriteLine("Phoenix Elixir: 1x Superior Healing + 1x Gold + 1x Enchanted Gem + 1x Flame Essence + 50g");

            Console.WriteLine("\n--- REAGENTS ---");
            Console.WriteLine("Blessed Water: 1x Seaweed + 1x Healing Herb + 1x Crystal Shard + 10g");
            Console.WriteLine("Magic Dust: 2x Mana Flower + 1x Mushroom Cap (makes 2x)");
            Console.WriteLine("Arcane Dust: 1x Magic Dust + 1x Crystal Shard + 1x Aqua Crystal + 15g");
            Console.WriteLine("Essence Extract: 1x Flame Essence + 1x Sulfur + 1x Ash Powder + 15g");
            Console.WriteLine("Oil Flask: 2x Sulfur + 1x Cactus Flesh (makes 2x)");
        }

        public static void ShowEnchantingRecipes()
        {
            Console.WriteLine("\n✨ === ENCHANTING RECIPES ===");
            Console.WriteLine("\n--- WEAPONS ---");
            Console.WriteLine("Flaming Blade: 1x Iron Sword + 2x Fire Crystal + 1x Flame Essence");
            Console.WriteLine("Frost Dagger: 1x Assassin's Dagger + 2x Crystal Shard + 1x Aqua Crystal");
            Console.WriteLine("Thunder Axe: 1x Battle Axe + 2x Sun Crystal + 1x Ancient Tablet");
            Console.WriteLine("Arcane Staff: 3x Crystal Shard + 2x Arcane Dust + 1x Ancient Tablet + 1x Wood");

            Console.WriteLine("\n--- STAFFS ---");
            Console.WriteLine("Apprentice Staff: 1x Wood + 2x Magic Dust + 1x Small Gem");
            Console.WriteLine("Fire Staff: 2x Wood + 3x Fire Crystal + 1x Flame Essence");
            Console.WriteLine("Ice Staff: 2x Wood + 3x Aqua Crystal + 2x Crystal Shard");
            Console.WriteLine("Storm Staff: 2x Wood + 3x Sun Crystal + 1x Ancient Tablet");
            Console.WriteLine("Staff of the Archmage: 1x Mithril Ore + 5x Arcane Dust + 2x Large Gem + 1x Ancient Tablet");

            Console.WriteLine("\n--- ARMOR ---");
            Console.WriteLine("Mage Robes: 3x Spider Silk + 2x Magic Dust + 1x Cotton");
            Console.WriteLine("Blessed Vestments: 2x Cotton + 2x Blessed Water + 1x Pearl");
            Console.WriteLine("Archmage Robes: 4x Spider Silk + 3x Arcane Dust + 2x Crystal Shard + 1x Enchanted Thread");
            Console.WriteLine("Elemental Cloak: 2x Fire Crystal + 2x Aqua Crystal + 2x Spider Silk");

            Console.WriteLine("\n--- MATERIALS ---");
            Console.WriteLine("Enchanted Thread: 1x Spider Silk + 1x Magic Dust");
            Console.WriteLine("Enchanted Gem: 1x Large Gem + 2x Arcane Dust + 1x Crystal Shard");
            Console.WriteLine("Mystic Essence: 2x Fire Crystal + 2x Aqua Crystal + 1x Sun Crystal");
            Console.WriteLine("Spell Scroll: 3x Magic Dust + 1x Ancient Tablet");
        }

        public static void ShowJewelcraftingRecipes()
        {
            Console.WriteLine("\n💎 === JEWELCRAFTING RECIPES ===");
            Console.WriteLine("\n--- RINGS ---");
            Console.WriteLine("Copper Band: 2x Copper Ore + 1x Small Gem");
            Console.WriteLine("Silver Ring: 2x Silver Ore + 1x Small Gem");
            Console.WriteLine("Golden Ring: 1x Gold Nugget + 1x Large Gem + 1x Magic Dust");
            Console.WriteLine("Pearl Ring: 2x Pearl + 1x Silver Ore + 1x Aqua Crystal");
            Console.WriteLine("Dragon Ring: 1x Gold Nugget + 2x Large Gem + 1x Fire Crystal + 1x Enchanted Gem");

            Console.WriteLine("\n--- NECKLACES ---");
            Console.WriteLine("Fang Necklace: 3x Fang + 1x Spider Silk");
            Console.WriteLine("Coral Pendant: 2x Coral Fragment + 1x Pearl + 1x Silver Ore");
            Console.WriteLine("Crystal Pendant: 2x Crystal Shard + 1x Small Gem + 1x Silver Ore");
            Console.WriteLine("Obsidian Amulet: 2x Obsidian + 2x Fire Crystal + 1x Gold Nugget");

            Console.WriteLine("\n--- AMULETS ---");
            Console.WriteLine("Golden Amulet: 1x Gold Nugget + 2x Large Gem");
            Console.WriteLine("Elemental Amulet: 1x Fire Crystal + 1x Aqua Crystal + 1x Sun Crystal + 1x Gold Nugget");
            Console.WriteLine("Arcane Amulet: 2x Enchanted Gem + 1x Ancient Tablet + 1x Mystic Essence");

            Console.WriteLine("\n--- GEM CUTTING ---");
            Console.WriteLine("Polished Gem: 2x Small Gem");
            Console.WriteLine("Cut Large Gem: 1x Large Gem (improved)");
            Console.WriteLine("Volcanic Glass Cut: 2x Volcanic Glass");
            Console.WriteLine("Refined Pearl: 3x Pearl");
        }
    }
}
