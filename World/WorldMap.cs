using Night.Characters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rpg_Dungeon
{
    #region World Map Manager

    internal class WorldMap
    {
        #region Properties

        private readonly List<Location> _allLocations;
        private readonly GameState _gameState;
        private readonly WorldGenerator _worldGenerator;

        #endregion

        #region Constructor

        public WorldMap(Weather weather, TimeOfDay timeTracker, int? worldSeed = null)
        {
            _worldGenerator = new WorldGenerator(worldSeed);
            _allLocations = new List<Location>();
            _gameState = new GameState
            {
                Weather = weather,
                TimeTracker = timeTracker,
                WorldSeed = _worldGenerator.Seed
            };
            InitializeWorld();
        }

        #endregion

        #region Initialization

        private void InitializeWorld()
        {
            Console.WriteLine($"\n🌍 Generating world with seed: {WorldGenerator.SeedToString(_worldGenerator.Seed)}");

            var majorTowns = _worldGenerator.GenerateMajorTowns();
            _allLocations.AddRange(majorTowns);

            var settlements = _worldGenerator.GenerateSettlements();
            _allLocations.AddRange(settlements);

            var camps = _worldGenerator.GenerateCamps();
            _allLocations.AddRange(camps);

            var enemyCamps = _worldGenerator.GenerateEnemyCamps();
            _allLocations.AddRange(enemyCamps);

            _worldGenerator.SetupDistances(_allLocations);

            Console.WriteLine($"✓ World generated: {majorTowns.Count} towns, {settlements.Count} settlements, {camps.Count} camps, {enemyCamps.Count} enemy camps");
        }


        #endregion

        #region Map Interface

        public void ShowWorldMap(List<Character> party)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("\n╔════════════════════════════════════════════════╗");
                Console.WriteLine("║              WORLD MAP                         ║");
                Console.WriteLine("╚════════════════════════════════════════════════╝");

                if (_gameState.Weather != null)
                {
                    Console.WriteLine($"🌤️  Current Weather: {_gameState.Weather.GetWeatherDescription()}");
                }
                if (_gameState.TimeTracker != null)
                {
                    Console.WriteLine($"🕐 Current Time: {_gameState.TimeTracker.GetTimeDescription()}");
                }

                Console.WriteLine("\n--- Filter Locations ---");
                Console.WriteLine("1) Show All Discovered Locations");
                Console.WriteLine("2) Show Major Towns");
                Console.WriteLine("3) Show Settlements");
                Console.WriteLine("4) Show Camps");
                Console.WriteLine("5) Show Enemy Camps ⚔️");
                Console.WriteLine("6) Travel to Location");
                Console.WriteLine("0) Close Map");
                Console.Write("Choice: ");

                var choice = Console.ReadLine()?.Trim() ?? "";

                switch (choice)
                {
                    case "1":
                        ShowAllLocations();
                        break;
                    case "2":
                        ShowLocationsByType(LocationCategory.Town);
                        break;
                    case "3":
                        ShowLocationsByType(LocationCategory.Settlement);
                        break;
                    case "4":
                        ShowLocationsByType(LocationCategory.Camp);
                        break;
                    case "5":
                        ShowEnemyCamps();
                        break;
                    case "6":
                        TravelToLocation(party);
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Invalid choice.");
                        Console.ReadKey();
                        break;
                }
            }
        }

        private void ShowAllLocations()
        {
            Console.Clear();
            Console.WriteLine("\n=== DISCOVERED LOCATIONS ===\n");

            var discovered = _allLocations.Where(l => l.IsDiscovered).OrderBy(l => l.Type).ThenBy(l => l.Name).ToList();

            if (discovered.Count == 0)
            {
                Console.WriteLine("No locations discovered yet!");
            }
            else
            {
                foreach (var location in discovered)
                {
                    location.DisplayInfo();
                }

                Console.WriteLine($"\n📍 Total: {discovered.Count}/{_allLocations.Count} locations discovered");
            }

            Console.ReadKey();
        }

        private void ShowLocationsByType(LocationCategory type)
        {
            Console.Clear();
            Console.WriteLine($"\n=== {type.ToString().ToUpper()}S ===\n");

            var locations = _allLocations.Where(l => l.Type == type && l.IsDiscovered).OrderBy(l => l.Name).ToList();

            if (locations.Count == 0)
            {
                Console.WriteLine($"No {type}s discovered yet!");
            }
            else
            {
                foreach (var location in locations)
                {
                    location.DisplayInfo();
                }
            }

            Console.ReadKey();
        }

        private void ShowEnemyCamps()
        {
            Console.Clear();
            Console.WriteLine("\n╔════════════════════════════════════════════════╗");
            Console.WriteLine("║            ⚔️ ENEMY CAMPS ⚔️                  ║");
            Console.WriteLine("╚════════════════════════════════════════════════╝\n");

            var enemyCamps = _allLocations
                .OfType<EnemyCamp>()
                .Where(ec => ec.IsDiscovered)
                .OrderBy(ec => ec.RequiredLevel)
                .ToList();

            if (enemyCamps.Count == 0)
            {
                Console.WriteLine("No enemy camps discovered yet!");
                Console.WriteLine("\n💡 Explore the world to find hostile camps you can assault.");
            }
            else
            {
                var cleared = enemyCamps.Count(ec => ec.IsCleared);
                Console.WriteLine($"Discovered: {enemyCamps.Count} | Cleared: {cleared} | Remaining: {enemyCamps.Count - cleared}\n");

                foreach (var camp in enemyCamps)
                {
                    camp.DisplayInfo();
                }
            }

            Console.ReadKey();
        }

        private void TravelToLocation(List<Character> party)
        {
            Console.Clear();
            Console.WriteLine("\n=== TRAVEL ===\n");

            var discovered = _allLocations.Where(l => l.IsDiscovered).OrderBy(l => l.Name).ToList();

            if (discovered.Count == 0)
            {
                Console.WriteLine("No locations available for travel!");
                Console.ReadKey();
                return;
            }

            for (int i = 0; i < discovered.Count; i++)
            {
                Console.WriteLine($"{i + 1}) {discovered[i].Name} ({discovered[i].Type})");
            }

            Console.WriteLine("0) Cancel");
            Console.Write("\nSelect destination: ");

            if (int.TryParse(Console.ReadLine(), out int choice) && choice > 0 && choice <= discovered.Count)
            {
                var destination = discovered[choice - 1];

                // Check level requirement
                if (party[0].Level < destination.RequiredLevel)
                {
                    Console.WriteLine($"\n⚠️  {destination.Name} requires level {destination.RequiredLevel}!");
                    Console.WriteLine($"   Your level: {party[0].Level}");
                    Console.ReadKey();
                    return;
                }

                // Simulate travel
                Console.WriteLine($"\n🗺️  Traveling to {destination.Name}...");

                if (_gameState.TimeTracker != null)
                {
                    _gameState.TimeTracker.AdvanceTime(2); // Travel takes time
                }

                Console.ReadKey();

                // Enter the location
                destination.Enter(party, _gameState);
            }
        }

        #endregion

        #region Helper Methods

        public void SetGameStateReferences(QuestBoard questBoard, BountyBoard bountyBoard,
                                          AchievementTracker achievementTracker, Journal journal,
                                          NPCManager npcManager, MainStoryline mainStoryline, FogOfWarMap fogOfWarMap)
        {
            _gameState.QuestBoard = questBoard;
            _gameState.BountyBoard = bountyBoard;
            _gameState.AchievementTracker = achievementTracker;
            _gameState.Journal = journal;
            _gameState.NPCManager = npcManager;
            _gameState.MainStoryline = mainStoryline;
            _gameState.FogOfWarMap = fogOfWarMap;
        }

        public void DiscoverLocation(string locationName)
        {
            var location = _allLocations.FirstOrDefault(l => l.Name == locationName);
            if (location != null)
            {
                location.IsDiscovered = true;
                _gameState.FogOfWarMap?.DiscoverLocation(locationName);
                Console.WriteLine($"\n✨ {locationName} has been discovered!");
            }
        }

        public int GetDiscoveredCount() => _allLocations.Count(l => l.IsDiscovered);

        public int GetTotalLocationCount() => _allLocations.Count;

        #endregion
    }

    #endregion
}
