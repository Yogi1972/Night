using Night.Characters;

namespace Rpg_Dungeon
{
    internal class Gnome : Race
    {
        public Gnome() : base("Gnome", healthBonus: -5, manaBonus: 20, staminaBonus: 0, strengthBonus: 0, agilityBonus: 1, intelligenceBonus: 4, armorBonus: 0,
            lore: "Small in stature but immense in intellect, gnomes are tireless inventors and natural arcanists. Their motto: 'If it hasn't exploded yet, you haven't pushed it far enough.'") { }
    }
}
