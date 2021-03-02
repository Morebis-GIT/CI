using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Generic.Helpers;
using ImagineCommunications.GamePlan.Domain.ScenarioCampaignFailures.Objects;
using NodaTime;
using xggameplan.AuditEvents;
using xggameplan.AutoBooks;
using xggameplan.core.Exceptions;
using xggameplan.CSVImporter;
using xggameplan.OutputFiles.Processing;
using xggameplan.OutputProcessors.Abstractions;
using xggameplan.Repository.CSV;

namespace xggameplan.core.OutputProcessors.Processors
{
    public class ScenarioCampaignFailureOutputFileProcessor : IOutputFileProcessor<ScenarioCampaignFailureOutput>
    {
        private readonly IOutputDataSnapshot _dataSnapshot;
        private readonly IAuditEventRepository _auditEventRepository;

        public ScenarioCampaignFailureOutputFileProcessor(IOutputDataSnapshot dataSnapshot, IAuditEventRepository auditEventRepository)
        {
            _dataSnapshot = dataSnapshot;
            _auditEventRepository = auditEventRepository;
        }

        public string FileName { get; } = OutputFileNames.CampaignFailure;

        public ScenarioCampaignFailureOutput ProcessFile(Guid scenarioId, string folder)
        {
            string filename = FileHelpers.GetPathToFileIfExists(folder, FileName);
            var result = new ScenarioCampaignFailureOutput
            {
                ScenarioId = scenarioId,
                Data = new List<ScenarioCampaignFailure>()
            };

            if (String.IsNullOrEmpty(filename))
            {
                return result;
            }

            _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForInformationMessage(0,
                0, $"Processing output file {filename}"));

            var importSettings = CSVImportSettings.GetImportSettings(filename, typeof(ScenarioCampaignFailureHeaderMap), typeof(ScenarioCampaignFailureIndexMap));
            using var scenarioCampaignFailureImportRepository = new CSVScenarioCampaignFailureImportRepository(importSettings);

            var fileData = scenarioCampaignFailureImportRepository.GetAll();

            if (fileData is null || !fileData.Any())
            {
                _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForException(0, 0,
                    $"Error generating Scenario Campaign Failure, data not found", new ObjectNotFoundException()));

                return result;
            }

            var salesAreas = _dataSnapshot.AllSalesAreas.Value.ToDictionary(s => s.CustomId);
            var campaigns = _dataSnapshot.AllCampaigns.Value.ToDictionary(c => c.ExternalId);

            foreach (var data in fileData)
            {
                try
                {
                    _ = salesAreas.TryGetValue(data.SalesAreaNumber, out var salesArea);
                    _ = campaigns.TryGetValue(data.CampaignExternalId, out var campaign);
                    var salesAreaName = salesArea?.Name ?? "Unknown";
                    var salesAreaGroupName = campaign?.SalesAreaCampaignTarget
                        .Where(sact => !(sact.SalesAreaGroup is null))
                        .Select(sact => sact.SalesAreaGroup)
                        .FirstOrDefault(sag =>
                        sag.SalesAreas.Any(sa => sa.Equals(salesAreaName, StringComparison.OrdinalIgnoreCase)))
                        ?.GroupName;

                    var scenarioCampaignFailure = new ScenarioCampaignFailure
                    {
                        ScenarioId = scenarioId,
                        ExternalCampaignId = data.CampaignExternalId,
                        SalesArea = salesAreaName,
                        SalesAreaGroup = salesAreaGroupName,
                        Length = Duration.FromSeconds(data.SpotLength),
                        MultipartNo = data.MultipartNumber,
                        StrikeWeightStartDate = DateHelper.GetDate(data.StrikeWeightStartDate.ToString(), "yyyyMMdd"),
                        StrikeWeightEndDate = DateHelper.GetDate(data.StrikeWeightEndDate.ToString(), "yyyyMMdd"),
                        DayPartStartTime = ToTimeSpan(data.DayPartStartTime.ToString().PadLeft(6, '0')),
                        DayPartEndTime = ToTimeSpan(data.DayPartEndTime.ToString().PadLeft(6, '0')),
                        DayPartDays = data.DayPartDays,
                        FailureType = data.FailureType,
                        FailureCount = data.NumberOfFailures,
                        PassesEncounteringFailure = DecryptPassIds(data.PassesEncounteringFailure),
                    };

                    result.Data.Add(scenarioCampaignFailure);
                }
                catch (ObjectNotFoundException exception)
                {
                    _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForException(0, 0,
                        $"Error generating Scenario Campaign Failure for campaign id {data.CampaignExternalId}", exception));
                }
            }

            _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForInformationMessage(0,
                0, $"Processed output file {filename}"));

            return result;
        }

        object IOutputFileProcessor.ProcessFile(Guid scenarioId, string folder) => ProcessFile(scenarioId, folder);

        private static TimeSpan ToTimeSpan(string time) =>
            TimeSpan.TryParseExact(time, "hhmmss", CultureInfo.InvariantCulture, out TimeSpan result)
                ? result
                : new TimeSpan(0, 0, 0);

        private static string DecryptPassIds(long input)
        {
            if (input == 0)
            {
                return string.Empty;
            }

            var power = 0;
            var results = new List<int>();
            while (input > 0)
            {
                var digit = input % 2;
                if (digit == 1)
                {
                    results.Add((int)Math.Pow(2, power));
                }
                input /= 2;
                power++;
            }

            return string.Join(", ", results);
        }
    }
}
