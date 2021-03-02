using System.Net.Http;

namespace xggameplan.Services.AccessTokenExtractor
{
    /// <summary>
    /// Provides methods for extracting access token from the request.
    /// </summary>
    public interface IAccessTokenExtractor
    {
        /// <summary>
        /// Extracts access token from the request. Returns null if access token is not present.
        /// </summary>
        /// <param name="request">Http request</param>
        /// <returns>Token or null</returns>
        string ExtractToken(HttpRequestMessage request);
    }
}
