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

using System.Diagnostics;
using System.Text.Json.Serialization;

namespace Nettify.EnglishDictionary
{
    /// <summary>
    /// A dictionary word
    /// </summary>
    [DebuggerDisplay("{Word} {PhoneticWord}: {Meanings.Length} meanings")]
    public partial class DictionaryWord
    {
        /// <summary>
        /// The definition class
        /// </summary>
        [DebuggerDisplay("{Definition}: {Synonyms.Length} synonyms, {Antonyms.Length} antonyms")]
        public partial class DefinitionType
        {
            /// <summary>
            /// Word definition
            /// </summary>
            [JsonPropertyName("definition")]
            public string Definition { get; set; } = "";

            /// <summary>
            /// List of synonyms based on the definition
            /// </summary>
            [JsonPropertyName("synonyms")]
            public string[]? Synonyms { get; set; }

            /// <summary>
            /// List of antonyms based on the definition
            /// </summary>
            [JsonPropertyName("antonyms")]
            public string[]? Antonyms { get; set; }

            /// <summary>
            /// Example in sentence
            /// </summary>
            [JsonPropertyName("example")]
            public string Example { get; set; } = "";
        }

        /// <summary>
        /// The license class
        /// </summary>
        [DebuggerDisplay("{Name}: {Url}")]
        public partial class License
        {
            /// <summary>
            /// License name
            /// </summary>
            [JsonPropertyName("name")]
            public string Name { get; set; } = "";

            /// <summary>
            /// License URL
            /// </summary>
            [JsonPropertyName("url")]
            public string Url { get; set; } = "";
        }

        /// <summary>
        /// Word meaning class
        /// </summary>
        [DebuggerDisplay("{PartOfSpeech}: {Definitions.Count} definitions, {Synonyms.Length} synonyms, {Antonyms.Length} antonyms")]
        public partial class Meaning
        {
            /// <summary>
            /// Part of speech, usually noun, verb, adjective, adverb, interjection, etc.
            /// </summary>
            [JsonPropertyName("partOfSpeech")]
            public string PartOfSpeech { get; set; } = "";

            /// <summary>
            /// List of word definitions. Words usually come with one or more definitions.
            /// </summary>
            [JsonPropertyName("definitions")]
            public DefinitionType[]? Definitions { get; set; }

            /// <summary>
            /// List of synonyms based on the word meaning
            /// </summary>
            [JsonPropertyName("synonyms")]
            public string[]? Synonyms { get; set; }

            /// <summary>
            /// List of antonyms based on the word meaning
            /// </summary>
            [JsonPropertyName("antonyms")]
            public string[]? Antonyms { get; set; }
        }

        /// <summary>
        /// Phonetic class
        /// </summary>
        [DebuggerDisplay("{Text}: {Audio}")]
        public partial class Phonetic
        {
            /// <summary>
            /// Phonetic representation of the word
            /// </summary>
            [JsonPropertyName("text")]
            public string Text { get; set; } = "";

            /// <summary>
            /// Link to the pronounciation, usually in MP3 format. Use BassBoom to play it after downloading it.
            /// </summary>
            [JsonPropertyName("audio")]
            public string Audio { get; set; } = "";

            /// <summary>
            /// From where did we get the audio from?
            /// </summary>
            [JsonPropertyName("sourceUrl")]
            public string SourceUrl { get; set; } = "";

            /// <summary>
            /// License information for the source
            /// </summary>
            [JsonPropertyName("license")]
            public License? License { get; set; }
        }

        /// <summary>
        /// The actual word
        /// </summary>
        [JsonPropertyName("word")]
        public string Word { get; set; } = "";

        /// <summary>
        /// The base phonetic representation of the word
        /// </summary>
        [JsonPropertyName("phonetic")]
        public string PhoneticWord { get; set; } = "";

        /// <summary>
        /// The alternative phonetic representations
        /// </summary>
        [JsonPropertyName("phonetics")]
        public Phonetic[]? Phonetics { get; set; }

        /// <summary>
        /// Word meanings
        /// </summary>
        [JsonPropertyName("meanings")]
        public Meaning[]? Meanings { get; set; }

        /// <summary>
        /// License information
        /// </summary>
        [JsonPropertyName("license")]
        public License? LicenseInfo { get; set; }

        /// <summary>
        /// List of where we got the word information from
        /// </summary>
        [JsonPropertyName("sourceUrls")]
        public string[]? SourceUrls { get; set; }
    }
}
