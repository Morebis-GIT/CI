using System;
using System.Collections.Generic;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.ScenarioCampaignResults.Objects;
using xggameplan.AuditEvents;
using xggameplan.AutoBooks;
using xggameplan.core.OutputProcessors;
using xggameplan.CSVImporter;
using xggameplan.OutputProcessors.Abstractions;
using xggameplan.Repository;
using xggameplan.Repository.CSV;

namespace xggameplan.OutputProcessors.Processors
{
    public class ScenarioCampaignLevelResultsFileProcessor : IOutputFileProcessor<ScenarioCampaignLevelResult>
    {
        private readonly IAuditEventRepository _audit;
        private readonly IMapper _mapper;

        public ScenarioCampaignLevelResultsFileProcessor(IAuditEventRepository auditEventRepository, IMapper mapper)
        {
            _audit = auditEventRepository;
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public string FileName { get; } = OutputFileNames.CampaignLevelRequirementsSummary;

        public ScenarioCampaignLevelResult ProcessFile(Guid scenarioId, string folder)
        {
            string pathToFile = FileHelpers.GetPathToFileIfExists(folder, FileName);

            var result = new ScenarioCampaignLevelResult { Id = scenarioId };

            if (string.IsNullOrEmpty(pathToFile))
            {
                _audit.Insert(AuditEventFactory.CreateAuditEventForWarningMessage(0, 0, $"File {pathToFile} was not found."));

                return result;
            }

            _audit.Insert(AuditEventFactory.CreateAuditEventForInformationMessage(0,
                0, $"Processing output file {pathToFile}"));

            var importSettings = CSVImportSettings.GetImportSettings(pathToFile, typeof(ScenarioCampaignLevelResultHeaderMap), typeof(ScenarioCampaignLevelResultIndexMap));
            IScenarioCampaignLevelResultsImportRepository repository = new CSVScenarioCampaignLevelResultsImportRepository(importSettings);

            var data = repository.GetAll();

            result.Items = _mapper.Map<List<ScenarioCampaignLevelResultItem>>(data);

            _audit.Insert(AuditEventFactory.CreateAuditEventForInformationMessage(0,
                0, $"Processed output file {pathToFile}"));

            return result;
        }

        object IOutputFileProcessor.ProcessFile(Guid scenarioId, string folder) => ProcessFile(scenarioId, folder);
    }
}
