namespace Rpg_Dungeon
{
    internal class Backpack : Item
    {
        public int Slots { get; }
        public new int Price => base.Price;

        public Backpack(string name, int slots, int price) : base(name, price)
        {
            Slots = slots;
        }
    }
}
