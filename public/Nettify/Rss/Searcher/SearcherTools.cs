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
using Nettify.Rss.Instance;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace Nettify.Rss.Searcher
{
    /// <summary>
    /// RSS feed searcher tools powered by Feedly
    /// </summary>
    public static class SearcherTools
    {
        private static readonly HttpClient client = new();

        /// <summary>
        /// Gets RSS feed list from search query (up to 100,000 feeds)
        /// </summary>
        /// <param name="searchTerm">Search term</param>
        /// <param name="feeds">Number of feeds to fetch</param>
        /// <returns>List of RSS feeds</returns>
        public static SearcherInstance[] GetRssFeeds(string searchTerm, int feeds = 100000)
        {
            string feedsJson = InitializeFeedJsonAsync(searchTerm, feeds).Result;
            return DeserializeFeedJson(feedsJson);
        }

        /// <summary>
        /// Gets RSS feed list from search query asynchronously (up to 100,000 feeds)
        /// </summary>
        /// <param name="searchTerm">Search term</param>
        /// <param name="feeds">Number of feeds to fetch</param>
        /// <returns>List of RSS feeds</returns>
        public static async Task<SearcherInstance[]> GetRssFeedsAsync(string searchTerm, int feeds = 100000)
        {
            string feedsJson = await InitializeFeedJsonAsync(searchTerm, feeds);
            return DeserializeFeedJson(feedsJson);
        }

        /// <summary>
        /// Gets a working <see cref="RSSFeed"/> instance from one of the search results
        /// </summary>
        /// <param name="feed">Feed obtained from <see cref="GetRssFeeds(string, int)"/> or <see cref="GetRssFeedsAsync(string, int)"/></param>
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
        /// <param name="feed">Feed obtained from <see cref="GetRssFeeds(string, int)"/> or <see cref="GetRssFeedsAsync(string, int)"/></param>
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

        private static async Task<string> InitializeFeedJsonAsync(string searchTerm, int feeds)
        {
            if (feeds < 1)
                feeds = 1;
            if (feeds > 100000)
                feeds = 100000;
            if (string.IsNullOrWhiteSpace(searchTerm))
                searchTerm = "tech";
            Uri feedsUri = new("https://cloud.feedly.com/v3/search/feeds/?n=" + feeds + "&query=" + searchTerm);
            string response = await client.GetStringAsync(feedsUri);
            return response;
        }

        private static SearcherInstance[] DeserializeFeedJson(string feedsJson)
        {
            var token = JsonObject.Parse(feedsJson) ??
                throw new RSSException(LanguageTools.GetLocalized("NETTIFY_RSS_EXCEPTION_NOFEEDS"));
            var feedsToken = token["results"];
            var instances = JsonSerializer.Deserialize<SearcherInstance[]>(feedsToken) ??
                throw new RSSException(LanguageTools.GetLocalized("NETTIFY_RSS_EXCEPTION_SEARCHRESULTDESERIALIZE"));
            return instances;
        }
    }
}
