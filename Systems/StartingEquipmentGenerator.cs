using Night.Items;
using System;
using System.Collections.Generic;

namespace Rpg_Dungeon.Systems
{
    /// <summary>
    /// Generates random starting equipment for new characters
    /// </summary>
    internal static class StartingEquipmentGenerator
    {
        private static readonly Random _rng = new Random();

        /// <summary>
        /// Give a character their starting equipment
        /// </summary>
        public static void GiveStartingEquipment(Night.Characters.Character character)
        {
            if (character == null) return;

            // Give class-appropriate weapon first
            var weapon = GenerateStartingWeapon(character);
            if (weapon != null)
            {
                character.Inventory.EquipItem(weapon, EquipmentSlot.Weapon);
                Console.WriteLine($"✓ Equipped: {weapon.Name}");
            }

            // Give class-appropriate armor
            var armor = GenerateStartingArmor(character);
            if (armor != null)
            {
                character.Inventory.EquipItem(armor, EquipmentSlot.Armor);
                Console.WriteLine($"✓ Equipped: {armor.Name}");
            }

            // Give class-appropriate offhand (if applicable)
            var offhand = GenerateStartingOffhand(character);
            if (offhand != null)
            {
                character.Inventory.EquipItem(offhand, EquipmentSlot.OffHand);
                Console.WriteLine($"✓ Equipped: {offhand.Name}");
            }

            // Always give at least a small backpack
            var backpack = GenerateBackpack();
            character.Inventory.EquipBackpack(backpack);
            Console.WriteLine($"✓ Received: {backpack.Name} (+{backpack.Slots} inventory slots)");

            // Give 1-2 belt pouches
            int pouchCount = _rng.Next(1, 3);
            for (int i = 0; i < pouchCount; i++)
            {
                var pouch = GeneratePouch();
                EquipmentSlot pouchSlot = i switch
                {
                    0 => EquipmentSlot.BeltPouch1,
                    1 => EquipmentSlot.BeltPouch2,
                    _ => EquipmentSlot.BeltPouch3
                };

                if (character.Inventory.EquipPouch(pouch, pouchSlot))
                {
                    Console.WriteLine($"✓ Received: {pouch.Name} (+{pouch.QuickSlots} quick slots) - Belt slot {i + 1}");
                }
            }

            // Give some basic starting items
            GiveStartingItems(character);
        }

        /// <summary>
        /// Generate class-appropriate starting weapon
        /// </summary>
        private static Equipment? GenerateStartingWeapon(Night.Characters.Character character)
        {
            string className = character.GetType().Name;

            return className switch
            {
                // Base Classes
                "Warrior" => CreateWeapon("Rusty Sword", str: 3, dur: 50, price: 30),
                "Mage" => CreateWeapon("Worn Staff", intel: 3, mana: 10, dur: 40, price: 35),
                "Rogue" => CreateWeapon("Dull Dagger", agi: 3, str: 1, dur: 45, price: 25),
                "Priest" => CreateWeapon("Simple Mace", str: 2, intel: 2, mana: 5, dur: 45, price: 30),

                // Champion Classes - Warriors
                "Berserker" => CreateWeapon("Crude Greataxe", str: 4, agi: -1, dur: 55, price: 40),
                "Guardian" => CreateWeapon("Battered Shield", str: 2, armor: 3, dur: 60, price: 35),
                "Paladin" => CreateWeapon("Tarnished Longsword", str: 3, intel: 1, dur: 50, price: 35),
                "Templar" => CreateWeapon("Blessed Hammer", str: 3, mana: 5, dur: 50, price: 40),

                // Champion Classes - Mages
                "Archmage" => CreateWeapon("Apprentice's Staff", intel: 4, mana: 15, dur: 40, price: 45),
                "Elementalist" => CreateWeapon("Elemental Rod", intel: 3, agi: 1, mana: 10, dur: 40, price: 40),
                "Necromancer" => CreateWeapon("Bone Wand", intel: 3, hp: -5, mana: 10, dur: 35, price: 35),
                "Oracle" => CreateWeapon("Divination Staff", intel: 3, mana: 12, dur: 40, price: 40),

                // Champion Classes - Rogues
                "Assassin" => CreateWeapon("Worn Twin Daggers", agi: 4, str: 1, dur: 45, price: 35),
                "Shadowblade" => CreateWeapon("Shadow Blade", agi: 3, intel: 1, dur: 40, price: 40),
                "Ranger" => CreateWeapon("Hunting Bow", agi: 3, str: 1, dur: 50, price: 35),

                // Champion Classes - Priests
                "Druid" => CreateWeapon("Nature's Staff", intel: 2, str: 1, mana: 8, dur: 45, price: 35),

                // Default fallback
                _ => CreateWeapon("Wooden Club", str: 2, dur: 30, price: 15)
            };
        }

