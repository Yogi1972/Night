using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rpg_Dungeon
{
    internal class WorldGenerator
    {
        private readonly int _seed;
        private readonly Random _rng;

        public int Seed => _seed;

        public WorldGenerator(int? seed = null)
        {
            _seed = seed ?? GenerateRandomSeed();
            _rng = new Random(_seed);
        }

        private static int GenerateRandomSeed()
        {
            return (int)(DateTime.Now.Ticks & 0x7FFFFFFF);
        }

        public static string SeedToString(int seed)
        {
            return seed.ToString("X8");
        }

        public static int StringToSeed(string seedString)
        {
            if (int.TryParse(seedString, System.Globalization.NumberStyles.HexNumber, null, out int seed))
            {
                return seed;
            }
            return GetHashCode(seedString);
        }

        private static int GetHashCode(string str)
        {
            unchecked
            {
                int hash = 23;
                foreach (char c in str)
                {
                    hash = hash * 31 + c;
                }
                return hash;
            }
        }

        #region Major Towns Generation

        public List<MajorTown> GenerateMajorTowns()
        {
            var towns = new List<MajorTown>();
            var townData = GetTownTemplates();

            int townCount = _rng.Next(6, 10);
            var selectedTowns = townData.OrderBy(x => _rng.Next()).Take(townCount).ToList();

            foreach (var (name, desc, specialty, baseLevel) in selectedTowns)
            {
                var levelVariation = _rng.Next(-2, 3);
                var finalLevel = Math.Max(1, baseLevel + levelVariation);
                var town = new MajorTown(name, desc, specialty, finalLevel);

                if (towns.Count == 0)
                {
                    town.IsDiscovered = true;
                }

                towns.Add(town);
            }

            return towns.OrderBy(t => t.RequiredLevel).ToList();
        }

        private List<(string name, string desc, string specialty, int level)> GetTownTemplates()
        {
            return new List<(string, string, string, int)>
            {
                ("Havenbrook", "A bustling trade city at the crossroads of major routes.", "Trade and Commerce", 1),
                ("Ironforge Citadel", "A fortress city built into the mountain, renowned for master craftsmen.", "Blacksmithing and Armor", 10),
                ("Mysthaven", "A mysterious port city shrouded in mist, home to powerful mages.", "Magic and Enchanting", 15),
                ("Sunspire", "A golden city in the desert, famous for exotic goods and treasures.", "Exotic Goods", 20),
                ("Shadowkeep", "A dark gothic city offering elite training and rare artifacts.", "Elite Training", 25),
                ("Crystalshore", "A coastal city with crystalline architecture, famous for jewelry.", "Jewelry and Gemcraft", 12),
                ("Emberpeak", "A city carved into a volcano, where alchemists harness geothermal power.", "Alchemy and Fire Crafts", 18),
                ("Stormwatch", "A fortified city atop cliffs, home to storm mages and navigators.", "Navigation and Storm Magic", 22),
                ("Frostholm", "An ice-bound city of hardy northerners and frost magic.", "Ice Crafting", 16),
                ("Goldenvale", "A prosperous agricultural city surrounded by golden fields.", "Agriculture and Brewing", 8),
                ("Deepstone", "An underground dwarven metropolis of ancient halls.", "Mining and Gems", 14),
                ("Skyreach", "A city built on floating islands connected by bridges.", "Flight and Wind Magic", 24)
            };
        }

        #endregion

        #region Settlements Generation

        public List<Settlement> GenerateSettlements()
        {
            var settlements = new List<Settlement>();
            var settlementTemplates = GetSettlementTemplates();

            int settlementCount = _rng.Next(10, 16);
            var selectedSettlements = settlementTemplates.OrderBy(x => _rng.Next()).Take(settlementCount).ToList();

            foreach (var (name, desc, level) in selectedSettlements)
            {
                var hasInn = _rng.Next(100) < 70;
                var hasBlacksmith = _rng.Next(100) < 50;
                var levelVariation = _rng.Next(-2, 3);
                var finalLevel = Math.Max(1, level + levelVariation);

                settlements.Add(new Settlement(name, desc, finalLevel, hasInn, hasBlacksmith));
            }

            return settlements.OrderBy(s => s.RequiredLevel).ToList();
        }

        private List<(string name, string desc, int level)> GetSettlementTemplates()
        {
            return new List<(string, string, int)>
            {
                ("Willowdale", "A peaceful farming village surrounded by wheat fields.", 1),
                ("Crossroads Keep", "A fortified waystation where three roads meet.", 3),
                ("Pinewood", "A logging community deep in the forest.", 5),
                ("Riverside", "A quiet fishing village by the great river.", 5),
                ("Stonebridge", "A settlement built around an ancient stone bridge.", 8),
                ("Frosthollow", "A cold mountain settlement of hardy folk.", 10),
                ("Oasis Rest", "A desert settlement around a precious water source.", 12),
                ("Moonwell", "A mystical settlement near a magical spring.", 15),
                ("Thornwall", "A walled settlement on the dangerous frontier.", 18),
                ("Ghostlight", "An eerie settlement near the haunted lands.", 20),
                ("Silvermist", "A foggy coastal village where sailors share tales.", 13),
                ("Copperhill", "A mining settlement extracting precious copper.", 16),
                ("Ravencrest", "A fortified settlement on a rocky outcrop.", 24),
                ("Bramblewood", "A hidden village protected by thorny hedges.", 7),
                ("Saltmarsh", "A swampy settlement built on stilts.", 11),
                ("Windmill Crossing", "A settlement powered by giant windmills.", 9)
            };
        }

        #endregion

        #region Camps Generation

        public List<Camp> GenerateCamps()
        {
            var camps = new List<Camp>();
            var campTemplates = GetCampTemplates();

            int campCount = _rng.Next(18, 26);
            var selectedCamps = campTemplates.OrderBy(x => _rng.Next()).Take(campCount).ToList();

            camps.AddRange(selectedCamps);

            return camps;
        }

        private List<Camp> GetCampTemplates()
        {
            return new List<Camp>
            {
                // Roadside Camps
                new Camp("Traveler's Rest", "A common stop for merchants and adventurers.", CampType.Roadside),
                new Camp("Wagon Circle", "A defensive camp formation used by caravans.", CampType.Roadside),
                new Camp("Milestone Camp", "A camp marked by an ancient stone milestone.", CampType.Roadside),
                new Camp("Crossroads Camp", "A busy camp where multiple paths converge.", CampType.Roadside),
                new Camp("Guard Post", "An abandoned guard post turned makeshift camp.", CampType.Roadside),
                new Camp("Merchant's Stop", "A popular rest stop for traveling traders.", CampType.Roadside),

                // Forest Camps
                new Camp("Hunter's Clearing", "A camp used by local hunters.", CampType.Forest),
                new Camp("Woodcutter's Site", "A clearing with stumps and a fire pit.", CampType.Forest),
                new Camp("Druid Circle", "A sacred grove with ancient standing stones.", CampType.Forest),
                new Camp("Ranger Outpost", "A hidden camp used by forest rangers.", CampType.Forest),
                new Camp("Mushroom Grove", "A camp in a grove of giant mushrooms.", CampType.Forest),

                // Mountain Camps
                new Camp("Eagle's Nest", "A high altitude camp with a commanding view.", CampType.Mountain),
                new Camp("Cave Shelter", "A natural cave offering protection from elements.", CampType.Mountain),
                new Camp("Mountain Pass Camp", "A camp at the highest point of the pass.", CampType.Mountain),
                new Camp("Windswept Ridge", "A camp on a windy mountain ridge.", CampType.Mountain),
                new Camp("Avalanche Shelter", "A protected alcove safe from avalanches.", CampType.Mountain),

                // Desert Camps
                new Camp("Dune Hollow", "A depression in the sand providing shelter from wind.", CampType.Desert),
                new Camp("Nomad Circle", "A traditional circular camp of desert nomads.", CampType.Desert),
                new Camp("Rock Shelter", "A camp sheltered by large desert rocks.", CampType.Desert),
                new Camp("Oasis Edge", "A camp on the edge of a small oasis.", CampType.Desert),

                // Riverside Camps
                new Camp("Fisher's Camp", "A camp by the water with drying racks.", CampType.Riverside),
                new Camp("Ferry Landing", "A camp near an old ferry crossing.", CampType.Riverside),
                new Camp("Beaver Dam", "A camp near a large beaver dam.", CampType.Riverside),
                new Camp("Seaside Camp", "A coastal camp with the sound of crashing waves.", CampType.Riverside),
                new Camp("Bridge Camp", "A camp beneath an old stone bridge.", CampType.Riverside),

                // Ruins Camps
                new Camp("Temple Steps", "A camp among crumbling temple ruins.", CampType.Ruins),
                new Camp("Old Fort", "A camp within the walls of an ancient fort.", CampType.Ruins),
                new Camp("Merchant's Rest", "A camp in the ruins of an old trading post.", CampType.Ruins),
                new Camp("Tower Ruins", "A camp in the shadow of a ruined tower.", CampType.Ruins)
            };
        }

        #endregion

        #region Enemy Camps Generation

        public List<EnemyCamp> GenerateEnemyCamps()
        {
            var enemyCamps = new List<EnemyCamp>();

            enemyCamps.AddRange(GenerateEnemyCampsByDifficulty(EnemyCampDifficulty.Weak, 3, 5, 3, 5));
            enemyCamps.AddRange(GenerateEnemyCampsByDifficulty(EnemyCampDifficulty.Normal, 5, 8, 5, 7));
            enemyCamps.AddRange(GenerateEnemyCampsByDifficulty(EnemyCampDifficulty.Strong, 10, 15, 4, 5));
            enemyCamps.AddRange(GenerateEnemyCampsByDifficulty(EnemyCampDifficulty.Dangerous, 15, 20, 3, 5));
            enemyCamps.AddRange(GenerateEnemyCampsByDifficulty(EnemyCampDifficulty.Deadly, 20, 25, 2, 3));
            enemyCamps.AddRange(GenerateEnemyCampsByDifficulty(EnemyCampDifficulty.Elite, 25, 30, 1, 2));

            return enemyCamps;
        }

        private List<EnemyCamp> GenerateEnemyCampsByDifficulty(EnemyCampDifficulty difficulty,
            int minLevel, int maxLevel, int minCount, int maxCount)
        {
            var camps = new List<EnemyCamp>();
            int count = _rng.Next(minCount, maxCount + 1);

            var campTypes = Enum.GetValues(typeof(EnemyCampType)).Cast<EnemyCampType>().ToList();

            for (int i = 0; i < count; i++)
            {
                var campType = campTypes[_rng.Next(campTypes.Count)];
                var level = _rng.Next(minLevel, maxLevel + 1);
                var (name, description) = GenerateEnemyCampDetails(campType, difficulty);

                camps.Add(new EnemyCamp(name, description, campType, difficulty, level));
            }

            return camps;
        }

        private (string name, string description) GenerateEnemyCampDetails(EnemyCampType type, EnemyCampDifficulty difficulty)
        {
            var prefixes = difficulty switch
            {
                EnemyCampDifficulty.Weak => new[] { "Small", "Ragged", "Crude", "Lesser" },
                EnemyCampDifficulty.Normal => new[] { "Hidden", "Dark", "Forgotten", "Abandoned" },
                EnemyCampDifficulty.Strong => new[] { "Fortified", "Wicked", "Ancient", "Cursed" },
                EnemyCampDifficulty.Dangerous => new[] { "Notorious", "Bloodstained", "Dread", "Infernal" },
                EnemyCampDifficulty.Deadly => new[] { "Legendary", "Terrifying", "Apocalyptic", "Doomed" },
                EnemyCampDifficulty.Elite => new[] { "Mythical", "Cataclysmic", "Supreme", "Ultimate" },
                _ => new[] { "Unknown" }
            };

            var prefix = prefixes[_rng.Next(prefixes.Length)];

            var (baseName, baseDesc) = type switch
            {
                EnemyCampType.GoblinWarcamp => ("Goblin Warcamp", "where goblins plan their raids"),
                EnemyCampType.BanditHideout => ("Bandit Hideout", "where outlaws gather and scheme"),
                EnemyCampType.UndeadGraveyard => ("Undead Graveyard", "where the dead refuse to rest"),
                EnemyCampType.OrcStronghold => ("Orc Stronghold", "a militarized orc encampment"),
                EnemyCampType.CultistShrine => ("Cult Shrine", "where dark rituals are performed"),
                EnemyCampType.DemonPortal => ("Demon Portal", "a gateway leaking demonic entities"),
                EnemyCampType.DragonLair => ("Dragon Lair", "the domain of an ancient dragon"),
                EnemyCampType.BeastDen => ("Beast Den", "where savage creatures make their home"),
                EnemyCampType.SpiderNest => ("Spider Nest", "a webbed area infested with giant spiders"),
                EnemyCampType.ElementalNexus => ("Elemental Nexus", "where elemental forces clash"),
                _ => ("Enemy Camp", "a dangerous enemy position")
            };

            var name = $"{prefix} {baseName}";
            var description = $"A {difficulty.ToString().ToLower()} camp {baseDesc}.";

            return (name, description);
        }

        #endregion

        #region Areas Generation

        public List<Area> GenerateAreas(Weather weather, TimeOfDay timeTracker)
        {
            var areas = new List<Area>();
            var areaTemplates = GetAreaTemplates();

            int areaCount = _rng.Next(8, 13);
            var selectedAreas = areaTemplates.OrderBy(x => _rng.Next()).Take(areaCount).ToList();

            foreach (var (name, desc, baseLevel) in selectedAreas)
            {
                var levelVariation = _rng.Next(-2, 3);
                var finalLevel = Math.Max(1, baseLevel + levelVariation);
                var area = new Area(name, desc, finalLevel, finalLevel == 1);

                area.SetWeather(weather);
                area.SetTimeTracker(timeTracker);

                GenerateDungeonsForArea(area);
                GenerateQuestSpotsForArea(area);

                areas.Add(area);
            }

            return areas.OrderBy(a => a.RecommendedLevel).ToList();
        }

        private List<(string name, string desc, int level)> GetAreaTemplates()
        {
            return new List<(string, string, int)>
            {
                ("Whispering Woods", "A peaceful forest near the town, perfect for beginners.", 1),
                ("Shadowfen Marsh", "A murky swamp filled with dangerous creatures.", 5),
                ("Frostpeak Mountains", "Treacherous mountain paths with ancient ruins.", 8),
                ("Scorching Sands", "An endless desert with buried secrets.", 10),
                ("Deathwhisper Graveyard", "A cursed cemetery where the dead don't rest.", 12),
                ("Ashfall Crater", "A volcanic region with rivers of lava.", 15),
                ("Moonlight Glade", "A magical forest where reality bends.", 18),
                ("Stormbreaker Shores", "Treacherous cliffs overlooking a stormy sea.", 20),
                ("Blightlands", "A corrupted region twisted by dark magic.", 25),
                ("Luminous Depths", "Underground caverns filled with glowing crystals.", 28),
                ("Fields of Valor", "The site of a legendary battle from ages past.", 32),
                ("Starfall Peak", "The highest peak where stars touch the earth.", 35),
                ("Thornwood Wilds", "A dense forest with carnivorous plants.", 6),
                ("Crimson Canyon", "A red rock canyon with bandit activity.", 13),
                ("Mistral Plateau", "A high windswept plateau with ancient monoliths.", 22)
            };
        }

        private void GenerateDungeonsForArea(Area area)
        {
            int dungeonCount = _rng.Next(2, 5);
            var dungeonNames = GetDungeonNamesByTheme(area.Name);

            for (int i = 0; i < dungeonCount; i++)
            {
                if (i < dungeonNames.Count)
                {
                    var levelOffset = _rng.Next(-1, 3);
                    var dungeonLevel = Math.Max(1, area.RecommendedLevel + levelOffset);
                    var floors = CalculateFloors(dungeonLevel);
                    var dungeonSeed = _rng.Next();

                    area.Dungeons.Add(new DungeonLocation(dungeonNames[i], dungeonLevel, floors, dungeonSeed));
                }
            }
        }

        private List<string> GetDungeonNamesByTheme(string areaName)
        {
            var generic = new[] { "Cave", "Lair", "Den", "Ruins", "Cavern", "Crypt", "Tomb", "Mine", "Temple", "Fortress" };
            var prefix = new[] { "Dark", "Ancient", "Lost", "Forgotten", "Cursed", "Hidden", "Deep", "Shadow", "Twisted", "Haunted" };

            var dungeonNames = new List<string>();
            for (int i = 0; i < 5; i++)
            {
                var p = prefix[_rng.Next(prefix.Length)];
                var g = generic[_rng.Next(generic.Length)];
                dungeonNames.Add($"{p} {g}");
            }

            return dungeonNames;
        }

        private int CalculateFloors(int dungeonLevel)
        {
            if (dungeonLevel < 5) return _rng.Next(2, 4);
            if (dungeonLevel < 10) return _rng.Next(3, 5);
            if (dungeonLevel < 15) return _rng.Next(4, 6);
            if (dungeonLevel < 25) return _rng.Next(5, 7);
            return _rng.Next(6, 9);
        }

        private void GenerateQuestSpotsForArea(Area area)
        {
            int questSpotCount = _rng.Next(2, 4);
            var questObjectives = new[] { "Healing Herb", "Mana Crystal", "Dungeon Level", "Goblin", "Skeleton", "Dragon", "Orc" };

            for (int i = 0; i < questSpotCount; i++)
            {
                var spotName = GenerateQuestSpotName();
                var spotDesc = GenerateQuestSpotDescription();
                var objective = questObjectives[_rng.Next(questObjectives.Length)];

                area.QuestSpots.Add(new QuestSpot(spotName, spotDesc, objective));
            }
        }

        private string GenerateQuestSpotName()
        {
            var adjectives = new[] { "Ancient", "Forgotten", "Hidden", "Sacred", "Mystical", "Cursed", "Lost", "Abandoned" };
            var locations = new[] { "Shrine", "Altar", "Grove", "Spring", "Monument", "Ruins", "Circle", "Well", "Tower", "Pillar" };

            return $"{adjectives[_rng.Next(adjectives.Length)]} {locations[_rng.Next(locations.Length)]}";
        }

        private string GenerateQuestSpotDescription()
        {
            var descriptions = new[]
            {
                "A mysterious location that may hold secrets.",
                "An interesting place worth investigating.",
                "A site of ancient power.",
                "A landmark of historical significance.",
                "A place where adventures often begin.",
                "A location marked on old treasure maps.",
                "A spot where strange energies converge."
            };

            return descriptions[_rng.Next(descriptions.Length)];
        }

        #endregion

        #region Distance Calculation

        public void SetupDistances(List<Location> allLocations, int? distanceSeed = null)
        {
            var distanceRng = new Random(distanceSeed ?? _seed);

            for (int i = 0; i < allLocations.Count; i++)
            {
                for (int j = i + 1; j < allLocations.Count; j++)
                {
                    var loc1 = allLocations[i];
                    var loc2 = allLocations[j];

                    int distance = CalculateDistance(loc1, loc2, distanceRng);

                    loc1.DistancesToOtherLocations[loc2.Name] = distance;
                    loc2.DistancesToOtherLocations[loc1.Name] = distance;
                }
            }
        }

        private int CalculateDistance(Location loc1, Location loc2, Random rng)
        {
            int baseLevelDiff = Math.Abs(loc1.RequiredLevel - loc2.RequiredLevel);
            int baseDistance = baseLevelDiff * 10 + rng.Next(10, 50);

            if (loc1.Type == LocationCategory.Town && loc2.Type == LocationCategory.Town)
            {
                baseDistance += rng.Next(50, 100);
            }
            else if (loc1.Type == LocationCategory.Camp || loc2.Type == LocationCategory.Camp)
            {
                baseDistance = (int)(baseDistance * 0.7);
            }

            return Math.Max(5, baseDistance);
        }

        #endregion
    }
}
