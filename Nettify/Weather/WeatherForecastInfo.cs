//
// Nettify  Copyright (C) 2023-2024  Aptivi
//
// This file is part of Nettify
//
// Nettify is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// Nettify is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY, without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.
//

using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;

namespace Nettify.Weather
{
    /// <summary>
    /// Forecast information
    /// </summary>
    [DebuggerDisplay("{Weather} @ {Temperature} [{TemperatureMeasurement}]")]
    public partial class WeatherForecastInfo
    {
        /// <summary>
        /// Weather condition
        /// </summary>
        public WeatherCondition Weather { get; }
        /// <summary>
        /// Temperature measurement
        /// </summary>
        public UnitMeasurement TemperatureMeasurement { get; }
        /// <summary>
        /// Temperature
        /// </summary>
        public double Temperature { get; }
        /// <summary>
        /// Humidity in percent
        /// </summary>
        public double Humidity { get; }
        /// <summary>
        /// Wind speed. Imperial: mph, Metric/Kelvin: m.s
        /// </summary>
        public double WindSpeed { get; }
        /// <summary>
        /// Wind direction in degrees
        /// </summary>
        public double WindDirection { get; }
        /// <summary>
        /// Weather token
        /// </summary>
        public JToken WeatherToken { get; }
        /// <summary>
        /// Whether the server type is OWM or TWC
        /// </summary>
        public WeatherServerType ServerType { get; }

        internal WeatherForecastInfo(WeatherCondition weather, UnitMeasurement temperatureMeasurement, double temperature, double humidity, double windSpeed, double windDirection, JToken weatherToken, WeatherServerType serverType)
        {
            Weather = weather;
            TemperatureMeasurement = temperatureMeasurement;
            Temperature = temperature;
            Humidity = humidity;
            WindSpeed = windSpeed;
            WindDirection = windDirection;
            WeatherToken = weatherToken ?? throw new ArgumentNullException(nameof(weatherToken));
            ServerType = serverType;
        }
    }

    /// <summary>
    /// Unit measurement
    /// </summary>
    public enum UnitMeasurement
    {
        /// <summary>
        /// Default unit measurement for OWM
        /// </summary>
        Kelvin = 1,
        /// <summary>
        /// Metric units (Celsius)
        /// </summary>
        Metric,
        /// <summary>
        /// Imperial units (Fahrenheit)
        /// </summary>
        Imperial
    }

    /// <summary>
    /// Weather condition
    /// </summary>
    public enum WeatherCondition
    {
        // Thunderstorms
        /// <summary>
        /// Thunderstorm with light rain
        /// </summary>
        ThunderstormLightRain = 200,
        /// <summary>
        /// Thunderstorm with rain
        /// </summary>
        ThunderstormRain,
        /// <summary>
        /// Thunderstorm with heavy rain
        /// </summary>
        ThunderstormHeavyRain,
        /// <summary>
        /// Light thunderstorm
        /// </summary>
        LightThunderstorm = 210,
        /// <summary>
        /// Thunderstorm
        /// </summary>
        Thunderstorm,
        /// <summary>
        /// Heavy thunderstorm
        /// </summary>
        HeavyThunderstorm,
        /// <summary>
        /// Ragged thunderstorm
        /// </summary>
        RaggedThunderstorm = 221,
        /// <summary>
        /// Thunderstorm with light drizzle
        /// </summary>
        ThunderstormLightDrizzle = 230,
        /// <summary>
        /// Thunderstorm with drizzle
        /// </summary>
        ThunderstormDrizzle = 231,
        /// <summary>
        /// Thunderstorm with heavy drizzle
        /// </summary>
        ThunderstormHeavyDrizzle = 232,

        // Drizzles
        /// <summary>
        /// Light intensity drizzle
        /// </summary>
        LightDrizzle = 300,
        /// <summary>
        /// Drizzle
        /// </summary>
        Drizzle,
        /// <summary>
        /// Heavy intensity drizzle
        /// </summary>
        HeavyDrizzle,
        /// <summary>
        /// Light intensity drizzle rain
        /// </summary>
        LightDrizzleRain = 310,
        /// <summary>
        /// Drizzle rain
        /// </summary>
        DrizzleRain,
        /// <summary>
        /// Heavy intensity drizzle rain
        /// </summary>
        HeavyDrizzleRain,
        /// <summary>
        /// Shower rain and drizzle
        /// </summary>
        DrizzleShowerRain,
        /// <summary>
        /// Heavy shower rain and drizzle
        /// </summary>
        DrizzleHeavyShowerRain,
        /// <summary>
        /// Shower drizzle
        /// </summary>
        ShowerDrizzle = 321,

