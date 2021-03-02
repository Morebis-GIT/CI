using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Caching;
using System.Web.Http;
using System.Web.Http.Description;
using ImagineCommunications.GamePlan.Domain.Recommendations;
using ImagineCommunications.GamePlan.Domain.Runs;
using ImagineCommunications.GamePlan.Domain.ScenarioResults.Objects;
using ImagineCommunications.GamePlan.Domain.Scenarios;
using xggameplan.AutoBooks.AWS;
using xggameplan.cloudaccess.Factory;
using xggameplan.common.BackgroundJobs;
using xggameplan.core.ReportGenerators;
using xggameplan.core.ReportGenerators.Interfaces;
using xggameplan.Errors;
using xggameplan.Jobs;
using xggameplan.model.External;
using xggameplan.model.Internal;
using xggameplan.Model;

namespace xggameplan.Controllers
{
    /// <summary>
    /// The Scenario Results Recommendations controller.
    /// </summary>
    /// <remarks>
    /// Some of the endpoints currently have 2 routes in an effort to standardise route formats.
    /// </remarks>
    /// <seealso cref="ScenarioResultsController">
    /// Other recommendation endpoints are still in the Scenario Results Controller.
    /// </seealso>
    [RoutePrefix("ScenarioResults")]
    public class RecommendationsController : ApiController
    {
        private readonly AWSSettings _awsSettings;
        private readonly IBackgroundJobManager _backgroundJobManager;
        private readonly MemoryCache _cache;
        private readonly IRecommendationRepository _recommendationRepository;
        private readonly IRecommendationsResultReportCreator _reportCreator;
        private readonly IRunRepository _runRepository;
        private readonly IScenarioRepository _scenarioRepository;
        private readonly IFactory _storageClientFactory;

        private const string RunNotFoundMessage = "Run was not found";
        private const string ScenarioNotFoundMessage = "Scenario was not found";

        public RecommendationsController(
            AWSSettings awsSettings,
            IBackgroundJobManager backgroundJobManager,
            MemoryCache cache,
            IRecommendationRepository recommendationRepository,
            IRecommendationsResultReportCreator reportCreator,
            IRunRepository runRepository,
            IScenarioRepository scenarioRepository,
            IFactory storageClientFactory)
        {
            _awsSettings = awsSettings;
            _backgroundJobManager = backgroundJobManager;
            _cache = cache;
            _recommendationRepository = recommendationRepository;
            _reportCreator = reportCreator;
            _runRepository = runRepository;
            _scenarioRepository = scenarioRepository;
            _storageClientFactory = storageClientFactory;
        }

        /// <summary>
        /// Get extended recommendations for a scenario, the smooth
        /// recommendations are not extended.
        /// </summary>
        /// <param name="scenarioId"></param>
        [HttpGet]
        [Route("{scenarioId}/recommendations/extended")]
        [ResponseType(typeof(List<RecommendationExtendedModel>))]
        public IHttpActionResult GetExtendedRecommendation([FromUri] Guid scenarioId)
        {
            var data = _recommendationRepository
                .GetByScenarioId(scenarioId).ToArray();

            if (data.Length == 0)
            {
                return NotFound();
            }

            return Ok(_reportCreator.GenerateReportData(data));
        }

        /// <summary>
        /// Check for the existence of an extended scenario recommendations results export
        /// </summary>
        /// <param name="scenarioId">The GUID of the scenario</param>
        /// <returns>
        /// An object containing a reference to the report for subscribing to updates from the
        /// generation job via SignalR and a boolean indicating whether it already exists or not.
        /// </returns>
        [HttpGet]
        [Route("{scenarioId}/recommendations/extended/export/existence")]
        [ResponseType(typeof(ReportExistenceViewModel))]
        public IHttpActionResult CheckExistenceOfRecommendationsReport([FromUri] Guid scenarioId)
        {
            string filePath;

            try
            {
                filePath = GetRecommendationsReportFilePath(scenarioId);
            }
            catch (ArgumentNullException exception)
            {
                return this.Error().ResourceNotFound(exception.Message);
            }

            var descriptor = new S3FileComment()
            {
                BucketName = _awsSettings.S3Bucket,
                FileNameWithPath = filePath
            };

            var engine = _storageClientFactory.GetEngine();

            var output = new ReportExistenceViewModel
            {
                ReportReference = CreateReportReference(scenarioId),
                Exists = engine.FileExists(descriptor)
            };

            return Ok(output);
        }

