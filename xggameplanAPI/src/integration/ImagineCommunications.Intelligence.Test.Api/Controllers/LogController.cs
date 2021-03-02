using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using ImagineCommunications.Intelligence.Test.Api.Helpers;
using ImagineCommunications.Intelligence.Test.Api.Services;

namespace ImagineCommunications.Intelligence.Test.Api.Controllers
{
    public class LogController : ApiController
    {
        private const string DateFormat = "dd-MM-yyyy";
        private readonly ILogFileExportService _logFileExportService;

        public LogController(ILogFileExportService logFileExportService)
        {
            _logFileExportService = logFileExportService;
        }

        [HttpGet]
        public async Task<IHttpActionResult> Get([FromUri] string date = "")
        {
            try
            {
                var incomingDate = String.IsNullOrEmpty(date) ? DateTime.UtcNow.Date : DateHelper.GetDate(date, DateFormat);
                var fileModel = await _logFileExportService.GetLogFileAsync(incomingDate);

                if (fileModel == null)
                {
                    return NotFound();
                }

                var result = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new ByteArrayContent(fileModel.Content),
                };
                result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                {
                    FileName = fileModel.Name
                };
                result.Content.Headers.ContentType = new MediaTypeHeaderValue(_logFileExportService.MediaType);
                result.Content.Headers.ContentEncoding.Add("gzip");

                return new ResponseMessageResult(result);
            }
            catch (ArgumentException)
            {
                return BadRequest();
            }
        }
    }
}
