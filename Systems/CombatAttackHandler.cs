using Night.Characters;
using Night.Combat;
using System;

namespace Rpg_Dungeon
{
    /// <summary>
    /// Handles all attack logic between characters and mobs
    /// </summary>
    internal static class CombatAttackHandler
    {
        private static readonly Random _rng = new Random();

        #region Character Attacks Mob

        /// <summary>
        /// Character attacks a mob using a d20 roll
        /// </summary>
        public static void Attack(Character attacker, Mob target)
        {
            if (attacker == null || target == null) return;
            if (!attacker.IsAlive) return;

            int roll = CombatSystem.RollD20();
            int attackStat = GetAttackStat(attacker);
            int targetDefense = 10 + (target.Agility / 2);

            Console.Write($"{attacker.Name} rolls d20: {roll} (+{attackStat} attack)");

            if (roll == 1)
            {
                Console.Write("Critical miss!");
                return;
            }

            int total = roll + attackStat;
            bool isCrit = roll == 20;
            
            if (isCrit || total > targetDefense)
            {
                int baseDamage = attackStat;
                if (attacker is Mage) baseDamage = attacker.Intelligence;
                else if (attacker is Rogue) baseDamage = attacker.Agility;
                else if (attacker is Priest) baseDamage = Math.Max(1, attacker.Intelligence / 2);

                int damage = isCrit ? baseDamage * 2 : baseDamage;
                Console.Write($"Hit! {attacker.Name} deals {damage} damage to {target.Name}.");
            }
            else
            {
                Console.Write($"{attacker.Name} misses {target.Name}.");
            }
        }

        #endregion

        #region Mob Attacks Character

        /// <summary>
        /// Mob attacks a character using d20
        /// </summary>
        public static void Attack(Mob attacker, Character target)
        {
            if (attacker == null || target == null) return;
            if (!target.IsAlive) return;

            int roll = CombatSystem.RollD20();
            int attackStat = attacker.Strength;

            // Apply AI speed bonus to effective agility for hit calculation
            int effectiveAgility = attacker.Agility;
            if (attacker.AI != null)
            {
                effectiveAgility += attacker.AI.GetSpeedBonus();
            }

            int targetDefense = 10 + (target.GetTotalAgility() / 2);

            Console.Write($"{attacker.Name} rolls d20: {roll} (+{attackStat} attack)");

            if (roll == 1)
            {
                Console.Write("Mob critically misses!");
                return;
            }

            int total = roll + attackStat;
            bool isCrit = roll == 20;
            
            if (isCrit || total > targetDefense)
            {
                // Check for dodge
                if (target.SkillTree != null)
                {
                    int dodgeChance = target.SkillTree.GetTotalDodgeChanceBonus();
                    if (dodgeChance > 0 && _rng.Next(1, 101) <= dodgeChance)
                    {
                        Console.Write($"{target.Name} dodges the attack! [DODGE]");
                        return;
                    }
                }

                int damage = isCrit ? attacker.Strength * 2 : attacker.Strength;

                // Apply AI damage modifier
                if (attacker.AI != null)
                {
                    double aiModifier = attacker.AI.GetDamageModifier();
                    if (aiModifier != 1.0)
                    {
                        int oldDamage = damage;
                        damage = (int)(damage * aiModifier);

                        if (attacker.AI.IsBerserkActive)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write($" [BERSERK: {oldDamage} → {damage}]");
                            Console.ResetColor();
                        }
                        else if (aiModifier > 1.0)
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.Write($" [{attacker.AI.BehaviorIcon}: {oldDamage} → {damage}]");
                            Console.ResetColor();
                        }
                    }
                }

                // Apply defense reduction from skills
                if (target.SkillTree != null)
                {
                    int defenseBonus = target.SkillTree.GetTotalDefenseBonus();
                    if (defenseBonus > 0)
                    {
                        int reduction = Math.Min(damage - 1, defenseBonus);
                        damage -= reduction;
                        Console.Write($" [Defense: -{reduction}]");
                    }
                }

                Console.Write($"{attacker.Name} hits! Deals {damage} damage to {target.Name}.");
                target.ReceiveDamage(damage);

