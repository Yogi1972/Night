using Night.Systems;
using Night.Characters;
using Rpg_Dungeon.Systems;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;

namespace Rpg_Dungeon
{
    internal static class TitleScreenManager
    {
        public static void Show()
        {
            try
            {
                Console.Clear();
            }
            catch
            {
                Console.WriteLine("\n\n\n");
            }

            // Opening sequence moved to occur after map & character creation in GameInitializer

            var originalColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("╔══════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                                                                  ║");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("║            ⚔️  RPG DUNGEON CRAWLER  ⚔️                          ║");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("║                                                                  ║");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("║              ~ Epic Adventures Await ~                           ║");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("║                                                                  ║");
            Console.WriteLine("╚══════════════════════════════════════════════════════════════════╝");
            Console.ForegroundColor = originalColor;
            Console.WriteLine();
            Console.WriteLine("  Venture into treacherous dungeons, battle fierce monsters,");
            Console.WriteLine("  complete epic quests, and build legendary heroes!");
            Console.WriteLine();
            Console.WriteLine("  🗡️  Choose from 4 unique classes: Warrior, Mage, Rogue, Priest");
            Console.WriteLine("  🏰 Explore dungeons, towns, and a vast world map");
            Console.WriteLine("  🎒 Collect loot, upgrade equipment, and master skills");
            Console.WriteLine("  👥 Play solo or with friends in local multiplayer");
            Console.WriteLine();

            ShowMainMenu();
        }

        private static void ShowMainMenu()
        {
            while (true)
            {
                Console.WriteLine("══════════════════════════════════════════════════════════════════");
                Console.WriteLine("                          MAIN MENU");
                Console.WriteLine("══════════════════════════════════════════════════════════════════");
                Console.WriteLine();
                Console.WriteLine("  1) ⚔️  Start New Game");
                Console.WriteLine("  2) 💾 Load Saved Game (Max 10 saves)");
                Console.WriteLine("  3) 👥 Local Multiplayer (Hotseat)");
                Console.WriteLine("  4) 🌐 Network Multiplayer (LAN)");
                Console.WriteLine("  5) 📂 Load Multiplayer Save (Max 10 saves)");
                Console.WriteLine("  6) 📖 How to Play");
                Console.WriteLine("  7) ℹ️  About");
                Console.WriteLine("  8) 📋 Error Logs & Diagnostics");
                Console.WriteLine("  9) 🔄 Check for Updates");
                Console.WriteLine(" 10) 🔁 Demo: Party Trade (reservation)");
                Console.WriteLine("  0) 🚪 Exit Game");
                Console.WriteLine();
                Console.Write("Choose an option: ");

                var choice = Console.ReadLine() ?? string.Empty;

                switch (choice.Trim())
                {
                    case "1":
                        GameInitializer.StartNewGame();
                        return;

                    case "2":
                        HandleLoadGame();
                        break;

                    case "3":
                        GameInitializer.StartMultiplayerGame();
                        return;

                    case "4":
                        ShowNetworkMultiplayerMenu();
                        break;

                    case "5":
                        HandleLoadMultiplayerGame();
                        break;

                    case "6":
                        ShowHowToPlay();
                        break;

                    case "7":
                        ShowAbout();
                        break;

                    case "8":
                        ShowErrorLogsMenu();
                        break;

                    case "9":
                        UpdateChecker.ShowUpdateCheckScreen();
                        break;

                    case "10":
                        DemoTradeBetweenParty();
                        break;
                        break;

                    case "0":
                        Console.WriteLine("\n🚪 Exiting game...");
                        Console.WriteLine("Thanks for playing! Farewell, adventurer! 👋");
                        Thread.Sleep(1000);
                        Environment.Exit(0);
                        break;

                    default:
                        Console.WriteLine("\n❌ Invalid selection. Please choose a valid option.");
                        Thread.Sleep(1000);
                        break;
                }

                if (choice.Trim() != "0" && choice.Trim() != "1" && choice.Trim() != "2" &&
                    choice.Trim() != "3" && choice.Trim() != "4" && choice.Trim() != "5")
                {
                    Console.WriteLine();
                }
            }
        }

