using Night.Characters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rpg_Dungeon
{
    internal static class AlchemyStation
    {
        public static void Open(List<Character> party)
        {
            var alchemists = party.Where(p => ProfessionManager.CanCraftWithProfession(p, CraftingProfession.Alchemy) ||
                                             p is Mage || p is Priest).ToList();

            if (alchemists.Count == 0)
            {
                Console.WriteLine("\n⚠️ No party members have Alchemy profession (or Mage/Priest class)!");
                Console.WriteLine("Press Enter to continue...");
                Console.ReadLine();
                return;
            }

            while (true)
            {
                Console.WriteLine("\n╔════════════════════════════════════════════════════════════════════╗");
                Console.WriteLine("║                    ⚗️ ALCHEMY LABORATORY                            ║");
                Console.WriteLine("╚════════════════════════════════════════════════════════════════════╝");
                Console.WriteLine("\n1) Brew Healing Potions");
                Console.WriteLine("2) Brew Mana Potions");
                Console.WriteLine("3) Brew Stamina Potions");
                Console.WriteLine("4) Brew Combat Potions");
                Console.WriteLine("5) Brew Special Elixirs");
                Console.WriteLine("6) Process Reagents");
                Console.WriteLine("7) View Alchemy Recipes");
                Console.WriteLine("0) Back");
                Console.Write("Choice: ");

                var choice = Console.ReadLine() ?? string.Empty;

                switch (choice.Trim())
                {
                    case "1":
                        BrewHealingPotions(alchemists);
                        break;
                    case "2":
                        BrewManaPotions(alchemists);
                        break;
                    case "3":
                        BrewStaminaPotions(alchemists);
                        break;
                    case "4":
                        BrewCombatPotions(alchemists);
                        break;
                    case "5":
                        BrewSpecialElixirs(alchemists);
                        break;
                    case "6":
                        ProcessReagents(alchemists);
                        break;
                    case "7":
                        RecipeDisplay.ShowAlchemyRecipes();
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

        private static void BrewHealingPotions(List<Character> alchemists)
        {
            Console.WriteLine("\n=== HEALING POTIONS ===");
            Console.WriteLine("1) Healing Potion - 2x Healing Herb (Heals 40 HP, 5g)");
            Console.WriteLine("2) Greater Healing Potion - 1x Healing Potion + 1x Blessed Water + 1x Mana Flower (Heals 80 HP, 15g)");
            Console.WriteLine("3) Superior Healing Potion - 2x Greater Healing Potion + 1x Large Gem + 1x Essence Extract (Heals 150 HP, 30g)");
            Console.WriteLine("4) Regeneration Potion - 3x Healing Herb + 1x Rootvine + 1x Magic Dust (Heal 60 HP + Regen, 20g)");
            Console.WriteLine("0) Cancel");
            Console.Write("Choose potion to brew: ");

            var choice = Console.ReadLine() ?? string.Empty;
            if (choice == "0") return;

            var brewer = CraftingHelper.SelectCrafterFromList(alchemists);
            if (brewer == null) return;

            bool success = false;
            switch (choice)
            {
                case "1":
                    success = CraftingHelper.BrewPotion(brewer,
                        new[] { ("Healing Herb", 2) },
                        new GenericItem("Healing Potion", 25),
                        5, "Healing Potion", CraftingProfession.Alchemy);
                    break;
                case "2":
                    success = CraftingHelper.BrewPotion(brewer,
                        new[] { ("Healing Potion", 1), ("Blessed Water", 1), ("Mana Flower", 1) },
                        new GenericItem("Greater Healing Potion", 50),
                        15, "Greater Healing Potion", CraftingProfession.Alchemy);
                    break;
                case "3":
                    success = CraftingHelper.BrewPotion(brewer,
                        new[] { ("Greater Healing Potion", 2), ("Large Gem", 1), ("Essence Extract", 1) },
                        new GenericItem("Superior Healing Potion", 100),
                        30, "Superior Healing Potion", CraftingProfession.Alchemy);
                    break;
                case "4":
                    success = CraftingHelper.BrewPotion(brewer,
                        new[] { ("Healing Herb", 3), ("Rootvine", 1), ("Magic Dust", 1) },
                        new GenericItem("Regeneration Potion", 65),
                        20, "Regeneration Potion", CraftingProfession.Alchemy);
                    break;
            }

            if (success)
            {
                int bonus = brewer.Intelligence / 5;
                Console.WriteLine($"\n✅ {brewer.Name} successfully brewed the potion!");
                if (bonus > 0)
                    Console.WriteLine($"   💡 Intelligence bonus: +{bonus}% effectiveness!");
            }
        }

        private static void BrewManaPotions(List<Character> alchemists)
        {
            Console.WriteLine("\n=== MANA POTIONS ===");
            Console.WriteLine("1) Potion of Mana - 2x Mana Flower + 1x Crystal Shard (Restores 40 Mana, 10g)");
            Console.WriteLine("2) Greater Mana Potion - 2x Potion of Mana + 1x Aqua Crystal + 1x Arcane Dust (Restores 80 Mana, 20g)");
            Console.WriteLine("3) Elixir of Arcane Power - 1x Greater Mana Potion + 1x Enchanted Gem + 1x Mystic Essence (Restores 150 Mana, 40g)");
            Console.WriteLine("4) Mana Surge Potion - 3x Mana Flower + 2x Magic Dust + 1x Sun Crystal (Restore 100 Mana instantly, 25g)");
            Console.WriteLine("0) Cancel");
            Console.Write("Choose potion to brew: ");

            var choice = Console.ReadLine() ?? string.Empty;
            if (choice == "0") return;

            var brewer = CraftingHelper.SelectCrafterFromList(alchemists);
            if (brewer == null) return;

            bool success = false;
            switch (choice)
            {
                case "1":
                    success = CraftingHelper.BrewPotion(brewer,
                        new[] { ("Mana Flower", 2), ("Crystal Shard", 1) },
                        new GenericItem("Potion of Mana", 30),
                        10, "Potion of Mana", CraftingProfession.Alchemy);
                    break;
                case "2":
                    success = CraftingHelper.BrewPotion(brewer,
                        new[] { ("Potion of Mana", 2), ("Aqua Crystal", 1), ("Arcane Dust", 1) },
                        new GenericItem("Greater Mana Potion", 60),
                        20, "Greater Mana Potion", CraftingProfession.Alchemy);
                    break;
                case "3":
                    success = CraftingHelper.BrewPotion(brewer,
                        new[] { ("Greater Mana Potion", 1), ("Enchanted Gem", 1), ("Mystic Essence", 1) },
                        new GenericItem("Elixir of Arcane Power", 120),
                        40, "Elixir of Arcane Power", CraftingProfession.Alchemy);
                    break;
                case "4":
                    success = CraftingHelper.BrewPotion(brewer,
                        new[] { ("Mana Flower", 3), ("Magic Dust", 2), ("Sun Crystal", 1) },
                        new GenericItem("Mana Surge Potion", 75),
                        25, "Mana Surge Potion", CraftingProfession.Alchemy);
                    break;
            }

            if (success)
            {
                int bonus = brewer.Intelligence / 5;
                Console.WriteLine($"\n✅ {brewer.Name} successfully brewed the potion!");
                if (bonus > 0)
                    Console.WriteLine($"   💡 Intelligence bonus: +{bonus}% effectiveness!");
            }
        }

        private static void BrewStaminaPotions(List<Character> alchemists)
        {
            Console.WriteLine("\n=== STAMINA POTIONS ===");
            Console.WriteLine("1) Potion of Stamina - 2x Dried Herbs + 1x Cactus Flesh (Restores 40 Stamina, 10g)");
            Console.WriteLine("2) Greater Stamina Potion - 2x Potion of Stamina + 1x Rootvine + 1x Sulfur (Restores 80 Stamina, 20g)");
            Console.WriteLine("3) Elixir of Endurance - 1x Greater Stamina Potion + 1x Flame Essence + 1x Large Gem (Restores 150 Stamina, 40g)");
            Console.WriteLine("4) Berserker's Brew - 2x Mushroom Cap + 2x Scorpion Venom + 1x Ash Powder (Restore 100 Stamina + Str boost, 25g)");
            Console.WriteLine("0) Cancel");
            Console.Write("Choose potion to brew: ");

            var choice = Console.ReadLine() ?? string.Empty;
            if (choice == "0") return;

            var brewer = CraftingHelper.SelectCrafterFromList(alchemists);
            if (brewer == null) return;

            bool success = false;
            switch (choice)
            {
                case "1":
                    success = CraftingHelper.BrewPotion(brewer,
                        new[] { ("Dried Herbs", 2), ("Cactus Flesh", 1) },
                        new GenericItem("Potion of Stamina", 30),
                        10, "Potion of Stamina", CraftingProfession.Alchemy);
                    break;
                case "2":
                    success = CraftingHelper.BrewPotion(brewer,
                        new[] { ("Potion of Stamina", 2), ("Rootvine", 1), ("Sulfur", 1) },
                        new GenericItem("Greater Stamina Potion", 60),
                        20, "Greater Stamina Potion", CraftingProfession.Alchemy);
                    break;
                case "3":
                    success = CraftingHelper.BrewPotion(brewer,
                        new[] { ("Greater Stamina Potion", 1), ("Flame Essence", 1), ("Large Gem", 1) },
                        new GenericItem("Elixir of Endurance", 120),
                        40, "Elixir of Endurance", CraftingProfession.Alchemy);
                    break;
                case "4":
                    success = CraftingHelper.BrewPotion(brewer,
                        new[] { ("Mushroom Cap", 2), ("Scorpion Venom", 2), ("Ash Powder", 1) },
                        new GenericItem("Berserker's Brew", 75),
                        25, "Berserker's Brew", CraftingProfession.Alchemy);
                    break;
            }

            if (success)
            {
                int bonus = brewer.Intelligence / 5;
                Console.WriteLine($"\n✅ {brewer.Name} successfully brewed the potion!");
                if (bonus > 0)
                    Console.WriteLine($"   💡 Intelligence bonus: +{bonus}% effectiveness!");
            }
        }

        private static void BrewCombatPotions(List<Character> alchemists)
        {
            Console.WriteLine("\n=== COMBAT POTIONS ===");
            Console.WriteLine("1) Potion of Strength - 2x Mushroom Cap + 1x Sulfur + 1x Raw Meat (Str +5, 5 min, 15g)");
            Console.WriteLine("2) Potion of Agility - 2x Dried Herbs + 1x Scorpion Venom + 1x Kelp Strand (Agi +5, 5 min, 15g)");
            Console.WriteLine("3) Potion of Intelligence - 2x Mana Flower + 1x Crystal Shard (Intel +5, 5 min, 20g)");
            Console.WriteLine("4) Potion of Fortitude - 2x Cactus Flesh + 1x Lava Rock + 1x Rootvine (HP +30, 10 min, 20g)");
            Console.WriteLine("5) Elixir of the Titan - 1x Obsidian + 1x Fire Crystal + 2x Flame Essence (All stats +3, 15 min, 50g)");
            Console.WriteLine("0) Cancel");
            Console.Write("Choose potion to brew: ");

            var choice = Console.ReadLine() ?? string.Empty;
            if (choice == "0") return;

            var brewer = CraftingHelper.SelectCrafterFromList(alchemists);
            if (brewer == null) return;

            bool success = false;
            switch (choice)
            {
                case "1":
                    success = CraftingHelper.BrewPotion(brewer,
                        new[] { ("Mushroom Cap", 2), ("Sulfur", 1), ("Raw Meat", 1) },
                        new GenericItem("Potion of Strength", 45),
                        15, "Potion of Strength", CraftingProfession.Alchemy);
                    break;
                case "2":
                    success = CraftingHelper.BrewPotion(brewer,
                        new[] { ("Dried Herbs", 2), ("Scorpion Venom", 1), ("Kelp Strand", 1) },
                        new GenericItem("Potion of Agility", 45),
                        15, "Potion of Agility", CraftingProfession.Alchemy);
                    break;
                case "3":
                    success = CraftingHelper.BrewPotion(brewer,
                        new[] { ("Mana Flower", 2), ("Crystal Shard", 1) },
                        new GenericItem("Potion of Intelligence", 50),
                        20, "Potion of Intelligence", CraftingProfession.Alchemy);
                    break;
                case "4":
                    success = CraftingHelper.BrewPotion(brewer,
                        new[] { ("Cactus Flesh", 2), ("Lava Rock", 1), ("Rootvine", 1) },
                        new GenericItem("Potion of Fortitude", 55),
                        20, "Potion of Fortitude", CraftingProfession.Alchemy);
                    break;
                case "5":
                    success = CraftingHelper.BrewPotion(brewer,
                        new[] { ("Obsidian", 1), ("Fire Crystal", 1), ("Flame Essence", 2) },
                        new GenericItem("Elixir of the Titan", 120),
                        50, "Elixir of the Titan", CraftingProfession.Alchemy);
                    break;
            }

            if (success)
                Console.WriteLine($"\n✅ {brewer.Name} successfully brewed the combat potion!");
        }

        private static void BrewSpecialElixirs(List<Character> alchemists)
        {
            Console.WriteLine("\n=== SPECIAL ELIXIRS ===");
            Console.WriteLine("1) Elixir of Fire Resistance - 2x Fire Crystal + 1x Lava Rock + 1x Healing Herb (Resist Fire, 15g)");
            Console.WriteLine("2) Elixir of Water Breathing - 2x Kelp Strand + 1x Fish Scales + 1x Seaweed (Water Breathing, 15g)");
            Console.WriteLine("3) Elixir of Vitality - 1x Healing Potion + 1x Potion of Mana + 1x Potion of Stamina (Restore All, 35g)");
            Console.WriteLine("4) Phoenix Elixir - 1x Superior Healing Potion + 1x Gold Nugget + 1x Enchanted Gem + 1x Flame Essence (Revive, 50g)");
            Console.WriteLine("5) Elixir of the Gods - 1x Mystic Essence + 1x Arcane Amulet + 2x Ancient Tablet (Ultimate Power, 100g)");
            Console.WriteLine("0) Cancel");
            Console.Write("Choose elixir to brew: ");

            var choice = Console.ReadLine() ?? string.Empty;
            if (choice == "0") return;

            var brewer = CraftingHelper.SelectCrafterFromList(alchemists);
            if (brewer == null) return;

            bool success = false;
            switch (choice)
            {
                case "1":
                    success = CraftingHelper.BrewPotion(brewer,
                        new[] { ("Fire Crystal", 2), ("Lava Rock", 1), ("Healing Herb", 1) },
                        new GenericItem("Elixir of Fire Resistance", 60),
                        15, "Elixir of Fire Resistance", CraftingProfession.Alchemy);
                    break;
                case "2":
                    success = CraftingHelper.BrewPotion(brewer,
                        new[] { ("Kelp Strand", 2), ("Fish Scales", 1), ("Seaweed", 1) },
                        new GenericItem("Elixir of Water Breathing", 60),
                        15, "Elixir of Water Breathing", CraftingProfession.Alchemy);
                    break;
                case "3":
                    success = CraftingHelper.BrewPotion(brewer,
                        new[] { ("Healing Potion", 1), ("Potion of Mana", 1), ("Potion of Stamina", 1) },
                        new GenericItem("Elixir of Vitality", 120),
                        35, "Elixir of Vitality", CraftingProfession.Alchemy);
                    break;
                case "4":
                    success = CraftingHelper.BrewPotion(brewer,
                        new[] { ("Superior Healing Potion", 1), ("Gold Nugget", 1), ("Enchanted Gem", 1), ("Flame Essence", 1) },
                        new GenericItem("Phoenix Elixir", 200),
                        50, "Phoenix Elixir", CraftingProfession.Alchemy);
                    break;
                case "5":
                    success = CraftingHelper.BrewPotion(brewer,
                        new[] { ("Mystic Essence", 1), ("Arcane Amulet", 1), ("Ancient Tablet", 2) },
                        new GenericItem("Elixir of the Gods", 500),
                        100, "Elixir of the Gods", CraftingProfession.Alchemy);
                    break;
            }

            if (success)
            {
                Console.WriteLine($"\n✅ {brewer.Name} successfully brewed the special elixir!");
                Console.WriteLine("   💡 Note: Special effects not yet fully implemented.");
            }
        }

        private static void ProcessReagents(List<Character> alchemists)
        {
            Console.WriteLine("\n=== PROCESS REAGENTS ===");
            Console.WriteLine("1) Blessed Water - 1x Seaweed + 1x Healing Herb + 1x Crystal Shard (Crafting Material, 10g)");
            Console.WriteLine("2) Magic Dust - 2x Mana Flower + 1x Mushroom Cap (Crafting Material, x2)");
            Console.WriteLine("3) Arcane Dust - 1x Magic Dust + 1x Crystal Shard + 1x Aqua Crystal (Crafting Material, 15g)");
            Console.WriteLine("4) Essence Extract - 1x Flame Essence + 1x Sulfur + 1x Ash Powder (Crafting Material, 15g)");
            Console.WriteLine("5) Oil Flask - 2x Sulfur + 1x Cactus Flesh (Crafting Material, x2)");
            Console.WriteLine("0) Cancel");
            Console.Write("Choose reagent to process: ");

            var choice = Console.ReadLine() ?? string.Empty;
            if (choice == "0") return;

            var crafter = CraftingHelper.SelectCrafterFromList(alchemists);
            if (crafter == null) return;

            bool success = false;
            switch (choice)
            {
                case "1":
                    success = CraftingHelper.BrewPotion(crafter,
                        new[] { ("Seaweed", 1), ("Healing Herb", 1), ("Crystal Shard", 1) },
                        new GenericItem("Blessed Water", 30),
                        10, "Blessed Water", CraftingProfession.Alchemy);
                    break;
                case "2":
                    success = CraftingHelper.CraftGenericItem(crafter,
                        new[] { ("Mana Flower", 2), ("Mushroom Cap", 1) },
                        new GenericItem("Magic Dust", 25),
                        "Magic Dust", CraftingProfession.Alchemy, 2);
                    break;
                case "3":
                    success = CraftingHelper.BrewPotion(crafter,
                        new[] { ("Magic Dust", 1), ("Crystal Shard", 1), ("Aqua Crystal", 1) },
                        new GenericItem("Arcane Dust", 50),
                        15, "Arcane Dust", CraftingProfession.Alchemy);
                    break;
                case "4":
                    success = CraftingHelper.BrewPotion(crafter,
                        new[] { ("Flame Essence", 1), ("Sulfur", 1), ("Ash Powder", 1) },
                        new GenericItem("Essence Extract", 45),
                        15, "Essence Extract", CraftingProfession.Alchemy);
                    break;
                case "5":
                    success = CraftingHelper.CraftGenericItem(crafter,
                        new[] { ("Sulfur", 2), ("Cactus Flesh", 1) },
                        new GenericItem("Oil Flask", 15),
                        "Oil Flask", CraftingProfession.Alchemy, 2);
                    break;
            }

            if (success)
                Console.WriteLine($"\n✅ {crafter.Name} successfully processed the reagents!");
        }
    }
}
