using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading.Tasks;
using System.Web.Hosting;
using System.Web.Http;
using System.Web.Http.Results;
using ImagineCommunications.GamePlan.Domain.Generic.Interfaces;
using ImagineCommunications.GamePlan.Domain.OutputFiles;
using xggameplan.core.Interfaces;
using xggameplan.core.TestEnvironment;
using xggameplan.Filters;
using xggameplan.TestEnv;

namespace xggameplan.Controllers
{
    [RoutePrefix("api")]
    [AuthorizeRequest("TestEnvironment")]
    public class TestEnvironmentMaintenanceController : ApiController
    {
        private readonly ITestEnvironmentDataService _testEnvironmentDataService;
        private readonly IOutputFileRepository _outputFileRepository;
        private readonly IEnumerable<IDatabaseIndexAwaiter> _indexAwaiters;
        private string[] _manifestResourceNames;

        public TestEnvironmentMaintenanceController(
            ITestEnvironmentDataService testEnvironmentDataService,
            IOutputFileRepository outputFileRepository,
            IEnumerable<IDatabaseIndexAwaiter> indexAwaiters)
        {
            _testEnvironmentDataService = testEnvironmentDataService;
            _outputFileRepository = outputFileRepository;
            _indexAwaiters = indexAwaiters;
        }

   
        [HttpPost]
        [Route("Tests/RunResult")]
        [TestEnvironment(TestEnvironmentFeatures.AutomationTests, TestEnvironmentFeatures.PerformanceTests)]
        public async Task<IHttpActionResult> PopulateRunResultData()
        {
            var outputFilesInfo = await PrepareOutputFilesAsync().ConfigureAwait(false);
            try
            {
                return Ok(_testEnvironmentDataService.PopulateRunResult(outputFilesInfo));
            }
            finally
            {
                CleanupOutputFiles(outputFilesInfo);
            }
        }

        [HttpPost]
        [Route("Tests/SmoothConfiguration")]
        [TestEnvironment(TestEnvironmentFeatures.AutomationTests, TestEnvironmentFeatures.PerformanceTests)]
        public IHttpActionResult PopulateSmoothConfiguration()
        {
            return Ok(_testEnvironmentDataService.PopulateSmoothConfiguration());
        }

        [HttpPost]
        [Route("Tests/WaitForIndexes")]
        [TestEnvironment(TestEnvironmentFeatures.AutomationTests, TestEnvironmentFeatures.PerformanceTests)]
        [TestEnvironmentIgnoreWaitForIndexes]
        public async Task<IHttpActionResult> WaitForIndexesAsync()
        {
            await Task.WhenAll(_indexAwaiters.Select(x => x.WaitForIndexesAsync())).ConfigureAwait(false);
            return Ok();
        }

        [HttpGet]
        [Route("Tests/GetPerformanceLogs/{date}")]
        [TestEnvironment(TestEnvironmentOptions.PerformanceLog)]
        [TestEnvironmentIgnoreWaitForIndexes]
        public async Task<IHttpActionResult> GetPerformanceTestsLog([FromUri] DateTime date)
        {
            var logFiles = PerformanceLog.FindLogFiles(date);
            if (!logFiles.Any())
            {
                return NotFound();
            }

            var content = await CreateLogFilesZip(logFiles).ConfigureAwait(false);
            var result = ActionContext.Request.CreateResponse(HttpStatusCode.OK);
            result.Content = new ByteArrayContent(content);
            result.Content.Headers.Add("IsCompressedContent", "true");
            result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                Size = content.Length,
                FileName = $"performance_test_logs_{date:yyyyMMdd}.zip"
            };

            result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/zip");
            return new ResponseMessageResult(result);
        }

        [HttpGet]
        [Route("Tests/GetPerformanceLogs")]
        [TestEnvironment(TestEnvironmentOptions.PerformanceLog)]
        [TestEnvironmentIgnoreWaitForIndexes]
        public async Task<IHttpActionResult> GetPerformanceTestsLog()
        {
            var logFiles = PerformanceLog.FindLogFiles();
            if (!logFiles.Any())
            {
                return NotFound();
            }

            var content = await CreateLogFilesZip(logFiles).ConfigureAwait(false);
            var result = ActionContext.Request.CreateResponse(HttpStatusCode.OK);
            result.Content = new ByteArrayContent(content);
            result.Content.Headers.Add("IsCompressedContent", "true");
            result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                Size = content.Length,
                FileName = $"performance_test_logs.zip"
            };

