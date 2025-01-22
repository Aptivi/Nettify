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

using System.Diagnostics;
using System.Text.Json.Serialization;

namespace Nettify.Rss.Searcher
{
    /// <summary>
    /// Feedly search result class instance
    /// </summary>
    [DebuggerDisplay("{title}: {description} [{website} | {id}]")]
    public class SearcherInstance
    {
        [JsonInclude]
        private double score = 0;
        [JsonInclude]
        private double lastUpdated = 0;
        [JsonInclude]
        private int estimatedEngagement = 0;
        [JsonInclude]
        private string description = "";
        [JsonInclude]
        private string language = "";
        [JsonInclude]
        private string id = "";
        [JsonInclude]
        private string title = "";
        [JsonInclude]
        private string coverUrl = "";
        [JsonInclude]
        private string website = "";
        [JsonInclude]
        private string[] topics = [];
        [JsonInclude]
        private int subscribers = 0;
        [JsonInclude]
        private string feedId = "";
        [JsonInclude]
        private double updated = 0;
        [JsonInclude]
        private double velocity = 0;
        [JsonInclude]
        private string iconUrl = "";
        [JsonInclude]
        private string visualUrl = "";
        [JsonInclude]
        private bool partial = false;
        [JsonInclude]
        private string logo = "";
        [JsonInclude]
        private string relatedLayout = "";
        [JsonInclude]
        private string relatedTarget = "";
        [JsonInclude]
        private string accentColor = "";

        /// <summary>
        /// Feed score
        /// </summary>
        public double Score =>
            score;

        /// <summary>
        /// Last updated time in Unix time (milliseconds)
        /// </summary>
        public double LastUpdated =>
            lastUpdated;

        /// <summary>
        /// Estimated feed engagement rate
        /// </summary>
        public int EstimatedEngagement =>
            estimatedEngagement;

        /// <summary>
        /// Description of the feed
        /// </summary>
        public string Description =>
            description;

        /// <summary>
        /// Language of the feed in two-letter language code, such as en, de, nl, ...
        /// </summary>
        public string Language =>
            language;

        /// <summary>
        /// Feed ID, including a link to the feed itself
        /// </summary>
        public string Id =>
            id;

        /// <summary>
        /// Feed title
        /// </summary>
        public string Title =>
            title;

        /// <summary>
        /// Cover URL
        /// </summary>
        public string CoverUrl =>
            coverUrl;

        /// <summary>
        /// Website of a feed (main home page)
        /// </summary>
        public string Website =>
            website;

        /// <summary>
        /// Feed topics and categories
        /// </summary>
        public string[] Topics =>
            topics;

        /// <summary>
        /// Number of Feedly subscribers
        /// </summary>
        public int Subscribers =>
            subscribers;

        /// <summary>
        /// Feed ID, including a link to the feed itself
        /// </summary>
        public string FeedId =>
            feedId;

        /// <summary>
        /// Updated time in Unix time (milliseconds)
        /// </summary>
        public double Updated =>
            updated;

        /// <summary>
        /// How frequent does this feed get updated?
        /// </summary>
        public double Velocity =>
            velocity;

        /// <summary>
        /// Icon URL
        /// </summary>
        public string IconUrl =>
            iconUrl;

        /// <summary>
        /// Visual URL
        /// </summary>
        public string VisualUrl =>
            visualUrl;

        /// <summary>
        /// Whether this syndication is partial or not
        /// </summary>
        public bool Partial =>
            partial;

        /// <summary>
        /// Logo URL
        /// </summary>
        public string Logo =>
            logo;

        /// <summary>
        /// Related layout
        /// </summary>
        public string RelatedLayout =>
            relatedLayout;

        /// <summary>
        /// Related target
        /// </summary>
        public string RelatedTarget =>
            relatedTarget;

        /// <summary>
        /// Accent color in hexadecimals
        /// </summary>
        public string AccentColor =>
            accentColor;

        [JsonConstructor]
        private SearcherInstance()
        { }
    }
}
