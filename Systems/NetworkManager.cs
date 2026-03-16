using Night.Characters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Rpg_Dungeon.Systems
{
    /// <summary>
    /// Manages network connections for LAN multiplayer
    /// </summary>
    internal class NetworkManager
    {
        #region Fields

        private TcpListener? _server;
        private TcpClient? _client;
        private NetworkStream? _stream;
        private bool _isHost;
        private bool _isConnected;
        private string _playerId;
        private Thread? _receiveThread;
        private Thread? _heartbeatThread;
        private Queue<NetworkMessage> _messageQueue;
        private readonly object _queueLock = new();

        // Reconnection support
        private string _sessionId;
        private string _reconnectToken;
        private DateTime _lastHeartbeat;
        private bool _attemptingReconnect;
        private int _reconnectAttempts;
        private const int MAX_RECONNECT_ATTEMPTS = 5;
        private const int RECONNECT_DELAY_MS = 3000;
        private const int HEARTBEAT_INTERVAL_MS = 5000;
        private const int HEARTBEAT_TIMEOUT_MS = 15000;

        // Connected clients (for host)
        private readonly Dictionary<string, ClientConnection> _connectedClients = new();

        public const int DEFAULT_PORT = 7777;

        #endregion

        #region Properties

        public bool IsHost => _isHost;
        public bool IsConnected => _isConnected;
        public string PlayerId => _playerId;
        public string SessionId => _sessionId;
        public string ReconnectToken => _reconnectToken;
        public bool IsReconnecting => _attemptingReconnect;

        #endregion

        #region Events

        public event Action<NetworkMessage>? OnMessageReceived;
        public event Action<string>? OnPlayerConnected;
        public event Action<string>? OnPlayerDisconnected;
        public event Action<string>? OnError;

        #endregion

        #region Constructor

        public NetworkManager()
        {
            _playerId = Guid.NewGuid().ToString();
            _sessionId = Guid.NewGuid().ToString();
            _reconnectToken = Guid.NewGuid().ToString();
            _messageQueue = new Queue<NetworkMessage>();
            _lastHeartbeat = DateTime.UtcNow;
            _attemptingReconnect = false;
            _reconnectAttempts = 0;
        }

        #endregion

        #region Server Methods (Host)

        /// <summary>
        /// Start hosting a game server
        /// </summary>
        public bool StartHost(int port = DEFAULT_PORT)
        {
            try
            {
                _server = new TcpListener(IPAddress.Any, port);
                _server.Start();
                _isHost = true;
                _isConnected = true;

                Console.WriteLine($"✅ Server started on port {port}");
                Console.WriteLine($"📡 Session ID: {_sessionId}");
                Console.WriteLine($"📡 Local IP: {GetLocalIPAddress()}");
                Console.WriteLine("⏳ Waiting for players to connect...");

                // Start accepting clients in background
                Task.Run(() => AcceptClients());

                // Start heartbeat monitoring
                StartHeartbeat();

                return true;
            }
            catch (Exception ex)
            {
                OnError?.Invoke($"Failed to start server: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Accept incoming client connections
        /// </summary>
        private async void AcceptClients()
        {
            while (_isHost && _server != null)
            {
                try
                {
                    var client = await _server.AcceptTcpClientAsync();
                    Console.WriteLine($"🎮 Player connected from {((IPEndPoint)client.Client.RemoteEndPoint!).Address}");
                    
                    // Handle this client in a separate task
                    _ = Task.Run(() => HandleClient(client));
                }
                catch (Exception ex)
                {
                    if (_isHost)
                    {
                        OnError?.Invoke($"Error accepting client: {ex.Message}");
                    }
                    break;
                }
            }
        }

        /// <summary>
        /// Handle individual client connection
        /// </summary>
        private void HandleClient(TcpClient client)
        {
            try
            {
                var stream = client.GetStream();
                var buffer = new byte[4096];

                while (_isHost && client.Connected)
                {
                    var bytesRead = stream.Read(buffer, 0, buffer.Length);
                    if (bytesRead > 0)
                    {
                        var json = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                        var message = NetworkMessage.FromJson(json);
                        
                        if (message != null)
                        {
                            EnqueueMessage(message);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Client disconnected: {ex.Message}");
            }
        }

        #endregion

        #region Client Methods (Join)

        /// <summary>
        /// Connect to a host server
        /// </summary>
        public bool ConnectToHost(string ipAddress, int port = DEFAULT_PORT)
        {
            try
            {
                _client = new TcpClient();
                _client.Connect(ipAddress, port);
                _stream = _client.GetStream();
                _isHost = false;
                _isConnected = true;

                Console.WriteLine($"✅ Connected to server at {ipAddress}:{port}");

                // Start receiving messages
                _receiveThread = new Thread(ReceiveMessages);
                _receiveThread.IsBackground = true;
                _receiveThread.Start();

                return true;
            }
            catch (Exception ex)
            {
                OnError?.Invoke($"Failed to connect: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Receive messages from server
        /// </summary>
        private void ReceiveMessages()
        {
            var buffer = new byte[4096];

            while (_isConnected && _stream != null)
            {
                try
                {
                    var bytesRead = _stream.Read(buffer, 0, buffer.Length);
                    if (bytesRead > 0)
                    {
                        var json = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                        var message = NetworkMessage.FromJson(json);
                        
                        if (message != null)
                        {
                            EnqueueMessage(message);
                        }
                    }
                    else
                    {
                        // Connection closed
                        break;
                    }
                }
                catch (Exception ex)
                {
                    if (_isConnected)
                    {
                        OnError?.Invoke($"Error receiving message: {ex.Message}");
                    }
                    break;
                }
            }

            Disconnect();
        }

        #endregion

        #region Message Handling

        /// <summary>
        /// Send a message to the network
        /// </summary>
        public void SendMessage(NetworkMessage message)
        {
            try
            {
                var json = message.ToJson();
                var data = Encoding.UTF8.GetBytes(json);

                if (_isHost && _server != null)
                {
                    // Broadcast to all clients (implement client list tracking)
                    // For now, just log
                    Console.WriteLine($"📤 [HOST] Sent: {message.Type}");
                }
                else if (_stream != null)
                {
                    _stream.Write(data, 0, data.Length);
                    Console.WriteLine($"📤 [CLIENT] Sent: {message.Type}");
                }
            }
            catch (Exception ex)
            {
                OnError?.Invoke($"Failed to send message: {ex.Message}");
            }
        }

        /// <summary>
        /// Enqueue received message for processing
        /// </summary>
        private void EnqueueMessage(NetworkMessage message)
        {
            lock (_queueLock)
            {
                _messageQueue.Enqueue(message);
            }

            OnMessageReceived?.Invoke(message);
        }

        /// <summary>
        /// Process all queued messages
        /// </summary>
        public List<NetworkMessage> GetPendingMessages()
        {
            lock (_queueLock)
            {
                var messages = _messageQueue.ToList();
                _messageQueue.Clear();
                return messages;
            }
        }

        #endregion

        #region Utility Methods

        /// <summary>
        /// Get local IP address
        /// </summary>
        public static string GetLocalIPAddress()
        {
            try
            {
                var host = Dns.GetHostEntry(Dns.GetHostName());
                foreach (var ip in host.AddressList)
                {
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                    {
                        return ip.ToString();
                    }
                }
                return "127.0.0.1";
            }
            catch
            {
                return "127.0.0.1";
            }
        }

        /// <summary>
        /// Disconnect from network
        /// </summary>
        public void Disconnect()
        {
            _isConnected = false;

            try
            {
                _stream?.Close();
                _client?.Close();
                _server?.Stop();
                _heartbeatThread?.Join(1000);

                Console.WriteLine("🔌 Disconnected from network");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during disconnect: {ex.Message}");
            }
        }

        #endregion

        #region Reconnection Support

        /// <summary>
        /// Attempt to reconnect to the host
        /// </summary>
        public bool AttemptReconnect(string ipAddress, int port = DEFAULT_PORT)
        {
            _attemptingReconnect = true;
            _reconnectAttempts = 0;

            Console.WriteLine($"\n🔄 Attempting to reconnect to {ipAddress}:{port}...");

            while (_reconnectAttempts < MAX_RECONNECT_ATTEMPTS)
            {
                _reconnectAttempts++;
                Console.WriteLine($"Attempt {_reconnectAttempts}/{MAX_RECONNECT_ATTEMPTS}...");

                try
                {
                    _client = new TcpClient();
                    _client.Connect(ipAddress, port);
                    _stream = _client.GetStream();

                    // Send reconnect message with token
                    var reconnectMsg = new NetworkMessage(NetworkMessageType.Reconnect, _playerId, "")
                    {
                        SessionId = _sessionId,
                        ReconnectToken = _reconnectToken
                    };
                    SendMessage(reconnectMsg);

                    // Wait for reconnect response
                    Thread.Sleep(1000);
                    var messages = GetPendingMessages();
                    bool reconnectSuccess = messages.Any(m => m.Type == NetworkMessageType.ReconnectSuccess);

                    if (reconnectSuccess)
                    {
                        _isConnected = true;
                        _attemptingReconnect = false;
                        _reconnectAttempts = 0;

                        // Restart receive thread
                        _receiveThread = new Thread(ReceiveMessages);
                        _receiveThread.IsBackground = true;
                        _receiveThread.Start();

                        // Restart heartbeat
                        StartHeartbeat();

                        Console.WriteLine("✅ Reconnected successfully!");
                        OnPlayerConnected?.Invoke(_playerId);
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Reconnect attempt {_reconnectAttempts} failed: {ex.Message}");
                }

                if (_reconnectAttempts < MAX_RECONNECT_ATTEMPTS)
                {
                    Console.WriteLine($"⏳ Waiting {RECONNECT_DELAY_MS / 1000} seconds before next attempt...");
                    Thread.Sleep(RECONNECT_DELAY_MS);
                }
            }

            _attemptingReconnect = false;
            Console.WriteLine($"❌ Failed to reconnect after {MAX_RECONNECT_ATTEMPTS} attempts");
            OnError?.Invoke("Reconnection failed - maximum attempts reached");
            return false;
        }

        /// <summary>
        /// Handle a reconnection request (host side)
        /// </summary>
        public bool HandleReconnectRequest(NetworkMessage message)
        {
            if (!_isHost) return false;

            try
            {
                // Verify session ID and reconnect token
                if (message.SessionId == _sessionId && !string.IsNullOrEmpty(message.ReconnectToken))
                {
                    // Find the disconnected client
                    if (_connectedClients.TryGetValue(message.SenderId, out var client))
                    {
                        // Verify reconnect token
                        if (client.ReconnectToken == message.ReconnectToken)
                        {
                            client.IsConnected = true;
                            client.LastHeartbeat = DateTime.UtcNow;

                            // Send success message
                            var response = new NetworkMessage(NetworkMessageType.ReconnectSuccess, _playerId, "")
                            {
                                SessionId = _sessionId
                            };
                            SendMessage(response);

                            Console.WriteLine($"✅ Player {message.SenderId} reconnected!");
                            OnPlayerConnected?.Invoke(message.SenderId);
                            return true;
                        }
                    }
                }

                // Send failure message
                var failMsg = new NetworkMessage(NetworkMessageType.ReconnectFailed, _playerId, "Invalid session or token");
                SendMessage(failMsg);
                return false;
            }
            catch (Exception ex)
            {
                OnError?.Invoke($"Error handling reconnect: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Start heartbeat monitoring
        /// </summary>
        private void StartHeartbeat()
        {
            _heartbeatThread = new Thread(() =>
            {
                while (_isConnected)
                {
                    try
                    {
                        if (_isHost)
                        {
                            // Check all clients for timeout
                            foreach (var client in _connectedClients.Values)
                            {
                                var timeSinceHeartbeat = DateTime.UtcNow - client.LastHeartbeat;
                                if (timeSinceHeartbeat.TotalMilliseconds > HEARTBEAT_TIMEOUT_MS)
                                {
                                    Console.WriteLine($"⚠️ Client {client.PlayerId} timed out (no heartbeat)");
                                    client.IsConnected = false;
                                    OnPlayerDisconnected?.Invoke(client.PlayerId);
                                }
                            }
                        }
                        else
                        {
                            // Send heartbeat to host
                            var heartbeat = new NetworkMessage(NetworkMessageType.Heartbeat, _playerId, "")
                            {
                                SessionId = _sessionId
                            };
                            SendMessage(heartbeat);

                            // Check if we've lost connection
                            var timeSinceHeartbeat = DateTime.UtcNow - _lastHeartbeat;
                            if (timeSinceHeartbeat.TotalMilliseconds > HEARTBEAT_TIMEOUT_MS)
                            {
                                Console.WriteLine("⚠️ Connection lost - heartbeat timeout");
                                _isConnected = false;
                                OnError?.Invoke("Connection lost");
                                break;
                            }
                        }

                        Thread.Sleep(HEARTBEAT_INTERVAL_MS);
                    }
                    catch (Exception ex)
                    {
                        if (_isConnected)
                        {
                            OnError?.Invoke($"Heartbeat error: {ex.Message}");
                        }
                        break;
                    }
                }
            });
            _heartbeatThread.IsBackground = true;
            _heartbeatThread.Start();
        }

        /// <summary>
        /// Update heartbeat timestamp
        /// </summary>
        public void UpdateHeartbeat(string playerId)
        {
            _lastHeartbeat = DateTime.UtcNow;

            if (_isHost && _connectedClients.TryGetValue(playerId, out var client))
            {
                client.LastHeartbeat = DateTime.UtcNow;
            }
        }

        #endregion

        #region Game State Sync

        /// <summary>
        /// Sync full game state (for reconnecting players)
        /// </summary>
        public void SyncGameState(GameStateSyncData gameState)
        {
            var json = System.Text.Json.JsonSerializer.Serialize(gameState);
            var message = new NetworkMessage(NetworkMessageType.FullStateSync, _playerId, json)
            {
                SessionId = _sessionId
            };
            SendMessage(message);
        }

        /// <summary>
        /// Request game state sync (after reconnect)
        /// </summary>
        public void RequestGameStateSync()
        {
            var message = new NetworkMessage(NetworkMessageType.GameStateSync, _playerId, "REQUEST")
            {
                SessionId = _sessionId
            };
            SendMessage(message);
        }

        #endregion
    }

    /// <summary>
    /// Client connection info for host tracking
    /// </summary>
    internal class ClientConnection
    {
        public string PlayerId { get; set; } = string.Empty;
        public TcpClient? Client { get; set; }
        public NetworkStream? Stream { get; set; }
        public DateTime LastHeartbeat { get; set; }
        public bool IsConnected { get; set; }
        public string ReconnectToken { get; set; } = string.Empty;
        public string PlayerName { get; set; } = string.Empty;
    }

    /// <summary>
    /// Network lobby manager for pre-game setup
    /// </summary>
    internal class NetworkLobby
    {
        private NetworkManager _networkManager;
        private LobbyStateData _lobbyState;
        private bool _isReady;

        public NetworkLobby(NetworkManager networkManager)
        {
            _networkManager = networkManager;
            _lobbyState = new LobbyStateData();
            _isReady = false;
        }

        public bool IsReady => _isReady;
        public LobbyStateData LobbyState => _lobbyState;

        /// <summary>
        /// Add player to lobby
        /// </summary>
        public void AddPlayer(PlayerJoinData player)
        {
            if (_lobbyState.Players.Count < _lobbyState.MaxPlayers)
            {
                _lobbyState.Players.Add(player);
                BroadcastLobbyUpdate();
            }
        }

        /// <summary>
        /// Remove player from lobby
        /// </summary>
        public void RemovePlayer(string playerName)
        {
            _lobbyState.Players.RemoveAll(p => p.PlayerName == playerName);
            BroadcastLobbyUpdate();
        }

        /// <summary>
        /// Broadcast lobby state to all players
        /// </summary>
        private void BroadcastLobbyUpdate()
        {
            var json = JsonSerializer.Serialize(_lobbyState);
            var message = new NetworkMessage(NetworkMessageType.LobbyUpdate, _networkManager.PlayerId, json);
            _networkManager.SendMessage(message);
        }

        /// <summary>
        /// Start the game
        /// </summary>
        public void StartGame()
        {
            _lobbyState.GameStarted = true;
            _isReady = true;

            var message = new NetworkMessage(NetworkMessageType.GameStart, _networkManager.PlayerId, "START");
            _networkManager.SendMessage(message);
        }
    }
}
