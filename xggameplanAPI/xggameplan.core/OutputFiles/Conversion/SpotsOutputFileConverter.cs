using System;
using System.IO;
using CsvHelper;
using xggameplan.CSVImporter;

namespace xggameplan.OutputFiles.Conversion
{
    /// <summary>
    /// Converts Spots output file.
    /// 
    /// As a temporary action then the data is modified to map the data in the test output files to local data.
    /// E.g. Local Campaign IDs etc.
    /// </summary>
    public class SpotsOutputFileConverter : IOutputFileConverter
    {
        private ConversionMappings _conversionMappings = new ConversionMappings();

        public SpotsOutputFileConverter(ConversionMappings conversionMappings)
        {
            _conversionMappings = conversionMappings;
        }

        public void Convert(string autoBookFile, string localFile)
        {
            bool converted = false;
            try
            {
                // Delete old file if exists
                if (File.Exists(localFile))
                {
                    File.Delete(localFile);
                }                

                // Convert AutoBook file to local file
                using (var reader = new CsvReader(new StreamReader(autoBookFile)))
                {
                    using (var fileStream = File.OpenWrite(localFile))
                    {
                        using (var writer = new CsvWriter(new StreamWriter(fileStream, reader.Configuration.Encoding)))
                        {
                            writer.Configuration.RegisterClassMap(typeof(SpotIndexMap));
                            reader.Configuration.RegisterClassMap(typeof(SpotIndexMap));
                            while (reader.Read())
                            {
                                var item = reader.GetRecord<SpotImport>();

                                ApplyMappings(item);

                                writer.WriteRecord<SpotImport>(item);
                            }
                        }
                    }
                    converted = true;
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                
                // Clean up if not converted
                if (!converted && File.Exists(localFile))
                {
                    File.Delete(localFile);
                }
            }
        }
        
        /// <summary>
        /// Applies mappings to record
        /// </summary>
        /// <param name="spotImport"></param>
        private void ApplyMappings(SpotImport spotImport)
        {
            // Update Campaign No            
            ConversionMapping<Int32> campaignMapping = _conversionMappings.Campaigns.Find(x => x.OldValue == spotImport.camp_no);
            if (campaignMapping != null)
            {
                spotImport.camp_no = campaignMapping.NewValue;
            }

            // Updates SalesArea
            ConversionMapping<Int32> salesAreaMapping = _conversionMappings.SalesAreas.Find(x => x.OldValue == spotImport.sare_no);
            if (campaignMapping != null)
            {
                spotImport.sare_no = salesAreaMapping.NewValue;
            }

            // Update SpotExternalRef
            ConversionMapping<Int64> slotExternalRefMapping = _conversionMappings.SpotExternalRefs.Find(x => x.OldValue == spotImport.spot_no);
            if (slotExternalRefMapping != null)
            {
                spotImport.spot_no = slotExternalRefMapping.NewValue;
            }
        }
    }
}