        /// <summary>
        /// Helper method to create weapons with stats
        /// </summary>
        private static Equipment CreateWeapon(string name, int str = 0, int agi = 0, int intel = 0,
                                              int hp = 0, int mana = 0, int stamina = 0, int armor = 0,
                                              int dur = 50, int price = 25)
        {
            return new Equipment(name, EquipmentType.Weapon, dur, price, str, agi, intel, hp, mana, stamina, armor);
        }

        /// <summary>
        /// Generate class-appropriate starting armor
        /// </summary>
        private static Equipment? GenerateStartingArmor(Night.Characters.Character character)
        {
            string className = character.GetType().Name;

            return className switch
            {
                // Base Classes
                "Warrior" => CreateArmor("Worn Leather Armor", armor: 5, hp: 10, dur: 60, price: 40),
                "Mage" => CreateArmor("Tattered Robes", intel: 2, mana: 15, dur: 40, price: 35),
                "Rogue" => CreateArmor("Padded Vest", agi: 2, armor: 2, dur: 50, price: 30),
                "Priest" => CreateArmor("Blessed Vestments", intel: 1, mana: 10, armor: 3, dur: 50, price: 35),

                // Champion Classes - Warriors
                "Berserker" => CreateArmor("Beast Hide", str: 2, armor: 4, dur: 55, price: 40),
                "Guardian" => CreateArmor("Dented Chainmail", armor: 8, hp: 15, dur: 70, price: 50),
                "Paladin" => CreateArmor("Squire's Plate", armor: 6, hp: 10, mana: 5, dur: 65, price: 45),
                "Templar" => CreateArmor("Holy Chain", armor: 6, mana: 10, dur: 60, price: 45),

                // Champion Classes - Mages
                "Archmage" => CreateArmor("Apprentice Robes", intel: 3, mana: 20, dur: 40, price: 45),
                "Elementalist" => CreateArmor("Elemental Cloak", intel: 2, agi: 1, mana: 15, dur: 40, price: 40),
                "Necromancer" => CreateArmor("Dark Shroud", intel: 2, mana: 15, armor: 1, dur: 35, price: 40),
                "Oracle" => CreateArmor("Seer's Vestments", intel: 2, mana: 18, dur: 40, price: 40),

                // Champion Classes - Rogues
                "Assassin" => CreateArmor("Shadow Leathers", agi: 3, armor: 3, dur: 50, price: 40),
                "Shadowblade" => CreateArmor("Nightweave Armor", agi: 2, intel: 1, armor: 3, dur: 45, price: 40),
                "Ranger" => CreateArmor("Hunter's Garb", agi: 2, str: 1, armor: 3, dur: 55, price: 35),

                // Champion Classes - Priests
                "Druid" => CreateArmor("Living Bark", intel: 1, hp: 5, armor: 4, dur: 50, price: 35),

                // Default fallback
                _ => CreateArmor("Cloth Tunic", armor: 1, dur: 40, price: 15)
            };
        }

        /// <summary>
        /// Generate class-appropriate starting offhand item (shield, focus, etc.)
        /// </summary>
        private static Equipment? GenerateStartingOffhand(Night.Characters.Character character)
        {
            string className = character.GetType().Name;

            // Only certain classes get starting offhand items
            return className switch
            {
                // Base Classes
                "Warrior" => CreateOffhand("Wooden Shield", armor: 3, hp: 5, dur: 50, price: 25),
                "Mage" => CreateOffhand("Cracked Orb", intel: 1, mana: 5, dur: 30, price: 20),
                "Rogue" => null, // Rogues often dual-wield or use 2H weapons
                "Priest" => CreateOffhand("Holy Symbol", intel: 1, mana: 8, dur: 40, price: 25),

                // Champion Classes get offhand only if it fits their style
                "Guardian" => CreateOffhand("Iron Shield", armor: 5, hp: 10, dur: 70, price: 40),
                "Paladin" => CreateOffhand("Sacred Shield", armor: 4, mana: 5, dur: 60, price: 35),
                "Archmage" => CreateOffhand("Spell Focus", intel: 2, mana: 10, dur: 35, price: 30),
                "Necromancer" => CreateOffhand("Skull Fetish", intel: 1, mana: 8, dur: 30, price: 25),
                "Oracle" => CreateOffhand("Crystal Orb", intel: 2, mana: 10, dur: 35, price: 30),

                // Most other classes don't get starting offhand
                _ => null
            };
        }

        /// <summary>
        /// Helper method to create armor with stats
        /// </summary>
        private static Equipment CreateArmor(string name, int str = 0, int agi = 0, int intel = 0,
                                             int hp = 0, int mana = 0, int stamina = 0, int armor = 0,
                                             int dur = 50, int price = 30)
        {
            return new Equipment(name, EquipmentType.Armor, dur, price, str, agi, intel, hp, mana, stamina, armor);
        }

