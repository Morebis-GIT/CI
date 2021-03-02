using System.IO;
using CsvHelper;
using xggameplan.CSVImporter;

namespace xggameplan.Repository.CSV
{
    public class CSVKPIImportRepository : ICSVKPIImportRepository
    {
        private readonly CSVImportSettings _settings;

        public CSVKPIImportRepository(CSVImportSettings settings)
        {
            _settings = settings;
        }

        public KPIImport Get()
        {
            using (var reader = new CsvReader(new StreamReader(_settings.FileName)))
            {
                reader.Configuration.HasHeaderRecord = _settings.IsCSV;
                reader.Configuration.RegisterClassMap(_settings.MapType);

                reader.Read();
                var importResult = reader.GetRecord<KPIImport>();

                return importResult;
            }
        }
    }
}
