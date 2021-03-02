using System;
using System.Collections.Generic;
using AutoMapper;
using xggameplan.AuditEvents;
using xggameplan.AutoBooks;
using xggameplan.CSVImporter;
using xggameplan.OutputFiles.Processing;
using xggameplan.OutputProcessors.Abstractions;
using xggameplan.Repository.CSV;

namespace xggameplan.core.OutputProcessors.Processors
{
    public class BaseRatingsOutputFileProcessor : IOutputFileProcessor<BaseRatingsOutput>
    {
        private readonly IAuditEventRepository _audit;
        private readonly IMapper _mapper;

        public BaseRatingsOutputFileProcessor(IAuditEventRepository audit, IMapper mapper)
        {
            _audit = audit ?? throw new ArgumentNullException(nameof(audit));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public string FileName { get; } = OutputFileNames.BaseRatings;

        public BaseRatingsOutput ProcessFile(Guid scenarioId, string folder)
        {
            string pathToFile = FileHelpers.GetPathToFileIfExists(folder, FileName);
            var result = new BaseRatingsOutput();

            if (String.IsNullOrEmpty(pathToFile))
            {
                _audit.Insert(AuditEventFactory.CreateAuditEventForWarningMessage(0, 0, $"File {pathToFile} was not found."));

                return result;
            }

            _audit.Insert(AuditEventFactory.CreateAuditEventForInformationMessage(0,
                0, $"Processing output file {pathToFile}"));

            try
            {
                var importSettings = CSVImportSettings.GetImportSettings(pathToFile, typeof(BaseRatingsHeaderMap), typeof(BaseRatingsIndexMap));

                var baseRatingsImportRepository = new CSVBaseRatingsImportRepository(importSettings);

                var data = baseRatingsImportRepository.GetAll();

                result.Data = _mapper.Map<IEnumerable<BaseRatings>>(data);

                _audit.Insert(AuditEventFactory.CreateAuditEventForInformationMessage(0,
                    0, $"Processed output file {pathToFile}"));

                return result;
            }
            catch (Exception exception)
            {
                _audit.Insert(AuditEventFactory.CreateAuditEventForException(0, 0, "Error processing base ratings file", exception));

                return result;
            }
        }

        object IOutputFileProcessor.ProcessFile(Guid scenarioId, string folder) => ProcessFile(scenarioId, folder);
    }
}
