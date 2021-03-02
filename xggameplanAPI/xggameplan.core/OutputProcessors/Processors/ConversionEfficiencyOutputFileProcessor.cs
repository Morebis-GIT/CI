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
    public class ConversionEfficiencyOutputFileProcessor : IOutputFileProcessor<ConversionEfficiencyOutput>
    {
        private readonly IAuditEventRepository _audit;
        private readonly IMapper _mapper;

        public ConversionEfficiencyOutputFileProcessor(IMapper mapper, IAuditEventRepository audit)
        {
            _audit = audit;
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public string FileName { get; } = OutputFileNames.ConversionEfficiency;

        public ConversionEfficiencyOutput ProcessFile(Guid scenarioId, string folder)
        {
            var filePath = FileHelpers.GetPathToFileIfExists(folder, FileName);
            return new ConversionEfficiencyOutput
            {
                Data = Process(filePath)
            };
        }

        object IOutputFileProcessor.ProcessFile(Guid scenarioId, string folder) => ProcessFile(scenarioId, folder);

        public IEnumerable<ConversionEfficiency> Process(string pathToFile)
        {
            if (!File.Exists(pathToFile))
            {
                _audit?.Insert(AuditEventFactory.CreateAuditEventForWarningMessage(0, 0, "File was not found."));

                return Enumerable.Empty<ConversionEfficiency>();
            }

            _audit?.Insert(AuditEventFactory.CreateAuditEventForInformationMessage(0,
                0, $"Processing output file {pathToFile}"));

            try
            {
                var importSettings = CSVImportSettings.GetImportSettings(pathToFile, typeof(ConversionEfficiencyHeaderMap), typeof(ConversionEfficiencyIndexMap));

                var conversionEfficiency = new CSVConversionEfficiencyImportRepository(importSettings);

                var data = conversionEfficiency.GetAll();

                var output = _mapper.Map<IEnumerable<ConversionEfficiency>>(data);

                _audit?.Insert(AuditEventFactory.CreateAuditEventForInformationMessage(0,
                    0, $"Processed output file {pathToFile}"));

                return output;
            }
            catch (Exception exception)
            {
                _audit?.Insert(AuditEventFactory.CreateAuditEventForException(0, 0, $"Error processing file {pathToFile}", exception));

                return Enumerable.Empty<ConversionEfficiency>();
            }
        }
    }
}
