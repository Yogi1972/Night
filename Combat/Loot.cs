using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rpg_Dungeon
{
    #region Loot Static Class

    internal static class Loot
    {
        #region Fields

        private static readonly Random _rng = new Random();

        #endregion

        #region Item Pools

        #region Warrior Items

        public static readonly IReadOnlyList<GenericItem> WarriorItems = new List<GenericItem>
        {
            new GenericItem("Short Sword"),
            new GenericItem("Long Sword"),
            new GenericItem("Broad Axe"),
            new GenericItem("Warhammer"),
            new GenericItem("Kite Shield"),
            new GenericItem("Chainmail"),
            new GenericItem("Scale Armor"),
            new GenericItem("Gauntlets"),
            new GenericItem("Boots of Stability"),
            new GenericItem("Helmet"),
            new GenericItem("Spear"),
            new GenericItem("Battle Banner"),
            new GenericItem("Heavy Crossbow"),
            new GenericItem("Rations"),
            new GenericItem("Adventurer's Belt"),
            new GenericItem("Sharpening Stone"),
            new GenericItem("Oil Flask"),
            new GenericItem("Iron Ingot"),
            new GenericItem("Soldier's Medallion"),
            new GenericItem("Tough Hide"),
            new GenericItem("Reinforced Bracers"),
            new GenericItem("Throwing Axe")
        };

        #endregion

        #region Mage Items

        public static readonly IReadOnlyList<GenericItem> MageItems = new List<GenericItem>
        {
            new GenericItem("Apprentice Staff"),
            new GenericItem("Wizard's Robe"),
            new GenericItem("Spellbook: Firebolt"),
            new GenericItem("Spellbook: Frost"),
            new GenericItem("Mana Crystal"),
            new GenericItem("Arcane Dust"),
            new GenericItem("Enchanted Thread"),
            new GenericItem("Rune Stone"),
            new GenericItem("Sage's Circlet"),
            new GenericItem("Potion of Mana"),
            new GenericItem("Scroll of Teleport"),
            new GenericItem("Earthen Globe"),
            new GenericItem("Crystal Shard"),
            new GenericItem("Alchemist's Kit"),
            new GenericItem("Feather Quill"),
            new GenericItem("Strange Herb"),
            new GenericItem("Lesser Talisman"),
            new GenericItem("Glyph Tablet"),
            new GenericItem("Mystic Ink"),
            new GenericItem("Elemental Core"),
            new GenericItem("Wand Fragment")
        };

        #endregion

        #region Rogue Items

        public static readonly IReadOnlyList<GenericItem> RogueItems = new List<GenericItem>
        {
            new GenericItem("Dagger"),
            new GenericItem("Shortbow"),
            new GenericItem("Lockpick Set"),
            new GenericItem("Cloak of Shadows"),
            new GenericItem("Thieves' Gloves"),
            new GenericItem("Poison Vial"),
            new GenericItem("Smoke Bomb"),
            new GenericItem("Boots of Silence"),
            new GenericItem("Grappling Hook"),
            new GenericItem("Trap Kit"),
            new GenericItem("Coin Purse"),
            new GenericItem("Forged Letter"),
            new GenericItem("Lockbox"),
            new GenericItem("Hidden Blade"),
            new GenericItem("Disguise Kit"),
            new GenericItem("Silk Rope"),
            new GenericItem("Vial of Acid"),
            new GenericItem("Spyglass"),
            new GenericItem("Thin Wire"),
            new GenericItem("Camouflage Paint"),
            new GenericItem("Thief's Token")
        };

        #endregion

        #region Priest Items

        public static readonly IReadOnlyList<GenericItem> PriestItems = new List<GenericItem>
        {
            new GenericItem("Priest's Staff"),
            new GenericItem("Cleric Robe"),
            new GenericItem("Potion of Healing"),
            new GenericItem("Herbal Poultice"),
            new GenericItem("Holy Symbol"),
            new GenericItem("Bandage Roll"),
            new GenericItem("Antidote"),
            new GenericItem("Sacred Oil"),
            new GenericItem("Prayer Beads"),
            new GenericItem("Tonic"),
            new GenericItem("Restorative Salve"),
            new GenericItem("Blessed Water"),
            new GenericItem("Light Crossbow"),
            new GenericItem("Priest's Satchel"),
            new GenericItem("Healing Herb"),
            new GenericItem("Medal of Mercy"),
            new GenericItem("Purifying Crystal"),
            new GenericItem("Comforting Tincture"),
            new GenericItem("Cleansing Cloth"),
            new GenericItem("Soothing Balm"),
            new GenericItem("Sacred Tome")
        };

        #endregion

        #endregion

        #region Methods

        public static List<Item> GetRandomItemsForRole(string role, int count, Random? rng = null)
        {
            rng ??= _rng;
            var pool = role?.Trim().ToLowerInvariant() switch
            {
                "warrior" => WarriorItems,
                "mage" => MageItems,
                "rogue" => RogueItems,
                "priest" => PriestItems,
                _ => WarriorItems
            };

            var picks = new List<Item>();
            var indices = Enumerable.Range(0, pool.Count).OrderBy(_ => rng.Next()).Take(Math.Min(count, pool.Count));
            foreach (var i in indices) picks.Add(new GenericItem(pool[i].Name));
            return picks;
        }

        // Convenience: get a single random item from any role pool (balanced across roles)
        public static Item GetRandomItem(Random? rng = null)
        {
            rng ??= _rng;
            var all = WarriorItems.Concat(MageItems).Concat(RogueItems).Concat(PriestItems).ToList();
            var item = all[rng.Next(all.Count)];
            return new GenericItem(item.Name);
        }

        #endregion
    }

    #endregion
}
