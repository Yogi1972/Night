using Night.Characters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rpg_Dungeon
{
    #region Enemy Camp Difficulty Enum

    internal enum EnemyCampDifficulty
    {
        Weak,       // Level 1-5 enemies
        Normal,     // Level 5-10 enemies
        Strong,     // Level 10-15 enemies
        Dangerous,  // Level 15-20 enemies
        Deadly,     // Level 20-25 enemies
        Elite       // Level 25+ enemies
    }

    #endregion

    #region Enemy Camp Type Enum

    internal enum EnemyCampType
    {
        BanditHideout,      // Human bandits and thieves
        GoblinWarcamp,      // Goblin raiders
        OrcStronghold,      // Orc warriors
        UndeadGraveyard,    // Undead creatures
        BeastDen,           // Wild beasts and monsters
        CultistShrine,      // Dark cultists
        DragonLair,         // Dragon and dragonkin
        DemonPortal,        // Demonic entities
        ElementalNexus,     // Elemental creatures
        SpiderNest          // Giant spiders
    }

    #endregion

    #region Enemy Camp Class

    internal class EnemyCamp : Location
    {
        #region Constants

        private const int WAVE_LOOT_CHANCE = 30; // Chance for loot after each wave
        private const int BONUS_LOOT_ROLLS = 2; // Extra loot rolls for boss
        private const int CLEARED_CAMP_SEARCH_CHANCE = 25; // Reduced since main loot improved
        private const int CLEARED_CAMP_SEARCH_MIN_GOLD = 20;
        private const int CLEARED_CAMP_SEARCH_MAX_GOLD = 50;
        private const double CLEARED_CAMP_REST_RECOVERY = 0.5; // 50%
        private const double WAVE_QUICK_RECOVERY = 0.1; // 10%

        #endregion

        #region Properties

        public EnemyCampType CampType { get; }
        public EnemyCampDifficulty Difficulty { get; }
        public int EnemyCount { get; }
        public bool IsCleared { get; private set; }
        public string BossName { get; }
        public int GoldReward { get; }
        public int ExperienceReward { get; }

        #endregion

        #region Fields

        private readonly Random _rng;
        private readonly CampLootTable _lootTable;
        private int _totalLootRolls; // Track accumulated loot from waves

        #endregion

        #region Constructor

        public EnemyCamp(string name, string description, EnemyCampType campType, EnemyCampDifficulty difficulty, int requiredLevel = 1)
            : base(name, description, LocationCategory.Dungeon, requiredLevel)
        {
            _rng = new Random(Guid.NewGuid().GetHashCode());
            CampType = campType;
            Difficulty = difficulty;
            IsCleared = false;
            EnemyCount = CalculateEnemyCount(difficulty);
            BossName = GenerateBossName(campType);
            (GoldReward, ExperienceReward) = CalculateRewards(difficulty);
            _lootTable = new CampLootTable(campType, difficulty, _rng);
            _totalLootRolls = 0;
        }

        #endregion

        #region Initialization

        private int CalculateEnemyCount(EnemyCampDifficulty difficulty)
        {
            return difficulty switch
            {
                EnemyCampDifficulty.Weak => _rng.Next(3, 6),
                EnemyCampDifficulty.Normal => _rng.Next(5, 8),
                EnemyCampDifficulty.Strong => _rng.Next(7, 10),
                EnemyCampDifficulty.Dangerous => _rng.Next(9, 12),
                EnemyCampDifficulty.Deadly => _rng.Next(11, 15),
                EnemyCampDifficulty.Elite => _rng.Next(13, 18),
                _ => 5
            };
        }

        private string GenerateBossName(EnemyCampType campType)
        {
            return campType switch
            {
                EnemyCampType.BanditHideout => "Bandit Chief",
                EnemyCampType.GoblinWarcamp => "Goblin Warlord",
                EnemyCampType.OrcStronghold => "Orc Chieftain",
                EnemyCampType.UndeadGraveyard => "Lich Lord",
                EnemyCampType.BeastDen => "Alpha Beast",
                EnemyCampType.CultistShrine => "Dark High Priest",
                EnemyCampType.DragonLair => "Young Dragon",
                EnemyCampType.DemonPortal => "Demon Commander",
                EnemyCampType.ElementalNexus => "Elemental Lord",
                EnemyCampType.SpiderNest => "Brood Mother",
                _ => "Camp Leader"
            };
        }

        private (int gold, int xp) CalculateRewards(EnemyCampDifficulty difficulty)
        {
            return difficulty switch
            {
                EnemyCampDifficulty.Weak => (50, 100),
                EnemyCampDifficulty.Normal => (100, 200),
                EnemyCampDifficulty.Strong => (200, 400),
                EnemyCampDifficulty.Dangerous => (350, 700),
                EnemyCampDifficulty.Deadly => (500, 1000),
                EnemyCampDifficulty.Elite => (800, 1500),
                _ => (100, 200)
            };
        }

        #endregion

        #region Methods

        public override void Enter(List<Character> party, GameState gameState)
        {
            if (party == null || party.Count == 0)
            {
                throw new ArgumentException("Party cannot be null or empty", nameof(party));
            }

            if (!IsDiscovered)
            {
                IsDiscovered = true;
                Console.WriteLine($"\n⚔️ Enemy camp discovered: {Name}!");
                Console.WriteLine($"   ⚠️ Danger Level: {Difficulty}");

                // Discover on fog of war map
                gameState.FogOfWarMap?.DiscoverLocation(Name);
            }

            gameState.FogOfWarMap?.SetCurrentLocation(Name);

            while (true)
            {
                Console.Clear();
                Console.WriteLine($"\n╔══════════════════════════════════════════════════════════════╗");
                Console.WriteLine($"║        ⚔️ {Name,-50}║");
                Console.WriteLine($"╚══════════════════════════════════════════════════════════════╝");
                Console.WriteLine($"🏴 {Description}");
                Console.WriteLine($"   Camp Type: {GetCampTypeDescription()}");
                Console.WriteLine($"   Difficulty: {GetDifficultyDescription()}");
                Console.WriteLine($"   Recommended Level: {RequiredLevel}+");

                if (IsCleared)
                {
                    Console.WriteLine($"\n   ✅ CLEARED - This camp has been conquered!");
                }
                else
                {
                    Console.WriteLine($"\n   ⚠️ HOSTILE - Estimated {EnemyCount} enemies + Boss ({BossName})");
                    Console.WriteLine($"   💰 Potential Rewards: {GoldReward} gold, {ExperienceReward} XP");
                }

                Console.WriteLine();

                if (IsCleared)
                {
                    Console.WriteLine("--- Camp Options (Cleared) ---");
                    Console.WriteLine("1) Search for Leftover Loot");
                    Console.WriteLine("2) Rest at Cleared Camp (50% HP/Mana recovery)");
                    Console.WriteLine("3) Survey the Area");
                    Console.WriteLine("0) Leave Camp");
                }
                else
                {
                    Console.WriteLine("--- Attack Options ---");
                    Console.WriteLine("1) ⚔️ ASSAULT THE CAMP (Start Combat)");
                    Console.WriteLine("2) 🔍 Scout the Enemy (View details)");
                    Console.WriteLine("3) 🏃 Retreat Safely");
                    Console.WriteLine("0) Leave Area");
                }

                Console.Write("Choice: ");
                var choice = Console.ReadLine()?.Trim() ?? "";

                if (IsCleared)
                {
                    switch (choice)
                    {
                        case "1":
                            SearchForLoot(party);
                            break;
                        case "2":
                            RestAtClearedCamp(party);
                            break;
                        case "3":
                            SurveyArea();
                            break;
                        case "0":
                            Console.WriteLine($"\nYou leave the cleared camp.");
                            return;
                        default:
                            Console.WriteLine("Invalid choice.");
                            Console.ReadKey();
                            break;
                    }
                }
                else
                {
                    switch (choice)
                    {
                        case "1":
                            AssaultCamp(party, gameState);
                            break;
                        case "2":
                            ScoutEnemies(party);
                            break;
                        case "3":
                            Console.WriteLine("\n🏃 You carefully retreat from the enemy camp...");
                            Console.ReadKey();
                            return;
                        case "0":
                            Console.WriteLine($"\nYou leave the area.");
                            return;
                        default:
                            Console.WriteLine("Invalid choice.");
                            Console.ReadKey();
                            break;
                    }
                }
            }
        }

        private void AssaultCamp(List<Character> party, GameState gameState)
        {
            Console.Clear();
            Console.WriteLine($"\n╔══════════════════════════════════════════════════════════════╗");
            Console.WriteLine($"║                   ⚔️ ASSAULT INITIATED ⚔️                    ║");
            Console.WriteLine($"╚══════════════════════════════════════════════════════════════╝");
            Console.WriteLine($"\n⚠️ You charge into {Name}!");
            Console.WriteLine($"The enemy prepares to defend their camp!\n");

            // Warn if party is under-leveled
            if (party.Max(p => p.Level) < RequiredLevel)
            {
                Console.WriteLine($"⚠️ WARNING: Your party may be under-leveled for this camp!");
                Console.Write("Continue anyway? (y/n): ");
                if (Console.ReadLine()?.Trim().ToLower() != "y")
                {
                    Console.WriteLine("Wisely retreating...");
                    Console.ReadKey();
                    return;
                }
            }

            Console.ReadKey();

            // Generate multiple encounters
            int waveCount = CalculateWaveCount();
            bool victory = true;

            for (int wave = 1; wave <= waveCount; wave++)
            {
                Console.Clear();
                Console.WriteLine($"\n═══════════════════════════════════════");
                Console.WriteLine($"   WAVE {wave} of {waveCount}");
                Console.WriteLine($"═══════════════════════════════════════\n");

                var encounter = new Encounter();
                bool isBossWave = (wave == waveCount);

                if (isBossWave)
                {
                    Console.WriteLine($"💀 The {BossName} emerges to defend the camp!");
                    encounter.GenerateEncounter(party, EncounterDifficulty.Boss);
                }
                else
                {
                    var waveDifficulty = Difficulty switch
                    {
                        EnemyCampDifficulty.Weak => EncounterDifficulty.Easy,
                        EnemyCampDifficulty.Normal => EncounterDifficulty.Normal,
                        EnemyCampDifficulty.Strong => EncounterDifficulty.Normal,
                        EnemyCampDifficulty.Dangerous => EncounterDifficulty.Hard,
                        EnemyCampDifficulty.Deadly => EncounterDifficulty.Hard,
                        EnemyCampDifficulty.Elite => EncounterDifficulty.Elite,
                        _ => EncounterDifficulty.Normal
                    };
                    encounter.GenerateEncounter(party, waveDifficulty);
                }

                Console.ReadKey();

                var battleResult = encounter.StartEncounter(party);

                if (battleResult)
                {
                    Console.WriteLine($"\n✅ Wave {wave} cleared!");

                    // Wave loot (not for boss wave, handled separately)
                    if (!isBossWave)
                    {
                        if (_rng.Next(100) < WAVE_LOOT_CHANCE)
                        {
                            _totalLootRolls++; // Accumulate for final loot distribution
                            Console.WriteLine("💎 Your party found some loot after the battle!");
                        }
                    }

                    if (!isBossWave && wave < waveCount)
                    {
                        Console.WriteLine("\n--- Quick Rest ---");
                        Console.WriteLine("Your party catches their breath before the next wave...");
                        foreach (var member in party.Where(m => m.IsAlive))
                        {
                            int quickHeal = (int)(member.GetTotalMaxHP() * WAVE_QUICK_RECOVERY);
                            int quickMana = (int)(member.GetTotalMaxMana() * WAVE_QUICK_RECOVERY);
                            member.Heal(quickHeal);
                            member.RestoreMana(quickMana);
                        }
                        Console.WriteLine($"(Recovered {WAVE_QUICK_RECOVERY * 100}% HP and Mana)");
                    }

                    Console.ReadKey();
                }
                else
                {
                    Console.WriteLine($"\n❌ Your party was defeated!");
                    Console.WriteLine($"You retreat from {Name} to lick your wounds...");
                    victory = false;
                    Console.ReadKey();
                    break;
                }
            }

            if (victory)
            {
                IsCleared = true;
                Console.Clear();
                Console.WriteLine($"\n╔══════════════════════════════════════════════════════════════╗");
                Console.WriteLine($"║                   🎉 VICTORY! 🎉                             ║");
                Console.WriteLine($"╚══════════════════════════════════════════════════════════════╝");
                Console.WriteLine($"\n🏆 {Name} has been conquered!");
                Console.WriteLine($"The enemy camp is cleared and no longer a threat.\n");

                DistributeRewards(party);

                // Achievement tracking
                gameState.AchievementTracker?.TrackProgress("ClearEnemyCamps", 1);

                Console.ReadKey();
            }
        }

        private int CalculateWaveCount()
        {
            return Difficulty switch
            {
                EnemyCampDifficulty.Weak => 2,
                EnemyCampDifficulty.Normal => 2,
                EnemyCampDifficulty.Strong => 3,
                EnemyCampDifficulty.Dangerous => 3,
                EnemyCampDifficulty.Deadly => 4,
                EnemyCampDifficulty.Elite => 4,
                _ => 2
            };
        }

        private void DistributeRewards(List<Character> party)
        {
            Console.WriteLine("--- Rewards ---");

            // Gold distribution
            int goldPerMember = GoldReward / party.Count;
            foreach (var member in party)
            {
                member.Inventory.AddGold(goldPerMember);
            }
            Console.WriteLine($"💰 Gold: {GoldReward} ({goldPerMember} per member)");

            // Experience distribution (only alive members)
            int xpPerMember = ExperienceReward / party.Count;
            foreach (var member in party.Where(m => m.IsAlive))
            {
                member.GainExperience(xpPerMember);
            }
            Console.WriteLine($"⭐ Experience: {ExperienceReward} ({xpPerMember} per member)");

            Console.WriteLine("\n--- Loot Distribution ---");

            // Generate loot from accumulated wave rolls
            var waveLoot = _lootTable.GenerateLoot(isBossLoot: false, bonusRolls: _totalLootRolls);

            // Generate boss loot (guaranteed better drops)
            var bossLoot = _lootTable.GenerateLoot(isBossLoot: true, bonusRolls: BONUS_LOOT_ROLLS);

            // Combine all loot
            var allLoot = waveLoot.Concat(bossLoot).ToList();

            if (allLoot.Count == 0)
            {
                Console.WriteLine("No special items were found.");
            }
            else
            {
                Console.WriteLine($"🎁 Found {allLoot.Count} item(s):\n");

                // Distribute items to party members
                int memberIndex = 0;
                var itemsAdded = new List<string>();
                var itemsLost = new List<string>();

                foreach (var item in allLoot)
                {
                    // Try to add to current member, if full try next member
                    bool added = false;
                    for (int i = 0; i < party.Count && !added; i++)
                    {
                        int targetIndex = (memberIndex + i) % party.Count;
                        if (party[targetIndex].Inventory.AddItem(item))
                        {
                            var rarity = DetermineItemRarity(item);
                            var rarityIcon = CampLootTable.GetRarityColor(rarity);
                            itemsAdded.Add($"   {rarityIcon} {item.Name} → {party[targetIndex].Name}");
                            added = true;
                            memberIndex = (targetIndex + 1) % party.Count; // Round-robin
                        }
                    }

                    if (!added)
                    {
                        itemsLost.Add($"   ❌ {item.Name} (no inventory space)");
                    }
                }

                // Display acquired items
                foreach (var msg in itemsAdded)
                {
                    Console.WriteLine(msg);
                }

                // Display lost items
                if (itemsLost.Count > 0)
                {
                    Console.WriteLine("\n⚠️ Items left behind:");
                    foreach (var msg in itemsLost)
                    {
                        Console.WriteLine(msg);
                    }
                }
            }

            Console.WriteLine();
            _totalLootRolls = 0; // Reset for next run
        }

        private LootRarity DetermineItemRarity(Item item)
        {
            // Simple heuristic based on item properties
            if (item is Equipment equipment)
            {
                int totalStats = equipment.StrengthBonus + equipment.AgilityBonus +
                                equipment.IntelligenceBonus + equipment.ArmorBonus;

                if (totalStats >= 12) return LootRarity.Legendary;
                if (totalStats >= 8) return LootRarity.Epic;
                if (totalStats >= 5) return LootRarity.Rare;
                if (totalStats >= 3) return LootRarity.Uncommon;
                return LootRarity.Common;
            }
            else if (item.Price >= 400)
            {
                return LootRarity.Epic;
            }
            else if (item.Price >= 200)
            {
                return LootRarity.Rare;
            }
            else if (item.Price >= 50)
            {
                return LootRarity.Uncommon;
            }

            return LootRarity.Common;
        }

        private void ScoutEnemies(List<Character> party)
        {
            Console.Clear();
            Console.WriteLine($"\n╔══════════════════════════════════════════════════════════════╗");
            Console.WriteLine($"║                   🔍 SCOUTING REPORT 🔍                      ║");
            Console.WriteLine($"╚══════════════════════════════════════════════════════════════╝");
            Console.WriteLine($"\nCamp: {Name}");
            Console.WriteLine($"Type: {GetCampTypeDescription()}");
            Console.WriteLine($"Difficulty: {GetDifficultyDescription()}");
            Console.WriteLine($"\n--- Enemy Intel ---");
            Console.WriteLine($"👥 Estimated Forces: {EnemyCount} enemies");
            Console.WriteLine($"👑 Leader: {BossName}");
            Console.WriteLine($"⚔️ Combat Waves: {CalculateWaveCount()}");
            Console.WriteLine($"\n--- Potential Rewards ---");
            Console.WriteLine($"💰 Gold: ~{GoldReward}");
            Console.WriteLine($"⭐ Experience: ~{ExperienceReward}");
            Console.WriteLine($"🎁 Loot: Wave drops (30% per wave) + Boss loot (guaranteed)");
            Console.WriteLine($"   Camp-specific items from {GetCampTypeDescription()}");
            Console.WriteLine($"   Possible drops: Equipment, Materials, Consumables");
            Console.WriteLine($"   Rarities: ⚪Common 🟢Uncommon 🔵Rare 🟣Epic 🟠Legendary");
            Console.WriteLine($"\n--- Party Assessment ---");

            int avgPartyLevel = party.Sum(p => p.Level) / party.Count;
            int levelDifference = avgPartyLevel - RequiredLevel;

            if (levelDifference >= 5)
            {
                Console.WriteLine("✅ Your party is well-prepared for this camp.");
            }
            else if (levelDifference >= 0)
            {
                Console.WriteLine("⚠️ Your party meets the requirements, but be careful.");
            }
            else if (levelDifference >= -3)
            {
                Console.WriteLine("⚠️ This will be challenging! Prepare thoroughly.");
            }
            else
            {
                Console.WriteLine("❌ This camp is too dangerous for your current level!");
            }

            Console.ReadKey();
        }

        private void SearchForLoot(List<Character> party)
        {
            Console.WriteLine("\n🔍 You search through the abandoned camp...");

            if (_rng.Next(100) < CLEARED_CAMP_SEARCH_CHANCE)
            {
                // Small chance to find leftover items
                var leftoverLoot = _lootTable.GenerateLoot(isBossLoot: false, bonusRolls: 0);

                int bonusGold = _rng.Next(CLEARED_CAMP_SEARCH_MIN_GOLD, CLEARED_CAMP_SEARCH_MAX_GOLD);
                int goldPerMember = bonusGold / party.Count;

                foreach (var member in party)
                {
                    member.Inventory.AddGold(goldPerMember);
                }

                Console.WriteLine($"💰 Found {bonusGold} gold coins hidden in the debris!");
                Console.WriteLine($"   ({goldPerMember} per party member)");

                if (leftoverLoot.Count > 0)
                {
                    Console.WriteLine("\n🎁 Also found:");
                    foreach (var item in leftoverLoot)
                    {
                        bool added = false;
                        for (int i = 0; i < party.Count && !added; i++)
                        {
                            if (party[i].Inventory.AddItem(item))
                            {
                                var rarity = DetermineItemRarity(item);
                                var rarityIcon = CampLootTable.GetRarityColor(rarity);
                                Console.WriteLine($"   {rarityIcon} {item.Name} → {party[i].Name}");
                                added = true;
                            }
                        }
                        if (!added)
                        {
                            Console.WriteLine($"   ❌ {item.Name} (no inventory space)");
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("Nothing of value remains.");
            }

            Console.ReadKey();
        }

        private void RestAtClearedCamp(List<Character> party)
        {
            Console.WriteLine("\n🔥 Your party rests at the cleared camp...");
            Console.WriteLine("Without enemies, you can recover more effectively.\n");

            foreach (var member in party)
            {
                int hpRestore = (int)(member.GetTotalMaxHP() * CLEARED_CAMP_REST_RECOVERY);
                int manaRestore = (int)(member.GetTotalMaxMana() * CLEARED_CAMP_REST_RECOVERY);

                member.Heal(hpRestore);
                member.RestoreMana(manaRestore);

                Console.WriteLine($"{member.Name} recovered {hpRestore} HP and {manaRestore} Mana.");
            }

            Console.WriteLine($"\n✨ The party feels refreshed ({CLEARED_CAMP_REST_RECOVERY * 100}% recovery).");
            Console.ReadKey();
        }

        private void SurveyArea()
        {
            Console.WriteLine($"\n👁️ You survey the conquered camp...");

            string[] observations = CampType switch
            {
                EnemyCampType.BanditHideout => new[] {
                    "Stolen goods are scattered everywhere.",
                    "Wanted posters show the bandits' faces.",
                    "A crude map shows other hideouts in the region."
                },
                EnemyCampType.GoblinWarcamp => new[] {
                    "Primitive weapons and crude armor litter the ground.",
                    "The stench of goblin cooking pots lingers.",
                    "Tribal totems mark their territory."
                },
                EnemyCampType.OrcStronghold => new[] {
                    "Massive weapons too heavy for humans lie abandoned.",
                    "Battle standards of defeated clans hang from poles.",
                    "The smell of blood and sweat is overwhelming."
                },
                EnemyCampType.UndeadGraveyard => new[] {
                    "Tombstones and graves surround the area.",
                    "An unholy presence still lingers in the air.",
                    "Bones and ancient artifacts are everywhere."
                },
                EnemyCampType.BeastDen => new[] {
                    "Animal bones and carcasses mark feeding grounds.",
                    "Deep claw marks scar the rocks and trees.",
                    "The den still smells of wild animals."
                },
                EnemyCampType.CultistShrine => new[] {
                    "Dark symbols are painted on every surface.",
                    "Ritual circles and candles form strange patterns.",
                    "You feel uneasy even after the cultists are gone."
                },
                EnemyCampType.DragonLair => new[] {
                    "Scorched earth and melted rock surround the lair.",
                    "Dragon scales glitter in the light.",
                    "The cave still radiates residual heat."
                },
                EnemyCampType.DemonPortal => new[] {
                    "Reality seems warped near the portal location.",
                    "Sulfur and brimstone fill the air.",
                    "Dark energy crackles around the area."
                },
                EnemyCampType.ElementalNexus => new[] {
                    "Elemental energies swirl unpredictably.",
                    "The ground shifts between ice, fire, and stone.",
                    "Magic pulses from the nexus point."
                },
                EnemyCampType.SpiderNest => new[] {
                    "Thick webs cover everything like curtains.",
                    "Egg sacs hang from the ceiling (thankfully empty).",
                    "The walls are lined with silk and chitin."
                },
                _ => new[] { "The area is now safe for travelers." }
            };

            Console.WriteLine(observations[_rng.Next(observations.Length)]);
            Console.ReadKey();
        }

        public override void DisplayInfo()
        {
            string icon = IsCleared ? "✅" : "⚔️";
            Console.WriteLine($"\n{icon} {Name}");
            Console.WriteLine($"   Type: Enemy Camp - {GetCampTypeDescription()}");
            Console.WriteLine($"   Difficulty: {GetDifficultyDescription()}");
            if (RequiredLevel > 1)
            {
                Console.WriteLine($"   Required Level: {RequiredLevel}");
            }
            Console.WriteLine($"   Status: {(IsCleared ? "✅ Cleared" : "⚠️ Hostile")}");
            if (!IsCleared)
            {
                Console.WriteLine($"   Rewards: {GoldReward} gold, {ExperienceReward} XP + Loot");
            }
        }

        private string GetCampTypeDescription()
        {
            return CampType switch
            {
                EnemyCampType.BanditHideout => "Bandit Hideout",
                EnemyCampType.GoblinWarcamp => "Goblin War Camp",
                EnemyCampType.OrcStronghold => "Orc Stronghold",
                EnemyCampType.UndeadGraveyard => "Undead Graveyard",
                EnemyCampType.BeastDen => "Beast Den",
                EnemyCampType.CultistShrine => "Cultist Shrine",
                EnemyCampType.DragonLair => "Dragon Lair",
                EnemyCampType.DemonPortal => "Demon Portal",
                EnemyCampType.ElementalNexus => "Elemental Nexus",
                EnemyCampType.SpiderNest => "Spider Nest",
                _ => "Enemy Camp"
            };
        }

        private string GetDifficultyDescription()
        {
            return Difficulty switch
            {
                EnemyCampDifficulty.Weak => "🟢 Weak (Lv 1-5)",
                EnemyCampDifficulty.Normal => "🟡 Normal (Lv 5-10)",
                EnemyCampDifficulty.Strong => "🟠 Strong (Lv 10-15)",
                EnemyCampDifficulty.Dangerous => "🔴 Dangerous (Lv 15-20)",
                EnemyCampDifficulty.Deadly => "🟣 Deadly (Lv 20-25)",
                EnemyCampDifficulty.Elite => "⚫ Elite (Lv 25+)",
                _ => "Unknown"
            };
        }

        #endregion
    }

    #endregion
}
