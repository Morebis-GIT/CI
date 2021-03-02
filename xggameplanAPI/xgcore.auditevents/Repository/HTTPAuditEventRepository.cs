using System;
using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json;

namespace xggameplan.AuditEvents
{
    /// <summary>
    /// HTTP audit event repository, passes event to HTTP endpoint
    /// </summary>
    public class HTTPAuditEventRepository : IAuditEventRepository
    {
        private List<HTTPAuditEventSettings> _httpAuditEventSettingsList = null;
        private const string _contentTypeJSON = "application/json";
        private const int _defaultTimeout = 60 * 1000 * 60;     // 1hr, allows callee to perform synchronous processing

        public HTTPAuditEventRepository(List<HTTPAuditEventSettings> httpAuditEventSettingsList)
        {
            _httpAuditEventSettingsList = httpAuditEventSettingsList;
        }

        public void Insert(AuditEvent auditEvent)
        {
            if (!Handles(auditEvent))
            {
                return;
            }
            HTTPAuditEventSettings auditEventSettings = _httpAuditEventSettingsList.Find(aes => aes.EventTypeId == auditEvent.EventTypeID);
            ExecuteREST(auditEvent, auditEventSettings, null);
        }

        public List<AuditEvent> Get(AuditEventFilter auditEventFilter)
        {
            throw new NotImplementedException();
        }

        public void Delete(AuditEventFilter auditEventFilter)
        {
            // No action
        }

        private bool Handles(AuditEvent auditEvent)
        {
            HTTPAuditEventSettings auditEventSettings = _httpAuditEventSettingsList.Find(aes => aes.EventTypeId == auditEvent.EventTypeID);
            return (auditEventSettings != null && auditEventSettings.RequestSettings != null && auditEventSettings.Enabled);
        }

        /// <summary>
        /// Executes request
        /// </summary>
        /// <param name="method"></param>
        /// <param name="placeholders"></param>
        private void ExecuteREST(AuditEvent auditEvent, HTTPAuditEventSettings auditEventSettings, Dictionary<string, string> placeholders)
        {
            if (auditEventSettings.Enabled)
            {
                HttpWebRequest webRequest = CreateHttpWebRequest(auditEvent, auditEventSettings.RequestSettings, placeholders);
                HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse();
                ThrowExceptionIfNotSuccess(webResponse, auditEventSettings.ResponseSettings.SuccessStatusCodes);
            }
        }

        /// <summary>
        /// Creates web request.
        /// </summary>
        /// <param name="auditEvent"></param>
        /// <param name="httpRequestSettings"></param>
        /// <param name="placeholders"></param>
        /// <returns></returns>
        private HttpWebRequest CreateHttpWebRequest(AuditEvent auditEvent, HTTPRequestSettings httpRequestSettings, Dictionary<string, string> placeholders)
        {
            string serialized = JsonConvert.SerializeObject(auditEvent, Formatting.Indented);

            // Set content
            string contentString = httpRequestSettings.ContentTemplate;
            contentString = contentString.Replace("{event_json}", serialized);
            if (!String.IsNullOrEmpty(contentString) && placeholders != null)
            {
                foreach (var placeholder in placeholders.Keys)
                {
                    contentString = contentString.Replace(placeholder, placeholders[placeholder]);
                }
            }

            // Process headers
            string contentType = "";
            Dictionary<string, string> headers = new Dictionary<string, string>();
            foreach (string header in httpRequestSettings.Headers.Keys)
            {
                switch (header)
                {
                    case "Content-Type":     // Should not be passed in headers collection
                        contentType = httpRequestSettings.Headers[header];
                        break;
                    default:
                        headers.Add(header, httpRequestSettings.Headers[header]);
                        break;
                }
            }

            // Set URL
            string url = httpRequestSettings.URL;
            if (placeholders != null)
            {
                foreach (var placeholder in placeholders.Keys)
                {
                    url = url.Replace(placeholder, placeholders[placeholder]);
                }
            }
            //url = url.Replace("{runId}", runId.ToString());

            HttpWebRequest webRequest = CreateHttpWebRequest(url, httpRequestSettings.Method, headers,
                                                String.IsNullOrEmpty(contentString) ? "" : contentType,
                                                String.IsNullOrEmpty(contentString) ? null : System.Text.Encoding.UTF8.GetBytes(contentString));
            webRequest.Timeout = _defaultTimeout;
            return webRequest;
        }

        protected HttpWebRequest CreateHttpWebRequest(string url, string method, Dictionary<string, string> headers,
                                                     string contentType, byte[] content)
        {
            WebRequest.DefaultWebProxy.Credentials = CredentialCache.DefaultNetworkCredentials;
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.Method = method;
            webRequest.CachePolicy = new System.Net.Cache.HttpRequestCachePolicy(System.Net.Cache.HttpRequestCacheLevel.NoCacheNoStore);
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
        /// Throws an exception if the response does not indicate one of the success status codes (typically 200)
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
