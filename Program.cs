using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Rpg_Dungeon.Systems;

namespace Rpg_Dungeon
{
    internal static class Program
    {
        private static void Main()
        {
            try
            {
                Console.WriteLine("PROGRAM STARTING...");
                Console.WriteLine($"Version: {VersionControl.FullVersion}");
                Console.WriteLine("Please wait...");
                Thread.Sleep(500);

                try
                {
                    Console.OutputEncoding = System.Text.Encoding.UTF8;
                    Console.WriteLine("UTF-8 encoding set.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"UTF-8 failed: {ex.Message}");
                    ErrorLogger.LogWarning($"UTF-8 encoding failed: {ex.Message}", "Non-critical - continuing with default encoding");
                }

                // Check for updates in the background
                CheckForUpdatesAsync();

                Console.WriteLine("Showing title screen...");
                Thread.Sleep(500);

                TitleScreenManager.Show();
            }
            catch (Exception ex)
            {
                Console.WriteLine("\n╔══════════════════════════════════════════════════════════════════╗");
                Console.WriteLine("║                    CRITICAL ERROR                                ║");
                Console.WriteLine("╚══════════════════════════════════════════════════════════════════╝");
                Console.WriteLine($"\n❌ An unexpected error occurred: {ex.Message}");

                Console.WriteLine($"\n📋 Error Type: {ex.GetType().Name}");

                string logFilePath = ErrorLogger.LogCriticalError(ex, "Unhandled exception in Main()");

                if (!string.IsNullOrEmpty(logFilePath))
                {
                    Console.WriteLine($"\n💾 Error details saved to: {logFilePath}");
                    Console.WriteLine("\n📧 To report this error:");
                    Console.WriteLine($"   1. Navigate to: {ErrorLogger.GetLogDirectory()}");
                    Console.WriteLine("   2. Send the error log file to the developer");
                    Console.WriteLine("\n💡 You can also press 'O' to open the error log folder now.");
                }

                Console.WriteLine("\n📋 Stack Trace:");
                Console.WriteLine(ex.StackTrace);
                Console.WriteLine("\nPress 'O' to open error logs folder, or Enter to exit...");

                var key = Console.ReadKey(true);
                if (key.KeyChar == 'O' || key.KeyChar == 'o')
                {
                    ErrorLogger.OpenLogDirectory();
                    Console.WriteLine("\nOpening error logs folder...");
                    Thread.Sleep(1000);
                }

                Environment.Exit(1);
            }
        }

        /// <summary>
        /// Checks for updates asynchronously in the background
        /// </summary>
        private static async void CheckForUpdatesAsync()
        {
            try
            {
                Console.WriteLine("🔍 Checking for updates...");
                var updateInfo = await UpdateChecker.CheckForUpdatesAsync();

                if (updateInfo == null)
                {
                    Console.WriteLine("⚠️  Unable to check for updates (no releases published yet or network issue)");
                }
                else if (UpdateChecker.IsNewerVersion(updateInfo))
                {
                    Console.WriteLine("╔══════════════════════════════════════════════════════════════════╗");
                    Console.WriteLine("║               🎉 NEW UPDATE AVAILABLE! 🎉                       ║");
                    Console.WriteLine("╚══════════════════════════════════════════════════════════════════╝");
                    Console.WriteLine($"📦 Latest Version: v{updateInfo.MajorVersion}.{updateInfo.MinorVersion}.{updateInfo.PatchVersion}");
                    Console.WriteLine($"📌 Your Version: v{VersionControl.FullVersion}");
                    Console.WriteLine($"💡 Visit {VersionControl.GitHubReleaseUrl} to download!");
                    Console.WriteLine();
                    Thread.Sleep(2000); // Give user time to see the message
                }
                else
                {
                    Console.WriteLine("✅ No updates available - You're running the latest version!");
                }
            }
            catch (Exception ex)
            {
                // Silently log update check failures - non-critical
                ErrorLogger.LogWarning($"Update check failed: {ex.Message}", "Non-critical - continuing without update check");
                Console.WriteLine("⚠️  Update check failed (continuing anyway)");
            }
        }
    }
}
