using System;
using System.IO;

namespace xggameplan.CSVImporter
{
    public class CSVImportSettings
    {
        public Type MapType { get; set; }
        public string FileName { get; set; }
        public bool IsCSV { get; set; }

        public static CSVImportSettings GetImportSettings(string fileName, Type headerMapType, Type indexMapType)
        {
            var settings = new CSVImportSettings
            {
                FileName = fileName
            };

            var fileFormat = Path.GetExtension(fileName).ToLower();

            switch (fileFormat)
            {
                case ".csv":
                    settings.IsCSV = true;
                    break;
                case ".out":
                    settings.IsCSV = false;
                    break;
                default:
                    throw new ArgumentException($"{fileFormat} is not valid output file format.");
            }

            settings.MapType = settings.IsCSV ? headerMapType : indexMapType;

            return settings;
        }
    }
}
