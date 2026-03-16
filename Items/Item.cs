using System;
using System.Text;

namespace Rpg_Dungeon
{
    internal enum Rarity
    {
        Common,
        Uncommon,
        Rare,
        Epic,
        Legendary
    }

    internal abstract class Item
    {
        public string Name { get; }
        public int Price { get; }

        // New properties for quality/rarity and optional description
        public Rarity Rarity { get; }
        public int Quality { get; } // 1..100
        public string? Description { get; }

        protected Item(string name, int price = 0, Rarity rarity = Rarity.Common, int quality = 100, string? description = null)
        {
            Name = name;
            Price = Math.Max(0, price);
            Rarity = rarity;
            Quality = Math.Clamp(quality, 1, 100);
            Description = description;
        }

        public virtual string GetTooltip()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"{Name} ({Rarity})");
            if (!string.IsNullOrWhiteSpace(Description)) sb.AppendLine(Description);
            sb.AppendLine($"Quality: {Quality}%");
            sb.AppendLine($"Price: {Price}g");
            return sb.ToString().TrimEnd();
        }

        public override string ToString() => Name;
    }
}
