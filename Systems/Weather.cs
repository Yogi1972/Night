using Night.Characters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rpg_Dungeon
{
    #region WeatherType Enum

    internal enum WeatherType
    {
        Clear,
        Cloudy,
        Rainy,
        Stormy,
        Foggy,
        Snowy,
        Windy
    }

    #endregion

    #region Weather Class

    internal class Weather
    {
        #region Fields and Properties

        private static readonly Random _rng = new Random();
        private WeatherType _currentWeather;
        private int _weatherDuration;
        private TimeOfDay? _timeTracker;

        public WeatherType CurrentWeather => _currentWeather;

        #endregion

        #region Constructor

        public Weather()
        {
            _currentWeather = WeatherType.Clear;
            _weatherDuration = _rng.Next(3, 8);
        }

        #endregion

        #region Weather System

        public void SetTimeTracker(TimeOfDay timeTracker)
        {
            _timeTracker = timeTracker;
        }

        public void UpdateWeather()
        {
            _weatherDuration--;
            if (_weatherDuration <= 0)
            {
                ChangeWeather();
                _weatherDuration = _rng.Next(3, 8);
            }
        }

        private void ChangeWeather()
        {
            int roll = _rng.Next(100);
            WeatherType newWeather = roll switch
            {
                < 30 => WeatherType.Clear,
                < 50 => WeatherType.Cloudy,
                < 65 => WeatherType.Rainy,
                < 75 => WeatherType.Stormy,
                < 85 => WeatherType.Foggy,
                < 92 => WeatherType.Snowy,
                _ => WeatherType.Windy
            };

            // Time-based weather restrictions
            if (_timeTracker != null)
            {
                // Can't be "Clear" (sunny) at night - change to cloudy or other night-appropriate weather
                if (_timeTracker.IsNighttime() && newWeather == WeatherType.Clear)
                {
                    var nightWeathers = new[] { WeatherType.Cloudy, WeatherType.Foggy, WeatherType.Windy };
                    newWeather = nightWeathers[_rng.Next(nightWeathers.Length)];
                }

                // Foggy is more common at dawn/dusk
                if ((_timeTracker.CurrentPeriod == TimePeriod.Dawn || _timeTracker.CurrentPeriod == TimePeriod.Dusk) && roll < 40)
                {
                    newWeather = WeatherType.Foggy;
                }

                // Snow less likely during hot noon
                if (_timeTracker.CurrentPeriod == TimePeriod.Noon && newWeather == WeatherType.Snowy && roll < 80)
                {
                    newWeather = WeatherType.Clear;
                }
            }

            if (newWeather != _currentWeather)
            {
                _currentWeather = newWeather;
                Console.WriteLine($"\n🌤️  The weather changes to {GetWeatherDescription()}");
            }
        }

        #endregion

        #region Weather Descriptions

        public string GetWeatherDescription()
        {
            // Adjust descriptions based on time of day
            if (_timeTracker != null && _timeTracker.IsNighttime())
            {
                return _currentWeather switch
                {
                    WeatherType.Clear => "🌙 Clear night - Stars shine brightly",
                    WeatherType.Cloudy => "☁️ Cloudy night - The moon is hidden",
                    WeatherType.Rainy => "🌧️ Rainy night - Rain falls in darkness",
                    WeatherType.Stormy => "⛈️ Stormy night - Thunder and lightning pierce the darkness",
                    WeatherType.Foggy => "🌫️ Foggy night - Thick fog obscures everything",
                    WeatherType.Snowy => "❄️ Snowy night - Snow falls under moonlight",
                    WeatherType.Windy => "💨 Windy night - Cold winds howl through the darkness",
                    _ => "Unknown"
                };
            }

            // Daytime descriptions
            return _currentWeather switch
            {
                WeatherType.Clear => "☀️ Clear skies - A beautiful day",
                WeatherType.Cloudy => "☁️ Cloudy - Grey clouds blanket the sky",
                WeatherType.Rainy => "🌧️ Rainy - Steady rain falls from above",
                WeatherType.Stormy => "⛈️ Stormy - Thunder rumbles and lightning flashes",
                WeatherType.Foggy => "🌫️ Foggy - Thick fog reduces visibility",
                WeatherType.Snowy => "❄️ Snowy - Snowflakes drift down gently",
                WeatherType.Windy => "💨 Windy - Strong winds howl across the land",
                _ => "Unknown"
            };
        }

        public string GetTravelFlavorText()
        {
            // For clear weather, check if it's day or night
            if (_currentWeather == WeatherType.Clear)
            {
                if (_timeTracker != null && _timeTracker.IsDaytime())
                {
                    return _rng.Next(3) switch
                    {
                        0 => "The sun warms your face as you travel.",
                        1 => "Birds sing in the distance. The journey is pleasant.",
                        _ => "Clear skies make for easy travel."
                    };
                }
                else
                {
                    // Clear night - stars visible
                    return _rng.Next(3) switch
                    {
                        0 => "Stars light your path through the clear night.",
                        1 => "The moon illuminates the way. The journey is peaceful.",
                        _ => "Clear night skies guide your travel."
                    };
                }
            }

            return _currentWeather switch
            {
                WeatherType.Cloudy => _rng.Next(3) switch
                {
                    0 => "Grey clouds loom overhead as you make your way.",
                    1 => "The overcast sky casts shadows on the path.",
                    _ => "You travel under dreary clouds."
                },
                WeatherType.Rainy => _rng.Next(3) switch
                {
                    0 => "Rain soaks your clothes and gear. The journey is miserable.",
                    1 => "You trudge through muddy paths as rain pours down.",
                    _ => "The rain makes every step more difficult."
                },
                WeatherType.Stormy => _rng.Next(3) switch
                {
                    0 => "Lightning cracks across the sky! Thunder shakes the ground.",
                    1 => "The storm rages around you. Travel is treacherous.",
                    _ => "You press on through the violent storm, seeking shelter."
                },
                WeatherType.Foggy => _rng.Next(3) switch
                {
                    0 => "Thick fog makes it hard to see the path ahead.",
                    1 => "You move slowly through the dense mist.",
                    _ => "The fog obscures everything beyond a few feet."
                },
                WeatherType.Snowy => _rng.Next(3) switch
                {
                    0 => "Snow crunches under your boots. Your breath mists in the cold air.",
                    1 => "The snowfall makes the journey slow and cold.",
                    _ => "You trudge through the snow, leaving tracks behind."
                },
                WeatherType.Windy => _rng.Next(3) switch
                {
                    0 => "Strong winds buffet you as you travel. Walking is difficult.",
                    1 => "The wind howls and whips at your cloak.",
                    _ => "You lean into the fierce winds, struggling forward."
                },
                _ => "You travel onward."
            };
        }

        #endregion

        #region Travel Effects

        public double GetTravelTimeModifier()
        {
            return _currentWeather switch
            {
                WeatherType.Clear => 1.0,      // Normal speed
                WeatherType.Cloudy => 1.0,     // Normal speed
                WeatherType.Rainy => 1.3,      // 30% slower
                WeatherType.Stormy => 1.5,     // 50% slower
                WeatherType.Foggy => 1.4,      // 40% slower
                WeatherType.Snowy => 1.4,      // 40% slower
                WeatherType.Windy => 1.2,      // 20% slower
                _ => 1.0
            };
        }

        // Some weather might cause damage during long travels
        public bool IsHazardousWeather()
        {
            return _currentWeather == WeatherType.Stormy || _currentWeather == WeatherType.Snowy;
        }

        public void ApplyTravelHazard(List<Character> party)
        {
            if (!IsHazardousWeather()) return;

            if (_currentWeather == WeatherType.Stormy && _rng.Next(100) < 20)
            {
                Console.WriteLine("\n⚡ Lightning strikes nearby! The party takes minor damage!");
                foreach (var member in party.Where(p => p.IsAlive))
                {
                    int damage = _rng.Next(3, 8);
                    member.ReceiveDamage(damage);
                    Console.WriteLine($"  {member.Name} takes {damage} damage from the storm!");
                }
            }
            else if (_currentWeather == WeatherType.Snowy && _rng.Next(100) < 15)
            {
                Console.WriteLine("\n❄️ The bitter cold saps your strength!");
                foreach (var member in party.Where(p => p.IsAlive))
                {
                    int damage = _rng.Next(2, 6);
                    member.ReceiveDamage(damage);
                    Console.WriteLine($"  {member.Name} takes {damage} damage from exposure!");
                }
            }
        }

        #endregion
    }

    #endregion
}
