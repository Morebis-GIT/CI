using System;
using System.Net.Http;

namespace xggameplan.Services.AccessTokenExtractor
{
    /// <summary>
    /// Access token extractor which tries to find a token in the requests "Authorization" header with the "Bearer" prefix.
    /// </summary>
    public class AuthorizationHeaderAccessTokenExtractor : IAccessTokenExtractor
    {
        /// <summary>
        /// Extracts token from the request's Authorization header. If the header is not present or has to incorrect type, null is returned.
        /// Empty token in the header is also not accepted and null is returned.
        /// </summary>
        /// <param name="request">Http request</param>
        /// <returns>Token or null</returns>
        public string ExtractToken(HttpRequestMessage request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var authorizationHeader = request.Headers.Authorization;

            if (authorizationHeader == null)
            {
                return null;
            }

            if (authorizationHeader.Scheme != "Bearer")
            {
                return null;
            }

            if (string.IsNullOrWhiteSpace(authorizationHeader.Parameter))
            {
                return null;
            }

            return authorizationHeader.Parameter;
        }
    }
}
