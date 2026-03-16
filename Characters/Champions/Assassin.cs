using Night.Characters;
using System;

namespace Rpg_Dungeon.Champions
{
    internal class Assassin : Rogue
    {
        public int ComboPoints { get; set; }

        public Assassin(string name) : base(name)
        {
            MaxHealth += 30;
            Health = MaxHealth;
            MaxStamina += 40;
            Stamina = MaxStamina;
            Agility += 15;
            Strength += 5;
            Intelligence += 3;
            ArmorRating += 2;
            ComboPoints = 0;
        }

        public override void Attack(Character target)
        {
            if (target == null || !target.IsAlive) return;
            target.ReceiveDamage(GetTotalAgility());
            ComboPoints = Math.Min(5, ComboPoints + 1);
            if (ComboPoints < 5)
                Console.WriteLine($"  [Combo: {ComboPoints}/5]");
        }

        public void DeathStrike(Character target)
        {
            if (ComboPoints < 5 || target == null || !target.IsAlive)
            {
                Console.WriteLine($"{Name} needs 5 combo points for Death Strike! (Current: {ComboPoints})");
                return;
            }

            int cost = 40;
            if (Stamina < cost)
            {
                Console.WriteLine($"{Name} doesn't have enough stamina for Death Strike!");
                return;
            }

            Stamina -= cost;
            ComboPoints = 0;
            int damage = GetTotalAgility() * 6;
            target.ReceiveDamage(damage);
            Console.WriteLine($"💀⚔️ {Name} executes DEATH STRIKE! [Stamina: -{cost}, Combo Points: CONSUMED]");
        }

        public void Vanish()
        {
            int cost = 25;
            if (Stamina < cost)
            {
                Console.WriteLine($"{Name} doesn't have enough stamina to Vanish!");
                return;
            }

            Stamina -= cost;
            ThreatLevel = Math.Max(0, ThreatLevel - 200);
            Console.WriteLine($"💨👤 {Name} VANISHES into the shadows! (-200 threat) [Stamina: -{cost}]");
        }
    }
}
