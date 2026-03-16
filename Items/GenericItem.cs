namespace Rpg_Dungeon
{
    internal class GenericItem : Item
    {
        public GenericItem(string name, int price = 10, Rarity rarity = Rarity.Common, int quality = 100, string? description = null)
            : base(name, price, rarity, quality, description) { }
    }
}
