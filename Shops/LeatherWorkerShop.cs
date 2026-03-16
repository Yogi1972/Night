using Night.Characters;
using Night.Shops;
using System;
using System.Collections.Generic;

namespace Rpg_Dungeon
{
    internal class LeatherWorkerShop : Shop
    {
        public override string ShopName => "Tanner's Workshop (Leather Worker)";

        private readonly List<StockEntry<GenericItem>> _leatherMaterials;
        private readonly List<StockEntry<Equipment>> _leatherGoods;

        public LeatherWorkerShop()
        {
            _leatherMaterials = new List<StockEntry<GenericItem>>
            {
                new StockEntry<GenericItem>(new GenericItem("Leather Scraps", 3), 3, 80),
                new StockEntry<GenericItem>(new GenericItem("Wolf Pelt", 8), 8, 50),
                new StockEntry<GenericItem>(new GenericItem("Bear Hide", 15), 15, 30),
                new StockEntry<GenericItem>(new GenericItem("Troll Hide", 25), 25, 20),
                new StockEntry<GenericItem>(new GenericItem("Dragon Leather", 120), 120, 5),
                new StockEntry<GenericItem>(new GenericItem("Shadow Silk", 95), 95, 8),
                new StockEntry<GenericItem>(new GenericItem("Tanning Oil", 5), 5, 60),
                new StockEntry<GenericItem>(new GenericItem("Leather Dye", 7), 7, 45),
                new StockEntry<GenericItem>(new GenericItem("Reinforcement Studs", 10), 10, 35),
                new StockEntry<GenericItem>(new GenericItem("Enchanted Leather", 70), 70, 10)
            };

            _leatherGoods = new List<StockEntry<Equipment>>
            {
                // ROGUE ARMOR - Basic to Advanced
                new StockEntry<Equipment>(new Equipment("Leather Armor", EquipmentType.Armor, 40, 60, armor: 4, agi: 3, hp: 10), 60, 12),
                new StockEntry<Equipment>(new Equipment("Studded Leather", EquipmentType.Armor, 50, 85, armor: 5, agi: 5, hp: 15), 85, 9),
                new StockEntry<Equipment>(new Equipment("Reinforced Leather", EquipmentType.Armor, 60, 110, armor: 6, agi: 7, hp: 20), 110, 7),
                new StockEntry<Equipment>(new Equipment("Hardened Leather", EquipmentType.Armor, 70, 135, armor: 7, agi: 9, hp: 28), 135, 5),
                new StockEntry<Equipment>(new Equipment("Shadow Leather", EquipmentType.Armor, 80, 165, armor: 8, agi: 11, stamina: 20), 165, 4),
                new StockEntry<Equipment>(new Equipment("Dragonskin Armor", EquipmentType.Armor, 90, 200, armor: 10, agi: 13, hp: 35), 200, 2),

                // ROGUE CHAMPION ARMOR
                new StockEntry<Equipment>(new Equipment("Assassin's Shroud", EquipmentType.Armor, 95, 230, armor: 9, agi: 18, stamina: 40), 230, 2),
                new StockEntry<Equipment>(new Equipment("Ranger's Chainmail", EquipmentType.Armor, 100, 220, armor: 11, agi: 15, hp: 45), 220, 2),
                new StockEntry<Equipment>(new Equipment("Shadowblade's Cloak", EquipmentType.Armor, 92, 225, armor: 8, agi: 19, intel: 6), 225, 1),

                // ROGUE WEAPONS
                new StockEntry<Equipment>(new Equipment("Steel Dagger", EquipmentType.Weapon, 45, 60, str: 3, agi: 5), 60, 12),
                new StockEntry<Equipment>(new Equipment("Curved Blade", EquipmentType.Weapon, 50, 75, agi: 8), 75, 10),
                new StockEntry<Equipment>(new Equipment("Twin Blades", EquipmentType.Weapon, 55, 90, str: 4, agi: 9), 90, 8),
                new StockEntry<Equipment>(new Equipment("Poison Dagger", EquipmentType.Weapon, 50, 95, agi: 10, intel: 3), 95, 7),
                new StockEntry<Equipment>(new Equipment("Shadow Blade", EquipmentType.Weapon, 60, 110, agi: 12), 110, 6),
                new StockEntry<Equipment>(new Equipment("Rapier", EquipmentType.Weapon, 58, 105, agi: 11, str: 3), 105, 6),
                new StockEntry<Equipment>(new Equipment("Stiletto", EquipmentType.Weapon, 52, 88, agi: 9, str: 2), 88, 8),

                // ROGUE CHAMPION WEAPONS
                new StockEntry<Equipment>(new Equipment("Assassin's Kris", EquipmentType.Weapon, 70, 150, agi: 16, str: 5), 150, 3),
                new StockEntry<Equipment>(new Equipment("Death's Touch", EquipmentType.Weapon, 75, 175, agi: 19, stamina: 25), 175, 2),
                new StockEntry<Equipment>(new Equipment("Ranger's Longbow", EquipmentType.Weapon, 68, 145, agi: 15, hp: 25), 145, 3),
                new StockEntry<Equipment>(new Equipment("Hunter's Pride", EquipmentType.Weapon, 72, 165, agi: 17, str: 5), 165, 2),
                new StockEntry<Equipment>(new Equipment("Shadowblade's Edge", EquipmentType.Weapon, 70, 160, agi: 18, intel: 6), 160, 2),
                new StockEntry<Equipment>(new Equipment("Void Dagger", EquipmentType.Weapon, 76, 185, agi: 20), 185, 1),

                // ROGUE OFF-HAND
                new StockEntry<Equipment>(new Equipment("Throwing Knives", EquipmentType.OffHand, 35, 50, agi: 4, str: 2), 50, 12),
                new StockEntry<Equipment>(new Equipment("Parrying Dagger", EquipmentType.OffHand, 40, 70, agi: 6, armor: 2), 70, 9),
                new StockEntry<Equipment>(new Equipment("Poison Vials", EquipmentType.OffHand, 38, 80, agi: 7, intel: 3), 80, 7),
                new StockEntry<Equipment>(new Equipment("Shadow Shuriken", EquipmentType.OffHand, 45, 95, agi: 9), 95, 6),
                new StockEntry<Equipment>(new Equipment("Assassin's Dirk", EquipmentType.OffHand, 50, 110, agi: 11, str: 4), 110, 5),
                new StockEntry<Equipment>(new Equipment("Assassin's Toolkit", EquipmentType.OffHand, 60, 160, agi: 15, stamina: 28), 160, 2),
                new StockEntry<Equipment>(new Equipment("Ranger's Quiver", EquipmentType.OffHand, 58, 150, agi: 13, hp: 22), 150, 3),

                // ROGUE ACCESSORIES
                new StockEntry<Equipment>(new Equipment("Leather Boots", EquipmentType.Accessory, 40, 50, agi: 4), 50, 12),
                new StockEntry<Equipment>(new Equipment("Leather Gloves", EquipmentType.Accessory, 38, 48, agi: 3), 48, 13),
                new StockEntry<Equipment>(new Equipment("Rogue's Gloves", EquipmentType.Accessory, 50, 70, agi: 6, stamina: 10), 70, 8),
                new StockEntry<Equipment>(new Equipment("Shadow Boots", EquipmentType.Accessory, 55, 85, agi: 8, stamina: 12), 85, 6),
                new StockEntry<Equipment>(new Equipment("Gloves of Precision", EquipmentType.Accessory, 60, 95, agi: 9), 95, 5),
                new StockEntry<Equipment>(new Equipment("Shadowweave Gloves", EquipmentType.Accessory, 70, 130, agi: 12, stamina: 20), 130, 3),
                new StockEntry<Equipment>(new Equipment("Assassin's Boots", EquipmentType.Accessory, 75, 145, agi: 14, stamina: 25), 145, 3),
                new StockEntry<Equipment>(new Equipment("Ranger's Bracers", EquipmentType.Accessory, 68, 135, agi: 11, hp: 22), 135, 3),

                // HYBRID ARMOR (Light Armor for Templars/Druids)
                new StockEntry<Equipment>(new Equipment("Templar's Leather", EquipmentType.Armor, 85, 155, armor: 9, str: 6, intel: 6, hp: 30), 155, 3),
                new StockEntry<Equipment>(new Equipment("Druid's Hide", EquipmentType.Armor, 82, 165, armor: 8, intel: 9, hp: 45, stamina: 22), 165, 2),
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
                Console.WriteLine("\nLeather Worker: 1) Buy Materials  2) Sell Materials  3) Buy Leather Goods  4) Repair Leather  0) Leave");
                Console.Write("Choice: ");
                var input = Console.ReadLine() ?? string.Empty;
                if (input.Trim() == "0") return;

                switch (input.Trim())
                {
                    case "1":
                        BuyFromStock(party, _leatherMaterials, "leather materials");
                        break;
                    case "2":
                        SellToStock(party, _leatherMaterials, "Leather Worker");
                        break;
                    case "3":
                        BuyEquipment(party, _leatherGoods);
                        break;
                    case "4":
                        RepairEquipmentByType(party, EquipmentType.Armor, "leather");
                        break;
                    default:
                        Console.WriteLine("Invalid choice.");
                        break;
                }
            }
        }
    }
}
