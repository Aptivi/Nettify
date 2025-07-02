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
using System.Net.Http;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace Nettify.Weather
{
    /// <summary>
    /// The forecast tools (OpenWeatherMap)
    /// </summary>
    public static class WeatherForecastOwm
    {
        internal static HttpClient WeatherDownloader = new();

        /// <summary>
        /// Gets current weather info from OpenWeatherMap
        /// </summary>
        /// <param name="CityID">City ID</param>
        /// <param name="APIKey">API key</param>
        /// <param name="Unit">The preferred unit to use</param>
        /// <returns>A class containing properties of weather information</returns>
        public static WeatherForecastInfo GetWeatherInfo(long CityID, string APIKey, UnitMeasurement Unit = UnitMeasurement.Metric)
        {
            string WeatherURL = $"https://api.openweathermap.org/data/2.5/weather?id={CityID}&appid={APIKey}";
            return GetWeatherInfo(WeatherURL, Unit);
        }

        /// <summary>
        /// Gets current weather info from OpenWeatherMap
        /// </summary>
        /// <param name="CityName">City name</param>
        /// <param name="APIKey">API Key</param>
        /// <param name="Unit">The preferred unit to use</param>
        /// <returns>A class containing properties of weather information</returns>
        public static WeatherForecastInfo GetWeatherInfo(string CityName, string APIKey, UnitMeasurement Unit = UnitMeasurement.Metric)
        {
            string WeatherURL = $"https://api.openweathermap.org/data/2.5/weather?q={CityName}&appid={APIKey}";
            return GetWeatherInfo(WeatherURL, Unit);
        }

        /// <summary>
        /// Gets current weather info from OpenWeatherMap
        /// </summary>
        /// <param name="WeatherURL">An URL to the weather API request</param>
        /// <param name="Unit">The preferred unit to use</param>
        /// <returns>A class containing properties of weather information</returns>
        internal static WeatherForecastInfo GetWeatherInfo(string WeatherURL, UnitMeasurement Unit = UnitMeasurement.Metric)
        {
            string WeatherData;
            Debug.WriteLine("Weather URL: {0} | Unit: {1}", WeatherURL, Unit);

            // Deal with measurements
            if (Unit == UnitMeasurement.Imperial)
                WeatherURL += "&units=imperial";
            else
                WeatherURL += "&units=metric";

            // Download and parse JSON data
            WeatherData = WeatherDownloader.GetStringAsync(WeatherURL).Result;
            JsonNode WeatherToken = JsonObject.Parse(WeatherData) ??
                throw new Exception(LanguageTools.GetLocalized("NETTIFY_WEATHER_EXCEPTION_NOWEATHERTOKEN"));
            return FinalizeInstallation(WeatherToken, Unit);
        }

        /// <summary>
        /// Gets current weather info from OpenWeatherMap
        /// </summary>
        /// <param name="CityID">City ID</param>
        /// <param name="APIKey">API key</param>
        /// <param name="Unit">The preferred unit to use</param>
        /// <returns>A class containing properties of weather information</returns>
        public static async Task<WeatherForecastInfo> GetWeatherInfoAsync(long CityID, string APIKey, UnitMeasurement Unit = UnitMeasurement.Metric)
        {
            string WeatherURL = $"https://api.openweathermap.org/data/2.5/weather?id={CityID}&appid={APIKey}";
            return await GetWeatherInfoAsync(WeatherURL, Unit);
        }

        /// <summary>
        /// Gets current weather info from OpenWeatherMap
        /// </summary>
        /// <param name="CityName">City name</param>
        /// <param name="APIKey">API Key</param>
        /// <param name="Unit">The preferred unit to use</param>
        /// <returns>A class containing properties of weather information</returns>
        public static async Task<WeatherForecastInfo> GetWeatherInfoAsync(string CityName, string APIKey, UnitMeasurement Unit = UnitMeasurement.Metric)
        {
            string WeatherURL = $"https://api.openweathermap.org/data/2.5/weather?q={CityName}&appid={APIKey}";
            return await GetWeatherInfoAsync(WeatherURL, Unit);
        }

        /// <summary>
        /// Gets current weather info from OpenWeatherMap
        /// </summary>
        /// <param name="WeatherURL">An URL to the weather API request</param>
        /// <param name="Unit">The preferred unit to use</param>
        /// <returns>A class containing properties of weather information</returns>
        internal static async Task<WeatherForecastInfo> GetWeatherInfoAsync(string WeatherURL, UnitMeasurement Unit = UnitMeasurement.Metric)
        {
            string WeatherData;
            Debug.WriteLine("Weather URL: {0} | Unit: {1}", WeatherURL, Unit);

            // Deal with measurements
            if (Unit == UnitMeasurement.Imperial)
                WeatherURL += "&units=imperial";
            else
                WeatherURL += "&units=metric";

            // Download and parse JSON data
            WeatherData = await WeatherDownloader.GetStringAsync(WeatherURL);
            JsonNode WeatherToken = JsonObject.Parse(WeatherData) ??
                throw new Exception(LanguageTools.GetLocalized("NETTIFY_WEATHER_EXCEPTION_NOWEATHERTOKEN"));
            return FinalizeInstallation(WeatherToken, Unit);
        }

        internal static WeatherForecastInfo FinalizeInstallation(JsonNode WeatherToken, UnitMeasurement Unit = UnitMeasurement.Metric)
        {
            var weather = (WeatherCondition?)WeatherToken["weather"]?[0]?["id"]?.GetValue<int>() ?? WeatherCondition.Clear;
            var temperature = (double)(WeatherToken["main"]?["temp"]?.GetValue<double>() ?? 0);
            var humidity = (double)(WeatherToken["main"]?["humidity"]?.GetValue<double>() ?? 0);
            var windSpeed = (double)(WeatherToken["wind"]?["speed"]?.GetValue<double>() ?? 0);
            var windDirection = (double)(WeatherToken["wind"]?["deg"]?.GetValue<double>() ?? 0);
            var temperatureMeasurement = Unit;
            WeatherForecastInfo WeatherInfo = new(weather, temperatureMeasurement, temperature, humidity, windSpeed, windDirection, (JsonObject)WeatherToken, WeatherServerType.OpenWeatherMap);
            return WeatherInfo;
        }

        /// <summary>
        /// Lists all the available cities
        /// </summary>
        public static Dictionary<long, string> ListAllCities()
        {
            string WeatherCityListURL = $"https://bulk.openweathermap.org/sample/city.list.json.gz";
            Stream WeatherCityListDataStream;
            Debug.WriteLine("Weather City List URL: {0}", WeatherCityListURL);

            // Open the stream to the city list URL
            WeatherCityListDataStream = WeatherDownloader.GetStreamAsync(WeatherCityListURL).Result;
            return FinalizeCityList(WeatherCityListDataStream);
        }

        /// <summary>
        /// Lists all the available cities
        /// </summary>
        public static async Task<Dictionary<long, string>> ListAllCitiesAsync()
        {
            string WeatherCityListURL = $"https://bulk.openweathermap.org/sample/city.list.json.gz";
            Stream WeatherCityListDataStream;
            Debug.WriteLine("Weather City List URL: {0}", WeatherCityListURL);

            // Open the stream to the city list URL
            WeatherCityListDataStream = await WeatherDownloader.GetStreamAsync(WeatherCityListURL);
            return FinalizeCityList(WeatherCityListDataStream);
        }

        internal static Dictionary<long, string> FinalizeCityList(Stream WeatherCityListDataStream)
        {
            var WeatherCityList = new Dictionary<long, string>();
            string uncompressed = WeatherForecast.Uncompress(WeatherCityListDataStream);
            JsonArray WeatherToken = (JsonArray?)JsonArray.Parse(uncompressed) ??
                throw new Exception(LanguageTools.GetLocalized("NETTIFY_WEATHER_EXCEPTION_NOWEATHERTOKEN"));

            // Put needed data to the class
            foreach (var WeatherCityToken in WeatherToken)
            {
                long cityId = (long)(WeatherCityToken?["id"] ?? 0);
                string cityName = (string?)WeatherCityToken?["name"] ?? "";
                if (!WeatherCityList.ContainsKey(cityId))
                    WeatherCityList.Add(cityId, cityName);
            }

            // Return list
            return WeatherCityList;
        }
    }
}