        // Simple demo that creates two characters and runs a reservation -> lease timer -> commit flow
        private static void DemoTradeBetweenParty()
        {
            Console.WriteLine("\n=== DEMO: PARTY TRADE RESERVATION ===");
            var a = new Warrior("Alice");
            var b = new Mage("Bob");
            // give Alice an item
            a.Inventory.AddItem(new GenericItem("Test Sword", 10));
            a.Inventory.AddItem(new GenericItem("Test Shield", 8));
            a.Inventory.AddGold(20);
            b.Inventory.AddGold(5);

            var party = new System.Collections.Generic.List<Night.Characters.Character> { a, b };

            Console.WriteLine($"{a.Name} inventory slots: {a.Inventory.Slots.Count}");
            Console.WriteLine("Select slot to reserve from Alice (1-based):");
            for (int i = 0; i < a.Inventory.Slots.Count; i++) Console.WriteLine($"{i+1}) {(a.Inventory.Slots[i]==null?"(empty)":a.Inventory.Slots[i]!.Name)}");
            var sel = Console.ReadLine() ?? string.Empty;
            if (!int.TryParse(sel, out var idx) || idx < 1 || idx > a.Inventory.Slots.Count) { Console.WriteLine("Invalid."); return; }
            var slot = idx - 1;

            Console.WriteLine("Reserving item...");
            var res = Systems.TradeManager.ReserveItem(a, slot, TimeSpan.FromSeconds(20));
            if (res == null)
            {
                Console.WriteLine("Failed to reserve item.");
                return;
            }

            Console.WriteLine($"Reservation created: {res.Value}. Lease: 20s");

            // show lease countdown
            for (int t = 20; t >= 0; t--)
            {
                Console.Write($"Lease remaining: {t}s   \r");
                System.Threading.Thread.Sleep(1000);
                // if reservation no longer exists, break
                // no public API to check reservation id in TradeManager; check slot reserved on inventory
                if (!a.Inventory.IsSlotReserved(slot))
                {
                    Console.WriteLine("\nReservation released or consumed.");
                    return;
                }
            }

            Console.WriteLine("\nLease expired (or about to). Attempting commit to Bob...");
            var ok = Systems.TradeManager.CommitTrade(new[] { res.Value }, b);
            if (ok) Console.WriteLine("Trade committed: item transferred to Bob."); else Console.WriteLine("Trade commit failed.");
        }

        private static void HandleLoadGame()
        {
            Console.WriteLine("\n╔════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                    LOAD SAVED GAME                             ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════════╝");
            Console.WriteLine();

            var saveFiles = Directory.GetFiles(Directory.GetCurrentDirectory(), "save_*.json");
            if (saveFiles.Length == 0)
            {
                Console.WriteLine("❌ No saved games found!");
                Console.WriteLine("\n💡 Tip: You can save your game by camping (option 3 in main menu)");
                Console.WriteLine("\nPress Enter to return to main menu...");
                Console.ReadLine();
                return;
            }

            Console.WriteLine($"📁 Found {saveFiles.Length} saved game(s):\n");
            for (int i = 0; i < saveFiles.Length; i++)
            {
                var fileName = Path.GetFileName(saveFiles[i]);
                var fileInfo = new FileInfo(saveFiles[i]);
                var fileDate = fileInfo.LastWriteTime;

                string displayName = fileName;
                string partyInfo = "";
                try
                {
                    var json = File.ReadAllText(saveFiles[i], Encoding.UTF8);
                    var opts = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var save = JsonSerializer.Deserialize<Options.SaveFile>(json, opts);
                    if (save != null)
                    {
                        if (!string.IsNullOrWhiteSpace(save.SaveName))
                        {
                            displayName = save.SaveName;
                        }
                        if (save.Party != null)
                        {
                            var partySize = save.Party.Count;
                            var avgLevel = save.Party.Count > 0 ? (int)save.Party.Average(p => p.Level) : 0;
                            partyInfo = $" - {partySize} heroes, Avg Lv {avgLevel}";
                        }
                    }
                }
                catch
                {
                    partyInfo = "";
                }

                Console.WriteLine($"  {i + 1}) 💾 {displayName}");
                Console.WriteLine($"      📅 {fileDate:yyyy-MM-dd HH:mm:ss}{partyInfo}");
                Console.WriteLine();
            }

            Console.WriteLine("0) Cancel and return to menu");
            Console.Write($"\nChoose save file to load (1-{saveFiles.Length}): ");

            var loadChoice = Console.ReadLine() ?? string.Empty;
            if (loadChoice.Trim() == "0")
            {
                Console.WriteLine("Cancelled.");
                Thread.Sleep(500);
                return;
            }

            if (int.TryParse(loadChoice, out int loadIdx) && loadIdx >= 1 && loadIdx <= saveFiles.Length)
            {
                var loaded = SaveGameManager.LoadSpecificSaveFile(saveFiles[loadIdx - 1]);
                if (loaded != null)
                {
                    Console.WriteLine("\n✓ Game loaded successfully!");
                    Thread.Sleep(1000);
                    GameLoopManager.Run(loaded, false);

                    // After game loop ends, clear console before returning to menu
                    try { Console.Clear(); } catch { Console.WriteLine("\n\n"); }
                }
                else
                {
                    Console.WriteLine("\n❌ Failed to load save file.");
                    Thread.Sleep(1500);
                }
            }
            else
            {
                Console.WriteLine("\n❌ Invalid selection.");
                Thread.Sleep(1000);
            }
        }

