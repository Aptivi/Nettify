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
    /// The log-in server enablement instructions
    /// </summary>
    [XmlRoot(ElementName = "enable")]
    [DebuggerDisplay("{Instruction}: {VisitUrl}")]
    public class Enable
    {
        /// <summary>
        /// The instruction
        /// </summary>
        [XmlElement(ElementName = "instruction")]
        public string Instruction { get; set; } = "";

        /// <summary>
        /// The URL to visit to enable login
        /// </summary>
        [XmlAttribute(AttributeName = "visiturl")]
        public string VisitUrl { get; set; } = "";
    }

}
