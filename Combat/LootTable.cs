using System;
using System.Collections.Generic;
using System.Linq;

namespace Rpg_Dungeon
{
    #region Loot Rarity Enum

    internal enum LootRarity
    {
        Common,     // 60% drop chance
        Uncommon,   // 25% drop chance
        Rare,       // 12% drop chance
        Epic,       // 3% drop chance
        Legendary   // <1% drop chance (boss only)
    }

    #endregion

    #region Loot Entry Class

    internal class LootEntry
    {
        public string ItemName { get; }
        public LootRarity Rarity { get; }
        public Func<Item> ItemFactory { get; }

        public LootEntry(string itemName, LootRarity rarity, Func<Item> itemFactory)
        {
            ItemName = itemName;
            Rarity = rarity;
            ItemFactory = itemFactory;
        }
    }

    #endregion

    #region Camp Loot Table Class

    internal class CampLootTable
    {
        private readonly Random _rng;
        private readonly List<LootEntry> _lootPool;
        private readonly EnemyCampType _campType;
        private readonly EnemyCampDifficulty _difficulty;

        public CampLootTable(EnemyCampType campType, EnemyCampDifficulty difficulty, Random rng)
        {
            _campType = campType;
            _difficulty = difficulty;
            _rng = rng;
            _lootPool = GenerateLootPool();
        }

        #region Loot Pool Generation

        private List<LootEntry> GenerateLootPool()
        {
            var pool = new List<LootEntry>();

            // Add camp-specific items
            pool.AddRange(GetCampSpecificLoot());

            // Add generic valuable items
            pool.AddRange(GetGenericLoot());

            // Add difficulty-scaled equipment
            pool.AddRange(GetEquipmentLoot());

            return pool;
        }

