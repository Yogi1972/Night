using Night.Characters;
using Night.Shops;
using System;
using System.Collections.Generic;

namespace Rpg_Dungeon
{
    #region Settlement Class

    internal class Settlement : Location
    {
        #region Properties

        public Inn Inn { get; }
        public bool HasShop { get; }
        public bool HasQuestBoard { get; }
        public SettlementShop? Shop { get; }

        #endregion

        #region Constructor

        public Settlement(string name, string description, int requiredLevel = 1, bool hasShop = false, bool hasQuestBoard = false)
            : base(name, description, LocationCategory.Settlement, requiredLevel)
        {
            Inn = new Inn(name);
            HasShop = hasShop;
            HasQuestBoard = hasQuestBoard;
            if (hasShop)
            {
                Shop = new SettlementShop(name);
            }
        }

        #endregion

        #region Methods

        public override void Enter(List<Character> party, GameState gameState)
        {
            if (!IsDiscovered)
            {
                IsDiscovered = true;
                Console.WriteLine($"\n✨ New location discovered: {Name}!");

                // Discover on fog of war map
                gameState.FogOfWarMap?.DiscoverLocation(Name);
                gameState.FogOfWarMap?.RevealNearbyLocations(Name, 8);
            }

            gameState.FogOfWarMap?.SetCurrentLocation(Name);

            while (true)
            {
                Console.Clear();
                Console.WriteLine($"\n╔══════════════════════════════════════════════╗");
                Console.WriteLine($"║        Welcome to {Name,-27}║");
                Console.WriteLine($"╚══════════════════════════════════════════════╝");
                Console.WriteLine($"🏘️  {Description}");

                // Show NPC count
                var npcs = gameState.NPCManager?.GetNPCsAtLocation(Name) ?? new List<NPC>();
                if (npcs.Count > 0)
                {
                    Console.WriteLine($"   👥 NPCs present: {npcs.Count}");
                }
                Console.WriteLine();

                if (gameState.Weather != null)
                {
                    Console.WriteLine($"Weather: {gameState.Weather.GetWeatherDescription()}");
                }
                if (gameState.TimeTracker != null)
                {
                    Console.WriteLine($"Time: {gameState.TimeTracker.GetTimeDescription()}");
                }

                Console.WriteLine("\n--- Settlement Services ---");
                Console.WriteLine("1) Visit Inn (Rest & Heal)");
                if (HasShop && Shop != null)
                {
                    Console.WriteLine("2) Visit Shop");
                }
                if (HasQuestBoard)
                {
                    Console.WriteLine("3) Check Quest Board");
                }
                if (npcs.Count > 0)
                {
                    Console.WriteLine("4) Talk to NPCs 👥");
                }
                Console.WriteLine("0) Leave Settlement");
                Console.Write("Choice: ");

                var choice = Console.ReadLine()?.Trim() ?? "";

                switch (choice)
                {
                    case "1":
                        Inn.VisitInn(party);
                        break;
                    case "2" when HasShop && Shop != null:
                        Shop.EnterShop(party[0]);
                        break;
                    case "3" when HasQuestBoard:
                        Console.WriteLine("Quest board functionality coming soon!");
                        Console.ReadKey();
                        break;
                    case "4" when npcs.Count > 0:
                        TalkToNPCs(party, gameState);
                        break;
                    case "0":
                        Console.WriteLine($"\nYou leave {Name}.");
                        return;
                    default:
                        Console.WriteLine("Invalid choice.");
                        Console.ReadKey();
                        break;
                }
            }
        }

        public override void DisplayInfo()
        {
            base.DisplayInfo();
            Console.WriteLine($"   Services: Inn" + (HasShop ? ", Shop" : "") + (HasQuestBoard ? ", Quest Board" : ""));
        }

