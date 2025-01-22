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

using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Nettify.Helpers
{
    internal class HttpGetRedirectingHandler : DelegatingHandler
    {
        private static readonly HttpStatusCode[] redirectCodes =
        [
            HttpStatusCode.Moved,
            HttpStatusCode.MovedPermanently,
            HttpStatusCode.Found,
            HttpStatusCode.Redirect,
            HttpStatusCode.RedirectKeepVerb,
            HttpStatusCode.RedirectMethod,
            HttpStatusCode.SeeOther,
            HttpStatusCode.TemporaryRedirect,
        ];

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // Get a response.
            var response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);

            // Check to see if we have a redirect
            if (request.Method == HttpMethod.Get && redirectCodes.Contains(response.StatusCode))
            {
                // We are redirecting. Make a request on the new location
                var location = response.Headers.Location;
                HttpRequestMessage newRequest = new(HttpMethod.Get, location);
                return await SendAsync(newRequest, cancellationToken);
            }

            // There is no redirect.
            return response;
        }

        internal HttpGetRedirectingHandler() :
            this(new HttpClientHandler())
        { }

        internal HttpGetRedirectingHandler(HttpMessageHandler handler)
        {
            InnerHandler = handler;
        }
    }
}
