using System.Collections.Generic;

namespace Rpg_Dungeon
{
    internal class LootResult
    {
        public int Gold { get; set; }
        public List<Item> Items { get; } = new List<Item>();
    }
}
