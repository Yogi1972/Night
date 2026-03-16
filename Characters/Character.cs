using Night.Items;
using Rpg_Dungeon;
using System;
using System.Collections.Generic;
using System.Text;

namespace Night.Characters
{
    #region Base Character Class

    internal abstract class Character
    {
        #region Properties
        // Basic character properties
        public string Name { get; set; }
        public int Health { get; protected set; }
        public int MaxHealth { get; protected set; }
        public int Mana { get; protected set; }
        public int MaxMana { get; protected set; }
        public int Stamina { get; protected set; }
        public int MaxStamina { get; protected set; }
        public int Strength { get; protected set; }
        public int Agility { get; protected set; }
        public int Intelligence { get; protected set; }
        public int ArmorRating { get; protected set; }
        // Derived properties
        public bool IsAlive => Health > 0;

        public Inventory Inventory { get; }

        public Pet? Pet { get; set; }

        // Race name for display and persistence
        public string RaceName { get; set; } = "Human";

        public int Level { get; private set; }

        public int Experience { get; private set; }

        public int ExperienceToNextLevel => Playerleveling.GetXPToNextLevel(Level);

        public SkillTree SkillTree { get; set; }

        public int ThreatLevel { get; set; }

        public string CurrentLocation { get; set; }

        public CombatStance CurrentStance { get; set; }

        public string? ChampionClass { get; set; }

        public bool HasChampionClass => !string.IsNullOrEmpty(ChampionClass);

        public CraftingProfession PrimaryProfession { get; set; }

        public CraftingProfession SecondaryProfession { get; set; }

        public List<CombatAbility> Abilities { get; set; }

        public List<StatusEffect> ActiveStatusEffects { get; set; }

        #endregion

        #region Constructor
        // Protected constructor to prevent direct instantiation of Character class
        protected Character(string name)
        {
            Name = name;
            Inventory = new Inventory();
            Level = 1;
            Experience = 0;
            ThreatLevel = 0;
            CurrentLocation = "Town";
            CurrentStance = CombatStance.Balanced;
            SkillTree = SkillTreeFactory.CreateSkillTreeForCharacter(this);
            PrimaryProfession = CraftingProfession.None;
            SecondaryProfession = CraftingProfession.None;
            Abilities = new List<CombatAbility>();
            ActiveStatusEffects = new List<StatusEffect>();
        }

        #endregion

        #region Stats and Progression
        // Method to apply racial bonuses to the character's base stats (called during character creation)
        public void ApplyRaceBonuses(Race race)
        {
            MaxHealth += race.HealthBonus;
            Health += race.HealthBonus;
            MaxMana += race.ManaBonus;
            Mana += race.ManaBonus;
            MaxStamina += race.StaminaBonus;
            Stamina += race.StaminaBonus;
            Strength += race.StrengthBonus;
            Agility += race.AgilityBonus;
            Intelligence += race.IntelligenceBonus;
            ArmorRating += race.ArmorBonus;
        }
        // Method to restore character progress (used when loading a saved game)
        public void RestoreProgress(int level, int experience, int health, int maxHealth, int mana, int maxMana, int stamina, int maxStamina, int str, int agi, int intel, int ar = 0)
        {
            Level = Math.Max(1, level);
            Experience = Math.Max(0, experience);
            MaxHealth = Math.Max(1, maxHealth);
            Health = Math.Min(MaxHealth, health);
            MaxMana = Math.Max(0, maxMana);
            Mana = Math.Min(MaxMana, mana);
            MaxStamina = Math.Max(0, maxStamina);
            Stamina = Math.Min(MaxStamina, stamina);
            Strength = str;
            Agility = agi;
            Intelligence = intel;
            ArmorRating = Math.Max(0, ar);
        }

        #endregion

        #region Stat Calculations

        public int GetTotalStrength()// Calculate total strength including equipment and skill bonuses
        {
            int total = Strength;
            if (Inventory.EquippedWeapon != null) total += Inventory.EquippedWeapon.StrengthBonus;
            if (Inventory.EquippedArmor != null) total += Inventory.EquippedArmor.StrengthBonus;
            if (Inventory.EquippedAccessory != null) total += Inventory.EquippedAccessory.StrengthBonus;
            if (Inventory.EquippedNecklace != null) total += Inventory.EquippedNecklace.StrengthBonus;
            if (Inventory.EquippedRing1 != null) total += Inventory.EquippedRing1.StrengthBonus;
            if (Inventory.EquippedRing2 != null) total += Inventory.EquippedRing2.StrengthBonus;
            if (Inventory.EquippedOffHand != null) total += Inventory.EquippedOffHand.StrengthBonus;
            if (SkillTree != null) total += SkillTree.GetTotalStrengthBonus();
            return total;
        }

