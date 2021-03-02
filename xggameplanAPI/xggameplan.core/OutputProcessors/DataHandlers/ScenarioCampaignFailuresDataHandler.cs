using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Campaigns;
using ImagineCommunications.GamePlan.Domain.Generic.Repository;
using ImagineCommunications.GamePlan.Domain.Generic.Types;
using ImagineCommunications.GamePlan.Domain.Runs.Objects;
using ImagineCommunications.GamePlan.Domain.ScenarioCampaignFailures;
using ImagineCommunications.GamePlan.Domain.ScenarioCampaignFailures.Objects;
using ImagineCommunications.GamePlan.Domain.Scenarios.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.FunctionalAreas;
using ImagineCommunications.GamePlan.Domain.Shared.System.Tenants;
using xggameplan.AuditEvents;
using xggameplan.AutoBooks.AWS;
using xggameplan.cloudaccess.Business;
using xggameplan.core.FeatureManagement.Interfaces;
using xggameplan.core.ReportGenerators.ScenarioCampaignFailures;
using xggameplan.Model;
using xggameplan.OutputFiles.Processing;
using xggameplan.OutputProcessors.Abstractions;

namespace xggameplan.core.OutputProcessors.DataHandlers
{
    public class ScenarioCampaignFailuresDataHandler : IOutputDataHandler<ScenarioCampaignFailureOutput>
    {
        private readonly AWSSettings _awsSettings;
        private readonly ICloudStorage _cloudStorage;
        private readonly IRepositoryFactory _repositoryFactory;
        private readonly IAuditEventRepository _auditEventRepository;
        private readonly IFeatureManager _featureManager;
        private readonly bool _saveData;

        public ScenarioCampaignFailuresDataHandler(AWSSettings awsSettings,
            ICloudStorage cloudStorage,
            IRepositoryFactory repositoryFactory,
            IFeatureManager featureManager,
            IAuditEventRepository auditEventRepository)
        {
            _awsSettings = awsSettings;
            _cloudStorage = cloudStorage;
            _repositoryFactory = repositoryFactory;
            _auditEventRepository = auditEventRepository;

            _saveData = featureManager.IsEnabled(nameof(ProductFeature.PrePostCampaignResults));
        }

        public void ProcessData(ScenarioCampaignFailureOutput data, Run run, Scenario scenario)
        {
            if (!_saveData)
            {
                PreGenerateReport(data.Data, run, scenario);
                return;
            }

            try
            {
                _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForInformationMessage(0,
                    0, $"Start insert of campaign failures. ScenarioId: {data.ScenarioId} Count: {data.Data.Count}"));
                using (var internalScope = _repositoryFactory.BeginRepositoryScope())
                {
                    var scenarioCampaignFailureRepository = internalScope.CreateRepository<IScenarioCampaignFailureRepository>();
                    scenarioCampaignFailureRepository.AddRange(data.Data, false);
                    scenarioCampaignFailureRepository.SaveChanges();
                }
                _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForInformationMessage(0,
                    0, $"Complete insert of campaign failures. ScenarioId: {data.ScenarioId} Count: {data.Data.Count}"));
            }
            catch (Exception e)
            {
                _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForException(0, 0, $"Error while processing scenario campaign failures. ScenarioId: {scenario.Id}", e));
                throw;
            }

            PreGenerateReport(data.Data, run, scenario);
        }

        private void PreGenerateReport(IReadOnlyCollection<ScenarioCampaignFailure> data, Run run, Scenario scenario)
        {
            _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForInformationMessage(0,
                0, $"Start scenario campaign failures report generation. ScenarioId: {scenario.Id} Count: {data.Count}"));
            try
            {
                var reportService = new ScenarioCampaignFailuresReportCreator();
                var reportData = new ScenarioCampaignFailuresDataSnapshot();
                var fileName = ScenarioCampaignFailuresReportCreator.GetFilePath(scenario.Name, run.ExecuteStartedDateTime.Value, scenario.Id);

                var failureTypes = data.Select(c => c.FailureType)
                    .Distinct()
                    .ToList();

                DayOfWeek tenantStartDayOfWeek;
                using (var internalScope = _repositoryFactory.BeginRepositoryScope())
                {
                    reportData.Campaigns = internalScope.CreateRepository<ICampaignRepository>().GetAllFlat();
                    reportData.FaultTypes = internalScope.CreateRepository<IFunctionalAreaRepository>().FindFaultTypes(failureTypes);
                    tenantStartDayOfWeek = internalScope.CreateRepository<ITenantSettingsRepository>().GetStartDayOfWeek();
                }

                using (var reportStream = reportService.GenerateReport(data, reportData, tenantStartDayOfWeek))
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
                        $"Error while generating report for scenario campaign failures. ScenarioId: {scenario.Id}", e
                    )
                );
                throw;
            }
            _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForInformationMessage(0,
                0, $"End scenario campaign failures report generation. ScenarioId: {scenario.Id}"));
        }
    }
}
