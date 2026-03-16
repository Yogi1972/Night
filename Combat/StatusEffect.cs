using Night.Characters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rpg_Dungeon
{
    /// <summary>
    /// Types of status effects that can be applied in combat
    /// </summary>
    internal enum StatusEffectType
    {
        Bleeding,    // Damage over time
        Stunned,     // Skip next turn
        Poisoned,    // Damage over time, stronger than bleed
        Burning,     // Damage over time, can spread
        Frozen,      // Reduced action options
        Weakened,    // Reduced damage output
        Vulnerable,  // Increased damage taken
        Regenerating, // Healing over time
        Strengthened // Increased damage output
    }

    /// <summary>
    /// A status effect applied to a character or mob during combat
    /// </summary>
    internal class StatusEffect
    {
        public StatusEffectType Type { get; }
        public int Duration { get; set; }
        public int Potency { get; }
        public string Source { get; }

        public StatusEffect(StatusEffectType type, int duration, int potency, string source)
        {
            Type = type;
            Duration = duration;
            Potency = potency;
            Source = source;
        }

        public string GetIcon()
        {
            return Type switch
            {
                StatusEffectType.Bleeding => "🩸",
                StatusEffectType.Stunned => "💫",
                StatusEffectType.Poisoned => "☠️",
                StatusEffectType.Burning => "🔥",
                StatusEffectType.Frozen => "❄️",
                StatusEffectType.Weakened => "⬇️",
                StatusEffectType.Vulnerable => "🎯",
                StatusEffectType.Regenerating => "💚",
                StatusEffectType.Strengthened => "⬆️",
                _ => "?"
            };
        }

        public ConsoleColor GetColor()
        {
            return Type switch
            {
                StatusEffectType.Bleeding => ConsoleColor.DarkRed,
                StatusEffectType.Stunned => ConsoleColor.Yellow,
                StatusEffectType.Poisoned => ConsoleColor.Green,
                StatusEffectType.Burning => ConsoleColor.Red,
                StatusEffectType.Frozen => ConsoleColor.Cyan,
                StatusEffectType.Weakened => ConsoleColor.DarkGray,
                StatusEffectType.Vulnerable => ConsoleColor.Magenta,
                StatusEffectType.Regenerating => ConsoleColor.Green,
                StatusEffectType.Strengthened => ConsoleColor.Yellow,
                _ => ConsoleColor.White
            };
        }

        public void ApplyEffect(Character target)
        {
            Console.ForegroundColor = GetColor();
            Console.Write($" {GetIcon()}");

            switch (Type)
            {
                case StatusEffectType.Bleeding:
                    target.ReceiveDamage(Potency);
                    Console.Write($" {target.Name} bleeds for {Potency} damage!");
                    break;

                case StatusEffectType.Poisoned:
                    target.ReceiveDamage(Potency);
                    Console.Write($" {target.Name} takes {Potency} poison damage!");
                    break;

                case StatusEffectType.Burning:
                    target.ReceiveDamage(Potency);
                    Console.Write($" {target.Name} burns for {Potency} damage!");
                    break;

                case StatusEffectType.Regenerating:
                    target.Heal(Potency);
                    Console.Write($" {target.Name} regenerates {Potency} HP!");
                    break;

                case StatusEffectType.Stunned:
                    Console.Write($" {target.Name} is stunned and cannot act!");
                    break;

                case StatusEffectType.Frozen:
                    Console.Write($" {target.Name} is frozen!");
                    break;

                case StatusEffectType.Weakened:
                    Console.Write($" {target.Name} is weakened! (-30% damage)");
                    break;

                case StatusEffectType.Vulnerable:
                    Console.Write($" {target.Name} is vulnerable! (+30% damage taken)");
                    break;

                case StatusEffectType.Strengthened:
                    Console.Write($" {target.Name} is empowered! (+30% damage)");
                    break;
            }

            Console.ResetColor();
            Duration--;
        }

        public bool IsExpired => Duration <= 0;

        public string GetDescription()
        {
            return $"{GetIcon()} {Type} ({Duration} turns, potency {Potency})";
        }
    }

    /// <summary>
    /// Manages status effects for characters and mobs
    /// </summary>
    internal static class StatusEffectManager
    {
        private static readonly Dictionary<string, List<StatusEffect>> _characterEffects = new();
        private static readonly Dictionary<string, List<StatusEffect>> _mobEffects = new();

        public static void AddEffect(Character target, StatusEffect effect)
        {
            string key = target.Name;
            if (!_characterEffects.ContainsKey(key))
            {
                _characterEffects[key] = new List<StatusEffect>();
            }

            // Check if same effect type already exists - refresh duration if stronger
            var existing = _characterEffects[key].FirstOrDefault(e => e.Type == effect.Type);
            if (existing != null)
            {
                if (effect.Potency >= existing.Potency)
                {
                    _characterEffects[key].Remove(existing);
                    _characterEffects[key].Add(effect);
                    Console.WriteLine($"{effect.GetIcon()} {effect.Type} effect refreshed on {target.Name}!");
                }
                else
                {
                    Console.WriteLine($"{target.Name} resists the weaker {effect.Type} effect!");
                }
            }
            else
            {
                _characterEffects[key].Add(effect);
                Console.ForegroundColor = effect.GetColor();
                Console.WriteLine($"{effect.GetIcon()} {target.Name} is now {effect.Type}! ({effect.Duration} turns)");
                Console.ResetColor();
            }
        }

        public static void ProcessEffects(Character target)
        {
            string key = target.Name;
            if (!_characterEffects.ContainsKey(key)) return;

            var effects = _characterEffects[key].ToList();
            foreach (var effect in effects)
            {
                effect.ApplyEffect(target);
            }

            // Remove expired effects
            _characterEffects[key].RemoveAll(e => e.IsExpired);
            if (_characterEffects[key].Count == 0)
            {
                _characterEffects.Remove(key);
            }
        }

        public static bool HasEffect(Character target, StatusEffectType type)
        {
            string key = target.Name;
            return _characterEffects.ContainsKey(key) &&
                   _characterEffects[key].Any(e => e.Type == type);
        }

        public static bool IsStunned(Character target)
        {
            return HasEffect(target, StatusEffectType.Stunned);
        }

        public static double GetDamageModifier(Character target)
        {
            double modifier = 1.0;

            if (HasEffect(target, StatusEffectType.Weakened))
                modifier *= 0.70; // -30% damage

            return modifier;
        }

        public static double GetDamageTakenModifier(Character target)
        {
            double modifier = 1.0;

            if (HasEffect(target, StatusEffectType.Vulnerable))
                modifier *= 1.30; // +30% damage taken

            return modifier;
        }

        public static List<StatusEffect> GetActiveEffects(Character target)
        {
            string key = target.Name;
            return _characterEffects.ContainsKey(key)
                ? new List<StatusEffect>(_characterEffects[key])
                : new List<StatusEffect>();
        }

        public static void ClearAll(Character target)
        {
            string key = target.Name;
            if (_characterEffects.ContainsKey(key))
            {
                _characterEffects.Remove(key);
                Console.WriteLine($"All status effects cleared from {target.Name}!");
            }
        }

        public static void ClearAllCombatEffects()
        {
            _characterEffects.Clear();
            _mobEffects.Clear();
        }
    }
}
