using Night.Characters;
using System;
using System.Collections.Generic;

namespace Rpg_Dungeon.Combat
{
    /// <summary>
    /// Environmental hazards that affect combat
    /// </summary>
    internal enum HazardType
    {
        None,
        Fire,           // Fire pools that deal damage
        Poison,         // Toxic clouds
        Spikes,         // Floor spikes
        Lightning,      // Electric arcs
        Ice,            // Slippery ice patches
        Earthquake,     // Tremors that stun
        Lava,           // Molten lava
        Thorns,         // Thorny vines
        Darkness,       // Reduced accuracy
        Sandstorm       // Reduces visibility
    }

    /// <summary>
    /// Environmental hazard in combat
    /// </summary>
    internal class EnvironmentalHazard
    {
        public HazardType Type { get; private set; }
        public int Intensity { get; private set; }
        public int Duration { get; set; }
        public string Description { get; private set; }

        public EnvironmentalHazard(HazardType type, int intensity, int duration)
        {
            Type = type;
            Intensity = intensity;
            Duration = duration;
            Description = GetDescription();
        }

        private string GetDescription()
        {
            return Type switch
            {
                HazardType.Fire => "🔥 Flames engulf the battlefield!",
                HazardType.Poison => "☠️  Toxic fumes fill the air!",
                HazardType.Spikes => "⚠️  Sharp spikes emerge from the ground!",
                HazardType.Lightning => "⚡ Lightning crackles overhead!",
                HazardType.Ice => "❄️  The ground is covered in ice!",
                HazardType.Earthquake => "💥 The ground shakes violently!",
                HazardType.Lava => "🌋 Molten lava bubbles up!",
                HazardType.Thorns => "🌿 Thorny vines cover the area!",
                HazardType.Darkness => "🌑 Darkness obscures your vision!",
                HazardType.Sandstorm => "🌪️  A sandstorm rages!",
                _ => "⚠️  Something strange is happening..."
            };
        }

        public string GetIcon()
        {
            return Type switch
            {
                HazardType.Fire => "🔥",
                HazardType.Poison => "☠️",
                HazardType.Spikes => "⚠️",
                HazardType.Lightning => "⚡",
                HazardType.Ice => "❄️",
                HazardType.Earthquake => "💥",
                HazardType.Lava => "🌋",
                HazardType.Thorns => "🌿",
                HazardType.Darkness => "🌑",
                HazardType.Sandstorm => "🌪️",
                _ => "⚠️"
            };
        }

        public void ApplyHazardEffect(Character target)
        {
            if (Duration <= 0) return;

            var rng = new Random();
            
            Console.WriteLine();
            VisualEffects.WriteColored($"{GetIcon()} ", GetHazardColor());
            Console.Write($"{target.Name} is affected by the {Type.ToString().ToLower()}! ");

            switch (Type)
            {
                case HazardType.Fire:
                    int fireDamage = Intensity + rng.Next(1, 6);
                    VisualEffects.WriteColored($"Takes {fireDamage} fire damage!\n", ConsoleColor.Red);
                    target.ReceiveDamage(fireDamage);
                    
                    if (rng.Next(1, 101) <= 20)
                    {
                        target.AddStatusEffect(new StatusEffect(StatusEffectType.Burning, 2, 3, "Fire Hazard"));
                        Console.WriteLine("   💥 Caught on fire!");
                    }
                    break;

                case HazardType.Poison:
                    int poisonDamage = Intensity;
                    VisualEffects.WriteColored($"Takes {poisonDamage} poison damage!\n", ConsoleColor.Green);
                    target.ReceiveDamage(poisonDamage);
                    target.AddStatusEffect(new StatusEffect(StatusEffectType.Poisoned, 2, 4, "Poison Cloud"));
                    break;

                case HazardType.Spikes:
                    int spikeDamage = Intensity + rng.Next(3, 8);
                    VisualEffects.WriteColored($"Pierced for {spikeDamage} damage!\n", ConsoleColor.Red);
                    target.ReceiveDamage(spikeDamage);
                    
                    if (rng.Next(1, 101) <= 30)
                    {
                        target.AddStatusEffect(new StatusEffect(StatusEffectType.Bleeding, 3, 3, "Spike Trap"));
                        Console.WriteLine("   🩸 Started bleeding!");
                    }
                    break;

                case HazardType.Lightning:
                    int lightningDamage = Intensity + rng.Next(5, 12);
                    VisualEffects.WriteColored($"Shocked for {lightningDamage} damage!\n", ConsoleColor.Cyan);
                    target.ReceiveDamage(lightningDamage);
                    
                    if (rng.Next(1, 101) <= 25)
                    {
                        target.AddStatusEffect(new StatusEffect(StatusEffectType.Stunned, 1, 0, "Lightning"));
                        Console.WriteLine("   ⚡ Stunned!");
                    }
                    break;

                case HazardType.Ice:
                    if (rng.Next(1, 101) <= 40)
                    {
                        VisualEffects.WriteColored("Slips on the ice!\n", ConsoleColor.Cyan);
                        target.AddStatusEffect(new StatusEffect(StatusEffectType.Frozen, 1, 0, "Ice"));
                    }
                    else
                    {
                        Console.WriteLine("Manages to keep balance!");
                    }
                    break;

                case HazardType.Earthquake:
                    int earthquakeDamage = Intensity / 2;
                    VisualEffects.WriteColored($"Knocked down for {earthquakeDamage} damage!\n", ConsoleColor.Yellow);
                    target.ReceiveDamage(earthquakeDamage);
                    
                    if (rng.Next(1, 101) <= 35)
                    {
                        target.AddStatusEffect(new StatusEffect(StatusEffectType.Stunned, 1, 0, "Earthquake"));
                        Console.WriteLine("   💫 Stunned by the tremor!");
                    }
                    break;

                case HazardType.Lava:
                    int lavaDamage = Intensity * 2;
                    VisualEffects.WriteColored($"BURNED for {lavaDamage} damage!\n", ConsoleColor.DarkRed);
                    target.ReceiveDamage(lavaDamage);
                    target.AddStatusEffect(new StatusEffect(StatusEffectType.Burning, 3, 5, "Lava"));
                    break;

                case HazardType.Thorns:
                    int thornDamage = Intensity + rng.Next(2, 5);
                    VisualEffects.WriteColored($"Entangled for {thornDamage} damage!\n", ConsoleColor.Green);
                    target.ReceiveDamage(thornDamage);
                    target.AddStatusEffect(new StatusEffect(StatusEffectType.Bleeding, 2, 2, "Thorns"));
                    break;

                case HazardType.Darkness:
                    VisualEffects.WriteColored("Accuracy reduced!\n", ConsoleColor.DarkGray);
                    Console.WriteLine("   🌑 30% miss chance added!");
                    break;

                case HazardType.Sandstorm:
                    int sandDamage = Intensity / 2;
                    VisualEffects.WriteColored($"Buffeted for {sandDamage} damage!\n", ConsoleColor.Yellow);
                    target.ReceiveDamage(sandDamage);
                    Console.WriteLine("   👁️  Vision obscured!");
                    break;
            }

            Duration--;
            System.Threading.Thread.Sleep(800);
        }

