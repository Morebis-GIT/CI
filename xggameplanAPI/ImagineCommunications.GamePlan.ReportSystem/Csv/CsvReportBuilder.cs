using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ImagineCommunications.GamePlan.ReportSystem.Csv.CsvReportConfiguration.EntityConfig;
using ImagineCommunications.GamePlan.ReportSystem.ReportConfiguration.EntityConfig;

namespace ImagineCommunications.GamePlan.ReportSystem.Csv
{
    public class CsvReportBuilder: ICsvReportBuilder
    {
        protected List<KeyValuePair<ICsvReportConfiguration, IEnumerable>> DataList { get; set; } = new List<KeyValuePair<ICsvReportConfiguration, IEnumerable>>();
        protected string CsvDelimiter { get; set; } = ",";
        protected string[] CsvComments { get; set; }

        public ICsvReportBuilder Comments(params string[] comments)
        {
            CsvComments = comments;
            return this;
        }

        public ICsvReportBuilder DataContent<T>(IEnumerable<T> data, ICsvReportConfiguration configuration) where T : class
        {
            DataList.Add(new KeyValuePair<ICsvReportConfiguration, IEnumerable>(configuration, data));
            return this;
        }

        public ICsvReportBuilder DataContent<T>(IEnumerable<T> data) where T : class
        {
            var config = CreateDefaultConfiguration<T>();
            return DataContent(data, config);
        }

        public ICsvReportBuilder Delimiter(string delimiter)
        {
            if (string.IsNullOrWhiteSpace(delimiter))
            {
                throw new ArgumentNullException(nameof(delimiter));
            }

            CsvDelimiter = delimiter;
            return this;
        }

        public void SaveAs(TextWriter writer)
        {
            InnerWrite(writer);
        }

        public string GetAsString()
        {
            using (var stream = new MemoryStream())
            using (TextWriter writer = new StreamWriter(stream))
            {
                InnerWrite(writer);
                return Encoding.UTF8.GetString(stream.ToArray());
            }
        }

        public byte[] GetAsByteArray()
        {
            using (var stream = new MemoryStream())
            using (TextWriter writer = new StreamWriter(stream))
            {
                InnerWrite(writer);
                return stream.ToArray();
            }
        }

        private void InnerWrite(TextWriter writer)
        {
            var dataWriter = new CsvDataWriter(writer)
            {
                Delimiter = CsvDelimiter
            };

            dataWriter.WriteComment(CsvComments);
            foreach (var data in DataList)
            {
                dataWriter.Write(data.Value, data.Key);
            }
        }

        protected virtual ICsvReportConfiguration CreateDefaultConfiguration<T>()
            where T : class
        {
            var configurationBuilder = new CsvConfigurationBuilder<T>(new ConfigurationOptions());
            return configurationBuilder.BuildConfiguration();
        }
    }
}