        private List<LootEntry> GetCampSpecificLoot()
        {
            return _campType switch
            {
                EnemyCampType.BanditHideout => new List<LootEntry>
                {
                    new("Stolen Jewelry", LootRarity.Common, () => new GenericItem("Stolen Jewelry", 30)),
                    new("Lockpick Set", LootRarity.Common, () => new GenericItem("Lockpick Set", 25)),
                    new("Bandit's Dagger", LootRarity.Uncommon, () => CreateWeapon("Bandit's Dagger", 2, 1)),
                    new("Concealed Armor", LootRarity.Uncommon, () => CreateArmor("Concealed Armor", 2, 0, 1)),
                    new("Master Thief's Cloak", LootRarity.Rare, () => CreateArmor("Master Thief's Cloak", 3, 0, 3)),
                    new("Bandit Lord's Blade", LootRarity.Epic, () => CreateWeapon("Bandit Lord's Blade", 5, 3)),
                },

                EnemyCampType.GoblinWarcamp => new List<LootEntry>
                {
                    new("Goblin Scrap Metal", LootRarity.Common, () => new GenericItem("Goblin Scrap Metal", 10)),
                    new("Crude Spear", LootRarity.Common, () => CreateWeapon("Crude Spear", 1, 0)),
                    new("Goblin War Paint", LootRarity.Uncommon, () => new GenericItem("Goblin War Paint", 40)),
                    new("Tribal Shield", LootRarity.Uncommon, () => CreateArmor("Tribal Shield", 2, 0, 0)),
                    new("Warlord's Trophy", LootRarity.Rare, () => new GenericItem("Warlord's Trophy", 150)),
                    new("Goblin King's Crown", LootRarity.Epic, () => CreateArmor("Goblin King's Crown", 4, 1, 1)),
                },

                EnemyCampType.OrcStronghold => new List<LootEntry>
                {
                    new("Orcish Iron", LootRarity.Common, () => new GenericItem("Orcish Iron", 35)),
                    new("Battle Axe", LootRarity.Common, () => CreateWeapon("Battle Axe", 2, 0)),
                    new("Orcish Warhammer", LootRarity.Uncommon, () => CreateWeapon("Orcish Warhammer", 3, 0)),
                    new("Spiked Armor", LootRarity.Uncommon, () => CreateArmor("Spiked Armor", 3, 0, 0)),
                    new("Chieftain's War Banner", LootRarity.Rare, () => new GenericItem("Chieftain's War Banner", 200)),
                    new("Orcish Doom Blade", LootRarity.Epic, () => CreateWeapon("Orcish Doom Blade", 7, 0)),
                },

                EnemyCampType.UndeadGraveyard => new List<LootEntry>
                {
                    new("Ancient Bone", LootRarity.Common, () => new GenericItem("Ancient Bone", 15)),
                    new("Grave Dust", LootRarity.Common, () => new GenericItem("Grave Dust", 25)),
                    new("Spectral Essence", LootRarity.Uncommon, () => new GenericItem("Spectral Essence", 60)),
                    new("Cursed Amulet", LootRarity.Uncommon, () => new GenericItem("Cursed Amulet", 75)),
                    new("Necromancer's Staff", LootRarity.Rare, () => CreateWeapon("Necromancer's Staff", 3, 4)),
                    new("Lich's Phylactery Fragment", LootRarity.Epic, () => new GenericItem("Lich's Phylactery Fragment", 500)),
                },

                EnemyCampType.BeastDen => new List<LootEntry>
                {
                    new("Beast Pelt", LootRarity.Common, () => new GenericItem("Beast Pelt", 20)),
                    new("Sharp Claw", LootRarity.Common, () => new GenericItem("Sharp Claw", 18)),
                    new("Beast Fang", LootRarity.Uncommon, () => new GenericItem("Beast Fang", 45)),
                    new("Primal Hide Armor", LootRarity.Uncommon, () => CreateArmor("Primal Hide Armor", 2, 1, 0)),
                    new("Alpha's Trophy", LootRarity.Rare, () => new GenericItem("Alpha's Trophy", 180)),
                    new("Savage Fang Necklace", LootRarity.Epic, () => CreateArmor("Savage Fang Necklace", 4, 2, 0)),
                },

                EnemyCampType.CultistShrine => new List<LootEntry>
                {
                    new("Dark Candle", LootRarity.Common, () => new GenericItem("Dark Candle", 12)),
                    new("Ritual Dagger", LootRarity.Common, () => CreateWeapon("Ritual Dagger", 1, 1)),
                    new("Forbidden Tome", LootRarity.Uncommon, () => new GenericItem("Forbidden Tome", 80)),
                    new("Cultist Robe", LootRarity.Uncommon, () => CreateArmor("Cultist Robe", 1, 0, 2)),
                    new("Dark Grimoire", LootRarity.Rare, () => new GenericItem("Dark Grimoire", 250)),
                    new("High Priest's Staff of Shadows", LootRarity.Epic, () => CreateWeapon("High Priest's Staff of Shadows", 4, 6)),
                },

                EnemyCampType.DragonLair => new List<LootEntry>
                {
                    new("Dragon Scale", LootRarity.Common, () => new GenericItem("Dragon Scale", 100)),
                    new("Dragon Tooth", LootRarity.Uncommon, () => new GenericItem("Dragon Tooth", 150)),
                    new("Dragon Claw", LootRarity.Uncommon, () => new GenericItem("Dragon Claw", 140)),
                    new("Dragonhide Armor", LootRarity.Rare, () => CreateArmor("Dragonhide Armor", 5, 0, 0)),
                    new("Dragon Heart", LootRarity.Rare, () => new GenericItem("Dragon Heart", 400)),
                    new("Dragonforged Sword", LootRarity.Epic, () => CreateWeapon("Dragonforged Sword", 8, 2)),
                },

                EnemyCampType.DemonPortal => new List<LootEntry>
                {
                    new("Demonic Ash", LootRarity.Common, () => new GenericItem("Demonic Ash", 30)),
                    new("Sulfur Crystal", LootRarity.Common, () => new GenericItem("Sulfur Crystal", 35)),
                    new("Hellfire Shard", LootRarity.Uncommon, () => new GenericItem("Hellfire Shard", 90)),
                    new("Demon Horn", LootRarity.Uncommon, () => new GenericItem("Demon Horn", 85)),
                    new("Infernal Blade", LootRarity.Rare, () => CreateWeapon("Infernal Blade", 5, 3)),
                    new("Demon Commander's Insignia", LootRarity.Epic, () => new GenericItem("Demon Commander's Insignia", 600)),
                },

                EnemyCampType.ElementalNexus => new List<LootEntry>
                {
                    new("Elemental Crystal", LootRarity.Common, () => new GenericItem("Elemental Crystal", 40)),
                    new("Mana Infused Stone", LootRarity.Common, () => new GenericItem("Mana Infused Stone", 45)),
                    new("Essence of Fire", LootRarity.Uncommon, () => new GenericItem("Essence of Fire", 70)),
                    new("Essence of Ice", LootRarity.Uncommon, () => new GenericItem("Essence of Ice", 70)),
                    new("Elemental Staff", LootRarity.Rare, () => CreateWeapon("Elemental Staff", 2, 6)),
                    new("Nexus Core", LootRarity.Epic, () => new GenericItem("Nexus Core", 550)),
                },

                EnemyCampType.SpiderNest => new List<LootEntry>
                {
                    new("Spider Silk", LootRarity.Common, () => new GenericItem("Spider Silk", 22)),
                    new("Poison Gland", LootRarity.Common, () => new GenericItem("Poison Gland", 28)),
                    new("Chitin Plate", LootRarity.Uncommon, () => new GenericItem("Chitin Plate", 50)),
                    new("Webweave Armor", LootRarity.Uncommon, () => CreateArmor("Webweave Armor", 2, 2, 0)),
                    new("Venom Fang Dagger", LootRarity.Rare, () => CreateWeapon("Venom Fang Dagger", 4, 1)),
                    new("Brood Mother's Carapace", LootRarity.Epic, () => CreateArmor("Brood Mother's Carapace", 6, 3, 0)),
                },

                _ => GetGenericLoot()
            };
        }

