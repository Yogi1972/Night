using Night.Characters;
using Rpg_Dungeon.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Rpg_Dungeon
{
    /// <summary>
    /// Manages networked multiplayer game sessions
    /// </summary>
    public static class NetworkMultiplayerManager
    {
        private static NetworkManager? _networkManager;
        private static NetworkLobby? _lobby;
        private static List<Character>? _party;

        /// <summary>
        /// Host a new network game
        /// </summary>
        public static void HostGame()
        {
            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                    HOST NETWORK GAME                             ║");
            Console.WriteLine("╚══════════════════════════════════════════════════════════════════╝");
            Console.WriteLine();

            // Get port
            Console.Write("Enter port (default 7777): ");
            string? portInput = Console.ReadLine();
            int port = NetworkManager.DEFAULT_PORT;
            if (!string.IsNullOrWhiteSpace(portInput) && int.TryParse(portInput, out int customPort))
            {
                port = customPort;
            }

            // Create network manager
            _networkManager = new NetworkManager();
            _lobby = new NetworkLobby(_networkManager);

            // Start server
            if (!_networkManager.StartHost(port))
            {
                Console.WriteLine("\n❌ Failed to start server!");
                Console.WriteLine("Press Enter to return to menu...");
                Console.ReadLine();
                return;
            }

            Console.WriteLine($"\n📡 Your IP Address: {NetworkManager.GetLocalIPAddress()}");
            Console.WriteLine($"📡 Port: {port}");
            Console.WriteLine("\n💡 Share this information with other players so they can join!");
            Console.WriteLine("\n━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");

            // Create host character
            Console.WriteLine("\n👤 HOST - Create Your Character");
            Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");

            var hostCharacter = CreateNetworkCharacter();
            _party = new List<Character> { hostCharacter };

            // Add host to lobby
            _lobby.AddPlayer(new PlayerJoinData
            {
                PlayerName = "Host",
                CharacterName = hostCharacter.Name,
                CharacterClass = hostCharacter.GetType().Name
            });

            // Wait for players to join
            Console.WriteLine("\n⏳ Waiting for players to join...");
            Console.WriteLine("Commands:");
            Console.WriteLine("  'start' - Start the game");
            Console.WriteLine("  'list' - Show connected players");
            Console.WriteLine("  'kick [name]' - Kick a player");
            Console.WriteLine("  'cancel' - Cancel and return to menu");
            Console.WriteLine();

            bool waiting = true;
            while (waiting)
            {
                Console.Write("> ");
                string? command = Console.ReadLine()?.Trim().ToLower();

                switch (command)
                {
                    case "start":
                        if (_party.Count >= 1)
                        {
                            _lobby.StartGame();
                            waiting = false;
                        }
                        else
                        {
                            Console.WriteLine("⚠️ Need at least 1 player to start!");
                        }
                        break;

                    case "list":
                        ShowConnectedPlayers();
                        break;

                    case "cancel":
                        _networkManager.Disconnect();
                        return;

                    default:
                        if (command?.StartsWith("kick ") == true)
                        {
                            string playerName = command.Substring(5);
                            KickPlayer(playerName);
                        }
                        else
                        {
                            Console.WriteLine("❌ Unknown command. Type 'start', 'list', 'kick [name]', or 'cancel'");
                        }
                        break;
                }
            }

            // Start game
            Console.WriteLine("\n🎮 Starting game with network multiplayer...");
            Thread.Sleep(1000);

            // Generate world seed
            int worldSeed = (int)(DateTime.Now.Ticks & 0x7FFFFFFF);
            
            GameLoopManager.Run(_party, true, worldSeed);

            // Cleanup
            _networkManager.Disconnect();
        }

        /// <summary>
        /// Join an existing network game
        /// </summary>
        public static void JoinGame()
        {
            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                    JOIN NETWORK GAME                             ║");
            Console.WriteLine("╚══════════════════════════════════════════════════════════════════╝");
            Console.WriteLine();

            // Get host IP
            Console.Write("Enter host IP address: ");
            string? ipAddress = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(ipAddress))
            {
                Console.WriteLine("❌ Invalid IP address!");
                Console.WriteLine("Press Enter to return to menu...");
                Console.ReadLine();
                return;
            }

            // Get port
            Console.Write("Enter port (default 7777): ");
            string? portInput = Console.ReadLine();
            int port = NetworkManager.DEFAULT_PORT;
            if (!string.IsNullOrWhiteSpace(portInput) && int.TryParse(portInput, out int customPort))
            {
                port = customPort;
            }

            // Create network manager
            _networkManager = new NetworkManager();

            // Connect to host
            Console.WriteLine($"\n🔌 Connecting to {ipAddress}:{port}...");
            if (!_networkManager.ConnectToHost(ipAddress, port))
            {
                Console.WriteLine("\n❌ Failed to connect to host!");
                Console.WriteLine("Press Enter to return to menu...");
                Console.ReadLine();
                return;
            }

            Console.WriteLine("✅ Connected successfully!");
            Console.WriteLine("\n━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");

            // Create character
            Console.WriteLine("\n👤 Create Your Character");
            Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");

            var character = CreateNetworkCharacter();

            // Send join request to host
            var joinData = new PlayerJoinData
            {
                PlayerName = "Player",
                CharacterName = character.Name,
                CharacterClass = character.GetType().Name
            };

            var joinMessage = new NetworkMessage(
                NetworkMessageType.PlayerJoin,
                _networkManager.PlayerId,
                System.Text.Json.JsonSerializer.Serialize(joinData)
            );

            _networkManager.SendMessage(joinMessage);

            Console.WriteLine("\n⏳ Waiting for host to start the game...");
            Console.WriteLine("(Host will start the game when ready)");

            // Wait for game start
            bool gameStarted = false;
            while (!gameStarted)
            {
                var messages = _networkManager.GetPendingMessages();
                foreach (var message in messages)
                {
                    if (message.Type == NetworkMessageType.GameStart)
                    {
                        gameStarted = true;
                        break;
                    }
                    else if (message.Type == NetworkMessageType.LobbyUpdate)
                    {
                        Console.WriteLine("📊 Lobby updated");
                    }
                }

                Thread.Sleep(100);
            }

            Console.WriteLine("\n🎮 Game starting!");
            Thread.Sleep(1000);

            // TODO: Sync game state and start gameplay
            Console.WriteLine("⚠️ Network gameplay synchronization is in development");
            Console.WriteLine("For now, the host controls the game flow.");
            Console.WriteLine("\nPress Enter to return to menu...");
            Console.ReadLine();

            // Cleanup
            _networkManager.Disconnect();
        }

        /// <summary>
        /// Create a character for network play
        /// </summary>
        private static Character CreateNetworkCharacter()
        {
            Console.Write("Character Name: ");
            var name = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(name))
                name = "Hero";

            Console.WriteLine("\nChoose class:");
            Console.WriteLine("1) Warrior");
            Console.WriteLine("2) Mage");
            Console.WriteLine("3) Rogue");
            Console.WriteLine("4) Priest");
            Console.Write("Choice: ");

            Character? character = null;
            string? choice = Console.ReadLine();

            switch (choice?.Trim())
            {
                case "1":
                    character = new Warrior(name);
                    break;
                case "2":
                    character = new Mage(name);
                    break;
                case "3":
                    character = new Rogue(name);
                    break;
                case "4":
                    character = new Priest(name);
                    break;
                default:
                    Console.WriteLine("Invalid choice, defaulting to Warrior");
                    character = new Warrior(name);
                    break;
            }

            // Apply default race (Human for simplicity in network games)
            character.ApplyRaceBonuses(new Human());
            
            // Give starting gold and equipment
            character.Inventory.AddGold(50);
            StartingEquipmentGenerator.GiveStartingEquipment(character);
            
            // Initialize abilities
            character.InitializeAbilities();

            Console.WriteLine($"\n✅ Created {character.Name} (Lv {character.Level} {character.GetType().Name})");

            return character;
        }

        /// <summary>
        /// Show connected players
        /// </summary>
        private static void ShowConnectedPlayers()
        {
            if (_lobby == null) return;

            Console.WriteLine("\n📊 Connected Players:");
            Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");

            for (int i = 0; i < _lobby.LobbyState.Players.Count; i++)
            {
                var player = _lobby.LobbyState.Players[i];
                Console.WriteLine($"{i + 1}. {player.PlayerName} - {player.CharacterName} ({player.CharacterClass})");
            }

            Console.WriteLine($"\nTotal: {_lobby.LobbyState.Players.Count}/{_lobby.LobbyState.MaxPlayers}");
            Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\n");
        }

        /// <summary>
        /// Kick a player from the lobby
        /// </summary>
        private static void KickPlayer(string playerName)
        {
            if (_lobby == null) return;

            var player = _lobby.LobbyState.Players.FirstOrDefault(p => 
                p.PlayerName.Equals(playerName, StringComparison.OrdinalIgnoreCase));

            if (player != null)
            {
                _lobby.RemovePlayer(player.PlayerName);
                Console.WriteLine($"👢 Kicked {player.PlayerName}");

                // Send disconnect message
                var message = new NetworkMessage(NetworkMessageType.Disconnect, _networkManager!.PlayerId, playerName);
                _networkManager.SendMessage(message);
            }
            else
            {
                Console.WriteLine($"❌ Player '{playerName}' not found");
            }
        }
    }
}
