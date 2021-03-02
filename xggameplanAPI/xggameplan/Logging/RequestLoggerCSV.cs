using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace xggameplan.Logging
{
    /// <summary>
    /// Logging HTTP request/response to CSV file
    /// 
    /// To use: HttpConfiguration.MessageHandlers.Add(new RequestLoggerCSV());
    /// </summary>
    public class RequestLoggerCSV : RequestLogger
    {
        private readonly string _outputFolder = "";
        private readonly bool _logRequests = false;
        private readonly bool _logResponses = false;
        private readonly bool _logContent = false;
        private const char _delimiter = '\t';
        private const int _maxHeaders = 30;     // Number of headers to log in CSV

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="outputFolder">Folder to log output to</param>        
        /// <param name="logRequests">Whether to log requests</param>
        /// <param name="logResponses">Whether to log responses</param>
        /// <param name="logContent">Whether to log request/response content</param>
        public RequestLoggerCSV(string outputFolder, bool logRequests, bool logResponses, bool logContent)
        {
            if (String.IsNullOrEmpty(outputFolder))
            {
                throw new ArgumentNullException(outputFolder);
            }
            if (!logRequests && !logResponses)
            {
                throw new ArgumentException("Cannot log nothing at all");
            }

            _outputFolder = outputFolder;
            _logRequests = logRequests;
            _logResponses = logResponses;
            _logContent = logContent;

            Directory.CreateDirectory(GetPhysicalPath(ReplaceFields(outputFolder)));
            if (logContent)
            {
                Directory.CreateDirectory(GetPhysicalPath(ReplaceFields(outputFolder) + @"\Content"));
            }
        }

        private static string GetDelimiterExtension(char delimiter)
        {
            switch (delimiter)
            {
                case '\t': return ".txt";
                default: return ".csv";
            }
        }

        protected override void Log(RequestInfo requestInfo)
        {
            if (!_logRequests || requestInfo == null)
            {
                return;
            }

            string requestFile = Path.Combine(GetPhysicalPath(ReplaceFields(_outputFolder)), ReplaceFields("{date}.requests" + GetDelimiterExtension(_delimiter)));
            bool writeHeaders = !File.Exists(requestFile);
            short attempts = 0;
            const short maxAttempts = 10;

            // Save to CSV, handle file locking
            do
            {
                try
                {
                    attempts++;
                    using (var writer = new StreamWriter(requestFile, true))
                    {
                        if (writeHeaders)
                        {
                            writer.WriteLine(GetCSVRequestColumnHeaders());
                        }
                        writer.WriteLine(GetCSVLine(requestInfo));
                        writer.Flush();
                        writer.Close();
                        attempts = Int16.MaxValue;      // Exit while
                    }
                }
                catch (Exception exception)   // Handle file locking
                {
                    if (IsFileInUseException(exception))
                    {
                        if (attempts >= maxAttempts)
                        {
                            throw;
                        }
                        Thread.Sleep(100);  // Wait before retry
                    }
                    else
                    {
                        throw;
                    }
                }
            } while (attempts < maxAttempts);

            if (_logContent)
            {
                string contentFile = Path.Combine(GetPhysicalPath(ReplaceFields(_outputFolder) + @"\Content"), String.Format("{0}.request.content", requestInfo.RequestID));
                LogContent(requestInfo, contentFile);
            }
        }

        private static bool IsFileInUseException(Exception exception) => exception.Message.Contains("being used by another process");

        protected override void Log(ResponseInfo responseInfo)
        {
            if (!_logResponses || responseInfo == null)
            {
                return;
            }

            string responseFile = Path.Combine(GetPhysicalPath(ReplaceFields(_outputFolder)), ReplaceFields("{date}.responses" + GetDelimiterExtension(_delimiter)));
            bool writeHeaders = !File.Exists(responseFile);
            short attempts = 0;
            const short maxAttempts = 10;

            // Save to CSV, handle file locking
            do
            {
                try
                {
                    attempts++;
                    using (var writer = new StreamWriter(responseFile, true))
                    {
                        if (writeHeaders)
                        {
                            writer.WriteLine(GetCSVResponseColumnHeaders());
                        }
                        writer.WriteLine(GetCSVLine(responseInfo));
                        writer.Flush();
                        writer.Close();
                        attempts = Int16.MaxValue;      // Exit while
                    }
                }
                catch (Exception exception)   // Handle file locking
                {
                    if (IsFileInUseException(exception))
                    {
                        if (attempts >= maxAttempts)
                        {
                            throw;
                        }
                        Thread.Sleep(100);  // Wait before retry
                    }
                    else
                    {
                        throw;
                    }
                }
            } while (attempts < maxAttempts);

            if (_logContent)
            {
                string contentFile = Path.Combine(GetPhysicalPath(ReplaceFields(_outputFolder)) + @"\Content", String.Format("{0}.response.content", responseInfo.RequestID));
                LogContent(responseInfo, contentFile);
            }
        }

        private string GetCSVRequestColumnHeaders()
        {
            var line = new StringBuilder("RequestID" + _delimiter + "Timestamp" + _delimiter + "Method" + _delimiter + "ContentType" + _delimiter + "ContentLength" +
                                                _delimiter + "IPAddress" + _delimiter + "URI");
            for (int index = 0; index < _maxHeaders; index++)
            {
                line.Append(String.Format("{0}H{1}", _delimiter, index + 1));
            }
            return line.ToString();
        }

        private string GetCSVLine(RequestInfo requestInfo) => String.Format("{1}{0}{2}{0}{3}{0}{4}{0}{5}{0}{6}{0}{7}{0}{8}", _delimiter, requestInfo.RequestID, requestInfo.Timestamp.ToString(_defaultDateTimeFormat), requestInfo.Method, requestInfo.ContentType, requestInfo.Body.Length, requestInfo.IpAddress, requestInfo.Uri, GetHeadersCSVLine(requestInfo.Headers, _maxHeaders));

        private string GetCSVResponseColumnHeaders()
        {
            var line = new StringBuilder("RequestID" + _delimiter + "Timestamp" + _delimiter + "StatusCode" + _delimiter + "ContentType" + _delimiter + "ContentLength" + _delimiter + "ElapsedTime");
            for (int index = 0; index < _maxHeaders; index++)
            {
                line.Append(String.Format("{0}H{1}", _delimiter, index + 1));
            }
            return line.ToString();
        }

        private string GetCSVLine(ResponseInfo responseInfo) => String.Format("{1}{0}{2}{0}{3}{0}{4}{0}{5}{0}{6}{0}{7}", _delimiter, responseInfo.RequestID, responseInfo.Timestamp.ToString(_defaultDateTimeFormat), responseInfo.StatusCode, responseInfo.ContentType, responseInfo.Body.Length, responseInfo.ElapsedTime.GetValueOrDefault(0), GetHeadersCSVLine(responseInfo.Headers, _maxHeaders));

        /// <summary>
        /// Returns headers for CSV in format H1{delimiter}{H2} etc, limits number of headers
        /// </summary>
        /// <param name="headers"></param>
        /// <param name="maxHeaders"></param>
        /// <returns></returns>
        private string GetHeadersCSVLine(Dictionary<string, string> headers, int maxHeaders)
        {
            var line = new StringBuilder("");
            int headerCount = 0;
            foreach (string header in headers.Keys)
            {
                headerCount++;
                if (headerCount > 1)
                {
                    line.Append(_delimiter);
                }
                line.Append(String.Format("{0}={1}", header, headers[header]));
                if (headerCount >= maxHeaders)
                {
                    break;
                }
            }

            // Add empty remaining columns
            while (headerCount < maxHeaders)
            {
                headerCount++;
                line.Append(_delimiter);
            }
            return line.ToString();
        }
    }
}
