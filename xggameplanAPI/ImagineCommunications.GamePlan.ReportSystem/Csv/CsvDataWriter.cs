using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using ImagineCommunications.GamePlan.ReportSystem.Csv.CsvReportConfiguration.EntityConfig;
using ImagineCommunications.GamePlan.ReportSystem.HeaderHumanizer;
using ImagineCommunications.GamePlan.ReportSystem.TypeAccessor;

namespace ImagineCommunications.GamePlan.ReportSystem.Csv
{
    public class CsvDataWriter
    {
        public string Delimiter { get; set; } = ",";
        protected TextWriter TextWriter { get; set; }
        public CsvDataWriter(TextWriter textWriter)
        {
            TextWriter = textWriter ?? throw new ArgumentNullException(nameof(textWriter));
        }

        public void WriteComment(params string[] comments)
        {
            if (comments == null || comments.Length == 0)
            {
                return;
            }

            var configuration = GetConfiguration();
            using (var writer = new CsvHelper.CsvWriter(TextWriter, configuration))
            {
                foreach (var comment in comments)
                {
                    writer.WriteComment(comment);
                    writer.NextRecord();
                }
            }
        }

        public void Write(IEnumerable source, ICsvReportConfiguration config)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (config is null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            var accessor = new TypeAccessor.TypeAccessor();

            Write(source, config, accessor);
        }

        public void Write<T>(IEnumerable<T> source, ICsvReportConfiguration config)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (config is null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            var accessor = new TypeAccessor<T>();

            Write(source, config, accessor);
        }

        private void Write(IEnumerable source, ICsvReportConfiguration config, TypeAccessor.TypeAccessor accessor)
        {
            var configuration = GetConfiguration();

            using (var writer = new CsvHelper.CsvWriter(TextWriter, configuration))
            {
                WriteHeader(config, writer);

                WriteRecords(source, config, accessor, writer);
            }
        }

        protected virtual CsvHelper.Configuration.Configuration GetConfiguration()
        {
            var configuration = new CsvHelper.Configuration.Configuration
            {
                Delimiter = Delimiter
            };
            return configuration;
        }

        protected virtual void WriteRecords(IEnumerable source, ICsvReportConfiguration config, TypeAccessor.TypeAccessor accessor, CsvHelper.CsvWriter writer)
        {
            foreach (var row in source)
            {
                var memberOptions = config.MemberConfigurations.GetActiveOrderlyOptions();
                foreach (var column in memberOptions)
                {
                    var value = accessor.GetValue(row, column.MemberName);
                    string formattedValue = value.ToString();
                    if (column.Formatter != null)
                    {
                        formattedValue = column.Formatter.Format(value);
                    }
                    else if (column.FormatterExpression != null)
                    {
                        formattedValue = ReportHelper.ReportHelper.CreateFormatFunc(value, column.FormatterExpression)(value);
                    }

                    writer.WriteField(formattedValue);
                }
                writer.NextRecord();
            }
        }

        protected virtual void WriteHeader(ICsvReportConfiguration config, CsvHelper.CsvWriter writer)
        {
            if (config.Options.IsHideHeader)
            {
                return;
            }

            var memberOptions = config.MemberConfigurations.GetActiveOrderlyOptions();
            foreach (var options in memberOptions)
            {
                var header = options.GetHeader(config.Options.UseHeaderHumanizer);

                writer.WriteField(header);
            }

            writer.NextRecord();
        }
    }
}
