using System.Collections.Generic;
using System.IO;
using CsvHelper;
using xggameplan.CSVImporter;

namespace xggameplan.Repository.CSV
{
    /// <summary>
    /// Repository for FailureRequirement instances, from CSV file
    /// </summary>
    public class CSVFailureImportRepository : IFailureImportRepository
    {
        private CSVImportSettings _settings;
        public CSVFailureImportRepository(CSVImportSettings settings)
        {
            _settings = settings;
        }    

        public IEnumerable<FailureImportSummary> GetAll()
        {
            var data = new Dictionary<string, FailureImportSummary>();

            using (var reader = new CsvReader(new StreamReader(_settings.FileName)))
            {
                reader.Configuration.HasHeaderRecord = _settings.IsCSV;

                if (_settings.IsCSV)
                {
                    reader.Configuration.RegisterClassMap(_settings.MapType); //if header - add mapping configuration 
                    reader.Read();          //skip over the header - needed to avoid CsvHelper.ReaderException : 'No header record was found'
                    reader.ReadHeader();    //needed to avoid CsvHelper.Header.ValidationException : 'Header with name 'aper_no' ....' 
                }
                
                while (reader.Read())
                {
                    var item = reader.GetRecord<FailureImport>();

                    string key = $"{item.CampaignNumber}-{item.FailureType}-{item.SalesAreaNumberOfBooking}";

                    if (!data.TryGetValue(key, out var failure)) 
                    {
                        failure = new FailureImportSummary()
                        {
                            Campaign = item.CampaignNumber,
                            Type = item.FailureType,
                            SalesAreaNumberOfBooking = item.SalesAreaNumberOfBooking
                        };

                        data.Add(key, failure);
                    }

                    failure.Failures += item.NumberOfFailures;
                }                
            }

            return data.Values; 
        }
    }
}
