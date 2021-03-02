using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Cache;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Hosting;
using System.Web.Http;
using System.Web.Http.Results;
using ImagineCommunications.GamePlan.Domain.Generic.Helpers;
using xggameplan.AutoBooks.Abstractions;
using xggameplan.Errors;
using xggameplan.Filters;
using xggameplan.Services;

namespace xggameplan.Controllers
{
    /// <summary>
    /// API for accessing logs
    /// </summary>
    /// <remarks>Cannot use LogsController, clashes with existing and won't show in Swagger.</remarks>
    [RoutePrefix("Logs")]
    public class LogController : ApiController
    {
        private readonly IAutoBooks _autoBooks;

        private const string _dateFormat = "yyyy-MM-dd";

        public LogController(IAutoBooks autoBooks) => _autoBooks = autoBooks;

        /// <summary>
        /// Returns events log for specific date
        /// </summary>
        /// <returns></returns>
        [Route("events")]
        [AuthorizeRequest("Logs")]
        public async Task<IHttpActionResult> GetEvents([FromUri] string date = "")
        {
            date = String.IsNullOrEmpty(date) ? DateTime.UtcNow.ToString(_dateFormat) : date;   // Default to today
            if (!String.IsNullOrEmpty(date) && !DateHelper.CanConvertToDate(date, _dateFormat))
            {
                return this.Error().InvalidParameters("Invalid date");
            }

            ResponseMessageResult result = await ReturnLogFileAsync(
                "events",
                DateHelper.GetDate(date, _dateFormat, DateTimeKind.Utc),
                null,
                GetCompressionFormatForResponse(Request, new string[] { "gzip" }),
                true);

            if (result is null)
            {
                return NotFound();
            }

            return result;
        }

        /// <summary>
        /// Returns requests log for specific date
        /// </summary>
        /// <returns></returns>
        [Route("requests")]
        [AuthorizeRequest("Logs")]
        public async Task<IHttpActionResult> GetRequests([FromUri] string date = "")
        {
            date = String.IsNullOrEmpty(date) ? DateTime.UtcNow.ToString(_dateFormat) : date;   // Default to today
            if (!String.IsNullOrEmpty(date) && !DateHelper.CanConvertToDate(date, _dateFormat))
            {
                return this.Error().InvalidParameters("Invalid date");
            }

            ResponseMessageResult result = await ReturnLogFileAsync(
                "requests",
                DateHelper.GetDate(date, _dateFormat, DateTimeKind.Utc),
                null,
                GetCompressionFormatForResponse(Request, new string[] { "gzip" }),
                true);

            if (result is null)
            {
                return NotFound();
            }
            return result;
        }

        /// <summary>
        /// Returns requests log for specific date
        /// </summary>
        /// <returns></returns>
        [Route("responses")]
        [AuthorizeRequest("Logs")]
        public async Task<IHttpActionResult> GetResponses([FromUri] string date = "")
        {
            date = String.IsNullOrEmpty(date) ? DateTime.UtcNow.ToString(_dateFormat) : date;   // Default to today
            if (!String.IsNullOrEmpty(date) && !DateHelper.CanConvertToDate(date, _dateFormat))
            {
                return this.Error().InvalidParameters("Invalid date");
            }

            ResponseMessageResult result = await ReturnLogFileAsync(
                "responses",
                DateHelper.GetDate(date, _dateFormat, DateTimeKind.Utc),
                null,
                GetCompressionFormatForResponse(Request, new string[] { "gzip" }),
                true);

            if (result is null)
            {
                return NotFound();
            }
            return result;
        }

        /// <summary>
        /// Returns Smooth log for specific date and sales area
        /// </summary>
        /// <returns></returns>
        [Route("smooth")]
        [AuthorizeRequest("Logs")]
        public async Task<IHttpActionResult> GetSmooth([FromUri] string date, [FromUri] string salesArea = "")
        {
            date = String.IsNullOrEmpty(date) ? DateTime.UtcNow.ToString(_dateFormat) : date;   // Default to today
            if (!String.IsNullOrEmpty(date) && !DateHelper.CanConvertToDate(date, _dateFormat))
            {
                return this.Error().InvalidParameters("Invalid date");
            }
            if (String.IsNullOrEmpty(salesArea))
            {
                return this.Error().InvalidParameters("Sales area is not set");
            }

            ResponseMessageResult result = await ReturnLogFileAsync(
                "smooth",
                DateHelper.GetDate(date, _dateFormat, DateTimeKind.Utc),
                salesArea,
                GetCompressionFormatForResponse(Request, new string[] { "gzip" }),
                true);

            if (result is null)
            {
                return NotFound();
            }

            return result;
        }

        /// <summary>
        /// Returns Smooth spot action log for specific date and sales area
        /// </summary>
        /// <returns></returns>
        [Route("smoothspotactions")]
        [AuthorizeRequest("Logs")]
        public async Task<IHttpActionResult> GetSmoothSpotActions([FromUri] string date, [FromUri] string salesArea = "")
        {
            date = String.IsNullOrEmpty(date) ? DateTime.UtcNow.ToString(_dateFormat) : date;   // Default to today

            if (!String.IsNullOrEmpty(date) && !DateHelper.CanConvertToDate(date, _dateFormat))
            {
                return this.Error().InvalidParameters("Invalid date");
            }

            if (String.IsNullOrEmpty(salesArea))
            {
                return this.Error().InvalidParameters("Sales area is not set");
            }

            ResponseMessageResult result = await ReturnLogFileAsync(
                "smooth_spot_actions",
                DateHelper.GetDate(date, _dateFormat, DateTimeKind.Utc),
                salesArea,
                GetCompressionFormatForResponse(Request, new string[] { "gzip" }),
                true);

            if (result is null)
            {
                return NotFound();
            }

            return result;
        }

