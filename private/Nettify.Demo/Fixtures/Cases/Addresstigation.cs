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

using System;
using Nettify.MailAddress;
using Textify.Versioning;

namespace Nettify.Demo.Fixtures.Cases
{
    internal class Addresstigation : IFixture
    {
        public string FixtureID => "Addresstigation";
        public void RunFixture()
        {
            // Prompt for an e-mail address
            Console.Write("Enter an e-mail address: ");
            string address = Console.ReadLine() ?? "";

            // Query it
            var ispInstance = IspTools.GetIspConfig(address);
            var ispMail = ispInstance.EmailProvider ??
                throw new Exception("Can't get mail");
            Console.WriteLine($"ISP Name: {ispMail.DisplayName} [{ispMail.DisplayShortName}]");
            Console.WriteLine($"Main domain: {ispMail.DominatingDomain}");
            foreach (string domain in ispMail.Domain ?? [])
                Console.WriteLine($"  Domain: {domain}");
            foreach (var server in ispMail.IncomingServer ?? [])
            {
                Console.WriteLine($"  Incoming server hostname: {server.Hostname}:{server.Port}");
                Console.WriteLine($"  Socket type: {server.SocketType}");
                Console.WriteLine($"  Server type: {server.Type}");
                Console.WriteLine($"  Username: {server.Username}");
                Console.WriteLine($"  Leave messages on server? {(server.Pop3 is not null ? server.Pop3.LeaveMessagesOnServer : "False")}");
                Console.WriteLine($"  Auth methods: {string.Join(", ", server.Authentication ?? [])}");
            }
            if (ispMail.OutgoingServer is not null)
            {
                Console.WriteLine($"Outgoing server hostname: {ispMail.OutgoingServer.Hostname}:{ispMail.OutgoingServer.Port}");
                Console.WriteLine($"Socket type: {ispMail.OutgoingServer.SocketType}");
                Console.WriteLine($"Server type: {ispMail.OutgoingServer.Type}");
                Console.WriteLine($"Username: {ispMail.OutgoingServer.Username}");
                Console.WriteLine($"Auth methods: {string.Join(", ", ispMail.OutgoingServer.AuthenticationMethods ?? [])}");
            }
        }
    }
}
