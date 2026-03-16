using Night.Characters;
using System;

namespace Rpg_Dungeon.Champions
{
    internal class Paladin : Warrior
    {
        public Paladin(string name) : base(name)
        {
            MaxHealth += 50;
            Health = MaxHealth;
            MaxMana = 50;
            Mana = MaxMana;
            Strength += 5;
            Intelligence += 8;
            ArmorRating += 4;
        }

        public void HolyStrike(Character target)
        {
            int staminaCost = 25;
            int manaCost = 15;
            if (Stamina < staminaCost || Mana < manaCost || target == null || !target.IsAlive)
            {
                Console.WriteLine($"{Name} doesn't have enough resources for Holy Strike!");
                return;
            }

            Stamina -= staminaCost;
            Mana -= manaCost;
            int damage = (GetTotalStrength() + GetTotalIntelligence()) * 2;
            target.ReceiveDamage(damage);
            Console.WriteLine($"⚔️✨ {Name} unleashes HOLY STRIKE! [Stamina: -{staminaCost}, Mana: -{manaCost}]");
        }

        public void DivineShield()
        {
            int cost = 30;
            if (Mana < cost)
            {
                Console.WriteLine($"{Name} doesn't have enough mana for Divine Shield!");
                return;
            }

            Mana -= cost;
            int shieldAmount = GetTotalIntelligence() * 3;
            Heal(shieldAmount);
            Console.WriteLine($"🛡️✨ {Name} activates DIVINE SHIELD, restoring {shieldAmount} HP! [Mana: -{cost}]");
        }
    }
}
