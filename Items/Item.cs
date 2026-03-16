using System;

namespace Rpg_Dungeon
{
    internal abstract class Item
    {
        public string Name { get; }
        public int Price { get; }

        protected Item(string name, int price = 0)
        {
            Name = name;
            Price = Math.Max(0, price);
        }

        public override string ToString() => Name;
    }
}
