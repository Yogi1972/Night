using System;
using System.Collections.Generic;
using System.Linq;

namespace Rpg_Dungeon
{
    internal class Mob
    {
        #region Properties

        public string Name { get; }
        public int Level { get; private set; }
        public int Health { get; }
        public int Strength { get; }
        public int Agility { get; }
        public int Intelligence { get; }
        public int ArmorRating { get; }
        public EnemyAI AI { get; private set; }

        private readonly int _goldMin;
        private readonly int _goldMax;
        private readonly List<(Item item, double chance)> _lootTable;

        #endregion

        #region Constructor

        public Mob(string name, int hp, int str, int agi, int intel, int goldMin, int goldMax, List<(Item, double)> lootTable, int level = 1, int armorRating = 0, EnemyAI? ai = null)
        {
            Name = name;
            Level = Math.Max(1, level);
            Health = hp;
            Strength = str;
            Agility = agi;
            Intelligence = intel;
            ArmorRating = Math.Max(0, armorRating);
            _goldMin = Math.Max(0, goldMin);
            _goldMax = Math.Max(_goldMin, goldMax);
            _lootTable = lootTable ?? new List<(Item, double)>();

            // Initialize AI - auto-generate if not provided
            AI = ai ?? EnemyAI.CreateForMob(this);
        }

        #endregion

        #region Scaling Methods

        public Mob ScaleToLevel(int targetLevel)
        {
            int lvl = Math.Max(1, targetLevel);

            int levelDiff = lvl - 1;
            int hp = Health + (levelDiff * 12);
            int str = Strength + (levelDiff * 2);
            int agi = Agility + (levelDiff * 2);
            int intel = Intelligence + (levelDiff * 2);
            int ar = ArmorRating + (levelDiff * 1);

            int goldMin = Math.Max(0, _goldMin + (levelDiff * 3));
            int goldMax = Math.Max(goldMin, _goldMax + (levelDiff * 7));

            var lootCopy = _lootTable.Select(t => (t.item, t.chance)).ToList();

            // Preserve AI behavior when scaling
            return new Mob(Name, hp, str, agi, intel, goldMin, goldMax, lootCopy, lvl, ar, AI);
        }

        public Mob ScaleForLevel(int dungeonLevel)
        {
            return ScaleToLevel(dungeonLevel);
        }

        #endregion

        #region Loot Generation

        public LootResult DropLoot(Random? rng = null)
        {
            rng ??= new Random();
            var res = new LootResult
            {
                Gold = rng.Next(_goldMin, _goldMax + 1)
            };

            double levelBonus = 1.0 + (Level - 1) * 0.02;

            foreach (var (item, chance) in _lootTable)
            {
                if (item == null) continue;
                if (chance <= 0) continue;

                double adjustedChance = Math.Min(0.95, chance * levelBonus);

                if (rng.NextDouble() <= adjustedChance)
                {
                    if (item is Backpack bp)
                    {
                        res.Items.Add(new Backpack(bp.Name, bp.Slots, bp.Price));
                    }
                    else if (item is Equipment eq)
                    {
                        var droppedEquip = new Equipment(eq.Name, eq.Type, eq.MaxDurability, eq.Price);
                        int damage = rng.Next(0, (int)(eq.MaxDurability * 0.2));
                        if (damage > 0) droppedEquip.Damage(damage);
                        res.Items.Add(droppedEquip);
                    }
                    else
                    {
                        res.Items.Add(new GenericItem(item.Name, item.Price));
                    }
                }
            }

            if (res.Items.Count == 0 && _lootTable.Count > 0)
            {
                var guaranteed = _lootTable.OrderByDescending(t => t.chance).FirstOrDefault();
                if (guaranteed.item != null)
                {
                    if (guaranteed.item is Backpack bp)
                    {
                        res.Items.Add(new Backpack(bp.Name, bp.Slots, bp.Price));
                    }
                    else if (guaranteed.item is Equipment eq)
                    {
                        var droppedEquip = new Equipment(eq.Name, eq.Type, eq.MaxDurability, eq.Price);
                        int damage = rng.Next(0, (int)(eq.MaxDurability * 0.2));
                        if (damage > 0) droppedEquip.Damage(damage);
                        res.Items.Add(droppedEquip);
                    }
                    else
                    {
                        res.Items.Add(new GenericItem(guaranteed.item.Name, guaranteed.item.Price));
                    }
                }
            }

            return res;
        }

        #endregion
    }
}
