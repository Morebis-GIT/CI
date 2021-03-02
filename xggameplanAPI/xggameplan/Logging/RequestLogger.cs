using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.IO;

namespace xggameplan.Logging
{
    /// <summary>
    /// Logging for HTTP request and response  
    /// </summary>
    public abstract class RequestLogger : DelegatingHandler
    {
        private string _ipFilter = "";                  // Client IP filtering, comma separated
        protected const string _defaultDateTimeFormat = "dd-MM-yyyy HH:mm:ss.fff";
        protected const string _defaultDateFormat = "dd-MM-yyyy";
        protected const string _defaultTimeFormat = "HH:mm:ss.fff";

        /// <summary>
        /// Enabled logging to be filtered for specific IP addresses. Multiple IP addresses should be specified as a comma separated list
        /// </summary>
        public string IPFilter
        {
            get { return _ipFilter; }
            set { _ipFilter = value; }
        }

        /// <summary>
        /// Handles request/response
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            DateTime requestTimestamp = DateTime.Now;
            string requestId = Guid.NewGuid().ToString();
            RequestInfo requestInfo = null;
            bool isValidForFilter = IsValidForFilter(request);

            // Read request
            if (isValidForFilter)
            {
                requestInfo = GetRequestInfo(requestId, requestTimestamp, request);
                if (request.Content != null)
                {
                    await request.Content.ReadAsByteArrayAsync()
                      .ContinueWith(task =>
                      {
                          requestInfo.Body = task.Result;
                      }, cancellationToken);
                }
            }

            // Read response
            return await base.SendAsync(request, cancellationToken)
                .ContinueWith(task =>
                {
                    HttpResponseMessage response = task.Result;
                    if (isValidForFilter)
                    {
                        try
                        {
                            ResponseInfo responseInfo = GetResponseInfo(requestId, DateTime.Now, response, requestTimestamp);
                            try
                            {
                                if (requestInfo != null)
                                {
                                    Log(requestInfo);
                                }
                            }
                            catch { };
                            try
                            {
                                if (responseInfo != null)
                                {
                                    Log(responseInfo);
                                }
                            }
                            catch { };
                        }
                        catch { };  // Ignore
                    }
                    return response;
                }, cancellationToken);
        }

        /// <summary>
        /// Returns physical path from the path specified
        /// </summary>
        /// <param name="folder">Path (Physical/Virtual)</param>
        /// <returns>Virtual path</returns>
        protected static string GetPhysicalPath(string folder)
        {
            if (folder.Contains(@"/"))   // Virtual path
            {
                return System.Web.Hosting.HostingEnvironment.MapPath(folder);
            }
            return folder;
        }

        /// <summary>
        /// Replaces fields in input string
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
	    protected static string ReplaceFields(string input)
        {
            string[] placeholders = new string[] { "{date}", "{time}", "{machinename}" };
            string[] values = new string[] { DateTime.Now.ToString(_defaultDateFormat), DateTime.Now.ToString(_defaultTimeFormat), Environment.MachineName };
            string output = input;
            for (int index = 0; index < placeholders.Length; index++)
            {
                output = output.Replace(placeholders[index], values[index]);
            }
            return output;
        }

        /// <summary>
        /// Log request content
        /// </summary>
        /// <param name="requestInfo"></param>
        /// <param name="file"></param>
        protected void LogContent(RequestInfo requestInfo, string file)
        {
            if (requestInfo.Body != null && requestInfo.Body.Length > 0)
            {
                File.WriteAllBytes(file, requestInfo.Body);
            }
        }

        /// <summary>
        /// Log response content
        /// </summary>
        /// <param name="responseInfo"></param>
        /// <param name="file"></param>
        protected void LogContent(ResponseInfo responseInfo, string file)
        {
            if (responseInfo.Body != null && responseInfo.Body.Length > 0)
            {
                File.WriteAllBytes(file, responseInfo.Body);
            }
        }

        private static string GetClientIp(HttpRequestMessage request)
        {
            string ip = "";
            //const string RemoteEndpointMessage = "System.ServiceModel.Channels.RemoteEndpointMessageProperty";

            try
            {
                if (request.Properties.ContainsKey("MS_HttpContext"))
                {
                    ip = ((HttpContextWrapper)request.Properties["MS_HttpContext"]).Request.UserHostAddress;
                }
                /*
                else if (request.Properties.ContainsKey(RemoteEndpointMessage.Name))
                {
                    dynamic prop = request.Properties[RemoteEndpointMessage.Name];
                    return prop.Address;
                }
                */
            }
            catch { };
            return ip;
        }

        private bool IsValidForFilter(HttpRequestMessage request)
        {
            if (String.IsNullOrEmpty(_ipFilter))
            {
                return true;
            }
            string[] ips = _ipFilter.Replace(" ", "").Split(',');
            string ip = GetClientIp(request);
            return Array.IndexOf(ips, ip) != -1;
        }

        private static ResponseInfo GetResponseInfo(string requestId, DateTime responseTime, HttpResponseMessage response, DateTime requestTime)
        {
            var responseLogInfo = new ResponseInfo() { RequestID = requestId };

            TimeSpan elapsed = responseTime - requestTime;

            responseLogInfo.StatusCode = (int)response.StatusCode;
            responseLogInfo.Timestamp = responseTime;
            responseLogInfo.ElapsedTime = (int)elapsed.TotalMilliseconds;

            if (response.Content != null)
            {
                responseLogInfo.Body = response.Content.ReadAsByteArrayAsync().Result;
                responseLogInfo.ContentType = response.Content.Headers.ContentType.MediaType;
                responseLogInfo.Headers = GetHeaders(response.Content.Headers);
            }
            return responseLogInfo;
        }

        private static RequestInfo GetRequestInfo(string requestId, DateTime requestTime, HttpRequestMessage request)
        {
            var requestLogInfo = new RequestInfo() { RequestID = requestId, Timestamp = requestTime };
            try
            {
                foreach (var header in request.Headers)
                {
                    if (header.Key.Equals("Content-Type"))
                    {
                        requestLogInfo.ContentType = header.Value.ToString();
                    }
                }
                requestLogInfo.IpAddress = GetClientIp(request);
                requestLogInfo.Method = request.Method.Method;
                requestLogInfo.Headers = GetHeaders(request.Headers);
                requestLogInfo.Timestamp = requestTime;
                requestLogInfo.Uri = request.RequestUri.ToString();
            }
            catch { };
            return requestLogInfo;
        }

        /// <summary>
        /// Logs request
        /// </summary>
        /// <param name="requestInfo"></param>
        protected virtual void Log(RequestInfo requestInfo)
        {

        }

        /// <summary>
        /// Logs response
        /// </summary>
        /// <param name="responseInfo"></param>
        protected virtual void Log(ResponseInfo responseInfo)
        {

        }

        private static Dictionary<string, string> GetHeaders(HttpContentHeaders headers)
        {
            var output = new Dictionary<string, string>();
            foreach (var item in headers.ToList())
            {
                output.Add(item.Key, headers.GetValues(item.Key).First().ToString());
            }
            return output;
        }

        private static Dictionary<string, string> GetHeaders(HttpRequestHeaders headers)
        {
            var output = new Dictionary<string, string>();
            foreach (var item in headers.ToList())
            {
                output.Add(item.Key, headers.GetValues(item.Key).First().ToString());
            }
            return output;
        }
    }
}
