using System;
using System.Threading;

namespace Night.Systems
{
    /// <summary>
    /// DIAGNOSTIC TEST - Use this to verify the program runs
    /// Replace Main() with this temporarily to test
    /// </summary>
    internal static class DiagnosticTest
    {
        public static void TestMain()
        {
            // Test 1: Basic output
            Console.WriteLine("TEST 1: Basic output works!");
            Thread.Sleep(500);

            // Test 2: UTF-8 encoding
            try
            {
                Console.OutputEncoding = System.Text.Encoding.UTF8;
                Console.WriteLine("TEST 2: UTF-8 encoding set successfully!");
                Console.WriteLine("Testing emoji: ⚔️ 🗡️ 🏰");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"TEST 2 FAILED: UTF-8 encoding error: {ex.Message}");
            }
            Thread.Sleep(500);

            // Test 3: Console.Clear()
            try
            {
                Console.Clear();
                Console.WriteLine("TEST 3: Console.Clear() works!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"TEST 3 FAILED: Console.Clear() error: {ex.Message}");
            }
            Thread.Sleep(500);

            // Test 4: Box-drawing characters
            Console.WriteLine("TEST 4: Box-drawing characters:");
            Console.WriteLine("╔══════════════════════════════════════╗");
            Console.WriteLine("║  RPG DUNGEON CRAWLER                 ║");
            Console.WriteLine("╚══════════════════════════════════════╝");
            Thread.Sleep(500);

            // Test 5: ReadLine
            Console.WriteLine("\nTEST 5: Input test");
            Console.Write("Type something and press Enter: ");
            var input = Console.ReadLine();
            Console.WriteLine($"You typed: {input}");
            Thread.Sleep(500);

            // Test 6: Menu loop
            Console.WriteLine("\nTEST 6: Menu loop test");
            int attempts = 0;
            while (attempts < 3)
            {
                Console.WriteLine($"\nAttempt {attempts + 1}/3");
                Console.WriteLine("1) Option 1");
                Console.WriteLine("0) Exit test");
                Console.Write("Choose: ");
                var choice = Console.ReadLine() ?? string.Empty;

                if (choice.Trim() == "0")
                {
                    Console.WriteLine("Exiting test...");
                    break;
                }
                else if (choice.Trim() == "1")
                {
                    Console.WriteLine("Option 1 selected!");
                }
                else
                {
                    Console.WriteLine("Invalid selection.");
                }

                attempts++;
            }

            Console.WriteLine("\n✅ ALL TESTS COMPLETED!");
            Console.WriteLine("Press Enter to exit...");
            Console.ReadLine();
        }
    }
}
