using Night.Characters;
using Night.Shops;
using System;
using System.Collections.Generic;

namespace Rpg_Dungeon
{
    internal class MerchantShop : Shop
    {
        public override string ShopName => "General Goods (Merchant)";

        private readonly List<Backpack> _stock;
        private readonly List<StockEntry<Equipment>> _generalEquipment;

        public MerchantShop()
        {
            _stock = new List<Backpack>
            {
                new Backpack("Small Backpack", 5, 50),
                new Backpack("Medium Backpack", 10, 100),
                new Backpack("Large Backpack", 20, 200)
            };

            _generalEquipment = new List<StockEntry<Equipment>>
            {
                // Utility Items - Torches & Lanterns
                new StockEntry<Equipment>(new Torch("Basic Torch", 8, 10), 10, 30),
                new StockEntry<Equipment>(new Torch("Long-Burning Torch", 15, 20), 20, 20),
                new StockEntry<Equipment>(new Torch("Eternal Torch", 25, 35), 35, 10),
                new StockEntry<Equipment>(new Equipment("Lantern", EquipmentType.OffHand, 50, 45, hp: 8), 45, 15),
                new StockEntry<Equipment>(new Equipment("Sturdy Lantern", EquipmentType.OffHand, 75, 70, hp: 15, armor: 1), 70, 10),
                new StockEntry<Equipment>(new Equipment("Enchanted Lantern", EquipmentType.OffHand, 100, 105, hp: 22, armor: 2, mana: 10), 105, 5),

                // Universal Accessories
                new StockEntry<Equipment>(new Equipment("Traveler's Cloak", EquipmentType.Accessory, 60, 55, hp: 12, stamina: 10), 55, 12),
                new StockEntry<Equipment>(new Equipment("Adventurer's Belt", EquipmentType.Accessory, 70, 65, hp: 15, stamina: 12), 65, 10),
                new StockEntry<Equipment>(new Equipment("Explorer's Boots", EquipmentType.Accessory, 65, 60, agi: 3, stamina: 15), 60, 11),
                new StockEntry<Equipment>(new Equipment("Mercenary's Gloves", EquipmentType.Accessory, 58, 52, str: 3, agi: 2), 52, 12),
                new StockEntry<Equipment>(new Equipment("Lucky Charm", EquipmentType.Accessory, 80, 95, hp: 18, mana: 12, stamina: 15), 95, 6),
                new StockEntry<Equipment>(new Equipment("Adventurer's Pack", EquipmentType.Accessory, 75, 85, hp: 20, stamina: 18), 85, 7),

                // Basic Starter Equipment
                new StockEntry<Equipment>(new Equipment("Rusty Sword", EquipmentType.Weapon, 30, 25, str: 2), 25, 20),
                new StockEntry<Equipment>(new Equipment("Worn Dagger", EquipmentType.Weapon, 28, 22, agi: 2), 22, 20),
                new StockEntry<Equipment>(new Equipment("Old Staff", EquipmentType.Weapon, 32, 28, intel: 3), 28, 18),
                new StockEntry<Equipment>(new Equipment("Crude Mace", EquipmentType.Weapon, 30, 26, str: 2, intel: 1), 26, 18),
                new StockEntry<Equipment>(new Equipment("Tattered Cloth", EquipmentType.Armor, 25, 20, armor: 2), 20, 22),
                new StockEntry<Equipment>(new Equipment("Worn Leather", EquipmentType.Armor, 28, 24, armor: 2, agi: 1), 24, 20),
                new StockEntry<Equipment>(new Equipment("Old Chainmail", EquipmentType.Armor, 35, 32, armor: 3, str: 1), 32, 15),

                // Multi-Class Universal Items
                new StockEntry<Equipment>(new Equipment("Versatile Blade", EquipmentType.Weapon, 55, 90, str: 5, agi: 4, intel: 3), 90, 6),
                new StockEntry<Equipment>(new Equipment("Balanced Armor", EquipmentType.Armor, 65, 120, armor: 6, str: 3, agi: 3, intel: 3, hp: 18), 120, 4),
                new StockEntry<Equipment>(new Equipment("All-Purpose Gloves", EquipmentType.Accessory, 70, 85, str: 3, agi: 3, intel: 3), 85, 6),
                new StockEntry<Equipment>(new Equipment("Champion's Boots", EquipmentType.Accessory, 75, 110, str: 4, agi: 4, hp: 22), 110, 4),

                // Universal Rings & Necklaces
                new StockEntry<Equipment>(new Equipment("Ring of Vitality", EquipmentType.Ring, 100, 75, hp: 25), 75, 8),
                new StockEntry<Equipment>(new Equipment("Ring of Power", EquipmentType.Ring, 100, 85, str: 4, intel: 4), 85, 6),
                new StockEntry<Equipment>(new Equipment("Ring of Balance", EquipmentType.Ring, 100, 105, str: 3, agi: 3, intel: 3), 105, 5),
                new StockEntry<Equipment>(new Equipment("Pendant of the Ancients", EquipmentType.Necklace, 100, 130, hp: 32, mana: 28), 130, 4),
                new StockEntry<Equipment>(new Equipment("Champion's Medallion", EquipmentType.Necklace, 100, 240, str: 8, agi: 8, intel: 8, hp: 50), 240, 1),
            };
        }

