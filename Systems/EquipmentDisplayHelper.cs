using Night.Characters;
using System;
using System.Collections.Generic;

namespace Rpg_Dungeon
{
    internal static class EquipmentDisplayHelper
    {
        public static void ShowEquippedItems(Character member)
        {
            Console.WriteLine($"\n=== {member.Name}'s Equipment ===");

            ShowEquippedSlot("Weapon", member.Inventory.EquippedWeapon);
            ShowEquippedSlot("Armor", member.Inventory.EquippedArmor);
            ShowEquippedSlot("Accessory", member.Inventory.EquippedAccessory);
            ShowEquippedSlot("Necklace", member.Inventory.EquippedNecklace);
            ShowEquippedSlot("Ring 1", member.Inventory.EquippedRing1);
            ShowEquippedSlot("Ring 2", member.Inventory.EquippedRing2);
            ShowOffHandSlot(member.Inventory.EquippedOffHand);
        }

        private static void ShowEquippedSlot(string slotName, Equipment? equipment)
        {
            Console.WriteLine($"{slotName}: {(equipment?.Name ?? "(none)")}");
            if (equipment != null)
            {
                Console.WriteLine($"  Durability: {equipment.Durability}/{equipment.MaxDurability}");
                DisplayEquipmentBonuses(equipment);
            }
        }

        private static void ShowOffHandSlot(Equipment? offHand)
        {
            if (offHand == null)
            {
                Console.WriteLine("Off-Hand: (none)");
                return;
            }

            string torchInfo = "";
            if (offHand is Torch torch)
            {
                torchInfo = torch.IsLit ? " 🔥" : " (unlit)";
            }
            Console.WriteLine($"Off-Hand: {offHand.Name}");
            Console.WriteLine($"  Durability: {offHand.Durability}/{offHand.MaxDurability}{torchInfo}");
            DisplayEquipmentBonuses(offHand);
        }

        public static void DisplayEquippedItem(string slotName, Equipment? equipment)
        {
            if (equipment != null)
            {
                string durWarning = equipment.Durability <= equipment.MaxDurability / 4 ? " [LOW!]" : "";
                Console.WriteLine($"    {slotName}: {equipment.Name} (Dur: {equipment.Durability}/{equipment.MaxDurability}){durWarning}");
            }
            else
            {
                Console.WriteLine($"    {slotName}: (none)");
            }
        }

        public static void DisplayOffHandItem(Equipment? offHand)
        {
            if (offHand != null)
            {
                string durWarning = offHand.Durability <= offHand.MaxDurability / 4 ? " [LOW!]" : "";
                string torchInfo = "";
                if (offHand is Torch torch)
                {
                    torchInfo = torch.IsLit ? " 🔥" : " (unlit)";
                }
                Console.WriteLine($"    Off-Hand: {offHand.Name} (Dur: {offHand.Durability}/{offHand.MaxDurability}){durWarning}{torchInfo}");
            }
            else
            {
                Console.WriteLine("    Off-Hand: (none)");
            }
        }

        public static void DisplayEquipmentBonuses(Equipment equipment)
        {
            var bonuses = new List<string>();
            if (equipment.StrengthBonus > 0) bonuses.Add($"+{equipment.StrengthBonus} STR");
            if (equipment.AgilityBonus > 0) bonuses.Add($"+{equipment.AgilityBonus} AGI");
            if (equipment.IntelligenceBonus > 0) bonuses.Add($"+{equipment.IntelligenceBonus} INT");
            if (equipment.MaxHPBonus > 0) bonuses.Add($"+{equipment.MaxHPBonus} HP");
            if (equipment.MaxManaBonus > 0) bonuses.Add($"+{equipment.MaxManaBonus} Mana");

            if (bonuses.Count > 0)
            {
                Console.WriteLine($"  Bonuses: {string.Join(", ", bonuses)}");
            }
        }
    }
}
