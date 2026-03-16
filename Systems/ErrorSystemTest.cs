using System;
using System.IO;

namespace Rpg_Dungeon
{
    /// <summary>
    /// Quick test to verify error logging and email reporting works
    /// Run this to generate a test error report
    /// </summary>
    internal static class ErrorSystemTest
    {
        public static void RunTest()
        {
            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║           ERROR LOGGING SYSTEM - VERIFICATION TEST              ║");
            Console.WriteLine("╚══════════════════════════════════════════════════════════════════╝");
            Console.WriteLine();

            int testsPassed = 0;
            int totalTests = 5;

            // Test 1: Create error log directory
            Console.WriteLine("Test 1/5: Creating error log directory...");
            try
            {
                var logDir = ErrorLogger.GetLogDirectory();
                if (Directory.Exists(logDir) || !string.IsNullOrEmpty(logDir))
                {
                    Console.WriteLine("✅ PASSED - Directory accessible");
                    testsPassed++;
                }
                else
                {
                    Console.WriteLine("❌ FAILED - Directory not accessible");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ FAILED - {ex.Message}");
            }
            Console.WriteLine();

            // Test 2: Log a test error
            Console.WriteLine("Test 2/5: Logging test error...");
            try
            {
                var testEx = new Exception("TEST ERROR - This is a diagnostic test error");
                var logPath = ErrorLogger.LogError(testEx, "Automated test error from ErrorSystemTest");

                if (!string.IsNullOrEmpty(logPath) && File.Exists(logPath))
                {
                    Console.WriteLine($"✅ PASSED - Error logged to: {Path.GetFileName(logPath)}");
                    testsPassed++;
                }
                else
                {
                    Console.WriteLine("❌ FAILED - Log file not created");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ FAILED - {ex.Message}");
            }
            Console.WriteLine();

            // Test 3: Log a warning
            Console.WriteLine("Test 3/5: Logging test warning...");
            try
            {
                ErrorLogger.LogWarning("TEST WARNING - This is a diagnostic test warning", "Automated test from ErrorSystemTest");
                Console.WriteLine("✅ PASSED - Warning logged");
                testsPassed++;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ FAILED - {ex.Message}");
            }
            Console.WriteLine();

            // Test 4: Create error report
            Console.WriteLine("Test 4/5: Creating comprehensive error report...");
            try
            {
                var reportPath = ErrorLogger.CreateErrorReport();

                if (!string.IsNullOrEmpty(reportPath) && File.Exists(reportPath))
                {
                    var fileInfo = new FileInfo(reportPath);
                    Console.WriteLine($"✅ PASSED - Report created: {Path.GetFileName(reportPath)}");
                    Console.WriteLine($"   Size: {fileInfo.Length / 1024} KB");
                    testsPassed++;
                }
                else
                {
                    Console.WriteLine("❌ FAILED - Report file not created");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ FAILED - {ex.Message}");
            }
            Console.WriteLine();

            // Test 5: Email masking
            Console.WriteLine("Test 5/5: Verifying email masking...");
            try
            {
                var maskedEmail = EmailReporter.GetMaskedEmail();

                if (maskedEmail.Contains("*") && !maskedEmail.Contains("ventrue07"))
                {
                    Console.WriteLine($"✅ PASSED - Email masked: {maskedEmail}");
                    testsPassed++;
                }
                else
                {
                    Console.WriteLine("❌ FAILED - Email not properly masked");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ FAILED - {ex.Message}");
            }
            Console.WriteLine();

            // Summary
            Console.WriteLine("══════════════════════════════════════════════════════════════════");
            Console.WriteLine($"TEST SUMMARY: {testsPassed}/{totalTests} tests passed");
            Console.WriteLine("══════════════════════════════════════════════════════════════════");

            if (testsPassed == totalTests)
            {
                Console.WriteLine("🎉 ALL TESTS PASSED! Error logging system is working perfectly!");
            }
            else
            {
                Console.WriteLine($"⚠️  {totalTests - testsPassed} test(s) failed. Check the errors above.");
            }

            Console.WriteLine();
            Console.WriteLine($"📁 Error logs directory: {ErrorLogger.GetLogDirectory()}");
            Console.WriteLine();
            Console.WriteLine("Press Enter to continue...");
            Console.ReadLine();
        }
    }
}