        private static void HandleLoadMultiplayerGame()
        {
            Console.WriteLine("\n╔════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                 LOAD MULTIPLAYER SAVED GAME                    ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════════╝");
            Console.WriteLine();

            var mpSaveFiles = Directory.GetFiles(Directory.GetCurrentDirectory(), "mpsave_*.json");
            if (mpSaveFiles.Length == 0)
            {
                Console.WriteLine("❌ No multiplayer saved games found!");
                Console.WriteLine("\n💡 Tip: You can save your game by camping (option 3 in main menu)");
                Console.WriteLine("\nPress Enter to return to main menu...");
                Console.ReadLine();
                return;
            }

            Console.WriteLine($"📁 Found {mpSaveFiles.Length} multiplayer saved game(s):\n");
            for (int i = 0; i < mpSaveFiles.Length; i++)
            {
                var fileName = Path.GetFileName(mpSaveFiles[i]);
                var fileInfo = new FileInfo(mpSaveFiles[i]);
                var fileDate = fileInfo.LastWriteTime;

                string displayName = fileName;
                string partyInfo = "";
                try
                {
                    var json = File.ReadAllText(mpSaveFiles[i], Encoding.UTF8);
                    var opts = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var save = JsonSerializer.Deserialize<Options.SaveFile>(json, opts);
                    if (save != null)
                    {
                        if (!string.IsNullOrWhiteSpace(save.SaveName))
                        {
                            displayName = save.SaveName;
                        }
                        if (save.Party != null)
                        {
                            var partySize = save.Party.Count;
                            var avgLevel = save.Party.Count > 0 ? (int)save.Party.Average(p => p.Level) : 0;
                            partyInfo = $" - {partySize} heroes, Avg Lv {avgLevel}";
                        }
                    }
                }
                catch
                {
                    partyInfo = "";
                }

                Console.WriteLine($"  {i + 1}) 🎮 {displayName}");
                Console.WriteLine($"      📅 {fileDate:yyyy-MM-dd HH:mm:ss}{partyInfo}");
                Console.WriteLine();
            }

            Console.WriteLine("0) Cancel and return to menu");
            Console.Write($"\nChoose save file to load (1-{mpSaveFiles.Length}): ");

            var mpLoadChoice = Console.ReadLine() ?? string.Empty;
            if (mpLoadChoice.Trim() == "0")
            {
                Console.WriteLine("Cancelled.");
                Thread.Sleep(500);
                return;
            }

            if (int.TryParse(mpLoadChoice, out int mpLoadIdx) && mpLoadIdx >= 1 && mpLoadIdx <= mpSaveFiles.Length)
            {
                var loadedMp = SaveGameManager.LoadSpecificSaveFile(mpSaveFiles[mpLoadIdx - 1]);
                if (loadedMp != null)
                {
                    Console.WriteLine("\n✓ Multiplayer save loaded successfully!");
                    Console.WriteLine("💡 Tip: Use option 7 in the main menu to configure multiplayer settings!");
                    Thread.Sleep(2000);
                    GameLoopManager.Run(loadedMp, true);

                    // After game loop ends, clear console before returning to menu
                    try { Console.Clear(); } catch { Console.WriteLine("\n\n"); }
                }
                else
                {
                    Console.WriteLine("\n❌ Failed to load save file.");
                    Thread.Sleep(1500);
                }
            }
            else
            {
                Console.WriteLine("\n❌ Invalid selection.");
                Thread.Sleep(1000);
            }
        }

