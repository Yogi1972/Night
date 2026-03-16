using Night.Characters;
using Night.Shops;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rpg_Dungeon
{
    internal class JewelerShop : Shop
    {
        public override string ShopName => "Golden Trinkets (Jeweler)";

        private readonly List<StockEntry<Equipment>> _jewelry;

        public JewelerShop()
        {
            _jewelry = new List<StockEntry<Equipment>>
            {
                // WARRIOR RINGS
                new StockEntry<Equipment>(new Equipment("Iron Ring", EquipmentType.Ring, 100, 40, str: 3), 40, 12),
                new StockEntry<Equipment>(new Equipment("Ring of Strength", EquipmentType.Ring, 100, 55, str: 5), 55, 10),
                new StockEntry<Equipment>(new Equipment("Berserker's Band", EquipmentType.Ring, 100, 75, str: 7, hp: 10), 75, 7),
                new StockEntry<Equipment>(new Equipment("Ring of Might", EquipmentType.Ring, 100, 95, str: 9), 95, 5),
                new StockEntry<Equipment>(new Equipment("Ring of the Berserker", EquipmentType.Ring, 100, 145, str: 11, stamina: 20), 145, 3),
                new StockEntry<Equipment>(new Equipment("Guardian's Band", EquipmentType.Ring, 100, 155, str: 8, armor: 5, hp: 35), 155, 2),
                new StockEntry<Equipment>(new Equipment("Paladin's Ring", EquipmentType.Ring, 100, 165, str: 9, intel: 9, mana: 25), 165, 2),
                new StockEntry<Equipment>(new Equipment("Titan's Ring", EquipmentType.Ring, 100, 200, str: 13, hp: 50), 200, 1),

                // WARRIOR NECKLACES
                new StockEntry<Equipment>(new Equipment("Iron Necklace", EquipmentType.Necklace, 100, 55, str: 4, hp: 12), 55, 10),
                new StockEntry<Equipment>(new Equipment("Amulet of Strength", EquipmentType.Necklace, 100, 85, str: 7, hp: 20), 85, 7),
                new StockEntry<Equipment>(new Equipment("Berserker's Torc", EquipmentType.Necklace, 100, 160, str: 11, stamina: 35), 160, 3),
                new StockEntry<Equipment>(new Equipment("Guardian's Pendant", EquipmentType.Necklace, 100, 175, armor: 7, hp: 55), 175, 2),
                new StockEntry<Equipment>(new Equipment("Paladin's Medallion", EquipmentType.Necklace, 100, 190, str: 10, intel: 11, mana: 35, hp: 40), 190, 2),
                new StockEntry<Equipment>(new Equipment("Champion's Chain", EquipmentType.Necklace, 100, 220, str: 13, armor: 5, hp: 60), 220, 1),

                // MAGE RINGS
                new StockEntry<Equipment>(new Equipment("Gold Ring", EquipmentType.Ring, 100, 50, intel: 4), 50, 11),
                new StockEntry<Equipment>(new Equipment("Ring of Intellect", EquipmentType.Ring, 100, 65, intel: 6), 65, 9),
                new StockEntry<Equipment>(new Equipment("Mystic Circle", EquipmentType.Ring, 100, 80, intel: 7, mana: 18), 80, 7),
                new StockEntry<Equipment>(new Equipment("Ring of Arcana", EquipmentType.Ring, 100, 110, intel: 9, mana: 28), 110, 5),
                new StockEntry<Equipment>(new Equipment("Ring of the Archmage", EquipmentType.Ring, 100, 165, intel: 14, mana: 50), 165, 2),
                new StockEntry<Equipment>(new Equipment("Necromancer's Band", EquipmentType.Ring, 100, 155, intel: 12, hp: 25, mana: 40), 155, 2),
                new StockEntry<Equipment>(new Equipment("Elementalist's Loop", EquipmentType.Ring, 100, 160, intel: 13, mana: 45), 160, 2),
                new StockEntry<Equipment>(new Equipment("Void Ring", EquipmentType.Ring, 100, 180, intel: 16, mana: 55), 180, 1),

                // MAGE NECKLACES
                new StockEntry<Equipment>(new Equipment("Silver Necklace", EquipmentType.Necklace, 100, 60, mana: 18), 60, 10),
                new StockEntry<Equipment>(new Equipment("Arcane Pendant", EquipmentType.Necklace, 100, 95, intel: 8, mana: 30), 95, 7),
                new StockEntry<Equipment>(new Equipment("Amulet of Wisdom", EquipmentType.Necklace, 100, 120, intel: 10, mana: 38), 120, 5),
                new StockEntry<Equipment>(new Equipment("Mystic Amulet", EquipmentType.Necklace, 100, 155, intel: 13, mana: 50), 155, 3),
                new StockEntry<Equipment>(new Equipment("Archmage's Medallion", EquipmentType.Necklace, 100, 210, intel: 20, mana: 80), 210, 1),
                new StockEntry<Equipment>(new Equipment("Necromancer's Amulet", EquipmentType.Necklace, 100, 195, intel: 17, hp: 40, mana: 65), 195, 2),
                new StockEntry<Equipment>(new Equipment("Elemental Talisman", EquipmentType.Necklace, 100, 200, intel: 18, mana: 70), 200, 1),

                // ROGUE RINGS
                new StockEntry<Equipment>(new Equipment("Silver Ring", EquipmentType.Ring, 100, 45, agi: 3), 45, 12),
                new StockEntry<Equipment>(new Equipment("Ring of Agility", EquipmentType.Ring, 100, 60, agi: 6), 60, 9),
                new StockEntry<Equipment>(new Equipment("Thief's Signet", EquipmentType.Ring, 100, 70, agi: 7, str: 2), 70, 8),
                new StockEntry<Equipment>(new Equipment("Ring of Swiftness", EquipmentType.Ring, 100, 100, agi: 9, stamina: 15), 100, 5),
                new StockEntry<Equipment>(new Equipment("Assassin's Band", EquipmentType.Ring, 100, 160, agi: 14, stamina: 30), 160, 2),
                new StockEntry<Equipment>(new Equipment("Ranger's Ring", EquipmentType.Ring, 100, 155, agi: 13, hp: 25), 155, 2),
                new StockEntry<Equipment>(new Equipment("Shadow Ring", EquipmentType.Ring, 100, 170, agi: 15, intel: 5), 170, 2),
                new StockEntry<Equipment>(new Equipment("Windwalker's Band", EquipmentType.Ring, 100, 190, agi: 17, stamina: 35), 190, 1),

                // ROGUE NECKLACES
                new StockEntry<Equipment>(new Equipment("Shadow Amulet", EquipmentType.Necklace, 100, 65, agi: 5, hp: 10), 65, 9),
                new StockEntry<Equipment>(new Equipment("Amulet of Agility", EquipmentType.Necklace, 100, 90, agi: 8, stamina: 15), 90, 6),
                new StockEntry<Equipment>(new Equipment("Shadow Pendant", EquipmentType.Necklace, 100, 125, agi: 11, stamina: 25), 125, 4),
                new StockEntry<Equipment>(new Equipment("Assassin's Choker", EquipmentType.Necklace, 100, 175, agi: 16, stamina: 35), 175, 2),
                new StockEntry<Equipment>(new Equipment("Ranger's Torc", EquipmentType.Necklace, 100, 170, agi: 14, hp: 35), 170, 2),
                new StockEntry<Equipment>(new Equipment("Shadowblade's Amulet", EquipmentType.Necklace, 100, 185, agi: 17, intel: 7), 185, 1),

                // PRIEST RINGS
                new StockEntry<Equipment>(new Equipment("Ring of Restoration", EquipmentType.Ring, 100, 70, intel: 5, mana: 12), 70, 8),
                new StockEntry<Equipment>(new Equipment("Blessed Band", EquipmentType.Ring, 100, 85, intel: 6, hp: 15), 85, 7),
                new StockEntry<Equipment>(new Equipment("Ring of Faith", EquipmentType.Ring, 100, 100, intel: 8, mana: 25), 100, 5),
                new StockEntry<Equipment>(new Equipment("Ring of Devotion", EquipmentType.Ring, 100, 125, intel: 10, hp: 28, mana: 30), 125, 4),
                new StockEntry<Equipment>(new Equipment("Templar's Signet", EquipmentType.Ring, 100, 160, str: 7, intel: 11, armor: 4), 160, 2),
                new StockEntry<Equipment>(new Equipment("Druid's Band", EquipmentType.Ring, 100, 155, intel: 11, hp: 40), 155, 2),
                new StockEntry<Equipment>(new Equipment("Oracle's Circle", EquipmentType.Ring, 100, 175, intel: 15, mana: 55), 175, 1),

                // PRIEST NECKLACES  
                new StockEntry<Equipment>(new Equipment("Holy Symbol", EquipmentType.Necklace, 100, 75, intel: 6, mana: 15), 75, 8),
                new StockEntry<Equipment>(new Equipment("Amulet of Protection", EquipmentType.Necklace, 100, 90, armor: 3, hp: 20), 90, 6),
                new StockEntry<Equipment>(new Equipment("Holy Symbol Necklace", EquipmentType.Necklace, 100, 115, intel: 9, mana: 32, hp: 25), 115, 4),
                new StockEntry<Equipment>(new Equipment("Divine Pendant", EquipmentType.Necklace, 100, 150, intel: 12, mana: 45, hp: 38), 150, 3),
                new StockEntry<Equipment>(new Equipment("Templar's Chain", EquipmentType.Necklace, 100, 185, str: 9, intel: 13, armor: 5, hp: 45), 185, 2),
                new StockEntry<Equipment>(new Equipment("Oracle's Vision", EquipmentType.Necklace, 100, 195, intel: 18, mana: 70), 195, 1),

                // UNIVERSAL LEGENDARY
                new StockEntry<Equipment>(new Equipment("Golden Necklace", EquipmentType.Necklace, 100, 135, str: 5, agi: 5, intel: 5), 135, 3),
                new StockEntry<Equipment>(new Equipment("Dragon Scale Amulet", EquipmentType.Necklace, 100, 250, str: 8, agi: 8, intel: 8, hp: 40), 250, 1),
                new StockEntry<Equipment>(new Equipment("Ring of Protection", EquipmentType.Ring, 100, 90, hp: 20, mana: 15), 90, 6),
                new StockEntry<Equipment>(new Equipment("Platinum Band", EquipmentType.Ring, 100, 130, str: 6, agi: 6), 130, 3),
                new StockEntry<Equipment>(new Equipment("Ring of Balance", EquipmentType.Ring, 100, 110, str: 4, agi: 4, intel: 4), 110, 5),
                new StockEntry<Equipment>(new Equipment("Ring of the Champion", EquipmentType.Ring, 100, 260, str: 9, agi: 9, intel: 9, hp: 50), 260, 1),
                new StockEntry<Equipment>(new Equipment("Ruby Ring", EquipmentType.Ring, 100, 100, str: 8, intel: 3), 100, 5),
                new StockEntry<Equipment>(new Equipment("Emerald Ring", EquipmentType.Ring, 100, 100, agi: 8, intel: 3), 100, 5),
                new StockEntry<Equipment>(new Equipment("Sapphire Ring", EquipmentType.Ring, 100, 105, intel: 9, mana: 20), 105, 5),
                new StockEntry<Equipment>(new Equipment("Diamond Ring", EquipmentType.Ring, 100, 220, str: 7, agi: 7, intel: 7, hp: 30), 220, 1)
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
                Console.WriteLine("\nJeweler: 1) Buy Jewelry  2) Sell Jewelry  3) Repair Jewelry  0) Leave");
                Console.Write("Choice: ");
                var input = Console.ReadLine() ?? string.Empty;
                if (input.Trim() == "0") return;

                switch (input.Trim())
                {
                    case "1":
                        BuyEquipment(party, _jewelry);
                        break;
                    case "2":
                        SellJewelry(party);
                        break;
                    case "3":
                        RepairJewelry(party);
                        break;
                    default:
                        Console.WriteLine("Invalid choice.");
                        break;
                }
            }
        }

        private void SellJewelry(List<Character> party)
        {
            Console.WriteLine("Select seller (party member):");
            for (int i = 0; i < party.Count; i++) Console.WriteLine($"{i + 1}) {party[i].Name} - Gold: {party[i].Inventory.Gold}");
            var who = Console.ReadLine() ?? string.Empty;
            if (!int.TryParse(who, out var whoIdx) || whoIdx < 1 || whoIdx > party.Count) { Console.WriteLine("Invalid."); return; }
            var seller = party[whoIdx - 1];

            var slots = seller.Inventory.Slots;
            Console.WriteLine("Select slot number of jewelry to sell:");
            for (int i = 0; i < slots.Count; i++) Console.WriteLine($"{i + 1}) {(slots[i] == null ? "(empty)" : slots[i]!.Name)}");
            var s = Console.ReadLine() ?? string.Empty;
            if (!int.TryParse(s, out var slotIdx) || slotIdx < 1 || slotIdx > slots.Count) { Console.WriteLine("Invalid."); return; }
            var item = slots[slotIdx - 1];
            if (item == null) { Console.WriteLine("No item."); return; }

            if (item is not Equipment eq || (eq.Type != EquipmentType.Necklace && eq.Type != EquipmentType.Ring))
            {
                Console.WriteLine("This is not jewelry!");
                return;
            }

            var shopEntry = _jewelry.FirstOrDefault(j => string.Equals(j.Item.Name, item.Name, StringComparison.OrdinalIgnoreCase));
            int sellPrice;
            if (shopEntry != null)
            {
                sellPrice = (int)Math.Ceiling(shopEntry.Price * 0.75);
                shopEntry.Quantity++;
            }
            else
            {
                sellPrice = (int)Math.Ceiling(eq.Price * 0.75);
            }

            seller.Inventory.AddGold(sellPrice);
            seller.Inventory.RemoveItem(slotIdx - 1);
            Console.WriteLine($"{seller.Name} sold {item.Name} for {sellPrice} gold.");
        }

        private void RepairJewelry(List<Character> party)
        {
            Console.WriteLine("\n=== JEWELRY REPAIR SERVICE ===");
            Console.WriteLine("Select who needs jewelry repair:");
            for (int i = 0; i < party.Count; i++)
            {
                Console.WriteLine($"{i + 1}) {party[i].Name} - Gold: {party[i].Inventory.Gold}");
            }
            var who = Console.ReadLine() ?? string.Empty;
            if (!int.TryParse(who, out var whoIdx) || whoIdx < 1 || whoIdx > party.Count) { Console.WriteLine("Invalid."); return; }
            var c = party[whoIdx - 1];

            Console.WriteLine($"\n{c.Name}'s Jewelry:");
            var repairableItems = new List<(string location, Equipment eq)>();
            int itemNum = 1;

            if (c.Inventory.EquippedNecklace != null)
            {
                var eq = c.Inventory.EquippedNecklace;
                string durStatus = eq.Durability <= eq.MaxDurability / 4 ? " [LOW!]" : "";
                Console.WriteLine($"{itemNum}) [EQUIPPED] Necklace: {eq.Name} (Dur: {eq.Durability}/{eq.MaxDurability}){durStatus} - Repair: {eq.RepairCost()}g");
                repairableItems.Add(("Necklace", eq));
                itemNum++;
            }

            if (c.Inventory.EquippedRing1 != null)
            {
                var eq = c.Inventory.EquippedRing1;
                string durStatus = eq.Durability <= eq.MaxDurability / 4 ? " [LOW!]" : "";
                Console.WriteLine($"{itemNum}) [EQUIPPED] Ring 1: {eq.Name} (Dur: {eq.Durability}/{eq.MaxDurability}){durStatus} - Repair: {eq.RepairCost()}g");
                repairableItems.Add(("Ring1", eq));
                itemNum++;
            }

            if (c.Inventory.EquippedRing2 != null)
            {
                var eq = c.Inventory.EquippedRing2;
                string durStatus = eq.Durability <= eq.MaxDurability / 4 ? " [LOW!]" : "";
                Console.WriteLine($"{itemNum}) [EQUIPPED] Ring 2: {eq.Name} (Dur: {eq.Durability}/{eq.MaxDurability}){durStatus} - Repair: {eq.RepairCost()}g");
                repairableItems.Add(("Ring2", eq));
                itemNum++;
            }

            var slots = c.Inventory.Slots;
            for (int i = 0; i < slots.Count; i++)
            {
                if (slots[i] is Equipment eq && (eq.Type == EquipmentType.Necklace || eq.Type == EquipmentType.Ring))
                {
                    string durStatus = eq.Durability <= eq.MaxDurability / 4 ? " [LOW!]" : "";
                    Console.WriteLine($"{itemNum}) [Inventory Slot {i + 1}]: {eq.Name} (Dur: {eq.Durability}/{eq.MaxDurability}){durStatus} - Repair: {eq.RepairCost()}g");
                    repairableItems.Add(($"Slot{i}", eq));
                    itemNum++;
                }
            }

            if (repairableItems.Count == 0)
            {
                Console.WriteLine("No jewelry to repair.");
                return;
            }

            Console.WriteLine("\nOptions:");
            Console.WriteLine("R) Repair All (pay full cost for all jewelry)");
            Console.WriteLine("0) Cancel");
            Console.Write("Choose jewelry number to repair or R for all: ");
            var sel = Console.ReadLine() ?? string.Empty;

            if (sel.Trim().Equals("R", StringComparison.OrdinalIgnoreCase))
            {
                int totalCost = repairableItems.Sum(item => item.eq.RepairCost());
                if (totalCost <= 0)
                {
                    Console.WriteLine("All jewelry is already at full durability!");
                    return;
                }

                Console.WriteLine($"\nTotal repair cost for all jewelry: {totalCost} gold");
                Console.Write("Proceed with repair all? (y/n): ");
                var confirm = Console.ReadLine() ?? string.Empty;
                if (!confirm.Trim().Equals("y", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("Repair cancelled.");
                    return;
                }

                if (!c.Inventory.SpendGold(totalCost))
                {
                    Console.WriteLine($"Not enough gold. You have {c.Inventory.Gold}g but need {totalCost}g.");
                    return;
                }

                foreach (var item in repairableItems)
                {
                    if (item.eq.RepairCost() > 0)
                    {
                        item.eq.Repair();
                        Console.WriteLine($"✓ {item.eq.Name} repaired to full durability.");
                    }
                }

                Console.WriteLine($"\n💎 All jewelry repaired! Gold remaining: {c.Inventory.Gold}");
            }
            else if (int.TryParse(sel, out var selIdx) && selIdx >= 1 && selIdx <= repairableItems.Count)
            {
                var chosen = repairableItems[selIdx - 1];
                int cost = chosen.eq.RepairCost();
                if (cost <= 0) { Console.WriteLine("Item is already at full durability!"); return; }
                if (!c.Inventory.SpendGold(cost))
                {
                    Console.WriteLine($"Not enough gold. Repair costs {cost}g but you have {c.Inventory.Gold}g.");
                    return;
                }
                chosen.eq.Repair();
                Console.WriteLine($"💎 {chosen.eq.Name} repaired to full durability! Gold remaining: {c.Inventory.Gold}");
            }
            else
            {
                Console.WriteLine("Invalid selection.");
            }
        }
    }
}