        public int GetTotalAgility()// Calculate total agility including equipment and skill bonuses
        {
            int total = Agility;
            if (Inventory.EquippedWeapon != null) total += Inventory.EquippedWeapon.AgilityBonus;
            if (Inventory.EquippedArmor != null) total += Inventory.EquippedArmor.AgilityBonus;
            if (Inventory.EquippedAccessory != null) total += Inventory.EquippedAccessory.AgilityBonus;
            if (Inventory.EquippedNecklace != null) total += Inventory.EquippedNecklace.AgilityBonus;
            if (Inventory.EquippedRing1 != null) total += Inventory.EquippedRing1.AgilityBonus;
            if (Inventory.EquippedRing2 != null) total += Inventory.EquippedRing2.AgilityBonus;
            if (Inventory.EquippedOffHand != null) total += Inventory.EquippedOffHand.AgilityBonus;
            if (SkillTree != null) total += SkillTree.GetTotalAgilityBonus();
            return total;
        }

        public int GetTotalIntelligence()// Calculate total intelligence including equipment and skill bonuses
        {
            int total = Intelligence;
            if (Inventory.EquippedWeapon != null) total += Inventory.EquippedWeapon.IntelligenceBonus;
            if (Inventory.EquippedArmor != null) total += Inventory.EquippedArmor.IntelligenceBonus;
            if (Inventory.EquippedAccessory != null) total += Inventory.EquippedAccessory.IntelligenceBonus;
            if (Inventory.EquippedNecklace != null) total += Inventory.EquippedNecklace.IntelligenceBonus;
            if (Inventory.EquippedRing1 != null) total += Inventory.EquippedRing1.IntelligenceBonus;
            if (Inventory.EquippedRing2 != null) total += Inventory.EquippedRing2.IntelligenceBonus;
            if (Inventory.EquippedOffHand != null) total += Inventory.EquippedOffHand.IntelligenceBonus;
            if (SkillTree != null) total += SkillTree.GetTotalIntelligenceBonus();
            return total;
        }

        public int GetTotalMaxHP()// Calculate total max health including equipment and skill bonuses
        {
            int total = MaxHealth;
            if (Inventory.EquippedWeapon != null) total += Inventory.EquippedWeapon.MaxHPBonus;
            if (Inventory.EquippedArmor != null) total += Inventory.EquippedArmor.MaxHPBonus;
            if (Inventory.EquippedAccessory != null) total += Inventory.EquippedAccessory.MaxHPBonus;
            if (Inventory.EquippedNecklace != null) total += Inventory.EquippedNecklace.MaxHPBonus;
            if (Inventory.EquippedRing1 != null) total += Inventory.EquippedRing1.MaxHPBonus;
            if (Inventory.EquippedRing2 != null) total += Inventory.EquippedRing2.MaxHPBonus;
            if (Inventory.EquippedOffHand != null) total += Inventory.EquippedOffHand.MaxHPBonus;
            if (SkillTree != null) total += SkillTree.GetTotalMaxHPBonus();
            return total;
        }

        public int GetTotalMaxMana()// Calculate total max mana including equipment and skill bonuses
        {
            int total = MaxMana;
            if (Inventory.EquippedWeapon != null) total += Inventory.EquippedWeapon.MaxManaBonus;
            if (Inventory.EquippedArmor != null) total += Inventory.EquippedArmor.MaxManaBonus;
            if (Inventory.EquippedAccessory != null) total += Inventory.EquippedAccessory.MaxManaBonus;
            if (Inventory.EquippedNecklace != null) total += Inventory.EquippedNecklace.MaxManaBonus;
            if (Inventory.EquippedRing1 != null) total += Inventory.EquippedRing1.MaxManaBonus;
            if (Inventory.EquippedRing2 != null) total += Inventory.EquippedRing2.MaxManaBonus;
            if (Inventory.EquippedOffHand != null) total += Inventory.EquippedOffHand.MaxManaBonus;
            if (SkillTree != null) total += SkillTree.GetTotalMaxManaBonus();
            return total;
        }

