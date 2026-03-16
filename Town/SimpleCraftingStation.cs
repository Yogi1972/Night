using Night.Characters;
using System;
using System.Collections.Generic;

namespace Rpg_Dungeon
{
    internal static class SimpleCraftingStation
    {
        public static void Open(List<Character> party)
        {
            Console.WriteLine("\n╔════════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                    🔥 SIMPLE CRAFTING                               ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════════════╝");
            Console.WriteLine("\nBasic items anyone can craft:");
            Console.WriteLine("1) Basic Torch - 1x Wood + 1x Cotton (Dur: 4 hours)");
            Console.WriteLine("2) Oil Torch - 1x Wood + 1x Cloth + 1x Sulfur (Dur: 6 hours)");
            Console.WriteLine("3) Campfire Kit - 3x Wood + 1x Cloth (Utility Item)");
            Console.WriteLine("4) Rope - 2x Spider Silk + 1x Cotton (Utility Item)");
            Console.WriteLine("5) Bandages - 2x Cotton + 1x Healing Herb (Heal 20 HP)");
            Console.WriteLine("0) Cancel");
            Console.Write("Choose item to craft: ");

            var choice = Console.ReadLine() ?? string.Empty;
            if (choice == "0") return;

            var crafter = CraftingHelper.SelectCrafter(party);
            if (crafter == null) return;

            bool success = false;
            switch (choice)
            {
                case "1":
                    success = CraftingHelper.CraftTorchItem(crafter,
                        new[] { ("Wood", 1), ("Cotton", 1) },
                        new Torch("Basic Torch", 4, 5),
                        "Basic Torch", CraftingProfession.None);
                    break;
                case "2":
                    success = CraftingHelper.CraftTorchItem(crafter,
                        new[] { ("Wood", 1), ("Cloth", 1), ("Sulfur", 1) },
                        new Torch("Oil Torch", 6, 12),
                        "Oil Torch", CraftingProfession.None);
                    break;
                case "3":
                    success = CraftingHelper.CraftGenericItem(crafter,
                        new[] { ("Wood", 3), ("Cloth", 1) },
                        new GenericItem("Campfire Kit", 15),
                        "Campfire Kit", CraftingProfession.None);
                    break;
                case "4":
                    success = CraftingHelper.CraftGenericItem(crafter,
                        new[] { ("Spider Silk", 2), ("Cotton", 1) },
                        new GenericItem("Rope", 12),
                        "Rope", CraftingProfession.None);
                    break;
                case "5":
                    success = CraftingHelper.CraftGenericItem(crafter,
                        new[] { ("Cotton", 2), ("Healing Herb", 1) },
                        new GenericItem("Bandages", 18),
                        "Bandages", CraftingProfession.None);
                    break;
            }

            if (success)
                Console.WriteLine($"\n✅ {crafter.Name} successfully crafted the item!");
        }
    }
}
