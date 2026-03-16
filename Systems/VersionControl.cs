using System;

namespace Rpg_Dungeon.Systems
{
    /// <summary>
    /// Manages version information for the RPG Dungeon Crawler game.
    /// Update this class when releasing new versions.
    /// </summary>
    internal static class VersionControl
    {
        /// <summary>
        /// Current major version number (breaking changes)
        /// </summary>
        public const int MajorVersion = 3;

        /// <summary>
        /// Current minor version number (new features)
        /// </summary>
        public const int MinorVersion = 5;

        /// <summary>
        /// Current patch version number (bug fixes)
        /// </summary>
        public const int PatchVersion = 0;

        /// <summary>
        /// Optional pre-release tag (e.g., "alpha", "beta", "rc1")
        /// Set to null or empty for stable releases
        /// </summary>
        public const string PreReleaseTag = "alpha";

        /// <summary>
        /// GitHub repository URL for the release version
        /// </summary>
        public const string GitHubReleaseUrl = "https://github.com/Yogi1972/Night/releases";

        /// <summary>
        /// GitHub repository URL for the development version
        /// </summary>
        public const string GitHubDevUrl = "https://github.com/Yogi1972/Night";

        /// <summary>
        /// GitHub API URL for latest release (used for update checking)
        /// Only checks official published releases - secure and read-only for users
        /// </summary>
        public const string GitHubVersionCheckUrl = "https://api.github.com/repos/Yogi1972/Night/releases/latest";

        /// <summary>
        /// Build date of this version
        /// </summary>
        public static readonly DateTime BuildDate = new DateTime(2026, 3, 16);

        /// <summary>
        /// Gets the full version string (e.g., "1.0.0" or "1.2.3-beta")
        /// </summary>
        public static string FullVersion
        {
            get
            {
                var version = $"{MajorVersion}.{MinorVersion}.{PatchVersion}";
                if (!string.IsNullOrWhiteSpace(PreReleaseTag))
                {
                    version += $"-{PreReleaseTag}";
                }
                return version;
            }
        }

        /// <summary>
        /// Gets the short version string (e.g., "1.0")
        /// </summary>
        public static string ShortVersion => $"{MajorVersion}.{MinorVersion}";

        /// <summary>
        /// Gets the full version info including build date
        /// </summary>
        public static string FullVersionInfo => $"Version {FullVersion} (Built: {BuildDate:yyyy-MM-dd})";

        /// <summary>
        /// Gets the .NET version the game is built with
        /// </summary>
        public const string DotNetVersion = ".NET 10";

        /// <summary>
        /// Gets the C# language version used
        /// </summary>
        public const string CSharpVersion = "C# 14.0";

        /// <summary>
        /// Checks if this is a pre-release version
        /// </summary>
        public static bool IsPreRelease => !string.IsNullOrWhiteSpace(PreReleaseTag);

        /// <summary>
        /// Gets a user-friendly version display string
        /// </summary>
        public static string DisplayVersion
        {
            get
            {
                var display = FullVersion;
                if (IsPreRelease)
                {
                    display += " 🚧";
                }
                return display;
            }
        }
    }
}
