using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Cache;
using System.Text;
using Newtonsoft.Json;

namespace xggameplan.common.Services
{
    /// <summary>
    /// Base class for a class that implements an interface to a REST API
    /// </summary>
    public abstract class InterfaceToREST
    {
        protected const string ContentTypeJSON = "application/json";

        /// <summary>
        /// Creates HttpWebRequest instance
        /// </summary>
        /// <param name="url"></param>
        /// <param name="method"></param>
        /// <param name="headers"></param>
        /// <param name="contentType"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        protected HttpWebRequest CreateHttpWebRequest(string url, string method, Dictionary<string, string> headers,
                                                       string contentType, byte[] content)
        {
            WebRequest.DefaultWebProxy.Credentials = CredentialCache.DefaultNetworkCredentials;
            var webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.Method = method;
            webRequest.CachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore);
            webRequest.KeepAlive = true;

            foreach (string key in headers.Keys)
            {
                webRequest.Headers.Add(key, headers[key]);
            }

            if (content != null)
            {
                webRequest.ContentType = contentType;
                webRequest.ContentLength = content.Length;
                webRequest.GetRequestStream().Write(content, 0, content.Length);
            }
            return webRequest;
        }

        /// <summary>
        /// Serializes the object to byte array for content body of request
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <returns></returns>
        protected byte[] SerializeContentBody<T>(T item)
        {
            string contentString = JsonConvert.SerializeObject(item, Formatting.Indented);
            return Encoding.UTF8.GetBytes(contentString);
        }

        /// <summary>
        /// Deserializes content body string to object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="contentString"></param>
        /// <returns></returns>
        protected T DeserializeContentBody<T>(string contentString)
        {
            T item = JsonConvert.DeserializeObject<T>(contentString);
            return item;
        }

        /// <summary>
        /// Deserializes content body bytes to object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="contentBytes"></param>
        /// <returns></returns>
        protected T DeserializeContentBody<T>(byte[] contentBytes)
        {
            string contentString = Encoding.UTF8.GetString(contentBytes);
            T item = JsonConvert.DeserializeObject<T>(contentString);
            return item;
        }

        /// <summary>
        /// Returns Default Empty HTTP header
        /// </summary>
        /// <returns></returns>
        protected Dictionary<string, string> GetEmptyHeaders()
        {
            return GetHeaders("empty", null);
        }

        /// <summary>
        /// Returns Default Empty HTTP header
        /// </summary>
        /// <param name="type"></param>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        protected Dictionary<string, string> GetAuthHeaders(string type, string accessToken)
        {
            return GetHeaders(type, accessToken);
        }

        /// <summary>
        /// Returns HTTP headers for specified profile
        /// </summary>
        /// <param name="profile"></param>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        private Dictionary<string, string> GetHeaders(string profile, string accessToken)
        {
            var headers = new Dictionary<string, string>();
            switch (profile)
            {
                case "bearer":
                    headers.Add("Authorization", $"Bearer {accessToken}");
                    break;

                default:
                    break;
            }
            return headers;
        }

        /// <summary>
        /// Throws an exception if the response does not indicate one of the
        /// success status codes (typically 200)
        /// </summary>
        /// <param name="webResponse"></param>
        /// <param name="successStatusCodes"></param>
        protected void ThrowExceptionIfNotSuccess(HttpWebResponse webResponse, List<HttpStatusCode> successStatusCodes)
        {
            if (webResponse == null)
            {
                throw new Exception("No web response received");
            }
            else if (!successStatusCodes.Contains(webResponse.StatusCode))
            {
                throw new Exception(string.Format("Web request returns status {0}", webResponse.StatusCode));
            }
        }
    }
}
