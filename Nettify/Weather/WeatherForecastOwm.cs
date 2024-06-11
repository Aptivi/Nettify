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
            string WeatherURL = $"http://api.openweathermap.org/data/2.5/weather?id={CityID}&appid={APIKey}";
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
            string WeatherURL = $"http://api.openweathermap.org/data/2.5/weather?q={CityName}&appid={APIKey}";
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
            JToken WeatherToken;
            Debug.WriteLine("Weather URL: {0} | Unit: {1}", WeatherURL, Unit);

            // Deal with measurements
            if (Unit == UnitMeasurement.Imperial)
                WeatherURL += "&units=imperial";
            else
                WeatherURL += "&units=metric";

            // Download and parse JSON data
            WeatherData = WeatherDownloader.GetStringAsync(WeatherURL).Result;
            WeatherToken = JToken.Parse(WeatherData);
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
            string WeatherURL = $"http://api.openweathermap.org/data/2.5/weather?id={CityID}&appid={APIKey}";
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
            string WeatherURL = $"http://api.openweathermap.org/data/2.5/weather?q={CityName}&appid={APIKey}";
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
            JToken WeatherToken;
            Debug.WriteLine("Weather URL: {0} | Unit: {1}", WeatherURL, Unit);

            // Deal with measurements
            if (Unit == UnitMeasurement.Imperial)
                WeatherURL += "&units=imperial";
            else
                WeatherURL += "&units=metric";

            // Download and parse JSON data
            WeatherData = await WeatherDownloader.GetStringAsync(WeatherURL);
            WeatherToken = JToken.Parse(WeatherData);
            return FinalizeInstallation(WeatherToken, Unit);
        }

        internal static WeatherForecastInfo FinalizeInstallation(JToken WeatherToken, UnitMeasurement Unit = UnitMeasurement.Metric)
        {
            var weather = (WeatherCondition)WeatherToken.SelectToken("weather").First.SelectToken("id").ToObject(typeof(WeatherCondition));
            var temperature = (double)WeatherToken.SelectToken("main").SelectToken("temp").ToObject(typeof(double));
            var humidity = (double)WeatherToken.SelectToken("main").SelectToken("humidity").ToObject(typeof(double));
            var windSpeed = (double)WeatherToken.SelectToken("wind").SelectToken("speed").ToObject(typeof(double));
            var windDirection = (double)WeatherToken.SelectToken("wind").SelectToken("deg").ToObject(typeof(double));
            var temperatureMeasurement = Unit;
            WeatherForecastInfo WeatherInfo = new(weather, temperatureMeasurement, temperature, humidity, windSpeed, windDirection, WeatherToken, WeatherServerType.OpenWeatherMap);
            return WeatherInfo;
        }

        /// <summary>
        /// Lists all the available cities
        /// </summary>
        public static Dictionary<long, string> ListAllCities()
        {
            string WeatherCityListURL = $"http://bulk.openweathermap.org/sample/city.list.json.gz";
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
            string WeatherCityListURL = $"http://bulk.openweathermap.org/sample/city.list.json.gz";
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
            JToken WeatherToken = JToken.Parse(uncompressed);

            // Put needed data to the class
            foreach (JToken WeatherCityToken in WeatherToken)
            {
                long cityId = (long)WeatherCityToken["id"];
                string cityName = (string)WeatherCityToken["name"];
                if (!WeatherCityList.ContainsKey(cityId))
                    WeatherCityList.Add(cityId, cityName);
            }

            // Return list
            return WeatherCityList;
        }
    }
}
