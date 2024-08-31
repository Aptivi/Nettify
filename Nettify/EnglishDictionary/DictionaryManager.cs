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
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Nettify.EnglishDictionary
{
    /// <summary>
    /// Dictionary management module
    /// </summary>
    public static partial class DictionaryManager
    {
        private static readonly List<DictionaryWord> CachedWords = [];
        private static readonly HttpClient DictClient = new HttpClient();

        /// <summary>
        /// Gets the word information and puts it into an array of dictionary words
        /// </summary>
        public static DictionaryWord[] GetWordInfo(string Word)
        {
            if (CachedWords.Any((word) => word.Word == Word))
            {
                // We already have a word, so there is no reason to download it again
                return CachedWords.Where((word) => word.Word == Word).ToArray();
            }
            else
            {
                // Download the word information
                HttpResponseMessage Response = DictClient.GetAsync($"https://api.dictionaryapi.dev/api/v2/entries/en/{Word}").Result;
                Response.EnsureSuccessStatusCode();
                Stream WordInfoStream = Response.Content.ReadAsStreamAsync().Result;
                string WordInfoString = new StreamReader(WordInfoStream).ReadToEnd();

                // Serialize it to DictionaryWord to cache it so that we don't have to download it again
                var Words = JsonSerializer.Deserialize<DictionaryWord[]>(WordInfoString);
                CachedWords.AddRange(Words);

                // Return the word
                return [.. CachedWords];
            }
        }

        /// <summary>
        /// Gets the word information and puts it into an array of dictionary words
        /// </summary>
        public static async Task<DictionaryWord[]> GetWordInfoAsync(string Word)
        {
            if (CachedWords.Any((word) => word.Word == Word))
            {
                // We already have a word, so there is no reason to download it again
                return CachedWords.Where((word) => word.Word == Word).ToArray();
            }
            else
            {
                // Download the word information
                HttpResponseMessage Response = await DictClient.GetAsync($"https://api.dictionaryapi.dev/api/v2/entries/en/{Word}");
                Response.EnsureSuccessStatusCode();
                Stream WordInfoStream = await Response.Content.ReadAsStreamAsync();
                string WordInfoString = new StreamReader(WordInfoStream).ReadToEnd();

                // Serialize it to DictionaryWord to cache it so that we don't have to download it again
                var Words = JsonSerializer.Deserialize<DictionaryWord[]>(WordInfoString);
                CachedWords.AddRange(Words);

                // Return the word
                return [.. CachedWords];
            }
        }
    }
}
