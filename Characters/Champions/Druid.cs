using Night.Characters;
using System;
using System.Collections.Generic;

namespace Rpg_Dungeon.Champions
{
    internal class Druid : Priest
    {
        public enum Form { Human, Bear, Cat }
        public Form CurrentForm { get; set; }

        public Druid(string name) : base(name)
        {
            MaxHealth += 50;
            Health = MaxHealth;
            MaxMana += 70;
            Mana = MaxMana;
            Intelligence += 10;
            Agility += 6;
            Strength += 4;
            ArmorRating += 4;
            CurrentForm = Form.Human;
        }

        public void Shapeshift(Form newForm)
        {
            int baseCost = 20;
            int cost = GetEffectiveManaCost(baseCost);
            if (Mana < cost)
            {
                Console.WriteLine($"{Name} doesn't have enough mana to Shapeshift!");
                return;
            }

            if (CurrentForm == newForm)
            {
                Console.WriteLine($"{Name} is already in {newForm} form!");
                return;
            }

            Mana -= cost;
            if (cost < baseCost) Console.WriteLine($" [Mana Efficiency: {baseCost} → {cost}]");
            CurrentForm = newForm;

            string icon = newForm switch
            {
                Form.Bear => "🐻",
                Form.Cat => "🐆",
                _ => "👤"
            };

            Console.WriteLine($"{icon}🌿 {Name} shapeshifts into {newForm} form! [Mana: -{cost}]");
        }

        public void NaturesWrath(Character target)
        {
            int baseCost = 30;
            int cost = GetEffectiveManaCost(baseCost);
            if (Mana < cost || target == null || !target.IsAlive)
            {
                Console.WriteLine($"{Name} doesn't have enough mana for Nature's Wrath!");
                return;
            }

            Mana -= cost;
            if (cost < baseCost) Console.WriteLine($" [Mana Efficiency: {baseCost} → {cost}]");
            int damage = CurrentForm switch
            {
                Form.Bear => (GetTotalStrength() + GetTotalIntelligence()) * 3,
                Form.Cat => (GetTotalAgility() + GetTotalIntelligence()) * 3,
                _ => GetTotalIntelligence() * 3
            };
            target.ReceiveDamage(damage);
            Console.WriteLine($"🌿💥 {Name} unleashes NATURE'S WRATH! [Mana: -{cost}]");
        }

        public void Rejuvenation(List<Character> party)
        {
            int baseCost = 35;
            int cost = GetEffectiveManaCost(baseCost);
            if (Mana < cost || party == null || party.Count == 0)
            {
                Console.WriteLine($"{Name} doesn't have enough mana for Rejuvenation!");
                return;
            }

            Mana -= cost;
            if (cost < baseCost) Console.WriteLine($" [Mana Efficiency: {baseCost} → {cost}]");
            int healAmount = GetTotalIntelligence() * 3;
            Console.WriteLine($"🌿💚 {Name} casts REJUVENATION on the party! [Mana: -{cost}]");

            foreach (var member in party)
            {
                if (member != null)
                {
                    member.Heal(healAmount);
                    member.RestoreMana(15);
                    member.RestoreStamina(15);
                    Console.WriteLine($"  → {member.Name} restored: {healAmount} HP, 15 Mana, 15 Stamina!");
                }
            }
        }
    }
}
