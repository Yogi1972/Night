using System;
using System.Threading;

namespace Rpg_Dungeon
{
    /// <summary>
    /// Provides visual enhancements for the game including colors, progress bars, and ASCII art
    /// </summary>
    internal static class VisualEffects
    {
        #region Color Methods

        public static void WriteColored(string text, ConsoleColor color)
        {
            var original = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.Write(text);
            Console.ForegroundColor = original;
        }

        public static void WriteLineColored(string text, ConsoleColor color)
        {
            var original = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(text);
            Console.ForegroundColor = original;
        }

        public static void WriteDamage(string text)
        {
            WriteColored(text, ConsoleColor.Red);
        }

        public static void WriteHealing(string text)
        {
            WriteColored(text, ConsoleColor.Green);
        }

        public static void WriteCritical(string text)
        {
            WriteColored(text, ConsoleColor.Yellow);
        }

        public static void WriteMagic(string text)
        {
            WriteColored(text, ConsoleColor.Cyan);
        }

        public static void WriteSuccess(string text)
        {
            WriteColored(text, ConsoleColor.Green);
        }

        public static void WriteDanger(string text)
        {
            WriteColored(text, ConsoleColor.Red);
        }

        public static void WriteInfo(string text)
        {
            WriteColored(text, ConsoleColor.Gray);
        }

        public static void WriteLegendary(string text)
        {
            WriteColored(text, ConsoleColor.Magenta);
        }

        #endregion

        #region Progress Bar

        public static void DrawProgressBar(int current, int max, int barWidth = 20, string label = "")
        {
            if (max <= 0) max = 1;
            double percentage = (double)current / max;
            int filled = (int)(percentage * barWidth);

            if (!string.IsNullOrEmpty(label))
            {
                Console.Write($"{label}: ");
            }

            Console.Write("[");

            var originalColor = Console.ForegroundColor;

            // Color based on percentage
            if (percentage >= 0.7) Console.ForegroundColor = ConsoleColor.Green;
            else if (percentage >= 0.4) Console.ForegroundColor = ConsoleColor.Yellow;
            else Console.ForegroundColor = ConsoleColor.Red;

            for (int i = 0; i < barWidth; i++)
            {
                Console.Write(i < filled ? "█" : "░");
            }

            Console.ForegroundColor = originalColor;
            Console.Write($"] {percentage * 100:F0}% ({current}/{max})");
        }

        public static void DrawProgressBarLine(int current, int max, int barWidth = 20, string label = "")
        {
            DrawProgressBar(current, max, barWidth, label);
            Console.WriteLine();
        }

        #endregion

        #region ASCII Art

        public static void ShowLevelUpAnimation()
        {
            var originalColor = Console.ForegroundColor;

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("╔═══════════════════════════════════════════════════════════╗");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("║                                                           ║");
            Console.WriteLine("║              ★ ✦ ★  LEVEL UP!  ★ ✦ ★                    ║");
            Console.WriteLine("║                                                           ║");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("╚═══════════════════════════════════════════════════════════╝");
            Console.ForegroundColor = originalColor;
            Thread.Sleep(800);
        }

        public static void ShowVictoryBanner()
        {
            var originalColor = Console.ForegroundColor;
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("╔═══════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                                                           ║");
            Console.WriteLine("║              ⚔️  ⚔️  VICTORY! ⚔️  ⚔️                      ║");
            Console.WriteLine("║                                                           ║");
            Console.WriteLine("╚═══════════════════════════════════════════════════════════╝");
            Console.ForegroundColor = originalColor;
            Console.WriteLine();
        }

        public static void ShowDefeatBanner()
        {
            var originalColor = Console.ForegroundColor;
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("╔═══════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                                                           ║");
            Console.WriteLine("║                 💀  DEFEAT  💀                            ║");
            Console.WriteLine("║                                                           ║");
            Console.WriteLine("╚═══════════════════════════════════════════════════════════╝");
            Console.ForegroundColor = originalColor;
            Console.WriteLine();
        }

        public static void ShowCriticalHitEffect()
        {
            var originalColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(" ⚡ CRITICAL HIT! ⚡ ");
            Console.ForegroundColor = originalColor;
        }

