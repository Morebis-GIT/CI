using System.Collections.Generic;
using System.IO;
using System.Net;
using xggameplan.AutoBooks.Abstractions;
using xggameplan.common.Services;

namespace xggameplan.AutoBooks.AWS
{
    /// <summary>
    /// Environments API for managing environment instances in AWS using the Provisioning API, uses REST
    /// </summary>
    public class AWSEnvironmentAPI : InterfaceToREST, IEnvironmentAPI<AWSPAEnvironment>
    {
        private string _baseUrl;
        private string _accessToken;
        private const string authType = "bearer";

        public AWSEnvironmentAPI(string baseUrl, string accessToken)
        {
            _baseUrl = baseUrl;
            _accessToken = accessToken;
        }

        /// <summary>
        /// Gets details of AutoBook Environment
        /// </summary>
        public AWSPAEnvironment Get()
        {
            string url = $"{_baseUrl}/environment";
            var webRequest = CreateHttpWebRequest(url, "GET", GetAuthHeaders(authType, _accessToken), string.Empty, null);
            var webResponse = (HttpWebResponse)webRequest.GetResponse();
            ThrowExceptionIfNotSuccess(webResponse, new List<HttpStatusCode>() { HttpStatusCode.OK });

            AWSPAEnvironment environment = null;
            using (var reader = new StreamReader(webResponse.GetResponseStream()))
            {
                string data = reader.ReadToEnd().ToLower();
                environment = DeserializeContentBody<AWSPAEnvironment>(data);
            }
            return environment;
        }

        /// <summary>
        /// Creates AutoBook Environment
        /// </summary>
        public void Create()
        {
            string url = $"{_baseUrl}/environment/provision";
            var webRequest = CreateHttpWebRequest(url, "POST", GetAuthHeaders(authType, _accessToken), ContentTypeJSON, null);
            var webResponse = (HttpWebResponse)webRequest.GetResponse();
            ThrowExceptionIfNotSuccess(webResponse, new List<HttpStatusCode>() { HttpStatusCode.OK });
        }

        /// <summary>
        /// Deletes AutoBook Environment
        /// </summary>
        public void Delete()
        {
            string url = $"{_baseUrl}/environment/provision";
            var webRequest = CreateHttpWebRequest(url, "DELETE", GetAuthHeaders(authType, _accessToken), string.Empty, null);
            var webResponse = (HttpWebResponse)webRequest.GetResponse();
            ThrowExceptionIfNotSuccess(webResponse, new List<HttpStatusCode>() { HttpStatusCode.OK });
        }
    }
}