        public int GetTotalMaxStamina()// Calculate total max stamina including equipment and skill bonuses
        {
            int total = MaxStamina;
            if (Inventory.EquippedWeapon != null) total += Inventory.EquippedWeapon.MaxStaminaBonus;
            if (Inventory.EquippedArmor != null) total += Inventory.EquippedArmor.MaxStaminaBonus;
            if (Inventory.EquippedAccessory != null) total += Inventory.EquippedAccessory.MaxStaminaBonus;
            if (Inventory.EquippedNecklace != null) total += Inventory.EquippedNecklace.MaxStaminaBonus;
            if (Inventory.EquippedRing1 != null) total += Inventory.EquippedRing1.MaxStaminaBonus;
            if (Inventory.EquippedRing2 != null) total += Inventory.EquippedRing2.MaxStaminaBonus;
            if (Inventory.EquippedOffHand != null) total += Inventory.EquippedOffHand.MaxStaminaBonus;
            if (SkillTree != null) total += SkillTree.GetTotalMaxStaminaBonus();
            return total;
        }

        public int GetTotalArmorRating()// Calculate total armor rating including equipment and skill bonuses, and apply combat stance modifiers
        {
            int total = ArmorRating;
            if (Inventory.EquippedWeapon != null) total += Inventory.EquippedWeapon.ArmorBonus;
            if (Inventory.EquippedArmor != null) total += Inventory.EquippedArmor.ArmorBonus;
            if (Inventory.EquippedAccessory != null) total += Inventory.EquippedAccessory.ArmorBonus;
            if (Inventory.EquippedNecklace != null) total += Inventory.EquippedNecklace.ArmorBonus;
            if (Inventory.EquippedRing1 != null) total += Inventory.EquippedRing1.ArmorBonus;
            if (Inventory.EquippedRing2 != null) total += Inventory.EquippedRing2.ArmorBonus;
            if (Inventory.EquippedOffHand != null) total += Inventory.EquippedOffHand.ArmorBonus;
            if (SkillTree != null) total += SkillTree.GetTotalArmorBonus();

            double stanceMultiplier = CombatStanceModifiers.GetArmorMultiplier(CurrentStance);
            total = (int)(total * stanceMultiplier);

            return total;
        }

        #endregion

        #region Health, Mana, and Stamina Management

        public virtual void ReceiveDamage(int amount)
        {
            if (amount <= 0) return;

            // Apply status effect modifiers
            double statusMultiplier = StatusEffectManager.GetDamageTakenModifier(this);// e.g. Vulnerable might increase damage taken by 25% (1.25 multiplier)
            if (statusMultiplier != 1.0)// Only display if there's a change
            {
                int modifiedAmount = (int)(amount * statusMultiplier);// Round down to nearest whole number
                if (modifiedAmount != amount)// Only display if the modified amount is different
                {
                    Console.Write($" [Vulnerable: {amount} → {modifiedAmount}]");// Show original and modified damage
                    amount = modifiedAmount;// Use modified damage for further calculations
                }
            }

            int totalAR = GetTotalArmorRating();
            int reducedDamage = Math.Max(1, amount - totalAR);

            if (totalAR > 0 && reducedDamage < amount)
            {
                Console.Write($" [AR: {totalAR}, Reduced: {amount} → {reducedDamage}]");
            }

            Health = Math.Max(0, Health - reducedDamage);
        }

        public virtual void Heal(int amount)
        {
            if (amount <= 0) return;
            Health = Math.Min(MaxHealth, Health + amount);
        }

        public virtual void RestoreMana(int amount)
        {
            if (amount <= 0) return;
            Mana = Math.Min(MaxMana, Mana + amount);
        }

        public virtual void RestoreStamina(int amount)
        {
            if (amount <= 0) return;
            Stamina = Math.Min(MaxStamina, Stamina + amount);
        }

        public void ModifyMana(int amount)
        {
            if (amount > 0)
            {
                RestoreMana(amount);
            }
            else
            {
                Mana = Math.Max(0, Mana + amount);
            }
        }

        public void ModifyStamina(int amount)
        {
            if (amount > 0)
            {
                RestoreStamina(amount);
            }
            else
            {
                Stamina = Math.Max(0, Stamina + amount);
            }
        }

        public int GetEffectiveManaCost(int baseCost)
        {
            if (SkillTree == null) return baseCost;

            double efficiency = SkillTree.GetTotalManaEfficiencyPercent();
            if (efficiency > 0)
            {
                int reducedCost = (int)(baseCost * (1.0 - efficiency));
                return Math.Max(1, reducedCost);
            }

            return baseCost;
        }

        #endregion

        #region Combat

        public void ChangeStance(CombatStance newStance)
        {
            if (CurrentStance == newStance)
            {
                Console.WriteLine($"{Name} is already in {newStance} stance.");
                return;
            }

            CurrentStance = newStance;
            Console.ForegroundColor = CombatStanceModifiers.GetStanceColor(newStance);
            Console.WriteLine($"\n{CombatStanceModifiers.GetStanceIcon(newStance)} {Name} switches to {CombatStanceModifiers.GetStanceDescription(newStance)}");
            Console.ResetColor();
        }

