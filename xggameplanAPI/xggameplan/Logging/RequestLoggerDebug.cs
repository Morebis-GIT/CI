using System;

namespace xggameplan.Logging
{
    /// <summary>
    /// Logging HTTP request/response to Visual Studio debug window. The fields that are logged are controlled by templates
    /// for the request and response. The constructors allow you to use the default templates or custom templates.
    /// 
    /// To use: HttpConfiguration.MessageHandlers.Add(new RequestLoggerDebug());
    /// </summary>
    public class RequestLoggerDebug : RequestLogger
    {
        private readonly string _requestTemplate = "";
        private readonly string _responseTemplate = "";

        // Template fields
        public static string TemplateFieldRequestID = "{requestid}";
        public static string TemplateFieldTimestamp = "{timestamp}";
        public static string TemplateFieldMethod = "{method}";
        public static string TemplateFieldContentType = "{contenttype}";
        public static string TemplateFieldURI = "{uri}";
        public static string TemplateFieldStatusCode = "{statuscode}";
        public static string TemplateFieldIPAddress = "{ipaddress}";

        /// <summary>
        /// Constructor, uses default template for request and response
        /// </summary>
        public RequestLoggerDebug()
        {
            _requestTemplate = "[I] " + TemplateFieldRequestID + " " + TemplateFieldTimestamp + " " + TemplateFieldMethod + " " + TemplateFieldContentType + " " + TemplateFieldURI;
            _responseTemplate = "[O] " + TemplateFieldRequestID + " " + TemplateFieldTimestamp + " " + TemplateFieldStatusCode + " " + TemplateFieldContentType;
        }

        /// <summary>
        /// Constructor, uses custom template for request and response. To disable output then pass empty template
        /// </summary>
        /// <param name="requestTemplate">Template for displaying request, contains fields to display</param>
        /// <param name="responseTemplate">Template for displaying response, contains fields to display</param>
        public RequestLoggerDebug(string requestTemplate, string responseTemplate)
        {
            if (String.IsNullOrEmpty(requestTemplate) && String.IsNullOrEmpty(responseTemplate))
            {
                throw new ArgumentException("At least one template must be set");
            }

            _requestTemplate = requestTemplate;
            _responseTemplate = responseTemplate;
        }

        protected override void Log(RequestInfo requestInfo)
        {
            if (!String.IsNullOrEmpty(_requestTemplate) && requestInfo != null)
            {
                string line = _requestTemplate.Replace(TemplateFieldRequestID, requestInfo.RequestID)
                            .Replace(TemplateFieldTimestamp, requestInfo.Timestamp.ToString(_defaultTimeFormat))
                            .Replace(TemplateFieldMethod, requestInfo.Method)
                            .Replace(TemplateFieldContentType, requestInfo.ContentType)
                            .Replace(TemplateFieldURI, requestInfo.Uri)
                            .Replace(TemplateFieldIPAddress, requestInfo.IpAddress);

                System.Diagnostics.Debug.WriteLine(ReplaceFields(line));
            }
        }

        protected override void Log(ResponseInfo responseInfo)
        {
            if (!String.IsNullOrEmpty(_responseTemplate) && responseInfo != null)
            {
                string line = _responseTemplate.Replace(TemplateFieldRequestID, responseInfo.RequestID)
                            .Replace(TemplateFieldTimestamp, responseInfo.Timestamp.ToString(_defaultTimeFormat))
                            .Replace(TemplateFieldStatusCode, responseInfo.StatusCode.ToString())
                            .Replace(TemplateFieldContentType, responseInfo.ContentType);

                System.Diagnostics.Debug.WriteLine(ReplaceFields(line));
            }
        }
    }
}