        public static void ShowLegendaryItemFound()
        {
            var originalColor = Console.ForegroundColor;
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("╔═══════════════════════════════════════════════════════════╗");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("║                                                           ║");
            Console.WriteLine("║        ✨ ★ ✨ LEGENDARY ITEM FOUND! ✨ ★ ✨            ║");
            Console.WriteLine("║                                                           ║");
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("╚═══════════════════════════════════════════════════════════╝");
            Console.ForegroundColor = originalColor;
            Thread.Sleep(1000);
        }

        public static void ShowMilestoneReward(int level)
        {
            var originalColor = Console.ForegroundColor;
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("╔═══════════════════════════════════════════════════════════╗");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("║                                                           ║");
            Console.WriteLine($"║         🏆  MILESTONE LEVEL {level,2} REACHED!  🏆            ║");
            Console.WriteLine("║                                                           ║");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("╚═══════════════════════════════════════════════════════════╝");
            Console.ForegroundColor = originalColor;
        }

        public static void ShowBossEncounterIntro(string bossName)
        {
            var originalColor = Console.ForegroundColor;
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("╔═══════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                                                           ║");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"║              ☠️  {bossName.ToUpper().PadRight(30)} ☠️               ║");
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("║                                                           ║");
            Console.WriteLine("╚═══════════════════════════════════════════════════════════╝");
            Console.ForegroundColor = originalColor;
            Thread.Sleep(1500);
        }

        public static void ShowChestAnimation()
        {
            var originalColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\n     ___");
            Console.WriteLine("    /   \\");
            Console.WriteLine("   |  💎 |");
            Console.WriteLine("   |_____|");
            Console.WriteLine("  📦 TREASURE! 📦");
            Console.ForegroundColor = originalColor;
            Thread.Sleep(500);
        }

        #endregion

        #region Animation Effects

        public static void TypewriterEffect(string text, int delayMs = 30)
        {
            foreach (char c in text)
            {
                Console.Write(c);
                Thread.Sleep(delayMs);
            }
            Console.WriteLine();
        }

        public static void FlashText(string text, ConsoleColor color, int times = 3)
        {
            var originalColor = Console.ForegroundColor;
            for (int i = 0; i < times; i++)
            {
                Console.ForegroundColor = color;
                Console.Write($"\r{text}");
                Thread.Sleep(200);
                Console.ForegroundColor = ConsoleColor.Black;
                Console.Write($"\r{new string(' ', text.Length)}");
                Thread.Sleep(200);
            }
            Console.ForegroundColor = color;
            Console.WriteLine($"\r{text}");
            Console.ForegroundColor = originalColor;
        }

        public static void PulseEffect(string text, ConsoleColor[] colors, int pulses = 2)
        {
            for (int i = 0; i < pulses; i++)
            {
                foreach (var color in colors)
                {
                    Console.ForegroundColor = color;
                    Console.Write($"\r{text}");
                    Thread.Sleep(150);
                }
            }
            Console.WriteLine();
            Console.ResetColor();
        }

        #endregion

        #region Combat Flavor Text

        private static readonly string[] _criticalHitMessages = new[]
        {
            "A devastating blow!",
            "CRUSHING STRIKE!",
            "Perfect hit!",
            "Flawless execution!",
            "BRUTAL IMPACT!",
            "A masterful strike!",
            "Legendary precision!"
        };

        private static readonly string[] _missMessages = new[]
        {
            "The attack goes wide!",
            "A complete whiff!",
            "Missed by a mile!",
            "The target sidesteps easily!",
            "Not even close!",
            "The attack fails to connect!"
        };

        private static readonly string[] _killMessages = new[]
        {
            "Vanquished!",
            "ELIMINATED!",
            "Defeated!",
            "DESTROYED!",
            "Obliterated!",
            "SLAIN!"
        };

        public static string GetRandomCritMessage()
        {
            var rng = new Random();
            return _criticalHitMessages[rng.Next(_criticalHitMessages.Length)];
        }

        public static string GetRandomMissMessage()
        {
            var rng = new Random();
            return _missMessages[rng.Next(_missMessages.Length)];
        }

        public static string GetRandomKillMessage()
        {
            var rng = new Random();
            return _killMessages[rng.Next(_killMessages.Length)];
        }

        #endregion
    }
}