        private List<LootEntry> GetGenericLoot()
        {
            return new List<LootEntry>
            {
                // Consumables (Common)
                new("Health Potion", LootRarity.Common, () => new GenericItem("Health Potion", 20)),
                new("Mana Potion", LootRarity.Common, () => new GenericItem("Mana Potion", 25)),
                new("Bandages", LootRarity.Common, () => new GenericItem("Bandages", 10)),
                new("Antidote", LootRarity.Common, () => new GenericItem("Antidote", 15)),
                new("Rations", LootRarity.Common, () => new GenericItem("Rations", 8)),

                // Utility items (Common/Uncommon)
                new("Rope", LootRarity.Common, () => new GenericItem("Rope", 8)),
                new("Torch", LootRarity.Common, () => new GenericItem("Torch", 5)),
                new("Herbal Poultice", LootRarity.Uncommon, () => new GenericItem("Herbal Poultice", 35)),
                new("Greater Health Potion", LootRarity.Uncommon, () => new GenericItem("Greater Health Potion", 50)),
                new("Greater Mana Potion", LootRarity.Uncommon, () => new GenericItem("Greater Mana Potion", 55)),

                // Valuable materials (Uncommon/Rare)
                new("Gemstone", LootRarity.Uncommon, () => new GenericItem("Gemstone", 100)),
                new("Rare Herb", LootRarity.Rare, () => new GenericItem("Rare Herb", 120)),
                new("Enchanted Crystal", LootRarity.Rare, () => new GenericItem("Enchanted Crystal", 200)),
            };
        }

        private List<LootEntry> GetEquipmentLoot()
        {
            int difficultyBonus = _difficulty switch
            {
                EnemyCampDifficulty.Weak => 0,
                EnemyCampDifficulty.Normal => 1,
                EnemyCampDifficulty.Strong => 2,
                EnemyCampDifficulty.Dangerous => 3,
                EnemyCampDifficulty.Deadly => 4,
                EnemyCampDifficulty.Elite => 5,
                _ => 0
            };

            return new List<LootEntry>
            {
                new("Iron Sword", LootRarity.Common, () => CreateWeapon("Iron Sword", 2 + difficultyBonus, 0)),
                new("Steel Armor", LootRarity.Uncommon, () => CreateArmor("Steel Armor", 3 + difficultyBonus, 0, 0)),
                new("Magic Ring", LootRarity.Rare, () => new GenericItem("Magic Ring", 100 + (difficultyBonus * 50))),
                new("Enchanted Weapon", LootRarity.Epic, () => CreateWeapon("Enchanted Weapon", 5 + difficultyBonus, 2 + difficultyBonus)),
            };
        }

        #endregion

        #region Loot Generation

        public List<Item> GenerateLoot(bool isBossLoot = false, int bonusRolls = 0)
        {
            var loot = new List<Item>();

            if (isBossLoot)
            {
                // Boss always drops 2-4 items with better rarity chances
                int bossItemCount = _rng.Next(2, 5);
                for (int i = 0; i < bossItemCount; i++)
                {
                    var item = RollForItem(isBossLoot: true);
                    if (item != null)
                    {
                        loot.Add(item);
                    }
                }
            }
            else
            {
                // Regular loot: 1-2 items + bonus rolls
                int baseRolls = _rng.Next(1, 3);
                int totalRolls = baseRolls + bonusRolls;

                for (int i = 0; i < totalRolls; i++)
                {
                    var item = RollForItem();
                    if (item != null)
                    {
                        loot.Add(item);
                    }
                }
            }

            // Add gold as "item" (displayed separately)
            int goldAmount = CalculateGoldDrop(isBossLoot);
            if (goldAmount > 0)
            {
                // We'll handle gold separately, but track it here
            }

            return loot.Where(i => i != null).Distinct().ToList();
        }

