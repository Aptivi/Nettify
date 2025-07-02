//
// Nettify  Copyright (C) 2023-2025  Aptivi
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

using Nettify.Language;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace Nettify.Weather
{
    /// <summary>
    /// The forecast tools (The Weather Channel)
    /// </summary>
    public static class WeatherForecast
    {
        internal static HttpClient WeatherDownloader = new();

        /// <summary>
        /// Gets current weather info from The Weather Channel
        /// </summary>
        /// <param name="latitude">City latitude</param>
        /// <param name="longitude">City longitude</param>
        /// <param name="APIKey">API Key</param>
        /// <param name="Unit">The preferred unit to use</param>
        /// <returns>A class containing properties of weather information</returns>
        public static WeatherForecastInfo GetWeatherInfo(double latitude, double longitude, string APIKey, UnitMeasurement Unit = UnitMeasurement.Metric)
        {
            string WeatherURL = $"https://api.weather.com/v3/wx/forecast/daily/15day?geocode={latitude},{longitude}&format=json&language=en-US&units={(Unit == UnitMeasurement.Metric ? 'm' : 'e')}&apiKey={APIKey}";
            return GetWeatherInfo(WeatherURL, Unit == UnitMeasurement.Kelvin ? UnitMeasurement.Imperial : Unit);
        }

        /// <summary>
        /// Gets current weather info from The Weather Channel
        /// </summary>
        /// <param name="WeatherURL">An URL to the weather API request</param>
        /// <param name="Unit">The preferred unit to use</param>
        /// <returns>A class containing properties of weather information</returns>
        internal static WeatherForecastInfo GetWeatherInfo(string WeatherURL, UnitMeasurement Unit = UnitMeasurement.Metric)
        {
            Debug.WriteLine("Weather URL: {0} | Unit: {1}", WeatherURL, Unit);

            // Deal with measurements
            Unit = Unit == UnitMeasurement.Kelvin ? UnitMeasurement.Imperial : Unit;

            // Download and parse JSON data
            WeatherDownloader.DefaultRequestHeaders.Add("Accept-Encoding", "gzip");
            var stream = WeatherDownloader.GetStreamAsync(WeatherURL).Result;
            WeatherDownloader.DefaultRequestHeaders.Remove("Accept-Encoding");
            string uncompressed = Uncompress(stream);
            var WeatherToken = JsonObject.Parse(uncompressed) ??
                throw new Exception(LanguageTools.GetLocalized("NETTIFY_WEATHER_EXCEPTION_NOWEATHERTOKEN"));
            return FinalizeInstallation(WeatherToken, Unit);
        }

        /// <summary>
        /// Gets current weather info from The Weather Channel
        /// </summary>
        /// <param name="latitude">City latitude</param>
        /// <param name="longitude">City longitude</param>
        /// <param name="APIKey">API key</param>
        /// <param name="Unit">The preferred unit to use</param>
        /// <returns>A class containing properties of weather information</returns>
        public static async Task<WeatherForecastInfo> GetWeatherInfoAsync(double latitude, double longitude, string APIKey, UnitMeasurement Unit = UnitMeasurement.Metric)
        {
            string WeatherURL = $"https://api.weather.com/v3/wx/forecast/daily/15day?geocode={latitude},{longitude}&format=json&language=en-US&units={(Unit == UnitMeasurement.Metric ? 'm' : 'e')}&apiKey={APIKey}";
            return await GetWeatherInfoAsync(WeatherURL, Unit);
        }

        /// <summary>
        /// Gets current weather info from The Weather Channel
        /// </summary>
        /// <param name="WeatherURL">An URL to the weather API request</param>
        /// <param name="Unit">The preferred unit to use</param>
        /// <returns>A class containing properties of weather information</returns>
        internal static async Task<WeatherForecastInfo> GetWeatherInfoAsync(string WeatherURL, UnitMeasurement Unit = UnitMeasurement.Metric)
        {
            Debug.WriteLine("Weather URL: {0} | Unit: {1}", WeatherURL, Unit);

            // Deal with measurements
            Unit = Unit == UnitMeasurement.Kelvin ? UnitMeasurement.Imperial : Unit;

            // Download and parse JSON data
            WeatherDownloader.DefaultRequestHeaders.Add("Accept-Encoding", "gzip");
            var stream = await WeatherDownloader.GetStreamAsync(WeatherURL);
            WeatherDownloader.DefaultRequestHeaders.Remove("Accept-Encoding");
            string uncompressed = Uncompress(stream);
            var WeatherToken = JsonObject.Parse(uncompressed) ??
                throw new Exception(LanguageTools.GetLocalized("NETTIFY_WEATHER_EXCEPTION_NOWEATHERTOKEN"));
            return FinalizeInstallation(WeatherToken, Unit);
        }

        internal static WeatherForecastInfo FinalizeInstallation(JsonNode WeatherToken, UnitMeasurement Unit = UnitMeasurement.Metric)
        {
            // Get the adjusted data
            T Adjust<T>(string dayPartData)
            {
                var dayPartArray = WeatherToken?["daypart"]?[0]?[dayPartData] ??
                    throw new Exception(LanguageTools.GetLocalized("NETTIFY_WEATHER_EXCEPTION_NODAYPART"));
                var adjusted = dayPartArray[0] ?? dayPartArray[1] ??
                    throw new Exception(LanguageTools.GetLocalized("NETTIFY_WEATHER_EXCEPTION_NOADJDAYPART"));
                return (T)adjusted.GetValue<T>();
            }

            // Now, get the necessary variables to get the weather condition info
            long iconCode = Adjust<int>("iconCode");
            WeatherCondition cond = WeatherCondition.Clear;
            switch (iconCode)
            {
                case 0:
                    // Tornado
                    cond = WeatherCondition.Tornado;
                    break;
                case 3:
                    // Strong storms
                    cond = WeatherCondition.HeavyThunderstorm;
                    break;
                case 4:
                    // Thunderstorms
                    cond = WeatherCondition.Thunderstorm;
                    break;
                case 5:
                case 7:
                    // Rain / Snow (5) - Wintry Mix (7)
                    cond = WeatherCondition.RainAndSnow;
                    break;
                case 6:
                    // Rain / Sleet
                    cond = WeatherCondition.Sleet;
                    break;
                case 8:
                    // Freezing Drizzle
                    cond = WeatherCondition.HeavyDrizzle;
                    break;
                case 9:
                    // Drizzle
                    cond = WeatherCondition.Drizzle;
                    break;
                case 10:
                    // Freezing Rain
                    cond = WeatherCondition.FreezingRain;
                    break;
                case 11:
                    // Showers
                    cond = WeatherCondition.ShowerRain;
                    break;
                case 12:
                    // Rain
                    cond = WeatherCondition.LightRain;
                    break;
                case 13:
                    // Flurries
                    cond = WeatherCondition.LightSnow;
                    break;
                case 15:
                case 16:
                    // Blowing/Drifting Snow (15) - Snow (16)
                    cond = WeatherCondition.Snow;
                    break;
                case 17:
                    // Hail
                    cond = WeatherCondition.LightShowerSleet;
                    break;
                case 18:
                    // Sleet
                    cond = WeatherCondition.Sleet;
                    break;
                case 19:
                    // Sandstorm
                    cond = WeatherCondition.Sand;
                    break;
                case 20:
                    // Foggy
                    cond = WeatherCondition.Fog;
                    break;
                case 21:
                    // Haze
                    cond = WeatherCondition.Haze;
                    break;
                case 22:
                    // Smoke
                    cond = WeatherCondition.Smoke;
                    break;
                case 27:
                case 28:
                    // Mostly Cloudy (27, 28)
                    cond = WeatherCondition.MostlyCloudy;
                    break;
                case 29:
                case 30:
                    // Partly Cloudy (29, 30)
                    cond = WeatherCondition.PartlyCloudy;
                    break;
                case 26:
                case 33:
                case 34:
                    // Cloudy (26) - Fair (33, 34)
                    cond = WeatherCondition.FewClouds;
                    break;
                case 35:
                    // Mixed rain and hail
                    cond = WeatherCondition.ModerateRain;
                    break;
                case 37:
                case 38:
                case 47:
                    // Isolated thunderstorms (37) - Scattered thunderstorms (38, 47)
                    cond = WeatherCondition.RaggedThunderstorm;
                    break;
                case 39:
                case 45:
                    // Scattered showers (39, 45)
                    cond = WeatherCondition.RaggedShowerRain;
                    break;
                case 40:
                    // Heavy rain
                    cond = WeatherCondition.HeavyRain;
                    break;
                case 14:
                case 41:
                case 46:
                    // Snow showers (14) - Scattered snow showers (41, 46)
                    cond = WeatherCondition.ShowerSnow;
                    break;
                case 42:
                case 43:
                    // Heavy snow (42) - Blizzard (43)
                    cond = WeatherCondition.HeavySnow;
                    break;
            }

            // Finally, create the forecast info instance
            var temperature = Adjust<double>("temperature");
            var humidity = Adjust<double>("qpf");
            var windSpeed = Adjust<double>("windSpeed");
            var windDirection = Adjust<double>("windDirection");
            var temperatureMeasurement = Unit;
            WeatherForecastInfo WeatherInfo = new(cond, temperatureMeasurement, temperature, humidity, windSpeed, windDirection, (JsonObject)WeatherToken, WeatherServerType.TheWeatherChannel);
            return WeatherInfo;
        }

        /// <summary>
        /// Lists all the available cities
        /// </summary>
        public static Dictionary<string, (double, double)> ListAllCities(string city, string APIKey)
        {
            string WeatherCityListURL = $"https://api.weather.com/v3/location/search?language=en-US&query={city}&format=json&apiKey={APIKey}";
            Stream WeatherCityListDataStream;
            Debug.WriteLine("Weather City List URL: {0}", WeatherCityListURL);

            // Open the stream to the city list URL
            WeatherDownloader.DefaultRequestHeaders.Add("Accept-Encoding", "gzip");
            WeatherCityListDataStream = WeatherDownloader.GetStreamAsync(WeatherCityListURL).Result;
            WeatherDownloader.DefaultRequestHeaders.Remove("Accept-Encoding");
            return FinalizeCityList(WeatherCityListDataStream);
        }

        /// <summary>
        /// Lists all the available cities
        /// </summary>
        public static async Task<Dictionary<string, (double, double)>> ListAllCitiesAsync(string city, string APIKey)
        {
            string WeatherCityListURL = $"https://api.weather.com/v3/location/search?language=en-US&query={city}&format=json&apiKey={APIKey}";
            Stream WeatherCityListDataStream;
            Debug.WriteLine("Weather City List URL: {0}", WeatherCityListURL);

            // Open the stream to the city list URL
            WeatherDownloader.DefaultRequestHeaders.Add("Accept-Encoding", "gzip");
            WeatherCityListDataStream = await WeatherDownloader.GetStreamAsync(WeatherCityListURL);
            WeatherDownloader.DefaultRequestHeaders.Remove("Accept-Encoding");
            return FinalizeCityList(WeatherCityListDataStream);
        }

        internal static Dictionary<string, (double, double)> FinalizeCityList(Stream WeatherCityListDataStream)
        {
            string uncompressed = Uncompress(WeatherCityListDataStream);
            var token = JsonObject.Parse(uncompressed) ??
                throw new Exception(LanguageTools.GetLocalized("NETTIFY_WEATHER_EXCEPTION_NOCITYLIST"));

            // Get the addresses, the latitudes, and the longitudes
            var loc = token["location"] ??
                throw new Exception(LanguageTools.GetLocalized("NETTIFY_WEATHER_EXCEPTION_NOLOCATION"));
            var addresses = (JsonArray?)loc["address"] ??
                throw new Exception(LanguageTools.GetLocalized("NETTIFY_WEATHER_EXCEPTION_NOADDRESSES"));
            var latitudes = (JsonArray?)loc["latitude"] ??
                throw new Exception(LanguageTools.GetLocalized("NETTIFY_WEATHER_EXCEPTION_NOLATS"));
            var longitudes = (JsonArray?)loc["longitude"] ??
                throw new Exception(LanguageTools.GetLocalized("NETTIFY_WEATHER_EXCEPTION_NOLONGS"));
            Debug.Assert(addresses.Count == latitudes.Count && addresses.Count == longitudes.Count && latitudes.Count == longitudes.Count);

            // Put needed data
            Dictionary<string, (double, double)> cities = [];
            for (int i = 0; i < addresses.Count; i++)
            {
                var address = (string?)addresses[i] ?? "";
                var lat = (double)(latitudes[i] ?? 0);
                var lng = (double)(longitudes[i] ?? 0);
                cities.Add(address, (lat, lng));
            }

            // Return list
            return cities;
        }

        internal static string Uncompress(Stream compressedDataStream)
        {
            GZipStream compressedData;
            var compressedUncompressed = new List<byte>();
            int compressedReadByte = 0;

            // Parse the weather list JSON. Since the output is gzipped, we'll have to uncompress it using stream, since the city list
            // is large anyways. This saves you from downloading full 45+ MB of text.
            compressedData = new GZipStream(compressedDataStream, CompressionMode.Decompress, false);
            while (compressedReadByte != -1)
            {
                compressedReadByte = compressedData.ReadByte();
                if (compressedReadByte != -1)
                    compressedUncompressed.Add((byte)compressedReadByte);
            }

            return Encoding.Default.GetString([.. compressedUncompressed]);
        }
    }
}
