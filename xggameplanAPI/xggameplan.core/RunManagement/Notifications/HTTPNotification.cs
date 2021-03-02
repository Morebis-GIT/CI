using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using ImagineCommunications.GamePlan.Domain.Runs.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.System.Settings;
using Newtonsoft.Json;

namespace xggameplan.RunManagement.Notifications
{
    /// <summary>
    /// Generates a notification via HTTP call (E.g. REST)
    /// </summary>
    public class HTTPTNotification : INotification<HTTPNotificationSettings>
    {
        private const int _defaultTimeout = 60 * 1000 * 60;     // 1hr, allows callee to perform synchronous processing

        public HTTPTNotification()
        {
        }

        public void RunCompleted(Run run, bool success, HTTPNotificationSettings httpNotificationSettings)
        {
            if (httpNotificationSettings.Enabled)
            {
                // Set content
                string contentString = httpNotificationSettings.MethodSettings.ContentTemplate;
                if (!String.IsNullOrEmpty(contentString))
                {
                    contentString = contentString.Replace("{runId}", run.Id.ToString());
                }

                // Process headers
                string contentType = "";
                Dictionary<string, string> headers = new Dictionary<string, string>();
                foreach (string header in httpNotificationSettings.MethodSettings.Headers.Keys)
                {
                    switch (header)
                    {
                        case "Content-Type":     // Should not be passed in headers collection
                            contentType = httpNotificationSettings.MethodSettings.Headers[header];
                            break;

                        default:
                            headers.Add(header, httpNotificationSettings.MethodSettings.Headers[header]);
                            break;
                    }
                }

                // Set URL
                string url = httpNotificationSettings.MethodSettings.URL;
                url = url.Replace("{runId}", run.Id.ToString());

                HttpWebRequest webRequest = CreateHttpWebRequest(url, httpNotificationSettings.MethodSettings.Method,
                                                    headers, _defaultTimeout,
                                                    String.IsNullOrEmpty(contentString) ? "" : contentType,
                                                    String.IsNullOrEmpty(contentString) ? null : System.Text.Encoding.UTF8.GetBytes(contentString));

                HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse();
                ThrowExceptionIfNotSuccess(webResponse, httpNotificationSettings.SucccessStatusCodes);
            }
        }

        /// <summary>
        /// Throw exception if HTTP status code does not indicate success
        /// </summary>
        /// <param name="webResponse"></param>
        /// <param name="successStatusCodes"></param>
        private void ThrowExceptionIfNotSuccess(HttpWebResponse webResponse, List<int> successStatusCodes)
        {
            if (webResponse == null)
            {
                throw new Exception("No web response received");
            }
            else if (!successStatusCodes.Contains(Convert.ToInt32(webResponse.StatusCode)))
            {
                throw new Exception(string.Format("Web request returns status {0}", webResponse.StatusCode));
            }
        }

        public void RunCompleted(Run run, RunScenario scenario, bool success, HTTPNotificationSettings httpNotificationSettings)
        {
            if (httpNotificationSettings.Enabled)
            {
                // Set content
                string contentString = httpNotificationSettings.MethodSettings.ContentTemplate;
                if (!String.IsNullOrEmpty(contentString))
                {
                    contentString = contentString.Replace("{runId}", run.Id.ToString());
                    contentString = contentString.Replace("{scenarioId}", scenario.Id.ToString());
                }

                // Process headers
                string contentType = "";
                Dictionary<string, string> headers = new Dictionary<string, string>();
                foreach (string header in httpNotificationSettings.MethodSettings.Headers.Keys)
                {
                    switch (header)
                    {
                        case "Content-Type":     // Should not be passed in headers collection
                            contentType = httpNotificationSettings.MethodSettings.Headers[header];
                            break;

                        default:
                            headers.Add(header, httpNotificationSettings.MethodSettings.Headers[header]);
                            break;
                    }
                }

                // Set URL
                string url = httpNotificationSettings.MethodSettings.URL;
                url = url.Replace("{runId}", run.Id.ToString());
                url = url.Replace("{scenarioId}", scenario.Id.ToString());

                HttpWebRequest webRequest = CreateHttpWebRequest(url, httpNotificationSettings.MethodSettings.Method,
                                                    headers, _defaultTimeout,
                                                    String.IsNullOrEmpty(contentString) ? "" : contentType,
                                                    String.IsNullOrEmpty(contentString) ? null : System.Text.Encoding.UTF8.GetBytes(contentString));

                HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse();
                ThrowExceptionIfNotSuccess(webResponse, httpNotificationSettings.SucccessStatusCodes);
            }
        }

