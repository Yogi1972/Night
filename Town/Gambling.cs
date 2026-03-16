using Night.Characters;
using System;
using System.Collections.Generic;

namespace Rpg_Dungeon
{
    #region Gambling Class

    internal class Gambling
    {
        #region Fields

        private readonly Random _random;

        #endregion

        #region Constructor

        public Gambling()
        {
            _random = new Random();
        }

        #endregion

        #region Main Menu

        public void OpenGamblingDen(List<Character> party)
        {
            if (party == null || party.Count == 0) return;

            while (true)
            {
                Console.WriteLine("\n╔════════════════════════════════════════╗");
                Console.WriteLine("║      The Lucky Dragon (Gambling)     ║");
                Console.WriteLine("╚════════════════════════════════════════╝");
                Console.WriteLine("'Try your luck! Fortunes can be won... or lost!'");
                Console.WriteLine("\n1) Dice Roll (10g minimum bet)");
                Console.WriteLine("2) High/Low Card (25g minimum bet)");
                Console.WriteLine("3) Coin Flip (Any bet)");
                Console.WriteLine("4) Slots (50g fixed bet)");
                Console.WriteLine("0) Leave");
                Console.Write("Choice: ");

                var choice = Console.ReadLine() ?? string.Empty;

                switch (choice.Trim())
                {
                    case "1":
                        PlayDiceRoll(party);
                        break;
                    case "2":
                        PlayHighLowCard(party);
                        break;
                    case "3":
                        PlayCoinFlip(party);
                        break;
                    case "4":
                        PlaySlots(party);
                        break;
                    case "0":
                        Console.WriteLine("'Come back anytime!'");
                        return;
                    default:
                        Console.WriteLine("Invalid choice.");
                        break;
                }
            }
        }

        #endregion

        #region Game Methods

        private void PlayDiceRoll(List<Character> party)
        {
            Console.WriteLine("\n=== Dice Roll ===");
            Console.WriteLine("Roll 2 dice. Total of 7 or 11 wins 2x your bet!");
            Console.WriteLine("Total of 2, 3, or 12 loses your bet.");
            Console.WriteLine("Any other number wins 1.5x your bet.");

            var player = SelectPlayer(party);
            if (player == null) return;

            Console.Write("How much do you bet? (Minimum 10g): ");
            var betInput = Console.ReadLine() ?? string.Empty;
            if (!int.TryParse(betInput, out var bet) || bet < 10)
            {
                Console.WriteLine("Invalid bet!");
                return;
            }

            if (!player.Inventory.SpendGold(bet))
            {
                Console.WriteLine("Not enough gold!");
                return;
            }

            int dice1 = _random.Next(1, 7);
            int dice2 = _random.Next(1, 7);
            int total = dice1 + dice2;

            Console.WriteLine($"\n🎲 Rolling... {dice1} + {dice2} = {total}");

            if (total == 7 || total == 11)
            {
                int winnings = bet * 2;
                player.Inventory.AddGold(winnings + bet);
                Console.WriteLine($"🎉 Lucky {total}! You win {winnings} gold! (Total: {winnings + bet}g)");
            }
            else if (total == 2 || total == 3 || total == 12)
            {
                Console.WriteLine($"💀 Snake eyes! You lose {bet} gold!");
            }
            else
            {
                int winnings = (int)(bet * 1.5);
                player.Inventory.AddGold(winnings + bet);
                Console.WriteLine($"✓ You win {winnings} gold! (Total: {winnings + bet}g)");
            }
        }

        private void PlayHighLowCard(List<Character> party)
        {
            Console.WriteLine("\n=== High/Low Card ===");
            Console.WriteLine("Guess if the next card will be higher or lower than 7.");
            Console.WriteLine("Correct guess wins 1.8x your bet!");

            var player = SelectPlayer(party);
            if (player == null) return;

            Console.Write("How much do you bet? (Minimum 25g): ");
            var betInput = Console.ReadLine() ?? string.Empty;
            if (!int.TryParse(betInput, out var bet) || bet < 25)
            {
                Console.WriteLine("Invalid bet!");
                return;
            }

            if (!player.Inventory.SpendGold(bet))
            {
                Console.WriteLine("Not enough gold!");
                return;
            }

            Console.Write("Will the card be (H)igh or (L)ow? ");
            var guess = Console.ReadLine()?.ToUpper() ?? "";

            if (guess != "H" && guess != "L")
            {
                player.Inventory.AddGold(bet);
                Console.WriteLine("Invalid choice! Bet returned.");
                return;
            }

            int card = _random.Next(1, 14); // 1-13 (Ace to King)
            string cardName = card switch
            {
                1 => "Ace",
                11 => "Jack",
                12 => "Queen",
                13 => "King",
                _ => card.ToString()
            };

            Console.WriteLine($"\n🃏 The card is... {cardName}!");

            bool isHigh = card > 7;
            bool won = (guess == "H" && isHigh) || (guess == "L" && !isHigh);

            if (won)
            {
                int winnings = (int)(bet * 1.8);
                player.Inventory.AddGold(winnings + bet);
                Console.WriteLine($"🎉 Correct! You win {winnings} gold! (Total: {winnings + bet}g)");
            }
            else
            {
                Console.WriteLine($"💀 Wrong! You lose {bet} gold!");
            }
        }

