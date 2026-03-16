using Night.Characters;
using System;

namespace Rpg_Dungeon.Champions
{
    internal class Ranger : Rogue
    {
        public bool IsFocused { get; set; }
        public int FocusDuration { get; set; }

        public Ranger(string name) : base(name)
        {
            MaxHealth += 40;
            Health = MaxHealth;
            MaxStamina += 35;
            Stamina = MaxStamina;
            Agility += 12;
            Strength += 6;
            Intelligence += 4;
            ArmorRating += 3;
            IsFocused = false;
            FocusDuration = 0;
        }

        public void MultiShot()
        {
            int cost = 30;
            if (Stamina < cost)
            {
                Console.WriteLine($"{Name} doesn't have enough stamina for Multi-Shot!");
                return;
            }

            Stamina -= cost;
            Console.WriteLine($"🏹🎯 {Name} fires MULTI-SHOT! Hitting all enemies! [Stamina: -{cost}]");
        }

        public void HuntersFocus()
        {
            int cost = 20;
            if (Stamina < cost)
            {
                Console.WriteLine($"{Name} doesn't have enough stamina for Hunter's Focus!");
                return;
            }

            Stamina -= cost;
            IsFocused = true;
            FocusDuration = 4;
            Console.WriteLine($"🎯👁️ {Name} activates HUNTER'S FOCUS! (+100% critical chance, lasts 4 turns) [Stamina: -{cost}]");
        }

        public override void Attack(Character target)
        {
            if (target == null || !target.IsAlive) return;
            int damage = GetTotalAgility();
            if (IsFocused)
            {
                damage = (int)(damage * 2.0);
                Console.Write($" [CRITICAL HIT]");
            }
            target.ReceiveDamage(damage);
        }

        public void DecrementFocus()
        {
            if (FocusDuration > 0)
            {
                FocusDuration--;
                if (FocusDuration == 0)
                {
                    IsFocused = false;
                    Console.WriteLine($"🎯 {Name}'s focus wanes.");
                }
            }
        }
    }
}
