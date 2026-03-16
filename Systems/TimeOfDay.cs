using System;
using System.Collections.Generic;
using System.Text;

namespace Rpg_Dungeon
{
    #region TimePeriod Enum

    internal enum TimePeriod
    {
        Dawn,
        Morning,
        Noon,
        Afternoon,
        Dusk,
        Evening,
        Night,
        Midnight
    }

    #endregion

    #region TimeOfDay Class

    internal class TimeOfDay
    {
        #region Fields and Properties

        private int _currentHour;
        private int _currentDay;
        private static readonly Random _rng = new Random();

        public int CurrentHour => _currentHour;
        public int CurrentDay => _currentDay;
        public TimePeriod CurrentPeriod => GetTimePeriod();

        #endregion

        #region Constructor

        public TimeOfDay()
        {
            _currentHour = 8;
            _currentDay = 1;
        }

        #endregion

        #region Time Management

        public void AdvanceTime(int hours)
        {
            if (hours <= 0) return;

            int oldHour = _currentHour;
            _currentHour += hours;

            // Handle day rollover
            while (_currentHour >= 24)
            {
                _currentHour -= 24;
                _currentDay++;
                Console.WriteLine($"\n🌅 A new day begins! Day {_currentDay}");
            }

            // Notify if time period changed significantly
            var oldPeriod = GetTimePeriodForHour(oldHour);
            var newPeriod = GetTimePeriod();
            if (oldPeriod != newPeriod)
            {
                Console.WriteLine($"⏰ Time passes... It is now {GetTimeDescription()}");
            }
        }

        #endregion

        #region Time Period Methods

        public TimePeriod GetTimePeriod()
        {
            return GetTimePeriodForHour(_currentHour);
        }

        private TimePeriod GetTimePeriodForHour(int hour)
        {
            return hour switch
            {
                >= 5 and < 7 => TimePeriod.Dawn,
                >= 7 and < 11 => TimePeriod.Morning,
                >= 11 and < 13 => TimePeriod.Noon,
                >= 13 and < 17 => TimePeriod.Afternoon,
                >= 17 and < 19 => TimePeriod.Dusk,
                >= 19 and < 21 => TimePeriod.Evening,
                >= 21 and < 24 => TimePeriod.Night,
                _ => TimePeriod.Midnight // 0-5am
            };
        }

        #endregion

        #region Time Descriptions

        public string GetTimeDescription()
        {
            string period = CurrentPeriod switch
            {
                TimePeriod.Dawn => "🌅 Dawn",
                TimePeriod.Morning => "🌄 Morning",
                TimePeriod.Noon => "☀️ Noon",
                TimePeriod.Afternoon => "🌤️ Afternoon",
                TimePeriod.Dusk => "🌆 Dusk",
                TimePeriod.Evening => "🌃 Evening",
                TimePeriod.Night => "🌙 Night",
                TimePeriod.Midnight => "🌑 Midnight",
                _ => "Unknown"
            };

            return $"{period} ({FormatHour(_currentHour)})";
        }

        private string FormatHour(int hour)
        {
            if (hour == 0) return "12:00 AM";
            if (hour < 12) return $"{hour}:00 AM";
            if (hour == 12) return "12:00 PM";
            return $"{hour - 12}:00 PM";
        }

        #endregion

        #region Time Descriptions

        public string GetAtmosphericDescription()
        {
            return CurrentPeriod switch
            {
                TimePeriod.Dawn => _rng.Next(3) switch
                {
                    0 => "The first rays of sunlight peek over the horizon.",
                    1 => "Birds begin their morning songs as dawn breaks.",
                    _ => "The sky lightens with the promise of a new day."
                },
                TimePeriod.Morning => _rng.Next(3) switch
                {
                    0 => "The morning sun climbs higher in the sky.",
                    1 => "Dew glistens on the grass in the morning light.",
                    _ => "The world awakens with the morning."
                },
                TimePeriod.Noon => _rng.Next(3) switch
                {
                    0 => "The sun reaches its zenith, casting short shadows.",
                    1 => "Midday heat warms the land.",
                    _ => "The sun blazes overhead at its peak."
                },
                TimePeriod.Afternoon => _rng.Next(3) switch
                {
                    0 => "The afternoon sun begins its slow descent.",
                    1 => "Long shadows stretch across the ground.",
                    _ => "The day continues under the afternoon sun."
                },
                TimePeriod.Dusk => _rng.Next(3) switch
                {
                    0 => "The sun sets, painting the sky in brilliant colors.",
                    1 => "Twilight falls as day gives way to night.",
                    _ => "The world is bathed in the golden light of dusk."
                },
                TimePeriod.Evening => _rng.Next(3) switch
                {
                    0 => "Darkness settles over the land. Stars begin to appear.",
                    1 => "The evening air grows cool and still.",
                    _ => "Lamplight flickers in distant windows."
                },
                TimePeriod.Night => _rng.Next(3) switch
                {
                    0 => "The moon illuminates your path through the darkness.",
                    1 => "Nocturnal creatures stir in the shadows.",
                    _ => "The night is deep and full of mystery."
                },
                TimePeriod.Midnight => _rng.Next(3) switch
                {
                    0 => "The witching hour. All is silent and dark.",
                    1 => "The deepest part of night. Even the stars seem dim.",
                    _ => "Midnight. The world sleeps while you remain vigilant."
                },
                _ => "Time passes."
            };
        }

        #endregion

        #region Helper Methods

        public bool IsDaytime()
        {
            return _currentHour >= 6 && _currentHour < 18;
        }

        public bool IsNighttime()
        {
            return !IsDaytime();
        }

        public string GetTimeSuggestion()
        {
            return CurrentPeriod switch
            {
                TimePeriod.Midnight => "⚠️ It's very late. Consider resting at an inn or making camp.",
                TimePeriod.Night => "🌙 Night has fallen. Combat is more dangerous in darkness.",
                TimePeriod.Dawn => "🌅 Dawn is breaking. A good time to start adventures!",
                TimePeriod.Morning => "☕ Morning. Shops are open and adventurers are active.",
                TimePeriod.Noon => "☀️ Midday. Peak time for dungeon delving.",
                TimePeriod.Dusk => "🌆 Dusk approaches. You may want to find shelter soon.",
                _ => string.Empty
            };
        }

        // Certain actions take different amounts of time
        public int GetTravelTimeHours(int distance)
        {
            // Base: 2 hours per 5 km (average walking speed)
            int baseTime = Math.Max(1, distance / 3);

            // Night travel takes longer (harder to see)
            if (IsNighttime())
            {
                baseTime = (int)(baseTime * 1.3);
            }

            return baseTime;
        }

        #endregion
    }

    #endregion
}
