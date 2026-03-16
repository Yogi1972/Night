using System;

namespace Rpg_Dungeon
{
    internal class Pet
    {
        #region Properties

        public string Name { get; set; }
        public PetType Type { get; }
        public int Level { get; private set; }
        public int Experience { get; private set; }
        public int ExperienceToLevel { get; private set; }
        public PetAbility Ability { get; }
        public int Loyalty { get; private set; }

        #endregion

        #region Constructor

        public Pet(string name, PetType type, PetAbility ability)
        {
            Name = name;
            Type = type;
            Ability = ability;
            Level = 1;
            Experience = 0;
            ExperienceToLevel = 100;
            Loyalty = 50;
        }

        #endregion

        #region Methods

        public void GainExperience(int amount)
        {
            Experience += amount;
            while (Experience >= ExperienceToLevel && Level < 10)
            {
                Experience -= ExperienceToLevel;
                Level++;
                ExperienceToLevel = (int)(ExperienceToLevel * 1.5);
                Console.WriteLine($"\n🐾 {Name} leveled up to Level {Level}!");
            }
        }

        public void IncreaseLoyalty(int amount)
        {
            Loyalty = Math.Min(100, Loyalty + amount);
            if (Loyalty == 100)
            {
                Console.WriteLine($"{Name} is completely devoted to you!");
            }
        }

        public void Feed()
        {
            IncreaseLoyalty(5);
            Console.WriteLine($"{Name} happily eats and feels closer to you. (Loyalty: {Loyalty}/100)");
        }

        public string GetAbilityDescription()
        {
            return Ability switch
            {
                PetAbility.HealthRegen => "Regenerates 5% HP after combat",
                PetAbility.LootBonus => "Increases gold drops by 20%",
                PetAbility.ExperienceBoost => "Increases XP gain by 10%",
                PetAbility.DamageBoost => "Increases damage by 5%",
                PetAbility.DefenseBoost => "Increases defense by 5%",
                PetAbility.ManaRegen => "Regenerates 5% Mana after combat",
                _ => "Unknown ability"
            };
        }

        public override string ToString()
        {
            return $"{Name} (Lv{Level} {Type}) - {GetAbilityDescription()} [Loyalty: {Loyalty}/100]";
        }

        #endregion
    }
}
