using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.Results;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.SmoothConfigurations;
using ImagineCommunications.GamePlan.Domain.SmoothConfigurations.Objects;
using ImagineCommunications.GamePlan.Domain.SmoothFailureMessages;
using ImagineCommunications.GamePlan.Process.Smooth.Services;
using xggameplan.Filters;
using xggameplan.Model;
using xggameplan.Services;

namespace xggameplan.Controllers
{
    [RoutePrefix("Smooth")]
    public class SmoothController : ApiController
    {
        private readonly ISmoothFailureMessageRepository _smoothFailureMessageRepository;
        private readonly ISmoothConfigurationRepository _smoothConfigurationRepository;
        private readonly IMapper _mapper;

        public SmoothController(
            ISmoothFailureMessageRepository smoothFailureMessageRepository,
            ISmoothConfigurationRepository smoothConfigurationRepository,
            IMapper mapper)
        {
            _smoothFailureMessageRepository = smoothFailureMessageRepository;
            _smoothConfigurationRepository = smoothConfigurationRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Returns list of Smooth failure reasons
        /// </summary>
        [Route("FailureMessages")]
        [AuthorizeRequest("Smooth")]
        [ResponseType(typeof(SmoothFailureMessageModel))]
        public IHttpActionResult GetFailureReasons()
        {
            var failureMessages = _smoothFailureMessageRepository.GetAll();
            var failureTypeModels = _mapper.Map<List<SmoothFailureMessageModel>>(failureMessages);
            return Ok(failureTypeModels);
        }

        /// <summary>
        /// Returns list of best break factors
        /// </summary>
        [Route("BestBreakFactors")]
        [AuthorizeRequest("Smooth")]
        [ResponseType(typeof(BestBreakFactors))]
        public IHttpActionResult GetBestBreakFactors()
        {
            var list = new List<BestBreakFactors>();
            ((BestBreakFactors[])Enum.GetValues(typeof(BestBreakFactors))).ToList().ForEach(item => list.Add(item));
            return Ok(new { bestBreakFactors = list });
        }

        /// <summary>
        /// Validates Smooth configuration
        /// </summary>
        [Route("Configuration/{id}/Validate")]
        [AuthorizeRequest("Smooth")]
        [ResponseType(typeof(SmoothConfiguration))]
        public IHttpActionResult ExecuteSmoothConfigurationValidation([FromUri] int id)
        {
            List<string> results = new List<string>();

            try
            {
                var smoothConfiguration = _smoothConfigurationRepository.GetById(id);
                if (smoothConfiguration == null)
                {
                    return NotFound();
                }

                // Validate Smooth configuration
                SmoothConfigurationValidator smoothConfigurationValidator = new SmoothConfigurationValidator();
                results = smoothConfigurationValidator.Validate(smoothConfiguration);
            }
            catch (Exception exception)
            {
                results.Add($"Smooth configuration is corrupted: {exception.Message}");
            }

            return Ok(new { messages = results });
        }

        /// <summary>
        /// Returns exported Smooth configuration for passes, CSV format
        /// </summary>
        [Route("Configuration/{id}/Passes/Export")]
        [AuthorizeRequest("Smooth")]
        public IHttpActionResult GetSmoothConfigurationForPasses([FromUri] int id)
        {
            // Get Smooth configuration
            var smoothConfiguration = _smoothConfigurationRepository.GetById(id);
            if (smoothConfiguration == null)
            {
                return NotFound();
            }

            string exportFile = Path.Combine(System.Web.Hosting.HostingEnvironment.MapPath("/Temp"), "smooth_config_passes.txt");
            try
            {
                // Export
                SmoothConfigurationCSVExporter smoothConfigurationExporter = new SmoothConfigurationCSVExporter(smoothConfiguration, (Char)9);
                smoothConfigurationExporter.ExportPasses(exportFile);

                // Return exported file
                System.Web.Http.Results.ResponseMessageResult result = GetFile(exportFile, GetCompressionFormatForResponse(Request, new string[] { "gzip" }), true);
                if (result == null)
                {
                    return NotFound();
                }

                return result;
            }
            finally
            {
                if (File.Exists(exportFile))
                {
                    File.Delete(exportFile);
                }
            }
        }

        /// <summary>
        /// Returns exported Smooth configuration for best break factor groups, CSV format
        /// </summary>
        [Route("Configuration/{id}/BestBreakFactorGroups/Export")]
        [AuthorizeRequest("Smooth")]
        public IHttpActionResult GetSmoothConfigurationForBestBreakFactorGroups([FromUri] int id)
        {
            // Get Smooth configuration
            var smoothConfiguration = _smoothConfigurationRepository.GetById(id);
            if (smoothConfiguration == null)
            {
                return NotFound();
            }

            string exportFile = Path.Combine(System.Web.Hosting.HostingEnvironment.MapPath("/Temp"), "smooth_config_best_break_factor_groups.txt");
            try
            {
                // Export
                SmoothConfigurationCSVExporter smoothConfigurationExporter = new SmoothConfigurationCSVExporter(smoothConfiguration, (Char)9);
                smoothConfigurationExporter.ExportBestBreakFactorGroups(exportFile);

                // Return exported file
                System.Web.Http.Results.ResponseMessageResult result = GetFile(exportFile, GetCompressionFormatForResponse(Request, new string[] { "gzip" }), true);
                if (result == null)
                {
                    return NotFound();
                }

                return result;
            }
            finally
            {
                if (File.Exists(exportFile))
                {
                    File.Delete(exportFile);
                }
            }
        }

        /// <summary>
        /// Determines the compression format, if any, to use for the response for the request
        /// </summary>
        /// <param name="request"></param>
        /// <param name="compressionFormats"></param>
        /// <returns></returns>
        private string GetCompressionFormatForResponse(HttpRequestMessage request, string[] compressionFormats)
        {
            if (request.Headers.AcceptEncoding != null)
            {
                foreach (var item in request.Headers.AcceptEncoding)
                {
                    if (Array.IndexOf(compressionFormats, item.Value.ToString().ToLower()) != -1)
                    {
                        return item.Value.ToString();
                    }
                }
            }

            return "";
        }

        /// <summary>
        /// Gets file to return in HTTP response
        /// </summary>
        /// <param name="file"></param>
        /// <param name="compressionFormat"></param>
        /// <param name="attachment"></param>
        /// <returns></returns>
        private static ResponseMessageResult GetFile(string file, string compressionFormat, bool attachment)
        {
            string logFolder = System.Web.Hosting.HostingEnvironment.MapPath("/Logs");

            if (!File.Exists(file))
            {
                return null;
            }

            string compressedFile = "";
            try
            {
                HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
                switch (compressionFormat)
                {
                    case "gzip":
                        compressedFile = Path.Combine(logFolder, string.Format("{0}.zip", Guid.NewGuid().ToString()));
                        CompressionUtilities.CompressGZipToFile(file, compressedFile);
                        result.Content = new System.Net.Http.ByteArrayContent(System.IO.File.ReadAllBytes(compressedFile));
                        result.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");

                        // Return as attachment
                        if (attachment)
                        {
                            result.Content.Headers.Add("Content-Encoding", "gzip");
                        }

                        break;

                    default:
                        result.Content = new System.Net.Http.ByteArrayContent(System.IO.File.ReadAllBytes(file));
                        result.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");

                        break;
                }

                if (attachment)
                {
                    result.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
                    {
                        FileName = Path.GetFileName(file)
                    };
                }

                return new System.Web.Http.Results.ResponseMessageResult(result);
            }
            finally
            {
                // Clean up
                if (!String.IsNullOrEmpty(compressedFile) && File.Exists(compressedFile))
                {
                    File.Delete(compressedFile);
                }
            }
        }
    }
}
