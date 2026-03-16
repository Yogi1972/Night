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
        private Thread? _heartbeatThread;
        private System.Threading.CancellationTokenSource? _cts;
        private System.Collections.Concurrent.ConcurrentQueue<NetworkMessage> _messageQueue;

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
        private readonly System.Collections.Concurrent.ConcurrentDictionary<string, ClientConnection> _connectedClients = new System.Collections.Concurrent.ConcurrentDictionary<string, ClientConnection>();

        // Lightweight client connection wrapper
        private class ClientConnection : IDisposable
        {
            public string PlayerId { get; }
            public TcpClient? Client { get; }
            public NetworkStream? Stream { get; }
            public string ReconnectToken { get; set; } = string.Empty;
            public volatile bool IsConnected = true;
            public DateTime LastHeartbeat { get; set; } = DateTime.UtcNow;

            public ClientConnection(TcpClient client, NetworkStream stream)
            {
                PlayerId = Guid.NewGuid().ToString();
                Client = client;
                Stream = stream;
                LastHeartbeat = DateTime.UtcNow;
            }

            public void Dispose()
            {
                try { Stream?.Close(); } catch { }
                try { Client?.Close(); } catch { }
            }
        }
        private Timer? _clientPruneTimer;
        private const int MAX_CLIENTS = 8; // per-host limit to avoid OOM under heavy connection churn

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
            _messageQueue = new System.Collections.Concurrent.ConcurrentQueue<NetworkMessage>();
            _lastHeartbeat = DateTime.UtcNow;
            _attemptingReconnect = false;
            _reconnectAttempts = 0;
            // Start periodic pruning of stale clients
            try
            {
                _clientPruneTimer = new Timer(_ => PruneClients(), null, TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1));
            }
            catch { }
            _cts = new System.Threading.CancellationTokenSource();
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
                    // start an async client handler to avoid blocking thread-pool
                    _ = Task.Run(() => HandleClientAsync(client, _cts?.Token ?? System.Threading.CancellationToken.None));
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
        /// Handle individual client connection asynchronously
        /// </summary>
        private async System.Threading.Tasks.Task HandleClientAsync(TcpClient client, System.Threading.CancellationToken ct)
        {
            ClientConnection? conn = null;
            try
            {
                var stream = client.GetStream();
                conn = new ClientConnection(client, stream);
                _connectedClients[conn.PlayerId] = conn;

                var buffer = new byte[4096];

                while (_isHost && client.Connected && !ct.IsCancellationRequested)
                {
                    try
                    {
                        var bytesRead = await stream.ReadAsync(buffer.AsMemory(0, buffer.Length), ct).ConfigureAwait(false);
                        if (bytesRead > 0)
                        {
                            var json = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                            var message = NetworkMessage.FromJson(json);
                            if (message != null)
                            {
                                message.SenderId = conn.PlayerId;
                                EnqueueMessage(message);
                            }
                        }
                        else
                        {
                            // remote closed
                            break;
                        }
                    }
                    catch (OperationCanceledException) { break; }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Client receive error: {ex.Message}");
                        break;
                    }
                }
            }
            finally
            {
                if (conn != null)
                {
                    _connectedClients.TryRemove(conn.PlayerId, out _);
                    conn.Dispose();
                }
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

                // Start receiving messages asynchronously
                _ = System.Threading.Tasks.Task.Run(() => ReceiveMessagesAsync(_cts?.Token ?? System.Threading.CancellationToken.None));

                return true;
            }
            catch (Exception ex)
            {
                OnError?.Invoke($"Failed to connect: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Async receive loop for client
        /// </summary>
        private async System.Threading.Tasks.Task ReceiveMessagesAsync(System.Threading.CancellationToken ct)
        {
            var buffer = new byte[4096];

            while (_isConnected && _stream != null && !ct.IsCancellationRequested)
            {
                try
                {
                    var bytesRead = await _stream.ReadAsync(buffer.AsMemory(0, buffer.Length), ct).ConfigureAwait(false);
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
                catch (OperationCanceledException) { break; }
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
            // Use concurrent queue to avoid locks and potential contention
            _messageQueue.Enqueue(message);

            // Invoke handlers asynchronously to avoid blocking network threads
            try
            {
                var handler = OnMessageReceived;
                if (handler != null)
                {
                    // Invoke asynchronously so slow handlers don't block network threads
                    System.Threading.Tasks.Task.Run(() =>
                    {
                        try { handler.Invoke(message); } catch { }
                    });
                }
            }
            catch { }
        }

        /// <summary>
        /// Process all queued messages
        /// </summary>
        public List<NetworkMessage> GetPendingMessages()
        {
            var messages = new List<NetworkMessage>();
            while (_messageQueue.TryDequeue(out var msg))
            {
                messages.Add(msg);
            }
            return messages;
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
                try { _cts?.Cancel(); } catch { }
                try { _cts?.Dispose(); } catch { }
                try { _clientPruneTimer?.Dispose(); } catch { }

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

                        // Restart receive loop
                        _ = System.Threading.Tasks.Task.Run(() => ReceiveMessagesAsync(_cts?.Token ?? System.Threading.CancellationToken.None));

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
                            foreach (var kv in _connectedClients)
                            {
                                var client = kv.Value;
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

        private void PruneClients()
        {
            try
            {
                var now = DateTime.UtcNow;
                var toRemove = new System.Collections.Generic.List<string>();
                foreach (var kv in _connectedClients)
                {
                    var client = kv.Value;
                    if (!client.IsConnected && (now - client.LastHeartbeat).TotalMinutes > 5)
                    {
                        toRemove.Add(kv.Key);
                    }
                }

                foreach (var id in toRemove)
                {
                    if (_connectedClients.TryRemove(id, out var removed))
                    {
                        try { removed.Dispose(); } catch { }
                        Console.WriteLine($"🧹 Pruned stale client: {id}");
                    }
                }

                if (_connectedClients.Count > MAX_CLIENTS)
                {
                    var drop = _connectedClients.Values.OrderBy(c => c.LastHeartbeat).Take(_connectedClients.Count - MAX_CLIENTS).ToList();
                    foreach (var c in drop)
                    {
                        if (_connectedClients.TryRemove(c.PlayerId, out var rem))
                        {
                            try { rem.Dispose(); } catch { }
                            Console.WriteLine($"🧹 Dropped client due to capacity: {c.PlayerId}");
                        }
                    }
                }
            }
            catch { }
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
