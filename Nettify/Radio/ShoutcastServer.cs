﻿//
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
using Newtonsoft.Json.Linq;
using HtmlAgilityPack;
using System.Net.Http;
using System.Threading.Tasks;

namespace Nettify.Radio
{
    /// <summary>
    /// A Shoutcast server
    /// </summary>
    public class ShoutcastServer
    {
        private ShoutcastVersion serverVersion;
        private int totalStreams;
        private int activeStreams;
        private int currentListeners;
        private int peakListeners;
        private int maxListeners;
        private int uniqueListeners;
        private int averageTime;
        private readonly List<StreamInfo> streams = [];
        internal JToken streamToken;
        internal HtmlDocument streamHtmlToken = new();
        internal HttpClient client = new();

        /// <summary>
        /// Server IP address
        /// </summary>
        public string ServerHost { get; }
        /// <summary>
        /// Server port
        /// </summary>
        public int ServerPort { get; }
        /// <summary>
        /// Server IP address with port
        /// </summary>
        public string ServerHostFull => ServerHost + ":" + ServerPort;
        /// <summary>
        /// Whether the Shoutcast server is using HTTPS or not
        /// </summary>
        public bool ServerHttps { get; }
        /// <summary>
        /// Server version (1.x, 2.x)
        /// </summary>
        public ShoutcastVersion ServerVersion => serverVersion;
        /// <summary>
        /// Total number of streams in the server
        /// </summary>
        public int TotalStreams => totalStreams;
        /// <summary>
        /// Active streams in the server
        /// </summary>
        public int ActiveStreams => activeStreams;
        /// <summary>
        /// How many people are listening to the server at this time?
        /// </summary>
        public int CurrentListeners => currentListeners;
        /// <summary>
        /// How many listeners did the server ever get at peak times?
        /// </summary>
        public int PeakListeners => peakListeners;
        /// <summary>
        /// How many people can listen to the server?
        /// </summary>
        public int MaxListeners => maxListeners;
        /// <summary>
        /// How many unique listeners are there?
        /// </summary>
        public int UniqueListeners => uniqueListeners;
        /// <summary>
        /// Average time on any active listener connections in seconds
        /// </summary>
        public int AverageTime => averageTime;
        /// <summary>
        /// Average time on any active listener connections in the time span
        /// </summary>
        public TimeSpan AverageTimeSpan => TimeSpan.FromSeconds(AverageTime);
        /// <summary>
        /// Available streams and their statistics
        /// </summary>
        public List<StreamInfo> Streams => streams;

        /// <summary>
        /// Connects to the Shoutcast server and gets the information
        /// </summary>
        /// <param name="serverHost">Server host name</param>
        /// <param name="serverPort">Server port</param>
        /// <param name="useHttps">Whether to use the HTTPS protocol or not</param>
        public ShoutcastServer(string serverHost, int serverPort, bool useHttps)
        {
            // Check to see if we're dealing with the secure Shoutcast server
            ServerHost = serverHost;
            if (useHttps)
            {
                if (!serverHost.Contains(Uri.SchemeDelimiter))
                    ServerHost = $"https://{serverHost}";
            }
            else
            {
                if (!serverHost.Contains(Uri.SchemeDelimiter))
                    ServerHost = $"http://{serverHost}";
            }
            ServerHttps = useHttps;

            // Install the values initially
            ServerPort = serverPort;
            serverVersion = ShoutcastVersion.v2;
        }

        /// <summary>
        /// Refreshes the statistics
        /// </summary>
        public void Refresh()
        {
            try
            {
                InitializeStatsAsync().Wait();

                // Determine version of Shoutcast
                if (serverVersion == ShoutcastVersion.v1)
                    FinalizeShoutcastV1();
                else
                    FinalizeShoutcastV2();
            }
            catch (Exception ex)
            {
                throw new ShoutcastServerException($"Failed to parse Shoutcast server {ServerHost}. More information can be found in the inner exception.", ex);
            }
        }

        /// <summary>
        /// Refreshes the statistics
        /// </summary>
        public async Task RefreshAsync()
        {
            try
            {
                await InitializeStatsAsync();

                // Determine version of Shoutcast
                if (serverVersion == ShoutcastVersion.v1)
                    FinalizeShoutcastV1();
                else
                    FinalizeShoutcastV2();
            }
            catch (Exception ex)
            {
                throw new ShoutcastServerException($"Failed to parse Shoutcast server {ServerHost}. More information can be found in the inner exception.", ex);
            }
        }

        internal async Task InitializeStatsAsync()
        {
            // Use the full address to download the statistics. Note that Shoutcast v2 streams will use the /statistics directory, which provides
            // more information than /7.html. If we're dealing with the first version, or if /statistics is disabled for some reason, fallback to
            // /7.html
            Uri statisticsUri = new(ServerHostFull + "/statistics?json=1");
            Uri fallbackUri = new(ServerHostFull + "/7.html");
            string serverResponse = await client.GetStringAsync(statisticsUri);

            // Shoutcast v1.x doesn't have /statistics...
            if (serverResponse.Contains("Invalid resource"))
            {
                // Detected v1. Fallback to /7.html
                serverVersion = ShoutcastVersion.v1;
                serverResponse = await client.GetStringAsync(fallbackUri);
                streamHtmlToken.LoadHtml(serverResponse);
            }
            else
                streamToken = JToken.Parse(serverResponse);
        }

        internal void FinalizeShoutcastV1()
        {
            // Shoutcast version v1.x, so use the html fallback token. Response values are as follows:
            // currentlisteners,streamstatus(S),peaklisteners,maxlisteners,uniquelisteners,bitrate(S),songtitle(S)
            // First, deal with the server settings.
            string[] response = streamHtmlToken.DocumentNode.SelectSingleNode("body").InnerText.Split(',');
            currentListeners = Convert.ToInt32(response[0]);
            peakListeners = Convert.ToInt32(response[2]);
            maxListeners = Convert.ToInt32(response[3]);
            uniqueListeners = Convert.ToInt32(response[4]);

            // Then, deal with the stream settings
            StreamInfo streamInfo = new(this, null);
            streams.Clear();
            streams.Add(streamInfo);
        }

        internal void FinalizeShoutcastV2()
        {
            // Shoutcast version v2.x, so use the JToken.
            // Use all the keys in the first object except the "streams" and "version", where we'd later use the former in StreamInfo to install
            // all the streams into the new class instance.
            totalStreams = (int)streamToken["totalstreams"];
            activeStreams = (int)streamToken["activestreams"];
            currentListeners = (int)streamToken["currentlisteners"];
            peakListeners = (int)streamToken["peaklisteners"];
            maxListeners = (int)streamToken["maxlisteners"];
            uniqueListeners = (int)streamToken["uniquelisteners"];
            averageTime = (int)streamToken["averagetime"];

            // Now, deal with the stream settings.
            streams.Clear();
            foreach (JToken stream in streamToken["streams"])
            {
                StreamInfo streamInfo = new(this, stream);
                streams.Add(streamInfo);
            }
        }
    }
}
