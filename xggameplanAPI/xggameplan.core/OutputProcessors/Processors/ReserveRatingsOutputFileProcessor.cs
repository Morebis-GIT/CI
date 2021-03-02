using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoMapper;
using xggameplan.AuditEvents;
using xggameplan.AutoBooks;
using xggameplan.CSVImporter;
using xggameplan.OutputFiles.Processing;
using xggameplan.OutputProcessors.Abstractions;
using xggameplan.Repository.CSV;

namespace xggameplan.core.OutputProcessors.Processors
{
    public class ReserveRatingsOutputFileProcessor : IOutputFileProcessor<ReserveRatingsOutput>
    {
        private readonly IAuditEventRepository _audit;
        private readonly IMapper _mapper;

        public ReserveRatingsOutputFileProcessor(IMapper mapper, IAuditEventRepository audit)
        {
            _audit = audit;
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public string FileName { get; } = OutputFileNames.ReserveRatings;

        public ReserveRatingsOutput ProcessFile(Guid scenarioId, string folder)
        {
            var filePath = FileHelpers.GetPathToFileIfExists(folder, FileName);
            return new ReserveRatingsOutput
            {
                Data = Process(filePath)
            };
        }

        object IOutputFileProcessor.ProcessFile(Guid scenarioId, string folder) => ProcessFile(scenarioId, folder);

        public IEnumerable<ReserveRatings> Process(string pathToFile)
        {
            if (!File.Exists(pathToFile))
            {
                _audit?.Insert(AuditEventFactory.CreateAuditEventForWarningMessage(0, 0, "File was not found."));

                return Enumerable.Empty<ReserveRatings>();
            }

            _audit?.Insert(AuditEventFactory.CreateAuditEventForInformationMessage(0,
                0, $"Processing output file {pathToFile}"));

            try
            {
                var importSettings = CSVImportSettings.GetImportSettings(pathToFile, typeof(ReserveRatingsHeaderMap), typeof(ReserveRatingsIndexMap));

                var baseRatingsImportRepository = new CSVReserveRatingsImportRepository(importSettings);

                var data = baseRatingsImportRepository.GetAll();

                var output = _mapper.Map<IEnumerable<ReserveRatings>>(data);

                _audit?.Insert(AuditEventFactory.CreateAuditEventForInformationMessage(0,
                    0, $"Processed output file {pathToFile}"));

                return output;
            }
            catch (Exception exception)
            {
                _audit?.Insert(AuditEventFactory.CreateAuditEventForException(0, 0, "Error processing reserve ratings file", exception));

                return Enumerable.Empty<ReserveRatings>();
            }
        }
    }
}
