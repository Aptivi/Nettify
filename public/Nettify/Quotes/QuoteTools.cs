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
using System.Collections.Generic;
using System.IO;
using Nettify.Language;
using Newtonsoft.Json.Linq;

namespace Nettify.Quotes
{
    /// <summary>
    /// Tools for quotes
    /// </summary>
    public static class QuoteTools
    {
        private static readonly Random random = new();
        private static JArray? quotesArray;

        /// <summary>
        /// Gets all quotes
        /// </summary>
        /// <returns>Array of quotes</returns>
        public static Quote[] GetAllQuotes()
        {
            var quotesArray = GetQuotesArray();
            if (quotesArray is null)
                return [];

            // Get all quotes!
            var quotes = new List<Quote>();
            for (int i = 0; i < quotesArray.Count; i++)
            {
                var quote = GetQuote(i);
                if (quote is not null)
                    quotes.Add(quote);
            }
            return [.. quotes];
        }

        /// <summary>
        /// Gets a random quote
        /// </summary>
        /// <returns>Quote instance containing content and author</returns>
        public static Quote? GetRandomQuote()
        {
            var quotesArray = GetQuotesArray();
            if (quotesArray is null)
                return null;

            // Now, get the content and the author
            return GetQuote(random.Next(quotesArray.Count));
        }

        /// <summary>
        /// Gets a quote
        /// </summary>
        /// <returns>Quote instance containing content and author</returns>
        public static Quote? GetQuote(int quoteNumber)
        {
            var quotesArray = GetQuotesArray();
            if (quotesArray is null)
                return null;

            // Now, get the content and the author
            var quoteToken = quotesArray[quoteNumber];
            string? content = (string?)quoteToken["content"] ?? LanguageTools.GetLocalized("NKS_AMUSEMENTS_QUOTE_QUOTEUNKNOWN");
            string? author = (string?)quoteToken["author"] ?? LanguageTools.GetLocalized("NKS_AMUSEMENTS_QUOTE_AUTHORUNKNOWN");
            return new(content, author);
        }

        internal static JArray? GetQuotesArray()
        {
            if (quotesArray is not null)
                return quotesArray;

            // Get a quote string from the resources
            var quotesResource = typeof(QuoteTools).Assembly.GetManifestResourceStream("Nettify.quotes.json");
            if (quotesResource is null)
                return null;
            string quotesString = new StreamReader(quotesResource).ReadToEnd();
            quotesArray = JArray.Parse(quotesString);
            return quotesArray;
        }
    }
}
