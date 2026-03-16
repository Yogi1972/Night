using Night.Characters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rpg_Dungeon
{
    #region Bank Class

    internal class Bank
    {
        #region Fields and Properties

        private readonly List<Item?> _storage;
        private int _depositedGold;
        private const int BaseStorageSlots = 20;
        private int _extraSlots;

        public int TotalSlots => BaseStorageSlots + _extraSlots;
        public int DepositedGold => _depositedGold;

        #endregion

        #region Constructor

        public Bank()
        {
            _storage = new List<Item?>(new Item?[BaseStorageSlots]);
            _depositedGold = 0;
            _extraSlots = 0;
        }

        #endregion

        #region Bank Interface

        public void OpenBank(List<Character> party)
        {
            if (party == null || party.Count == 0) return;

            while (true)
            {
                Console.WriteLine("\n╔════════════════════════════════════════╗");
                Console.WriteLine("║       GreyWolf Bank & Storage         ║");
                Console.WriteLine("╚════════════════════════════════════════╝");
                Console.WriteLine($"Stored Gold: {_depositedGold}g");
                Console.WriteLine($"Storage: {CountStoredItems()}/{TotalSlots} slots used");
                Console.WriteLine("\n1) Deposit Gold");
                Console.WriteLine("2) Withdraw Gold");
                Console.WriteLine("3) Deposit Items");
                Console.WriteLine("4) Withdraw Items");
                Console.WriteLine("5) View Storage");
                Console.WriteLine("6) Upgrade Storage (500g for +10 slots)");
                Console.WriteLine("0) Leave");
                Console.Write("Choice: ");

                var choice = Console.ReadLine() ?? string.Empty;

                switch (choice.Trim())
                {
                    case "1":
                        DepositGold(party);
                        break;
                    case "2":
                        WithdrawGold(party);
                        break;
                    case "3":
                        DepositItems(party);
                        break;
                    case "4":
                        WithdrawItems(party);
                        break;
                    case "5":
                        ViewStorage();
                        break;
                    case "6":
                        UpgradeStorage(party);
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Invalid choice.");
                        break;
                }
            }
        }

        #endregion

        #region Gold Transaction Methods

        private void DepositGold(List<Character> party)
        {
            Console.WriteLine("\nWho will deposit gold?");
            for (int i = 0; i < party.Count; i++)
            {
                Console.WriteLine($"{i + 1}) {party[i].Name} - Gold: {party[i].Inventory.Gold}");
            }

            var input = Console.ReadLine() ?? string.Empty;
            if (!int.TryParse(input, out var idx) || idx < 1 || idx > party.Count) return;
            var character = party[idx - 1];

            Console.Write($"How much gold to deposit? (Max: {character.Inventory.Gold}): ");
            var amountInput = Console.ReadLine() ?? string.Empty;
            if (!int.TryParse(amountInput, out var amount) || amount <= 0) return;

            if (!character.Inventory.SpendGold(amount))
            {
                Console.WriteLine("Not enough gold!");
                return;
            }

            _depositedGold += amount;
            Console.WriteLine($"\n💰 {character.Name} deposited {amount} gold.");
            Console.WriteLine($"Bank balance: {_depositedGold}g");
        }

        private void WithdrawGold(List<Character> party)
        {
            if (_depositedGold == 0)
            {
                Console.WriteLine("No gold in the bank!");
                return;
            }

            Console.WriteLine("\nWho will withdraw gold?");
            for (int i = 0; i < party.Count; i++)
            {
                Console.WriteLine($"{i + 1}) {party[i].Name} - Gold: {party[i].Inventory.Gold}");
            }

            var input = Console.ReadLine() ?? string.Empty;
            if (!int.TryParse(input, out var idx) || idx < 1 || idx > party.Count) return;
            var character = party[idx - 1];

            Console.Write($"How much gold to withdraw? (Available: {_depositedGold}): ");
            var amountInput = Console.ReadLine() ?? string.Empty;
            if (!int.TryParse(amountInput, out var amount) || amount <= 0) return;

            if (amount > _depositedGold)
            {
                Console.WriteLine("Not enough gold in the bank!");
                return;
            }

            _depositedGold -= amount;
            character.Inventory.AddGold(amount);
            Console.WriteLine($"\n💰 {character.Name} withdrew {amount} gold.");
            Console.WriteLine($"Bank balance: {_depositedGold}g");
        }

        #endregion

        #region Item Management Methods

        private void DepositItems(List<Character> party)
        {
            Console.WriteLine("\nWho will deposit items?");
            for (int i = 0; i < party.Count; i++)
            {
                Console.WriteLine($"{i + 1}) {party[i].Name}");
            }

            var input = Console.ReadLine() ?? string.Empty;
            if (!int.TryParse(input, out var idx) || idx < 1 || idx > party.Count) return;
            var character = party[idx - 1];

            var slots = character.Inventory.Slots;
            Console.WriteLine("\nSelect item to deposit:");
            for (int i = 0; i < slots.Count; i++)
            {
                var item = slots[i];
                if (item != null)
                {
                    string display = item.Name;
                    if (item is Equipment eq)
                        display += $" [{eq.Type}] (Dur {eq.Durability}/{eq.MaxDurability})";
                    Console.WriteLine($"{i + 1}) {display}");
                }
            }

            Console.Write("Item slot: ");
            var slotInput = Console.ReadLine() ?? string.Empty;
            if (!int.TryParse(slotInput, out var slotIdx) || slotIdx < 1 || slotIdx > slots.Count) return;

            var itemToDeposit = slots[slotIdx - 1];
            if (itemToDeposit == null)
            {
                Console.WriteLine("No item in that slot!");
                return;
            }

            if (!AddItemToStorage(itemToDeposit))
            {
                Console.WriteLine("Bank storage is full!");
                return;
            }

            character.Inventory.RemoveItem(slotIdx - 1);
            Console.WriteLine($"\n📦 {itemToDeposit.Name} deposited into bank storage.");
        }

        private void WithdrawItems(List<Character> party)
        {
            var storedItems = GetStoredItems();
            if (storedItems.Count == 0)
            {
                Console.WriteLine("No items in storage!");
                return;
            }

            Console.WriteLine("\nWho will withdraw items?");
            for (int i = 0; i < party.Count; i++)
            {
                Console.WriteLine($"{i + 1}) {party[i].Name}");
            }

            var input = Console.ReadLine() ?? string.Empty;
            if (!int.TryParse(input, out var idx) || idx < 1 || idx > party.Count) return;
            var character = party[idx - 1];

            Console.WriteLine("\nStored Items:");
            for (int i = 0; i < storedItems.Count; i++)
            {
                var (slotIdx, item) = storedItems[i];
                string display = item.Name;
                if (item is Equipment eq)
                    display += $" [{eq.Type}] (Dur {eq.Durability}/{eq.MaxDurability})";
                Console.WriteLine($"{i + 1}) {display}");
            }

            Console.Write("Withdraw which item? ");
            var itemInput = Console.ReadLine() ?? string.Empty;
            if (!int.TryParse(itemInput, out var itemIdx) || itemIdx < 1 || itemIdx > storedItems.Count) return;

            var (storageSlot, itemToWithdraw) = storedItems[itemIdx - 1];

            if (!character.Inventory.AddItem(itemToWithdraw))
            {
                Console.WriteLine("Inventory full!");
                return;
            }

            _storage[storageSlot] = null;
            Console.WriteLine($"\n📦 {itemToWithdraw.Name} withdrawn from bank storage.");
        }

        #endregion

        #region Storage Management

        private void ViewStorage()
        {
            Console.WriteLine("\n=== Bank Storage ===");
            Console.WriteLine($"Gold: {_depositedGold}g");
            Console.WriteLine($"Items: {CountStoredItems()}/{TotalSlots} slots");

            var storedItems = GetStoredItems();
            if (storedItems.Count == 0)
            {
                Console.WriteLine("No items stored.");
                return;
            }

            Console.WriteLine("\nStored Items:");
            foreach (var (slotIdx, item) in storedItems)
            {
                string display = item.Name;
                if (item is Equipment eq)
                    display += $" [{eq.Type}] (Dur {eq.Durability}/{eq.MaxDurability})";
                Console.WriteLine($"- {display}");
            }
        }

        private void UpgradeStorage(List<Character> party)
        {
            Console.WriteLine($"\nUpgrade storage capacity from {TotalSlots} to {TotalSlots + 10} slots for 500 gold.");
            Console.WriteLine("Who will pay?");
            for (int i = 0; i < party.Count; i++)
            {
                Console.WriteLine($"{i + 1}) {party[i].Name} - Gold: {party[i].Inventory.Gold}");
            }

            var input = Console.ReadLine() ?? string.Empty;
            if (!int.TryParse(input, out var idx) || idx < 1 || idx > party.Count) return;
            var payer = party[idx - 1];

            if (!payer.Inventory.SpendGold(500))
            {
                Console.WriteLine("Not enough gold!");
                return;
            }

            _extraSlots += 10;
            for (int i = 0; i < 10; i++)
            {
                _storage.Add(null);
            }

            Console.WriteLine($"\n🏦 Storage upgraded! New capacity: {TotalSlots} slots");
        }

        #endregion

        #region Helper Methods

        private bool AddItemToStorage(Item item)
        {
            for (int i = 0; i < _storage.Count; i++)
            {
                if (_storage[i] == null)
                {
                    _storage[i] = item;
                    return true;
                }
            }
            return false;
        }

        private int CountStoredItems()
        {
            return _storage.Count(item => item != null);
        }

        private List<(int slotIdx, Item item)> GetStoredItems()
        {
            var items = new List<(int, Item)>();
            for (int i = 0; i < _storage.Count; i++)
            {
                if (_storage[i] != null)
                {
                    items.Add((i, _storage[i]!));
                }
            }
            return items;
        }

        #endregion
    }

    #endregion
}
