using System;
using System.IO;
using System.Text.Json;

namespace Rpg_Dungeon.Systems
{
    internal class UserSettings
    {
        public bool AutoUpdateSilent { get; set; } = false;
    }

    internal static class SettingsManager
    {
        private static readonly string _settingsPath = Path.Combine(AppContext.BaseDirectory, "user_settings.json");
        private static UserSettings? _cached;

        public static UserSettings Load()
        {
            if (_cached != null) return _cached;
            try
            {
                if (File.Exists(_settingsPath))
                {
                    var json = File.ReadAllText(_settingsPath);
                    _cached = JsonSerializer.Deserialize<UserSettings>(json) ?? new UserSettings();
                }
                else
                {
                    _cached = new UserSettings();
                    Save(_cached);
                }
            }
            catch
            {
                _cached = new UserSettings();
            }
            return _cached;
        }

        public static void Save(UserSettings settings)
        {
            try
            {
                var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(_settingsPath, json);
                _cached = settings;
            }
            catch
            {
                // ignore
            }
        }
    }
}
