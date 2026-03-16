using Night.Characters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rpg_Dungeon
{
    #region AI Behavior Types

    /// <summary>
    /// Different AI behavior patterns for enemies
    /// </summary>
    internal enum AIBehaviorType
    {
        Aggressive,     // Targets lowest HP characters
        Defensive,      // Focuses on survival, high armor
        Tactical,       // Targets mages/healers first
        Berserker,      // Gets stronger when low HP
        Support,        // Heals/buffs other enemies (for future multi-enemy)
        Balanced        // Standard behavior
    }

    #endregion

    /// <summary>
    /// AI behavior system for enemies with different tactical approaches
    /// </summary>
    internal class EnemyAI
    {
        #region Fields

        private readonly Random _rng = new Random();
        private AIBehaviorType _behaviorType;
        private bool _isBerserkActive = false;
        private int _originalHealth = 100;

        #endregion

        #region Properties

        public AIBehaviorType BehaviorType => _behaviorType;
        public string BehaviorName => GetBehaviorName();
        public string BehaviorIcon => GetBehaviorIcon();
        public bool IsBerserkActive => _isBerserkActive;

        #endregion

        #region Constructor

        public EnemyAI(AIBehaviorType behaviorType = AIBehaviorType.Balanced)
        {
            _behaviorType = behaviorType;
        }

        /// <summary>
        /// Creates AI based on mob level and type
        /// </summary>
        public static EnemyAI CreateForMob(Mob mob, Random? rng = null)
        {
            rng ??= new Random();

            // Assign behavior based on mob characteristics
            AIBehaviorType behavior;

            // High strength = Aggressive or Berserker
            if (mob.Strength > mob.Agility && mob.Strength > mob.Intelligence)
            {
                behavior = rng.Next(2) == 0 ? AIBehaviorType.Aggressive : AIBehaviorType.Berserker;
            }
            // High agility = Tactical
            else if (mob.Agility > mob.Strength && mob.Agility > mob.Intelligence)
            {
                behavior = AIBehaviorType.Tactical;
            }
            // High intelligence = Support or Tactical
            else if (mob.Intelligence > mob.Strength && mob.Intelligence > mob.Agility)
            {
                behavior = rng.Next(2) == 0 ? AIBehaviorType.Support : AIBehaviorType.Tactical;
            }
            // High armor = Defensive
            else if (mob.ArmorRating > 5)
            {
                behavior = AIBehaviorType.Defensive;
            }
            // Default
            else
            {
                behavior = AIBehaviorType.Balanced;
            }

            // Boss-level enemies (high level) more likely to be tactical or berserker
            if (mob.Level >= 10)
            {
                if (rng.Next(100) < 40) // 40% chance
                {
                    behavior = rng.Next(2) == 0 ? AIBehaviorType.Tactical : AIBehaviorType.Berserker;
                }
            }

            return new EnemyAI(behavior);
        }

        #endregion

        #region Behavior Names and Icons

        private string GetBehaviorName()
        {
            return _behaviorType switch
            {
                AIBehaviorType.Aggressive => "Aggressive",
                AIBehaviorType.Defensive => "Defensive",
                AIBehaviorType.Tactical => "Tactical",
                AIBehaviorType.Berserker => "Berserker",
                AIBehaviorType.Support => "Support",
                AIBehaviorType.Balanced => "Balanced",
                _ => "Unknown"
            };
        }

        private string GetBehaviorIcon()
        {
            return _behaviorType switch
            {
                AIBehaviorType.Aggressive => "⚔️",
                AIBehaviorType.Defensive => "🛡️",
                AIBehaviorType.Tactical => "🎯",
                AIBehaviorType.Berserker => "😈",
                AIBehaviorType.Support => "💚",
                AIBehaviorType.Balanced => "⚖️",
                _ => "❓"
            };
        }

        #endregion

        #region Target Selection

        /// <summary>
        /// Selects a target from the party based on AI behavior
        /// </summary>
        public Character SelectTarget(List<Character> party)
        {
            var aliveTargets = party.Where(c => c.IsAlive).ToList();
            if (aliveTargets.Count == 0) return party[0]; // Fallback
            if (aliveTargets.Count == 1) return aliveTargets[0]; // Only one choice

            return _behaviorType switch
            {
                AIBehaviorType.Aggressive => SelectLowestHealth(aliveTargets),
                AIBehaviorType.Tactical => SelectPriorityTarget(aliveTargets),
                AIBehaviorType.Defensive => SelectRandomTarget(aliveTargets),
                AIBehaviorType.Berserker => SelectLowestHealth(aliveTargets),
                AIBehaviorType.Support => SelectRandomTarget(aliveTargets),
                AIBehaviorType.Balanced => SelectBalancedTarget(aliveTargets),
                _ => SelectRandomTarget(aliveTargets)
            };
        }

        private Character SelectLowestHealth(List<Character> targets)
        {
            // Target character with lowest HP percentage
            return targets.OrderBy(c => (double)c.Health / c.MaxHealth).First();
        }

        private Character SelectPriorityTarget(List<Character> targets)
        {
            // Prioritize: Priest > Mage > Rogue > Warrior
            var priest = targets.FirstOrDefault(c => c is Priest);
            if (priest != null) return priest;

            var mage = targets.FirstOrDefault(c => c is Mage);
            if (mage != null) return mage;

            var rogue = targets.FirstOrDefault(c => c is Rogue);
            if (rogue != null) return rogue;

            return targets[0]; // Fallback to first (likely Warrior)
        }

        private Character SelectRandomTarget(List<Character> targets)
        {
            return targets[_rng.Next(targets.Count)];
        }

        private Character SelectBalancedTarget(List<Character> targets)
        {
            // 60% chance lowest HP, 40% random
            if (_rng.Next(100) < 60)
            {
                return SelectLowestHealth(targets);
            }
            return SelectRandomTarget(targets);
        }

        #endregion

        #region Combat Modifiers

        /// <summary>
        /// Updates AI state based on current health
        /// </summary>
        public void UpdateState(int currentHealth, int maxHealth)
        {
            _originalHealth = maxHealth;

            // Berserker activation at 30% HP
            if (_behaviorType == AIBehaviorType.Berserker)
            {
                double healthPercent = (double)currentHealth / maxHealth;
                _isBerserkActive = healthPercent <= 0.3;
            }
        }

        /// <summary>
        /// Gets damage modifier based on AI state
        /// </summary>
        public double GetDamageModifier()
        {
            return _behaviorType switch
            {
                AIBehaviorType.Aggressive => 1.15, // +15% damage
                AIBehaviorType.Berserker when _isBerserkActive => 1.5, // +50% when below 30% HP!
                AIBehaviorType.Berserker => 1.0,
                AIBehaviorType.Defensive => 0.85, // -15% damage
                AIBehaviorType.Tactical => 1.1, // +10% damage
                AIBehaviorType.Support => 0.9, // -10% damage
                AIBehaviorType.Balanced => 1.0,
                _ => 1.0
            };
        }

        /// <summary>
        /// Gets armor modifier based on AI state
        /// </summary>
        public int GetArmorBonus()
        {
            return _behaviorType switch
            {
                AIBehaviorType.Defensive => 5, // +5 armor
                AIBehaviorType.Aggressive => -2, // -2 armor (glass cannon)
                AIBehaviorType.Berserker when _isBerserkActive => -3, // -3 armor when berserk
                _ => 0
            };
        }

        /// <summary>
        /// Gets attack speed modifier (for future use with turn order)
        /// </summary>
        public int GetSpeedBonus()
        {
            return _behaviorType switch
            {
                AIBehaviorType.Tactical => 3, // +3 agility
                AIBehaviorType.Berserker when _isBerserkActive => 5, // +5 agility when berserk!
                AIBehaviorType.Defensive => -2, // -2 agility (slow but tanky)
                _ => 0
            };
        }

        #endregion

        #region Special Actions

        /// <summary>
        /// Checks if AI should perform a special action this turn
        /// </summary>
        public bool ShouldUseSpecialAction(int currentHealth, int maxHealth)
        {
            double healthPercent = (double)currentHealth / maxHealth;

            return _behaviorType switch
            {
                AIBehaviorType.Defensive when healthPercent < 0.5 => _rng.Next(100) < 30, // 30% chance to defend
                AIBehaviorType.Tactical => _rng.Next(100) < 15, // 15% chance for tactical move
                AIBehaviorType.Support when healthPercent < 0.7 => _rng.Next(100) < 25, // 25% chance to heal
                _ => false
            };
        }

        /// <summary>
        /// Gets description of special action
        /// </summary>
        public string GetSpecialActionDescription()
        {
            return _behaviorType switch
            {
                AIBehaviorType.Defensive => "takes a defensive stance!",
                AIBehaviorType.Tactical => "repositions tactically!",
                AIBehaviorType.Support => "attempts to heal itself!",
                _ => "prepares..."
            };
        }

        /// <summary>
        /// Applies special action effects
        /// </summary>
        public int ApplySpecialAction(int currentHealth, int maxHealth)
        {
            return _behaviorType switch
            {
                AIBehaviorType.Defensive => Math.Min(maxHealth, currentHealth + (maxHealth / 10)), // Heal 10%
                AIBehaviorType.Support => Math.Min(maxHealth, currentHealth + (maxHealth / 5)), // Heal 20%
                AIBehaviorType.Tactical => currentHealth, // No healing, just repositioning
                _ => currentHealth
            };
        }

        #endregion

        #region Pathfinding Integration

        /// <summary>
        /// Requests a path on a simple grid between two points using the central Pathfinding helper.
        /// This is a small adapter so EnemyAI consumers can ask for a path without directly using Pathfinding types.
        /// </summary>
        public List<Pathfinding.GridPoint> GetPathToTarget(int startX, int startY, int goalX, int goalY, bool[,] walkableGrid)
        {
            var start = new Pathfinding.GridPoint(startX, startY);
            var goal = new Pathfinding.GridPoint(goalX, goalY);
            try
            {
                return Pathfinding.GetPath(start, goal, walkableGrid);
            }
            catch
            {
                return new List<Pathfinding.GridPoint>();
            }
        }

        #endregion

        #region Display

        /// <summary>
        /// Displays AI behavior information
        /// </summary>
        public void DisplayBehavior()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write($"{BehaviorIcon} AI Behavior: ");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write(BehaviorName);
            Console.ResetColor();

            if (_behaviorType == AIBehaviorType.Berserker && _isBerserkActive)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write(" [BERSERK MODE!]");
                Console.ResetColor();
            }

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine($"   {GetBehaviorDescription()}");
            Console.ResetColor();
        }

        private string GetBehaviorDescription()
        {
            return _behaviorType switch
            {
                AIBehaviorType.Aggressive => "Focuses on dealing maximum damage to weak targets",
                AIBehaviorType.Defensive => "High defense, occasional healing",
                AIBehaviorType.Tactical => "Prioritizes dangerous targets like healers and mages",
                AIBehaviorType.Berserker => _isBerserkActive 
                    ? "⚠️ ENRAGED! Massive damage and speed boost!" 
                    : "Becomes extremely dangerous when injured",
                AIBehaviorType.Support => "Heals and supports (would buff allies in group combat)",
                AIBehaviorType.Balanced => "Standard combat approach",
                _ => "Unknown behavior pattern"
            };
        }

        /// <summary>
        /// Gets a battle cry or taunt based on behavior
        /// </summary>
        public string GetBattleCry()
        {
            if (_behaviorType == AIBehaviorType.Berserker && _isBerserkActive)
            {
                string[] berserkerCries = {
                    "RAAAAGH! I will destroy you!",
                    "You will PAY for this!",
                    "My rage knows no bounds!",
                    "Feel my wrath!"
                };
                return berserkerCries[_rng.Next(berserkerCries.Length)];
            }

            return _behaviorType switch
            {
                AIBehaviorType.Aggressive => "I will crush the weak!",
                AIBehaviorType.Tactical => "Your healer won't save you...",
                AIBehaviorType.Defensive => "My defense is impenetrable!",
                AIBehaviorType.Support => "I will endure!",
                _ => ""
            };
        }

        #endregion
    }
}
