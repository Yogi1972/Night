using Night.Characters;
using Night.Items;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rpg_Dungeon
{
    internal class Trading
    {
        private readonly Random _rng = new Random();

        /// <summary>
        /// Main trading interface for party members to trade items and gold
        /// </summary>
        public void OpenTradeMenu(List<Character> party)
        {
            if (party == null || party.Count < 2)
            {
                Console.WriteLine("❌ You need at least 2 party members to trade!");
                return;
            }

            while (true)
            {
                Console.WriteLine("\n╔═══════════════ TRADING ═══════════════╗");
                Console.WriteLine("║  Exchange items and gold with party  ║");
                Console.WriteLine("╚═══════════════════════════════════════╝");
                Console.WriteLine("\nSelect trading mode:");
                Console.WriteLine("1) Direct Trade (1-on-1)");
                Console.WriteLine("2) Party Trade Pool (All members)");
                Console.WriteLine("3) Gift Item to Party Member");
                Console.WriteLine("4) Gift Gold to Party Member");
                Console.WriteLine("0) Exit Trading");
                Console.Write("\nChoice: ");

                var choice = Console.ReadLine()?.Trim();

                switch (choice)
                {
                    case "1":
                        DirectTrade(party);
                        break;
                    case "2":
                        PartyTradePool(party);
                        break;
                    case "3":
                        GiftItem(party);
                        break;
                    case "4":
                        GiftGold(party);
                        break;
                    case "0":
                        Console.WriteLine("✅ Trade session ended.");
                        return;
                    default:
                        Console.WriteLine("❌ Invalid choice.");
                        break;
                }
            }
        }

        /// <summary>
        /// 1-on-1 trading between two party members
        /// </summary>
        private void DirectTrade(List<Character> party)
        {
            Console.WriteLine("\n━━━━━━━ DIRECT TRADE ━━━━━━━");

            // Select first trader
            Console.WriteLine("\nWho is offering items?");
            var trader1 = SelectCharacter(party);
            if (trader1 == null) return;

            // Select second trader
            Console.WriteLine("\nWho is receiving the offer?");
            var availableTraders = party.Where(p => p != trader1).ToList();
            var trader2 = SelectCharacter(availableTraders);
            if (trader2 == null) return;

            Console.WriteLine($"\n🤝 Trade initiated between {trader1.Name} and {trader2.Name}");

            // Trader 1's offer
            var offer1 = BuildTradeOffer(trader1, $"{trader1.Name}'s Offer");
            if (offer1 == null)
            {
                Console.WriteLine("❌ Trade cancelled.");
                return;
            }

            // Trader 2's offer
            var offer2 = BuildTradeOffer(trader2, $"{trader2.Name}'s Offer");
            if (offer2 == null)
            {
                Console.WriteLine("❌ Trade cancelled.");
                return;
            }

            // Show trade summary
            Console.WriteLine("\n╔══════════════ TRADE SUMMARY ══════════════╗");
            Console.WriteLine($"  {trader1.Name} offers:");
            if (offer1.Gold > 0) Console.WriteLine($"    💰 {offer1.Gold} gold");
            foreach (var item in offer1.Items)
            {
                Console.WriteLine($"    • {item.Name}");
            }

            Console.WriteLine($"\n  {trader2.Name} offers:");
            if (offer2.Gold > 0) Console.WriteLine($"    💰 {offer2.Gold} gold");
            foreach (var item in offer2.Items)
            {
                Console.WriteLine($"    • {item.Name}");
            }
            Console.WriteLine("╚═══════════════════════════════════════════╝");

            Console.Write("\nConfirm trade? (y/n): ");
            var confirm = Console.ReadLine()?.Trim().ToLower();

            if (confirm == "y" || confirm == "yes")
            {
                ExecuteTrade(trader1, trader2, offer1, offer2);
            }
            else
            {
                Console.WriteLine("❌ Trade cancelled.");
            }
        }

        /// <summary>
        /// Party-wide trade pool where everyone can contribute and take items
        /// </summary>
        private void PartyTradePool(List<Character> party)
        {
            Console.WriteLine("\n━━━━━━━ PARTY TRADE POOL ━━━━━━━");
            Console.WriteLine("All party members can contribute items to a shared pool,");
            Console.WriteLine("then everyone can claim items they need.");

            var tradePool = new List<Item>();
            var goldPool = 0;

            // Contribution phase
            Console.WriteLine("\n--- CONTRIBUTION PHASE ---");
            foreach (var member in party)
            {
                Console.WriteLine($"\n{member.Name}'s turn to contribute:");
                Console.WriteLine("1) Add items to pool");
                Console.WriteLine("2) Add gold to pool");
                Console.WriteLine("3) Skip contribution");
                Console.Write("Choice: ");

                var choice = Console.ReadLine()?.Trim();

                if (choice == "1")
                {
                    ContributeItemsToPool(member, tradePool);
                }
                else if (choice == "2")
                {
                    goldPool += ContributeGoldToPool(member);
                }
            }

            // Show pool contents
            Console.WriteLine("\n╔══════════════ TRADE POOL ══════════════╗");
            Console.WriteLine($"  💰 Total Gold: {goldPool}");
            Console.WriteLine($"  📦 Total Items: {tradePool.Count}");
            if (tradePool.Count > 0)
            {
                Console.WriteLine("\n  Items in pool:");
                for (int i = 0; i < tradePool.Count; i++)
                {
                    Console.WriteLine($"    {i + 1}) {tradePool[i].Name}");
                }
            }
            Console.WriteLine("╚════════════════════════════════════════╝");

            // Distribution phase
            if (tradePool.Count > 0 || goldPool > 0)
            {
                Console.WriteLine("\n--- DISTRIBUTION PHASE ---");
                foreach (var member in party)
                {
                    Console.WriteLine($"\n{member.Name}'s turn to claim:");
                    Console.WriteLine("1) Take items from pool");
                    Console.WriteLine("2) Take gold from pool");
                    Console.WriteLine("3) Skip");
                    Console.Write("Choice: ");

                    var choice = Console.ReadLine()?.Trim();

                    if (choice == "1")
                    {
                        ClaimItemsFromPool(member, tradePool);
                    }
                    else if (choice == "2")
                    {
                        goldPool -= ClaimGoldFromPool(member, goldPool);
                    }
                }

                // Distribute remaining gold evenly
                if (goldPool > 0)
                {
                    int goldPerMember = goldPool / party.Count;
                    foreach (var member in party)
                    {
                        member.Inventory.AddGold(goldPerMember);
                    }
                    Console.WriteLine($"\n💰 Remaining {goldPool} gold distributed evenly among party.");
                }

                // Remaining items notification
                if (tradePool.Count > 0)
                {
                    Console.WriteLine($"\n⚠️  {tradePool.Count} items remain unclaimed in the pool.");
                }
            }

            Console.WriteLine("\n✅ Trade pool session complete!");
        }

        /// <summary>
        /// Simple gift system - one character gives item to another
        /// </summary>
        private void GiftItem(List<Character> party)
        {
            Console.WriteLine("\n━━━━━━━ GIFT ITEM ━━━━━━━");

            Console.WriteLine("\nWho is giving the gift?");
            var giver = SelectCharacter(party);
            if (giver == null) return;

            Console.WriteLine("\nWho is receiving the gift?");
            var availableReceivers = party.Where(p => p != giver).ToList();
            var receiver = SelectCharacter(availableReceivers);
            if (receiver == null) return;

            // Show giver's inventory
            Console.WriteLine($"\n{giver.Name}'s Inventory:");
            DisplayInventory(giver);

            Console.Write("\nEnter slot number to gift (0 to cancel): ");
            if (!int.TryParse(Console.ReadLine(), out int slot) || slot <= 0)
            {
                Console.WriteLine("❌ Gift cancelled.");
                return;
            }

            slot--; // Convert to 0-based index

            if (slot < 0 || slot >= giver.Inventory.Slots.Count || giver.Inventory.Slots[slot] == null)
            {
                Console.WriteLine("❌ Invalid slot or empty slot.");
                return;
            }

            var item = giver.Inventory.Slots[slot];
            if (item == null) return;

            // Attempt transfer
            if (receiver.Inventory.AddItem(item))
            {
                giver.Inventory.RemoveItem(slot);
                Console.WriteLine($"✨ {giver.Name} gave {item.Name} to {receiver.Name}!");
            }
            else
            {
                Console.WriteLine($"❌ {receiver.Name}'s inventory is full!");
            }
        }

        /// <summary>
        /// Simple gift system - one character gives gold to another
        /// </summary>
        private void GiftGold(List<Character> party)
        {
            Console.WriteLine("\n━━━━━━━ GIFT GOLD ━━━━━━━");

            Console.WriteLine("\nWho is giving gold?");
            var giver = SelectCharacter(party);
            if (giver == null) return;

            Console.WriteLine("\nWho is receiving gold?");
            var availableReceivers = party.Where(p => p != giver).ToList();
            var receiver = SelectCharacter(availableReceivers);
            if (receiver == null) return;

            Console.WriteLine($"\n{giver.Name} has {giver.Inventory.Gold} gold.");
            Console.Write("How much gold to gift? ");

            if (!int.TryParse(Console.ReadLine(), out int amount) || amount <= 0)
            {
                Console.WriteLine("❌ Invalid amount.");
                return;
            }

            if (amount > giver.Inventory.Gold)
            {
                Console.WriteLine($"❌ {giver.Name} doesn't have that much gold!");
                return;
            }

            giver.Inventory.AddGold(-amount);
            receiver.Inventory.AddGold(amount);
            Console.WriteLine($"✨ {giver.Name} gave {amount} gold to {receiver.Name}!");
        }

        #region Helper Methods

        /// <summary>
        /// Select a character from a list
        /// </summary>
        private Character? SelectCharacter(List<Character> characters)
        {
            if (characters == null || characters.Count == 0)
            {
                Console.WriteLine("❌ No characters available.");
                return null;
            }

            for (int i = 0; i < characters.Count; i++)
            {
                var c = characters[i];
                Console.WriteLine($"{i + 1}) {c.Name} (Lv {c.Level}) - {c.Inventory.Gold}g");
            }

            Console.Write("\nSelect character (0 to cancel): ");
            if (!int.TryParse(Console.ReadLine(), out int choice) || choice <= 0 || choice > characters.Count)
            {
                return null;
            }

            return characters[choice - 1];
        }

        /// <summary>
        /// Build a trade offer from a character
        /// </summary>
        private TradeOffer? BuildTradeOffer(Character trader, string title)
        {
            var offer = new TradeOffer();

            Console.WriteLine($"\n━━━━━━━ {title} ━━━━━━━");
            Console.WriteLine("Build your trade offer:");

            while (true)
            {
                Console.WriteLine("\n1) Add item to offer");
                Console.WriteLine("2) Add gold to offer");
                Console.WriteLine("3) Review offer");
                Console.WriteLine("4) Confirm offer");
                Console.WriteLine("0) Cancel trade");
                Console.Write("Choice: ");

                var choice = Console.ReadLine()?.Trim();

                switch (choice)
                {
                    case "1":
                        AddItemToOffer(trader, offer);
                        break;
                    case "2":
                        AddGoldToOffer(trader, offer);
                        break;
                    case "3":
                        ReviewOffer(offer, trader.Name);
                        break;
                    case "4":
                        return offer;
                    case "0":
                        return null;
                    default:
                        Console.WriteLine("❌ Invalid choice.");
                        break;
                }
            }
        }

        /// <summary>
        /// Add item to trade offer
        /// </summary>
        private void AddItemToOffer(Character trader, TradeOffer offer)
        {
            Console.WriteLine($"\n{trader.Name}'s Inventory:");
            DisplayInventory(trader);

            Console.Write("\nEnter slot number to add (0 to cancel): ");
            if (!int.TryParse(Console.ReadLine(), out int slot) || slot <= 0)
            {
                return;
            }

            slot--; // Convert to 0-based index

            if (slot < 0 || slot >= trader.Inventory.Slots.Count || trader.Inventory.Slots[slot] == null)
            {
                Console.WriteLine("❌ Invalid slot or empty slot.");
                return;
            }

            var item = trader.Inventory.Slots[slot];
            if (item == null) return;

            if (offer.ItemSlots.Contains(slot))
            {
                Console.WriteLine("❌ Item already added to offer.");
                return;
            }

            offer.Items.Add(item);
            offer.ItemSlots.Add(slot);
            Console.WriteLine($"✅ Added {item.Name} to offer.");
        }

        /// <summary>
        /// Add gold to trade offer
        /// </summary>
        private void AddGoldToOffer(Character trader, TradeOffer offer)
        {
            Console.WriteLine($"\n{trader.Name} has {trader.Inventory.Gold} gold.");
            Console.WriteLine($"Current offer includes {offer.Gold} gold.");
            Console.Write("How much gold to add? ");

            if (!int.TryParse(Console.ReadLine(), out int amount) || amount <= 0)
            {
                Console.WriteLine("❌ Invalid amount.");
                return;
            }

            if (offer.Gold + amount > trader.Inventory.Gold)
            {
                Console.WriteLine($"❌ {trader.Name} doesn't have that much gold!");
                return;
            }

            offer.Gold += amount;
            Console.WriteLine($"✅ Added {amount} gold to offer (Total: {offer.Gold}g).");
        }

        /// <summary>
        /// Review current trade offer
        /// </summary>
        private void ReviewOffer(TradeOffer offer, string traderName)
        {
            Console.WriteLine($"\n--- {traderName}'s Current Offer ---");
            if (offer.Gold > 0)
            {
                Console.WriteLine($"💰 Gold: {offer.Gold}");
            }
            if (offer.Items.Count > 0)
            {
                Console.WriteLine("📦 Items:");
                foreach (var item in offer.Items)
                {
                    Console.WriteLine($"  • {item.Name}");
                }
            }
            if (offer.Gold == 0 && offer.Items.Count == 0)
            {
                Console.WriteLine("  (Empty offer)");
            }
        }

        /// <summary>
        /// Execute the trade between two characters
        /// </summary>
        private void ExecuteTrade(Character trader1, Character trader2, TradeOffer offer1, TradeOffer offer2)
        {
            // Validate trade is still possible
            if (offer1.Gold > trader1.Inventory.Gold || offer2.Gold > trader2.Inventory.Gold)
            {
                Console.WriteLine("❌ Trade failed: Insufficient gold!");
                return;
            }

            // Check inventory space
            int trader1NeedsSlots = offer2.Items.Count - offer1.Items.Count;
            int trader2NeedsSlots = offer1.Items.Count - offer2.Items.Count;

            int trader1FreeSlots = CountFreeSlots(trader1.Inventory);
            int trader2FreeSlots = CountFreeSlots(trader2.Inventory);

            if (trader1NeedsSlots > trader1FreeSlots || trader2NeedsSlots > trader2FreeSlots)
            {
                Console.WriteLine("❌ Trade failed: Insufficient inventory space!");
                return;
            }

            // Remove items from trader1
            foreach (int slot in offer1.ItemSlots.OrderByDescending(s => s))
            {
                trader1.Inventory.RemoveItem(slot);
            }

            // Remove items from trader2
            foreach (int slot in offer2.ItemSlots.OrderByDescending(s => s))
            {
                trader2.Inventory.RemoveItem(slot);
            }

            // Transfer gold
            if (offer1.Gold > 0)
            {
                trader1.Inventory.AddGold(-offer1.Gold);
                trader2.Inventory.AddGold(offer1.Gold);
            }
            if (offer2.Gold > 0)
            {
                trader2.Inventory.AddGold(-offer2.Gold);
                trader1.Inventory.AddGold(offer2.Gold);
            }

            // Transfer items
            foreach (var item in offer2.Items)
            {
                trader1.Inventory.AddItem(item);
            }
            foreach (var item in offer1.Items)
            {
                trader2.Inventory.AddItem(item);
            }

            Console.WriteLine("\n✅ Trade completed successfully!");
            Console.WriteLine($"🤝 {trader1.Name} and {trader2.Name} exchanged items!");
        }

        /// <summary>
        /// Contribute items to the party trade pool
        /// </summary>
        private void ContributeItemsToPool(Character member, List<Item> pool)
        {
            while (true)
            {
                Console.WriteLine($"\n{member.Name}'s Inventory:");
                DisplayInventory(member);

                Console.Write("\nEnter slot number to add to pool (0 to finish): ");
                if (!int.TryParse(Console.ReadLine(), out int slot) || slot <= 0)
                {
                    break;
                }

                slot--; // Convert to 0-based index

                if (slot < 0 || slot >= member.Inventory.Slots.Count || member.Inventory.Slots[slot] == null)
                {
                    Console.WriteLine("❌ Invalid slot or empty slot.");
                    continue;
                }

                var item = member.Inventory.Slots[slot];
                if (item == null) continue;

                pool.Add(item);
                member.Inventory.RemoveItem(slot);
                Console.WriteLine($"✅ {item.Name} added to pool.");
            }
        }

        /// <summary>
        /// Contribute gold to the party trade pool
        /// </summary>
        private int ContributeGoldToPool(Character member)
        {
            Console.WriteLine($"\n{member.Name} has {member.Inventory.Gold} gold.");
            Console.Write("How much to contribute? ");

            if (!int.TryParse(Console.ReadLine(), out int amount) || amount <= 0)
            {
                Console.WriteLine("❌ Invalid amount.");
                return 0;
            }

            if (amount > member.Inventory.Gold)
            {
                Console.WriteLine($"❌ {member.Name} doesn't have that much gold!");
                return 0;
            }

            member.Inventory.AddGold(-amount);
            Console.WriteLine($"✅ {member.Name} contributed {amount} gold to pool.");
            return amount;
        }

        /// <summary>
        /// Claim items from the party trade pool
        /// </summary>
        private void ClaimItemsFromPool(Character member, List<Item> pool)
        {
            while (pool.Count > 0)
            {
                Console.WriteLine("\nItems in pool:");
                for (int i = 0; i < pool.Count; i++)
                {
                    Console.WriteLine($"{i + 1}) {pool[i].Name}");
                }

                Console.Write("\nSelect item to take (0 to finish): ");
                if (!int.TryParse(Console.ReadLine(), out int choice) || choice <= 0)
                {
                    break;
                }

                choice--; // Convert to 0-based index

                if (choice < 0 || choice >= pool.Count)
                {
                    Console.WriteLine("❌ Invalid selection.");
                    continue;
                }

                var item = pool[choice];
                if (member.Inventory.AddItem(item))
                {
                    pool.RemoveAt(choice);
                    Console.WriteLine($"✅ {member.Name} took {item.Name}!");
                }
                else
                {
                    Console.WriteLine($"❌ {member.Name}'s inventory is full!");
                    break;
                }
            }
        }

        /// <summary>
        /// Claim gold from the party trade pool
        /// </summary>
        private int ClaimGoldFromPool(Character member, int availableGold)
        {
            Console.WriteLine($"\nPool has {availableGold} gold available.");
            Console.Write("How much to take? ");

            if (!int.TryParse(Console.ReadLine(), out int amount) || amount <= 0)
            {
                Console.WriteLine("❌ Invalid amount.");
                return 0;
            }

            if (amount > availableGold)
            {
                Console.WriteLine("❌ Not enough gold in pool!");
                return 0;
            }

            member.Inventory.AddGold(amount);
            Console.WriteLine($"✅ {member.Name} took {amount} gold from pool.");
            return amount;
        }

        /// <summary>
        /// Display character's inventory
        /// </summary>
        private void DisplayInventory(Character character)
        {
            Console.WriteLine($"Gold: {character.Inventory.Gold}g");
            Console.WriteLine($"Slots: {CountUsedSlots(character.Inventory)}/{character.Inventory.TotalSlots}");

            for (int i = 0; i < character.Inventory.Slots.Count; i++)
            {
                var item = character.Inventory.Slots[i];
                if (item != null)
                {
                    string itemDesc = item.Name;
                    if (item is Equipment eq)
                    {
                        itemDesc += $" (Price: {eq.Price}g)";
                    }
                    Console.WriteLine($"  [{i + 1}] {itemDesc}");
                }
                else
                {
                    Console.WriteLine($"  [{i + 1}] (Empty)");
                }
            }
        }

        /// <summary>
        /// Count free inventory slots
        /// </summary>
        private int CountFreeSlots(Inventory inventory)
        {
            int free = 0;
            foreach (var slot in inventory.Slots)
            {
                if (slot == null) free++;
            }
            return free;
        }

        /// <summary>
        /// Count used inventory slots
        /// </summary>
        private int CountUsedSlots(Inventory inventory)
        {
            int used = 0;
            foreach (var slot in inventory.Slots)
            {
                if (slot != null) used++;
            }
            return used;
        }

        #endregion
    }

    /// <summary>
    /// Represents a trade offer from one character
    /// </summary>
    internal class TradeOffer
    {
        public List<Item> Items { get; } = new List<Item>();
        public List<int> ItemSlots { get; } = new List<int>(); // Track original slot indices
        public int Gold { get; set; } = 0;
    }
}
