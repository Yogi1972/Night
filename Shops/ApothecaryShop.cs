using Night.Characters;
using Night.Shops;
using System;
using System.Collections.Generic;

namespace Rpg_Dungeon
{
    internal class ApothecaryShop : Shop
    {
        public override string ShopName => "Healing Hands (Apothecary)";

        private readonly List<StockEntry<GenericItem>> _potionsAndHerbs;
        private readonly List<StockEntry<Equipment>> _healingEquipment;

        public ApothecaryShop()
        {
            _potionsAndHerbs = new List<StockEntry<GenericItem>>
            {
                // Basic Healing
                new StockEntry<GenericItem>(new GenericItem("Healing Potion", 20), 20, 50),
                new StockEntry<GenericItem>(new GenericItem("Greater Healing Potion", 45), 45, 25),
                new StockEntry<GenericItem>(new GenericItem("Superior Healing Potion", 80), 80, 10),
                new StockEntry<GenericItem>(new GenericItem("Elixir of Vitality", 120), 120, 5),

                // Mana Restoration
                new StockEntry<GenericItem>(new GenericItem("Potion of Mana", 25), 25, 40),
                new StockEntry<GenericItem>(new GenericItem("Greater Mana Potion", 50), 50, 20),
                new StockEntry<GenericItem>(new GenericItem("Superior Mana Potion", 90), 90, 8),
                new StockEntry<GenericItem>(new GenericItem("Elixir of Clarity", 130), 130, 4),

                // Stamina Restoration
                new StockEntry<GenericItem>(new GenericItem("Potion of Stamina", 25), 25, 35),
                new StockEntry<GenericItem>(new GenericItem("Greater Stamina Potion", 50), 50, 18),
                new StockEntry<GenericItem>(new GenericItem("Superior Stamina Potion", 85), 85, 7),
                new StockEntry<GenericItem>(new GenericItem("Elixir of Energy", 125), 125, 4),

                // Utility Potions
                new StockEntry<GenericItem>(new GenericItem("Antidote", 15), 15, 30),
                new StockEntry<GenericItem>(new GenericItem("Restorative Salve", 25), 25, 25),
                new StockEntry<GenericItem>(new GenericItem("Bandages", 5), 5, 80),
                new StockEntry<GenericItem>(new GenericItem("Blessed Water", 30), 30, 20),
                new StockEntry<GenericItem>(new GenericItem("Phoenix Down", 200), 200, 3),
                new StockEntry<GenericItem>(new GenericItem("Elixir of Fortitude", 100), 100, 6),
                new StockEntry<GenericItem>(new GenericItem("Potion of Resistance", 60), 60, 12),

                // Crafting Ingredients
                new StockEntry<GenericItem>(new GenericItem("Healing Herb", 8), 8, 60),
                new StockEntry<GenericItem>(new GenericItem("Moonflower", 15), 15, 30),
                new StockEntry<GenericItem>(new GenericItem("Bloodroot", 12), 12, 35),
                new StockEntry<GenericItem>(new GenericItem("Nightshade", 18), 18, 25),
                new StockEntry<GenericItem>(new GenericItem("Mandrake Root", 22), 22, 20),
                new StockEntry<GenericItem>(new GenericItem("Dragon's Breath Flower", 50), 50, 8),
                new StockEntry<GenericItem>(new GenericItem("Phoenix Feather", 180), 180, 2),
                new StockEntry<GenericItem>(new GenericItem("Unicorn Horn Powder", 160), 160, 3),
            };

            _healingEquipment = new List<StockEntry<Equipment>>
            {
                // PRIEST OFF-HAND ITEMS
                new StockEntry<Equipment>(new Equipment("Healing Pouch", EquipmentType.OffHand, 40, 55, intel: 2, hp: 15), 55, 8),
                new StockEntry<Equipment>(new Equipment("Medicine Bag", EquipmentType.OffHand, 50, 80, intel: 4, hp: 25, mana: 10), 80, 6),
                new StockEntry<Equipment>(new Equipment("Herbalist's Satchel", EquipmentType.OffHand, 60, 110, intel: 5, hp: 35, mana: 15), 110, 4),
                new StockEntry<Equipment>(new Equipment("Prayer Beads", EquipmentType.OffHand, 55, 95, intel: 6, mana: 20), 95, 5),
                new StockEntry<Equipment>(new Equipment("Holy Symbol", EquipmentType.OffHand, 65, 120, intel: 8, mana: 25, hp: 20), 120, 4),
                new StockEntry<Equipment>(new Equipment("Divine Relic", EquipmentType.OffHand, 75, 160, intel: 10, mana: 35, hp: 30), 160, 3),
                new StockEntry<Equipment>(new Equipment("Templar's Codex", EquipmentType.OffHand, 80, 190, str: 7, intel: 12, mana: 30), 190, 2),
                new StockEntry<Equipment>(new Equipment("Druid's Idol", EquipmentType.OffHand, 78, 185, intel: 12, hp: 40, stamina: 25), 185, 2),
                new StockEntry<Equipment>(new Equipment("Oracle's Tome", EquipmentType.OffHand, 82, 200, intel: 15, mana: 60), 200, 1),

                // PRIEST RINGS
                new StockEntry<Equipment>(new Equipment("Ring of Faith", EquipmentType.Ring, 100, 85, intel: 6, mana: 20), 85, 4),
                new StockEntry<Equipment>(new Equipment("Ring of Devotion", EquipmentType.Ring, 100, 110, intel: 8, hp: 25, mana: 25), 110, 3),
                new StockEntry<Equipment>(new Equipment("Templar's Signet", EquipmentType.Ring, 100, 150, str: 7, intel: 10, armor: 4), 150, 2),
                new StockEntry<Equipment>(new Equipment("Druid's Band", EquipmentType.Ring, 100, 145, intel: 10, hp: 35), 145, 2),
                new StockEntry<Equipment>(new Equipment("Oracle's Circle", EquipmentType.Ring, 100, 165, intel: 13, mana: 50), 165, 1),

                // PRIEST NECKLACES
                new StockEntry<Equipment>(new Equipment("Amulet of Protection", EquipmentType.Necklace, 100, 80, armor: 3, hp: 20), 80, 5),
                new StockEntry<Equipment>(new Equipment("Holy Symbol Necklace", EquipmentType.Necklace, 100, 105, intel: 7, mana: 30, hp: 25), 105, 3),
                new StockEntry<Equipment>(new Equipment("Divine Pendant", EquipmentType.Necklace, 100, 140, intel: 10, mana: 40, hp: 35), 140, 2),
                new StockEntry<Equipment>(new Equipment("Templar's Chain", EquipmentType.Necklace, 100, 170, str: 8, intel: 12, armor: 5, hp: 40), 170, 2),
                new StockEntry<Equipment>(new Equipment("Oracle's Vision", EquipmentType.Necklace, 100, 180, intel: 16, mana: 65), 180, 1),
            };
        }