        /// <summary>
        /// Helper method to create offhand items with stats
        /// </summary>
        private static Equipment CreateOffhand(string name, int str = 0, int agi = 0, int intel = 0,
                                               int hp = 0, int mana = 0, int stamina = 0, int armor = 0,
                                               int dur = 40, int price = 20)
        {
            return new Equipment(name, EquipmentType.OffHand, dur, price, str, agi, intel, hp, mana, stamina, armor);
        }

        /// <summary>
        /// Generate a random backpack
        /// </summary>
        private static Backpack GenerateBackpack()
        {
            int roll = _rng.Next(100);

            if (roll < 60)
            {
                // Small backpack (most common)
                return new Backpack("Small Leather Backpack", slots: 5, price: 50);
            }
            else if (roll < 90)
            {
                // Medium backpack
                return new Backpack("Travel Backpack", slots: 8, price: 100);
            }
            else
            {
                // Large backpack (rare)
                return new Backpack("Adventurer's Pack", slots: 12, price: 200);
            }
        }

        /// <summary>
        /// Generate a random belt pouch
        /// </summary>
        private static Pouch GeneratePouch()
        {
            int roll = _rng.Next(100);

            if (roll < 50)
            {
                // Small pouch (most common)
                return new Pouch("Small Belt Pouch", PouchType.Small, quickSlots: 2, price: 20);
            }
            else if (roll < 80)
            {
                // Medium pouch
                return new Pouch("Leather Belt Pouch", PouchType.Medium, quickSlots: 3, price: 40);
            }
            else if (roll < 95)
            {
                // Large pouch
                return new Pouch("Large Belt Pouch", PouchType.Large, quickSlots: 4, price: 75);
            }
            else
            {
                // Specialized pouch (rare)
                var specialTypes = new[]
                {
                    (PouchType.Alchemist, "Alchemist's Pouch", 3, 60),
                    (PouchType.Coin, "Coin Purse", 2, 50)
                };

                var chosen = specialTypes[_rng.Next(specialTypes.Length)];
                return new Pouch(chosen.Item2, chosen.Item1, chosen.Item3, chosen.Item4);
            }
        }

        /// <summary>
        /// Give some basic starting items to help survive
        /// </summary>
        private static void GiveStartingItems(Night.Characters.Character character)
        {
            // Give 2-4 health potions
            int potionCount = _rng.Next(2, 5);
            for (int i = 0; i < potionCount; i++)
            {
                var potion = new GenericItem("Healing Potion", 20);
                if (character.Inventory.AddItem(potion) && i == 0)
                {
                    Console.WriteLine($"✓ Received: {potionCount}x Healing Potion");
                }
            }

            // 30% chance for a torch
            if (_rng.Next(100) < 30)
            {
                var torch = new Torch("Wooden Torch", maxBurnTime: 5, price: 15);
                if (character.Inventory.AddItem(torch))
                {
                    Console.WriteLine("✓ Received: Wooden Torch (5 hours)");
                }
            }

            // 20% chance for a rope
            if (_rng.Next(100) < 20)
            {
                var rope = new GenericItem("Rope", price: 25);
                if (character.Inventory.AddItem(rope))
                {
                    Console.WriteLine("✓ Received: Rope");
                }
            }
        }

        /// <summary>
        /// Create a custom starting loadout for testing
        /// </summary>
        public static void GiveCustomLoadout(Night.Characters.Character character, BackpackSize size, int pouchCount)
        {
            if (character == null) return;

            // Give specified backpack size
            Backpack backpack = size switch
            {
                BackpackSize.Small => new Backpack("Small Leather Backpack", 5, 50),
                BackpackSize.Medium => new Backpack("Travel Backpack", 8, 100),
                BackpackSize.Large => new Backpack("Adventurer's Pack", 12, 200),
                BackpackSize.Massive => new Backpack("Explorer's Hauler", 20, 500),
                _ => new Backpack("Small Leather Backpack", 5, 50)
            };

            character.Inventory.EquipBackpack(backpack);
            Console.WriteLine($"✓ Equipped: {backpack.Name} (+{backpack.Slots} slots)");

            // Give specified number of pouches
            pouchCount = Math.Clamp(pouchCount, 0, 3);
            for (int i = 0; i < pouchCount; i++)
            {
                var pouch = new Pouch($"Belt Pouch {i + 1}", PouchType.Medium, 3, 40);
                EquipmentSlot slot = i switch
                {
                    0 => EquipmentSlot.BeltPouch1,
                    1 => EquipmentSlot.BeltPouch2,
                    _ => EquipmentSlot.BeltPouch3
                };

                if (character.Inventory.EquipPouch(pouch, slot))
                {
                    Console.WriteLine($"✓ Equipped: {pouch.Name} (+{pouch.QuickSlots} quick slots)");
                }
            }

            GiveStartingItems(character);
        }
    }

    /// <summary>
    /// Backpack size options for custom loadouts
    /// </summary>
    internal enum BackpackSize
    {
        Small,
        Medium,
        Large,
        Massive
    }
}
