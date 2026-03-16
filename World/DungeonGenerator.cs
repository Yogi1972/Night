using System;
using System.Collections.Generic;
using System.Linq;

namespace Rpg_Dungeon
{
    internal class DungeonGenerator
    {
        private readonly Random _rng;

        public DungeonGenerator(int seed)
        {
            _rng = new Random(seed);
        }

        public DungeonFloor GenerateFloor(int floorNumber, int totalFloors)
        {
            var floor = new DungeonFloor(floorNumber);

            int roomCount = _rng.Next(5, 9);
            var rooms = new List<DungeonRoom> { floor.StartRoom };

            for (int i = 1; i < roomCount; i++)
            {
                RoomType type = DetermineRoomType(i, roomCount);
                var newRoom = new DungeonRoom(type);
                rooms.Add(newRoom);
                floor.AllRooms.Add(newRoom);
            }

            ConnectRooms(rooms, floor);
            AddBranchingPaths(rooms, floor);

            floor.BossRoom = rooms[rooms.Count - 1];

            if (floorNumber < totalFloors)
            {
                AddStairsDown(floor, rooms);
            }

            return floor;
        }

        private RoomType DetermineRoomType(int roomIndex, int totalRooms)
        {
            if (roomIndex == totalRooms - 1)
            {
                return RoomType.Boss;
            }

            int roll = _rng.Next(100);

            if (roll < 50) return RoomType.Combat;
            if (roll < 70) return RoomType.Elite;
            if (roll < 85) return RoomType.Treasure;
            return RoomType.Empty;
        }

        private void ConnectRooms(List<DungeonRoom> rooms, DungeonFloor floor)
        {
            for (int i = 0; i < rooms.Count - 1; i++)
            {
                var room = rooms[i];
                var nextRoom = rooms[i + 1];

                var directions = new[] { "north", "south", "east", "west" };
                var dir = directions[_rng.Next(directions.Length)];
                var oppositeDir = GetOppositeDirection(dir);

                if (_rng.Next(100) < 40)
                {
                    var hallway = new DungeonRoom(RoomType.Empty, LocationType.Hallway);
                    floor.AllRooms.Add(hallway);
                    room.Exits[dir] = hallway;
                    hallway.Exits[oppositeDir] = room;
                    hallway.Exits[dir] = nextRoom;
                    nextRoom.Exits[oppositeDir] = hallway;
                }
                else
                {
                    room.Exits[dir] = nextRoom;
                    nextRoom.Exits[oppositeDir] = room;
                }
            }
        }

        private void AddBranchingPaths(List<DungeonRoom> rooms, DungeonFloor floor)
        {
            if (rooms.Count >= 4)
            {
                for (int i = 0; i < rooms.Count / 2; i++)
                {
                    var room1 = rooms[_rng.Next(rooms.Count - 1)];
                    var room2 = rooms[_rng.Next(1, rooms.Count)];

                    if (room1 != room2)
                    {
                        TryConnectRooms(room1, room2);
                    }
                }
            }
        }

        private void TryConnectRooms(DungeonRoom room1, DungeonRoom room2)
        {
            var directions = new[] { "north", "south", "east", "west" };
            var availableDirections = directions.Where(d => room1.Exits[d] == null).ToList();

            if (availableDirections.Count > 0)
            {
                var dir = availableDirections[_rng.Next(availableDirections.Count)];
                var oppositeDir = GetOppositeDirection(dir);

                if (room2.Exits[oppositeDir] == null)
                {
                    room1.Exits[dir] = room2;
                    room2.Exits[oppositeDir] = room1;
                }
            }
        }

        private void AddStairsDown(DungeonFloor floor, List<DungeonRoom> rooms)
        {
            var stairsRoom = new DungeonRoom(RoomType.Stairs);
            floor.StairsDown = stairsRoom;
            floor.AllRooms.Add(stairsRoom);

            var connectRoom = rooms[_rng.Next(Math.Max(1, rooms.Count - 1))];
            var directions = new[] { "north", "south", "east", "west" };
            var availableDirections = directions.Where(d => connectRoom.Exits[d] == null).ToList();

            if (availableDirections.Count > 0)
            {
                var dir = availableDirections[_rng.Next(availableDirections.Count)];
                var oppositeDir = GetOppositeDirection(dir);
                connectRoom.Exits[dir] = stairsRoom;
                stairsRoom.Exits[oppositeDir] = connectRoom;
            }
        }

        private string GetOppositeDirection(string direction)
        {
            return direction switch
            {
                "north" => "south",
                "south" => "north",
                "east" => "west",
                "west" => "east",
                _ => "north"
            };
        }

