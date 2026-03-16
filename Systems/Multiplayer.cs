using Night.Characters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace Rpg_Dungeon
{
    #region PlayerInfo Class

    internal class PlayerInfo
    {
        #region Properties

        public string PlayerName { get; set; }
        public Character Character { get; set; }
        public int KillCount { get; set; }
        public int TotalDamageDealt { get; set; }
        public int GoldEarned { get; set; }
        public int ChestsOpened { get; set; }
        public int DuelsWon { get; set; }
        public int DuelsLost { get; set; }

        #endregion

        #region Constructor

        public PlayerInfo(string playerName, Character character)
        {
            PlayerName = playerName;
            Character = character;
            KillCount = 0;
            TotalDamageDealt = 0;
            GoldEarned = 0;
            ChestsOpened = 0;
            DuelsWon = 0;
            DuelsLost = 0;
        }

        #endregion

        #region Methods

        public void DisplayStats()
        {
            Console.WriteLine($"\n=== {PlayerName}'s Stats ===");
            Console.WriteLine($"Character: {Character.Name} (Lv {Character.Level} {Character.GetType().Name})");
            Console.WriteLine($"HP: {Character.Health}/{Character.MaxHealth} | Mana: {Character.Mana}/{Character.MaxMana}");
            Console.WriteLine($"Gold: {Character.Inventory.Gold}");
            Console.WriteLine($"\nCombat Stats:");
            Console.WriteLine($"  Kills: {KillCount}");
            Console.WriteLine($"  Total Damage Dealt: {TotalDamageDealt}");
            Console.WriteLine($"  Gold Earned: {GoldEarned}g");
            Console.WriteLine($"  Chests Opened: {ChestsOpened}");
            if (DuelsWon > 0 || DuelsLost > 0)
            {
                Console.WriteLine($"\nDuel Record:");
                Console.WriteLine($"  Wins: {DuelsWon}");
                Console.WriteLine($"  Losses: {DuelsLost}");
                double winRate = DuelsWon + DuelsLost > 0 ? (double)DuelsWon / (DuelsWon + DuelsLost) * 100 : 0;
                Console.WriteLine($"  Win Rate: {winRate:F1}%");
            }
        }

        #endregion
    }

    #endregion

    #region Multiplayer Class

    internal class Multiplayer
    {
        #region Fields

        private List<PlayerInfo> _players;
        private bool _sessionActive;
        private readonly Trading _trading;

        #endregion

        #region Constructor

        public Multiplayer()
        {
            _players = new List<PlayerInfo>();
            _sessionActive = false;
            _trading = new Trading();
        }

        #endregion

        #region Session Management

        // Get player name for a character (for display purposes)
        public string GetPlayerTag(Character character)
        {
            var player = _players.FirstOrDefault(p => p.Character == character);
            return player != null ? $"[{player.PlayerName}]" : "[No Player]";
        }

        // Check if any multiplayer session is active
        public bool IsSessionActive()
        {
            return _sessionActive && _players.Count > 0;
        }

        // Quick health check accessible from main menu
        public void QuickHealthCheck(List<Character> party)
        {
            ShowPartyHealthStatus(party);
            Console.Write("\nPress Enter to continue...");
            Console.ReadLine();
        }

        // Get list of unclaimed characters
        private List<Character> GetUnclaimedCharacters(List<Character> party)
        {
            return party.Where(c => !_players.Any(p => p.Character == c)).ToList();
        }

        #endregion

        #region Session Setup

        public void SetupMultiplayer(List<Character> party)
        {
            Console.WriteLine("\n=== MULTIPLAYER SETUP ===");
            Console.WriteLine($"Party size: {party.Count}");

            int numPlayers = 0;
            while (numPlayers < 1 || numPlayers > party.Count)
            {
                Console.Write($"How many human players? (1-{party.Count}): ");
                if (!int.TryParse(Console.ReadLine(), out numPlayers) || numPlayers < 1 || numPlayers > party.Count)
                {
                    Console.WriteLine("Invalid number.");
                    numPlayers = 0;
                }
            }

            _players.Clear();

            // Assign players to characters
            var availableCharacters = new List<Character>(party);
            for (int i = 0; i < numPlayers; i++)
            {
                Console.Write($"\nPlayer {i + 1} name: ");
                string playerName = Console.ReadLine() ?? $"Player {i + 1}";
                if (string.IsNullOrWhiteSpace(playerName)) playerName = $"Player {i + 1}";

                Console.WriteLine("Choose your character:");
                for (int j = 0; j < availableCharacters.Count; j++)
                {
                    var c = availableCharacters[j];
                    Console.WriteLine($"{j + 1}) {c.Name} (Lv {c.Level} {c.GetType().Name})");
                }

                int charChoice = 0;
                while (charChoice < 1 || charChoice > availableCharacters.Count)
                {
                    Console.Write("Choice: ");
                    if (!int.TryParse(Console.ReadLine(), out charChoice) || charChoice < 1 || charChoice > availableCharacters.Count)
                    {
                        Console.WriteLine("Invalid choice.");
                        charChoice = 0;
                    }
                }

                var chosenChar = availableCharacters[charChoice - 1];
                _players.Add(new PlayerInfo(playerName, chosenChar));
                availableCharacters.RemoveAt(charChoice - 1);

                Console.WriteLine($"✅ {playerName} is now controlling {chosenChar.Name}!");
            }

            _sessionActive = true;
            Console.WriteLine($"\n🎮 {numPlayers} player(s) ready!");

            // Show unclaimed characters if any
            var unclaimed = GetUnclaimedCharacters(party);
            if (unclaimed.Count > 0)
            {
                Console.WriteLine($"\n⚠️ Unclaimed characters ({unclaimed.Count}):");
                foreach (var c in unclaimed)
                {
                    Console.WriteLine($"  - {c.Name} (Lv {c.Level} {c.GetType().Name})");
                }
                Console.WriteLine("Players can join and claim these later!");
            }
        }

        // Join existing multiplayer session (for late arrivals)
        private void JoinExistingSession(List<Character> party)
        {
            if (!_sessionActive)
            {
                Console.WriteLine("❌ No active multiplayer session! Setup a new session or load a saved one first.");
                return;
            }

            var unclaimed = GetUnclaimedCharacters(party);
            if (unclaimed.Count == 0)
            {
                Console.WriteLine("❌ All characters are already claimed! No available characters to join.");
                return;
            }

            Console.WriteLine("\n=== JOIN EXISTING SESSION ===");
            Console.Write("Enter your player name: ");
            string playerName = Console.ReadLine() ?? "New Player";
            if (string.IsNullOrWhiteSpace(playerName)) playerName = "New Player";

            // Check if player name already exists
            if (_players.Any(p => p.PlayerName.Equals(playerName, StringComparison.OrdinalIgnoreCase)))
            {
                Console.WriteLine($"⚠️ Player name '{playerName}' is already taken! Choose a different name.");
                return;
            }

            Console.WriteLine("\nAvailable characters to claim:");
            for (int i = 0; i < unclaimed.Count; i++)
            {
                var c = unclaimed[i];
                Console.WriteLine($"{i + 1}) {c.Name} (Lv {c.Level} {c.GetType().Name})");
                Console.WriteLine($"    HP: {c.Health}/{c.MaxHealth} | Mana: {c.Mana}/{c.MaxMana} | Gold: {c.Inventory.Gold}g");
            }

            Console.Write("\nChoose character: ");
            if (!int.TryParse(Console.ReadLine(), out int choice) || choice < 1 || choice > unclaimed.Count)
            {
                Console.WriteLine("Invalid choice.");
                return;
            }

            var chosenChar = unclaimed[choice - 1];
            _players.Add(new PlayerInfo(playerName, chosenChar));
            Console.WriteLine($"\n✅ Welcome {playerName}! You are now controlling {chosenChar.Name}!");
            Console.WriteLine($"🎮 Total players in session: {_players.Count}");

            // Show remaining unclaimed characters if any
            var remainingUnclaimed = GetUnclaimedCharacters(party);
            if (remainingUnclaimed.Count > 0)
            {
                Console.WriteLine($"\n⚠️ {remainingUnclaimed.Count} character(s) still unclaimed:");
                foreach (var c in remainingUnclaimed)
                {
                    Console.WriteLine($"  - {c.Name}");
                }
            }
            else
            {
                Console.WriteLine("\n🎉 All characters are now claimed!");
            }
        }

        #endregion

        #region View Methods

        // View current player assignments
        private void ViewPlayerAssignments(List<Character> party)
        {
            Console.WriteLine("\n=== PLAYER ASSIGNMENTS ===");

            if (!_sessionActive || _players.Count == 0)
            {
                Console.WriteLine("❌ No active multiplayer session!");
                return;
            }

            Console.WriteLine($"\n📊 Active Players ({_players.Count}):");
            foreach (var player in _players)
            {
                Console.WriteLine($"\n🎮 {player.PlayerName}");
                Console.WriteLine($"   → Controls: {player.Character.Name} (Lv {player.Character.Level} {player.Character.GetType().Name})");
                Console.WriteLine($"   → HP: {player.Character.Health}/{player.Character.MaxHealth} | Mana: {player.Character.Mana}/{player.Character.MaxMana}");
                Console.WriteLine($"   → Gold: {player.Character.Inventory.Gold}g");
            }

            var unclaimed = GetUnclaimedCharacters(party);
            if (unclaimed.Count > 0)
            {
                Console.WriteLine($"\n⚠️ Unclaimed Characters ({unclaimed.Count}):");
                foreach (var c in unclaimed)
                {
                    Console.WriteLine($"   🔓 {c.Name} (Lv {c.Level} {c.GetType().Name}) - Available to join");
                }
                Console.WriteLine("\n💡 TIP: More players can join using 'Join Existing Session' option!");
            }
            else
            {
                Console.WriteLine("\n✅ All characters are claimed!");
            }
        }

        // Show party health status (especially useful for healers)
        private void ShowPartyHealthStatus(List<Character> party)
        {
            Console.WriteLine("\n=== PARTY HEALTH STATUS ===");
            Console.WriteLine("💚 = Healthy | 💛 = Injured | ❤️ = Critical\n");

            // Sort party by health percentage (lowest first - most damaged)
            var sortedByHealth = party
                .Select(c => new
                {
                    Character = c,
                    HealthPercent = c.MaxHealth > 0 ? (double)c.Health / c.MaxHealth * 100 : 0,
                    ManaPercent = c.MaxMana > 0 ? (double)c.Mana / c.MaxMana * 100 : 0,
                    MissingHealth = c.MaxHealth - c.Health,
                    Player = _sessionActive ? _players.FirstOrDefault(p => p.Character == c) : null
                })
                .OrderBy(x => x.HealthPercent)
                .ToList();

            Console.WriteLine("📊 Priority Healing Order (Most Damaged First):\n");

            int priority = 1;
            foreach (var info in sortedByHealth)
            {
                var c = info.Character;
                string playerTag = info.Player != null ? $"[{info.Player.PlayerName}]" : "[No Player]";
                string healthIcon = info.HealthPercent >= 70 ? "💚" :
                                   info.HealthPercent >= 35 ? "💛" : "❤️";

                string urgency = info.HealthPercent < 35 ? " ⚠️ CRITICAL!" :
                                info.HealthPercent < 70 ? " ⚡ Needs Healing" : " ✓ Healthy";

                Console.WriteLine($"{priority}. {healthIcon} {c.Name} {playerTag} - {c.GetType().Name}");
                Console.WriteLine($"   HP: {c.Health}/{c.MaxHealth} ({info.HealthPercent:F1}%) | Mana: {c.Mana}/{c.MaxMana} ({info.ManaPercent:F1}%)");
                Console.WriteLine($"   Missing HP: {info.MissingHealth} {urgency}");

                // Show if character is a priest
                if (c is Priest)
                {
                    int healPower = c.Intelligence * 2;
                    int healCost = 12;
                    int possibleHeals = c.Mana / healCost;
                    Console.WriteLine($"   🩺 PRIEST - Heal Power: {healPower} HP | Heals Available: {possibleHeals}");
                }

                Console.WriteLine();
                priority++;
            }

            // Show summary stats
            var totalHealth = sortedByHealth.Sum(x => x.Character.Health);
            var totalMaxHealth = sortedByHealth.Sum(x => x.Character.MaxHealth);
            var totalMissingHealth = totalMaxHealth - totalHealth;
            var avgHealthPercent = totalMaxHealth > 0 ? (double)totalHealth / totalMaxHealth * 100 : 0;

            Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
            Console.WriteLine($"📊 Party Summary:");
            Console.WriteLine($"   Total HP: {totalHealth}/{totalMaxHealth} ({avgHealthPercent:F1}%)");
            Console.WriteLine($"   Total Damage Taken: {totalMissingHealth} HP");
            Console.WriteLine($"   Critical Members: {sortedByHealth.Count(x => x.HealthPercent < 35)}");
            Console.WriteLine($"   Injured Members: {sortedByHealth.Count(x => x.HealthPercent < 70)}");
            Console.WriteLine($"   Healthy Members: {sortedByHealth.Count(x => x.HealthPercent >= 70)}");

            // Show priest recommendations
            var priests = sortedByHealth.Where(x => x.Character is Priest).ToList();
            if (priests.Any())
            {
                Console.WriteLine("\n🩺 Priest Recommendations:");
                foreach (var priest in priests)
                {
                    int healPower = priest.Character.Intelligence * 2;
                    int healCost = 12;
                    int possibleHeals = priest.Character.Mana / healCost;

                    Console.WriteLine($"\n   {priest.Character.Name} {(priest.Player != null ? $"[{priest.Player.PlayerName}]" : "")}:");
                    Console.WriteLine($"   - Can perform {possibleHeals} heal(s) for {healPower} HP each");

                    // Suggest who to heal
                    var mostDamaged = sortedByHealth.First();
                    if (mostDamaged.Character != priest.Character && mostDamaged.MissingHealth > 0)
                    {
                        Console.WriteLine($"   💡 Suggestion: Heal {mostDamaged.Character.Name} (missing {mostDamaged.MissingHealth} HP)");
                    }
                }
            }
            else
            {
                Console.WriteLine("\n⚠️ No priests in party! Consider resting or using items.");
            }

            Console.WriteLine("\n💡 TIP: Priests should prioritize characters at the top of this list!");
        }

        #endregion

        #region Menu Interface

        public void OpenMultiplayerMenu(List<Character> party)
        {
            while (true)
            {
                Console.WriteLine("\n=== MULTIPLAYER MENU ===");
                if (_sessionActive)
                {
                    Console.WriteLine($"📊 Active Session: {_players.Count} player(s) connected");
                    var unclaimed = GetUnclaimedCharacters(party);
                    if (unclaimed.Count > 0)
                    {
                        Console.WriteLine($"⚠️ {unclaimed.Count} unclaimed character(s)");
                    }
                }
                else
                {
                    Console.WriteLine("❌ No active session");
                }

                Console.WriteLine("\n--- Session Management ---");
                Console.WriteLine("1) Setup Players (New Session)");
                Console.WriteLine("2) Join Existing Session");
                Console.WriteLine("3) View Player Assignments");
                Console.WriteLine("4) Party Health Status (Priest View)");
                Console.WriteLine("\n--- Activities ---");
                Console.WriteLine("5) View Player Stats");
                Console.WriteLine("6) Trading Post");
                Console.WriteLine("7) Dueling Arena (PvP)");
                Console.WriteLine("8) Co-op Dungeon Challenge");
                Console.WriteLine("9) Leaderboard");
                Console.WriteLine("\n--- Save/Load ---");
                Console.WriteLine("10) Save Multiplayer Session");
                Console.WriteLine("11) Load Multiplayer Session");
                Console.WriteLine("0) Back");
                Console.Write("Choose: ");

                var choice = Console.ReadLine() ?? string.Empty;

                switch (choice.Trim())
                {
                    case "1":
                        SetupMultiplayer(party);
                        break;
                    case "2":
                        JoinExistingSession(party);
                        break;
                    case "3":
                        ViewPlayerAssignments(party);
                        break;
                    case "4":
                        ShowPartyHealthStatus(party);
                        break;
                    case "5":
                        ViewPlayerStats();
                        break;
                    case "6":
                        TradingPost();
                        break;
                    case "7":
                        DuelingArena();
                        break;
                    case "8":
                        CoopDungeonChallenge(party);
                        break;
                    case "9":
                        ShowLeaderboard();
                        break;
                    case "10":
                        SaveMultiplayerSession();
                        break;
                    case "11":
                        LoadMultiplayerSession(party);
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Invalid choice.");
                        break;
                }
            }
        }

        #endregion

        #region Player Activities

        private void ViewPlayerStats()
        {
            if (_players.Count == 0)
            {
                Console.WriteLine("No multiplayer session active. Setup players first!");
                return;
            }

            foreach (var player in _players)
            {
                player.DisplayStats();
            }
        }

        private void TradingPost()
        {
            if (_players.Count < 2)
            {
                Console.WriteLine("Need at least 2 players for trading!");
                return;
            }

            Console.WriteLine("\n=== MULTIPLAYER TRADING POST ===");
            Console.WriteLine("Active Players:");
            for (int i = 0; i < _players.Count; i++)
            {
                var player = _players[i];
                Console.WriteLine($"{i + 1}) {player.PlayerName} → {player.Character.Name} (Lv {player.Character.Level})");
                Console.WriteLine($"    Gold: {player.Character.Inventory.Gold}g");
            }

            // Use the comprehensive trading system
            var characters = _players.Select(p => p.Character).ToList();
            _trading.OpenTradeMenu(characters);

            Console.WriteLine("\n✅ Returned to Multiplayer Menu.");
        }

        private void DuelingArena()
        {
            if (_players.Count < 2)
            {
                Console.WriteLine("Need at least 2 players for dueling!");
                return;
            }

            Console.WriteLine("\n=== DUELING ARENA ===");
            Console.WriteLine("⚔️ PLAYER VS PLAYER COMBAT ⚔️");

            Console.WriteLine("\nSelect Player 1 (challenger):");
            for (int i = 0; i < _players.Count; i++)
            {
                Console.WriteLine($"{i + 1}) {_players[i].PlayerName} ({_players[i].Character.Name} - HP: {_players[i].Character.Health}/{_players[i].Character.MaxHealth})");
            }

            if (!int.TryParse(Console.ReadLine(), out int p1Idx) || p1Idx < 1 || p1Idx > _players.Count)
            {
                Console.WriteLine("Invalid selection.");
                return;
            }

            Console.WriteLine("\nSelect Player 2 (defender):");
            for (int i = 0; i < _players.Count; i++)
            {
                if (i != p1Idx - 1)
                    Console.WriteLine($"{i + 1}) {_players[i].PlayerName} ({_players[i].Character.Name} - HP: {_players[i].Character.Health}/{_players[i].Character.MaxHealth})");
            }

            if (!int.TryParse(Console.ReadLine(), out int p2Idx) || p2Idx < 1 || p2Idx > _players.Count || p2Idx == p1Idx)
            {
                Console.WriteLine("Invalid selection.");
                return;
            }

            var player1 = _players[p1Idx - 1];
            var player2 = _players[p2Idx - 1];

            // Store original HP to restore after duel
            int p1OrigHP = player1.Character.Health;
            int p2OrigHP = player2.Character.Health;

            Console.WriteLine($"\n{player1.PlayerName} vs {player2.PlayerName} - FIGHT!");

            // Simple turn-based combat
            bool player1Turn = true;
            while (player1.Character.Health > 0 && player2.Character.Health > 0)
            {
                var attacker = player1Turn ? player1 : player2;
                var defender = player1Turn ? player2 : player1;

                Console.WriteLine($"\n--- {attacker.PlayerName}'s Turn ---");
                Console.WriteLine($"{attacker.Character.Name}: {attacker.Character.Health} HP | {defender.Character.Name}: {defender.Character.Health} HP");
                Console.WriteLine("1) Attack");
                Console.WriteLine("2) Use Ability (costs mana)");
                Console.WriteLine("3) Defend (+50% defense this turn)");

                var action = Console.ReadLine() ?? string.Empty;

                if (action == "1")
                {
                    int damage = attacker.Character.Strength + new Random().Next(1, 11);
                    defender.Character.ReceiveDamage(damage);
                    attacker.TotalDamageDealt += damage;
                    Console.WriteLine($"{attacker.Character.Name} attacks for {damage} damage!");
                }
                else if (action == "2")
                {
                    if (attacker.Character.Mana >= 10)
                    {
                        // Use mana (simplified - use RestoreMana with negative won't work, so we skip mana cost for duel)
                        int damage = attacker.Character.Intelligence + new Random().Next(10, 21);
                        defender.Character.ReceiveDamage(damage);
                        attacker.TotalDamageDealt += damage;
                        Console.WriteLine($"{attacker.Character.Name} uses ability for {damage} damage!");
                    }
                    else
                    {
                        Console.WriteLine("Not enough mana! Turn wasted.");
                    }
                }
                else if (action == "3")
                {
                    Console.WriteLine($"{attacker.Character.Name} takes a defensive stance!");
                    // Defender takes less damage next turn (simplified - just heal a bit)
                    int healAmount = attacker.Character.MaxHealth / 10;
                    attacker.Character.Heal(healAmount);
                    Console.WriteLine($"{attacker.Character.Name} recovers {healAmount} HP!");
                }

                player1Turn = !player1Turn;
            }

            // Determine winner
            PlayerInfo winner, loser;
            if (player1.Character.Health > 0)
            {
                winner = player1;
                loser = player2;
            }
            else
            {
                winner = player2;
                loser = player1;
            }

            Console.WriteLine($"\n🏆 {winner.PlayerName} WINS THE DUEL! 🏆");
            winner.DuelsWon++;
            loser.DuelsLost++;

            // Restore HP after duel (heal to original HP)
            int p1HealAmount = p1OrigHP - player1.Character.Health;
            int p2HealAmount = p2OrigHP - player2.Character.Health;
            if (p1HealAmount > 0) player1.Character.Heal(p1HealAmount);
            if (p2HealAmount > 0) player2.Character.Heal(p2HealAmount);
            Console.WriteLine("\nBoth players' health restored after duel.");
        }

        private void CoopDungeonChallenge(List<Character> party)
        {
            if (_players.Count == 0)
            {
                Console.WriteLine("No multiplayer session active. Setup players first!");
                return;
            }

            Console.WriteLine("\n=== CO-OP DUNGEON CHALLENGE ===");
            Console.WriteLine("Work together to complete a dungeon!");
            Console.WriteLine($"{_players.Count} player(s) ready.");

            // Reset session stats
            foreach (var player in _players)
            {
                player.KillCount = 0;
                player.TotalDamageDealt = 0;
                player.GoldEarned = 0;
                player.ChestsOpened = 0;
            }

            var rng = new Random();
            int levels = rng.Next(2, 5); // 2-4 floors
            Console.WriteLine($"Generated Co-op Dungeon: {levels} floor(s).");

            Console.Write("\nPress Enter to start...");
            Console.ReadLine();

            var dungeon = new Dungeon(levels);
            dungeon.Explore(party);

            // Show co-op results
            Console.WriteLine("\n=== CO-OP DUNGEON COMPLETE! ===");
            Console.WriteLine("Player Performance:");
            foreach (var player in _players)
            {
                Console.WriteLine($"\n{player.PlayerName} ({player.Character.Name}):");
                Console.WriteLine($"  Kills: {player.KillCount}");
                Console.WriteLine($"  Damage: {player.TotalDamageDealt}");
                Console.WriteLine($"  Gold Earned: {player.GoldEarned}g");
                Console.WriteLine($"  Chests: {player.ChestsOpened}");
            }

            // MVP calculation
            var mvp = _players.OrderByDescending(p => p.KillCount + (p.TotalDamageDealt / 10) + p.ChestsOpened).First();
            Console.WriteLine($"\n⭐ MVP: {mvp.PlayerName}! ⭐");
        }

        private void ShowLeaderboard()
        {
            if (_players.Count == 0)
            {
                Console.WriteLine("No multiplayer session active. Setup players first!");
                return;
            }

            Console.WriteLine("\n=== LEADERBOARD ===");

            Console.WriteLine("\n🏆 Most Kills:");
            foreach (var p in _players.OrderByDescending(p => p.KillCount).Take(3))
            {
                Console.WriteLine($"  {p.PlayerName}: {p.KillCount} kills");
            }

            Console.WriteLine("\n💥 Most Damage:");
            foreach (var p in _players.OrderByDescending(p => p.TotalDamageDealt).Take(3))
            {
                Console.WriteLine($"  {p.PlayerName}: {p.TotalDamageDealt} damage");
            }

            Console.WriteLine("\n💰 Richest Player:");
            foreach (var p in _players.OrderByDescending(p => p.Character.Inventory.Gold).Take(3))
            {
                Console.WriteLine($"  {p.PlayerName}: {p.Character.Inventory.Gold}g");
            }

            Console.WriteLine("\n⚔️ Best Duelist:");
            foreach (var p in _players.OrderByDescending(p => p.DuelsWon).Take(3))
            {
                if (p.DuelsWon > 0)
                {
                    double winRate = p.DuelsWon + p.DuelsLost > 0 ? (double)p.DuelsWon / (p.DuelsWon + p.DuelsLost) * 100 : 0;
                    Console.WriteLine($"  {p.PlayerName}: {p.DuelsWon}W-{p.DuelsLost}L ({winRate:F1}%)");
                }
            }
        }

        #endregion

        #region Stats Tracking

        // Helper method to track kills for multiplayer stats
        public void TrackKill(Character character, int damageDealt, int goldEarned)
        {
            var player = _players.FirstOrDefault(p => p.Character == character);
            if (player != null)
            {
                player.KillCount++;
                player.TotalDamageDealt += damageDealt;
                player.GoldEarned += goldEarned;
            }
        }

        // Helper method to track chest opening
        public void TrackChestOpened(Character character)
        {
            var player = _players.FirstOrDefault(p => p.Character == character);
            if (player != null)
            {
                player.ChestsOpened++;
            }
        }

        #endregion

        #region Save/Load System

        // Save multiplayer session
        private void SaveMultiplayerSession()
        {
            if (_players.Count == 0)
            {
                Console.WriteLine("No multiplayer session active! Setup players first.");
                return;
            }

            try
            {
                var fileName = $"multiplayer_{DateTime.Now:yyyyMMdd_HHmmss}.json";

                var save = new MultiplayerSaveFile
                {
                    Created = DateTime.Now,
                    PlayerCount = _players.Count,
                    Players = _players.Select(p => new PlayerData
                    {
                        PlayerName = p.PlayerName,
                        CharacterName = p.Character.Name,
                        KillCount = p.KillCount,
                        TotalDamageDealt = p.TotalDamageDealt,
                        GoldEarned = p.GoldEarned,
                        ChestsOpened = p.ChestsOpened,
                        DuelsWon = p.DuelsWon,
                        DuelsLost = p.DuelsLost
                    }).ToList()
                };

                var opts = new JsonSerializerOptions { WriteIndented = true };
                var json = JsonSerializer.Serialize(save, opts);
                File.WriteAllText(fileName, json, Encoding.UTF8);
                Console.WriteLine($"\n✅ Multiplayer session saved to '{fileName}'!");
                Console.WriteLine($"Saved {_players.Count} player(s) and their statistics.");
                Console.WriteLine("\n💡 TIP: Make sure to also save your game (party/inventory) from the main menu!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Failed to save multiplayer session: {ex.Message}");
            }
        }

        // Load multiplayer session
        private void LoadMultiplayerSession(List<Character> party)
        {
            try
            {
                Console.WriteLine("\n=== LOAD MULTIPLAYER SESSION ===");
                Console.WriteLine("Enter multiplayer save filename (or press enter to list saves):");
                var input = Console.ReadLine() ?? string.Empty;
                string file = input.Trim();

                if (string.IsNullOrWhiteSpace(file))
                {
                    var files = Directory.GetFiles(Directory.GetCurrentDirectory(), "multiplayer_*.json");
                    if (files.Length == 0)
                    {
                        Console.WriteLine("No multiplayer save files found.");
                        return;
                    }

                    Console.WriteLine("\nAvailable multiplayer saves:");
                    for (int i = 0; i < files.Length; i++)
                    {
                        var fi = new FileInfo(files[i]);
                        Console.WriteLine($"{i + 1}) {Path.GetFileName(files[i])} (Created: {fi.CreationTime})");
                    }

                    Console.Write("\nChoose save number to load: ");
                    var sel = Console.ReadLine() ?? string.Empty;
                    if (!int.TryParse(sel, out var idx) || idx < 1 || idx > files.Length)
                    {
                        Console.WriteLine("Invalid selection.");
                        return;
                    }
                    file = files[idx - 1];
                }

                if (!File.Exists(file))
                {
                    Console.WriteLine("File not found.");
                    return;
                }

                var json = File.ReadAllText(file, Encoding.UTF8);
                var opts = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var save = JsonSerializer.Deserialize<MultiplayerSaveFile>(json, opts);

                if (save == null || save.Players == null)
                {
                    Console.WriteLine("Invalid save file format.");
                    return;
                }

                // Check if party has enough characters
                if (save.PlayerCount > party.Count)
                {
                    Console.WriteLine($"⚠️ Warning: Save file has {save.PlayerCount} players but current party only has {party.Count} characters.");
                    Console.WriteLine("Some player assignments may not be restored.");
                }

                Console.WriteLine($"\n💾 Loading session with {save.PlayerCount} player(s)...");
                Console.Write("\n🎮 Start with partial session if not all players present? (y/n): ");
                var partialAnswer = Console.ReadLine() ?? string.Empty;
                bool allowPartial = partialAnswer.Trim().Equals("y", StringComparison.OrdinalIgnoreCase);

                // Clear current multiplayer session
                _players.Clear();

                // Restore player data
                int restoredCount = 0;
                int skippedCount = 0;
                foreach (var playerData in save.Players)
                {
                    // Find matching character by name
                    var character = party.FirstOrDefault(c => c.Name == playerData.CharacterName);
                    if (character == null)
                    {
                        Console.WriteLine($"⚠️ Character '{playerData.CharacterName}' not found in party. Skipping player '{playerData.PlayerName}'.");
                        skippedCount++;
                        continue;
                    }

                    // Check if character is already assigned
                    if (_players.Any(p => p.Character == character))
                    {
                        Console.WriteLine($"⚠️ Character '{playerData.CharacterName}' already assigned. Skipping duplicate.");
                        skippedCount++;
                        continue;
                    }

                    // Restore player
                    var playerInfo = new PlayerInfo(playerData.PlayerName, character)
                    {
                        KillCount = playerData.KillCount,
                        TotalDamageDealt = playerData.TotalDamageDealt,
                        GoldEarned = playerData.GoldEarned,
                        ChestsOpened = playerData.ChestsOpened,
                        DuelsWon = playerData.DuelsWon,
                        DuelsLost = playerData.DuelsLost
                    };

                    _players.Add(playerInfo);
                    restoredCount++;
                    Console.WriteLine($"✅ Restored: {playerData.PlayerName} → {playerData.CharacterName}");
                }

                if (restoredCount > 0)
                {
                    _sessionActive = true;
                    Console.WriteLine($"\n🎮 Successfully loaded {restoredCount} player(s)!");

                    if (skippedCount > 0)
                    {
                        Console.WriteLine($"⚠️ {skippedCount} player(s) were not restored (missing characters or duplicates)");
                    }

                    Console.WriteLine("\n📊 Player Statistics Restored:");
                    foreach (var p in _players)
                    {
                        Console.WriteLine($"\n🎮 {p.PlayerName} ({p.Character.Name}):");
                        Console.WriteLine($"  Kills: {p.KillCount} | Damage: {p.TotalDamageDealt} | Gold: {p.GoldEarned}g");
                        Console.WriteLine($"  Chests: {p.ChestsOpened} | Duels: {p.DuelsWon}W-{p.DuelsLost}L");
                    }

                    // Show unclaimed characters
                    var unclaimed = GetUnclaimedCharacters(party);
                    if (unclaimed.Count > 0)
                    {
                        Console.WriteLine($"\n⚠️ Unclaimed Characters ({unclaimed.Count}):");
                        foreach (var c in unclaimed)
                        {
                            Console.WriteLine($"  🔓 {c.Name} (Lv {c.Level} {c.GetType().Name})");
                        }
                        Console.WriteLine("\n💡 TIP: Missing players can join later using 'Join Existing Session'!");

                        if (allowPartial)
                        {
                            Console.WriteLine("✅ Partial session enabled - you can start playing now!");
                        }
                    }
                    else
                    {
                        Console.WriteLine("\n✅ All characters from save file are claimed!");
                    }
                }
                else
                {
                    _sessionActive = false;
                    Console.WriteLine("\n❌ No players were restored. Make sure you loaded the correct game save first!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Failed to load multiplayer session: {ex.Message}");
            }
        }

        #endregion

        #region Save Data Classes

        // Data transfer objects for multiplayer save/load
        private class MultiplayerSaveFile
        {
            public DateTime Created { get; set; }
            public int PlayerCount { get; set; }
            public List<PlayerData> Players { get; set; } = new List<PlayerData>();
        }

        private class PlayerData
        {
            public string PlayerName { get; set; } = string.Empty;
            public string CharacterName { get; set; } = string.Empty;
            public int KillCount { get; set; }
            public int TotalDamageDealt { get; set; }
            public int GoldEarned { get; set; }
            public int ChestsOpened { get; set; }
            public int DuelsWon { get; set; }
            public int DuelsLost { get; set; }
        }

        #endregion
    }

    #endregion
}
