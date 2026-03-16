using Night.Characters;
using System;

namespace Rpg_Dungeon
{
    internal class Warrior : Character
    {
        public bool IsTaunting { get; set; }
        public int TauntDuration { get; set; }

        public Warrior(string name) : base(name)
        {
            Health = 120;
            MaxHealth = 120;
            Mana = 0;
            MaxMana = 0;
            Stamina = 100;
            MaxStamina = 100;
            Strength = 18;
            Agility = 8;
            Intelligence = 6;
            ArmorRating = 8;
            IsTaunting = false;
            TauntDuration = 0;
        }

        public override void Attack(Character target)
        {
            if (target == null || !target.IsAlive) return;
            target.ReceiveDamage(GetTotalStrength());
        }

        public override void SpecialAbility(Character target)
        {
            int cost = 20;
            if (Stamina < cost || target == null || !target.IsAlive) return;
            Stamina -= cost;
            Console.WriteLine($" [Stamina: -{cost}]");
            target.ReceiveDamage(GetTotalStrength() * 2);
        }

        public void Taunt()
        {
            int cost = 15;
            if (Stamina < cost)
            {
                Console.WriteLine($"{Name} doesn't have enough stamina to taunt! (Need {cost}, have {Stamina})");
                return;
            }

            Stamina -= cost;
            ThreatLevel += 100;
            IsTaunting = true;
            TauntDuration = 2;
            Console.WriteLine($"🛡️ {Name} TAUNTS enemies, drawing their attention! (+100 threat, lasts 2 turns) [Stamina: -{cost}]");
        }

        public void DecrementTaunt()
        {
            if (TauntDuration > 0)
            {
                TauntDuration--;
                if (TauntDuration == 0)
                {
                    IsTaunting = false;
                    Console.WriteLine($"🛡️ {Name}'s taunt effect has worn off.");
                }
            }
        }
    }
}
