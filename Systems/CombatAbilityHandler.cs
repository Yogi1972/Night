using Night.Characters;
using Night.Combat;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rpg_Dungeon
{
    /// <summary>
    /// Handles all combat ability display and execution
    /// </summary>
    internal static class CombatAbilityHandler
    {
        private static readonly Random _rng = new Random();

        /// <summary>
        /// Display character's available abilities with cooldowns and costs
        /// </summary>
        public static void DisplayAbilities(Character character)
        {
            if (character.Abilities == null || character.Abilities.Count == 0)
            {
                Console.WriteLine("  No abilities available.");
                return;
            }

            for (int i = 0; i < character.Abilities.Count; i++)
            {
                var ability = character.Abilities[i];
                Console.ForegroundColor = ability.CanUse(character) ? ConsoleColor.Cyan : ConsoleColor.DarkGray;

                string status = ability.CanUse(character) ? "✓" : "✗";
                string cooldownText = ability.CurrentCooldown > 0 ? $" (CD: {ability.CurrentCooldown})" : "";

                // Show effective cost (accounts for Sage mana reduction + skill tree efficiency)
                int displayCost = ability.ResourceCost;
                if (ability.ResourceType == ResourceType.Mana)
                {
                    displayCost = character.GetEffectiveManaCost(ability.ResourceCost);
                }
                string costText = displayCost > 0 ? $" [{displayCost} {ability.ResourceType}]" : "";

                Console.WriteLine($"  A{i + 1}. {status} {ability.Icon} {ability.Name}{costText}{cooldownText}");

                if (!ability.CanUse(character) && ability.CurrentCooldown == 0)
                {
                    Console.WriteLine($"      ⚠️  Insufficient {ability.ResourceType}");
                }

                Console.ResetColor();
            }
        }

        /// <summary>
        /// Use an ability in combat
        /// </summary>
        public static void UseAbilityInCombat(Character user, CombatAbility ability, Mob target, 
            ref int mobHp, List<Character> party, ComboSystem? comboSystem = null)
        {
            if (!ability.CanUse(user))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\n❌ {user.Name} cannot use {ability.Name}!");
                Console.ResetColor();
                return;
            }

            // Check for combo
            ComboPattern? combo = null;
            if (comboSystem != null)
            {
                combo = comboSystem.CheckForCombo(ability.Name, user);
            }

            // Consume resources
            ability.ConsumeResource(user);

            // Display ability usage
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"╔═══════════════════════════════════════════╗");
            Console.WriteLine($"║  {ability.Icon} {ability.Name.ToUpper()}");
            Console.WriteLine($"╚═══════════════════════════════════════════╝");
            Console.ResetColor();
            Console.WriteLine($"{user.Name} uses {ability.Name}!");

            // Apply ability effects based on target type
            switch (ability.TargetType)
            {
                case TargetType.SingleEnemy:
                    ApplyAbilityToEnemy(user, ability, target, ref mobHp, combo, comboSystem);
                    break;

                case TargetType.AllEnemies:
                    ApplyAbilityToEnemy(user, ability, target, ref mobHp, combo, comboSystem);
                    Console.WriteLine($"💥 (Would hit all enemies in group combat)");
                    break;

                case TargetType.Self:
                    ApplyAbilityToAlly(user, ability, user);
                    break;

                case TargetType.SingleAlly:
                    var selectedAlly = SelectAllyTarget(party);
                    if (selectedAlly != null)
                    {
                        ApplyAbilityToAlly(user, ability, selectedAlly);
                    }
                    break;

                case TargetType.AllAllies:
                    foreach (var ally in party.Where(p => p.IsAlive))
                    {
                        ApplyAbilityToAlly(user, ability, ally);
                    }
                    break;
            }

            // Advance combo system turn
            if (comboSystem != null)
            {
                comboSystem.AdvanceTurn();
            }

            Console.WriteLine();
        }

        /// <summary>
        /// Apply ability effects to an enemy
        /// </summary>
        public static void ApplyAbilityToEnemy(Character user, CombatAbility ability, Mob target, 
            ref int mobHp, ComboPattern? combo = null, ComboSystem? comboSystem = null)
        {
            if (ability.BaseDamage > 0 || ability.DamageMultiplier > 1.0)
            {
                int damage = ability.CalculateDamage(user);

                // Apply combo bonus if detected
                int baseDamageBeforeCombo = damage;
                if (combo != null && comboSystem != null)
                {
                    damage = comboSystem.ApplyComboBonus(damage, combo);
                    int comboBonus = damage - baseDamageBeforeCombo;

                    // Display combo notification
                    comboSystem.DisplayCombo(combo, user, user, comboBonus);
                }

                // Apply combat stance modifier
                double stanceMultiplier = CombatStanceModifiers.GetDamageMultiplier(user.CurrentStance);
                if (stanceMultiplier != 1.0)
                {
                    int oldDamage = damage;
                    damage = (int)(damage * stanceMultiplier);
                    Console.WriteLine($"  {CombatStanceModifiers.GetStanceIcon(user.CurrentStance)} Stance Bonus: {oldDamage} → {damage}");
                }

                // Apply mob armor
                int finalDamage = Math.Max(1, damage - target.ArmorRating);
                if (finalDamage < damage)
                {
                    Console.WriteLine($"  🛡️  Armor Reduction: {damage} → {finalDamage}");
                }

                mobHp = Math.Max(0, mobHp - finalDamage);

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"  💥 {finalDamage} damage dealt to {target.Name}!");
                Console.ResetColor();
                Console.WriteLine($"  📊 {target.Name} HP: {mobHp}/{target.Health}");

                // Generate threat
                int threat = finalDamage;
                if (user is Warrior) threat = (int)(threat * 1.5);
                user.ThreatLevel += threat;
            }

            // Apply status effect if ability has one
            if (ability.StatusEffect.HasValue)
            {
                var statusEffect = new StatusEffect(
                    ability.StatusEffect.Value,
                    ability.StatusDuration,
                    ability.StatusPotency,
                    user.Name
                );

                Console.ForegroundColor = statusEffect.GetColor();
                Console.WriteLine($"  {statusEffect.GetIcon()} {statusEffect.Type} applied! ({ability.StatusDuration} turns, {ability.StatusPotency} potency)");
                Console.ResetColor();
            }
        }

        /// <summary>
        /// Apply ability effects to an ally
        /// </summary>
        public static void ApplyAbilityToAlly(Character user, CombatAbility ability, Character target)
        {
            if (ability.StatusEffect.HasValue)
            {
                var statusEffect = new StatusEffect(
                    ability.StatusEffect.Value,
                    ability.StatusDuration,
                    ability.StatusPotency,
                    user.Name
                );

                target.AddStatusEffect(statusEffect);

                Console.ForegroundColor = statusEffect.GetColor();
                Console.WriteLine($"  {statusEffect.GetIcon()} {statusEffect.Type} applied to {target.Name}! ({ability.StatusDuration} turns)");
                Console.ResetColor();

                // Generate small amount of threat for support abilities
                user.ThreatLevel += 5;
            }
        }

        /// <summary>
        /// Let player select an ally target
        /// </summary>
        public static Character? SelectAllyTarget(List<Character> party)
        {
            Console.WriteLine("\nSelect target ally:");
            var aliveAllies = party.Where(p => p.IsAlive).ToList();

            for (int i = 0; i < aliveAllies.Count; i++)
            {
                var ally = aliveAllies[i];
                Console.WriteLine($"  {i + 1}. {ally.Name} - HP: {ally.Health}/{ally.MaxHealth}");
            }

            Console.Write("Select (1-{0}): ", aliveAllies.Count);
            string input = Console.ReadLine() ?? "";

            if (int.TryParse(input, out int choice) && choice >= 1 && choice <= aliveAllies.Count)
            {
                return aliveAllies[choice - 1];
            }

            Console.WriteLine("Invalid selection!");
            return null;
        }

        /// <summary>
        /// Use special ability (legacy system)
        /// </summary>
        public static void UseSpecialAgainstMob(Character member, Mob mob, ref int mobHp, 
            List<Character> party, Dictionary<string, bool> specialUsedLastTurn)
        {
            Console.Write($"{member.Name} uses their special ability!");
            int roll = CombatSystem.RollD20();

            // Priest special acts as a heal on allies
            if (member is Priest priest)
            {
                if (specialUsedLastTurn.TryGetValue(member.Name, out bool used) && used)
                {
                    Console.WriteLine($"{member.Name} cannot use special again so soon!");
                    return;
                }

                Console.WriteLine("\nChoose healing action:");
                Console.WriteLine("1) Heal single ally (12 Mana)");
                Console.WriteLine("2) Mass Heal all party members (25 Mana)");
                var healChoice = Console.ReadLine() ?? string.Empty;

                if (healChoice == "1")
                {
                    Console.WriteLine("Choose ally to heal:");
                    for (int i = 0; i < party.Count; i++) 
                        Console.WriteLine($"{i + 1}) {party[i].Name} - HP {party[i].Health}/{party[i].MaxHealth}");
                    
                    var sel = Console.ReadLine() ?? string.Empty;
                    if (!int.TryParse(sel, out var idx) || idx < 1 || idx > party.Count)
                    {
                        Console.WriteLine("Invalid selection.");
                        return;
                    }
                    
                    var target = party[idx - 1];
                    priest.HealAlly(target);
                    priest.ThreatLevel += 10;
                }
                else if (healChoice == "2")
                {
                    priest.SpecialAbility(party);
                    priest.ThreatLevel += 25;
                }
                else
                {
                    Console.WriteLine("Invalid selection.");
                    return;
                }

                specialUsedLastTurn[member.Name] = true;
                return;
            }

            // Offensive special abilities
            int targetDefense = -10 + (mobHp / 5);
            int attackStat = GetAttackStat(member) + 5;

            Console.WriteLine($"{member.Name} rolls d20: {roll} (+{attackStat}) against defense {targetDefense}");
            
            if (specialUsedLastTurn.TryGetValue(member.Name, out bool used2) && used2)
            {
                Console.WriteLine($"{member.Name} cannot use special again so soon!");
                return;
            }
            
            if (roll == 1)
            {
                Console.WriteLine($"{member.Name}'s special ability critically fails!");
                specialUsedLastTurn[member.Name] = true;
                return;
            }

            int total = roll + attackStat;
            bool crit = roll == 20;

            // Check for additional crit chance from skills
            if (!crit && member.SkillTree != null)
            {
                int critChance = member.SkillTree.GetTotalCritChanceBonus();
                if (critChance > 0 && _rng.Next(1, 101) <= critChance)
                {
                    crit = true;
                    Console.WriteLine(" [SKILL CRIT!]");
                }
            }

            if (crit || total > targetDefense)
            {
                int baseDamage = attackStat * 2;
                if (member is Mage) baseDamage = member.GetTotalIntelligence() * 3;
                else if (member is Rogue) baseDamage = member.GetTotalAgility() * 3;
                else if (member is Warrior) baseDamage = member.GetTotalStrength() * 3;

                int dmg = crit ? baseDamage * 2 : baseDamage;

                // Apply skill damage bonus
                if (member.SkillTree != null)
                {
                    dmg += member.SkillTree.GetTotalDamageBonus();
                }

                // Apply life steal
                if (member.SkillTree != null)
                {
                    double lifeStealPercent = member.SkillTree.GetTotalLifeStealPercent();
                    if (lifeStealPercent > 0)
                    {
                        int healAmount = (int)(dmg * lifeStealPercent);
                        if (healAmount > 0)
                        {
                            member.Heal(healAmount);
                            Console.WriteLine($" [Life Steal: +{healAmount} HP]");
                        }
                    }
                }

                // Apply mob armor
                int finalDamage = dmg;
                if (mob.ArmorRating > 0)
                {
                    finalDamage = Math.Max(1, dmg - mob.ArmorRating);
                    if (finalDamage < dmg)
                    {
                        Console.WriteLine($" [Mob AR: {mob.ArmorRating}, {dmg} → {finalDamage}]");
                    }
                }

                mobHp = Math.Max(0, mobHp - finalDamage);
                Console.WriteLine($"{member.Name}'s special hits {mob.Name} for {finalDamage} damage (mob HP now {mobHp}).");

                // Generate threat
                int threat = finalDamage;
                if (member is Warrior) threat = (int)(threat * 1.5);
                member.ThreatLevel += threat;
            }
            else
            {
                Console.WriteLine($"{member.Name}'s special misses {mob.Name}.");
            }

            specialUsedLastTurn[member.Name] = true;
        }

        private static int GetAttackStat(Character c)
        {
            if (c is Mage) return c.GetTotalIntelligence();
            if (c is Rogue) return c.GetTotalAgility();
            if (c is Priest) return Math.Max(1, c.GetTotalIntelligence() / 2);
            return c.GetTotalStrength();
        }
    }
}
