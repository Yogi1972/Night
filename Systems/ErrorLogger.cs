using System;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Reflection;
using System.Linq;

namespace Rpg_Dungeon
{
    /// <summary>
    /// Comprehensive error logging system that captures detailed error information
    /// for debugging and support purposes.
    /// </summary>
    internal static class ErrorLogger
    {
        private static readonly string LogDirectory = Path.Combine(Directory.GetCurrentDirectory(), "ErrorLogs");
        private static readonly string CurrentLogFile = Path.Combine(LogDirectory, $"error_log_{DateTime.Now:yyyy-MM-dd}.txt");
        private static readonly object LockObject = new object();

        static ErrorLogger()
        {
            EnsureLogDirectoryExists();
        }

        private static void EnsureLogDirectoryExists()
        {
            try
            {
                if (!Directory.Exists(LogDirectory))
                {
                    Directory.CreateDirectory(LogDirectory);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Warning: Could not create error log directory: {ex.Message}");
            }
        }

        /// <summary>
        /// Logs a critical error with full system and application context
        /// </summary>
        public static string LogCriticalError(Exception ex, string additionalContext = "")
        {
            try
            {
                lock (LockObject)
                {
                    var logEntry = BuildErrorLogEntry(ex, "CRITICAL", additionalContext);
                    WriteToLogFile(logEntry);
                    return CurrentLogFile;
                }
            }
            catch (Exception logEx)
            {
                Console.WriteLine($"⚠️ Failed to write to error log: {logEx.Message}");
                return string.Empty;
            }
        }

        /// <summary>
        /// Logs a general error
        /// </summary>
        public static string LogError(Exception ex, string additionalContext = "")
        {
            try
            {
                lock (LockObject)
                {
                    var logEntry = BuildErrorLogEntry(ex, "ERROR", additionalContext);
                    WriteToLogFile(logEntry);
                    return CurrentLogFile;
                }
            }
            catch (Exception logEx)
            {
                Console.WriteLine($"⚠️ Failed to write to error log: {logEx.Message}");
                return string.Empty;
            }
        }

        /// <summary>
        /// Logs a warning
        /// </summary>
        public static void LogWarning(string message, string additionalContext = "")
        {
            try
            {
                lock (LockObject)
                {
                    var logEntry = BuildWarningLogEntry(message, additionalContext);
                    WriteToLogFile(logEntry);
                }
            }
            catch (Exception logEx)
            {
                Console.WriteLine($"⚠️ Failed to write to error log: {logEx.Message}");
            }
        }

        private static string BuildErrorLogEntry(Exception ex, string severity, string additionalContext)
        {
            var sb = new StringBuilder();
            sb.AppendLine("═══════════════════════════════════════════════════════════════════════════");
            sb.AppendLine($"[{severity}] ERROR REPORT - {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            sb.AppendLine("═══════════════════════════════════════════════════════════════════════════");
            sb.AppendLine();

            // Error Details
            sb.AppendLine("ERROR DETAILS:");
            sb.AppendLine($"  Type: {ex.GetType().FullName}");
            sb.AppendLine($"  Message: {ex.Message}");
            sb.AppendLine($"  Source: {ex.Source ?? "Unknown"}");
            sb.AppendLine();

            // Stack Trace
            sb.AppendLine("STACK TRACE:");
            sb.AppendLine(ex.StackTrace ?? "  No stack trace available");
            sb.AppendLine();

            // Inner Exception
            if (ex.InnerException != null)
            {
                sb.AppendLine("INNER EXCEPTION:");
                sb.AppendLine($"  Type: {ex.InnerException.GetType().FullName}");
                sb.AppendLine($"  Message: {ex.InnerException.Message}");
                sb.AppendLine($"  Stack Trace: {ex.InnerException.StackTrace}");
                sb.AppendLine();
            }

            // Additional Context
            if (!string.IsNullOrWhiteSpace(additionalContext))
            {
                sb.AppendLine("ADDITIONAL CONTEXT:");
                sb.AppendLine($"  {additionalContext}");
                sb.AppendLine();
            }

            // System Information
            sb.AppendLine("SYSTEM INFORMATION:");
            sb.AppendLine($"  OS: {Environment.OSVersion}");
            sb.AppendLine($"  .NET Version: {Environment.Version}");
            sb.AppendLine($"  64-bit OS: {Environment.Is64BitOperatingSystem}");
            sb.AppendLine($"  64-bit Process: {Environment.Is64BitProcess}");
            sb.AppendLine($"  Working Directory: {Directory.GetCurrentDirectory()}");
            sb.AppendLine();

            // Application Information
            try
            {
                var assembly = Assembly.GetExecutingAssembly();
                var version = assembly.GetName().Version;
                sb.AppendLine("APPLICATION INFORMATION:");
                sb.AppendLine($"  Name: RPG Dungeon Crawler");
                sb.AppendLine($"  Version: {version}");
                sb.AppendLine($"  Assembly: {assembly.FullName}");
                sb.AppendLine($"  Location: {assembly.Location}");
                sb.AppendLine();
            }
            catch
            {
                sb.AppendLine("APPLICATION INFORMATION: Could not retrieve");
                sb.AppendLine();
            }

            // Performance Information
            try
            {
                var currentProcess = Process.GetCurrentProcess();
                sb.AppendLine("PERFORMANCE METRICS:");
                sb.AppendLine($"  Memory Usage: {currentProcess.WorkingSet64 / 1024 / 1024} MB");
                sb.AppendLine($"  Thread Count: {currentProcess.Threads.Count}");
                sb.AppendLine($"  Uptime: {DateTime.Now - currentProcess.StartTime}");
                sb.AppendLine();
            }
            catch
            {
                sb.AppendLine("PERFORMANCE METRICS: Could not retrieve");
                sb.AppendLine();
            }

            sb.AppendLine("═══════════════════════════════════════════════════════════════════════════");
            sb.AppendLine();
            sb.AppendLine();

            return sb.ToString();
        }

        private static string BuildWarningLogEntry(string message, string additionalContext)
        {
            var sb = new StringBuilder();
            sb.AppendLine("───────────────────────────────────────────────────────────────────────────");
            sb.AppendLine($"[WARNING] {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            sb.AppendLine("───────────────────────────────────────────────────────────────────────────");
            sb.AppendLine($"  Message: {message}");

            if (!string.IsNullOrWhiteSpace(additionalContext))
            {
                sb.AppendLine($"  Context: {additionalContext}");
            }

            sb.AppendLine("───────────────────────────────────────────────────────────────────────────");
            sb.AppendLine();

            return sb.ToString();
        }

        private static void WriteToLogFile(string logEntry)
        {
            EnsureLogDirectoryExists();
            File.AppendAllText(CurrentLogFile, logEntry, Encoding.UTF8);
        }

        /// <summary>
        /// Gets the path to the current error log file
        /// </summary>
        public static string GetCurrentLogFilePath()
        {
            return CurrentLogFile;
        }

        /// <summary>
        /// Gets the path to the error logs directory
        /// </summary>
        public static string GetLogDirectory()
        {
            return LogDirectory;
        }

        /// <summary>
        /// Opens the error log directory in Windows Explorer
        /// </summary>
        public static void OpenLogDirectory()
        {
            try
            {
                if (Directory.Exists(LogDirectory))
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = LogDirectory,
                        UseShellExecute = true,
                        Verb = "open"
                    });
                }
                else
                {
                    Console.WriteLine("📁 No error logs directory found (no errors have been logged yet).");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Could not open log directory: {ex.Message}");
            }
        }

        /// <summary>
        /// Creates a comprehensive error report file that can be sent to the developer
        /// </summary>
        public static string CreateErrorReport()
        {
            try
            {
                EnsureLogDirectoryExists();
                var reportPath = Path.Combine(LogDirectory, $"ERROR_REPORT_{DateTime.Now:yyyyMMdd_HHmmss}.txt");
                var sb = new StringBuilder();

                sb.AppendLine("╔═══════════════════════════════════════════════════════════════════════════╗");
                sb.AppendLine("║                   RPG DUNGEON CRAWLER - ERROR REPORT                      ║");
                sb.AppendLine("║                    Please send this file to the developer                 ║");
                sb.AppendLine("╚═══════════════════════════════════════════════════════════════════════════╝");
                sb.AppendLine();
                sb.AppendLine($"Report Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                sb.AppendLine();

                // System Info
                sb.AppendLine("═══════════════════════════════════════════════════════════════════════════");
                sb.AppendLine("SYSTEM INFORMATION");
                sb.AppendLine("═══════════════════════════════════════════════════════════════════════════");
                sb.AppendLine($"OS: {Environment.OSVersion}");
                sb.AppendLine($".NET Version: {Environment.Version}");
                sb.AppendLine($"64-bit OS: {Environment.Is64BitOperatingSystem}");
                sb.AppendLine($"64-bit Process: {Environment.Is64BitProcess}");
                sb.AppendLine($"Machine Name: {Environment.MachineName}");
                sb.AppendLine($"User: {Environment.UserName}");
                sb.AppendLine($"Processor Count: {Environment.ProcessorCount}");
                sb.AppendLine($"System Directory: {Environment.SystemDirectory}");
                sb.AppendLine();

                // Application Info
                try
                {
                    var assembly = Assembly.GetExecutingAssembly();
                    var version = assembly.GetName().Version;
                    sb.AppendLine("═══════════════════════════════════════════════════════════════════════════");
                    sb.AppendLine("APPLICATION INFORMATION");
                    sb.AppendLine("═══════════════════════════════════════════════════════════════════════════");
                    sb.AppendLine($"Name: RPG Dungeon Crawler");
                    sb.AppendLine($"Version: {version}");
                    sb.AppendLine($"Location: {assembly.Location}");
                    sb.AppendLine($"Working Directory: {Directory.GetCurrentDirectory()}");
                    sb.AppendLine();
                }
                catch
                {
                    sb.AppendLine("APPLICATION INFORMATION: Could not retrieve");
                    sb.AppendLine();
                }

                // Include all error logs
                var logFiles = GetAllLogFiles();
                if (logFiles.Length > 0)
                {
                    sb.AppendLine("═══════════════════════════════════════════════════════════════════════════");
                    sb.AppendLine("ERROR LOG ENTRIES (Last 5 files)");
                    sb.AppendLine("═══════════════════════════════════════════════════════════════════════════");
                    sb.AppendLine();

                    foreach (var logFile in logFiles.OrderByDescending(f => new FileInfo(f).LastWriteTime).Take(5))
                    {
                        sb.AppendLine($"--- Log File: {Path.GetFileName(logFile)} ---");
                        sb.AppendLine();
                        sb.AppendLine(File.ReadAllText(logFile, Encoding.UTF8));
                        sb.AppendLine();
                    }
                }
                else
                {
                    sb.AppendLine("═══════════════════════════════════════════════════════════════════════════");
                    sb.AppendLine("ERROR LOG ENTRIES");
                    sb.AppendLine("═══════════════════════════════════════════════════════════════════════════");
                    sb.AppendLine("No errors logged.");
                    sb.AppendLine();
                }

                // Save Files Info
                sb.AppendLine("═══════════════════════════════════════════════════════════════════════════");
                sb.AppendLine("SAVE FILES");
                sb.AppendLine("═══════════════════════════════════════════════════════════════════════════");
                var saveFiles = Directory.GetFiles(Directory.GetCurrentDirectory(), "save_*.json");
                if (saveFiles.Length > 0)
                {
                    sb.AppendLine($"Found {saveFiles.Length} save file(s):");
                    foreach (var save in saveFiles)
                    {
                        var fileInfo = new FileInfo(save);
                        sb.AppendLine($"  • {Path.GetFileName(save)} - {fileInfo.Length / 1024} KB - Modified: {fileInfo.LastWriteTime:yyyy-MM-dd HH:mm:ss}");
                    }
                }
                else
                {
                    sb.AppendLine("No save files found.");
                }
                sb.AppendLine();

                sb.AppendLine("═══════════════════════════════════════════════════════════════════════════");
                sb.AppendLine("END OF REPORT");
                sb.AppendLine("═══════════════════════════════════════════════════════════════════════════");

                File.WriteAllText(reportPath, sb.ToString(), Encoding.UTF8);
                return reportPath;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Failed to create error report: {ex.Message}");
                return string.Empty;
            }
        }

        /// <summary>
        /// Gets all error log files
        /// </summary>
        public static string[] GetAllLogFiles()
        {
            try
            {
                EnsureLogDirectoryExists();
                if (Directory.Exists(LogDirectory))
                {
                    return Directory.GetFiles(LogDirectory, "error_log_*.txt");
                }
                return Array.Empty<string>();
            }
            catch
            {
                return Array.Empty<string>();
            }
        }

        /// <summary>
        /// Displays error log statistics
        /// </summary>
        public static void ShowLogStatistics()
        {
            try
            {
                var logFiles = GetAllLogFiles();
                Console.WriteLine("\n╔════════════════════════════════════════════════════════════════╗");
                Console.WriteLine("║                    ERROR LOG STATISTICS                        ║");
                Console.WriteLine("╚════════════════════════════════════════════════════════════════╝");
                Console.WriteLine();

                if (logFiles.Length == 0)
                {
                    Console.WriteLine("✅ No error logs found - your game is running smoothly!");
                }
                else
                {
                    Console.WriteLine($"📁 Total log files: {logFiles.Length}");
                    Console.WriteLine($"📂 Log directory: {LogDirectory}");
                    Console.WriteLine();

                    foreach (var logFile in logFiles)
                    {
                        var fileInfo = new FileInfo(logFile);
                        var fileName = Path.GetFileName(logFile);
                        var errorCount = CountErrorsInFile(logFile);
                        Console.WriteLine($"  • {fileName}");
                        Console.WriteLine($"    Size: {fileInfo.Length / 1024} KB | Errors: {errorCount} | Modified: {fileInfo.LastWriteTime:yyyy-MM-dd HH:mm:ss}");
                        Console.WriteLine();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Error displaying statistics: {ex.Message}");
            }
        }

        private static int CountErrorsInFile(string filePath)
        {
            try
            {
                var content = File.ReadAllText(filePath);
                return content.Split(new[] { "[CRITICAL]", "[ERROR]" }, StringSplitOptions.None).Length - 1;
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// Clears old log files (older than specified days)
        /// </summary>
        public static void CleanOldLogs(int daysToKeep = 30)
        {
            try
            {
                var logFiles = GetAllLogFiles();
                var cutoffDate = DateTime.Now.AddDays(-daysToKeep);
                int deletedCount = 0;

                foreach (var logFile in logFiles)
                {
                    var fileInfo = new FileInfo(logFile);
                    if (fileInfo.LastWriteTime < cutoffDate)
                    {
                        File.Delete(logFile);
                        deletedCount++;
                    }
                }

                if (deletedCount > 0)
                {
                    Console.WriteLine($"🗑️ Cleaned up {deletedCount} old log file(s).");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Error cleaning old logs: {ex.Message}");
            }
        }
    }
}
