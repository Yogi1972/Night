using Night.Characters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rpg_Dungeon
{
    internal static class GameLoopManager
    {
        private static bool _isMultiplayerMode = false;
        private static int? _currentWorldSeed = null;

        public static bool IsMultiplayerMode => _isMultiplayerMode;
        public static int? CurrentWorldSeed => _currentWorldSeed;

        public static void Run(List<Character> party, bool isMultiplayer = false, int? worldSeed = null)
        {
            _isMultiplayerMode = isMultiplayer;
            _currentWorldSeed = worldSeed;
            var town = new Town();
            var multiplayer = new Multiplayer();
            var trading = new Trading();
            var questBoard = new QuestBoard();
            var bountyBoard = new BountyBoard();
            var achievementTracker = new AchievementTracker();
            var weather = new Weather();
            var timeTracker = new TimeOfDay();
            var journal = new Journal();
            var worldMap = new Map(weather, timeTracker, worldSeed);
            var npcManager = new NPCManager();
            var mainStoryline = new MainStoryline();
            var fogOfWarMap = new FogOfWarMap();

            weather.SetTimeTracker(timeTracker);

            town.SetTrackers(questBoard, bountyBoard, achievementTracker);
            town.SetJournal(journal);
            town.SetWeather(weather);
            town.SetTimeTracker(timeTracker);

            if (worldSeed.HasValue)
            {
                Console.WriteLine($"\n🌍 World Seed: {WorldGenerator.SeedToString(worldSeed.Value)}");
                System.Threading.Thread.Sleep(1500);
            }

            // Show story introduction
            if (!mainStoryline.HasSeenIntro())
            {
                mainStoryline.ShowIntroduction();
            }

            while (true)
            {
                weather.UpdateWeather();

                // Display current story objective
                mainStoryline.DisplayCurrentObjective();

                DisplayMainMenu(multiplayer);

                var choice = Console.ReadLine() ?? string.Empty;

                if (choice.Trim() == "1")
                {
                    HandleViewParty(party, multiplayer);
                }
                else if (choice.Trim() == "2")
                {
                    HandleViewInventory(party, multiplayer);
                }
                else if (choice.Trim() == "3")
                {
                    var result = Camping.CampMenu(party, weather, timeTracker);
                    if (result != null)
                    {
                        Run(result);
                        return;
                    }
                    Options.MarkCamped();
                }
                else if (choice.Trim() == "4")
                {
                    var gameState = new GameState
                    {
                        NPCManager = npcManager,
                        Journal = journal,
                        MainStoryline = mainStoryline,
                        FogOfWarMap = fogOfWarMap,
                        Weather = weather,
                        TimeTracker = timeTracker
                    };
                    town.EnterTown(party, gameState);
                }
                else if (choice.Trim() == "5")
                {
                    var merchant = new MerchantShop();
                    merchant.OpenShop(party);
                }
                else if (choice.Trim() == "6")
                {
                    worldMap.OpenMap(party, questBoard, bountyBoard, achievementTracker, journal, mainStoryline, npcManager, fogOfWarMap);
                }
                else if (choice.Trim() == "7")
                {
                    multiplayer.OpenMultiplayerMenu(party);
                }
                else if (choice.Trim() == "8" && multiplayer.IsSessionActive())
                {
                    multiplayer.QuickHealthCheck(party);
                }
                else if (choice.Trim() == "9")
                {
                    Console.WriteLine($"\nCurrent Weather: {weather.GetWeatherDescription()}");
                    Console.WriteLine("\nThe weather affects travel time and can be hazardous on long journeys.");
                }
                else if (choice.Trim() == "10")
                {
                    Console.WriteLine($"\nCurrent Time: {timeTracker.GetTimeDescription()}");
                    Console.WriteLine(timeTracker.GetAtmosphericDescription());
                    var suggestion = timeTracker.GetTimeSuggestion();
                    if (!string.IsNullOrEmpty(suggestion))
                    {
                        Console.WriteLine(suggestion);
                    }
                }
                else if (choice.Trim() == "11")
                {
                    HandleSkillTree(party, multiplayer);
                }
                else if (choice.Trim() == "12")
                {
                    journal.OpenJournal(party);
                }
                else if (choice.Trim() == "13")
                {
                    trading.OpenTradeMenu(party);
                }
                else if (choice.Trim() == "14")
                {
                    TestEncounterMenu(party);
                }
                else if (choice.Trim() == "15")
                {
                    fogOfWarMap.DisplayMap(party, mainStoryline);
                }
                else if (choice.Trim() == "16")
                {
                    mainStoryline.DisplayStoryJournal();
                }
                else if (choice.Trim() == "17")
                {
                    DisplayWorldInfo();
                }
                else if (choice.Trim() == "18")
                {
                    PvPArena.OpenArena(party);
                }
                else if (choice.Trim() == "19")
                {
                    SecretDiscovery.ShowDiscoveredSecrets();
                    Console.WriteLine("\nPress any key to continue...");
                    Console.ReadKey(true);
                }
                else if (choice.Trim() == "20")
                {
                    // Secret code entry
                    Console.Write("\nEnter code: ");
                    var code = Console.ReadLine() ?? string.Empty;
                    if (!SecretDiscovery.CheckSecretCode(code, party))
                    {
                        VisualEffects.WriteInfo("Invalid code.\n");
                    }
                    System.Threading.Thread.Sleep(1000);
                }
                else if (choice.Trim() == "21")
                {
                    AchievementSummary.ShowHeroicSummary(party);
                }
                else if (choice.Trim() == "0")
                {
                    Console.Write("Are you sure you want to quit the game? (y/n): ");
                    var ans = Console.ReadLine() ?? string.Empty;
                    if (ans.Trim().Equals("y", StringComparison.OrdinalIgnoreCase))
                    {
                        Console.WriteLine("\n💾 Auto-saving your progress...");
                        Options.SaveGameAutomatic(party, _isMultiplayerMode);
                        Console.WriteLine("✅ Progress saved! Thanks for playing! 👋");
                        System.Threading.Thread.Sleep(1500);
                        Environment.Exit(0);
                    }
                }
                else
                {
                    Console.WriteLine("Invalid selection.");
                }
            }
        }

        private static void DisplayMainMenu(Multiplayer multiplayer)
        {
            Console.WriteLine("\n╔═══════════════════════════════════════════════════════════╗");
            VisualEffects.WriteLineColored("║                    MAIN MENU                              ║", ConsoleColor.Cyan);
            Console.WriteLine("╚═══════════════════════════════════════════════════════════╝");
            Console.WriteLine("1) View party");
            Console.WriteLine("2) View inventory / equip items");
            Console.WriteLine("3) Set up camp");
            Console.WriteLine("4) Visit town");
            Console.WriteLine("5) Visit merchant");
            Console.WriteLine("6) World Map (Explore Areas & Dungeons)");
            Console.WriteLine("7) Multiplayer (Local Play)");
            if (multiplayer.IsSessionActive())
            {
                Console.WriteLine("8) Quick Health Check (Healer View)");
            }
            Console.WriteLine("9) Check weather");
            Console.WriteLine("10) Check time");
            Console.WriteLine("11) Skill Tree");
            Console.WriteLine("12) Open Journal");
            Console.WriteLine("13) Trade with Party");
            Console.WriteLine("14) Test Encounter System");
            Console.WriteLine("15) View Fog of War Map 🗺️");
            Console.WriteLine("16) View Story Progress 📖");
            Console.WriteLine("17) View World Info 🌍");
            VisualEffects.WriteLineColored("18) ⚔️  PvP Arena (NEW!)", ConsoleColor.Red);
            VisualEffects.WriteLineColored("19) 🔍 Secrets Discovered", ConsoleColor.Magenta);
            VisualEffects.WriteLineColored("20) 🎮 Enter Secret Code", ConsoleColor.DarkMagenta);
            VisualEffects.WriteLineColored("21) 🏆 Achievement Summary", ConsoleColor.Yellow);
            Console.WriteLine("0) Quit");
            Console.Write("Choose: ");
        }

        private static void HandleViewParty(List<Character> party, Multiplayer multiplayer)
        {
            Console.WriteLine("\n╔═══════════════════════════════════════════════════════════╗");
            VisualEffects.WriteLineColored("║                    PARTY STATUS                           ║", ConsoleColor.Cyan);
            Console.WriteLine("╚═══════════════════════════════════════════════════════════╝\n");

            foreach (var p in party)
            {
                string title = Playerleveling.GetLevelTitle(p.Level);
                string playerTag = multiplayer.IsSessionActive() ? $" {multiplayer.GetPlayerTag(p)}" : "";

                VisualEffects.WriteLineColored($"▸ {p.Name}{playerTag} (Lv {p.Level} {title}) {p.GetType().Name}", ConsoleColor.Yellow);

                // Health bar
                Console.Write("  ");
                VisualEffects.DrawProgressBarLine(p.Health, p.GetTotalMaxHP(), 25, "HP");

                // Mana bar (for casters)
                if (p is Mage || p is Priest)
                {
                    Console.Write("  ");
                    VisualEffects.DrawProgressBarLine(p.Mana, p.GetTotalMaxMana(), 25, "MP");
                }

                // Stamina bar (for physical classes)
                if (p is Warrior || p is Rogue)
                {
                    Console.Write("  ");
                    VisualEffects.DrawProgressBarLine(p.Stamina, p.GetTotalMaxStamina(), 25, "SP");
                }

                VisualEffects.WriteInfo($"  💰 Gold: {p.Inventory.Gold}\n");
                Console.WriteLine($"  Stats - Str: {p.GetTotalStrength()} ({p.Strength}+{p.GetTotalStrength() - p.Strength}), Agi: {p.GetTotalAgility()} ({p.Agility}+{p.GetTotalAgility() - p.Agility}), Int: {p.GetTotalIntelligence()} ({p.Intelligence}+{p.GetTotalIntelligence() - p.Intelligence})");

                Console.WriteLine("  Equipment:");
                EquipmentDisplayHelper.DisplayEquippedItem("Weapon", p.Inventory.EquippedWeapon);
                EquipmentDisplayHelper.DisplayEquippedItem("Armor", p.Inventory.EquippedArmor);
                EquipmentDisplayHelper.DisplayEquippedItem("Accessory", p.Inventory.EquippedAccessory);
                EquipmentDisplayHelper.DisplayEquippedItem("Necklace", p.Inventory.EquippedNecklace);
                EquipmentDisplayHelper.DisplayEquippedItem("Ring 1", p.Inventory.EquippedRing1);
                EquipmentDisplayHelper.DisplayEquippedItem("Ring 2", p.Inventory.EquippedRing2);
                EquipmentDisplayHelper.DisplayOffHandItem(p.Inventory.EquippedOffHand);
                Console.WriteLine();
            }

            Console.Write("\nView detailed level progress for a character? (Enter 1-{0} or 0 to skip): ", party.Count);
            var detailChoice = Console.ReadLine() ?? string.Empty;
            if (int.TryParse(detailChoice, out var detailIdx) && detailIdx >= 1 && detailIdx <= party.Count)
            {
                Playerleveling.DisplayLevelProgress(party[detailIdx - 1]);
            }
        }

        private static void HandleViewInventory(List<Character> party, Multiplayer multiplayer)
        {
            Console.WriteLine("Select party member to view inventory:");
            for (int i = 0; i < party.Count; i++)
            {
                string playerTag = multiplayer.IsSessionActive() ? $" {multiplayer.GetPlayerTag(party[i])}" : "";
                Console.WriteLine($"{i + 1}) {party[i].Name}{playerTag}");
            }
            var s = Console.ReadLine() ?? string.Empty;
            if (!int.TryParse(s, out var idx) || idx < 1 || idx > party.Count)
            {
                Console.WriteLine("Invalid.");
                return;
            }
            var member = party[idx - 1];

            EquipmentDisplayHelper.ShowEquippedItems(member);
            ShowInventorySlots(member);
            HandleInventoryOptions(member);
        }

        private static void ShowInventorySlots(Character member)
        {
            var slots = member.Inventory.Slots;
            Console.WriteLine($"\n=== Inventory === {member.Inventory}");
            for (int i = 0; i < slots.Count; i++)
            {
                var it = slots[i];
                var disp = it == null ? "(empty)" : it.Name;
                if (it is Torch torch)
                {
                    string status = torch.IsLit ? "🔥 Lit" : "Unlit";
                    disp += $" [Torch - {status}, {torch.BurnTimeHours}h fuel]";
                }
                else if (it is Equipment eq)
                {
                    disp += $" [{eq.Type}] (Dur {eq.Durability}/{eq.MaxDurability})";
                }
                if (it is Backpack bp) disp += $" [Backpack +{bp.Slots} slots]";
                Console.WriteLine($"{i + 1}) {disp}");
            }
        }

        private static void HandleInventoryOptions(Character member)
        {
            Console.WriteLine("\nOptions:");
            Console.WriteLine("E) Equip item from inventory");
            Console.WriteLine("U) Unequip item");
            Console.WriteLine("B) Equip backpack from slot");
            Console.WriteLine("T) Toggle torch (light/extinguish)");
            Console.WriteLine("Enter) Continue");
            Console.Write("Choice: ");
            var opt = Console.ReadLine() ?? string.Empty;

            if (opt.Trim().StartsWith("E", StringComparison.OrdinalIgnoreCase))
            {
                HandleEquipItem(member);
            }
            else if (opt.Trim().StartsWith("U", StringComparison.OrdinalIgnoreCase))
            {
                HandleUnequipItem(member);
            }
            else if (opt.Trim().StartsWith("B", StringComparison.OrdinalIgnoreCase))
            {
                HandleEquipBackpack(member);
            }
        }

        private static void HandleEquipItem(Character member)
        {
            Console.WriteLine("Enter slot number of equipment to equip:");
            var si = Console.ReadLine() ?? string.Empty;
            var slots = member.Inventory.Slots;

            if (!int.TryParse(si, out var sidx) || sidx < 1 || sidx > slots.Count)
            {
                Console.WriteLine("Invalid slot.");
                return;
            }

            var it = slots[sidx - 1];
            if (it is Equipment equipment)
            {
                EquipmentSlot targetSlot = equipment.Type switch
                {
                    EquipmentType.Weapon => EquipmentSlot.Weapon,
                    EquipmentType.Armor => EquipmentSlot.Armor,
                    EquipmentType.Accessory => EquipmentSlot.Accessory,
                    EquipmentType.Necklace => EquipmentSlot.Necklace,
                    EquipmentType.Ring => EquipmentSlot.Ring1,
                    EquipmentType.OffHand => EquipmentSlot.OffHand,
                    _ => EquipmentSlot.Weapon
                };

                if (equipment.Type == EquipmentType.Ring)
                {
                    Console.WriteLine("Which ring slot?");
                    Console.WriteLine("1) Ring 1");
                    Console.WriteLine("2) Ring 2");
                    var ringChoice = Console.ReadLine() ?? string.Empty;
                    targetSlot = ringChoice.Trim() == "2" ? EquipmentSlot.Ring2 : EquipmentSlot.Ring1;
                }

                member.Inventory.RemoveItem(sidx - 1);

                if (member.Inventory.EquipItem(equipment, targetSlot))
                {
                    Console.WriteLine($"{equipment.Name} equipped to {targetSlot} slot!");
                }
                else
                {
                    member.Inventory.AddItem(equipment);
                    Console.WriteLine("Failed to equip item.");
                }
            }
            else
            {
                Console.WriteLine("Selected item is not equipment.");
            }
        }

        private static void HandleUnequipItem(Character member)
        {
            Console.WriteLine("Unequip which slot?");
            Console.WriteLine("1) Weapon");
            Console.WriteLine("2) Armor");
            Console.WriteLine("3) Accessory");
            Console.WriteLine("4) Necklace");
            Console.WriteLine("5) Ring 1");
            Console.WriteLine("6) Ring 2");
            var us = Console.ReadLine() ?? string.Empty;

            EquipmentSlot? slotToUnequip = us.Trim() switch
            {
                "1" => EquipmentSlot.Weapon,
                "2" => EquipmentSlot.Armor,
                "3" => EquipmentSlot.Accessory,
                "4" => EquipmentSlot.Necklace,
                "5" => EquipmentSlot.Ring1,
                "6" => EquipmentSlot.Ring2,
                _ => null
            };

            if (slotToUnequip.HasValue)
            {
                if (member.Inventory.UnequipItem(slotToUnequip.Value))
                {
                    Console.WriteLine($"{slotToUnequip.Value} slot unequipped.");
                }
                else
                {
                    Console.WriteLine("Nothing equipped in that slot or inventory full.");
                }
            }
        }

        private static void HandleEquipBackpack(Character member)
        {
            Console.WriteLine("Enter slot number of backpack to equip:");
            var si = Console.ReadLine() ?? string.Empty;
            var slots = member.Inventory.Slots;

            if (!int.TryParse(si, out var sidx) || sidx < 1 || sidx > slots.Count)
            {
                Console.WriteLine("Invalid slot.");
                return;
            }

            var it = slots[sidx - 1];
            if (it is Backpack bp)
            {
                if (member.Inventory.EquipBackpack(bp))
                    Console.WriteLine("Backpack equipped.");
                else
                    Console.WriteLine("Failed to equip backpack.");
            }
            else
            {
                Console.WriteLine("Selected item is not a backpack.");
            }
        }

        private static void HandleSkillTree(List<Character> party, Multiplayer multiplayer)
        {
            Console.WriteLine("\nSelect party member to view skill tree:");
            for (int i = 0; i < party.Count; i++)
            {
                string playerTag = multiplayer.IsSessionActive() ? $" {multiplayer.GetPlayerTag(party[i])}" : "";
                var skillPoints = party[i].SkillTree?.SkillPoints ?? 0;
                string pointsInfo = skillPoints > 0 ? $" [⚡ {skillPoints} points available!]" : "";
                Console.WriteLine($"{i + 1}) {party[i].Name}{playerTag} (Lv {party[i].Level} {party[i].GetType().Name}){pointsInfo}");
            }
            var s = Console.ReadLine() ?? string.Empty;
            if (int.TryParse(s, out var idx) && idx >= 1 && idx <= party.Count)
            {
                SkillTreeFactory.ShowSkillTree(party[idx - 1]);
            }
            else
            {
                Console.WriteLine("Invalid selection.");
            }
        }

        private static void TestEncounterMenu(List<Character> party)
        {
            Console.WriteLine("\n=== Test Encounter System ===");
            Console.WriteLine("Choose encounter type:");
            Console.WriteLine("1) Easy Encounter");
            Console.WriteLine("2) Normal Encounter");
            Console.WriteLine("3) Hard Encounter");
            Console.WriteLine("4) Elite Encounter");
            Console.WriteLine("5) Boss Encounter");
            Console.WriteLine("6) Random Encounter");
            Console.WriteLine("0) Cancel");
            Console.Write("Choice: ");

            var choice = Console.ReadLine() ?? string.Empty;

            switch (choice.Trim())
            {
                case "1":
                    Encounter.QuickEncounter(party, EncounterDifficulty.Easy);
                    break;
                case "2":
                    Encounter.QuickEncounter(party, EncounterDifficulty.Normal);
                    break;
                case "3":
                    Encounter.QuickEncounter(party, EncounterDifficulty.Hard);
                    break;
                case "4":
                    Encounter.QuickEncounter(party, EncounterDifficulty.Elite);
                    break;
                case "5":
                    var bossEncounter = new Encounter();
                    bossEncounter.GenerateBossEncounter(party);
                    bossEncounter.StartEncounter(party);
                    break;
                case "6":
                    int avgLevel = party.Count > 0 ? (int)party.Average(c => c.Level) : 1;
                    Encounter.RandomEncounter(party, avgLevel);
                    break;
                case "0":
                    Console.WriteLine("Cancelled.");
                    break;
                default:
                    Console.WriteLine("Invalid choice.");
                    break;
            }

            Console.WriteLine("\nPress Enter to continue...");
            Console.ReadLine();
        }

        private static void DisplayWorldInfo()
        {
            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                         WORLD INFORMATION                        ║");
            Console.WriteLine("╚══════════════════════════════════════════════════════════════════╝");
            Console.WriteLine();

            if (_currentWorldSeed.HasValue)
            {
                Console.WriteLine($"🌍 World Seed: {WorldGenerator.SeedToString(_currentWorldSeed.Value)}");
                Console.WriteLine($"   (Numeric: {_currentWorldSeed.Value})");
                Console.WriteLine();
                Console.WriteLine("💡 About World Seeds:");
                Console.WriteLine("   • Each seed generates a unique world layout");
                Console.WriteLine("   • Same seed = Same towns, dungeons, and camps");
                Console.WriteLine("   • Use this seed to replay the same world");
                Console.WriteLine("   • Share seeds with friends for co-op challenges!");
                Console.WriteLine();
                Console.WriteLine("📝 To use this seed in a new game:");
                Console.WriteLine($"   Enter '{WorldGenerator.SeedToString(_currentWorldSeed.Value)}' when prompted");
            }
            else
            {
                Console.WriteLine("⚠️  No world seed information available.");
                Console.WriteLine("   (This game may have been started before the seed system)");
            }

            Console.WriteLine("\nPress Enter to return to main menu...");
            Console.ReadLine();
        }
    }
}
