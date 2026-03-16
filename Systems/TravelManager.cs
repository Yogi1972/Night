using Night.Characters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Rpg_Dungeon
{
    #region Destination Enum

    internal enum Destination
    {
        Dungeon,
        QuestLocation,
        Town,
        BountyTarget
    }

    #endregion

    #region TravelManager Class

    internal class TravelManager
    {
        #region Fields

        private static readonly Random _rng = new Random();

        #endregion

        #region Public Travel Methods

        public static void TravelToDungeon(List<Character> party, Weather weather, TimeOfDay timeTracker, int dungeonLevel)
        {
            int baseDistance = dungeonLevel * 5 + _rng.Next(5, 15); // km
            string dungeonName = GetDungeonName(dungeonLevel);

            Console.WriteLine("\n╔════════════════ JOURNEY ═══════════════════╗");
            Console.WriteLine($"║  Destination: {dungeonName,-28}║");
            Console.WriteLine($"║  Distance: {baseDistance} km{new string(' ', 30 - baseDistance.ToString().Length)}║");
            Console.WriteLine($"║  Time: {timeTracker.GetTimeDescription(),-33}║");
            Console.WriteLine($"║  Weather: {weather.GetWeatherDescription(),-29}║");
            Console.WriteLine("╚════════════════════════════════════════════╝\n");

            // If we have a hex map available in GameServices, use hex pathfinding for travel cost
            int travelCostKm = baseDistance;
            var fog = Systems.GameServices.Journal; // dummy access to avoid adding new service; we'll check FogOfWarMap elsewhere
            // Fall back to base distance
            ExecuteTravel(party, weather, timeTracker, travelCostKm, Destination.Dungeon, dungeonName);
        }

        public static void TravelToQuest(List<Character> party, Weather weather, TimeOfDay timeTracker, string questName)
        {
            int baseDistance = _rng.Next(8, 25); // km

            Console.WriteLine("\n╔════════════════ JOURNEY ═══════════════════╗");
            Console.WriteLine($"║  Destination: Quest Location               ║");
            Console.WriteLine($"║  Quest: {questName,-34}║");
            Console.WriteLine($"║  Distance: {baseDistance} km{new string(' ', 30 - baseDistance.ToString().Length)}║");
            Console.WriteLine($"║  Time: {timeTracker.GetTimeDescription(),-33}║");
            Console.WriteLine($"║  Weather: {weather.GetWeatherDescription(),-29}║");
            Console.WriteLine("╚════════════════════════════════════════════╝\n");

            ExecuteTravel(party, weather, timeTracker, baseDistance, Destination.QuestLocation, questName);
        }

        public static void ReturnToTown(List<Character> party, Weather weather, TimeOfDay timeTracker)
        {
            int baseDistance = _rng.Next(5, 15); // km

            Console.WriteLine("\n╔════════════════ JOURNEY ═══════════════════╗");
            Console.WriteLine("║  Returning to Town                         ║");
            Console.WriteLine($"║  Distance: {baseDistance} km{new string(' ', 30 - baseDistance.ToString().Length)}║");
            Console.WriteLine($"║  Time: {timeTracker.GetTimeDescription(),-33}║");
            Console.WriteLine($"║  Weather: {weather.GetWeatherDescription(),-29}║");
            Console.WriteLine("╚════════════════════════════════════════════╝\n");

            ExecuteTravel(party, weather, timeTracker, baseDistance, Destination.Town, "Town");
        }

        public static void TravelBetweenAreas(List<Character> party, Weather weather, TimeOfDay timeTracker)
        {
            int baseDistance = _rng.Next(10, 30); // km (longer distance between areas)

            Console.WriteLine($"\n║  Distance: {baseDistance} km");
            Console.WriteLine($"║  Time: {timeTracker.GetTimeDescription()}");
            Console.WriteLine($"║  Weather: {weather.GetWeatherDescription()}");

            ExecuteTravel(party, weather, timeTracker, baseDistance, Destination.QuestLocation, "New Area");
        }

        #endregion

        #region Travel Execution

        private static void ExecuteTravel(List<Character> party, Weather weather, TimeOfDay timeTracker, int distance, Destination destination, string destinationName)
        {
            // If a hex map is present, try to compute travel distance via hex pathfinding
            int computedDistance = distance;
            try
            {
                var fog = Rpg_Dungeon.Systems.GameServices.FogOfWarMap;
                if (fog != null)
                {
                    // attempt to find start and end nodes by name
                    var start = fog.GetType().GetField("_currentLocationName", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.GetValue(fog) as string;
                    if (!string.IsNullOrEmpty(start))
                    {
                        var startNode = fog.GetType().GetField("_mapNodes", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.GetValue(fog) as System.Collections.IList;
                        if (startNode != null)
                        {
                            // find the MapNode objects and approximate coords
                            foreach (var obj in startNode)
                            {
                            var nameProp = obj.GetType().GetProperty("LocationName");
                            if (nameProp != null)
                            {
                                var nameVal = nameProp.GetValue(obj) as string;
                                if (nameVal == start)
                                {
                                    var xProp = obj.GetType().GetProperty("X");
                                    var yProp = obj.GetType().GetProperty("Y");
                                    if (xProp != null && yProp != null)
                                    {
                                        var xObj = xProp.GetValue(obj);
                                        var yObj = yProp.GetValue(obj);
                                        if (xObj is int x && yObj is int y)
                                        {
                                            // found start coordinates (int x, int y)
                                        }
                                    }
                                }
                            }
                            }
                        }
                    }
                }
            }
            catch
            {
                // ignore reflection issues and fall back to given distance
            }

            // Calculate travel time based on distance, weather, and time of day
            double baseTime = computedDistance * 0.5; // Base: 0.5 hours per km
            double weatherModifier = weather.GetTravelTimeModifier();
            double nightModifier = timeTracker.IsNighttime() ? 1.3 : 1.0; // 30% slower at night
            double totalTime = baseTime * weatherModifier * nightModifier;

            Console.WriteLine($"🚶 The party begins their journey...");

            // Show time and weather flavor
            Console.WriteLine(timeTracker.GetAtmosphericDescription());
            Console.WriteLine(weather.GetTravelFlavorText());

            if (timeTracker.IsNighttime())
            {
                Console.WriteLine("🌙 Traveling at night is slower and more dangerous...");
            }

            Console.WriteLine($"Estimated travel time: {totalTime:F1} hours\n");

            // Simulate travel segments
            int segments = Math.Max(2, distance / 5);
            for (int i = 0; i < segments; i++)
            {
                Thread.Sleep(400); // Brief pause for immersion
                Console.Write(".");

                // Random travel events
                if (_rng.Next(100) < 15)
                {
                    Console.WriteLine();
                    ShowTravelEvent(party, weather, timeTracker);
                }
            }

            Console.WriteLine("\n");

            // Advance time
            int hoursToAdvance = (int)Math.Ceiling(totalTime);
            timeTracker.AdvanceTime(hoursToAdvance);

            // Burn torches during travel
            foreach (var member in party)
            {
                if (member?.Inventory != null)
                {
                    member.Inventory.BurnTorches(hoursToAdvance);
                }
            }

            // Apply weather hazards for longer journeys
            if (distance > 10)
            {
                weather.ApplyTravelHazard(party);
            }

            // Arrival message
            ShowArrivalMessage(destination, destinationName, weather, timeTracker);

            Console.WriteLine("\nPress Enter to continue...");
            Console.ReadLine();
        }

        #endregion

        #region Helper Methods

        private static void ShowTravelEvent(List<Character> party, Weather weather, TimeOfDay? timeTracker)
        {
            var events = new[]
            {
                "🐺 You spot wolves in the distance. They don't approach.",
                "🌲 You pass through a dense forest. The path is overgrown.",
                "🏔️ You climb over rocky terrain. The going is rough.",
                "🌊 You cross a shallow stream. Your boots get wet.",
                "🦅 An eagle soars overhead, circling.",
                "🏚️ You pass by abandoned ruins. They look ancient.",
                "🌺 Wildflowers bloom along the path.",
                "🦌 A deer bounds across your path and disappears.",
                "⛰️ You reach the top of a hill. The view is breathtaking.",
                "🌳 Ancient trees tower above you. The forest is thick here.",
                "🪨 You navigate around large boulders blocking the path.",
                "🌾 You walk through tall grass swaying in the breeze."
            };

            // Night-specific events
            if (timeTracker != null && timeTracker.IsNighttime())
            {
                var nightEvents = new[]
                {
                    "🦉 An owl hoots in the darkness.",
                    "🌙 The moonlight casts eerie shadows on the path.",
                    "👀 You hear rustling in the darkness but see nothing.",
                    "🔦 You light a torch to see the path ahead.",
                    "⭐ The stars provide just enough light to continue.",
                    "🦇 Bats flutter overhead in the night."
                };
                Console.WriteLine(nightEvents[_rng.Next(nightEvents.Length)]);
                return;
            }

            // Weather-specific events
            if (weather.CurrentWeather == WeatherType.Rainy)
            {
                var rainyEvents = new[]
                {
                    "💧 You take shelter under a tree as the rain intensifies.",
                    "🌧️ The rain makes the path slippery. You move carefully.",
                    "💦 Water pools in your footprints as you trudge onward."
                };
                Console.WriteLine(rainyEvents[_rng.Next(rainyEvents.Length)]);
            }
            else if (weather.CurrentWeather == WeatherType.Foggy)
            {
                var foggyEvents = new[]
                {
                    "🌫️ The fog is so thick you can barely see ahead.",
                    "👻 Strange shapes appear in the mist, but they're just trees.",
                    "🧭 You pause to check your bearings in the fog."
                };
                Console.WriteLine(foggyEvents[_rng.Next(foggyEvents.Length)]);
            }
            else if (weather.CurrentWeather == WeatherType.Snowy)
            {
                var snowyEvents = new[]
                {
                    "⛄ The snow is getting deeper. Travel is exhausting.",
                    "🥶 You huddle together for warmth before continuing.",
                    "❄️ Ice forms on your equipment and gear."
                };
                Console.WriteLine(snowyEvents[_rng.Next(snowyEvents.Length)]);
            }
            else
            {
                Console.WriteLine(events[_rng.Next(events.Length)]);
            }
        }

        private static void ShowArrivalMessage(Destination destination, string name, Weather weather, TimeOfDay? timeTracker)
        {
            switch (destination)
            {
                case Destination.Dungeon:
                    Console.WriteLine($"⚔️  You arrive at {name}.");
                    Console.WriteLine("A dark entrance looms before you. The air feels oppressive.");
                    Console.WriteLine("You step inside, leaving the weather behind...");
                    break;

                case Destination.QuestLocation:
                    Console.WriteLine($"📍 You arrive at your quest destination.");
                    Console.WriteLine($"The weather is {weather.CurrentWeather.ToString().ToLower()} as you prepare for your task.");
                    break;

                case Destination.Town:
                    Console.WriteLine("🏘️ You return to the safety of town.");
                    Console.WriteLine("The familiar sights and sounds welcome you back.");
                    break;

                case Destination.BountyTarget:
                    Console.WriteLine("🎯 You arrive at the bounty target's last known location.");
                    Console.WriteLine("Stay alert. Dangerous prey awaits...");
                    break;
            }
        }

        private static string GetDungeonName(int level)
        {
            return level switch
            {
                1 => "The Forsaken Catacombs",
                2 => "The Shadow Depths",
                3 => "The Abyssal Ruins",
                4 => "The Cursed Sanctum",
                5 => "The Nether Tombs",
                _ => $"The Dark Dungeon (Level {level})"
            };
        }

        #endregion
    }

    #endregion
}