        private void TalkToNPCs(List<Character> party, GameState gameState)
        {
            if (gameState.NPCManager == null || gameState.Journal == null || gameState.MainStoryline == null)
            {
                Console.WriteLine("NPC system not available.");
                return;
            }

            var npcs = gameState.NPCManager.GetNPCsAtLocation(Name);

            if (npcs.Count == 0)
            {
                Console.WriteLine("\nThere's no one around to talk to at the moment.");
                Console.ReadKey();
                return;
            }

            while (true)
            {
                Console.WriteLine($"\n╔══════════════════════════════════════════════╗");
                Console.WriteLine($"║        NPCs in {Name,-30}║");
                Console.WriteLine($"╚══════════════════════════════════════════════╝");

                for (int i = 0; i < npcs.Count; i++)
                {
                    var npc = npcs[i];
                    string questIndicator = npc.AvailableQuests.Count > 0 ? " ❗" : "";
                    Console.WriteLine($"{i + 1}) {npc.Name} - {npc.Type}{questIndicator}");
                }

                Console.WriteLine("0) Back");
                Console.Write("\nTalk to whom? ");

                var choice = Console.ReadLine()?.Trim() ?? "";
                if (choice == "0") return;

                if (int.TryParse(choice, out int npcIndex) && npcIndex > 0 && npcIndex <= npcs.Count)
                {
                    npcs[npcIndex - 1].Interact(party, gameState.Journal, gameState.MainStoryline);
                }
                else
                {
                    Console.WriteLine("Invalid choice.");
                    Console.ReadKey();
                }
            }
        }

        #endregion
    }

    #endregion

    #region Inn Class

    internal class Inn
    {
        private readonly string _settlementName;
        private static readonly Random _rng = new Random();

        public Inn(string settlementName)
        {
            _settlementName = settlementName;
        }

        public void VisitInn(List<Character> party)
        {
            Console.Clear();
            Console.WriteLine($"\n╔══════════════════════════════════════════════╗");
            Console.WriteLine($"║           {_settlementName} Inn                    ║");
            Console.WriteLine($"╚══════════════════════════════════════════════╝");
            Console.WriteLine("🛏️  A cozy inn with warm beds and hearty meals.\n");

            while (true)
            {
                Console.WriteLine("\n--- Inn Services ---");
                Console.WriteLine("1) Rest for the Night (50 gold per person) - Full HP/Mana/Stamina");
                Console.WriteLine("2) Quick Meal (15 gold per person) - Restore 50% HP");
                Console.WriteLine("3) Ale and Stories (5 gold) - Hear local rumors");
                Console.WriteLine("0) Leave Inn");
                Console.Write("Choice: ");

                var choice = Console.ReadLine()?.Trim() ?? "";

                switch (choice)
                {
                    case "1":
                        RestForNight(party);
                        break;
                    case "2":
                        QuickMeal(party);
                        break;
                    case "3":
                        HearRumors(party);
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Invalid choice.");
                        break;
                }
            }
        }

        private void RestForNight(List<Character> party)
        {
            int costPerPerson = 50;
            int totalCost = costPerPerson * party.Count;

            Console.WriteLine($"\nTotal cost: {totalCost} gold for {party.Count} party member(s).");
            Console.Write("Rest for the night? (y/n): ");

            if (Console.ReadLine()?.Trim().ToLower() != "y")
            {
                Console.WriteLine("Maybe another time.");
                return;
            }

            var player = party[0];
            if (player.Inventory.Gold < totalCost)
            {
                Console.WriteLine($"You need {totalCost} gold but only have {player.Inventory.Gold}.");
                Console.ReadKey();
                return;
            }

            player.Inventory.SpendGold(totalCost);

            Console.WriteLine("\n💤 The party rests through the night...");
            Console.WriteLine("You dream of adventures and distant lands...");

            foreach (var member in party)
            {
                int healthToRestore = member.GetTotalMaxHP() - member.Health;
                int manaToRestore = member.GetTotalMaxMana() - member.Mana;
                int staminaToRestore = member.GetTotalMaxStamina() - member.Stamina;

                member.Heal(healthToRestore);
                member.RestoreMana(manaToRestore);
                member.RestoreStamina(staminaToRestore);
            }

            Console.WriteLine("\n✨ The party is fully rested and restored!");
            Console.WriteLine("HP, Mana, and Stamina restored to maximum!");
            Console.ReadKey();
        }