        public abstract void Attack(Character target);

        public abstract void SpecialAbility(Character target);

        #endregion

        #region Experience and Leveling

        public void GainExperience(int amount)
        {
            if (amount <= 0) return;
            if (Level >= Playerleveling.MaxLevel)
            {
                Console.WriteLine($"{Name} is already at max level ({Playerleveling.MaxLevel})!");
                return;
            }

            Experience += amount;
            Console.WriteLine($"{Name} gains {amount} XP ({Experience}/{ExperienceToNextLevel}).");

            while (Experience >= ExperienceToNextLevel && Level < Playerleveling.MaxLevel)
            {
                Experience -= ExperienceToNextLevel;
                LevelUp();
            }
        }

        private void LevelUp()
        {
            Level++;

            var gains = Playerleveling.GetStatGainsForLevel(this, Level);

            MaxHealth += gains.Health;
            Health = MaxHealth;
            MaxMana += gains.Mana;
            Mana = MaxMana;
            MaxStamina += gains.Stamina;
            Stamina = MaxStamina;
            Strength += gains.Strength;
            Agility += gains.Agility;
            Intelligence += gains.Intelligence;
            ArmorRating += gains.ArmorRating;

            string title = Playerleveling.GetLevelTitle(Level);

            VisualEffects.ShowLevelUpAnimation();

            VisualEffects.WriteLineColored($"✨ {Name} reached Level {Level}! ({title}) ✨", ConsoleColor.Cyan);
            Console.WriteLine();

            VisualEffects.WriteSuccess($"📈 Stat Gains: {gains}\n");
            VisualEffects.WriteHealing("💚 Health, Mana, and Stamina fully restored!\n");

            if (SkillTree != null)
            {
                SkillTree.AddSkillPoint();
                VisualEffects.WriteLineColored($"⭐ Earned 1 Skill Point! Total: {SkillTree.SkillPoints}", ConsoleColor.Yellow);
            }

            if (MilestoneRewards.IsMilestone(Level))
            {
                MilestoneRewards.AwardMilestoneReward(this);
            }

            Console.WriteLine();
        }

        #endregion

        #region Combat Abilities

        /// <summary>
        /// Initialize abilities for this character based on their class
        /// </summary>
        public void InitializeAbilities()
        {
            Abilities = AbilityFactory.GetAbilitiesForCharacter(this);
        }

        /// <summary>
        /// Use a combat ability
        /// </summary>
        public bool UseAbility(CombatAbility ability, Character? target = null)
        {
            if (!ability.CanUse(this))
            {
                Console.WriteLine($"{Name} cannot use {ability.Name}!");
                return false;
            }

            ability.ConsumeResource(this);
            return true;
        }

        /// <summary>
        /// Reduce cooldowns on all abilities (call at end of turn)
        /// </summary>
        public void ReduceAbilityCooldowns()
        {
            foreach (var ability in Abilities)
            {
                ability.ReduceCooldown();
            }
        }

        /// <summary>
        /// Add a status effect to this character
        /// </summary>
        public void AddStatusEffect(StatusEffect effect)
        {
            // Check if same effect type already exists
            var existing = ActiveStatusEffects.Find(e => e.Type == effect.Type);
            if (existing != null)
            {
                // Replace if new effect is stronger
                if (effect.Potency >= existing.Potency)
                {
                    ActiveStatusEffects.Remove(existing);
                    ActiveStatusEffects.Add(effect);
                }
            }
            else
            {
                ActiveStatusEffects.Add(effect);
            }
        }

        /// <summary>
        /// Process all active status effects (call at end of turn)
        /// </summary>
        public void ProcessStatusEffects()
        {
            if (ActiveStatusEffects.Count == 0) return;

            Console.WriteLine($"\n{Name}'s status effects:");
            for (int i = ActiveStatusEffects.Count - 1; i >= 0; i--)
            {
                var effect = ActiveStatusEffects[i];
                effect.ApplyEffect(this);

                if (effect.IsExpired)
                {
                    ActiveStatusEffects.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// Check if character has a specific status effect
        /// </summary>
        public bool HasStatusEffect(StatusEffectType type)
        {
            return ActiveStatusEffects.Exists(e => e.Type == type);
        }

        /// <summary>
        /// Remove all status effects
        /// </summary>
        public void ClearStatusEffects()
        {
            ActiveStatusEffects.Clear();
        }

        #endregion
    }

    #endregion
}