            result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/zip");
            return new ResponseMessageResult(result);
        }

        [HttpDelete]
        [Route("Tests/DeletePerformanceLogs/{date}")]
        [TestEnvironment(TestEnvironmentOptions.PerformanceLog)]
        [TestEnvironmentIgnoreWaitForIndexes]
        public IHttpActionResult DeletePerformanceTestsLog([FromUri] DateTime date)
        {
            var logFiles = PerformanceLog.FindLogFiles(date);
            if (!logFiles.Any())
            {
                return NotFound();
            }

            RemoveLogFiles(logFiles);
            return Ok();
        }

        [HttpDelete]
        [Route("Tests/DeletePerformanceLogs")]
        [TestEnvironment(TestEnvironmentOptions.PerformanceLog)]
        [TestEnvironmentIgnoreWaitForIndexes]
        public IHttpActionResult DeletePerformanceTestsLog()
        {
            var logFiles = PerformanceLog.FindLogFiles();
            if (!logFiles.Any())
            {
                return NotFound();
            }

            RemoveLogFiles(logFiles);
            return Ok();
        }

        private async Task<TestEnvironmentOutputFilesInfo> PrepareOutputFilesAsync()
        {
            var tempFolder =
                Path.Combine(HostingEnvironment.MapPath($"/Temp/TestEnvironment/OutputFiles/{Guid.NewGuid()}"));
            Directory.CreateDirectory(tempFolder);

            var fileNames = await Task.WhenAll(_outputFileRepository.GetAll()
                .Select(x => new { FileName = x.FileId, ResourceName = GetFullResourceName(x.FileId) })
                .Where(x => x.ResourceName != null)
                .Select(async x =>
                {
                    using (var resourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(x.ResourceName))
                    {
                        using (var fileStream = new FileStream(Path.Combine(tempFolder, x.FileName), FileMode.Create))
                        {
                            await resourceStream.CopyToAsync(fileStream).ConfigureAwait(false);
                            await fileStream.FlushAsync().ConfigureAwait(false);
                        }
                    }

                    return x.FileName;
                })).ConfigureAwait(false);

            return new TestEnvironmentOutputFilesInfo()
            {
                LocalFolder = tempFolder,
                FileNames = fileNames
            };
        }

        private void CleanupOutputFiles(TestEnvironmentOutputFilesInfo outputFilesInfo)
        {
            foreach (var fileName in outputFilesInfo.FileNames)
            {
                try
                {
                    File.Delete(Path.Combine(outputFilesInfo.LocalFolder, fileName));
                }
                catch (DirectoryNotFoundException) { }
                catch (IOException) { }
                catch (UnauthorizedAccessException) { }
            }

            try
            {
                Directory.Delete(outputFilesInfo.LocalFolder);
            }
            catch (DirectoryNotFoundException) { }
            catch (IOException) { }
            catch (UnauthorizedAccessException) { }
        }

        private string[] ManifestResourceNames => _manifestResourceNames ??
                                                  (_manifestResourceNames = Assembly.GetExecutingAssembly()
                                                      .GetManifestResourceNames());

        private string GetFullResourceName(string name) => ManifestResourceNames
                    .FirstOrDefault(x => x.EndsWith(name, true, CultureInfo.InvariantCulture));

        private async Task<byte[]> CreateLogFilesZip(IEnumerable<string> logFiles)
        {
            byte[] content;
            using (var contentStream = new MemoryStream())
            using (var zip = new ZipArchive(contentStream, ZipArchiveMode.Create))
            {
                foreach (var logFile in logFiles)
                {
                    using (var fileStream = new FileStream(logFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    using (var entryStream = zip.CreateEntry(Path.GetFileName(logFile), CompressionLevel.Optimal).Open())
                    {
                        await fileStream.CopyToAsync(entryStream).ConfigureAwait(false);
                    }
                }

                content = contentStream.ToArray();
            }

            return content;
        }

        private void RemoveLogFiles(IEnumerable<string> logFiles)
        {
            foreach (var logFile in logFiles)
            {
                try
                {
                    File.Delete(logFile);
                }
                catch (IOException)
                {
                }
            }
        }
    }
}
