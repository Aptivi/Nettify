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

using Nettify.Weather;
using System;

namespace Nettify.Demo.Fixtures.Cases
{
    internal class ForecastOwm : IFixture
    {
        public string FixtureID => "ForecastOwm";
        public void RunFixture()
        {
			string ApiKey;
			string StringID;
            WeatherForecastInfo forecastInfo;
			bool IsNumeric;

			// ID or name
			Console.Write("Enter city ID or name: ");
			StringID = Console.ReadLine() ?? "";
			IsNumeric = long.TryParse(StringID, out long FinalID);

			// API key
			Console.Write("Enter API key: ");
			ApiKey = Console.ReadLine() ?? "";

			// Get weather info
			if (IsNumeric)
				forecastInfo = WeatherForecastOwm.GetWeatherInfo(FinalID, ApiKey, UnitMeasurement.Metric);
			else
				forecastInfo = WeatherForecastOwm.GetWeatherInfo(StringID, ApiKey, UnitMeasurement.Metric);

			// Print the weather information
			Console.WriteLine("Weather State: " + forecastInfo.Weather);
			Console.WriteLine("Temperature: " + forecastInfo.Temperature);
			Console.WriteLine("Humidity: " + forecastInfo.Humidity);
			Console.WriteLine("Wind Speed: " + forecastInfo.WindSpeed);
			Console.WriteLine("Wind Direction: " + forecastInfo.WindDirection);
		}
    }
}
