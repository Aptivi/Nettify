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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

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
            string WeatherURL = $"http://api.weather.com/v3/wx/forecast/daily/15day?geocode={latitude},{longitude}&format=json&language=en-US&units={(Unit == UnitMeasurement.Metric ? 'm' : 'e')}&apiKey={APIKey}";
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
            string WeatherData;
            JToken WeatherToken;
            Debug.WriteLine("Weather URL: {0} | Unit: {1}", WeatherURL, Unit);

            // Deal with measurements
            Unit = Unit == UnitMeasurement.Kelvin ? UnitMeasurement.Imperial : Unit;

            // Download and parse JSON data
            WeatherData = WeatherDownloader.GetStringAsync(WeatherURL).Result;
            WeatherToken = JToken.Parse(WeatherData);
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
            string WeatherURL = $"http://api.weather.com/v3/wx/forecast/daily/15day?geocode={latitude},{longitude}&format=json&language=en-US&units={(Unit == UnitMeasurement.Metric ? 'm' : 'e')}&apiKey={APIKey}";
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
            string WeatherData;
            JToken WeatherToken;
            Debug.WriteLine("Weather URL: {0} | Unit: {1}", WeatherURL, Unit);

            // Deal with measurements
            Unit = Unit == UnitMeasurement.Kelvin ? UnitMeasurement.Imperial : Unit;

            // Download and parse JSON data
            WeatherData = await WeatherDownloader.GetStringAsync(WeatherURL);
            WeatherToken = JToken.Parse(WeatherData);
            return FinalizeInstallation(WeatherToken, Unit);
        }

        internal static WeatherForecastInfo FinalizeInstallation(JToken WeatherToken, UnitMeasurement Unit = UnitMeasurement.Metric)
        {
            WeatherForecastInfo WeatherInfo = new()
            {
                // Put needed data to the class
                // TODO: Handle weather condition translation
                Weather = (WeatherCondition)WeatherToken.SelectToken("weather").First.SelectToken("id").ToObject(typeof(WeatherCondition)),
                Temperature = (double)WeatherToken["daypart"]["temperature"][1].ToObject(typeof(double)),
                Humidity = (double)WeatherToken["daypart"]["humidity"][1].ToObject(typeof(double)),
                WindSpeed = (double)WeatherToken["daypart"]["windSpeed"][1].ToObject(typeof(double)),
                WindDirection = (double)WeatherToken["daypart"]["windDirection"][1].ToObject(typeof(double)),
                TemperatureMeasurement = Unit
            };
            return WeatherInfo;
        }

        /// <summary>
        /// Lists all the available cities
        /// </summary>
        public static Dictionary<string, (double, double)> ListAllCities(string city, string APIKey)
        {
            string WeatherCityListURL = $"http://api.weather.com/v3/location/search?language=en-US&query={city}&format=json&apiKey={APIKey}";
            Stream WeatherCityListDataStream;
            Debug.WriteLine("Weather City List URL: {0}", WeatherCityListURL);

            // Open the stream to the city list URL
            WeatherCityListDataStream = WeatherDownloader.GetStreamAsync(WeatherCityListURL).Result;
            return FinalizeCityList(WeatherCityListDataStream);
        }

        /// <summary>
        /// Lists all the available cities
        /// </summary>
        public static async Task<Dictionary<string, (double, double)>> ListAllCitiesAsync(string city, string APIKey)
        {
            string WeatherCityListURL = $"http://api.weather.com/v3/location/search?language=en-US&query={city}&format=json&apiKey={APIKey}";
            Stream WeatherCityListDataStream;
            Debug.WriteLine("Weather City List URL: {0}", WeatherCityListURL);

            // Open the stream to the city list URL
            WeatherCityListDataStream = await WeatherDownloader.GetStreamAsync(WeatherCityListURL);
            return FinalizeCityList(WeatherCityListDataStream);
        }

        internal static Dictionary<string, (double, double)> FinalizeCityList(Stream WeatherCityListDataStream)
        {
            // Get the token
            var reader = new StreamReader(WeatherCityListDataStream);
            string json = reader.ReadToEnd();
            var token = JToken.Parse(json);

            // Get the addresses, the latitudes, and the longitudes
            var loc = token["location"];
            var addresses = (JArray)loc["address"];
            var latitudes = (JArray)loc["latitude"];
            var longitudes = (JArray)loc["longitude"];
            Debug.Assert(addresses.Count == latitudes.Count && addresses.Count == longitudes.Count && latitudes.Count == longitudes.Count);

            // Put needed data
            Dictionary<string, (double, double)> cities = [];
            for (int i = 0; i < addresses.Count; i++)
            {
                var address = (string)addresses[i];
                var lat = (double)latitudes[i];
                var lng = (double)longitudes[i];
                cities.Add(address, (lat, lng));
            }

            // Return list
            return cities;
        }
    }
}
