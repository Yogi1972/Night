using Night.Characters;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Rpg_Dungeon
{
    /// <summary>
    /// Provides an exciting opening sequence to hook players immediately
    /// </summary>
    internal static class OpeningHook
    {
        public static void ShowOpeningSequence()
        {
            Console.Clear();

            var originalColor = Console.ForegroundColor;

            // Dramatic opening
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("\n\n");
            VisualEffects.TypewriterEffect("The ground trembles beneath your feet...", 40);
            Thread.Sleep(800);

            Console.ForegroundColor = ConsoleColor.Red;
            VisualEffects.TypewriterEffect("A monstrous roar echoes through the darkness...", 40);
            Thread.Sleep(800);

            Console.ForegroundColor = ConsoleColor.Yellow;
            VisualEffects.TypewriterEffect("This is your chance to prove yourself...", 40);
            Thread.Sleep(800);

            Console.ForegroundColor = originalColor;
            Console.WriteLine("\n");

            // Show dramatic enemy appearance
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("╔═══════════════════════════════════════════════════════════╗");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("║                                                           ║");
            Console.WriteLine("║              A GOBLIN RAIDER ATTACKS!                     ║");
            Console.WriteLine("║                                                           ║");
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("╚═══════════════════════════════════════════════════════════╝");
            Console.ForegroundColor = originalColor;

            Thread.Sleep(1000);

            Console.WriteLine("\nPress any key to defend yourself!");
            Console.ReadKey(true);

            // Quick tutorial combat
            ShowTutorialCombat();
        }

        private static void ShowTutorialCombat()
        {
            Console.Clear();

            VisualEffects.WriteLineColored("\n╔═══════════════════════════════════════════════════════════╗", ConsoleColor.Cyan);
            VisualEffects.WriteLineColored("║                  COMBAT TUTORIAL                          ║", ConsoleColor.Cyan);
            VisualEffects.WriteLineColored("╚═══════════════════════════════════════════════════════════╝\n", ConsoleColor.Cyan);

            Console.WriteLine("🎯 Combat Basics:");
            Console.WriteLine("  • Roll d20 to attack (higher = better)");
            Console.WriteLine("  • Critical Hit (20) = Double damage! ⚡");
            Console.WriteLine("  • Critical Fail (1) = Complete miss! 💨");
            Console.WriteLine("  • Use abilities strategically");
            Console.WriteLine();

            var rng = new Random();

            // Enhanced tutorial enemy
            string enemyName = "Goblin Raider";
            int enemyMaxHp = 30;
            int enemyHp = enemyMaxHp;
            int enemyAttack = 4;
            int playerMaxHp = 50;
            int playerHp = playerMaxHp;
            int playerAttack = 6;
            bool enemyIsStunned = false;

            // Combat loop
            int turnCount = 0;
            while (enemyHp > 0 && playerHp > 0)
            {
                turnCount++;
                Console.WriteLine($"\n═══ Turn {turnCount} ═══");

                // Show status
                VisualEffects.WriteLineColored($"⚔️  {enemyName}", ConsoleColor.Red);
                Console.WriteLine($"    HP: [{new string('█', (int)((double)enemyHp / enemyMaxHp * 20))}{new string('░', 20 - (int)((double)enemyHp / enemyMaxHp * 20))}] {enemyHp}/{enemyMaxHp}");
                if (enemyIsStunned) VisualEffects.WriteColored("    💫 STUNNED!\n", ConsoleColor.Yellow);

                Console.WriteLine();
                VisualEffects.WriteLineColored("💚 Your Stats", ConsoleColor.Green);
                Console.WriteLine($"    HP: [{new string('█', (int)((double)playerHp / playerMaxHp * 20))}{new string('░', 20 - (int)((double)playerHp / playerMaxHp * 20))}] {playerHp}/{playerMaxHp}");
                Console.WriteLine();

                // Player turn
                Console.WriteLine("Choose your action:");
                Console.WriteLine("  [1] ⚔️  Attack");
                Console.WriteLine("  [2] 💥 Power Strike (costs 5 HP, +5 damage)");
                Console.WriteLine("  [3] 🛡️  Defensive Stance (reduce next damage by 50%)");
                if (turnCount > 1) Console.WriteLine("  [4] 💫 Stun Attack (50% chance to skip enemy turn)");

                Console.Write("\nYour choice: ");
                string choice = Console.ReadLine()?.Trim() ?? "1";

                int damage = 0;
                bool isDefending = false;

                Console.WriteLine();

                switch (choice)
                {
                    case "1": // Normal attack
                        int roll = rng.Next(1, 21);
                        Console.WriteLine($"🎲 You roll d20: {roll}");
                        Thread.Sleep(400);

                        if (roll == 1)
                        {
                            VisualEffects.WriteLineColored("💨 CRITICAL MISS! Your attack goes wide!", ConsoleColor.DarkGray);
                        }
                        else if (roll == 20)
                        {
                            VisualEffects.ShowCriticalHitEffect();
                            damage = (playerAttack + roll) * 2;
                            VisualEffects.WriteDamage($"💥 CRITICAL HIT! You deal {damage} damage!\n");
                        }
                        else
                        {
                            damage = playerAttack + (roll / 2);
                            VisualEffects.WriteDamage($"⚔️  You strike for {damage} damage!\n");
                        }
                        break;

                    case "2": // Power Strike
                        if (playerHp > 5)
                        {
                            playerHp -= 5;
                            int powerRoll = rng.Next(1, 21);
                            damage = playerAttack + powerRoll + 5;
                            VisualEffects.WriteColored($"❤️  You sacrifice 5 HP for power!\n", ConsoleColor.Yellow);
                            Thread.Sleep(300);
                            VisualEffects.WriteDamage($"💥 POWER STRIKE! You deal {damage} damage!\n");
                        }
                        else
                        {
                            VisualEffects.WriteColored("⚠️  Not enough HP! Performing normal attack...\n", ConsoleColor.Yellow);
                            damage = playerAttack + rng.Next(5, 15);
                            VisualEffects.WriteDamage($"⚔️  You deal {damage} damage!\n");
                        }
                        break;

                    case "3": // Defensive Stance
                        isDefending = true;
                        VisualEffects.WriteSuccess("🛡️  You brace for impact! Next damage reduced by 50%\n");
                        break;

                    case "4": // Stun Attack
                        if (turnCount > 1)
                        {
                            if (rng.Next(1, 101) <= 50)
                            {
                                enemyIsStunned = true;
                                damage = playerAttack + rng.Next(1, 10);
                                VisualEffects.WriteSuccess($"💫 STUN SUCCESS! Enemy stunned and takes {damage} damage!\n");
                            }
                            else
                            {
                                damage = playerAttack + rng.Next(1, 8);
                                VisualEffects.WriteColored($"💫 Stun failed, but you deal {damage} damage!\n", ConsoleColor.Yellow);
                            }
                        }
                        break;

                    default:
                        damage = playerAttack + rng.Next(5, 12);
                        VisualEffects.WriteDamage($"⚔️  You strike for {damage} damage!\n");
                        break;
                }

                enemyHp = Math.Max(0, enemyHp - damage);
                Thread.Sleep(500);

                if (enemyHp <= 0)
                {
                    break;
                }

                VisualEffects.WriteInfo($"Enemy HP: {enemyHp}/{enemyMaxHp}\n");
                Thread.Sleep(600);

                // Enemy turn
                if (enemyIsStunned)
                {
                    VisualEffects.WriteColored($"💫 {enemyName} is stunned and can't attack!\n", ConsoleColor.Yellow);
                    enemyIsStunned = false;
                    Thread.Sleep(800);
                }
                else
                {
                    Console.WriteLine($"\n🔴 {enemyName} attacks!");
                    Thread.Sleep(400);

                    int enemyRoll = rng.Next(1, 21);
                    Console.WriteLine($"🎲 Enemy rolls: {enemyRoll}");
                    Thread.Sleep(300);

                    if (enemyRoll == 1)
                    {
                        VisualEffects.WriteSuccess("✨ The enemy misses completely!\n");
                    }
                    else
                    {
                        int enemyDamage = enemyAttack + (enemyRoll / 3);

                        if (isDefending)
                        {
                            enemyDamage = enemyDamage / 2;
                            VisualEffects.WriteInfo($"🛡️  Blocked! Reduced to {enemyDamage} damage!\n");
                        }

                        if (enemyRoll == 20)
                        {
                            enemyDamage *= 2;
                            VisualEffects.WriteLineColored("💥 ENEMY CRITICAL HIT!", ConsoleColor.Red);
                        }

                        playerHp = Math.Max(0, playerHp - enemyDamage);
                        VisualEffects.WriteDamage($"💔 You take {enemyDamage} damage!\n");
                        Thread.Sleep(400);
                        VisualEffects.WriteInfo($"Your HP: {playerHp}/{playerMaxHp}\n");
                    }
                }

                Thread.Sleep(800);
                Console.WriteLine("\nPress Enter to continue...");
                Console.ReadLine();
                Console.Clear();
            }

            // Victory!
            Console.WriteLine();
            VisualEffects.ShowVictoryBanner();
            VisualEffects.WriteSuccess($"💀 {enemyName} was {VisualEffects.GetRandomKillMessage()}\n");

            Console.WriteLine();
            VisualEffects.WriteSuccess("💰 You found 10 gold!\n");
            VisualEffects.WriteSuccess("⭐ You gained 25 XP!\n");
            VisualEffects.WriteSuccess($"❤️  Restored to full health! ({playerMaxHp}/{playerMaxHp} HP)\n");

            Thread.Sleep(1500);

            Console.WriteLine();
            VisualEffects.WriteLineColored("╔═══════════════════════════════════════════════════════════╗", ConsoleColor.Green);
            VisualEffects.WriteLineColored("║            TUTORIAL COMPLETE!                             ║", ConsoleColor.Green);
            VisualEffects.WriteLineColored("╚═══════════════════════════════════════════════════════════╝", ConsoleColor.Green);
            Console.WriteLine();

            Console.WriteLine("🎮 What awaits you in the full game:");
            Console.WriteLine();
            Console.WriteLine("  🧙 Create custom characters with unique races & classes");
            Console.WriteLine("  🗺️  Explore vast dungeons and mystical towns");
            Console.WriteLine("  📜 Complete epic quests and earn achievements");
            Console.WriteLine("  ⚔️  Master combat with stances, combos & abilities");
            Console.WriteLine("  🎒 Collect legendary equipment and rare artifacts");
            Console.WriteLine("  ⭐ Unlock champion classes and prestige levels");
            Console.WriteLine("  👥 Challenge other players in multiplayer mode");
            Console.WriteLine();

            Console.WriteLine("Press any key to begin your adventure...");
            Console.ReadKey(true);
        }

        public static bool ShouldShowOpening()
        {
            // Show opening hook for first-time players (can be expanded with save file check)
            Console.WriteLine("\n╔═══════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                                                           ║");
            Console.WriteLine("║     Would you like to see the opening sequence?          ║");
            Console.WriteLine("║     (Recommended for first-time players)                  ║");
            Console.WriteLine("║                                                           ║");
            Console.WriteLine("╚═══════════════════════════════════════════════════════════╝");
            Console.WriteLine();
            Console.Write("Show opening? (y/n): ");

            var response = Console.ReadLine()?.Trim().ToLower();
            return response == "y" || response == "yes";
        }
    }
}
