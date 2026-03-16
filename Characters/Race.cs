using System;
using System.Collections.Generic;
using System.Text;

namespace Night.Characters
{
    #region Base Race Class

    internal abstract class Race
    {
        #region Properties

        public string Name { get; }

        public int HealthBonus { get; }
        public int ManaBonus { get; }
        public int StaminaBonus { get; }
        public int StrengthBonus { get; }
        public int AgilityBonus { get; }
        public int IntelligenceBonus { get; }
        public int ArmorBonus { get; }

        #endregion

        #region Constructor

        protected Race(string name, int healthBonus = 0, int manaBonus = 0, int staminaBonus = 0, int strengthBonus = 0, int agilityBonus = 0, int intelligenceBonus = 0, int armorBonus = 0)
        {
            Name = name;
            HealthBonus = healthBonus;
            ManaBonus = manaBonus;
            StaminaBonus = staminaBonus;
            StrengthBonus = strengthBonus;
            AgilityBonus = agilityBonus;
            IntelligenceBonus = intelligenceBonus;
            ArmorBonus = armorBonus;
        }

        #endregion

        #region Methods

        public override string ToString()
        {
            var parts = new List<string> { $"HP+{HealthBonus}" };
            if (ManaBonus != 0) parts.Add($"Mana+{ManaBonus}");
            if (StaminaBonus != 0) parts.Add($"Stamina+{StaminaBonus}");
            parts.Add($"Str+{StrengthBonus}");
            parts.Add($"Agi+{AgilityBonus}");
            parts.Add($"Int+{IntelligenceBonus}");
            parts.Add($"AR+{ArmorBonus}");
            return $"{Name} ({string.Join(", ", parts)})";
        }

        #endregion
    }

    #endregion
}
