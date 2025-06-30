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

using Nettify.MailAddress.IspInfo;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;

namespace Nettify.MailAddress
{
    /// <summary>
    /// Internet Service Provider server information
    /// </summary>
    public static class IspTools
    {
        private readonly static Assembly thisAssembly = Assembly.GetAssembly(typeof(IspTools));

        /// <summary>
        /// A list of known ISP hosts
        /// </summary>
        public static string[] KnownIspHosts
        {
            get
            {
                // Get the resource names
                string[] resourceNames = thisAssembly.GetManifestResourceNames();
                int prefixLength = "Nettify.".Length;
                List<string> hosts = [];

                // Process and return them
                foreach (string resourceName in resourceNames)
                {
                    // Check to see if this is a resource that contains ISP info
                    if (!resourceName.EndsWith(".xml"))
                        continue;

                    // Now, get the final host and add it
                    string finalHost = resourceName.Substring(prefixLength);
                    finalHost = finalHost.Substring(0, finalHost.LastIndexOf(".xml"));
                    hosts.Add(finalHost);
                }
                return [.. hosts];
            }
        }
        
        /// <summary>
        /// Checks to see if your mail ISP is known
        /// </summary>
        /// <param name="address">A valid and full e-mail address containing your mail provider</param>
        /// <returns>True if your ISP is known; false otherwise</returns>
        public static bool IsIspKnownByMail(string address)
        {
            string hostName = new Uri($"mailto:{address}").Host;
            return IsIspKnown(hostName);
        }

        /// <summary>
        /// Checks to see if your mail ISP is known
        /// </summary>
        /// <param name="host">A valid host name that pertains to your mail provider</param>
        /// <returns>True if your ISP is known; false otherwise</returns>
        public static bool IsIspKnown(string host) =>
            KnownIspHosts.Contains(host);

        /// <summary>
        /// Gets the ISP configuration for the specified mail address
        /// </summary>
        /// <param name="address">The mail address to parse. Must include the ISP hostname.</param>
        /// <returns>The ISP client config for specified mail address</returns>
        public static ClientConfig GetIspConfig(string address)
        {
            string hostName = new Uri($"mailto:{address}").Host;
            return GetIspConfigFromHost(hostName);
        }

        /// <summary>
        /// Gets the ISP configuration for the specified host
        /// </summary>
        /// <param name="host">The ISP hostname.</param>
        /// <returns>The ISP client config for specified host</returns>
        public static ClientConfig GetIspConfigFromHost(string host)
        {
            // Check to see if the ISP is known
            if (!IsIspKnown(host))
                throw new ArgumentException(string.Format("ISP {0} not known.", host));

            // Get the final database address
            var xmlStream = thisAssembly.GetManifestResourceStream($"Nettify.{host}.xml");
            string xmlContent = new StreamReader(xmlStream).ReadToEnd();

            // Get the client config
            ClientConfig clientConfig;
            XmlSerializer xmlSerializer = new(typeof(ClientConfig),
                new XmlRootAttribute("clientConfig")
                {
                    IsNullable = false
                });
            StringReader sr = new(xmlContent);
            clientConfig = (ClientConfig)xmlSerializer.Deserialize(sr);
            return clientConfig;
        }
    }
}
