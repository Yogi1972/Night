using Night.Characters;
using System;

namespace Rpg_Dungeon.Champions
{
    internal class Berserker : Warrior
    {
        public bool IsRaging { get; set; }
        public int RageDuration { get; set; }

        public Berserker(string name) : base(name)
        {
            MaxHealth += 80;
            Health = MaxHealth;
            MaxStamina += 50;
            Stamina = MaxStamina;
            Strength += 12;
            Agility += 3;
            ArmorRating += 2;
            IsRaging = false;
            RageDuration = 0;
        }

        public void Rage()
        {
            int cost = 30;
            if (Stamina < cost)
            {
                Console.WriteLine($"{Name} doesn't have enough stamina for Rage!");
                return;
            }

            Stamina -= cost;
            IsRaging = true;
            RageDuration = 3;
            Console.WriteLine($"💢🔥 {Name} enters BERSERK RAGE! (+200% damage, lasts 3 turns) [Stamina: -{cost}]");
        }

        public override void Attack(Character target)
        {
            if (target == null || !target.IsAlive) return;
            int damage = GetTotalStrength();
            if (IsRaging)
            {
                damage = (int)(damage * 3.0);
                Console.Write($" [RAGE BONUS]");
            }
            target.ReceiveDamage(damage);
        }

        public void DecrementRage()
        {
            if (RageDuration > 0)
            {
                RageDuration--;
                if (RageDuration == 0)
                {
                    IsRaging = false;
                    Console.WriteLine($"💢 {Name}'s rage subsides.");
                }
            }
        }
    }
}
