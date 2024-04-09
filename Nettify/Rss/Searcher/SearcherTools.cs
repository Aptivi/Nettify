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

using Nettify.Rss.Instance;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Nettify.Rss.Searcher
{
    /// <summary>
    /// RSS feed searcher tools powered by Feedly
    /// </summary>
    public static class SearcherTools
    {
        private static readonly HttpClient client = new();
        private const string feedlySearchLink = "https://cloud.feedly.com/v3/search/feeds/?n=100000&query=";

        /// <summary>
        /// Gets RSS feed list from search query (up to 100,000 feeds)
        /// </summary>
        /// <param name="searchTerm">Search term</param>
        /// <returns>List of RSS feeds</returns>
        public static SearcherInstance[] GetRssFeeds(string searchTerm)
        {
            string feedsJson = InitializeFeedJsonAsync(searchTerm).Result;
            return DeserializeFeedJson(feedsJson);
        }

        /// <summary>
        /// Gets RSS feed list from search query asynchronously (up to 100,000 feeds)
        /// </summary>
        /// <param name="searchTerm">Search term</param>
        /// <returns>List of RSS feeds</returns>
        public static async Task<SearcherInstance[]> GetRssFeedsAsync(string searchTerm)
        {
            string feedsJson = await InitializeFeedJsonAsync(searchTerm);
            return DeserializeFeedJson(feedsJson);
        }

        /// <summary>
        /// Gets a working <see cref="RSSFeed"/> instance from one of the search results
        /// </summary>
        /// <param name="feed">Feed obtained from <see cref="GetRssFeeds(string)"/> or <see cref="GetRssFeedsAsync(string)"/></param>
        /// <returns>A working RSS feed instance</returns>
        public static RSSFeed GetFeedFromSearcher(SearcherInstance feed)
        {
            // Strip the "feed/" prefix
            string id = feed.FeedId ?? feed.Id;
            id = id.Substring(5);

            // Now, try to get the RSS feed
            var rssFeed = new RSSFeed(id, RSSFeedType.Infer);
            rssFeed.Refresh();

            // If successful, return it
            return rssFeed;
        }

        /// <summary>
        /// Gets a working <see cref="RSSFeed"/> instance from one of the search results asynchronously
        /// </summary>
        /// <param name="feed">Feed obtained from <see cref="GetRssFeeds(string)"/> or <see cref="GetRssFeedsAsync(string)"/></param>
        /// <returns>A working RSS feed instance</returns>
        public static async Task<RSSFeed> GetFeedFromSearcherAsync(SearcherInstance feed)
        {
            // Strip the "feed/" prefix
            string id = feed.FeedId ?? feed.Id;
            id = id.Substring(5);

            // Now, try to get the RSS feed
            var rssFeed = new RSSFeed(id, RSSFeedType.Infer);
            await rssFeed.RefreshAsync();

            // If successful, return it
            return rssFeed;
        }

        private static async Task<string> InitializeFeedJsonAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                searchTerm = "tech";
            Uri feedsUri = new(feedlySearchLink + searchTerm);
            string response = await client.GetStringAsync(feedsUri);
            return response;
        }

        private static SearcherInstance[] DeserializeFeedJson(string feedsJson)
        {
            var token = JObject.Parse(feedsJson);
            var feedsToken = (JArray)token["results"];
            var instances = JsonConvert.DeserializeObject<SearcherInstance[]>(feedsToken.ToString());
            return instances;
        }
    }
}
