using Night.Characters;
using System;

namespace Rpg_Dungeon.Champions
{
    internal class Guardian : Warrior
    {
        public bool IsShielding { get; set; }
        public int ShieldDuration { get; set; }

        public Guardian(string name) : base(name)
        {
            MaxHealth += 100;
            Health = MaxHealth;
            MaxStamina += 40;
            Stamina = MaxStamina;
            Strength += 6;
            Agility -= 2;
            ArmorRating += 10;
            IsShielding = false;
            ShieldDuration = 0;
        }

        public void ShieldWall()
        {
            int cost = 35;
            if (Stamina < cost)
            {
                Console.WriteLine($"{Name} doesn't have enough stamina for Shield Wall!");
                return;
            }

            Stamina -= cost;
            IsShielding = true;
            ShieldDuration = 3;
            Console.WriteLine($"🛡️🏰 {Name} raises SHIELD WALL! (Damage reduced by 75%, lasts 3 turns) [Stamina: -{cost}]");
        }

        public override void ReceiveDamage(int amount)
        {
            if (IsShielding)
            {
                amount = (int)(amount * 0.25);
                Console.Write($" [SHIELD WALL: 75% reduced]");
            }
            base.ReceiveDamage(amount);
        }

        public void DecrementShield()
        {
            if (ShieldDuration > 0)
            {
                ShieldDuration--;
                if (ShieldDuration == 0)
                {
                    IsShielding = false;
                    Console.WriteLine($"🛡️ {Name}'s Shield Wall fades.");
                }
            }
        }
    }
}
