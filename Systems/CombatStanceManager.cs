using Night.Characters;
using Night.Combat;
using System;

namespace Rpg_Dungeon
{
    /// <summary>
    /// Handles combat stance selection and management
    /// </summary>
    internal static class CombatStanceManager
    {
        /// <summary>
        /// Display stance selection menu and change character's stance
        /// </summary>
        public static void ChangeStanceMenu(Character character)
        {
            Console.WriteLine($"\n--- Combat Stance Selection for {character.Name} ---");
            Console.WriteLine($"Current: {CombatStanceModifiers.GetStanceDescription(character.CurrentStance)}");
            Console.WriteLine("\nAvailable Stances:");
            Console.WriteLine("1) ⚖️  BALANCED - No modifiers (default)");
            Console.WriteLine("2) ⚔️  AGGRESSIVE - +30% damage, -20% armor (glass cannon)");
            Console.WriteLine("3) 🛡️  DEFENSIVE - -20% damage, +40% armor (tank mode)");
            Console.WriteLine("0) Cancel");
            Console.Write("\nSelect stance: ");

            var choice = Console.ReadLine() ?? string.Empty;

            switch (choice.Trim())
            {
                case "1":
                    character.ChangeStance(CombatStance.Balanced);
                    break;
                case "2":
                    character.ChangeStance(CombatStance.Aggressive);
                    break;
                case "3":
                    character.ChangeStance(CombatStance.Defensive);
                    break;
                case "0":
                    Console.WriteLine("Stance unchanged.");
                    break;
                default:
                    Console.WriteLine("Invalid choice. Stance unchanged.");
                    break;
            }
        }
    }
}
