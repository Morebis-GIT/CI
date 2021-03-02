using System;
using System.Collections.Generic;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.Generic.Repository;
using ImagineCommunications.GamePlan.Domain.Generic.Types;
using ImagineCommunications.GamePlan.Domain.Optimizer.Facilities;
using ImagineCommunications.GamePlan.Domain.Runs.Objects;
using ImagineCommunications.GamePlan.Domain.ScenarioCampaignResults;
using ImagineCommunications.GamePlan.Domain.ScenarioCampaignResults.Objects;
using ImagineCommunications.GamePlan.Domain.Scenarios.Objects;
using xggameplan.AuditEvents;
using xggameplan.AutoBooks.AWS;
using xggameplan.cloudaccess.Business;
using xggameplan.core.FeatureManagement.Interfaces;
using xggameplan.core.ReportGenerators;
using xggameplan.core.ReportGenerators.Interfaces;
using xggameplan.core.ReportGenerators.ScenarioCampaignResults;
using xggameplan.Model;
using xggameplan.OutputProcessors.Abstractions;

namespace xggameplan.core.OutputProcessors.DataHandlers
{
    public class ScenarioCampaignResultsDataHandler : IOutputDataHandler<ScenarioCampaignResult>
    {
        private readonly IRepositoryFactory _repositoryFactory;
        private readonly IScenarioCampaignResultReportCreator _campaignResultReportCreator;
        private readonly AWSSettings _awsSettings;
        private readonly ICloudStorage _cloudStorage;
        private readonly IAuditEventRepository _auditEventRepository;
        private readonly IMapper _mapper;
        private readonly IFacilityRepository _facilityRepository;

        private readonly bool _includeScenarioPerformanceMeasurementKpIs;
        private readonly bool _saveData;

        public ScenarioCampaignResultsDataHandler(IRepositoryFactory repositoryFactory, AWSSettings awsSettings,
            ICloudStorage cloudStorage, IScenarioCampaignResultReportCreator campaignResultReportCreator,
            IFeatureManager featureManager, IAuditEventRepository auditEventRepository, IMapper mapper,
            IFacilityRepository facilityRepository)
        {
            _repositoryFactory = repositoryFactory;
            _campaignResultReportCreator = campaignResultReportCreator;
            _awsSettings = awsSettings;
            _cloudStorage = cloudStorage;
            _auditEventRepository = auditEventRepository;
            _mapper = mapper;
            _facilityRepository = facilityRepository;

            _includeScenarioPerformanceMeasurementKpIs = featureManager.IsEnabled(nameof(ProductFeature.ScenarioPerformanceMeasurementKPIs));
            _saveData = featureManager.IsEnabled(nameof(ProductFeature.PrePostCampaignResults));
        }

        public void ProcessData(ScenarioCampaignResult data, Run run, Scenario scenario)
        {
            var scenarioId = data.Id;

            if (!_saveData)
            {
                const string campaignLevelReportFacility = "XGSCRC";

                var facility = _facilityRepository.GetByCode(campaignLevelReportFacility);

                if (facility is null || facility.Enabled == false)
                {
                    PreGenerateReport(data.Items, run, scenario);
                }

                return;
            }

            try
            {
                _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForInformationMessage(0,
                    0, $"Start insert of scenario campaigns. ScenarioId: {scenarioId} Count: {data.Items.Count}"));
                using (var innerScope = _repositoryFactory.BeginRepositoryScope())
                {
                    var scenarioCampaignResultRepository = innerScope.CreateRepository<IScenarioCampaignResultRepository>();
                    scenarioCampaignResultRepository.AddOrUpdate(data);
                    scenarioCampaignResultRepository.SaveChanges();
                }
                _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForInformationMessage(0,
                    0, $"End insert of scenario campaigns. ScenarioId: {scenarioId} Count: {data.Items.Count}"));
            }
            catch (Exception e)
            {
                _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForException(0, 0, $"Error while processing scenario campaign results. ScenarioId: {scenarioId}", e));
                throw;
            }

            PreGenerateReport(data.Items, run, scenario);
        }

        private void PreGenerateReport(List<ScenarioCampaignResultItem> data, Run run, Scenario scenario)
        {
            _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForInformationMessage(0,
                0, $"Start scenario campaign results report generation. ScenarioId: {scenario.Id} Count: {data.Count}"));
            try
            {
                _campaignResultReportCreator.EnablePerformanceKPIColumns(_includeScenarioPerformanceMeasurementKpIs);
                string fileName = ReportFileNameHelper.ScenarioCampaignResult(scenario, run.ExecuteStartedDateTime.Value);

                using (var reportStream = _campaignResultReportCreator.GenerateReport(() =>
                    ScenarioCampaignResultReportHelper.MapToExtendedResults(data, scenario.CampaignPassPriorities, _mapper)))
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
                _auditEventRepository.Insert(
                    AuditEventFactory.CreateAuditEventForException(
                        0,
                        0,
                        $"Error while generating report for scenario campaign results. ScenarioId: {scenario.Id}", e
                    )
                );
                throw;
            }
            _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForInformationMessage(0,
                0, $"End scenario campaign results report generation. ScenarioId: {scenario.Id}"));
        }
    }
}
