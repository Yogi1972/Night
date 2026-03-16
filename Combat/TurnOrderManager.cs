using Night.Characters;
using Rpg_Dungeon;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Night.Combat
{
    /// <summary>
    /// Manages turn order in combat based on initiative and agility
    /// </summary>
    internal class TurnOrderManager
    {
        private List<CombatActor> _turnQueue;
        private int _currentTurnIndex;
        private int _roundNumber;

        public TurnOrderManager()
        {
            _turnQueue = new List<CombatActor>();
            _currentTurnIndex = 0;
            _roundNumber = 1;
        }

        /// <summary>
        /// Calculate initiative for a combatant (Agility + d20 roll)
        /// </summary>
        public int CalculateInitiative(Character character)
        {
            Random rand = new Random(Guid.NewGuid().GetHashCode());
            int agilityBonus = character.Agility / 2; // Agility provides bonus to initiative
            int roll = rand.Next(1, 21); // d20 roll
            return agilityBonus + roll;
        }

        /// <summary>
        /// Calculate initiative for a mob
        /// </summary>
        public int CalculateInitiative(Mob mob)
        {
            Random rand = new Random(Guid.NewGuid().GetHashCode());
            // Mobs get a flat bonus based on their level
            int levelBonus = mob.Level / 2;
            int roll = rand.Next(1, 21); // d20 roll
            return levelBonus + roll;
        }

        /// <summary>
        /// Generate turn order for the combat encounter
        /// </summary>
        public void GenerateTurnOrder(Character player, List<Mob> enemies)
        {
            _turnQueue.Clear();
            _currentTurnIndex = 0;
            _roundNumber = 1;

            // Add player to queue
            _turnQueue.Add(new CombatActor
            {
                IsPlayer = true,
                Character = player,
                Mob = null,
                Initiative = CalculateInitiative(player),
                Name = player.Name,
                IsAlive = player.IsAlive
            });

            // Add enemies to queue
            foreach (var enemy in enemies)
            {
                _turnQueue.Add(new CombatActor
                {
                    IsPlayer = false,
                    Character = null,
                    Mob = enemy,
                    Initiative = CalculateInitiative(enemy),
                    Name = enemy.Name,
                    IsAlive = true // Mobs start alive
                });
            }

            // Sort by initiative (highest first)
            _turnQueue = _turnQueue.OrderByDescending(a => a.Initiative).ToList();
        }

        /// <summary>
        /// Get the next actor in turn order
        /// </summary>
        public CombatActor GetNextActor()
        {
            // Skip dead actors
            while (_currentTurnIndex < _turnQueue.Count)
            {
                var actor = _turnQueue[_currentTurnIndex];
                
                if (actor.IsAlive)
                {
                    return actor;
                }
                
                _currentTurnIndex++;
            }

            // If we've gone through everyone, start a new round
            _currentTurnIndex = 0;
            _roundNumber++;
            
            return GetNextActor();
        }

        /// <summary>
        /// Advance to the next turn
        /// </summary>
        public void AdvanceTurn()
        {
            _currentTurnIndex++;
            
            // If we've reached the end of the queue, start a new round
            if (_currentTurnIndex >= _turnQueue.Count)
            {
                _currentTurnIndex = 0;
                _roundNumber++;
            }
        }

        /// <summary>
        /// Update the alive status of an actor
        /// </summary>
        public void UpdateActorStatus(bool isPlayer, string name, bool isAlive)
        {
            var actor = _turnQueue.FirstOrDefault(a => a.IsPlayer == isPlayer && a.Name == name);
            if (actor != null)
            {
                actor.IsAlive = isAlive;
            }
        }

        /// <summary>
        /// Display the turn order to the player
        /// </summary>
        public void DisplayTurnOrder()
        {
            Console.WriteLine();
            Console.WriteLine("╔════════════════════════════════════════════╗");
            Console.WriteLine($"║  Turn Order - Round {_roundNumber,-2}                      ║");
            Console.WriteLine("╚════════════════════════════════════════════╝");
            
            for (int i = 0; i < _turnQueue.Count; i++)
            {
                var actor = _turnQueue[i];
                
                if (!actor.IsAlive)
                    continue;

                string indicator = (i == _currentTurnIndex) ? "➤" : " ";
                string typeIcon = actor.IsPlayer ? "🎮" : "👹";
                string nameDisplay = actor.Name.Length > 20 ? actor.Name.Substring(0, 20) : actor.Name;
                
                Console.ForegroundColor = (i == _currentTurnIndex) ? ConsoleColor.Yellow : ConsoleColor.Gray;
                Console.WriteLine($"{indicator} {typeIcon} {nameDisplay,-20} [Init: {actor.Initiative}]");
                Console.ResetColor();
            }
            
            Console.WriteLine();
        }

        /// <summary>
        /// Get the current round number
        /// </summary>
        public int GetRoundNumber()
        {
            return _roundNumber;
        }

        /// <summary>
        /// Check if combat should continue (at least one player and one enemy alive)
        /// </summary>
        public bool ShouldContinueCombat()
        {
            bool hasLivingPlayer = _turnQueue.Any(a => a.IsPlayer && a.IsAlive);
            bool hasLivingEnemy = _turnQueue.Any(a => !a.IsPlayer && a.IsAlive);
            
            return hasLivingPlayer && hasLivingEnemy;
        }

        /// <summary>
        /// Get remaining combatants count
        /// </summary>
        public (int players, int enemies) GetRemainingCombatants()
        {
            int players = _turnQueue.Count(a => a.IsPlayer && a.IsAlive);
            int enemies = _turnQueue.Count(a => !a.IsPlayer && a.IsAlive);
            
            return (players, enemies);
        }
    }

    /// <summary>
    /// Represents a combatant in the turn order
    /// </summary>
    internal class CombatActor
    {
        public bool IsPlayer { get; set; }
        public Character? Character { get; set; }
        public Mob? Mob { get; set; }
        public int Initiative { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsAlive { get; set; }
    }
}
