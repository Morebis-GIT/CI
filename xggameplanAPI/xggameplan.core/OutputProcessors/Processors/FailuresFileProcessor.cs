using System;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.ScenarioFailures.Objects;
using xggameplan.AuditEvents;
using xggameplan.AutoBooks;
using xggameplan.CSVImporter;
using xggameplan.OutputProcessors.Abstractions;
using xggameplan.Repository;
using xggameplan.Repository.CSV;

namespace xggameplan.core.OutputProcessors.Processors
{
    public class FailuresFileProcessor : IOutputFileProcessor<Failures>
    {
        private readonly IOutputDataSnapshot _dataSnapshot;
        private readonly IAuditEventRepository _audit;

        public FailuresFileProcessor(IOutputDataSnapshot dataSnapshot, IAuditEventRepository audit)
        {
            _dataSnapshot = dataSnapshot;
            _audit = audit;
        }

        public string FileName { get; } = OutputFileNames.Failures;

        public Failures ProcessFile(Guid scenarioId, string folder)
        {
            string pathToFile = FileHelpers.GetPathToFileIfExists(folder, FileName);
            var failures = new Failures {Id = scenarioId};

            if (string.IsNullOrEmpty(pathToFile))
            {
                _audit.Insert(AuditEventFactory.CreateAuditEventForWarningMessage(0, 0, $"File {pathToFile} was not found."));

                return failures;
            }

            _audit.Insert(AuditEventFactory.CreateAuditEventForInformationMessage(0,
                0, $"Processing output file {pathToFile}"));

            var importSettings = CSVImportSettings.GetImportSettings(pathToFile, typeof(FailureHeaderMap), typeof(FailureIndexMap));

            var campaignsIndex = _dataSnapshot.AllCampaigns.Value.ToDictionary(c => (long)c.CustomId);
            var salesAreaIndex = _dataSnapshot.AllSalesAreas.Value.ToDictionary(c => c.CustomId);

            IFailureImportRepository failureRepository = new CSVFailureImportRepository(importSettings);

            foreach (var summary in failureRepository.GetAll())
            {
                var failure = new Failure();

                if (!campaignsIndex.TryGetValue(summary.Campaign, out var campaign))
                {
                    failure.ExternalId = failure.CampaignName = "Unknown";
                }
                else
                {
                    failure.ExternalId = campaign.ExternalId;
                    failure.CampaignName = campaign.Name;
                }

                failure.Type = summary.Type;
                failure.Failures = summary.Failures;
                failure.Campaign = summary.Campaign;

                failure.SalesAreaName = !salesAreaIndex.TryGetValue(summary.SalesAreaNumberOfBooking, out var salesArea)
                    ? "Unknown"
                    : salesArea.Name;

                failures.Items.Add(failure);
            }

            _audit.Insert(AuditEventFactory.CreateAuditEventForInformationMessage(0,
                0, $"Processed output file {pathToFile}"));

            return failures;
        }

        object IOutputFileProcessor.ProcessFile(Guid scenarioId, string folder) => ProcessFile(scenarioId, folder);
    }
}
