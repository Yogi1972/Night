using Night.Characters;
using Night.Shops;
using System;
using System.Collections.Generic;

namespace Rpg_Dungeon
{
    #region Major Town Class

    internal class MajorTown : Location
    {
        #region Properties

        public string TownSpecialty { get; }
        public bool HasFullServices { get; }

        #endregion

        #region Constructor

        public MajorTown(string name, string description, string specialty, int requiredLevel = 1)
            : base(name, description, LocationCategory.Town, requiredLevel)
        {
            TownSpecialty = specialty;
            HasFullServices = true;
        }

        #endregion

        #region Methods

        public override void Enter(List<Character> party, GameState gameState)
        {
            if (!IsDiscovered)
            {
                IsDiscovered = true;
                Console.WriteLine($"\n✨ New town discovered: {Name}!");
                Console.WriteLine($"   Specialty: {TownSpecialty}");

                // Discover on fog of war map
                gameState.FogOfWarMap?.DiscoverLocation(Name);
                gameState.FogOfWarMap?.SetCurrentLocation(Name);
                gameState.FogOfWarMap?.RevealNearbyLocations(Name, 15);
            }

            // Check story progression
            gameState.MainStoryline?.CheckStoryProgression(Name, party, gameState.Journal);

            DisplayTownWelcome();

            while (true)
            {
                Console.WriteLine($"\n╔══════════════════════════════════════════════╗");
                Console.WriteLine($"║        {Name,-37}║");
                Console.WriteLine($"╚══════════════════════════════════════════════╝");
                Console.WriteLine($"🏰 {Description}");
                Console.WriteLine($"   Specialty: {TownSpecialty}");

                // Show NPC count
                var npcs = gameState.NPCManager?.GetNPCsAtLocation(Name) ?? new List<NPC>();
                if (npcs.Count > 0)
                {
                    Console.WriteLine($"   👥 NPCs in town: {npcs.Count}");
                }
                Console.WriteLine();

                Console.WriteLine("--- Town Services ---");
                Console.WriteLine("1) Visit Inn");
                Console.WriteLine("2) Visit Shops");
                Console.WriteLine("3) Visit Quest Board");
                Console.WriteLine("4) Visit Training Hall");
                Console.WriteLine("5) Visit Bank");
                Console.WriteLine("6) Explore Town Districts");
                Console.WriteLine("7) Talk to NPCs 👥");
                Console.WriteLine("8) 📜 View Town Lore");
                Console.WriteLine("0) Leave Town");
                Console.Write("Choice: ");

                var choice = Console.ReadLine()?.Trim() ?? "";

                switch (choice)
                {
                    case "1":
                        VisitInn(party);
                        break;
                    case "2":
                        VisitShops(party);
                        break;
                    case "3":
                        Console.WriteLine("\nQuest board services...");
                        Console.ReadKey();
                        break;
                    case "4":
                        Console.WriteLine("\nTraining hall services...");
                        Console.ReadKey();
                        break;
                    case "5":
                        Console.WriteLine("\nBank services...");
                        Console.ReadKey();
                        break;
                    case "6":
                        ExploreDistricts(party);
                        break;
                    case "7":
                        TalkToNPCs(party, gameState);
                        break;
                    case "8":
                        WorldLore.DisplayTownLore(Name);
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

        private void DisplayTownWelcome()
        {
            Console.Clear();
            Console.WriteLine("\n" + new string('═', 50));
            Console.WriteLine($"  Welcome to {Name}!");
            Console.WriteLine($"  {Description}");
            Console.WriteLine(new string('═', 50));
            Console.ReadKey();
        }

        private void VisitInn(List<Character> party)
        {
            var inn = new Inn(Name);
            inn.VisitInn(party);
        }

        private void VisitShops(List<Character> party)
        {
            Console.WriteLine($"\n🏪 {Name} Market District");
            Console.WriteLine("1) General Store");
            Console.WriteLine("2) Blacksmith");
            Console.WriteLine("3) Apothecary");
            Console.WriteLine("4) Specialty Shop");
            Console.WriteLine("0) Back");
            Console.Write("Choice: ");

            var choice = Console.ReadLine()?.Trim() ?? "";

            switch (choice)
            {
                case "1":
                case "2":
                case "3":
                case "4":
                    Console.WriteLine("\nShop functionality integrated with main town system.");
                    Console.ReadKey();
                    break;
            }
        }

        private void ExploreDistricts(List<Character> party)
        {
            Console.WriteLine($"\n🏛️ {Name} Districts");
            Console.WriteLine("Each district has unique shops and services!");
            Console.WriteLine($"This town specializes in: {TownSpecialty}");
            Console.ReadKey();
        }

        public override void DisplayInfo()
        {
            base.DisplayInfo();
            Console.WriteLine($"   Specialty: {TownSpecialty}");
            Console.WriteLine($"   Services: Full (Inn, Shops, Quest Board, Training, Bank)");
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
                    string storyIndicator = npc.IsStoryNPC ? " 📖" : "";
                    Console.WriteLine($"{i + 1}) {npc.Name} - {npc.Type}{questIndicator}{storyIndicator}");
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
}