        private static void ShowHowToPlay()
        {
            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                         HOW TO PLAY                              ║");
            Console.WriteLine("╚══════════════════════════════════════════════════════════════════╝");
            Console.WriteLine();
            Console.WriteLine("📜 GAME BASICS:");
            Console.WriteLine("  • Create a party of 1-4 heroes with unique classes and races");
            Console.WriteLine("  • Explore dungeons, complete quests, and hunt bounties");
            Console.WriteLine("  • Level up, unlock skills, and upgrade equipment");
            Console.WriteLine("  • Manage inventory, gold, and resources");
            Console.WriteLine();
            Console.WriteLine("⚔️  COMBAT:");
            Console.WriteLine("  • Turn-based combat using d20 rolls (like D&D)");
            Console.WriteLine("  • Natural 20 = Critical Hit! Natural 1 = Critical Miss!");
            Console.WriteLine("  • Use attacks, special abilities, or items each turn");
            Console.WriteLine("  • Defeat enemies to earn XP, gold, and loot");
            Console.WriteLine();
            Console.WriteLine("🎒 EQUIPMENT:");
            Console.WriteLine("  • Equip weapons, armor, accessories, necklaces, and rings");
            Console.WriteLine("  • Use off-hand slot for torches to illuminate dark places");
            Console.WriteLine("  • Equipment has durability - repair at blacksmith");
            Console.WriteLine("  • Torches burn over time and provide light in dungeons");
            Console.WriteLine("  • Items provide stat bonuses (STR, AGI, INT, HP, Mana)");
            Console.WriteLine("  • Expand inventory with backpacks");
            Console.WriteLine();
            Console.WriteLine("🌟 PROGRESSION:");
            Console.WriteLine("  • Gain XP from combat to level up");
            Console.WriteLine("  • Unlock skill trees with powerful passive abilities");
            Console.WriteLine("  • Complete quests and bounties for rewards");
            Console.WriteLine("  • Earn achievements for special accomplishments");
            Console.WriteLine();
            Console.WriteLine("🏙️  TOWN FEATURES:");
            Console.WriteLine("  • Visit merchants to buy/sell items");
            Console.WriteLine("  • Repair equipment at the blacksmith");
            Console.WriteLine("  • Accept quests and bounties");
            Console.WriteLine("  • Store gold at the bank");
            Console.WriteLine("  • Craft items and brew potions");
            Console.WriteLine("  • Adopt pets for combat bonuses");
            Console.WriteLine();
            Console.WriteLine("🌍 WORLD EXPLORATION:");
            Console.WriteLine("  • Explore multiple areas with unique dungeons");
            Console.WriteLine("  • Weather and time of day affect gameplay");
            Console.WriteLine("  • Camp to rest, heal, and save your progress");
            Console.WriteLine("  • Random events add surprises to your journey");
            Console.WriteLine();
            Console.WriteLine("💡 TIPS:");
            Console.WriteLine("  • Save often at camp (option 3 in main menu)");
            Console.WriteLine("  • Give your saves custom names for easy identification");
            Console.WriteLine("  • Game auto-saves when you exit (keeps up to 10 saves per mode)");
            Console.WriteLine("  • Balance your party with different classes");
            Console.WriteLine("  • Keep equipment repaired for maximum effectiveness");
            Console.WriteLine("  • Craft torches at camp and light them before exploring");
            Console.WriteLine("  • Complete easy quests first to build strength");
            Console.WriteLine("  • Use the journal to track active quests and bounties");
            Console.WriteLine();
            Console.WriteLine("Press Enter to return to main menu...");
            Console.ReadLine();
        }

        private static void ShowAbout()
        {
            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                            ABOUT                                 ║");
            Console.WriteLine("╚══════════════════════════════════════════════════════════════════╝");
            Console.WriteLine();
            Console.WriteLine("  🎮 RPG Dungeon Crawler");
            Console.WriteLine($"  📅 Version: {VersionControl.DisplayVersion}");
            Console.WriteLine($"  🔧 Built with: {VersionControl.DotNetVersion} & {VersionControl.CSharpVersion}");
            Console.WriteLine();
            Console.WriteLine("  A classic dungeon-crawling RPG featuring:");
            Console.WriteLine();
            Console.WriteLine("    ⚔️  Turn-based tactical combat");
            Console.WriteLine("    🎲 D20 dice roll mechanics");
            Console.WriteLine("    🏰 Procedurally generated dungeons");
            Console.WriteLine("    📜 Dynamic quest system");
            Console.WriteLine("    🎯 Bounty hunting");
            Console.WriteLine("    🌟 Skill trees and progression");
            Console.WriteLine("    🐾 Pet companions");
            Console.WriteLine("    🌦️  Weather and time systems");
            Console.WriteLine("    💎 Crafting and trading");
            Console.WriteLine("    👥 Local multiplayer support");
            Console.WriteLine();
            Console.WriteLine("  Thank you for playing!");
            Console.WriteLine();
            Console.WriteLine("Press Enter to return to main menu...");
            Console.ReadLine();
        }

