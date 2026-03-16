using Night.Characters;
using System;

namespace Rpg_Dungeon.Champions
{
    internal class Elementalist : Mage
    {
        public enum Element { Fire, Ice, Lightning, Earth }
        public Element CurrentElement { get; set; }

        public Elementalist(string name) : base(name)
        {
            MaxHealth += 25;
            Health = MaxHealth;
            MaxMana += 90;
            Mana = MaxMana;
            Intelligence += 13;
            Agility += 4;
            ArmorRating += 2;
            CurrentElement = Element.Fire;
        }

        public void ElementalBlast(Character target)
        {
            int baseCost = 35;
            int cost = GetEffectiveManaCost(baseCost);
            if (Mana < cost || target == null || !target.IsAlive)
            {
                Console.WriteLine($"{Name} doesn't have enough mana for Elemental Blast!");
                return;
            }

            Mana -= cost;
            if (cost < baseCost) Console.WriteLine($" [Mana Efficiency: {baseCost} → {cost}]");
            int damage = GetTotalIntelligence() * 4;
            target.ReceiveDamage(damage);

            string elementIcon = CurrentElement switch
            {
                Element.Fire => "🔥",
                Element.Ice => "❄️",
                Element.Lightning => "⚡",
                Element.Earth => "🌍",
                _ => "💥"
            };

            Console.WriteLine($"{elementIcon} {Name} unleashes {CurrentElement.ToString().ToUpper()} BLAST! [Mana: -{cost}]");
        }

        public void SwitchElement()
        {
            CurrentElement = (Element)(((int)CurrentElement + 1) % 4);
            Console.WriteLine($"🔄✨ {Name} attunes to {CurrentElement} element!");
        }

        public void ElementalStorm()
        {
            int baseCost = 60;
            int cost = GetEffectiveManaCost(baseCost);
            if (Mana < cost)
            {
                Console.WriteLine($"{Name} doesn't have enough mana for Elemental Storm!");
                return;
            }

            Mana -= cost;
            if (cost < baseCost) Console.WriteLine($" [Mana Efficiency: {baseCost} → {cost}]");
            Console.WriteLine($"🌪️💫 {Name} channels ELEMENTAL STORM! All enemies will be hit! [Mana: -{cost}]");
        }
    }
}
