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

using Nettify.Radio;
using System;

namespace Nettify.Demo.Fixtures.Cases
{
    internal class ShoutCastStats : IFixture
    {
        public string FixtureID => "ShoutCastStats";
        public void RunFixture()
		{
			// Prompt for server address and port
			Console.Write("Enter server address for SHOUTcast: ");
			string address = Console.ReadLine();
			Console.Write("Enter server port for SHOUTcast: ");
			string port = Console.ReadLine();

			// Check the port number
			bool isNumeric = int.TryParse(port, out int portInt);
			if (!isNumeric)
			{
				Console.WriteLine("Invalid port number.");
				return;
			}

			try
			{
				// Parse the address and the port
				if (!address.Contains(Uri.SchemeDelimiter))
					address = Uri.UriSchemeHttps + Uri.SchemeDelimiter + address;
				bool UseSecure = address.Contains("https://");
				Console.WriteLine("Getting information about stream {0}:{1}...", address, portInt);

				// Get the stream info
				ShoutcastServer ParsedServer = new(address, portInt, UseSecure);
				ParsedServer.Refresh();

				// Print the server information
				Console.WriteLine($"Host: {ParsedServer.ServerHost}");
				Console.WriteLine($"Port: {ParsedServer.ServerPort}");
				Console.WriteLine($"URL: {ParsedServer.ServerHostFull}");
				Console.WriteLine($"Secure protocol: {ParsedServer.ServerHttps}");
				Console.WriteLine($"Server version: {ParsedServer.ServerVersion}");
				Console.WriteLine($"Total streams: {ParsedServer.TotalStreams}");
				Console.WriteLine($"Active streams: {ParsedServer.ActiveStreams}");
				Console.WriteLine($"Current listeners: {ParsedServer.CurrentListeners}");
				Console.WriteLine($"Peak listeners: {ParsedServer.PeakListeners}");
				Console.WriteLine($"Max listeners: {ParsedServer.MaxListeners}");
				Console.WriteLine($"Unique listeners: {ParsedServer.UniqueListeners}");
				Console.WriteLine($"Average time (s): {ParsedServer.AverageTime}");
				Console.WriteLine($"Average time (span): {ParsedServer.AverageTimeSpan}");

				// Print the stream information
				foreach (StreamInfo stream in ParsedServer.Streams)
				{
					Console.WriteLine("---------------------------------------");
					Console.WriteLine($"Stream ID: {stream.StreamId}");
					Console.WriteLine($"Stream title: {stream.StreamTitle}");
					Console.WriteLine($"Current song: {stream.SongTitle}");
					Console.WriteLine($"Current listeners: {stream.CurrentListeners}");
					Console.WriteLine($"Peak listeners: {stream.PeakListeners}");
					Console.WriteLine($"Max listeners: {stream.MaxListeners}");
					Console.WriteLine($"Unique listeners: {stream.UniqueListeners}");
					Console.WriteLine($"Average time (s): {stream.AverageTime}");
					Console.WriteLine($"Average time (span): {stream.AverageTimeSpan}");
					Console.WriteLine($"Stream genre: {stream.StreamGenre}");
					Console.WriteLine($"Stream homepage: {stream.StreamHomepage}");
					Console.WriteLine($"Stream hits: {stream.StreamHits}");
					Console.WriteLine($"Stream status: {stream.StreamStatus}");
					Console.WriteLine($"Backup status: {stream.BackupStatus}");
					Console.WriteLine($"Stream listed: {stream.StreamListed}");
					Console.WriteLine($"Stream path: {stream.StreamPath}");
					Console.WriteLine($"Stream uptime (s): {stream.StreamUptime}");
					Console.WriteLine($"Stream uptime (span): {stream.StreamUptimeSpan}");
					Console.WriteLine($"Stream bitrate: {stream.BitRate}");
					Console.WriteLine($"Stream sample rate: {stream.SampleRate}");
					Console.WriteLine($"MIME info: {stream.MimeInfo}");
				}
				Console.WriteLine("---------------------------------------");
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error trying to get information for the server: {0}", ex.Message);
				Console.WriteLine(ex.StackTrace);
				if (ex.InnerException != null)
				{
					Console.WriteLine("If this is a problem in this component of Nettify, report it: {0}", ex.InnerException.Message);
					Console.WriteLine(ex.InnerException.StackTrace);
				}
			}
		}
    }
}
