using Night.Characters;
using Night.Combat;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rpg_Dungeon
{
    #region Combo Data Structures

    /// <summary>
    /// Represents a combo attack pattern between two abilities
    /// </summary>
    internal class ComboPattern
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string FirstAbility { get; set; } = string.Empty;
        public string SecondAbility { get; set; } = string.Empty;
        public double DamageMultiplier { get; set; } = 1.5; // Default 50% bonus
        public string SpecialEffect { get; set; } = string.Empty;
        public string Icon { get; set; } = "⚡";
        
        public ComboPattern(string name, string first, string second, double multiplier = 1.5, string effect = "", string icon = "⚡")
        {
            Name = name;
            FirstAbility = first;
            SecondAbility = second;
            DamageMultiplier = multiplier;
            SpecialEffect = effect;
            Icon = icon;
            Description = $"{first} → {second}";
        }
    }

    /// <summary>
    /// Tracks recent ability usage for combo detection
    /// </summary>
    internal class ComboTracker
    {
        public string AbilityName { get; set; } = string.Empty;
        public Character User { get; set; } = null!;
        public int TurnUsed { get; set; }
        
        public ComboTracker(string abilityName, Character user, int turn)
        {
            AbilityName = abilityName;
            User = user;
            TurnUsed = turn;
        }
    }

    #endregion

    /// <summary>
    /// Manages combo attacks between party members for enhanced damage and effects
    /// </summary>
    internal class ComboSystem
    {
        #region Fields

        private readonly List<ComboPattern> _comboPatterns;
        private readonly List<ComboTracker> _recentAbilities;
        private int _currentTurn;
        private int _comboCounter;
        private const int ComboWindowTurns = 2; // Abilities must be used within 2 turns

        #endregion

        #region Constructor

        public ComboSystem()
        {
            _comboPatterns = new List<ComboPattern>();
            _recentAbilities = new List<ComboTracker>();
            _currentTurn = 0;
            _comboCounter = 0;
            InitializeComboPatterns();
        }

        #endregion

        #region Initialization

        private void InitializeComboPatterns()
        {
            // Warrior + Rogue Combos
            _comboPatterns.Add(new ComboPattern(
                "Coordinated Strike",
                "Power Strike",
                "Backstab",
                1.6, // 60% bonus
                "Stunning Blow",
                "⚔️"
            ));

            _comboPatterns.Add(new ComboPattern(
                "Bleeding Fury",
                "Intimidating Shout",
                "Poison Blade",
                1.5,
                "Extended Bleed",
                "💀"
            ));

            // Mage + Warrior Combos
            _comboPatterns.Add(new ComboPattern(
                "Flame Strike",
                "Fireball",
                "Power Strike",
                1.7, // 70% bonus
                "Explosive Impact",
                "🔥⚔️"
            ));

            _comboPatterns.Add(new ComboPattern(
                "Frozen Shatter",
                "Ice Bolt",
                "Whirlwind Attack",
                1.8, // 80% bonus
                "Ice Explosion",
                "❄️💥"
            ));

            // Mage + Rogue Combos
            _comboPatterns.Add(new ComboPattern(
                "Lightning Assassination",
                "Lightning Storm",
                "Backstab",
                1.75,
                "Electrocute",
                "⚡🗡️"
            ));

            _comboPatterns.Add(new ComboPattern(
                "Burning Shadow",
                "Fireball",
                "Shadow Step",
                1.55,
                "Fire Trail",
                "🔥👤"
            ));

            // Priest + Warrior Combos
            _comboPatterns.Add(new ComboPattern(
                "Divine Retribution",
                "Holy Smite",
                "Power Strike",
                1.65,
                "Holy Fire",
                "✝️⚔️"
            ));

            _comboPatterns.Add(new ComboPattern(
                "Wrath Strike",
                "Wrath",
                "Whirlwind Attack",
                1.7,
                "Judgment",
                "⚡🌪️"
            ));

            // Priest + Rogue Combos
            _comboPatterns.Add(new ComboPattern(
                "Blessed Blade",
                "Divine Shield",
                "Poison Blade",
                1.5,
                "Purifying Strike",
                "🛡️☠️"
            ));

            // Priest + Mage Combos
            _comboPatterns.Add(new ComboPattern(
                "Arcane Judgment",
                "Wrath",
                "Fireball",
                1.75,
                "Holy Inferno",
                "✝️🔥"
            ));

            _comboPatterns.Add(new ComboPattern(
                "Frost Blessing",
                "Divine Shield",
                "Ice Bolt",
                1.6,
                "Frozen Sanctuary",
                "🛡️❄️"
            ));

            // Multi-Rogue Combos
            _comboPatterns.Add(new ComboPattern(
                "Dual Assassination",
                "Backstab",
                "Backstab",
                1.9, // 90% bonus!
                "Perfect Execution",
                "🗡️🗡️"
            ));

            // Multi-Mage Combos
            _comboPatterns.Add(new ComboPattern(
                "Elemental Chaos",
                "Fireball",
                "Ice Bolt",
                1.85,
                "Steam Explosion",
                "🔥❄️"
            ));

            _comboPatterns.Add(new ComboPattern(
                "Storm Cascade",
                "Lightning Storm",
                "Lightning Storm",
                2.0, // 100% bonus!
                "Thunderstorm",
                "⚡⚡"
            ));

            // Multi-Warrior Combos
            _comboPatterns.Add(new ComboPattern(
                "Shield Wall Assault",
                "Defensive Stance",
                "Whirlwind Attack",
                1.6,
                "Fortress Strike",
                "🛡️🌪️"
            ));
        }

        #endregion

        #region Combo Detection

        /// <summary>
        /// Records an ability usage and checks for combos
        /// </summary>
        public ComboPattern? CheckForCombo(string abilityName, Character user)
        {
            // Clean up old abilities outside combo window
            _recentAbilities.RemoveAll(a => _currentTurn - a.TurnUsed > ComboWindowTurns);

            // Check if this ability forms a combo with any recent ability
            foreach (var recent in _recentAbilities)
            {
                // Find matching combo pattern
                var combo = _comboPatterns.FirstOrDefault(c =>
                    (c.FirstAbility == recent.AbilityName && c.SecondAbility == abilityName) ||
                    (c.SecondAbility == recent.AbilityName && c.FirstAbility == abilityName)
                );

                if (combo != null)
                {
                    // Combo detected!
                    _comboCounter++;
                    
                    // Remove the triggering ability from recent list
                    _recentAbilities.Remove(recent);
                    
                    return combo;
                }
            }

            // No combo found, add this ability to recent list
            _recentAbilities.Add(new ComboTracker(abilityName, user, _currentTurn));
            
            return null;
        }

        /// <summary>
        /// Advances the turn counter
        /// </summary>
        public void AdvanceTurn()
        {
            _currentTurn++;
        }

        /// <summary>
        /// Resets combo tracking for new combat
        /// </summary>
        public void ResetCombat()
        {
            _recentAbilities.Clear();
            _currentTurn = 0;
        }

        #endregion

        #region Combo Application

        /// <summary>
        /// Applies combo bonus damage to an attack
        /// </summary>
        public int ApplyComboBonus(int baseDamage, ComboPattern combo)
        {
            return (int)(baseDamage * combo.DamageMultiplier);
        }

        /// <summary>
        /// Displays combo notification to player
        /// </summary>
        public void DisplayCombo(ComboPattern combo, Character user1, Character user2, int bonusDamage)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\n╔═══════════════════════════════════════════════════════════╗");
            Console.Write("║  ");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write($"{combo.Icon} COMBO ATTACK: {combo.Name}! {combo.Icon}");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("  ║");
            Console.WriteLine("╚═══════════════════════════════════════════════════════════╝");
            Console.ResetColor();

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"🎯 {user1.Name} and {user2.Name} coordinate their attacks!");
            Console.WriteLine($"📊 {combo.Description}");
            
            if (!string.IsNullOrEmpty(combo.SpecialEffect))
            {
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine($"✨ Special Effect: {combo.SpecialEffect}");
            }
            
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"💥 Bonus Damage: +{bonusDamage} ({combo.DamageMultiplier:P0} multiplier)");
            Console.ResetColor();
            
            Console.WriteLine($"🔥 Combo Chain: {_comboCounter}\n");
        }

        /// <summary>
        /// Displays available combos at start of combat
        /// </summary>
        public void DisplayAvailableCombos(List<Character> party)
        {
            if (party.Count < 2) return;

            var partyAbilities = party
                .Where(c => c.IsAlive && c.Abilities != null)
                .SelectMany(c => c.Abilities.Select(a => a.Name))
                .Distinct()
                .ToList();

            var possibleCombos = _comboPatterns
                .Where(c => partyAbilities.Contains(c.FirstAbility) && partyAbilities.Contains(c.SecondAbility))
                .ToList();

            if (possibleCombos.Count == 0) return;

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n╔════════════════════════════════════════════════════════╗");
            Console.WriteLine("║  ⚡ POSSIBLE COMBO ATTACKS IN THIS BATTLE ⚡         ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════╝");
            Console.ResetColor();

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("💡 TIP: Use these ability combinations for bonus damage!\n");
            Console.ResetColor();

            foreach (var combo in possibleCombos.Take(5)) // Show top 5
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write($"  {combo.Icon} ");
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write($"{combo.Name}");
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write($" (+{combo.DamageMultiplier:P0})");
                Console.ResetColor();
                Console.WriteLine();
                
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine($"     → {combo.Description}");
                Console.ResetColor();
            }

            if (possibleCombos.Count > 5)
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine($"\n     ... and {possibleCombos.Count - 5} more combos!");
                Console.ResetColor();
            }

            Console.WriteLine();
        }

        #endregion

        #region Stats

        /// <summary>
        /// Gets the current combo counter
        /// </summary>
        public int GetComboCounter() => _comboCounter;

        /// <summary>
        /// Gets total available combo patterns
        /// </summary>
        public int GetTotalCombos() => _comboPatterns.Count;

        #endregion
    }
}
