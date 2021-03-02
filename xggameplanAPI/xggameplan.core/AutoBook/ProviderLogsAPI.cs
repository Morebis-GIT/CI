using System.Collections.Generic;
using System.IO;
using System.Net;
using xggameplan.common.Services;
using xggameplan.AutoBooks.Abstractions;

namespace xggameplan.AutoBooks
{
    /// <summary>
    /// Environments API for managing environment instances in AWS using the Provisioning API, uses REST
    /// </summary>
    public class ProviderLogsAPI : InterfaceToREST, IProviderLogsAPI
    {
        private string _baseUrl;
        private string _accessToken;
        private const string authType = "bearer";

        public ProviderLogsAPI(string baseUrl, string accessToken)
        {
            _baseUrl = baseUrl;
            _accessToken = accessToken;
        }

        /// <summary>
        /// Returns List of Provider Logs for a specified day from the Autobooks Provider API
        /// </summary>
        /// <param name="logDate"></param>
        /// <returns></returns>
        public List<ProviderLog> GetLogs(string logDate)
        {
            string url = $"{_baseUrl}/logs/{logDate}";
            var webRequest = CreateHttpWebRequest(url, "GET", GetAuthHeaders(authType, _accessToken), string.Empty, null);
            var webResponse = (HttpWebResponse)webRequest.GetResponse();
            ThrowExceptionIfNotSuccess(webResponse, new List<HttpStatusCode>() { HttpStatusCode.OK });
            var providerLogs = new List<ProviderLog>();
            using (var reader = new StreamReader(webResponse.GetResponseStream()))
            {
                string data = reader.ReadToEnd().ToLower();
                providerLogs = DeserializeContentBody<List<ProviderLog>>(data);
            }
            return providerLogs;
        }

    }
}
