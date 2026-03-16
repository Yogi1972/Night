using Night.Characters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rpg_Dungeon
{
    #region Camping Static Class

    internal class Camping
    {
        #region Main Camp Menu

        public static List<Character>? CampMenu(List<Character> party, Weather? weather = null, TimeOfDay? timeTracker = null)
        {
            if (party == null || party.Count == 0)
            {
                Console.WriteLine("There is no party to camp with.");
                return null;
            }

            Console.WriteLine("\n╔════════════════════════════════════════╗");
            Console.WriteLine("║          Setting Up Camp              ║");
            Console.WriteLine("╚════════════════════════════════════════╝");

            if (timeTracker != null)
            {
                Console.WriteLine($"Time: {timeTracker.GetTimeDescription()}");
                Console.WriteLine(timeTracker.GetAtmosphericDescription());
            }

            if (weather != null)
            {
                Console.WriteLine($"Weather: {weather.GetWeatherDescription()}");
                Console.WriteLine(GetCampWeatherFlavor(weather.CurrentWeather));
            }

            while (true)
            {
                Console.WriteLine("\nWhat would you like to do?");
                Console.WriteLine("1) Rest");
                Console.WriteLine("2) Forage");
                Console.WriteLine("3) Search for Resources");
                Console.WriteLine("4) Crafting Workshop");
                Console.WriteLine("5) View party");
                Console.WriteLine("6) Options (save/load/exit)");
                Console.WriteLine("0) Leave camp");
                Console.Write("Choice: ");
                var choice = Console.ReadLine() ?? string.Empty;

                switch (choice.Trim())
                {
                    case "1":
                        Rest(party, weather, timeTracker);
                        // Mark that the party has camped so Options can allow save/exit
                        Options.MarkCamped();
                        break;
                    case "2":
                        Forage(party, weather);
                        break;
                    case "3":
                        SearchForResources(party, weather);
                        break;
                    case "4":
                        Crafting.OpenCraftingWorkshop(party);
                        break;
                    case "5":
                        ShowParty(party);
                        break;
                    case "6":
                        var loaded = Options.ShowOptions(party, GameLoopManager.IsMultiplayerMode);
                        if (loaded != null) return loaded;
                        break;
                    case "0":
                        Console.WriteLine(GetBreakCampMessage(weather));
                        return null;
                    default:
                        Console.WriteLine("Invalid choice. Enter 0-7.");
                        break;
                }
            }
        }

        #endregion

        #region Flavor Text

        private static string GetCampWeatherFlavor(WeatherType weatherType)
        {
            return weatherType switch
            {
                WeatherType.Clear => "🔥 The fire crackles warmly under the clear sky.",
                WeatherType.Cloudy => "☁️ Grey clouds overhead, but the camp is comfortable.",
                WeatherType.Rainy => "🌧️ You huddle under makeshift shelters as rain patters down.",
                WeatherType.Stormy => "⛈️ Thunder rumbles. The storm makes camping difficult!",
                WeatherType.Foggy => "🌫️ Thick fog surrounds your camp. Visibility is poor.",
                WeatherType.Snowy => "❄️ Snow falls gently. You gather close to the fire for warmth.",
                WeatherType.Windy => "💨 Wind whips at your tent. You secure the stakes.",
                _ => "The camp is set up and ready."
            };
        }

        private static string GetBreakCampMessage(Weather? weather)
        {
            if (weather == null) return "You break camp and move on.";

            return weather.CurrentWeather switch
            {
                WeatherType.Clear => "☀️ You pack up under pleasant skies and continue your journey.",
                WeatherType.Rainy => "🌧️ You hurriedly pack up your wet gear and press onward.",
                WeatherType.Stormy => "⛈️ You abandon camp and seek better shelter!",
                WeatherType.Snowy => "❄️ You brush off the snow and continue through the cold.",
                _ => "You break camp and move on."
            };
        }

        #endregion

        #region Rest System

        public static void Rest(List<Character> party, Weather? weather = null, TimeOfDay? timeTracker = null, int hours = 8)
        {
            if (party == null || party.Count == 0) return;

            hours = Math.Max(1, hours);
            Console.WriteLine($"\n🏕️ Resting for {hours} hour(s)...");

            // Weather affects rest quality
            if (weather != null)
            {
                switch (weather.CurrentWeather)
                {
                    case WeatherType.Stormy:
                        Console.WriteLine("⛈️ The storm makes it hard to rest. Recovery is reduced!");
                        hours = (int)(hours * 0.7);
                        break;
                    case WeatherType.Rainy:
                        Console.WriteLine("🌧️ The rain is uncomfortable. Rest is less effective.");
                        hours = (int)(hours * 0.8);
                        break;
                    case WeatherType.Clear:
                        // Clear weather at night means clear skies, not sunny
                        if (timeTracker != null && timeTracker.IsDaytime())
                        {
                            Console.WriteLine("☀️ Perfect weather for resting. Recovery is enhanced!");
                        }
                        else
                        {
                            Console.WriteLine("🌙 Clear skies and calm night. Rest is peaceful.");
                        }
                        hours = (int)(hours * 1.1);
                        break;
                }
            }

            foreach (var member in party)
            {
                if (member == null) continue;
                if (!member.IsAlive)
                {
                    Console.WriteLine($"{member.Name} is down and cannot be healed by resting.");
                    continue;
                }

                // Full rest restores to maximum
                int oldHealth = member.Health;
                int oldMana = member.Mana;
                int oldStamina = member.Stamina;

                member.Heal(member.MaxHealth); // Heal up to max
                member.RestoreMana(member.MaxMana); // Restore mana up to max
                member.RestoreStamina(member.MaxStamina); // Restore stamina up to max

                int healthRestored = member.Health - oldHealth;
                int manaRestored = member.Mana - oldMana;
                int staminaRestored = member.Stamina - oldStamina;

                // Display appropriate resource restoration
                if (member is Warrior || member is Rogue)
                {
                    Console.WriteLine($"{member.Name} is fully rested! HP: {member.Health}/{member.MaxHealth} (+{healthRestored}), Stamina: {member.Stamina}/{member.MaxStamina} (+{staminaRestored})");
                }
                else
                {
                    Console.WriteLine($"{member.Name} is fully rested! HP: {member.Health}/{member.MaxHealth} (+{healthRestored}), Mana: {member.Mana}/{member.MaxMana} (+{manaRestored})");
                }
            }

            // Advance time after resting
            if (timeTracker != null)
            {
                timeTracker.AdvanceTime(hours);
            }

            // Burn torches during rest (if lit)
            foreach (var member in party)
            {
                if (member?.Inventory != null)
                {
                    member.Inventory.BurnTorches(hours);
                }
            }
        }

        #endregion

        #region Foraging System

        public static void Forage(List<Character> party, Weather? weather = null)
        {
            if (party == null || party.Count == 0) return;

            var rand = new Random();
            var avgAgi = AverageAgility(party);
            // Base 25% chance, +1% per average agility point, capped at 75%
            var chance = Math.Min(0.75, 0.25 + (avgAgi * 0.01));

            // Weather affects foraging success
            if (weather != null)
            {
                switch (weather.CurrentWeather)
                {
                    case WeatherType.Clear:
                        chance *= 1.2; // 20% bonus in clear weather
                        Console.WriteLine($"🌞 Foraging in clear weather (success chance {chance:P0})...");
                        break;
                    case WeatherType.Rainy:
                    case WeatherType.Stormy:
                        chance *= 0.7; // 30% penalty in rain/storm
                        Console.WriteLine($"🌧️ Foraging in poor weather (success chance {chance:P0})...");
                        break;
                    case WeatherType.Foggy:
                        chance *= 0.8; // 20% penalty in fog
                        Console.WriteLine($"🌫️ Foraging in the fog (success chance {chance:P0})...");
                        break;
                    default:
                        Console.WriteLine($"Foraging (success chance {chance:P0})...");
                        break;
                }
            }
            else
            {
                Console.WriteLine($"Foraging (success chance {chance:P0})...");
            }

            if (rand.NextDouble() <= chance)
            {
                // Expanded foraging results including torch materials
                var finds = new[]
                {
                    "edible mushrooms", "herbs", "a small pouch of coins", "a useful scrap of cloth",
                    "Wood", "Cloth", "Oil Flask", "Iron Ore", "Copper Ore",
                    "Leather Scraps", "Wolf Pelt", "Bone", "Fang"
                };
                var found = finds[rand.Next(finds.Length)];
                Console.WriteLine($"✅ Success! You find {found} while foraging.");

                // Add the foraged item as a GenericItem to a random party member's inventory
                if (found == "Wood" || found == "Cloth" || found == "Oil Flask" ||
                    found == "Iron Ore" || found == "Copper Ore" || found == "Leather Scraps" ||
                    found == "Wolf Pelt" || found == "Bone" || found == "Fang")
                {
                    var luckyMember = party[rand.Next(party.Count)];
                    var item = new GenericItem(found, 5);
                    if (luckyMember.Inventory.AddItem(item))
                    {
                        Console.WriteLine($"📦 {luckyMember.Name} adds {found} to their inventory!");
                    }
                    else
                    {
                        Console.WriteLine($"⚠️ {luckyMember.Name}'s inventory is full. {found} was left behind.");
                    }
                }

                // Weather-specific finds
                if (weather != null && weather.CurrentWeather == WeatherType.Rainy)
                {
                    if (rand.Next(100) < 30)
                    {
                        Console.WriteLine("🍄 You also find some rain-soaked mushrooms!");
                    }
                }
            }
            else
            {
                Console.WriteLine("❌ The party returns empty-handed.");
                if (weather != null && (weather.CurrentWeather == WeatherType.Stormy || weather.CurrentWeather == WeatherType.Rainy))
                {
                    Console.WriteLine("The weather made foraging nearly impossible.");
                }
            }
        }

        #endregion

        #region Resource Searching

        private static void SearchForResources(List<Character> party, Weather? weather)
        {
            if (party == null || party.Count == 0) return;

            // Determine terrain from current location
            // Try to get location coordinates from a party member
            var leader = party.FirstOrDefault();
            if (leader == null) return;

            // For now, use a simple method to determine terrain based on location name
            // In a more integrated system, you'd get actual coordinates from the world map
            TerrainType terrain = DetermineTerrainFromLocation(leader.CurrentLocation);

            ResourceGathering.SearchForResources(party, terrain, weather);
        }

        private static TerrainType DetermineTerrainFromLocation(string locationName)
        {
            // Map location names to terrain types based on region
            var locationLower = locationName.ToLower();

            if (locationLower.Contains("ironforge") || locationLower.Contains("mountain") ||
                locationLower.Contains("stormwatch") || locationLower.Contains("cliff") ||
                locationLower.Contains("eagle") || locationLower.Contains("cave") ||
                locationLower.Contains("frost"))
                return TerrainType.Mountains;

            if (locationLower.Contains("mysthaven") || locationLower.Contains("crystalshore") ||
                locationLower.Contains("coast") || locationLower.Contains("seaside") ||
                locationLower.Contains("fisher") || locationLower.Contains("ferry"))
                return TerrainType.Water;

            if (locationLower.Contains("sunspire") || locationLower.Contains("desert") ||
                locationLower.Contains("oasis") || locationLower.Contains("dune") ||
                locationLower.Contains("nomad") || locationLower.Contains("sand"))
                return TerrainType.Desert;

            if (locationLower.Contains("shadowkeep") || locationLower.Contains("forest") ||
                locationLower.Contains("hunter") || locationLower.Contains("woodcutter") ||
                locationLower.Contains("druid") || locationLower.Contains("ranger") ||
                locationLower.Contains("pinewood"))
                return TerrainType.Forest;

            if (locationLower.Contains("emberpeak") || locationLower.Contains("volcanic") ||
                locationLower.Contains("lava") || locationLower.Contains("ash"))
                return TerrainType.Volcanic;

            // Default to plains
            return TerrainType.Plains;
        }

        #endregion

        #region Helper Methods

        private static double AverageAgility(List<Character> party)
        {
            if (party == null || party.Count == 0) return 0.0;
            var valid = party.Where(p => p != null).ToList();
            if (valid.Count == 0) return 0.0;
            return valid.Average(p => p.Agility);
        }

        private static void ShowParty(List<Character> party)
        {
            Console.WriteLine("Party status:");
            foreach (var member in party)
            {
                if (member == null) continue;
                Console.WriteLine($"- {member.Name}: HP={member.Health} {(member.IsAlive ? "(Alive)" : "(Down)")}, Mana={member.Mana}, Class={member.GetType().Name}");
            }
        }

        #endregion
    }

    #endregion
}
