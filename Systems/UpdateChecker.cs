using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Rpg_Dungeon.Systems
{
    /// <summary>
    /// Handles checking for game updates from GitHub
    /// </summary>
    internal static class UpdateChecker
    {
        private static readonly HttpClient _httpClient = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(10)
        };

        /// <summary>
        /// Version information from GitHub Releases API
        /// </summary>
        public class VersionInfo
        {
            public int MajorVersion { get; set; }
            public int MinorVersion { get; set; }
            public int PatchVersion { get; set; }
            public string? PreReleaseTag { get; set; }
            public string? ReleaseNotes { get; set; }
            public string? ReleaseDate { get; set; }
        }

        /// <summary>
        /// GitHub API release response model
        /// </summary>
        private class GitHubRelease
        {
            public string? tag_name { get; set; }
            public string? name { get; set; }
            public string? body { get; set; }
            public string? published_at { get; set; }
            public bool prerelease { get; set; }
            public GitHubAsset[]? assets { get; set; }
        }

        /// <summary>
        /// GitHub API asset response model
        /// </summary>
        private class GitHubAsset
        {
            public string? name { get; set; }
            public string? browser_download_url { get; set; }
            public long size { get; set; }
        }

        /// <summary>
        /// Check for updates from GitHub Releases API
        /// </summary>
        public static async Task<VersionInfo?> CheckForUpdatesAsync()
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("User-Agent", "Rpg-Dungeon-Crawler/3.0");
                _httpClient.DefaultRequestHeaders.Add("Accept", "application/vnd.github+json");
                _httpClient.DefaultRequestHeaders.Add("X-GitHub-Api-Version", "2022-11-28");

                var response = await _httpClient.GetStringAsync(VersionControl.GitHubVersionCheckUrl);
                var githubRelease = JsonSerializer.Deserialize<GitHubRelease>(response, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (githubRelease == null || string.IsNullOrWhiteSpace(githubRelease.tag_name))
                {
                    return null;
                }

                return ParseGitHubRelease(githubRelease);
            }
            catch (HttpRequestException ex)
            {
                // Check if it's a 404 - no releases published yet
                if (ex.Message.Contains("404"))
                {
                    ErrorLogger.LogWarning("No GitHub releases published yet", "Update check skipped - no releases available");
                }
                else
                {
                    ErrorLogger.LogWarning($"Failed to check for updates: {ex.Message}", "Network error or GitHub unavailable");
                }
                return null;
            }
            catch (TaskCanceledException)
            {
                ErrorLogger.LogWarning("Update check timed out", "Network may be slow or unavailable");
                return null;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogWarning($"Update check failed: {ex.Message}", "Unexpected error during update check");
                return null;
            }
        }

        /// <summary>
        /// Parse GitHub release info into VersionInfo
        /// </summary>
        private static VersionInfo ParseGitHubRelease(GitHubRelease release)
        {
            var versionInfo = new VersionInfo
            {
                ReleaseNotes = release.body,
                ReleaseDate = release.published_at
            };

            // Parse tag name (e.g., "v2.1.0" or "2.1.0-alpha")
            string tagName = release.tag_name?.TrimStart('v', 'V') ?? "0.0.0";

            // Split by '-' to separate version from pre-release tag
            string[] parts = tagName.Split('-');
            string versionPart = parts[0];

            if (parts.Length > 1)
            {
                versionInfo.PreReleaseTag = parts[1];
            }

            // Parse version numbers
            string[] versionNumbers = versionPart.Split('.');
            if (versionNumbers.Length >= 1 && int.TryParse(versionNumbers[0], out int major))
            {
                versionInfo.MajorVersion = major;
            }
            if (versionNumbers.Length >= 2 && int.TryParse(versionNumbers[1], out int minor))
            {
                versionInfo.MinorVersion = minor;
            }
            if (versionNumbers.Length >= 3 && int.TryParse(versionNumbers[2], out int patch))
            {
                versionInfo.PatchVersion = patch;
            }

            return versionInfo;
        }

        /// <summary>
        /// Compare versions and determine if update is available
        /// </summary>
        public static bool IsNewerVersion(VersionInfo remoteVersion)
        {
            if (remoteVersion == null) return false;

            if (remoteVersion.MajorVersion > VersionControl.MajorVersion) return true;
            if (remoteVersion.MajorVersion < VersionControl.MajorVersion) return false;

            if (remoteVersion.MinorVersion > VersionControl.MinorVersion) return true;
            if (remoteVersion.MinorVersion < VersionControl.MinorVersion) return false;

            if (remoteVersion.PatchVersion > VersionControl.PatchVersion) return true;
            if (remoteVersion.PatchVersion < VersionControl.PatchVersion) return false;

            // Same version but remote is stable and local is pre-release
            if (string.IsNullOrWhiteSpace(remoteVersion.PreReleaseTag) && !string.IsNullOrWhiteSpace(VersionControl.PreReleaseTag))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Show update check screen with results
        /// </summary>
        public static void ShowUpdateCheckScreen()
        {
            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                      CHECK FOR UPDATES                           ║");
            Console.WriteLine("╚══════════════════════════════════════════════════════════════════╝");
            Console.WriteLine();
            Console.WriteLine($"  📌 Current Version: {VersionControl.FullVersion}");
            Console.WriteLine($"  📅 Build Date: {VersionControl.BuildDate:yyyy-MM-dd}");
            Console.WriteLine();
            Console.WriteLine("  🔍 Checking for updates...");
            Console.WriteLine();

            try
            {
                var task = CheckForUpdatesAsync();
                task.Wait();
                var remoteVersion = task.Result;

                if (remoteVersion == null)
                {
                    Console.WriteLine("  ⚠️  Unable to check for updates.");
                    Console.WriteLine("  • Check your internet connection");
                    Console.WriteLine("  • GitHub may be temporarily unavailable");
                    Console.WriteLine();
                    Console.WriteLine("  💡 You can manually check for updates at:");
                    Console.WriteLine($"     {VersionControl.GitHubReleaseUrl}");
                }
                else if (IsNewerVersion(remoteVersion))
                {
                    Console.WriteLine("  🎉 NEW UPDATE AVAILABLE!");
                    Console.WriteLine();
                    Console.WriteLine($"  📦 Latest Version: {remoteVersion.MajorVersion}.{remoteVersion.MinorVersion}.{remoteVersion.PatchVersion}");

                    if (!string.IsNullOrWhiteSpace(remoteVersion.PreReleaseTag))
                    {
                        Console.WriteLine($"  🚧 Pre-release: {remoteVersion.PreReleaseTag}");
                    }

                    if (!string.IsNullOrWhiteSpace(remoteVersion.ReleaseDate))
                    {
                        Console.WriteLine($"  📅 Released: {remoteVersion.ReleaseDate}");
                    }

                    Console.WriteLine();

                    if (!string.IsNullOrWhiteSpace(remoteVersion.ReleaseNotes))
                    {
                        Console.WriteLine("  📝 What's New:");
                        Console.WriteLine($"     {remoteVersion.ReleaseNotes}");
                        Console.WriteLine();
                    }

                    Console.WriteLine("  💡 Press 'A' to auto-update, 'D' to download manually, or Enter to continue...");
                    var key = Console.ReadKey(true);

                    if (key.KeyChar == 'A' || key.KeyChar == 'a')
                    {
                        DownloadAndInstallUpdate(remoteVersion);
                    }
                    else if (key.KeyChar == 'D' || key.KeyChar == 'd')
                    {
                        OpenGitHubReleases();
                    }
                }
                else
                {
                    Console.WriteLine("  ✅ You're running the latest version!");
                    Console.WriteLine();
                    Console.WriteLine("  📌 No updates available at this time.");

                    if (VersionControl.IsPreRelease)
                    {
                        Console.WriteLine();
                        Console.WriteLine("  🚧 You're running a pre-release version.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("  ❌ Error checking for updates:");
                Console.WriteLine($"     {ex.Message}");
                ErrorLogger.LogWarning($"Update check failed: {ex.Message}", "Error in ShowUpdateCheckScreen");
            }

            Console.WriteLine();
            Console.WriteLine("  💡 Press Enter to return or 'R' to view releases on GitHub...");

            var finalKey = Console.ReadKey(true);
            if (finalKey.KeyChar == 'R' || finalKey.KeyChar == 'r')
            {
                OpenGitHubReleases();
            }
        }

        /// <summary>
        /// Open the GitHub releases page in the default browser
        /// </summary>
        public static void OpenGitHubReleases()
        {
            try
            {
                Console.WriteLine();
                Console.WriteLine("  🌐 Opening GitHub releases page...");
                Process.Start(new ProcessStartInfo
                {
                    FileName = VersionControl.GitHubReleaseUrl,
                    UseShellExecute = true
                });
                System.Threading.Thread.Sleep(1000);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  ❌ Failed to open browser: {ex.Message}");
                Console.WriteLine($"  📋 Copy this URL manually: {VersionControl.GitHubReleaseUrl}");
                ErrorLogger.LogWarning($"Failed to open GitHub URL: {ex.Message}", "Browser launch failed");
                System.Threading.Thread.Sleep(2000);
            }
        }

        /// <summary>
        /// Download and install update automatically
        /// </summary>
        private static void DownloadAndInstallUpdate(VersionInfo remoteVersion)
        {
            try
            {
                Console.WriteLine();
                Console.WriteLine("  🔄 Starting auto-update...");
                Console.WriteLine();

                // Get download URL
                Console.WriteLine("  📡 Fetching download information...");
                var downloadUrl = GetDownloadUrl(remoteVersion).Result;

                if (string.IsNullOrWhiteSpace(downloadUrl))
                {
                    Console.WriteLine("  ❌ Could not find download file.");
                    Console.WriteLine("  💡 Falling back to manual download...");
                    System.Threading.Thread.Sleep(2000);
                    OpenGitHubReleases();
                    return;
                }

                Console.WriteLine($"  ✅ Found: {Path.GetFileName(downloadUrl)}");
                Console.WriteLine();

                // Download update
                string tempZipPath = Path.Combine(Path.GetTempPath(), "rpg_update.zip");
                string tempExtractPath = Path.Combine(Path.GetTempPath(), "rpg_update");

                Console.WriteLine("  ⬇️  Downloading update...");
                DownloadFileAsync(downloadUrl, tempZipPath).Wait();
                Console.WriteLine("  ✅ Download complete!");
                Console.WriteLine();

                // Extract files
                Console.WriteLine("  📦 Extracting files...");
                if (Directory.Exists(tempExtractPath))
                {
                    Directory.Delete(tempExtractPath, true);
                }
                ZipFile.ExtractToDirectory(tempZipPath, tempExtractPath);
                Console.WriteLine("  ✅ Extraction complete!");
                Console.WriteLine();

                // Create updater script
                string updaterScript = CreateUpdaterScript(tempExtractPath);
                Console.WriteLine("  🔧 Preparing installation...");
                Console.WriteLine();

                // Launch updater and exit
                Console.WriteLine("  🚀 Launching installer...");
                Console.WriteLine("  ⏳ The game will restart automatically.");
                Console.WriteLine();
                Console.WriteLine("  Please wait...");

                Process.Start(new ProcessStartInfo
                {
                    FileName = "powershell.exe",
                    Arguments = $"-ExecutionPolicy Bypass -WindowStyle Hidden -File \"{updaterScript}\"",
                    UseShellExecute = false,
                    CreateNoWindow = true
                });

                System.Threading.Thread.Sleep(2000);
                Environment.Exit(0);
            }
            catch (Exception ex)
            {
                Console.WriteLine();
                Console.WriteLine($"  ❌ Auto-update failed: {ex.Message}");
                Console.WriteLine("  💡 Please download manually from GitHub.");
                ErrorLogger.LogWarning($"Auto-update failed: {ex.Message}", "DownloadAndInstallUpdate error");
                System.Threading.Thread.Sleep(3000);
                OpenGitHubReleases();
            }
        }

        /// <summary>
        /// Get the download URL for the latest release
        /// </summary>
        private static async Task<string?> GetDownloadUrl(VersionInfo remoteVersion)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("User-Agent", "Rpg-Dungeon-Crawler/3.0");
                _httpClient.DefaultRequestHeaders.Add("Accept", "application/vnd.github+json");
                _httpClient.DefaultRequestHeaders.Add("X-GitHub-Api-Version", "2022-11-28");

                var response = await _httpClient.GetStringAsync(VersionControl.GitHubVersionCheckUrl);
                var githubRelease = JsonSerializer.Deserialize<GitHubRelease>(response, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (githubRelease?.assets == null || githubRelease.assets.Length == 0)
                {
                    return null;
                }

                // Find the zip file
                foreach (var asset in githubRelease.assets)
                {
                    if (asset.name?.EndsWith(".zip", StringComparison.OrdinalIgnoreCase) == true)
                    {
                        return asset.browser_download_url;
                    }
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Download a file with progress
        /// </summary>
        private static async Task DownloadFileAsync(string url, string destinationPath)
        {
            using var response = await _httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();

            var totalBytes = response.Content.Headers.ContentLength ?? 0;
            using var contentStream = await response.Content.ReadAsStreamAsync();
            using var fileStream = new FileStream(destinationPath, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true);

            var buffer = new byte[8192];
            long totalRead = 0;
            int bytesRead;
            int lastProgress = -1;

            while ((bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
            {
                await fileStream.WriteAsync(buffer, 0, bytesRead);
                totalRead += bytesRead;

                if (totalBytes > 0)
                {
                    int progress = (int)((totalRead * 100) / totalBytes);
                    if (progress != lastProgress && progress % 10 == 0)
                    {
                        Console.Write($"\r  📊 Progress: {progress}%  ");
                        lastProgress = progress;
                    }
                }
            }
            Console.WriteLine();
        }

        /// <summary>
        /// Create a PowerShell script to update files and restart the game
        /// </summary>
        private static string CreateUpdaterScript(string extractPath)
        {
            string currentDir = AppDomain.CurrentDomain.BaseDirectory;
            string currentExe = Process.GetCurrentProcess().MainModule?.FileName ?? "Night.exe";
            string scriptPath = Path.Combine(Path.GetTempPath(), "rpg_updater.ps1");

            // Build script line by line to avoid string literal issues
            var scriptLines = new System.Collections.Generic.List<string>
            {
                "Write-Host 'RPG Dungeon Auto-Updater' -ForegroundColor Cyan",
                "Write-Host '=========================' -ForegroundColor Cyan",
                "Write-Host ''",
                "",
                "# Wait for game to close",
                "Write-Host 'Waiting for game to close...' -ForegroundColor Yellow",
                "Start-Sleep -Seconds 2",
                "",
                "# Get all processes named Night",
                "$processes = Get-Process -Name 'Night' -ErrorAction SilentlyContinue",
                "if ($processes) {",
                "    Write-Host 'Closing game process...' -ForegroundColor Yellow",
                "    $processes | Stop-Process -Force",
                "    Start-Sleep -Seconds 1",
                "}",
                "",
                "Write-Host 'Installing update...' -ForegroundColor Yellow",
                "Write-Host ''",
                "",
                "# Copy new files",
                $"$sourceDir = '{extractPath}'",
                $"$targetDir = '{currentDir}'",
                "",
                "try {",
                "    # Backup old exe",
                "    $oldExe = Join-Path $targetDir 'Night.exe'",
                "    $backupExe = Join-Path $targetDir 'Night.exe.backup'",
                "    if (Test-Path $oldExe) {",
                "        Copy-Item $oldExe $backupExe -Force",
                "    }",
                "",
                "    # Copy all files",
                "    Get-ChildItem -Path $sourceDir -Recurse | ForEach-Object {",
                "        $targetPath = $_.FullName.Replace($sourceDir, $targetDir)",
                "        if ($_.PSIsContainer) {",
                "            if (-not (Test-Path $targetPath)) {",
                "                New-Item -ItemType Directory -Path $targetPath -Force | Out-Null",
                "            }",
                "        } else {",
                "            Copy-Item $_.FullName $targetPath -Force",
                "            Write-Host \"  + $($_.Name)\" -ForegroundColor Green",
                "        }",
                "    }",
                "",
                "    Write-Host ''",
                "    Write-Host '[OK] Update installed successfully!' -ForegroundColor Green",
                "    Write-Host ''",
                "    Write-Host 'Restarting game...' -ForegroundColor Cyan",
                "    Start-Sleep -Seconds 2",
                "",
                "    # Restart game",
                $"    Start-Process -FilePath '{currentExe}' -WorkingDirectory $targetDir",
                "",
                "    # Cleanup",
                "    Start-Sleep -Seconds 2",
                "    Remove-Item -Path $sourceDir -Recurse -Force -ErrorAction SilentlyContinue",
                $"    Remove-Item -Path '{Path.Combine(Path.GetTempPath(), "rpg_update.zip")}' -Force -ErrorAction SilentlyContinue",
                "    Remove-Item -Path $backupExe -Force -ErrorAction SilentlyContinue",
                "    Remove-Item -Path $MyInvocation.MyCommand.Path -Force -ErrorAction SilentlyContinue",
                "}",
                "catch {",
                "    Write-Host ''",
                "    Write-Host '[ERROR] Update failed!' -ForegroundColor Red",
                "    Write-Host \"Error: $($_.Exception.Message)\" -ForegroundColor Red",
                "    Write-Host ''",
                "    Write-Host 'Restoring backup...' -ForegroundColor Yellow",
                "",
                "    # Restore backup",
                "    $oldExe = Join-Path $targetDir 'Night.exe'",
                "    $backupExe = Join-Path $targetDir 'Night.exe.backup'",
                "    if (Test-Path $backupExe) {",
                "        Copy-Item $backupExe $oldExe -Force",
                "        Remove-Item $backupExe -Force",
                "    }",
                "",
                "    Write-Host 'Restarting game...' -ForegroundColor Cyan",
                $"    Start-Process -FilePath '{currentExe}' -WorkingDirectory $targetDir",
                "",
                "    Start-Sleep -Seconds 5",
                "}"
            };

            string script = string.Join(Environment.NewLine, scriptLines);
            File.WriteAllText(scriptPath, script);
            return scriptPath;
        }

            }
        }