        /// <summary>
        /// Enqueue a background job that will generate and upload an extended scenario
        /// recommendations results export, giving updates via SignalR along the way
        /// </summary>
        /// <param name="scenarioId">The GUID of the scenario</param>
        /// <returns>An object containing a reference to the report for subscribing to updates from the
        /// generation job via SignalR and an enum value corresponding to the status.</returns>
        [HttpPost]
        [Route("{scenarioId}/recommendations/extended/export")]
        [ResponseType(typeof(ReportExportStatusNotification))]
        public IHttpActionResult CreateRecommendationsReport([FromUri] Guid scenarioId)
        {
            var reportReference = CreateReportReference(scenarioId);

            var cachedStatus = _cache.GetCacheItem(reportReference);
            var cachePolicy = new CacheItemPolicy { AbsoluteExpiration = DateTime.Now.AddDays(1) };

            // To prevent queuing the background job more than once
            if (!(cachedStatus is null) &&
                cachedStatus.Value is ReportExportStatus status &&
                (status == ReportExportStatus.Requested ||
                status == ReportExportStatus.Queued ||
                status == ReportExportStatus.Generating ||
                status == ReportExportStatus.Uploading ||
                status == ReportExportStatus.Available))
            {
                return Ok(new ReportExportStatusNotification
                {
                    reportReference = reportReference,
                    status = status
                });
            }
            else
            {
                status = ReportExportStatus.Requested;
                _cache.Set(reportReference, status, cachePolicy);
            }

            string filePath;

            try
            {
                filePath = GetRecommendationsReportFilePath(scenarioId);
            }
            catch (ArgumentNullException exception)
            {
                return this.Error().ResourceNotFound(exception.Message);
            }

            var descriptor = new S3FileComment
            {
                BucketName = _awsSettings.S3Bucket,
                FileNameWithPath = filePath
            };

            var engine = _storageClientFactory.GetEngine();

            if (engine.FileExists(descriptor))
            {
                var message = $"A recommendations report already exists for the specified scenario ID. ({scenarioId})";
                return this.Error().BadRequest(message);
            }

            var recommendations = _recommendationRepository
                .GetByScenarioId(scenarioId);

            if (!recommendations.Any())
            {
                var message = $"No recommendations are available for the specified scenario ID. ({scenarioId})";
                return this.Error().BadRequest(message);
            }

            var jobParameters = new RecommendationsReportGenerationJobParameters
            {
                FilePath = filePath,
                ReportReference = reportReference,
                ScenarioId = scenarioId
            };

            status = ReportExportStatus.Queued;
            _cache.Set(reportReference, status, cachePolicy);

            _backgroundJobManager.StartJob<GenerateRecommendationsReportBackgroundJob>(
                new BackgroundJobParameter<RecommendationsReportGenerationJobParameters>(jobParameters));

            return Ok(new ReportExportStatusNotification
            {
                reportReference = reportReference,
                status = status
            });
        }

        /// <summary>
        /// Get a link to download the extended scenario recommendations results export.
        /// </summary>
        /// <param name="scenarioId">The GUID of the scenario</param>
        /// <returns>A url to download the report from</returns>
        [HttpGet]
        [Route("{scenarioId}/recommendations/extended/export/get-link")]
        [Route("{scenarioId}/recommendations/extended/export/link")]
        [ResponseType(typeof(string))]
        public IHttpActionResult GetRecommendationsReportLink([FromUri] Guid scenarioId)
        {
            string filePath;

            try
            {
                filePath = GetRecommendationsReportFilePath(scenarioId);
            }
            catch (ArgumentNullException exception)
            {
                return this.Error().ResourceNotFound(exception.Message);
            }

            var descriptor = new S3FileComment
            {
                BucketName = _awsSettings.S3Bucket,
                FileNameWithPath = filePath
            };

            var engine = _storageClientFactory.GetEngine();

            if (!engine.FileExists(descriptor))
            {
                var message = $"Unable to locate an existing recommendations report for the specified scenario ID. ({scenarioId})";
                return this.Error().ResourceNotFound(message);
            }

            var link = engine.GetPreSignedUrl(descriptor);

            return Content(HttpStatusCode.OK, link);
        }

        private string GetRecommendationsReportFilePath(Guid scenarioId)
        {
            var run = _runRepository.FindByScenarioId(scenarioId);
            if (run is null)
            {
                throw new ArgumentNullException(RunNotFoundMessage);
            }

            var scenario = _scenarioRepository.Get(scenarioId);
            if (scenario is null)
            {
                throw new ArgumentNullException(ScenarioNotFoundMessage);
            }

            var filePath = ReportFileNameHelper.RecommendationResult(run, scenario);

            return filePath;
        }

        private string CreateReportReference(Guid scenarioId) => $"{scenarioId}-extended-recommendations";
    }
}
