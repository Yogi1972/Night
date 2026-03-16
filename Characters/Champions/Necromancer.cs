using Night.Characters;
using System;

namespace Rpg_Dungeon.Champions
{
    internal class Necromancer : Mage
    {
        public int SoulEssence { get; set; }

        public Necromancer(string name) : base(name)
        {
            MaxHealth += 20;
            Health = MaxHealth;
            MaxMana += 80;
            Mana = MaxMana;
            Intelligence += 12;
            Strength += 2;
            ArmorRating += 2;
            SoulEssence = 0;
        }

        public void DrainLife(Character target)
        {
            int baseCost = 30;
            int cost = GetEffectiveManaCost(baseCost);
            if (Mana < cost || target == null || !target.IsAlive)
            {
                Console.WriteLine($"{Name} doesn't have enough mana for Drain Life!");
                return;
            }

            Mana -= cost;
            if (cost < baseCost) Console.WriteLine($" [Mana Efficiency: {baseCost} → {cost}]");
            int damage = GetTotalIntelligence() * 3;
            target.ReceiveDamage(damage);
            int heal = damage / 2;
            Heal(heal);
            SoulEssence += 5;
            Console.WriteLine($"💀💚 {Name} drains {damage} HP from {target.Name}, healing for {heal}! (+5 Soul Essence) [Mana: -{cost}]");
        }

        public void RaiseUndead()
        {
            int cost = 50;
            if (SoulEssence < cost)
            {
                Console.WriteLine($"{Name} doesn't have enough Soul Essence to Raise Undead! (Need {cost}, have {SoulEssence})");
                return;
            }

            SoulEssence -= cost;
            Console.WriteLine($"💀⚰️ {Name} raises an UNDEAD MINION to fight alongside the party! [Soul Essence: -{cost}]");
            Console.WriteLine($"   (Minion will assist in combat for 5 turns)");
        }
    }
}
