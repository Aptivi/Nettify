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

namespace Nettify.Rss.Instance
{
    /// <summary>
    /// Enumeration for RSS feed type
    /// </summary>
    public enum RSSFeedType
    {
        /// <summary>
        /// The RSS format is RSS 2.0
        /// </summary>
        RSS2,
        /// <summary>
        /// The RSS format is RSS 1.0
        /// </summary>
        RSS1,
        /// <summary>
        /// The RSS format is Atom
        /// </summary>
        Atom,
        /// <summary>
        /// Try to infer RSS type
        /// </summary>
        Infer = 1024
    }
}
