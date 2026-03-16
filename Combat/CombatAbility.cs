using Night.Characters;
using System;
using System.Collections.Generic;

namespace Rpg_Dungeon
{
    /// <summary>
    /// Resource types that abilities can consume
    /// </summary>
    internal enum ResourceType
    {
        Mana,
        Stamina,
        Health,
        None
    }

    /// <summary>
    /// Ability targeting types
    /// </summary>
    internal enum TargetType
    {
        SingleEnemy,
        AllEnemies,
        SingleAlly,
        AllAllies,
        Self
    }

    /// <summary>
    /// Represents a combat ability that characters can use
    /// </summary>
    internal class CombatAbility
    {
        public string Name { get; }
        public string Description { get; }
        public ResourceType ResourceType { get; }
        public int ResourceCost { get; }
        public int Cooldown { get; }
        public int CurrentCooldown { get; set; }
        public TargetType TargetType { get; }
        public int BaseDamage { get; }
        public StatusEffectType? StatusEffect { get; }
        public int StatusDuration { get; }
        public int StatusPotency { get; }
        public double DamageMultiplier { get; }
        public string Icon { get; }

        public CombatAbility(
            string name,
            string description,
            ResourceType resourceType,
            int resourceCost,
            int cooldown,
            TargetType targetType,
            int baseDamage = 0,
            double damageMultiplier = 1.0,
            StatusEffectType? statusEffect = null,
            int statusDuration = 0,
            int statusPotency = 0,
            string icon = "⚔️")
        {
            Name = name;
            Description = description;
            ResourceType = resourceType;
            ResourceCost = resourceCost;
            Cooldown = cooldown;
            CurrentCooldown = 0;
            TargetType = targetType;
            BaseDamage = baseDamage;
            DamageMultiplier = damageMultiplier;
            StatusEffect = statusEffect;
            StatusDuration = statusDuration;
            StatusPotency = statusPotency;
            Icon = icon;
        }

        /// <summary>
        /// Check if the ability can be used by the character
        /// </summary>
        public bool CanUse(Character user)
        {
            if (CurrentCooldown > 0)
                return false;

            return ResourceType switch
            {
                ResourceType.Mana => user.Mana >= user.GetEffectiveManaCost(ResourceCost),
                ResourceType.Stamina => user.Stamina >= ResourceCost,
                ResourceType.Health => user.Health > ResourceCost,
                ResourceType.None => true,
                _ => false
            };
        }

        /// <summary>
        /// Consume the resource cost
        /// </summary>
        public void ConsumeResource(Character user)
        {
            switch (ResourceType)
            {
                case ResourceType.Mana:
                    int effectiveManaCost = user.GetEffectiveManaCost(ResourceCost);
                    if (effectiveManaCost < ResourceCost)
                    {
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.Write($" [📖 Mana Reduced: {ResourceCost} → {effectiveManaCost}]");
                        Console.ResetColor();
                    }
                    user.ModifyMana(-effectiveManaCost);
                    break;
                case ResourceType.Stamina:
                    user.ModifyStamina(-ResourceCost);
                    break;
                case ResourceType.Health:
                    user.ReceiveDamage(ResourceCost);
                    break;
            }

            CurrentCooldown = Cooldown;
        }

        /// <summary>
        /// Calculate the damage this ability will deal
        /// </summary>
        public int CalculateDamage(Character user)
        {
            int statBonus = ResourceType switch
            {
                ResourceType.Mana => user.Intelligence,
                ResourceType.Stamina => user.GetTotalStrength(),
                _ => user.GetTotalStrength()
            };

            return (int)((BaseDamage + statBonus) * DamageMultiplier);
        }

        /// <summary>
        /// Reduce cooldown by 1 (call each turn)
        /// </summary>
        public void ReduceCooldown()
        {
            if (CurrentCooldown > 0)
                CurrentCooldown--;
        }

        /// <summary>
        /// Get a display string for the ability
        /// </summary>
        public string GetDisplayString()
        {
            string cooldownText = CurrentCooldown > 0 ? $" (CD: {CurrentCooldown})" : "";
            string costText = ResourceCost > 0 ? $" [{ResourceCost} {ResourceType}]" : "";
            return $"{Icon} {Name}{costText}{cooldownText}";
        }

