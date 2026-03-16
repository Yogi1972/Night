using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Night.Characters;

namespace Rpg_Dungeon.Systems
{
    /// <summary>
    /// Simple in-memory trade reservation manager used to demonstrate trade locks and commit flow.
    /// This is authoritative and should live on the server in a real multiplayer setup.
    /// </summary>
    internal static class TradeManager
    {
        // reservationId -> (owner character, slotIndex, expiresAt)
        private class Reservation
        {
            public Guid Id { get; }
            public Character Owner { get; }
            public int SlotIndex { get; }
            public DateTime ExpiresAtUtc { get; set; }

            public Reservation(Guid id, Character owner, int slotIndex, DateTime expires)
            {
                Id = id;
                Owner = owner;
                SlotIndex = slotIndex;
                ExpiresAtUtc = expires;
            }
        }

        private static readonly ConcurrentDictionary<Guid, Reservation> _reservations = new ConcurrentDictionary<Guid, Reservation>();
        private static readonly Timer _cleanupTimer;
        private static readonly TimeSpan DefaultTtl = TimeSpan.FromSeconds(30);
        // Maximum allowed TTL to avoid long-lived reservations consuming memory
        private static readonly TimeSpan MaxTtl = TimeSpan.FromHours(1);

        static TradeManager()
        {
            _cleanupTimer = new Timer(_ => CleanupExpired(), null, TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(10));
            // Ensure cleanup on process exit to release timer resources
            try { AppDomain.CurrentDomain.ProcessExit += (_, __) => { _cleanupTimer?.Dispose(); }; } catch { }
        }

        private static void CleanupExpired()
        {
            var now = DateTime.UtcNow;
            // Snapshot current reservations to avoid enumeration issues
            var snapshot = _reservations.Values.ToArray();
            foreach (var r in snapshot)
            {
                if (r.ExpiresAtUtc <= now)
                {
                    _reservations.TryRemove(r.Id, out _);
                    // release reservation on owner's inventory
                    try { r.Owner.Inventory.ReleaseReservation(r.Id); } catch { }
                }
            }
        }

        /// <summary>
        /// Reserve an item slot on behalf of a character. Returns reservation id if successful.
        /// </summary>
        public static Guid? ReserveItem(Character owner, int slotIndex, TimeSpan? ttl = null)
        {
            if (owner == null) return null;
            var lid = Guid.NewGuid();
            var duration = ttl ?? DefaultTtl;
            if (duration > MaxTtl) duration = MaxTtl;
            // Try to reserve on owner's inventory first
            var ok = owner.Inventory.TryReserveSlot(lid, slotIndex);
            if (!ok) return null;

            var res = new Reservation(lid, owner, slotIndex, DateTime.UtcNow.Add(duration));
            if (!_reservations.TryAdd(lid, res))
            {
                owner.Inventory.ReleaseReservation(lid);
                return null;
            }

            return lid;
        }

        /// <summary>
        /// Release a reservation (e.g., on cancel or failure)
        /// </summary>
        public static bool ReleaseReservation(Guid reservationId)
        {
            if (!_reservations.TryRemove(reservationId, out var res)) return false;
            try { res.Owner.Inventory.ReleaseReservation(reservationId); } catch { }
            return true;
        }

        /// <summary>
        /// Commit a trade by consuming reserved items and transferring them to destination inventories.
        /// Expects both sides to have active reservations (atomic commit across reservations passed).
        /// </summary>
        public static bool CommitTrade(Guid[] reservationIds, Character toCharacter)
        {
            if (reservationIds == null || reservationIds.Length == 0) return false;
            if (toCharacter == null) return false;

            // Validate all reservations exist and belong to someone
            var resList = new List<Reservation>();
            foreach (var id in reservationIds)
            {
                if (!_reservations.TryGetValue(id, out var r)) return false;
                resList.Add(r);
            }

            // Ensure destination has enough free slots to accept items
            // We'll take items from owners and add to the `toCharacter` inventory
            int needed = resList.Count;
            if (toCharacter.Inventory.FreeSlotCount() < needed)
            {
                return false; // insufficient space
            }

            // Atomically take items from each owner
            var takenItems = new List<Item>();
            var takenReservations = new List<Guid>();
            try
            {
                foreach (var r in resList)
                {
                    var item = r.Owner.Inventory.TakeReservedItem(r.Id);
                    if (item == null) throw new Exception("Failed to take reserved item");
                    takenItems.Add(item);
                    takenReservations.Add(r.Id);
                }

                // Add items to destination
                foreach (var it in takenItems)
                {
                    toCharacter.Inventory.AddItem(it);
                }

                // Remove reservations
                foreach (var id in takenReservations)
                {
                    _reservations.TryRemove(id, out _);
                }

                return true;
            }
            catch
            {
                // rollback: if any failures, try to put items back to original owners' inventory
                for (int i = 0; i < takenItems.Count; i++)
                {
                    try { resList[i].Owner.Inventory.AddItem(takenItems[i]); } catch { }
                }

                // release any reservations that still exist
                foreach (var id in reservationIds)
                {
                    ReleaseReservation(id);
                }

                return false;
            }
        }
    }
}
