using Night.Characters;
using System;

namespace Rpg_Dungeon.Champions
{
    internal class Shadowblade : Rogue
    {
        public int ShadowEnergy { get; set; }

        public Shadowblade(string name) : base(name)
        {
            MaxHealth += 35;
            Health = MaxHealth;
            MaxStamina += 45;
            Stamina = MaxStamina;
            Agility += 14;
            Strength += 4;
            Intelligence += 5;
            ArmorRating += 2;
            ShadowEnergy = 0;
        }

        public override void Attack(Character target)
        {
            if (target == null || !target.IsAlive) return;
            target.ReceiveDamage(GetTotalAgility());
            ShadowEnergy = Math.Min(100, ShadowEnergy + 10);
            if (ShadowEnergy < 100)
                Console.WriteLine($"  [Shadow Energy: {ShadowEnergy}/100]");
        }

        public void ShadowStep(Character target)
        {
            int cost = 35;
            if (Stamina < cost || ShadowEnergy < 50 || target == null || !target.IsAlive)
            {
                Console.WriteLine($"{Name} needs 50 Shadow Energy and {cost} stamina for Shadow Step!");
                return;
            }

            Stamina -= cost;
            ShadowEnergy -= 50;
            int damage = GetTotalAgility() * 4;
            target.ReceiveDamage(damage);
            Console.WriteLine($"🌑⚔️ {Name} performs SHADOW STEP! [Stamina: -{cost}, Shadow Energy: -50]");
        }

        public void ShadowCloak()
        {
            if (ShadowEnergy < 100)
            {
                Console.WriteLine($"{Name} needs 100 Shadow Energy for Shadow Cloak! (Current: {ShadowEnergy})");
                return;
            }

            ShadowEnergy = 0;
            ThreatLevel = 0;
            int heal = MaxHealth / 4;
            Heal(heal);
            Console.WriteLine($"👤🌫️ {Name} activates SHADOW CLOAK! (Threat reset, healed {heal} HP) [Shadow Energy: -100]");
        }
    }
}