        public void InventoryLock(Run run, HTTPNotificationSettings httpNotificationSettings)
        {
            if (!httpNotificationSettings.Enabled)
            {
                return;
            }
            // Set content
            string contentString = httpNotificationSettings.MethodSettings.ContentTemplate;
            if (!String.IsNullOrEmpty(contentString))
            {
                var payload = new
                {
                    salesAreas = run.SalesAreaPriorities,
                    campaigns = run.Campaigns,
                    id = run.Id,
                    description = run.Description,
                    startDateTime = run.StartDate,
                    endDateTime = run.EndDate,
                    author = new
                    {
                        id = run.Author.Id,
                        name = run.Author.Name,
                    },
                };
                contentString = contentString.Replace("{json}", JsonConvert.SerializeObject(payload));
            }

            // Process headers
            string contentType = "";
            Dictionary<string, string> headers = new Dictionary<string, string>();
            foreach (string header in httpNotificationSettings.MethodSettings.Headers.Keys)
            {
                switch (header)
                {
                    case "Content-Type":     // Should not be passed in headers collection
                        contentType = httpNotificationSettings.MethodSettings.Headers[header];
                        break;

                    default:
                        headers.Add(header, httpNotificationSettings.MethodSettings.Headers[header]);
                        break;
                }
            }

            // Set URL
            string url = httpNotificationSettings.MethodSettings.URL;

            HttpWebRequest webRequest = CreateHttpWebRequest(url, httpNotificationSettings.MethodSettings.Method,
                                                headers, _defaultTimeout,
                                                String.IsNullOrEmpty(contentString) ? "" : contentType,
                                                String.IsNullOrEmpty(contentString) ? null : Encoding.UTF8.GetBytes(contentString));

            HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse();
            ThrowExceptionIfNotSuccess(webResponse, httpNotificationSettings.SucccessStatusCodes);
        }

        public void InventoryUnlock(Run run, Guid? scenarioId, HTTPNotificationSettings httpNotificationSettings)
        {
            if (!httpNotificationSettings.Enabled)
            {
                return;
            }
            // Set content
            string contentString = httpNotificationSettings.MethodSettings.ContentTemplate;
            if (!String.IsNullOrEmpty(contentString))
            {
                var payload = new
                {
                    runid = run.Id,
                    scenarioId,
                };
                contentString = contentString.Replace("{json}", JsonConvert.SerializeObject(payload));
            }

            // Process headers
            string contentType = "";
            Dictionary<string, string> headers = new Dictionary<string, string>();
            foreach (string header in httpNotificationSettings.MethodSettings.Headers.Keys)
            {
                switch (header)
                {
                    case "Content-Type":     // Should not be passed in headers collection
                        contentType = httpNotificationSettings.MethodSettings.Headers[header];
                        break;

                    default:
                        headers.Add(header, httpNotificationSettings.MethodSettings.Headers[header]);
                        break;
                }
            }

            // Set URL
            string url = httpNotificationSettings.MethodSettings.URL;

            HttpWebRequest webRequest = CreateHttpWebRequest(url, httpNotificationSettings.MethodSettings.Method,
                                                headers, _defaultTimeout,
                                                String.IsNullOrEmpty(contentString) ? "" : contentType,
                                                String.IsNullOrEmpty(contentString) ? null : Encoding.UTF8.GetBytes(contentString));

            HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse();
            ThrowExceptionIfNotSuccess(webResponse, httpNotificationSettings.SucccessStatusCodes);
        }

        public void SmoothCompleted(Run run, bool success, HTTPNotificationSettings httpNotificationSettings)
        {
            if (httpNotificationSettings.Enabled)
            {
                // Set content
                string contentString = httpNotificationSettings.MethodSettings.ContentTemplate;
                if (!String.IsNullOrEmpty(contentString))
                {
                    contentString = contentString.Replace("{runId}", run.Id.ToString());
                }

                // Process headers
                string contentType = "";
                Dictionary<string, string> headers = new Dictionary<string, string>();
                foreach (string header in httpNotificationSettings.MethodSettings.Headers.Keys)
                {
                    switch (header)
                    {
                        case "Content-Type":     // Should not be passed in headers collection
                            contentType = httpNotificationSettings.MethodSettings.Headers[header];
                            break;

                        default:
                            headers.Add(header, httpNotificationSettings.MethodSettings.Headers[header]);
                            break;
                    }
                }

                // Set URL
                string url = httpNotificationSettings.MethodSettings.URL;
                url = url.Replace("{runId}", run.Id.ToString());

                HttpWebRequest webRequest = CreateHttpWebRequest(url, httpNotificationSettings.MethodSettings.Method,
                                                    headers, _defaultTimeout,
                                                    String.IsNullOrEmpty(contentString) ? "" : contentType,
                                                    String.IsNullOrEmpty(contentString) ? null : System.Text.Encoding.UTF8.GetBytes(contentString));

                HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse();
                ThrowExceptionIfNotSuccess(webResponse, httpNotificationSettings.SucccessStatusCodes);
            }
        }

        private static HttpWebRequest CreateHttpWebRequest(string url, string method, Dictionary<string, string> headers,
                                                           int timeoutMilliseconds,
                                                           string contentType, byte[] content)
        {
            WebRequest.DefaultWebProxy.Credentials = CredentialCache.DefaultNetworkCredentials;
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.Method = method;
            webRequest.CachePolicy = new System.Net.Cache.HttpRequestCachePolicy(System.Net.Cache.HttpRequestCacheLevel.NoCacheNoStore);
            webRequest.Timeout = (timeoutMilliseconds > 0 ? timeoutMilliseconds : webRequest.Timeout);
            webRequest.KeepAlive = true;

            // Add request headers
            foreach (string key in headers.Keys)
            {
                webRequest.Headers.Add(key, headers[key]);
            }

            // Set proxy
            //SetWebProxy(webRequest, httpRequest.ProxySettings);

            if (content != null)
            {
                webRequest.ContentType = contentType;
                webRequest.ContentLength = content.Length;
                webRequest.GetRequestStream().Write(content, 0, content.Length);
                webRequest.GetRequestStream().Close();
            }
            return webRequest;
        }
    }
}
