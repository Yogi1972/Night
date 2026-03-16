using Night.Characters;
using Night.Items;
using System;
using System.Collections.Generic;

namespace Rpg_Dungeon
{
    /// <summary>
    /// Manages legendary items - ultra-rare equipment with unique lore and powerful abilities
    /// </summary>
    internal static class LegendaryItemSystem
    {
        private static readonly Random _rng = new Random();

        #region Legendary Item Definitions

        private static readonly List<LegendaryItemTemplate> _legendaryItems = new List<LegendaryItemTemplate>
        {
            new LegendaryItemTemplate(
                "Dragonheart Blade",
                "A sword forged from dragon scales, still warm to the touch. Legend says it was quenched in dragon's blood.",
                EquipmentType.Weapon,
                25, // Level requirement
                75, 0, 0, 20, 5, 0, 0 // Health, Mana, Stamina, Strength, Agility, Intelligence, Armor
            ),
            new LegendaryItemTemplate(
                "Crown of the Archmage",
                "An ancient crown that pulses with raw magical energy. Worn by the first Archmage of the realm.",
                EquipmentType.Armor,
                30,
                50, 100, 0, 0, 5, 25, 5
            ),
            new LegendaryItemTemplate(
                "Shadow's Embrace",
                "Armor crafted from living shadow. Those who wear it can feel the darkness whispering secrets.",
                EquipmentType.Armor,
                35,
                100, 0, 50, 5, 20, 5, 15
            ),
            new LegendaryItemTemplate(
                "Gauntlets of Eternity",
                "Gloves that never wear, never tear, and grant the wearer incredible might. Said to be a gift from the gods.",
                EquipmentType.Armor,
                20,
                50, 25, 25, 15, 10, 10, 5
            ),
            new LegendaryItemTemplate(
                "Boots of the Windwalker",
                "These boots allow the wearer to move with impossible speed. Some say they can walk on air.",
                EquipmentType.Armor,
                25,
                75, 0, 50, 5, 25, 5, 5
            ),
            new LegendaryItemTemplate(
                "Amulet of the Phoenix",
                "A golden amulet containing a phoenix feather. The wearer can feel life energy coursing through them.",
                EquipmentType.Accessory,
                40,
                150, 75, 75, 10, 10, 15, 10
            ),
            new LegendaryItemTemplate(
                "Soulstealer",
                "A cursed blade that hungers for souls. Each life it takes makes it stronger.",
                EquipmentType.Weapon,
                45,
                0, 0, 0, 30, 15, 5, 0
            ),
            new LegendaryItemTemplate(
                "Aegis of the Titan",
                "A shield wielded by an ancient titan. Its protection is unmatched in all the realms.",
                EquipmentType.Armor,
                50,
                200, 50, 100, 10, 0, 5, 30
            ),
            new LegendaryItemTemplate(
                "Staff of the Cosmos",
                "A staff that channels the power of the stars themselves. Reality bends around it.",
                EquipmentType.Weapon,
                55,
                100, 150, 0, 5, 10, 35, 5
            ),
            new LegendaryItemTemplate(
                "Ring of Infinite Potential",
                "A simple ring that unlocks the wearer's true potential. It adapts to its bearer.",
                EquipmentType.Accessory,
                60,
                100, 100, 100, 15, 15, 15, 10
            )
        };

        #endregion

        #region Public Methods

        /// <summary>
        /// Attempt to generate a legendary item drop. Very rare!
        /// </summary>
        public static Equipment? TryGenerateLegendaryDrop(int characterLevel)
        {
            // Base chance: 0.5% (1 in 200)
            double baseChance = 0.005;

            // Increase chance slightly with level (max +0.5% at level 100)
            double levelBonus = (characterLevel / 100.0) * 0.005;
            double totalChance = baseChance + levelBonus;

            if (_rng.NextDouble() < totalChance)
            {
                // Filter legendaries by level requirement
                var eligible = _legendaryItems.FindAll(item => item.LevelRequirement <= characterLevel + 5);

                if (eligible.Count > 0)
                {
                    var template = eligible[_rng.Next(eligible.Count)];
                    return template.CreateEquipment();
                }
            }

            return null;
        }

        /// <summary>
        /// Get a guaranteed legendary item for milestone rewards
        /// </summary>
        public static Equipment GetLegendaryForLevel(int characterLevel)
        {
            var eligible = _legendaryItems.FindAll(item =>
                item.LevelRequirement <= characterLevel &&
                item.LevelRequirement >= characterLevel - 10);

            if (eligible.Count > 0)
            {
                var template = eligible[_rng.Next(eligible.Count)];
                return template.CreateEquipment();
            }

            // Fallback: return the lowest level legendary
            return _legendaryItems[0].CreateEquipment();
        }

        /// <summary>
        /// Display dramatic legendary item found message
        /// </summary>
        public static void AnnounceItemFound(Equipment item)
        {
            VisualEffects.ShowLegendaryItemFound();

            Console.WriteLine();
            VisualEffects.WriteLegendary("═══════════════════════════════════════════════════════════\n");
            VisualEffects.WriteLegendary($"   ✨ {item.Name} ✨\n");
            VisualEffects.WriteLegendary("═══════════════════════════════════════════════════════════\n");

            Console.WriteLine();
            VisualEffects.WriteLineColored("\"This item radiates immense power...\"", ConsoleColor.Cyan);
            Console.WriteLine();
        }

        #endregion

        #region Helper Classes

        private class LegendaryItemTemplate
        {
            public string Name { get; }
            public string Description { get; }
            public EquipmentType Type { get; }
            public int LevelRequirement { get; }
            public int Health { get; }
            public int Mana { get; }
            public int Stamina { get; }
            public int Strength { get; }
            public int Agility { get; }
            public int Intelligence { get; }
            public int Armor { get; }

            public LegendaryItemTemplate(string name, string description, EquipmentType type,
                int levelReq, int hp, int mana, int stamina, int str, int agi, int intel, int armor)
            {
                Name = name;
                Description = description;
                Type = type;
                LevelRequirement = levelReq;
                Health = hp;
                Mana = mana;
                Stamina = stamina;
                Strength = str;
                Agility = agi;
                Intelligence = intel;
                Armor = armor;
            }

            public Equipment CreateEquipment()
            {
                return new Equipment(
                    Name,
                    Type,
                    500, // Max durability for legendary items
                    LevelRequirement * 100, // Price
                    Strength,
                    Agility,
                    Intelligence,
                    Health,
                    Mana,
                    Stamina,
                    Armor
                );
            }
        }

        #endregion
    }
}
