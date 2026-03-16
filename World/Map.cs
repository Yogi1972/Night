using Night.Characters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rpg_Dungeon
{
    internal class Map
    {
        #region Properties

        private readonly List<Area> _areas;
        private Area? _currentArea;
        private readonly Weather _weather;
        private readonly TimeOfDay _timeTracker;
        private readonly WorldGenerator _worldGenerator;

        #endregion

        #region Constructor

        public Map(Weather weather, TimeOfDay timeTracker, int? worldSeed = null)
        {
            _worldGenerator = new WorldGenerator(worldSeed);
            _areas = new List<Area>();
            _weather = weather;
            _timeTracker = timeTracker;
            InitializeAreas();
        }

        #endregion

        #region Initialization

        private void InitializeAreas()
        {
            _areas.AddRange(_worldGenerator.GenerateAreas(_weather, _timeTracker));

            if (_areas.Count > 0)
            {
                _areas[0].IsUnlocked = true;
                _currentArea = _areas[0];
            }
        }

        #endregion

        #region Map Navigation

        public void OpenMap(List<Character> party, QuestBoard questBoard, BountyBoard bountyBoard,
                           AchievementTracker achievementTracker, Journal journal, MainStoryline mainStoryline,
                           NPCManager npcManager, FogOfWarMap fogOfWarMap)
        {
            while (true)
            {
                Console.WriteLine("\n╔════════════════════════════════════════╗");
                Console.WriteLine("║            World Map                  ║");
                Console.WriteLine("╚════════════════════════════════════════╝");

                // Calculate party level for unlocking areas
                int partyLevel = party.Max(p => p.Level);

                // Unlock areas based on party level
                foreach (var area in _areas)
                {
                    if (!area.IsUnlocked && partyLevel >= area.RecommendedLevel)
                    {
                        area.IsUnlocked = true;
                        Console.WriteLine($"\n🎉 NEW AREA UNLOCKED: {area.Name}!");
                    }
                }

                Console.WriteLine($"\n📍 Current Location: {_currentArea?.Name ?? "Town"}");
                Console.WriteLine($"\n=== Available Areas ===");

                for (int i = 0; i < _areas.Count; i++)
                {
                    var area = _areas[i];
                    string status = area.IsUnlocked ? "✓" : "🔒";
                    string current = area == _currentArea ? " <- YOU ARE HERE" : "";
                    string levelInfo = area.IsUnlocked ? $"(Lv {area.RecommendedLevel})" : $"[Requires Lv {area.RecommendedLevel}]";

                    Console.WriteLine($"{status} {i + 1}) {area.Name} {levelInfo}{current}");

                    if (area.IsUnlocked)
                    {
                        int dungeonCount = area.Dungeons.Count;
                        int completedDungeons = area.Dungeons.Count(d => d.IsCompleted);
                        Console.WriteLine($"      🏰 Dungeons: {completedDungeons}/{dungeonCount} | 📜 Quest Spots: {area.QuestSpots.Count}");
                    }
                }

                Console.WriteLine("\n=== Options ===");
                Console.WriteLine("T) Travel to Area");
                Console.WriteLine("V) View Area Details");
                Console.WriteLine("E) Explore Current Area");
                Console.WriteLine("M) View Fog of War Map 🗺️");
                Console.WriteLine("0) Return to Main Menu");
                Console.Write("Choose: ");

                var choice = Console.ReadLine() ?? string.Empty;

                switch (choice.Trim().ToUpper())
                {
                    case "T":
                        TravelToArea(party);
                        break;
                    case "V":
                        ViewAreaDetails();
                        break;
                    case "E":
                        if (_currentArea != null)
                        {
                            _currentArea.ExploreArea(party, questBoard, bountyBoard, achievementTracker, journal);
                        }
                        else
                        {
                            Console.WriteLine("You are currently in town. Select an area to travel to first.");
                        }
                        break;
                    case "M":
                        fogOfWarMap.DisplayMap(party, mainStoryline);
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Invalid choice.");
                        break;
                }
            }
        }

        private void TravelToArea(List<Character> party)
        {
            Console.WriteLine("\n=== Travel to Area ===");
            var unlockedAreas = _areas.Where(a => a.IsUnlocked).ToList();

            if (unlockedAreas.Count == 0)
            {
                Console.WriteLine("No areas unlocked yet!");
                return;
            }

            for (int i = 0; i < unlockedAreas.Count; i++)
            {
                var area = unlockedAreas[i];
                string current = area == _currentArea ? " <- Current" : "";
                Console.WriteLine($"{i + 1}) {area.Name} (Lv {area.RecommendedLevel}){current}");
            }

            Console.Write("\nTravel to which area (or 0 to cancel): ");
            var input = Console.ReadLine() ?? string.Empty;
            if (!int.TryParse(input, out var idx) || idx < 1 || idx > unlockedAreas.Count)
            {
                return;
            }

            var selectedArea = unlockedAreas[idx - 1];

            if (selectedArea == _currentArea)
            {
                Console.WriteLine("You are already in this area!");
                return;
            }

            // Travel with weather effects
            Console.WriteLine($"\n🚶 Traveling from {_currentArea?.Name ?? "Town"} to {selectedArea.Name}...");
            TravelManager.TravelBetweenAreas(party, _weather, _timeTracker);

            _currentArea = selectedArea;
            Console.WriteLine($"\n✨ Arrived at {selectedArea.Name}!");
            selectedArea.DisplayAreaInfo();
            Console.WriteLine("\nPress Enter to continue...");
            Console.ReadLine();
        }

        private void ViewAreaDetails()
        {
            Console.WriteLine("\n=== View Area Details ===");

            for (int i = 0; i < _areas.Count; i++)
            {
                var area = _areas[i];
                if (area.IsUnlocked)
                {
                    Console.WriteLine($"{i + 1}) {area.Name}");
                }
                else
                {
                    Console.WriteLine($"{i + 1}) 🔒 ???");
                }
            }

            Console.Write("\nView which area (or 0 to cancel): ");
            var input = Console.ReadLine() ?? string.Empty;
            if (!int.TryParse(input, out var idx) || idx < 1 || idx > _areas.Count)
            {
                return;
            }

            var selectedArea = _areas[idx - 1];

            if (!selectedArea.IsUnlocked)
            {
                Console.WriteLine($"\n🔒 This area is locked. Reach level {selectedArea.RecommendedLevel} to unlock it.");
                return;
            }

            selectedArea.DisplayAreaInfo();
            Console.WriteLine("\nPress Enter to continue...");
            Console.ReadLine();
        }

        #endregion

        #region Helper Methods

        public Area? GetCurrentArea()
        {
            return _currentArea;
        }

        public List<Area> GetAllAreas()
        {
            return _areas;
        }

        public void SetCurrentArea(Area area)
        {
            if (_areas.Contains(area))
            {
                _currentArea = area;
            }
        }

        #endregion
    }
}
