using System;

namespace Rpg_Dungeon
{
    /// <summary>
    /// Combat stance affects damage output and damage taken
    /// </summary>
    internal enum CombatStance
    {
        Balanced,    // Default: No bonuses or penalties
        Aggressive,  // +30% damage dealt, -20% armor
        Defensive    // -20% damage dealt, +40% armor
    }

    /// <summary>
    /// Helper class for combat stance modifiers
    /// </summary>
    internal static class CombatStanceModifiers
    {
        public static double GetDamageMultiplier(CombatStance stance)
        {
            return stance switch
            {
                CombatStance.Aggressive => 1.30,  // +30% damage
                CombatStance.Defensive => 0.80,   // -20% damage
                CombatStance.Balanced => 1.0,     // Normal damage
                _ => 1.0
            };
        }

        public static double GetArmorMultiplier(CombatStance stance)
        {
            return stance switch
            {
                CombatStance.Aggressive => 0.80,  // -20% armor
                CombatStance.Defensive => 1.40,   // +40% armor
                CombatStance.Balanced => 1.0,     // Normal armor
                _ => 1.0
            };
        }

        public static string GetStanceDescription(CombatStance stance)
        {
            return stance switch
            {
                CombatStance.Aggressive => "⚔️  AGGRESSIVE (+30% DMG, -20% Armor)",
                CombatStance.Defensive => "🛡️  DEFENSIVE (-20% DMG, +40% Armor)",
                CombatStance.Balanced => "⚖️  BALANCED (No modifiers)",
                _ => "UNKNOWN"
            };
        }

        public static string GetStanceIcon(CombatStance stance)
        {
            return stance switch
            {
                CombatStance.Aggressive => "⚔️",
                CombatStance.Defensive => "🛡️",
                CombatStance.Balanced => "⚖️",
                _ => "?"
            };
        }

        public static ConsoleColor GetStanceColor(CombatStance stance)
        {
            return stance switch
            {
                CombatStance.Aggressive => ConsoleColor.Red,
                CombatStance.Defensive => ConsoleColor.Blue,
                CombatStance.Balanced => ConsoleColor.White,
                _ => ConsoleColor.Gray
            };
        }
    }
}