        /// <summary>
        /// Returns task log for specific date
        /// </summary>
        /// <returns></returns>
        [Route("tasks")]
        [AuthorizeRequest("Logs")]
        public async Task<IHttpActionResult> GetTasks([FromUri] string date = "")
        {
            date = String.IsNullOrEmpty(date) ? DateTime.UtcNow.ToString(_dateFormat) : date;   // Default to today
            if (!String.IsNullOrEmpty(date) && !DateHelper.CanConvertToDate(date, _dateFormat))
            {
                return this.Error().InvalidParameters("Invalid date");
            }

            ResponseMessageResult result = await ReturnLogFileAsync(
                "tasks",
                DateHelper.GetDate(date, _dateFormat, DateTimeKind.Utc),
                null,
                GetCompressionFormatForResponse(Request, new string[] { "gzip" }),
                true);

            if (result is null)
            {
                return NotFound();
            }
            return result;
        }

        /// <summary>
        /// Returns scheduled task log for specific date
        /// </summary>
        /// <returns></returns>
        [Route("scheduledtasks")]
        [AuthorizeRequest("Logs")]
        public async Task<IHttpActionResult> GetScheduledTasks([FromUri] string date = "")
        {
            date = String.IsNullOrEmpty(date) ? DateTime.UtcNow.ToString(_dateFormat) : date;   // Default to today
            if (!String.IsNullOrEmpty(date) && !DateHelper.CanConvertToDate(date, _dateFormat))
            {
                return this.Error().InvalidParameters("Invalid date");
            }

            ResponseMessageResult result = await ReturnLogFileAsync(
                "TaskLog",
                DateHelper.GetDate(date, _dateFormat, DateTimeKind.Utc),
                null,
                GetCompressionFormatForResponse(Request, new string[] { "gzip" }),
                true);

            if (result is null)
            {
                return NotFound();
            }
            return result;
        }

        /// <summary>
        /// Determines the compression format, if any, to use for the response for the request
        /// </summary>
        /// <param name="request"></param>
        /// <param name="compressionFormats"></param>
        /// <returns></returns>
        private string GetCompressionFormatForResponse(HttpRequestMessage request, string[] compressionFormats)
        {
            if (request.Headers.AcceptEncoding is null)
            {
                return string.Empty;
            }

            foreach (var item in request.Headers.AcceptEncoding)
            {
                if (Array.IndexOf(compressionFormats, item.Value.ToString().ToLower()) != -1)
                {
                    return item.Value.ToString();
                }
            }

            return string.Empty;
        }

        private static HttpWebRequest CreateHttpWebRequest(
            string url,
            string method,
            Dictionary<string, string> headers,
            int timeoutMilliseconds,
            string contentType,
            byte[] content)
        {
            WebRequest.DefaultWebProxy.Credentials = CredentialCache.DefaultNetworkCredentials;
            var webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.Method = method;
            webRequest.CachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore);
            webRequest.Timeout = timeoutMilliseconds > 0 ? timeoutMilliseconds : webRequest.Timeout;
            webRequest.KeepAlive = true;

            // Add request headers
            foreach (string key in headers.Keys)
            {
                webRequest.Headers.Add(key, headers[key]);
            }

            if (content != null)
            {
                webRequest.ContentType = contentType;
                webRequest.ContentLength = content.Length;
                webRequest.GetRequestStream().Write(content, 0, content.Length);
                webRequest.GetRequestStream().Close();
            }

            return webRequest;
        }

        /// <summary>
        /// Returns log file to return
        /// </summary>
        /// <param name="logType"></param>
        /// <param name="date"></param>
        /// <param name="salesArea"></param>
        /// <param name="compressionFormat"></param>
        /// <param name="attachment"></param>
        /// <returns></returns>
        private static async Task<ResponseMessageResult> ReturnLogFileAsync(
            string logType,
            DateTime date,
            string salesArea,
            string compressionFormat,
            bool attachment)
        {
            string logFolder = logType.ToLower().Equals("tasklog") ?
                HostingEnvironment.MapPath("/Scripts/Tasks/Logs") :
                HostingEnvironment.MapPath("/Logs");

            string formattedDate = date.ToString("dd-MM-yyyy");

            string logFile = string.IsNullOrEmpty(salesArea) ?
                Path.Combine(logFolder, $"{formattedDate}.{logType}.txt") :
                Path.Combine(logFolder, $"{formattedDate}.{salesArea}.{logType}.txt");

            if (!File.Exists(logFile))
            {
                return null;
            }

            string compressedFile = "";
            try
            {
                var result = new HttpResponseMessage(HttpStatusCode.OK);
                switch (compressionFormat)
                {
                    case "gzip":
                        string filename = Guid.NewGuid().ToString();
                        compressedFile = Path.Combine(logFolder, $"{filename}.zip");

                        await CompressionUtilities
                            .CompressGZipToFileAsync(logFile, compressedFile)
                            .ConfigureAwait(false);

                        result.Content = new ByteArrayContent(File.ReadAllBytes(compressedFile));
                        result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

                        if (attachment)
                        {
                            result.Content.Headers.Add("Content-Encoding", "gzip");
                        }
                        break;

                    default:
                        result.Content = new ByteArrayContent(File.ReadAllBytes(logFile));
                        result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                        break;
                }

                if (attachment)
                {
                    result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                    {
                        FileName = Path.GetFileName(logFile)
                    };
                }

                return new ResponseMessageResult(result);
            }
            finally
            {
                if (!String.IsNullOrEmpty(compressedFile) && File.Exists(compressedFile))
                {
                    File.Delete(compressedFile);
                }
            }
        }
    }
}
