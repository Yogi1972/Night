using Night.Characters;
using Night.Shops;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rpg_Dungeon
{
    internal class Town
    {
        #region Fields

        private readonly Shops _shops;
        private readonly NorthSide _northSide;
        private readonly EastSide _eastSide;
        private readonly SouthSide _southSide;
        private readonly WestSide _westSide;
        private readonly CentralSquare _centralSquare;
        private readonly Trading _trading;

        #endregion

        #region Constructor

        public Town()
        {
            _shops = new Shops();
            _northSide = new NorthSide(_shops);
            _eastSide = new EastSide();
            _southSide = new SouthSide(_shops);
            _westSide = new WestSide(_shops);
            _centralSquare = new CentralSquare();
            _trading = new Trading();
        }

        #endregion

        #region Setup Methods

        public void SetTrackers(QuestBoard questBoard, BountyBoard bountyBoard, AchievementTracker achievementTracker)
        {
            _eastSide.SetTrackers(questBoard, bountyBoard);
            _southSide.SetAchievementTracker(achievementTracker);
        }

        public void SetJournal(Journal journal)
        {
            _eastSide.SetJournal(journal);
        }

        public void SetWeather(Weather weather)
        {
            _eastSide.SetWeather(weather);
            _centralSquare.SetWeather(weather);
        }

        public void SetTimeTracker(TimeOfDay timeTracker)
        {
            _eastSide.SetTimeTracker(timeTracker);
            _centralSquare.SetTimeTracker(timeTracker);
        }

        #endregion

        #region Town Interface

        public void EnterTown(List<Character> party, GameState? gameState = null)
        {
            if (party == null || party.Count == 0)
            {
                Console.WriteLine("You have no party to enter town with.");
                return;
            }

            while (true)
            {
                _centralSquare.ShowCentralSquare(party);

                // Show NPC count if GameState is available
                if (gameState?.NPCManager != null)
                {
                    var npcs = gameState.NPCManager.GetNPCsAtLocation("GreyWolf Town");
                    if (npcs.Count > 0)
                    {
                        Console.WriteLine($"   👥 {npcs.Count} NPCs are around town");
                    }
                }

                Console.WriteLine("\n--- Town Directions ---");
                Console.WriteLine("Where would you like to go?");
                Console.WriteLine("1) North Side (Crafters District)");
                Console.WriteLine("2) East Side (Service Quarter)");
                Console.WriteLine("3) South Side (Mystic Quarter)");
                Console.WriteLine("4) West Side (Entertainment District)");
                Console.WriteLine("9) Quest Board (East Side)");
                Console.WriteLine("10) Bounty Board (East Side)");
                Console.WriteLine("11) Pet Shop (West Side)");
                Console.WriteLine("12) Bank & Storage (East Side)");
                Console.WriteLine("13) The Lucky Dragon (Gambling - West Side)");
                Console.WriteLine("14) Hall of Fame (Achievements - South Side)");
                Console.WriteLine("5) Trade with Party Members");
                if (gameState?.NPCManager != null)
                {
                    Console.WriteLine("6) Talk to People Around Town 👥");
                }
                Console.WriteLine("0) Leave Town");
                Console.Write("Choice: ");

                var choice = Console.ReadLine() ?? string.Empty;

                switch (choice.Trim())
                {
                    case "1":
                        _northSide.EnterNorthSide(party);
                        break;
                    case "2":
                        _eastSide.EnterEastSide(party);
                        break;
                    case "3":
                        _southSide.EnterSouthSide(party);
                        break;
                    case "4":
                        _westSide.EnterWestSide(party);
                        break;
                    case "5":
                        _trading.OpenTradeMenu(party);
                        break;
                    case "9":
                        _eastSide.EnterEastSide(party);
                        break;
                    case "10":
                        _eastSide.EnterEastSide(party);
                        break;
                    case "11":
                        _westSide.EnterWestSide(party);
                        break;
                    case "12":
                        _eastSide.EnterEastSide(party);
                        break;
                    case "13":
                        _westSide.EnterWestSide(party);
                        break;
                    case "14":
                        _southSide.EnterSouthSide(party);
                        break;
                    case "6" when gameState?.NPCManager != null:
                        TalkToTownNPCs(party, gameState);
                        break;
                    case "0":
                        Console.WriteLine("\nYou leave the town and head back on the road.");
                        return;
                    default:
                        Console.WriteLine("Invalid choice. Please choose 0-6.");
                        break;
                }
            }
        }

        private void TalkToTownNPCs(List<Character> party, GameState gameState)
        {
            if (gameState.NPCManager == null || gameState.Journal == null || gameState.MainStoryline == null)
            {
                Console.WriteLine("NPC system not available.");
                Console.ReadKey();
                return;
            }

            var npcs = gameState.NPCManager.GetNPCsAtLocation("GreyWolf Town");

            if (npcs.Count == 0)
            {
                Console.WriteLine("\nThe town seems unusually quiet at the moment.");
                Console.ReadKey();
                return;
            }

            while (true)
            {
                Console.WriteLine($"\n╔══════════════════════════════════════════════╗");
                Console.WriteLine($"║        People of GreyWolf Town             ║");
                Console.WriteLine($"╚══════════════════════════════════════════════╝");

                for (int i = 0; i < npcs.Count; i++)
                {
                    var npc = npcs[i];
                    string questIndicator = npc.AvailableQuests.Count > 0 ? " ❗" : "";
                    string typeIcon = GetNPCTypeIcon(npc.Type);
                    Console.WriteLine($"{i + 1}) {typeIcon} {npc.Name} - {npc.Type}{questIndicator}");
                }

                Console.WriteLine("0) Back to Town Square");
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

        private string GetNPCTypeIcon(NPCType type)
        {
            return type switch
            {
                NPCType.Elder => "👴",
                NPCType.Guard => "🛡️",
                NPCType.Merchant => "💰",
                NPCType.Questgiver => "📜",
                NPCType.Informant => "🕵️",
                NPCType.Traveler => "🎒",
                NPCType.Citizen => "👤",
                _ => "👤"
            };
        }

        #endregion
    }
}
