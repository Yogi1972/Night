using Night.Characters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rpg_Dungeon
{
    internal static class Mobs
    {
        private static readonly Random _rng = new Random();

        private static readonly List<(string Name, int BaseHP, int BaseStr, int BaseAgi, int BaseInt, int GoldMin, int GoldMax, int BaseAR)> _mobTemplates = new()
        {
            ("Goblin Warrior", 20, 7, 6, 2, 5, 15, 1),
            ("Goblin Rogue", 18, 4, 12, 2, 6, 16, 0),
            ("Goblin Shaman", 16, 3, 6, 10, 8, 18, 0),
            ("Orc Berserker", 40, 15, 5, 2, 10, 25, 6),
            ("Orc Hunter", 38, 10, 10, 3, 11, 26, 4),
            ("Orc Priest", 42, 8, 5, 12, 15, 30, 5),
            ("Skeleton Warrior", 25, 9, 7, 3, 8, 20, 3),
            ("Skeleton Archer", 22, 6, 11, 3, 9, 21, 1),
            ("Skeleton Mage", 20, 4, 6, 13, 12, 25, 1),
            ("Zombie", 35, 10, 4, 2, 12, 22, 3),
            ("Zombie Brute", 45, 14, 3, 1, 10, 20, 5),
            ("Spider", 18, 6, 12, 3, 7, 18, 1),
            ("Giant Spider", 28, 8, 14, 4, 10, 22, 2),
            ("Wolf", 28, 9, 11, 4, 9, 20, 2),
            ("Dire Wolf", 38, 12, 13, 4, 12, 25, 3),
            ("Troll Warrior", 60, 18, 4, 2, 15, 35, 10),
            ("Troll Berserker", 65, 20, 5, 2, 18, 38, 9),
            ("Bandit Thug", 32, 12, 8, 4, 12, 28, 5),
            ("Bandit Assassin", 28, 8, 14, 5, 15, 32, 2),
            ("Bandit Leader", 40, 13, 12, 6, 20, 40, 6),
            ("Dark Mage", 30, 4, 7, 18, 18, 32, 1),
            ("Dark Priest", 32, 5, 6, 16, 20, 35, 2),
            ("Wraith", 35, 8, 10, 12, 20, 35, 3),
            ("Wraith Knight", 42, 12, 9, 10, 25, 40, 6),
            ("Ogre Warrior", 70, 20, 3, 2, 20, 40, 12),
            ("Ogre Brute", 80, 22, 4, 1, 22, 42, 13),
            ("Ogre Shaman", 65, 14, 4, 10, 25, 45, 8),
            ("Dragon Whelp", 80, 16, 14, 12, 40, 80, 12),
            ("Dragon", 100, 20, 12, 15, 50, 100, 15),
            ("Ancient Dragon", 130, 24, 14, 20, 75, 150, 18)
        };

        public static Mob GetRandomMobForParty(List<Character> party, int targetLevel)
        {
            if (party == null || party.Count == 0)
            {
                return CreateMobFromTemplate(0, 1);
            }

            int partyAvgLevel = (int)party.Average(c => c.Level);
            int mobLevel = Math.Max(1, targetLevel > 0 ? targetLevel : partyAvgLevel);

            return CreateMobFromTemplate(_rng.Next(_mobTemplates.Count), mobLevel);
        }

        private static Mob CreateMobFromTemplate(int templateIndex, int level)
        {
            var template = _mobTemplates[templateIndex % _mobTemplates.Count];

            var lootTable = new List<(Item, double)>
            {
                // Potions & Consumables
                (new GenericItem("Health Potion", 15), 0.22),
                (new GenericItem("Mana Potion", 20), 0.18),
                (new GenericItem("Stamina Potion", 18), 0.14),
                (new GenericItem("Greater Health Potion", 35), 0.09),
                (new GenericItem("Greater Mana Potion", 40), 0.07),
                (new GenericItem("Greater Stamina Potion", 38), 0.06),
                (new GenericItem("Elixir of Vitality", 50), 0.04),
                (new GenericItem("Elixir of Power", 55), 0.03),
                (new GenericItem("Elixir of Speed", 52), 0.03),
                (new GenericItem("Phoenix Down", 100), 0.02),

                // WARRIOR CLASS WEAPONS
                (new Equipment("Rusty Sword", EquipmentType.Weapon, 8, 15, str: 3), 0.13),
                (new Equipment("Common Sword", EquipmentType.Weapon, 12, 25, str: 5), 0.11),
                (new Equipment("Iron Axe", EquipmentType.Weapon, 14, 30, str: 6), 0.09),
                (new Equipment("Battle Mace", EquipmentType.Weapon, 15, 35, str: 7), 0.08),
                (new Equipment("Warhammer", EquipmentType.Weapon, 18, 45, str: 9), 0.06),
                (new Equipment("Greatsword", EquipmentType.Weapon, 20, 55, str: 11, hp: 15), 0.05),
                (new Equipment("Executioner's Axe", EquipmentType.Weapon, 22, 65, str: 13), 0.04),
                (new Equipment("Flaming Sword", EquipmentType.Weapon, 16, 60, str: 8, intel: 4), 0.04),

                // WARRIOR CHAMPION WEAPONS
                (new Equipment("Paladin's Blade", EquipmentType.Weapon, 20, 80, str: 10, intel: 8, mana: 20), 0.03),
                (new Equipment("Holy Avenger", EquipmentType.Weapon, 22, 95, str: 12, intel: 10, hp: 25), 0.02),
                (new Equipment("Berserker's Greataxe", EquipmentType.Weapon, 25, 90, str: 15, hp: 30), 0.02),
                (new Equipment("Rage Hammer", EquipmentType.Weapon, 24, 85, str: 14, stamina: 25), 0.02),
                (new Equipment("Guardian's Mace", EquipmentType.Weapon, 20, 75, str: 9, armor: 5, hp: 40), 0.03),
                (new Equipment("Tower Breaker", EquipmentType.Weapon, 26, 100, str: 13, armor: 6), 0.01),

                // MAGE CLASS WEAPONS
                (new Equipment("Apprentice Staff", EquipmentType.Weapon, 10, 30, intel: 6, mana: 10), 0.10),
                (new Equipment("Enchanted Staff", EquipmentType.Weapon, 12, 40, intel: 8, mana: 15), 0.08),
                (new Equipment("Mystic Wand", EquipmentType.Weapon, 10, 38, intel: 7, mana: 12), 0.07),
                (new Equipment("Crystal Staff", EquipmentType.Weapon, 14, 50, intel: 10, mana: 20), 0.06),
                (new Equipment("Sorcerer's Rod", EquipmentType.Weapon, 12, 48, intel: 9, mana: 18), 0.05),
                (new Equipment("Staff of the Magi", EquipmentType.Weapon, 16, 70, intel: 13, mana: 30), 0.03),

                // MAGE CHAMPION WEAPONS
                (new Equipment("Archmage's Staff", EquipmentType.Weapon, 18, 100, intel: 18, mana: 50), 0.02),
                (new Equipment("Meteor Staff", EquipmentType.Weapon, 20, 110, intel: 20, mana: 60), 0.01),
                (new Equipment("Necromancer's Scythe", EquipmentType.Weapon, 16, 95, intel: 15, mana: 40, hp: 30), 0.02),
                (new Equipment("Soul Reaper", EquipmentType.Weapon, 18, 105, intel: 17, mana: 45), 0.01),
                (new Equipment("Elementalist's Conduit", EquipmentType.Weapon, 17, 98, intel: 16, mana: 45), 0.02),
                (new Equipment("Staff of the Elements", EquipmentType.Weapon, 19, 108, intel: 19, mana: 55), 0.01),

                // ROGUE CLASS WEAPONS
                (new Equipment("Steel Dagger", EquipmentType.Weapon, 10, 28, str: 3, agi: 4), 0.09),
                (new Equipment("Twin Blades", EquipmentType.Weapon, 12, 40, str: 4, agi: 7), 0.07),
                (new Equipment("Longbow", EquipmentType.Weapon, 11, 32, agi: 6), 0.07),
                (new Equipment("Crossbow", EquipmentType.Weapon, 13, 38, agi: 7, str: 2), 0.06),
                (new Equipment("Poison Dagger", EquipmentType.Weapon, 10, 45, agi: 8, intel: 3), 0.05),
                (new Equipment("Shadow Blade", EquipmentType.Weapon, 14, 55, agi: 10), 0.04),

                // ROGUE CHAMPION WEAPONS
                (new Equipment("Assassin's Kris", EquipmentType.Weapon, 15, 85, agi: 15, str: 5), 0.02),
                (new Equipment("Death's Touch", EquipmentType.Weapon, 16, 95, agi: 17, stamina: 20), 0.01),
                (new Equipment("Ranger's Longbow", EquipmentType.Weapon, 14, 80, agi: 14, hp: 20), 0.02),
                (new Equipment("Hunter's Pride", EquipmentType.Weapon, 16, 90, agi: 16, str: 4), 0.01),
                (new Equipment("Shadowblade's Edge", EquipmentType.Weapon, 15, 88, agi: 16, intel: 5), 0.02),
                (new Equipment("Void Dagger", EquipmentType.Weapon, 17, 98, agi: 18), 0.01),

                // PRIEST CLASS WEAPONS
                (new Equipment("Wooden Staff", EquipmentType.Weapon, 10, 25, intel: 5, mana: 10), 0.09),
                (new Equipment("Holy Mace", EquipmentType.Weapon, 12, 35, str: 4, intel: 6), 0.07),
                (new Equipment("Blessed Staff", EquipmentType.Weapon, 14, 45, intel: 8, mana: 20), 0.06),
                (new Equipment("Divine Rod", EquipmentType.Weapon, 13, 42, intel: 7, hp: 15), 0.05),
                (new Equipment("Radiant Mace", EquipmentType.Weapon, 16, 58, str: 6, intel: 9, mana: 25), 0.04),

                // PRIEST CHAMPION WEAPONS
                (new Equipment("Templar's Hammer", EquipmentType.Weapon, 18, 85, str: 10, intel: 10, armor: 4), 0.02),
                (new Equipment("Divine Judgment", EquipmentType.Weapon, 20, 95, str: 12, intel: 12, hp: 30), 0.01),
                (new Equipment("Druid's Totem", EquipmentType.Weapon, 16, 82, intel: 11, hp: 35, stamina: 20), 0.02),
                (new Equipment("Nature's Embrace", EquipmentType.Weapon, 18, 92, intel: 13, hp: 40), 0.01),
                (new Equipment("Oracle's Scepter", EquipmentType.Weapon, 17, 88, intel: 16, mana: 50), 0.02),
                (new Equipment("Staff of Prophecy", EquipmentType.Weapon, 19, 98, intel: 18, mana: 60), 0.01),

                // WARRIOR ARMOR
                (new Equipment("Leather Armor", EquipmentType.Armor, 10, 20, armor: 4), 0.11),
                (new Equipment("Studded Leather Armor", EquipmentType.Armor, 12, 28, armor: 5, agi: 2), 0.09),
                (new Equipment("Chainmail Armor", EquipmentType.Armor, 16, 35, armor: 7), 0.08),
                (new Equipment("Scale Mail", EquipmentType.Armor, 18, 40, armor: 8, str: 2), 0.07),
                (new Equipment("Plate Armor", EquipmentType.Armor, 22, 55, armor: 12, str: 3), 0.05),
                (new Equipment("Heavy Plate Armor", EquipmentType.Armor, 25, 70, armor: 15, str: 4, hp: 25), 0.04),
                (new Equipment("Knight's Armor", EquipmentType.Armor, 24, 65, armor: 13, str: 3, hp: 20), 0.04),

                // WARRIOR CHAMPION ARMOR
                (new Equipment("Paladin's Plate", EquipmentType.Armor, 26, 90, armor: 14, str: 5, intel: 6, hp: 30), 0.02),
                (new Equipment("Guardian's Bulwark", EquipmentType.Armor, 30, 110, armor: 18, str: 4, hp: 60), 0.01),
                (new Equipment("Berserker's Hide", EquipmentType.Armor, 24, 85, armor: 10, str: 8, stamina: 30), 0.02),

                // MAGE ARMOR
                (new Equipment("Tattered Cloth Armor", EquipmentType.Armor, 6, 12, armor: 2), 0.12),
                (new Equipment("Mage Robes", EquipmentType.Armor, 8, 30, armor: 3, intel: 5, mana: 15), 0.09),
                (new Equipment("Enchanted Robes", EquipmentType.Armor, 10, 45, armor: 4, intel: 7, mana: 25), 0.07),
                (new Equipment("Sorcerer's Vestments", EquipmentType.Armor, 12, 58, armor: 5, intel: 9, mana: 35), 0.05),
                (new Equipment("Arcane Robes", EquipmentType.Armor, 14, 72, armor: 6, intel: 11, mana: 45), 0.04),

                // MAGE CHAMPION ARMOR
                (new Equipment("Archmage's Regalia", EquipmentType.Armor, 16, 105, armor: 7, intel: 18, mana: 70), 0.02),
                (new Equipment("Necromancer's Shroud", EquipmentType.Armor, 15, 98, armor: 6, intel: 15, hp: 35, mana: 50), 0.02),
                (new Equipment("Elementalist's Mantle", EquipmentType.Armor, 16, 102, armor: 7, intel: 17, mana: 60), 0.01),

                // ROGUE ARMOR
                (new Equipment("Leather Tunic", EquipmentType.Armor, 9, 22, armor: 3, agi: 3), 0.10),
                (new Equipment("Rogue's Leather", EquipmentType.Armor, 11, 32, armor: 4, agi: 5), 0.08),
                (new Equipment("Shadow Leather", EquipmentType.Armor, 13, 48, armor: 5, agi: 7, stamina: 15), 0.06),
                (new Equipment("Assassin's Garb", EquipmentType.Armor, 14, 62, armor: 6, agi: 9, stamina: 20), 0.04),

                // ROGUE CHAMPION ARMOR
                (new Equipment("Assassin's Shroud", EquipmentType.Armor, 16, 92, armor: 7, agi: 15, stamina: 30), 0.02),
                (new Equipment("Ranger's Chainmail", EquipmentType.Armor, 17, 88, armor: 8, agi: 12, hp: 35), 0.02),
                (new Equipment("Shadowblade's Cloak", EquipmentType.Armor, 15, 90, armor: 6, agi: 16, intel: 4), 0.01),

                // PRIEST ARMOR
                (new Equipment("Acolyte Robes", EquipmentType.Armor, 8, 25, armor: 3, intel: 4, mana: 12), 0.09),
                (new Equipment("Priest's Vestments", EquipmentType.Armor, 10, 38, armor: 4, intel: 6, mana: 20, hp: 15), 0.07),
                (new Equipment("Blessed Armor", EquipmentType.Armor, 20, 65, armor: 10, intel: 5, hp: 30), 0.04),
                (new Equipment("Holy Raiment", EquipmentType.Armor, 14, 55, armor: 6, intel: 8, mana: 30, hp: 25), 0.05),

                // PRIEST CHAMPION ARMOR
                (new Equipment("Templar's Plate", EquipmentType.Armor, 24, 95, armor: 12, str: 8, intel: 10, hp: 40), 0.02),
                (new Equipment("Druid's Nature Guard", EquipmentType.Armor, 18, 90, armor: 8, intel: 9, hp: 45, stamina: 25), 0.02),
                (new Equipment("Oracle's Sacred Robes", EquipmentType.Armor, 16, 100, armor: 7, intel: 16, mana: 70), 0.01),

                // WARRIOR SHIELDS & OFF-HAND
                (new Equipment("Wooden Shield", EquipmentType.OffHand, 12, 22, armor: 3), 0.11),
                (new Equipment("Iron Shield", EquipmentType.OffHand, 16, 35, armor: 5, str: 2), 0.08),
                (new Equipment("Steel Shield", EquipmentType.OffHand, 20, 48, armor: 7, str: 3), 0.06),
                (new Equipment("Tower Shield", EquipmentType.OffHand, 24, 65, armor: 10, str: 4, hp: 30), 0.04),
                (new Equipment("Kite Shield", EquipmentType.OffHand, 18, 52, armor: 8, str: 3, agi: 2), 0.05),
                (new Equipment("Guardian's Aegis", EquipmentType.OffHand, 28, 95, armor: 14, str: 5, hp: 50), 0.02),
                (new Equipment("Paladin's Shield", EquipmentType.OffHand, 26, 90, armor: 12, str: 4, intel: 6, mana: 20), 0.02),

                // MAGE OFF-HAND
                (new Equipment("Spell Tome", EquipmentType.OffHand, 8, 40, intel: 6, mana: 20), 0.07),
                (new Equipment("Magical Orb", EquipmentType.OffHand, 10, 45, intel: 7, mana: 25), 0.05),
                (new Equipment("Crystal Focus", EquipmentType.OffHand, 12, 55, intel: 9, mana: 35), 0.04),
                (new Equipment("Grimoire of Power", EquipmentType.OffHand, 14, 70, intel: 12, mana: 45), 0.03),
                (new Equipment("Arcane Codex", EquipmentType.OffHand, 16, 95, intel: 16, mana: 60), 0.02),
                (new Equipment("Necromantic Tome", EquipmentType.OffHand, 15, 88, intel: 14, hp: 25, mana: 40), 0.02),
                (new Equipment("Elemental Orb", EquipmentType.OffHand, 16, 92, intel: 15, mana: 50), 0.01),

                // ROGUE OFF-HAND
                (new Equipment("Throwing Knives", EquipmentType.OffHand, 8, 30, agi: 4, str: 2), 0.07),
                (new Equipment("Parrying Dagger", EquipmentType.OffHand, 10, 35, agi: 6, armor: 2), 0.06),
                (new Equipment("Poison Vials", EquipmentType.OffHand, 6, 42, agi: 5, intel: 3), 0.04),
                (new Equipment("Shadow Shuriken", EquipmentType.OffHand, 8, 50, agi: 8), 0.03),
                (new Equipment("Assassin's Toolkit", EquipmentType.OffHand, 12, 75, agi: 12, stamina: 20), 0.02),

                // PRIEST OFF-HAND
                (new Equipment("Prayer Beads", EquipmentType.OffHand, 10, 35, intel: 5, mana: 15), 0.06),
                (new Equipment("Holy Symbol", EquipmentType.OffHand, 12, 45, intel: 7, mana: 20, hp: 15), 0.05),
                (new Equipment("Divine Relic", EquipmentType.OffHand, 14, 60, intel: 9, mana: 30, hp: 25), 0.03),
                (new Equipment("Templar's Codex", EquipmentType.OffHand, 16, 85, str: 6, intel: 10, mana: 25), 0.02),
                (new Equipment("Druid's Idol", EquipmentType.OffHand, 15, 82, intel: 11, hp: 30, stamina: 20), 0.02),

                // WARRIOR RINGS
                (new Equipment("Iron Ring", EquipmentType.Ring, 100, 25, str: 2), 0.09),
                (new Equipment("Ring of Might", EquipmentType.Ring, 100, 40, str: 4), 0.06),
                (new Equipment("Ring of the Berserker", EquipmentType.Ring, 100, 65, str: 6, stamina: 15), 0.03),
                (new Equipment("Guardian's Band", EquipmentType.Ring, 100, 70, str: 4, armor: 4, hp: 25), 0.02),
                (new Equipment("Paladin's Ring", EquipmentType.Ring, 100, 75, str: 5, intel: 5, mana: 15), 0.02),

                // MAGE RINGS
                (new Equipment("Gold Ring", EquipmentType.Ring, 100, 45, intel: 4), 0.06),
                (new Equipment("Ring of Arcana", EquipmentType.Ring, 100, 55, intel: 6, mana: 20), 0.05),
                (new Equipment("Ring of the Archmage", EquipmentType.Ring, 100, 85, intel: 10, mana: 35), 0.02),
                (new Equipment("Necromancer's Band", EquipmentType.Ring, 100, 80, intel: 8, hp: 20, mana: 25), 0.02),
                (new Equipment("Elementalist's Loop", EquipmentType.Ring, 100, 82, intel: 9, mana: 30), 0.02),

                // ROGUE RINGS
                (new Equipment("Silver Ring", EquipmentType.Ring, 100, 35, agi: 3), 0.07),
                (new Equipment("Ring of Swiftness", EquipmentType.Ring, 100, 50, agi: 5, stamina: 10), 0.05),
                (new Equipment("Assassin's Band", EquipmentType.Ring, 100, 80, agi: 9, stamina: 20), 0.02),
                (new Equipment("Ranger's Ring", EquipmentType.Ring, 100, 75, agi: 8, hp: 20), 0.02),
                (new Equipment("Shadow Ring", EquipmentType.Ring, 100, 85, agi: 10, intel: 3), 0.02),

                // PRIEST RINGS
                (new Equipment("Ring of Faith", EquipmentType.Ring, 100, 48, intel: 5, mana: 15), 0.05),
                (new Equipment("Ring of Devotion", EquipmentType.Ring, 100, 60, intel: 7, hp: 20, mana: 20), 0.04),
                (new Equipment("Templar's Signet", EquipmentType.Ring, 100, 82, str: 6, intel: 8, armor: 3), 0.02),
                (new Equipment("Druid's Band", EquipmentType.Ring, 100, 78, intel: 8, hp: 30), 0.02),
                (new Equipment("Oracle's Circle", EquipmentType.Ring, 100, 88, intel: 11, mana: 40), 0.01),

                // UNIVERSAL RINGS
                (new Equipment("Ring of Vitality", EquipmentType.Ring, 100, 50, hp: 20), 0.05),
                (new Equipment("Ring of Power", EquipmentType.Ring, 100, 55, str: 3, intel: 3), 0.04),
                (new Equipment("Ring of Balance", EquipmentType.Ring, 100, 60, str: 2, agi: 2, intel: 2), 0.03),
                (new Equipment("Ring of the Champion", EquipmentType.Ring, 100, 100, str: 5, agi: 5, intel: 5, hp: 30), 0.01),

                // WARRIOR NECKLACES
                (new Equipment("Leather Necklace", EquipmentType.Necklace, 100, 20, hp: 10), 0.09),
                (new Equipment("Amulet of Strength", EquipmentType.Necklace, 100, 45, str: 4, hp: 15), 0.06),
                (new Equipment("Berserker's Torc", EquipmentType.Necklace, 100, 75, str: 7, stamina: 25), 0.02),
                (new Equipment("Guardian's Pendant", EquipmentType.Necklace, 100, 85, armor: 6, hp: 40), 0.02),
                (new Equipment("Paladin's Medallion", EquipmentType.Necklace, 100, 90, str: 6, intel: 7, mana: 25, hp: 30), 0.01),

                // MAGE NECKLACES
                (new Equipment("Silver Necklace", EquipmentType.Necklace, 100, 35, mana: 15), 0.08),
                (new Equipment("Amulet of Wisdom", EquipmentType.Necklace, 100, 50, intel: 5, mana: 20), 0.06),
                (new Equipment("Arcane Pendant", EquipmentType.Necklace, 100, 70, intel: 8, mana: 35), 0.04),
                (new Equipment("Archmage's Medallion", EquipmentType.Necklace, 100, 100, intel: 15, mana: 60), 0.01),
                (new Equipment("Necromancer's Amulet", EquipmentType.Necklace, 100, 95, intel: 12, hp: 30, mana: 45), 0.01),
                (new Equipment("Elemental Talisman", EquipmentType.Necklace, 100, 98, intel: 14, mana: 50), 0.01),

                // ROGUE NECKLACES
                (new Equipment("Amulet of Agility", EquipmentType.Necklace, 100, 42, agi: 4, stamina: 10), 0.06),
                (new Equipment("Shadow Pendant", EquipmentType.Necklace, 100, 58, agi: 7, stamina: 18), 0.04),
                (new Equipment("Assassin's Choker", EquipmentType.Necklace, 100, 85, agi: 12, stamina: 25), 0.02),
                (new Equipment("Ranger's Torc", EquipmentType.Necklace, 100, 80, agi: 10, hp: 25), 0.02),
                (new Equipment("Shadowblade's Amulet", EquipmentType.Necklace, 100, 88, agi: 13, intel: 5), 0.01),

                // PRIEST NECKLACES
                (new Equipment("Amulet of Protection", EquipmentType.Necklace, 100, 45, armor: 3, hp: 15), 0.06),
                (new Equipment("Holy Symbol Necklace", EquipmentType.Necklace, 100, 55, intel: 6, mana: 25, hp: 20), 0.05),
                (new Equipment("Divine Pendant", EquipmentType.Necklace, 100, 72, intel: 9, mana: 35, hp: 30), 0.03),
                (new Equipment("Templar's Chain", EquipmentType.Necklace, 100, 92, str: 7, intel: 10, armor: 4, hp: 35), 0.01),
                (new Equipment("Druid's Totem Necklace", EquipmentType.Necklace, 100, 88, intel: 10, hp: 45), 0.01),
                (new Equipment("Oracle's Vision", EquipmentType.Necklace, 100, 95, intel: 14, mana: 55), 0.01),

                // UNIVERSAL NECKLACES
                (new Equipment("Pendant of the Ancients", EquipmentType.Necklace, 100, 70, hp: 25, mana: 20), 0.03),
                (new Equipment("Champion's Medallion", EquipmentType.Necklace, 100, 110, str: 6, agi: 6, intel: 6, hp: 40), 0.01),

                // ACCESSORIES (Universal)
                (new Equipment("Copper Bracers", EquipmentType.Accessory, 100, 18, armor: 1), 0.10),
                (new Equipment("Iron Bracers", EquipmentType.Accessory, 100, 30, armor: 2, str: 2), 0.08),
                (new Equipment("Steel Bracers", EquipmentType.Accessory, 100, 45, armor: 3, str: 3), 0.05),
                (new Equipment("Gloves of Precision", EquipmentType.Accessory, 100, 42, agi: 4), 0.06),
                (new Equipment("Mage's Gloves", EquipmentType.Accessory, 100, 48, intel: 5, mana: 15), 0.05),
                (new Equipment("Gauntlets of Power", EquipmentType.Accessory, 100, 65, str: 6, armor: 4), 0.03),
                (new Equipment("Shadowweave Gloves", EquipmentType.Accessory, 100, 60, agi: 7, stamina: 15), 0.03),
                (new Equipment("Holy Gauntlets", EquipmentType.Accessory, 100, 70, intel: 6, armor: 3, hp: 20), 0.03),

                // CRAFTING & RARE MATERIALS
                (new GenericItem("Rare Gem", 75), 0.07),
                (new GenericItem("Ancient Coin", 50), 0.09),
                (new GenericItem("Magical Essence", 100), 0.04),
                (new GenericItem("Dragon Scale", 150), 0.02),
                (new GenericItem("Shadow Crystal", 85), 0.05),
                (new GenericItem("Holy Water", 40), 0.08),
                (new GenericItem("Demon Blood", 90), 0.04),
                (new GenericItem("Phoenix Feather", 120), 0.03),
                (new GenericItem("Unicorn Horn", 110), 0.03),
                (new GenericItem("Void Essence", 95), 0.04),
                (new GenericItem("Nature's Blessing", 70), 0.05),
                (new GenericItem("Arcane Dust", 60), 0.06),
                (new GenericItem("Soul Shard", 80), 0.05),
                (new GenericItem("Enchanted Leather", 55), 0.06),
                (new GenericItem("Mithril Ore", 130), 0.02),
                (new GenericItem("Adamantite Bar", 160), 0.01)
            };

            var mob = new Mob(
                template.Name,
                template.BaseHP,
                template.BaseStr,
                template.BaseAgi,
                template.BaseInt,
                template.GoldMin,
                template.GoldMax,
                lootTable,
                1,
                template.BaseAR
            );

            return mob.ScaleToLevel(level);
        }
    }
}
