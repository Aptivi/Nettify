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
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Xml;

namespace Nettify.Rss.Instance
{
    /// <summary>
    /// RSS article instance
    /// </summary>
    [DebuggerDisplay("{ArticleTitle}: {ArticleLink}")]
    public class RSSArticle
    {
        private readonly string articleTitle;
        private readonly string articleLink;
        private readonly string articleDescription;
        private readonly ReadOnlyDictionary<string, XmlNode> articleVariables;

        /// <summary>
        /// RSS Article Title
        /// </summary>
        public string ArticleTitle =>
            articleTitle;

        /// <summary>
        /// RSS Article Link
        /// </summary>
        public string ArticleLink =>
            articleLink;

        /// <summary>
        /// RSS Article Descirption
        /// </summary>
        public string ArticleDescription =>
            articleDescription;

        /// <summary>
        /// RSS Article Parameters
        /// </summary>
        public ReadOnlyDictionary<string, XmlNode> ArticleVariables =>
            articleVariables;

        /// <summary>
        /// Makes a new instance of RSS article
        /// </summary>
        /// <param name="ArticleTitle">Article title</param>
        /// <param name="ArticleLink">Link to article</param>
        /// <param name="ArticleDescription">Article description</param>
        /// <param name="ArticleVariables">Article variables as <see cref="XmlNode"/>s</param>
        internal RSSArticle(string ArticleTitle, string ArticleLink, string ArticleDescription, Dictionary<string, XmlNode> ArticleVariables)
        {
            articleTitle = !string.IsNullOrWhiteSpace(ArticleTitle) ? ArticleTitle.Trim() : "";
            articleLink = !string.IsNullOrWhiteSpace(ArticleLink) ? ArticleLink.Trim() : "";
            articleDescription = !string.IsNullOrWhiteSpace(ArticleDescription) ? ArticleDescription.Trim() : "";
            articleVariables = new(ArticleVariables);
        }
    }
}
