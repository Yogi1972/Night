using Night.Characters;

namespace Rpg_Dungeon
{
    internal class Human : Race
    {
        public Human() : base("Human", healthBonus: 10, manaBonus: 5, staminaBonus: 5, strengthBonus: 2, agilityBonus: 2, intelligenceBonus: 2, armorBonus: 2,
            lore: "Adaptable and ambitious, humans thrive in every corner of the world. Their short lifespans drive them to achieve greatness with urgency no other race can match.") { }
    }
}
