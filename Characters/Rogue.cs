using Night.Characters;
using System;

namespace Rpg_Dungeon
{
    internal class Rogue : Character
    {
        public Rogue(string name) : base(name)
        {
            Health = 85;
            MaxHealth = 85;
            Mana = 0;
            MaxMana = 0;
            Stamina = 80;
            MaxStamina = 80;
            Strength = 10;
            Agility = 18;
            Intelligence = 8;
            ArmorRating = 4;
        }

        public override void Attack(Character target)
        {
            if (target == null || !target.IsAlive) return;
            target.ReceiveDamage(GetTotalAgility());
        }

        public override void SpecialAbility(Character target)
        {
            int cost = 15;
            if (Stamina < cost || target == null || !target.IsAlive) return;
            Stamina -= cost;
            Console.WriteLine($" [Stamina: -{cost}]");
            target.ReceiveDamage(GetTotalAgility() * 3);
        }
    }
}