        public override void OpenShop(List<Character> party)
        {
            if (party == null || party.Count == 0)
            {
                Console.WriteLine("No customers.");
                return;
            }

            while (true)
            {
                Console.WriteLine("\nApothecary: 1) Buy Potions & Herbs  2) Sell Ingredients  3) Brew Potion  4) Buy Healing Equipment  0) Leave");
                Console.Write("Choice: ");
                var input = Console.ReadLine() ?? string.Empty;
                if (input.Trim() == "0") return;

                switch (input.Trim())
                {
                    case "1":
                        BuyFromStock(party, _potionsAndHerbs, "potions and herbs");
                        break;
                    case "2":
                        SellToStock(party, _potionsAndHerbs, "Apothecary");
                        break;
                    case "3":
                        BrewPotion(party);
                        break;
                    case "4":
                        BuyEquipment(party, _healingEquipment);
                        break;
                    default:
                        Console.WriteLine("Invalid choice.");
                        break;
                }
            }
        }

        private void BrewPotion(List<Character> party)
        {
            Console.WriteLine("Select who will brew:");
            for (int i = 0; i < party.Count; i++) Console.WriteLine($"{i + 1}) {party[i].Name}");
            var who = Console.ReadLine() ?? string.Empty;
            if (!int.TryParse(who, out var whoIdx) || whoIdx < 1 || whoIdx > party.Count) { Console.WriteLine("Invalid."); return; }
            var c = party[whoIdx - 1];

            Console.WriteLine("\nBrew recipes:");
            Console.WriteLine("1) Healing Potion - Requires 2x Healing Herb (Cost: 5 gold)");
            Console.WriteLine("2) Greater Healing Potion - Requires 1x Healing Potion + 1x Blessed Water (Cost: 15 gold)");
            Console.WriteLine("0) Cancel");

            var choice = Console.ReadLine() ?? string.Empty;
            if (choice == "0") return;

            if (choice == "1")
            {
                var slots = c.Inventory.Slots;
                var herbSlots = new List<int>();
                for (int i = 0; i < slots.Count; i++)
                {
                    if (slots[i]?.Name == "Healing Herb") herbSlots.Add(i);
                }

                if (herbSlots.Count < 2) { Console.WriteLine("Need 2 Healing Herbs!"); return; }
                if (c.Inventory.Gold < 5) { Console.WriteLine("Need 5 gold!"); return; }

                c.Inventory.SpendGold(5);
                c.Inventory.RemoveItem(herbSlots[0]);
                c.Inventory.RemoveItem(herbSlots[1]);
                c.Inventory.AddItem(new GenericItem("Healing Potion", 20));
                Console.WriteLine($"{c.Name} brewed a Healing Potion!");
            }
            else if (choice == "2")
            {
                var slots = c.Inventory.Slots;
                int potionSlot = -1;
                int waterSlot = -1;

                for (int i = 0; i < slots.Count; i++)
                {
                    if (slots[i]?.Name == "Healing Potion" && potionSlot == -1) potionSlot = i;
                    if (slots[i]?.Name == "Blessed Water" && waterSlot == -1) waterSlot = i;
                }

                if (potionSlot == -1 || waterSlot == -1) { Console.WriteLine("Need Healing Potion and Blessed Water!"); return; }
                if (c.Inventory.Gold < 15) { Console.WriteLine("Need 15 gold!"); return; }

                c.Inventory.SpendGold(15);
                c.Inventory.RemoveItem(potionSlot);
                c.Inventory.RemoveItem(waterSlot);
                c.Inventory.AddItem(new GenericItem("Greater Healing Potion", 45));
                Console.WriteLine($"{c.Name} brewed a Greater Healing Potion!");
            }
        }
    }
}
