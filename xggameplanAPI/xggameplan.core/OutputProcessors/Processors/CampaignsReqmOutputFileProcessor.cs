using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using xggameplan.AuditEvents;
using xggameplan.AutoBooks;
using xggameplan.CSVImporter;
using xggameplan.OutputFiles.Processing;
using xggameplan.OutputProcessors.Abstractions;
using xggameplan.Repository;
using xggameplan.Repository.CSV;

namespace xggameplan.core.OutputProcessors.Processors
{
    public class CampaignsReqmOutputFileProcessor : IOutputFileProcessor<CampaignsReqmOutput>
    {
        private readonly IAuditEventRepository _audit;
        private readonly IMapper _mapper;

        public CampaignsReqmOutputFileProcessor(IAuditEventRepository audit, IMapper mapper)
        {
            _audit = audit ?? throw new ArgumentNullException(nameof(audit));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public string FileName { get; } = OutputFileNames.CampaignLevel;

        public CampaignsReqmOutput ProcessFile(Guid scenarioId, string folder)
        {
            string pathToFile = FileHelpers.GetPathToFileIfExists(folder, FileName);
            var result = new CampaignsReqmOutput();

            if (String.IsNullOrEmpty(pathToFile))
            {
                _audit.Insert(AuditEventFactory.CreateAuditEventForWarningMessage(0, 0, $"File {pathToFile} was not found."));

                return result;
            }

            _audit.Insert(AuditEventFactory.CreateAuditEventForInformationMessage(0,
                0, $"Processing output file {pathToFile}"));

            var importSettings = CSVImportSettings.GetImportSettings(pathToFile, typeof(CampaignsReqmHeaderMap), typeof(CampaignsReqmIndexMap));

            ICampaignsReqmImportRepository _fileReader = new CSVCampaignsReqmImportRepository(importSettings);

            var data = _fileReader.GetAll().ToList();

            result.Data = _mapper.Map<IEnumerable<CampaignsReqm>>(data);

            _audit.Insert(AuditEventFactory.CreateAuditEventForInformationMessage(0,
                0, $"Processed output file {pathToFile}"));

            return result;
        }

        object IOutputFileProcessor.ProcessFile(Guid scenarioId, string folder) => ProcessFile(scenarioId, folder);
    }
}
