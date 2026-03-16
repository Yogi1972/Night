using Night.Characters;
using Night.Shops;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rpg_Dungeon
{
    internal class BlacksmithShop : Shop
    {
        public override string ShopName => "The Rusty Anvil (Blacksmith)";

        private readonly List<StockEntry<GenericItem>> _ores;
        private readonly List<StockEntry<Equipment>> _metalArmor;

        public BlacksmithShop()
        {
            _ores = new List<StockEntry<GenericItem>>
            {
                new StockEntry<GenericItem>(new GenericItem("Copper Ore", 3), 3, 100),
                new StockEntry<GenericItem>(new GenericItem("Iron Ore", 5), 5, 80),
                new StockEntry<GenericItem>(new GenericItem("Steel Ingot", 12), 12, 40),
                new StockEntry<GenericItem>(new GenericItem("Silver Ore", 15), 15, 30),
                new StockEntry<GenericItem>(new GenericItem("Gold Nugget", 60), 60, 10),
                new StockEntry<GenericItem>(new GenericItem("Mithril Ore", 130), 130, 5),
                new StockEntry<GenericItem>(new GenericItem("Adamantite Bar", 200), 200, 2),
                new StockEntry<GenericItem>(new GenericItem("Coal", 2), 2, 120),
                new StockEntry<GenericItem>(new GenericItem("Whetstone", 8), 8, 60),
                new StockEntry<GenericItem>(new GenericItem("Forge Hammer", 25), 25, 15)
            };

            _metalArmor = new List<StockEntry<Equipment>>
            {
                // WARRIOR WEAPONS - Basic to Advanced
                new StockEntry<Equipment>(new Equipment("Iron Sword", EquipmentType.Weapon, 50, 75, str: 5), 75, 15),
                new StockEntry<Equipment>(new Equipment("Steel Axe", EquipmentType.Weapon, 60, 90, str: 7), 90, 12),
                new StockEntry<Equipment>(new Equipment("War Hammer", EquipmentType.Weapon, 65, 95, str: 8), 95, 10),
                new StockEntry<Equipment>(new Equipment("Battle Sword", EquipmentType.Weapon, 70, 110, str: 10), 110, 8),
                new StockEntry<Equipment>(new Equipment("Great Axe", EquipmentType.Weapon, 80, 130, str: 12), 130, 6),
                new StockEntry<Equipment>(new Equipment("Greatsword", EquipmentType.Weapon, 85, 145, str: 13, hp: 20), 145, 5),
                new StockEntry<Equipment>(new Equipment("Executioner's Axe", EquipmentType.Weapon, 90, 160, str: 15), 160, 4),
                new StockEntry<Equipment>(new Equipment("Warlord's Blade", EquipmentType.Weapon, 95, 180, str: 17, hp: 30), 180, 3),
                new StockEntry<Equipment>(new Equipment("Mace of Reckoning", EquipmentType.Weapon, 88, 155, str: 14, armor: 3), 155, 4),
                new StockEntry<Equipment>(new Equipment("Spiked Flail", EquipmentType.Weapon, 82, 140, str: 12, agi: 4), 140, 5),

                // WARRIOR CHAMPION WEAPONS
                new StockEntry<Equipment>(new Equipment("Paladin's Blade", EquipmentType.Weapon, 100, 220, str: 12, intel: 10, mana: 25), 220, 2),
                new StockEntry<Equipment>(new Equipment("Berserker's Greataxe", EquipmentType.Weapon, 105, 240, str: 18, hp: 40), 240, 2),
                new StockEntry<Equipment>(new Equipment("Guardian's Mace", EquipmentType.Weapon, 100, 210, str: 11, armor: 6, hp: 50), 210, 2),

                // WARRIOR ARMOR - Basic to Advanced
                new StockEntry<Equipment>(new Equipment("Chainmail Armor", EquipmentType.Armor, 70, 100, armor: 7, str: 3, hp: 20), 100, 10),
                new StockEntry<Equipment>(new Equipment("Scale Mail", EquipmentType.Armor, 75, 115, armor: 8, str: 3, hp: 25), 115, 8),
                new StockEntry<Equipment>(new Equipment("Plate Helmet", EquipmentType.Armor, 40, 60, armor: 3, hp: 15), 60, 12),
                new StockEntry<Equipment>(new Equipment("Iron Breastplate", EquipmentType.Armor, 75, 115, armor: 9, str: 4, hp: 30), 115, 7),
                new StockEntry<Equipment>(new Equipment("Steel Plate Armor", EquipmentType.Armor, 90, 150, armor: 12, str: 6, hp: 40), 150, 5),
                new StockEntry<Equipment>(new Equipment("Heavy Plate Armor", EquipmentType.Armor, 100, 175, armor: 15, str: 7, hp: 50), 175, 4),
                new StockEntry<Equipment>(new Equipment("Knight's Armor", EquipmentType.Armor, 95, 165, armor: 13, str: 5, hp: 45), 165, 4),
                new StockEntry<Equipment>(new Equipment("Reinforced Plate", EquipmentType.Armor, 105, 190, armor: 16, str: 8, hp: 60), 190, 3),
                new StockEntry<Equipment>(new Equipment("Dragonscale Plate", EquipmentType.Armor, 110, 220, armor: 18, str: 9, hp: 70, agi: 3), 220, 2),

                // WARRIOR CHAMPION ARMOR
                new StockEntry<Equipment>(new Equipment("Paladin's Plate", EquipmentType.Armor, 115, 250, armor: 16, str: 7, intel: 8, hp: 50, mana: 20), 250, 2),
                new StockEntry<Equipment>(new Equipment("Guardian's Bulwark", EquipmentType.Armor, 120, 280, armor: 20, str: 6, hp: 80), 280, 1),
                new StockEntry<Equipment>(new Equipment("Berserker's Warplate", EquipmentType.Armor, 110, 240, armor: 14, str: 10, stamina: 40), 240, 2),

                // SHIELDS & OFF-HAND
                new StockEntry<Equipment>(new Equipment("Wooden Shield", EquipmentType.OffHand, 40, 55, armor: 3, hp: 10), 55, 15),
                new StockEntry<Equipment>(new Equipment("Iron Shield", EquipmentType.OffHand, 60, 85, armor: 5, str: 2, hp: 20), 85, 10),
                new StockEntry<Equipment>(new Equipment("Steel Shield", EquipmentType.OffHand, 75, 120, armor: 8, str: 3, hp: 30), 120, 6),
                new StockEntry<Equipment>(new Equipment("Tower Shield", EquipmentType.OffHand, 90, 180, armor: 12, str: 4, hp: 50), 180, 4),
                new StockEntry<Equipment>(new Equipment("Buckler", EquipmentType.OffHand, 35, 45, armor: 2, agi: 2), 45, 12),
                new StockEntry<Equipment>(new Equipment("Kite Shield", EquipmentType.OffHand, 70, 110, armor: 9, str: 3, agi: 2), 110, 5),
                new StockEntry<Equipment>(new Equipment("Guardian's Aegis", EquipmentType.OffHand, 100, 250, armor: 16, str: 6, hp: 70), 250, 2),
                new StockEntry<Equipment>(new Equipment("Paladin's Shield", EquipmentType.OffHand, 95, 230, armor: 14, str: 5, intel: 7, mana: 25), 230, 2),

                // ACCESSORIES - Warrior/Paladin/Guardian/Berserker
                new StockEntry<Equipment>(new Equipment("Iron Bracers", EquipmentType.Accessory, 50, 50, armor: 2, str: 2), 50, 10),
                new StockEntry<Equipment>(new Equipment("Steel Gauntlets", EquipmentType.Accessory, 60, 75, armor: 3, str: 4), 75, 8),
                new StockEntry<Equipment>(new Equipment("Reinforced Boots", EquipmentType.Accessory, 55, 65, armor: 2, agi: 2, hp: 10), 65, 9),
                new StockEntry<Equipment>(new Equipment("Gauntlets of Power", EquipmentType.Accessory, 70, 110, armor: 4, str: 7), 110, 5),
                new StockEntry<Equipment>(new Equipment("Plate Boots", EquipmentType.Accessory, 65, 85, armor: 3, str: 3, hp: 15), 85, 6),
                new StockEntry<Equipment>(new Equipment("Champion's Gauntlets", EquipmentType.Accessory, 80, 145, armor: 5, str: 9, hp: 20), 145, 3),
                new StockEntry<Equipment>(new Equipment("Berserker's Bracers", EquipmentType.Accessory, 75, 130, str: 8, stamina: 20), 130, 3),
                new StockEntry<Equipment>(new Equipment("Guardian's Vambraces", EquipmentType.Accessory, 85, 155, armor: 6, str: 6, hp: 35), 155, 2),

                // ROGUE WEAPONS - Daggers and Light Blades
                new StockEntry<Equipment>(new Equipment("Steel Dagger", EquipmentType.Weapon, 45, 60, str: 3, agi: 5), 60, 12),
                new StockEntry<Equipment>(new Equipment("Curved Blade", EquipmentType.Weapon, 50, 75, agi: 8), 75, 9),
                new StockEntry<Equipment>(new Equipment("Twin Blades", EquipmentType.Weapon, 55, 90, str: 4, agi: 9), 90, 7),
                new StockEntry<Equipment>(new Equipment("Poison Dagger", EquipmentType.Weapon, 50, 95, agi: 10, intel: 3), 95, 6),
                new StockEntry<Equipment>(new Equipment("Shadow Blade", EquipmentType.Weapon, 60, 110, agi: 12), 110, 5),
                new StockEntry<Equipment>(new Equipment("Assassin's Kris", EquipmentType.Weapon, 70, 150, agi: 16, str: 5), 150, 3),
                new StockEntry<Equipment>(new Equipment("Void Dagger", EquipmentType.Weapon, 75, 170, agi: 18, intel: 4), 170, 2),

                // TEMPLAR WEAPONS (Hybrid Str/Int)
                new StockEntry<Equipment>(new Equipment("Holy Mace", EquipmentType.Weapon, 65, 125, str: 8, intel: 8), 125, 4),
                new StockEntry<Equipment>(new Equipment("Templar's Hammer", EquipmentType.Weapon, 85, 200, str: 12, intel: 12, armor: 5), 200, 2),
                new StockEntry<Equipment>(new Equipment("Divine Judgment", EquipmentType.Weapon, 90, 230, str: 14, intel: 14, hp: 40), 230, 1),
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
                Console.WriteLine("\nBlacksmith: 1) Buy Ore  2) Sell Ore  3) Repair Equipment 4) Buy equipment 0) Leave");
                Console.Write("Choice: ");
                var input = Console.ReadLine() ?? string.Empty;
                if (input.Trim() == "0") return;
                if (input.Trim() == "1")
                {
                    BuyOre(party);
                }
                else if (input.Trim() == "2")
                {
                    SellOre(party);
                }
                else if (input.Trim() == "3")
                {
                    RepairAllEquipment(party);
                }
                else if (input.Trim() == "4")
                {
                    BuyEquipment(party, _metalArmor);
                }
                else
                {
                    Console.WriteLine("Invalid choice.");
                }
            }
        }

        private void BuyOre(List<Character> party)
        {
            BuyFromStock(party, _ores, "ores");
        }

        private void SellOre(List<Character> party)
        {
            SellToStock(party, _ores, "Blacksmith");
        }
    }
}
