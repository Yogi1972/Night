using Night.Characters;
using Rpg_Dungeon.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Rpg_Dungeon
{
    /// <summary>
    /// Comprehensive multiplayer menu with reconnect and save/load support
    /// </summary>
    internal static class MultiplayerMenu
    {
        private static MultiplayerSessionManager? _sessionManager;
        private static string? _lastHostIp;
        private static int _lastPort = NetworkManager.DEFAULT_PORT;

        /// <summary>
        /// Main multiplayer menu
        /// </summary>
        public static void Show(List<Character> party)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("╔══════════════════════════════════════════════════════════════════╗");
                Console.WriteLine("║                    MULTIPLAYER MENU                              ║");
                Console.WriteLine("╚══════════════════════════════════════════════════════════════════╝");
                Console.WriteLine();

                if (_sessionManager != null && _sessionManager.IsActive)
                {
                    Console.WriteLine("🟢 SESSION ACTIVE");
                    _sessionManager.DisplaySessionInfo();
                }
                else
                {
                    Console.WriteLine("🔴 No active session");
                }

                Console.WriteLine("\n━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
                Console.WriteLine("\n--- Session Management ---");
                Console.WriteLine("1) Host New Game (LAN)");
                Console.WriteLine("2) Join Game (LAN)");
                Console.WriteLine("3) Reconnect to Last Session");
                Console.WriteLine("4) End Current Session");
                Console.WriteLine();
                Console.WriteLine("--- Save/Load ---");
                Console.WriteLine("5) Save Multiplayer Session");
                Console.WriteLine("6) Load Multiplayer Session");
                Console.WriteLine("7) List Saved Sessions");
                Console.WriteLine();
                Console.WriteLine("--- Local Multiplayer (Same PC) ---");
                Console.WriteLine("8) Local Multiplayer Menu");
                Console.WriteLine();
                Console.WriteLine("0) Back to Main Menu");
                Console.WriteLine();
                Console.Write("Choose: ");

                var choice = Console.ReadLine()?.Trim();

                switch (choice)
                {
                    case "1":
                        HostNewGame(party);
                        break;
                    case "2":
                        JoinGame(party);
                        break;
                    case "3":
                        ReconnectToSession();
                        break;
                    case "4":
                        EndSession();
                        break;
                    case "5":
                        SaveSession();
                        break;
                    case "6":
                        LoadSession(party);
                        break;
                    case "7":
                        ListSessions();
                        break;
                    case "8":
                        ShowLocalMultiplayer(party);
                        break;
                    case "0":
                        if (_sessionManager != null && _sessionManager.IsActive)
                        {
                            Console.Write("\n⚠️ Session is active! End it first? (y/n): ");
                            if (Console.ReadLine()?.Trim().ToLower() == "y")
                            {
                                EndSession();
                            }
                        }
                        return;
                    default:
                        Console.WriteLine("❌ Invalid choice");
                        Thread.Sleep(1000);
                        break;
                }
            }
        }

        #region Session Operations

        private static void HostNewGame(List<Character> party)
        {
            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                    HOST NEW GAME                                 ║");
            Console.WriteLine("╚══════════════════════════════════════════════════════════════════╝");
            Console.WriteLine();

            if (_sessionManager != null && _sessionManager.IsActive)
            {
                Console.WriteLine("⚠️ A session is already active!");
                Console.Write("End current session and start new? (y/n): ");
                if (Console.ReadLine()?.Trim().ToLower() != "y")
                {
                    return;
                }
                _sessionManager.EndSession();
            }

            Console.Write("\nEnter port (default 7777, press Enter for default): ");
            var portInput = Console.ReadLine();
            int port = NetworkManager.DEFAULT_PORT;
            
            if (!string.IsNullOrWhiteSpace(portInput) && int.TryParse(portInput, out int customPort))
            {
                port = customPort;
            }

            _sessionManager = new MultiplayerSessionManager();
            
            if (_sessionManager.StartHostSession(party, port))
            {
                _lastPort = port;
                _lastHostIp = NetworkManager.GetLocalIPAddress();
                
                Console.WriteLine($"\n✅ Hosting on {_lastHostIp}:{port}");
                Console.WriteLine("\n💡 Share this IP and port with players!");
                Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
                Console.WriteLine("\n⏳ Waiting for players...");
                Console.WriteLine("\nCommands:");
                Console.WriteLine("  's' - Start game");
                Console.WriteLine("  'w' - Wait for more players");
                Console.WriteLine("  'c' - Cancel");
                
                bool waiting = true;
                while (waiting)
                {
                    Console.Write("\n> ");
                    var cmd = Console.ReadLine()?.Trim().ToLower();
                    
                    switch (cmd)
                    {
                        case "s":
                            Console.WriteLine("🎮 Starting game...");
                            waiting = false;
                            break;
                        case "w":
                            Console.WriteLine("⏳ Still waiting...");
                            _sessionManager.DisplaySessionInfo();
                            break;
                        case "c":
                            _sessionManager.EndSession();
                            _sessionManager = null;
                            return;
                        default:
                            Console.WriteLine("❌ Unknown command");
                            break;
                    }
                }
                
                Console.WriteLine("\n✅ Game session ready!");
            }
            else
            {
                Console.WriteLine("\n❌ Failed to start host session");
                _sessionManager = null;
            }
            
            Console.WriteLine("\nPress Enter to continue...");
            Console.ReadLine();
        }

        private static void JoinGame(List<Character> party)
        {
            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                    JOIN GAME                                     ║");
            Console.WriteLine("╚══════════════════════════════════════════════════════════════════╝");
            Console.WriteLine();

            if (_sessionManager != null && _sessionManager.IsActive)
            {
                Console.WriteLine("⚠️ Already in a session!");
                Console.WriteLine("Press Enter to continue...");
                Console.ReadLine();
                return;
            }

            Console.Write("Enter host IP address: ");
            var ipAddress = Console.ReadLine();
            
            if (string.IsNullOrWhiteSpace(ipAddress))
            {
                Console.WriteLine("❌ Invalid IP address");
                Thread.Sleep(1500);
                return;
            }

            Console.Write("Enter port (default 7777, press Enter for default): ");
            var portInput = Console.ReadLine();
            int port = NetworkManager.DEFAULT_PORT;
            
            if (!string.IsNullOrWhiteSpace(portInput) && int.TryParse(portInput, out int customPort))
            {
                port = customPort;
            }

            _sessionManager = new MultiplayerSessionManager();
            
            if (_sessionManager.JoinSession(ipAddress, port))
            {
                _lastHostIp = ipAddress;
                _lastPort = port;
                
                Console.WriteLine($"\n✅ Successfully joined session at {ipAddress}:{port}");
                Thread.Sleep(1000);
                
                // Display synced game state
                _sessionManager.DisplaySessionInfo();
            }
            else
            {
                Console.WriteLine("\n❌ Failed to join session");
                _sessionManager = null;
            }
            
            Console.WriteLine("\nPress Enter to continue...");
            Console.ReadLine();
        }

        private static void ReconnectToSession()
        {
            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                    RECONNECT                                     ║");
            Console.WriteLine("╚══════════════════════════════════════════════════════════════════╝");
            Console.WriteLine();

            if (string.IsNullOrWhiteSpace(_lastHostIp))
            {
                Console.WriteLine("❌ No previous session to reconnect to!");
                Console.WriteLine("Join a game first.");
                Console.WriteLine("\nPress Enter to continue...");
                Console.ReadLine();
                return;
            }

            if (_sessionManager?.NetworkManager == null)
            {
                Console.WriteLine("❌ No session manager available");
                Console.WriteLine("\nPress Enter to continue...");
                Console.ReadLine();
                return;
            }

            Console.WriteLine($"🔄 Reconnecting to {_lastHostIp}:{_lastPort}...");
            Console.WriteLine();

            bool success = _sessionManager.NetworkManager.AttemptReconnect(_lastHostIp, _lastPort);
            
            if (success)
            {
                Console.WriteLine("\n✅ Reconnection successful!");
                Thread.Sleep(1000);
                _sessionManager.DisplaySessionInfo();
            }
            else
            {
                Console.WriteLine("\n❌ Reconnection failed");
                Console.WriteLine("💡 The host may have ended the session or the connection timed out.");
            }
            
            Console.WriteLine("\nPress Enter to continue...");
            Console.ReadLine();
        }

        private static void EndSession()
        {
            if (_sessionManager == null || !_sessionManager.IsActive)
            {
                Console.WriteLine("\n❌ No active session to end");
                Thread.Sleep(1000);
                return;
            }

            Console.Write("\n⚠️ Are you sure you want to end the session? (y/n): ");
            if (Console.ReadLine()?.Trim().ToLower() != "y")
            {
                return;
            }

            Console.Write("💾 Save session before ending? (y/n): ");
            if (Console.ReadLine()?.Trim().ToLower() == "y")
            {
                SaveSession();
            }

            _sessionManager.EndSession();
            _sessionManager = null;
            
            Console.WriteLine("\n✅ Session ended");
            Thread.Sleep(1000);
        }

        #endregion

        #region Save/Load Operations

        private static void SaveSession()
        {
            if (_sessionManager == null || !_sessionManager.IsActive)
            {
                Console.WriteLine("\n❌ No active session to save");
                Console.WriteLine("Press Enter to continue...");
                Console.ReadLine();
                return;
            }

            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                    SAVE SESSION                                  ║");
            Console.WriteLine("╚══════════════════════════════════════════════════════════════════╝");
            Console.WriteLine();

            Console.Write("Enter session name: ");
            var sessionName = Console.ReadLine();
            
            if (string.IsNullOrWhiteSpace(sessionName))
            {
                sessionName = $"Session_{DateTime.Now:yyyyMMdd_HHmmss}";
                Console.WriteLine($"Using default name: {sessionName}");
            }

            if (_sessionManager.SaveSession(sessionName))
            {
                Console.WriteLine("\n✅ Session saved successfully!");
                Console.WriteLine("💡 Remember to also save your character progress from the main menu!");
            }
            else
            {
                Console.WriteLine("\n❌ Failed to save session");
            }
            
            Console.WriteLine("\nPress Enter to continue...");
            Console.ReadLine();
        }

        private static void LoadSession(List<Character> party)
        {
            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                    LOAD SESSION                                  ║");
            Console.WriteLine("╚══════════════════════════════════════════════════════════════════╝");
            Console.WriteLine();

            if (_sessionManager != null && _sessionManager.IsActive)
            {
                Console.WriteLine("⚠️ A session is already active!");
                Console.Write("End current session and load? (y/n): ");
                if (Console.ReadLine()?.Trim().ToLower() != "y")
                {
                    return;
                }
                _sessionManager.EndSession();
            }

            _sessionManager = new MultiplayerSessionManager();
            var savedSessions = _sessionManager.ListSavedSessions();

            if (savedSessions.Count == 0)
            {
                Console.WriteLine("❌ No saved sessions found");
                Console.WriteLine("\nPress Enter to continue...");
                Console.ReadLine();
                return;
            }

            Console.WriteLine("Available sessions:\n");
            for (int i = 0; i < savedSessions.Count; i++)
            {
                Console.WriteLine($"{i + 1}) {savedSessions[i]}");
            }

            Console.Write("\nSelect session (0 to cancel): ");
            if (int.TryParse(Console.ReadLine(), out int choice) && choice > 0 && choice <= savedSessions.Count)
            {
                if (_sessionManager.LoadSession(savedSessions[choice - 1], party))
                {
                    Console.WriteLine("\n✅ Session loaded successfully!");
                    Console.WriteLine("💡 You can now host or join to resume playing");
                }
                else
                {
                    Console.WriteLine("\n❌ Failed to load session");
                    _sessionManager = null;
                }
            }
            else if (choice != 0)
            {
                Console.WriteLine("\n❌ Invalid choice");
            }

            Console.WriteLine("\nPress Enter to continue...");
            Console.ReadLine();
        }

        private static void ListSessions()
        {
            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                    SAVED SESSIONS                                ║");
            Console.WriteLine("╚══════════════════════════════════════════════════════════════════╝");
            Console.WriteLine();

            var tempManager = new MultiplayerSessionManager();
            var savedSessions = tempManager.ListSavedSessions();

            if (savedSessions.Count == 0)
            {
                Console.WriteLine("❌ No saved sessions found");
            }
            else
            {
                Console.WriteLine($"Found {savedSessions.Count} saved session(s):\n");
                foreach (var session in savedSessions)
                {
                    Console.WriteLine($"  📁 {session}");
                }
            }

            Console.WriteLine("\nPress Enter to continue...");
            Console.ReadLine();
        }

        #endregion

        #region Local Multiplayer

        private static void ShowLocalMultiplayer(List<Character> party)
        {
            // Use the existing Multiplayer class for same-PC multiplayer
            var multiplayer = new Multiplayer();
            multiplayer.OpenMultiplayerMenu(party);
        }

        #endregion
    }
}