        public override string ToString()
        {
            return GetDisplayString();
        }
    }

    /// <summary>
    /// Factory for creating class-specific abilities
    /// </summary>
    internal static class AbilityFactory
    {
        /// <summary>
        /// Get abilities for Warrior class
        /// </summary>
        public static List<CombatAbility> GetWarriorAbilities()
        {
            return new List<CombatAbility>
            {
                new CombatAbility(
                    name: "Power Strike",
                    description: "A devastating melee attack dealing 150% damage",
                    resourceType: ResourceType.Stamina,
                    resourceCost: 15,
                    cooldown: 2,
                    targetType: TargetType.SingleEnemy,
                    baseDamage: 10,
                    damageMultiplier: 1.5,
                    icon: "💥"
                ),
                new CombatAbility(
                    name: "Defensive Stance",
                    description: "Take a defensive stance, reducing damage taken",
                    resourceType: ResourceType.Stamina,
                    resourceCost: 10,
                    cooldown: 4,
                    targetType: TargetType.Self,
                    statusEffect: StatusEffectType.Regenerating,
                    statusDuration: 3,
                    statusPotency: 5,
                    icon: "🛡️"
                ),
                new CombatAbility(
                    name: "Whirlwind Attack",
                    description: "Attack all enemies for moderate damage",
                    resourceType: ResourceType.Stamina,
                    resourceCost: 25,
                    cooldown: 5,
                    targetType: TargetType.AllEnemies,
                    baseDamage: 8,
                    damageMultiplier: 0.8,
                    icon: "🌪️"
                ),
                new CombatAbility(
                    name: "Intimidating Shout",
                    description: "Weaken all enemies",
                    resourceType: ResourceType.Stamina,
                    resourceCost: 20,
                    cooldown: 6,
                    targetType: TargetType.AllEnemies,
                    statusEffect: StatusEffectType.Weakened,
                    statusDuration: 3,
                    statusPotency: 30,
                    icon: "😠"
                )
            };
        }

        /// <summary>
        /// Get abilities for Mage class
        /// </summary>
        public static List<CombatAbility> GetMageAbilities()
        {
            return new List<CombatAbility>
            {
                new CombatAbility(
                    name: "Fireball",
                    description: "Blast an enemy with fire, causing burn",
                    resourceType: ResourceType.Mana,
                    resourceCost: 20,
                    cooldown: 2,
                    targetType: TargetType.SingleEnemy,
                    baseDamage: 15,
                    damageMultiplier: 1.8,
                    statusEffect: StatusEffectType.Burning,
                    statusDuration: 3,
                    statusPotency: 5,
                    icon: "🔥"
                ),
                new CombatAbility(
                    name: "Ice Bolt",
                    description: "Strike with ice, freezing the target",
                    resourceType: ResourceType.Mana,
                    resourceCost: 18,
                    cooldown: 2,
                    targetType: TargetType.SingleEnemy,
                    baseDamage: 12,
                    damageMultiplier: 1.6,
                    statusEffect: StatusEffectType.Frozen,
                    statusDuration: 2,
                    statusPotency: 0,
                    icon: "❄️"
                ),
                new CombatAbility(
                    name: "Mana Shield",
                    description: "Create a magical barrier that regenerates health",
                    resourceType: ResourceType.Mana,
                    resourceCost: 25,
                    cooldown: 6,
                    targetType: TargetType.Self,
                    statusEffect: StatusEffectType.Regenerating,
                    statusDuration: 4,
                    statusPotency: 8,
                    icon: "✨"
                ),
                new CombatAbility(
                    name: "Lightning Storm",
                    description: "Call down lightning on all enemies",
                    resourceType: ResourceType.Mana,
                    resourceCost: 40,
                    cooldown: 7,
                    targetType: TargetType.AllEnemies,
                    baseDamage: 18,
                    damageMultiplier: 1.2,
                    statusEffect: StatusEffectType.Stunned,
                    statusDuration: 1,
                    statusPotency: 0,
                    icon: "⚡"
                )
            };
        }

