using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Rpg_Dungeon
{
    /// <summary>
    /// Handles sending error reports via email to the developer
    /// Email address is masked for security
    /// </summary>
    internal static class EmailReporter
    {
        // Obfuscated email - decoded at runtime
        private static readonly byte[] ObfuscatedEmail = Convert.FromBase64String("dmVudHJ1ZTA3QGdtYWlsLmNvbQ==");
        private static readonly string DeveloperEmail = Encoding.UTF8.GetString(ObfuscatedEmail);
        private static readonly string MaskedEmail = "v*****7@g***l.com";

        /// <summary>
        /// Sends an error report via email using user's default email client
        /// </summary>
        public static void SendErrorReportViaEmailClient(string reportPath)
        {
            try
            {
                if (string.IsNullOrEmpty(reportPath) || !File.Exists(reportPath))
                {
                    Console.WriteLine("❌ Error report file not found!");
                    return;
                }

                Console.WriteLine("\n📧 Preparing email...");
                Console.WriteLine($"   To: Developer ({MaskedEmail})");
                Console.WriteLine($"   Attachment: {Path.GetFileName(reportPath)}");
                Console.WriteLine();

                var reportContent = File.ReadAllText(reportPath, Encoding.UTF8);
                var fileName = Path.GetFileName(reportPath);

                // Create mailto link with pre-filled content
                var subject = Uri.EscapeDataString($"RPG Dungeon Crawler - Error Report - {DateTime.Now:yyyy-MM-dd}");
                var body = Uri.EscapeDataString(
                    $"Hi Developer,\n\n" +
                    $"I encountered an error while playing RPG Dungeon Crawler.\n\n" +
                    $"Error Report File: {fileName}\n" +
                    $"Date: {DateTime.Now:yyyy-MM-dd HH:mm:ss}\n\n" +
                    $"Please see the attached error report file for full details.\n" +
                    $"The file is located at:\n{reportPath}\n\n" +
                    $"Description of what I was doing:\n" +
                    $"[Please describe what you were doing when the error occurred]\n\n" +
                    $"--- ERROR REPORT CONTENT (First 500 chars) ---\n" +
                    $"{(reportContent.Length > 500 ? reportContent.Substring(0, 500) + "..." : reportContent)}\n\n" +
                    $"Thank you!"
                );

                var mailtoUrl = $"mailto:{DeveloperEmail}?subject={subject}&body={body}";

                // Try to open default email client
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = mailtoUrl,
                    UseShellExecute = true
                });

                Console.WriteLine("✅ Email client opened!");
                Console.WriteLine();
                Console.WriteLine("📝 IMPORTANT:");
                Console.WriteLine($"   1. Your email client should have opened with a pre-filled message");
                Console.WriteLine($"   2. Please ATTACH the error report file manually:");
                Console.WriteLine($"      {reportPath}");
                Console.WriteLine($"   3. Add a description of what you were doing when the error occurred");
                Console.WriteLine($"   4. Click Send in your email client");
                Console.WriteLine();
                Console.WriteLine("💡 TIP: You can copy the file path above and paste it into your email");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Failed to open email client: {ex.Message}");
                Console.WriteLine();
                Console.WriteLine("📧 MANUAL EMAIL INSTRUCTIONS:");
                Console.WriteLine($"   1. Open your email client manually");
                Console.WriteLine($"   2. Send email to: {MaskedEmail}");
                Console.WriteLine($"   3. Subject: RPG Dungeon Crawler - Error Report");
                Console.WriteLine($"   4. Attach: {reportPath}");
                Console.WriteLine($"   5. Describe what you were doing when the error occurred");
            }
        }

        /// <summary>
        /// Shows quick email instructions
        /// </summary>
        public static void ShowEmailInstructions()
        {
            Console.WriteLine("\n╔════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                 HOW TO SEND ERROR REPORT                       ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════════╝");
            Console.WriteLine();
            Console.WriteLine("📧 Send error reports to the developer:");
            Console.WriteLine($"   Email: {MaskedEmail}");
            Console.WriteLine();
            Console.WriteLine("📋 What to include:");
            Console.WriteLine("   1. The ERROR_REPORT_*.txt file (from ErrorLogs folder)");
            Console.WriteLine("   2. Description of what you were doing");
            Console.WriteLine("   3. Steps to reproduce the error (if known)");
            Console.WriteLine("   4. Your save file (if the error is related to save/load)");
            Console.WriteLine();
            Console.WriteLine("💡 TIP: Use option 3 in the Error Logs menu to create a report,");
            Console.WriteLine("   then use option 6 to send it automatically!");
            Console.WriteLine();
        }

        /// <summary>
        /// Creates an error report and prepares it for emailing
        /// </summary>
        public static void CreateAndPrepareEmailReport()
        {
            Console.WriteLine("\n📧 Creating error report for email...");
            var reportPath = ErrorLogger.CreateErrorReport();

            if (string.IsNullOrEmpty(reportPath))
            {
                Console.WriteLine("❌ Failed to create error report.");
                return;
            }

            Console.WriteLine($"✅ Report created: {Path.GetFileName(reportPath)}");
            Console.WriteLine();

            SendErrorReportViaEmailClient(reportPath);
        }

        /// <summary>
        /// Gets the masked email address for display
        /// </summary>
        public static string GetMaskedEmail()
        {
            return MaskedEmail;
        }

        /// <summary>
        /// Creates a test error log and sends it
        /// </summary>
        public static void SendTestErrorReport()
        {
            Console.WriteLine("\n🧪 CREATING TEST ERROR REPORT");
            Console.WriteLine("═══════════════════════════════════════════════════════════════════");
            Console.WriteLine();

            try
            {
                // Create a test exception
                Console.WriteLine("📝 Generating test error...");
                var testException = new Exception(
                    "This is a TEST error report generated for diagnostic purposes. " +
                    "No actual error occurred. This is used to verify the error reporting system works correctly."
                );

                // Add a fake stack trace for realism
                try
                {
                    throw testException;
                }
                catch (Exception ex)
                {
                    // Log the test error
                    Console.WriteLine("💾 Logging test error...");
                    ErrorLogger.LogError(ex, "TEST ERROR - Generated via Error Logs Menu → Send Test Report. This is for diagnostic purposes only.");
                }

                // Create the full report
                Console.WriteLine("📋 Creating comprehensive error report...");
                var reportPath = ErrorLogger.CreateErrorReport();

                if (string.IsNullOrEmpty(reportPath))
                {
                    Console.WriteLine("❌ Failed to create test report.");
                    return;
                }

                Console.WriteLine($"✅ Test report created successfully!");
                Console.WriteLine($"   File: {Path.GetFileName(reportPath)}");
                Console.WriteLine($"   Location: {reportPath}");
                Console.WriteLine();

                Console.WriteLine("📧 Would you like to send this test report now?");
                Console.WriteLine("   (This will open your email client)");
                Console.WriteLine();
                Console.WriteLine("  Y) Yes, open email client to send");
                Console.WriteLine("  N) No, I'll send it manually later");
                Console.Write("\nYour choice (Y/N): ");

                var choice = Console.ReadKey(true);
                Console.WriteLine();

                if (choice.KeyChar == 'Y' || choice.KeyChar == 'y')
                {
                    Console.WriteLine();
                    SendErrorReportViaEmailClient(reportPath);
                }
                else
                {
                    Console.WriteLine();
                    Console.WriteLine("📁 Test report saved. You can send it later from the ErrorLogs folder.");
                    Console.WriteLine($"   Location: {ErrorLogger.GetLogDirectory()}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n❌ Error creating test report: {ex.Message}");
                ErrorLogger.LogError(ex, "Failed to create test error report");
            }
        }
    }
}
