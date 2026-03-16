using Night.Characters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rpg_Dungeon
{
    #region Terrain Type Enum

    public enum TerrainType
    {
        Plains,
        Forest,
        Mountains,
        Desert,
        Volcanic,
        Water,
        Cave,
        Swamp
    }

    #endregion

    #region Resource Data

    internal class ResourceNode
    {
        public string Name { get; set; }
        public TerrainType Terrain { get; set; }
        public CraftingProfession RequiredProfession { get; set; }
        public int BaseSuccessRate { get; set; }
        public int MinAmount { get; set; }
        public int MaxAmount { get; set; }

        public ResourceNode(string name, TerrainType terrain, CraftingProfession profession, int successRate, int minAmount = 1, int maxAmount = 3)
        {
            Name = name;
            Terrain = terrain;
            RequiredProfession = profession;
            BaseSuccessRate = successRate;
            MinAmount = minAmount;
            MaxAmount = maxAmount;
        }
    }

    #endregion

    #region Resource Gathering System

    internal static class ResourceGathering
    {
        private static readonly List<ResourceNode> ResourceDatabase = new()
        {
            // === MOUNTAIN RESOURCES (15 total) ===
            new ResourceNode("Iron Ore", TerrainType.Mountains, CraftingProfession.None, 65, 1, 3),
            new ResourceNode("Silver Ore", TerrainType.Mountains, CraftingProfession.None, 40, 1, 2),
            new ResourceNode("Gold Nugget", TerrainType.Mountains, CraftingProfession.None, 20, 1, 1),
            new ResourceNode("Mithril Ore", TerrainType.Mountains, CraftingProfession.Blacksmithing, 30, 1, 2),
            new ResourceNode("Small Gem", TerrainType.Mountains, CraftingProfession.Jewelcrafting, 35, 1, 2),
            new ResourceNode("Large Gem", TerrainType.Mountains, CraftingProfession.Jewelcrafting, 15, 1, 1),
            new ResourceNode("Crystal Shard", TerrainType.Mountains, CraftingProfession.Enchanting, 25, 1, 2),
            new ResourceNode("Copper Ore", TerrainType.Mountains, CraftingProfession.None, 55, 1, 2),
            new ResourceNode("Stone", TerrainType.Mountains, CraftingProfession.None, 75, 2, 4),
            new ResourceNode("Granite", TerrainType.Mountains, CraftingProfession.Blacksmithing, 40, 1, 2),
            new ResourceNode("Quartz", TerrainType.Mountains, CraftingProfession.Enchanting, 35, 1, 2),
            new ResourceNode("Ice Crystal", TerrainType.Mountains, CraftingProfession.Alchemy, 30, 1, 1),
            new ResourceNode("Mountain Herb", TerrainType.Mountains, CraftingProfession.Herbalism, 45, 1, 2),
            new ResourceNode("Eagle Feather", TerrainType.Mountains, CraftingProfession.None, 25, 1, 1),
            new ResourceNode("Bear Pelt", TerrainType.Mountains, CraftingProfession.Leatherworking, 30, 1, 1),

            // === FOREST RESOURCES (16 total) ===
            new ResourceNode("Wood", TerrainType.Forest, CraftingProfession.None, 75, 2, 4),
            new ResourceNode("Healing Herb", TerrainType.Forest, CraftingProfession.Herbalism, 60, 1, 3),
            new ResourceNode("Mana Flower", TerrainType.Forest, CraftingProfession.Herbalism, 45, 1, 2),
            new ResourceNode("Wolf Pelt", TerrainType.Forest, CraftingProfession.Leatherworking, 50, 1, 2),
            new ResourceNode("Spider Silk", TerrainType.Forest, CraftingProfession.None, 35, 1, 2),
            new ResourceNode("Deer Hide", TerrainType.Forest, CraftingProfession.Leatherworking, 40, 1, 2),
            new ResourceNode("Mushroom Cap", TerrainType.Forest, CraftingProfession.Herbalism, 55, 1, 3),
            new ResourceNode("Rootvine", TerrainType.Forest, CraftingProfession.Herbalism, 40, 1, 2),
            new ResourceNode("Hardwood", TerrainType.Forest, CraftingProfession.None, 45, 1, 2),
            new ResourceNode("Pine Resin", TerrainType.Forest, CraftingProfession.Alchemy, 50, 1, 2),
            new ResourceNode("Moss", TerrainType.Forest, CraftingProfession.Herbalism, 65, 1, 3),
            new ResourceNode("Wild Berries", TerrainType.Forest, CraftingProfession.None, 60, 1, 3),
            new ResourceNode("Bat Wing", TerrainType.Forest, CraftingProfession.Alchemy, 35, 1, 2),
            new ResourceNode("Fox Pelt", TerrainType.Forest, CraftingProfession.Leatherworking, 38, 1, 2),
            new ResourceNode("Birch Bark", TerrainType.Forest, CraftingProfession.None, 55, 1, 2),
            new ResourceNode("Acorn", TerrainType.Forest, CraftingProfession.Herbalism, 70, 2, 4),

            // === DESERT RESOURCES (14 total) ===
            new ResourceNode("Sandstone", TerrainType.Desert, CraftingProfession.None, 70, 2, 4),
            new ResourceNode("Cactus Flesh", TerrainType.Desert, CraftingProfession.Herbalism, 55, 1, 2),
            new ResourceNode("Ancient Tablet", TerrainType.Desert, CraftingProfession.Enchanting, 15, 1, 1),
            new ResourceNode("Desert Chitin", TerrainType.Desert, CraftingProfession.Leatherworking, 45, 1, 2),
            new ResourceNode("Sun Crystal", TerrainType.Desert, CraftingProfession.Jewelcrafting, 25, 1, 1),
            new ResourceNode("Scorpion Venom", TerrainType.Desert, CraftingProfession.Alchemy, 35, 1, 2),
            new ResourceNode("Dried Herbs", TerrainType.Desert, CraftingProfession.Herbalism, 50, 1, 2),
            new ResourceNode("Sand", TerrainType.Desert, CraftingProfession.None, 80, 2, 4),
            new ResourceNode("Amber", TerrainType.Desert, CraftingProfession.Jewelcrafting, 30, 1, 1),
            new ResourceNode("Lizard Scale", TerrainType.Desert, CraftingProfession.Leatherworking, 42, 1, 2),
            new ResourceNode("Scarab Shell", TerrainType.Desert, CraftingProfession.Alchemy, 38, 1, 2),
            new ResourceNode("Desert Rose", TerrainType.Desert, CraftingProfession.Herbalism, 35, 1, 1),
            new ResourceNode("Fossil", TerrainType.Desert, CraftingProfession.None, 20, 1, 1),
            new ResourceNode("Mirage Essence", TerrainType.Desert, CraftingProfession.Enchanting, 25, 1, 1),

            // === VOLCANIC RESOURCES (13 total) ===
            new ResourceNode("Obsidian", TerrainType.Volcanic, CraftingProfession.Blacksmithing, 55, 1, 2),
            new ResourceNode("Sulfur", TerrainType.Volcanic, CraftingProfession.Alchemy, 60, 1, 3),
            new ResourceNode("Fire Crystal", TerrainType.Volcanic, CraftingProfession.Enchanting, 30, 1, 2),
            new ResourceNode("Lava Rock", TerrainType.Volcanic, CraftingProfession.None, 70, 2, 3),
            new ResourceNode("Flame Essence", TerrainType.Volcanic, CraftingProfession.Alchemy, 25, 1, 1),
            new ResourceNode("Volcanic Glass", TerrainType.Volcanic, CraftingProfession.Jewelcrafting, 35, 1, 2),
            new ResourceNode("Ash Powder", TerrainType.Volcanic, CraftingProfession.Alchemy, 50, 1, 2),
            new ResourceNode("Basalt", TerrainType.Volcanic, CraftingProfession.Blacksmithing, 48, 1, 2),
            new ResourceNode("Pumice", TerrainType.Volcanic, CraftingProfession.None, 60, 1, 3),
            new ResourceNode("Fire Thorn", TerrainType.Volcanic, CraftingProfession.Herbalism, 35, 1, 1),
            new ResourceNode("Magma Core", TerrainType.Volcanic, CraftingProfession.Blacksmithing, 18, 1, 1),
            new ResourceNode("Ember Shard", TerrainType.Volcanic, CraftingProfession.Enchanting, 28, 1, 2),
            new ResourceNode("Salamander Scale", TerrainType.Volcanic, CraftingProfession.Leatherworking, 32, 1, 1),

            // === WATER/COASTAL RESOURCES (16 total) ===
            new ResourceNode("Seaweed", TerrainType.Water, CraftingProfession.Herbalism, 65, 1, 3),
            new ResourceNode("Pearl", TerrainType.Water, CraftingProfession.Jewelcrafting, 20, 1, 1),
            new ResourceNode("Coral Fragment", TerrainType.Water, CraftingProfession.None, 45, 1, 2),
            new ResourceNode("Saltpeter", TerrainType.Water, CraftingProfession.Alchemy, 50, 1, 2),
            new ResourceNode("Aqua Crystal", TerrainType.Water, CraftingProfession.Enchanting, 30, 1, 2),
            new ResourceNode("Fish Scales", TerrainType.Water, CraftingProfession.Leatherworking, 55, 1, 3),
            new ResourceNode("Kelp Strand", TerrainType.Water, CraftingProfession.Herbalism, 60, 1, 2),
            new ResourceNode("Shell", TerrainType.Water, CraftingProfession.None, 70, 2, 3),
            new ResourceNode("Starfish", TerrainType.Water, CraftingProfession.Alchemy, 40, 1, 2),
            new ResourceNode("Shark Tooth", TerrainType.Water, CraftingProfession.Leatherworking, 28, 1, 1),
            new ResourceNode("Sea Glass", TerrainType.Water, CraftingProfession.Jewelcrafting, 35, 1, 2),
            new ResourceNode("Driftwood", TerrainType.Water, CraftingProfession.None, 60, 1, 2),
            new ResourceNode("Tide Crystal", TerrainType.Water, CraftingProfession.Enchanting, 25, 1, 1),
            new ResourceNode("Clam", TerrainType.Water, CraftingProfession.None, 55, 1, 2),
            new ResourceNode("Sea Urchin", TerrainType.Water, CraftingProfession.Alchemy, 38, 1, 2),
            new ResourceNode("Whale Bone", TerrainType.Water, CraftingProfession.Blacksmithing, 15, 1, 1),

            // === PLAINS RESOURCES (16 total) ===
            new ResourceNode("Cotton", TerrainType.Plains, CraftingProfession.None, 60, 1, 3),
            new ResourceNode("Wildflowers", TerrainType.Plains, CraftingProfession.Herbalism, 70, 1, 3),
            new ResourceNode("Rabbit Pelt", TerrainType.Plains, CraftingProfession.Leatherworking, 50, 1, 2),
            new ResourceNode("Iron Ore", TerrainType.Plains, CraftingProfession.None, 35, 1, 2),
            new ResourceNode("Clay", TerrainType.Plains, CraftingProfession.None, 55, 1, 3),
            new ResourceNode("Wheat Stalk", TerrainType.Plains, CraftingProfession.Herbalism, 65, 1, 3),
            new ResourceNode("Copper Ore", TerrainType.Plains, CraftingProfession.None, 45, 1, 2),
            new ResourceNode("Flax", TerrainType.Plains, CraftingProfession.None, 58, 1, 2),
            new ResourceNode("Honey", TerrainType.Plains, CraftingProfession.Alchemy, 40, 1, 1),
            new ResourceNode("Feather", TerrainType.Plains, CraftingProfession.None, 65, 1, 3),
            new ResourceNode("Egg", TerrainType.Plains, CraftingProfession.None, 45, 1, 2),
            new ResourceNode("Grass Blade", TerrainType.Plains, CraftingProfession.Herbalism, 75, 2, 4),
            new ResourceNode("Leather Scraps", TerrainType.Plains, CraftingProfession.Leatherworking, 52, 1, 2),
            new ResourceNode("Raw Meat", TerrainType.Plains, CraftingProfession.None, 48, 1, 2),
            new ResourceNode("Bone", TerrainType.Plains, CraftingProfession.None, 42, 1, 2),
            new ResourceNode("Fang", TerrainType.Plains, CraftingProfession.Leatherworking, 35, 1, 1)
        };

        public static TerrainType DetermineTerrainFromChar(char terrainChar)
        {
            return terrainChar switch
            {
                '^' => TerrainType.Mountains,
                '~' => TerrainType.Water,
                '≋' => TerrainType.Desert,
                '░' => TerrainType.Forest,
                '▒' => TerrainType.Volcanic,
                '□' => TerrainType.Plains,
                ' ' => TerrainType.Plains,
                _ => TerrainType.Plains
            };
        }

        public static void SearchForResources(List<Character> party, TerrainType currentTerrain, Weather? weather = null)
        {
            if (party == null || party.Count == 0)
            {
                Console.WriteLine("No one to search for resources.");
                return;
            }

            Console.WriteLine("\n╔════════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                    🔍 SEARCH FOR RESOURCES                          ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════════════╝");
            Console.WriteLine($"\n🌍 Current Terrain: {GetTerrainDescription(currentTerrain)}");
            Console.WriteLine($"📦 Available resources in {currentTerrain} terrain:");

            var availableResources = ResourceDatabase.Where(r => r.Terrain == currentTerrain).ToList();

            if (availableResources.Count == 0)
            {
                Console.WriteLine("No resources available in this terrain.");
                return;
            }

            // Group by profession requirement
            var anyoneResources = availableResources.Where(r => r.RequiredProfession == CraftingProfession.None).ToList();
            var professionResources = availableResources.Where(r => r.RequiredProfession != CraftingProfession.None).ToList();

            if (anyoneResources.Count > 0)
            {
                Console.WriteLine("\n✅ Common Resources (anyone can gather):");
                foreach (var res in anyoneResources)
                {
                    Console.WriteLine($"   • {res.Name} (Success: {res.BaseSuccessRate}%)");
                }
            }

            if (professionResources.Count > 0)
            {
                Console.WriteLine("\n⚙️ Profession-Specific Resources:");
                foreach (var res in professionResources)
                {
                    var icon = ProfessionManager.GetProfessionIcon(res.RequiredProfession);
                    var hasProf = party.Any(p => p.PrimaryProfession == res.RequiredProfession ||
                                                  p.SecondaryProfession == res.RequiredProfession);
                    var marker = hasProf ? "✓" : "✗";
                    Console.WriteLine($"   {marker} {res.Name} - {icon} {res.RequiredProfession} (Success: {res.BaseSuccessRate}%)");
                }
            }

            Console.WriteLine("\n👥 Select party member to search for resources:");
            for (int i = 0; i < party.Count; i++)
            {
                var member = party[i];
                var professions = ProfessionManager.GetCharacterProfessions(member);
                var profText = professions.Count > 0
                    ? $" [{string.Join(", ", professions.Select(p => ProfessionManager.GetProfessionIcon(p) + p.ToString()))}]"
                    : " [No professions]";
                Console.WriteLine($"{i + 1}) {member.Name}{profText} - Agi: {member.Agility}");
            }
            Console.WriteLine("0) Cancel");
            Console.Write("\nChoice: ");

            var choice = Console.ReadLine()?.Trim() ?? "";
            if (choice == "0") return;

            if (!int.TryParse(choice, out var idx) || idx < 1 || idx > party.Count)
            {
                Console.WriteLine("Invalid selection.");
                return;
            }

            var gatherer = party[idx - 1];
            GatherResources(gatherer, currentTerrain, weather);
        }

        private static void GatherResources(Character gatherer, TerrainType terrain, Weather? weather)
        {
            Console.WriteLine($"\n🔍 {gatherer.Name} searches the {terrain} terrain for resources...");

            var availableResources = ResourceDatabase
                .Where(r => r.Terrain == terrain)
                .Where(r => r.RequiredProfession == CraftingProfession.None ||
                           gatherer.PrimaryProfession == r.RequiredProfession ||
                           gatherer.SecondaryProfession == r.RequiredProfession)
                .ToList();

            if (availableResources.Count == 0)
            {
                Console.WriteLine($"❌ {gatherer.Name} cannot gather any resources here without the right professions.");
                return;
            }

            var rng = new Random();
            var foundResources = new List<(string name, int amount)>();

            // Weather modifier
            double weatherModifier = GetWeatherModifier(weather);
            if (weatherModifier != 1.0)
            {
                Console.WriteLine($"   {GetWeatherMessage(weather)}");
            }

            // Agility bonus: +1% success per point
            int agilityBonus = gatherer.Agility;

            foreach (var resource in availableResources)
            {
                double successChance = (resource.BaseSuccessRate + agilityBonus) * weatherModifier / 100.0;
                successChance = Math.Min(0.95, Math.Max(0.05, successChance)); // Cap between 5% and 95%

                if (rng.NextDouble() <= successChance)
                {
                    int amount = rng.Next(resource.MinAmount, resource.MaxAmount + 1);
                    foundResources.Add((resource.Name, amount));
                }
            }

            if (foundResources.Count == 0)
            {
                Console.WriteLine($"❌ {gatherer.Name} returns empty-handed.");
                return;
            }

            Console.WriteLine($"\n✅ {gatherer.Name} found:");
            foreach (var (name, amount) in foundResources)
            {
                Console.WriteLine($"   📦 {amount}x {name}");

                // Add to inventory
                for (int i = 0; i < amount; i++)
                {
                    var item = new GenericItem(name, 10);
                    if (!gatherer.Inventory.AddItem(item))
                    {
                        Console.WriteLine($"   ⚠️ {gatherer.Name}'s inventory is full! {name} was left behind.");
                        break;
                    }
                }
            }
        }

        public static TerrainType DetermineCurrentTerrain(int x, int y)
        {
            // Match the terrain generation logic from FogOfWarMap

            // Northwest Mountains (10-35x, 0-15y)
            if (x >= 10 && x <= 35 && y >= 0 && y <= 15)
            {
                var rng = new Random(42 + x * 100 + y);
                return rng.Next(100) < 70 ? TerrainType.Mountains : TerrainType.Forest;
            }

            // Northeast Coast (70-99x, 0-20y)
            if (x >= 70 && y <= 20)
            {
                return TerrainType.Water;
            }

            // Southeast Desert (70-99x, 21-39y)
            if (x >= 70 && y >= 21)
            {
                return TerrainType.Desert;
            }

            // Southwest Forest (0-25x, 25-39y)
            if (x <= 25 && y >= 25)
            {
                return TerrainType.Forest;
            }

            // South-Central Volcanic (25-45x, 30-39y)
            if (x >= 25 && x <= 45 && y >= 30)
            {
                return TerrainType.Volcanic;
            }

            // North-Central Cliffs (55-75x, 0-10y)
            if (x >= 55 && x <= 75 && y <= 10)
            {
                return TerrainType.Mountains;
            }

            // Default to Plains
            return TerrainType.Plains;
        }

        private static double GetWeatherModifier(Weather? weather)
        {
            if (weather == null) return 1.0;

            return weather.CurrentWeather switch
            {
                WeatherType.Clear => 1.2,
                WeatherType.Rainy => 0.8,
                WeatherType.Stormy => 0.5,
                WeatherType.Foggy => 0.9,
                WeatherType.Snowy => 0.7,
                WeatherType.Windy => 0.9,
                _ => 1.0
            };
        }

        private static string GetWeatherMessage(Weather? weather)
        {
            if (weather == null) return "";

            return weather.CurrentWeather switch
            {
                WeatherType.Clear => "☀️ Perfect weather for gathering! (+20% success)",
                WeatherType.Rainy => "🌧️ The rain makes gathering harder. (-20% success)",
                WeatherType.Stormy => "⛈️ The storm severely hampers gathering! (-50% success)",
                WeatherType.Foggy => "🌫️ Fog obscures resources. (-10% success)",
                WeatherType.Snowy => "❄️ Snow covers many resources. (-30% success)",
                WeatherType.Windy => "💨 Strong winds make gathering difficult. (-10% success)",
                _ => ""
            };
        }

        private static string GetTerrainDescription(TerrainType terrain)
        {
            return terrain switch
            {
                TerrainType.Mountains => "⛰️ Rocky mountains and cliffs",
                TerrainType.Forest => "🌲 Dense woodlands",
                TerrainType.Desert => "🏜️ Arid sandy dunes",
                TerrainType.Volcanic => "🌋 Volcanic lava fields",
                TerrainType.Water => "🌊 Coastal waters",
                TerrainType.Plains => "🌾 Open grasslands",
                _ => "Unknown terrain"
            };
        }

        public static List<ResourceNode> GetResourcesForTerrain(TerrainType terrain)
        {
            return ResourceDatabase.Where(r => r.Terrain == terrain).ToList();
        }

        public static void DisplayResourceGuide()
        {
            Console.WriteLine("\n╔════════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                    📖 RESOURCE GATHERING GUIDE                      ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════════════╝");

            var terrains = Enum.GetValues<TerrainType>();
            foreach (var terrain in terrains)
            {
                var resources = GetResourcesForTerrain(terrain);
                if (resources.Count == 0) continue;

                Console.WriteLine($"\n{GetTerrainDescription(terrain)}:");
                foreach (var res in resources.OrderByDescending(r => r.BaseSuccessRate))
                {
                    var profIcon = res.RequiredProfession != CraftingProfession.None
                        ? $" - {ProfessionManager.GetProfessionIcon(res.RequiredProfession)} {res.RequiredProfession}"
                        : " - Anyone";
                    Console.WriteLine($"   • {res.Name}{profIcon} ({res.BaseSuccessRate}% base)");
                }
            }

            Console.WriteLine("\n💡 Tips:");
            Console.WriteLine("   • Higher Agility = Better gathering success");
            Console.WriteLine("   • Clear weather improves gathering (+20%)");
            Console.WriteLine("   • Storms reduce gathering success (-50%)");
            Console.WriteLine("   • Profession training unlocks rare resources");

            Console.WriteLine("\n\nPress Enter to continue...");
            Console.ReadLine();
        }
    }

    #endregion
}
