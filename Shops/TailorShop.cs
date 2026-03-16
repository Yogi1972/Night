using Night.Characters;
using Night.Shops;
using System;
using System.Collections.Generic;

namespace Rpg_Dungeon
{
    internal class TailorShop : Shop
    {
        public override string ShopName => "Fine Threads (Tailor)";

        private readonly List<StockEntry<GenericItem>> _clothMaterials;
        private readonly List<StockEntry<Equipment>> _clothGoods;

        public TailorShop()
        {
            _clothMaterials = new List<StockEntry<GenericItem>>
            {
                new StockEntry<GenericItem>(new GenericItem("Torn Cloth", 2), 2, 100),
                new StockEntry<GenericItem>(new GenericItem("Linen", 6), 6, 70),
                new StockEntry<GenericItem>(new GenericItem("Fine Linen", 12), 12, 40),
                new StockEntry<GenericItem>(new GenericItem("Silk", 18), 18, 30),
                new StockEntry<GenericItem>(new GenericItem("Spider Silk", 10), 10, 45),
                new StockEntry<GenericItem>(new GenericItem("Enchanted Thread", 20), 20, 25),
                new StockEntry<GenericItem>(new GenericItem("Mooncloth", 45), 45, 15),
                new StockEntry<GenericItem>(new GenericItem("Shadowweave Cloth", 55), 55, 12),
                new StockEntry<GenericItem>(new GenericItem("Spellthread", 35), 35, 20),
                new StockEntry<GenericItem>(new GenericItem("Eternal Thread", 90), 90, 6),
                new StockEntry<GenericItem>(new GenericItem("Imbued Silk", 65), 65, 10)
            };

            _clothGoods = new List<StockEntry<Equipment>>
            {
                // MAGE ARMOR - Basic to Advanced
                new StockEntry<Equipment>(new Equipment("Cloth Robes", EquipmentType.Armor, 35, 50, armor: 3, intel: 4, mana: 12), 50, 12),
                new StockEntry<Equipment>(new Equipment("Mage Robes", EquipmentType.Armor, 40, 75, armor: 4, intel: 6, mana: 20), 75, 10),
                new StockEntry<Equipment>(new Equipment("Enchanted Robes", EquipmentType.Armor, 50, 105, armor: 5, intel: 9, mana: 30), 105, 7),
                new StockEntry<Equipment>(new Equipment("Sorcerer's Vestments", EquipmentType.Armor, 60, 135, armor: 6, intel: 12, mana: 40), 135, 5),
                new StockEntry<Equipment>(new Equipment("Arcane Robes", EquipmentType.Armor, 70, 170, armor: 7, intel: 14, mana: 50), 170, 4),
                new StockEntry<Equipment>(new Equipment("Voidweave Robes", EquipmentType.Armor, 75, 195, armor: 8, intel: 16, mana: 60), 195, 3),

                // MAGE CHAMPION ARMOR
                new StockEntry<Equipment>(new Equipment("Archmage's Regalia", EquipmentType.Armor, 95, 280, armor: 10, intel: 24, mana: 90), 280, 2),
                new StockEntry<Equipment>(new Equipment("Necromancer's Shroud", EquipmentType.Armor, 90, 260, armor: 9, intel: 21, hp: 50, mana: 75), 260, 2),
                new StockEntry<Equipment>(new Equipment("Elementalist's Mantle", EquipmentType.Armor, 92, 270, armor: 9, intel: 23, mana: 80), 270, 1),

                // PRIEST ARMOR - Basic to Advanced
                new StockEntry<Equipment>(new Equipment("Acolyte Robes", EquipmentType.Armor, 38, 55, armor: 3, intel: 5, mana: 15), 55, 11),
                new StockEntry<Equipment>(new Equipment("Priest's Vestments", EquipmentType.Armor, 45, 80, armor: 4, intel: 7, mana: 22, hp: 18), 80, 9),
                new StockEntry<Equipment>(new Equipment("Holy Raiment", EquipmentType.Armor, 55, 115, armor: 6, intel: 10, mana: 35, hp: 30), 115, 6),
                new StockEntry<Equipment>(new Equipment("Divine Vestments", EquipmentType.Armor, 65, 150, armor: 7, intel: 12, mana: 45, hp: 40), 150, 4),
                new StockEntry<Equipment>(new Equipment("Sacred Robes", EquipmentType.Armor, 75, 185, armor: 8, intel: 14, mana: 55, hp: 50), 185, 3),

                // PRIEST CHAMPION ARMOR
                new StockEntry<Equipment>(new Equipment("Templar's Vestments", EquipmentType.Armor, 88, 240, armor: 10, str: 9, intel: 13, hp: 55), 240, 2),
                new StockEntry<Equipment>(new Equipment("Druid's Nature Guard", EquipmentType.Armor, 85, 255, armor: 9, intel: 14, hp: 60, stamina: 30), 255, 2),
                new StockEntry<Equipment>(new Equipment("Oracle's Sacred Robes", EquipmentType.Armor, 90, 275, armor: 8, intel: 20, mana: 85), 275, 1),

                // PRIEST WEAPONS
                new StockEntry<Equipment>(new Equipment("Wooden Staff", EquipmentType.Weapon, 48, 65, intel: 6, mana: 15), 65, 10),
                new StockEntry<Equipment>(new Equipment("Blessed Staff", EquipmentType.Weapon, 58, 95, intel: 10, mana: 28), 95, 7),
                new StockEntry<Equipment>(new Equipment("Divine Rod", EquipmentType.Weapon, 62, 115, intel: 11, hp: 22), 115, 6),
                new StockEntry<Equipment>(new Equipment("Radiant Mace", EquipmentType.Weapon, 68, 145, str: 7, intel: 12, mana: 35), 145, 4),
                new StockEntry<Equipment>(new Equipment("Staff of Light", EquipmentType.Weapon, 72, 165, intel: 14, mana: 45, hp: 30), 165, 3),

                // PRIEST OFF-HAND
                new StockEntry<Equipment>(new Equipment("Prayer Book", EquipmentType.OffHand, 40, 65, intel: 5, mana: 18, hp: 12), 65, 10),
                new StockEntry<Equipment>(new Equipment("Holy Symbol", EquipmentType.OffHand, 50, 90, intel: 7, mana: 25, hp: 18), 90, 7),
                new StockEntry<Equipment>(new Equipment("Blessed Tome", EquipmentType.OffHand, 60, 135, intel: 10, mana: 38, hp: 28), 135, 5),
                new StockEntry<Equipment>(new Equipment("Divine Relic", EquipmentType.OffHand, 70, 170, intel: 12, mana: 48, hp: 38), 170, 3),
                new StockEntry<Equipment>(new Equipment("Sanctuary Codex", EquipmentType.OffHand, 78, 200, intel: 15, mana: 60, hp: 45), 200, 2),

                // CLOTH ACCESSORIES (Mage & Priest)
                new StockEntry<Equipment>(new Equipment("Silk Cloak", EquipmentType.Accessory, 45, 65, intel: 4, mana: 15), 65, 9),
                new StockEntry<Equipment>(new Equipment("Enchanted Cloak", EquipmentType.Accessory, 55, 90, intel: 6, mana: 22), 90, 6),
                new StockEntry<Equipment>(new Equipment("Mage's Gloves", EquipmentType.Accessory, 50, 75, intel: 5, mana: 18), 75, 8),
                new StockEntry<Equipment>(new Equipment("Sorcerer's Gloves", EquipmentType.Accessory, 60, 105, intel: 8, mana: 28), 105, 5),
                new StockEntry<Equipment>(new Equipment("Holy Gauntlets", EquipmentType.Accessory, 65, 120, intel: 7, armor: 3, hp: 25), 120, 4),
                new StockEntry<Equipment>(new Equipment("Archmage's Gloves", EquipmentType.Accessory, 80, 175, intel: 14, mana: 45), 175, 2),
                new StockEntry<Equipment>(new Equipment("Blessed Sandals", EquipmentType.Accessory, 58, 95, intel: 6, hp: 20, mana: 15), 95, 6),
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
                Console.WriteLine("\nTailor: 1) Buy Cloth Materials  2) Sell Materials  3) Buy Cloth Goods  4) Repair Cloth  0) Leave");
                Console.Write("Choice: ");
                var input = Console.ReadLine() ?? string.Empty;
                if (input.Trim() == "0") return;

                switch (input.Trim())
                {
                    case "1":
                        BuyFromStock(party, _clothMaterials, "cloth materials");
                        break;
                    case "2":
                        SellToStock(party, _clothMaterials, "Tailor");
                        break;
                    case "3":
                        BuyEquipment(party, _clothGoods);
                        break;
                    case "4":
                        RepairEquipmentByType(party, EquipmentType.Armor, "cloth");
                        break;
                    default:
                        Console.WriteLine("Invalid choice.");
                        break;
                }
            }
        }
    }
}
