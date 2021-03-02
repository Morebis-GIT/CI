using System;
using System.Collections.Generic;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.Generic.Types;
using ImagineCommunications.GamePlan.Domain.Optimizer.Facilities;
using ImagineCommunications.GamePlan.Domain.Runs.Objects;
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
    public class ScenarioCampaignLevelResultsDataHandler : IOutputDataHandler<ScenarioCampaignLevelResult>
    {
        private readonly IScenarioCampaignResultReportCreator _campaignResultReportCreator;
        private readonly AWSSettings _awsSettings;
        private readonly ICloudStorage _cloudStorage;
        private readonly IAuditEventRepository _auditEventRepository;
        private readonly IMapper _mapper;
        private readonly IFacilityRepository _facilityRepository;

        private readonly bool _includeScenarioPerformanceMeasurementKpIs;
        private readonly bool _saveData;

        public ScenarioCampaignLevelResultsDataHandler(AWSSettings awsSettings, ICloudStorage cloudStorage,
            IScenarioCampaignResultReportCreator campaignResultReportCreator, IFeatureManager featureManager,
            IAuditEventRepository auditEventRepository, IMapper mapper, IFacilityRepository facilityRepository)
        {
            _campaignResultReportCreator = campaignResultReportCreator;
            _awsSettings = awsSettings;
            _cloudStorage = cloudStorage;
            _auditEventRepository = auditEventRepository;
            _mapper = mapper;
            _facilityRepository = facilityRepository;

            _includeScenarioPerformanceMeasurementKpIs = featureManager.IsEnabled(nameof(ProductFeature.ScenarioPerformanceMeasurementKPIs));
            _saveData = featureManager.IsEnabled(nameof(ProductFeature.PrePostCampaignResults));
        }

        public void ProcessData(ScenarioCampaignLevelResult data, Run run, Scenario scenario)
        {
            const string campaignLevelReportFacility = "XGSCRC";

            var facility = _facilityRepository.GetByCode(campaignLevelReportFacility);

            if (!_saveData && facility != null && facility.Enabled)
            {
                PreGenerateReport(data.Items, run, scenario);
            }
        }

        private void PreGenerateReport(List<ScenarioCampaignLevelResultItem> data, Run run, Scenario scenario)
        {
            _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForInformationMessage(0,
                0, $"Start scenario campaign level results report generation. ScenarioId: {scenario.Id} Count: {data.Count}"));
            try
            {
                _campaignResultReportCreator.EnablePerformanceKPIColumns(_includeScenarioPerformanceMeasurementKpIs);
                _campaignResultReportCreator.EnableCampaignLevel(!_saveData);
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
                        $"Error while generating report for scenario campaign level results. ScenarioId: {scenario.Id}", e
                    )
                );
                throw;
            }
            _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForInformationMessage(0,
                0, $"End scenario campaign level results report generation. ScenarioId: {scenario.Id}"));
        }
    }
}
