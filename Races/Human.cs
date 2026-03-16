using Night.Characters;

namespace Rpg_Dungeon
{
    internal class Human : Race
    {
        public Human() : base("Human", healthBonus: 10, manaBonus: 5, staminaBonus: 5, strengthBonus: 2, agilityBonus: 2, intelligenceBonus: 2, armorBonus: 2) { }
    }
}
