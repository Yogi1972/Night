using Night.Characters;
using System;
using System.Collections.Generic;

namespace Rpg_Dungeon.Champions
{
    internal class Oracle : Priest
    {
        public int Prophecies { get; set; }

        public Oracle(string name) : base(name)
        {
            MaxHealth += 35;
            Health = MaxHealth;
            MaxMana += 100;
            Mana = MaxMana;
            Intelligence += 14;
            Agility += 5;
            Strength += 2;
            ArmorRating += 3;
            Prophecies = 3;
        }

        public void Foresight()
        {
            if (Prophecies <= 0)
            {
                Console.WriteLine($"{Name} has no Prophecies remaining!");
                return;
            }

            Prophecies--;
            Console.WriteLine($"🔮✨ {Name} uses FORESIGHT! Next attack will be automatically dodged! [Prophecies: {Prophecies}/3]");
        }

        public void DivineIntervention(Character target)
        {
            int baseCost = 60;
            int cost = GetEffectiveManaCost(baseCost);
            if (Mana < cost || target == null)
            {
                Console.WriteLine($"{Name} doesn't have enough mana for Divine Intervention!");
                return;
            }

            Mana -= cost;
            if (cost < baseCost) Console.WriteLine($" [Mana Efficiency: {baseCost} → {cost}]");
            int healAmount = target.MaxHealth - target.Health;
            target.Heal(healAmount);
            Console.WriteLine($"✨🙏 {Name} performs DIVINE INTERVENTION! {target.Name} fully healed! [Mana: -{cost}]");
        }

        public void MassResurrection(List<Character> party)
        {
            if (Prophecies < 3)
            {
                Console.WriteLine($"{Name} needs 3 Prophecies for Mass Resurrection! (Current: {Prophecies})");
                return;
            }

            int baseCost = 80;
            int cost = GetEffectiveManaCost(baseCost);
            if (Mana < cost || party == null || party.Count == 0)
            {
                Console.WriteLine($"{Name} doesn't have enough mana for Mass Resurrection!");
                return;
            }

            Mana -= cost;
            Prophecies = 0;
            if (cost < baseCost) Console.WriteLine($" [Mana Efficiency: {baseCost} → {cost}]");
            Console.WriteLine($"⚰️✨ {Name} channels MASS RESURRECTION! All fallen allies rise! [Mana: -{cost}, Prophecies: -3]");

            foreach (var member in party)
            {
                if (member != null && !member.IsAlive)
                {
                    member.Heal(member.MaxHealth / 2);
                    Console.WriteLine($"  → {member.Name} resurrected with {member.Health} HP!");
                }
            }
        }

        public void RestoreProphecy()
        {
            if (Prophecies < 3)
            {
                Prophecies++;
                Console.WriteLine($"🔮 {Name} gains a Prophecy! [{Prophecies}/3]");
            }
        }
    }
}
