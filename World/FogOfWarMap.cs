using Night.Characters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rpg_Dungeon
{
    #region Map Node Class

    internal class MapNode
    {
        public string LocationName { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public LocationCategory Type { get; set; }
        public bool IsDiscovered { get; set; }
        public bool IsCurrentLocation { get; set; }
        public int RequiredLevel { get; set; }

        public MapNode(string name, int x, int y, LocationCategory type, int reqLevel, bool discovered = false)
        {
            LocationName = name;
            X = x;
            Y = y;
            Type = type;
            RequiredLevel = reqLevel;
            IsDiscovered = discovered;
            IsCurrentLocation = false;
        }

        public string GetMapIcon()
        {
            if (!IsDiscovered) return "█";

            if (IsCurrentLocation) return "★";

            return Type switch
            {
                LocationCategory.Town => "◈",
                LocationCategory.Settlement => "■",
                LocationCategory.Camp => "▲",
                LocationCategory.Dungeon => "☠",
                _ => "●"
            };
        }
    }

    #endregion

    #region Fog of War Map

    internal class FogOfWarMap
    {
        #region Fields

        private readonly List<MapNode> _mapNodes;
        private readonly int _mapWidth;
        private readonly int _mapHeight;
        private string _currentLocationName;

        #endregion

        #region Constructor

        public FogOfWarMap()
        {
            _mapWidth = 100;
            _mapHeight = 40;
            _mapNodes = new List<MapNode>();
            _currentLocationName = "Havenbrook";
            InitializeMapNodes();
        }

        #endregion

        #region Initialization

        private void InitializeMapNodes()
        {
            // === MAJOR TOWNS (8) - Strategic positions ===

            // Original 5 towns
            _mapNodes.Add(new MapNode("Havenbrook", 50, 20, LocationCategory.Town, 1, true)); // Center - Starting
            _mapNodes.Add(new MapNode("Ironforge Citadel", 25, 10, LocationCategory.Town, 10, false)); // Northwest Mountains
            _mapNodes.Add(new MapNode("Mysthaven", 75, 12, LocationCategory.Town, 15, false)); // Northeast Coast
            _mapNodes.Add(new MapNode("Sunspire", 80, 28, LocationCategory.Town, 20, false)); // Southeast Desert
            _mapNodes.Add(new MapNode("Shadowkeep", 20, 32, LocationCategory.Town, 25, false)); // Southwest Dark

            // New towns
            _mapNodes.Add(new MapNode("Crystalshore", 88, 15, LocationCategory.Town, 12, false)); // Far East Coast
            _mapNodes.Add(new MapNode("Emberpeak", 35, 35, LocationCategory.Town, 18, false)); // South Volcanic
            _mapNodes.Add(new MapNode("Stormwatch", 65, 5, LocationCategory.Town, 22, false)); // North Cliffs

            // === SETTLEMENTS (13) - Between major towns and strategic areas ===

            // Original 10 settlements
            _mapNodes.Add(new MapNode("Willowdale", 45, 23, LocationCategory.Settlement, 1, false));
            _mapNodes.Add(new MapNode("Crossroads Keep", 55, 16, LocationCategory.Settlement, 3, false));
            _mapNodes.Add(new MapNode("Pinewood", 38, 13, LocationCategory.Settlement, 5, false));
            _mapNodes.Add(new MapNode("Riverside", 60, 22, LocationCategory.Settlement, 5, false));
            _mapNodes.Add(new MapNode("Stonebridge", 32, 20, LocationCategory.Settlement, 8, false));
            _mapNodes.Add(new MapNode("Frosthollow", 22, 6, LocationCategory.Settlement, 10, false));
            _mapNodes.Add(new MapNode("Oasis Rest", 75, 25, LocationCategory.Settlement, 12, false));
            _mapNodes.Add(new MapNode("Moonwell", 68, 10, LocationCategory.Settlement, 15, false));
            _mapNodes.Add(new MapNode("Thornwall", 85, 20, LocationCategory.Settlement, 18, false));
            _mapNodes.Add(new MapNode("Ghostlight", 16, 26, LocationCategory.Settlement, 20, false));

            // New settlements
            _mapNodes.Add(new MapNode("Silvermist", 82, 18, LocationCategory.Settlement, 13, false)); // Near Crystalshore
            _mapNodes.Add(new MapNode("Copperhill", 28, 28, LocationCategory.Settlement, 16, false)); // Between Shadowkeep/Emberpeak
            _mapNodes.Add(new MapNode("Ravencrest", 58, 8, LocationCategory.Settlement, 24, false)); // Near Stormwatch

            // === CAMPS (23) - Scattered across the map ===

            // Roadside Camps (5)
            _mapNodes.Add(new MapNode("Traveler's Rest", 48, 18, LocationCategory.Camp, 1, false));
            _mapNodes.Add(new MapNode("Wagon Circle", 52, 22, LocationCategory.Camp, 1, false));
            _mapNodes.Add(new MapNode("Milestone Camp", 58, 18, LocationCategory.Camp, 1, false));
            _mapNodes.Add(new MapNode("Crossroads Camp", 54, 13, LocationCategory.Camp, 1, false));
            _mapNodes.Add(new MapNode("Guard Post", 40, 16, LocationCategory.Camp, 1, false));

            // Forest Camps (4)
            _mapNodes.Add(new MapNode("Hunter's Clearing", 35, 17, LocationCategory.Camp, 1, false));
            _mapNodes.Add(new MapNode("Woodcutter's Site", 41, 10, LocationCategory.Camp, 1, false));
            _mapNodes.Add(new MapNode("Druid Circle", 62, 16, LocationCategory.Camp, 1, false));
            _mapNodes.Add(new MapNode("Ranger Outpost", 70, 18, LocationCategory.Camp, 1, false));

            // Mountain Camps (4)
            _mapNodes.Add(new MapNode("Eagle's Nest", 27, 8, LocationCategory.Camp, 1, false));
            _mapNodes.Add(new MapNode("Cave Shelter", 21, 13, LocationCategory.Camp, 1, false));
            _mapNodes.Add(new MapNode("Mountain Pass Camp", 24, 5, LocationCategory.Camp, 1, false));
            _mapNodes.Add(new MapNode("Windswept Ridge", 30, 32, LocationCategory.Camp, 1, false));

            // Desert Camps (3)
            _mapNodes.Add(new MapNode("Dune Hollow", 84, 30, LocationCategory.Camp, 1, false));
            _mapNodes.Add(new MapNode("Nomad Circle", 78, 33, LocationCategory.Camp, 1, false));
            _mapNodes.Add(new MapNode("Rock Shelter", 86, 23, LocationCategory.Camp, 1, false));

            // Riverside/Coastal Camps (4)
            _mapNodes.Add(new MapNode("Fisher's Camp", 64, 24, LocationCategory.Camp, 1, false));
            _mapNodes.Add(new MapNode("Ferry Landing", 55, 25, LocationCategory.Camp, 1, false));
            _mapNodes.Add(new MapNode("Beaver Dam", 58, 28, LocationCategory.Camp, 1, false));
            _mapNodes.Add(new MapNode("Seaside Camp", 90, 14, LocationCategory.Camp, 1, false));

            // Ruins Camps (3)
            _mapNodes.Add(new MapNode("Temple Steps", 13, 28, LocationCategory.Camp, 1, false));
            _mapNodes.Add(new MapNode("Old Fort", 18, 35, LocationCategory.Camp, 1, false));
            _mapNodes.Add(new MapNode("Merchant's Rest", 68, 32, LocationCategory.Camp, 1, false));

            // === ENEMY CAMPS (20) - Hostile combat locations ===

            // Weak Enemy Camps (Level 1-5) - 4 camps
            _mapNodes.Add(new MapNode("Goblin Raiding Post", 42, 24, LocationCategory.Dungeon, 3, false));
            _mapNodes.Add(new MapNode("Bandit's Hollow", 47, 26, LocationCategory.Dungeon, 2, false));
            _mapNodes.Add(new MapNode("Wolf Den", 36, 19, LocationCategory.Dungeon, 3, false));
            _mapNodes.Add(new MapNode("Spider Hollow", 57, 24, LocationCategory.Dungeon, 4, false));

            // Normal Enemy Camps (Level 5-10) - 5 camps
            _mapNodes.Add(new MapNode("Blackwood Bandits", 33, 14, LocationCategory.Dungeon, 6, false));
            _mapNodes.Add(new MapNode("Goblin Warband Camp", 44, 28, LocationCategory.Dungeon, 7, false));
            _mapNodes.Add(new MapNode("Restless Graves", 26, 18, LocationCategory.Dungeon, 8, false));
            _mapNodes.Add(new MapNode("Bear's Territory", 39, 8, LocationCategory.Dungeon, 7, false));
            _mapNodes.Add(new MapNode("Abandoned Mine", 50, 12, LocationCategory.Dungeon, 8, false));

            // Strong Enemy Camps (Level 10-15) - 4 camps
            _mapNodes.Add(new MapNode("Orc War Camp", 30, 15, LocationCategory.Dungeon, 12, false));
            _mapNodes.Add(new MapNode("Dark Cult Altar", 24, 22, LocationCategory.Dungeon, 13, false));
            _mapNodes.Add(new MapNode("Haunted Battlefield", 15, 20, LocationCategory.Dungeon, 14, false));
            _mapNodes.Add(new MapNode("Troll's Bridge", 48, 30, LocationCategory.Dungeon, 12, false));

            // Dangerous Enemy Camps (Level 15-20) - 4 camps
            _mapNodes.Add(new MapNode("Ironpeak Orcs", 20, 8, LocationCategory.Dungeon, 17, false));
            _mapNodes.Add(new MapNode("Demon's Gate", 12, 30, LocationCategory.Dungeon, 18, false));
            _mapNodes.Add(new MapNode("Crimson Bandit Fort", 72, 28, LocationCategory.Dungeon, 16, false));
            _mapNodes.Add(new MapNode("Elemental Rift", 76, 16, LocationCategory.Dungeon, 19, false));

            // Deadly Enemy Camps (Level 20-25) - 2 camps
            _mapNodes.Add(new MapNode("Necropolis", 14, 34, LocationCategory.Dungeon, 22, false));
            _mapNodes.Add(new MapNode("Doomhammer Clan", 17, 12, LocationCategory.Dungeon, 23, false));

            // Elite Enemy Camps (Level 25+) - 1 camp
            _mapNodes.Add(new MapNode("Shadowflame Lair", 10, 38, LocationCategory.Dungeon, 27, false));
        }

        #endregion

        #region Map Display

        public void DisplayMap(List<Character> party, MainStoryline storyline)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("╔════════════════════════════════════════════════════════════════════════════════╗");
                Console.WriteLine("║                            🗺️  WORLD MAP                                      ║");
                Console.WriteLine("╚════════════════════════════════════════════════════════════════════════════════╝");

                // Update current location
                foreach (var node in _mapNodes)
                {
                    node.IsCurrentLocation = node.LocationName == _currentLocationName;
                }

                // Create the map grid
                char[,] mapGrid = new char[_mapHeight, _mapWidth];

                // First, fill with terrain features (unsettled areas)
                Random terrainRng = new Random(42); // Fixed seed for consistent terrain
                for (int y = 0; y < _mapHeight; y++)
                {
                    for (int x = 0; x < _mapWidth; x++)
                    {
                        // Create natural terrain patterns
                        mapGrid[y, x] = GenerateTerrainChar(x, y, terrainRng);
                    }
                }

                // Place discovered locations on top of terrain
                foreach (var node in _mapNodes.Where(n => n.IsDiscovered))
                {
                    if (node.Y < _mapHeight && node.X < _mapWidth)
                    {
                        mapGrid[node.Y, node.X] = node.GetMapIcon()[0];
                    }
                }

                // Draw the map
                Console.WriteLine("┌" + new string('─', _mapWidth) + "┐");
                for (int y = 0; y < _mapHeight; y++)
                {
                    Console.Write("│");
                    for (int x = 0; x < _mapWidth; x++)
                    {
                        Console.Write(mapGrid[y, x]);
                    }
                    Console.WriteLine("│");
                }
                Console.WriteLine("└" + new string('─', _mapWidth) + "┘");

                // Legend
                Console.WriteLine("\n📍 Legend:");
                Console.WriteLine("  ★ = Your Location  |  ◈ = Major Town  |  ■ = Settlement  |  ▲ = Camp  |  ☠ = Enemy Camp");
                Console.WriteLine("  ~ = Water  |  ^ = Mountains  |  ≋ = Desert  |  ░ = Forest  |  □ = Plains  |  █ = Undiscovered");

                Console.WriteLine($"\n📍 Current Location: {_currentLocationName}");

                int discoveredCount = _mapNodes.Count(n => n.IsDiscovered);
                int totalCount = _mapNodes.Count;
                Console.WriteLine($"🗺️  Exploration: {discoveredCount}/{totalCount} locations discovered ({(discoveredCount * 100 / totalCount)}%)");

                Console.WriteLine("\n--- Options ---");
                Console.WriteLine("L) List Discovered Locations");
                Console.WriteLine("S) View Story Objectives");
                Console.WriteLine("T) Tips for Exploration");
                Console.WriteLine("0) Close Map");
                Console.Write("\nChoice: ");

                var choice = Console.ReadLine()?.Trim().ToUpper() ?? "";

                switch (choice)
                {
                    case "L":
                        ListDiscoveredLocations();
                        break;
                    case "S":
                        storyline.DisplayCurrentObjective();
                        Console.WriteLine("\nPress Enter to continue...");
                        Console.ReadLine();
                        break;
                    case "T":
                        DisplayExplorationTips();
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Invalid choice.");
                        Console.WriteLine("Press Enter to continue...");
                        Console.ReadLine();
                        break;
                }
            }
        }

        private void ListDiscoveredLocations()
        {
            Console.WriteLine("\n╔════════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                    DISCOVERED LOCATIONS                           ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════════════╝");

            var discoveredNodes = _mapNodes.Where(n => n.IsDiscovered).OrderBy(n => n.Type).ThenBy(n => n.LocationName).ToList();

            var towns = discoveredNodes.Where(n => n.Type == LocationCategory.Town).ToList();
            var settlements = discoveredNodes.Where(n => n.Type == LocationCategory.Settlement).ToList();
            var camps = discoveredNodes.Where(n => n.Type == LocationCategory.Camp).ToList();

            if (towns.Count > 0)
            {
                Console.WriteLine("\n🏰 Major Towns:");
                foreach (var node in towns)
                {
                    string current = node.IsCurrentLocation ? " ⬅ YOU ARE HERE" : "";
                    Console.WriteLine($"   ◈ {node.LocationName} (Lv {node.RequiredLevel}){current}");
                }
            }

            if (settlements.Count > 0)
            {
                Console.WriteLine("\n🏘️  Settlements:");
                foreach (var node in settlements)
                {
                    string current = node.IsCurrentLocation ? " ⬅ YOU ARE HERE" : "";
                    Console.WriteLine($"   ■ {node.LocationName} (Lv {node.RequiredLevel}){current}");
                }
            }

            if (camps.Count > 0)
            {
                Console.WriteLine("\n⛺ Camps:");
                foreach (var node in camps)
                {
                    string current = node.IsCurrentLocation ? " ⬅ YOU ARE HERE" : "";
                    Console.WriteLine($"   ▲ {node.LocationName}{current}");
                }
            }

            Console.WriteLine("\n\nPress Enter to continue...");
            Console.ReadLine();
        }

        private void DisplayExplorationTips()
        {
            Console.WriteLine("\n╔════════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                    EXPLORATION TIPS                               ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════════════╝");
            Console.WriteLine();
            Console.WriteLine("💡 How to Discover New Locations:");
            Console.WriteLine("   • Travel using the World Map to unlock new areas");
            Console.WriteLine("   • Visit major towns to reveal nearby locations");
            Console.WriteLine("   • Complete story chapters to unlock distant regions");
            Console.WriteLine("   • Talk to NPCs for hints about undiscovered places");
            Console.WriteLine();
            Console.WriteLine("🎯 Strategic Exploration:");
            Console.WriteLine("   • Follow the main storyline to visit all major towns");
            Console.WriteLine("   • Each major town unlocks surrounding settlements and camps");
            Console.WriteLine("   • Higher level areas remain hidden until you're strong enough");
            Console.WriteLine();
            Console.WriteLine("⭐ Benefits of Exploration:");
            Console.WriteLine("   • Unlock new quests and NPCs");
            Console.WriteLine("   • Find unique shops and services");
            Console.WriteLine("   • Discover rare items and equipment");
            Console.WriteLine("   • Gain achievements for full exploration");

            Console.WriteLine("\n\nPress Enter to continue...");
            Console.ReadLine();
        }

        #endregion

        #region Map Updates

        public void DiscoverLocation(string locationName)
        {
            var node = _mapNodes.FirstOrDefault(n => n.LocationName == locationName);
            if (node != null && !node.IsDiscovered)
            {
                node.IsDiscovered = true;
                Console.WriteLine($"\n🗺️  {locationName} has been added to your map!");
            }
        }

        public void SetCurrentLocation(string locationName)
        {
            _currentLocationName = locationName;
        }

        public void RevealNearbyLocations(string centerLocation, int radius)
        {
            var centerNode = _mapNodes.FirstOrDefault(n => n.LocationName == centerLocation);
            if (centerNode == null) return;

            foreach (var node in _mapNodes)
            {
                if (node.IsDiscovered) continue;

                int distance = Math.Abs(node.X - centerNode.X) + Math.Abs(node.Y - centerNode.Y);
                if (distance <= radius)
                {
                    node.IsDiscovered = true;
                }
            }
        }

        public int GetDiscoveredCount() => _mapNodes.Count(n => n.IsDiscovered);

        public int GetTotalLocations() => _mapNodes.Count;

        #endregion

        #region Terrain Generation

        private char GenerateTerrainChar(int x, int y, Random rng)
        {
            // Create regions based on position
            // Northwest = Mountains (10-30x, 0-15y)
            if (x >= 10 && x <= 35 && y >= 0 && y <= 15)
            {
                return rng.Next(100) < 70 ? '^' : (rng.Next(100) < 50 ? '░' : ' ');
            }

            // Northeast = Coastal/Water (70-99x, 0-20y)
            if (x >= 70 && y <= 20)
            {
                return rng.Next(100) < 60 ? '~' : ' ';
            }

            // Southeast = Desert (70-99x, 21-39y)
            if (x >= 70 && y >= 21)
            {
                return rng.Next(100) < 65 ? '≋' : ' ';
            }

            // Southwest = Dark Forest (0-25x, 25-39y)
            if (x <= 25 && y >= 25)
            {
                return rng.Next(100) < 70 ? '░' : ' ';
            }

            // South-Central = Volcanic (25-45x, 30-39y)
            if (x >= 25 && x <= 45 && y >= 30)
            {
                return rng.Next(100) < 60 ? '▒' : ' ';
            }

            // North-Central = Cliffs/High Ground (55-75x, 0-10y)
            if (x >= 55 && x <= 75 && y <= 10)
            {
                return rng.Next(100) < 50 ? '^' : ' ';
            }

            // Central areas = Plains with some forests
            if (rng.Next(100) < 15)
            {
                return '░'; // Occasional forest
            }
            else if (rng.Next(100) < 10)
            {
                return '□'; // Plains marker
            }

            return ' '; // Open space
        }

        #endregion
    }

    #endregion
}
