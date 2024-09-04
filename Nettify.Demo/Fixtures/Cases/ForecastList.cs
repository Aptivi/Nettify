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

using Nettify.Weather;
using System;

namespace Nettify.Demo.Fixtures.Cases
{
    internal class ForecastList : IFixture
    {
        public string FixtureID => "ForecastList";
        public void RunFixture()
        {
            // City
            Console.Write("Enter city name: ");
            string city = Console.ReadLine() ?? "";

            // API key
            Console.Write("Enter TWC API key: ");
            string ApiKey = Console.ReadLine() ?? "";

            // List all cities
            var longsLats = WeatherForecast.ListAllCities(city, ApiKey);
            foreach (var longLat in longsLats)
            {
                string name = longLat.Key;
                (double latitude, double longitude) = longLat.Value;
                Console.WriteLine($"Name: {name,-55}\t\tlat: {latitude,-10}\tlng: {longitude,-10}");
            }
		}
    }
}
