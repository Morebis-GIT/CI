using xggameplan.CSVImporter;

namespace xggameplan.Repository.CSV
{
    public class CSVSpotReqmImportRepository : CSVImportRepositoryBase<SpotReqmImport>, ISpotReqmImportRepository
    {
        //private string _file;

        public CSVSpotReqmImportRepository(CSVImportSettings importSettings)
        {
            _settings = importSettings;
        }

        //public IEnumerable<SpotReqmImport> GetAll()
        //{
        //    List<SpotReqmImport> spotImports = new List<SpotReqmImport>();

        //    //XGG162 Disabled
        //    using (var reader = new CsvReader(new StreamReader(_file)))
        //    {
        //        reader.Configuration.HasHeaderRecord = false;
        //        reader.Configuration.RegisterClassMap(typeof(SpotReqmMap));
        //        while (reader.Read())
        //        {
        //            // Read                    
        //            var item = reader.GetRecord<SpotReqmImport>();
        //            spotImports.Add(item);
        //        }
        //    }
        //    return spotImports;
        //}
    }
}
