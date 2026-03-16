using Night.Characters;
using Night.Combat;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rpg_Dungeon
{
    /// <summary>
    /// Handles item usage during combat
    /// </summary>
    internal static class CombatItemHandler
    {
        private static readonly Random _rng = new Random();

        /// <summary>
        /// Use an item during combat
        /// </summary>
        public static void UseItemDuringCombat(Character member, List<Character> party, Mob mob, ref int mobHp)
        {
            Console.WriteLine($"\n{member.Name} tries to use an item during combat.");

            var slots = member.Inventory.Slots;
            if (slots.All(s => s == null))
            {
                Console.WriteLine("Inventory is empty!");
                return;
            }

            Console.WriteLine("Select slot number to use (0 to cancel):");
            for (int i = 0; i < slots.Count; i++)
            {
                var item = slots[i];
                if (item == null)
                {
                    Console.WriteLine($"{i + 1}) (empty)");
                }
                else
                {
                    Console.WriteLine($"{i + 1}) {item.Name}");
                }
            }

            Console.Write("Choice: ");
            var s = Console.ReadLine() ?? string.Empty;
            if (!int.TryParse(s, out var slotIdx) || slotIdx < 0 || slotIdx > slots.Count)
            {
                Console.WriteLine("Invalid selection.");
                return;
            }
            if (slotIdx == 0)
            {
                Console.WriteLine("Cancelled.");
                return;
            }

            var selectedItem = slots[slotIdx - 1];
            if (selectedItem == null)
            {
                Console.WriteLine("No item in that slot.");
                return;
            }

            var itemName = selectedItem.Name.ToLowerInvariant();

            // Handle healing items
            if (IsHealingItem(itemName))
            {
                UseHealingItem(member, party, selectedItem, slotIdx - 1);
                return;
            }

            // Handle mana restoration
            if (IsManaItem(itemName))
            {
                UseManaItem(member, selectedItem, slotIdx - 1);
                return;
            }

            // Handle stamina restoration
            if (IsStaminaItem(itemName))
            {
                UseStaminaItem(member, selectedItem, slotIdx - 1);
                return;
            }

            // Handle antidotes/cleansing
            if (IsCleansingItem(itemName))
            {
                UseCleansingItem(member, selectedItem, slotIdx - 1);
                return;
            }

            // Handle offensive consumables
            if (IsOffensiveItem(itemName))
            {
                UseOffensiveItem(member, mob, ref mobHp, selectedItem, slotIdx - 1);
                return;
            }

            // Item cannot be used in combat
            Console.WriteLine($"{selectedItem.Name} cannot be used in combat or has no effect.");
        }

        #region Item Type Checks

        private static bool IsHealingItem(string itemName)
        {
            return itemName.Contains("healing") || itemName.Contains("potion of healing") ||
                   itemName.Contains("poultice") || itemName.Contains("healing herb") ||
                   itemName.Contains("restorative") || itemName.Contains("salve") ||
                   itemName.Contains("tonic") || itemName.Contains("bandage");
        }

        private static bool IsManaItem(string itemName)
        {
            return itemName.Contains("mana") || itemName.Contains("potion of mana") ||
                   itemName.Contains("mana crystal") || itemName.Contains("arcane dust");
        }

        private static bool IsStaminaItem(string itemName)
        {
            return itemName.Contains("stamina") || itemName.Contains("potion of stamina") ||
                   itemName.Contains("endurance");
        }

        private static bool IsCleansingItem(string itemName)
        {
            return itemName.Contains("antidote") || itemName.Contains("cleansing") ||
                   itemName.Contains("purifying") || itemName.Contains("blessed water");
        }

        private static bool IsOffensiveItem(string itemName)
        {
            return itemName.Contains("bomb") || itemName.Contains("smoke bomb") ||
                   itemName.Contains("vial of acid") || itemName.Contains("poison vial") ||
                   itemName.Contains("oil flask");
        }

        #endregion

        #region Item Usage Handlers

        private static void UseHealingItem(Character member, List<Character> party, Item item, int slotIndex)
        {
            Console.WriteLine("Choose target to heal:");
            for (int i = 0; i < party.Count; i++)
            {
                var status = party[i].IsAlive ? $"HP {party[i].Health}" : "(Down)";
                Console.WriteLine($"{i + 1}) {party[i].Name} - {status}");
            }

            Console.Write("Target: ");
            var targetInput = Console.ReadLine() ?? string.Empty;
            if (!int.TryParse(targetInput, out var targetIdx) || targetIdx < 1 || targetIdx > party.Count)
            {
                Console.WriteLine("Invalid target.");
                return;
            }

            var target = party[targetIdx - 1];
            if (!target.IsAlive)
            {
                Console.WriteLine($"{target.Name} is down and cannot be healed with this item.");
                return;
            }

            int healAmount = 30 + (member.Intelligence / 2);
            int oldHealth = target.Health;
            target.Heal(healAmount);
            int actualHealed = target.Health - oldHealth;
            member.Inventory.RemoveItem(slotIndex);
            Console.WriteLine($"{member.Name} uses {item.Name} on {target.Name}, healing {actualHealed} HP! ({target.Health}/{target.MaxHealth})");
        }

        private static void UseManaItem(Character member, Item item, int slotIndex)
        {
            int restore = 30 + (member.Intelligence / 2);
            int oldMana = member.Mana;
            member.RestoreMana(restore);
            int actualRestored = member.Mana - oldMana;
            member.Inventory.RemoveItem(slotIndex);
            Console.WriteLine($"{member.Name} uses {item.Name}, restoring {actualRestored} Mana! ({member.Mana}/{member.MaxMana})");
        }

        private static void UseStaminaItem(Character member, Item item, int slotIndex)
        {
            int restore = 30 + (member.Strength / 2);
            int oldStamina = member.Stamina;
            member.RestoreStamina(restore);
            int actualRestored = member.Stamina - oldStamina;
            member.Inventory.RemoveItem(slotIndex);
            Console.WriteLine($"{member.Name} uses {item.Name}, restoring {actualRestored} Stamina! ({member.Stamina}/{member.MaxStamina})");
        }

        private static void UseCleansingItem(Character member, Item item, int slotIndex)
        {
            Console.WriteLine($"{member.Name} uses {item.Name}. (Status effect removal not yet implemented)");
            member.Inventory.RemoveItem(slotIndex);
        }

        private static void UseOffensiveItem(Character member, Mob mob, ref int mobHp, Item item, int slotIndex)
        {
            int roll = CombatSystem.RollD20();
            int attackBonus = member.Agility / 2;
            int targetDefense = 10 + (mob.Agility / 2);

            Console.WriteLine($"{member.Name} throws {item.Name}! Roll: {roll} (+{attackBonus}) vs Defense {targetDefense}");

            if (roll == 1)
            {
                Console.WriteLine("Critical failure! The item is wasted.");
                member.Inventory.RemoveItem(slotIndex);
                return;
            }

            int total = roll + attackBonus;
            if (roll == 20 || total > targetDefense)
            {
                int damage = 15 + (member.Intelligence / 2);
                if (roll == 20) damage *= 2;
                mobHp = Math.Max(0, mobHp - damage);
                Console.WriteLine($"Hit! {item.Name} deals {damage} damage to {mob.Name}! (Mob HP now {mobHp})");
            }
            else
            {
                Console.WriteLine($"Miss! The {item.Name} has no effect.");
            }

            member.Inventory.RemoveItem(slotIndex);
        }

        #endregion
    }
}