        private static void ShowErrorLogsMenu()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("╔══════════════════════════════════════════════════════════════════╗");
                Console.WriteLine("║                   ERROR LOGS & DIAGNOSTICS                       ║");
                Console.WriteLine("╚══════════════════════════════════════════════════════════════════╝");
                Console.WriteLine();
                Console.WriteLine("  1) 📊 View Error Log Statistics");
                Console.WriteLine("  2) 📁 Open Error Logs Folder");
                Console.WriteLine("  3) 📧 Create Error Report (to send to developer)");
                Console.WriteLine("  4) 🗑️  Clean Old Logs (30+ days)");
                Console.WriteLine("  5) 🧪 Run Diagnostic Tests");
                Console.WriteLine("  6) ✉️  Send Error Report via Email");
                Console.WriteLine("  7) 📤 Create & Send Test Report to Developer");
                Console.WriteLine("  8) ℹ️  Email Instructions");
                Console.WriteLine("  9) ✅ Verify Error System (Run Tests)");
                Console.WriteLine("  0) ⬅️  Return to Main Menu");
                Console.WriteLine();
                Console.Write("Choose an option: ");

                var choice = Console.ReadLine() ?? string.Empty;

                switch (choice.Trim())
                {
                    case "1":
                        Console.Clear();
                        ErrorLogger.ShowLogStatistics();
                        Console.WriteLine("\nPress Enter to continue...");
                        Console.ReadLine();
                        break;

                    case "2":
                        Console.WriteLine("\n📂 Opening error logs folder...");
                        ErrorLogger.OpenLogDirectory();
                        Thread.Sleep(1500);
                        break;

                    case "3":
                        Console.WriteLine("\n📧 Creating comprehensive error report...");
                        var reportPath = ErrorLogger.CreateErrorReport();
                        if (!string.IsNullOrEmpty(reportPath))
                        {
                            Console.WriteLine($"\n✅ Error report created successfully!");
                            Console.WriteLine($"\n📄 Report saved to:");
                            Console.WriteLine($"   {reportPath}");
                            Console.WriteLine();
                            Console.WriteLine("📧 TO SEND THIS REPORT:");
                            Console.WriteLine("   • Use option 6 to send via email automatically");
                            Console.WriteLine("   • Or manually attach this file to an email");
                            Console.WriteLine();
                            Console.WriteLine("💡 Press 'E' to send via email now, 'O' to open folder, or Enter to continue...");

                            var key = Console.ReadKey(true);
                            if (key.KeyChar == 'E' || key.KeyChar == 'e')
                            {
                                Console.WriteLine();
                                EmailReporter.SendErrorReportViaEmailClient(reportPath);
                                Console.WriteLine("\nPress Enter to continue...");
                                Console.ReadLine();
                            }
                            else if (key.KeyChar == 'O' || key.KeyChar == 'o')
                            {
                                ErrorLogger.OpenLogDirectory();
                                Console.WriteLine("\nOpening folder...");
                                Thread.Sleep(1500);
                            }
                        }
                        else
                        {
                            Console.WriteLine("\n❌ Failed to create error report.");
                            Thread.Sleep(1500);
                        }
                        break;

                    case "4":
                        Console.WriteLine("\n🗑️  Cleaning old error logs (30+ days old)...");
                        ErrorLogger.CleanOldLogs(30);
                        Thread.Sleep(1500);
                        break;

                    case "5":
                        Console.WriteLine("\n🧪 Running diagnostic tests...");
                        Console.WriteLine("   (This will test system features)");
                        Thread.Sleep(1000);
                        DiagnosticTest.TestMain();
                        break;

                    case "6":
                        EmailReporter.CreateAndPrepareEmailReport();
                        Console.WriteLine("\nPress Enter to continue...");
                        Console.ReadLine();
                        break;

                    case "7":
                        Console.WriteLine("\n📤 SEND TEST REPORT TO DEVELOPER");
                        Console.WriteLine("═══════════════════════════════════════════════════════════════════");
                        Console.WriteLine();
                        Console.WriteLine("⚠️  This will create a test error log and prepare it for sending.");
                        Console.WriteLine($"   Developer email: {EmailReporter.GetMaskedEmail()}");
                        Console.WriteLine();
                        Console.WriteLine("   This is useful for:");
                        Console.WriteLine("   • Testing the error reporting system");
                        Console.WriteLine("   • Verifying email functionality");
                        Console.WriteLine("   • Providing system info to developer");
                        Console.WriteLine();
                        Console.Write("Continue? (Y/N): ");

                        var confirm = Console.ReadKey(true);
                        Console.WriteLine();

                        if (confirm.KeyChar == 'Y' || confirm.KeyChar == 'y')
                        {
                            EmailReporter.SendTestErrorReport();
                            Console.WriteLine("\nPress Enter to continue...");
                            Console.ReadLine();
                        }
                        else
                        {
                            Console.WriteLine("Cancelled.");
                            Thread.Sleep(1000);
                        }
                        break;

                    case "8":
                        EmailReporter.ShowEmailInstructions();
                        Console.WriteLine("Press Enter to continue...");
                        Console.ReadLine();
                        break;

                    case "9":
                        ErrorSystemTest.RunTest();
                        break;

                    case "0":
                        return;

                    default:
                        Console.WriteLine("\n❌ Invalid selection. Please choose a valid option.");
                        Thread.Sleep(1000);
                        break;
                }
            }
        }

        private static void ShowNetworkMultiplayerMenu()
        {
            // Load current game party (if available) for multiplayer
            var party = new List<Night.Characters.Character>();

            // Try to load last saved game for multiplayer
            Console.WriteLine("\n💡 Loading character data for multiplayer...");

            // Use the comprehensive new multiplayer menu
            MultiplayerMenu.Show(party.Count > 0 ? party : GameInitializer.CreateDefaultParty());
        }

        private static void ShowIPAddress()
        {
            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                      YOUR IP ADDRESS                             ║");
            Console.WriteLine("╚══════════════════════════════════════════════════════════════════╝");
            Console.WriteLine();

            string ipAddress = Rpg_Dungeon.Systems.NetworkManager.GetLocalIPAddress();

            Console.WriteLine($"  📡 Your Local IP Address: {ipAddress}");
            Console.WriteLine();
            Console.WriteLine("  💡 Share this IP with friends so they can join your game!");
            Console.WriteLine();
            Console.WriteLine("  ⚠️  NOTE: This only works on the same local network (LAN)");
            Console.WriteLine("     - Same Wi-Fi network");
            Console.WriteLine("     - Same wired network");
            Console.WriteLine("     - Connected via router");
            Console.WriteLine();
            Console.WriteLine("Press Enter to return...");
            Console.ReadLine();
        }

        private static void ShowNetworkHelp()
        {
            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║               NETWORK MULTIPLAYER HELP                           ║");
            Console.WriteLine("╚══════════════════════════════════════════════════════════════════╝");
            Console.WriteLine();
            Console.WriteLine("  🎮 HOW TO PLAY NETWORK MULTIPLAYER:");
            Console.WriteLine();
            Console.WriteLine("  1️⃣  HOST SETUP:");
            Console.WriteLine("     • Choose 'Host a Game' from the menu");
            Console.WriteLine("     • Create your character");
            Console.WriteLine("     • Share your IP address with friends (displayed on screen)");
            Console.WriteLine("     • Wait for players to join");
            Console.WriteLine("     • Type 'start' when ready to begin");
            Console.WriteLine();
            Console.WriteLine("  2️⃣  JOIN SETUP:");
            Console.WriteLine("     • Choose 'Join a Game' from the menu");
            Console.WriteLine("     • Enter the host's IP address");
            Console.WriteLine("     • Create your character");
            Console.WriteLine("     • Wait for host to start the game");
            Console.WriteLine();
            Console.WriteLine("  ⚠️  REQUIREMENTS:");
            Console.WriteLine("     ✓ All players must be on the same local network (LAN)");
            Console.WriteLine("     ✓ Firewall may need to allow the game (Port 7777)");
            Console.WriteLine("     ✓ Host must know their IP address");
            Console.WriteLine();
            Console.WriteLine("  🔧 TROUBLESHOOTING:");
            Console.WriteLine("     • Can't connect? Check firewall settings");
            Console.WriteLine("     • Wrong IP? Use 'Show My IP Address' option");
            Console.WriteLine("     • Still issues? Try using IP address like 192.168.x.x");
            Console.WriteLine();
            Console.WriteLine("  📝 DEFAULT PORT: 7777");
            Console.WriteLine("     (You can change this when hosting)");
            Console.WriteLine();
            Console.WriteLine("Press Enter to return...");
            Console.ReadLine();
        }
    }
}
