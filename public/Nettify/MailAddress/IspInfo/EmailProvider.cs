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
    /// The E-mail provider (ISP) information
    /// </summary>
    [XmlRoot(ElementName = "emailProvider")]
    [DebuggerDisplay("{DisplayName}: {Domain.Length} domains with {DominatingDomain} as dominating domain")]
    public class EmailProvider
    {
        /// <summary>
        /// The list of domains
        /// </summary>
        [XmlElement(ElementName = "domain")]
        public string[]? Domain { get; set; }

        /// <summary>
        /// The full name for the ISP mail server
        /// </summary>
        [XmlElement(ElementName = "displayName")]
        public string DisplayName { get; set; } = "";

        /// <summary>
        /// The short name for the ISP mail server
        /// </summary>
        [XmlElement(ElementName = "displayShortName")]
        public string DisplayShortName { get; set; } = "";

        /// <summary>
        /// List of incoming servers
        /// </summary>
        [XmlElement(ElementName = "incomingServer")]
        public IncomingServer[]? IncomingServer { get; set; }

        /// <summary>
        /// Outgoing server
        /// </summary>
        [XmlElement(ElementName = "outgoingServer")]
        public OutgoingServer? OutgoingServer { get; set; }

        /// <summary>
        /// Documentation information
        /// </summary>
        [XmlElement(ElementName = "documentation")]
        public Documentation[]? Documentation { get; set; }

        /// <summary>
        /// The dominating domain
        /// </summary>
        [XmlAttribute(AttributeName = "id")]
        public string DominatingDomain { get; set; } = "";
    }

}
