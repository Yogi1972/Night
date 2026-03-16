using Night.Characters;
using Night.Shops;
using System;
using System.Collections.Generic;

namespace Rpg_Dungeon
{
    internal class MageShop : Shop
    {
        public override string ShopName => "Arcane Emporium (Mage Shop)";

        private readonly List<StockEntry<GenericItem>> _magicItems;
        private readonly List<StockEntry<Equipment>> _magicEquipment;

        public MageShop()
        {
            _magicItems = new List<StockEntry<GenericItem>>
            {
                // Basic Magic Materials
                new StockEntry<GenericItem>(new GenericItem("Magic Dust", 15), 15, 50),
                new StockEntry<GenericItem>(new GenericItem("Arcane Dust", 20), 20, 40),
                new StockEntry<GenericItem>(new GenericItem("Mana Crystal", 30), 30, 30),
                new StockEntry<GenericItem>(new GenericItem("Greater Mana Crystal", 60), 60, 15),

                // Advanced Magic Materials
                new StockEntry<GenericItem>(new GenericItem("Spell Scroll", 40), 40, 20),
                new StockEntry<GenericItem>(new GenericItem("Enchanted Gem", 50), 50, 18),
                new StockEntry<GenericItem>(new GenericItem("Void Crystal", 75), 75, 12),
                new StockEntry<GenericItem>(new GenericItem("Soul Gem", 90), 90, 10),
                new StockEntry<GenericItem>(new GenericItem("Arcane Essence", 110), 110, 8),

                // Rare Magic Materials
                new StockEntry<GenericItem>(new GenericItem("Phoenix Ash", 150), 150, 5),
                new StockEntry<GenericItem>(new GenericItem("Star Fragment", 200), 200, 3),
                new StockEntry<GenericItem>(new GenericItem("Dragon's Heart Stone", 250), 250, 2),

                // Class-Specific Materials
                new StockEntry<GenericItem>(new GenericItem("Necromantic Essence", 95), 95, 8),
                new StockEntry<GenericItem>(new GenericItem("Elemental Prism", 105), 105, 7),
                new StockEntry<GenericItem>(new GenericItem("Shadow Crystal", 85), 85, 10),
                new StockEntry<GenericItem>(new GenericItem("Holy Essence", 88), 88, 9),

                // Potions
                new StockEntry<GenericItem>(new GenericItem("Potion of Mana", 25), 25, 40),
                new StockEntry<GenericItem>(new GenericItem("Greater Mana Potion", 50), 50, 22),
                new StockEntry<GenericItem>(new GenericItem("Potion of Intellect", 70), 70, 12),
                new StockEntry<GenericItem>(new GenericItem("Elixir of Arcane Power", 140), 140, 5)
            };

            _magicEquipment = new List<StockEntry<Equipment>>
            {
                // MAGE WEAPONS - Staves
                new StockEntry<Equipment>(new Equipment("Apprentice Staff", EquipmentType.Weapon, 45, 70, intel: 6, mana: 15), 70, 10),
                new StockEntry<Equipment>(new Equipment("Enchanted Staff", EquipmentType.Weapon, 50, 95, intel: 9, mana: 22), 95, 8),
                new StockEntry<Equipment>(new Equipment("Arcane Staff", EquipmentType.Weapon, 55, 110, intel: 11, mana: 28), 110, 6),
                new StockEntry<Equipment>(new Equipment("Crystal Staff", EquipmentType.Weapon, 60, 130, intel: 13, mana: 35), 130, 5),
                new StockEntry<Equipment>(new Equipment("Sorcerer's Rod", EquipmentType.Weapon, 58, 125, intel: 12, mana: 32), 125, 5),
                new StockEntry<Equipment>(new Equipment("Staff of the Magi", EquipmentType.Weapon, 70, 165, intel: 16, mana: 45), 165, 3),
                new StockEntry<Equipment>(new Equipment("Mystic Wand", EquipmentType.Weapon, 52, 105, intel: 10, mana: 25), 105, 7),

                // MAGE CHAMPION WEAPONS
                new StockEntry<Equipment>(new Equipment("Archmage's Staff", EquipmentType.Weapon, 85, 240, intel: 22, mana: 70), 240, 2),
                new StockEntry<Equipment>(new Equipment("Meteor Staff", EquipmentType.Weapon, 90, 280, intel: 25, mana: 80), 280, 1),
                new StockEntry<Equipment>(new Equipment("Necromancer's Scythe", EquipmentType.Weapon, 80, 230, intel: 19, mana: 55, hp: 40), 230, 2),
                new StockEntry<Equipment>(new Equipment("Soul Reaper", EquipmentType.Weapon, 88, 265, intel: 21, mana: 65, hp: 35), 265, 1),
                new StockEntry<Equipment>(new Equipment("Elementalist's Conduit", EquipmentType.Weapon, 82, 235, intel: 20, mana: 60), 235, 2),
                new StockEntry<Equipment>(new Equipment("Staff of the Elements", EquipmentType.Weapon, 92, 275, intel: 24, mana: 75), 275, 1),

                // MAGE ARMOR
                new StockEntry<Equipment>(new Equipment("Cloth Robes", EquipmentType.Armor, 40, 55, armor: 3, intel: 5, mana: 12), 55, 10),
                new StockEntry<Equipment>(new Equipment("Mage Robes", EquipmentType.Armor, 50, 85, armor: 4, intel: 7, mana: 20), 85, 8),
                new StockEntry<Equipment>(new Equipment("Enchanted Robes", EquipmentType.Armor, 60, 115, armor: 5, intel: 9, mana: 30), 115, 6),
                new StockEntry<Equipment>(new Equipment("Sorcerer's Vestments", EquipmentType.Armor, 70, 145, armor: 6, intel: 12, mana: 40), 145, 4),
                new StockEntry<Equipment>(new Equipment("Arcane Robes", EquipmentType.Armor, 80, 180, armor: 7, intel: 14, mana: 50), 180, 3),
                new StockEntry<Equipment>(new Equipment("Archmage's Regalia", EquipmentType.Armor, 95, 250, armor: 9, intel: 22, mana: 80), 250, 2),
                new StockEntry<Equipment>(new Equipment("Necromancer's Shroud", EquipmentType.Armor, 90, 235, armor: 8, intel: 19, hp: 45, mana: 65), 235, 2),
                new StockEntry<Equipment>(new Equipment("Elementalist's Mantle", EquipmentType.Armor, 92, 245, armor: 8, intel: 21, mana: 70), 245, 1),

                // MAGE OFF-HAND
                new StockEntry<Equipment>(new Equipment("Spell Tome", EquipmentType.OffHand, 50, 75, intel: 6, mana: 22), 75, 9),
                new StockEntry<Equipment>(new Equipment("Crystal Orb", EquipmentType.OffHand, 60, 110, intel: 9, mana: 32), 110, 6),
                new StockEntry<Equipment>(new Equipment("Arcane Codex", EquipmentType.OffHand, 70, 150, intel: 12, mana: 45), 150, 4),
                new StockEntry<Equipment>(new Equipment("Focus Crystal", EquipmentType.OffHand, 55, 95, intel: 7, mana: 25), 95, 7),
                new StockEntry<Equipment>(new Equipment("Grimoire of Power", EquipmentType.OffHand, 75, 170, intel: 14, mana: 55), 170, 3),
                new StockEntry<Equipment>(new Equipment("Necromantic Tome", EquipmentType.OffHand, 80, 210, intel: 17, hp: 35, mana: 60), 210, 2),
                new StockEntry<Equipment>(new Equipment("Elemental Orb", EquipmentType.OffHand, 82, 220, intel: 18, mana: 65), 220, 2),
                new StockEntry<Equipment>(new Equipment("Void Sphere", EquipmentType.OffHand, 78, 195, intel: 16, mana: 58), 195, 2),

                // MAGE RINGS
                new StockEntry<Equipment>(new Equipment("Wizard's Ring", EquipmentType.Ring, 100, 65, intel: 5, mana: 15), 65, 8),
                new StockEntry<Equipment>(new Equipment("Ring of Intellect", EquipmentType.Ring, 100, 85, intel: 7), 85, 6),
                new StockEntry<Equipment>(new Equipment("Ring of Arcana", EquipmentType.Ring, 100, 110, intel: 9, mana: 28), 110, 4),
                new StockEntry<Equipment>(new Equipment("Ring of the Archmage", EquipmentType.Ring, 100, 165, intel: 14, mana: 50), 165, 2),
                new StockEntry<Equipment>(new Equipment("Necromancer's Band", EquipmentType.Ring, 100, 155, intel: 12, hp: 25, mana: 40), 155, 2),
                new StockEntry<Equipment>(new Equipment("Elementalist's Loop", EquipmentType.Ring, 100, 160, intel: 13, mana: 45), 160, 2),

                // MAGE NECKLACES
                new StockEntry<Equipment>(new Equipment("Arcane Amulet", EquipmentType.Necklace, 100, 80, intel: 6, mana: 20), 80, 7),
                new StockEntry<Equipment>(new Equipment("Amulet of Wisdom", EquipmentType.Necklace, 100, 105, intel: 8, mana: 28), 105, 5),
                new StockEntry<Equipment>(new Equipment("Arcane Pendant", EquipmentType.Necklace, 100, 140, intel: 11, mana: 40), 140, 3),
                new StockEntry<Equipment>(new Equipment("Archmage's Medallion", EquipmentType.Necklace, 100, 200, intel: 18, mana: 75), 200, 1),
                new StockEntry<Equipment>(new Equipment("Necromancer's Amulet", EquipmentType.Necklace, 100, 185, intel: 15, hp: 38, mana: 60), 185, 2),
                new StockEntry<Equipment>(new Equipment("Elemental Talisman", EquipmentType.Necklace, 100, 190, intel: 17, mana: 65), 190, 1),
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
                Console.WriteLine("\nMage Shop: 1) Buy Magic Items  2) Sell Magic Items  3) Buy Magic Equipment  4) Enchant Equipment  0) Leave");
                Console.Write("Choice: ");
                var input = Console.ReadLine() ?? string.Empty;
                if (input.Trim() == "0") return;

                switch (input.Trim())
                {
                    case "1":
                        BuyFromStock(party, _magicItems, "magic items");
                        break;
                    case "2":
                        SellToStock(party, _magicItems, "Mage");
                        break;
                    case "3":
                        BuyEquipment(party, _magicEquipment);
                        break;
                    case "4":
                        EnchantEquipment(party);
                        break;
                    default:
                        Console.WriteLine("Invalid choice.");
                        break;
                }
            }
        }

