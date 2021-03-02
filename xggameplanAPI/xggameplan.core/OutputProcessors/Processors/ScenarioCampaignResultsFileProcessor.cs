using System;
using System.Globalization;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Generic.Helpers;
using ImagineCommunications.GamePlan.Domain.Generic.Types;
using ImagineCommunications.GamePlan.Domain.ScenarioCampaignResults.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreas;
using xggameplan.AuditEvents;
using xggameplan.AutoBooks;
using xggameplan.core.FeatureManagement.Attributes;
using xggameplan.CSVImporter;
using xggameplan.OutputProcessors.Abstractions;
using xggameplan.Repository;
using xggameplan.Repository.CSV;

namespace xggameplan.core.OutputProcessors.Processors
{
    [FeatureFilter(nameof(ProductFeature.ScenarioCampaignResultsProcessing))]
    public class ScenarioCampaignResultsFileProcessor : IOutputFileProcessor<ScenarioCampaignResult>
    {
        private readonly IOutputDataSnapshot _dataSnapshot;
        private readonly IAuditEventRepository _audit;

        public ScenarioCampaignResultsFileProcessor(IOutputDataSnapshot dataSnapshot, IAuditEventRepository auditEventRepository)
        {
            _dataSnapshot = dataSnapshot;
            _audit = auditEventRepository;
        }

        public string FileName { get; } = OutputFileNames.CampaignRequirementsSummary;

        public ScenarioCampaignResult ProcessFile(Guid scenarioId, string folder)
        {
            string pathToFile = FileHelpers.GetPathToFileIfExists(folder, FileName);
            var result = new ScenarioCampaignResult { Id = scenarioId };

            if (string.IsNullOrEmpty(pathToFile))
            {
                _audit.Insert(AuditEventFactory.CreateAuditEventForWarningMessage(0, 0, $"File {pathToFile} was not found."));

                return result;
            }

            _audit.Insert(AuditEventFactory.CreateAuditEventForInformationMessage(0,
                0, $"Processing output file {pathToFile}"));

            var importSettings = CSVImportSettings.GetImportSettings(pathToFile, typeof(ScenarioCampaignResultHeaderMap), typeof(ScenarioCampaignResultIndexMap));
            IScenarioCampaignResultsImportRepository repository = new CSVScenarioCampaignResultsImportRepository(importSettings);

            var data = repository.GetAll();

            var salesAreaIndex = _dataSnapshot.AllSalesAreas.Value.ToDictionary(c => c.CustomId);

            foreach (var importItem in data)
            {
                salesAreaIndex.TryGetValue(importItem.SalesAreaNo, out var salesArea);

                result.Items.Add(MapImportItem(importItem, salesArea));
            }

            _audit.Insert(AuditEventFactory.CreateAuditEventForInformationMessage(0,
                0, $"Processed output file {pathToFile}"));

            return result;
        }

        object IOutputFileProcessor.ProcessFile(Guid scenarioId, string folder) => ProcessFile(scenarioId, folder);

        private static DateTime GetDate(long value)
        {
            return DateHelper.GetDate(value.ToString(CultureInfo.InvariantCulture), "yyyyMMdd", DateTimeKind.Utc);
        }

        private static ScenarioCampaignResultItem MapImportItem(ScenarioCampaignResultImport importItem, SalesArea salesArea) =>
            new ScenarioCampaignResultItem
            {
                CampaignExternalId = importItem.CampaignExternalId,
                SalesAreaName = (salesArea == null) ? "Unknown" : salesArea.Name,
                SpotLength = importItem.SpotLength,
                StrikeWeightStartDate = GetDate(importItem.StrikeWeightStartDate),
                StrikeWeightEndDate = GetDate(importItem.StrikeWeightEndDate),
                DaypartName = importItem.DaypartName,
                TargetRatings = importItem.TargetRatings,
                PreRunRatings = importItem.PreRunRatings,
                ISRCancelledRatings = importItem.ISRCancelledRatings,
                ISRCancelledSpots = importItem.ISRCancelledSpots,
                RSCancelledRatings = importItem.RSCancelledRatings,
                RSCancelledSpots = importItem.RSCancelledSpots,
                OptimiserRatings = importItem.OptimiserRatings,
                OptimiserBookedSpots = importItem.OptimiserBookedSpots,
                ZeroRatedSpots = importItem.ZeroRatedSpots,
                NominalValue = importItem.NominalValue,
                PassThatDelivered100Percent = importItem.PassThatDelivered100Percent
            };
    }
}
