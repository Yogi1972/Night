using Rpg_Dungeon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Night.Items
{
    #region Inventory Class

    internal class Inventory
    {
        #region Fields and Properties

        private readonly List<Item?> _slots;

        public int BaseSlots { get; } = 10;
        public int ExtraSlots { get; private set; } = 0;
        public int TotalSlots => BaseSlots + ExtraSlots;
        public int Gold { get; private set; }

        public Equipment? EquippedWeapon { get; private set; }
        public Equipment? EquippedArmor { get; private set; }
        public Equipment? EquippedAccessory { get; private set; }
        public Equipment? EquippedNecklace { get; private set; }
        public Equipment? EquippedRing1 { get; private set; }
        public Equipment? EquippedRing2 { get; private set; }
        public Equipment? EquippedOffHand { get; private set; }
        public Pouch? EquippedBeltPouch1 { get; private set; }
        public Pouch? EquippedBeltPouch2 { get; private set; }
        public Pouch? EquippedBeltPouch3 { get; private set; }

        public int TotalQuickSlots
        {
            get
            {
                int total = 0;
                if (EquippedBeltPouch1 != null) total += EquippedBeltPouch1.QuickSlots;
                if (EquippedBeltPouch2 != null) total += EquippedBeltPouch2.QuickSlots;
                if (EquippedBeltPouch3 != null) total += EquippedBeltPouch3.QuickSlots;
                return total;
            }
        }

        public IReadOnlyList<Item?> Slots => _slots.AsReadOnly();

        #endregion

        #region Constructor

        public Inventory()
        {
            _slots = new List<Item?>(new Item?[BaseSlots]);
            Gold = 0;
        }

        #endregion

        #region Item Management

        public bool AddItem(Item item)
        {
            EnsureCapacity(TotalSlots);
            for (int i = 0; i < _slots.Count; i++)
            {
                if (_slots[i] == null)
                {
                    _slots[i] = item;
                    return true;
                }
            }

            return false;
        }

        public bool RemoveItem(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= _slots.Count) return false;
            _slots[slotIndex] = null;
            return true;
        }

        #endregion

        #region Gold Management

        public void AddGold(int amount)
        {
            Gold = Math.Max(0, Gold + amount);
        }

        public bool SpendGold(int amount)
        {
            if (amount <= 0) return false;
            if (Gold < amount) return false;
            Gold -= amount;
            return true;
        }

        #endregion

        #region Backpack Management

        public bool EquipBackpack(Backpack backpack)
        {
            if (backpack == null) return false;
            if (backpack.Slots <= 0) return false;
            ExtraSlots = backpack.Slots;
            EnsureCapacity(TotalSlots);
            return true;
        }

        #endregion

        #region Pouch Management

        public bool EquipPouch(Pouch pouch, EquipmentSlot slot)
        {
            if (pouch == null) return false;
            if (slot != EquipmentSlot.BeltPouch1 && slot != EquipmentSlot.BeltPouch2 && slot != EquipmentSlot.BeltPouch3)
                return false;

            Pouch? currentPouch = slot switch
            {
                EquipmentSlot.BeltPouch1 => EquippedBeltPouch1,
                EquipmentSlot.BeltPouch2 => EquippedBeltPouch2,
                EquipmentSlot.BeltPouch3 => EquippedBeltPouch3,
                _ => null
            };

            if (currentPouch != null)
            {
                if (!AddItem(currentPouch))
                {
                    Console.WriteLine("Inventory full! Cannot unequip current pouch.");
                    return false;
                }
            }

            switch (slot)
            {
                case EquipmentSlot.BeltPouch1:
                    EquippedBeltPouch1 = pouch;
                    break;
                case EquipmentSlot.BeltPouch2:
                    EquippedBeltPouch2 = pouch;
                    break;
                case EquipmentSlot.BeltPouch3:
                    EquippedBeltPouch3 = pouch;
                    break;
            }

            return true;
        }

        public bool UnequipPouch(EquipmentSlot slot)
        {
            Pouch? toUnequip = slot switch
            {
                EquipmentSlot.BeltPouch1 => EquippedBeltPouch1,
                EquipmentSlot.BeltPouch2 => EquippedBeltPouch2,
                EquipmentSlot.BeltPouch3 => EquippedBeltPouch3,
                _ => null
            };

            if (toUnequip == null) return false;

            if (!AddItem(toUnequip))
            {
                Console.WriteLine("Inventory full! Cannot unequip pouch.");
                return false;
            }

            switch (slot)
            {
                case EquipmentSlot.BeltPouch1:
                    EquippedBeltPouch1 = null;
                    break;
                case EquipmentSlot.BeltPouch2:
                    EquippedBeltPouch2 = null;
                    break;
                case EquipmentSlot.BeltPouch3:
                    EquippedBeltPouch3 = null;
                    break;
            }

            return true;
        }

        #endregion

        #region Equipment Management

        public bool EquipItem(Equipment equipment, EquipmentSlot slot)
        {
            if (equipment == null) return false;

            Equipment? currentlyEquipped = slot switch
            {
                EquipmentSlot.Weapon => EquippedWeapon,
                EquipmentSlot.Armor => EquippedArmor,
                EquipmentSlot.Accessory => EquippedAccessory,
                EquipmentSlot.Necklace => EquippedNecklace,
                EquipmentSlot.Ring1 => EquippedRing1,
                EquipmentSlot.Ring2 => EquippedRing2,
                EquipmentSlot.OffHand => EquippedOffHand,
                _ => null
            };

            if (currentlyEquipped != null)
            {
                if (!AddItem(currentlyEquipped))
                {
                    Console.WriteLine("Inventory full! Cannot unequip current item.");
                    return false;
                }
            }

            switch (slot)
            {
                case EquipmentSlot.Weapon:
                    EquippedWeapon = equipment;
                    break;
                case EquipmentSlot.Armor:
                    EquippedArmor = equipment;
                    break;
                case EquipmentSlot.Accessory:
                    EquippedAccessory = equipment;
                    break;
                case EquipmentSlot.Necklace:
                    EquippedNecklace = equipment;
                    break;
                case EquipmentSlot.Ring1:
                    EquippedRing1 = equipment;
                    break;
                case EquipmentSlot.Ring2:
                    EquippedRing2 = equipment;
                    break;
                case EquipmentSlot.OffHand:
                    EquippedOffHand = equipment;
                    break;
                default:
                    return false;
            }

            return true;
        }

        public bool UnequipItem(EquipmentSlot slot)
        {
            Equipment? toUnequip = slot switch
            {
                EquipmentSlot.Weapon => EquippedWeapon,
                EquipmentSlot.Armor => EquippedArmor,
                EquipmentSlot.Accessory => EquippedAccessory,
                EquipmentSlot.Necklace => EquippedNecklace,
                EquipmentSlot.Ring1 => EquippedRing1,
                EquipmentSlot.Ring2 => EquippedRing2,
                EquipmentSlot.OffHand => EquippedOffHand,
                _ => null
            };

            if (toUnequip == null) return false;

            if (!AddItem(toUnequip))
            {
                Console.WriteLine("Inventory full! Cannot unequip item.");
                return false;
            }

            switch (slot)
            {
                case EquipmentSlot.Weapon:
                    EquippedWeapon = null;
                    break;
                case EquipmentSlot.Armor:
                    EquippedArmor = null;
                    break;
                case EquipmentSlot.Accessory:
                    EquippedAccessory = null;
                    break;
                case EquipmentSlot.Necklace:
                    EquippedNecklace = null;
                    break;
                case EquipmentSlot.Ring1:
                    EquippedRing1 = null;
                    break;
                case EquipmentSlot.Ring2:
                    EquippedRing2 = null;
                    break;
                case EquipmentSlot.OffHand:
                    EquippedOffHand = null;
                    break;
            }

            return true;
        }

        public void DamageEquipment(int amount)
        {
            if (amount <= 0) return;

            EquippedWeapon?.Damage(amount);
            EquippedArmor?.Damage(amount);
            EquippedAccessory?.Damage(amount);
            EquippedNecklace?.Damage(amount);
            EquippedRing1?.Damage(amount);
            EquippedRing2?.Damage(amount);
            EquippedOffHand?.Damage(amount);

            if (EquippedWeapon?.IsBroken == true)
            {
                Console.WriteLine($"⚠ {EquippedWeapon.Name} broke and was unequipped!");
                EquippedWeapon = null;
            }
            if (EquippedArmor?.IsBroken == true)
            {
                Console.WriteLine($"⚠ {EquippedArmor.Name} broke and was unequipped!");
                EquippedArmor = null;
            }
            if (EquippedAccessory?.IsBroken == true)
            {
                Console.WriteLine($"⚠ {EquippedAccessory.Name} broke and was unequipped!");
                EquippedAccessory = null;
            }
            if (EquippedNecklace?.IsBroken == true)
            {
                Console.WriteLine($"⚠ {EquippedNecklace.Name} broke and was unequipped!");
                EquippedNecklace = null;
            }
            if (EquippedRing1?.IsBroken == true)
            {
                Console.WriteLine($"⚠ {EquippedRing1.Name} broke and was unequipped!");
                EquippedRing1 = null;
            }
            if (EquippedRing2?.IsBroken == true)
            {
                Console.WriteLine($"⚠ {EquippedRing2.Name} broke and was unequipped!");
                EquippedRing2 = null;
            }
            if (EquippedOffHand?.IsBroken == true)
            {
                Console.WriteLine($"⚠ {EquippedOffHand.Name} broke and was unequipped!");
                EquippedOffHand = null;
            }
        }

        #endregion

        #region Helper Methods

        public void BurnTorches(int hours)
        {
            if (EquippedOffHand is Torch torch && torch.IsLit)
            {
                torch.Burn(hours);

                if (torch.IsBurnedOut)
                {
                    EquippedOffHand = null;
                }
            }
        }

        private void EnsureCapacity(int desired)
        {
            if (_slots.Count >= desired) return;
            int toAdd = desired - _slots.Count;
            for (int i = 0; i < toAdd; i++) _slots.Add(null);
        }

        public override string ToString()
        {
            var used = _slots.Count(s => s != null);
            return $"Slots: {used}/{TotalSlots}, Gold: {Gold}";
        }

        #endregion
    }

    #endregion
}
