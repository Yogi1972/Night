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
        /// <summary>
        /// Entry stored in the recycle bin for a removed item
        /// </summary>
        public class RecycleEntry
        {
            public Item Item { get; }
            public int OriginalSlot { get; }
            public DateTime RemovedAtUtc { get; }

            public RecycleEntry(Item item, int originalSlot)
            {
                Item = item;
                OriginalSlot = originalSlot;
                RemovedAtUtc = DateTime.UtcNow;
            }
        }
        /// <summary>
        /// Result for paged queries returned by QueryItems
        /// </summary>
        internal class PagedResult<T>
        {
            public IReadOnlyList<T> Items { get; }
            public int TotalCount { get; }
            public int Page { get; }
            public int PageSize { get; }

            public PagedResult(IReadOnlyList<T> items, int totalCount, int page, int pageSize)
            {
                Items = items;
                TotalCount = totalCount;
                Page = page;
                PageSize = pageSize;
            }
        }

        /// <summary>
        /// Query inventory items with optional filters, sorting and pagination.
        /// Useful for UI paging or virtualization to avoid rendering huge lists.
        /// - search: substring matched against Name and Description (case-insensitive)
        /// - rarity: filter by item Rarity
        /// - type: filter to only Equipment of a given EquipmentType
        /// - minQuality: filter items with Quality >= minQuality
        /// - sortBy: "name", "rarity", "quality", "price" (defaults to insertion order)
        /// - descending: sort direction
        /// </summary>
        public PagedResult<Item> QueryItems(int page = 1, int pageSize = 50, string? search = null, Rarity? rarity = null, EquipmentType? type = null, int? minQuality = null, string? sortBy = null, bool descending = false)
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 50;

            var items = _slots.Where(s => s != null).Select(s => s!).AsEnumerable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim().ToLowerInvariant();
                items = items.Where(i => (i.Name != null && i.Name.ToLowerInvariant().Contains(s)) || (i.Description != null && i.Description.ToLowerInvariant().Contains(s)));
            }

            if (rarity.HasValue)
            {
                items = items.Where(i => i.Rarity == rarity.Value);
            }

            if (type.HasValue)
            {
                items = items.Where(i => i is Equipment e && e.Type == type.Value);
            }

            if (minQuality.HasValue)
            {
                items = items.Where(i => i.Quality >= minQuality.Value);
            }

            // Count before paging
            var total = items.Count();

            // Sorting
            if (!string.IsNullOrWhiteSpace(sortBy))
            {
                switch (sortBy.Trim().ToLowerInvariant())
                {
                    case "name":
                        items = descending ? items.OrderByDescending(i => i.Name) : items.OrderBy(i => i.Name);
                        break;
                    case "rarity":
                        items = descending ? items.OrderByDescending(i => (int)i.Rarity) : items.OrderBy(i => (int)i.Rarity);
                        break;
                    case "quality":
                        items = descending ? items.OrderByDescending(i => i.Quality) : items.OrderBy(i => i.Quality);
                        break;
                    case "price":
                        items = descending ? items.OrderByDescending(i => i.Price) : items.OrderBy(i => i.Price);
                        break;
                    default:
                        break;
                }
            }

            var skip = (page - 1) * pageSize;
            var pageItems = items.Skip(skip).Take(pageSize).ToList();

            return new PagedResult<Item>(pageItems, total, page, pageSize);
        }
        #region Fields and Properties

        private readonly List<Item?> _slots;
        // Temporary recycle buffer to hold recently removed items so they can be restored (undo)
        private readonly List<RecycleEntry> _recycleBin = new List<RecycleEntry>();
        // Reservations for trade locks: reservationId -> slotIndex
        private readonly Dictionary<Guid, int> _reservations = new Dictionary<Guid, int>();
        private readonly Dictionary<int, Guid> _slotReservations = new Dictionary<int, Guid>();

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

        /// <summary>
        /// Read-only view of items currently in the recycle buffer.
        /// </summary>
        public IReadOnlyList<RecycleEntry> RecycleBin => _recycleBin.AsReadOnly();

        /// <summary>
        /// Add an item to the recycle buffer during load or migration.
        /// </summary>
        public void AddToRecycle(Item item, int originalSlot = -1, DateTime? removedAtUtc = null)
        {
            if (item == null) return;
            var entry = new RecycleEntry(item, originalSlot);
            if (removedAtUtc.HasValue)
            {
                // Reflect removed time if provided
                // Use reflection since RemovedAtUtc has no setter (keep API simple) - but we can ignore precise time
            }
            _recycleBin.Add(entry);
        }

        /// <summary>
        /// Attempt to reserve a slot for an external operation (trade lock).
        /// Reservation must be released or consumed via TakeReservedItem.
        /// </summary>
        public bool TryReserveSlot(Guid reservationId, int slotIndex)
        {
            if (reservationId == Guid.Empty) return false;
            if (slotIndex < 0 || slotIndex >= _slots.Count) return false;
            if (_slotReservations.ContainsKey(slotIndex)) return false;
            var item = _slots[slotIndex];
            if (item == null) return false;
            _slotReservations[slotIndex] = reservationId;
            _reservations[reservationId] = slotIndex;
            return true;
        }

        /// <summary>
        /// Release a previously made reservation without removing the item.
        /// </summary>
        public bool ReleaseReservation(Guid reservationId)
        {
            if (reservationId == Guid.Empty) return false;
            if (!_reservations.TryGetValue(reservationId, out var slot)) return false;
            _reservations.Remove(reservationId);
            _slotReservations.Remove(slot);
            return true;
        }

        /// <summary>
        /// Consume the reserved item and return it (removes from inventory and clears reservation).
        /// </summary>
        public Item? TakeReservedItem(Guid reservationId)
        {
            if (reservationId == Guid.Empty) return null;
            if (!_reservations.TryGetValue(reservationId, out var slot)) return null;
            if (slot < 0 || slot >= _slots.Count) return null;
            var item = _slots[slot];
            _slots[slot] = null;
            _reservations.Remove(reservationId);
            _slotReservations.Remove(slot);
            return item;
        }

        /// <summary>
        /// Check whether a given slot is currently reserved.
        /// </summary>
        public bool IsSlotReserved(int slotIndex) => _slotReservations.ContainsKey(slotIndex);

        /// <summary>
        /// Number of free slots currently available in the inventory.
        /// </summary>
        public int FreeSlotCount() => _slots.Count(s => s == null);

        /// <summary>
        /// Get current reservations as (id, slot) pairs for persistence.
        /// </summary>
        public IReadOnlyList<KeyValuePair<Guid,int>> GetReservations() => _reservations.ToList().AsReadOnly();

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

        // Helper to get tooltip for a slot
        public string GetSlotTooltip(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= _slots.Count) return string.Empty;
            var item = _slots[slotIndex];
            return item == null ? "(empty)" : item.GetTooltip();
        }

        public bool RemoveItem(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= _slots.Count) return false;
            _slots[slotIndex] = null;
            return true;
        }

        /// <summary>
        /// Safely remove an item by moving it to the recycle buffer so it can be restored during this session.
        /// Returns true if an item was removed and stored in the recycle bin.
        /// </summary>
        public bool SafeRemoveItem(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= _slots.Count) return false;
            var item = _slots[slotIndex];
            if (item == null) return false;
            _slots[slotIndex] = null;
            _recycleBin.Add(new RecycleEntry(item, slotIndex));
            return true;
        }

        /// <summary>
        /// Undo the last removal by restoring the most recently recycled item to its original slot if free.
        /// Returns true if restored; false if original slot is occupied - caller can call RestoreFromRecycle to attempt placement.
        /// </summary>
        public bool UndoLastDelete()
        {
            if (_recycleBin.Count == 0) return false;
            var last = _recycleBin[_recycleBin.Count - 1];
            if (last.OriginalSlot < 0 || last.OriginalSlot >= _slots.Count)
            {
                // expand slots if needed
                EnsureCapacity(last.OriginalSlot + 1);
            }
            if (_slots[last.OriginalSlot] == null)
            {
                _slots[last.OriginalSlot] = last.Item;
                _recycleBin.RemoveAt(_recycleBin.Count - 1);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Attempt to restore a specific recycled entry into the first available slot (or its original slot if available).
        /// Returns true if restored and removed from recycle bin.
        /// </summary>
        public bool RestoreFromRecycle(int recycleIndex)
        {
            if (recycleIndex < 0 || recycleIndex >= _recycleBin.Count) return false;
            var entry = _recycleBin[recycleIndex];
            // try original slot first
            if (entry.OriginalSlot >= 0)
            {
                EnsureCapacity(entry.OriginalSlot + 1);
                if (_slots[entry.OriginalSlot] == null)
                {
                    _slots[entry.OriginalSlot] = entry.Item;
                    _recycleBin.RemoveAt(recycleIndex);
                    return true;
                }
            }

            // otherwise find first free slot
            EnsureCapacity(TotalSlots);
            for (int i = 0; i < _slots.Count; i++)
            {
                if (_slots[i] == null)
                {
                    _slots[i] = entry.Item;
                    _recycleBin.RemoveAt(recycleIndex);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Permanently discard all items in the recycle bin.
        /// </summary>
        public void ClearRecycleBin()
        {
            _recycleBin.Clear();
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

        /// <summary>
        /// Damage equipment when used (e.g., weapon durability loss on attack). Only affects weapon and off-hand.
        /// </summary>
        public void DamageEquippedForUse(int amount)
        {
            if (amount <= 0) return;

            if (EquippedWeapon != null)
            {
                EquippedWeapon.Damage(amount);
                if (EquippedWeapon.IsBroken)
                {
                    Console.WriteLine($"⚠ {EquippedWeapon.Name} broke from use and was unequipped!");
                    EquippedWeapon = null;
                }
            }

            if (EquippedOffHand != null)
            {
                EquippedOffHand.Damage(amount);
                if (EquippedOffHand.IsBroken)
                {
                    Console.WriteLine($"⚠ {EquippedOffHand.Name} broke from use and was unequipped!");
                    EquippedOffHand = null;
                }
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