        private ConsoleColor GetHazardColor()
        {
            return Type switch
            {
                HazardType.Fire or HazardType.Lava => ConsoleColor.Red,
                HazardType.Poison or HazardType.Thorns => ConsoleColor.Green,
                HazardType.Lightning or HazardType.Ice => ConsoleColor.Cyan,
                HazardType.Earthquake or HazardType.Spikes => ConsoleColor.Yellow,
                HazardType.Darkness => ConsoleColor.DarkGray,
                HazardType.Sandstorm => ConsoleColor.DarkYellow,
                _ => ConsoleColor.White
            };
        }

        public bool IsExpired => Duration <= 0;

        public void AnnounceHazard()
        {
            Console.WriteLine();
            VisualEffects.WriteLineColored("═══════════════════════════════════════════════════════════", ConsoleColor.Yellow);
            VisualEffects.WriteLineColored($"  {Description}", GetHazardColor());
            VisualEffects.WriteLineColored($"  Duration: {Duration} turns | Intensity: {Intensity}", ConsoleColor.Gray);
            VisualEffects.WriteLineColored("═══════════════════════════════════════════════════════════", ConsoleColor.Yellow);
            Console.WriteLine();
            System.Threading.Thread.Sleep(1500);
        }
    }

    /// <summary>
    /// Manager for environmental hazards in combat
    /// </summary>
    internal static class HazardManager
    {
        /// <summary>
        /// Determine if a hazard should spawn based on location and level
        /// </summary>
        public static EnvironmentalHazard? GenerateHazard(string location, int areaLevel)
        {
            var rng = new Random();
            
            // 25% base chance + area level scaling
            int hazardChance = 25 + Math.Min(areaLevel / 3, 15);
            
            if (rng.Next(1, 101) > hazardChance)
            {
                return null;
            }

            HazardType hazardType = location.ToLower() switch
            {
                var l when l.Contains("volcano") || l.Contains("fire") => HazardType.Lava,
                var l when l.Contains("swamp") || l.Contains("bog") => HazardType.Poison,
                var l when l.Contains("cave") || l.Contains("dungeon") => rng.Next(1, 101) <= 50 ? HazardType.Spikes : HazardType.Darkness,
                var l when l.Contains("storm") || l.Contains("sky") => HazardType.Lightning,
                var l when l.Contains("ice") || l.Contains("frost") || l.Contains("tundra") => HazardType.Ice,
                var l when l.Contains("desert") || l.Contains("wasteland") => HazardType.Sandstorm,
                var l when l.Contains("forest") || l.Contains("jungle") => HazardType.Thorns,
                var l when l.Contains("ruins") => HazardType.Earthquake,
                _ => (HazardType)rng.Next(1, 11) // Random hazard
            };

            int intensity = 5 + (areaLevel / 2) + rng.Next(1, 6);
            int duration = 3 + rng.Next(0, 4);

            return new EnvironmentalHazard(hazardType, intensity, duration);
        }

        /// <summary>
        /// Apply hazard to all combatants
        /// </summary>
        public static void ApplyHazardToAll(EnvironmentalHazard? hazard, List<Character> combatants)
        {
            if (hazard == null || hazard.IsExpired) return;

            foreach (var character in combatants)
            {
                if (character.IsAlive)
                {
                    hazard.ApplyHazardEffect(character);
                }
            }
        }

        /// <summary>
        /// Get accuracy penalty from hazard
        /// </summary>
        public static double GetAccuracyModifier(EnvironmentalHazard? hazard)
        {
            if (hazard == null || hazard.IsExpired) return 1.0;

            return hazard.Type switch
            {
                HazardType.Darkness => 0.7,    // 30% miss chance
                HazardType.Sandstorm => 0.85,  // 15% miss chance
                HazardType.Ice => 0.9,         // 10% miss chance
                _ => 1.0
            };
        }

        /// <summary>
        /// Check if character can move freely (not frozen/stunned by hazard)
        /// </summary>
        public static bool CanActFreely(Character character, EnvironmentalHazard? hazard)
        {
            if (hazard == null || hazard.IsExpired) return true;

            return hazard.Type != HazardType.Ice || !character.HasStatusEffect(StatusEffectType.Frozen);
        }
    }
}