        /// <summary>
        /// Get abilities for Rogue class
        /// </summary>
        public static List<CombatAbility> GetRogueAbilities()
        {
            return new List<CombatAbility>
            {
                new CombatAbility(
                    name: "Backstab",
                    description: "Strike from the shadows for massive damage",
                    resourceType: ResourceType.Stamina,
                    resourceCost: 18,
                    cooldown: 3,
                    targetType: TargetType.SingleEnemy,
                    baseDamage: 12,
                    damageMultiplier: 2.0,
                    statusEffect: StatusEffectType.Bleeding,
                    statusDuration: 4,
                    statusPotency: 4,
                    icon: "🗡️"
                ),
                new CombatAbility(
                    name: "Poison Blade",
                    description: "Coat your weapon in deadly poison",
                    resourceType: ResourceType.Stamina,
                    resourceCost: 15,
                    cooldown: 4,
                    targetType: TargetType.SingleEnemy,
                    baseDamage: 8,
                    damageMultiplier: 1.3,
                    statusEffect: StatusEffectType.Poisoned,
                    statusDuration: 5,
                    statusPotency: 6,
                    icon: "☠️"
                ),
                new CombatAbility(
                    name: "Shadow Step",
                    description: "Become elusive, avoiding damage",
                    resourceType: ResourceType.Stamina,
                    resourceCost: 20,
                    cooldown: 5,
                    targetType: TargetType.Self,
                    statusEffect: StatusEffectType.Regenerating,
                    statusDuration: 2,
                    statusPotency: 10,
                    icon: "👤"
                ),
                new CombatAbility(
                    name: "Fan of Knives",
                    description: "Throw knives at all enemies",
                    resourceType: ResourceType.Stamina,
                    resourceCost: 25,
                    cooldown: 6,
                    targetType: TargetType.AllEnemies,
                    baseDamage: 10,
                    damageMultiplier: 0.9,
                    icon: "🔪"
                )
            };
        }

        /// <summary>
        /// Get abilities for Priest class
        /// </summary>
        public static List<CombatAbility> GetPriestAbilities()
        {
            return new List<CombatAbility>
            {
                new CombatAbility(
                    name: "Holy Smite",
                    description: "Strike with divine power",
                    resourceType: ResourceType.Mana,
                    resourceCost: 15,
                    cooldown: 2,
                    targetType: TargetType.SingleEnemy,
                    baseDamage: 10,
                    damageMultiplier: 1.4,
                    icon: "✝️"
                ),
                new CombatAbility(
                    name: "Divine Shield",
                    description: "Protect an ally with holy magic",
                    resourceType: ResourceType.Mana,
                    resourceCost: 20,
                    cooldown: 5,
                    targetType: TargetType.SingleAlly,
                    statusEffect: StatusEffectType.Regenerating,
                    statusDuration: 4,
                    statusPotency: 7,
                    icon: "🛡️"
                ),
                new CombatAbility(
                    name: "Healing Prayer",
                    description: "Restore health to all allies",
                    resourceType: ResourceType.Mana,
                    resourceCost: 30,
                    cooldown: 4,
                    targetType: TargetType.AllAllies,
                    statusEffect: StatusEffectType.Regenerating,
                    statusDuration: 3,
                    statusPotency: 12,
                    icon: "🙏"
                ),
                new CombatAbility(
                    name: "Wrath",
                    description: "Channel divine fury to damage all enemies",
                    resourceType: ResourceType.Mana,
                    resourceCost: 35,
                    cooldown: 7,
                    targetType: TargetType.AllEnemies,
                    baseDamage: 14,
                    damageMultiplier: 1.3,
                    statusEffect: StatusEffectType.Vulnerable,
                    statusDuration: 2,
                    statusPotency: 30,
                    icon: "⚡"
                )
            };
        }

        /// <summary>
        /// Get abilities based on character class
        /// </summary>
        public static List<CombatAbility> GetAbilitiesForCharacter(Character character)
        {
            return character switch
            {
                Warrior => GetWarriorAbilities(),
                Mage => GetMageAbilities(),
                Rogue => GetRogueAbilities(),
                Priest => GetPriestAbilities(),
                _ => new List<CombatAbility>() // Default empty list
            };
        }
    }
}
