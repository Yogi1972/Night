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
    /// <summary>
    /// Manages multiplayer sessions with full save/load and reconnect support
    /// </summary>
    internal class MultiplayerSessionManager
    {
        #region Fields

        private NetworkManager? _networkManager;
        private GameStateSyncData _currentGameState;
        private Dictionary<string, PlayerSessionData> _playerSessions;
        private bool _isActive;
        private readonly string _saveDirectory;

        #endregion

        #region Properties

        public bool IsActive => _isActive;
        public NetworkManager? NetworkManager => _networkManager;
        public GameStateSyncData CurrentGameState => _currentGameState;

        #endregion

        #region Constructor

        public MultiplayerSessionManager()
        {
            _currentGameState = new GameStateSyncData();
            _playerSessions = new Dictionary<string, PlayerSessionData>();
            _isActive = false;
            
            _saveDirectory = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "RPG_Dungeon",
                "MultiplayerSessions"
            );
            
            Directory.CreateDirectory(_saveDirectory);
        }

        #endregion

        #region Session Management

        /// <summary>
        /// Start a new multiplayer session as host
        /// </summary>
        public bool StartHostSession(List<Character> party, int port = NetworkManager.DEFAULT_PORT)
        {
            try
            {
                _networkManager = new NetworkManager();
                
                // Setup event handlers
                _networkManager.OnPlayerConnected += HandlePlayerConnected;
                _networkManager.OnPlayerDisconnected += HandlePlayerDisconnected;
                _networkManager.OnMessageReceived += HandleNetworkMessage;
                _networkManager.OnError += HandleNetworkError;

                if (!_networkManager.StartHost(port))
                {
                    return false;
                }

                InitializeGameState(party);
                _isActive = true;

                Console.WriteLine($"🎮 Multiplayer session started!");
                Console.WriteLine($"📋 Session ID: {_networkManager.SessionId}");
                
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Failed to start host session: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Join an existing multiplayer session
        /// </summary>
        public bool JoinSession(string ipAddress, int port = NetworkManager.DEFAULT_PORT)
        {
            try
            {
                _networkManager = new NetworkManager();
                
                // Setup event handlers
                _networkManager.OnPlayerConnected += HandlePlayerConnected;
                _networkManager.OnPlayerDisconnected += HandlePlayerDisconnected;
                _networkManager.OnMessageReceived += HandleNetworkMessage;
                _networkManager.OnError += HandleNetworkError;

                if (!_networkManager.ConnectToHost(ipAddress, port))
                {
                    return false;
                }

                _isActive = true;
                
                // Request game state sync
                Thread.Sleep(500);
                _networkManager.RequestGameStateSync();

                Console.WriteLine("✅ Joined multiplayer session!");
                
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Failed to join session: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// End the current session
        /// </summary>
        public void EndSession()
        {
            if (_networkManager != null)
            {
                _networkManager.Disconnect();
                _networkManager = null;
            }

            _isActive = false;
            _playerSessions.Clear();
            
            Console.WriteLine("🔚 Multiplayer session ended");
        }

        #endregion

        #region Connection Handling

        /// <summary>
        /// Handle player connected event
        /// </summary>
        private void HandlePlayerConnected(string playerId)
        {
            Console.WriteLine($"🎮 Player connected: {playerId}");

            if (!_playerSessions.ContainsKey(playerId))
            {
                _playerSessions[playerId] = new PlayerSessionData
                {
                    PlayerId = playerId,
                    ConnectedTime = DateTime.UtcNow,
                    IsConnected = true
                };
            }
            else
            {
                _playerSessions[playerId].IsConnected = true;
                _playerSessions[playerId].ReconnectCount++;
                Console.WriteLine($"🔄 Player {playerId} reconnected (reconnects: {_playerSessions[playerId].ReconnectCount})");
            }

            // Send current game state to newly connected/reconnected player
            if (_networkManager != null && _networkManager.IsHost)
            {
                _networkManager.SyncGameState(_currentGameState);
            }
        }

        /// <summary>
        /// Handle player disconnected event
        /// </summary>
        private void HandlePlayerDisconnected(string playerId)
        {
            Console.WriteLine($"🔌 Player disconnected: {playerId}");

            if (_playerSessions.TryGetValue(playerId, out var session))
            {
                session.IsConnected = false;
                session.LastDisconnectTime = DateTime.UtcNow;
                
                Console.WriteLine($"⏰ Player can reconnect within 5 minutes using their reconnect token");
            }
        }

        /// <summary>
        /// Handle network error
        /// </summary>
        private void HandleNetworkError(string error)
        {
            Console.WriteLine($"⚠️ Network error: {error}");
            
            // If we're a client and lost connection, attempt to reconnect
            if (_networkManager != null && !_networkManager.IsHost && !_networkManager.IsConnected)
            {
                Console.WriteLine("🔄 Connection lost! Attempting to reconnect...");
                // Auto-reconnect will be handled by the caller
            }
        }

        /// <summary>
        /// Handle incoming network messages
        /// </summary>
        private void HandleNetworkMessage(NetworkMessage message)
        {
            try
            {
                switch (message.Type)
                {
                    case NetworkMessageType.Heartbeat:
                        _networkManager?.UpdateHeartbeat(message.SenderId);
                        break;

                    case NetworkMessageType.Reconnect:
                        if (_networkManager != null && _networkManager.IsHost)
                        {
                            _networkManager.HandleReconnectRequest(message);
                        }
                        break;

                    case NetworkMessageType.GameStateSync:
                        if (_networkManager != null && _networkManager.IsHost && message.Data == "REQUEST")
                        {
                            _networkManager.SyncGameState(_currentGameState);
                        }
                        break;

                    case NetworkMessageType.FullStateSync:
                        if (_networkManager != null && !_networkManager.IsHost)
                        {
                            _currentGameState = JsonSerializer.Deserialize<GameStateSyncData>(message.Data) 
                                ?? new GameStateSyncData();
                            Console.WriteLine("✅ Game state synchronized");
                        }
                        break;

                    case NetworkMessageType.CombatAction:
                        // Handle combat actions
                        var combatAction = JsonSerializer.Deserialize<CombatActionData>(message.Data);
                        if (combatAction != null)
                        {
                            Console.WriteLine($"⚔️ {combatAction.CharacterName} performed {combatAction.ActionType}");
                        }
                        break;

                    case NetworkMessageType.ChatMessage:
                        Console.WriteLine($"💬 [{message.SenderId}]: {message.Data}");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error handling message: {ex.Message}");
            }
        }

        #endregion

        #region Game State Management

        /// <summary>
        /// Initialize game state for a new session
        /// </summary>
        private void InitializeGameState(List<Character> party)
        {
            _currentGameState = new GameStateSyncData
            {
                SessionId = _networkManager?.SessionId ?? Guid.NewGuid().ToString(),
                Players = party.Select(c => new PlayerStateData
                {
                    CharacterName = c.Name,
                    Health = c.Health,
                    MaxHealth = c.MaxHealth,
                    Mana = c.Mana,
                    MaxMana = c.MaxMana,
                    Gold = c.Inventory.Gold,
                    IsConnected = true
                }).ToList(),
                CurrentLocation = "Starting Area",
                TurnNumber = 0,
                InCombat = false
            };
        }

        /// <summary>
        /// Update game state
        /// </summary>
        public void UpdateGameState(List<Character> party, string location, bool inCombat = false)
        {
            _currentGameState.TurnNumber++;
            _currentGameState.CurrentLocation = location;
            _currentGameState.InCombat = inCombat;

            // Update player states
            foreach (var player in _currentGameState.Players)
            {
                var character = party.FirstOrDefault(c => c.Name == player.CharacterName);
                if (character != null)
                {
                    player.Health = character.Health;
                    player.MaxHealth = character.MaxHealth;
                    player.Mana = character.Mana;
                    player.MaxMana = character.MaxMana;
                    player.Gold = character.Inventory.Gold;
                }
            }

            // Broadcast state update to all connected players
            if (_networkManager != null && _networkManager.IsHost)
            {
                _networkManager.SyncGameState(_currentGameState);
            }
        }

        #endregion

        #region Save/Load

        /// <summary>
        /// Save the current multiplayer session
        /// </summary>
        public bool SaveSession(string sessionName)
        {
            try
            {
                var saveData = new MultiplayerSessionSaveData
                {
                    SessionId = _currentGameState.SessionId,
                    SessionName = sessionName,
                    SavedAt = DateTime.UtcNow,
                    GameState = _currentGameState,
                    PlayerSessions = _playerSessions.Values.ToList()
                };

                var fileName = $"{sessionName}_{DateTime.Now:yyyyMMdd_HHmmss}.mpsave";
                var filePath = Path.Combine(_saveDirectory, fileName);

                var json = JsonSerializer.Serialize(saveData, new JsonSerializerOptions 
                { 
                    WriteIndented = true 
                });
                
                File.WriteAllText(filePath, json, Encoding.UTF8);

                Console.WriteLine($"💾 Session saved: {filePath}");
                Console.WriteLine($"📊 Saved {_playerSessions.Count} player session(s)");
                
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Failed to save session: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Load a multiplayer session
        /// </summary>
        public bool LoadSession(string fileName, List<Character> party)
        {
            try
            {
                var filePath = Path.Combine(_saveDirectory, fileName);
                
                if (!File.Exists(filePath))
                {
                    Console.WriteLine($"❌ Save file not found: {fileName}");
                    return false;
                }

                var json = File.ReadAllText(filePath, Encoding.UTF8);
                var saveData = JsonSerializer.Deserialize<MultiplayerSessionSaveData>(json);

                if (saveData == null)
                {
                    Console.WriteLine("❌ Invalid save file format");
                    return false;
                }

                // Restore game state
                _currentGameState = saveData.GameState;
                _playerSessions = saveData.PlayerSessions.ToDictionary(p => p.PlayerId, p => p);

                Console.WriteLine($"✅ Session loaded: {saveData.SessionName}");
                Console.WriteLine($"📋 Session ID: {saveData.SessionId}");
                Console.WriteLine($"📊 Restored {_playerSessions.Count} player session(s)");
                Console.WriteLine($"📍 Location: {_currentGameState.CurrentLocation}");
                Console.WriteLine($"🎯 Turn: {_currentGameState.TurnNumber}");

                // Update party characters with loaded state
                foreach (var playerState in _currentGameState.Players)
                {
                    var character = party.FirstOrDefault(c => c.Name == playerState.CharacterName);
                    if (character != null)
                    {
                        Console.WriteLine($"  ✓ {playerState.CharacterName}: {playerState.Health}/{playerState.MaxHealth} HP");
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Failed to load session: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// List all saved sessions
        /// </summary>
        public List<string> ListSavedSessions()
        {
            try
            {
                var files = Directory.GetFiles(_saveDirectory, "*.mpsave");
                return files.Select(Path.GetFileName).Where(f => f != null).Cast<string>().ToList();
            }
            catch
            {
                return new List<string>();
            }
        }

        #endregion

        #region Utility

        /// <summary>
        /// Get connected players count
        /// </summary>
        public int GetConnectedPlayersCount()
        {
            return _playerSessions.Values.Count(p => p.IsConnected);
        }

        /// <summary>
        /// Get session info summary
        /// </summary>
        public void DisplaySessionInfo()
        {
            if (!_isActive)
            {
                Console.WriteLine("❌ No active multiplayer session");
                return;
            }

            Console.WriteLine("\n╔══════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                  MULTIPLAYER SESSION INFO                        ║");
            Console.WriteLine("╚══════════════════════════════════════════════════════════════════╝");
            Console.WriteLine();
            Console.WriteLine($"📋 Session ID: {_currentGameState.SessionId}");
            Console.WriteLine($"📍 Location: {_currentGameState.CurrentLocation}");
            Console.WriteLine($"🎯 Turn: {_currentGameState.TurnNumber}");
            Console.WriteLine($"⚔️ In Combat: {(_currentGameState.InCombat ? "Yes" : "No")}");
            Console.WriteLine($"\n👥 Players ({GetConnectedPlayersCount()}/{_playerSessions.Count}):");
            
            foreach (var session in _playerSessions.Values)
            {
                var statusIcon = session.IsConnected ? "🟢" : "🔴";
                var reconnectInfo = session.ReconnectCount > 0 ? $" (reconnects: {session.ReconnectCount})" : "";
                Console.WriteLine($"  {statusIcon} {session.PlayerId}{reconnectInfo}");
                
                if (!session.IsConnected && session.LastDisconnectTime.HasValue)
                {
                    var timeSinceDisconnect = DateTime.UtcNow - session.LastDisconnectTime.Value;
                    Console.WriteLine($"      Last seen: {timeSinceDisconnect.TotalMinutes:F1} minutes ago");
                }
            }
        }

        #endregion
    }

    #region Data Classes

    /// <summary>
    /// Player session tracking data
    /// </summary>
    internal class PlayerSessionData
    {
        public string PlayerId { get; set; } = string.Empty;
        public string PlayerName { get; set; } = string.Empty;
        public DateTime ConnectedTime { get; set; }
        public DateTime? LastDisconnectTime { get; set; }
        public bool IsConnected { get; set; }
        public int ReconnectCount { get; set; }
    }

    /// <summary>
    /// Complete session save data
    /// </summary>
    internal class MultiplayerSessionSaveData
    {
        public string SessionId { get; set; } = string.Empty;
        public string SessionName { get; set; } = string.Empty;
        public DateTime SavedAt { get; set; }
        public GameStateSyncData GameState { get; set; } = new();
        public List<PlayerSessionData> PlayerSessions { get; set; } = new();
    }

    #endregion
}
