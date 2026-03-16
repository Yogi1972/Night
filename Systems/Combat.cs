using Night.Characters;
using Night.Combat;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rpg_Dungeon
{
    /// <summary>
    /// Main Combat coordinator - delegates to specialized handlers
    /// Refactored for better maintainability and separation of concerns
    /// </summary>
    internal static class CombatSystem
    {
        #region Fields

        private static readonly Random _rng = new Random();
        public static Dictionary<string, bool> SpecialUsedLastTurn = new Dictionary<string, bool>();
        private static ComboSystem? _comboSystem;

        #endregion

        #region Utility Methods

        /// <summary>
        /// Roll a d20 (1-20)
        /// </summary>
        public static int RollD20()
        {
            return _rng.Next(1, 21);
        }

        #endregion

        #region Attack Methods - Delegated to CombatAttackHandler

        /// <summary>
        /// Character attacks a mob using a d20 roll
        /// </summary>
        public static void Attack(Character attacker, Mob target)
        {
            CombatAttackHandler.Attack(attacker, target);
        }

        /// <summary>
        /// Mob attacks a character using d20
        /// </summary>
        public static void Attack(Mob attacker, Character target)
        {
            CombatAttackHandler.Attack(attacker, target);
        }

        #endregion

        #region Combat Encounter - Main Entry Points

        /// <summary>
        /// Run a simple encounter between a party and a single mob. Returns true if party wins.
        /// </summary>
        public static bool RunEncounter(List<Character> party, Mob mob)
        {
            if (party == null || party.Count == 0 || mob == null) return false;

            int mobHp = mob.Health;

            // Display combat start
            DisplayCombatStart(party, mob, mobHp);

            // Reset threat levels
            ResetCombatState(party);

            // Main combat loop
            while (mobHp > 0 && party.Any(p => p.IsAlive))
            {
                // Party turn
                foreach (var member in party.Where(p => p.IsAlive))
                {
                    if (mobHp <= 0) break;

                    if (!ProcessCharacterTurn(member, mob, ref mobHp, party))
                    {
                        continue; // Stunned or skipped turn
                    }
                }

                if (mobHp <= 0) break;

                // Mob turn
                ProcessMobTurn(party, mob);
            }

            // Handle combat end
            return HandleCombatEnd(party, mob, mobHp);
        }

        /// <summary>
        /// Run an encounter with initiative-based turn order system
        /// </summary>
        public static bool RunEncounterWithTurnOrder(List<Character> party, Mob mob)
        {
            if (party == null || party.Count == 0 || mob == null) return false;

            int mobHp = mob.Health;

            // Display combat start
            DisplayCombatStart(party, mob, mobHp);

            // Display AI behavior
            if (mob.AI != null)
            {
                mob.AI.UpdateState(mobHp, mob.Health);
                mob.AI.DisplayBehavior();

                string battleCry = mob.AI.GetBattleCry();
                if (!string.IsNullOrEmpty(battleCry))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"💬 {mob.Name}: \"{battleCry}\"");
                    Console.ResetColor();
                }
            }
            Console.WriteLine();

            // Initialize combo system
            _comboSystem = new ComboSystem();
            _comboSystem.DisplayAvailableCombos(party);

            // Reset threat levels
            ResetCombatState(party);

            // Initialize turn order system
            var turnOrderManager = new TurnOrderManager();
            var enemies = new List<Mob> { mob };
            var player = party.FirstOrDefault(p => p.IsAlive);
            if (player == null) return false;

            turnOrderManager.GenerateTurnOrder(player, enemies);

            // Display initiative rolls
            DisplayInitiativeRolls(party, mob);
            turnOrderManager.DisplayTurnOrder();

            // Main combat loop with turn order
            while (turnOrderManager.ShouldContinueCombat() && mobHp > 0)
            {
                var actor = turnOrderManager.GetNextActor();

                if (actor.IsPlayer && actor.Character != null)
                {
                    ProcessPlayerTurnWithTurnOrder(actor.Character, mob, ref mobHp, party, turnOrderManager);
                }
                else if (!actor.IsPlayer && actor.Mob != null)
                {
                    ProcessMobTurnWithTurnOrder(party, mob, ref mobHp, turnOrderManager, actor.Mob);
                }

                // Check if combat should end
                if (mobHp <= 0)
                {
                    turnOrderManager.UpdateActorStatus(false, mob.Name, false);
                    break;
                }

                turnOrderManager.AdvanceTurn();

                // Optional: Show turn order every few turns
                if (turnOrderManager.GetRoundNumber() > 1 && actor.IsPlayer)
                {
                    Console.WriteLine("\n[Press Enter to continue]");
                    Console.ReadLine();
                }
            }

            // Handle combat end
            return HandleCombatEnd(party, mob, mobHp);
        }

        #endregion

        #region Combat Flow Helpers

        private static void DisplayCombatStart(List<Character> party, Mob mob, int mobHp)
        {
            int partyAvgLevel = CalculatePartyAverageLevel(party);
            string mobRank = Playerleveling.GetMobRankTitle(mob.Level, partyAvgLevel);

            Console.WriteLine();
            if (mobRank.Contains("BOSS") || mobRank.Contains("ELITE"))
            {
                VisualEffects.WriteDanger($"⚔️  COMBAT! A Level {mob.Level} {mob.Name} {mobRank} appears! (HP {mobHp})\n");
            }
            else
            {
                VisualEffects.WriteLineColored($"⚔️  Encounter begins! A Level {mob.Level} {mob.Name} {mobRank} appears! (HP {mobHp})", ConsoleColor.Yellow);
            }
            Console.WriteLine();
        }

        private static void DisplayInitiativeRolls(List<Character> party, Mob mob)
        {
            Console.WriteLine("🎲 Initiative Rolls:");
            foreach (var p in party.Where(x => x.IsAlive))
            {
                Console.WriteLine($"  {p.Name}: Agility {p.Agility} → Initiative calculated");
            }
            Console.WriteLine($"  {mob.Name}: Level {mob.Level} → Initiative calculated");
            Console.WriteLine();
        }

        private static void ResetCombatState(List<Character> party)
        {
            foreach (var p in party)
            {
                p.ThreatLevel = 0;
                if (p is Warrior warrior)
                {
                    warrior.IsTaunting = false;
                    warrior.TauntDuration = 0;
                }
            }
        }

        private static int CalculatePartyAverageLevel(List<Character> party)
        {
            int totalLevel = 0;
            int aliveCount = 0;
            foreach (var p in party)
            {
                if (p.IsAlive)
                {
                    totalLevel += p.Level;
                    aliveCount++;
                }
            }
            return aliveCount > 0 ? totalLevel / aliveCount : 1;
        }

        private static bool ProcessCharacterTurn(Character member, Mob mob, ref int mobHp, List<Character> party)
        {
            // Process status effects at start of turn
            StatusEffectManager.ProcessEffects(member);

            if (member.Abilities != null && member.Abilities.Count > 0)
            {
                member.ReduceAbilityCooldowns();
            }

            if (member.ActiveStatusEffects != null && member.ActiveStatusEffects.Count > 0)
            {
                member.ProcessStatusEffects();
            }

            // Check if stunned
            if (StatusEffectManager.IsStunned(member) || member.HasStatusEffect(StatusEffectType.Stunned))
            {
                Console.WriteLine($"\n💫 {member.Name} is stunned and skips this turn!");
                return false;
            }

            // Display turn info
            DisplayTurnInfo(member, mob, mobHp);

            // Get and process player action
            string action = GetPlayerAction(member);
            ProcessPlayerAction(member, action, mob, ref mobHp, party);

            return true;
        }

        private static void DisplayTurnInfo(Character member, Mob mob, int mobHp)
        {
            Console.WriteLine($"\n╔═══════════════════════════════════════════════════════════════════╗");
            Console.WriteLine($"║  {member.Name}'s Turn (Lv {member.Level})");
            Console.WriteLine($"╚═══════════════════════════════════════════════════════════════════╝");

            // Display stats
            Console.Write($"💚 HP: {member.Health}/{member.MaxHealth}");
            if (member is Warrior || member is Rogue)
                Console.Write($" | ⚡ Stamina: {member.Stamina}/{member.MaxStamina}");
            else if (member is Mage || member is Priest)
                Console.Write($" | 🔮 Mana: {member.Mana}/{member.MaxMana}");

            Console.Write($" | 🎯 Threat: {member.ThreatLevel}");
            if (member is Warrior w && w.IsTaunting)
            {
                Console.Write($" | 🛡️ [TAUNTING:{w.TauntDuration}]");
            }
            Console.WriteLine();

            // Display stance
            Console.Write($"Stance: {CombatStanceModifiers.GetStanceIcon(member.CurrentStance)} {member.CurrentStance}");

            // Display active status effects
            if (member.ActiveStatusEffects.Any())
            {
                Console.Write(" | Status: ");
                foreach (var effect in member.ActiveStatusEffects)
                {
                    Console.Write($"{effect.GetIcon()}{effect.Type}({effect.Duration}) ");
                }
            }
            Console.WriteLine();

            // Display enemy status
            Console.WriteLine($"\n🎯 Enemy: {mob.Name} (Lv {mob.Level}) | HP: {mobHp}/{mob.Health}");

            // Display abilities
            if (member.Abilities != null && member.Abilities.Count > 0)
            {
                Console.WriteLine("\n⚔️  ABILITIES:");
                CombatAbilityHandler.DisplayAbilities(member);
            }
        }

        private static string GetPlayerAction(Character member)
        {
            Console.WriteLine("\n📋 ACTIONS:");
            Console.Write($"1. ⚔️  Attack ");
            Console.Write($"\n2. ✨ Special (Legacy) ");

            if (member is Warrior)
            {
                Console.Write($"\n3. 🛡️  Taunt (Draw aggro) ");
                Console.Write($"\n4. 🔄 Change Stance ");
                Console.Write($"\n5. 💼 Use Item ");
                Console.Write($"\n6. ⏭️  Pass ");
            }
            else
            {
                Console.Write($"\n3. 🔄 Change Stance ");
                Console.Write($"\n4. 💼 Use Item ");
                Console.Write($"\n5. ⏭️  Pass ");
            }

            if (member.Abilities != null && member.Abilities.Count > 0)
            {
                Console.Write($"\n\n💫 Quick Abilities: A1-A{member.Abilities.Count}");
            }

            Console.Write("\n\nAction: ");
            return (Console.ReadLine() ?? string.Empty).Trim().ToUpper();
        }

        private static void ProcessPlayerAction(Character member, string action, Mob mob, ref int mobHp, List<Character> party)
        {
            // Handle ability quick-select (A1, A2, etc.)
            if (action.StartsWith("A") && action.Length >= 2 && member.Abilities != null && member.Abilities.Count > 0)
            {
                if (int.TryParse(action.Substring(1), out int abilityIndex) && abilityIndex >= 1 && abilityIndex <= member.Abilities.Count)
                {
                    var ability = member.Abilities[abilityIndex - 1];
                    CombatAbilityHandler.UseAbilityInCombat(member, ability, mob, ref mobHp, party, _comboSystem);
                    SpecialUsedLastTurn[member.Name] = false;
                    return;
                }
                else
                {
                    Console.WriteLine("Invalid ability selection!");
                    return;
                }
            }

            switch (action)
            {
                case "1": // Attack
                    SpecialUsedLastTurn[member.Name] = false;
                    CombatAttackHandler.PerformBasicAttack(member, mob, ref mobHp);
                    break;

                case "2": // Special (Legacy)
                    CombatAbilityHandler.UseSpecialAgainstMob(member, mob, ref mobHp, party, SpecialUsedLastTurn);
                    break;

                case "3":
                    if (member is Warrior)
                    {
                        // Taunt
                        SpecialUsedLastTurn[member.Name] = false;
                        ((Warrior)member).Taunt();
                    }
                    else
                    {
                        // Change Stance
                        CombatStanceManager.ChangeStanceMenu(member);
                    }
                    break;

                case "4":
                    if (member is Warrior)
                    {
                        // Change Stance (Warrior)
                        CombatStanceManager.ChangeStanceMenu(member);
                    }
                    else
                    {
                        // Use Item
                        SpecialUsedLastTurn[member.Name] = false;
                        CombatItemHandler.UseItemDuringCombat(member, party, mob, ref mobHp);
                    }
                    break;

                case "5":
                    if (member is Warrior)
                    {
                        // Use Item (Warrior)
                        SpecialUsedLastTurn[member.Name] = false;
                        CombatItemHandler.UseItemDuringCombat(member, party, mob, ref mobHp);
                    }
                    else
                    {
                        // Pass
                        Console.WriteLine($"{member.Name} passes.");
                    }
                    break;

                default:
                    Console.WriteLine($"{member.Name} passes.");
                    break;
            }
        }

        private static void ProcessMobTurn(List<Character> party, Mob mob)
        {
            // Decrement taunt duration for warriors
            foreach (var member in party.Where(p => p is Warrior))
            {
                ((Warrior)member).DecrementTaunt();
            }

            // Select target
            var target = SelectMobTarget(party, mob);
            if (target != null)
            {
                Attack(mob, target);
            }
        }

        private static Character? SelectMobTarget(List<Character> party, Mob mob)
        {
            var alive = party.Where(p => p.IsAlive).ToList();
            if (alive.Count == 0) return null;

            // Check for taunters first
            var taunters = alive.Where(p => p is Warrior warrior && warrior.IsTaunting).ToList();
            if (taunters.Any())
            {
                var target = taunters.OrderByDescending(p => p.ThreatLevel).First();
                Console.WriteLine($"\n🎯 {target.Name}'s taunt forces {mob.Name} to attack!");
                return target;
            }

            // Otherwise, target highest threat
            var target2 = alive.OrderByDescending(p => p.ThreatLevel).First();
            if (target2.ThreatLevel > 0)
            {
                Console.WriteLine($"\n🎯 {mob.Name} targets {target2.Name} (Threat: {target2.ThreatLevel})!");
            }
            return target2;
        }

        private static void ProcessPlayerTurnWithTurnOrder(Character member, Mob mob, ref int mobHp, 
            List<Character> party, TurnOrderManager turnOrderManager)
        {
            if (!member.IsAlive)
            {
                turnOrderManager.AdvanceTurn();
                return;
            }

            // Process status effects
            StatusEffectManager.ProcessEffects(member);
            if (member.Abilities != null && member.Abilities.Count > 0)
            {
                member.ReduceAbilityCooldowns();
            }
            if (member.ActiveStatusEffects != null && member.ActiveStatusEffects.Count > 0)
            {
                member.ProcessStatusEffects();
            }

            // Check if stunned
            if (StatusEffectManager.IsStunned(member) || member.HasStatusEffect(StatusEffectType.Stunned))
            {
                Console.WriteLine($"\n💫 {member.Name} is stunned and skips this turn!");
                turnOrderManager.AdvanceTurn();
                return;
            }

            // Display turn header with round number
            Console.WriteLine($"\n╔═══════════════════════════════════════════════════════════════════╗");
            Console.WriteLine($"║  {member.Name}'s Turn (Lv {member.Level}) - Round {turnOrderManager.GetRoundNumber()}");
            Console.WriteLine($"╚═══════════════════════════════════════════════════════════════════╝");

            DisplayTurnInfo(member, mob, mobHp);

            // Get and process action
            string action = GetPlayerAction(member);
            ProcessPlayerAction(member, action, mob, ref mobHp, party);

            // Update actor status
            turnOrderManager.UpdateActorStatus(true, member.Name, member.IsAlive);
        }

        private static void ProcessMobTurnWithTurnOrder(List<Character> party, Mob mob, ref int mobHp,
            TurnOrderManager turnOrderManager, Mob actorMob)
        {
            if (mobHp <= 0)
            {
                turnOrderManager.UpdateActorStatus(false, actorMob.Name, false);
                turnOrderManager.AdvanceTurn();
                return;
            }

            Console.WriteLine($"\n╔═══════════════════════════════════════════╗");
            Console.WriteLine($"║  {actorMob.Name}'s Turn - Round {turnOrderManager.GetRoundNumber()}");
            Console.WriteLine($"╚═══════════════════════════════════════════╝");

            // Decrement taunt duration
            foreach (var member in party.Where(p => p is Warrior))
            {
                ((Warrior)member).DecrementTaunt();
            }

            // Update AI state and check for special action
            if (mob.AI != null)
            {
                mob.AI.UpdateState(mobHp, mob.Health);

                if (mob.AI.ShouldUseSpecialAction(mobHp, mob.Health))
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"⚡ {mob.Name} {mob.AI.GetSpecialActionDescription()}");
                    Console.ResetColor();

                    int healedHp = mob.AI.ApplySpecialAction(mobHp, mob.Health);
                    if (healedHp > mobHp)
                    {
                        int healing = healedHp - mobHp;
                        mobHp = healedHp;
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"💚 {mob.Name} recovers {healing} HP! (HP: {mobHp}/{mob.Health})");
                        Console.ResetColor();
                    }

                    turnOrderManager.AdvanceTurn();
                    return;
                }
            }

            // Select target
            var target = SelectMobTargetWithAI(party, mob);
            if (target != null)
            {
                Attack(mob, target);
            }

            // Update actor status
            turnOrderManager.UpdateActorStatus(false, actorMob.Name, mobHp > 0);
        }

        private static Character? SelectMobTargetWithAI(List<Character> party, Mob mob)
        {
            var alive = party.Where(p => p.IsAlive).ToList();
            if (alive.Count == 0) return null;

            // Check for taunters
            var taunters = alive.Where(p => p is Warrior warrior && warrior.IsTaunting).ToList();
            if (taunters.Any())
            {
                var target = taunters.OrderByDescending(p => p.ThreatLevel).First();
                Console.WriteLine($"🎯 {target.Name}'s taunt forces {mob.Name} to attack!");
                return target;
            }

            // Use AI if available
            if (mob.AI != null)
            {
                var target = mob.AI.SelectTarget(alive);
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"🎯 {mob.AI.BehaviorIcon} AI targets {target.Name}!");
                Console.ResetColor();
                return target;
            }

            // Fallback to threat-based
            var target2 = alive.OrderByDescending(p => p.ThreatLevel).First();
            if (target2.ThreatLevel > 0)
            {
                Console.WriteLine($"🎯 {mob.Name} targets {target2.Name} (Threat: {target2.ThreatLevel})!");
            }
            return target2;
        }

        private static bool HandleCombatEnd(List<Character> party, Mob mob, int mobHp)
        {
            bool partyWon = mobHp <= 0;

            if (partyWon)
            {
                CombatRewardHandler.AwardVictoryRewards(party, mob, _rng);
            }
            else
            {
                VisualEffects.ShowDefeatBanner();
                VisualEffects.WriteDanger("💀 The party was defeated...\n");
            }

            // Clear all status effects after combat
            StatusEffectManager.ClearAllCombatEffects();

            return partyWon;
        }

        #endregion
    }
}