                // Damage equipment when character takes damage
                int durabilityLoss = _rng.Next(1, 4);
                target.Inventory.DamageEquipment(durabilityLoss);
            }
            else
            {
                Console.Write($"{attacker.Name} misses {target.Name}.");
            }
        }

        #endregion

        #region Basic Attack (for combat encounters)

        /// <summary>
        /// Perform a basic attack with full damage calculation
        /// </summary>
        public static void PerformBasicAttack(Character member, Mob mob, ref int mobHp)
        {
            int roll = CombatSystem.RollD20();
            int attackStat = GetAttackStat(member);
            int targetDefense = 10 + (mob.Agility / 2);

            Console.Write($"{member.Name} rolls d20: {roll} (+{attackStat}) against defense {targetDefense}");

            if (roll == 1)
            {
                VisualEffects.WriteDanger($"\n❌ {member.Name} critically misses! {VisualEffects.GetRandomMissMessage()}\n");
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
                    VisualEffects.WriteColored(" [SKILL CRIT!]", ConsoleColor.Yellow);
                }
            }

            if (crit || total > targetDefense)
            {
                if (crit)
                {
                    VisualEffects.ShowCriticalHitEffect();
                    Console.WriteLine($" {VisualEffects.GetRandomCritMessage()}");
                }

                int baseDamage = attackStat;
                if (member is Mage) baseDamage = member.GetTotalIntelligence();
                else if (member is Rogue) baseDamage = member.GetTotalAgility();
                else if (member is Priest) baseDamage = Math.Max(1, member.GetTotalIntelligence() / 2);
                else if (member is Warrior) baseDamage = member.GetTotalStrength();

                int dmg = crit ? baseDamage * 2 : baseDamage;

                // Apply combat stance damage modifier
                double stanceMultiplier = CombatStanceModifiers.GetDamageMultiplier(member.CurrentStance);
                if (stanceMultiplier != 1.0)
                {
                    int stanceDmg = (int)(dmg * stanceMultiplier);
                    if (stanceDmg != dmg)
                    {
                        Console.Write($" [{CombatStanceModifiers.GetStanceIcon(member.CurrentStance)} Stance: {dmg} → {stanceDmg}]");
                        dmg = stanceDmg;
                    }
                }

                // Apply status effect damage modifiers
                double statusMultiplier = StatusEffectManager.GetDamageModifier(member);
                if (statusMultiplier != 1.0)
                {
                    int statusDmg = (int)(dmg * statusMultiplier);
                    if (statusDmg != dmg)
                    {
                        Console.Write($" [Weakened: {dmg} → {statusDmg}]");
                        dmg = statusDmg;
                    }
                }

                // Apply skill damage bonus
                if (member.SkillTree != null)
                {
                    dmg += member.SkillTree.GetTotalDamageBonus();
                }

                // Apply pet damage boost
                if (member.Pet != null && member.Pet.Ability == PetAbility.DamageBoost)
                {
                    dmg = (int)(dmg * 1.05);
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
                            Console.Write($" [Life Steal: +{healAmount} HP]");
                        }
                    }
                }

                // Apply mob armor rating reduction
                int finalDamage = dmg;
                if (mob.ArmorRating > 0)
                {
                    finalDamage = Math.Max(1, dmg - mob.ArmorRating);
                    if (finalDamage < dmg)
                    {
                        Console.Write($" [Mob AR: {mob.ArmorRating}, {dmg} → {finalDamage}]");
                    }
                }

                mobHp = Math.Max(0, mobHp - finalDamage);
                VisualEffects.WriteDamage($"{member.Name} hits {mob.Name} for {finalDamage} damage! ");
                VisualEffects.WriteInfo($"(mob HP: {mobHp}/{mob.Health})\n");

                // Generate threat
                int threat = finalDamage;
                if (member is Warrior) threat = (int)(threat * 1.5);
                member.ThreatLevel += threat;
            }
            else
            {
                VisualEffects.WriteInfo($"{member.Name} misses {mob.Name}. {VisualEffects.GetRandomMissMessage()}\n");
            }
        }

        #endregion

        #region Helper Methods

        private static int GetAttackStat(Character c)
        {
            if (c is Mage) return c.GetTotalIntelligence();
            if (c is Rogue) return c.GetTotalAgility();
            if (c is Priest) return Math.Max(1, c.GetTotalIntelligence() / 2);
            return c.GetTotalStrength(); // Warrior and default
        }

        #endregion
    }
}
