using System;
using System.Collections.Generic;
using System.IO;
using ImagineCommunications.GamePlan.Domain;
using ImagineCommunications.GamePlan.Domain.ScenarioResults;
using ImagineCommunications.GamePlan.Domain.ScenarioResults.Objects;
using xggameplan.OutputProcessors.Abstractions;
using xggameplan.CSVImporter;
using xggameplan.OutputFiles.Conversion;
using xggameplan.Repository.CSV;

namespace xggameplan.OutputFiles.Processing
{
    public class LmkKpiOutputFileProcessor : ILandmarkOutputFileProcessor
    {
        private static readonly List<MetricDescriptor<LmkKpiImport>> Metrics = new List<MetricDescriptor<LmkKpiImport>>
        {
            new MetricDescriptor<LmkKpiImport>(ScenarioKPINames.RatingCampaignsRatedSpots, "largenumber", i => i.RatingCampaignsRatedSpots),
            new MetricDescriptor<LmkKpiImport>(ScenarioKPINames.SpotCampaignsRatedSpots, "largenumber", i => i.SpotCampaignsRatedSpots),
            new MetricDescriptor<LmkKpiImport>(ScenarioKPINames.TotalZeroRatedSpots, "largenumber", i => i.ZeroRatedSpotsBooked),
            new MetricDescriptor<LmkKpiImport>(ScenarioKPINames.BaseDemographicRatings, "largenumber", i => i.BaseRatingsAchieved),
            new MetricDescriptor<LmkKpiImport>(ScenarioKPINames.TotalRatingCampaignSpots, "largenumber", i => i.RatingCampaignsExistingSpots),
            new MetricDescriptor<LmkKpiImport>(ScenarioKPINames.TotalSpotCampaignSpots, "largenumber", i => i.SpotCampaignsExistingSpots),
            new MetricDescriptor<LmkKpiImport>(ScenarioKPINames.TotalValueDelivered, "largenumber", i => i.NominalValueDelivered),
            new MetricDescriptor<LmkKpiImport>(ScenarioKPINames.TotalNominalValue, "largenumber", i => i.TotalNominalValue),
            new MetricDescriptor<LmkKpiImport>(ScenarioKPINames.DifferenceValue, "largenumber", i => i.PlusMinusValueDelivered),
            new MetricDescriptor<LmkKpiImport>(ScenarioKPINames.DifferenceValuePercentage, "largenumber", i => i.PercentageValueDelivered),
            new MetricDescriptor<LmkKpiImport>(ScenarioKPINames.DifferenceValueWithPayback, "largenumber", i => i.PlusMinusValueDeliveredIncludingPayback),
            new MetricDescriptor<LmkKpiImport>(ScenarioKPINames.DifferenceValuePercentagePayback, "largenumber", i => i.PercentageValueDeliveredIncludingPayback),
            new MetricDescriptor<LmkKpiImport>(ScenarioKPINames.TotalRevenue, "largenumber", i => i.TotalCampaignRevenue),
            new MetricDescriptor<LmkKpiImport>(ScenarioKPINames.TotalPayback, "largenumber", i => i.TotalCampaignPayback),
            new MetricDescriptor<LmkKpiImport>(ScenarioKPINames.TotalZeroRatedSpots, "largenumber", i => i.ZeroRatedSpotsBooked)
        };

        private readonly IOutputFileConverter _outputFileConverter;
        private readonly KPISource _resultSource;

        public LmkKpiOutputFileProcessor(IOutputFileConverter outputFileConverter, KPISource resultSource)
        {
            _outputFileConverter = outputFileConverter;
            _resultSource = resultSource;
        }

        public LmkKpiOutputFileProcessor(KPISource resultSource)
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

            var kpiRepository = new CSVLmkKpiImportRepository(CSVImportSettings.GetImportSettings(file, typeof(LmkKpiHeaderMap), typeof(LmkKpiIndexMap)));
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
