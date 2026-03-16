using Night.Characters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rpg_Dungeon
{
    // Room types for different dungeon encounters
    internal enum RoomType
    {
        Empty,          // Safe room, no encounter
        Combat,         // Regular mob fight
        Elite,          // Tougher enemy
        Treasure,       // Loot chest
        Boss,           // Floor boss
        Stairs          // Stairs to next level
    }

    // Location types for dungeon navigation
    internal enum LocationType
    {
        Room,           // Actual room with encounters
        Hallway,        // Corridor connecting rooms
        Intersection    // Hallway junction with multiple exits
    }

    // A single room in the dungeon
    internal class DungeonRoom
    {
        public RoomType Type { get; set; }
        public LocationType Location { get; set; }
        public bool Visited { get; set; }
        public bool Cleared { get; set; }
        public Dictionary<string, DungeonRoom?> Exits { get; } // North, South, East, West

        public DungeonRoom(RoomType type, LocationType location = LocationType.Room)
        {
            Type = type;
            Location = location;
            Visited = false;
            Cleared = false;
            Exits = new Dictionary<string, DungeonRoom?>
            {
                { "north", null },
                { "south", null },
                { "east", null },
                { "west", null }
            };
        }

        public string GetExitsDescription()
        {
            var exits = new List<string>();
            if (Exits["north"] != null) exits.Add("North");
            if (Exits["south"] != null) exits.Add("South");
            if (Exits["east"] != null) exits.Add("East");
            if (Exits["west"] != null) exits.Add("West");
            return exits.Count > 0 ? string.Join(", ", exits) : "None";
        }
    }

    // Floor in a multi-level dungeon
    internal class DungeonFloor
    {
        public int FloorNumber { get; }
        public DungeonRoom StartRoom { get; }
        public DungeonRoom? StairsDown { get; set; }
        public DungeonRoom? StairsUp { get; set; }
        public DungeonRoom? BossRoom { get; set; }
        public List<DungeonRoom> AllRooms { get; }

        public DungeonFloor(int floorNumber)
        {
            FloorNumber = floorNumber;
            AllRooms = new List<DungeonRoom>();
            StartRoom = new DungeonRoom(RoomType.Empty);
            AllRooms.Add(StartRoom);
        }
    }

    internal class Dungeon
    {
        private readonly int _totalFloors;
        private readonly Random _rng;
        private readonly DungeonGenerator _generator;
        private readonly List<DungeonFloor> _floors;
        private int _currentFloorIndex;
        private DungeonRoom _currentRoom;
        private QuestBoard? _questBoard;
        private BountyBoard? _bountyBoard;
        private AchievementTracker? _achievementTracker;
        private Journal? _journal;
        private readonly RandomEventManager _randomEvents;
        private readonly Trading _trading;

        public Dungeon(int floors = 3, int? seed = null)
        {
            _totalFloors = Math.Max(1, floors);
            _floors = new List<DungeonFloor>();

            int dungeonSeed = seed ?? (int)(DateTime.Now.Ticks & 0x7FFFFFFF);
            _rng = new Random(dungeonSeed);
            _generator = new DungeonGenerator(dungeonSeed);
            _randomEvents = new RandomEventManager();
            _trading = new Trading();

            for (int i = 0; i < _totalFloors; i++)
            {
                _floors.Add(_generator.GenerateFloor(i + 1, _totalFloors));
            }

            _currentFloorIndex = 0;
            _currentRoom = _floors[0].StartRoom;
        }


        private string GetHallwayDescription()
        {
            return _generator.GetHallwayDescription();
        }

        private string GetRoomAtmosphere(RoomType type, int floorNumber)
        {
            return _generator.GetRoomAtmosphere(type, floorNumber);
        }

        // Main exploration loop
        public void Explore(List<Character> party, Journal? journal = null, QuestBoard? questBoard = null, BountyBoard? bountyBoard = null, AchievementTracker? achievementTracker = null)
        {
            if (party == null || party.Count == 0)
            {
                Console.WriteLine("No party to explore the dungeon.");
                return;
            }

            // Store trackers for use during dungeon
            _journal = journal;
            _questBoard = questBoard;
            _bountyBoard = bountyBoard;
            _achievementTracker = achievementTracker;

            int totalLevel = 0;
            int aliveCount = 0;
            foreach (var p in party)
            {
                if (p.IsAlive)
                {
                    totalLevel += p.Level;
                    aliveCount++;
                }
            }
            int partyAvgLevel = aliveCount > 0 ? totalLevel / aliveCount : 0;
            Console.WriteLine($"╔═══════════════════════════════════════╗");
            Console.WriteLine($"║   ENTERING DUNGEON: {_totalFloors} FLOORS          ║");
            Console.WriteLine($"║   Party Average Level: {partyAvgLevel,-2}            ║");
            Console.WriteLine($"╚═══════════════════════════════════════╝");

            _currentFloorIndex = 0;
            _currentRoom = _floors[0].StartRoom;
            _currentRoom.Visited = true;

            bool exploring = true;
            while (exploring)
            {
                bool anyAlive = false;
                foreach (var p in party)
                {
                    if (p.IsAlive)
                    {
                        anyAlive = true;
                        break;
                    }
                }
                if (!anyAlive)
                {
                    Console.WriteLine("\n💀 Your entire party has been defeated!");
                    return;
                }

                // Show current status
                DisplayRoomStatus();

                // Handle room encounter
                if (!_currentRoom.Cleared)
                {
                    bool encounterResult = HandleRoomEncounter(party, partyAvgLevel);
                    if (!encounterResult)
                    {
                        Console.WriteLine("\n💀 The party has been defeated!");
                        return;
                    }
                    _currentRoom.Cleared = true;

                    // Chance for random event after clearing room (25%)
                    _randomEvents.TriggerRandomEvent(party, 25);
                }

                // Show navigation options
                Console.WriteLine("\n━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
                Console.WriteLine("What would you like to do?");
                Console.WriteLine("  [N/S/E/W] - Move in a direction");
                Console.WriteLine("  [M] - Show map");
                Console.WriteLine("  [P] - View party status");
                Console.WriteLine("  [T] - Trade with party members");
                Console.WriteLine("  [L] - Leave dungeon");
                Console.Write("\nChoice: ");

                var input = (Console.ReadLine() ?? "").Trim().ToLower();

                switch (input)
                {
                    case "n":
                    case "north":
                        MoveDirection("north");
                        break;
                    case "s":
                    case "south":
                        MoveDirection("south");
                        break;
                    case "e":
                    case "east":
                        MoveDirection("east");
                        break;
                    case "w":
                    case "west":
                        MoveDirection("west");
                        break;
                    case "m":
                    case "map":
                        ShowMap();
                        break;
                    case "p":
                    case "party":
                        ShowPartyStatus(party);
                        break;
                    case "t":
                    case "trade":
                        _trading.OpenTradeMenu(party);
                        break;
                    case "l":
                    case "leave":
                        Console.WriteLine("\n🏃 The party leaves the dungeon.");
                        exploring = false;
                        break;
                    default:
                        Console.WriteLine("❌ Invalid command.");
                        break;
                }

                // Check for dungeon completion
                if (_currentFloorIndex >= _totalFloors - 1 &&
                    _floors[_currentFloorIndex].BossRoom?.Cleared == true)
                {
                    Console.WriteLine("\n╔═══════════════════════════════════════╗");
                    Console.WriteLine("║  🎉 DUNGEON CONQUERED! 🎉            ║");
                    Console.WriteLine("║  The party emerges victorious!       ║");
                    Console.WriteLine("╚═══════════════════════════════════════╝");

                    // Track dungeon completion achievement
                    _achievementTracker?.TrackDungeonComplete();

                    exploring = false;
                }
            }
        }

        private void DisplayRoomStatus()
        {
            var floor = _floors[_currentFloorIndex];
            Console.WriteLine("\n╔═══════════════════════════════════════╗");
            Console.WriteLine($"║  Floor {floor.FloorNumber}/{_totalFloors}                              ║");

            // Show location type
            string locationType = _currentRoom.Location switch
            {
                LocationType.Room => "Room",
                LocationType.Hallway => "Hallway",
                LocationType.Intersection => "Intersection",
                _ => "Unknown"
            };
            Console.WriteLine($"║  Location: {locationType,-27}║");

            string roomDesc = GetRoomDescription(_currentRoom);
            Console.WriteLine($"║  {roomDesc,-37}║");

            // Show exploration status
            string explorationStatus = _currentRoom.Visited ? "Explored" : "First Visit";
            Console.WriteLine($"║  Status: {explorationStatus,-29}║");

            if (_currentRoom.Cleared)
            {
                Console.WriteLine("║  ✓ Room Cleared                       ║");
            }

            Console.WriteLine($"║  Exits: {_currentRoom.GetExitsDescription(),-28}║");
            Console.WriteLine("╚═══════════════════════════════════════╝");
        }

        private string GetRoomDescription(DungeonRoom room)
        {
            if (!room.Visited)
                return "Unknown Area (Not yet explored)";

            // Hallways have simpler descriptions
            if (room.Location == LocationType.Hallway)
            {
                int exitCount = 0;
                foreach (var e in room.Exits)
                {
                    if (e.Value != null)
                        exitCount++;
                }

                if (exitCount > 2)
                    return "🔀 Corridor Junction";

                var hallwayTypes = new[]
                {
                    "🚶 Narrow Corridor",
                    "🕯️ Torch-lit Passage",
                    "🌫️ Misty Hallway",
                    "🪨 Stone Corridor"
                };
                return hallwayTypes[_rng.Next(hallwayTypes.Length)];
            }

            if (room.Location == LocationType.Intersection)
            {
                return "🔀 Crossroads";
            }

            // Room descriptions with icons
            return room.Type switch
            {
                RoomType.Empty => "🛡️ Safe Chamber",
                RoomType.Combat => "💀 Monster Den",
                RoomType.Elite => "⚔️  Champion's Arena",
                RoomType.Treasure => "💎 Treasure Vault",
                RoomType.Boss => "👹 BOSS CHAMBER",
                RoomType.Stairs => "🪜 Descent",
                _ => "Unknown"
            };
        }

        private bool HandleRoomEncounter(List<Character> party, int partyAvgLevel)
        {
            var floor = _floors[_currentFloorIndex];

            // Hallways have atmospheric descriptions
            if (_currentRoom.Location == LocationType.Hallway)
            {
                Console.WriteLine($"\n🚶 {GetHallwayDescription()}");
                return true;
            }

            if (_currentRoom.Location == LocationType.Intersection)
            {
                Console.WriteLine("\n🔀 You reach a crossroads. Multiple passages branch off in different directions.");
                int exitCount = 0;
                foreach (var e in _currentRoom.Exits)
                {
                    if (e.Value != null)
                        exitCount++;
                }
                Console.WriteLine($"You can travel in {exitCount} directions from here.");
                return true;
            }

            // Show atmospheric description for rooms
            string atmosphere = GetRoomAtmosphere(_currentRoom.Type, floor.FloorNumber);
            Console.WriteLine($"\n{atmosphere}");
            Console.WriteLine();

            switch (_currentRoom.Type)
            {
                case RoomType.Empty:
                    Console.WriteLine("✨ You take a moment to rest and assess your surroundings.");
                    return true;

                case RoomType.Combat:
                    Console.WriteLine("⚔️  An enemy appears from the shadows!");
                    var mob = Mobs.GetRandomMobForParty(party, floor.FloorNumber);
                    return HandleCombat(party, mob, partyAvgLevel);

                case RoomType.Elite:
                    Console.WriteLine("⚔️  A POWERFUL ELITE blocks your path!");
                    var eliteMob = Mobs.GetRandomMobForParty(party, floor.FloorNumber + 2);
                    // Elite has higher stats via higher level scaling
                    return HandleCombat(party, eliteMob, partyAvgLevel);

                case RoomType.Treasure:
                    Console.WriteLine("💎 Your eyes widen at the sight of treasure!");
                    HandleTreasure(party);
                    return true;

                case RoomType.Boss:
                    Console.WriteLine("👹 The air grows heavy with dread. The floor guardian emerges!");
                    var boss = Mobs.GetRandomMobForParty(party, floor.FloorNumber + 5); // Bosses are much higher level
                    Console.WriteLine($"💀 {boss.Name} - The Floor Guardian! (HP: {boss.Health})");
                    return HandleCombat(party, boss, partyAvgLevel, isBoss: true);

                case RoomType.Stairs:
                    // Check if boss has been defeated before allowing descent
                    var currentFloor = _floors[_currentFloorIndex];
                    if (currentFloor.BossRoom != null && !currentFloor.BossRoom.Cleared)
                    {
                        Console.WriteLine("🚫 A shimmering magical barrier blocks the stairs!");
                        Console.WriteLine("The guardian's magic seals this passage. Defeat the floor boss to proceed.");
                        return true;
                    }

                    Console.WriteLine("🪜 The path downward is now clear!");
                    Console.WriteLine("Press Enter to descend to the next floor...");
                    Console.ReadLine();
                    DescendStairs();
                    return true;

                default:
                    return true;
            }
        }

        private bool HandleCombat(List<Character> party, Mob mob, int partyAvgLevel, bool isBoss = false)
        {
            string rank = Playerleveling.GetMobRankTitle(mob.Level, partyAvgLevel);
            Console.WriteLine($"Level {mob.Level} {mob.Name} {rank}");
            Console.WriteLine($"HP: {mob.Health} | STR: {mob.Strength} | AGI: {mob.Agility}");

            bool victory = CombatSystem.RunEncounter(party, mob);

            if (victory)
            {
                // Track quest progress (kill quests) in both places for compatibility
                _questBoard?.UpdateQuestProgress(mob.Name);
                _journal?.UpdateQuestProgress(mob.Name);

                // Check bounty completion in both places for compatibility
                _bountyBoard?.CheckBountyCompletion(mob.Name, _journal);
                _journal?.CheckBountyCompletion(mob.Name);

                // Track achievements
                _achievementTracker?.TrackCombatKill(mob.Name);

                if (isBoss)
                {
                    Console.WriteLine("\n🎉 BOSS DEFEATED! You found rare loot!");
                    // Bonus treasure for boss kills
                    HandleTreasure(party);
                }
            }

            return victory;
        }

        private void HandleTreasure(List<Character> party)
        {
            var treasureDescriptions = new[]
            {
                "You pry open the chest with anticipation...",
                "The lock gives way and the chest creaks open...",
                "With careful hands, you unlock the treasure...",
                "The chest bursts open in a shower of golden light!"
            };
            Console.WriteLine(treasureDescriptions[_rng.Next(treasureDescriptions.Length)]);

            int goldAmount = _rng.Next(50, 200) * (_currentFloorIndex + 1);
            Console.WriteLine($"💰 The party found {goldAmount} gold pieces!");

            // Distribute gold evenly
            int goldPerMember = goldAmount / party.Count;
            foreach (var member in party)
            {
                member.Inventory.AddGold(goldPerMember);
            }

            // Track treasure chest opened
            _achievementTracker?.TrackChestOpened();

            // Random chance for item
            if (_rng.Next(100) < 60)
            {
                // Give random loot item to a random party member
                var randomMember = party[_rng.Next(party.Count)];
                var mob = Mobs.GetRandomMobForParty(party, _currentFloorIndex + 1);
                var lootResult = mob.DropLoot(_rng);

                if (lootResult.Items.Count > 0)
                {
                    var item = lootResult.Items[_rng.Next(lootResult.Items.Count)];
                    randomMember.Inventory.AddItem(item);
                    var foundDescriptions = new[]
                    {
                        $"✨ {randomMember.Name} discovers a {item.Name} hidden among the treasure!",
                        $"🎁 {randomMember.Name} finds a valuable {item.Name}!",
                        $"⭐ {randomMember.Name} uncovers a {item.Name}!"
                    };
                    Console.WriteLine(foundDescriptions[_rng.Next(foundDescriptions.Length)]);
                }
            }

            Console.WriteLine("\nPress Enter to continue...");
            Console.ReadLine();
        }

        private void MoveDirection(string direction)
        {
            if (_currentRoom.Exits[direction] != null)
            {
                var previousLocation = _currentRoom.Location;
                _currentRoom = _currentRoom.Exits[direction]!;

                // Check if this is first visit for special message
                bool firstVisit = !_currentRoom.Visited;
                _currentRoom.Visited = true;

                // Descriptive movement messages based on location types
                string movementMsg = (_currentRoom.Location, previousLocation) switch
                {
                    (LocationType.Hallway, LocationType.Room) => $"🚶 You leave the room and enter a {(firstVisit ? "dark" : "familiar")} corridor heading {direction}...",
                    (LocationType.Room, LocationType.Hallway) => $"🚪 You emerge from the corridor into a {(firstVisit ? "new" : "previously explored")} chamber to the {direction}...",
                    (LocationType.Hallway, LocationType.Hallway) => $"🚶 You continue down the winding hallway {direction}...",
                    (LocationType.Intersection, LocationType.Hallway) => $"🔀 The corridor opens into a crossroads to the {direction}...",
                    (LocationType.Hallway, LocationType.Intersection) => $"🚶 You choose a path and head {direction}...",
                    (LocationType.Room, LocationType.Room) => $"🚪 You pass through a doorway {direction} into another chamber...",
                    _ => $"🚶 Moving {direction}..."
                };

                Console.WriteLine($"\n{movementMsg}");

                // Add first visit flavor
                if (firstVisit && _currentRoom.Location == LocationType.Hallway)
                {
                    Console.WriteLine(GetHallwayDescription());
                }
            }
            else
            {
                var wallDescriptions = new[]
                {
                    "A solid stone wall blocks your path.",
                    "The way is sealed. You cannot go this way.",
                    "There is no passage in that direction.",
                    "A dead end. You must turn back."
                };
                Console.WriteLine($"\n❌ {wallDescriptions[_rng.Next(wallDescriptions.Length)]}");
            }
        }

        private void DescendStairs()
        {
            if (_currentFloorIndex < _totalFloors - 1)
            {
                _currentFloorIndex++;
                _currentRoom = _floors[_currentFloorIndex].StartRoom;
                _currentRoom.Visited = true;

                var descentDescriptions = new[]
                {
                    "The stairs creak under your weight as you descend into darkness.",
                    "You carefully navigate the worn steps, going deeper underground.",
                    "The air grows colder as you descend to the next level.",
                    "Your torches illuminate the way down into the depths."
                };
                Console.WriteLine($"\n⬇️  {descentDescriptions[_rng.Next(descentDescriptions.Length)]}");
                Console.WriteLine($"You have reached Floor {_currentFloorIndex + 1}!");
                Console.WriteLine("The dungeon grows more dangerous the deeper you go...");
            }
            else
            {
                Console.WriteLine("\n❌ The stairs lead nowhere. This is the deepest floor.");
            }
        }

        private void ShowMap()
        {
            var floor = _floors[_currentFloorIndex];
            Console.WriteLine("\n╔═══════════════════════════════ FLOOR MAP ════════════════════════════════╗");
            Console.WriteLine($"  Floor {floor.FloorNumber}/{_totalFloors}");
            Console.WriteLine("  Legend: [X]=You  [✓]=Cleared  [~]=Hall  [B]=Boss  [S]=Stairs  [T]=Treasure");
            Console.WriteLine("══════════════════════════════════════════════════════════════════════════════");
            Console.WriteLine();

            // Build coordinate map by traversing from start room
            var roomCoords = new Dictionary<DungeonRoom, (int x, int y)>();
            BuildRoomCoordinates(floor.StartRoom, 0, 0, roomCoords, new HashSet<DungeonRoom>());

            // Find bounds
            int minX = int.MaxValue, maxX = int.MinValue;
            int minY = int.MaxValue, maxY = int.MinValue;

            foreach (var coord in roomCoords.Values)
            {
                if (coord.x < minX) minX = coord.x;
                if (coord.x > maxX) maxX = coord.x;
                if (coord.y < minY) minY = coord.y;
                if (coord.y > maxY) maxY = coord.y;
            }

            int width = maxX - minX + 1;
            int height = maxY - minY + 1;

            // Create two arrays: one for player position, one for cleared status
            bool[,] playerPositionArray = new bool[width, height];
            bool[,] clearedStatusArray = new bool[width, height];
            DungeonRoom?[,] roomArray = new DungeonRoom[width, height];

            // Fill arrays
            foreach (var kvp in roomCoords)
            {
                var room = kvp.Key;
                int gridX = kvp.Value.x - minX;
                int gridY = kvp.Value.y - minY;

                if (!room.Visited) continue;

                roomArray[gridX, gridY] = room;
                clearedStatusArray[gridX, gridY] = room.Cleared;
                playerPositionArray[gridX, gridY] = (room == _currentRoom);
            }

            // Draw grid using the arrays
            for (int y = 0; y < height; y++)
            {
                // Draw horizontal connections
                for (int x = 0; x < width; x++)
                {
                    var room = roomArray[x, y];
                    if (room != null)
                    {
                        Console.Write("  ");
                        var eastRoom = (x + 1 < width) ? roomArray[x + 1, y] : null;
                        if (room.Exits["east"] == eastRoom && eastRoom != null)
                            Console.Write("──");
                        else
                            Console.Write("  ");
                    }
                    else
                    {
                        Console.Write("      ");
                    }
                }
                Console.WriteLine();

                // Draw rooms using both arrays
                for (int x = 0; x < width; x++)
                {
                    var room = roomArray[x, y];
                    if (room != null)
                    {
                        bool isPlayerHere = playerPositionArray[x, y];
                        bool isCleared = clearedStatusArray[x, y];
                        string symbol = GetRoomSymbolFromArrays(room, isPlayerHere, isCleared);
                        Console.Write($" {symbol} ");
                    }
                    else
                    {
                        Console.Write("     ");
                    }
                }
                Console.WriteLine();

                // Draw vertical connections
                for (int x = 0; x < width; x++)
                {
                    var room = roomArray[x, y];
                    if (room != null)
                    {
                        var southRoom = (y + 1 < height) ? roomArray[x, y + 1] : null;
                        if (room.Exits["south"] == southRoom && southRoom != null)
                            Console.Write("  │  ");
                        else
                            Console.Write("     ");
                    }
                    else
                    {
                        Console.Write("     ");
                    }
                }
                Console.WriteLine();
            }

            Console.WriteLine();
            Console.WriteLine("══════════════════════════════════════════════════════════════════════════════");
            Console.WriteLine("  📍 PARTY LOCATION:");
            Console.WriteLine($"     {GetRoomSymbol(_currentRoom)} {GetRoomDescription(_currentRoom)}");
            Console.WriteLine($"     Exits: {_currentRoom.GetExitsDescription()}");
            Console.WriteLine("╚══════════════════════════════════════════════════════════════════════════════╝");

            // Show options to enter rooms
            Console.WriteLine("\n--- Map Options ---");
            Console.WriteLine("E) Enter/Select a Room");
            Console.WriteLine("0) Close Map");
            Console.Write("\nChoice: ");

            var choice = Console.ReadLine()?.Trim().ToUpper() ?? "";

            if (choice == "E")
            {
                SelectRoomFromMap(roomCoords);
            }
        }

        private void BuildRoomCoordinates(DungeonRoom room, int x, int y, Dictionary<DungeonRoom, (int x, int y)> coords, HashSet<DungeonRoom> visited)
        {
            if (visited.Contains(room) || coords.ContainsKey(room))
                return;

            coords[room] = (x, y);
            visited.Add(room);

            if (room.Exits["north"] != null)
                BuildRoomCoordinates(room.Exits["north"]!, x, y - 1, coords, visited);
            if (room.Exits["south"] != null)
                BuildRoomCoordinates(room.Exits["south"]!, x, y + 1, coords, visited);
            if (room.Exits["east"] != null)
                BuildRoomCoordinates(room.Exits["east"]!, x + 1, y, coords, visited);
            if (room.Exits["west"] != null)
                BuildRoomCoordinates(room.Exits["west"]!, x - 1, y, coords, visited);
        }

        private DungeonRoom? GetRoomAtCoord(Dictionary<DungeonRoom, (int x, int y)> coords, int x, int y)
        {
            foreach (var kvp in coords)
            {
                if (kvp.Value.x == x && kvp.Value.y == y)
                    return kvp.Key;
            }
            return null;
        }

        private void SelectRoomFromMap(Dictionary<DungeonRoom, (int x, int y)> roomCoords)
        {
            var visitedRooms = roomCoords.Keys.Where(r => r.Visited).ToList();

            if (visitedRooms.Count == 0)
            {
                Console.WriteLine("\n❌ No rooms have been visited yet!");
                Console.WriteLine("Press Enter to continue...");
                Console.ReadLine();
                return;
            }

            Console.WriteLine("\n╔═══════════════════════════════════════╗");
            Console.WriteLine("║         SELECT ROOM TO ENTER          ║");
            Console.WriteLine("╚═══════════════════════════════════════╝");

            for (int i = 0; i < visitedRooms.Count; i++)
            {
                var room = visitedRooms[i];
                var coords = roomCoords[room];
                string status = room == _currentRoom ? " ⬅ CURRENT" : (room.Cleared ? " [✓]" : "");
                Console.WriteLine($"{i + 1}) {GetRoomDescription(room)} at ({coords.x}, {coords.y}){status}");
            }

            Console.WriteLine("0) Cancel");
            Console.Write("\nSelect room to enter (or 0 to cancel): ");

            if (int.TryParse(Console.ReadLine(), out int choice) && choice > 0 && choice <= visitedRooms.Count)
            {
                var selectedRoom = visitedRooms[choice - 1];

                if (selectedRoom == _currentRoom)
                {
                    Console.WriteLine("\n✓ You are already in this room!");
                }
                else
                {
                    _currentRoom = selectedRoom;
                    Console.WriteLine($"\n✓ Moved to {GetRoomDescription(_currentRoom)}");
                }

                Console.WriteLine("Press Enter to continue...");
                Console.ReadLine();
            }
        }

        private string GetRoomSymbolFromArrays(DungeonRoom room, bool isPlayerHere, bool isCleared)
        {
            // Check player position array first
            if (isPlayerHere)
                return "[X]";

            // Check room type
            if (room.Location == LocationType.Hallway || room.Location == LocationType.Intersection)
                return "[~]";

            // Check cleared status array
            if (isCleared)
                return "[✓]";

            // Check special room types
            if (room.Type == RoomType.Boss)
                return "[B]";
            if (room.Type == RoomType.Stairs)
                return "[S]";
            if (room.Type == RoomType.Treasure)
                return "[T]";

            return "[ ]";
        }

        private string GetRoomSymbol(DungeonRoom room)
        {
            if (room == _currentRoom)
                return "[X]";
            else if (room.Location == LocationType.Hallway || room.Location == LocationType.Intersection)
                return "[~]";
            else if (room.Cleared)
                return "[✓]";
            else if (room.Type == RoomType.Boss)
                return "[B]";
            else if (room.Type == RoomType.Stairs)
                return "[S]";
            else if (room.Type == RoomType.Treasure)
                return "[T]";
            else
                return "[ ]";
        }

        private void ShowPartyStatus(List<Character> party)
        {
            Console.WriteLine("\n╔═══════════════ PARTY STATUS ═══════════════╗");
            foreach (var member in party)
            {
                string status = member.IsAlive ? "✓" : "💀";
                Console.WriteLine($"  {status} {member.Name} (Lv {member.Level})");
                Console.WriteLine($"     HP: {member.Health}/{member.MaxHealth} | Mana: {member.Mana}/{member.MaxMana}");
                Console.WriteLine($"     Gold: {member.Inventory.Gold}g");
            }
            Console.WriteLine("╚════════════════════════════════════════════╝");
            Console.WriteLine("\nPress Enter to continue...");
            Console.ReadLine();
        }
    }
}