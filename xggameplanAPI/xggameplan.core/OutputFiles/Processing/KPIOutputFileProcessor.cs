using System;
using System.Collections.Generic;
using System.IO;
using ImagineCommunications.GamePlan.Domain;
using ImagineCommunications.GamePlan.Domain.ScenarioResults.Objects;
using xggameplan.OutputProcessors.Abstractions;
using xggameplan.CSVImporter;
using xggameplan.OutputFiles.Conversion;
using xggameplan.Repository;
using xggameplan.Repository.CSV;

namespace xggameplan.OutputFiles.Processing
{
    public sealed class KPIOutputFileProcessor : ILandmarkOutputFileProcessor
    {
        private static readonly List<MetricDescriptor<KPIImport>> Metrics = new List<MetricDescriptor<KPIImport>>
        {
            new MetricDescriptor<KPIImport>("percentbelow75",            "largenumber", i =>
                i.CampaignCompletionLessThen5 + i.CampaignCompletionFrom5To10 + i.CampaignCompletionFrom10To15 + i.CampaignCompletionFrom15To20 +
                i.CampaignCompletionFrom20To25 + i.CampaignCompletionFrom25To30 + i.CampaignCompletionFrom30To35 + i.CampaignCompletionFrom35To40 +
                i.CampaignCompletionFrom40To45 + i.CampaignCompletionFrom45To50 + i.CampaignCompletionFrom50To55 + i.CampaignCompletionFrom55To60 +
                i.CampaignCompletionFrom60To65 + i.CampaignCompletionFrom65To70 + i.CampaignCompletionFrom70To75),
            new MetricDescriptor<KPIImport>("percent75to95",            "largenumber", i =>
                i.CampaignCompletionFrom75To80 + i.CampaignCompletionFrom80To85 + i.CampaignCompletionFrom85To90 + i.CampaignCompletionFrom90To95),
            new MetricDescriptor<KPIImport>("percent95to105",            "largenumber", i => i.CampaignCompletionFrom95To100 + i.CampaignCompletionFrom100To105),
            new MetricDescriptor<KPIImport>("percentgreater105",         "largenumber", i =>
                i.CampaignCompletionFrom105To110 + i.CampaignCompletionFrom110To115 + i.CampaignCompletionOver115),
            new MetricDescriptor<KPIImport>("averageEfficiency",         "largenumber", i => i.AverageEfficiency),
            new MetricDescriptor<KPIImport>("totalSpotsBooked",          "largenumber", i => i.TotalSpotsBooked),
            new MetricDescriptor<KPIImport>("remainaudience",            "largenumber", i => i.RemainingAudience),
            new MetricDescriptor<KPIImport>("remainingAvailability",     "largenumber", i => i.RemainingAvailability),
            new MetricDescriptor<KPIImport>("standardAverageCompletion", "percentage",  i => i.StandardAverageCompletion),
            new MetricDescriptor<KPIImport>("weightedAverageCompletion", "percentage",  i => i.WeightedAverageCompletion)
        };

        private readonly IOutputFileConverter _outputFileConverter;
        private readonly KPISource _resultSource;

        public KPIOutputFileProcessor(IOutputFileConverter outputFileConverter, KPISource resultSource)
        {
            _outputFileConverter = outputFileConverter;
            _resultSource = resultSource;
        }

        public KPIOutputFileProcessor(KPISource resultSource)
            : this(null, resultSource)
        {
            _resultSource = resultSource;
        }

        public void ProcessFile(string file, ScenarioResult scenarioResult)
        {
            if (!File.Exists(file))
            {
                return;
            }

            if (_outputFileConverter != null)
            {
                string convertedFile = $"{file}.tmp";
                _outputFileConverter.Convert(file, convertedFile);
                File.Delete(file);
                File.Move(convertedFile, file);
            }

            ICSVKPIImportRepository kpiRepository = new CSVKPIImportRepository(CSVImportSettings.GetImportSettings(file, typeof(KPIHeaderMap), typeof(KPIIndexMap)));
            var metrics = kpiRepository.Get();

            var result = new List<KPI>();
            foreach (var metric in Metrics)
            {
                result.Add(new KPI
                {
                    Name = metric.Name,
                    Displayformat = metric.DisplayFormat,
                    Value = metric.Value(metrics),
                    ResultSource = _resultSource
                });
            }

            if (_resultSource == KPISource.Gameplan)
            {
                scenarioResult.Metrics.AddRange(result);
            }
            else if (_resultSource == KPISource.Landmark)
            {
                scenarioResult.LandmarkMetrics.AddRange(result);
            }
            else
            {
                throw new InvalidOperationException($"Unknown result source {_resultSource}");
            }
        }
    }
}
