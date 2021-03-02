using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Cache;
using System.Net.Mail;

namespace xggameplan.common.Email
{
    /// <summary>
    /// Sends emails via SendGrid REST API
    /// </summary>
    public class SendGridHttpEmailConnection : IEmailConnection
    {
        private readonly EmailConnectionSettings _emailConnectionSettings;
        private const string BaseUrl = "https://api.sendgrid.com/v3";

        public SendGridHttpEmailConnection(EmailConnectionSettings emailConnectionSettings)
        {
            _emailConnectionSettings = emailConnectionSettings;
        }

        public void SendEmail(MailMessage message)
        {
            // Get the body for the request
            string content = GetEmailRequestBody(message);

            // Send request
            var headers = new Dictionary<string, string>();
            headers.Add("Authorization", string.Format("Bearer {0}", (string)_emailConnectionSettings.Settings["APIKey"]));
            HttpWebRequest webRequest = CreateHttpWebRequest(string.Format("{0}/mail/send", BaseUrl), "POST", headers, "application/json", System.Text.Encoding.UTF8.GetBytes(content));
            var webResponse = (HttpWebResponse)webRequest.GetResponse();
            ThrowExceptionIfNotSuccess(webResponse, new List<HttpStatusCode>() { HttpStatusCode.OK, HttpStatusCode.Accepted });
        }

        /// <summary>
        /// Returns the email in JSON format for the request body
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private static string GetEmailRequestBody(MailMessage message)
        {
            const char quotes = '"';

            string json = "{" + Environment.NewLine +
                "\t" + quotes + "personalizations" + quotes + ": [" + Environment.NewLine +
                "\t\t" + "{" + Environment.NewLine +
                "\t\t\t" + quotes + "to" + quotes + ": [" + Environment.NewLine +
                "\t\t\t\t" + "{" + Environment.NewLine +
                "\t\t\t\t\t" + quotes + "email" + quotes + ": " + quotes + message.To[0].Address + quotes + "," + Environment.NewLine +
                "\t\t\t\t\t" + quotes + "name" + quotes + ": " + quotes + message.To[0].DisplayName + quotes + Environment.NewLine +
                "\t\t\t\t" + "}" + Environment.NewLine +
                "\t\t\t" + "]," + Environment.NewLine +
                "\t\t\t" + quotes + "subject" + quotes + ": " + quotes + message.Subject + quotes + Environment.NewLine +
                "\t\t" + "}" + Environment.NewLine +
                "\t" + "]," + Environment.NewLine +
                "\t" + quotes + "from" + quotes + ": {" + Environment.NewLine +
                "\t\t" + quotes + "email" + quotes + ": " + quotes + message.From.Address + quotes + "," + Environment.NewLine +
                "\t\t" + quotes + "name" + quotes + ": " + quotes + message.From.DisplayName + quotes + Environment.NewLine +
                "\t" + "}," + Environment.NewLine +
                "\t" + quotes + "content" + quotes + ": [" + Environment.NewLine +
                "\t\t" + "{" + Environment.NewLine +
                "\t\t\t" + quotes + "type" + quotes + ": " + quotes + (message.IsBodyHtml ? "text/html" : "text/plain") + quotes + "," + Environment.NewLine +
                "\t\t\t" + quotes + "value" + quotes + ": " + quotes + message.Body + quotes + Environment.NewLine +
                "\t\t" + "}" + Environment.NewLine +
                "\t" + "]" + Environment.NewLine +
            "}";
            return json;
        }

        private static HttpWebRequest CreateHttpWebRequest(string url, string method, Dictionary<string, string> headers,
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

        private static void ThrowExceptionIfNotSuccess(HttpWebResponse webResponse, IEnumerable<HttpStatusCode> successStatusCodes)
        {
            if (Array.IndexOf(successStatusCodes.ToArray(), webResponse.StatusCode) == -1)
            {
                throw new Exception(string.Format("Web request returns status {0}", webResponse.StatusCode));
            }
        }
    }
}
