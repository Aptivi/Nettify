﻿//
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

namespace Nettify.Radio
{
    /// <summary>
    /// Happens when the error in the server has occurred
    /// </summary>
    public class ShoutcastServerException : Exception
    {
        /// <summary>
        /// Throws the server exception
        /// </summary>
        public ShoutcastServerException() { }
        /// <summary>
        /// Throws the server exception
        /// </summary>
        public ShoutcastServerException(string message) : base(message) { }
        /// <summary>
        /// Throws the server exception
        /// </summary>
        public ShoutcastServerException(string message, Exception inner) : base(message, inner) { }
        /// <summary>
        /// Throws the server exception
        /// </summary>
        protected ShoutcastServerException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}