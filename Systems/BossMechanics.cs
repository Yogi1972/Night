using Night.Characters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rpg_Dungeon.Systems
{
    /// <summary>
    /// Boss-specific combat mechanics including phases, enrage, and special abilities
    /// </summary>
    internal static class BossMechanics
    {
        /// <summary>
        /// Boss phase data
        /// </summary>
        public class BossPhase
        {
            public int PhaseNumber { get; set; }
            public double HPThreshold { get; set; } // Percentage (0.0 - 1.0)
            public string PhaseName { get; set; } = "";
            public string TransitionMessage { get; set; } = "";
            public Dictionary<string, int> StatModifiers { get; set; } = new();
            public List<string> NewAbilities { get; set; } = new();
            public string PhaseEffect { get; set; } = ""; // AOE, summon, etc.
        }

        /// <summary>
        /// Boss fight state
        /// </summary>
        public class BossFightState
        {
            public int CurrentPhase { get; set; } = 1;
            public bool IsEnraged { get; set; } = false;
            public int EnrageTurnCounter { get; set; } = 0;
            public int TurnsSincePhaseChange { get; set; } = 0;
            public List<Character> SummonedMinions { get; set; } = new();
            public Dictionary<string, int> PhaseAbilityCooldowns { get; set; } = new();
        }

        /// <summary>
        /// Check if boss should transition to next phase
        /// </summary>
        public static bool CheckPhaseTransition(Character boss, List<BossPhase> phases, BossFightState state)
        {
            if (boss == null || phases == null || state == null) return false;

            double hpPercent = (double)boss.Health / boss.MaxHealth;
            var nextPhase = phases.FirstOrDefault(p => p.PhaseNumber == state.CurrentPhase + 1);

            if (nextPhase != null && hpPercent <= nextPhase.HPThreshold)
            {
                TransitionToPhase(boss, nextPhase, state);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Transition boss to new phase
        /// </summary>
        private static void TransitionToPhase(Character boss, BossPhase phase, BossFightState state)
        {
            Console.WriteLine();
            VisualEffects.WriteLineColored("═══════════════════════════════════════════════════════════", ConsoleColor.Red);
            VisualEffects.WriteLineColored($"💀 {phase.TransitionMessage}", ConsoleColor.Yellow);
            VisualEffects.WriteLineColored("═══════════════════════════════════════════════════════════", ConsoleColor.Red);
            Console.WriteLine();

            state.CurrentPhase = phase.PhaseNumber;
            state.TurnsSincePhaseChange = 0;

            // Apply stat modifiers
            foreach (var modifier in phase.StatModifiers)
            {
                switch (modifier.Key.ToLower())
                {
                    case "attack":
                        VisualEffects.WriteColored($"⚔️  {boss.Name}'s attack increases by {modifier.Value}!\n", ConsoleColor.Red);
                        break;
                    case "defense":
                        VisualEffects.WriteColored($"🛡️  {boss.Name}'s defense increases by {modifier.Value}!\n", ConsoleColor.Yellow);
                        break;
                    case "speed":
                        VisualEffects.WriteColored($"⚡ {boss.Name}'s speed increases by {modifier.Value}!\n", ConsoleColor.Cyan);
                        break;
                }
            }

            // Execute phase effect
            ExecutePhaseEffect(boss, phase, state);

            System.Threading.Thread.Sleep(2000);
        }

        /// <summary>
        /// Execute phase-specific effect
        /// </summary>
        private static void ExecutePhaseEffect(Character boss, BossPhase phase, BossFightState state)
        {
            switch (phase.PhaseEffect.ToLower())
            {
                case "aoe":
                    VisualEffects.WriteLineColored($"💥 {boss.Name} unleashes a devastating area attack!", ConsoleColor.Red);
                    Console.WriteLine("⚠️  All party members take damage!");
                    break;

                case "summon":
                    VisualEffects.WriteLineColored($"🌀 {boss.Name} summons reinforcements!", ConsoleColor.Magenta);
                    // Summon logic would be implemented in combat system
                    break;

                case "heal":
                    int healAmount = boss.MaxHealth / 10;
                    boss.Heal(healAmount);
                    VisualEffects.WriteLineColored($"✨ {boss.Name} regenerates {healAmount} HP!", ConsoleColor.Green);
                    break;

                case "enrage":
                    state.IsEnraged = true;
                    VisualEffects.WriteLineColored($"😤 {boss.Name} enters a berserker rage!", ConsoleColor.DarkRed);
                    Console.WriteLine("💢 Attack speed and damage greatly increased!");
                    break;

                case "shield":
                    VisualEffects.WriteLineColored($"🛡️  {boss.Name} conjures a protective barrier!", ConsoleColor.Cyan);
                    Console.WriteLine("⚠️  Damage reduced by 50% for the next 3 turns!");
                    break;
            }
        }

        /// <summary>
        /// Check and trigger enrage mechanic
        /// </summary>
        public static void CheckEnrage(Character boss, BossFightState state, int turnLimit)
        {
            if (state.IsEnraged) return;

            state.EnrageTurnCounter++;

            if (state.EnrageTurnCounter >= turnLimit)
            {
                TriggerEnrage(boss, state);
            }
            else if (state.EnrageTurnCounter == turnLimit - 3)
            {
                VisualEffects.WriteColored("⚠️  WARNING: Boss is becoming unstable! ", ConsoleColor.Yellow);
                Console.WriteLine($"Defeat it in {turnLimit - state.EnrageTurnCounter} turns or it will enrage!");
            }
        }

        /// <summary>
        /// Trigger boss enrage
        /// </summary>
        private static void TriggerEnrage(Character boss, BossFightState state)
        {
            state.IsEnraged = true;

            Console.WriteLine();
            VisualEffects.WriteLineColored("╔═══════════════════════════════════════════════════════════╗", ConsoleColor.DarkRed);
            VisualEffects.WriteLineColored("║                    ⚠️  BOSS ENRAGED ⚠️                     ║", ConsoleColor.Red);
            VisualEffects.WriteLineColored("╚═══════════════════════════════════════════════════════════╝", ConsoleColor.DarkRed);
            Console.WriteLine();

            VisualEffects.WriteLineColored($"💢 {boss.Name} has lost patience and entered a berserk state!", ConsoleColor.Red);
            Console.WriteLine();
            Console.WriteLine("  • Attack damage DOUBLED");
            Console.WriteLine("  • Attack speed increased by 50%");
            Console.WriteLine("  • Special abilities used more frequently");
            Console.WriteLine();

            System.Threading.Thread.Sleep(2500);
        }

        /// <summary>
        /// Get damage multiplier based on boss state
        /// </summary>
        public static double GetDamageMultiplier(BossFightState state)
        {
            double multiplier = 1.0;

            if (state.IsEnraged)
            {
                multiplier *= 2.0;
            }

            // Phase-based scaling
            multiplier *= 1.0 + (state.CurrentPhase - 1) * 0.25;

            return multiplier;
        }

        /// <summary>
        /// Execute boss special ability
        /// </summary>
        public static void ExecuteBossAbility(Character boss, Character target, string abilityName, BossFightState state)
        {
            Console.WriteLine();
            VisualEffects.WriteLineColored($"💫 {boss.Name} uses {abilityName}!", ConsoleColor.Magenta);
            Console.WriteLine();

            var rng = new Random();

            switch (abilityName.ToLower())
            {
                case "devastating strike":
                    int damage = (int)(boss.Strength * 2 * GetDamageMultiplier(state));
                    VisualEffects.WriteColored($"⚔️  Massive strike for {damage} damage!\n", ConsoleColor.Red);
                    target.ReceiveDamage(damage);
                    break;

                case "life drain":
                    int drainDamage = boss.Strength + 15;
                    int healAmount = drainDamage / 2;
                    VisualEffects.WriteColored($"🩸 Drains {drainDamage} HP and heals for {healAmount}!\n", ConsoleColor.DarkRed);
                    target.ReceiveDamage(drainDamage);
                    boss.Heal(healAmount);
                    break;

                case "ground slam":
                    VisualEffects.WriteColored("💥 The ground shakes violently!\n", ConsoleColor.Yellow);
                    Console.WriteLine("⚠️  All combatants are stunned for 1 turn!");
                    break;

                case "shadow bolt":
                    int magicDamage = (int)(boss.Intelligence * 1.5);
                    VisualEffects.WriteColored($"🌑 Dark magic deals {magicDamage} damage!\n", ConsoleColor.DarkMagenta);
                    target.ReceiveDamage(magicDamage);
                    
                    if (rng.Next(1, 101) <= 30)
                    {
                        Console.WriteLine("💀 Target is cursed! (-10% damage for 3 turns)");
                        target.AddStatusEffect(new StatusEffect(
                            StatusEffectType.Weakened,
                            duration: 3,
                            potency: 10,
                            source: boss.Name
                        ));
                    }
                    break;

                case "summon adds":
                    int minionCount = 2;
                    VisualEffects.WriteColored($"🌀 Summons {minionCount} minions!\n", ConsoleColor.Magenta);
                    Console.WriteLine("⚠️  Focus on the boss or clear the adds!");
                    break;

                default:
                    VisualEffects.WriteColored($"❓ {boss.Name} uses an unknown ability!\n", ConsoleColor.Gray);
                    break;
            }

            System.Threading.Thread.Sleep(1500);
        }

        /// <summary>
        /// Create a predefined boss with phases
        /// </summary>
        public static List<BossPhase> CreateBossPhases(string bossType)
        {
            return bossType.ToLower() switch
            {
                "goblin_king" => new List<BossPhase>
                {
                    new BossPhase
                    {
                        PhaseNumber = 1,
                        HPThreshold = 1.0,
                        PhaseName = "Confident",
                        TransitionMessage = "The battle begins!",
                        StatModifiers = new Dictionary<string, int>(),
                        NewAbilities = new List<string> { "Devastating Strike" }
                    },
                    new BossPhase
                    {
                        PhaseNumber = 2,
                        HPThreshold = 0.66,
                        PhaseName = "Wounded",
                        TransitionMessage = "The Goblin King roars in fury!",
                        StatModifiers = new Dictionary<string, int> { { "attack", 5 } },
                        NewAbilities = new List<string> { "Ground Slam" },
                        PhaseEffect = "summon"
                    },
                    new BossPhase
                    {
                        PhaseNumber = 3,
                        HPThreshold = 0.33,
                        PhaseName = "Desperate",
                        TransitionMessage = "The Goblin King enters a berserker rage!",
                        StatModifiers = new Dictionary<string, int> { { "attack", 10 }, { "speed", 3 } },
                        NewAbilities = new List<string> { "Life Drain" },
                        PhaseEffect = "enrage"
                    }
                },

                "dark_sorcerer" => new List<BossPhase>
                {
                    new BossPhase
                    {
                        PhaseNumber = 1,
                        HPThreshold = 1.0,
                        PhaseName = "Studying",
                        TransitionMessage = "The Dark Sorcerer begins weaving dark magic!",
                        NewAbilities = new List<string> { "Shadow Bolt" }
                    },
                    new BossPhase
                    {
                        PhaseNumber = 2,
                        HPThreshold = 0.5,
                        PhaseName = "Channeling",
                        TransitionMessage = "Dark energies swirl around the sorcerer!",
                        StatModifiers = new Dictionary<string, int> { { "defense", 10 } },
                        NewAbilities = new List<string> { "Life Drain", "Summon Adds" },
                        PhaseEffect = "shield"
                    },
                    new BossPhase
                    {
                        PhaseNumber = 3,
                        HPThreshold = 0.25,
                        PhaseName = "Unleashed",
                        TransitionMessage = "The sorcerer unleashes his full power!",
                        StatModifiers = new Dictionary<string, int> { { "attack", 15 } },
                        PhaseEffect = "aoe"
                    }
                },

                _ => new List<BossPhase>
                {
                    new BossPhase
                    {
                        PhaseNumber = 1,
                        HPThreshold = 1.0,
                        PhaseName = "Normal",
                        TransitionMessage = "Boss fight begins!"
                    }
                }
            };
        }
    }
}