        private void PlayCoinFlip(List<Character> party)
        {
            Console.WriteLine("\n=== Coin Flip ===");
            Console.WriteLine("Heads or Tails? Win 2x your bet!");

            var player = SelectPlayer(party);
            if (player == null) return;

            Console.Write("How much do you bet? ");
            var betInput = Console.ReadLine() ?? string.Empty;
            if (!int.TryParse(betInput, out var bet) || bet <= 0)
            {
                Console.WriteLine("Invalid bet!");
                return;
            }

            if (!player.Inventory.SpendGold(bet))
            {
                Console.WriteLine("Not enough gold!");
                return;
            }

            Console.Write("Call it! (H)eads or (T)ails? ");
            var guess = Console.ReadLine()?.ToUpper() ?? "";

            if (guess != "H" && guess != "T")
            {
                player.Inventory.AddGold(bet);
                Console.WriteLine("Invalid choice! Bet returned.");
                return;
            }

            bool coinIsHeads = _random.Next(2) == 0;
            string result = coinIsHeads ? "Heads" : "Tails";

            Console.WriteLine($"\n🪙 Flipping... {result}!");

            bool won = (guess == "H" && coinIsHeads) || (guess == "T" && !coinIsHeads);

            if (won)
            {
                int winnings = bet * 2;
                player.Inventory.AddGold(winnings);
                Console.WriteLine($"🎉 You win {bet} gold! (Total: {winnings}g)");
            }
            else
            {
                Console.WriteLine($"💀 You lose {bet} gold!");
            }
        }

        private void PlaySlots(List<Character> party)
        {
            Console.WriteLine("\n=== Slot Machine ===");
            Console.WriteLine("Cost: 50 gold");
            Console.WriteLine("🍒 🍒 🍒 = 500g");
            Console.WriteLine("💎 💎 💎 = 250g");
            Console.WriteLine("🔔 🔔 🔔 = 150g");
            Console.WriteLine("⭐ ⭐ ⭐ = 100g");
            Console.WriteLine("Any 2 matching = 25g");

            var player = SelectPlayer(party);
            if (player == null) return;

            if (!player.Inventory.SpendGold(50))
            {
                Console.WriteLine("Not enough gold! Need 50g.");
                return;
            }

            var symbols = new[] { "🍒", "💎", "🔔", "⭐", "7️⃣", "🍀" };
            var reel1 = symbols[_random.Next(symbols.Length)];
            var reel2 = symbols[_random.Next(symbols.Length)];
            var reel3 = symbols[_random.Next(symbols.Length)];

            Console.WriteLine("\n🎰 Spinning...");
            System.Threading.Thread.Sleep(500);
            Console.WriteLine($"  {reel1} | {reel2} | {reel3}");

            int winnings = 0;

            if (reel1 == reel2 && reel2 == reel3)
            {
                winnings = reel1 switch
                {
                    "🍒" => 500,
                    "💎" => 250,
                    "🔔" => 150,
                    "⭐" => 100,
                    "7️⃣" => 777,
                    "🍀" => 200,
                    _ => 50
                };
                Console.WriteLine($"\n💰 JACKPOT! Three {reel1}!");
            }
            else if (reel1 == reel2 || reel2 == reel3 || reel1 == reel3)
            {
                winnings = 25;
                Console.WriteLine("\n✓ Two matching symbols!");
            }
            else
            {
                Console.WriteLine("\n💀 No match. Better luck next time!");
            }

            if (winnings > 0)
            {
                player.Inventory.AddGold(winnings);
                Console.WriteLine($"You win {winnings} gold!");
            }
        }

        #endregion

        #region Helper Methods

        private Character? SelectPlayer(List<Character> party)
        {
            Console.WriteLine("\nWho's gambling?");
            for (int i = 0; i < party.Count; i++)
            {
                Console.WriteLine($"{i + 1}) {party[i].Name} - Gold: {party[i].Inventory.Gold}");
            }

            var input = Console.ReadLine() ?? string.Empty;
            if (!int.TryParse(input, out var idx) || idx < 1 || idx > party.Count)
            {
                Console.WriteLine("Invalid choice.");
                return null;
            }

            return party[idx - 1];
        }

        #endregion
    }

    #endregion
}
