using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.Generic.Repository;
using ImagineCommunications.GamePlan.Domain.Recommendations;
using ImagineCommunications.GamePlan.Domain.Recommendations.Objects;
using ImagineCommunications.GamePlan.Domain.Runs.Objects;
using ImagineCommunications.GamePlan.Domain.Scenarios.Objects;
using xggameplan.AuditEvents;
using xggameplan.AutoBooks.AWS;
using xggameplan.cloudaccess.Business;
using xggameplan.core.ReportGenerators;
using xggameplan.core.ReportGenerators.Interfaces;
using xggameplan.Model;
using xggameplan.OutputFiles.Processing;
using xggameplan.OutputProcessors.Abstractions;

namespace xggameplan.core.OutputProcessors.DataHandlers
{
    public class SpotsReqmOutputDataHandler : IOutputDataHandler<SpotsReqmOutput>
    {
        private readonly AWSSettings _awsSettings;
        private readonly ICloudStorage _cloudStorage;
        private readonly IRepositoryFactory _repositoryFactory;
        private readonly IRecommendationsResultReportCreator _recommendationsReportCreator;
        private readonly IAuditEventRepository _auditEventRepository;

        public SpotsReqmOutputDataHandler(AWSSettings awsSettings,
            ICloudStorage cloudStorage,
            IRepositoryFactory repositoryFactory,
            IRecommendationsResultReportCreator recommendationsReportCreator,
            IAuditEventRepository auditEventRepository)
        {
            _awsSettings = awsSettings;
            _cloudStorage = cloudStorage;
            _repositoryFactory = repositoryFactory;
            _recommendationsReportCreator = recommendationsReportCreator;
            _auditEventRepository = auditEventRepository;
        }

        public void ProcessData(SpotsReqmOutput data, Run run, Scenario scenario)
        {
            try
            {
                _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForInformationMessage(0,
                    0, $"Start insert of recommendations. ScenarioId: {data.ScenarioId} Count: {data.Recommendations.Count}"));

                using (var innerScope = _repositoryFactory.BeginRepositoryScope())
                {
                    var recommendationBatchRepository = innerScope.CreateRepository<IRecommendationRepository>();
                    recommendationBatchRepository.Insert(data.Recommendations, false);
                    recommendationBatchRepository.SaveChanges();
                }

                _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForInformationMessage(0,
                    0, $"End insert of recommendations. ScenarioId: {data.ScenarioId}"));
            }
            catch (Exception e)
            {
                _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForException(0, 0, $"Error while processing scenario recommendations. ScenarioId: {scenario.Id}", e));
                throw;
            }

            // do not generate report for scenario with smooth
            if (run.Smooth)
            {
                return;
            }

            var fileName = ReportFileNameHelper.RecommendationResult(run, scenario);
            PreGenerateReport(data.Recommendations, fileName, scenario.Id);
        }

        private void PreGenerateReport(IReadOnlyCollection<Recommendation> data, string fileName, Guid scenarioId)
        {
            _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForInformationMessage(0,
                0, $"Start recommendations report generation. ScenarioId: {scenarioId} Count: {data.Count}"));
            try
            {
                using (var reportStream = _recommendationsReportCreator.GenerateReport(data))
                {
                    _ = _cloudStorage.Upload(new S3UploadComment
                    {
                        BucketName = _awsSettings.S3Bucket,
                        DestinationFilePath = fileName,
                        FileStream = reportStream
                    });
                }
            }
            catch (Exception e)
            {
                _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForException(0, 0, $"Error while generating report for scenario recommendations. ScenarioId: {scenarioId}", e));
                throw;
            }

            _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForInformationMessage(0,
                0, $"End recommendations report generation. ScenarioId: {scenarioId}"));
        }
    }
}
