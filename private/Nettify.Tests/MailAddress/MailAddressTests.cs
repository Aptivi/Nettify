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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nettify.MailAddress;
using Shouldly;

namespace Nettify.Tests.MailAddress
{
    [TestClass]
    public sealed class MailAddressTests
    {
        [TestMethod]
        public void TestListMailAddresses()
        {
            var ispHosts = IspTools.KnownIspHosts;
            ispHosts.Length.ShouldBeGreaterThan(0);
        }

        [TestMethod]
        public void TestGoogleMailCheck()
        {
            bool known = IspTools.IsIspKnown("gmail.com");
            known.ShouldBe(true);
        }

        [TestMethod]
        public void TestGoogleMailGetConfig()
        {
            var config = IspTools.GetIspConfigFromHost("gmail.com");
            config.EmailProvider.ShouldNotBeNull();
            config.EmailProvider.DisplayName.ShouldBe("Google Mail");
            config.EmailProvider.DominatingDomain.ShouldBe("googlemail.com");
        }
    }
}
