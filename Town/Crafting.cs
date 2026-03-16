using Night.Characters;
using System.Collections.Generic;

namespace Rpg_Dungeon
{
    internal static class Crafting
    {
        public static void OpenCraftingWorkshop(List<Character> party)
        {
            CraftingWorkshop.Open(party);
        }

        public static void OpenForge(List<Character> party)
        {
            BlacksmithingStation.Open(party);
        }

        public static void OpenPotionBrewing(List<Character> party)
        {
            AlchemyStation.Open(party);
        }
    }
}

