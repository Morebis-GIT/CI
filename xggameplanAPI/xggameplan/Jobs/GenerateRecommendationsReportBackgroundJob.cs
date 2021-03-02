using System;
using System.IO;
using System.Linq;
using System.Runtime.Caching;
using System.Threading;
using System.Threading.Tasks;
using ImagineCommunications.GamePlan.Domain.Recommendations;
using ImagineCommunications.GamePlan.Domain.Runs;
using ImagineCommunications.GamePlan.Domain.ScenarioResults.Objects;
using ImagineCommunications.GamePlan.Domain.Scenarios;
using xggameplan.AuditEvents;
using xggameplan.AutoBooks.AWS;
using xggameplan.cloudaccess.Factory;
using xggameplan.common.BackgroundJobs;
using xggameplan.core.ReportGenerators.Interfaces;
using xggameplan.Hubs;
using xggameplan.model.Internal;
using xggameplan.Model;

namespace xggameplan.Jobs
{
    public class GenerateRecommendationsReportBackgroundJob : IBackgroundJob
    {
        private readonly AWSSettings _awsSettings;
        private readonly IAuditEventRepository _auditEventRepository;
        private readonly MemoryCache _cache;
        private readonly ReportExportNotificationHub _exportStatusNotifier;
        private readonly IRecommendationRepository _recommendationRepository;
        private readonly IRecommendationsResultReportCreator _recommendationsResultReportCreator;
        private readonly IRunRepository _runRepository;
        private readonly IScenarioRepository _scenarioRepository;
        private readonly IFactory _storageClientFactory;

        private ReportExportStatus status;

        public GenerateRecommendationsReportBackgroundJob(
            AWSSettings awsSettings,
            IAuditEventRepository auditEventRepository,
            MemoryCache cache,
            ReportExportNotificationHub exportStatusNotifier,
            IRecommendationRepository recommendationRepository,
            IRecommendationsResultReportCreator recommendationsResultReportCreator,
            IRunRepository runRepository,
            IScenarioRepository scenarioRepository,
            IFactory storageClientFactory)
        {
            _awsSettings = awsSettings;
            _cache = cache;
            _exportStatusNotifier = exportStatusNotifier;
            _recommendationRepository = recommendationRepository;
            _recommendationsResultReportCreator = recommendationsResultReportCreator;
            _runRepository = runRepository;
            _scenarioRepository = scenarioRepository;
            _storageClientFactory = storageClientFactory;
        }

        public async Task Execute(CancellationToken cancellationToken,
            RecommendationsReportGenerationJobParameters input)
        {
            var recommendations = _recommendationRepository
                .GetByScenarioId(input.ScenarioId)
                .ToArray();

            if (!recommendations.Any())
            {
                status = ReportExportStatus.GenerationFailed;
                _cache.Set(input.ReportReference, status, CreatePreCompletionCachePolicy());

                var message = $"No recommendations are available for the specified scenario ID. ({input.ScenarioId})";

                _exportStatusNotifier.NotifyGroup(new ReportExportStatusNotification
                {
                    reportReference = input.ReportReference,
                    status = status,
                    message = message
                });

                LogError(new ArgumentException(message), message);

                return;
            }

            Stream reportStream = null;

            try
            {
                status = ReportExportStatus.Generating;
                _cache.Set(input.ReportReference, status, CreatePreCompletionCachePolicy());

                _exportStatusNotifier.NotifyGroup(new ReportExportStatusNotification
                {
                    reportReference = input.ReportReference,
                    status = status
                });

                reportStream = _recommendationsResultReportCreator.GenerateReport(recommendations);
            }
            catch (Exception exception)
            {
                status = ReportExportStatus.GenerationFailed;
                _cache.Set(input.ReportReference, status, CreatePreCompletionCachePolicy());

                _exportStatusNotifier.NotifyGroup(new ReportExportStatusNotification
                {
                    reportReference = input.ReportReference,
                    status = status
                });

                LogError(exception, $"Something went wrong while generating a recommendation report for scenario with ID '{input.ScenarioId}'.");

                reportStream.Dispose();

                return;
            }

            using (reportStream)
            {
                try
                {
                    status = ReportExportStatus.Uploading;
                    _cache.Set(input.ReportReference, status, CreatePreCompletionCachePolicy());

                    _exportStatusNotifier.NotifyGroup(new ReportExportStatusNotification
                    {
                        reportReference = input.ReportReference,
                        status = status
                    });

                    var engine = _storageClientFactory.GetEngine();

                    engine.Upload(new S3UploadComment()
                    {
                        BucketName = _awsSettings.S3Bucket,
                        DestinationFilePath = input.FilePath,
                        FileStream = reportStream
                    });
                }
                catch (Exception exception)
                {
                    status = ReportExportStatus.UploadFailed;
                    _cache.Set(input.ReportReference, status, CreatePreCompletionCachePolicy());

                    _exportStatusNotifier.NotifyGroup(new ReportExportStatusNotification
                    {
                        reportReference = input.ReportReference,
                        status = status
                    });

                    LogError(exception, $"Something went wrong while uploading the recommendation report for scenario with ID '{input.ScenarioId}'.");

                    return;
                }
            }

            status = ReportExportStatus.Available;
            _cache.Set(input.ReportReference, status, new CacheItemPolicy { AbsoluteExpiration = DateTime.Now.AddMinutes(5) });

            _exportStatusNotifier.NotifyGroup(new ReportExportStatusNotification
            {
                reportReference = input.ReportReference,
                status = status
            });
        }

        // Expecting each step to take significantly less than a day
        private CacheItemPolicy CreatePreCompletionCachePolicy() => new CacheItemPolicy { AbsoluteExpiration = DateTime.Now.AddDays(1) };

        private void LogError(Exception exception, string message)
        {
            var auditEvent = AuditEventFactory.CreateAuditEventForException(0, 0, message, exception);
            _auditEventRepository.Insert(auditEvent);
        }
    }
}
