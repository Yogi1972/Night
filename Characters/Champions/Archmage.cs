using Night.Characters;
using System;

namespace Rpg_Dungeon.Champions
{
    internal class Archmage : Mage
    {
        public Archmage(string name) : base(name)
        {
            MaxHealth += 30;
            Health = MaxHealth;
            MaxMana += 100;
            Mana = MaxMana;
            Intelligence += 15;
            Agility += 2;
            ArmorRating += 3;
        }

        public void Meteor(Character target)
        {
            int baseCost = 40;
            int cost = GetEffectiveManaCost(baseCost);
            if (Mana < cost || target == null || !target.IsAlive)
            {
                Console.WriteLine($"{Name} doesn't have enough mana for Meteor!");
                return;
            }

            Mana -= cost;
            if (cost < baseCost) Console.WriteLine($" [Mana Efficiency: {baseCost} → {cost}]");
            int damage = GetTotalIntelligence() * 5;
            target.ReceiveDamage(damage);
            Console.WriteLine($"☄️🔥 {Name} summons a devastating METEOR! [Mana: -{cost}]");
        }

        public void ArcaneShield()
        {
            int baseCost = 25;
            int cost = GetEffectiveManaCost(baseCost);
            if (Mana < cost)
            {
                Console.WriteLine($"{Name} doesn't have enough mana for Arcane Shield!");
                return;
            }

            Mana -= cost;
            if (cost < baseCost) Console.WriteLine($" [Mana Efficiency: {baseCost} → {cost}]");
            int shieldAmount = GetTotalIntelligence() * 2;
            Heal(shieldAmount);
            Console.WriteLine($"🔮✨ {Name} conjures ARCANE SHIELD, absorbing {shieldAmount} HP! [Mana: -{cost}]");
        }
    }
}
