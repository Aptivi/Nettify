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

using HtmlAgilityPack;
using Nettify.Rss.Instance;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Xml;

namespace Nettify.Rss
{
    /// <summary>
    /// The RSS tools
    /// </summary>
    public static class RSSTools
    {
        internal static HttpClient Client = new();

        /// <summary>
        /// Make instances of RSS Article from feed node and type
        /// </summary>
        /// <param name="FeedNode">Feed XML node</param>
        /// <param name="FeedType">Feed type</param>
        public static List<RSSArticle> MakeRssArticlesFromFeed(XmlNodeList FeedNode, RSSFeedType FeedType)
        {
            var Articles = new List<RSSArticle>();
            switch (FeedType)
            {
                case RSSFeedType.RSS2:
                    foreach (XmlNode Node in FeedNode[0]) // <channel>
                    {
                        foreach (XmlNode Child in Node.ChildNodes) // <item>
                        {
                            if (Child.Name == "item")
                            {
                                var Article = MakeArticleFromFeed(Child);
                                Articles.Add(Article);
                            }
                        }
                    }

                    break;
                case RSSFeedType.RSS1:
                    foreach (XmlNode Node in FeedNode[0]) // <channel> or <item>
                    {
                        if (Node.Name == "item")
                        {
                            var Article = MakeArticleFromFeed(Node);
                            Articles.Add(Article);
                        }
                    }

                    break;
                case RSSFeedType.Atom:
                    foreach (XmlNode Node in FeedNode[0]) // <feed>
                    {
                        if (Node.Name == "entry")
                        {
                            var Article = MakeArticleFromFeed(Node);
                            Articles.Add(Article);
                        }
                    }

                    break;
                default:
                    throw new RSSException("Invalid RSS feed type.");
            }
            return Articles;
        }

        /// <summary>
        /// Generates an instance of article from feed
        /// </summary>
        /// <param name="Article">The child node which holds the entire article</param>
        /// <returns>An article</returns>
        public static RSSArticle MakeArticleFromFeed(XmlNode Article)
        {
            // Variables
            var Parameters = new Dictionary<string, XmlNode>();
            string Title = default, Link = default, Description = default;

            // Parse article
            foreach (XmlNode ArticleNode in Article.ChildNodes)
            {
                // Check the title
                if (ArticleNode.Name == "title")
                    // Trimming newlines and spaces is necessary, since some RSS feeds (GitHub commits) might return string with trailing and leading spaces and newlines.
                    Title = ArticleNode.InnerText.Trim(Convert.ToChar(Convert.ToChar(13)), Convert.ToChar(Convert.ToChar(10)), ' ');

                // Check the link
                if (ArticleNode.Name == "link")
                    // Links can be in href attribute, so check that.
                    if (ArticleNode.Attributes.Count != 0 & ArticleNode.Attributes.GetNamedItem("href") is not null)
                        Link = ArticleNode.Attributes.GetNamedItem("href").InnerText;
                    else
                        Link = ArticleNode.InnerText;

                // Check the summary
                if (ArticleNode.Name == "summary" | ArticleNode.Name == "content" | ArticleNode.Name == "description")
                {
                    // It can be of HTML type, or plain text type.
                    if (ArticleNode.Attributes.Count != 0 & ArticleNode.Attributes.GetNamedItem("type") is not null)
                    {
                        if (ArticleNode.Attributes.GetNamedItem("type").Value == "html")
                        {
                            // Extract plain text from HTML
                            var HtmlContent = new HtmlDocument();
                            HtmlContent.LoadHtml(ArticleNode.InnerText.Trim(Convert.ToChar(Convert.ToChar(13)), Convert.ToChar(Convert.ToChar(10)), ' '));

                            // Some feeds have no node called "pre," so work around this...
                            var PreNode = HtmlContent.DocumentNode.SelectSingleNode("pre");
                            if (PreNode is null)
                                Description = HtmlContent.DocumentNode.InnerText;
                            else
                                Description = PreNode.InnerText;
                        }
                        else
                            Description = ArticleNode.InnerText.Trim(Convert.ToChar(Convert.ToChar(13)), Convert.ToChar(Convert.ToChar(10)), ' ');
                    }
                    else
                        Description = ArticleNode.InnerText.Trim(Convert.ToChar(Convert.ToChar(13)), Convert.ToChar(Convert.ToChar(10)), ' ');
                }
                if (!Parameters.ContainsKey(ArticleNode.Name))
                    Parameters.Add(ArticleNode.Name, ArticleNode);
            }
            return new RSSArticle(Title, Link, Description, Parameters);
        }

        /// <summary>
        /// Gets a feed property
        /// </summary>
        /// <param name="FeedProperty">Feed property name</param>
        /// <param name="FeedNode">Feed XML node</param>
        /// <param name="FeedType">Feed type</param>
        public static object GetFeedProperty(string FeedProperty, XmlNodeList FeedNode, RSSFeedType FeedType)
        {
            switch (FeedType)
            {
                case RSSFeedType.RSS2:
                    foreach (XmlNode Node in FeedNode[0]) // <channel>
                        foreach (XmlNode Child in Node.ChildNodes)
                            if (Child.Name == FeedProperty)
                                return Child.InnerXml;

                    break;
                case RSSFeedType.RSS1:
                    foreach (XmlNode Node in FeedNode[0]) // <channel> or <item>
                        foreach (XmlNode Child in Node.ChildNodes)
                            if (Child.Name == FeedProperty)
                                return Child.InnerXml;

                    break;
                case RSSFeedType.Atom:
                    foreach (XmlNode Node in FeedNode[0]) // Children of <feed>
                        if (Node.Name == FeedProperty)
                            return Node.InnerXml;

                    break;
                default:
                    throw new RSSException("Invalid RSS feed type.");
            }
            return "";
        }
    }
}