        private void QuickMeal(List<Character> party)
        {
            int costPerPerson = 15;
            int totalCost = costPerPerson * party.Count;

            var player = party[0];
            if (player.Inventory.Gold < totalCost)
            {
                Console.WriteLine($"You need {totalCost} gold but only have {player.Inventory.Gold}.");
                Console.ReadKey();
                return;
            }

            player.Inventory.SpendGold(totalCost);

            Console.WriteLine("\n🍲 The innkeeper serves hot stew and fresh bread...");

            foreach (var member in party)
            {
                int healAmount = member.GetTotalMaxHP() / 2;
                member.Heal(healAmount);
                Console.WriteLine($"{member.Name} recovered {healAmount} HP!");
            }

            Console.ReadKey();
        }

        private void HearRumors(List<Character> party)
        {
            var player = party[0];
            if (player.Inventory.Gold < 5)
            {
                Console.WriteLine("You need 5 gold for a drink.");
                Console.ReadKey();
                return;
            }

            player.Inventory.SpendGold(5);

            string[] rumors = {
                "A traveler mentioned seeing strange lights in the northern mountains...",
                "The old hermit in the woods has rare herbs for sale, they say...",
                "I heard the bandits have been more active on the roads lately. Be careful!",
                "There's talk of ancient treasure buried in the desert ruins...",
                "Some adventurers never returned from the Shadowfen Marsh. Spooky!",
                "The blacksmith in the capital can forge legendary weapons... for a price.",
                "They say a dragon has been spotted near Frostpeak. Just rumors, I'm sure...",
                "Merchants are paying good coin for goblin ears these days.",
                "The guild is always looking for brave souls to take on dangerous quests.",
                "I wouldn't go near the graveyard after dark if I were you..."
            };

            Console.WriteLine($"\n🍺 The bartender pours you an ale and shares a rumor:");
            Console.WriteLine($"'{rumors[_rng.Next(rumors.Length)]}'");
            Console.ReadKey();
        }
    }

    #endregion

    #region Settlement Shop Class

    internal class SettlementShop
    {
        private readonly string _name;
        private readonly List<StockEntry<GenericItem>> _generalStock;

        public SettlementShop(string settlementName)
        {
            _name = $"{settlementName} General Store";
            _generalStock = new List<StockEntry<GenericItem>>
            {
                new StockEntry<GenericItem>(new GenericItem("Health Potion", 25), 25, 10),
                new StockEntry<GenericItem>(new GenericItem("Mana Potion", 30), 30, 8),
                new StockEntry<GenericItem>(new GenericItem("Bandages", 10), 10, 15),
                new StockEntry<GenericItem>(new GenericItem("Travel Rations", 15), 15, 20)
            };
        }

        public void EnterShop(Character player)
        {
            while (true)
            {
                Console.WriteLine($"\n🏪 Welcome to {_name}!");
                Console.WriteLine($"💰 Your Gold: {player.Inventory.Gold}");
                Console.WriteLine("\n--- Available Items ---");

                for (int i = 0; i < _generalStock.Count; i++)
                {
                    var entry = _generalStock[i];
                    Console.WriteLine($"{i + 1}) {entry.Item.Name} - {entry.Price} gold (Stock: {entry.Quantity})");
                }

                Console.WriteLine("0) Leave Shop");
                Console.Write("\nBuy which item? ");

                var choice = Console.ReadLine()?.Trim() ?? "";

                if (choice == "0")
                {
                    Console.WriteLine("Come back soon!");
                    return;
                }

                if (int.TryParse(choice, out int itemIndex) && itemIndex > 0 && itemIndex <= _generalStock.Count)
                {
                    var entry = _generalStock[itemIndex - 1];
                    if (entry.Quantity <= 0)
                    {
                        Console.WriteLine("Out of stock!");
                        Console.ReadKey();
                        continue;
                    }

                    if (player.Inventory.SpendGold(entry.Price))
                    {
                        player.Inventory.AddItem(new GenericItem(entry.Item.Name, entry.Price));
                        entry.Quantity--;
                        Console.WriteLine($"✅ Purchased {entry.Item.Name}!");
                    }
                    else
                    {
                        Console.WriteLine("Not enough gold!");
                    }
                    Console.ReadKey();
                }
                else
                {
                    Console.WriteLine("Invalid choice.");
                    Console.ReadKey();
                }
            }
        }
    }

    #endregion
}