        // Rains
        /// <summary>
        /// Light rain
        /// </summary>
        LightRain = 500,
        /// <summary>
        /// Moderate rain
        /// </summary>
        ModerateRain,
        /// <summary>
        /// Heavy rain
        /// </summary>
        HeavyRain,
        /// <summary>
        /// Very heavy rain
        /// </summary>
        VeryHeavyRain,
        /// <summary>
        /// Extreme rain
        /// </summary>
        ExtremeRain,
        /// <summary>
        /// Freezing rain
        /// </summary>
        FreezingRain = 511,
        /// <summary>
        /// Light shower rain
        /// </summary>
        LightShowerRain = 520,
        /// <summary>
        /// Shower rain
        /// </summary>
        ShowerRain,
        /// <summary>
        /// Heavy shower rain
        /// </summary>
        HeavyShowerRain,
        /// <summary>
        /// Ragged shower rain
        /// </summary>
        RaggedShowerRain = 531,

        // Snows
        /// <summary>
        /// Light snow
        /// </summary>
        LightSnow = 600,
        /// <summary>
        /// Snow
        /// </summary>
        Snow,
        /// <summary>
        /// Heavy snow
        /// </summary>
        HeavySnow,
        /// <summary>
        /// Sleet
        /// </summary>
        Sleet = 611,
        /// <summary>
        /// Light shower sleet
        /// </summary>
        LightShowerSleet,
        /// <summary>
        /// Shower sleet
        /// </summary>
        ShowerSleet,
        /// <summary>
        /// Light rain and snow
        /// </summary>
        LightRainAndSnow = 615,
        /// <summary>
        /// Rain and snow
        /// </summary>
        RainAndSnow,
        /// <summary>
        /// Light shower snow
        /// </summary>
        LightShowerSnow = 620,
        /// <summary>
        /// Shower snow
        /// </summary>
        ShowerSnow,
        /// <summary>
        /// Heavy shower snow
        /// </summary>
        HeavyShowerSnow,

        // Atmosphere
        /// <summary>
        /// Misty weather
        /// </summary>
        Mist = 701,
        /// <summary>
        /// Smoky weather
        /// </summary>
        Smoke = 711,
        /// <summary>
        /// Hazy weather
        /// </summary>
        Haze = 721,
        /// <summary>
        /// Sand or dust whirls
        /// </summary>
        DustWhirls = 731,
        /// <summary>
        /// Foggy weather
        /// </summary>
        Fog = 741,
        /// <summary>
        /// Sandy weather
        /// </summary>
        Sand = 751,
        /// <summary>
        /// Dusty weather
        /// </summary>
        Dust = 761,
        /// <summary>
        /// Volcanic ash
        /// </summary>
        Ash,
        /// <summary>
        /// Squall
        /// </summary>
        Squall = 771,
        /// <summary>
        /// Tornado
        /// </summary>
        Tornado = 781,

        // Clear and Clouds
        /// <summary>
        /// Clear sky (free of clouds)
        /// </summary>
        Clear = 800,
        /// <summary>
        /// Few clouds (11-25%)
        /// </summary>
        FewClouds,
        /// <summary>
        /// Partly cloudy (Scattered, 25-50%)
        /// </summary>
        PartlyCloudy,
        /// <summary>
        /// Broken Clouds (51-84%)
        /// </summary>
        BrokenClouds,
        /// <summary>
        /// Mostly Cloudy (Overcast, 85-100%)
        /// </summary>
        MostlyCloudy
    }

    /// <summary>
    /// Weather server type
    /// </summary>
    public enum WeatherServerType
    {
        /// <summary>
        /// OpenWeatherMap
        /// </summary>
        OpenWeatherMap,
        /// <summary>
        /// The Weather Channel
        /// </summary>
        TheWeatherChannel,
    }
}
