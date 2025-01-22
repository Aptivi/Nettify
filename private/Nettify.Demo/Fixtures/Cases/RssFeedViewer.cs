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

using Nettify.Rss.Instance;
using System;

namespace Nettify.Demo.Fixtures.Cases
{
    internal class RssFeedViewer : IFixture
    {
        public string FixtureID => "RssFeedViewer";
        public void RunFixture()
		{
			// Prompt for RSS URL
			Console.Write("Enter RSS URL: ");
			string address = Console.ReadLine() ?? "";

			// Populate the RSS info
			var feed = new RSSFeed(address, RSSFeedType.Infer);
			feed.Refresh();
			Console.WriteLine("Title: {0}", feed.FeedTitle);
			Console.WriteLine("Description: {0}", feed.FeedDescription);
			Console.WriteLine("URL: {0}\n", feed.FeedUrl);

			// Populate the article info
			foreach (RSSArticle article in feed.FeedArticles)
			{
				Console.WriteLine("  Title: {0}", article.ArticleTitle);
				Console.WriteLine("  Description: {0}", article.ArticleDescription);
				Console.WriteLine("  URL: {0}\n", article.ArticleLink);
			}
		}
    }
}
