using Night.Characters;
using System;

namespace Rpg_Dungeon
{
    internal class Mage : Character
    {
        public Mage(string name) : base(name)
        {
            Health = 70;
            MaxHealth = 70;
            Mana = 100;
            MaxMana = 100;
            Stamina = 0;
            MaxStamina = 0;
            Strength = 4;
            Agility = 6;
            Intelligence = 20;
            ArmorRating = 2;
        }

        public override void Attack(Character target)
        {
            if (target == null || !target.IsAlive) return;
            int baseCost = 5;
            int cost = GetEffectiveManaCost(baseCost);
            if (Mana >= cost)
            {
                Mana -= cost;
                target.ReceiveDamage(GetTotalIntelligence());
            }
        }

        public override void SpecialAbility(Character target)
        {
            int baseCost = 15;
            int cost = GetEffectiveManaCost(baseCost);
            if (Mana < cost || target == null || !target.IsAlive) return;
            Mana -= cost;
            if (cost < baseCost) Console.WriteLine($" [Mana Efficiency: {baseCost} → {cost}]");
            target.ReceiveDamage(GetTotalIntelligence() * 3);
        }
    }
}
