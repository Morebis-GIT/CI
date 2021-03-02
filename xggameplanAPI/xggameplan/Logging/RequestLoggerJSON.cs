using System;
using System.IO;
using System.Text;

namespace xggameplan.Logging
{
    /// <summary>
    /// Logging HTTP request/response to JSON file
    /// 
    /// To use: HttpConfiguration.MessageHandlers.Add(new RequestLoggerJSON());
    /// </summary>
    public class RequestLoggerJSON : RequestLogger
    {
        private readonly string _outputFolder = "";
        private readonly bool _logRequests = false;
        private readonly bool _logResponses = false;
        private readonly bool _logContent = false;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="outputFolder">Folder to log output</param>
        /// <param name="logRequests">Whether to log requests</param>
        /// <param name="logResponses">Whether to log responses</param>
        /// <param name="logContent">Whether to log content</param>
        public RequestLoggerJSON(string outputFolder, bool logRequests, bool logResponses, bool logContent)
        {
            if (String.IsNullOrEmpty(outputFolder))
            {
                throw new ArgumentNullException(outputFolder);
            }

            _outputFolder = outputFolder;
            _logRequests = logRequests;
            _logResponses = logResponses;
            _logContent = logContent;

            Directory.CreateDirectory(GetPhysicalPath(ReplaceFields(_outputFolder)));
            if (logContent)
            {
                Directory.CreateDirectory(GetPhysicalPath(ReplaceFields(_outputFolder) + @"\Content"));
            }
        }

        protected override void Log(RequestInfo requestInfo)
        {
            if (requestInfo != null)
            {
                string requestFile = Path.Combine(GetPhysicalPath(ReplaceFields(_outputFolder)), String.Format("{0}.request.json", requestInfo.RequestID));
                File.WriteAllText(requestFile, GetJSONString(requestInfo));

                if (_logContent && requestInfo.Body != null && requestInfo.Body.Length > 0)
                {
                    string contentFile = Path.Combine(GetPhysicalPath(ReplaceFields(_outputFolder)) + @"\Content", String.Format("{0}.request.content", requestInfo.RequestID));
                    LogContent(requestInfo, contentFile);
                }
            }
        }

        protected override void Log(ResponseInfo responseInfo)
        {
            if (responseInfo != null)
            {
                string responseFile = Path.Combine(GetPhysicalPath(ReplaceFields(_outputFolder)), String.Format("{0}.response.json", responseInfo.RequestID));
                File.WriteAllText(responseFile, GetJSONString(responseInfo));

                if (_logContent && responseInfo.Body != null && responseInfo.Body.Length > 0)
                {
                    string contentFile = Path.Combine(GetPhysicalPath(ReplaceFields(_outputFolder)) + @"\Content", String.Format("{0}.response.content", responseInfo.RequestID));
                    LogContent(responseInfo, contentFile);
                }
            }
        }

        private static string GetJSONString(RequestInfo requestInfo)
        {
            char quotes = '"';
            var json = new StringBuilder("{" + Environment.NewLine);
            json.Append(String.Format("\t{0}{1}{0} : {0}{2}{0}", quotes, "RequestID", requestInfo.RequestID) + Environment.NewLine);
            json.Append(String.Format("\t{0}{1}{0} : {0}{2}{0}", quotes, "Timestamp", requestInfo.Timestamp.ToString(_defaultDateTimeFormat)) + Environment.NewLine);
            json.Append(String.Format("\t{0}{1}{0} : {0}{2}{0}", quotes, "Method", requestInfo.Method) + Environment.NewLine);
            json.Append(String.Format("\t{0}{1}{0} : {0}{2}{0}", quotes, "ContentType", requestInfo.ContentType) + Environment.NewLine);
            json.Append(String.Format("\t{0}{1}{0} : {0}{2}{0}", quotes, "URI", requestInfo.Uri) + Environment.NewLine);
            json.Append(String.Format("\t{0}{1}{0} : {0}{2}{0}", quotes, "IPAddress", requestInfo.IpAddress) + Environment.NewLine);

            // Add headers
            json.Append(String.Format("\t{0}{1}{0} : ", quotes, "Headers") + "{" + Environment.NewLine);
            foreach (string header in requestInfo.Headers.Keys)
            {
                json.Append(String.Format("\t\t{0}{1}{0} : {0}{2}{0}", quotes, header, requestInfo.Headers[header].ToString()) + Environment.NewLine);
            }
            json.Append("\t}" + Environment.NewLine);
            json.Append("}" + Environment.NewLine);
            return json.ToString();
        }

        private static string GetJSONString(ResponseInfo responseInfo)
        {
            char quotes = '"';
            var json = new StringBuilder("{" + Environment.NewLine);
            json.Append(String.Format("\t{0}{1}{0} : {0}{2}{0}", quotes, "RequestID", responseInfo.RequestID) + Environment.NewLine);
            json.Append(String.Format("\t{0}{1}{0} : {0}{2}{0}", quotes, "Timestamp", responseInfo.Timestamp.ToString(_defaultDateTimeFormat)) + Environment.NewLine);
            json.Append(String.Format("\t{0}{1}{0} : {0}{2}{0}", quotes, "ContentType", responseInfo.ContentType) + Environment.NewLine);
            json.Append(String.Format("\t{0}{1}{0} : {0}{2}{0}", quotes, "StatusCode", responseInfo.StatusCode) + Environment.NewLine);

            // Add headers
            json.Append(String.Format("\t{0}{1}{0} : ", quotes, "Headers") + "{" + Environment.NewLine);
            foreach (string header in responseInfo.Headers.Keys)
            {
                json.Append(String.Format("\t\t{0}{1}{0} : {0}{2}{0}", quotes, header, responseInfo.Headers[header].ToString()) + Environment.NewLine);
            }
            json.Append("\t}" + Environment.NewLine);
            json.Append("}" + Environment.NewLine);
            return json.ToString();
        }
    }
}
