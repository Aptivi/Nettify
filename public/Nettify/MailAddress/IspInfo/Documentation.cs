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
using System.Xml.Serialization;

namespace Nettify.MailAddress.IspInfo
{

    /// <summary>
    /// Documentation information
    /// </summary>
    [XmlRoot(ElementName = "documentation")]
    [DebuggerDisplay("{Description}: {Url}")]
    public class Documentation
    {
        /// <summary>
        /// Documentation description
        /// </summary>
        [XmlElement(ElementName = "descr")]
        public string Description { get; set; } = "";

        /// <summary>
        /// Documentation URL
        /// </summary>
        [XmlAttribute(AttributeName = "url")]
        public string Url { get; set; } = "";
    }

}