        private Item? RollForItem(bool isBossLoot = false)
        {
            var rarity = RollRarity(isBossLoot);
            var eligibleItems = _lootPool.Where(entry => entry.Rarity == rarity).ToList();

            if (eligibleItems.Count == 0)
            {
                // Fallback to any rarity if none found
                eligibleItems = _lootPool;
            }

            if (eligibleItems.Count == 0)
            {
                return null;
            }

            var selectedEntry = eligibleItems[_rng.Next(eligibleItems.Count)];
            return selectedEntry.ItemFactory();
        }

        private LootRarity RollRarity(bool isBossLoot = false)
        {
            int roll = _rng.Next(1, 101);

            if (isBossLoot)
            {
                // Boss loot has better chances
                if (roll <= 5) return LootRarity.Legendary;
                if (roll <= 20) return LootRarity.Epic;
                if (roll <= 45) return LootRarity.Rare;
                if (roll <= 75) return LootRarity.Uncommon;
                return LootRarity.Common;
            }
            else
            {
                // Regular loot
                if (roll <= 3) return LootRarity.Epic;
                if (roll <= 15) return LootRarity.Rare;
                if (roll <= 40) return LootRarity.Uncommon;
                return LootRarity.Common;
            }
        }

        private int CalculateGoldDrop(bool isBossLoot)
        {
            int baseGold = _difficulty switch
            {
                EnemyCampDifficulty.Weak => _rng.Next(10, 30),
                EnemyCampDifficulty.Normal => _rng.Next(20, 50),
                EnemyCampDifficulty.Strong => _rng.Next(40, 80),
                EnemyCampDifficulty.Dangerous => _rng.Next(70, 120),
                EnemyCampDifficulty.Deadly => _rng.Next(100, 180),
                EnemyCampDifficulty.Elite => _rng.Next(150, 250),
                _ => _rng.Next(20, 50)
            };

            if (isBossLoot)
            {
                baseGold = (int)(baseGold * 2.5); // Boss drops significantly more gold
            }

            return baseGold;
        }

        #endregion

        #region Helper Methods

        private Equipment CreateWeapon(string name, int strBonus, int intBonus)
        {
            int durability = 80 + (_rng.Next(-10, 11));
            int price = (strBonus + intBonus) * 50;
            return new Equipment(name, EquipmentType.Weapon, durability, price,
                str: strBonus, intel: intBonus);
        }

        private Equipment CreateArmor(string name, int armorBonus, int strBonus, int agiBonus)
        {
            int durability = 100 + (_rng.Next(-15, 16));
            int price = (armorBonus + strBonus + agiBonus) * 40;
            return new Equipment(name, EquipmentType.Armor, durability, price,
                str: strBonus, agi: agiBonus, armor: armorBonus);
        }

        public static string GetRarityColor(LootRarity rarity)
        {
            return rarity switch
            {
                LootRarity.Common => "⚪",
                LootRarity.Uncommon => "🟢",
                LootRarity.Rare => "🔵",
                LootRarity.Epic => "🟣",
                LootRarity.Legendary => "🟠",
                _ => "⚪"
            };
        }

        public static string GetRarityName(LootRarity rarity)
        {
            return rarity switch
            {
                LootRarity.Common => "Common",
                LootRarity.Uncommon => "Uncommon",
                LootRarity.Rare => "Rare",
                LootRarity.Epic => "Epic",
                LootRarity.Legendary => "Legendary",
                _ => "Unknown"
            };
        }

        public string GetLootThemeDescription()
        {
            return _campType switch
            {
                EnemyCampType.BanditHideout => "Rogue gear, stolen valuables, concealed weapons",
                EnemyCampType.GoblinWarcamp => "Crude weapons, tribal items, scrap materials",
                EnemyCampType.OrcStronghold => "Heavy weapons, battle gear, war trophies",
                EnemyCampType.UndeadGraveyard => "Dark artifacts, necromantic items, cursed relics",
                EnemyCampType.BeastDen => "Pelts, claws, primal armor, hunting trophies",
                EnemyCampType.CultistShrine => "Ritual items, dark magic gear, forbidden knowledge",
                EnemyCampType.DragonLair => "Dragon parts, dragonforged items, ancient treasures",
                EnemyCampType.DemonPortal => "Infernal weapons, demonic materials, hellish artifacts",
                EnemyCampType.ElementalNexus => "Elemental essences, magic crystals, arcane items",
                EnemyCampType.SpiderNest => "Spider silk, poison items, chitin armor",
                _ => "Various items and equipment"
            };
        }

        #endregion
    }

    #endregion
}