        public string GetHallwayDescription()
        {
            var descriptions = new[]
            {
                "The corridor is damp and covered in moss.",
                "Torches flicker along the stone walls.",
                "You hear water dripping in the distance.",
                "Ancient carvings adorn the walls.",
                "The air grows cold and still.",
                "Cobwebs hang from the ceiling.",
                "Your footsteps echo through the passage.",
                "A musty smell fills the air.",
                "The walls are lined with old bones.",
                "Strange symbols glow faintly on the floor.",
                "Dust swirls in the stale air.",
                "The corridor twists and turns mysteriously.",
                "Shadows dance on the walls.",
                "The stone floor is worn smooth by age."
            };
            return descriptions[_rng.Next(descriptions.Length)];
        }

        public string GetRoomAtmosphere(RoomType type, int floorNumber)
        {
            return type switch
            {
                RoomType.Empty => GetEmptyRoomDescription(),
                RoomType.Combat => GetCombatRoomDescription(),
                RoomType.Elite => GetEliteRoomDescription(),
                RoomType.Treasure => GetTreasureRoomDescription(),
                RoomType.Boss => GetBossRoomDescription(floorNumber),
                RoomType.Stairs => GetStairsDescription(),
                _ => "The room is unremarkable."
            };
        }

        private string GetEmptyRoomDescription()
        {
            var descriptions = new[]
            {
                "The room is quiet and peaceful. A safe haven.",
                "Sunlight streams through cracks in the ceiling. You can rest here.",
                "An abandoned campsite sits in the corner. This seems safe.",
                "The room feels unusually calm. A good place to catch your breath.",
                "Old furniture lies broken but the area is secure.",
                "A peaceful chamber away from danger."
            };
            return descriptions[_rng.Next(descriptions.Length)];
        }

        private string GetCombatRoomDescription()
        {
            var descriptions = new[]
            {
                "Blood stains mark the floor. Something dangerous lurks here.",
                "You hear growling ahead. Combat is inevitable.",
                "The smell of death fills the air. Enemies await.",
                "Claw marks scar the walls. Prepare for battle.",
                "Bones litter the ground. This is a killing ground.",
                "The air is thick with tension. Danger is near."
            };
            return descriptions[_rng.Next(descriptions.Length)];
        }

        private string GetEliteRoomDescription()
        {
            var descriptions = new[]
            {
                "The air crackles with malevolent energy. A powerful foe awaits.",
                "Skulls are piled in the corners. Something formidable dwells here.",
                "The temperature drops sharply. You sense a dangerous presence.",
                "Weapons and armor litter the ground. Many have failed here.",
                "Dark energy pulses through the room. A champion guards this place.",
                "Trophies from previous victims line the walls."
            };
            return descriptions[_rng.Next(descriptions.Length)];
        }

        private string GetTreasureRoomDescription()
        {
            var descriptions = new[]
            {
                "An ornate chest sits in the center of the room, gleaming with promise.",
                "Gold coins glitter in the torchlight. Treasure awaits!",
                "You spot a locked chest covered in dust and cobwebs.",
                "Precious items are scattered across the floor. Someone left in a hurry.",
                "A vault door stands ajar, revealing treasures within.",
                "Gems sparkle from every surface. This is a treasure trove!"
            };
            return descriptions[_rng.Next(descriptions.Length)];
        }

        private string GetBossRoomDescription(int floorNumber)
        {
            var descriptions = new[]
            {
                $"A massive chamber opens before you. The Floor {floorNumber} Guardian awaits on a throne of bones.",
                $"The room trembles with dark power. The master of Floor {floorNumber} prepares to defend their domain.",
                $"Torches ignite around the perimeter. The Floor {floorNumber} Boss has been expecting you.",
                $"An arena-like chamber with the Floor {floorNumber} champion waiting in the center.",
                $"The air grows heavy. The Floor {floorNumber} guardian emerges from the shadows."
            };
            return descriptions[_rng.Next(descriptions.Length)];
        }

        private string GetStairsDescription()
        {
            var descriptions = new[]
            {
                "Ancient stairs spiral downward into darkness.",
                "A stone staircase descends deeper into the dungeon.",
                "Worn steps lead to the next floor below.",
                "A spiral staircase carved from solid rock leads down.",
                "Stone steps descend into shadow and mystery."
            };
            return descriptions[_rng.Next(descriptions.Length)];
        }

        public int GetNextRandom(int maxValue)
        {
            return _rng.Next(maxValue);
        }

        public int GetNextRandom(int minValue, int maxValue)
        {
            return _rng.Next(minValue, maxValue);
        }
    }
}
