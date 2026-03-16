namespace Rpg_Dungeon
{
    /// <summary>
    /// Pouch item that can be equipped to belt for quick-access storage
    /// </summary>
    internal class Pouch : Item
    {
        public int QuickSlots { get; }
        public PouchType PouchType { get; }

        public Pouch(string name, PouchType pouchType, int quickSlots, int price) : base(name, price)
        {
            PouchType = pouchType;
            QuickSlots = quickSlots;
        }

        public override string ToString()
        {
            return $"{Name} ({QuickSlots} quick slots)";
        }
    }

    internal enum PouchType
    {
        Small,      // 2 quick slots
        Medium,     // 3 quick slots
        Large,      // 4 quick slots
        Alchemist,  // Specialized for potions
        Coin        // Gold storage bonus
    }
}
