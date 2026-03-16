using Night.Characters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rpg_Dungeon
{
    // Skill types determine how skills function
    internal enum SkillType
    {
        Passive,    // Always active, provides permanent bonuses
        Active,     // Usable in combat or exploration
        Ultimate    // Powerful abilities with long cooldowns
    }

    // Skill categories for organization
    internal enum SkillCategory
    {
        Offense,    // Damage dealing
        Defense,    // Damage reduction, HP bonuses
        Utility,    // Mana, resource management, exploration
        Support     // Healing, buffs for party
    }

    // Individual skill definition
    internal class Skill
    {
        public string Name { get; }
        public string Description { get; }
        public SkillType Type { get; }
        public SkillCategory Category { get; }
        public int MaxRank { get; }
        public int CurrentRank { get; private set; }
        public int RequiredLevel { get; }
        public string? PrerequisiteSkill { get; }
        public int PrerequisiteRank { get; }

        // Stat bonuses (per rank)
        public int StrengthBonus { get; }
        public int AgilityBonus { get; }
        public int IntelligenceBonus { get; }
        public int MaxHPBonus { get; }
        public int MaxManaBonus { get; }
        public int MaxStaminaBonus { get; }

        // Combat bonuses (per rank)
        public int DamageBonus { get; }
        public int CritChanceBonus { get; } // Percentage
        public int DefenseBonus { get; }
        public int ArmorBonus { get; }
        public int DodgeChanceBonus { get; } // Percentage

        // Special effects
        public double LifeStealPercent { get; } // Percentage of damage returned as health
        public double ManaEfficiencyPercent { get; } // Reduce mana costs
        public double GoldBonusPercent { get; } // Extra gold from loot
        public double XPBonusPercent { get; } // Extra experience

        public Skill(string name, string description, SkillType type, SkillCategory category,
                    int maxRank, int requiredLevel,
                    string? prerequisiteSkill = null, int prerequisiteRank = 0,
                    int strBonus = 0, int agiBonus = 0, int intBonus = 0,
                    int hpBonus = 0, int manaBonus = 0, int staminaBonus = 0,
                    int damageBonus = 0, int critBonus = 0, int defenseBonus = 0, int armorBonus = 0, int dodgeBonus = 0,
                    double lifeSteal = 0, double manaEfficiency = 0, double goldBonus = 0, double xpBonus = 0)
        {
            Name = name;
            Description = description;
            Type = type;
            Category = category;
            MaxRank = maxRank;
            RequiredLevel = requiredLevel;
            PrerequisiteSkill = prerequisiteSkill;
            PrerequisiteRank = prerequisiteRank;
            StrengthBonus = strBonus;
            AgilityBonus = agiBonus;
            IntelligenceBonus = intBonus;
            MaxHPBonus = hpBonus;
            MaxManaBonus = manaBonus;
            MaxStaminaBonus = staminaBonus;
            DamageBonus = damageBonus;
            CritChanceBonus = critBonus;
            DefenseBonus = defenseBonus;
            ArmorBonus = armorBonus;
            DodgeChanceBonus = dodgeBonus;
            LifeStealPercent = lifeSteal;
            ManaEfficiencyPercent = manaEfficiency;
            GoldBonusPercent = goldBonus;
            XPBonusPercent = xpBonus;
            CurrentRank = 0;
        }

        public bool CanLearn(int characterLevel, Dictionary<string, Skill> learnedSkills)
        {
            // Check level requirement
            if (characterLevel < RequiredLevel) return false;

            // Check if already maxed
            if (CurrentRank >= MaxRank) return false;

            // Check prerequisite
            if (!string.IsNullOrEmpty(PrerequisiteSkill))
            {
                if (!learnedSkills.TryGetValue(PrerequisiteSkill, out var prereq))
                    return false;
                if (prereq.CurrentRank < PrerequisiteRank)
                    return false;
            }

            return true;
        }

        public void IncreaseRank()
        {
            if (CurrentRank < MaxRank)
                CurrentRank++;
        }

        public override string ToString()
        {
            string rankInfo = MaxRank > 1 ? $" (Rank {CurrentRank}/{MaxRank})" : CurrentRank > 0 ? " [LEARNED]" : "";
            string reqInfo = $"Req: Lv{RequiredLevel}";
            if (!string.IsNullOrEmpty(PrerequisiteSkill))
                reqInfo += $", {PrerequisiteSkill} Rank {PrerequisiteRank}";

            return $"{Name}{rankInfo} - {Description} [{reqInfo}]";
        }
    }

    // Skill tree manager for a character
    internal class SkillTree
    {
        public Dictionary<string, Skill> AvailableSkills { get; }
        public Dictionary<string, Skill> LearnedSkills { get; }
        public int SkillPoints { get; private set; }

        public SkillTree()
        {
            AvailableSkills = new Dictionary<string, Skill>();
            LearnedSkills = new Dictionary<string, Skill>();
            SkillPoints = 0;
        }

        public void AddSkillPoint()
        {
            SkillPoints++;
        }

        public bool LearnSkill(string skillName, int characterLevel)
        {
            if (SkillPoints <= 0)
            {
                Console.WriteLine("No skill points available!");
                return false;
            }

            if (!AvailableSkills.TryGetValue(skillName, out var skill))
            {
                Console.WriteLine("Skill not found!");
                return false;
            }

            if (!skill.CanLearn(characterLevel, LearnedSkills))
            {
                Console.WriteLine("Cannot learn this skill yet! Check requirements.");
                return false;
            }

            SkillPoints--;
            skill.IncreaseRank();

            if (!LearnedSkills.ContainsKey(skillName))
                LearnedSkills.Add(skillName, skill);

            Console.WriteLine($"✨ Learned {skill.Name} (Rank {skill.CurrentRank})!");
            return true;
        }

        public int GetTotalStrengthBonus()
        {
            return LearnedSkills.Values.Sum(s => s.StrengthBonus * s.CurrentRank);
        }

        public int GetTotalAgilityBonus()
        {
            return LearnedSkills.Values.Sum(s => s.AgilityBonus * s.CurrentRank);
        }

        public int GetTotalIntelligenceBonus()
        {
            return LearnedSkills.Values.Sum(s => s.IntelligenceBonus * s.CurrentRank);
        }

        public int GetTotalMaxHPBonus()
        {
            return LearnedSkills.Values.Sum(s => s.MaxHPBonus * s.CurrentRank);
        }

        public int GetTotalMaxManaBonus()
        {
            return LearnedSkills.Values.Sum(s => s.MaxManaBonus * s.CurrentRank);
        }

        public int GetTotalMaxStaminaBonus()
        {
            return LearnedSkills.Values.Sum(s => s.MaxStaminaBonus * s.CurrentRank);
        }

        public int GetTotalDamageBonus()
        {
            return LearnedSkills.Values.Sum(s => s.DamageBonus * s.CurrentRank);
        }

        public int GetTotalCritChanceBonus()
        {
            return LearnedSkills.Values.Sum(s => s.CritChanceBonus * s.CurrentRank);
        }

        public int GetTotalDefenseBonus()
        {
            return LearnedSkills.Values.Sum(s => s.DefenseBonus * s.CurrentRank);
        }

        public int GetTotalArmorBonus()
        {
            return LearnedSkills.Values.Sum(s => s.ArmorBonus * s.CurrentRank);
        }

        public int GetTotalDodgeChanceBonus()
        {
            return LearnedSkills.Values.Sum(s => s.DodgeChanceBonus * s.CurrentRank);
        }

        public double GetTotalLifeStealPercent()
        {
            return LearnedSkills.Values.Sum(s => s.LifeStealPercent * s.CurrentRank);
        }

        public double GetTotalManaEfficiencyPercent()
        {
            return LearnedSkills.Values.Sum(s => s.ManaEfficiencyPercent * s.CurrentRank);
        }

        public double GetTotalGoldBonusPercent()
        {
            return LearnedSkills.Values.Sum(s => s.GoldBonusPercent * s.CurrentRank);
        }

        public double GetTotalXPBonusPercent()
        {
            return LearnedSkills.Values.Sum(s => s.XPBonusPercent * s.CurrentRank);
        }

        public bool HasSkill(string skillName, int minRank = 1)
        {
            return LearnedSkills.TryGetValue(skillName, out var skill) && skill.CurrentRank >= minRank;
        }
    }

    // Static class to define and create skill trees for each character class
    internal static class SkillTreeFactory
    {
        // Create skill tree for Warrior class
        public static SkillTree CreateWarriorTree()
        {
            var tree = new SkillTree();

            // TIER 1 - Basic Skills (Level 1+)
            tree.AvailableSkills.Add("Heavy Armor Mastery", new Skill(
                "Heavy Armor Mastery", "Increases max HP and defense", SkillType.Passive, SkillCategory.Defense,
                maxRank: 5, requiredLevel: 1, hpBonus: 10, defenseBonus: 1));

            tree.AvailableSkills.Add("Weapon Mastery", new Skill(
                "Weapon Mastery", "Increases strength and weapon damage", SkillType.Passive, SkillCategory.Offense,
                maxRank: 5, requiredLevel: 1, strBonus: 2, damageBonus: 2));

            tree.AvailableSkills.Add("Endurance", new Skill(
                "Endurance", "Increases max HP significantly", SkillType.Passive, SkillCategory.Defense,
                maxRank: 5, requiredLevel: 1, hpBonus: 15));

            tree.AvailableSkills.Add("Battle Rage", new Skill(
                "Battle Rage", "Increases damage based on missing health", SkillType.Passive, SkillCategory.Offense,
                maxRank: 3, requiredLevel: 5, damageBonus: 3));

            // TIER 2 - Intermediate Skills (Level 10+)
            tree.AvailableSkills.Add("Shield Block", new Skill(
                "Shield Block", "Chance to block incoming attacks", SkillType.Passive, SkillCategory.Defense,
                maxRank: 3, requiredLevel: 10, prerequisiteSkill: "Heavy Armor Mastery", prerequisiteRank: 3,
                defenseBonus: 2, dodgeBonus: 5));

            tree.AvailableSkills.Add("Brutal Strike", new Skill(
                "Brutal Strike", "Power Strike deals more damage", SkillType.Passive, SkillCategory.Offense,
                maxRank: 5, requiredLevel: 10, prerequisiteSkill: "Weapon Mastery", prerequisiteRank: 3,
                damageBonus: 4));

            tree.AvailableSkills.Add("War Cry", new Skill(
                "War Cry", "Intimidate enemies, reducing their attack", SkillType.Active, SkillCategory.Utility,
                maxRank: 3, requiredLevel: 12, defenseBonus: 3));

            tree.AvailableSkills.Add("Second Wind", new Skill(
                "Second Wind", "Restore health when HP drops low", SkillType.Active, SkillCategory.Defense,
                maxRank: 3, requiredLevel: 15, prerequisiteSkill: "Endurance", prerequisiteRank: 3,
                hpBonus: 20));

            // TIER 3 - Advanced Skills (Level 25+)
            tree.AvailableSkills.Add("Whirlwind Attack", new Skill(
                "Whirlwind Attack", "Attack multiple enemies at once", SkillType.Active, SkillCategory.Offense,
                maxRank: 3, requiredLevel: 25, prerequisiteSkill: "Brutal Strike", prerequisiteRank: 3,
                damageBonus: 5, strBonus: 3));

            tree.AvailableSkills.Add("Iron Skin", new Skill(
                "Iron Skin", "Massively increases defense and HP", SkillType.Passive, SkillCategory.Defense,
                maxRank: 5, requiredLevel: 25, prerequisiteSkill: "Shield Block", prerequisiteRank: 2,
                hpBonus: 25, defenseBonus: 4));

            tree.AvailableSkills.Add("Blood Fury", new Skill(
                "Blood Fury", "Convert damage dealt to health", SkillType.Passive, SkillCategory.Offense,
                maxRank: 3, requiredLevel: 30, prerequisiteSkill: "Battle Rage", prerequisiteRank: 3,
                lifeSteal: 0.05)); // 5% per rank

            tree.AvailableSkills.Add("Unstoppable", new Skill(
                "Unstoppable", "Immune to critical hits from enemies", SkillType.Passive, SkillCategory.Defense,
                maxRank: 1, requiredLevel: 35, prerequisiteSkill: "Iron Skin", prerequisiteRank: 3,
                defenseBonus: 5));

            // TIER 4 - Ultimate Skills (Level 50+)
            tree.AvailableSkills.Add("Titan's Strength", new Skill(
                "Titan's Strength", "Massive strength and damage increase", SkillType.Passive, SkillCategory.Offense,
                maxRank: 5, requiredLevel: 50, prerequisiteSkill: "Whirlwind Attack", prerequisiteRank: 2,
                strBonus: 5, damageBonus: 8, hpBonus: 30));

            tree.AvailableSkills.Add("Last Stand", new Skill(
                "Last Stand", "When HP drops to 0, survive with 1 HP once per combat", SkillType.Passive, SkillCategory.Defense,
                maxRank: 1, requiredLevel: 60, prerequisiteSkill: "Unstoppable", prerequisiteRank: 1,
                hpBonus: 50));

            tree.AvailableSkills.Add("Warlord's Command", new Skill(
                "Warlord's Command", "Ultimate ability: Boost entire party's damage", SkillType.Ultimate, SkillCategory.Support,
                maxRank: 3, requiredLevel: 75, prerequisiteSkill: "Titan's Strength", prerequisiteRank: 3,
                damageBonus: 10));

            return tree;
        }

        // Create skill tree for Mage class
        public static SkillTree CreateMageTree()
        {
            var tree = new SkillTree();

            // TIER 1 - Basic Skills (Level 1+)
            tree.AvailableSkills.Add("Arcane Knowledge", new Skill(
                "Arcane Knowledge", "Increases intelligence and mana", SkillType.Passive, SkillCategory.Utility,
                maxRank: 5, requiredLevel: 1, intBonus: 2, manaBonus: 15));

            tree.AvailableSkills.Add("Spell Power", new Skill(
                "Spell Power", "Increases magic damage", SkillType.Passive, SkillCategory.Offense,
                maxRank: 5, requiredLevel: 1, damageBonus: 3, intBonus: 1));

            tree.AvailableSkills.Add("Mana Conservation", new Skill(
                "Mana Conservation", "Reduces mana cost of spells", SkillType.Passive, SkillCategory.Utility,
                maxRank: 5, requiredLevel: 1, manaEfficiency: 0.05)); // 5% per rank

            tree.AvailableSkills.Add("Elemental Affinity", new Skill(
                "Elemental Affinity", "Bonus damage with elemental spells", SkillType.Passive, SkillCategory.Offense,
                maxRank: 3, requiredLevel: 5, damageBonus: 4));

            // TIER 2 - Intermediate Skills (Level 10+)
            tree.AvailableSkills.Add("Critical Strike", new Skill(
                "Critical Strike", "Increases critical hit chance", SkillType.Passive, SkillCategory.Offense,
                maxRank: 5, requiredLevel: 10, prerequisiteSkill: "Spell Power", prerequisiteRank: 3,
                critBonus: 2)); // 2% per rank

            tree.AvailableSkills.Add("Mana Shield", new Skill(
                "Mana Shield", "Use mana to absorb damage", SkillType.Active, SkillCategory.Defense,
                maxRank: 3, requiredLevel: 10, prerequisiteSkill: "Arcane Knowledge", prerequisiteRank: 3,
                defenseBonus: 3, manaBonus: 20));

            tree.AvailableSkills.Add("Arcane Missiles", new Skill(
                "Arcane Missiles", "Cast multiple magic missiles", SkillType.Active, SkillCategory.Offense,
                maxRank: 5, requiredLevel: 12, prerequisiteSkill: "Elemental Affinity", prerequisiteRank: 2,
                damageBonus: 5, intBonus: 2));

            tree.AvailableSkills.Add("Meditation", new Skill(
                "Meditation", "Restore mana faster during rest", SkillType.Passive, SkillCategory.Utility,
                maxRank: 3, requiredLevel: 15, prerequisiteSkill: "Mana Conservation", prerequisiteRank: 3,
                manaBonus: 25));

            // TIER 3 - Advanced Skills (Level 25+)
            tree.AvailableSkills.Add("Chain Lightning", new Skill(
                "Chain Lightning", "Lightning spell hits multiple targets", SkillType.Active, SkillCategory.Offense,
                maxRank: 3, requiredLevel: 25, prerequisiteSkill: "Arcane Missiles", prerequisiteRank: 3,
                damageBonus: 7, intBonus: 4));

            tree.AvailableSkills.Add("Spell Mastery", new Skill(
                "Spell Mastery", "Massively increases spell power", SkillType.Passive, SkillCategory.Offense,
                maxRank: 5, requiredLevel: 25, prerequisiteSkill: "Critical Strike", prerequisiteRank: 3,
                intBonus: 4, damageBonus: 6, critBonus: 3));

            tree.AvailableSkills.Add("Arcane Barrier", new Skill(
                "Arcane Barrier", "Permanent magical shield", SkillType.Passive, SkillCategory.Defense,
                maxRank: 5, requiredLevel: 30, prerequisiteSkill: "Mana Shield", prerequisiteRank: 2,
                hpBonus: 15, defenseBonus: 4, manaBonus: 30));

            tree.AvailableSkills.Add("Time Warp", new Skill(
                "Time Warp", "Chance to act twice in combat", SkillType.Passive, SkillCategory.Utility,
                maxRank: 2, requiredLevel: 40, prerequisiteSkill: "Meditation", prerequisiteRank: 3,
                intBonus: 5));

            // TIER 4 - Ultimate Skills (Level 50+)
            tree.AvailableSkills.Add("Meteor Storm", new Skill(
                "Meteor Storm", "Ultimate spell: Devastating area damage", SkillType.Ultimate, SkillCategory.Offense,
                maxRank: 3, requiredLevel: 50, prerequisiteSkill: "Chain Lightning", prerequisiteRank: 2,
                intBonus: 8, damageBonus: 15));

            tree.AvailableSkills.Add("Archmage's Wisdom", new Skill(
                "Archmage's Wisdom", "Peak magical power and efficiency", SkillType.Passive, SkillCategory.Utility,
                maxRank: 5, requiredLevel: 60, prerequisiteSkill: "Spell Mastery", prerequisiteRank: 4,
                intBonus: 6, manaBonus: 40, manaEfficiency: 0.10));

            tree.AvailableSkills.Add("Immortality", new Skill(
                "Immortality", "Automatically teleport to safety when HP reaches 0", SkillType.Passive, SkillCategory.Defense,
                maxRank: 1, requiredLevel: 75, prerequisiteSkill: "Arcane Barrier", prerequisiteRank: 4,
                manaBonus: 50));

            return tree;
        }

        // Create skill tree for Rogue class
        public static SkillTree CreateRogueTree()
        {
            var tree = new SkillTree();

            // TIER 1 - Basic Skills (Level 1+)
            tree.AvailableSkills.Add("Dual Wielding", new Skill(
                "Dual Wielding", "Attack twice with bonus damage", SkillType.Passive, SkillCategory.Offense,
                maxRank: 5, requiredLevel: 1, agiBonus: 2, damageBonus: 2));

            tree.AvailableSkills.Add("Evasion", new Skill(
                "Evasion", "Increases dodge chance", SkillType.Passive, SkillCategory.Defense,
                maxRank: 5, requiredLevel: 1, agiBonus: 1, dodgeBonus: 3));

            tree.AvailableSkills.Add("Lockpicking", new Skill(
                "Lockpicking", "Find extra loot and treasure", SkillType.Passive, SkillCategory.Utility,
                maxRank: 5, requiredLevel: 1, goldBonus: 0.05)); // 5% per rank

            tree.AvailableSkills.Add("Sneak Attack", new Skill(
                "Sneak Attack", "Increased critical damage", SkillType.Passive, SkillCategory.Offense,
                maxRank: 5, requiredLevel: 5, critBonus: 3, damageBonus: 2));

            // TIER 2 - Intermediate Skills (Level 10+)
            tree.AvailableSkills.Add("Shadow Step", new Skill(
                "Shadow Step", "Teleport behind enemy for guaranteed crit", SkillType.Active, SkillCategory.Offense,
                maxRank: 3, requiredLevel: 10, prerequisiteSkill: "Sneak Attack", prerequisiteRank: 3,
                agiBonus: 3, critBonus: 5));

            tree.AvailableSkills.Add("Acrobatics", new Skill(
                "Acrobatics", "Increased agility and dodge", SkillType.Passive, SkillCategory.Defense,
                maxRank: 5, requiredLevel: 10, prerequisiteSkill: "Evasion", prerequisiteRank: 3,
                agiBonus: 3, dodgeBonus: 4));

            tree.AvailableSkills.Add("Pickpocket", new Skill(
                "Pickpocket", "Steal gold from enemies during combat", SkillType.Active, SkillCategory.Utility,
                maxRank: 3, requiredLevel: 12, prerequisiteSkill: "Lockpicking", prerequisiteRank: 3,
                goldBonus: 0.10));

            tree.AvailableSkills.Add("Blade Dance", new Skill(
                "Blade Dance", "Multiple quick strikes", SkillType.Active, SkillCategory.Offense,
                maxRank: 5, requiredLevel: 15, prerequisiteSkill: "Dual Wielding", prerequisiteRank: 4,
                agiBonus: 4, damageBonus: 5));

            // TIER 3 - Advanced Skills (Level 25+)
            tree.AvailableSkills.Add("Deadly Poison", new Skill(
                "Deadly Poison", "Attacks apply poison damage over time", SkillType.Passive, SkillCategory.Offense,
                maxRank: 5, requiredLevel: 25, prerequisiteSkill: "Blade Dance", prerequisiteRank: 3,
                damageBonus: 6, agiBonus: 3));

            tree.AvailableSkills.Add("Smoke Bomb", new Skill(
                "Smoke Bomb", "Escape from combat or avoid attacks", SkillType.Active, SkillCategory.Utility,
                maxRank: 3, requiredLevel: 25, prerequisiteSkill: "Acrobatics", prerequisiteRank: 4,
                dodgeBonus: 8));

            tree.AvailableSkills.Add("Master Thief", new Skill(
                "Master Thief", "Significantly increased gold and loot", SkillType.Passive, SkillCategory.Utility,
                maxRank: 5, requiredLevel: 30, prerequisiteSkill: "Pickpocket", prerequisiteRank: 3,
                goldBonus: 0.15, agiBonus: 2));

            tree.AvailableSkills.Add("Assassination", new Skill(
                "Assassination", "Chance for instant kill on low HP enemies", SkillType.Passive, SkillCategory.Offense,
                maxRank: 3, requiredLevel: 35, prerequisiteSkill: "Shadow Step", prerequisiteRank: 3,
                critBonus: 5, damageBonus: 10));

            // TIER 4 - Ultimate Skills (Level 50+)
            tree.AvailableSkills.Add("Shadow Clone", new Skill(
                "Shadow Clone", "Create a clone to fight alongside you", SkillType.Ultimate, SkillCategory.Offense,
                maxRank: 3, requiredLevel: 50, prerequisiteSkill: "Assassination", prerequisiteRank: 2,
                agiBonus: 8, damageBonus: 12));

            tree.AvailableSkills.Add("Phantom Strike", new Skill(
                "Phantom Strike", "Guaranteed critical with massive damage", SkillType.Ultimate, SkillCategory.Offense,
                maxRank: 3, requiredLevel: 60, prerequisiteSkill: "Deadly Poison", prerequisiteRank: 4,
                damageBonus: 20, critBonus: 10));

            tree.AvailableSkills.Add("Treasure Hunter", new Skill(
                "Treasure Hunter", "Doubles all gold and loot found", SkillType.Passive, SkillCategory.Utility,
                maxRank: 1, requiredLevel: 70, prerequisiteSkill: "Master Thief", prerequisiteRank: 5,
                goldBonus: 0.50));

            return tree;
        }

        // Create skill tree for Priest class
        public static SkillTree CreatePriestTree()
        {
            var tree = new SkillTree();

            // TIER 1 - Basic Skills (Level 1+)
            tree.AvailableSkills.Add("Divine Touch", new Skill(
                "Divine Touch", "Increases healing effectiveness", SkillType.Passive, SkillCategory.Support,
                maxRank: 5, requiredLevel: 1, intBonus: 2));

            tree.AvailableSkills.Add("Holy Light", new Skill(
                "Holy Light", "Basic healing spell heals more", SkillType.Passive, SkillCategory.Support,
                maxRank: 5, requiredLevel: 1, intBonus: 2, manaBonus: 15));

            tree.AvailableSkills.Add("Blessed Protection", new Skill(
                "Blessed Protection", "Increases HP and defense", SkillType.Passive, SkillCategory.Defense,
                maxRank: 5, requiredLevel: 1, hpBonus: 12, defenseBonus: 1));

            tree.AvailableSkills.Add("Prayer", new Skill(
                "Prayer", "Restore mana through meditation", SkillType.Passive, SkillCategory.Utility,
                maxRank: 5, requiredLevel: 5, manaBonus: 18, intBonus: 1));

            // TIER 2 - Intermediate Skills (Level 10+)
            tree.AvailableSkills.Add("Greater Heal", new Skill(
                "Greater Heal", "Healing spells restore much more HP", SkillType.Passive, SkillCategory.Support,
                maxRank: 5, requiredLevel: 10, prerequisiteSkill: "Divine Touch", prerequisiteRank: 3,
                intBonus: 4));

            tree.AvailableSkills.Add("Shield of Faith", new Skill(
                "Shield of Faith", "Protect allies with holy barrier", SkillType.Active, SkillCategory.Defense,
                maxRank: 5, requiredLevel: 10, prerequisiteSkill: "Blessed Protection", prerequisiteRank: 3,
                hpBonus: 20, defenseBonus: 3));

            tree.AvailableSkills.Add("Cleanse", new Skill(
                "Cleanse", "Remove debuffs and cure ailments", SkillType.Active, SkillCategory.Support,
                maxRank: 3, requiredLevel: 12, prerequisiteSkill: "Holy Light", prerequisiteRank: 4));

            tree.AvailableSkills.Add("Divine Wisdom", new Skill(
                "Divine Wisdom", "Increased intelligence and mana", SkillType.Passive, SkillCategory.Utility,
                maxRank: 5, requiredLevel: 15, prerequisiteSkill: "Prayer", prerequisiteRank: 4,
                intBonus: 4, manaBonus: 25));

            // TIER 3 - Advanced Skills (Level 25+)
            tree.AvailableSkills.Add("Mass Heal", new Skill(
                "Mass Heal", "Heal entire party at once", SkillType.Active, SkillCategory.Support,
                maxRank: 5, requiredLevel: 25, prerequisiteSkill: "Greater Heal", prerequisiteRank: 4,
                intBonus: 5, manaBonus: 30));

            tree.AvailableSkills.Add("Guardian Angel", new Skill(
                "Guardian Angel", "Prevent ally death once per combat", SkillType.Active, SkillCategory.Support,
                maxRank: 3, requiredLevel: 25, prerequisiteSkill: "Shield of Faith", prerequisiteRank: 4,
                hpBonus: 30));

            tree.AvailableSkills.Add("Holy Smite", new Skill(
                "Holy Smite", "Deal damage while healing yourself", SkillType.Active, SkillCategory.Offense,
                maxRank: 5, requiredLevel: 30, prerequisiteSkill: "Cleanse", prerequisiteRank: 2,
                intBonus: 5, damageBonus: 8));

            tree.AvailableSkills.Add("Enlightenment", new Skill(
                "Enlightenment", "Gain experience faster", SkillType.Passive, SkillCategory.Utility,
                maxRank: 3, requiredLevel: 35, prerequisiteSkill: "Divine Wisdom", prerequisiteRank: 5,
                xpBonus: 0.10, intBonus: 6)); // 10% per rank

            // TIER 4 - Ultimate Skills (Level 50+)
            tree.AvailableSkills.Add("Resurrection", new Skill(
                "Resurrection", "Bring fallen allies back to life", SkillType.Ultimate, SkillCategory.Support,
                maxRank: 3, requiredLevel: 50, prerequisiteSkill: "Guardian Angel", prerequisiteRank: 3,
                intBonus: 8, manaBonus: 50));

            tree.AvailableSkills.Add("Divine Intervention", new Skill(
                "Divine Intervention", "Fully heal and shield entire party", SkillType.Ultimate, SkillCategory.Support,
                maxRank: 3, requiredLevel: 60, prerequisiteSkill: "Mass Heal", prerequisiteRank: 5,
                intBonus: 10, hpBonus: 50, manaBonus: 40));

            tree.AvailableSkills.Add("Sanctification", new Skill(
                "Sanctification", "Ascend to divine state, massive bonuses", SkillType.Ultimate, SkillCategory.Support,
                maxRank: 1, requiredLevel: 75, prerequisiteSkill: "Resurrection", prerequisiteRank: 2,
                intBonus: 15, hpBonus: 100, manaBonus: 100, defenseBonus: 10));

            return tree;
        }

        // Get appropriate skill tree for character class
        public static SkillTree CreateSkillTreeForCharacter(Character character)
        {
            return character switch
            {
                Warrior => CreateWarriorTree(),
                Mage => CreateMageTree(),
                Rogue => CreateRogueTree(),
                Priest => CreatePriestTree(),
                _ => new SkillTree()
            };
        }

        // Display skill tree UI
        public static void ShowSkillTree(Character character)
        {
            if (character.SkillTree == null)
            {
                Console.WriteLine("No skill tree available!");
                return;
            }

            while (true)
            {
                var tree = character.SkillTree;
                Console.WriteLine($"\n╔════════════════════════════════════════╗");
                Console.WriteLine($"║     {character.Name}'s Skill Tree ({character.GetType().Name})     ║");
                Console.WriteLine($"╚════════════════════════════════════════╝");
                Console.WriteLine($"Available Skill Points: {tree.SkillPoints}");
                Console.WriteLine($"Character Level: {character.Level}\n");

                // Group skills by tier based on required level
                var tier1 = tree.AvailableSkills.Values.Where(s => s.RequiredLevel < 10).OrderBy(s => s.RequiredLevel).ToList();
                var tier2 = tree.AvailableSkills.Values.Where(s => s.RequiredLevel >= 10 && s.RequiredLevel < 25).OrderBy(s => s.RequiredLevel).ToList();
                var tier3 = tree.AvailableSkills.Values.Where(s => s.RequiredLevel >= 25 && s.RequiredLevel < 50).OrderBy(s => s.RequiredLevel).ToList();
                var tier4 = tree.AvailableSkills.Values.Where(s => s.RequiredLevel >= 50).OrderBy(s => s.RequiredLevel).ToList();

                if (tier1.Any())
                {
                    Console.WriteLine("=== TIER 1: Basic Skills ===");
                    DisplaySkillList(tier1, character.Level, tree.LearnedSkills);
                }

                if (tier2.Any())
                {
                    Console.WriteLine("\n=== TIER 2: Intermediate Skills ===");
                    DisplaySkillList(tier2, character.Level, tree.LearnedSkills);
                }

                if (tier3.Any())
                {
                    Console.WriteLine("\n=== TIER 3: Advanced Skills ===");
                    DisplaySkillList(tier3, character.Level, tree.LearnedSkills);
                }

                if (tier4.Any())
                {
                    Console.WriteLine("\n=== TIER 4: Ultimate Skills ===");
                    DisplaySkillList(tier4, character.Level, tree.LearnedSkills);
                }

                // Show skill tree menu
                Console.WriteLine("\n--- Skill Tree Options ---");
                Console.WriteLine("1) Learn Skill (spend skill point)");
                Console.WriteLine("2) View Learned Skills");
                Console.WriteLine("3) View Skill Bonuses");
                Console.WriteLine("0) Exit");
                Console.Write("Choice: ");

                var input = Console.ReadLine() ?? string.Empty;
                switch (input.Trim())
                {
                    case "1":
                        LearnSkillMenu(character);
                        break;
                    case "2":
                        ViewLearnedSkills(character);
                        break;
                    case "3":
                        ViewSkillBonuses(character);
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Invalid choice.");
                        break;
                }
            }
        }

        private static void DisplaySkillList(List<Skill> skills, int characterLevel, Dictionary<string, Skill> learned)
        {
            foreach (var skill in skills)
            {
                bool isLearned = learned.ContainsKey(skill.Name);
                bool canLearn = skill.CanLearn(characterLevel, learned);
                string status = isLearned ? $"[Rank {skill.CurrentRank}/{skill.MaxRank}]" :
                               canLearn ? "[AVAILABLE]" : "[LOCKED]";

                string typeIcon = skill.Type switch
                {
                    SkillType.Passive => "⚡",
                    SkillType.Active => "⚔️",
                    SkillType.Ultimate => "🌟",
                    _ => "•"
                };

                Console.WriteLine($"{typeIcon} {skill.Name} {status}");
                Console.WriteLine($"   {skill.Description}");
                Console.WriteLine($"   Req: Level {skill.RequiredLevel}" +
                    (string.IsNullOrEmpty(skill.PrerequisiteSkill) ? "" : $", {skill.PrerequisiteSkill} Rank {skill.PrerequisiteRank}"));

                // Show bonuses
                var bonuses = new List<string>();
                if (skill.StrengthBonus > 0) bonuses.Add($"+{skill.StrengthBonus * skill.CurrentRank}/{skill.StrengthBonus * skill.MaxRank} STR");
                if (skill.AgilityBonus > 0) bonuses.Add($"+{skill.AgilityBonus * skill.CurrentRank}/{skill.AgilityBonus * skill.MaxRank} AGI");
                if (skill.IntelligenceBonus > 0) bonuses.Add($"+{skill.IntelligenceBonus * skill.CurrentRank}/{skill.IntelligenceBonus * skill.MaxRank} INT");
                if (skill.MaxHPBonus > 0) bonuses.Add($"+{skill.MaxHPBonus * skill.CurrentRank}/{skill.MaxHPBonus * skill.MaxRank} Max HP");
                if (skill.MaxManaBonus > 0) bonuses.Add($"+{skill.MaxManaBonus * skill.CurrentRank}/{skill.MaxManaBonus * skill.MaxRank} Max Mana");
                if (skill.DamageBonus > 0) bonuses.Add($"+{skill.DamageBonus * skill.CurrentRank}/{skill.DamageBonus * skill.MaxRank} Damage");
                if (skill.CritChanceBonus > 0) bonuses.Add($"+{skill.CritChanceBonus * skill.CurrentRank}/{skill.CritChanceBonus * skill.MaxRank}% Crit");
                if (skill.DefenseBonus > 0) bonuses.Add($"+{skill.DefenseBonus * skill.CurrentRank}/{skill.DefenseBonus * skill.MaxRank} Defense");
                if (skill.DodgeChanceBonus > 0) bonuses.Add($"+{skill.DodgeChanceBonus * skill.CurrentRank}/{skill.DodgeChanceBonus * skill.MaxRank}% Dodge");
                if (skill.LifeStealPercent > 0) bonuses.Add($"{skill.LifeStealPercent * skill.CurrentRank * 100:F0}/{skill.LifeStealPercent * skill.MaxRank * 100:F0}% Life Steal");
                if (skill.ManaEfficiencyPercent > 0) bonuses.Add($"{skill.ManaEfficiencyPercent * skill.CurrentRank * 100:F0}/{skill.ManaEfficiencyPercent * skill.MaxRank * 100:F0}% Mana Cost Reduction");
                if (skill.GoldBonusPercent > 0) bonuses.Add($"+{skill.GoldBonusPercent * skill.CurrentRank * 100:F0}/{skill.GoldBonusPercent * skill.MaxRank * 100:F0}% Gold");
                if (skill.XPBonusPercent > 0) bonuses.Add($"+{skill.XPBonusPercent * skill.CurrentRank * 100:F0}/{skill.XPBonusPercent * skill.MaxRank * 100:F0}% XP");

                if (bonuses.Any())
                    Console.WriteLine($"   Bonuses: {string.Join(", ", bonuses)}");

                Console.WriteLine();
            }
        }

        private static void LearnSkillMenu(Character character)
        {
            if (character.SkillTree == null) return;

            while (true)
            {
                var tree = character.SkillTree;

                if (tree.SkillPoints <= 0)
                {
                    Console.WriteLine("You have no skill points! Gain levels to earn skill points.");
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey();
                    return;
                }

                Console.WriteLine("\n=== LEARN SKILL ===");
                Console.WriteLine($"Available Skill Points: {tree.SkillPoints}");
                Console.WriteLine($"Your Level: {character.Level}\n");

                // Show only learnable skills
                var learnableSkills = tree.AvailableSkills.Values
                    .Where(s => s.CanLearn(character.Level, tree.LearnedSkills))
                    .OrderBy(s => s.RequiredLevel)
                    .ToList();

                if (!learnableSkills.Any())
                {
                    Console.WriteLine("No skills available to learn right now.");
                    Console.WriteLine("Level up to unlock more skills or complete prerequisites!");
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey();
                    return;
                }

                for (int i = 0; i < learnableSkills.Count; i++)
                {
                    var skill = learnableSkills[i];
                    Console.WriteLine($"{i + 1}) {skill.Name} (Rank {skill.CurrentRank}/{skill.MaxRank})");
                    Console.WriteLine($"   {skill.Description}");
                    Console.WriteLine($"   Type: {skill.Type} | Category: {skill.Category}");

                    // Show what you'll gain
                    var bonuses = new List<string>();
                    if (skill.StrengthBonus > 0) bonuses.Add($"+{skill.StrengthBonus} STR");
                    if (skill.AgilityBonus > 0) bonuses.Add($"+{skill.AgilityBonus} AGI");
                    if (skill.IntelligenceBonus > 0) bonuses.Add($"+{skill.IntelligenceBonus} INT");
                    if (skill.MaxHPBonus > 0) bonuses.Add($"+{skill.MaxHPBonus} Max HP");
                    if (skill.MaxManaBonus > 0) bonuses.Add($"+{skill.MaxManaBonus} Max Mana");
                    if (skill.DamageBonus > 0) bonuses.Add($"+{skill.DamageBonus} Damage");
                    if (skill.CritChanceBonus > 0) bonuses.Add($"+{skill.CritChanceBonus}% Crit");
                    if (skill.DefenseBonus > 0) bonuses.Add($"+{skill.DefenseBonus} Defense");
                    if (skill.DodgeChanceBonus > 0) bonuses.Add($"+{skill.DodgeChanceBonus}% Dodge");
                    if (skill.LifeStealPercent > 0) bonuses.Add($"+{skill.LifeStealPercent * 100:F0}% Life Steal");
                    if (skill.ManaEfficiencyPercent > 0) bonuses.Add($"-{skill.ManaEfficiencyPercent * 100:F0}% Mana Cost");
                    if (skill.GoldBonusPercent > 0) bonuses.Add($"+{skill.GoldBonusPercent * 100:F0}% Gold");
                    if (skill.XPBonusPercent > 0) bonuses.Add($"+{skill.XPBonusPercent * 100:F0}% XP");

                    if (bonuses.Any())
                        Console.WriteLine($"   Gain per rank: {string.Join(", ", bonuses)}");
                    Console.WriteLine();
                }

                Console.Write("Enter skill number to learn (0 to cancel): ");
                var input = Console.ReadLine() ?? string.Empty;
                if (!int.TryParse(input, out var choice) || choice < 1 || choice > learnableSkills.Count)
                {
                    Console.WriteLine("Cancelled.");
                    return;
                }

                var selectedSkill = learnableSkills[choice - 1];
                if (tree.LearnSkill(selectedSkill.Name, character.Level))
                {
                    Console.WriteLine($"🎓 {character.Name} learned {selectedSkill.Name}!");

                    if (tree.SkillPoints > 0)
                    {
                        Console.WriteLine($"\nYou still have {tree.SkillPoints} skill point(s) remaining!");
                        Console.Write("Continue training? (y/n): ");
                        var continueChoice = Console.ReadLine() ?? string.Empty;
                        if (!continueChoice.Trim().Equals("y", StringComparison.OrdinalIgnoreCase))
                        {
                            return;
                        }
                    }
                    else
                    {
                        Console.WriteLine("No more skill points remaining.");
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey();
                        return;
                    }
                }
                else
                {
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey();
                }
            }
        }

        private static void ViewLearnedSkills(Character character)
        {
            if (character.SkillTree == null) return;
            var tree = character.SkillTree;

            Console.WriteLine($"\n=== {character.Name}'s Learned Skills ===");

            if (!tree.LearnedSkills.Any())
            {
                Console.WriteLine("No skills learned yet!");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                return;
            }

            foreach (var kvp in tree.LearnedSkills.OrderBy(s => s.Value.RequiredLevel))
            {
                var skill = kvp.Value;
                string typeIcon = skill.Type switch
                {
                    SkillType.Passive => "⚡",
                    SkillType.Active => "⚔️",
                    SkillType.Ultimate => "🌟",
                    _ => "•"
                };

                Console.WriteLine($"\n{typeIcon} {skill.Name} - Rank {skill.CurrentRank}/{skill.MaxRank}");
                Console.WriteLine($"   {skill.Description}");
                Console.WriteLine($"   Type: {skill.Type} | Category: {skill.Category}");

                // Show current bonuses
                var bonuses = new List<string>();
                int rank = skill.CurrentRank;
                if (skill.StrengthBonus > 0) bonuses.Add($"+{skill.StrengthBonus * rank} STR");
                if (skill.AgilityBonus > 0) bonuses.Add($"+{skill.AgilityBonus * rank} AGI");
                if (skill.IntelligenceBonus > 0) bonuses.Add($"+{skill.IntelligenceBonus * rank} INT");
                if (skill.MaxHPBonus > 0) bonuses.Add($"+{skill.MaxHPBonus * rank} Max HP");
                if (skill.MaxManaBonus > 0) bonuses.Add($"+{skill.MaxManaBonus * rank} Max Mana");
                if (skill.DamageBonus > 0) bonuses.Add($"+{skill.DamageBonus * rank} Damage");
                if (skill.CritChanceBonus > 0) bonuses.Add($"+{skill.CritChanceBonus * rank}% Crit");
                if (skill.DefenseBonus > 0) bonuses.Add($"+{skill.DefenseBonus * rank} Defense");
                if (skill.DodgeChanceBonus > 0) bonuses.Add($"+{skill.DodgeChanceBonus * rank}% Dodge");
                if (skill.LifeStealPercent > 0) bonuses.Add($"+{skill.LifeStealPercent * rank * 100:F0}% Life Steal");
                if (skill.ManaEfficiencyPercent > 0) bonuses.Add($"-{skill.ManaEfficiencyPercent * rank * 100:F0}% Mana Cost");
                if (skill.GoldBonusPercent > 0) bonuses.Add($"+{skill.GoldBonusPercent * rank * 100:F0}% Gold");
                if (skill.XPBonusPercent > 0) bonuses.Add($"+{skill.XPBonusPercent * rank * 100:F0}% XP");

                if (bonuses.Any())
                    Console.WriteLine($"   Current Bonuses: {string.Join(", ", bonuses)}");
            }

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        private static void ViewSkillBonuses(Character character)
        {
            if (character.SkillTree == null) return;
            var tree = character.SkillTree;

            Console.WriteLine($"\n=== {character.Name}'s Total Skill Bonuses ===");

            Console.WriteLine("\n--- Stats ---");
            Console.WriteLine($"Strength: +{tree.GetTotalStrengthBonus()}");
            Console.WriteLine($"Agility: +{tree.GetTotalAgilityBonus()}");
            Console.WriteLine($"Intelligence: +{tree.GetTotalIntelligenceBonus()}");
            Console.WriteLine($"Max HP: +{tree.GetTotalMaxHPBonus()}");
            Console.WriteLine($"Max Mana: +{tree.GetTotalMaxManaBonus()}");

            Console.WriteLine("\n--- Combat ---");
            Console.WriteLine($"Damage: +{tree.GetTotalDamageBonus()}");
            Console.WriteLine($"Critical Chance: +{tree.GetTotalCritChanceBonus()}%");
            Console.WriteLine($"Defense: +{tree.GetTotalDefenseBonus()}");
            Console.WriteLine($"Dodge Chance: +{tree.GetTotalDodgeChanceBonus()}%");

            Console.WriteLine("\n--- Special Effects ---");
            if (tree.GetTotalLifeStealPercent() > 0)
                Console.WriteLine($"Life Steal: {tree.GetTotalLifeStealPercent() * 100:F0}%");
            if (tree.GetTotalManaEfficiencyPercent() > 0)
                Console.WriteLine($"Mana Cost Reduction: {tree.GetTotalManaEfficiencyPercent() * 100:F0}%");
            if (tree.GetTotalGoldBonusPercent() > 0)
                Console.WriteLine($"Gold Bonus: +{tree.GetTotalGoldBonusPercent() * 100:F0}%");
            if (tree.GetTotalXPBonusPercent() > 0)
                Console.WriteLine($"XP Bonus: +{tree.GetTotalXPBonusPercent() * 100:F0}%");

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }
    }
}
