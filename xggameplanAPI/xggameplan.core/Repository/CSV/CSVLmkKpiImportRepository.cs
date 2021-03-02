using System.IO;
using CsvHelper;
using xggameplan.CSVImporter;

namespace xggameplan.Repository.CSV
{
    public class CSVLmkKpiImportRepository
    {
        private readonly CSVImportSettings _settings;

        public CSVLmkKpiImportRepository(CSVImportSettings settings)
        {
            _settings = settings;
        }

        public LmkKpiImport Get()
        {
            using (var reader = new CsvReader(new StreamReader(_settings.FileName)))
            {
                reader.Configuration.HasHeaderRecord = _settings.IsCSV;
                reader.Configuration.RegisterClassMap(_settings.MapType);

                reader.Read();
                var importResult = reader.GetRecord<LmkKpiImport>();

                return importResult;
            }
        }
    }
}
