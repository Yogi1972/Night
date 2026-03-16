using Night.Characters;
using System;
using System.Collections.Generic;

namespace Rpg_Dungeon
{
    internal class Priest : Character
    {
        public Priest(string name) : base(name)
        {
            Health = 80;
            MaxHealth = 80;
            Mana = 120;
            MaxMana = 120;
            Stamina = 0;
            MaxStamina = 0;
            Strength = 6;
            Agility = 8;
            Intelligence = 16;
            ArmorRating = 5;
        }

        public override void Attack(Character target)
        {
            if (target == null || !target.IsAlive) return;
            target.ReceiveDamage(GetTotalIntelligence() / 2);
        }

        public void HealAlly(Character target)
        {
            int baseCost = 12;
            int cost = GetEffectiveManaCost(baseCost);
            if (Mana < cost || target == null) return;
            Mana -= cost;
            if (cost < baseCost) Console.WriteLine($" [Mana Efficiency: {baseCost} → {cost}]");
            int healAmount = GetTotalIntelligence() * 2;
            target.Heal(healAmount);
            Console.WriteLine($"{Name} heals {target.Name} for {healAmount} HP!");
        }

        public override void SpecialAbility(Character target)
        {
            HealAlly(target);
        }

        public void SpecialAbility(List<Character> party)
        {
            int baseCost = 25;
            int cost = GetEffectiveManaCost(baseCost);
            if (Mana < cost || party == null || party.Count == 0) return;
            Mana -= cost;
            if (cost < baseCost) Console.WriteLine($" [Mana Efficiency: {baseCost} → {cost}]");

            int healAmount = GetTotalIntelligence() * 2;
            Console.WriteLine($"{Name} casts Mass Heal on the entire party!");

            foreach (var member in party)
            {
                if (member != null)
                {
                    member.Heal(healAmount);
                    Console.WriteLine($"  → {member.Name} healed for {healAmount} HP! (HP: {member.Health}/{member.MaxHealth})");
                }
            }
        }
    }
}
