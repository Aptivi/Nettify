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

using Nettify.Rss.Searcher;
using System;

namespace Nettify.Demo.Fixtures.Cases
{
    internal class RssFeedSearcher : IFixture
    {
        public string FixtureID => "RssFeedSearcher";
        public void RunFixture()
		{
			// Prompt for search term
			Console.Write("Enter search term: ");
			string address = Console.ReadLine() ?? "";

			// Populate the feed info
			var feeds = SearcherTools.GetRssFeeds(address);
			foreach (var feed in feeds)
			{
				Console.WriteLine("  Title: {0}", feed.Title);
				Console.WriteLine("  Description: {0}", feed.Description);
				Console.WriteLine("  URL: {0}\n", feed.Id);
			}
		}
    }
}
