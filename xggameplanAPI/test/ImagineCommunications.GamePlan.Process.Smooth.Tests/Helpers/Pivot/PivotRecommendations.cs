using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Breaks.Objects;
using ImagineCommunications.GamePlan.Domain.Recommendations.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.Programmes.Objects;

namespace ImagineCommunications.GamePlan.Process.Smooth.Tests.Helpers.Pivot
{
    public class PivotRecommendations
    {
        public Dictionary<string, List<string>> BreaksVsProductIds { get; }
        public Dictionary<string, int> ProductIdCountInBreak { get; }

        public List<PivotSalesArea> SalesAreas { get; }

        public PivotRecommendations(
            IReadOnlyCollection<Recommendation> recommendations,
            IReadOnlyDictionary<string, Break> allBreaks,
            IReadOnlyDictionary<string, Programme> allProgrammes)
        {
            BreaksVsProductIds = new Dictionary<string, List<string>>();
            ProductIdCountInBreak = new Dictionary<string, int>();
            SalesAreas = new List<PivotSalesArea>();

            var salesAreas = recommendations
                .GroupBy(r => r.SalesArea)
                .Select(g => new
                {
                    SalesAreaName = g.Key,
                    ProductIds = GetProductIds(g)
                });

            foreach (var salesAreaItem in salesAreas)
            {
                string salesAreaName = salesAreaItem.SalesAreaName;

                var salesArea = new PivotSalesArea(
                    salesAreaName,
                    salesAreaItem.ProductIds);

                var programmes = recommendations
                    .Where(r => r.SalesArea == salesAreaName && r.ExternalProgrammeReference != null)
                    .GroupBy(r => r.ExternalProgrammeReference)
                    .Select(g => new
                    {
                        ExternalProgrammeReference = g.Key,
                        ProductIds = GetProductIds(g)
                    });

                foreach (var programmeItem in programmes)
                {
                    string externalProgrammeReference = programmeItem.ExternalProgrammeReference;

                    var pivotProgramme = new PivotProgramme(
                        allProgrammes,
                        externalProgrammeReference,
                        programmeItem.ProductIds);

                    var breaks = recommendations
                        .Where(r =>
                        {
                            return r.SalesArea == salesAreaName
                                && r.ExternalProgrammeReference == externalProgrammeReference;
                        })
                        .GroupBy(r => r.ExternalBreakNo)
                        .Select(g => new
                        {
                            ExternalBreakNo = g.Key,
                            ProductIds = GetProductIds(g)
                        });

                    foreach (var br in breaks)
                    {
                        string externalBreakNo = br.ExternalBreakNo;

                        var pivotBreak = new PivotBreak(
                            allBreaks,
                            externalBreakNo,
                            br.ProductIds);

                        if (!BreaksVsProductIds.ContainsKey(externalBreakNo))
                        {
                            BreaksVsProductIds.Add(
                                externalBreakNo,
                                new List<string>(br.ProductIds)
                                );
                        }

                        var recommendationsForEachBreak = recommendations
                            .Where(r => r.ExternalBreakNo == externalBreakNo);

                        foreach (var recommendation in recommendationsForEachBreak)
                        {
                            var productIdCountInBreakKey = externalBreakNo + "_" + recommendation.Product;
                            if (ProductIdCountInBreak.ContainsKey(productIdCountInBreakKey))
                            {
                                ProductIdCountInBreak[productIdCountInBreakKey]++;
                            }
                            else
                            {
                                ProductIdCountInBreak.Add(productIdCountInBreakKey, 1);
                            }
                            var pivotSpot = new PivotSpot(recommendation);
                            pivotBreak.Spots.Add(pivotSpot);
                        }

                        pivotProgramme.Breaks.Add(pivotBreak);
                    }

                    salesArea.Programmes.Add(pivotProgramme);
                }

                SalesAreas.Add(salesArea);
            }
        }

        public PivotSalesArea SalesArea(string salesAreaName) =>
            SalesAreas.Find(s => String.Equals(s.Name, salesAreaName));

        public IEnumerable<string> SalesAreaByNames() => SalesAreas.Select(s => s.Name);

        private static string[] GetProductIds(IGrouping<string, Recommendation> g) =>
            g.Select(x => x.Product)
            .Distinct()
            .OrderBy(i => i)
            .ToArray();

        public int GetCountOfProductInABreak(string br, IEnumerable<string> productsIds)
        {
            int count = 0;

            foreach (string productExternalId in productsIds)
            {
                if (ProductIdCountInBreak.TryGetValue($"{br}_{productExternalId}", out int value))
                {
                    count += value;
                }
            }

            return count;
        }
    }
}