        public override void OpenShop(List<Character> party)
        {
            if (party == null || party.Count == 0)
            {
                Console.WriteLine("No customers available.");
                return;
            }

            while (true)
            {
                Console.WriteLine("\nMerchant items:");
                for (int i = 0; i < _stock.Count; i++)
                {
                    var bp = _stock[i];
                    Console.WriteLine($"{i + 1}) {bp.Name} - +{bp.Slots} slots - {bp.Price} gold");
                }
                Console.WriteLine("E) Buy General Equipment");
                Console.WriteLine("S) Sell item");
                Console.WriteLine("0) Leave shop");
                Console.Write("Choose an item to buy: ");
                var input = Console.ReadLine() ?? string.Empty;
                if (input.Trim().Equals("0", StringComparison.OrdinalIgnoreCase)) return;
                if (input.Trim().Equals("E", StringComparison.OrdinalIgnoreCase))
                {
                    BuyEquipment(party, _generalEquipment);
                    continue;
                }
                if (input.Trim().Equals("S", StringComparison.OrdinalIgnoreCase))
                {
                    HandleSell(party);
                    continue;
                }

                if (!int.TryParse(input, out var choice) || choice < 1 || choice > _stock.Count)
                {
                    Console.WriteLine("Invalid selection.");
                    continue;
                }

                var selected = _stock[choice - 1];
                Console.WriteLine($"Who should buy the {selected.Name}? Enter party index (1-{party.Count}): ");
                for (int i = 0; i < party.Count; i++) Console.WriteLine($"{i + 1}) {party[i].Name} - {party[i].Inventory.Gold} gold");

                var who = Console.ReadLine() ?? string.Empty;
                if (!int.TryParse(who, out var idx) || idx < 1 || idx > party.Count)
                {
                    Console.WriteLine("Invalid party selection.");
                    continue;
                }

                var buyer = party[idx - 1];
                if (buyer.Inventory.SpendGold(selected.Price))
                {
                    buyer.Inventory.EquipBackpack(selected);
                    Console.WriteLine($"{buyer.Name} bought and equipped {selected.Name} (+{selected.Slots} slots).");
                }
                else
                {
                    Console.WriteLine($"{buyer.Name} doesn't have enough gold ({selected.Price} required).");
                }
            }
        }

        private void HandleSell(List<Character> party)
        {
            Console.WriteLine("Select seller (party member index):");
            for (int i = 0; i < party.Count; i++) Console.WriteLine($"{i + 1}) {party[i].Name} - {party[i].Inventory.Gold} gold");
            var who = Console.ReadLine() ?? string.Empty;
            if (!int.TryParse(who, out var idx) || idx < 1 || idx > party.Count)
            {
                Console.WriteLine("Invalid selection.");
                return;
            }

            var seller = party[idx - 1];
            var slots = seller.Inventory.Slots;
            Console.WriteLine($"Items for {seller.Name}:");
            for (int i = 0; i < slots.Count; i++)
            {
                var it = slots[i];
                Console.WriteLine($"{i + 1}) {(it == null ? "(empty)" : it.Name)}");
            }

            Console.WriteLine("Enter slot number to sell (0 to cancel):");
            var s = Console.ReadLine() ?? string.Empty;
            if (!int.TryParse(s, out var slotIdx) || slotIdx < 0 || slotIdx > slots.Count) { Console.WriteLine("Invalid slot."); return; }
            if (slotIdx == 0) return;
            var item = slots[slotIdx - 1];
            if (item == null)
            {
                Console.WriteLine("No item in that slot.");
                return;
            }

            int sellPrice = (int)Math.Ceiling(item.Price * 0.75);
            seller.Inventory.AddGold(sellPrice);
            seller.Inventory.RemoveItem(slotIdx - 1);
            Console.WriteLine($"{seller.Name} sold {item.Name} for {sellPrice} gold (merchant pays 75% of base price).");
        }
    }
}
