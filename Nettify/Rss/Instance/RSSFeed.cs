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
using System.Diagnostics;
using System.Threading.Tasks;
using System.Xml;

namespace Nettify.Rss.Instance
{
    /// <summary>
    /// RSS feed class instance
    /// </summary>
    [DebuggerDisplay("{FeedTitle}: {FeedUrl}")]
    public class RSSFeed
    {
        private string _FeedUrl = "";
        private RSSFeedType _FeedType;
        private string _FeedTitle = "";
        private string _FeedDescription = "";
        private RSSArticle[] _FeedArticles = [];

        /// <summary>
        /// A URL to RSS feed
        /// </summary>
        public string FeedUrl =>
            _FeedUrl;

        /// <summary>
        /// RSS feed type
        /// </summary>
        public RSSFeedType FeedType =>
            _FeedType;

        /// <summary>
        /// RSS feed type
        /// </summary>
        public string FeedTitle =>
            _FeedTitle;

        /// <summary>
        /// RSS feed description (Atom feeds not supported and always return an empty string)
        /// </summary>
        public string FeedDescription =>
            _FeedDescription;

        /// <summary>
        /// Feed articles
        /// </summary>
        public RSSArticle[] FeedArticles =>
            _FeedArticles;

        /// <summary>
        /// Makes a new instance of an RSS feed class
        /// </summary>
        /// <param name="FeedUrl">A URL to RSS feed</param>
        /// <param name="FeedType">A feed type to parse. If set to Infer, it will automatically detect the type based on contents.</param>
        public RSSFeed(string FeedUrl, RSSFeedType FeedType)
        {
            _FeedUrl = FeedUrl;
            _FeedType = FeedType;
        }

        /// <summary>
        /// Refreshes the RSS class instance
        /// </summary>
        public void Refresh()
            => Refresh(_FeedUrl, _FeedType);

        /// <summary>
        /// Refreshes the RSS class instance
        /// </summary>
        /// <param name="FeedUrl">A URL to RSS feed</param>
        /// <param name="FeedType">A feed type to parse. If set to Infer, it will automatically detect the type based on contents.</param>
        public void Refresh(string FeedUrl, RSSFeedType FeedType)
        {
            // Make a web request indicator
            var FeedWebRequest = RSSTools.Client.GetAsync(FeedUrl).Result;

            // Load the RSS feed and get the feed XML document
            var FeedStream = FeedWebRequest.Content.ReadAsStreamAsync().Result;
            var FeedDocument = new XmlDocument();
            FeedDocument.Load(FeedStream);
            Finalize(FeedDocument, FeedType);
        }

        /// <summary>
        /// Refreshes the RSS class instance
        /// </summary>
        public async Task RefreshAsync()
            => await RefreshAsync(_FeedUrl, _FeedType);

        /// <summary>
        /// Refreshes the RSS class instance
        /// </summary>
        /// <param name="FeedUrl">A URL to RSS feed</param>
        /// <param name="FeedType">A feed type to parse. If set to Infer, it will automatically detect the type based on contents.</param>
        public async Task RefreshAsync(string FeedUrl, RSSFeedType FeedType)
        {
            // Make a web request indicator
            var FeedWebRequest = await RSSTools.Client.GetAsync(FeedUrl);

            // Load the RSS feed and get the feed XML document
            var FeedStream = await FeedWebRequest.Content.ReadAsStreamAsync();
            var FeedDocument = new XmlDocument();
            FeedDocument.Load(FeedStream);
            Finalize(FeedDocument, FeedType);
        }

        internal void Finalize(XmlDocument feedDocument, RSSFeedType feedType)
        {
            // Infer feed type
            var FeedNodeList = default(XmlNodeList);
            if (feedType == RSSFeedType.Infer)
            {
                if (feedDocument.GetElementsByTagName("rss").Count != 0)
                {
                    FeedNodeList = feedDocument.GetElementsByTagName("rss");
                    _FeedType = RSSFeedType.RSS2;
                }
                else if (feedDocument.GetElementsByTagName("rdf:RDF").Count != 0)
                {
                    FeedNodeList = feedDocument.GetElementsByTagName("rdf:RDF");
                    _FeedType = RSSFeedType.RSS1;
                }
                else if (feedDocument.GetElementsByTagName("feed").Count != 0)
                {
                    FeedNodeList = feedDocument.GetElementsByTagName("feed");
                    _FeedType = RSSFeedType.Atom;
                }
            }
            else if (feedType == RSSFeedType.RSS2)
            {
                FeedNodeList = feedDocument.GetElementsByTagName("rss");
                if (FeedNodeList.Count == 0)
                    throw new RSSException("Invalid RSS2 feed.");
            }
            else if (feedType == RSSFeedType.RSS1)
            {
                FeedNodeList = feedDocument.GetElementsByTagName("rdf:RDF");
                if (FeedNodeList.Count == 0)
                    throw new RSSException("Invalid RSS1 feed.");
            }
            else if (feedType == RSSFeedType.Atom)
            {
                FeedNodeList = feedDocument.GetElementsByTagName("feed");
                if (FeedNodeList.Count == 0)
                    throw new RSSException("Invalid Atom feed.");
            }

            // Populate basic feed properties
            if (FeedNodeList is null)
                throw new RSSException("Can't get node list for this feed.");
            string FeedTitle = Convert.ToString(RSSTools.GetFeedProperty("title", FeedNodeList, _FeedType));
            string FeedDescription = Convert.ToString(RSSTools.GetFeedProperty("description", FeedNodeList, _FeedType));

            // Populate articles
            var Articles = RSSTools.MakeRssArticlesFromFeed(FeedNodeList, _FeedType);

            // Install the variables to a new instance
            _FeedUrl = FeedUrl;
            _FeedTitle = FeedTitle.Trim();
            _FeedDescription = FeedDescription.Trim();
            if (_FeedArticles.Length != 0 & Articles.Length != 0)
            {
                if (!_FeedArticles[0].Equals(Articles[0]))
                    _FeedArticles = Articles;
            }
            else
                _FeedArticles = Articles;
        }
    }
}
