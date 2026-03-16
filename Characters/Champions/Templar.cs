using Night.Characters;
using System;
using System.Collections.Generic;

namespace Rpg_Dungeon.Champions
{
    internal class Templar : Priest
    {
        public Templar(string name) : base(name)
        {
            MaxHealth += 60;
            Health = MaxHealth;
            MaxMana += 60;
            Mana = MaxMana;
            Strength += 10;
            Intelligence += 8;
            Agility += 3;
            ArmorRating += 6;
        }

        public void SmiteEvil(Character target)
        {
            int baseCost = 25;
            int cost = GetEffectiveManaCost(baseCost);
            if (Mana < cost || target == null || !target.IsAlive)
            {
                Console.WriteLine($"{Name} doesn't have enough mana for Smite Evil!");
                return;
            }

            Mana -= cost;
            if (cost < baseCost) Console.WriteLine($" [Mana Efficiency: {baseCost} → {cost}]");
            int damage = (GetTotalIntelligence() + GetTotalStrength()) * 2;
            target.ReceiveDamage(damage);
            Console.WriteLine($"⚔️✨ {Name} SMITES EVIL! [Mana: -{cost}]");
        }

        public void LayOnHands(Character target)
        {
            int baseCost = 40;
            int cost = GetEffectiveManaCost(baseCost);
            if (Mana < cost || target == null)
            {
                Console.WriteLine($"{Name} doesn't have enough mana for Lay on Hands!");
                return;
            }

            Mana -= cost;
            if (cost < baseCost) Console.WriteLine($" [Mana Efficiency: {baseCost} → {cost}]");
            int healAmount = GetTotalIntelligence() * 4;
            target.Heal(healAmount);
            Console.WriteLine($"✨🙏 {Name} uses LAY ON HANDS on {target.Name}, healing {healAmount} HP! [Mana: -{cost}]");
        }

        public void Consecration(List<Character> party)
        {
            int baseCost = 50;
            int cost = GetEffectiveManaCost(baseCost);
            if (Mana < cost || party == null || party.Count == 0)
            {
                Console.WriteLine($"{Name} doesn't have enough mana for Consecration!");
                return;
            }

            Mana -= cost;
            if (cost < baseCost) Console.WriteLine($" [Mana Efficiency: {baseCost} → {cost}]");
            int healAmount = GetTotalIntelligence() * 3;
            Console.WriteLine($"✨⚜️ {Name} invokes CONSECRATION! Party healed and protected! [Mana: -{cost}]");

            foreach (var member in party)
            {
                if (member != null)
                {
                    member.Heal(healAmount);
                    Console.WriteLine($"  → {member.Name} healed for {healAmount} HP and gains +{GetTotalIntelligence()} AR for 2 turns!");
                }
            }
        }
    }
}