        private void EnchantEquipment(List<Character> party)
        {
            Console.WriteLine("Select who has equipment to enchant:");
            for (int i = 0; i < party.Count; i++) Console.WriteLine($"{i + 1}) {party[i].Name}");
            var who = Console.ReadLine() ?? string.Empty;
            if (!int.TryParse(who, out var whoIdx) || whoIdx < 1 || whoIdx > party.Count) { Console.WriteLine("Invalid."); return; }
            var c = party[whoIdx - 1];

            Console.WriteLine("Enchanting increases equipment durability by 20%.");
            Console.WriteLine("Cost: 50 gold + Magic Dust");

            if (c.Inventory.Gold < 50) { Console.WriteLine("Not enough gold."); return; }

            var slots = c.Inventory.Slots;
            int dustSlot = -1;
            for (int i = 0; i < slots.Count; i++)
            {
                if (slots[i]?.Name == "Magic Dust")
                {
                    dustSlot = i;
                    break;
                }
            }

            if (dustSlot == -1) { Console.WriteLine("You need Magic Dust to enchant!"); return; }

            var equipmentSlots = new List<(int idx, Equipment eq)>();
            for (int i = 0; i < slots.Count; i++)
            {
                if (slots[i] is Equipment eq)
                {
                    equipmentSlots.Add((i, eq));
                    Console.WriteLine($"{equipmentSlots.Count}) {eq.Name} [{eq.Type}] (Dur {eq.Durability}/{eq.MaxDurability})");
                }
            }

            if (equipmentSlots.Count == 0) { Console.WriteLine("No equipment to enchant."); return; }
            Console.WriteLine("Choose equipment to enchant:");
            var sel = Console.ReadLine() ?? string.Empty;
            if (!int.TryParse(sel, out var selIdx) || selIdx < 1 || selIdx > equipmentSlots.Count) { Console.WriteLine("Invalid."); return; }

            var chosen = equipmentSlots[selIdx - 1];
            c.Inventory.SpendGold(50);
            c.Inventory.RemoveItem(dustSlot);

            Console.WriteLine($"{chosen.eq.Name} has been enchanted! (This is a cosmetic feature - durability bonus not yet implemented)");
        }
    }
}
