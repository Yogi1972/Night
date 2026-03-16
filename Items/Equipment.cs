using System;

namespace Rpg_Dungeon
{
    internal class Equipment : Item
    {
        #region Properties

        public int Durability { get; private set; }
        public int MaxDurability { get; }
        public EquipmentType Type { get; }

        public int StrengthBonus { get; }
        public int AgilityBonus { get; }
        public int IntelligenceBonus { get; }
        public int MaxHPBonus { get; }
        public int MaxManaBonus { get; }
        public int MaxStaminaBonus { get; }
        public int ArmorBonus { get; }

        public bool IsBroken => Durability <= 0;

        #endregion

        #region Constructor

        public Equipment(string name, EquipmentType type, int maxDurability, int price = 50,
                        int str = 0, int agi = 0, int intel = 0, int hp = 0, int mana = 0, int stamina = 0, int armor = 0) : base(name, price)
        {
            MaxDurability = Math.Max(1, maxDurability);
            Durability = MaxDurability;
            Type = type;
            StrengthBonus = str;
            AgilityBonus = agi;
            IntelligenceBonus = intel;
            MaxHPBonus = hp;
            MaxManaBonus = mana;
            MaxStaminaBonus = stamina;
            ArmorBonus = armor;
        }

        #endregion

        #region Methods

        public void Damage(int amount)
        {
            if (amount <= 0) return;
            Durability = Math.Max(0, Durability - amount);
        }

        public int RepairCost()
        {
            var missing = MaxDurability - Durability;
            if (missing <= 0) return 0;
            int costPerPoint = Math.Max(1, base.Price / 10);
            return missing * costPerPoint;
        }

        public void Repair()
        {
            Durability = MaxDurability;
        }

        #endregion
    }
}
